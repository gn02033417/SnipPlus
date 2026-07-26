# Capture Backend 可行性研究

## 1. Document Control

| Field | Value |
|---|---|
| Document ID | `RESEARCH-TECH-CAPTURE-001` |
| Title | Capture Backend Feasibility |
| Status | Draft |
| Research Type | Technology Feasibility / Official Evidence Baseline |
| Research Date | 2026-07-26 |
| Technology Decision | `TD-003` — Capture Backend；依 `Architecture/TECHNOLOGY-DECISION-ROADMAP.md`，目前為 `Candidate` |
| UI Framework Decision | Unresolved — `ADR-0002` remains Draft |
| Rendering Decision | Not made |
| Runtime Verification | Not performed |
| Capture Decision | Not made |
| Capture Execution Authorized | No |
| Owner | TBD |
| Last reviewed | Not reviewed |

本文件是 Capture Backend 的官方文件可行性基線，不是技術選型決定、實作規格、Runtime Spike 報告或 Capture ADR。

## 2. Purpose

本研究回答 SnipPlus 的 Capture Backend 需要具備哪些能力，以及 Windows 平台候選策略在官方文件層級已確認、部分確認或仍未知的風險。研究結果只用來界定後續證據工作與 Runtime Spike，不授權建立 Capture API 呼叫、專案、原型或截圖功能。

研究特別關注：

- Windows 11 桌面、視窗與顯示器影像來源的取得方式。
- 單次擷取與必要的短期 frame acquisition 是否適合作為產品來源。
- Virtual desktop、單一顯示器、多顯示器與負座標。
- Per-monitor DPI、logical DIP、physical pixel 與 region crop 的座標一致性。
- Cursor inclusion/exclusion、透明或 layered window，以及 overlay self-capture。
- HDR、wide color、SDR conversion、pixel format 與色彩忠實度。
- Protected content、secure desktop/UAC 與使用者隱私界線。
- First-frame latency、CPU/GPU/memory、device loss、session 變更與 recovery。
- WinUI 3 / WPF interop，以及 packaged / unpackaged host 的影響。

## 3. Decision Boundary

### 3.1 In scope

| Area | Research question |
|---|---|
| Desktop/display image acquisition | 候選策略是否能提供可用的桌面或顯示器影像來源？ |
| One-shot capture | 是否可取得一次可供後續流程使用的 source frame？ |
| Short-term frame acquisition | 若需短暫 frame sequence 才能得到穩定來源，官方文件有哪些限制？ |
| Virtual desktop | 是否能覆蓋虛擬桌面範圍，而不是只假設單一螢幕？ |
| Single monitor | 單一顯示器與顯示器 rotation 的基本行為為何？ |
| Multi-monitor | 多顯示器來源、排列、拓撲變更與獨立 rotation 的風險為何？ |
| Negative coordinates | 左側或上方顯示器形成的負 X/Y 是否能正確映射？ |
| Per-monitor DPI | logical DIP 與 physical pixel 是否能被明確分開並驗證？ |
| Region crop source | Capture Backend 是否能回傳 crop 所需的 frame bounds 與座標證據？ |
| Cursor | Cursor 是否由來源提供、可排除，或必須由後續流程處理？ |
| Overlay exclusion | 產品自己的 Selection Overlay 是否可能出現在來源中？排除能力有哪些限制？ |
| HDR / wide color | HDR、wide color、scRGB、SDR conversion 與 metadata 是否有明確證據？ |
| Protected / secure content | 受保護內容及 secure desktop 是否可取得、不可取得或只可部分取得？ |
| Failure / recovery | device loss、desktop switch、mode change、session change 後如何分類風險？ |
| Host interoperability | WinUI 3、WPF、native interop 與 packaged/unpackaged 對候選策略的影響為何？ |

### 3.2 Must not decide

本文件不得決定以下事項：

- UI Framework；`ADR-0002` 的狀態維持 Draft。
- Rendering Technology；Rendering Research 不因本文件改變。
- Global hotkey 或 Print Screen interception。
- Selection Overlay 的實作方式、視窗生命週期或 UI Workflow。
- Clipboard API。
- PNG encoder、儲存格式、Annotation model 或產品資料結構。
- Formal SDK/runtime version、class/interface/service/source code。
- Capture candidate 的最終選擇或 Capture ADR。

### 3.3 Non-goals

- 不呼叫任何 Capture API。
- 不讀取真實 desktop pixels。
- 不建立 Screenshot、Recording、Image artifact 或測試圖片。
- 不建立 project、solution、prototype、source code 或 runtime result。
- 不執行 restore、build、run、publish、test 或 runtime spike。
- 不下載、安裝或修改 SDK、runtime、driver 或套件。
- 不建立 Capture ADR。
- 不修改 `ADR-0002`、Rendering Research 或既有 UI/Rendering 文件。
- 不實作 Print Screen、Selection Overlay、Clipboard 或 screenshot pipeline。

## 4. Evidence Vocabulary

### 4.1 Evidence status

本文件所有候選判斷只使用下列狀態：

| Status | Meaning |
|---|---|
| `Confirmed by official documentation` | 官方文件直接描述該能力或介面行為；不表示產品整合已驗證。 |
| `Partially supported` | 官方文件只支持能力的一部分，或仍有產品邊界未定義。 |
| `Requires runtime prototype` | 官方文件不足以確認產品行為，必須在未來受控 Runtime Spike 中取得證據。 |
| `Unknown` | 本研究尚未有足夠官方證據，不可推導為不支援。 |
| `Not aligned` | 候選本身的模型與 SnipPlus 需求方向不一致；仍不可視為正式淘汰決定。 |

### 4.2 Evidence layers

| Layer | Meaning | This document may claim |
|---|---|---|
| Documented capability | 官方 API 或平台文件明確記載的能力。 | `Confirmed by official documentation` 或 `Partially supported` |
| Product inference | 將官方能力對照 SnipPlus 需求後的合理推論。 | 必須標示為推論，不能寫成官方承諾。 |
| Unknown | 官方文件沒有回答或產品邊界尚未測量的問題。 | `Unknown` 或 `Requires runtime prototype` |
| Official evidence | Microsoft 官方文件的文字、API identity、限制與適用範圍。 | 可作為候選 baseline。 |
| Local availability | 本機是否安裝 SDK、runtime、driver 或可建置。 | 本文件不檢查，固定為 `Unknown`。 |
| Runtime evidence | 真實 host、display topology、DPI、HDR、overlay 與 failure 行為。 | 本文件不產生，固定為未驗證。 |

### 4.3 Interpretation boundary

API 存在不等於產品適用；managed wrapper 存在不等於 native 限制已消失；官方版本文件不等於本機 availability；文件宣稱支援不等於 SnipPlus 的 coordinate、overlay、color 或 recovery contract 已通過 Runtime Verification。

## 5. Candidate Strategies

候選策略固定為以下五項，名稱與 ID 在本研究範圍內保持穩定：

| Candidate ID | Candidate strategy | Position in this research |
|---|---|---|
| `CAP-OPT-001` | Windows Graphics Capture | Windows modern capture model；以 display/window capture item 與 frame pool 為研究對象。 |
| `CAP-OPT-002` | DXGI Desktop Duplication API | 以 output duplication、desktop image、dirty/move metadata 與 cursor metadata 為研究對象。 |
| `CAP-OPT-003` | GDI-based desktop capture | 以 desktop DC、compatible bitmap 與 `BitBlt`/`CAPTUREBLT` 行為為研究對象。 |
| `CAP-OPT-004` | Window-oriented capture mechanisms | 以特定視窗或視窗內容為來源的機制；本研究不把任何單一未決 API 當成選型。 |
| `CAP-OPT-005` | Hybrid primary/fallback capture strategy | 將兩種或以上候選組成主路徑與 fallback；依賴各候選的獨立證據，不是已選架構。 |

## 6. Official Evidence Baseline

| Evidence ID | Official source and claim boundary | Related candidates | Related criteria |
|---|---|---|---|
| `CAP-OFF-EVID-001` | [Screen capture - Windows apps](https://learn.microsoft.com/en-us/windows/apps/develop/media-authoring-processing/screen-capture) 說明 `Windows.Graphics.Capture` 可取得 display/window frames，用於 video streams 或 snapshots；也說明 frame pool 重建與 capture UI 邊界。 | `CAP-OPT-001` | `CAP-002`, `CAP-003`, `CAP-004`, `CAP-019`, `CAP-020`, `CAP-021` |
| `CAP-OFF-EVID-002` | [GraphicsCaptureSession.IsBorderRequired](https://learn.microsoft.com/en-us/uwp/api/windows.graphics.capture.graphicscapturesession.isborderrequired?view=winrt-28000) 說明 capture border 預設與 borderless access consent；不能推導成任意 overlay self-capture exclusion。 | `CAP-OPT-001` | `CAP-010`, `CAP-015`, `CAP-016` |
| `CAP-OFF-EVID-003` | [Desktop Duplication API](https://learn.microsoft.com/en-us/windows/win32/direct3ddxgi/desktop-dup-api) 說明 `AcquireNextFrame`、dirty/move regions、cursor metadata、rotation 與桌面影像處理邊界。 | `CAP-OPT-002` | `CAP-002`, `CAP-003`, `CAP-004`, `CAP-005`, `CAP-011`, `CAP-012`, `CAP-013` |
| `CAP-OFF-EVID-004` | [Desktop duplication](https://learn.microsoft.com/en-us/windows-hardware/drivers/display/desktop-duplication-api) 說明 DXGI desktop duplication 可取得一個或多個 active displays 的桌面內容與 metadata，並涉及 GPU memory 與 protected content。 | `CAP-OPT-002` | `CAP-005`, `CAP-013`, `CAP-015`, `CAP-018` |
| `CAP-OFF-EVID-005` | [IDXGIOutputDuplication](https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_2/nn-dxgi1_2-idxgioutputduplication) 說明 desktop switch、mode change 或 fullscreen app 可能使 duplication invalid，需 release/recreate；不能推導 recovery 已完成。 | `CAP-OPT-002` | `CAP-019`, `CAP-016`, `CAP-022` |
| `CAP-OFF-EVID-006` | [DXGI_OUTDUPL_DESC](https://learn.microsoft.com/en-us/windows/win32/api/dxgi1_2/ns-dxgi1_2-dxgi_outdupl_desc) 說明 output description、rotation、desktop image format 與 `DXGI_FORMAT_B8G8R8A8_UNORM`。 | `CAP-OPT-002` | `CAP-003`, `CAP-005`, `CAP-013`, `CAP-014` |
| `CAP-OFF-EVID-007` | [Capturing an Image](https://learn.microsoft.com/en-us/windows/win32/gdi/capturing-an-image?redirectedfrom=MSDN) 以 bitmap、compatible DC 與 `BitBlt` 示範桌面影像擷取概念；sample 不代表 SnipPlus 的 DPI、HDR 或 recovery contract。 | `CAP-OPT-003` | `CAP-002`, `CAP-003`, `CAP-004`, `CAP-017`, `CAP-018` |
| `CAP-OFF-EVID-008` | [BitBlt function](https://learn.microsoft.com/en-us/windows/win32/api/wingdi/nf-wingdi-bitblt) 說明 rectangle pixel copy、logical coordinates、`CAPTUREBLT` 可包含 layered windows，以及 BitBlt 不執行 color management。 | `CAP-OPT-003` | `CAP-008`, `CAP-009`, `CAP-010`, `CAP-014` |
| `CAP-OFF-EVID-009` | [LogicalToPhysicalPointForPerMonitorDPI](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-logicaltophysicalpointforpermonitordpi) 說明 logical/physical point conversion；不能替代產品整體 coordinate contract。 | `CAP-OPT-001`, `CAP-OPT-002`, `CAP-OPT-003`, `CAP-OPT-004` | `CAP-007`, `CAP-008` |
| `CAP-OFF-EVID-010` | [DPI awareness context](https://learn.microsoft.com/en-us/windows/win32/hidpi/dpi-awareness-context) 說明 per-monitor aware 與 per-monitor v2 的平台概念；host 的實際 awareness 尚未驗證。 | All | `CAP-007`, `CAP-020`, `CAP-021` |
| `CAP-OFF-EVID-011` | [Declaring managed apps DPI-aware](https://learn.microsoft.com/en-us/windows/win32/hidpi/declaring-managed-apps-dpi-aware) 說明 WPF device-independent units、OS scaling 與跨 DPI monitor 的 redraw/re-layout 邊界。 | `CAP-OPT-004`, `CAP-OPT-005` | `CAP-007`, `CAP-021` |
| `CAP-OFF-EVID-012` | [SetWindowDisplayAffinity](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowdisplayaffinity) 說明 `WDA_EXCLUDEFROMCAPTURE` 的 top-level window exclusion、DWM composing 與非 DRM/security guarantee 限制。 | `CAP-OPT-001`, `CAP-OPT-002`, `CAP-OPT-003`, `CAP-OPT-004` | `CAP-010`, `CAP-015`, `CAP-016` |

### 6.1 Baseline limitations

- 上述來源是官方 capability evidence，不是 SnipPlus 的 runtime evidence。
- 沒有官方文件回答的問題，保留為 `Unknown` 或 `Requires runtime prototype`。
- 本研究不把 Microsoft sample 的可執行性、範例輸出或 API signature 當成產品通過條件。
- 本機 SDK、runtime、GPU driver、Windows build 與 display hardware availability 均為 `Unknown`。

## 7. Criteria Catalog

| Criterion ID | Criterion | Required interpretation |
|---|---|---|
| `CAP-001` | Windows 11 platform availability | 候選在 SnipPlus 目標 Windows 11 桌面環境的官方存在性與適用範圍。 |
| `CAP-002` | One-shot capture suitability | 能否取得一個可供後續 pipeline 使用的 source frame，而不必把產品設計成長時間錄影。 |
| `CAP-003` | Virtual desktop coverage | 能否描述虛擬桌面或多 output 的 coverage 與來源邊界。 |
| `CAP-004` | Single-monitor capture | 單一顯示器影像取得是否有直接官方證據。 |
| `CAP-005` | Multi-monitor support | 多顯示器、排列、rotation、output 選擇與拓撲變更的支援程度。 |
| `CAP-006` | Negative virtual-screen coordinates | 左/上方顯示器的負 X/Y 是否可被完整保留與映射。 |
| `CAP-007` | Per-monitor DPI coordinate fidelity | logical DIP、physical pixel、monitor DPI 與 selection mapping 是否可穩定對齊。 |
| `CAP-008` | Region crop precision | source bounds、selection bounds、rounding 與邊界像素是否可精準裁切。 |
| `CAP-009` | Transparent/layered window behavior | 透明、layered、DWM-composed window 的來源行為是否有官方證據。 |
| `CAP-010` | Overlay self-capture avoidance | Selection Overlay 是否會被捕捉，以及排除能力是否可靠、可驗證。 |
| `CAP-011` | Cursor inclusion control | cursor 是否包含於 source、可明確排除，或需額外合成/處理。 |
| `CAP-012` | Occluded/minimized window implications | 被遮蔽、最小化或不可見視窗的 capture semantics。 |
| `CAP-013` | HDR and wide-color behavior | HDR、wide color、scRGB、mixed HDR/SDR 與來源 pixel format 的行為。 |
| `CAP-014` | SDR conversion and color fidelity | SDR conversion、色彩管理、alpha 與 source fidelity 的責任界線。 |
| `CAP-015` | Protected-content behavior | DRM、protected surface 或 protected content 的不可取得/黑畫面等行為。 |
| `CAP-016` | Secure desktop/UAC boundary | secure desktop、UAC、desktop switch 與 session boundary 的限制。 |
| `CAP-017` | Startup and first-frame latency | first-frame timing 是否可量測；產品閾值暫為 `TBD`。 |
| `CAP-018` | CPU/GPU/memory implications | frame pool、GPU surface、CPU copy、memory pressure 與 resource lifecycle。 |
| `CAP-019` | Device-loss and recovery behavior | device loss、mode change、desktop switch、session change 後的 recovery。 |
| `CAP-020` | Packaged/unpackaged compatibility | packaged 與 unpackaged host 的 API、consent、manifest、lifecycle 差異。 |
| `CAP-021` | WinUI 3/WPF host interoperability | WinUI 3、WPF 與 native boundary 的可調用性、frame ownership 與 coordinate interop。 |
| `CAP-022` | Testability and deterministic evidence | 是否能在不保存私人桌面內容的前提下建立可重複、可審查的證據。 |

產品 latency、FPS、memory、pixel tolerance、HDR quality、crop rounding 等 thresholds 均維持 `TBD`；本文件不臆造 KPI。

## 8. Candidate Identity Matrix

| Candidate | Exact API identity | Technology owner | SDK/header/namespace | Managed/native boundary | Minimum platform | Architecture | Evidence |
|---|---|---|---|---|---|---|---|
| `CAP-OPT-001` Windows Graphics Capture | `Windows.Graphics.Capture`; `GraphicsCaptureItem`; `GraphicsCaptureSession`; frame pool | Windows graphics / Windows App SDK and WinRT surface | WinRT namespace；具體 SDK/runtime version `TBD` | WinRT/managed projection 與 graphics frame/native resource interop；wrapper 不消除 platform limits | Windows 10+ capability line is documented；SnipPlus Windows 11 target applicability still needs host evidence | Display/window capture item plus frame acquisition; no product API selected | `CAP-OFF-EVID-001`, `CAP-OFF-EVID-002` |
| `CAP-OPT-002` DXGI Desktop Duplication API | `IDXGIOutputDuplication`; `AcquireNextFrame`; `DXGI_OUTDUPL_DESC` | DirectX / DXGI | `dxgi1_2.h`; Direct3D/DXGI native boundary | Native COM/GPU resource boundary；managed interop and device ownership are open | Windows 8+ API family documented；Windows 11 host/device availability unverified | Per-output duplication with desktop image and metadata | `CAP-OFF-EVID-003` to `CAP-OFF-EVID-006` |
| `CAP-OPT-003` GDI-based desktop capture | `GetDC`/desktop DC concept; `CreateCompatibleDC`; `CreateCompatibleBitmap`; `BitBlt`; `CAPTUREBLT` | Win32 GDI / User32 | `wingdi.h`, `winuser.h`; GDI32/User32 | Native HDC/HBITMAP and managed bitmap conversion boundary | Long-standing Windows desktop API family; target behavior still needs controlled evidence | Synchronous DC-to-bitmap copy | `CAP-OFF-EVID-007`, `CAP-OFF-EVID-008` |
| `CAP-OPT-004` Window-oriented capture mechanisms | Candidate family only；exact API intentionally unresolved | Windows windowing/capture surface | Exact header/namespace `TBD` | Likely window handle, WinRT item or native interop boundary；cannot assume one model | `TBD` | Captures a window-oriented source rather than an arbitrary virtual desktop | `Unknown`; future source selection required |
| `CAP-OPT-005` Hybrid primary/fallback capture strategy | Strategy composition；exact primary/fallback APIs intentionally unresolved | Depends on selected candidates | Depends on selected candidates | Multiple native/managed boundaries and normalized backend contract | Depends on selected candidates | Common contract with primary, fallback and failure classification | Derived from candidate evidence; not independently proven |

### 8.1 Identity constraints

- GDI、DXGI 與 WinRT identity 分開追蹤；不能把它們視為同一種 capture API。
- API existence 不等於 one-shot、multi-monitor、DPI、HDR 或 overlay suitability。
- Managed wrapper 不代表 native device loss、pixel format、security 或 coordinate 限制已消失。
- Official version line 不等於本機 SDK、runtime、driver 或 hardware availability。
- Local availability 固定 `Unknown`；build/runtime verification 固定 `No`。

## 9. Host Compatibility Matrix

| Candidate | WinUI 3 | WPF | Host-independent core possible | Native interop required | Packaging dependency | Open risk |
|---|---|---|---|---|---|---|
| `CAP-OPT-001` Windows Graphics Capture | Official docs describe WinUI 3 frame-pool considerations；host integration still needs runtime evidence | API identity may be callable through projection/interop；WPF frame ownership and UI-thread boundary `Requires runtime prototype` | Candidate-specific acquisition core may be separated from UI workflow；not proven | WinRT/graphics resource conversion likely required | Consent, manifest and packaged/unpackaged behavior `Requires runtime prototype` | `CAP-007`, `CAP-010`, `CAP-013`, `CAP-020`, `CAP-021` |
| `CAP-OPT-002` DXGI Desktop Duplication | Can be isolated behind native/interop core；WinUI host integration not verified | Can be isolated behind native/interop core；WPF bitmap/resource ownership not verified | Yes as a product boundary hypothesis；not an implementation decision | Yes: COM, Direct3D/DXGI, GPU resource and device lifecycle | Packaged/unpackaged behavior and Store restriction need verification | `CAP-005`, `CAP-019`, `CAP-020`, `CAP-021` |
| `CAP-OPT-003` GDI-based desktop capture | Win32 interop boundary required；no WinUI suitability conclusion | Win32 interop and bitmap conversion boundary required；DPI/color behavior open | Yes as a synchronous native source boundary hypothesis | Yes: HDC/HBITMAP and resource cleanup | Lower API packaging dependency is an inference, not a compatibility proof | `CAP-007`, `CAP-013`, `CAP-014`, `CAP-021` |
| `CAP-OPT-004` Window-oriented capture mechanisms | Exact API and source ownership unresolved | Exact API and source ownership unresolved | Possible only after exact candidate identity | `TBD` | `TBD` | Window semantics, occlusion, minimized state, consent and source bounds |
| `CAP-OPT-005` Hybrid | Host must normalize multiple acquisition and failure models | Host must normalize multiple acquisition and failure models | Common core contract is a design hypothesis | At least one native/WinRT interop boundary likely | Union of selected candidates' constraints | Fallback correctness, duplicate behavior, inconsistent pixels and recovery |

### 9.1 Compatibility questions that remain separate

對每個候選，以下問題必須獨立回答，不能由「API callable」代替：

1. Host 能否建立 capture item 或 device？
2. Host 能否取得一個 frame？
3. Host 能否把 frame 映射到正確的 virtual desktop / monitor coordinates？
4. Host 能否在 per-monitor DPI 下維持 physical pixel fidelity？
5. Host 能否排除自己的 overlay？
6. Host 能否在 device/session 變更後 recovery？
7. Runtime prototype 是否能產生不含私人桌面資料的證據？

Capture Backend 不得擁有 UI Workflow State，也不得把 UI framework 的生命週期假設偷偷寫入候選比較。

## 10. Criteria Comparison Matrix

| Criterion | Windows Graphics Capture | DXGI Desktop Duplication | GDI | Window-oriented | Hybrid | Evidence |
|---|---|---|---|---|---|---|
| `CAP-001` Windows 11 platform availability | Confirmed by official documentation | Confirmed by official documentation | Confirmed by official documentation | Unknown | Partially supported | `CAP-OFF-EVID-001`, `003`, `007` |
| `CAP-002` One-shot capture suitability | Confirmed by official documentation | Partially supported | Confirmed by official documentation | Unknown | Partially supported | `CAP-OFF-EVID-001`, `003`, `007` |
| `CAP-003` Virtual desktop coverage | Partially supported | Partially supported | Partially supported | Unknown | Requires runtime prototype | `CAP-OFF-EVID-001`, `004`, `007` |
| `CAP-004` Single-monitor capture | Confirmed by official documentation | Confirmed by official documentation | Confirmed by official documentation | Unknown | Partially supported | `CAP-OFF-EVID-001`, `003`, `007` |
| `CAP-005` Multi-monitor support | Requires runtime prototype | Partially supported | Unknown | Unknown | Requires runtime prototype | `CAP-OFF-EVID-003`, `004`, `006` |
| `CAP-006` Negative virtual-screen coordinates | Requires runtime prototype | Requires runtime prototype | Requires runtime prototype | Unknown | Requires runtime prototype | `CAP-OFF-EVID-009`, `010` |
| `CAP-007` Per-monitor DPI coordinate fidelity | Requires runtime prototype | Requires runtime prototype | Requires runtime prototype | Requires runtime prototype | Requires runtime prototype | `CAP-OFF-EVID-009`, `010`, `011` |
| `CAP-008` Region crop precision | Requires runtime prototype | Requires runtime prototype | Partially supported | Unknown | Requires runtime prototype | `CAP-OFF-EVID-007`, `008`, `009` |
| `CAP-009` Transparent/layered window behavior | Partially supported | Unknown | Partially supported | Unknown | Requires runtime prototype | `CAP-OFF-EVID-001`, `008` |
| `CAP-010` Overlay self-capture avoidance | Unknown | Unknown | Unknown | Unknown | Requires runtime prototype | `CAP-OFF-EVID-002`, `012` |
| `CAP-011` Cursor inclusion control | Unknown | Partially supported | Unknown | Unknown | Requires runtime prototype | `CAP-OFF-EVID-003` |
| `CAP-012` Occluded/minimized window implications | Unknown | Unknown | Unknown | Requires runtime prototype | Requires runtime prototype | Official source semantics incomplete |
| `CAP-013` HDR and wide-color behavior | Unknown | Unknown | Unknown | Unknown | Requires runtime prototype | `CAP-OFF-EVID-004`, `006`, `008` |
| `CAP-014` SDR conversion and color fidelity | Unknown | Partially supported | Partially supported | Unknown | Requires runtime prototype | `CAP-OFF-EVID-006`, `008` |
| `CAP-015` Protected-content behavior | Unknown | Partially supported | Unknown | Unknown | Requires runtime prototype | `CAP-OFF-EVID-002`, `004`, `012` |
| `CAP-016` Secure desktop/UAC boundary | Unknown | Partially supported | Unknown | Unknown | Requires runtime prototype | `CAP-OFF-EVID-005`, `012` |
| `CAP-017` Startup and first-frame latency | Requires runtime prototype | Requires runtime prototype | Requires runtime prototype | Requires runtime prototype | Requires runtime prototype | No product threshold; `TBD` |
| `CAP-018` CPU/GPU/memory implications | Requires runtime prototype | Requires runtime prototype | Requires runtime prototype | Requires runtime prototype | Requires runtime prototype | `CAP-OFF-EVID-004`, `005`, `007` |
| `CAP-019` Device-loss and recovery behavior | Partially supported | Partially supported | Unknown | Unknown | Requires runtime prototype | `CAP-OFF-EVID-001`, `005` |
| `CAP-020` Packaged/unpackaged compatibility | Partially supported | Unknown | Partially supported | Unknown | Requires runtime prototype | `CAP-OFF-EVID-001`, `005` |
| `CAP-021` WinUI 3/WPF host interoperability | Requires runtime prototype | Requires runtime prototype | Requires runtime prototype | Unknown | Requires runtime prototype | `CAP-OFF-EVID-001`, `010`, `011` |
| `CAP-022` Testability and deterministic evidence | Requires runtime prototype | Requires runtime prototype | Requires runtime prototype | Unknown | Requires runtime prototype | All candidate evidence remains incomplete |

### 10.1 Matrix interpretation

此矩陣沒有選出 primary candidate，也沒有排除任何候選。`Partially supported` 表示官方 evidence 只覆蓋部分需求；`Requires runtime prototype` 表示必須在受控環境取得產品邊界證據；`Unknown` 表示不能從目前官方資料作可靠結論。

## 11. Critical Capture Gates

| Gate ID | Gate | Status | Required evidence before Capture ADR |
|---|---|---|---|
| `CAP-GATE-001` | One-shot capture can produce usable source frame | Partially satisfied | 至少一個候選在 synthetic environment 取得可審查的 source frame metadata 與功能結果。 |
| `CAP-GATE-002` | Virtual desktop and monitor coordinate mapping | Requires runtime prototype | 多螢幕排列、monitor bounds、origin 與 source mapping 的 evidence。 |
| `CAP-GATE-003` | Per-monitor DPI and negative-coordinate correctness | Requires runtime prototype | PMv2、mixed DPI、負 X/Y 與 physical-pixel mapping evidence。 |
| `CAP-GATE-004` | Region crop pixel fidelity | Requires runtime prototype | Known synthetic pattern、source bounds、crop bounds、rounding 與誤差證據。 |
| `CAP-GATE-005` | Overlay self-capture risk controllable | Requires runtime prototype | Overlay 顯示/隱藏、排除、timing、flicker 與 failure evidence。 |
| `CAP-GATE-006` | Cursor behavior explicit | Requires runtime prototype | Cursor included/excluded 的明確 policy 與可重複結果。 |
| `CAP-GATE-007` | HDR/SDR color risk bounded | Requires runtime prototype | Mixed HDR/SDR、pixel format、conversion、metadata 與可接受風險。 |
| `CAP-GATE-008` | Protected and secure content behavior defined | Partially satisfied | 官方限制 + synthetic/public boundary evidence；不得宣稱 bypass。 |
| `CAP-GATE-009` | Failure and recovery path defined | Partially satisfied | device loss、mode/session change、retry/recreate 與分類結果。 |
| `CAP-GATE-010` | WinUI 3/WPF interoperability evaluable | Requires runtime prototype | 兩種 host 的 frame ownership、coordinate、lifecycle 與 cleanup evidence。 |
| `CAP-GATE-011` | Evidence reproducible without private desktop data | Requires runtime prototype | synthetic/public fixture、metadata-only log、retention 與 redaction evidence。 |

Gate status vocabulary：`Satisfied by documentation`、`Partially satisfied`、`Requires runtime prototype`、`Unsatisfied`、`Not evaluated`。目前沒有 Gate 可作為 Capture ADR 的充分授權。

## 12. Coordinate Contract

### 12.1 Coordinate terms

| Term | Contract requirement |
|---|---|
| Virtual-screen origin | 必須明確記錄 virtual desktop origin；不得假設 `(0,0)` 是左上角。 |
| Monitor physical pixel bounds | 每個 monitor 的 physical pixel bounds 必須可與 source frame bounds 對應。 |
| Logical DIP | UI selection 使用的 logical/DIP 座標必須與 capture physical pixel 座標分層記錄。 |
| Negative X/Y | 左側或上方 monitor 的負座標必須保留符號、範圍與轉換證據。 |
| Selection rectangle owner | Selection/Workflow 提供 capture intent；Capture Backend 不擁有 selection state。 |
| Capture source bounds | Backend 回傳 source bounds、origin、size、monitor identity 與 frame metadata。 |
| Crop conversion | 必須保存 selection-to-source 的轉換輸入、輸出與使用的 DPI context。 |
| Boundary rounding | 像素邊界 rounding、inclusive/exclusive、sub-pixel 行為維持 `TBD`，不得臆造。 |
| Display topology changes | topology、resolution、rotation、DPI 或 monitor removal 造成的 frame invalidation 必須分類。 |
| Frame/overlay timing | selection frame、capture frame、overlay visibility 與 timestamp 的關係必須可觀察。 |

### 12.2 Ownership and invariants

1. Workflow/Selection 提供 capture intent；Backend 不修改 Selection。
2. Backend 回傳 frame metadata、source bounds 與 coordinate mapping evidence。
3. Backend 不把 logical DIP 直接當成 physical pixel。
4. Backend 不假設所有 monitor 共享 DPI、rotation、origin 或 pixel format。
5. Crop conversion 失敗時必須回傳可分類的 failure，不得靜默裁切成錯誤範圍。
6. Display topology 變更後，舊 frame 與舊 mapping 不得被默認視為仍有效。

## 13. Overlay Self-capture Boundary

| Risk area | Question | Current status |
|---|---|---|
| Overlay enters source frame | Selection Overlay 顯示時是否被同一 capture source 捕捉？ | Unknown；需 spike |
| Capture/overlay timing | 擷取發生於 overlay 隱藏前、隱藏後或 composition 之間？ | Requires runtime prototype |
| Window exclusion | `WDA_EXCLUDEFROMCAPTURE` 或候選專屬 exclusion 是否適用於每個候選？ | Partially supported；不是 universal guarantee |
| Borderless consent | WGC capture border 的 consent 是否能滿足產品 overlay boundary？ | Partially supported；需區分 border 與 self-capture |
| Hide-overlay flicker | 暫時隱藏 overlay 是否造成 flicker、focus change 或使用者可見副作用？ | Requires runtime prototype |
| Focus changes | capture、overlay、selection focus 變更是否改變 source 或 keyboard workflow？ | Unknown |
| Multi-monitor consistency | overlay 在不同 monitor、DPI、rotation 下是否一致被排除？ | Requires runtime prototype |
| Candidate dependence | exclusion 是否需要 HWND、capture item、DWM composition 或 process ownership？ | Partially supported；需逐候選驗證 |

### 13.1 Boundary statement

本文件不決定 overlay 隱藏策略、window affinity 設定、capture border policy 或 UI timing。官方文件可確認 exclusion API 的存在與限制，但不能把它解讀為 DRM、protected-content 或所有 capture path 的安全保證。

## 14. Color, HDR and Pixel Format

### 14.1 Questions to preserve

| Topic | Required question | Current status |
|---|---|---|
| Pixel format | 候選回傳何種 format、channel order、bit depth 與 row layout？ | Partially known for DXGI；others `Unknown` |
| Alpha semantics | alpha 是否有效、預乘、透明或只是 opaque surface？ | Unknown |
| Mixed HDR/SDR | 不同 monitor、window 或 source 的 HDR/SDR 混合如何呈現？ | Unknown |
| Wide color/scRGB | wide color、scRGB 與普通 SDR 的 conversion 由哪一層負責？ | Unknown |
| Source vs PNG responsibility | source fidelity 與後續 PNG encoding 必須分開；本文件不選 PNG encoder。 | Boundary fixed |
| Hardware/software conversion | conversion 是 GPU、OS、API、host 或後續 pipeline 的責任？ | Unknown |
| Metadata preservation | color profile、HDR metadata、orientation、DPI metadata 是否保留？ | Unknown |
| Evidence gaps | 不能以 SDR sample 推導 HDR 支援。 | Gap registered |

### 14.2 Current conclusions

- DXGI 官方文件明確記載 desktop image format 為 `DXGI_FORMAT_B8G8R8A8_UNORM`；這不等於 HDR fidelity 已確認。
- GDI `BitBlt` 官方文件明確說明不執行 color management；這是風險 evidence，不是完整產品結論。
- WGC、window-oriented 與 hybrid 的 HDR、alpha、metadata 行為在本研究尚不足以結論。
- 不宣告任何候選「支援 HDR」；HDR/SDR 僅能在 future runtime spike 中取得可審查 evidence。

## 15. Security and Privacy Boundary

### 15.1 Security behavior

| Boundary | Required interpretation |
|---|---|
| Protected content | 受保護內容可能是黑畫面、不可取得或有候選特定行為；不得假設可讀取。 |
| Secure desktop/UAC | secure desktop、UAC prompt、desktop switch 不得假設可擷取。 |
| Consent/picker | capture picker、user consent、border permission、manifest 或 session restriction 必須逐候選確認。 |
| No bypass | 本研究不尋找、不設計、不宣稱繞過 OS、DRM、secure desktop 或使用者同意。 |
| Process ownership | window exclusion 可能有 current-process、top-level window、DWM composition 等限制。 |
| Failure semantics | denied、protected、unavailable、device-lost、session-changed 必須被分類，不得被當成空白成功 frame。 |

### 15.2 Research evidence privacy

未來 runtime evidence 必須符合：

- 優先使用 synthetic/public fixture、測試圖形與公開視窗。
- 不保存真實 desktop screenshot、recording、私人視窗影像或 clipboard data。
- 日誌以 metadata、dimensions、coordinates、status、timestamps 與分類結果為主。
- 若錯誤訊息含私人視窗 title、路徑或帳號，必須在 evidence 中移除或替換。
- 不將本文件視為存取使用者桌面內容的授權。

## 16. Ownership Boundary

### 16.1 Capture Backend owns

- Source acquisition。
- Frame metadata。
- Source bounds 與 coordinate mapping evidence。
- Candidate-specific failure classification。
- Device、session、frame pool 或 output lifecycle。
- Candidate-specific cleanup、recreate 與 recovery signals。
- 將來源限制傳給上層，而不是自行改寫產品 workflow。

### 16.2 Capture Backend does not own

- Global hotkey。
- Print Screen interception。
- Workflow State。
- Selection State。
- Overlay visibility policy 或 UI focus policy。
- Annotation model。
- Rendering Technology。
- Clipboard。
- PNG encoding/storage。
- Product-wide focus policy。

### 16.3 Boundary rule

Capture Backend 的最小責任是「依 capture intent 取得來源並回報其可觀察限制」。它不得將 UI framework、selection workflow、rendering implementation 或 clipboard/storage 決策藏在候選 API 內。

## 17. Future Runtime Spikes

以下 12 個 Spike 只定義 future evidence，不建立 project、prototype 或執行結果。每一項都必須在另行授權後才可開始。

| Spike ID | Purpose | Candidates | Host frameworks | Synthetic environment | Preconditions | Required evidence | Functional pass condition | Measurement fields | Failure implication | Safety/privacy boundary | Dependency | Prohibited scope |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| `CAP-SPIKE-001` | 驗證單一 monitor one-shot source frame | 001, 002, 003 | WinUI 3, WPF | 公開測試圖形；單一 monitor | Human authorization；合法 API availability | source frame metadata、dimensions、status | 可取得一次可辨識來源且 metadata 完整 | first-frame、size、format、CPU/GPU/memory `TBD` | 候選不可作為 one-shot baseline 或需降級 | 不保存 desktop image；只保留 metadata | CAP-GATE-001 | 不做產品 pipeline、PNG、UI、ADR |
| `CAP-SPIKE-002` | 驗證 virtual desktop coverage | 001, 002, 003, 005 | WinUI 3, WPF | 兩個 synthetic monitors | topology 可控；人類授權 | monitor bounds、virtual origin、source mapping | 所有預期 bounds 可對應且無 silent crop | origin、monitor rectangles、frame count | 只能形成 per-monitor source 或需 hybrid | 不使用私人內容 | CAP-GATE-002 | 不決定主候選 |
| `CAP-SPIKE-003` | 驗證負座標 monitor | 001, 002, 003, 005 | WinUI 3, WPF | 左側或上方 synthetic monitor | `CAP-SPIKE-002` baseline | signed coordinates、mapping、crop result metadata | 負 X/Y 保留且 mapping 可重現 | min/max X/Y、rounding、DPI | coordinate contract 未通過 | synthetic pattern only | CAP-SPIKE-002, CAP-GATE-003 | 不修改 Selection implementation |
| `CAP-SPIKE-004` | 驗證 mixed-DPI mapping | 001, 002, 003, 004, 005 | WinUI 3, WPF | 不同 DPI 的兩個 monitors | PMv2 host context 可確認 | DIP/physical pairs、DPI、bounds、rounding | selection intent 與 source pixels 對齊 | DPI、scale、rects、pixel error `TBD` | backend 不能保證 crop fidelity | 不保存私人影像 | CAP-GATE-003 | 不決定 UI framework |
| `CAP-SPIKE-005` | 驗證 region crop fidelity | 001, 002, 003, 004, 005 | WinUI 3, WPF | 已知格線/色塊 fixture | source mapping baseline | source bounds、selection、crop、boundary policy | 預期邊界像素一致且誤差標準可追蹤 | crop rect、rounding、pixel error `TBD` | 需調整 contract 或淘汰該 path | fixture metadata only | CAP-SPIKE-003, 004 | 不做 PNG/annotation |
| `CAP-SPIKE-006` | 驗證 overlay self-capture | 001, 002, 003, 004, 005 | WinUI 3, WPF | synthetic overlay-like window | overlay visibility test authorization | included/excluded result、timing、focus、flicker observation | 行為可明確分類並可採用 policy | timestamps、visibility、border、focus | 需改為 hide/exclude/fallback 或無法採用 | 不擷取私人視窗 | CAP-GATE-005 | 不實作正式 Selection Overlay |
| `CAP-SPIKE-007` | 驗證 cursor inclusion/exclusion | 001, 002, 003, 004, 005 | WinUI 3, WPF | synthetic cursor position/fixture | cursor policy 尚未決定 | cursor present/absent、metadata、composition path | cursor 行為可明確記錄 | position、included flag、timing | 需額外合成或明確不支援 | 只用公開測試桌面 | CAP-GATE-006 | 不改產品 cursor policy |
| `CAP-SPIKE-008` | 觀察 HDR/SDR 行為 | 001, 002, 003, 005 | WinUI 3, WPF | public/synthetic HDR and SDR fixture | HDR hardware/session 授權 | format、color metadata、conversion、mixed topology | 風險可界定；非宣稱 HDR supported | format、profile、metadata、conversion path | HDR risk remains blocking | 不保存畫面；metadata only | CAP-GATE-007 | 不選 color pipeline |
| `CAP-SPIKE-009` | 觀察 protected-content 與 secure boundary | 001, 002, 003, 004 | WinUI 3, WPF | synthetic protected/public boundary where lawful | 不接觸私人 DRM；授權確認 | denied/black/unavailable/error classification | 不繞過安全邊界且 failure semantics 明確 | status、error class、session state | 必須拒絕、降級或提示，不可假成功 | 禁止 bypass；不保存 content | CAP-GATE-008 | 不研究繞過技術 |
| `CAP-SPIKE-010` | 驗證 failure/device-loss recovery | 001, 002, 003, 005 | WinUI 3, WPF | controlled display/session/device events | 可安全觸發的公開測試條件 | invalidation、release/recreate、retry、cleanup | failure 可分類、resource 可回收、recovery 可重現 | event、latency、attempt、resource state | 無法 recovery 則不得宣稱 resilient | 不保存 desktop frames | CAP-GATE-009 | 不建立產品 retry service |
| `CAP-SPIKE-011` | 驗證 WinUI 3/WPF interop | 001, 002, 003, 005 | WinUI 3, WPF | 相同 synthetic fixture | 各 host 的單獨授權與 baseline | callability、frame ownership、coordinate、cleanup | 兩 host 的結果與限制可比較 | host、thread、resource owner、status | 需 host-specific adapter 或放棄候選 | 不建產品 UI | CAP-GATE-010 | 不決定 ADR-0002 |
| `CAP-SPIKE-012` | 觀察 first-frame/resource 使用量 | 001, 002, 003, 005 | WinUI 3, WPF | 重複 synthetic capture cycle | product thresholds 仍 `TBD` | first-frame、allocation、release、CPU/GPU/memory | 可取得可重複觀測；不預設通過 | timing、working set、GPU、buffers | 形成後續 KPI 與 resource risk | metadata only | CAP-SPIKE-001, CAP-GATE-011 | 不發布、不做 production benchmark |

## 18. Evidence Gap Register

| Gap ID | Missing claim | Candidate | Related criteria | Related gate | Official sources checked | Why insufficient | Required future evidence | Local inspection required | Build required | Runtime required | Blocks feasibility conclusion | Status |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| `CAP-GAP-001` | Virtual desktop 全範圍與 output mapping | 001, 002, 003 | 003, 005, 006 | 002 | `CAP-OFF-EVID-001`, `003`, `004`, `007` | 文件描述 display/output 能力，但未給 SnipPlus 的 topology contract | 兩個 synthetic monitors、bounds、origin、mapping | No | No | Yes | Yes | Open |
| `CAP-GAP-002` | 負 X/Y 與 mixed-DPI crop fidelity | All | 006, 007, 008 | 003, 004 | `CAP-OFF-EVID-009`, `010`, `011` | point conversion 文件不足以證明完整 frame/crop pipeline | signed coordinate fixture、DPI matrix、pixel comparison | No | No | Yes | Yes | Open |
| `CAP-GAP-003` | Overlay self-capture 是否可控制 | All | 009, 010 | 005 | `CAP-OFF-EVID-002`, `012` | border、window exclusion 與 capture path 限制不等價 | controlled overlay visibility/exclusion/timing evidence | No | No | Yes | Yes | Open |
| `CAP-GAP-004` | Cursor inclusion/exclusion contract | 001, 002, 003 | 011 | 006 | `CAP-OFF-EVID-003` | DXGI metadata 不代表其他候選或產品 policy | synthetic cursor evidence、included flag、timing | No | No | Yes | Yes | Open |
| `CAP-GAP-005` | HDR/wide-color/mixed SDR 行為 | 001, 002, 003, 005 | 013, 014 | 007 | `CAP-OFF-EVID-004`, `006`, `008` | fixed format 或 no color management 無法完成 HDR 結論 | lawful synthetic HDR/SDR observation與metadata | No | No | Yes | Yes | Open |
| `CAP-GAP-006` | Protected content 與 secure desktop semantics | All | 015, 016 | 008 | `CAP-OFF-EVID-002`, `004`, `005`, `012` | 官方限制未形成 SnipPlus failure classification | public/synthetic boundary observation，不繞過保護 | No | No | Yes | Yes | Open |
| `CAP-GAP-007` | Device loss、mode change、desktop switch recovery | 001, 002, 005 | 019 | 009 | `CAP-OFF-EVID-001`, `005` | recreate/release 文件尚未證明產品 recovery path | controlled invalidation、cleanup、recreate evidence | No | No | Yes | Yes | Open |
| `CAP-GAP-008` | Packaged/unpackaged permission 與 lifecycle | 001, 002, 003, 004 | 020 | 010 | `CAP-OFF-EVID-001`, `005` | API 文件不足以完成 host packaging boundary | authorized host matrix、manifest/consent/lifecycle evidence | No | No | Yes | Yes | Open |
| `CAP-GAP-009` | WinUI 3/WPF frame ownership 與 coordinate interop | 001, 002, 003, 005 | 021 | 010 | `CAP-OFF-EVID-001`, `010`, `011` | framework guidance 不等於 capture resource interop result | same-fixture two-host comparison | No | No | Yes | Yes | Open |
| `CAP-GAP-010` | First-frame latency與resource量測方法 | All | 017, 018 | 001, 011 | Official capability docs only | 沒有產品 thresholds 或實際 measurement | authorized metadata-only measurement protocol | No | No | Yes | Yes | Open |
| `CAP-GAP-011` | Occluded/minimized window behavior | 001, 004 | 012 | 001, 005 | `CAP-OFF-EVID-001` and window-oriented evidence incomplete | source semantics 隨 API/window state 變化，官方 baseline 不足 | public window state matrix | No | No | Yes | Yes | Open |
| `CAP-GAP-012` | Exact Window-oriented candidate identity | 004 | 001, 002, 012, 020, 021 | 001, 010 | No single API selected by scope | candidate family 不是足夠的 API identity | separate authorized candidate research | No | No | Yes | Yes | Open |

### 18.1 Gap interpretation

「尚未找到足夠官方文件」不能被解讀為「不支援」。Gap 只表示目前不能作可靠 feasibility conclusion；`Local inspection required`、`Build required`、`Runtime required` 的 `No/Yes` 是本研究文件邊界，不是已執行紀錄。

## 19. Evidence Readiness

### 19.1 Readiness vocabulary

| Readiness | Meaning |
|---|---|
| `Sufficient for Capture ADR` | 官方與 runtime evidence 足以開始 Capture ADR，且所有 blocking gates 已被處理。 |
| `Partially sufficient` | 能開始下一輪受控 evidence work，但仍有 blocking gaps，不能形成 Capture ADR。 |
| `Insufficient for Capture ADR` | 核心 capability、coordinate、overlay、color、security 或 recovery evidence 尚不足。 |

### 19.2 Current readiness

| Field | Current value |
|---|---|
| Evidence Readiness | Partially sufficient |
| Runtime Verification | Not performed |
| Capture Execution Authorized | No |
| Capture Decision | Not made |
| Rendering Decision | Not made |
| UI Framework Decision | Unresolved — `ADR-0002` remains Draft |
| Blocking gaps | `CAP-GAP-001` through `CAP-GAP-012` remain Open |
| Blocking gates | `CAP-GATE-002` through `CAP-GATE-007`, `CAP-GATE-010`, `CAP-GATE-011` require future prototype evidence |

`Partially sufficient` 僅表示官方 baseline 足以規劃下一輪 evidence work；不表示任何 Capture candidate 已被選取，也不表示已授權執行。若後續治理要求更保守的門檻，本文件可在 review 時改為 `Insufficient for Capture ADR`，但目前不以狀態文字掩蓋已完成的官方 baseline。

## 20. Traceability

### 20.1 Product-to-evidence chain

| Product requirement / boundary | CAP criterion | Official evidence | Candidate | CAP Gate | Future CAP Spike | Future decision artifact |
|---|---|---|---|---|---|---|
| 取得一次可用的桌面來源 | `CAP-002`, `CAP-004` | `CAP-OFF-EVID-001`, `003`, `007` | 001, 002, 003 | `CAP-GATE-001` | `CAP-SPIKE-001` | Future Capture Backend ADR |
| 支援多顯示器與虛擬桌面 | `CAP-003`, `CAP-005`, `CAP-006` | `CAP-OFF-EVID-003`, `004`, `006`, `009` | 001, 002, 003, 005 | `CAP-GATE-002` | `CAP-SPIKE-002`, `003` | Future Capture Backend ADR |
| mixed-DPI 與 physical crop | `CAP-007`, `CAP-008` | `CAP-OFF-EVID-009`, `010`, `011` | All | `CAP-GATE-003`, `004` | `CAP-SPIKE-004`, `005` | Future Capture Backend ADR |
| 不捕捉 Selection Overlay | `CAP-009`, `CAP-010` | `CAP-OFF-EVID-002`, `012` | All | `CAP-GATE-005` | `CAP-SPIKE-006` | Future Capture Backend ADR |
| Cursor 行為可預期 | `CAP-011` | `CAP-OFF-EVID-003` | 001, 002, 003 | `CAP-GATE-006` | `CAP-SPIKE-007` | Future Capture Backend ADR |
| HDR/SDR risk 可界定 | `CAP-013`, `CAP-014` | `CAP-OFF-EVID-006`, `008` | 001, 002, 003, 005 | `CAP-GATE-007` | `CAP-SPIKE-008` | Future Capture Backend ADR |
| Security boundary 不被繞過 | `CAP-015`, `CAP-016` | `CAP-OFF-EVID-002`, `004`, `005`, `012` | All | `CAP-GATE-008` | `CAP-SPIKE-009` | Future Capture Backend ADR |
| failure/recovery 可分類 | `CAP-019` | `CAP-OFF-EVID-001`, `005` | 001, 002, 005 | `CAP-GATE-009` | `CAP-SPIKE-010` | Future Capture Backend ADR |
| host compatibility 可評估 | `CAP-020`, `CAP-021` | `CAP-OFF-EVID-001`, `010`, `011` | All | `CAP-GATE-010` | `CAP-SPIKE-011` | Future Capture Backend ADR |
| evidence 不含私人桌面資料 | `CAP-022` | Official evidence baseline；local/runtime 未執行 | All | `CAP-GATE-011` | `CAP-SPIKE-012` | Future Capture Backend ADR |

### 20.2 Upstream repository traceability

| Upstream artifact | Relationship |
|---|---|
| `Architecture/TECHNOLOGY-DECISION-ROADMAP.md` | `TD-003` 是 Capture Backend；依 `TD-001`、`TD-002` 與 `MOD-008` boundary；目前沒有 technical selection。 |
| `Architecture/adr/ADR-0002-ui-framework-selection.md` | `ADR-0002` 只處理 UI Framework，仍為 Draft；本研究不改 UI decision。 |
| `docs/Research/Technology/01-ui-framework-feasibility.md` | `RESEARCH-TECH-UI-001` 提供 UI host、overlay、multi-monitor、DPI 與 input 的上游研究邊界；本文件不取代它。 |
| `docs/Research/Technology/10-rendering-technology-feasibility.md` | `RESEARCH-TECH-RENDER-001` 處理 Rendering Technology；Capture Backend boundary 維持獨立。 |
| Frozen PRD | 產品需求中的 desktop capture、selection、privacy 與 workflow boundary 是本研究的 product traceability source；本文件不改 PRD。 |
| Capture / Workflow / Platform Specs | 提供 capture intent、coordinate、failure、privacy 與 host boundary 的規格來源；實際 contract 仍須經後續治理確認。 |
| `ARCH-0002` | Capture Backend 的模組/責任邊界上游來源；本文件以 ownership boundary 作可行性對照。 |
| `ARCH-0003` | Workflow/Selection 與 capture intent 的邊界上游來源。 |
| `ARCH-0004` | Platform/input/coordinate 與 desktop boundary 的上游來源。 |
| `ARCH-0005` | Reliability/privacy/failure boundary 的上游來源。 |

## 21. Completion Boundary

### 21.1 Completed by this document

- 固定 Capture Backend 的五個候選策略與 identity boundary。
- 建立 `CAP-001` 至 `CAP-022` 共 22 項 criteria。
- 建立 22-row candidate comparison matrix。
- 建立 `CAP-GATE-001` 至 `CAP-GATE-011` 共 11 個 critical gates。
- 建立 coordinate contract、overlay self-capture、color/HDR/pixel format、security/privacy 與 ownership boundary。
- 建立 `CAP-SPIKE-001` 至 `CAP-SPIKE-012` 共 12 個 future runtime spikes。
- 建立 `CAP-GAP-001` 至 `CAP-GAP-012` evidence gap register。
- 建立 official evidence baseline 與 upstream traceability。
- 明確保留 `Runtime Verification: Not performed`、`Capture Execution Authorized: No`、`Capture Decision: Not made`、`Rendering Decision: Not made`。

### 21.2 Explicitly not completed

- 沒有 Capture candidate selection。
- 沒有 Capture ADR。
- 沒有 Capture API 呼叫、project、prototype、source code、runtime result 或 screenshot artifact。
- 沒有修改 UI framework、Rendering Technology、Selection Overlay、Clipboard、PNG 或產品 workflow。
- 沒有下載、安裝、restore、build、run、publish、test 或 runtime verification。

### 21.3 Review handoff

下一個治理動作應是由具名授權者決定是否授權某一組 `CAP-SPIKE-*`，並明確指定 authority、role、date、synthetic environment、evidence retention 與 stop conditions。沒有該授權前，不得把本文件的 `Requires runtime prototype` 轉成已完成證據，也不得建立 Capture ADR。

