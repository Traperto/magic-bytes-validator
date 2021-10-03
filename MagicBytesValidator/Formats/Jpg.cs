using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats
{
    public class Jpg : FileType
    {
        public Jpg() : base(
            "image/jpeg",
            new[] {"jpg", "jpeg", "jpe"},
            new[]
            {
                new byte[] {255, 216, 255, 219},
                new byte[] {255, 216, 255, 224, 0, 16, 74, 70},
                new byte[] {73, 70, 0, 1},
                new byte[] {255, 216, 255, 238},
                new byte[] {105, 102, 0, 0}
            })
        {
        }
    }
}