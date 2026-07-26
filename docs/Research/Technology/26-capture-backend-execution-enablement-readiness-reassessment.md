# Capture Backend Execution Enablement Readiness Reassessment

| Field | Value |
|---|---|
| Document ID | RESEARCH-TECH-CAPTURE-007 |
| Title | Capture Backend Execution Enablement Readiness Reassessment |
| Status | Draft |
| Research Type | Evidence-based Enablement Reassessment |
| Parent Official Evidence Baseline | RESEARCH-TECH-CAPTURE-006 |
| Parent Enablement Specification | RESEARCH-TECH-CAPTURE-005 |
| Parent Closure Plan | RESEARCH-TECH-CAPTURE-004 |
| Parent Execution Readiness | RESEARCH-TECH-CAPTURE-003 |
| Official-source Research | Not performed in this document |
| Local Environment Inspection | Not performed |
| Package Cache Inspection | Not performed |
| Build Verification | Not performed |
| Runtime Verification | Not performed |
| Closure Execution Authorized | No |
| Capture Runtime Spike Authorized | No |
| Evidence Write Authorized | No |
| UI Framework Decision | Unresolved — ADR-0002 remains Draft |
| Rendering Decision | Not made |
| Capture Decision | Not made |
| Authorization Request Created | No |
| Owner | TBD |
| Last reviewed | Not reviewed |

## 1. Purpose

本文件只回答：

> 納入 RESEARCH-TECH-CAPTURE-006 的 Microsoft 官方證據後，CAP-ENABLE-001 至 CAP-ENABLE-007 是否已具備提交 Capture prerequisite closure execution authorization review 的充分規格？

這是 Readiness Reassessment，不是：

- 新的 Official Research。
- Closure Execution。
- Authorization Request。
- Human Authorization Decision。
- Build／Runtime Record。
- Capture Backend Decision。
- Capture ADR。

本文件只重新評估既有證據、既有缺口、既有候選與既有授權邊界，不把文件完成誤寫為執行授權或技術能力驗證。

## 2. Scope

重新評估範圍：

- CAP-OFF-EVID-001..012。
- CAP-OFF-GAP-001..015。
- CAP-OPT-001..005。
- CAP-PAIR-001..010。
- CAP-PREQ-001..030。
- CAP-BLOCK-001..012。
- CAP-BA-001..007。
- CAP-CLOSE-001..007。
- CAP-ENABLE-001..007。
- CAP-CGATE-001..010。
- Phase C1 authorization packaging readiness。

Phase C2／C3 只能作為 Deferred dependency，不得全部升格為 Phase C1 authorization blocker。任何 Phase C1 blocker 必須能說明它如何阻止安全、可追溯且範圍明確的 prerequisite closure authorization review。

## 3. Non-goals

本文件不得：

- 進行新的官方網路研究。
- 執行本機盤點或 Package Cache 查詢。
- 執行 dotnet --info、workload、AppX、Registry 或 SDK inventory。
- 下載、安裝或 Restore。
- 建立 Project、Solution、Prototype、Source Code 或 Result directory。
- 執行 Build、Run、Publish 或 Capture API。
- 擷取任何桌面、視窗、Frame 或 Recording。
- 建立 Screenshot、PNG、Log、Measurement 或 Evidence Artifact。
- 建立 CAP-AUTH。
- 修改 RESEARCH-TECH-CAPTURE-001..006。
- 修改 UI／Rendering Research Line。
- 修改 ADR-0002。
- 建立 Capture ADR。
- 選擇 Capture Backend。
- 開始正式截圖功能。

## 4. Controlled Vocabulary

### 4.1 Evidence Acceptance Status

只能使用：

- Accepted。
- Accepted with limitation。
- Insufficient。
- Conflicting。
- Not applicable。

Accepted 只表示官方 claim 可納入重新評估；不表示本機 API、SDK、Header、Project、Package、Restore、Build 或 Runtime 已存在或通過。

### 4.2 Evidence Gap Disposition

只能使用：

- Open。
- Accepted documentation limitation。
- Requires local inspection。
- Requires package acquisition evidence。
- Requires project evidence。
- Requires build evidence。
- Requires runtime evidence。
- Requires evidence-write authority。
- Deferred Phase C2。
- Deferred Phase C3。

### 4.3 Enablement Reassessment Status

只能使用：

- Specified。
- Partially specified。
- Blocked。
- Deferred。
- Not applicable。

三套 vocabulary 不得混用。Gate specification status 不得使用 Satisfied、Passed 或 Resolved。

### 4.4 Authorization Boundary Terms

本文件使用下列獨立 operation classes：

| Operation class | 本文件處理方式 |
|---|---|
| Local inspection | 未執行；只能列為後續證據需求 |
| Package acquisition | 未執行；不能由官方文件代替 |
| Project creation | 未執行；與 Restore、Build 分開 |
| Restore | 未執行；不因 Sample 存在而推定成功 |
| Build | 未執行；不等同 Run 或 Runtime |
| Runtime | 未執行；不等同 Capture API authorization |
| Capture API | 未授權；本文件不呼叫 |
| Evidence persistence | 未授權；不建立 Evidence Artifact |
| Display mutation | 未授權；不修改使用者桌面狀態 |

## 5. Official Evidence Acceptance Matrix

覆蓋正好 12 筆 CAP-OFF-EVID：

| Evidence ID | Claim | Candidate | Host | Acceptance status | Limitation | Reassessment use |
|---|---|---|---|---|---|---|
| CAP-OFF-EVID-001 | Windows App SDK 可作為 Windows desktop host 的官方平台背景 | All | WinUI 3／WPF | Accepted with limitation | 不證明本 Repository 的 target、package 或 local availability | Host/framework boundary |
| CAP-OFF-EVID-002 | Windows App SDK UI Interop 提供 HWND、HMONITOR 及相關 interop 身分 | WGC、DXGI、GDI、Window-oriented | WinUI 3／WPF | Accepted with limitation | 不證明每個 Candidate 的 source creation 或 Host runtime path | Handle and interop identity |
| CAP-OFF-EVID-003 | App window management 可處理不同 desktop host 的 window identity／interop context | Window-oriented、WGC | WinUI 3／WPF | Accepted with limitation | 不證明 capture frame、occlusion 或 overlay semantics | Host activation context |
| CAP-OFF-EVID-004 | Windows Graphics Capture 的 item、frame pool、session 與畫面擷取流程可由官方文件辨識 | CAP-OPT-001 | WinUI 3／WPF | Accepted with limitation | 不證明本機 API、frame fidelity、crop 或 product contract | Candidate and frame identity |
| CAP-OFF-EVID-005 | GraphicsCaptureItem 的 window／display source identity 及其 API surface 可由官方文件辨識 | CAP-OPT-001 | WinUI 3／WPF | Accepted | 不證明 Host integration、permission result 或 frame acquisition runtime | Source identity |
| CAP-OFF-EVID-006 | DXGI Desktop Duplication 的 output duplication、AcquireNextFrame 及 surface identity 可由官方文件辨識 | CAP-OPT-002 | WinUI 3／WPF | Accepted with limitation | 不證明 managed host interop、topology recovery 或 product coordinate contract | Output and frame identity |
| CAP-OFF-EVID-007 | BitBlt 提供 GDI transfer semantics，包含 CAPTUREBLT 相關 API boundary | CAP-OPT-003 | WinUI 3／WPF | Accepted with limitation | 不證明 logical／physical coordinate policy、overlay semantics 或 local behavior | GDI source and transfer |
| CAP-OFF-EVID-008 | PrintWindow 提供 HWND／HDC window-oriented rendering boundary | CAP-OPT-004 | WinUI 3／WPF | Accepted with limitation | 不證明 minimized、occluded、layered 或 composed content 的產品語義 | Window-oriented limitation |
| CAP-OFF-EVID-009 | GetDIBits 提供 HBITMAP／DIB readback identity | CAP-OPT-003、CAP-OPT-004 | WinUI 3／WPF | Accepted | 不證明 GPU texture readback、crop fidelity 或 image export policy | CPU readback boundary |
| CAP-OFF-EVID-010 | Windows app development 與 UI migration guidance 保留 WinUI 3／WPF framework boundary | All | WinUI 3／WPF | Accepted with limitation | ADR-0002 尚未決定，不能視為本產品 Host decision | Shared UI dependency |
| CAP-OFF-EVID-011 | Screen capture official material provides documented frame/pixel-format caveats | CAP-OPT-001、CAP-OPT-002 | WinUI 3／WPF | Accepted with limitation | Pixel format 不等於 alpha、PNG、crop 或 pixel-difference pass | Pixel and frame limits |
| CAP-OFF-EVID-012 | Desktop app guidance separates API、package、deployment and host concerns | All | WinUI 3／WPF | Accepted with limitation | 不證明本 Repository 的 Project、Restore、Build、output 或 cleanup | Packaging and operation boundary |

規則：

- Accepted 只接受官方文件實際涵蓋的 claim。
- Microsoft Sample 只能支持其實際展示範圍。
- 同一 Evidence 不得擴張成未記載的 Host、Packaging 或 Runtime support。
- 12 筆 Evidence 不足以直接推出 Capture Candidate selection。

## 6. Official Evidence Gap Disposition Matrix

覆蓋正好 15 個 CAP-OFF-GAP：

| Gap ID | Candidate | Host | Current gap | Disposition | Blocks Phase C1 authorization request | Required next evidence |
|---|---|---|---|---|---|---|
| CAP-OFF-GAP-001 | All | WinUI 3／WPF | Exact local framework、SDK、target identity 未知 | Requires local inspection | Yes | Read-only environment record |
| CAP-OFF-GAP-002 | All | WinUI 3／WPF | Exact API／SDK／package／interop version identity 未封閉 | Requires package acquisition evidence | Yes | Versioned package and API identity record |
| CAP-OFF-GAP-003 | WGC、DXGI、GDI、Window-oriented | WinUI 3／WPF | Candidate–Host activation route 未由本 Repository 證明 | Requires project evidence | Yes | Isolated host/project specification and later record |
| CAP-OFF-GAP-004 | WGC、DXGI | WinUI 3／WPF | Source item、output、frame domain 與 product source mapping 未封閉 | Requires runtime evidence | Yes | Authorized source/frame observation |
| CAP-OFF-GAP-005 | DXGI Desktop Duplication | WinUI 3／WPF | Output、rotation、topology 及 device lifecycle 未封閉 | Requires runtime evidence | Yes | Authorized per-output recovery evidence |
| CAP-OFF-GAP-006 | GDI-based capture | WinUI 3／WPF | Logical／physical unit、DPI conversion 及 bitmap contract 未封閉 | Requires runtime evidence | Yes | Authorized DPI and bitmap evidence |
| CAP-OFF-GAP-007 | Window-oriented mechanisms | WinUI 3／WPF | Occluded、minimized、layered、composed window semantics 未封閉 | Deferred Phase C2 | No; deferred | Lawful synthetic window scenarios |
| CAP-OFF-GAP-008 | Hybrid strategy | WinUI 3／WPF | Shared frame、coordinate and fallback semantics 不是單一官方 API claim | Accepted documentation limitation | Yes | Constituent identity and product contract |
| CAP-OFF-GAP-009 | All | WinUI 3／WPF | Negative coordinates、edges、rounding、off-by-one 未封閉 | Requires runtime evidence | Yes | Coordinate fixture and deterministic comparison |
| CAP-OFF-GAP-010 | All | WinUI 3／WPF | Overlay self-capture、exclusion、system indication 未完整證明 | Deferred Phase C2 | No; deferred | Lawful overlay scenarios and policy review |
| CAP-OFF-GAP-011 | All | WinUI 3／WPF | Protected content、Secure Desktop、UAC、denied access behavior 未封閉 | Deferred Phase C2 | No; deferred | Platform boundary evidence and lawful scenarios |
| CAP-OFF-GAP-012 | WGC、DXGI | WinUI 3／WPF | Device-loss、topology-change、access-lost recovery sequence 未封閉 | Deferred Phase C3 | No; deferred | Authorized recovery runtime evidence |
| CAP-OFF-GAP-013 | WGC、DXGI | WinUI 3／WPF | GPU texture 到 CPU readback、crop、pixel difference 未封閉 | Requires runtime evidence | Yes | Authorized readback and fidelity evidence |
| CAP-OFF-GAP-014 | All | WinUI 3／WPF | Official docs 無法證明本 Repository 的 Package Restore、Build、output、cleanup | Requires build evidence | Yes | Isolated project/build record |
| CAP-OFF-GAP-015 | All | WinUI 3／WPF | Privacy、retention、evidence root、cleanup owner 未決定 | Requires evidence-write authority | Yes | Human-owned governance and evidence authorization |

Gap disposition 的判定規則：

- API identity、Host activation、graphics-device boundary、Project scope、Package／Restore／Build scope不明時，保持 Phase C1 阻塞。
- HDR、完整 Cursor、完整 Overlay exclusion、Recovery performance 與後期 resource observation 可 Deferred。
- Accepted documentation limitation 不代表技術能力已驗證。
- 缺少官方聲明不得改寫成 Not supported。

## 7. Candidate Identity Reassessment

建立正好五列；Recommendation 只表示規格狀態，不形成排名或產品選擇：

| Candidate | API identity completeness | Host activation evidence | Device／frame evidence | Packaging evidence | Remaining uncertainty | Recommendation |
|---|---|---|---|---|---|---|
| CAP-OPT-001 Windows Graphics Capture | Candidate identity identified | API／WinRT／HWND route identifiable; Host integration unverified | D3D11 frame-pool identity documented; CPU comparison open | Windows SDK／Windows App SDK identity known at family level; package target TBD | Local availability、Host source creation、coordinate、crop、privacy | Candidate remains in reassessment; no selection |
| CAP-OPT-002 DXGI Desktop Duplication | Candidate identity identified | Native COM／DXGI route identifiable; managed Host integration unverified | Output duplication and frame surface documented; recovery and readback open | Windows SDK family identifiable; project target TBD | Adapter/output mapping、rotation、device loss、coordinate | Candidate remains in reassessment; no selection |
| CAP-OPT-003 GDI-based capture | Candidate identity identified | HDC／HBITMAP native route identifiable; Host boundary unverified | DIB readback documented; product pixel contract open | Windows desktop API family identifiable; target TBD | DPI、logical units、overlay and fidelity | Candidate remains in reassessment; no selection |
| CAP-OPT-004 Window-oriented mechanisms | Partially identified | HWND／HDC route identifiable; source semantics incomplete | PrintWindow result boundary documented; frame fidelity open | Desktop API identity identifiable; host behavior TBD | Occlusion、minimized、layered、composition | Candidate remains in reassessment; no exclusion without evidence |
| CAP-OPT-005 Hybrid strategy | Blocked by unresolved official evidence | Constituent routes identifiable only | Mixed frame and coordinate contract not defined | Candidate-dependent; no single package identity | Fallback trigger、shared contract、ownership and recovery | Strategy composition remains TBD; no selection |

## 8. Candidate–Host Pair Reassessment

覆蓋 CAP-PAIR-001..010，WinUI 3 與 WPF 保持分開：

| Pair | Previous readiness | Accepted official evidence | Remaining local／project／build／runtime requirement | New recommendation | Blocking IDs |
|---|---|---|---|---|---|
| CAP-PAIR-001 WGC × WinUI 3 | Blocked | CAP-OFF-EVID-001、002、004、005 | Local identity、package、Host source creation、frame and coordinate runtime | Partially specified; retain for closure review | CAP-OFF-GAP-001、002、003、004 |
| CAP-PAIR-002 WGC × WPF | Blocked | CAP-OFF-EVID-001、002、003、004、005 | Local identity、WPF interop、package、Host source creation、frame runtime | Partially specified; retain for closure review | CAP-OFF-GAP-001、002、003、004 |
| CAP-PAIR-003 DXGI × WinUI 3 | Blocked | CAP-OFF-EVID-001、002、006 | Local identity、native interop、output mapping、device and frame runtime | Partially specified; retain for closure review | CAP-OFF-GAP-001、002、003、005 |
| CAP-PAIR-004 DXGI × WPF | Blocked | CAP-OFF-EVID-001、002、006 | Local identity、WPF native interop、output mapping、device and frame runtime | Partially specified; retain for closure review | CAP-OFF-GAP-001、002、003、005 |
| CAP-PAIR-005 GDI × WinUI 3 | Blocked | CAP-OFF-EVID-001、002、007、009 | Local identity、HDC/HBITMAP interop、DPI and bitmap runtime | Partially specified; retain for closure review | CAP-OFF-GAP-001、002、003、006 |
| CAP-PAIR-006 GDI × WPF | Blocked | CAP-OFF-EVID-001、007、009 | Local identity、WPF HDC/HBITMAP interop、DPI and bitmap runtime | Partially specified; retain for closure review | CAP-OFF-GAP-001、002、003、006 |
| CAP-PAIR-007 Window-oriented × WinUI 3 | Blocked | CAP-OFF-EVID-002、003、008 | Local identity、window source contract、occlusion and frame runtime | Deferred for later validation; not excluded | CAP-OFF-GAP-001、003、007、010 |
| CAP-PAIR-008 Window-oriented × WPF | Blocked | CAP-OFF-EVID-002、003、008 | Local identity、WPF window source contract、occlusion and frame runtime | Deferred for later validation; not excluded | CAP-OFF-GAP-001、003、007、010 |
| CAP-PAIR-009 Hybrid × WinUI 3 | Blocked | CAP-OFF-EVID-004、006、007、008 | Constituent identity、shared contract、fallback and recovery evidence | Blocked; strategy remains TBD | CAP-OFF-GAP-002、003、008、009 |
| CAP-PAIR-010 Hybrid × WPF | Blocked | CAP-OFF-EVID-004、006、007、008 | Constituent identity、WPF interop、shared contract、fallback evidence | Blocked; strategy remains TBD | CAP-OFF-GAP-002、003、008、009 |

Pair rules：

- API 可呼叫不代表 Host source creation 已驗證。
- Host activation 文件不代表可成功取得 Frame。
- Sample Build 不得被推定為目前 Repository Build。
- Unknown 不得在沒有直接證據時改為 Excluded with evidence。
- Pair recommendation 不得形成 Capture Candidate selection。

## 9. Activation／Interop Reassessment

| Candidate–Host | Source creation identity | Handle／object identity | Interop route | Thread／dispatcher requirement | Documentary completeness | Remaining evidence |
|---|---|---|---|---|---|---|
| WGC × WinUI 3 | Picker、DisplayId、WindowId source identity documented at API level | GraphicsCaptureItem、HWND、HMONITOR、WindowId or DisplayId | WinRT／Windows App SDK interop | Dispatcher and lifetime behavior in this Host unverified | Partially specified | Host project and runtime source creation |
| WGC × WPF | Window／display item identity available at API level | GraphicsCaptureItem plus HWND／HMONITOR mapping | WPF HWND with WinRT／COM interop | Host integration and dispatcher behavior unverified | Partially specified | WPF isolated project and runtime |
| DXGI × WinUI 3 | Per-output duplication identity documented | IDXGIOutput、IDXGIOutputDuplication、D3D11 device | Native COM／DXGI／D3D11 interop | Thread and device lifetime unverified | Partially specified | Adapter/output and device runtime evidence |
| DXGI × WPF | Per-output duplication identity documented | IDXGIOutput、IDXGIOutputDuplication、D3D11 device | WPF native interop | Host integration and lifetime unverified | Partially specified | WPF isolated project and runtime |
| GDI × WinUI 3 | HDC／HBITMAP source identity documented | HWND、HDC、HBITMAP | wingdi.h／User32 native interop | UI-thread cost and ownership unverified | Partially specified | HDC lifetime and bitmap runtime record |
| GDI × WPF | HDC／HBITMAP source identity documented | HWND、HDC、HBITMAP | WPF Win32 interop | UI-thread cost and ownership unverified | Partially specified | WPF bitmap runtime record |
| Window-oriented × WinUI 3 | PrintWindow HWND source identity documented | HWND and target HDC | winuser.h／User32 interop | Synchronous owner-render behavior documented; product behavior open | Partially specified | Lawful window scenarios |
| Window-oriented × WPF | PrintWindow HWND source identity documented | HWND and target HDC | WPF HWND／User32 interop | Synchronous owner-render behavior documented; product behavior open | Partially specified | Occlusion and minimized scenarios |
| Hybrid × WinUI 3 | Constituent source creation selected later | Candidate-dependent object set | Multiple WinRT／COM／DXGI／GDI paths | Shared ownership and fallback thread model TBD | Insufficient | Constituent decision and contract |
| Hybrid × WPF | Constituent source creation selected later | Candidate-dependent object set | Multiple WinRT／COM／DXGI／GDI paths | Shared ownership and fallback thread model TBD | Insufficient | Constituent decision and contract |

本矩陣不宣稱任何 Host 已建立、已 Build 或已取得 Frame。沒有直接官方證據時，不自行推定 threading、dispatcher、lifetime 或 source creation behavior。

## 10. Graphics Device／Frame Reassessment

| Candidate | Device requirement | Frame acquisition identity | Pixel format evidence | CPU access／readback evidence | Static specification status | Runtime need |
|---|---|---|---|---|---|---|
| Windows Graphics Capture | Direct3D11 device for Direct3D11CaptureFramePool | GraphicsCaptureItem、Direct3D11CaptureFramePool、Direct3D11CaptureFrame | Official frame/pixel-format caveat accepted with limitation | GPU texture to CPU comparison remains open | Partially specified | Yes for readback、crop、fidelity、cleanup |
| DXGI Desktop Duplication | D3D11 device/context and per-output duplication | IDXGIOutputDuplication、AcquireNextFrame surface | B8G8R8A8_UNORM identity documented | CPU processing and comparison remain open | Partially specified | Yes for readback、rotation、device recovery |
| GDI-based capture | Device contexts and compatible bitmap | HDC、HBITMAP、DIB | GDI transfer/readback documented; product format policy open | GetDIBits readback documented, product comparison open | Partially specified | Yes for DPI、bitmap、crop、fidelity |
| Window-oriented mechanisms | HWND source and target HDC | PrintWindow HDC result | Pixel semantics depend on owner render | Separate readback path required | Partially specified | Yes for occlusion、minimized、composition |
| Hybrid strategy | Each constituent device path | Mixed frame objects | Shared pixel contract TBD | Conversion and comparison TBD | Insufficient | Yes after constituent identity |

固定邊界：

- Frame acquisition 不等於 Crop fidelity。
- GPU texture 不等於 CPU pixel comparison。
- Pixel format 已知不等於 PNG export 已完成。
- Alpha behavior 未確認時保持 Unknown。
- Device recreation 文件不等於 Recovery observation。

## 11. Coordinate Evidence Reassessment

Rounding policy: TBD。

| Capability | Official contribution | Static definition status | Remaining runtime requirement | Phase | Blocking effect |
|---|---|---|---|---|---|
| Virtual desktop | API／environment bounds 可作為輸入 | Partially specified | Read-only topology record and product mapping | C1 | Blocks coordinate contract |
| Per-output／per-monitor source | Output-oriented source identity is documented | Partially specified | Per-output mapping observation | C1 | Blocks source mapping |
| Negative coordinates | Official APIs do not close SnipPlus signed contract | Insufficient | Signed-coordinate fixture | C1 | Blocks deterministic crop |
| Physical pixel bounds | Frame／output dimensions are documented at API level | Partially specified | Physical bounds record | C1 | Blocks crop evidence |
| DPI responsibility | GDI logical-unit behavior is documented; host policy open | Insufficient | Per-monitor DPI observation | C1 | Blocks mixed-DPI contract |
| Rotation | DXGI documentation describes rotation handling boundary | Partially specified | Multi-output rotation fixture | C2 | Deferred unless C1 safety scope depends on it |
| Frame size | Item／surface dimensions can be recorded | Partially specified | Frame-to-source mapping | C1 | Blocks crop mapping |
| Source bounds | Source identity and frame bounds remain separate | Partially specified | Source bounds record | C1 | Blocks evidence schema |
| Crop conversion | No product inclusive／exclusive contract in API docs | Insufficient | Deterministic crop fixture | C1 | Blocks off-by-one closure |
| Inclusive／exclusive edges | Not closed by official API material | Insufficient | Edge comparison record | C1 | Blocks exact-region claim |
| Rounding policy | No product decision | TBD | Human-owned contract and runtime confirmation | C1 | Blocks mixed-DPI claim |
| Off-by-one detection | Requires expected／observed comparison | Insufficient | One-pixel border fixture | C1 | Blocks fidelity claim |
| Topology timing | Display changes and frame timing remain product flow concerns | Insufficient | Authorized topology-change observation | C3 | Deferred recovery dependency |

本矩陣不得形成產品 Coordinate ADR。Mixed-DPI runtime behavior不得由文件推定為已通過。

## 12. Security／Privacy／Failure Reassessment

| Boundary | Accepted official behavior | Static specification effect | Runtime verification still required | Deferred allowed | Blocking ID |
|---|---|---|---|---|---|
| Protected content | Platform may limit or alter capture behavior; no bypass claim accepted | Preserve lawful limitation and explicit status | Yes if in product scope | Yes, C2 | CAP-OFF-GAP-011 |
| Secure Desktop／UAC | No evidence permits bypassing secure boundaries | No bypass requirement may be added | Yes only under lawful test authority | Yes, C2 | CAP-OFF-GAP-011 |
| User consent／picker | Picker and source identity are separate from product authorization | Consent path must be documented independently | Yes for Host behavior | Yes, C2 | CAP-OFF-GAP-003 |
| Capture access denial | Denial remains a possible boundary | Failure state and stop rule must be specified | Yes for recovery behavior | Yes, C2 | CAP-OFF-GAP-011 |
| Session isolation | Official API material does not prove cross-session behavior | Keep as Unknown | Yes when in scope | Yes, C2 | CAP-OFF-GAP-011 |
| Black／blank frame | Frame source and visual result are not interchangeable | Blank frame must not be declared success | Yes for observation | Yes, C2 | CAP-OFF-GAP-004 |
| Source closure | Item/session/frame lifecycle remains separate from product cleanup | Cleanup boundary must be explicit | Yes for Dispose/close | Yes, C3 | CAP-OFF-GAP-012 |
| Output duplication access lost | Recovery boundary is candidate-specific | Do not infer recovery success from error documentation | Yes for observation | Yes, C3 | CAP-OFF-GAP-012 |
| Display topology change | Topology timing is not a product contract | Record as deferred dependency | Yes for recovery | Yes, C3 | CAP-OFF-GAP-012 |
| Frame-pool resize | Resize identity does not prove correct crop or fidelity | Keep resize and crop separate | Yes for runtime | Yes, C3 | CAP-OFF-GAP-013 |
| Cleanup／Dispose | API lifecycle guidance does not prove repository cleanup | Require owner, root and retention boundary | Yes after evidence authority | No for C1 governance | CAP-OFF-GAP-015 |

不得規劃規避平台安全限制。Security documentation 不等於 privacy review；failure code 或 recovery guidance 不等於實際 Recovery 已驗證。

## 13. Enablement Item Reassessment

建立正好七列；只提出新建議，不修改 RESEARCH-TECH-CAPTURE-005：

| Enablement Item | Previous status | Accepted Evidence IDs | Relevant Gap IDs | Specification improvement | Remaining gap | New status recommendation |
|---|---|---|---|---|---|---|
| CAP-ENABLE-001 | Partially specified | CAP-OFF-EVID-001、002、003、010 | CAP-OFF-GAP-001、002、003 | Framework、Host、interop and local identity are explicitly separated | Shared UI authority、exact local target and Host source creation remain open | Partially specified |
| CAP-ENABLE-002 | Partially specified | CAP-OFF-EVID-004、005、006、007、008、009 | CAP-OFF-GAP-002、003、004、005、006、008 | Five candidate identities and ten Host pairs remain distinct | Exact version、interop and source behavior remain open | Partially specified |
| CAP-ENABLE-003 | Blocked | CAP-OFF-EVID-001、012 | CAP-OFF-GAP-001、002、014 | Project、Package、Restore、Build are separate operation classes | No local or project evidence exists | Blocked |
| CAP-ENABLE-004 | Partially specified | CAP-OFF-EVID-004、005、008、011 | CAP-OFF-GAP-004、007、010、011 | Synthetic scene and source behavior are separated from official claims | Window、overlay、protected and scene behavior remain runtime/policy gaps | Partially specified |
| CAP-ENABLE-005 | Partially specified | CAP-OFF-EVID-004、006、007、009、011 | CAP-OFF-GAP-004、005、006、009、013 | Coordinate、crop、frame and readback obligations are explicit | Rounding、edges、DPI and fidelity remain open | Partially specified |
| CAP-ENABLE-006 | Partially specified | CAP-OFF-EVID-004、006、008、012 | CAP-OFF-GAP-011、012、014、015 | Privacy、evidence and cleanup are separated from API identity | Evidence owner、retention and build/evidence authority remain open | Partially specified |
| CAP-ENABLE-007 | Blocked | CAP-OFF-EVID-005、006、008 | CAP-OFF-GAP-007、010、012、015 | Runtime, recovery and evidence write remain independent boundaries | No runtime or evidence-write authorization exists | Blocked |

## 14. Closure Gate Reassessment

覆蓋 CAP-CGATE-001..010。Gate specification status 只能使用 Specified、Partially specified、Blocked 或 Deferred：

| Closure Gate | Official evidence contribution | Documentary requirement status | Remaining non-documentary requirement | Gate specification status |
|---|---|---|---|---|
| CAP-CGATE-001 | Host/framework boundary and UI interop identity | Partially specified | Exact local framework／SDK identity and shared UI authority | Blocked |
| CAP-CGATE-002 | Candidate API／SDK identity | Specified | Exact package/version and candidate decision remain separate | Specified |
| CAP-CGATE-003 | Candidate–Host project／interop boundary | Partially specified | Host-specific source creation and isolated project evidence | Blocked |
| CAP-CGATE-004 | Basic synthetic scene and source semantics | Partially specified | Scene acceptance and source behavior runtime evidence | Partially specified |
| CAP-CGATE-005 | Virtual desktop／monitor／negative-coordinate model | Partially specified | Product coordinate contract and topology record | Blocked |
| CAP-CGATE-006 | Crop and off-by-one method | Partially specified | Rounding, edge and fidelity evidence | Blocked |
| CAP-CGATE-007 | Frame／metadata／privacy evidence obligation | Partially specified | Evidence schema, privacy owner and evidence-write authority | Blocked |
| CAP-CGATE-008 | Project／Restore／Build／Runtime／Evidence separation | Insufficient | Independent operation authorizations and repository evidence | Blocked |
| CAP-CGATE-009 | Result storage and cleanup boundary | Insufficient | Retention, root, cleanup owner and evidence authorization | Blocked |
| CAP-CGATE-010 | Runtime execution remains independent authorization | Specified | Future runtime authorization and stop rules | Deferred |

## 15. Prerequisite and Blocker Impact Matrix

本矩陣逐一覆蓋 CAP-PREQ-001..030 與 CAP-BLOCK-001..012。本文件不修改上游狀態。

| Source item | Official evidence contribution | Remaining evidence class | Related Enablement Item | Phase C1 impact | Status recommendation |
|---|---|---|---|---|---|
| CAP-PREQ-001 | Host family evidence from CAP-OFF-EVID-001、010 | Local inspection | CAP-ENABLE-001 | Blocks | Requires local identity |
| CAP-PREQ-002 | Interop family evidence from CAP-OFF-EVID-002、003 | Shared UI authority | CAP-ENABLE-001 | Blocks | Partially specified |
| CAP-PREQ-003 | Candidate API family identity | Package acquisition evidence | CAP-ENABLE-002 | Blocks | Requires package evidence |
| CAP-PREQ-004 | Candidate namespace／header identity | Project evidence | CAP-ENABLE-002 | Blocks | Requires project evidence |
| CAP-PREQ-005 | Graphics device family identity | Project evidence | CAP-ENABLE-002 | Blocks | Requires project evidence |
| CAP-PREQ-006 | Frame object identity | Runtime evidence | CAP-ENABLE-002 | Blocks | Requires runtime evidence |
| CAP-PREQ-007 | Source item identity | Runtime evidence | CAP-ENABLE-002 | Blocks | Requires runtime evidence |
| CAP-PREQ-008 | Host invocation boundary | Project evidence | CAP-ENABLE-002 | Blocks | Requires project evidence |
| CAP-PREQ-009 | Native／WinRT interop boundary | Project evidence | CAP-ENABLE-002 | Blocks | Requires project evidence |
| CAP-PREQ-010 | Candidate package identity | Package acquisition evidence | CAP-ENABLE-002 | Blocks | Requires package evidence |
| CAP-PREQ-011 | Device and frame dependency mapping | Runtime evidence | CAP-ENABLE-002 | Blocks | Requires runtime evidence |
| CAP-PREQ-012 | Hybrid constituent identity | Requires project evidence | CAP-ENABLE-002 | Blocks | Strategy remains TBD |
| CAP-PREQ-013 | Basic synthetic scene contract | Experimental project | CAP-ENABLE-004 | Blocks | Requires project evidence |
| CAP-PREQ-014 | Coordinate origin and bounds | Runtime evidence | CAP-ENABLE-005 | Blocks | Requires runtime evidence |
| CAP-PREQ-015 | Negative-coordinate mapping | Runtime evidence | CAP-ENABLE-005 | Blocks | Requires runtime evidence |
| CAP-PREQ-016 | DPI and rounding policy | Runtime evidence | CAP-ENABLE-005 | Blocks | Rounding policy TBD |
| CAP-PREQ-017 | Window source behavior | Deferred Phase C2 | CAP-ENABLE-004 | No; deferred | Deferred Phase C2 |
| CAP-PREQ-018 | Overlay-like source behavior | Deferred Phase C2 | CAP-ENABLE-004 | No; deferred | Deferred Phase C2 |
| CAP-PREQ-019 | Occluded/minimized source behavior | Deferred Phase C2 | CAP-ENABLE-004 | No; deferred | Deferred Phase C2 |
| CAP-PREQ-020 | Protected-content boundary | Deferred Phase C2 | CAP-ENABLE-004 | No; deferred | Deferred Phase C2 |
| CAP-PREQ-021 | Device/topology recovery trigger | Deferred Phase C3 | CAP-ENABLE-007 | No; deferred | Deferred Phase C3 |
| CAP-PREQ-022 | Frame metadata schema | Runtime evidence | CAP-ENABLE-005 | Blocks | Requires runtime evidence |
| CAP-PREQ-023 | Crop and edge comparison | Runtime evidence | CAP-ENABLE-005 | Blocks | Requires runtime evidence |
| CAP-PREQ-024 | Privacy review owner | Requires evidence-write authority | CAP-ENABLE-006 | Blocks | Requires authority |
| CAP-PREQ-025 | Protected-content privacy boundary | Requires evidence-write authority | CAP-ENABLE-006 | Blocks | Requires authority |
| CAP-PREQ-026 | Retention and cleanup contract | Requires evidence-write authority | CAP-ENABLE-006 | Blocks | Requires authority |
| CAP-PREQ-027 | Isolated Project／Package／Restore／Build scope | Build evidence | CAP-ENABLE-003 | Blocks | Requires build evidence |
| CAP-PREQ-028 | Runtime authorization and stop rules | Requires runtime evidence | CAP-ENABLE-007 | Blocks | Authorization remains No |
| CAP-PREQ-029 | Evidence root and retention policy | Requires evidence-write authority | CAP-ENABLE-006 | Blocks | Requires authority |
| CAP-PREQ-030 | Cleanup confirmation | Requires evidence-write authority | CAP-ENABLE-006 | Blocks | Requires authority |
| CAP-BLOCK-001 | Shared Host identity unresolved | Shared UI authority | CAP-ENABLE-001 | Blocks | Open |
| CAP-BLOCK-002 | Candidate API identity not tied to project | Package acquisition evidence | CAP-ENABLE-002 | Blocks | Open |
| CAP-BLOCK-003 | Synthetic scene acceptance not evidenced | Experimental project | CAP-ENABLE-004 | Blocks | Open |
| CAP-BLOCK-004 | Coordinate mapping not closed | Runtime evidence | CAP-ENABLE-005 | Blocks | Open |
| CAP-BLOCK-005 | Overlay behavior deferred | Deferred Phase C2 | CAP-ENABLE-004 | No; deferred | Deferred Phase C2 |
| CAP-BLOCK-006 | Window/cursor behavior deferred | Deferred Phase C2 | CAP-ENABLE-004 | No; deferred | Deferred Phase C2 |
| CAP-BLOCK-007 | Crop and pixel evidence not closed | Runtime evidence | CAP-ENABLE-005 | Blocks | Open |
| CAP-BLOCK-008 | Privacy boundary not owned | Requires evidence-write authority | CAP-ENABLE-006 | Blocks | Open |
| CAP-BLOCK-009 | Evidence persistence/cleanup not authorized | Requires evidence-write authority | CAP-ENABLE-006 | Blocks | Open |
| CAP-BLOCK-010 | Project／Restore／Build scope not authorized | Build evidence | CAP-ENABLE-003 | Blocks | Open |
| CAP-BLOCK-011 | Runtime execution not authorized | Requires runtime evidence | CAP-ENABLE-007 | Blocks | Open |
| CAP-BLOCK-012 | Recovery remains later-phase dependency | Deferred Phase C3 | CAP-ENABLE-007 | No; deferred | Deferred Phase C3 |

## 16. Authorization Readiness Matrix

建立正好七列。最後一欄只使用 Yes、Partially 或 No；Yes 只表示未來可形成 Authorization Request，不代表已授權。

| Enablement Item | Required operation classes | Specification complete | Shared UI authority dependency | Capture-specific authority identifiable | R4 boundary separated | Ready to package into authorization request |
|---|---|---|---|---|---|---|
| CAP-ENABLE-001 | Local inspection、Host/framework identity、shared UI review | Partially | Pending | Partially | Yes | No |
| CAP-ENABLE-002 | Package identity、Project boundary、interop and candidate review | Partially | Pending | Partially | Yes | No |
| CAP-ENABLE-003 | Project creation、Package acquisition、Restore、Build | No | Pending | Yes | Yes | No |
| CAP-ENABLE-004 | Synthetic scene and source semantics | Partially | Pending | Partially | Yes | Partially |
| CAP-ENABLE-005 | Coordinate、crop、frame、readback evidence | No | Pending | Partially | Yes | No |
| CAP-ENABLE-006 | Privacy review、Evidence persistence、cleanup | No | Pending | No | Yes | No |
| CAP-ENABLE-007 | Runtime、Capture API、recovery and stop boundary | No | Pending | Yes | Yes | No |

R4 Capture API、Runtime、Evidence persistence 與 Display mutation 必須保持在後續獨立授權邊界。Shared UI authority 尚未核准不一定阻止形成 request，但必須阻止實際執行。

## 17. Shared UI Authority Dependency

UI-AUTH-001..008 全部維持 Pending。Capture 不得複製 Shared Host authorization。

| Shared capability | UI authority source | Current decision | Effect on authorization request readiness | Effect on execution |
|---|---|---|---|---|
| UI-AUTH-001 Host framework identity | RESEARCH-TECH-UI-007..009、ADR-0002 | Pending | Prevents complete package | Execution prohibited |
| UI-AUTH-002 Window handle ownership | RESEARCH-TECH-UI-007..009 | Pending | Requires boundary note | Execution prohibited |
| UI-AUTH-003 Overlay lifecycle | RESEARCH-TECH-UI-007..009 | Pending | May remain deferred if not C1 safety-critical | Execution prohibited |
| UI-AUTH-004 Coordinate authority | RESEARCH-TECH-UI-007..009 | Pending | Requires explicit ownership | Execution prohibited |
| UI-AUTH-005 Rendering dependency | RESEARCH-TECH-RENDER-003 | Pending | Requires shared contract reference | Execution prohibited |
| UI-AUTH-006 Project/build authority | AGENTS.md and repository workflow | Pending | Separately packageable only after explicit scope | Build prohibited |
| UI-AUTH-007 Evidence persistence authority | Future human-owned governance | Pending | Blocks evidence request package | Evidence write prohibited |
| UI-AUTH-008 Display mutation authority | Future runtime authorization boundary | Pending | Blocks any mutation scope | Display mutation prohibited |

Capture-specific API、Interop、Synthetic Scene 與實驗 Project delta 可以另行描述，但不得冒充 Shared UI authority。Project creation、Restore、Build、Runtime、Evidence persistence 與 Capture API invocation 必須分開授權。

## 18. Remaining Minimum Actions

以下只列阻止形成 Capture prerequisite closure execution authorization request 的最小事項：

| Action | Source IDs | Required evidence／specification | Documentary or execution requirement | Blocks authorization request |
|---|---|---|---|---|
| Resolve host/framework and shared authority identity | CAP-PREQ-001、002、CAP-BLOCK-001、CAP-CGATE-001 | Exact local target and owner boundary | Documentation plus authorized local inspection later | Yes |
| Resolve candidate API/package/interop identity | CAP-PREQ-003..012、CAP-BLOCK-002、CAP-CGATE-002、003 | Candidate-specific identity without selection by this document | Documentation plus package/project evidence later | Yes |
| Define isolated Project／Restore／Build scope | CAP-PREQ-027、CAP-BLOCK-010、CAP-CGATE-008 | Exact project delta, package root, restore and build scope | Separate project/build authorization later | Yes |
| Freeze C1 synthetic scene contract | CAP-PREQ-013、CAP-BLOCK-003、CAP-CGATE-004 | Scene manifest and acceptance fields | Specification first; runtime later | Yes |
| Freeze coordinate/crop/evidence method | CAP-PREQ-014..016、022、023、CAP-BLOCK-004、007 | Bounds, edges, rounding TBD owner, comparison method | Specification plus authorized runtime evidence later | Yes |
| Assign privacy/evidence/cleanup authority | CAP-PREQ-024..026、029、030、CAP-BLOCK-008、009 | Evidence root, retention, owner and cleanup contract | Human-owned authorization required | Yes |
| Separate future runtime authorization | CAP-PREQ-028、CAP-BLOCK-011、CAP-CGATE-010 | Stop rules, API/runtime/evidence boundaries | Independent runtime authorization later | Yes |

下列項目不得自動列為 Phase C1 authorization-request blocker：

- 完整 HDR branch。
- 完整 Overlay self-capture。
- 完整 Cursor behavior。
- Device-loss performance。
- CPU／GPU／memory 量測。
- Phase C2／C3 Runtime 結果。

若其中某項阻止安全操作範圍被明確描述，才可在後續文件中說明理由。

## 19. Mechanical Decision Derivation

Decision derivation 僅使用本文件矩陣：

1. 12 筆 CAP-OFF-EVID 已可接受為有限的官方 static claims。
2. 15 個 CAP-OFF-GAP 中，CAP-OFF-GAP-001、002、003、004、005、006、009、013、014、015 仍直接影響 Phase C1 的 local、project、build、coordinate、fidelity 或 evidence authority boundary。
3. CAP-ENABLE-003、CAP-ENABLE-007 維持 Blocked；其餘 Enablement Item 最多 Partially specified。
4. CAP-CGATE-001、003、005、006、007、008、009 維持 Blocked；CAP-CGATE-010 只表示未來 runtime 必須獨立授權。
5. Authorization Readiness 最後一欄沒有全部為 Yes，且 CAP-ENABLE-003、005、006、007 為 No。
6. 因此本文件的 Final Decision 為：

Final Decision: Not ready to request capture prerequisite closure execution authorization。

這個 Final Decision 只表示尚未具備形成該 request 的完整規格；不表示任何 Candidate 已排除、不表示平台不支援，也不表示未來不能提出新的 authorization review。

## 20. Fixed Status Boundary

不論 Final Decision 為何，固定：

- Authorization Request Created: No。
- Closure Execution Authorized: No。
- Local Environment Inspection: Not performed。
- Package Cache Inspection: Not performed。
- Build Verification: Not performed。
- Runtime Verification: Not performed。
- Capture Runtime Spike Authorized: No。
- Evidence Write Authorized: No。
- Capture Decision: Not made。
- Rendering Decision: Not made。

本文件沒有執行新的官方研究、本機盤點、Package Cache、下載、安裝、Restore、Build、Run、Capture API 或 Runtime Spike。

## 21. Traceability

| Trace source | Mapping | Future use | Current state |
|---|---|---|---|
| CAP-OFF-EVID-001..012 | Candidate／Host／device／frame official claim | Enablement and gate reassessment | Accepted with limitations |
| CAP-OFF-GAP-001..015 | Unresolved official or repository claim | Prerequisite and blocker disposition | Open or deferred |
| CAP-OPT-001..005 | Candidate API identity | Pair and future decision review | No selection |
| CAP-PAIR-001..010 | Candidate–Host evidence mapping | Future host prototype and runtime spike | No ranking |
| CAP-PREQ-001..030 | Prerequisite implication | Future closure plan review | Not closed |
| CAP-BLOCK-001..012 | Blocker implication | Future enablement review | Open or deferred |
| CAP-BA-001..007 | Blocking action mapping | Future closure authorization review | Referenced; not executed |
| CAP-CLOSE-001..007 | Closure action mapping | Future closure execution | Not executed |
| CAP-ENABLE-001..007 | Evidence and operation readiness | Future authorization packaging | Partially specified or Blocked |
| CAP-CGATE-001..010 | Closure gate sufficiency | Future closure authorization | Not passed |
| RESEARCH-TECH-UI-007..009 | Shared UI authority context | Future shared UI review | Inherited only |
| RESEARCH-TECH-RENDER-003 | Rendering dependency context | Future synthetic scene review | Referenced only |
| ADR-0002-ui-framework-selection.md | UI decision context | Future UI authority review | Draft; unresolved |
| Architecture/TECHNOLOGY-DECISION-ROADMAP.md | Technology decision context | Future candidate decision | No decision made |

## 完成條件

- 只建立 docs/Research/Technology/26-capture-backend-execution-enablement-readiness-reassessment.md。
- 不修改任何其他文件。
- 覆蓋 12 筆 CAP-OFF-EVID。
- 覆蓋 15 個 CAP-OFF-GAP。
- 覆蓋五個 Candidate。
- 覆蓋十個 CAP-PAIR。
- 覆蓋 30 個 CAP-PREQ 與 12 個 CAP-BLOCK。
- 建立正好七列 Enablement Reassessment。
- 建立正好十列 Closure Gate Reassessment。
- 建立正好七列 Authorization Readiness。
- Final Decision 可由矩陣機械式推導。
- 不建立 CAP-AUTH 或 Human Decision Record。
- 所有實際執行授權維持 No。
- 不執行新的官方研究、本機盤點、Package Cache、下載、安裝、Restore、Build、Run、Capture API 或 Runtime Spike。
- 不建立 Project、Prototype、Result、Source Code、Capture Frame、Screenshot 或 Evidence Artifact。
- 不修改 UI／Rendering Research Line。
- 不建立 Capture ADR。
- git diff --check 應通過。

完成後停止本文件任務，等待下一個單一文件指令。
