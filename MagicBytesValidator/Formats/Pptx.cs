using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

public class Pptx : FileTypeWithStartSequences
{
    public Pptx() : base(
        new[] { "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
        new[] { "pptx" },
        new[]
        {
            new byte[] { 80, 75, 3, 4 },
            new byte[] { 80, 75, 5, 6 },
            new byte[] { 80, 75, 7, 8 }
        }
    )
    {
    }
}