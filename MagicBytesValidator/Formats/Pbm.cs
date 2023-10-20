using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

public class Pbm : FileTypeWithStartSequences
{
    public Pbm() : base(
        new[] { "image/x-portable-bitmap" },
        new[] { "pbm" },
        new[]
        {
            new byte[] { 80, 49, 10 }
        }
    )
    {
    }
}