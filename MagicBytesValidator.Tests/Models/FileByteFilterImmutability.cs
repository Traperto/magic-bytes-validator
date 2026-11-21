namespace MagicBytesValidator.Tests.Models;

public class FileByteFilterImmutability
{
    [Fact]
    public void Should_reject_all_configuration_after_freeze()
    {
        var fileType = new TestFileType().Freeze();

        Assert.True(fileType.IsFrozen);
        Assert.Throws<InvalidOperationException>(() => fileType.StartsWith([0x01]));
        Assert.Throws<InvalidOperationException>(() => fileType.StartsWithAnyOf([[0x01]]));
        Assert.Throws<InvalidOperationException>(() => fileType.EndsWith([0x01]));
        Assert.Throws<InvalidOperationException>(() => fileType.EndsWithAnyOf([[0x01]]));
        Assert.Throws<InvalidOperationException>(() => fileType.EndsWithIgnoringTrailing([0x01], [0x20]));
        Assert.Throws<InvalidOperationException>(() => fileType.EndsWithZipEndOfCentralDirectory());
        Assert.Throws<InvalidOperationException>(() => fileType.Anywhere([0x01]));
        Assert.Throws<InvalidOperationException>(() => fileType.Anywhere([[0x01]]));
        Assert.Throws<InvalidOperationException>(() => fileType.CompoundFileStreamAnyOf(["WordDocument"]));
        Assert.Throws<InvalidOperationException>(() => fileType.Specific(new FileByteFilter.ByteCheck(0, [0x01])));
        Assert.Throws<InvalidOperationException>(() => fileType.SpecificAnyOf([new FileByteFilter.ByteCheck(0, [0x01])]));
        Assert.Throws<InvalidOperationException>(() => fileType.TailContains(4, [0x01]));
    }

    [Fact]
    public void Should_still_match_after_freeze()
    {
        var fileType = new TestFileType().StartsWith([0x01]).Freeze();

        Assert.True(fileType.Matches([0x01]));
        Assert.False(fileType.Matches([0x02]));
    }

    [Fact]
    public void Should_not_freeze_implicitly()
    {
        var fileType = new TestFileType().StartsWith([0x01]);

        _ = fileType.Matches([0x01]);

        Assert.False(fileType.IsFrozen);
    }

    [Fact]
    public void Should_expose_byte_check_bytes()
    {
        var byteCheck = new FileByteFilter.ByteCheck(-2, [0x01, null]);

        Assert.Equal(-2, byteCheck.Offset);
        Assert.Equal([0x01, null], byteCheck.Bytes);
    }
}
