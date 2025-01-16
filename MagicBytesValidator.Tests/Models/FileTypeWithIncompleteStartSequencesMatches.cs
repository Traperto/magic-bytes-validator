namespace MagicBytesValidator.Tests.Models;

public class FileTypeWithIncompleteStartSequencesMatches
{
    [Fact]
    public void Should_match_valid_file()
    {
        var sut = new Aif();

        var testDataOne = new byte[] { 0x46, 0x4F, 0x52, 0x4D, 0x11, 0x12, 0x19, 0x98, 0x41, 0x49, 0x46, 0x46, 0x11 };
        var testDataTwo = new byte[] { 0x46, 0x4F, 0x52, 0x4D, 0x00, 0x01, 0x02, 0x03, 0x41, 0x49, 0x46, 0x46, 0x11 };

        Assert.True(sut.Matches(testDataOne));
        Assert.True(sut.Matches(testDataTwo));
    }
    
    [Fact]
    public void Should_not_match_invalid_file()
    {
        var sut = new Aif();

        var testData = new byte[] { 0x00, 0x4F, 0x52, 0x4D, 0x00, 0x12, 0x19, 0x98, 0x41, 0x49, 0x46, 0x46, 0x11 };

        Assert.False(sut.Matches(testData));
    }
}