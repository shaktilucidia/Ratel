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
using Microsoft.Extensions.Options;
using ratel_backend_users.Constants;
using ratel_backend_users.DAO.Contexts;
using ratel_backend_users.DAO.Models.Creatures;
using ratel_backend_users.Models.Settings;
using ratel_backend_users.Services.Abstract;
using ratel_shared_auxiliary.UoW.Abstract;

namespace ratel_backend_users.Services.Implementation;

public class UsersAndRolesInitializer
(
    IUnitOfWork unitOfWork,
    MainDbContext dbContext,
    RoleManager<CreatureRoleDbo> rolesManager,
    ILogger<UsersAndRolesInitializer> logger,
    IOptions<AdministratorAccountSettings> administratorAccountSettings
)
: IUsersAndRolesInitializer
{
    public async Task InitAsync(CancellationToken cancellationToken)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        await dbContext
            .Database
            .ExecuteSqlRawAsync
            (
                $"SELECT pg_advisory_xact_lock({Init.InitUsersAndRolesTransactionCode})",
                cancellationToken
            );


        #region Init roles

            foreach (var roleName in Init.Roles)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (await rolesManager.RoleExistsAsync(roleName))
                {
                    continue;
                }
                
                var result = await rolesManager.CreateAsync(new CreatureRoleDbo(roleName));

                if (!result.Succeeded)
                {
                    var errors = string.Join
                    (
                        "; ",
                        result.Errors.Select(e => $"{ e.Code }: { e.Description }")
                    );

                    throw new InvalidOperationException($"Failed to create role '{roleName}': {errors}");
                }
            }

        #endregion
        
        logger.LogCritical
        (
            "Admin account { login } : { password }",
            administratorAccountSettings.Value.Login,
            administratorAccountSettings.Value.Password
        );
        
        await transaction.CommitAsync(cancellationToken);
    }
}