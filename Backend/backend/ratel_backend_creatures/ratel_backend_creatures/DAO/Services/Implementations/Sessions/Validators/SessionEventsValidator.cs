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
/// Use this to validate session events
/// </summary>
public static class SessionEventsValidator
{
    /// <summary>
    /// Validate event against session with given ID
    /// </summary>
    public static void ValidateAgainstSession
    (
        this SessionEventDbo sessionEvent,
        Guid sessionId
    )
    {
        ArgumentNullException.ThrowIfNull(sessionEvent);

        if (sessionEvent.Id.Equals(Guid.Empty))
        {
            throw new ArgumentException("Session event must have an ID", nameof(sessionEvent));
        }
        
        if (sessionId.Equals(Guid.Empty))
        {
            throw new ArgumentException("Session ID is empty", nameof(sessionId));
        }

        if (sessionEvent.SessionId != sessionId)
        {
            throw new ArgumentException("Session event doesn't belong to given session", nameof(sessionEvent));
        }
        
        DatesTimesValidator.MustBeUtcAndNotDefault(sessionEvent.OccurredAt);
        
        if (!Enum.IsDefined(sessionEvent.Type))
        {
            throw new ArgumentException("Undefined event type", nameof(sessionEvent));
        }
        
        ArgumentNullException.ThrowIfNull(sessionEvent.Source);
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionEvent.DeviceInfo);
    }
}