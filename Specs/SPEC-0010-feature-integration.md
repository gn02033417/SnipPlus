# SPEC-0010 Feature Integration

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `SPEC-0010` |
| Version | `1.3` |
| Status | `Accepted` |
| Last reviewed | `2026-07-27` |
| Covered features | `FEAT-001`–`FEAT-005` |
| Normative sources | `PRD-0004`、`PRD-0005`、`PRD-0006`、`SPEC-0003`–`SPEC-0009` |

## 2. Canonical Integrated Workflow

```text
ResidentReady
→ PrintScreen
→ Validate four-4K display and Virtual Desktop envelope
→ Freeze all supported displays
→ Present Virtual Desktop mask within performance target
→ Select across displays
→ Lock Selection
→ Show function bar
→ Adjust Selection and optionally edit annotations with pointer input
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

If Clipboard fails after PNG creation, retain the PNG and return to Editing with the current Selection and Annotation revision preserved.

### Cancel

```text
Esc or Cancel
→ No output
→ Cleanup
→ Restore previous focus
→ ResidentReady
```

Esc cancels before Selection、during drag and after Selection lock／during Editing. A transient first-Esc keyboard-editing hierarchy is not required in v1.

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
| PrintScreen entry、four-4K capacity validation、display freeze、Virtual Desktop Selection and application exit | `FEAT-001` |
| Function bar、pointer-driven Annotation objects and Undo／Redo | `FEAT-002` |
| Clipboard publication | `FEAT-003` |
| PNG Save As、Downloads default and file creation／retention | `FEAT-004` |
| Cancel、progress／error feedback、cleanup and focus restoration | `FEAT-005` |
| Shared state transitions | `COMP-001` only |

Each feature returns typed outcomes to `COMP-001`; no feature independently declares workflow completion.

## 4. Shared Session Context

Every participating feature receives the same immutable Session context containing at least:

- Session ID;
- pre-capture foreground context reference;
- Virtual Desktop bounds and origin;
- supported-capacity validation result;
- per-display frame identity、physical bounds and DPI mapping;
- current Selection revision;
- Annotation revision;
- cancellation context.

A mismatched Session ID or revision is stale and cannot advance the workflow.

## 5. Capacity and Performance Integration

The integrated workflow uses the accepted envelope from `PRD-0006 §11`:

- `1`–`4` active logical desktop surfaces;
- each display `≤ 3840 × 2160`;
- total active source pixels `≤ 33,177,600`;
- Virtual Desktop width and height each `≤ 16,384`;
- final Selection width and height each `≤ 16,384`;
- final Selection area `≤ 67,108,864`;
- an 8K display is outside v1.

No feature may silently relax、downscale or partially satisfy this envelope.

Mandatory runtime profiles include:

- **Owner Reference:** primary `2560 × 1440`、lower `1920 × 1080` at Windows scaling `150%`、left `2560 × 1440`;
- **Standard:** up to two displays and `16,588,800` total source pixels;
- **Maximum:** up to four displays、each no larger than `3840 × 2160`.

The integrated flow must meet:

- capture-start p95 `≤ 500 ms` Owner Reference／Standard and `≤ 1,000 ms` Maximum;
- pointer interaction frame time p95 `≤ 33 ms`;
- visible pointer／UI response p95 `≤ 100 ms`;
- Complete and Save output-size latency tiers;
- `300 ms` progress-state threshold;
- accepted memory and repeated-session cleanup limits.

Measurement uses `3` warm-up runs and at least `30` measured runs per scenario, reporting p50、p95 and maximum.

## 6. Selection and Annotation Integration

- Annotation objects use Frozen Virtual Desktop physical-pixel coordinates.
- Selection changes do not transform Annotation geometry.
- Final rendering intersects Annotation geometry with current Selection bounds.
- Reselection may reveal or clip existing objects without deleting them.
- Selection operations are not inserted into Annotation Undo／Redo history.
- Annotation actions are optional, but the function bar and explicit commitment are mandatory.
- Non-display gaps contain no source content and contribute transparent pixels to final output.
- Selection adjustment、Annotation creation and object editing are pointer-driven in v1.
- Function-bar Undo／Redo remains required.

## 7. Keyboard Boundary Integration

Required keys:

- PrintScreen starts capture when takeover is enabled.
- Esc cancels the current capture Session at the accepted workflow stages.

Deferred:

- keyboard-only Annotation;
- F6／Tab function-bar、canvas、object or handle traversal as a complete workflow;
- single-letter tool shortcuts;
- Ctrl-based Undo／Redo、Save or Complete shortcuts;
- Delete and Arrow-key object manipulation;
- keyboard-created Annotation objects;
- pointer-unused acceptance after `SelectionLocked`.

Normal text editing and Chinese IME remain supported. Required controls expose accessible names and state, and selected／error state is not communicated by color alone.

## 8. Output Integration

- Complete invokes final render and Clipboard only.
- Save invokes final render、Save As、PNG and Clipboard.
- Save As initially proposes Downloads and the timestamp filename; the user may change destination and filename.
- Complete and Save use the same alpha-preserving rendering semantics.
- Mouse release、Selection change and Annotation change do not invoke output.
- Save As cancellation returns to Editing.
- PNG save failure returns to Editing without Clipboard update.
- Clipboard failure after PNG success retains the PNG、returns to Editing and reports the partial outcome.
- A commit still running after `300 ms` displays non-blocking progress.
- Cleanup and focus restoration occur only after successful commitment、Cancel、unsupported-capacity failure or another terminal failure.

## 9. Residency、Exit、Concurrency and Stale Results

- Only one interactive capture Session may own overlays at a time.
- A second capture request while a Session is active must not silently replace the current Session.
- Late frame、render、save or Clipboard results from an older Session are ignored and cleaned up.
- Cancel invalidates future completion of that Session.
- MainWindow `X` exits SnipPlus and releases takeover; it does not transition to a hidden tray-resident mode.
- Any explicit tray Exit action uses the same exit path.
- Unsupported capacity cleans up and returns to `ResidentReady`; it does not disable future PrintScreen requests.

## 10. Acceptance Criteria

| ID | Criterion |
| --- | --- |
| `SPEC-0010-AC-001` | The integrated flow contains no path from mouse release directly to Clipboard. |
| `SPEC-0010-AC-002` | All displays、Selection、annotations and outputs share one Session context. |
| `SPEC-0010-AC-003` | Complete and Save have the exact output responsibilities defined by their owning Specs. |
| `SPEC-0010-AC-004` | Recoverable output failure retains Editing state and current revisions. |
| `SPEC-0010-AC-005` | Only COMP-001 advances shared workflow state. |
| `SPEC-0010-AC-006` | Stale asynchronous results cannot complete a newer or cancelled Session. |
| `SPEC-0010-AC-007` | Successful and cancelled Sessions restore the previous application without opening MainWindow. |
| `SPEC-0010-AC-008` | MainWindow `X` exits SnipPlus and releases PrintScreen takeover rather than hiding to tray. |
| `SPEC-0010-AC-009` | Non-display gaps are transparent in Complete and Save output. |
| `SPEC-0010-AC-010` | Clipboard failure after PNG success retains the PNG and returns to Editing. |
| `SPEC-0010-AC-011` | Every supported topology fits the four-4K envelope; unsupported capacity fails before Selection without partial capture. |
| `SPEC-0010-AC-012` | Owner Reference、Standard and Maximum measurements satisfy PRD-0006 quantitative targets. |
| `SPEC-0010-AC-013` | V1 requires PrintScreen and Esc behavior but does not require keyboard-only Annotation or non-PrintScreen tool／action shortcuts. |
| `SPEC-0010-AC-014` | Commit work exceeding `300 ms` shows responsive progress without changing success semantics. |

The previously accepted 8K-capable and complete keyboard-only integrated scenario is superseded and deferred.