namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Gz : FileByteFilter
{
    public Gz() : base(
        ["application/gzip"],
        ["gz"]
    )
    {
        StartsWith([0x1F, 0x8B]);
    }
}