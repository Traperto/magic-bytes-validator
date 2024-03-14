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
using System.Linq;

namespace MagicBytesValidator.Tests.Streams;

public class FindAllMatchesAsync
{
    [Fact]
    public async Task Should_find_all_by_magic_byte_sequence()
    {
        var matchingFileType1 = new FileTypeWithStartSequences(
            new[] { "matching" },
            new[] { "mtch" },
            new[]
            {
                new byte[] { 0x11, 0x12, 0x19, 0x20 },
                new byte[] { 0x11, 0x12, 0x18 },
                new byte[] { 0x11, 0x12 },
            }
        );

        var matchingFileType2 = new FileTypeWithStartSequences(
            new[] { "also/matching" },
            new[] { "mtch2" },
            new[]
            {
                new byte[] { 0x11, 0x12 },
                new byte[] { 0x11, 0x22, 0x44, 0x55 }
            }
        );

        var mapping = new Mock<IMapping>();
        mapping
           .SetupGet(m => m.FileTypes)
           .Returns(new[] { matchingFileType1, matchingFileType2 });

        var sut = new StreamFileTypeProvider(mapping.Object);

        var stream = new MemoryStream(new byte[] { 0x11, 0x12, 0x18 });

        var result = (await sut.FindAllMatchesAsync(stream, CancellationToken.None))
           .ToList();

        result.Should().HaveCount(2);
        result.Should().Contain(matchingFileType1);
        result.Should().Contain(matchingFileType2);
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

        _ = await sut.FindAllMatchesAsync(stream, CancellationToken.None);

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

        var result = await sut.FindAllMatchesAsync(stream, CancellationToken.None);

        result.Should().BeEmpty();
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

        result.Should().BeEmpty();
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