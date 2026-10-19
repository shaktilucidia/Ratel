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

using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using ratel_backend_creatures.Models.Business.Sessions;
using ratel_backend_creatures.Services.Abstract;
using ratel_shared_auxiliary.Helpers;
using ratel_shared_auxiliary.Services.Abstract;

namespace ratel_backend_creatures.Services.Implementation;

public class AccessTokensService
(
    ITimeProvider timeProvider,
    AccessTokensSigningKey accessTokensSigningKey
): IAccessTokensService
{
    public IssuedAccessToken Issue
    (
        Guid creatureId,
        Guid sessionId,
        string issuer,
        string audience
    )
    {
        if (creatureId.Equals(Guid.Empty))
        {
            throw new ArgumentException("Creature ID is not specified", nameof(creatureId));
        }
        
        if (sessionId.Equals(Guid.Empty))
        {
            throw new ArgumentException("Session ID is not specified", nameof(sessionId));
        }
        
        ArgumentException.ThrowIfNullOrWhiteSpace(issuer);
        ArgumentException.ThrowIfNullOrWhiteSpace(audience);

        var now = timeProvider.GetNow();
        var expiresAt = now.AddMinutes(Constants.Sessions.AccessTokenLifetime);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,

            Subject = new ClaimsIdentity(
            [
                new Claim("sub", creatureId.ToString()),
                new Claim("sid", sessionId.ToString()),
                new Claim("jti", SecureGuidGenerator.Generate().ToString())
            ]),

            IssuedAt = now,
            NotBefore = now,
            Expires = expiresAt,

            SigningCredentials = accessTokensSigningKey.Credentials
        };

        var token = new JsonWebTokenHandler().CreateToken(descriptor);

        return new IssuedAccessToken(token, expiresAt);
    }
}