# ROADMAP

狀態：`Accepted`

## Completed

- PRD v1.0：Freeze Approved。
- Specification v1.0：Freeze Approved。
- Architecture baseline：Freeze Approved。
- ADR-0002 through ADR-0007：Accepted。
- Implementation Contracts：Accepted。
- Project Structure and Toolchain Baseline：Accepted。
- Implementation Readiness Review：`Approved for first vertical slice implementation`。
- Repeated prerequisite／readiness／authorization／closure document pattern：Stopped。

## Current phase — First vertical slice implementation

狀態：`Verified`

Required scope：

1. Create the approved solution、configuration and project skeleton.
2. Restore and build the empty x64 baseline.
3. Implement Core contracts、state、cancel and failure handling with tests.
4. Implement WinUI 3 host and region selection.
5. Implement one-shot Windows.Graphics.Capture monitor acquisition and crop.
6. Produce the canonical SoftwareBitmap result.
7. Display through the Composition／Win2D adapter.
8. Publish through the DataPackage Clipboard adapter.
9. Run non-interactive tests and explicitly authorized Windows platform verification.
10. Record build、test、runtime and cleanup evidence.

Verified results：

- `SnipPlus.sln`、4 個 source projects 與 3 個 test projects 已建立。
- Locked restore、Release x64 build 與 packaged WinUI 3 publish 已完成。
- 非互動測試 24 passed；Rendering filter 6 passed。
- Packaged runtime 已以 public synthetic fixture 完成 region selection、Windows.Graphics.Capture、crop、render、PNG 與 Clipboard publication。
- Windows platform filter 可單獨執行；support check passed。未封裝 MSTest runner 的 in-memory frame test 因缺少 Windows App Runtime package graph 會標記 `Inconclusive`，不影響 packaged runtime verification。
- 未改變 Frozen behavior、Architecture ownership 或明確 non-goal。

Exit criteria：

- Solution restores and builds reproducibly.
- Required Unit、Contract and deterministic Rendering tests pass.
- Approved Windows platform verification produces reviewable evidence.
- Cancel、failure、retry and cleanup paths are verified.
- No Frozen behavior or Architecture ownership is changed by implementation.
- CHANGELOG and implementation evidence reflect actual results.

## Explicit first-slice non-goals

- Global hotkey／Print Screen interception.
- Multi-monitor stitched capture.
- Window-capture product mode.
- Annotation mutation tools.
- File Output UI.
- HDR/wide-color preservation.
- DXGI/GDI fallback.
- Telemetry、cloud、OCR、plugins、updates or release publication.

## Next phase — Product hardening

狀態：`In progress`

進入條件已滿足。目前已完成第一個 hardening slice：cancellation transition／cleanup tests 與 packaged `Start Capture → Cancel` runtime verification。

Windows in-memory frame platform test 現已透過 Windows App SDK 2.3 bootstrap 建立 package graph，並以實際 primary display id 執行成功。下一個 bounded slice 尚未選定；不得因目前測試全綠而擴張至明確 non-goal。

Begins only after the first slice is verified. It may address：

- Additional capture modes.
- Annotation tools.
- Output UI.
- Compatibility and performance.
- Logging/configuration.
- Packaging、signing、distribution and updates.
- ARM64 and wider Windows support.

## Documentation policy

No more pre-coding paperwork is planned. New documentation requires a concrete implementation finding、scope change or superseding decision. Normal next changes are code、tests、CHANGELOG and evidence records.
