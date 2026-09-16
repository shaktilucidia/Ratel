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

using ratel_backend_users.Models.Business.Creatures;
using ratel_backend_users_dtos.Registration.Enums;

namespace ratel_backend_users.Services.Abstract;

/// <summary>
/// Service, used to register users
/// </summary>
public interface IRegistrationService
{
    /// <summary>
    /// Checks if login available or not
    /// </summary>
    /// <returns>True if login is available</returns>
    Task<bool> IsLoginAvailableAsync(string login);

    /// <summary>
    /// Register creature
    /// </summary>
    /// <param name="login">Creature's login</param>
    /// <param name="password">Creature's password</param>
    /// <returns>Errors set and creature (if registration was successfull, otherwise null)
    /// Registration is successful if errors set is empty</returns>
    Task<Tuple<IReadOnlySet<RegistrationError>, Creature?>> RegisterAsync
    (
        string login,
        string password,
        CancellationToken cancellationToken = default
    );
    
    /// <summary>
    /// Add roles to existing creatures
    /// </summary>
    /// <param name="creatureId">Creature ID</param>
    /// <param name="roles">Roles names</param>
    Task AddRoleToCreatureAsync(Guid creatureId, IReadOnlyCollection<string> roles);
}
