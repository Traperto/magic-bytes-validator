using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

public class Dxr : FileTypeWithStartSequences
{
    public Dxr() : base(
        new[] { "application/x-director" },
        new[] { "dxr", "dcr", "dir" },
        new[]
        {
            new byte[] { 77, 86, 57, 51 }
        }
    )
    {
    }
}