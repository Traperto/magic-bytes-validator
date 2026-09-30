namespace MagicBytesValidator.Tests;

/// <summary>
/// DOC, XLS and PPT share the OLE/CFBF header and must be told apart by their root streams (see #178).
/// </summary>
public class DocXlsDisambiguationTests
{
    private const string SummaryInformation = "\u0005SummaryInformation";

    [Fact]
    public async Task Word97Document_ShouldBeUnambiguouslyDoc()
    {
        using var stream = CompoundFileBuilder.BuildStream(["WordDocument", "1Table", SummaryInformation]);

        Assert.IsType<Doc>(Assert.Single(await FindCloseMatchesAsync(stream)));
    }

    [Fact]
    public async Task Excel97Workbook_ShouldBeUnambiguouslyXls()
    {
        using var stream = CompoundFileBuilder.BuildStream(["Workbook", SummaryInformation]);

        Assert.IsType<Xls>(Assert.Single(await FindCloseMatchesAsync(stream)));
    }

    [Fact]
    public async Task Excel95Book_ShouldBeUnambiguouslyXls()
    {
        using var stream = CompoundFileBuilder.BuildStream(["Book", SummaryInformation]);

        Assert.IsType<Xls>(Assert.Single(await FindCloseMatchesAsync(stream)));
    }

    [Fact]
    public async Task PowerPointDocument_ShouldBeUnambiguouslyPpt()
    {
        using var stream = CompoundFileBuilder.BuildStream(["PowerPoint Document", "Current User", SummaryInformation]);

        Assert.IsType<Ppt>(Assert.Single(await FindCloseMatchesAsync(stream)));
    }

    [Fact]
    public async Task WordDocumentWithEmbeddedWorkbook_ShouldBeUnambiguouslyDoc()
    {
        using var stream = CompoundFileBuilder.BuildStream(
            ["WordDocument", "1Table"],
            ("_1234567890", ["Workbook", "\u0001Ole"])
        );

        Assert.IsType<Doc>(Assert.Single(await FindCloseMatchesAsync(stream)));
    }

    [Fact]
    public async Task PresentationWithEmbeddedWorkbookAndDocument_ShouldBeUnambiguouslyPpt()
    {
        using var stream = CompoundFileBuilder.BuildStream(
            ["PowerPoint Document", "Current User"],
            ("MBD0001", ["Workbook"]),
            ("MBD0002", ["WordDocument"])
        );

        Assert.IsType<Ppt>(Assert.Single(await FindCloseMatchesAsync(stream)));
    }

    [Fact]
    public async Task StreamNamesInContent_ShouldNotBeMistakenForStreams()
    {
        // "Workbook" as UTF-16 text inside the document content must not turn a DOC into an XLS.
        var bytes = CompoundFileBuilder.Build(["WordDocument"]);
        var workbookText = System.Text.Encoding.Unicode.GetBytes("Workbook Book PowerPoint Document");
        bytes = bytes.Concat(workbookText).ToArray();

        using var stream = new MemoryStream(bytes);

        Assert.IsType<Doc>(Assert.Single(await FindCloseMatchesAsync(stream)));
    }

    [Fact]
    public async Task Issue178Xls_ShouldBeValidatedAsXls()
    {
        // The FAT sector at offset 512 starts with FD FF FF FF, like the file reported in #178.
        var bytes = CompoundFileBuilder.Build(["Workbook"]);
        Assert.Equal([0xFD, 0xFF, 0xFF, 0xFF], bytes.Skip(512).Take(4));

        using var stream = new MemoryStream(bytes);
        var validator = new Validator();

        Assert.True(await validator.IsValidAsync(stream, new Xls(), CancellationToken.None));
        Assert.False(await validator.IsValidAsync(stream, new Doc(), CancellationToken.None));
        Assert.False(await validator.IsValidAsync(stream, new Ppt(), CancellationToken.None));
    }

    [Fact]
    public void StreamNames_ShouldBeComparedCaseInsensitive()
    {
        Assert.True(new Xls().Matches(CompoundFileBuilder.Build(["WORKBOOK"])));
    }

    [Fact]
    public void UnknownCompoundFile_ShouldMatchNoOfficeFormat()
    {
        var bytes = CompoundFileBuilder.Build([SummaryInformation, "__properties_version1.0"]);

        Assert.False(new Doc().Matches(bytes));
        Assert.False(new Xls().Matches(bytes));
        Assert.False(new Ppt().Matches(bytes));
    }

    [Fact]
    public void StreamInNestedStorageOnly_ShouldNotMatch()
    {
        var bytes = CompoundFileBuilder.Build([], ("Embedded", ["WordDocument"]));

        Assert.False(new Doc().Matches(bytes));
    }

    [Fact]
    public void HeaderOnly_ShouldNotMatch()
    {
        var bytes = CompoundFileBuilder.Signature.Concat(new byte[504]).ToArray();

        Assert.False(new Doc().Matches(bytes));
        Assert.False(new Xls().Matches(bytes));
        Assert.False(new Ppt().Matches(bytes));
    }

    [Fact]
    public void TruncatedFile_ShouldNotMatch()
    {
        var bytes = CompoundFileBuilder.Build(["WordDocument"]);

        Assert.False(new Doc().Matches(bytes.Take(1024).ToArray()));
    }

    [Fact]
    public void CyclicDirectoryChain_ShouldNotHangAndNotMatchOtherFormats()
    {
        var bytes = CompoundFileBuilder.Build(["WordDocument"]);

        // Let the (single) directory sector 1 point to itself in the FAT.
        BitConverter.GetBytes(1u).CopyTo(bytes, 512 + 4);

        Assert.True(new Doc().Matches(bytes));
        Assert.False(new Xls().Matches(bytes));
    }

    [Fact]
    public void CyclicSiblingTree_ShouldNotHang()
    {
        var bytes = CompoundFileBuilder.Build(["WordDocument", "1Table"]);

        // Entry 2 ("1Table") gets entry 1 ("WordDocument") as right sibling, closing a cycle.
        var entry2Offset = 1024 + (2 * 128);
        BitConverter.GetBytes(1u).CopyTo(bytes, entry2Offset + 0x48);

        Assert.True(new Doc().Matches(bytes));
    }

    [Fact]
    public void CompoundFileStreamAnyOf_ShouldRejectEmptyNames()
    {
        Assert.Throws<MagicBytesValidator.Exceptions.ArgumentEmptyException>(() => new StreamNameFilter([]));
    }

    private static Task<IReadOnlyList<IFileType>> FindCloseMatchesAsync(Stream stream)
    {
        return new StreamFileTypeProvider(new Mapping()).FindCloseMatchesAsync(stream, CancellationToken.None);
    }

    // Has no parameterless constructor, so assembly scanning (see MappingRegister) ignores it.
    private sealed class StreamNameFilter : FileByteFilter
    {
        public StreamNameFilter(string[] streamNames) : base(["application/x-test"], ["test"])
        {
            CompoundFileStreamAnyOf(streamNames);
        }
    }
}
