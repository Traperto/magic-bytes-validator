namespace MagicBytesValidator.Tests;

public class DocXlsDisambiguationTests
{
    private static readonly byte[] OleHeader =
    [
        0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1
    ];

    [Fact]
    public async Task XlsSignatureAtOffset512_ShouldMatchXls_AndNotDoc()
    {
        // Arrange
        var validator = new Validator();
        var xls = new Xls();
        var doc = new Doc();

        using var stream = BuildOleLikeStreamWithSubHeaderAt512(
            [0xFD, 0xFF, 0xFF, 0xFF, 0x24, 0x00] // matches Xls ByteCheck with wildcard in slot 5
        );

        // Act
        var isXls = await validator.IsValidAsync(stream, xls, CancellationToken.None);
        stream.Position = 0;
        var isDoc = await validator.IsValidAsync(stream, doc, CancellationToken.None);

        // Assert
        Assert.True(isXls);
        Assert.False(isDoc);
    }

    [Fact]
    public async Task DocSignatureAtOffset512_ShouldMatchDoc()
    {
        // Arrange
        var validator = new Validator();
        var doc = new Doc();

        using var stream = BuildOleLikeStreamWithSubHeaderAt512(
            [0xEC, 0xA5, 0xC1, 0x00] // classic DOC subheader at offset 512
        );

        // Act
        var isDoc = await validator.IsValidAsync(stream, doc, CancellationToken.None);

        // Assert
        Assert.True(isDoc);
    }

    private static MemoryStream BuildOleLikeStreamWithSubHeaderAt512(byte[] subHeader)
    {
        var bytes = new byte[1024];

        Array.Copy(OleHeader, 0, bytes, 0, OleHeader.Length);
        Array.Copy(subHeader, 0, bytes, 512, subHeader.Length);

        return new MemoryStream(bytes);
    }
}