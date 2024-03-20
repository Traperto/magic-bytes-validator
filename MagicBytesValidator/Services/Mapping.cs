namespace MagicBytesValidator.Services;

/// <inheritdoc />
public class Mapping : IMapping
{
    /// <inheritdoc />
    public IReadOnlyList<IFileType> FileTypes => _fileTypes;

    private readonly List<IFileType> _fileTypes;

    public Mapping()
    {
        var currentAssembly = typeof(Mapping).GetTypeInfo().Assembly;
        _fileTypes = FileTypeCollector.CollectFileTypesForAssembly(currentAssembly).ToList();
    }

    /// <inheritdoc />
    public IFileType? FindByMimeType(string mimeType)
    {
        if (string.IsNullOrEmpty(mimeType))
        {
            throw new ArgumentEmptyException(nameof(mimeType));
        }

        return _fileTypes
            .FirstOrDefault(f => f.MimeTypes.Any(fm => fm
                .Equals(mimeType, StringComparison.InvariantCultureIgnoreCase)));
    }

    /// <inheritdoc />
    public IFileType? FindByExtension(string extension)
    {
        if (string.IsNullOrEmpty(extension))
        {
            throw new ArgumentEmptyException(nameof(extension));
        }

        return _fileTypes.FirstOrDefault(
            f => f.Extensions.Any(fe => string.Equals(fe, extension,
                StringComparison.InvariantCultureIgnoreCase))
        );
    }

    /// <inheritdoc />
    public void Register(IFileType fileType)
    {
        _fileTypes.Add(fileType);
    }

    public void Register(IEnumerable<IFileType> fileTypes)
    {
        _fileTypes.AddRange(fileTypes);
    }

    public void Register(Assembly assembly)
    {
        _fileTypes.AddRange(FileTypeCollector.CollectFileTypesForAssembly(assembly));
    }
}