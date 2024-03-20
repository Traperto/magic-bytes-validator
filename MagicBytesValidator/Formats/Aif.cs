namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Aif : FileByteFilter
{
    public Aif() : base(
        ["audio/x-aiff"],
        ["aif", "aiff", "aifc"]
    )
    {
        StartsWith([0x46, 0x4F, 0x52, 0x4D, null, null, null, null, 0x41, 0x49, 0x46, 0x46]);
    }
}