namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Z : FileByteFilter
{
    public Z() : base(
        ["application/x-compress"],
        ["z"]
    )
    {
        StartsWithAnyOf([
            [0x1F, 0x9D],
            [0x1F, 0xA0]
        ]);
    }
}