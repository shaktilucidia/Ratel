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

namespace ratel_backend_users.Constants;

/// <summary>
/// Constants, related to microservice init
/// </summary>
public static class Init
{
    /// <summary>
    /// Use this code for inter-instances users and roles creation lock
    /// </summary>
    public const int InitUsersAndRolesTransactionCode = 83259143;

    /// <summary>
    /// This roles will be created on server init
    /// </summary>
    public static readonly IReadOnlyCollection<string> Roles = 
    [
        ServerRole.User,
        ServerRole.Administrator
    ];
}