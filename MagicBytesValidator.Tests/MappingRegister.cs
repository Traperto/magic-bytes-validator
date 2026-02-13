namespace MagicBytesValidator.Tests;

public class MappingRegister
{
    private readonly Mapping _mapping = new();

    private readonly IFileType _trpFileType = new TestFileType(
        ["traperto/trp"],
        ["trp"]
    ).StartsWith([0x74, 0x72, 0x61, 0x70, 0x65, 0x72, 0x74, 0x6f]);

    [Fact]
    public void Should_register_single_filetype()
    {
        _mapping.Register(_trpFileType);
        Assert.Contains(_trpFileType, _mapping.FileTypes);
    }

    [Fact]
    public void Should_register_list_filetype()
    {
        var kryptobiFileType = new TestFileType(
            ["traperto/tobiasjanssen"],
            ["tjn"]
        ).StartsWith([0x74, 0x6f, 0x62, 0x69, 0x61, 0x73, 0x6a, 0x61, 0x6e, 0x73, 0x73, 0x65, 0x6e]);

        _mapping.Register([kryptobiFileType]);
        Assert.Contains(kryptobiFileType, _mapping.FileTypes);
    }

    [Fact]
    public void Should_register_assembly_fileTypes()
    {
        var assembly = typeof(AssemblyFacade).Assembly;
        _mapping.Register(assembly);

        Assert.Contains(_mapping.FileTypes, f => f.MimeTypes.Contains("facade/trp"));
    }
}

public class AssemblyFacade : FileByteFilter
{
    public AssemblyFacade() : base(
        ["facade/trp"],
        ["trp"]
    )
    {
        StartsWith([0x74, 0x72, 0x70]);
    }
}