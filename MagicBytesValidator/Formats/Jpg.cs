using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

public class Jpg : FileType
{
    public Jpg() : base(
        new[] { "image/jpeg" },
        new[] { "jpg", "jpeg", "jpe" },
        new[]
        {
            new byte[] { 255, 216, 255 },
            new byte[] { 73, 70, 0, 1 },
            new byte[] { 105, 102, 0, 0 }
        }
    )
    {
    }
}