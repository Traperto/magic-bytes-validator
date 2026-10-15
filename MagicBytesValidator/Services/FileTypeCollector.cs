namespace MagicBytesValidator.Services;

public static class FileTypeCollector
{
    /// <summary>
    /// Creates an instance of every non-abstract <see cref="IFileType"/> with a parameterless constructor
    /// in the given assembly.
    /// </summary>
    public static IEnumerable<IFileType> CollectFileTypesForAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        return assembly.GetTypes()
            .Where(t => typeof(IFileType).IsAssignableFrom(t))
            .Where(t => !t.GetTypeInfo().IsAbstract)
            .Where(t => t.GetConstructors().Any(c => c.GetParameters().Length == 0))
            .Select(Activator.CreateInstance)
            .OfType<IFileType>()
            .ToList();
    }
}