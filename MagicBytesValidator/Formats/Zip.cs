namespace MagicBytesValidator.Formats;

/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Zip : FileByteFilter
{
    public Zip() : this(
        ["application/zip", "application/x-zip-compressed"],
        ["zip"]
    )
    {
    }

    public Zip(string[] mimeTypes, string[] extensions) : base(mimeTypes, extensions)
    {
        StartsWithAnyOf([
                [0x50, 0x4B, 0x03, 0x04],
                [0x50, 0x4B, 0x05, 0x06],
                [0x50, 0x4B, 0x07, 0x08]
            ])
            .EndsWith([
                0x50, 0x4B, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, 0x00, 0x00, 0x00
            ]);
    }
}