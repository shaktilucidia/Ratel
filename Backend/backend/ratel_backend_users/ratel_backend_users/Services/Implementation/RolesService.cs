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

using ratel_backend_users.DAO.Services.Abstract;
using ratel_backend_users.Services.Abstract;

namespace ratel_backend_users.Services.Implementation;

public class RolesService
(
    IRolesDao rolesDao
) : IRolesService
{
    public async Task<IDictionary<Guid, IReadOnlyCollection<string>>> GetRolesNamesForCreaturesAsync
    (
        IReadOnlyCollection<Guid> creaturesIds,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(creaturesIds);
        
        if (creaturesIds.Count is 0)
        {
            throw new ArgumentException("Don't call this method without providing non-empty IDs list", nameof(creaturesIds));
        }
        
        return await rolesDao.GetRolesNamesForCreaturesAsync(creaturesIds, cancellationToken);
    }
}