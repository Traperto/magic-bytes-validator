namespace MagicBytesValidator.Formats;

/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Rtf : FileByteFilter
{
    public Rtf() : base(
        ["application/rtf"],
        ["rtf"]
    )
    {
        StartsWith([0x7B, 0x5C, 0x72, 0x74, 0x66, 0x31])
            .EndsWith([0x7D]);
    }
}