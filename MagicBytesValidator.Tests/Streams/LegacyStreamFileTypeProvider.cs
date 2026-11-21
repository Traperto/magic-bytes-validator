namespace MagicBytesValidator.Tests.Streams;

/// <summary>
/// Ensures that implementations written against 2.5 (without the validationType overloads) keep working.
/// </summary>
public class LegacyStreamFileTypeProvider
{
    private static readonly IFileType[] Matches = [new Gif()];

    [Fact]
    public async Task Should_delegate_strict_validation_to_existing_methods()
    {
        IStreamFileTypeProvider sut = new LegacyProvider();
        var stream = new MemoryStream();

        Assert.Same(Matches, await sut.FindAllMatchesAsync(stream, CancellationToken.None, FileByteType.Strict));
        Assert.Same(Matches, await sut.FindCloseMatchesAsync(stream, CancellationToken.None, FileByteType.Strict));
        Assert.Same(Matches[0], await sut.TryFindUnambiguousAsync(stream, CancellationToken.None, FileByteType.Strict));
    }

    [Fact]
    public async Task Should_reject_other_validation_types()
    {
        IStreamFileTypeProvider sut = new LegacyProvider();
        var stream = new MemoryStream();

        await Assert.ThrowsAsync<NotSupportedException>(() =>
            sut.FindAllMatchesAsync(stream, CancellationToken.None, FileByteType.Lazy));
        await Assert.ThrowsAsync<NotSupportedException>(() =>
            sut.FindCloseMatchesAsync(stream, CancellationToken.None, FileByteType.Lazy));
        await Assert.ThrowsAsync<NotSupportedException>(() =>
            sut.TryFindUnambiguousAsync(stream, CancellationToken.None, FileByteType.Lazy));
    }

    private sealed class LegacyProvider : IStreamFileTypeProvider
    {
        [Obsolete("Only implemented to satisfy the interface.")]
        public Task<IFileType?> FindByMagicByteSequenceAsync(Stream stream, CancellationToken cancellationToken)
        {
            return TryFindUnambiguousAsync(stream, cancellationToken);
        }

        public Task<IEnumerable<IFileType>> FindAllMatchesAsync(Stream stream, CancellationToken cancellationToken)
        {
            return Task.FromResult<IEnumerable<IFileType>>(Matches);
        }

        public Task<IEnumerable<IFileType>> FindCloseMatchesAsync(Stream stream, CancellationToken cancellationToken)
        {
            return Task.FromResult<IEnumerable<IFileType>>(Matches);
        }

        public Task<IFileType?> TryFindUnambiguousAsync(Stream stream, CancellationToken cancellationToken)
        {
            return Task.FromResult<IFileType?>(Matches[0]);
        }
    }
}
