using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Mpg : FileTypeWithStartSequences
{
    public Mpg() : base(
        new[] { "video/mpeg" },
        new[] { "mpg", "mpeg", "mpe", "m2p", "vob" },
        new[]
        {
            new byte[] { 0x00, 0x00, 0x01, 0xB3 },
            new byte[] { 0x00, 0x00, 0x01, 0xBA }
        }
    )
    {
    }
}