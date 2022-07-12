# MagicBytesValidator
Recognize filetypes from `Streams` or `IFormfiles` using mime types or file extensions and validate them against the magic bytes according to filetypes.
The existing `FileTypes` can be expanded in various ways.


### How to install?

Install nuget package into your project

```powershell
Install-Package MagicBytesValidator -Version 1.0.0
```

```powershell
dotnet add package MagicBytesValidator --version 1.0.0
```

```powershell
<PackageReference Include="MagicBytesValidator" Version="1.0.0" />
```

### How to use it?

Create a new instance of the Validator

```c#
var validator = new Validator(); //or initialize it with DI
```

Check a file with the stream and the filetype

```c#
var isValid = await _validator.IsValidAsync(memoryStream, fileType);
```

Find a filetype by extension or mimetype

```c#
var pngFileType = validator.Mapping.FindByExtension("png");
var pdfFileType = validator.Mapping.FindByMimeType("application/pdf");
var fileType = _formFileTypeProvider.FindFileTypeForFormFile(file); // in case of a given IFormFile
```

### Expand default the filetype list

you can add or register your own filetypes. Use the validator or create an instance of the mapping.

```c#
var mapper = new Mapping();
```

Register a single Filetype

```c#
mapper.Register(
trpFileType = new FileType("traperto/trp", new[] { "trp" },
                                        new[] { new byte[] { 0x74, 0x72, 0x61, 0x70, 0x65, 0x72, 0x74, 0x6f } });
)
```

you can also register a list of filetypes

```c#
mapper.Register( listOfFileTypes );
```

You can also create variants of `FileType` and register them by passing the Assembly of the new FileTypes, e.g.
`mapping.Register(typeof(CustomFileType).Assembly);`. This will register all FileTypes of the given Assembly that are also
not abstract and have an empty constructor!

```c#
var assembly = typeof(MyAssembly).Assembly;
_mapping.Register(assembly);
)
public class CustomFileType : FileType
{
   public CustomFileType() : base("facade/trp", new[] { "trp" },
                                  new[] { new byte[] { 0x74, 0x72, 0x70 } })
    {
    }
}
```

### List of Filetypes

| Mimetype                                        | Extension                                  | Magicbytes (decimal)                                                                                                |
|-------------------------------------------------|--------------------------------------------|---------------------------------------------------------------------------------------------------------------------|
| audio/x-pn-realaudio-plugin                     | rpm                                        | 237 171 238 219                                                                                                     |
| application/octet-stream                        | bin<br />file<br />com<br />class<br />ini | 83 80 48 49<br />201<br />202 254 186 190                                                                           |
| video/3gpp                                      | 3gp                                        | 102 116 121 112 51 103                                                                                              |
| image/x-icon                                    | ico                                        | 0 0 1 0                                                                                                             |
| image/gif                                       | gif                                        | 71 73 70 56 55 97<br />71 73 70 56 57 97                                                                            |
| image/tiff                                      | tif<br />tiff                              | 73 73 42 0 <br />77 77 0 42                                                                                         |
| image/jpeg                                      | jpg<br />jpeg<br />jpe                     | 255 216 255 219<br />255 216 255 224 0 16 74<br />70 73 70 0 1<br />255 216 255 238<br />105 102 0 0                |
| image/png                                       | png                                        | 137 80 78 71 13 10 26 10                                                                                            |
| video/ogg                                       | ogg<br />ogv                               | 79 103 103 83                                                                                                       |
| audio/basic                                     | snd<br />au                                | 56 83 86 88<br />65 73 70 70                                                                                        |
| application/dsptype                             | tsp                                        | 77 90                                                                                                               |
| text/plain                                      | txt                                        | 239 187 191<br />255 254<br />254 255<br />255 254 0 0                                                              |
| application/zip                                 | zip                                        | 80 75 3 4                                                                                                           |
| application                                     | docx<br />xlsx                             | 80 75 7 8                                                                                                           |
| application/vnd.oasis.opendocument.presentation | odp                                        | 80 75 7 8                                                                                                           |
| application/vnd.oasis.opendocument.spreadsheet  | ods                                        | 80 75 7 8                                                                                                           |
| application/vnd.oasis.opendocument.text         | odt                                        | 80 75 7 8                                                                                                           |
| audio/mpeg                                      | mp3                                        | 73 68 51                                                                                                            |
| image/bmp                                       | bmp                                        | 66 77                                                                                                               |
| audio/x-midi                                    | midi<br />mid                              | 77 84 104 100                                                                                                       |
| application/msword                              | doc<br />dot                               | 208 207 17 224 161 177 26 255                                                                                       |
| application/msexcel                             | xlx<br />xla                               | 208 207 17 224 161 177 26 255                                                                                       |
| application/mspowerpoint                        | ppt<br />ppz<br />pps<br />pt              | 208 207 17 224 161 177 26 225                                                                                       |
| application/gzip                                | gz                                         | 31 139                                                                                                              |
| video/webm                                      | webm                                       | 26 69 223 163                                                                                                       |
| application/rtf                                 | rtf                                        | 123 92 114 116 102 49                                                                                               |
| text/tab-separated-values                       | tsv                                        | 71                                                                                                                  |
| video/mpeg                                      | mpg<br />mpeg<br />mpe                     | 71<br />0 0 1 186<br />0 0 1 179                                                                                    |
| video/mp4                                       | mp4                                        | 102 116 121 112 105 115 111 109 <br />102, 116, 121, 112, 109, 112, 52, 50 <br />102, 116, 121, 112, 77, 83, 62, 86 |
| image/x-portable-bitmap                         | pbm                                        | 80 49 10                                                                                                            |
| image/x-portable-graymap                        | pgm                                        | 80 50 10                                                                                                            |
| image/x-portable-pixmap                         | ppm                                        | 80 51 10                                                                                                            |
| application/pdf                                 | pdf                                        | 25 50 44 46                                                                                                         |

### What is the licence?

MIT License Feel free :yum:


```
▓▓  ▓▓▓▓▓▓▓▓▓▓
▓▓          ▓▓
▓▓▓▓▓▓▓▓▓   ▓▓
▓▓          ▓▓            traperto GmbH
▓▓  ▓▓▓▓▓▓▓▓▓▓
▓▓
▓▓▓▓▓▓▓▓▓   ▓▓
```