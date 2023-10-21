using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Swf : FileTypeWithStartSequences
{
    public Swf() : base(
        new[] { "application/x-shockwave-flash" },
        new[] { "swf" },
        new[]
        {
            new byte[] { 0x43, 0x57, 0x53 },
            new byte[] { 0x46, 0x57, 0x53 },
            new byte[] { 0x5A, 0x57, 0x53 }
        }
    )
    {
    }
}