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
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

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

#region Authentication

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
                ValidAudience = "http://127.0.0.1:9000",  
                ValidIssuer = "http://127.0.0.1:9000",  
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("qdTvZEUUzKwf0o3ncp0pl7k7Zp1ZxXGSRBXGqjFbYESyZ9tTjgI7AAcwKMDh9ZE9"))  
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

app.MapControllers();

app.Run();