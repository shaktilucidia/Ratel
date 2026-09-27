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

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ratel_shared_auxiliary.Helpers;

/// <summary>
/// Helper with useful stuff for DI
/// </summary>
public static class DiHelper
{
    /// <summary>
    /// Register settings class
    /// </summary>
    public static void RegisterSettings<TSettings>(WebApplicationBuilder builder) where TSettings : class
    {
        builder.Services.Configure<TSettings>(builder.Configuration.GetSection(typeof(TSettings).Name));
        builder.Services.AddSingleton<TSettings>
        (
            provider => provider
                .GetRequiredService<IOptions<TSettings>>()
                .Value
        );
    }
}