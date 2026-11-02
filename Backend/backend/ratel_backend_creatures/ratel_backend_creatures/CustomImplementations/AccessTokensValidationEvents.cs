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

using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ratel_backend_creatures.DAO.Services.Abstract;
using ratel_shared_auxiliary.Services.Abstract;

namespace ratel_backend_creatures.CustomImplementations;

/// <summary>
/// Here we have code, called after access tokens being validated
/// </summary>
public class AccessTokensValidationEvents
(
    ISessionsDao sessionsDao,
    ILogger<AccessTokensValidationEvents> logger,
    ITimeProvider timeProvider
) : JwtBearerEvents
{
    public override async Task TokenValidated
    (
        TokenValidatedContext context
    )
    {
        ArgumentNullException.ThrowIfNull(context.Principal);
        
        var creatureIdRaw = context
            .Principal
            .Claims
            .Single(claim => claim.Type.Equals("sub"))
            .Value;
        
        var sessionIdRaw = context
            .Principal
            .Claims
            .Single(claim => claim.Type.Equals("sid"))
            .Value;

        if
        (
            !Guid.TryParse(creatureIdRaw, out var creatureId)
            ||
            creatureId == Guid.Empty
        )
        {
            context.Fail($"Access token has an invalid creature ID, raw value = \"{ creatureIdRaw }\"");

            return;
        }

        if
        (
            !Guid.TryParse(sessionIdRaw, out var sessionId)
            ||
            sessionId == Guid.Empty
        )
        {
            context.Fail($"Access token has an invalid session ID, raw value = \" { sessionIdRaw } \"");
            return;
        }

        var sessionInfo = await sessionsDao
            .GetSessionInfoAsync
            (
                creatureId,
                sessionId,
                context.HttpContext.RequestAborted
            );

        if (sessionInfo is null)
        {
            logger.LogWarning
            (
                "Unable to find session with ID = {SessionId} for creature with ID = {CreatureId}",
                sessionId,
                creatureId
            );
            
            context.Fail($"Unable to find session with ID = { sessionId } for creature with ID = { creatureId }");
            return;
        }

        if (sessionInfo!.Item1)
        {
            var rawToken = (context.SecurityToken as JwtSecurityToken)?.RawData; 
            
            logger.LogWarning
            (
                "Session {SessionID} was revoked, but was attempted to use with access token {AccessToken}",
                sessionId,
                rawToken
            );
            
            context.Fail($"Session {sessionId} was revoked, but was attempted to use with access token {rawToken}");
            return;
        }

        var now = timeProvider.GetNow();
        var lastSessionEventTime = sessionInfo.Item2;

        if (now < lastSessionEventTime)
        {
            logger.LogWarning
            (
                "Session {SessionID} have it last event in future, with time {LastEventDate}",
                sessionId,
                lastSessionEventTime
            );
            
            context.Fail($"Session {sessionId} have it last event in future, with time {lastSessionEventTime}");
            return;
        }
        
        var sessionAge = (now - lastSessionEventTime).TotalDays;

        if (sessionAge > Constants.Sessions.SessionExpirationTime)
        {
            logger.LogWarning
            (
                "Attempt to use session {SessionID}, which expired {SessionAge} days ago",
                sessionId,
                sessionAge
            );
            
            context.Fail($"Attempt to use session {sessionId}, which expired {sessionAge} days ago");
            return;
        }
    }
}