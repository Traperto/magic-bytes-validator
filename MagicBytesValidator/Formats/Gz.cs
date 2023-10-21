using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Gz : FileTypeWithStartSequences
{
    public Gz() : base(
        new[] { "application/gzip" },
        new[] { "gz" },
        new byte[] { 0x1F, 0x8B }
    )
    {
    }
}