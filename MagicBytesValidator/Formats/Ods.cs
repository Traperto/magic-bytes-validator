namespace MagicBytesValidator.Formats;

/// <see href="https://www.openoffice.org/framework/documentation/mimetypes/mimetypes.html"/>
public class Ods : Zip
{
    public Ods() : base(
        ["application/vnd.oasis.opendocument.spreadsheet"],
        ["ods"]
    )
    {
        Anywhere([.. "mimetypeapplication/vnd.oasis.opendocument.spreadsheet"u8.ToArray().Select(b => (byte?)b)]);
    }
}