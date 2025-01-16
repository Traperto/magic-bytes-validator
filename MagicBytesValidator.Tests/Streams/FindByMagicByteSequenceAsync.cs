#pragma warning disable CS0618 // Type or member is obsolete

namespace MagicBytesValidator.Tests.Streams;

public class FindByMagicByteSequenceAsync
{
    [Fact]
    public async Task Should_find_by_magic_byte_sequence()
    {
        var matchingFileType = new FileByteFilter(
            ["matching"],
            ["mtch"]
        ).StartsWithAnyOf([
            [0x11, 0x12, 0x19, 0x20],
            [0x11, 0x12, 0x18],
            [0x11, 0x12],
        ]);

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
           .Returns(new[] { matchingFileType, mismatchingFileType });

        var sut = new StreamFileTypeProvider(mapping.Object);

        var stream = new MemoryStream(new byte[] { 0x11, 0x12, 0x18 });

        var result = await sut.FindByMagicByteSequenceAsync(stream, CancellationToken.None);

        Assert.Same(matchingFileType, result);
        Assert.NotSame(mismatchingFileType, result);
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

        var result = await sut.FindByMagicByteSequenceAsync(stream, CancellationToken.None);

        Assert.Same(matchingFileType, result);
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

        var result = await sut.FindByMagicByteSequenceAsync(stream, CancellationToken.None);

        Assert.Null(result);
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

        var result = await sut.FindByMagicByteSequenceAsync(stream, CancellationToken.None);

        Assert.Null(result);
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
            async () => await sut.FindByMagicByteSequenceAsync(null!, CancellationToken.None)
        );
    }

    [Fact]
    public async Task Should_find_by_magic_byte_sequence_with_offset()
    {
        var matchingFileType = new FileByteFilter(
            ["matching"],
            ["mtch"]
        ).Anywhere([0x11, 0x12, 0x18]);

        var mismatchingFileType = new FileByteFilter(
            ["mismatching"],
            ["mism"]
        ).StartsWithAnyOf([
            [0x11, 0x22, 0xFF],
            [0x11, 0x22, 0x44, 0x55]
        ]);

        var mapping = new Mock<IMapping>();
        mapping
           .SetupGet(m => m.FileTypes)
           .Returns(new[] { matchingFileType, mismatchingFileType });

        var sut = new StreamFileTypeProvider(mapping.Object);

        var stream = new MemoryStream(new byte[] { 0x00, 0x00, 0x11, 0x12, 0x18 });

        var result = await sut.FindByMagicByteSequenceAsync(stream, CancellationToken.None);

        Assert.Same(matchingFileType, result);
        Assert.NotSame(mismatchingFileType, result);
    }

    [Fact]
    public async Task Should_handle_unknown_file_type_by_offset_in_type()
    {
        var mismatchingFileType = new FileByteFilter(
            ["mismatching"],
            ["mism"]
        ).StartsWithAnyOf([
            [0x11, 0x22, 0x44, 0x55]
        ]);

        var mapping = new Mock<IMapping>();
        mapping
           .SetupGet(m => m.FileTypes)
           .Returns(new[] { mismatchingFileType });

        var sut = new StreamFileTypeProvider(mapping.Object);

        var stream = new MemoryStream(new byte[] { 0x11, 0x22 });

        var result = await sut.FindByMagicByteSequenceAsync(stream, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Should_handle_unknown_file_type_by_offset_in_stream()
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

        var stream = new MemoryStream(new byte[] { 0x00, 0x00, 0x11, 0x22 });

        var result = await sut.FindByMagicByteSequenceAsync(stream, CancellationToken.None);

        Assert.Null(result);
    }
}
#pragma warning restore CS0618 // Type or member is obsolete