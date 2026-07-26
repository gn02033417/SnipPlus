# Clipboard Integration Prerequisite Closure Plan

| Field | Value |
|---|---|
| Document ID | `RESEARCH-TECH-CLIPBOARD-004` |
| Title | Clipboard Integration Prerequisite Closure Plan |
| Status | Draft |
| Research Type | Prerequisite Closure Plan |
| Technology Decision | `TD-004 Clipboard Integration` |
| Parent Readiness Record | `RESEARCH-TECH-CLIPBOARD-003` |
| Parent Runtime Plan | `RESEARCH-TECH-CLIPBOARD-002` |
| Parent Feasibility | `RESEARCH-TECH-CLIPBOARD-001` |
| Closure Execution Status | Not started |
| Closure Execution Authorized | No |
| Build Verification | Not performed |
| Runtime Verification | Not performed |
| Clipboard Runtime Spike Authorized | No |
| Clipboard Read Authorized | No |
| Clipboard Write Authorized | No |
| Clipboard Clear Authorized | No |
| Evidence Write Authorized | No |
| UI Framework | Unresolved; `ADR-0002` remains Draft |
| Clipboard Decision | Not made |
| Capture Decision | Not made |
| Rendering Decision | Not made |
| Owner | TBD |
| Last Reviewed | Not reviewed |

## 1. Purpose

本文件只規劃如何以最小、可追溯、分階段且明確授權的方式，收斂 `RESEARCH-TECH-CLIPBOARD-003` 中的六個 Phase L1 blocking actions。它是 Closure Plan，不是 Closure Execution Record、Execution Enablement Specification、Authorization Request、Clipboard Runtime Spike、Technology Decision、ADR 或正式功能設計。

本文件的結果只能是後續 closure review 的輸入。任何 `Resolved`、`Approved`、`Authorized`、`Executed` 或 runtime 結果都必須由獨立且獲准的後續紀錄提供。

## 2. Scope

本計畫涵蓋：

- `CLIP-BA-001..006` 與一對一的 `CLIP-CLOSE-001..006`。
- `CLIP-PREQ-001..032`、`CLIP-BLOCK-001..013`、`CLIP-PAIR-001..010`、`CLIP-SPIKE-001..012`。
- Phase L1 所需的 Shared UI Host、Candidate、Project、Format、Consumer、Isolation、Synthetic Image、Threading、Evidence、Privacy、Cleanup 與授權條件。
- L2、L3 的延後依賴與重新啟動條件；延後不會自動解除 L1 blocker。
- Shared UI、Rendering、Capture、Clipboard、File Output 與 Architecture 的責任邊界。

## 3. Non-goals and Frozen Boundaries

本文件不會：

- 執行任何 closure action。
- 讀取、寫入、清除、備份或觀察目前使用者 Clipboard。
- 進行新的官方網路研究或本機環境盤點。
- 下載、安裝 SDK、runtime、package、tool 或 workload。
- 建立 Project、Solution、Prototype、Source Code、Payload、Result 或 Evidence。
- Restore、Build、Run、Publish、Test 或執行 Runtime Spike。
- 修改 Clipboard History、Cloud Clipboard、帳號、裝置或系統設定。
- 建立 Clipboard Authorization Request 或 Clipboard ADR。
- 修改 `RESEARCH-TECH-CLIPBOARD-001..003`、UI、Capture、Rendering Research Line 或 `ADR-0002`。
- 選擇 UI、Capture、Rendering 或 Clipboard Technology。
- 開始 Clipboard 或截圖功能。

## 4. Controlled Vocabulary

### 4.1 Closure Action Status

Closure Action 只能使用 `Planned`、`Blocked`、`Deferred`、`Not applicable`。本文件不得使用 `Completed`、`Resolved`、`Approved`、`Authorized` 或 `Executed` 表示 closure action 已完成。

### 4.2 Target Status Recommendation

每個 closure action 可以提出 `Resolved`、`Partially resolved`、`Blocked`、`Deferred` 或 `Not applicable` 的目標建議；這些只是建議，不會修改上游 readiness 狀態。

### 4.3 Dependency Ownership

依賴責任分類固定為：Shared UI research、Rendering research、Capture research、Clipboard-specific、Clipboard isolation、Synthetic image、Format/consumer、Threading/COM、Platform state、Evidence/privacy、Authorization。

### 4.4 Operation Classification

Operation Classification 只能從以下集合選擇：Official-source research、Local read-only inspection、Repository documentation mutation、Synthetic image specification、Experimental asset creation、Package acquisition、Development environment installation、Experimental project creation、Build execution、Clipboard read、Clipboard write、Clipboard clear、Runtime execution、Evidence capture/persistence、History/Cloud setting mutation。

本文件只記錄未來可能需要的分類，不執行分類所代表的操作。

## 5. Closure Action Binding

| Closure Action | Source Blocking Action | Binding rule |
|---|---|---|
| `CLIP-CLOSE-001` | `CLIP-BA-001` | 唯一收斂 Shared WPF/WinUI 3 Host dependency |
| `CLIP-CLOSE-002` | `CLIP-BA-002` | 唯一收斂 Candidate API/Interop identity |
| `CLIP-CLOSE-003` | `CLIP-BA-003` | 唯一收斂 Clipboard isolation 與 operation boundary |
| `CLIP-CLOSE-004` | `CLIP-BA-004` | 唯一收斂 Synthetic Image input contract |
| `CLIP-CLOSE-005` | `CLIP-BA-005` | 唯一收斂 Format/Consumer verification path |
| `CLIP-CLOSE-006` | `CLIP-BA-006` | 唯一收斂 Evidence/Cleanup/authority path |

不得 renumber、delete、merge 或 split `CLIP-BA`。不得新增第七個 L1 action。需要細分時只能在對應的 `CLIP-CLOSE` 下建立 sub-step。上游資料缺口使用 `CLIP-CLOSURE-GAP-xxx` 記錄，不直接修改上游文件。

## 6. Shared UI Dependency Reuse Matrix

`RESEARCH-TECH-UI-001` 是現有 UI research source。Repository 中未找到 `UI-AUTH-*` 文件，因此本文件不建立或假設該授權；Shared UI authority 必須由後續正式紀錄提供。

| Clipboard Closure Action | Shared capability | Existing UI source | Current status | Reusable scope | New authorization needed | Duplication prohibited |
|---|---|---|---|---|---|---|
| `CLIP-CLOSE-001` | Windows 11 x64 baseline | `docs/Research/Technology/01-ui-framework-feasibility.md` / `RESEARCH-TECH-UI-001` | Unresolved | Host identity and boundary only | Shared UI authority | Duplicate host approval |
| `CLIP-CLOSE-001` | WPF experimental build path | `RESEARCH-TECH-UI-001` | Candidate | Host path definition only | Project/Build authority later | Duplicate WPF host decision |
| `CLIP-CLOSE-001` | WinUI 3 experimental build path | `RESEARCH-TECH-UI-001` | Candidate | Host path definition only | Project/Build authority later | Duplicate WinUI 3 host decision |
| `CLIP-CLOSE-001` | .NET / Windows SDK | `RESEARCH-TECH-UI-001`; `RESEARCH-TECH-CAPTURE-001` | Not locally verified | Version identity to be recorded later | Local inspection and Build authority | Duplicate environment baseline |
| `CLIP-CLOSE-001` | Windows App SDK | `RESEARCH-TECH-UI-001` | Candidate | Package boundary only | Package/Project authority later | Duplicate package approval |
| `CLIP-CLOSE-001` | Experimental Project isolation | `Architecture/ARCH-0002-layer-model.md`; `ARCH-0004-component-boundaries.md` | Defined as boundary | Separate project identity | Project creation authority later | Clipboard project inside product source |
| `CLIP-CLOSE-001` | Package Restore | `AGENTS.md` execution boundary | Not granted | Authority separation only | Restore authority later | Implicit restore permission |
| `CLIP-CLOSE-001` | Build execution | `AGENTS.md`; `Architecture/ARCH-0001-architecture-principles.md` | Not granted | Build authority separation only | Build authority later | Duplicate Build authority |
| `CLIP-CLOSE-001` | Packaged/unpackaged mode | `ADR-0002-ui-framework-selection.md`; `RESEARCH-TECH-UI-001` | Not fixed | Comparison boundary only | Project/Build authority later | Treating mode as selected technology |
| `CLIP-CLOSE-001` | Evidence root policy | `ARCH-0005-component-interactions.md`; `RESEARCH-TECH-CAPTURE-001` | Not fixed | Root/privacy contract only | Evidence write authority later | Duplicate evidence root policy |
| `CLIP-CLOSE-001` | Privacy/cleanup | `ARCH-0005-component-interactions.md` | Boundary only | No-payload and cleanup rules | Evidence/Privacy authority later | Clipboard-specific duplicate policy |
| `CLIP-CLOSE-001` | Project/Restore/Build authority | `AGENTS.md` | Not granted | Reuse global prohibition | Explicit scoped authority later | Granting through this plan |
| `CLIP-CLOSE-001` | Runtime authority | `AGENTS.md`; `RESEARCH-TECH-UI-001` | Not granted | Runtime boundary only | Runtime authority later | Granting through this plan |

If shared authority remains unapproved, the dependent closure action remains `Blocked`. Clipboard-specific work can be planned but cannot be treated as approved Shared UI work.

## 7. Rendering and Capture Boundary

| Clipboard requirement | Rendering/Capture dependency | Source research | Required for L1 | Remaining boundary |
|---|---|---|---|---|
| Synthetic image source | Rendering defines an abstract deterministic image contract; Capture is not the producer | `RESEARCH-TECH-RENDER-001`; `RESEARCH-TECH-CAPTURE-001` | Yes | No formal Capture output may be substituted |
| Pixel format | Rendering records representation semantics; Clipboard records transport format | `RESEARCH-TECH-RENDER-001` | Yes | No renderer selection |
| Alpha mode | Rendering defines comparison terms; Consumer observes decoded result | `RESEARCH-TECH-RENDER-001` | Yes | No alpha fidelity claim |
| Row stride | Format contract and decoder observation | `RESEARCH-TECH-CLIPBOARD-002` | Yes | No runtime measurement |
| Color metadata | Rendering boundary supplies terms; Clipboard does not own color policy | `RESEARCH-TECH-RENDER-001` | Yes | SDR/wide-color substitute remains TBD |
| Final rendered image handoff | Capture output and Clipboard/File Output are parallel downstream consumers | `ARCH-0003-module-catalog.md`; `ARCH-0005-component-interactions.md` | Yes | No shared workflow state mutation |

Capture output is not runtime test data for this plan. A Clipboard failure must not trigger a Capture or Rendering rerun. Clipboard and File Output remain parallel downstream paths; this plan does not select either technology.

## 8. Clipboard-specific Dependency Matrix

| Candidate | Required API/Interop | COM/STA | Dispatcher | Packaging | Project | Build | Clipboard operation | Closure Action |
|---|---|---|---|---|---|---|---|---|
| `OPT-001` WPF Clipboard | WPF `Clipboard` / `IDataObject` | Host-specific evidence | WPF Dispatcher | Separate comparison | Isolated WPF project | Later authority | Read/Write/Clear separately | `CLIP-CLOSE-002` |
| `OPT-002` WinRT Clipboard | WinRT Clipboard / `DataPackage` | Apartment evidence | WinUI 3 Dispatcher | Package context recorded | Isolated WinUI 3 project | Later authority | Read/Write/Clear separately | `CLIP-CLOSE-002` |
| `OPT-003` OLE Clipboard | OLE / COM `IDataObject` | Explicit COM/STA contract | Host adapter if bound | Separate comparison | Native/host experiment | Later authority | Read/Write/Clear separately | `CLIP-CLOSE-002` |
| `OPT-004` Raw Win32 Clipboard | Raw Win32 Clipboard APIs | Native thread/COM evidence | Host adapter if bound | Separate comparison | Native/host experiment | Later authority | Read/Write/Clear separately | `CLIP-CLOSE-002` |
| `OPT-005` Host-neutral adapter | Adapter composition only | Delegates to candidate | Delegates to host | Host-specific | Isolated adapter project | Later authority | Never an authority itself | `CLIP-CLOSE-002` |

API identity is not local availability. All candidate identities remain experimental and unverified; Build verified = No and Runtime verified = No. This matrix does not rank or select a candidate.

## 9. Candidate–Host Pair Closure Matrix

| Pair | Current readiness | Blocking IDs | Required Closure Action | Required evidence | Target recommendation | Deferred phase |
|---|---|---|---|---|---|---|
| `CLIP-PAIR-001` WPF Clipboard/WPF | Conditionally eligible; blocked | `CLIP-BLOCK-001..013` | `CLIP-CLOSE-001..006` | Host, API, isolation, format, consumer, authority | Partially resolved | L2/L3 details deferred |
| `CLIP-PAIR-002` WinRT Clipboard/WinUI 3 | Conditionally eligible; blocked | `CLIP-BLOCK-001..013` | `CLIP-CLOSE-001..006` | Host, API, package, isolation, format, authority | Partially resolved | L2/L3 details deferred |
| `CLIP-PAIR-003` OLE/WPF | Conditionally eligible; blocked | `CLIP-BLOCK-001..013` | `CLIP-CLOSE-001..006` | COM/STA, ownership, format, consumer, cleanup | Partially resolved | L2/L3 details deferred |
| `CLIP-PAIR-004` Raw Win32/WPF | Conditionally eligible; blocked | `CLIP-BLOCK-001..013` | `CLIP-CLOSE-001..006` | Native API, format, handle, cleanup | Partially resolved | L2/L3 details deferred |
| `CLIP-PAIR-005` WPF Clipboard/WinUI 3 | Unknown | `CLIP-BLOCK-001,003,004,006..013` | `CLIP-CLOSE-001..006` | Adapter and host evidence | Deferred | L2 |
| `CLIP-PAIR-006` WinRT Clipboard/WPF | Unknown | `CLIP-BLOCK-001,003,004,006..013` | `CLIP-CLOSE-001..006` | Projection and host evidence | Deferred | L2 |
| `CLIP-PAIR-007` OLE/WinUI 3 | Conditionally eligible; blocked | `CLIP-BLOCK-001..013` | `CLIP-CLOSE-001..006` | COM/STA, adapter, package, cleanup | Partially resolved | L2/L3 details deferred |
| `CLIP-PAIR-008` Raw Win32/WinUI 3 | Conditionally eligible; blocked | `CLIP-BLOCK-001..013` | `CLIP-CLOSE-001..006` | Native adapter, package, thread, cleanup | Partially resolved | L2/L3 details deferred |
| `CLIP-PAIR-009` Host-neutral/WPF | Conditionally eligible; blocked | `CLIP-BLOCK-001..013` | `CLIP-CLOSE-001..006` | Adapter contract and host STA | Partially resolved | L2/L3 details deferred |
| `CLIP-PAIR-010` Host-neutral/WinUI 3 | Conditionally eligible; blocked | `CLIP-BLOCK-001..013` | `CLIP-CLOSE-001..006` | Adapter contract and host apartment | Partially resolved | L2/L3 details deferred |

`Unknown` is not `Excluded`. An excluded pair would require explicit evidence. Deferred L2/L3 details do not block the documentary Phase L1 plan unless they are required by a specific L1 gate.

## 10. Closure Action Register

### 10.1 `CLIP-CLOSE-001` — Shared UI Host dependency

- Closure Action ID: `CLIP-CLOSE-001`
- Source Blocking Action: `CLIP-BA-001`
- Blocking condition: WPF/WinUI 3 Host identity and authority remain unresolved.
- Related `CLIP-PREQ`: `CLIP-PREQ-001..003`
- Related `CLIP-BLOCK`: `CLIP-BLOCK-001..002`
- Related `CLIP-PAIR`: `CLIP-PAIR-001..010`
- Related `CLIP-SPIKE`: `CLIP-SPIKE-001..012`
- Related `CLIP-GATE`: `CLIP-GATE-001`, `CLIP-GATE-003`, `CLIP-GATE-008`
- Dependency ownership: Shared UI research; Authorization
- Shared UI source IDs: `RESEARCH-TECH-UI-001`; `UI-AUTH-*` not located; `TBD`
- Rendering source IDs: `RESEARCH-TECH-RENDER-001`
- Capture source IDs: `RESEARCH-TECH-CAPTURE-001`
- Existing evidence: UI feasibility and Architecture boundaries exist; host runtime/build evidence does not.
- Current limitation: `ADR-0002` is Draft and no UI Host authority record exists.
- Required final evidence: Approved host identity, target baseline, package context, and scoped authority record.
- Proposed closure operation: Documentary binding of Shared UI Host source and future authority boundary.
- Operation classification: Repository documentation mutation; Local read-only inspection later if separately authorized.
- Exact scope: WPF and WinUI 3 Host identity, target/version fields, launch context, and authority reuse.
- Explicit exclusions: No Project creation, package acquisition, Restore, Build, Runtime, Clipboard operation, or technology selection.
- Official-source research required: No for this plan; future evidence may be required.
- Local inspection required: No for this plan.
- Network access required: No.
- Package acquisition required: No.
- Installation required: No.
- Repository mutation required: Only this plan document.
- Synthetic image asset required: No.
- Experimental project required: No.
- Restore required: No.
- Build required: No.
- Clipboard read required: No.
- Clipboard write required: No.
- Clipboard clear required: No.
- Runtime execution required: No.
- Evidence write required: No; future evidence authority is separate.
- History/Cloud setting mutation required: No.
- Administrator privilege required: No.
- Human authorization required: Yes, in a later scoped record.
- Expected files/directories: No additional files or directories in this plan.
- Expected machine effect: None.
- Existing Clipboard impact: None.
- Privacy impact: None; no Clipboard data is accessed.
- Success condition: A later review can identify one permitted Host path and its authority without modifying `ADR-0002` here.
- Failure condition: Host identity, authority source, or target baseline remains ambiguous.
- Stop condition: Any request expands into installation, Project, Build, Runtime, or Clipboard operation.
- Rollback/cleanup: Revert only the future documentation change through normal review; no machine cleanup is needed.
- Resulting prerequisite recommendation: `CLIP-PREQ-001..003` Partially resolved.
- Resulting blocker recommendation: `CLIP-BLOCK-001..002` Partially resolved, not closed.
- Resulting pair recommendation: All pairs remain Blocked pending other actions.
- Phase L1 impact: Required; L1 Host boundary becomes reviewable only.
- Owner: UI owner TBD.
- Status: Planned.
- Open questions: Which Host, target framework, package mode, and authority record are permitted?

### 10.2 `CLIP-CLOSE-002` — Candidate API/Interop identity

- Closure Action ID: `CLIP-CLOSE-002`
- Source Blocking Action: `CLIP-BA-002`
- Blocking condition: Candidate API/Interop route is documented but not bound to an experimental identity.
- Related `CLIP-PREQ`: `CLIP-PREQ-004..008`
- Related `CLIP-BLOCK`: `CLIP-BLOCK-003`
- Related `CLIP-PAIR`: `CLIP-PAIR-001..010`
- Related `CLIP-SPIKE`: `CLIP-SPIKE-001..005`
- Related `CLIP-GATE`: `CLIP-GATE-002`, `CLIP-GATE-003`, `CLIP-GATE-006`
- Dependency ownership: Clipboard-specific; Threading/COM
- Shared UI source IDs: `RESEARCH-TECH-UI-001`; `UI-AUTH-*` not located; `TBD`
- Rendering source IDs: `RESEARCH-TECH-RENDER-001`
- Capture source IDs: `RESEARCH-TECH-CAPTURE-001`
- Existing evidence: Five candidate identities and ten Candidate–Host pairs are documented.
- Current limitation: Local package/project identity, COM boundary, and runtime route are unverified.
- Required final evidence: At least one scoped API/Interop identity with host, thread, project, and evidence boundary.
- Proposed closure operation: Documentary binding of candidate identity to a future isolated experiment.
- Operation classification: Repository documentation mutation; Experimental project creation later if authorized.
- Exact scope: `OPT-001..005`, API identity, adapter composition, host binding, and exclusions.
- Explicit exclusions: No API call, Clipboard access, candidate ranking, technology selection, or source code.
- Official-source research required: No for this plan; existing feasibility baseline is reused.
- Local inspection required: No for this plan.
- Network access required: No.
- Package acquisition required: No.
- Installation required: No.
- Repository mutation required: Only this plan document.
- Synthetic image asset required: No.
- Experimental project required: Future, not for this plan.
- Restore required: Future, not for this plan.
- Build required: Future, not for this plan.
- Clipboard read required: No.
- Clipboard write required: No.
- Clipboard clear required: No.
- Runtime execution required: No.
- Evidence write required: No.
- History/Cloud setting mutation required: No.
- Administrator privilege required: No.
- Human authorization required: Yes, separately for Project/Build/Runtime/Clipboard.
- Expected files/directories: No additional files or directories in this plan.
- Expected machine effect: None.
- Existing Clipboard impact: None.
- Privacy impact: None; no payload is created or accessed.
- Success condition: One or more candidate routes have a reviewable identity without claiming implementation support.
- Failure condition: Candidate identity remains only a generic API label or requires unapproved machine mutation.
- Stop condition: Any candidate access, package install, Project creation, or runtime action is requested under this plan.
- Rollback/cleanup: Documentation-only review rollback; no runtime cleanup.
- Resulting prerequisite recommendation: `CLIP-PREQ-004..008` Partially resolved.
- Resulting blocker recommendation: `CLIP-BLOCK-003` Partially resolved, not closed.
- Resulting pair recommendation: Candidate pairs remain Blocked or Unknown until host and operation gates are met.
- Phase L1 impact: Required; API/Interop identity becomes reviewable only.
- Owner: Clipboard owner TBD.
- Status: Planned.
- Open questions: Which candidate identity is permitted for the first isolated experiment?

### 10.3 `CLIP-CLOSE-003` — Clipboard isolation and operation boundary

- Closure Action ID: `CLIP-CLOSE-003`
- Source Blocking Action: `CLIP-BA-003`
- Blocking condition: Isolation policy and separate Read/Write/Clear permission are not authorized.
- Related `CLIP-PREQ`: `CLIP-PREQ-013..015`
- Related `CLIP-BLOCK`: `CLIP-BLOCK-006`
- Related `CLIP-PAIR`: `CLIP-PAIR-001..010`
- Related `CLIP-SPIKE`: `CLIP-SPIKE-001..012`
- Related `CLIP-GATE`: `CLIP-GATE-004`, `CLIP-GATE-008`, `CLIP-GATE-009`, `CLIP-GATE-010`
- Dependency ownership: Clipboard isolation; Authorization; Evidence/privacy
- Shared UI source IDs: `RESEARCH-TECH-UI-001`; `UI-AUTH-*` not located; `TBD`
- Rendering source IDs: `RESEARCH-TECH-RENDER-001`
- Capture source IDs: `RESEARCH-TECH-CAPTURE-001`
- Existing evidence: No approved isolated user/VM/session policy exists.
- Current limitation: Current Clipboard must not be read, backed up, overwritten, or cleared.
- Required final evidence: Written isolation policy, no-read/no-clear rule, overwrite consent, privacy stop, and cleanup authority.
- Proposed closure operation: Documentary definition of an isolated environment and independently scoped operation authorities.
- Operation classification: Repository documentation mutation; Clipboard read/write/clear only in a later authorized record.
- Exact scope: Dedicated account/VM/session, synthetic-only content, pre/post policy, and process cleanup.
- Explicit exclusions: No current Clipboard inspection, backup, clear, History/Cloud mutation, or environment creation.
- Official-source research required: No for this plan.
- Local inspection required: No for this plan.
- Network access required: No.
- Package acquisition required: No.
- Installation required: No.
- Repository mutation required: Only this plan document.
- Synthetic image asset required: No; specification is a separate action.
- Experimental project required: No.
- Restore required: No.
- Build required: No.
- Clipboard read required: No.
- Clipboard write required: No.
- Clipboard clear required: No.
- Runtime execution required: No.
- Evidence write required: No.
- History/Cloud setting mutation required: No.
- Administrator privilege required: No.
- Human authorization required: Yes, separately per operation.
- Expected files/directories: No additional files or directories in this plan.
- Expected machine effect: None.
- Existing Clipboard impact: None.
- Privacy impact: None; the plan explicitly forbids access to existing content.
- Success condition: A later authorization review can prove the experiment is isolated and operation permissions are independent.
- Failure condition: Isolation relies on the current interactive session or cannot guarantee no private data.
- Stop condition: Any attempt to inspect, write, clear, back up, sync, or change settings without a later explicit authority record.
- Rollback/cleanup: No machine state was changed; future runtime cleanup must be defined before authorization.
- Resulting prerequisite recommendation: `CLIP-PREQ-013..015` Partially resolved.
- Resulting blocker recommendation: `CLIP-BLOCK-006` Partially resolved, not closed.
- Resulting pair recommendation: All pairs remain Blocked.
- Phase L1 impact: Required; isolation and permission boundary become reviewable only.
- Owner: Privacy owner TBD.
- Status: Planned.
- Open questions: Which dedicated isolation mode is permitted and who grants each operation?

### 10.4 `CLIP-CLOSE-004` — Synthetic Image input contract

- Closure Action ID: `CLIP-CLOSE-004`
- Source Blocking Action: `CLIP-BA-004`
- Blocking condition: Synthetic Image contract is planned but no approved runtime input exists.
- Related `CLIP-PREQ`: `CLIP-PREQ-016`
- Related `CLIP-BLOCK`: `CLIP-BLOCK-007`
- Related `CLIP-PAIR`: `CLIP-PAIR-001..010`
- Related `CLIP-SPIKE`: `CLIP-SPIKE-001..005`
- Related `CLIP-GATE`: `CLIP-GATE-005`, `CLIP-GATE-006`, `CLIP-GATE-007`
- Dependency ownership: Synthetic image; Rendering research
- Shared UI source IDs: `RESEARCH-TECH-UI-001`; `UI-AUTH-*` not located; `TBD`
- Rendering source IDs: `RESEARCH-TECH-RENDER-001`
- Capture source IDs: `RESEARCH-TECH-CAPTURE-001`
- Existing evidence: Runtime plan describes synthetic requirements but no asset was created.
- Current limitation: No image bytes, payload, or runtime input may be produced by this plan.
- Required final evidence: Approved deterministic specification with dimensions, markers, alpha, color, language, and identity.
- Proposed closure operation: Documentary specification only; later asset creation requires separate authority.
- Operation classification: Synthetic image specification; Experimental asset creation later.
- Exact scope: Fixed dimensions, size classes, one-pixel border, markers, alpha reference, RGB/grayscale, mixed language, coordinates, and SDR/wide-color substitute.
- Explicit exclusions: No image asset, payload, rendering run, Clipboard write, consumer launch, or pixel result.
- Official-source research required: No for this plan.
- Local inspection required: No for this plan.
- Network access required: No.
- Package acquisition required: No.
- Installation required: No.
- Repository mutation required: Only this plan document.
- Synthetic image asset required: Future; not for this plan.
- Experimental project required: No.
- Restore required: No.
- Build required: No.
- Clipboard read required: No.
- Clipboard write required: No.
- Clipboard clear required: No.
- Runtime execution required: No.
- Evidence write required: No.
- History/Cloud setting mutation required: No.
- Administrator privilege required: No.
- Human authorization required: Yes, for asset creation and later execution.
- Expected files/directories: No asset or result directory in this plan.
- Expected machine effect: None.
- Existing Clipboard impact: None.
- Privacy impact: None; only future synthetic content is in scope.
- Success condition: A later asset request can be deterministic without choosing a rendering or Clipboard technology.
- Failure condition: Specification depends on unrecorded pixels, product screenshots, or user Clipboard content.
- Stop condition: Any request creates image bytes or executes a rendering/Clipboard path.
- Rollback/cleanup: Documentation-only review rollback; no asset cleanup.
- Resulting prerequisite recommendation: `CLIP-PREQ-016` Partially resolved.
- Resulting blocker recommendation: `CLIP-BLOCK-007` Partially resolved, not closed.
- Resulting pair recommendation: All pairs remain Blocked.
- Phase L1 impact: Required; input contract becomes reviewable only.
- Owner: Evidence owner TBD.
- Status: Planned.
- Open questions: Which size classes and color substitute are approved without product thresholds?

### 10.5 `CLIP-CLOSE-005` — Format and Consumer verification path

- Closure Action ID: `CLIP-CLOSE-005`
- Source Blocking Action: `CLIP-BA-005`
- Blocking condition: Format, multi-format, Alpha/pixel/color, and Consumer evidence paths are incomplete.
- Related `CLIP-PREQ`: `CLIP-PREQ-017..025`
- Related `CLIP-BLOCK`: `CLIP-BLOCK-008..009`
- Related `CLIP-PAIR`: `CLIP-PAIR-001..010`
- Related `CLIP-SPIKE`: `CLIP-SPIKE-001..005`, `CLIP-SPIKE-009`
- Related `CLIP-GATE`: `CLIP-GATE-006`, `CLIP-GATE-007`, `CLIP-GATE-010`
- Dependency ownership: Format/consumer; Rendering research; Clipboard-specific
- Shared UI source IDs: `RESEARCH-TECH-UI-001`; `UI-AUTH-*` not located; `TBD`
- Rendering source IDs: `RESEARCH-TECH-RENDER-001`
- Capture source IDs: `RESEARCH-TECH-CAPTURE-001`
- Existing evidence: Candidate format list and consumer categories exist in the runtime plan.
- Current limitation: No runtime format enumeration, consumer observation, or fidelity result exists.
- Required final evidence: Scoped format publication, consumer identity, Alpha/pixel/color method, and no-private-data boundary.
- Proposed closure operation: Documentary format/consumer contract and measurement-method specification.
- Operation classification: Repository documentation mutation; Evidence capture/persistence later.
- Exact scope: Framework Bitmap, CF_BITMAP, CF_DIB, CF_DIBV5, registered PNG, OLE `IDataObject`, WinRT `DataPackage`, multi-format, and isolated consumers.
- Explicit exclusions: No consumer launch, payload creation, Clipboard write, runtime pixel comparison, third-party application use, or technology selection.
- Official-source research required: No for this plan.
- Local inspection required: No for this plan.
- Network access required: No.
- Package acquisition required: No.
- Installation required: No.
- Repository mutation required: Only this plan document.
- Synthetic image asset required: Future, through `CLIP-CLOSE-004` authority.
- Experimental project required: Future, not for this plan.
- Restore required: No.
- Build required: No.
- Clipboard read required: No.
- Clipboard write required: No.
- Clipboard clear required: No.
- Runtime execution required: No.
- Evidence write required: No.
- History/Cloud setting mutation required: No.
- Administrator privilege required: No.
- Human authorization required: Yes, for consumer/runtime/evidence operations.
- Expected files/directories: No consumer project, payload, or result directory in this plan.
- Expected machine effect: None.
- Existing Clipboard impact: None.
- Privacy impact: None; real user applications and content are excluded.
- Success condition: A later Spike can identify producer representation, consumer boundary, and observation method independently.
- Failure condition: Format claims rely on API names without representation or consumer evidence.
- Stop condition: Any consumer launch, Clipboard access, runtime conversion, or persistence is requested under this plan.
- Rollback/cleanup: Documentation-only review rollback; no consumer or process cleanup.
- Resulting prerequisite recommendation: `CLIP-PREQ-017..025` Partially resolved.
- Resulting blocker recommendation: `CLIP-BLOCK-008..009` Partially resolved, not closed.
- Resulting pair recommendation: All pairs remain Blocked.
- Phase L1 impact: Required; format and consumer path becomes reviewable only.
- Owner: Clipboard owner TBD; Rendering owner TBD.
- Status: Planned.
- Open questions: Which isolated consumer set and fidelity observation method are approved?

### 10.6 `CLIP-CLOSE-006` — Evidence, privacy, cleanup, and authority path

- Closure Action ID: `CLIP-CLOSE-006`
- Source Blocking Action: `CLIP-BA-006`
- Blocking condition: Evidence persistence, privacy review, cleanup confirmation, and execution authority are ungranted.
- Related `CLIP-PREQ`: `CLIP-PREQ-031..032`
- Related `CLIP-BLOCK`: `CLIP-BLOCK-012..013`
- Related `CLIP-PAIR`: `CLIP-PAIR-001..010`
- Related `CLIP-SPIKE`: `CLIP-SPIKE-001..012`
- Related `CLIP-GATE`: `CLIP-GATE-008`, `CLIP-GATE-009`, `CLIP-GATE-010`, `CLIP-GATE-011`
- Dependency ownership: Evidence/privacy; Authorization
- Shared UI source IDs: `RESEARCH-TECH-UI-001`; `UI-AUTH-*` not located; `TBD`
- Rendering source IDs: `RESEARCH-TECH-RENDER-001`
- Capture source IDs: `RESEARCH-TECH-CAPTURE-001`
- Existing evidence: `AGENTS.md` prohibits unrequested Build/Run/Test and the upstream readiness record marks all execution authority as ungranted.
- Current limitation: No artifact schema, evidence root, cleanup record, Project/Restore/Build authority, or Clipboard operation authority exists.
- Required final evidence: Separate authority record, privacy-safe artifact schema, result root, cleanup confirmation, and independent failure evidence.
- Proposed closure operation: Documentary authority matrix and evidence/cleanup contract; future authorization review is separate.
- Operation classification: Repository documentation mutation; Evidence capture/persistence later.
- Exact scope: Authority separation for Project, Restore, Build, Clipboard Read/Write/Clear, Runtime, Evidence Write, History/Cloud, privacy, and cleanup.
- Explicit exclusions: No evidence artifact, result directory, payload, source code, runtime, Clipboard operation, setting mutation, or authority grant.
- Official-source research required: No for this plan.
- Local inspection required: No for this plan.
- Network access required: No.
- Package acquisition required: No.
- Installation required: No.
- Repository mutation required: Only this plan document.
- Synthetic image asset required: No.
- Experimental project required: No.
- Restore required: No.
- Build required: No.
- Clipboard read required: No.
- Clipboard write required: No.
- Clipboard clear required: No.
- Runtime execution required: No.
- Evidence write required: No.
- History/Cloud setting mutation required: No.
- Administrator privilege required: No.
- Human authorization required: Yes; every authority must be independently scoped.
- Expected files/directories: No result or evidence directory in this plan.
- Expected machine effect: None.
- Existing Clipboard impact: None.
- Privacy impact: None; no payload is persisted or observed.
- Success condition: A later authorization review can distinguish planning, execution, and evidence persistence.
- Failure condition: One broad approval implicitly grants multiple operations or permits persistent private data.
- Stop condition: Any evidence write, result directory creation, runtime, build, or Clipboard access is attempted.
- Rollback/cleanup: No machine state changed; future cleanup must be part of the authorized execution record.
- Resulting prerequisite recommendation: `CLIP-PREQ-031..032` Partially resolved.
- Resulting blocker recommendation: `CLIP-BLOCK-012..013` Partially resolved, not closed.
- Resulting pair recommendation: All pairs remain Blocked.
- Phase L1 impact: Required; authority and evidence boundary becomes reviewable only.
- Owner: Evidence owner TBD; Product owner TBD.
- Status: Planned.
- Open questions: Who may grant each operation and what is the minimum persistent evidence set?

## 11. Phase L1 Minimum Closure Gates

Gate status is limited to `Specified`, `Partially specified`, `Blocked`, or `Deferred`; this plan never uses `Satisfied`, `Passed`, or `Resolved`.

| Gate | Minimum condition | Covered by | Gate Plan Status | Later evidence required |
|---|---|---|---|---|
| `CLIP-CGATE-001` | Shared WPF/WinUI 3 Host build dependency has a clear reference and authorization path | `CLIP-CLOSE-001` | Partially specified | Host and authority record |
| `CLIP-CGATE-002` | At least one Clipboard Candidate exact API/Interop identity is fixed | `CLIP-CLOSE-002` | Partially specified | Candidate identity record |
| `CLIP-CGATE-003` | Candidate–Host Project, COM, and Dispatcher boundary is specified | `CLIP-CLOSE-001..002` | Partially specified | Project/thread boundary |
| `CLIP-CGATE-004` | Clipboard isolation and existing-content protection policy is specified | `CLIP-CLOSE-003` | Blocked | Isolation approval and cleanup |
| `CLIP-CGATE-005` | Basic Synthetic Image is fully specified | `CLIP-CLOSE-004` | Partially specified | Approved deterministic specification |
| `CLIP-CGATE-006` | Bitmap, DIB/DIBV5, PNG, and multi-format methods are specified | `CLIP-CLOSE-005` | Partially specified | Format contract |
| `CLIP-CGATE-007` | Consumer interoperability and Alpha/pixel evidence method is specified | `CLIP-CLOSE-005` | Blocked | Consumer and fidelity method |
| `CLIP-CGATE-008` | Read/Write/Clear/Runtime/Evidence authority is separated | `CLIP-CLOSE-003`, `006` | Blocked | Separate authorization records |
| `CLIP-CGATE-009` | Result storage, privacy, and cleanup boundary is specified | `CLIP-CLOSE-003`, `006` | Blocked | Evidence schema and cleanup |
| `CLIP-CGATE-010` | Clipboard/File Output failure is independent | `CLIP-CLOSE-005`, `006` | Partially specified | Parallel failure evidence |
| `CLIP-CGATE-011` | Runtime execution remains a later, separate authorization | `CLIP-CLOSE-006` | Specified | Future execution authorization |

## 12. Clipboard Isolation Closure Plan

| Isolation concern | Existing definition | Remaining gap | Future policy/evidence | Runtime required | Evidence write required | Closure Action |
|---|---|---|---|---|---|---|
| Dedicated test account/VM/session | Planned in upstream runtime plan | Mode and owner unknown | Explicit isolated environment record | Yes | Yes | `CLIP-CLOSE-003` |
| Existing Clipboard data | No-read/no-backup boundary | Precondition proof method unknown | Synthetic-only precondition | Yes | Yes | `CLIP-CLOSE-003` |
| Read prohibition | Not granted | Enforcement record absent | Read authority = No by default | No | No | `CLIP-CLOSE-003` |
| Clear prohibition | Not granted | Enforcement record absent | Clear authority = No by default | No | No | `CLIP-CLOSE-003` |
| Overwrite consent | Not granted | Human consent path unknown | Separate Write authority | Yes | Yes | `CLIP-CLOSE-003` |
| History disabled/enabled | Deferred branch | Must not change settings | Observe only in isolated later phase | Yes | Yes | `CLIP-CLOSE-003` |
| Cloud disabled/enabled | Deferred branch | Account/device boundary unknown | Separate platform-state authority | Yes | Yes | `CLIP-CLOSE-003` |
| Consumer boundary | Isolated consumer required | Consumer identity not fixed | Approved synthetic-only consumer | Yes | Yes | `CLIP-CLOSE-005` |
| Residual payload cleanup | Cleanup required | Confirmation method absent | Cleanup record without payload logging | Yes | Yes | `CLIP-CLOSE-006` |
| Process termination cleanup | Planned | Normal/abnormal method deferred | Later lifecycle evidence | Yes | Yes | `CLIP-CLOSE-006` |
| Failure stop | Stop rules planned | Authority and evidence not granted | Stop record with no retry thresholds here | Yes | Yes | `CLIP-CLOSE-006` |
| Private-data detection | Prohibited user-content access | Synthetic-only guarantee not established | Privacy review before runtime | Yes | Yes | `CLIP-CLOSE-003`, `006` |

This plan does not create, start, or inspect an isolation environment.

## 13. Synthetic Image Closure Plan

| Synthetic requirement | Existing definition | Remaining gap | Future asset required | Runtime required | Evidence write required | Closure Action |
|---|---|---|---|---|---|---|
| Fixed dimensions | Planned | Exact values not approved | Yes | Yes | Yes | `CLIP-CLOSE-004` |
| Size classes | Planned | Classes and limits TBD | Yes | Yes | Yes | `CLIP-CLOSE-004` |
| Alpha reference | Planned | Straight/premultiplied contract TBD | Yes | Yes | Yes | `CLIP-CLOSE-004` |
| One-pixel border | Planned | Marker identity TBD | Yes | Yes | Yes | `CLIP-CLOSE-004` |
| Known markers | Planned | Coordinates and values TBD | Yes | Yes | Yes | `CLIP-CLOSE-004` |
| RGB/grayscale | Planned | Exact pair TBD | Yes | Yes | Yes | `CLIP-CLOSE-004` |
| Mixed language | Planned | Stable Unicode sample TBD | Yes | Yes | Yes | `CLIP-CLOSE-004` |
| Known coordinates | Planned | Coordinate list TBD | Yes | Yes | Yes | `CLIP-CLOSE-004` |
| SDR/wide-color substitute | Planned | Substitute and metadata TBD | Yes | Yes | Yes | `CLIP-CLOSE-004` |

No image bytes or payload are created by this plan.

## 14. Format Closure Plan

| Format | Existing definition | Remaining gap | Future asset/consumer required | Runtime required | Evidence write required | Closure Action |
|---|---|---|---|---|---|---|
| Framework Bitmap | Candidate representation | Producer/consumer conversion | Yes | Yes | Yes | `CLIP-CLOSE-005` |
| `CF_BITMAP` | Candidate native representation | Alpha and handle semantics | Yes | Yes | Yes | `CLIP-CLOSE-005` |
| `CF_DIB` | Candidate native representation | Header/stride/mask semantics | Yes | Yes | Yes | `CLIP-CLOSE-005` |
| `CF_DIBV5` | Candidate native representation | Color metadata and masks | Yes | Yes | Yes | `CLIP-CLOSE-005` |
| Registered PNG | Candidate stream representation | Registration/decoder identity | Yes | Yes | Yes | `CLIP-CLOSE-005` |
| OLE `IDataObject` | Candidate publication boundary | Enumeration and lifetime | Yes | Yes | Yes | `CLIP-CLOSE-005` |
| WinRT `DataPackage` | Candidate publication boundary | Projection and package context | Yes | Yes | Yes | `CLIP-CLOSE-005` |
| Multi-format | Candidate fallback boundary | Atomicity and selection | Yes | Yes | Yes | `CLIP-CLOSE-005` |

## 15. Consumer Closure Plan

| Consumer class | Existing definition | Remaining gap | Future asset/consumer required | Runtime required | Evidence write required | Closure Action |
|---|---|---|---|---|---|---|
| WPF test consumer | Candidate | Exact isolated consumer TBD | Yes | Yes | Yes | `CLIP-CLOSE-005` |
| WinUI 3 test consumer | Candidate | Exact isolated consumer TBD | Yes | Yes | Yes | `CLIP-CLOSE-005` |
| Win32/OLE consumer | Candidate | Process and ownership boundary TBD | Yes | Yes | Yes | `CLIP-CLOSE-005` |
| Basic editor | Candidate | No real application selected | Yes | Yes | Yes | `CLIP-CLOSE-005` |
| Office-style consumer | Deferred | Privacy and installation boundary | Yes | Yes | Yes | `CLIP-CLOSE-005` |
| Browser class | Deferred | Browser identity and session boundary | Yes | Yes | Yes | `CLIP-CLOSE-005` |
| Clipboard History | Deferred | Platform state and account boundary | Yes | Yes | Yes | `CLIP-CLOSE-003`, `006` |
| Cloud Clipboard | Deferred | Sync/account/device boundary | Yes | Yes | Yes | `CLIP-CLOSE-003`, `006` |

No consumer Project or application is created or launched here.

## 16. Threading, COM, Evidence, and Cleanup Closure Plan

| Capability | Existing definition | Remaining gap | Required method/tool | Runtime required | Evidence write required | Closure Action |
|---|---|---|---|---|---|---|
| WPF UI STA | Required | Observation route absent | Later isolated host observation | Yes | Yes | `CLIP-CLOSE-002`, `006` |
| WPF background STA | Required | Boundary and marshal behavior TBD | Later controlled thread observation | Yes | Yes | `CLIP-CLOSE-002`, `006` |
| WPF background MTA | Required | Boundary and failure behavior TBD | Later controlled thread observation | Yes | Yes | `CLIP-CLOSE-002`, `006` |
| WinUI 3 UI thread | Required | Dispatcher evidence absent | Later isolated host observation | Yes | Yes | `CLIP-CLOSE-001`, `006` |
| WinUI 3 background thread | Required | Projection boundary TBD | Later controlled thread observation | Yes | Yes | `CLIP-CLOSE-002`, `006` |
| COM initialization | Required | Owner and HRESULT/exception record TBD | Later explicit COM observation | Yes | Yes | `CLIP-CLOSE-002`, `006` |
| Dispatcher shutdown | Required | Shutdown ordering TBD | Later lifecycle observation | Yes | Yes | `CLIP-CLOSE-006` |
| Cancellation during retry | Required | Retry policy not approved | Later authorized cancellation observation | Yes | Yes | `CLIP-CLOSE-006` |
| Clipboard contention | Deferred | Safe synthetic source absent | Later isolated contention source | Yes | Yes | `CLIP-CLOSE-003`, `006` |
| Retry observation | Deferred | Count/interval/timeout TBD | Later authorized observation | Yes | Yes | `CLIP-CLOSE-006` |
| Ownership/lifetime | Deferred | Immediate/delayed ownership TBD | Later process/consumer observation | Yes | Yes | `CLIP-CLOSE-006` |
| Normal/abnormal process termination | Deferred | Cleanup proof absent | Later lifecycle observation | Yes | Yes | `CLIP-CLOSE-006` |
| Pixel comparison | Planned | Method and input contract incomplete | Later synthetic comparison | Yes | Yes | `CLIP-CLOSE-004`, `005` |
| Alpha comparison | Planned | Mode and marker incomplete | Later synthetic comparison | Yes | Yes | `CLIP-CLOSE-004`, `005` |
| Memory observation | Deferred | Size classes and threshold TBD | Later isolated resource observation | Yes | Yes | `CLIP-CLOSE-006` |
| Privacy review | Required | Artifact policy absent | Later review record | No | Yes | `CLIP-CLOSE-003`, `006` |
| Cleanup confirmation | Required | Confirmation schema absent | Later cleanup record | Yes | Yes | `CLIP-CLOSE-006` |
| Parallel File Output result | Required | Independent failure evidence absent | Later parallel observer | Yes | Yes | `CLIP-CLOSE-006` |

本計畫不自行制定 Retry 次數、間隔、Timeout、記憶體或 Pixel 差異門檻。

## 17. Deferred Scope Register

| Deferred target | Target phase | Deferred reason | Reactivation condition | Affected candidates | Affected pairs | Affected spikes | Blocks Phase L1 |
|---|---|---|---|---|---|---|---|
| Full STA/MTA matrix | L2 | L1 only needs boundary definition | Host and thread authority exists | `OPT-001..005` | `PAIR-001..010` | `SPIKE-006..012` | No |
| Clipboard contention | L2 | Safe synthetic owner not defined | Isolation and Write authority exist | `OPT-001..005` | `PAIR-001..010` | `SPIKE-007`, `012` | No |
| Retry timing | L2 | Thresholds are not product decisions | Explicit retry study authorized | `OPT-001..005` | `PAIR-001..010` | `SPIKE-007`, `012` | No |
| Ownership/process lifetime | L2 | Lifecycle observer not authorized | Process authority and cleanup method exist | `OPT-001..005` | `PAIR-001..010` | `SPIKE-008` | No |
| Large-image memory | L2 | Size and memory thresholds TBD | Synthetic asset and resource authority exist | `OPT-001..005` | `PAIR-001..010` | `SPIKE-009` | No |
| History enabled | L3 | Platform state and privacy risk | Isolated account/device approval | `OPT-001..005` | `PAIR-001..010` | `SPIKE-010` | No |
| Cloud Clipboard | L3 | Account/device sync boundary unknown | Isolated sync authority exists | `OPT-001..005` | `PAIR-001..010` | `SPIKE-010` | No |
| Packaged/unpackaged full comparison | L2 | Project/build authority absent | Host and project baseline fixed | `OPT-001..005` | `PAIR-001..010` | `SPIKE-011` | No |
| Third-party Consumer full matrix | L3 | Privacy and installation boundary | Approved isolated consumers exist | `OPT-001..005` | `PAIR-001..010` | `SPIKE-005`, `010` | No |
| Abnormal termination | L2 | Process lifecycle authority absent | Cleanup policy and runtime authority exist | `OPT-001..005` | `PAIR-001..010` | `SPIKE-008` | No |
| Phase L2 | L2 | L1 closure is not complete | L1 gates reviewed and authority separated | `OPT-001..005` | `PAIR-001..010` | `SPIKE-006..009`, `011`, `012` | No |
| Phase L3 | L3 | History/Cloud and full ecosystem are deferred | L2 evidence and platform isolation exist | `OPT-001..005` | `PAIR-001..010` | `SPIKE-010` | No |

Deferred does not permanently remove any item from Clipboard Research.

## 18. Full Impact Matrix

下表將上游 32 prerequisites、13 blockers、10 pairs、12 spikes、6 blocking actions 與 6 closure actions 映射到本計畫。`Current status` 只引用上游 readiness；`Target recommendation` 是本文件建議，不修改上游狀態。

| Source item | Phase | Closure Action | Required evidence | Current status | Target recommendation |
|---|---|---|---|---|---|
| `CLIP-PREQ-001..003` | L1 | `CLIP-CLOSE-001` | Host identity, target baseline, authority | Blocked | Partially resolved |
| `CLIP-PREQ-004..008` | L1 | `CLIP-CLOSE-002` | Candidate API/Interop and adapter identity | Partially resolved | Partially resolved |
| `CLIP-PREQ-009..011` | L1/L2 | `CLIP-CLOSE-002`, `006` | STA/MTA, COM, Dispatcher | Partially resolved | Deferred details |
| `CLIP-PREQ-012` | L1/L2 | `CLIP-CLOSE-001` | Package mode and comparison scope | Partially resolved | Partially resolved |
| `CLIP-PREQ-013..015` | L1 | `CLIP-CLOSE-003` | Isolation and separate operation authority | Blocked | Partially resolved |
| `CLIP-PREQ-016` | L1 | `CLIP-CLOSE-004` | Deterministic Synthetic Image specification | Blocked | Partially resolved |
| `CLIP-PREQ-017..022` | L1 | `CLIP-CLOSE-005` | Format and multi-format contract | Partially resolved | Partially resolved |
| `CLIP-PREQ-023..025` | L1 | `CLIP-CLOSE-005` | Alpha, pixel, color, Consumer method | Blocked | Partially resolved |
| `CLIP-PREQ-026..030` | L2/L3 | `CLIP-CLOSE-003`, `006` | Contention, retry, lifetime, memory, History/Cloud | Blocked | Deferred |
| `CLIP-PREQ-031..032` | L1 | `CLIP-CLOSE-006` | Evidence, cleanup, authority | Blocked | Partially resolved |
| `CLIP-BLOCK-001..002` | L1 | `CLIP-CLOSE-001` | Shared Host dependency | Open | Partially resolved |
| `CLIP-BLOCK-003` | L1 | `CLIP-CLOSE-002` | Candidate route identity | Open | Partially resolved |
| `CLIP-BLOCK-004..005` | L1/L2 | `CLIP-CLOSE-001`, `002` | Thread, COM, package boundary | Open | Deferred details |
| `CLIP-BLOCK-006` | L1 | `CLIP-CLOSE-003` | Isolation and permission policy | Open | Partially resolved |
| `CLIP-BLOCK-007` | L1 | `CLIP-CLOSE-004` | Synthetic Image contract | Open | Partially resolved |
| `CLIP-BLOCK-008..009` | L1 | `CLIP-CLOSE-005` | Format, Consumer, fidelity method | Open | Partially resolved |
| `CLIP-BLOCK-010..011` | L2/L3 | `CLIP-CLOSE-003`, `006` | Contention, retry, memory, History/Cloud | Open | Deferred |
| `CLIP-BLOCK-012..013` | L1 | `CLIP-CLOSE-006` | Evidence and authority records | Open | Partially resolved |
| `CLIP-PAIR-001..004` | L1 | `CLIP-CLOSE-001..006` | Host-specific candidate evidence | Blocked | Partially resolved |
| `CLIP-PAIR-005..006` | L2 | `CLIP-CLOSE-001..006` | Cross-host adapter evidence | Not evaluated | Deferred |
| `CLIP-PAIR-007..010` | L1/L2 | `CLIP-CLOSE-001..006` | Native/adapter host evidence | Blocked | Partially resolved |
| `CLIP-SPIKE-001..005` | L1 | `CLIP-CLOSE-001..006` | Host, format, consumer, authority | Blocked | Partially resolved |
| `CLIP-SPIKE-006..009` | L2 | `CLIP-CLOSE-003`, `006` | Thread, retry, lifetime, memory | Blocked | Deferred |
| `CLIP-SPIKE-010` | L3 | `CLIP-CLOSE-003`, `006` | History/Cloud isolation | Blocked | Deferred |
| `CLIP-SPIKE-011..012` | L2 | `CLIP-CLOSE-001`, `006` | Package modes and parallel File Output | Blocked | Deferred |
| `CLIP-BA-001` | L1 | `CLIP-CLOSE-001` | Shared UI Host closure evidence | Open | Partially resolved |
| `CLIP-BA-002` | L1 | `CLIP-CLOSE-002` | Candidate identity evidence | Open | Partially resolved |
| `CLIP-BA-003` | L1 | `CLIP-CLOSE-003` | Isolation and operation boundary | Open | Partially resolved |
| `CLIP-BA-004` | L1 | `CLIP-CLOSE-004` | Synthetic Image contract | Open | Partially resolved |
| `CLIP-BA-005` | L1 | `CLIP-CLOSE-005` | Format/Consumer method | Open | Partially resolved |
| `CLIP-BA-006` | L1 | `CLIP-CLOSE-006` | Evidence/cleanup/authority | Open | Partially resolved |
| `CLIP-CLOSE-001..006` | L1 | One-to-one source action binding | Future closure review | Planned | Not yet resolved |

### 18.1 Exact ID Coverage Ledger

The following ledger keeps every upstream identifier explicit so that the closure plan can be audited without interpreting shorthand ranges:

| Identifier family | Explicit identifiers |
|---|---|
| Prerequisites | `CLIP-PREQ-001`, `CLIP-PREQ-002`, `CLIP-PREQ-003`, `CLIP-PREQ-004`, `CLIP-PREQ-005`, `CLIP-PREQ-006`, `CLIP-PREQ-007`, `CLIP-PREQ-008`, `CLIP-PREQ-009`, `CLIP-PREQ-010`, `CLIP-PREQ-011`, `CLIP-PREQ-012`, `CLIP-PREQ-013`, `CLIP-PREQ-014`, `CLIP-PREQ-015`, `CLIP-PREQ-016`, `CLIP-PREQ-017`, `CLIP-PREQ-018`, `CLIP-PREQ-019`, `CLIP-PREQ-020`, `CLIP-PREQ-021`, `CLIP-PREQ-022`, `CLIP-PREQ-023`, `CLIP-PREQ-024`, `CLIP-PREQ-025`, `CLIP-PREQ-026`, `CLIP-PREQ-027`, `CLIP-PREQ-028`, `CLIP-PREQ-029`, `CLIP-PREQ-030`, `CLIP-PREQ-031`, `CLIP-PREQ-032` |
| Blockers | `CLIP-BLOCK-001`, `CLIP-BLOCK-002`, `CLIP-BLOCK-003`, `CLIP-BLOCK-004`, `CLIP-BLOCK-005`, `CLIP-BLOCK-006`, `CLIP-BLOCK-007`, `CLIP-BLOCK-008`, `CLIP-BLOCK-009`, `CLIP-BLOCK-010`, `CLIP-BLOCK-011`, `CLIP-BLOCK-012`, `CLIP-BLOCK-013` |
| Candidate–Host pairs | `CLIP-PAIR-001`, `CLIP-PAIR-002`, `CLIP-PAIR-003`, `CLIP-PAIR-004`, `CLIP-PAIR-005`, `CLIP-PAIR-006`, `CLIP-PAIR-007`, `CLIP-PAIR-008`, `CLIP-PAIR-009`, `CLIP-PAIR-010` |
| Spikes | `CLIP-SPIKE-001`, `CLIP-SPIKE-002`, `CLIP-SPIKE-003`, `CLIP-SPIKE-004`, `CLIP-SPIKE-005`, `CLIP-SPIKE-006`, `CLIP-SPIKE-007`, `CLIP-SPIKE-008`, `CLIP-SPIKE-009`, `CLIP-SPIKE-010`, `CLIP-SPIKE-011`, `CLIP-SPIKE-012` |
| Blocking actions | `CLIP-BA-001`, `CLIP-BA-002`, `CLIP-BA-003`, `CLIP-BA-004`, `CLIP-BA-005`, `CLIP-BA-006` |
| Closure actions | `CLIP-CLOSE-001`, `CLIP-CLOSE-002`, `CLIP-CLOSE-003`, `CLIP-CLOSE-004`, `CLIP-CLOSE-005`, `CLIP-CLOSE-006` |

## 19. Recommended Closure Order

固定順序如下；這只是規劃，不是執行授權：

1. Confirm Shared UI Host evidence and authority reuse boundary.
2. Fix Phase L1 Candidate API/Interop identity.
3. Fix Candidate–Host Project, Package/Restore, and Build scope.
4. Fix Clipboard Isolation policy.
5. Fix Basic Synthetic Image, Format, and Consumer contract.
6. Fix Threading, Alpha/pixel, Privacy, and Cleanup evidence method.
7. Separate Clipboard Read/Write/Clear, Runtime, and Evidence authority.
8. Submit a future Clipboard prerequisite closure execution authorization review.
9. Execute a future approved Closure Action.
10. Re-evaluate Clipboard Runtime Spike readiness in a future record.

## 20. Authorization Boundary

| Operation | Current authorization | Execution permitted |
|---|---|---|
| Official-source research | Not granted | No |
| Local read-only inspection | Not granted | No |
| Package acquisition | Not granted | No |
| SDK/Tool installation | Not granted | No |
| Synthetic image creation | Not granted | No |
| Consumer asset creation | Not granted | No |
| Experimental Project creation | Not granted | No |
| Restore | Not granted | No |
| Build | Not granted | No |
| Clipboard read | Not granted | No |
| Clipboard write | Not granted | No |
| Clipboard clear | Not granted | No |
| Runtime execution | Not granted | No |
| Evidence write | Not granted | No |
| Result directory creation | Not granted | No |
| History/Cloud setting mutation | Not granted | No |

所有 `Execution permitted` 固定為 `No`。本文件不會把 Closure Plan 當成任何操作授權。

## 21. Closure Plan Status

Closure plan status 只能使用 `Closure plan complete`、`Partially complete` 或 `Incomplete`。另行判定可使用 `Ready to request clipboard prerequisite closure execution authorization`、`Conditionally ready to request clipboard prerequisite closure execution authorization` 或 `Not ready`。

本文件固定狀態如下：

- Closure plan status: `Partially complete`
- Authorization readiness: `Not ready`
- Closure Execution Authorized: No
- Build Verification: Not performed
- Runtime Verification: Not performed
- Clipboard Runtime Spike Authorized: No
- Clipboard Read Authorized: No
- Clipboard Write Authorized: No
- Clipboard Clear Authorized: No
- Evidence Write Authorized: No
- Clipboard Decision: Not made
- Capture Decision: Not made
- Rendering Decision: Not made

這不是 Runtime Spike readiness，也不是執行授權。六個 action 只有規劃狀態，沒有任何 closure result。

## 22. Traceability

```text
CLIP-PREQ / CLIP-BLOCK
  -> CLIP-BA
  -> CLIP-CLOSE
  -> Required authority
  -> Future closure evidence
  -> Candidate-Host readiness
  -> CLIP Spike readiness reassessment
  -> Future Clipboard decision
```

Normative and boundary references：

- `RESEARCH-TECH-CLIPBOARD-001` — `docs/Research/Technology/29-clipboard-integration-feasibility.md`
- `RESEARCH-TECH-CLIPBOARD-002` — `docs/Research/Technology/30-clipboard-integration-runtime-spike-plan.md`
- `RESEARCH-TECH-CLIPBOARD-003` — `docs/Research/Technology/31-clipboard-integration-runtime-spike-execution-readiness.md`
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

實際文件名稱與 Document ID 以 Repository 現況為準；本文件不建立 `UI-AUTH-*` 假想來源。

## 23. Completion Conditions

- 只建立 `docs/Research/Technology/32-clipboard-integration-prerequisite-closure-plan.md`。
- Document ID 固定為 `RESEARCH-TECH-CLIPBOARD-004`。
- 不修改任何其他文件。
- 建立正好六個 `CLIP-CLOSE-001..006`。
- 保持六組 `CLIP-BA -> CLIP-CLOSE` 一對一。
- 不新增、刪除、合併或拆分 `CLIP-BA`。
- 完整覆蓋 32 個 prerequisite、13 個 blocker、10 個 pair 與 12 個 spike。
- 建立 11 個 `CLIP-CGATE-001..011`。
- 重用 Shared UI authority，不建立重複授權。
- 建立 Rendering/Capture boundary，不選擇其 Technology。
- 所有 `Current authorization = Not granted`。
- 所有 `Execution permitted = No`。
- 不讀取、寫入、清除或備份 Clipboard。
- 不執行官方研究、本機盤點、下載、安裝、Restore、Build、Run、Test 或 Runtime Spike。
- 不建立 Project、Prototype、Payload、Result、Source Code 或 Evidence。
- 不建立 Clipboard ADR。
- 不修改 UI、Capture、Rendering Research Line 或 `ADR-0002`。
- 不開始 Clipboard 或截圖功能。
- 完成靜態 whitespace 檢查與 `git diff --check`。
