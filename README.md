# MagicBytesValidator
Recognize filetypes from `Streams` or `IFormfiles` using mime types or file extensions and validate them against the magic bytes according to the filetypes.
The existing `FileTypes` can be expanded in various ways.

### How to install?

- Install nuget package into your project:

```powershell
Install-Package MagicBytesValidator -Version 2.0.0
```

```bash
dotnet add package MagicBytesValidator --version 2.0.0
```

- Reference in your csproj:
```xml
<PackageReference Include="MagicBytesValidator" Version="2.0.0" />
```

### How to use it?

- Create new instances of the validator & providers:
```c#
var validator = new MagicBytesValidator.Services.Validator();
var formFileTypeProvider = new MagicBytesValidator.Services.Http.FormFileTypeProvider();
var streamFileTypeProvider = new MagicBytesValidator.Services.Streams.StreamFileTypeProvider();
```

- Find a filetype by extension or mimetype:
```c#
var pngFileType = validator.Mapping.FindByExtension("png");
var pdfFileType = validator.Mapping.FindByMimeType("application/pdf");
```
- Determine & validate a filetype by uploaded IFormFile:
```c#
var fileType = await formFileTypeProvider.FindValidatedTypeAsync(formFile, null, CancellationToken.None);
```

- Determine the file type of a file by its stream
```c#
var fileType = await streamFileTypeProvider.TryFindUnambiguousAsync(fileStream, CancellationToken.None);
```

- Check a file with its stream and filetype:
```c#
var isValid = await validator.IsValidAsync(memoryStream, fileType, CancellationToken.None);
```

#### Expand the filetype mapping

- Get mapping:
```c#
// use the validator:
var mapping = validator.Mapping;

// use the formFileTypeProvider:
var mapping = formFileTypeProvider.Mapping;

// or create a new instance of the mapping:
var mapping = new MagicBytesValidator.Services.Mapping(); 
```

- Register a single Filetype:
```c#
mapping.Register(
    new FileByteFilter(
        "traperto/trp",  // mime type
        new[] { "trp" } // file extensions
    ) {
        // magic byte sequences
        StartsWith([
            0x78, 0x6c, 0x2f, 0x5f, 0x72, 0x65
        ])
            .EndsWith([
            0xFF, 0xFF
        ])
    }
)
```

- FileTypes with specific offset checks:
```c#
mapping.Register(
    new FileByteFilter(
        "traperto/trp",  // mime type
        new[] { "trp" } // file extensions
    ) {
        // magic byte sequences
        Specific(new ByteCheck(512, [0xFD]));
    }
)
```
ByteCheck allows for negative offset values to look for a specific offset counting from the end of file

- Register a list of filetypes:
```c#
mapping.Register(listOfFileTypes);
```

You can also create variants of `IFileType` and register them by passing the Assembly of the new FileTypes, e.g.
`mapping.Register(typeof(CustomFileType).Assembly);`. This will register all FileTypes of the given Assembly that are also
not abstract and have an empty constructor!

```c#
public class CustomFileType : FileTypeWithStartSequences
{
    public CustomFileType() : base(
        "traperto/trp",  // mime type
        new[] { "trp" }, // file extensions
        new[] {          // magic byte sequences
            new byte[] { 0x74, 0x72, 0x61, 0x70, 0x65, 0x72, 0x74, 0x6f }
        }
    )
    {
    }
}

var assembly = typeof(CustomFileType).Assembly;
_mapping.Register(assembly);
```

### CLI
There's a CLI tool (_MagicBytesValidator.CLI_) which can be used to determine
MIME types for a local file by calling the following command.
```shell
dotnet run --project MagicBytesValidator.CLI -- [PATH]
```

This can be useful when debugging or validating newly added FileTypes.

### List of Filetypes

| Mimetype                                        | Extension                                  | Magicbytes (decimal)                                                                                                       |
|-------------------------------------------------|--------------------------------------------|----------------------------------------------------------------------------------------------------------------------------|
| audio/x-pn-realaudio-plugin                     | rpm                                        | 237 171 238 219                                                                                                            |
| application/octet-stream                        | bin<br />file<br />com<br />class<br />ini | <ul><li>83 80 48 49</li><li>201</li><li>202 254 186 190</li></ul>                                                          |
| video/3gpp                                      | 3gp                                        | 102 116 121 112 51 103                                                                                                     |
| image/x-icon                                    | ico                                        | 0 0 1 0                                                                                                                    |
| image/gif                                       | gif                                        | <ul><li>71 73 70 56 55 97</li><li>71 73 70 56 57 97</li></ul>                                                              |
| image/tiff                                      | tif<br />tiff                              | <ul><li>73 73 42 0</li><li>77 77 0 42</li></ul>                                                                            |
| image/jpeg                                      | jpg<br />jpeg<br />jpe                     | <ul><li>255 216 255 219</li><li>255 216 255 224 0 16 74</li><li>70 73 70 0 1</li><li>255 216 255 238</li><li>105 102 0 0</li></ul> |
| image/png                                       | png                                        | 137 80 78 71 13 10 26 10                                                                                                   |
| video/ogg                                       | ogg<br />ogv                               | 79 103 103 83                                                                                                              |
| audio/basic                                     | snd<br />au                                | <ul><li>56 83 86 88</li><li>65 73 70 70</li></ul>                                                                          |
| application/dsptype                             | tsp                                        | 77 90                                                                                                                      |
| text/plain                                      | txt                                        | <ul><li>239 187 191</li><li>255 254</li><li>254 255</li><li>255 254 0 0</li></ul>                                          |
| application/zip                                 | zip                                        | 80 75 3 4                                                                                                                  |
| application                                     | docx<br />xlsx                             | 80 75 7 8                                                                                                                  |
| application/vnd.oasis.opendocument.presentation | odp                                        | 80 75 7 8                                                                                                                  |
| application/vnd.oasis.opendocument.spreadsheet  | ods                                        | 80 75 7 8                                                                                                                  |
| application/vnd.oasis.opendocument.text         | odt                                        | 80 75 7 8                                                                                                                  |
| audio/mpeg                                      | mp3                                        | 73 68 51                                                                                                                   |
| image/bmp                                       | bmp                                        | 66 77                                                                                                                      |
| audio/x-midi                                    | midi<br />mid                              | 77 84 104 100                                                                                                              |
| application/msword                              | doc<br />dot                               | 208 207 17 224 161 177 26 255                                                                                              |
| application/msexcel                             | xlx<br />xla                               | 208 207 17 224 161 177 26 255                                                                                              |
| application/mspowerpoint                        | ppt<br />ppz<br />pps<br />pt              | 208 207 17 224 161 177 26 225                                                                                              |
| application/gzip                                | gz                                         | 31 139                                                                                                                     |
| video/webm                                      | webm                                       | 26 69 223 163                                                                                                              |
| application/rtf                                 | rtf                                        | 123 92 114 116 102 49                                                                                                      |
| text/tab-separated-values                       | tsv                                        | 71                                                                                                                         |
| video/mpeg                                      | mpg<br />mpeg<br />mpe                     | <ul><li>71</li><li>0 0 1 186</li><li>0 0 1 179</li></ul>                                                                   |
| video/mp4                                       | mp4                                        | <ul><li>102 116 121 112 105 115 111 109</li><li>102 116 121 112 109 112 52 50</li><li>102 116 121 112 77 83 62 86</li></ul>|
| image/x-portable-bitmap                         | pbm                                        | 80 49 10                                                                                                                   |
| image/x-portable-graymap                        | pgm                                        | 80 50 10                                                                                                                   |
| image/x-portable-pixmap                         | ppm                                        | 80 51 10                                                                                                                   |
| application/pdf                                 | pdf                                        | 25 50 44 46                                                                                                                |

### What is the licence?

MIT License

```
▓▓  ▓▓▓▓▓▓▓▓▓▓
▓▓          ▓▓
▓▓▓▓▓▓▓▓▓   ▓▓
▓▓          ▓▓            traperto GmbH
▓▓  ▓▓▓▓▓▓▓▓▓▓
▓▓
▓▓▓▓▓▓▓▓▓   ▓▓
```