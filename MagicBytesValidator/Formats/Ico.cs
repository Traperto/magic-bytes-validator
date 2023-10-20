using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

public class Ico : FileTypeWithStartSequences
{
    public Ico() : base(
        new[] { "image/x-icon" },
        new[] { "ico" },
        new[]
        {
            new byte[] { 0, 0, 1, 0 }
        }
    )
    {
    }
}