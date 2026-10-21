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

namespace ratel_backend_creatures.Constants;

/// <summary>
/// Constants, related to sessions
/// </summary>
public static class Sessions
{
    /// <summary>
    /// Session refresh token length
    /// </summary>
    public const int RefreshTokenLength = 64;

    /// <summary>
    /// Hexadecimal refresh token hash representation have this length in characters
    /// </summary>
    public const int RefreshTokenHashHexStringLength = 128;

    /// <summary>
    /// Refresh token lifetime in days
    /// </summary>
    public const int RefreshTokenLifetime = 30;

    /// <summary>
    /// Session default name length
    /// </summary>
    public const int SessionDefaultNameLength = 24;

    /// <summary>
    /// Access tokens lifetime, in minutes
    /// </summary>
    public const int AccessTokenLifetime = 10;
    
    /// <summary>
    /// Access tokens signing private key minimal size (in bits)
    /// </summary>
    public const int AccessTokenMinimumKeySize = 4096;

    /// <summary>
    /// Access token issuer name
    /// </summary>
    public const string AccessTokenIssuer = "ratel-creatures";

    /// <summary>
    /// Allowed time skew for access tokens validation in seconds
    /// </summary>
    public const int AccessTokenAllowedTimeSkew = 30;
}