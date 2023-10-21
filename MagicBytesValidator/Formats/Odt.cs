namespace MagicBytesValidator.Formats;

public class Odt : Zip
{
    public Odt() : base(
        new[] { "application/vnd.oasis.opendocument.text" },
        new[] { "odt" }
    )
    {
    }
}