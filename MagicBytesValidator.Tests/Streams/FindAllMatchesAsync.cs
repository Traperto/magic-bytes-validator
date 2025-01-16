namespace MagicBytesValidator.Tests.Streams;

public class FindAllMatchesAsync
{
    [Fact]
    public async Task Should_find_all_by_magic_byte_sequence()
    {
        var matchingFileType1 = new FileByteFilter(
            ["matching"],
            ["mtch"]
        ).StartsWithAnyOf([
            [0x11, 0x12, 0x19, 0x20],
            [0x11, 0x12, 0x18],
            [0x11, 0x12],
        ]);

        var matchingFileType2 = new FileByteFilter(
            ["also/matching"],
            ["mtch2"]
        ).StartsWithAnyOf([
            [0x11, 0x12],
            [0x11, 0x22, 0x44, 0x55]
        ]);

        var mapping = new Mock<IMapping>();
        mapping
           .SetupGet(m => m.FileTypes)
           .Returns(new[] { matchingFileType1, matchingFileType2 });

        var sut = new StreamFileTypeProvider(mapping.Object);

        var stream = new MemoryStream(new byte[] { 0x11, 0x12, 0x18 });

        var result = (await sut.FindAllMatchesAsync(stream, CancellationToken.None))
           .ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(matchingFileType1, result);
        Assert.Contains(matchingFileType2, result);
    }

    [Fact]
    public async Task Should_reset_stream_position()
    {
        var matchingFileType = new FileByteFilter(
            ["matching"],
            ["mtch"]
        ).StartsWithAnyOf([
            [0x11, 0x12, 0x19, 0x20],
            [0x11, 0x12, 0x18],
            [0x11, 0x12],
        ]);

        var mapping = new Mock<IMapping>();
        mapping
           .SetupGet(m => m.FileTypes)
           .Returns(new[] { matchingFileType });

        var sut = new StreamFileTypeProvider(mapping.Object);

        var stream = new MemoryStream(new byte[] { 0x11, 0x12, 0x18 })
        {
            Position = 1
        };

        _ = await sut.FindAllMatchesAsync(stream, CancellationToken.None);

        Assert.Equal(1, stream.Position);
    }

    [Fact]
    public async Task Should_handle_unknown_file_type()
    {
        var mismatchingFileType = new FileByteFilter(
            ["mismatching"],
            ["mism"]
        ).StartsWithAnyOf([
            [0x11, 0x22],
            [0x11, 0x22, 0x44, 0x55]
        ]);

        var mapping = new Mock<IMapping>();
        mapping
           .SetupGet(m => m.FileTypes)
           .Returns(new[] { mismatchingFileType });

        var sut = new StreamFileTypeProvider(mapping.Object);

        var stream = new MemoryStream(new byte[] { 0x12, 0x11 });

        var result = await sut.FindAllMatchesAsync(stream, CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task Should_handle_empty_mapping()
    {
        var mapping = new Mock<IMapping>();
        mapping
           .SetupGet(m => m.FileTypes)
           .Returns(Array.Empty<IFileType>());

        var sut = new StreamFileTypeProvider(mapping.Object);

        var stream = new MemoryStream(new byte[] { 0x12, 0x11 });

        var result = await sut.FindAllMatchesAsync(stream, CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task Should_throw_on_null_given()
    {
        var mapping = new Mock<IMapping>();
        mapping
           .SetupGet(m => m.FileTypes)
           .Returns(Array.Empty<IFileType>());

        var sut = new StreamFileTypeProvider(mapping.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await sut.FindAllMatchesAsync(null!, CancellationToken.None)
        );
    }
}