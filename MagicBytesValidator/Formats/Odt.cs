namespace MagicBytesValidator.Formats;

public class Odt : Zip
{
    public Odt() : base(
        ["application/vnd.oasis.opendocument.text"],
        ["odt"]
    )
    {
    }
}