using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

// TODO: Check if correct

public class Bin : FileTypeWithStartSequences
{
    public Bin() : base(
        new[] { "application/octet-stream" },
        new[] { "bin", "file", "com", "class", "ini" },
        new[]
        {
            new byte[] { 83, 80, 48, 49 },
            new byte[] { 201 },
            new byte[] { 202, 254, 186, 190 }
        }
    )
    {
    }
}