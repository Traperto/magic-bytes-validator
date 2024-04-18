namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Dxr : FileByteFilter
{
    public Dxr() : base(
        ["application/x-director"],
        ["dxr", "dcr", "dir"]
    )
    {
        StartsWithAnyOf([
            [0x58, 0x46, 0x49, 0x52, null, null, null, null, 0x33, 0x39, 0x56, 0x4D],
            [0x52, 0x49, 0x46, 0x58, null, null, null, null, 0x4D, 0x56, 0x39, 0x33]
        ]);
    }
}