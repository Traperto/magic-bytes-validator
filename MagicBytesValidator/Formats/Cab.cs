using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats
{
    public class Cab : FileType
    {
        public Cab() : base(
            "application/x-shockwave-flash",
            new[] {"cab", "swf"},
            new[]
            {
                new byte[] {77, 83, 67, 70},
                new byte[] {67, 87, 83},
                new byte[] {73, 83, 99, 40}
            })
        {
        }
    }
}