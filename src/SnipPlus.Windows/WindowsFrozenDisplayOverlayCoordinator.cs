using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Globalization;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Input;
using Microsoft.UI.Text;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using SnipPlus.Contracts;
using Windows.Foundation;
using Windows.Graphics;
using Windows.Graphics.Imaging;
using WinRT.Interop;

namespace SnipPlus.Windows;

public sealed class WindowsFrozenDisplayOverlayCoordinator :
    IAllDisplayOverlayPresentationCoordinator,
    IFunctionBarPresentationCoordinator
{
    private readonly object _gate = new();
    private readonly Dictionary<Guid, IReadOnlyList<OverlaySurface>> _sessions = new();
    private readonly Dictionary<Guid, FunctionBarSessionPresentation> _functionBars = new();
    private readonly IFunctionBarPlacementService _placementService;
    private readonly ICompleteExecutionTraceSink? _traceSink;
    private bool _disposed;

    public WindowsFrozenDisplayOverlayCoordinator(
        IFunctionBarPlacementService placementService,
        ICompleteExecutionTraceSink? traceSink = null)
    {
        _placementService = placementService
            ?? throw new ArgumentNullException(nameof(placementService));
        _traceSink = traceSink;
    }

    public async ValueTask<FrozenDisplayOverlayPresentationOutcome> PresentAsync(
        FrozenDisplayOverlayPresentationRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        var inputBoundary = new SessionInputBoundary(
            request.Plan.SessionId,
            request.Plan.CoordinateVersion,
            request.InputSink,
            request.EditingInputRouter);
        var surfaces = new List<OverlaySurface>(request.Plan.Displays.Count);
        try
        {
            if (request.Plan.Displays.Count == 0
                || request.Plan.Displays.Select(display => display.DisplayId)
                    .Distinct(StringComparer.Ordinal).Count() != request.Plan.Displays.Count)
            {
                return Failed(request.Plan.SessionId, FailureCode.OverlayCreationFailed,
                    "The overlay plan must contain one unique surface for every display.");
            }

            foreach (var descriptor in request.Plan.Displays)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var surface = new OverlaySurface(descriptor, inputBoundary, _traceSink);
                surfaces.Add(surface);
                await surface.InitializeAsync(cancellationToken).ConfigureAwait(true);
            }

            lock (_gate)
            {
                if (_disposed)
                {
                    DisposeSurfaces(surfaces);
                    return new FrozenDisplayOverlayPresentationOutcome.Cancelled("ApplicationExiting");
                }

                if (_sessions.ContainsKey(request.Plan.SessionId))
                {
                    DisposeSurfaces(surfaces);
                    return Failed(request.Plan.SessionId, FailureCode.OverlayCreationFailed,
                        "An overlay session already exists for this capture session.");
                }

                _sessions.Add(request.Plan.SessionId, surfaces);
            }

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                OverlaySurface.ShowAll(surfaces);
            }
            catch (OperationCanceledException)
            {
                await CloseAsync(request.Plan.SessionId, CancellationToken.None).ConfigureAwait(true);
                return new FrozenDisplayOverlayPresentationOutcome.Cancelled("CancellationToken");
            }
            catch (Exception exception)
            {
                await CloseAsync(request.Plan.SessionId, CancellationToken.None).ConfigureAwait(true);
                return Failed(
                    request.Plan.SessionId,
                    FailureCode.OverlayPresentationFailed,
                    exception.GetType().Name,
                    exception.HResult);
            }

            return new FrozenDisplayOverlayPresentationOutcome.Ready();
        }
        catch (OperationCanceledException)
        {
            DisposeSurfaces(surfaces);
            return new FrozenDisplayOverlayPresentationOutcome.Cancelled("CancellationToken");
        }
        catch (Exception exception)
        {
            DisposeSurfaces(surfaces);
            return Failed(
                request.Plan.SessionId,
                FailureCode.OverlayCreationFailed,
                exception.GetType().Name,
                exception.HResult);
        }
    }

    public FunctionBarPresentationResult Prepare(FunctionBarPresentationRequest request) =>
        PrepareOrReposition(request, allowExisting: false);

    public FunctionBarPresentationResult Reposition(FunctionBarPresentationRequest request) =>
        PrepareOrReposition(request, allowExisting: true);

    public FunctionBarPresentationResult Show(
        Guid sessionId,
        string coordinateVersion,
        int selectionRevision)
    {
        lock (_gate)
        {
            if (!_functionBars.TryGetValue(sessionId, out var presentation))
            {
                return FunctionBarResult(
                    FunctionBarPresentationResultKind.StaleSession,
                    sessionId,
                    coordinateVersion,
                    selectionRevision,
                    null,
                    null,
                    "The Function Bar session is no longer active.");
            }

            if (!string.Equals(
                    presentation.Request.CoordinateVersion,
                    coordinateVersion,
                    StringComparison.Ordinal)
                || selectionRevision != presentation.Request.Selection.SelectionRevision)
            {
                return FunctionBarResult(
                    FunctionBarPresentationResultKind.StaleSelectionRevision,
                    sessionId,
                    coordinateVersion,
                    selectionRevision,
                    presentation.Placement,
                    null,
                    "The Function Bar show request is stale.");
            }

            presentation.Surface.ClearFeedback();
            presentation.Surface.SetVisible(true);
            return FunctionBarResult(
                FunctionBarPresentationResultKind.Shown,
                sessionId,
                coordinateVersion,
                selectionRevision,
                presentation.Placement,
                null,
                "The Function Bar is visible.");
        }
    }

    public FunctionBarPresentationResult ShowFeedback(
        Guid sessionId,
        string coordinateVersion,
        int selectionRevision,
        string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        lock (_gate)
        {
            if (!_functionBars.TryGetValue(sessionId, out var presentation))
            {
                return FunctionBarResult(
                    FunctionBarPresentationResultKind.StaleSession,
                    sessionId,
                    coordinateVersion,
                    selectionRevision,
                    null,
                    null,
                    "The Function Bar feedback session is no longer active.");
            }

            if (!string.Equals(
                    presentation.Request.CoordinateVersion,
                    coordinateVersion,
                    StringComparison.Ordinal)
                || selectionRevision != presentation.Request.Selection.SelectionRevision)
            {
                return FunctionBarResult(
                    FunctionBarPresentationResultKind.StaleSelectionRevision,
                    sessionId,
                    coordinateVersion,
                    selectionRevision,
                    presentation.Placement,
                    null,
                    "The Function Bar feedback request is stale.");
            }

            presentation.Surface.ShowFeedback(message);
            presentation.Surface.SetVisible(true);
            return FunctionBarResult(
                FunctionBarPresentationResultKind.Shown,
                sessionId,
                coordinateVersion,
                selectionRevision,
                presentation.Placement,
                null,
                message);
        }
    }

    public FunctionBarPresentationResult Hide(Guid sessionId)
    {
        lock (_gate)
        {
            if (!_functionBars.TryGetValue(sessionId, out var presentation))
            {
                return FunctionBarResult(
                    FunctionBarPresentationResultKind.Hidden,
                    sessionId,
                    string.Empty,
                    0,
                    null,
                    null,
                    "The Function Bar was already hidden.");
            }

            presentation.Surface.SetVisible(false);
            return FunctionBarResult(
                FunctionBarPresentationResultKind.Hidden,
                sessionId,
                presentation.Request.CoordinateVersion,
                presentation.Request.Selection.SelectionRevision,
                presentation.Placement,
                null,
                "The Function Bar is hidden while the Selection is adjusted.");
        }
    }

    public FunctionBarPresentationResult Close(Guid sessionId)
    {
        FunctionBarSessionPresentation? presentation;
        lock (_gate)
        {
            _functionBars.Remove(sessionId, out presentation);
        }

        presentation?.Dispose();
        return FunctionBarResult(
            FunctionBarPresentationResultKind.Closed,
            sessionId,
            presentation?.Request.CoordinateVersion ?? string.Empty,
            presentation?.Request.Selection.SelectionRevision ?? 0,
            presentation?.Placement,
            null,
            "The Function Bar is closed.");
    }

    public void ApplySelection(SelectionVisualState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        IReadOnlyList<OverlaySurface>? surfaces;
        lock (_gate)
        {
            _sessions.TryGetValue(state.SessionId, out surfaces);
        }

        if (surfaces is null)
        {
            return;
        }

        foreach (var surface in surfaces)
        {
            surface.ApplySelection(state);
        }
    }

    public void ApplyAnnotation(AnnotationPresentationSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        IReadOnlyList<OverlaySurface>? surfaces;
        lock (_gate)
        {
            _sessions.TryGetValue(snapshot.SessionId, out surfaces);
        }

        if (surfaces is null)
        {
            return;
        }

        foreach (var surface in surfaces)
        {
            surface.ApplyAnnotation(snapshot);
        }
    }

    public async ValueTask CloseAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Close(sessionId);
        IReadOnlyList<OverlaySurface>? surfaces;
        lock (_gate)
        {
            _sessions.Remove(sessionId, out surfaces);
        }

        if (surfaces is null)
        {
            return;
        }

        foreach (var surface in surfaces)
        {
            cancellationToken.ThrowIfCancellationRequested();
            surface.Dispose();
        }

        await ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        IReadOnlyList<OverlaySurface>[] sessions;
        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            sessions = _sessions.Values.ToArray();
            _sessions.Clear();
            var functionBars = _functionBars.Values.ToArray();
            _functionBars.Clear();
            foreach (var functionBar in functionBars)
            {
                functionBar.Dispose();
            }
        }

        foreach (var surfaces in sessions)
        {
            DisposeSurfaces(surfaces);
        }

        GC.SuppressFinalize(this);
    }

    private static void DisposeSurfaces(IEnumerable<OverlaySurface> surfaces)
    {
        foreach (var surface in surfaces.Reverse())
        {
            surface.Dispose();
        }
    }

    private static FrozenDisplayOverlayPresentationOutcome.Failed Failed(
        Guid correlationId,
        FailureCode code,
        string message,
        int? nativeCode = null) => new(Failure.Create(
        code,
        FailureCategory.Resource,
        FailureRecoverability.RetryNewIntent,
        nameof(WindowsFrozenDisplayOverlayCoordinator),
        correlationId,
        message,
        nativeCode: nativeCode));

    private FunctionBarPresentationResult PrepareOrReposition(
        FunctionBarPresentationRequest request,
        bool allowExisting)
    {
        var phase = "PrepareOrRepositionCore";
        try
        {
            return PrepareOrRepositionCore(
                request,
                allowExisting,
                currentPhase => phase = currentPhase);
        }
        catch (Exception exception)
        {
            return FunctionBarResult(
                FunctionBarPresentationResultKind.Failed,
                request.SessionId,
                request.CoordinateVersion,
                request.Selection.SelectionRevision,
                null,
                Failure.Create(
                    FailureCode.FunctionBarPresentationFailed,
                    FailureCategory.Resource,
                    FailureRecoverability.RetryNewIntent,
                    nameof(WindowsFrozenDisplayOverlayCoordinator),
                    request.SessionId,
                    $"Function Bar presentation failed during {phase}: "
                        + $"{exception.GetType().Name}: {exception.Message}",
                    nativeCode: exception.HResult),
                $"The Function Bar could not be presented during {phase}.");
        }
    }

    private FunctionBarPresentationResult PrepareOrRepositionCore(
        FunctionBarPresentationRequest request,
        bool allowExisting,
        Action<string> setPhase)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(setPhase);
        setPhase("LookupSession");
        IReadOnlyList<OverlaySurface>? surfaces;
        FunctionBarSessionPresentation? existing;
        lock (_gate)
        {
            _sessions.TryGetValue(request.SessionId, out surfaces);
            _functionBars.TryGetValue(request.SessionId, out existing);
        }
        var wasVisible = existing?.Surface.IsVisible == true;

        if (surfaces is null)
        {
            return FunctionBarResult(
                FunctionBarPresentationResultKind.StaleSession,
                request.SessionId,
                request.CoordinateVersion,
                request.Selection.SelectionRevision,
                null,
                null,
                "The Function Bar belongs to a stale capture session.");
        }

        if (surfaces.Any(surface => !string.Equals(
                surface.CoordinateVersion,
                request.CoordinateVersion,
                StringComparison.Ordinal)))
        {
            return FunctionBarResult(
                FunctionBarPresentationResultKind.StaleSession,
                request.SessionId,
                request.CoordinateVersion,
                request.Selection.SelectionRevision,
                existing?.Placement,
                null,
                "The Function Bar coordinate version is stale.");
        }

        if (existing is not null
            && (request.Selection.SelectionRevision < existing.Request.Selection.SelectionRevision
                || (!allowExisting
                    && existing.Request.Selection.SelectionRevision != request.Selection.SelectionRevision)))
        {
            return FunctionBarResult(
                FunctionBarPresentationResultKind.StaleSelectionRevision,
                request.SessionId,
                request.CoordinateVersion,
                request.Selection.SelectionRevision,
                existing.Placement,
                null,
                "The Function Bar already has a newer Selection revision.");
        }

        setPhase("ReadWorkAreas");
        var workAreas = new List<FunctionBarDisplayWorkArea>(surfaces.Count);
        foreach (var surface in surfaces)
        {
            if (!surface.TryGetPhysicalWorkArea(out var workArea))
            {
                return FunctionBarResult(
                    FunctionBarPresentationResultKind.Failed,
                    request.SessionId,
                    request.CoordinateVersion,
                    request.Selection.SelectionRevision,
                    null,
                    CreateFailure(
                        request.SessionId,
                        FailureCode.InvalidWorkArea,
                        "A display Work Area could not be read."),
                    "The Function Bar could not determine a display Work Area.");
            }

            workAreas.Add(new FunctionBarDisplayWorkArea(
                surface.DisplayId,
                surface.PhysicalBounds,
                workArea,
                surface.RasterizationScale,
                surface.RasterizationScale));
        }

        setPhase("SelectAnchor");
        var anchor = _placementService.Place(new FunctionBarPlacementRequest(
            request.SessionId,
            request.CoordinateVersion,
            request.Selection.SelectionRevision,
            request.Selection.NormalizedPhysicalBounds!.Value,
            workAreas,
            new PhysicalPixelSize(1, 1),
            MarginPixels: 8,
            request.Selection.CurrentPhysicalPoint));
        if (anchor is not FunctionBarPlacementOutcome.Ready anchorReady)
        {
            return FunctionBarResult(
                FunctionBarPresentationResultKind.Failed,
                request.SessionId,
                request.CoordinateVersion,
                request.Selection.SelectionRevision,
                null,
                anchor is FunctionBarPlacementOutcome.Failed failed
                    ? failed.Failure
                    : CreateFailure(
                        request.SessionId,
                        FailureCode.FunctionBarPlacementFailed,
                        "The Function Bar anchor could not be selected."),
                "The Function Bar anchor could not be selected.");
        }

        var anchorSurface = surfaces.FirstOrDefault(
            surface => string.Equals(
                surface.DisplayId,
                anchorReady.Placement.DisplayId,
                StringComparison.Ordinal));
        if (anchorSurface is null)
        {
            return FunctionBarResult(
                FunctionBarPresentationResultKind.Failed,
                request.SessionId,
                request.CoordinateVersion,
                request.Selection.SelectionRevision,
                null,
                CreateFailure(
                    request.SessionId,
                    FailureCode.InvalidWorkArea,
                    "The Function Bar anchor display is no longer available."),
                "The Function Bar anchor display is no longer available.");
        }

        FunctionBarSurface? surfaceForBar = null;
        var reusesExisting = existing is not null
            && ReferenceEquals(existing.Surface.Owner, anchorSurface);
        setPhase(reusesExisting ? "UpdateExistingSurface" : "CreateFunctionBar");
        if (reusesExisting)
        {
            surfaceForBar = existing!.Surface;
            surfaceForBar.Update(request);
        }
        else
        {
            surfaceForBar = anchorSurface.CreateFunctionBar(
                request,
                phase => setPhase($"CreateFunctionBar.{phase}"));
        }

        setPhase("MeasureFunctionBar");
        if (!surfaceForBar.TryMeasurePhysicalSize(out var measuredSize))
        {
            if (!reusesExisting)
            {
                surfaceForBar.Deactivate();
            }

            return FunctionBarResult(
                FunctionBarPresentationResultKind.Failed,
                request.SessionId,
                request.CoordinateVersion,
                request.Selection.SelectionRevision,
                null,
                CreateFailure(
                    request.SessionId,
                    FailureCode.BarMeasurementFailed,
                    "The Function Bar could not be measured."),
                "The Function Bar could not be measured.");
        }

        setPhase("PlaceMeasuredFunctionBar");
        var placed = _placementService.Place(new FunctionBarPlacementRequest(
            request.SessionId,
            request.CoordinateVersion,
            request.Selection.SelectionRevision,
            request.Selection.NormalizedPhysicalBounds.Value,
            workAreas,
            measuredSize,
            MarginPixels: 8,
            request.Selection.CurrentPhysicalPoint));
        if (placed is not FunctionBarPlacementOutcome.Ready placedReady)
        {
            if (!reusesExisting)
            {
                surfaceForBar.Deactivate();
            }

            return FunctionBarResult(
                FunctionBarPresentationResultKind.Failed,
                request.SessionId,
                request.CoordinateVersion,
                request.Selection.SelectionRevision,
                null,
                placed is FunctionBarPlacementOutcome.Failed failed
                    ? failed.Failure
                    : CreateFailure(
                        request.SessionId,
                        FailureCode.FunctionBarPlacementFailed,
                        "The Function Bar could not be placed."),
                "The Function Bar could not be placed.");
        }

        setPhase("ApplyPlacement");
        surfaceForBar.ApplyPlacement(placedReady.Placement);
        surfaceForBar.SetVisible(
            FunctionBarSurface.GetRepositionVisibility(existing is not null, wasVisible));
        var next = new FunctionBarSessionPresentation(request, placedReady.Placement, surfaceForBar);
        setPhase("RegisterSession");
        lock (_gate)
        {
            if (_disposed || !_sessions.ContainsKey(request.SessionId))
            {
                next.Dispose();
                return FunctionBarResult(
                    FunctionBarPresentationResultKind.StaleSession,
                    request.SessionId,
                    request.CoordinateVersion,
                    request.Selection.SelectionRevision,
                    null,
                    null,
                    "The Function Bar session ended while it was being prepared.");
            }

            _functionBars[request.SessionId] = next;
        }

        if (existing is not null && !ReferenceEquals(existing.Surface, surfaceForBar))
        {
            existing.Dispose();
        }

        return FunctionBarResult(
            FunctionBarPresentationResultKind.Ready,
            request.SessionId,
            request.CoordinateVersion,
            request.Selection.SelectionRevision,
            placedReady.Placement,
            null,
            "The Function Bar is prepared and hidden.");
    }

    private static FunctionBarPresentationResult FunctionBarResult(
        FunctionBarPresentationResultKind kind,
        Guid sessionId,
        string coordinateVersion,
        int selectionRevision,
        FunctionBarPlacementResult? placement,
        Failure? failure,
        string message) => new(
        kind,
        sessionId,
        coordinateVersion,
        selectionRevision,
        placement,
        failure,
        message);

    private static Failure CreateFailure(Guid correlationId, FailureCode code, string message) =>
        Failure.Create(
            code,
            FailureCategory.Resource,
            FailureRecoverability.RetryNewIntent,
            nameof(WindowsFrozenDisplayOverlayCoordinator),
            correlationId,
            message);

    private sealed class FunctionBarSessionPresentation : IDisposable
    {
        public FunctionBarSessionPresentation(
            FunctionBarPresentationRequest request,
            FunctionBarPlacementResult placement,
            FunctionBarSurface surface)
        {
            Request = request;
            Placement = placement;
            Surface = surface;
        }

        public FunctionBarPresentationRequest Request { get; }

        public FunctionBarPlacementResult Placement { get; }

        public FunctionBarSurface Surface { get; }

        public void Dispose() => Surface.Deactivate();
    }

    private sealed class FunctionBarSurface : IDisposable
    {
        private readonly OverlaySurface _owner;
        private readonly Border _root;
        private readonly StackPanel _panel;
        private readonly ScrollViewer _scrollViewer;
        private readonly TextBlock _feedbackText;
        private readonly IReadOnlyDictionary<FunctionBarCommand, Button> _buttons;
        private readonly IReadOnlyDictionary<EditingToolKind, RadioButton> _toolButtons;
        private readonly IReadOnlyDictionary<ArrowLineEndStyle, RadioButton> _arrowLineModeButtons;
        private readonly IReadOnlyDictionary<PrivacyRegionMode, RadioButton> _privacyModeButtons;
        private static readonly IReadOnlyDictionary<string, ArgbColor> ColorOptions =
            new Dictionary<string, ArgbColor>(StringComparer.Ordinal)
            {
                ["Red"] = ArgbColor.Red,
                ["Yellow"] = new ArgbColor(255, 255, 235, 59),
                ["Blue"] = new ArgbColor(255, 60, 140, 255),
                ["Red 50%"] = new ArgbColor(128, 220, 60, 60),
                ["Yellow 50%"] = new ArgbColor(128, 255, 235, 59),
                ["Blue 50%"] = new ArgbColor(128, 60, 140, 255)
            };
        private readonly NumberBox _nextNumberBox;
        private readonly ComboBox _colorBox;
        private readonly NumberBox _thicknessBox;
        private readonly NumberBox _fontSizeBox;
        private readonly NumberBox _markerSizeBox;
        private readonly TextBox _nextNumberTextBox;
        private readonly TextBox _thicknessTextBox;
        private readonly TextBox _fontSizeTextBox;
        private readonly TextBox _markerSizeTextBox;
        private readonly ToggleButton _boldToggle;
        private readonly Button _deleteObjectButton;
        private readonly Button _editTextButton;
        private readonly CancelCommandGate _cancelCommandGate = new();
        private readonly CancelCommandGate _completeCommandGate = new();
        private readonly CancelCommandGate _undoCommandGate = new();
        private readonly CancelCommandGate _redoCommandGate = new();
        private FunctionBarPresentationRequest _request = null!;
        private bool _updatingNextNumber;
        private bool _updatingStyleControls;
        private bool _initialized;
        private bool _isVisible;
        private bool _disposed;

        public FunctionBarSurface(
            OverlaySurface owner,
            Action<string>? setPhase = null)
        {
            _owner = owner;
            setPhase?.Invoke("CreateDetachedRoot");
            _root = CreateRoot();
            ApplyPreloadVisibility(_root);
            setPhase?.Invoke("CreatePanel");
            _panel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            _scrollViewer = new ScrollViewer
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Disabled,
                HorizontalScrollMode = ScrollMode.Enabled,
                VerticalScrollMode = ScrollMode.Disabled
            };
            setPhase?.Invoke("CreateFeedbackText");
            _feedbackText = new TextBlock
            {
                Foreground = new SolidColorBrush(ColorHelper.FromArgb(255, 255, 255, 255)),
                Margin = new Thickness(4, 0, 8, 0),
                TextWrapping = TextWrapping.Wrap,
                Visibility = Visibility.Collapsed
            };
            setPhase?.Invoke("CreateNextNumberBox");
            _nextNumberBox = CreateNextNumberBox();
            setPhase?.Invoke("CreateNextNumberTextBox");
            _nextNumberTextBox = CreateNumericTextBox("Next number");
            setPhase?.Invoke("CreateColorBox");
            _colorBox = CreateColorBox();
            setPhase?.Invoke("CreateThicknessBox");
            _thicknessBox = CreateStyleNumberBox("Thickness", 1, 64);
            setPhase?.Invoke("CreateThicknessTextBox");
            _thicknessTextBox = CreateNumericTextBox("Thickness");
            setPhase?.Invoke("CreateFontSizeBox");
            _fontSizeBox = CreateStyleNumberBox("Font size", 8, 144);
            setPhase?.Invoke("CreateFontSizeTextBox");
            _fontSizeTextBox = CreateNumericTextBox("Font size");
            setPhase?.Invoke("CreateMarkerSizeBox");
            _markerSizeBox = CreateStyleNumberBox("Marker size", 8, 256);
            setPhase?.Invoke("CreateMarkerSizeTextBox");
            _markerSizeTextBox = CreateNumericTextBox("Marker size");
            setPhase?.Invoke("CreateBoldToggle");
            _boldToggle = new ToggleButton { Content = "Bold" };
            ApplyControlVisualStyle(_boldToggle, GetButtonVisualStyle());
            setPhase?.Invoke("CreateDeleteObjectButton");
            _deleteObjectButton = CreateButton("Delete selected");
            setPhase?.Invoke("CreateEditTextButton");
            _editTextButton = CreateButton("Edit text");
            setPhase?.Invoke("CreateCommandButtons");
            _buttons = new Dictionary<FunctionBarCommand, Button>
            {
                [FunctionBarCommand.Complete] = CreateButton("Complete"),
                [FunctionBarCommand.Save] = CreateButton("Save"),
                [FunctionBarCommand.Cancel] = CreateButton("Cancel"),
                [FunctionBarCommand.Undo] = CreateButton("Undo"),
                [FunctionBarCommand.Redo] = CreateButton("Redo")
            };
            setPhase?.Invoke("CreateToolButtons");
            _toolButtons = new Dictionary<EditingToolKind, RadioButton>
            {
                [EditingToolKind.Selection] = CreateToolButton("Selection"),
                [EditingToolKind.Rectangle] = CreateToolButton("Rectangle"),
                [EditingToolKind.ArrowLine] = CreateToolButton("Arrow / Line"),
                [EditingToolKind.Highlighter] = CreateToolButton("Highlighter"),
                [EditingToolKind.Text] = CreateToolButton("Text"),
                [EditingToolKind.PrivacyRegion] = CreateToolButton("Mosaic / Blur"),
                [EditingToolKind.NumberedMarker] = CreateToolButton("Numbered Marker")
            };
            setPhase?.Invoke("CreateArrowLineModeButtons");
            _arrowLineModeButtons = new Dictionary<ArrowLineEndStyle, RadioButton>
            {
                [ArrowLineEndStyle.Arrow] = CreateModeButton("Arrow"),
                [ArrowLineEndStyle.None] = CreateModeButton("Line")
            };
            setPhase?.Invoke("CreatePrivacyModeButtons");
            _privacyModeButtons = new Dictionary<PrivacyRegionMode, RadioButton>
            {
                [PrivacyRegionMode.Mosaic] = CreatePrivacyModeButton("Mosaic"),
                [PrivacyRegionMode.Blur] = CreatePrivacyModeButton("Blur")
            };
            _buttons[FunctionBarCommand.Complete].Click += OnCompleteClicked;
            _buttons[FunctionBarCommand.Save].Click += OnSaveClicked;
            _buttons[FunctionBarCommand.Cancel].Click += OnCancelClicked;
            _buttons[FunctionBarCommand.Undo].Click += OnUndoClicked;
            _buttons[FunctionBarCommand.Redo].Click += OnRedoClicked;
            _toolButtons[EditingToolKind.Selection].Click += OnSelectionToolClicked;
            _toolButtons[EditingToolKind.Rectangle].Click += OnRectangleToolClicked;
            _toolButtons[EditingToolKind.ArrowLine].Click += OnArrowLineToolClicked;
            _toolButtons[EditingToolKind.Highlighter].Click += OnHighlighterToolClicked;
            _toolButtons[EditingToolKind.Text].Click += OnTextToolClicked;
            _toolButtons[EditingToolKind.PrivacyRegion].Click += OnPrivacyRegionToolClicked;
            _toolButtons[EditingToolKind.NumberedMarker].Click += OnNumberedMarkerToolClicked;
            _nextNumberBox.ValueChanged += OnNextNumberChanged;
            _nextNumberTextBox.TextChanged += OnNextNumberTextChanged;
            _arrowLineModeButtons[ArrowLineEndStyle.Arrow].Click += OnArrowModeClicked;
            _arrowLineModeButtons[ArrowLineEndStyle.None].Click += OnLineModeClicked;
            _privacyModeButtons[PrivacyRegionMode.Mosaic].Click += OnMosaicModeClicked;
            _privacyModeButtons[PrivacyRegionMode.Blur].Click += OnBlurModeClicked;
            _colorBox.SelectionChanged += OnColorChanged;
            _thicknessBox.ValueChanged += OnThicknessChanged;
            _thicknessTextBox.TextChanged += OnThicknessTextChanged;
            _fontSizeBox.ValueChanged += OnFontSizeChanged;
            _fontSizeTextBox.TextChanged += OnFontSizeTextChanged;
            _markerSizeBox.ValueChanged += OnMarkerSizeChanged;
            _markerSizeTextBox.TextChanged += OnMarkerSizeTextChanged;
            _boldToggle.Checked += OnBoldChanged;
            _boldToggle.Unchecked += OnBoldChanged;
            _deleteObjectButton.Click += OnDeleteObjectClicked;
            _editTextButton.Click += OnEditTextClicked;
            _root.PointerPressed += OnPointerPressed;
            AttachContent(setPhase);
        }

        public OverlaySurface Owner => _owner;

        public FrameworkElement Root => _root;

        public bool IsVisible => _isVisible;

        public void AttachContent(Action<string>? setPhase = null)
        {
            ObjectDisposedException.ThrowIf(_disposed, nameof(FunctionBarSurface));
            setPhase?.Invoke("AttachFeedbackText");
            _panel.Children.Add(_feedbackText);

            var toolIndex = 0;
            foreach (var toolButton in _toolButtons.Values)
            {
                setPhase?.Invoke($"AttachToolButton{toolIndex++}");
                _panel.Children.Add(toolButton);
            }

            setPhase?.Invoke("AttachColorBox");
            _panel.Children.Add(_colorBox);
            setPhase?.Invoke("AttachNextNumberTextBox");
            _panel.Children.Add(_nextNumberTextBox);
            setPhase?.Invoke("AttachThicknessTextBox");
            _panel.Children.Add(_thicknessTextBox);
            setPhase?.Invoke("AttachFontSizeTextBox");
            _panel.Children.Add(_fontSizeTextBox);
            setPhase?.Invoke("AttachMarkerSizeTextBox");
            _panel.Children.Add(_markerSizeTextBox);
            setPhase?.Invoke("AttachBoldToggle");
            _panel.Children.Add(_boldToggle);
            setPhase?.Invoke("AttachDeleteObjectButton");
            _panel.Children.Add(_deleteObjectButton);
            setPhase?.Invoke("AttachEditTextButton");
            _panel.Children.Add(_editTextButton);

            var arrowLineModeIndex = 0;
            foreach (var modeButton in _arrowLineModeButtons.Values)
            {
                setPhase?.Invoke($"AttachArrowLineModeButton{arrowLineModeIndex++}");
                _panel.Children.Add(modeButton);
            }

            var privacyModeIndex = 0;
            foreach (var modeButton in _privacyModeButtons.Values)
            {
                setPhase?.Invoke($"AttachPrivacyModeButton{privacyModeIndex++}");
                _panel.Children.Add(modeButton);
            }

            var commandButtonIndex = 0;
            foreach (var button in _buttons.Values)
            {
                setPhase?.Invoke($"AttachCommandButton{commandButtonIndex++}");
                _panel.Children.Add(button);
            }

            setPhase?.Invoke("AttachRootChild");
            _scrollViewer.Content = _panel;
            _root.Child = _scrollViewer;
        }

        public void Initialize(
            FunctionBarPresentationRequest request,
            Action<string>? setPhase = null)
        {
            ObjectDisposedException.ThrowIf(_disposed, nameof(FunctionBarSurface));
            ArgumentNullException.ThrowIfNull(request);
            _request = request;
            _initialized = true;
            _isVisible = false;
            ApplyVisibility(_root, visible: false);
            setPhase?.Invoke("UpdateRequest");
            Update(request);
        }

        public void Update(FunctionBarPresentationRequest request)
        {
            _request = request;
            if (request.Availability.IsEnabled(FunctionBarCommand.Complete)
                || request.Availability.IsEnabled(FunctionBarCommand.Save))
            {
                _completeCommandGate.Reset();
            }

            if (!request.Availability.IsEnabled(FunctionBarCommand.Complete)
                && !request.Availability.IsEnabled(FunctionBarCommand.Cancel))
            {
                ClearFeedback();
            }

            if (request.Availability.IsEnabled(FunctionBarCommand.Cancel))
            {
                _cancelCommandGate.Reset();
            }

            foreach (var pair in _buttons)
            {
                pair.Value.IsEnabled = request.Availability.IsEnabled(pair.Key);
            }

            foreach (var pair in _toolButtons)
            {
                pair.Value.IsChecked = pair.Key == request.ActiveTool;
                pair.Value.IsEnabled = request.ToolSelectionSink is not null;
            }

            foreach (var pair in _arrowLineModeButtons)
            {
                var selectedArrow = request.SelectedObject?.OriginalObject?.Content
                    is ArrowLineAnnotationContent arrowContent
                    ? arrowContent.Style.EndStyle
                    : request.ActiveArrowLineEndStyle;
                pair.Value.IsChecked = pair.Key == selectedArrow;
                pair.Value.IsEnabled = request.AnnotationObjectEditingSink is not null
                    && request.SelectedObject?.OriginalObject?.ToolKind == AnnotationToolKind.ArrowLine
                    || request.ToolSelectionSink is not null
                    && request.ActiveTool == EditingToolKind.ArrowLine;
            }

            foreach (var pair in _privacyModeButtons)
            {
                var selectedPrivacy = request.SelectedObject?.OriginalObject?.Content
                    is PrivacyRegionAnnotationContent privacy
                    ? privacy.Mode
                    : request.ActivePrivacyRegionMode;
                pair.Value.IsChecked = selectedPrivacy == pair.Key;
                pair.Value.IsEnabled = request.AnnotationObjectEditingSink is not null
                    && request.SelectedObject?.OriginalObject?.ToolKind == AnnotationToolKind.PrivacyRegion
                    || request.PrivacyRegionModeSelectionSink is not null
                    && request.ActiveTool == EditingToolKind.PrivacyRegion;
            }

            UpdateStyleControls(request);

            _updatingNextNumber = true;
            try
            {
                _nextNumberBox.Visibility = request.ActiveTool == EditingToolKind.NumberedMarker
                    ? Visibility.Visible
                    : Visibility.Collapsed;
                _nextNumberTextBox.Visibility = _nextNumberBox.Visibility;
                _nextNumberBox.IsEnabled = request.NextNumberSelectionSink is not null
                    && request.ActiveTool == EditingToolKind.NumberedMarker;
                _nextNumberTextBox.IsEnabled = _nextNumberBox.IsEnabled;
                if (request.ActiveNumberedMarkerNextNumber is int nextNumber)
                {
                    _nextNumberBox.Value = nextNumber;
                    _nextNumberTextBox.Text = nextNumber.ToString(CultureInfo.InvariantCulture);
                }
            }
            finally
            {
                _updatingNextNumber = false;
            }
        }

        public bool TryMeasurePhysicalSize(out PhysicalPixelSize size)
        {
            size = default;
            if (_disposed)
            {
                return false;
            }

            var scale = _owner.RasterizationScale;
            if (!double.IsFinite(scale) || scale <= 0)
            {
                return false;
            }

            var maxWidthDip = 960d;
            if (_owner.TryGetPhysicalWorkArea(out var workArea))
            {
                maxWidthDip = Math.Min(
                    maxWidthDip,
                    Math.Max(240d, (workArea.Width - 16) / scale));
            }

            const double heightDip = 56d;
            _root.MaxWidth = maxWidthDip;
            _root.Width = maxWidthDip;
            _root.Height = heightDip;
            size = new PhysicalPixelSize(
                checked((int)Math.Round(maxWidthDip * scale)),
                checked((int)Math.Round(heightDip * scale)));
            return size.IsPositive;
        }

        public void ApplyPlacement(FunctionBarPlacementResult placement)
        {
            var bounds = _owner.PhysicalBounds;
            var scale = _owner.RasterizationScale;
            Canvas.SetLeft(_root, (placement.FunctionBarPhysicalBounds.Left - bounds.Left) / scale);
            Canvas.SetTop(_root, (placement.FunctionBarPhysicalBounds.Top - bounds.Top) / scale);
            _root.Width = placement.FunctionBarPhysicalBounds.Width / scale;
            _root.Height = placement.FunctionBarPhysicalBounds.Height / scale;
            _owner.TraceFunctionBarDiagnostic(
                "FunctionBar.Surface.Placement",
                $"DisplayId={_owner.DisplayId};PhysicalBounds={placement.FunctionBarPhysicalBounds.Left},{placement.FunctionBarPhysicalBounds.Top},{placement.FunctionBarPhysicalBounds.Right},{placement.FunctionBarPhysicalBounds.Bottom};RasterizationScale={scale:0.###};CanvasLeft={Canvas.GetLeft(_root):0.##};CanvasTop={Canvas.GetTop(_root):0.##};Width={_root.Width:0.##};Height={_root.Height:0.##}");
        }

        public void SetVisible(bool visible)
        {
            if (!_disposed)
            {
                _isVisible = visible;
                ApplyVisibility(_root, visible);
                _owner.TraceFunctionBarDiagnostic(
                    "FunctionBar.Surface.Visibility",
                    $"DisplayId={_owner.DisplayId};Requested={visible};Visibility={_root.Visibility};Opacity={_root.Opacity:0.##};IsHitTestVisible={_root.IsHitTestVisible};CanvasLeft={Canvas.GetLeft(_root):0.##};CanvasTop={Canvas.GetTop(_root):0.##};Width={_root.Width:0.##};Height={_root.Height:0.##}");
            }
        }

        public void Deactivate()
        {
            if (_disposed)
            {
                return;
            }

            _initialized = false;
            ClearFeedback();
            SetVisible(false);
        }

        public void ClearFeedback()
        {
            if (!_disposed)
            {
                _feedbackText.Text = string.Empty;
                _feedbackText.Visibility = Visibility.Collapsed;
            }
        }

        public void ShowFeedback(string message)
        {
            if (!_disposed)
            {
                _feedbackText.Text = message;
                _feedbackText.Visibility = Visibility.Visible;
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            Deactivate();
            _disposed = true;
            _buttons[FunctionBarCommand.Complete].Click -= OnCompleteClicked;
            _buttons[FunctionBarCommand.Save].Click -= OnSaveClicked;
            _buttons[FunctionBarCommand.Cancel].Click -= OnCancelClicked;
            _buttons[FunctionBarCommand.Undo].Click -= OnUndoClicked;
            _buttons[FunctionBarCommand.Redo].Click -= OnRedoClicked;
            _toolButtons[EditingToolKind.Selection].Click -= OnSelectionToolClicked;
            _toolButtons[EditingToolKind.Rectangle].Click -= OnRectangleToolClicked;
            _toolButtons[EditingToolKind.ArrowLine].Click -= OnArrowLineToolClicked;
            _toolButtons[EditingToolKind.Highlighter].Click -= OnHighlighterToolClicked;
            _toolButtons[EditingToolKind.Text].Click -= OnTextToolClicked;
            _toolButtons[EditingToolKind.PrivacyRegion].Click -= OnPrivacyRegionToolClicked;
            _toolButtons[EditingToolKind.NumberedMarker].Click -= OnNumberedMarkerToolClicked;
            _nextNumberBox.ValueChanged -= OnNextNumberChanged;
            _nextNumberTextBox.TextChanged -= OnNextNumberTextChanged;
            _arrowLineModeButtons[ArrowLineEndStyle.Arrow].Click -= OnArrowModeClicked;
            _arrowLineModeButtons[ArrowLineEndStyle.None].Click -= OnLineModeClicked;
            _privacyModeButtons[PrivacyRegionMode.Mosaic].Click -= OnMosaicModeClicked;
            _privacyModeButtons[PrivacyRegionMode.Blur].Click -= OnBlurModeClicked;
            _colorBox.SelectionChanged -= OnColorChanged;
            _thicknessBox.ValueChanged -= OnThicknessChanged;
            _thicknessTextBox.TextChanged -= OnThicknessTextChanged;
            _fontSizeBox.ValueChanged -= OnFontSizeChanged;
            _fontSizeTextBox.TextChanged -= OnFontSizeTextChanged;
            _markerSizeBox.ValueChanged -= OnMarkerSizeChanged;
            _markerSizeTextBox.TextChanged -= OnMarkerSizeTextChanged;
            _boldToggle.Checked -= OnBoldChanged;
            _boldToggle.Unchecked -= OnBoldChanged;
            _deleteObjectButton.Click -= OnDeleteObjectClicked;
            _editTextButton.Click -= OnEditTextClicked;
            _root.PointerPressed -= OnPointerPressed;
            _panel.Children.Clear();
            _root.Child = null;
        }

        private static void ApplyVisibility(Border root, bool visible)
        {
            var state = GetVisibilityState(visible);
            root.Visibility = state.IsLayoutParticipating
                ? Visibility.Visible
                : Visibility.Collapsed;
            root.Opacity = state.Opacity;
            root.IsHitTestVisible = state.IsHitTestVisible;
        }

        private static void ApplyPreloadVisibility(Border root)
        {
            root.Visibility = Visibility.Collapsed;
            root.Opacity = 0;
            root.IsHitTestVisible = false;
        }

        private static Border CreateRoot()
        {
            return new Border
            {
                Background = new SolidColorBrush(ColorHelper.FromArgb(255, 11, 18, 32)),
                BorderBrush = new SolidColorBrush(ColorHelper.FromArgb(255, 169, 189, 210)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(6)
            };
        }

        private static FunctionBarVisibilityState GetVisibilityState(bool visible) => visible
            ? new FunctionBarVisibilityState(true, 1, true)
            : new FunctionBarVisibilityState(true, 0, false);

        internal static bool GetRepositionVisibility(
            bool hasExistingPresentation,
            bool wasVisible) => hasExistingPresentation && wasVisible;

        private readonly record struct FunctionBarVisibilityState(
            bool IsLayoutParticipating,
            double Opacity,
            bool IsHitTestVisible);

        private static bool TryConvertToPhysicalSize(
            Size desired,
            double rasterizationScale,
            out PhysicalPixelSize size)
        {
            size = default;
            if (!double.IsFinite(rasterizationScale)
                || rasterizationScale <= 0
                || !double.IsFinite(desired.Width)
                || !double.IsFinite(desired.Height)
                || desired.Width <= 0
                || desired.Height <= 0)
            {
                return false;
            }

            var width = (int)Math.Ceiling(desired.Width * rasterizationScale);
            var height = (int)Math.Ceiling(desired.Height * rasterizationScale);
            if (width <= 0 || height <= 0)
            {
                return false;
            }

            size = new PhysicalPixelSize(width, height);
            return true;
        }

        private static Button CreateButton(string label)
        {
            var visualStyle = GetButtonVisualStyle();
            var button = new Button
            {
                Content = label,
                Padding = new Thickness(8, 4, 8, 4),
                Margin = new Thickness(2, 0, 2, 0),
                CornerRadius = new CornerRadius(4),
                IsTabStop = true
            };
            ApplyControlVisualStyle(button, visualStyle);
            Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(button, label);
            return button;
        }

        private static RadioButton CreateToolButton(string label)
        {
            var button = new RadioButton
            {
                Content = label,
                GroupName = "SnipPlusEditingTool",
                Padding = new Thickness(8, 4, 8, 4),
                Margin = new Thickness(2, 0, 2, 0),
                IsTabStop = true
            };
            ApplyControlVisualStyle(button, GetButtonVisualStyle());
            Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(
                button,
                $"Editing tool {label}");
            return button;
        }

        private static RadioButton CreateModeButton(string label)
        {
            var button = CreateToolButton(label);
            button.GroupName = "SnipPlusArrowLineMode";
            Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(
                button,
                $"Arrow or line mode {label}");
            return button;
        }

        private static NumberBox CreateNextNumberBox()
        {
            var numberBox = new NumberBox
            {
                Minimum = 1,
                Maximum = int.MaxValue,
                SmallChange = 1,
                Width = 120,
                Margin = new Thickness(4, 0, 4, 0),
                Header = "Next number",
                Visibility = Visibility.Collapsed,
                IsTabStop = true
            };
            ApplyControlVisualStyle(numberBox, GetButtonVisualStyle());
            Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(
                numberBox,
                "Next numbered marker number");
            return numberBox;
        }

        private static ComboBox CreateColorBox()
        {
            var box = new ComboBox
            {
                PlaceholderText = "Color",
                Width = 120,
                Margin = new Thickness(4, 0, 4, 0),
                IsTabStop = true
            };
            ApplyControlVisualStyle(box, GetButtonVisualStyle());
            foreach (var colorName in ColorOptions.Keys)
            {
                var item = new ComboBoxItem { Content = colorName, Tag = colorName };
                ApplyControlVisualStyle(item, GetButtonVisualStyle());
                box.Items.Add(item);
            }

            Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(box, "Annotation color");
            return box;
        }

        private static NumberBox CreateStyleNumberBox(string header, double minimum, double maximum)
        {
            var box = new NumberBox
            {
                PlaceholderText = header,
                Minimum = minimum,
                Maximum = maximum,
                SmallChange = 1,
                Width = 112,
                Margin = new Thickness(4, 0, 4, 0),
                Visibility = Visibility.Collapsed,
                IsTabStop = true
            };
            ApplyControlVisualStyle(box, GetButtonVisualStyle());
            Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(box, $"Annotation {header}");
            return box;
        }

        private static TextBox CreateNumericTextBox(string header)
        {
            var box = new TextBox
            {
                Width = 112,
                Margin = new Thickness(4, 0, 4, 0),
                Visibility = Visibility.Collapsed,
                IsTabStop = true,
                PlaceholderText = header
            };
            ApplyControlVisualStyle(box, GetButtonVisualStyle());
            Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(
                box,
                $"Annotation {header}");
            return box;
        }

        private static FunctionBarButtonVisualStyle GetButtonVisualStyle() =>
            new(
                ColorHelper.FromArgb(255, 255, 255, 255),
                ColorHelper.FromArgb(255, 39, 54, 74),
                ColorHelper.FromArgb(255, 169, 189, 210));

        private static void ApplyControlVisualStyle(
            Control control,
            FunctionBarButtonVisualStyle visualStyle)
        {
            control.Foreground = new SolidColorBrush(visualStyle.Foreground);
            control.Background = new SolidColorBrush(visualStyle.Background);
            control.BorderBrush = new SolidColorBrush(visualStyle.Border);
            control.BorderThickness = new Thickness(1);
            control.FontWeight = FontWeights.SemiBold;
        }

        private void OnCancelClicked(object sender, RoutedEventArgs args)
        {
            if (!_cancelCommandGate.TryBegin())
            {
                return;
            }

            _owner.TraceFunctionBarDiagnostic("FunctionBar.Command.Clicked", "Command=Cancel");

            var command = new FunctionBarCommandRequest(
                _request.SessionId,
                _request.CoordinateVersion,
                _request.Selection.SelectionRevision,
                _request.AnnotationRevision,
                FunctionBarCommand.Cancel);
            _buttons[FunctionBarCommand.Cancel].IsEnabled = false;

            var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            if (dispatcherQueue is null
                || !dispatcherQueue.TryEnqueue(() =>
                {
                    if (_disposed)
                    {
                        return;
                    }

                    var result = _request.CommandSink.Execute(command);
                    _owner.TraceFunctionBarDiagnostic(
                        "FunctionBar.Command.Result",
                        $"Command=Cancel;Result={result.Kind}");
                    if (result.Kind != FunctionBarCommandResultKind.Accepted)
                    {
                        _cancelCommandGate.Reset();
                        if (!_disposed)
                        {
                            _buttons[FunctionBarCommand.Cancel].IsEnabled = true;
                        }
                    }
                }))
            {
                _cancelCommandGate.Reset();
                _buttons[FunctionBarCommand.Cancel].IsEnabled = true;
            }

        }

        private void OnUndoClicked(object sender, RoutedEventArgs args) =>
            DispatchHistoryCommand(FunctionBarCommand.Undo, _undoCommandGate);

        private void OnRedoClicked(object sender, RoutedEventArgs args) =>
            DispatchHistoryCommand(FunctionBarCommand.Redo, _redoCommandGate);

        private void DispatchHistoryCommand(
            FunctionBarCommand command,
            CancelCommandGate gate)
        {
            if (!gate.TryBegin())
            {
                return;
            }

            _owner.TraceFunctionBarDiagnostic(
                "FunctionBar.Command.Clicked",
                $"Command={command}");

            var request = new FunctionBarCommandRequest(
                _request.SessionId,
                _request.CoordinateVersion,
                _request.Selection.SelectionRevision,
                _request.AnnotationRevision,
                command);
            _buttons[command].IsEnabled = false;
            var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            if (dispatcherQueue is null
                || !dispatcherQueue.TryEnqueue(() =>
                {
                    if (_disposed)
                    {
                        return;
                    }

                    var result = _request.CommandSink.Execute(request);
                    _owner.TraceFunctionBarDiagnostic(
                        "FunctionBar.Command.Result",
                        $"Command={command};Result={result.Kind}");
                    gate.Reset();
                    if (!_disposed && result.Kind != FunctionBarCommandResultKind.Accepted)
                    {
                        Update(_request);
                    }
                }))
            {
                gate.Reset();
                if (!_disposed)
                {
                    Update(_request);
                }
            }
        }

        private void OnCompleteClicked(object sender, RoutedEventArgs args)
        {
            if (!_completeCommandGate.TryBegin())
            {
                return;
            }

            _owner.TraceFunctionBarDiagnostic("FunctionBar.Command.Clicked", "Command=Complete");

            var command = new FunctionBarCommandRequest(
                _request.SessionId,
                _request.CoordinateVersion,
                _request.Selection.SelectionRevision,
                _request.AnnotationRevision,
                FunctionBarCommand.Complete);
            _buttons[FunctionBarCommand.Complete].IsEnabled = false;

            var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            if (dispatcherQueue is null
                || !dispatcherQueue.TryEnqueue(() =>
                {
                    if (_disposed)
                    {
                        return;
                    }

                    var result = _request.CommandSink.Execute(command);
                    _owner.TraceFunctionBarDiagnostic(
                        "FunctionBar.Command.Result",
                        $"Command=Complete;Result={result.Kind}");
                    if (result.Kind != FunctionBarCommandResultKind.Accepted)
                    {
                        _completeCommandGate.Reset();
                        if (!_disposed)
                        {
                            _buttons[FunctionBarCommand.Complete].IsEnabled =
                                _request.Availability.IsEnabled(FunctionBarCommand.Complete);
                        }
                    }
                }))
            {
                _completeCommandGate.Reset();
                _buttons[FunctionBarCommand.Complete].IsEnabled =
                    _request.Availability.IsEnabled(FunctionBarCommand.Complete);
            }
        }

        private void OnSaveClicked(object sender, RoutedEventArgs args)
        {
            if (!_completeCommandGate.TryBegin())
            {
                return;
            }

            _owner.TraceFunctionBarDiagnostic("FunctionBar.Command.Clicked", "Command=Save");

            var command = new FunctionBarCommandRequest(
                _request.SessionId,
                _request.CoordinateVersion,
                _request.Selection.SelectionRevision,
                _request.AnnotationRevision,
                FunctionBarCommand.Save);
            _buttons[FunctionBarCommand.Save].IsEnabled = false;

            var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            if (dispatcherQueue is null
                || !dispatcherQueue.TryEnqueue(() =>
                {
                    if (_disposed)
                    {
                        return;
                    }

                    var result = _request.CommandSink.Execute(command);
                    _owner.TraceFunctionBarDiagnostic(
                        "FunctionBar.Command.Result",
                        $"Command=Save;Result={result.Kind}");
                    if (result.Kind != FunctionBarCommandResultKind.Accepted)
                    {
                        _completeCommandGate.Reset();
                        if (!_disposed)
                        {
                            _buttons[FunctionBarCommand.Save].IsEnabled =
                                _request.Availability.IsEnabled(FunctionBarCommand.Save);
                        }
                    }
                }))
            {
                _completeCommandGate.Reset();
                _buttons[FunctionBarCommand.Save].IsEnabled =
                    _request.Availability.IsEnabled(FunctionBarCommand.Save);
            }
        }

        private void OnSelectionToolClicked(object sender, RoutedEventArgs args) =>
            SelectTool(EditingToolKind.Selection);

        private void OnRectangleToolClicked(object sender, RoutedEventArgs args) =>
            SelectTool(EditingToolKind.Rectangle);

        private void OnArrowLineToolClicked(object sender, RoutedEventArgs args) =>
            SelectTool(EditingToolKind.ArrowLine, _request.ActiveArrowLineEndStyle);

        private void OnHighlighterToolClicked(object sender, RoutedEventArgs args) =>
            SelectTool(EditingToolKind.Highlighter);

        private void OnTextToolClicked(object sender, RoutedEventArgs args) =>
            SelectTool(EditingToolKind.Text);

        private void OnPrivacyRegionToolClicked(object sender, RoutedEventArgs args) =>
            SelectTool(EditingToolKind.PrivacyRegion, privacyMode: _request.ActivePrivacyRegionMode);

        private void OnNumberedMarkerToolClicked(object sender, RoutedEventArgs args) =>
            SelectTool(EditingToolKind.NumberedMarker);

        private void OnNextNumberChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (_disposed || _updatingNextNumber || _request.NextNumberSelectionSink is null)
            {
                return;
            }

            if (!double.IsFinite(args.NewValue)
                || args.NewValue < 1
                || args.NewValue > int.MaxValue
                || args.NewValue != Math.Truncate(args.NewValue))
            {
                Update(_request);
                return;
            }

            var result = _request.NextNumberSelectionSink.SetNextNumber(
                new SetNextNumberRequest(
                    _request.SessionId,
                    _request.CoordinateVersion,
                    _request.Selection.SelectionRevision,
                    _request.AnnotationRevision,
                    (int)args.NewValue));
            if (result.Kind is not SetNextNumberResultKind.Succeeded
                and not SetNextNumberResultKind.NoChange)
            {
                Update(_request);
            }
        }

        private void OnNextNumberTextChanged(object sender, TextChangedEventArgs args)
        {
            if (_disposed || _updatingNextNumber || _request.NextNumberSelectionSink is null
                || !TryParseTextValue(_nextNumberTextBox.Text, out var value)
                || !IsWholeNumber(value, 1, int.MaxValue))
            {
                return;
            }

            var result = _request.NextNumberSelectionSink.SetNextNumber(
                new SetNextNumberRequest(
                    _request.SessionId,
                    _request.CoordinateVersion,
                    _request.Selection.SelectionRevision,
                    _request.AnnotationRevision,
                    (int)value));
            if (result.Kind is not SetNextNumberResultKind.Succeeded
                and not SetNextNumberResultKind.NoChange)
            {
                Update(_request);
            }
        }

        private void UpdateStyleControls(FunctionBarPresentationRequest request)
        {
            var selected = request.SelectedObject?.OriginalObject;
            var selectedContent = selected?.Content;
            var sink = request.AnnotationObjectEditingSink;
            var objectSelected = selected is not null
                && request.SelectedObject?.HasSelection == true
                && sink is not null;
            var creationDefaults = !objectSelected && request.ActiveTool != EditingToolKind.Selection
                && sink is not null;
            var styleControlsActive = objectSelected || creationDefaults;
            _colorBox.Visibility = styleControlsActive ? Visibility.Visible : Visibility.Collapsed;
            _deleteObjectButton.Visibility = objectSelected ? Visibility.Visible : Visibility.Collapsed;
            _editTextButton.Visibility = selected?.ToolKind == AnnotationToolKind.Text && objectSelected
                ? Visibility.Visible
                : Visibility.Collapsed;
            var selectedPaint = selected?.ToolKind is AnnotationToolKind.Rectangle
                or AnnotationToolKind.ArrowLine
                or AnnotationToolKind.HighlighterStroke;
            var creationPaint = creationDefaults && request.ActiveTool is EditingToolKind.Rectangle
                    or EditingToolKind.ArrowLine
                    or EditingToolKind.Highlighter;
            _thicknessBox.Visibility = selectedPaint || creationPaint
                ? Visibility.Visible
                : Visibility.Collapsed;
            _thicknessTextBox.Visibility = _thicknessBox.Visibility;
            var textStyleActive = selected?.ToolKind == AnnotationToolKind.Text
                || (creationDefaults && request.ActiveTool == EditingToolKind.Text);
            _fontSizeBox.Visibility = textStyleActive
                ? Visibility.Visible
                : Visibility.Collapsed;
            _fontSizeTextBox.Visibility = _fontSizeBox.Visibility;
            _boldToggle.Visibility = textStyleActive
                ? Visibility.Visible
                : Visibility.Collapsed;
            var markerStyleActive = selected?.ToolKind == AnnotationToolKind.NumberedMarker
                || (creationDefaults && request.ActiveTool == EditingToolKind.NumberedMarker);
            _markerSizeBox.Visibility = markerStyleActive
                ? Visibility.Visible
                : Visibility.Collapsed;
            _markerSizeTextBox.Visibility = _markerSizeBox.Visibility;
            _colorBox.IsEnabled = styleControlsActive;
            _deleteObjectButton.IsEnabled = objectSelected;
            _editTextButton.IsEnabled = objectSelected && selected?.ToolKind == AnnotationToolKind.Text;
            _thicknessBox.IsEnabled = styleControlsActive;
            _thicknessTextBox.IsEnabled = styleControlsActive;
            _fontSizeBox.IsEnabled = styleControlsActive;
            _fontSizeTextBox.IsEnabled = styleControlsActive;
            _boldToggle.IsEnabled = styleControlsActive;
            _markerSizeBox.IsEnabled = styleControlsActive;
            _markerSizeTextBox.IsEnabled = styleControlsActive;

            _updatingStyleControls = true;
            try
            {
                if (selectedContent is RectangleAnnotationContent rectangle)
                {
                    SelectColor(rectangle.Style.StrokeColor);
                    _thicknessBox.Value = rectangle.Style.StrokeThickness;
                    SetTextValue(_thicknessTextBox, rectangle.Style.StrokeThickness);
                }
                else if (selectedContent is ArrowLineAnnotationContent arrow)
                {
                    SelectColor(arrow.Style.StrokeColor);
                    _thicknessBox.Value = arrow.Style.StrokeThickness;
                    SetTextValue(_thicknessTextBox, arrow.Style.StrokeThickness);
                    SetModeButtonState(_arrowLineModeButtons, arrow.Style.EndStyle);
                }
                else if (selectedContent is HighlighterStrokeContent highlighter)
                {
                    SelectColor(highlighter.Style.StrokeColor);
                    _thicknessBox.Value = highlighter.Style.StrokeThickness;
                    SetTextValue(_thicknessTextBox, highlighter.Style.StrokeThickness);
                }
                else if (selectedContent is TextAnnotationContent text)
                {
                    SelectColor(text.Style.Color);
                    _fontSizeBox.Value = text.Style.FontSize;
                    SetTextValue(_fontSizeTextBox, text.Style.FontSize);
                    _boldToggle.IsChecked = text.Style.Bold;
                }
                else if (selectedContent is NumberedMarkerAnnotationContent marker)
                {
                    SelectColor(marker.Style.Color);
                    _markerSizeBox.Value = marker.Style.Size;
                    SetTextValue(_markerSizeTextBox, marker.Style.Size);
                }
                else if (selectedContent is PrivacyRegionAnnotationContent privacy)
                {
                    SetModeButtonState(_privacyModeButtons, privacy.Mode);
                }
                else if (request.ActiveTool == EditingToolKind.Rectangle)
                {
                    SelectColor(request.ActiveRectangleStyle.StrokeColor);
                    _thicknessBox.Value = request.ActiveRectangleStyle.StrokeThickness;
                    SetTextValue(_thicknessTextBox, request.ActiveRectangleStyle.StrokeThickness);
                }
                else if (request.ActiveTool == EditingToolKind.ArrowLine)
                {
                    SelectColor(request.ActiveArrowLineStyle.StrokeColor);
                    _thicknessBox.Value = request.ActiveArrowLineStyle.StrokeThickness;
                    SetTextValue(_thicknessTextBox, request.ActiveArrowLineStyle.StrokeThickness);
                }
                else if (request.ActiveTool == EditingToolKind.Highlighter)
                {
                    SelectColor(request.ActiveHighlighterStyle.StrokeColor);
                    _thicknessBox.Value = request.ActiveHighlighterStyle.StrokeThickness;
                    SetTextValue(_thicknessTextBox, request.ActiveHighlighterStyle.StrokeThickness);
                }
                else if (request.ActiveTool == EditingToolKind.Text)
                {
                    SelectColor(request.ActiveTextStyle.Color);
                    _fontSizeBox.Value = request.ActiveTextStyle.FontSize;
                    SetTextValue(_fontSizeTextBox, request.ActiveTextStyle.FontSize);
                    _boldToggle.IsChecked = request.ActiveTextStyle.Bold;
                }
                else if (request.ActiveTool == EditingToolKind.NumberedMarker)
                {
                    SelectColor(request.ActiveNumberedMarkerStyle.Color);
                    _markerSizeBox.Value = request.ActiveNumberedMarkerStyle.Size;
                    SetTextValue(_markerSizeTextBox, request.ActiveNumberedMarkerStyle.Size);
                }
            }
            finally
            {
                _updatingStyleControls = false;
            }
        }

        private void SelectColor(ArgbColor color)
        {
            for (var index = 0; index < _colorBox.Items.Count; index++)
            {
                if (_colorBox.Items[index] is ComboBoxItem item
                    && item.Tag is string colorName
                    && ColorOptions.TryGetValue(colorName, out var candidate)
                    && candidate == color)
                {
                    _colorBox.SelectedIndex = index;
                    return;
                }
            }

            _colorBox.SelectedIndex = -1;
        }

        private static void SetModeButtonState<T>(
            IReadOnlyDictionary<T, RadioButton> buttons,
            T value)
            where T : struct, Enum
        {
            foreach (var pair in buttons)
            {
                pair.Value.IsChecked = EqualityComparer<T>.Default.Equals(pair.Key, value);
            }
        }

        private void ApplySelectedStyle(AnnotationObjectStyleChange change)
        {
            if (_disposed || _updatingStyleControls
                || _request.AnnotationObjectEditingSink is not IAnnotationObjectEditingSink sink
                )
            {
                return;
            }

            var objectId = _request.SelectedObject?.SelectedObjectId;
            var result = sink.ChangeStyle(new AnnotationObjectStyleChangeRequest(
                _request.SessionId,
                _request.CoordinateVersion,
                _request.Selection.SelectionRevision,
                _request.AnnotationRevision,
                objectId,
                change));
            if (result.Kind is not AnnotationObjectEditResultKind.Restyled)
            {
                Update(_request);
            }
        }

        private void OnColorChanged(object sender, SelectionChangedEventArgs args)
        {
            if (_colorBox.SelectedItem is ComboBoxItem { Tag: string colorName }
                && ColorOptions.TryGetValue(colorName, out var color))
            {
                ApplySelectedStyle(new AnnotationObjectStyleChange(Color: color));
            }
        }

        private void OnThicknessChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (IsWholeNumber(args.NewValue, 1, 64))
            {
                ApplySelectedStyle(new AnnotationObjectStyleChange(Thickness: (int)args.NewValue));
            }
        }

        private void OnFontSizeChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (double.IsFinite(args.NewValue)
                && args.NewValue >= TextAnnotationStyle.MinFontSize
                && args.NewValue <= TextAnnotationStyle.MaxFontSize)
            {
                ApplySelectedStyle(new AnnotationObjectStyleChange(FontSize: args.NewValue));
            }
        }

        private void OnMarkerSizeChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            if (IsWholeNumber(args.NewValue, NumberedMarkerAnnotationStyle.MinSize,
                NumberedMarkerAnnotationStyle.MaxSize))
            {
                ApplySelectedStyle(new AnnotationObjectStyleChange(MarkerSize: (int)args.NewValue));
            }
        }

        private void OnThicknessTextChanged(object sender, TextChangedEventArgs args)
        {
            if (_disposed || _updatingStyleControls
                || !TryParseTextValue(_thicknessTextBox.Text, out var value))
            {
                return;
            }

            if (IsWholeNumber(value, 1, 64))
            {
                ApplySelectedStyle(new AnnotationObjectStyleChange(Thickness: (int)value));
            }
        }

        private void OnFontSizeTextChanged(object sender, TextChangedEventArgs args)
        {
            if (_disposed || _updatingStyleControls
                || !TryParseTextValue(_fontSizeTextBox.Text, out var value))
            {
                return;
            }

            if (value >= TextAnnotationStyle.MinFontSize
                && value <= TextAnnotationStyle.MaxFontSize)
            {
                ApplySelectedStyle(new AnnotationObjectStyleChange(FontSize: value));
            }
        }

        private void OnMarkerSizeTextChanged(object sender, TextChangedEventArgs args)
        {
            if (_disposed || _updatingStyleControls
                || !TryParseTextValue(_markerSizeTextBox.Text, out var value))
            {
                return;
            }

            if (IsWholeNumber(value, NumberedMarkerAnnotationStyle.MinSize,
                NumberedMarkerAnnotationStyle.MaxSize))
            {
                ApplySelectedStyle(new AnnotationObjectStyleChange(MarkerSize: (int)value));
            }
        }

        private void OnBoldChanged(object sender, RoutedEventArgs args) =>
            ApplySelectedStyle(new AnnotationObjectStyleChange(Bold: _boldToggle.IsChecked == true));

        private void OnDeleteObjectClicked(object sender, RoutedEventArgs args)
        {
            if (_disposed
                || _request.AnnotationObjectEditingSink is not IAnnotationObjectEditingSink sink
                || _request.SelectedObject?.SelectedObjectId is not AnnotationObjectId objectId)
            {
                return;
            }

            sink.Delete(new AnnotationObjectDeleteRequest(
                _request.SessionId,
                _request.CoordinateVersion,
                _request.Selection.SelectionRevision,
                _request.AnnotationRevision,
                objectId));
        }

        private void OnEditTextClicked(object sender, RoutedEventArgs args)
        {
            if (_disposed
                || _request.AnnotationObjectEditingSink is not IAnnotationObjectEditingSink sink
                || _request.SelectedObject?.SelectedObjectId is not AnnotationObjectId objectId)
            {
                return;
            }

            sink.BeginTextEdit(new AnnotationObjectSelectionRequest(
                _request.SessionId,
                _request.CoordinateVersion,
                _request.Selection.SelectionRevision,
                _request.AnnotationRevision,
                objectId));
        }

        private static bool IsWholeNumber(double value, double minimum, double maximum) =>
            double.IsFinite(value)
            && value >= minimum
            && value <= maximum
            && value == Math.Truncate(value);

        private static bool TryParseTextValue(string text, out double value) =>
            double.TryParse(
                text,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out value);

        private static void SetTextValue(TextBox box, double value) =>
            box.Text = value.ToString("0.##", CultureInfo.InvariantCulture);

        private void OnArrowModeClicked(object sender, RoutedEventArgs args) =>
            SelectArrowLineMode(ArrowLineEndStyle.Arrow);

        private void OnLineModeClicked(object sender, RoutedEventArgs args) =>
            SelectArrowLineMode(ArrowLineEndStyle.None);

        private void OnMosaicModeClicked(object sender, RoutedEventArgs args) =>
            SelectPrivacyMode(PrivacyRegionMode.Mosaic);

        private void OnBlurModeClicked(object sender, RoutedEventArgs args) =>
            SelectPrivacyMode(PrivacyRegionMode.Blur);

        private void SelectArrowLineMode(ArrowLineEndStyle mode)
        {
            if (_request.SelectedObject?.SelectedObjectId is AnnotationObjectId objectId
                && _request.AnnotationObjectEditingSink is IAnnotationObjectEditingSink sink
                && _request.SelectedObject.OriginalObject?.ToolKind == AnnotationToolKind.ArrowLine)
            {
                ApplySelectedStyle(new AnnotationObjectStyleChange(ArrowLineEndStyle: mode));
                return;
            }

            SelectTool(EditingToolKind.ArrowLine, mode);
        }

        private void SelectPrivacyMode(PrivacyRegionMode mode)
        {
            if (_request.SelectedObject?.SelectedObjectId is AnnotationObjectId objectId
                && _request.AnnotationObjectEditingSink is IAnnotationObjectEditingSink sink
                && _request.SelectedObject.OriginalObject?.ToolKind == AnnotationToolKind.PrivacyRegion)
            {
                ApplySelectedStyle(new AnnotationObjectStyleChange(PrivacyMode: mode));
                return;
            }

            SelectPrivacyRegionMode(mode);
        }

        private void SelectTool(
            EditingToolKind tool,
            ArrowLineEndStyle arrowLineEndStyle = ArrowLineEndStyle.Arrow,
            PrivacyRegionMode? privacyMode = null)
        {
            var sink = _request.ToolSelectionSink;
            if (_disposed || sink is null)
            {
                return;
            }

            _owner.TraceFunctionBarDiagnostic(
                "FunctionBar.Tool.Clicked",
                $"Tool={tool}");
            var result = sink.SelectTool(new EditingToolSelectionRequest(
                _request.SessionId,
                _request.CoordinateVersion,
                _request.Selection.SelectionRevision,
                _request.AnnotationRevision,
                tool)
            {
                RequestedArrowLineEndStyle = arrowLineEndStyle,
                RequestedPrivacyRegionMode = privacyMode
            });
            _owner.TraceFunctionBarDiagnostic(
                "FunctionBar.Tool.Result",
                $"Tool={tool};Result={result.Kind}");
            if (result.Kind != EditingToolSelectionResultKind.Selected)
            {
                Update(_request);
            }
        }

        private void SelectPrivacyRegionMode(PrivacyRegionMode mode)
        {
            var sink = _request.PrivacyRegionModeSelectionSink;
            if (_disposed || sink is null)
            {
                return;
            }

            var result = sink.SelectPrivacyRegionMode(new PrivacyRegionModeSelectionRequest(
                _request.SessionId,
                _request.CoordinateVersion,
                _request.Selection.SelectionRevision,
                _request.AnnotationRevision,
                mode));
            if (result.Kind != PrivacyRegionModeSelectionResultKind.Selected)
            {
                Update(_request);
            }
        }

        private static RadioButton CreatePrivacyModeButton(string label)
        {
            var button = CreateModeButton(label);
            button.GroupName = "SnipPlusPrivacyRegionMode";
            Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(
                button,
                $"Privacy Region mode {label}");
            return button;
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs args)
        {
            if (!_initialized || _disposed)
            {
                return;
            }

            args.Handled = true;
        }

        private sealed class CancelCommandGate
        {
            private int _pending;

            public bool TryBegin() => Interlocked.Exchange(ref _pending, 1) == 0;

            public void Reset() => Volatile.Write(ref _pending, 0);
        }

        private readonly record struct FunctionBarButtonVisualStyle(
            global::Windows.UI.Color Foreground,
            global::Windows.UI.Color Background,
            global::Windows.UI.Color Border);
    }

    private sealed class SessionInputBoundary : ISelectionInputSink
    {
        private readonly object _gate = new();
        private readonly Guid _sessionId;
        private readonly string _coordinateVersion;
        private readonly ISelectionInputSink _inner;
        private readonly IEditingInputRouter? _editingRouter;
        private SelectionInputResult _lastResult;
        private RectanglePointerResult _lastRectangleResult;
        private ArrowLinePointerResult _lastArrowLineResult;
        private HighlighterPointerResult _lastHighlighterResult;
        private PrivacyRegionPointerResult _lastPrivacyRegionResult;
        private NumberedMarkerPointerResult _lastNumberedMarkerResult;
        private AnnotationObjectEditResult? _lastObjectResult;
        private TextDraftResult _lastTextResult;
        private TextDraftRequest? _textRequest;
        private int? _activePointerId;
        private int? _rectangleSelectionRevision;
        private AnnotationRevision? _rectangleAnnotationRevision;
        private int? _arrowLineSelectionRevision;
        private AnnotationRevision? _arrowLineAnnotationRevision;
        private int? _highlighterSelectionRevision;
        private AnnotationRevision? _highlighterAnnotationRevision;
        private int? _privacySelectionRevision;
        private AnnotationRevision? _privacyAnnotationRevision;
        private int? _numberedMarkerSelectionRevision;
        private AnnotationRevision? _numberedMarkerAnnotationRevision;
        private int? _objectSelectionRevision;
        private AnnotationRevision? _objectAnnotationRevision;
        private int? _objectPointerId;
        private bool _releaseConsumed;

        public SessionInputBoundary(
            Guid sessionId,
            string coordinateVersion,
            ISelectionInputSink inner)
            : this(sessionId, coordinateVersion, inner, null)
        {
        }

        public SessionInputBoundary(
            Guid sessionId,
            string coordinateVersion,
            ISelectionInputSink inner,
            IEditingInputRouter? editingRouter)
        {
            _sessionId = sessionId;
            _coordinateVersion = coordinateVersion
                ?? throw new ArgumentNullException(nameof(coordinateVersion));
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _editingRouter = editingRouter;
            _lastResult = new SelectionInputResult(
                SelectionInputResultKind.Ignored,
                SelectionVisualState.Initial(sessionId, coordinateVersion),
                "No Selection input has been accepted.");
            _lastRectangleResult = new RectanglePointerResult(
                RectanglePointerResultKind.NoActiveDraft,
                editingRouter?.ActiveTool ?? EditingToolKind.Selection,
                sessionId,
                coordinateVersion,
                editingRouter?.CurrentSelectionRevision ?? 0,
                editingRouter?.CurrentAnnotationRevision ?? AnnotationRevision.Initial,
                null,
                null,
                null,
                null,
                "No Rectangle input has been accepted.");
            _lastArrowLineResult = new ArrowLinePointerResult(
                ArrowLinePointerResultKind.NoActiveDraft,
                editingRouter?.ActiveTool ?? EditingToolKind.Selection,
                editingRouter?.ActiveArrowLineEndStyle ?? ArrowLineEndStyle.Arrow,
                sessionId,
                coordinateVersion,
                editingRouter?.CurrentSelectionRevision ?? 0,
                editingRouter?.CurrentAnnotationRevision ?? AnnotationRevision.Initial,
                null,
                null,
                null,
                null,
                "No Arrow or line input has been accepted.");
            _lastHighlighterResult = new HighlighterPointerResult(
                HighlighterPointerResultKind.NoActiveDraft,
                editingRouter?.ActiveTool ?? EditingToolKind.Selection,
                editingRouter?.ActiveHighlighterStyle ?? HighlighterAnnotationStyle.Default,
                sessionId,
                coordinateVersion,
                editingRouter?.CurrentSelectionRevision ?? 0,
                editingRouter?.CurrentAnnotationRevision ?? AnnotationRevision.Initial,
                null,
                null,
                null,
                null,
                "No Highlighter input has been accepted.");
            _lastPrivacyRegionResult = new PrivacyRegionPointerResult(
                PrivacyRegionPointerResultKind.NoActiveDraft,
                editingRouter?.ActiveTool ?? EditingToolKind.Selection,
                editingRouter?.ActivePrivacyRegionMode ?? PrivacyRegionMode.Mosaic,
                editingRouter?.ActivePrivacyRegionEffectParameters
                    ?? new(
                        PrivacyRegionEffectParameters.MinMosaicBlockSize,
                        PrivacyRegionEffectParameters.MinBlurRadius),
                sessionId,
                coordinateVersion,
                editingRouter?.CurrentSelectionRevision ?? 0,
                editingRouter?.CurrentAnnotationRevision ?? AnnotationRevision.Initial,
                null,
                null,
                null,
                null,
                null,
                "No Privacy Region input has been accepted.");
            _lastNumberedMarkerResult = new NumberedMarkerPointerResult(
                NumberedMarkerPointerResultKind.NoActiveDraft,
                editingRouter?.ActiveTool ?? EditingToolKind.Selection,
                editingRouter?.ActiveNumberedMarkerNextNumber ?? 1,
                editingRouter?.ActiveNumberedMarkerStyle ?? NumberedMarkerAnnotationStyle.Default,
                sessionId,
                coordinateVersion,
                editingRouter?.CurrentSelectionRevision ?? 0,
                editingRouter?.CurrentAnnotationRevision ?? AnnotationRevision.Initial,
                null,
                null,
                null,
                null,
                "No Numbered Marker input has been accepted.");
            _lastTextResult = new TextDraftResult(
                TextDraftResultKind.NoActiveDraft,
                editingRouter?.ActiveTool ?? EditingToolKind.Selection,
                sessionId,
                coordinateVersion,
                editingRouter?.CurrentSelectionRevision ?? 0,
                editingRouter?.CurrentAnnotationRevision ?? AnnotationRevision.Initial,
                null,
                string.Empty,
                editingRouter?.ActiveTextStyle ?? TextAnnotationStyle.Default,
                null,
                null,
                null,
                "No Text draft is active.");
        }

        public bool UsesRectangleTool =>
            _editingRouter?.ActiveTool == EditingToolKind.Rectangle;

        public bool UsesArrowLineTool =>
            _editingRouter?.ActiveTool == EditingToolKind.ArrowLine;

        public bool UsesHighlighterTool =>
            _editingRouter?.ActiveTool == EditingToolKind.Highlighter;

        public bool UsesTextTool =>
            _editingRouter?.ActiveTool == EditingToolKind.Text;

        public bool UsesPrivacyRegionTool =>
            _editingRouter?.ActiveTool == EditingToolKind.PrivacyRegion;

        public bool UsesNumberedMarkerTool =>
            _editingRouter?.ActiveTool == EditingToolKind.NumberedMarker;

        public bool UsesObjectEditing =>
            _editingRouter?.IsObjectEditingEnabled == true;

        public IEditingInputRouter? EditingRouter => _editingRouter;

        public TextDraftResult PointerPressedText(SelectionPointerEvent input)
        {
            ArgumentNullException.ThrowIfNull(input);
            if (_editingRouter is null)
            {
                return _lastTextResult;
            }

            lock (_gate)
            {
                if (_activePointerId is not null)
                {
                    return _lastTextResult with
                    {
                        Kind = TextDraftResultKind.DraftMismatch,
                        Message = "Another pointer interaction is already active."
                    };
                }
            }

            var result = _editingRouter.BeginTextDraft(new TextDraftPointerEvent(
                input.SessionId,
                input.CoordinateVersion,
                _editingRouter.CurrentSelectionRevision,
                _editingRouter.CurrentAnnotationRevision,
                input.PointerId,
                input.GlobalPhysicalPoint));
            lock (_gate)
            {
                _lastTextResult = result;
                _textRequest = result.Request;
            }

            return result;
        }

        public TextDraftResult UpdateTextDraftContent(string text)
        {
            if (_editingRouter is null)
            {
                return _lastTextResult;
            }

            TextDraftRequest? request;
            lock (_gate)
            {
                request = _textRequest;
            }

            if (request is null)
            {
                return _lastTextResult;
            }

            var result = _editingRouter.UpdateTextDraftContent(request, text);
            lock (_gate)
            {
                _lastTextResult = result;
                _textRequest = result.Kind is TextDraftResultKind.Committed
                    or TextDraftResultKind.Cancelled
                    ? null
                    : result.Request;
            }

            return result;
        }

        public TextDraftResult UpdateTextDraftStyle(TextAnnotationStyle? style)
        {
            if (_editingRouter is null)
            {
                return _lastTextResult;
            }

            TextDraftRequest? request;
            lock (_gate)
            {
                request = _textRequest;
            }

            if (request is null)
            {
                return _lastTextResult;
            }

            var result = _editingRouter.UpdateTextDraftStyle(request, style);
            lock (_gate)
            {
                _lastTextResult = result;
                _textRequest = result.Request;
            }

            return result;
        }

        public TextDraftResult CommitTextDraft()
        {
            if (_editingRouter is null)
            {
                return _lastTextResult;
            }

            TextDraftRequest? request;
            lock (_gate)
            {
                request = _textRequest;
            }

            if (request is null)
            {
                return _lastTextResult;
            }

            var result = _editingRouter.CommitTextDraft(request);
            lock (_gate)
            {
                _lastTextResult = result;
                _textRequest = result.Kind == TextDraftResultKind.Committed
                    ? null
                    : result.Request;
            }

            return result;
        }

        public TextDraftResult CancelTextDraft()
        {
            if (_editingRouter is null)
            {
                return _lastTextResult;
            }

            TextDraftRequest? request;
            lock (_gate)
            {
                request = _textRequest;
            }

            if (request is null)
            {
                return _lastTextResult;
            }

            var result = _editingRouter.CancelTextDraft(request);
            lock (_gate)
            {
                _lastTextResult = result;
                _textRequest = null;
            }

            return result;
        }

        public AnnotationObjectEditResult PointerPressedObject(SelectionPointerEvent input)
        {
            ArgumentNullException.ThrowIfNull(input);
            if (_editingRouter is null)
            {
                return _lastObjectResult ?? new AnnotationObjectEditResult(
                    AnnotationObjectEditResultKind.StaleSession,
                    AnnotationObjectSelectionState.Empty(
                        input.SessionId,
                        input.CoordinateVersion,
                        0,
                        AnnotationRevision.Initial),
                    null,
                    null,
                    null,
                    "Annotation object input is unavailable.");
            }

            lock (_gate)
            {
                if (_activePointerId is not null)
                {
                    return (_lastObjectResult ?? ObjectResultFallback()) with
                    {
                        Kind = AnnotationObjectEditResultKind.PointerMismatch,
                        Message = "Another pointer interaction is already active."
                    };
                }
            }

            var result = _editingRouter.PointerPressed(new AnnotationObjectPointerEvent(
                input.SessionId,
                input.CoordinateVersion,
                _editingRouter.CurrentSelectionRevision,
                _editingRouter.CurrentAnnotationRevision,
                input.PointerId,
                input.GlobalPhysicalPoint));
            lock (_gate)
            {
                _lastObjectResult = result;
                if (result.Kind == AnnotationObjectEditResultKind.EditStarted)
                {
                    _activePointerId = input.PointerId;
                    _objectPointerId = input.PointerId;
                    _objectSelectionRevision = _editingRouter.CurrentSelectionRevision;
                    _objectAnnotationRevision = _editingRouter.CurrentAnnotationRevision;
                    _releaseConsumed = false;
                }
            }

            return result;
        }

        public AnnotationObjectEditResult PointerMovedObject(SelectionPointerEvent input)
        {
            ArgumentNullException.ThrowIfNull(input);
            if (_editingRouter is null || _objectPointerId is not int pointerId)
            {
                return _lastObjectResult ?? ObjectResultFallback();
            }

            var result = _editingRouter.PointerMoved(new AnnotationObjectPointerEvent(
                input.SessionId,
                input.CoordinateVersion,
                _objectSelectionRevision ?? _editingRouter.CurrentSelectionRevision,
                _objectAnnotationRevision ?? _editingRouter.CurrentAnnotationRevision,
                pointerId,
                input.GlobalPhysicalPoint));
            lock (_gate)
            {
                _lastObjectResult = result;
            }

            return result;
        }

        public AnnotationObjectEditResult PointerReleasedObject(SelectionPointerEvent input)
        {
            ArgumentNullException.ThrowIfNull(input);
            if (_editingRouter is null || _objectPointerId is not int pointerId)
            {
                return _lastObjectResult ?? ObjectResultFallback();
            }

            lock (_gate)
            {
                if (_releaseConsumed)
                {
                    return _lastObjectResult ?? ObjectResultFallback();
                }

                _releaseConsumed = true;
            }

            var result = _editingRouter.PointerReleased(new AnnotationObjectPointerEvent(
                input.SessionId,
                input.CoordinateVersion,
                _objectSelectionRevision ?? _editingRouter.CurrentSelectionRevision,
                _objectAnnotationRevision ?? _editingRouter.CurrentAnnotationRevision,
                pointerId,
                input.GlobalPhysicalPoint));
            lock (_gate)
            {
                _lastObjectResult = result;
                _objectPointerId = null;
                _objectSelectionRevision = null;
                _objectAnnotationRevision = null;
                _activePointerId = null;
            }

            return result;
        }

        public AnnotationObjectEditResult PointerReleasedObjectFromNative(PhysicalPoint point) =>
            PointerReleasedObject(new SelectionPointerEvent(
                _sessionId,
                _coordinateVersion,
                _objectPointerId ?? 0,
                point));

        public SelectionInputResult PointerPressed(SelectionPointerEvent input)
        {
            ArgumentNullException.ThrowIfNull(input);
            lock (_gate)
            {
                if (_activePointerId is not null)
                {
                    return Ignored("Another Selection interaction is already active.");
                }
            }

            var result = _inner.PointerPressed(input);
            lock (_gate)
            {
                _lastResult = result;
                if (IsActiveInteraction(result.State.InteractionMode))
                {
                    _activePointerId = result.State.ActivePointerId ?? input.PointerId;
                    _releaseConsumed = false;
                }
            }

            return result;
        }

        public RectanglePointerResult PointerPressedRectangle(SelectionPointerEvent input)
        {
            ArgumentNullException.ThrowIfNull(input);
            if (_editingRouter is null)
            {
                return _lastRectangleResult;
            }

            lock (_gate)
            {
                if (_activePointerId is not null)
                {
                    return _lastRectangleResult with
                    {
                        Kind = RectanglePointerResultKind.PointerMismatch,
                        Message = "Another pointer interaction is already active."
                    };
                }
            }

            var result = _editingRouter.PointerPressed(ToRectangleInput(input));
            lock (_gate)
            {
                _lastRectangleResult = result;
                if (result.Kind == RectanglePointerResultKind.DraftStarted)
                {
                    _activePointerId = input.PointerId;
                    _rectangleSelectionRevision = _editingRouter.CurrentSelectionRevision;
                    _rectangleAnnotationRevision = _editingRouter.CurrentAnnotationRevision;
                    _releaseConsumed = false;
                }
            }

            return result;
        }

        public ArrowLinePointerResult PointerPressedArrowLine(SelectionPointerEvent input)
        {
            ArgumentNullException.ThrowIfNull(input);
            if (_editingRouter is null)
            {
                return _lastArrowLineResult;
            }

            lock (_gate)
            {
                if (_activePointerId is not null)
                {
                    return _lastArrowLineResult with
                    {
                        Kind = ArrowLinePointerResultKind.PointerMismatch,
                        Message = "Another pointer interaction is already active."
                    };
                }
            }

            var result = _editingRouter.PointerPressed(ToArrowLineInput(input));
            lock (_gate)
            {
                _lastArrowLineResult = result;
                if (result.Kind == ArrowLinePointerResultKind.DraftStarted)
                {
                    _activePointerId = input.PointerId;
                    _arrowLineSelectionRevision = _editingRouter.CurrentSelectionRevision;
                    _arrowLineAnnotationRevision = _editingRouter.CurrentAnnotationRevision;
                    _releaseConsumed = false;
                }
            }

            return result;
        }

        public HighlighterPointerResult PointerPressedHighlighter(SelectionPointerEvent input)
        {
            ArgumentNullException.ThrowIfNull(input);
            if (_editingRouter is null)
            {
                return _lastHighlighterResult;
            }

            lock (_gate)
            {
                if (_activePointerId is not null)
                {
                    return _lastHighlighterResult with
                    {
                        Kind = HighlighterPointerResultKind.PointerMismatch,
                        Message = "Another pointer interaction is already active."
                    };
                }
            }

            var result = _editingRouter.PointerPressed(ToHighlighterInput(input));
            lock (_gate)
            {
                _lastHighlighterResult = result;
                if (result.Kind == HighlighterPointerResultKind.DraftStarted)
                {
                    _activePointerId = input.PointerId;
                    _highlighterSelectionRevision = _editingRouter.CurrentSelectionRevision;
                    _highlighterAnnotationRevision = _editingRouter.CurrentAnnotationRevision;
                    _releaseConsumed = false;
                }
            }

            return result;
        }

        public PrivacyRegionPointerResult PointerPressedPrivacyRegion(SelectionPointerEvent input)
        {
            ArgumentNullException.ThrowIfNull(input);
            if (_editingRouter is null)
            {
                return _lastPrivacyRegionResult;
            }

            lock (_gate)
            {
                if (_activePointerId is not null)
                {
                    return _lastPrivacyRegionResult with
                    {
                        Kind = PrivacyRegionPointerResultKind.PointerMismatch,
                        Message = "Another pointer interaction is already active."
                    };
                }
            }

            var result = _editingRouter.PointerPressed(ToPrivacyRegionInput(input));
            lock (_gate)
            {
                _lastPrivacyRegionResult = result;
                if (result.Kind == PrivacyRegionPointerResultKind.DraftStarted)
                {
                    _activePointerId = input.PointerId;
                    _privacySelectionRevision = _editingRouter.CurrentSelectionRevision;
                    _privacyAnnotationRevision = _editingRouter.CurrentAnnotationRevision;
                    _releaseConsumed = false;
                }
            }

            return result;
        }

        public NumberedMarkerPointerResult PointerPressedNumberedMarker(SelectionPointerEvent input)
        {
            ArgumentNullException.ThrowIfNull(input);
            if (_editingRouter is null)
            {
                return _lastNumberedMarkerResult;
            }

            lock (_gate)
            {
                if (_activePointerId is not null)
                {
                    return _lastNumberedMarkerResult with
                    {
                        Kind = NumberedMarkerPointerResultKind.PointerMismatch,
                        Message = "Another pointer interaction is already active."
                    };
                }
            }

            var result = _editingRouter.PointerPressed(ToNumberedMarkerInput(input));
            lock (_gate)
            {
                _lastNumberedMarkerResult = result;
                if (result.Kind == NumberedMarkerPointerResultKind.DraftStarted)
                {
                    _activePointerId = input.PointerId;
                    _numberedMarkerSelectionRevision = _editingRouter.CurrentSelectionRevision;
                    _numberedMarkerAnnotationRevision = _editingRouter.CurrentAnnotationRevision;
                    _releaseConsumed = false;
                }
            }

            return result;
        }

        public SelectionInputResult PointerMoved(SelectionPointerEvent input)
        {
            ArgumentNullException.ThrowIfNull(input);
            var result = _inner.PointerMoved(NormalizePointer(input));
            lock (_gate)
            {
                _lastResult = result;
            }

            return result;
        }

        public RectanglePointerResult PointerMovedRectangle(SelectionPointerEvent input)
        {
            ArgumentNullException.ThrowIfNull(input);
            if (_editingRouter is null)
            {
                return _lastRectangleResult;
            }

            var result = _editingRouter.PointerMoved(
                ToRectangleInput(NormalizePointer(input)));
            lock (_gate)
            {
                _lastRectangleResult = result;
            }

            return result;
        }

        public ArrowLinePointerResult PointerMovedArrowLine(SelectionPointerEvent input)
        {
            ArgumentNullException.ThrowIfNull(input);
            if (_editingRouter is null)
            {
                return _lastArrowLineResult;
            }

            var result = _editingRouter.PointerMoved(
                ToArrowLineInput(NormalizePointer(input)));
            lock (_gate)
            {
                _lastArrowLineResult = result;
            }

            return result;
        }

        public HighlighterPointerResult PointerMovedHighlighter(SelectionPointerEvent input)
        {
            ArgumentNullException.ThrowIfNull(input);
            if (_editingRouter is null)
            {
                return _lastHighlighterResult;
            }

            var result = _editingRouter.PointerMoved(
                ToHighlighterInput(NormalizePointer(input)));
            lock (_gate)
            {
                _lastHighlighterResult = result;
            }

            return result;
        }

        public PrivacyRegionPointerResult PointerMovedPrivacyRegion(SelectionPointerEvent input)
        {
            ArgumentNullException.ThrowIfNull(input);
            if (_editingRouter is null)
            {
                return _lastPrivacyRegionResult;
            }

            var result = _editingRouter.PointerMoved(
                ToPrivacyRegionInput(NormalizePointer(input)));
            lock (_gate)
            {
                _lastPrivacyRegionResult = result;
            }

            return result;
        }

        public NumberedMarkerPointerResult PointerMovedNumberedMarker(SelectionPointerEvent input)
        {
            ArgumentNullException.ThrowIfNull(input);
            if (_editingRouter is null)
            {
                return _lastNumberedMarkerResult;
            }

            var result = _editingRouter.PointerMoved(
                ToNumberedMarkerInput(NormalizePointer(input)));
            lock (_gate)
            {
                _lastNumberedMarkerResult = result;
            }

            return result;
        }

        public SelectionInputResult PointerReleased(SelectionPointerEvent input)
        {
            ArgumentNullException.ThrowIfNull(input);
            lock (_gate)
            {
                if (_releaseConsumed || _activePointerId is null)
                {
                    return _lastResult;
                }

                _releaseConsumed = true;
            }

            var result = _inner.PointerReleased(NormalizePointer(input));
            lock (_gate)
            {
                _lastResult = result;
                _activePointerId = null;
            }

            return result;
        }

        public RectanglePointerResult PointerReleasedRectangle(SelectionPointerEvent input)
        {
            ArgumentNullException.ThrowIfNull(input);
            if (_editingRouter is null)
            {
                return _lastRectangleResult;
            }

            lock (_gate)
            {
                if (_releaseConsumed || _activePointerId is null)
                {
                    return _lastRectangleResult;
                }

                _releaseConsumed = true;
            }

            var result = _editingRouter.PointerReleased(
                ToRectangleInput(NormalizePointer(input)));
            lock (_gate)
            {
                _lastRectangleResult = result;
                _activePointerId = null;
                _rectangleSelectionRevision = null;
                _rectangleAnnotationRevision = null;
            }

            return result;
        }

        public ArrowLinePointerResult PointerReleasedArrowLine(SelectionPointerEvent input)
        {
            ArgumentNullException.ThrowIfNull(input);
            if (_editingRouter is null)
            {
                return _lastArrowLineResult;
            }

            lock (_gate)
            {
                if (_releaseConsumed || _activePointerId is null)
                {
                    return _lastArrowLineResult;
                }

                _releaseConsumed = true;
            }

            var result = _editingRouter.PointerReleased(
                ToArrowLineInput(NormalizePointer(input)));
            lock (_gate)
            {
                _lastArrowLineResult = result;
                _activePointerId = null;
                _arrowLineSelectionRevision = null;
                _arrowLineAnnotationRevision = null;
            }

            return result;
        }

        public HighlighterPointerResult PointerReleasedHighlighter(SelectionPointerEvent input)
        {
            ArgumentNullException.ThrowIfNull(input);
            if (_editingRouter is null)
            {
                return _lastHighlighterResult;
            }

            lock (_gate)
            {
                if (_releaseConsumed || _activePointerId is null)
                {
                    return _lastHighlighterResult;
                }

                _releaseConsumed = true;
            }

            var result = _editingRouter.PointerReleased(
                ToHighlighterInput(NormalizePointer(input)));
            lock (_gate)
            {
                _lastHighlighterResult = result;
                _activePointerId = null;
                _highlighterSelectionRevision = null;
                _highlighterAnnotationRevision = null;
            }

            return result;
        }

        public PrivacyRegionPointerResult PointerReleasedPrivacyRegion(SelectionPointerEvent input)
        {
            ArgumentNullException.ThrowIfNull(input);
            if (_editingRouter is null)
            {
                return _lastPrivacyRegionResult;
            }

            lock (_gate)
            {
                if (_releaseConsumed || _activePointerId is null)
                {
                    return _lastPrivacyRegionResult;
                }

                _releaseConsumed = true;
            }

            var result = _editingRouter.PointerReleased(
                ToPrivacyRegionInput(NormalizePointer(input)));
            lock (_gate)
            {
                _lastPrivacyRegionResult = result;
                _activePointerId = null;
                _privacySelectionRevision = null;
                _privacyAnnotationRevision = null;
                _numberedMarkerSelectionRevision = null;
                _numberedMarkerAnnotationRevision = null;
            }

            return result;
        }

        public NumberedMarkerPointerResult PointerReleasedNumberedMarker(SelectionPointerEvent input)
        {
            ArgumentNullException.ThrowIfNull(input);
            if (_editingRouter is null)
            {
                return _lastNumberedMarkerResult;
            }

            lock (_gate)
            {
                if (_releaseConsumed || _activePointerId is null)
                {
                    return _lastNumberedMarkerResult;
                }

                _releaseConsumed = true;
            }

            var result = _editingRouter.PointerReleased(
                ToNumberedMarkerInput(NormalizePointer(input)));
            lock (_gate)
            {
                _lastNumberedMarkerResult = result;
                _activePointerId = null;
                _numberedMarkerSelectionRevision = null;
                _numberedMarkerAnnotationRevision = null;
            }

            return result;
        }

        public SelectionInputResult PointerReleasedFromNative(PhysicalPoint point) =>
            PointerReleased(new SelectionPointerEvent(
                _sessionId,
                _coordinateVersion,
                GetActivePointerId(),
                point));

        public RectanglePointerResult PointerReleasedRectangleFromNative(PhysicalPoint point) =>
            PointerReleasedRectangle(new SelectionPointerEvent(
                _sessionId,
                _coordinateVersion,
                GetActivePointerId(),
                point));

        public ArrowLinePointerResult PointerReleasedArrowLineFromNative(PhysicalPoint point) =>
            PointerReleasedArrowLine(new SelectionPointerEvent(
                _sessionId,
                _coordinateVersion,
                GetActivePointerId(),
                point));

        public HighlighterPointerResult PointerReleasedHighlighterFromNative(PhysicalPoint point) =>
            PointerReleasedHighlighter(new SelectionPointerEvent(
                _sessionId,
                _coordinateVersion,
                GetActivePointerId(),
                point));

        public PrivacyRegionPointerResult PointerReleasedPrivacyRegionFromNative(PhysicalPoint point) =>
            PointerReleasedPrivacyRegion(new SelectionPointerEvent(
                _sessionId,
                _coordinateVersion,
                GetActivePointerId(),
                point));

        public NumberedMarkerPointerResult PointerReleasedNumberedMarkerFromNative(PhysicalPoint point) =>
            PointerReleasedNumberedMarker(new SelectionPointerEvent(
                _sessionId,
                _coordinateVersion,
                GetActivePointerId(),
                point));

        public SelectionInputResult Escape(Guid sessionId, string coordinateVersion)
        {
            if (_editingRouter is not null && _editingRouter.IsObjectEditingEnabled)
            {
                _lastObjectResult = _editingRouter.CancelEdit(sessionId, coordinateVersion);
            }

            var result = _inner.Escape(sessionId, coordinateVersion);
            lock (_gate)
            {
                _lastResult = result;
                _activePointerId = null;
                _releaseConsumed = true;
                _rectangleSelectionRevision = null;
                _rectangleAnnotationRevision = null;
                _arrowLineSelectionRevision = null;
                _arrowLineAnnotationRevision = null;
                _highlighterSelectionRevision = null;
                _highlighterAnnotationRevision = null;
                _privacySelectionRevision = null;
                _privacyAnnotationRevision = null;
                _objectSelectionRevision = null;
                _objectAnnotationRevision = null;
                _objectPointerId = null;
                _lastRectangleResult = _editingRouter is null
                    ? _lastRectangleResult
                    : _lastRectangleResult with
                    {
                        Kind = RectanglePointerResultKind.Cancelled,
                        ActiveTool = _editingRouter.ActiveTool,
                        Message = "Rectangle input cancelled with the capture session."
                    };
                _lastArrowLineResult = _editingRouter is null
                    ? _lastArrowLineResult
                    : _lastArrowLineResult with
                    {
                        Kind = ArrowLinePointerResultKind.Cancelled,
                        ActiveTool = _editingRouter.ActiveTool,
                        ActiveEndStyle = _editingRouter.ActiveArrowLineEndStyle,
                        Message = "Arrow or line input cancelled with the capture session."
                    };
                _lastHighlighterResult = _editingRouter is null
                    ? _lastHighlighterResult
                    : _lastHighlighterResult with
                    {
                        Kind = HighlighterPointerResultKind.Cancelled,
                        ActiveTool = _editingRouter.ActiveTool,
                        ActiveStyle = _editingRouter.ActiveHighlighterStyle,
                        Message = "Highlighter input cancelled with the capture session."
                    };
                _lastPrivacyRegionResult = _editingRouter is null
                    ? _lastPrivacyRegionResult
                    : _lastPrivacyRegionResult with
                    {
                        Kind = PrivacyRegionPointerResultKind.Cancelled,
                        ActiveTool = _editingRouter.ActiveTool,
                        ActiveMode = _editingRouter.ActivePrivacyRegionMode,
                        ActiveEffectParameters = _editingRouter.ActivePrivacyRegionEffectParameters,
                        Message = "Privacy Region input cancelled with the capture session."
                    };
                _lastNumberedMarkerResult = _editingRouter is null
                    ? _lastNumberedMarkerResult
                    : _lastNumberedMarkerResult with
                    {
                        Kind = NumberedMarkerPointerResultKind.Cancelled,
                        ActiveTool = _editingRouter.ActiveTool,
                        ActiveNextNumber = _editingRouter.ActiveNumberedMarkerNextNumber,
                        ActiveStyle = _editingRouter.ActiveNumberedMarkerStyle,
                        Message = "Numbered Marker input cancelled with the capture session."
                    };
            }

            return result;
        }

        public static void NotifyCaptureChanged()
        {
            // Capture changes do not imply a mouse release. WM_LBUTTONUP or
            // XAML PointerReleased remains the only commit boundary.
        }

        private SelectionPointerEvent NormalizePointer(SelectionPointerEvent input)
        {
            lock (_gate)
            {
                return _activePointerId is int pointerId
                    ? input with { PointerId = pointerId }
                    : input;
            }
        }

        private RectanglePointerEvent ToRectangleInput(SelectionPointerEvent input) =>
            new(
                input.SessionId,
                input.CoordinateVersion,
                _rectangleSelectionRevision
                    ?? _editingRouter?.CurrentSelectionRevision
                    ?? 0,
                _rectangleAnnotationRevision
                    ?? _editingRouter?.CurrentAnnotationRevision
                    ?? AnnotationRevision.Initial,
                input.PointerId,
                input.GlobalPhysicalPoint);

        private ArrowLinePointerEvent ToArrowLineInput(SelectionPointerEvent input) =>
            new(
                input.SessionId,
                input.CoordinateVersion,
                _arrowLineSelectionRevision
                    ?? _editingRouter?.CurrentSelectionRevision
                    ?? 0,
                _arrowLineAnnotationRevision
                    ?? _editingRouter?.CurrentAnnotationRevision
                    ?? AnnotationRevision.Initial,
                input.PointerId,
                input.GlobalPhysicalPoint);

        private HighlighterPointerEvent ToHighlighterInput(SelectionPointerEvent input) =>
            new(
                input.SessionId,
                input.CoordinateVersion,
                _highlighterSelectionRevision
                    ?? _editingRouter?.CurrentSelectionRevision
                    ?? 0,
                _highlighterAnnotationRevision
                    ?? _editingRouter?.CurrentAnnotationRevision
                    ?? AnnotationRevision.Initial,
                input.PointerId,
                input.GlobalPhysicalPoint);

        private PrivacyRegionPointerEvent ToPrivacyRegionInput(SelectionPointerEvent input) =>
            new(
                input.SessionId,
                input.CoordinateVersion,
                _privacySelectionRevision
                    ?? _editingRouter?.CurrentSelectionRevision
                    ?? 0,
                _privacyAnnotationRevision
                    ?? _editingRouter?.CurrentAnnotationRevision
                    ?? AnnotationRevision.Initial,
                input.PointerId,
                input.GlobalPhysicalPoint);

        private NumberedMarkerPointerEvent ToNumberedMarkerInput(SelectionPointerEvent input) =>
            new(
                input.SessionId,
                input.CoordinateVersion,
                _numberedMarkerSelectionRevision
                    ?? _editingRouter?.CurrentSelectionRevision
                    ?? 0,
                _numberedMarkerAnnotationRevision
                    ?? _editingRouter?.CurrentAnnotationRevision
                    ?? AnnotationRevision.Initial,
                input.PointerId,
                input.GlobalPhysicalPoint);

        private int GetActivePointerId()
        {
            lock (_gate)
            {
                return _activePointerId ?? 0;
            }
        }

        private SelectionInputResult Ignored(string message)
        {
            lock (_gate)
            {
                return new SelectionInputResult(
                    SelectionInputResultKind.Ignored,
                    _lastResult.State,
                    message);
            }
        }

        private AnnotationObjectEditResult ObjectResultFallback()
        {
            var editingRouter = _editingRouter;
            return new AnnotationObjectEditResult(
                AnnotationObjectEditResultKind.NoActiveEdit,
                editingRouter?.SelectedObject
                    ?? AnnotationObjectSelectionState.Empty(
                        _sessionId,
                        _coordinateVersion,
                        editingRouter?.CurrentSelectionRevision ?? 0,
                        editingRouter?.CurrentAnnotationRevision ?? AnnotationRevision.Initial),
                null,
                null,
                null,
                "No annotation object edit is active.");
        }

        private static bool IsActiveInteraction(SelectionInteractionMode mode) => mode is
            SelectionInteractionMode.InitialDragging
            or SelectionInteractionMode.Moving
            or SelectionInteractionMode.ResizingLeft
            or SelectionInteractionMode.ResizingTop
            or SelectionInteractionMode.ResizingRight
            or SelectionInteractionMode.ResizingBottom
            or SelectionInteractionMode.ResizingTopLeft
            or SelectionInteractionMode.ResizingTopRight
            or SelectionInteractionMode.ResizingBottomLeft
            or SelectionInteractionMode.ResizingBottomRight
            or SelectionInteractionMode.Reselecting;
    }

    private sealed class OverlaySurface : IDisposable
    {
        private const int PerMonitorAwareV2 = -4;
        private const int GwlExStyle = -20;
        private const int SwHide = 0;
        private const nint WsExAppWindow = 0x00040000;
        private const nint WsExToolWindow = 0x00000080;
        private const int GwlWndProc = -4;
        private const uint WmLButtonUp = 0x0202;
        private const uint WmCaptureChanged = 0x0215;
        private const uint SwpNoSize = 0x0001;
        private const uint SwpNoMove = 0x0002;
        private const uint SwpNoZOrder = 0x0004;
        private const uint SwpNoActivate = 0x0010;
        private const uint SwpShowWindow = 0x0040;
        private const int OverlayMaskAlpha = 0x99;
        private const double HandleVisualSizePixels = 8;

        private readonly FrozenDisplayOverlayDescriptor _descriptor;
        private readonly SessionInputBoundary _inputBoundary;
        private readonly ICompleteExecutionTraceSink? _traceSink;
        private readonly Window _window = new();
        private readonly Grid _root = new();
        private readonly Image _image = new();
        private readonly CrosshairCanvas _canvas = new();
        private readonly Rectangle _maskTop = CreateMask();
        private readonly Rectangle _maskLeft = CreateMask();
        private readonly Rectangle _maskRight = CreateMask();
        private readonly Rectangle _maskBottom = CreateMask();
        private readonly Rectangle _selectionBorder = new()
        {
            Fill = new SolidColorBrush(ColorHelper.FromArgb(0, 0, 0, 0)),
            Stroke = new SolidColorBrush(ColorHelper.FromArgb(255, 187, 215, 255)),
            StrokeThickness = 2,
            Visibility = Visibility.Collapsed
        };
        private readonly Rectangle _objectSelectionBorder = new()
        {
            Fill = new SolidColorBrush(ColorHelper.FromArgb(0, 0, 0, 0)),
            Stroke = new SolidColorBrush(ColorHelper.FromArgb(255, 255, 255, 255)),
            StrokeThickness = 1,
            StrokeDashArray = new DoubleCollection { 3, 2 },
            Visibility = Visibility.Collapsed,
            IsHitTestVisible = false
        };
        private readonly IReadOnlyDictionary<AnnotationObjectEditHandleKind, Rectangle> _objectHandles =
            new Dictionary<AnnotationObjectEditHandleKind, Rectangle>
            {
                [AnnotationObjectEditHandleKind.StartEndpoint] = CreateHandle(),
                [AnnotationObjectEditHandleKind.EndEndpoint] = CreateHandle(),
                [AnnotationObjectEditHandleKind.LeftEdge] = CreateHandle(),
                [AnnotationObjectEditHandleKind.TopEdge] = CreateHandle(),
                [AnnotationObjectEditHandleKind.RightEdge] = CreateHandle(),
                [AnnotationObjectEditHandleKind.BottomEdge] = CreateHandle(),
                [AnnotationObjectEditHandleKind.TopLeftCorner] = CreateHandle(),
                [AnnotationObjectEditHandleKind.TopRightCorner] = CreateHandle(),
                [AnnotationObjectEditHandleKind.BottomLeftCorner] = CreateHandle(),
                [AnnotationObjectEditHandleKind.BottomRightCorner] = CreateHandle()
            };
        private readonly List<Rectangle> _annotationPreviews = new();
        private readonly List<Line> _arrowLinePreviews = new();
        private readonly List<FrameworkElement> _highlighterPreviews = new();
        private readonly List<TextBlock> _textPreviews = new();
        private readonly List<FrameworkElement> _numberedMarkerPreviews = new();
        private readonly List<PrivacyPreview> _privacyPreviews = new();
        private readonly PointerReleaseDispatchGate _pointerReleaseDispatchGate = new();
        private readonly Grid _textEditorHost = new()
        {
            Visibility = Visibility.Collapsed,
            IsHitTestVisible = true,
            Background = new SolidColorBrush(ColorHelper.FromArgb(235, 24, 24, 24))
        };
        private readonly TextBox _textEditor = new()
        {
            AcceptsReturn = true,
            TextWrapping = TextWrapping.Wrap,
            IsSpellCheckEnabled = false,
            IsHitTestVisible = true,
            MinWidth = 120,
            MinHeight = 48,
            Margin = new Thickness(4)
        };
        private readonly StackPanel _textEditorActions = new()
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right
        };
        private readonly Button _textCommitButton = new() { Content = "Commit" };
        private readonly Button _textCancelButton = new() { Content = "Discard" };
        private readonly IReadOnlyDictionary<SelectionHitTestKind, Rectangle> _handles =
            new Dictionary<SelectionHitTestKind, Rectangle>
            {
                [SelectionHitTestKind.LeftEdge] = CreateHandle(),
                [SelectionHitTestKind.TopEdge] = CreateHandle(),
                [SelectionHitTestKind.RightEdge] = CreateHandle(),
                [SelectionHitTestKind.BottomEdge] = CreateHandle(),
                [SelectionHitTestKind.TopLeftCorner] = CreateHandle(),
                [SelectionHitTestKind.TopRightCorner] = CreateHandle(),
                [SelectionHitTestKind.BottomLeftCorner] = CreateHandle(),
                [SelectionHitTestKind.BottomRightCorner] = CreateHandle()
            };
        private readonly FunctionBarSurface _functionBarHost;
        private FunctionBarSurface? _functionBar;
        private AppWindow? _appWindow;
        private nint _handle;
        private nint _previousWindowProc;
        private WindowProcDelegate? _windowProc;
        private bool _nativeInputBoundaryInstalled;
        private double _rasterizationScale = 1;
        private bool _updatingTextEditor;
        private TextDraftPresentation? _selectedTextEdit;
        private bool _disposed;

        public string DisplayId => _descriptor.DisplayId;

        public PhysicalRect PhysicalBounds => _descriptor.PhysicalBoundsInVirtualDesktop;

        public string CoordinateVersion => _descriptor.CoordinateVersion;

        public double RasterizationScale => _rasterizationScale;

        public OverlaySurface(
            FrozenDisplayOverlayDescriptor descriptor,
            SessionInputBoundary inputBoundary,
            ICompleteExecutionTraceSink? traceSink)
        {
            _descriptor = descriptor;
            _inputBoundary = inputBoundary
                ?? throw new ArgumentNullException(nameof(inputBoundary));
            _traceSink = traceSink;
            _image.Stretch = Stretch.Fill;
            _canvas.IsTabStop = true;
            _canvas.Background = new SolidColorBrush(ColorHelper.FromArgb(0, 0, 0, 0));
            _canvas.Cursor = InputSystemCursor.Create(InputSystemCursorShape.Cross);
            _canvas.PointerPressed += OnPointerPressed;
            _canvas.PointerMoved += OnPointerMoved;
            _canvas.PointerReleased += OnPointerReleased;
            _canvas.PointerCaptureLost += OnPointerCaptureLost;
            _canvas.KeyDown += OnKeyDown;
            _textEditor.TextChanged += OnTextEditorTextChanged;
            _textCommitButton.Click += OnTextCommitClicked;
            _textCancelButton.Click += OnTextCancelClicked;
            _textEditorHost.PointerPressed += OnTextEditorPointerPressed;
            Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(
                _textEditor,
                "Text annotation editor");
            Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(
                _textCommitButton,
                "Commit text annotation");
            Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(
                _textCancelButton,
                "Discard text annotation");
            _textEditorActions.Children.Add(_textCommitButton);
            _textEditorActions.Children.Add(_textCancelButton);
            _textEditorHost.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            _textEditorHost.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Grid.SetRow(_textEditor, 0);
            Grid.SetRow(_textEditorActions, 1);
            _textEditorHost.Children.Add(_textEditor);
            _textEditorHost.Children.Add(_textEditorActions);
            _root.Children.Add(_image);
            _root.Children.Add(_canvas);
            _canvas.Children.Add(_maskTop);
            _canvas.Children.Add(_maskLeft);
            _canvas.Children.Add(_maskRight);
            _canvas.Children.Add(_maskBottom);
            _canvas.Children.Add(_selectionBorder);
            _canvas.Children.Add(_objectSelectionBorder);
            foreach (var handle in _handles.Values)
            {
                _canvas.Children.Add(handle);
            }

            foreach (var handle in _objectHandles.Values)
            {
                _canvas.Children.Add(handle);
            }

            _canvas.Children.Add(_textEditorHost);
            _functionBarHost = new FunctionBarSurface(this);
            _canvas.Children.Add(_functionBarHost.Root);
        }

        public async ValueTask InitializeAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _window.Content = _root;
            _window.Activate();
            var handle = WindowNative.GetWindowHandle(_window);
            if (handle == 0)
            {
                throw new InvalidOperationException("The overlay window handle is unavailable.");
            }

            _handle = handle;
            _traceSink?.Record(new CompleteExecutionTraceEntry
            {
                TimestampUtc = DateTimeOffset.UtcNow,
                SessionId = _descriptor.SessionId,
                SelectionRevision = -1,
                WorkflowState = WorkflowState.Selecting,
                CompleteStage = CompleteExecutionStage.Diagnostic,
                Component = nameof(WindowsFrozenDisplayOverlayCoordinator),
                ManagedThreadId = Environment.CurrentManagedThreadId,
                DiagnosticEvent = "Overlay.Initialized",
                DiagnosticMessage = $"DisplayId={_descriptor.DisplayId};Handle=0x{handle.ToInt64():X}"
            });
            InstallNativeInputBoundary();
            _ = ShowWindow(handle, SwHide);

            _appWindow = AppWindow.GetFromWindowId(
                Win32Interop.GetWindowIdFromWindow(handle));
            var previousDpiContext = SetThreadDpiAwarenessContext(new nint(PerMonitorAwareV2));
            try
            {
                var presenter = OverlappedPresenter.Create();
                presenter.SetBorderAndTitleBar(false, false);
                presenter.IsAlwaysOnTop = true;
                presenter.IsMaximizable = false;
                presenter.IsMinimizable = false;
                presenter.IsResizable = false;
                _appWindow.SetPresenter(presenter);
                _appWindow.IsShownInSwitchers = false;
                _appWindow.Move(new PointInt32(
                    _descriptor.PhysicalBoundsInVirtualDesktop.Left,
                    _descriptor.PhysicalBoundsInVirtualDesktop.Top));
                _appWindow.Resize(new SizeInt32(
                    _descriptor.PixelSize.Width,
                    _descriptor.PixelSize.Height));
                _appWindow.Hide();
                ApplyToolWindowStyle(handle);
                _rasterizationScale = _root.XamlRoot?.RasterizationScale ?? 1;
                _canvas.Width = _descriptor.PixelSize.Width / _rasterizationScale;
                _canvas.Height = _descriptor.PixelSize.Height / _rasterizationScale;

                if (_descriptor.Frame.FrozenFrame.ImageResult
                    is not SoftwareBitmapImageResult imageResult)
                {
                    throw new InvalidOperationException(
                        "The frozen display frame is not a canonical SoftwareBitmap.");
                }

                await WinUiImagePresentationAdapter.PresentAsync(
                    _image,
                    imageResult,
                    cancellationToken).ConfigureAwait(true);
                ApplySelection(SelectionVisualState.Initial(
                    _descriptor.SessionId,
                    _descriptor.CoordinateVersion));
            }
            finally
            {
                _ = SetThreadDpiAwarenessContext(previousDpiContext);
            }
        }

        public FunctionBarSurface CreateFunctionBar(
            FunctionBarPresentationRequest request,
            Action<string>? setPhase = null)
        {
            ObjectDisposedException.ThrowIf(_disposed, nameof(OverlaySurface));
            setPhase?.Invoke("UsePrebuiltSurface");
            _functionBar?.Deactivate();
            _functionBar = _functionBarHost;
            _functionBarHost.Initialize(request, setPhase);
            return _functionBar;
        }

        public void RemoveFunctionBar(FunctionBarSurface functionBar)
        {
            if (ReferenceEquals(_functionBar, functionBar))
            {
                functionBar.Deactivate();
                _functionBar = null;
            }
        }

        public bool TryGetPhysicalWorkArea(out PhysicalRect workArea)
        {
            workArea = default;
            var bounds = _descriptor.PhysicalBoundsInVirtualDesktop;
            if (!bounds.IsPositive)
            {
                return false;
            }

            var center = new PointNative(
                bounds.Left + (int)Math.Min(int.MaxValue, bounds.Width64 / 2),
                bounds.Top + (int)Math.Min(int.MaxValue, bounds.Height64 / 2));
            var monitor = MonitorFromPoint(center, MonitorDefaultToNearest);
            if (monitor == 0)
            {
                return false;
            }

            var info = new MonitorInfoNative
            {
                Size = Marshal.SizeOf<MonitorInfoNative>()
            };
            if (!GetMonitorInfo(monitor, ref info))
            {
                return false;
            }

            workArea = new PhysicalRect(
                info.Work.Left,
                info.Work.Top,
                info.Work.Right,
                info.Work.Bottom);
            return workArea.IsPositive;
        }

        public static void ShowAll(IReadOnlyList<OverlaySurface> surfaces)
        {
            ArgumentNullException.ThrowIfNull(surfaces);
            if (surfaces.Count == 0)
            {
                return;
            }

            var deferredPosition = BeginDeferWindowPos(surfaces.Count);
            if (deferredPosition == 0)
            {
                throw new InvalidOperationException(
                    "The overlay windows could not be prepared for an atomic show.");
            }

            try
            {
                foreach (var surface in surfaces)
                {
                    ObjectDisposedException.ThrowIf(surface._disposed, nameof(OverlaySurface));
                    deferredPosition = DeferWindowPos(
                        deferredPosition,
                        surface._handle,
                        0,
                        0,
                        0,
                        0,
                        0,
                        SwpNoSize
                            | SwpNoMove
                            | SwpNoZOrder
                            | SwpNoActivate
                            | SwpShowWindow);
                    if (deferredPosition == 0)
                    {
                        throw new InvalidOperationException(
                            "The overlay windows could not be staged for an atomic show.");
                    }
                }

                if (!EndDeferWindowPos(deferredPosition))
                {
                    throw new InvalidOperationException(
                        "The overlay windows could not be shown as one batch.");
                }

                FocusSessionEscapeOwner(surfaces[0]);
                deferredPosition = 0;
            }
            finally
            {
                if (deferredPosition != 0)
                {
                    _ = EndDeferWindowPos(deferredPosition);
                }
            }
        }

        public void ApplySelection(SelectionVisualState state)
        {
            if (_disposed
                || state.SessionId != _descriptor.SessionId
                || !string.Equals(
                    state.CoordinateVersion,
                    _descriptor.CoordinateVersion,
                    StringComparison.Ordinal))
            {
                return;
            }

            var width = _descriptor.PixelSize.Width / _rasterizationScale;
            var height = _descriptor.PixelSize.Height / _rasterizationScale;
            var bounds = state.NormalizedPhysicalBounds;
            if (bounds is not PhysicalRect selection
                || !selection.Intersects(_descriptor.PhysicalBoundsInVirtualDesktop))
            {
                SetMask(_maskTop, 0, 0, width, height);
                SetMask(_maskLeft, 0, 0, 0, 0);
                SetMask(_maskRight, 0, 0, 0, 0);
                SetMask(_maskBottom, 0, 0, 0, 0);
                _selectionBorder.Visibility = Visibility.Collapsed;
                HideHandles();
            }
            else
            {
                var intersection = selection.Intersection(
                    _descriptor.PhysicalBoundsInVirtualDesktop);
                var left = (intersection.Left - _descriptor.PhysicalBoundsInVirtualDesktop.Left)
                    / _rasterizationScale;
                var top = (intersection.Top - _descriptor.PhysicalBoundsInVirtualDesktop.Top)
                    / _rasterizationScale;
                var right = (intersection.Right - _descriptor.PhysicalBoundsInVirtualDesktop.Left)
                    / _rasterizationScale;
                var bottom = (intersection.Bottom - _descriptor.PhysicalBoundsInVirtualDesktop.Top)
                    / _rasterizationScale;
                SetMask(_maskTop, 0, 0, width, top);
                SetMask(_maskLeft, 0, top, left, bottom - top);
                SetMask(_maskRight, right, top, width - right, bottom - top);
                SetMask(_maskBottom, 0, bottom, width, height - bottom);
                SetCanvasRectangle(_selectionBorder, left, top, right - left, bottom - top);
                _selectionBorder.Visibility = intersection.IsPositive
                    ? Visibility.Visible
                    : Visibility.Collapsed;
                ApplyHandles(selection, width, height);
            }

            ApplyCursor(state);

        }

        public void ApplyAnnotation(AnnotationPresentationSnapshot snapshot)
        {
            if (_disposed
                || snapshot.SessionId != _descriptor.SessionId
                || !string.Equals(
                    snapshot.CoordinateVersion,
                    _descriptor.CoordinateVersion,
                    StringComparison.Ordinal))
            {
                return;
            }

            ClearAnnotationPreviews();
            var selection = snapshot.SelectionPhysicalBounds;
            var renderObjects = snapshot.Document.Objects.ToList();
            if (snapshot.SelectedObject?.OriginalObject is AnnotationObject original
                && snapshot.SelectedObject.PreviewObject is AnnotationObject preview
                && preview.ObjectId == original.ObjectId)
            {
                var index = renderObjects.FindIndex(value => value.ObjectId == original.ObjectId);
                if (index >= 0)
                {
                    renderObjects[index] = preview;
                }
            }

            foreach (var annotationObject in renderObjects)
            {
                if (annotationObject.ToolKind == AnnotationToolKind.PrivacyRegion
                    && annotationObject.Content is PrivacyRegionAnnotationContent privacyContent)
                {
                    AddPrivacyPreview(annotationObject.Geometry, privacyContent, selection);
                }
            }

            foreach (var annotationObject in renderObjects)
            {
                if (annotationObject.ToolKind == AnnotationToolKind.Rectangle
                    && annotationObject.Content is RectangleAnnotationContent content)
                {
                    AddAnnotationPreview(annotationObject.Geometry, content.Style, selection);
                }
                else if (annotationObject.ToolKind == AnnotationToolKind.ArrowLine
                    && annotationObject.Content is ArrowLineAnnotationContent arrowLineContent)
                {
                    AddArrowLinePreview(arrowLineContent.Segment, arrowLineContent.Style, selection);
                }
                else if (annotationObject.ToolKind == AnnotationToolKind.HighlighterStroke
                    && annotationObject.Content is HighlighterStrokeContent highlighterContent)
                {
                    AddHighlighterPreview(
                        highlighterContent.Path.Points,
                        highlighterContent.Style,
                        selection);
                }
                else if (annotationObject.ToolKind == AnnotationToolKind.Text
                    && annotationObject.Content is TextAnnotationContent textContent)
                {
                    AddTextPreview(textContent, selection);
                }
                else if (annotationObject.ToolKind == AnnotationToolKind.NumberedMarker
                    && annotationObject.Content is NumberedMarkerAnnotationContent markerContent)
                {
                    AddNumberedMarkerPreview(
                        annotationObject.Geometry,
                        markerContent,
                        selection,
                        isDraft: false);
                }
            }

            if (snapshot.DraftPhysicalBounds is PhysicalRect draft
                && draft.IsPositive)
            {
                AddAnnotationPreview(
                    draft,
                    RectangleAnnotationStyle.Default,
                    selection);
            }

            if (snapshot.DraftArrowLineSegment is PhysicalLineSegment draftArrowLine)
            {
                AddArrowLinePreview(
                    draftArrowLine,
                    ArrowLineAnnotationStyle.Default with
                    {
                        EndStyle = snapshot.ActiveArrowLineEndStyle
                    },
                    selection);
            }

            if (snapshot.DraftHighlighterPoints is IReadOnlyList<PhysicalPoint> draftHighlighterPoints)
            {
                AddHighlighterPreview(
                    draftHighlighterPoints,
                    snapshot.ActiveHighlighterStyle,
                    selection);
            }

            if (snapshot.DraftPrivacyRegionBounds is PhysicalRect draftPrivacyRegion
                && draftPrivacyRegion.IsPositive
                && snapshot.DraftPrivacyRegionMode is PrivacyRegionMode draftPrivacyMode
                && snapshot.DraftPrivacyRegionEffectParameters is PrivacyRegionEffectParameters draftPrivacyParameters)
            {
                AddPrivacyPreview(
                    draftPrivacyRegion,
                    new PrivacyRegionAnnotationContent(draftPrivacyMode, draftPrivacyParameters),
                    selection);
            }

            if (snapshot.SelectedTextEdit is TextDraftPresentation selectedTextEdit)
            {
                _selectedTextEdit = selectedTextEdit;
                ShowTextEditor(selectedTextEdit, selection, existingEdit: true);
            }
            else if (snapshot.DraftText is TextDraftPresentation draftText)
            {
                _selectedTextEdit = null;
                ShowTextEditor(draftText, selection, existingEdit: false);
            }
            else
            {
                _selectedTextEdit = null;
                HideTextEditor();
            }

            if (snapshot.DraftNumberedMarker is NumberedMarkerDraftPresentation draftMarker)
            {
                AddNumberedMarkerPreview(
                    draftMarker.Bounds,
                    new NumberedMarkerAnnotationContent(draftMarker.Number, draftMarker.Style),
                    selection,
                    isDraft: true);
            }

            ApplyObjectAdornment(snapshot);

            _canvas.Cursor = snapshot.ActiveTool is EditingToolKind.Rectangle
                or EditingToolKind.ArrowLine
                or EditingToolKind.Highlighter
                or EditingToolKind.PrivacyRegion
                or EditingToolKind.NumberedMarker
                ? InputSystemCursor.Create(InputSystemCursorShape.Cross)
                : InputSystemCursor.Create(InputSystemCursorShape.Arrow);
        }

        private void ApplyObjectAdornment(AnnotationPresentationSnapshot snapshot)
        {
            HideObjectHandles();
            var state = snapshot.SelectedObject;
            var selected = state?.PreviewObject ?? state?.OriginalObject;
            if (selected is null
                || snapshot.SelectionPhysicalBounds is not PhysicalRect selection
                || !selected.Geometry.Intersects(selection)
                || !selected.Geometry.Intersects(_descriptor.PhysicalBoundsInVirtualDesktop))
            {
                _objectSelectionBorder.Visibility = Visibility.Collapsed;
                return;
            }

            var visible = selected.Geometry
                .Intersection(selection)
                .Intersection(_descriptor.PhysicalBoundsInVirtualDesktop);
            if (!visible.IsPositive)
            {
                _objectSelectionBorder.Visibility = Visibility.Collapsed;
                return;
            }

            SetCanvasRectangle(
                _objectSelectionBorder,
                (visible.Left - _descriptor.PhysicalBoundsInVirtualDesktop.Left) / _rasterizationScale,
                (visible.Top - _descriptor.PhysicalBoundsInVirtualDesktop.Top) / _rasterizationScale,
                visible.Width / _rasterizationScale,
                visible.Height / _rasterizationScale);
            _objectSelectionBorder.Visibility = Visibility.Visible;

            var handles = selected.ToolKind == AnnotationToolKind.ArrowLine
                && selected.Content is ArrowLineAnnotationContent arrow
                ? new Dictionary<AnnotationObjectEditHandleKind, PhysicalPoint>
                {
                    [AnnotationObjectEditHandleKind.StartEndpoint] = arrow.Segment.Start,
                    [AnnotationObjectEditHandleKind.EndEndpoint] = arrow.Segment.End
                }
                    : selected.ToolKind is AnnotationToolKind.NumberedMarker
                        or AnnotationToolKind.Text
                    ? new Dictionary<AnnotationObjectEditHandleKind, PhysicalPoint>()
                    : GetRectangleHandles(selected.Geometry);
            foreach (var pair in handles)
            {
                if (!_objectHandles.TryGetValue(pair.Key, out var handle)
                    || !IsPointOnDisplay(pair.Value))
                {
                    continue;
                }

                var size = HandleVisualSizePixels / _rasterizationScale;
                var left = (pair.Value.X - _descriptor.PhysicalBoundsInVirtualDesktop.Left)
                    / _rasterizationScale - size / 2;
                var top = (pair.Value.Y - _descriptor.PhysicalBoundsInVirtualDesktop.Top)
                    / _rasterizationScale - size / 2;
                SetCanvasRectangle(handle, left, top, size, size);
                handle.Visibility = Visibility.Visible;
            }
        }

        private static Dictionary<AnnotationObjectEditHandleKind, PhysicalPoint> GetRectangleHandles(
            PhysicalRect bounds)
        {
            var middleX = checked((int)(((long)bounds.Left + bounds.Right) / 2));
            var middleY = checked((int)(((long)bounds.Top + bounds.Bottom) / 2));
            return new()
            {
                [AnnotationObjectEditHandleKind.LeftEdge] = new(bounds.Left, middleY),
                [AnnotationObjectEditHandleKind.TopEdge] = new(middleX, bounds.Top),
                [AnnotationObjectEditHandleKind.RightEdge] = new(bounds.Right, middleY),
                [AnnotationObjectEditHandleKind.BottomEdge] = new(middleX, bounds.Bottom),
                [AnnotationObjectEditHandleKind.TopLeftCorner] = new(bounds.Left, bounds.Top),
                [AnnotationObjectEditHandleKind.TopRightCorner] = new(bounds.Right, bounds.Top),
                [AnnotationObjectEditHandleKind.BottomLeftCorner] = new(bounds.Left, bounds.Bottom),
                [AnnotationObjectEditHandleKind.BottomRightCorner] = new(bounds.Right, bounds.Bottom)
            };
        }

        private void HideObjectHandles()
        {
            _objectSelectionBorder.Visibility = Visibility.Collapsed;
            foreach (var handle in _objectHandles.Values)
            {
                handle.Visibility = Visibility.Collapsed;
            }
        }

        private void AddNumberedMarkerPreview(
            PhysicalRect bounds,
            NumberedMarkerAnnotationContent content,
            PhysicalRect? selection,
            bool isDraft)
        {
            var visible = bounds.Intersection(_descriptor.PhysicalBoundsInVirtualDesktop);
            if (selection is PhysicalRect selectionBounds)
            {
                visible = visible.Intersection(selectionBounds);
            }

            if (!visible.IsPositive || !bounds.IsPositive)
            {
                return;
            }

            var marker = new Grid
            {
                Width = bounds.Width / _rasterizationScale,
                Height = bounds.Height / _rasterizationScale,
                IsHitTestVisible = false,
                Visibility = Visibility.Visible,
                Clip = new RectangleGeometry
                {
                    Rect = new global::Windows.Foundation.Rect(
                        (visible.Left - bounds.Left) / _rasterizationScale,
                        (visible.Top - bounds.Top) / _rasterizationScale,
                        visible.Width / _rasterizationScale,
                        visible.Height / _rasterizationScale)
                }
            };
            var color = content.Style.Color;
            marker.Children.Add(new Ellipse
            {
                Fill = new SolidColorBrush(ColorHelper.FromArgb(
                    color.A,
                    color.R,
                    color.G,
                    color.B)),
                Stroke = new SolidColorBrush(ColorHelper.FromArgb(255, 255, 255, 255)),
                StrokeThickness = Math.Max(1, content.Style.Size / 16d) / _rasterizationScale,
                IsHitTestVisible = false
            });
            marker.Children.Add(new TextBlock
            {
                Text = content.Number.ToString(System.Globalization.CultureInfo.InvariantCulture),
                FontSize = Math.Max(10, content.Style.Size * 0.45) / _rasterizationScale,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(ColorHelper.FromArgb(255, 255, 255, 255)),
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                IsHitTestVisible = false
            });
            if (isDraft)
            {
                marker.Children.Add(new Border
                {
                    BorderBrush = new SolidColorBrush(ColorHelper.FromArgb(255, 255, 255, 255)),
                    BorderThickness = new Thickness(2 / _rasterizationScale),
                    CornerRadius = new CornerRadius(bounds.Width / _rasterizationScale / 2),
                    IsHitTestVisible = false
                });
            }

            SetCanvasRectangle(
                marker,
                (bounds.Left - _descriptor.PhysicalBoundsInVirtualDesktop.Left)
                    / _rasterizationScale,
                (bounds.Top - _descriptor.PhysicalBoundsInVirtualDesktop.Top)
                    / _rasterizationScale,
                bounds.Width / _rasterizationScale,
                bounds.Height / _rasterizationScale);
            _numberedMarkerPreviews.Add(marker);
            _canvas.Children.Add(marker);
        }

        private void AddTextPreview(
            TextAnnotationContent content,
            PhysicalRect? selection)
        {
            var visible = content.BoundsInVirtualDesktop
                .Intersection(_descriptor.PhysicalBoundsInVirtualDesktop);
            if (selection is PhysicalRect selectionBounds)
            {
                visible = visible.Intersection(selectionBounds);
            }

            if (!visible.IsPositive)
            {
                return;
            }

            var text = new TextBlock
            {
                Text = content.Text,
                FontFamily = new FontFamily(content.Style.FontFamily),
                FontSize = content.Style.FontSize,
                FontWeight = content.Style.Bold ? FontWeights.Bold : FontWeights.Normal,
                Foreground = new SolidColorBrush(ColorHelper.FromArgb(
                    content.Style.Color.A,
                    content.Style.Color.R,
                    content.Style.Color.G,
                    content.Style.Color.B)),
                TextWrapping = TextWrapping.Wrap,
                IsHitTestVisible = false,
                Visibility = Visibility.Visible
            };
            SetCanvasRectangle(
                text,
                (visible.Left - _descriptor.PhysicalBoundsInVirtualDesktop.Left)
                    / _rasterizationScale,
                (visible.Top - _descriptor.PhysicalBoundsInVirtualDesktop.Top)
                    / _rasterizationScale,
                visible.Width / _rasterizationScale,
                visible.Height / _rasterizationScale);
            _textPreviews.Add(text);
            _canvas.Children.Add(text);
        }

        private void AddPrivacyPreview(
            PhysicalRect geometry,
            PrivacyRegionAnnotationContent content,
            PhysicalRect? selection)
        {
            var visible = geometry.Intersection(_descriptor.PhysicalBoundsInVirtualDesktop);
            if (selection is PhysicalRect selectionBounds)
            {
                visible = visible.Intersection(selectionBounds);
            }

            if (!visible.IsPositive
                || _descriptor.Frame.FrozenFrame.ImageResult is not SoftwareBitmapImageResult source)
            {
                return;
            }

            SoftwareBitmapImageResult preview;
            try
            {
                preview = FrozenPrivacyEffectRenderer.Render(
                    source,
                    _descriptor.PhysicalBoundsInVirtualDesktop,
                    visible,
                    content);
            }
            catch
            {
                return;
            }

            var image = new Image
            {
                Stretch = Stretch.Fill,
                IsHitTestVisible = false,
                Visibility = Visibility.Visible
            };
            SetCanvasRectangle(
                image,
                (visible.Left - _descriptor.PhysicalBoundsInVirtualDesktop.Left)
                    / _rasterizationScale,
                (visible.Top - _descriptor.PhysicalBoundsInVirtualDesktop.Top)
                    / _rasterizationScale,
                visible.Width / _rasterizationScale,
                visible.Height / _rasterizationScale);
            _privacyPreviews.Add(new PrivacyPreview(image, preview));
            _canvas.Children.Add(image);
            _ = PresentPrivacyPreviewAsync(image, preview);
        }

        private static async Task PresentPrivacyPreviewAsync(
            Image image,
            SoftwareBitmapImageResult preview)
        {
            try
            {
                await WinUiImagePresentationAdapter
                    .PresentAsync(image, preview, CancellationToken.None)
                    .ConfigureAwait(true);
            }
            catch
            {
                image.Source = null;
            }
        }

        private void ShowTextEditor(
            TextDraftPresentation draft,
            PhysicalRect? selection,
            bool existingEdit)
        {
            var displayBounds = _descriptor.PhysicalBoundsInVirtualDesktop;
            if (!Contains(displayBounds, draft.AnchorInVirtualDesktop))
            {
                if (existingEdit)
                {
                    _selectedTextEdit = null;
                }
                HideTextEditor();
                return;
            }

            var visible = draft.BoundsInVirtualDesktop.Intersection(displayBounds);
            if (selection is PhysicalRect selectionBounds)
            {
                visible = visible.Intersection(selectionBounds);
            }

            if (!visible.IsPositive)
            {
                if (existingEdit)
                {
                    _selectedTextEdit = null;
                }
                HideTextEditor();
                return;
            }

            _updatingTextEditor = true;
            try
            {
                if (!string.Equals(_textEditor.Text, draft.Text, StringComparison.Ordinal))
                {
                    _textEditor.Text = draft.Text;
                }

                _textEditor.FontFamily = new FontFamily(draft.Style.FontFamily);
                _textEditor.FontSize = draft.Style.FontSize;
                _textEditor.FontWeight = draft.Style.Bold ? FontWeights.Bold : FontWeights.Normal;
                _textEditor.Foreground = new SolidColorBrush(ColorHelper.FromArgb(
                    draft.Style.Color.A,
                    draft.Style.Color.R,
                    draft.Style.Color.G,
                    draft.Style.Color.B));
                SetCanvasRectangle(
                    _textEditorHost,
                    (visible.Left - displayBounds.Left) / _rasterizationScale,
                    (visible.Top - displayBounds.Top) / _rasterizationScale,
                    visible.Width / _rasterizationScale,
                    visible.Height / _rasterizationScale);
                _textEditorHost.IsHitTestVisible = true;
                _textEditorHost.Visibility = Visibility.Visible;
            }
            finally
            {
                _updatingTextEditor = false;
            }

            _ = _textEditor.Focus(FocusState.Programmatic);
        }

        private void HideTextEditor()
        {
            _textEditorHost.Visibility = Visibility.Collapsed;
            _textEditorHost.IsHitTestVisible = false;
        }

        private void OnTextEditorTextChanged(object sender, TextChangedEventArgs args)
        {
            if (_disposed || _updatingTextEditor)
            {
                return;
            }

            if (_selectedTextEdit is TextDraftPresentation selectedTextEdit
                && _inputBoundary.EditingRouter is IEditingInputRouter editingRouter
                && editingRouter.SelectedObject.SelectedObjectId is AnnotationObjectId objectId
                && editingRouter.SelectedObject.TextEditDraftId is Guid draftId)
            {
                editingRouter.UpdateTextEdit(new AnnotationObjectTextEditRequest(
                    _descriptor.SessionId,
                    _descriptor.CoordinateVersion,
                    editingRouter.CurrentSelectionRevision,
                    editingRouter.CurrentAnnotationRevision,
                    objectId,
                    draftId,
                    _textEditor.Text));
            }
            else
            {
                _inputBoundary.UpdateTextDraftContent(_textEditor.Text);
            }
        }

        private void OnTextCommitClicked(object sender, RoutedEventArgs args)
        {
            if (!_disposed)
            {
                if (_selectedTextEdit is not null
                    && _inputBoundary.EditingRouter is IEditingInputRouter editingRouter
                    && editingRouter.SelectedObject.SelectedObjectId is AnnotationObjectId objectId
                    && editingRouter.SelectedObject.TextEditDraftId is Guid draftId)
                {
                    editingRouter.CommitTextEdit(new AnnotationObjectTextEditRequest(
                        _descriptor.SessionId,
                        _descriptor.CoordinateVersion,
                        editingRouter.CurrentSelectionRevision,
                        editingRouter.CurrentAnnotationRevision,
                        objectId,
                        draftId,
                        _textEditor.Text));
                }
                else
                {
                    _inputBoundary.CommitTextDraft();
                }
            }
        }

        private void OnTextCancelClicked(object sender, RoutedEventArgs args)
        {
            if (!_disposed)
            {
                if (_selectedTextEdit is not null)
                {
                    _inputBoundary.EditingRouter?.CancelEdit(
                        _descriptor.SessionId,
                        _descriptor.CoordinateVersion);
                }
                else
                {
                    _inputBoundary.CancelTextDraft();
                }
            }
        }

        private static void OnTextEditorPointerPressed(
            object sender,
            PointerRoutedEventArgs args) => args.Handled = true;

        private void AddAnnotationPreview(
            PhysicalRect geometry,
            RectangleAnnotationStyle style,
            PhysicalRect? selection)
        {
            var visible = geometry.Intersection(_descriptor.PhysicalBoundsInVirtualDesktop);
            if (selection is PhysicalRect selectionBounds)
            {
                visible = visible.Intersection(selectionBounds);
            }

            if (!visible.IsPositive)
            {
                return;
            }

            var rectangle = new Rectangle
            {
                Fill = new SolidColorBrush(ColorHelper.FromArgb(0, 0, 0, 0)),
                Stroke = new SolidColorBrush(ColorHelper.FromArgb(
                    style.StrokeColor.A,
                    style.StrokeColor.R,
                    style.StrokeColor.G,
                    style.StrokeColor.B)),
                StrokeThickness = style.StrokeThickness / _rasterizationScale,
                IsHitTestVisible = false,
                Visibility = Visibility.Visible
            };
            var left = (visible.Left - _descriptor.PhysicalBoundsInVirtualDesktop.Left)
                / _rasterizationScale;
            var top = (visible.Top - _descriptor.PhysicalBoundsInVirtualDesktop.Top)
                / _rasterizationScale;
            SetCanvasRectangle(
                rectangle,
                left,
                top,
                visible.Width / _rasterizationScale,
                visible.Height / _rasterizationScale);
            _annotationPreviews.Add(rectangle);
            _canvas.Children.Add(rectangle);
        }

        private void AddArrowLinePreview(
            PhysicalLineSegment geometry,
            ArrowLineAnnotationStyle style,
            PhysicalRect? selection)
        {
            var visibleBounds = _descriptor.PhysicalBoundsInVirtualDesktop;
            if (selection is PhysicalRect selectionBounds)
            {
                visibleBounds = visibleBounds.Intersection(selectionBounds);
            }

            if (!visibleBounds.IsPositive
                || !TryClipLine(geometry, visibleBounds, out var start, out var end))
            {
                return;
            }

            AddLinePreview(start, end, style);
            if (style.EndStyle == ArrowLineEndStyle.Arrow
                && Contains(visibleBounds, geometry.End))
            {
                AddArrowHeadPreview(geometry, style, visibleBounds);
            }
        }

        private void AddHighlighterPreview(
            IReadOnlyList<PhysicalPoint> points,
            HighlighterAnnotationStyle style,
            PhysicalRect? selection)
        {
            if (points.Count < 2)
            {
                return;
            }

            var visibleBounds = _descriptor.PhysicalBoundsInVirtualDesktop;
            if (selection is PhysicalRect selectionBounds)
            {
                visibleBounds = visibleBounds.Intersection(selectionBounds);
            }

            if (!visibleBounds.IsPositive)
            {
                return;
            }

            for (var index = 1; index < points.Count; index++)
            {
                var segment = new PhysicalLineSegment(points[index - 1], points[index]);
                if (!TryClipLine(segment, visibleBounds, out var start, out var end))
                {
                    continue;
                }

                AddHighlighterLine(start, end, style);
                AddHighlighterCap(start, style);
                AddHighlighterCap(end, style);
            }
        }

        private void AddHighlighterLine(
            PhysicalPoint start,
            PhysicalPoint end,
            HighlighterAnnotationStyle style)
        {
            var line = new Line
            {
                X1 = (start.X - _descriptor.PhysicalBoundsInVirtualDesktop.Left)
                    / _rasterizationScale,
                Y1 = (start.Y - _descriptor.PhysicalBoundsInVirtualDesktop.Top)
                    / _rasterizationScale,
                X2 = (end.X - _descriptor.PhysicalBoundsInVirtualDesktop.Left)
                    / _rasterizationScale,
                Y2 = (end.Y - _descriptor.PhysicalBoundsInVirtualDesktop.Top)
                    / _rasterizationScale,
                Stroke = new SolidColorBrush(ColorHelper.FromArgb(
                    style.StrokeColor.A,
                    style.StrokeColor.R,
                    style.StrokeColor.G,
                    style.StrokeColor.B)),
                StrokeThickness = style.StrokeThickness / _rasterizationScale,
                IsHitTestVisible = false,
                Visibility = Visibility.Visible
            };
            _highlighterPreviews.Add(line);
            _canvas.Children.Add(line);
        }

        private void AddHighlighterCap(
            PhysicalPoint point,
            HighlighterAnnotationStyle style)
        {
            var diameter = style.StrokeThickness / _rasterizationScale;
            var cap = new Ellipse
            {
                Width = diameter,
                Height = diameter,
                Fill = new SolidColorBrush(ColorHelper.FromArgb(
                    style.StrokeColor.A,
                    style.StrokeColor.R,
                    style.StrokeColor.G,
                    style.StrokeColor.B)),
                IsHitTestVisible = false,
                Visibility = Visibility.Visible
            };
            var left = (point.X - _descriptor.PhysicalBoundsInVirtualDesktop.Left)
                / _rasterizationScale - diameter / 2;
            var top = (point.Y - _descriptor.PhysicalBoundsInVirtualDesktop.Top)
                / _rasterizationScale - diameter / 2;
            Canvas.SetLeft(cap, left);
            Canvas.SetTop(cap, top);
            _highlighterPreviews.Add(cap);
            _canvas.Children.Add(cap);
        }

        private void AddLinePreview(
            PhysicalPoint start,
            PhysicalPoint end,
            ArrowLineAnnotationStyle style)
        {
            var line = new Line
            {
                X1 = (start.X - _descriptor.PhysicalBoundsInVirtualDesktop.Left)
                    / _rasterizationScale,
                Y1 = (start.Y - _descriptor.PhysicalBoundsInVirtualDesktop.Top)
                    / _rasterizationScale,
                X2 = (end.X - _descriptor.PhysicalBoundsInVirtualDesktop.Left)
                    / _rasterizationScale,
                Y2 = (end.Y - _descriptor.PhysicalBoundsInVirtualDesktop.Top)
                    / _rasterizationScale,
                Stroke = new SolidColorBrush(ColorHelper.FromArgb(
                    style.StrokeColor.A,
                    style.StrokeColor.R,
                    style.StrokeColor.G,
                    style.StrokeColor.B)),
                StrokeThickness = style.StrokeThickness / _rasterizationScale,
                IsHitTestVisible = false,
                Visibility = Visibility.Visible
            };
            _arrowLinePreviews.Add(line);
            _canvas.Children.Add(line);
        }

        private void AddArrowHeadPreview(
            PhysicalLineSegment geometry,
            ArrowLineAnnotationStyle style,
            PhysicalRect visibleBounds)
        {
            var dx = geometry.End.X - geometry.Start.X;
            var dy = geometry.End.Y - geometry.Start.Y;
            var length = Math.Sqrt((double)(dx * dx) + (double)(dy * dy));
            if (length <= 0)
            {
                return;
            }

            var size = Math.Max(6, style.StrokeThickness * 4);
            var ux = dx / length;
            var uy = dy / length;
            var baseX = geometry.End.X - ux * size;
            var baseY = geometry.End.Y - uy * size;
            var sin = 0.5;
            var left = new PhysicalPoint(
                (int)Math.Round(baseX * 1 + (uy * size * sin), MidpointRounding.AwayFromZero),
                (int)Math.Round(baseY * 1 - (ux * size * sin), MidpointRounding.AwayFromZero));
            var right = new PhysicalPoint(
                (int)Math.Round(baseX * 1 - (uy * size * sin), MidpointRounding.AwayFromZero),
                (int)Math.Round(baseY * 1 + (ux * size * sin), MidpointRounding.AwayFromZero));
            if (Contains(visibleBounds, left))
            {
                AddLinePreview(geometry.End, left, style);
            }

            if (Contains(visibleBounds, right))
            {
                AddLinePreview(geometry.End, right, style);
            }
        }

        private void ClearAnnotationPreviews()
        {
            foreach (var preview in _annotationPreviews)
            {
                _canvas.Children.Remove(preview);
            }

            _annotationPreviews.Clear();
            foreach (var preview in _arrowLinePreviews)
            {
                _canvas.Children.Remove(preview);
            }

            _arrowLinePreviews.Clear();
            foreach (var preview in _highlighterPreviews)
            {
                _canvas.Children.Remove(preview);
            }

            _highlighterPreviews.Clear();
            foreach (var preview in _textPreviews)
            {
                _canvas.Children.Remove(preview);
            }

            _textPreviews.Clear();
            foreach (var preview in _numberedMarkerPreviews)
            {
                _canvas.Children.Remove(preview);
            }

            _numberedMarkerPreviews.Clear();
            foreach (var preview in _privacyPreviews)
            {
                preview.Image.Source = null;
                _canvas.Children.Remove(preview.Image);
                preview.ImageResult.Dispose();
            }

            _privacyPreviews.Clear();
        }

        private static bool Contains(PhysicalRect bounds, PhysicalPoint point) =>
            point.X >= bounds.Left
            && point.X <= bounds.Right
            && point.Y >= bounds.Top
            && point.Y <= bounds.Bottom;

        private sealed record PrivacyPreview(
            Image Image,
            SoftwareBitmapImageResult ImageResult);

        private static bool TryClipLine(
            PhysicalLineSegment segment,
            PhysicalRect bounds,
            out PhysicalPoint start,
            out PhysicalPoint end)
        {
            start = default;
            end = default;
            if (!segment.IsPositive || !bounds.IsPositive)
            {
                return false;
            }

            var x0 = (double)segment.Start.X;
            var y0 = (double)segment.Start.Y;
            var dx = segment.End.X - x0;
            var dy = segment.End.Y - y0;
            var t0 = 0d;
            var t1 = 1d;
            if (!Clip(-dx, x0 - bounds.Left, ref t0, ref t1)
                || !Clip(dx, bounds.Right - x0, ref t0, ref t1)
                || !Clip(-dy, y0 - bounds.Top, ref t0, ref t1)
                || !Clip(dy, bounds.Bottom - y0, ref t0, ref t1))
            {
                return false;
            }

            start = new PhysicalPoint(
                (int)Math.Round(x0 + t0 * dx, MidpointRounding.AwayFromZero),
                (int)Math.Round(y0 + t0 * dy, MidpointRounding.AwayFromZero));
            end = new PhysicalPoint(
                (int)Math.Round(x0 + t1 * dx, MidpointRounding.AwayFromZero),
                (int)Math.Round(y0 + t1 * dy, MidpointRounding.AwayFromZero));
            return start != end;
        }

        private static bool Clip(double p, double q, ref double t0, ref double t1)
        {
            if (p == 0)
            {
                return q >= 0;
            }

            var ratio = q / p;
            if (p < 0)
            {
                if (ratio > t1)
                {
                    return false;
                }

                if (ratio > t0)
                {
                    t0 = ratio;
                }
            }
            else
            {
                if (ratio < t0)
                {
                    return false;
                }

                if (ratio < t1)
                {
                    t1 = ratio;
                }
            }

            return true;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _functionBar?.Deactivate();
            _disposed = true;
            _functionBar = null;
            _canvas.PointerPressed -= OnPointerPressed;
            _canvas.PointerMoved -= OnPointerMoved;
            _canvas.PointerReleased -= OnPointerReleased;
            _canvas.PointerCaptureLost -= OnPointerCaptureLost;
            _canvas.KeyDown -= OnKeyDown;
            _canvas.Cursor = null;
            ClearAnnotationPreviews();
            HideTextEditor();
            _textEditor.TextChanged -= OnTextEditorTextChanged;
            _textCommitButton.Click -= OnTextCommitClicked;
            _textCancelButton.Click -= OnTextCancelClicked;
            _textEditorHost.PointerPressed -= OnTextEditorPointerPressed;
            HideHandles();
            HideObjectHandles();
            RemoveNativeInputBoundary();
            try
            {
                _appWindow?.Hide();
            }
            catch
            {
            }

            try
            {
                _window.Close();
            }
            catch
            {
            }

            try
            {
                _image.Source = null;
                _textEditorHost.Children.Clear();
                _window.Content = null;
            }
            catch
            {
            }

            try
            {
                _canvas.Children.Remove(_functionBarHost.Root);
                _functionBarHost.Dispose();
            }
            catch
            {
            }
        }

        private void OnPointerPressed(object sender, PointerRoutedEventArgs args)
        {
            if (_disposed || !TryGetGlobalPointer(out var point))
            {
                return;
            }

            _pointerReleaseDispatchGate.Reset();
            _canvas.Focus(FocusState.Pointer);
            var handle = WindowNative.GetWindowHandle(_window);
            var pointer = new SelectionPointerEvent(
                _descriptor.SessionId,
                _descriptor.CoordinateVersion,
                checked((int)args.Pointer.PointerId),
                point);
            var capturesPointer = _inputBoundary.UsesTextTool
                ? _inputBoundary.PointerPressedText(pointer).Kind
                    == TextDraftResultKind.DraftStarted
                : _inputBoundary.UsesHighlighterTool
                ? _inputBoundary.PointerPressedHighlighter(pointer).Kind
                    == HighlighterPointerResultKind.DraftStarted
                : _inputBoundary.UsesPrivacyRegionTool
                ? _inputBoundary.PointerPressedPrivacyRegion(pointer).Kind
                    == PrivacyRegionPointerResultKind.DraftStarted
                : _inputBoundary.UsesNumberedMarkerTool
                ? _inputBoundary.PointerPressedNumberedMarker(pointer).Kind
                    == NumberedMarkerPointerResultKind.DraftStarted
                : _inputBoundary.UsesArrowLineTool
                ? _inputBoundary.PointerPressedArrowLine(pointer).Kind
                    == ArrowLinePointerResultKind.DraftStarted
                : _inputBoundary.UsesRectangleTool
                ? _inputBoundary.PointerPressedRectangle(pointer).Kind
                    == RectanglePointerResultKind.DraftStarted
                : _inputBoundary.UsesObjectEditing
                ? _inputBoundary.PointerPressedObject(pointer).Kind
                    is AnnotationObjectEditResultKind.EditStarted
                    or AnnotationObjectEditResultKind.Selected
                : _inputBoundary.PointerPressed(pointer).Kind is SelectionInputResultKind.Dragging
                    or SelectionInputResultKind.Moving
                    or SelectionInputResultKind.Resizing
                    or SelectionInputResultKind.Reselecting;
            if (capturesPointer)
            {
                _ = SetCapture(handle);
            }
            args.Handled = true;
        }

        private void OnPointerMoved(object sender, PointerRoutedEventArgs args)
        {
            if (_disposed || !TryGetGlobalPointer(out var point))
            {
                return;
            }

            var pointer = new SelectionPointerEvent(
                _descriptor.SessionId,
                _descriptor.CoordinateVersion,
                checked((int)args.Pointer.PointerId),
                point);
            if (_inputBoundary.UsesTextTool)
            {
                args.Handled = true;
                return;
            }

            if (_inputBoundary.UsesHighlighterTool)
            {
                _inputBoundary.PointerMovedHighlighter(pointer);
            }
            else if (_inputBoundary.UsesPrivacyRegionTool)
            {
                _inputBoundary.PointerMovedPrivacyRegion(pointer);
            }
            else if (_inputBoundary.UsesNumberedMarkerTool)
            {
                _inputBoundary.PointerMovedNumberedMarker(pointer);
            }
            else if (_inputBoundary.UsesArrowLineTool)
            {
                _inputBoundary.PointerMovedArrowLine(pointer);
            }
            else if (_inputBoundary.UsesRectangleTool)
            {
                _inputBoundary.PointerMovedRectangle(pointer);
            }
            else if (_inputBoundary.UsesObjectEditing)
            {
                _inputBoundary.PointerMovedObject(pointer);
            }
            else
            {
                _inputBoundary.PointerMoved(pointer);
            }
            args.Handled = true;
        }

        private void OnPointerReleased(object sender, PointerRoutedEventArgs args)
        {
            var pointAvailable = TryGetGlobalPointer(out var point);
            TracePointerDiagnostic(
                "Overlay.PointerReleased.XamlReceived",
                $"PointAvailable={pointAvailable};PointerId={args.Pointer.PointerId}");
            if (_disposed || !pointAvailable)
            {
                return;
            }

            var pointer = new SelectionPointerEvent(
                _descriptor.SessionId,
                _descriptor.CoordinateVersion,
                checked((int)args.Pointer.PointerId),
                point);
            if (_inputBoundary.UsesTextTool)
            {
                args.Handled = true;
                return;
            }

            if (!_pointerReleaseDispatchGate.TryClaim())
            {
                TracePointerDiagnostic(
                    "Overlay.PointerReleased.DuplicateSuppressed",
                    "Origin=Xaml");
                _ = ReleaseCapture();
                args.Handled = true;
                return;
            }

            DispatchPointerReleased(pointer, "Xaml");
            _ = ReleaseCapture();
            args.Handled = true;
        }

        private void DispatchPointerReleased(SelectionPointerEvent pointer, string origin)
        {
            if (_disposed)
            {
                return;
            }

            string resultKind;
            if (_inputBoundary.UsesHighlighterTool)
            {
                resultKind = _inputBoundary.PointerReleasedHighlighter(pointer).Kind.ToString();
            }
            else if (_inputBoundary.UsesPrivacyRegionTool)
            {
                resultKind = _inputBoundary.PointerReleasedPrivacyRegion(pointer).Kind.ToString();
            }
            else if (_inputBoundary.UsesNumberedMarkerTool)
            {
                resultKind = _inputBoundary.PointerReleasedNumberedMarker(pointer).Kind.ToString();
            }
            else if (_inputBoundary.UsesArrowLineTool)
            {
                resultKind = _inputBoundary.PointerReleasedArrowLine(pointer).Kind.ToString();
            }
            else if (_inputBoundary.UsesRectangleTool)
            {
                resultKind = _inputBoundary.PointerReleasedRectangle(pointer).Kind.ToString();
            }
            else if (_inputBoundary.UsesObjectEditing)
            {
                resultKind = _inputBoundary.PointerReleasedObject(pointer).Kind.ToString();
            }
            else
            {
                resultKind = _inputBoundary.PointerReleased(pointer).Kind.ToString();
            }

            _traceSink?.Record(new CompleteExecutionTraceEntry
            {
                TimestampUtc = DateTimeOffset.UtcNow,
                SessionId = _descriptor.SessionId,
                SelectionRevision = -1,
                WorkflowState = WorkflowState.Selecting,
                CompleteStage = CompleteExecutionStage.Diagnostic,
                Component = nameof(WindowsFrozenDisplayOverlayCoordinator),
                ManagedThreadId = Environment.CurrentManagedThreadId,
                DiagnosticEvent = "Overlay.PointerReleased.Dispatched",
                DiagnosticMessage = $"Origin={origin};Result={resultKind}"
            });
        }

        private void OnPointerCaptureLost(object sender, PointerRoutedEventArgs args)
        {
            TracePointerDiagnostic("Overlay.PointerCaptureLost", "Origin=Xaml");
            SessionInputBoundary.NotifyCaptureChanged();
            args.Handled = true;
        }

        private void OnKeyDown(object sender, KeyRoutedEventArgs args)
        {
            if (args.Key == global::Windows.System.VirtualKey.Escape)
            {
                var sessionId = _descriptor.SessionId;
                var coordinateVersion = _descriptor.CoordinateVersion;
                _ = _canvas.DispatcherQueue.TryEnqueue(() =>
                {
                    _ = _inputBoundary.Escape(sessionId, coordinateVersion);
                });
                args.Handled = true;
            }
        }

        private static bool TryGetGlobalPointer(out PhysicalPoint point)
        {
            if (GetCursorPos(out var cursor))
            {
                point = new PhysicalPoint(cursor.X, cursor.Y);
                return true;
            }

            point = default;
            return false;
        }

        private static Rectangle CreateMask() => new()
        {
            Fill = new SolidColorBrush(ColorHelper.FromArgb(
                OverlayMaskAlpha,
                0,
                0,
                0))
        };

        private static Rectangle CreateHandle() => new()
        {
            Fill = new SolidColorBrush(ColorHelper.FromArgb(255, 255, 255, 255)),
            Stroke = new SolidColorBrush(ColorHelper.FromArgb(255, 40, 96, 160)),
            StrokeThickness = 1,
            Visibility = Visibility.Collapsed,
            IsHitTestVisible = false
        };

        private void ApplyCursor(SelectionVisualState state)
        {
            var hitTest = state.InteractionMode is
                SelectionInteractionMode.Moving
                or SelectionInteractionMode.ResizingLeft
                or SelectionInteractionMode.ResizingTop
                or SelectionInteractionMode.ResizingRight
                or SelectionInteractionMode.ResizingBottom
                or SelectionInteractionMode.ResizingTopLeft
                or SelectionInteractionMode.ResizingTopRight
                or SelectionInteractionMode.ResizingBottomLeft
                or SelectionInteractionMode.ResizingBottomRight
                ? state.ActiveHitTest
                : state.HoverHitTest;
            _canvas.Cursor = InputSystemCursor.Create(CursorShapeFor(hitTest));
        }

        private static InputSystemCursorShape CursorShapeFor(SelectionHitTestKind hitTest) => hitTest switch
        {
            SelectionHitTestKind.Interior => InputSystemCursorShape.SizeAll,
            SelectionHitTestKind.LeftEdge or SelectionHitTestKind.RightEdge =>
                InputSystemCursorShape.SizeWestEast,
            SelectionHitTestKind.TopEdge or SelectionHitTestKind.BottomEdge =>
                InputSystemCursorShape.SizeNorthSouth,
            SelectionHitTestKind.TopLeftCorner or SelectionHitTestKind.BottomRightCorner =>
                InputSystemCursorShape.SizeNorthwestSoutheast,
            SelectionHitTestKind.TopRightCorner or SelectionHitTestKind.BottomLeftCorner =>
                InputSystemCursorShape.SizeNortheastSouthwest,
            _ => InputSystemCursorShape.Cross
        };

        private void ApplyHandles(
            PhysicalRect selection,
            double canvasWidth,
            double canvasHeight)
        {
            foreach (var pair in _handles)
            {
                if (!TryGetHandleCenter(selection, pair.Key, out var center)
                    || !IsHandleOnDisplay(center, pair.Key))
                {
                    pair.Value.Visibility = Visibility.Collapsed;
                    continue;
                }

                var size = HandleVisualSizePixels / _rasterizationScale;
                var left = (center.X - _descriptor.PhysicalBoundsInVirtualDesktop.Left)
                    / _rasterizationScale - size / 2;
                var top = (center.Y - _descriptor.PhysicalBoundsInVirtualDesktop.Top)
                    / _rasterizationScale - size / 2;
                left = Math.Clamp(left, 0, Math.Max(0, canvasWidth - size));
                top = Math.Clamp(top, 0, Math.Max(0, canvasHeight - size));
                SetCanvasRectangle(pair.Value, left, top, size, size);
                pair.Value.Visibility = Visibility.Visible;
            }
        }

        private void HideHandles()
        {
            foreach (var handle in _handles.Values)
            {
                handle.Visibility = Visibility.Collapsed;
            }
        }

        private bool IsHandleOnDisplay(
            PhysicalPoint center,
            SelectionHitTestKind handle)
        {
            var probeX = UsesRightEdge(handle) ? center.X - 1 : center.X;
            var probeY = UsesBottomEdge(handle) ? center.Y - 1 : center.Y;
            var bounds = _descriptor.PhysicalBoundsInVirtualDesktop;
            return probeX >= bounds.Left
                && probeX < bounds.Right
                && probeY >= bounds.Top
                && probeY < bounds.Bottom;
        }

        private bool IsPointOnDisplay(PhysicalPoint point) =>
            point.X >= _descriptor.PhysicalBoundsInVirtualDesktop.Left
            && point.X < _descriptor.PhysicalBoundsInVirtualDesktop.Right
            && point.Y >= _descriptor.PhysicalBoundsInVirtualDesktop.Top
            && point.Y < _descriptor.PhysicalBoundsInVirtualDesktop.Bottom;

        private static bool TryGetHandleCenter(
            PhysicalRect selection,
            SelectionHitTestKind handle,
            out PhysicalPoint center)
        {
            var middleX = checked((int)(((long)selection.Left + selection.Right) / 2));
            var middleY = checked((int)(((long)selection.Top + selection.Bottom) / 2));
            center = handle switch
            {
                SelectionHitTestKind.LeftEdge => new(selection.Left, middleY),
                SelectionHitTestKind.TopEdge => new(middleX, selection.Top),
                SelectionHitTestKind.RightEdge => new(selection.Right, middleY),
                SelectionHitTestKind.BottomEdge => new(middleX, selection.Bottom),
                SelectionHitTestKind.TopLeftCorner => new(selection.Left, selection.Top),
                SelectionHitTestKind.TopRightCorner => new(selection.Right, selection.Top),
                SelectionHitTestKind.BottomLeftCorner => new(selection.Left, selection.Bottom),
                SelectionHitTestKind.BottomRightCorner => new(selection.Right, selection.Bottom),
                _ => default
            };
            return handle is not SelectionHitTestKind.Outside
                and not SelectionHitTestKind.Interior;
        }

        private static bool UsesRightEdge(SelectionHitTestKind handle) => handle is
            SelectionHitTestKind.RightEdge
            or SelectionHitTestKind.TopRightCorner
            or SelectionHitTestKind.BottomRightCorner;

        private static bool UsesBottomEdge(SelectionHitTestKind handle) => handle is
            SelectionHitTestKind.BottomEdge
            or SelectionHitTestKind.BottomLeftCorner
            or SelectionHitTestKind.BottomRightCorner;

        private sealed class PointerReleaseDispatchGate
        {
            private int _claimed;

            public void Reset() => Volatile.Write(ref _claimed, 0);

            public bool TryClaim() => Interlocked.CompareExchange(ref _claimed, 1, 0) == 0;
        }

        private sealed class CrosshairCanvas : Canvas
        {
            public InputCursor? Cursor
            {
                get => ProtectedCursor;
                set => ProtectedCursor = value;
            }
        }

        private static void SetMask(
            Rectangle rectangle,
            double left,
            double top,
            double width,
            double height) => SetCanvasRectangle(
            rectangle,
            left,
            top,
            width,
            height);

        private static void SetCanvasRectangle(
            FrameworkElement element,
            double left,
            double top,
            double width,
            double height)
        {
            Canvas.SetLeft(element, left);
            Canvas.SetTop(element, top);
            element.Width = Math.Max(0, width);
            element.Height = Math.Max(0, height);
        }

        private static void ApplyToolWindowStyle(nint handle)
        {
            var style = GetWindowLongPtr(handle, GwlExStyle);
            style &= ~WsExAppWindow;
            style |= WsExToolWindow;
            _ = SetWindowLongPtr(handle, GwlExStyle, style);
        }

        private static void FocusSessionEscapeOwner(OverlaySurface surface)
        {
            _ = SetForegroundWindow(surface._handle);
            _ = surface._canvas.Focus(FocusState.Programmatic);
        }

        private void InstallNativeInputBoundary()
        {
            _windowProc = WindowProc;
            _previousWindowProc = SetWindowLongPtr(
                _handle,
                GwlWndProc,
                Marshal.GetFunctionPointerForDelegate(_windowProc));
            if (_previousWindowProc == 0)
            {
                _windowProc = null;
                throw new Win32Exception(
                    Marshal.GetLastWin32Error(),
                    "The overlay native input boundary could not be installed.");
            }

            _nativeInputBoundaryInstalled = true;
        }

        private void RemoveNativeInputBoundary()
        {
            if (!_nativeInputBoundaryInstalled)
            {
                return;
            }

            _ = SetWindowLongPtr(_handle, GwlWndProc, _previousWindowProc);
            _nativeInputBoundaryInstalled = false;
            _previousWindowProc = 0;
            _windowProc = null;
        }

        private nint WindowProc(
            nint windowHandle,
            uint message,
            nint wParam,
            nint lParam)
        {
            if (!_disposed)
            {
                if (message == WmLButtonUp)
                {
                    var pointAvailable = TryGetGlobalPointer(out var point);
                    TracePointerDiagnostic(
                        "Overlay.PointerReleased.NativeMessage",
                        $"PointAvailable={pointAvailable}");
                    if (pointAvailable)
                    {
                        if (_pointerReleaseDispatchGate.TryClaim())
                        {
                            DispatchNativePointerReleased(point);
                        }
                        else
                        {
                            TracePointerDiagnostic(
                                "Overlay.PointerReleased.DuplicateSuppressed",
                                "Origin=Native");
                        }

                        _ = ReleaseCapture();
                    }
                }
                else if (message == WmCaptureChanged)
                {
                    TracePointerDiagnostic("Overlay.PointerCaptureChanged", "Origin=Native");
                    SessionInputBoundary.NotifyCaptureChanged();
                }
            }

            return CallWindowProc(_previousWindowProc, windowHandle, message, wParam, lParam);
        }

        private void DispatchNativePointerReleased(PhysicalPoint point)
        {
            if (_disposed)
            {
                return;
            }

            string resultKind;
            if (_inputBoundary.UsesHighlighterTool)
            {
                resultKind = _inputBoundary.PointerReleasedHighlighterFromNative(point).Kind.ToString();
            }
            else if (_inputBoundary.UsesPrivacyRegionTool)
            {
                resultKind = _inputBoundary.PointerReleasedPrivacyRegionFromNative(point).Kind.ToString();
            }
            else if (_inputBoundary.UsesNumberedMarkerTool)
            {
                resultKind = _inputBoundary.PointerReleasedNumberedMarkerFromNative(point).Kind.ToString();
            }
            else if (_inputBoundary.UsesArrowLineTool)
            {
                resultKind = _inputBoundary.PointerReleasedArrowLineFromNative(point).Kind.ToString();
            }
            else if (_inputBoundary.UsesRectangleTool)
            {
                resultKind = _inputBoundary.PointerReleasedRectangleFromNative(point).Kind.ToString();
            }
            else if (_inputBoundary.UsesObjectEditing)
            {
                resultKind = _inputBoundary.PointerReleasedObjectFromNative(point).Kind.ToString();
            }
            else
            {
                resultKind = _inputBoundary.PointerReleasedFromNative(point).Kind.ToString();
            }

            _traceSink?.Record(new CompleteExecutionTraceEntry
            {
                TimestampUtc = DateTimeOffset.UtcNow,
                SessionId = _descriptor.SessionId,
                SelectionRevision = -1,
                WorkflowState = WorkflowState.Selecting,
                CompleteStage = CompleteExecutionStage.Diagnostic,
                Component = nameof(WindowsFrozenDisplayOverlayCoordinator),
                ManagedThreadId = Environment.CurrentManagedThreadId,
                DiagnosticEvent = "Overlay.PointerReleased.Dispatched",
                DiagnosticMessage = $"Origin=Native;Result={resultKind}"
            });
        }

        private void TracePointerDiagnostic(string diagnosticEvent, string? diagnosticMessage = null)
        {
            _traceSink?.Record(new CompleteExecutionTraceEntry
            {
                TimestampUtc = DateTimeOffset.UtcNow,
                SessionId = _descriptor.SessionId,
                SelectionRevision = -1,
                WorkflowState = WorkflowState.Selecting,
                CompleteStage = CompleteExecutionStage.Diagnostic,
                Component = nameof(WindowsFrozenDisplayOverlayCoordinator),
                ManagedThreadId = Environment.CurrentManagedThreadId,
                DiagnosticEvent = diagnosticEvent,
                DiagnosticMessage = diagnosticMessage
            });
        }

        internal void TraceFunctionBarDiagnostic(string diagnosticEvent, string? diagnosticMessage = null)
        {
            _traceSink?.Record(new CompleteExecutionTraceEntry
            {
                TimestampUtc = DateTimeOffset.UtcNow,
                SessionId = _descriptor.SessionId,
                SelectionRevision = -1,
                WorkflowState = WorkflowState.Editing,
                CompleteStage = CompleteExecutionStage.Diagnostic,
                Component = nameof(WindowsFrozenDisplayOverlayCoordinator),
                ManagedThreadId = Environment.CurrentManagedThreadId,
                DiagnosticEvent = diagnosticEvent,
                DiagnosticMessage = diagnosticMessage
            });
        }

        [DllImport("user32.dll")]
        private static extern nint SetCapture(nint hWnd);

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate nint WindowProcDelegate(
            nint hWnd,
            uint message,
            nint wParam,
            nint lParam);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out PointNative lpPoint);

        private const uint MonitorDefaultToNearest = 2;

        [DllImport("user32.dll")]
        private static extern nint MonitorFromPoint(PointNative point, uint flags);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetMonitorInfo(
            nint monitor,
            ref MonitorInfoNative monitorInfo);

        [DllImport("user32.dll")]
        private static extern nint SetThreadDpiAwarenessContext(nint dpiContext);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(nint hWnd, int command);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(nint hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern nint BeginDeferWindowPos(int numberOfWindows);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern nint DeferWindowPos(
            nint hWinPosInfo,
            nint hWnd,
            nint hWndInsertAfter,
            int x,
            int y,
            int cx,
            int cy,
            uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EndDeferWindowPos(nint hWinPosInfo);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW")]
        private static extern nint GetWindowLongPtr(nint hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
        private static extern nint SetWindowLongPtr(nint hWnd, int nIndex, nint dwNewLong);

        [DllImport("user32.dll")]
        private static extern nint CallWindowProc(
            nint lpPrevWndFunc,
            nint hWnd,
            uint message,
            nint wParam,
            nint lParam);

        [StructLayout(LayoutKind.Sequential)]
        private readonly struct PointNative
        {
            public PointNative(int x, int y)
            {
                X = x;
                Y = y;
            }

            public readonly int X;
            public readonly int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MonitorInfoNative
        {
            public int Size;
            public RectNative Monitor;
            public RectNative Work;
            public uint Flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private readonly struct RectNative
        {
            public readonly int Left;
            public readonly int Top;
            public readonly int Right;
            public readonly int Bottom;
        }
    }
}
