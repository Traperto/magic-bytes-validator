namespace MagicBytesValidator.Extensions;

internal static class StreamExtensions
{
    /// <summary>
    /// Reads the whole stream from its beginning and restores the previous position afterwards (also on failure).
    /// A single <see cref="Stream.ReadAsync(Memory{byte}, CancellationToken)"/> may return fewer bytes than
    /// requested (e.g. file- or network-backed streams), so this keeps reading until the end of the stream.
    /// Non-seekable streams (e.g. a raw request body) can neither be rewound nor restored, so their remaining
    /// content is read and the stream is consumed afterwards.
    /// </summary>
    /// <exception cref="ArgumentException">When the stream is too large to be held in a single byte array</exception>
    public static async Task<byte[]> ReadAllBytesFromStartAsync(this Stream stream, CancellationToken cancellationToken)
    {
        if (!stream.CanSeek)
        {
            using var remainingContent = new MemoryStream();
            await stream.CopyToAsync(remainingContent, cancellationToken);

            return remainingContent.ToArray();
        }

        if (stream.Length > Array.MaxLength)
        {
            throw new ArgumentException(
                $"Streams larger than {Array.MaxLength} bytes are not supported (got {stream.Length} bytes).",
                nameof(stream));
        }

        var previousStreamPosition = stream.Position;

        try
        {
            stream.Position = 0;

            var buffer = new byte[stream.Length];
            var bytesRead = await stream.ReadAtLeastAsync(
                buffer,
                buffer.Length,
                throwOnEndOfStream: false,
                cancellationToken);

            return bytesRead == buffer.Length ? buffer : buffer[..bytesRead];
        }
        finally
        {
            stream.Position = previousStreamPosition;
        }
    }
}
