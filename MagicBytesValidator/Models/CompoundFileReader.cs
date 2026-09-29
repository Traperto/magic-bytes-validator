using System.Buffers.Binary;

namespace MagicBytesValidator.Models;

/// <summary>
/// Minimal, read-only reader for the Compound File Binary Format (CFBF / OLE2), which is the container of legacy
/// Office files (doc, xls, ppt). All of them share the same file header, so the application can only be told apart
/// by the names of the streams stored in the root storage (e.g. "WordDocument", "Workbook", "PowerPoint Document").
/// The reader is defensive: malformed, truncated or cyclic structures result in no names instead of an exception.
/// </summary>
/// <see href="https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-cfb/53989ce4-7b05-4f8d-829b-d08d6148375b"/>
internal static class CompoundFileReader
{
   private static readonly byte[] Signature = [0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1];

   private const int HeaderSize = 512;
   private const int HeaderDifatEntries = 109;
   private const int DirectoryEntrySize = 128;

   private const uint MaxRegularSector = 0xFFFFFFFA;
   private const uint NoStream = 0xFFFFFFFF;

   private const byte StreamObject = 0x02;
   private const byte RootStorageObject = 0x05;

   /// <summary>
   /// Returns the names of all streams that are direct children of the root storage. Streams inside nested
   /// storages (e.g. embedded OLE objects) are intentionally ignored, so a Word document with an embedded
   /// Excel workbook is not reported as a workbook.
   /// </summary>
   public static IReadOnlyCollection<string> ReadRootStreamNames(byte[] file)
   {
      try
      {
         return TryReadRootStreamNames(file) ?? [];
      }
      catch (ArgumentException)
      {
         // Offsets pointing outside of the byte array: treat as malformed.
         return [];
      }
   }

   private static IReadOnlyCollection<string>? TryReadRootStreamNames(byte[] file)
   {
      if (file.Length < HeaderSize || !file.AsSpan(0, Signature.Length).SequenceEqual(Signature))
      {
         return null;
      }

      var sectorShift = ReadUInt16(file, 0x1E);
      if (sectorShift is not (9 or 12))
      {
         return null;
      }

      var sectorSize = 1 << sectorShift;
      var entriesPerSector = sectorSize / sizeof(uint);
      var maxSectorCount = (file.Length / sectorSize) + 1;

      var fatSectors = ReadFatSectors(file, sectorSize, entriesPerSector, maxSectorCount);
      if (fatSectors is null)
      {
         return null;
      }

      var directory = ReadDirectory(file, sectorSize, entriesPerSector, maxSectorCount, fatSectors);
      if (directory.Count == 0 || directory[0][0x42] != RootStorageObject)
      {
         return null;
      }

      return CollectChildStreamNames(directory, ReadUInt32(directory[0], 0x4C));
   }

   /// <summary>
   /// Collects the sector ids of the FAT from the header DIFAT and the (optional) DIFAT sector chain.
   /// </summary>
   private static List<uint>? ReadFatSectors(byte[] file, int sectorSize, int entriesPerSector, int maxSectorCount)
   {
      var fatSectorCount = ReadUInt32(file, 0x2C);
      if (fatSectorCount > maxSectorCount)
      {
         return null;
      }

      var fatSectors = new List<uint>((int)fatSectorCount);

      for (var index = 0; index < HeaderDifatEntries && fatSectors.Count < fatSectorCount; index++)
      {
         fatSectors.Add(ReadUInt32(file, 0x4C + (index * sizeof(uint))));
      }

      var difatSector = ReadUInt32(file, 0x44);
      for (var visited = 0; fatSectors.Count < fatSectorCount; visited++)
      {
         if (difatSector > MaxRegularSector || visited > maxSectorCount)
         {
            return null;
         }

         var difatOffset = SectorOffset(difatSector, sectorSize);
         if (difatOffset + sectorSize > file.Length)
         {
            return null;
         }

         // The last entry of a DIFAT sector points to the next DIFAT sector.
         for (var index = 0; index < entriesPerSector - 1 && fatSectors.Count < fatSectorCount; index++)
         {
            fatSectors.Add(ReadUInt32(file, (int)difatOffset + (index * sizeof(uint))));
         }

         difatSector = ReadUInt32(file, (int)difatOffset + ((entriesPerSector - 1) * sizeof(uint)));
      }

      return fatSectors;
   }

   private static List<byte[]> ReadDirectory(
      byte[] file,
      int sectorSize,
      int entriesPerSector,
      int maxSectorCount,
      List<uint> fatSectors)
   {
      var entries = new List<byte[]>();
      var sector = ReadUInt32(file, 0x30);

      for (var visited = 0; sector <= MaxRegularSector && visited <= maxSectorCount; visited++)
      {
         var sectorOffset = SectorOffset(sector, sectorSize);
         if (sectorOffset + sectorSize > file.Length)
         {
            // Truncated file: keep what could be read so far.
            break;
         }

         for (var entryOffset = 0; entryOffset < sectorSize; entryOffset += DirectoryEntrySize)
         {
            entries.Add(file.AsSpan((int)(sectorOffset + entryOffset), DirectoryEntrySize).ToArray());
         }

         var fatSectorIndex = sector / (uint)entriesPerSector;
         if (fatSectorIndex >= fatSectors.Count)
         {
            break;
         }

         var fatOffset = SectorOffset(fatSectors[(int)fatSectorIndex], sectorSize)
                         + ((sector % (uint)entriesPerSector) * sizeof(uint));
         if (fatOffset + sizeof(uint) > file.Length)
         {
            break;
         }

         sector = ReadUInt32(file, (int)fatOffset);
      }

      return entries;
   }

   /// <summary>
   /// Children of a storage are organized as a (red-black) tree via left/right sibling ids.
   /// </summary>
   private static HashSet<string> CollectChildStreamNames(List<byte[]> directory, uint firstChild)
   {
      var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
      var visited = new HashSet<uint>();
      var pending = new Stack<uint>();
      pending.Push(firstChild);

      while (pending.Count > 0)
      {
         var entryId = pending.Pop();
         if (entryId == NoStream || entryId >= directory.Count || !visited.Add(entryId))
         {
            continue;
         }

         var entry = directory[(int)entryId];
         pending.Push(ReadUInt32(entry, 0x44));
         pending.Push(ReadUInt32(entry, 0x48));

         var nameLength = ReadUInt16(entry, 0x40);
         if (entry[0x42] == StreamObject && nameLength is >= 2 and <= 64 && nameLength % 2 == 0)
         {
            // The name length includes the terminating null character.
            names.Add(System.Text.Encoding.Unicode.GetString(entry, 0, nameLength - 2));
         }
      }

      return names;
   }

   private static long SectorOffset(uint sector, int sectorSize)
   {
      // The header occupies the first sector (512 bytes for v3, padded to 4096 bytes for v4).
      return ((long)sector + 1) * sectorSize;
   }

   private static ushort ReadUInt16(byte[] bytes, int offset)
   {
      return BinaryPrimitives.ReadUInt16LittleEndian(bytes.AsSpan(offset));
   }

   private static uint ReadUInt32(byte[] bytes, int offset)
   {
      return BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(offset));
   }
}
