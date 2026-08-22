using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;
using SnipPlus.Windows;

namespace SnipPlus.Windows.Tests;

[TestClass]
public sealed class WindowsCompleteExecutionTraceSinkTests
{
    private static readonly System.Text.Json.JsonSerializerOptions WebJsonOptions =
        new(System.Text.Json.JsonSerializerDefaults.Web);

    [TestMethod]
    [TestCategory("Unit")]
    public void ClearRemovesDiagnosticLogAndIsIdempotent()
    {
        var directory = Path.Combine(
            Path.GetTempPath(),
            "SnipPlusTests",
            Guid.NewGuid().ToString("N"));
        var path = Path.Combine(directory, "diagnostics.jsonl");

        try
        {
            var sink = new WindowsCompleteExecutionTraceSink(path);
            sink.Record(new CompleteExecutionTraceEntry
            {
                TimestampUtc = DateTimeOffset.UnixEpoch,
                SessionId = Guid.NewGuid(),
                SelectionRevision = 0,
                WorkflowState = WorkflowState.ResidentReady,
                CompleteStage = CompleteExecutionStage.Diagnostic,
                Component = "test",
                DiagnosticEvent = "test"
            });

            Assert.IsTrue(File.Exists(path));

            Assert.IsTrue(sink.Clear());
            Assert.IsTrue(sink.Clear());

            Assert.IsFalse(File.Exists(path));
        }
        finally
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }
        }
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void RecordApplicationDiagnosticWritesAStableDiagnosticEntry()
    {
        var directory = Path.Combine(
            Path.GetTempPath(),
            "SnipPlusTests",
            Guid.NewGuid().ToString("N"));
        var path = Path.Combine(directory, "diagnostics.jsonl");

        try
        {
            var sink = new WindowsCompleteExecutionTraceSink(path);
            sink.RecordApplicationDiagnostic("Application.Started");

            var line = File.ReadAllText(path);
            var entry = System.Text.Json.JsonSerializer.Deserialize<CompleteExecutionTraceEntry>(
                line,
                WebJsonOptions);

            Assert.IsNotNull(entry);
            Assert.AreEqual(Guid.Empty, entry.SessionId);
            Assert.AreEqual(-1, entry.SelectionRevision);
            Assert.AreEqual(WorkflowState.ResidentReady, entry.WorkflowState);
            Assert.AreEqual(CompleteExecutionStage.Diagnostic, entry.CompleteStage);
            Assert.AreEqual("Application.Started", entry.DiagnosticEvent);
        }
        finally
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }
        }
    }
}
