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

using System.Net.Http.Json;
using System.Text.Json;
using ratel_backend_users_client.Services.Abstract;
using ratel_backend_users_dtos.Registration.DTOs;
using ratel_backend_users_dtos.Registration.Enums;
using ratel_backend_users_dtos.Registration.Extensions;
using ratel_backend_users_dtos.Registration.Requests;
using ratel_backend_users_dtos.Registration.Responses;

namespace ratel_backend_users_client.Services.Implementations;

public class RegistrationClient
(
    HttpClient httpClient
) : IRegistrationClient
{
    public async Task<bool> IsLoginAvailableAsync
    (
        string login,
        CancellationToken cancellationToken
    )
    {
        using var response = await httpClient
            .PostAsJsonAsync
            (
                $"creatures/registration/is_login_available",
                new IsLoginAvailableRequest()
                {
                    LoginData = new IsLoginAvailableRequestDto()
                    {
                        Login = login
                    }
                },
                cancellationToken
            )
            .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        return JsonSerializer
            .Deserialize<IsLoginAvailableResponse>(await response.Content.ReadAsStringAsync(cancellationToken))
            !.AvailabilityData
            .IsAvailable;
    }

    public async Task<IReadOnlySet<RegistrationError>> RegisterAsync
    (
        string login,
        string password,
        CancellationToken cancellationToken
    )
    {
        using var response = await httpClient
            .PostAsJsonAsync
            (
                $"creatures/registration/register",
                new CreatureRegistrationRequest()
                {
                    RegistrationData = new CreatureRegistrationDataDto()
                    {
                        Login = login,
                        Password = password
                    }
                },
                cancellationToken
            )
            .ConfigureAwait(false);

        // Non-successful status codes are handled in ToRegistrationResultAsync()
        return await response.ToRegistrationResultAsync(cancellationToken);
    }
}
