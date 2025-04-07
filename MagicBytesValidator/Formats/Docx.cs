namespace MagicBytesValidator.Formats;

/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Docx : Zip
{
    public Docx() : base(
        ["application/vnd.openxmlformats-officedocument.wordprocessingml.document"],
        ["docx"])
    {
        Anywhere([
            0x77, 0x6F, 0x72, 0x64, 0x2F, 0x5F, 0x72, 0x65, 0x6C, 0x73, 0x2F, 0x64, 0x6F,
            0x63, 0x75, 0x6D, 0x65, 0x6E, 0x74, 0x2E, 0x78, 0x6D, 0x6C, 0x2E, 0x72, 0x65, 0x6C, 0x73
        ]);
    }
}