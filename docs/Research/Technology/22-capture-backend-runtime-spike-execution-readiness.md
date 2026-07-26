# Capture Backend Runtime Spike Execution Readiness

## Document Control

| Field | Value |
|---|---|
| Document ID | `RESEARCH-TECH-CAPTURE-003` |
| Title | Capture Backend Runtime Spike Execution Readiness |
| Status | Draft |
| Research Type | Runtime Execution Readiness |
| Parent Runtime Plan | `RESEARCH-TECH-CAPTURE-002` |
| Parent Feasibility | `RESEARCH-TECH-CAPTURE-001` |
| Execution Status | Not started |
| Build Verification | Not performed |
| Runtime Verification | Not performed |
| UI Framework Decision | Unresolved — `ADR-0002` remains Draft |
| Rendering Decision | Not made |
| Capture Decision | Not made |
| Capture Runtime Spike Authorized | No |
| Evidence Write Authorized | No |
| Owner | TBD |
| Last reviewed | Not reviewed |

本文件是 Execution Readiness Assessment，不是新的 Runtime Plan、Authorization Request 或執行紀錄。它只判斷前置條件、阻塞、Candidate–Host pair、Synthetic Scene、Environment、Evidence/Privacy 與授權依賴是否足以進入未來的執行授權審查。

## 1. Purpose

本文件只回答：

> 執行 `CAP-SPIKE-001` 至 `CAP-SPIKE-012` 前，哪些 Host、Capture API、Project、Build、Display、Synthetic Scene、Evidence、Privacy 與授權條件必須先具備；目前各 Candidate–Host 組合及各 Spike 是否已可執行？

本文件不執行任何 Spike，也不把 readiness 判斷當成 execution authorization。

## 2. Scope

只評估：

- `CAP-OPT-001..005`。
- WinUI 3 與 WPF。
- `CAP-SPIKE-001..012`。
- `CAP-001..022`。
- `CAP-GATE-001..011`。
- `CAP-GAP-001..012`。
- Capture API/SDK/Interop prerequisites。
- Experimental Project/Restore/Build prerequisites。
- Synthetic capture scene readiness。
- Display/DPI/HDR environment readiness。
- Evidence capability 與 privacy controls。
- Capture Runtime execution authorization dependency。

## 3. Non-goals

本文件不得：

- 執行 Capture API。
- 執行任何 Spike。
- 建立 Project、Solution、Prototype 或 Source Code。
- 擷取桌面、視窗或螢幕。
- 建立 Screenshot、Recording、Frame、PNG 或 Pixel-difference Artifact。
- 建立 Result directory。
- 執行下載、安裝、Restore、Build、Run 或 Publish。
- 選擇 Capture Backend。
- 建立 Capture ADR。
- 修改 UI/Rendering Research Line。
- 修改 `RESEARCH-TECH-CAPTURE-001` 或 `RESEARCH-TECH-CAPTURE-002`。
- 開始正式截圖功能。

## 4. Controlled Vocabulary

### 4.1 Prerequisite Status

只能使用：`Resolved`、`Partially resolved`、`Blocked`、`Deferred`、`Not applicable`。

### 4.2 Candidate–Host Readiness

只能使用：`Ready`、`Conditionally ready`、`Blocked`、`Excluded with evidence`、`Not evaluated`。

### 4.3 Spike Readiness

只能使用：`Ready`、`Blocked`、`Deferred`、`Not applicable`。

### 4.4 Authorization Status

只能使用：`Not granted`、`Pending separate authorization`。

本文件不把任何文件 review、靜態檢查或既有規劃視為 runtime authorization；不得以 readiness 文字代替真正的授權輸入。

## 5. Dependency Classification

| Class | 說明 |
|---|---|
| `Shared UI-host dependency` | WinUI 3/WPF、SDK、Build Tool、Project isolation。 |
| `Capture-candidate dependency` | WGC、DXGI、GDI、Window-oriented、Hybrid 專屬需求。 |
| `Graphics-device dependency` | D3D11 device、DXGI adapter、frame pool、device loss。 |
| `Display-environment dependency` | Monitor、DPI、HDR、color、topology。 |
| `Synthetic-scene dependency` | 公開且無敏感資訊的固定擷取場景。 |
| `Evidence dependency` | Frame、metadata、coordinate、timing、privacy review。 |
| `Authorization dependency` | Project、Restore、Build、Runtime、Evidence write。 |

### 5.1 Dependency rules

- 共用 UI Host blocker 必須引用既有 UI Research ID。
- Capture 不得重新建立 Shared UI authorization。
- Build authority 不代表 Runtime authority。
- Runtime authority 不代表 Evidence persistence authority。
- Rendering prerequisite 只有在建立 synthetic scene 或檢查結果必須顯示時才列為依賴；不得讓 Capture Research 選擇 Rendering Technology。
- 沒有實際證據時不得將 prerequisite 標示為 `Resolved`。

## 6. Shared Research Dependency Matrix

| Capture requirement | Source research item | Current status | Reusable evidence | Remaining Capture condition |
|---|---|---|---|---|
| Windows 11 x64 baseline | `RESEARCH-TECH-CAPTURE-001`、`RESEARCH-TECH-CAPTURE-002` | Partially resolved | Official capability baseline、runtime plan | 實際 host/environment record；尚未檢查 |
| WinUI 3 experimental build path | `RESEARCH-TECH-UI-007` | Blocked | UI research boundary only | 具體實驗版本、Project/build path 與分離授權 |
| WPF experimental build path | `RESEARCH-TECH-UI-008` | Blocked | WPF host scope only | 具體實驗版本、Project/build path 與分離授權 |
| .NET/Windows SDK | `RESEARCH-TECH-UI-009`、`RESEARCH-TECH-RENDER-003` | Blocked | Version is not fixed by prior research | exact baseline、local availability 與 build authority |
| Windows App SDK | `RESEARCH-TECH-UI-007` | Blocked | Official/source boundary only | package/runtime identity 與 host compatibility |
| Experimental Project isolation | `RESEARCH-TECH-RENDER-003` | Blocked | Readiness boundary；未建立 project | project creation authority、source isolation、build authority |
| Display topology | `RESEARCH-TECH-CAPTURE-001` | Blocked | Coordinate contract與spike plan | lawful synthetic display environment |
| Per-monitor DPI | `RESEARCH-TECH-CAPTURE-001`、`RESEARCH-TECH-UI-001` | Blocked | DPI terminology與mapping boundary | host context、mixed-DPI observation |
| GPU/driver | `RESEARCH-TECH-CAPTURE-001` | Blocked | candidate risk catalog | local environment record、device observation |
| HDR observation | `RESEARCH-TECH-CAPTURE-001` | Deferred | HDR risk remains open | lawful HDR/SDR branch與privacy boundary |
| Evidence storage policy | `RESEARCH-TECH-CAPTURE-002` | Blocked | result artifact plan only | Evidence write decision；目前固定 No |
| Safety/cleanup | `RESEARCH-TECH-CAPTURE-002` | Partially resolved | stop/cleanup rules | execution-specific cleanup evidence |
| Shared Project/Restore/Build authority | `RESEARCH-TECH-RENDER-003` | Blocked | no command execution | separate human/owner authorization |
| Shared Runtime authority | `RESEARCH-TECH-RENDER-003` | Blocked | runtime not performed | separate execution authorization |

此表只重用上游研究的邊界，不重新建立 UI、Rendering 或 Project authorization。

## 7. Capture Prerequisite Register

### 7.1 Register contract

每個 prerequisite 必須包含下列欄位；本表的 `Current status` 只使用第 4 節 vocabulary，沒有實際證據的項目不標示為 `Resolved`。

| ID | Description | Dependency class | Related candidates | Related hosts | Related spikes | Related criteria | Related gates | Existing evidence | Current status | Required final evidence | Local inspection required | Package acquisition required | Project creation required | Restore required | Build required | Runtime required | Evidence write required | Authorization required | Resolution condition | Owner | Open questions |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| `CAP-PREQ-001` | Host Framework experimental version | Shared UI-host dependency | 001–005 | WinUI 3/WPF | 001–012 | 020, 021 | 010 | UI research boundary | Blocked | exact host/version pair | Yes | No | Yes | No | Yes | Yes | No | Pending separate authorization | host/version recorded and isolated | TBD | exact versions |
| `CAP-PREQ-002` | .NET/Windows SDK baseline | Shared UI-host dependency | 001–005 | WinUI 3/WPF | 001–012 | 001, 020, 021 | 010 | official/local split | Blocked | baseline identity and local evidence | Yes | No | No | No | No | No | No | Pending separate authorization | authorized read-only inspection and later project decision | TBD | SDK/runtime boundary |
| `CAP-PREQ-003` | Windows Graphics Capture API identity and activation path | Capture-candidate dependency | 001 | WinUI 3/WPF | 001, 002, 004, 006, 008, 010, 011, 012 | 001, 002, 020, 021 | 001, 005, 007, 009, 010 | official baseline | Partially resolved | exact experimental identity and host path | Yes | No | Yes | No | Yes | Yes | No | Pending separate authorization | API identity plus isolated host path documented | TBD | activation/consent |
| `CAP-PREQ-004` | WGC Capture Item creation path | Capture-candidate dependency | 001 | WinUI 3/WPF | 001, 002, 006, 011 | 002, 003, 004, 010, 021 | 001, 002, 010 | official docs incomplete for product host | Blocked | host creation observation | No | No | Yes | No | Yes | Yes | No | Pending separate authorization | future runtime evidence | TBD | item source boundary |
| `CAP-PREQ-005` | WGC frame-pool and graphics-device requirement | Graphics-device dependency | 001 | WinUI 3/WPF | 001, 008, 010, 012 | 002, 018, 019, 021 | 001, 007, 009, 010 | official frame-pool baseline | Blocked | device/frame lifecycle evidence | Yes | No | Yes | No | Yes | Yes | No | Pending separate authorization | device ownership and recovery observed | TBD | device/thread ownership |
| `CAP-PREQ-006` | DXGI Desktop Duplication and D3D11 requirement | Capture-candidate dependency | 002 | WinUI 3/WPF | 001, 002, 003, 005, 007, 008, 010, 011, 012 | 001–005, 018, 019, 021 | 001–004, 006, 007, 009, 010 | official DXGI baseline | Partially resolved | adapter/device/output observation | Yes | No | Yes | No | Yes | Yes | No | Pending separate authorization | device/output path isolated | TBD | D3D11/COM boundary |
| `CAP-PREQ-007` | Adapter/output selection strategy | Graphics-device dependency | 002, 005 | WinUI 3/WPF | 002, 003, 004, 010 | 003, 005, 006, 019 | 002, 003, 009 | official per-output evidence | Blocked | output mapping evidence | Yes | No | Yes | No | Yes | Yes | No | Pending separate authorization | output identity maps to monitor bounds | TBD | rotation/topology |
| `CAP-PREQ-008` | GDI capture API and bitmap format requirement | Capture-candidate dependency | 003 | WinUI 3/WPF | 001, 002, 003, 005, 006, 007, 008, 012 | 001–004, 008, 009, 014, 018 | 001–005, 007, 010 | official GDI baseline | Partially resolved | HDC/bitmap/format observation | Yes | No | Yes | No | Yes | Yes | No | Pending separate authorization | isolated native bitmap path | TBD | color/cleanup |
| `CAP-PREQ-009` | Window-oriented candidate source limitations | Capture-candidate dependency | 004 | WinUI 3/WPF | 001, 002, 005, 006, 009, 011 | 001, 002, 009, 012, 020, 021 | 001, 005, 008, 010 | candidate family only | Blocked | exact API identity and limitation evidence | Yes | No | Yes | No | Yes | Yes | No | Pending separate authorization | exact candidate is named and reviewed | TBD | occlusion/minimized |
| `CAP-PREQ-010` | Hybrid composition and fallback boundary | Capture-candidate dependency | 005 | WinUI 3/WPF | 001, 002, 003, 005, 006, 007, 008, 010, 011, 012 | 001–022 | 001–011 | parent strategy only | Blocked | named primary/fallback composition and failure contract | Yes | No | Yes | No | Yes | Yes | No | Pending separate authorization | component candidates independently eligible | TBD | duplicate/fallback semantics |
| `CAP-PREQ-011` | Candidate–Host interop path | Shared UI-host dependency | 001–005 | WinUI 3/WPF | 001, 004, 005, 006, 010, 011, 012 | 020, 021 | 009, 010 | host matrix only | Blocked | per-pair ownership and cleanup evidence | Yes | No | Yes | No | Yes | Yes | No | Pending separate authorization | pair register not blocked by unknown identity | TBD | thread/resource ownership |
| `CAP-PREQ-012` | Packaged/unpackaged requirements | Authorization dependency | 001–005 | WinUI 3/WPF | 001, 002, 004, 011 | 020, 021 | 010 | parent compatibility boundary | Blocked | separate host/package record | Yes | No | Yes | No | Yes | Yes | No | Pending separate authorization | packaging state explicitly scoped | TBD | consent/manifest |
| `CAP-PREQ-013` | Synthetic scene definition | Synthetic-scene dependency | 001–005 | WinUI 3/WPF | 001–012 | 008, 013, 014, 022 | 004, 007, 011 | scene contract only | Blocked | scene specification and later fixture evidence | No | No | No | No | No | Yes | No | Pending separate authorization | scene contract reviewed | TBD | fixed dimensions |
| `CAP-PREQ-014` | Coordinate reference model | Evidence dependency | 001–005 | WinUI 3/WPF | 002–006, 011 | 003, 006, 007, 008, 021 | 002–004, 010 | coordinate contract | Blocked | mapping record and edge semantics | No | No | No | No | No | Yes | No | Pending separate authorization | all domains have owner/unit/origin | TBD | rounding |
| `CAP-PREQ-015` | Negative-coordinate scenario | Display-environment dependency | 001–005 | WinUI 3/WPF | 002, 003, 004, 005, 011 | 006–008 | 002–004 | planned scene only | Blocked | lawful synthetic topology observation | Yes | No | No | No | No | Yes | No | Pending separate authorization | signed coordinates observed | TBD | display topology |
| `CAP-PREQ-016` | Mixed-DPI scenario | Display-environment dependency | 001–005 | WinUI 3/WPF | 004, 005, 011 | 007, 008, 021 | 003, 004, 010 | DPI boundary only | Blocked | host/monitor DPI mapping evidence | Yes | No | No | No | No | Yes | No | Pending separate authorization | DPI context recorded | TBD | host awareness |
| `CAP-PREQ-017` | Overlay self-capture test boundary | Evidence dependency | 001–005 | WinUI 3/WPF | 006 | 009, 010, 012 | 005, 011 | overlay boundary only | Blocked | synthetic inclusion/exclusion observation | No | No | No | No | No | Yes | No | Pending separate authorization | no product overlay required | TBD | timing/flicker |
| `CAP-PREQ-018` | Cursor control test boundary | Evidence dependency | 001–005 | WinUI 3/WPF | 007 | 011 | 006 | cursor risk baseline | Blocked | cursor state/metadata evidence | No | No | No | No | No | Yes | No | Pending separate authorization | synthetic cursor fixture defined | TBD | composition |
| `CAP-PREQ-019` | HDR/SDR observation boundary | Display-environment dependency | 001, 002, 003, 005 | WinUI 3/WPF | 008 | 013, 014 | 007 | official format/color gap | Deferred | lawful synthetic HDR/SDR evidence | Yes | No | No | No | No | Yes | No | Pending separate authorization | C2 branch conditions defined | TBD | hardware/profile |
| `CAP-PREQ-020` | Protected-content substitute | Display-environment dependency | 001–004 | WinUI 3/WPF | 009 | 015, 016 | 008 | security/privacy boundary | Blocked | refusal/boundary classification | No | No | No | No | No | Yes | No | Pending separate authorization | no bypass and public fixture | TBD | failure taxonomy |
| `CAP-PREQ-021` | Device-loss/display-change trigger | Graphics-device dependency | 001, 002, 003, 005 | WinUI 3/WPF | 010, 012 | 018, 019, 020 | 009 | recovery plan only | Blocked | invalidation/recreate/cleanup evidence | Yes | No | No | No | No | Yes | No | Pending separate authorization | safe trigger is specified | TBD | event safety |
| `CAP-PREQ-022` | Frame/crop evidence method | Evidence dependency | 001–005 | WinUI 3/WPF | 001–005 | 002–008, 022 | 001–004, 011 | result artifact plan only | Blocked | session/evidence method | No | No | No | No | No | Yes | No | Pending separate authorization | evidence method reviewed | TBD | persistence boundary |
| `CAP-PREQ-023` | Pixel-difference method | Evidence dependency | 001–005 | WinUI 3/WPF | 005 | 008, 014, 022 | 004, 007, 011 | threshold remains TBD | Blocked | synthetic diff observation | No | No | No | No | No | Yes | No | Pending separate authorization | method and threshold owner defined | TBD | color/rounding |
| `CAP-PREQ-024` | Timing/resource observation method | Evidence dependency | 001, 002, 003, 005 | WinUI 3/WPF | 001, 010, 012 | 017, 018, 019, 022 | 001, 009, 011 | measurement vocabulary only | Blocked | metadata-only measurement evidence | No | No | No | No | No | Yes | No | Pending separate authorization | metric definitions and threshold owner | TBD | sampling |
| `CAP-PREQ-025` | Privacy review | Evidence dependency | 001–005 | WinUI 3/WPF | 001–012 | 015, 016, 022 | 008, 011 | privacy rules | Partially resolved | review/cleanup confirmation | No | No | No | No | No | Yes | No | Pending separate authorization | review checklist accepted | TBD | redaction |
| `CAP-PREQ-026` | Result storage and cleanup | Evidence dependency | 001–005 | WinUI 3/WPF | 001–012 | 018, 019, 022 | 009, 011 | result plan only | Blocked | explicit retention/cleanup decision | No | No | No | No | No | Yes | Yes | Pending separate authorization | persistence boundary separately decided | TBD | retention |
| `CAP-PREQ-027` | Project/Restore/Build authorization | Authorization dependency | 001–005 | WinUI 3/WPF | 001–012 | 020, 021 | 010 | no authorization in this document | Blocked | separate authorization input | No | No | Yes | Yes | Yes | No | No | Pending separate authorization | owner grants exact scope | TBD | no implementation |
| `CAP-PREQ-028` | Runtime execution authorization | Authorization dependency | 001–005 | WinUI 3/WPF | 001–012 | 001–022 | 001–011 | no authorization in this document | Blocked | separate execution authorization | No | No | No | No | No | Yes | No | Pending separate authorization | named scope/conditions/date | TBD | human decision |
| `CAP-PREQ-029` | Evidence write authorization | Authorization dependency | 001–005 | WinUI 3/WPF | 001–012 | 022 | 011 | fixed No | Blocked | separate persistence authorization | No | No | No | No | No | No | Yes | Pending separate authorization | named destination/retention/format | TBD | sensitive data |
| `CAP-PREQ-030` | Stop and cleanup authority | Authorization dependency | 001–005 | WinUI 3/WPF | 001–012 | 015, 016, 019, 022 | 008, 009, 011 | plan stop rules | Partially resolved | execution-specific stop/cleanup confirmation | No | No | No | No | No | Yes | No | Pending separate authorization | stop owner and cleanup boundary named | TBD | interruption |

## 8. Blocker Register

Blocker Status 只能使用：`Open`、`Resolved`、`Accepted limitation`。本文件不關閉任何 blocker，因為 readiness 文件本身沒有實際執行證據。

| Blocker ID | Source prerequisite | Description | Severity | Affected candidates | Affected hosts | Affected spikes | Affected phase | Required resolution | Evidence required | Shared dependency | Authorization dependency | Owner | Status |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| `CAP-BLOCK-001` | `CAP-PREQ-001`, `002` | Host Framework、SDK、runtime exact identity 未固定。 | Blocking | 001–005 | WinUI 3/WPF | 001–012 | C1–C3 | 取得具名實驗 baseline 並保留 source boundary。 | host/version record | `RESEARCH-TECH-UI-007..009` | Pending separate authorization | TBD | Open |
| `CAP-BLOCK-002` | `CAP-PREQ-003..011` | Candidate API/interop path 尚未形成可隔離的實驗 pair。 | Blocking | 001–005 | WinUI 3/WPF | 001–012 | C1 | 完成 candidate identity 與 pair prerequisites。 | API identity、ownership、cleanup | `RESEARCH-TECH-CAPTURE-001` | Pending separate authorization | TBD | Open |
| `CAP-BLOCK-003` | `CAP-PREQ-013` | Synthetic Scene 只有規劃，沒有可供後續執行的固定 fixture。 | Blocking | 001–005 | WinUI 3/WPF | 001–005 | C1 | 以文件與另行授權的 fixture 定義完成 scene。 | scene reference、privacy review | Capture-only | Pending separate authorization | TBD | Open |
| `CAP-BLOCK-004` | `CAP-PREQ-014..016` | Coordinate domains、negative coordinate、mixed-DPI 的 execution evidence 尚未存在。 | Blocking | 001–005 | WinUI 3/WPF | 002–005, 011 | C1 | 取得 mapping/edge evidence；rounding 維持 `TBD` 直到決策。 | coordinate record、crop comparison | `RESEARCH-TECH-CAPTURE-001` | Pending separate authorization | TBD | Open |
| `CAP-BLOCK-005` | `CAP-PREQ-017..020` | Overlay、Cursor、HDR/SDR、protected boundary 尚未具備可執行 evidence boundary。 | Non-blocking for C1 | 001–005 | WinUI 3/WPF | 006–009 | C2 | C2 branch 條件與 synthetic/privacy boundary。 | functional/security/color observation | Capture-only | Pending separate authorization | TBD | Open |
| `CAP-BLOCK-006` | `CAP-PREQ-021` | Device-loss/display-change trigger 尚未證明可安全且可重複。 | Non-blocking for C1 | 001, 002, 003, 005 | WinUI 3/WPF | 010, 012 | C3 | safe trigger、cleanup/recreate method。 | invalidation/recovery record | Capture-only | Pending separate authorization | TBD | Open |
| `CAP-BLOCK-007` | `CAP-PREQ-022..024` | Frame/crop、pixel-difference、timing/resource evidence method 尚未有可保存邊界。 | Blocking | 001–005 | WinUI 3/WPF | 001–005, 012 | C1–C3 | 先確定 session-only 或 future persistence boundary。 | evidence schema、measurement definitions | Capture-only | Pending separate authorization | TBD | Open |
| `CAP-BLOCK-008` | `CAP-PREQ-025`, `030` | Privacy review、stop、cleanup 的執行 owner 未具名。 | Blocking | 001–005 | WinUI 3/WPF | 001–012 | C1–C3 | 指定 review/stop/cleanup owner 與 redaction rule。 | privacy/cleanup confirmation | Capture-only | Pending separate authorization | TBD | Open |
| `CAP-BLOCK-009` | `CAP-PREQ-026`, `029` | Evidence persistence 目前明確為 No；不得建立結果檔或目錄。 | Blocking for persistent evidence | 001–005 | WinUI 3/WPF | 001–012 | C1–C3 | 另行決定 session-only 或 persistence scope。 | retention/format/destination | Capture-only | Pending separate authorization | TBD | Open |
| `CAP-BLOCK-010` | `CAP-PREQ-027` | Project/Restore/Build authority 尚未取得。 | Blocking for project-based execution | 001–005 | WinUI 3/WPF | 001–012 | C1–C3 | 具名 owner 指定 exact scope；不由本文件推導。 | authorization input | `RESEARCH-TECH-RENDER-003` | Pending separate authorization | TBD | Open |
| `CAP-BLOCK-011` | `CAP-PREQ-028` | Runtime execution authority 尚未取得。 | Blocking | 001–005 | WinUI 3/WPF | 001–012 | C1–C3 | 另行取得具名、範圍與 stop condition 的 execution decision。 | execution authorization input | Capture-only | Pending separate authorization | TBD | Open |
| `CAP-BLOCK-012` | `CAP-PREQ-009`, `010` | Window-oriented exact identity 與 Hybrid composition 尚未固定。 | Blocking for affected candidates | 004, 005 | WinUI 3/WPF | 001–012 | C1–C3 | exact API/source limitation與組成候選完成。 | identity/fallback evidence | Capture-only | Pending separate authorization | TBD | Open |

## 9. Candidate Identity and Local Availability Baseline

本表不做 ranking。`Local availability` 沒有現行證據時固定為 `Unknown`；所有 `Build verified` 與 `Runtime verified` 固定為 `No`。

| Candidate | API/SDK identity | Host | Exact experimental identity | Official evidence | Local availability | Build verified | Runtime verified | Status |
|---|---|---|---|---|---|---|---|---|
| `CAP-OPT-001` Windows Graphics Capture | `Windows.Graphics.Capture` / WinRT | WinUI 3/WPF | exact SDK/runtime version `TBD` | Official baseline in `RESEARCH-TECH-CAPTURE-001` | Unknown | No | No | Partially resolved |
| `CAP-OPT-002` DXGI Desktop Duplication | `IDXGIOutputDuplication` / D3D11/DXGI | WinUI 3/WPF | exact SDK/runtime/device identity `TBD` | Official baseline in `RESEARCH-TECH-CAPTURE-001` | Unknown | No | No | Partially resolved |
| `CAP-OPT-003` GDI-based desktop capture | Win32 GDI / `BitBlt` / bitmap | WinUI 3/WPF | exact managed/native boundary `TBD` | Official baseline in `RESEARCH-TECH-CAPTURE-001` | Unknown | No | No | Partially resolved |
| `CAP-OPT-004` Window-oriented capture mechanisms | exact API identity `TBD` | WinUI 3/WPF | exact API/source identity `TBD` | Candidate family only | Unknown | No | No | Blocked |
| `CAP-OPT-005` Hybrid primary/fallback | named composition of selected APIs `TBD` | WinUI 3/WPF | primary/fallback versions `TBD` | Derived strategy only | Unknown | No | No | Blocked |

## 10. Candidate–Host Pair Register

Pair ID 由 `CAP-PAIR-001` 起。五個 Candidate × 兩個 Host 正好十列；即使未知或阻塞也保留列。所有 `Execution authorization` 只能為 `Not granted` 或 `Pending separate authorization`。

| Pair ID | Candidate | Host | Eligibility from | Required API/SDK | Required graphics device | Native interop requirement | Packaging requirement | Project requirement | Restore requirement | Build requirement | Runtime requirement | Current readiness | Blocking IDs | Exclusion evidence | Execution authorization | Notes |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| `CAP-PAIR-001` | 001 | WinUI 3 | Conditionally eligible | WinRT capture item/frame pool | graphics device/frame pool | WinRT/graphics resource | manifest/consent boundary `TBD` | Yes | No | Yes | Yes | Blocked | 001, 002, 003, 004 | None | Not granted | WGC host path未證明 |
| `CAP-PAIR-002` | 001 | WPF | Conditionally eligible | WinRT capture item/frame pool | graphics device/frame pool | WinRT/bitmap/resource | packaging/consent `TBD` | Yes | No | Yes | Yes | Blocked | 001, 002, 003, 004 | None | Not granted | WPF frame ownership未證明 |
| `CAP-PAIR-003` | 002 | WinUI 3 | Conditionally eligible | DXGI duplication/D3D11 | D3D11 adapter/output | COM/GPU resource | packaged state `TBD` | Yes | No | Yes | Yes | Blocked | 001, 002, 006, 007 | None | Not granted | output mapping未證明 |
| `CAP-PAIR-004` | 002 | WPF | Conditionally eligible | DXGI duplication/D3D11 | D3D11 adapter/output | COM/GPU/bitmap | packaged state `TBD` | Yes | No | Yes | Yes | Blocked | 001, 002, 006, 007 | None | Not granted | WPF resource ownership未證明 |
| `CAP-PAIR-005` | 003 | WinUI 3 | Conditionally eligible | GDI/DC/bitmap | not required by API identity; device behavior `TBD` | HDC/HBITMAP interop | packaged state `TBD` | Yes | No | Yes | Yes | Blocked | 001, 002, 008 | None | Pending separate authorization | color/DPI behavior未證明 |
| `CAP-PAIR-006` | 003 | WPF | Conditionally eligible | GDI/DC/bitmap | not required by API identity; device behavior `TBD` | HDC/HBITMAP/managed bitmap | packaged state `TBD` | Yes | No | Yes | Yes | Blocked | 001, 002, 008 | None | Pending separate authorization | bitmap cleanup未證明 |
| `CAP-PAIR-007` | 004 | WinUI 3 | Unknown | exact API `TBD` | `TBD` | `TBD` | `TBD` | Yes | No | Yes | Yes | Not evaluated | 001, 002, 009, 012 | identity incomplete | Not granted | source limitation未定 |
| `CAP-PAIR-008` | 004 | WPF | Unknown | exact API `TBD` | `TBD` | `TBD` | `TBD` | Yes | No | Yes | Yes | Not evaluated | 001, 002, 009, 012 | identity incomplete | Not granted | source limitation未定 |
| `CAP-PAIR-009` | 005 | WinUI 3 | Conditionally eligible | named primary/fallback `TBD` | union of selected devices | multiple boundaries | union of candidates | Yes | No | Yes | Yes | Blocked | 001, 002, 010, 012 | composition incomplete | Not granted | fallback contract未證明 |
| `CAP-PAIR-010` | 005 | WPF | Conditionally eligible | named primary/fallback `TBD` | union of selected devices | multiple boundaries | union of candidates | Yes | No | Yes | Yes | Blocked | 001, 002, 010, 012 | composition incomplete | Not granted | fallback contract未證明 |

## 11. Synthetic Scene Readiness

本文件不建立實際 Scene、Window、Image 或 Asset。Readiness 只描述是否具備未來可執行的規格與條件。

| Scene capability | Specification status | Asset required | Runtime dependency | Evidence dependency | Readiness | Blocking ID |
|---|---|---|---|---|---|---|
| Fixed physical canvas | Partially resolved | synthetic fixture later | C1 | source/frame metadata | Blocked | 003 |
| Fixed logical canvas | Partially resolved | synthetic fixture later | C1 | DIP/physical record | Blocked | 003, 004 |
| Color blocks | Partially resolved | synthetic fixture later | C1/C2 | color metadata | Blocked | 003, 007 |
| One-pixel borders | Partially resolved | synthetic fixture later | C1 | crop/pixel comparison | Blocked | 003, 004, 007 |
| Coordinate grid | Partially resolved | synthetic fixture later | C1 | coordinate mapping | Blocked | 004 |
| Corner markers | Partially resolved | synthetic fixture later | C1 | crop evidence | Blocked | 003, 004 |
| Center marker | Partially resolved | synthetic fixture later | C1 | source alignment | Blocked | 003 |
| Mixed-language text | Partially resolved | synthetic fixture later | C1 | content identity only | Blocked | 003 |
| Alpha gradient | Deferred | synthetic fixture later | C2 | alpha/pixel format | Deferred | 005 |
| SDR color block | Partially resolved | synthetic fixture later | C1/C2 | color/pixel metadata | Blocked | 005, 007 |
| Wide-color substitute | Deferred | lawful synthetic fixture | C2 | format/profile | Deferred | 005 |
| Cursor target | Partially resolved | synthetic target later | C2 | cursor observation | Blocked | 005 |
| Synthetic overlay window | Partially resolved | synthetic test window later | C2 | self-capture evidence | Blocked | 005, 008 |
| Occluded-window scenario | Deferred | public synthetic window | C2 | window semantics | Deferred | 005 |
| Minimized-window scenario | Deferred | public synthetic window | C2 | window semantics | Deferred | 005 |
| Negative-coordinate placement | Partially resolved | display topology later | C1 | signed coordinate record | Blocked | 004 |
| Same-DPI multi-monitor | Partially resolved | lawful topology later | C1 | monitor bounds | Blocked | 004 |
| Mixed-DPI multi-monitor | Deferred | lawful topology later | C2 | DPI mapping | Deferred | 004 |
| HDR/SDR combination | Deferred | lawful HDR branch | C2 | format/profile | Deferred | 005 |
| Protected-content substitute | Partially resolved | public substitute | C2 | refusal/boundary class | Blocked | 005, 008 |
| Display-change/device-loss trigger | Deferred | controlled public event | C3 | recovery observation | Deferred | 006 |

## 12. Coordinate Evidence Readiness

| Coordinate capability | Definition status | Required observation | Required artifact | Current readiness | Blocking ID |
|---|---|---|---|---|---|
| Virtual-screen origin | Partially resolved | signed origin and virtual bounds | session record; persistence not authorized | Blocked | 004 |
| Physical monitor bounds | Partially resolved | monitor bounds/rotation | session record; persistence not authorized | Blocked | 004 |
| DIP bounds | Partially resolved | host logical bounds and DPI context | session record; persistence not authorized | Blocked | 004 |
| Selection intent | Partially resolved | owner/unit/timestamp | session record; persistence not authorized | Blocked | 004 |
| Source frame bounds | Partially resolved | source origin/size/format | session record; persistence not authorized | Blocked | 004, 007 |
| Frame-local coordinates | Partially resolved | frame origin and row orientation | session record; persistence not authorized | Blocked | 004 |
| Crop conversion | Partially resolved | input/output rect and scale | future crop record; not authorized | Blocked | 004, 007 |
| Negative coordinates | Partially resolved | signed X/Y preservation | future mapping record; not authorized | Blocked | 004 |
| Edge semantics | Deferred | inclusive/exclusive behavior | future mapping record; not authorized | Deferred | 004 |
| Rounding rule | Deferred | actual rounding observation | future mapping record; `TBD` | Deferred | 004 |
| Timestamp correlation | Partially resolved | selection/frame/overlay times | session metadata; persistence not authorized | Blocked | 004, 008 |
| Off-by-one detection | Partially resolved | one-pixel border comparison | future diff record; not authorized | Blocked | 004, 007 |
| Topology-change behavior | Deferred | mapping invalidation/rebuild | future recovery record; not authorized | Deferred | 006 |

`Rounding rule` 未決時保持 `TBD`，不得在 readiness 文件中形成產品決策。

## 13. Evidence and Privacy Readiness

| Evidence capability | Planned method | Runtime required | File persistence required | Privacy risk | Current readiness | Blocking effect |
|---|---|---|---|---|---|---|
| Environment record | session metadata fields | Yes | No | Low if redacted | Blocked | no runtime authority |
| Frame metadata | dimensions/format/status only | Yes | No | Low | Blocked | no runtime authority |
| Coordinate mapping | signed bounds/scale/timestamps | Yes | No | Low | Blocked | coordinate blocker |
| Synthetic source reference | future fixture identity | Yes | No | Low | Blocked | scene blocker |
| Captured frame | future controlled artifact | Yes | Yes | High | Blocked | Evidence Write Authorized No |
| Crop output | future controlled artifact | Yes | Yes | High | Blocked | Evidence Write Authorized No |
| Pixel difference | metadata/diff result | Yes | Maybe | Medium | Blocked | method/threshold blocker |
| Color/pixel-format metadata | session metadata | Yes | No | Low | Deferred | HDR/C2 blocker |
| Timing | measurement fields | Yes | No | Low | Blocked | no runtime authority |
| CPU/GPU/memory observation | metadata/measurement | Yes | No | Low | Deferred | C3 blocker |
| Failure reproduction | event/status class | Yes | Maybe | Medium | Deferred | safe trigger blocker |
| Recovery observation | lifecycle/status class | Yes | Maybe | Medium | Deferred | C3 blocker |
| Diagnostic log | redacted session log | Yes | Maybe | Medium | Blocked | privacy/retention blocker |
| Privacy review | human review checklist | No | No | High if failed | Partially resolved | owner not named |
| Cleanup confirmation | release/retention record | Yes | Maybe | Medium | Partially resolved | cleanup owner not named |

明確規定：

- 沒有 Evidence Write authorization 時，不得建立 Artifact。
- Session-only observation 不等於可持久化 Evidence。
- 真實桌面內容不得用作 Evidence。
- Protected content 與 Secure Desktop 不得被規避。
- Frame acquisition 成功不能單獨關閉 coordinate、privacy 或 fidelity blocker。

## 14. Environment Readiness

| Environment requirement | Existing evidence | Status | Required phase | Deferred allowed | Affected spikes |
|---|---|---|---|---|---|
| Windows 11 x64 | target assumption only | Blocked | C1 | No | 001–012 |
| GPU/driver | official risk only | Blocked | C1/C3 | No | 001, 002, 010, 012 |
| D3D11 capability | official API identity only | Blocked | C1 | No for DXGI | 001–005, 010 |
| Single monitor | scene plan only | Blocked | C1 | No | 001 |
| Multi-monitor | scene plan only | Blocked | C1 | No | 002–005, 011 |
| Negative-coordinate layout | scene plan only | Blocked | C1 | No | 003–005, 011 |
| Same DPI | scene plan only | Blocked | C1 | No | 002, 003, 005 |
| Mixed DPI | scene plan only | Deferred | C2 | Yes until C1 | 004, 005, 011 |
| HDR disabled baseline | no local observation | Deferred | C2 | Yes until C1 | 008 |
| HDR enabled branch | no local observation | Deferred | C2 | Yes | 008 |
| Packaged | no local observation | Deferred | C2/C3 | Yes until host path | 001, 002, 011 |
| Unpackaged | no local observation | Blocked for any project run | C1 | No for first host baseline | 001–012 |
| Debug | no build | Blocked | C1 | No for experimental build | 001–012 |
| Release | no build | Deferred | C3 | Yes until baseline | 012 |
| Cold start | no runtime | Blocked | C1 | No | 001, 012 |
| Warm start | no runtime | Deferred | C3 | Yes until baseline | 012 |
| Display topology change | no trigger | Deferred | C3 | Yes until C1/C2 | 010, 012 |
| Device-loss trigger | no trigger | Deferred | C3 | Yes until baseline | 010, 012 |
| Stable power mode | no local observation | Deferred | C3 | Yes until resource phase | 012 |

本表不要求 Phase C1 等待所有 HDR、完整 deployment 或後期 resource observation，但 C1 的必要 host/project/runtime authority 仍未取得，因此整體維持未就緒。

## 15. Per-Spike Readiness Matrix

本表正好十二列；Readiness 由 Pair、Prerequisite、Environment、Evidence 與 Authorization 機械式推導。所有 `Execution authorized` 固定為 `No`。

| Spike | Required pairs | Required prerequisites | Required environment | Required evidence | Privacy condition | Readiness | Blocking IDs | Execution authorized |
|---|---|---|---|---|---|---|---|---|
| `CAP-SPIKE-001` | 001–006、basic 009/010 | 001–008、013、022–030 | single monitor、host baseline | frame metadata、environment、privacy、cleanup | synthetic only; no persistence | Blocked | 001, 002, 003, 007, 008, 010, 011 | No |
| `CAP-SPIKE-002` | 001–006、009/010 | 001–008、013–015、022 | same-DPI multi-monitor | bounds、origin、coverage | synthetic topology only | Blocked | 001, 002, 003, 004, 007, 010, 011 | No |
| `CAP-SPIKE-003` | 001–006、009/010 | 001–008、013–016、022 | negative-coordinate topology | signed coordinate mapping | synthetic topology only | Blocked | 001, 002, 003, 004, 007, 010, 011 | No |
| `CAP-SPIKE-004` | 001–010 | 001–008、013–016、022–024 | mixed-DPI monitors | DIP/physical mapping、rounding | synthetic topology only | Blocked | 001, 002, 003, 004, 007, 010, 011 | No |
| `CAP-SPIKE-005` | 001–010 | 001–008、013–016、022–024 | known crop fixture | expected/observed crop、pixel difference | frame/crop persistence not allowed | Blocked | 003, 004, 007, 009, 011 | No |
| `CAP-SPIKE-006` | 001–010 | 001–005、013、017、022、025, 030 | synthetic overlay-like window | inclusion/exclusion、timing、privacy | no formal SnipPlus Overlay | Deferred | 005, 008, 011 | No |
| `CAP-SPIKE-007` | 001–010 | 001–005、013、018、022、025 | synthetic cursor target | cursor state/metadata | no private input | Deferred | 005, 008, 011 | No |
| `CAP-SPIKE-008` | 001–006、009/010 | 001–005、013、019、022–026 | HDR/SDR lawful branch | format/profile/conversion | synthetic color only | Deferred | 005, 009, 011 | No |
| `CAP-SPIKE-009` | 001–008 | 001–005、013、020、025, 030 | lawful boundary substitute | refusal/error/privacy class | no bypass; no protected content | Deferred | 005, 008, 011 | No |
| `CAP-SPIKE-010` | 001–006、009/010 | 001–008、021–030 | controlled invalidation event | failure/recreate/cleanup | no private frames | Deferred | 006, 007, 008, 011 | No |
| `CAP-SPIKE-011` | 001–010 | 001–012、013–016、022–030 | same fixture, two hosts | host ownership/mapping/cleanup | separate host evidence | Blocked | 001, 002, 004, 007, 010, 011 | No |
| `CAP-SPIKE-012` | 001–006、009/010 | 001–008、013, 021, 022–030 | repeated synthetic cycle | timing/resource/cleanup | metadata-first; no frame persistence | Deferred | 006, 007, 009, 011 | No |

`Deferred` 只表示可排到較後 phase；它不表示已執行或具備授權。

## 16. Phase Readiness

### 16.1 C1 — Basic Capture and Coordinate Correctness

最低條件：

- 至少一個 WinUI 3 Pair 與一個 WPF Pair 的 Project/Build path 已規格化。
- 至少一個 one-shot Candidate 的 API identity 已固定。
- Synthetic basic scene 已規格化。
- Single-monitor、virtual desktop、negative coordinate、crop evidence method 已規格化。
- Project/Restore/Build authority 可明確區分。
- Runtime execution 仍由後續獨立授權管理。

| Input | Current value | Effect |
|---|---|---|
| Shared UI prerequisites | Blocked | C1 不可進入 execution review |
| Capture prerequisites | Blocked | C1 不可進入 execution review |
| Synthetic scene | Blocked | C1 不可取得可比較 source |
| Coordinate evidence | Blocked | C1 不可證明 mapping/crop |
| Project/Build authority | Not granted | 不得建立或建置實驗 project |
| Runtime authority | Not granted | 不得執行 Spike |
| Phase readiness | Not ready | 機械式推導結果 |

### 16.2 C2 — Display, Overlay and Color

另外需要：mixed DPI、overlay self-capture、cursor control、HDR/SDR observation、protected/secure boundary。

| Input | Current value | Effect |
|---|---|---|
| C1 baseline | Not ready | C2 不可假設 source/coordinate 正確 |
| Display environment | Deferred/Blocked | C2 branch 不可執行 |
| Privacy/security boundary | Partially resolved | 需具名 review/stop owner |
| Evidence write | Not granted | 不得持久化 frame/crop |
| Phase readiness | Not ready | 不可進入 execution review |

### 16.3 C3 — Recovery, Interop and Resource

另外需要：device-loss/display-change trigger、WinUI 3/WPF interoperability、timing 與 resource observation。

| Input | Current value | Effect |
|---|---|---|
| C1/C2 baseline | Not ready | C3 不可重用未驗證 source |
| Recovery trigger | Deferred | 不得觸發未授權 system/device event |
| Host interop | Blocked | pair ownership 未完成 |
| Resource measurement | Deferred | threshold 與 method 未核准 |
| Phase readiness | Not ready | 不可進入 execution review |

每個 Phase 只能使用 `Ready`、`Conditionally ready`、`Not ready`；目前三個 Phase 均為 `Not ready`。

## 17. Minimum Blocking Action Set

本節只列真正阻止最早 Phase C1 的事項，不把 C2/C3 的全部項目升格為 C1 blocker。每個 action 都是 readiness 條件，不是執行授權。

| Action ID | Blocking condition | Source prerequisites | Source blockers | Affected pairs | Affected spikes | Required evidence | Documentary or execution requirement | Mutation required | Authorization dependency | Completion condition |
|---|---|---|---|---|---|---|---|---|---|---|
| `CAP-BA-001` | Host Framework/SDK exact baseline 未固定 | 001, 002 | 001 | 001–010 | 001–005, 011 | host/version identity | Documentary plus later local inspection | No | Pending separate authorization |具名 baseline 與 host scope |
| `CAP-BA-002` | Candidate one-shot API identity 未形成 pair | 003–011 | 002 | 001–010 | 001–005 | API/interop ownership | Documentary baseline; later controlled execution | No | Pending separate authorization | 至少一組 one-shot pair 可被清楚描述 |
| `CAP-BA-003` | Project/Restore/Build scope 未取得 | 027 | 010 | 001–010 | 001–005, 011 | authorization input | Separate decision record; no action here | No | Pending separate authorization | exact project/build boundary named |
| `CAP-BA-004` | Synthetic basic scene 未具備固定規格 | 013 | 003 | 001–010 | 001–005 | scene contract/reference | Documentary then future fixture | No | Pending separate authorization | fixture identity/geometry/privacy fixed |
| `CAP-BA-005` | Coordinate/mapping/crop evidence method 未完成 | 014, 015, 016, 022, 023 | 004, 007 | 001–010 | 002–005 | mapping/edge/pixel method | Documentary readiness; future runtime evidence | No | Pending separate authorization | required domains/owners/rounding boundary fixed |
| `CAP-BA-006` | Evidence/Privacy/cleanup boundary 未具名 | 025, 026, 030 | 008, 009 | 001–010 | 001–005 | review/retention/cleanup scope | Documentary plus separate evidence decision | No | Pending separate authorization | named review/stop/cleanup scope |
| `CAP-BA-007` | Runtime execution scope 未取得 | 028 | 011 | 001–010 | 001–005, 011 | execution authorization input | Separate authorization review | No | Pending separate authorization | Spike IDs、host、stop conditions explicitly listed |

Capture-specific action 才建立新的 `CAP-BA`；共用 UI Host action 必須引用既有 UI Research，不重複建立。

## 18. Overall Readiness Decision

### 18.1 Decision vocabulary

只能使用：

- `Ready for capture runtime spike execution authorization review`
- `Conditionally ready for capture runtime spike execution authorization review`
- `Not ready`

### 18.2 Mechanical derivation

| Input | Current value | Derivation effect |
|---|---|---|
| Open shared UI blockers | Open | prevents full readiness |
| Open Capture prerequisites | Open/Blocked | prevents full readiness |
| Candidate–Host pair readiness | Blocked/Not evaluated | prevents execution review |
| Synthetic scene readiness | Blocked/Deferred | prevents comparable evidence |
| Coordinate evidence readiness | Blocked/Deferred | prevents fidelity conclusion |
| Environment readiness | Blocked/Deferred | prevents safe runtime setup |
| Evidence/privacy readiness | Blocked/Partially resolved | persistence and privacy not closed |
| Authorization status | Not granted / Pending separate authorization | execution not permitted |
| Overall Readiness Decision | `Not ready` | mechanical result |

固定狀態：

- `Build Verification: Not performed`
- `Runtime Verification: Not performed`
- `Capture Runtime Spike Authorized: No`
- `Evidence Write Authorized: No`
- `Capture Decision: Not made`
- `Rendering Decision: Not made`

依目前狀態，Overall Readiness 固定為 `Not ready`。這不是 Candidate rejection，也不是 Capture decision。

## 19. Traceability

### 19.1 Readiness chain

`CAP prerequisite / blocker` → `CAP Candidate–Host Pair` → `CAP Spike` → `Phase readiness` → `Minimum Blocking Action` → `Future execution authorization review` → `Future Capture decision`

### 19.2 Upstream references

| Upstream artifact | Relationship |
|---|---|
| `docs/Research/Technology/20-capture-backend-feasibility.md` | Candidate、criteria、gates、gaps、coordinate、privacy 與 ownership baseline。 |
| `docs/Research/Technology/21-capture-backend-runtime-spike-plan.md` | 十二個 Spike、scene、evidence、phase、stop 與 artifact plan。 |
| `docs/Research/Technology/01-ui-framework-feasibility.md` | UI host、overlay、multi-monitor、DPI 與 input boundary。 |
| `docs/Research/Technology/07-ui-framework-runtime-validation-plan.md` | UI host runtime evidence 的上游 plan boundary。 |
| `docs/Research/Technology/08-ui-framework-runtime-validation-execution-readiness.md` | UI execution readiness 的治理邊界；不由本文件覆蓋。 |
| `docs/Research/Technology/09-ui-framework-runtime-validation-authorization-request.md` | UI authorization request 的治理邊界；不由本文件代替。 |
| `docs/Research/Technology/12-rendering-technology-runtime-spike-execution-readiness.md` | Rendering readiness 的 separation 與 execution boundary。 |
| `Architecture/adr/ADR-0002-ui-framework-selection.md` | UI Framework decision 仍 Draft。 |
| `Architecture/TECHNOLOGY-DECISION-ROADMAP.md` | `TD-003` Capture Backend roadmap。 |
| Frozen PRD、Specs、Architecture | product requirement、workflow、platform、privacy、failure traceability。 |

實際名稱與 ID 必須從 Repository 原樣引用；本文件不創造不存在的上游證據。

## 完成條件

- 只建立 `22-capture-backend-runtime-spike-execution-readiness.md`。
- 不修改任何其他文件。
- 建立 Capture prerequisite 與 blocker registers。
- 建立 Candidate identity baseline。
- 覆蓋十個 Candidate–Host Pair。
- 建立 Synthetic Scene、Coordinate、Evidence/Privacy 與 Environment readiness。
- 建立正好十二列 Per-Spike Readiness。
- 建立 Phase C1–C3 readiness。
- 建立最小 `CAP-BA` 集合。
- Overall Readiness 必須由矩陣機械式推導。
- 所有 Spike execution authorization 為 `No`。
- 不建立 Project、Prototype、Result directory、Source Code、Capture Frame 或 Evidence。
- 不執行 Capture API、Screenshot、Recording、下載、安裝、Restore、Build、Run 或 Runtime Spike。
- 不建立 Capture ADR。
- 不修改 UI/Rendering Research Line。
- `git diff --check` 通過。

完成後停止；任何後續執行都必須先取得獨立且具名的 execution authorization。

