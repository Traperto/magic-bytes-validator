namespace MagicBytesValidator.Models;

/// <summary>
/// An IFileType contains all necessary information to identify and validate the type of a file (based on MIME type,
/// extensions and magic-byte sequences).
/// </summary>
public interface IFileType
{
    /// <summary>
    /// MIME types of a file
    /// <example>["image/gif"]</example>
    /// </summary>
    public IReadOnlyList<string> MimeTypes { get; }

    /// <summary>
    /// File extensions for a type
    /// <example>[ "gif" ]</example>
    /// </summary>
    public IReadOnlyList<string> Extensions { get; }

    /// <summary>
    /// Returns whether a given file (as byte array) matches the file type.
    /// Implementations must be safe to call from multiple threads concurrently.
    /// </summary>
    public bool Matches(byte[] fileByteStream, FileByteType type = FileByteType.Strict);
}
