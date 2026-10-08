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
using ratel_backend_creatures_dtos.Sessions.DTOs;
using ratel_backend_creatures_dtos.Sessions.Enums;
using ratel_backend_creatures_dtos.Sessions.Responses;

namespace ratel_backend_creatures_dtos.Sessions.Extensions;

/// <summary>
/// Extension to work with session creation result
/// </summary>
public static class SessionCreationResultExtension
{
    /// <summary>
    /// Problem with this URL is being returned when some validations failed during session creation
    /// </summary>
    private const string ValidationFailedProblemType = "https://ratel.chat/problems/session/create/failed-validation";
    
    /// <summary>
    /// Problem with this URL is being returned when creature is not found during session creation
    /// </summary>
    private const string CreatureNotFoundProblemType = "https://ratel.chat/problems/session/create/creature-not-found";
    
    /// <summary>
    /// Problem with this URL is being returned when password is wrong during session creation
    /// </summary>
    private const string WrongPasswordProblemType = "https://ratel.chat/problems/session/create/wrong-password";
    
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
        IsWrong
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
        [ValidationError.IsWrong] = "is_wrong"
    };
    
    /// <summary>
    /// Fields names, related to errors
    /// </summary>
    private static readonly Dictionary<SessionCreationError, string> _errorsToFieldsNames = new ()
    {
        [SessionCreationError.LoginEmpty] = _fieldsNames[FieldName.Login],
        [SessionCreationError.CreatureNotFound] = _fieldsNames[FieldName.Login],
        [SessionCreationError.PasswordEmpty] = _fieldsNames[FieldName.Password],
        [SessionCreationError.WrongPassword] = _fieldsNames[FieldName.Password]
    };
    
    /// <summary>
    /// Descriptions, related to errors
    /// </summary>
    private static readonly Dictionary<SessionCreationError, string> _errorsToDescriptions = new ()
    {
        [SessionCreationError.LoginEmpty] = _validationErrors[ValidationError.IsEmpty],
        [SessionCreationError.CreatureNotFound] = _validationErrors[ValidationError.IsWrong],
        [SessionCreationError.PasswordEmpty] = _validationErrors[ValidationError.IsEmpty],
        [SessionCreationError.WrongPassword] = _validationErrors[ValidationError.IsWrong]
    };
    
    /// <summary>
    /// Convert registration result to action result
    /// </summary>
    public static IActionResult ToActionResult
    (
        this ControllerBase controller,
        IReadOnlySet<SessionCreationError> errors,
        string refreshToken,
        string accessToken,
        DateTime accessTokenValidTill
    )
    {
        ArgumentNullException.ThrowIfNull(errors);
        
        if (!errors.Any())
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken);
            ArgumentException.ThrowIfNullOrWhiteSpace(accessToken);
            
            return controller.Ok
            (
                new CreateSessionResponse()
                {
                    SessionData = new CreateSessionResponseDto()
                    {
                        RefreshToken = refreshToken,
                        AccessToken = accessToken,
                        AccessTokenValidTill = accessTokenValidTill
                    }
                }
            );
        }

        if (errors.Contains(SessionCreationError.LoginEmpty) || errors.Contains(SessionCreationError.PasswordEmpty))
        {
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
                    Title = "Session creation failed",
                    Type = ValidationFailedProblemType
                }
            );
        }

        if (errors.Contains(SessionCreationError.CreatureNotFound))
        {
            return controller.NotFound
            (
                new ProblemDetails()
                {
                    Type = CreatureNotFoundProblemType,
                    Title = "Creature not found"
                }
            );
        }
        
        if (errors.Contains(SessionCreationError.WrongPassword))
        {
            return controller.BadRequest
            (
                new ProblemDetails()
                {
                    Type = WrongPasswordProblemType,
                    Title = "Wrong password",
                    Extensions =
                    {
                        ["code"] = "invalid_credentials"
                    }
                }
            );
        }

        throw new NotImplementedException("Bug in code, probably new error code was added, but not processed");
    }
}