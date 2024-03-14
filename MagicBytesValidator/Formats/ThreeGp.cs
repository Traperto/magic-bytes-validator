namespace MagicBytesValidator.Formats;

/// <summary>
/// Definition for 3GP (as C# does not allow leading numbers for class names, we call it "ThreeGP" here.
/// </summary>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
public class ThreeGp : FileByteFilter
{
    public ThreeGp() : base(
        ["video/3gpp"],
        ["3gp"]
    )
    {
        StartsWith([null, null, null, null, 0x66, 0x74, 0x79, 0x70, 0x33, 0x67, 0x70]);
    }
}