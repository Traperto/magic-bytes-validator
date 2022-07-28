using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats
{
    public class Xlsx : FileType
    {
        public Xlsx() : base(
                             "application/vnd.openxmlformats-officedocument.wordprocessingml.sheet",
                             new[] { "xlsx" },
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