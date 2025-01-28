namespace MagicBytesValidator.Formats;

/// <see href="https://www.openoffice.org/framework/documentation/mimetypes/mimetypes.html"/>
public class Odt : Zip
{
    public Odt() : base(
        ["application/vnd.oasis.opendocument.text"],
        ["odt"]
    )
    {
        Anywhere([.. "mimetypeapplication/vnd.oasis.opendocument.text"u8.ToArray().Select(b => (byte?)b)]);
    }
}