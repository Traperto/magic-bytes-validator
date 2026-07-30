namespace MagicBytesValidator.Tests;

public class ZipLargeFileEocdTests
{
    [Fact]
    public async Task Zip_WithLargeCentralDirectoryOffset_MustBeValid()
    {
        // Arrange
        var validator = new Validator();
        var zip = new Zip();

        using var stream = BuildZipLikeBytesWithEocd(
            centralDirectoryOffset: 0x0126BCBC // > 16 MB => MSB = 0x01
        );

        // Act
        var isValid = await validator.IsValidAsync(stream, zip, CancellationToken.None);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public async Task Xlsx_WithLargeCentralDirectoryOffset_MustBeValid()
    {
        // Arrange
        var validator = new Validator();
        var xlsx = new Xlsx();

        using var stream = BuildMinimalXlsxLikeBytesWithEocd(
            centralDirectoryOffset: 0x0126BCBC // > 16 MB
        );

        // Act
        var isValid = await validator.IsValidAsync(stream, xlsx, CancellationToken.None);

        // Assert
        Assert.True(isValid);
    }

    private static MemoryStream BuildZipLikeBytesWithEocd(uint centralDirectoryOffset)
    {
        // Small buffer is enough because this validator checks signatures/patterns only.
        var bytes = new byte[256];

        // Local file header signature at start: PK\x03\x04
        bytes[0] = 0x50;
        bytes[1] = 0x4B;
        bytes[2] = 0x03;
        bytes[3] = 0x04;

        // EOCD (22 bytes) at end
        var eocdStart = bytes.Length - 22;
        bytes[eocdStart + 0] = 0x50; // P
        bytes[eocdStart + 1] = 0x4B; // K
        bytes[eocdStart + 2] = 0x05;
        bytes[eocdStart + 3] = 0x06;

        // disk numbers + entry counts + size of central dir can stay 0 for this signature test

        // offset of start of central directory (little endian)
        bytes[eocdStart + 16] = (byte)(centralDirectoryOffset & 0xFF);
        bytes[eocdStart + 17] = (byte)((centralDirectoryOffset >> 8) & 0xFF);
        bytes[eocdStart + 18] = (byte)((centralDirectoryOffset >> 16) & 0xFF);
        bytes[eocdStart + 19] = (byte)((centralDirectoryOffset >> 24) & 0xFF); // becomes 0x01 here

        // comment length = 0
        bytes[eocdStart + 20] = 0x00;
        bytes[eocdStart + 21] = 0x00;

        return new MemoryStream(bytes);
    }

    private static MemoryStream BuildMinimalXlsxLikeBytesWithEocd(uint centralDirectoryOffset)
    {
        var bytes = BuildZipLikeBytesWithEocd(centralDirectoryOffset).ToArray();

        // Add marker searched by Xlsx.Anywhere(...)
        var marker = "xl/_rels/workbook.xml.rels"u8.ToArray();
        Array.Copy(marker, 0, bytes, 32, marker.Length);

        return new MemoryStream(bytes);
    }
}