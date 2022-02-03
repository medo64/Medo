/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

//2021-11-25: Refactored to use pattern matching
//2021-07-06: Refactored for .NET 5
//2013-03-26: Initial version

namespace Medo.Security.Cryptography;

using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Various password generation algorithms compatible with Unix crypt.
/// Based on:
///     http://www.akkadia.org/drepper/SHA-crypt.txt
///     http://www.freebsd.org/cgi/cvsweb.cgi/~checkout~/src/lib/libcrypt/crypt.c?rev=1.2
///     http://httpd.apache.org/docs/2.2/misc/password_encryptions.html
/// </summary>
/// <example>
/// <code>
/// var hash = CryptPassword.Create("Test", CryptPasswordAlgorithm.Sha256);
/// if (CryptPassword.Verify("Test", hash)) {
///     // DO SOMETHING
/// }
/// </code>
/// </example>
public static class CryptPassword {

    private static readonly UTF8Encoding Utf8WithoutBom = new(false);
    private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();
    private static readonly char[] Base64Characters = new char[] { '.', '/', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

    private const int DefaultMdSaltSize = 8;
    private const int DefaultShaSaltSize = 16;

    private const int DefaultMd5IterationCount = 1000;
    private const int DefaultShaIterationCount = 5000;

    /// <summary>
    /// Default salt size. It will be 8 bytes for MD5 algorithms and 16 bytes for SHA algorithm family.
    /// </summary>
    public const int DefaultSaltSize = -1;

    /// <summary>
    /// Default iteration count. It will be 1000 for MD5 algorithms and 5000 for SHA algorithm family.
    /// </summary>
    public const int DefaultIterationCount = -1;


    /// <summary>
    /// Hashes a password.
    /// </summary>
    /// <param name="password">Password to hash. It is converted to bytes using UTF-8 encoding.</param>
    /// <exception cref="ArgumentNullException">Password cannot be null.</exception>
    public static string Create(string password)
        => Create(password, CryptPasswordAlgorithm.Sha512, DefaultSaltSize, DefaultIterationCount);

    /// <summary>
    /// Hashes a password.
    /// </summary>
    /// <param name="password">Password to hash. It is converted to bytes using UTF-8 encoding.</param>
    /// <param name="algorithm">Algorithm to use.</param>
    /// <exception cref="ArgumentNullException">Password cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Unknown algorithm.</exception>
    public static string Create(string password, CryptPasswordAlgorithm algorithm)
        => Create(password, algorithm, DefaultSaltSize, DefaultIterationCount);

    /// <summary>
    /// Hashes a password.
    /// </summary>
    /// <param name="password">Password to hash. It is converted to bytes using UTF-8 encoding.</param>
    /// <param name="algorithm">Algorithm to use.</param>
    /// <param name="saltSize">Salt size. Must be between 0 and 16 bytes.</param>
    /// <exception cref="ArgumentNullException">Password cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Unknown algorithm. -or- Salt size must be between 0 and 16 bytes.</exception>
    public static string Create(string password, CryptPasswordAlgorithm algorithm, int saltSize)
        => Create(password, algorithm, saltSize, DefaultIterationCount);

    /// <summary>
    /// Hashes a password.
    /// </summary>
    /// <param name="password">Password to hash. It is converted to bytes using UTF-8 encoding.</param>
    /// <param name="algorithm">Algorithm to use.</param>
    /// <param name="saltSize">Salt size. Must be between 0 and 16 bytes.</param>
    /// <param name="iterationCount">Number of iterations. If value is 0 then default value is used.</param>
    /// <exception cref="System.ArgumentNullException">Password cannot be null.</exception>
    /// <exception cref="System.ArgumentOutOfRangeException">Unknown algorithm. -or- Salt size must be between 0 and 16 bytes.</exception>
    public static string Create(string password, CryptPasswordAlgorithm algorithm, int saltSize, int iterationCount) {
        if (password == null) { throw new ArgumentNullException(nameof(password), "Password cannot be null."); }
        return Create(Utf8WithoutBom.GetBytes(password), algorithm, saltSize, iterationCount);
    }


    /// <summary>
    /// Hashes a password.
    /// </summary>
    /// <param name="password">Password to hash.</param>
    /// <exception cref="ArgumentNullException">Password cannot be null.</exception>
    public static string Create(byte[] password)
        => Create(password, CryptPasswordAlgorithm.Sha512, DefaultSaltSize, DefaultIterationCount);

    /// <summary>
    /// Hashes a password.
    /// </summary>
    /// <param name="password">Password to hash.</param>
    /// <param name="algorithm">Algorithm to use.</param>
    /// <exception cref="ArgumentNullException">Password cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Unknown algorithm.</exception>
    public static string Create(byte[] password, CryptPasswordAlgorithm algorithm)
        => Create(password, algorithm, DefaultSaltSize, DefaultIterationCount);

    /// <summary>
    /// Hashes a password.
    /// </summary>
    /// <param name="password">Password to hash.</param>
    /// <param name="algorithm">Algorithm to use.</param>
    /// <param name="saltSize">Salt size. Must be between 0 and 16 bytes.</param>
    /// <exception cref="ArgumentNullException">Password cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Unknown algorithm. -or- Salt size must be between 0 and 16 bytes.</exception>
    public static string Create(byte[] password, CryptPasswordAlgorithm algorithm, int saltSize)
        => Create(password, algorithm, saltSize, DefaultIterationCount);

    /// <summary>
    /// Hashes a password.
    /// </summary>
    /// <param name="password">Password to hash.</param>
    /// <param name="algorithm">Algorithm to use.</param>
    /// <param name="saltSize">Salt size. Must be between 0 and 16 bytes.</param>
    /// <param name="iterationCount">Number of iterations. If value is DefaultIterationCount, the default value for algorithm is used.</param>
    /// <exception cref="ArgumentNullException">Password cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Unknown algorithm. -or- Salt size must be between 0 and 16 bytes. -or- Iteration count must be between 1000 and 999999999.</exception>
    public static string Create(byte[] password, CryptPasswordAlgorithm algorithm, int saltSize, int iterationCount) {
        if (saltSize is not DefaultSaltSize and (< 0 or > 16)) { throw new ArgumentOutOfRangeException(nameof(saltSize), "Salt size must be between 0 and 16 bytes."); }

        if (saltSize == DefaultSaltSize) {
            saltSize = algorithm switch {
                CryptPasswordAlgorithm.MD5 => DefaultMdSaltSize,
                CryptPasswordAlgorithm.MD5Apache => DefaultMdSaltSize,
                CryptPasswordAlgorithm.Sha256 => DefaultShaSaltSize,
                CryptPasswordAlgorithm.Sha512 => DefaultShaSaltSize,
                _ => DefaultShaSaltSize,
            };
        }

        var salt = new byte[saltSize];
        Rng.GetBytes(salt);
        for (var i = 0; i < salt.Length; i++) {  // make it an ascii
            salt[i] = (byte)Base64Characters[salt[i] % Base64Characters.Length];
        }

        return Create(password, algorithm, salt, iterationCount);
    }


    /// <summary>
    /// Hashes a password.
    /// </summary>
    /// <param name="password">Password to hash.</param>
    /// <param name="algorithm">Algorithm to use.</param>
    /// <param name="salt">Salt. Used as is without limitation of the field size.</param>
    /// <exception cref="ArgumentNullException">Password cannot be null. -or- Salt cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Unknown algorithm. -or- Iteration count must be between 1000 and 999999999.</exception>
    public static string Create(byte[] password, CryptPasswordAlgorithm algorithm, byte[] salt)
        => Create(password, algorithm, salt, DefaultIterationCount);

    /// <summary>
    /// Hashes a password.
    /// </summary>
    /// <param name="password">Password to hash.</param>
    /// <param name="algorithm">Algorithm to use.</param>
    /// <param name="salt">Salt. Used as is without limitation of the field size.</param>
    /// <param name="iterationCount">Number of iterations. If value is DefaultIterationCount, the default value for algorithm is used.</param>
    /// <exception cref="ArgumentNullException">Password cannot be null. -or- Salt cannot be null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Unknown algorithm. -or- Iteration count must be between 1000 and 999999999.</exception>
    public static string Create(byte[] password, CryptPasswordAlgorithm algorithm, byte[] salt, int iterationCount) {
        if (password == null) { throw new ArgumentNullException(nameof(password), "Password cannot be null."); }
        if (salt == null) { throw new ArgumentNullException(nameof(salt), "Salt cannot be null."); }
        if (iterationCount is not DefaultIterationCount and (< 1000 or > 999999999)) { throw new ArgumentOutOfRangeException(nameof(iterationCount), "Iteration count must be between 1000 and 999999999."); }

        return algorithm switch {
            CryptPasswordAlgorithm.MD5 => CreateMd5Basic(password, salt, iterationCount),
            CryptPasswordAlgorithm.MD5Apache => CreateMd5Apache(password, salt, iterationCount),
            CryptPasswordAlgorithm.Sha256 => CreateSha256(password, salt, iterationCount),
            CryptPasswordAlgorithm.Sha512 => CreateSha512(password, salt, iterationCount),
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm), "Unknown algorithm."),
        };
    }


    /// <summary>
    /// Verifies password agains hash.
    /// </summary>
    /// <param name="password">Password to check.</param>
    /// <param name="passwordHash">Hashed password.</param>
    /// <exception cref="System.ArgumentNullException">Password cannot be null.</exception>
    public static bool Verify(string password, string passwordHash) {
        if (password == null) { throw new ArgumentNullException(nameof(password), "Password cannot be null."); }
        return Verify(Utf8WithoutBom.GetBytes(password), passwordHash);
    }

    /// <summary>
    /// Verifies
    /// </summary>
    /// <param name="password">Password to check.</param>
    /// <param name="passwordHash">Hashed password.</param>
    /// <exception cref="System.ArgumentNullException">Password cannot be null.</exception>
    public static bool Verify(byte[] password, string passwordHash) {
        if (password == null) { throw new ArgumentNullException(nameof(password), "Password cannot be null."); }
        if (passwordHash == null) { return false; }

        if (!(SplitHashedPassword(passwordHash, out var id, out var iterationCount, out var salt, out var hash))) { return false; }

        string hashCalc;
        switch (id) {  // algorithm
            case "1": SplitHashedPassword(CreateMd5Basic(password, salt, iterationCount), out hashCalc); break;
            case "apr1": SplitHashedPassword(CreateMd5Apache(password, salt, iterationCount), out hashCalc); break;
            case "5": SplitHashedPassword(CreateSha256(password, salt, iterationCount), out hashCalc); break;
            case "6": SplitHashedPassword(CreateSha512(password, salt, iterationCount), out hashCalc); break;
            default: return false;
        }
        return string.Equals(hash, hashCalc);
    }


    #region SHA 256/512

    private static string CreateSha256(byte[] password, byte[] salt, int iterationCount) {
        if (iterationCount == DefaultIterationCount) { iterationCount = DefaultShaIterationCount; }
        var c = CreateSha(isSha256: true, password, salt, iterationCount);

        var sb = new StringBuilder();
        sb.Append("$5$");
        if (iterationCount != DefaultShaIterationCount) {
            sb.Append("rounds=" + iterationCount.ToString(CultureInfo.InvariantCulture) + "$");
        }
        sb.Append(Encoding.ASCII.GetString(salt));
        sb.Append('$');
        Base64TripetFill(sb, c[00], c[10], c[20]);
        Base64TripetFill(sb, c[21], c[01], c[11]);
        Base64TripetFill(sb, c[12], c[22], c[02]);
        Base64TripetFill(sb, c[03], c[13], c[23]);
        Base64TripetFill(sb, c[24], c[04], c[14]);
        Base64TripetFill(sb, c[15], c[25], c[05]);
        Base64TripetFill(sb, c[06], c[16], c[26]);
        Base64TripetFill(sb, c[27], c[07], c[17]);
        Base64TripetFill(sb, c[18], c[28], c[08]);
        Base64TripetFill(sb, c[09], c[19], c[29]);
        Base64TripetFill(sb, null, c[31], c[30]);
        return sb.ToString();
    }

    private static string CreateSha512(byte[] password, byte[] salt, int iterationCount) {
        if (iterationCount == DefaultIterationCount) { iterationCount = DefaultShaIterationCount; }
        var c = CreateSha(isSha256: false, password, salt, iterationCount);

        var sb = new StringBuilder();
        sb.Append("$6$");
        if (iterationCount != DefaultShaIterationCount) {
            sb.Append("rounds=" + iterationCount.ToString(CultureInfo.InvariantCulture) + "$");
        }
        sb.Append(Encoding.ASCII.GetString(salt));
        sb.Append('$');
        Base64TripetFill(sb, c[00], c[21], c[42]);
        Base64TripetFill(sb, c[22], c[43], c[01]);
        Base64TripetFill(sb, c[44], c[02], c[23]);
        Base64TripetFill(sb, c[03], c[24], c[45]);
        Base64TripetFill(sb, c[25], c[46], c[04]);
        Base64TripetFill(sb, c[47], c[05], c[26]);
        Base64TripetFill(sb, c[06], c[27], c[48]);
        Base64TripetFill(sb, c[28], c[49], c[07]);
        Base64TripetFill(sb, c[50], c[08], c[29]);
        Base64TripetFill(sb, c[09], c[30], c[51]);
        Base64TripetFill(sb, c[31], c[52], c[10]);
        Base64TripetFill(sb, c[53], c[11], c[32]);
        Base64TripetFill(sb, c[12], c[33], c[54]);
        Base64TripetFill(sb, c[34], c[55], c[13]);
        Base64TripetFill(sb, c[56], c[14], c[35]);
        Base64TripetFill(sb, c[15], c[36], c[57]);
        Base64TripetFill(sb, c[37], c[58], c[16]);
        Base64TripetFill(sb, c[59], c[17], c[38]);
        Base64TripetFill(sb, c[18], c[39], c[60]);
        Base64TripetFill(sb, c[40], c[61], c[19]);
        Base64TripetFill(sb, c[62], c[20], c[41]);
        Base64TripetFill(sb, null, null, c[63]);
        return sb.ToString();
    }

    private static byte[] CreateSha(bool isSha256, byte[] password, byte[] salt, int iterationCount) {
        byte[] hashA;
        using var digestA = isSha256 ? (HashAlgorithm)SHA256.Create() : SHA512.Create();  // step 1
        AddDigest(digestA, password);  // step 2
        AddDigest(digestA, salt);  // step 3

        byte[] hashB;
        var digestB = isSha256 ? (HashAlgorithm)SHA256.Create() : SHA512.Create();  // step 4
        AddDigest(digestB, password);  // step 5
        AddDigest(digestB, salt);  // step 6
        AddDigest(digestB, password);  // step 7
        hashB = FinishDigest(digestB);  // step 8
        AddRepeatedDigest(digestA, hashB, password.Length);  // step 9/10

        var passwordLenght = password.Length;
        while (passwordLenght > 0) {  // step 11
            if ((passwordLenght & 0x01) != 0) {  // bit 1
                AddDigest(digestA, hashB);
            } else {  // bit 0
                AddDigest(digestA, password);
            }
            passwordLenght >>= 1;
        }
        hashA = FinishDigest(digestA);  // step 12

        byte[] hashDP;
        using var digestDP = isSha256 ? (HashAlgorithm)SHA256.Create() : SHA512.Create();  // step 13
        for (var i = 0; i < password.Length; i++) {  // step 14
            AddDigest(digestDP, password);
        }
        hashDP = FinishDigest(digestDP);  // step 15
        var p = ProduceBytes(hashDP, password.Length);  // step 16

        byte[] hashDS;
        using var digestDS = isSha256 ? (HashAlgorithm)SHA256.Create() : SHA512.Create();  // step 17
        for (var i = 0; i < (16 + hashA[0]); i++) {  // step 18
            AddDigest(digestDS, salt);
        }
        hashDS = FinishDigest(digestDS);  // step 19
        var s = ProduceBytes(hashDS, salt.Length);  // step 20

        var hashAC = hashA;
        for (var i = 0; i < iterationCount; i++) {  // step 21
            using var digestC = isSha256 ? (HashAlgorithm)SHA256.Create() : SHA512.Create();   // step 21a
            if ((i % 2) == 1) {  // step 21b
                AddDigest(digestC, p);
            } else {  // step 21c
                AddDigest(digestC, hashAC);
            }
            if ((i % 3) != 0) { AddDigest(digestC, s); }  // step 21d
            if ((i % 7) != 0) { AddDigest(digestC, p); }  // step 21e
            if ((i % 2) == 1) {  // step 21f
                AddDigest(digestC, hashAC);
            } else {  // step 21g
                AddDigest(digestC, p);
            }
            hashAC = FinishDigest(digestC);  // step 21h
        }
        return hashAC;
    }

    #endregion

    #region MD5

    private static string CreateMd5Basic(byte[] password, byte[] salt, int iterationCount) {
        if (iterationCount == DefaultIterationCount) { iterationCount = DefaultMd5IterationCount; }
        return CreateMd5(password, salt, iterationCount, "$1$");
    }

    private static string CreateMd5Apache(byte[] password, byte[] salt, int iterationCount) {
        if (iterationCount == DefaultIterationCount) { iterationCount = DefaultMd5IterationCount; }
        return CreateMd5(password, salt, iterationCount, "$apr1$");
    }

    private static string CreateMd5(byte[] password, byte[] salt, int iterationCount, string magic) {
        byte[] hashA;
        using var digestA = MD5.Create();  // step 1

        // password+magic+salt
        AddDigest(digestA, password);  // step 2
        AddDigest(digestA, Encoding.ASCII.GetBytes(magic));
        AddDigest(digestA, salt);  // step 3

        byte[] hashB;
        using var digestB = MD5.Create();  // step 4
        AddDigest(digestB, password);  // step 5
        AddDigest(digestB, salt);  // step 6
        AddDigest(digestB, password);  // step 7
        hashB = FinishDigest(digestB);  // step 8
        AddRepeatedDigest(digestA, hashB, password.Length);  // step 9/10

        var passwordLenght = password.Length;
        while (passwordLenght > 0) {  // step 11
            if ((passwordLenght & 0x01) != 0) {  // bit 1
                AddDigest(digestA, new byte[] { 0x00 });
            } else {  // bit 0
                AddDigest(digestA, new byte[] { password[0] });
            }
            passwordLenght >>= 1;
        }
        hashA = FinishDigest(digestA);  // step 12

        var hashAC = hashA;
        for (var i = 0; i < iterationCount; i++) {  // step 21
            using var digestC = MD5.Create();  // step 21a
            if ((i % 2) == 1) {  // step 21b
                AddDigest(digestC, password);
            } else {  // step 21c
                AddDigest(digestC, hashAC);
            }
            if ((i % 3) != 0) { AddDigest(digestC, salt); }  // step 21d
            if ((i % 7) != 0) { AddDigest(digestC, password); }  // step 21e
            if ((i % 2) == 1) {  // step 21f
                AddDigest(digestC, hashAC);
            } else {  // step 21g
                AddDigest(digestC, password);
            }
            hashAC = FinishDigest(digestC);  // step 21h
        }

        var c = hashAC;

        var sb = new StringBuilder();
        sb.Append(magic);
        if (iterationCount != DefaultMd5IterationCount) {
            sb.Append("rounds=" + iterationCount.ToString(CultureInfo.InvariantCulture) + "$");
        }
        sb.Append(Encoding.ASCII.GetString(salt));
        sb.Append('$');
        Base64TripetFill(sb, c[00], c[06], c[12]);
        Base64TripetFill(sb, c[01], c[07], c[13]);
        Base64TripetFill(sb, c[02], c[08], c[14]);
        Base64TripetFill(sb, c[03], c[09], c[15]);
        Base64TripetFill(sb, c[04], c[10], c[05]);
        Base64TripetFill(sb, null, null, c[11]);
        return sb.ToString();
    }

    #endregion

    #region Helpers

    private static void AddDigest(HashAlgorithm digest, byte[] bytes) {
        if (bytes.Length == 0) { return; }
        var hashLen = digest.HashSize / 8;

        var offset = 0;
        var remaining = bytes.Length;
        while (remaining > 0) {
            digest.TransformBlock(bytes, offset, (remaining >= hashLen) ? hashLen : remaining, null, 0);
            remaining -= hashLen;
            offset += hashLen;
        }
    }

    private static void AddRepeatedDigest(HashAlgorithm digest, byte[] bytes, int length) {
        var hashLen = digest.HashSize / 8;
        while (length > 0) {
            digest.TransformBlock(bytes, 0, (length >= hashLen) ? hashLen : length, null, 0);
            length -= hashLen;
        }
    }

    private static byte[] ProduceBytes(byte[] hash, int lenght) {
        var hashLen = hash.Length;
        var produced = new byte[lenght];
        var offset = 0;
        while (lenght > 0) {
            Buffer.BlockCopy(hash, 0, produced, offset, (lenght >= hashLen) ? hashLen : lenght);
            offset += hashLen;
            lenght -= hashLen;
        }

        return produced;
    }

    private static byte[] FinishDigest(HashAlgorithm digest) {
        digest.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
        return digest.Hash ?? Array.Empty<byte>();
    }

    private static void Base64TripetFill(StringBuilder sb, byte? byte2, byte? byte1, byte? byte0) {
        if (byte0 != null) {
            sb.Append(Base64Characters[byte0.Value & 0x3F]);
            if (byte1 != null) {
                sb.Append(Base64Characters[((byte1.Value & 0x0F) << 2) | (byte0.Value >> 6)]);
                if (byte2 != null) {
                    sb.Append(Base64Characters[((byte2.Value & 0x03) << 4) | (byte1.Value >> 4)]);
                    sb.Append(Base64Characters[byte2.Value >> 2]);
                } else {
                    sb.Append(Base64Characters[byte1.Value >> 4]);
                }
            } else {
                sb.Append(Base64Characters[byte0.Value >> 6]);
            }
        }
    }

    private static bool SplitHashedPassword(string hashedPassword, out string hash) {
        return SplitHashedPassword(hashedPassword, out _, out _, out _, out hash);
    }

    private static bool SplitHashedPassword(string hashedPassword, out string id, out int iterationCount, out byte[] salt, out string hash) {
        var parts = hashedPassword.Split('$');
        if (parts.Length < 4) {
            id = string.Empty;
            iterationCount = 0;
            salt = Array.Empty<byte>();
            hash = string.Empty;
            return false;
        }

        id = parts[1];
        if (parts[2].StartsWith("rounds=", StringComparison.Ordinal) && (parts.Length >= 5) && (int.TryParse(parts[2][7..], NumberStyles.Integer, CultureInfo.InvariantCulture, out iterationCount))) {
            salt = Encoding.ASCII.GetBytes(parts[3]);
            hash = parts[4];
        } else {
            iterationCount = DefaultIterationCount;
            salt = Encoding.ASCII.GetBytes(parts[2]);
            hash = parts[3];
        }
        return true;
    }

    #endregion

}


/// <summary>
/// Algorithm to use for hasing a password.
/// </summary>
public enum CryptPasswordAlgorithm {
    /// <summary>
    /// MD-5 algoritm.
    /// </summary>
    MD5 = 10,

    /// <summary>
    /// Apache's MD-5 algoritm.
    /// </summary>
    MD5Apache = 11,

    /// <summary>
    /// SHA-256 algoritm.
    /// </summary>
    Sha256 = 50,

    /// <summary>
    /// SHA-512 algoritm.
    /// </summary>
    Sha512 = 60,
}
