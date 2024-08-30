namespace MagicBytesValidator.Tools;

public class Program
{
    public static void Main(string[] args)
    {
        var argument = args.FirstOrDefault()?.ToLower() ?? "help";

        switch (argument)
        {
            case "table":
                PrintTypesTable();
                break;
            case "help":
                PrintHelp();
                break;
            default:
                Console.Error.WriteLine($"Unknown option '{argument}'. Maybe try using 'help'?");
                break;
        }
    }

    private static void PrintTypesTable()
    {
        Console.WriteLine();
        Console.WriteLine("|FileType|Extensions|MIME Types|");
        Console.WriteLine("|-|-|-|");

        foreach (var fileType in (new Mapping()).FileTypes)
        {
            var mimeTypeList = string.Join(", ", fileType.MimeTypes);
            var extensionList = string.Join(", ", fileType.Extensions);

            Console.WriteLine($"|{fileType.GetType().Name.ToUpper()}|{extensionList}|{mimeTypeList}|");
        }
    }

    private static void PrintHelp()
    {
        Console.WriteLine("""

                          Usage: dotnet run -- [tool]

                          Available tools:

                          - table
                            Prints a markdown table containing all known file types, including extensions and MIME types.
                            Useful for updating the project README file.

                          - help
                            Shows this help

                          (traperto GmbH, 2024)
                          """);
    }
}