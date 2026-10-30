using System.Globalization;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using XmlEditorReloaded.Core.Diagnostics;

namespace XmlEditorReloaded.Data.Diagnostics;

/// <summary>
/// Result of the start-up write probe of the application-data directory.
/// </summary>
/// <param name="IsWritable">Whether a probe file could be created, written, and deleted.</param>
/// <param name="Message">Human-readable description of the result, including the directory path.</param>
public sealed record StartupWriteProbe(bool IsWritable, string Message);

/// <summary>
/// Start-up diagnostics that explain access failures without requiring elevation: the current user,
/// whether the process is elevated, and the write access and metadata of the files the application
/// needs. The probe only creates and deletes its own temporary file; it never changes attributes or
/// ACLs of existing files.
/// </summary>
public static class StartupDiagnostics
{
    /// <summary>
    /// Probes whether the application can create, write, and delete a file in
    /// <paramref name="directory"/>. The directory is created when it does not exist.
    /// </summary>
    /// <param name="directory">Application-data directory to probe.</param>
    /// <returns>The probe result; on failure the message names the path and the most likely cause.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="directory"/> is empty.</exception>
    public static StartupWriteProbe ProbeWriteAccess(string directory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);

        var probePath = Path.Combine(directory, $".write-probe-{Guid.NewGuid():N}.tmp");
        try
        {
            Directory.CreateDirectory(directory);
            using (var stream = new FileStream(probePath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                stream.WriteByte(0);
                stream.Flush(flushToDisk: true);
            }

            return new StartupWriteProbe(
                true,
                $"Write access to the application data directory '{directory}' is available.");
        }
        catch (Exception exception) when (
            exception is IOException
                or UnauthorizedAccessException
                or NotSupportedException
                or ArgumentException
                or SecurityException)
        {
            return new StartupWriteProbe(
                false,
                FileAccessFailure.DescribeWriteFailure(directory, exception));
        }
        finally
        {
            try
            {
                if (File.Exists(probePath))
                {
                    File.Delete(probePath);
                }
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                // A leftover probe file is harmless and is reported by the write probe of the next start.
            }
        }
    }

    /// <summary>
    /// Builds a multi-line start-up report with the user, the elevation state, the application-data
    /// directory, and the existence, attributes, and owner of the files the application uses.
    /// </summary>
    /// <param name="applicationDataDirectory">Directory that stores settings, layouts, and the database.</param>
    /// <param name="targetFiles">Files whose metadata should be recorded, for example the log and settings.</param>
    /// <returns>One report line per fact, ready to be written to the operation log.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="applicationDataDirectory"/> is empty.</exception>
    public static string DescribeEnvironment(string applicationDataDirectory, params string[] targetFiles)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(applicationDataDirectory);
        ArgumentNullException.ThrowIfNull(targetFiles);

        var builder = new StringBuilder();
        builder.Append("Preflight: user = ")
            .Append(Environment.UserDomainName)
            .Append('\\')
            .AppendLine(Environment.UserName);
        builder.Append("Preflight: elevated = ")
            .AppendLine(Environment.IsPrivilegedProcess ? "yes" : "no");
        builder.Append("Preflight: application data directory = ")
            .AppendLine(applicationDataDirectory);
        builder.Append("Preflight: temporary directory = ")
            .AppendLine(Path.GetTempPath());

        foreach (var path in targetFiles)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                builder.Append("Preflight: ").AppendLine(DescribeFile(path));
            }
        }

        return builder.ToString().TrimEnd();
    }

    /// <summary>
    /// Describes one file: whether it exists, and, when it does, its size, attributes, and owner.
    /// </summary>
    /// <param name="path">File to inspect.</param>
    /// <returns>A single description line; unavailable metadata is reported as such, not thrown.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is empty.</exception>
    public static string DescribeFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        var info = new FileInfo(path);
        if (!info.Exists)
        {
            return $"file '{path}': exists = no";
        }

        var attributes = "unavailable";
        try
        {
            attributes = info.Attributes.ToString();
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or SecurityException)
        {
            // The attributes are diagnostics only.
        }

        var size = info.Length.ToString(CultureInfo.InvariantCulture);
        return $"file '{path}': exists = yes, size = {size}, attributes = {attributes}, owner = {DescribeOwner(info)}";
    }

    private static string DescribeOwner(FileInfo info)
    {
        if (!OperatingSystem.IsWindows())
        {
            return "unknown (not Windows)";
        }

        try
        {
            var security = info.GetAccessControl();
            return security.GetOwner(typeof(NTAccount))?.Value ?? "unknown";
        }
        catch (Exception exception) when (
            exception is UnauthorizedAccessException
                or IOException
                or SecurityException
                or InvalidOperationException
                or PlatformNotSupportedException
                or IdentityNotMappedException)
        {
            return $"unavailable ({exception.GetType().Name})";
        }
    }
}
