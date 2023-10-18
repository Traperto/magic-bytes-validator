using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MagicBytesValidator.Models;

namespace MagicBytesValidator.Services;

public interface IValidator
{
    /// <summary>
    /// Mapping that is used during validation
    /// </summary>
    Mapping Mapping { get; }

    /// <summary>
    /// Validates a given file-Stream against a given FileType and returns if the Stream is valid or not.
    /// </summary>
    /// <param name="fileStream">Stream of the file that should be validated</param>
    /// <param name="fileType">FileType that the stream should be validated against</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns whether the Stream matches one of the FileStream's magic-byte sequences.</returns>
    Task<bool> IsValidAsync(Stream fileStream, FileType fileType, CancellationToken cancellationToken);
}