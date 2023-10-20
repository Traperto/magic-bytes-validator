using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

public class Ppt2 : FileTypeWithStartSequences
{
    public Ppt2() : base(
        new[] { "application/vnd.ms-powerpoint" },
        new[] { "ppt", "ppz", "pps", "pot" },
        new[]
        {
            new byte[] { 208, 207, 17, 224, 161, 177, 26, 225 }
        }
    )
    {
    }
}