namespace MagicBytesValidator.Services.Http;

/// <inheritdoc />
public class FormFileTypeProvider : IFormFileTypeProvider
{
   private const char FileExtensionSeparator = '.';

   /// <inheritdoc />
   public Mapping Mapping { get; }

   private readonly IValidator _validator;

   public FormFileTypeProvider(
      Mapping? mapping = null,
      IValidator? validator = null
   )
   {
      Mapping = mapping ?? new Mapping();
      _validator = validator ?? new Validator(Mapping);
   }

   /// <inheritdoc />
   [Obsolete("Use FindValidatedType instead.")]
   public IFileType? FindFileTypeForFormFile(IFormFile formFile)
   {
      /* If the form file has a file name with an extension, we'll try to find the fileType by it first.
       * If not, we'll try loading it by its given content type. */
      var extension = GetExtension(formFile.FileName);
      var mediaType = GetMediaType(formFile.ContentType);

      var fileType = extension is not null
         ? Mapping.FindByExtension(extension)
         : mediaType is not null
            ? Mapping.FindByMimeType(mediaType)
            : null;

      if (fileType is null)
      {
         /* We don't know about the files' extension or MIME type. */
         return null;
      }

      if (!fileType.MimeTypes.Contains(mediaType, StringComparer.OrdinalIgnoreCase))
      {
         /* This can only occur if the given form file has a file name and its extension indicates a different
          * MIME type as (also given) Content-Type. This *can* be an indicator that someone is trying to
          * mess with us. As we are a bit paranoid and also the file type is not unambiguous, we'll throw. */
         throw new MimeTypeMismatchException(fileType.MimeTypes, formFile.ContentType);
      }

      return fileType;
   }

   public async Task<IFileType?> FindValidatedTypeAsync(
      IFormFile formFile,
      Stream? formFileStream,
      CancellationToken cancellationToken,
      FileByteType validationType = FileByteType.Strict
   )
   {
      var mediaType = GetMediaType(formFile.ContentType);
      var fileTypeByContentType = mediaType is not null
         ? Mapping.FindByMimeType(mediaType)
         : null;

      if (fileTypeByContentType is null)
      {
         return null;
      }

      var extension = GetExtension(formFile.FileName);
      var fileTypeByExtension = extension is not null
         ? Mapping.FindByExtension(extension)
         : null;

      if (fileTypeByExtension is not null
          && fileTypeByExtension.GetType() != fileTypeByContentType.GetType())
      {
         throw new MimeTypeMismatchException(fileTypeByExtension.MimeTypes, formFile.ContentType);
      }

      /* Only dispose the stream if we opened it ourselves; a given stream belongs to the caller. */
      var ownsStream = formFileStream is null;
      var stream = formFileStream ?? formFile.OpenReadStream();

      bool contentIsValid;
      try
      {
         contentIsValid = await _validator.IsValidAsync(
            stream,
            fileTypeByContentType,
            cancellationToken,
            validationType
         );
      }
      finally
      {
         if (ownsStream)
         {
            await stream.DisposeAsync();
         }
      }

      return !contentIsValid
         ? throw new MimeTypeMismatchException(formFile.ContentType)
         : fileTypeByContentType;
   }

   /// <summary>
   /// Returns the file extension without the leading separator or null if the file name has none
   /// (e.g. "file" or "file.").
   /// </summary>
   private static string? GetExtension(string? fileName)
   {
      var extension = Path.GetExtension(fileName)?.TrimStart(FileExtensionSeparator);

      return string.IsNullOrEmpty(extension) ? null : extension;
   }

   /// <summary>
   /// Returns the media type of a Content-Type header value without parameters
   /// (e.g. "text/plain; charset=utf-8" becomes "text/plain") or null if it is missing or invalid.
   /// </summary>
   private static string? GetMediaType(string? contentType)
   {
      return System.Net.Http.Headers.MediaTypeHeaderValue.TryParse(contentType, out var parsed)
         ? parsed.MediaType
         : null;
   }
}
