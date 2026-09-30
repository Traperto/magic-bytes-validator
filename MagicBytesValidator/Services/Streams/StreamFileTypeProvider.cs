namespace MagicBytesValidator.Services.Streams;

public class StreamFileTypeProvider : IStreamFileTypeProvider
{
    /// <inheritdoc />
    public IMapping Mapping { get; }

    public StreamFileTypeProvider(IMapping? mapping = null)
    {
        Mapping = mapping ?? new Mapping();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<IFileType>> FindAllMatchesAsync(
        Stream stream,
        CancellationToken cancellationToken,
        FileByteType validationType = FileByteType.Strict)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var streamBuffer = await stream.ReadAllBytesFromStartAsync(cancellationToken);

        return Mapping.FileTypes
            .Where(fileType => fileType.Matches(streamBuffer, validationType))
            .ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<IFileType>> FindCloseMatchesAsync(
        Stream stream,
        CancellationToken cancellationToken,
        FileByteType validationType = FileByteType.Strict)
    {
        var matches = await FindAllMatchesAsync(stream, cancellationToken, validationType);

        return matches
            .Where(m1 => matches.All(m2 => !m2.GetType().IsSubclassOf(m1.GetType())))
            .ToList();
    }

    /// <inheritdoc />
    public async Task<IFileType?> TryFindUnambiguousAsync(
        Stream stream,
        CancellationToken cancellationToken,
        FileByteType validationType = FileByteType.Strict)
    {
        var closeMatches = await FindCloseMatchesAsync(stream, cancellationToken, validationType);

        return closeMatches.Count == 1 ? closeMatches[0] : null;
    }
}
