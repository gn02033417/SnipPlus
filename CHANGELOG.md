# Changelog

本檔案依 Keep a Changelog 精神記錄對使用者、維護者與文件治理有意義的變更。尚未有產品版本發布。

## [Unreleased]

### Added

- 建立 Repository 文件治理、Research／Analysis／Decision、PRD、Specs、Architecture、ADR governance、Development Guide、Coding Standard、ROADMAP 與 TODO。
- 建立第一階段 solution／project skeleton：`SnipPlus.sln`、4 個 source projects、3 個 test projects、中央套件管理與 committed lock files。
- 新增 `ADR-0002`：WinUI 3 UI Framework。
- 新增 `ADR-0003`：WinUI XAML／Microsoft.UI.Composition + Win2D rendering adapter。
- 新增 `ADR-0004`：Windows.Graphics.Capture Capture Backend。
- 新增 `ADR-0005`：BGRA8 premultiplied SoftwareBitmap canonical image representation。
- 新增 `ADR-0006`：WinRT DataPackage Clipboard integration with history／roaming disabled by default。
- 新增 `ADR-0007`：MSTest.Sdk + Microsoft.Testing.Platform testing strategy。
- 新增 `Architecture/IMPLEMENTATION-CONTRACTS.md` 與 `Architecture/PROJECT-STRUCTURE.md`。
- 新增 `SnipPlus.Contracts` workflow、capture、coordinate、image、failure、Clipboard 與 Output contracts。
- 新增 `SnipPlus.Core` workflow state authority、coordinate mapping 與 capture／Clipboard coordination foundation。
- 新增 canonical BGRA8 premultiplied `SoftwareBitmap` image pipeline、PNG encoding、crop 與 Win2D rendering adapter。
- 新增 Windows.Graphics.Capture monitor adapter、WinRT Clipboard adapter、packaged WinUI shell 與 deterministic test foundations。
- 新增 `PRD-TRACEABILITY-MATRIX-001`，逐項比對 accepted v1 requirements、current code、tests and runtime evidence。

### Changed

- 產品基線更新為 Accepted v1.1：手動啟動並常駐、使用者控制 PrintScreen 接管、凍結所有螢幕、跨螢幕矩形選區、SelectionLocked、mandatory Editing／confirmation、required Annotation tools、Complete-to-Clipboard、Save-to-PNG-and-Clipboard、Cancel／failure preservation／focus restoration。
- `PRD-0004`–`PRD-0006`、`SPEC-0003`–`SPEC-0010`、`IMPLEMENTATION-CONTRACTS-001`、Implementation Readiness 與 Repository rules 已對齊新產品基線。
- `ARCH-0001`–`ARCH-0005`、Architecture Baseline Review、System Overview、Architecture Diagram、Project Structure and Architecture index 已重整，移除 optional-Annotation、single-display、mouse-release-to-output 與 unconditional parallel Clipboard／Output assumptions。
- `README.md`、`docs/index.md`、`ROADMAP.md`、`TODO.md`、PRD／Specs index、Development Guide、Technology Decision Roadmap、Repository Audit 與 UI Wireframe 已對齊目前 Repository 狀態與 conformance correction order。
- PRD Freeze Review 與 Specification Baseline Review 更新為目前 v1.1 acceptance records；早期 v1.0 review content 保留為歷史背景，不再覆蓋 Accepted baseline。
- 目前程式正式分類為可重用的單螢幕 WGC／same-frame crop／image／PNG／Clipboard technical prototype，而不是 SnipPlus v1 product-complete implementation。
- 現行實作順序固定為 resident lifecycle → PrintScreen → Frozen Virtual Desktop → cross-monitor Selection → SelectionLocked adjustment → accepted state graph → function bar／commitments／focus → Annotation → history／clipping → Complete → Save → failure／revision／accessibility → authorized multi-display verification。
- `global.json` 加入 .NET 10 的 Microsoft.Testing.Platform test runner opt-in。
- Packaged `win-x64` restore graph、manifest assets、runtime identifiers and framework-dependent MSIX validation were corrected without replacing accepted SDK or package versions。
- Capture workflow technical prototype改為 Selection 前只取得一次完整 monitor frame，Selection 顯示同一張 frame，最終 Crop 使用同一 Frozen Frame，不再於 Selection 後重新擷取。
- Packaged runtime verification 改用 SnipPlus 內部 synthetic checkerboard source；正常產品啟動不建立該 source，未加入 external GUI fixture launch code。
- Clipboard adapter保留 production WinRT publication／Flush behavior and added deterministic publisher／flush seams for retry and cancellation tests。

### Verified — historical technical foundation

- Locked restore、Release x64 build and packaged WinUI 3 build succeeded for the earlier technical slice。
- Non-interactive Unit／Contract／Rendering／Clipboard tests and categorized Windows platform tests were previously recorded as passing at the time of execution。
- One-display Windows.Graphics.Capture frame acquisition、same-frame crop、BGRA8 result、PNG encoding、Win2D presentation and Clipboard publication were verified through synthetic or categorized platform evidence。
- Packaged synthetic checkerboard verification demonstrated the superseded sequence `Start Capture → one-display Selection → mouse release → immediate crop／Clipboard`。
- The historical verification did not persist private desktop screenshots or Clipboard payloads and did not launch Paint during the corrected synthetic run。

The historical build／test／runtime evidence remains valid only for those technical foundations. It is not evidence of accepted v1 resident PrintScreen、all-display freeze、cross-monitor Selection、Editing／Annotation、Save As or focus restoration conformance.

### Current conformance status

- Resident lifecycle and PrintScreen takeover：Missing。
- Frozen Virtual Desktop and per-display frame ownership：Missing。
- Cross-monitor selection：Missing。
- Mouse-release-to-output behavior：Incorrect／obsolete。
- Selection adjustment and accepted state graph：Missing／obsolete current model。
- Editing function bar and required Annotation tools：Missing。
- Save As and PNG file-delivery workflow：Missing。
- Recoverable Editing preservation、stale-revision protection and focus restoration：Missing。

Detailed status and required actions are maintained in `PRD/PRD-TRACEABILITY-MATRIX.md`.

### Not released

- No release publication、Store deployment or release artifact submission has occurred。
- Existing packages were used only for development／runtime verification。
