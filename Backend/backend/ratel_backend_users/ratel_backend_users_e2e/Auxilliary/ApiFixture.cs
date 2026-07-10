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

using Microsoft.Extensions.DependencyInjection;
using ratel_backend_users_client;
using ratel_backend_users_client.Services.Abstract;

namespace ratel_backend_users_e2e.Auxilliary;

/// <summary>
/// Clients and settings are here
/// </summary>
public sealed class ApiFixture : IAsyncLifetime
{
    /// <summary>
    /// Services provider for DI
    /// </summary>
    private IServiceProvider _servicesProvider { get; set; } = null!;
    
    #region Clients
    
    public IRegistrationClient RegistrationClient => _servicesProvider.GetRequiredService<IRegistrationClient>();
    
    #endregion
    
    public Task InitializeAsync()
    {
        var services = new ServiceCollection();

        services.AddRatelBackendUsersClients
        (
            options
            =>
            {
                options.BaseUrl = Environment.GetEnvironmentVariable("E2E_BASE_URL")
                                  ??
                                  "http://localhost:9000/api/";
                options.Timeout = 120;
            }
        );

        _servicesProvider = services.BuildServiceProvider();

        return Task.CompletedTask;
    }

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