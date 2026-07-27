# SnipPlus Implementation Contracts

## Document Control

| Field | Value |
| --- | --- |
| Document ID | `IMPLEMENTATION-CONTRACTS-001` |
| Status | `Accepted` |
| Version | `2.1` |
| Product revision date | `2026-07-27` |
| Scope | SnipPlus v1 resident PrintScreen、multi-display selection、editing、Clipboard and PNG output |
| Normative references | Accepted PRD-0004 through PRD-0006 and SPEC-0003 through SPEC-0010 |

## 1. Contract Principles

1. `COMP-001` is the sole shared Workflow State Authority.
2. Platform adapters return typed outcomes; they do not declare product completion or mutate shared state.
3. One capture session owns one stable Frozen Virtual Desktop context.
4. Selection、annotations、render requests and output requests carry the same Session ID and revision identities.
5. Mouse release locks selection and never commits output.
6. The editing／confirmation stage is mandatory; annotation actions are optional.
7. Complete and Save are explicit, distinct commitments.
8. Cleanup is idempotent and focus restoration is a workflow obligation.
9. Stale asynchronous outcomes never advance a newer or cancelled session.
10. MainWindow `X` exits SnipPlus、releases PrintScreen takeover and never hides the process to tray.
11. Physical non-display gaps render as transparent output pixels.
12. A successfully created PNG is retained if later Clipboard publication fails.
13. Private desktop content is never persisted as repository evidence.

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

## 3. Capture Session Context

```text
CaptureSessionContext
- SessionId
- RequestedAt
- Cancellation
- PreCaptureForegroundContext
- VirtualDesktopSnapshot
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

The contract supports negative origins、mixed-DPI displays and irregular arrangements. It does not require one giant bitmap.

## 4. Frozen Frame Ownership

- All required display frames are acquired before interactive selection begins.
- Each frame is immutable for the session.
- The session owner disposes every frame exactly once.
- Selection preview、annotation preview and final render reference these frames; they do not recapture desktop content.
- Frame acquisition failure prevents entry into Selection.
- Display-context mismatch returns a typed failure and never silently substitutes another frame.
- No display frame exists for physical gaps; gap output is generated as transparent pixels by the render contract.

## 5. Selection Contract

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
- Zero-size or invalid bounds cannot lock.
- Selection interior is presented without the dim mask; outside remains dimmed.
- Locking selection creates no Clipboard or file output.
- Moving、resizing or replacing selection increments Selection Revision.
- Selection operations are not part of Annotation Undo／Redo.

## 6. Editing Session Contract

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

The editing session begins after a valid selection lock and remains until Complete、successful Save、Cancel or terminal failure.

The user can commit with an empty AnnotationDocument. `RetainedOutputFiles` records files already created by a Save attempt whose later Clipboard obligation failed.

## 7. Annotation Document Contract

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
- visibility／selection state excluded from final output UI.

Required object kinds:

- Rectangle;
- ArrowLine;
- HighlighterStroke;
- Text;
- PrivacyRegion with Mosaic or Blur mode;
- NumberedMarker.

Required behavior:

- Objects are clipped, not deleted, when outside current selection.
- Selection changes do not transform objects.
- Applicable objects support select、move、resize、restyle and delete.
- Undo／Redo operates on annotation mutations only.

## 8. Tool-specific Contracts

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

## 9. Render Contract

```text
FinalRenderRequest
- SessionId
- SelectionRevisionId
- AnnotationRevisionId
- SelectionBounds
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

- Compose pixels from every display intersecting the selection.
- Use the same frozen frames shown during selection.
- Fill selected physical non-display gaps with transparent pixels (`BGRA alpha = 0`).
- Clip annotations to selection bounds.
- Exclude dim masks、selection border、handles、pointer、function bar and normal SnipPlus windows.
- Produce one immutable canonical ImageResult with alpha preserved.
- Rendering a stale revision cannot be committed.

## 10. Clipboard Contract

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
- Failure returns control to Editing with selection and annotations preserved.
- A Save-originated Clipboard failure never deletes or rolls back `RetainedFileReference`.
- Platform success does not directly transition workflow state.

## 11. PNG Save Contract

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

## 12. Focus and Cleanup Contract

```text
WorkflowCleanupRequest
- SessionId
- CompletionKind: Completed | Cancelled | Failed | ApplicationExit
- PreCaptureForegroundContext
```

Cleanup includes:

- close all display overlays;
- close function bars and transient editing UI;
- release pointer capture;
- cancel or invalidate pending session work;
- dispose frozen frames and temporary renders;
- restore the pre-capture foreground application where permitted for capture-session completion;
- never automatically foreground the SnipPlus main window;
- release PrintScreen takeover and terminate the process for `ApplicationExit`.

Cleanup is idempotent. User-created retained PNG files are not temporary resources and are not deleted by cleanup.

## 13. Workflow Transitions

Minimum legal transitions:

```text
ResidentReady → CaptureRequested → Freezing → Selecting
Selecting → SelectionLocked | Cancelled | Failed
SelectionLocked → Selecting | Editing | Cancelled | Failed
Editing → CommittingClipboard | Saving | Cancelled | Failed
CommittingClipboard → Completed | Editing
Saving → Completed | Editing
Completed | Cancelled | Failed → ResidentReady
ResidentReady → Exited
```

A `Saving → Editing` transition may include a retained PNG FileReference.

Only `COMP-001` applies transitions.

## 14. Failure Contract

Every Failure includes:

- stable code;
- category;
- recoverability;
- operation boundary;
- Session ID;
- user-facing message key;
- diagnostic details without private desktop content;
- optional retained output FileReference.

Recoverable output failure preserves Editing. Clipboard failure after PNG success reports the retained file. Terminal capture-context failure cleans up and restores focus.

## 15. Verification Obligations

Tests must cover at least:

- takeover enabled／disabled behavior;
- MainWindow `X` and explicit Exit release takeover and terminate rather than hide to tray;
- all-display frame acquisition before Selection;
- cross-display selection、negative coordinates and transparent gap output;
- mouse release without output;
- selection move、resize and replacement;
- annotation clipping and coordinate stability;
- tool object creation and applicable editing;
- annotation Undo／Redo boundaries;
- Complete Clipboard-only flow;
- Save As initial Downloads folder、destination override、cancel and PNG failure;
- Clipboard failure after PNG success retains the file and returns to Editing;
- stale session／revision outcomes;
- cleanup and focus-restoration requests;
- no automatic external GUI fixture in non-interactive tests.

The previous single-monitor、optional post-capture Annotation、automatic Clipboard、close-to-tray、opaque gap and rollback-undecided contracts are superseded.