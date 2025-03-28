namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
public class Jpg : FileByteFilter
{
    public Jpg() : base(
        ["image/jpeg"],
        ["jpg", "jpeg", "jpe", "jif", "jfif", "jfi"]
    )
    {
        /* - JPEG must start with its header
         * - Image data must stop with trailer 0xFF 0xD9
         * - JPEG may contain metadata after trailer.
         *   - Currently available: Samsung specific metadata (ending on "SEFT") */
       
        // TODO: We might remove the `EndsWithAnyOf` as metadata can be quite diverse.
        StartsWith([0xFF, 0xD8, 0xFF])
            .Anywhere([0xFF, 0xD9])
            .EndsWithAnyOf([
                [0xFF, 0xD9],
                [0x53, 0x45, 0x46, 0x54]
            ]);
    }
}