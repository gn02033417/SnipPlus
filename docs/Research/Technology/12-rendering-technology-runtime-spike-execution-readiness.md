# Rendering Technology Runtime Spike Execution Readiness

本文件把 `RESEARCH-TECH-RENDER-002` 的執行條件轉換為可審核的就緒度判定。它只描述未來 runtime spike 是否具備開始審核的前置條件，不代表任何 package、SDK、project、prototype、source code 或 runtime result 已建立。

## Document Control

| Field | Value |
|---|---|
| Document ID | `RESEARCH-TECH-RENDER-003` |
| Title | Rendering Technology Runtime Spike Execution Readiness |
| Status | Draft |
| Research Type | Runtime Execution Readiness |
| Execution Status | Not started |
| Runtime Verification | Not performed |
| Build Verification | Not performed |
| Parent Plan | `RESEARCH-TECH-RENDER-002` |
| Technology Decision | `TD-002 Rendering Technology` |
| Host Framework Decision | Unresolved |
| Rendering Decision | Not made |
| Runtime Spike Execution Authorized | No |
| Owner | TBD |
| Last reviewed | Not reviewed |

## 1. Purpose

本文件的目的如下：

1. 將 `RESEARCH-TECH-RENDER-002` 的 execution condition 轉成明確的 readiness 判定。
2. 分離 rendering candidate 的前置條件與共用 UI framework research 的前置條件。
3. 避免重複提出需求，或繞過 `RESEARCH-TECH-UI-009` 的人工作業授權邊界。
4. 分別判定每個 Candidate–Host pair 與每個 runtime spike 是否具備條件。
5. 建立未來 enablement 所需的最小 blocker set。

## 2. Scope

本文件涵蓋：

- `RND-OPT-001` 至 `RND-OPT-005`。
- WinUI 3 與 WPF host framework。
- `RND-SPIKE-001` 至 `RND-SPIKE-010`。
- Runtime spike 所需的版本與 package baseline。
- Host build path、candidate package/native dependency 與 synthetic workload。
- Display、DPI、color、reference image、pixel-difference 與 evidence storage baseline。
- Safety、cleanup 與人工作業授權依賴。

## 3. Non-goals

本文件不會：

- 執行任何 runtime spike。
- 建立 Project、Solution、Prototype 或 source code。
- 建立 result directory、reference image、render output 或 measurement artifact。
- 執行 restore、build、run、publish、package 安裝、SDK 安裝、download 或 measurement。
- 做出 rendering technology decision。
- 建立或修改 `TD-002` ADR。
- 修改 `ADR-0002`。
- 凍結任何 Runtime、SDK 或 rendering library 的正式版本選擇。
- 修改 PRD、Specs、Architecture 或既有 UI research status。
- 重新要求由 `RESEARCH-TECH-UI-009` 管理的 shared host authority。
- 撰寫或開始任何截圖功能。

## 4. Readiness Vocabulary

### 4.1 Prerequisite Status

前置條件只能使用下列狀態：

| Status | Definition |
|---|---|
| Resolved | 已有直接 evidence，且符合本文件定義的 closure condition。 |
| Partially resolved | 已有部分定義或既有 research evidence，但仍缺少執行所需的直接 evidence。 |
| Blocked | 存在未解決條件，會阻止相依的 pair、spike 或 phase。 |
| Deferred | 目前 phase 不必完成，但必須保留在後續 phase 的 traceability。 |
| Not applicable | 有明確 evidence 證明該條件不適用。 |

沒有 evidence 的條件不得標記為 `Resolved`。

### 4.2 Candidate–Host Pair Readiness

| Status | Definition |
|---|---|
| Ready | Pair 的版本、依賴、host、evidence、environment 與 authorization 條件均已滿足。 |
| Conditionally ready | 只有明確列出的非阻塞限制尚待確認，且不得直接執行。 |
| Blocked | 至少一個必要條件未解決。 |
| Excluded with evidence | 有直接 evidence 證明 pair 不納入該研究範圍。 |
| Not evaluated | 尚未具備足夠資料進行判定。 |

### 4.3 Spike Readiness 與 Authorization

| Item | Allowed value |
|---|---|
| Spike Readiness | `Ready`、`Blocked`、`Deferred`、`Not applicable` |
| Execution Authorization | `No`、`Pending separate authorization` |
| Current document authorization | `No` |

本文件禁止使用單獨的 `Yes` 表示 execution authorization。Plan 完成、package page 存在或 host 可以推測可用，均不足以改變 authorization。

## 5. Dependency Separation Rules

| Dependency class | Boundary | Required interpretation |
|---|---|---|
| Shared UI-host dependency | WinUI 3/WPF host、SDK、build tool、project isolation、UI research line | 必須引用既有 `BA-*`、`UI-ENABLE-*` 或 `UI-AUTH-*`；不得複製 shared host authorization。 |
| Rendering-candidate dependency | Win2D、SkiaSharp、Direct2D interop | 可記錄為 rendering prerequisite，但不得在本文件要求或執行 package acquisition。 |
| Rendering-workload dependency | Synthetic canvas、reference output、pixel comparison | 必須先定義 workload 與 evidence method；本文件不建立實際 asset。 |
| Environment dependency | GPU、DPI、monitor、HDR、color context | 必須區分既有 baseline、待驗證環境與可 deferred 的 phase。 |
| Evidence dependency | Log、render output、PNG、measurement、comparison tooling | Evidence path 未被驗證前，不得宣稱 fidelity gate 已通過。 |
| Authorization dependency | Install、download、restore、project、build、runtime 的人工作業 permission | 授權是獨立狀態，不能由文件完整度推導。 |

### 5.1 Separation Rules

- Shared host blocker 必須引用既有 UI research item，不建立第二份 host authorization。
- Rendering package/tool 可以列為 prerequisite，但本文件不請求、不下載、不安裝、不 restore。
- UI host build success 不等於 rendering candidate availability。
- Package restore success 不等於 runtime gate passed。
- `RESEARCH-TECH-UI-009` 的人工作業授權仍是 shared UI boundary 的唯一 authority。
- 本文件的 `Ready` 只代表可進入未來 authorization review，不代表已獲 runtime execution permission。

## 6. Shared UI Research Dependency Matrix

| Rendering dependency | Source UI item | Current UI status | Rendering impact | Can be inherited | Remaining condition |
|---|---|---|---|---|---|
| WinUI 3 experimental build path | `RESEARCH-TECH-UI-007` / `BA-001` / `UI-AUTH-001` | Partially resolved | Blocking | No | 需完成既有 UI build-path provenance 與獨立 authorization。 |
| WPF experimental build path | `RESEARCH-TECH-UI-007` / `BA-002` / `UI-AUTH-002` | Partially resolved | Blocking | No | 需完成既有 WPF equivalent path definition 與 authorization。 |
| Windows x64 baseline | `RESEARCH-TECH-UI-007` / `UI-PREQ-002` | Partially resolved | Blocking for R1 | Conditional | 需在 authorized host path 中記錄 actual baseline。 |
| Display topology | `RESEARCH-TECH-UI-007` / `BA-003` | Blocked | Blocking for DPI/fidelity | No | 需有 single、multi-monitor 與 topology record。 |
| Per-monitor DPI | `RESEARCH-TECH-UI-007` / `BA-004` | Blocked | Blocking for coordinate/fidelity | No | 需有 same-DPI 與 heterogeneous-DPI evidence path。 |
| Evidence root policy | `RESEARCH-TECH-UI-007` / `BA-006` | Partially resolved | Blocking for evidence governance | Conditional | 需確認每次 spike 的 allowed root、naming 與 retention。 |
| Safety/cleanup | `RESEARCH-TECH-UI-007` / `BA-007` | Partially resolved | Blocking for isolated execution | Conditional | 需完成 cleanup acceptance，且不得污染 product source。 |
| Isolated experimental project boundary | `RESEARCH-TECH-UI-008` / `UI-ENABLE-005` | Blocked | Blocking | No | 需由 UI authorization line 建立與確認 isolation boundary。 |
| Restore/Build authorization | `RESEARCH-TECH-UI-009` / `UI-AUTH-001`..`UI-AUTH-007` | Blocked | Blocking | No | 需取得既有 UI enablement 的 separate authorization。 |
| Runtime execution authorization | `RESEARCH-TECH-UI-009` / `BA-008` / `UI-AUTH-008` | Blocked | Blocking | No | 需由人工作業另行核准；本文件不代替該核准。 |

本表不修改 `RESEARCH-TECH-UI-007`、`RESEARCH-TECH-UI-008` 或 `RESEARCH-TECH-UI-009` 的 status。共享依賴的 blocker 只在 rendering register 中被引用，不會被重複開立為另一個 UI action。

## 7. Rendering Prerequisite Register

| Prerequisite ID | Description | Dependency class | Related candidates | Related hosts | Related spikes | Source evidence | Current status | Required final evidence | Mutation required | Separate authorization required | Resolution condition | Owner | Open questions |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| `RND-PREQ-001` | Precise experimental host framework version | Shared UI-host | All | WinUI 3, WPF | All | `RESEARCH-TECH-RENDER-002` §7 | Blocked | Version and provenance record | No | Yes | Exact version is tied to an authorized isolated host path | TBD | Which host is first authorized? |
| `RND-PREQ-002` | .NET/Windows App SDK baseline | Shared UI-host | All | WinUI 3 | `001`–`010` | UI `BA-001` | Blocked | SDK/runtime baseline record | No | Yes | Baseline is recorded from actual authorized environment | TBD | What baseline is permitted? |
| `RND-PREQ-003` | Framework-native render path definition | Rendering-candidate | `RND-OPT-001`, `002` | WinUI 3, WPF | `001`, `002`, `003` | Feasibility §5 | Partially resolved | Host-native path description and output evidence | No | Yes | One path is defined per host and tested later | TBD | Is WPF baseline native-only? |
| `RND-PREQ-004` | Direct2D/DirectWrite interop path | Rendering-candidate | `RND-OPT-003` | WPF | `004`, `005` | Feasibility §5 | Blocked | Interop boundary and native dependency evidence | No | Yes | Interop boundary is isolated and attributable | TBD | Which interop surface is allowed? |
| `RND-PREQ-005` | Win2D package/host compatibility | Rendering-candidate | `RND-OPT-004` | WinUI 3 | `006`, `007` | Feasibility §5 | Blocked | Official version, package asset and host compatibility record | No | Yes | Candidate package is authorized and proven compatible | TBD | Which version is available in the authorized environment? |
| `RND-PREQ-006` | SkiaSharp package/native asset compatibility | Rendering-candidate | `RND-OPT-005` | WinUI 3, WPF | `008`, `009` | Feasibility §5 | Blocked | Managed/native asset and host compatibility record | No | Yes | Native assets are resolved in isolated spike scope | TBD | Which native asset/runtime pair is permitted? |
| `RND-PREQ-007` | Hybrid boundary | Rendering-candidate | `RND-OPT-003`..`005` | WinUI 3, WPF | `004`–`009` | Feasibility §6 | Partially resolved | Explicit ownership of host, interop and candidate layers | No | Yes | No layer has ambiguous rendering ownership | TBD | Where is text and input ownership? |
| `RND-PREQ-008` | Synthetic workload readiness | Rendering-workload | All | WinUI 3, WPF | All | Plan §5 | Partially resolved | Workload contract and reproducible input sequence | Yes, future-only | Yes | All required workload items are defined and reviewable | TBD | Which assets require human sign-off? |
| `RND-PREQ-009` | Reference image generation method | Rendering-workload | All | WinUI 3, WPF | `003`, `006`, `008`, `010` | Plan §8 | Blocked | Reproducible reference method and provenance | Yes, future-only | Yes | Reference output can be regenerated without product source | TBD | What reference renderer is authoritative? |
| `RND-PREQ-010` | PNG decoder/pixel comparison method | Evidence | All | WinUI 3, WPF | `003`, `006`, `008`, `010` | Plan §8 | Blocked | Decoder, channel order, threshold and report format | Yes, future-only | Yes | Method is defined before any measurement | TBD | What comparison tool is permitted? |
| `RND-PREQ-011` | Text/font fallback test assets | Rendering-workload | `RND-OPT-001`..`005` | WinUI 3, WPF | `003`, `006`, `008`, `010` | Feasibility §7 | Blocked | Fixed mixed-language text and font provenance | Yes, future-only | Yes | Text cases and fallback observation are reproducible | TBD | Which fonts are guaranteed on the host? |
| `RND-PREQ-012` | Mosaic reference algorithm | Rendering-workload | `RND-OPT-001`..`005` | WinUI 3, WPF | `003`, `006`, `008`, `010` | Feasibility §7 | Blocked | Deterministic algorithm and expected output description | Yes, future-only | Yes | Mosaic transformation is independent of UI screenshot code | TBD | What boundary behavior is required? |
| `RND-PREQ-013` | DPI/coordinate scenarios | Environment | All | WinUI 3, WPF | `003`, `005`, `007`, `010` | UI `BA-004` | Blocked | Logical-to-pixel cases for same and mixed DPI | No | Yes | Scenarios are recorded with actual monitor topology | TBD | Which scale factors are in R1? |
| `RND-PREQ-014` | Alpha/pixel format baseline | Rendering-workload | All | WinUI 3, WPF | `003`, `006`, `008` | Feasibility §7 | Blocked | Format, alpha mode and conversion evidence | No | Yes | Candidate output can be compared on equal format terms | TBD | Premultiplied or straight alpha? |
| `RND-PREQ-015` | Color-space/HDR observation method | Environment | All | WinUI 3, WPF | `003`, `006`, `008`, `010` | Plan §8 | Deferred | Observation record and metadata policy | No | Yes | R2/R3 method exists before relevant spike | TBD | Is HDR required or observation-only? |
| `RND-PREQ-016` | CPU/GPU/memory observation method | Environment | All | WinUI 3, WPF | `002`, `004`, `007`, `009` | Plan §8 | Deferred | Reproducible observation fields and timing policy | No | Yes | R3 method is defined before resource spike | TBD | Which counters are in scope? |
| `RND-PREQ-017` | Evidence naming/storage | Evidence | All | WinUI 3, WPF | All | UI `BA-006` | Partially resolved | Approved root, naming, manifest and retention record | No | Yes | Evidence can be stored without modifying product repository | TBD | Which root is approved? |
| `RND-PREQ-018` | Candidate isolation/cleanup | Authorization | All | WinUI 3, WPF | All | UI `BA-007` | Partially resolved | Cleanup checklist and post-run boundary evidence | Yes, future-only | Yes | No generated artifact remains outside approved scope | TBD | Who accepts cleanup? |
| `RND-PREQ-019` | Package acquisition authorization | Authorization | `RND-OPT-004`, `005` | WinUI 3, WPF | `006`–`009` | UI `UI-AUTH-003` | Blocked | Explicit permission record and acquired-version provenance | Yes, future-only | Yes | Human authorization exists for the exact package scope | TBD | Is network acquisition permitted? |
| `RND-PREQ-020` | Build authorization | Authorization | All | WinUI 3, WPF | All | UI `UI-AUTH-005`..`007` | Blocked | Exact project/build authorization and build log | Yes, future-only | Yes | Human authorization exists for isolated build | TBD | Which configuration is authorized? |
| `RND-PREQ-021` | Runtime authorization | Authorization | All | WinUI 3, WPF | All | UI `UI-AUTH-008` | Blocked | Exact runtime authorization and runtime log | Yes, future-only | Yes | Human authorization exists for the selected spike only | TBD | Which spike may run first? |

## 8. Rendering Blocker Register

Blocker 只有在有 resolution evidence 時才能關閉；「可能可用」、「package page 存在」或「理論上相容」都不是 closure evidence。

| Blocker ID | Source prerequisite | Description | Severity | Affected Candidate–Host pairs | Affected Spikes | Required resolution | Evidence required | Shared UI dependency | Owner | Status |
|---|---|---|---|---|---|---|---|---|---|---|
| `RND-BLOCK-001` | `RND-PREQ-001`..`003` | Host framework、SDK 與 native render path 尚未有實際 authorized provenance。 | Blocking | All non-excluded pairs | All | 完成既有 UI host path 與 candidate path 的 separated baseline。 | Version、host、build-path record | `BA-001`, `BA-002`, `UI-AUTH-001`, `UI-AUTH-002` | TBD | Open |
| `RND-BLOCK-002` | `RND-PREQ-013` | Display topology 與 per-monitor DPI 未完成。 | Blocking | All pairs needing coordinate/fidelity evidence | `003`, `005`, `007`, `010` | 完成 single/multi-monitor 與 same/mixed-DPI baseline。 | Environment record and coordinate evidence | `BA-003`, `BA-004` | TBD | Open |
| `RND-BLOCK-003` | `RND-PREQ-008` | Synthetic content/input workload 尚未可獨立、可重現地執行。 | Blocking | All pairs | All | 完成 workload contract 與 isolation boundary。 | Workload manifest and input sequence | `BA-005`, `UI-ENABLE-005` | TBD | Open |
| `RND-BLOCK-004` | `RND-PREQ-017` | Evidence root、naming、retention 尚未正式可用。 | Blocking | All pairs | All | 取得 approved evidence storage policy。 | Manifest, path policy and cleanup record | `BA-006` | TBD | Open |
| `RND-BLOCK-005` | `RND-PREQ-018` | Candidate isolation 與 cleanup acceptance 尚未完成。 | Blocking | All pairs | All | 建立並接受不污染 product source 的 cleanup boundary。 | Before/after scope record | `BA-007` | TBD | Open |
| `RND-BLOCK-006` | `RND-PREQ-019`..`021` | Package、build 與 runtime action 尚未取得 separate human authorization。 | Blocking | All pairs | All | 依 exact scope 取得 authorization；不得由本文件推導。 | Authorization record and audit trail | `BA-008`, `UI-AUTH-003`..`008` | TBD | Open |
| `RND-BLOCK-007` | `RND-PREQ-005`..`007` | Candidate package/native dependency 與 hybrid boundary 尚未有版本及 ownership evidence。 | Blocking | Win2D/SkiaSharp/Direct2D pairs | `004`–`009` | 完成 candidate-specific dependency baseline。 | Dependency manifest and isolation evidence | None; rendering-specific | TBD | Open |
| `RND-BLOCK-008` | `RND-PREQ-009`..`012` | Fidelity、PNG、font 與 mosaic comparison method 尚未建立。 | Non-blocking for earliest vector-only R1; blocking for fidelity phase | Fidelity pairs | `003`, `006`, `008`, `010` | 在 R2 前完成 reproducible evidence method。 | Reference/output comparison package | None; rendering-specific | TBD | Open |
| `RND-BLOCK-009` | `RND-PREQ-015`..`016` | Color/HDR 與 resource observation 尚未定義。 | Non-blocking for earliest R1; blocking for R3 | Resource/color pairs | `002`, `004`, `007`, `009` | 在 R3 前完成 observation method。 | Observation record and metadata | None; rendering-specific | TBD | Open |

## 9. Candidate Version and Dependency Baseline

Official version 與 local existing 狀態分開記錄。Exact version 尚未確定時使用 `TBD` 或 `Unknown`；official package page 的存在不代表 local availability。

| Candidate | Experimental version | Official source | Host | Managed/native dependency | Local availability | Build verified | Runtime verified | Status |
|---|---|---|---|---|---|---|---|---|
| `RND-OPT-001` Framework-native baseline | TBD | `RESEARCH-TECH-RENDER-001` feasibility candidate record | WinUI 3 | Host framework rendering surface | Unknown | No | No | Blocked |
| `RND-OPT-002` WPF-native baseline | TBD | `RESEARCH-TECH-RENDER-001` feasibility candidate record | WPF | WPF rendering surface and text path | Unknown | No | No | Blocked |
| `RND-OPT-003` Direct2D/DirectWrite interop | TBD | `RESEARCH-TECH-RENDER-001` feasibility candidate record | WPF | Native Direct2D/DirectWrite interop | Unknown | No | No | Blocked |
| `RND-OPT-004` Win2D | TBD | `RESEARCH-TECH-RENDER-001` feasibility candidate record | WinUI 3 | Win2D managed/native package assets | Unknown | No | No | Blocked |
| `RND-OPT-005` SkiaSharp | TBD | `RESEARCH-TECH-RENDER-001` feasibility candidate record | WinUI 3, WPF | Managed package plus native runtime asset | Unknown | No | No | Blocked |

此表只服務 future spike 的 dependency provenance，不構成正式產品版本選擇。

## 10. Candidate–Host Pair Register

`Eligibility` 沿用 `RESEARCH-TECH-RENDER-002`；`Current readiness` 是本文件的新判定。Unknown eligibility 不得改寫成 `Excluded with evidence`。

| Pair ID | Candidate | Host | Eligibility from `RESEARCH-TECH-RENDER-002` | Required Host capability | Required candidate dependency | Native interop requirement | Package acquisition requirement | Build requirement | Runtime requirement | Current readiness | Blocking IDs | Exclusion evidence | Execution authorization | Notes |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| `RND-PAIR-001` | `RND-OPT-001` Framework-native | WinUI 3 | Conditionally eligible | WinUI 3 isolated build path | Host-native rendering surface | No | Host SDK/package baseline | Yes | Yes | Blocked | `001`, `002`, `003`, `006` | None | No | Shared UI authorization required first. |
| `RND-PAIR-002` | `RND-OPT-001` Framework-native | WPF | Not evaluated | WPF isolated build path | Host-native rendering surface | No | Host SDK/runtime baseline | Yes | Yes | Blocked | `001`, `002`, `006` | None | No | Requires WPF path definition. |
| `RND-PAIR-003` | `RND-OPT-002` WPF-native | WinUI 3 | Not evaluated | WinUI 3 host boundary | Candidate not yet defined | No | Candidate baseline | Yes | Yes | Not evaluated | `001`, `006` | None | No | Technically meaningful only if host boundary is defined. |
| `RND-PAIR-004` | `RND-OPT-002` WPF-native | WPF | Conditionally eligible | WPF isolated build path | WPF native render surface | No | Host SDK/runtime baseline | Yes | Yes | Blocked | `001`, `002`, `006` | None | No | Shared WPF authorization required. |
| `RND-PAIR-005` | `RND-OPT-003` Direct2D/DirectWrite | WinUI 3 | Not evaluated | WinUI 3 interop host boundary | Direct2D/DirectWrite native path | Yes | Native dependency authorization | Yes | Yes | Blocked | `001`, `004`, `006`, `007` | None | No | Not a host-native baseline. |
| `RND-PAIR-006` | `RND-OPT-003` Direct2D/DirectWrite | WPF | Conditionally eligible | WPF interop host boundary | Direct2D/DirectWrite native path | Yes | Native dependency authorization | Yes | Yes | Blocked | `001`, `004`, `006`, `007` | None | No | Candidate-specific native evidence absent. |
| `RND-PAIR-007` | `RND-OPT-004` Win2D | WinUI 3 | Conditionally eligible | WinUI 3 isolated build path | Win2D package/native assets | Possible | Package acquisition authorization | Yes | Yes | Blocked | `001`, `005`, `006`, `007` | None | No | Package page is not local availability evidence. |
| `RND-PAIR-008` | `RND-OPT-004` Win2D | WPF | Excluded with evidence | WPF host boundary | Win2D host compatibility | Possible | Package acquisition authorization | Yes | Yes | Excluded with evidence | `005` | Plan eligibility excludes this pairing | No | Retain exclusion evidence; do not silently reuse. |
| `RND-PAIR-009` | `RND-OPT-005` SkiaSharp | WinUI 3 | Conditionally eligible | WinUI 3 isolated build path | SkiaSharp managed/native assets | Possible | Package/native acquisition authorization | Yes | Yes | Blocked | `001`, `006`, `007` | None | No | Native asset provenance is absent. |
| `RND-PAIR-010` | `RND-OPT-005` SkiaSharp | WPF | Conditionally eligible | WPF isolated build path | SkiaSharp managed/native assets | Possible | Package/native acquisition authorization | Yes | Yes | Blocked | `001`, `006`, `007` | None | No | Native asset and WPF path both require evidence. |

所有非排除 pair 的 execution authorization 均為 `No`。`RND-PAIR-008` 的 exclusion 只來自既有 plan eligibility，不代表 Win2D 在其他 host 已被驗證。

## 11. Synthetic Workload Readiness

本節只定義 readiness，不建立實際 reference image、PNG、canvas asset 或 screenshot artifact。

| Workload item | Definition status | Reference artifact required | Tool/dependency | Readiness | Blocking ID |
|---|---|---|---|---|---|
| Canvas logical size | Partially resolved | No; future manifest only | Host coordinate API | Blocked | `RND-BLOCK-003` |
| Export pixel size | Partially resolved | Yes; future output | PNG/export path | Blocked | `RND-BLOCK-003`, `004` |
| Background color blocks | Partially resolved | Yes; future reference | Color/alpha method | Blocked | `RND-BLOCK-003`, `008` |
| Selection rectangle | Partially resolved | Yes; future reference | Vector primitive path | Blocked | `RND-BLOCK-003` |
| Semi-transparent mask | Partially resolved | Yes; future reference | Alpha/pixel format baseline | Blocked | `RND-BLOCK-003`, `008` |
| Vector shapes | Partially resolved | Yes; future reference | Candidate render path | Blocked | `RND-BLOCK-003`, `007` |
| Stroke width variations | Partially resolved | Yes; future reference | Logical-to-pixel mapping | Blocked | `RND-BLOCK-002`, `003` |
| Rotation | Partially resolved | Yes; future reference | Transform path | Blocked | `RND-BLOCK-002`, `003` |
| Resize handles | Partially resolved | Yes; future reference | Input/hit-test sequence | Blocked | `RND-BLOCK-003` |
| Overlapping layers | Partially resolved | Yes; future reference | Alpha/compositing path | Blocked | `RND-BLOCK-003`, `008` |
| Mixed-language text | Blocked | Yes; future reference | Font asset provenance | Blocked | `RND-BLOCK-003`, `008` |
| Font fallback | Blocked | Yes; future reference | Fixed font/fallback set | Blocked | `RND-BLOCK-003`, `008` |
| Mosaic region | Blocked | Yes; future reference | Mosaic reference algorithm | Blocked | `RND-BLOCK-003`, `008` |
| Clipping | Partially resolved | Yes; future reference | Candidate clip path | Blocked | `RND-BLOCK-003` |
| Alpha gradient | Blocked | Yes; future reference | Alpha/pixel format method | Blocked | `RND-BLOCK-003`, `008` |
| Scale variations | Partially resolved | Yes; future reference | DPI/scale scenarios | Blocked | `RND-BLOCK-002`, `003` |
| Pointer/hit-test sequence | Blocked | No; future input manifest | Host input API | Blocked | `RND-BLOCK-003` |

## 12. Fidelity Evidence Readiness

| Evidence capability | Required method | Tool/dependency | Current status | Runtime required | Blocking effect |
|---|---|---|---|---|---|
| Reference image | Reproducible generation method with provenance | Reference renderer and manifest | Blocked | Yes | Blocks fidelity comparison |
| Rendered output | Capture/export from each candidate path | Candidate render path | Blocked | Yes | Blocks output evidence |
| PNG export | Fixed dimensions, channel order and metadata policy | PNG encoder/decoder | Blocked | Yes | Blocks pixel evidence |
| Pixel-difference calculation | Defined comparison, threshold and report format | Comparison tooling | Blocked | Yes | Blocks quantitative fidelity claim |
| Alpha-channel inspection | Explicit alpha mode and conversion rules | Pixel inspection tooling | Blocked | Yes | Blocks compositing claim |
| Logical-to-pixel coordinate comparison | Same-DPI and mixed-DPI coordinate cases | Display/DPI baseline | Blocked | Yes | Blocks coordinate claim |
| Text/font fallback inspection | Fixed multilingual text and font provenance | Host fonts and text path | Blocked | Yes | Blocks text fidelity claim |
| Mosaic comparison | Deterministic mosaic reference algorithm | Workload algorithm | Blocked | Yes | Blocks mosaic claim |
| Color-space metadata | Record color profile, HDR state and conversion observation | Display/color metadata tooling | Deferred | Yes | Blocks R2/R3 color claim only |
| Failure reproduction | Stable input, environment and evidence manifest | Evidence storage and cleanup | Blocked | Yes | Blocks reliable failure triage |

Visual observation alone 不是 PNG fidelity evidence。PNG export success alone 不是 display/export consistency evidence。沒有 threshold 的 pixel diff 只能算 comparative output，不能自行產生品質結論。本文件不發明 color threshold。

## 13. Environment Readiness

| Environment requirement | Available evidence | Status | Required for phase | Deferred allowed | Affected spikes |
|---|---|---|---|---|---|
| Windows 11 x64 baseline | Existing UI research identifies required baseline; no runtime record | Partially resolved | R1 | No | All |
| GPU/driver | No runtime inventory in this research line | Blocked | R3 | Yes for R1/R2 | `002`, `004`, `007`, `009` |
| Single monitor baseline | Requirement identified; no actual record | Blocked | R1 | No | `003`, `005`, `010` |
| Multi-monitor topology | Requirement identified; no actual record | Blocked | R2 | Yes for earliest R1 | `003`, `005`, `010` |
| Same DPI | Requirement identified; no actual record | Blocked | R1 | No | `003`, `005`, `010` |
| Heterogeneous DPI | Requirement identified; no actual record | Blocked | R2 | Yes for earliest R1 | `003`, `005`, `010` |
| HDR | No runtime observation method | Deferred | R3 | Yes | `003`, `006`, `008`, `010` |
| Acceleration state | No runtime observation method | Deferred | R3 | Yes | `002`, `004`, `007`, `009` |
| Software fallback path | No runtime observation method | Deferred | R3 | Yes | `002`, `004`, `007`, `009` |
| Debug/Release | Build path not authorized | Blocked | R1 | No | All |
| Cold/warm state | No runtime protocol | Deferred | R3 | Yes | `002`, `004`, `007`, `009` |
| Stable power mode | No runtime record | Deferred | R3 | Yes | `002`, `004`, `007`, `009` |

Phase R1 不等待與核心 correctness 無關的 ARM64 或 full Packaging matrix；這些條件不得被錯誤提升為 R1 blocker，但仍須在適用 phase 維持 traceability。

## 14. Per-Spike Readiness Matrix

| Spike | Required pairs | Required prerequisites | Required environment | Required evidence capability | Readiness | Blocking IDs | Execution authorized |
|---|---|---|---|---|---|---|---|
| `RND-SPIKE-001` Framework-native WinUI baseline | `RND-PAIR-001` | `001`, `002`, `003`, `017`, `018`, `020`, `021` | x64, single monitor, same DPI | Rendered output, failure reproduction | Blocked | `001`, `003`, `004`, `005`, `006` | No |
| `RND-SPIKE-002` Framework-native WPF baseline | `RND-PAIR-004` | `001`, `002`, `003`, `017`, `018`, `020`, `021` | x64, single monitor, same DPI | Rendered output, failure reproduction | Blocked | `001`, `003`, `004`, `005`, `006` | No |
| `RND-SPIKE-003` Basic vector correctness | `RND-PAIR-001`, `004` | `003`, `008`, `013`, `014`, `017`, `020`, `021` | x64, single monitor, same DPI | Reference image, rendered output, coordinate comparison | Blocked | `001`..`006` | No |
| `RND-SPIKE-004` Direct2D/DirectWrite interop | `RND-PAIR-006` | `004`, `007`, `016`, `019`, `020`, `021` | x64, acceleration observation | Rendered output, resource observation | Blocked | `001`, `006`, `007` | No |
| `RND-SPIKE-005` DPI and coordinate behavior | `RND-PAIR-001`, `004` | `008`, `013`, `014`, `017`, `020`, `021` | Same and heterogeneous DPI | Logical-to-pixel comparison | Blocked | `002`, `003`, `004`, `005`, `006` | No |
| `RND-SPIKE-006` Win2D package candidate | `RND-PAIR-007` | `005`, `008`, `017`, `018`, `019`, `020`, `021` | x64, single monitor, same DPI | Rendered output, PNG export | Blocked | `001`, `005`, `006`, `007` | No |
| `RND-SPIKE-007` Win2D hybrid boundary | `RND-PAIR-007` | `005`, `007`, `008`, `013`, `016`, `017`, `019`, `020`, `021` | x64, acceleration observation | Coordinate, resource and failure evidence | Blocked | `001`, `002`, `005`, `006`, `007` | No |
| `RND-SPIKE-008` SkiaSharp package candidate | `RND-PAIR-009`, `010` | `006`, `008`, `014`, `017`, `018`, `019`, `020`, `021` | x64, single monitor, same DPI | Rendered output, PNG, alpha inspection | Blocked | `001`, `005`, `006`, `007` | No |
| `RND-SPIKE-009` SkiaSharp resource behavior | `RND-PAIR-009`, `010` | `006`, `007`, `008`, `016`, `017`, `018`, `019`, `020`, `021` | GPU/driver, acceleration, cold/warm | Resource observation, failure reproduction | Blocked | `001`, `005`, `006`, `007`, `009` | No |
| `RND-SPIKE-010` Fidelity/export comparison | `RND-PAIR-001`, `004`, `007`, `009` | `008`..`018`, `020`, `021` | DPI, color and export baseline | All fidelity capabilities | Blocked | `001`..`009` | No |

## 15. Phase Readiness

| Phase | Minimum definition | Current status | Blocking interpretation |
|---|---|---|---|
| R1 Core Rendering Correctness | 至少一個 WinUI 3 與一個 WPF host build path evidence、framework-native baseline、synthetic vector/hit-test workload、evidence storage/cleanup 可執行，且 separately obtained project/build permission。 | Not ready | Shared UI blockers、RND-specific pair baseline、workload、storage/cleanup 與 authorization 尚未完成。 |
| R2 Fidelity and Export | R1 plus reference output method、PNG export/pixel comparison method、text/mosaic/alpha/DPI evidence path。 | Not ready | R1 未 ready，且 fidelity evidence register 仍有 open blockers。 |
| R3 Interop/Resource | R2 plus host interoperability pair、CPU/GPU/memory method、color/HDR method。 | Not ready | R2 未 ready，且 interop/resource/color prerequisites 尚未完成。 |

Current `Build Verification` 與 `Runtime Verification` 均為 `Not performed`，因此任何 phase 都不得標記為 `Ready`。

## 16. Minimum Blocking Action Set

本表只列出最早可進入 R1 所真正需要的 action。R2/R3-only 的 color、HDR、resource 與 full fidelity 工作不被提前提升成 R1 blocker。

| Action ID | Blocking condition | Source IDs | Required evidence | Mutation required | Authorization dependency |
|---|---|---|---|---|---|
| `RND-BA-001` | Shared WinUI 3/WPF host build provenance 尚未成立。 | `BA-001`, `BA-002`, `UI-AUTH-001`, `UI-AUTH-002`, `RND-PREQ-001`..`003` | Exact host/SDK baseline and isolated build-path record | Future isolated experimental project only | Existing UI host/build authorization; do not duplicate |
| `RND-BA-002` | R1 所需的 display、single-monitor 與 same-DPI baseline 尚未成立。 | `BA-003`, `BA-004`, `RND-PREQ-013` | Actual topology and coordinate baseline | Future environment record only | Existing UI environment authorization |
| `RND-BA-003` | Synthetic vector、hit-test 與 input workload 尚未隔離且可重現。 | `BA-005`, `UI-ENABLE-005`, `RND-PREQ-008` | Workload manifest and input sequence | Future workload artifact only | Existing UI isolation authorization |
| `RND-BA-004` | Evidence root、naming、retention 與 cleanup boundary 尚未被接受。 | `BA-006`, `BA-007`, `RND-PREQ-017`, `018` | Storage policy, manifest and cleanup acceptance | Future evidence directory only | Existing evidence/safety authorization |
| `RND-BA-005` | 至少一個 framework-native Candidate–Host pair 的 version/dependency baseline 尚未完成。 | `RND-PREQ-001`..`007`, `RND-BLOCK-007` | Candidate-host dependency provenance | Future package/project isolation only | Exact package/tool action requires separate approval |
| `RND-BA-006` | Rendering candidate 的 package、build 與 runtime action 尚未得到獨立 permission。 | `RND-PREQ-019`..`021`, `RND-BLOCK-006` | Exact-scope human authorization record | Future authorized action only | `RESEARCH-TECH-UI-009` boundary plus rendering-specific approval |

`RND-BA-001` 至 `RND-BA-004` 引用既有 UI actions，不建立重複的 shared host request。`RND-BA-005` 與 `RND-BA-006` 是 rendering-specific minimum actions。Action 數量由 evidence dependency 決定，不預設固定數量。

## 17. Overall Readiness Decision

### 17.1 Fixed Current State

| Decision field | Current value |
|---|---|
| Build Verification | Not performed |
| Runtime Verification | Not performed |
| Runtime Spike Execution Authorized | No |
| Rendering Decision | Not made |
| Overall Readiness Decision | Not ready |

### 17.2 Derivation

Overall readiness is derived from:

`Open shared UI blockers + Open rendering prerequisites + Candidate–Host pair readiness + Per-Spike readiness + Authorization status -> Overall Readiness Decision`

本文件允許的 overall decision values 是：

- `Ready for rendering runtime spike execution authorization review`
- `Conditionally ready for rendering runtime spike execution authorization review`
- `Not ready`

目前因為 shared UI blockers、rendering prerequisites、pair readiness、per-spike readiness 及 authorization 均未滿足，唯一正確判定是 `Not ready`。文件完成不會自動將它改成 `Ready`，也不會授權任何 spike。

## 18. Traceability

### 18.1 Source Documents

- [`docs/Research/Technology/10-rendering-technology-feasibility.md`](10-rendering-technology-feasibility.md)
- [`docs/Research/Technology/11-rendering-technology-runtime-spike-plan.md`](11-rendering-technology-runtime-spike-plan.md)
- [`docs/Research/Technology/07-ui-framework-phase1-readiness-reassessment.md`](07-ui-framework-phase1-readiness-reassessment.md)
- [`docs/Research/Technology/08-ui-framework-phase1-execution-enablement-specification.md`](08-ui-framework-phase1-execution-enablement-specification.md)
- [`docs/Research/Technology/09-ui-framework-phase1-enablement-execution-authorization-request.md`](09-ui-framework-phase1-enablement-execution-authorization-request.md)
- [`Architecture/adr/ADR-0002-ui-framework-selection.md`](../../../Architecture/adr/ADR-0002-ui-framework-selection.md)
- [`Architecture/TECHNOLOGY-DECISION-ROADMAP.md`](../../../Architecture/TECHNOLOGY-DECISION-ROADMAP.md)

### 18.2 Decision Chain

```text
UI shared dependency
  -> RND prerequisite
  -> Candidate–Host pair
  -> RND Spike
  -> Phase readiness
  -> Future execution authorization review
  -> Future TD-002 decision
```

`TD-002` 只有在 future spikes 完成、evidence 可重現、authorization boundary 清楚且決策 criteria 被滿足後，才能進入正式 decision review。本文件不提前做該決策。

## 19. Completion Boundary

本文件完成時的邊界如下：

- 只有本文件被建立；沒有建立 result directory、project、prototype、source code 或 image asset。
- Shared UI dependency matrix、`RND-PREQ-*`、`RND-BLOCK-*`、candidate baseline、pair register、workload、fidelity、environment、十個 spike、R1–R3 readiness 與 minimum blocking action set 均已列出。
- 所有 execution authorization 均為 `No`。
- 沒有執行 download、install、restore、build、run、measure 或 runtime spike。
- 沒有修改 `ADR-0002` 或建立 `TD-002` ADR。
- Build 與 Runtime verification 均維持 `Not performed`。
- Overall readiness 維持 `Not ready`。
- 未開始任何截圖功能或截圖相關 coding。
