namespace HotelDroid.Api.Services;

/// <summary>
/// Generates GUIDs and converts them to base32 (URL-safe, compact) format.
/// </summary>
public static class IdGenerator
{
    private const string Base32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

    /// <summary>
    /// Generate a new GUID and convert it to base32 format.
    /// Result is 26 characters, URL-safe, and suitable for file names.
    /// </summary>
    /// <returns>GUID converted to base32 (e.g., "a1b2c3d4e5f6g7h8i9j0k1l2")</returns>
    public static string GenerateId()
    {
        var guid = Guid.NewGuid();
        return ConvertToBase32(guid.ToByteArray());
    }

    /// <summary>
    /// Convert a GUID to base32 format.
    /// Uses lowercase for consistency with file naming conventions.
    /// </summary>
    /// <param name="bytes">Byte array from GUID.ToByteArray()</param>
    /// <returns>Base32-encoded string (22 characters)</returns>
    private static string ConvertToBase32(byte[] bytes)
    {
        if (bytes.Length != 16)
            throw new ArgumentException("Input must be 16 bytes (GUID size)", nameof(bytes));

        // RFC 4648 base32 encoding (A-Z, 2-7)
        // Adapted to use lowercase for file naming
        int buffer = 0;
        int bufferSize = 0;
        var result = new System.Text.StringBuilder();

        foreach (byte b in bytes)
        {
            buffer = (buffer << 8) | b;
            bufferSize += 8;

            while (bufferSize >= 5)
            {
                bufferSize -= 5;
                int index = (buffer >> bufferSize) & 31;
                result.Append(char.ToLowerInvariant(Base32Alphabet[index]));
            }
        }

        // Handle remaining bits
        if (bufferSize > 0)
        {
            int index = (buffer << (5 - bufferSize)) & 31;
            result.Append(char.ToLowerInvariant(Base32Alphabet[index]));
        }

        return result.ToString();
    }

    /// <summary>
    /// Validate that a string is a valid GUID-base32 ID.
    /// </summary>
    /// <param name="id">ID to validate</param>
    /// <returns>True if valid base32 ID (22 lowercase alphanumeric chars), false otherwise</returns>
    public static bool IsValidId(string id)
    {
        if (string.IsNullOrEmpty(id) || id.Length != 26)
            return false;

        return id.All(c => char.IsLower(c) && (char.IsLetter(c) || char.IsDigit(c)));
    }
}
