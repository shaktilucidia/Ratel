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

using ratel_shared_auxiliary.Validators;

namespace ratel_backend_creatures.Helpers.Sessions;

/// <summary>
/// Common helper for sessions
/// </summary>
public static class SessionsHelper
{
    /// <summary>
    /// Calculate session expiration time based on last activity time
    /// </summary>
    public static DateTime CalculateSessionExpirationTime(DateTime sessionLastActivityTime)
    {
        DatesTimesValidator.MustBeUtcAndNotDefault(sessionLastActivityTime);
        
        return sessionLastActivityTime.AddDays(Constants.Sessions.SessionLifetime);
    }
}