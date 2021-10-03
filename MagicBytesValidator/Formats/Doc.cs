using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats
{
    public class Doc : FileType
    {
        public Doc() : base(
            "application/msword",
            new[] {"doc", "dot"},
            new[]
            {
                new byte[] {208, 207, 17, 224, 161, 177, 26, 225}
            })
        {
        }
    }
}