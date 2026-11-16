namespace MagicBytesValidator.Tests;

/// <summary>
/// A ZIP file may end with a comment of up to 65535 bytes after the end of central directory record (#197).
/// </summary>
public class ZipCommentTests
{
    private const int EndOfCentralDirectoryLength = 22;

    [Theory]
    [InlineData(FileByteType.Strict)]
    [InlineData(FileByteType.Lazy)]
    public async Task Xlsx_with_zip_comment_must_be_valid(FileByteType validationType)
    {
        var validator = new Validator();
        using var stream = new MemoryStream(BuildXlsx("Aspose.Cells for Java v24.3.0"));

        var isValid = await validator.IsValidAsync(stream, new Xlsx(), CancellationToken.None, validationType);

        Assert.True(isValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(4)]
    [InlineData(ushort.MaxValue)]
    public void Zip_with_comment_of_any_length_must_match(int commentLength)
    {
        var bytes = BuildXlsx(new string('A', commentLength));

        Assert.True(new Zip().Matches(bytes));
        Assert.True(new Xlsx().Matches(bytes));
    }

    [Fact]
    public void Zip_comment_containing_an_end_of_central_directory_signature_must_match()
    {
        var bytes = BuildXlsx("PK\u0005\u0006 inside the comment");

        Assert.True(new Zip().Matches(bytes));
    }

    [Fact]
    public async Task Xlsx_with_zip_comment_must_be_detected()
    {
        var sut = new StreamFileTypeProvider(new Mapping());
        using var stream = new MemoryStream(BuildXlsx("Aspose.Cells for Java v24.3.0"));

        var matches = (await sut.FindAllMatchesAsync(stream, CancellationToken.None)).ToList();

        Assert.Contains(matches, fileType => fileType is Xlsx);
        Assert.Contains(matches, fileType => fileType.GetType() == typeof(Zip));
    }

    [Fact]
    public void Zip_with_bytes_after_end_of_central_directory_not_declared_as_comment_must_not_match()
    {
        var bytes = BuildXlsx(string.Empty).Concat("garbage"u8.ToArray()).ToArray();

        Assert.False(new Zip().Matches(bytes));
    }

    [Fact]
    public void Zip_with_comment_length_not_matching_the_trailing_bytes_must_not_match()
    {
        var bytes = BuildXlsx("comment");
        var commentLengthOffset = bytes.Length - "comment".Length - 2;
        BinaryPrimitives.WriteUInt16LittleEndian(bytes.AsSpan(commentLengthOffset), 3);

        Assert.False(new Zip().Matches(bytes));
    }

    [Fact]
    public void Zip_without_end_of_central_directory_must_not_match()
    {
        var bytes = BuildXlsx(string.Empty)[..^EndOfCentralDirectoryLength];

        Assert.False(new Zip().Matches(bytes));
    }

    /// <summary>Creates a minimal XLSX-like ZIP archive and appends the given comment to it.</summary>
    private static byte[] BuildXlsx(string comment)
    {
        using var output = new MemoryStream();

        using (var archive = new ZipArchive(output, ZipArchiveMode.Create, leaveOpen: true))
        {
            using var writer = new StreamWriter(archive.CreateEntry("xl/_rels/workbook.xml.rels").Open());
            writer.Write("<Relationships/>");
        }

        var bytes = output.ToArray();
        var commentBytes = Encoding.ASCII.GetBytes(comment);

        // The archive is written without a comment, so the record is located in the last 22 bytes.
        BinaryPrimitives.WriteUInt16LittleEndian(bytes.AsSpan(bytes.Length - 2), (ushort)commentBytes.Length);

        return bytes.Concat(commentBytes).ToArray();
    }
}
