# SPEC-0010 Feature Integration

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `SPEC-0010` |
| Version | `1.2` |
| Status | `Accepted` |
| Last reviewed | `2026-07-27` |
| Covered features | `FEAT-001`–`FEAT-005` |
| Normative sources | `PRD-0004`、`PRD-0005`、`PRD-0006`、`SPEC-0003`–`SPEC-0009` |

## 2. Canonical Integrated Workflow

```text
ResidentReady
→ PrintScreen
→ Validate display and Virtual Desktop envelope
→ Freeze all displays
→ Present Virtual Desktop mask within performance target
→ Select across displays
→ Lock Selection
→ Show function bar
→ Adjust Selection and optionally edit annotations by pointer or keyboard
→ Complete OR Save OR Cancel
```

### Complete

```text
Validate final output dimensions
→ Render current revision with transparent non-display gaps
→ show busy state after 300 ms if still running
→ Clipboard
→ Cleanup
→ Restore previous focus
→ ResidentReady
```

### Save

```text
Validate final output dimensions
→ Render current revision with transparent non-display gaps
→ Save As with Downloads as initial folder
→ PNG at user-selected destination
→ Retain PNG
→ show busy state after 300 ms if post-dialog work is still running
→ Clipboard
→ Cleanup only after Clipboard success
→ Restore previous focus
→ ResidentReady
```

If Clipboard fails after PNG creation, retain the PNG and return to Editing with the current revision and keyboard focus context preserved.

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

### Unsupported Capacity

```text
Capture request
→ topology or output envelope exceeds v1 limit
→ no partial Selection
→ cleanup partial resources
→ actionable limit feedback
→ restore previous focus
→ ResidentReady
```

## 3. Feature Ownership

| Concern | Owning feature |
| --- | --- |
| PrintScreen entry、capacity validation、display freeze、Virtual Desktop Selection and application-exit entry | `FEAT-001` |
| Function bar、keyboard focus model、annotation objects、Undo／Redo | `FEAT-002` |
| Clipboard publication | `FEAT-003` |
| PNG Save As、Downloads default and file creation／retention | `FEAT-004` |
| Cancel、progress／error feedback、cleanup and focus restoration | `FEAT-005` |
| Shared state transitions | `COMP-001` only |

Feature ownership does not permit independent workflow completion. Each feature returns typed outcomes to `COMP-001`.

## 4. Shared Session Context

Every participating feature receives the same immutable session context containing at least:

- Session ID;
- pre-capture foreground context reference;
- Virtual Desktop bounds and origin;
- supported-capacity validation result;
- per-display frame identity、physical bounds and DPI mapping;
- current Selection revision;
- Annotation revision;
- current keyboard focus context where required for recovery;
- cancellation context.

A result from a mismatched Session ID or revision is stale and cannot advance the workflow.

## 5. Capacity and Performance Integration

The integrated workflow uses the accepted envelope from `PRD-0006 §11`:

- `1`–`4` active logical desktop surfaces;
- each display `≤ 7,680 × 4,320`;
- total active source pixels `≤ 66,355,200`;
- Virtual Desktop width and height each `≤ 16,384`;
- final Selection width and height each `≤ 16,384`;
- final Selection area `≤ 67,108,864`.

No feature may silently relax、downscale or partially satisfy this envelope.

The integrated flow must meet:

- capture-start p95 `≤ 500 ms` Standard and `≤ 1,000 ms` Maximum;
- interaction frame time p95 `≤ 33 ms`;
- visible input response p95 `≤ 100 ms`;
- Complete and Save output-size latency tiers;
- `300 ms` progress-state threshold;
- accepted memory and repeated-session cleanup limits.

Measurement uses the release protocol in `PRD-0006 §3.4`.

## 6. Selection and Annotation Integration

- Annotation objects use Frozen Virtual Desktop physical-pixel coordinates.
- Selection changes do not transform Annotation geometry.
- Final rendering intersects Annotation geometry with current Selection bounds.
- Reselection may reveal or clip existing objects without deleting them.
- Selection operations are not inserted into Annotation Undo／Redo history.
- Annotation actions are optional, but the function bar and explicit commitment are mandatory.
- Non-display gaps contain no source content and contribute transparent pixels to final output.
- From `SelectionLocked`, every required v1 Editing and Annotation operation is available without pointer input.
- Keyboard movement and resize use deterministic `1`-pixel and `10`-pixel increments.
- Function-bar、canvas、object and handle focus follow `SPEC-0009`; dialogs and failures restore focus predictably.

## 7. Output Integration

- Complete invokes final render and Clipboard only.
- Save invokes final render、Save As、PNG and Clipboard.
- Save As initially proposes Downloads and the timestamp filename; the user may change both destination and filename.
- Complete and Save use the same rendering semantics and alpha-preserving result.
- Mouse release、Selection change and Annotation change do not invoke output.
- Save As cancellation returns to Editing.
- PNG save failure returns to Editing without Clipboard update.
- Clipboard failure after PNG success retains the PNG、returns to Editing and reports the partial outcome.
- A commit still running after `300 ms` displays non-blocking progress.
- Cleanup and focus restoration occur only after successful commitment、Cancel、unsupported-capacity failure or another terminal failure.

## 8. Residency、Exit、Concurrency and Stale Results

- Only one interactive capture session may own the overlays at a time unless a later product decision changes this.
- A second capture request while a session is active must not silently replace the current session; exact user feedback remains an implementation acceptance item.
- Late frame、render、save or Clipboard results from an older session are ignored and cleaned up.
- Cancel invalidates future completion of that session.
- Closing MainWindow with `X` exits SnipPlus and releases takeover; it does not transition to a hidden tray-resident mode.
- If a System Tray surface exists, its explicit Exit action uses the same exit path.
- An unsupported-capacity request cleans up and returns to `ResidentReady`; it does not disable future PrintScreen requests.

## 9. Keyboard-only Integrated Scenario

Acceptance begins with a valid `SelectionLocked` state and the pointer unused afterward.

The scenario must complete:

1. F6 and Tab navigation between function bar、canvas、objects and handles.
2. Keyboard creation of every required v1 Annotation tool.
3. Object selection、movement、resize、style／mode editing、delete、Undo and Redo.
4. Text entry with Chinese IME and no accidental tool-shortcut activation.
5. Save As cancellation and focus return.
6. Complete、Save and Cancel invocation.
7. Transient-state Esc dismissal followed by stable-Editing Esc cancellation.
8. Visible focus、High Contrast、200% scaling and Narrator-readable names／state.
9. No keyboard trap.

Initial crosshair region creation remains outside this keyboard-only scenario.

## 10. Acceptance Criteria

| ID | Criterion |
| --- | --- |
| `SPEC-0010-AC-001` | The integrated flow contains no path from mouse release directly to Clipboard. |
| `SPEC-0010-AC-002` | All displays、Selection、annotations and outputs share one session context. |
| `SPEC-0010-AC-003` | Complete and Save have the exact output responsibilities defined by their owning Specs. |
| `SPEC-0010-AC-004` | Recoverable output failure retains Editing state、current revision and applicable keyboard focus context. |
| `SPEC-0010-AC-005` | Only COMP-001 advances shared workflow state. |
| `SPEC-0010-AC-006` | Stale asynchronous results cannot complete a newer or cancelled session. |
| `SPEC-0010-AC-007` | Successful and cancelled sessions restore the previous application without opening the SnipPlus main window. |
| `SPEC-0010-AC-008` | MainWindow `X` exits SnipPlus and releases PrintScreen takeover rather than hiding to tray. |
| `SPEC-0010-AC-009` | Non-display gaps are transparent in Complete and Save output. |
| `SPEC-0010-AC-010` | Clipboard failure after PNG success retains the PNG and returns to Editing. |
| `SPEC-0010-AC-011` | Every supported topology and output fits the accepted envelope; unsupported capacity fails before Selection without partial capture. |
| `SPEC-0010-AC-012` | Integrated capture、interaction、output and memory measurements satisfy PRD-0006 quantitative targets. |
| `SPEC-0010-AC-013` | The keyboard-only scenario completes from SelectionLocked with the pointer unused. |
| `SPEC-0010-AC-014` | Commit work exceeding `300 ms` shows responsive progress without changing success semantics. |

The previous integration sequence with undefined capacity、performance or keyboard acceptance is superseded.