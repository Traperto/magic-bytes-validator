using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

/// <summary>
/// As plain text is not really defined by magic bytes, handle with care when using this file-type.
/// </summary>
public class Txt : FileType
{
    public Txt() : base(
        new[] { "text/plain" },
        new[] { "txt" },
        new[]
        {
            new byte[] { 239, 187, 191 },
            new byte[] { 255, 254 },
            new byte[] { 254, 255 },
            new byte[] { 255, 254, 0, 0 }
        }
    )
    {
    }
}