# SPEC-0006 Workflow Boundaries and Feedback

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `SPEC-0006` |
| Feature ID | `FEAT-005` |
| Version | `1.1` |
| Status | `Accepted` |
| Last reviewed | `2026-07-27` |
| Normative sources | `PRD-0004`、`PRD-0005`、`PRD-0006`、`SPEC-0003`、`SPEC-0005`、`SPEC-0007`、`SPEC-0008`、`SPEC-0009` |

## 2. Outcome Classes

| Outcome | Meaning | Required next boundary |
| --- | --- | --- |
| `Completed` | Complete Clipboard succeeded, or Save PNG and Clipboard both succeeded. | Cleanup、restore focus、return to `ResidentReady`. |
| `UserCancelled` | User pressed Esc／Cancel or cancelled the whole session. | No output、cleanup、restore focus. |
| `SaveDialogCancelled` | User closed or cancelled Save As. | Return to Editing; preserve state. |
| `RecoverableFailure` | Render、save or Clipboard failed while the current session remains usable. | Return to Editing; preserve selection and annotations. |
| `RecoverableFailureWithRetainedFile` | PNG was created, Clipboard later failed and the PNG remains at the selected destination. | Return to Editing、preserve state、report retained file and Clipboard failure. |
| `TerminalFailure` | Frozen frames、display context or required session resources are invalid. | Cleanup、restore focus、return to `ResidentReady`. |
| `StaleOutcome` | Async result belongs to an old session or revision. | Ignore for state advancement and dispose its resources. |
| `ApplicationExit` | MainWindow `X` or an explicit Exit action terminates SnipPlus. | Release takeover、cleanup owned resources、terminate process. |

## 3. Cancel Rules

- Esc before selection cancels the whole session.
- Esc during drag cancels the drag and the whole session.
- Esc in locked-selection or Editing cancels the whole session.
- Cancel produces no Clipboard or file output.
- Cancel closes all overlays and function bars.
- Cancel restores the application active before PrintScreen.
- Cancel does not show the SnipPlus main window.

## 4. Success Rules

- Complete is successful only after Clipboard publication succeeds.
- Save is successful only after PNG creation and Clipboard publication both succeed.
- Successful completion is silent: no success Toast、Dialog or forced foreground SnipPlus window.
- Cleanup and focus restoration are part of success, not optional follow-up work.

## 5. Recoverable Failure Rules

For render、save or Clipboard failure when the session remains valid:

- remain or return to Editing;
- preserve frozen frames、selection and annotation document;
- identify the failed operation;
- allow retry or Cancel;
- do not report success;
- do not close capture UI.

If Clipboard fails after PNG creation:

- retain the PNG at the destination selected in Save As;
- do not attempt rollback or deletion;
- report that file saving succeeded but Clipboard delivery failed;
- return to Editing with the current selection and annotations preserved.

Save As cancellation is not an error and must not show failure feedback.

## 6. Terminal Failure and Application Exit

Terminal failure includes an invalid or lost frozen display context、unrecoverable capture resource loss or a state-integrity violation.

The system must:

- invalidate pending work;
- close capture overlays and function bars;
- dispose resources idempotently;
- restore the previous work context where permitted;
- provide concise failure feedback without exposing private screen content;
- return to a state that can accept a new capture request.

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

## 8. Feedback Rules

- Success: no notification.
- Save As cancel: no error; return to Editing.
- Recoverable failure: show operation-specific actionable feedback within the editing context.
- Clipboard failure after PNG success: state that the PNG was retained and Clipboard delivery failed.
- Terminal failure: show concise failure feedback after cleanup or through the resident application surface without reopening the main window solely for success／cancel.
- Diagnostic details must not contain screenshot pixels、Clipboard payload or unredacted private window titles.

## 9. Acceptance Criteria

| ID | Criterion |
| --- | --- |
| `SPEC-0006-AC-001` | Esc cancels at pre-selection、drag and Editing stages without output. |
| `SPEC-0006-AC-002` | Save As cancellation returns to Editing without error. |
| `SPEC-0006-AC-003` | Recoverable output failure preserves selection and annotations. |
| `SPEC-0006-AC-004` | Complete and Save cannot report success before their required Clipboard obligation succeeds. |
| `SPEC-0006-AC-005` | Complete、Save success and Cancel close all capture UI and request focus restoration. |
| `SPEC-0006-AC-006` | Successful completion produces no success notification. |
| `SPEC-0006-AC-007` | Stale outcomes cannot advance workflow state. |
| `SPEC-0006-AC-008` | Cleanup is safe when invoked more than once. |
| `SPEC-0006-AC-009` | Clipboard failure after PNG success retains the PNG and reports a partial outcome without reporting Save success. |
| `SPEC-0006-AC-010` | MainWindow `X` exits SnipPlus、releases takeover and does not hide to tray. |

The previous generic Cancel／Error boundary with undefined file retention and exit side effects is superseded.