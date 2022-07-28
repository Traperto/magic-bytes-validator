using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats
{
    public class Odt : FileType
    {
        public Odt() : base(
            "application/vnd.oasis.opendocument.text",
            new[] {"odt"},
            new[]
            {
                new byte[] { 50, 75, 3, 4 },
                new byte[] { 50, 75, 5, 6 },
                new byte[] { 50, 75, 7, 8 }
            })
        {
        }
    }
}