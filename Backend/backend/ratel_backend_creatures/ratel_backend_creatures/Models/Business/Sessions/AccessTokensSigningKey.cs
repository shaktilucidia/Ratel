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

using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using ratel_backend_creatures.Models.Settings;

namespace ratel_backend_creatures.Models.Business.Sessions;

/// <summary>
/// Stores private key, used to sign access tokens
/// </summary>
public class AccessTokensSigningKey : IDisposable
{
    /// <summary>
    /// Private key itself
    /// </summary>
    private readonly RSA _privateKey;

    /// <summary>
    /// Key and key algorithm
    /// </summary>
    public SigningCredentials Credentials { get; }

    public AccessTokensSigningKey(AccessTokensSettings keySettings)
    {
        ArgumentNullException.ThrowIfNull(keySettings);
        ArgumentException.ThrowIfNullOrWhiteSpace(keySettings.PrivateKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(keySettings.PrivateKeyId);

        _privateKey = RSA.Create();

        try
        {
            _privateKey.ImportFromPem(keySettings.PrivateKey);

            if (_privateKey.KeySize < Constants.Sessions.AccessTokenMinimumKeySize)
            {
                throw new InvalidOperationException
                (
                    $"Access token signing key must be at least " +
                    $"{ Constants.Sessions.AccessTokenMinimumKeySize } bits; " +
                    $"actual size is { _privateKey.KeySize } bits"
                );
            }

            // Verify that the imported key supports signing.
            // Importing only a public key must fail this check
            _privateKey.SignData
            (
                new byte[] { 0 },
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1
            );

            Credentials = new SigningCredentials
            (
                new RsaSecurityKey(_privateKey)
                {
                    KeyId = keySettings.PrivateKeyId
                },
                SecurityAlgorithms.RsaSha256
            );
        }
        catch
        {
            _privateKey.Dispose();
            throw;
        }
    }

    public void Dispose()
    {
        _privateKey.Dispose();
    }
}