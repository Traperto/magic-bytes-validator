namespace MagicBytesValidator.Services.Streams;

public interface IStreamFileTypeProvider
{
    /// <summary>
    /// Tries to find a <see cref="IFileType"/> via given magic byte sequence by given file stream.
    /// Beware that certain file types (e.g. txt files) have no magic bytes sequence and
    /// could therefore be mismatched.
    /// </summary>
    /// <exception cref="ArgumentNullException">When given stream is null</exception>
    [Obsolete("Use TryFindUnambiguousAsync instead. FindByMagicByteSequenceAsync will be removed in version 3.0.")]
    Task<IFileType?> FindByMagicByteSequenceAsync(Stream stream, CancellationToken cancellationToken);

    /// <summary>
    /// Determines all <see cref="IFileType"/>s that match a given file stream.
    /// </summary>
    Task<IEnumerable<IFileType>> FindAllMatchesAsync(Stream stream, CancellationToken cancellationToken);

    /// <summary>
    /// Determines all <see cref="IFileType"/>s that match a given file stream using the given validation type.
    /// </summary>
    /// <param name="stream">Stream whose content is matched</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <param name="validationType">Controls the validation strictness</param>
    /// <exception cref="ArgumentNullException">When given stream is null</exception>
    /// <remarks>
    /// The default implementation only exists for compatibility with existing implementations of this interface
    /// and supports <see cref="FileByteType.Strict"/> only.
    /// </remarks>
    Task<IEnumerable<IFileType>> FindAllMatchesAsync(
        Stream stream,
        CancellationToken cancellationToken,
        FileByteType validationType)
    {
        return validationType == FileByteType.Strict
            ? FindAllMatchesAsync(stream, cancellationToken)
            : throw new NotSupportedException($"{GetType().Name} does not support {validationType} validation.");
    }

    /// <summary>
    /// Determines <see cref="IFileType"/>s that match a given file stream.
    /// If the result contains both a base file type (such as <see cref="Formats.Zip"/>)
    /// and a specific variant of this base (such as <see cref="Formats.Docx"/>, which is
    /// based on a zip archive), the base type will be omitted and only the specific type
    /// is included as it is "closer" to the file content.
    /// </summary>
    Task<IEnumerable<IFileType>> FindCloseMatchesAsync(Stream stream, CancellationToken cancellationToken);

    /// <summary>
    /// Like <see cref="FindCloseMatchesAsync(Stream, CancellationToken)"/>, but using the given validation type.
    /// </summary>
    /// <inheritdoc cref="FindAllMatchesAsync(Stream, CancellationToken, FileByteType)" path="/param"/>
    /// <inheritdoc cref="FindAllMatchesAsync(Stream, CancellationToken, FileByteType)" path="/remarks"/>
    Task<IEnumerable<IFileType>> FindCloseMatchesAsync(
        Stream stream,
        CancellationToken cancellationToken,
        FileByteType validationType)
    {
        return validationType == FileByteType.Strict
            ? FindCloseMatchesAsync(stream, cancellationToken)
            : throw new NotSupportedException($"{GetType().Name} does not support {validationType} validation.");
    }

    /// <summary>
    /// Determines close <see cref="IFileType"/>s that match a given file stream and - if only
    /// one file type matches - return this type or otherwise null. Note that, if a stream
    /// matches both a base file type such as <see cref="Formats.Zip"/>) and a specific variant
    /// of this base (such as <see cref="Formats.Docx"/>, the base type won't be taken into
    /// account (as the specific type is "closer" to the file content).
    /// See also <see cref="FindCloseMatchesAsync(Stream, CancellationToken)"/>.
    /// </summary>
    Task<IFileType?> TryFindUnambiguousAsync(Stream stream, CancellationToken cancellationToken);

    /// <summary>
    /// Like <see cref="TryFindUnambiguousAsync(Stream, CancellationToken)"/>, but using the given validation type.
    /// </summary>
    /// <inheritdoc cref="FindAllMatchesAsync(Stream, CancellationToken, FileByteType)" path="/param"/>
    /// <inheritdoc cref="FindAllMatchesAsync(Stream, CancellationToken, FileByteType)" path="/remarks"/>
    Task<IFileType?> TryFindUnambiguousAsync(
        Stream stream,
        CancellationToken cancellationToken,
        FileByteType validationType)
    {
        return validationType == FileByteType.Strict
            ? TryFindUnambiguousAsync(stream, cancellationToken)
            : throw new NotSupportedException($"{GetType().Name} does not support {validationType} validation.");
    }
}
