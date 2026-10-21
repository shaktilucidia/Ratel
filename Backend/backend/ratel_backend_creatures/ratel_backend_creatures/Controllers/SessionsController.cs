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
using ratel_backend_creatures_dtos.Sessions.Extensions;
using ratel_backend_creatures_dtos.Sessions.Requests;
using ratel_backend_creatures.Services.Abstract;
using ratel_shared_auxiliary.Controllers;

namespace ratel_backend_creatures.Controllers;

/// <summary>
/// Controller, related to sessions
/// </summary>
[Route("sessions")]
[ApiController]
public class SessionsController
(
    ISessionsService sessionsService
) : RatelControllerBase
{
    /// <summary>
    /// Create a session
    /// </summary>
    [AllowAnonymous]
    [Route("create")]
    [HttpPost]
    public async Task<IActionResult> CreateAsync
    (
        [FromBody] CreateSessionRequest request,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = await sessionsService.CreateAsync
        (
            request.CreateSessionData.Login,
            request.CreateSessionData.Password,
            RequestSource, 
            UserAgent,
            cancellationToken
        ); 
        
        return this
            .ToActionResult
            (
                result.Item1,
                result.Item2,
                result.Item3,
                result.Item4
            );
    }
    
    /// <summary>
    /// TODO: Delete it, just for debugging
    /// </summary>
    [Authorize]
    [HttpGet("current_session")]
    public IActionResult GetCurrentSession()
    {
        var creatureId = Guid.Parse(User.FindFirst("sub")!.Value);
        var sessionId = Guid.Parse(User.FindFirst("sid")!.Value);

        return Ok(new
        {
            CreatureId = creatureId,
            SessionId = sessionId
        });
    }
}