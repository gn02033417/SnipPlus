# SPEC-0003 System Requirements

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `SPEC-0003` |
| Version | `1.1` |
| Status | `Accepted` |
| Last reviewed | `2026-07-27` |
| Sources | `PRD-0004`、`PRD-0005`、`PRD-0006` |

## 2. Shared System Requirements

### SR-001 — Resident Capture Entry

SnipPlus must support a manually started resident process and a user-controlled PrintScreen takeover setting. Disabled takeover must not intercept PrintScreen.

Traces to：`FR-001`–`FR-005`、`NFR-020`、`NFR-023`。

### SR-002 — Frozen Virtual Desktop Session

One capture session must own a stable set of frozen frames for all connected displays and one Virtual Desktop coordinate snapshot until the session ends.

Traces to：`FR-006`–`FR-012`、`NFR-004`–`NFR-013`。

### SR-003 — Selection Lifecycle

The system must distinguish initial selection、dragging、locked selection、selection move、selection resize、reselection、cancel and invalid-selection failure. Mouse release locks selection but does not commit output.

Traces to：`FR-008`–`FR-016`、`FR-036`–`FR-038`。

### SR-004 — Editing and Confirmation Lifecycle

After a valid locked selection, the editing／confirmation stage always exists. Annotation actions are optional; explicit Complete、Save or Cancel is required to leave the stage.

Traces to：`FR-013`–`FR-029`、`NFR-014`–`NFR-019`。

### SR-005 — Annotation Object Model

The system must support editable annotation objects anchored to Frozen Virtual Desktop coordinates, output clipping to the current selection and annotation-only Undo／Redo history.

Traces to：`FR-017`–`FR-029`。

### SR-006 — Clipboard Commit

Complete commits one rendered final image to Clipboard. The workflow does not end until Clipboard delivery succeeds.

Traces to：`FR-030`、`FR-034`、`FR-044`–`FR-045`。

### SR-007 — PNG Save Commit

Save opens Save As、supports PNG、uses the timestamp filename proposal and requires both file creation and Clipboard delivery before the session is considered successful.

Traces to：`FR-031`–`FR-034`。

### SR-008 — Cancel and Focus Restoration

Cancel produces no Clipboard or file output、closes all capture UI and restores the pre-capture foreground application without showing the SnipPlus main window.

Traces to：`FR-036`–`FR-043`、`NFR-020`–`NFR-023`。

### SR-009 — Failure Preservation

Recoverable render、save or Clipboard failure must preserve the locked selection and annotation state for retry or cancel.

Traces to：`FR-034`、`FR-044`–`FR-045`、`NFR-007`–`NFR-008`。

## 3. Logical State Contract

| State | Meaning | Valid next states |
| --- | --- | --- |
| `ResidentReady` | SnipPlus is running and can accept an enabled entry. | `CaptureRequested` |
| `CaptureRequested` | PrintScreen or an authorized secondary entry starts a session. | `Freezing`、`Cancelled`、`Failed` |
| `Freezing` | All display frames and coordinate context are being frozen. | `Selecting`、`Cancelled`、`Failed` |
| `Selecting` | Initial crosshair drag is active. | `SelectionLocked`、`Cancelled`、`Failed` |
| `SelectionLocked` | A valid region exists and can be moved、resized or replaced. | `Selecting`、`Editing`、`Cancelled`、`Failed` |
| `Editing` | Function bar is visible; annotations may be edited or skipped. | `CommittingClipboard`、`Saving`、`Cancelled`、`Failed` |
| `CommittingClipboard` | Complete is rendering and publishing Clipboard. | `Completed`、`Editing` |
| `Saving` | Save As、PNG creation and Clipboard publishing are in progress. | `Completed`、`Editing` |
| `Completed` | Required output succeeded and cleanup／focus restoration can finish. | `ResidentReady` |
| `Cancelled` | User abandoned the session without output. | `ResidentReady` |
| `Failed` | A non-recoverable session failure requires cleanup. | `ResidentReady` |

No platform adapter may mutate this shared state directly. `COMP-001` remains the sole authority.

## 4. Required Invariants

1. All frozen frames、selection geometry、annotations and output belong to one Session ID.
2. Selection and annotations use one Virtual Desktop coordinate snapshot.
3. Mouse release never writes Clipboard.
4. Annotation actions may be skipped; the confirmation stage may not be skipped.
5. Complete never creates a file.
6. Save uses PNG and also writes Clipboard.
7. Cancel never writes Clipboard or creates a file.
8. Recoverable output failure returns to `Editing` with state preserved.
9. Capture UI cleanup and focus restoration occur on Complete、successful Save、Cancel and terminal failure.
10. Normal product operation does not launch an external GUI fixture.

## 5. Acceptance Criteria

| ID | Criterion |
| --- | --- |
| `SPEC-0003-AC-001` | The state model distinguishes mouse release from explicit output commitment. |
| `SPEC-0003-AC-002` | One session can represent a rectangular selection crossing multiple displays. |
| `SPEC-0003-AC-003` | Annotation editing can be skipped without bypassing Complete／Save confirmation. |
| `SPEC-0003-AC-004` | Complete、Save and Cancel have distinct output effects. |
| `SPEC-0003-AC-005` | Recoverable output failure returns to Editing with state preserved. |
| `SPEC-0003-AC-006` | Focus restoration and SnipPlus-window exclusion are explicit system obligations. |

The previous optional-Annotation and immediate-Clipboard state contract is superseded.
