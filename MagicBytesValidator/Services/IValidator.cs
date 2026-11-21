namespace MagicBytesValidator.Services;

public interface IValidator
{
    /// <summary>
    /// Mapping that is used during validation
    /// </summary>
    /// <remarks>Will be typed as <see cref="IMapping"/> in version 3.0.</remarks>
    Mapping Mapping { get; }

    /// <summary>
    /// Validates a given file-Stream against a given FileType and returns if the Stream is valid or not.
    /// </summary>
    Task<bool> IsValidAsync(
       Stream fileStream,
       IFileType fileType,
       CancellationToken cancellationToken,
       FileByteType validationType = FileByteType.Strict
    );
}
