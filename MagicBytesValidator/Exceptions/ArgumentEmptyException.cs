using System;

namespace MagicBytesValidator.Exceptions
{
    /// <summary>
    /// Exception that can be thrown if a given argument is empty or contains an empty value.
    /// </summary>
    public class ArgumentEmptyException : ArgumentNullException
    {
        /// <summary>
        /// Creates a new ArgumentEmptyException.
        /// </summary>
        /// <param name="parameterName">Name of the invalid argument.</param>
        public ArgumentEmptyException(string parameterName) : base($"{parameterName} must not be empty or null.")
        {
        }
    }
}