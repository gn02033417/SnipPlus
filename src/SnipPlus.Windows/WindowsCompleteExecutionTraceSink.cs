using System.Text.Json;
using SnipPlus.Contracts;
using Windows.Storage;

namespace SnipPlus.Windows;

public sealed class WindowsCompleteExecutionTraceSink : ICompleteExecutionTraceSink
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly object _gate = new();
    private readonly string? _diagnosticsPathOverride;

    public WindowsCompleteExecutionTraceSink(string? diagnosticsPath = null)
    {
        _diagnosticsPathOverride = diagnosticsPath;
    }

    public void Record(CompleteExecutionTraceEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        try
        {
            var path = GetDiagnosticsPath();
            var diagnosticsDirectory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(diagnosticsDirectory))
            {
                Directory.CreateDirectory(diagnosticsDirectory);
            }

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

    public void RecordApplicationDiagnostic(
        string diagnosticEvent,
        string? diagnosticMessage = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(diagnosticEvent);

        Record(new CompleteExecutionTraceEntry
        {
            TimestampUtc = DateTimeOffset.UtcNow,
            SessionId = Guid.Empty,
            SelectionRevision = -1,
            WorkflowState = WorkflowState.ResidentReady,
            CompleteStage = CompleteExecutionStage.Diagnostic,
            Component = nameof(WindowsCompleteExecutionTraceSink),
            ManagedThreadId = Environment.CurrentManagedThreadId,
            DiagnosticEvent = diagnosticEvent,
            DiagnosticMessage = diagnosticMessage
        });
    }

    public bool Clear()
    {
        try
        {
            lock (_gate)
            {
                var path = GetDiagnosticsPath();
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                return true;
            }
        }
        catch
        {
            // Diagnostic cleanup must never affect the application.
            return false;
        }
    }

    private string GetDiagnosticsPath() => _diagnosticsPathOverride ?? Path.Combine(
        ApplicationData.Current.LocalCacheFolder.Path,
        "Diagnostics",
        "stage6c-complete-failure.jsonl");
}
