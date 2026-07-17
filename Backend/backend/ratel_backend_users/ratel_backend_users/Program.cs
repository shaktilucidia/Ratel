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
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ratel_backend_users.Constants;
using ratel_backend_users.DAO.Contexts;
using ratel_backend_users.DAO.Models.Creatures;
using ratel_backend_users.Models.Settings;
using ratel_backend_users.Services.Abstract;
using ratel_backend_users.Services.Implementation;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

#region DI

    #region Scoped

    builder.Services.AddScoped<IRegistrationService, RegistrationService>();
    builder.Services.AddScoped<IHealthService, HealthService>();

    #endregion

#endregion

#region Settings

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(nameof(JwtSettings)));
builder.Services.Configure<TempoSettings>(builder.Configuration.GetSection(nameof(TempoSettings)));

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
                options.ConfigureCommandSpanNameProvider(command =>
                {
                    // Span name in traces
                    return $"PostgreSQL {command.CommandText.Split(' ')[0]}";
                });

                options.ConfigureCommandFilter(command =>
                {
                    // Do not trace tiny SELECT 1 queries, they are used for health checks and are not important
                    return !command.CommandText.StartsWith("SELECT 1");
                });
            }
        );

        var mainDataSource = mainDataSourceBuilder.Build();

        builder.Services.AddSingleton(mainDataSource);

        builder.Services.AddDbContext<MainDbContext>
        (
            options
            =>
            options.UseNpgsql(mainDataSource), ServiceLifetime.Transient
        );

    #endregion
#endregion

#region Identity framework

    // Identity
    builder.Services.AddIdentity<CreatureDbo, CreatureRoleDbo>()
        .AddEntityFrameworkStores<MainDbContext>()
        .AddDefaultTokenProviders();

    builder.Services.Configure<IdentityOptions>(options =>
    {
        // Password settings
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequiredUniqueChars = 4;
    });

    var jwtSettings = builder
        .Configuration
        .GetSection(nameof(JwtSettings))
        .Get<JwtSettings>();

    _ = jwtSettings ?? throw new ArgumentNullException(nameof(jwtSettings), "JWT settings aren't specified");

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })

    // Adding Jwt Bearer
    .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = jwtSettings.ValidAudience,
                ValidIssuer = jwtSettings.ValidIssuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
            };
        }
    );

#endregion

#region Swagger

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen
    (
        sgo =>
        {
            sgo.SwaggerDoc("v1", new OpenApiInfo() { Title = "Ratel users API", Version = "v1" });

            sgo.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = @"JWT Authorization token",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            sgo.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("bearer", document)] = []
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
                    .AddPrometheusExporter();
            }
        );

    #endregion

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseCors();
app.UseAuthorization();

app.UseResponseCompression();

app.MapPrometheusScrapingEndpoint();

app.MapControllers();

#region  Apply migrations mode

if (args.Contains(Commandline.ApplyMigrationsArgName))
{
    using var scope = app.Services.CreateScope();

    var db = scope.ServiceProvider.GetRequiredService<MainDbContext>();

    await db.Database.MigrateAsync();

    return;
}

#endregion

app.Run();
