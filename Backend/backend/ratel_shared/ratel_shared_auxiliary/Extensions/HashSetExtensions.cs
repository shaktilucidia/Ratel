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

namespace ratel_shared_auxiliary.Extensions;

/// <summary>
/// Useful extensions for Hash Set
/// </summary>
public static class HashSetExtensions
{
    /// <summary>
    /// Add new value to HashSet or throw an exception if it already exists
    /// </summary>
    /// <typeparam name="T">HashSet item type</typeparam>
    /// <param name="set">Add item to this set</param>
    /// <param name="value">What to add</param>
    /// <exception cref="InvalidOperationException">In case if item already exists in set</exception>
    public static void AddUnique<T>
    (
        this HashSet<T> set,
        T value
    )
    {
        if (!set.Add(value))
        {
            throw new InvalidOperationException($"Duplicate value '{ value }' was added");
        }
    }
}
