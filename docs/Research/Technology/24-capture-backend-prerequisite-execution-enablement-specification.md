# Capture Backend Prerequisite Execution Enablement Specification

| Field | Value |
|---|---|
| Document ID | RESEARCH-TECH-CAPTURE-005 |
| Title | Capture Backend Prerequisite Execution Enablement Specification |
| Status | Draft |
| Research Type | Execution Enablement Specification |
| Parent Closure Plan | RESEARCH-TECH-CAPTURE-004 |
| Parent Readiness Record | RESEARCH-TECH-CAPTURE-003 |
| Parent Runtime Plan | RESEARCH-TECH-CAPTURE-002 |
| Parent Feasibility | RESEARCH-TECH-CAPTURE-001 |
| Closure Execution Status | Not started |
| Closure Execution Authorized | No |
| Build Verification | Not performed |
| Runtime Verification | Not performed |
| Capture Runtime Spike Authorized | No |
| Evidence Write Authorized | No |
| UI Framework Decision | Unresolved — ADR-0002 remains Draft |
| Rendering Decision | Not made |
| Capture Decision | Not made |
| Owner | TBD |
| Last reviewed | Not reviewed |

## 1. Purpose

本文件只回答：

> CAP-CLOSE-001 至 CAP-CLOSE-007 在提交 Capture prerequisite closure execution authorization review 前，必須具備哪些精確操作範圍、Candidate／Host identity、Project／Restore／Build 邊界、Synthetic Scene、Coordinate／Evidence 方法、安全條件與 rollback 義務？

這是 Execution Enablement Specification，不是 Closure Execution Record、Authorization Request、Capture Runtime Spike、Capture Backend Decision 或 Capture ADR。

## 2. Scope

本文件只規格化：

- CAP-CLOSE-001..007
- CAP-BA-001..007
- CAP-PREQ-001..030
- CAP-BLOCK-001..012
- CAP-PAIR-001..010
- CAP-SPIKE-001..012
- CAP-CGATE-001..010
- Phase C1 最小 Host、Candidate、Project、Build、Scene、Coordinate、Evidence enablement
- Shared UI authority inheritance
- Capture-specific authorization delta 的規格邊界；不建立授權

## 3. Non-goals

本文件不得：

- 執行任何 Enablement Item。
- 執行官方網路研究或本機盤點。
- 查詢 Package Cache。
- 下載或安裝 SDK、Runtime、Package、Tool 或 workload。
- 建立 Project、Solution、Prototype、Source Code 或 Result directory。
- 執行 Restore、Build、Run、Publish 或 Capture API。
- 擷取桌面、視窗、螢幕、Frame 或 Recording。
- 建立 Screenshot、PNG、Reference Frame、Pixel Difference、Log 或 Measurement Artifact。
- 建立 Authorization Request。
- 修改 RESEARCH-TECH-CAPTURE-001..004。
- 修改 UI／Rendering Research Line。
- 修改 ADR-0002 或建立 Capture ADR。
- 選擇 Capture Backend。
- 開始正式截圖功能。
- 建立 CAP-AUTH 或任何等效的 capture-specific 授權記錄。

## 4. Controlled Vocabulary

### 4.1 Enablement Item Status

只能使用：

- Specified
- Partially specified
- Blocked
- Deferred
- Not applicable

不得把 Completed、Resolved、Approved、Authorized 或 Executed 當作 Enablement Item status。

### 4.2 Specification Evidence Status

只能使用：

- Accepted from parent evidence
- Confirmed by official source
- Partially specified
- Unknown
- Conflicting
- Not applicable

本文件未執行官方來源查詢或本機盤點，因此沒有項目可宣稱 Confirmed by official source。

### 4.3 Execution Permission

本文件所有執行權限均為 No。Current authorization 固定為 Not granted。Specification complete 只代表未來可以進入獨立 authorization review，不代表已授權或已執行。

## 5. Enablement Binding

建立正好七組一對一 binding：

- CAP-ENABLE-001 → CAP-CLOSE-001 → CAP-BA-001
- CAP-ENABLE-002 → CAP-CLOSE-002 → CAP-BA-002
- CAP-ENABLE-003 → CAP-CLOSE-003 → CAP-BA-003
- CAP-ENABLE-004 → CAP-CLOSE-004 → CAP-BA-004
- CAP-ENABLE-005 → CAP-CLOSE-005 → CAP-BA-005
- CAP-ENABLE-006 → CAP-CLOSE-006 → CAP-BA-006
- CAP-ENABLE-007 → CAP-CLOSE-007 → CAP-BA-007

規則：

- 不重新編號、不合併、不拆分。
- 不增加第八個 Phase C1 Blocking Action。
- 不足之處建立 CAP-ENABLE-GAP-xxx，而不是改寫上游 ID。
- 不直接修改上游文件。
- Shared UI authority 不得重新包裝為 capture-specific authority。

## 6. 每個 Enablement Item 固定欄位

每個 CAP-ENABLE 必須包含同一組欄位：

- Enablement Item ID
- Source Closure Action
- Source Blocking Action
- Related prerequisites
- Related blockers
- Related pairs
- Related spikes
- Related upstream gates
- Related closure gates
- Dependency ownership
- Shared UI source IDs
- Rendering source IDs
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
- Experimental project required
- Restore required
- Build required
- Capture API invocation required
- Runtime execution required
- Evidence persistence required
- Display／system mutation required
- Administrator privilege required
- Human authorization required
- Expected files／directories
- Expected Package／Cache effects
- Expected machine effects
- Privacy impact
- Risk classification
- Failure impact
- Stop conditions
- Rollback／cleanup requirement
- Success condition
- Result artifact obligation
- Resulting prerequisite recommendation
- Resulting blocker recommendation
- Resulting pair recommendation
- Phase C1 effect
- Owner
- Status
- Open questions

欄位語意：

- Related、Source 與 Traceability 欄位只引用既有 ID。
- Required final evidence 描述未來證據義務，不代表本輪建立 evidence。
- Operation required 與 Execution permission 分開。
- Build、Runtime、Capture API、Evidence persistence、Display／system mutation 必須分開。
- Status 只使用本文件的 controlled vocabulary。
- Human authorization required 不是 authorization granted。

## 7. Operation Classification

| Classification | Risk | 本文件處理方式 |
| --- | --- | --- |
| Official-source research | R0 | 只列為 future lookup；本輪未執行 |
| Local read-only inspection | R0 | 只列為 future inspection；本輪未執行 |
| Repository documentation mutation | R1 | 本輪只建立本文件 |
| Synthetic asset specification | R1 | 只描述 future fixture；不建立 asset |
| Experimental asset creation | R1 | 未授權；不建立 |
| Experimental project creation | R1 | 未授權；不建立 |
| Package acquisition | R2 | 未授權；不查詢、下載或變更 cache |
| Package Restore | R2 | 未授權；不執行 |
| Build execution | R2 | 未授權；不執行 |
| Development environment installation | R3 | 未授權；不執行 |
| Capture API invocation | R4 | 未授權；不執行 |
| Runtime execution | R4 | 未授權；不執行 |
| Evidence persistence | R4 | 未授權；不執行 |
| Display／system mutation | R4 | 未授權；不執行 |


規則：

- 本文件只能規格化 R0 至 R3 的未來操作內容。
- R4 只能列為後續獨立授權邊界。
- 一項操作涉及多層級時採最高風險。
- Build 不得被解讀成 Run。
- Runtime 不得被解讀成 Evidence write。
- Evidence write 不得被解讀成 Display mutation。

## 8. Shared UI Authority Inheritance Matrix

Shared UI authority 只被繼承，不在本文件重新申請：

| Capture Enablement | Shared capability | UI source IDs | Current UI decision | Reusable scope | Capture-specific extension | Duplicate request prohibited |
| --- | --- | --- | --- | --- | --- | --- |
| CAP-ENABLE-001 | Windows 11 x64 baseline | RESEARCH-TECH-UI-007, RESEARCH-TECH-UI-008 | Blocked | Host baseline fields and provenance | Capture candidate identity only | 第二份 OS baseline |
| CAP-ENABLE-001 | WinUI 3 experimental build path | RESEARCH-TECH-UI-007, UI-AUTH-001 | Blocked | Host path and package boundary | Capture interop details | 重複 WinUI host authorization |
| CAP-ENABLE-001 | WPF experimental build path | RESEARCH-TECH-UI-008, UI-AUTH-002 | Blocked | Host path and runtime boundary | Capture interop details | 重複 WPF host authorization |
| CAP-ENABLE-001 | .NET／Windows SDK | RESEARCH-TECH-UI-007, UI-AUTH-003, UI-AUTH-004 | Blocked | Identity and evidence fields | Candidate API mapping | Capture-specific SDK authority |
| CAP-ENABLE-001 | Windows App SDK | RESEARCH-TECH-UI-007, UI-AUTH-001 | Blocked | Package/runtime identity boundary | Candidate package relation | 另一組 package permission |
| CAP-ENABLE-003 | Experimental Project isolation | RESEARCH-TECH-UI-008, UI-AUTH-005 | Blocked | Isolated project rule | Capture pair path only | 在 Capture 文件假設 project exists |
| CAP-ENABLE-003 | Package Restore | RESEARCH-TECH-UI-009, UI-AUTH-006 | Blocked | Restore scope only | Candidate package list | 把 Restore 當成 Runtime permission |
| CAP-ENABLE-003 | Build execution | RESEARCH-TECH-UI-009, UI-AUTH-007 | Blocked | Explicit build scope | Candidate build identity | 把 Build authority 當成 Runtime authority |
| CAP-ENABLE-005 | Display topology | RESEARCH-TECH-UI-009, UI-AUTH-003 | Blocked | Topology and evidence fields | Capture-source mapping | 建立第二份 display authorization |
| CAP-ENABLE-005 | Per-monitor DPI | RESEARCH-TECH-UI-009, UI-AUTH-004 | Blocked | DPI contract only | Capture crop mapping | 自行決定 rounding policy |
| CAP-ENABLE-006 | Evidence storage policy | RESEARCH-TECH-UI-009, UI-AUTH-006 | Blocked | Governance and retention boundary | Capture evidence schema | 建立第二個 evidence root |
| CAP-ENABLE-006 | Safety／cleanup | RESEARCH-TECH-UI-009, UI-AUTH-007 | Blocked | Cleanup principles and stop rules | Capture-specific manifest | 以文件聲明取代 acceptance |
| CAP-ENABLE-007 | Runtime execution authority | RESEARCH-TECH-UI-009, UI-AUTH-008 | Blocked | Authority separation only | Independent capture review | 把 UI authority 延伸成 Capture authority |


Shared authority 尚未批准時，依賴項保持 Blocked 或 Partially specified。Capture-specific API、Interop 或 Scene scope 可以被規格化，但本文件不得申請授權。

## 9. Rendering Dependency Enablement Boundary

Rendering source 只作為 future synthetic scene 的輸入邊界：

| Capture capability | Rendering dependency | Rendering source | Minimal Phase C1 method | Product rendering dependency | Remaining gap |
| --- | --- | --- | --- | --- | --- |
| Fixed synthetic surface | Host-native synthetic surface | RESEARCH-TECH-RENDER-003 | Specification only | Not applicable to product renderer | Future fixture contract |
| Color blocks | Host-native color blocks | RESEARCH-TECH-RENDER-003 | Specification only | Not applicable to product renderer | Color manifest not frozen |
| One-pixel border | Pixel-precise fixture contract | RESEARCH-TECH-RENDER-003 | Specification only | Not applicable to product renderer | Rounding remains TBD |
| Coordinate grid | Simple host drawing | RESEARCH-TECH-RENDER-003 | Specification only | Not applicable to product renderer | Grid does not choose rendering technology |
| Corner／center markers | Synthetic marker geometry | RESEARCH-TECH-RENDER-003 | Specification only | Not applicable to product renderer | Geometry evidence later |
| Mixed-language text | Host text fixture | RESEARCH-TECH-RENDER-003 | Specification only | Not applicable to product renderer | Font environment later |
| Alpha gradient | Synthetic fixture | RESEARCH-TECH-RENDER-003 | Deferred | Not applicable to product renderer | C2/C3 fidelity scope |
| Overlay-like synthetic window | Separate synthetic host surface | RESEARCH-TECH-RENDER-003 | Deferred | Not SnipPlus Overlay | No product overlay mutation |
| Wide-color substitute | Synthetic color reference | RESEARCH-TECH-RENDER-003 | Deferred | Not an HDR decision | C2/C3 color scope |
| Result inspection surface | Future host inspection only | RESEARCH-TECH-RENDER-003 | Deferred | Not a product renderer | No result directory |


不得選擇 Rendering Candidate。不得把 temporary synthetic surface 變成產品 Architecture。沒有 Rendering Technology 依賴的項目標示 Not applicable。

## 10. Candidate Experimental Identity Baseline

| Candidate | Host | Exact API／SDK identity | Experimental identity／version | Graphics device | Interop | Packaging | Local availability | Build verified | Runtime verified |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Windows Graphics Capture | WinUI 3 | TBD | TBD | D3D11 path TBD | Host interop TBD | Packaged／unpackaged TBD | Unknown | No | No |
| Windows Graphics Capture | WPF | TBD | TBD | D3D11 path TBD | Host interop TBD | Packaged／unpackaged TBD | Unknown | No | No |
| DXGI Desktop Duplication | WinUI 3 | DXGI／D3D11 identity TBD | TBD | Adapter/output TBD | Native interop TBD | Host-dependent TBD | Unknown | No | No |
| DXGI Desktop Duplication | WPF | DXGI／D3D11 identity TBD | TBD | Adapter/output TBD | Native interop TBD | Host-dependent TBD | Unknown | No | No |
| GDI | WinUI 3 | GDI／bitmap identity TBD | TBD | CPU／bitmap path | Native interop TBD | Host-dependent TBD | Unknown | No | No |
| GDI | WPF | GDI／bitmap identity TBD | TBD | CPU／bitmap path | Native interop TBD | Host-dependent TBD | Unknown | No | No |
| Window-oriented mechanisms | WinUI 3 | Exact API identity TBD | TBD | Host/window compositor TBD | Window interop TBD | Host-dependent TBD | Unknown | No | No |
| Window-oriented mechanisms | WPF | Exact API identity TBD | TBD | Host/window compositor TBD | Window interop TBD | Host-dependent TBD | Unknown | No | No |
| Hybrid strategy | WinUI 3 | Constituent WGC／DXGI／GDI／window APIs TBD | TBD | Each constituent path TBD | Explicit hybrid ownership TBD | Candidate-dependent TBD | Unknown | No | No |
| Hybrid strategy | WPF | Constituent WGC／DXGI／GDI／window APIs TBD | TBD | Each constituent path TBD | Explicit hybrid ownership TBD | Candidate-dependent TBD | Unknown | No | No |


官方 identity 與本機 availability 分開。無證據時使用 TBD 或 Unknown。所有 Build verified 與 Runtime verified 固定為 No。Hybrid 必須列出構成 API。Experimental identity 不代表產品版本或 Candidate 選擇。

## 11. Candidate–Host Enablement Matrix

| Pair | Candidate | Host | Current readiness | Shared UI dependency | Capture dependency | Required operation | Enablement Item | Target recommendation |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| CAP-PAIR-001 | Windows Graphics Capture | WinUI 3 | Blocked | Inherited; not duplicated | Exact API／SDK／Interop TBD | Future isolated review | CAP-ENABLE-001 | Keep in scope; no ranking |
| CAP-PAIR-002 | Windows Graphics Capture | WPF | Blocked | Inherited; not duplicated | Exact API／SDK／Interop TBD | Future isolated review | CAP-ENABLE-001 | Keep in scope; no ranking |
| CAP-PAIR-003 | DXGI Desktop Duplication | WinUI 3 | Blocked | Inherited; not duplicated | Exact API／SDK／Interop TBD | Future isolated review | CAP-ENABLE-002 | Keep in scope; no ranking |
| CAP-PAIR-004 | DXGI Desktop Duplication | WPF | Blocked | Inherited; not duplicated | Exact API／SDK／Interop TBD | Future isolated review | CAP-ENABLE-002 | Keep in scope; no ranking |
| CAP-PAIR-005 | GDI | WinUI 3 | Blocked | Inherited; not duplicated | Exact API／SDK／Interop TBD | Future isolated review | CAP-ENABLE-003 | Keep in scope; no ranking |
| CAP-PAIR-006 | GDI | WPF | Blocked | Inherited; not duplicated | Exact API／SDK／Interop TBD | Future isolated review | CAP-ENABLE-003 | Keep in scope; no ranking |
| CAP-PAIR-007 | Window-oriented mechanisms | WinUI 3 | Blocked | Inherited; not duplicated | Exact API／SDK／Interop TBD | Future isolated review | CAP-ENABLE-004 | Keep in scope; no ranking |
| CAP-PAIR-008 | Window-oriented mechanisms | WPF | Blocked | Inherited; not duplicated | Exact API／SDK／Interop TBD | Future isolated review | CAP-ENABLE-004 | Keep in scope; no ranking |
| CAP-PAIR-009 | Hybrid strategy | WinUI 3 | Blocked | Inherited; not duplicated | Exact API／SDK／Interop TBD | Future isolated review | CAP-ENABLE-005 | Keep in scope; no ranking |
| CAP-PAIR-010 | Hybrid strategy | WPF | Blocked | Inherited; not duplicated | Exact API／SDK／Interop TBD | Future isolated review | CAP-ENABLE-005 | Keep in scope; no ranking |


十個 Pair 均有一列。Unknown 不得轉為 Excluded with evidence。Deferred Pair 必須保留 C2／C3 reactivation condition。不依 Package availability 形成 Candidate ranking。所有 Pair execution authority 仍為 No。

## 12. Repository Isolation Boundary

只規劃，不建立：

- experiments/capture/<host>/<candidate>/
- docs/Research/Technology/results/capture/

規定：

- Experimental Project 不得位於產品 Source tree。
- 不得被產品 Project reference。
- 不得建立正式 Architecture component。
- 每個 Host／Candidate 使用隔離路徑。
- Synthetic asset、temporary output、Build output 與 Result 必須可識別。
- 所有未來新增項目必須有 cleanup manifest。
- 本文件不得建立上述目錄。

## 13. Project／Package／Restore／Build 分離規格

### 13.1 Experimental Project Creation

未來 enablement record 必須記錄：

- Host
- Candidate
- Target framework
- Architecture
- Packaged／unpackaged mode
- Repository path
- Minimal contents
- Native interop boundary
- 明確禁止的產品內容

本文件只規格化欄位，不建立 project。

### 13.2 Package Acquisition／Restore

未來 enablement record 必須記錄：

- Package／SDK identity
- Experimental version
- Package source
- Expected dependencies
- Native asset implications
- Cache effects
- Offline limitation
- Rollback limitation

本文件不查詢 Package Cache、不下載、不安裝、不 Restore。

### 13.3 Build Verification

未來 build record 必須記錄：

- Build tool
- Configuration
- Architecture
- Packaging mode
- Expected outputs
- Required logs
- Exit-code handling
- Cleanup requirements

固定：

- Capture API invocation permitted: No
- Run permitted: No
- Runtime execution permitted: No
- Evidence persistence permitted: No
- Display／system mutation permitted: No

## 14. Synthetic Scene Enablement

| Capability | Existing specification | Future asset | Creation required | Runtime required | Enablement Item | Remaining gap |
| --- | --- | --- | --- | --- | --- | --- |
| Fixed physical canvas | CAP-PREQ-013 listed | Exact dimensions and pixel manifest | Yes, future fixture | Yes, future authorization | CAP-ENABLE-004 | Dimensions TBD |
| Fixed logical canvas | CAP-PREQ-013 listed | DIP-to-pixel relation | Yes, future fixture | Yes, future authorization | CAP-ENABLE-004 | DIP relation TBD |
| High-contrast color blocks | CAP-PREQ-013 listed | Color manifest | Yes, future fixture | Yes, future authorization | CAP-ENABLE-004 | Color values TBD |
| One-pixel border | CAP-PREQ-014 listed | Edge observation contract | Yes, future fixture | Yes, future authorization | CAP-ENABLE-004 | Edge result TBD |
| Coordinate grid | CAP-PREQ-014 listed | Origin and labels | Yes, future fixture | Yes, future authorization | CAP-ENABLE-004 | Label layout TBD |
| Corner markers | CAP-PREQ-013 listed | Marker geometry | Yes, future fixture | Yes, future authorization | CAP-ENABLE-004 | Geometry TBD |
| Center marker | CAP-PREQ-013 listed | Marker geometry | Yes, future fixture | Yes, future authorization | CAP-ENABLE-004 | Geometry TBD |
| Mixed-language text | CAP-PREQ-013 listed | Font/environment record | Yes, future fixture | Yes, future authorization | CAP-ENABLE-004 | Font set TBD |
| Alpha gradient | CAP-PREQ-013 listed | Format and profile | Yes, future fixture | C2 authorization | CAP-ENABLE-004 | Deferred fidelity scope |
| SDR block | CAP-PREQ-013 listed | Baseline color metadata | Yes, future fixture | C1/C2 authorization | CAP-ENABLE-004 | Metadata TBD |
| Wide-color substitute | CAP-PREQ-013 listed | Lawful conversion path | Yes, future fixture | C2 authorization | CAP-ENABLE-004 | Deferred color scope |
| Cursor target | CAP-PREQ-017 listed | Inclusion/exclusion contract | Yes, future fixture | C2 authorization | CAP-ENABLE-004 | Cursor policy TBD |
| Overlay-like window | CAP-PREQ-018 listed | Synthetic ownership boundary | Yes, future fixture | C2 authorization | CAP-ENABLE-004 | No product overlay |
| Occluded-window scenario | CAP-PREQ-019 listed | Expected semantics | Yes, future fixture | C2 authorization | CAP-ENABLE-004 | Semantics TBD |
| Minimized-window scenario | CAP-PREQ-019 listed | Expected semantics | Yes, future fixture | C2 authorization | CAP-ENABLE-004 | Semantics TBD |
| Negative-coordinate placement | CAP-PREQ-015 listed | Signed coordinate contract | Yes, future fixture | C1 authorization | CAP-ENABLE-004 | Signed origin TBD |
| Same-DPI multi-monitor | CAP-PREQ-015 listed | Topology manifest | Yes, future fixture | C1 authorization | CAP-ENABLE-004 | Topology TBD |
| Mixed-DPI multi-monitor | CAP-PREQ-016 listed | Per-monitor mapping | Yes, future fixture | C2 authorization | CAP-ENABLE-004 | Mapping TBD |
| Protected-content substitute | CAP-PREQ-020 listed | Lawful substitute behavior | Yes, future fixture | C2 authorization | CAP-ENABLE-004 | No real protected content |
| Display-change／device-loss trigger | CAP-PREQ-021 listed | Trigger and cleanup contract | No static asset alone | C3 authorization | CAP-ENABLE-004 | Runtime trigger deferred |


規則：

- 本文件只規格化 future asset。
- 不建立 Window、Scene、Image 或 Source Code。
- Mixed-DPI、HDR、Device-loss 等後期 Scene 可 Deferred，但須保留 reactivation condition。
- Protected-content substitute 不得使用真正受保護內容。

## 15. Coordinate／Crop Enablement

| Capability | Existing definition | Required static specification | Runtime verification required | Enablement Item | Remaining gap |
| --- | --- | --- | --- | --- | --- |
| Virtual-screen origin | CAP-PREQ-014, CAP-PREQ-015 | Signed origin evidence | Yes, future authorization | CAP-ENABLE-005 | Origin TBD |
| Monitor physical bounds | CAP-PREQ-014, CAP-PREQ-015 | Exact bounds record | Yes, future authorization | CAP-ENABLE-005 | Bounds TBD |
| DIP bounds | CAP-PREQ-016 | Per-monitor conversion | Yes, future authorization | CAP-ENABLE-005 | Conversion TBD |
| Selection intent | CAP-PREQ-014 | Owner and timestamp | Yes, future authorization | CAP-ENABLE-005 | Manifest TBD |
| Capture-source bounds | CAP-PREQ-013, CAP-PREQ-022 | Candidate-specific source identity | Yes, future authorization | CAP-ENABLE-005 | Source identity TBD |
| Frame-local bounds | CAP-PREQ-022 | Origin and dimensions | Yes, future authorization | CAP-ENABLE-005 | Frame schema TBD |
| Crop conversion | CAP-PREQ-023 | Source-to-crop mapping | Yes, future authorization | CAP-ENABLE-005 | Mapping TBD |
| Negative coordinates | CAP-PREQ-015 | Signed mapping observation | Yes, future authorization | CAP-ENABLE-005 | Observation TBD |
| Inclusive／exclusive edges | CAP-PREQ-023 | Contract not frozen | Yes, future authorization | CAP-ENABLE-005 | Semantics TBD |
| Rounding policy | CAP-PREQ-016, CAP-PREQ-023 | Decision marker | Yes, future authorization | CAP-ENABLE-005 | TBD; no product decision |
| Timestamp correlation | CAP-PREQ-022, CAP-PREQ-024 | Selection/frame relation | Yes, future authorization | CAP-ENABLE-005 | Correlation TBD |
| Off-by-one detection | CAP-PREQ-023 | Expected/observed comparison | Yes, future authorization | CAP-ENABLE-005 | Method TBD |
| Pixel-difference method | CAP-PREQ-023 | Analysis procedure without threshold | Yes, future authorization | CAP-ENABLE-005 | Threshold not decided |
| Frame metadata | CAP-PREQ-022, CAP-PREQ-024 | Candidate schema | Yes, future authorization | CAP-ENABLE-005, CAP-ENABLE-006 | Schema TBD |
| Privacy review | CAP-PREQ-025, CAP-PREQ-026 | Reviewer and retention boundary | Yes, future authorization | CAP-ENABLE-006 | Reviewer TBD |
| Cleanup confirmation | CAP-PREQ-026, CAP-PREQ-030 | Stop/rollback evidence | Yes, future authorization | CAP-ENABLE-006, CAP-ENABLE-007 | Manifest TBD |


Rounding policy 未決時維持 TBD，不形成產品級座標決策。Static specification 不得被視為 Crop fidelity 已通過。

## 16. Evidence Method Enablement

| Evidence capability | Planned method | Required tool／library | Runtime required | Persistence required | Authorization class | Enablement Item |
| --- | --- | --- | --- | --- | --- | --- |
| Environment record | Read-only environment manifest | TBD | No in this document | No in this document | Future R0 inspection | CAP-ENABLE-001 |
| Frame metadata | Candidate-specific metadata schema | TBD | Future authorization | Future evidence authorization | R4 boundary | CAP-ENABLE-002, CAP-ENABLE-005 |
| Coordinate mapping | Deterministic source-to-frame map | TBD | Future authorization | Future evidence authorization | R4 boundary | CAP-ENABLE-005 |
| Synthetic source reference | Fixture manifest reference | TBD | Future authorization | Future evidence authorization | R4 boundary | CAP-ENABLE-004, CAP-ENABLE-005 |
| Captured frame | Future lawful frame artifact | TBD | Future authorization | Future evidence authorization | R4 boundary | CAP-ENABLE-007 |
| Crop output | Future crop artifact with source relation | TBD | Future authorization | Future evidence authorization | R4 boundary | CAP-ENABLE-005, CAP-ENABLE-007 |
| Pixel difference | Future comparison procedure | TBD | Future authorization | Future evidence authorization | R4 boundary | CAP-ENABLE-005 |
| Pixel format／color metadata | Future format record | TBD | Future authorization | Future evidence authorization | R4 boundary | CAP-ENABLE-004, CAP-ENABLE-005 |
| Timing | Future selection/frame timestamps | TBD | Future authorization | Future evidence authorization | R4 boundary | CAP-ENABLE-005 |
| CPU／GPU／memory observation | Future session observation only | TBD | Future authorization | Future evidence authorization | R4 boundary | CAP-ENABLE-002, CAP-ENABLE-007 |
| Failure reproduction | Future isolated reproduction log | TBD | Future authorization | Future evidence authorization | R4 boundary | CAP-ENABLE-003, CAP-ENABLE-007 |
| Recovery observation | Future stop/recovery record | TBD | Future authorization | Future evidence authorization | R4 boundary | CAP-ENABLE-006, CAP-ENABLE-007 |
| Diagnostic log | Future scoped diagnostic record | TBD | Future authorization | Future evidence authorization | R4 boundary | CAP-ENABLE-006, CAP-ENABLE-007 |
| Privacy review | Future reviewer and retention record | TBD | Future authorization | Future evidence authorization | R4 boundary | CAP-ENABLE-006 |
| Cleanup confirmation | Future cleanup manifest and confirmation | TBD | Future authorization | Future evidence authorization | R4 boundary | CAP-ENABLE-006, CAP-ENABLE-007 |


規則：

- Runtime 與持久化 Evidence 均在本輪授權範圍外。
- 不建立實際 Evidence。
- 不自行設定像素差、時間、記憶體或色差門檻。
- Session observation 不等於持久 Evidence。

## 17. Phase C1 Enablement Gates

| Closure Gate | Required specification | Related Enablement Items | Current specification status | Remaining gap |
| --- | --- | --- | --- | --- |
| CAP-CGATE-001 | Shared WinUI 3／WPF Host build dependencies 有明確引用或授權路徑 | CAP-ENABLE-001, CAP-ENABLE-003 | Blocked | Shared UI authority and exact host identity |
| CAP-CGATE-002 | 至少一個 one-shot Candidate 的精確 API／SDK identity 固定 | CAP-ENABLE-002 | Blocked | Official identity and local availability |
| CAP-CGATE-003 | Candidate–Host Project／Interop boundary 規格化 | CAP-ENABLE-002, CAP-ENABLE-003 | Blocked | Pair-specific boundary |
| CAP-CGATE-004 | Basic synthetic scene 完整規格化 | CAP-ENABLE-004 | Partially specified | Fixture dimensions, font and color manifest |
| CAP-CGATE-005 | Virtual desktop、monitor、negative-coordinate model 規格化 | CAP-ENABLE-005 | Blocked | Topology and signed mapping |
| CAP-CGATE-006 | Region crop 與 off-by-one method 規格化 | CAP-ENABLE-005 | Blocked | Edge semantics and rounding TBD |
| CAP-CGATE-007 | Frame、metadata、coordinate、privacy evidence obligation 規格化 | CAP-ENABLE-005, CAP-ENABLE-006 | Partially specified | Reviewer, retention and evidence schema |
| CAP-CGATE-008 | Project、Package／Restore、Build、Runtime、Evidence write authority 分離 | CAP-ENABLE-003, CAP-ENABLE-006, CAP-ENABLE-007 | Blocked | Independent authorities and scope records |
| CAP-CGATE-009 | Result storage 與 cleanup boundary 規格化 | CAP-ENABLE-006 | Partially specified | Root, manifest and rollback owner |
| CAP-CGATE-010 | Runtime execution 保留為後續獨立授權 | CAP-ENABLE-007 | Specified | Independent review record not created |


Status 只能使用 Specified、Partially specified、Blocked、Deferred、Not applicable；不得使用 Satisfied、Passed 或 Resolved。

## 18. Authorization Packaging Matrix

| Enablement Item | Operation classifications | Highest risk | Shared UI authority dependency | Capture-specific authority required | Current authorization | Execution permitted |
| --- | --- | --- | --- | --- | --- | --- |
| CAP-ENABLE-001 | R0/R1 planning; future R2/R3/R4 as applicable | R4 boundary | Required; inherited only | Required in a later independent review | Not granted | No |
| CAP-ENABLE-002 | R0/R1 planning; future R2/R3/R4 as applicable | R4 boundary | Required; inherited only | Required in a later independent review | Not granted | No |
| CAP-ENABLE-003 | R0/R1 planning; future R2/R3/R4 as applicable | R4 boundary | Required; inherited only | Required in a later independent review | Not granted | No |
| CAP-ENABLE-004 | R0/R1 planning; future R2/R3/R4 as applicable | R4 boundary | Required; inherited only | Required in a later independent review | Not granted | No |
| CAP-ENABLE-005 | R0/R1 planning; future R2/R3/R4 as applicable | R4 boundary | Required; inherited only | Required in a later independent review | Not granted | No |
| CAP-ENABLE-006 | R0/R1 planning; future R2/R3/R4 as applicable | R4 boundary | Required; inherited only | Required in a later independent review | Not granted | No |
| CAP-ENABLE-007 | R0/R1 planning; future R2/R3/R4 as applicable | R4 boundary | Required; inherited only | Required in a later independent review | Not granted | No |


固定：

- Current authorization: Not granted
- Execution permitted: No
- 本文件不得建立 capture runtime、evidence write 或 display mutation authorization。
- Shared UI authority 與 Capture-specific authority 必須分開。

### 18.1 Per-item authorization invariant

每個 Enablement Item 都必須保留相同的未授權狀態：

| Enablement Item | Current authorization | Execution permitted |
|---|---|---|
| CAP-ENABLE-001 | Current authorization: Not granted | Execution permitted: No |
| CAP-ENABLE-002 | Current authorization: Not granted | Execution permitted: No |
| CAP-ENABLE-003 | Current authorization: Not granted | Execution permitted: No |
| CAP-ENABLE-004 | Current authorization: Not granted | Execution permitted: No |
| CAP-ENABLE-005 | Current authorization: Not granted | Execution permitted: No |
| CAP-ENABLE-006 | Current authorization: Not granted | Execution permitted: No |
| CAP-ENABLE-007 | Current authorization: Not granted | Execution permitted: No |

## 19. Enablement Completeness Matrix

| Blocking Action | Closure Action | Enablement Item | Specification complete | Shared authority identified | Capture authority identified | Evidence obligation identified | Remaining gap |
| --- | --- | --- | --- | --- | --- | --- | --- |
| CAP-BA-001 | CAP-CLOSE-001 | CAP-ENABLE-001 | Partially | Partially | No | Partially | Windows 11 x64 baseline、WinUI 3／WPF host path、.NET／Windows SDK 與 Windows App SDK identity 尚未由官方來源與本機唯讀證據配對。 |
| CAP-BA-002 | CAP-CLOSE-002 | CAP-ENABLE-002 | Partially | Partially | No | Partially | 五個候選方案的精確 API／SDK identity、graphics device、interop 與 packaged／unpackaged 邊界尚未固定。 |
| CAP-BA-003 | CAP-CLOSE-003 | CAP-ENABLE-003 | No | Partially | No | Partially | 隔離實驗專案路徑、target framework、package identity、restore 與 build scope 尚未取得人類授權；不得在產品 source tree 建立。 |
| CAP-BA-004 | CAP-CLOSE-004 | CAP-ENABLE-004 | Partially | Partially | No | Partially | 固定 surface、色塊、邊框、grid、markers、文字、alpha、overlay-like 與後期場景的 future asset contract 尚未轉成可授權的實驗規格。 |
| CAP-BA-005 | CAP-CLOSE-005 | CAP-ENABLE-005 | Partially | Partially | No | Partially | virtual screen、monitor bounds、DIP、negative coordinates、crop edge、rounding、timestamp 與 pixel-difference method 尚未形成產品級決策；rounding 維持 TBD。 |
| CAP-BA-006 | CAP-CLOSE-006 | CAP-ENABLE-006 | Partially | Partially | No | Partially | privacy review、retention、evidence root、diagnostic log、cleanup manifest 與 rollback evidence 尚未取得 storage/write 授權。 |
| CAP-BA-007 | CAP-CLOSE-007 | CAP-ENABLE-007 | No | Partially | No | Partially | Capture API invocation、runtime execution、display/system mutation、stop rule 與獨立 capture authorization review 尚未被授權。 |


Specification complete 的 Yes 只表示規格足以進入後續 Authorization Request，不代表已授權或已執行。所有 capture authority 仍為 No。

## 20. Final Enablement Status

本文件目前狀態：

- Final Enablement Status: Not ready to request capture prerequisite closure execution authorization
- Closure Execution Authorized: No
- Build Verification: Not performed
- Runtime Verification: Not performed
- Capture Runtime Spike Authorized: No
- Evidence Write Authorized: No
- Capture Decision: Not made
- Rendering Decision: Not made

判定依據：

Open CAP-ENABLE-GAP + Shared UI authority dependencies + Candidate identity completeness + Project／Package／Restore／Build scope completeness + Synthetic Scene completeness + Coordinate／Evidence obligation completeness → Final Enablement Status。

即使未來結果改為 Ready，仍不能推導為已授權、已 Build、已 Runtime、已 Capture 或已寫入 Evidence。

## 21. Detailed Enablement Item Specifications

以下七節各自包含完整固定欄位；空白、未知或尚未授權之處以 TBD、Unknown、Partially specified、Blocked 或 Deferred 表示。

### CAP-ENABLE-001

| Fixed field | Specification |
| --- | --- |
| Enablement Item ID | CAP-ENABLE-001 |
| Source Closure Action | CAP-CLOSE-001 |
| Source Blocking Action | CAP-BA-001 |
| Related prerequisites | CAP-PREQ-001..004 |
| Related blockers | CAP-BLOCK-001 |
| Related pairs | CAP-PAIR-001..010 |
| Related spikes | CAP-SPIKE-001..005, CAP-SPIKE-011 |
| Related upstream gates | CAP-GATE-001, CAP-GATE-002 |
| Related closure gates | CAP-CGATE-001 |
| Dependency ownership | Shared UI authority owner; capture owner not assigned |
| Shared UI source IDs | RESEARCH-TECH-UI-007, RESEARCH-TECH-UI-008, RESEARCH-TECH-UI-009；依項目繼承，不重新申請。 |
| Rendering source IDs | RESEARCH-TECH-RENDER-003；僅 future synthetic scene 邊界。 |
| Existing specification evidence | RESEARCH-TECH-CAPTURE-001..004；上游 evidence 只接受引用，不在本文件重寫。 |
| Current unresolved specification | Windows 11 x64 baseline、WinUI 3／WPF host path、.NET／Windows SDK 與 Windows App SDK identity 尚未由官方來源與本機唯讀證據配對。 |
| Required final evidence | 逐項 enablement evidence record、scope identity、authority record、cleanup confirmation；本輪不建立。 |
| Proposed enablement operation | 將 closure action 所需的 future operation 拆成可審查、可停止、可 rollback 的最小規格單元。 |
| Operation classifications | Official-source research R0；Local read-only inspection R0；Repository documentation mutation R1；future execution items 依最高風險標示。 |
| Exact scope | 只涵蓋 CAP-CLOSE-001、CAP-BA-001 及其明列 dependency；不得擴大到產品 source tree。 |
| Explicit exclusions | 本文件不執行 operation、不建立 project／asset／result、不選擇 candidate、不開始正式截圖功能。 |
| Official-source lookup required | Yes；未來僅查 Microsoft Learn、Windows App SDK、WinUI、WPF、DXGI、GDI 或對應官方 API 文件；本輪未查。 |
| Local read-only inspection required | Yes；只允許確認 exact path、tool identity、OS／display／SDK evidence；本輪未做。 |
| Network access required | No for this document；future official-source lookup 另受授權控制。 |
| Package acquisition required | No in this document；future operation must record identity、version、source、cache effect。 |
| Installation required | No in this document；任何 installation 另行授權。 |
| Repository mutation required | Documentation file only；不得建立 product／experiment source。 |
| Experimental asset required | Specification of future asset: Yes；creation: No。 |
| Experimental project required | Specification of isolated project: Yes；creation: No。 |
| Restore required | No in this document；future restore is a separate operation。 |
| Build required | No in this document；future build is separate from Run。 |
| Capture API invocation required | No；future capture runtime requires an independent authorization boundary。 |
| Runtime execution required | No；future runtime remains outside current permission。 |
| Evidence persistence required | No；future persistence requires evidence-write authorization。 |
| Display／system mutation required | No；no display change, window, cursor, overlay or system state mutation。 |
| Administrator privilege required | Unknown；must be explicitly checked before any future operation; not requested。 |
| Human authorization required | Yes；Current authorization: Not granted。 |
| Expected files／directories | Only docs/Research/Technology/24-capture-backend-prerequisite-execution-enablement-specification.md；future experiment/result roots are planned, not created。 |
| Expected Package／Cache effects | None in this document；no package query, download, restore or cache mutation。 |
| Expected machine effects | None；no process、window、display、GPU、file output or system setting mutation。 |
| Privacy impact | No runtime data collected；future frame、metadata、logs and environment records require privacy review and retention boundary。 |
| Risk classification | R0 for planning/review；future package/build R2、installation R3、runtime/evidence/display R4。 |
| Failure impact | No runtime impact now；future failure must stop at the named operation and preserve diagnostic context without creating false evidence。 |
| Stop conditions | Missing authority、identity、scope、privacy boundary、cleanup manifest、unexpected output、unexpected display change or any instruction outside this document。 |
| Rollback／cleanup requirement | No cleanup needed for this document beyond reverting the documentation change if requested；future operations require manifest, output root and rollback owner。 |
| Success condition | The enablement item is sufficiently specified for a later authorization review while remaining unexecuted。 |
| Result artifact obligation | This document only; no runtime result, frame, PNG, log, measurement or evidence artifact。 |
| Resulting prerequisite recommendation | Keep listed prerequisites linked to CAP-CLOSE-001; do not mark them completed。 |
| Resulting blocker recommendation | Keep CAP-BLOCK-001 as Blocked or Deferred according to parent evidence; no blocker closure is asserted。 |
| Resulting pair recommendation | Keep CAP-PAIR-001..010 in scope; no ranking, exclusion or candidate selection。 |
| Phase C1 effect | Defines a minimum specification boundary for Phase C1; does not grant entry or execution permission。 |
| Owner | Shared UI authority owner; capture owner not assigned |
| Status | Partially specified |
| Open questions | 誰是 owner、何時取得 shared UI authority、何時可提出 capture-specific authorization request、哪些 evidence fields由真人核准。 |


### CAP-ENABLE-002

| Fixed field | Specification |
| --- | --- |
| Enablement Item ID | CAP-ENABLE-002 |
| Source Closure Action | CAP-CLOSE-002 |
| Source Blocking Action | CAP-BA-002 |
| Related prerequisites | CAP-PREQ-003..012 |
| Related blockers | CAP-BLOCK-002 |
| Related pairs | CAP-PAIR-001..010 |
| Related spikes | CAP-SPIKE-001..005, CAP-SPIKE-011 |
| Related upstream gates | CAP-GATE-002, CAP-GATE-003 |
| Related closure gates | CAP-CGATE-002, CAP-CGATE-003 |
| Dependency ownership | Capture Backend research owner; candidate decision owner not assigned |
| Shared UI source IDs | RESEARCH-TECH-UI-007, RESEARCH-TECH-UI-008, RESEARCH-TECH-UI-009；依項目繼承，不重新申請。 |
| Rendering source IDs | RESEARCH-TECH-RENDER-003；僅 future synthetic scene 邊界。 |
| Existing specification evidence | RESEARCH-TECH-CAPTURE-001..004；上游 evidence 只接受引用，不在本文件重寫。 |
| Current unresolved specification | 五個候選方案的精確 API／SDK identity、graphics device、interop 與 packaged／unpackaged 邊界尚未固定。 |
| Required final evidence | 逐項 enablement evidence record、scope identity、authority record、cleanup confirmation；本輪不建立。 |
| Proposed enablement operation | 將 closure action 所需的 future operation 拆成可審查、可停止、可 rollback 的最小規格單元。 |
| Operation classifications | Official-source research R0；Local read-only inspection R0；Repository documentation mutation R1；future execution items 依最高風險標示。 |
| Exact scope | 只涵蓋 CAP-CLOSE-002、CAP-BA-002 及其明列 dependency；不得擴大到產品 source tree。 |
| Explicit exclusions | 本文件不執行 operation、不建立 project／asset／result、不選擇 candidate、不開始正式截圖功能。 |
| Official-source lookup required | Yes；未來僅查 Microsoft Learn、Windows App SDK、WinUI、WPF、DXGI、GDI 或對應官方 API 文件；本輪未查。 |
| Local read-only inspection required | Yes；只允許確認 exact path、tool identity、OS／display／SDK evidence；本輪未做。 |
| Network access required | No for this document；future official-source lookup 另受授權控制。 |
| Package acquisition required | No in this document；future operation must record identity、version、source、cache effect。 |
| Installation required | No in this document；任何 installation 另行授權。 |
| Repository mutation required | Documentation file only；不得建立 product／experiment source。 |
| Experimental asset required | Specification of future asset: Yes；creation: No。 |
| Experimental project required | Specification of isolated project: Yes；creation: No。 |
| Restore required | No in this document；future restore is a separate operation。 |
| Build required | No in this document；future build is separate from Run。 |
| Capture API invocation required | No；future capture runtime requires an independent authorization boundary。 |
| Runtime execution required | No；future runtime remains outside current permission。 |
| Evidence persistence required | No；future persistence requires evidence-write authorization。 |
| Display／system mutation required | No；no display change, window, cursor, overlay or system state mutation。 |
| Administrator privilege required | Unknown；must be explicitly checked before any future operation; not requested。 |
| Human authorization required | Yes；Current authorization: Not granted。 |
| Expected files／directories | Only docs/Research/Technology/24-capture-backend-prerequisite-execution-enablement-specification.md；future experiment/result roots are planned, not created。 |
| Expected Package／Cache effects | None in this document；no package query, download, restore or cache mutation。 |
| Expected machine effects | None；no process、window、display、GPU、file output or system setting mutation。 |
| Privacy impact | No runtime data collected；future frame、metadata、logs and environment records require privacy review and retention boundary。 |
| Risk classification | R0 for planning/review；future package/build R2、installation R3、runtime/evidence/display R4。 |
| Failure impact | No runtime impact now；future failure must stop at the named operation and preserve diagnostic context without creating false evidence。 |
| Stop conditions | Missing authority、identity、scope、privacy boundary、cleanup manifest、unexpected output、unexpected display change or any instruction outside this document。 |
| Rollback／cleanup requirement | No cleanup needed for this document beyond reverting the documentation change if requested；future operations require manifest, output root and rollback owner。 |
| Success condition | The enablement item is sufficiently specified for a later authorization review while remaining unexecuted。 |
| Result artifact obligation | This document only; no runtime result, frame, PNG, log, measurement or evidence artifact。 |
| Resulting prerequisite recommendation | Keep listed prerequisites linked to CAP-CLOSE-002; do not mark them completed。 |
| Resulting blocker recommendation | Keep CAP-BLOCK-002 as Blocked or Deferred according to parent evidence; no blocker closure is asserted。 |
| Resulting pair recommendation | Keep CAP-PAIR-001..010 in scope; no ranking, exclusion or candidate selection。 |
| Phase C1 effect | Defines a minimum specification boundary for Phase C1; does not grant entry or execution permission。 |
| Owner | Capture Backend research owner; candidate decision owner not assigned |
| Status | Partially specified |
| Open questions | 誰是 owner、何時取得 shared UI authority、何時可提出 capture-specific authorization request、哪些 evidence fields由真人核准。 |


### CAP-ENABLE-003

| Fixed field | Specification |
| --- | --- |
| Enablement Item ID | CAP-ENABLE-003 |
| Source Closure Action | CAP-CLOSE-003 |
| Source Blocking Action | CAP-BA-003 |
| Related prerequisites | CAP-PREQ-027 |
| Related blockers | CAP-BLOCK-010 |
| Related pairs | CAP-PAIR-001..010 |
| Related spikes | CAP-SPIKE-001..005, CAP-SPIKE-011 |
| Related upstream gates | CAP-GATE-004 |
| Related closure gates | CAP-CGATE-001, CAP-CGATE-003, CAP-CGATE-008 |
| Dependency ownership | Experimental project and build owner not assigned |
| Shared UI source IDs | RESEARCH-TECH-UI-007, RESEARCH-TECH-UI-008, RESEARCH-TECH-UI-009；依項目繼承，不重新申請。 |
| Rendering source IDs | RESEARCH-TECH-RENDER-003；僅 future synthetic scene 邊界。 |
| Existing specification evidence | RESEARCH-TECH-CAPTURE-001..004；上游 evidence 只接受引用，不在本文件重寫。 |
| Current unresolved specification | 隔離實驗專案路徑、target framework、package identity、restore 與 build scope 尚未取得人類授權；不得在產品 source tree 建立。 |
| Required final evidence | 逐項 enablement evidence record、scope identity、authority record、cleanup confirmation；本輪不建立。 |
| Proposed enablement operation | 將 closure action 所需的 future operation 拆成可審查、可停止、可 rollback 的最小規格單元。 |
| Operation classifications | Official-source research R0；Local read-only inspection R0；Repository documentation mutation R1；future execution items 依最高風險標示。 |
| Exact scope | 只涵蓋 CAP-CLOSE-003、CAP-BA-003 及其明列 dependency；不得擴大到產品 source tree。 |
| Explicit exclusions | 本文件不執行 operation、不建立 project／asset／result、不選擇 candidate、不開始正式截圖功能。 |
| Official-source lookup required | Yes；未來僅查 Microsoft Learn、Windows App SDK、WinUI、WPF、DXGI、GDI 或對應官方 API 文件；本輪未查。 |
| Local read-only inspection required | Yes；只允許確認 exact path、tool identity、OS／display／SDK evidence；本輪未做。 |
| Network access required | No for this document；future official-source lookup 另受授權控制。 |
| Package acquisition required | No in this document；future operation must record identity、version、source、cache effect。 |
| Installation required | No in this document；任何 installation 另行授權。 |
| Repository mutation required | Documentation file only；不得建立 product／experiment source。 |
| Experimental asset required | Specification of future asset: Yes；creation: No。 |
| Experimental project required | Specification of isolated project: Yes；creation: No。 |
| Restore required | No in this document；future restore is a separate operation。 |
| Build required | No in this document；future build is separate from Run。 |
| Capture API invocation required | No；future capture runtime requires an independent authorization boundary。 |
| Runtime execution required | No；future runtime remains outside current permission。 |
| Evidence persistence required | No；future persistence requires evidence-write authorization。 |
| Display／system mutation required | No；no display change, window, cursor, overlay or system state mutation。 |
| Administrator privilege required | Unknown；must be explicitly checked before any future operation; not requested。 |
| Human authorization required | Yes；Current authorization: Not granted。 |
| Expected files／directories | Only docs/Research/Technology/24-capture-backend-prerequisite-execution-enablement-specification.md；future experiment/result roots are planned, not created。 |
| Expected Package／Cache effects | None in this document；no package query, download, restore or cache mutation。 |
| Expected machine effects | None；no process、window、display、GPU、file output or system setting mutation。 |
| Privacy impact | No runtime data collected；future frame、metadata、logs and environment records require privacy review and retention boundary。 |
| Risk classification | R0 for planning/review；future package/build R2、installation R3、runtime/evidence/display R4。 |
| Failure impact | No runtime impact now；future failure must stop at the named operation and preserve diagnostic context without creating false evidence。 |
| Stop conditions | Missing authority、identity、scope、privacy boundary、cleanup manifest、unexpected output、unexpected display change or any instruction outside this document。 |
| Rollback／cleanup requirement | No cleanup needed for this document beyond reverting the documentation change if requested；future operations require manifest, output root and rollback owner。 |
| Success condition | The enablement item is sufficiently specified for a later authorization review while remaining unexecuted。 |
| Result artifact obligation | This document only; no runtime result, frame, PNG, log, measurement or evidence artifact。 |
| Resulting prerequisite recommendation | Keep listed prerequisites linked to CAP-CLOSE-003; do not mark them completed。 |
| Resulting blocker recommendation | Keep CAP-BLOCK-010 as Blocked or Deferred according to parent evidence; no blocker closure is asserted。 |
| Resulting pair recommendation | Keep CAP-PAIR-001..010 in scope; no ranking, exclusion or candidate selection。 |
| Phase C1 effect | Defines a minimum specification boundary for Phase C1; does not grant entry or execution permission。 |
| Owner | Experimental project and build owner not assigned |
| Status | Blocked |
| Open questions | 誰是 owner、何時取得 shared UI authority、何時可提出 capture-specific authorization request、哪些 evidence fields由真人核准。 |


### CAP-ENABLE-004

| Fixed field | Specification |
| --- | --- |
| Enablement Item ID | CAP-ENABLE-004 |
| Source Closure Action | CAP-CLOSE-004 |
| Source Blocking Action | CAP-BA-004 |
| Related prerequisites | CAP-PREQ-013, CAP-PREQ-017..021 |
| Related blockers | CAP-BLOCK-003, CAP-BLOCK-005, CAP-BLOCK-006, CAP-BLOCK-012 |
| Related pairs | CAP-PAIR-001..010 |
| Related spikes | CAP-SPIKE-006..010, CAP-SPIKE-012 |
| Related upstream gates | CAP-GATE-005 |
| Related closure gates | CAP-CGATE-004 |
| Dependency ownership | Synthetic scene specification owner not assigned |
| Shared UI source IDs | RESEARCH-TECH-UI-007, RESEARCH-TECH-UI-008, RESEARCH-TECH-UI-009；依項目繼承，不重新申請。 |
| Rendering source IDs | RESEARCH-TECH-RENDER-003；僅 future synthetic scene 邊界。 |
| Existing specification evidence | RESEARCH-TECH-CAPTURE-001..004；上游 evidence 只接受引用，不在本文件重寫。 |
| Current unresolved specification | 固定 surface、色塊、邊框、grid、markers、文字、alpha、overlay-like 與後期場景的 future asset contract 尚未轉成可授權的實驗規格。 |
| Required final evidence | 逐項 enablement evidence record、scope identity、authority record、cleanup confirmation；本輪不建立。 |
| Proposed enablement operation | 將 closure action 所需的 future operation 拆成可審查、可停止、可 rollback 的最小規格單元。 |
| Operation classifications | Official-source research R0；Local read-only inspection R0；Repository documentation mutation R1；future execution items 依最高風險標示。 |
| Exact scope | 只涵蓋 CAP-CLOSE-004、CAP-BA-004 及其明列 dependency；不得擴大到產品 source tree。 |
| Explicit exclusions | 本文件不執行 operation、不建立 project／asset／result、不選擇 candidate、不開始正式截圖功能。 |
| Official-source lookup required | Yes；未來僅查 Microsoft Learn、Windows App SDK、WinUI、WPF、DXGI、GDI 或對應官方 API 文件；本輪未查。 |
| Local read-only inspection required | Yes；只允許確認 exact path、tool identity、OS／display／SDK evidence；本輪未做。 |
| Network access required | No for this document；future official-source lookup 另受授權控制。 |
| Package acquisition required | No in this document；future operation must record identity、version、source、cache effect。 |
| Installation required | No in this document；任何 installation 另行授權。 |
| Repository mutation required | Documentation file only；不得建立 product／experiment source。 |
| Experimental asset required | Specification of future asset: Yes；creation: No。 |
| Experimental project required | Specification of isolated project: Yes；creation: No。 |
| Restore required | No in this document；future restore is a separate operation。 |
| Build required | No in this document；future build is separate from Run。 |
| Capture API invocation required | No；future capture runtime requires an independent authorization boundary。 |
| Runtime execution required | No；future runtime remains outside current permission。 |
| Evidence persistence required | No；future persistence requires evidence-write authorization。 |
| Display／system mutation required | No；no display change, window, cursor, overlay or system state mutation。 |
| Administrator privilege required | Unknown；must be explicitly checked before any future operation; not requested。 |
| Human authorization required | Yes；Current authorization: Not granted。 |
| Expected files／directories | Only docs/Research/Technology/24-capture-backend-prerequisite-execution-enablement-specification.md；future experiment/result roots are planned, not created。 |
| Expected Package／Cache effects | None in this document；no package query, download, restore or cache mutation。 |
| Expected machine effects | None；no process、window、display、GPU、file output or system setting mutation。 |
| Privacy impact | No runtime data collected；future frame、metadata、logs and environment records require privacy review and retention boundary。 |
| Risk classification | R0 for planning/review；future package/build R2、installation R3、runtime/evidence/display R4。 |
| Failure impact | No runtime impact now；future failure must stop at the named operation and preserve diagnostic context without creating false evidence。 |
| Stop conditions | Missing authority、identity、scope、privacy boundary、cleanup manifest、unexpected output、unexpected display change or any instruction outside this document。 |
| Rollback／cleanup requirement | No cleanup needed for this document beyond reverting the documentation change if requested；future operations require manifest, output root and rollback owner。 |
| Success condition | The enablement item is sufficiently specified for a later authorization review while remaining unexecuted。 |
| Result artifact obligation | This document only; no runtime result, frame, PNG, log, measurement or evidence artifact。 |
| Resulting prerequisite recommendation | Keep listed prerequisites linked to CAP-CLOSE-004; do not mark them completed。 |
| Resulting blocker recommendation | Keep CAP-BLOCK-003, CAP-BLOCK-005, CAP-BLOCK-006, CAP-BLOCK-012 as Blocked or Deferred according to parent evidence; no blocker closure is asserted。 |
| Resulting pair recommendation | Keep CAP-PAIR-001..010 in scope; no ranking, exclusion or candidate selection。 |
| Phase C1 effect | Defines a minimum specification boundary for Phase C1; does not grant entry or execution permission。 |
| Owner | Synthetic scene specification owner not assigned |
| Status | Partially specified |
| Open questions | 誰是 owner、何時取得 shared UI authority、何時可提出 capture-specific authorization request、哪些 evidence fields由真人核准。 |


### CAP-ENABLE-005

| Fixed field | Specification |
| --- | --- |
| Enablement Item ID | CAP-ENABLE-005 |
| Source Closure Action | CAP-CLOSE-005 |
| Source Blocking Action | CAP-BA-005 |
| Related prerequisites | CAP-PREQ-014..016, CAP-PREQ-022..023 |
| Related blockers | CAP-BLOCK-004, CAP-BLOCK-007 |
| Related pairs | CAP-PAIR-001..010 |
| Related spikes | CAP-SPIKE-002..005 |
| Related upstream gates | CAP-GATE-006, CAP-GATE-007 |
| Related closure gates | CAP-CGATE-005, CAP-CGATE-006, CAP-CGATE-007 |
| Dependency ownership | Coordinate and evidence-method owner not assigned |
| Shared UI source IDs | RESEARCH-TECH-UI-007, RESEARCH-TECH-UI-008, RESEARCH-TECH-UI-009；依項目繼承，不重新申請。 |
| Rendering source IDs | RESEARCH-TECH-RENDER-003；僅 future synthetic scene 邊界。 |
| Existing specification evidence | RESEARCH-TECH-CAPTURE-001..004；上游 evidence 只接受引用，不在本文件重寫。 |
| Current unresolved specification | virtual screen、monitor bounds、DIP、negative coordinates、crop edge、rounding、timestamp 與 pixel-difference method 尚未形成產品級決策；rounding 維持 TBD。 |
| Required final evidence | 逐項 enablement evidence record、scope identity、authority record、cleanup confirmation；本輪不建立。 |
| Proposed enablement operation | 將 closure action 所需的 future operation 拆成可審查、可停止、可 rollback 的最小規格單元。 |
| Operation classifications | Official-source research R0；Local read-only inspection R0；Repository documentation mutation R1；future execution items 依最高風險標示。 |
| Exact scope | 只涵蓋 CAP-CLOSE-005、CAP-BA-005 及其明列 dependency；不得擴大到產品 source tree。 |
| Explicit exclusions | 本文件不執行 operation、不建立 project／asset／result、不選擇 candidate、不開始正式截圖功能。 |
| Official-source lookup required | Yes；未來僅查 Microsoft Learn、Windows App SDK、WinUI、WPF、DXGI、GDI 或對應官方 API 文件；本輪未查。 |
| Local read-only inspection required | Yes；只允許確認 exact path、tool identity、OS／display／SDK evidence；本輪未做。 |
| Network access required | No for this document；future official-source lookup 另受授權控制。 |
| Package acquisition required | No in this document；future operation must record identity、version、source、cache effect。 |
| Installation required | No in this document；任何 installation 另行授權。 |
| Repository mutation required | Documentation file only；不得建立 product／experiment source。 |
| Experimental asset required | Specification of future asset: Yes；creation: No。 |
| Experimental project required | Specification of isolated project: Yes；creation: No。 |
| Restore required | No in this document；future restore is a separate operation。 |
| Build required | No in this document；future build is separate from Run。 |
| Capture API invocation required | No；future capture runtime requires an independent authorization boundary。 |
| Runtime execution required | No；future runtime remains outside current permission。 |
| Evidence persistence required | No；future persistence requires evidence-write authorization。 |
| Display／system mutation required | No；no display change, window, cursor, overlay or system state mutation。 |
| Administrator privilege required | Unknown；must be explicitly checked before any future operation; not requested。 |
| Human authorization required | Yes；Current authorization: Not granted。 |
| Expected files／directories | Only docs/Research/Technology/24-capture-backend-prerequisite-execution-enablement-specification.md；future experiment/result roots are planned, not created。 |
| Expected Package／Cache effects | None in this document；no package query, download, restore or cache mutation。 |
| Expected machine effects | None；no process、window、display、GPU、file output or system setting mutation。 |
| Privacy impact | No runtime data collected；future frame、metadata、logs and environment records require privacy review and retention boundary。 |
| Risk classification | R0 for planning/review；future package/build R2、installation R3、runtime/evidence/display R4。 |
| Failure impact | No runtime impact now；future failure must stop at the named operation and preserve diagnostic context without creating false evidence。 |
| Stop conditions | Missing authority、identity、scope、privacy boundary、cleanup manifest、unexpected output、unexpected display change or any instruction outside this document。 |
| Rollback／cleanup requirement | No cleanup needed for this document beyond reverting the documentation change if requested；future operations require manifest, output root and rollback owner。 |
| Success condition | The enablement item is sufficiently specified for a later authorization review while remaining unexecuted。 |
| Result artifact obligation | This document only; no runtime result, frame, PNG, log, measurement or evidence artifact。 |
| Resulting prerequisite recommendation | Keep listed prerequisites linked to CAP-CLOSE-005; do not mark them completed。 |
| Resulting blocker recommendation | Keep CAP-BLOCK-004, CAP-BLOCK-007 as Blocked or Deferred according to parent evidence; no blocker closure is asserted。 |
| Resulting pair recommendation | Keep CAP-PAIR-001..010 in scope; no ranking, exclusion or candidate selection。 |
| Phase C1 effect | Defines a minimum specification boundary for Phase C1; does not grant entry or execution permission。 |
| Owner | Coordinate and evidence-method owner not assigned |
| Status | Partially specified |
| Open questions | 誰是 owner、何時取得 shared UI authority、何時可提出 capture-specific authorization request、哪些 evidence fields由真人核准。 |


### CAP-ENABLE-006

| Fixed field | Specification |
| --- | --- |
| Enablement Item ID | CAP-ENABLE-006 |
| Source Closure Action | CAP-CLOSE-006 |
| Source Blocking Action | CAP-BA-006 |
| Related prerequisites | CAP-PREQ-024..026, CAP-PREQ-029..030 |
| Related blockers | CAP-BLOCK-008, CAP-BLOCK-009 |
| Related pairs | CAP-PAIR-001..010 |
| Related spikes | CAP-SPIKE-001..005, CAP-SPIKE-012 |
| Related upstream gates | CAP-GATE-007, CAP-GATE-008, CAP-GATE-009 |
| Related closure gates | CAP-CGATE-007, CAP-CGATE-008, CAP-CGATE-009 |
| Dependency ownership | Evidence governance and cleanup owner not assigned |
| Shared UI source IDs | RESEARCH-TECH-UI-007, RESEARCH-TECH-UI-008, RESEARCH-TECH-UI-009；依項目繼承，不重新申請。 |
| Rendering source IDs | RESEARCH-TECH-RENDER-003；僅 future synthetic scene 邊界。 |
| Existing specification evidence | RESEARCH-TECH-CAPTURE-001..004；上游 evidence 只接受引用，不在本文件重寫。 |
| Current unresolved specification | privacy review、retention、evidence root、diagnostic log、cleanup manifest 與 rollback evidence 尚未取得 storage/write 授權。 |
| Required final evidence | 逐項 enablement evidence record、scope identity、authority record、cleanup confirmation；本輪不建立。 |
| Proposed enablement operation | 將 closure action 所需的 future operation 拆成可審查、可停止、可 rollback 的最小規格單元。 |
| Operation classifications | Official-source research R0；Local read-only inspection R0；Repository documentation mutation R1；future execution items 依最高風險標示。 |
| Exact scope | 只涵蓋 CAP-CLOSE-006、CAP-BA-006 及其明列 dependency；不得擴大到產品 source tree。 |
| Explicit exclusions | 本文件不執行 operation、不建立 project／asset／result、不選擇 candidate、不開始正式截圖功能。 |
| Official-source lookup required | Yes；未來僅查 Microsoft Learn、Windows App SDK、WinUI、WPF、DXGI、GDI 或對應官方 API 文件；本輪未查。 |
| Local read-only inspection required | Yes；只允許確認 exact path、tool identity、OS／display／SDK evidence；本輪未做。 |
| Network access required | No for this document；future official-source lookup 另受授權控制。 |
| Package acquisition required | No in this document；future operation must record identity、version、source、cache effect。 |
| Installation required | No in this document；任何 installation 另行授權。 |
| Repository mutation required | Documentation file only；不得建立 product／experiment source。 |
| Experimental asset required | Specification of future asset: Yes；creation: No。 |
| Experimental project required | Specification of isolated project: Yes；creation: No。 |
| Restore required | No in this document；future restore is a separate operation。 |
| Build required | No in this document；future build is separate from Run。 |
| Capture API invocation required | No；future capture runtime requires an independent authorization boundary。 |
| Runtime execution required | No；future runtime remains outside current permission。 |
| Evidence persistence required | No；future persistence requires evidence-write authorization。 |
| Display／system mutation required | No；no display change, window, cursor, overlay or system state mutation。 |
| Administrator privilege required | Unknown；must be explicitly checked before any future operation; not requested。 |
| Human authorization required | Yes；Current authorization: Not granted。 |
| Expected files／directories | Only docs/Research/Technology/24-capture-backend-prerequisite-execution-enablement-specification.md；future experiment/result roots are planned, not created。 |
| Expected Package／Cache effects | None in this document；no package query, download, restore or cache mutation。 |
| Expected machine effects | None；no process、window、display、GPU、file output or system setting mutation。 |
| Privacy impact | No runtime data collected；future frame、metadata、logs and environment records require privacy review and retention boundary。 |
| Risk classification | R0 for planning/review；future package/build R2、installation R3、runtime/evidence/display R4。 |
| Failure impact | No runtime impact now；future failure must stop at the named operation and preserve diagnostic context without creating false evidence。 |
| Stop conditions | Missing authority、identity、scope、privacy boundary、cleanup manifest、unexpected output、unexpected display change or any instruction outside this document。 |
| Rollback／cleanup requirement | No cleanup needed for this document beyond reverting the documentation change if requested；future operations require manifest, output root and rollback owner。 |
| Success condition | The enablement item is sufficiently specified for a later authorization review while remaining unexecuted。 |
| Result artifact obligation | This document only; no runtime result, frame, PNG, log, measurement or evidence artifact。 |
| Resulting prerequisite recommendation | Keep listed prerequisites linked to CAP-CLOSE-006; do not mark them completed。 |
| Resulting blocker recommendation | Keep CAP-BLOCK-008, CAP-BLOCK-009 as Blocked or Deferred according to parent evidence; no blocker closure is asserted。 |
| Resulting pair recommendation | Keep CAP-PAIR-001..010 in scope; no ranking, exclusion or candidate selection。 |
| Phase C1 effect | Defines a minimum specification boundary for Phase C1; does not grant entry or execution permission。 |
| Owner | Evidence governance and cleanup owner not assigned |
| Status | Partially specified |
| Open questions | 誰是 owner、何時取得 shared UI authority、何時可提出 capture-specific authorization request、哪些 evidence fields由真人核准。 |


### CAP-ENABLE-007

| Fixed field | Specification |
| --- | --- |
| Enablement Item ID | CAP-ENABLE-007 |
| Source Closure Action | CAP-CLOSE-007 |
| Source Blocking Action | CAP-BA-007 |
| Related prerequisites | CAP-PREQ-028 |
| Related blockers | CAP-BLOCK-011 |
| Related pairs | CAP-PAIR-001..010 |
| Related spikes | CAP-SPIKE-001..005, CAP-SPIKE-011..012 |
| Related upstream gates | CAP-GATE-010, CAP-GATE-011 |
| Related closure gates | CAP-CGATE-008, CAP-CGATE-010 |
| Dependency ownership | Runtime authority owner not assigned |
| Shared UI source IDs | RESEARCH-TECH-UI-007, RESEARCH-TECH-UI-008, RESEARCH-TECH-UI-009；依項目繼承，不重新申請。 |
| Rendering source IDs | RESEARCH-TECH-RENDER-003；僅 future synthetic scene 邊界。 |
| Existing specification evidence | RESEARCH-TECH-CAPTURE-001..004；上游 evidence 只接受引用，不在本文件重寫。 |
| Current unresolved specification | Capture API invocation、runtime execution、display/system mutation、stop rule 與獨立 capture authorization review 尚未被授權。 |
| Required final evidence | 逐項 enablement evidence record、scope identity、authority record、cleanup confirmation；本輪不建立。 |
| Proposed enablement operation | 將 closure action 所需的 future operation 拆成可審查、可停止、可 rollback 的最小規格單元。 |
| Operation classifications | Official-source research R0；Local read-only inspection R0；Repository documentation mutation R1；future execution items 依最高風險標示。 |
| Exact scope | 只涵蓋 CAP-CLOSE-007、CAP-BA-007 及其明列 dependency；不得擴大到產品 source tree。 |
| Explicit exclusions | 本文件不執行 operation、不建立 project／asset／result、不選擇 candidate、不開始正式截圖功能。 |
| Official-source lookup required | Yes；未來僅查 Microsoft Learn、Windows App SDK、WinUI、WPF、DXGI、GDI 或對應官方 API 文件；本輪未查。 |
| Local read-only inspection required | Yes；只允許確認 exact path、tool identity、OS／display／SDK evidence；本輪未做。 |
| Network access required | No for this document；future official-source lookup 另受授權控制。 |
| Package acquisition required | No in this document；future operation must record identity、version、source、cache effect。 |
| Installation required | No in this document；任何 installation 另行授權。 |
| Repository mutation required | Documentation file only；不得建立 product／experiment source。 |
| Experimental asset required | Specification of future asset: Yes；creation: No。 |
| Experimental project required | Specification of isolated project: Yes；creation: No。 |
| Restore required | No in this document；future restore is a separate operation。 |
| Build required | No in this document；future build is separate from Run。 |
| Capture API invocation required | No；future capture runtime requires an independent authorization boundary。 |
| Runtime execution required | No；future runtime remains outside current permission。 |
| Evidence persistence required | No；future persistence requires evidence-write authorization。 |
| Display／system mutation required | No；no display change, window, cursor, overlay or system state mutation。 |
| Administrator privilege required | Unknown；must be explicitly checked before any future operation; not requested。 |
| Human authorization required | Yes；Current authorization: Not granted。 |
| Expected files／directories | Only docs/Research/Technology/24-capture-backend-prerequisite-execution-enablement-specification.md；future experiment/result roots are planned, not created。 |
| Expected Package／Cache effects | None in this document；no package query, download, restore or cache mutation。 |
| Expected machine effects | None；no process、window、display、GPU、file output or system setting mutation。 |
| Privacy impact | No runtime data collected；future frame、metadata、logs and environment records require privacy review and retention boundary。 |
| Risk classification | R0 for planning/review；future package/build R2、installation R3、runtime/evidence/display R4。 |
| Failure impact | No runtime impact now；future failure must stop at the named operation and preserve diagnostic context without creating false evidence。 |
| Stop conditions | Missing authority、identity、scope、privacy boundary、cleanup manifest、unexpected output、unexpected display change or any instruction outside this document。 |
| Rollback／cleanup requirement | No cleanup needed for this document beyond reverting the documentation change if requested；future operations require manifest, output root and rollback owner。 |
| Success condition | The enablement item is sufficiently specified for a later authorization review while remaining unexecuted。 |
| Result artifact obligation | This document only; no runtime result, frame, PNG, log, measurement or evidence artifact。 |
| Resulting prerequisite recommendation | Keep listed prerequisites linked to CAP-CLOSE-007; do not mark them completed。 |
| Resulting blocker recommendation | Keep CAP-BLOCK-011 as Blocked or Deferred according to parent evidence; no blocker closure is asserted。 |
| Resulting pair recommendation | Keep CAP-PAIR-001..010 in scope; no ranking, exclusion or candidate selection。 |
| Phase C1 effect | Defines a minimum specification boundary for Phase C1; does not grant entry or execution permission。 |
| Owner | Runtime authority owner not assigned |
| Status | Blocked |
| Open questions | 誰是 owner、何時取得 shared UI authority、何時可提出 capture-specific authorization request、哪些 evidence fields由真人核准。 |



## 22. Explicit Coverage Index

本節把上游文件的範圍展開為逐一 ID，避免使用範圍文字取代可查核的 coverage。

### 22.1 Capture prerequisite coverage

| ID | Related Closure Action | Current recommendation |
|---|---|---|
| CAP-PREQ-001 | CAP-CLOSE-001 | Partially specified |
| CAP-PREQ-002 | CAP-CLOSE-001 | Partially specified |
| CAP-PREQ-003 | CAP-CLOSE-002 | Partially specified |
| CAP-PREQ-004 | CAP-CLOSE-002 | Partially specified |
| CAP-PREQ-005 | CAP-CLOSE-002 | Partially specified |
| CAP-PREQ-006 | CAP-CLOSE-002 | Partially specified |
| CAP-PREQ-007 | CAP-CLOSE-002 | Partially specified |
| CAP-PREQ-008 | CAP-CLOSE-002 | Partially specified |
| CAP-PREQ-009 | CAP-CLOSE-002 | Partially specified |
| CAP-PREQ-010 | CAP-CLOSE-002 | Partially specified |
| CAP-PREQ-011 | CAP-CLOSE-002 | Partially specified |
| CAP-PREQ-012 | CAP-CLOSE-002 | Deferred |
| CAP-PREQ-013 | CAP-CLOSE-004 | Partially specified |
| CAP-PREQ-014 | CAP-CLOSE-005 | Partially specified |
| CAP-PREQ-015 | CAP-CLOSE-005 | Partially specified |
| CAP-PREQ-016 | CAP-CLOSE-005 | Partially specified |
| CAP-PREQ-017 | CAP-CLOSE-004 | Deferred |
| CAP-PREQ-018 | CAP-CLOSE-004 | Deferred |
| CAP-PREQ-019 | CAP-CLOSE-004 | Deferred |
| CAP-PREQ-020 | CAP-CLOSE-004 | Deferred |
| CAP-PREQ-021 | CAP-CLOSE-004 | Deferred |
| CAP-PREQ-022 | CAP-CLOSE-005 | Partially specified |
| CAP-PREQ-023 | CAP-CLOSE-005 | Partially specified |
| CAP-PREQ-024 | CAP-CLOSE-006 | Deferred |
| CAP-PREQ-025 | CAP-CLOSE-006 | Partially specified |
| CAP-PREQ-026 | CAP-CLOSE-006 | Partially specified |
| CAP-PREQ-027 | CAP-CLOSE-003 | Blocked |
| CAP-PREQ-028 | CAP-CLOSE-007 | Blocked |
| CAP-PREQ-029 | CAP-CLOSE-006 | Deferred |
| CAP-PREQ-030 | CAP-CLOSE-006 | Partially specified |

### 22.2 Capture blocker coverage

| ID | Related Closure Action | Current recommendation |
|---|---|---|
| CAP-BLOCK-001 | CAP-CLOSE-001 | Blocked |
| CAP-BLOCK-002 | CAP-CLOSE-002 | Blocked |
| CAP-BLOCK-003 | CAP-CLOSE-004 | Blocked |
| CAP-BLOCK-004 | CAP-CLOSE-005 | Blocked |
| CAP-BLOCK-005 | CAP-CLOSE-004, CAP-CLOSE-006 | Deferred |
| CAP-BLOCK-006 | CAP-CLOSE-004, CAP-CLOSE-007 | Deferred |
| CAP-BLOCK-007 | CAP-CLOSE-005 | Blocked |
| CAP-BLOCK-008 | CAP-CLOSE-006 | Blocked |
| CAP-BLOCK-009 | CAP-CLOSE-006 | Blocked |
| CAP-BLOCK-010 | CAP-CLOSE-003 | Blocked |
| CAP-BLOCK-011 | CAP-CLOSE-007 | Blocked |
| CAP-BLOCK-012 | CAP-CLOSE-006, CAP-CLOSE-007 | Deferred |

### 22.3 Runtime spike coverage

| ID | Closure Action dependency | Target phase |
|---|---|---|
| CAP-SPIKE-001 | CAP-CLOSE-001..007 | C1 |
| CAP-SPIKE-002 | CAP-CLOSE-001..005 | C1 |
| CAP-SPIKE-003 | CAP-CLOSE-001..005 | C1 |
| CAP-SPIKE-004 | CAP-CLOSE-001..005 | C1/C2 |
| CAP-SPIKE-005 | CAP-CLOSE-001..006 | C1 |
| CAP-SPIKE-006 | CAP-CLOSE-004, CAP-CLOSE-006 | C2 |
| CAP-SPIKE-007 | CAP-CLOSE-004, CAP-CLOSE-005 | C2 |
| CAP-SPIKE-008 | CAP-CLOSE-004, CAP-CLOSE-006 | C2 |
| CAP-SPIKE-009 | CAP-CLOSE-004, CAP-CLOSE-006 | C2 |
| CAP-SPIKE-010 | CAP-CLOSE-006, CAP-CLOSE-007 | C3 |
| CAP-SPIKE-011 | CAP-CLOSE-001..003, CAP-CLOSE-007 | C1/C3 |
| CAP-SPIKE-012 | CAP-CLOSE-006, CAP-CLOSE-007 | C3 |

## 23. Traceability

| Source | Enablement binding | Future review input | Current state |
| --- | --- | --- | --- |
| CAP-BA-001..007 | CAP-BA → CAP-CLOSE → CAP-ENABLE | Future closure authorization review | Referenced; not executed |
| CAP-PREQ-001..030 | Prerequisite → related CAP-ENABLE | Future closure evidence | Referenced; not closed |
| CAP-BLOCK-001..012 | Blocker → CAP-CLOSE／CAP-ENABLE | Future reassessment | Open or deferred |
| CAP-PAIR-001..010 | Pair → candidate identity and isolated project scope | Future runtime spike readiness | In scope; no ranking |
| CAP-SPIKE-001..012 | Spike → operation and evidence obligation | Future readiness reassessment | Not authorized |
| CAP-CGATE-001..010 | Closure gate → enablement completeness | Future authorization review | Specified or blocked |
| RESEARCH-TECH-UI-007..009 | Shared UI authority inheritance | Future shared review | Inherited only |
| RESEARCH-TECH-RENDER-003 | Rendering dependency boundary | Future synthetic scene review | Referenced only |
| Architecture/adr/ADR-0002-ui-framework-selection.md | UI decision context | Future UI authority review | Draft; unresolved |
| Architecture/TECHNOLOGY-DECISION-ROADMAP.md | Technology decision context | Future candidate decision | No decision made |


## 24. Completion Conditions

本文件完成條件：

- 只新增 docs/Research/Technology/24-capture-backend-prerequisite-execution-enablement-specification.md。
- 建立正好七個 CAP-ENABLE。
- 維持七組 CAP-BA → CAP-CLOSE → CAP-ENABLE 一對一。
- 覆蓋十個 Candidate–Host Pair。
- 區分 Shared UI authority 與 Capture-specific authority。
- 區分 Project creation、Package acquisition、Restore、Build、Runtime 與 Evidence persistence。
- 建立 Synthetic Scene、Coordinate／Crop 與 Evidence Method enablement。
- 覆蓋十個 CAP-CGATE。
- 所有 Current authorization = Not granted。
- 所有 Execution permitted = No。
- 不建立 capture-specific authorization record。
- 不執行官方研究、本機盤點、下載、安裝、Restore、Build、Run、Capture API 或 Runtime Spike。
- 不建立實驗目錄、Project、Prototype、Result、Source Code、Capture Frame 或 Evidence。
- 不建立 Capture ADR。
- 不修改 UI／Rendering Research Line。
- 不修改任何 RESEARCH-TECH-CAPTURE-001..004。
- 不開始正式截圖功能。
- git diff --check 應通過。
