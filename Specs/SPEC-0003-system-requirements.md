# SPEC-0003 System Requirements

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `SPEC-0003` |
| Version | `1.4` |
| Status | `Accepted` |
| Last reviewed | `2026-07-27` |
| Sources | `PRD-0004`、`PRD-0005`、`PRD-0006` |

## 2. Shared System Requirements

### SR-001 — Resident Capture Entry and Exit

SnipPlus supports a manually started resident process and a user-controlled PrintScreen takeover setting. Disabled takeover does not intercept PrintScreen. MainWindow `X` exits the process、releases takeover and does not hide the application to the System Tray.

Traces to：`FR-001`–`FR-005`、`FR-046`–`FR-047`、`NFR-020`、`NFR-023`。

### SR-002 — Frozen Virtual Desktop Session

One capture session owns a stable set of frozen frames for all connected displays and one Virtual Desktop coordinate snapshot until the session ends. Physical gaps contain no source pixels and render as transparent output pixels.

Traces to：`FR-006`–`FR-012`、`FR-048`、`NFR-004`–`NFR-013`。

### SR-003 — Selection Lifecycle

The system distinguishes initial selection、dragging、locked selection、selection move、selection resize、reselection、cancel and invalid-selection failure. Mouse release locks selection but does not commit output.

### SR-004 — Editing and Confirmation Lifecycle

After a valid locked selection, the editing／confirmation stage always exists. Annotation actions are optional; explicit Complete、Save or Cancel is required to leave the stage.

### SR-005 — Annotation Object Model

The system supports editable pointer-driven annotation objects anchored to Frozen Virtual Desktop coordinates、output clipping and annotation-only Undo／Redo history.

### SR-006 — Clipboard Commit

Complete commits one rendered final image to Clipboard. The workflow does not end until Clipboard delivery succeeds.

### SR-007 — PNG Save Commit

Save opens Save As with Downloads as the initial folder、supports PNG、uses the timestamp filename proposal and submits the same rendered image to Clipboard after PNG creation. If Clipboard later fails, the PNG is retained and the workflow returns to Editing.

### SR-008 — Cancel and Focus Restoration

Cancel produces no Clipboard or file output、closes capture UI and restores the pre-capture foreground application without showing the SnipPlus main window.

### SR-009 — Failure Preservation

Recoverable render、save or Clipboard failure preserves locked selection and annotation state. A previously created PNG is not deleted solely because later Clipboard delivery failed.

### SR-010 — Quantitative Performance Contract

Release verification uses the profile、latency、frame-time、output-time、memory and measurement protocol in `PRD-0006 §3`.

Required release gates include:

- PrintScreen accepted → interactive all-display Selection: p95 `≤ 500 ms` for Owner Reference／Standard and p95 `≤ 1,000 ms` for Maximum;
- pointer-driven Selection and Annotation frame time: p95 `≤ 33 ms`;
- discrete pointer／UI action → visible response: p95 `≤ 100 ms`;
- size-tiered Complete and Save targets;
- non-blocking busy／progress state after `300 ms`;
- accepted idle、peak、cleanup and repeated-session memory limits.

Performance targets are release gates, not runtime cancellation timeouts.

### SR-011 — Supported Display and Output Envelope

A v1 session is supported only when all of the following are true:

- `1`–`4` active logical desktop display surfaces;
- each display `≤ 3840 × 2160` physical pixels;
- total active source pixels `≤ 33,177,600`;
- Virtual Desktop bounding width and height each `≤ 16,384` physical pixels;
- final Selection width and height each `≤ 16,384` pixels;
- final Selection area `≤ 67,108,864` pixels.

Transparent topology gaps count toward final Selection area. Mirrored outputs resolving to one logical desktop surface count once. An 8K display is outside v1.

If any limit is exceeded, the workflow fails before interactive Selection、omits no displays、releases partial resources、restores the previous work context and remains resident for a later request.

### SR-012 — Keyboard Scope Boundary

V1 does not require a keyboard-only Annotation workflow or non-PrintScreen tool／action shortcuts.

Deferred examples include:

- F6／Tab zone navigation as a complete product workflow;
- single-letter tool shortcuts;
- Ctrl-based Undo／Redo、Save or Complete shortcuts;
- Delete and Arrow-key object manipulation;
- keyboard-created default Annotation objects;
- a pointer-unused acceptance scenario after `SelectionLocked`.

PrintScreen remains the required global capture key. Esc remains the required capture-cancellation key according to the accepted workflow.

## 3. Logical State Contract

| State | Meaning | Valid next states |
| --- | --- | --- |
| `ResidentReady` | SnipPlus is running and can accept an enabled entry. | `CaptureRequested`、`Exited` |
| `CaptureRequested` | PrintScreen or an authorized secondary entry starts a session. | `Freezing`、`Cancelled`、`Failed` |
| `Freezing` | Display frames、coordinate context and capacity are established. | `Selecting`、`Cancelled`、`Failed` |
| `Selecting` | Initial crosshair drag is active. | `SelectionLocked`、`Cancelled`、`Failed` |
| `SelectionLocked` | A valid region exists and can be moved、resized or replaced. | `Selecting`、`Editing`、`Cancelled`、`Failed` |
| `Editing` | Function bar is visible; pointer-driven annotations may be edited or skipped. | `CommittingClipboard`、`Saving`、`Cancelled`、`Failed` |
| `CommittingClipboard` | Complete is rendering and publishing Clipboard. | `Completed`、`Editing` |
| `Saving` | Save As、PNG creation and Clipboard publishing are in progress. | `Completed`、`Editing` |
| `Completed` | Required output succeeded and cleanup／focus restoration can finish. | `ResidentReady` |
| `Cancelled` | User abandoned the session without output. | `ResidentReady` |
| `Failed` | Unsupported capacity or a non-recoverable failure requires cleanup. | `ResidentReady` |
| `Exited` | SnipPlus released takeover and terminated. | — |

A transition from `Saving` back to `Editing` may carry `RetainedFileReference`. Only `COMP-001` advances shared state.

## 4. Required Invariants

1. All frozen frames、selection geometry、annotations and output belong to one Session ID.
2. Selection and annotations use one Virtual Desktop coordinate snapshot.
3. Mouse release never writes Clipboard.
4. Annotation actions may be skipped; the confirmation stage may not be skipped.
5. Complete never creates a file.
6. Save uses PNG and also writes Clipboard.
7. Save As initially proposes Downloads but permits a user-selected destination.
8. Cancel never writes Clipboard or creates a file.
9. Recoverable output failure returns to Editing with state preserved.
10. A successfully written PNG remains after later Clipboard failure.
11. Physical non-display gaps render with transparent pixels.
12. Capture UI cleanup and focus restoration occur on Complete、successful Save、Cancel and terminal failure.
13. MainWindow `X` exits and releases takeover; it does not hide to tray.
14. Unsupported display or output capacity never produces partial capture.
15. V1 Annotation acceptance is pointer-driven; keyboard-only Annotation and non-PrintScreen shortcuts are deferred.
16. A progress indicator does not replace UI responsiveness.
17. Normal product operation does not launch an external GUI fixture.

## 5. Acceptance Criteria

| ID | Criterion |
| --- | --- |
| `SPEC-0003-AC-001` | The state model distinguishes mouse release from explicit output commitment. |
| `SPEC-0003-AC-002` | One session can represent a rectangular selection crossing multiple displays. |
| `SPEC-0003-AC-003` | Annotation editing can be skipped without bypassing Complete／Save confirmation. |
| `SPEC-0003-AC-004` | Complete、Save and Cancel have distinct output effects. |
| `SPEC-0003-AC-005` | Recoverable output failure returns to Editing with state preserved. |
| `SPEC-0003-AC-006` | Focus restoration and SnipPlus-window exclusion are explicit obligations. |
| `SPEC-0003-AC-007` | Closing MainWindow exits SnipPlus and releases PrintScreen takeover. |
| `SPEC-0003-AC-008` | Non-display gaps are transparent in the final rendered image. |
| `SPEC-0003-AC-009` | Clipboard failure after PNG success retains the PNG and returns to Editing. |
| `SPEC-0003-AC-010` | Owner Reference、Standard and Maximum scenarios satisfy PRD-0006 performance and memory targets. |
| `SPEC-0003-AC-011` | Configurations within four 4K displays and the allocation envelope are accepted; over-limit configurations fail before Selection without partial capture. |
| `SPEC-0003-AC-012` | V1 does not require keyboard-only Annotation or non-PrintScreen tool／action shortcuts; PrintScreen and Esc behavior remain verified. |

The previously accepted 8K-capable envelope and complete keyboard-only Annotation contract are superseded.