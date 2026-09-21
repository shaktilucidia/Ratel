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

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ratel_backend_users.DAO.Models.Creatures;
using ratel_backend_users.Services.Abstract;

namespace ratel_backend_users.Services.Implementation;

public class CreaturesService
(
    UserManager<CreatureDbo> userManager
) : ICreaturesService
{
    public async Task<IDictionary<string, Guid?>> GetIdsByLoginsAsync
    (
        IReadOnlyCollection<string> logins,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(logins);

        if (logins.Count is 0)
        {
            throw new ArgumentException("Empty logins collection", nameof(logins));
        }

        if (logins.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException("At least one login is null or empty", nameof(logins));
        }

        var loginsToNormalizedLogins = logins
            .Distinct()
            .ToDictionary
            (
                l => l,
                l => userManager.NormalizeName(l)
            );

        if (logins.Count != loginsToNormalizedLogins.Count)
        {
            throw new ArgumentException("Some logins are non-unique", nameof(logins));
        }
        
        var normalizedLogins = loginsToNormalizedLogins
            .Values
            .Distinct()
            .ToList();

        if (normalizedLogins.Count != logins.Count)
        {
            throw new ArgumentException("Normalization lead to non-unique logins", nameof(logins));
        }

        var idsByNormalizedLogins = await userManager
            .Users
            .Where(c => normalizedLogins.Contains(c.NormalizedUserName!))
            .Select
            (
                c => new
                {
                    Login = c.NormalizedUserName!,
                    c.Id
                }
            )
            .ToDictionaryAsync
            (
                c => c.Login,
                c => c.Id,
                cancellationToken
            );
        
        return loginsToNormalizedLogins
            .ToDictionary
            (
                kvp => kvp.Key,
                kvp => idsByNormalizedLogins.TryGetValue(kvp.Value, out var id) ? (Guid?)id : null
            );
    }
}