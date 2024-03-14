namespace MagicBytesValidator.Services;

public class Validator : IValidator
{
    /// <inheritdoc />
    public Mapping Mapping { get; }

    public Validator(Mapping? mapping = null)
    {
        Mapping = mapping ?? new Mapping();
    }

    /// <inheritdoc />
    public async Task<bool> IsValidAsync(Stream fileStream, IFileType fileType, CancellationToken cancellationToken)
    {
        var previousStreamPosition = fileStream.Position;
        fileStream.Position = 0;

        var streamBuffer = new byte[fileStream.Length];
        _ = await fileStream.ReadAsync(streamBuffer, cancellationToken);

        fileStream.Position = previousStreamPosition;

        return fileType.Matches(streamBuffer);
    }
}