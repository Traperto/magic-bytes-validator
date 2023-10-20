using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

public class Odp : FileTypeWithStartSequences
{
    public Odp() : base(
        new[] { "application/vnd.oasis.opendocument.presentation" },
        new[] { "odp" },
        new[]
        {
            new byte[] { 80, 75, 7, 8 }
        }
    )
    {
    }
}