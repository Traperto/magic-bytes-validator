using System.Collections.Generic;
using System.Reflection;
using MagicBytesValidator.Exceptions;
using MagicBytesValidator.Models;

namespace MagicBytesValidator.Services
{
    public interface IMapping
    {
        /// <summary>
        /// Currently registered <see cref="FileType"/>
        /// </summary>
        IReadOnlyList<FileType> FileTypes { get; }

        /// <summary>
        /// Tries to find a known <see cref="FileType"/> by given MIME type.
        /// </summary>
        /// <param name="mimeType">MIME type that should be searched for</param>
        /// <returns>FileType that belongs to the given MIME type</returns>
        /// <exception cref="ArgumentEmptyException">When given MIME type is null or empty</exception>
        FileType? FindByMimeType(string mimeType);

        /// <summary>
        /// Tries to find a known <see cref="FileType"/> by given file extension.
        /// </summary>
        /// <param name="extension">File extension that should be searched for</param>
        /// <returns>FileType that contains the given file extension</returns>
        /// <exception cref="ArgumentEmptyException">When given file extension is null or empty</exception>
        FileType? FindByExtension(string extension);

        /// <summary>
        /// Registers a new <see cref="FileType"/> in the mapping.
        /// </summary>
        /// <param name="fileType">FileType to register</param>
        void Register(FileType fileType);

        /// <summary>
        /// Registers a collection of <see cref="FileType"/> in the mapping.
        /// </summary>
        /// <param name="fileTypes">Collection of FileType to register</param>
        void Register(IReadOnlyList<FileType> fileTypes);
        
        /// <summary>
        /// Registers all <see cref="FileType"/> that a part of given assembly
        /// </summary>
        /// <param name="assembly"></param>
        void Register(Assembly assembly);
    }
}