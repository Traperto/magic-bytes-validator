namespace MagicBytesValidator.Services.Http;

/// <inheritdoc />
public class FormFileTypeProvider : IFormFileTypeProvider
{
    private const char _FILE_EXTENSION_SEPARATOR = '.';

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
        var fileType = formFile.FileName.Contains(_FILE_EXTENSION_SEPARATOR)
            ? Mapping.FindByExtension(formFile.FileName.Split(_FILE_EXTENSION_SEPARATOR).Last())
            : Mapping.FindByMimeType(formFile.ContentType);

        if (fileType is null)
        {
            /* We don't know about the files' extension or MIME type. */
            return null;
        }

        if (fileType.MimeTypes.Contains(formFile.ContentType) == false)
        {
            /* This can only occur if the given form file has a file name and its extension indicates a different
             * MIME type as (also given) Content-Type. This *can* be an indicator that someone is trying to
             * mess with us. As we are a bit paranoid and also the file type is not unambiguous, we'll throw. */
            throw new MimeTypeMismatchException(fileType.MimeTypes, formFile.ContentType);
        }

        return fileType;
    }

    /// <inheritdoc />
    public async Task<IFileType?> FindValidatedTypeAsync(
        IFormFile formFile,
        Stream? formFileStream,
        CancellationToken cancellationToken
    )
    {
        var fileTypeByContentType = Mapping.FindByMimeType(formFile.ContentType);
        if (fileTypeByContentType is null)
        {
            return null;
        }

        var fileTypeByExtension = formFile.FileName.Contains(_FILE_EXTENSION_SEPARATOR)
            ? Mapping.FindByExtension(formFile.FileName.Split(_FILE_EXTENSION_SEPARATOR).Last())
            : null;

        if (
            fileTypeByExtension is not null
            && fileTypeByExtension.GetType() != fileTypeByContentType.GetType()
        )
        {
            /* This can only occur if the given form file has a file name and its extension indicates a different
             * MIME type as (also given) Content-Type. This *can* be an indicator that someone is trying to
             * mess with us. As we are a bit paranoid and also the file type is not unambiguous, we'll throw. */
            throw new MimeTypeMismatchException(fileTypeByExtension.MimeTypes, formFile.ContentType);
        }

        var contentIsValid = await _validator.IsValidAsync(
            formFileStream ?? formFile.OpenReadStream(),
            fileTypeByContentType,
            cancellationToken
        );

        if (!contentIsValid)
        {
            throw new MimeTypeMismatchException(formFile.ContentType);
        }

        return fileTypeByContentType;
    }
}