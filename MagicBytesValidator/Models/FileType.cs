using System;
using System.Linq;

namespace MagicBytesValidator.Models;

/// <inheritdoc />
[Obsolete("Use other variants of IFileType instead.")]
public class FileType : FileTypeWithStartSequences
{
    /// <summary>
    /// Number of bytes from the start of the file that should be skipped for magic byte validation.
    /// </summary>
    public uint MagicByteOffset { get; }

    public FileType(
        string[] mimeTypes,
        string[] extensions,
        byte[][] magicByteSequences,
        uint magicByteOffset
    ) : base(mimeTypes, extensions, magicByteSequences)
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