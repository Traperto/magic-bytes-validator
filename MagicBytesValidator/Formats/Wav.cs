namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/WAV"/>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Wav : FileByteFilter
{
    public Wav() : base(
        ["audio/wav", "audio/x-wav"],
        ["wav"]
    )
    {
        // RIFF header (52 49 46 46) followed by 4 bytes of file size, then WAVE (57 41 56 45)
        StartsWith([0x52, 0x49, 0x46, 0x46, null, null, null, null, 0x57, 0x41, 0x56, 0x45]);
    }
}
