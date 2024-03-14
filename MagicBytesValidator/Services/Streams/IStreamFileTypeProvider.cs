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
    /// Tries to determine an unambiguous <see cref="IFileType"/> that matches a given file stream.
    /// Returns <see cref="IFileType"/> in case it's the only (registered) type that matches.
    /// As soon as multiple file types match the file, null will be returned.
    /// If no type matches, null will be returned.
    /// </summary>
    /// <returns>Only one matching (known) <see cref="IFileType"/> that matches.</returns>
    Task<IFileType?> TryFindUnambiguousAsync(Stream stream, CancellationToken cancellationToken);
}