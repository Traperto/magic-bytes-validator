namespace MagicBytesValidator.Services.Streams;

public interface IStreamFileTypeProvider
{
    /// <summary>
    /// Tries to find a <see cref="IFileType"/> via given magic byte sequence by given file stream.
    /// Beware that certain file types (e.g. txt files) have no magic bytes sequence and
    /// could therefore be mismatched.
    /// </summary>
    /// <exception cref="ArgumentNullException">When given stream is null</exception>
    [Obsolete("Use TryFindUnambiguousAsync instead.")]
    Task<IFileType?> FindByMagicByteSequenceAsync(Stream stream, CancellationToken cancellationToken);

    /// <summary>
    /// Determines all <see cref="IFileType"/>s that match a given file stream.
    /// </summary>
    Task<IEnumerable<IFileType>> FindAllMatchesAsync(Stream stream, CancellationToken cancellationToken);

    /// <summary>
    /// Determines <see cref="IFileType"/>s that match a given file stream.
    /// If the result contains both a base file type (such as <see cref="Formats.Zip"/>)
    /// and a specific variant of this base (such as <see cref="Formats.Docx"/>, which is 
    /// based on a zip archive), the base type will be omitted and only the specific type
    /// is included as it is "closer" to the file content.
    /// </summary>
    Task<IEnumerable<IFileType>> FindCloseMatchesAsync(Stream stream, CancellationToken cancellationToken);

    /// <summary>
    /// Determines close <see cref="IFileType"/>s that match a given file stream and - if only
    /// one file type matches - return this type or otherwise null. Note that, if a stream
    /// matches both a base file type such as <see cref="Formats.Zip"/>) and a specific variant
    /// of this base (such as <see cref="Formats.Docx"/>, the base type won't be taken into
    /// account (as the specific type is "closer" to the file content).
    /// See also <see cref="FindCloseMatchesAsync"/>.
    /// </summary>
    Task<IFileType?> TryFindUnambiguousAsync(Stream stream, CancellationToken cancellationToken);
}