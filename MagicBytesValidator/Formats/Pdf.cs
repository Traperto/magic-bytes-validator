using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats
{
    public class Pdf : FileType
    {
        public Pdf() : base(
            new[] { "application/pdf" },
            new[] { "pdf" },
            new[]
            {
                new byte[] { 0x25, 0x50, 0x44, 0x46 }
            })
        {
        }
    }
}