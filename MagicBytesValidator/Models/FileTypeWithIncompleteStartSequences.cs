using System.Linq;
using MagicBytesValidator.Exceptions;
using MagicBytesValidator.Extensions;

namespace MagicBytesValidator.Models;

/// <summary>
/// Variant of <see cref="IFileType"/> that is based on magic byte sequences at the start of the file.
/// The sequences can have undefined bytes though. Those bytes are represented as null in the MagicByteSequences.
/// </summary>
public class FileTypeWithIncompleteStartSequences : IFileType
{
    /// <inheritdoc />
    public string[] MimeTypes { get; }

    /// <inheritdoc />
    public string[] Extensions { get; }

    /// <summary>
    /// List of magic-byte sequences to identify a file type based on the file contents
    /// <example>[ [47 49 46 38 37 61], [47 49 46 38 39 61] ] for "gif" files</example>
    /// </summary>
    public byte?[][] MagicByteSequences { get; }

    public FileTypeWithIncompleteStartSequences(string[] mimeTypes, string[] extensions, byte?[] magicByteSequence)
        : this(mimeTypes, extensions, new[] { magicByteSequence })
    {
    }

    public FileTypeWithIncompleteStartSequences(string[] mimeTypes, string[] extensions, byte?[][] magicByteSequences)
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

    public virtual bool Matches(byte[] bytes)
    {
        return MagicByteSequences.Any(sequence =>
        {
            if (bytes.Length < sequence.Length)
            {
                return false;
            }

            foreach (var (sequenceByte, index) in sequence.AsIndexed())
            {
                if (sequenceByte == null)
                {
                    continue;
                }

                if (sequenceByte != bytes[index])
                {
                    return false;
                }
            }

            return true;
        });
    }
}