namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/MPEG-4_Part_14"/>
/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
public class M4a : FileByteFilter
{
    public M4a() : base(
        ["audio/mp4", "audio/x-m4a"],
        ["m4a"]
    )
    {
        // ftyp box at offset 4, followed by M4A or M4B brand
        StartsWithAnyOf([
            [null, null, null, null, 0x66, 0x74, 0x79, 0x70, 0x4D, 0x34, 0x41, 0x20], // ftypM4A
            [null, null, null, null, 0x66, 0x74, 0x79, 0x70, 0x4D, 0x34, 0x42, 0x20], // ftypM4B
        ]);
    }
}
