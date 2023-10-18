using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

public class Pgm : FileType
{
    public Pgm() : base(
        new[] { "image/x-portable-graymap" },
        new[] { "pgm" },
        new[]
        {
            new byte[] { 80, 50, 10 }
        }
    )
    {
    }
}