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

using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace ratel_shared_auxiliary.Controllers;

/// <summary>
/// Inherit all controllers from this class
/// </summary>
public abstract class RatelControllerBase : ControllerBase
{
    /// <summary>
    /// Request came from this address
    /// </summary>
    protected IPAddress RequestSource
    {
        get
        {
            var source=
                HttpContext
                    .Connection
                    .RemoteIpAddress
                    ??
                    throw new InvalidOperationException("Client IP address is unavailable");

            if (source.IsIPv4MappedToIPv6)
            {
                source = source.MapToIPv4();
            }

            return source;
        }
    }

    /// <summary>
    /// Requestor user agent
    /// </summary>
    protected string UserAgent
    {
        get
        {
            var userAgent = Request.Headers.UserAgent.ToString();

            return string.IsNullOrWhiteSpace(userAgent)
                ? "Missing or empty UserAgent header"
                : userAgent;
        }
    }
}