# MagicBytesValidator

Recognize file types from `Stream`s or `IFormFile`s using MIME types or file extensions and validate them against the magic bytes according to the file types.
The existing file types can be expanded in various ways.

### How to install

- Install nuget package into your project:

  ```powershell
  Install-Package MagicBytesValidator -Version 2.1.6
  ```

  ```bash
  dotnet add package MagicBytesValidator --version 2.1.6
  ```

- Reference in your csproj:
  ```xml
  <PackageReference Include="MagicBytesValidator" Version="2.1.6" />
  ```

### How to use

- Check if a stream content matches a file type:

  ```c#
  var validator = new MagicBytesValidator.Services.Validator();
  var pngFileType = validator.Mapping.FindByExtension("png");

  var isValidPng = await validator.IsValidAsync(memoryStream, pngFileType, CancellationToken.None);
  ```

- Determine and validate a file type by uploaded `IFormFile`:

  ```c#
  var formFileTypeProvider = new MagicBytesValidator.Services.Http.FormFileTypeProvider();

  try {
      var fileType = await formFileTypeProvider.FindValidatedTypeAsync(formFile, null, CancellationToken.None);

      if (fileType is not null) {
          // Further code
      } {
          // Can't determine type
      }
  } catch(MagicBytesValidator.Exceptions.Http.MimeTypeMismatchException) {
      // Content and given MIME type / extension don't match.
  }
  ```

- Determine the file type of a file by its stream:

  ```c#
  var streamFileTypeProvider = new MagicBytesValidator.Services.Streams.StreamFileTypeProvider();
  var fileType = await streamFileTypeProvider.TryFindUnambiguousAsync(fileStream, CancellationToken.None);

  if (fileType is not null) {
      // Further code
  } else {
      // Can't determine unambiguous type
  }
  ```

#### Add custom file types

- Register a custom type with filters:

  ```c#
  public class CustomType : MagicBytesValidator.Models.FileByteFilter
  {
      public CustomType() : base(
          ["traperto/trp"], // mime types
          ["trp"] // extensions
      )
      {
          // defined magic byte sequences
          StartsWith([
              0x78, 0x6c, 0x2f, 0x5f, 0x72, 0x65
          ])
          .EndsWith([
              0xFF, 0xFF
          ])
          .Specific(new ByteCheck(512, [0xFD])); // offset: 512 bytes, negative offset looks for a specific offset from the end of file
      }
  }

  var mapping = new MagicBytesValidator.Services.Mapping();
  mapping.Register(new CustomType());
  mapping.Register(new[] { new CustomType() }); // Add multiple types

  // Registering all `IFileType`s of the given assembly that are also not abstract and have an empty constructor.
  _mapping.Register(typeof(CustomType).Assembly);
  ```

### CLI tool

There's a CLI tool (_MagicBytesValidator.CLI_) which can be used to determine
MIME types for a local file by calling the following command:

```shell
dotnet run --project MagicBytesValidator.CLI -- [PATH]
```

This can be useful when debugging or validating newly added FileTypes.

### List of file types

| FileType | Extensions                                                                                                       | MIME Types                                                                |
| -------- | ---------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------- |
| AIF      | aif, aiff, aifc                                                                                                  | audio/x-aiff                                                              |
| BIN      | bin, file, com, class, ini                                                                                       | application/octet-stream                                                  |
| BMP      | bmp                                                                                                              | image/bmp                                                                 |
| CAB      | cab                                                                                                              | application/vnd.ms-cab-compressed, application/x-cab-compressed           |
| DOC      | doc, dot                                                                                                         | application/msword                                                        |
| DOCX     | docx                                                                                                             | application/vnd.openxmlformats-officedocument.wordprocessingml.document   |
| DXR      | dxr, dcr, dir                                                                                                    | application/x-director                                                    |
| EXE      | exe, com, dll, drv, pif, qts, qtx , sys, acm, ax, cpl, fon, ocx, olb, scr, vbx, vxd, mui, iec, ime, rs, tsp, efi | application/x-dosexec, application/x-msdos-program                        |
| GIF      | gif                                                                                                              | image/gif                                                                 |
| GZ       | gz                                                                                                               | application/gzip                                                          |
| HEIC     | heic, heif                                                                                                       | image/heic, image/heif                                                    |
| ICO      | ico                                                                                                              | image/x-icon                                                              |
| JPG      | jpg, jpeg, jpe, jif, jfif, jfi                                                                                   | image/jpeg                                                                |
| MIDI     | midi, mid                                                                                                        | audio/x-midi                                                              |
| MP3      | mp3                                                                                                              | audio/mpeg                                                                |
| MP4      | mp4                                                                                                              | video/mp4                                                                 |
| MPG      | mpg, mpeg, mpe, m2p, vob                                                                                         | video/mpeg                                                                |
| ODP      | odp                                                                                                              | application/vnd.oasis.opendocument.presentation                           |
| ODS      | ods                                                                                                              | application/vnd.oasis.opendocument.spreadsheet                            |
| ODT      | odt                                                                                                              | application/vnd.oasis.opendocument.text                                   |
| OGV      | ogv, ogg, oga                                                                                                    | video/ogg                                                                 |
| PBM      | pbm                                                                                                              | image/x-portable-bitmap                                                   |
| PDF      | pdf                                                                                                              | application/pdf                                                           |
| PGM      | pgm                                                                                                              | image/x-portable-graymap                                                  |
| PNG      | png                                                                                                              | image/png                                                                 |
| PPM      | ppm                                                                                                              | image/x-portable-pixmap                                                   |
| PPT      | ppt, ppz, pps, pot                                                                                               | application/mspowerpoint, application/vnd.ms-powerpoint                   |
| PPTX     | pptx                                                                                                             | application/vnd.openxmlformats-officedocument.presentationml.presentation |
| RAR      | rar                                                                                                              | application/vnd.rar, application/x-rar-compressed                         |
| RPM      | rpm                                                                                                              | application/x-rpm, application/x-redhat-package-manager                   |
| RTF      | rtf                                                                                                              | application/rtf                                                           |
| SND      | snd, au                                                                                                          | audio/basic                                                               |
| SVG      | svg, svgz                                                                                                        | image/svg+xml                                                             |
| SWF      | swf                                                                                                              | application/x-shockwave-flash                                             |
| 3GP      | 3gp                                                                                                              | video/3gpp                                                                |
| TIF      | tif, tiff                                                                                                        | image/tiff                                                                |
| TSV      | ts, tsv, tsa, mpg, mpeg                                                                                          | video/mp2t                                                                |
| TXT      | txt                                                                                                              | text/plain                                                                |
| WEBM     | mkv, mka, mks, mk3d, webm                                                                                        | video/webm                                                                |
| XLS      | xls, xla                                                                                                         | application/msexcel                                                       |
| XLSX     | xlsx                                                                                                             | application/vnd.openxmlformats-officedocument.spreadsheetml.sheet         |
| XML      | xml                                                                                                              | application/xml, text/xml                                                 |
| Z        | z                                                                                                                | application/x-compress                                                    |
| ZIP      | zip                                                                                                              | application/zip, application/x-zip-compressed                             |

### License

[MIT License](./LICENSE)

```
▓▓  ▓▓▓▓▓▓▓▓▓
▓▓         ▓▓
▓▓▓▓▓▓▓▓▓  ▓▓
▓▓         ▓▓            traperto GmbH
▓▓  ▓▓▓▓▓▓▓▓▓
▓▓
▓▓▓▓▓▓▓▓▓  ▓▓
```
