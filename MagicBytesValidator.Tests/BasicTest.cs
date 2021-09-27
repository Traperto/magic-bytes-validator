using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MagicBytesValidator.Services;
using Xunit;

namespace MagicBytesValidator.Tests
{
    public class BasicTest
    {
        [Fact]
        public async Task Should_validate_correctly()
        {
            // Arrange
            var validator = new Validator();

            var fileTypeGif = validator.Mapping.FindByExtension("gif");
            var fileTypePng = validator.Mapping.FindByMimeType("image/png");

            var gifStream = new MemoryStream();
            await gifStream.WriteAsync(new byte[] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61, 0x01, 0x02, 0x03 });

            fileTypeGif.Should().NotBeNull();
            fileTypePng.Should().NotBeNull();

            // Act
            var isValidGif = await validator.IsValidAsync(gifStream, fileTypeGif!, CancellationToken.None);
            var isValidPng = await validator.IsValidAsync(gifStream, fileTypePng!, CancellationToken.None);

            // Assert
            isValidGif.Should().BeTrue("stream contains gif magic-byte sequence");
            isValidPng.Should().BeFalse("stream contains gif magic-byte sequence");
        }
    }
}