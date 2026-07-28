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

- Resident lifecycle、direct application exit and PrintScreen takeover：static implementation、locked restore、Release x64 build、deterministic non-interactive tests 與目前 HEAD 的 packaged Windows Runtime verification 已完成並通過；第一個 Slice 可標示為 Conforms。第二個 slice 已完成 PrintScreen／secondary request 到 `COMP-001` 的 `ResidentReady → CaptureRequested` 邊界。第三、第四個 slice 已完成四螢幕容量／Frozen Virtual Desktop、per-display frame ownership、Windows topology／WGC freezing integration，並以 Owner Reference 三螢幕完成 runtime verification。第五個 slice 已完成 Owner Reference packaged Overlay／Crosshair／initial cross-monitor Selection runtime verification；FR-006～FR-010 的 Maximum four-4K envelope 仍維持 Partial，Selection adjustment、Editing、output 與 Annotation 尚未開始。

### Verified — PrintScreen Capture Request Boundary Slice (2026-07-28)

- 建立平台中立 `CaptureRequest`、`CaptureRequestResult`、`CaptureRequestSource`、typed rejection reason 與 `ICaptureRequestBoundary` contracts。
- `WorkflowStateAuthority` 的正式初始狀態為 `ResidentReady`；唯一新增合法 transition 為 `ResidentReady → CaptureRequested`。本 slice 沒有進入 `Freezing`、Capture 或 Selection。
- PrintScreen `RequestId`／`ReceivedAt` 由 `PrintScreenReceivedEventArgs` 原值傳入 request；`SecondaryInAppCommand` 透過同一個 Core application boundary 進入。
- 第二個 active request 會回傳 typed `Busy`，保留第一個 request identity，且不產生第二次 transition。舊 `CaptureWorkflowCoordinator` 保留為技術程式碼，但在正式 `ResidentReady` authority 下無法繞過 request boundary。
- PrintScreen 與 Start Capture 都不呼叫 `BeginCaptureAsync`，request boundary 不持有 `ICaptureService`、Clipboard 或 PNG adapter。
- Locked restore 成功。
- Release x64 build 成功，0 warnings、0 errors。
- 非互動測試 50/50 通過，0 失敗、0 略過；包含新的 request／state／composition tests，以及 `ResidentLifecycleCoordinatorTests` 與 `WindowsPrintScreenTakeoverTests`。
- 本 slice 10 個 C# 檔案的限定範圍 `dotnet format --verify-no-changes` 通過。
- 本節的非互動驗證沒有啟動 SnipPlus、Paint、Notepad、Snipping Tool 或其他外部 GUI，也沒有執行 Interactive／Manual tests；current-HEAD packaged runtime evidence 另見下節。

### Verified — CaptureRequested to Freezing Foundation Slice (2026-07-28)

- 新增平台中立 `SupportedCapacityPolicy` 與 typed `CapacityValidationOutcome`，涵蓋 `1`–`4` displays、每個 `3840 × 2160`、總來源 `33,177,600` pixels、Virtual Desktop `16,384 × 16,384`、Selection `16,384 × 16,384` 與 `67,108,864` pixels 上限。
- 新增 immutable `VirtualDesktopSnapshot`／`DisplaySnapshot`，保留 negative coordinates、mixed DPI、arbitrary arrangement 與 `Transparent` gap policy；沒有把 gap rasterize 成假的 display，也沒有建立 giant bitmap。
- 新增 `CaptureSessionContext`、`FrozenDisplayFrame`、`FrozenDisplayFrameSet` 與 typed `CaptureFreezingCoordinator`；唯一新增正式 transition 為 `CaptureRequested → Freezing`，每個 accepted request 只建立一個 Session，且 frame set 是 per-display 資源的唯一 owner。
- deterministic tests 覆蓋容量邊界、invalid topology、negative／mixed-DPI／gap snapshot、session identity、stale／busy／cancelled／disposed request、frame duplicate／missing／unknown／mismatch、partial failure cleanup 與 idempotent disposal。
- `dotnet restore SnipPlus.sln --locked-mode` 成功。
- `dotnet build SnipPlus.sln -c Release -p:Platform=x64 --no-restore` 成功，0 warnings、0 errors。
- 非互動測試 70/70 通過，0 失敗、0 略過；Contracts 14/14、Core 41/41，Windows tests 包含於完整結果。
- 本 Slice 11 個實際修改 C# 檔案的限定範圍 `dotnet format --verify-no-changes --include ...` 通過；沒有修正全 Repository 既有 formatting baseline。
- 未執行 Windows multi-display topology／real WGC runtime、Overlay、Selection 或 `Freezing → Selecting`；本 Slice 沒有啟動 SnipPlus、Paint、Notepad、Snipping Tool 或其他外部 GUI，也沒有執行 Interactive／Manual tests。

### Verified — Windows Multi-display Freezing Integration Slice (2026-07-28)

- 新增 `WindowsDisplayTopologySource`／`WindowsDisplayTopologyProvider`：以 Windows `DisplayArea.FindAll()` 取得 active logical display surfaces，以 physical `OuterBounds` 建立 Virtual Desktop bounds，並讀取 monitor DPI X/Y 與 rotation／orientation；只使用 stable display identity，不把 monitor name、device path 或私人資料放入 failure evidence。
- 新增 `WindowsDisplayTopologyMapper`：將 Windows descriptors 轉成 platform-neutral `VirtualDesktopSnapshot`／`DisplaySnapshot`，保留 negative coordinates、mixed DPI、physical pixel size、arbitrary layout 與 deterministic coordinate version；mirrored logical surface 只保留一次，gap 不產生假的 display。
- 演進 `WindowsGraphicsCaptureAdapter`：每個 adapter 只擁有一個 display source、自己的 frame pool／capture session，支援 prepare → start → first-frame lifecycle；frame 驗證 session、display、coordinate version、physical bounds、pixel size，並產生 BGRA8 premultiplied、sRGB SDR canonical `SoftwareBitmap`，不包含 cursor、不 crop、不 merge。
- 新增 `WindowsFrozenDisplayFrameSetProvider` 與 `IAllDisplayFrameProvider`：capacity 已由 Core 在建立 Session 前驗證；所有 display adapters 先建立／prepare，再統一 start，之後並行收集每個 display 的第一個 frame。只有完整 `FrozenDisplayFrameSet` 才成功；partial failure、timeout、cancel、stale session、coordinate-version change 與 late frame 都會清理，不進入 `Selecting`。
- 新增 typed Windows integration outcomes，涵蓋 topology unavailable／invalid、unsupported capacity、capture unsupported／permission、source unavailable、frame timeout／size mismatch、display-context change、cancelled、stale session、partial acquisition 與 unexpected failure。
- `dotnet restore SnipPlus.sln --locked-mode` 成功。
- `dotnet build SnipPlus.sln -c Release -p:Platform=x64 --no-restore` 成功，0 warnings、0 errors。
- 非互動測試 83/83 通過，0 失敗、0 略過；新增 Core all-display boundary tests、Windows topology mapping tests 與 Windows frozen-frame-set orchestration／cleanup tests。
- 本 Slice 11 個實際修改 C# 檔案的限定範圍 `dotnet format --verify-no-changes --no-restore --include ...` 通過；全 Repository 既有 formatting baseline 未修正。
- 未執行真實三螢幕 topology／WGC runtime、Overlay、Frozen Canvas、Crosshair、Selection 或 `Freezing → Selecting`；本 Slice 未啟動 SnipPlus、Paint、Notepad、Snipping Tool 或其他外部 GUI，也未執行 Interactive／Manual tests。真實多螢幕 runtime verification 保留給本 Slice 完成後的明確授權步驟。

### Verified — Current-HEAD Windows Multi-display Freezing Runtime (2026-07-28)

- Runtime 從 clean fix commit `369281c73606a073fe9acaed6f1678eefade9972` 的 `SnipPlus.Windows.Tests` test host 執行；Windows App Runtime 2.3 bootstrap 成功。未經 MainWindow、未啟動 SnipPlus GUI、Paint、Notepad 或 Snipping Tool，亦未保存桌面像素、截圖或 Clipboard payload。
- Windows 11 x64 build `26200` 實際辨識 3 個 displays；Virtual Desktop bounds 為 `(-2560,0)–(2560,2520)`，保留負座標，`GapPolicy=Transparent`，連續 topology snapshot 的 `CoordinateVersion` 穩定。
- Owner Reference topology 實際解析為 primary `2560×1440`、DPI `1.00×1.00`，左側 `2560×1440`、DPI `1.00×1.00`，以及下方 `1920×1080`、DPI `1.50×1.50`、`LandscapeFlipped` 的 display；下方螢幕未再被誤判為 `1280×720` logical size。Capacity 為 `Supported`，Total Source Pixels 為 `9,446,400`。
- 真實 per-display WGC frame acquisition 通過：每個 session 取得完整 3-frame set，frame 數量／Display bounds／pixel size／SessionId／CoordinateVersion 一致；每個 frame 為 BGRA8、Premultiplied、sRGB SDR、`CursorIncluded=false`，不建立 giant bitmap、不 crop、不寫入 Clipboard、不建立 PNG。3 個獨立 session 均成功，CapturedAt 最大差分別為約 `47.57 ms`、`18.21 ms`、`33.49 ms`。
- Runtime 發現並修正兩個第四個 Slice 相容性問題：WinRT `DisplayArea.FindAll()` collection 改用 index access 避免 test host 的 `InvalidCastException (0x80004002)`；topology enumeration 暫時切換至 per-monitor DPI-aware context 後還原，取得正確 physical bounds／DPI。
- 真實 cancellation 在本機硬體於 `250 ms` 取消前已完成 frame acquisition，依規則記錄為 `Inconclusive`；deterministic cancellation／cleanup tests 仍保留。未進入 `Selecting`，未顯示 Frozen Canvas／Overlay／Crosshair／Selection。
- 修正後 Release x64 build 為 `0 warnings、0 errors`；非互動測試 `83/83` 通過、`0` 失敗、`0` 略過；兩個本 Slice C# 檔案限定範圍 `dotnet format --verify-no-changes` 通過。

### Verified — Current-HEAD Packaged Request Boundary Runtime (2026-07-28)

- 以 current HEAD `a94f8fd` 建立並部署 x64 Development MSIX；package Identity 為 `SnipPlus.App`、Version `1.0.0.0`，重裝後 installed `SnipPlus.App.dll` 與 Release x64 build 的 SHA-256 一致。
- Windows 11 x64 build `26200`、3 個 physical displays、單一可觀察的 SnipPlus MainWindow；啟動後沒有自動進入 Capture，設定與狀態文字一致。沒有啟動 Paint、Notepad 或其他外部 GUI fixture。
- A：啟用 takeover、關閉並重新啟動後，PrintScreen 顯示 request accepted；再次按鍵顯示 active request rejected／Busy，沒有進入 `Freezing`、`BeginCaptureAsync`、Capture Overlay、Selection、PNG 或 Clipboard。
- B：重新啟動後先按 Start Capture 顯示同一個 request accepted boundary；接著按 PrintScreen 顯示 Busy，沒有取代 active request。
- C：PrintScreen first 後再按 Start Capture 顯示 Busy，第一個 request 保持 active。
- D：停用 takeover 後 PrintScreen 沒有更新 SnipPlus 狀態；Start Capture 仍可進入同一個 `ResidentReady → CaptureRequested` boundary。停用後 Windows 自身曾顯示 `SnippingTool` overlay，並已在驗證後清理；該 overlay 不是 SnipPlus 啟動的 test fixture。
- E：在 `CaptureRequested` 狀態以 MainWindow `X` 關閉後，`SnipPlus.App` process 完全結束；重新啟動後設定仍為 disabled、狀態回到 resident ready，PrintScreen 沒有進入 SnipPlus boundary。
- 已核對沒有 hidden SnipPlus resident process；由停用 PrintScreen 產生的 Windows `SnippingTool` process 也已清理。未保存真實桌面截圖或 Clipboard payload。
- 部署期間發現同 Identity／Version 的舊 installed package 未被直接覆蓋；移除後重新安裝 current-HEAD package 並以 installed DLL hash 重新確認。未建立新 signing certificate；只使用既有本機開發憑證完成本機信任設定。

### Verified — Frozen Display Presentation and Initial Selection Static Slice (2026-07-28)

- 新增 platform-neutral presentation／selection contracts、`FrozenDisplayOverlayPlanBuilder`、`InitialSelectionCoordinator` 與 `CapturePresentationWorkflowCoordinator`；正式流程由 `CaptureRequested → Freezing` 接續 complete frozen frame set、all-display presentation、`Selecting`，並在有效 mouse release 後只經由 `COMP-001` 進入 `SelectionLocked`。
- MainWindow 的 Start Capture 與 PrintScreen request boundary 接入同一個 presentation coordinator；MainWindow 在任何 frame acquisition 前先進入 capture-source exclusion／hide，hide 失敗不進入 capture。正常產品入口不再呼叫舊的 `BeginCaptureAsync`／`CompleteSelectionAsync`，也不啟動 Clipboard、PNG 或其他外部 GUI。
- Windows overlay 以每個 display 一個 physical-bounds window 呈現自己的 frozen frame；所有 overlay surface 初始化完成前保持隱藏，使用 PMA V2、topmost／無 taskbar surface、dim-outside／clear-inside mask、全域 crosshair 與跨螢幕 normalized physical selection。沒有建立 giant bitmap 或變更 frozen frames。
- Esc 在 `Freezing`、`Selecting`、`SelectionLocked` 進入取消清理；frame set、session、overlay 與 source exclusion 均具明確 cleanup／idempotence，完成後回到 `ResidentReady`，不自動重新顯示 MainWindow。
- `dotnet restore SnipPlus.sln --locked-mode` 成功；Release x64 build 成功，0 warnings、0 errors；非互動測試 94/94 通過，0 失敗、0 略過。
- 本 Slice 19 個 C# 檔案的限定範圍 `dotnet format --verify-no-changes --no-restore --include ...` 通過，`git diff --check` 通過；未修正全 Repository 的既有 formatting baseline。
- 本 Slice 尚未執行 packaged Windows Overlay／Frozen Canvas／Crosshair／Selection runtime、真實 pointer interaction 或 topology-change runtime；未啟動 SnipPlus GUI、Paint、Notepad、Snipping Tool 或其他外部 GUI，亦未執行 Interactive／Manual tests。

### Windows Runtime Verification — Fifth Slice Initial Attempt Blocked by Capture Access (2026-07-28)

- 以目前工作樹建立並部署 x64 Development MSIX；Identity `SnipPlus.App`、Version `1.0.0.0` 未變更。安裝後 manifest 實際包含 `uap11:Capability Name="graphicsCaptureProgrammatic"` 與 `runFullTrust`，簽章驗證成功。
- 修正 packaged WGC compatibility issue：`graphicsCaptureProgrammatic` 原先誤放在 `rescap:Capability`，已改為 Windows manifest schema 要求的 `uap11:Capability`；保留 `GraphicsCaptureAccess.RequestAccessAsync(Programmatic)` preflight，並在 MainWindow 隱藏前執行。
- Locked restore 成功；Release x64 build 成功，0 warnings、0 errors；非互動測試 94/94 通過，0 失敗、0 略過；本 Slice C# 窄範圍 `dotnet format --verify-no-changes --no-restore --include ...` 通過。
- Windows 11 x64 build `26200`、Owner Reference 三螢幕環境中，clean packaged Start Capture 在 WGC preflight 明確回報 `DeniedByUser`；MainWindow 未被隱藏，沒有建立 overlay、Frozen Canvas、Crosshair、Frozen frame set 或 Selection。實際 PrintScreen 嘗試也未形成可觀察 overlay，最後狀態為 bounded preflight `TimeoutException`；不能標示 WGC frame、Overlay 或 Selection runtime 通過。
- 未啟動 Paint、Notepad、Snipping Tool 或其他外部 GUI；未保存真實桌面截圖、frozen frame 或 Clipboard payload。測試後 `SnipPlus.App` process 已清理。
- 第五個 Slice 仍為 `Partial／Runtime blocked by DeniedByUser`；未開始第六個 Slice，也未進入 Selection adjustment、Function Bar、Annotation、Clipboard、PNG 或其他後續功能。

### Verified — Fifth Slice Packaged Overlay, Crosshair and Initial Selection Runtime (2026-07-28)

- 以 clean commits `8e1b174`、`438467e` 重建並部署 x64 Development MSIX；PackageFullName `SnipPlus.App_1.0.0.0_x64__26728c12bvz0c`、PackageFamilyName `SnipPlus.App_26728c12bvz0c`，package signature `Valid`。Release 與 installed `SnipPlus.App.dll` SHA-256 均為 `9586B1913BEB3A10822180395048A1262CDAEFCD206B9FBE13360D5B60F3DDF8`。
- Installed manifest 實際包含 `uap11:Capability Name="graphicsCaptureProgrammatic"` 與 `runFullTrust`；Windows 11 x64 build `26200` 的 Owner Reference 三螢幕 WGC preflight 實際回傳 `Allowed`。Solution locked restore 成功；Release x64 solution build 為 `0 warnings、0 errors`；packaged MSIX build 為 `0 errors`，僅有既有 `mspdbcmf.exe` symbols warning；非互動測試 `94/94` 通過，`0` 失敗、`0` 略過；修改檔案的限定範圍 format 通過。
- Runtime display topology 為 Primary `(0,0)–(2560,1440)` @ `96 DPI`、Left `(-2560,0)–(0,1440)` @ `96 DPI`、Lower `(300,1440)–(2220,2520)`、physical `1920×1080` @ `144 DPI`（1.5）。MainWindow 在 capture 前可見，三個 overlay ready 後同一 HWND 已隱藏。
- `Start Capture` sampling 為 `0x12 → 3x88`；實際 PrintScreen sampling 為 `0x13 → 3x87`。兩者均沒有 `1` 或 `2` 個 display-sized overlay；三個 overlay 均覆蓋各自 physical bounds，沒有建立 giant bitmap。Pointer crosshair／mask／selection source path 在五組單螢幕及跨螢幕拖曳中實際運作；未保存桌面 screenshot 或 frozen frame，因此沒有提交任何私人像素證據。
- Primary 正向／反向、Left↔Primary、Primary↔Lower 150%、Left↔Lower 跨 Gap 全部在 mouse release 後保持 `SelectionLocked`，沒有 crop、PNG 或 Clipboard sequence 變更。Esc 在 Drag 前、Drag 中及 Lower display focus 後均關閉所有 overlay；SelectionLocked 後的 Esc cleanup 亦通過。沒有 Function Bar、Resize Handles 或 Editing transition。
- 正常流程未啟動 Paint、Notepad、Snipping Tool 或其他 GUI fixture；targeted scan 只觀察到兩個本次之前已存在且無視窗標題的 `SnippingTool` background processes，未由本次 workflow 啟動或操作。第六個 Slice 未開始；FR-012、Gap Rasterization、Function Bar、Annotation、Clipboard、PNG 與後續 output 仍未完成。

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

### Verified — Current-HEAD Packaged Windows Runtime (2026-07-27)

- 以 current HEAD `ede6933` 建立 x64 Development MSIX；package generation 成功，`mspdbcmf.exe` symbols warning 1 項、0 errors。未修改 Package Identity 或 Version。
- 既有開發憑證缺少 MSIX 所需的 Code Signing EKU；本次只在目前使用者／本機部署範圍建立 self-signed Code Signing development certificate，未建立或分享正式 signing certificate，未提交憑證檔。
- 目前 HEAD package 已註冊並安裝；installed `SnipPlus.App.dll` 與 current Release x64 build 的 SHA-256 相同。Runtime 使用 Windows 11 x64 build `26200`、3 displays；只啟動 SnipPlus MainWindow。
- A：啟動後只有 MainWindow、沒有自動 Capture，初始設定為 disabled，UI 狀態一致。
- B：啟用後實際 PrintScreen 收到 application event boundary；狀態為 `PrintScreen received. Capture workflow is not started in this slice.`，未啟動 `BeginCaptureAsync`、Capture Overlay、Selection UI、PNG 或 Clipboard。
- C：以停用後重新啟用 cycle 驗證 registration 可再次建立；直接重複 enable 的冪等性由 deterministic tests 覆蓋，UI 沒有獨立的第二個 Enable 命令。
- D：停用後實際 PrintScreen 不再更新 SnipPlus 狀態。
- E／F：disabled 與 enabled 設定在 MainWindow X、process 結束及重新啟動後分別正確還原；enabled restart 後實際 PrintScreen 再次收到一次 boundary event。
- G／H：MainWindow X 後 `SnipPlus.App` process 消失，沒有 hidden resident process；停用後退出及重複 cleanup 沒有未處理例外。退出後未保留 SnipPlus 接收事件的 process。
- Runtime verification 期間未啟動 Paint、Notepad、Snipping Tool 或其他外部 GUI fixture；未保存真實桌面截圖或 Clipboard payload。既存的 Windows overlay 曾短暫影響 UI hit-test，但未被啟動或操作。後續本機憑證 cleanup 曾意外開啟 certutil utility，已立即停止，未參與產品驗證。

### Not Released

- No release publication、Store deployment or release artifact submission has occurred。
- Existing packages were used only for development／runtime verification。
