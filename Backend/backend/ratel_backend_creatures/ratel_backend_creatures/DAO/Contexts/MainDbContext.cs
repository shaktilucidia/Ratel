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

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ratel_backend_creatures.Constants.DAO;
using ratel_backend_creatures.DAO.Models.Creatures;
using ratel_backend_creatures.DAO.Models.Sessions;

namespace ratel_backend_creatures.DAO.Contexts;

/// <summary>
/// Main database context, can be used as Identity Framework context
/// </summary>
public class MainDbContext
(
    DbContextOptions<MainDbContext> options
) : IdentityDbContext<CreatureDbo, CreatureRoleDbo, Guid>(options)
{
    /// <summary>
    /// Creatures sessions
    /// </summary>
    public DbSet<CreatureSessionDbo> CreatureSessions => Set<CreatureSessionDbo>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<CreatureSessionDbo>
        (
            entity =>
            {
                entity.HasKey(session => session.Id);

                entity.Property(session => session.Name).HasMaxLength(Sessions.NameMaxLength);

                entity.HasIndex(session => session.CreatureId);

                // Session have one creature, creature have many sessions
                entity.HasOne<CreatureDbo>()
                    .WithMany()
                    .HasForeignKey(session => session.CreatureId);

                // Session have many tokens, token belongs to one session
                entity.HasMany<SessionRefreshTokenDbo>(session => session.RefreshTokens)
                    .WithOne()
                    .HasForeignKey(token => token.SessionId);

                // Session have many events, event belongs to one session
                entity.HasMany<SessionEventDbo>(session => session.Events)
                    .WithOne()
                    .HasForeignKey(sessionEvent => sessionEvent.SessionId);
            }
        );
    }
}