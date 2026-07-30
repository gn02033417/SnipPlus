using System.Text.Json;
using SnipPlus.Contracts;
using Windows.Storage;

namespace SnipPlus.Windows;

public sealed class WindowsCompleteExecutionTraceSink : ICompleteExecutionTraceSink
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly object _gate = new();

    public void Record(CompleteExecutionTraceEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        try
        {
            var diagnosticsDirectory = Path.Combine(
                ApplicationData.Current.LocalCacheFolder.Path,
                "Diagnostics");
            Directory.CreateDirectory(diagnosticsDirectory);
            var path = Path.Combine(diagnosticsDirectory, "stage6c-complete-failure.jsonl");
            var line = JsonSerializer.Serialize(entry, SerializerOptions) + Environment.NewLine;
            lock (_gate)
            {
                File.AppendAllText(path, line);
            }
        }
        catch
        {
            // Trace failure must never affect the capture workflow.
        }
    }
}
