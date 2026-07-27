# System Overview

狀態：`Accepted`

## 1. Product architecture intent

SnipPlus v1 is a resident Windows screenshot application. When the user enables PrintScreen takeover, one explicit PrintScreen action starts an all-display frozen capture session. The user selects a cross-monitor rectangle、enters a mandatory editing／confirmation stage and explicitly chooses Complete、Save or Cancel.

The architecture separates product workflow from WinUI and Windows side effects so that session、selection、annotation、commitment and failure rules remain deterministic and testable.

## 2. Runtime responsibilities

| Boundary | Responsibility | Current project mapping |
| --- | --- | --- |
| App composition | WinUI host、window／overlay composition、function-bar presentation and adapter wiring | `SnipPlus.App` |
| Product workflow | Shared-state authority、session progression、commitment routing and cancellation | `SnipPlus.Core` |
| Domain capabilities | Virtual Desktop、selection、annotation、render intent、Clipboard／Output semantics and failure classification | `SnipPlus.Core` plus shared contracts |
| Contracts | Platform-neutral Session、Selection、Annotation、Image、Delivery、Failure and Outcome types | `SnipPlus.Contracts` |
| Windows integration | PrintScreen、display／DPI／focus、WGC、Win2D、Clipboard、Save As and PNG side effects | `SnipPlus.Windows` and App platform composition |

Current source projects already exist. The accepted v1 responsibilities are broader than the current single-display prototype and require conformance correction.

## 3. Accepted runtime flow

```text
ResidentReady
→ PrintScreen takeover accepts explicit request
→ Record foreground context and display topology
→ Freeze one frame for every connected display
→ Present one logical Frozen Virtual Desktop
→ Cross-monitor Selection
→ SelectionLocked
→ Editing／confirmation function bar
→ optional Annotation actions
→ Complete OR Save OR Cancel
```

### Complete

```text
Freeze current revisions
→ render selected and annotated image
→ publish Clipboard
→ cleanup
→ restore previous work context
→ ResidentReady
```

### Save

```text
Freeze current revisions
→ render selected and annotated image
→ Windows Save As
→ write PNG
→ publish the same image to Clipboard
→ cleanup
→ restore previous work context
→ ResidentReady
```

### Cancel

```text
No output
→ invalidate pending outcomes
→ cleanup all capture UI and frames
→ restore previous work context
→ ResidentReady
```

## 4. Core invariants

- `COMP-001` is the only shared Workflow State Authority.
- All display frames、selection、annotations and outputs belong to one Session ID and coordinate version.
- Selection and annotations use Frozen Virtual Desktop physical coordinates; mixed-DPI input is mapped deterministically.
- Mouse release never commits output.
- Editing／confirmation is mandatory; creating annotations is optional.
- Complete creates no file.
- Save uses PNG and also writes Clipboard.
- Recoverable output failure preserves Editing state.
- Terminal failure and Cancel perform idempotent cleanup.
- SnipPlus normal windows are excluded from frozen source content.
- Successful completion is silent and restores the previous application.

## 5. Data and privacy

- Frozen display frames and annotation state are session-local and disposable.
- The final image leaves memory only through explicit Clipboard or user-directed PNG Save.
- No cloud、sync、sharing or external processing is part of v1.
- Real screenshots、window titles and Clipboard payloads are not committed as repository evidence.
- Normal product operation and non-interactive verification do not launch external GUI fixtures.

## 6. Current implementation relationship

Reusable today:

- one-display WGC acquisition;
- one immutable frame and same-frame crop;
- one-display mask presentation;
- canonical SoftwareBitmap image pipeline;
- PNG encoding;
- Clipboard delivery with bounded retry;
- low-level state、coordinate、image and adapter tests.

Not yet conforming:

- resident lifecycle and PrintScreen takeover;
- all-display topology and frame ownership;
- cross-monitor Selection;
- SelectionLocked and Editing states;
- selection adjustment;
- function bar and Annotation;
- Save As and file delivery;
- revision／stale-outcome protection;
- foreground-context restoration.

Detailed status is maintained in `PRD/PRD-TRACEABILITY-MATRIX.md`.

## 7. Open decisions

- Representation of gaps between irregularly arranged displays.
- Exact System Tray and MainWindow close-button behavior.
- PNG retention when Clipboard fails after file creation.
- Final keyboard-only Annotation acceptance standard.
- Quantitative performance targets after measurement.
