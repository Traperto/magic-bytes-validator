using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats
{
    public class Pptx : FileType
    {
        public Pptx() : base(
                             "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                             new[] { "pptx" },
                             new[]
                             {
                                 new byte[] { 50, 75, 3, 4 },
                                 new byte[] { 50, 75, 5, 6 },
                                 new byte[] { 50, 75, 7, 8 }
                             })
        {
        }
    }
}