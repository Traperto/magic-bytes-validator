namespace MagicBytesValidator.Formats;

public class Ods : Zip
{
    public Ods() : base(
        new[] { "application/vnd.oasis.opendocument.spreadsheet" },
        new[] { "ods" }
    )
    {
    }
}