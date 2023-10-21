using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Mp3 : FileTypeWithStartSequences
{
    public Mp3() : base(
        new[] { "audio/mpeg" },
        new[] { "mp3" },
        new[]
        {
            new byte[] { 0x49, 0x44, 0x33 },
            new byte[] { 0xFF, 0xFB },
            new byte[] { 0xFF, 0xF3 },
            new byte[] { 0xFF, 0xF2 },
        }
    )
    {
    }
}