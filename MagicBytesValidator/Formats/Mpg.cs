using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats
{
    public class Mpg : FileType
    {
        public Mpg() : base(
            "video/mpeg",
            new[] {"mpg", "mpeg", "mpe"},
            new[]
            {
                new byte[] {71},
                new byte[] {0, 0, 1, 186},
                new byte[] {0, 0, 1, 179}
            })
        {
        }
    }
}