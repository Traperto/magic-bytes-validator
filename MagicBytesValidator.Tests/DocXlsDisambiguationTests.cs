namespace MagicBytesValidator.Tests;

public class DocXlsDisambiguationTests
{
    private static readonly byte[] OleHeader =
    [
        0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1
    ];

    [Fact]
    public async Task XlsWorkbookStreamName_ShouldMatchXls_AndNotDoc()
    {
        var validator = new Validator();
        var xls = new Xls();
        var doc = new Doc();

        using var stream = BuildOleLikeStreamWithUtf16Marker("Workbook");

        var isXls = await validator.IsValidAsync(stream, xls, CancellationToken.None);
        stream.Position = 0;
        var isDoc = await validator.IsValidAsync(stream, doc, CancellationToken.None);

        Assert.True(isXls);
        Assert.False(isDoc);
    }

    [Fact]
    public async Task DocWordDocumentStreamName_ShouldMatchDoc_AndNotXls()
    {
        var validator = new Validator();
        var doc = new Doc();
        var xls = new Xls();

        using var stream = BuildOleLikeStreamWithUtf16Marker("WordDocument");

        var isDoc = await validator.IsValidAsync(stream, doc, CancellationToken.None);
        stream.Position = 0;
        var isXls = await validator.IsValidAsync(stream, xls, CancellationToken.None);

        Assert.True(isDoc);
        Assert.False(isXls);
    }

    [Fact]
    public async Task LegacyXlsBookStreamName_ShouldMatchXls()
    {
        var validator = new Validator();
        var xls = new Xls();

        using var stream = BuildOleLikeStreamWithUtf16Marker("Book");

        var isXls = await validator.IsValidAsync(stream, xls, CancellationToken.None);

        Assert.True(isXls);
    }

    [Fact]
    public async Task ClassicDocOffset512Marker_ShouldMatchDoc()
    {
        var validator = new Validator();
        var doc = new Doc();

        using var stream = BuildOleLikeStreamWithOffset512Marker([0xEC, 0xA5, 0xC1, 0x00]);

        var isDoc = await validator.IsValidAsync(stream, doc, CancellationToken.None);

        Assert.True(isDoc);
    }

    [Fact]
    public async Task ClassicXlsOffset512Marker_ShouldMatchXls()
    {
        var validator = new Validator();
        var xls = new Xls();

        using var stream = BuildOleLikeStreamWithOffset512Marker([0xFD, 0xFF, 0xFF, 0xFF, 0x24, 0x00]);

        var isXls = await validator.IsValidAsync(stream, xls, CancellationToken.None);

        Assert.True(isXls);
    }

    private static MemoryStream BuildOleLikeStreamWithUtf16Marker(string marker)
    {
        var bytes = new byte[4096];
        var markerBytes = System.Text.Encoding.Unicode.GetBytes(marker);

        Array.Copy(OleHeader, 0, bytes, 0, OleHeader.Length);
        Array.Copy(markerBytes, 0, bytes, 1536, markerBytes.Length);

        return new MemoryStream(bytes);
    }

    private static MemoryStream BuildOleLikeStreamWithOffset512Marker(byte[] marker)
    {
        var bytes = new byte[2048];

        Array.Copy(OleHeader, 0, bytes, 0, OleHeader.Length);
        Array.Copy(marker, 0, bytes, 512, marker.Length);

        return new MemoryStream(bytes);
    }
}