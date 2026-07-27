# SnipPlus Implementation Contracts

## Document Control

| Field | Value |
| --- | --- |
| Document ID | `IMPLEMENTATION-CONTRACTS-001` |
| Status | `Accepted` |
| Version | `2.0` |
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
10. Private desktop content is never persisted as repository evidence.

## 2. Resident Entry Contract

```text
ResidentCaptureEntry
- TakeoverEnabled: bool
- TryAcceptPrintScreen(): CaptureEntryOutcome
- DisableTakeover(): void
```

Required semantics:

- SnipPlus is manually started before it can accept PrintScreen.
- Enabled takeover accepts one request when workflow state allows it.
- Disabled takeover does not intercept PrintScreen.
- Process exit or disabling takeover releases interception.
- An in-app entry can call the same request boundary but is secondary.

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

The contract supports negative origins and mixed-DPI displays. It does not require one giant bitmap.

## 4. Frozen Frame Ownership

- All required display frames are acquired before interactive selection begins.
- Each frame is immutable for the session.
- The session owner disposes every frame exactly once.
- Selection preview、annotation preview and final render reference these frames; they do not recapture desktop content.
- Frame acquisition failure prevents entry into Selection.
- Display-context mismatch returns a typed failure and never silently substitutes another frame.

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

- One rectangle may span multiple displays.
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
```

The editing session begins after a valid selection lock and remains until Complete、successful Save、Cancel or terminal failure.

The user can commit with an empty AnnotationDocument.

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
- visibility／selection state that is not included in final output UI.

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

- Compose pixels from all displays intersecting the selection.
- Use the same frozen frames shown during selection.
- Clip annotations to selection bounds.
- Exclude dim masks、selection border、handles、pointer、function bar and normal SnipPlus windows.
- Produce one immutable canonical ImageResult.
- Rendering a stale revision cannot be committed.

Representation of non-display gaps in irregular layouts remains unresolved and must be decided before that path is implemented.

## 10. Clipboard Contract

```text
ClipboardCommitRequest
- SessionId
- ResultId
- ImageResult
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
- Platform success does not directly transition workflow state.

## 11. PNG Save Contract

```text
SaveRequest
- SessionId
- ResultId
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

- Suggested filename is `SnipPlus_yyyy-MM-dd_HHmmss.png`.
- Save As is shown each time.
- UserCancelled returns to Editing without Clipboard update.
- Save failure returns to Editing without Clipboard update.
- After Save success, the same ImageResult is submitted to Clipboard.
- The overall Save workflow completes only after both operations succeed.
- Rollback of an already-created file after Clipboard failure remains unresolved.

## 12. Focus and Cleanup Contract

```text
WorkflowCleanupRequest
- SessionId
- CompletionKind: Completed | Cancelled | Failed
- PreCaptureForegroundContext
```

Cleanup includes:

- close all display overlays;
- close function bars and transient editing UI;
- release pointer capture;
- cancel or invalidate pending session work;
- dispose frozen frames and temporary renders;
- restore the pre-capture foreground application where permitted;
- never automatically foreground the SnipPlus main window.

Cleanup is idempotent.

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
```

Only `COMP-001` applies transitions.

## 14. Failure Contract

Every Failure includes:

- stable code;
- category;
- recoverability;
- operation boundary;
- Session ID;
- user-facing message key;
- diagnostic details without private desktop content.

Recoverable output failure preserves Editing. Terminal capture-context failure cleans up and restores focus.

## 15. Verification Obligations

Tests must cover at least:

- takeover enabled／disabled behavior;
- all-display frame acquisition before Selection;
- cross-display selection and negative coordinates;
- mouse release without output;
- selection move、resize and replacement;
- annotation clipping and coordinate stability;
- tool object creation and applicable editing;
- annotation Undo／Redo boundaries;
- Complete Clipboard-only flow;
- Save As cancel、Save failure、Clipboard failure and success;
- stale session／revision outcomes;
- cleanup and focus-restoration requests;
- no automatic external GUI fixture in non-interactive tests.

The previous single-monitor、optional post-capture Annotation and automatic Clipboard contracts are superseded.
