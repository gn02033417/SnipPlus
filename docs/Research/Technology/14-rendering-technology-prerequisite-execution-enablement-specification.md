# Rendering Technology Prerequisite Execution Enablement Specification

本文件將 `RND-CLOSE-001` 至 `RND-CLOSE-006` 轉成可供未來人工審核的 execution enablement specification。它只定義精確範圍、依賴、風險、證據義務與 rollback boundary，不執行任何 enablement item。

## Document Control

| Field | Value |
|---|---|
| Document ID | `RESEARCH-TECH-RENDER-005` |
| Title | Rendering Technology Prerequisite Execution Enablement Specification |
| Status | Draft |
| Research Type | Execution Enablement Specification |
| Execution Status | Not started |
| Parent Closure Plan | `RESEARCH-TECH-RENDER-004` |
| Parent Readiness Record | `RESEARCH-TECH-RENDER-003` |
| Technology Decision | `TD-002 Rendering Technology` |
| Host Framework Decision | Unresolved |
| Build Verification | Not performed |
| Runtime Verification | Not performed |
| Closure Execution Authorized | No |
| Runtime Spike Execution Authorized | No |
| Rendering Decision | Not made |
| Owner | TBD |
| Last reviewed | Not reviewed |
| Version | 0.1 |
| Preparation date | 2026-07-26 |
| Normative References | `RESEARCH-TECH-RENDER-004`, existing UI authorization boundary |
| Informative References | `RESEARCH-TECH-RENDER-001` 至 `RESEARCH-TECH-RENDER-003` |
| Supersedes | None |
| Superseded by | None |

## 1. 任務目的

本文件只回答：

> `RND-CLOSE-001` 至 `RND-CLOSE-006` 在提交人工執行授權前，必須具備哪些精確版本、操作範圍、Repository 邊界、系統影響、證據義務及 rollback 條件？

這是 Execution Enablement Specification。

本文件不是：

- Closure Execution Record。
- Authorization Request。
- Runtime Spike Authorization。
- Runtime Spike。
- Rendering Decision。
- TD-002 ADR。

## 2. Scope

本文件只處理：

- `RND-CLOSE-001` 至 `RND-CLOSE-006`。
- `RND-BA-001` 至 `RND-BA-006`。
- Phase R1 所需的 Host/candidate enablement。
- Candidate package 與 native dependency identity。
- Experimental Project/Restore/Build 邊界。
- Synthetic workload 靜態準備。
- Evidence method 與 storage boundary。
- Shared UI authority inheritance。
- Rendering-specific 人工授權需求。

Phase R2/R3 的完整驗證只能作為後續依賴，不加入本輪 execution scope。

## 3. Non-goals

本任務不得：

- 執行任何 Enablement Item。
- 執行本機唯讀盤點。
- 查詢或改變 NuGet cache。
- 下載或安裝 SDK、Runtime、Package 或工具。
- 建立 Project、Solution、Prototype 或 Result directory。
- 執行 Restore、Build、Run、Publish 或 Runtime Spike。
- 建立 PNG、Reference Image、Pixel Difference、Log 或 Measurement Artifact。
- 申請或批准 Runtime execution。
- 修改 UI Research Line。
- 修改 `RESEARCH-TECH-RENDER-001` 至 `RESEARCH-TECH-RENDER-004`。
- 修改 `ADR-0002`。
- 建立 TD-002 ADR。
- 選擇 Rendering Technology 或正式產品版本。

## 4. Status Vocabulary

### 4.1 Enablement Item Status

只能使用：

`Specified`、`Partially specified`、`Blocked`、`Deferred`、`Not applicable`

不得使用：

`Completed`、`Resolved`、`Approved`、`Authorized`、`Executed`

### 4.2 Specification Evidence Status

只能使用：

`Confirmed by official source`、`Accepted from parent evidence`、`Partially specified`、`Unknown`、`Conflicting`、`Not applicable`

### 4.3 Execution Permission

只能使用：

`No`

本文件所有 operation 的 execution permission 均為 `No`。即使 specification complete，也不表示 operation 已獲准或已執行。

## 5. Enablement Item Binding

建立一對一綁定：

| Enablement Item | Closure Action | Blocking Action |
|---|---|---|
| `RND-ENABLE-001` | `RND-CLOSE-001` | `RND-BA-001` |
| `RND-ENABLE-002` | `RND-CLOSE-002` | `RND-BA-002` |
| `RND-ENABLE-003` | `RND-CLOSE-003` | `RND-BA-003` |
| `RND-ENABLE-004` | `RND-CLOSE-004` | `RND-BA-004` |
| `RND-ENABLE-005` | `RND-CLOSE-005` | `RND-BA-005` |
| `RND-ENABLE-006` | `RND-CLOSE-006` | `RND-BA-006` |

規則：

- 不得重新編號、合併或拆分。
- 不得增加第七個 Phase R1 Blocking Action。
- 上游內容不足時建立 `RND-ENABLE-GAP-xxx`。
- 不得直接修改上游文件。

## 6. 每個 Enablement Item 的固定欄位

每個 `RND-ENABLE` 必須包含下列欄位。欄位值只描述 future enablement boundary；本文件不執行其中任何 operation：

- Enablement Item ID
- Source Closure Action
- Source Blocking Action
- Related Closure Gates
- Related prerequisites
- Related blockers
- Related Candidate–Host pairs
- Related Spikes
- Dependency ownership
- Shared UI source IDs
- Existing specification evidence
- Current unresolved specification
- Required final evidence
- Proposed enablement operation
- Operation classifications
- Exact scope
- Explicit exclusions
- Official-source lookup required
- Local read-only inspection required
- Network access required
- Package acquisition required
- Installation required
- Repository mutation required
- Experimental project required
- Restore required
- Build required
- Runtime execution required
- Evidence capture required
- System configuration mutation required
- Administrator privilege required
- Human authorization required
- Expected files or directories
- Expected package/cache changes
- Expected machine changes
- Risk classification
- Failure impact
- Stop condition
- Rollback/cleanup requirement
- Success condition
- Result artifact obligation
- Resulting prerequisite recommendation
- Resulting blocker recommendation
- Resulting pair recommendation
- Phase R1 effect
- Owner
- Status
- Open questions

## 6.1 RND-ENABLE-001 — Shared Host Build Path

| Field | Specification |
|---|---|
| Enablement Item ID | `RND-ENABLE-001` |
| Source Closure Action | `RND-CLOSE-001` |
| Source Blocking Action | `RND-BA-001` |
| Related Closure Gates | `RND-CGATE-001`, `002`, `003` |
| Related prerequisites | `RND-PREQ-001`, `002`, `003` |
| Related blockers | `RND-BLOCK-001` |
| Related Candidate–Host pairs | `RND-PAIR-001`, `002`, `004` |
| Related Spikes | `RND-SPIKE-001`, `002`, `003` |
| Dependency ownership | Shared UI research |
| Shared UI source IDs | `BA-001`, `BA-002`, `UI-AUTH-001`, `UI-AUTH-002` |
| Existing specification evidence | Parent closure plan and UI research boundary；沒有 execution evidence。 |
| Current unresolved specification | Exact host framework、SDK/runtime version、build path provenance 與 first authorized host。 |
| Required final evidence | Exact version record、host path、isolated boundary、build provenance 與 ownership manifest。 |
| Proposed enablement operation | Future host-path enablement review；只在 approval 後進行必要的 isolated project/build preparation。 |
| Operation classifications | Repository documentation mutation；Experimental project creation；Build execution。 |
| Exact scope | One approved WinUI 3 path and one approved WPF path needed for R1 comparison。 |
| Explicit exclusions | Product source tree、runtime execution、formal architecture component、TD-002 decision。 |
| Official-source lookup required | Yes, future only。 |
| Local read-only inspection required | Future only；not in this document。 |
| Network access required | TBD；must be approved separately。 |
| Package acquisition required | Host-specific only if separately approved。 |
| Installation required | Not granted。 |
| Repository mutation required | Future isolated specification/evidence manifest only。 |
| Experimental project required | Future isolated project may be required。 |
| Restore required | Future only；permission No。 |
| Build required | Future only；permission No。 |
| Runtime execution required | No for enablement specification。 |
| Evidence capture required | Host/build provenance only in future；no current artifact。 |
| System configuration mutation required | No planned。 |
| Administrator privilege required | TBD; must be explicit if proposed。 |
| Human authorization required | Yes; reuse UI host/build authority。 |
| Expected files or directories | Future approved isolated host manifest；none created now。 |
| Expected package/cache changes | None now；future package effect must be enumerated。 |
| Expected machine changes | None now。 |
| Risk classification | R2: project/build boundary and host dependency。 |
| Failure impact | R1 host baseline remains blocked; no candidate comparison may start。 |
| Stop condition | Host/version/path or UI authority is ambiguous。 |
| Rollback/cleanup requirement | Future temporary project/build output must be removed within approved boundary。 |
| Success condition | Host path and version are explicit, isolated and traceable。 |
| Result artifact obligation | Future provenance manifest; no current result artifact。 |
| Resulting prerequisite recommendation | `RND-PREQ-001`..`003` -> Partially specified or later evidence-backed state。 |
| Resulting blocker recommendation | `RND-BLOCK-001` remains Blocked without direct evidence。 |
| Resulting pair recommendation | Affected pairs remain Blocked until pair evidence exists。 |
| Phase R1 effect | Direct R1 blocker。 |
| Owner | TBD |
| Status | Blocked |
| Open questions | Which WinUI 3/WPF baseline is allowed first? |

## 6.2 RND-ENABLE-002 — Display and DPI Baseline

| Field | Specification |
|---|---|
| Enablement Item ID | `RND-ENABLE-002` |
| Source Closure Action | `RND-CLOSE-002` |
| Source Blocking Action | `RND-BA-002` |
| Related Closure Gates | `RND-CGATE-005`, `006` |
| Related prerequisites | `RND-PREQ-013` |
| Related blockers | `RND-BLOCK-002` |
| Related Candidate–Host pairs | `RND-PAIR-001`, `004`, `007`, `009`, `010` |
| Related Spikes | `RND-SPIKE-003`, `005`, `006`, `008`, `010` |
| Dependency ownership | Environment |
| Shared UI source IDs | `BA-003`, `BA-004` |
| Existing specification evidence | UI research identifies display/DPI dependency；沒有 actual environment record。 |
| Current unresolved specification | Minimum monitor topology、same-DPI scale、logical-to-pixel coordinate protocol。 |
| Required final evidence | Approved environment manifest、single-monitor/same-DPI record、coordinate cases。 |
| Proposed enablement operation | Future baseline-record preparation without changing DPI/HDR/system configuration。 |
| Operation classifications | Local read-only inspection；Evidence capture without runtime UI。 |
| Exact scope | R1 single-monitor and same-DPI baseline only。 |
| Explicit exclusions | Full multi-monitor、heterogeneous-DPI matrix、HDR mutation、runtime rendering output。 |
| Official-source lookup required | No planned。 |
| Local read-only inspection required | Future only；not in this document。 |
| Network access required | No planned。 |
| Package acquisition required | No。 |
| Installation required | No。 |
| Repository mutation required | Future environment/evidence manifest only。 |
| Experimental project required | No for environment specification。 |
| Restore required | No。 |
| Build required | No。 |
| Runtime execution required | No for baseline specification。 |
| Evidence capture required | Future environment record。 |
| System configuration mutation required | No；configuration changes are excluded。 |
| Administrator privilege required | No planned。 |
| Human authorization required | Yes; reuse UI environment authority。 |
| Expected files or directories | Future environment manifest；none created now。 |
| Expected package/cache changes | None。 |
| Expected machine changes | None planned。 |
| Risk classification | R1: environment evidence boundary。 |
| Failure impact | Coordinate/fidelity claims remain blocked。 |
| Stop condition | Baseline requires changing user/system display settings。 |
| Rollback/cleanup requirement | Future temporary records only；restore any explicitly approved temporary state。 |
| Success condition | Same-DPI baseline can be reproduced without system mutation。 |
| Result artifact obligation | Future environment record; no current artifact。 |
| Resulting prerequisite recommendation | `RND-PREQ-013` remains Blocked until evidence。 |
| Resulting blocker recommendation | `RND-BLOCK-002` remains Blocked without direct baseline。 |
| Resulting pair recommendation | Coordinate-dependent pairs remain Blocked。 |
| Phase R1 effect | Direct R1 blocker。 |
| Owner | TBD |
| Status | Blocked |
| Open questions | Which scale factor and display topology define the R1 baseline? |

## 6.3 RND-ENABLE-003 — Synthetic Workload

| Field | Specification |
|---|---|
| Enablement Item ID | `RND-ENABLE-003` |
| Source Closure Action | `RND-CLOSE-003` |
| Source Blocking Action | `RND-BA-003` |
| Related Closure Gates | `RND-CGATE-005` |
| Related prerequisites | `RND-PREQ-008`, `011`, `012`, `013`, `014` |
| Related blockers | `RND-BLOCK-003` |
| Related Candidate–Host pairs | All non-excluded pairs |
| Related Spikes | `RND-SPIKE-001`..`010` |
| Dependency ownership | Rendering-specific |
| Shared UI source IDs | `BA-005`, `UI-ENABLE-005` |
| Existing specification evidence | Parent readiness workload contract；沒有實際 workload asset。 |
| Current unresolved specification | Fixed constants、input sequence、font/mosaic provenance 與 isolation manifest。 |
| Required final evidence | Workload manifest、static asset list、input sequence、isolation record。 |
| Proposed enablement operation | Future static workload specification and isolated asset preparation。 |
| Operation classifications | Repository documentation mutation；Repository experimental asset creation。 |
| Exact scope | Canvas constants、vectors、hit-test、text、alpha、mosaic 與 scale cases。 |
| Explicit exclusions | Product data、user content、runtime output、PNG、measurement、formal UI feature。 |
| Official-source lookup required | No planned。 |
| Local read-only inspection required | No current execution。 |
| Network access required | No planned。 |
| Package acquisition required | No for workload definition。 |
| Installation required | No。 |
| Repository mutation required | Future isolated workload manifest/asset only。 |
| Experimental project required | Future spike may require one; not this document。 |
| Restore required | No for static workload specification。 |
| Build required | No for static workload specification。 |
| Runtime execution required | No。 |
| Evidence capture required | Future workload manifest only。 |
| System configuration mutation required | No。 |
| Administrator privilege required | No planned。 |
| Human authorization required | Yes; workload scope and isolation must be approved。 |
| Expected files or directories | Future approved workload boundary；none created now。 |
| Expected package/cache changes | None。 |
| Expected machine changes | None。 |
| Risk classification | R1: source/data isolation and reproducibility。 |
| Failure impact | All candidate comparisons can be confounded by product or user data。 |
| Stop condition | Workload depends on non-reproducible state or product source。 |
| Rollback/cleanup requirement | Remove future temporary assets from approved isolated boundary。 |
| Success condition | Same static workload and input sequence can be consumed by each eligible pair。 |
| Result artifact obligation | Future workload manifest; no current asset。 |
| Resulting prerequisite recommendation | `RND-PREQ-008`, `011`..`014` remain Partially specified or Blocked。 |
| Resulting blocker recommendation | `RND-BLOCK-003` remains Blocked until isolation evidence。 |
| Resulting pair recommendation | All affected pairs remain Blocked。 |
| Phase R1 effect | Direct R1 blocker。 |
| Owner | TBD |
| Status | Blocked |
| Open questions | Which fixed text, font and mosaic inputs are permitted? |

## 6.4 RND-ENABLE-004 — Evidence Storage and Method

| Field | Specification |
|---|---|
| Enablement Item ID | `RND-ENABLE-004` |
| Source Closure Action | `RND-CLOSE-004` |
| Source Blocking Action | `RND-BA-004` |
| Related Closure Gates | `RND-CGATE-006` |
| Related prerequisites | `RND-PREQ-009`, `010`, `017`, `018` |
| Related blockers | `RND-BLOCK-004`, `005`, `008` |
| Related Candidate–Host pairs | All non-excluded pairs |
| Related Spikes | All spikes |
| Dependency ownership | Evidence |
| Shared UI source IDs | `BA-006`, `BA-007` |
| Existing specification evidence | Parent closure plan defines evidence/storage requirement；沒有 evidence root。 |
| Current unresolved specification | Root、naming、manifest、retention、cleanup、comparison method。 |
| Required final evidence | Approved storage policy、manifest format、cleanup record、method provenance。 |
| Proposed enablement operation | Future evidence method and storage-boundary specification。 |
| Operation classifications | Repository documentation mutation；Evidence capture only in a later authorized phase。 |
| Exact scope | Non-runtime environment/build/dependency records and future evidence governance。 |
| Explicit exclusions | Actual output、PNG、reference image、pixel difference、measurement、runtime capture。 |
| Official-source lookup required | Future method research only。 |
| Local read-only inspection required | Future only；not in this document。 |
| Network access required | Not granted。 |
| Package acquisition required | Not granted。 |
| Installation required | No planned。 |
| Repository mutation required | Future policy/manifest only。 |
| Experimental project required | No for policy specification。 |
| Restore required | No。 |
| Build required | No。 |
| Runtime execution required | No。 |
| Evidence capture required | Future policy record; actual evidence excluded。 |
| System configuration mutation required | No。 |
| Administrator privilege required | No planned。 |
| Human authorization required | Yes; existing evidence/safety authority plus future method scope。 |
| Expected files or directories | Future approved evidence root; none created now。 |
| Expected package/cache changes | None。 |
| Expected machine changes | None。 |
| Risk classification | R1: evidence governance and cleanup。 |
| Failure impact | Evidence cannot be attributed, compared or safely removed。 |
| Stop condition | Storage path or cleanup effect is ambiguous。 |
| Rollback/cleanup requirement | Future evidence must have manifest-based cleanup within approved scope。 |
| Success condition | Evidence method and storage boundary are reviewable before any capture。 |
| Result artifact obligation | Future policy/manifest; no current result。 |
| Resulting prerequisite recommendation | `RND-PREQ-009`, `010`, `017`, `018` remain Partially specified or Blocked。 |
| Resulting blocker recommendation | `RND-BLOCK-004`, `005`, `008` remain Open/Blocked as applicable。 |
| Resulting pair recommendation | Pair readiness cannot advance on visual assumption alone。 |
| Phase R1 effect | Direct R1 blocker for evidence governance。 |
| Owner | TBD |
| Status | Blocked |
| Open questions | Which root, retention period and comparison method are accepted? |

## 6.5 RND-ENABLE-005 — Candidate Package and Native Dependency

| Field | Specification |
|---|---|
| Enablement Item ID | `RND-ENABLE-005` |
| Source Closure Action | `RND-CLOSE-005` |
| Source Blocking Action | `RND-BA-005` |
| Related Closure Gates | `RND-CGATE-003`, `004`, `007` |
| Related prerequisites | `RND-PREQ-004`, `005`, `006`, `007`, `019` |
| Related blockers | `RND-BLOCK-006`, `007` |
| Related Candidate–Host pairs | `RND-PAIR-001`, `004`, `006`, `007`, `009`, `010` |
| Related Spikes | `RND-SPIKE-001`..`009` |
| Dependency ownership | Rendering-specific |
| Shared UI source IDs | `UI-AUTH-003`, `UI-AUTH-004`, `UI-AUTH-005` |
| Existing specification evidence | Candidate list and feasibility rationale；沒有 exact local package/native evidence。 |
| Current unresolved specification | Exact package IDs/versions、source、native assets、host compatibility、cache effect。 |
| Required final evidence | Candidate identity baseline、native asset manifest、host compatibility、package provenance。 |
| Proposed enablement operation | Future package identity and dependency enablement review；不在本文件取得 package。 |
| Operation classifications | Official-source research；Package acquisition；Package Restore。 |
| Exact scope | Only candidate-specific dependency identity for approved R1/R2 pair。 |
| Explicit exclusions | Broad package upgrade、product dependency、formal version selection、runtime execution。 |
| Official-source lookup required | Yes, future source review only。 |
| Local read-only inspection required | Future only；not in this document。 |
| Network access required | TBD; explicit permission required。 |
| Package acquisition required | Possibly; exact Package ID/version must be approved。 |
| Installation required | Possibly; not granted。 |
| Repository mutation required | Future dependency/provenance manifest only。 |
| Experimental project required | Future pair verification may require one。 |
| Restore required | Future only; permission No。 |
| Build required | Future only; permission No。 |
| Runtime execution required | No for enablement specification。 |
| Evidence capture required | Future dependency inventory; no current artifact。 |
| System configuration mutation required | No planned。 |
| Administrator privilege required | TBD; must be explicit。 |
| Human authorization required | Yes; package scope is rendering-specific and separate。 |
| Expected files or directories | Future dependency manifest and approved temporary cache/project boundary。 |
| Expected package/cache changes | Future package/cache impact must be enumerated before approval。 |
| Expected machine changes | None now。 |
| Risk classification | R2/R3: package and native dependency side effects。 |
| Failure impact | Candidate-host pair cannot be attributed or safely compared。 |
| Stop condition | Package identity/version/native asset is TBD at execution time。 |
| Rollback/cleanup requirement | Future cache/project mutation must be reversible or explicitly accepted。 |
| Success condition | Exact dependency identity and host boundary are documented without ambiguity。 |
| Result artifact obligation | Future dependency manifest; no current package artifact。 |
| Resulting prerequisite recommendation | Candidate-specific prerequisites remain Blocked until evidence。 |
| Resulting blocker recommendation | `RND-BLOCK-007` remains Blocked until dependency provenance。 |
| Resulting pair recommendation | Unknown/unevaluated pairs remain unchanged; no selection by convenience。 |
| Phase R1 effect | Direct R1 blocker for non-native comparison scope。 |
| Owner | TBD |
| Status | Blocked |
| Open questions | Which exact candidate and version may enter the first authorized review? |

## 6.6 RND-ENABLE-006 — Closure Execution Authorization Packaging

| Field | Specification |
|---|---|
| Enablement Item ID | `RND-ENABLE-006` |
| Source Closure Action | `RND-CLOSE-006` |
| Source Blocking Action | `RND-BA-006` |
| Related Closure Gates | `RND-CGATE-007`, `008` |
| Related prerequisites | `RND-PREQ-019`, `020`, `021` |
| Related blockers | `RND-BLOCK-006` |
| Related Candidate–Host pairs | All non-excluded pairs |
| Related Spikes | All spikes |
| Dependency ownership | Authorization |
| Shared UI source IDs | `BA-008`, `UI-AUTH-003`..`UI-AUTH-008` |
| Existing specification evidence | Parent records all authorization as not granted and permission No。 |
| Current unresolved specification | Exact operator、scope、duration、stop condition、cleanup、evidence obligation。 |
| Required final evidence | Future authorization package with exact actions, owner, expiry, audit trail and rollback。 |
| Proposed enablement operation | Future human authorization review only；本文件不提出 approval request。 |
| Operation classifications | Repository documentation mutation；future authorization packaging。 |
| Exact scope | Exact selected enablement item; no implicit expansion to runtime spike。 |
| Explicit exclusions | Runtime Spike、formal decision、broad repository mutation、system setting changes。 |
| Official-source lookup required | No current operation。 |
| Local read-only inspection required | No current operation。 |
| Network access required | Not granted。 |
| Package acquisition required | Not granted。 |
| Installation required | Not granted。 |
| Repository mutation required | Future authorization record only。 |
| Experimental project required | Not granted。 |
| Restore required | Not granted。 |
| Build required | Not granted。 |
| Runtime execution required | Not granted; separate future review。 |
| Evidence capture required | Authorization audit record only; no actual evidence now。 |
| System configuration mutation required | No planned。 |
| Administrator privilege required | Not granted。 |
| Human authorization required | Yes; explicit and separate。 |
| Expected files or directories | Future authorization record; none created now。 |
| Expected package/cache changes | None now。 |
| Expected machine changes | None now。 |
| Risk classification | R3/R4: authorization and possible environment mutation。 |
| Failure impact | Plan could be mistaken for permission; execution boundary would be unsafe。 |
| Stop condition | Any action is requested without exact permission or scope。 |
| Rollback/cleanup requirement | Future authorization must name rollback, cleanup and expiry conditions。 |
| Success condition | Each future action has an explicit permission, owner and bounded effect。 |
| Result artifact obligation | Future authorization package only; no current artifact。 |
| Resulting prerequisite recommendation | `RND-PREQ-019`..`021` remain Blocked。 |
| Resulting blocker recommendation | `RND-BLOCK-006` remains Blocked until separate authorization exists。 |
| Resulting pair recommendation | No pair receives execution permission from this specification。 |
| Phase R1 effect | Direct R1 enablement blocker; runtime remains separate。 |
| Owner | TBD |
| Status | Blocked |
| Open questions | Who can approve each action, and what exact expiry applies? |

## 7. Shared UI Authority Inheritance Matrix

| Rendering Enablement | Shared capability | UI source IDs | Current UI decision | Reusable scope | Rendering-specific extension | Duplicate request prohibited |
|---|---|---|---|---|---|---|
| `RND-ENABLE-001` | WinUI 3 experimental build path | `RESEARCH-TECH-UI-007`, `BA-001`, `UI-AUTH-001` | Pending/Not ready | Host path definition only | Candidate render path and version | Do not create `RND-AUTH` for WinUI host |
| `RND-ENABLE-001` | WPF experimental build path | `RESEARCH-TECH-UI-007`, `BA-002`, `UI-AUTH-002` | Pending/Not ready | Host path definition only | Candidate render path and version | Do not create a second WPF request |
| `RND-ENABLE-002` | x64 Windows baseline | `RESEARCH-TECH-UI-007`, `UI-PREQ-002` | Partially resolved | Baseline definition | Rendering environment fields | Do not duplicate x64 action |
| `RND-ENABLE-003` | Experimental Project isolation | `RESEARCH-TECH-UI-008`, `UI-ENABLE-005` | Not ready | Isolation rule | Candidate-specific directory boundary | Do not create duplicate isolation authority |
| `RND-ENABLE-005` | Package Restore | `RESEARCH-TECH-UI-009`, `UI-AUTH-003` | Pending | Shared restore boundary | Exact candidate package identity | Do not treat restore as runtime permission |
| `RND-ENABLE-001` | Build execution | `RESEARCH-TECH-UI-009`, `UI-AUTH-005`..`007` | Not granted | Build authority boundary | Candidate build scope | Do not imply build permission |
| `RND-ENABLE-004` | Evidence root | `RESEARCH-TECH-UI-007`, `BA-006` | Partially resolved | Root policy | Rendering method/manifest | Do not create second root |
| `RND-ENABLE-004` | Safety/cleanup | `RESEARCH-TECH-UI-007`, `BA-007` | Partially resolved | Cleanup rule | Candidate artifact list | Do not close by statement alone |
| `RND-ENABLE-006` | Runtime execution authorization | `RESEARCH-TECH-UI-009`, `BA-008`, `UI-AUTH-008` | Not granted | Separate future review | No runtime authority is inherited | Do not merge with closure enablement |

要求：

- 既有 `UI-AUTH` 不得重新包裝成 `RND-AUTH`。
- Shared UI decision 尚為 `Pending` 時，依賴它的 `RND-ENABLE` 保持 `Blocked` 或 `Partially specified`。
- Rendering-specific package scope 可以額外規格化，但不得在本文件中申請。

## 8. Experimental Candidate Identity Baseline

| Candidate | Official technology/package identity | Experimental version | Official source | Source date | Host scope | Managed dependency | Native dependency | Local availability | Build verified | Runtime verified |
|---|---|---|---|---|---|---|---|---|---|---|
| Framework-native retained-mode | Host-native rendering surface | TBD | Parent feasibility record | Not checked | WinUI 3, WPF | Host framework | Host-native | Unknown | No | No |
| Direct2D/DirectWrite | Direct2D/DirectWrite interop surface | TBD | Parent feasibility record | Not checked | WPF; other host only if eligible | Interop binding | Direct2D/DirectWrite | Unknown | No | No |
| Win2D | Win2D managed/native package | TBD | Parent feasibility record | Not checked | WinUI 3 | Win2D package | Win2D native assets | Unknown | No | No |
| SkiaSharp | SkiaSharp managed package | TBD | Parent feasibility record | Not checked | WinUI 3, WPF | SkiaSharp package | SkiaSharp native runtime asset | Unknown | No | No |
| Hybrid strategy | Host plus candidate ownership boundary | TBD | Parent feasibility record | Not checked | WinUI 3, WPF | Host/candidate bridge | Candidate-specific interop | Unknown | No | No |

要求：

- 官方版本與本機 availability 分開。
- 沒有官方 evidence 時使用 `TBD`。
- 沒有本機 evidence 時使用 `Unknown`。
- 所有 `Build verified = No`。
- 所有 `Runtime verified = No`。
- 可進行官方來源研究，但不得執行本機查詢。
- Experimental version 不代表正式產品版本。

## 9. Candidate–Host Enablement Matrix

| Pair | Candidate | Host | Current readiness | Shared UI dependency | Candidate dependency | Required operation | Enablement Item | Target recommendation |
|---|---|---|---|---|---|---|---|---|
| `RND-PAIR-001` | Framework-native | WinUI 3 | Blocked | Host path, x64, isolation, build | Host-native surface | Host/build enablement | `RND-ENABLE-001`..`006` | Blocked |
| `RND-PAIR-002` | Framework-native | WPF | Not evaluated | WPF path, x64, isolation, build | Host-native surface | Host/build enablement | `RND-ENABLE-001`..`006` | Not evaluated |
| `RND-PAIR-003` | WPF-native | WinUI 3 | Not evaluated | WinUI 3 path and isolation | Candidate host boundary | Candidate identity review | `RND-ENABLE-001`, `005`, `006` | Not evaluated |
| `RND-PAIR-004` | WPF-native | WPF | Blocked | WPF path, x64, isolation, build | WPF native surface | Host/build enablement | `RND-ENABLE-001`..`006` | Blocked |
| `RND-PAIR-005` | Direct2D/DirectWrite | WinUI 3 | Not evaluated | WinUI 3 path | Native interop | Interop dependency review | `RND-ENABLE-001`, `005`, `006` | Deferred |
| `RND-PAIR-006` | Direct2D/DirectWrite | WPF | Blocked | WPF path and build | Native interop | Interop dependency review | `RND-ENABLE-001`, `005`, `006` | Deferred |
| `RND-PAIR-007` | Win2D | WinUI 3 | Blocked | WinUI 3 path, package, isolation | Win2D package/native assets | Package identity and restore scope | `RND-ENABLE-003`..`006` | Deferred |
| `RND-PAIR-008` | Win2D | WPF | Excluded with evidence | WPF scope | Win2D host compatibility | None; retain exclusion | None | Excluded with evidence |
| `RND-PAIR-009` | SkiaSharp | WinUI 3 | Blocked | WinUI 3 path, package, isolation | Managed/native assets | Package identity and native boundary | `RND-ENABLE-003`..`006` | Deferred |
| `RND-PAIR-010` | SkiaSharp | WPF | Blocked | WPF path, package, isolation | Managed/native assets | Package identity and native boundary | `RND-ENABLE-001`, `003`..`006` | Deferred |

規則：

- 所有 Pair 都有一列。
- `Unknown` eligibility 不得改成 `Excluded with evidence`。
- Pair 若 Deferred 至 R2/R3，必須記錄 reactivation condition。
- 不得因某套件較容易取得而選擇候選。

## 10. Operation Classification

| Classification | Risk level |
|---|---|
| `Official-source research` | R0 |
| `Local read-only inspection` | R0 |
| `Repository documentation mutation` | R1 |
| `Repository experimental asset creation` | R1 |
| `Package acquisition` | R2 |
| `Package Restore` | R2 |
| `Development environment installation` | R3 |
| `Experimental project creation` | R1 |
| `Build execution` | R2 |
| `Evidence capture without runtime UI` | R1 |
| `Runtime execution` | R4 |
| `System configuration mutation` | R4 |

規則：

- 本文件只能規格化 R0 至 R3。
- R4 可列為明確排除項目，但不得申請。
- 一個 operation 涉及多個層級時採最高風險。
- `Build execution` 不得被解讀為 `Run`。
- `Evidence capture without runtime UI` 不包含任何 runtime rendering output、recording 或 product UI capture。

## 11. Repository Isolation Boundary

只規劃，不建立：

```text
experiments/rendering/<host>/<candidate>/
docs/Research/Technology/results/rendering/
```

必須規定：

- Experimental Project 不得放入正式產品 Source tree。
- 不得被產品 Project reference。
- 不得建立正式 Architecture component。
- 每個 Host/Candidate 使用隔離目錄。
- Package、Build output、temporary asset 與 Result 必須可識別。
- 所有新增項目必須可由 cleanup manifest 清除。
- 本文件不得建立上述目錄。

## 12. Project/Restore/Build Enablement Specification

三類權限必須分開定義，不得合併。

### 12.1 Experimental Project Creation

未來 specification 必須記錄：

- 預計 Host。
- Candidate。
- Target framework。
- Repository path。
- Minimal project contents。
- Explicitly prohibited product contents。

### 12.2 Package Acquisition/Restore

未來 specification 必須記錄：

- 精確 Package ID。
- Experimental version。
- Package source。
- Expected transitive dependencies。
- Native asset implications。
- Cache effect。
- Offline/rollback limitation。

### 12.3 Build Verification

未來 specification 必須記錄：

- Build tool。
- Build configuration。
- Architecture。
- Expected outputs。
- Required logs。
- Exit-code handling。
- Cleanup requirements。

固定：

`Run permitted: No`、`Runtime execution permitted: No`

## 13. Synthetic Workload Enablement

| Workload capability | Existing specification | Future static asset | Creation required | Runtime required | Enablement Item | Remaining gap |
|---|---|---|---|---|---|---|
| Canvas constants | Workload contract | Manifest only | Future | No | `RND-ENABLE-003` | Dimensions not frozen |
| Background color blocks | Workload list | Static definition | Future | No | `RND-ENABLE-003` | Expected color/alpha policy |
| Selection rectangle constants | Workload list | Static geometry | Future | No | `RND-ENABLE-003` | Geometry not frozen |
| Vector geometry | Candidate comparison scope | Static geometry | Future | No | `RND-ENABLE-003` | Shape list not frozen |
| Stroke variations | Workload list | Static constants | Future | No | `RND-ENABLE-003` | Width cases not frozen |
| Rotation values | Workload list | Static constants | Future | No | `RND-ENABLE-003` | Transform cases not frozen |
| Handle geometry | Workload list | Static geometry | Future | No | `RND-ENABLE-003` | Hit-test boundary not frozen |
| Overlapping layers | Workload list | Static layer manifest | Future | No | `RND-ENABLE-003` | Compositing cases not frozen |
| Mixed-language text | Text/fallback requirement | Static strings | Future | No | `RND-ENABLE-003` | Strings and fonts not frozen |
| Font fallback strings | Font requirement | Static text manifest | Future | No | `RND-ENABLE-003` | Fallback provenance missing |
| Mosaic input pattern | Mosaic requirement | Static algorithm input | Future | No | `RND-ENABLE-003` | Boundary rules missing |
| Clipping geometry | Workload list | Static geometry | Future | No | `RND-ENABLE-003` | Clip cases not frozen |
| Alpha gradient | Alpha requirement | Static constants | Future | No | `RND-ENABLE-003` | Format/alpha mode missing |
| Scale cases | DPI requirement | Static scale manifest | Future | No | `RND-ENABLE-002`, `003` | Same/mixed-DPI split missing |
| Pointer/hit-test sequence | Input requirement | Static sequence | Future | No | `RND-ENABLE-003` | Coordinate sequence missing |

本文件只定義未來資產，不建立資產。

## 14. Evidence Method Enablement

| Evidence capability | Planned method | Required tool/library | Package or install required | Runtime required | Authorization class | Enablement Item |
|---|---|---|---|---|---|---|
| Environment record | Fixed host/display/DPI manifest | Approved record format | No | No | R1 | `RND-ENABLE-002` |
| Build log | Capture future authorized build output | Approved log format | No | No | R2 | `RND-ENABLE-001` |
| Dependency inventory | Record exact package/native identity | Approved manifest format | No | No | R2 | `RND-ENABLE-005` |
| Rendered output | Future candidate output only | Future candidate path | TBD | Yes | R4; excluded now | `RND-ENABLE-004` |
| PNG export | Future output export method | Future encoder/decoder | TBD | Yes | R4; excluded now | `RND-ENABLE-004` |
| Reference image | Future reproducible reference method | Future reference method | TBD | Yes | R4; excluded now | `RND-ENABLE-004` |
| Pixel-difference calculation | Future defined comparative method | Future comparison method | TBD | Yes | R4; excluded now | `RND-ENABLE-004` |
| Alpha inspection | Future channel/alpha inspection | Future pixel inspection method | TBD | Yes | R4; excluded now | `RND-ENABLE-004` |
| Coordinate comparison | Future logical/pixel comparison | Environment and evidence method | No planned | Yes | R4; excluded now | `RND-ENABLE-002`, `004` |
| Font fallback inspection | Future fixed font case | Future host font record | TBD | Yes | R4; excluded now | `RND-ENABLE-003`, `004` |
| Mosaic comparison | Future deterministic algorithm comparison | Future algorithm method | No planned | Yes | R4; excluded now | `RND-ENABLE-003`, `004` |
| Failure reproduction | Future manifest-based replay | Approved evidence manifest | No planned | Yes | R4; excluded now | `RND-ENABLE-003`, `004` |
| Cleanup confirmation | Future before/after scope record | Cleanup manifest | No | No | R1 | `RND-ENABLE-004` |

要求：

- Runtime 型 Evidence 必須標示為本輪授權範圍外。
- 不得建立實際 Evidence。
- 不得自行設定 pixel-difference 或效能通過門檻。

## 15. Phase R1 Enablement Gate

| Closure Gate | Required specification | Related Enablement Items | Current specification status | Remaining gap |
|---|---|---|---|---|
| `RND-CGATE-001` | WinUI 3 Host build path evidence or explicit authorized path | `RND-ENABLE-001`, `006` | Blocked | Host/version authority not available |
| `RND-CGATE-002` | WPF Host build path evidence or explicit authorized path | `RND-ENABLE-001`, `006` | Blocked | WPF path authority not available |
| `RND-CGATE-003` | Framework-native baseline fixed | `RND-ENABLE-001`, `005` | Blocked | Candidate/host identity unresolved |
| `RND-CGATE-004` | One non-native package/interop path specified | `RND-ENABLE-005` | Blocked | Exact dependency and authority unresolved |
| `RND-CGATE-005` | Synthetic vector/hit-test workload specified | `RND-ENABLE-003` | Partially specified | Constants and input sequence not frozen |
| `RND-CGATE-006` | Evidence storage, naming and cleanup specified | `RND-ENABLE-004` | Partially specified | Root and cleanup acceptance missing |
| `RND-CGATE-007` | Build/Project/Package permissions separated | `RND-ENABLE-001`, `005`, `006` | Blocked | Exact future scope not authorized |
| `RND-CGATE-008` | Runtime execution remains separately controlled | `RND-ENABLE-006` | Specified | Future runtime authorization remains required |

Status 只能使用 `Specified`、`Partially specified`、`Blocked`、`Deferred`；不得使用 `Satisfied`、`Passed` 或 `Resolved`。

## 16. Authorization Packaging Matrix

| Enablement Item | Operation classifications | Highest risk | Shared UI authority dependency | Rendering-specific authority required | Current authorization | Execution permitted |
|---|---|---|---|---|---|---|
| `RND-ENABLE-001` | Documentation, project, build | R2 | `UI-AUTH-001`, `002`, `005`..`007` | Host/candidate build scope | Not granted | No |
| `RND-ENABLE-002` | Read-only environment, evidence manifest | R1 | `BA-003`, `004` | Rendering coordinate cases | Not granted | No |
| `RND-ENABLE-003` | Documentation, experimental asset definition | R1 | `BA-005`, `UI-ENABLE-005` | Workload asset scope | Not granted | No |
| `RND-ENABLE-004` | Documentation, evidence governance | R1 | `BA-006`, `007` | Rendering evidence method scope | Not granted | No |
| `RND-ENABLE-005` | Official research, package, restore | R2 | `UI-AUTH-003`, `004` | Exact candidate/package/native scope | Not granted | No |
| `RND-ENABLE-006` | Authorization packaging | R3/R4 | `BA-008`, `UI-AUTH-008` | Exact closure operation scope | Not granted | No |

所有項目固定：

`Current authorization: Not granted`、`Execution permitted: No`

本文件不得建立 `RND-AUTH` ID；正式 Request ID 留待後續 Authorization Request 文件。

## 17. Enablement Completeness Matrix

| Blocking Action | Closure Action | Enablement Item | Specification complete | Shared authority identified | Rendering authority identified | Evidence obligation identified | Remaining gap |
|---|---|---|---|---|---|---|---|
| `RND-BA-001` | `RND-CLOSE-001` | `RND-ENABLE-001` | Partially | Yes | Partially | Yes | Exact host/version and build boundary |
| `RND-BA-002` | `RND-CLOSE-002` | `RND-ENABLE-002` | Partially | Yes | Partially | Yes | Actual baseline and coordinate protocol |
| `RND-BA-003` | `RND-CLOSE-003` | `RND-ENABLE-003` | Partially | Yes | Yes | Yes | Static workload constants and isolation manifest |
| `RND-BA-004` | `RND-CLOSE-004` | `RND-ENABLE-004` | Partially | Yes | Yes | Yes | Evidence root and cleanup acceptance |
| `RND-BA-005` | `RND-CLOSE-005` | `RND-ENABLE-005` | No | Yes | Partially | Yes | Exact package/native identity and host compatibility |
| `RND-BA-006` | `RND-CLOSE-006` | `RND-ENABLE-006` | Partially | Yes | Yes | Yes | Exact human authorization scope |

`Specification complete` 只能使用 `Yes`、`Partially`、`No`。`Yes` 不代表 operation 已獲授權或已執行。

## 18. Final Enablement Status

允許值：

- `Ready to request rendering prerequisite closure execution authorization`
- `Conditionally ready to request rendering prerequisite closure execution authorization`
- `Not ready to request rendering prerequisite closure execution authorization`

判定由下列項目推導：

```text
Open RND-ENABLE-GAP
  + Shared UI authority dependencies
  + Candidate identity completeness
  + Project/Restore/Build scope completeness
  + Evidence obligation completeness
  -> Final Enablement Status
```

目前 Final Enablement Status：

`Not ready to request rendering prerequisite closure execution authorization`

即使未來判定為 Ready，仍固定：

`Closure Execution Authorized: No`、`Build Verification: Not performed`、`Runtime Verification: Not performed`、`Runtime Spike Execution Authorized: No`、`Rendering Decision: Not made`

## 19. Traceability

### 19.1 Enablement Chain

```text
RND-BA
  -> RND-CLOSE
  -> RND-ENABLE
  -> Shared UI authority / Rendering-specific authority
  -> Future authorization request
  -> Future closure execution evidence
  -> Rendering readiness reassessment
  -> Future TD-002 decision
```

### 19.2 References

- [`docs/Research/Technology/10-rendering-technology-feasibility.md`](10-rendering-technology-feasibility.md)
- [`docs/Research/Technology/11-rendering-technology-runtime-spike-plan.md`](11-rendering-technology-runtime-spike-plan.md)
- [`docs/Research/Technology/12-rendering-technology-runtime-spike-execution-readiness.md`](12-rendering-technology-runtime-spike-execution-readiness.md)
- [`docs/Research/Technology/13-rendering-technology-prerequisite-closure-plan.md`](13-rendering-technology-prerequisite-closure-plan.md)
- [`docs/Research/Technology/07-ui-framework-phase1-readiness-reassessment.md`](07-ui-framework-phase1-readiness-reassessment.md)
- [`docs/Research/Technology/08-ui-framework-phase1-execution-enablement-specification.md`](08-ui-framework-phase1-execution-enablement-specification.md)
- [`docs/Research/Technology/09-ui-framework-phase1-enablement-execution-authorization-request.md`](09-ui-framework-phase1-enablement-execution-authorization-request.md)
- [`Architecture/adr/ADR-0002-ui-framework-selection.md`](../../../Architecture/adr/ADR-0002-ui-framework-selection.md)
- [`Architecture/TECHNOLOGY-DECISION-ROADMAP.md`](../../../Architecture/TECHNOLOGY-DECISION-ROADMAP.md)

## Completion Boundary

本輪完成條件：

- 只建立 `14-rendering-technology-prerequisite-execution-enablement-specification.md`。
- 不修改 README、任何索引、CHANGELOG、TODO 或其他文件。
- 建立正好 6 個 `RND-ENABLE`。
- 保持 6 組 `RND-BA -> RND-CLOSE -> RND-ENABLE` 一對一。
- 覆蓋 10 個 Candidate–Host Pair。
- 區分 Shared UI authority 與 Rendering-specific authority。
- 分開規格化 Project creation、Package acquisition/Restore 與 Build。
- 建立 Synthetic Workload 與 Evidence Method enablement。
- 建立 8 個 Phase R1 Enablement Gate。
- 所有 `Current authorization = Not granted`。
- 所有 `Execution permitted = No`。
- 不建立 `RND-AUTH`。
- 不執行本機盤點、下載、安裝、Restore、Build、Run 或 Runtime Spike。
- 不建立任何實驗目錄、Project、Prototype、Result 或 Evidence。
- 不修改 `ADR-0002` 或建立 TD-002 ADR。
- 未開始任何截圖功能或截圖相關 coding。
- `git diff --check` 必須通過；完成後立即停止。
