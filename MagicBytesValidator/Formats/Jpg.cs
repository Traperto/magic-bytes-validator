namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
public class Jpg : FileByteFilter
{
    public Jpg() : base(
        ["image/jpeg"],
        ["jpg", "jpeg", "jpe", "jif", "jfif", "jfi"]
    )
    {
        StartsWith([0xFF, 0xD8, 0xFF])
            .EndsWith([0xFF, 0xD9]);
    }
}