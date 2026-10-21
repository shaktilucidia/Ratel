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

using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ratel_backend_creatures.CustomImplementations;

/// <summary>
/// Here we have code, called after access tokens being validated
/// </summary>
public class AccessTokensValidationEvents : JwtBearerEvents
{
    public override Task TokenValidated
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

            return Task.CompletedTask;
        }

        if
        (
            !Guid.TryParse(sessionIdRaw, out var sessionId)
            ||
            sessionId == Guid.Empty
        )
        {
            context.Fail($"Access token has an invalid session ID, raw value = \" { sessionIdRaw } \"");

            return Task.CompletedTask;
        }

        // TODO: Put session validation here
        
        return Task.CompletedTask;
    }
}