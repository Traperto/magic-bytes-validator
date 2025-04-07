namespace MagicBytesValidator.Formats;

/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Xlsx : Zip
{
    public Xlsx() : base(
        ["application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"],
        ["xlsx"]
    )
    {
        Anywhere([
            0x78, 0x6c, 0x2f, 0x5f, 0x72, 0x65, 0x6c, 0x73, 0x2f, 0x77, 0x6f, 0x72, 0x6b, 0x62, 0x6f, 0x6f, 0x6b, 0x2e,
            0x78, 0x6d, 0x6c, 0x2e, 0x72, 0x65, 0x6c, 0x73
        ]);
    }
}