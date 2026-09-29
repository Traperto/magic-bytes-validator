namespace MagicBytesValidator.Formats;

/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Doc : FileByteFilter
{
    public Doc() : base(
        ["application/msword"],
        ["doc", "dot"]
    )
    {
        // DOC, XLS and PPT share the OLE header; the root "WordDocument" stream identifies Word (6.0 and later).
        StartsWith([0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1])
            .CompoundFileStreamAnyOf(["WordDocument"]);
    }
}
