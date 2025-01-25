namespace MagicBytesValidator.Tests.Streams;

public class FindCloseMatchesAsync
{
    [Fact]
    public async Task Should_skip_parent_types()
    {
        var parentFileType = new ParentFileType();
        var childFileType = new ChildFileType();
        var unrelatedFileType = new TestFileType()
            .Anywhere([3, 4, 5]);

        var mapping = new Mock<IMapping>();
        mapping
            .SetupGet(m => m.FileTypes)
            .Returns([
                unrelatedFileType,
                parentFileType,
                childFileType,
            ]);

        var sut = new StreamFileTypeProvider(mapping.Object);

        /* Matches both parent and child file type -> result should only include child */
        var stream = new MemoryStream([1, 2, 3, 4, 5, 6]);

        var result = (await sut.FindCloseMatchesAsync(stream, CancellationToken.None)).ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(childFileType, result);
        Assert.Contains(unrelatedFileType, result);
    }

    [Fact]
    public async Task Should_work_with_multiple_layers()
    {
        var parentFileType = new ParentFileType();
        var childFileType = new ChildFileType();
        var grandchildFileType = new GrandchildFileType();

        var mapping = new Mock<IMapping>();
        mapping
            .SetupGet(m => m.FileTypes)
            .Returns([
                grandchildFileType,
                childFileType,
                parentFileType,
            ]);

        var sut = new StreamFileTypeProvider(mapping.Object);

        /* Matches parent, child and grandchild file type -> result should only include grandchild */
        var stream = new MemoryStream([1, 2, 3, 10, 11, 12, 4, 5, 6]);

        var result = (await sut.FindCloseMatchesAsync(stream, CancellationToken.None)).ToList();

        Assert.Single(result);
        Assert.Contains(grandchildFileType, result);
    }
}

