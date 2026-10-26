using System.Reflection;
using XmlEditorReloaded.App.Resources;

namespace XmlEditorReloaded.App.ViewModels;

/// <summary>
/// Content of the About window: assembly product, version, description, and optional copyright.
/// The values come from the assembly metadata of the application, so a release build shows the
/// product data configured for that build.
/// </summary>
public sealed class AboutViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AboutViewModel"/> class from the application
    /// assembly metadata.
    /// </summary>
    public AboutViewModel()
        : this(typeof(AboutViewModel).Assembly)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AboutViewModel"/> class from the metadata of the
    /// given assembly.
    /// </summary>
    /// <param name="assembly">Assembly whose product, version, description, and copyright are shown.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/> is null.</exception>
    public AboutViewModel(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var assemblyName = assembly.GetName();
        ProductName = ReadMetadata<AssemblyProductAttribute>(assembly, static attribute => attribute.Product)
            ?? assemblyName.Name
            ?? string.Empty;
        VersionText = ReadInformationalVersion(assembly) ?? assemblyName.Version?.ToString() ?? string.Empty;
        Description = ReadMetadata<AssemblyDescriptionAttribute>(assembly, static attribute => attribute.Description)
            ?? string.Empty;
        CopyrightText = ReadMetadata<AssemblyCopyrightAttribute>(assembly, static attribute => attribute.Copyright)
            ?? string.Empty;

        Title = Strings.Format("AboutTitleFormat", ProductName);
        VersionLabel = Strings.Get("AboutVersionLabel");
        DescriptionLabel = Strings.Get("AboutDescriptionLabel");
        CopyrightLabel = Strings.Get("AboutCopyrightLabel");
        OkText = Strings.Get("AboutOk");
    }

    /// <summary>
    /// Gets the window title, which includes the product name.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Gets the product name reported by the assembly metadata.
    /// </summary>
    public string ProductName { get; }

    /// <summary>
    /// Gets the assembly informational version without a source-revision suffix.
    /// </summary>
    public string VersionText { get; }

    /// <summary>
    /// Gets the version label.
    /// </summary>
    public string VersionLabel { get; }

    /// <summary>
    /// Gets the build description reported by the assembly metadata, or an empty string.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the description label.
    /// </summary>
    public string DescriptionLabel { get; }

    /// <summary>
    /// Gets the copyright text reported by the assembly metadata, or an empty string.
    /// </summary>
    public string CopyrightText { get; }

    /// <summary>
    /// Gets the copyright label.
    /// </summary>
    public string CopyrightLabel { get; }

    /// <summary>
    /// Gets a value indicating whether the description line is available.
    /// </summary>
    public bool HasDescription => Description.Length > 0;

    /// <summary>
    /// Gets a value indicating whether the copyright line is available.
    /// </summary>
    public bool HasCopyright => CopyrightText.Length > 0;

    /// <summary>
    /// Gets the OK button text.
    /// </summary>
    public string OkText { get; }

    private static string? ReadInformationalVersion(Assembly assembly)
    {
        var informational = ReadMetadata<AssemblyInformationalVersionAttribute>(
            assembly,
            static attribute => attribute.InformationalVersion);
        if (string.IsNullOrWhiteSpace(informational))
        {
            return null;
        }

        var revisionSeparator = informational.IndexOf('+', StringComparison.Ordinal);
        return revisionSeparator < 0 ? informational : informational[..revisionSeparator];
    }

    private static string? ReadMetadata<TAttribute>(Assembly assembly, Func<TAttribute, string?> selector)
        where TAttribute : Attribute
    {
        var attribute = assembly.GetCustomAttribute<TAttribute>();
        var value = attribute is null ? null : selector(attribute);
        return string.IsNullOrWhiteSpace(value) ? null : value;
    }
}
