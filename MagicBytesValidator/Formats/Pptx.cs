namespace MagicBytesValidator.Formats;

/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Pptx : Zip
{
    public Pptx() : base(
        new[] { "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
        new[] { "pptx" }
    )
    {
    }
}