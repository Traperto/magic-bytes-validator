using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats
{
    public class Gz : FileType
    {
        public Gz() : base(
            "application/gzip",
            new[] {"gz"},
            new[]
            {
                new byte[] {31, 139}
            })
        {
        }
    }
}