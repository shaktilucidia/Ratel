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
using ratel_backend_users_dtos.Registration.Requests;
using ratel_backend_users_dtos.Registration.Responses;
using ratel_backend_users_dtos.Roles.Requests;
using ratel_backend_users_dtos.Roles.Responses;

namespace ratel_backend_users_client.Services.Implementations;

public class CreaturesClient
(
    HttpClient httpClient
) : ICreaturesClient
{
    public async Task<IReadOnlyDictionary<string, Guid?>> GetIdsByLoginsAsync
    (
        IReadOnlyCollection<string> logins,
        CancellationToken cancellationToken = default
    )
    {
        using var response = await httpClient
            .PostAsJsonAsync
            (
                $"creatures/creatures/get_ids_by_logins",
                new GetCreaturesIdsByLoginsRequest()
                {
                    Logins = logins
                },
                cancellationToken
            )
            .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        return JsonSerializer
            .Deserialize<GetCreaturesIdsByLoginsResponse>(await response.Content.ReadAsStringAsync(cancellationToken))!
            .CreaturesIds
            .ToDictionary
            (
                c => c.Login,
                c => c.Id
            );
    }
}