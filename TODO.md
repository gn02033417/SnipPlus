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
- [x] Define the maximum source envelope as four 4K displays.
- [x] Define the Owner Reference three-display mixed-DPI verification profile.
- [x] Defer keyboard-only Annotation and non-PrintScreen tool／action shortcuts.

These items do not mean the accepted v1 workflow is implemented.

## P0 — v1 Workflow Conformance Correction

Work in order. Do not begin a later item while an earlier prerequisite is unresolved.

- [ ] Implement a manually started resident application lifecycle.
- [ ] Implement MainWindow `X` as direct application exit with no close-to-tray behavior.
- [ ] Implement a user-controlled PrintScreen takeover setting and release interception when disabled or exiting.
- [ ] If a System Tray surface exists, route its explicit Exit action through the same shutdown path.
- [ ] Route enabled PrintScreen through the single Workflow State Authority.
- [ ] Implement the accepted four-4K capacity policy and typed over-limit outcomes.
- [ ] Implement one Frozen Virtual Desktop Session with per-display snapshots and frozen-frame ownership.
- [ ] Present all displays as one logical frozen Selection canvas with crosshair and semi-transparent mask.
- [ ] Support one rectangular Selection spanning multiple displays.
- [ ] Render physical non-display gaps as transparent pixels.
- [ ] Replace mouse-release-to-output with a locked-Selection state.
- [ ] Support pointer Selection move、four-edge／four-corner resize and reselection.
- [ ] Replace the obsolete state graph with the accepted v1 state contract.
- [ ] Add the mandatory Editing／confirmation function bar.
- [ ] Implement Complete、Save and Cancel commitment boundaries.
- [ ] Show non-blocking commit progress after `300 ms`.
- [ ] Record and restore the pre-capture foreground application without reopening MainWindow.
- [x] Implement the Annotation document and object identity／revision model.
- [x] Implement pointer-driven Rectangle creation／draft preview／commit; object editing、other tools、annotation-aware final render and runtime acceptance remain incomplete, so FR-017 is still `Partial`.
- [x] Implement pointer-driven Arrow／Line creation／draft preview／commit with Arrow／Line mode; object editing、other tools、annotation-aware final render and runtime acceptance remain incomplete, so FR-018 is still `Partial`.
- [ ] Implement pointer-driven semi-transparent Highlighter.
- [ ] Implement Text with Microsoft JhengHei、color、font size、bold、Windows editing and Chinese IME.
- [ ] Implement pointer-driven rectangular Mosaic／Blur with per-object mode.
- [ ] Implement pointer-driven Numbered Marker with preserved gaps and configurable next number.
- [ ] Implement pointer object selection、move、resize、style changes and delete.
- [ ] Implement function-bar Annotation-only Undo／Redo.
- [ ] Anchor annotations to Frozen Virtual Desktop coordinates and clip output to current Selection.
- [ ] Implement final render with capacity revalidation.
- [ ] Place Clipboard publication only behind explicit Complete or successful Save.
- [ ] Implement Windows Save As、PNG-only output、Downloads initial folder and timestamp proposal.
- [ ] Allow the user to change destination and filename.
- [ ] Ensure Save writes the same rendered result to PNG and Clipboard.
- [ ] Retain PNG and return to Editing when Clipboard fails after PNG success.
- [ ] Preserve Editing state after recoverable failure.
- [ ] Reject stale Session／Selection／Annotation／output outcomes.
- [ ] Add required accessible names and non-color-only selected／error indicators.
- [ ] Add Owner Reference、Standard and Maximum performance measurement harnesses using `3` warm-up plus `30` measured runs.
- [ ] Verify capture-start p95 `500 ms`／`1,000 ms` targets.
- [ ] Verify pointer interaction p95 `33 ms` frame time and `100 ms` visible response.
- [ ] Verify Complete and Save output-size latency tiers.
- [ ] Verify idle `250 MB`、peak `2.0 GB`、cleanup and 20-session memory limits.
- [ ] Run explicitly authorized Owner Reference、Standard and Maximum multi-display runtime verification.
- [ ] Update the conformance matrix after each verified slice.

## Finalized v1 Capacity Envelope

- `1`–`4` active logical desktop display surfaces.
- Each display `≤ 3840 × 2160` physical pixels.
- Total active source pixels `≤ 33,177,600`.
- Virtual Desktop width and height each `≤ 16,384`.
- Final Selection width and height each `≤ 16,384`.
- Final Selection area `≤ 67,108,864` pixels.
- Transparent gaps count toward final Selection area.
- 8K displays are outside v1.
- Unsupported configurations fail before Selection without partial capture.

## Owner Reference Configuration

- primary `2560 × 1440`;
- lower `1920 × 1080` at Windows scaling `150%`;
- left `2560 × 1440`.

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
- Keyboard-only Annotation workflow.
- F6／Tab zone and object traversal as a complete workflow.
- Single-letter tool shortcuts.
- Ctrl-based Undo／Redo、Save or Complete shortcuts.
- Delete and Arrow-key object manipulation.
- Keyboard-created Annotation objects.
- HDR preservation、ARM64、cloud、sharing、plugins、telemetry、updates and release publication.

## Evidence Rules

- Build and tests must not be reported as proof of product conformance without relevant behavior evidence.
- Performance evidence must report p50、p95、maximum、profile、output size and memory values.
- Final runtime acceptance includes the Owner Reference three-display mixed-DPI profile and a Maximum four-4K profile.
- V1 evidence must not claim keyboard-only Annotation or non-PrintScreen shortcut conformance.
- Interactive verification requires explicit authorization in the current task.
- Do not persist real desktop screenshots or Clipboard payloads as repository evidence.
- Update `CHANGELOG.md` and the existing conformance matrix with actual findings; do not create another readiness／closure chain.
