namespace MagicBytesValidator.Tests;

/// <summary>
/// Builds minimal but structurally valid compound files (OLE2 / CFBF, version 3) as used by legacy Office formats.
/// Only the directory is populated (all streams are empty), which is all the format detection looks at.
/// Directory sectors are deliberately chained in reverse order to make sure readers follow the FAT.
/// </summary>
internal static class CompoundFileBuilder
{
   private const int SectorSize = 512;
   private const int EntrySize = 128;
   private const int EntriesPerSector = SectorSize / EntrySize;

   private const uint FreeSector = 0xFFFFFFFF;
   private const uint EndOfChain = 0xFFFFFFFE;
   private const uint FatSector = 0xFFFFFFFD;
   private const uint NoStream = 0xFFFFFFFF;

   private const byte StorageObject = 0x01;
   private const byte StreamObject = 0x02;
   private const byte RootStorageObject = 0x05;

   public static readonly byte[] Signature = [0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1];

   private sealed record Entry(string Name, byte Type, List<Entry> Children);

   /// <param name="rootStreams">Streams that are direct children of the root storage.</param>
   /// <param name="nestedStorages">Sub-storages (e.g. embedded objects) with the streams they contain.</param>
   public static byte[] Build(string[] rootStreams, params (string Storage, string[] Streams)[] nestedStorages)
   {
      var root = new Entry("Root Entry", RootStorageObject, []);
      root.Children.AddRange(rootStreams.Select(name => new Entry(name, StreamObject, [])));
      root.Children.AddRange(nestedStorages.Select(storage => new Entry(
         storage.Storage,
         StorageObject,
         storage.Streams.Select(name => new Entry(name, StreamObject, [])).ToList()
      )));

      var entries = new List<Entry>();
      Flatten(root, entries);

      var directorySectorCount = (entries.Count + EntriesPerSector - 1) / EntriesPerSector;
      var file = new byte[SectorSize * (2 + directorySectorCount)];

      WriteHeader(file, directorySectorCount);
      WriteFat(file, directorySectorCount);

      for (var index = 0; index < entries.Count; index++)
      {
         WriteEntry(file, DirectoryEntryOffset(index, directorySectorCount), entries[index], entries);
      }

      return file;
   }

   public static MemoryStream BuildStream(string[] rootStreams, params (string Storage, string[] Streams)[] nestedStorages)
   {
      return new MemoryStream(Build(rootStreams, nestedStorages));
   }

   private static void Flatten(Entry entry, List<Entry> entries)
   {
      entries.Add(entry);
      foreach (var child in entry.Children)
      {
         Flatten(child, entries);
      }
   }

   private static void WriteHeader(byte[] file, int directorySectorCount)
   {
      Signature.CopyTo(file, 0);
      WriteUInt16(file, 0x18, 0x003E); // minor version
      WriteUInt16(file, 0x1A, 0x0003); // major version
      WriteUInt16(file, 0x1C, 0xFFFE); // byte order
      WriteUInt16(file, 0x1E, 9); // sector shift (512 bytes)
      WriteUInt16(file, 0x20, 6); // mini sector shift
      WriteUInt32(file, 0x2C, 1); // number of FAT sectors
      WriteUInt32(file, 0x30, (uint)directorySectorCount); // first directory sector (chain runs backwards)
      WriteUInt32(file, 0x38, 4096); // mini stream cutoff
      WriteUInt32(file, 0x3C, EndOfChain); // first mini FAT sector
      WriteUInt32(file, 0x44, EndOfChain); // first DIFAT sector

      WriteUInt32(file, 0x4C, 0); // DIFAT[0]: FAT lives in sector 0
      for (var index = 1; index < 109; index++)
      {
         WriteUInt32(file, 0x4C + (index * 4), FreeSector);
      }
   }

   private static void WriteFat(byte[] file, int directorySectorCount)
   {
      const int fatOffset = SectorSize;

      for (var sector = 0; sector < SectorSize / 4; sector++)
      {
         WriteUInt32(file, fatOffset + (sector * 4), FreeSector);
      }

      WriteUInt32(file, fatOffset, FatSector);

      // Directory sectors 1..n are chained n -> n-1 -> ... -> 1.
      for (var sector = directorySectorCount; sector >= 1; sector--)
      {
         WriteUInt32(file, fatOffset + (sector * 4), sector == 1 ? EndOfChain : (uint)(sector - 1));
      }
   }

   private static int DirectoryEntryOffset(int entryIndex, int directorySectorCount)
   {
      var sector = directorySectorCount - (entryIndex / EntriesPerSector);
      return ((sector + 1) * SectorSize) + ((entryIndex % EntriesPerSector) * EntrySize);
   }

   private static void WriteEntry(byte[] file, int offset, Entry entry, List<Entry> entries)
   {
      var nameBytes = System.Text.Encoding.Unicode.GetBytes(entry.Name);
      nameBytes.CopyTo(file, offset);
      WriteUInt16(file, offset + 0x40, (ushort)(nameBytes.Length + 2));
      file[offset + 0x42] = entry.Type;
      file[offset + 0x43] = 0x01; // black

      // Siblings are linked as a degenerate tree via the right sibling id, which is still a valid tree.
      var parent = entries.FirstOrDefault(candidate => candidate.Children.Contains(entry));
      var siblingIndex = parent?.Children.IndexOf(entry) ?? -1;
      var rightSibling = parent is not null && siblingIndex + 1 < parent.Children.Count
         ? (uint)entries.IndexOf(parent.Children[siblingIndex + 1])
         : NoStream;

      WriteUInt32(file, offset + 0x44, NoStream);
      WriteUInt32(file, offset + 0x48, rightSibling);
      WriteUInt32(file, offset + 0x4C, entry.Children.Count > 0 ? (uint)entries.IndexOf(entry.Children[0]) : NoStream);
      WriteUInt32(file, offset + 0x74, EndOfChain); // start sector (empty stream)
   }

   private static void WriteUInt16(byte[] file, int offset, ushort value)
   {
      BinaryPrimitives.WriteUInt16LittleEndian(file.AsSpan(offset), value);
   }

   private static void WriteUInt32(byte[] file, int offset, uint value)
   {
      BinaryPrimitives.WriteUInt32LittleEndian(file.AsSpan(offset), value);
   }
}
