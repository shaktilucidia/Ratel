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

using ratel_backend_users_dtos.Registration.Enums;
using ratel_backend_users_e2e.Auxilliary;
using Shouldly;

namespace ratel_backend_users_e2e.Tests;

/// <summary>
/// Tests, related to registration
/// </summary>
public class RegistrationTests
(
    ApiFixture fixture
) : IClassFixture<ApiFixture>
{
    /// <summary>
    /// Available login must be recognized as available
    /// </summary>
    [Fact]
    public async Task AvailableLoginMustBeAvailable()
    {
        #region Act

            var isAvailable = await fixture.RegistrationClient.IsLoginAvailableAsync(LoginsHelper.GenerateUniqueLogin());

        #endregion

        #region Assert

            isAvailable.ShouldBe(true);

        #endregion
    }

    /// <summary>
    /// Unavailable login must be recognized as unavailable
    /// </summary>
    [Fact]
    public async Task UnavailableLoginMustBeUnavailable()
    {
        #region Arrange

            var unavailableLogin = LoginsHelper.GenerateUniqueLogin();

            await fixture.RegistrationClient.RegisterAsync(unavailableLogin, PasswordsHelper.GenerateCorrectPassword());

        #endregion

        #region Act

            var isAvailable = await fixture.RegistrationClient.IsLoginAvailableAsync(unavailableLogin);

        #endregion

        #region Assert

            isAvailable.ShouldBe(false);

        #endregion
    }

    /// <summary>
    /// Empty login must lead to "empty login" validation error
    /// </summary>
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task EmptyLoginMustProduceValidationError(bool isUseCorrectPassword)
    {
        #region Act

            var registrationErrors = await fixture.RegistrationClient.RegisterAsync
            (
                string.Empty,
                PasswordsHelper.GeneratePassword(isUseCorrectPassword)
            );

        #endregion

        #region Assert

            registrationErrors.ShouldContain(RegistrationError.FailedLoginEmpty);

        #endregion
    }

    /// <summary>
    /// Empty password must lead to "empty password" validation error
    /// </summary>
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task EmptyPasswordMustProduceValidationError(bool isUseCorrectLogin)
    {
        #region Act

            var registrationErrors = await fixture.RegistrationClient.RegisterAsync
            (
                LoginsHelper.GenerateLogin(isUseCorrectLogin), string.Empty
            );

        #endregion

        #region Assert

            registrationErrors.ShouldContain(RegistrationError.FailedPasswordEmpty);

        #endregion
    }

    /// <summary>
    /// Weak password must lead to "weak password" validation error
    /// </summary>
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task WeakPasswordMustProduceValidationError(bool isUseCorrectLogin)
    {
        #region Act

            var registrationErrors = await fixture.RegistrationClient.RegisterAsync
            (
                LoginsHelper.GenerateLogin(isUseCorrectLogin),
                PasswordsHelper.GeneratePassword(false)
            );

        #endregion

        #region Assert

            registrationErrors.ShouldContain(RegistrationError.FailedPasswordTooWeak);

        #endregion
    }
}
