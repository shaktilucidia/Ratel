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
using ratel_backend_users_dtos.Constants;
using ratel_backend_users.Constants;
using ratel_backend_users.DAO.Contexts;
using ratel_backend_users.DAO.Models.Creatures;
using ratel_backend_users.DAO.Services.Abstract;
using ratel_backend_users.Models.Settings;
using ratel_backend_users.Services.Abstract;
using ratel_shared_auxiliary.Extensions;
using ratel_shared_auxiliary.UoW.Abstract;

namespace ratel_backend_users.Services.Implementation;

public class UsersAndRolesInitializer
(
    IUnitOfWork unitOfWork,
    MainDbContext dbContext,
    RoleManager<CreatureRoleDbo> rolesManager,
    ILogger<UsersAndRolesInitializer> logger,
    AdministratorAccountSettings administratorAccountSettings,
    UserManager<CreatureDbo> userManager,
    IRolesDao rolesDao,
    IRegistrationService registrationService
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

        #region Create administrative account if no admins exists

            if
            (
                !string.IsNullOrWhiteSpace(administratorAccountSettings.Login)
                &&
                !string.IsNullOrWhiteSpace(administratorAccountSettings.Password)
            )
            {
                await CreateAdministrativeAccountAsync(cancellationToken);
            }
            else
            {
                logger.LogInformation
                (
                    "Skipping administrative account creation, administrator login and/or password aren't set"
                );
            }
            
        #endregion
        
        await transaction.CommitAsync(cancellationToken);
    }

    private async Task CreateAdministrativeAccountAsync(CancellationToken cancellationToken)
    {
        var administratorRole =
            await rolesManager.FindByNameAsync(ServerRole.Administrator)
            ??
            throw new InvalidOperationException("Bug in code, administrator role is missing");
    
        if (!await rolesDao.IsCreaturesWithRoleExistsAsync(administratorRole.Id, cancellationToken))
        {
            if (await userManager.FindByNameAsync(administratorAccountSettings.Login) is not null)
            {
                throw new InvalidOperationException
                (
                    $"Trying to create administrative account, but administrator login \"{ administratorAccountSettings.Login }\" is taken by ordinary user"
                );
            }
            
            logger.LogInformation
            (
                "Creating administrative account {Login}",
                administratorAccountSettings.Login
            );

            var adminDbo = new CreatureDbo()
            {
                UserName = administratorAccountSettings.Login,
                SecurityStamp = Guid.NewGuid().ToString() // TODO: Is this secure?
            };

            var result = await userManager.CreateAsync(adminDbo, administratorAccountSettings.Password);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException
                (
                    $"Failed to create administrative account, called { administratorAccountSettings.Login }" +
                    $"Errors: { result.GetErrorsAndDescriptions() }"
                );
            }
        
            await registrationService.AddRoleToCreatureAsync
            (
                adminDbo.Id, 
                [
                    ServerRole.User,
                    ServerRole.Administrator
                ]
            );
        
            logger.LogInformation
            (
                "Created administrative account {Login}",
                administratorAccountSettings.Login
            );
        }
    }
}