namespace MagicBytesValidator.Formats;

/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
public class Pdf : FileByteFilter
{
   public Pdf() : base(
      ["application/pdf"],
      ["pdf"]
   )
   {
      StartsWith([0x25, 0x50, 0x44, 0x46, 0x2D]); // %PDF-

      // Some tools append line breaks or other whitespace after the %%EOF marker (#194).
      EndsWithIgnoringTrailing(
         [0x25, 0x25, 0x45, 0x4F, 0x46], // %%EOF
         [0x00, 0x09, 0x0A, 0x0C, 0x0D, 0x20], // PDF whitespace characters (ISO 32000-1, 7.2.2)
         FileByteType.Strict
      );

      TailContains(
         1024,
         [0x25, 0x25, 0x45, 0x4F, 0x46], // %%EOF
         FileByteType.Lazy
      );
   }
}
