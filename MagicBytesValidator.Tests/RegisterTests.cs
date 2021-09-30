using FluentAssertions;
using MagicBytesValidator.Models;
using MagicBytesValidator.Services;
using Xunit;

namespace MagicBytesValidator.Tests
{
    public class RegisterTests
    {
        private readonly Mapping _mapping;
        private readonly FileType _trpFileType;

        public RegisterTests()
        {
            _mapping = new Mapping();
            _trpFileType = new FileType("traperto/trp", new[] { "trp" },
                                        new[] { new byte[] { 0x74, 0x72, 0x61, 0x70, 0x65, 0x72, 0x74, 0x6f } });
        }


        [Fact]
        public void Should_register_single_filetype()
        {
            _mapping.Register(_trpFileType);
            _mapping.FileTypes.Should().Contain(_trpFileType);
        }

        [Fact]
        public void Should_register_list_filetype()
        {
            var neonJsFileType = new FileType("traperto/niklasschmidt", new[] { "nms" },
                                              new[]
                                              {
                                                  new byte[]
                                                  {
                                                      0x6e, 0x69, 0x6b, 0x6c, 0x61, 0x73, 0x73, 0x63, 0x68, 0x6d, 0x69,
                                                      0x64, 0x74
                                                  }
                                              });

            var kryptobiFileType = new FileType("traperto/tobiasjanssen", new[] { "tjn" },
                                                new[]
                                                {
                                                    new byte[]
                                                    {
                                                        0x74, 0x6f, 0x62, 0x69, 0x61, 0x73, 0x6a, 0x61, 0x6e, 0x73,
                                                        0x73, 0x65, 0x6e
                                                    }
                                                });


            _mapping.Register(new[] { neonJsFileType, kryptobiFileType });
            _mapping.FileTypes.Should().Contain(neonJsFileType).And.Contain(kryptobiFileType);
        }

        [Fact]
        public void Should_register_assembly_fileTypes()
        {
            var assembly = typeof(AssemblyFacade).Assembly;
            _mapping.Register(assembly);

            _mapping.FileTypes.Should().Contain(f => f.MimeType == "facade/trp");
        }
    }

    public class AssemblyFacade : FileType
    {
        public AssemblyFacade() : base("facade/trp", new[] { "trp" },
                                       new[] { new byte[] { 0x74, 0x72, 0x70 } })
        {
        }
    }
}