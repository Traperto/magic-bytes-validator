namespace MagicBytesValidator.Exceptions;

/// <summary>
/// Exception that is thrown if a given argument is empty or contains an empty value.
/// A <c>null</c> argument results in an <see cref="ArgumentNullException"/> instead.
/// </summary>
public class ArgumentEmptyException : ArgumentException
{
    /// <summary>
    /// Creates a new ArgumentEmptyException.
    /// </summary>
    /// <param name="parameterName">Name of the invalid argument.</param>
    public ArgumentEmptyException(string parameterName)
        : base("Value must not be empty or contain empty values.", parameterName)
    {
    }
}