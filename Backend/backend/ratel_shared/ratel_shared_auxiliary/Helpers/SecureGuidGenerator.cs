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
/// Use it to generate secure GUIDs
/// </summary>
public static class SecureGuidGenerator
{
    /// <summary>
    /// Generates a non-empty identifier using cryptographically strong random bytes for all 128 bits
    /// Does not enforce UUID version or variant bits
    /// </summary>
    public static Guid Generate()
    {
        Span<byte> bytes = stackalloc byte[16];

        try
        {
            while (true)
            {
                RandomNumberGenerator.Fill(bytes);

                var result = new Guid(bytes);

                if (result != Guid.Empty)
                {
                    return result;
                }
            }
        }
        finally
        {
            CryptographicOperations.ZeroMemory(bytes);
        }
    }
}