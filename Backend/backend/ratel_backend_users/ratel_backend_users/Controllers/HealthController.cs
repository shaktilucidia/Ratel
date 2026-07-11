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
using ratel_backend_users.Services.Abstract;

namespace ratel_backend_users.Controllers;

/// <summary>
/// Healthcheck controller (for k8s)
/// </summary>
[Route("api/users/health")]
[ApiController]
public class HealthController
(
    IHealthService healthService
)
: ControllerBase
{
    /// <summary>
    /// Checks if service has started
    /// </summary>
    /// <returns>200 if service started</returns>
    [AllowAnonymous]
    [HttpGet]
    [Route("is_started")]
    public async Task<ActionResult> IsStartedAsync()
    {
        return Ok();
    }
    
    /// <summary>
    /// Checks is service alive
    /// </summary>
    /// <returns>200 if service alive</returns>
    [AllowAnonymous]
    [HttpGet]
    [Route("is_alive")]
    public async Task<ActionResult> IsAliveAsync()
    {
        return Ok();
    }
    
    /// <summary>
    /// Checks is service ready
    /// </summary>
    /// <returns>200 if service is ready</returns>
    [AllowAnonymous]
    [HttpGet]
    [Route("is_ready")]
    public async Task<ActionResult> IsReadyAsync()
    {
        return
            await healthService.IsReadyAsync()
                ? Ok()
                : StatusCode(StatusCodes.Status503ServiceUnavailable);
    }
}