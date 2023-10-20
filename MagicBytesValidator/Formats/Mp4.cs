using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

public class Mp4 : FileTypeWithOffsetStartSequences
{
    public Mp4() : base(
        new[] { "video/mp4" },
        new[] { "mp4" },
        new[]
        {
            new byte[] { 102, 116, 121, 112, 105, 115, 111, 109 },
            new byte[] { 102, 116, 121, 112, 109, 112, 52, 50 },
            new byte[] { 102, 116, 121, 112, 77, 83, 62, 86 },
        },
        4
    )
    {
    }
}