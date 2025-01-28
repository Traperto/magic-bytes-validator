namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
/// <see href="https://www.openoffice.org/framework/documentation/mimetypes/mimetypes.html"/>
public class Odp : Zip
{
    public Odp() : base(
        ["application/vnd.oasis.opendocument.presentation"],
        ["odp"]
    )
    {
        Anywhere([.. "mimetypeapplication/vnd.oasis.opendocument.presentation"u8.ToArray().Select(b => (byte?)b)]);
    }
}