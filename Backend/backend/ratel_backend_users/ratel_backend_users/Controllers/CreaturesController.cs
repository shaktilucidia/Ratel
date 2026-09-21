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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ratel_backend_users_dtos.Roles.DTOs;
using ratel_backend_users_dtos.Roles.Requests;
using ratel_backend_users_dtos.Roles.Responses;
using ratel_backend_users.Services.Abstract;

namespace ratel_backend_users.Controllers;

/// <summary>
/// Controller, related to creatures
/// </summary>
[Route("creatures")]
[ApiController]
public class CreaturesController
(
    ICreaturesService creaturesService
) : ControllerBase
{
    /// <summary>
    /// Get creatures IDs
    /// </summary>
    [AllowAnonymous]
    [Route("get_ids_by_logins")]
    [HttpPost]
    public async Task<ActionResult<GetCreaturesIdsByLoginsResponse>> GetIdsByLoginsAsync
    (
        [FromBody] GetCreaturesIdsByLoginsRequest request,
        CancellationToken cancellationToken
    )
    {
        return Ok
        (
            new GetCreaturesIdsByLoginsResponse()
            {
                CreaturesIds = (await creaturesService.GetIdsByLoginsAsync
                (
                    request.Logins,
                    cancellationToken
                ))
                .Select
                (
                    kvp
                    =>
                    new CreatureIdDto()
                    {
                        Login = kvp.Key,
                        Id = kvp.Value
                    }
                )
                .ToList()
            }
        );
    }
}