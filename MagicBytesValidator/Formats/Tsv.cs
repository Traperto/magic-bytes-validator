namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
/// <see href="https://en.wikipedia.org/wiki/MPEG_transport_stream"/>
public class Tsv : FileByteFilter
{
    public Tsv() : base(
        ["video/mp2t"],
        ["ts", "tsv", "tsa", "mpg", "mpeg"]
    )
    {
        StartsWith([0x47]);
    }
}