# Clipboard Integration Prerequisite Execution Enablement Specification

| Field | Value |
|---|---|
| Document ID | `RESEARCH-TECH-CLIPBOARD-005` |
| Title | Clipboard Integration Prerequisite Execution Enablement Specification |
| Status | Draft |
| Research Type | Execution Enablement Specification |
| Technology Decision | `TD-004 Clipboard Integration` |
| Parent Closure Plan | `RESEARCH-TECH-CLIPBOARD-004` |
| Parent Execution Readiness | `RESEARCH-TECH-CLIPBOARD-003` |
| Parent Runtime Plan | `RESEARCH-TECH-CLIPBOARD-002` |
| Parent Feasibility | `RESEARCH-TECH-CLIPBOARD-001` |
| Enablement Execution Status | Not started |
| Closure Execution Authorized | No |
| Clipboard Runtime Spike Authorized | No |
| Clipboard Read Authorized | No |
| Clipboard Write Authorized | No |
| Clipboard Clear Authorized | No |
| Evidence Write Authorized | No |
| Build Verification | Not performed |
| Runtime Verification | Not performed |
| UI Framework Decision | Unresolved; `ADR-0002` remains Draft |
| Shared UI Authorization Artifact | Not found / TBD |
| Rendering Decision | Not made |
| Capture Decision | Not made |
| Clipboard Decision | Not made |
| Owner | TBD |
| Last reviewed | Not reviewed |

## 1. Purpose

本文件只規格化 `CLIP-CLOSE-001..006` 在提交 Clipboard prerequisite closure execution authorization review 前，所需的精確操作範圍、Host/Candidate identity、Project/Restore/Build 邊界、隔離環境、Synthetic Image、Format/Consumer、Threading/COM、Evidence/Privacy 與權限分離規格。

這是 Execution Enablement Specification，不是 Closure Execution Record、Authorization Request、Human Authorization Decision、Clipboard Runtime Spike、Clipboard Technology Decision 或 Clipboard ADR。

## 2. Scope

本文件只規格化：

- `CLIP-BA-001..006`。
- `CLIP-CLOSE-001..006`。
- `CLIP-PREQ-001..032`、`CLIP-BLOCK-001..013`、`CLIP-PAIR-001..010`、`CLIP-SPIKE-001..012`。
- `CLIP-GATE-001..010` 與 `CLIP-CGATE-001..011`。
- Phase L1 最小 Host、Candidate、Project、Isolation、Synthetic Image、Format、Consumer、Threading、Evidence 及授權條件。
- Shared UI authority dependency 與 Clipboard-specific authorization delta。
- Phase L2/L3 的 Deferred dependency；不得將全部 L2/L3 項目升格為 Phase L1 blocker。

## 3. Non-goals and Frozen Boundaries

本文件不得：

- 執行任何 Enablement Item。
- 讀取、寫入、清除或備份 Windows Clipboard。
- 進行新的官方研究或本機盤點。
- 查詢 Package Cache。
- 下載或安裝 SDK、Runtime、Package、Tool 或 workload。
- 建立 Project、Solution、Prototype、Source Code、Consumer 或 Clipboard payload。
- 建立 Result directory、Log 或 Evidence Artifact。
- Restore、Build、Run、Publish、Test 或執行 Runtime Spike。
- 修改 Clipboard History、Cloud Clipboard、帳號、裝置或系統設定。
- 建立 Authorization Request 或作出 Human Authorization Decision。
- 修改 `RESEARCH-TECH-CLIPBOARD-001..004`、UI/Capture/Rendering Research Line 或 `ADR-0002`。
- 建立 Clipboard ADR 或選擇 Clipboard Technology。
- 開始正式 Clipboard 或截圖功能。

## 4. Controlled Vocabulary

### 4.1 Enablement Item Status

只能使用 `Specified`、`Partially specified`、`Blocked`、`Deferred` 或 `Not applicable`。不得使用 `Completed`、`Resolved`、`Approved`、`Authorized` 或 `Executed`。

### 4.2 Specification Evidence Status

只能使用 `Accepted from parent evidence`、`Confirmed by official source`、`Partially specified`、`Unknown`、`Conflicting` 或 `Not applicable`。

### 4.3 Execution Permission

本文件所有 execution permission 固定為 `No`。Specification 可描述未來 operation，但不授予 operation。

### 4.4 Shared Authority Reference Status

只能使用 `Existing artifact identified`、`Research evidence identified, authority artifact absent`、`TBD` 或 `Not applicable`。

Repository 沒有 `UI-AUTH-*` 文件；因此 Shared UI authority 使用 `Research evidence identified, authority artifact absent`，Authority reference 使用 `TBD`，不得虛構授權 ID。

## 5. Enablement Binding

| Enablement Item | Closure Action | Blocking Action |
|---|---|---|
| `CLIP-ENABLE-001` | `CLIP-CLOSE-001` | `CLIP-BA-001` |
| `CLIP-ENABLE-002` | `CLIP-CLOSE-002` | `CLIP-BA-002` |
| `CLIP-ENABLE-003` | `CLIP-CLOSE-003` | `CLIP-BA-003` |
| `CLIP-ENABLE-004` | `CLIP-CLOSE-004` | `CLIP-BA-004` |
| `CLIP-ENABLE-005` | `CLIP-CLOSE-005` | `CLIP-BA-005` |
| `CLIP-ENABLE-006` | `CLIP-CLOSE-006` | `CLIP-BA-006` |

不得重新編號、合併或拆分。不得新增第七個 Phase L1 Blocking Action。上游資訊不足時使用 `CLIP-ENABLE-GAP-xxx`，不得修改上游文件。

## 6. Fixed Fields for Every Enablement Item

每個 `CLIP-ENABLE` 必須擁有同一組欄位。下列六個 action sections 逐欄填寫，任何欄位不得省略：

- Enablement Item ID
- Source Closure Action
- Source Blocking Action
- Blocking condition
- Related prerequisites
- Related blockers
- Related Candidate–Host pairs
- Related Runtime Spikes
- Related upstream gates
- Related closure gates
- Dependency ownership
- Shared UI source IDs
- Shared authority reference status
- Rendering source IDs
- Capture source IDs
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
- Experimental asset required
- Consumer asset required
- Experimental Project required
- Restore required
- Build required
- Clipboard read required
- Clipboard write required
- Clipboard clear required
- Runtime execution required
- Evidence persistence required
- History/Cloud setting mutation required
- Administrator privilege required
- Human authorization required
- Expected files/directories
- Expected Package/Cache effects
- Expected machine effects
- Expected Clipboard effect
- Privacy impact
- Risk classification
- Failure impact
- Stop conditions
- Rollback/cleanup requirement
- Success condition
- Result artifact obligation
- Resulting prerequisite recommendation
- Resulting blocker recommendation
- Resulting Pair recommendation
- Phase L1 effect
- Owner
- Status
- Open questions

## 7. Enablement Item Specifications

### 7.1 `CLIP-ENABLE-001` — Shared UI Host

- Enablement Item ID: `CLIP-ENABLE-001`
- Source Closure Action: `CLIP-CLOSE-001`
- Source Blocking Action: `CLIP-BA-001`
- Blocking condition: WPF/WinUI 3 Host identity and shared authority remain unresolved.
- Related prerequisites: `CLIP-PREQ-001..003`
- Related blockers: `CLIP-BLOCK-001..002`
- Related Candidate–Host pairs: `CLIP-PAIR-001..010`
- Related Runtime Spikes: `CLIP-SPIKE-001..012`
- Related upstream gates: `CLIP-GATE-001`, `CLIP-GATE-003`, `CLIP-GATE-006`
- Related closure gates: `CLIP-CGATE-001`, `CLIP-CGATE-003`, `CLIP-CGATE-011`
- Dependency ownership: Shared UI research; Authorization
- Shared UI source IDs: `RESEARCH-TECH-UI-001`; `UI-AUTH-*` not found
- Shared authority reference status: Research evidence identified, authority artifact absent; reference `TBD`
- Rendering source IDs: `RESEARCH-TECH-RENDER-001`
- Capture source IDs: `RESEARCH-TECH-CAPTURE-001`
- Existing specification evidence: UI feasibility, Architecture boundaries, and `ADR-0002` Draft state.
- Current unresolved specification: Host target, SDK alignment, package mode, and authority owner.
- Required final evidence: Reviewable Host identity, target baseline, package context, and explicit Shared UI authority reference.
- Proposed enablement operation: Bind Shared UI research evidence to a future isolated Host specification.
- Operation classifications: Repository documentation mutation; future Local read-only inspection.
- Exact scope: WPF and WinUI 3 Host identity, target/version, launch context, package mode, and authority reuse.
- Explicit exclusions: No Project, Package, Restore, Build, Runtime, Clipboard operation, or technology selection.
- Official-source lookup required: No for this specification.
- Local read-only inspection required: No for this specification.
- Network access required: No.
- Package acquisition required: No.
- Installation required: No.
- Repository mutation required: Only this document.
- Experimental asset required: No.
- Consumer asset required: No.
- Experimental Project required: No.
- Restore required: No.
- Build required: No.
- Clipboard read required: No.
- Clipboard write required: No.
- Clipboard clear required: No.
- Runtime execution required: No.
- Evidence persistence required: No.
- History/Cloud setting mutation required: No.
- Administrator privilege required: No.
- Human authorization required: Yes, in a later scoped record.
- Expected files/directories: No additional files or directories in this specification.
- Expected Package/Cache effects: None.
- Expected machine effects: None.
- Expected Clipboard effect: None.
- Privacy impact: None; no Clipboard data is accessed.
- Risk classification: R1 documentation; future environment operations are separate.
- Failure impact: Host-dependent enablement remains Blocked.
- Stop conditions: Any installation, Project, Restore, Build, Runtime, or Clipboard request under this item.
- Rollback/cleanup requirement: Documentation review rollback only; no machine cleanup.
- Success condition: Host and authority boundary can be reviewed without claiming approval.
- Result artifact obligation: No result artifact in this specification.
- Resulting prerequisite recommendation: `CLIP-PREQ-001..003` Partially specified.
- Resulting blocker recommendation: `CLIP-BLOCK-001..002` Blocked.
- Resulting Pair recommendation: All pairs remain Blocked pending remaining enablement items.
- Phase L1 effect: Host boundary is specified but not enabled.
- Owner: UI owner TBD.
- Status: Blocked.
- Open questions: Which Host, target framework, package mode, and authority record are permitted?

### 7.2 `CLIP-ENABLE-002` — Candidate API/Interop Identity

- Enablement Item ID: `CLIP-ENABLE-002`
- Source Closure Action: `CLIP-CLOSE-002`
- Source Blocking Action: `CLIP-BA-002`
- Blocking condition: Candidate API/Interop route is not bound to an experimental identity.
- Related prerequisites: `CLIP-PREQ-004..008`
- Related blockers: `CLIP-BLOCK-003`
- Related Candidate–Host pairs: `CLIP-PAIR-001..010`
- Related Runtime Spikes: `CLIP-SPIKE-001..005`
- Related upstream gates: `CLIP-GATE-001`, `CLIP-GATE-003`, `CLIP-GATE-005`
- Related closure gates: `CLIP-CGATE-002`, `CLIP-CGATE-003`, `CLIP-CGATE-006`
- Dependency ownership: Clipboard-specific; Threading/COM
- Shared UI source IDs: `RESEARCH-TECH-UI-001`; `UI-AUTH-*` not found
- Shared authority reference status: Research evidence identified, authority artifact absent; reference `TBD`
- Rendering source IDs: `RESEARCH-TECH-RENDER-001`
- Capture source IDs: `RESEARCH-TECH-CAPTURE-001`
- Existing specification evidence: Five candidate identities and ten Candidate–Host pairs in parent research.
- Current unresolved specification: Exact experimental API identity, host adapter composition, and package boundary.
- Required final evidence: At least one reviewable API/Interop identity with Host, thread, Project, and evidence boundary.
- Proposed enablement operation: Bind candidate identities to future isolated experiments without selection.
- Operation classifications: Repository documentation mutation; future Experimental Project creation.
- Exact scope: `OPT-001..005`, API identity, adapter composition, COM/STA, Dispatcher, and exclusions.
- Explicit exclusions: No API call, Package acquisition, Project creation, Clipboard operation, ranking, or technology selection.
- Official-source lookup required: No for this specification.
- Local read-only inspection required: No for this specification.
- Network access required: No.
- Package acquisition required: No.
- Installation required: No.
- Repository mutation required: Only this document.
- Experimental asset required: No.
- Consumer asset required: No.
- Experimental Project required: Future, not in this specification.
- Restore required: Future, not in this specification.
- Build required: Future, not in this specification.
- Clipboard read required: No.
- Clipboard write required: No.
- Clipboard clear required: No.
- Runtime execution required: No.
- Evidence persistence required: No.
- History/Cloud setting mutation required: No.
- Administrator privilege required: No.
- Human authorization required: Yes, separately per future operation.
- Expected files/directories: No additional files or directories in this specification.
- Expected Package/Cache effects: None.
- Expected machine effects: None.
- Expected Clipboard effect: None.
- Privacy impact: None; no payload is created or accessed.
- Risk classification: R1 documentation; future candidate operations are R2-R4.
- Failure impact: Candidate-dependent enablement remains Blocked.
- Stop conditions: Any candidate API call, package install, Project creation, or runtime action.
- Rollback/cleanup requirement: Documentation review rollback; no runtime cleanup.
- Success condition: Candidate route identity is reviewable without claiming local or runtime support.
- Result artifact obligation: No candidate result artifact in this specification.
- Resulting prerequisite recommendation: `CLIP-PREQ-004..008` Partially specified.
- Resulting blocker recommendation: `CLIP-BLOCK-003` Blocked.
- Resulting Pair recommendation: Candidate pairs remain Blocked or Unknown.
- Phase L1 effect: Candidate identity is specified but not enabled.
- Owner: Clipboard owner TBD.
- Status: Blocked.
- Open questions: Which candidate identity is allowed for a future isolated experiment?

### 7.3 `CLIP-ENABLE-003` — Clipboard Isolation and Operation Boundary

- Enablement Item ID: `CLIP-ENABLE-003`
- Source Closure Action: `CLIP-CLOSE-003`
- Source Blocking Action: `CLIP-BA-003`
- Blocking condition: Isolation policy and separate Read/Write/Clear permissions are not authorized.
- Related prerequisites: `CLIP-PREQ-013..015`
- Related blockers: `CLIP-BLOCK-006`
- Related Candidate–Host pairs: `CLIP-PAIR-001..010`
- Related Runtime Spikes: `CLIP-SPIKE-001..012`
- Related upstream gates: `CLIP-GATE-008`, `CLIP-GATE-009`, `CLIP-GATE-010`
- Related closure gates: `CLIP-CGATE-004`, `CLIP-CGATE-008`, `CLIP-CGATE-009`
- Dependency ownership: Clipboard isolation; Authorization; Evidence/privacy
- Shared UI source IDs: `RESEARCH-TECH-UI-001`; `UI-AUTH-*` not found
- Shared authority reference status: Research evidence identified, authority artifact absent; reference `TBD`
- Rendering source IDs: `RESEARCH-TECH-RENDER-001`
- Capture source IDs: `RESEARCH-TECH-CAPTURE-001`
- Existing specification evidence: No approved isolated account/VM/session policy exists.
- Current unresolved specification: Isolation mode, existing-content policy, overwrite consent, and cleanup proof.
- Required final evidence: Written isolated environment policy, no-read/no-clear rule, privacy stop, and cleanup authority.
- Proposed enablement operation: Define the future isolated environment and independent operation permissions.
- Operation classifications: Repository documentation mutation; future Clipboard read/write/clear only under separate authority.
- Exact scope: Dedicated user/VM/session, synthetic-only content, pre/post state, consumer boundary, and process cleanup.
- Explicit exclusions: No current Clipboard inspection, backup, clear, History/Cloud mutation, or environment creation.
- Official-source lookup required: No for this specification.
- Local read-only inspection required: No for this specification.
- Network access required: No.
- Package acquisition required: No.
- Installation required: No.
- Repository mutation required: Only this document.
- Experimental asset required: No.
- Consumer asset required: No.
- Experimental Project required: No.
- Restore required: No.
- Build required: No.
- Clipboard read required: No.
- Clipboard write required: No.
- Clipboard clear required: No.
- Runtime execution required: No.
- Evidence persistence required: No.
- History/Cloud setting mutation required: No.
- Administrator privilege required: No.
- Human authorization required: Yes, separately per operation.
- Expected files/directories: No isolation directory in this specification.
- Expected Package/Cache effects: None.
- Expected machine effects: None.
- Expected Clipboard effect: None.
- Privacy impact: None; current content is explicitly out of scope.
- Risk classification: R1 documentation; future Clipboard operations are R4.
- Failure impact: All Clipboard operation enablement remains Blocked.
- Stop conditions: Any Clipboard inspection, write, clear, backup, sync, or setting mutation.
- Rollback/cleanup requirement: No machine state changed; future cleanup must be pre-specified.
- Success condition: A future authorization review can distinguish isolation from operation permission.
- Result artifact obligation: No isolation result artifact in this specification.
- Resulting prerequisite recommendation: `CLIP-PREQ-013..015` Partially specified.
- Resulting blocker recommendation: `CLIP-BLOCK-006` Blocked.
- Resulting Pair recommendation: All pairs remain Blocked.
- Phase L1 effect: Isolation boundary is specified but not enabled.
- Owner: Privacy owner TBD.
- Status: Blocked.
- Open questions: Which dedicated isolation mode is permitted and who grants each operation?

### 7.4 `CLIP-ENABLE-004` — Synthetic Image Input

- Enablement Item ID: `CLIP-ENABLE-004`
- Source Closure Action: `CLIP-CLOSE-004`
- Source Blocking Action: `CLIP-BA-004`
- Blocking condition: Synthetic Image contract is not approved and no runtime input may be created here.
- Related prerequisites: `CLIP-PREQ-016`
- Related blockers: `CLIP-BLOCK-007`
- Related Candidate–Host pairs: `CLIP-PAIR-001..010`
- Related Runtime Spikes: `CLIP-SPIKE-001..005`
- Related upstream gates: `CLIP-GATE-010`
- Related closure gates: `CLIP-CGATE-005`, `CLIP-CGATE-006`, `CLIP-CGATE-007`
- Dependency ownership: Synthetic image; Rendering research
- Shared UI source IDs: `RESEARCH-TECH-UI-001`; `UI-AUTH-*` not found
- Shared authority reference status: Research evidence identified, authority artifact absent; reference `TBD`
- Rendering source IDs: `RESEARCH-TECH-RENDER-001`
- Capture source IDs: `RESEARCH-TECH-CAPTURE-001`
- Existing specification evidence: Parent plan defines deterministic synthetic-image requirements only.
- Current unresolved specification: Dimensions, markers, alpha, color metadata, coordinates, and identity.
- Required final evidence: Approved deterministic specification and a separately authorized future asset identity.
- Proposed enablement operation: Specify future Synthetic Image input without creating bytes.
- Operation classifications: Repository documentation mutation; Synthetic image specification.
- Exact scope: Dimensions, size classes, border, markers, alpha, RGB/grayscale, language, coordinates, SDR/wide-color substitute, run identity.
- Explicit exclusions: No Bitmap, PNG, DIB, payload, rendering run, Clipboard write, or pixel result.
- Official-source lookup required: No for this specification.
- Local read-only inspection required: No for this specification.
- Network access required: No.
- Package acquisition required: No.
- Installation required: No.
- Repository mutation required: Only this document.
- Experimental asset required: Future, not in this specification.
- Consumer asset required: No.
- Experimental Project required: No.
- Restore required: No.
- Build required: No.
- Clipboard read required: No.
- Clipboard write required: No.
- Clipboard clear required: No.
- Runtime execution required: No.
- Evidence persistence required: No.
- History/Cloud setting mutation required: No.
- Administrator privilege required: No.
- Human authorization required: Yes, for future asset and execution.
- Expected files/directories: No asset, payload, or result directory.
- Expected Package/Cache effects: None.
- Expected machine effects: None.
- Expected Clipboard effect: None.
- Privacy impact: None; only future synthetic content is in scope.
- Risk classification: R1 documentation; future asset/runtime is R4 when paired with Clipboard.
- Failure impact: Format/Consumer enablement remains Blocked.
- Stop conditions: Any image creation, payload creation, render, or Clipboard operation.
- Rollback/cleanup requirement: Documentation-only rollback; no asset cleanup.
- Success condition: Future asset creation can be deterministic without selecting a technology.
- Result artifact obligation: No image or evidence artifact in this specification.
- Resulting prerequisite recommendation: `CLIP-PREQ-016` Partially specified.
- Resulting blocker recommendation: `CLIP-BLOCK-007` Blocked.
- Resulting Pair recommendation: All pairs remain Blocked.
- Phase L1 effect: Synthetic input contract is specified but not enabled.
- Owner: Evidence owner TBD.
- Status: Blocked.
- Open questions: Which size classes and color substitute are approved?

### 7.5 `CLIP-ENABLE-005` — Format and Consumer Verification

- Enablement Item ID: `CLIP-ENABLE-005`
- Source Closure Action: `CLIP-CLOSE-005`
- Source Blocking Action: `CLIP-BA-005`
- Blocking condition: Format, Consumer, Alpha/pixel/color, and interoperability methods lack runtime evidence.
- Related prerequisites: `CLIP-PREQ-017..025`
- Related blockers: `CLIP-BLOCK-008..009`
- Related Candidate–Host pairs: `CLIP-PAIR-001..010`
- Related Runtime Spikes: `CLIP-SPIKE-001..005`, `CLIP-SPIKE-009`
- Related upstream gates: `CLIP-GATE-006`, `CLIP-GATE-007`, `CLIP-GATE-010`
- Related closure gates: `CLIP-CGATE-006`, `CLIP-CGATE-007`, `CLIP-CGATE-010`
- Dependency ownership: Format/consumer; Rendering research; Clipboard-specific
- Shared UI source IDs: `RESEARCH-TECH-UI-001`; `UI-AUTH-*` not found
- Shared authority reference status: Research evidence identified, authority artifact absent; reference `TBD`
- Rendering source IDs: `RESEARCH-TECH-RENDER-001`
- Capture source IDs: `RESEARCH-TECH-CAPTURE-001`
- Existing specification evidence: Parent plan lists candidate formats and consumer classes.
- Current unresolved specification: Producer/consumer representation, fidelity method, lifetime, and privacy-safe consumer route.
- Required final evidence: Scoped format publication, consumer identity, Alpha/pixel/color method, lifetime, and no-private-data boundary.
- Proposed enablement operation: Specify future Format/Consumer observation without launching a consumer.
- Operation classifications: Repository documentation mutation; future Evidence capture/persistence.
- Exact scope: Framework Bitmap, CF_BITMAP, CF_DIB, CF_DIBV5, registered PNG, OLE IDataObject, WinRT DataPackage, multi-format, and isolated consumers.
- Explicit exclusions: No consumer launch, payload creation, Clipboard write, runtime conversion, third-party app, or technology selection.
- Official-source lookup required: No for this specification.
- Local read-only inspection required: No for this specification.
- Network access required: No.
- Package acquisition required: No.
- Installation required: No.
- Repository mutation required: Only this document.
- Experimental asset required: Future through `CLIP-ENABLE-004` authority.
- Consumer asset required: Future, not in this specification.
- Experimental Project required: Future, not in this specification.
- Restore required: No.
- Build required: No.
- Clipboard read required: No.
- Clipboard write required: No.
- Clipboard clear required: No.
- Runtime execution required: No.
- Evidence persistence required: No.
- History/Cloud setting mutation required: No.
- Administrator privilege required: No.
- Human authorization required: Yes, for consumer/runtime/evidence operations.
- Expected files/directories: No consumer Project, payload, or result directory.
- Expected Package/Cache effects: None.
- Expected machine effects: None.
- Expected Clipboard effect: None.
- Privacy impact: None; real user content and unapproved applications are excluded.
- Risk classification: R1 documentation; future consumer/runtime is R4.
- Failure impact: Format/Consumer-dependent enablement remains Blocked.
- Stop conditions: Any consumer launch, Clipboard operation, runtime conversion, or persistence.
- Rollback/cleanup requirement: Documentation-only rollback; no process cleanup.
- Success condition: Future Spike can identify representation, consumer, and observation method independently.
- Result artifact obligation: No consumer or fidelity result in this specification.
- Resulting prerequisite recommendation: `CLIP-PREQ-017..025` Partially specified.
- Resulting blocker recommendation: `CLIP-BLOCK-008..009` Blocked.
- Resulting Pair recommendation: All pairs remain Blocked.
- Phase L1 effect: Format and Consumer method is specified but not enabled.
- Owner: Clipboard owner TBD; Rendering owner TBD.
- Status: Blocked.
- Open questions: Which isolated Consumer set and fidelity method are permitted?

### 7.6 `CLIP-ENABLE-006` — Evidence, Privacy, Cleanup, and Authority

- Enablement Item ID: `CLIP-ENABLE-006`
- Source Closure Action: `CLIP-CLOSE-006`
- Source Blocking Action: `CLIP-BA-006`
- Blocking condition: Evidence persistence, privacy review, cleanup confirmation, and operation authority are ungranted.
- Related prerequisites: `CLIP-PREQ-031..032`
- Related blockers: `CLIP-BLOCK-012..013`
- Related Candidate–Host pairs: `CLIP-PAIR-001..010`
- Related Runtime Spikes: `CLIP-SPIKE-001..012`
- Related upstream gates: `CLIP-GATE-008`, `CLIP-GATE-009`, `CLIP-GATE-010`
- Related closure gates: `CLIP-CGATE-008`, `CLIP-CGATE-009`, `CLIP-CGATE-010`, `CLIP-CGATE-011`
- Dependency ownership: Evidence/privacy; Authorization
- Shared UI source IDs: `RESEARCH-TECH-UI-001`; `UI-AUTH-*` not found
- Shared authority reference status: Research evidence identified, authority artifact absent; reference `TBD`
- Rendering source IDs: `RESEARCH-TECH-RENDER-001`
- Capture source IDs: `RESEARCH-TECH-CAPTURE-001`
- Existing specification evidence: `AGENTS.md` execution boundary and parent readiness authority state.
- Current unresolved specification: Evidence root, schema, cleanup record, Project/Restore/Build authority, and Clipboard operation authority.
- Required final evidence: Separate authority record, privacy-safe evidence schema, result root, cleanup confirmation, and independent failure evidence.
- Proposed enablement operation: Specify future authority packaging and Evidence/Cleanup contract.
- Operation classifications: Repository documentation mutation; future Evidence capture/persistence.
- Exact scope: Project, Package, Restore, Build, Clipboard Read/Write/Clear, Runtime, Evidence, History/Cloud, privacy, and cleanup separation.
- Explicit exclusions: No Evidence Artifact, Result directory, payload, source code, runtime, Clipboard operation, setting mutation, or authority grant.
- Official-source lookup required: No for this specification.
- Local read-only inspection required: No for this specification.
- Network access required: No.
- Package acquisition required: No.
- Installation required: No.
- Repository mutation required: Only this document.
- Experimental asset required: No.
- Consumer asset required: No.
- Experimental Project required: No.
- Restore required: No.
- Build required: No.
- Clipboard read required: No.
- Clipboard write required: No.
- Clipboard clear required: No.
- Runtime execution required: No.
- Evidence persistence required: No.
- History/Cloud setting mutation required: No.
- Administrator privilege required: No.
- Human authorization required: Yes; every operation must be independently scoped.
- Expected files/directories: No result, evidence, or cleanup directory.
- Expected Package/Cache effects: None.
- Expected machine effects: None.
- Expected Clipboard effect: None.
- Privacy impact: None; no payload is persisted or observed.
- Risk classification: R1 documentation; future evidence/runtime is R4.
- Failure impact: No closure action can be submitted as executable.
- Stop conditions: Any evidence write, result directory creation, runtime, build, or Clipboard access.
- Rollback/cleanup requirement: No machine state changed; future cleanup must be part of an authorized execution record.
- Success condition: Future authorization review can distinguish specification, execution, and evidence persistence.
- Result artifact obligation: No result artifact in this specification.
- Resulting prerequisite recommendation: `CLIP-PREQ-031..032` Partially specified.
- Resulting blocker recommendation: `CLIP-BLOCK-012..013` Blocked.
- Resulting Pair recommendation: All pairs remain Blocked.
- Phase L1 effect: Authority and evidence boundary is specified but not enabled.
- Owner: Evidence owner TBD; Product owner TBD.
- Status: Blocked.
- Open questions: Who may grant each operation and what is the minimum persistent evidence set?

## 8. Operation Classification

| Classification | Risk |
|---|---|
| Official-source research | R0 |
| Local read-only inspection | R0 |
| Repository documentation mutation | R1 |
| Synthetic image specification | R1 |
| Experimental asset creation | R1 |
| Consumer asset creation | R1 |
| Experimental Project creation | R1 |
| Package acquisition | R2 |
| Package Restore | R2 |
| Build execution | R2 |
| Development environment installation | R3 |
| Clipboard read | R4 |
| Clipboard write | R4 |
| Clipboard clear | R4 |
| Runtime execution | R4 |
| Evidence persistence | R4 |
| History/Cloud setting mutation | R4 |

規則：

- 本文件只能規格化未來操作，不得執行。
- 一項操作涉及多層級時採最高風險。
- Project creation 不等於 Restore。
- Restore 不等於 Build。
- Build 不等於 Run。
- Run 不等於 Clipboard Write。
- Clipboard Write 不等於 Clipboard Read 或 Clear。
- Runtime 不等於 Evidence persistence。
- History/Cloud mutation 必須維持獨立授權。

## 9. Shared UI Evidence and Authority Matrix

| Shared capability | Existing UI research source | Authority artifact found | Authority reference | Reusable evidence | Clipboard-specific extension | Current effect |
|---|---|---|---|---|---|---|
| Windows 11 x64 baseline | `RESEARCH-TECH-UI-001` | No | TBD | Host baseline terms | Clipboard host identity | Blocked |
| WPF experimental build path | `RESEARCH-TECH-UI-001` | No | TBD | WPF host boundary | Candidate Host binding | Blocked |
| WinUI 3 experimental build path | `RESEARCH-TECH-UI-001` | No | TBD | WinUI 3 host boundary | Candidate Host binding | Blocked |
| .NET SDK/Runtime | `RESEARCH-TECH-UI-001`; `RESEARCH-TECH-CAPTURE-001` | No | TBD | Target identity terms | Project target | Blocked |
| Windows SDK | `RESEARCH-TECH-UI-001` | No | TBD | Platform baseline terms | Native interop scope | Blocked |
| Windows App SDK | `RESEARCH-TECH-UI-001` | No | TBD | Package boundary terms | WinRT candidate scope | Blocked |
| Experimental Project isolation | `ARCH-0002`; `ARCH-0004` | No | TBD | Responsibility boundary | Clipboard experiment path | Blocked |
| Package acquisition | `AGENTS.md` | No | TBD | No-install rule | Future package identity | Blocked |
| Restore | `AGENTS.md` | No | TBD | Separate authority rule | Future dependency restore | Blocked |
| Build | `AGENTS.md`; `ARCH-0001` | No | TBD | Separate build authority | Future experimental build | Blocked |
| Packaged/unpackaged mode | `ADR-0002`; `RESEARCH-TECH-UI-001` | No | TBD | Comparison boundary | Pair enablement | Blocked |
| Evidence root policy | `ARCH-0005`; `RESEARCH-TECH-CAPTURE-001` | No | TBD | Privacy boundary | Clipboard evidence root | Blocked |
| Privacy/cleanup | `ARCH-0005` | No | TBD | Failure and cleanup terms | Payload cleanup | Blocked |
| Runtime authority | `AGENTS.md`; `RESEARCH-TECH-UI-001` | No | TBD | Runtime boundary | Clipboard operation runtime | Blocked |

`CLIP-ENABLE-GAP-001` is required because shared authority evidence exists but no Shared UI authority artifact exists.

## 10. Rendering/Capture Handoff Boundary

| Clipboard input requirement | Rendering/Capture source | Required immutable metadata | Clipboard responsibility | Explicitly excluded responsibility | Remaining gap |
|---|---|---|---|---|---|
| Physical pixel dimensions | `RESEARCH-TECH-RENDER-001`; `RESEARCH-TECH-CAPTURE-001` | Width, height, identity | Publish scoped representation | No pixel generation | Values TBD |
| Pixel format | `RESEARCH-TECH-RENDER-001` | Format identity | Preserve/describe transport format | No rendering selection | Conversion TBD |
| Alpha mode | `RESEARCH-TECH-RENDER-001` | Straight/premultiplied term | Carry format-specific semantics | No alpha policy ownership | Method TBD |
| Premultiplication state | `RESEARCH-TECH-RENDER-001` | Explicit state | Record representation | No conversion claim | Evidence TBD |
| Row stride | `RESEARCH-TECH-CLIPBOARD-002` | Stride and header relation | Preserve format contract | No memory layout result | Values TBD |
| Color-space metadata | `RESEARCH-TECH-RENDER-001` | Color metadata | Preserve/record metadata | No HDR policy selection | SDR substitute TBD |
| Final rendered image identity | `ARCH-0003`; `ARCH-0005` | Image identity and source boundary | Accept scoped handoff | No Capture rerun | Handoff contract TBD |
| Synthetic Image identity | `RESEARCH-TECH-CLIPBOARD-004` | Run-independent identity | Consume future approved input | No asset creation here | Asset authority absent |
| Clipboard publication result | `SPEC-0007`; `SPEC-0010` | Operation/result separation | Own Clipboard publication | No shared workflow mutation | Runtime evidence absent |
| Parallel File Output result | `ARCH-0003`; `ARCH-0005` | Independent result identity | Remain parallel | No Clipboard failure rerun | Observer TBD |

正式 Capture output不得作為 Phase L1 Clipboard Spike 輸入。Synthetic Image 與正式產品 Render pipeline 分離；Clipboard failure 不得重跑 Capture 或 Rendering；Clipboard adapter 不得修改 Shared Workflow State；Clipboard 與 File Output 成功/失敗保持平行獨立。本文件不選擇 Rendering 或 Capture Technology。

## 11. Candidate Experimental Identity Baseline

| Candidate | Host | Exact API/Interop identity | Experimental identity/version | COM/STA | Dispatcher | Packaging | Local availability | Build verified | Runtime verified |
|---|---|---|---|---|---|---|---|---|---|
| `OPT-001` WPF Clipboard/IDataObject | WPF | WPF Clipboard / `IDataObject` | TBD | Host STA evidence required | WPF Dispatcher | TBD | Unknown | No | No |
| `OPT-001` WPF Clipboard/IDataObject | WinUI 3 | WPF Clipboard adapter | TBD | Adapter STA evidence required | WinUI 3 Dispatcher | TBD | Unknown | No | No |
| `OPT-002` WinRT Clipboard/DataPackage | WPF | WinRT Clipboard adapter | TBD | Apartment evidence required | WPF Dispatcher | TBD | Unknown | No | No |
| `OPT-002` WinRT Clipboard/DataPackage | WinUI 3 | WinRT Clipboard / `DataPackage` | TBD | Apartment evidence required | WinUI 3 Dispatcher | TBD | Unknown | No | No |
| `OPT-003` OLE Clipboard/COM IDataObject | WPF | OLE / COM `IDataObject` | TBD | Explicit COM/STA | WPF Dispatcher | TBD | Unknown | No | No |
| `OPT-003` OLE Clipboard/COM IDataObject | WinUI 3 | OLE / COM adapter | TBD | Explicit COM/STA | WinUI 3 Dispatcher | TBD | Unknown | No | No |
| `OPT-004` Raw Win32 Clipboard API | WPF | Raw Win32 Clipboard APIs | TBD | Native thread/COM evidence | WPF Dispatcher if bound | TBD | Unknown | No | No |
| `OPT-004` Raw Win32 Clipboard API | WinUI 3 | Raw Win32 native adapter | TBD | Native thread/COM evidence | WinUI 3 Dispatcher if bound | TBD | Unknown | No | No |
| `OPT-005` Host-neutral adapter strategy | WPF | Adapter composition, not API authority | TBD | Delegated | WPF Dispatcher | TBD | Unknown | No | No |
| `OPT-005` Host-neutral adapter strategy | WinUI 3 | Adapter composition, not API authority | TBD | Delegated | WinUI 3 Dispatcher | TBD | Unknown | No | No |

API identity 與 Local availability 分開。所有 Experimental identity 未定時使用 `TBD`；所有 Build verified = No；所有 Runtime verified = No；Adapter strategy 只列出組成，不形成 Candidate ranking 或 selection。

## 12. Candidate–Host Enablement Matrix

| Pair | Candidate | Host | Current readiness | Shared dependency | Clipboard-specific dependency | Required operation | Enablement Item | Target recommendation |
|---|---|---|---|---|---|---|---|---|
| `CLIP-PAIR-001` | WPF Clipboard | WPF | Conditionally eligible; blocked | Shared UI Host | WPF API, STA, isolation | Future Project/Build/Write | `CLIP-ENABLE-001..006` | Partially specified |
| `CLIP-PAIR-002` | WinRT Clipboard | WinUI 3 | Conditionally eligible; blocked | Shared UI Host | DataPackage, apartment, package | Future Project/Build/Write | `CLIP-ENABLE-001..006` | Partially specified |
| `CLIP-PAIR-003` | OLE Clipboard | WPF | Conditionally eligible; blocked | Shared UI Host | COM/STA, ownership | Future Project/Build/Write | `CLIP-ENABLE-001..006` | Partially specified |
| `CLIP-PAIR-004` | Raw Win32 Clipboard | WPF | Conditionally eligible; blocked | Shared UI Host | Native API, handle, cleanup | Future Project/Build/Write | `CLIP-ENABLE-001..006` | Partially specified |
| `CLIP-PAIR-005` | WPF Clipboard adapter | WinUI 3 | Unknown | Shared UI Host | Cross-host adapter | Future isolated adapter Project | `CLIP-ENABLE-001..006` | Deferred L2 |
| `CLIP-PAIR-006` | WinRT Clipboard adapter | WPF | Unknown | Shared UI Host | Cross-host projection | Future isolated adapter Project | `CLIP-ENABLE-001..006` | Deferred L2 |
| `CLIP-PAIR-007` | OLE Clipboard | WinUI 3 | Conditionally eligible; blocked | Shared UI Host | COM/STA adapter | Future Project/Build/Write | `CLIP-ENABLE-001..006` | Partially specified |
| `CLIP-PAIR-008` | Raw Win32 Clipboard | WinUI 3 | Conditionally eligible; blocked | Shared UI Host | Native adapter | Future Project/Build/Write | `CLIP-ENABLE-001..006` | Partially specified |
| `CLIP-PAIR-009` | Host-neutral adapter | WPF | Conditionally eligible; blocked | Shared UI Host | Adapter contract, STA | Future isolated adapter Project | `CLIP-ENABLE-001..006` | Partially specified |
| `CLIP-PAIR-010` | Host-neutral adapter | WinUI 3 | Conditionally eligible; blocked | Shared UI Host | Adapter contract, apartment | Future isolated adapter Project | `CLIP-ENABLE-001..006` | Partially specified |

Unknown 不得直接變成 Excluded with evidence。Deferred Pair 必須有 L2/L3 reactivation condition。所有 Pair execution authority 維持 No；不得以 Framework API 存在形成 Candidate ranking。

## 13. Repository Isolation Boundary

本文件只規劃、不建立：

- `experiments/clipboard/<host>/<candidate>/`
- `docs/Research/Technology/results/clipboard/`

規定：

- Experimental Project 不得位於產品 Source tree。
- Experimental Project 不得被產品 Project reference。
- Experimental Project 不得成為正式 Architecture component。
- Synthetic Image、Consumer、Payload、Build output 與 Result 必須可區分。
- 每個 Host/Candidate 使用隔離路徑。
- 所有未來新增項目必須有 Cleanup Manifest。
- 本文件不得建立上述目錄。

## 14. Project/Package/Restore/Build Separation

### 14.1 Experimental Project

| Field | Required specification |
|---|---|
| Host | WPF or WinUI 3 identity, not selected here |
| Candidate | One scoped Candidate identity, not ranked here |
| Target framework | TBD until Shared UI authority exists |
| Process architecture | x64 baseline candidate, not locally verified |
| Packaged/unpackaged mode | Explicit comparison field, not selected |
| Repository path | Isolated experimental path only |
| Apartment model | Explicit per Candidate/Host |
| Dispatcher model | Explicit per Host |
| Native/COM interop boundary | Adapter-owned and separately recorded |
| Minimal contents | Only the future Spike contract |
| Prohibited product content | Product source, workflow state, user payload, screenshot feature |

### 14.2 Package Acquisition and Restore

| Field | Required specification |
|---|---|
| Package/SDK identity | Exact identity required later |
| Experimental version | Explicit version required later |
| Package source | Explicit source required later |
| Expected dependencies | Recorded before acquisition |
| Native asset implications | Recorded before Restore |
| Cache effects | Recorded before Restore; no cache query here |
| Offline limitation | Recorded before acquisition |
| Rollback limitation | Recorded before acquisition |
| Authorization | Package acquisition and Restore are separate and not granted |

### 14.3 Build Verification

| Field | Required specification |
|---|---|
| Build tool | Exact tool required later |
| Configuration | Explicit configuration required later |
| Architecture | Explicit architecture required later |
| Packaging mode | Explicit mode required later |
| Expected outputs | Isolated output path required later |
| Required logs | Future evidence schema only |
| Exit-code handling | Future execution record only |
| Cleanup requirements | Cleanup Manifest required later |

固定：Clipboard read permitted = No；Clipboard write permitted = No；Clipboard clear permitted = No；Run permitted = No；Runtime execution permitted = No；Evidence persistence permitted = No。

## 15. Clipboard Isolation Enablement

| Isolation capability | Existing specification | Required final specification | Future environment | Clipboard operation dependency | Enablement Item | Remaining gap |
|---|---|---|---|---|---|---|
| Dedicated test user/VM/isolated session | Planned | Owner, boundary, pre/post policy | Isolated account/VM/session | All operations | `CLIP-ENABLE-003` | Mode TBD |
| Existing Clipboard state policy | No-read boundary | Synthetic-only precondition | Isolated environment | Read/Write | `CLIP-ENABLE-003` | Proof method TBD |
| Read prohibition | Not granted | Independent Read permission = No | Future record | Read | `CLIP-ENABLE-003` | Authority artifact absent |
| Clear prohibition | Not granted | Independent Clear permission = No | Future record | Clear | `CLIP-ENABLE-003` | Authority artifact absent |
| Overwrite consent | Not granted | Explicit Write consent | Future isolated session | Write | `CLIP-ENABLE-003` | Human owner TBD |
| Residual payload cleanup | Planned | Cleanup Manifest and confirmation | Future isolated session | Write/Clear | `CLIP-ENABLE-003`, `006` | Method TBD |
| Process termination cleanup | Deferred | Normal/abnormal cleanup contract | Future lifecycle test | Runtime | `CLIP-ENABLE-006` | Process owner TBD |
| History disabled branch | Deferred | Observe without setting mutation | Future isolated state | Runtime | `CLIP-ENABLE-003`, `006` | State authority absent |
| History enabled branch | Deferred | Separate state observation | Future isolated state | Runtime | `CLIP-ENABLE-003`, `006` | Account boundary TBD |
| Cloud disabled branch | Deferred | Observe without setting mutation | Future isolated device | Runtime | `CLIP-ENABLE-003`, `006` | Device authority absent |
| Cloud enabled isolated branch | Deferred | Separate account/device policy | Future isolated device | Runtime | `CLIP-ENABLE-003`, `006` | Sync boundary TBD |
| Test account/device boundary | Not specified | Named isolated owner and cleanup | Future isolated environment | Runtime | `CLIP-ENABLE-003` | Owner TBD |
| Consumer boundary | Candidate list exists | Approved synthetic-only Consumer | Future isolated consumer | Write/Runtime | `CLIP-ENABLE-005` | Consumer TBD |
| Private-data detection | Prohibited user-content access | Privacy stop and review | Future isolated environment | All operations | `CLIP-ENABLE-003`, `006` | Review owner TBD |
| Failure stop enforcement | Stop rule planned | Explicit stop record | Future authorized run | Runtime/Evidence | `CLIP-ENABLE-006` | Authority absent |

本文件不得建立或啟動隔離環境。

## 16. Synthetic Image Enablement

| Capability | Existing definition | Future asset | Creation required | Runtime required | Evidence required | Enablement Item |
|---|---|---|---|---|---|---|
| Fixed dimensions | Planned | Yes | Yes | Yes | Yes | `CLIP-ENABLE-004` |
| Small/normal/large classes | Planned | Yes | Yes | Yes | Yes | `CLIP-ENABLE-004` |
| Opaque/transparent regions | Planned | Yes | Yes | Yes | Yes | `CLIP-ENABLE-004` |
| Alpha gradient | Planned | Yes | Yes | Yes | Yes | `CLIP-ENABLE-004` |
| Premultiplied/straight Alpha references | Planned | Yes | Yes | Yes | Yes | `CLIP-ENABLE-004` |
| One-pixel border | Planned | Yes | Yes | Yes | Yes | `CLIP-ENABLE-004` |
| Corner/center markers | Planned | Yes | Yes | Yes | Yes | `CLIP-ENABLE-004` |
| RGB/grayscale | Planned | Yes | Yes | Yes | Yes | `CLIP-ENABLE-004` |
| Mixed-language text | Planned | Yes | Yes | Yes | Yes | `CLIP-ENABLE-004` |
| Fine-line pattern | Planned | Yes | Yes | Yes | Yes | `CLIP-ENABLE-004` |
| Known pixel coordinates | Planned | Yes | Yes | Yes | Yes | `CLIP-ENABLE-004` |
| SDR block | Planned | Yes | Yes | Yes | Yes | `CLIP-ENABLE-004` |
| Wide-color substitute metadata | Planned | Yes | Yes | Yes | Yes | `CLIP-ENABLE-004` |
| Synthetic run identifier | Planned | Yes | Yes | Yes | Yes | `CLIP-ENABLE-004` |

本文件不得建立 Bitmap、PNG、DIB 或 Payload。

## 17. Format and Consumer Enablement

### 17.1 Format Matrix

| Format | Producer representation | Consumer representation | Alpha method | Pixel/color method | Lifetime method | Project requirement | Runtime requirement | Evidence requirement | Enablement Item | Remaining gap |
|---|---|---|---|---|---|---|---|---|---|---|
| Framework Bitmap | TBD | WPF/WinUI test | TBD | TBD | TBD | Future | Future | Future | `CLIP-ENABLE-005` | Representation |
| `CF_BITMAP` | Native bitmap handle | Win32/OLE | TBD | TBD | Handle ownership | Future | Future | Future | `CLIP-ENABLE-005` | Alpha/handle |
| `CF_DIB` | DIB header/pixels | Win32/OLE | TBD | Stride/header | Data lifetime | Future | Future | Future | `CLIP-ENABLE-005` | Header semantics |
| `CF_DIBV5` | DIBV5 header/pixels | Win32/OLE | TBD | Mask/color metadata | Data lifetime | Future | Future | Future | `CLIP-ENABLE-005` | Color semantics |
| PNG registered format | PNG stream | Isolated decoder | TBD | Decoded comparison | Stream lifetime | Future | Future | Future | `CLIP-ENABLE-005` | Registration/decoder |
| OLE `IDataObject` | Format collection | OLE consumer | Per format | Per format | Ownership/delay | Future | Future | Future | `CLIP-ENABLE-005` | Enumeration |
| WinRT `DataPackage` | DataPackage | WinUI/isolated consumer | Per representation | Per representation | Projection lifetime | Future | Future | Future | `CLIP-ENABLE-005` | Projection |
| Multi-format publication | Multiple representations | Scoped consumer set | Per format | Per format | Atomicity/selection | Future | Future | Future | `CLIP-ENABLE-005` | Selection |

### 17.2 Consumer Matrix

| Consumer | Consumer boundary | Project requirement | Runtime requirement | Evidence requirement | Enablement Item | Remaining gap |
|---|---|---|---|---|---|---|
| WPF test consumer | Isolated synthetic-only | Future | Future | Future | `CLIP-ENABLE-005` | Identity TBD |
| WinUI 3 test consumer | Isolated synthetic-only | Future | Future | Future | `CLIP-ENABLE-005` | Identity TBD |
| Win32/OLE test consumer | Separate process | Future | Future | Future | `CLIP-ENABLE-005` | Lifetime TBD |
| Basic image-editor class | Isolated application class | Future | Future | Future | `CLIP-ENABLE-005` | Application TBD |
| Office-style consumer class | Deferred privacy boundary | Future | Future | Future | `CLIP-ENABLE-005` | Installation boundary |
| Browser class | Deferred session boundary | Future | Future | Future | `CLIP-ENABLE-005` | Browser boundary |
| Clipboard History surface | Deferred platform state | Future | Future | Future | `CLIP-ENABLE-005`, `006` | Account/state |
| Cloud Clipboard surface | Deferred sync state | Future | Future | Future | `CLIP-ENABLE-005`, `006` | Device/account |

不得開啟、操作或建立任何 Consumer。

## 18. Threading/COM Enablement

| Scenario | Apartment requirement | Dispatcher requirement | COM initialization | Project required | Runtime required | Evidence required | Enablement Item |
|---|---|---|---|---|---|---|---|
| WPF UI STA | Explicit STA | WPF Dispatcher | Host-specific | Future | Future | Future | `CLIP-ENABLE-002`, `006` |
| WPF background STA | Explicit STA | Marshal to UI | Host-specific | Future | Future | Future | `CLIP-ENABLE-002`, `006` |
| WPF background MTA | Explicit MTA | Marshal to UI | Explicit boundary | Future | Future | Future | `CLIP-ENABLE-002`, `006` |
| WinUI 3 UI thread | Host apartment | WinUI Dispatcher | Projection-specific | Future | Future | Future | `CLIP-ENABLE-001`, `006` |
| WinUI 3 background thread | Explicit background apartment | Marshal to UI | Projection-specific | Future | Future | Future | `CLIP-ENABLE-002`, `006` |
| OLE with COM initialized | Explicit COM/STA | Host-specific | Required | Future | Future | Future | `CLIP-ENABLE-002`, `006` |
| OLE without required initialization | Failure observation only | Host-specific | Missing-state observation | Future | Future | Future | `CLIP-ENABLE-006` |
| Dispatcher shutdown | Host shutdown contract | Explicit shutdown order | Cleanup responsibility | Future | Future | Future | `CLIP-ENABLE-006` |
| Application shutdown during publication | Lifecycle boundary | Dispatcher termination | Ownership boundary | Future | Future | Future | `CLIP-ENABLE-006` |
| Cancellation during retry | Explicit cancellation contract | Dispatcher-safe cancel | COM cleanup | Future | Future | Future | `CLIP-ENABLE-006` |

官方文件描述不得視為 Runtime 通過。

## 19. Evidence Method Enablement

| Evidence capability | Planned method | Clipboard operation required | Persistence required | Privacy classification | Authorization class | Enablement Item |
|---|---|---|---|---|---|---|
| Environment record | Future isolated record | No | Future | Synthetic metadata only | R0/R1 | `CLIP-ENABLE-001`, `006` |
| Synthetic Image specification | Documentary specification | No | No | Non-sensitive | R1 | `CLIP-ENABLE-004` |
| Producer payload metadata | Future synthetic metadata | Write | Future | Synthetic only | R4 | `CLIP-ENABLE-005`, `006` |
| Format enumeration | Future isolated observation | Read/Write | Future | Synthetic only | R4 | `CLIP-ENABLE-005`, `006` |
| Consumer observation | Future isolated consumer | Read/Write | Future | Synthetic only | R4 | `CLIP-ENABLE-005`, `006` |
| Pixel comparison | Future deterministic comparison | Read/Write | Future | Synthetic only | R4 | `CLIP-ENABLE-004`, `005`, `006` |
| Alpha comparison | Future deterministic comparison | Read/Write | Future | Synthetic only | R4 | `CLIP-ENABLE-004`, `005`, `006` |
| Color metadata | Future format observation | Read/Write | Future | Synthetic only | R4 | `CLIP-ENABLE-005`, `006` |
| Thread/Apartment record | Future isolated observation | Write | Future | Diagnostic | R4 | `CLIP-ENABLE-002`, `006` |
| Dispatcher observation | Future isolated observation | Write | Future | Diagnostic | R4 | `CLIP-ENABLE-001`, `006` |
| Contention failure | Future synthetic owner | Write | Future | Synthetic only | R4 | `CLIP-ENABLE-003`, `006` |
| Retry timing | Future authorized observation | Write | Future | Diagnostic | R4 | `CLIP-ENABLE-006` |
| Ownership/lifetime | Future process observation | Write | Future | Diagnostic | R4 | `CLIP-ENABLE-006` |
| Process termination | Future lifecycle observation | Write | Future | Diagnostic | R4 | `CLIP-ENABLE-006` |
| Memory observation | Future isolated resource observation | Write | Future | Diagnostic | R4 | `CLIP-ENABLE-006` |
| History/Cloud observation | Future isolated platform observation | Read/Write | Future | High privacy | R4 | `CLIP-ENABLE-003`, `006` |
| Parallel File Output result | Future independent observer | No Clipboard authority required | Future | Synthetic only | R1/R4 | `CLIP-ENABLE-006` |
| Diagnostic log | Future execution record | Operation-specific | Future | Privacy reviewed | R4 | `CLIP-ENABLE-006` |
| Privacy review | Human review record | No | Future | Privacy control | R1/R4 | `CLIP-ENABLE-003`, `006` |
| Cleanup confirmation | Future Cleanup Manifest | Write/Clear | Future | Synthetic only | R4 | `CLIP-ENABLE-003`, `006` |

Runtime 與 Persistent Evidence 均不在本輪授權範圍。不得建立實際 Evidence；不得自行設定 Retry、Timeout、Memory、Pixel 或 Alpha 門檻；Session observation 不等於 Persistent Evidence。

## 20. Phase L1 Enablement Gates

| Closure Gate | Required specification | Related Enablement Items | Current specification status | Remaining gap |
|---|---|---|---|---|
| `CLIP-CGATE-001` | Shared WPF/WinUI 3 Host build dependency and authority path | `CLIP-ENABLE-001` | Blocked | Shared authority artifact |
| `CLIP-CGATE-002` | One exact Clipboard Candidate API/Interop identity | `CLIP-ENABLE-002` | Partially specified | Experimental identity |
| `CLIP-CGATE-003` | Candidate–Host Project, COM, Dispatcher boundary | `CLIP-ENABLE-001`, `002` | Partially specified | Host/project record |
| `CLIP-CGATE-004` | Isolation and existing-content protection policy | `CLIP-ENABLE-003` | Blocked | Isolation authority |
| `CLIP-CGATE-005` | Basic Synthetic Image fully specified | `CLIP-ENABLE-004` | Partially specified | Approved values |
| `CLIP-CGATE-006` | Bitmap, DIB/DIBV5, PNG, multi-format methods | `CLIP-ENABLE-005` | Partially specified | Format method |
| `CLIP-CGATE-007` | Consumer interoperability and Alpha/pixel method | `CLIP-ENABLE-005` | Blocked | Consumer identity/method |
| `CLIP-CGATE-008` | Read/Write/Clear/Runtime/Evidence authority separated | `CLIP-ENABLE-003`, `006` | Blocked | Separate authority records |
| `CLIP-CGATE-009` | Result storage, privacy, cleanup boundary | `CLIP-ENABLE-003`, `006` | Blocked | Evidence/cleanup schema |
| `CLIP-CGATE-010` | Clipboard/File Output failure independent | `CLIP-ENABLE-005`, `006` | Partially specified | Failure evidence |
| `CLIP-CGATE-011` | Runtime remains later separate authorization | `CLIP-ENABLE-006` | Specified | Future authorization review |

Status 只能使用 `Specified`、`Partially specified`、`Blocked`、`Deferred` 或 `Not applicable`。不得使用 `Satisfied`、`Passed` 或 `Resolved`。

## 21. Authorization Packaging Matrix

| Enablement Item | Operation classifications | Highest risk | Shared UI authority dependency | Clipboard-specific authority required | Current authorization | Execution permitted |
|---|---|---|---|---|---|---|
| `CLIP-ENABLE-001` | Project/Package/Restore/Build/Runtime | R4 | Required; artifact not found | Host-bound Clipboard authority later | Not granted | No |
| `CLIP-ENABLE-002` | Project/Package/Restore/Build/Clipboard/Runtime | R4 | Required; artifact not found | Candidate operation authority later | Not granted | No |
| `CLIP-ENABLE-003` | Clipboard read/write/clear/Runtime | R4 | Required; artifact not found | Isolation and per-operation authority | Not granted | No |
| `CLIP-ENABLE-004` | Synthetic image specification/asset | R4 when published | Required for Host-bound run | Asset and later Clipboard authority | Not granted | No |
| `CLIP-ENABLE-005` | Consumer/Clipboard/Runtime/Evidence | R4 | Required; artifact not found | Consumer and format authority | Not granted | No |
| `CLIP-ENABLE-006` | Evidence/Runtime/History/Cloud | R4 | Required; artifact not found | Independent operation/evidence authority | Not granted | No |

固定：Current authorization = `Not granted`；Execution permitted = `No`。本文件不得建立 `CLIP-AUTH`、Clipboard Read/Write/Clear authorization、Runtime authorization、Evidence Write authorization 或 History/Cloud mutation authorization。

## 22. Enablement Completeness Matrix

| Blocking Action | Closure Action | Enablement Item | Specification complete | Shared authority identified | Clipboard authority identified | Evidence obligation identified | Remaining gap |
|---|---|---|---|---|---|---|---|
| `CLIP-BA-001` | `CLIP-CLOSE-001` | `CLIP-ENABLE-001` | Partially | No | No | Partially | `CLIP-ENABLE-GAP-001` |
| `CLIP-BA-002` | `CLIP-CLOSE-002` | `CLIP-ENABLE-002` | Partially | No | No | Partially | Candidate identity |
| `CLIP-BA-003` | `CLIP-CLOSE-003` | `CLIP-ENABLE-003` | Partially | No | No | Partially | Isolation authority |
| `CLIP-BA-004` | `CLIP-CLOSE-004` | `CLIP-ENABLE-004` | Partially | No | No | Partially | Synthetic values/asset authority |
| `CLIP-BA-005` | `CLIP-CLOSE-005` | `CLIP-ENABLE-005` | Partially | No | No | Partially | Format/Consumer method |
| `CLIP-BA-006` | `CLIP-CLOSE-006` | `CLIP-ENABLE-006` | Partially | No | No | Partially | Evidence and authority records |

`Specification complete = Yes` 只代表規格可進入後續 Authorization Request；不代表已授權或已執行。本文件目前沒有任何 `Yes`。

## 23. Full Impact and Coverage Index

| Source item | Related Enablement Item | Remaining evidence class | Phase L1 impact | Recommendation |
|---|---|---|---|---|
| `CLIP-PREQ-001..003` | `CLIP-ENABLE-001` | Shared Host/authority | Required | Partially specified |
| `CLIP-PREQ-004..008` | `CLIP-ENABLE-002` | Candidate API/Interop | Required | Partially specified |
| `CLIP-PREQ-009..012` | `CLIP-ENABLE-001`, `002` | Thread/COM/package | Required/deferred details | Deferred details |
| `CLIP-PREQ-013..015` | `CLIP-ENABLE-003` | Isolation/operation authority | Required | Blocked |
| `CLIP-PREQ-016` | `CLIP-ENABLE-004` | Synthetic Image | Required | Partially specified |
| `CLIP-PREQ-017..025` | `CLIP-ENABLE-005` | Format/Consumer/fidelity | Required | Blocked |
| `CLIP-PREQ-026..030` | `CLIP-ENABLE-003`, `006` | Contention/retry/lifetime/memory/History | Deferred | Deferred |
| `CLIP-PREQ-031..032` | `CLIP-ENABLE-006` | Evidence/authority | Required | Blocked |
| `CLIP-BLOCK-001..002` | `CLIP-ENABLE-001` | Shared Host | Required | Blocked |
| `CLIP-BLOCK-003` | `CLIP-ENABLE-002` | Candidate identity | Required | Blocked |
| `CLIP-BLOCK-004..005` | `CLIP-ENABLE-001`, `002` | Thread/package | L1/L2 | Deferred details |
| `CLIP-BLOCK-006` | `CLIP-ENABLE-003` | Isolation | Required | Blocked |
| `CLIP-BLOCK-007` | `CLIP-ENABLE-004` | Synthetic Image | Required | Blocked |
| `CLIP-BLOCK-008..009` | `CLIP-ENABLE-005` | Format/Consumer | Required | Blocked |
| `CLIP-BLOCK-010..011` | `CLIP-ENABLE-003`, `006` | Runtime/platform state | L2/L3 | Deferred |
| `CLIP-BLOCK-012..013` | `CLIP-ENABLE-006` | Evidence/authority | Required | Blocked |
| `CLIP-PAIR-001..004` | `CLIP-ENABLE-001..006` | Host-specific evidence | Required | Blocked |
| `CLIP-PAIR-005..006` | `CLIP-ENABLE-001..006` | Cross-host adapter | L2 | Deferred |
| `CLIP-PAIR-007..010` | `CLIP-ENABLE-001..006` | Native/adapter evidence | L1/L2 | Blocked |
| `CLIP-SPIKE-001..005` | `CLIP-ENABLE-001..006` | Host/format/Consumer | Required | Blocked |
| `CLIP-SPIKE-006..009` | `CLIP-ENABLE-003`, `006` | Thread/retry/lifetime/memory | L2 | Deferred |
| `CLIP-SPIKE-010` | `CLIP-ENABLE-003`, `006` | History/Cloud | L3 | Deferred |
| `CLIP-SPIKE-011..012` | `CLIP-ENABLE-001`, `006` | Package/File Output | L2 | Deferred |
| `CLIP-BA-001` | `CLIP-ENABLE-001` | Shared Host | Required | Blocked |
| `CLIP-BA-002` | `CLIP-ENABLE-002` | Candidate identity | Required | Blocked |
| `CLIP-BA-003` | `CLIP-ENABLE-003` | Isolation | Required | Blocked |
| `CLIP-BA-004` | `CLIP-ENABLE-004` | Synthetic Image | Required | Blocked |
| `CLIP-BA-005` | `CLIP-ENABLE-005` | Format/Consumer | Required | Blocked |
| `CLIP-BA-006` | `CLIP-ENABLE-006` | Evidence/authority | Required | Blocked |
| `CLIP-CLOSE-001..006` | `CLIP-ENABLE-001..006` | Future authorization input | Required | Not ready |

本矩陣只能提出 Recommendation，不得修改上游狀態。

### 23.1 Exact Coverage Ledger

| Identifier family | Explicit identifiers |
|---|---|
| Prerequisites | `CLIP-PREQ-001`, `CLIP-PREQ-002`, `CLIP-PREQ-003`, `CLIP-PREQ-004`, `CLIP-PREQ-005`, `CLIP-PREQ-006`, `CLIP-PREQ-007`, `CLIP-PREQ-008`, `CLIP-PREQ-009`, `CLIP-PREQ-010`, `CLIP-PREQ-011`, `CLIP-PREQ-012`, `CLIP-PREQ-013`, `CLIP-PREQ-014`, `CLIP-PREQ-015`, `CLIP-PREQ-016`, `CLIP-PREQ-017`, `CLIP-PREQ-018`, `CLIP-PREQ-019`, `CLIP-PREQ-020`, `CLIP-PREQ-021`, `CLIP-PREQ-022`, `CLIP-PREQ-023`, `CLIP-PREQ-024`, `CLIP-PREQ-025`, `CLIP-PREQ-026`, `CLIP-PREQ-027`, `CLIP-PREQ-028`, `CLIP-PREQ-029`, `CLIP-PREQ-030`, `CLIP-PREQ-031`, `CLIP-PREQ-032` |
| Blockers | `CLIP-BLOCK-001`, `CLIP-BLOCK-002`, `CLIP-BLOCK-003`, `CLIP-BLOCK-004`, `CLIP-BLOCK-005`, `CLIP-BLOCK-006`, `CLIP-BLOCK-007`, `CLIP-BLOCK-008`, `CLIP-BLOCK-009`, `CLIP-BLOCK-010`, `CLIP-BLOCK-011`, `CLIP-BLOCK-012`, `CLIP-BLOCK-013` |
| Candidate–Host pairs | `CLIP-PAIR-001`, `CLIP-PAIR-002`, `CLIP-PAIR-003`, `CLIP-PAIR-004`, `CLIP-PAIR-005`, `CLIP-PAIR-006`, `CLIP-PAIR-007`, `CLIP-PAIR-008`, `CLIP-PAIR-009`, `CLIP-PAIR-010` |
| Spikes | `CLIP-SPIKE-001`, `CLIP-SPIKE-002`, `CLIP-SPIKE-003`, `CLIP-SPIKE-004`, `CLIP-SPIKE-005`, `CLIP-SPIKE-006`, `CLIP-SPIKE-007`, `CLIP-SPIKE-008`, `CLIP-SPIKE-009`, `CLIP-SPIKE-010`, `CLIP-SPIKE-011`, `CLIP-SPIKE-012` |
| Blocking actions | `CLIP-BA-001`, `CLIP-BA-002`, `CLIP-BA-003`, `CLIP-BA-004`, `CLIP-BA-005`, `CLIP-BA-006` |
| Closure actions | `CLIP-CLOSE-001`, `CLIP-CLOSE-002`, `CLIP-CLOSE-003`, `CLIP-CLOSE-004`, `CLIP-CLOSE-005`, `CLIP-CLOSE-006` |
| Enablement items | `CLIP-ENABLE-001`, `CLIP-ENABLE-002`, `CLIP-ENABLE-003`, `CLIP-ENABLE-004`, `CLIP-ENABLE-005`, `CLIP-ENABLE-006` |
| Closure gates | `CLIP-CGATE-001`, `CLIP-CGATE-002`, `CLIP-CGATE-003`, `CLIP-CGATE-004`, `CLIP-CGATE-005`, `CLIP-CGATE-006`, `CLIP-CGATE-007`, `CLIP-CGATE-008`, `CLIP-CGATE-009`, `CLIP-CGATE-010`, `CLIP-CGATE-011` |
| Upstream gates | `CLIP-GATE-001`, `CLIP-GATE-002`, `CLIP-GATE-003`, `CLIP-GATE-004`, `CLIP-GATE-005`, `CLIP-GATE-006`, `CLIP-GATE-007`, `CLIP-GATE-008`, `CLIP-GATE-009`, `CLIP-GATE-010` |

## 24. Final Enablement Status

Enablement status 只能使用 `Ready to request clipboard prerequisite closure execution authorization`、`Conditionally ready to request clipboard prerequisite closure execution authorization` 或 `Not ready to request clipboard prerequisite closure execution authorization`。

### 24.1 Mechanical Derivation

```text
  Open CLIP-ENABLE-GAP
    AND Shared UI authority artifact status
    AND Candidate/Host identity completeness
    AND Project/Package/Restore/Build scope
    AND Isolation completeness
    AND Synthetic Image completeness
    AND Format/Consumer completeness
    AND Threading/COM completeness
    AND Evidence/Privacy obligations
    AND R4 authority separation
  -> Final Enablement Status
```

本文件狀態：

- Open `CLIP-ENABLE-GAP`: Yes (`CLIP-ENABLE-GAP-001`)
- Shared UI authority artifact status: Not found / TBD
- Candidate/Host identity completeness: Partially specified
- Project/Package/Restore/Build scope: Partially specified
- Isolation completeness: Blocked
- Synthetic Image completeness: Partially specified
- Format/Consumer completeness: Blocked
- Threading/COM completeness: Partially specified
- Evidence/Privacy obligations: Blocked
- R4 authority separation: Not granted
- Final Enablement Status: `Not ready to request clipboard prerequisite closure execution authorization`

即使未來結果變成 Ready，也仍固定：

- Closure Execution Authorized: No
- Clipboard Runtime Spike Authorized: No
- Clipboard Read Authorized: No
- Clipboard Write Authorized: No
- Clipboard Clear Authorized: No
- Evidence Write Authorized: No
- Build Verification: Not performed
- Runtime Verification: Not performed
- Clipboard Decision: Not made
- Capture Decision: Not made
- Rendering Decision: Not made

## 25. Traceability

```text
CLIP-BA
  -> CLIP-CLOSE
  -> CLIP-ENABLE
  -> Shared UI evidence / missing authority artifact
  -> Clipboard-specific authority
  -> Future closure authorization request
  -> Future closure evidence
  -> Runtime Spike readiness reassessment
  -> Future Clipboard decision
```

引用：

- `RESEARCH-TECH-CLIPBOARD-001` — `docs/Research/Technology/29-clipboard-integration-feasibility.md`
- `RESEARCH-TECH-CLIPBOARD-002` — `docs/Research/Technology/30-clipboard-integration-runtime-spike-plan.md`
- `RESEARCH-TECH-CLIPBOARD-003` — `docs/Research/Technology/31-clipboard-integration-runtime-spike-execution-readiness.md`
- `RESEARCH-TECH-CLIPBOARD-004` — `docs/Research/Technology/32-clipboard-integration-prerequisite-closure-plan.md`
- `TD-004 Clipboard Integration` — `Architecture/TECHNOLOGY-DECISION-ROADMAP.md`
- `RESEARCH-TECH-UI-001` — `docs/Research/Technology/01-ui-framework-feasibility.md`
- `RESEARCH-TECH-RENDER-001` — `docs/Research/Technology/10-rendering-technology-feasibility.md`
- `RESEARCH-TECH-CAPTURE-001` — `docs/Research/Technology/20-capture-backend-feasibility.md`
- `ARCH-0001` — `Architecture/ARCH-0001-architecture-principles.md`
- `ARCH-0002` — `Architecture/ARCH-0002-layer-model.md`
- `ARCH-0003` — `Architecture/ARCH-0003-module-catalog.md`
- `ARCH-0004` — `Architecture/ARCH-0004-component-boundaries.md`
- `ARCH-0005` — `Architecture/ARCH-0005-component-interactions.md`
- `ADR-0002` — `Architecture/adr/ADR-0002-ui-framework-selection.md`
- `PRD-0005` — `PRD/PRD-0005-functional-requirements.md`
- `SPEC-0007` — `Specs/SPEC-0007-clipboard-handoff.md`
- `SPEC-0010` — `Specs/SPEC-0010-feature-integration.md`
- `AGENTS.md` — repository execution and authorization boundary

不得引用不存在的 `UI-AUTH-*`。

## 26. Completion Conditions

- 只建立 `docs/Research/Technology/33-clipboard-integration-prerequisite-execution-enablement-specification.md`。
- Document ID 固定為 `RESEARCH-TECH-CLIPBOARD-005`。
- 不修改任何其他文件。
- 建立正好六個 `CLIP-ENABLE-001..006`。
- 保持六組 `CLIP-BA -> CLIP-CLOSE -> CLIP-ENABLE` 一對一。
- 完整覆蓋 32 個 Prerequisite、13 個 Blocker、10 個 Pair、12 個 Spike、6 個 `CLIP-BA`、6 個 `CLIP-CLOSE` 與 11 個 `CLIP-CGATE`。
- 不虛構 `UI-AUTH-*`；Shared UI authority artifact 缺失必須明確標示並建立 `CLIP-ENABLE-GAP-001`。
- 區分 Project、Package acquisition、Restore、Build、Clipboard Read/Write/Clear、Runtime 及 Evidence persistence。
- 建立 Isolation、Synthetic Image、Format/Consumer、Threading/COM 及 Evidence enablement 規格。
- 所有 Current authorization = Not granted。
- 所有 Execution permitted = No。
- 不建立 Authorization Request 或 Human Decision。
- 不讀取、寫入、清除或備份 Clipboard。
- 不執行官方研究、本機盤點、下載、安裝、Restore、Build、Run、Test 或 Runtime Spike。
- 不建立 Project、Consumer、Payload、Result、Source Code 或 Evidence。
- 不修改 UI/Capture/Rendering Research Line。
- 不選擇 Clipboard Technology。
- 不建立 Clipboard ADR。
- 不開始 Clipboard 或截圖功能。
- 完成 `git diff --check` 與靜態 whitespace 檢查。
