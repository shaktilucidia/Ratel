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
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace ratel_backend_creatures.Helpers.Sessions;

/// <summary>
/// Helper to work with sessions refresh tokens
/// </summary>
public static class SessionsRefreshTokensHelper
{
    /// <summary>
    /// Create session refresh token
    /// </summary>
    public static string Create()
    {
        return WebEncoders.Base64UrlEncode
        (
            RandomNumberGenerator.GetBytes(Constants.Sessions.RefreshTokenLength)
        );
    }
    
    /// <summary>
    /// Hash token
    /// </summary>
    public static string Hash(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        
        return Convert.ToHexString( SHA512.HashData(Encoding.UTF8.GetBytes(token)));
    }
}