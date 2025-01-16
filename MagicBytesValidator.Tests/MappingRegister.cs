namespace MagicBytesValidator.Tests;

public class MappingRegister
{
    private readonly Mapping _mapping = new();

    private readonly IFileType _trpFileType = new FileByteFilter(
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
        var cusDbcLaikaFileType = new FileByteFilter(
            ["traperto/laikaschmidt"],
            ["nms"]
        ).StartsWith([0x6e, 0x69, 0x6b, 0x6c, 0x61, 0x73, 0x73, 0x63, 0x68, 0x6d, 0x69, 0x64, 0x74]);

        var kryptobiFileType = new FileByteFilter(
            ["traperto/tobiasjanssen"],
            ["tjn"]
        ).StartsWith([0x74, 0x6f, 0x62, 0x69, 0x61, 0x73, 0x6a, 0x61, 0x6e, 0x73, 0x73, 0x65, 0x6e]);

        _mapping.Register(new[] { cusDbcLaikaFileType, kryptobiFileType });
        Assert.Contains(kryptobiFileType, _mapping.FileTypes);
        Assert.Contains(cusDbcLaikaFileType, _mapping.FileTypes);
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