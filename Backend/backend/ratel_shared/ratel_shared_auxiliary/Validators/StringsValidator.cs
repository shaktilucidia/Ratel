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

namespace ratel_shared_auxiliary.Validators;

/// <summary>
/// Use it to validate strings
/// </summary>
public class StringsValidator
{
    /// <summary>
    /// Validates that string is not null, not empty and is HEX-sequence
    /// </summary>
    public static void MustBeNonEmptyHexSequence(string st)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(st);

        if (!st.All(Uri.IsHexDigit))
        {
            throw new ArgumentException("String is not hexadecimal sequence", nameof(st));
        }
    }
}