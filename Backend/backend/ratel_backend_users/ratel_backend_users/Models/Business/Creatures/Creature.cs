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

using ratel_backend_users.DAO.Models.Creatures;

namespace ratel_backend_users.Models.Business.Creatures;

/// <summary>
/// Just a creature
/// </summary>
public class Creature
{
    /// <summary>
    /// User ID
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// User login
    /// </summary>
    public string Login { get; }

    public Creature
    (
        Guid id,
        string login
    )
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID cannot be empty", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(login))
        {
            throw new ArgumentException("Login cannot be empty", nameof(login));
        }
        
        Id = id;
        Login = login;
    }

    public Creature(CreatureDbo creatureDbo)
    {
        Id = creatureDbo.Id;
        Login = creatureDbo.UserName ?? throw new ArgumentNullException(nameof(creatureDbo.UserName));
    }
}