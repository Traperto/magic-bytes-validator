using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats
{
    public class Docx : FileType
    {
        public Docx() : base(
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            new[] {"docx", "xlsx"},
            new[]
            {
                new byte[] {80, 75, 7, 8}
            })
        {
        }
    }
}