using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Midi : FileTypeWithStartSequences
{
    public Midi() : base(
        new[] { "audio/x-midi" },
        new[] { "midi", "mid" },
        new byte[] { 0x4D, 0x54, 0x68, 0x64 }
    )
    {
    }
}