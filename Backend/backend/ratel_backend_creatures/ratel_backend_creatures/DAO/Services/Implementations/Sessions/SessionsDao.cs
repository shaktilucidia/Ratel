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

using Microsoft.EntityFrameworkCore;
using ratel_backend_creatures.DAO.Contexts;
using ratel_backend_creatures.DAO.Models.Sessions;
using ratel_backend_creatures.DAO.Services.Abstract;
using ratel_backend_creatures.DAO.Services.Implementations.Sessions.Validators;
using ratel_backend_creatures.Enums.Sessions;
using ratel_shared_auxiliary.Validators;

namespace ratel_backend_creatures.DAO.Services.Implementations.Sessions;

public class SessionsDao
(
    MainDbContext dbContext
) : ISessionsDao
{
    public Task CreateSessionAsync
    (
        CreatureSessionDbo session,
        CancellationToken cancellationToken
    )
    {
        SessionsValidator.ValidateFreshSession(session);

        dbContext.CreatureSessions.Add(session);

        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Tuple<bool, DateTime>?> GetSessionInfoAsync
    (
        Guid creatureId,
        Guid sessionId,
        CancellationToken cancellationToken
    )
    {
        GuidsValidator.MustBeNonEmpty(creatureId);
        GuidsValidator.MustBeNonEmpty(sessionId);
        
        return dbContext
            .CreatureSessions
            .Where
            (
                s => s.CreatureId == creatureId && s.Id == sessionId
            )
            .Select
            (
                s => new Tuple<bool, DateTime>
                (
                    s.Events.Any(e => e.Type == SessionEventType.Revoked),
                    s.Events.Max(e => e.OccurredAt)
                )
            )
            .SingleOrDefaultAsync(cancellationToken);
    }
}