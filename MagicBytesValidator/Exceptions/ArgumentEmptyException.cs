namespace MagicBytesValidator.Exceptions;

/// <summary>
/// Exception that can be thrown if a given argument is empty or contains an empty value.
/// </summary>
/// <remarks>
/// Starting with version 3.0, this exception derives from <see cref="ArgumentException"/> instead of
/// <see cref="ArgumentNullException"/> and <c>null</c> arguments throw an <see cref="ArgumentNullException"/>.
/// </remarks>
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