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

using ratel_backend_creatures.DAO.Models.Sessions;
using ratel_shared_auxiliary.Validators;

namespace ratel_backend_creatures.DAO.Services.Implementations.Sessions.Validators;

/// <summary>
/// Use it to validate session refresh tokens
/// </summary>
public static class SessionRefreshTokensValidator
{
    /// <summary>
    /// Validate token hash format
    /// </summary>
    public static void ValidateTokenHashFormat(string token)
    {
        StringsValidator.MustBeNonEmptyHexSequence(token);

        if (token.Length != Constants.Sessions.RefreshTokenHashHexStringLength)
        {
            throw new ArgumentException("Token hash has wrong length", nameof(token));
        }
    }

    /// <summary>
    /// Validate token against a session
    /// </summary>
    public static void ValidateRefreshTokenAgainstSession
    (
        this SessionRefreshTokenDbo token,
        Guid sessionId
    )
    {
        ArgumentNullException.ThrowIfNull(token);

        if (token.Id.Equals(Guid.Empty))
        {
            throw new ArgumentException("Token must have an ID", nameof(token));
        }
        
        if (sessionId.Equals(Guid.Empty))
        {
            throw new ArgumentException("Session ID is empty", nameof(sessionId));
        }
        
        if (token.SessionId != sessionId)
        {
            throw new ArgumentException("Refresh token doesn't belong to given session", nameof(token.SessionId));
        }
        
        ValidateTokenHashFormat(token.Hash);
        
        DatesTimesValidator.MustBeUtcAndNotDefault(token.CreatedAt);
        DatesTimesValidator.MustBeUtcAndNotDefault(token.ExpiresAt);

        if (token.ExpiresAt <= token.CreatedAt)
        {
            throw new ArgumentException("ExpiresAt time is less or equal CreatedAt");
        }

        if (token.UsedAt.HasValue)
        {
            DatesTimesValidator.MustBeUtcAndNotDefault(token.UsedAt.Value);

            if (token.UsedAt.Value >= token.ExpiresAt || token.UsedAt.Value <= token.CreatedAt)
            {
                throw new ArgumentException("Wrong token usage date", nameof(token.UsedAt));
            }
        }
    }
}