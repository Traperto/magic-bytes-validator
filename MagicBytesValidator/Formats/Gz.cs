using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

public class Gz : FileTypeWithStartSequences
{
    public Gz() : base(
        new[] { "application/gzip" },
        new[] { "gz" },
        new[]
        {
            new byte[] { 31, 139 }
        }
    )
    {
    }
}