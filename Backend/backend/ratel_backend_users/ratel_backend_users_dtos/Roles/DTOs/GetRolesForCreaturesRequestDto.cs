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

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ratel_backend_users_dtos.Roles.DTOs;

/// <summary>
/// Get roles for creatures DTO
/// </summary>
public class GetRolesForCreaturesRequestDto
{
    /// <summary>
    /// Creatures list
    /// </summary>
    [JsonPropertyOrder(0)]
    [JsonPropertyName("creatures_ids")]
    [property: MinLength(1)]
    public required IReadOnlyCollection<Guid> CreaturesIds { get; set; }
}