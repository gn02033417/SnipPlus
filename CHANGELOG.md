﻿﻿# Changelog

本檔案依 Keep a Changelog 精神記錄對使用者、維護者與文件治理有意義的變更。尚未有產品版本發布。

## [Unreleased]

### Added — Stage 7C Pointer-driven Arrow／Line Creation (2026-07-31)

- Added platform-neutral `ArrowLine` editing-tool selection, immutable Arrow／Line end-style／color／thickness contracts, physical line-segment content, revision-aware pointer requests／outcomes and presentation snapshot state. `Arrow` and no-arrow `Line` are one tool with an explicit end-style mode.
- Added Core-owned Arrow／Line draft／commit routing through `AnnotationEditingCoordinator` and `CapturePresentationWorkflowCoordinator`; pointer release commits one immutable `ArrowLine` object with exact Frozen Virtual Desktop endpoints, deterministic object identity／Z-order and stale-session／Selection／Annotation revision rejection.
- Added Function Bar `Arrow / Line` selection plus `Arrow`／`Line` mode controls. Windows overlays render committed and draft line segments per display, clip them to the current Selection／display intersection, and draw an arrowhead only for Arrow mode without creating additional top-level windows.
- Added deterministic Contracts／Core／Workflow／Windows clipping coverage. Locked restore and Release x64 build succeeded with `0` warnings／`0` errors; non-interactive tests `178/178` passed with `0` failures／`0` skips. No MSIX、Publish、install、GUI、Interactive／Manual runtime verification or real desktop／Clipboard payload was used in this slice.

### Added — Stage 7B Pointer-driven Rectangle Creation (2026-07-31)

- Added platform-neutral `EditingToolKind.Selection`／`Rectangle`, immutable typed Rectangle style／content, tool-selection requests, revision-aware Rectangle pointer requests／outcomes and immutable annotation presentation snapshots. Selection remains outside `AnnotationToolKind`.
- Added Core-owned `AnnotationEditingCoordinator`: Editing starts in Selection mode; Rectangle press／move／release validates Session／CoordinateVersion／SelectionRevision／AnnotationRevision／PointerId／Selection bounds, keeps a session-owned draft without mutating the document, normalizes reverse physical coordinates, commits one Rectangle object with deterministic Core-owned ID／Z-order／default style and clears the draft. Stale、outside、invalid、pointer-mismatch、no-draft and overflow cases are typed outcomes; selection adjustment changes clipping only.
- Added Function Bar Selection／Rectangle RadioButton controls with accessible names and non-color-only selected state. Windows per-display overlays render committed and draft Rectangle previews using physical display intersection and Selection clipping only; underlying Annotation geometry remains unchanged and no new top-level window is created.
- Added Stage 6C safety coverage: an empty Annotation Document keeps the existing Complete／render／Clipboard path, while a non-empty document stays in Editing with typed feedback and does not execute base-only render or Clipboard delivery. No Annotation-aware final output was claimed.
- Locked restore and Release x64 build succeeded with `0` warnings／`0` errors; non-interactive tests `173/173` passed with `0` failures／`0` skips. No MSIX、Publish、install、GUI、Interactive／Manual runtime verification or real desktop／Clipboard payload was used in this slice. Limited formatting verification and final Git evidence are recorded after this entry is committed.

### Added — Stage 7A Annotation Document Foundation (2026-07-30)

- Added platform-neutral `AnnotationObjectId`、`AnnotationRevision`、accepted `AnnotationToolKind` values、physical-pixel `AnnotationObject` geometry and immutable `AnnotationDocument` contracts. Object collections are read-only、session-bound and deterministically ordered by Z-order.
- Added Core-owned `AnnotationDocumentCoordinator` with one current document per Capture Session、typed Add／Replace／Remove outcomes、stale Session／Annotation Revision rejection、duplicate／missing／invalid／no-change outcomes and explicit revision-overflow handling.
- Integrated the document lifecycle with the existing `CapturePresentationWorkflowCoordinator`: an empty document is created once at `SelectionLocked → Editing`; Selection move／resize／reselection does not alter Annotation geometry or revision; recoverable Complete failure retains the document; successful Complete、Cancel、Esc、terminal failure and Dispose clear ownership; a new Session receives a new document.
- Added deterministic Contracts／Core tests for identity、tool values、validation、immutable collections、mutation revisions／ordering／typed failures、Selection adjustment separation、recoverable failure retention、successful cleanup and new-Session ownership. No Annotation tool、style、render、Undo／Redo、Save or UI was added.
- Locked restore succeeded. Release x64 solution build succeeded with `0 warnings／0 errors`; non-interactive tests `155/155` passed with `0 failures／0 skips`; the six modified C# files passed limited `dotnet format --verify-no-changes --no-restore --include ...`; `git diff --check` passed. No MSIX、Publish、install、GUI、interactive／manual runtime or real desktop／Clipboard payload was used in this slice.

### Verified — Stage 6C Complete Clipboard Runtime (2026-07-30)

- Repository owner confirmed the current installed Development MSIX completed a selection and copied it to Clipboard; pasting produced the exact selected position／content.
- The packaged trace reached `SetContentAfter`, initially encountered bounded Clipboard contention at `Flush` (`0x800401D0`) on attempts 1–3, then reached `FlushAfter` on attempt 4 and returned through the completed workflow to `ResidentReady`.
- This is runtime evidence for the current Owner Reference Complete／paste scenario. Broader Selection adjustment cases、four-4K envelope、performance、Save and Annotation remain outside this evidence; Stage 7 was not started. No desktop screenshot or Clipboard payload was saved.

### Corrected — Stage 6C Clipboard STA Runtime Boundary (2026-07-30)

- The latest packaged trace reached the Clipboard dispatcher but `RoInitialize(RO_INIT_MULTITHREADED)` failed before `DataPackage` creation with `0x80010106 (RPC_E_CHANGED_MODE)`. This confirms the synchronous WinUI entrypoint now reaches the existing STA boundary; the MTA initializer was the new direct failure.
- `WindowsClipboardRuntimeInitializer` now requests `RO_INIT_SINGLETHREADED`, which is compatible with the WinUI dispatcher STA, and keeps the existing balanced `RoUninitialize()` cleanup scope.
- Locked restore succeeded、Release x64 build succeeded with `0 warnings／0 errors`、all non-interactive tests `142/142` passed、and modified-file format verification passed. The new signed Development MSIX requires another packaged Complete retry; Clipboard runtime success is not yet claimed.

### Fixed — Stage 6C WinUI Entry-Point COM Boundary (2026-07-30)

- The latest packaged trace reached `RuntimeInitializationAfter`, completed `DataPackage` construction and failed only at `Clipboard.SetContentWithOptions()` with `0x800401F0 (CO_E_NOTINITIALIZED)`; Render and PNG encoding had already succeeded.
- Replaced the custom WinUI `async Main` with a synchronous `[STAThread] Main`. Secondary `AppInstance` activation redirection now runs on a worker thread and is awaited through `CoWaitForMultipleObjects`, preserving single-instance routing without blocking the WinUI STA or changing the Clipboard payload／output contract.
- Locked restore succeeded、Release x64 build succeeded with `0 warnings／0 errors`、all non-interactive tests `142/142` passed、and the modified-file format verification passed. The newly signed Development MSIX requires one packaged Complete retry; Clipboard runtime success is not yet claimed.

### Fixed — Stage 6C Clipboard Payload Runtime Boundary (2026-07-30)

- The MTA correction reached `RuntimeInitializationAfter`, but the next packaged trace still failed at `Clipboard.SetContentWithOptions()` with `0x800401F0 (CO_E_NOTINITIALIZED)`; `Flush()` was not reached.
- Moved `InMemoryRandomAccessStream.Seek`、`DataPackage`、`RandomAccessStreamReference`、`SetBitmap` and `ClipboardContentOptions` creation into the same dispatcher／`RoInitialize(RO_INIT_MULTITHREADED)` scope as `SetContent`／`Flush`.
- Added deterministic payload-boundary trace ordering coverage. Release x64 build remains `0 warnings／0 errors` and non-interactive tests remain `142/142`; the new signed package still requires one packaged Complete retry.

### Corrected — Stage 6C Clipboard Runtime Apartment Mode (2026-07-30)

- The first post-fix packaged trace showed `RuntimeInitializationException` with `0x80010106 (RPC_E_CHANGED_MODE)` before `SetContent`; the current dispatcher thread was already using a different apartment mode.
- `WindowsClipboardRuntimeInitializer` now uses `RoInitialize(RO_INIT_MULTITHREADED)`, matching the actual packaged dispatcher thread and the Windows Runtime initialization contract. `RoUninitialize()` remains balanced through the existing scope.
- This correction still requires one packaged Complete retry; no success is claimed until the new trace reaches `SetContentAfter` and `FlushAfter` or records the next concrete failure.

### Fixed — Stage 6C Clipboard WinRT Apartment Initialization (2026-07-30)

- Repository owner packaged trace confirmed `Clipboard.SetContentWithOptions()` failed directly with `0x800401F0 (CO_E_NOTINITIALIZED)` after Render／PNG succeeded; `Flush()` was not reached. The failure was not dispatcher enqueue or Clipboard contention.
- `WindowsClipboardRuntimeInitializer` now calls `RoInitialize(RO_INIT_SINGLETHREADED)` at the actual Clipboard publication boundary and balances it with `RoUninitialize()` after `SetContent`／`Flush`; the production platform resource injects this initializer without changing `COMP-001`, retry policy, Clipboard history／roaming defaults or output semantics.
- Added deterministic ordering coverage for runtime initialization、`SetContent`、`Flush` and cleanup. Locked restore succeeded、Release x64 build succeeded with `0 warnings／0 errors`、non-interactive tests `142/142` passed、modified-file format verification and `git diff --check` passed.
- The corrected signed packaged MSIX was installed for the next Repository owner Complete retry; post-fix Clipboard／paste runtime acceptance remains pending. No desktop screenshot or Clipboard payload was saved, and Stage 7 was not started.

### Added — Stage 6C Clipboard Boundary Diagnostics (2026-07-30)

- Repository owner 重試後的 packaged trace 仍顯示 Render／PNG 成功，Clipboard 在 `0x800401F0 (CO_E_NOTINITIALIZED)` 失敗；同時確認 installed `SnipPlus.App.dll`／`SnipPlus.Windows.dll` 與上一個 artifact 一致，因此新增更細的流程診斷。
- Complete trace 現在記錄 managed thread id、dispatcher availability／`HasThreadAccess`、enqueue result、callback entry、`SetContent` 前後、`Flush` 前後、exception type 與 HRESULT；診斷不記錄桌面像素、Clipboard payload、私人路徑或視窗資料。
- 新增 deterministic trace assertions；Release x64 build 0 warnings／0 errors、非互動 tests `141/141` 通過，限定範圍 format verify 與 `git diff --check` 通過。修正後 artifact 仍須重新執行 packaged Complete 才能判定下一個根因。

### Fixed — Stage 6C Clipboard COM Dispatcher Boundary (2026-07-30)

- Packaged `stage6c-complete-failure.jsonl` 已確認 Render、結果驗證與 PNG encoding 成功，Clipboard publication 以 `0x800401F0` (`CO_E_NOTINITIALIZED`) 失敗；根因是 WinRT Clipboard 呼叫在未初始化 COM 的 workflow thread 執行，不是 crop、render 或 PNG 內容錯誤。
- `SnipPlus.Windows` 新增 `IClipboardDeliveryDispatcher`／`DispatcherQueueClipboardDeliveryDispatcher`；`MainWindow` 將 WinUI `DispatcherQueue` 注入 Clipboard adapter，確保 `DataPackage` publication 與必要的 `Clipboard.Flush()` 在 UI／COM dispatcher boundary 執行。既有 bounded retry、cancellation、Clipboard History／roaming 禁用與 typed failure 不變，未以假成功取代失敗。
- 新增 dispatcher unavailable 與 caller 不具 thread access 的 deterministic tests；Release x64 build 0 warnings／0 errors、非互動 tests `141/141` 通過，限定範圍 C# format verify 與 `git diff --check` 通過。
- 修正後尚未重新執行 Repository owner 的 packaged Complete／實際貼上驗收；Stage 6C 仍為 `Partial／post-fix packaged Clipboard verification pending`。未啟動 Paint、Notepad 或其他外部 GUI，未保存桌面截圖或 Clipboard payload，Stage 7 未開始。

### Corrected — Stage 6C Exact WinRT Clipboard Call Boundary (2026-07-30)

- Repository owner 使用上一個修正版 artifact 重試後，trace 仍確認 Render／PNG 成功、`ClipboardPublicationRejected` 與 `0x800401F0 (CO_E_NOTINITIALIZED)`；installed `SnipPlus.App.dll`／`SnipPlus.Windows.dll` 與 artifact 一致，因此排除 artifact mismatch。
- 前一版只將整個 async delivery 啟動到 dispatcher；`await PngEncoder` 後仍可能在其他 thread 繼續。現在只有實際的同步 `Clipboard.SetContentWithOptions()` 與 `Clipboard.Flush()` 會在 dispatcher callback 內執行，確保 WinRT Clipboard 操作不會落到未初始化 COM 的 thread。
- Build 0 warnings／0 errors、非互動 tests `141/141` 通過、限定範圍 format verify 與 `git diff --check` 通過。修正後 packaged Complete／實際貼上仍待重新驗收，Stage 6C 維持 Partial。

### Corrected — Stage 6C Complete Failure Trace and Recovery (2026-07-30)

- Repository owner 使用已確認一致的 Release／MSIX／Installed `SnipPlus.App.dll` Artifact 重現了 Complete 後 Function Bar 立即消失、Overlay 保留、Clipboard 不變，且移動 Pointer 後 Function Bar 才重新出現的問題；本次不再把 Artifact mismatch 或先前偶發的 startup crash 當作 Complete 根因。
- 已確認 Function Bar 缺陷根因：`ReturnToEditing` 只呼叫 `Reposition`，而 Windows `PrepareOrReposition` 會先將 hosted Function Bar 設為不可見；Pointer movement 後的 selection update 才會另外呼叫 `Show`。現在 failure recovery 會先清除 `_completeInProgress`，返回 `Editing`，再用最新 `SelectionRevision` 執行 Stage 6C `Reposition`、`Show`，並在 Capture UI 顯示可理解的 Render／Clipboard failure feedback；不關閉 Overlay、Session 或 FrozenDisplayFrameSet。
- 新增平台中立 `CompleteExecutionStage`／`CompleteExecutionTraceEntry`／`ICompleteExecutionTraceSink`，以及 packaged App `LocalCache\Diagnostics\stage6c-complete-failure.jsonl` sink。Trace 僅記錄 stage、typed `FailureCode`／category、HRESULT、component、workflow state、session／revision、selection／result 尺寸、display count 與 Clipboard attempt；不記錄桌面像素、Clipboard payload、視窗標題或私人路徑，Trace 寫入失敗不影響產品流程。
- 新增 deterministic recovery／trace assertions；locked restore 成功、Release x64 solution build 成功（`0 warnings、0 errors`）、非互動測試 `139/139` 通過（`0 failures、0 skips`）、本次修改 C# 檔案限定範圍 format verify 與 `git diff --check` 通過。
- Source implementation commit 為 `8eec140919c6871c6ead50a522f42b0d54bea4a0`；matching Development MSIX 以同一 source code 重新產生並重新安裝。PackageFullName 為 `SnipPlus.App_1.0.0.0_x64__26728c12bvz0c`，Release／package／installed DLL SHA-256 均為 `2C06F5E9DCB1166216FB4AD88D631197B3D276D86EC5C375149F63F0FDC85DA3`，簽署後 MSIX SHA-256 為 `C0FBC90C4AF45F364E80D55E9ED01C0DA4E57D96524865774AAD143F12FA3A10`；最終不可覆寫 Artifact 已保存於 `D:\MEGA\SnipPlusArtifacts\Stage6C\deed1740f34d34d0f830194480558d414eabcefc`，先前 Artifact 亦未覆寫。
- 本次未取得 Repository owner 實際按下 Complete 後的 packaged JSONL failure stage，因此尚未宣稱 Render、Result validation、PNG encoding 或 Clipboard 的實際 runtime root cause；Stage 6C 維持 `Partial／packaged failure-stage verification pending`。未啟動 Paint、Notepad 或其他外部 GUI，未保存桌面截圖或 Clipboard payload，Stage 7 未開始。

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

- Resident lifecycle、direct application exit and PrintScreen takeover：Stage 5 source correction、locked restore、Release x64 build、limited format verification、deterministic non-interactive tests and Repository owner Owner Reference manual acceptance passed, with the documented Windows native PrintScreen compatibility prerequisite. 第二個 slice 已完成 PrintScreen／secondary request 到 `COMP-001` 的 `ResidentReady → CaptureRequested` 邊界。第三、第四個 slice 已完成四螢幕容量／Frozen Virtual Desktop、per-display frame ownership、Windows topology／WGC freezing integration，並以 Owner Reference 三螢幕完成 runtime verification。第五個 slice 已通過 Owner Reference 的 Frozen Overlay／Crosshair／initial Selection acceptance；FR-006～FR-010 的 Maximum four-4K envelope 仍維持 Partial，Selection adjustment、Editing、output 與 Annotation 尚未開始。

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

- 以目前工作樹建立並部署 x64 Development MSIX；Identity `SnipPlus.App`、Version `1.0.0.0` 未變更。安裝後 manifest 實際包含 `uap11:Capability Name="graphicsCaptureProgrammatic"` 與
unFullTrust`，簽章驗證成功。
- 修正 packaged WGC compatibility issue：`graphicsCaptureProgrammatic` 原先誤放在
escap:Capability`，已改為 Windows manifest schema 要求的 `uap11:Capability`；保留 `GraphicsCaptureAccess.RequestAccessAsync(Programmatic)` preflight，並在 MainWindow 隱藏前執行。
- Locked restore 成功；Release x64 build 成功，0 warnings、0 errors；非互動測試 94/94 通過，0 失敗、0 略過；本 Slice C# 窄範圍 `dotnet format --verify-no-changes --no-restore --include ...` 通過。
- Windows 11 x64 build `26200`、Owner Reference 三螢幕環境中，clean packaged Start Capture 在 WGC preflight 明確回報 `DeniedByUser`；MainWindow 未被隱藏，沒有建立 overlay、Frozen Canvas、Crosshair、Frozen frame set 或 Selection。實際 PrintScreen 嘗試也未形成可觀察 overlay，最後狀態為 bounded preflight `TimeoutException`；不能標示 WGC frame、Overlay 或 Selection runtime 通過。
- 未啟動 Paint、Notepad、Snipping Tool 或其他外部 GUI；未保存真實桌面截圖、frozen frame 或 Clipboard payload。測試後 `SnipPlus.App` process 已清理。
- 第五個 Slice 仍為 `Partial／Runtime blocked by DeniedByUser`；未開始第六個 Slice，也未進入 Selection adjustment、Function Bar、Annotation、Clipboard、PNG 或其他後續功能。

### Reclassified — Fifth Slice Packaged Overlay, Crosshair and Initial Selection Runtime (2026-07-28)

- Earlier packaged runtime evidence covered three physical overlays, frozen-frame presentation, mask, cross-display drag and `SelectionLocked`, but it is not accepted as final Crosshair conformance.
- Repository owner manual acceptance found that the implementation rendered two XAML crosshair `Line` elements per overlay. The result showed three visual crosses simultaneously and did not behave as one pointer-following system cursor.
- The earlier result is therefore retained as historical runtime evidence only. The fifth Slice remains `Partial`; the sixth Slice has not started.

### Corrected — Fifth Slice System Crosshair Cursor and Manual Acceptance Package (2026-07-28)

- Removed the per-overlay XAML crosshair lines and their `SelectionVisualState.CurrentPhysicalPoint` positioning path. Each overlay input surface now uses one WinUI `InputSystemCursorShape.Cross` via `ProtectedCursor`; disposal clears the cursor without installing a global hook or affecting other applications.
- Added deterministic coverage proving idle pointer movement leaves `SelectionStatus.None`, `SelectionRevision` and the selection state unchanged. Existing drag, cross-display, release-to-`SelectionLocked`, cancellation and no-output tests remain in the non-interactive suite.
- Locked restore succeeded; Release x64 solution build succeeded with `0 warnings、0 errors`; non-interactive tests `96/96` passed with `0` failures and `0` skips; changed-file-only format verification and `git diff --check` passed.
- Development MSIX was built from clean fix commit `03716d39dfe1517cc43712ff5e4505ae8fb5472b`; package signature was `Valid`. PackageFullName remained `SnipPlus.App_1.0.0.0_x64__26728c12bvz0c` and PackageFamilyName remained `SnipPlus.App_26728c12bvz0c`. Release and installed `SnipPlus.App.dll` SHA-256 both equal `A2279DD57D3B6AEC17AEFA5A98324A22A192C4196CC28922B4EF47E0FEE0B0A0`.
- For manual acceptance, `Package.appxmanifest` changed `uap:VisualElements AppListEntry` from `none` to `default`. The rebuilt and reinstalled package is `Ok` and is now registered in Windows `StartApps` as `SnipPlus.App_26728c12bvz0c!App`.
- The Development MSIX is prepared for Repository owner manual acceptance. No SnipPlus GUI, Paint, Notepad, Snipping Tool or other external GUI was launched in this correction; no desktop screenshot, frozen frame or Clipboard payload was saved. Fifth Slice status is `Implementation and automated verification passed; Repository owner manual acceptance pending`; Selection adjustment, Function Bar, Annotation, Clipboard, PNG and the sixth Slice remain unstarted.

### Corrected — Stage 5 Esc Resident Lifecycle Boundary (2026-07-29)

- Repository owner manual acceptance confirmed the three-display Frozen Overlay、mask、single system Crosshair、single／cross-display initial Selection、`SelectionLocked` without output and the secondary Start Capture entry. It found two failures: Esc terminated the SnipPlus process, and PrintScreen could not be re-used from another application after that termination.
- Root cause identified in the Windows overlay input path: `OnKeyDown` synchronously entered the Core cancellation boundary while the focused overlay was still handling `KeyDown`; the existing cleanup then synchronously called `Window.Close()` on the overlay windows. Esc now queues the same `ISelectionInputSink.Escape` boundary through the overlay `DispatcherQueue`, so the key callback returns before session cleanup closes the overlays. No PrintScreen registration architecture or capture workflow was rewritten.
- Added deterministic Core coverage that repeated Esc／late cancellation is safe, returns to `ResidentReady`, disposes the cancelled Session once and accepts a subsequent PrintScreen request.
- `dotnet restore SnipPlus.sln --locked-mode` succeeded; Release x64 build succeeded with `0 warnings、0 errors`; non-interactive tests `97/97` passed with `0` failures and `0` skips; changed-file-only format verification and `git diff --check` passed.
- From clean commit `21a49b0bfd04584e3c9d5e936100fe1fe3fc2e2b`, `src/SnipPlus.App/AppPackages/SnipPlus.App_1.0.0.0_x64_Test/SnipPlus.App_1.0.0.0_x64.msix` was generated with one pre-existing `mspdbcmf.exe` symbols warning and `0` errors, then locally development-signed with a valid signature. The old exact same-version package was removed because Windows rejected a content-different reinstall (`0x80073CFB`); the clean package was installed as `SnipPlus.App_1.0.0.0_x64__26728c12bvz0c`, status `Ok`, and registered in Start menu as `SnipPlus`.
- Release and installed `SnipPlus.App.dll` SHA-256 both equal `3E73CDB16BA7775EA8C683506A31760FED4F81C8677FBC792B2AC31D058AE2C8`. Deployment ended with `0` running `SnipPlus.App` processes. This is package/artifact evidence only; no post-fix interactive runtime behavior is claimed.
- The corrected source is ready for Repository owner re-acceptance of five items: background-focus PrintScreen, Esc overlay-only cleanup with resident process, no MainWindow reopening, post-cancel background-focus PrintScreen and MainWindow X full exit. No Paint、Notepad、Snipping Tool or other external GUI was launched, and no desktop screenshot、frozen frame or Clipboard payload was saved.
- Stage 5 remains `Implementation and automated verification passed; Repository owner five-item manual re-acceptance pending`; Selection adjustment, Function Bar, Annotation, Clipboard, PNG and the sixth Slice remain unstarted.

### Corrected — Stage 5 Background PrintScreen, Pre-drag Esc and Single-instance Activation (2026-07-29)

- Repository owner manual acceptance reported three defects: background-focus PrintScreen started the Windows native capture surface instead of reaching SnipPlus; Esc before any pointer interaction did not close the overlays; and repeated Start-menu activation created additional `SnipPlus.App.exe` processes instead of activating the existing resident MainWindow. Drag Esc、SelectionLocked Esc、Crosshair、single／cross-display Selection and MainWindow X were previously reported as passing and remain regression boundaries.
- Root causes were the MainWindow HWND being used as the resident PrintScreen owner, Esc being available only through a focused overlay Canvas `KeyDown`, and the generated XAML entry point creating a new MainWindow on every process activation.
- `SnipPlus.App` now uses a custom `Program.Main` with `WinRT.ComWrappersSupport.InitializeComWrappers()`, fixed `AppInstance` key `SnipPlus.Main`, `FindOrRegisterForKey`、`IsCurrent`、`RedirectActivationToAsync` and `Activated`; secondary processes redirect before XAML initialization and exit without creating MainWindow、resident lifecycle or a second hotkey registration. Resident activation shows and activates the existing MainWindow only in `ResidentReady`; active capture and application exit ignore activation.
- `WindowsPrintScreenTakeover` now owns a process-lifetime message-only HWND for `RegisterHotKey`／`WM_HOTKEY`, independent of MainWindow visibility. The previous idempotent registration／release and application-exit cleanup boundaries remain in use. No keyboard hook, Registry change or Windows native PrintScreen setting change was added.
- Overlay batch presentation now assigns one session Escape focus owner after the atomic show; all overlays still share the same Core cancellation boundary, while pointer interaction can transfer focus to the active surface. No permanent global Escape hook was added.
- Locked restore succeeded. Release x64 build succeeded with `0 warnings、0 errors`. Non-interactive tests `101/101` passed with `0` failures and `0` skips, including `ResidentActivationBoundaryTests` and the application-owned message-window contract test. Limited changed-file `dotnet format --verify-no-changes --no-restore --include ...` passed; `git diff --check` passed.
- From clean commit `701ea21a466795c89af8f7b47feb2f3769fac9fd`, Development MSIX was built at `src/SnipPlus.App/AppPackages/SnipPlus.App_1.0.0.0_x64_Test/SnipPlus.App_1.0.0.0_x64.msix`; packaging emitted the existing `mspdbcmf.exe` symbols warning and `0` errors. Release and packaged `SnipPlus.App.dll` SHA-256 both equal `70C8E7D84DCA2496FC15CDA58328AF64171C6105288C988A10B22649E8161A98`.
- This correction has not yet run packaged Windows Runtime verification. Background `WM_HOTKEY` delivery while another application is foreground, native PrintScreen suppression, pre-drag Esc, repeated Start-menu activation, MainWindow reactivation, process counts and current-package MainWindow X cleanup remain Repository owner manual re-acceptance items. No SnipPlus GUI, Paint, Notepad, Snipping Tool or other external GUI was launched during automated verification; no desktop screenshot、frozen frame or Clipboard payload was saved.
- Stage 5 remains `Implementation and automated verification passed; Repository owner re-acceptance pending`; Selection adjustment、Function Bar、Annotation、Clipboard、PNG and Stage 6 remain unstarted.

### Accepted — Stage 5 Repository Owner Manual Acceptance and Compatibility Guidance (2026-07-29)

- Repository owner manual acceptance passed for the Owner Reference environment: background-focus PrintScreen, single-system Crosshair, single／cross-display initial Selection, pre-drag／drag／SelectionLocked Esc cancellation, resident-process retention after Esc, no automatic MainWindow reopening, repeated post-cancel PrintScreen sessions, single-instance activation, active-session activation isolation and MainWindow `X` full exit.
- The owner also confirmed that Clipboard remained unchanged and no PNG was created. Selection adjustment、Function Bar、Annotation、Complete、Clipboard output、PNG、Save As、gap rasterization、Maximum four-4K runtime and performance acceptance remain outside the completed Stage 5 scope.
- Windows compatibility prerequisite is now documented: the Windows 11 setting `使用 Print Screen 鍵開啟螢幕擷取` must be disabled for SnipPlus to receive background PrintScreen while another application is foreground. With that Windows setting disabled, Windows native capture did not appear and SnipPlus received the request; SnipPlus does not change this system setting automatically.
- MainWindow now shows the compatibility notice `若背景程式中按 PrintScreen 仍開啟 Windows 截圖工具，請關閉 Windows 的「使用 Print Screen 鍵開啟螢幕擷取」設定。` and provides `開啟 Windows 鍵盤設定`, using the platform `ISettingsLauncher` boundary and official `ms-settings:easeofaccess-keyboard` URI. Launch failure is returned as a typed result; no Registry、Group Policy or PrintScreen registration mutation is performed by this command.
- Locked restore succeeded; Release x64 solution build succeeded with `0 warnings、0 errors`; non-interactive tests `106/106` passed with `0` failures and `0` skips; limited changed-C# format verification and `git diff --check` passed. The added tests cover the compatibility notice／status, official URI boundary, success, typed launch failure and exception failure without opening the Settings GUI.
- Stage 5 is `Repository owner manual acceptance passed for the Owner Reference environment, with the documented Windows PrintScreen compatibility prerequisite`. Stage 6 has not started.

### Verified — Stage 6A SelectionLocked Adjustment Static Slice (2026-07-29)

- Added platform-neutral `SelectionInteractionMode`、`SelectionHitTestKind` and `SelectionHitTesting` contracts for moving、four-edge／four-corner resize and outside-drag reselection. The selection coordinator preserves `SelectionStatus.Locked` and the sole `COMP-001` `WorkflowStateAuthority` state while applying immutable physical geometry revisions.
- Locked Selection adjustments clamp moves to the Virtual Desktop bounds、normalize resize geometry、flip the effective handle when an edge or corner crosses its opposite side、reject zero／invalid geometry and restore the previous locked bounds on invalid release. Escape cancels the active session through the existing cleanup boundary.
- Windows overlay presentation now declares eight logical handles, maps handle placement to each display's physical bounds and rasterization scale, avoids placing handles in topology gaps and maps the system cursor to the current hit-test mode. No Function Bar、Editing、Annotation、Undo／Redo、Complete、Clipboard、PNG or Save behavior was added.
- Added deterministic contract、move／resize／reselection／rollback／pointer-ownership、COMP-001 preservation and Windows handle／cursor mapping tests. Synthetic mixed-DPI／negative-coordinate physical input remains platform-neutral; no real desktop pixels or Clipboard payload were saved.
- Locked restore succeeded; Release x64 solution build succeeded with `0 warnings、0 errors`; non-interactive tests `115/115` passed with `0` failures and `0` skips; limited Stage 6A C# format verification and `git diff --check` passed. No GUI、MSIX、Interactive or Manual verification was run.
- Stage 6A is `static implementation and deterministic verification passed; packaged Windows runtime verification pending`. Stage 6B and later slices have not started.

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

### Added — Stage 6B Editing and Function Bar Foundation (2026-07-29)

- 新增 `WorkflowState.Editing` 與唯一 `WorkflowStateAuthority` 的 `SelectionLocked → Editing → Cancelled` 合法狀態邊界；Selection visual status 仍保持 `Locked`。
- 新增 platform-neutral Function Bar command、placement、presentation、stale session／revision 與 cleanup contracts。Stage 6B 只啟用 Cancel；Complete、Save、Undo、Redo 保持 disabled，未接上 Clipboard、PNG 或其他 output。
- 新增 Core physical placement service：依 Selection intersection、current pointer、穩定 DisplayId 選擇 anchor display；使用 physical Work Area、DPI、Below／Above／clamped placement 與邊界驗證。
- Windows overlay 內以 hosted Canvas／Border 建立單一 Function Bar，量測 DIP 尺寸後轉成 physical pixels；調整 Selection 時 hide，revision 改變後 reposition／show，session 結束時冪等清理。不新增 top-level window、taskbar 或 Alt-Tab entry。
- 新增 deterministic placement、state transition、Editing／Cancel、Function Bar availability、準備失敗清理與 Windows hosted-bar structure contract tests；所有測試使用 synthetic geometry，未啟動 GUI、WGC、Paint、Notepad、Settings、Clipboard 或 PNG。
- Locked restore 成功；Release x64 solution build 成功（`0 warnings、0 errors`）；非互動測試 `126/126` 通過（`0 failures、0 skips`）；限定 Stage 6B C# format verify 與 `git diff --check` 通過。Stage 6A／6B packaged Windows runtime 尚未驗證，Stage 7 及後續功能未開始。
- Development MSIX 生成成功：`src/SnipPlus.App/AppPackages/SnipPlus.App_1.0.0.0_x64_Test/SnipPlus.App_1.0.0.0_x64.msix`；package manifest 保持 `AppListEntry="default"`、`graphicsCaptureProgrammatic`、`runFullTrust`，package 內 `SnipPlus.App.dll` 與 Release DLL SHA-256 均為 `ab60b0d7b22ecf8c1aa9d794298933a8a25f44efb83846396c5038c601c5e742`。Packaging 僅因缺少 `mspdbcmf.exe` 顯示 symbols warning，無 package error；本次未安裝、未啟動，不能視為 Windows Runtime evidence。

### Corrected — Stage 6B Initial Selection Release and Function Bar Preparation (2026-07-29)

- Repository owner Stage 6 manual acceptance failed at the initial Selection release: the three Frozen Overlays appeared and the initial Selection could be dragged, but all Overlays closed immediately after mouse release and the Function Bar was not visible. Stage 7 was not started.
- The source-level failure path is confirmed as `FunctionBarSurface` creating its root with `Visibility.Collapsed` before `TryMeasurePhysicalSize`; a zero `DesiredSize` returns typed `FailureCode.BarMeasurementFailed`, `PrepareEditing` calls `FailCurrentAsync`, and the required session cleanup closes all Overlays. No new interactive runtime trace was collected during this correction; this is the deterministic path matching the owner observation.
- Function Bar preparation now keeps the hosted root layout-participating with `Visibility.Visible`, `Opacity=0` and `IsHitTestVisible=false`; the measured DIP size is deterministically converted using the anchor display rasterization scale. Show／Hide switches only opacity and hit testing, and actual close／dispose remains the only path that collapses and removes the root.
- Added deterministic visibility-policy and DIP-to-physical measurement tests, strengthened the valid Selection release assertion to require no Overlay cleanup, and added typed `BarMeasurementFailed` cleanup coverage. Complete、Save、Undo、Redo、Clipboard、PNG and Annotation remain outside this correction.
- Locked restore succeeded; Release x64 solution build succeeded with `0 warnings、0 errors`; non-interactive tests `128/128` passed with `0` failures and `0` skips; the three modified C# files passed limited `dotnet format --verify-no-changes`; `git diff --check` passed.
- Stage 6 status is `Implementation and automated verification passed; Repository owner re-acceptance pending`. Stage 7 has not started. No SnipPlus GUI、Paint、Notepad、Snipping Tool、WGC、Clipboard or PNG runtime was started or saved during this correction.

### Corrected — Stage 6 Cross-display Move Release, Cancel and Function Bar Contrast (2026-07-29)

- Repository owner Stage 6 acceptance remains failed only for cross-display Selection move release、Function Bar Cancel、Function Bar text contrast and the required distinction between legal Physical Gap coverage and Virtual Desktop outer-boundary clamping. All other reported Stage 6 checks remain recorded as passed; Stage 7 has not started.
- The cross-display release path now has one session-owned input boundary shared by every Overlay. It normalizes PointerId across top-level Overlay windows, keeps one active Selection interaction, treats `PointerCaptureLost`／`WM_CAPTURECHANGED` as capture notifications rather than releases, and uses session-scoped `WM_LBUTTONUP` plus XAML `PointerReleased` as one deduplicated commit boundary. A duplicate or late release is ignored without a second Core commit.
- Function Bar Cancel now snapshots the session／coordinate／revision request, disables the Cancel button through the pending dispatch gate and queues the command through the Overlay UI `DispatcherQueue` after the Button routed event returns. This prevents synchronous Function Bar／Overlay cleanup reentrancy; the existing Core `CancelCurrentAsync` remains the sole cleanup path.
- Function Bar buttons now use a shared high-contrast visual policy: white foreground, dark background and visible border. `IsEnabled=false` remains the command availability authority for Complete／Save／Undo／Redo; no output or Annotation command was added.
- Owner Reference boundary evidence now includes a 150% lower-display physical `1920 × 1080` scenario that can retain a Selection across the display-to-gap boundary while deterministic move clamping keeps the rectangle within `(-2560,0)–(2560,2520)`. No clamp change was needed because the existing Core Virtual Desktop clamp already preserves Selection size and constrains all four outer edges.
- Added deterministic tests for cross-window PointerId normalization、native/XAML release deduplication、Cancel single-dispatch gating、high-contrast visual policy and Owner Reference Physical Gap／Virtual Bounds behavior. The non-interactive suite is `132/132` passed with `0` failures and `0` skips.
- Locked restore succeeded; Release x64 build succeeded with `0` warnings and `0` errors; the three modified C# files passed limited `dotnet format --verify-no-changes`; `git diff --check` passed. A clean-commit Development MSIX was generated, signed and installed with matching Release／package／installed DLL hashes. A post-fix packaged manual re-acceptance attempt was blocked before MainWindow appeared: Windows Error Reporting recorded `Microsoft.UI.Xaml.dll` with exception `0xc000027b`; no Selection、Function Bar or output scenario was executed after this launch failure. No source change was made for this separate startup/runtime blocker, and Stage 7 was not started.

### Added — Stage 6C Complete to Clipboard Vertical Slice (2026-07-30)

- 新增 platform-neutral `IFrozenDisplayFrameSetRenderer` 與 typed render outcomes；Complete 只接受 current Session／coordinate version／Selection revision 的有效 `SelectionInteractionMode.Locked`，並以 `Editing → ResultReady → Delivering → Completed → ResidentReady` 由唯一 `WorkflowStateAuthority` 管理。
- `WindowsFrozenDisplayFrameSetRenderer` 從同一個 Session-owned `FrozenDisplayFrameSet` 合成 canonical BGRA8、Premultiplied、sRGB SDR `SoftwareBitmap`；只配置 Selection 尺寸、逐 display copy physical intersection、不 stretch、不重新擷取，非顯示 Gap 保留透明 alpha `0`。
- Function Bar 現在只啟用 Complete／Cancel；Complete 使用 deferred DispatcherQueue command gate，重複命令回傳 `Busy`。Save、Undo、Redo、Annotation、PNG、Save As 均未加入。
- Complete 成功後只透過既有 `WinRtClipboardDeliveryAdapter` 發布 canonical result，History／Roaming 維持 disabled，成功後關閉 Function Bar／Overlays、dispose Session frame set 並回到 resident；render 或 Clipboard failure 保留 Editing、Selection 與 FrozenDisplayFrameSet，只 dispose temporary result 並回報 typed feedback。
- 新增 deterministic Core／Windows tests：Complete success、same frozen frame set／single acquisition、duplicate gate、render failure retention、Clipboard failure retention、negative-coordinate physical composition、transparent topology gap、canonical BGRA8 metadata 與 zero-size rejection。
- Locked restore 成功；Release x64 solution build 成功，`0 warnings、0 errors`；非互動測試 `138/138` 通過，`0 failures、0 skips`；本 Slice 11 個 C# 檔案限定範圍 `dotnet format --verify-no-changes --no-restore --include ...` 通過；`git diff --check` 通過。
- 由 clean source commit `b0ccd21df83e4bf2f7462842b9aa2a26f9c1d961` 建立 Stage 6C x64 MSIX，並保存為不可覆寫 Artifact：`D:\MEGA\SnipPlusArtifacts\Stage6C\b0ccd21df83e4bf2f7462842b9aa2a26f9c1d961\SnipPlus.Stage6C.CompleteClipboard.x64.msix`。MSIX SHA-256 為 `3B30AF0CCBB3C4A1E6E0D5B083BE5A9E815E840A3B600192DB84711923F32482`；package 與 Release `SnipPlus.App.dll` SHA-256 均為 `1EE6C16FD8444CE84721A2409BF7022FBE40C16B909214C50FEB72C27F20E0E4`。此 Artifact 尚未安裝，未宣稱 packaged runtime 通過；封裝只產生既有 `mspdbcmf.exe` symbols warning 1 項，0 errors。
- 本次未啟動 SnipPlus GUI、Paint、Notepad、Snipping Tool 或其他外部 GUI，未保存真實桌面／Frozen Frame／Clipboard payload；Stage 6C packaged Windows runtime、四螢幕／Maximum four-4K、performance、Save、Annotation 與 Stage 7 仍未開始。

### Not Released

- No release publication、Store deployment or release artifact submission has occurred。
- Existing packages were used only for development／runtime verification。
