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

using System.Text.Json.Serialization;

namespace ratel_backend_creatures_dtos.Sessions.DTOs;

/// <summary>
/// Information about current session
/// </summary>
public class SessionInfoDto
{
    /// <summary>
    /// Creature ID
    /// </summary>
    [JsonPropertyOrder(0)]
    [JsonPropertyName("creature_id")]
    public required Guid CreatureId { get; set; }
    
    /// <summary>
    /// Session ID
    /// </summary>
    [JsonPropertyOrder(1)]
    [JsonPropertyName("session_id")]
    public required Guid SessionId { get; set; }
    
    /// <summary>
    /// Session will expire at this time if it will be no token refreshes
    /// </summary>
    [JsonPropertyOrder(2)]
    [JsonPropertyName("session_valid_till")]
    public required DateTime SessionValidTill { get; set; }
    
    /// <summary>
    /// Current access token will expire at this time
    /// </summary>
    [JsonPropertyOrder(3)]
    [JsonPropertyName("access_token_valid_till")]
    public required DateTime AccessTokenValidTill { get; set; }
}