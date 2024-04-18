namespace MagicBytesValidator.Formats;

// TODO: Check if correct

public class Bin : FileByteFilter
{
    public Bin() : base(
        ["application/octet-stream"],
        ["bin", "file", "com", "class", "ini"]
    )
    {
        StartsWithAnyOf([
            [0x53, 0x50, 0x30, 0x31],
            [0xC9],
            [0xCA, 0xFE, 0xBA, 0xBE]
        ]);
    }
}