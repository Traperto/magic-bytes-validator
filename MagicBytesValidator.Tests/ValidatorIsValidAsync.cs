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

public class ValidatorIsValidAsync
{
    private readonly Validator _validator;
    private readonly IFileType _fileTypeGif;
    private readonly IFileType _fileTypePng;
    private readonly MemoryStream _gifMemoryStream;
    private readonly MemoryStream _pngMemoryStream;

    public ValidatorIsValidAsync()
    {
        var gif = new Gif();
        var png = new Png();
        _gifMemoryStream = new MemoryStream(gif.MagicByteSequences[0]);
        _pngMemoryStream = new MemoryStream(png.MagicByteSequences[0]);

        _validator = new Validator();
        _fileTypeGif = _validator.Mapping.FindByExtension(gif.Extensions[0]) ?? throw new NullReferenceException();
        _fileTypePng = _validator.Mapping.FindByMimeType(png.MimeTypes.First()) ?? throw new NullReferenceException();
    }

    [Fact]
    public async Task Should_validate_gif()
    {
        // Act
        var valid = await _validator.IsValidAsync(_gifMemoryStream, _fileTypeGif, CancellationToken.None);

        // Assert
        valid.Should().BeTrue();
    }

    [Fact]
    public async Task Should_fail_incorrect_extension()
    {
        // Act
        var invalidExtension = await _validator.IsValidAsync(_gifMemoryStream, _fileTypePng, CancellationToken.None);

        // Assert
        invalidExtension.Should().BeFalse();
    }

    [Fact]
    public async Task Should_fail_incorrect_mimetype()
    {
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
        // Act
        var invalidMagicByte = await _validator.IsValidAsync(_pngMemoryStream, _fileTypeGif, CancellationToken.None);

        // Assert
        invalidMagicByte.Should().BeFalse();
    }
}