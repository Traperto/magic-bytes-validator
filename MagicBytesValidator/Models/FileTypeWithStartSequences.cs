namespace MagicBytesValidator.Models;

/// <summary>
/// Variant of <see cref="IFileType"/> that is based on magic byte sequences at the start of the file
/// </summary>
public class FileTypeWithStartSequences : IFileType
{
    /// <inheritdoc />
    public string[] MimeTypes { get; }

    /// <inheritdoc />
    public string[] Extensions { get; }

    /// <summary>
    /// List of magic-byte sequences to identify a file type based on the file contents
    /// <example>[ [47 49 46 38 37 61], [47 49 46 38 39 61] ] for "gif" files</example>
    /// </summary>
    public byte[][] MagicByteSequences { get; }

    public FileTypeWithStartSequences(string[] mimeTypes, string[] extensions, byte[] magicByteSequence)
        : this(mimeTypes, extensions, new[] { magicByteSequence })
    {
    }

    public FileTypeWithStartSequences(string[] mimeTypes, string[] extensions, byte[][] magicByteSequences)
    {
        if (!mimeTypes.Any() || mimeTypes.Any(string.IsNullOrEmpty))
        {
            throw new ArgumentEmptyException(nameof(mimeTypes));
        }

        if (!extensions.Any() || extensions.Any(string.IsNullOrEmpty))
        {
            throw new ArgumentEmptyException(nameof(extensions));
        }

        if (!magicByteSequences.Any() || magicByteSequences.Any(mbs => mbs.Length == 0))
        {
            throw new ArgumentEmptyException(nameof(magicByteSequences));
        }

        MimeTypes = mimeTypes;
        Extensions = extensions;
        MagicByteSequences = magicByteSequences;
    }

    public virtual bool Matches(byte[] fileByteStream)
    {
        return MagicByteSequences.Any(mb => mb.SequenceEqual(fileByteStream.Take(mb.Length)));
    }
}