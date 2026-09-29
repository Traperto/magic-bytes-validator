namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
public class Ppt : FileByteFilter
{
    public Ppt() : base(
        ["application/mspowerpoint", "application/vnd.ms-powerpoint"],
        ["ppt", "ppz", "pps", "pot"]
    )
    {
        // DOC, XLS and PPT share the OLE header; the root "PowerPoint Document" stream identifies PowerPoint.
        StartsWith([0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1])
            .CompoundFileStreamAnyOf(["PowerPoint Document"]);
    }
}