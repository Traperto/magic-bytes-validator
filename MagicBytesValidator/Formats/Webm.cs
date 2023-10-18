using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

public class Webm : FileType
{
    public Webm() : base(
        new[] { "video/webm" },
        new[] { "webm" },
        new[]
        {
            new byte[] { 26, 69, 223, 163 }
        }
    )
    {
    }
}