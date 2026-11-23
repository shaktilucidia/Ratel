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
using ratel_backend_creatures_dtos.Sessions.DTOs;
using ratel_backend_creatures_dtos.Sessions.Enums;
using ratel_backend_creatures_dtos.Sessions.Extensions;
using ratel_backend_creatures_dtos.Sessions.Requests;
using ratel_backend_creatures_dtos.Sessions.Responses;
using ratel_backend_creatures.Helpers.Sessions;
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
                result.errors,
                result.refreshToken,
                result.accessToken,
                result.accessTokenExpiration
            );
    }
    
    /// <summary>
    /// Get current session information
    /// </summary>
    [Authorize]
    [HttpGet("info")]
    public async Task<ActionResult<SessionInfoResponse>> GetCurrentSessionInfoAsync(CancellationToken cancellationToken)
    {
        var creatureId = User.GetCreatureId();
        var sessionId = User.GetSessionId();
        var sessionInfo = await sessionsService
            .GetSessionInfoAsync
            (
                creatureId,
                sessionId,
                cancellationToken
            );
        
        return Ok
        (
            new SessionInfoResponse()
            {
                SessionInfo = new SessionInfoDto()
                {
                    CreatureId = creatureId,
                    SessionId = sessionId,
                    // Session is already validated in token validator and can't be null
                    SessionValidTill = SessionsHelper.CalculateSessionExpirationTime(sessionInfo!.Value.lastActivityTime),
                    AccessTokenValidTill = User.GetAccessTokenExpirationTime()
                }
            }
        );
    }

    /// <summary>
    /// Creature's sessions
    /// </summary>
    [Authorize]
    [HttpPost("list")]
    public async Task<ActionResult<SessionInfoResponse>> ListSessionsAsync
    (
        [FromBody] ListSessionsRequest request,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var creatureId = User.GetCreatureId();

        return Ok
        (
            new ListSessionsResponse()
            {
                Sessions
                =
                (
                    await sessionsService
                        .ListSessionsForCreatureAsync
                        (
                            creatureId,
                            request.Parameters.IsListExpired,
                            request.Parameters.IsListRevoked,
                            cancellationToken
                        )
                )
                .Select
                (
                    sessionInfo
                    =>
                    new ListSessionsResponseDto()
                    {
                        Id = sessionInfo.id,
                        Name = sessionInfo.name,
                        IsExpired = sessionInfo.isExpired,
                        IsRevoked = sessionInfo.isRevoked,
                        LastActivityTime = sessionInfo.lastActivityTime,
                        LastActivitySource = sessionInfo.lastActivitySource,
                        LastActivityDeviceInfo = sessionInfo.lastActivityDeviceInfo
                    }
                )
                .ToList()
            }
        );
    }

    /// <summary>
    /// Get new access / refresh tokens
    /// </summary>
    [AllowAnonymous]
    [Route("refresh")]
    [HttpPost]
    public async Task<IActionResult> RefreshAccessTokenAsync
    (
        [FromBody] RefreshAccessTokenRequest request,
        CancellationToken cancellationToken
    )
    {
        return this
            .ToActionResult
            (
                new HashSet<RefreshAccessTokenError>(),
                "YiffyRefreshToken",
                "YuffyAccessToken",
                DateTime.UtcNow
            );
    }
}