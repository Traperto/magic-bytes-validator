namespace MagicBytesValidator.Services.Streams;

public class StreamFileTypeProvider : IStreamFileTypeProvider
{
    /// <summary>
    /// Mapping whose file types are matched against the streams
    /// </summary>
    public IMapping Mapping { get; }

    public StreamFileTypeProvider(IMapping? mapping = null)
    {
        Mapping = mapping ?? new Mapping();
    }

    [Obsolete("Use TryFindUnambiguousAsync instead. FindByMagicByteSequenceAsync will be removed in version 3.0.")]
    public Task<IFileType?> FindByMagicByteSequenceAsync(Stream stream, CancellationToken cancellationToken)
    {
        return TryFindUnambiguousAsync(stream, cancellationToken);
    }

    /// <inheritdoc />
    public Task<IEnumerable<IFileType>> FindAllMatchesAsync(Stream stream, CancellationToken cancellationToken)
    {
        return FindAllMatchesAsync(stream, cancellationToken, FileByteType.Strict);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<IFileType>> FindAllMatchesAsync(
        Stream stream,
        CancellationToken cancellationToken,
        FileByteType validationType)
    {
        if (stream is null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        var streamBuffer = await stream.ReadAllBytesFromStartAsync(cancellationToken);

        return Mapping.FileTypes
            .Where(fileType => fileType.Matches(streamBuffer, validationType))
            .ToList();
    }

    /// <inheritdoc />
    public Task<IEnumerable<IFileType>> FindCloseMatchesAsync(Stream stream, CancellationToken cancellationToken)
    {
        return FindCloseMatchesAsync(stream, cancellationToken, FileByteType.Strict);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<IFileType>> FindCloseMatchesAsync(
        Stream stream,
        CancellationToken cancellationToken,
        FileByteType validationType)
    {
        var matches = (await FindAllMatchesAsync(stream, cancellationToken, validationType)).ToList();

        return matches
            .Where(m1 => matches.All(m2 => !m2.GetType().IsSubclassOf(m1.GetType())))
            .ToList();
    }

    /// <inheritdoc />
    public Task<IFileType?> TryFindUnambiguousAsync(Stream stream, CancellationToken cancellationToken)
    {
        return TryFindUnambiguousAsync(stream, cancellationToken, FileByteType.Strict);
    }

    /// <inheritdoc />
    public async Task<IFileType?> TryFindUnambiguousAsync(
        Stream stream,
        CancellationToken cancellationToken,
        FileByteType validationType)
    {
        var closeMatches = (await FindCloseMatchesAsync(stream, cancellationToken, validationType)).ToList();

        return closeMatches.Count == 1 ? closeMatches[0] : null;
    }
}
