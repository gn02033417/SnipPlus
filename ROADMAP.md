# ROADMAP

狀態：`Accepted`

## Completed Foundations

- Accepted current PRD and Specification baseline.
- Accepted Architecture principles、layers、modules、components and interactions.
- ADR-0002 through ADR-0007.
- Implementation Contracts v2.2.
- Project Structure and toolchain baseline.
- Solution、four source projects and three test projects.
- Reusable one-display WGC、same-frame crop、BGRA8 image、PNG encoder、Win2D presentation and Clipboard retry foundations.
- Requirements-to-code conformance audit.
- Product decisions for direct application exit、transparent display gaps、Downloads Save As default and retained PNG after Clipboard failure.
- Quantitative performance、memory、capacity and keyboard-only Editing／Annotation acceptance baseline.
- Repetitive prerequisite／readiness／authorization／closure document pattern stopped.

## Current Phase — v1 Workflow Conformance Correction

狀態：`Active when explicitly authorized`

The current technical prototype does not conform to the accepted v1 product workflow. Implementation must proceed in this order:

1. Resident lifecycle、direct MainWindow exit and user-controlled PrintScreen takeover setting.
2. PrintScreen entry integrated with `COMP-001`.
3. Capacity policy、Frozen Virtual Desktop session context and per-display frame ownership.
4. All-display frozen presentation、crosshair and cross-monitor initial Selection.
5. Locked Selection、move、edge／corner resize and reselection.
6. Accepted state graph including `ResidentReady`、`Freezing`、`Selecting`、`SelectionLocked`、`Editing`、`CommittingClipboard` and `Saving`.
7. Function bar、Complete／Save／Cancel、progress and foreground-context restoration.
8. Annotation document、required tools、keyboard focus model and object editing.
9. Annotation-only Undo／Redo、Virtual Desktop anchoring、Selection clipping and keyboard-only acceptance.
10. Complete final render、capacity revalidation、transparent gap output and Clipboard.
11. Windows Save As with Downloads default、PNG file delivery、same-result Clipboard and retained-file partial outcome.
12. Recoverable failure preservation、stale-session／revision protection、performance／memory evidence and accessibility.
13. Explicitly authorized Standard and Maximum multi-display runtime verification.

### Phase Rules

- Do not continue from the obsolete mouse-release-to-Clipboard sequence.
- Reuse technical foundations only after their row in the conformance matrix is reviewed.
- Each completed slice updates code、tests、CHANGELOG and `PRD-TRACEABILITY-MATRIX-001`.
- Passing build or test counts do not prove user-visible conformance.
- Do not begin a later step while an earlier prerequisite remains `Missing` or `Incorrect`.
- Do not relax accepted limits because implementation is difficult; product changes require explicit approval.

## Required v1 Capabilities

- Manual startup and residency while the application is running.
- MainWindow `X` directly exits and releases PrintScreen takeover; no close-to-tray behavior.
- User-controlled PrintScreen takeover.
- Capacity validation before Selection and typed unsupported-limit failure.
- All-display Frozen Virtual Desktop.
- Cross-monitor rectangular Selection.
- Transparent output for physical non-display gaps.
- Selection movement、edge／corner resize and reselection by pointer and keyboard.
- Mandatory Editing／confirmation function bar.
- Rectangle、Arrow／Line、Highlighter、Text、Mosaic／Blur and Numbered Marker.
- Annotation selection、movement、resize、style changes、delete、Undo and Redo by pointer and keyboard.
- Complete to Clipboard with progress after `300 ms`.
- Save As initially opens Downloads、saves PNG and writes Clipboard.
- Retain PNG when later Clipboard delivery fails.
- Cancel、recoverable failure preservation、cleanup and focus restoration.
- Visible focus、High Contrast、200% scaling、Narrator state and Chinese IME support.

## Accepted Quality Gates

### Performance and memory

- Capture start p95 `≤ 500 ms` Standard、`≤ 1,000 ms` Maximum.
- Interaction frame time p95 `≤ 33 ms`; discrete response p95 `≤ 100 ms`.
- Complete p95 tiers `≤ 1.5 s`、`4 s`、`8 s`.
- Save p95 tiers `≤ 2 s`、`6 s`、`12 s` after Save As confirmation.
- Idle private working set `≤ 250 MB`; maximum peak `≤ 2.0 GB`.
- Cleanup and 20-session retained-memory limits per `PRD-0006`.

### Capacity

- `1`–`4` active logical displays.
- Each `≤ 7,680 × 4,320`.
- Total source pixels `≤ 66,355,200`.
- Virtual Desktop width and height each `≤ 16,384`.
- Selection area `≤ 67,108,864` pixels with dimension limits.

### Keyboard-only Editing

- Scope begins at `SelectionLocked`.
- Complete function-bar、object、handle、tool、style、Undo／Redo、Save、Complete and Cancel workflow works without pointer input.
- Initial crosshair Selection remains pointer-driven in v1.

## Explicitly Deferred Capabilities

- Opaque freehand pen.
- Ellipse Annotation.
- Pin image to desktop.
- OCR.
- Capture history.
- Delayed capture.
- Additional save formats beyond PNG.
- Font-family selection、italic、underline and text background.
- HDR preservation.
- ARM64 and broader public support matrix.
- Cloud、sharing、plugins、telemetry、updates and release publication.

## Later Phase — Release and Compatibility Hardening

Begins only after accepted v1 workflow and quality-gate conformance are demonstrated. It may cover:

- broader Windows／display／DPI compatibility beyond the accepted envelope;
- revised limits based on measured evidence and explicit product approval;
- logging and configuration selected from operational needs;
- packaging、signing、distribution and update strategy;
- additional deferred capabilities through explicit product decisions.

## Documentation Policy

No new prerequisite、readiness、authorization or closure document family is planned. Product changes update existing PRD／Specs. Durable technology changes use a targeted ADR. Normal implementation progress updates source、tests、CHANGELOG and the existing conformance matrix.