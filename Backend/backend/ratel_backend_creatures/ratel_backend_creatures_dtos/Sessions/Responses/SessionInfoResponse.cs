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
using ratel_backend_creatures_dtos.Sessions.DTOs;

namespace ratel_backend_creatures_dtos.Sessions.Responses;

/// <summary>
/// Response with session info
/// </summary>
public class SessionInfoResponse
{
    /// <summary>
    /// Current session info
    /// </summary>
    [JsonPropertyOrder(0)]
    [JsonPropertyName("session_info")]
    public required SessionInfoDto SessionInfo { get; set; }
}