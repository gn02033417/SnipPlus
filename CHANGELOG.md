# Changelog

本檔案依 Keep a Changelog 精神記錄對使用者、維護者與文件治理有意義的變更。尚未有產品版本發布。

## [Unreleased]

### Added

- 建立 Repository 文件治理、PRD、Specs、Architecture、ADR、Development Guide、Coding Standard、ROADMAP 與 TODO。
- 建立 solution／project skeleton：`SnipPlus.sln`、4 個 source projects、3 個 test projects、中央套件管理與 committed lock files。
- 新增 ADR-0002 至 ADR-0007。
- 新增 `Architecture/IMPLEMENTATION-CONTRACTS.md` 與 `Architecture/PROJECT-STRUCTURE.md`。
- 新增 Contracts、Core、Windows、WinUI shell 與 deterministic test foundations。
- 新增 `PRD-TRACEABILITY-MATRIX-001` 比對 accepted v1 requirements、current code、tests and runtime evidence。

### Changed

- 產品與品質基線完成定案；不再存在會阻塞 v1 實作的可見產品決策。
- MainWindow `X` 固定為直接結束 SnipPlus、解除 PrintScreen 接管、不隱藏至 System Tray。
- 不規則螢幕排列中的非顯示區域固定為最終影像透明像素，並要求 BGRA／PNG／Clipboard 保留 alpha。
- Windows Save As 初始資料夾固定為下載資料夾；PNG 成功但後續 Clipboard 失敗時保留 PNG、返回 Editing 並回報部分成功。
- 定義 v1 支援容量：`1`–`4` 個 logical displays、每個最大 `7,680 × 4,320`、總來源像素 `66,355,200`、Virtual Desktop 單邊最大 `16,384`、Selection 面積最大 `67,108,864` pixels。
- 超過任何容量限制時，流程必須在 Selection 前失敗、不可省略或部分擷取螢幕、必須清理並恢復先前工作內容。
- 定義量化效能目標：capture start p95 `500 ms`／`1,000 ms`、interaction p95 `33 ms` frame time、input response p95 `100 ms`、Complete p95 `1.5／4／8 s`、Save p95 `2／6／12 s`。
- 定義 commit 超過 `300 ms` 的 non-blocking progress、idle `250 MB`、maximum peak `2.0 GB`、cleanup 與 20-session memory limits。
- 定義完整 Keyboard-only Editing／Annotation：從 `SelectionLocked` 開始，包含 F6／Tab focus model、tool shortcuts、keyboard object creation、`1`／`10` pixel movement／resize、IME、High Contrast、200% scaling、Narrator state 與 no keyboard trap。
- 初始 crosshair Selection 維持 pointer-driven；keyboard-only 標準涵蓋完整 Editing／Annotation stage，而不是整個 capture entry。
- Esc 行為固定為：先關閉 transient picker／popover／text editor／uncommitted creation，再由 stable Editing 的 Esc 取消 capture session。
- `PRD-0006` 更新為 v1.3；`SPEC-0003` v1.3、`SPEC-0005` v1.2、`SPEC-0006` v1.2、`SPEC-0009` v1.1、`SPEC-0010` v1.2。
- `IMPLEMENTATION-CONTRACTS-001` 更新為 v2.2；Conformance Matrix 更新為 v2.2。
- Repository rules、Readiness Review、Freeze Review、Spec Baseline Review、README、Architecture index、Roadmap、TODO 與 docs index 已同步完整品質基線。
- 現行實作順序固定為 resident lifecycle／direct exit → PrintScreen → capacity／Frozen Virtual Desktop → cross-monitor Selection → SelectionLocked → state graph → function bar／progress／focus → Annotation／keyboard → history／clipping → Complete → Save → failure／performance／accessibility → authorized Standard／Maximum verification。
- Capture technical prototype維持 Selection 前只取得一次 frame，Selection 與 Crop 使用相同 Frozen Frame。
- 正常產品與 non-interactive tests 不啟動 Paint 或其他 external GUI fixture。

### Verified — Historical Technical Foundation

- Locked restore、Release x64 build and packaged WinUI 3 build曾於早期 technical slice 成功。
- Non-interactive Unit／Contract／Rendering／Clipboard tests and categorized Windows platform tests曾記錄成功。
- One-display WGC、same-frame crop、BGRA8、PNG encoding、Win2D presentation and Clipboard publication曾以 synthetic 或 categorized evidence 驗證。
- Packaged synthetic checkerboard驗證的是已淘汰的 `Start Capture → one-display Selection → mouse release → immediate crop／Clipboard` 流程。

Historical evidence只適用於上述技術基礎，不證明 resident PrintScreen、capacity、cross-monitor Selection、keyboard Editing、quantitative performance、Save As 或 focus restoration conforming。

### Current Conformance Status

- Resident lifecycle、direct application exit and PrintScreen takeover：Missing／Partial foundation only。
- Capacity policy and unsupported-limit outcomes：Missing。
- Frozen Virtual Desktop and per-display frame ownership：Missing。
- Cross-monitor Selection and transparent gap output：Missing。
- Mouse-release-to-output：Incorrect／obsolete。
- Selection adjustment and accepted state graph：Missing／obsolete current model。
- Editing function bar、Annotation tools and keyboard focus model：Missing。
- Save As、Downloads default、PNG delivery and retained-file workflow：Missing。
- Performance／memory measurement evidence：Missing。
- Recoverable Editing preservation、stale-revision protection、accessibility and focus restoration：Missing。

Detailed status and required actions are maintained in `PRD/PRD-TRACEABILITY-MATRIX.md`.

### Not Released

- No release publication、Store deployment or release artifact submission has occurred。
- Existing packages were used only for development／runtime verification。