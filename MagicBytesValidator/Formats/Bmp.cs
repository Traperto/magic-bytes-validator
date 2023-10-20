using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

public class Bmp : FileTypeWithStartSequences
{
    public Bmp() : base(
        new[] { "image/bmp" },
        new[] { "bmp" },
        new[]
        {
            new byte[] { 66, 77 }
        }
    )
    {
    }
}