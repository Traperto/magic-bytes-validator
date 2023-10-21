using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Tif : FileTypeWithStartSequences
{
    public Tif() : base(
        new[] { "image/tiff" },
        new[] { "tif", "tiff" },
        new[]
        {
            new byte[] { 0x49, 0x20, 0x49 },
            new byte[] { 0x49, 0x49, 0x2A, 0x00 },
            new byte[] { 0x4D, 0x4D, 0x00, 0x2A },
            new byte[] { 0x4D, 0x4D, 0x00, 0x2B }
        }
    )
    {
    }
}