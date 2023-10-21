using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Z : FileTypeWithStartSequences
{
    public Z() : base(
        new[] { "application/x-compress" },
        new[] { "z" },
        new[]
        {
            new byte[] { 0x1F, 0x9D },
            new byte[] { 0x1F, 0xA0 }
        }
    )
    {
    }
}