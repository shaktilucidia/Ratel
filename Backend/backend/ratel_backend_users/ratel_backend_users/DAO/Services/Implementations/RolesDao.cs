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

using Microsoft.EntityFrameworkCore;
using ratel_backend_users.DAO.Contexts;
using ratel_backend_users.DAO.Services.Abstract;

namespace ratel_backend_users.DAO.Services.Implementations;

public class RolesDao
(
    MainDbContext dbContext
) : IRolesDao
{
    public async Task<IDictionary<Guid, IReadOnlyCollection<string>>> GetRolesNamesForCreaturesAsync
    (
        IReadOnlyCollection<Guid> creaturesIds,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(creaturesIds);
        
        if (creaturesIds.Count is 0)
        {
            throw new ArgumentException("Don't call this method without providing non-empty IDs list", nameof(creaturesIds));
        }
        
        var distinctCreaturesIds = creaturesIds
            .Distinct();

        var assignments = await dbContext
            .UserRoles
            .Where(cr => distinctCreaturesIds.Contains(cr.UserId))
            .Join
            (
                dbContext.Roles,
                roleAssignment => roleAssignment.RoleId,
                creatureRole => creatureRole.Id,
                (roleAssignment, creatureRole) => new
                {
                    CreatureId = roleAssignment.UserId,
                    RoleName = creatureRole.Name!
                }
            )
            .ToListAsync(cancellationToken);
        
        var rolesByCreature = assignments
        .ToLookup
        (
            a => a.CreatureId,
            a => a.RoleName
        );

        return distinctCreaturesIds
        .ToDictionary(
            id => id,
            id => (IReadOnlyCollection<string>)rolesByCreature[id].ToList()
        );
    }
}