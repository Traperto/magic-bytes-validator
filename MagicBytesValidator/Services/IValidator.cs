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
    Task<bool> IsValidAsync(Stream fileStream, IFileType fileType, CancellationToken cancellationToken);
}