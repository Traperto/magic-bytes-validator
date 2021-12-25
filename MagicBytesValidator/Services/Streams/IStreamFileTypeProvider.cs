using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MagicBytesValidator.Models;

namespace MagicBytesValidator.Services.Streams
{
    public interface IStreamFileTypeProvider
    {
        /// <summary>
        /// Tries to find a <see cref="FileType"/> via given magic byte sequence by given file stream.
        /// Beware that certain file types (e.g. txt files) have no magic bytes sequence and
        /// could therefore be mismatched.
        /// </summary>
        /// <param name="stream">Stream that should be identified</param>
        /// <param name="cancellationToken"Cancellation token</param>
        /// <returns>FileType that belongs to given byte sequence via magic bytes</returns>
        /// <exception cref="ArgumentNullException">When given stream is null</exception>
        Task<FileType?> FindByMagicByteSequenceAsync(Stream stream, CancellationToken cancellationToken);
    }
}
