using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;

namespace SnipPlus.Contracts.Tests;

[TestClass]
public sealed class CapacityAndSessionContractTests
{
    [TestMethod]
    [TestCategory("Contract")]
    public void OwnerReferenceTopologySupportsNegativeCoordinatesMixedDpiAndTransparentGaps()
    {
        var snapshot = CreateOwnerReferenceSnapshot();

        var result = new SupportedCapacityPolicy().ValidateTopology(snapshot);

        Assert.IsTrue(result.IsSupported);
        Assert.AreEqual(GapPolicy.Transparent, snapshot.GapPolicy);
        Assert.AreEqual(new PhysicalPoint(-2560, 0), snapshot.VirtualOrigin);
        Assert.AreEqual(1.5, snapshot.Displays.Single(display => display.DisplayId == "lower").DpiScaleX);
        Assert.IsFalse(snapshot.Displays.Any(display => display.PhysicalBoundsInVirtualDesktop.Contains(new PhysicalRect(1920, 1500, 2560, 2000))));
    }

    [TestMethod]
    [TestCategory("Contract")]
    public void FourMaximumDisplaysAreSupported()
    {
        var displays = new[]
        {
            Display("a", new(0, 0, 3840, 2160)),
            Display("b", new(3840, 0, 7680, 2160)),
            Display("c", new(0, 2160, 3840, 4320)),
            Display("d", new(3840, 2160, 7680, 4320))
        };
        var snapshot = new VirtualDesktopSnapshot(
            "maximum-v1",
            new(0, 0, 7680, 4320),
            new(0, 0),
            displays);

        var result = new SupportedCapacityPolicy().ValidateTopology(snapshot);

        Assert.IsTrue(result.IsSupported);
    }

    [TestMethod]
    [TestCategory("Contract")]
    public void DisplayCountAndDimensionsProduceTypedFailures()
    {
        var policy = new SupportedCapacityPolicy();
        var empty = new VirtualDesktopSnapshot("empty-v1", new(0, 0, 100, 100), new(0, 0), Array.Empty<DisplaySnapshot>());
        var five = Enumerable.Range(0, 5)
            .Select(index => Display(index.ToString(CultureInfo.InvariantCulture), new(index * 100, 0, (index + 1) * 100, 100)))
            .ToArray();
        var fiveSnapshot = new VirtualDesktopSnapshot("five-v1", new(0, 0, 500, 100), new(0, 0), five);
        var eightK = new VirtualDesktopSnapshot(
            "8k-v1",
            new(0, 0, 7680, 4320),
            new(0, 0),
            new[] { Display("8k", new(0, 0, 7680, 4320)) });

        Assert.AreEqual(CapacityValidationKind.UnsupportedDisplayCount, policy.ValidateTopology(empty).Kind);
        Assert.AreEqual(CapacityValidationKind.UnsupportedDisplayCount, policy.ValidateTopology(fiveSnapshot).Kind);
        Assert.AreEqual(CapacityValidationKind.UnsupportedDisplayDimensions, policy.ValidateTopology(eightK).Kind);
    }

    [TestMethod]
    [TestCategory("Contract")]
    public void TotalSourceAndVirtualDesktopLimitsAreIndependentTypedFailures()
    {
        var displays = new[]
        {
            Display("a", new(0, 0, 100, 100)),
            Display("b", new(100, 0, 200, 100))
        };
        var snapshot = new VirtualDesktopSnapshot("limits-v1", new(0, 0, 20_000, 100), new(0, 0), displays);
        var policy = new SupportedCapacityPolicy { MaximumTotalSourcePixels = 1 };

        Assert.AreEqual(CapacityValidationKind.UnsupportedVirtualDesktopBounds, new SupportedCapacityPolicy().ValidateTopology(snapshot).Kind);
        Assert.AreEqual(CapacityValidationKind.UnsupportedTotalSourcePixels, policy.ValidateTopology(
            new VirtualDesktopSnapshot("pixels-v1", new(0, 0, 200, 100), new(0, 0), displays)).Kind);
    }

    [TestMethod]
    [TestCategory("Contract")]
    public void SelectionDimensionsAndAreaAreValidatedSeparately()
    {
        var policy = new SupportedCapacityPolicy();

        var dimensions = policy.ValidateSelection(new(0, 0, 16_385, 100));
        var area = policy.ValidateSelection(new(0, 0, 16_384, 4_097));

        Assert.AreEqual(CapacityValidationKind.UnsupportedSelectionDimensions, dimensions.Kind);
        Assert.AreEqual(16_385L, dimensions.ActualValue);
        Assert.AreEqual(SupportedCapacityPolicy.MaxSelectionWidth, dimensions.LimitValue);
        Assert.AreEqual(CapacityValidationKind.UnsupportedSelectionArea, area.Kind);
        Assert.AreEqual(16_384L * 4_097L, area.ActualValue);
        Assert.AreEqual(SupportedCapacityPolicy.MaxSelectionArea, area.LimitValue);
    }

    [TestMethod]
    [TestCategory("Contract")]
    public void SnapshotRejectsDuplicateIdsInvalidDpiInvalidBoundsAndOutOfBoundsDisplay()
    {
        var valid = Display("one", new(0, 0, 100, 100));

        AssertArgumentException(() => CreateDuplicateSnapshot(valid));
        AssertArgumentException(() => _ = CreateInvalidDpiSnapshot());
        AssertArgumentException(() => _ = CreateOutOfBoundsSnapshot());
    }

    [TestMethod]
    [TestCategory("Contract")]
    public void SnapshotDisplaysAreReadOnlyAndGapPolicyDoesNotInventFrames()
    {
        var snapshot = CreateOwnerReferenceSnapshot();

        Assert.IsInstanceOfType<IReadOnlyList<DisplaySnapshot>>(snapshot.Displays);
        AssertNotSupported(() => ((IList<DisplaySnapshot>)snapshot.Displays)[0] = Display("changed", new(0, 0, 1, 1)));
        Assert.AreEqual(GapPolicy.Transparent, snapshot.GapPolicy);
    }

    [TestMethod]
    [TestCategory("Contract")]
    public void SessionPreservesRequestIdentityAndDisposesAttachedFramesIdempotently()
    {
        var request = CaptureRequest.CreateSecondary(Guid.NewGuid(), DateTimeOffset.UnixEpoch);
        var snapshot = new VirtualDesktopSnapshot(
            "session-v1",
            new(0, 0, 2, 2),
            new(0, 0),
            new[] { Display("one", new(0, 0, 2, 2)) });
        var session = new CaptureSessionContext(
            request,
            snapshot,
            CapacityValidationOutcome.Supported(),
            new ForegroundContextReference("synthetic", DateTimeOffset.UnixEpoch),
            CancellationToken.None);
        var image = new SessionImageResult(session.SessionId);
        var frame = new FrozenDisplayFrame(
            session.SessionId,
            "one",
            Guid.NewGuid(),
            snapshot.CoordinateVersion,
            new(0, 0, 2, 2),
            new(2, 2),
            new FrozenCaptureFrame(image));

        Assert.IsTrue(FrozenDisplayFrameSet.TryCreate(session, snapshot.Displays, new[] { frame }, out var frameSet, out var validation));
        Assert.IsTrue(validation.IsValid);
        Assert.IsTrue(session.TryAttachFrozenDisplayFrames(frameSet!));
        Assert.AreEqual(request.RequestId, session.RequestId);
        Assert.AreEqual(request.RequestedAt, session.RequestedAt);
        Assert.AreEqual(CaptureSessionStatus.FrozenFrameSetReady, session.Status);

        session.Dispose();
        session.Dispose();

        Assert.IsTrue(session.IsDisposed);
        Assert.IsTrue(image.IsDisposed);
    }

    [TestMethod]
    [TestCategory("Contract")]
    public void FrameSetRejectsDuplicateMissingAndUnknownDisplays()
    {
        var duplicateFixture = CreateFrameFixture();
        var duplicateFirst = CreateFrame(duplicateFixture.Session, "one", duplicateFixture.Snapshot.Displays[0]);
        var duplicateSecond = CreateFrame(duplicateFixture.Session, "one", duplicateFixture.Snapshot.Displays[0]);
        Assert.IsFalse(FrozenDisplayFrameSet.TryCreate(
            duplicateFixture.Session,
            duplicateFixture.Snapshot.Displays,
            new[] { duplicateFirst.Frame, duplicateSecond.Frame },
            out _,
            out var duplicateValidation));
        Assert.AreEqual(FrozenDisplayFrameSetFailureKind.DuplicateDisplay, duplicateValidation.FailureKind);
        Assert.IsTrue(duplicateFirst.Image.IsDisposed);
        Assert.IsTrue(duplicateSecond.Image.IsDisposed);

        var missingFixture = CreateFrameFixture();
        var missing = CreateFrame(missingFixture.Session, "one", missingFixture.Snapshot.Displays[0]);
        Assert.IsFalse(FrozenDisplayFrameSet.TryCreate(
            missingFixture.Session,
            missingFixture.Snapshot.Displays,
            new[] { missing.Frame },
            out _,
            out var missingValidation));
        Assert.AreEqual(FrozenDisplayFrameSetFailureKind.MissingDisplay, missingValidation.FailureKind);

        var unknownFixture = CreateFrameFixture();
        var unknown = CreateFrame(unknownFixture.Session, "unknown", unknownFixture.Snapshot.Displays[0]);
        Assert.IsFalse(FrozenDisplayFrameSet.TryCreate(
            unknownFixture.Session,
            unknownFixture.Snapshot.Displays,
            new[] { unknown.Frame },
            out _,
            out var unknownValidation));
        Assert.AreEqual(FrozenDisplayFrameSetFailureKind.UnknownDisplay, unknownValidation.FailureKind);
    }

    [TestMethod]
    [TestCategory("Contract")]
    public void FrameSetRejectsSessionCoordinateBoundsAndPixelMismatches()
    {
        var sessionFixture = CreateFrameFixture();
        var wrongSessionId = Guid.NewGuid();
        var sessionMismatch = CreateFrame(
            sessionFixture.Session,
            "one",
            sessionFixture.Snapshot.Displays[0],
            imageSessionId: wrongSessionId,
            frameSessionId: wrongSessionId);
        Assert.IsFalse(FrozenDisplayFrameSet.TryCreate(
            sessionFixture.Session,
            sessionFixture.Snapshot.Displays,
            new[] { sessionMismatch.Frame },
            out _,
            out var sessionValidation));
        Assert.AreEqual(FrozenDisplayFrameSetFailureKind.SessionMismatch, sessionValidation.FailureKind);

        var coordinateFixture = CreateFrameFixture();
        var coordinateMismatch = CreateFrame(
            coordinateFixture.Session,
            "one",
            coordinateFixture.Snapshot.Displays[0],
            coordinateVersion: "other-coordinate");
        Assert.IsFalse(FrozenDisplayFrameSet.TryCreate(
            coordinateFixture.Session,
            coordinateFixture.Snapshot.Displays,
            new[] { coordinateMismatch.Frame },
            out _,
            out var coordinateValidation));
        Assert.AreEqual(FrozenDisplayFrameSetFailureKind.CoordinateVersionMismatch, coordinateValidation.FailureKind);

        var boundsFixture = CreateFrameFixture();
        var boundsMismatch = CreateFrame(
            boundsFixture.Session,
            "one",
            boundsFixture.Snapshot.Displays[0],
            bounds: new(0, 0, 1, 2));
        Assert.IsFalse(FrozenDisplayFrameSet.TryCreate(
            boundsFixture.Session,
            boundsFixture.Snapshot.Displays,
            new[] { boundsMismatch.Frame },
            out _,
            out var boundsValidation));
        Assert.AreEqual(FrozenDisplayFrameSetFailureKind.BoundsMismatch, boundsValidation.FailureKind);

        var pixelFixture = CreateFrameFixture();
        var pixelMismatch = CreateFrame(
            pixelFixture.Session,
            "one",
            pixelFixture.Snapshot.Displays[0],
            pixelSize: new(3, 2));
        Assert.IsFalse(FrozenDisplayFrameSet.TryCreate(
            pixelFixture.Session,
            pixelFixture.Snapshot.Displays,
            new[] { pixelMismatch.Frame },
            out _,
            out var pixelValidation));
        Assert.AreEqual(FrozenDisplayFrameSetFailureKind.PixelSizeMismatch, pixelValidation.FailureKind);
    }

    private static VirtualDesktopSnapshot CreateOwnerReferenceSnapshot() => new(
        "owner-reference-v1",
        new(-2560, 0, 2560, 2520),
        new(-2560, 0),
        new[]
        {
            Display("primary", new(0, 0, 2560, 1440)),
            Display("lower", new(0, 1440, 1920, 2520), 1.5),
            Display("left", new(-2560, 0, 0, 1440))
        });

    private static DisplaySnapshot Display(string id, PhysicalRect bounds, double dpi = 1) => new(
        id,
        bounds,
        dpi,
        dpi,
        "Landscape",
        new(bounds.Width, bounds.Height),
        $"surface-{id}");

    private static (CaptureSessionContext Session, VirtualDesktopSnapshot Snapshot) CreateFrameFixture()
    {
        var request = CaptureRequest.CreateSecondary(Guid.NewGuid(), DateTimeOffset.UnixEpoch);
        var snapshot = new VirtualDesktopSnapshot(
            "frame-set-v1",
            new(0, 0, 4, 2),
            new(0, 0),
            new[] { Display("one", new(0, 0, 2, 2)), Display("two", new(2, 0, 4, 2)) });
        return (new CaptureSessionContext(request, snapshot, CapacityValidationOutcome.Supported(), null, CancellationToken.None), snapshot);
    }

    private static (FrozenDisplayFrame Frame, SessionImageResult Image) CreateFrame(
        CaptureSessionContext session,
        string displayId,
        DisplaySnapshot display,
        Guid? imageSessionId = null,
        Guid? frameSessionId = null,
        string? coordinateVersion = null,
        PhysicalRect? bounds = null,
        PhysicalPixelSize? pixelSize = null)
    {
        var actualBounds = bounds ?? display.PhysicalBoundsInVirtualDesktop;
        var actualPixelSize = pixelSize ?? display.ExpectedFrozenFramePixelSize;
        var image = new SessionImageResult(
            imageSessionId ?? session.SessionId,
            actualPixelSize.Width,
            actualPixelSize.Height,
            actualBounds);
        var frame = new FrozenDisplayFrame(
            frameSessionId ?? session.SessionId,
            displayId,
            Guid.NewGuid(),
            coordinateVersion ?? session.VirtualDesktopSnapshot.CoordinateVersion,
            actualBounds,
            actualPixelSize,
            new FrozenCaptureFrame(image));
        return (frame, image);
    }

    private static void AssertArgumentException(Action action)
    {
        var threw = false;
        try
        {
            action();
        }
        catch (ArgumentException)
        {
            threw = true;
        }

        Assert.IsTrue(threw, "Expected an ArgumentException.");
    }

    private static VirtualDesktopSnapshot CreateDuplicateSnapshot(DisplaySnapshot valid) => new(
        "duplicate-v1",
        new(0, 0, 200, 100),
        new(0, 0),
        new[] { valid, Display("one", new(100, 0, 200, 100)) });

    private static VirtualDesktopSnapshot CreateInvalidDpiSnapshot() => new(
        "dpi-v1",
        new(0, 0, 100, 100),
        new(0, 0),
        new[] { new DisplaySnapshot("dpi", new(0, 0, 100, 100), double.NaN, 1, "Landscape", new(100, 100), "surface") });

    private static VirtualDesktopSnapshot CreateOutOfBoundsSnapshot() => new(
        "bounds-v1",
        new(0, 0, 100, 100),
        new(0, 0),
        new[] { Display("outside", new(90, 90, 110, 110)) });

    private static void AssertNotSupported(Action action)
    {
        var threw = false;
        try
        {
            action();
        }
        catch (NotSupportedException)
        {
            threw = true;
        }

        Assert.IsTrue(threw, "Expected a NotSupportedException.");
    }

    private sealed class SessionImageResult : IImageResult
    {
        public SessionImageResult(
            Guid sessionId,
            int pixelWidth = 2,
            int pixelHeight = 2,
            PhysicalRect? sourceBounds = null)
        {
            Metadata = new ImageResultMetadata
            {
                ResultId = Guid.NewGuid(),
                SessionId = sessionId,
                PixelWidth = pixelWidth,
                PixelHeight = pixelHeight,
                PixelFormat = ImagePixelFormat.Bgra8,
                AlphaMode = ImageAlphaMode.Premultiplied,
                ColorSpace = ImageColorSpace.SrgbSdr,
                DpiX = 96,
                DpiY = 96,
                RowStride = checked(pixelWidth * 4),
                SourceKind = SourceKind.Monitor,
                SourcePhysicalBounds = sourceBounds ?? new(0, 0, pixelWidth, pixelHeight),
                CropPhysicalBounds = sourceBounds ?? new(0, 0, pixelWidth, pixelHeight),
                CapturedAt = DateTimeOffset.UnixEpoch
            };
        }

        public ImageResultMetadata Metadata { get; }

        public bool IsDisposed { get; private set; }

        public IImageResultLease AcquireLease() => throw new NotSupportedException();

        public void Dispose() => IsDisposed = true;
    }
}
