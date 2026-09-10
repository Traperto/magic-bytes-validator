namespace MagicBytesValidator.Tests.Http;

public class FindValidatedTypeAsync
{
    private static readonly byte[] OleHeader =
    [
        0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1
    ];

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

    [Fact]
    public async Task Should_validate_legacy_doc()
    {
        var formFile = ProvideFile("legacy.doc", "application/msword", BuildLegacyDocBytes());

        var sut = new FormFileTypeProvider();

        var result = await sut.FindValidatedTypeAsync(
            formFile,
            null,
            CancellationToken.None
        );

        Assert.IsType<Doc>(result);
    }

    [Fact]
    public async Task Should_validate_legacy_xls()
    {
        var formFile = ProvideFile("legacy.xls", "application/msexcel", BuildLegacyXlsBytes());

        var sut = new FormFileTypeProvider();

        var result = await sut.FindValidatedTypeAsync(
            formFile,
            null,
            CancellationToken.None
        );

        Assert.IsType<Xls>(result);
    }

    [Fact]
    public async Task Should_validate_legacy_ppt()
    {
        var formFile = ProvideFile("legacy.ppt", "application/vnd.ms-powerpoint", BuildLegacyPptBytes());

        var sut = new FormFileTypeProvider();

        var result = await sut.FindValidatedTypeAsync(
            formFile,
            null,
            CancellationToken.None
        );

        Assert.IsType<Ppt>(result);
    }

    [Fact]
    public async Task Should_validate_modern_docx()
    {
        var formFile = ProvideFile(
            "modern.docx",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            BuildOpenXmlBytes("word/_rels/document.xml.rels")
        );

        var sut = new FormFileTypeProvider();

        var result = await sut.FindValidatedTypeAsync(
            formFile,
            null,
            CancellationToken.None
        );

        Assert.IsType<Docx>(result);
    }

    [Fact]
    public async Task Should_validate_modern_xlsx()
    {
        var formFile = ProvideFile(
            "modern.xlsx",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            BuildOpenXmlBytes("xl/_rels/workbook.xml.rels")
        );

        var sut = new FormFileTypeProvider();

        var result = await sut.FindValidatedTypeAsync(
            formFile,
            null,
            CancellationToken.None
        );

        Assert.IsType<Xlsx>(result);
    }

    [Fact]
    public async Task Should_validate_modern_pptx()
    {
        var formFile = ProvideFile(
            "modern.pptx",
            "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            BuildOpenXmlBytes("ppt/_rels/presentation.xml.rels")
        );

        var sut = new FormFileTypeProvider();

        var result = await sut.FindValidatedTypeAsync(
            formFile,
            null,
            CancellationToken.None
        );

        Assert.IsType<Pptx>(result);
    }

    private static IFormFile ProvideFile(string name, string contentType, byte[] fileContents)
    {
        var fileStream = new MemoryStream(fileContents);

        return new FormFile(
            new MemoryStream(fileContents),
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

    private static IFormFile ProvideGifFile(string name, string contentType)
    {
        byte[] gifSequence = [0x47, 0x49, 0x46, 0x38, 0x39, 0x61];
        var fileContents = gifSequence.Concat(new byte[] { 0x11, 0x12 }).ToArray();

        return ProvideFile(name, contentType, fileContents);
    }

    private static byte[] BuildLegacyDocBytes()
    {
        var bytes = new byte[4096];
        var markerBytes = System.Text.Encoding.Unicode.GetBytes("WordDocument");

        Array.Copy(OleHeader, 0, bytes, 0, OleHeader.Length);
        Array.Copy(markerBytes, 0, bytes, 1536, markerBytes.Length);

        return bytes;
    }

    private static byte[] BuildLegacyXlsBytes()
    {
        var bytes = new byte[4096];
        var markerBytes = System.Text.Encoding.Unicode.GetBytes("Workbook");

        Array.Copy(OleHeader, 0, bytes, 0, OleHeader.Length);
        Array.Copy(markerBytes, 0, bytes, 1536, markerBytes.Length);

        return bytes;
    }

    private static byte[] BuildLegacyPptBytes()
    {
        var bytes = new byte[2048];
        var marker = new byte[] { 0xA0, 0x46, 0x1D, 0xF0 };

        Array.Copy(OleHeader, 0, bytes, 0, OleHeader.Length);
        Array.Copy(marker, 0, bytes, 512, marker.Length);

        return bytes;
    }

    private static byte[] BuildOpenXmlBytes(string marker)
    {
        var bytes = BuildZipLikeBytesWithEocd(0x12345678);
        var markerBytes = System.Text.Encoding.ASCII.GetBytes(marker);

        Array.Copy(markerBytes, 0, bytes, 64, markerBytes.Length);

        return bytes;
    }

    private static byte[] BuildZipLikeBytesWithEocd(uint centralDirectoryOffset)
    {
        var bytes = new byte[512];

        bytes[0] = 0x50;
        bytes[1] = 0x4B;
        bytes[2] = 0x03;
        bytes[3] = 0x04;

        var eocdStart = bytes.Length - 22;
        bytes[eocdStart + 0] = 0x50;
        bytes[eocdStart + 1] = 0x4B;
        bytes[eocdStart + 2] = 0x05;
        bytes[eocdStart + 3] = 0x06;

        bytes[eocdStart + 16] = (byte)(centralDirectoryOffset & 0xFF);
        bytes[eocdStart + 17] = (byte)((centralDirectoryOffset >> 8) & 0xFF);
        bytes[eocdStart + 18] = (byte)((centralDirectoryOffset >> 16) & 0xFF);
        bytes[eocdStart + 19] = (byte)((centralDirectoryOffset >> 24) & 0xFF);

        bytes[eocdStart + 20] = 0x00;
        bytes[eocdStart + 21] = 0x00;

        return bytes;
    }
}