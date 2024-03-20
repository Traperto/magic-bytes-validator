namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Odp : Zip
{
    public Odp() : base(
        ["application/vnd.oasis.opendocument.presentation"],
        ["odp"]
    )
    {
    }
}