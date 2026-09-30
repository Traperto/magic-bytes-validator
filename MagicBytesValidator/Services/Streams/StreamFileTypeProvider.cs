namespace MagicBytesValidator.Services.Streams;

public class StreamFileTypeProvider : IStreamFileTypeProvider
{
    private readonly IMapping _mapping;

    public StreamFileTypeProvider(IMapping mapping)
    {
        _mapping = mapping;
    }

    [Obsolete("Use TryFindUnambiguousAsync instead")]
    public Task<IFileType?> FindByMagicByteSequenceAsync(Stream stream, CancellationToken cancellationToken)
    {
        return TryFindUnambiguousAsync(stream, cancellationToken);
    }

    public async Task<IEnumerable<IFileType>> FindAllMatchesAsync(Stream stream, CancellationToken cancellationToken)
    {
        if (stream is null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        var streamBuffer = await stream.ReadAllBytesFromStartAsync(cancellationToken);

        return _mapping.FileTypes.Where(fileType => fileType.Matches(streamBuffer));
    }

    public async Task<IEnumerable<IFileType>> FindCloseMatchesAsync(Stream stream, CancellationToken cancellationToken)
    {
        var matches = (await FindAllMatchesAsync(stream, cancellationToken)).ToList();

        return matches.Where(m1 =>
            matches.All(m2 => !m2.GetType().IsSubclassOf(m1.GetType())));
    }

    public async Task<IFileType?> TryFindUnambiguousAsync(Stream stream, CancellationToken cancellationToken)
    {
        var closeMatches = (await FindCloseMatchesAsync(stream, cancellationToken)).ToList();

        return closeMatches.Count == 1 ? closeMatches[0] : null;
    }
}
