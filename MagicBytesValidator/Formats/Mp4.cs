using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Mp4 : FileTypeWithIncompleteStartSequences
{
    public Mp4() : base(
        new[] { "video/mp4" },
        new[] { "mp4" },
        new[]
        {
            new byte?[] { null, null, null, null, 0x66, 0x74, 0x79, 0x70, 0x69, 0x73, 0x6F, 0x6D },
            new byte?[] { null, null, null, null, 0x66, 0x74, 0x79, 0x70, 0x6D, 0x70, 0x34, 0x32 },
            new byte?[] { null, null, null, null, 0x66, 0x74, 0x79, 0x70, 0x4D, 0x53, 0x3E, 0x56 },
            new byte?[] { null, null, null, null, 0x66, 0x74, 0x79, 0x70, 0x4D, 0x53, 0x4E, 0x56 },
        }
    )
    {
    }
}