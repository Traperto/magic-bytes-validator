namespace MagicBytesValidator.Tests.Models;

public class FileByteFilterMatches
{
    [Fact]
    public void Should_match_pdf()
    {
        var pdf = new Pdf();

        var pdfTestData = "%PDF-\n%%EOF\n"u8.ToArray();

        Assert.True(pdf.Matches(pdfTestData));
    }

    [Fact]
    public void Should_not_match_pdf()
    {
        var pdf = new Pdf();

        var pdfTestData = "%PDDF-\n%%EEOF\n"u8.ToArray();

        Assert.False(pdf.Matches(pdfTestData));
    }

    [Fact]
    public void Should_match_pdf_with_trailing_bytes_in_default_mode()
    {
       var pdf = new Pdf();

       var pdfTestData = "%PDF-\n%%EOF\nTRAILING"u8.ToArray();

       Assert.True(pdf.Matches(pdfTestData, FileByteType.Lazy));
    }

    [Fact]
    public void Should_not_match_pdf_with_trailing_bytes_in_strict_mode()
    {
       var pdf = new Pdf();

       var pdfTestData = "%PDF-\n%%EOF\nTRAILING"u8.ToArray();

       Assert.False(pdf.Matches(pdfTestData, FileByteType.Strict));
    }

    [Fact]
    public void Should_not_match_pdf_when_eof_is_not_within_last_1024_bytes_in_default_mode()
    {
       var pdf = new Pdf();

       var prefix = "%PDF-\n"u8.ToArray();
       var eof = "%%EOF\n"u8.ToArray();

       // put EOF early, then add >1024 bytes afterwards so it falls outside the last 1024 bytes
       var trailing = new byte[1100];
       for (var i = 0; i < trailing.Length; i++)
       {
          trailing[i] = (byte)'A';
       }

       var pdfTestData = prefix
          .Concat(eof)
          .Concat(trailing)
          .ToArray();

       Assert.False(pdf.Matches(pdfTestData, FileByteType.Lazy));
    }

    [Fact]
    public void Should_match_pdf_when_eof_marker_is_present_in_default_mode()
    {
       var pdf = new Pdf();

       var pdfTestData = "%PDF-\n...%%EOF...TAIL"u8.ToArray();

       Assert.True(pdf.Matches(pdfTestData, FileByteType.Lazy));
    }

    [Fact]
    public void Should_match_pdf_in_strict_mode_when_eof_is_at_end()
    {
       var pdf = new Pdf();

       var pdfTestData = "%PDF-\n%%EOF"u8.ToArray();

       Assert.True(pdf.Matches(pdfTestData, FileByteType.Strict));
    }

    [Fact]
    public void Should_match_ppt()
    {
        var ppt = new Ppt();

        var pptTestData = CompoundFileBuilder.Build(["PowerPoint Document", "Current User"]);

        Assert.True(ppt.Matches(pptTestData));
    }

    [Fact]
    public void Should_not_match_ppt_without_powerpoint_stream()
    {
        // Valid compound file, but without the "PowerPoint Document" stream
        var ppt = new Ppt();

        var pptTestData = CompoundFileBuilder.Build(["WordDocument"]);

        Assert.False(ppt.Matches(pptTestData), "Compound file without PowerPoint stream is no ppt");
    }

    [Fact]
    public void Should_not_match_start_ppt()
    {
        var ppt = new Ppt();

        var pptTestData = CompoundFileBuilder.Build(["PowerPoint Document"]);
        Array.Fill<byte>(pptTestData, 0xFD, 0, 8);

        Assert.False(ppt.Matches(pptTestData), "Directory valid but incorrect starting data");
    }

    [Fact]
    public void Should_match_xlsx()
    {
        var xlsx = new Xlsx();

        // Some random data at start and end, xlsx looks for specific bytes anywhere in the file
        // random parts are marked with 0xFF
        var xlsxTestData = new byte[]
        {
            0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00, 0xFF, 0xFF, 0xFF, 0x78, 0x6c,
            0x2f, 0x5f, 0x72, 0x65, 0x6c, 0x73, 0x2f, 0x77, 0x6f, 0x72, 0x6b, 0x62, 0x6f,
            0x6f, 0x6b, 0x2e, 0x78, 0x6d, 0x6c, 0x2e, 0x72, 0x65, 0x6c, 0x73, 0xFF, 0xFF,
            0x50, 0x4B, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00
        };

        Assert.True(xlsx.Matches(xlsxTestData));
    }

    [Fact]
    public void Should_not_match_xlsx()
    {
        var xlsx = new Xlsx();

        var xlsxTestData = new byte[]
        {
            0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00, 0xFF, 0xFF, 0xFF, 0x78, 0x6c,
            0x2f, 0x5f, 0x72, 0x65, 0x6c, 0x73, 0x2f, 0x77, 0x6f, 0x72, 0x6b, 0x62, 0x6f,
            0x6f, 0x6b, 0x2e, 0x78, 0xFF, 0xFF, 0xFF, 0x72, 0x65, 0x6c, 0x73, 0xFF, 0xFF
        };

        Assert.False(xlsx.Matches(xlsxTestData), "specific byte array has invalid bytes");
    }

    [Fact]
    public void Should_match_heic()
    {
        var heic = new Heic();

        var testStream = new byte[]
        {
            0x00,0x00,0x00,0x18, 0x66, 0x74, 0x79, 0x70, 0x68, 0x65, 0x69, 0x63
        };

        Assert.True(heic.Matches(testStream));
    }
}
