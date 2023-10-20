using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

public class Tsp : FileTypeWithStartSequences
{
    public Tsp() : base(
        new[] { "application/dsptype" },
        new[] { "tsp" },
        new[]
        {
            new byte[] { 77, 90 }
        }
    )
    {
    }
}