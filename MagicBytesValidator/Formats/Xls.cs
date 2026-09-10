namespace MagicBytesValidator.Formats;

/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Xls : FileByteFilter
{
    private static readonly byte?[] WorkbookStreamName = ToUtf16LePattern("Workbook");
    private static readonly byte?[] BookStreamName = ToUtf16LePattern("Book");

    public Xls() : base(
        ["application/msexcel"],
        ["xls", "xla"]
    )
    {
        StartsWith([0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1])
            .AnywhereAnyOf([
                [0xFD, 0xFF, 0xFF, 0xFF, null, 0x00],
                [0xFD, 0xFF, 0xFF, 0xFF, null, 0x02],
                [0xFD, 0xFF, 0xFF, 0xFF, 0x20, 0x00, 0x00, 0x00],
                [0x09, 0x08, 0x10, 0x00, 0x00, 0x06, 0x05, 0x00],
                WorkbookStreamName,
                BookStreamName
            ]);
    }

    private static byte?[] ToUtf16LePattern(string value)
    {
        return System.Text.Encoding.Unicode
            .GetBytes(value)
            .Select(static currentByte => (byte?)currentByte)
            .ToArray();
    }
}