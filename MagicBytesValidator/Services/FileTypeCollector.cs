namespace MagicBytesValidator.Services;

public static class FileTypeCollector
{
    [Obsolete("Use CollectFileTypesForAssembly instead.")]
    public static IEnumerable<IFileType> CollectFileTypes(Assembly? assembly = null)
    {
        assembly ??= typeof(Mapping).GetTypeInfo().Assembly;
        return CollectFileTypesForAssembly(assembly);
    }

    public static IEnumerable<IFileType> CollectFileTypesForAssembly(Assembly assembly)
    {
        if (assembly is null)
        {
            throw new ArgumentEmptyException(nameof(assembly));
        }

        return assembly.GetTypes()
            .Where(t => typeof(IFileType).IsAssignableFrom(t))
            .Where(t => !t.GetTypeInfo().IsAbstract)
            .Where(t => t.GetConstructors().Any(c => c.GetParameters().Length == 0))
            .Select(Activator.CreateInstance)
            .OfType<IFileType>()
            .ToList();
    }
}