using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Cab : FileTypeWithStartSequences
{
    public Cab() : base(
        new[] { "application/vnd.ms-cab-compressed", "application/x-cab-compressed" },
        new[] { "cab" },
        new[]
        {
            new byte[] { 0x49, 0x53, 0x63, 0x28 },
            new byte[] { 0x4D, 0x53, 0x43, 0x46 }
        }
    )
    {
    }
}