using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats
{
    public class Mp4 : FileType
    {
        public Mp4() : base(
            "audio/mp4",
            new[] {"mp4"},
            new[]
            {
                new byte[] {102, 116, 121, 112, 105, 115, 111, 109}
            })
        {
        }
    }
}