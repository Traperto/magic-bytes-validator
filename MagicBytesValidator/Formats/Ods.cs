namespace MagicBytesValidator.Formats;

public class Ods : Zip
{
    public Ods() : base(
        ["application/vnd.oasis.opendocument.spreadsheet"],
        ["ods"]
    )
    {
    }
}