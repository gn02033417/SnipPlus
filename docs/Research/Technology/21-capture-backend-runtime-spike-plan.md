# Capture Backend Runtime Spike Plan

## Document Control

| Field | Value |
|---|---|
| Document ID | `RESEARCH-TECH-CAPTURE-002` |
| Title | Capture Backend Runtime Spike Plan |
| Status | Draft |
| Research Type | Runtime Evidence Plan |
| Parent Feasibility | `RESEARCH-TECH-CAPTURE-001` |
| Execution Status | Not started |
| Build Verification | Not performed |
| Runtime Verification | Not performed |
| UI Framework Decision | Unresolved — `ADR-0002` remains Draft |
| Rendering Decision | Not made |
| Capture Decision | Not made |
| Capture Runtime Spike Authorized | No |
| Owner | TBD |
| Last reviewed | Not reviewed |

本文件只規劃如何在未來以一致、可重複、可比較且不暴露私人桌面資料的方式執行 `CAP-SPIKE-001` 至 `CAP-SPIKE-012`。本文件不是執行紀錄、Capture API 實作、Project、Prototype、Result 或 Capture ADR。

## 1. Scope

本計畫只規劃：

- `CAP-OPT-001` 至 `CAP-OPT-005`。
- WinUI 3 與 WPF Host。
- `CAP-SPIKE-001` 至 `CAP-SPIKE-012`。
- Synthetic capture environment。
- Coordinate、DPI、Overlay、Cursor、HDR、Recovery 與 interoperability evidence。
- Candidate comparison rules。
- Evidence artifact requirements。
- Safety、privacy 與 cleanup boundary。

本計畫不執行任何 Spike，也不預填任何執行結果。

## 2. Spike Binding Policy

### 2.1 Upstream binding

| Rule | Requirement |
|---|---|
| Identity | 保留 `CAP-SPIKE-001` 至 `CAP-SPIKE-012` 的原始 ID。 |
| Purpose | 不刪除、合併或改變上游 Spike 的原始目的。 |
| Parent | 所有 Spike 都回溯至 `RESEARCH-TECH-CAPTURE-001`。 |
| Gap handling | 上游規格不足時建立 `CAP-PLAN-GAP-xxx`，不直接改寫 parent feasibility。 |
| Status | 十二個 Spike 的 Status 全部固定為 `Planned`。 |
| Execution | 本文件不代表任何 Spike 已獲准執行。 |

Spike Status 只能使用：`Planned`、`Ready`、`Running`、`Blocked`、`Completed`、`Invalidated`。本文件不得使用 `Running` 或 `Completed` 作為現況。

### 2.2 Candidate binding

| Candidate ID | Candidate | Bound to parent feasibility |
|---|---|---|
| `CAP-OPT-001` | Windows Graphics Capture | 保留官方 frame acquisition、consent、border 與 frame-pool 限制。 |
| `CAP-OPT-002` | DXGI Desktop Duplication API | 保留 per-output、metadata、cursor、rotation 與 device invalidation 限制。 |
| `CAP-OPT-003` | GDI-based desktop capture | 保留 DC、bitmap、`BitBlt`、layered window 與 color-management 限制。 |
| `CAP-OPT-004` | Window-oriented capture mechanisms | 保留 window source、occlusion、minimized state 與 exact API 未定的限制。 |
| `CAP-OPT-005` | Hybrid primary/fallback capture strategy | 必須明列實際組成候選，不得當成單一 API。 |

## 3. Candidate Eligibility Matrix

Eligibility 只能使用：`Eligible`、`Conditionally eligible`、`Not aligned`、`Unknown`。

Runtime inclusion 只能使用：`Planned`、`Blocked`、`Excluded with evidence`、`Not evaluated`。

| Candidate | WinUI 3 eligibility | WPF eligibility | Required API/interop | Packaging implication | Runtime inclusion | Evidence basis |
|---|---|---|---|---|---|---|
| `CAP-OPT-001` Windows Graphics Capture | Conditionally eligible | Conditionally eligible | WinRT capture item、frame pool、graphics resource interop | consent、manifest、lifecycle 需個別驗證 | Planned | `RESEARCH-TECH-CAPTURE-001` official evidence baseline |
| `CAP-OPT-002` DXGI Desktop Duplication | Conditionally eligible | Conditionally eligible | DXGI/Direct3D COM、GPU resource、device lifecycle | packaged/unpackaged 及 host interop 未定 | Planned | `RESEARCH-TECH-CAPTURE-001` official evidence baseline |
| `CAP-OPT-003` GDI-based desktop capture | Conditionally eligible | Conditionally eligible | Win32 HDC/HBITMAP、managed bitmap boundary | native interop 為必要風險；無法由 API existence 結論 | Planned | `RESEARCH-TECH-CAPTURE-001` official evidence baseline |
| `CAP-OPT-004` Window-oriented capture mechanisms | Unknown | Unknown | exact API、window identity、source ownership `TBD` | exact packaging/consent boundary `TBD` | Planned | candidate family；需另行補齊 identity |
| `CAP-OPT-005` Hybrid primary/fallback capture strategy | Conditionally eligible | Conditionally eligible | selected candidate 的複合 interop 與 normalized contract | 取決於實際 primary/fallback composition | Planned | derived strategy；不是單一 API evidence |

規則：`Unknown` 不得直接變成 `Excluded with evidence`；API 存在不得作為 `Eligible` 的唯一理由；Window-oriented 必須保留 source limitation；Hybrid 必須列出實際組成候選。

## 4. Controlled Comparison Rules

未來執行時，所有候選必須遵守下列比較控制：

1. 使用相同 Windows Build、CPU architecture 與 GPU。
2. 使用相同 Host Framework configuration。
3. 使用相同 Synthetic capture scene。
4. 使用相同 Display topology 與 DPI 情境。
5. 使用相同 physical-pixel crop rectangle。
6. 使用相同 logical selection intent。
7. Cold start 與 warm start 分開記錄。
8. 首 frame 與後續 frame 分開記錄。
9. Candidate-specific optimization 必須揭露。
10. 不得只為單一候選改變 Synthetic scene。
11. Capture success 不代表 coordinate fidelity 通過。
12. Frame 取得不代表 Overlay exclusion 通過。
13. Debug 與 Release 結果不得混合比較。
14. Packaged 與 unpackaged 結果不得混合比較。
15. Hardware 與 software conversion 必須分開記錄。
16. Prototype 不得直接進入產品 Source tree。
17. 若候選需要不同前置條件，必須在 Environment Record 中明列，不得用缺少前置條件掩蓋比較差異。
18. 若某項 evidence 只在單一 host 出現，結果只能標為 host-specific，不得泛化到另一 host。

## 5. Synthetic Capture Scene Contract

本節只規劃 scene，不建立 scene、project、image 或 capture artifact。

### 5.1 Required scene elements

- 固定 Canvas pixel size。
- 固定 logical size。
- 高對比色塊。
- 1-pixel 邊界線。
- 座標刻度。
- 四角識別標記。
- 中心點標記。
- 中英文文字。
- Alpha gradient。
- Wide-color/SDR 測試色塊。
- Cursor target。
- Overlay-like synthetic window。
- 部分遮擋 window。
- 最小化 window 情境。
- Negative-coordinate monitor positioning。
- Same-DPI multi-monitor。
- Mixed-DPI multi-monitor。
- HDR/SDR monitor combination。
- Protected-content substitute；不使用真正受保護內容。
- Device-loss/display-change trigger plan。

### 5.2 Explicit exclusions

不得使用：

- 使用者真實桌面。
- 私人應用程式。
- 電子郵件、通訊軟體或瀏覽器內容。
- 真實 Credential、Token 或個人資料。
- 正式 SnipPlus Overlay。
- 正式 Screenshot workflow。
- 真實 DRM 或任何需要繞過保護的內容。

### 5.3 Scene invariants

| Invariant | Requirement |
|---|---|
| Geometry | 每次執行使用相同已核准 scene geometry。 |
| Coordinates | scene 內的 marker 必須能對應到預先定義的 synthetic coordinates。 |
| Color | 色塊與 profile metadata 必須分別記錄；不可用肉眼描述代替。 |
| Content | scene 不得包含私人資訊。 |
| Host | WinUI 3 與 WPF scene 需能使用相同的意圖與對照資料。 |
| Cleanup | 執行後不保留未授權 frame、crop 或 diagnostic artifact。 |

## 6. Coordinate Verification Contract

### 6.1 Coordinate domains

| Coordinate domain | Owner | Unit | Origin | Required evidence |
|---|---|---|---|---|
| Virtual desktop coordinates | Platform/host boundary | physical pixel | virtual-screen origin | signed origin、virtual bounds、topology |
| Monitor physical coordinates | Capture Backend boundary | physical pixel | monitor top-left | monitor bounds、rotation、identity abstraction |
| Host logical DIP coordinates | UI host boundary | logical DIP | host/client origin | DPI context、scale、conversion input |
| Selection intent coordinates | Workflow/Selection | defined logical or physical unit `TBD` | selection owner origin | intent、timestamp、DPI context |
| Capture-source coordinates | Capture Backend | source pixel | source frame origin | source bounds、source origin、frame metadata |
| Frame-local coordinates | Capture Backend | physical pixel | frame top-left | frame size、format、row orientation |
| Crop rectangle | crop boundary | physical pixel | source frame origin | input rect、converted rect、rounding、edge semantics |
| Export pixel coordinates | future output boundary | physical pixel | output top-left | expected dimensions、crop result、format metadata |

### 6.2 Required verification rules

- Negative coordinate handling 必須保存 signed values。
- Logical-to-physical conversion 必須保存 conversion context。
- Rounding mode 維持 `TBD`，直到正式決策。
- Inclusive/exclusive edge semantics 必須明確記錄。
- Monitor topology change 必須使舊 mapping 重新評估。
- Frame timestamp 與 selection timestamp 必須可比較。
- Expected crop pixel dimensions 必須由輸入 contract 推導，不由實際輸出反推。
- Off-by-one detection 必須使用 synthetic 1-pixel 邊界線。
- Capture Backend 不得修改 Selection intent。
- Capture Backend 必須回報不能可靠映射的狀態，而不是靜默校正。

### 6.3 Coordinate observation record

未來每個受影響 Spike 至少記錄：

| Field | Required value |
|---|---|
| Selection intent | 原始意圖與單位 |
| Selection timestamp | 執行時記錄 |
| DPI context | host 與 monitor context |
| Virtual origin | signed physical coordinates |
| Source bounds | source frame physical bounds |
| Converted crop | 轉換後 physical rectangle |
| Rounding | 實際使用值或 `TBD` |
| Expected size | 由 contract 推導的尺寸 |
| Observed size | 執行時記錄；目前不預填 |
| Pixel boundary result | `Not executed` 直到取得 evidence |

## 7. Environment Record

未來每次 Spike 必填；現在不填入不存在的結果。

| Field | Requirement |
|---|---|
| Windows edition/build | 執行時記錄 |
| CPU architecture | 執行時記錄 |
| CPU/GPU/driver | 執行時記錄 |
| Host Framework/version | 執行時記錄 |
| Candidate API/version | 執行時記錄 |
| SDK/Runtime | 執行時記錄 |
| Packaged state | `Packaged` 或 `Unpackaged` |
| Monitor count | 執行時記錄 |
| Monitor bounds | 執行時記錄 |
| Primary monitor | 執行時記錄 |
| DPI scaling per monitor | 執行時記錄 |
| HDR state per monitor | 執行時記錄 |
| Color profile/space | 執行時記錄 |
| Hardware acceleration | `On`、`Off` 或 `Unknown` |
| Build configuration | `Debug` 或 `Release`；未執行前不預填 |
| Test timestamp | 執行時記錄 |

## 8. Evidence Types

未來可使用的 Evidence type 固定為：

- `Environment record`
- `Functional observation`
- `Frame metadata`
- `Coordinate mapping record`
- `Synthetic source reference`
- `Captured frame artifact`
- `Crop output artifact`
- `Pixel-difference result`
- `Color/pixel-format metadata`
- `Timing measurement`
- `CPU/GPU/memory observation`
- `Failure reproduction`
- `Recovery observation`
- `Diagnostic log`
- `Privacy review`
- `Cleanup confirmation`

本文件只定義未來 Evidence，不建立任何 Artifact。

## 9. Functional and Measurement Vocabulary

### 9.1 Functional Result

只能使用：`Pass`、`Fail`、`Blocked`、`Not executed`。

### 9.2 Comparative metrics

可記錄：

- first-frame duration。
- one-shot completion duration。
- crop dimension difference。
- coordinate offset。
- pixel-difference count。
- CPU usage。
- GPU usage。
- memory usage。
- recovery duration。
- resource cleanup observation。

### 9.3 Threshold-dependent metrics

產品門檻尚未核准時固定使用：

- `Threshold: TBD`
- `Decision use: Informative only`

不得自行設定毫秒、FPS、記憶體、色差或像素差門檻。

## 10. Privacy-preserving Evidence Rules

未來執行時必須遵守：

1. Runtime Evidence 只能來自 synthetic scene。
2. 不保存真實桌面影像。
3. 不顯示私人 window title。
4. Diagnostic log 必須移除使用者名稱及不相關路徑。
5. Window handle 可匿名化。
6. Monitor device identity 只保留技術必要欄位。
7. Protected-content 測試不得規避 Windows 安全限制。
8. Secure Desktop 不得以自動化方式嘗試繞過。
9. Evidence review 必須確認沒有私人影像或 Credential。
10. 發現私人內容時立即停止，並依已核准清理政策處理未核准 Artifact。
11. 不輸出完整 environment variables、credential stores、NuGet config 或 Registry export。
12. 不把錯誤訊息中的帳號、token、路徑或視窗標題帶入結果。

## 11. Per-Spike Execution Specification

每個 Spike 固定使用以下欄位：Spike ID、Title、Status、Related CAP criteria、Related CAP Gates、Candidate technologies、Eligible Host Frameworks、Purpose、Dependencies、Preconditions、Synthetic environment、Display/DPI variations、Execution sequence、Required evidence、Functional pass condition、Coordinate verification、Crop fidelity verification、Timing measurements、Resource observations、Privacy checks、Failure condition、Failure implication、Recovery expectation、Known limitations、Stop conditions、Cleanup、Future result destination、Open questions。

所有 `Status` 固定為 `Planned`。`Execution sequence` 只描述未來操作，不包含 Source Code；任何 Spike 規格都不得被解讀為正式 Capture Architecture。

### 11.1 `CAP-SPIKE-001` — Single-monitor one-shot capture

| Field | Plan |
|---|---|
| Spike ID | `CAP-SPIKE-001` |
| Title | Single-monitor one-shot capture |
| Status | Planned |
| Related CAP criteria | `CAP-001`, `CAP-002`, `CAP-004`, `CAP-017`, `CAP-022` |
| Related CAP Gates | `CAP-GATE-001`, `CAP-GATE-011` |
| Candidate technologies | `CAP-OPT-001`, `002`, `003`, `004`, `005` |
| Eligible Host Frameworks | WinUI 3、WPF；逐 host 分開記錄 |
| Purpose | 確認候選能否取得一個 synthetic source frame，而非長時間錄影。 |
| Dependencies | Synthetic scene、Environment Record、human authorization |
| Preconditions | 單一 monitor、固定 scene、候選 identity 與 privacy boundary 已確認 |
| Synthetic environment | 固定 canvas、marker、1-pixel border、public test content |
| Display/DPI variations | baseline single monitor；DPI 變體另由 `004` 處理 |
| Execution sequence | 建立環境記錄；準備 scene；依候選取得 single frame；記錄 metadata；清理未授權 artifact |
| Required evidence | environment、functional observation、frame metadata、privacy review、cleanup confirmation |
| Functional pass condition | 可取得一次可識別來源且 metadata 完整；不等同 crop 或 overlay 通過 |
| Coordinate verification | 記錄 monitor bounds、source origin、frame size；不預填結果 |
| Crop fidelity verification | 只記錄是否安排測量；實際結果由 `005` 取得 |
| Timing measurements | first-frame、one-shot completion；threshold `TBD` |
| Resource observations | buffer、CPU/GPU/memory、cleanup observation |
| Privacy checks | synthetic only；無私人視窗、Credential 或桌面影像保存 |
| Failure condition | API unavailable、source frame unavailable、metadata incomplete 或 privacy boundary 失敗 |
| Failure implication | candidate 尚不能作 basic baseline；不得直接淘汰其他候選 |
| Recovery expectation | 記錄 failure classification；recovery 由 `010` 處理 |
| Known limitations | 單 monitor 不代表 virtual desktop、DPI 或 HDR 通過 |
| Stop conditions | 需要真實桌面、未核准權限、未核准 package/build/runtime 或 scene 不可重現 |
| Cleanup | 釋放候選資源；不建立未授權結果檔 |
| Future result destination | `docs/Research/Technology/results/capture/CAP-SPIKE-001-result.md`，本輪不建立 |
| Open questions | first-frame threshold、format、host-specific cleanup |

### 11.2 `CAP-SPIKE-002` — Virtual desktop capture

| Field | Plan |
|---|---|
| Spike ID | `CAP-SPIKE-002` |
| Title | Virtual desktop capture |
| Status | Planned |
| Related CAP criteria | `CAP-003`, `CAP-005`, `CAP-008` |
| Related CAP Gates | `CAP-GATE-002`, `CAP-GATE-004` |
| Candidate technologies | `CAP-OPT-001`, `002`, `003`, `005` |
| Eligible Host Frameworks | WinUI 3、WPF |
| Purpose | 比較多 monitor topology 與 virtual desktop coverage。 |
| Dependencies | `001` baseline、synthetic two-monitor scene、coordinate contract |
| Preconditions | 兩個 synthetic monitors、固定 arrangement、合法 authorization |
| Synthetic environment | 同 scene 置於兩個 monitor；四角 marker 與 origin marker |
| Display/DPI variations | same-DPI、不同排列、topology metadata |
| Execution sequence | 記錄 topology；準備相同 scene；依候選觀察 coverage；記錄 bounds 與 frame metadata |
| Required evidence | monitor bounds、virtual origin、source mapping、functional observation、privacy review |
| Functional pass condition | 預期 bounds 可對應，且沒有 silent crop；未涵蓋的 output 必須明確分類 |
| Coordinate verification | signed virtual origin、每 monitor physical bounds、source mapping |
| Crop fidelity verification | 只安排，不在本 Spike 宣告 crop 通過 |
| Timing measurements | first-frame 與 one-shot time；threshold `TBD` |
| Resource observations | multi-output buffers、GPU/CPU/memory observation |
| Privacy checks | 只使用 synthetic monitors；不保存 desktop content |
| Failure condition | 只取得單一 output、bounds 不可映射、topology 變動或資料不完整 |
| Failure implication | 候選可能只能作 per-monitor source；需保留 hybrid/分 monitor 路徑研究 |
| Recovery expectation | topology 變更交由 `010` 分析 |
| Known limitations | coverage 不代表 mixed-DPI 或 negative-coordinate 正確 |
| Stop conditions | 需要真實使用者 monitor content、系統設定變更或未核准權限 |
| Cleanup | 釋放 frame/source；不保存未核准影像 |
| Future result destination | `docs/Research/Technology/results/capture/CAP-SPIKE-002-result.md`，本輪不建立 |
| Open questions | virtual origin contract、per-output merge、rotation handling |

### 11.3 `CAP-SPIKE-003` — Negative-coordinate monitor

| Field | Plan |
|---|---|
| Spike ID | `CAP-SPIKE-003` |
| Title | Negative-coordinate monitor |
| Status | Planned |
| Related CAP criteria | `CAP-006`, `CAP-007`, `CAP-008` |
| Related CAP Gates | `CAP-GATE-002`, `CAP-GATE-003`, `CAP-GATE-004` |
| Candidate technologies | `CAP-OPT-001`, `002`, `003`, `005` |
| Eligible Host Frameworks | WinUI 3、WPF |
| Purpose | 驗證左側/上方 monitor 的 signed coordinates 與 source mapping。 |
| Dependencies | `002` topology baseline、coordinate record |
| Preconditions | 可控 negative-coordinate synthetic monitor、固定 scene、authorization |
| Synthetic environment | marker 跨越 virtual origin；左側或上方 output |
| Display/DPI variations | same-DPI baseline；mixed-DPI 由 `004` 處理 |
| Execution sequence | 記錄 signed bounds；準備 marker；觀察來源 mapping；記錄 crop input/output metadata |
| Required evidence | signed coordinates、mapping、functional observation、off-by-one observation |
| Functional pass condition | 負 X/Y 保留且 mapping 可重現；不得以絕對值或靜默位移修正 |
| Coordinate verification | virtual origin、monitor bounds、frame-local coordinates、rounding `TBD` |
| Crop fidelity verification | 以 1-pixel border 設計 future comparison；結果未執行 |
| Timing measurements | selection/source timestamp；threshold `TBD` |
| Resource observations | allocation/cleanup metadata |
| Privacy checks | synthetic only；不保存真實 desktop |
| Failure condition | signed value 遺失、origin 錯誤、mapping 不可重現 |
| Failure implication | coordinate contract 未通過；候選不可宣稱 virtual desktop correctness |
| Recovery expectation | topology change 交由 `010` |
| Known limitations | 不涵蓋 mixed-DPI scale、HDR 或 overlay exclusion |
| Stop conditions | 需要改 system display layout 或讀取私人內容 |
| Cleanup | 清除 temporary in-memory state；不寫出 artifact |
| Future result destination | `docs/Research/Technology/results/capture/CAP-SPIKE-003-result.md`，本輪不建立 |
| Open questions | negative origin ownership、rounding、rotation interplay |

### 11.4 `CAP-SPIKE-004` — Mixed-DPI coordinate mapping

| Field | Plan |
|---|---|
| Spike ID | `CAP-SPIKE-004` |
| Title | Mixed-DPI coordinate mapping |
| Status | Planned |
| Related CAP criteria | `CAP-007`, `CAP-008`, `CAP-021` |
| Related CAP Gates | `CAP-GATE-003`, `CAP-GATE-004`, `CAP-GATE-010` |
| Candidate technologies | `CAP-OPT-001`, `002`, `003`, `004`, `005` |
| Eligible Host Frameworks | WinUI 3、WPF；各 host 分開 |
| Purpose | 驗證 logical DIP、physical pixel、monitor DPI 與 crop input 的分層。 |
| Dependencies | `003` coordinate baseline、PMv2 host context、synthetic fixture |
| Preconditions | 兩個不同 DPI synthetic monitors；host awareness 可記錄 |
| Synthetic environment | 相同 marker 與 known border，置於不同 DPI output |
| Display/DPI variations | 至少兩個 DPI；實際 scale 執行時記錄，不在此文件決定 |
| Execution sequence | 記錄 host/monitor DPI；建立同一 selection intent；觀察 source mapping；記錄 conversion fields |
| Required evidence | DIP/physical pairs、DPI、bounds、rounding、host comparison |
| Functional pass condition | selection intent 與 source physical pixels 對齊；若不能，明確標記 limitation |
| Coordinate verification | logical-to-physical record、monitor context、edge semantics |
| Crop fidelity verification | 以 border/marker future comparison；不預填 pixel result |
| Timing measurements | conversion/capture timing；threshold `TBD` |
| Resource observations | host resource ownership、cleanup、memory |
| Privacy checks | synthetic scene、no private window title |
| Failure condition | scale context 遺失、host mismatch、off-by-one 或 silent OS scaling |
| Failure implication | backend 不能保證 crop fidelity；需 adapter/constraint 或候選降級 |
| Recovery expectation | DPI change 需重新建立 mapping；由 future evidence 記錄 |
| Known limitations | 不決定 UI Framework 或 formal DPI policy |
| Stop conditions | 需要修改 system DPI、未核准 host/project 或私人內容 |
| Cleanup | 釋放 host/source resources；不建立 output artifact |
| Future result destination | `docs/Research/Technology/results/capture/CAP-SPIKE-004-result.md`，本輪不建立 |
| Open questions | rounding mode、DIP ownership、host-specific conversion |

### 11.5 `CAP-SPIKE-005` — Region crop fidelity

| Field | Plan |
|---|---|
| Spike ID | `CAP-SPIKE-005` |
| Title | Region crop fidelity |
| Status | Planned |
| Related CAP criteria | `CAP-008`, `CAP-013`, `CAP-014` |
| Related CAP Gates | `CAP-GATE-004`, `CAP-GATE-007` |
| Candidate technologies | `CAP-OPT-001`, `002`, `003`, `004`, `005` |
| Eligible Host Frameworks | WinUI 3、WPF |
| Purpose | 用已知 synthetic pattern 驗證 source bounds、selection、crop 與 boundary policy。 |
| Dependencies | `003`、`004` coordinate mapping、known fixture |
| Preconditions | crop contract、expected dimensions、rounding rule status 已記錄 |
| Synthetic environment | grid、1-pixel border、corner markers、color blocks |
| Display/DPI variations | single、negative、mixed-DPI scenarios from prior spikes |
| Execution sequence | 記錄 expected rect；取得 future source；轉換 crop；比對 marker/dimensions；記錄差異 |
| Required evidence | source/crop bounds、dimension、pixel-difference、functional observation |
| Functional pass condition | 預期 boundary pixels 對齊且差異可分類；未核准 threshold 不宣稱通過 |
| Coordinate verification | 來源 origin、converted rect、inclusive/exclusive、rounding |
| Crop fidelity verification | 1-pixel line、corner marker、expected/observed size、difference count |
| Timing measurements | conversion/crop duration；threshold `TBD` |
| Resource observations | source/crop buffer lifecycle、CPU/memory |
| Privacy checks | fixture only；不保存私人 image |
| Failure condition | crop dimension mismatch、offset、missing marker 或 format conversion unknown |
| Failure implication | crop gate remains open；不得直接選 candidate |
| Recovery expectation | crop failure 不應被誤報為 capture unavailable；分類為 fidelity failure |
| Known limitations | future PNG/encoder 不在此 Spike 決定 |
| Stop conditions | 需要正式 product pipeline、PNG storage 或私人內容 |
| Cleanup | 不保留 frame/crop；依授權範圍清理 temporary data |
| Future result destination | `docs/Research/Technology/results/capture/CAP-SPIKE-005-result.md`，本輪不建立 |
| Open questions | accepted pixel difference、color comparison method、rounding |

### 11.6 `CAP-SPIKE-006` — Overlay self-capture behavior

| Field | Plan |
|---|---|
| Spike ID | `CAP-SPIKE-006` |
| Title | Overlay self-capture behavior |
| Status | Planned |
| Related CAP criteria | `CAP-009`, `CAP-010`, `CAP-012` |
| Related CAP Gates | `CAP-GATE-005`, `CAP-GATE-011` |
| Candidate technologies | `CAP-OPT-001`, `002`, `003`, `004`, `005` |
| Eligible Host Frameworks | WinUI 3、WPF |
| Purpose | 觀察 source 是否包含 overlay-like synthetic window，以及 exclusion/timing 的限制。 |
| Dependencies | `001` source baseline、synthetic overlay-like window、privacy authorization |
| Preconditions | 不使用正式 SnipPlus Overlay；只使用 synthetic test window |
| Synthetic environment | public synthetic content plus overlay-like rectangle/marker |
| Display/DPI variations | single、multi-monitor、mixed-DPI coverage where authorized |
| Execution sequence | 記錄 overlay visibility/timestamps；觀察 source；比較 include/exclude state；記錄 focus/flicker |
| Required evidence | included/excluded observation、timing、focus、border、privacy review |
| Functional pass condition | self-capture behavior 可明確分類；不可把 borderless consent 當成 exclusion proof |
| Coordinate verification | overlay/source bounds、monitor、timestamp 關係 |
| Crop fidelity verification | 僅檢查 synthetic overlay marker 是否進入 crop；不判定 product crop |
| Timing measurements | visibility-to-capture timing、flicker observation；threshold `TBD` |
| Resource observations | window/source lifecycle、cleanup |
| Privacy checks | 不使用真實視窗；不保存私人內容 |
| Failure condition | overlay inclusion 不可預期、排除只在特定 path、focus changed 或 flicker |
| Failure implication | 需要明確 hide/exclude/fallback policy；gate remains open |
| Recovery expectation | overlay policy failure 不應重試成成功 frame；分類回報 |
| Known limitations | 不決定正式 Overlay implementation 或 policy |
| Stop conditions | 需要正式 product overlay、window affinity bypass 或 security bypass |
| Cleanup | 關閉 synthetic window；清理 temporary observation |
| Future result destination | `docs/Research/Technology/results/capture/CAP-SPIKE-006-result.md`，本輪不建立 |
| Open questions | exclusion ownership、DWM applicability、focus policy |

### 11.7 `CAP-SPIKE-007` — Cursor inclusion/exclusion

| Field | Plan |
|---|---|
| Spike ID | `CAP-SPIKE-007` |
| Title | Cursor inclusion/exclusion |
| Status | Planned |
| Related CAP criteria | `CAP-011`, `CAP-022` |
| Related CAP Gates | `CAP-GATE-006`, `CAP-GATE-011` |
| Candidate technologies | `CAP-OPT-001`, `002`, `003`, `004`, `005` |
| Eligible Host Frameworks | WinUI 3、WPF |
| Purpose | 觀察 cursor 是否包含、可控制或需額外合成。 |
| Dependencies | `001` baseline、public synthetic cursor target |
| Preconditions | cursor policy 尚未決定；不得把 observation 變成 product policy |
| Synthetic environment | known cursor position/target、synthetic scene |
| Display/DPI variations | single、negative、mixed-DPI target where authorized |
| Execution sequence | 記錄 cursor position；依候選觀察 source；記錄 included flag/metadata；清理 |
| Required evidence | cursor present/absent、metadata、timing、functional observation |
| Functional pass condition | cursor 行為可明確記錄並可比較；未知狀態不得假設排除 |
| Coordinate verification | cursor position 與 frame/source/monitor coordinates |
| Crop fidelity verification | target 是否位於 expected crop；不建立 image artifact |
| Timing measurements | cursor/source timestamp；threshold `TBD` |
| Resource observations | cursor metadata/resource cleanup |
| Privacy checks | synthetic cursor；不收集使用者輸入內容 |
| Failure condition | cursor behavior path-specific、位置映射錯誤或無法分類 |
| Failure implication | 需額外 composition 或 explicit limitation |
| Recovery expectation | cursor loss 不應被當作 frame loss |
| Known limitations | 不決定最終 cursor product policy |
| Stop conditions | 需要攔截私人輸入、global hook 或未核准 workflow |
| Cleanup | 停止 synthetic cursor fixture；清理 metadata scope |
| Future result destination | `docs/Research/Technology/results/capture/CAP-SPIKE-007-result.md`，本輪不建立 |
| Open questions | cursor ownership、composition timing、per-candidate control |

### 11.8 `CAP-SPIKE-008` — HDR/SDR observation

| Field | Plan |
|---|---|
| Spike ID | `CAP-SPIKE-008` |
| Title | HDR/SDR observation |
| Status | Planned |
| Related CAP criteria | `CAP-013`, `CAP-014`, `CAP-018` |
| Related CAP Gates | `CAP-GATE-007`, `CAP-GATE-011` |
| Candidate technologies | `CAP-OPT-001`, `002`, `003`, `005` |
| Eligible Host Frameworks | WinUI 3、WPF |
| Purpose | 界定 HDR、wide color、SDR conversion、format 與 metadata risk。 |
| Dependencies | `005` crop baseline、lawful synthetic HDR/SDR fixture、hardware authorization |
| Preconditions | HDR hardware/session 可合法使用；沒有私人內容 |
| Synthetic environment | public/synthetic HDR and SDR color blocks |
| Display/DPI variations | mixed HDR/SDR monitors；實際狀態需記錄 |
| Execution sequence | 記錄 monitor color state；觀察 source format/metadata；比較 conversion path；記錄 unknown |
| Required evidence | format、profile、metadata、conversion path、functional observation |
| Functional pass condition | risk 可界定；不得宣稱 HDR supported 只因取得 frame |
| Coordinate verification | monitor/source bounds；避免 color observation 混入 coordinate conclusion |
| Crop fidelity verification | 只用 color blocks/metadata 識別；不宣告 PNG fidelity |
| Timing measurements | conversion/capture timing；threshold `TBD` |
| Resource observations | hardware/software conversion、GPU/memory |
| Privacy checks | synthetic color fixture；不保存 desktop image |
| Failure condition | format/metadata 不明、mixed state 不可重現或 conversion path unknown |
| Failure implication | HDR gate remains open；需要 explicit limitation/unsupported policy |
| Recovery expectation | display mode change 交由 `010`；不以 retry 掩蓋 color loss |
| Known limitations | 不選 color pipeline、PNG encoder 或 output profile policy |
| Stop conditions | 需要修改 HDR system settings、讀取私人內容或繞過 protected content |
| Cleanup | 恢復/清理測試資源；不保留畫面 |
| Future result destination | `docs/Research/Technology/results/capture/CAP-SPIKE-008-result.md`，本輪不建立 |
| Open questions | source format、alpha、profile retention、SDR conversion owner |

### 11.9 `CAP-SPIKE-009` — Protected-content and secure-boundary observation

| Field | Plan |
|---|---|
| Spike ID | `CAP-SPIKE-009` |
| Title | Protected-content and secure-boundary observation |
| Status | Planned |
| Related CAP criteria | `CAP-015`, `CAP-016`, `CAP-022` |
| Related CAP Gates | `CAP-GATE-008`, `CAP-GATE-011` |
| Candidate technologies | `CAP-OPT-001`, `002`, `003`, `004` |
| Eligible Host Frameworks | WinUI 3、WPF |
| Purpose | 定義 denied、black、unavailable、desktop/session boundary 的 failure semantics。 |
| Dependencies | public/synthetic boundary fixture、security review、authorization |
| Preconditions | 不接觸私人 DRM；不嘗試 bypass；只做 lawful observation |
| Synthetic environment | protected-content substitute 與公開 boundary fixture |
| Display/DPI variations | session/desktop boundary as lawfully observable；不改安全設定 |
| Execution sequence | 記錄 boundary；嘗試受核准 observation；記錄 status/error class；停止於 security boundary |
| Required evidence | denied/black/unavailable/error classification、privacy review、stop confirmation |
| Functional pass condition | security boundary 不被繞過，failure semantics 明確 |
| Coordinate verification | 只記錄公開 metadata；不以空白 frame 當成 valid coordinates |
| Crop fidelity verification | Not executed for protected/secure content |
| Timing measurements | optional failure response time；threshold `TBD` |
| Resource observations | cleanup、session state；不讀取 protected surface |
| Privacy checks | no credentials、no protected content、no secure desktop capture |
| Failure condition | requires bypass、administrator privilege、private content 或 unexpected exposure |
| Failure implication | 立即停止；candidate must expose refusal/limitation |
| Recovery expectation | 不以 retry 規避 security refusal；分類為 denied/unavailable |
| Known limitations | 不研究繞過技術、不宣稱 secure desktop capture |
| Stop conditions | 任何 bypass、private content、elevated privilege 或 unclear safety |
| Cleanup | 立即終止；清除未核准 temporary state |
| Future result destination | `docs/Research/Technology/results/capture/CAP-SPIKE-009-result.md`，本輪不建立 |
| Open questions | failure taxonomy、user-facing boundary、session state contract |

### 11.10 `CAP-SPIKE-010` — Failure/device-loss recovery

| Field | Plan |
|---|---|
| Spike ID | `CAP-SPIKE-010` |
| Title | Failure/device-loss recovery |
| Status | Planned |
| Related CAP criteria | `CAP-018`, `CAP-019`, `CAP-020` |
| Related CAP Gates | `CAP-GATE-009`, `CAP-GATE-011` |
| Candidate technologies | `CAP-OPT-001`, `002`, `003`, `005` |
| Eligible Host Frameworks | WinUI 3、WPF |
| Purpose | 觀察 invalidation、release/recreate、retry、cleanup 與 resource state。 |
| Dependencies | `001` baseline、controlled public display/session/device event |
| Preconditions | 事件可安全觸發且不修改未核准 system settings |
| Synthetic environment | synthetic scene、controlled display/mode/session event plan |
| Display/DPI variations | display change、topology change、session change where lawful |
| Execution sequence | 建立 baseline；觸發核准 event；記錄 invalidation；觀察 cleanup/recreate；分類 result |
| Required evidence | event、error class、release/recreate、retry、cleanup、recovery observation |
| Functional pass condition | failure 可分類、資源可回收、recovery 可重現；不能以 silent stale frame 通過 |
| Coordinate verification | topology/DPI change 後重新記錄 source mapping |
| Crop fidelity verification | recovery 後需重新建立 expected source；不重用舊 mapping |
| Timing measurements | invalidation/recovery duration；threshold `TBD` |
| Resource observations | buffers、device、memory、cleanup、leak indicators |
| Privacy checks | no private content；只保留 metadata/error class |
| Failure condition | event unsafe、resource leak、stale frame、recovery nondeterministic |
| Failure implication | resilient gate remains open；不得宣稱 fallback correctness |
| Recovery expectation | explicit recreate/stop classification；不無限 retry |
| Known limitations | 不建立 production retry service 或 error policy |
| Stop conditions | 需要 driver/system mutation、administrator privilege 或 private desktop |
| Cleanup | release/recreate boundary 完成後清理；不留 result directory |
| Future result destination | `docs/Research/Technology/results/capture/CAP-SPIKE-010-result.md`，本輪不建立 |
| Open questions | retry count、backoff、device ownership、session semantics |

### 11.11 `CAP-SPIKE-011` — WinUI 3/WPF interoperability

| Field | Plan |
|---|---|
| Spike ID | `CAP-SPIKE-011` |
| Title | WinUI 3/WPF interoperability |
| Status | Planned |
| Related CAP criteria | `CAP-020`, `CAP-021`, `CAP-022` |
| Related CAP Gates | `CAP-GATE-010`, `CAP-GATE-011` |
| Candidate technologies | `CAP-OPT-001`, `002`, `003`, `005` |
| Eligible Host Frameworks | WinUI 3、WPF；完全分開執行與比較 |
| Purpose | 比較 callability、frame ownership、coordinate、thread/lifecycle、cleanup。 |
| Dependencies | same synthetic fixture、`004` mapping、host authorization |
| Preconditions | UI decision remains unresolved；不得以 Spike 選 UI framework |
| Synthetic environment | identical public/synthetic scene and topology |
| Display/DPI variations | baseline、mixed-DPI、multi-monitor as available |
| Execution sequence | 分別記錄 host environment；依候選取得 metadata；比較 ownership/lifecycle；清理 |
| Required evidence | host、thread、resource owner、callability、mapping、cleanup |
| Functional pass condition | 兩 host 結果與限制可比較；不要求兩 host 結果相同 |
| Coordinate verification | host logical context 與 source physical mapping 分開 |
| Crop fidelity verification | 只使用同一 fixture 的 plan；實際 result 未執行 |
| Timing measurements | per-host first-frame/resource timing；threshold `TBD` |
| Resource observations | thread/resource owner、cleanup、GPU/CPU/memory |
| Privacy checks | no real windows、titles、credentials or desktop content |
| Failure condition | host-specific interop failure、ownership unclear 或 lifecycle leak |
| Failure implication | 需要 host-specific adapter/constraint；不可泛化為 framework decision |
| Recovery expectation | 依候選 recovery；host lifecycle change 重新評估 |
| Known limitations | 不修改 ADR-0002、不選 UI Framework |
| Stop conditions | 需要正式 product UI、project source code 或未核准 host package |
| Cleanup | 各 host 獨立 cleanup；不混合 artifacts |
| Future result destination | `docs/Research/Technology/results/capture/CAP-SPIKE-011-result.md`，本輪不建立 |
| Open questions | thread affinity、resource conversion、packaging、host boundary |

### 11.12 `CAP-SPIKE-012` — First-frame and resource observation

| Field | Plan |
|---|---|
| Spike ID | `CAP-SPIKE-012` |
| Title | First-frame and resource observation |
| Status | Planned |
| Related CAP criteria | `CAP-017`, `CAP-018`, `CAP-022` |
| Related CAP Gates | `CAP-GATE-001`, `CAP-GATE-009`, `CAP-GATE-011` |
| Candidate technologies | `CAP-OPT-001`, `002`, `003`, `005` |
| Eligible Host Frameworks | WinUI 3、WPF |
| Purpose | 建立 first-frame、allocation、release、CPU/GPU/memory 的可重複觀察方法。 |
| Dependencies | `001` baseline、stable synthetic cycle、thresholds remain `TBD` |
| Preconditions | 不進 production benchmark；只做 authorized observation |
| Synthetic environment | 重複相同 synthetic capture cycle |
| Display/DPI variations | baseline plus selected topology; each variation separated |
| Execution sequence | 記錄 cold/warm state；重複受核准 cycle；記錄 timing/resource；cleanup |
| Required evidence | first-frame、allocation、release、CPU/GPU/memory、cleanup、privacy review |
| Functional pass condition | 可取得可重複觀測；不預設 performance pass |
| Coordinate verification | 確認同一 scene/source bounds；不以 timing 取代 mapping evidence |
| Crop fidelity verification | 只確認 fixture binding；crop result 由 `005` 負責 |
| Timing measurements | first-frame、one-shot、recovery；threshold `TBD` |
| Resource observations | working set、GPU、buffers、release、cleanup |
| Privacy checks | metadata-only reporting；不保存 frame |
| Failure condition | measurement non-repeatable、resource not released、scene drift |
| Failure implication | KPI/threshold work remains open；不可排名 candidates |
| Recovery expectation | resource observation 與 recovery 分別記錄，不混成成功率 |
| Known limitations | 不建立 production performance claim |
| Stop conditions | 需要正式 benchmark、private scene、package/build not authorized |
| Cleanup | 完成 cycle 後釋放所有 temporary resource；不建立 artifact |
| Future result destination | `docs/Research/Technology/results/capture/CAP-SPIKE-012-result.md`，本輪不建立 |
| Open questions | sampling method、threshold owner、memory metric definition |

## 12. Gate Coverage Matrix

| Spike | CAP criteria | CAP Gate | Candidates | Hosts | Display scenario | Evidence required |
|---|---|---|---|---|---|---|
| `CAP-SPIKE-001` | `CAP-001`, `002`, `004`, `017`, `022` | `CAP-GATE-001`, `011` | 001–005 | WinUI 3/WPF | single monitor | frame metadata、functional、environment、privacy、cleanup |
| `CAP-SPIKE-002` | `CAP-003`, `005`, `008` | `CAP-GATE-002`, `004` | 001, 002, 003, 005 | WinUI 3/WPF | same-DPI multi-monitor | bounds、origin、mapping、coverage |
| `CAP-SPIKE-003` | `CAP-006`, `007`, `008` | `CAP-GATE-002`, `003`, `004` | 001, 002, 003, 005 | WinUI 3/WPF | negative-coordinate monitor | signed coordinates、mapping、border |
| `CAP-SPIKE-004` | `CAP-007`, `008`, `021` | `CAP-GATE-003`, `004`, `010` | 001–005 | WinUI 3/WPF | mixed-DPI monitors | DIP/physical、DPI、rounding、host evidence |
| `CAP-SPIKE-005` | `CAP-008`, `013`, `014` | `CAP-GATE-004`, `007` | 001–005 | WinUI 3/WPF | known crop fixture | expected/observed rect、pixel diff、format |
| `CAP-SPIKE-006` | `CAP-009`, `010`, `012` | `CAP-GATE-005`, `011` | 001–005 | WinUI 3/WPF | overlay-like synthetic window | inclusion、exclusion、timing、focus |
| `CAP-SPIKE-007` | `CAP-011`, `022` | `CAP-GATE-006`, `011` | 001–005 | WinUI 3/WPF | synthetic cursor target | cursor state、metadata、timing |
| `CAP-SPIKE-008` | `CAP-013`, `014`, `018` | `CAP-GATE-007`, `011` | 001, 002, 003, 005 | WinUI 3/WPF | HDR/SDR monitors | format、profile、conversion、resource |
| `CAP-SPIKE-009` | `CAP-015`, `016`, `022` | `CAP-GATE-008`, `011` | 001–004 | WinUI 3/WPF | lawful security boundary | denied/black/unavailable、stop/privacy |
| `CAP-SPIKE-010` | `CAP-018`, `019`, `020` | `CAP-GATE-009`, `011` | 001, 002, 003, 005 | WinUI 3/WPF | controlled invalidation | failure、recreate、recovery、cleanup |
| `CAP-SPIKE-011` | `CAP-020`, `021`, `022` | `CAP-GATE-010`, `011` | 001, 002, 003, 005 | WinUI 3/WPF | same fixture, two hosts | callability、ownership、mapping、cleanup |
| `CAP-SPIKE-012` | `CAP-017`, `018`, `022` | `CAP-GATE-001`, `009`, `011` | 001, 002, 003, 005 | WinUI 3/WPF | repeated synthetic cycle | timing、resource、cleanup、repeatability |

Coverage rule：`CAP-001..022` 必須至少由 official evidence 或 future Runtime Spike 覆蓋；`CAP-GATE-001..011` 必須有執行路徑。若發現未覆蓋項目，建立 `CAP-PLAN-GAP-xxx`，不得因同一 Spike 涵蓋多項 Gate 而省略 Evidence requirement。

## 13. Execution Phases

### Phase C1 — Basic Capture and Coordinate Correctness

| Field | Plan |
|---|---|
| Included | `001`, `002`, `003`, `005` |
| Entry criteria | synthetic scene、environment record、candidate identity、authorization boundary |
| Exit criteria | basic source/caption mapping evidence ready for review；不代表 candidate selected |
| Blocking conditions | no authorization、private content、coordinate contract undefined |
| Required candidates | 001–005 where eligibility permits |
| Required hosts | WinUI 3、WPF separately |
| Required evidence | environment、source metadata、bounds、coordinate、crop plan |
| Privacy boundary | synthetic/public only |
| Cleanup requirement | no unapproved frame/crop/result directory |

### Phase C2 — Display, Overlay and Color Behavior

| Field | Plan |
|---|---|
| Included | `004`, `006`, `007`, `008`, `009` |
| Entry criteria | C1 coordinate baseline or explicit documented dependency exception |
| Exit criteria | DPI、overlay、cursor、HDR/SDR、security risk classified |
| Blocking conditions | bypass request、private content、uncontrolled display mutation |
| Required candidates | each candidate whose identity and host boundary are authorized |
| Required hosts | WinUI 3、WPF separately |
| Required evidence | mapping、overlay timing、cursor、format、security failure classification |
| Privacy boundary | no real desktop、no DRM bypass、no secure desktop automation |
| Cleanup requirement | synthetic fixture and temporary state removed within authorization |

### Phase C3 — Recovery, Interop and Resource Observation

| Field | Plan |
|---|---|
| Included | `010`, `011`, `012` |
| Entry criteria | preceding evidence scope stable；recovery events lawful and controllable |
| Exit criteria | failure/recovery、host interop、resource observation ready for decision review |
| Blocking conditions | device instability、resource leak、unclear ownership、unapproved build/package |
| Required candidates | 001, 002, 003, 005; 004 only after identity completion |
| Required hosts | WinUI 3、WPF separately |
| Required evidence | failure class、recreate/cleanup、host comparison、timing/resource |
| Privacy boundary | metadata-first；no desktop image persistence |
| Cleanup requirement | explicit resource release and cleanup confirmation |

## 14. Stop Rules

未來執行時，下列任一情況必須立即停止：

- 需要擷取私人桌面內容。
- 需要規避 Protected Content 或 Secure Desktop。
- 需要管理員權限但未獲授權。
- 需要修改 Display、DPI、HDR、Registry 或系統設定。
- 需要改變正式 SnipPlus workflow。
- 需要建立產品 Source Code。
- Candidate comparison scene 不再等價。
- Coordinate contract 在候選間不一致。
- Evidence 包含 Credential 或私人資訊。
- Capture 造成持續性顯示或 device instability。
- Result 無法重現。
- 實際操作超出核准 Spike。
- 需要未核准的 Package、Build 或 Runtime operation。
- 需要未核准網路、下載、Restore 或 installer。
- output redirection、persistent log 或 result directory 未明確授權。

停止後只能回報 stop reason，不得把未完成狀態標為 `Pass` 或 `Completed`。

## 15. Result Artifact Plan

本輪只規劃，不建立：

`docs/Research/Technology/results/capture/`

未來可能使用：

- `CAP-SPIKE-001-result.md` 至 `CAP-SPIKE-012-result.md`。
- environment records。
- synthetic source references。
- captured frames。
- crop outputs。
- coordinate mapping records。
- pixel-difference data。
- timing records。
- diagnostic logs。
- failure/recovery records。
- privacy review。
- cleanup confirmation。

本輪不得建立目錄、Markdown evidence、JSON/TXT inventory、Screenshot、Result 或任何 Result Artifact。未來是否可持久化必須另有明確授權；本文件不給予該授權。

## 16. Decision Evidence Roll-up

| CAP Gate | Candidate | Host | Display scenario | Result | Evidence completeness | Capture decision impact |
|---|---|---|---|---|---|---|
| `CAP-GATE-001` | 001–005 | WinUI 3/WPF | single monitor | Not executed | Not evaluated | Insufficient evidence |
| `CAP-GATE-002` | 001, 002, 003, 005 | WinUI 3/WPF | virtual desktop | Not executed | Not evaluated | Insufficient evidence |
| `CAP-GATE-003` | 001–005 | WinUI 3/WPF | negative/mixed-DPI | Not executed | Not evaluated | Insufficient evidence |
| `CAP-GATE-004` | 001–005 | WinUI 3/WPF | synthetic crop fixture | Not executed | Not evaluated | Insufficient evidence |
| `CAP-GATE-005` | 001–005 | WinUI 3/WPF | overlay-like window | Not executed | Not evaluated | Insufficient evidence |
| `CAP-GATE-006` | 001–005 | WinUI 3/WPF | synthetic cursor | Not executed | Not evaluated | Insufficient evidence |
| `CAP-GATE-007` | 001, 002, 003, 005 | WinUI 3/WPF | HDR/SDR fixture | Not executed | Not evaluated | Insufficient evidence |
| `CAP-GATE-008` | 001–004 | WinUI 3/WPF | lawful security boundary | Not executed | Not evaluated | Insufficient evidence |
| `CAP-GATE-009` | 001, 002, 003, 005 | WinUI 3/WPF | controlled invalidation | Not executed | Not evaluated | Insufficient evidence |
| `CAP-GATE-010` | 001, 002, 003, 005 | WinUI 3/WPF | same fixture, two hosts | Not executed | Not evaluated | Insufficient evidence |
| `CAP-GATE-011` | 001–005 | WinUI 3/WPF | metadata/privacy review | Not executed | Not evaluated | Insufficient evidence |

`Capture decision impact` 只能使用：`Supports candidate`、`Challenges candidate`、`Neutral`、`Insufficient evidence`。本表所有 Result 都是計畫初始值 `Not executed`，不是實際測試結果，也沒有 candidate ranking。

## 17. Readiness to Execute

Readiness 只能使用：`Ready for capture runtime spike execution`、`Conditionally ready`、`Not ready`。

目前不具備執行條件：

- UI Framework 尚未決定。
- Candidate 精確實驗版本尚未固定。
- Project、Restore、Build 尚未授權。
- Capture Runtime execution 尚未授權。
- Result/Evidence write boundary 尚未授權。
- Synthetic environment、host、display topology 與 privacy authority 尚未形成實際執行授權。

因此目前 Readiness 由前置條件機械式推導為：

`Not ready`

固定狀態：

- `Build Verification: Not performed`
- `Runtime Verification: Not performed`
- `Capture Runtime Spike Authorized: No`
- `Capture Decision: Not made`
- `Rendering Decision: Not made`

## 18. Traceability

### 18.1 Evidence chain

`Product requirement` → `CAP criterion` → `CAP Gate` → `CAP Spike` → `Runtime evidence` → `Future Capture Backend decision`

### 18.2 Upstream references

| Upstream artifact | Relationship |
|---|---|
| `docs/Research/Technology/20-capture-backend-feasibility.md` | Parent feasibility、candidate、criteria、gates、privacy 與 ownership baseline。 |
| `docs/Research/Technology/01-ui-framework-feasibility.md` | UI host、overlay、multi-monitor、DPI 與 input boundary。 |
| `docs/Research/Technology/07-ui-framework-runtime-validation-plan.md` | UI host runtime evidence 的邊界參考；不因本文件取得 UI 決策。 |
| `docs/Research/Technology/10-rendering-technology-feasibility.md` | Rendering 與 Capture boundary；本文件不修改 rendering line。 |
| `Architecture/adr/ADR-0002-ui-framework-selection.md` | UI Framework decision 仍 Draft；不由本計畫決定。 |
| `Architecture/TECHNOLOGY-DECISION-ROADMAP.md` | `TD-003` Capture Backend roadmap 與 dependency boundary。 |
| Frozen PRD、Capture/Workflow/Platform Specs | Product requirement、selection intent、privacy 與 failure traceability。 |

實際文件 ID 與名稱必須從 Repository 原樣引用，不得猜測或將 future result 當成已存在文件。

## 完成條件

- 只建立 `21-capture-backend-runtime-spike-plan.md`。
- 不修改 README、索引、CHANGELOG、TODO 或其他文件。
- 保留 `CAP-SPIKE-001..012` 原始 ID、名稱與範圍。
- 十二個 Spike Status 全部為 `Planned`。
- 建立 Candidate Eligibility Matrix。
- 建立 Controlled Comparison Rules。
- 建立 Synthetic Capture Scene 與 Coordinate Verification Contract。
- 建立 Privacy-preserving Evidence Rules。
- 每個 Spike 具備完整固定欄位。
- 建立 Criteria/Gate Coverage Matrix。
- 建立 Phase C1–C3、Stop Rules 與 Result Artifact Plan。
- 不建立 Result directory。
- 不建立 Project、Prototype、Source Code 或 Capture Artifact。
- 不執行 Capture API、Screenshot、Recording、Restore、Build、Run 或 Runtime Spike。
- 不修改 UI/Rendering Research Line。
- 不建立 Capture ADR。
- `git diff --check` 通過。

完成這份計畫後停止；後續若要執行，必須先有明確的 execution authorization。

