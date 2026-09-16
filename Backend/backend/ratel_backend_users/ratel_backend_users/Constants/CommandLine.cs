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
/// Commandline constants
/// </summary>
public static class CommandLine
{
    /// <summary>
    /// Add this argument to command line to run migrations
    /// </summary>
    public const string ApplyMigrationsArgName = "apply_migrations";

    /// <summary>
    /// Use this once during server setup to create roles and administrator account
    /// </summary>
    public const string InitUsersArgName = "init_users";

    /// <summary>
    /// Call this after each version update to update roles/users
    /// </summary>
    public const string UpdateUsersArgName = "update_users";
}
