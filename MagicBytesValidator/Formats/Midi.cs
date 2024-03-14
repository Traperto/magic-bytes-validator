namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Midi : FileByteFilter
{
    public Midi() : base(
        ["audio/x-midi"],
        ["midi", "mid"]
    )
    {
        StartsWith([0x4D, 0x54, 0x68, 0x64]);
    }
}