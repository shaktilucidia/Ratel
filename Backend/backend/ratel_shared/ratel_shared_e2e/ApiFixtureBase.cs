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


using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ratel_shared_e2e.Models.Settings;

namespace ratel_shared_e2e;

/// <summary>
/// Clients and settings are here
/// </summary>
public abstract class ApiFixtureBase : IAsyncLifetime
{
    /// <summary>
    /// Services provider for DI
    /// </summary>
    protected IServiceProvider _servicesProvider { get; set; } = null!;

    public Task InitializeAsync()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var services = new ServiceCollection();

        services.Configure<CommonSettings>(configuration.GetSection(nameof(CommonSettings)));

        var commonSettings = configuration
        .GetSection(nameof(CommonSettings))
        .Get<CommonSettings>();

        _ = commonSettings ?? throw new ArgumentNullException(nameof(commonSettings), "Common E2E settings aren't specified");

        ConfigureServices
        (
            services,
            configuration,
            commonSettings
        );

        _servicesProvider = services.BuildServiceProvider();

        return Task.CompletedTask;
    }

    protected abstract void ConfigureServices
    (
        IServiceCollection services,
        IConfiguration configuration,
        CommonSettings commonSettings
    );

    public async Task DisposeAsync()
    {
        if (_servicesProvider is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }
        else if (_servicesProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
        else
        {
            throw new InvalidOperationException($"Unsupported service provider type: { _servicesProvider.GetType().FullName }");
        }
    }
}
