using System.Text;
using XmlEditorReloaded.Data.Help;

namespace XmlEditorReloaded.HelpTool;

/// <summary>
/// Generates and verifies the embedded <c>help.json</c> resource from the Markdown help documents.
/// </summary>
internal static class Program
{
    private static int Main(string[] args)
    {
        try
        {
            var options = Options.Parse(args);
            if (options.ShowHelp)
            {
                Console.WriteLine(Options.Usage);
                return 0;
            }

            var json = HelpDataBuilder.BuildJson(options.Source, options.Language);
            if (options.Check)
            {
                if (!File.Exists(options.Output))
                {
                    Console.Error.WriteLine($"Help data '{options.Output}' does not exist.");
                    return 2;
                }

                var existing = File.ReadAllText(options.Output);
                if (!string.Equals(existing, json, StringComparison.Ordinal))
                {
                    Console.Error.WriteLine(
                        $"Help data '{options.Output}' is out of date. Run the help tool without --check.");
                    return 2;
                }

                Console.WriteLine($"Help data '{options.Output}' is up to date.");
                return 0;
            }

            var directory = Path.GetDirectoryName(options.Output);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(options.Output, json, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            Console.WriteLine($"Wrote {options.Output} ({json.Length:N0} characters).");
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception.Message);
            return 1;
        }
    }

    private sealed record Options(
        string Source,
        string Output,
        string Language,
        bool Check,
        bool ShowHelp)
    {
        public const string Usage = """
            Usage: XmlEditorReloaded.HelpTool [--source <help/tables>] [--output <help.json>]
                                              [--language <code>] [--check] [--help]

              --source    Directory with the Markdown table documents. Defaults to the nearest
                          help/tables directory found above the current directory.
              --output    Generated JSON file. Defaults to
                          <repo>/src/XmlEditorReloaded.Data/Help/help.json (or help.ru.json for ru).
              --language  Language code written to the data (default: en).
              --check     Compare the generated data with the output file and fail on mismatch.
            """;

        public static Options Parse(string[] args)
        {
            string? source = null;
            string? output = null;
            var language = "en";
            var check = false;
            var showHelp = false;
            for (var index = 0; index < args.Length; index++)
            {
                var argument = args[index];
                switch (argument)
                {
                    case "--help" or "-h":
                        showHelp = true;
                        break;
                    case "--check":
                        check = true;
                        break;
                    case "--source":
                        source = NextValue(args, ref index, argument);
                        break;
                    case "--output":
                        output = NextValue(args, ref index, argument);
                        break;
                    case "--language":
                        language = NextValue(args, ref index, argument);
                        break;
                    default:
                        throw new ArgumentException($"Unknown argument '{argument}'.");
                }
            }

            source = Path.GetFullPath(source ?? FindDefaultSource());
            output = Path.GetFullPath(output ?? DefaultOutput(source, language));
            return new Options(source, output, language, check, showHelp);
        }

        private static string NextValue(string[] args, ref int index, string argument)
        {
            if (index + 1 >= args.Length || args[index + 1].StartsWith("--", StringComparison.Ordinal))
            {
                throw new ArgumentException($"Argument '{argument}' requires a value.");
            }

            return args[++index];
        }

        private static string FindDefaultSource()
        {
            var directory = new DirectoryInfo(Environment.CurrentDirectory);
            while (directory is not null)
            {
                var candidate = Path.Combine(directory.FullName, "help", "tables");
                if (Directory.Exists(candidate))
                {
                    return candidate;
                }

                directory = directory.Parent;
            }

            throw new ArgumentException(
                "Could not find a help/tables directory. Pass --source explicitly.");
        }

        private static string DefaultOutput(string source, string language)
        {
            var directory = new DirectoryInfo(source);
            while (directory is not null)
            {
                var helpDirectory = Path.Combine(
                    directory.FullName,
                    "src",
                    "XmlEditorReloaded.Data",
                    "Help");
                if (Directory.Exists(helpDirectory))
                {
                    var fileName = string.Equals(language, "ru", StringComparison.OrdinalIgnoreCase)
                        ? "help.ru.json"
                        : "help.json";
                    return Path.Combine(helpDirectory, fileName);
                }

                directory = directory.Parent;
            }

            throw new ArgumentException(
                "Could not find src/XmlEditorReloaded.Data/Help. Pass --output explicitly.");
        }
    }
}
