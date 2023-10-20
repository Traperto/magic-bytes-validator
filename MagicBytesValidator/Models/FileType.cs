using System;

namespace MagicBytesValidator.Models;

/// <inheritdoc />
[Obsolete("Use other variants of IFileType instead.")]
public class FileType : FileTypeWithOffsetStartSequences
{
    public FileType(string[] mimeTypes, string[] extensions, byte[][] magicByteSequences, uint magicByteOffset = 0)
        : base(mimeTypes, extensions, magicByteSequences, magicByteOffset)
    {
    }
}