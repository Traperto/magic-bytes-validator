using System.IO;
using System.Linq;
using FluentAssertions;
using MagicBytesValidator.Exceptions.Http;
using MagicBytesValidator.Formats;
using MagicBytesValidator.Services.Http;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace MagicBytesValidator.Tests.Http;

public class FindFileTypeForFormFile
{
    [Fact]
    public void Should_find_by_extension()
    {
        var formFile = ProvideGifFile("trp.gif", "image/gif");

        var sut = new FormFileTypeProvider();

        var result = sut.FindFileTypeForFormFile(formFile);

        result.Should().BeOfType<Gif>();
    }

    [Fact]
    public void Should_find_by_content_type()
    {
        var formFile = ProvideGifFile(string.Empty, "image/gif");

        var sut = new FormFileTypeProvider();

        var result = sut.FindFileTypeForFormFile(formFile);

        result.Should().BeOfType<Gif>();
    }

    [Fact]
    public void Should_return_null_on_not_found()
    {
        var formFile = ProvideGifFile(string.Empty, "trp/nms");

        var sut = new FormFileTypeProvider();

        var result = sut.FindFileTypeForFormFile(formFile);

        result.Should().BeNull();
    }

    [Fact]
    public void Should_throw_on_mismatch()
    {
        var formFile = ProvideGifFile("trp.gif", "image/png");

        var sut = new FormFileTypeProvider();

        Assert.Throws<MimeTypeMismatchException>(() => sut.FindFileTypeForFormFile(formFile));
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