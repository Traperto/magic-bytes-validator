namespace MagicBytesValidator.Tests;

public class MappingRegister
{
    private readonly Mapping _mapping = new();

    private readonly IFileType _testFileType = new TestFileType(
        ["application/x-test"],
        ["test"]
    ).StartsWith([0x74, 0x65, 0x73, 0x74]);

    [Fact]
    public void Should_register_single_filetype()
    {
        _mapping.Register(_testFileType);
        Assert.Contains(_testFileType, _mapping.FileTypes);
    }

    [Fact]
    public void Should_register_list_filetype()
    {
        var otherFileType = new TestFileType(
            ["application/x-other"],
            ["other"]
        ).StartsWith([0x6f, 0x74, 0x68, 0x65, 0x72]);

        _mapping.Register([otherFileType]);
        Assert.Contains(otherFileType, _mapping.FileTypes);
    }

    [Fact]
    public void Should_register_assembly_fileTypes()
    {
        var assembly = typeof(AssemblyFacade).Assembly;
        _mapping.Register(assembly);

        Assert.Contains(_mapping.FileTypes, f => f.MimeTypes.Contains("application/x-facade"));
    }
}

public class AssemblyFacade : FileByteFilter
{
    public AssemblyFacade() : base(
        ["application/x-facade"],
        ["facade"]
    )
    {
        StartsWith([0x66, 0x61, 0x63, 0x61, 0x64, 0x65]);
    }
}