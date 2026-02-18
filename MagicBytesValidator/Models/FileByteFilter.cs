namespace MagicBytesValidator.Models;

// TODO: currently Anywhere() doesnt support null bytes in Array

public abstract class FileByteFilter : IFileType
{
    private readonly List<ByteCheck> _neededByteChecks = [];
    private readonly List<ByteCheck[]> _oneOfEachByteChecks = [];
    private readonly List<byte?[]> _anywhereByteChecks = [];
    private readonly List<TailContainsCheck> _tailContainsChecks = [];

    private readonly List<ByteCheck> _neededByteChecksStrict = [];
    private readonly List<ByteCheck[]> _oneOfEachByteChecksStrict = [];
    private readonly List<byte?[]> _anywhereByteChecksStrict = [];
    private readonly List<TailContainsCheck> _tailContainsChecksStrict = [];

    private readonly List<ByteCheck> _neededByteChecksDefault = [];
    private readonly List<ByteCheck[]> _oneOfEachByteChecksDefault = [];
    private readonly List<byte?[]> _anywhereByteChecksDefault = [];
    private readonly List<TailContainsCheck> _tailContainsChecksDefault = [];

    private sealed record TailContainsCheck(int LastNBytes, byte?[] Pattern);

    private readonly FileByteType _defaultType;

    public string[] MimeTypes { get; }
    public string[] Extensions { get; }

    protected FileByteFilter(string[] mimeTypes, string[] extensions, FileByteType type = FileByteType.Strict)
    {
        if (!mimeTypes.Any() || mimeTypes.Any(string.IsNullOrEmpty))
        {
            throw new ArgumentEmptyException($"{nameof(mimeTypes)} cannot be null or empty");
        }

        if (!extensions.Any() || extensions.Any(string.IsNullOrEmpty))
        {
            throw new ArgumentEmptyException($"{nameof(extensions)} cannot be null or empty");
        }

        MimeTypes = mimeTypes;
        Extensions = extensions;
        _defaultType = type;
    }

    public class ByteCheck(int offset, byte?[] bytesToCheck)
    {
        public int Offset = offset;
        public readonly byte?[] ByteArray = bytesToCheck;
    }

    public bool Matches(byte[] fileByteStream, FileByteType type = FileByteType.Lazy)
    {
        foreach (var neededByteCheck in _neededByteChecks)
        {
            if (!CheckBytes(neededByteCheck, fileByteStream))
            {
                return false;
            }
        }

        foreach (var oneOf in _oneOfEachByteChecks)
        {
            if (!oneOf.Any(byteToCheck => CheckBytes(byteToCheck, fileByteStream)))
            {
                return false;
            }
        }

        foreach (var byteCheckWithoutOffset in _anywhereByteChecks)
        {
            var found = false;

            for (var index = 1; index <= fileByteStream.Length; index++)
            {
                if (byteCheckWithoutOffset.Cast<byte>()
                    .SequenceEqual(fileByteStream.Skip(index).Take(byteCheckWithoutOffset.Length)))
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                return false;
            }
        }

        foreach (var tc in _tailContainsChecks)
        {
            if (!CheckTailContains(tc, fileByteStream))
            {
                return false;
            }
        }

        if (type == FileByteType.Strict)
        {
            foreach (var neededByteCheck in _neededByteChecksStrict)
            {
                if (!CheckBytes(neededByteCheck, fileByteStream))
                {
                    return false;
                }
            }

            foreach (var oneOf in _oneOfEachByteChecksStrict)
            {
                if (!oneOf.Any(byteToCheck => CheckBytes(byteToCheck, fileByteStream)))
                {
                    return false;
                }
            }

            foreach (var byteCheckWithoutOffset in _anywhereByteChecksStrict)
            {
                var found = false;

                for (var index = 1; index <= fileByteStream.Length; index++)
                {
                    if (byteCheckWithoutOffset.Cast<byte>()
                        .SequenceEqual(fileByteStream.Skip(index).Take(byteCheckWithoutOffset.Length)))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    return false;
                }
            }

            foreach (var tc in _tailContainsChecksStrict)
            {
                if (!CheckTailContains(tc, fileByteStream))
                {
                    return false;
                }
            }

            return true;
        }

        if (type == FileByteType.Lazy)
        {
            foreach (var neededByteCheck in _neededByteChecksDefault)
            {
                if (!CheckBytes(neededByteCheck, fileByteStream))
                {
                    return false;
                }
            }

            foreach (var oneOf in _oneOfEachByteChecksDefault)
            {
                if (!oneOf.Any(byteToCheck => CheckBytes(byteToCheck, fileByteStream)))
                {
                    return false;
                }
            }

            foreach (var byteCheckWithoutOffset in _anywhereByteChecksDefault)
            {
                var found = false;

                for (var index = 1; index <= fileByteStream.Length; index++)
                {
                    if (byteCheckWithoutOffset.Cast<byte>()
                        .SequenceEqual(fileByteStream.Skip(index).Take(byteCheckWithoutOffset.Length)))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    return false;
                }
            }

            foreach (var tc in _tailContainsChecksDefault)
            {
                if (!CheckTailContains(tc, fileByteStream))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private List<ByteCheck> SelectNeededChecks(FileByteType? type)
    {
        if (type == FileByteType.Strict)
        {
            return _neededByteChecksStrict;
        }

        if (type == FileByteType.Lazy)
        {
            return _neededByteChecksDefault;
        }

        return _neededByteChecks;
    }

    private List<ByteCheck[]> SelectOneOfEachChecks(FileByteType? type)
    {
        if (type == FileByteType.Strict)
        {
            return _oneOfEachByteChecksStrict;
        }

        if (type == FileByteType.Lazy)
        {
            return _oneOfEachByteChecksDefault;
        }

        return _oneOfEachByteChecks;
    }

    private List<byte?[]> SelectAnywhereChecks(FileByteType? type)
    {
        if (type == FileByteType.Strict)
        {
            return _anywhereByteChecksStrict;
        }

        if (type == FileByteType.Lazy)
        {
            return _anywhereByteChecksDefault;
        }

        return _anywhereByteChecks;
    }

    private List<TailContainsCheck> SelectTailContainsChecks(FileByteType? type)
    {
        if (type == FileByteType.Strict)
        {
            return _tailContainsChecksStrict;
        }

        if (type == FileByteType.Lazy)
        {
            return _tailContainsChecksDefault;
        }

        return _tailContainsChecks;
    }

    public FileByteFilter StartsWith(byte?[] bytesToCheck, FileByteType? type = null)
    {
        SelectNeededChecks(type).Add(new ByteCheck(0, bytesToCheck));
        return this;
    }

    public FileByteFilter StartsWithAnyOf(byte?[][] bytesToCheck, FileByteType? type = null)
    {
        SelectOneOfEachChecks(type).Add(bytesToCheck.Select(byteArray => new ByteCheck(0, byteArray)).ToArray());
        return this;
    }

    public FileByteFilter EndsWith(byte?[] bytesToCheck, FileByteType? type = null)
    {
        SelectNeededChecks(type).Add(new ByteCheck(-bytesToCheck.Length, bytesToCheck));
        return this;
    }

    public FileByteFilter EndsWithAnyOf(byte?[][] bytesToCheck, FileByteType? type = null)
    {
        SelectOneOfEachChecks(type).Add(bytesToCheck.Select(byteArray => new ByteCheck(-byteArray.Length, byteArray)).ToArray());
        return this;
    }

    public FileByteFilter Anywhere(byte?[] bytesToCheck, FileByteType? type = null)
    {
        SelectAnywhereChecks(type).Add(bytesToCheck);
        return this;
    }

    public FileByteFilter Anywhere(byte?[][] bytesToCheck, FileByteType? type = null)
    {
        foreach (var byteArrayToCheck in bytesToCheck)
        {
            Anywhere(byteArrayToCheck, type);
        }

        return this;
    }

    public FileByteFilter Specific(ByteCheck bytesToCheck, FileByteType? type = null)
    {
        SelectNeededChecks(type).Add(bytesToCheck);
        return this;
    }

    public FileByteFilter SpecificAnyOf(ByteCheck[] bytesToCheck, FileByteType? type = null)
    {
        SelectOneOfEachChecks(type).Add(bytesToCheck.Select(byteCheck => byteCheck).ToArray());
        return this;
    }

    public FileByteFilter TailContains(int lastNBytes, byte?[] bytesToCheck, FileByteType? type = null)
    {
        SelectTailContainsChecks(type).Add(new TailContainsCheck(lastNBytes, bytesToCheck));
        return this;
    }

    private bool CheckBytes(ByteCheck byteToCheck, byte[] fileStreamToCheck)
    {
        // Check ending of file stream
        // since in the current format we have the fileStream Length only here calculate the offset
        if (byteToCheck.Offset < 0)
        {
            byteToCheck.Offset = fileStreamToCheck.Length - byteToCheck.ByteArray.Length;
        }

        if (fileStreamToCheck.Length - Math.Abs(byteToCheck.Offset) < byteToCheck.ByteArray.Length)
        {
            return false;
        }

        foreach (var (sequenceByte, index) in byteToCheck.ByteArray.AsIndexed())
        {
            if (sequenceByte == null)
            {
                continue;
            }

            if (sequenceByte != fileStreamToCheck[byteToCheck.Offset + index])
            {
                return false;
            }
        }

        return true;
    }

    private static bool CheckTailContains(TailContainsCheck check, byte[] fileStreamToCheck)
    {
        var pattern = check.Pattern;

        if (pattern.Length == 0)
        {
            return true;
        }

        var start = Math.Max(0, fileStreamToCheck.Length - check.LastNBytes);
        var tailLength = fileStreamToCheck.Length - start;

        if (tailLength < pattern.Length)
        {
            return false;
        }

        for (var tailOffset = 0; tailOffset <= tailLength - pattern.Length; tailOffset++)
        {
            var ok = true;

            for (var patternIndex = 0; patternIndex < pattern.Length; patternIndex++)
            {
                var b = pattern[patternIndex];

                if (b.HasValue && fileStreamToCheck[start + tailOffset + patternIndex] != b.Value)
                {
                    ok = false;
                    break;
                }
            }

            if (ok)
            {
                return true;
            }
        }

        return false;
    }
}
