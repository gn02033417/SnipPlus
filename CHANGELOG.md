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
- 新增 deterministic resident lifecycle tests，覆蓋設定載入、單次註冊／解除、失敗回復、event boundary 與 application-exit／Dispose idempotence；本次非互動測試已全部通過。

### Verified — Historical Technical Foundation

- Locked restore、Release x64 build and packaged WinUI 3 build曾於早期 technical slice 成功。
- Non-interactive Unit／Contract／Rendering／Clipboard tests and categorized Windows platform tests曾記錄成功。
- One-display WGC、same-frame crop、BGRA8、PNG encoding、Win2D presentation and Clipboard publication曾以 synthetic 或 categorized evidence 驗證。
- Packaged synthetic checkerboard驗證的是已淘汰的 `Start Capture → one-display Selection → mouse release → immediate crop／Clipboard` 流程。

Historical evidence只適用於上述技術基礎，不證明 resident PrintScreen、four-4K capacity、cross-monitor Selection、pointer Annotation、quantitative performance、Save As 或 focus restoration conforming。

### Verified — Resident Lifecycle Conformance Slice (2026-07-27)

- Locked restore 成功。
- Release x64 build 成功，0 warnings、0 errors。
- 非互動測試 45/45 通過，0 失敗、0 略過；`ResidentLifecycleCoordinatorTests` 10/10、`WindowsPrintScreenTakeoverTests` 2/2 均包含於完整測試結果。
- 本 Slice 7 個 C# 檔案的限定範圍 `dotnet format --verify-no-changes` 通過。
- 全 Repository formatting baseline 仍有 4,704 項既有問題，不屬於本 Slice，未予修正。
- Windows Runtime、真實 `RegisterHotKey`／`WM_HOTKEY`、跨程序設定還原及 MainWindow X 結束程序尚未執行驗證；本次嘗試因 packaged artifact 與目前 HEAD 不一致而阻擋。
- 本次未啟動 SnipPlus、Paint、Notepad 或其他外部 GUI，亦未執行 Interactive／Manual tests。

### Current Conformance Status

- Resident lifecycle、direct application exit and PrintScreen takeover：static implementation、locked restore、Release x64 build 與 deterministic non-interactive tests 已完成並通過；Windows Runtime verification 因 packaged artifact 與目前 HEAD 不一致而阻擋，仍不宣稱完全 Conforms。

### Windows Runtime Verification — Blocked (2026-07-27)

- Windows 11 x64、三個顯示器環境；只啟動已存在的 SnipPlus packaged runtime，未啟動外部 GUI。
- 已存在的 packaged runtime 可以開啟單一 MainWindow、保持 `Ready` 且未自動開始 Capture；但畫面與 accessibility tree 都沒有本 Slice 必須存在的 PrintScreen takeover checkbox。
- 唯讀 artifact 比對確認已安裝 package 與目前 HEAD `8b3ebe9` 的 Release build 不一致；直接啟動目前 build output 也未形成可觀察的 SnipPlus window。
- 舊 packaged artifact 已透過 MainWindow `X` 關閉，之後沒有留下 `SnipPlus.App` process。
- 因沒有可對應目前 HEAD 的 packaged runtime，B～H、真實 PrintScreen registration／release、跨程序設定還原與目前程式的 process-exit 行為均未執行，不能標示為通過。
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
