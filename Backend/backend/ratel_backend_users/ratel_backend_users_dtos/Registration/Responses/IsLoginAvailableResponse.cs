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

using ratel_backend_users_dtos.Registration.DTOs;

namespace ratel_backend_users_dtos.Registration.Responses;

/// <summary>
/// Login availability response
/// </summary>
public class IsLoginAvailableResponse
{
    /// <summary>
    /// Login availability data
    /// </summary>
    public required IsLoginAvailableResponseDto AvailabilityData { get; set; }
}