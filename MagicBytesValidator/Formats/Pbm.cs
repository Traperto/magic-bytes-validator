using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats
{
    public class Pbm : FileType
    {
        public Pbm() : base(
            "image/x-portable-bitmap",
            new[] {"pbm"},
            new[]
            {
                new byte[] {80, 49, 10}
            })
        {
        }
    }
}