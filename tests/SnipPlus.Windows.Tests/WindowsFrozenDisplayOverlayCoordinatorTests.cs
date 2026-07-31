using System.Reflection;
using Microsoft.UI.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SnipPlus.Contracts;
using SnipPlus.Windows;

namespace SnipPlus.Windows.Tests;

[TestClass]
public sealed class WindowsFrozenDisplayOverlayCoordinatorTests
{
    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void OverlayUsesSystemCursorSurfaceWithoutDrawnCrosshairLines()
    {
        var overlaySurface = typeof(WindowsFrozenDisplayOverlayCoordinator)
            .GetNestedType("OverlaySurface", BindingFlags.NonPublic);

        Assert.IsNotNull(overlaySurface);
        Assert.IsNull(overlaySurface.GetField(
            "_crosshairHorizontal",
            BindingFlags.Instance | BindingFlags.NonPublic));
        Assert.IsNull(overlaySurface.GetField(
            "_crosshairVertical",
            BindingFlags.Instance | BindingFlags.NonPublic));

        var canvasField = overlaySurface.GetField(
            "_canvas",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(canvasField);
        Assert.AreEqual("CrosshairCanvas", canvasField.FieldType.Name);

        var cursorProperty = canvasField.FieldType.GetProperty(
            "Cursor",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        Assert.IsNotNull(cursorProperty);
        Assert.AreEqual(typeof(InputCursor), cursorProperty.PropertyType);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void OverlayDeclaresEightLogicalHandlesAndMapsCursors()
    {
        var overlaySurface = typeof(WindowsFrozenDisplayOverlayCoordinator)
            .GetNestedType("OverlaySurface", BindingFlags.NonPublic);

        Assert.IsNotNull(overlaySurface);
        var handlesField = overlaySurface.GetField(
            "_handles",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(handlesField);
        Assert.IsTrue(handlesField.FieldType.IsGenericType);
        Assert.AreEqual(
            typeof(IReadOnlyDictionary<SelectionHitTestKind, Microsoft.UI.Xaml.Shapes.Rectangle>),
            handlesField.FieldType);

        var cursorMapper = overlaySurface.GetMethod(
            "CursorShapeFor",
            BindingFlags.Static | BindingFlags.NonPublic);
        Assert.IsNotNull(cursorMapper);
        Assert.AreEqual(
            InputSystemCursorShape.Cross,
            cursorMapper.Invoke(null, new object[] { SelectionHitTestKind.Outside }));
        Assert.AreEqual(
            InputSystemCursorShape.SizeAll,
            cursorMapper.Invoke(null, new object[] { SelectionHitTestKind.Interior }));
        Assert.AreEqual(
            InputSystemCursorShape.SizeWestEast,
            cursorMapper.Invoke(null, new object[] { SelectionHitTestKind.LeftEdge }));
        Assert.AreEqual(
            InputSystemCursorShape.SizeNorthSouth,
            cursorMapper.Invoke(null, new object[] { SelectionHitTestKind.TopEdge }));
        Assert.AreEqual(
            InputSystemCursorShape.SizeNorthwestSoutheast,
            cursorMapper.Invoke(null, new object[] { SelectionHitTestKind.TopLeftCorner }));
        Assert.AreEqual(
            InputSystemCursorShape.SizeNortheastSouthwest,
            cursorMapper.Invoke(null, new object[] { SelectionHitTestKind.TopRightCorner }));
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void FunctionBarIsHostedInsideOverlayAndHasNoTopLevelWindowType()
    {
        var functionBarSurface = typeof(WindowsFrozenDisplayOverlayCoordinator)
            .GetNestedType("FunctionBarSurface", BindingFlags.NonPublic);

        Assert.IsNotNull(functionBarSurface);
        Assert.AreEqual(
            typeof(Microsoft.UI.Xaml.Controls.Border),
            functionBarSurface.GetField("_root", BindingFlags.Instance | BindingFlags.NonPublic)!.FieldType);
        Assert.IsNotNull(functionBarSurface.GetField(
            "_buttons",
            BindingFlags.Instance | BindingFlags.NonPublic));
        Assert.IsNotNull(functionBarSurface.GetMethod(
            "TryMeasurePhysicalSize",
            BindingFlags.Instance | BindingFlags.Public));
        Assert.IsNull(functionBarSurface.GetField(
            "_window",
            BindingFlags.Instance | BindingFlags.NonPublic));
        var toolButtons = functionBarSurface.GetField(
            "_toolButtons",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(toolButtons);
        Assert.AreEqual(
            typeof(IReadOnlyDictionary<EditingToolKind, Microsoft.UI.Xaml.Controls.RadioButton>),
            toolButtons.FieldType);
        Assert.IsNotNull(functionBarSurface.GetField(
            "_arrowLineModeButtons",
            BindingFlags.Instance | BindingFlags.NonPublic));
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void FunctionBarVisibilityPolicyKeepsPreparationLayoutParticipatingButNonInteractive()
    {
        var functionBarSurface = typeof(WindowsFrozenDisplayOverlayCoordinator)
            .GetNestedType("FunctionBarSurface", BindingFlags.NonPublic);

        Assert.IsNotNull(functionBarSurface);
        var getVisibilityState = functionBarSurface.GetMethod(
            "GetVisibilityState",
            BindingFlags.Static | BindingFlags.NonPublic);
        Assert.IsNotNull(getVisibilityState);

        var hidden = getVisibilityState.Invoke(null, new object[] { false })!;
        var shown = getVisibilityState.Invoke(null, new object[] { true })!;
        var stateType = hidden.GetType();

        Assert.IsTrue((bool)stateType.GetProperty("IsLayoutParticipating")!.GetValue(hidden)!);
        Assert.AreEqual(0d, (double)stateType.GetProperty("Opacity")!.GetValue(hidden)!);
        Assert.IsFalse((bool)stateType.GetProperty("IsHitTestVisible")!.GetValue(hidden)!);
        Assert.IsTrue((bool)stateType.GetProperty("IsLayoutParticipating")!.GetValue(shown)!);
        Assert.AreEqual(1d, (double)stateType.GetProperty("Opacity")!.GetValue(shown)!);
        Assert.IsTrue((bool)stateType.GetProperty("IsHitTestVisible")!.GetValue(shown)!);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void FunctionBarMeasurementConvertsDipToPhysicalPixelsDeterministically()
    {
        var functionBarSurface = typeof(WindowsFrozenDisplayOverlayCoordinator)
            .GetNestedType("FunctionBarSurface", BindingFlags.NonPublic);

        Assert.IsNotNull(functionBarSurface);
        var convert = functionBarSurface.GetMethod(
            "TryConvertToPhysicalSize",
            BindingFlags.Static | BindingFlags.NonPublic);
        Assert.IsNotNull(convert);

        var arguments = new object?[]
        {
            new global::Windows.Foundation.Size(120, 40),
            1.5,
            null
        };
        var converted = (bool)convert.Invoke(null, arguments)!;

        Assert.IsTrue(converted);
        Assert.AreEqual(new PhysicalPixelSize(180, 60), arguments[2]);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void FunctionBarUsesHighContrastButtonVisualPolicy()
    {
        var functionBarSurface = typeof(WindowsFrozenDisplayOverlayCoordinator)
            .GetNestedType("FunctionBarSurface", BindingFlags.NonPublic);

        Assert.IsNotNull(functionBarSurface);
        var getStyle = functionBarSurface.GetMethod(
            "GetButtonVisualStyle",
            BindingFlags.Static | BindingFlags.NonPublic);
        Assert.IsNotNull(getStyle);

        var style = getStyle.Invoke(null, null)!;
        var styleType = style.GetType();
        var foreground = (global::Windows.UI.Color)styleType
            .GetProperty("Foreground")!.GetValue(style)!;
        var background = (global::Windows.UI.Color)styleType
            .GetProperty("Background")!.GetValue(style)!;

        Assert.AreEqual(255, foreground.A);
        Assert.AreEqual(255, foreground.R);
        Assert.AreEqual(255, foreground.G);
        Assert.AreEqual(255, foreground.B);
        Assert.AreEqual(255, background.A);
        Assert.IsTrue(background.R < foreground.R);
        Assert.IsTrue(background.G < foreground.G);
        Assert.IsTrue(background.B < foreground.B);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void AnnotationPreviewIsOwnedByEachOverlayAndUsesPresentationSnapshot()
    {
        var overlaySurface = typeof(WindowsFrozenDisplayOverlayCoordinator)
            .GetNestedType("OverlaySurface", BindingFlags.NonPublic);

        Assert.IsNotNull(overlaySurface);
        var previewField = overlaySurface.GetField(
            "_annotationPreviews",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(previewField);
        Assert.AreEqual(
            typeof(List<Microsoft.UI.Xaml.Shapes.Rectangle>),
            previewField.FieldType);
        var arrowLinePreviewField = overlaySurface.GetField(
            "_arrowLinePreviews",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(arrowLinePreviewField);
        Assert.AreEqual(
            typeof(List<Microsoft.UI.Xaml.Shapes.Line>),
            arrowLinePreviewField.FieldType);
        var highlighterPreviewField = overlaySurface.GetField(
            "_highlighterPreviews",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(highlighterPreviewField);
        Assert.AreEqual(
            typeof(List<Microsoft.UI.Xaml.FrameworkElement>),
            highlighterPreviewField.FieldType);
        var apply = overlaySurface.GetMethod(
            "ApplyAnnotation",
            BindingFlags.Instance | BindingFlags.Public);
        Assert.IsNotNull(apply);
        var clip = overlaySurface.GetMethod(
            "TryClipLine",
            BindingFlags.Static | BindingFlags.NonPublic);
        Assert.IsNotNull(clip);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void ArrowLinePreviewClipsPhysicalSegmentToDisplaySelectionBounds()
    {
        var overlaySurface = typeof(WindowsFrozenDisplayOverlayCoordinator)
            .GetNestedType("OverlaySurface", BindingFlags.NonPublic);
        Assert.IsNotNull(overlaySurface);
        var clip = overlaySurface.GetMethod(
            "TryClipLine",
            BindingFlags.Static | BindingFlags.NonPublic);
        Assert.IsNotNull(clip);

        var arguments = new object?[]
        {
            new PhysicalLineSegment(
                new PhysicalPoint(-10, 10),
                new PhysicalPoint(30, 10)),
            new PhysicalRect(0, 0, 20, 20),
            null,
            null
        };
        var clipped = (bool)clip.Invoke(null, arguments)!;

        Assert.IsTrue(clipped);
        Assert.AreEqual(new PhysicalPoint(0, 10), arguments[2]);
        Assert.AreEqual(new PhysicalPoint(20, 10), arguments[3]);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void OverlayPresentationSnapshotKeepsDraftAndCommittedGeometrySeparate()
    {
        var sessionId = Guid.NewGuid();
        var document = new AnnotationDocument(
            sessionId,
            new AnnotationRevision(1),
            new[]
            {
                new AnnotationObject(
                    AnnotationObjectId.New(),
                    sessionId,
                    AnnotationToolKind.Rectangle,
                    new PhysicalRect(-10, 10, 20, 30),
                    0)
            });
        var snapshot = new AnnotationPresentationSnapshot(
            sessionId,
            "windows-v1",
            4,
            document.Revision,
            new PhysicalRect(0, 0, 10, 20),
            EditingToolKind.Rectangle,
            new PhysicalRect(2, 3, 8, 9),
            document);

        Assert.AreEqual(1, snapshot.Document.Objects.Count);
        Assert.AreEqual(new PhysicalRect(2, 3, 8, 9), snapshot.DraftPhysicalBounds);
        Assert.AreEqual(new PhysicalRect(0, 0, 10, 20), snapshot.SelectionPhysicalBounds);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void CancelCommandGateAcceptsOnlyOnePendingCancelAndCanReset()
    {
        var functionBarSurface = typeof(WindowsFrozenDisplayOverlayCoordinator)
            .GetNestedType("FunctionBarSurface", BindingFlags.NonPublic);
        Assert.IsNotNull(functionBarSurface);
        var gateType = functionBarSurface.GetNestedType(
            "CancelCommandGate",
            BindingFlags.NonPublic);
        Assert.IsNotNull(gateType);

        var gate = Activator.CreateInstance(
            gateType,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            binder: null,
            args: Array.Empty<object>(),
            culture: null)!;
        var tryBegin = gateType.GetMethod("TryBegin")!;
        var reset = gateType.GetMethod("Reset")!;

        Assert.IsTrue((bool)tryBegin.Invoke(gate, null)!);
        Assert.IsFalse((bool)tryBegin.Invoke(gate, null)!);
        reset.Invoke(gate, null);
        Assert.IsTrue((bool)tryBegin.Invoke(gate, null)!);
    }

    [TestMethod]
    [TestCategory("Unit")]
    [TestCategory("Contract")]
    public void SessionInputBoundaryNormalizesPointerIdsAndCommitsReleaseOnce()
    {
        var sessionId = Guid.NewGuid();
        var sink = new RecordingSelectionInputSink(sessionId, "boundary-v1");
        var boundaryType = typeof(WindowsFrozenDisplayOverlayCoordinator)
            .GetNestedType("SessionInputBoundary", BindingFlags.NonPublic);
        Assert.IsNotNull(boundaryType);

        var boundary = (ISelectionInputSink)Activator.CreateInstance(
            boundaryType,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            binder: null,
            args: new object[] { sessionId, "boundary-v1", sink },
            culture: null)!;

        boundary.PointerPressed(Input(sessionId, "boundary-v1", 10, 100, 100));
        boundary.PointerMoved(Input(sessionId, "boundary-v1", 42, 150, 150));

        var nativeRelease = boundaryType.GetMethod("PointerReleasedFromNative")!;
        var released = (SelectionInputResult)nativeRelease.Invoke(
            boundary,
            new object[] { new PhysicalPoint(200, 200) })!;
        var duplicate = boundary.PointerReleased(
            Input(sessionId, "boundary-v1", 77, 200, 200));

        Assert.AreEqual(SelectionInputResultKind.Locked, released.Kind);
        Assert.AreEqual(SelectionInputResultKind.Locked, duplicate.Kind);
        Assert.AreEqual(1, sink.ReleaseCalls.Count);
        Assert.AreEqual(10, sink.MovedCalls.Single().PointerId);
        Assert.AreEqual(10, sink.ReleaseCalls.Single().PointerId);
    }

    private static SelectionPointerEvent Input(
        Guid sessionId,
        string coordinateVersion,
        int pointerId,
        int x,
        int y) => new(
            sessionId,
            coordinateVersion,
            pointerId,
            new PhysicalPoint(x, y));

    private sealed class RecordingSelectionInputSink : ISelectionInputSink
    {
        private SelectionVisualState _state;

        public RecordingSelectionInputSink(Guid sessionId, string coordinateVersion)
        {
            _state = SelectionVisualState.Initial(sessionId, coordinateVersion);
        }

        public List<SelectionPointerEvent> MovedCalls { get; } = new();

        public List<SelectionPointerEvent> ReleaseCalls { get; } = new();

        public SelectionInputResult PointerPressed(SelectionPointerEvent input)
        {
            _state = _state with
            {
                SelectionRevision = _state.SelectionRevision + 1,
                Status = SelectionStatus.Dragging,
                InteractionMode = SelectionInteractionMode.InitialDragging,
                ActivePointerId = input.PointerId,
                CurrentPhysicalPoint = input.GlobalPhysicalPoint
            };
            return Result(SelectionInputResultKind.Dragging, "pressed");
        }

        public SelectionInputResult PointerMoved(SelectionPointerEvent input)
        {
            MovedCalls.Add(input);
            _state = _state with { CurrentPhysicalPoint = input.GlobalPhysicalPoint };
            return Result(SelectionInputResultKind.Dragging, "moved");
        }

        public SelectionInputResult PointerReleased(SelectionPointerEvent input)
        {
            ReleaseCalls.Add(input);
            _state = _state with
            {
                SelectionRevision = _state.SelectionRevision + 1,
                Status = SelectionStatus.Locked,
                InteractionMode = SelectionInteractionMode.Locked,
                ActivePointerId = null,
                CurrentPhysicalPoint = input.GlobalPhysicalPoint
            };
            return Result(SelectionInputResultKind.Locked, "released");
        }

        public SelectionInputResult Escape(Guid sessionId, string coordinateVersion)
        {
            _state = _state with
            {
                Status = SelectionStatus.Cancelled,
                InteractionMode = SelectionInteractionMode.Cancelled,
                ActivePointerId = null
            };
            return Result(SelectionInputResultKind.Cancelled, "escaped");
        }

        private SelectionInputResult Result(
            SelectionInputResultKind kind,
            string message) => new(kind, _state, message);
    }
}
