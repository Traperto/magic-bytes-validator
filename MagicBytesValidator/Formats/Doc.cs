namespace MagicBytesValidator.Formats;

/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Doc : FileByteFilter
{
    public Doc() : base(
        ["application/msword"],
        ["doc", "dot"]
    )
    {
        StartsWith([0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1])
            .SpecificAnyOf([
                new ByteCheck(512, [0xEC, 0xA5, 0xC1, 0x00])
            ]);
    }
}