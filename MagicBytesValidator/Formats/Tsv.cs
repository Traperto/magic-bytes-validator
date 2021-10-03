using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats
{
    public class Tsv : FileType
    {
        public Tsv() : base(
            "text/tab-separated-values",
            new[] {"tsv"},
            new[]
            {
                new byte[] {71}
            })
        {
        }
    }
}