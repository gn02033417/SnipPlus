# SPEC-0010 Feature Integration

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `SPEC-0010` |
| Version | `1.1` |
| Status | `Accepted` |
| Last reviewed | `2026-07-27` |
| Covered features | `FEAT-001`–`FEAT-005` |
| Normative sources | `PRD-0004`、`PRD-0005`、`PRD-0006`、`SPEC-0003`–`SPEC-0009` |

## 2. Canonical Integrated Workflow

```text
ResidentReady
→ PrintScreen
→ Freeze all displays
→ Present Virtual Desktop mask
→ Select across displays
→ Lock selection
→ Show function bar
→ Adjust selection and optionally edit annotations
→ Complete OR Save OR Cancel
```

### Complete

```text
Render current revision with transparent non-display gaps
→ Clipboard
→ Cleanup
→ Restore previous focus
→ ResidentReady
```

### Save

```text
Render current revision with transparent non-display gaps
→ Save As with Downloads as initial folder
→ PNG at user-selected destination
→ Retain PNG
→ Clipboard
→ Cleanup only after Clipboard success
→ Restore previous focus
→ ResidentReady
```

If Clipboard fails after PNG creation, retain the PNG and return to Editing with the current revision preserved.

### Cancel

```text
No output
→ Cleanup
→ Restore previous focus
→ ResidentReady
```

### Application Exit

```text
MainWindow X or explicit Exit
→ Release PrintScreen takeover
→ Cleanup owned application resources
→ Terminate SnipPlus
```

MainWindow `X` does not hide the application to the System Tray.

## 3. Feature Ownership

| Concern | Owning feature |
| --- | --- |
| PrintScreen entry、display freeze、Virtual Desktop selection and application-exit entry | `FEAT-001` |
| Function bar、annotation objects、Undo／Redo | `FEAT-002` |
| Clipboard publication | `FEAT-003` |
| PNG Save As、Downloads default and file creation／retention | `FEAT-004` |
| Cancel、error preservation、cleanup and focus restoration | `FEAT-005` |
| Shared state transitions | `COMP-001` only |

Feature ownership does not permit independent workflow completion. Each feature returns outcomes to `COMP-001`.

## 4. Shared Session Context

Every participating feature receives the same immutable session context containing at least:

- Session ID;
- pre-capture foreground context reference;
- Virtual Desktop bounds and origin;
- per-display frame identity、physical bounds and DPI mapping;
- current selection revision;
- annotation revision;
- cancellation context.

A result from a mismatched Session ID or revision is stale and cannot advance the workflow.

## 5. Selection and Annotation Integration

- Annotation objects use Frozen Virtual Desktop coordinates.
- Selection changes do not transform annotation geometry.
- Final rendering intersects annotation geometry with current selection bounds.
- Reselection may reveal or clip existing objects without deleting them.
- Selection operations are not inserted into annotation Undo／Redo history.
- Annotation actions are optional, but the function bar and explicit commitment are mandatory.
- Non-display gaps contain no source content and contribute transparent pixels to final output.

## 6. Output Integration

- Complete invokes final render and Clipboard only.
- Save invokes final render、Save As、PNG and Clipboard.
- Save As initially proposes Downloads and the timestamp filename; the user may change both destination and filename.
- Complete and Save must use the same rendering semantics.
- Mouse release、selection change and annotation change do not invoke output.
- Save As cancellation returns to Editing.
- PNG save failure returns to Editing without Clipboard update.
- Clipboard failure after PNG success retains the PNG、returns to Editing and reports the partial outcome.
- Cleanup and focus restoration occur only after successful commitment、Cancel or terminal failure.

## 7. Residency、Exit、Concurrency and Stale Results

- Only one interactive capture session may own the overlays at a time unless a later product decision changes this.
- A second capture request while a session is active must not silently replace the current session; exact user feedback remains an implementation acceptance item.
- Late frame、render、save or Clipboard results from an older session are ignored and cleaned up.
- Cancel invalidates future completion of that session.
- Closing MainWindow with `X` exits SnipPlus and releases takeover; it does not transition to a hidden tray-resident mode.
- If a System Tray surface exists, its explicit Exit action uses the same exit path.

## 8. Acceptance Criteria

| ID | Criterion |
| --- | --- |
| `SPEC-0010-AC-001` | The integrated flow contains no path from mouse release directly to Clipboard. |
| `SPEC-0010-AC-002` | All displays、selection、annotations and outputs share one session context. |
| `SPEC-0010-AC-003` | Complete and Save have the exact output responsibilities defined by their owning Specs. |
| `SPEC-0010-AC-004` | Recoverable output failure retains Editing state and current revision. |
| `SPEC-0010-AC-005` | Only COMP-001 advances shared workflow state. |
| `SPEC-0010-AC-006` | Stale asynchronous results cannot complete a newer or cancelled session. |
| `SPEC-0010-AC-007` | Successful and cancelled sessions restore the previous application without opening the SnipPlus main window. |
| `SPEC-0010-AC-008` | MainWindow `X` exits SnipPlus and releases PrintScreen takeover rather than hiding to tray. |
| `SPEC-0010-AC-009` | Non-display gaps are transparent in Complete and Save output. |
| `SPEC-0010-AC-010` | Clipboard failure after PNG success retains the PNG and returns to Editing. |

The previous integration sequence that treated Annotation as an optional post-capture branch、Clipboard as an automatic next state or PNG rollback as unresolved is superseded.