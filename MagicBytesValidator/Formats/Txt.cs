namespace MagicBytesValidator.Formats;

/// <summary>
/// As plain text is not really defined by magic bytes but often uses BOMs that we can look for.
/// Handle this file-type with care!
/// </summary>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Txt : FileByteFilter
{
    public Txt() : base(
        new[] { "text/plain" },
        new[] { "txt" }
    )
    {
        StartsWithAnyOf([
            [0xEF, 0xBB, 0xBF],
            [0xFF, 0xFE],
            [0xFE, 0xFF],
            [0xFF, 0xFE, 0x00, 0x00],
            [0x00, 0x00, 0xFE, 0xFF]
        ]);
    }
}