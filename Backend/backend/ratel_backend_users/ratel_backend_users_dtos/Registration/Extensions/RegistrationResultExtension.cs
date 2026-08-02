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
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ratel_backend_users_dtos.Registration.Enums;
using ratel_shared_auxiliary.Extensions;

namespace ratel_backend_users_dtos.Registration.Extensions;

/// <summary>
/// Extension to work with registration result
/// </summary>
public static class RegistrationResultExtension
{
    /// <summary>
    /// Problem with this URL is being returned when login is taken
    /// </summary>
    private const string LoginIsTakenProblemType = "https://ratel.chat/problems/registration/login-is-taken";

    /// <summary>
    /// Problem with this URL is being returned when some validations failed during registration
    /// </summary>
    private const string ValidationFailedProblemType = "https://ratel.chat/problems/registration/failed-validation";

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
    /// Maps pairs "field and description" to registration errors
    /// </summary>
    private static readonly IReadOnlyDictionary<(string Field, string Description), RegistrationError> _validationProblemsToErrors =
        _errorsToFieldsNames
        .Keys
        .ToDictionary
        (
            error =>
            (
                Field: _errorsToFieldsNames[error],
                Description: _errorsToDescriptions[error]
            ),
            error => error
        );

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
                    .Conflict
                    (
                        new ProblemDetails
                        {
                            Type = LoginIsTakenProblemType,
                            Title = "Login is taken",
                            Status = StatusCodes.Status409Conflict
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
                Type = ValidationFailedProblemType
            }
        );
    }

    public static async Task<IReadOnlySet<RegistrationError>> ToRegistrationResult
    (
        this HttpResponseMessage response,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(response);

        return response.StatusCode switch
        {
            HttpStatusCode.Created => new HashSet<RegistrationError>(),

            HttpStatusCode.Conflict => await ParseConflictAsync(response, cancellationToken),

            HttpStatusCode.UnprocessableEntity => await ParseValidationErrorsAsync(response, cancellationToken),

            _ => throw new InvalidOperationException
                (
                    "Unexpected HTTP status code in registration response: { (int)response.StatusCode } ({ response.StatusCode })"
                )
        };
    }

    private static async Task<IReadOnlySet<RegistrationError>> ParseConflictAsync
    (
        HttpResponseMessage response,
        CancellationToken cancellationToken = default
    )
    {
        var problemDetails = await response
            .Content
            .ReadFromJsonAsync<ProblemDetails>(cancellationToken)
            ??
            throw new InvalidOperationException("Registration conflict response contains no ProblemDetails");

        return problemDetails.Type switch
        {
            LoginIsTakenProblemType => new HashSet<RegistrationError>
            {
                RegistrationError.FailedLoginTaken
            },

            _ => throw new InvalidOperationException($"Unknown registration conflict problem type: '{ problemDetails.Type }'")
        };
    }

    private static async Task<IReadOnlySet<RegistrationError>> ParseValidationErrorsAsync
    (
        HttpResponseMessage response,
        CancellationToken cancellationToken = default
    )
    {
        var problemDetails = await response
            .Content
            .ReadFromJsonAsync<ValidationProblemDetails>(cancellationToken)
            ??
            throw new InvalidOperationException("Registration validation response contains no ValidationProblemDetails");


        if (problemDetails.Type != ValidationFailedProblemType)
        {
            throw new InvalidOperationException($"Unknown registration validation problem type: '{ problemDetails.Type }'");
        }

        var result = new HashSet<RegistrationError>();

        foreach (var (field, descriptions) in problemDetails.Errors)
        {
            foreach (var description in descriptions)
            {
                if (!_validationProblemsToErrors.TryGetValue((field, description), out var error))
                {
                    throw new InvalidOperationException
                    (
                        $"Unknown registration validation error: Field='{ field }', Description='{ description }'"
                    );
                }

                result.AddUnique(error);
            }
        }

        if (!result.Any())
        {
            throw new InvalidOperationException("Registration validation response contains no errors");
        }

        return result;
    }
}
