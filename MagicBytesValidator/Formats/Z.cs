using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

public class Z : FileTypeWithStartSequences
{
    public Z() : base(
        new[] { "application/x-compress" },
        new[] { "z" },
        new[]
        {
            new byte[] { 31, 157 }
        }
    )
    {
    }
}