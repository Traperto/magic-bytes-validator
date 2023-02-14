using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicBytesValidator.Models;

namespace MagicBytesValidator.Services
{
    public class Validator : IValidator
    {
        /// <summary>
        /// Mapping that is used during validation
        /// </summary>
        public Mapping Mapping { get; }

        public Validator(Mapping? mapping = null)
        {
            Mapping = mapping ?? new Mapping();
        }

        /// <summary>
        /// Validates a given file-Stream against a given FileType and returns if the Stream is valid or not.
        /// </summary>
        /// <param name="fileStream">Stream of the file that should be validated</param>
        /// <param name="fileType">FileType that the stream should be validated against</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Returns  if the Stream matches one of the FileStream's magic-byte sequences.</returns>
        public async Task<bool> IsValidAsync(
            Stream fileStream,
            FileType fileType,
            CancellationToken cancellationToken
        )
        {
            var maxLengthFileTypeMagicByteSequences = fileType.MagicByteSequences.Max(mb => mb.Length);
            var streamBytes = new byte[maxLengthFileTypeMagicByteSequences];

            var currentFileStreamPosition = fileStream.Position;
            fileStream.Position = fileType.MagicByteOffset; /* Reset the stream to get to the first bytes. */

            await fileStream.ReadAsync(
                streamBytes.AsMemory(0, maxLengthFileTypeMagicByteSequences),
                cancellationToken
                );

            fileStream.Position = currentFileStreamPosition; /* Reset the position */

            return fileType.MagicByteSequences.Any(mb => mb.SequenceEqual(streamBytes.Take(mb.Length)));
        }
    }
}