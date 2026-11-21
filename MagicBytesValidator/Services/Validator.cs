namespace MagicBytesValidator.Services;

public class Validator : IValidator
{
   /// <inheritdoc />
   public IMapping Mapping { get; }

   public Validator(IMapping? mapping = null)
   {
      Mapping = mapping ?? new Mapping();
   }

   /// <inheritdoc />
   public async Task<bool> IsValidAsync(
      Stream fileStream,
      IFileType fileType,
      CancellationToken cancellationToken,
      FileByteType validationType = FileByteType.Strict
   )
   {
      ArgumentNullException.ThrowIfNull(fileStream);
      ArgumentNullException.ThrowIfNull(fileType);

      var streamBuffer = await fileStream.ReadAllBytesFromStartAsync(cancellationToken);

      return fileType.Matches(streamBuffer, validationType);
   }
}
