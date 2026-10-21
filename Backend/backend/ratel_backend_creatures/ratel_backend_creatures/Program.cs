// Ratel - Opensource federated messenger
// Copyright (C) 2026 Shakti Lucidia
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System.IO.Compression;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ratel_backend_creatures.Constants;
using ratel_backend_creatures.CustomImplementations;
using ratel_backend_creatures.DAO.Contexts;
using ratel_backend_creatures.DAO.Models.Creatures;
using ratel_backend_creatures.DAO.Services.Abstract;
using ratel_backend_creatures.DAO.Services.Implementations;
using ratel_backend_creatures.DAO.Services.Implementations.Sessions;
using ratel_backend_creatures.Models.Business.Sessions;
using ratel_backend_creatures.Models.Settings;
using ratel_backend_creatures.Services.Abstract;
using ratel_backend_creatures.Services.Implementation;
using ratel_shared_auxiliary.Helpers;
using ratel_shared_auxiliary.Models.Settings;
using ratel_shared_auxiliary.Services.Abstract;
using ratel_shared_auxiliary.UoW.Abstract;
using ratel_shared_auxiliary.UoW.Implementations;
using Serilog;

#region Detect run mode

    var isApplyMigrations = args.Contains(CommandLine.ApplyMigrationsArgName);
    var isInitCreatures = args.Contains(CommandLine.InitCreaturesArgName);
    var isUpdateCreatures = args.Contains(CommandLine.UpdateCreaturesArgName);

    var selectedModesCount =
        (isApplyMigrations ? 1 : 0)
        +
        (isInitCreatures ? 1 : 0)
        +
        (isUpdateCreatures ? 1 : 0);

    if (selectedModesCount > 1)
    {
        throw new ArgumentException
        (
            "Only one of apply_migrations, init_creatures, or update_creatures may be specified"
        );
    }

    var cleanedFromModeArgs = args
        .Where
        (
            arg
            =>
            arg is not CommandLine.ApplyMigrationsArgName
            and not CommandLine.InitCreaturesArgName
            and not CommandLine.UpdateCreaturesArgName
        )
        .ToArray();

#endregion

var builder = WebApplication.CreateBuilder(cleanedFromModeArgs);

#region DI

    #region Scoped

        #region DAOs

            builder.Services.AddScoped<IRolesDao, RolesDao>();
            builder.Services.AddScoped<ISessionsDao, SessionsDao>();
        
        #endregion

        #region Custom implementations

            builder.Services.AddScoped<AccessTokensValidationEvents>();

        #endregion
    
        #region Services
    
            builder.Services.AddScoped<IRegistrationService, RegistrationService>();
            builder.Services.AddScoped<IHealthService, HealthService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork<MainDbContext>>();
            builder.Services.AddScoped<ICreaturesAndRolesInitializer, CreaturesAndRolesInitializer>();
            builder.Services.AddScoped<IRolesService, RolesService>();
            builder.Services.AddScoped<ICreaturesService, CreaturesService>();
            builder.Services.AddScoped<ISessionsService, SessionsService>();
    
        #endregion
    
    #endregion

    #region Singletions

        builder.Services.AddSingleton<ITimeProvider, ratel_shared_auxiliary.Services.Implementations.TimeProvider>();
        builder.Services.AddSingleton<IAccessTokensService, AccessTokensService>();
        builder.Services.AddSingleton<AccessTokensSigningKey>();
    
    #endregion

#endregion

#region Settings
    
    DiHelper.RegisterSettings<TempoSettings>(builder.Services, builder.Configuration);
    DiHelper.RegisterSettings<AdministratorAccountSettings>(builder.Services, builder.Configuration);
    DiHelper.RegisterSettings<AccessTokensSettings>(builder.Services, builder.Configuration);
    DiHelper.RegisterSettings<ReverseProxiesSettings>(builder.Services, builder.Configuration);

#endregion

builder.Services.AddControllers();

#region Compression
    builder.Services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true; // SECURITY RISK AHEAD! PageRead this fist before enabling: https://learn.microsoft.com/en-us/aspnet/core/performance/response-compression?view=aspnetcore-6.0
        options.Providers.Add<BrotliCompressionProvider>(); // Brotli is widespread
        options.Providers.Add<GzipCompressionProvider>(); // GZIP as fallback
    });

    builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
    {
        options.Level = CompressionLevel.SmallestSize;
    });

    builder.Services.Configure<GzipCompressionProviderOptions>(options =>
    {
        options.Level = CompressionLevel.SmallestSize;
    });
#endregion
    
#region Reverse proxies
    
    builder
        .Services
        .AddOptions<ForwardedHeadersOptions>()
        .Configure<ReverseProxiesSettings>
        (
            (options, settings)
            =>
            {
                ArgumentNullException.ThrowIfNull(settings.TrustedPodsNetworks);

                if (settings.TrustedPodsNetworks.Length is 0)
                {
                    throw new InvalidOperationException("At least one trusted pod network must be configured");
                }

                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                
                options.ForwardLimit = 1; // One proxy between clients and backend: Envoy.

                options.KnownProxies.Clear();
                options.KnownIPNetworks.Clear();

                settings
                    .TrustedPodsNetworks
                    .Select(System.Net.IPNetwork.Parse)
                    .ToList()
                    .ForEach(options.KnownIPNetworks.Add);
            }
        );
    
#endregion

#region CORS
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy
        (
            policy =>
            {
                policy
                    .WithOrigins
                    (
                        "http://localhost:8080"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            }
        );
    });
#endregion

#region  DB Contexts

    #region Main

        var mainDataSourceBuilder = new NpgsqlDataSourceBuilder
        (
            builder.Configuration.GetConnectionString("MainConnection")
            ??
            throw new InvalidOperationException("Main connection string is not set")
        );

        mainDataSourceBuilder.ConfigureTracing
        (
            options
            =>
            {
                // Span name in traces
                options.ConfigureCommandSpanNameProvider(command => $"PostgreSQL {command.CommandText.Split(' ')[0]}");

                // Do not trace tiny SELECT 1 queries, they are used for health checks and are not important
                options.ConfigureCommandFilter(command => !command.CommandText.StartsWith("SELECT 1"));
            }
        );

        var mainDataSource = mainDataSourceBuilder.Build();

        builder.Services.AddSingleton(mainDataSource);

        builder.Services.AddDbContext<MainDbContext>
        (
            options => options.UseNpgsql(mainDataSource)
        );

    #endregion
    
#endregion
    
#region Identity framework

    builder.Services
        .AddIdentity<CreatureDbo, CreatureRoleDbo>()
        .AddEntityFrameworkStores<MainDbContext>()
        .AddDefaultTokenProviders();

    builder.Services.Configure<IdentityOptions>
    (
        options
        =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireDigit = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequiredUniqueChars = 4;
        }
    );

#endregion
    
#region Access token authentication

    builder
    .Services
    .AddAuthentication
    (
        options
        =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
        }
    )
    .AddJwtBearer();

    builder
    .Services
    .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<AccessTokensSigningKey>
    (
        (options, signingKey)
        =>
        {
            options.MapInboundClaims = false; // Preserve claim names: sub, sid, jti
            
            options.SaveToken = false; // Do not retain the raw token in authentication properties
            
            options.IncludeErrorDetails = false; // Do not expose validation error details to the client

            options.TokenValidationParameters = new TokenValidationParameters
            {
                RequireSignedTokens = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey.Credentials.Key,

                ValidAlgorithms =
                [
                    SecurityAlgorithms.RsaSha256
                ],

                ValidateIssuer = true,
                ValidIssuer = Sessions.AccessTokenIssuer,

                ValidateAudience = true,
                RequireAudience = true,
                ValidAudience = ratel_shared_auxiliary.Constants.Sessions.AccessTokenAudience,

                ValidateLifetime = true,
                RequireExpirationTime = true,
                ClockSkew = TimeSpan.FromSeconds(Sessions.AccessTokenAllowedTimeSkew),

                NameClaimType = "sub"
            };

            options.EventsType = typeof(AccessTokensValidationEvents);
        }
    );

    builder
    .Services
    .AddAuthorization
    (
        options
        =>
        {
            var requireAuthenticatedCreature = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            options.DefaultPolicy = requireAuthenticatedCreature;
            options.FallbackPolicy = requireAuthenticatedCreature;
        }
    );    
    
#endregion
    
#region Swagger

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen
    (
        sgo =>
        {
            sgo.SwaggerDoc("v1", new OpenApiInfo() { Title = "Ratel creatures API", Version = "v1" });

            sgo.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "JWT Authorization header using the Bearer scheme",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header
            });

            sgo.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });
        }
    );

#endregion

#region Logging

    #region Serilog

        builder
            .Host
            .UseSerilog
            (
                (context, configuration)
                =>
                configuration
                    .ReadFrom
                    .Configuration(context.Configuration)
                    .Enrich
                    .WithProperty("Service", Microservice.Name)
                    .Enrich
                    .WithProperty("Version", typeof(Program).Assembly.GetName().Version!.ToString())
                    .Enrich
                    .WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
            );

    #endregion

    #region OpenTelemetry

    var tempoSettings = builder
        .Configuration
        .GetSection(nameof(TempoSettings))
        .Get<TempoSettings>();

    _ = tempoSettings ?? throw new ArgumentNullException(nameof(tempoSettings), "Tempo settings aren't specified");

    builder
        .Services
        .AddOpenTelemetry()
        .WithTracing
        (
            tracing =>
            {
                tracing
                    .SetResourceBuilder
                    (
                        ResourceBuilder
                            .CreateDefault()
                            .AddService(Microservice.Name)
                    )
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()

                    .AddSource("Npgsql")

                    .AddOtlpExporter
                    (
                        options =>
                        {
                            options.Endpoint = new Uri(tempoSettings.Uri);
                        }
                    );
            }
        )
        .WithMetrics
        (
            metrics =>
            {
                metrics
                    .AddMeter("Microsoft.EntityFrameworkCore")
                    .AddAspNetCoreInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddProcessInstrumentation()
                    .AddPrometheusExporter()
                    .AddMeter(Metrics.Registration);
            }
        );

    #endregion

#endregion

var app = builder.Build();

#region Run selected mode

if (isApplyMigrations || isInitCreatures || isUpdateCreatures)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

    #region Apply migrations mode

    if (isApplyMigrations)
    {
        await db.Database.MigrateAsync();
    }

    #endregion
    
    #region Init creatures and roles

    if (isInitCreatures)
    {
        var creaturesAndRolesInitializer = scope.ServiceProvider.GetRequiredService<ICreaturesAndRolesInitializer>();

        await creaturesAndRolesInitializer.InitAsync();
    }
    
    #endregion
    
    #region Update creatures and roles

    if (isUpdateCreatures)
    {
        throw new NotImplementedException("Not implemented yet");
    }
    
    #endregion

    return 0;
}

#endregion
    
_ = app.Services.GetRequiredService<AccessTokensSigningKey>(); // Check access tokens signing private key load
    
#region Middlewares
    
    app.UseForwardedHeaders();
        
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger
        (
            o =>
            {
                o
                    .PreSerializeFilters
                    .Add
                    (
                        (document, _) =>
                        {
                            var paths = new OpenApiPaths();

                            foreach (var path in document.Paths)
                            {
                                paths.Add
                                (
                                    $"{ Microservice.UrlPrefix }{ path.Key }",
                                    path.Value
                                );
                            }

                            document.Paths = paths;
                        }
                    );
            }
        );
        
        app.UseSwaggerUI();
    }

    app.UseAuthentication();
    app.UseCors();
    app.UseAuthorization();

    app.UseResponseCompression();

    app.MapPrometheusScrapingEndpoint().AllowAnonymous();

    app
        .MapGroup(Microservice.UrlPrefix)
        .MapControllers();
    
#endregion

app.Run();

return 0;
