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
using ratel_backend_creatures_dtos.Constants;
using ratel_backend_creatures_dtos.Registration.Enums;
using ratel_backend_creatures.DAO.Models.Creatures;
using ratel_backend_creatures.Metrics;
using ratel_backend_creatures.Models.Business.Creatures;
using ratel_backend_creatures.Services.Abstract;
using ratel_shared_auxiliary.Extensions;
using ratel_shared_auxiliary.UoW.Abstract;
using ratel_shared_observability.Metrics;

namespace ratel_backend_creatures.Services.Implementation;

public class RegistrationService
(
    ILogger<RegistrationService> logger,
    UserManager<CreatureDbo> userManager,
    IUnitOfWork unitOfWork
) : IRegistrationService
{
    public async Task<bool> IsLoginAvailableAsync
    (
        string login,
        CancellationToken cancellationToken
    )
    {
        _ = login ?? throw new ArgumentNullException(nameof(login), "Login must be specified, at least empty string.");

        return await userManager.FindByNameAsync(login) == null;
    }

    public async Task<Tuple<IReadOnlySet<RegistrationError>, Creature?>> RegisterAsync
    (
        string login,
        string password,
        CancellationToken cancellationToken
    )
    {
        using var _ = new MetricsTimer(RegistrationMetrics.RegistrationDuration);

        var errors = new HashSet<RegistrationError>();

        if (string.IsNullOrWhiteSpace(login))
        {
            errors.AddUnique(RegistrationError.FailedLoginEmpty);
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            errors.AddUnique(RegistrationError.FailedPasswordEmpty);
        }

        if (await userManager.FindByNameAsync(login) != null)
        {
            errors.Add(RegistrationError.FailedLoginTaken);
        }

        await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        var creatureDbo = new CreatureDbo()
        {
            UserName = login,
            SecurityStamp = Guid.NewGuid().ToString() // TODO: Is this secure?
        };

        var result = await userManager.CreateAsync(creatureDbo, password);
        if (!result.Succeeded)
        {
            // Mostly probably password is too weak
            errors.AddUnique(RegistrationError.FailedPasswordTooWeak);
            
            logger.LogError
            (
                "Failed to create account with login {Login}, errors {Errors}",
                login,
                result.GetErrorsAndDescriptions()
            );
        }

        if (errors.Count > 0)
        {
            RegistrationMetrics.RegistrationAttemptsCount.Add(1, new KeyValuePair<string, object?>("is_successful", false));
            return new Tuple<IReadOnlySet<RegistrationError>, Creature?>(errors, null);
        }

        var creature = new Creature(creatureDbo);

        #region Add roles
        
            await AddRoleToCreatureAsync(creature.Id, [ ServerRole.User ]);
        
        #endregion

        if (errors.Count > 0)
        {
            RegistrationMetrics.RegistrationAttemptsCount.Add(1, new KeyValuePair<string, object?>("is_successful", false));
            throw new InvalidOperationException("Bug in a code, successful registration, but errors aren't empty!");
        }

        await transaction.CommitAsync(cancellationToken);

        RegistrationMetrics.RegistrationAttemptsCount.Add(1, new KeyValuePair<string, object?>("is_successful", true));

        return new Tuple<IReadOnlySet<RegistrationError>, Creature?>(errors, creature);
    }

    public async Task AddRoleToCreatureAsync(Guid creatureId, IReadOnlyCollection<string> roles)
    {
        var creature = (await userManager.FindByIdAsync(creatureId.ToString()))
                       ??
                       throw new ArgumentException($"Creature with ID = { creatureId } was not found", nameof(creatureId));

        var currentRoles = await userManager.GetRolesAsync(creature);

        var rolesToAdd = roles
            .Except(currentRoles, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (rolesToAdd.Count is 0)
        {
            return;
        }

        var result = await userManager.AddToRolesAsync(creature, rolesToAdd);
        
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Failed to assign roles to creature { creature.UserName }: { result.GetErrorsAndDescriptions() }");
        }
    }
}
