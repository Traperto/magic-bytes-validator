using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MagicBytesValidator.Exceptions.Http;
using MagicBytesValidator.Formats;
using MagicBytesValidator.Services.Http;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace MagicBytesValidator.Tests.Http;

public class FindValidatedTypeAsync
{
    [Fact]
    public async Task Should_find_by_extension()
    {
        var formFile = ProvideGifFile("trp.gif", "image/gif");

        var sut = new FormFileTypeProvider();

        var result = await sut.FindValidatedTypeAsync(
            formFile,
            null,
            CancellationToken.None
        );

        result.Should().BeOfType<Gif>();
    }

    [Fact]
    public async Task Should_find_by_content_type()
    {
        var formFile = ProvideGifFile(string.Empty, "image/gif");

        var sut = new FormFileTypeProvider();

        var result = await sut.FindValidatedTypeAsync(
            formFile,
            null,
            CancellationToken.None
        );

        result.Should().BeOfType<Gif>();
    }

    [Fact]
    public async Task Should_return_null_on_not_found()
    {
        var formFile = ProvideGifFile(string.Empty, "trp/crly");

        var sut = new FormFileTypeProvider();

        var result = await sut.FindValidatedTypeAsync(
            formFile,
            null,
            CancellationToken.None
        );

        result.Should().BeNull();
    }

    [Fact]
    public async Task Should_throw_on_type_vs_name_mismatch()
    {
        var formFile = ProvideGifFile("trp.gif", "image/png");

        var sut = new FormFileTypeProvider();

        await Assert.ThrowsAsync<MimeTypeMismatchException>(async () =>
            await sut.FindValidatedTypeAsync(
                formFile,
                null,
                CancellationToken.None
            )
        );
    }

    [Fact]
    public async Task Should_throw_on_type_vs_content_mismatch()
    {
        var formFile = ProvideGifFile("trp.png", "image/png");

        var sut = new FormFileTypeProvider();

        await Assert.ThrowsAsync<MimeTypeMismatchException>(async () =>
            await sut.FindValidatedTypeAsync(
                formFile,
                null,
                CancellationToken.None
            )
        );
    }

    private static IFormFile ProvideGifFile(string name, string contentType)
    {
        var fileTypeGif = new Gif();
        var fileContents = fileTypeGif.MagicByteSequences.First().Concat(new byte[] { 0x11, 0x12 }).ToArray();
        var fileStream = new MemoryStream(fileContents);

        return new FormFile(
            new MemoryStream(fileContents.ToArray()),
            0,
            fileStream.Length,
            name,
            name
        )
        {
            Headers = new HeaderDictionary
            {
                { "Content-Type", contentType }
            }
        };
    }
}