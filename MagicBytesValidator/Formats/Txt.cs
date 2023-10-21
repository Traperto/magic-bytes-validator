using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

/// <summary>
/// As plain text is not really defined by magic bytes but often uses BOMs that we can look for.
/// Handle this file-type with care!
/// </summary>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Txt : FileTypeWithStartSequences
{
    public Txt() : base(
        new[] { "text/plain" },
        new[] { "txt" },
        new[]
        {
            new byte[] { 0xEF, 0xBB, 0xBF },
            new byte[] { 0xFF, 0xFE },
            new byte[] { 0xFE, 0xFF },
            new byte[] { 0xFF, 0xFE, 0x00, 0x00 },
            new byte[] { 0x00, 0x00, 0xFE, 0xFF }
        }
    )
    {
    }
}