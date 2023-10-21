using System.Linq;
using MagicBytesValidator.Models;

namespace MagicBytesValidator.Formats;

/// <see href="https://www.garykessler.net/library/file_sigs.html"/>
/// <see href="https://en.wikipedia.org/wiki/List_of_file_signatures"/>
public class Zip : IFileType
{
    private static readonly byte[][] ZipMagicByteSequences =
    {
        new byte[] { 0x50, 0x4B, 0x03, 0x04 },
        new byte[] { 0x50, 0x4B, 0x05, 0x06 },
        new byte[] { 0x50, 0x4B, 0x07, 0x08 }
    };

    public string[] MimeTypes { get; }
    public string[] Extensions { get; }

    public Zip()
    {
        MimeTypes = new[] { "application/zip", "application/x-zip-compressed" };
        Extensions = new[] { "zip" };
    }

    public Zip(string[] mimeTypes, string[] extensions)
    {
        MimeTypes = mimeTypes;
        Extensions = extensions;
    }

    public bool Matches(byte[] bytes)
    {
        // Trailer:  0x50, 0x4B (17 characters), 0x00, 0x00, 0x00
        if (!bytes.TakeLast(22).Take(2).SequenceEqual(new byte[] { 0x50, 0x4B }))
        {
            return false;
        }

        if (!bytes.TakeLast(3).SequenceEqual(new byte[] { 0x00, 0x00, 0x00 }))
        {
            return false;
        }

        return ZipMagicByteSequences.Any(mb =>
            mb.SequenceEqual(bytes.Take(mb.Length))
        );
    }
}