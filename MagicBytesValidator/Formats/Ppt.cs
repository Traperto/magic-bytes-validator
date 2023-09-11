using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats
{
    public class Ppt : FileType
    {
        public Ppt() : base(
            new[] { "application/mspowerpoint" },
            new[] { "ppt", "ppz", "pps", "pot" },
            new[]
            {
                new byte[] { 208, 207, 17, 224, 161, 177, 26, 225 }
            })
        {
        }
    }
}