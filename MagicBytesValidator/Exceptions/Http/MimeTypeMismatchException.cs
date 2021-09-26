using System;

namespace MagicBytesValidator.Exceptions.Http
{
    /// <summary>
    /// Exception that can be thrown if two MIME types (that should be equal) are different.
    /// </summary>
    public class MimeTypeMismatchException : Exception
    {
        public MimeTypeMismatchException(
            string mimeType1, 
            string mimeType2
            ) : base($"Mismatch of MIME types ('{mimeType1}' != '{mimeType2}')")
        {
        }
    }
}