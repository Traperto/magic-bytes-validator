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
    public string[] MimeTypes { get; }

    /// <summary>
    /// File extensions for a type
    /// <example>[ "gif" ]</example>
    /// </summary>
    public string[] Extensions { get; }

    /// <summary>
    /// Returns whether a given file (as byte array) matches the file type
    /// </summary>
    public bool Matches(byte[] fileByteStream);
}