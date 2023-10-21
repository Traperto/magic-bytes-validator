namespace MagicBytesValidator.Formats;

/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Docx : Zip
{
    public Docx() : base(
        new[] { "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
        new[] { "docx" }
    )
    {
    }
}