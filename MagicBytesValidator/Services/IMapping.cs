using System.Collections.Generic;
using MagicBytesValidator.Exceptions;
using MagicBytesValidator.Models;

namespace MagicBytesValidator.Services
{
    public interface IMapping
    {
        /// <summary>
        /// Currently registered FileTypes
        /// </summary>
        IReadOnlyList<FileType> FileTypes { get; }

        /// <summary>
        /// Tries to find a known FileType by given MIME type.
        /// </summary>
        /// <param name="mimeType">MIME type that should be searched for</param>
        /// <returns>FileType that belongs to the given MIME type</returns>
        /// <exception cref="ArgumentEmptyException">When given MIME type is null or empty</exception>
        FileType? FindByMimeType(string mimeType);

        /// <summary>
        /// Tries to find a known FileType by given file extension.
        /// </summary>
        /// <param name="extension">File extension that should be searched for</param>
        /// <returns>FileType that contains the given file extension</returns>
        /// <exception cref="ArgumentEmptyException">When given file extension is null or empty</exception>
        FileType? FindByExtension(string extension);

        /// <summary>
        /// Registers a new FileType in the mapping.
        /// </summary>
        /// <param name="fileType">FileType to register</param>
        /// <exception cref="DuplicateEntryException">
        /// When any property of FileType would result in a duplicate entry in the mapping
        /// </exception>
        void Register(FileType fileType);

        /// <summary>
        /// Registers new file type by given parameters
        /// </summary>
        /// <param name="mimeType">MIME type of the new file type</param>
        /// <param name="extensions">File extensions of the new file type</param>
        /// <param name="magicByteSequences">Magic byte sequences of the new file type</param>
        /// <exception cref="ArgumentEmptyException">
        /// When any property of FileType is empty or contains empty values
        /// </exception>
        /// <exception cref="DuplicateEntryException">
        /// When any property of FileType would result in a duplicate entry in the mapping
        /// </exception>
        void Register(string mimeType, string[] extensions, byte[][] magicByteSequences);
    }
}