using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats
{
    public class Xls : FileType
    {
        public Xls() : base(
            new[] { "application/msexcel" },
            new[] { "xls", "xla" },
            new[]
            {
                new byte[] { 208, 207, 17, 224, 161, 177, 26, 225 }
            })
        {
        }
    }
}