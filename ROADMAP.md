# ROADMAP

狀態：`Accepted`

## Completed Foundations

- Accepted current PRD and Specification baseline.
- Accepted Architecture principles、layers、modules、components and interactions.
- ADR-0002 through ADR-0007.
- Implementation Contracts v2.3.
- Project Structure and toolchain baseline.
- Solution、four source projects and three test projects.
- Reusable one-display WGC、same-frame crop、BGRA8 image、PNG encoder、Win2D presentation and Clipboard retry foundations.
- Requirements-to-code conformance audit.
- Product decisions for direct exit、transparent gaps、Downloads Save As and retained PNG.
- Quantitative performance、memory and four-4K capacity baseline.
- Keyboard-only Annotation and non-PrintScreen shortcuts explicitly deferred.
- Repetitive prerequisite／readiness／authorization／closure document pattern stopped.

## Current Phase — v1 Workflow Conformance Correction

狀態：`Active when explicitly authorized`

Implementation must proceed in this order:

1. Resident lifecycle、direct MainWindow exit and user-controlled PrintScreen takeover setting.
2. PrintScreen entry integrated with `COMP-001`.
3. Four-4K capacity policy、Frozen Virtual Desktop Session and per-display frame ownership.
4. All-display frozen presentation、crosshair and cross-monitor initial Selection.
5. Locked Selection、pointer move、edge／corner resize and reselection.
6. Accepted state graph including `ResidentReady`、`Freezing`、`Selecting`、`SelectionLocked`、`Editing`、`CommittingClipboard` and `Saving`.
7. Function bar、Complete／Save／Cancel、progress and foreground-context restoration.
8. Annotation document、required pointer-driven tools and object editing.
9. Annotation-only Undo／Redo、Virtual Desktop anchoring and Selection clipping.
10. Complete final render、capacity revalidation、transparent gap output and Clipboard.
11. Windows Save As with Downloads default、PNG file delivery、same-result Clipboard and retained-file outcome.
12. Recoverable failure preservation、stale-session／revision protection、performance／memory evidence and required accessibility.
13. Explicitly authorized Owner Reference、Standard and Maximum runtime verification.

### Phase Rules

- Do not continue from the obsolete mouse-release-to-Clipboard sequence.
- Reuse technical foundations only after matrix review.
- Each completed slice updates code、tests、CHANGELOG and `PRD-TRACEABILITY-MATRIX-001`.
- Passing build or test counts do not prove user-visible conformance.
- Do not begin a later step while an earlier prerequisite remains `Missing` or `Incorrect`.
- Do not silently add deferred keyboard shortcuts or keyboard-only Annotation.

## Required v1 Capabilities

- Manual startup and residency while running.
- MainWindow `X` directly exits and releases PrintScreen takeover.
- User-controlled PrintScreen takeover.
- Capacity validation before Selection and typed over-limit failure.
- All-display Frozen Virtual Desktop.
- Cross-monitor rectangular Selection.
- Transparent output for physical non-display gaps.
- Pointer-based Selection movement、edge／corner resize and reselection.
- Mandatory Editing／confirmation function bar.
- Rectangle、Arrow／Line、Highlighter、Text、Mosaic／Blur and Numbered Marker.
- Pointer-based Annotation selection、movement、resize、style changes and delete.
- Function-bar Undo and Redo.
- Complete to Clipboard with progress after `300 ms`.
- Save As initially opens Downloads、saves PNG and writes Clipboard.
- Retain PNG when later Clipboard delivery fails.
- Cancel、recoverable failure preservation、cleanup and focus restoration.
- PrintScreen entry and Esc cancellation.
- Accessible control names and non-color-only selected／error state.

## Accepted Quality Gates

### Performance and Memory

- Capture start p95 `≤ 500 ms` Owner Reference／Standard、`≤ 1,000 ms` Maximum.
- Pointer interaction frame time p95 `≤ 33 ms`; visible response p95 `≤ 100 ms`.
- Complete p95 tiers `≤ 1.5 s`、`4 s`、`8 s`.
- Save p95 tiers `≤ 2 s`、`6 s`、`12 s` after Save As confirmation.
- Idle private working set `≤ 250 MB`; maximum peak `≤ 2.0 GB`.
- Cleanup and 20-session retained-memory limits per `PRD-0006`.
- Measurement uses 3 warm-ups and at least 30 measured runs.

### Capacity

- `1`–`4` active logical displays.
- Each `≤ 3840 × 2160`.
- Total source pixels `≤ 33,177,600`.
- Virtual Desktop width and height each `≤ 16,384`.
- Selection width and height each `≤ 16,384`; area `≤ 67,108,864` pixels.
- 8K displays are outside v1.

### Owner Reference Runtime Profile

- primary `2560 × 1440`;
- lower `1920 × 1080` at Windows scaling `150%`;
- left `2560 × 1440`.

## Explicitly Deferred Capabilities

- Opaque freehand pen.
- Ellipse Annotation.
- Pin image to desktop.
- OCR.
- Capture history.
- Delayed capture.
- Additional save formats beyond PNG.
- Font-family selection、italic、underline and text background.
- Keyboard-only Annotation workflow.
- F6／Tab zone and object traversal as a complete workflow.
- Single-letter tool shortcuts.
- Ctrl-based Undo／Redo、Save or Complete shortcuts.
- Delete and Arrow-key object manipulation.
- Keyboard-created Annotation objects.
- HDR preservation.
- ARM64 and broader public support matrix.
- Cloud、sharing、plugins、telemetry、updates and release publication.

## Later Phase — Release and Compatibility Hardening

Begins only after v1 workflow and quality-gate conformance are demonstrated. It may cover:

- broader Windows／display／DPI compatibility beyond the accepted envelope;
- revised limits based on measured evidence and explicit approval;
- keyboard-only Annotation and shortcut design;
- logging and configuration selected from operational needs;
- packaging、signing、distribution and update strategy;
- other deferred capabilities through explicit product decisions.

## Documentation Policy

No new prerequisite、readiness、authorization or closure document family is planned. Product changes update existing PRD／Specs. Normal implementation progress updates source、tests、CHANGELOG and the existing conformance matrix.