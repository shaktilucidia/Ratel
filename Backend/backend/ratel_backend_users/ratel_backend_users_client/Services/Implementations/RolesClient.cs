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
using ratel_backend_users_dtos.Roles.DTOs;
using ratel_backend_users_dtos.Roles.Requests;
using ratel_backend_users_dtos.Roles.Responses;

namespace ratel_backend_users_client.Services.Implementations;

public class RolesClient
(
    HttpClient httpClient
) : IRolesClient
{
    public async Task<IReadOnlyDictionary<Guid, IReadOnlyCollection<string>>> GetRolesByCreaturesIdsAsync
    (
        IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken
    )
    {
        using var response = await httpClient
            .PostAsJsonAsync
            (
                $"creatures/roles/get_for_creatures",
                new GetRolesForCreaturesRequest()
                {
                    RequestData = new GetRolesForCreaturesRequestDto()
                    {
                        CreaturesIds = ids
                    }
                },
                cancellationToken
            )
            .ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        return JsonSerializer
            .Deserialize<GetRolesForCreaturesResponse>(await response.Content.ReadAsStringAsync(cancellationToken))!
            .RolesForCreature
            .ToDictionary
            (
                r => r.CreatureId,
                r => r.RolesNames
            );
    }
}