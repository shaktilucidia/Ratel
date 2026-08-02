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

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ratel_backend_users_dtos.Registration.Enums;

namespace ratel_backend_users_dtos.Registration.Extensions;

/// <summary>
/// Extension to work with registration result
/// </summary>
public static class RegistrationResultExtension
{
    /// <summary>
    /// Fields, on what validation may fail
    /// </summary>
    private enum FieldName
    {
        Login,
        Password
    }

    /// <summary>
    /// Possible validation errors, independent of fields
    /// </summary>
    private enum ValidationError
    {
        IsEmpty,
        TooWeak
    }

    /// <summary>
    /// Machine-readable fields names for validation
    /// </summary>
    private static readonly Dictionary<FieldName, string> _fieldsNames = new ()
    {
        [FieldName.Login] = "login",
        [FieldName.Password] = "password"
    };

    /// <summary>
    /// Machine-readable validation errors
    /// </summary>
    private static readonly Dictionary<ValidationError, string> _validationErrors = new ()
    {
        [ValidationError.IsEmpty] = "is_empty",
        [ValidationError.TooWeak] = "too_weak"
    };

    /// <summary>
    /// Fields names, related to errors
    /// </summary>
    private static readonly Dictionary<RegistrationError, string> _errorsToFieldsNames = new ()
    {
        [RegistrationError.FailedLoginEmpty] = _fieldsNames[FieldName.Login],
        [RegistrationError.FailedPasswordEmpty] = _fieldsNames[FieldName.Password],
        [RegistrationError.FailedPasswordTooWeak] = _fieldsNames[FieldName.Password]
    };

    /// <summary>
    /// Descriptions, related to erros
    /// </summary>
    private static readonly Dictionary<RegistrationError, string> _errorsToDescriptions = new ()
    {
        [RegistrationError.FailedLoginEmpty] = _validationErrors[ValidationError.IsEmpty],
        [RegistrationError.FailedPasswordEmpty] = _validationErrors[ValidationError.IsEmpty],
        [RegistrationError.FailedPasswordTooWeak] = _validationErrors[ValidationError.TooWeak]
    };

    /// <summary>
    /// Convert registration result to action result
    /// </summary>
    public static IActionResult ToActionResult(this ControllerBase controller, IReadOnlySet<RegistrationError> errors)
    {
        if (!errors.Any())
        {
            return controller.Created();
        }

        if (errors.Contains(RegistrationError.FailedLoginTaken))
        {
            return controller
                    .UnprocessableEntity
                    (
                        new ProblemDetails
                        {
                            Type = "https://ratel.chat/problems/registration/login-is-taken",
                            Title = "Login is empty",
                            Status = StatusCodes.Status422UnprocessableEntity
                        }
                    );
        }

        if
        (
            errors
            .Overlaps
            (
                [
                        RegistrationError.FailedLoginTaken
                ]
            )
        )
        {
            throw new InvalidOperationException($"Bug in code. Some non-validation errors left unprocessed! Errors: [{ string.Join(", ", errors) }]");
        }

        return controller.UnprocessableEntity
        (
            new ValidationProblemDetails
            (
                errors
                .GroupBy
                (
                    e => _errorsToFieldsNames[e]
                )
                .ToDictionary
                (
                    e => e.Key,
                    e => e.Select
                    (
                        e => _errorsToDescriptions[e]
                    )
                    .ToArray()
                )
            )
            {
                Status = StatusCodes.Status422UnprocessableEntity,
                Title = "Registration failed",
                Type = "https://ratel.im/problems/registration/failed-validation"
            }
        );
    }

    // /// <summary>
    // /// Convert registration server response to registration result
    // /// </summary>
    // public static RegistrationResult ToResult(this HttpResponseMessage response)
    // {
    //     return response.StatusCode switch
    //     {
    //         System.Net.HttpStatusCode.Created => RegistrationResult.Created,

    //         System.Net.HttpStatusCode.UnprocessableEntity => RegistrationResult.FailedPasswordTooWeak,

    //         System.Net.HttpStatusCode.Conflict => RegistrationResult.FailedLoginTaken,

    //         _ => throw new ArgumentOutOfRangeException(nameof(response))
    //     };
    // }
}
