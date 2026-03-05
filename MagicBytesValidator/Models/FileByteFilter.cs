namespace MagicBytesValidator.Models;

public abstract class FileByteFilter : IFileType
{
   private readonly FileByteCheck _baseFileByteChecks = new();
   private readonly FileByteCheck _strictFileByteChecks = new();
   private readonly FileByteCheck _lazyFileByteChecks = new();

   public string[] MimeTypes { get; }
   public string[] Extensions { get; }

   protected FileByteFilter(
      string[] mimeTypes,
      string[] extensions)
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
      public readonly int Offset = offset;
      public readonly byte?[] ByteArray = bytesToCheck;
   }

   private sealed class TailContainsCheck(int lastNBytes, byte?[] pattern)
   {
      public int LastNBytes { get; } = lastNBytes;
      public byte?[] Pattern { get; } = pattern;
   }

   private sealed class FileByteCheck
   {
      public List<ByteCheck> Needed { get; } = [];
      public List<ByteCheck[]> AnyOf { get; } = [];
      public List<byte?[]> Anywhere { get; } = [];
      public List<TailContainsCheck> TailContains { get; } = [];

      /* A file matches only if:
          - every Needed check matches at its fixed offset,
          - for each AnyOf-group at least one alternative matches,
          - every Anywhere-pattern occurs somewhere in the stream (null bytes act as wildcards),
          - every TailContains check finds its pattern within the last bytes. */
      public bool Matches(byte[] fileByteStream)
      {
         return Needed.All(check => CheckBytes(check, fileByteStream))
                && AnyOf.All(group => group.Any(check => CheckBytes(check, fileByteStream)))
                && Anywhere.All(pattern => ContainsPatternAnywhere(pattern, fileByteStream))
                && TailContains.All(check => CheckTailContains(check, fileByteStream));
      }
   }

   public bool Matches(
      byte[] fileByteStream,
      FileByteType type = FileByteType.Strict)
   {
      ArgumentNullException.ThrowIfNull(fileByteStream);

      // Basic rules must always match; then type-specific rules.
      return _baseFileByteChecks.Matches(fileByteStream)
             && GetChecksByType(type).Matches(fileByteStream);
   }

   public FileByteFilter StartsWith(
      byte?[] bytesToCheck,
      FileByteType? type = null)
   {
      ArgumentNullException.ThrowIfNull(bytesToCheck);

      GetChecksByType(type).Needed.Add(new ByteCheck(0, bytesToCheck));
      return this;
   }

   public FileByteFilter StartsWithAnyOf(
      byte?[][] bytesToCheck,
      FileByteType? type = null)
   {
      ArgumentNullException.ThrowIfNull(bytesToCheck);

      GetChecksByType(type)
         .AnyOf
         .Add(bytesToCheck.Select(byteArray => new ByteCheck(0, byteArray)).ToArray());

      return this;
   }

   public FileByteFilter EndsWith(
      byte?[] bytesToCheck,
      FileByteType? type = null)
   {
      ArgumentNullException.ThrowIfNull(bytesToCheck);

      GetChecksByType(type).Needed.Add(new ByteCheck(-bytesToCheck.Length, bytesToCheck));
      return this;
   }

   public FileByteFilter EndsWithAnyOf(
      byte?[][] bytesToCheck,
      FileByteType? type = null)
   {
      ArgumentNullException.ThrowIfNull(bytesToCheck);

      GetChecksByType(type)
         .AnyOf
         .Add(bytesToCheck.Select(byteArray => new ByteCheck(-byteArray.Length, byteArray)).ToArray());

      return this;
   }

   public FileByteFilter Anywhere(
      byte?[] bytesToCheck,
      FileByteType? type = null)
   {
      ArgumentNullException.ThrowIfNull(bytesToCheck);

      GetChecksByType(type).Anywhere.Add(bytesToCheck);
      return this;
   }

   public FileByteFilter Anywhere(
      byte?[][] bytesToCheck,
      FileByteType? type = null)
   {
      ArgumentNullException.ThrowIfNull(bytesToCheck);

      foreach (var byteArrayToCheck in bytesToCheck)
      {
         Anywhere(byteArrayToCheck, type);
      }

      return this;
   }

   public FileByteFilter Specific(
      ByteCheck bytesToCheck,
      FileByteType? type = null)
   {
      ArgumentNullException.ThrowIfNull(bytesToCheck);

      GetChecksByType(type).Needed.Add(bytesToCheck);
      return this;
   }

   public FileByteFilter SpecificAnyOf(
      ByteCheck[] bytesToCheck,
      FileByteType? type = null)
   {
      ArgumentNullException.ThrowIfNull(bytesToCheck);

      GetChecksByType(type).AnyOf.Add(bytesToCheck.ToArray());
      return this;
   }

   public FileByteFilter TailContains(
      int lastNBytes,
      byte?[] bytesToCheck,
      FileByteType? type = null)
   {
      ArgumentNullException.ThrowIfNull(bytesToCheck);

      GetChecksByType(type).TailContains.Add(new TailContainsCheck(lastNBytes, bytesToCheck));
      return this;
   }

   private FileByteCheck GetChecksByType(FileByteType? type)
   {
      return type switch
      {
         FileByteType.Strict => _strictFileByteChecks,
         FileByteType.Lazy => _lazyFileByteChecks,
         _ => _baseFileByteChecks
      };
   }

   private static bool CheckBytes(ByteCheck byteToCheck, byte[] fileStreamToCheck)
   {
      var offset = byteToCheck.Offset >= 0
         ? byteToCheck.Offset
         : fileStreamToCheck.Length - byteToCheck.ByteArray.Length;

      if (offset < 0 || fileStreamToCheck.Length - offset < byteToCheck.ByteArray.Length)
      {
         return false;
      }

      for (var index = 0; index < byteToCheck.ByteArray.Length; index++)
      {
         var expected = byteToCheck.ByteArray[index];

         if (expected.HasValue && fileStreamToCheck[offset + index] != expected.Value)
         {
            return false;
         }
      }

      return true;
   }

   private static bool ContainsPatternAnywhere(
      byte?[] pattern,
      byte[] fileStreamToCheck)
   {
      if (pattern.Length == 0)
      {
         return true;
      }

      if (fileStreamToCheck.Length < pattern.Length)
      {
         return false;
      }

      for (var offset = 0; offset <= fileStreamToCheck.Length - pattern.Length; offset++)
      {
         if (MatchesPatternAt(fileStreamToCheck, offset, pattern))
         {
            return true;
         }
      }

      return false;
   }

   private static bool MatchesPatternAt(
      byte[] fileStreamToCheck,
      int offset,
      byte?[] pattern)
   {
      for (var index = 0; index < pattern.Length; index++)
      {
         var expectedByte = pattern[index];

         if (expectedByte.HasValue && fileStreamToCheck[offset + index] != expectedByte.Value)
         {
            return false;
         }
      }

      return true;
   }

   private static bool CheckTailContains(
      TailContainsCheck check,
      byte[] fileStreamToCheck)
   {
      var pattern = check.Pattern;
      var start = Math.Max(0, fileStreamToCheck.Length - check.LastNBytes);
      var tailLength = fileStreamToCheck.Length - start;

      if (tailLength < pattern.Length)
      {
         return false;
      }

      for (var offset = start; offset <= fileStreamToCheck.Length - pattern.Length; offset++)
      {
         if (MatchesPatternAt(fileStreamToCheck, offset, pattern))
         {
            return true;
         }
      }

      return false;
   }
}
