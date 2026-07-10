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

using FluentAssertions;
using ratel_backend_users_client.Services.Implementations;
using ratel_backend_users_e2e.Auxilliary;

namespace ratel_backend_users_e2e.Tests;

/// <summary>
/// Tests, related to registration
/// </summary>
public class RegistrationTests(ApiFixture fixture) : IClassFixture<ApiFixture>
{
    /// <summary>
    /// Available login must be recognized as available
    /// </summary>
    [Fact]
    public async Task AvailableLoginMustBeAvailable()
    {
        #region Arrange

        var availableLogin = $"Available_Login_{ Guid.NewGuid() }";

        #endregion

        #region Act

        var isAvailable = await fixture.RegistrationClient.IsLoginAvailableAsync(availableLogin);

        #endregion

        #region Assert

        isAvailable.Should().BeTrue();

        #endregion
    }
    
}