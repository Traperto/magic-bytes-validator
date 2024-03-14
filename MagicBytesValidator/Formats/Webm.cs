namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
public class Webm : FileByteFilter
{
    public Webm() : base(
        ["video/webm"],
        ["mkv", "mka", "mks", "mk3d", "webm"]
    )
    {
        StartsWith([0x1A, 0x45, 0xDF, 0xA3]);
    }
}