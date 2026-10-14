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

namespace ratel_shared_auxiliary.Validators;

/// <summary>
/// Use it to validate DateTime
/// </summary>
public static class DatesTimesValidator
{
    /// <summary>
    /// Validates that DateTime is in UTC and doesn't have a default value
    /// </summary>
    public static void MustBeUtcAndNotDefault(DateTime dt)
    {
        if (dt.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("DateTime must be in UTC", nameof(dt));
        }

        if (dt.Equals(default))
        {
            throw new ArgumentException("DateTime is default, a bug is somewhere", nameof(dt));
        }
    }
}