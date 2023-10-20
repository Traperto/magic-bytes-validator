using System.Linq;

namespace MagicBytesValidator.Models;

/// <summary>
/// Variant of <see cref="IFileType"/> that is based on magic byte sequences that begin after an offset at the
/// start of a file.
/// </summary>
public class FileTypeWithOffsetStartSequences : FileTypeWithStartSequences
{
    /// <summary>
    /// Number of bytes from the start of the file that should be skipped for magic byte validation.
    /// </summary>
    public uint MagicByteOffset { get; }

    public FileTypeWithOffsetStartSequences(
        string[] mimeTypes,
        string[] extensions,
        byte[][] magicByteSequences,
        uint magicByteOffset
    ): base(mimeTypes, extensions, magicByteSequences)
    {
        MagicByteOffset = magicByteOffset;
    }

    public override bool Matches(byte[] bytes)
    {
        return MagicByteSequences.Any(mb => mb.SequenceEqual(
            bytes
               .Skip((int)MagicByteOffset)
               .Take(mb.Length))
        );
    }
}