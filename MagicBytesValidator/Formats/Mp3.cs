using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats
{
    public class Mp3 : FileType
    {
        public Mp3() : base(
            new[] { "audio/mpeg" },
            new[] { "mp3" },
            new[]
            {
                new byte[] { 73, 68, 51 }
            })
        {
        }
    }
}