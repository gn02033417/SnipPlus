# ROADMAP

狀態：`Accepted`

## Completed Foundations

- Accepted v1.2 PRD and current Specification baseline.
- Accepted Architecture principles、layers、modules、components and interactions.
- ADR-0002 through ADR-0007.
- Implementation Contracts v2.1.
- Project Structure and toolchain baseline.
- Solution、four source projects and three test projects.
- Reusable one-display WGC、same-frame crop、BGRA8 image、PNG encoder、Win2D presentation and Clipboard retry foundations.
- Requirements-to-code conformance audit.
- Product decisions for direct application exit、transparent display gaps、Downloads Save As default and retained PNG after Clipboard failure.
- Repetitive prerequisite／readiness／authorization／closure document pattern stopped.

## Current Phase — v1 Workflow Conformance Correction

狀態：`Active when explicitly authorized`

The current technical prototype does not conform to the accepted v1 product workflow. Implementation must proceed in this order:

1. Resident lifecycle、direct MainWindow exit and user-controlled PrintScreen takeover setting.
2. PrintScreen entry integrated with `COMP-001`.
3. Frozen Virtual Desktop session context and per-display frame ownership.
4. All-display frozen presentation、crosshair and cross-monitor initial selection.
5. Locked selection、move、edge／corner resize and reselection.
6. Accepted state graph including `ResidentReady`、`Freezing`、`Selecting`、`SelectionLocked`、`Editing`、`CommittingClipboard` and `Saving`.
7. Function bar、Complete／Save／Cancel commitments and foreground-context restoration.
8. Annotation document、required tools and object editing.
9. Annotation-only Undo／Redo、Virtual Desktop anchoring and selection clipping.
10. Complete final render、transparent gap output and Clipboard.
11. Windows Save As with Downloads default、PNG file delivery、same-result Clipboard and retained-file partial outcome.
12. Recoverable failure preservation、stale-session／revision protection and accessibility.
13. Explicitly authorized multi-display runtime verification.

### Phase Rules

- Do not continue from the obsolete mouse-release-to-Clipboard sequence.
- Reuse technical foundations only after their row in the conformance matrix is reviewed.
- Each completed slice updates code、tests、CHANGELOG and `PRD-TRACEABILITY-MATRIX-001`.
- Passing build or test counts do not prove user-visible conformance.
- Do not begin a later step while an earlier prerequisite remains `Missing` or `Incorrect`.

## Required v1 Capabilities

- Manual startup and residency while the application is running.
- MainWindow `X` directly exits and releases PrintScreen takeover; no close-to-tray behavior.
- User-controlled PrintScreen takeover.
- All-display Frozen Virtual Desktop.
- Cross-monitor rectangular selection.
- Transparent output for physical non-display gaps.
- Selection movement、edge／corner resize and reselection.
- Mandatory editing／confirmation function bar.
- Rectangle、Arrow／Line、Highlighter、Text、Mosaic／Blur and Numbered Marker.
- Annotation selection、movement、resize、style changes、delete、Undo and Redo.
- Complete to Clipboard.
- Save As initially opens Downloads、saves PNG and writes Clipboard.
- Retain PNG when later Clipboard delivery fails.
- Cancel、recoverable failure preservation、cleanup and focus restoration.

## Explicitly Deferred Capabilities

- Opaque freehand pen.
- Ellipse annotation.
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

Begins only after accepted v1 workflow conformance is demonstrated. It may cover:

- Performance measurement and limits.
- Supported display-count and maximum Virtual Desktop dimensions.
- Final keyboard-only Annotation acceptance.
- Broader Windows／display／DPI compatibility.
- Logging and configuration selected from actual operational needs.
- Packaging、signing、distribution and update strategy.
- Additional deferred capabilities through explicit product decisions.

## Documentation Policy

No new prerequisite、readiness、authorization or closure document family is planned. Product changes update existing PRD／Specs. Durable technology changes use a targeted ADR. Normal implementation progress updates source、tests、CHANGELOG and the existing conformance matrix.