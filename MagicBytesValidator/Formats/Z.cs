using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

public class Z : FileType
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