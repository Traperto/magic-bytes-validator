using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

public class Gif : FileTypeWithStartSequences
{
    public Gif() : base(
        new[] { "image/gif" },
        new[] { "gif" },
        new[]
        {
            new byte[] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 },
            new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 }
        }
    )
    {
    }
}