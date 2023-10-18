using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MagicBytesValidator.Formats;
using MagicBytesValidator.Models;
using MagicBytesValidator.Services;
using Xunit;

namespace MagicBytesValidator.Tests;

public class IsValidTests
{
    private readonly Validator _validator;
    private readonly FileType _fileTypeGif;
    private readonly FileType _fileTypePng;
    private readonly FileType _fileTypeZip;
    private readonly MemoryStream _gifMemoryStream;
    private readonly MemoryStream _zipMemoryStream;
    private readonly Gif _gif;
    private readonly Png _png;
    private readonly Zip _zip;

    public IsValidTests()
    {
        _gif = new Gif();
        _png = new Png();
        _zip = new Zip();
        _gifMemoryStream = new MemoryStream();
        _zipMemoryStream = new MemoryStream();

        _validator = new Validator();
        _fileTypeGif = _validator.Mapping.FindByExtension(_gif.Extensions[0]) ?? throw new NullReferenceException();
        _fileTypePng = _validator.Mapping.FindByMimeType(_png.MimeTypes.First()) ?? throw new NullReferenceException();
        _fileTypeZip = _validator.Mapping.FindByMimeType(_zip.MimeTypes.First()) ?? throw new NullReferenceException();
    }

    [Fact]
    public async Task Should_validate_gif()
    {
        await _gifMemoryStream.WriteAsync(_gif.MagicByteSequences[0]);

        // Act
        var valid = await _validator.IsValidAsync(_gifMemoryStream, _fileTypeGif, CancellationToken.None);

        // Assert
        valid.Should().BeTrue();
    }

    [Fact]
    public async Task Should_validate_zip()
    {
        await _zipMemoryStream.WriteAsync(_zip.MagicByteSequences[0]);

        // Act
        var valid = await _validator.IsValidAsync(_zipMemoryStream, _fileTypeZip, CancellationToken.None);

        // Assert
        valid.Should().BeTrue();
    }

    [Fact]
    public async Task Should_fail_incorrect_extension()
    {
        await _gifMemoryStream.WriteAsync(_gif.MagicByteSequences[0]);

        // Act
        var invalidExtension = await _validator.IsValidAsync(_gifMemoryStream, _fileTypePng, CancellationToken.None);

        // Assert
        invalidExtension.Should().BeFalse();
    }

    [Fact]
    public async Task Should_fail_incorrect_mimetype()
    {
        await _gifMemoryStream.WriteAsync(_gif.MagicByteSequences[0]);

        // Act
        var inValidMimeType = await _validator.IsValidAsync(_gifMemoryStream, _fileTypePng, CancellationToken.None);

        // Assert
        inValidMimeType.Should().BeFalse();
    }

    [Fact]
    public async Task Should_work_with_offset()
    {
        var stream = new MemoryStream();
        await stream.WriteAsync(new byte[] { 0, 0, 0, 32, 102, 116, 121, 112, 109, 112, 52, 50, 0, 0, 0, 0, 109 });

        // Act
        var isValidMimeType = await _validator.IsValidAsync(stream, new Mp4(), CancellationToken.None);

        // Assert
        isValidMimeType.Should().BeTrue();
    }

    [Fact]
    public async Task Should_fail_incorrect_magicByte_sequence()
    {
        await _gifMemoryStream.WriteAsync(_png.MagicByteSequences[0]);

        // Act
        var invalidMagicByte = await _validator.IsValidAsync(_gifMemoryStream, _fileTypeGif, CancellationToken.None);

        // Assert
        invalidMagicByte.Should().BeFalse();
    }
}