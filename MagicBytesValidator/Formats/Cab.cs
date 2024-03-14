namespace MagicBytesValidator.Formats;

/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Cab : FileByteFilter
{
    public Cab() : base(
        ["application/vnd.ms-cab-compressed", "application/x-cab-compressed"],
        ["cab"]
    )
    {
        StartsWithAnyOf(
            [
                [0x49, 0x53, 0x63, 0x28],
                [0x4D, 0x53, 0x43, 0x46]
            ]
        );
    }
}