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
using Microsoft.AspNetCore.Identity;
using ratel_backend_creatures_dtos.Sessions.Enums;
using ratel_backend_creatures.DAO.Models.Creatures;
using ratel_backend_creatures.DAO.Models.Sessions;
using ratel_backend_creatures.DAO.Services.Abstract;
using ratel_backend_creatures.Enums.Sessions;
using ratel_backend_creatures.Helpers.Sessions;
using ratel_backend_creatures.Services.Abstract;
using ratel_shared_auxiliary.Extensions;
using ratel_shared_auxiliary.Helpers;
using ratel_shared_auxiliary.Services.Abstract;
using ratel_shared_auxiliary.UoW.Abstract;

namespace ratel_backend_creatures.Services.Implementation;

public class SessionsService
(
    UserManager<CreatureDbo> userManager,
    SignInManager<CreatureDbo> signInManager,
    IUnitOfWork unitOfWork,
    ITimeProvider timeProvider,
    ISessionsDao sessionsDao,
    IAccessTokensService accessTokensService
) : ISessionsService
{
    public async Task<Tuple<IReadOnlySet<SessionCreationError>, string?, string?, DateTime?>> CreateAsync
    (
        string login,
        string password,
        IPAddress source,
        string deviceInfo,
        CancellationToken cancellationToken
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(deviceInfo);
        
        var errors = new HashSet<SessionCreationError>();
        
        if (string.IsNullOrWhiteSpace(login))
        {
            errors.AddUnique(SessionCreationError.LoginEmpty);
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            errors.AddUnique(SessionCreationError.PasswordEmpty);
        }
        
        if (errors.Count > 0)
        {
            return new Tuple<IReadOnlySet<SessionCreationError>, string?, string?, DateTime?>
            (
                errors,
                null,
                null,
                null
            );
        }
        
        var creature = await userManager.FindByNameAsync(login);

        if (creature is null)
        {
            return new Tuple<IReadOnlySet<SessionCreationError>, string?, string?, DateTime?>
            (
                new HashSet<SessionCreationError>()
                {
                    SessionCreationError.CreatureNotFound
                },
                null,
                null,
                null
            );
        }
        
        if (await userManager.GetTwoFactorEnabledAsync(creature))
        {
            throw new NotImplementedException("2FA is not implemented yet");
        }

        var result = await signInManager.CheckPasswordSignInAsync
        (
            creature,
            password,
            lockoutOnFailure: false
        );

        if (!result.Succeeded)
        {
            return new Tuple<IReadOnlySet<SessionCreationError>, string?, string?, DateTime?>
            (
                new HashSet<SessionCreationError>()
                {
                    SessionCreationError.WrongPassword
                },
                null,
                null,
                null
            );
        }
        
        #region Creating session
        
            await using var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

            var now = timeProvider.GetNow();
            var sessionId = SecureGuidGenerator.Generate();
            var refreshToken = SessionsRefreshTokensHelper.Create();
            
            var session = new CreatureSessionDbo()
            {
                Id = sessionId,
                CreatureId = creature.Id,
                Name = SecureStringsGenerator.Generate(Constants.Sessions.SessionDefaultNameLength),
                
                RefreshTokens = new List<SessionRefreshTokenDbo>()
                {
                    new SessionRefreshTokenDbo()
                    {
                        Id = SecureGuidGenerator.Generate(),
                        SessionId = sessionId,
                        Hash = SessionsRefreshTokensHelper.Hash(refreshToken),
                        CreatedAt = now,
                        ExpiresAt = now.AddDays(Constants.Sessions.RefreshTokenLifetime),
                        UsedAt = null
                    }
                },
                
                Events = new List<SessionEventDbo>()
                {
                    new SessionEventDbo()
                    {
                        Id = SecureGuidGenerator.Generate(),
                        SessionId = sessionId,
                        OccurredAt = now,
                        Type = SessionEventType.Created,
                        Source = source,
                        DeviceInfo = deviceInfo
                    }
                }
            };

            await sessionsDao.CreateSessionAsync(session, cancellationToken);
            
            await transaction.CommitAsync(cancellationToken);
            
        #endregion
        
        #region Issuing access token

        var accessToken = accessTokensService.Issue
        (
            creature.Id,
            sessionId,
            Constants.Sessions.AccessTokenIssuer,
            ratel_shared_auxiliary.Constants.Sessions.AccessTokenAudience
        );

        #endregion

        return new Tuple<IReadOnlySet<SessionCreationError>, string?, string?, DateTime?>
        (
            new HashSet<SessionCreationError>(),
            refreshToken,
            accessToken.Value,
            accessToken.ExpiresAt
        );
    }
}