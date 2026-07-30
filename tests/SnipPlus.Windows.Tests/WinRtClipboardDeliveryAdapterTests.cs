using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;
using SnipPlus.Windows;
using Windows.ApplicationModel.DataTransfer;

namespace SnipPlus.Windows.Tests;

[TestClass]
public sealed class WinRtClipboardDeliveryAdapterTests
{
    private static readonly string[] DispatcherDiagnosticEvents =
    {
        "BeforeEnqueue",
        "AfterEnqueue",
        "CallbackEntered",
        "SetContentBefore",
        "SetContentAfter",
        "FlushBefore",
        "FlushAfter"
    };

    private static readonly string[] RuntimeInitializationCallOrder =
    {
        "Enter",
        "SetContent",
        "Flush",
        "Dispose"
    };

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Clipboard")]
    public async Task BusyClipboardRetriesAndFlushesAfterSuccess()
    {
        using var image = CreateImage();
        var attempts = 0;
        var flushes = 0;
        var historyAllowed = true;
        var roamingAllowed = true;
        var adapter = new WinRtClipboardDeliveryAdapter(
            (_, options) =>
            {
                attempts++;
                historyAllowed = options.IsAllowedInHistory;
                roamingAllowed = options.IsRoamable;
                return attempts == 3;
            },
            () => flushes++);

        var result = await adapter.DeliverAsync(
            CreateRequest(image, maximumAttempts: 3, retryBudget: TimeSpan.FromSeconds(1)),
            CancellationToken.None);

        var delivered = result as ClipboardDeliveryResult.Delivered;
        Assert.IsNotNull(delivered);
        Assert.AreEqual(3, delivered.Attempts);
        Assert.AreEqual(3, attempts);
        Assert.AreEqual(1, flushes);
        Assert.IsFalse(historyAllowed);
        Assert.IsFalse(roamingAllowed);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Clipboard")]
    public async Task BusyClipboardStopsAtRetryBudget()
    {
        using var image = CreateImage();
        var attempts = 0;
        var flushes = 0;
        var adapter = new WinRtClipboardDeliveryAdapter(
            (_, _) =>
            {
                attempts++;
                return false;
            },
            () => flushes++);

        var result = await adapter.DeliverAsync(
            CreateRequest(image, maximumAttempts: 5, retryBudget: TimeSpan.FromMilliseconds(1)),
            CancellationToken.None);

        var retryable = result as ClipboardDeliveryResult.RetryableFailure;
        Assert.IsNotNull(retryable);
        Assert.AreEqual(FailureCode.ClipboardBusy, retryable.Failure.Code);
        Assert.AreEqual(1, retryable.AttemptsUsed);
        Assert.AreEqual(1, attempts);
        Assert.AreEqual(0, flushes);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Clipboard")]
    [TestCategory("Cancellation")]
    public async Task CancellationDuringRetryReturnsCancelledWithoutFlush()
    {
        using var image = CreateImage();
        using var cancellation = new CancellationTokenSource();
        var attempts = 0;
        var flushes = 0;
        var adapter = new WinRtClipboardDeliveryAdapter(
            (_, _) =>
            {
                attempts++;
                cancellation.Cancel();
                return false;
            },
            () => flushes++);

        var result = await adapter.DeliverAsync(
            CreateRequest(image, maximumAttempts: 5, retryBudget: TimeSpan.FromSeconds(1)),
            cancellation.Token);

        var cancelled = result as ClipboardDeliveryResult.Cancelled;
        Assert.IsNotNull(cancelled);
        Assert.AreEqual("CancellationToken", cancelled.CancellationOrigin);
        Assert.AreEqual(1, attempts);
        Assert.AreEqual(0, flushes);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Clipboard")]
    public async Task ClipboardPublicationUsesDispatcherWhenCallerLacksThreadAccess()
    {
        using var image = CreateImage();
        var dispatches = 0;
        var publications = 0;
        var flushes = 0;
        var trace = new FakeCompleteExecutionTraceSink();
        var dispatcher = new FakeClipboardDeliveryDispatcher(
            hasThreadAccess: false,
            callback =>
            {
                dispatches++;
                callback();
                return true;
            });
        var adapter = new WinRtClipboardDeliveryAdapter(
            (_, _) =>
            {
                publications++;
                return true;
            },
            () => flushes++,
            traceSink: trace,
            dispatcher: dispatcher);

        var result = await adapter.DeliverAsync(
            CreateRequest(image, maximumAttempts: 1, retryBudget: TimeSpan.Zero),
            CancellationToken.None);

        Assert.IsInstanceOfType<ClipboardDeliveryResult.Delivered>(result);
        Assert.AreEqual(1, dispatches);
        Assert.AreEqual(1, publications);
        Assert.AreEqual(1, flushes);
        var diagnosticEvents = trace.Entries
            .Where(entry => entry.DiagnosticEvent is not null)
            .Select(entry => entry.DiagnosticEvent)
            .ToHashSet();
        CollectionAssert.IsSubsetOf(DispatcherDiagnosticEvents, diagnosticEvents.ToArray());
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Clipboard")]
    public async Task ClipboardPublicationInitializesRuntimeAroundSetContentAndFlush()
    {
        using var image = CreateImage();
        var calls = new List<string>();
        var initializer = new FakeClipboardRuntimeInitializer(calls);
        var adapter = new WinRtClipboardDeliveryAdapter(
            (_, _) =>
            {
                calls.Add("SetContent");
                return true;
            },
            () => calls.Add("Flush"),
            runtimeInitializer: initializer);

        var result = await adapter.DeliverAsync(
            CreateRequest(image, maximumAttempts: 1, retryBudget: TimeSpan.Zero),
            CancellationToken.None);

        Assert.IsInstanceOfType<ClipboardDeliveryResult.Delivered>(result);
        CollectionAssert.AreEqual(RuntimeInitializationCallOrder, calls);
        Assert.AreEqual(1, initializer.EnterCount);
        Assert.AreEqual(1, initializer.DisposeCount);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Clipboard")]
    public async Task UnavailableClipboardDispatcherReturnsTypedFailureWithoutPublishing()
    {
        using var image = CreateImage();
        var publications = 0;
        var flushes = 0;
        var dispatcher = new FakeClipboardDeliveryDispatcher(
            hasThreadAccess: false,
            _ => false);
        var adapter = new WinRtClipboardDeliveryAdapter(
            (_, _) =>
            {
                publications++;
                return true;
            },
            () => flushes++,
            dispatcher: dispatcher);

        var result = await adapter.DeliverAsync(
            CreateRequest(image, maximumAttempts: 1, retryBudget: TimeSpan.Zero),
            CancellationToken.None);

        var failure = result as ClipboardDeliveryResult.TerminalFailure;
        Assert.IsNotNull(failure);
        Assert.AreEqual(FailureCode.ClipboardPublicationRejected, failure.Failure.Code);
        Assert.AreEqual(0, publications);
        Assert.AreEqual(0, flushes);
    }

    private sealed class FakeClipboardDeliveryDispatcher : IClipboardDeliveryDispatcher
    {
        private readonly Func<Action, bool> _tryEnqueue;

        public FakeClipboardDeliveryDispatcher(
            bool hasThreadAccess,
            Func<Action, bool> tryEnqueue)
        {
            HasThreadAccess = hasThreadAccess;
            _tryEnqueue = tryEnqueue;
        }

        public bool HasThreadAccess { get; }

        public bool TryEnqueue(Action callback) => _tryEnqueue(callback);
    }

    private sealed class FakeCompleteExecutionTraceSink : ICompleteExecutionTraceSink
    {
        public List<CompleteExecutionTraceEntry> Entries { get; } = new();

        public void Record(CompleteExecutionTraceEntry entry) => Entries.Add(entry);
    }

    private sealed class FakeClipboardRuntimeInitializer : IClipboardRuntimeInitializer
    {
        private readonly List<string> _calls;

        public FakeClipboardRuntimeInitializer(List<string> calls)
        {
            _calls = calls;
        }

        public int EnterCount { get; private set; }

        public int DisposeCount { get; private set; }

        public IDisposable Enter()
        {
            EnterCount++;
            _calls.Add("Enter");
            return new Scope(this);
        }

        private sealed class Scope : IDisposable
        {
            private readonly FakeClipboardRuntimeInitializer _owner;
            private int _disposed;

            public Scope(FakeClipboardRuntimeInitializer owner)
            {
                _owner = owner;
            }

            public void Dispose()
            {
                if (Interlocked.Exchange(ref _disposed, 1) != 0)
                {
                    return;
                }

                _owner.DisposeCount++;
                _owner._calls.Add("Dispose");
            }
        }
    }

    private static ClipboardDeliveryRequest CreateRequest(
        SoftwareBitmapImageResult image,
        int maximumAttempts,
        TimeSpan retryBudget) => new()
        {
            DeliveryId = Guid.NewGuid(),
            SessionId = image.Metadata.SessionId,
            ResultId = image.Metadata.ResultId,
            ImageResult = image,
            MaximumAttempts = maximumAttempts,
            RetryBudget = retryBudget,
            HistoryAllowed = false,
            RoamingAllowed = false,
            Cancellation = CancellationToken.None
        };

    private static SoftwareBitmapImageResult CreateImage() =>
        SoftwareBitmapFactory.CreateFromPremultipliedBgra(
            new byte[] { 1, 2, 3, 255 },
            new ImageResultMetadata
            {
                ResultId = Guid.NewGuid(),
                SessionId = Guid.NewGuid(),
                PixelWidth = 1,
                PixelHeight = 1,
                PixelFormat = ImagePixelFormat.Bgra8,
                AlphaMode = ImageAlphaMode.Premultiplied,
                ColorSpace = ImageColorSpace.SrgbSdr,
                DpiX = 96,
                DpiY = 96,
                RowStride = 4,
                SourceKind = SourceKind.Monitor,
                SourcePhysicalBounds = new PhysicalRect(0, 0, 1, 1),
                CropPhysicalBounds = new PhysicalRect(0, 0, 1, 1),
                CapturedAt = DateTimeOffset.UnixEpoch
            });
}
