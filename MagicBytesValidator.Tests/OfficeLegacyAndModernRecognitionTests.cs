namespace MagicBytesValidator.Tests;

public class OfficeLegacyAndModernRecognitionTests
{
    [Fact]
    public async Task LegacyDocOle_ShouldMatchDoc_AndNotDocx()
    {
        var validator = new Validator();
        var doc = new Doc();
        var docx = new Docx();

        using var stream = CompoundFileBuilder.BuildStream(["WordDocument"]);

        var isDoc = await validator.IsValidAsync(stream, doc, CancellationToken.None);
        stream.Position = 0;
        var isDocx = await validator.IsValidAsync(stream, docx, CancellationToken.None);

        Assert.True(isDoc);
        Assert.False(isDocx);
    }

    [Fact]
    public async Task LegacyXlsOle_ShouldMatchXls_AndNotXlsx()
    {
        var validator = new Validator();
        var xls = new Xls();
        var xlsx = new Xlsx();

        using var stream = CompoundFileBuilder.BuildStream(["Workbook"]);

        var isXls = await validator.IsValidAsync(stream, xls, CancellationToken.None);
        stream.Position = 0;
        var isXlsx = await validator.IsValidAsync(stream, xlsx, CancellationToken.None);

        Assert.True(isXls);
        Assert.False(isXlsx);
    }

    [Fact]
    public async Task LegacyPptOle_ShouldMatchPpt_AndNotPptx()
    {
        var validator = new Validator();
        var ppt = new Ppt();
        var pptx = new Pptx();

        using var stream = CompoundFileBuilder.BuildStream(["PowerPoint Document"]);

        var isPpt = await validator.IsValidAsync(stream, ppt, CancellationToken.None);
        stream.Position = 0;
        var isPptx = await validator.IsValidAsync(stream, pptx, CancellationToken.None);

        Assert.True(isPpt);
        Assert.False(isPptx);
    }

    [Fact]
    public async Task ModernDocxZip_ShouldMatchDocx_AndNotDoc()
    {
        var validator = new Validator();
        var docx = new Docx();
        var doc = new Doc();

        using var stream = BuildOfficeOpenXmlLikeZipStream("word/_rels/document.xml.rels");

        var isDocx = await validator.IsValidAsync(stream, docx, CancellationToken.None);
        stream.Position = 0;
        var isDoc = await validator.IsValidAsync(stream, doc, CancellationToken.None);

        Assert.True(isDocx);
        Assert.False(isDoc);
    }

    [Fact]
    public async Task ModernXlsxZip_ShouldMatchXlsx_AndNotXls()
    {
        var validator = new Validator();
        var xlsx = new Xlsx();
        var xls = new Xls();

        using var stream = BuildOfficeOpenXmlLikeZipStream("xl/_rels/workbook.xml.rels");

        var isXlsx = await validator.IsValidAsync(stream, xlsx, CancellationToken.None);
        stream.Position = 0;
        var isXls = await validator.IsValidAsync(stream, xls, CancellationToken.None);

        Assert.True(isXlsx);
        Assert.False(isXls);
    }

    [Fact]
    public async Task ModernPptxZip_ShouldMatchPptx_AndNotPpt()
    {
        var validator = new Validator();
        var pptx = new Pptx();
        var ppt = new Ppt();

        using var stream = BuildOfficeOpenXmlLikeZipStream("ppt/_rels/presentation.xml.rels");

        var isPptx = await validator.IsValidAsync(stream, pptx, CancellationToken.None);
        stream.Position = 0;
        var isPpt = await validator.IsValidAsync(stream, ppt, CancellationToken.None);

        Assert.True(isPptx);
        Assert.False(isPpt);
    }

    private static MemoryStream BuildOfficeOpenXmlLikeZipStream(string marker)
    {
        var bytes = BuildZipLikeBytesWithEocd(0x12345678).ToArray();
        var markerBytes = System.Text.Encoding.ASCII.GetBytes(marker);

        Array.Copy(markerBytes, 0, bytes, 64, markerBytes.Length);

        return new MemoryStream(bytes);
    }

    private static MemoryStream BuildZipLikeBytesWithEocd(uint centralDirectoryOffset)
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

        return new MemoryStream(bytes);
    }
}

