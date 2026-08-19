using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;
using SnipPlus.Windows;
using Windows.Storage;
using Windows.Storage.Streams;

namespace SnipPlus.Windows.Tests;

[TestClass]
public sealed class WindowsPngSaveServiceTests
{
    [TestMethod]
    [TestCategory("Output")]
    public async Task CancelledPickerDoesNotEncodeOrWrite()
    {
        var picker = new FakePicker(null);
        var encodeCalls = 0;
        var service = new WindowsPngSaveService(
            picker,
            (_, _) =>
            {
                encodeCalls++;
                return ValueTask.FromException<IRandomAccessStream>(
                    new InvalidOperationException("encoder should not run"));
            });

        var request = CreateRequest();
        var result = await service.SaveAsync(request, CancellationToken.None);

        Assert.IsInstanceOfType<PngSaveResult.SaveDialogCancelled>(result);
        Assert.AreEqual(0, encodeCalls);
        Assert.AreEqual(1, picker.CallCount);
    }

    [TestMethod]
    [TestCategory("Output")]
    public async Task EncodingFailureDoesNotProduceWriteEvidence()
    {
        var target = new FailingOpenTarget("capture.png");
        var picker = new FakePicker(target);
        var service = new WindowsPngSaveService(
            picker,
            (_, _) => ValueTask.FromException<IRandomAccessStream>(
                new InvalidOperationException("synthetic encoder failure")));

        var request = CreateRequest();
        var result = await service.SaveAsync(request, CancellationToken.None);

        var failed = (PngSaveResult.PngEncodingFailed)result;
        Assert.AreEqual(FailureCode.EncodingFailed, failed.Failure.Code);
        Assert.AreEqual(0, target.OpenCalls);
        Assert.AreEqual(1, target.DisposeCalls);
    }

    [TestMethod]
    [TestCategory("Output")]
    public async Task SuccessfulSaveWritesExactEncodedBytesAndMatchingEvidence()
    {
        var path = Path.Combine(Path.GetTempPath(), $"SnipPlus-{Guid.NewGuid():N}.png");
        File.WriteAllBytes(path, Array.Empty<byte>());
        try
        {
            var target = new TempFileTarget(path);
            var picker = new FakePicker(target);
            var payload = Encoding.ASCII.GetBytes("synthetic-png-bytes");
            var service = new WindowsPngSaveService(
                picker,
                (_, cancellationToken) => CreateEncodedStreamAsync(payload, cancellationToken));

            var request = CreateRequest() with { SuggestedFileName = "SnipPlus_2026-08-19_070605.png" };
            var result = await service.SaveAsync(request, CancellationToken.None);

            var saved = (PngSaveResult.Saved)result;
            CollectionAssert.AreEqual(payload, File.ReadAllBytes(path));
            Assert.AreEqual(request.SessionId, saved.PngWrite.SessionId);
            Assert.AreEqual(request.ResultId, saved.PngWrite.ResultId);
            Assert.AreEqual("synthetic-target.png", saved.DestinationIdentifier);
            Assert.AreEqual(1, target.OpenCalls);
            Assert.AreEqual(1, target.DisposeCalls);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [TestMethod]
    [TestCategory("Output")]
    public async Task NonPngTargetIsRejectedBeforeEncoding()
    {
        var target = new FailingOpenTarget("capture.jpg");
        var encodeCalls = 0;
        var service = new WindowsPngSaveService(
            new FakePicker(target),
            (_, _) =>
            {
                encodeCalls++;
                return ValueTask.FromException<IRandomAccessStream>(
                    new InvalidOperationException("encoder should not run"));
            });

        var result = await service.SaveAsync(CreateRequest(), CancellationToken.None);

        var failed = (PngSaveResult.PngWriteFailed)result;
        Assert.AreEqual(FailureCode.OutputWriteFailed, failed.Failure.Code);
        Assert.AreEqual(0, encodeCalls);
        Assert.AreEqual(0, target.OpenCalls);
        Assert.AreEqual(1, target.DisposeCalls);
    }

    [TestMethod]
    [TestCategory("Output")]
    public async Task WriteFailureDoesNotProduceWriteEvidence()
    {
        var target = new FailingOpenTarget("capture.png");
        var service = new WindowsPngSaveService(
            new FakePicker(target),
            (_, cancellationToken) => CreateEncodedStreamAsync(
                Encoding.ASCII.GetBytes("synthetic-png-bytes"),
                cancellationToken));

        var result = await service.SaveAsync(CreateRequest(), CancellationToken.None);

        var failed = (PngSaveResult.PngWriteFailed)result;
        Assert.AreEqual(FailureCode.OutputWriteFailed, failed.Failure.Code);
        Assert.AreEqual(1, target.OpenCalls);
        Assert.AreEqual(1, target.DisposeCalls);
    }

    private static PngSaveRequest CreateRequest()
    {
        var sessionId = Guid.NewGuid();
        var resultId = Guid.NewGuid();
        return new PngSaveRequest
        {
            SessionId = sessionId,
            ResultId = resultId,
            ImageResult = new SyntheticImageResult(sessionId, resultId),
            SuggestedFileName = "SnipPlus_2026-08-19_070605.png"
        };
    }

    private static async ValueTask<IRandomAccessStream> CreateEncodedStreamAsync(
        byte[] bytes,
        CancellationToken cancellationToken)
    {
        var stream = new InMemoryRandomAccessStream();
        var output = stream.GetOutputStreamAt(0);
        using var writer = new DataWriter(output);
        writer.WriteBytes(bytes);
        await writer.StoreAsync().AsTask(cancellationToken);
        writer.DetachStream();
        stream.Seek(0);
        return stream;
    }

    private sealed class FakePicker : IWindowsPngSavePicker
    {
        private readonly IWindowsPngSaveTarget? _target;

        public FakePicker(IWindowsPngSaveTarget? target)
        {
            _target = target;
        }

        public int CallCount { get; private set; }

        public ValueTask<IWindowsPngSaveTarget?> PickAsync(
            string suggestedFileName,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            CallCount++;
            return ValueTask.FromResult(_target);
        }
    }

    private sealed class TempFileTarget : IWindowsPngSaveTarget
    {
        private readonly string _path;

        public TempFileTarget(string path)
        {
            _path = path;
        }

        public string FileName => Path.GetFileName(_path);

        public string Identifier => "synthetic-target.png";

        public int OpenCalls { get; private set; }

        public int DisposeCalls { get; private set; }

        public async ValueTask<IRandomAccessStream> OpenWriteAsync(
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            OpenCalls++;
            var file = await StorageFile.GetFileFromPathAsync(_path).AsTask(cancellationToken);
            return await file.OpenAsync(FileAccessMode.ReadWrite).AsTask(cancellationToken);
        }

        public void Dispose()
        {
            DisposeCalls++;
        }
    }

    private sealed class FailingOpenTarget : IWindowsPngSaveTarget
    {
        public FailingOpenTarget(string fileName)
        {
            FileName = fileName;
        }

        public string FileName { get; }

        public string Identifier => "synthetic-target";

        public int OpenCalls { get; private set; }

        public int DisposeCalls { get; private set; }

        public ValueTask<IRandomAccessStream> OpenWriteAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            OpenCalls++;
            return ValueTask.FromException<IRandomAccessStream>(
                new IOException("synthetic write failure"));
        }

        public void Dispose()
        {
            DisposeCalls++;
        }
    }

    private sealed class SyntheticImageResult : IImageResult
    {
        public SyntheticImageResult(Guid sessionId, Guid resultId)
        {
            Metadata = new ImageResultMetadata
            {
                SessionId = sessionId,
                ResultId = resultId,
                SelectionRevision = 1,
                AnnotationRevision = AnnotationRevision.Initial,
                PixelWidth = 2,
                PixelHeight = 2,
                PixelFormat = ImagePixelFormat.Bgra8,
                AlphaMode = ImageAlphaMode.Premultiplied,
                ColorSpace = ImageColorSpace.SrgbSdr,
                DpiX = 96,
                DpiY = 96,
                RowStride = 8,
                SourceKind = SourceKind.Monitor,
                SourcePhysicalBounds = new PhysicalRect(0, 0, 2, 2),
                CropPhysicalBounds = new PhysicalRect(0, 0, 2, 2),
                CapturedAt = DateTimeOffset.UnixEpoch
            };
        }

        public ImageResultMetadata Metadata { get; }

        public bool IsDisposed => false;

        public IImageResultLease AcquireLease() =>
            throw new NotSupportedException("Synthetic encoder does not acquire a bitmap lease.");

        public void Dispose()
        {
        }
    }
}
