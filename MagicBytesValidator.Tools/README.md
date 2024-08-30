# MagicByteValidator.Tools

## About this project

This is a helper project for internal tasks, e. g. updating the main [README.md](../README.md).
**It is not meant to be used by non-traperto developers.**  
If you want to debug your file types or sample files, use the [MagicBytesValidator.CLI](../MagicBytesValidator.CLI/)
project.

## Usage

```shell
dotnet run --project MagicBytesValidator.Tools -- [tool]
```

### Available tools

- _table_: Prints a markdown table containing all known file types, including extensions and MIME types.  
  Useful for updating the project README file.
- _help_: Shows help page of this project.