# TODO

狀態：`Accepted`

This file tracks implementation and evidence work. Product scope is owned by accepted PRD／Specs; conformance status is owned by `PRD/PRD-TRACEABILITY-MATRIX.md`.

## Completed Technical Foundations

- [x] Create solution、configuration、four source projects and three test projects.
- [x] Establish the accepted C#／.NET／Windows App SDK／Win2D／MSTest toolchain.
- [x] Implement one-display WGC acquisition.
- [x] Acquire a frozen frame before selection and crop the same frame.
- [x] Implement one-display clear-inside／dim-outside selection mask.
- [x] Implement canonical BGRA8 premultiplied SoftwareBitmap ownership.
- [x] Implement deterministic crop and in-memory PNG encoding.
- [x] Implement Win2D／WinUI image presentation foundation.
- [x] Implement WinRT Clipboard delivery with bounded cancellable retry、history disabled and roaming disabled.
- [x] Add low-level Unit、Contract、Rendering and authorized Platform tests for these foundations.
- [x] Complete static requirements-to-code conformance review.
- [x] Resolve MainWindow exit／PrintScreen release behavior.
- [x] Resolve transparent output for irregular-layout display gaps.
- [x] Resolve Save As Downloads default and PNG retention after later Clipboard failure.

These items do not mean the accepted v1 workflow is implemented.

## P0 — v1 Workflow Conformance Correction

Work in order. Do not begin a later item while an earlier prerequisite is unresolved.

- [ ] Implement a manually started resident application lifecycle.
- [ ] Implement MainWindow `X` as direct application exit with no close-to-tray behavior.
- [ ] Implement a user-controlled PrintScreen takeover setting and release interception when disabled or exiting.
- [ ] If a System Tray surface exists, route its explicit Exit action through the same shutdown path.
- [ ] Route enabled PrintScreen through the single Workflow State Authority.
- [ ] Define and implement one Frozen Virtual Desktop session with per-display snapshots and frozen-frame ownership.
- [ ] Present all displays as one logical frozen selection canvas with crosshair and semi-transparent mask.
- [ ] Support one rectangular selection spanning multiple displays.
- [ ] Render physical non-display gaps as transparent pixels.
- [ ] Replace mouse-release-to-output with a locked-selection state.
- [ ] Support selection move、four-edge／four-corner resize and reselection.
- [ ] Replace the obsolete state graph with the accepted v1 state contract.
- [ ] Add the mandatory editing／confirmation function bar.
- [ ] Implement Complete、Save and Cancel commitment boundaries.
- [ ] Record and restore the pre-capture foreground application without reopening the SnipPlus main window.
- [ ] Implement the annotation document and object identity／revision model.
- [ ] Implement Rectangle.
- [ ] Implement Arrow／Line.
- [ ] Implement semi-transparent freehand Highlighter.
- [ ] Implement Text with Microsoft JhengHei、color、font size and bold.
- [ ] Implement rectangular Mosaic／Blur with per-object mode.
- [ ] Implement Numbered Marker with preserved numbering gaps and configurable next number.
- [ ] Implement object selection、move、resize、style changes and delete.
- [ ] Implement annotation-only Undo／Redo.
- [ ] Anchor annotations to Frozen Virtual Desktop coordinates and clip output to the current selection.
- [ ] Implement final render for the current selection and annotation revision.
- [ ] Place Clipboard publication only behind explicit Complete or successful Save.
- [ ] Implement Windows Save As、PNG-only output、Downloads initial folder and `SnipPlus_yyyy-MM-dd_HHmmss.png` proposal.
- [ ] Allow the user to change destination and filename.
- [ ] Ensure Save writes the same rendered result to PNG and Clipboard.
- [ ] Retain the PNG and return to Editing when Clipboard fails after PNG success.
- [ ] Preserve Editing state after recoverable render、save or Clipboard failure.
- [ ] Reject stale session／selection／annotation／output outcomes.
- [ ] Add required accessibility names and non-color-only state indicators.
- [ ] Run explicitly authorized multi-display runtime verification.
- [ ] Update the conformance matrix after each verified slice.

## Remaining Open Decisions

- [ ] Define final keyboard-only Annotation acceptance standard.
- [ ] Define quantitative performance targets after measurement.
- [ ] Define supported display-count and maximum Virtual Desktop dimensions from evidence.

## Deferred Product Capabilities

Do not implement without a later explicit product decision:

- Opaque freehand pen.
- Ellipse annotation.
- Pin image to desktop.
- OCR.
- Capture history.
- Delayed capture.
- Additional image formats.
- Font-family selection、italic、underline and text background.
- HDR preservation、ARM64、cloud、sharing、plugins、telemetry、updates and release publication.

## Evidence Rules

- Build and tests must not be reported as proof of product conformance without relevant behavior evidence.
- Interactive verification requires explicit authorization in the current task.
- Do not persist real desktop screenshots or Clipboard payloads as repository evidence.
- Update `CHANGELOG.md` and the existing conformance matrix with actual findings; do not create another readiness／closure chain.