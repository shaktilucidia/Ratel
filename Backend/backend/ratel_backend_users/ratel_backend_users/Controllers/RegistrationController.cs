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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ratel_backend_users_dtos.Registration.DTOs;
using ratel_backend_users_dtos.Registration.Requests;
using ratel_backend_users_dtos.Registration.Responses;
using ratel_backend_users.Extensions.Registration;
using ratel_backend_users.Services.Abstract;

namespace ratel_backend_users.Controllers;

/// <summary>
/// Controller, related to users registration
/// </summary>
[Route("api/users/registration")]
[ApiController]
public class RegistrationController
(
    IRegistrationService registrationService
) : ControllerBase
{
    /// <summary>
    /// Check is login available?
    /// </summary>
    [AllowAnonymous]
    [Route("is_login_available")]
    [HttpPost]
    public async Task<ActionResult<IsLoginAvailableResponse>> IsLoginAvailableAsync([FromBody] IsLoginAvailableRequest request)
    {
        _ = request ?? throw new ArgumentNullException(nameof(request), "Request must be provided");
        
        return Ok
        (
            new IsLoginAvailableResponse()
            {
                AvailabilityData = new IsLoginAvailableResponseDto()
                {
                    IsAvailable = await registrationService.IsLoginAvailableAsync(request.LoginData.Login)
                }
            }
        );
    }

    /// <summary>
    /// Register a creature
    /// </summary>
    [AllowAnonymous]
    [Route("register")]
    [HttpPost]
    public async Task<IActionResult> RegisterAsync([FromBody] CreatureRegistrationRequest request)
    {
        _ = request ?? throw new ArgumentNullException(nameof(request), "Request must be provided");

        return this
            .ToActionResult
            (
                (await registrationService.RegisterAsync(request.RegistrationData.Login, request.RegistrationData.Password))
                .Item1
            );
    }
}