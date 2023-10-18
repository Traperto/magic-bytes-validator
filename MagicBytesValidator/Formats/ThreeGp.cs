using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

/// <summary>
/// Definition for 3GP (as C# does not allow leading numbers for class names, we call it "ThreeGP" here.
/// </summary>
public class ThreeGp : FileType
{
    public ThreeGp() : base(
        new[] { "video/3gpp" },
        new[] { "3gp" },
        new[]
        {
            new byte[] { 102, 116, 121, 112, 51, 103 }
        }
    )
    {
    }
}