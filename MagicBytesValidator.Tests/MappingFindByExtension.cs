namespace MagicBytesValidator.Tests;

public class MappingFindByExtension
{
    private readonly Mapping _mapping = new();

    [Theory]
    [InlineData("mpg", typeof(Mpg))]
    [InlineData("mpeg", typeof(Mpg))]
    [InlineData("ts", typeof(Tsv))]
    [InlineData("qtx", typeof(Exe))]
    public void Should_find_by_extension(string extension, Type expectedType)
    {
        Assert.IsType(expectedType, _mapping.FindByExtension(extension));
    }

    [Fact]
    public void Should_throw_on_invalid_lookup_arguments()
    {
        Assert.Throws<ArgumentNullException>(() => _mapping.FindByExtension(null!));
        Assert.Throws<ArgumentNullException>(() => _mapping.FindByMimeType(null!));

        var emptyExtension = Assert.Throws<ArgumentEmptyException>(() => _mapping.FindByExtension(string.Empty));
        Assert.Equal("extension", emptyExtension.ParamName);

        var emptyMimeType = Assert.Throws<ArgumentEmptyException>(() => _mapping.FindByMimeType(string.Empty));
        Assert.Equal("mimeType", emptyMimeType.ParamName);
    }

    [Fact]
    public void Should_throw_on_invalid_file_type_definition()
    {
        Assert.Throws<ArgumentNullException>(() => new TestFileType(null!, ["trp"]));

        var emptyMimeTypes = Assert.Throws<ArgumentEmptyException>(() => new TestFileType([], ["trp"]));
        Assert.Equal("mimeTypes", emptyMimeTypes.ParamName);

        var emptyExtension = Assert.Throws<ArgumentEmptyException>(() => new TestFileType(["traperto/trp"], [""]));
        Assert.Equal("extensions", emptyExtension.ParamName);
    }

    [Fact]
    public void Should_have_unique_extensions_for_media_types()
    {
        /* Ambiguous extensions make the result of FindByExtension depend on the (unspecified) reflection order. */
        var ambiguousExtensions = _mapping.FileTypes
            .SelectMany(fileType => fileType.Extensions.Select(extension => (extension, fileType)))
            .GroupBy(entry => entry.extension, StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        /* "com" is shared by Exe and the generic Bin type on purpose. */
        Assert.Equal(["com"], ambiguousExtensions);
    }
}
