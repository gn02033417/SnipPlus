# ROADMAP

狀態：`Accepted`

## Completed foundations

- Accepted v1.1 PRD and current Specification baseline.
- Accepted Architecture principles、layers、modules、components and interactions.
- ADR-0002 through ADR-0007.
- Implementation Contracts v2.0.
- Project Structure and toolchain baseline.
- Solution、four source projects and three test projects.
- Reusable one-display WGC、same-frame crop、BGRA8 image、PNG encoder、Win2D presentation and Clipboard retry foundations.
- Requirements-to-code conformance audit.
- Repetitive prerequisite／readiness／authorization／closure document pattern stopped.

## Current phase — v1 workflow conformance correction

狀態：`Active when explicitly authorized`

The current technical prototype does not conform to the accepted v1 product workflow. Implementation must proceed in this order:

1. Resident lifecycle and user-controlled PrintScreen takeover setting.
2. PrintScreen entry integrated with `COMP-001`.
3. Frozen Virtual Desktop session context and per-display frame ownership.
4. All-display frozen presentation、crosshair and cross-monitor initial selection.
5. Locked selection、move、edge／corner resize and reselection.
6. Accepted state graph including `ResidentReady`、`Freezing`、`Selecting`、`SelectionLocked`、`Editing`、`CommittingClipboard` and `Saving`.
7. Function bar、Complete／Save／Cancel commitments and foreground-context restoration.
8. Annotation document、required tools and object editing.
9. Annotation-only Undo／Redo、Virtual Desktop anchoring and selection clipping.
10. Complete final render plus Clipboard.
11. Windows Save As、PNG file delivery plus the same Clipboard result.
12. Recoverable failure preservation、stale-session／revision protection and accessibility.
13. Explicitly authorized multi-display runtime verification.

### Phase rules

- Do not continue from the obsolete mouse-release-to-Clipboard sequence.
- Reuse technical foundations only after their row in the conformance matrix is reviewed.
- Each completed slice updates code、tests、CHANGELOG and `PRD-TRACEABILITY-MATRIX-001`.
- Passing build or test counts do not prove user-visible conformance.
- Do not begin a later step while an earlier prerequisite remains `Missing` or `Incorrect`.

## Required v1 capabilities

- Manual startup and background residency.
- User-controlled PrintScreen takeover.
- All-display Frozen Virtual Desktop.
- Cross-monitor rectangular selection.
- Selection movement、edge／corner resize and reselection.
- Mandatory editing／confirmation function bar.
- Rectangle、Arrow／Line、Highlighter、Text、Mosaic／Blur and Numbered Marker.
- Annotation selection、movement、resize、style changes、delete、Undo and Redo.
- Complete to Clipboard.
- Save to PNG and Clipboard.
- Cancel、recoverable failure preservation、cleanup and focus restoration.

## Explicitly deferred capabilities

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

## Later phase — release and compatibility hardening

Begins only after accepted v1 workflow conformance is demonstrated. It may cover:

- Performance measurement and limits.
- Broader Windows／display／DPI compatibility.
- Logging and configuration selected from actual operational needs.
- Packaging、signing、distribution and update strategy.
- Additional deferred product capabilities through explicit product decisions.

## Documentation policy

No new prerequisite、readiness、authorization or closure document family is planned. Product changes update existing PRD／Specs. Durable technology changes use a targeted ADR. Normal implementation progress updates source、tests、CHANGELOG and the existing conformance matrix.
