using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

/// <see href="https://github.com/mime-types/mime-types-data/issues/14"/>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
public class Rpm : FileTypeWithStartSequences
{
    public Rpm() : base(
        new[] { "application/x-rpm", "application/x-redhat-package-manager" },
        new[] { "rpm" },
        new byte[] { 0xED, 0xAB, 0xEE, 0xDB }
    )
    {
    }
}