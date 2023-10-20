using System.Collections.Generic;
using System.Reflection;
using MagicBytesValidator.Exceptions;
using MagicBytesValidator.Models;

namespace MagicBytesValidator.Services;

public interface IMapping
{
    /// <summary>
    /// Currently registered <see cref="IFileType"/>
    /// </summary>
    IReadOnlyList<IFileType> FileTypes { get; }

    /// <summary>
    /// Tries to find a known <see cref="IFileType"/> by given MIME type.
    /// </summary>
    /// <exception cref="ArgumentEmptyException">When given MIME type is null or empty</exception>
    IFileType? FindByMimeType(string mimeType);

    /// <summary>
    /// Tries to find a known <see cref="IFileType"/> by given file extension.
    /// </summary>
    /// <exception cref="ArgumentEmptyException">When given file extension is null or empty</exception>
    IFileType? FindByExtension(string extension);

    /// <summary>
    /// Registers a new <see cref="IFileType"/> in the mapping.
    /// </summary>
    void Register(IFileType fileType);

    /// <summary>
    /// Registers a collection of <see cref="IFileType"/> in the mapping.
    /// </summary>
    void Register(IEnumerable<IFileType> fileTypes);

    /// <summary>
    /// Registers all <see cref="IFileType"/> that a part of given assembly
    /// </summary>
    /// <param name="assembly">Assembly that will be searched for <see cref="IFileType"/>s</param>
    void Register(Assembly assembly);
}