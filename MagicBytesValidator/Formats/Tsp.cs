using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats
{
    public class Tsp : FileType
    {
        public Tsp() : base(
            "application/dsptype",
            new[] {"tsp"},
            new[]
            {
                new byte[] {77, 90}
            })
        {
        }
    }
}