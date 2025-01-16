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

        Assert.IsType<Gif>(result);
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

        Assert.IsType<Gif>(result);
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

        Assert.Null(result);
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
        byte[] gifSequence = [0x47, 0x49, 0x46, 0x38, 0x39, 0x61];
        var fileContents = gifSequence.Concat(new byte[] { 0x11, 0x12 }).ToArray();
        var fileStream = new MemoryStream(fileContents.ToArray());

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