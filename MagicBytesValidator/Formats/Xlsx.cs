using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats
{
    public class Xlsx : FileType
    {
        public Xlsx() : base(
                             "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                             new[] { "xlsx" },
                             new[]
                             {
                                 new byte[] { 80, 75, 3, 4 },
                                 new byte[] { 80, 75, 5, 6 },
                                 new byte[] { 80, 75, 7, 8 }
                             })
        {
        }
    }
}