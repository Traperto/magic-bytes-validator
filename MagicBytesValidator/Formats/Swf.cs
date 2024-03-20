namespace MagicBytesValidator.Formats;

/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Swf : FileByteFilter
{
    public Swf() : base(
        ["application/x-shockwave-flash"],
        ["swf"]
    )
    {
        StartsWithAnyOf([
            [0x43, 0x57, 0x53],
            [0x46, 0x57, 0x53],
            [0x5A, 0x57, 0x53]
        ]);
    }
}