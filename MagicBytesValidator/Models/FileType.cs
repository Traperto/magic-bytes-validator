using System.Linq;
using MagicBytesValidator.Exceptions;

namespace MagicBytesValidator.Models
{
    /// <summary>
    /// A FileType contains all necessary information to identify and validate the type of a file (based on MIME type,
    /// extensions and magic-byte sequences).
    /// </summary>
    public class FileType
    {
        /// <summary>
        /// MIME type of a file
        /// <example>"image/gif"</example>
        /// </summary>
        public string MimeType { get; }
        /// <summary>
        /// List of file extensions for a type
        /// <example>[ "gif" ]</example>
        /// </summary>
        public string[] Extensions { get; }
        /// <summary>
        /// List of magic-byte sequences to identify a file type based on the file contents
        /// <example>[ [47 49 46 38 37 61], [47 49 46 38 39 61] ] for "gif" files</example>
        /// </summary>
        public byte[][] MagicByteSequences { get; }

        /// <summary>
        /// Creates a new FileType
        /// </summary>
        /// <param name="mimeType">MIME type of the new file type</param>
        /// <param name="extensions">File extensions of the new file type</param>
        /// <param name="magicByteSequences">Magic byte sequences of the new file type</param>
        /// <exception cref="ArgumentEmptyException">
        /// When any property of FileType is empty or contains empty values
        /// </exception>
        public FileType(string mimeType, string[] extensions, byte[][] magicByteSequences)
        {
            if (string.IsNullOrEmpty(mimeType))
            {
                throw new ArgumentEmptyException(nameof(mimeType));
            }

            if (!extensions.Any() || extensions.Any(string.IsNullOrEmpty))
            {
                throw new ArgumentEmptyException(nameof(extensions));
            }

            if (!magicByteSequences.Any() || magicByteSequences.Any(mbs => mbs.Length == 0))
            {
                throw new ArgumentEmptyException(nameof(magicByteSequences));
            }

            MimeType = mimeType;
            Extensions = extensions;
            MagicByteSequences = magicByteSequences;
        }
    }
}