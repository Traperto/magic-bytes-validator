# MagicBytesValidator

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
var isValid = _validator.IsValidAsync(memoryStream, fileType);
```

Create a filetype by extension or mimetype

```c#
var pngileType = validator.Mapping.FindByExtension("png");
var pdfFileType = validator.Mapping.FindByMimeType("application/pdf");
```

### Expand standard the filetype list

you can add or register your own filetypes. Use the validator or create an instance of the Mapper.

```c#
var mapper = new Mapping();
```

Register a single Filetype

```c#
mapper.Register(
_trpFileType = new FileType("traperto/trp", new[] { "trp" },
                                        new[] { new byte[] { 0x74, 0x72, 0x61, 0x70, 0x65, 0x72, 0x74, 0x6f } });
)
```

you can also register a list of filetypes

```c#
mapper.Register( listOfFileTypes );
```

you can register new fileTypes by an assembly

```c#
var assembly = typeof(MyAssembly).Assembly;
_mapping.Register(assembly);
)
public class AssemblyFacade : FileType
{
   public AssemblyFacade() : base("facade/trp", new[] { "trp" },
                                  new[] { new byte[] { 0x74, 0x72, 0x70 } })
    {
    }
}
```

### List of Filetypes

| Mimetype                                        | Extension              | Magicbytes                                                                       |
|-------------------------------------------------|------------------------|----------------------------------------------------------------------------------|
| audio/x-pn-realaudio-plugin                     | rpm                    | 237 171 238 219                                                                  |
| application/octet-stream                        | bin file com class ini | 83 80 48 49 201 202 254 186 190                                                  |
| video/3gpp                                      | 3gp                    | 102 116 121 112 51 103                                                           |
| image/x-icon                                    | ico                    | 0 0 1 0                                                                          |
| image/gif                                       | gif                    | 71 73 70 56 55 97 71 73 70 56 57 97                                              |
| image/tiff                                      | tif tiff               | 73 73 42 0 77 77 0 42                                                            |
| image/jpeg                                      | jpg jpeg jpe           | 255 216 255 219 255 216 255 224 0 16 74 70 73 70 0 1 255 216 255 238 105 102 0 0 |
| image/png                                       | png                    | 137 80 78 71 13 10 26 10                                                         |
| video/ogg                                       | ogg ogv                | 79 103 103 83                                                                    |
| audio/basic                                     | snd au                 | 56 83 86 88 65 73 70 70                                                          |
| application/dsptype                             | tsp                    | 77 90                                                                            |
| text/plain                                      | txt                    | 239 187 191 255 254 254 255 255 254 0 0                                          |
| application/zip                                 | zip                    | 80 75 3 4                                                                        |
| application                                     | docx xlsx              | 80 75 7 8                                                                        |
| application/vnd.oasis.opendocument.presentation | odp                    | 80 75 7 8                                                                        |
| application/vnd.oasis.opendocument.spreadsheet  | ods                    | 80 75 7 8                                                                        |
| application/vnd.oasis.opendocument.text         | odt                    | 80 75 7 8                                                                        |
| audio/mpeg                                      | mp3                    | 73 68 51                                                                         |
| image/bmp                                       | bmp                    | 66 77                                                                            |
| audio/x-midi                                    | midi mid               | 77 84 104 100                                                                    |
| application/msword                              | doc dot                | 208 207 17 224 161 177 26 255                                                    |
| application/msexcel                             | xlx xla                | 208 207 17 224 161 177 26 255                                                    |
| application/mspowerpoint                        | ppt ppz pps pt         | 208 207 17 224 161 177 26 225                                                    |
| application/gzip                                | gz                     | 31 139                                                                           |
| video/webm                                      | webm                   | 26 69 223 163                                                                    |
| application/rtf                                 | rtf                    | 123 92 114 116 102 49                                                            |
| text/tab-separated-values                       | tsv                    | 71                                                                               |
| video/mpeg                                      | mpg mpeg mpe           | 71 0 0 1 186 0 0 1 179                                                           |
| audio/mp4                                       | mp4                    | 102 116 121 112 105 115 111 109                                                  |
| image/x-portable-bitmap                         | pbm                    | 80 49 10                                                                         |
| image/x-portable-graymap                        | pgm                    | 80 50 10                                                                         |
| image/x-portable-pixmap                         | ppm                    | 80 51 10                                                                         |
| application/x-director                          | dxr dcr dir            | 77 86 57 51                                                                      |
| application/x-director                          | dxr dcr dir            | 77 86 57 51                                                                      |
| application/pdf                                 | pdf                    | 25 50 44 46                                                                      |

### What is the licence?

MIT License Feel free :yum:
