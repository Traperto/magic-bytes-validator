namespace MagicBytesValidator.Tests;

public class StreamReadingTests
{
    private static readonly byte[] GifBytes = [0x47, 0x49, 0x46, 0x38, 0x39, 0x61, 0x11, 0x12];

    [Fact]
    public async Task Validator_should_validate_non_seekable_stream()
    {
        var validator = new Validator();
        using var stream = new NonSeekableStream(GifBytes);

        var isValid = await validator.IsValidAsync(stream, new Gif(), CancellationToken.None);

        Assert.True(isValid);
    }

    [Fact]
    public async Task StreamFileTypeProvider_should_find_type_of_non_seekable_stream()
    {
        var sut = new StreamFileTypeProvider(new Mapping());
        using var stream = new NonSeekableStream(GifBytes);

        var result = await sut.TryFindUnambiguousAsync(stream, CancellationToken.None);

        Assert.IsType<Gif>(result);
    }

    [Fact]
    public async Task Validator_should_restore_stream_position_on_cancellation()
    {
        var validator = new Validator();
        using var stream = new MemoryStream(GifBytes) { Position = 3 };

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            await validator.IsValidAsync(stream, new Gif(), new CancellationToken(canceled: true)));

        Assert.Equal(3, stream.Position);
    }

    [Fact]
    public async Task FormFileTypeProvider_should_dispose_stream_it_opened()
    {
        var stream = new TrackingStream(GifBytes);
        var formFile = ProvideFormFile(stream);

        var sut = new FormFileTypeProvider();

        var result = await sut.FindValidatedTypeAsync(formFile, null, CancellationToken.None);

        Assert.IsType<Gif>(result);
        Assert.True(stream.IsDisposed);
    }

    [Fact]
    public async Task FormFileTypeProvider_should_dispose_stream_it_opened_on_mismatch()
    {
        var stream = new TrackingStream([0x00, 0x01, 0x02]);
        var formFile = ProvideFormFile(stream);

        var sut = new FormFileTypeProvider();

        await Assert.ThrowsAsync<MimeTypeMismatchException>(async () =>
            await sut.FindValidatedTypeAsync(formFile, null, CancellationToken.None));

        Assert.True(stream.IsDisposed);
    }

    [Fact]
    public async Task FormFileTypeProvider_should_not_dispose_given_stream()
    {
        var formFile = ProvideFormFile(new TrackingStream(GifBytes));
        var givenStream = new TrackingStream(GifBytes);

        var sut = new FormFileTypeProvider();

        var result = await sut.FindValidatedTypeAsync(formFile, givenStream, CancellationToken.None);

        Assert.IsType<Gif>(result);
        Assert.False(givenStream.IsDisposed);
    }

    private static IFormFile ProvideFormFile(Stream stream)
    {
        var formFile = new Mock<IFormFile>();
        formFile.SetupGet(f => f.FileName).Returns("trp.gif");
        formFile.SetupGet(f => f.ContentType).Returns("image/gif");
        formFile.Setup(f => f.OpenReadStream()).Returns(stream);

        return formFile.Object;
    }

    /// <summary>Stream like a raw request body: readable, but neither seekable nor aware of its length.</summary>
    private sealed class NonSeekableStream(byte[] data) : MemoryStream(data)
    {
        public override bool CanSeek => false;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }
    }

    private sealed class TrackingStream(byte[] data) : MemoryStream(data)
    {
        public bool IsDisposed { get; private set; }

        protected override void Dispose(bool disposing)
        {
            IsDisposed = true;
            base.Dispose(disposing);
        }
    }
}
