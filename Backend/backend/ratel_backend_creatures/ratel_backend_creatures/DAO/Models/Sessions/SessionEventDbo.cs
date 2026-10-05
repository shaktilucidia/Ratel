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
using ratel_backend_creatures.Enums.Sessions;

namespace ratel_backend_creatures.DAO.Models.Sessions;

/// <summary>
/// Event, related to session
/// </summary>
public sealed class SessionEventDbo
{
    /// <summary>
    /// Event ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Event relates to this session
    /// </summary>
    public Guid SessionId { get; set; }
    
    /// <summary>
    /// When even happened
    /// </summary>
    public DateTime OccurredAt { get; set; }
    
    /// <summary>
    /// What happened
    /// </summary>
    public SessionEventType Type { get; set; }

    /// <summary>
    /// Event came from this IP
    /// </summary>
    public required IPAddress Source { get; set; }

    /// <summary>
    /// Arbitrary string with device information (useragent-style)
    /// </summary>
    public required string DeviceInfo { get; set; }
}