using System;

namespace MagicBytesValidator.Exceptions;

/// <summary>
/// Exception that can be thrown if something would result in a duplicate entry if added.
/// </summary>
public class DuplicateEntryException : Exception
{
    /// <summary>
    /// Creates a new DuplicateEntryException.
    /// </summary>
    /// <param name="parameterName">Name of the parameter that would cause a duplicate entry.</param>
    public DuplicateEntryException(string parameterName) : base(
        $"Value of {parameterName} would result in a duplicate entry."
    )
    {
    }
}