# TODO

狀態：`Accepted`

This file tracks implementation and evidence work. Product scope is owned by accepted PRD／Specs; conformance status is owned by `PRD/PRD-TRACEABILITY-MATRIX.md`.

## Completed Technical and Product Foundations

- [x] Create solution、configuration、four source projects and three test projects.
- [x] Establish the accepted C#／.NET／Windows App SDK／Win2D／MSTest toolchain.
- [x] Implement one-display WGC acquisition.
- [x] Acquire a frozen frame before Selection and crop the same frame.
- [x] Implement one-display clear-inside／dim-outside Selection mask.
- [x] Implement canonical BGRA8 premultiplied SoftwareBitmap ownership.
- [x] Implement deterministic crop and in-memory PNG encoding.
- [x] Implement Win2D／WinUI image presentation foundation.
- [x] Implement WinRT Clipboard delivery with bounded cancellable retry、history disabled and roaming disabled.
- [x] Add low-level Unit、Contract、Rendering and authorized Platform tests for these foundations.
- [x] Complete static requirements-to-code conformance review.
- [x] Resolve MainWindow exit／PrintScreen release behavior.
- [x] Resolve transparent output for irregular-layout display gaps.
- [x] Resolve Save As Downloads default and PNG retention after later Clipboard failure.
- [x] Define quantitative performance、responsiveness、output、memory and cleanup targets.
- [x] Define maximum display count、per-display size、Virtual Desktop dimensions and Selection area.
- [x] Define complete keyboard-only Editing／Annotation acceptance from `SelectionLocked`.

These items do not mean the accepted v1 workflow is implemented.

## P0 — v1 Workflow Conformance Correction

Work in order. Do not begin a later item while an earlier prerequisite is unresolved.

- [ ] Implement a manually started resident application lifecycle.
- [ ] Implement MainWindow `X` as direct application exit with no close-to-tray behavior.
- [ ] Implement a user-controlled PrintScreen takeover setting and release interception when disabled or exiting.
- [ ] If a System Tray surface exists, route its explicit Exit action through the same shutdown path.
- [ ] Route enabled PrintScreen through the single Workflow State Authority.
- [ ] Implement the accepted capacity policy and typed over-limit outcomes.
- [ ] Implement one Frozen Virtual Desktop session with per-display snapshots and frozen-frame ownership.
- [ ] Present all displays as one logical frozen Selection canvas with crosshair and semi-transparent mask.
- [ ] Support one rectangular Selection spanning multiple displays.
- [ ] Render physical non-display gaps as transparent pixels.
- [ ] Replace mouse-release-to-output with a locked-Selection state.
- [ ] Support Selection move、four-edge／four-corner resize and reselection by pointer and keyboard.
- [ ] Replace the obsolete state graph with the accepted v1 state contract.
- [ ] Add the mandatory Editing／confirmation function bar.
- [ ] Implement F6、Tab／Shift+Tab zone、object and handle navigation.
- [ ] Implement Complete、Save and Cancel commitment boundaries.
- [ ] Show non-blocking commit progress after `300 ms`.
- [ ] Record and restore the pre-capture foreground application without reopening MainWindow.
- [ ] Implement the Annotation document and object identity／revision model.
- [ ] Implement Rectangle with pointer and keyboard creation.
- [ ] Implement Arrow／Line with pointer and keyboard creation.
- [ ] Implement semi-transparent Highlighter with pointer and deterministic keyboard creation.
- [ ] Implement Text with Microsoft JhengHei、color、font size、bold、Windows editing and Chinese IME.
- [ ] Implement rectangular Mosaic／Blur with per-object mode and keyboard creation.
- [ ] Implement Numbered Marker with preserved gaps、configurable next number and keyboard placement.
- [ ] Implement object selection、move、resize、style changes and delete by pointer and keyboard.
- [ ] Implement `1`-pixel Arrow and `10`-pixel Shift+Arrow movement／resize.
- [ ] Implement Annotation-only Undo／Redo.
- [ ] Anchor annotations to Frozen Virtual Desktop coordinates and clip output to the current Selection.
- [ ] Implement final render with capacity revalidation.
- [ ] Place Clipboard publication only behind explicit Complete or successful Save.
- [ ] Implement Windows Save As、PNG-only output、Downloads initial folder and timestamp proposal.
- [ ] Allow the user to change destination and filename.
- [ ] Ensure Save writes the same rendered result to PNG and Clipboard.
- [ ] Retain PNG and return to Editing when Clipboard fails after PNG success.
- [ ] Preserve Editing state and keyboard focus context after recoverable failure.
- [ ] Reject stale session／Selection／Annotation／output outcomes.
- [ ] Add visible focus、High Contrast、200% scaling and Narrator names／states.
- [ ] Add Standard and Maximum performance measurement harnesses using `3` warm-up plus `30` measured runs.
- [ ] Verify capture-start p95 `500 ms`／`1,000 ms` targets.
- [ ] Verify interaction p95 `33 ms` frame time and `100 ms` input response.
- [ ] Verify Complete and Save output-size latency tiers.
- [ ] Verify idle `250 MB`、peak `2.0 GB`、cleanup and 20-session memory limits.
- [ ] Run explicitly authorized Standard and Maximum multi-display runtime verification.
- [ ] Update the conformance matrix after each verified slice.

## Finalized v1 Capacity Envelope

- `1`–`4` active logical desktop display surfaces.
- Each display `≤ 7,680 × 4,320` physical pixels.
- Total active source pixels `≤ 66,355,200`.
- Virtual Desktop width and height each `≤ 16,384`.
- Final Selection width and height each `≤ 16,384`.
- Final Selection area `≤ 67,108,864` pixels.
- Unsupported configurations fail before Selection without partial capture.

## Deferred Product Capabilities

Do not implement without a later explicit product decision:

- Opaque freehand pen.
- Ellipse Annotation.
- Pin image to desktop.
- OCR.
- Capture history.
- Delayed capture.
- Additional image formats.
- Font-family selection、italic、underline and text background.
- HDR preservation、ARM64、cloud、sharing、plugins、telemetry、updates and release publication.

## Evidence Rules

- Build and tests must not be reported as proof of product conformance without relevant behavior evidence.
- Performance evidence must report p50、p95、maximum、profile、output size and memory values.
- Keyboard-only acceptance begins at `SelectionLocked` with pointer unused afterward.
- Interactive verification requires explicit authorization in the current task.
- Do not persist real desktop screenshots or Clipboard payloads as repository evidence.
- Update `CHANGELOG.md` and the existing conformance matrix with actual findings; do not create another readiness／closure chain.