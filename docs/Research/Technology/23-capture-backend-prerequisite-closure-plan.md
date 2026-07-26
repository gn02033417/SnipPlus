# Capture Backend Prerequisite Closure Plan

| Field | Value |
|---|---|
| Document ID | `RESEARCH-TECH-CAPTURE-004` |
| Title | Capture Backend Prerequisite Closure Plan |
| Status | Draft |
| Research Type | Prerequisite Closure Plan |
| Parent Readiness Record | `RESEARCH-TECH-CAPTURE-003` |
| Parent Runtime Plan | `RESEARCH-TECH-CAPTURE-002` |
| Parent Feasibility | `RESEARCH-TECH-CAPTURE-001` |
| Technology Decision | 從 `Architecture/TECHNOLOGY-DECISION-ROADMAP.md` 原樣引用，不得猜測編號 |
| UI Framework Decision | Unresolved — `ADR-0002` remains Draft |
| Rendering Decision | Not made |
| Capture Decision | Not made |
| Closure Execution Status | Not started |
| Closure Execution Authorized | No |
| Build Verification | Not performed |
| Runtime Verification | Not performed |
| Capture Runtime Spike Authorized | No |
| Evidence Write Authorized | No |
| Owner | TBD |
| Last reviewed | Not reviewed |

## 1. Purpose

本文件只回答：

> 如何以最小、可追溯、分階段且需要明確授權的方式，關閉 `RESEARCH-TECH-CAPTURE-003` 中阻止 Phase C1 的七個 Capture Blocking Actions？

這是 Closure Plan，不是 Closure Execution Record、Execution Enablement Specification、Authorization Request、Runtime Spike、Capture Backend Decision 或 Capture ADR。

## 2. Scope

本文件只處理：

- `CAP-BA-001..007`
- `CAP-PREQ-001..030`
- `CAP-BLOCK-001..012`
- `CAP-PAIR-001..010`
- `CAP-SPIKE-001..012`
- `CAP-GATE-001..011`
- Phase C1 所需的最小 Host、Candidate、Project、Build、Synthetic Scene、Coordinate、Evidence 與 authorization 條件。

Phase C2／C3 項目只能標示為後續依賴或 Deferred，不得自動成為 Phase C1 blocker。

## 3. Non-goals

本文件不得：

- 執行任何 Closure Action。
- 執行本機盤點或 Package Cache 查詢。
- 進行新的官方網路研究。
- 下載或安裝 SDK、Runtime、Package 或工具。
- 建立 Project、Solution、Prototype、Source Code 或 Result directory。
- 執行 Restore、Build、Run、Publish 或 Capture Runtime Spike。
- 呼叫 Capture API。
- 擷取桌面、視窗、螢幕、Frame 或 Recording。
- 建立 Screenshot、PNG、Reference Frame、Pixel Difference 或 Measurement Artifact。
- 建立 Capture Authorization Request。
- 修改 `RESEARCH-TECH-CAPTURE-001..003`。
- 修改 UI／Rendering Research Line。
- 修改 `ADR-0002`。
- 建立 Capture ADR。
- 選擇 Capture Backend。
- 開始正式截圖功能。

## 4. Controlled Vocabulary

### 4.1 Closure Action Status

Closure Action 的 `Status` 只能使用：

- `Planned`
- `Blocked`
- `Deferred`
- `Not applicable`

Closure Action 的 `Status` 不得使用 `Completed`、`Resolved`、`Approved`、`Authorized` 或 `Executed`。

### 4.2 Target Status Recommendation

對上游 prerequisite、blocker 或 pair 的建議狀態只能使用：

- `Resolved`
- `Partially resolved`
- `Blocked`
- `Deferred`
- `Not applicable`

這是未來執行後的建議詞彙，不代表目前已完成。

### 4.3 Dependency Ownership

依賴責任只能使用：

- `Shared UI research`
- `Rendering research`
- `Capture-specific`
- `Graphics device`
- `Display environment`
- `Synthetic scene`
- `Evidence`
- `Authorization`

## 5. Closure Action Binding

建立正好七組一對一 Binding：

| Closure Action | Source Blocking Action | Current plan status | Rule |
|---|---|---|---|
| `CAP-CLOSE-001` | `CAP-BA-001` | `Blocked` | 不重新編號、不合併、不拆分 |
| `CAP-CLOSE-002` | `CAP-BA-002` | `Blocked` | 不重新編號、不合併、不拆分 |
| `CAP-CLOSE-003` | `CAP-BA-003` | `Blocked` | 不重新編號、不合併、不拆分 |
| `CAP-CLOSE-004` | `CAP-BA-004` | `Planned` | 不重新編號、不合併、不拆分 |
| `CAP-CLOSE-005` | `CAP-BA-005` | `Blocked` | 不重新編號、不合併、不拆分 |
| `CAP-CLOSE-006` | `CAP-BA-006` | `Blocked` | 不重新編號、不合併、不拆分 |
| `CAP-CLOSE-007` | `CAP-BA-007` | `Blocked` | 不重新編號、不合併、不拆分 |

Closure Action 可以包含多個 sub-step，但每個 sub-step 仍必須對應唯一的 `CAP-BA`。上游資料不足時建立 `CAP-CLOSURE-GAP-xxx`，不得修改上游文件；不得建立第八個 Phase C1 Blocking Action。

## 6. 每個 Closure Action 固定欄位

每個 `CAP-CLOSE` 必須包含以下欄位：

- Closure Action ID
- Source Blocking Action
- Blocking condition
- Related `CAP-PREQ`
- Related `CAP-BLOCK`
- Related `CAP-PAIR`
- Related `CAP-SPIKE`
- Related `CAP-GATE`
- Dependency ownership
- Shared UI source IDs
- Rendering source IDs，如適用
- Existing evidence
- Current limitation
- Required final evidence
- Proposed closure operation
- Operation classification
- Exact scope
- Explicit exclusions
- Official-source research required
- Local inspection required
- Network access required
- Package acquisition required
- Installation required
- Repository mutation required
- Experimental project required
- Restore required
- Build required
- Runtime execution required
- Capture API invocation required
- Evidence write required
- Display／system mutation required
- Administrator privilege required
- Human authorization required
- Expected files／directories
- Expected machine effect
- Privacy impact
- Success condition
- Failure condition
- Stop condition
- Rollback／cleanup
- Resulting prerequisite recommendation
- Resulting blocker recommendation
- Resulting pair recommendation
- Phase C1 impact
- Owner
- Status
- Open questions

## 7. Operation Classification

| Classification | 說明 |
|---|---|
| `Official-source research` | 查核官方第一方資料；本文件不執行 |
| `Local read-only inspection` | 唯讀查核本機環境；本文件不執行 |
| `Repository documentation mutation` | 只建立研究文件；本文件本身屬於此分類 |
| `Synthetic asset specification` | 只規格化未來測試資產 |
| `Experimental asset creation` | 未來建立 synthetic scene 或測試資產 |
| `Package acquisition` | 未來下載或 Restore Package |
| `Development environment installation` | 未來安裝 SDK、Runtime 或工具 |
| `Experimental project creation` | 未來建立隔離式 Capture Project |
| `Build execution` | 未來編譯 Capture Project |
| `Capture runtime execution` | 未來呼叫 Capture API |
| `Evidence capture／persistence` | 未來建立 Frame、Log、PNG 或量測資料 |
| `Display／system mutation` | 未來修改 Display、DPI、HDR 或系統狀態 |

本文件只描述分類，不執行任何分類所代表的操作。

## 8. Shared UI Dependency Reuse Matrix

Shared Host 依賴必須重用既有 UI research 與既有 authorization，不得建立重複授權。

| Closure action | Shared capability | Existing UI source | Current status | Reusable scope | New authorization needed | Duplication prohibited |
|---|---|---|---|---|---|---|
| `CAP-CLOSE-001` | Windows 11 x64 baseline | `RESEARCH-TECH-UI-007`, `RESEARCH-TECH-UI-008` | Blocked | baseline definition only | Yes, if evidence is missing | 新增第二份 OS baseline |
| `CAP-CLOSE-001` | WinUI 3 experimental build path | `RESEARCH-TECH-UI-007`, `UI-AUTH-001` | Blocked | host provenance and path boundary | Separate review | 重複 WinUI host authorization |
| `CAP-CLOSE-001` | WPF experimental build path | `RESEARCH-TECH-UI-008`, `UI-AUTH-002` | Blocked | WPF host provenance and path boundary | Separate review | 重複 WPF host authorization |
| `CAP-CLOSE-001` | .NET／Windows SDK | `RESEARCH-TECH-UI-007`, `UI-AUTH-003`, `UI-AUTH-004` | Blocked | version and evidence fields | Separate review | Capture-specific SDK authority |
| `CAP-CLOSE-001` | Windows App SDK | `RESEARCH-TECH-UI-007`, `UI-AUTH-001` | Blocked | package/runtime identity boundary | Separate review | 另一組 package permission |
| `CAP-CLOSE-003` | Experimental Project isolation | `RESEARCH-TECH-UI-008`, `UI-AUTH-005` | Blocked | isolated project rule | Separate review | 在 Capture 文件中假設 project exists |
| `CAP-CLOSE-003` | Package Restore | `RESEARCH-TECH-UI-009`, `UI-AUTH-006` | Blocked | restore scope only | Separate review | 把 Restore 當成 Runtime permission |
| `CAP-CLOSE-003` | Build execution | `RESEARCH-TECH-UI-009`, `UI-AUTH-007` | Blocked | explicit build scope only | Separate review | 把 build authority 當成 runtime authority |
| `CAP-CLOSE-005` | Display topology | `RESEARCH-TECH-UI-009`, `UI-AUTH-003` | Blocked | reuse definitions and evidence fields | Separate review | 建立第二份 display authorization |
| `CAP-CLOSE-005` | Per-monitor DPI | `RESEARCH-TECH-UI-009`, `UI-AUTH-004` | Blocked | reuse DPI contract | Separate review | 自行決定 rounding policy |
| `CAP-CLOSE-006` | Evidence storage policy | `RESEARCH-TECH-UI-009`, `UI-AUTH-006` | Blocked | governance and retention boundary | Separate review | 建立第二個 evidence root |
| `CAP-CLOSE-006` | Safety／cleanup | `RESEARCH-TECH-UI-009`, `UI-AUTH-007` | Blocked | cleanup principles and stop rules | Separate review | 以文件聲明取代 acceptance |
| `CAP-CLOSE-007` | Runtime execution authority | `RESEARCH-TECH-UI-009`, `UI-AUTH-008` | Blocked | authority separation only | Separate review | 把 UI authority 延伸成 Capture authority |
| `CAP-CLOSE-007` | Independent authorization review | `RESEARCH-TECH-UI-009`, `UI-AUTH-008` | Blocked | review boundary and decision ownership | Separate review | 由 Closure Plan 代替真人決策 |

規則：UI authority 未核准時，相關 Closure Action 維持 `Blocked`。Capture-specific extension 可以規格化，但不得在本文件中核准。

## 9. Rendering Dependency Boundary

Capture Research 不得選擇 Rendering Candidate。若未來需要 synthetic surface，只能使用 Host-native 最小 surface，並將其視為測試前置條件，而不是產品 Rendering Decision。

| Capture requirement | Rendering dependency | Rendering source | Required for C1 | Remaining boundary |
|---|---|---|---|---|
| Synthetic scene display | Host-native synthetic surface | `RESEARCH-TECH-RENDER-003` | Yes, specification only | 不選擇 rendering technology |
| Coordinate grid | Simple host drawing | `RESEARCH-TECH-RENDER-003` | Yes, specification only | grid 不等於產品 renderer |
| One-pixel border | Pixel-precise fixture contract | `RESEARCH-TECH-RENDER-003` | Yes, specification only | rounding remains TBD |
| Color blocks | Host-native color blocks | `RESEARCH-TECH-RENDER-003` | Yes, specification only | 不形成 color pipeline decision |
| Alpha gradient | Synthetic fixture | `RESEARCH-TECH-RENDER-003` | No, later fidelity scope | C2/C3 dependency |
| Overlay-like synthetic window | Separate synthetic host surface | `RESEARCH-TECH-RENDER-003` | No, later overlay scope | 不使用正式 SnipPlus Overlay |
| Wide-color substitute | Synthetic color reference | `RESEARCH-TECH-RENDER-003` | No, later HDR/SDR scope | 不形成 HDR decision |
| Result inspection surface | Future host inspection only | `RESEARCH-TECH-RENDER-003` | No | 不建立 Result directory |

不需要 Rendering Technology 的項目必須標示 `Not applicable`；Rendering Runtime authority 不得被 Capture Closure Plan 取代。

## 10. Capture-specific Dependency Matrix

官方 API identity、本機 availability、Build verification 與 Runtime verification 必須分開；未固定的 experimental identity 使用 `TBD`。

| Candidate | Required API／SDK | Graphics device | Interop | Packaging | Project requirement | Build requirement | Runtime requirement | Closure Action |
|---|---|---|---|---|---|---|---|---|
| Windows Graphics Capture | WGC API／SDK identity `TBD` | D3D11／graphics device path `TBD` | Host interop boundary | Packaged/unpackaged separately | Isolated pair project | Separate build authority | Separate runtime authority | `CAP-CLOSE-002`, `003` |
| DXGI Desktop Duplication | DXGI／D3D11 identity `TBD` | Adapter/output identity `TBD` | Native interop | Deployment path `TBD` | Isolated pair project | Separate build authority | Separate runtime authority | `CAP-CLOSE-002`, `003` |
| GDI | GDI／bitmap identity `TBD` | CPU/bitmap path | Native interop boundary | Host-dependent | Isolated pair project | Separate build authority | Separate runtime authority | `CAP-CLOSE-002`, `003` |
| Window-oriented mechanisms | Exact API identity `TBD` | Host/window compositor boundary | Window interop | Host-dependent | Pair-specific project if eligible | Separate build authority | Separate runtime authority | `CAP-CLOSE-002`, `003` |
| Hybrid strategy | Constituent WGC/DXGI/GDI or window APIs listed separately | Each constituent device path | Explicit hybrid ownership | Candidate-dependent | Isolated pair project | Separate build authority | Separate runtime authority | `CAP-CLOSE-002`, `003` |

目前五個 Candidate 的本機 availability 均不得由本文件推測；Build verified 與 Runtime verified 維持 `No`。

## 11. Candidate–Host Pair Closure Matrix

以下完整覆蓋 `CAP-PAIR-001..010`。每個 Pair 均保留，不因文件規劃而排除或排名。

| Pair | Current readiness | Blocking IDs | Required Closure Action | Required evidence | Target recommendation | Deferred phase |
|---|---|---|---|---|---|---|
| `CAP-PAIR-001` WGC × WinUI 3 | Blocked | `CAP-BLOCK-001`, `002`, `010`, `011` | `CAP-CLOSE-001..003` | API/SDK/host/project/build boundary | Partially resolved | C1 baseline |
| `CAP-PAIR-002` WGC × WPF | Blocked | `CAP-BLOCK-001`, `002`, `010`, `011` | `CAP-CLOSE-001..003` | API/SDK/host/project/build boundary | Partially resolved | C1 baseline |
| `CAP-PAIR-003` DXGI × WinUI 3 | Blocked | `CAP-BLOCK-001`, `002`, `010`, `011` | `CAP-CLOSE-001..003` | DXGI/device/host/build boundary | Partially resolved | C1 baseline |
| `CAP-PAIR-004` DXGI × WPF | Blocked | `CAP-BLOCK-001`, `002`, `010`, `011` | `CAP-CLOSE-001..003` | DXGI/device/host/build boundary | Partially resolved | C1 baseline |
| `CAP-PAIR-005` GDI × WinUI 3 | Blocked | `CAP-BLOCK-001`, `002`, `010`, `011` | `CAP-CLOSE-001..003` | bitmap/interop/host boundary | Partially resolved | C1 baseline |
| `CAP-PAIR-006` GDI × WPF | Blocked | `CAP-BLOCK-001`, `002`, `010`, `011` | `CAP-CLOSE-001..003` | bitmap/interop/host boundary | Partially resolved | C1 baseline |
| `CAP-PAIR-007` Window-oriented × WinUI 3 | Blocked | `CAP-BLOCK-001`, `002`, `010`, `011` | `CAP-CLOSE-001..003` | limitation/eligibility boundary | Blocked | C2 or excluded only with evidence |
| `CAP-PAIR-008` Window-oriented × WPF | Blocked | `CAP-BLOCK-001`, `002`, `010`, `011` | `CAP-CLOSE-001..003` | limitation/eligibility boundary | Blocked | C2 or excluded only with evidence |
| `CAP-PAIR-009` Hybrid × WinUI 3 | Blocked | `CAP-BLOCK-001`, `002`, `010`, `011` | `CAP-CLOSE-001..003` | constituent API ownership | Blocked | C2/C3 |
| `CAP-PAIR-010` Hybrid × WPF | Blocked | `CAP-BLOCK-001`, `002`, `010`, `011` | `CAP-CLOSE-001..003` | constituent API ownership | Blocked | C2/C3 |

`Unknown` 不得直接建議 `Excluded with evidence`；排除必須有可引用的官方或未來 Runtime Evidence。不得形成 Candidate 排名或選擇。

## 12. Phase C1 Minimum Closure Gates

本計畫建立正好十個 Phase C1 Minimum Closure Gates；Gate Plan Status 只能使用 `Specified`、`Partially specified`、`Blocked` 或 `Deferred`。不得使用 `Satisfied`、`Passed` 或 `Resolved`。

| Gate | Minimum closure condition | Source Closure Action | Gate Plan Status |
|---|---|---|---|
| `CAP-CGATE-001` | Shared WinUI 3／WPF Host build dependencies 已有明確引用或授權路徑 | `CAP-CLOSE-001`, `003` | Blocked |
| `CAP-CGATE-002` | 至少一個 one-shot Candidate 的精確 API／SDK identity 已固定 | `CAP-CLOSE-002` | Blocked |
| `CAP-CGATE-003` | Candidate–Host Project／Interop boundary 已規格化 | `CAP-CLOSE-002`, `003` | Blocked |
| `CAP-CGATE-004` | Basic synthetic scene 已完整規格化 | `CAP-CLOSE-004` | Specified |
| `CAP-CGATE-005` | Virtual desktop、monitor、negative-coordinate coordinate model 已規格化 | `CAP-CLOSE-005` | Blocked |
| `CAP-CGATE-006` | Region crop 與 off-by-one evidence method 已規格化 | `CAP-CLOSE-005` | Blocked |
| `CAP-CGATE-007` | Frame、metadata、coordinate 及 privacy evidence obligation 已規格化 | `CAP-CLOSE-005`, `006` | Blocked |
| `CAP-CGATE-008` | Project、Package／Restore、Build、Runtime 與 Evidence write authority 已分離 | `CAP-CLOSE-003`, `006`, `007` | Blocked |
| `CAP-CGATE-009` | Result storage 與 cleanup boundary 已規格化 | `CAP-CLOSE-006` | Blocked |
| `CAP-CGATE-010` | Runtime execution 仍保留為後續獨立授權 | `CAP-CLOSE-007` | Specified |

### 12.1 Upstream Gate Coverage

The upstream `CAP-GATE-001..011` set remains a source dependency. It is not replaced by the ten C1 closure gates above.

| Upstream gate | Referenced Closure Action | Current recommendation |
|---|---|---|
| `CAP-GATE-001` | `CAP-CLOSE-001` | Blocked |
| `CAP-GATE-002` | `CAP-CLOSE-001`, `CAP-CLOSE-002` | Blocked |
| `CAP-GATE-003` | `CAP-CLOSE-002` | Blocked |
| `CAP-GATE-004` | `CAP-CLOSE-003` | Blocked |
| `CAP-GATE-005` | `CAP-CLOSE-004` | Partially resolved |
| `CAP-GATE-006` | `CAP-CLOSE-005` | Blocked |
| `CAP-GATE-007` | `CAP-CLOSE-005`, `CAP-CLOSE-006` | Blocked |
| `CAP-GATE-008` | `CAP-CLOSE-006` | Blocked |
| `CAP-GATE-009` | `CAP-CLOSE-006` | Blocked |
| `CAP-GATE-010` | `CAP-CLOSE-007` | Deferred |
| `CAP-GATE-011` | `CAP-CLOSE-007` | Blocked |

## 13. Synthetic Scene Closure Plan

本節只規劃未來 fixture，不建立實際 Scene、Window 或 Asset。

| Scene capability | Existing specification | Remaining gap | Future asset required | Runtime required | Closure Action |
|---|---|---|---|---|---|
| Fixed physical canvas | `CAP-PREQ-013` listed | exact dimensions | Yes, future fixture | Yes | `CAP-CLOSE-004` |
| Fixed logical canvas | `CAP-PREQ-013` listed | DIP-to-pixel relation | Yes, future fixture | Yes | `CAP-CLOSE-004`, `005` |
| High-contrast color blocks | `CAP-PREQ-013` listed | color manifest | Yes, future fixture | Yes | `CAP-CLOSE-004` |
| One-pixel border | `CAP-PREQ-014` listed | edge observation | Yes, future fixture | Yes | `CAP-CLOSE-004`, `005` |
| Coordinate grid | `CAP-PREQ-014` listed | origin and labels | Yes, future fixture | Yes | `CAP-CLOSE-004`, `005` |
| Corner markers | `CAP-PREQ-013` listed | marker geometry | Yes, future fixture | Yes | `CAP-CLOSE-004` |
| Center marker | `CAP-PREQ-013` listed | marker geometry | Yes, future fixture | Yes | `CAP-CLOSE-004` |
| Mixed-language text | `CAP-PREQ-013` listed | font/environment record | Yes, future fixture | Yes | `CAP-CLOSE-004` |
| Alpha gradient | `CAP-PREQ-013` listed | format and profile | Yes, future fixture | C2 | `CAP-CLOSE-004` |
| SDR block | `CAP-PREQ-013` listed | baseline color metadata | Yes, future fixture | C1/C2 | `CAP-CLOSE-004` |
| Wide-color substitute | `CAP-PREQ-013` listed | lawful conversion path | Yes, future fixture | C2 | `CAP-CLOSE-004` |
| Cursor target | `CAP-PREQ-017` listed | inclusion/exclusion contract | Yes, future fixture | C2 | `CAP-CLOSE-004`, `005` |
| Overlay-like window | `CAP-PREQ-018` listed | synthetic ownership boundary | Yes, future fixture | C2 | `CAP-CLOSE-004`, `006` |
| Occluded-window scenario | `CAP-PREQ-019` listed | expected semantics | Yes, future fixture | C2 | `CAP-CLOSE-004` |
| Minimized-window scenario | `CAP-PREQ-019` listed | expected semantics | Yes, future fixture | C2 | `CAP-CLOSE-004` |
| Negative-coordinate placement | `CAP-PREQ-015` listed | signed coordinate contract | Yes, future fixture | C1 | `CAP-CLOSE-004`, `005` |
| Same-DPI multi-monitor | `CAP-PREQ-015` listed | topology manifest | Yes, future fixture | C1 | `CAP-CLOSE-004`, `005` |
| Mixed-DPI multi-monitor | `CAP-PREQ-016` listed | per-monitor mapping | Yes, future fixture | C2 | `CAP-CLOSE-004`, `005` |
| Protected-content substitute | `CAP-PREQ-020` listed | lawful substitute behavior | Yes, future fixture | C2 | `CAP-CLOSE-004`, `006` |
| Display-change／device-loss trigger | `CAP-PREQ-021` listed | trigger and cleanup contract | No static asset alone | C3 | `CAP-CLOSE-006`, `007` |

## 14. Coordinate and Evidence Closure Plan

本節不得自行建立產品級 pixel-difference 門檻；`Rounding policy` 未決時保持 `TBD`。

| Capability | Existing definition | Remaining gap | Required method／tool | Evidence write required | Closure Action |
|---|---|---|---|---|---|
| Virtual-screen origin | `CAP-PREQ-014`, `015` | signed origin evidence | future read-only environment record | Yes, future authorization | `CAP-CLOSE-005` |
| Monitor physical bounds | `CAP-PREQ-014`, `015` | exact bounds record | future display metadata | Yes, future authorization | `CAP-CLOSE-005` |
| DIP bounds | `CAP-PREQ-016` | per-monitor conversion | future host metadata | Yes, future authorization | `CAP-CLOSE-005` |
| Selection intent | `CAP-PREQ-014` | owner and timestamp | future selection manifest | Yes, future authorization | `CAP-CLOSE-005` |
| Source frame bounds | `CAP-PREQ-013`, `022` | candidate-specific source identity | future frame metadata | Yes, future authorization | `CAP-CLOSE-002`, `005` |
| Frame-local bounds | `CAP-PREQ-022` | origin and dimensions | future frame metadata | Yes, future authorization | `CAP-CLOSE-005` |
| Crop conversion | `CAP-PREQ-023` | source-to-crop mapping | future deterministic crop record | Yes, future authorization | `CAP-CLOSE-005` |
| Negative coordinates | `CAP-PREQ-015` | signed mapping observation | future topology fixture | Yes, future authorization | `CAP-CLOSE-005` |
| Inclusive／exclusive edge semantics | `CAP-PREQ-023` | contract not frozen | documentation first, runtime later | Yes, future authorization | `CAP-CLOSE-005` |
| Rounding policy | `CAP-PREQ-016`, `023` | decision not made | keep `TBD` | Yes, future authorization | `CAP-CLOSE-005` |
| Timestamp correlation | `CAP-PREQ-022`, `024` | selection/frame relation | future timestamp manifest | Yes, future authorization | `CAP-CLOSE-005` |
| Off-by-one detection | `CAP-PREQ-023` | expected/observed comparison | future synthetic fixture | Yes, future authorization | `CAP-CLOSE-005` |
| Pixel-difference method | `CAP-PREQ-023` | threshold not decided | future analysis procedure | Yes, future authorization | `CAP-CLOSE-005` |
| Frame metadata | `CAP-PREQ-022`, `024` | candidate schema | future metadata record | Yes, future authorization | `CAP-CLOSE-005`, `006` |
| Privacy review | `CAP-PREQ-025`, `026` | reviewer and retention boundary | future review record | Yes, future authorization | `CAP-CLOSE-006` |
| Cleanup confirmation | `CAP-PREQ-026`, `030` | stop/rollback evidence | future cleanup record | Yes, future authorization | `CAP-CLOSE-006`, `007` |

Session-only observation 不等於持久 Evidence；沒有 Evidence Write authorization 時不得建立 Artifact。

## 15. Deferred Scope Register

每項 Deferred scope 都必須保留 reactivation condition，不得因 Deferred 而永久移出 Capture Research。

| Deferred scope | Target phase | Deferred reason | Reactivation condition | Affected candidates | Affected pairs | Affected spikes | Blocks Phase C1 |
|---|---|---|---|---|---|---|---|
| Mixed-DPI full coverage | C2 | C1 先固定 basic coordinate contract | C1 mapping evidence available | All | `001..010` | `002..005` | No |
| Overlay self-capture | C2 | 需要 synthetic overlay boundary | synthetic ownership and privacy review | All | `001..010` | `006` | No |
| Cursor inclusion／exclusion | C2 | 需要 explicit cursor contract | cursor state evidence authorized | All | `001..010` | `007` | No |
| HDR／SDR branch | C2 | rendering/color authority unresolved | SDR baseline and later color authority | All | `001..010` | `008` | No |
| Protected／secure boundary | C2 | lawful substitute only | substitute and refusal evidence defined | All | `001..010` | `009` | No |
| Packaged／unpackaged full matrix | C2 | C1 only needs one isolated path | packaging scope separately authorized | Candidate-dependent | `001..010` | `001..005` | No |
| Device-loss／display-change recovery | C3 | recovery trigger and cleanup not ready | controlled invalidation authority | All | `001..010` | `010` | No |
| WinUI 3／WPF full interoperability | C3 | host boundary remains separate | both host paths independently specified | All | `001..010` | `011` | No |
| Cold／warm timing | C3 | timing/resources are later evidence | timing schema and runtime authority | All | `001..010` | `012` | No |
| CPU／GPU／memory observation | C3 | resource evidence is not C1 minimum | resource observation method authorized | All | `001..010` | `012` | No |
| Phase C2／C3 | C2/C3 | later evidence line | prior phase review complete | All | `001..010` | `006..012` | No |

## 16. Full Impact Matrix

本表完整覆蓋 30 個 prerequisite、12 個 blocker、10 個 pair、12 個 spike、7 個 blocking action 與 7 個 closure action；本文件只提出 recommendation，不修改上游狀態。

| Source item | Phase | Closure Action | Required evidence | Current status | Target recommendation |
|---|---|---|---|---|---|
| `CAP-PREQ-001..004`; `CAP-BLOCK-001`; `CAP-PAIR-001..010`; `CAP-SPIKE-001..005,011`; `CAP-BA-001` | C1 | `CAP-CLOSE-001` | host/framework/SDK identity and shared authority | Blocked | Partially resolved |
| `CAP-PREQ-003..011`; `CAP-BLOCK-002`; `CAP-PAIR-001..010`; `CAP-SPIKE-001..005,011`; `CAP-BA-002` | C1 | `CAP-CLOSE-002` | one-shot API/SDK/interop identity | Blocked | Partially resolved |
| `CAP-PREQ-027`; `CAP-BLOCK-010`; `CAP-PAIR-001..010`; `CAP-SPIKE-001..005,011`; `CAP-BA-003` | C1 | `CAP-CLOSE-003` | exact isolated project/restore/build scope | Blocked | Blocked |
| `CAP-PREQ-013`; `CAP-BLOCK-003`; `CAP-PAIR-001..010`; `CAP-SPIKE-001..005`; `CAP-BA-004` | C1 | `CAP-CLOSE-004` | synthetic scene contract | Planned | Partially resolved |
| `CAP-PREQ-014..016,022,023`; `CAP-BLOCK-004,007`; `CAP-PAIR-001..010`; `CAP-SPIKE-002..005`; `CAP-BA-005` | C1 | `CAP-CLOSE-005` | coordinate, crop, edge and metadata method | Blocked | Partially resolved |
| `CAP-PREQ-025,026,030`; `CAP-BLOCK-008,009`; `CAP-PAIR-001..010`; `CAP-SPIKE-001..005`; `CAP-BA-006` | C1 | `CAP-CLOSE-006` | privacy, retention, evidence and cleanup boundary | Blocked | Partially resolved |
| `CAP-PREQ-028`; `CAP-BLOCK-011`; `CAP-PAIR-001..010`; `CAP-SPIKE-001..005,011`; `CAP-BA-007` | C1 | `CAP-CLOSE-007` | future runtime authorization input and stop rules | Blocked | Blocked |
| `CAP-PREQ-005..012,017..021,024,029`; `CAP-BLOCK-005,006,012`; `CAP-SPIKE-006..010,012`; deferred C2/C3 dependencies | C2/C3 | Referenced by `CAP-CLOSE-004..007` | later overlay, cursor, HDR, protected boundary, recovery and resources | Deferred | Deferred |

### 16.1 Explicit Coverage Index

The range references above are expanded here so every upstream item has an explicit place in this Closure Plan. An explicit ID is not evidence that the item is closed.

#### Capture prerequisite coverage

| ID | Closure Action | Current recommendation |
|---|---|---|
| `CAP-PREQ-001` | `CAP-CLOSE-001` | Partially resolved |
| `CAP-PREQ-002` | `CAP-CLOSE-001` | Partially resolved |
| `CAP-PREQ-003` | `CAP-CLOSE-002` | Partially resolved |
| `CAP-PREQ-004` | `CAP-CLOSE-002` | Partially resolved |
| `CAP-PREQ-005` | `CAP-CLOSE-002` | Partially resolved |
| `CAP-PREQ-006` | `CAP-CLOSE-002` | Partially resolved |
| `CAP-PREQ-007` | `CAP-CLOSE-002` | Partially resolved |
| `CAP-PREQ-008` | `CAP-CLOSE-002` | Partially resolved |
| `CAP-PREQ-009` | `CAP-CLOSE-002` | Partially resolved |
| `CAP-PREQ-010` | `CAP-CLOSE-002` | Partially resolved |
| `CAP-PREQ-011` | `CAP-CLOSE-002` | Partially resolved |
| `CAP-PREQ-012` | `CAP-CLOSE-002` | Deferred |
| `CAP-PREQ-013` | `CAP-CLOSE-004` | Partially resolved |
| `CAP-PREQ-014` | `CAP-CLOSE-005` | Partially resolved |
| `CAP-PREQ-015` | `CAP-CLOSE-005` | Partially resolved |
| `CAP-PREQ-016` | `CAP-CLOSE-005` | Partially resolved |
| `CAP-PREQ-017` | `CAP-CLOSE-004` | Deferred |
| `CAP-PREQ-018` | `CAP-CLOSE-004` | Deferred |
| `CAP-PREQ-019` | `CAP-CLOSE-004` | Deferred |
| `CAP-PREQ-020` | `CAP-CLOSE-004` | Deferred |
| `CAP-PREQ-021` | `CAP-CLOSE-004` | Deferred |
| `CAP-PREQ-022` | `CAP-CLOSE-005` | Partially resolved |
| `CAP-PREQ-023` | `CAP-CLOSE-005` | Partially resolved |
| `CAP-PREQ-024` | `CAP-CLOSE-006` | Deferred |
| `CAP-PREQ-025` | `CAP-CLOSE-006` | Partially resolved |
| `CAP-PREQ-026` | `CAP-CLOSE-006` | Partially resolved |
| `CAP-PREQ-027` | `CAP-CLOSE-003` | Blocked |
| `CAP-PREQ-028` | `CAP-CLOSE-007` | Blocked |
| `CAP-PREQ-029` | `CAP-CLOSE-006` | Deferred |
| `CAP-PREQ-030` | `CAP-CLOSE-006` | Partially resolved |

#### Capture blocker coverage

| ID | Related Closure Action | Current recommendation |
|---|---|---|
| `CAP-BLOCK-001` | `CAP-CLOSE-001` | Blocked |
| `CAP-BLOCK-002` | `CAP-CLOSE-002` | Blocked |
| `CAP-BLOCK-003` | `CAP-CLOSE-004` | Blocked |
| `CAP-BLOCK-004` | `CAP-CLOSE-005` | Blocked |
| `CAP-BLOCK-005` | `CAP-CLOSE-004`, `CAP-CLOSE-006` | Deferred |
| `CAP-BLOCK-006` | `CAP-CLOSE-004`, `CAP-CLOSE-007` | Deferred |
| `CAP-BLOCK-007` | `CAP-CLOSE-005` | Blocked |
| `CAP-BLOCK-008` | `CAP-CLOSE-006` | Blocked |
| `CAP-BLOCK-009` | `CAP-CLOSE-006` | Blocked |
| `CAP-BLOCK-010` | `CAP-CLOSE-003` | Blocked |
| `CAP-BLOCK-011` | `CAP-CLOSE-007` | Blocked |
| `CAP-BLOCK-012` | `CAP-CLOSE-006`, `CAP-CLOSE-007` | Deferred |

#### Capture spike coverage

| ID | Closure Action dependency | Target phase |
|---|---|---|
| `CAP-SPIKE-001` | `CAP-CLOSE-001..007` | C1 |
| `CAP-SPIKE-002` | `CAP-CLOSE-001..005` | C1 |
| `CAP-SPIKE-003` | `CAP-CLOSE-001..005` | C1 |
| `CAP-SPIKE-004` | `CAP-CLOSE-001..005` | C1/C2 |
| `CAP-SPIKE-005` | `CAP-CLOSE-001..006` | C1 |
| `CAP-SPIKE-006` | `CAP-CLOSE-004`, `CAP-CLOSE-006` | C2 |
| `CAP-SPIKE-007` | `CAP-CLOSE-004`, `CAP-CLOSE-005` | C2 |
| `CAP-SPIKE-008` | `CAP-CLOSE-004`, `CAP-CLOSE-006` | C2 |
| `CAP-SPIKE-009` | `CAP-CLOSE-004`, `CAP-CLOSE-006` | C2 |
| `CAP-SPIKE-010` | `CAP-CLOSE-006`, `CAP-CLOSE-007` | C3 |
| `CAP-SPIKE-011` | `CAP-CLOSE-001..003`, `CAP-CLOSE-007` | C1/C3 |
| `CAP-SPIKE-012` | `CAP-CLOSE-006`, `CAP-CLOSE-007` | C3 |

## 17. Detailed Closure Action Plans

### 17.1 `CAP-CLOSE-001` — Host Framework／SDK exact baseline

| Field | Value |
|---|---|
| Closure Action ID | `CAP-CLOSE-001` |
| Source Blocking Action | `CAP-BA-001` |
| Blocking condition | Host Framework／SDK exact baseline 未固定 |
| Related `CAP-PREQ` | `CAP-PREQ-001`, `CAP-PREQ-002` |
| Related `CAP-BLOCK` | `CAP-BLOCK-001` |
| Related `CAP-PAIR` | `CAP-PAIR-001..010` |
| Related `CAP-SPIKE` | `CAP-SPIKE-001..005`, `CAP-SPIKE-011` |
| Related `CAP-GATE` | `CAP-CGATE-001` |
| Dependency ownership | `Shared UI research`; `Authorization` |
| Shared UI source IDs | `RESEARCH-TECH-UI-007`, `RESEARCH-TECH-UI-008`, `RESEARCH-TECH-UI-009`, `UI-AUTH-001..004` |
| Rendering source IDs | `RESEARCH-TECH-RENDER-003` only for dependency boundary |
| Existing evidence | 上游文件只保留 research boundary；沒有本機 availability 或 build verification |
| Current limitation | 具名 host、SDK、Windows App SDK、.NET 與 x64 baseline 尚未形成可執行 identity |
| Required final evidence | 具名版本、host path、source provenance、authority owner 與 scope record |
| Proposed closure operation | 先重用 UI authority；若缺欄位，另行提出最小的唯讀或文件 closure review |
| Operation classification | `Repository documentation mutation`; future `Local read-only inspection` |
| Exact scope | `CAP-PREQ-001..002` 與 shared host prerequisites |
| Explicit exclusions | 不安裝、不下載、不 Restore、不 Build、不 Runtime、不修改 UI 文件 |
| Official-source research required | No in this plan |
| Local inspection required | Future, separately authorized |
| Network access required | No in this plan |
| Package acquisition required | No in this plan |
| Installation required | No |
| Repository mutation required | No beyond this document |
| Experimental project required | No for closure plan |
| Restore required | No |
| Build required | No |
| Runtime execution required | No |
| Capture API invocation required | No |
| Evidence write required | Future, separately authorized |
| Display／system mutation required | No |
| Administrator privilege required | No in this plan |
| Human authorization required | Yes, before any inspection or acquisition |
| Expected files／directories | None in this plan |
| Expected machine effect | None |
| Privacy impact | No runtime data; future evidence must be synthetic and governed |
| Success condition | 具名 baseline 與 host scope 可被下一階段引用，且 authority boundary 清楚 |
| Failure condition | 版本、來源或 owner 仍為 TBD，或 shared UI authority 被重複建立 |
| Stop condition | 任何需要安裝、下載、Restore、Build、Run 或 Runtime 的請求未獲獨立授權 |
| Rollback／cleanup | 本文件無 machine mutation；刪除 future draft record only by owner decision |
| Resulting prerequisite recommendation | `CAP-PREQ-001..002` → `Partially resolved` only after evidence |
| Resulting blocker recommendation | `CAP-BLOCK-001` remains `Open` until final evidence |
| Resulting pair recommendation | Pair readiness remains `Blocked` until host identity is complete |
| Phase C1 impact | Blocking |
| Owner | TBD |
| Status | `Blocked` |
| Open questions | Exact host framework/version、SDK identity、owner、scope 與 future evidence destination |

### 17.2 `CAP-CLOSE-002` — Candidate one-shot API identity

| Field | Value |
|---|---|
| Closure Action ID | `CAP-CLOSE-002` |
| Source Blocking Action | `CAP-BA-002` |
| Blocking condition | Candidate one-shot API identity 未形成 Candidate–Host pair |
| Related `CAP-PREQ` | `CAP-PREQ-003..011` |
| Related `CAP-BLOCK` | `CAP-BLOCK-002` |
| Related `CAP-PAIR` | `CAP-PAIR-001..010` |
| Related `CAP-SPIKE` | `CAP-SPIKE-001..005`, `CAP-SPIKE-011` |
| Related `CAP-GATE` | `CAP-CGATE-002`, `CAP-CGATE-003` |
| Dependency ownership | `Capture-specific`; `Graphics device`; `Authorization` |
| Shared UI source IDs | `RESEARCH-TECH-UI-007`, `RESEARCH-TECH-UI-008` |
| Rendering source IDs | `RESEARCH-TECH-RENDER-003` for scene/host boundary only |
| Existing evidence | 五個候選已列出；官方 API identity 與 runtime-critical behavior 分開 |
| Current limitation | 沒有一組 one-shot Candidate–Host API/SDK/interop identity 通過 closure boundary |
| Required final evidence | API/SDK identity、constituent APIs、graphics device、host interop、limitations 與 eligibility record |
| Proposed closure operation | 建立 candidate-specific identity matrix；不以 API 存在單獨推導 Eligible |
| Operation classification | `Repository documentation mutation`; future `Official-source research` |
| Exact scope | `CAP-PREQ-003..011` 與 `CAP-PAIR-001..010` |
| Explicit exclusions | 不選擇 backend、不排名、不建立 project、不呼叫 Capture API |
| Official-source research required | Future, separately authorized |
| Local inspection required | Future only if availability must be established |
| Network access required | No in this plan |
| Package acquisition required | No |
| Installation required | No |
| Repository mutation required | No beyond future bounded research record |
| Experimental project required | Future, separately authorized |
| Restore required | Future only, separately authorized |
| Build required | Future only, separately authorized |
| Runtime execution required | No for closure plan |
| Capture API invocation required | No |
| Evidence write required | Future, separately authorized |
| Display／system mutation required | No |
| Administrator privilege required | No in this plan |
| Human authorization required | Yes, before official research expansion or execution |
| Expected files／directories | None in this plan |
| Expected machine effect | None |
| Privacy impact | None now; future runtime must use synthetic content |
| Success condition | 至少一組 one-shot pair 可清楚描述而不形成產品選擇 |
| Failure condition | identity、interop 或 eligibility 仍為 Unknown/TBD |
| Stop condition | 任何候選選定、下載、project、build 或 runtime 行為 |
| Rollback／cleanup | 無 machine mutation；撤回未核准 draft recommendation |
| Resulting prerequisite recommendation | `CAP-PREQ-003..011` → `Partially resolved` only with evidence |
| Resulting blocker recommendation | `CAP-BLOCK-002` remains `Open` until identity evidence |
| Resulting pair recommendation | 不得從 Unknown 改為 Excluded with evidence |
| Phase C1 impact | Blocking |
| Owner | TBD |
| Status | `Blocked` |
| Open questions | 第一組 one-shot identity、host parity、graphics device、interop 與 packaging boundary |

### 17.3 `CAP-CLOSE-003` — Project／Restore／Build scope

| Field | Value |
|---|---|
| Closure Action ID | `CAP-CLOSE-003` |
| Source Blocking Action | `CAP-BA-003` |
| Blocking condition | Project／Restore／Build scope 未取得 |
| Related `CAP-PREQ` | `CAP-PREQ-027` |
| Related `CAP-BLOCK` | `CAP-BLOCK-010` |
| Related `CAP-PAIR` | `CAP-PAIR-001..010` |
| Related `CAP-SPIKE` | `CAP-SPIKE-001..005`, `CAP-SPIKE-011` |
| Related `CAP-GATE` | `CAP-CGATE-001`, `CAP-CGATE-003`, `CAP-CGATE-008` |
| Dependency ownership | `Shared UI research`; `Authorization` |
| Shared UI source IDs | `RESEARCH-TECH-UI-008`, `RESEARCH-TECH-UI-009`, `UI-AUTH-005..007` |
| Rendering source IDs | `RESEARCH-TECH-RENDER-003` only for isolation boundary |
| Existing evidence | 既有文件要求 isolation，但沒有建立 project 或取得 execution authority |
| Current limitation | project、package、restore、build、runtime 與 evidence scope 尚未分離成具名決策 |
| Required final evidence | exact project root、source boundary、package scope、restore scope、build command owner、output boundary |
| Proposed closure operation | 撰寫 future project/build scope record，並為每個 mutation 分開授權 |
| Operation classification | `Repository documentation mutation`; future `Experimental project creation`; future `Build execution` |
| Exact scope | `CAP-PREQ-027`, `CAP-PAIR-001..010` 的 project/build fields |
| Explicit exclusions | 不建立 project、不 Restore、不 Build、不 Publish、不 Run |
| Official-source research required | No in this plan |
| Local inspection required | Future, separately authorized |
| Network access required | No |
| Package acquisition required | Future only, separately authorized |
| Installation required | No in this plan |
| Repository mutation required | Future isolated path only |
| Experimental project required | Future, separately authorized |
| Restore required | Future, separately authorized |
| Build required | Future, separately authorized |
| Runtime execution required | No |
| Capture API invocation required | No |
| Evidence write required | Future, separately authorized |
| Display／system mutation required | No |
| Administrator privilege required | No in this plan |
| Human authorization required | Yes, per project/package/restore/build scope |
| Expected files／directories | None in this plan |
| Expected machine effect | None |
| Privacy impact | Source and package boundary must exclude user data |
| Success condition | exact project/build boundary named and separately authorized |
| Failure condition | shared repository or product source tree is mutated without scope |
| Stop condition | any command execution outside explicit future authorization |
| Rollback／cleanup | future isolated project cleanup record; no cleanup now |
| Resulting prerequisite recommendation | `CAP-PREQ-027` → `Partially resolved` only after scope evidence |
| Resulting blocker recommendation | `CAP-BLOCK-010` remains `Open` until authority exists |
| Resulting pair recommendation | all pair readiness remains `Blocked` |
| Phase C1 impact | Blocking |
| Owner | TBD |
| Status | `Blocked` |
| Open questions | project location、package source、restore authority、build owner、output cleanup |

### 17.4 `CAP-CLOSE-004` — Synthetic basic scene contract

| Field | Value |
|---|---|
| Closure Action ID | `CAP-CLOSE-004` |
| Source Blocking Action | `CAP-BA-004` |
| Blocking condition | Synthetic basic scene 未具備固定規格 |
| Related `CAP-PREQ` | `CAP-PREQ-013` |
| Related `CAP-BLOCK` | `CAP-BLOCK-003` |
| Related `CAP-PAIR` | `CAP-PAIR-001..010` |
| Related `CAP-SPIKE` | `CAP-SPIKE-001..005` |
| Related `CAP-GATE` | `CAP-CGATE-004` |
| Dependency ownership | `Synthetic scene`; `Rendering research` |
| Shared UI source IDs | `RESEARCH-TECH-UI-005`, `RESEARCH-TECH-UI-009` where applicable; no duplicate authority |
| Rendering source IDs | `RESEARCH-TECH-RENDER-003` |
| Existing evidence | Readiness record lists required scene capabilities but no actual assets |
| Current limitation | fixture dimensions、labels、markers、privacy and ownership are not frozen |
| Required final evidence | scene contract, geometry manifest, color/text manifest, privacy rules and fixture identity |
| Proposed closure operation | 只建立 future synthetic scene specification；後續 asset creation must be separate |
| Operation classification | `Synthetic asset specification` |
| Exact scope | `CAP-PREQ-013` 與 C1 basic scene only |
| Explicit exclusions | 不建立 Scene、Window、Image、Asset、Screenshot、Frame 或 Result |
| Official-source research required | No in this plan |
| Local inspection required | No |
| Network access required | No |
| Package acquisition required | No |
| Installation required | No |
| Repository mutation required | Future specification only |
| Experimental project required | No for plan |
| Restore required | No |
| Build required | No |
| Runtime execution required | No |
| Capture API invocation required | No |
| Evidence write required | Future only |
| Display／system mutation required | No |
| Administrator privilege required | No |
| Human authorization required | Yes before asset creation or runtime |
| Expected files／directories | None in this plan |
| Expected machine effect | None |
| Privacy impact | Contract must prohibit real desktop and private data |
| Success condition | fixture identity/geometry/privacy fixed without product rendering decision |
| Failure condition | fixture depends on user desktop or undefined pixels |
| Stop condition | any asset, window or frame creation |
| Rollback／cleanup | no asset exists to clean up in this plan |
| Resulting prerequisite recommendation | `CAP-PREQ-013` → `Partially resolved` after contract review |
| Resulting blocker recommendation | `CAP-BLOCK-003` remains `Open` until contract evidence |
| Resulting pair recommendation | pair readiness remains at least `Conditionally ready` only after other blockers close |
| Phase C1 impact | Blocking |
| Owner | TBD |
| Status | `Planned` |
| Open questions | exact canvas size、font availability、color profile、asset owner、fixture retention |

### 17.5 `CAP-CLOSE-005` — Coordinate／mapping／crop evidence method

| Field | Value |
|---|---|
| Closure Action ID | `CAP-CLOSE-005` |
| Source Blocking Action | `CAP-BA-005` |
| Blocking condition | Coordinate、mapping、crop evidence method 未完成 |
| Related `CAP-PREQ` | `CAP-PREQ-014`, `015`, `016`, `022`, `023` |
| Related `CAP-BLOCK` | `CAP-BLOCK-004`, `CAP-BLOCK-007` |
| Related `CAP-PAIR` | `CAP-PAIR-001..010` |
| Related `CAP-SPIKE` | `CAP-SPIKE-002..005` |
| Related `CAP-GATE` | `CAP-CGATE-005`, `CAP-CGATE-006`, `CAP-CGATE-007` |
| Dependency ownership | `Display environment`; `Evidence`; `Capture-specific` |
| Shared UI source IDs | `RESEARCH-TECH-UI-009`, `UI-AUTH-003`, `UI-AUTH-004` |
| Rendering source IDs | `RESEARCH-TECH-RENDER-003` for pixel/coordinate boundary |
| Existing evidence | Coordinate domains and negative/mixed-DPI risks are listed; rounding remains TBD |
| Current limitation | origin、DIP、physical bounds、edge semantics、crop and timestamp contract are not final |
| Required final evidence | mapping manifest, expected crop dimensions, off-by-one method, timestamp relation and owner |
| Proposed closure operation | document the contract first; future runtime evidence is a separate authorization |
| Operation classification | `Repository documentation mutation`; future `Evidence capture／persistence` |
| Exact scope | `CAP-PREQ-014..016`, `022`, `023` |
| Explicit exclusions | 不決定 rounding、不建立 pixel threshold、不擷取 frame、不修改 display/DPI |
| Official-source research required | No in this plan |
| Local inspection required | Future, separately authorized |
| Network access required | No |
| Package acquisition required | No |
| Installation required | No |
| Repository mutation required | Documentation only in this plan |
| Experimental project required | Future only |
| Restore required | No in this plan |
| Build required | No in this plan |
| Runtime execution required | Future only |
| Capture API invocation required | No |
| Evidence write required | Future, separately authorized |
| Display／system mutation required | No |
| Administrator privilege required | No |
| Human authorization required | Yes before local or runtime evidence |
| Expected files／directories | None in this plan |
| Expected machine effect | None |
| Privacy impact | Synthetic-only evidence and retention boundary required |
| Success condition | coordinate domains, owners, edges and crop method are traceable |
| Failure condition | mixed coordinate units or undefined rounding are used as product decisions |
| Stop condition | any display mutation, runtime capture or unapproved evidence write |
| Rollback／cleanup | remove only future draft evidence record; no runtime cleanup now |
| Resulting prerequisite recommendation | `CAP-PREQ-014..016,022,023` → `Partially resolved` after contract evidence |
| Resulting blocker recommendation | `CAP-BLOCK-004`, `007` remain `Open` until required evidence |
| Resulting pair recommendation | pair readiness remains `Blocked` until mapping evidence |
| Phase C1 impact | Blocking |
| Owner | TBD |
| Status | `Blocked` |
| Open questions | rounding policy、edge semantics、timestamp tolerance、crop owner、evidence root |

### 17.6 `CAP-CLOSE-006` — Evidence／Privacy／cleanup boundary

| Field | Value |
|---|---|
| Closure Action ID | `CAP-CLOSE-006` |
| Source Blocking Action | `CAP-BA-006` |
| Blocking condition | Evidence、Privacy、retention 與 cleanup boundary 未具名 |
| Related `CAP-PREQ` | `CAP-PREQ-025`, `CAP-PREQ-026`, `CAP-PREQ-030` |
| Related `CAP-BLOCK` | `CAP-BLOCK-008`, `CAP-BLOCK-009` |
| Related `CAP-PAIR` | `CAP-PAIR-001..010` |
| Related `CAP-SPIKE` | `CAP-SPIKE-001..005` |
| Related `CAP-GATE` | `CAP-CGATE-007..009` |
| Dependency ownership | `Evidence`; `Authorization` |
| Shared UI source IDs | `RESEARCH-TECH-UI-009`, `UI-AUTH-006`, `UI-AUTH-007` |
| Rendering source IDs | `RESEARCH-TECH-RENDER-003` only for synthetic/result boundary |
| Existing evidence | 上游已要求 synthetic-only、privacy review、cleanup and no persistence without authority |
| Current limitation | reviewer、retention、artifact classes、cleanup owner and stop record are not approved |
| Required final evidence | privacy review, retention rule, artifact manifest, cleanup confirmation and owner |
| Proposed closure operation | document governance and future evidence decision boundary |
| Operation classification | `Repository documentation mutation`; future `Evidence capture／persistence` |
| Exact scope | `CAP-PREQ-025`, `026`, `030` |
| Explicit exclusions | 不建立 Evidence、Frame、PNG、Result directory；不使用 real desktop |
| Official-source research required | No in this plan |
| Local inspection required | No |
| Network access required | No |
| Package acquisition required | No |
| Installation required | No |
| Repository mutation required | Documentation only |
| Experimental project required | No for plan |
| Restore required | No |
| Build required | No |
| Runtime execution required | No |
| Capture API invocation required | No |
| Evidence write required | No now; future separately authorized |
| Display／system mutation required | No |
| Administrator privilege required | No |
| Human authorization required | Yes before any artifact persistence |
| Expected files／directories | None in this plan |
| Expected machine effect | None |
| Privacy impact | High if future controls fail; synthetic-only and no security bypass are mandatory |
| Success condition | named reviewer, retention, stop, cleanup and evidence root boundaries |
| Failure condition | session observation is treated as persistent evidence or real desktop is used |
| Stop condition | privacy breach, unapproved artifact, missing cleanup or missing authority |
| Rollback／cleanup | future artifact manifest and cleanup record; no artifact now |
| Resulting prerequisite recommendation | `CAP-PREQ-025,026,030` → `Partially resolved` after governance evidence |
| Resulting blocker recommendation | `CAP-BLOCK-008`, `009` remain `Open` until review |
| Resulting pair recommendation | pair readiness cannot be `Ready` without privacy/evidence authority |
| Phase C1 impact | Blocking |
| Owner | TBD |
| Status | `Blocked` |
| Open questions | evidence root、retention、reviewer、cleanup owner、artifact classes、privacy sign-off |

### 17.7 `CAP-CLOSE-007` — Runtime execution scope

| Field | Value |
|---|---|
| Closure Action ID | `CAP-CLOSE-007` |
| Source Blocking Action | `CAP-BA-007` |
| Blocking condition | Runtime execution scope 未取得 |
| Related `CAP-PREQ` | `CAP-PREQ-028` |
| Related `CAP-BLOCK` | `CAP-BLOCK-011` |
| Related `CAP-PAIR` | `CAP-PAIR-001..010` |
| Related `CAP-SPIKE` | `CAP-SPIKE-001..005`, `CAP-SPIKE-011` |
| Related `CAP-GATE` | `CAP-CGATE-008`, `CAP-CGATE-010` |
| Dependency ownership | `Authorization` |
| Shared UI source IDs | `RESEARCH-TECH-UI-009`, `UI-AUTH-008` |
| Rendering source IDs | `RESEARCH-TECH-RENDER-003` only as independent dependency |
| Existing evidence | Runtime authority is explicitly separate and currently not granted |
| Current limitation | no named owner, spike scope, host, stop condition, evidence permission or expiry |
| Required final evidence | future authorization input listing exact Spike IDs, pair, host, stop/cleanup and evidence scope |
| Proposed closure operation | prepare a future authorization review package; do not submit or approve it here |
| Operation classification | `Repository documentation mutation`; future `Capture runtime execution` |
| Exact scope | `CAP-PREQ-028`, `CAP-SPIKE-001..005`, `011` |
| Explicit exclusions | 不呼叫 API、不 Run、不建立 frame、不寫 evidence、不選 backend |
| Official-source research required | No in this plan |
| Local inspection required | No |
| Network access required | No |
| Package acquisition required | No |
| Installation required | No |
| Repository mutation required | No beyond future bounded authorization record |
| Experimental project required | Future only, separately authorized |
| Restore required | Future only if separately authorized |
| Build required | Future only if separately authorized |
| Runtime execution required | Future only, separately authorized |
| Capture API invocation required | Future only, separately authorized |
| Evidence write required | Future only, separately authorized |
| Display／system mutation required | No in this plan |
| Administrator privilege required | Not assumed; future scope must state it explicitly |
| Human authorization required | Yes, explicit and per operation |
| Expected files／directories | None in this plan |
| Expected machine effect | None |
| Privacy impact | Future capture may expose data; synthetic-only condition is mandatory |
| Success condition | future review has exact Spike IDs、host、stop conditions and authority owner |
| Failure condition | runtime authority is inferred from build, UI, or documentation status |
| Stop condition | any missing authority, privacy violation, unexpected desktop content or cleanup failure |
| Rollback／cleanup | future runtime-specific cleanup record; no runtime now |
| Resulting prerequisite recommendation | `CAP-PREQ-028` remains `Blocked` until independent authorization |
| Resulting blocker recommendation | `CAP-BLOCK-011` remains `Open` |
| Resulting pair recommendation | no pair can be runtime-ready under current authority |
| Phase C1 impact | Blocking |
| Owner | TBD |
| Status | `Blocked` |
| Open questions | decision owner、exact scope、expiry、stop conditions、privacy reviewer、evidence write authority |

## 18. Recommended Closure Order

固定順序如下：

1. 確認 Shared UI Host evidence 與 authority reuse boundary。
2. 固定 Phase C1 Candidate API／SDK／interop identity。
3. 固定 Candidate–Host Project、Package／Restore 與 Build scope。
4. 固定 Basic Synthetic Scene。
5. 固定 Coordinate、Crop、Frame metadata 與 Privacy evidence method。
6. 固定 Result storage、Evidence write 與 cleanup boundary。
7. 未來提交 Capture prerequisite closure execution authorization review。
8. 未來執行獲准 Closure Action。
9. 未來重新評估 Capture Runtime Spike readiness。

本文件只完成第 1 至第 6 項的規劃，不執行第 7 至第 9 項。

## 19. Authorization Boundary

所有 `Current authorization` 固定為 `Not granted`，所有 `Execution permitted` 固定為 `No`。

| Operation | Current authorization | Execution permitted |
|---|---|---|
| Official-source research | Not granted | No |
| Local read-only inspection | Not granted | No |
| Package acquisition | Not granted | No |
| SDK／Tool installation | Not granted | No |
| Synthetic asset creation | Not granted | No |
| Experimental Project creation | Not granted | No |
| Restore | Not granted | No |
| Build | Not granted | No |
| Capture API invocation | Not granted | No |
| Runtime execution | Not granted | No |
| Evidence write | Not granted | No |
| Result directory creation | Not granted | No |
| Display／system mutation | Not granted | No |

Build authority 不等於 Runtime authority；Runtime authority 不等於 Evidence persistence authority；UI authority 不等於 Capture authority。

## 20. Closure Plan Status

`Closure plan complete`、`Partially complete`、`Incomplete` 是本文件內容完整度，不是執行結果。

本文件目前：

- Closure plan status: `Closure plan complete`
- Closure execution status: `Not started`
- Closure execution authorized: `No`
- Build Verification: `Not performed`
- Runtime Verification: `Not performed`
- Capture Runtime Spike Authorized: `No`
- Evidence Write Authorized: `No`
- Capture Decision: `Not made`
- Rendering Decision: `Not made`

另行判定的 future authorization review status 只能使用：

- `Ready to request capture prerequisite closure execution authorization`
- `Conditionally ready to request capture prerequisite closure execution authorization`
- `Not ready`

依目前 authorization boundary，future review recommendation 為 `Not ready`；這不是 Capture Runtime Spike readiness，也不是執行授權。

## 21. Traceability

```text
CAP-PREQ / CAP-BLOCK
        ↓
CAP-BA
        ↓
CAP-CLOSE
        ↓
Required authority
        ↓
Future closure evidence
        ↓
Candidate–Host readiness
        ↓
CAP Spike readiness reassessment
        ↓
Future Capture Backend decision
```

本文件至少引用：

- `RESEARCH-TECH-CAPTURE-001`
- `RESEARCH-TECH-CAPTURE-002`
- `RESEARCH-TECH-CAPTURE-003`
- `RESEARCH-TECH-UI-007`
- `RESEARCH-TECH-UI-008`
- `RESEARCH-TECH-UI-009`
- `RESEARCH-TECH-RENDER-003`
- `Architecture/adr/ADR-0002-ui-framework-selection.md`
- `Architecture/TECHNOLOGY-DECISION-ROADMAP.md`
- 相關 Frozen PRD、Specs、Architecture

實際文件名稱與 ID 必須從 Repository 原樣引用。

## 完成條件

- 只建立 `23-capture-backend-prerequisite-closure-plan.md`。
- 不修改任何其他文件。
- 建立正好七個 `CAP-CLOSE`。
- 保持七組 `CAP-BA → CAP-CLOSE` 一對一。
- 不新增、刪除、合併或拆分 `CAP-BA`。
- 完整覆蓋 30 個 prerequisite、12 個 blocker、10 個 pair、12 個 spike。
- 建立 10 個 Phase C1 Minimum Closure Gates。
- 明確重用 Shared UI authority，不建立重複授權。
- 建立 Rendering Dependency Boundary，不選擇 Rendering Technology。
- 所有 `Current authorization = Not granted`。
- 所有 `Execution permitted = No`。
- 不執行官方研究、本機盤點、下載、安裝、Restore、Build、Run 或 Capture Runtime Spike。
- 不建立 Project、Prototype、Result、Source Code、Capture Frame 或 Evidence。
- 不建立 Capture ADR。
- 不修改 UI／Rendering Research Line。
- `git diff --check` 通過。

完成後停止。
