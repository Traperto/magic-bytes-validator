using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

public class Tif : FileType
{
    public Tif() : base(
        new[] { "image/tiff" },
        new[] { "tif", "tiff" },
        new[]
        {
            new byte[] { 73, 73, 42, 0 },
            new byte[] { 77, 77, 0, 42 }
        }
    )
    {
    }
}