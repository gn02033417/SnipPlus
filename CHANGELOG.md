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
- v1 最大來源容量修正為 `1`–`4` 個 logical displays、每個最大 `3840 × 2160`、總來源像素 `33,177,600`；8K display 明確排除於 v1。
- Virtual Desktop 寬高單邊維持最大 `16,384`，Selection 寬高單邊維持最大 `16,384`，Selection 面積維持最大 `67,108,864` pixels，以容納四台 4K 不規則排列形成的透明空白。
- 超過任何容量限制時，流程必須在 Selection 前或 Selection lock 前失敗、不可省略／縮小／部分擷取螢幕，並必須清理資源與恢復原本應用程式。
- 加入 Repository owner 實際三螢幕驗證 Profile：主螢幕 `2560 × 1440`、下方 `1920 × 1080` 且 Windows scaling `150%`、左側 `2560 × 1440`。
- 量化效能目標維持：capture start p95 `500 ms`／`1,000 ms`、pointer interaction p95 `33 ms` frame time、visible response p95 `100 ms`、Complete p95 `1.5／4／8 s`、Save p95 `2／6／12 s`。
- 測量規則固定為 3 次 warm-up、至少 30 次 measured runs，回報 p50、p95、maximum；Save As 使用者決策時間不計入 Save latency。
- Commit 超過 `300 ms` 顯示 non-blocking progress；idle `250 MB`、maximum peak `2.0 GB`、cleanup 與 20-session memory limits 維持不變。
- V1 Annotation、Selection adjustment 與 object editing 固定為 pointer-driven。
- 完整 keyboard-only Annotation 與 PrintScreen 以外的 tool／action shortcuts 延後，包括 F6／Tab workflow、single-letter tools、Ctrl-based Undo／Redo／Save／Complete、Delete、Arrow-key manipulation 與 keyboard-created objects。
- PrintScreen 仍是必要 global capture key；先前已確認的 Esc 仍是核心 capture-cancellation key，不視為 Annotation shortcut。
- Esc 在框選前、拖曳中及 SelectionLocked／Editing 階段直接取消整個 Capture Session；不採用 first-Esc transient editor hierarchy 作為 v1 需求。
- 基本 accessible names、非單靠顏色表示 selected／error state、一般文字輸入與 Chinese IME 仍屬 v1。
- `PRD-0004` 更新為 v1.4、`PRD-0005` v1.3、`PRD-0006` v1.4、`SPEC-0003` v1.4、`SPEC-0005` v1.3、`SPEC-0006` v1.3、`SPEC-0009` v1.2、`SPEC-0010` v1.3。
- `IMPLEMENTATION-CONTRACTS-001` 更新為 v2.3；Conformance Matrix 更新為 v2.3。
- Repository rules、Readiness Review、Freeze Review、Spec／Architecture Baseline Review、README、Architecture／PRD／Specs indexes、Roadmap、TODO、docs index、system overview 與 UI wireframe 已同步目前品質基線。
- 現行實作順序固定為 resident lifecycle／direct exit → PrintScreen → four-4K capacity／Frozen Virtual Desktop → cross-monitor Selection → pointer SelectionLocked adjustment → state graph → function bar／progress／focus → pointer Annotation → history／clipping → Complete → Save → failure／performance／accessibility → authorized Owner Reference／Standard／Maximum verification。
- Capture technical prototype維持 Selection 前只取得一次 frame，Selection 與 Crop 使用相同 Frozen Frame。
- 正常產品與 non-interactive tests 不啟動 Paint 或其他 external GUI fixture。
- 新增平台中立 `IPrintScreenTakeover`、`IPrintScreenTakeoverSettingsStore` 與 `ResidentLifecycleCoordinator`，集中處理 persisted takeover state、冪等註冊／解除、PrintScreen event boundary 與 application-exit cleanup。
- `SnipPlus.Windows` 新增 Windows `RegisterHotKey`／`UnregisterHotKey` implementation 與 `ApplicationData.Current.LocalSettings` persistence；註冊失敗時不保留 enabled state。
- MainWindow 新增 PrintScreen takeover checkbox；X 關閉時先釋放 takeover，再以 `Environment.Exit(0)` 結束 SnipPlus process。PrintScreen event 本次只到 application boundary，不啟動 `BeginCaptureAsync`。
- 新增 deterministic resident lifecycle tests，覆蓋設定載入、單次註冊／解除、失敗回復、event boundary 與 application-exit／Dispose idempotence。依本次任務限制，尚未執行測試。

### Verified — Historical Technical Foundation

- Locked restore、Release x64 build and packaged WinUI 3 build曾於早期 technical slice 成功。
- Non-interactive Unit／Contract／Rendering／Clipboard tests and categorized Windows platform tests曾記錄成功。
- One-display WGC、same-frame crop、BGRA8、PNG encoding、Win2D presentation and Clipboard publication曾以 synthetic 或 categorized evidence 驗證。
- Packaged synthetic checkerboard驗證的是已淘汰的 `Start Capture → one-display Selection → mouse release → immediate crop／Clipboard` 流程。

Historical evidence只適用於上述技術基礎，不證明 resident PrintScreen、four-4K capacity、cross-monitor Selection、pointer Annotation、quantitative performance、Save As 或 focus restoration conforming。

### Current Conformance Status

- Resident lifecycle、direct application exit and PrintScreen takeover：已加入 static implementation 與 deterministic tests；本次未執行 build／test／runtime，尚不宣稱已通過驗證。
- Four-4K capacity policy and unsupported-limit outcomes：Missing。
- Frozen Virtual Desktop and per-display frame ownership：Missing。
- Cross-monitor Selection and transparent gap output：Missing。
- Mouse-release-to-output：Incorrect／obsolete。
- Pointer Selection adjustment and accepted state graph：Missing／obsolete current model。
- Editing function bar and pointer-driven Annotation tools：Missing。
- Save As、Downloads default、PNG delivery and retained-file workflow：Missing。
- Performance／memory measurement evidence：Missing。
- Recoverable Editing preservation、stale-revision protection、baseline accessibility and focus restoration：Missing。
- Keyboard-only Annotation and non-PrintScreen shortcuts：Deferred，not a missing v1 capability。

Detailed status and required actions are maintained in `PRD/PRD-TRACEABILITY-MATRIX.md`.

### Not Released

- No release publication、Store deployment or release artifact submission has occurred。
- Existing packages were used only for development／runtime verification。
