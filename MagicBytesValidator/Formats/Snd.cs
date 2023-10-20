using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

public class Snd : FileTypeWithStartSequences
{
    public Snd() : base(
        new[] { "audio/basic" },
        new[] { "snd", "au" },
        new[]
        {
            new byte[] { 56, 83, 86, 88 },
            new byte[] { 65, 73, 70, 70 }
        }
    )
    {
    }
}