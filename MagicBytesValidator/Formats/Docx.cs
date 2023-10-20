using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

public class Docx : FileTypeWithStartSequences
{
    public Docx() : base(
        new[] { "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
        new[] { "docx" },
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