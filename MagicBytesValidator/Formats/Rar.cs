using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
public class Rar : FileTypeWithStartSequences
{
    public Rar() : base(
        new[] { "application/vnd.rar", "application/x-rar-compressed" },
        new[] { "rar" },
        new[]
        {
            new byte[] { 0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x00 },
            new byte[] { 0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x01, 0x00 }
        }
    )
    {
    }
}