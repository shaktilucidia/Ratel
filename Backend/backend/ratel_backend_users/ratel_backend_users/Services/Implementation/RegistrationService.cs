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
using ratel_backend_users.Enums.Registration;
using ratel_backend_users.Models.Business.Creatures;
using ratel_backend_users.Services.Abstract;

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

    public async Task<Tuple<RegistrationResult, Creature?>> RegisterAsync(string login, string password)
    {
        if (string.IsNullOrWhiteSpace(login))
        {
            return new Tuple<RegistrationResult, Creature?>(RegistrationResult.FailedLoginEmpty, null);
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            return new Tuple<RegistrationResult, Creature?>(RegistrationResult.FailedPasswordEmpty, null);
        }

        if (await userManager.FindByNameAsync(login) != null)
        {
            return new Tuple<RegistrationResult, Creature?>(RegistrationResult.FailedLoginTaken, null);
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
            return new Tuple<RegistrationResult, Creature?>(RegistrationResult.FailedPasswordTooWeak, null);
        }

        var creature = new Creature(creatureDbo);

        // TODO: Add roles

        return new Tuple<RegistrationResult, Creature?>(RegistrationResult.Created, creature);
    }
}
