using System.Linq;
using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
public class Jpg : FileTypeWithStartSequences
{
    public Jpg() : base(
        new[] { "image/jpeg" },
        new[] { "jpg", "jpeg", "jpe", "jif", "jfif", "jfi" },
        new byte[] { 0xFF, 0xD8, 0xFF }
    )
    {
    }

    public override bool Matches(byte[] bytes)
    {
        return base.Matches(bytes)
            && bytes.TakeLast(2).SequenceEqual(new byte[] { 0xFF, 0xD9 });
    }
}