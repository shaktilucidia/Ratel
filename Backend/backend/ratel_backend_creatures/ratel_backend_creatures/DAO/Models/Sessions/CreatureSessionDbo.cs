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
using ratel_backend_creatures.DAO.Models.Creatures;

namespace ratel_backend_creatures.DAO.Models.Sessions;

/// <summary>
/// One of creature's sessions
/// </summary>
public sealed class CreatureSessionDbo
{
    /// <summary>
    /// Session ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Creature ID
    /// </summary>
    public Guid CreatureId { get; set; }

    /// <summary>
    /// Session name
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Refresh tokens for session
    /// </summary>
    public required ICollection<SessionRefreshTokenDbo> RefreshTokens { get; set; }

    /// <summary>
    /// Events, related to session (created, updated, revoked and so on)
    /// </summary>
    public required ICollection<SessionEventDbo> Events { get; set; }
}