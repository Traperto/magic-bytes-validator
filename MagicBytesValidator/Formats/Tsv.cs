namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
/// <see href="https://en.wikipedia.org/wiki/MPEG_transport_stream"/>
public class Tsv : FileByteFilter
{
    public Tsv() : base(
        ["video/mp2t"],
        ["ts", "tsa"]
    )
    {
        /* A transport stream consists of 188-byte packets that all start with the sync byte 0x47 ("G").
         * Checking the first byte alone would also match e.g. every GIF, so we check the second packet as well. */
        StartsWith([0x47])
            .Specific(new ByteCheck(188, [0x47]));
    }
}