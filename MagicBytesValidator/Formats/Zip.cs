using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats
{
    public class Zip : FileType
    {
        public Zip() : base(
            "application/zip",
            new[] {"zip"},
            new[]
            {
                new byte[] {80, 75, 3, 4}
            })
        {
        }
    }
}