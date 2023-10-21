using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Dxr : FileTypeWithIncompleteStartSequences
{
    public Dxr() : base(
        new[] { "application/x-director" },
        new[] { "dxr", "dcr", "dir" },
        new[]
        {
            new byte?[] { 0x52, 0x49, 0x46, 0x58, null, null, null, null, 0x4D, 0x56, 0x39, 0x33 },
            new byte?[] { 0x58, 0x46, 0x49, 0x52, null, null, null, null, 0x33, 0x39, 0x56, 0x4D }
        }
    )
    {
    }
}