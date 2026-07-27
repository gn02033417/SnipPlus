# System Overview

狀態：`Accepted`

## 1. Product Architecture Intent

SnipPlus v1 is a resident Windows screenshot application. When the user enables PrintScreen takeover, one explicit PrintScreen action starts a capacity-validated all-display Frozen Virtual Desktop Session. The user creates a cross-monitor rectangle、enters mandatory Editing／confirmation and explicitly chooses Complete、Save or Cancel.

V1 Annotation and Selection adjustment are pointer-driven. PrintScreen entry and Esc cancellation are required; keyboard-only Annotation and non-PrintScreen tool／action shortcuts are deferred.

## 2. Runtime Responsibilities

| Boundary | Responsibility | Current project mapping |
| --- | --- | --- |
| App composition | WinUI host、overlay composition、function bar and adapter wiring | `SnipPlus.App` |
| Product workflow | Shared-state authority、Session progression、commitment routing and cancellation | `SnipPlus.Core` |
| Domain capabilities | Capacity、Virtual Desktop、Selection、Annotation、render intent、Clipboard／Output semantics and failure classification | `SnipPlus.Core` plus shared contracts |
| Contracts | Platform-neutral Session、Selection、Annotation、Image、Delivery、Failure and Outcome types | `SnipPlus.Contracts` |
| Windows integration | PrintScreen、display／DPI／focus、WGC、Win2D、Clipboard、Save As and PNG side effects | `SnipPlus.Windows` and App platform composition |

Current source projects exist, but the accepted v1 responsibilities are broader than the single-display prototype.

## 3. Accepted Runtime Flow

```text
ResidentReady
→ enabled PrintScreen accepted
→ record foreground context and display topology
→ validate four-4K support envelope
→ freeze one frame for every supported display
→ present one logical Frozen Virtual Desktop
→ cross-monitor Selection
→ SelectionLocked
→ Editing／confirmation function bar
→ optional pointer-driven Annotation actions
→ Complete OR Save OR Cancel
```

### Complete

```text
Freeze current revisions
→ render selected and annotated image with transparent gaps
→ show progress after 300 ms if still running
→ publish Clipboard
→ cleanup
→ restore previous work context
→ ResidentReady
```

### Save

```text
Freeze current revisions
→ render selected and annotated image with transparent gaps
→ Windows Save As in Downloads by default
→ write and retain PNG
→ publish the same image to Clipboard
→ cleanup only after Clipboard success
→ restore previous work context
→ ResidentReady
```

Clipboard failure after PNG success retains the PNG and returns to Editing.

### Cancel

```text
Esc or Cancel
→ no output
→ invalidate pending outcomes
→ cleanup capture UI and frames
→ restore previous work context
→ ResidentReady
```

## 4. Capacity and Quality Boundary

Supported source topology:

- `1`–`4` active logical display surfaces;
- each display `≤ 3840 × 2160`;
- total source pixels `≤ 33,177,600`;
- Virtual Desktop width and height each `≤ 16,384`;
- final Selection width and height each `≤ 16,384`;
- final Selection area `≤ 67,108,864` pixels;
- transparent gaps count toward final Selection area;
- 8K displays are outside v1.

Mandatory Owner Reference verification:

- primary `2560 × 1440`;
- lower `1920 × 1080` at Windows scaling `150%`;
- left `2560 × 1440`.

Performance measurement uses 3 warm-ups and at least 30 measured runs. The exact p95、memory and output-size targets are normative in `PRD-0006`.

## 5. Core Invariants

- `COMP-001` is the only shared Workflow State Authority.
- Capacity validation occurs before interactive Selection and final allocation.
- All frames、Selection、annotations and outputs belong to one Session ID and coordinate version.
- Mixed-DPI pointer input maps deterministically to Frozen Virtual Desktop physical coordinates.
- Mouse release never commits output.
- Editing／confirmation is mandatory; Annotation actions are optional.
- Complete creates no file.
- Save uses PNG and also writes Clipboard.
- Recoverable output failure preserves Editing state.
- Non-display gaps are transparent.
- Terminal failure and Cancel perform idempotent cleanup.
- SnipPlus windows are excluded from frozen source content.
- Successful completion is silent and restores the previous application.

## 6. Keyboard Boundary

Required:

- PrintScreen capture entry;
- Esc capture cancellation;
- ordinary text entry and Chinese IME;
- accessible names and non-color-only selected／error state.

Deferred:

- complete keyboard-only Annotation;
- F6／Tab zone and object traversal;
- tool、Ctrl、Delete and Arrow-key shortcuts;
- keyboard-created Annotation objects;
- pointer-unused Editing acceptance.

## 7. Data and Privacy

- Frozen frames and Annotation state are Session-local and disposable.
- The final image leaves memory only through explicit Clipboard or user-directed PNG Save.
- No cloud、sync、sharing or external processing is part of v1.
- Real screenshots、window titles and Clipboard payloads are not committed as Repository evidence.
- Normal product operation and non-interactive verification do not launch external GUI fixtures.

## 8. Current Implementation Relationship

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
- four-4K capacity policy;
- all-display topology and frame ownership;
- cross-monitor Selection;
- SelectionLocked and Editing states;
- pointer Selection adjustment;
- function bar and pointer-driven Annotation;
- Save As and file delivery;
- revision／stale-outcome protection;
- performance／memory evidence;
- foreground-context restoration.

Keyboard-only Annotation is deferred and is not a missing v1 capability.

Detailed status is maintained in `PRD/PRD-TRACEABILITY-MATRIX.md`.