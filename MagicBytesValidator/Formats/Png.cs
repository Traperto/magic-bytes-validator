using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

public class Png : FileType
{
    public Png() : base(
        new[] { "image/png" },
        new[] { "png" },
        new[]
        {
            new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }
        }
    )
    {
    }
}