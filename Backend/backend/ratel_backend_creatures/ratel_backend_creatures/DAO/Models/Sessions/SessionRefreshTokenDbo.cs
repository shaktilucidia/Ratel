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

namespace ratel_backend_creatures.DAO.Models.Sessions;

/// <summary>
/// Session refresh token
/// </summary>
public sealed class SessionRefreshTokenDbo
{
    /// <summary>
    /// Token ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Token relates to this session
    /// </summary>
    public Guid SessionId { get; set; }

    /// <summary>
    /// Token hash
    /// </summary>
    public required string TokenHash { get; set; }

    /// <summary>
    /// Token created at this time
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Token must be used before this time
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Use time, populated if token was used
    /// </summary>
    public DateTime? UsedAt { get; set; }
}