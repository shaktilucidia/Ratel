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
using ratel_backend_users.DAO.Models.Creatures;
using ratel_backend_users.Metrics;
using ratel_backend_users.Models.Business.Creatures;
using ratel_backend_users.Services.Abstract;
using ratel_backend_users_dtos.Registration.Enums;
using ratel_shared_auxiliary.Extensions;
using ratel_shared_observability.Metrics;

namespace ratel_backend_users.Services.Implementation;

public class RegistrationService
(
    ILogger<RegistrationService> logger,
    UserManager<CreatureDbo> userManager
) : IRegistrationService
{
    public async Task<bool> IsLoginAvailableAsync(string login)
    {
        _ = login ?? throw new ArgumentNullException(nameof(login), "Login must be specified, at least empty string.");

        // TODO: Delete it, I'm just testing logging
        logger
            .LogInformation
            (
                "Checking if login \"{login}\" is available",
                login
            );

        return await userManager.FindByNameAsync(login) == null;
    }

    public async Task<Tuple<IReadOnlySet<RegistrationError>, Creature?>> RegisterAsync
    (
        string login,
        string password
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
        }

        if (errors.Any())
        {
            RegistrationMetrics.RegistrationAttemptsCount.Add(1, new KeyValuePair<string, object?>("is_successful", false));
            return new Tuple<IReadOnlySet<RegistrationError>, Creature?>(errors, null);
        }

        var creature = new Creature(creatureDbo);

        // TODO: Add roles

        if (errors.Any())
        {
            RegistrationMetrics.RegistrationAttemptsCount.Add(1, new KeyValuePair<string, object?>("is_successful", false));
            throw new InvalidOperationException("Bug in a code, successfull registration, but errors aren't empty!");
        }

        RegistrationMetrics.RegistrationAttemptsCount.Add(1, new KeyValuePair<string, object?>("is_successful", true));
        return new Tuple<IReadOnlySet<RegistrationError>, Creature?>(errors, creature);
    }
}
