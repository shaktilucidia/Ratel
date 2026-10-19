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

using System.Security.Cryptography;

namespace ratel_shared_auxiliary.Helpers;

/// <summary>
/// Use it to generate cryptographically-strong random strings
/// </summary>
public static class SecureStringsGenerator
{
    /// <summary>
    /// Alphabet for generated strings
    /// </summary>
    private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    
    /// <summary>
    /// Generates a cryptographically random string of Latin letters
    /// </summary>
    public static string Generate(int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);

        return string.Create
        (
            length,
            Alphabet,
            static (characters, alphabet)
            =>
            {
                for (var i = 0; i < characters.Length; i++)
                {
                    characters[i] = alphabet[RandomNumberGenerator.GetInt32(alphabet.Length)];
                }
            }
        );
    }
}