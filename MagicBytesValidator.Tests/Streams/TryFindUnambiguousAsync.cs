namespace MagicBytesValidator.Tests.Streams;

public class TryFindUnambiguousAsync
{
    [Fact]
    public async Task Should_find_by_magic_byte_sequence()
    {
        var matchingFileType = new TestFileType()
            .StartsWithAnyOf([
                [0x11, 0x12, 0x19, 0x20],
                [0x11, 0x12, 0x18],
                [0x11, 0x12],
            ]);

        var mismatchingFileType = new TestFileType()
            .StartsWithAnyOf([
                [0x11, 0x22],
                [0x11, 0x22, 0x44, 0x55]
            ]);

        var mapping = new Mock<IMapping>();
        mapping
            .SetupGet(m => m.FileTypes)
            .Returns([matchingFileType, mismatchingFileType]);

        var sut = new StreamFileTypeProvider(mapping.Object);

        var stream = new MemoryStream([0x11, 0x12, 0x18]);

        var result = await sut.TryFindUnambiguousAsync(stream, CancellationToken.None);

        Assert.Same(matchingFileType, result);
        Assert.NotSame(mismatchingFileType, result);
    }

    [Fact]
    public async Task Should_reset_stream_position()
    {
        var matchingFileType = new TestFileType()
            .StartsWithAnyOf([
                [0x11, 0x12, 0x19, 0x20],
                [0x11, 0x12, 0x18],
                [0x11, 0x12],
            ]);

        var mapping = new Mock<IMapping>();
        mapping
            .SetupGet(m => m.FileTypes)
            .Returns([matchingFileType]);

        var sut = new StreamFileTypeProvider(mapping.Object);

        var stream = new MemoryStream([0x11, 0x12, 0x18])
        {
            Position = 1
        };

        var result = await sut.TryFindUnambiguousAsync(stream, CancellationToken.None);

        Assert.Same(matchingFileType, result);
        Assert.Equal(1, stream.Position);
    }

    [Fact]
    public async Task Should_handle_unknown_file_type()
    {
        var mismatchingFileType = new TestFileType()
            .StartsWithAnyOf([
                [0x11, 0x22],
                [0x11, 0x22, 0x44, 0x55]
            ]);

        var mapping = new Mock<IMapping>();
        mapping
            .SetupGet(m => m.FileTypes)
            .Returns([mismatchingFileType]);

        var sut = new StreamFileTypeProvider(mapping.Object);

        var stream = new MemoryStream([0x12, 0x11]);

        var result = await sut.TryFindUnambiguousAsync(stream, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Should_handle_empty_mapping()
    {
        var mapping = new Mock<IMapping>();
        mapping
            .SetupGet(m => m.FileTypes)
            .Returns([]);

        var sut = new StreamFileTypeProvider(mapping.Object);

        var stream = new MemoryStream([0x12, 0x11]);

        var result = await sut.TryFindUnambiguousAsync(stream, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Should_throw_on_null_given()
    {
        var mapping = new Mock<IMapping>();
        mapping
            .SetupGet(m => m.FileTypes)
            .Returns([]);

        var sut = new StreamFileTypeProvider(mapping.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await sut.TryFindUnambiguousAsync(null!, CancellationToken.None)
        );
    }

    [Fact]
    public async Task Should_use_closest_types()
    {
        var parentFileType = new ParentFileType();
        var childFileType = new ChildFileType();

        var mapping = new Mock<IMapping>();
        mapping
            .SetupGet(m => m.FileTypes)
            .Returns([
                parentFileType,
                childFileType,
            ]);

        var sut = new StreamFileTypeProvider(mapping.Object);

        /* Matches both parent and child file type -> result should only include child */
        var stream = new MemoryStream([1, 2, 3, 4, 5, 6]);

        var result = await sut.TryFindUnambiguousAsync(stream, CancellationToken.None);

        Assert.Same(childFileType, result);
    }
}