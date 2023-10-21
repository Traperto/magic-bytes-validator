using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
/// <see href="https://en.wikipedia.org/wiki/MPEG_transport_stream"/>
public class Tsv : FileTypeWithStartSequences
{
    public Tsv() : base(
        new[] { "video/mp2t" },
        new[] { "ts", "tsv", "tsa", "mpg", "mpeg" },
        new byte[] { 0x47 }
    )
    {
    }
}