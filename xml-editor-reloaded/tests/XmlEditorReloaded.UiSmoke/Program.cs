using XmlEditorReloaded.UiSmoke.Scenarios;
using System.IO;

namespace XmlEditorReloaded.UiSmoke;

/// <summary>
/// Entry point of the UI smoke runner.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Runs the smoke runner.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    /// <returns>Zero when every selected scenario passed.</returns>
    [STAThread]
    private static int Main(string[] args)
    {
        Console.OutputEncoding = SmokeOptions.ConsoleEncoding;
        try
        {
            var options = SmokeOptions.Parse(args);
            var strings = UiStrings.Load(
                Path.Combine(options.RepositoryRoot, "xml-editor-reloaded", "src", "XmlEditorReloaded.App", "Resources"),
                options.UiLanguage);
            var catalog = ChecklistCatalog.Load(
                Path.Combine(options.RepositoryRoot, "M23A-PARITY", "MANUAL-SMOKE-CHECKLIST.md"));

            if (options.List)
            {
                PrintCatalog(catalog);
                return 0;
            }

            NativeInput.KeepDisplayOn();
            try
            {
                return new SmokeRunner(options, strings, catalog).Run();
            }
            finally
            {
                NativeInput.AllowDisplaySleep();
            }
        }
        catch (ArgumentException exception)
        {
            Console.Error.WriteLine(exception.Message);
            return 2;
        }
    }

    private static void PrintCatalog(ChecklistCatalog catalog)
    {
        var implemented = ScenarioCatalog.All.Select(static scenario => scenario.Id).ToHashSet(StringComparer.Ordinal);
        foreach (var id in catalog.Ids)
        {
            var state = implemented.Contains(id) ? "automated" : ScenarioCatalog.IsManual(id) ? "manual" : "planned";
            Console.WriteLine($"{id}  {state,-10} {catalog.TitleOf(id)}");
        }
    }
}
