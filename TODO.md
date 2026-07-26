# TODO

狀態：`Accepted`

The pre-coding documentation backlog is complete. This file now tracks implementation and evidence work only.

## P0 — First vertical slice

- [x] Create `SnipPlus.sln`、`global.json`、`Directory.Build.props`、`Directory.Packages.props` and `.editorconfig`.
- [x] Create the four approved source projects and three test projects.
- [x] Restore with locked dependencies and build the empty Release x64 baseline.
- [x] Implement Implementation Contracts and Core workflow/state/failure behavior.
- [x] Implement WinUI 3 app shell and explicit capture command.
- [x] Implement single-monitor region selection and coordinate conversion.
- [x] Implement Windows.Graphics.Capture one-shot acquisition and crop.
- [x] Implement BGRA8 premultiplied SoftwareBitmap result ownership.
- [x] Implement Composition／Win2D display adapter.
- [x] Implement DataPackage Clipboard publication、privacy options、Flush and bounded retry.
- [x] Add Unit、Contract and deterministic Rendering tests.
- [x] Run explicitly authorized interactive Capture/Clipboard verification.
- [x] Record restore、build、test、runtime and cleanup evidence.

第一個 Vertical Slice 已驗證完成。已知限制是未封裝 MSTest runner 的 Windows in-memory frame test 會因缺少 Windows App Runtime package graph 標記 `Inconclusive`；packaged runtime capture 已成功。

## P0 — Stop/report conditions

Report before expanding scope if：

- Pinned dependencies cannot restore/build together.
- WGC or Win2D fails the accepted boundary.
- Coordinate/crop fidelity is not deterministic.
- Clipboard consumer compatibility requires a materially different API/format.
- A project dependency cycle is required.
- Frozen behavior or Architecture ownership must change.
- Implementation enters an explicit non-goal.

## P1 — After first-slice verification

- [x] Harden cancellation transition／cleanup paths and verify packaged `Start Capture → Cancel` runtime behavior.
- [x] Run the Windows in-memory frame platform test with a resolvable Windows App Runtime 2.3 package graph and an actual display id.
- [ ] Decide whether to add file Output UI.
- [ ] Define production logging/configuration needs from actual failures.
- [ ] Decide final packaging、signing and distribution.
- [ ] Establish broader Windows/ARM64 support matrix.
- [ ] Add compatible annotation tools through new Specs if prioritized.

## Completed documentation

- [x] PRD、Specification and Architecture baselines frozen.
- [x] ADR-0002 UI Framework.
- [x] ADR-0003 Rendering Technology.
- [x] ADR-0004 Capture Backend.
- [x] ADR-0005 Image Representation.
- [x] ADR-0006 Clipboard Integration.
- [x] ADR-0007 Testing Strategy.
- [x] Implementation Contracts.
- [x] Project Structure and Toolchain Baseline.
- [x] Implementation Readiness Review approved.
- [x] Repository entry/status/index documents aligned.
- [x] Clipboard D1 039→052 closure chain stopped.

## Deferred product capabilities

Not part of the first slice：OCR、cloud sync、sharing、cross-platform、plugins、advanced annotation、multi-monitor stitching、window mode、HDR preservation、telemetry and release/update infrastructure.
