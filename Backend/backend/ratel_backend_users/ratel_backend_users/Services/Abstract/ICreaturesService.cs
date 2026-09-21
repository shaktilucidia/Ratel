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

namespace ratel_backend_users.Services.Abstract;

/// <summary>
/// Service to work with creatures
/// </summary>
public interface ICreaturesService
{
    /// <summary>
    /// Get creatures IDs by logins. Empty logins not allowed and will lead to exception
    /// Duplicated logins will be distincted
    /// </summary>
    /// <param name="logins">Logins</param>
    /// <returns>Logins to IDs dictionary. ID will be null for unknown logins</returns>
    Task<IDictionary<string, Guid?>> GetIdsByLoginsAsync
    (
        IReadOnlyCollection<string> logins,
        CancellationToken cancellationToken = default
    );
}