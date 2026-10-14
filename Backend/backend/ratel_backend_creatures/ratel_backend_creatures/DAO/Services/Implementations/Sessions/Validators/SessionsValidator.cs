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
using ratel_backend_creatures.Enums.Sessions;

namespace ratel_backend_creatures.DAO.Services.Implementations.Sessions.Validators;

/// <summary>
/// Use it to validate sessions
/// </summary>
public static class SessionsValidator
{
    /// <summary>
    /// Validates freshly-created session. Call it before adding session to DB
    /// </summary>
    public static void ValidateFreshSession(CreatureSessionDbo session)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(session.RefreshTokens);
        ArgumentNullException.ThrowIfNull(session.Events);
        
        if (session.Id.Equals(Guid.Empty))
        {
            throw new ArgumentException("Session must have an ID", nameof(session));
        }
        
        if (session.CreatureId.Equals(Guid.Empty))
        {
            throw new ArgumentException("CreatureId must not be empty", nameof(session));
        }
        
        ArgumentException.ThrowIfNullOrWhiteSpace(session.Name);

        if (session.Name.Length > Constants.DAO.Sessions.NameMaxLength)
        {
            throw new ArgumentException("Session name exceeds the maximum allowed length.", nameof(session));
        }

        if (session.RefreshTokens.Count != 1)
        {
            throw new ArgumentException("New session must have exactly one refresh token", nameof(session));
        }

        var refreshToken = session.RefreshTokens.Single();
        
        refreshToken.ValidateRefreshTokenAgainstSession(session.Id);
        
        if (refreshToken.UsedAt.HasValue)
        {
            throw new ArgumentException("Refresh token in session must not be marked as used", nameof(session));
        }
        
        if (session.Events.Count != 1)
        {
            throw new ArgumentException("New session must have exactly one event", nameof(session));
        }

        var sessionEvent = session.Events.Single();
        
        sessionEvent.ValidateAgainstSession(session.Id);

        if (sessionEvent.Type != SessionEventType.Created)
        {
            throw new ArgumentException("First event in session must have Created type", nameof(session));
        }
    }
}