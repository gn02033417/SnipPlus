# SPEC-0006 Workflow Boundaries and Feedback

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `SPEC-0006` |
| Feature ID | `FEAT-005` |
| Version | `1.2` |
| Status | `Accepted` |
| Last reviewed | `2026-07-27` |
| Normative sources | `PRD-0004`、`PRD-0005`、`PRD-0006`、`SPEC-0003`、`SPEC-0005`、`SPEC-0007`、`SPEC-0008`、`SPEC-0009` |

## 2. Outcome Classes

| Outcome | Meaning | Required next boundary |
| --- | --- | --- |
| `Completed` | Complete Clipboard succeeded, or Save PNG and Clipboard both succeeded. | Cleanup、restore focus、return to `ResidentReady`. |
| `UserCancelled` | User pressed Esc／Cancel from a stable session state or cancelled the whole session. | No output、cleanup、restore focus. |
| `TransientEditingDismissed` | Esc closed a picker、popover、text editor or uncommitted creation operation. | Return to stable Editing; preserve committed state. |
| `SaveDialogCancelled` | User closed or cancelled Save As. | Return to Editing; preserve state. |
| `RecoverableFailure` | Render、save or Clipboard failed while the current session remains usable. | Return to Editing; preserve Selection and annotations. |
| `RecoverableFailureWithRetainedFile` | PNG was created, Clipboard later failed and the PNG remains at the selected destination. | Return to Editing、preserve state、report retained file and Clipboard failure. |
| `TerminalFailure` | Unsupported capacity、frozen frames、display context or required session resources are invalid. | Cleanup、restore focus、return to `ResidentReady`. |
| `StaleOutcome` | Async result belongs to an old session or revision. | Ignore for state advancement and dispose its resources. |
| `ApplicationExit` | MainWindow `X` or an explicit Exit action terminates SnipPlus. | Release takeover、cleanup owned resources、terminate process. |

## 3. Cancel and Esc Rules

- Esc before Selection cancels the whole session.
- Esc during drag cancels the drag and the whole session.
- Esc from stable `SelectionLocked` or stable Editing cancels the whole session.
- When a picker、popover、text editor or uncommitted object-creation operation owns focus, the first Esc closes or abandons that transient state and returns to stable Editing without cancelling the session.
- A subsequent Esc from stable Editing cancels the session.
- Cancel produces no Clipboard or file output.
- Cancel closes all overlays and function bars.
- Cancel restores the application active before PrintScreen.
- Cancel does not show the SnipPlus main window.

## 4. Success and Progress Rules

- Complete is successful only after Clipboard publication succeeds.
- Save is successful only after PNG creation and Clipboard publication both succeed.
- Successful completion is silent: no success Toast、Dialog or forced foreground SnipPlus window.
- Cleanup and focus restoration are part of success, not optional follow-up work.
- If a Complete or Save commit remains in progress for `300 ms`, show a non-blocking busy／progress state inside the Editing context.
- The progress state must not prevent Cancel where cancellation remains supported.
- The progress state disappears on success、recoverable failure、Cancel or terminal failure.
- Performance acceptance uses `PRD-0006 §3`; a progress indicator does not make an otherwise frozen UI conforming.

## 5. Recoverable Failure Rules

For render、save or Clipboard failure when the session remains valid:

- remain or return to Editing;
- preserve frozen frames、Selection and Annotation document;
- identify the failed operation;
- allow retry or Cancel;
- do not report success;
- do not close capture UI;
- restore keyboard focus to the operation or object that initiated the failed command where possible.

If Clipboard fails after PNG creation:

- retain the PNG at the destination selected in Save As;
- do not attempt rollback or deletion;
- report that file saving succeeded but Clipboard delivery failed;
- return to Editing with the current Selection and annotations preserved.

Save As cancellation is not an error and must not show failure feedback.

## 6. Terminal Failure and Application Exit

Terminal failure includes:

- an invalid or lost frozen display context;
- unsupported display、Virtual Desktop or output capacity;
- unrecoverable capture resource loss;
- a state-integrity violation.

The system must:

- invalidate pending work;
- close capture overlays and function bars;
- dispose resources idempotently;
- restore the previous work context where permitted;
- provide concise failure feedback without exposing private screen content;
- return to a state that can accept a new capture request.

Unsupported-capacity feedback must identify the supported v1 limits without listing private display identifiers.

Application exit is distinct from capture Cancel:

- MainWindow `X` exits SnipPlus; it does not hide to tray;
- any explicit tray Exit action uses the same exit path;
- PrintScreen takeover is released before or as part of process termination;
- no hidden resident process remains.

## 7. Focus and Window Rules

- Pre-capture foreground context is recorded before SnipPlus capture UI becomes visible.
- Normal SnipPlus windows are excluded from the frozen source.
- Complete、successful Save、Cancel and terminal failure never automatically show the main window.
- Restoration failure must not be hidden, but it must not cause repeated focus stealing.
- Function-bar、canvas、object、handle、picker、text-editor and Save As transitions must return keyboard focus predictably and must not create a keyboard trap.

## 8. Feedback Rules

- Success: no notification.
- Commit in progress beyond `300 ms`: non-blocking busy／progress state.
- Save As cancel: no error; return to Editing.
- Transient Esc dismissal: no error; return to stable Editing.
- Recoverable failure: show operation-specific actionable feedback within the Editing context.
- Clipboard failure after PNG success: state that the PNG was retained and Clipboard delivery failed.
- Unsupported capacity: state the relevant display／Virtual Desktop／output limit and return to `ResidentReady` after cleanup.
- Terminal failure: show concise failure feedback after cleanup or through the resident application surface without reopening the main window solely for success／cancel.
- Diagnostic details must not contain screenshot pixels、Clipboard payload or unredacted private window titles.

## 9. Acceptance Criteria

| ID | Criterion |
| --- | --- |
| `SPEC-0006-AC-001` | Esc cancels at pre-selection、drag and stable Editing stages without output. |
| `SPEC-0006-AC-002` | Save As cancellation returns to Editing without error. |
| `SPEC-0006-AC-003` | Recoverable output failure preserves Selection and annotations. |
| `SPEC-0006-AC-004` | Complete and Save cannot report success before their required Clipboard obligation succeeds. |
| `SPEC-0006-AC-005` | Complete、Save success and Cancel close all capture UI and request focus restoration. |
| `SPEC-0006-AC-006` | Successful completion produces no success notification. |
| `SPEC-0006-AC-007` | Stale outcomes cannot advance workflow state. |
| `SPEC-0006-AC-008` | Cleanup is safe when invoked more than once. |
| `SPEC-0006-AC-009` | Clipboard failure after PNG success retains the PNG and reports a partial outcome without reporting Save success. |
| `SPEC-0006-AC-010` | MainWindow `X` exits SnipPlus、releases takeover and does not hide to tray. |
| `SPEC-0006-AC-011` | The first Esc closes transient Editing state; Esc from stable Editing cancels the session. |
| `SPEC-0006-AC-012` | A commit still running after `300 ms` shows a responsive busy／progress state. |
| `SPEC-0006-AC-013` | Unsupported capacity fails before Selection、cleans up and reports the supported limit without partial capture. |
| `SPEC-0006-AC-014` | Dialog、picker、text-editor and failure transitions restore keyboard focus without a keyboard trap. |

The previous generic Cancel／Error boundary、undefined Esc hierarchy、undefined progress feedback and unresolved capacity behavior are superseded.