using MagicBytesValidator.Exceptions.Http;
using MagicBytesValidator.Models;
using Microsoft.AspNetCore.Http;

namespace MagicBytesValidator.Services.Http
{
    public interface IFormFileTypeProvider
    {
        /// <summary>
        /// Mapping that is used for providing information
        /// </summary>
        Mapping Mapping { get; }

        /// <summary>
        /// Tries to find matching FileType for given IFormFile.
        /// </summary>
        /// <param name="formFile">Given IFormFile</param>
        /// <returns>Matching FileType (if known)</returns>
        /// <exception cref="MimeTypeMismatchException">
        /// When file-type by extension and given content-type (IFormFile.ContentType) differ.
        /// In this case, someone <i>could</i> try to circumvent the validation.
        /// </exception>
        FileType? FindFileTypeForFormFile(IFormFile formFile);
    }
}