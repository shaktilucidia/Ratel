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

using System.Data;
using Microsoft.EntityFrameworkCore;
using ratel_shared_auxiliary.UoW.Abstract;

namespace ratel_shared_auxiliary.UoW.Implementations
{
    public sealed class UnitOfWork<TDbContext>
    (
        TDbContext dbContext
    )
    : IUnitOfWork where TDbContext : DbContext
    {
        public async Task<IUnitOfWorkTransaction> BeginTransactionAsync
        (
            CancellationToken cancellationToken,
            IsolationLevel isolationLevel
        )
        {
            var transaction = await dbContext
                                .Database
                                .BeginTransactionAsync(isolationLevel, cancellationToken);

            return new UnitOfWorkTransaction(transaction);
        }
    }
}
