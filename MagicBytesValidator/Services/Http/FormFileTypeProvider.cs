using System.Linq;
using MagicBytesValidator.Exceptions.Http;
using MagicBytesValidator.Models;
using Microsoft.AspNetCore.Http;

namespace MagicBytesValidator.Services.Http;

/// <summary>
/// Service that provides file information for given <see cref="IFormFile"/>.
/// </summary>
public class FormFileTypeProvider : IFormFileTypeProvider
{
    private const char _FILE_EXTENSION_SEPARATOR = '.';

    /// <summary>
    /// Mapping that is used for providing information
    /// </summary>
    public Mapping Mapping { get; }

    public FormFileTypeProvider(Mapping? mapping = null)
    {
        Mapping = mapping ?? new Mapping();
    }

    /// <summary>
    /// Tries to find matching FileType for given IFormFile.
    /// </summary>
    /// <param name="formFile">Given IFormFile</param>
    /// <returns>Matching FileType (if known)</returns>
    /// <exception cref="MimeTypeMismatchException">
    /// When file-type by extension and given content-type (IFormFile.ContentType) differ.
    /// In this case, someone <i>could</i> try to circumvent the validation.
    /// </exception>
    public FileType? FindFileTypeForFormFile(IFormFile formFile)
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
}