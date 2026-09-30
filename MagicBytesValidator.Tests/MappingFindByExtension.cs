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
