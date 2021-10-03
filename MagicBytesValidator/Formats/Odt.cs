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
                new byte[] {80, 75, 7, 8}
            })
        {
        }
    }
}