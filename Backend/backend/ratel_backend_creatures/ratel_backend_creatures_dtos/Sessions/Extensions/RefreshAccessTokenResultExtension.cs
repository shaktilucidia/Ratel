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
/// Extension to work with access token refresh result
/// </summary>
public static class RefreshAccessTokenResultExtension
{
    /// <summary>
    /// Problem with this URL is being returned when some validations failed during access token refresh
    /// </summary>
    private const string ValidationFailedProblemType = "https://ratel.chat/problems/session/refresh/failed-validation";
    
    /// <summary>
    /// Fields, on what validation may fail
    /// </summary>
    private enum FieldName
    {
        SessionId,
        RefreshToken
    }
    
    /// <summary>
    /// Possible validation errors, independent of fields
    /// </summary>
    private enum ValidationError
    {
        IsEmpty
    }
    
    /// <summary>
    /// Machine-readable fields names for validation
    /// </summary>
    private static readonly Dictionary<FieldName, string> _fieldsNames = new ()
    {
        [FieldName.SessionId] = "session_id",
        [FieldName.RefreshToken] = "token"
    };
    
    /// <summary>
    /// Machine-readable validation errors
    /// </summary>
    private static readonly Dictionary<ValidationError, string> _validationErrors = new ()
    {
        [ValidationError.IsEmpty] = "is_empty"
    };
    
    /// <summary>
    /// Fields names, related to errors
    /// </summary>
    private static readonly Dictionary<RefreshAccessTokenError, string> _errorsToFieldsNames = new ()
    {
        [RefreshAccessTokenError.SessionIdIsEmpty] = _fieldsNames[FieldName.SessionId],
        [RefreshAccessTokenError.TokenIsEmpty] = _fieldsNames[FieldName.RefreshToken]
    };
    
    /// <summary>
    /// Descriptions, related to errors
    /// </summary>
    private static readonly Dictionary<RefreshAccessTokenError, string> _errorsToDescriptions = new ()
    {
        [RefreshAccessTokenError.SessionIdIsEmpty] = _validationErrors[ValidationError.IsEmpty],
        [RefreshAccessTokenError.TokenIsEmpty] = _validationErrors[ValidationError.IsEmpty]
    };
    
    /// <summary>
    /// Tokens refresh result to action result
    /// </summary>
    public static IActionResult ToActionResult
    (
        this ControllerBase controller,
        IReadOnlySet<RefreshAccessTokenError> errors,
        string? refreshToken,
        string? accessToken,
        DateTime? accessTokenValidTill
    )
    {
        ArgumentNullException.ThrowIfNull(errors);
        
        if (!errors.Any())
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken);
            ArgumentException.ThrowIfNullOrWhiteSpace(accessToken);
            ArgumentNullException.ThrowIfNull(accessTokenValidTill);
            
            return controller.Ok
            (
                new RefreshAccessTokenResponse()
                {
                    RefreshedTokens = new RefreshAccessTokenResponseDto()
                    {
                        RefreshToken = refreshToken,
                        AccessToken = accessToken,
                        AccessTokenValidTill = accessTokenValidTill.Value
                    }
                }
            );
        }
        
        if (errors.Contains(RefreshAccessTokenError.SessionIdIsEmpty) || errors.Contains(RefreshAccessTokenError.TokenIsEmpty))
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
                    Title = "Token refresh failed",
                    Type = ValidationFailedProblemType
                }
            );
        }
        
        if (errors.Contains(RefreshAccessTokenError.UnableToRefresh))
        {
            return controller.Unauthorized();
        }

        throw new NotImplementedException("Bug in code, probably new error code was added, but not processed");
    }
}