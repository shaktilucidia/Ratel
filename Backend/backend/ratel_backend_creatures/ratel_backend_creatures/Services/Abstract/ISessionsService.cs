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

using ratel_backend_creatures_dtos.Sessions.Enums;

namespace ratel_backend_creatures.Services.Abstract;

/// <summary>
/// Service to work with sessions
/// </summary>
public interface ISessionsService
{
    /// <summary>
    /// Create session
    /// </summary>
    /// <param name="login">Login</param>
    /// <param name="password">Password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Tuple with errors, refresh token, access token and access token lifetime
    /// Tokens and lifetime aren't null only if there are no errors</returns>
    Task<Tuple<IReadOnlySet<SessionCreationError>, string?, string?, DateTime?>> CreateAsync
    (
        string login,
        string password,
        CancellationToken cancellationToken = default
    );  
}