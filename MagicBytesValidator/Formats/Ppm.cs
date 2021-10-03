using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats
{
    public class Ppm : FileType
    {
        public Ppm() : base(
            "image/x-portable-pixmap",
            new[] {"ppm"},
            new[]
            {
                new byte[] {80, 51, 10}
            })
        {
        }
    }
}