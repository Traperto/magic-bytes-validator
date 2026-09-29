namespace MagicBytesValidator.Tests;

/// <summary>
/// Streams may return fewer bytes per read than requested (e.g. disk-backed IFormFile, network streams).
/// Checks against the end of the file must still see the real trailing bytes (#176).
/// </summary>
public class PartialReadStreamTests
{
    private static readonly byte[] PngBytes =
    [
        0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A,
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82
    ];

    [Fact]
    public async Task Validator_should_read_whole_stream_when_reads_are_partial()
    {
        var validator = new Validator();
        using var stream = new PartialReadStream(PngBytes, maxBytesPerRead: 3);

        var isValid = await validator.IsValidAsync(stream, new Png(), CancellationToken.None);

        Assert.True(isValid);
    }

    [Fact]
    public async Task Validator_should_validate_large_zip_offset_when_reads_are_partial()
    {
        var validator = new Validator();
        using var stream = new PartialReadStream(BuildZipBytesWithEocd(0x0126BCBC), maxBytesPerRead: 16);

        var isValid = await validator.IsValidAsync(stream, new Zip(), CancellationToken.None);

        Assert.True(isValid);
    }

    [Fact]
    public async Task Validator_should_restore_stream_position_when_reads_are_partial()
    {
        var validator = new Validator();
        using var stream = new PartialReadStream(PngBytes, maxBytesPerRead: 3);
        stream.Position = 5;

        _ = await validator.IsValidAsync(stream, new Png(), CancellationToken.None);

        Assert.Equal(5, stream.Position);
    }

    [Fact]
    public async Task StreamFileTypeProvider_should_read_whole_stream_when_reads_are_partial()
    {
        var sut = new StreamFileTypeProvider(new Mapping());
        using var stream = new PartialReadStream(PngBytes, maxBytesPerRead: 3);

        var result = await sut.TryFindUnambiguousAsync(stream, CancellationToken.None);

        Assert.IsType<Png>(result);
    }

    private static byte[] BuildZipBytesWithEocd(uint centralDirectoryOffset)
    {
        var bytes = new byte[256];
        bytes[0] = 0x50;
        bytes[1] = 0x4B;
        bytes[2] = 0x03;
        bytes[3] = 0x04;

        var eocdStart = bytes.Length - 22;
        bytes[eocdStart + 0] = 0x50;
        bytes[eocdStart + 1] = 0x4B;
        bytes[eocdStart + 2] = 0x05;
        bytes[eocdStart + 3] = 0x06;
        BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(eocdStart + 16), centralDirectoryOffset);

        return bytes;
    }

    /// <summary>Seekable stream that never returns more than <c>maxBytesPerRead</c> bytes per read call.</summary>
    private sealed class PartialReadStream(byte[] data, int maxBytesPerRead) : MemoryStream(data)
    {
        public override int Read(byte[] buffer, int offset, int count)
        {
            return base.Read(buffer, offset, Math.Min(count, maxBytesPerRead));
        }

        public override int Read(Span<byte> buffer)
        {
            return base.Read(buffer[..Math.Min(buffer.Length, maxBytesPerRead)]);
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return base.ReadAsync(buffer, offset, Math.Min(count, maxBytesPerRead), cancellationToken);
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return base.ReadAsync(buffer[..Math.Min(buffer.Length, maxBytesPerRead)], cancellationToken);
        }
    }
}
