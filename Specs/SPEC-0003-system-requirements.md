# SPEC-0003 System Requirements

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `SPEC-0003` |
| Version | `1.3` |
| Status | `Accepted` |
| Last reviewed | `2026-07-27` |
| Sources | `PRD-0004`、`PRD-0005`、`PRD-0006` |

## 2. Shared System Requirements

### SR-001 — Resident Capture Entry and Exit

SnipPlus must support a manually started resident process and a user-controlled PrintScreen takeover setting. Disabled takeover must not intercept PrintScreen. Closing MainWindow with `X` exits the process、releases takeover and does not hide the application to the System Tray. Any explicit tray Exit action uses the same shutdown path.

Traces to：`FR-001`–`FR-005`、`FR-046`–`FR-047`、`NFR-020`、`NFR-023`。

### SR-002 — Frozen Virtual Desktop Session

One capture session must own a stable set of frozen frames for all connected displays and one Virtual Desktop coordinate snapshot until the session ends. Physical gaps between irregularly arranged displays contain no source pixels and render as transparent output pixels.

Traces to：`FR-006`–`FR-012`、`FR-048`、`NFR-004`–`NFR-013`。

### SR-003 — Selection Lifecycle

The system must distinguish initial selection、dragging、locked selection、selection move、selection resize、reselection、cancel and invalid-selection failure. Mouse release locks selection but does not commit output.

Traces to：`FR-008`–`FR-016`、`FR-036`–`FR-038`。

### SR-004 — Editing and Confirmation Lifecycle

After a valid locked selection, the editing／confirmation stage always exists. Annotation actions are optional; explicit Complete、Save or Cancel is required to leave the stage.

Traces to：`FR-013`–`FR-029`、`NFR-014`–`NFR-019`。

### SR-005 — Annotation Object Model

The system must support editable annotation objects anchored to Frozen Virtual Desktop coordinates、output clipping to the current selection and annotation-only Undo／Redo history.

Traces to：`FR-017`–`FR-029`。

### SR-006 — Clipboard Commit

Complete commits one rendered final image to Clipboard. The workflow does not end until Clipboard delivery succeeds.

Traces to：`FR-030`、`FR-034`、`FR-044`–`FR-045`。

### SR-007 — PNG Save Commit

Save opens Save As with Downloads as the initial folder、supports PNG、uses the timestamp filename proposal and submits the same rendered image to Clipboard after PNG creation. If Clipboard later fails, the PNG is retained and the workflow returns to Editing with an actionable Clipboard error.

Traces to：`FR-031`–`FR-034`、`FR-049`–`FR-050`。

### SR-008 — Cancel and Focus Restoration

Cancel produces no Clipboard or file output、closes all capture UI and restores the pre-capture foreground application without showing the SnipPlus main window.

Traces to：`FR-036`–`FR-043`、`NFR-020`–`NFR-023`。

### SR-009 — Failure Preservation

Recoverable render、save or Clipboard failure must preserve the locked selection and annotation state for retry or cancel. A previously created PNG is not deleted solely because later Clipboard delivery failed.

Traces to：`FR-034`、`FR-044`–`FR-045`、`FR-050`、`NFR-007`–`NFR-008`。

### SR-010 — Quantitative Performance Contract

Release verification must use the profile、latency、frame-time、output-time、memory and measurement protocol in `PRD-0006 §3`.

The system must:

- make all-display Selection interactive within p95 `500 ms` for Standard and p95 `1,000 ms` for Maximum profile;
- keep Selection and Annotation interaction at p95 `≤ 33 ms` frame time;
- show visible response to discrete input within p95 `100 ms`;
- meet the size-tiered Complete and Save targets;
- expose a non-blocking busy／progress state after `300 ms` when a commit remains in progress;
- satisfy the accepted idle、peak、cleanup and repeated-session memory limits.

Performance targets are release gates, not permission to terminate a valid operation merely because one individual run crosses a p95 threshold.

Traces to：`NFR-001`–`NFR-003`。

### SR-011 — Supported Display and Output Envelope

A v1 session is supported only when all of the following are true:

- `1`–`4` active logical desktop display surfaces;
- each display `≤ 7,680 × 4,320` physical pixels;
- total active source pixels `≤ 66,355,200`;
- Virtual Desktop bounding width and height each `≤ 16,384` physical pixels;
- final Selection width and height each `≤ 16,384` pixels;
- final Selection area `≤ 67,108,864` pixels.

Transparent topology gaps count toward final Selection area. Mirrored outputs resolving to one logical desktop surface count once.

If any limit is exceeded, the workflow must fail before interactive Selection、omit no displays、release partial resources、restore the previous work context and remain resident for a later request.

Traces to：`NFR-009`–`NFR-013`、`NFR-037`–`NFR-039`。

### SR-012 — Keyboard-only Editing and Annotation

After `SelectionLocked`, every required Editing and Annotation operation must be achievable without pointer input according to `PRD-0006 §9` and `SPEC-0009`.

The contract includes:

- F6 zone navigation;
- Tab／Shift+Tab control、object and handle navigation;
- deterministic keyboard object creation for every required tool;
- Arrow and Shift+Arrow `1`／`10` physical-pixel movement and resize;
- tool、Undo／Redo、Save、Complete and Delete commands;
- transient-state Esc handling before stable-Editing session cancellation;
- visible focus、High Contrast、200% scaling、Narrator-readable state and Chinese IME compatibility;
- no keyboard trap.

Initial crosshair Selection creation remains pointer-driven in v1; the complete keyboard-only standard begins at `SelectionLocked`.

Traces to：`NFR-029`–`NFR-031`。

## 3. Logical State Contract

| State | Meaning | Valid next states |
| --- | --- | --- |
| `ResidentReady` | SnipPlus is running and can accept an enabled entry. | `CaptureRequested`、`Exited` |
| `CaptureRequested` | PrintScreen or an authorized secondary entry starts a session. | `Freezing`、`Cancelled`、`Failed` |
| `Freezing` | All display frames and coordinate context are being frozen and capacity-validated. | `Selecting`、`Cancelled`、`Failed` |
| `Selecting` | Initial crosshair drag is active. | `SelectionLocked`、`Cancelled`、`Failed` |
| `SelectionLocked` | A valid region exists and can be moved、resized or replaced. | `Selecting`、`Editing`、`Cancelled`、`Failed` |
| `Editing` | Function bar is visible; annotations may be edited or skipped by pointer or keyboard. | `CommittingClipboard`、`Saving`、`Cancelled`、`Failed` |
| `CommittingClipboard` | Complete is rendering and publishing Clipboard. | `Completed`、`Editing` |
| `Saving` | Save As、PNG creation and Clipboard publishing are in progress. | `Completed`、`Editing` |
| `Completed` | Required output succeeded and cleanup／focus restoration can finish. | `ResidentReady` |
| `Cancelled` | User abandoned the session without output. | `ResidentReady` |
| `Failed` | An unsupported envelope、invalid session or non-recoverable resource failure requires cleanup. | `ResidentReady` |
| `Exited` | SnipPlus released takeover and terminated. | — |

A transition from `Saving` back to `Editing` may carry `RetainedFileReference` when PNG creation succeeded but Clipboard failed.

No platform adapter may mutate this shared state directly. `COMP-001` remains the sole authority.

## 4. Required Invariants

1. All frozen frames、selection geometry、annotations and output belong to one Session ID.
2. Selection and annotations use one Virtual Desktop coordinate snapshot.
3. Mouse release never writes Clipboard.
4. Annotation actions may be skipped; the confirmation stage may not be skipped.
5. Complete never creates a file.
6. Save uses PNG and also writes Clipboard.
7. Save As initially proposes Downloads but permits a user-selected destination.
8. Cancel never writes Clipboard or creates a file.
9. Recoverable output failure returns to `Editing` with state preserved.
10. A successfully written PNG remains at its selected destination after later Clipboard failure.
11. Physical non-display gaps render with transparent pixels.
12. Capture UI cleanup and focus restoration occur on Complete、successful Save、Cancel and terminal failure.
13. MainWindow `X` exits the application and releases takeover; it does not hide to tray.
14. Unsupported display or output capacity never produces partial capture.
15. Keyboard-only acceptance begins at `SelectionLocked` and covers every required v1 Editing／Annotation operation.
16. A progress indicator does not replace the requirement to keep the UI message loop responsive.
17. Normal product operation does not launch an external GUI fixture.

## 5. Acceptance Criteria

| ID | Criterion |
| --- | --- |
| `SPEC-0003-AC-001` | The state model distinguishes mouse release from explicit output commitment. |
| `SPEC-0003-AC-002` | One session can represent a rectangular selection crossing multiple displays. |
| `SPEC-0003-AC-003` | Annotation editing can be skipped without bypassing Complete／Save confirmation. |
| `SPEC-0003-AC-004` | Complete、Save and Cancel have distinct output effects. |
| `SPEC-0003-AC-005` | Recoverable output failure returns to Editing with state preserved. |
| `SPEC-0003-AC-006` | Focus restoration and SnipPlus-window exclusion are explicit system obligations. |
| `SPEC-0003-AC-007` | Closing MainWindow exits SnipPlus and releases PrintScreen takeover. |
| `SPEC-0003-AC-008` | Non-display gaps are transparent in the final rendered image. |
| `SPEC-0003-AC-009` | Clipboard failure after PNG success retains the PNG and returns to Editing. |
| `SPEC-0003-AC-010` | Standard and Maximum performance scenarios satisfy the p95、responsiveness and memory targets in PRD-0006. |
| `SPEC-0003-AC-011` | Configurations inside the complete display／output envelope are accepted; configurations exceeding any limit fail before Selection without partial capture. |
| `SPEC-0003-AC-012` | From SelectionLocked, the complete keyboard-only Editing／Annotation scenario finishes with the pointer unused. |

The previous optional-Annotation、immediate-Clipboard、close-to-tray、undefined capacity、undefined keyboard and unquantified performance contracts are superseded.