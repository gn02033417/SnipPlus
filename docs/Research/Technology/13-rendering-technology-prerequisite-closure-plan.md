# Rendering Technology Prerequisite Closure Plan

本文件是 `RESEARCH-TECH-RENDER-003` 的 prerequisite closure plan。它把阻止 Phase R1 的 Rendering action 轉成可追溯的未來工作邊界，但不執行任何 closure action、不建立實際 evidence，也不授權 runtime spike。

## Document Control

| Field | Value |
|---|---|
| Document ID | `RESEARCH-TECH-RENDER-004` |
| Title | Rendering Technology Prerequisite Closure Plan |
| Status | Draft |
| Research Type | Prerequisite Closure Plan |
| Execution Status | Not started |
| Build Verification | Not performed |
| Runtime Verification | Not performed |
| Parent Readiness Record | `RESEARCH-TECH-RENDER-003` |
| Parent Runtime Plan | `RESEARCH-TECH-RENDER-002` |
| Technology Decision | `TD-002 Rendering Technology` |
| Host Framework Decision | Unresolved |
| Rendering Decision | Not made |
| Runtime Spike Execution Authorized | No |
| Owner | TBD |
| Last reviewed | Not reviewed |
| Version | 0.1 |
| Preparation date | 2026-07-26 |
| Normative References | `RESEARCH-TECH-RENDER-003`, existing UI authorization boundary |
| Informative References | `RESEARCH-TECH-RENDER-001`, `RESEARCH-TECH-RENDER-002` |
| Supersedes | None |
| Superseded by | None |

## 1. 任務目的

本文件只回答：

> 如何以最小、可追溯且需明確人工授權的方式，關閉 `RESEARCH-TECH-RENDER-003` 中阻止 Phase R1 的 6 個 Rendering Blocking Actions？

這是 Closure Plan，不是 Closure Execution Record。

## 2. Scope

本文件只處理：

- `RND-BA-001` 至 `RND-BA-006`。
- `RND-PREQ-001` 至 `RND-PREQ-021`。
- `RND-BLOCK-001` 至 `RND-BLOCK-009`。
- `RND-PAIR-001` 至 `RND-PAIR-010`。
- `RND-SPIKE-001` 至 `RND-SPIKE-010`。
- Phase R1 所需的最小 Host、candidate、workload、evidence 與 authorization 條件。

Phase R2/R3 項目只能分類為後續依賴，不得自動成為 Phase R1 blocker。

## 3. Non-goals

本文件不得：

- 執行任何 Closure Action。
- 執行唯讀系統盤點。
- 查詢或變更本機 Package cache。
- 下載或安裝工具、SDK、Runtime 或 Package。
- 執行 Restore、Build、Run、Publish 或 Runtime Spike。
- 建立 Project、Solution、Prototype、Result directory 或 Source Code。
- 建立 Reference Image、PNG、Pixel Difference 或 Measurement Artifact。
- 申請或核准 Runtime Spike execution。
- 修改 UI Research Line 文件。
- 修改 `RESEARCH-TECH-RENDER-001` 至 `RESEARCH-TECH-RENDER-003`。
- 修改 `ADR-0002`。
- 建立 TD-002 ADR。
- 選擇 Rendering Technology。

## 4. Status Vocabulary

### 4.1 Closure Action Status

Closure Action 只能使用：

`Planned`、`Blocked`、`Deferred`、`Not applicable`

Closure Action 不得使用：

`Completed`、`Resolved`、`Approved`、`Authorized`

### 4.2 Target Status Recommendation

對上游 prerequisite、blocker、pair 或 phase 的建議只能使用：

`Resolved`、`Partially resolved`、`Blocked`、`Deferred`、`Not applicable`

這些是未來 action 完成後的 recommendation，不是本文件現在宣稱的結果。

### 4.3 Dependency Ownership

Dependency ownership 只能使用：

`Shared UI research`、`Rendering-specific`、`Environment`、`Evidence`、`Authorization`

### 4.4 Current Boundary

- 本文件可以規劃 future operation，但不能執行 operation。
- `Current authorization` 一律為 `Not granted`。
- `Execution permitted` 一律為 `No`。
- Plan 完整不等於 closure action 已完成。
- Closure recommendation 不會修改上游文件狀態。

## 5. Closure Action Binding

建立一對一綁定：

| Closure Action | Source Blocking Action |
|---|---|
| `RND-CLOSE-001` | `RND-BA-001` |
| `RND-CLOSE-002` | `RND-BA-002` |
| `RND-CLOSE-003` | `RND-BA-003` |
| `RND-CLOSE-004` | `RND-BA-004` |
| `RND-CLOSE-005` | `RND-BA-005` |
| `RND-CLOSE-006` | `RND-BA-006` |

規則：

- 保留原始 `RND-BA` 名稱與語意。
- 不得重新編號、合併或拆分 Blocking Action。
- Closure Action 可以包含多個依序執行的 sub-step，但不得因此建立新的 `RND-BA`。
- 上游描述不足時建立 `RND-CLOSURE-GAP-xxx`，不得直接修改上游文件。

## 6. Closure Action Definitions

每個 `RND-CLOSE` 都使用相同的固定欄位。下列內容只描述未來的 closure boundary；所有 future operation 均須另外取得授權。

### 6.1 RND-CLOSE-001 — Shared Host Build Provenance

| Field | Value |
|---|---|
| Closure Action ID | `RND-CLOSE-001` |
| Source Blocking Action | `RND-BA-001` |
| Blocking condition | WinUI 3/WPF host、SDK 與 framework-native path 尚未有實際 authorized provenance。 |
| Related `RND-PREQ` | `RND-PREQ-001`, `002`, `003` |
| Related `RND-BLOCK` | `RND-BLOCK-001` |
| Related Candidate–Host pairs | `RND-PAIR-001`, `002`, `004` |
| Related Spikes | `RND-SPIKE-001`, `002`, `003` |
| Dependency ownership | Shared UI research |
| Shared UI source IDs | `BA-001`, `BA-002`, `UI-AUTH-001`, `UI-AUTH-002` |
| Current evidence | `RESEARCH-TECH-RENDER-003` readiness record；沒有 closure execution evidence。 |
| Required final evidence | Exact host/SDK baseline、isolated build-path provenance、candidate host ownership record。 |
| Proposed closure operation | 在既有 UI authorization boundary 內，確認允許的 WinUI 3 與 WPF host path；只建立 rendering 需要的 provenance record。 |
| Operation classification | Repository documentation mutation；可能包含 future read-only inspection。 |
| Read-only inspection required | Future only；本文件不執行。 |
| Network access required | TBD；不得由本文件推導。 |
| Package acquisition required | Candidate-specific acquisition 另由 `RND-CLOSE-005` 管理。 |
| Installation required | No for this plan；future host setup 需 separate authorization。 |
| Repository mutation required | Future documentation/evidence boundary only。 |
| Experimental project required | Future isolated project may be required。 |
| Restore required | Future only；not authorized。 |
| Build required | Future only；not authorized。 |
| Runtime execution required | No for closure planning；future spike separately authorized。 |
| System configuration mutation required | No planned。 |
| Administrator privilege required | TBD；must be explicitly recorded if later proposed。 |
| Human authorization required | Yes；reuse existing UI host/build authority。 |
| Expected files or directories | Future approved provenance record；no path is created now。 |
| Expected system effect | Future isolated host capability record；no current system effect。 |
| Success condition | Host and SDK provenance is explicit, isolated and traceable to UI authorization。 |
| Failure condition | Host path, version or ownership remains ambiguous。 |
| Rollback and cleanup | Remove only future temporary provenance/evidence artifacts within approved scope。 |
| Resulting prerequisite recommendation | `RND-PREQ-001`..`003` -> Partially resolved or Resolved only with direct evidence。 |
| Resulting blocker recommendation | `RND-BLOCK-001` remains Blocked until closure evidence exists。 |
| Resulting pair recommendation | `RND-PAIR-001`, `002`, `004` remain Blocked until exact pair evidence exists。 |
| Phase R1 impact | Direct R1 blocker。 |
| Owner | TBD |
| Status | Blocked |
| Open questions | Which host path and exact baseline are allowed first? |

### 6.2 RND-CLOSE-002 — Display and DPI Baseline

| Field | Value |
|---|---|
| Closure Action ID | `RND-CLOSE-002` |
| Source Blocking Action | `RND-BA-002` |
| Blocking condition | Single-monitor、same-DPI、topology 與 coordinate baseline 尚未有 actual record。 |
| Related `RND-PREQ` | `RND-PREQ-013` |
| Related `RND-BLOCK` | `RND-BLOCK-002` |
| Related Candidate–Host pairs | `RND-PAIR-001`, `004`, `007`, `009`, `010` |
| Related Spikes | `RND-SPIKE-003`, `005`, `006`, `008`, `010` |
| Dependency ownership | Environment |
| Shared UI source IDs | `BA-003`, `BA-004` |
| Current evidence | UI research identifies the requirement；沒有 actual environment evidence。 |
| Required final evidence | Approved environment record、single-monitor/same-DPI coordinate cases、topology provenance。 |
| Proposed closure operation | Future authorized environment baseline capture；不改變系統 DPI 或 monitor configuration。 |
| Operation classification | Evidence capture；可能包含 read-only inspection。 |
| Read-only inspection required | Future only；本文件不執行。 |
| Network access required | No planned。 |
| Package acquisition required | No。 |
| Installation required | No planned。 |
| Repository mutation required | Future environment/evidence manifest only。 |
| Experimental project required | No for baseline planning；future spike may require isolated host。 |
| Restore required | No for baseline planning。 |
| Build required | No for baseline planning。 |
| Runtime execution required | Future spike only；not this plan。 |
| System configuration mutation required | No；DPI/HDR settings must not be changed by default。 |
| Administrator privilege required | No planned；if later required, stop for explicit review。 |
| Human authorization required | Yes；reuse existing UI environment authority。 |
| Expected files or directories | Future approved environment manifest；no file is created now。 |
| Expected system effect | Future record only；no current system effect。 |
| Success condition | Same-DPI R1 baseline is reproducible without altering system configuration。 |
| Failure condition | Topology、DPI or logical-to-pixel mapping cannot be attributed。 |
| Rollback and cleanup | Delete only future temporary environment evidence under approved retention policy。 |
| Resulting prerequisite recommendation | `RND-PREQ-013` -> Partially resolved or Resolved only with direct evidence。 |
| Resulting blocker recommendation | `RND-BLOCK-002` remains Blocked for missing direct baseline。 |
| Resulting pair recommendation | Coordinate-dependent pairs remain Blocked until evidence is complete。 |
| Phase R1 impact | Direct R1 blocker for coordinate correctness。 |
| Owner | TBD |
| Status | Blocked |
| Open questions | Which scale factor and monitor topology are the minimum R1 baseline? |

### 6.3 RND-CLOSE-003 — Synthetic Workload Isolation

| Field | Value |
|---|---|
| Closure Action ID | `RND-CLOSE-003` |
| Source Blocking Action | `RND-BA-003` |
| Blocking condition | Synthetic vector、hit-test、pointer/input sequence 與 isolated boundary 尚未可重現。 |
| Related `RND-PREQ` | `RND-PREQ-008`, `011`, `012`, `013`, `014` |
| Related `RND-BLOCK` | `RND-BLOCK-003` |
| Related Candidate–Host pairs | All non-excluded pairs |
| Related Spikes | `RND-SPIKE-001`..`010` |
| Dependency ownership | Rendering-specific |
| Shared UI source IDs | `BA-005`, `UI-ENABLE-005` |
| Current evidence | `RESEARCH-TECH-RENDER-003` workload list；沒有實際 workload manifest。 |
| Required final evidence | Workload manifest、fixed input sequence、asset provenance、project isolation record。 |
| Proposed closure operation | Future documentation and isolated workload preparation；不建立實際 asset 或 prototype。 |
| Operation classification | Repository documentation mutation；future experimental project creation may be separate。 |
| Read-only inspection required | Future only；本文件不執行。 |
| Network access required | No planned。 |
| Package acquisition required | No for workload definition。 |
| Installation required | No for workload definition。 |
| Repository mutation required | Future workload manifest only。 |
| Experimental project required | Future runtime execution may require one。 |
| Restore required | Future only；not authorized。 |
| Build required | Future only；not authorized。 |
| Runtime execution required | Future spike only；not this closure plan。 |
| System configuration mutation required | No planned。 |
| Administrator privilege required | No planned。 |
| Human authorization required | Yes；reuse UI isolation authority plus rendering-specific scope。 |
| Expected files or directories | Future workload manifest under approved evidence root；none now。 |
| Expected system effect | No current system effect。 |
| Success condition | Same synthetic workload and input sequence can be consumed by an isolated candidate pair。 |
| Failure condition | Workload depends on product source, user data or non-reproducible interaction。 |
| Rollback and cleanup | Remove future temporary workload artifacts within approved boundary。 |
| Resulting prerequisite recommendation | `RND-PREQ-008`, `011`..`014` -> Partially resolved or Resolved with evidence。 |
| Resulting blocker recommendation | `RND-BLOCK-003` remains Blocked until isolation evidence exists。 |
| Resulting pair recommendation | All pair readiness remains Blocked until the workload contract is usable。 |
| Phase R1 impact | Direct R1 blocker。 |
| Owner | TBD |
| Status | Blocked |
| Open questions | Which font, mosaic and pointer assets are permitted for the first workload? |

### 6.4 RND-CLOSE-004 — Evidence Storage and Cleanup

| Field | Value |
|---|---|
| Closure Action ID | `RND-CLOSE-004` |
| Source Blocking Action | `RND-BA-004` |
| Blocking condition | Evidence root、naming、retention、cleanup 與 repository boundary 尚未被接受。 |
| Related `RND-PREQ` | `RND-PREQ-017`, `018` |
| Related `RND-BLOCK` | `RND-BLOCK-004`, `RND-BLOCK-005` |
| Related Candidate–Host pairs | All non-excluded pairs |
| Related Spikes | All spikes |
| Dependency ownership | Evidence |
| Shared UI source IDs | `BA-006`, `BA-007` |
| Current evidence | UI research has a policy requirement；沒有 actual evidence root or cleanup record。 |
| Required final evidence | Approved root policy、manifest、retention rule、before/after cleanup record。 |
| Proposed closure operation | Future policy confirmation and cleanup rehearsal within an approved isolated scope。 |
| Operation classification | Repository documentation mutation；future evidence capture may be separate。 |
| Read-only inspection required | Future only；本文件不執行。 |
| Network access required | No planned。 |
| Package acquisition required | No。 |
| Installation required | No。 |
| Repository mutation required | Future policy/manifest only。 |
| Experimental project required | No for policy planning。 |
| Restore required | No。 |
| Build required | No。 |
| Runtime execution required | No for policy planning。 |
| System configuration mutation required | No。 |
| Administrator privilege required | No planned。 |
| Human authorization required | Yes；reuse existing evidence and safety authority。 |
| Expected files or directories | Future approved evidence root；not created now。 |
| Expected system effect | No current system effect。 |
| Success condition | Future evidence can be stored, identified and removed without touching product source。 |
| Failure condition | Evidence path is ambiguous, retention is undefined or cleanup leaves artifacts。 |
| Rollback and cleanup | Future rehearsal must clean only its own approved scope。 |
| Resulting prerequisite recommendation | `RND-PREQ-017`, `018` -> Partially resolved or Resolved with direct evidence。 |
| Resulting blocker recommendation | `RND-BLOCK-004`, `005` remain Blocked until accepted evidence exists。 |
| Resulting pair recommendation | Pair readiness cannot exceed Blocked before evidence boundary is accepted。 |
| Phase R1 impact | Direct R1 blocker。 |
| Owner | TBD |
| Status | Blocked |
| Open questions | Which evidence root and retention policy will be accepted? |

### 6.5 RND-CLOSE-005 — Candidate–Host Dependency Baseline

| Field | Value |
|---|---|
| Closure Action ID | `RND-CLOSE-005` |
| Source Blocking Action | `RND-BA-005` |
| Blocking condition | 至少一個 framework-native pair 與一個 rendering-specific candidate 的 version/dependency provenance 尚未固定。 |
| Related `RND-PREQ` | `RND-PREQ-004`..`007`, `019` |
| Related `RND-BLOCK` | `RND-BLOCK-006`, `RND-BLOCK-007` |
| Related Candidate–Host pairs | `RND-PAIR-001`, `004`, `006`, `007`, `009`, `010` |
| Related Spikes | `RND-SPIKE-001`..`009` |
| Dependency ownership | Rendering-specific |
| Shared UI source IDs | `UI-AUTH-003`, `UI-AUTH-004`, `UI-AUTH-005` |
| Current evidence | Feasibility and readiness documents list candidates；沒有 exact local dependency provenance。 |
| Required final evidence | Candidate version、official source、managed/native asset、host compatibility、acquisition provenance。 |
| Proposed closure operation | Future candidate-specific baseline preparation after separate package/tool authorization；不下載、不安裝、不 restore。 |
| Operation classification | Package acquisition；future experimental project creation may be separate。 |
| Read-only inspection required | Future only；本文件不執行。 |
| Network access required | TBD；must be explicitly approved。 |
| Package acquisition required | Possibly; exact candidate scope must be approved。 |
| Installation required | Possibly; never implicit。 |
| Repository mutation required | Future dependency/provenance manifest only。 |
| Experimental project required | Future pair verification may require one。 |
| Restore required | Future only；not authorized。 |
| Build required | Future only；not authorized。 |
| Runtime execution required | No for planning; future spike separately authorized。 |
| System configuration mutation required | No planned。 |
| Administrator privilege required | TBD；must stop if later required。 |
| Human authorization required | Yes；package/build scope requires exact separate approval。 |
| Expected files or directories | Future dependency manifest and approved cache/project boundary；none now。 |
| Expected system effect | No current system effect。 |
| Success condition | At least the R1 candidate-host baseline has exact, attributable dependencies。 |
| Failure condition | Version is TBD, package is only web-visible, or native asset ownership is ambiguous。 |
| Rollback and cleanup | Remove future package/project artifacts only inside approved temporary scope。 |
| Resulting prerequisite recommendation | Candidate-specific `RND-PREQ-004`..`007`, `019` -> Partially resolved or Resolved with evidence。 |
| Resulting blocker recommendation | `RND-BLOCK-007` remains Blocked until dependency provenance exists。 |
| Resulting pair recommendation | Only evidence-backed pairs may move from Blocked; unknown pairs remain Not evaluated。 |
| Phase R1 impact | Direct R1 blocker for candidate comparison。 |
| Owner | TBD |
| Status | Blocked |
| Open questions | Which candidate is the first non-native comparison target, and which exact version is allowed? |

### 6.6 RND-CLOSE-006 — Rendering Closure Execution Authorization

| Field | Value |
|---|---|
| Closure Action ID | `RND-CLOSE-006` |
| Source Blocking Action | `RND-BA-006` |
| Blocking condition | Package、project、restore、build、evidence 與 runtime actions 尚未取得 exact-scope human permission。 |
| Related `RND-PREQ` | `RND-PREQ-019`, `020`, `021` |
| Related `RND-BLOCK` | `RND-BLOCK-006` |
| Related Candidate–Host pairs | All non-excluded pairs |
| Related Spikes | All spikes |
| Dependency ownership | Authorization |
| Shared UI source IDs | `BA-008`, `UI-AUTH-003`..`UI-AUTH-008` |
| Current evidence | `RESEARCH-TECH-RENDER-003` explicitly records authorization No。 |
| Required final evidence | Exact-scope authorization record，操作清單、期限、owner、cleanup 與 stop condition。 |
| Proposed closure operation | Future authorization review only；不在本文件提出實際 permission request。 |
| Operation classification | Repository documentation mutation；future authorization review。 |
| Read-only inspection required | No current execution。 |
| Network access required | Not granted；future scope-specific。 |
| Package acquisition required | Not granted。 |
| Installation required | Not granted。 |
| Repository mutation required | Future authorization record only。 |
| Experimental project required | Not granted。 |
| Restore required | Not granted。 |
| Build required | Not granted。 |
| Runtime execution required | Not granted；runtime spike remains separate。 |
| System configuration mutation required | No planned。 |
| Administrator privilege required | Not granted。 |
| Human authorization required | Yes；must be explicit and separate from this plan。 |
| Expected files or directories | Future authorization record；no file is created now。 |
| Expected system effect | No current system effect。 |
| Success condition | Each future operation has an explicit permission and execution remains bounded。 |
| Failure condition | Permission is inferred from plan completeness or a broad instruction。 |
| Rollback and cleanup | Future operation must have explicit stop, rollback and cleanup conditions。 |
| Resulting prerequisite recommendation | `RND-PREQ-019`..`021` remain Blocked until human permission exists。 |
| Resulting blocker recommendation | `RND-BLOCK-006` remains Blocked until exact authorization evidence exists。 |
| Resulting pair recommendation | No pair receives execution permission from this document。 |
| Phase R1 impact | Direct R1 blocker；runtime remains a later separate authorization。 |
| Owner | TBD |
| Status | Blocked |
| Open questions | Who may approve each exact operation and what is the permitted scope? |

## 7. Operation Classification

本節只定義分類，不執行任何分類中的操作。

| Classification | Definition |
|---|---|
| `Read-only inspection` | 官方或本機唯讀資料查核。 |
| `Repository documentation mutation` | 僅建立或修改研究文件；本輪只建立本文件。 |
| `Package acquisition` | NuGet Restore 或 Package download。 |
| `Development environment installation` | 安裝 SDK、Build Tool 或工具。 |
| `Experimental project creation` | 建立隔離式 Rendering Project。 |
| `Build execution` | 編譯候選 Project。 |
| `Runtime execution` | 啟動 Rendering Prototype 或 Spike。 |
| `Evidence capture` | 建立輸出、Log、PNG 或量測資料。 |
| `System configuration mutation` | 修改 DPI、HDR、Registry 或系統設定。 |

Package acquisition、installation、project、restore、build、runtime、evidence capture 與 system configuration mutation 均必須有 separate human authorization；本文件不執行。

## 8. Shared UI Dependency Reuse Matrix

| Rendering closure action | Required shared capability | Existing UI source | Current status | New authorization needed | Duplication prohibited |
|---|---|---|---|---|---|
| `RND-CLOSE-001` | WinUI 3 build path | `RESEARCH-TECH-UI-007`, `BA-001`, `UI-AUTH-001` | Blocked | No new shared request; existing boundary only | Do not create a second WinUI host authorization |
| `RND-CLOSE-001` | WPF build path | `RESEARCH-TECH-UI-007`, `BA-002`, `UI-AUTH-002` | Blocked | No new shared request; existing boundary only | Do not create a second WPF host authorization |
| `RND-CLOSE-001` | Windows x64 baseline | `RESEARCH-TECH-UI-007`, `UI-PREQ-002` | Partially resolved | Existing UI environment authority | Do not create rendering copy of x64 action |
| `RND-CLOSE-003` | Isolated experimental project boundary | `RESEARCH-TECH-UI-008`, `UI-ENABLE-005` | Blocked | Existing UI isolation authority plus candidate scope | Do not create duplicate isolation policy |
| `RND-CLOSE-005` | Package Restore boundary | `RESEARCH-TECH-UI-009`, `UI-AUTH-003` | Blocked | Exact package scope later | Do not treat package permission as host permission |
| `RND-CLOSE-001` | Build verification boundary | `RESEARCH-TECH-UI-009`, `UI-AUTH-005`..`007` | Blocked | Existing UI build authority | Do not convert rendering readiness into build permission |
| `RND-CLOSE-004` | Evidence root | `RESEARCH-TECH-UI-007`, `BA-006` | Partially resolved | Existing evidence policy | Do not create a second root without approval |
| `RND-CLOSE-004` | Safety/cleanup | `RESEARCH-TECH-UI-007`, `BA-007` | Partially resolved | Existing safety authority | Do not close from a plan statement alone |
| `RND-CLOSE-006` | Runtime execution authorization | `RESEARCH-TECH-UI-009`, `BA-008`, `UI-AUTH-008` | Blocked | Separate future authorization | Do not merge with enablement authorization |

要求：

- 已存在的 `UI-AUTH` 不得重新建立 Rendering 版本。
- Rendering closure 只能引用或增加 candidate-specific 範圍。
- Shared UI authorization 尚未核准時，相關 `RND-CLOSE` 保持 `Blocked`。

## 9. Rendering-specific Dependency Matrix

| Candidate | Required package/API | Exact experimental version status | Native asset/interop | Host scope | Required evidence | Closure Action |
|---|---|---|---|---|---|---|
| Framework-native retained-mode | Host-native rendering surface | TBD | Host framework only | WinUI 3, WPF | Host path、version、output provenance | `RND-CLOSE-001` |
| Direct2D/DirectWrite | Direct2D/DirectWrite interop surface | TBD | Native interop | WPF; other host only if separately eligible | Interop boundary、native dependency、ownership | `RND-CLOSE-005` |
| Win2D | Win2D managed/native package | TBD | Win2D native assets | WinUI 3 | Package identity、host compatibility、output path | `RND-CLOSE-005` |
| SkiaSharp | SkiaSharp managed package and native runtime asset | TBD | Managed/native asset pair | WinUI 3, WPF | Asset provenance、host compatibility、resource path | `RND-CLOSE-005` |
| Hybrid strategy | Host + candidate ownership boundary | TBD | Candidate-specific interop | WinUI 3, WPF | Layer ownership、input/text boundary、cleanup | `RND-CLOSE-005` |

規則：

- 官方可用版本與本機 availability 分開。
- 版本未查核時使用 `TBD`。
- 不得將 Package 網頁存在視為本機可建置。
- 不得將 Runtime package 存在視為 SDK 或 Build capability。
- 不得決定產品正式版本。

## 10. Candidate–Host Pair Closure Matrix

| Pair | Eligibility from `RESEARCH-TECH-RENDER-002` | Current readiness | Blocking IDs | Required Closure Action | Required evidence | Target recommendation |
|---|---|---|---|---|---|---|
| `RND-PAIR-001` Framework-native / WinUI 3 | Conditionally eligible | Blocked | `RND-BLOCK-001`, `002`, `003`, `004`, `005`, `006` | `RND-CLOSE-001`..`006` as applicable | Host path、workload、DPI、evidence、authorization | Blocked |
| `RND-PAIR-002` Framework-native / WPF | Not evaluated | Not evaluated | `RND-BLOCK-001`, `006` | `RND-CLOSE-001`, `006` | WPF path and exact authorization | Not evaluated |
| `RND-PAIR-003` WPF-native / WinUI 3 | Not evaluated | Not evaluated | `RND-BLOCK-001`, `006` | `RND-CLOSE-001`, `006` | Host boundary and candidate provenance | Not evaluated |
| `RND-PAIR-004` WPF-native / WPF | Conditionally eligible | Blocked | `RND-BLOCK-001`, `002`, `003`, `004`, `005`, `006` | `RND-CLOSE-001`..`006` as applicable | WPF host and R1 workload evidence | Blocked |
| `RND-PAIR-005` Direct2D/DirectWrite / WinUI 3 | Not evaluated | Not evaluated | `RND-BLOCK-001`, `006`, `007` | `RND-CLOSE-001`, `005`, `006` | Interop and native dependency provenance | Not evaluated |
| `RND-PAIR-006` Direct2D/DirectWrite / WPF | Conditionally eligible | Blocked | `RND-BLOCK-001`, `006`, `007` | `RND-CLOSE-001`, `005`, `006` | WPF interop boundary and native asset record | Blocked |
| `RND-PAIR-007` Win2D / WinUI 3 | Conditionally eligible | Blocked | `RND-BLOCK-001`, `003`, `004`, `005`, `006`, `007` | `RND-CLOSE-003`..`006` | Package, workload, output and cleanup evidence | Blocked |
| `RND-PAIR-008` Win2D / WPF | Excluded with evidence | Excluded with evidence | `RND-BLOCK-007` | No closure action can re-enable it | Plan exclusion evidence retained | Excluded with evidence |
| `RND-PAIR-009` SkiaSharp / WinUI 3 | Conditionally eligible | Blocked | `RND-BLOCK-001`, `003`, `004`, `005`, `006`, `007` | `RND-CLOSE-003`..`006` | Native asset、host、workload、resource boundary | Blocked |
| `RND-PAIR-010` SkiaSharp / WPF | Conditionally eligible | Blocked | `RND-BLOCK-001`, `003`, `004`, `005`, `006`, `007` | `RND-CLOSE-001`, `003`..`006` | WPF host and managed/native asset evidence | Blocked |

要求：

- 所有 Pair 都有一列。
- `Unknown` eligibility 不得直接建議 `Excluded with evidence`。
- 排除 Pair 必須保留可引用的排除證據。
- Pair 可被 Phase R2/R3 `Deferred`，但必須說明不阻塞 Phase R1 的原因。

## 11. Phase R1 Minimum Closure Gate

| Gate | 最低條件 | Related Closure Action | Gate Plan Status |
|---|---|---|---|
| `RND-CGATE-001` | 至少一個 WinUI 3 Host build path 已有證據或明確授權路徑。 | `RND-CLOSE-001`, `006` | Blocked |
| `RND-CGATE-002` | 至少一個 WPF Host build path 已有證據或明確授權路徑。 | `RND-CLOSE-001`, `006` | Blocked |
| `RND-CGATE-003` | Framework-native baseline 已固定。 | `RND-CLOSE-001`, `005` | Blocked |
| `RND-CGATE-004` | 至少一個非 native candidate 的 package/interop 路徑已規格化。 | `RND-CLOSE-005` | Blocked |
| `RND-CGATE-005` | Synthetic vector/hit-test workload 已完全規格化。 | `RND-CLOSE-003` | Blocked |
| `RND-CGATE-006` | Evidence storage、naming 與 cleanup 可執行。 | `RND-CLOSE-004` | Blocked |
| `RND-CGATE-007` | Build/Project/Package 權限已明確分離。 | `RND-CLOSE-006` | Blocked |
| `RND-CGATE-008` | Runtime execution 仍由後續獨立授權管理。 | `RND-CLOSE-006` | Deferred |

Gate Plan Status 只能使用 `Specified`、`Partially specified`、`Blocked`、`Deferred`；本文件不得使用 `Satisfied`。

## 12. Workload and Evidence Closure Plan

| Capability | Existing definition | Remaining gap | Required tool/asset | Mutation required | Closure Action |
|---|---|---|---|---|---|
| Synthetic canvas definition | Workload contract listed in `RESEARCH-TECH-RENDER-003` | Logical/pixel dimensions and manifest not frozen | Future workload manifest | Future only | `RND-CLOSE-003` |
| Vector shapes | Candidate comparison scope exists | Exact shape list and expected output not recorded | Future synthetic vector definition | Future only | `RND-CLOSE-003` |
| Hit-test sequence | Pointer/input requirement listed | Deterministic sequence and coordinate cases missing | Future input manifest | Future only | `RND-CLOSE-003` |
| Mixed-language text | Text/fallback requirement listed | Fixed strings and font provenance missing | Future text asset | Future only | `RND-CLOSE-003` |
| Font fallback assets | Fallback is a required test concern | Host font availability not evidenced | Future font manifest | Future only | `RND-CLOSE-003` |
| Mosaic reference algorithm | Mosaic is a required workload item | Deterministic algorithm and boundary rules missing | Future algorithm description | Future only | `RND-CLOSE-003` |
| Alpha/pixel format | Alpha inspection is a fidelity requirement | Channel order and alpha mode not fixed | Future format manifest | Future only | `RND-CLOSE-003` |
| PNG export | PNG is a required evidence type | Encoder/decoder and metadata policy missing | Future PNG method | Future only | `RND-CLOSE-004` |
| Reference image generation | Reference output is required | Reproducible authoritative method missing | Future reference method | Future only | `RND-CLOSE-004` |
| Pixel-difference calculation | Comparison requirement is known | Threshold/report format is not defined | Future comparison method | Future only | `RND-CLOSE-004` |
| Coordinate comparison | Logical-to-pixel comparison is required | Same/mixed DPI evidence path missing | Future environment/evidence manifest | Future only | `RND-CLOSE-002`, `004` |
| Failure reproduction | Failure evidence is required | Stable manifest and replay boundary missing | Future failure manifest | Future only | `RND-CLOSE-003`, `004` |
| Cleanup confirmation | Cleanup is a shared safety requirement | Before/after scope record missing | Future cleanup checklist | Future only | `RND-CLOSE-004` |

不得建立實際資產、reference image、PNG、result directory 或 measurement artifact。

## 13. Deferred Scope Register

| Deferred item | Target phase | Deferred reason | Reactivation condition | Affected pairs | Affected spikes | Blocks Phase R1 |
|---|---|---|---|---|---|---|
| Full multi-monitor matrix | R2 | R1 只需 approved single-monitor baseline | R1 baseline is accepted and R2 authorized | All coordinate pairs | `003`, `005`, `010` | No |
| Heterogeneous DPI full coverage | R2 | R1 不等待完整 topology matrix | Same-DPI baseline and R2 gate available | All coordinate pairs | `005`, `010` | No |
| HDR branch | R3 | Color/HDR is not minimum vector correctness | R3 color observation method exists | All fidelity pairs | `003`, `006`, `008`, `010` | No |
| Software fallback | R3 | Resource behavior is not R1 minimum | Acceleration observation method exists | Interop/resource pairs | `002`, `004`, `007`, `009` | No |
| CPU/GPU/memory observations | R3 | No R1 resource claim is made | R3 measurement policy and authorization | `006`, `007`, `009` | `004`, `007`, `009` | No |
| Color-space deep verification | R3 | No color threshold is invented in R1 | Color metadata method and evidence path | All fidelity pairs | `010` | No |
| Phase R2 fidelity completeness | R2 | R1 gate is correctness-focused | Reference/PNG/pixel methods are authorized | All eligible pairs | `003`, `006`, `008`, `010` | No |
| Phase R3 interop/resource completeness | R3 | Requires pair and environment evidence | R2 closure plus R3 authorization | `006`, `007`, `009`, `010` | `004`, `007`, `009` | No |

Deferred 不代表永久移出 TD-002 研究；每項都必須保留 reactivation condition 與 traceability。

## 14. Full Impact Matrix

### 14.1 Coverage Summary

本矩陣完整涵蓋：

- 21 個 `RND-PREQ`：`001` 至 `021`。
- 9 個 `RND-BLOCK`：`001` 至 `009`。
- 10 個 `RND-PAIR`：`001` 至 `010`。
- 10 個 `RND-SPIKE`：`001` 至 `010`。
- 6 個 `RND-BA`：`001` 至 `006`。
- 6 個 `RND-CLOSE`：`001` 至 `006`。

### 14.2 Prerequisite Impact

| Source item | Phase | Closure Action | Required evidence | Current status | Target recommendation |
|---|---|---|---|---|---|
| `RND-PREQ-001` | R1 | `RND-CLOSE-001` | Host version provenance | Blocked | Blocked |
| `RND-PREQ-002` | R1 | `RND-CLOSE-001` | SDK/runtime baseline | Blocked | Blocked |
| `RND-PREQ-003` | R1 | `RND-CLOSE-001` | Native path definition | Partially resolved | Partially resolved |
| `RND-PREQ-004` | R2/R3 | `RND-CLOSE-005` | Direct2D interop record | Blocked | Deferred |
| `RND-PREQ-005` | R2 | `RND-CLOSE-005` | Win2D compatibility | Blocked | Deferred |
| `RND-PREQ-006` | R2/R3 | `RND-CLOSE-005` | SkiaSharp native asset record | Blocked | Deferred |
| `RND-PREQ-007` | R2/R3 | `RND-CLOSE-005` | Hybrid ownership boundary | Partially resolved | Deferred |
| `RND-PREQ-008` | R1 | `RND-CLOSE-003` | Workload manifest | Partially resolved | Blocked |
| `RND-PREQ-009` | R2 | `RND-CLOSE-004` | Reference generation method | Blocked | Deferred |
| `RND-PREQ-010` | R2 | `RND-CLOSE-004` | PNG/pixel method | Blocked | Deferred |
| `RND-PREQ-011` | R2 | `RND-CLOSE-003` | Font/fallback assets | Blocked | Deferred |
| `RND-PREQ-012` | R2 | `RND-CLOSE-003` | Mosaic algorithm | Blocked | Deferred |
| `RND-PREQ-013` | R1 | `RND-CLOSE-002` | DPI/topology evidence | Blocked | Blocked |
| `RND-PREQ-014` | R2 | `RND-CLOSE-003` | Alpha/pixel format | Blocked | Deferred |
| `RND-PREQ-015` | R3 | `RND-CLOSE-004` | Color/HDR method | Deferred | Deferred |
| `RND-PREQ-016` | R3 | `RND-CLOSE-004` | CPU/GPU/memory method | Deferred | Deferred |
| `RND-PREQ-017` | R1 | `RND-CLOSE-004` | Evidence root/manifest | Partially resolved | Blocked |
| `RND-PREQ-018` | R1 | `RND-CLOSE-004` | Cleanup acceptance | Partially resolved | Blocked |
| `RND-PREQ-019` | R1/R2 | `RND-CLOSE-005` | Package authorization/provenance | Blocked | Blocked |
| `RND-PREQ-020` | R1 | `RND-CLOSE-006` | Build authorization | Blocked | Blocked |
| `RND-PREQ-021` | R1/R2/R3 | `RND-CLOSE-006` | Runtime authorization record | Blocked | Blocked |

### 14.3 Blocker Impact

| Source item | Phase | Closure Action | Required evidence | Current status | Target recommendation |
|---|---|---|---|---|---|
| `RND-BLOCK-001` | R1 | `RND-CLOSE-001` | Host/SDK/path provenance | Open | Blocked |
| `RND-BLOCK-002` | R1 | `RND-CLOSE-002` | Display/DPI baseline | Open | Blocked |
| `RND-BLOCK-003` | R1 | `RND-CLOSE-003` | Synthetic isolation | Open | Blocked |
| `RND-BLOCK-004` | R1 | `RND-CLOSE-004` | Evidence storage policy | Open | Blocked |
| `RND-BLOCK-005` | R1 | `RND-CLOSE-004` | Cleanup acceptance | Open | Blocked |
| `RND-BLOCK-006` | R1 | `RND-CLOSE-006` | Exact authorization | Open | Blocked |
| `RND-BLOCK-007` | R1/R2 | `RND-CLOSE-005` | Candidate dependency provenance | Open | Blocked |
| `RND-BLOCK-008` | R2 | `RND-CLOSE-004` | Fidelity comparison method | Open | Deferred |
| `RND-BLOCK-009` | R3 | `RND-CLOSE-004` | Resource/color observation | Open | Deferred |

### 14.4 Pair Impact

| Source item | Phase | Closure Action | Required evidence | Current status | Target recommendation |
|---|---|---|---|---|---|
| `RND-PAIR-001` | R1 | `RND-CLOSE-001`..`006` | WinUI native R1 evidence | Blocked | Blocked |
| `RND-PAIR-002` | R1 | `RND-CLOSE-001`, `006` | WPF host evidence | Not evaluated | Not evaluated |
| `RND-PAIR-003` | R2 | `RND-CLOSE-001`, `005`, `006` | Host boundary evidence | Not evaluated | Not evaluated |
| `RND-PAIR-004` | R1 | `RND-CLOSE-001`..`006` | WPF native R1 evidence | Blocked | Blocked |
| `RND-PAIR-005` | R2/R3 | `RND-CLOSE-001`, `005`, `006` | Direct2D interop evidence | Not evaluated | Deferred |
| `RND-PAIR-006` | R2/R3 | `RND-CLOSE-001`, `005`, `006` | WPF interop evidence | Blocked | Deferred |
| `RND-PAIR-007` | R2 | `RND-CLOSE-003`..`006` | Win2D package/workload evidence | Blocked | Deferred |
| `RND-PAIR-008` | N/A | None | Plan exclusion evidence | Excluded with evidence | Excluded with evidence |
| `RND-PAIR-009` | R2/R3 | `RND-CLOSE-003`..`006` | SkiaSharp WinUI evidence | Blocked | Deferred |
| `RND-PAIR-010` | R2/R3 | `RND-CLOSE-001`, `003`..`006` | SkiaSharp WPF evidence | Blocked | Deferred |

### 14.5 Spike Impact

| Source item | Phase | Closure Action | Required evidence | Current status | Target recommendation |
|---|---|---|---|---|---|
| `RND-SPIKE-001` | R1 | `RND-CLOSE-001`, `003`, `004`, `006` | WinUI native baseline | Blocked | Blocked |
| `RND-SPIKE-002` | R1 | `RND-CLOSE-001`, `003`, `004`, `006` | WPF native baseline | Blocked | Blocked |
| `RND-SPIKE-003` | R1 | `RND-CLOSE-001`..`004`, `006` | Vector/hit-test evidence | Blocked | Blocked |
| `RND-SPIKE-004` | R2/R3 | `RND-CLOSE-001`, `005`, `006` | Interop/resource evidence | Blocked | Deferred |
| `RND-SPIKE-005` | R1/R2 | `RND-CLOSE-002`..`004`, `006` | DPI coordinate evidence | Blocked | Blocked |
| `RND-SPIKE-006` | R2 | `RND-CLOSE-003`..`006` | Win2D package evidence | Blocked | Deferred |
| `RND-SPIKE-007` | R2/R3 | `RND-CLOSE-002`..`006` | Hybrid/resource evidence | Blocked | Deferred |
| `RND-SPIKE-008` | R2 | `RND-CLOSE-003`..`006` | SkiaSharp output evidence | Blocked | Deferred |
| `RND-SPIKE-009` | R3 | `RND-CLOSE-005`, `006` | Resource observation | Blocked | Deferred |
| `RND-SPIKE-010` | R2 | `RND-CLOSE-002`..`006` | Full fidelity evidence | Blocked | Deferred |

### 14.6 Blocking Action and Closure Impact

| Source item | Phase | Closure Action | Required evidence | Current status | Target recommendation |
|---|---|---|---|---|---|
| `RND-BA-001` | R1 | `RND-CLOSE-001` | Host/SDK provenance | Blocked | Blocked |
| `RND-BA-002` | R1 | `RND-CLOSE-002` | DPI/topology baseline | Blocked | Blocked |
| `RND-BA-003` | R1 | `RND-CLOSE-003` | Workload isolation | Blocked | Blocked |
| `RND-BA-004` | R1 | `RND-CLOSE-004` | Storage/cleanup acceptance | Blocked | Blocked |
| `RND-BA-005` | R1 | `RND-CLOSE-005` | Candidate dependency baseline | Blocked | Blocked |
| `RND-BA-006` | R1 | `RND-CLOSE-006` | Exact authorization | Blocked | Blocked |
| `RND-CLOSE-001` | R1 | Self | Future host provenance | Blocked | Blocked |
| `RND-CLOSE-002` | R1 | Self | Future DPI evidence | Blocked | Blocked |
| `RND-CLOSE-003` | R1 | Self | Future workload evidence | Blocked | Blocked |
| `RND-CLOSE-004` | R1 | Self | Future evidence boundary | Blocked | Blocked |
| `RND-CLOSE-005` | R1/R2 | Self | Future candidate provenance | Blocked | Blocked |
| `RND-CLOSE-006` | R1 | Self | Future authorization record | Blocked | Blocked |

本節只能提出 recommendation，不得修改上游狀態。

## 15. Recommended Closure Order

固定順序：

1. 確認 Shared UI authority 與可重用範圍。
2. 固定 Runtime Spike 專用候選版本與 package identity。
3. 固定 Candidate–Host pair 的 build/interop requirements。
4. 固定 synthetic workload 與 evidence method。
5. 固定 Project、Restore、Build 的最小授權範圍。
6. 執行未來獨立的 Closure Execution Authorization Review。
7. 執行 Closure Action 並建立 Evidence。
8. 重新評估 Rendering Runtime Spike readiness。

本文件只完成第 1 至第 5 項的規劃，不執行。

## 16. Authorization Boundary

| Operation | Current authorization | Execution permitted |
|---|---|---|
| Official version inspection | Not granted | No |
| Local read-only inspection | Not granted | No |
| Package acquisition | Not granted | No |
| Tool/SDK installation | Not granted | No |
| Project creation | Not granted | No |
| Restore | Not granted | No |
| Build | Not granted | No |
| Evidence capture | Not granted | No |
| Runtime execution | Not granted | No |

所有 `Execution permitted` 必須為 `No`。本文件不把研究文件的建立權限擴大成任何環境或 runtime 操作權限。

## 17. Closure Plan Status

| Field | Value |
|---|---|
| Closure Plan Status | Closure plan complete |
| Readiness to request rendering prerequisite closure execution authorization | Not ready |
| Build Verification | Not performed |
| Runtime Verification | Not performed |
| Runtime Spike Execution Authorized | No |
| Rendering Decision | Not made |

Closure plan complete 只表示本規劃文件已列出必要欄位與邊界，不表示任何 closure action 已執行。

允許的 readiness request 值為：

- `Ready to request rendering prerequisite closure execution authorization`
- `Conditionally ready to request rendering prerequisite closure execution authorization`
- `Not ready`

目前只能使用 `Not ready`。

## 18. Traceability

### 18.1 Closure Chain

```text
RND-PREQ / RND-BLOCK
  -> RND-BA
  -> RND-CLOSE
  -> Required authorization
  -> Future closure evidence
  -> Candidate–Host readiness
  -> RND Spike readiness reassessment
  -> Future TD-002 decision
```

### 18.2 References

- [`docs/Research/Technology/10-rendering-technology-feasibility.md`](10-rendering-technology-feasibility.md)
- [`docs/Research/Technology/11-rendering-technology-runtime-spike-plan.md`](11-rendering-technology-runtime-spike-plan.md)
- [`docs/Research/Technology/12-rendering-technology-runtime-spike-execution-readiness.md`](12-rendering-technology-runtime-spike-execution-readiness.md)
- [`docs/Research/Technology/07-ui-framework-phase1-readiness-reassessment.md`](07-ui-framework-phase1-readiness-reassessment.md)
- [`docs/Research/Technology/08-ui-framework-phase1-execution-enablement-specification.md`](08-ui-framework-phase1-execution-enablement-specification.md)
- [`docs/Research/Technology/09-ui-framework-phase1-enablement-execution-authorization-request.md`](09-ui-framework-phase1-enablement-execution-authorization-request.md)
- [`Architecture/adr/ADR-0002-ui-framework-selection.md`](../../../Architecture/adr/ADR-0002-ui-framework-selection.md)
- [`Architecture/TECHNOLOGY-DECISION-ROADMAP.md`](../../../Architecture/TECHNOLOGY-DECISION-ROADMAP.md)

本文件不得修改上述來源文件；來源狀態只能在各自的 review 或 authorization 流程中變更。

## Completion Boundary

本輪完成條件：

- 只建立 `13-rendering-technology-prerequisite-closure-plan.md`。
- 不修改 README、索引、CHANGELOG、TODO 或其他文件。
- 建立正好 6 個 `RND-CLOSE`，一對一對應 6 個 `RND-BA`。
- 不新增、刪除、合併或拆分 Blocking Action。
- 完整覆蓋 21 個 prerequisite、9 個 blocker、10 個 pair、10 個 spike。
- 明確重用 UI Research Line 的共用權限。
- 建立 Phase R1 Minimum Closure Gate。
- 建立 Deferred Scope Register。
- 所有操作的 `Current authorization = Not granted`。
- 所有 `Execution permitted = No`。
- 不執行唯讀查核、下載、安裝、Restore、Build、Run 或 Runtime Spike。
- 不建立 Project、Prototype、Result、Source Code 或實際 Evidence。
- 不修改 `ADR-0002` 或建立 TD-002 ADR。
- 未開始任何截圖功能或截圖相關 coding。
- `git diff --check` 必須通過；本文件完成後立即停止。
