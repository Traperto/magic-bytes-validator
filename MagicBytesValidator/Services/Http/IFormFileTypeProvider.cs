using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MagicBytesValidator.Exceptions.Http;
using MagicBytesValidator.Models;
using Microsoft.AspNetCore.Http;

namespace MagicBytesValidator.Services.Http;

/// <summary>
/// Service that provides file information for given <see cref="IFormFile"/>.
/// </summary>
public interface IFormFileTypeProvider
{
    /// <summary>
    /// Mapping that is used for providing information
    /// </summary>
    Mapping Mapping { get; }

    /// <summary>
    /// Tries to find matching FileType for given IFormFile.
    /// </summary>
    /// <exception cref="MimeTypeMismatchException">
    /// When file-type by extension and given content-type (IFormFile.ContentType) differ.
    /// In this case, someone <i>could</i> try to circumvent the validation.
    /// </exception>
    [Obsolete("Use FindValidatedType instead.")]
    IFileType? FindFileTypeForFormFile(IFormFile formFile);

    /// <summary>
    /// Tries to find matching <see cref="IFileType"/> for given <see cref="IFormFile"/> that also matches
    /// the content of the form file.
    /// </summary>
    /// <param name="formFile"><see cref="IFormFile"/> that the <see cref="IFileType"/> should be found for</param>
    /// <param name="formFileStream">
    /// Optional. If the file stream for the form file is already loaded, it can be included here.
    /// This prevents opening a read stream for the same file multiple times.
    /// <b>However, never include streams of other files than the given form file! Otherwise the validation may be
    /// wrong and could be circumvented!</b>
    /// </param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>
    /// <see cref="IFileType"/> that matches by the form files content type, content (and extension, if given)
    /// </returns>
    /// <exception cref="MimeTypeMismatchException">
    /// When file-type by extension and given content-type (IFormFile.ContentType) differ.
    /// In this case, someone <i>could</i> try to circumvent the validation.
    /// </exception>
    Task<IFileType?> FindValidatedTypeAsync(
        IFormFile formFile,
        Stream? formFileStream,
        CancellationToken cancellationToken
    );
}