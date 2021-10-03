using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats
{
    public class Bmp : FileType
    {
        public Bmp() : base(
            "image/bmp",
            new[] {"bmp"},
            new[]
            {
                new byte[] {66, 77}
            })
        {
        }
    }
}