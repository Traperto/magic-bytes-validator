using System.Threading.Tasks;
using MagicBytesValidator.Models;
using MagicBytesValidator.Services;
using Xunit;
using Moq;
using MagicBytesValidator.Services.Streams;
using System.Threading;
using System.IO;
using FluentAssertions;
using System;

namespace MagicBytesValidator.Tests.Streams;

public class TryFindUnambiguousAsync
{
    [Fact]
    public async Task Should_find_by_magic_byte_sequence()
    {
        var matchingFileType = new FileTypeWithStartSequences(
            new[] { "matching" },
            new[] { "mtch" },
            new[]
            {
                new byte[] { 0x11, 0x12, 0x19, 0x20 },
                new byte[] { 0x11, 0x12, 0x18 },
                new byte[] { 0x11, 0x12 },
            }
        );

        var mismatchingFileType = new FileTypeWithStartSequences(
            new[] { "mismatching" },
            new[] { "mism" },
            new[]
            {
                new byte[] { 0x11, 0x22 },
                new byte[] { 0x11, 0x22, 0x44, 0x55 }
            }
        );

        var mapping = new Mock<IMapping>();
        mapping
            .SetupGet(m => m.FileTypes)
            .Returns(new[] { matchingFileType, mismatchingFileType });

        var sut = new StreamFileTypeProvider(mapping.Object);

        var stream = new MemoryStream(new byte[] { 0x11, 0x12, 0x18 });

        var result = await sut.TryFindUnambiguousAsync(stream, CancellationToken.None);

        result.Should().Be(matchingFileType);
        result.Should().NotBe(mismatchingFileType);
    }

    [Fact]
    public async Task Should_reset_stream_position()
    {
        var matchingFileType = new FileTypeWithStartSequences(
            new[] { "matching" },
            new[] { "mtch" },
            new[]
            {
                new byte[] { 0x11, 0x12, 0x19, 0x20 },
                new byte[] { 0x11, 0x12, 0x18 },
                new byte[] { 0x11, 0x12 },
            }
        );

        var mapping = new Mock<IMapping>();
        mapping
            .SetupGet(m => m.FileTypes)
            .Returns(new[] { matchingFileType });

        var sut = new StreamFileTypeProvider(mapping.Object);

        var stream = new MemoryStream(new byte[] { 0x11, 0x12, 0x18 })
        {
            Position = 1
        };

        var result = await sut.TryFindUnambiguousAsync(stream, CancellationToken.None);

        result.Should().Be(matchingFileType);
        stream.Position.Should().Be(1);
    }

    [Fact]
    public async Task Should_handle_unknown_file_type()
    {
        var mismatchingFileType = new FileTypeWithStartSequences(
            new[] { "mismatching" },
            new[] { "mism" },
            new[]
            {
                new byte[] { 0x11, 0x22 },
                new byte[] { 0x11, 0x22, 0x44, 0x55 }
            }
        );

        var mapping = new Mock<IMapping>();
        mapping
            .SetupGet(m => m.FileTypes)
            .Returns(new[] { mismatchingFileType });

        var sut = new StreamFileTypeProvider(mapping.Object);

        var stream = new MemoryStream(new byte[] { 0x12, 0x11 });

        var result = await sut.TryFindUnambiguousAsync(stream, CancellationToken.None);

        result.Should().BeNull();
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

        var result = await sut.TryFindUnambiguousAsync(stream, CancellationToken.None);

        result.Should().BeNull();
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
            async () => await sut.TryFindUnambiguousAsync(null!, CancellationToken.None)
        );
    }
}