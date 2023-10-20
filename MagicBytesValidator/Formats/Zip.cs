using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

public class Zip : FileTypeWithStartSequences
{
    public Zip() : base(
        new[] { "application/zip", "application/x-zip-compressed" },
        new[] { "zip" },
        new[]
        {
            new byte[] { 80, 75, 3, 4 }
        }
    )
    {
    }
}