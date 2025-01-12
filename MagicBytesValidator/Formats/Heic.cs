namespace MagicBytesValidator.Formats;

/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Heic : FileByteFilter
{
  public Heic() : base(
      ["image/heic", "image/heif"],
      ["heic", "heif"]
  )
  {
    Specific(new ByteCheck(4, [0x66, 0x74, 0x79, 0x70, 0x68, 0x65, 0x69, 0x63]));
  }

}