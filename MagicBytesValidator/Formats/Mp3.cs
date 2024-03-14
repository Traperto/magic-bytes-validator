namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Mp3 : FileByteFilter
{
    public Mp3() : base(
        ["audio/mpeg"],
        ["mp3"]
    )
    {
        StartsWithAnyOf([
            [0x49, 0x44, 0x33],
            [0xFF, 0xFB],
            [0xFF, 0xF3],
            [0xFF, 0xF2]
        ]);
    }
}