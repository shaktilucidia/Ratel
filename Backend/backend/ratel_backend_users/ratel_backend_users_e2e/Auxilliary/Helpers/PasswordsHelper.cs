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

namespace ratel_backend_users_e2e.Auxilliary;

/// <summary>
/// Helper to work with passwords
/// </summary>
public static class PasswordsHelper
{
    /// <summary>
    /// Generates correct password
    ///
    /// !! IT IS NOT CRYPTOGRAPHICALLY STRONG !!
    /// </summary>
    public static string GenerateCorrectPassword()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        return new string
        (
            Enumerable
                .Range(0, 31)
                .Select(_ => chars[Random.Shared.Next(chars.Length)])
                .ToArray()
        );
    }

    /// <summary>
    /// Generates password, by default it will be correct password, otherwise weak unacceptable password
    /// </summary>
    public static string GeneratePassword(bool isGenerateCorrectPassword = true)
    {
        return isGenerateCorrectPassword ? GenerateCorrectPassword() : "yiff";
    }

}
