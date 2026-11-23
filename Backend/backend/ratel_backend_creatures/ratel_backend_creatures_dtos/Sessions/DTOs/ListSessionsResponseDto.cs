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

using System.Net;
using System.Text.Json.Serialization;

namespace ratel_backend_creatures_dtos.Sessions.DTOs;

/// <summary>
/// DTO with information about one of creature's sessions
/// </summary>
public class ListSessionsResponseDto
{
    /// <summary>
    /// Session ID
    /// </summary>
    [JsonPropertyOrder(0)]
    [JsonPropertyName("id")]
    public required Guid Id { get; set; }
    
    /// <summary>
    /// Session name
    /// </summary>
    [JsonPropertyOrder(1)]
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    /// <summary>
    /// Is expired?
    /// </summary>
    [JsonPropertyOrder(2)]
    [JsonPropertyName("is_expired")]
    public required bool IsExpired { get; set; }
    
    /// <summary>
    /// Is revoked?
    /// </summary>
    [JsonPropertyOrder(3)]
    [JsonPropertyName("is_revoked")]
    public required bool IsRevoked { get; set; }
    
    /// <summary>
    /// Last activity time
    /// </summary>
    [JsonPropertyOrder(4)]
    [JsonPropertyName("last_activity_time")]
    public required DateTime LastActivityTime { get; set; }
    
    /// <summary>
    /// Last activity source (IP address)
    /// </summary>
    [JsonPropertyOrder(5)]
    [JsonPropertyName("last_activity_source")]
    public required IPAddress LastActivitySource { get; set; }
    
    /// <summary>
    /// Last activity device info (usually UserAgent)
    /// </summary>
    [JsonPropertyOrder(6)]
    [JsonPropertyName("last_activity_device_info")]
    public required string LastActivityDeviceInfo { get; set; }
}