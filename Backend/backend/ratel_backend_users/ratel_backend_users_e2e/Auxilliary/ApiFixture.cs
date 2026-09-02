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
using ratel_backend_users_client;
using ratel_backend_users_client.Services.Abstract;
using ratel_shared_e2e;
using ratel_shared_e2e.Models.Settings;

namespace ratel_backend_users_e2e.Auxilliary;

/// <summary>
/// Clients and settings are here
/// </summary>
public sealed class ApiFixture : ApiFixtureBase
{
    #region Clients

    public IRegistrationClient RegistrationClient => _servicesProvider.GetRequiredService<IRegistrationClient>();

    #endregion

    protected override void ConfigureServices
    (
        IServiceCollection services,
        IConfiguration configuration,
        CommonSettings commonSettings
    )
    {
        services.AddRatelBackendUsersClients
        (
            options
            =>
            {
                options.BaseUrl = Environment.GetEnvironmentVariable("E2E_BASE_URL")
                                  ??
                                  commonSettings.BaseUrl
                                  ??
                                  throw new InvalidOperationException("E2E base URL is not configured");

                options.Timeout = commonSettings.Timeout;
            }
        );
    }
}
