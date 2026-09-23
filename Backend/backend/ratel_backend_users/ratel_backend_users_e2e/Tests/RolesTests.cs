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

using ratel_backend_users_dtos.Constants;
using ratel_backend_users_e2e.Auxilliary;
using Shouldly;

namespace ratel_backend_users_e2e.Tests;

/// <summary>
/// Tests, related to roles
/// </summary>
public class RolesTests
(
    ApiFixture fixture
) : IClassFixture<ApiFixture>
{
    /// <summary>
    /// Freshly-registered creature must have at least User role
    /// </summary>
    [Fact]
    public async Task RegisteredCreatureMustHaveAtLeastUserRole()
    {
        #region Arrange

            var creatureLogin = LoginsHelper.GenerateLogin();
        
        #endregion
        
        #region Act

            await fixture.RegistrationClient.RegisterAsync
            (
                creatureLogin, PasswordsHelper.GeneratePassword()
            );

            var creatureId = (await fixture.CreaturesClient.GetIdsByLoginsAsync
            (
                [
                    creatureLogin
                ]
            ))
            .Single()
            .Value!
            .Value;

            var creaturesRoles = await fixture.RolesClient.GetRolesByCreaturesIdsAsync
            (
                [
                    creatureId
                ]
            );
            
        #endregion

        #region Assert
        
        creaturesRoles.ShouldNotBeEmpty();
        creaturesRoles.Count.ShouldBe(1);
        creaturesRoles.ShouldContainKey(creatureId);

        creaturesRoles[creatureId].ShouldNotBeNull();
        creaturesRoles[creatureId].ShouldNotBeEmpty();
        creaturesRoles[creatureId].ShouldContain(ServerRole.User);
        
        #endregion
    }
}