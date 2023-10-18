using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

public class Ods : FileType
{
    public Ods() : base(
        new[] { "application/vnd.oasis.opendocument.spreadsheet" },
        new[] { "ods" },
        new[]
        {
            new byte[] { 80, 75, 7, 8 }
        }
    )
    {
    }
}