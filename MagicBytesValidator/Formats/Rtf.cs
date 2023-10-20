using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

public class Rtf : FileTypeWithStartSequences
{
    public Rtf() : base(
        new[] { "application/rtf" },
        new[] { "rtf" },
        new[]
        {
            new byte[] { 123, 92, 114, 116, 102, 49 }
        }
    )
    {
    }
}