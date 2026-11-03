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

using System.Security.Claims;

namespace ratel_backend_creatures.Helpers.Sessions;

/// <summary>
/// Helper to work with claims
/// </summary>
public static class ClaimsHelper
{
    /// <summary>
    /// Get creature ID from claims
    /// </summary>
    public static Guid GetCreatureId(this ClaimsPrincipal principal)
    {
        return ExtractGuidFromClaim(principal.GetRawClaim(Constants.Sessions.CreatureIdClaimType));
    }
    
    /// <summary>
    /// Get session ID from claims
    /// </summary>
    public static Guid GetSessionId(this ClaimsPrincipal principal)
    {
        return ExtractGuidFromClaim(principal.GetRawClaim(Constants.Sessions.SessionIdClaimType));
    }
    
    /// <summary>
    /// Get access token expiration time
    /// </summary>
    public static DateTime GetAccessTokenExpirationTime(this ClaimsPrincipal principal)
    {
        return ExtractDateTimeInUnixTimeFormatFromClaim(principal.GetRawClaim(Constants.Sessions.AccessTokenExpirationTimeClaimType));
    }
    
    /// <summary>
    /// Get raw claim
    /// </summary>
    private static string GetRawClaim
    (
        this ClaimsPrincipal principal,
        string claimType
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(claimType);
        
        var rawClaim = principal
            .Claims
            .SingleOrDefault(claim => claim.Type.Equals(claimType));

        if (rawClaim is null)
        {
            throw new InvalidOperationException($"{ claimType } claim is absent in claims");
        }
        
        return rawClaim.Value;
    }

    /// <summary>
    /// Extract non-empty GUID from raw claim or throw an exception
    /// </summary>
    private static Guid ExtractGuidFromClaim(string rawClaim)
    {
        if
        (
            !Guid.TryParse(rawClaim, out var result)
            ||
            result == Guid.Empty
        )
        {
            throw new InvalidOperationException($"Claim is not a GUID. Raw value is { rawClaim }");
        }

        return result;
    }

    /// <summary>
    /// Extract long number from raw claim or throw an exception
    /// </summary>
    private static long ExtractLongFromClaim(string rawClaim)
    {
        if (!long.TryParse(rawClaim, out var result))
        {
            throw new InvalidOperationException($"Claim is not a long number. Raw value is { rawClaim }");
        }

        return result;
    }
    
    /// <summary>
    /// Extract time from claim. It is expected that claim stores it as seconds since 1970
    /// </summary>
    private static DateTime ExtractDateTimeInUnixTimeFormatFromClaim(string rawClaim)
    {
        return DateTimeOffset.FromUnixTimeSeconds(ExtractLongFromClaim(rawClaim)).UtcDateTime;
    }
}