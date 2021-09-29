using System;
using System.Collections.Generic;
using System.Linq;
using MagicBytesValidator.Exceptions;
using MagicBytesValidator.Formats;
using MagicBytesValidator.Models;

namespace MagicBytesValidator.Services
{
    /// <inheritdoc />
    public class Mapping : IMapping
    {
        /// <inheritdoc />
        public IReadOnlyList<FileType> FileTypes => _fileTypes;

        private readonly IReadOnlyList<FileType> _fileTypes = FileTypeCollector.CollectFileTypes();

        /// <inheritdoc />
        public FileType? FindByMimeType(string mimeType)
        {
            if (string.IsNullOrEmpty(mimeType))
            {
                throw new ArgumentEmptyException(nameof(mimeType));
            }

            return _fileTypes.FirstOrDefault(f =>
                                                 string.Equals(f.MimeType, mimeType,
                                                               StringComparison.InvariantCultureIgnoreCase)
                                            );
        }

        /// <inheritdoc />
        public FileType? FindByExtension(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                throw new ArgumentEmptyException(nameof(extension));
            }

            return _fileTypes.FirstOrDefault(
                                             f => f.Extensions.Any(fe => string.Equals(fe, extension,
                                                                    StringComparison.InvariantCultureIgnoreCase))
                                            );
        }

        /// <inheritdoc />
        public void Register(FileType fileType)
        {
            if (FindByMimeType(fileType.MimeType) is not null)
            {
                throw new DuplicateEntryException(nameof(fileType));
            }

            if (fileType.Extensions.Any(e => FindByExtension(e) is not null))
            {
                throw new DuplicateEntryException(nameof(fileType));
            }

            if (fileType.MagicByteSequences.Any(mbs => FindByMagicByteSequence(mbs) is not null))
            {
                throw new DuplicateEntryException(nameof(fileType));
            }

            _fileTypes.ToList().Add(fileType);
        }

        /// <inheritdoc />
        public void Register(string mimeType, string[] extensions, byte[][] magicByteSequences)
        {
            Register(
                     new FileType(mimeType, extensions, magicByteSequences)
                    );
        }

        /// <summary>
        /// Tries to find a known FileType by magic byte sequence.
        /// </summary>
        /// <param name="magicByteSequence">Magic byte sequence that should be searched for</param>
        /// <returns>FileType that contains the given magic byte sequence</returns>
        /// <exception cref="ArgumentEmptyException">When magicByteSequence is empty</exception>
        private FileType? FindByMagicByteSequence(byte[] magicByteSequence)
        {
            if (!magicByteSequence.Any())
            {
                throw new ArgumentEmptyException(nameof(magicByteSequence));
            }

            return _fileTypes.FirstOrDefault(
                                             f => f.MagicByteSequences.Any(mb => mb.SequenceEqual(magicByteSequence))
                                            );
        }
    }
}