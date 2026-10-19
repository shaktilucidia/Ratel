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

using ratel_backend_creatures.Models.Business.Sessions;

namespace ratel_backend_creatures.Services.Abstract;

/// <summary>
/// Use it to work with access tokens
/// </summary>
public interface IAccessTokensService
{
    /// <summary>
    /// Issue access token
    /// </summary>
    /// <param name="creatureId">Issue token for this creature</param>
    /// <param name="sessionId">Issue token for this session</param>
    /// <param name="issuer">Issuer</param>
    /// <param name="audience">Who can use token</param>
    /// <returns>Issued token</returns>
    IssuedAccessToken Issue
    (
        Guid creatureId,
        Guid sessionId,
        string issuer,
        string audience
    );
}