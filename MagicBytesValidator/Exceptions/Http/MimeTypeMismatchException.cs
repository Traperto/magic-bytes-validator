using System;
using System.Collections.Generic;

namespace MagicBytesValidator.Exceptions.Http;

/// <summary>
/// Exception that can be thrown if two MIME types (that should be equal) are different.
/// </summary>
public class MimeTypeMismatchException : Exception
{
    public MimeTypeMismatchException(
        IEnumerable<string> mimeTypes,
        string mimeType2
    ) : base($"Mismatch of MIME types ('{mimeType2}' not in ['{string.Join(",", mimeTypes)}'].)")
    {
    }
}