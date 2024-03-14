using MagicBytesValidator.CLI.Exceptions;
using MagicBytesValidator.Services;
using MagicBytesValidator.Services.Streams;

namespace MagicBytesValidator.CLI;

public class Program
{
    public static void Main()
    {
        Console.WriteLine();
        var streamFileTypeProvider = GetStreamFileTypeProvider();

        try
        {
            var path = LoadPathFromArgs();
            var file = File.Open(path, FileMode.Open, FileAccess.Read);

            var matches = streamFileTypeProvider
               .FindAllMatchesAsync(file, CancellationToken.None)
               .GetAwaiter()
               .GetResult()
               .ToList();

            if (matches.Count == 0)
            {
                Console.WriteLine("No matches.");
                return;
            }

            Console.WriteLine($"{"FileType",-10}| {"Extensions",-40}| {"MIME Types",-80}");
            Console.WriteLine(new string('-', 130));

            foreach (var match in matches)
            {
                var mimeTypeList = string.Join(", ", match.MimeTypes);
                var extensionList = string.Join(", ", match.Extensions);

                Console.WriteLine($"{match.GetType().Name,-10}| {extensionList,-40}| {mimeTypeList,-80}");
            }

            Console.WriteLine();
            Console.WriteLine($"Unambiguous match: {(matches.Count == 1 ? "Yes" : "No")}");
        }
        catch (InvalidProgramCallException)
        {
            Console.Error.WriteLine(
                """
                Usage: dotnet run -- [PATH]
                PATH must point to an existing, readable file
                """);
        }
        catch (FileNotFoundException)
        {
            Console.Error.WriteLine("Error: File not found");
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(
                """
                Error: Internal Exception.
                This should not happen. Please file a GitHub issue with the stack trace attached:
                """);
            Console.Error.WriteLine(exception);
        }
    }

    private static string LoadPathFromArgs()
    {
        var args = Environment.GetCommandLineArgs();
        if (
            args is not { Length: 2 }
            || args is not [_, var path]
            || string.IsNullOrWhiteSpace(path)
        )
        {
            throw new InvalidProgramCallException();
        }

        return Path.GetFullPath(path);
    }

    private static StreamFileTypeProvider GetStreamFileTypeProvider()
    {
        return new StreamFileTypeProvider(new Mapping());
    }
}