namespace MagicBytesValidator.Tests.Models;

public class FileByteFilterMatches
{
    [Fact]
    public void Should_match_pdf()
    {
        var pdf = new Pdf();

        var pdfTestData = "%PDF-\n%%EOF\n"u8.ToArray();

        pdf.Matches(pdfTestData).Should().BeTrue();
    }
    
    [Fact]
    public void Should_not_match_pdf()
    {
        var pdf = new Pdf();

        var pdfTestData = "%PDDF-\n%%EEOF\n"u8.ToArray();

        pdf.Matches(pdfTestData).Should().BeFalse();
    }
    
    [Fact]
    public void Should_match_ppt()
    {
        var pdf = new Ppt();

        // We need to check for an offset of 512
        var pdfTestData = new byte[520];
        var startingData = new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 };
        var offsetData = new byte[] { 0xFD, 0xFF, 0xFF, 0xFF, 0x53, 0x00, 0x00, 0x00 };

        // Start of ppt file
        for (var startIndex = 0; startIndex < startingData.Length; startIndex++)
        {
            pdfTestData[startIndex] = startingData[startIndex];
        }
        
        // Content of ppt file at offset 512
        for (var endIndex = 0; endIndex < offsetData.Length; endIndex++)
        {
            pdfTestData[endIndex + 512] = offsetData[endIndex];
        }

        pdf.Matches(pdfTestData).Should().BeTrue();
    }
    
    [Fact]
    public void Should_not_match_offset_ppt()
    {
        // Valid Start but Invalid offset Data
        var pdf = new Ppt();

        // We need to check for an offset of 512
        var pdfTestData = new byte[520];
        var startingData = new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 };
        var offsetData = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        // Start of ppt file
        for (var startIndex = 0; startIndex < startingData.Length; startIndex++)
        {
            pdfTestData[startIndex] = startingData[startIndex];
        }
        
        // Content of ppt file at offset 512
        for (var endIndex = 0; endIndex < offsetData.Length; endIndex++)
        {
            pdfTestData[endIndex + 512] = offsetData[endIndex];
        }

        pdf.Matches(pdfTestData).Should().BeFalse("Starting data correct but data at offset 512 invalid");
    }
    
    [Fact]
    public void Should_not_match_start_ppt()
    {
        var pdf = new Ppt();

        // We need to check for an offset of 512
        var pdfTestData = new byte[520];
        var startingData = new byte[] { 0xFD, 0xFD, 0xFD, 0xFD, 0xFD, 0xFD, 0xFD, 0xFD };
        var offsetData = new byte[] { 0xFD, 0xFF, 0xFF, 0xFF, 0x53, 0x00, 0x00, 0x00 };

        // Start of ppt file
        for (var startIndex = 0; startIndex < startingData.Length; startIndex++)
        {
            pdfTestData[startIndex] = startingData[startIndex];
        }
        
        // Content of ppt file at offset 512
        for (var endIndex = 0; endIndex < offsetData.Length; endIndex++)
        {
            pdfTestData[endIndex + 512] = offsetData[endIndex];
        }

        pdf.Matches(pdfTestData).Should().BeFalse("Offset data valid but incorrect starting data");
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

        xlsx.Matches(xlsxTestData).Should().BeTrue();
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

        xlsx.Matches(xlsxTestData).Should().BeFalse("specific byte array has invalid bytes");
    }
}