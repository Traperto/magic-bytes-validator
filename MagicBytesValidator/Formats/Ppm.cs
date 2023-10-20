using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

public class Ppm : FileTypeWithStartSequences
{
    public Ppm() : base(
        new[] { "image/x-portable-pixmap" },
        new[] { "ppm" },
        new[]
        {
            new byte[] { 80, 51, 10 }
        }
    )
    {
    }
}