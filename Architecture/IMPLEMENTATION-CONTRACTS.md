# SnipPlus Implementation Contracts

## Document Control

| Field | Value |
| --- | --- |
| Document ID | `IMPLEMENTATION-CONTRACTS-001` |
| Status | `Accepted` |
| Version | `2.3` |
| Product revision date | `2026-07-27` |
| Scope | SnipPlus v1 resident PrintScreen、four-4K multi-display Selection、pointer-driven Editing、Clipboard and PNG output |
| Normative references | Accepted PRD-0004 through PRD-0006 and SPEC-0003 through SPEC-0010 |

## 1. Contract Principles

1. `COMP-001` is the sole shared Workflow State Authority.
2. Platform adapters return typed outcomes and never mutate shared workflow state.
3. One capture Session owns one stable Frozen Virtual Desktop context.
4. Selection、Annotation、render and output requests carry the same Session ID and revision identities.
5. Mouse release locks Selection and never commits output.
6. Editing／confirmation is mandatory; Annotation actions are optional.
7. Complete and Save are explicit、distinct commitments.
8. Cleanup is idempotent and focus restoration is a workflow obligation.
9. Stale asynchronous outcomes never advance a newer or cancelled Session.
10. MainWindow `X` exits SnipPlus、releases PrintScreen takeover and never hides the process to tray.
11. Physical non-display gaps render as transparent output pixels.
12. A successfully created PNG is retained if later Clipboard publication fails.
13. Supported capacity is validated before interactive Selection and final render; partial display capture is prohibited.
14. Performance targets are measured release gates, not arbitrary runtime timeouts.
15. V1 Annotation and object editing are pointer-driven.
16. Keyboard-only Annotation and non-PrintScreen tool／action shortcuts are deferred.
17. PrintScreen capture entry and Esc cancellation remain required.
18. Private desktop content is never persisted as repository evidence.

## 2. Resident Entry and Exit Contract

```text
ResidentCaptureEntry
- TakeoverEnabled: bool
- TryAcceptPrintScreen(): CaptureEntryOutcome
- DisableTakeover(): void
- ExitApplication(): ExitOutcome
```

Required semantics:

- SnipPlus is manually started before it can accept PrintScreen.
- Enabled takeover accepts one request when workflow state allows it.
- Disabled takeover does not intercept PrintScreen.
- An in-app entry may call the same request boundary but remains secondary.
- MainWindow `X` invokes `ExitApplication`; it is not hide-to-tray.
- Any explicit tray Exit command invokes the same boundary.
- Exit releases interception、invalidates owned work、cleans resources and terminates the process.
- No hidden resident process remains after Exit.

## 3. Supported Capacity Contract

```text
SupportedCapacityPolicy
- MinDisplayCount: 1
- MaxDisplayCount: 4
- MaxDisplayWidth: 3840
- MaxDisplayHeight: 2160
- MaxTotalSourcePixels: 33177600
- MaxVirtualDesktopWidth: 16384
- MaxVirtualDesktopHeight: 16384
- MaxSelectionWidth: 16384
- MaxSelectionHeight: 16384
- MaxSelectionArea: 67108864
```

```text
CapacityValidationOutcome
- Supported
- UnsupportedDisplayCount
- UnsupportedDisplayDimensions
- UnsupportedTotalSourcePixels
- UnsupportedVirtualDesktopBounds
- UnsupportedSelectionDimensions
- UnsupportedSelectionArea
```

Required semantics:

- Active logical desktop surfaces are counted; mirrored outputs resolving to one logical surface count once.
- Each display is no larger than 4K UHD physical resolution.
- An 8K display is unsupported in v1.
- Transparent topology gaps count toward final Selection area.
- The larger final-area ceiling exists to permit transparent gaps while preserving the four-4K source guarantee.
- Topology capacity is validated before interactive Selection.
- Selection capacity is validated before `SelectionLocked` and final render.
- Unsupported capacity never omits、downscales or partially captures displays.
- Unsupported capacity returns a typed terminal outcome、cleans resources、restores the previous work context and returns to `ResidentReady`.

## 4. Verification Profiles

```text
OwnerReferenceProfile
- Display1: 2560x1440 primary
- Display2: 1920x1080 lower, Windows scale 150%
- Display3: 2560x1440 left
```

```text
StandardProfile
- DisplayCount: <= 2
- TotalSourcePixels: <= 16588800
```

```text
MaximumProfile
- DisplayCount: <= 4
- EachDisplay: <= 3840x2160
- TotalSourcePixels: <= 33177600
```

Final acceptance includes the Repository owner’s three-display mixed-DPI configuration and a Maximum-profile configuration.

## 5. Capture Session Context

```text
CaptureSessionContext
- SessionId
- RequestedAt
- Cancellation
- PreCaptureForegroundContext
- VirtualDesktopSnapshot
- CapacityValidation
- FrozenDisplayFrames
```

The context is immutable except for owned disposable resources and cancellation state.

### Virtual Desktop Snapshot

```text
VirtualDesktopSnapshot
- CoordinateVersion
- VirtualPhysicalBounds
- VirtualOrigin
- Displays[]
- GapPolicy: Transparent
```

### Display Snapshot

```text
DisplaySnapshot
- DisplayId
- PhysicalBoundsInVirtualDesktop
- DpiScaleX
- DpiScaleY
- RotationOrOrientation
- FrozenFrameId
- FrozenFramePixelSize
```

The model supports negative origins、mixed DPI and irregular arrangements inside the accepted envelope. One giant bitmap is not required.

## 6. Frozen Frame Ownership

- All required display frames are acquired before interactive Selection begins.
- Each frame is immutable for the Session.
- The Session owner disposes every frame exactly once.
- Selection preview、Annotation preview and final render use the same frames; they never recapture desktop content.
- Frame acquisition failure prevents entry into Selection.
- Display-context mismatch returns typed failure.
- Physical gaps have no frame and are produced as transparent pixels by final render.

## 7. Selection Contract

```text
SelectionState
- SessionId
- RevisionId
- BoundsInVirtualDesktop
- Phase: None | Dragging | Locked
- HandleMode
```

Valid pointer operations:

- BeginSelection(point)
- UpdateSelection(point)
- LockSelection()
- MoveSelection(delta)
- ResizeSelection(handle, point)
- ReplaceSelection(newBounds)
- CancelSession()

Required semantics:

- One rectangle may span multiple displays and physical gaps.
- Zero-size、invalid or capacity-exceeding bounds cannot lock.
- Selection interior is unmasked; outside remains dimmed.
- Locking Selection creates no Clipboard or file output.
- Moving、resizing or replacing increments Selection Revision.
- Selection operations are not part of Annotation Undo／Redo.
- Keyboard movement and resize are not v1 requirements.

## 8. Editing Session Contract

```text
EditingSession
- SessionId
- SelectionRevisionId
- AnnotationRevisionId
- ActiveTool
- AnnotationDocument
- CanUndo
- CanRedo
- RetainedOutputFiles[]
```

Editing begins after a valid Selection lock and remains until Complete、successful Save、Cancel or terminal failure.

The user may commit with an empty AnnotationDocument. `RetainedOutputFiles` records PNG files created by a Save attempt whose later Clipboard obligation failed.

## 9. Annotation Document Contract

```text
AnnotationDocument
- SessionId
- RevisionId
- Objects[]
- UndoStack
- RedoStack
```

Every object includes:

- ObjectId;
- ToolKind;
- Geometry in Frozen Virtual Desktop coordinates;
- Z-order;
- supported style properties;
- object-specific content;
- transient selection state excluded from final output.

Required object kinds:

- Rectangle;
- ArrowLine;
- HighlighterStroke;
- Text;
- PrivacyRegion with Mosaic or Blur mode;
- NumberedMarker.

Required behavior:

- Objects are clipped, not deleted, outside current Selection.
- Selection changes do not transform objects.
- Applicable objects support pointer select、move、resize、restyle and delete.
- Function-bar Undo／Redo operates on Annotation mutations only.
- Text editing supports Chinese IME input.
- Keyboard-only object creation／editing and non-PrintScreen shortcuts are deferred.

## 10. Tool-specific Contracts

### ArrowLine

```text
ArrowLineStyle
- EndStyle: Arrow | None
- Color
- Thickness
```

### Highlighter

```text
HighlighterStyle
- Color
- Thickness
- Opacity
- RoundedEnds: true
```

### Text

```text
TextStyle
- FontFamily: Microsoft JhengHei
- FontSize
- Color
- Bold
```

### Privacy Region

```text
PrivacyEffect
- Mode: Mosaic | Blur
- BoundsInVirtualDesktop
- Strength
```

### Numbered Marker

```text
NumberedMarker
- Number
- Center
- Size
- Color
```

Deleting a marker does not recalculate other numbers. Next-number state is explicit and Undo restores original numbers.

## 11. Keyboard Boundary Contract

Required keys:

```text
PrintScreen  Start capture when takeover is enabled
Esc          Cancel the current capture session according to workflow stage
```

Deferred from v1:

- F6／Tab zone traversal as a complete workflow;
- single-letter tool shortcuts;
- Ctrl+Z／Ctrl+Y／Ctrl+S／Ctrl+Enter shortcuts;
- Delete shortcut;
- Arrow-key object or Selection manipulation;
- keyboard-created Annotation objects;
- pointer-unused Editing acceptance.

Platform controls may retain normal text-entry behavior, but no broader keyboard-only product claim may be made.

## 12. Render Contract

```text
FinalRenderRequest
- SessionId
- SelectionRevisionId
- AnnotationRevisionId
- SelectionBounds
- CapacityValidation
- VirtualDesktopSnapshot
- FrozenFrames
- AnnotationDocument
```

Required semantics:

- Revalidate Selection dimensions and area before allocation.
- Compose every display intersecting the Selection.
- Use the same frozen frames shown during Selection.
- Fill non-display gaps with transparent pixels (`BGRA alpha = 0`).
- Clip annotations to Selection bounds.
- Exclude masks、Selection border、handles、pointer、function bar and normal SnipPlus windows.
- Produce one immutable canonical ImageResult with alpha preserved.
- Stale revisions cannot be committed.
- Over-capacity output returns typed failure and never allocates an unbounded bitmap.

## 13. Clipboard Contract

```text
ClipboardCommitRequest
- SessionId
- ResultId
- ImageResult
- Origin: Complete | Save
- RetainedFileReference: optional
```

Required semantics:

- Invoked only by Complete or after successful PNG creation in Save.
- Bounded、cancellable busy retry is allowed.
- History and roaming remain disabled by default.
- Payload lifetime is preserved after success.
- Failure returns control to Editing with Selection and annotations preserved.
- Save-originated failure never deletes or rolls back the retained PNG.
- Platform success does not directly transition workflow state.

## 14. PNG Save Contract

```text
SaveRequest
- SessionId
- ResultId
- SuggestedFolder: Downloads
- SuggestedFileName
- Format: PNG
- ImageResult
```

Required semantics:

- Save As is shown each time.
- Suggested folder is Downloads.
- Suggested filename is `SnipPlus_yyyy-MM-dd_HHmmss.png`.
- The user may choose another folder or filename.
- Save As cancellation returns to Editing without Clipboard update.
- PNG failure returns to Editing without Clipboard update.
- After PNG success, the same ImageResult is submitted to Clipboard.
- The PNG is retained immediately after successful file creation.
- Later Clipboard failure retains the PNG、returns to Editing and reports the partial outcome.
- Overall Save completes only after both PNG and Clipboard succeed.

## 15. Performance Contract

Measurement protocol:

```text
WarmupRuns: 3
MeasuredRuns: >= 30
Report: P50, P95, Maximum
```

Reference environment:

- Windows 11 24H2 x64;
- 16 GB RAM or more;
- Direct3D 11-class hardware acceleration;
- SSD;
- Release x64 without debugger.

Release targets:

- PrintScreen → interactive Selection: p95 `≤ 500 ms` Owner Reference／Standard、`≤ 1,000 ms` Maximum.
- Pointer-driven Selection／Annotation frame time: p95 `≤ 33 ms`.
- Pointer／UI action → visible response: p95 `≤ 100 ms`.
- Complete: `≤ 8,294,400` pixels p95 `≤ 1.5 s`; `≤ 33,177,600` p95 `≤ 4 s`; `≤ 67,108,864` p95 `≤ 8 s`.
- Save after Save As confirmation: same tiers p95 `≤ 2 s`、`≤ 6 s`、`≤ 12 s`.
- Commit still running after `300 ms` exposes non-blocking progress.
- Idle private working set `≤ 250 MB`.
- Maximum-envelope peak private working set `≤ 2.0 GB`.
- Within `10 s` of cleanup, working set returns to idle baseline plus `150 MB` or less.
- After `20` Standard sessions, retained steady-state growth is `≤ 50 MB`.

Save As user decision time is excluded.

## 16. Focus、Progress and Cleanup Contract

Cleanup includes:

- close all display overlays;
- close function bars and transient Editing UI;
- release pointer capture;
- cancel or invalidate pending Session work;
- dispose frozen frames and temporary renders;
- restore the pre-capture foreground application where permitted;
- never automatically foreground the SnipPlus main window;
- release PrintScreen takeover and terminate for `ApplicationExit`.

Additional rules:

- Progress appears after `300 ms` for an unfinished commit.
- Success remains silent.
- Cleanup is idempotent.
- User-created retained PNG files are not temporary resources and are not deleted by cleanup.

## 17. Workflow Transitions

```text
ResidentReady → CaptureRequested → Freezing → Selecting
CaptureRequested | Freezing → Failed when capacity is unsupported
Selecting → SelectionLocked | Cancelled | Failed
SelectionLocked → Selecting | Editing | Cancelled | Failed
Editing → CommittingClipboard | Saving | Cancelled | Failed
CommittingClipboard → Completed | Editing
Saving → Completed | Editing
Completed | Cancelled | Failed → ResidentReady
ResidentReady → Exited
```

Only `COMP-001` applies transitions.

## 18. Verification Obligations

Tests and authorized runtime evidence must cover:

- takeover enabled／disabled and exact release on exit;
- four-4K capacity boundaries and all over-limit failure classes;
- Owner Reference three-display mixed-DPI mapping;
- all-display frame acquisition before Selection;
- cross-display Selection、negative coordinates and transparent gaps;
- mouse release without output;
- pointer-driven Selection move、resize and replacement;
- pointer-driven Annotation creation and applicable editing;
- Annotation clipping and Undo／Redo boundaries;
- Complete Clipboard-only flow;
- Save As Downloads default、destination override、cancel and PNG failure;
- PNG retention after later Clipboard failure;
- Esc cancellation at accepted workflow stages;
- progress after `300 ms`;
- Owner Reference、Standard and Maximum timing／memory scenarios;
- stale Session／revision outcomes;
- cleanup and focus restoration;
- required accessible names and non-color-only state indicators;
- absence of any v1 keyboard-only Annotation conformance claim;
- no automatic external GUI fixture in non-interactive tests.

The previous 8K-capable envelope and complete keyboard-only Annotation contract are superseded.