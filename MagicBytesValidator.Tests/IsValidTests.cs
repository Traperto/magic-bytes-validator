using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MagicBytesValidator.Formats;
using MagicBytesValidator.Models;
using MagicBytesValidator.Services;
using Xunit;

namespace MagicBytesValidator.Tests
{
    public class IsValidTests
    {
        private readonly Validator _validator;
        private readonly FileType _fileTypeGif;
        private readonly FileType _fileTypePng;
        private readonly MemoryStream _gifMemoryStream;
        private readonly Gif _gif;
        private readonly Png _png;

        public IsValidTests()
        {
            _gif = new Gif();
            _png = new Png();
            _gifMemoryStream = new MemoryStream();

            _validator = new Validator();
            _fileTypeGif = _validator.Mapping.FindByExtension(_gif.Extensions.First());
            _fileTypePng = _validator.Mapping.FindByMimeType(_png.MimeType);
        }

        [Fact]
        public async Task Should_validate()
        {
            await _gifMemoryStream.WriteAsync(_gif.MagicByteSequences.First());

            // Act
            var valid = await _validator.IsValidAsync(_gifMemoryStream, _fileTypeGif, CancellationToken.None);

            // Assert
            valid.Should().BeTrue();
        }

        [Fact]
        public async Task Should_fail_incorrect_extension()
        {
            await _gifMemoryStream.WriteAsync(_gif.MagicByteSequences.First());

            // Act
            var invalidExtension = await _validator.IsValidAsync(_gifMemoryStream, _fileTypePng, CancellationToken.None);

            // Assert
            invalidExtension.Should().BeFalse();
        }

        [Fact]
        public async Task Should_fail_incorrect_mimetype()
        {
            await _gifMemoryStream.WriteAsync(_gif.MagicByteSequences.First());

            // Act
            var inValidMimeType = await _validator.IsValidAsync(_gifMemoryStream, _fileTypePng, CancellationToken.None);

            // Assert
            inValidMimeType.Should().BeFalse();
        }

        [Fact]
        public async Task Should_work_with_offset()
        {
            var stream = new MemoryStream();
            await stream.WriteAsync(new byte[] { 0,0,0,32,102,116,121,112,109,112,52,50,0,0,0,0,109} );
            

            // Act
            var isValidMimeType = await _validator.IsValidAsync(stream, new Mp4(), CancellationToken.None);

            // Assert
            isValidMimeType.Should().BeTrue();
        }

        [Fact]
        public async Task Should_fail_incorrect_magicByte_sequence()
        {
            await _gifMemoryStream.WriteAsync(_png.MagicByteSequences.First());

            // Act
            var invalidMagicByte = await _validator.IsValidAsync(_gifMemoryStream, _fileTypeGif, CancellationToken.None);

            // Assert
            invalidMagicByte.Should().BeFalse();
        }
    }
}