using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats
{
    public class Tsv : FileType
    {
        public Tsv() : base(
            new[] { "text/tab-separated-values" },
            new[] { "tsv" },
            new[]
            {
                new byte[] { 71 }
            })
        {
        }
    }
}