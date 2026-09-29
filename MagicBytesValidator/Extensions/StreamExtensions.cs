namespace MagicBytesValidator.Extensions;

internal static class StreamExtensions
{
    /// <summary>
    /// Reads the whole stream from its beginning and restores the previous position afterwards.
    /// A single <see cref="Stream.ReadAsync(Memory{byte}, CancellationToken)"/> may return fewer bytes than
    /// requested (e.g. file- or network-backed streams), so this keeps reading until the end of the stream.
    /// </summary>
    public static async Task<byte[]> ReadAllBytesFromStartAsync(this Stream stream, CancellationToken cancellationToken)
    {
        var previousStreamPosition = stream.Position;
        stream.Position = 0;

        var buffer = new byte[stream.Length];
        var bytesRead = await stream.ReadAtLeastAsync(
            buffer,
            buffer.Length,
            throwOnEndOfStream: false,
            cancellationToken);

        stream.Position = previousStreamPosition;

        return bytesRead == buffer.Length ? buffer : buffer[..bytesRead];
    }
}
