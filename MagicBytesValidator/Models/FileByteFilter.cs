namespace MagicBytesValidator.Models;

/// <summary>
/// Base class for file types that are identified by magic byte checks, configured via the fluent methods.
/// </summary>
/// <remarks>
/// A filter is frozen when it is used for the first time (see <see cref="Matches"/>) or when <see cref="Freeze"/>
/// is called. Afterwards its checks can't be changed anymore, so a filter can safely be shared between threads.
/// Configure a filter completely (e.g. in its constructor) before using it.
/// </remarks>
public abstract class FileByteFilter : IFileType
{
   private readonly FileByteCheck _baseFileByteChecks = new();
   private readonly FileByteCheck _strictFileByteChecks = new();
   private readonly FileByteCheck _lazyFileByteChecks = new();

   private volatile bool _isFrozen;

   /// <inheritdoc />
   public IReadOnlyList<string> MimeTypes { get; }

   /// <inheritdoc />
   public IReadOnlyList<string> Extensions { get; }

   /// <summary>
   /// Whether the checks of this filter can't be changed anymore.
   /// </summary>
   public bool IsFrozen => _isFrozen;

   protected FileByteFilter(
      string[] mimeTypes,
      string[] extensions)
   {
      ArgumentNullException.ThrowIfNull(mimeTypes);
      ArgumentNullException.ThrowIfNull(extensions);

      if (mimeTypes.Length == 0 || mimeTypes.Any(string.IsNullOrEmpty))
      {
         throw new ArgumentEmptyException(nameof(mimeTypes));
      }

      if (extensions.Length == 0 || extensions.Any(string.IsNullOrEmpty))
      {
         throw new ArgumentEmptyException(nameof(extensions));
      }

      MimeTypes = Array.AsReadOnly(mimeTypes.ToArray());
      Extensions = Array.AsReadOnly(extensions.ToArray());
   }

   /// <summary>
   /// Checks the given bytes at a fixed offset. A negative offset counts from the end of the file,
   /// e.g. an offset of -4 starts at the fourth to last byte. <c>null</c> bytes act as wildcards.
   /// </summary>
   public sealed class ByteCheck
   {
      /// <summary>
      /// Offset of the first byte to check. Negative values count from the end of the file.
      /// </summary>
      public int Offset { get; }

      /// <summary>
      /// Expected bytes, <c>null</c> matches any byte.
      /// </summary>
      public IReadOnlyList<byte?> Bytes { get; }

      internal byte?[] Pattern { get; }

      public ByteCheck(int offset, byte?[] bytesToCheck)
      {
         ArgumentNullException.ThrowIfNull(bytesToCheck);

         Offset = offset;
         Pattern = bytesToCheck.ToArray();
         Bytes = Array.AsReadOnly(Pattern);
      }
   }

   private sealed class TailContainsCheck(int lastNBytes, byte?[] pattern)
   {
      public int LastNBytes { get; } = lastNBytes;
      public byte?[] Pattern { get; } = pattern.ToArray();
   }

   private sealed class FileByteCheck
   {
      public List<ByteCheck> Needed { get; } = [];
      public List<ByteCheck[]> AnyOf { get; } = [];
      public List<byte?[]> Anywhere { get; } = [];
      public List<string[]> CompoundFileStreamAnyOf { get; } = [];
      public List<TailContainsCheck> TailContains { get; } = [];

      /* A file matches only if:
          - every Needed check matches at its fixed offset,
          - for each AnyOf-group at least one alternative matches,
          - every Anywhere-pattern occurs somewhere in the stream (null bytes act as wildcards),
          - for each CompoundFileStreamAnyOf-group the OLE root storage contains at least one of the named streams,
          - every TailContains check finds its pattern within the last bytes. */
      public bool Matches(byte[] fileByteStream)
      {
         return Needed.All(check => CheckBytes(check, fileByteStream))
                && AnyOf.All(group => group.Any(check => CheckBytes(check, fileByteStream)))
                && Anywhere.All(pattern => ContainsPatternAnywhere(pattern, fileByteStream))
                && CheckCompoundFileStreams(CompoundFileStreamAnyOf, fileByteStream)
                && TailContains.All(check => CheckTailContains(check, fileByteStream));
      }
   }

   /// <inheritdoc />
   /// <remarks>Freezes the filter, see <see cref="Freeze"/>.</remarks>
   public bool Matches(
      byte[] fileByteStream,
      FileByteType type = FileByteType.Strict)
   {
      ArgumentNullException.ThrowIfNull(fileByteStream);

      Freeze();

      // Basic rules must always match; then type-specific rules.
      return _baseFileByteChecks.Matches(fileByteStream)
             && GetChecksByType(type).Matches(fileByteStream);
   }

   public FileByteFilter StartsWith(
      byte?[] bytesToCheck,
      FileByteType? type = null)
   {
      ArgumentNullException.ThrowIfNull(bytesToCheck);

      ChecksToConfigure(type).Needed.Add(new ByteCheck(0, bytesToCheck));
      return this;
   }

   public FileByteFilter StartsWithAnyOf(
      byte?[][] bytesToCheck,
      FileByteType? type = null)
   {
      ArgumentNullException.ThrowIfNull(bytesToCheck);

      ChecksToConfigure(type)
         .AnyOf
         .Add(bytesToCheck.Select(byteArray => new ByteCheck(0, byteArray)).ToArray());

      return this;
   }

   public FileByteFilter EndsWith(
      byte?[] bytesToCheck,
      FileByteType? type = null)
   {
      ArgumentNullException.ThrowIfNull(bytesToCheck);

      ChecksToConfigure(type).Needed.Add(new ByteCheck(-bytesToCheck.Length, bytesToCheck));
      return this;
   }

   public FileByteFilter EndsWithAnyOf(
      byte?[][] bytesToCheck,
      FileByteType? type = null)
   {
      ArgumentNullException.ThrowIfNull(bytesToCheck);

      ChecksToConfigure(type)
         .AnyOf
         .Add(bytesToCheck.Select(byteArray => new ByteCheck(-byteArray.Length, byteArray)).ToArray());

      return this;
   }

   public FileByteFilter Anywhere(
      byte?[] bytesToCheck,
      FileByteType? type = null)
   {
      ArgumentNullException.ThrowIfNull(bytesToCheck);

      ChecksToConfigure(type).Anywhere.Add(bytesToCheck.ToArray());
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

   /// <summary>
   /// Requires the file to be a compound file (OLE2) whose root storage contains at least one stream with one of
   /// the given names (case-insensitive). Legacy Office formats share the same header and can only be told apart
   /// this way.
   /// </summary>
   public FileByteFilter CompoundFileStreamAnyOf(
      string[] streamNames,
      FileByteType? type = null)
   {
      ArgumentNullException.ThrowIfNull(streamNames);

      if (streamNames.Length == 0 || streamNames.Any(string.IsNullOrEmpty))
      {
         throw new ArgumentEmptyException(nameof(streamNames));
      }

      ChecksToConfigure(type).CompoundFileStreamAnyOf.Add(streamNames.ToArray());
      return this;
   }

   public FileByteFilter Specific(
      ByteCheck bytesToCheck,
      FileByteType? type = null)
   {
      ArgumentNullException.ThrowIfNull(bytesToCheck);

      ChecksToConfigure(type).Needed.Add(bytesToCheck);
      return this;
   }

   public FileByteFilter SpecificAnyOf(
      ByteCheck[] bytesToCheck,
      FileByteType? type = null)
   {
      ArgumentNullException.ThrowIfNull(bytesToCheck);

      ChecksToConfigure(type).AnyOf.Add(bytesToCheck.ToArray());
      return this;
   }

   public FileByteFilter TailContains(
      int lastNBytes,
      byte?[] bytesToCheck,
      FileByteType? type = null)
   {
      ArgumentNullException.ThrowIfNull(bytesToCheck);

      ChecksToConfigure(type).TailContains.Add(new TailContainsCheck(lastNBytes, bytesToCheck));
      return this;
   }

   /// <summary>
   /// Prevents any further changes to the checks of this filter. Called automatically on first use.
   /// </summary>
   public FileByteFilter Freeze()
   {
      if (!_isFrozen)
      {
         _isFrozen = true;
      }

      return this;
   }

   private FileByteCheck ChecksToConfigure(FileByteType? type)
   {
      if (_isFrozen)
      {
         throw new InvalidOperationException(
            $"{GetType().Name} is frozen and can't be changed anymore, as it has already been used.");
      }

      return GetChecksByType(type);
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
      // A negative offset counts from the end of the file (e.g. -4 starts at the fourth to last byte).
      var offset = byteToCheck.Offset >= 0
         ? byteToCheck.Offset
         : fileStreamToCheck.Length + byteToCheck.Offset;

      if (offset < 0 || fileStreamToCheck.Length - offset < byteToCheck.Pattern.Length)
      {
         return false;
      }

      for (var index = 0; index < byteToCheck.Pattern.Length; index++)
      {
         var expected = byteToCheck.Pattern[index];

         if (expected.HasValue && fileStreamToCheck[offset + index] != expected.Value)
         {
            return false;
         }
      }

      return true;
   }

   private static bool CheckCompoundFileStreams(
      List<string[]> streamNameGroups,
      byte[] fileStreamToCheck)
   {
      if (streamNameGroups.Count == 0)
      {
         return true;
      }

      var rootStreamNames = CompoundFileReader.ReadRootStreamNames(fileStreamToCheck);

      return streamNameGroups.All(group => group.Any(rootStreamNames.Contains));
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
