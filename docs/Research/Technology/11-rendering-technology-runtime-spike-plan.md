# Rendering Technology Runtime Spike Plan

本文件是 `TD-002 Rendering Technology` 的 Runtime Evidence Plan。它只定義未來如何以一致、可重複、可比較的方式執行 `RESEARCH-TECH-RENDER-001` 所列的十個 Rendering Spike；本輪不執行 Runtime、Prototype、Project、Build、Restore、Screenshot、Capture 或產品 Coding。

## Document Control

| Field | Value |
| --- | --- |
| Document ID | `RESEARCH-TECH-RENDER-002` |
| Title | Rendering Technology Runtime Spike Plan |
| Status | `Draft` |
| Research Type | Runtime Evidence Plan |
| Execution Status | `Not started` |
| Runtime Verification | `Not performed` |
| Parent Research | `RESEARCH-TECH-RENDER-001` |
| Technology Decision | `TD-002 Rendering Technology` |
| Host Framework Decision | `Unresolved — ADR-0002 remains Draft` |
| Rendering Decision | `Not made` |
| Owner | TBD |
| Last reviewed | Not reviewed |
| Version | 0.1 |
| Preparation date | 2026-07-26 |
| Normative References | `docs/Research/Technology/10-rendering-technology-feasibility.md`, `Architecture/adr/ADR-0002-ui-framework-selection.md`, `Architecture/TECHNOLOGY-DECISION-ROADMAP.md` |
| Informative References | `docs/Research/Technology/01-ui-framework-feasibility.md`, `docs/Research/Technology/07-ui-framework-phase1-readiness-reassessment.md`, `PRD-0002`, `PRD-0003`, `PRD-0004`, `PRD-0006`, `SPEC-0005`, `SPEC-0009`, `SPEC-0010`, `ARCH-0002`, `ARCH-0003`, `ARCH-0004`, `ARCH-0005` |
| Supersedes | None |
| Superseded by | None |

## 1. Purpose

本文件只回答：如何在未決的 Host Framework 與 Rendering Technology 邊界下，執行 `RND-SPIKE-001` 至 `RND-SPIKE-010`，並產生足以重新評估 `TD-002` 的 Runtime Evidence。

本計畫的目的：

- 固定候選技術與 Host Framework 的比較方式。
- 固定 synthetic workload、座標情境與 interaction sequence。
- 固定環境紀錄、證據類型、量測分類與保存規則。
- 防止 future Prototype 階段任意改變通過條件。
- 把功能、fidelity、performance、interop 與 cleanup 證據分開記錄。
- 為未來 Rendering ADR 提供可追溯的 Runtime Evidence，而不預先產生決策。

## 2. Scope

### 2.1 Candidate technologies

本計畫保留上游文件的五個候選，名稱與 ID 不得重新命名：

| Candidate ID | Candidate technology |
| --- | --- |
| `RND-OPT-001` | Framework-native retained-mode rendering |
| `RND-OPT-002` | Direct2D／DirectWrite |
| `RND-OPT-003` | Win2D |
| `RND-OPT-004` | SkiaSharp |
| `RND-OPT-005` | Hybrid interaction and rendering surface |

### 2.2 Host frameworks

- WinUI 3。
- WPF。

`ADR-0002` 尚未完成前，不得只測試單一 Host。某候選與某 Host 若標示為 `Not aligned`，可以不執行該組合，但必須保留官方或 repository evidence 與排除原因。

### 2.3 Evidence boundary

本計畫只涵蓋：

- Selection rectangle continuous redraw。
- 多個向量物件與 resize／rotation handles。
- Text、font fallback 與 scaling。
- Mosaic／pixelation interaction。
- Hit testing accuracy。
- 不同 DPI 下的同一 render intent。
- Display render 與 PNG export comparison。
- Alpha／transparent overlay composition。
- CPU／GPU／memory observation。
- WinUI 3／WPF host interoperability。

候選可以被拆成多個 sub-path 觀察，但不得把某個 single sub-path 的結果宣告為完整 Rendering Technology 結論。

## 3. Non-goals and Authorization Boundary

本文件不得授權或執行下列工作：

- 執行任何 Runtime Spike、Prototype 或 product experiment。
- 建立 Project、Solution、Result directory 或正式產品 Source Code。
- 執行 Restore、Build、Run、Publish、Performance Test 或 Deployment。
- 選擇 Rendering Technology、Desktop UI Framework、Capture Backend 或 Clipboard API。
- 建立或修改 Rendering ADR。
- 修改 `ADR-0002` 的 Decision、Status 或 Proposed Decision。
- 修改 frozen PRD、Specs、Architecture baseline 或 Technology Decision Roadmap。
- 使用真實桌面像素、Print Screen hook、Capture API、Clipboard 或使用者私人圖片。
- 建立正式 Annotation Tool、Workflow State Authority、Capture coordinator 或 Clipboard coordinator。
- 將 future Spike code 直接移入產品 Source tree。

「Execution sequence」只描述未來可能的受控操作，不代表本輪已取得執行授權。

## 4. Spike Binding Policy

### 4.1 Upstream binding

下表是對 `RESEARCH-TECH-RENDER-001` 的一對一 binding。不得重新編號、刪除或合併。

| Spike ID | Upstream title | Primary RND criteria | Binding status |
| --- | --- | --- | --- |
| `RND-SPIKE-001` | Selection rectangle continuous redraw | `RND-001`, `RND-002`, `RND-003`, `RND-017` | Planned |
| `RND-SPIKE-002` | Multiple vector objects and resize handles | `RND-006`, `RND-007`, `RND-008`, `RND-010`, `RND-012` | Planned |
| `RND-SPIKE-003` | Text, font fallback and scaling | `RND-003`, `RND-004`, `RND-009`, `RND-013`, `RND-014` | Planned |
| `RND-SPIKE-004` | Mosaic interaction | `RND-001`, `RND-011`, `RND-012`, `RND-014`, `RND-016` | Planned |
| `RND-SPIKE-005` | Hit testing accuracy | `RND-003`, `RND-007`, `RND-008`, `RND-017` | Planned |
| `RND-SPIKE-006` | Same screen output at different DPI | `RND-003`, `RND-004`, `RND-005`, `RND-015`, `RND-017` | Planned |
| `RND-SPIKE-007` | Display render versus PNG export comparison | `RND-013`, `RND-014`, `RND-017`, `RND-018` | Planned |
| `RND-SPIKE-008` | Alpha and transparent overlay | `RND-001`, `RND-012`, `RND-014`, `RND-015` | Planned |
| `RND-SPIKE-009` | CPU/GPU and memory observation | `RND-016`, `RND-017`, `RND-018` | Planned |
| `RND-SPIKE-010` | WinUI 3/WPF host interoperability | `RND-003`, `RND-004`, `RND-005`, `RND-018` | Planned |

### 4.2 Spike status vocabulary

Spike Status 只能使用：

- `Planned`
- `Ready`
- `Running`
- `Blocked`
- `Completed`
- `Invalidated`

本文件中的十個 Spike 全部維持 `Planned`。`Running`、`Completed` 與 `Invalidated` 只能在未來取得明確授權並保存相應 evidence 後使用。

### 4.3 Plan gap policy

- 發現上游研究缺口時，新增 `RND-PLAN-GAP-###` 記錄，不改寫上游文件的歷史內容。
- `RND-PLAN-GAP` 必須包含 Gap、Affected Spike、Missing evidence、Impact、Owner、Resolution condition 與 Status。
- 未有來源的 `Not aligned` 不得關閉 Candidate–Host 組合。
- `Unknown` 不得直接轉成 `Excluded with evidence`。
- 未完成或失敗的 Spike 不得改寫成成功結果。

## 5. Candidate–Host Eligibility Matrix

### 5.1 Eligibility vocabulary

Eligibility 只能使用：`Eligible`、`Conditionally eligible`、`Not aligned`、`Unknown`。

Runtime inclusion 只能使用：`Planned`、`Blocked`、`Excluded with evidence`、`Not evaluated`。

`Eligible` 只表示該組合可以進入 future controlled comparison，不表示產品適用、決策接受或 runtime 已驗證。

### 5.2 Matrix

| Candidate | WinUI 3 eligibility | WPF eligibility | Required interop | Evidence basis | Runtime inclusion |
| --- | --- | --- | --- | --- | --- |
| `RND-OPT-001` Framework-native retained-mode rendering | Conditionally eligible；host primitives 可做 basic drawing，interactive overlay 仍需 spike | Conditionally eligible；WPF Visual／DrawingVisual 與 Visual-to-PNG 有官方 evidence，完整 workflow 仍需 spike | Host-native rendering boundary；兩個 host 需分別記錄 | `RESEARCH-TECH-RENDER-001` Section 4、`RND-EVID-001` 至 `RND-EVID-003` | Planned |
| `RND-OPT-002` Direct2D／DirectWrite | Conditionally eligible；需要 WinUI surface／input interop | Conditionally eligible；需要 WPF／HWND surface interop | Native surface、resource lifecycle 與 input bridge | `RESEARCH-TECH-RENDER-001` Section 5、`RND-EVID-006`、`RND-EVID-007` | Planned |
| `RND-OPT-003` Win2D | Eligible for WinUI-oriented comparison；package、DPI、device lifecycle 與 product overlay 仍需 spike | Conditionally eligible；需要非 native WPF bridge，不能由 package reference 推論產品適用 | Win2D surface、WPF bridge、input mapping | `RND-EVID-004`、`RND-EVID-005`、`RND-EVID-007` | Planned |
| `RND-OPT-004` SkiaSharp | Unknown；host adapter 與 dependency evidence 尚未建立 | Unknown；host adapter 與 dependency evidence 尚未建立 | WinUI／WPF surface adapters、dependency provenance | `RESEARCH-TECH-RENDER-001` 對 SkiaSharp 的 `Unknown` findings | Not evaluated |
| `RND-OPT-005` Hybrid interaction and rendering surface | Conditionally eligible；需 abstract render intent 與 WinUI surface boundary | Conditionally eligible；需 abstract render intent 與 WPF surface boundary | Domain render intent、兩個 host adapters、surface lifecycle | `RND-EVID-006`、`RND-EVID-007`、`RND-EVID-008` | Planned |

### 5.3 Eligibility rules

- Package 可引用不是 `Eligible` 的唯一依據。
- Basic drawing 可行不等於 interactive annotation、overlay composition 或 PNG export 可行。
- Eligibility 需要 candidate capability、host surface、input mapping、DPI mapping、resource lifecycle 與 evidence provenance 都被明確記錄。
- 任何需要 product decision 的 capability 只能保留 `Conditionally eligible` 或 `Unknown`。
- `Not evaluated` 只表示本計畫尚未納入 runtime inclusion，不等於 `Not aligned`。

## 6. Controlled Comparison Rules

### 6.1 Environment invariants

未來執行同一批比較時，必須使用：

- 相同 Windows Build、CPU architecture、GPU、driver 與硬體條件；若無法相同，必須在 Environment Record 中揭露差異。
- 相同 synthetic workload、object count、文字內容、pointer sequence、logical canvas 與 output pixel size。
- 相同 DPI 情境與相同 coordinate conversion 定義。
- 相同 candidate scope；不得只因某 candidate 的工具較容易取得而提前排除。
- 相同 build configuration；Debug 與 Release 不得混合比較。
- 相同 cold-start／warm-repeat 分類；首次啟動與重複執行分開記錄。
- 相同 evidence capture 時機與 cleanup sequence。

### 6.2 Implementation neutrality

- Framework-native optimization 必須完整揭露，不得把候選特有的快取、預載或資源策略隱藏在比較結果中。
- 不得只為單一候選加入未定義的特殊快取、預測性更新或手工調整。
- Display render 與 PNG export 必須分開量測，即使它們共用 render intent。
- 功能通過不代表 fidelity 或 performance 通過。
- 主觀觀察、實際量測值、參考圖像、PNG artifact 與 pixel-difference result 必須分欄保存。
- 測試工具或 future Prototype 不能直接成為產品 Source tree 的依賴。

### 6.3 Reproducibility

每次 future attempt 必須記錄 candidate、host、版本、環境、configuration、timestamp、attempt number、evidence artifact、cleanup confirmation 與 outcome classification。缺少必要欄位時，結果只能標示 `Blocked` 或 `Not executed`。

## 7. Synthetic Rendering Workload Contract

### 7.1 Fixed baseline

- 固定 logical canvas size：1024 × 768。
- 固定 output pixel size；實際 pixel dimensions 在 Environment Record 中填寫，不得由結果倒推規格。
- 固定高對比背景色塊與透明背景案例。
- 固定 selection rectangle initial position `(128, 96)`、size `160 × 120`。
- 固定 rectangle、line/path、arrow-like path、circle、rotated object、text run、mosaic region 與 clipping region。
- 固定不同 stroke widths、opacity、layer order、rotation 與 resize handles。
- 固定中英文、mixed-script 與 font-fallback 字元。
- 固定半透明 selection、alpha gradient 與 overlapping objects。
- 固定最小與最大 logical scale case。
- 固定 Pointer／hit-test／focus restore／cancel sequence。

### 7.2 Workload invariants

同一 Spike 的 candidate/host comparison 必須保持：

- 相同 object count 與 object geometry。
- 相同 text content、font request 與 fallback characters。
- 相同 pointer points、event ordering、cancel point 與 focus state label。
- 相同 logical coordinate、DPI branch、output pixel target 與 clipping boundary。
- 相同 alpha、color block 與 mosaic region input。
- 相同 iteration count；若 iteration count 改變，必須建立新的 attempt record。

### 7.3 Explicit exclusions

Workload 不得包含：

- 真實桌面像素或使用者私人圖片。
- Capture API、Print Screen hook 或 platform capture coordinator。
- Clipboard read/write。
- 正式產品 UI、正式 Annotation Tool 或 product workflow state。
- 未經記錄的外部服務、package download、installer 或 deployment action。

## 8. Test Environment Record

### 8.1 Required fields

| Field | Required value or rule |
| --- | --- |
| Windows edition／build | 執行時記錄；不得預填 |
| Host Framework／version | WinUI 3 或 WPF 及實際版本 |
| Rendering candidate／version | Candidate ID、package/API/adapter version；無版本時記錄 Unknown |
| Runtime／SDK | 執行時記錄；不得把 package presence 當作 SDK proof |
| CPU architecture | `x64`、`ARM64` 或實際值 |
| CPU／GPU／driver | 執行時記錄 |
| Monitor／resolution | 顯示器、排列、resolution 與 virtual desktop bounds |
| DPI scaling | 每個 monitor 的 scaling 與 active host scale |
| HDR state | On、Off 或 Unknown；不推論 color policy |
| Build configuration | `Debug`、`Release` 或其他明確值 |
| Hardware acceleration | On、Off 或 Unknown |
| Export pixel format | 執行時記錄；不預設 alpha 或 color format |
| Color-space context | 執行時記錄；不預設 sRGB 或 HDR downgrade |
| Test timestamp | ISO 8601 timestamp |
| Attempt number | 單一 candidate／host／configuration 的遞增編號 |
| Authorization record | Build authorization 與 Runtime authorization 的分別引用 |
| Cleanup confirmation | Candidate/host switch 前必填 |

### 8.2 Environment integrity rules

- 不得預填不存在的結果、版本、成功條件或 hardware state。
- 不同硬體或 Windows Build 的結果不得放在同一 comparative set 中而不標示差異。
- Environment Record 不得包含 secret、token、credential、cookie 或私人資料。
- 缺少 environment provenance 時，不得把觀察升級為 candidate comparison result。

## 9. Evidence Types

### 9.1 Allowed evidence types

未來每個 Spike 必須從下列類型中標示 Required、Optional 或 Prohibited：

- Functional observation。
- Measured value。
- Reference image。
- Rendered output。
- PNG export artifact。
- Pixel-difference result。
- Diagnostic log。
- Environment record。
- Screen recording。
- Failure reproduction。
- Cleanup confirmation。

### 9.2 Evidence package rules

每個 Spike 的 evidence package 至少包含：

1. Environment Record。
2. Synthetic workload identifier 與固定 input summary。
3. Execution sequence version。
4. Required evidence type 或明確的 `Not executed`／`Blocked` reason。
5. Functional classification。
6. Fidelity comparison classification；不適用時記錄原因。
7. Performance observation classification；未量測時不得虛構數值。
8. Failure condition、failure implication 與 known limitation。
9. Cleanup confirmation。

文字說明不得單獨宣稱 PNG fidelity、DPI correctness、hit-test accuracy、resource recovery 或 performance 已通過。

## 10. Measurement Classification

### 10.1 Functional Gate

Functional Gate 只能使用：

- `Pass`
- `Fail`
- `Blocked`
- `Not executed`

Functional Gate 的判定必須引用 concrete evidence。`Not executed` 不是 `Fail`，也不是成功。

### 10.2 Comparative Metric

可以記錄實際值，但不得在沒有 product-approved threshold 時自行建立 KPI。可記錄項目包括：

- Redraw duration。
- Frame consistency。
- Hit-test duration。
- Export duration。
- Memory usage。
- CPU／GPU observation。
- Pixel-difference count。
- Resource creation／release count。

每個值必須附帶 measurement method、unit、sample count、warm/cold classification、environment reference 與 limitations。

### 10.3 Threshold-dependent Metric

產品門檻未核准時，固定記錄：

| Field | Value |
| --- | --- |
| Threshold | `TBD` |
| Decision use | `Informative only` |
| Product KPI authority | Not assigned |

不得自行設定 FPS、毫秒、memory size、pixel-difference tolerance、font tolerance、alpha tolerance 或 color-difference KPI。

## 11. Per-Spike Execution Specifications

### 11.0 Common fixed schema

每個 Spike 必須固定包含以下欄位：

`Spike ID`、`Title`、`Status`、`Related RND criteria`、`Related RND Gate`、`Candidate technologies`、`Eligible Host Frameworks`、`Dependencies`、`Purpose`、`Preconditions`、`Synthetic workload`、`Environment variations`、`Execution sequence`、`Evidence required`、`Measurements`、`Functional pass condition`、`Fidelity comparison`、`Performance observation`、`Failure condition`、`Failure implication`、`Known limitations`、`Safety and cleanup`、`Result artifact destination`、`Open questions`。

Execution sequence 只能描述 future controlled operation，不得包含 Source Code、產品 implementation 或本輪執行結果。

### 11.1 `RND-SPIKE-001` — Selection rectangle continuous redraw

| Field | Plan |
| --- | --- |
| Spike ID | `RND-SPIKE-001` |
| Title | Selection rectangle continuous redraw |
| Status | Planned |
| Related RND criteria | `RND-001`, `RND-002`, `RND-003`, `RND-017` |
| Related RND Gate | `RND-GATE-001`, `RND-GATE-003`, `RND-GATE-008` |
| Candidate technologies | `RND-OPT-001` 至 `RND-OPT-005`，依 Eligibility Matrix 納入 |
| Eligible Host Frameworks | WinUI 3、WPF |
| Dependencies | `ADR-0002` 保持 Draft；host surface authorization；fixed synthetic baseline |
| Purpose | 觀察連續 pointer updates 下 selection rectangle 的 bounds、alpha、重繪完整性與 coordinate mapping。 |
| Preconditions | Candidate/Host eligibility 已記錄；環境欄位完整；沒有真實桌面像素；Runtime authorization 已單獨核准。 |
| Synthetic workload | 1024 × 768 logical canvas；initial `(128,96)` / `160 × 120`；最小、反向、跨 boundary 與連續 pointer sequence。 |
| Environment variations | WinUI 3／WPF；same-DPI 與已授權的不同 DPI；cold start／warm repeat。 |
| Execution sequence | 建立固定 scene → 初始化 selection bounds → 依固定 pointer sequence 更新 → 記錄每個 bounds 與 output → 執行 cancel/focus restore → 保存 cleanup。 |
| Evidence required | Environment record、rendered output、functional observation、diagnostic log、cleanup confirmation；reference image optional。 |
| Measurements | Bounds mapping、redraw duration、frame consistency、invalidated region observation；threshold `TBD`。 |
| Functional pass condition | 每次更新都能產生可追蹤 bounds 與完整 redraw record，且不存在未解釋的殘影或 coordinate jump。 |
| Fidelity comparison | 比對 host logical bounds、renderer units 與 output pixel bounds；差異必須有 mapping explanation。 |
| Performance observation | 記錄 redraw observation；不建立 latency KPI。 |
| Failure condition | bounds 不可重現、host/renderer mapping 遺失、cancel 後仍持續更新或 cleanup 不完整。 |
| Failure implication | 保留 Candidate 的 `Requires runtime prototype` 或 `Blocked`，並建立 `RND-PLAN-GAP`；不得直接作技術決策。 |
| Known limitations | 不涵蓋真實 desktop overlay、Capture entry、focus policy 或 final product UI。 |
| Safety and cleanup | 只使用 synthetic surface；停止 pointer sequence；釋放 host resources；保存 cleanup confirmation。 |
| Result artifact destination | Future only: `docs/Research/Technology/results/rendering/RND-SPIKE-001-result.md`；本輪不建立。 |
| Open questions | Overlay 是否單一 surface；不同 monitor 的 selection ownership；product-approved redraw threshold。 |

### 11.2 `RND-SPIKE-002` — Multiple vector objects and resize handles

| Field | Plan |
| --- | --- |
| Spike ID | `RND-SPIKE-002` |
| Title | Multiple vector objects and resize handles |
| Status | Planned |
| Related RND criteria | `RND-006`, `RND-007`, `RND-008`, `RND-010`, `RND-012` |
| Related RND Gate | `RND-GATE-002`, `RND-GATE-008` |
| Candidate technologies | `RND-OPT-001` 至 `RND-OPT-005`，依 Eligibility Matrix 納入 |
| Eligible Host Frameworks | WinUI 3、WPF |
| Dependencies | `SPEC-0009`、`SPEC-0010`；fixed object set；Domain/Rendering ownership boundary。 |
| Purpose | 觀察多個向量物件、layer order、rotation bounds 與 resize handles 的 render output 與 interaction boundary。 |
| Preconditions | 不建立正式 Annotation model；object semantics 以 synthetic render intent 提供；host input mapping 已記錄。 |
| Synthetic workload | Rectangle、line/path、arrow-like path、circle、rotated object、text run；至少兩個重疊物件與八向 handles。 |
| Environment variations | WinUI 3／WPF；same-DPI／approved DPI branches；不同 stroke widths 與 opacity。 |
| Execution sequence | 建立固定 render intents → render layer order → 依固定 pointer sequence hit/resize/rotate → 記錄 geometry → reset scene → cleanup。 |
| Evidence required | Environment record、rendered output、functional observation、hit-test record、cleanup confirmation。 |
| Measurements | Object bounds、handle bounds、transform values、z-order result、mismatch count。 |
| Functional pass condition | 每個 object 與 handle 都能由固定 input 產生可追蹤 geometry/result；未定義的 product interaction 不得被補造。 |
| Fidelity comparison | 比對 untransformed/rotated geometry、stroke alignment、clip boundary 與 output pixels。 |
| Performance observation | 記錄 object-count 與 redraw observation；不設定 object-count KPI。 |
| Failure condition | handle geometry 與 object bounds 不一致、z-order 不可重現、transform/clipping 結果無法解釋。 |
| Failure implication | 重新檢查 Domain hit-test intent、host adapter 或 renderer boundary；不把 failure 寫成 candidate rejection。 |
| Known limitations | 不決定 Annotation Tool catalog、serialization、Undo／Redo 或 selection-state authority。 |
| Safety and cleanup | 不寫入產品 state；清除 synthetic objects；釋放 surface/visual resources；保存 cleanup。 |
| Result artifact destination | Future only: `docs/Research/Technology/results/rendering/RND-SPIKE-002-result.md`；本輪不建立。 |
| Open questions | Handles 是否由 host visual 提供；rotated object 的 hit-test semantics；layer order authority。 |

### 11.3 `RND-SPIKE-003` — Text, font fallback and scaling

| Field | Plan |
| --- | --- |
| Spike ID | `RND-SPIKE-003` |
| Title | Text, font fallback and scaling |
| Status | Planned |
| Related RND criteria | `RND-003`, `RND-004`, `RND-009`, `RND-013`, `RND-014` |
| Related RND Gate | `RND-GATE-003`, `RND-GATE-004`, `RND-GATE-006` |
| Candidate technologies | `RND-OPT-001` 至 `RND-OPT-005`，依 Eligibility Matrix 納入 |
| Eligible Host Frameworks | WinUI 3、WPF |
| Dependencies | Font policy remains TBD；text render intent；output pixel target；environment font record。 |
| Purpose | 觀察 text layout、font fallback、baseline、Unicode coverage 與 DPI scaling 對 display/output raster 的影響。 |
| Preconditions | 固定 text content 與 requested font metadata；不得因本 Spike 決定 product font policy。 |
| Synthetic workload | Latin、CJK、mixed-script、fallback glyph、不同 text lengths、baseline、rotation 與 scale cases。 |
| Environment variations | WinUI 3／WPF；100%、125%、150%、200% 或實際可授權 DPI cases；cold/warm font resolution。 |
| Execution sequence | 記錄 requested font → render fixed text runs → capture layout/baseline/fallback evidence → export candidate → compare → cleanup。 |
| Evidence required | Environment record、font resolution record、rendered output、PNG export artifact、pixel/fidelity comparison、cleanup。 |
| Measurements | Layout bounds、baseline、resolved font/fallback metadata、output pixel difference；threshold `TBD`。 |
| Functional pass condition | 每個 text run 都能記錄 layout、fallback 與 output path；缺少 font metadata 時不得標示 Pass。 |
| Fidelity comparison | 比對 display/export 的 glyph placement、baseline、scaling、alpha、color 與 clipping。 |
| Performance observation | 記錄 text layout/render observation；不建立 typing、frame 或 latency KPI。 |
| Failure condition | fallback 不可追蹤、baseline 漂移、Unicode output 不可重現、display/export path 差異無法解釋。 |
| Failure implication | 補充 font/render intent 或 export boundary；保留 `Unknown`／`Requires runtime prototype`。 |
| Known limitations | 不選定字型、語系支援矩陣、accessibility policy 或 text annotation feature。 |
| Safety and cleanup | 只使用 synthetic strings；不讀取私人文件；清除 font/resource handles；保存 environment 與 cleanup。 |
| Result artifact destination | Future only: `docs/Research/Technology/results/rendering/RND-SPIKE-003-result.md`；本輪不建立。 |
| Open questions | Font fallback authority；font embedding；CJK baseline tolerance；output color policy。 |

### 11.4 `RND-SPIKE-004` — Mosaic interaction

| Field | Plan |
| --- | --- |
| Spike ID | `RND-SPIKE-004` |
| Title | Mosaic interaction |
| Status | Planned |
| Related RND criteria | `RND-001`, `RND-011`, `RND-012`, `RND-014`, `RND-016` |
| Related RND Gate | `RND-GATE-001`, `RND-GATE-005`, `RND-GATE-006`, `RND-GATE-009` |
| Candidate technologies | `RND-OPT-002`、`RND-OPT-003`、`RND-OPT-004`、`RND-OPT-005`；`RND-OPT-001` 作 host baseline。 |
| Eligible Host Frameworks | WinUI 3、WPF |
| Dependencies | Synthetic bounded region；effect path evidence；alpha/color record；no real image input。 |
| Purpose | 觀察 bounded mosaic／pixelation region 在移動、縮放、旋轉時的 effect boundary、alpha edge 與 repaint。 |
| Preconditions | 固定 color-block workload；effect parameters 以 test metadata 記錄；不得決定 algorithm 或 product tool。 |
| Synthetic workload | Synthetic image-like color blocks、bounded effect region、不同 region size、edge、scale 與 clipping。 |
| Environment variations | WinUI 3／WPF；GPU/acceleration On、Off、Unknown；same-DPI 與 approved DPI branches。 |
| Execution sequence | 建立 synthetic region → apply fixed effect intent → move/resize/rotate → record display/export candidates → compare edge/output → cleanup。 |
| Evidence required | Environment record、rendered output、PNG artifact if authorized、pixel/fidelity result、resource observation、cleanup。 |
| Measurements | Region bounds、edge alpha、pixel-difference count、repaint/resource observation；threshold `TBD`。 |
| Functional pass condition | Region bounds 與 effect result 可重現，且 failure/resource state 可被記錄。 |
| Fidelity comparison | 比對 interaction-time 與 export-time region、edge alpha、interpolation、color 與 clipping。 |
| Performance observation | 記錄 effect/repaint observation；不設定 effect latency KPI。 |
| Failure condition | region drift、edge artifacts 無法解釋、effect resource recovery 遺失或 output path 不一致。 |
| Failure implication | 拆分 effect sub-path、標示 `Not aligned` 或 `Requires runtime prototype`；不得把整個 renderer 改成未授權的 CPU bitmap pipeline。 |
| Known limitations | 不讀取真實圖片；不決定 mosaic algorithm、quality threshold 或 product exposure。 |
| Safety and cleanup | 僅使用 synthetic color blocks；釋放 effect/device resources；保存 cleanup confirmation。 |
| Result artifact destination | Future only: `docs/Research/Technology/results/rendering/RND-SPIKE-004-result.md`；本輪不建立。 |
| Open questions | Interaction-time 是否必須 effect；GPU/software export 差異；alpha edge policy。 |

### 11.5 `RND-SPIKE-005` — Hit testing accuracy

| Field | Plan |
| --- | --- |
| Spike ID | `RND-SPIKE-005` |
| Title | Hit testing accuracy |
| Status | Planned |
| Related RND criteria | `RND-003`, `RND-007`, `RND-008`, `RND-017` |
| Related RND Gate | `RND-GATE-002`, `RND-GATE-003`, `RND-GATE-008` |
| Candidate technologies | `RND-OPT-001` 至 `RND-OPT-005`，依 Eligibility Matrix 納入 |
| Eligible Host Frameworks | WinUI 3、WPF |
| Dependencies | Fixed geometry；host input coordinate record；Domain hit-test intent；z-order rule。 |
| Purpose | 觀察 point、geometry、z-order、rotated object 與 handles 的 hit-test result 是否與 render intent 對齊。 |
| Preconditions | 固定 query sequence；明確記錄 input point、logical point、candidate objects 與 expected evidence，不預設 product selection policy。 |
| Synthetic workload | 重疊 rectangle、line/path、rotated bounds、text run、transparent object、handles 與 edge points。 |
| Environment variations | WinUI 3／WPF；DPI branches；pointer input 與 geometry query 分開記錄。 |
| Execution sequence | 建立 render intents → render → 執行 fixed point/geometry queries → 記錄 traversal/result → repeat → cleanup。 |
| Evidence required | Environment record、hit-test diagnostic log、functional observation、rendered output、cleanup confirmation。 |
| Measurements | Query duration、result order、mismatch count、coordinate conversion record。 |
| Functional pass condition | 每個 query 都產生可重現 decision record，且 mismatch 有明確 cause 或 limitation。 |
| Fidelity comparison | 比對 visual object geometry、transparent/opaque behavior、z-order 與 host coordinate conversion。 |
| Performance observation | 記錄 query observation；不建立 query latency KPI。 |
| Failure condition | query result 不可重現、z-order 未定義、host point 與 logical point 無法追蹤。 |
| Failure implication | 補充 host adapter 或 Domain hit-test contract；candidate 保持 `Unknown`／`Requires runtime prototype`。 |
| Known limitations | 不新增 selection state authority、不修改 workflow state、不建立 input service。 |
| Safety and cleanup | 不寫入正式 selection state；清除 synthetic visual tree；保存 query log 與 cleanup。 |
| Result artifact destination | Future only: `docs/Research/Technology/results/rendering/RND-SPIKE-005-result.md`；本輪不建立。 |
| Open questions | Geometry hit-test tolerance；transparent object policy；rotated handle precedence。 |

### 11.6 `RND-SPIKE-006` — Same screen output at different DPI

| Field | Plan |
| --- | --- |
| Spike ID | `RND-SPIKE-006` |
| Title | Same screen output at different DPI |
| Status | Planned |
| Related RND criteria | `RND-003`, `RND-004`, `RND-005`, `RND-015`, `RND-017` |
| Related RND Gate | `RND-GATE-003`, `RND-GATE-008`, `RND-GATE-009` |
| Candidate technologies | `RND-OPT-001` 至 `RND-OPT-005`，依 Eligibility Matrix 納入 |
| Eligible Host Frameworks | WinUI 3、WPF |
| Dependencies | Per-monitor environment record；fixed logical scene；coordinate mapping contract；DPI authorization。 |
| Purpose | 觀察相同 abstract render intent 在不同 display DPI、host scale 與 output pixel mapping 下的幾何、文字與像素結果。 |
| Preconditions | 記錄 monitor arrangement、DPI、logical/output dimensions；不得修改 display settings 以製造結果。 |
| Synthetic workload | Fixed scene、selection、vector、text、mosaic、alpha、rotation、clipping 與 fixed logical coordinates。 |
| Environment variations | Same-DPI、heterogeneous-DPI、100%／125%／150%／200% 或實際授權的 subsets。 |
| Execution sequence | 建立 same logical intent → render each authorized DPI case → record all coordinate spaces → compare → restore/cleanup。 |
| Evidence required | Environment record、rendered outputs、coordinate mapping log、pixel/fidelity comparison、cleanup confirmation。 |
| Measurements | Host DIPs、renderer units、physical pixels、output pixels、rounding、clipping、mismatch count。 |
| Functional pass condition | 每個 case 都可完整記錄 mapping；不得遺失 monitor、DPI 或 output provenance。 |
| Fidelity comparison | 比對 logical geometry、text baseline、stroke、alpha、mosaic bounds、rotation 與 output dimensions。 |
| Performance observation | 只記錄 DPI transition/resource observation；不設定 transition latency KPI。 |
| Failure condition | mapping 不一致、跨 monitor bounds drift、output dimensions 無法解釋或 environment record 不完整。 |
| Failure implication | 建立 coordinate `RND-PLAN-GAP`、限制未支援 topology 或保留 `Requires runtime prototype`。 |
| Known limitations | 不操作真實 Capture、不變更 display settings、不決定 Capture Backend。 |
| Safety and cleanup | 不修改系統顯示設定；關閉 future host surface；保存 monitor/DPI record。 |
| Result artifact destination | Future only: `docs/Research/Technology/results/rendering/RND-SPIKE-006-result.md`；本輪不建立。 |
| Open questions | Display physical pixel authority；HDR/DPI interaction；rounding rule owner。 |

### 11.7 `RND-SPIKE-007` — Display render versus PNG export comparison

| Field | Plan |
| --- | --- |
| Spike ID | `RND-SPIKE-007` |
| Title | Display render versus PNG export comparison |
| Status | Planned |
| Related RND criteria | `RND-013`, `RND-014`, `RND-017`, `RND-018` |
| Related RND Gate | `RND-GATE-004`, `RND-GATE-006`, `RND-GATE-008`, `RND-GATE-009` |
| Candidate technologies | `RND-OPT-001` 至 `RND-OPT-005`，依 Eligibility Matrix 納入 |
| Eligible Host Frameworks | WinUI 3、WPF |
| Dependencies | Output boundary `SPEC-0008`；fixed render intent；authorized export path；alpha/color record。 |
| Purpose | 比較 display render 與 PNG export 是否共用 render intent，並追蹤 geometry、text、effect、alpha、color 與 clipping 差異。 |
| Preconditions | Export pixel format、output dimensions、color-space context 與 encoder boundary 必須在 Environment Record 中記錄。 |
| Synthetic workload | Fixed vector、text、mosaic、alpha、rotation、clipping、selection 與 overlapping objects。 |
| Environment variations | WinUI 3／WPF；GPU/software path if separately authorized；same-DPI/approved DPI cases。 |
| Execution sequence | 建立 intent → capture display candidate → generate export candidate → record paths/metadata → compare pixels/geometry → cleanup。 |
| Evidence required | Environment record、display rendered output、PNG export artifact、pixel-difference result、diagnostic log、cleanup。 |
| Measurements | Pixel dimensions、alpha difference、color difference、geometry bounds、text baseline、stroke/clipping mismatch count。 |
| Functional pass condition | Display/export path 與每個差異都可追蹤；缺少 output metadata 時不得 Pass。 |
| Fidelity comparison | 必須逐項比對 dimensions、alpha、color、font fallback、stroke alignment、rotation、clipping 與 mosaic output。 |
| Performance observation | 分開記錄 display render 與 export duration；threshold `TBD`。 |
| Failure condition | export artifact 缺失、path 未知、pixel mismatch 無法解釋或 alpha/color metadata 遺失。 |
| Failure implication | 分離 display/export renderer boundary 或補充 Output evidence；不得建立 product encoder decision。 |
| Known limitations | 不決定 image storage format、PNG delivery policy、color profile 或 Output KPI。 |
| Safety and cleanup | Future export 只使用 synthetic scene；不寫入正式 output directory；清理 temporary artifact。 |
| Result artifact destination | Future only: `docs/Research/Technology/results/rendering/RND-SPIKE-007-result.md`；本輪不建立。 |
| Open questions | Same render path 是否必要；alpha/color authority；PNG encoder ownership。 |

### 11.8 `RND-SPIKE-008` — Alpha and transparent overlay

| Field | Plan |
| --- | --- |
| Spike ID | `RND-SPIKE-008` |
| Title | Alpha and transparent overlay |
| Status | Planned |
| Related RND criteria | `RND-001`, `RND-012`, `RND-014`, `RND-015` |
| Related RND Gate | `RND-GATE-001`, `RND-GATE-006`, `RND-GATE-009` |
| Candidate technologies | `RND-OPT-001` 至 `RND-OPT-005`，依 Eligibility Matrix 納入 |
| Eligible Host Frameworks | WinUI 3、WPF |
| Dependencies | Surface pixel format；premultiplied/straight alpha record；synthetic background；color-space record。 |
| Purpose | 觀察 transparent surface、alpha mode、composition order 與 output alpha preservation。 |
| Preconditions | 明確區分 in-app surface 與 desktop backdrop；不讀取真實桌面；alpha/color metadata 已定義為待記錄欄位。 |
| Synthetic workload | Transparent background、半透明 selection、overlapping annotation-like shapes、opaque synthetic color blocks、alpha gradient。 |
| Environment variations | WinUI 3／WPF；GPU/software path if authorized；HDR On/Off/Unknown。 |
| Execution sequence | 建立 transparent surface → render fixed layers → compose synthetic background → record surface/pixel state → export if authorized → cleanup。 |
| Evidence required | Environment record、rendered output、pixel format/alpha record、PNG artifact if authorized、cleanup confirmation。 |
| Measurements | Alpha mode、edge color、composition order、transparent pixel values、color/HDR observation。 |
| Functional pass condition | Alpha behavior 可由 recorded surface/pixel contract 重現，且不把 desktop backdrop 誤列為 output。 |
| Fidelity comparison | 比對 transparent display、synthetic background composition 與 export alpha/color preservation。 |
| Performance observation | 記錄 composition/resource observation；不設定 alpha performance KPI。 |
| Failure condition | alpha mode 遺失、premultiplication 差異不可解釋、edge color contamination 或 HDR state 未記錄。 |
| Failure implication | 補充 surface contract、限制 candidate sub-path 或保留 `Unknown`。 |
| Known limitations | 不決定 Acrylic、desktop backdrop、HDR policy、color management 或 product transparency behavior。 |
| Safety and cleanup | 僅使用 synthetic backdrop；不讀取 desktop pixels；釋放 transparent surface；保存 cleanup。 |
| Result artifact destination | Future only: `docs/Research/Technology/results/rendering/RND-SPIKE-008-result.md`；本輪不建立。 |
| Open questions | Alpha mode authority；color-space conversion；HDR downgrade boundary。 |

### 11.9 `RND-SPIKE-009` — CPU/GPU and memory observation

| Field | Plan |
| --- | --- |
| Spike ID | `RND-SPIKE-009` |
| Title | CPU/GPU and memory observation |
| Status | Planned |
| Related RND criteria | `RND-016`, `RND-017`, `RND-018` |
| Related RND Gate | `RND-GATE-005`, `RND-GATE-007`, `RND-GATE-008`, `RND-GATE-009` |
| Candidate technologies | `RND-OPT-001` 至 `RND-OPT-005`，依 Eligibility Matrix 納入 |
| Eligible Host Frameworks | WinUI 3、WPF |
| Dependencies | Stable synthetic workload；environment record；approved observation tools；resource cleanup boundary。 |
| Purpose | 觀察 redraw、text、mosaic、export 與 resource recovery 的 CPU/GPU/memory evidence，不建立 production benchmark。 |
| Preconditions | Measurement method、sampling interval、cold/warm classification 與 hardware provenance 已記錄；沒有 performance KPI。 |
| Synthetic workload | Fixed scene、fixed iteration count、selection movement、vector objects、text fallback、mosaic effect、export request。 |
| Environment variations | Candidate/Host pair；hardware acceleration On/Off/Unknown；cold start/warm repeat；authorized DPI cases。 |
| Execution sequence | Record environment → run fixed workload → capture observation samples → trigger authorized recovery condition → verify cleanup → classify。 |
| Evidence required | Environment record、measured values、diagnostic log、resource lifecycle observation、failure reproduction if applicable、cleanup。 |
| Measurements | CPU/GPU observation、memory samples、resource create/release、device-loss/recovery record、export/redraw duration。 |
| Functional pass condition | Workload 可重複執行，resource lifecycle 與 cleanup 可追蹤；不要求數值門檻。 |
| Fidelity comparison | 確認 observation workload 在各 candidate/host 使用等價 input/output，而非只比較數字。 |
| Performance observation | 保存實際 observation 與 method；所有 threshold 維持 `TBD`。 |
| Failure condition | resource leak evidence、recovery 未記錄、environment 不同、sampling method 不一致或 workload drift。 |
| Failure implication | 標示 `Blocked`、建立 `RND-PLAN-GAP` 或補充 lifecycle evidence；不宣告 candidate winner/rejection。 |
| Known limitations | 不建立效能 KPI、不代表 production workload、不代表 long-running service behavior。 |
| Safety and cleanup | 限制 iteration；避免寫入正式資料；先完成 resource cleanup 再切換 candidate/host。 |
| Result artifact destination | Future only: `docs/Research/Technology/results/rendering/RND-SPIKE-009-result.md`；本輪不建立。 |
| Open questions | Observation tool authority；GPU counter availability；acceptable resource lifetime。 |

### 11.10 `RND-SPIKE-010` — WinUI 3/WPF host interoperability

| Field | Plan |
| --- | --- |
| Spike ID | `RND-SPIKE-010` |
| Title | WinUI 3/WPF host interoperability |
| Status | Planned |
| Related RND criteria | `RND-003`, `RND-004`, `RND-005`, `RND-018` |
| Related RND Gate | `RND-GATE-003`, `RND-GATE-007`, `RND-GATE-008` |
| Candidate technologies | `RND-OPT-001` 至 `RND-OPT-005`，依 Candidate–Host Eligibility Matrix 分開記錄。 |
| Eligible Host Frameworks | WinUI 3、WPF |
| Dependencies | `ADR-0002` remains Draft；host adapter evidence；package/reference provenance；fixed render intent。 |
| Purpose | 觀察相同 abstract render intent 在 WinUI 3 與 WPF 的 surface、input、DPI、lifecycle、resource 與 output evidence。 |
| Preconditions | 每個 host/candidate pair 的 eligibility、native interop、version、SDK/runtime 與 authorization 已記錄。 |
| Synthetic workload | Fixed scene、pointer sequence、DPI cases、resource recovery condition、display/export request。 |
| Environment variations | WinUI 3／WPF；candidate-specific surface adapter；same hardware/build where possible；cold/warm。 |
| Execution sequence | Record pair provenance → host fixed scene → execute same render intent/input sequence → record surface/input/DPI/lifecycle → compare → cleanup。 |
| Evidence required | Environment record、package/reference record、rendered output、input/DPI mapping、lifecycle log、PNG artifact if authorized、cleanup。 |
| Measurements | Can reference、basic drawing、interactive annotation、overlay、export evidence level；interop/resource observations。 |
| Functional pass condition | 每個 pair 都能明確記錄各 capability 的 evidence level；缺少 host provenance 時不得 Pass。 |
| Fidelity comparison | 比對 render intent、geometry、text、alpha、mosaic、clipping、DPI mapping 與 display/export result。 |
| Performance observation | 分開記錄 host adapter/resource observation；不設定 cross-host performance KPI。 |
| Failure condition | host surface 不可掛接、input mapping 遺失、DPI path 不一致、lifecycle/cleanup 不完整或 pair 無法等價比較。 |
| Failure implication | 保留 host-specific boundary、`Unknown`、`Conditionally eligible` 或 `Requires runtime prototype`；不得修改 `ADR-0002`。 |
| Known limitations | 不接受 Desktop UI Framework，不建立正式 host project，不把 package reference 當作 product compatibility。 |
| Safety and cleanup | 不修改 system settings；不讀取 desktop pixels；清除兩個 host 的 future surface/resources；保存 cleanup。 |
| Result artifact destination | Future only: `docs/Research/Technology/results/rendering/RND-SPIKE-010-result.md`；本輪不建立。 |
| Open questions | Host parity 是否逐 capability 要求；interop ownership；WPF bridge lifecycle；ADR-0002 downstream boundary。 |

## 12. Gate Coverage Matrix

### 12.1 Spike-to-gate coverage

| Spike | RND criteria | RND Gate | Candidates | Host Frameworks | Evidence required |
| --- | --- | --- | --- | --- | --- |
| `RND-SPIKE-001` | `RND-001`, `RND-002`, `RND-003`, `RND-017` | `RND-GATE-001`, `RND-GATE-003`, `RND-GATE-008` | `RND-OPT-001` 至 `RND-OPT-005` | WinUI 3、WPF | Rendered output、bounds record、diagnostic log、environment、cleanup |
| `RND-SPIKE-002` | `RND-006`, `RND-007`, `RND-008`, `RND-010`, `RND-012` | `RND-GATE-002`, `RND-GATE-008` | `RND-OPT-001` 至 `RND-OPT-005` | WinUI 3、WPF | Geometry、hit-test、handle、transform、rendered output、cleanup |
| `RND-SPIKE-003` | `RND-003`, `RND-004`, `RND-009`, `RND-013`, `RND-014` | `RND-GATE-003`, `RND-GATE-004`, `RND-GATE-006` | `RND-OPT-001` 至 `RND-OPT-005` | WinUI 3、WPF | Font/fallback、layout、rendered output、PNG artifact、fidelity result |
| `RND-SPIKE-004` | `RND-001`, `RND-011`, `RND-012`, `RND-014`, `RND-016` | `RND-GATE-001`, `RND-GATE-005`, `RND-GATE-006`, `RND-GATE-009` | `RND-OPT-001` 至 `RND-OPT-005` | WinUI 3、WPF | Effect output、edge alpha、resource observation、cleanup |
| `RND-SPIKE-005` | `RND-003`, `RND-007`, `RND-008`, `RND-017` | `RND-GATE-002`, `RND-GATE-003`, `RND-GATE-008` | `RND-OPT-001` 至 `RND-OPT-005` | WinUI 3、WPF | Query log、coordinate mapping、result order、rendered output |
| `RND-SPIKE-006` | `RND-003`, `RND-004`, `RND-005`, `RND-015`, `RND-017` | `RND-GATE-003`, `RND-GATE-008`, `RND-GATE-009` | `RND-OPT-001` 至 `RND-OPT-005` | WinUI 3、WPF | Monitor/DPI record、coordinate mapping、rendered outputs、fidelity result |
| `RND-SPIKE-007` | `RND-013`, `RND-014`, `RND-017`, `RND-018` | `RND-GATE-004`, `RND-GATE-006`, `RND-GATE-008`, `RND-GATE-009` | `RND-OPT-001` 至 `RND-OPT-005` | WinUI 3、WPF | Display output、PNG artifact、pixel-difference、path metadata |
| `RND-SPIKE-008` | `RND-001`, `RND-012`, `RND-014`, `RND-015` | `RND-GATE-001`, `RND-GATE-006`, `RND-GATE-009` | `RND-OPT-001` 至 `RND-OPT-005` | WinUI 3、WPF | Alpha/pixel format、transparent output、color/HDR record、cleanup |
| `RND-SPIKE-009` | `RND-016`, `RND-017`, `RND-018` | `RND-GATE-005`, `RND-GATE-007`, `RND-GATE-008`, `RND-GATE-009` | `RND-OPT-001` 至 `RND-OPT-005` | WinUI 3、WPF | Environment、measured values、resource lifecycle、diagnostic log |
| `RND-SPIKE-010` | `RND-003`, `RND-004`, `RND-005`, `RND-018` | `RND-GATE-003`, `RND-GATE-007`, `RND-GATE-008` | `RND-OPT-001` 至 `RND-OPT-005` | WinUI 3、WPF | Pair provenance、surface/input/DPI/lifecycle evidence、output comparison |

### 12.2 Criteria coverage checklist

| Criteria range | Coverage source |
| --- | --- |
| `RND-001` through `RND-005` | `RND-SPIKE-001`, `RND-SPIKE-003`, `RND-SPIKE-005`, `RND-SPIKE-006`, `RND-SPIKE-007`, `RND-SPIKE-008`, `RND-SPIKE-010` |
| `RND-006` through `RND-010` | `RND-SPIKE-002`, `RND-SPIKE-003`, `RND-SPIKE-005` |
| `RND-011` through `RND-015` | `RND-SPIKE-003`, `RND-SPIKE-004`, `RND-SPIKE-006`, `RND-SPIKE-007`, `RND-SPIKE-008` |
| `RND-016` through `RND-018` | `RND-SPIKE-001`, `RND-SPIKE-004`, `RND-SPIKE-005`, `RND-SPIKE-006`, `RND-SPIKE-007`, `RND-SPIKE-009`, `RND-SPIKE-010` |

`RND-001` 至 `RND-018` 均有 future Spike 或 upstream official evidence coverage；實際結果仍為 `Not executed`。

### 12.3 Gate status boundary

`RND-GATE-001` 至 `RND-GATE-009` 的執行路徑已定義，但本文件不填入 gate outcome。未來若缺少 required evidence，gate 只能是 `Blocked`、`Not executed` 或上游允許的 evidence status，不得被文字敘述直接關閉。

## 13. Execution Order

### Phase R1 — Core Rendering Correctness

- `RND-SPIKE-001` Selection rectangle continuous redraw。
- `RND-SPIKE-002` Multiple vector objects and resize handles。
- `RND-SPIKE-005` Hit testing accuracy。
- Transform／clipping evidence within the above controlled scenes。

Entry criteria：synthetic workload、host/candidate eligibility、environment record schema 與 separate authorization 已確認。

Exit criteria：每個已執行 Spike 都有 functional、fidelity、failure、cleanup evidence，或明確 `Blocked`／`Not executed` reason。

### Phase R2 — Fidelity and Export

- `RND-SPIKE-003` Text, font fallback and scaling。
- `RND-SPIKE-004` Mosaic interaction。
- `RND-SPIKE-006` Same screen output at different DPI。
- `RND-SPIKE-007` Display render versus PNG export comparison。
- `RND-SPIKE-008` Alpha and transparent overlay。

Entry criteria：R1 的 coordinate、object、hit-test limitation 已保存；Output、alpha、font、color metadata 欄位已具備。

Exit criteria：display/export、DPI、text、mosaic、alpha、color 與 clipping evidence 可追溯；未核准的 threshold 仍為 `TBD`。

### Phase R3 — Interop and Resource Observation

- `RND-SPIKE-009` CPU/GPU and memory observation。
- `RND-SPIKE-010` WinUI 3/WPF host interoperability。

Entry criteria：前兩階段的 workload 與 environment record 可重複；resource cleanup 與 host adapter boundary 已明確。

Exit criteria：interop、lifecycle、resource observation、cleanup 與 cross-host limitations 已記錄，不代表 TD-002 已決定。

每個 Phase 必須各自具備 Entry criteria、Exit criteria、Blocking condition 與 Required evidence。前一 Phase 完成不自動授權下一 Phase。

## 14. Stop Rules

未來執行時，發生下列任一情況必須停止該 Spike 或 Phase：

- 需要先決定 `ADR-0002` 才能繼續，但該決定尚未完成。
- 需要導入正式 Capture Backend、Clipboard 或真實桌面像素。
- 需要修改 frozen PRD、Specs、Architecture 或 upstream research document。
- 需要讓 Rendering component 擁有 Workflow State、Annotation Domain、Capture 或 Clipboard coordination。
- Candidate comparison workload 不再等價，或特定 candidate 使用未揭露的特殊 optimization。
- Host、Windows Build、hardware、DPI 或 version 不同且無法校正或記錄。
- 結果不可重現、required evidence 無法保存或 cleanup 未完成。
- Display 與 export 的座標、alpha、color 或 pixel dimensions 定義不一致。
- 需要新增未經記錄的 product dependency、package、installer 或 SDK。
- 出現 credential、secret、私人圖片或其他不在 synthetic scope 的資料。
- 任何人要求把 `Not executed`、`Blocked` 或 `Unknown` 改寫成成功。

Stop 後必須留下 stop reason、last completed step、known limitation、cleanup state 與下一個 resolution condition；不得刪除失敗 evidence。

## 15. Result Artifact Plan

### 15.1 Future-only destination

本任務只規劃，不建立下列目錄或檔案：

`docs/Research/Technology/results/rendering/`

未來預定檔名：

- `RND-SPIKE-001-result.md`
- `RND-SPIKE-002-result.md`
- `RND-SPIKE-003-result.md`
- `RND-SPIKE-004-result.md`
- `RND-SPIKE-005-result.md`
- `RND-SPIKE-006-result.md`
- `RND-SPIKE-007-result.md`
- `RND-SPIKE-008-result.md`
- `RND-SPIKE-009-result.md`
- `RND-SPIKE-010-result.md`

### 15.2 Artifact contents

每個 future result artifact 可包含：

- Environment Record。
- Synthetic workload identifier。
- Candidate／Host／version／attempt metadata。
- Required evidence inventory。
- Functional Gate classification。
- Fidelity comparison。
- Comparative Metrics 與 method。
- Failure reproduction 與 failure implication。
- Known limitations。
- Cleanup confirmation。
- Open questions 與 `RND-PLAN-GAP` references。

本輪不得建立 results directory、result artifact、PNG output、reference image、screen recording、diagnostic log 或任何 Runtime output。

## 16. Decision Evidence Roll-up

### 16.1 Roll-up schema

| RND Gate | Candidate | Host | Result | Evidence completeness | TD-002 impact |
| --- | --- | --- | --- | --- | --- |
| `RND-GATE-001` | TBD | TBD | Not executed | TBD | Not evaluated |
| `RND-GATE-002` | TBD | TBD | Not executed | TBD | Not evaluated |
| `RND-GATE-003` | TBD | TBD | Not executed | TBD | Not evaluated |
| `RND-GATE-004` | TBD | TBD | Not executed | TBD | Not evaluated |
| `RND-GATE-005` | TBD | TBD | Not executed | TBD | Not evaluated |
| `RND-GATE-006` | TBD | TBD | Not executed | TBD | Not evaluated |
| `RND-GATE-007` | TBD | TBD | Not executed | TBD | Not evaluated |
| `RND-GATE-008` | TBD | TBD | Not executed | TBD | Not evaluated |
| `RND-GATE-009` | TBD | TBD | Not executed | TBD | Not evaluated |

### 16.2 TD-002 impact vocabulary

Future roll-up 的 `TD-002 impact` 只能使用：

- `Supports candidate`
- `Challenges candidate`
- `Neutral`
- `Insufficient evidence`

本文件的 `Not evaluated` 是未執行邊界，不是 candidate support 或 challenge 結論。不得預填實際 Runtime result。

## 17. Readiness to Execute

### 17.1 Allowed readiness values

Readiness 只能使用：

- `Ready for rendering runtime spike execution`
- `Conditionally ready`
- `Not ready`

### 17.2 Current readiness

| Field | Value |
| --- | --- |
| Readiness to Execute | `Not ready` |
| ADR-0002 | Draft／未決 |
| Runtime／SDK／Candidate versions | 尚未為本計畫固定 |
| Project／Build path | 未建立、未授權 |
| Experimental environment | 未建立、未授權 |
| Runtime Verification | `Not performed` |
| Runtime Spike Execution | `Not authorized` |

`Not ready` 由下列 prerequisites 推導：

- `ADR-0002` 尚未完成，Host Framework 尚未成為可接受的固定前提。
- Runtime／SDK／Candidate versions 尚未固定並取得 provenance。
- Project、Build path、experimental environment 與 future result destination 尚未授權。
- 本輪明確禁止 Restore、Build、Run、Prototype、Runtime Spike 與 product Source Code。

即使未來改為 `Conditionally ready`，也必須逐一核對 Phase entry criteria、candidate-host eligibility、authorization、environment integrity 與 stop rules；前一階段的核准不會自動授權下一階段。

## 18. Traceability

### 18.1 Required sources

| Traceability target | Source |
| --- | --- |
| Parent Rendering feasibility | `docs/Research/Technology/10-rendering-technology-feasibility.md` |
| UI framework technical evidence | `docs/Research/Technology/01-ui-framework-feasibility.md` |
| Phase 1 readiness and authorization boundary | `docs/Research/Technology/07-ui-framework-phase1-readiness-reassessment.md` |
| Host framework decision boundary | `Architecture/adr/ADR-0002-ui-framework-selection.md` |
| Technology decision ordering | `Architecture/TECHNOLOGY-DECISION-ROADMAP.md` |
| Product requirements | `PRD/PRD-0002-user-experience-principles.md`, `PRD/PRD-0003-product-vision.md`, `PRD/PRD-0004-core-workflow.md`, `PRD/PRD-0006-non-functional-requirements.md` |
| Workflow／annotation specifications | `Specs/SPEC-0005-capture-workflow.md`, `Specs/SPEC-0009-annotation-capability.md`, `Specs/SPEC-0010-feature-integration.md` |
| Architecture boundaries | `Architecture/ARCH-0002-layer-model.md`, `Architecture/ARCH-0003-module-catalog.md`, `Architecture/ARCH-0004-component-boundaries.md`, `Architecture/ARCH-0005-component-interactions.md` |

### 18.2 Evidence chain

```text
Product requirement
  -> RND criterion
  -> RND Gate
  -> RND Spike
  -> Runtime evidence package
  -> Evidence roll-up
  -> Future TD-002 decision
```

This chain is descriptive and future-facing. It does not create a Rendering Decision, accept `ADR-0002`, authorize a Project or authorize product Coding.

## 19. Completion Boundary

### 19.1 Completion conditions

本文件完成時，必須符合：

- 本輪只建立 `docs/Research/Technology/11-rendering-technology-runtime-spike-plan.md`。
- 不修改 README、索引、CHANGELOG、TODO 或任何其他文件。
- 保留原有十個 Spike ID、名稱與範圍。
- 十個 Spike Status 全部為 `Planned`。
- 建立 Candidate–Host Eligibility Matrix、Controlled Comparison Rules、Synthetic Rendering Workload Contract 與 Test Environment Record。
- 每個 Spike 具備 common fixed schema 的所有欄位。
- 建立 Gate Coverage Matrix，並覆蓋 `RND-001` 至 `RND-018` 與 `RND-GATE-001` 至 `RND-GATE-009`。
- 建立 Execution Order、Stop Rules、Result Artifact Plan、Decision Evidence Roll-up 與 Readiness to Execute。
- Readiness to Execute 維持 `Not ready`。
- 不建立 results directory、Result、Project、Prototype、Source Code 或 Build File。
- 不執行 Restore、Build、Run、Performance Test、Runtime Spike 或任何 Runtime Verification。
- 不修改 `ADR-0002`，不建立 Rendering ADR，不作 Rendering Decision。
- `git diff --check` 通過，且文件沒有 trailing whitespace。

### 19.2 Current completion state

| Field | Value |
| --- | --- |
| Plan document | Created |
| Spike definitions | 10 bound, all `Planned` |
| Runtime execution | `Not started` / `Not authorized` |
| Runtime Verification | `Not performed` |
| Readiness to Execute | `Not ready` |
| Rendering Decision | `Not made` |
| Rendering ADR | Not created or modified |
| Result directory | Not created |
| Project／Prototype／Source Code | Not created |

完成本文件不代表任何 Candidate 已成為 `Ready`、`Accepted` 或產品適用，也不代表可以開始 Capture、Annotation、Rendering Coding 或 Runtime Spike。
