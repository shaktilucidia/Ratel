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
/// Response to an attempt to create session
/// </summary>
public class CreateSessionResponseDto
{
    /// <summary>
    /// Session refresh token
    /// </summary>
    [JsonPropertyOrder(0)]
    [JsonPropertyName("refresh_token")]
    public required string RefreshToken { get; set; }
    
    /// <summary>
    /// Use this to make API calls
    /// </summary>
    [JsonPropertyOrder(1)]
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; set; }
    
    /// <summary>
    /// Access token is valid till this time
    /// </summary>
    [JsonPropertyOrder(2)]
    [JsonPropertyName("access_token_valid_till")]
    public DateTime AccessTokenValidTill { get; set; }
}