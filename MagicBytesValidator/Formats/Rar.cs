using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats
{
    public class Rar : FileType
    {
        public Rar() : base(
            new[] { "application/vnd.rar", "application/x-rar-compressed" },
            new[] { "rar" },
            new[]
            {
                new byte[] { 82, 97, 114, 33, 26, 7, 1, 0 }
            })
        {
        }
    }
}