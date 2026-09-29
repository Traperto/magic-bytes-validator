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
   public async Task<bool> IsValidAsync(
      Stream fileStream,
      IFileType fileType,
      CancellationToken cancellationToken,
      FileByteType validationType = FileByteType.Strict
   )
   {
      var streamBuffer = await fileStream.ReadAllBytesFromStartAsync(cancellationToken);

      return fileType.Matches(streamBuffer, validationType);
   }
}
