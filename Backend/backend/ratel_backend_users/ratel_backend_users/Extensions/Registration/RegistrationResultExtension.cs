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

using Microsoft.AspNetCore.Mvc;
using ratel_backend_users.Enums.Registration;

namespace ratel_backend_users.Extensions.Registration;

/// <summary>
/// Extension to work with registration result
/// </summary>
public static class RegistrationResultExtension
{
    /// <summary>
    /// Convert registration result to action result
    /// </summary>
    public static IActionResult ToActionResult(this ControllerBase controller, RegistrationResult result)
    {
        return result switch
        {
            RegistrationResult.Created => controller.Created(),

            RegistrationResult.FailedLoginEmpty => controller.UnprocessableEntity("Login empty"),
            
            RegistrationResult.FailedLoginTaken => controller.Conflict(),
            
            RegistrationResult.FailedPasswordEmpty => controller.UnprocessableEntity("Password empty"),
            
            RegistrationResult.FailedPasswordTooWeak => controller.UnprocessableEntity("Password too weak"),
            
            _ => throw new ArgumentOutOfRangeException(nameof(result))
        };
    }
}