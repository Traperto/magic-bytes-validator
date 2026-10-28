namespace MagicBytesValidator.Tests.Models;

public class FileByteFilterImmutability
{
    [Fact]
    public void Should_freeze_on_first_match()
    {
        var fileType = new TestFileType().StartsWith([0x01]);
        Assert.False(fileType.IsFrozen);

        _ = fileType.Matches([0x01]);

        Assert.True(fileType.IsFrozen);
        Assert.Throws<InvalidOperationException>(() => fileType.EndsWith([0x02]));
    }

    [Fact]
    public void Should_reject_all_configuration_after_freeze()
    {
        var fileType = new TestFileType().Freeze();

        Assert.Throws<InvalidOperationException>(() => fileType.StartsWith([0x01]));
        Assert.Throws<InvalidOperationException>(() => fileType.StartsWithAnyOf([[0x01]]));
        Assert.Throws<InvalidOperationException>(() => fileType.EndsWith([0x01]));
        Assert.Throws<InvalidOperationException>(() => fileType.EndsWithAnyOf([[0x01]]));
        Assert.Throws<InvalidOperationException>(() => fileType.Anywhere([0x01]));
        Assert.Throws<InvalidOperationException>(() => fileType.Anywhere([[0x01]]));
        Assert.Throws<InvalidOperationException>(() => fileType.CompoundFileStreamAnyOf(["WordDocument"]));
        Assert.Throws<InvalidOperationException>(() => fileType.Specific(new FileByteFilter.ByteCheck(0, [0x01])));
        Assert.Throws<InvalidOperationException>(() => fileType.SpecificAnyOf([new FileByteFilter.ByteCheck(0, [0x01])]));
        Assert.Throws<InvalidOperationException>(() => fileType.TailContains(4, [0x01]));
    }

    [Fact]
    public void Should_copy_byte_patterns()
    {
        byte?[] pattern = [0x01, 0x02];
        var fileType = new TestFileType().StartsWith(pattern);

        pattern[0] = 0xFF;

        Assert.True(fileType.Matches([0x01, 0x02]));
        Assert.False(fileType.Matches([0xFF, 0x02]));
    }

    [Fact]
    public void Should_copy_byte_check_bytes()
    {
        byte?[] pattern = [0x01, 0x02];
        var byteCheck = new FileByteFilter.ByteCheck(0, pattern);

        pattern[0] = 0xFF;

        Assert.Equal([0x01, 0x02], byteCheck.Bytes);
    }

    [Fact]
    public void Should_copy_mime_types_and_extensions()
    {
        string[] mimeTypes = ["traperto/trp"];
        string[] extensions = ["trp"];
        var fileType = new TestFileType(mimeTypes, extensions);

        mimeTypes[0] = "changed/type";
        extensions[0] = "changed";

        Assert.Equal(["traperto/trp"], fileType.MimeTypes);
        Assert.Equal(["trp"], fileType.Extensions);
    }

    [Fact]
    public void Should_not_allow_modifying_mime_types_and_extensions()
    {
        var fileType = new TestFileType(["traperto/trp"], ["trp"]);

        var mimeTypes = Assert.IsAssignableFrom<IList<string>>(fileType.MimeTypes);
        var extensions = Assert.IsAssignableFrom<IList<string>>(fileType.Extensions);

        Assert.Throws<NotSupportedException>(() => mimeTypes[0] = "changed/type");
        Assert.Throws<NotSupportedException>(() => extensions[0] = "changed");
    }
}
