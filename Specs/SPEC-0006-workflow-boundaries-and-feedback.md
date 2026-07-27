# SPEC-0006 Workflow Boundaries and Feedback

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `SPEC-0006` |
| Feature ID | `FEAT-005` |
| Version | `1.3` |
| Status | `Accepted` |
| Last reviewed | `2026-07-27` |
| Normative sources | `PRD-0004`、`PRD-0005`、`PRD-0006`、`SPEC-0003`、`SPEC-0005`、`SPEC-0007`、`SPEC-0008`、`SPEC-0009` |

## 2. Outcome Classes

| Outcome | Meaning | Required next boundary |
| --- | --- | --- |
| `Completed` | Complete Clipboard succeeded, or Save PNG and Clipboard both succeeded. | Cleanup、restore focus、return to `ResidentReady`. |
| `UserCancelled` | User pressed Esc／Cancel from any accepted capture or Editing stage. | No output、cleanup、restore focus. |
| `SaveDialogCancelled` | User closed or cancelled Save As. | Return to Editing; preserve state. |
| `RecoverableFailure` | Render、save or Clipboard failed while the current Session remains usable. | Return to Editing; preserve Selection and annotations. |
| `RecoverableFailureWithRetainedFile` | PNG was created, Clipboard later failed and the PNG remains at the selected destination. | Return to Editing、preserve state、report retained file and Clipboard failure. |
| `TerminalFailure` | Unsupported capacity、frozen frames、display context or required Session resources are invalid. | Cleanup、restore focus、return to `ResidentReady`. |
| `StaleOutcome` | Async result belongs to an old Session or revision. | Ignore for state advancement and dispose its resources. |
| `ApplicationExit` | MainWindow `X` or an explicit Exit action terminates SnipPlus. | Release takeover、cleanup owned resources、terminate process. |

## 3. Cancel and Esc Rules

- Esc before Selection cancels the whole Session.
- Esc during Selection drag cancels the drag and the whole Session.
- Esc after Selection lock、while the function bar is visible or during Editing cancels the whole Session.
- V1 does not define a first-Esc transient-dismissal hierarchy for Annotation editors、pickers or popovers.
- Keyboard-only Annotation and non-PrintScreen tool／action shortcuts are deferred.
- Save As cancellation remains a distinct dialog outcome and returns to Editing without cancelling the capture Session.
- Cancel produces no Clipboard or file output.
- Cancel closes all overlays and function bars.
- Cancel restores the application active before PrintScreen.
- Cancel does not show the SnipPlus main window.

Esc remains a core capture-cancellation key and is not part of the deferred keyboard-only Annotation feature.

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

For render、save or Clipboard failure when the Session remains valid:

- remain or return to Editing;
- preserve frozen frames、Selection and Annotation document;
- identify the failed operation;
- allow pointer-based retry or Cancel;
- do not report success;
- do not close capture UI.

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

Unsupported-capacity feedback must reflect the accepted v1 envelope:

- up to `4` active logical displays;
- each display no larger than `3840 × 2160`;
- total source pixels no greater than `33,177,600`;
- Virtual Desktop and final-output allocation limits from `PRD-0006 §11`.

Application exit is distinct from capture Cancel:

- MainWindow `X` exits SnipPlus; it does not hide to tray;
- any explicit tray Exit action uses the same exit path;
- PrintScreen takeover is released before or as part of process termination;
- no hidden resident process remains.

## 7. Focus and Window Rules

- Pre-capture foreground context is recorded before SnipPlus capture UI becomes visible.
- Normal SnipPlus windows are excluded from the frozen source.
- Complete、successful Save、Cancel and terminal failure never automatically show MainWindow.
- Save As cancellation and recoverable failure return the user to the existing Editing context.
- Restoration failure must not be hidden, but it must not cause repeated focus stealing.
- A complete keyboard-only focus-navigation or keyboard-trap acceptance standard is deferred from v1.

## 8. Feedback Rules

- Success: no notification.
- Commit in progress beyond `300 ms`: non-blocking busy／progress state.
- Save As cancel: no error; return to Editing.
- Recoverable failure: show operation-specific actionable feedback within Editing.
- Clipboard failure after PNG success: state that the PNG was retained and Clipboard delivery failed.
- Unsupported capacity: state the relevant supported limit and return to `ResidentReady` after cleanup.
- Terminal failure: show concise failure feedback without exposing screenshot pixels、Clipboard payload or unredacted private window titles.

## 9. Acceptance Criteria

| ID | Criterion |
| --- | --- |
| `SPEC-0006-AC-001` | Esc cancels at pre-selection、drag、SelectionLocked and Editing stages without output. |
| `SPEC-0006-AC-002` | Save As cancellation returns to Editing without error. |
| `SPEC-0006-AC-003` | Recoverable output failure preserves Selection and annotations. |
| `SPEC-0006-AC-004` | Complete and Save cannot report success before their required Clipboard obligation succeeds. |
| `SPEC-0006-AC-005` | Complete、Save success and Cancel close all capture UI and request focus restoration. |
| `SPEC-0006-AC-006` | Successful completion produces no success notification. |
| `SPEC-0006-AC-007` | Stale outcomes cannot advance workflow state. |
| `SPEC-0006-AC-008` | Cleanup is safe when invoked more than once. |
| `SPEC-0006-AC-009` | Clipboard failure after PNG success retains the PNG and reports a partial outcome without reporting Save success. |
| `SPEC-0006-AC-010` | MainWindow `X` exits SnipPlus、releases takeover and does not hide to tray. |
| `SPEC-0006-AC-011` | A commit still running after `300 ms` shows a responsive busy／progress state. |
| `SPEC-0006-AC-012` | Unsupported capacity fails before Selection、cleans up and reports the accepted four-4K limit without partial capture. |
| `SPEC-0006-AC-013` | V1 acceptance does not require transient Esc hierarchy、keyboard-only Annotation or non-PrintScreen tool／action shortcuts. |

The previously accepted transient-Esc and complete keyboard-only focus workflow are superseded and deferred.