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
using Microsoft.AspNetCore.Mvc;
using ratel_backend_users_dtos.Registration.Requests;
using ratel_backend_users_dtos.Roles.DTOs;
using ratel_backend_users_dtos.Roles.Requests;
using ratel_backend_users_dtos.Roles.Responses;
using ratel_backend_users.Services.Abstract;

namespace ratel_backend_users.Controllers;

/// <summary>
/// Controller, related to roles
/// </summary>
[Route("roles")]
[ApiController]
public class RolesController
(
    IRolesService rolesService
) : ControllerBase
{
    /// <summary>
    /// Get roles, assigned to creatures
    /// </summary>
    [AllowAnonymous]
    [Route("get_for_creatures")]
    [HttpPost]
    public async Task<ActionResult<GetRolesForCreaturesResponse>> GetRolesForCreaturesAsync
    (
        [FromBody] GetRolesForCreaturesRequest request,
        CancellationToken cancellationToken
    )
    {
        return Ok
        (
            new GetRolesForCreaturesResponse()
            {
                RolesForCreature = (await rolesService.GetRolesNamesForCreaturesAsync
                (
                    request.RequestData.CreaturesIds,
                    cancellationToken
                ))
                .Select(r => new RolesForCreatureDto()
                {
                    CreatureId = r.Key,
                    RolesNames = r.Value
                })
                .ToList()
            }
        );
    }
}