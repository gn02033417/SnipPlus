# SnipPlus Implementation Contracts

## Document Control

| Field | Value |
| --- | --- |
| Document ID | `IMPLEMENTATION-CONTRACTS-001` |
| Status | `Accepted` |
| Version | `2.2` |
| Product revision date | `2026-07-27` |
| Scope | SnipPlus v1 resident PrintScreen、multi-display Selection、Editing、Clipboard and PNG output |
| Normative references | Accepted PRD-0004 through PRD-0006 and SPEC-0003 through SPEC-0010 |

## 1. Contract Principles

1. `COMP-001` is the sole shared Workflow State Authority.
2. Platform adapters return typed outcomes; they do not declare product completion or mutate shared state.
3. One capture session owns one stable Frozen Virtual Desktop context.
4. Selection、annotations、render requests and output requests carry the same Session ID and revision identities.
5. Mouse release locks Selection and never commits output.
6. The Editing／confirmation stage is mandatory; Annotation actions are optional.
7. Complete and Save are explicit、distinct commitments.
8. Cleanup is idempotent and focus restoration is a workflow obligation.
9. Stale asynchronous outcomes never advance a newer or cancelled session.
10. MainWindow `X` exits SnipPlus、releases PrintScreen takeover and never hides the process to tray.
11. Physical non-display gaps render as transparent output pixels.
12. A successfully created PNG is retained if later Clipboard publication fails.
13. Supported capacity is validated before interactive Selection and before final render; partial display capture is prohibited.
14. Performance targets are measured release gates, not arbitrary runtime timeouts.
15. From `SelectionLocked`, every required Editing／Annotation operation is available without pointer input.
16. Private desktop content is never persisted as repository evidence.

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
- An in-app entry can call the same request boundary but is secondary.
- MainWindow `X` invokes `ExitApplication`; it is not a hide-to-tray command.
- If a System Tray surface exists, its explicit Exit command invokes the same boundary.
- Exit releases PrintScreen interception、cancels or invalidates owned work、cleans up resources and terminates the process.
- No hidden resident process remains after Exit.

## 3. Supported Capacity Contract

```text
SupportedCapacityPolicy
- MinDisplayCount: 1
- MaxDisplayCount: 4
- MaxDisplayWidth: 7680
- MaxDisplayHeight: 4320
- MaxTotalSourcePixels: 66355200
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
- Transparent topology gaps count toward final Selection area.
- Topology capacity is validated before acquiring or presenting interactive Selection.
- Selection capacity is validated before `SelectionLocked` and before final render.
- Unsupported capacity never omits、downscales or partially captures displays.
- Unsupported capacity returns a typed terminal outcome、cleans up partial resources、restores the previous work context and returns to `ResidentReady`.

## 4. Capture Session Context

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
- Rotation／orientation metadata when required
- FrozenFrameId
- FrozenFramePixelSize
```

The contract supports negative origins、mixed-DPI displays and irregular arrangements inside the accepted envelope. It does not require one giant bitmap.

## 5. Frozen Frame Ownership

- All required display frames are acquired before interactive Selection begins.
- Each frame is immutable for the session.
- The session owner disposes every frame exactly once.
- Selection preview、Annotation preview and final render reference these frames; they do not recapture desktop content.
- Frame acquisition failure prevents entry into Selection.
- Display-context mismatch returns a typed failure and never silently substitutes another frame.
- No display frame exists for physical gaps; gap output is generated as transparent pixels by the render contract.

## 6. Selection Contract

```text
SelectionState
- SessionId
- RevisionId
- BoundsInVirtualDesktop
- Phase: None | Dragging | Locked
- HandleMode
```

Valid operations:

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
- Selection interior is presented without the dim mask; outside remains dimmed.
- Locking Selection creates no Clipboard or file output.
- Moving、resizing or replacing Selection increments Selection Revision.
- Selection operations are not part of Annotation Undo／Redo.
- Keyboard movement and resize use `1` physical pixel per Arrow and `10` per Shift+Arrow.

## 7. Editing Session Contract

```text
EditingSession
- SessionId
- SelectionRevisionId
- AnnotationRevisionId
- ActiveTool
- AnnotationDocument
- KeyboardFocusContext
- CanUndo
- CanRedo
- RetainedOutputFiles[]
```

The Editing session begins after a valid Selection lock and remains until Complete、successful Save、Cancel or terminal failure.

The user can commit with an empty AnnotationDocument. `RetainedOutputFiles` records files already created by a Save attempt whose later Clipboard obligation failed.

## 8. Keyboard Focus and Command Contract

```text
KeyboardFocusContext
- ActiveZone: FunctionBar | Canvas
- FocusedControlId: optional
- FocusedObjectId: optional
- FocusedHandle: optional
- TransientOwner: None | Picker | Popover | TextEditor | ObjectCreation | SaveDialog
```

Required commands when text entry is not active:

```text
F6             Cycle FunctionBar／Canvas zones
Tab            Next control／object／handle in active zone
Shift+Tab      Previous control／object／handle
V/R/A/H/T/M/N Select required tools
Ctrl+Z/Y      Undo／Redo
Ctrl+S        Save
Ctrl+Enter    Complete
Delete        Delete selected annotation
Arrow         Move selected object／Selection or focused handle by 1 pixel
Shift+Arrow   Same operation by 10 pixels
Enter／Space  Activate focused command or value action
```

Required semantics:

- `F6` cycles major zones; `Tab` traversal inside Canvas follows Selection、objects in deterministic z-order and applicable handles.
- Keyboard tool activation creates a deterministic default object inside the current Selection and focuses it.
- Highlighter creates a short horizontal stroke、Text creates and focuses a text box、Numbered Marker is placed at Selection center.
- Tool shortcuts do not fire while text entry owns the keystroke.
- First Esc closes or abandons `TransientOwner`; Esc from stable Editing cancels the session.
- Dialog、picker、text-editor、failure and Save As transitions restore focus to the invoking context where possible.
- Focus is visible by more than color alone and exposed through accessibility APIs.
- No keyboard trap is permitted.

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
- visibility／selection state excluded from final output UI;
- keyboard-accessible move and applicable resize semantics.

Required object kinds:

- Rectangle;
- ArrowLine;
- HighlighterStroke;
- Text;
- PrivacyRegion with Mosaic or Blur mode;
- NumberedMarker.

Required behavior:

- Objects are clipped, not deleted, when outside current Selection.
- Selection changes do not transform objects.
- Applicable objects support select、move、resize、restyle and delete through pointer and keyboard.
- Undo／Redo operates on Annotation mutations only.
- Text editing supports normal Windows editing and Chinese IME input.

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

Font-family selection、italic、underline and background are not in v1.

### Privacy Region

```text
PrivacyEffect
- Mode: Mosaic | Blur
- BoundsInVirtualDesktop
- Strength
```

The mode is stored per object.

### Numbered Marker

```text
NumberedMarker
- Number
- Center
- Size
- Color
```

Deleting a marker does not recalculate other numbers. Next-number state is explicit and Undo restores original numbers.

## 11. Render Contract

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

```text
FinalRenderOutcome
- Success(ImageResult)
- Cancelled
- Failed(Failure)
```

Required semantics:

- Revalidate Selection dimensions and area before allocating final output.
- Compose pixels from every display intersecting the Selection.
- Use the same frozen frames shown during Selection.
- Fill selected physical non-display gaps with transparent pixels (`BGRA alpha = 0`).
- Clip annotations to Selection bounds.
- Exclude dim masks、Selection border、handles、pointer、function bar and normal SnipPlus windows.
- Produce one immutable canonical ImageResult with alpha preserved.
- Rendering a stale revision cannot be committed.
- Output beyond the capacity envelope returns typed failure and never allocates an unbounded bitmap.

## 12. Clipboard Contract

```text
ClipboardCommitRequest
- SessionId
- ResultId
- ImageResult
- Origin: Complete | Save
- RetainedFileReference: optional
```

```text
ClipboardCommitOutcome
- Succeeded
- RetryableFailure
- Failed
- Cancelled
```

Required semantics:

- Invoked only by Complete or after successful PNG creation in Save.
- Bounded、cancellable busy retry is allowed.
- History and roaming remain disabled by default.
- Payload lifetime is preserved after success.
- Failure returns control to Editing with Selection、annotations and focus context preserved.
- A Save-originated Clipboard failure never deletes or rolls back `RetainedFileReference`.
- Platform success does not directly transition workflow state.

## 13. PNG Save Contract

```text
SaveRequest
- SessionId
- ResultId
- SuggestedFolder: Downloads
- SuggestedFileName
- Format: PNG
- ImageResult
```

```text
SaveOutcome
- Succeeded(FileReference)
- UserCancelled
- Failed(Failure)
```

Required semantics:

- Suggested folder is the user’s Downloads folder.
- Suggested filename is `SnipPlus_yyyy-MM-dd_HHmmss.png`.
- Save As is shown each time.
- The user may choose another folder or filename.
- UserCancelled returns to Editing without Clipboard update.
- Save failure returns to Editing without Clipboard update.
- After Save success, the same ImageResult is submitted to Clipboard.
- The created PNG is retained immediately after successful file creation.
- If later Clipboard publication fails, retain the PNG、attach the FileReference to the recoverable outcome and return to Editing.
- The overall Save workflow completes only after both PNG and Clipboard succeed.

## 14. Performance Contract

```text
PerformanceProfile
- Name: Standard | Maximum
- DisplayCount
- TotalSourcePixels
- OutputPixels
```

```text
PerformanceMeasurement
- ScenarioId
- WarmupRuns: 3
- MeasuredRuns: >= 30
- P50
- P95
- Maximum
- PrivateWorkingSet
```

Required release targets:

- PrintScreen accepted → interactive all-display Selection: p95 `≤ 500 ms` Standard、`≤ 1,000 ms` Maximum.
- Selection／Annotation frame time: p95 `≤ 33 ms`.
- Discrete input → visible response: p95 `≤ 100 ms`.
- Complete: `≤ 8,294,400` pixels p95 `≤ 1.5 s`; `≤ 33,177,600` p95 `≤ 4 s`; `≤ 67,108,864` p95 `≤ 8 s`.
- Save after Save As confirmation: same tiers p95 `≤ 2 s`、`≤ 6 s`、`≤ 12 s`.
- A commit still running after `300 ms` exposes non-blocking progress.
- Idle private working set `≤ 250 MB`.
- Maximum-envelope peak private working set `≤ 2.0 GB`.
- Within `10 s` of cleanup, working set returns to idle baseline plus `150 MB` or less.
- After `20` Standard sessions, retained steady-state growth is `≤ 50 MB`.

Measurement uses Release x64、no debugger、Windows 11 24H2 x64、16 GB RAM or more、hardware-accelerated Direct3D 11-class GPU and SSD. Save As user decision time is excluded.

## 15. Progress and Feedback Contract

```text
CommitProgressState
- SessionId
- Operation: Complete | Save
- StartedAt
- IsCancellable
- MessageKey
```

Required semantics:

- Do not display success feedback.
- If commit duration reaches `300 ms`, display progress without blocking input processing.
- Progress is removed on success、recoverable failure、Cancel or terminal failure.
- Unsupported capacity reports the relevant supported limit and no private display identifier.
- Recoverable failure restores Editing focus context.

## 16. Focus and Cleanup Contract

```text
WorkflowCleanupRequest
- SessionId
- CompletionKind: Completed | Cancelled | Failed | ApplicationExit
- PreCaptureForegroundContext
```

Cleanup includes:

- close all display overlays;
- close function bars and transient Editing UI;
- release pointer capture;
- release keyboard focus ownership held by transient UI;
- cancel or invalidate pending session work;
- dispose frozen frames and temporary renders;
- restore the pre-capture foreground application where permitted for capture-session completion;
- never automatically foreground the SnipPlus main window;
- release PrintScreen takeover and terminate the process for `ApplicationExit`.

Cleanup is idempotent. User-created retained PNG files are not temporary resources and are not deleted by cleanup.

## 17. Workflow Transitions

Minimum legal transitions:

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

A `Saving → Editing` transition may include a retained PNG FileReference. Transient Editing dismissal does not leave `Editing`.

Only `COMP-001` applies transitions.

## 18. Failure Contract

Every Failure includes:

- stable code;
- category;
- recoverability;
- operation boundary;
- Session ID;
- user-facing message key;
- diagnostic details without private desktop content;
- optional retained output FileReference;
- optional capacity-limit classification;
- optional keyboard focus restoration target.

Recoverable output failure preserves Editing. Clipboard failure after PNG success reports the retained file. Unsupported capacity and terminal capture-context failure clean up and restore focus.

## 19. Verification Obligations

Tests must cover at least:

- takeover enabled／disabled behavior;
- MainWindow `X` and explicit Exit release takeover and terminate rather than hide to tray;
- every supported-capacity boundary and each over-limit failure class;
- all-display frame acquisition before Selection;
- cross-display Selection、negative coordinates and transparent gap output;
- mouse release without output;
- Selection move、resize and replacement;
- keyboard `1`／`10` pixel movement and handle resize;
- F6、Tab、Shift+Tab focus order and no keyboard trap;
- keyboard creation of every required v1 tool;
- Chinese IME input without tool-shortcut interception;
- Annotation clipping and coordinate stability;
- tool object creation and applicable editing;
- Annotation Undo／Redo boundaries;
- Complete Clipboard-only flow;
- Save As initial Downloads folder、destination override、cancel and PNG failure;
- Clipboard failure after PNG success retains the file and returns to Editing;
- transient Esc before stable-Editing cancellation;
- progress display after `300 ms`;
- Standard and Maximum performance timing、memory and repeated-session cleanup;
- stale session／revision outcomes;
- cleanup and focus-restoration requests;
- visible focus、High Contrast、200% scaling and Narrator state;
- no automatic external GUI fixture in non-interactive tests.

The previous undefined capacity、unquantified performance and incomplete keyboard-accessibility contracts are superseded.