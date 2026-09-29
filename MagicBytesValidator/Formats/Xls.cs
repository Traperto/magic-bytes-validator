namespace MagicBytesValidator.Formats;

/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Xls : FileByteFilter
{
    public Xls() : base(
        ["application/msexcel"],
        ["xls", "xla"]
    )
    {
        // DOC, XLS and PPT share the OLE header; the root "Workbook" (Excel 97+) or "Book" (Excel 5/95) stream
        // identifies Excel.
        StartsWith([0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1])
            .CompoundFileStreamAnyOf(["Workbook", "Book"]);
    }
}
