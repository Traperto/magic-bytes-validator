# MagicBytesValidator

Recognize file types from `Stream`s or `IFormFile`s using MIME types or file extensions and validate them against the magic bytes according to the file types.
The existing file types can be expanded in various ways.

### How to install

There are two packages:

- `MagicBytesValidator`: the core library to validate `Stream`s. It has no dependencies.
- `MagicBytesValidator.AspNetCore`: validation of uploaded `IFormFile`s and registration in the dependency
  injection container of ASP.NET Core. It references the core package.

```bash
dotnet add package MagicBytesValidator --version 3.0.0
dotnet add package MagicBytesValidator.AspNetCore --version 3.0.0
```

- Or reference in your csproj:
```xml
<PackageReference Include="MagicBytesValidator" Version="3.0.0" />
<PackageReference Include="MagicBytesValidator.AspNetCore" Version="3.0.0" />
```

Upgrading from 2.x? See [Upgrading to 3.0](#upgrading-to-30).

### Register in ASP.NET Core (MagicBytesValidator.AspNetCore)

```c#
using MagicBytesValidator.AspNetCore;

builder.Services.AddMagicBytesValidator();

// or with custom file types:
builder.Services.AddMagicBytesValidator(mapping => mapping.Register(new CustomType()));
```

This registers `IMapping`, `IValidator`, `IStreamFileTypeProvider` and `IFormFileTypeProvider` as singletons
that share the same mapping.

### How to use

- Check if a stream content matches a file type:

  ```c#
  var validator = new MagicBytesValidator.Services.Validator();
  var pngFileType = validator.Mapping.FindByExtension("png");

  var isValidPng = await validator.IsValidAsync(memoryStream, pngFileType, CancellationToken.None);
  ```

- Find and validate the file type of an uploaded `IFormFile` by its Content-Type, extension and content
  (requires `MagicBytesValidator.AspNetCore`):

  ```c#
  var formFileTypeProvider = new MagicBytesValidator.AspNetCore.Services.Http.FormFileTypeProvider();

  try {
      var fileType = await formFileTypeProvider.FindValidatedTypeAsync(formFile, null, CancellationToken.None);

      if (fileType is not null) {
          // Further code
      } else {
          // Can't determine type
      }
  } catch(MagicBytesValidator.AspNetCore.Exceptions.Http.MimeTypeMismatchException) {
      // Content and given MIME type / extension don't match.
  }
  ```

- Determine the file type of a stream by its content only:

  ```c#
  var streamFileTypeProvider = new MagicBytesValidator.Services.Streams.StreamFileTypeProvider();
  var fileType = await streamFileTypeProvider.TryFindUnambiguousAsync(fileStream, CancellationToken.None);

  if (fileType is not null) {
      // Further code
  } else {
      // Unknown type or more than one type matches the content
  }
  ```

### Stream handling and memory usage

- The whole stream is read into memory for validation, as some checks (e.g. for docx/xlsx/pptx or legacy
  Office files) need the complete file content. Limit the upload size before validating, e.g. via Kestrel's
  `MaxRequestBodySize` or `FormOptions.MultipartBodyLengthLimit`. Streams larger than `Array.MaxLength` bytes
  are rejected with an `ArgumentException`.
- Seekable streams are read from their beginning and their position is restored afterwards.
- Non-seekable streams (e.g. a raw request body) are read from their current position and are consumed
  afterwards. Buffer them (e.g. in a `MemoryStream`) if you need the content again.
- `FindValidatedTypeAsync` disposes the stream it opens via `IFormFile.OpenReadStream()`. A stream passed in
  as `formFileStream` is left open.

## Validation strictness (FileByteType)

Some formats support multiple validation strategies (e.g. strict vs. lazy rules).
For this purpose, the library exposes `FileByteType`:

- `FileByteType.Strict` (default)
- `FileByteType.Lazy` (optional relaxed/lazy rules for certain formats)

### Validate an uploaded IFormFile with a specific validation type

The form file provider accepts an optional `validationType` parameter:

```c#
var fileType = await formFileTypeProvider.FindValidatedTypeAsync(
    formFile,
    null,
    CancellationToken.None,
    validationType: MagicBytesValidator.Models.FileByteType.Strict
);
```

Example using `Lazy` (lazy rules if the format supports it):

```c#
var fileType = await formFileTypeProvider.FindValidatedTypeAsync(
    formFile,
    null,
    CancellationToken.None,
    validationType: MagicBytesValidator.Models.FileByteType.Lazy
);
```

### Validate a stream with a specific validation type

The validator also accepts an optional `validationType` parameter:

```c#
var isValid = await validator.IsValidAsync(
    memoryStream,
    fileType,
    CancellationToken.None,
    validationType: MagicBytesValidator.Models.FileByteType.Strict
);
```

Example using `Lazy`:

```c#
var isValid = await validator.IsValidAsync(
    memoryStream,
    fileType,
    CancellationToken.None,
    validationType: MagicBytesValidator.Models.FileByteType.Lazy
);
```

The methods of `StreamFileTypeProvider` (`FindAllMatchesAsync`, `FindCloseMatchesAsync` and
`TryFindUnambiguousAsync`) accept the same optional `validationType` parameter.

> Note: If a format does not define any `Lazy`-specific checks, `Lazy` behaves like “global checks only”.
> This keeps existing formats unchanged unless they opt into mode-specific rules.

### Example: Lazy ("relaxed") PDF validation

Some PDFs contain additional trailing bytes after the `%%EOF` marker. While strict validation may require the file
to end with `%%EOF`, lazy validation can accept the `%%EOF` marker anywhere within the last 1024 bytes of the file
(behaviour tolerated by common PDF viewers).

## Expand the file type mapping

- Get mapping:

```csharp
// use the validator:
var mapping = validator.Mapping;

// use the formFileTypeProvider:
var mapping = formFileTypeProvider.Mapping;

// or create a new instance of the mapping:
var mapping = new MagicBytesValidator.Services.Mapping();
```

### Add custom file types

- Define a custom type by deriving from `FileByteFilter` and register it:

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
          .Specific(new ByteCheck(512, [0xFD])); // byte 0xFD at offset 512
      }
  }

  var mapping = new MagicBytesValidator.Services.Mapping();
  mapping.Register(new CustomType());
  mapping.Register(new[] { new CustomType() }); // Add multiple types

  // Registering all `IFileType`s of the given assembly that are also not abstract and have an empty constructor.
  mapping.Register(typeof(CustomType).Assembly);
  ```

A `ByteCheck` with a negative offset counts from the end of the file, e.g. `new ByteCheck(-4, [0xFD])` expects
`0xFD` at the fourth to last byte.
Byte sequences may contain `null` as a wildcard for a single arbitrary byte.

A `FileByteFilter` is frozen when it is used for the first time (or when `Freeze()` is called). Afterwards its
checks can't be changed anymore and further configuration throws an `InvalidOperationException`, so configure
your types completely in their constructor. Frozen filters can safely be used from multiple threads.

### Optional: register mode-specific magic byte checks (Strict/Lazy)

When configuring a `FileByteFilter`, fluent methods accept an optional `FileByteType` parameter.
If omitted, the check is global (applies to all validation types). If specified, the check applies only
to that validation type.

Example:

```csharp
StartsWith(new byte?[] { 0x25, 0x50, 0x44, 0x46, 0x2D }) // global
    .EndsWithAnyOf(new[]
    {
        new byte?[] { 0x25, 0x25, 0x45, 0x4F, 0x46 }
    }, MagicBytesValidator.Models.FileByteType.Strict)      // strict only
    .TailContains(1024, new byte?[] { 0x25, 0x25, 0x45, 0x4F, 0x46 },
        MagicBytesValidator.Models.FileByteType.Lazy);   // lazy only
```

### CLI tool

There's a CLI tool (_MagicBytesValidator.CLI_) which can be used to determine
MIME types for a local file by calling the following command:

```shell
dotnet run --project MagicBytesValidator.CLI -- [PATH]
```

This can be useful when debugging or validating newly added FileTypes.

### Upgrading to 3.0

**Packages**

- Everything related to ASP.NET Core moved to the new package `MagicBytesValidator.AspNetCore`:
  `FormFileTypeProvider`, `IFormFileTypeProvider` and `MimeTypeMismatchException`. Their namespaces changed
  from `MagicBytesValidator.Services.Http` and `MagicBytesValidator.Exceptions.Http` to
  `MagicBytesValidator.AspNetCore.Services.Http` and `MagicBytesValidator.AspNetCore.Exceptions.Http`.
  The core package no longer depends on ASP.NET Core.
- Use `services.AddMagicBytesValidator()` to register the services in the dependency injection container.

**Removed APIs**

| Removed                                          | Replacement                                                        |
| ------------------------------------------------ | ------------------------------------------------------------------ |
| `FormFileTypeProvider.FindFileTypeForFormFile`   | `FindValidatedTypeAsync`                                           |
| `StreamFileTypeProvider.FindByMagicByteSequenceAsync` | `TryFindUnambiguousAsync`                                     |
| `FileTypeCollector.CollectFileTypes`             | `FileTypeCollector.CollectFileTypesForAssembly` or `Mapping.Register(assembly)` |
| `EnumerableExtensions.AsIndexed`                 | –                                                                  |

**Changed APIs**

- `Mapping` properties and constructor parameters of `Validator`, `FormFileTypeProvider` and their interfaces
  are typed as `IMapping` instead of `Mapping`. `StreamFileTypeProvider` got an optional mapping parameter and
  a `Mapping` property as well.
- `IFileType.MimeTypes` and `IFileType.Extensions` are `IReadOnlyList<string>` instead of `string[]`.
- `FindAllMatchesAsync` and `FindCloseMatchesAsync` return `IReadOnlyList<IFileType>` instead of
  `IEnumerable<IFileType>`.
- `ByteCheck.Offset` and `ByteCheck.ByteArray` (fields) were replaced by the read-only properties `Offset` and
  `Bytes`.
- `ArgumentEmptyException` derives from `ArgumentException` instead of `ArgumentNullException`. `null`
  arguments throw an `ArgumentNullException` instead.

**Changed behaviour**

- `FileByteFilter`s are frozen after their first use, see [Add custom file types](#add-custom-file-types).
- `TryFindUnambiguousAsync` returns `null` if more than one file type matches (it returned the first match before).
- A negative `ByteCheck` offset counts from the end of the file (before, every negative offset checked the very
  last bytes).
- `FindValidatedTypeAsync` ignores parameters of the Content-Type (e.g. `; charset=utf-8`) and returns `null`
  instead of throwing if the Content-Type is missing.
- MPEG transport streams (`video/mp2t`) are detected by the first two packets, so files starting with `G`
  (e.g. GIFs) are no longer detected as such. The extensions `tsv`, `mpg` and `mpeg` no longer belong to them.

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
| EXE      | exe, com, dll, drv, pif, qts, qtx, sys, acm, ax, cpl, fon, ocx, olb, scr, vbx, vxd, mui, iec, ime, rs, tsp, efi  | application/x-dosexec, application/x-msdos-program                        |
| FLAC     | flac                                                                                                             | audio/flac                                                                |
| GIF      | gif                                                                                                              | image/gif                                                                 |
| GZ       | gz                                                                                                               | application/gzip                                                          |
| HEIC     | heic, heif                                                                                                       | image/heic, image/heif                                                    |
| ICO      | ico                                                                                                              | image/x-icon                                                              |
| JPG      | jpg, jpeg, jpe, jif, jfif, jfi                                                                                   | image/jpeg                                                                |
| M4A      | m4a                                                                                                              | audio/mp4, audio/x-m4a                                                    |
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
| TSV      | ts, tsa                                                                                                          | video/mp2t                                                                |
| TXT      | txt                                                                                                              | text/plain                                                                |
| WAV      | wav                                                                                                              | audio/wav, audio/x-wav                                                    |
| WEBM     | mkv, mka, mks, mk3d, webm                                                                                        | video/webm                                                                |
| XLS      | xls, xla                                                                                                         | application/msexcel                                                       |
| XLSX     | xlsx                                                                                                             | application/vnd.openxmlformats-officedocument.spreadsheetml.sheet         |
| XML      | xml                                                                                                              | application/xml, text/xml                                                 |
| Z        | z                                                                                                                | application/x-compress                                                    |
| ZIP      | zip                                                                                                              | application/zip, application/x-zip-compressed                             |

### License

[MIT License](./LICENSE)
