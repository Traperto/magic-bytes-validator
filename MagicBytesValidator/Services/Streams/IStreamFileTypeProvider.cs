namespace MagicBytesValidator.Services.Streams;

public interface IStreamFileTypeProvider
{
    /// <summary>
    /// Mapping whose file types are matched against the streams
    /// </summary>
    IMapping Mapping { get; }

    /// <summary>
    /// Determines all <see cref="IFileType"/>s that match a given file stream.
    /// Beware that certain file types (e.g. txt files) have no magic bytes sequence and
    /// could therefore be mismatched.
    /// </summary>
    /// <param name="stream">Stream whose content is matched</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <param name="validationType">
    /// Optional. Controls the validation strictness. Defaults to <see cref="FileByteType.Strict"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">When given stream is null</exception>
    Task<IReadOnlyList<IFileType>> FindAllMatchesAsync(
        Stream stream,
        CancellationToken cancellationToken,
        FileByteType validationType = FileByteType.Strict);

    /// <summary>
    /// Determines <see cref="IFileType"/>s that match a given file stream.
    /// If the result contains both a base file type (such as <see cref="Formats.Zip"/>)
    /// and a specific variant of this base (such as <see cref="Formats.Docx"/>, which is 
    /// based on a zip archive), the base type will be omitted and only the specific type
    /// is included as it is "closer" to the file content.
    /// </summary>
    /// <inheritdoc cref="FindAllMatchesAsync" path="/param"/>
    /// <exception cref="ArgumentNullException">When given stream is null</exception>
    Task<IReadOnlyList<IFileType>> FindCloseMatchesAsync(
        Stream stream,
        CancellationToken cancellationToken,
        FileByteType validationType = FileByteType.Strict);

    /// <summary>
    /// Determines close <see cref="IFileType"/>s that match a given file stream and - if exactly
    /// one file type matches - returns this type or otherwise null. Note that, if a stream
    /// matches both a base file type such as <see cref="Formats.Zip"/>) and a specific variant
    /// of this base (such as <see cref="Formats.Docx"/>, the base type won't be taken into
    /// account (as the specific type is "closer" to the file content).
    /// See also <see cref="FindCloseMatchesAsync"/>.
    /// </summary>
    /// <inheritdoc cref="FindAllMatchesAsync" path="/param"/>
    /// <exception cref="ArgumentNullException">When given stream is null</exception>
    Task<IFileType?> TryFindUnambiguousAsync(
        Stream stream,
        CancellationToken cancellationToken,
        FileByteType validationType = FileByteType.Strict);
}