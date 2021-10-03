using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats
{
    public class Ogv : FileType
    {
        public Ogv() : base(
            "video/ogg",
            new[] {"ogv", "ogg"},
            new[]
            {
                new byte[] {79, 103, 103, 83}
            })
        {
        }
    }
}