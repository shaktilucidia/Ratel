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
using ratel_backend_users_client.Models.Options;
using ratel_backend_users_client.Services.Abstract;
using ratel_backend_users_client.Services.Implementations;

namespace ratel_backend_users_client;

/// <summary>
/// Use it to set up dependency injection
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Call this to set up users clients DI
    /// </summary>
    public static IServiceCollection AddRatelBackendUsersClients
    (
        this IServiceCollection services,
        Action<ClientsOptions> configureClients
    )
    {
        #region Set up HTTP clients
        
        var clientsOptions = new ClientsOptions()
        {
            BaseUrl = string.Empty,
            Timeout = -1
        };
        
        configureClients(clientsOptions);

        services.AddSingleton(clientsOptions);

        services.AddHttpClient<IRegistrationClient, RegistrationClient>
        (
            client
            =>
            {
                client.BaseAddress = new Uri(clientsOptions.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(clientsOptions.Timeout);
            }
        );
        
        #endregion

        return services;
    }
}