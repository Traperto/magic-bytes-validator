namespace MagicBytesValidator.Models;

// TODO: currently Anywhere() doesnt support null bytes in Array

public class FileByteFilter : IFileType
{
    private readonly List<ByteCheck> _neededByteChecks = [];
    private readonly List<ByteCheck[]> _oneOfEachByteChecks = [];
    private readonly List<byte?[]> _anywhereByteChecks = [];

    public string[] MimeTypes { get; }
    public string[] Extensions { get; }

    public FileByteFilter(string[] mimeTypes, string[] extensions)
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
    }

    public class ByteCheck(int offset, byte?[] bytesToCheck)
    {
        public int Offset = offset;
        public readonly byte?[] ByteArray = bytesToCheck;
    }

    public bool Matches(byte[] fileByteStream)
    {
        foreach (var neededByteCheck in _neededByteChecks)
        {
            if (!CheckBytes(neededByteCheck, fileByteStream))
                return false;
        }

        foreach (var oneOf in _oneOfEachByteChecks)
        {
            if (!oneOf.Any(byteToCheck => CheckBytes(byteToCheck, fileByteStream)))
            {
                return false;
            }
        }

        // Then check byteArrays without fixed offsets
        // mainly byteArrays from Anywhere()
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

        return true;
    }

    public FileByteFilter StartsWith(byte?[] bytesToCheck)
    {
        _neededByteChecks.Add(new ByteCheck(0, bytesToCheck));
        return this;
    }

    public FileByteFilter StartsWithAnyOf(byte?[][] bytesToCheck)
    {
        _oneOfEachByteChecks.Add(bytesToCheck.Select(byteArray => new ByteCheck(0, byteArray)).ToArray());
        return this;
    }

    public FileByteFilter EndsWith(byte?[] bytesToCheck)
    {
        _neededByteChecks.Add(new ByteCheck(-bytesToCheck.Length, bytesToCheck));
        return this;
    }

    public FileByteFilter EndsWithAnyOf(byte?[][] bytesToCheck)
    {
        _oneOfEachByteChecks.Add(bytesToCheck.Select(byteArray => new ByteCheck(-byteArray.Length, byteArray)).ToArray());
        return this;
    }

    public FileByteFilter Anywhere(byte?[] bytesToCheck)
    {
        _anywhereByteChecks.Add(bytesToCheck);
        return this;
    }

    public FileByteFilter Anywhere(byte?[][] bytesToCheck)
    {
        foreach (var byteArrayToCheck in bytesToCheck)
        {
            Anywhere(byteArrayToCheck);
        }

        return this;
    }

    public FileByteFilter Specific(ByteCheck bytesToCheck)
    {
        _neededByteChecks.Add(bytesToCheck);
        return this;
    }

    public FileByteFilter SpecificAnyOf(ByteCheck[] bytesToCheck)
    {
        _oneOfEachByteChecks.Add(bytesToCheck.Select(byteArray => byteArray).ToArray());
        return this;
    }

    private bool CheckBytes(ByteCheck byteToCheck, byte[] fileStreamToCheck)
    {
        // Check ending of file stream
        // since in the current format we have the fileStream Length only here calculate the offset
        if (byteToCheck.Offset < 0)
            byteToCheck.Offset = fileStreamToCheck.Length - byteToCheck.ByteArray.Length;

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
}