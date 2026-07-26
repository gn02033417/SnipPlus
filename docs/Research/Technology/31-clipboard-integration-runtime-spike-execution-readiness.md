# Clipboard Integration Runtime Spike Execution Readiness

| Field | Value |
|---|---|
| Document ID | RESEARCH-TECH-CLIPBOARD-003 |
| Title | Clipboard Integration Runtime Spike Execution Readiness |
| Status | Draft |
| Research Type | Runtime Execution Readiness |
| Technology Decision | `TD-004` — Clipboard Integration |
| Parent Runtime Plan | `RESEARCH-TECH-CLIPBOARD-002` |
| Parent Feasibility | `RESEARCH-TECH-CLIPBOARD-001` |
| Execution Status | Not started |
| Local Clipboard Inspection | Not performed |
| Build Verification | Not performed |
| Runtime Verification | Not performed |
| Clipboard Runtime Spike Authorized | No |
| Clipboard Read Authorized | No |
| Clipboard Write Authorized | No |
| Clipboard Clear Authorized | No |
| Evidence Write Authorized | No |
| UI Framework Decision | Unresolved — `ADR-0002` remains Draft |
| Rendering Decision | Not made |
| Capture Decision | Not made |
| Clipboard Decision | Not made |
| Owner | TBD |
| Last reviewed | Not reviewed |
| Readiness date | 2026-07-26 |

本文件只評估執行 `CLIP-SPIKE-001..012` 前的前置條件與阻塞狀態，不執行任何 Spike。它不授權 Clipboard 讀寫、不建立實驗 Project、不建立 payload、不產生 Evidence Artifact，也不開始正式 Clipboard 或截圖功能。

## 1. Purpose

本文件只回答：

> 執行 `CLIP-SPIKE-001..012` 前，哪些 Host、Clipboard API、Project、Build、隔離環境、Synthetic Image、Format、Consumer、Threading、Privacy、Cleanup 及授權條件必須具備；目前各 Candidate–Host Pair 及各 Spike 是否可執行？

這是 Execution Readiness Assessment，不是：

- Runtime Spike Execution。
- Authorization Request。
- Clipboard 讀寫紀錄。
- Clipboard Technology Decision。
- Clipboard ADR。
- 正式 Clipboard 功能。

本文件的結論只能描述 readiness、blocker、缺少的 evidence 與待授權事項，不得把文件完成誤寫成 runtime ready、production ready 或 technology selected。

## 2. Scope

只評估：

- `CLIP-OPT-001..005`。
- `CLIP-PAIR-001..010`。
- `CLIP-001..022`。
- `CLIP-GATE-001..010`。
- `CLIP-GAP-001..018`。
- `CLIP-SPIKE-001..012`。
- WPF 與 WinUI 3 Host。
- Clipboard API／Interop prerequisite。
- Experimental Project／Restore／Build prerequisite。
- Clipboard isolation environment。
- Synthetic Image readiness。
- Clipboard Format 及 Consumer readiness。
- STA／COM／Dispatcher readiness。
- Contention／Retry observation readiness。
- Ownership／Process lifetime readiness。
- History／Cloud Clipboard boundary。
- Evidence、Privacy 及 Cleanup readiness。
- Clipboard 與 File Output 平行獨立性。
- Clipboard Read／Write／Clear／Runtime／Evidence 權限。

所有判斷都以 `RESEARCH-TECH-CLIPBOARD-002` 的 runtime plan、`RESEARCH-TECH-CLIPBOARD-001` 的 feasibility baseline、Repository 內實際 UI／Capture／Rendering Research 與 frozen PRD／Specs 為依據。

## 3. Non-goals

不得：

- 讀取、寫入、清除或備份 Windows Clipboard。
- 執行任何 Runtime Spike。
- 建立 Project、Solution、Prototype 或 Source Code。
- 建立 Bitmap、DIB、DIBV5、PNG 或 Clipboard payload。
- 建立 Result directory 或 Evidence Artifact。
- 執行下載、安裝、Restore、Build、Run、Publish 或 Test。
- 修改 Clipboard History 或 Cloud Clipboard 設定。
- 修改 UI／Capture／Rendering Research Line。
- 修改 `ADR-0002`。
- 建立 Clipboard ADR。
- 選擇 Clipboard Technology。
- 開始正式 Clipboard 或截圖功能。

## 4. Controlled Vocabulary

### 4.1 Prerequisite Status

只能使用：

- `Resolved`
- `Partially resolved`
- `Blocked`
- `Deferred`
- `Not applicable`

`Resolved` 只可在具有足夠、可追溯、與該 prerequisite 直接相關的 evidence 後使用。官方文件存在不等於 local、build 或 runtime prerequisite 已 Resolved。

### 4.2 Candidate–Host Readiness

只能使用：

- `Ready`
- `Conditionally ready`
- `Blocked`
- `Excluded with evidence`
- `Not evaluated`

本文件不得因 API 存在而標示 `Ready`。

### 4.3 Spike Readiness

只能使用：

- `Ready`
- `Blocked`
- `Deferred`
- `Not applicable`

所有目前 Spike 的 readiness 都由 prerequisite、pair、isolation、evidence 與 authorization 矩陣推導，不能由 runtime plan 完成度直接推導為 `Ready`。

### 4.4 Authorization Status

只能使用：

- `Not granted`
- `Pending separate authorization`

不得使用：

- `Approved`
- `Authorized`
- `Executed`
- `Passed`

## 5. Dependency Classification

| Class | 說明 |
|---|---|
| Shared UI-host dependency | WPF／WinUI 3、SDK、Build Tool、Project isolation |
| Clipboard-candidate dependency | Framework Clipboard、WinRT、OLE、Raw Win32、Adapter |
| Clipboard-isolation dependency | 不含私人 Clipboard 資料的隔離環境 |
| Synthetic-image dependency | 固定且無敏感資料的測試影像 |
| Format／consumer dependency | Clipboard Format 及貼上端互通驗證 |
| Threading／COM dependency | STA、COM、Dispatcher 及 Process lifetime |
| Platform-state dependency | History、Cloud Clipboard 及封裝模式 |
| Evidence／privacy dependency | Result、Log、Pixel comparison 及敏感資料控制 |
| Authorization dependency | Project、Restore、Build、Clipboard 操作、Runtime 及 Evidence |

規則：

- Shared UI Host blocker 必須引用既有 UI Research。
- Clipboard Research 不得重新建立 Shared UI 授權。
- Build authority 不代表 Run authority。
- Runtime authority 不代表 Clipboard Read／Write／Clear authority。
- Clipboard Write authority 不代表 Evidence persistence authority。
- Clipboard Clear 必須保持獨立且預設不授權。

## 6. Shared Research Dependency Matrix

| Clipboard requirement | Source research item | Current status | Reusable evidence | Remaining Clipboard condition |
|---|---|---|---|---|
| Windows 11 x64 baseline | `docs/Research/Technology/20-capture-backend-feasibility.md` / `RESEARCH-TECH-CAPTURE-001`；`docs/Research/Technology/29-clipboard-integration-feasibility.md` / `RESEARCH-TECH-CLIPBOARD-001` | Partially resolved | Windows-first desktop boundary and official Clipboard baseline | Local OS、architecture、Host 與 Clipboard runtime 尚未驗證 |
| WinUI 3 experimental build path | `docs/Research/Technology/01-ui-framework-feasibility.md` / `RESEARCH-TECH-UI-001`；`Specs/SPEC-0003-system-requirements.md` | Partially resolved | WinUI 3 is a research candidate；ADR-0002 remains Draft | Experimental Project、SDK、Restore、Build 與 runtime identity 未建立 |
| WPF experimental build path | `docs/Research/Technology/01-ui-framework-feasibility.md` / `RESEARCH-TECH-UI-001`；`Specs/SPEC-0003-system-requirements.md` | Partially resolved | WPF is a research candidate；ADR-0002 remains Draft | Experimental Project、SDK、Restore、Build 與 runtime identity 未建立 |
| .NET 及 Windows SDK | `RESEARCH-TECH-UI-001`、`RESEARCH-TECH-CAPTURE-001` | Partially resolved | Research target context | Local installed versions and compatible project target 未檢查 |
| Windows App SDK | `RESEARCH-TECH-UI-001` | Partially resolved | WinUI 3 feasibility context | Version、package 與 local availability 未驗證 |
| Experimental Project isolation | `RESEARCH-TECH-CLIPBOARD-002` | Blocked | Runtime plan defines future artifact boundary | Project 尚未授權或建立 |
| Package Restore | AGENTS.md build boundary；`RESEARCH-TECH-CLIPBOARD-002` | Blocked | Restore is separately controlled | Restore authority 未授權，未執行 |
| Build Tool | `RESEARCH-TECH-UI-001`、`ADR-0002` | Blocked | Build is a separate prerequisite | Build Tool、target 與 authority 未驗證 |
| Packaged／unpackaged mode | `ADR-0002`、`RESEARCH-TECH-CLIPBOARD-002` | Partially resolved | Runtime plan separates both modes | Actual package mode and pair evidence 未存在 |
| Evidence root policy | `RESEARCH-TECH-CLIPBOARD-002` | Blocked | Future result path is only planned | Evidence Write authority 未授權；Result directory 不得建立 |
| Privacy／cleanup policy | `RESEARCH-TECH-CLIPBOARD-001`、`RESEARCH-TECH-CLIPBOARD-002` | Partially resolved | Synthetic-only、no user Clipboard mutation boundary | Isolated environment and cleanup evidence 未驗證 |
| Shared Project／Restore／Build authority | AGENTS.md、`Architecture/ARCH-0001-architecture-principles.md` | Blocked | No build/run by default | 本文件不授權任何 Project／Restore／Build 操作 |
| Runtime authority | `RESEARCH-TECH-CLIPBOARD-002` | Blocked | Runtime plan records authority as No | Runtime authority 未授權 |

本矩陣只表示 documentation readiness。任何 `Partially resolved` 都不能作為 Runtime 執行許可。

## 7. Clipboard Prerequisite Register

建立連續 ID：`CLIP-PREQ-001..032`。每項固定包含完整 prerequisite fields；`Current status` 只反映 readiness，不表示已執行。

| Prerequisite ID | Description | Dependency class | Related candidates | Related hosts | Related pairs | Related spikes | Related criteria／gates | Existing evidence | Current status | Required final evidence | Project | Restore | Build | Clipboard read | Clipboard write | Clipboard clear | Runtime | Evidence write | Network | Authorization | Resolution condition | Owner | Open questions |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| CLIP-PREQ-001 | WPF experimental Host identity | Shared UI-host dependency | OPT-001, OPT-005 | WPF | PAIR-001, 005, 009 | SPIKE-001, 003..012 | CLIP-002, 006, GATE-006 | RESEARCH-TECH-UI-001 | Blocked | Project target、Host version、launch context | Yes | No | Yes | No | No | No | No | No | No | Pending | Local Host identity is documented and reviewable | UI owner TBD | Which WPF target is allowed? |
| CLIP-PREQ-002 | WinUI 3 experimental Host identity | Shared UI-host dependency | OPT-002, OPT-005 | WinUI 3 | PAIR-002, 006, 010 | SPIKE-002..012 | CLIP-003, 004, 006, GATE-006 | RESEARCH-TECH-UI-001 | Blocked | Project target、Windows App SDK、package context | Yes | No | Yes | No | No | No | No | No | No | Pending | Local Host identity is documented and reviewable | UI owner TBD | Which WinUI 3 target is allowed? |
| CLIP-PREQ-003 | .NET／Windows SDK baseline | Shared UI-host dependency | OPT-001..005 | WPF, WinUI 3 | PAIR-001..010 | SPIKE-001..012 | CLIP-001, 003, 004, GATE-006 | RESEARCH-TECH-UI-001 | Blocked | Installed and target versions with compatibility evidence | Yes | Yes | Yes | No | No | No | No | No | No | Pending | Version baseline is recorded without modifying machine state | Build owner TBD | Target framework and SDK alignment |
| CLIP-PREQ-004 | WPF Clipboard／`IDataObject` identity | Clipboard-candidate dependency | OPT-001, OPT-005 | WPF | PAIR-001, 005, 009 | SPIKE-001, 005..009 | CLIP-006, 010, 017, GATE-001, 005 | RESEARCH-TECH-CLIPBOARD-001 | Partially resolved | API identity, package reference and runtime route evidence | Yes | Yes | Yes | No | Yes | No | Yes | Yes | No | Pending | Candidate-specific route is separately reviewable | Clipboard owner TBD | Host adapter boundary |
| CLIP-PREQ-005 | WinRT Clipboard／`DataPackage` identity | Clipboard-candidate dependency | OPT-002, OPT-005 | WinUI 3 | PAIR-002, 006, 010 | SPIKE-002, 005..010 | CLIP-003, 006, 017, 019, GATE-001, 005, 008 | RESEARCH-TECH-CLIPBOARD-001 | Partially resolved | API identity, projection, package context and consumer evidence | Yes | Yes | Yes | No | Yes | No | Yes | Yes | No | Pending | WinRT route and package boundary are reviewable | Clipboard owner TBD | Representation and lifetime |
| CLIP-PREQ-006 | OLE Clipboard／COM `IDataObject` identity | Clipboard-candidate dependency | OPT-003, OPT-005 | WPF, WinUI 3, Win32／OLE | PAIR-003, 007 | SPIKE-003, 005..008, 011, 012 | CLIP-007, 013, 017, 018, GATE-003, 005 | RESEARCH-TECH-CLIPBOARD-001 | Partially resolved | COM initialization、ownership、format and lifetime evidence | Yes | Yes | Yes | No | Yes | No | Yes | Yes | No | Pending | OLE route has explicit COM／STA contract | Clipboard owner TBD | Owner and process exit |
| CLIP-PREQ-007 | Raw Win32 Clipboard API identity | Clipboard-candidate dependency | OPT-004, OPT-005 | WPF, WinUI 3, Win32／OLE | PAIR-004, 008 | SPIKE-003..009, 011, 012 | CLIP-006, 007, 008, 017, GATE-001, 005 | RESEARCH-TECH-CLIPBOARD-001 | Partially resolved | Native API、format、handle and cleanup evidence | Yes | Yes | Yes | No | Yes | No | Yes | Yes | No | Pending | Native route is isolated and separately reviewable | Clipboard owner TBD | Format conversion and cleanup |
| CLIP-PREQ-008 | Host-neutral Adapter composition boundary | Clipboard-candidate dependency | OPT-005 | WPF, WinUI 3 | PAIR-009, 010 | SPIKE-001..012 | CLIP-003, 006, 009, 017, GATE-006, 009 | ARCH-0003、ARCH-0004、RESEARCH-TECH-CLIPBOARD-001 | Partially resolved | Separate WPF／WinUI／native adapter evidence without shared workflow ownership | Yes | Yes | Yes | No | Yes | No | Yes | Yes | No | Pending | Adapter boundary is documented and independently testable | Architecture owner TBD | Adapter composition without API selection |
| CLIP-PREQ-009 | WPF STA／Dispatcher requirement | Threading／COM dependency | OPT-001, OPT-005 | WPF | PAIR-001, 005, 009 | SPIKE-001, 006..008, 011, 012 | CLIP-013, 014, GATE-003, 004 | RESEARCH-TECH-CLIPBOARD-001、RESEARCH-TECH-CLIPBOARD-002 | Partially resolved | UI STA、Dispatcher、marshal and shutdown observations | Yes | Yes | Yes | No | Yes | No | Yes | Yes | No | Pending | Host thread contract is observed in isolation | UI owner TBD | Background STA／MTA behavior |
| CLIP-PREQ-010 | WinUI 3 Dispatcher／Apartment requirement | Threading／COM dependency | OPT-002, OPT-005 | WinUI 3 | PAIR-002, 006, 010 | SPIKE-002, 006..008, 011, 012 | CLIP-003, 004, 013, 014, GATE-003, 006 | RESEARCH-TECH-CLIPBOARD-001、RESEARCH-TECH-CLIPBOARD-002 | Partially resolved | UI／background route、dispatcher、package and shutdown evidence | Yes | Yes | Yes | No | Yes | No | Yes | Yes | No | Pending | Host thread contract is observed in isolation | UI owner TBD | Projection and dispatcher boundary |
| CLIP-PREQ-011 | COM initialization responsibility | Threading／COM dependency | OPT-003, OPT-004, OPT-005 | WPF, WinUI 3 | PAIR-003, 004, 007, 008, 009, 010 | SPIKE-003, 006..008, 011 | CLIP-013, 017, GATE-003, 005 | Official baseline in RESEARCH-TECH-CLIPBOARD-001 | Partially resolved | COM state、HRESULT／exception、cleanup and cancellation evidence | Yes | Yes | Yes | No | Yes | No | Yes | Yes | No | Pending | COM boundary is explicit for each native route | Clipboard owner TBD | Behavior without required initialization |
| CLIP-PREQ-012 | Packaged／unpackaged requirements | Platform-state dependency | OPT-001..005 | WPF, WinUI 3 | PAIR-001..010 | SPIKE-001, 002, 005, 008, 010, 011 | CLIP-004, 005, 013, 017, 019, GATE-006, 007, 008 | ADR-0002、RESEARCH-TECH-CLIPBOARD-002 | Partially resolved | Same pair compared in both modes with environment evidence | Yes | Yes | Yes | No | Yes | No | Yes | Yes | No | Pending | Actual package mode and identity are recorded | Build owner TBD | Packaging impact |
| CLIP-PREQ-013 | Clipboard isolation environment | Clipboard-isolation dependency | OPT-001..005 | WPF, WinUI 3 | PAIR-001..010 | SPIKE-001..012 | CLIP-022, GATE-010 | RESEARCH-TECH-CLIPBOARD-001、002 | Blocked | Isolated user／VM/session and pre/post cleanup evidence | Yes | No | No | No | Yes | No | Yes | Yes | No | Pending | Isolation is approved and demonstrably free of private data | Privacy owner TBD | Which isolation mode is allowed? |
| CLIP-PREQ-014 | Existing Clipboard data policy | Clipboard-isolation dependency | OPT-001..005 | WPF, WinUI 3 | PAIR-001..010 | SPIKE-001..012 | CLIP-021, 022, GATE-008, 010 | RESEARCH-TECH-CLIPBOARD-001、002 | Blocked | Written policy that does not read／backup／clear user Clipboard | No | No | No | No | No | No | No | No | No | Pending | Isolation policy is accepted without user data access | Privacy owner TBD | How to prove no private content? |
| CLIP-PREQ-015 | Clipboard Read／Write／Clear permission separation | Authorization dependency | OPT-001..005 | WPF, WinUI 3 | PAIR-001..010 | SPIKE-001..012 | CLIP-021, 022, GATE-009, 010 | RESEARCH-TECH-CLIPBOARD-001、002 | Blocked | Separate signed authority for each operation | No | No | No | No | No | No | No | No | No | Pending | Each permission is independently explicit | Product owner TBD | Clear remains prohibited by default |
| CLIP-PREQ-016 | Synthetic Image specification | Synthetic-image dependency | OPT-001..005 | WPF, WinUI 3 | PAIR-001..010 | SPIKE-001..012 | CLIP-010, 011, 012, 022, GATE-002, 010 | RESEARCH-TECH-CLIPBOARD-002 | Blocked | Approved fixed dimensions、markers、alpha、color metadata and identity | No | No | No | No | No | No | No | Yes | No | Pending | Specification approved; image creation separately authorized | Evidence owner TBD | Size classes and color substitute |
| CLIP-PREQ-017 | Framework Bitmap representation | Clipboard-candidate dependency | OPT-001, OPT-002, OPT-005 | WPF, WinUI 3 | PAIR-001, 002, 009, 010 | SPIKE-001, 002, 005 | CLIP-006, 010, 011, GATE-001, 002 | RESEARCH-TECH-CLIPBOARD-001、002 | Partially resolved | Producer／consumer representation and alpha／pixel comparison | Yes | Yes | Yes | No | Yes | No | Yes | Yes | No | Pending | Representation is fixed only for a scoped Spike | Clipboard owner TBD | Cross-host conversion |
| CLIP-PREQ-018 | `CF_BITMAP` representation | Clipboard-candidate dependency | OPT-003, OPT-004 | WPF, WinUI 3, Win32／OLE | PAIR-003, 004, 007, 008 | SPIKE-003, 005 | CLIP-006, 007, 010, 011, GATE-001, 002 | RESEARCH-TECH-CLIPBOARD-001、002 | Partially resolved | Format enumeration、Alpha limitation、handle and consumer evidence | Yes | Yes | Yes | No | Yes | No | Yes | Yes | No | Pending | Format is isolated and no Alpha assumption is made | Clipboard owner TBD | Alpha fidelity |
| CLIP-PREQ-019 | `CF_DIB` representation | Clipboard-candidate dependency | OPT-003, OPT-004 | WPF, WinUI 3, Win32／OLE | PAIR-003, 004, 007, 008 | SPIKE-003, 005 | CLIP-006, 007, 010, 011, 012, GATE-001, 002 | RESEARCH-TECH-CLIPBOARD-001、002 | Partially resolved | Header、stride、mask、pixel、Alpha and consumer evidence | Yes | Yes | Yes | No | Yes | No | Yes | Yes | No | Pending | DIB fields and consumer interpretation are recorded | Clipboard owner TBD | Alpha and color responsibility |
| CLIP-PREQ-020 | `CF_DIBV5` representation | Clipboard-candidate dependency | OPT-003, OPT-004 | WPF, WinUI 3, Win32／OLE | PAIR-003, 004, 007, 008 | SPIKE-003, 005 | CLIP-006, 007, 010, 011, 012, GATE-001, 002 | RESEARCH-TECH-CLIPBOARD-001、002 | Partially resolved | Header、channel mask、color metadata and consumer evidence | Yes | Yes | Yes | No | Yes | No | Yes | Yes | No | Pending | DIBV5 fields and fidelity are separately observed | Clipboard owner TBD | Color profile and mask semantics |
| CLIP-PREQ-021 | PNG registered-format representation | Clipboard-candidate dependency | OPT-003, OPT-004, OPT-005 | WPF, WinUI 3, Win32／OLE | PAIR-003, 004, 007, 008, 009, 010 | SPIKE-004, 005 | CLIP-006, 008, 010, 011, 012, GATE-001, 002 | RESEARCH-TECH-CLIPBOARD-001、002 | Partially resolved | Stream metadata、decoder identity、decoded pixel／Alpha evidence | Yes | Yes | Yes | No | Yes | No | Yes | Yes | No | Pending | PNG stream and decoded Bitmap remain separate | Clipboard owner TBD | Registered format and decoder behavior |
| CLIP-PREQ-022 | Multi-format publication boundary | Clipboard-candidate dependency | OPT-001..005 | WPF, WinUI 3, Win32／OLE | PAIR-001..010 | SPIKE-005, 008, 010 | CLIP-009, 018, 019, GATE-001, 005, 008 | RESEARCH-TECH-CLIPBOARD-001、002 | Partially resolved | Enumeration、atomicity、consumer selection and lifetime evidence | Yes | Yes | Yes | No | Yes | No | Yes | Yes | No | Pending | Each representation and publication boundary is separately observable | Clipboard owner TBD | Atomicity and fallback |
| CLIP-PREQ-023 | Alpha／premultiplication boundary | Format／consumer dependency | OPT-001..005 | WPF, WinUI 3, Win32／OLE | PAIR-001..010 | SPIKE-001..005 | CLIP-010, 011, GATE-002, 010 | RESEARCH-TECH-CLIPBOARD-001、002 | Blocked | Known alpha markers, mode, conversion and decoded comparison | No | No | No | No | No | No | Yes | Yes | No | Pending | Alpha responsibility is explicit per representation | Rendering owner TBD | Premultiplied versus straight |
| CLIP-PREQ-024 | Pixel format and color metadata | Format／consumer dependency | OPT-001..005 | WPF, WinUI 3, Win32／OLE | PAIR-001..010 | SPIKE-001..005, 009 | CLIP-011, 012, GATE-002, 010 | RESEARCH-TECH-CLIPBOARD-001、002 | Blocked | Pixel comparison、stride、color metadata and SDR／wide-color boundary | No | No | No | No | No | No | Yes | Yes | No | Pending | Format responsibility and measurement method are approved | Rendering owner TBD | HDR-to-SDR responsibility |
| CLIP-PREQ-025 | Consumer interoperability method | Format／consumer dependency | OPT-001..005 | WPF, WinUI 3, Win32／OLE | PAIR-001..010 | SPIKE-001..005, 010 | CLIP-006, 009, 010, 022, GATE-001, 010 | RESEARCH-TECH-CLIPBOARD-002 | Blocked | Isolated consumer identity、observation method and privacy boundary | No | No | No | No | No | No | Yes | Yes | No | Pending | Consumer route is approved without real user applications | QA owner TBD | Third-party observation boundary |
| CLIP-PREQ-026 | Contention creation and observation method | Threading／COM dependency | OPT-001..005 | WPF, WinUI 3, Win32／OLE | PAIR-001..010 | SPIKE-007, 012 | CLIP-015, 016, 021, GATE-004, 009 | RESEARCH-TECH-CLIPBOARD-002 | Blocked | Safe synthetic contention source、initial failure、attempt and cleanup evidence | No | No | No | No | Yes | No | Yes | Yes | No | Pending | Contention is reproducible without user Clipboard access | Clipboard owner TBD | Safe contention source |
| CLIP-PREQ-027 | Retry observation method | Threading／COM dependency | OPT-001..005 | WPF, WinUI 3, Win32／OLE | PAIR-001..010 | SPIKE-007, 012 | CLIP-015, 016, 021, GATE-004, 009 | RESEARCH-TECH-CLIPBOARD-002 | Blocked | Attempt count、interval、timeout、cancel、owner and final result evidence | No | No | No | No | Yes | No | Yes | Yes | No | Pending | Retry is bounded and does not rerun Capture／Rendering | Clipboard owner TBD | Threshold、interval、timeout remain TBD |
| CLIP-PREQ-028 | Ownership／Process lifetime method | Threading／COM dependency | OPT-001..005 | WPF, WinUI 3, Win32／OLE | PAIR-001..010 | SPIKE-008, 010 | CLIP-017, 018, 019, GATE-005, 008 | RESEARCH-TECH-CLIPBOARD-002 | Blocked | Immediate／delayed、normal／abnormal exit、consumer timing、cleanup evidence | No | No | No | No | Yes | No | Yes | Yes | No | Pending | Owner and lifetime boundary can be independently observed | Clipboard owner TBD | Process exit and delayed rendering |
| CLIP-PREQ-029 | Large-image memory observation | Evidence／privacy dependency | OPT-001..005 | WPF, WinUI 3, Win32／OLE | PAIR-001..010 | SPIKE-009 | CLIP-010, 011, 012, 020, 021, GATE-002, 005, 010 | RESEARCH-TECH-CLIPBOARD-002 | Blocked | Size classes、representation size、peak memory、failure and cleanup evidence | No | No | No | No | Yes | No | Yes | Yes | No | Pending | Memory boundary is isolated and synthetic | Performance owner TBD | Product thresholds remain TBD |
| CLIP-PREQ-030 | Clipboard History／Cloud isolation | Platform-state dependency | OPT-001..005 | WPF, WinUI 3, Win32／OLE | PAIR-001..010 | SPIKE-010 | CLIP-019, 020, 021, 022, GATE-008, 010 | RESEARCH-TECH-CLIPBOARD-001、002 | Blocked | Isolated state observation、privacy review、no setting mutation and cleanup | No | No | No | No | Yes | No | Yes | Yes | No | Pending | History／Cloud branch is isolated and separately authorized | Privacy owner TBD | Account、device and sync boundary |
| CLIP-PREQ-031 | Evidence、Privacy 及 Cleanup method | Evidence／privacy dependency | OPT-001..005 | WPF, WinUI 3, Win32／OLE | PAIR-001..010 | SPIKE-001..012 | CLIP-021, 022, GATE-005, 008, 010 | RESEARCH-TECH-CLIPBOARD-002 | Blocked | Artifact schema、privacy review、cleanup confirmation and no payload logging | No | No | No | No | No | No | Yes | Yes | No | Pending | Evidence Write separately authorized and cleanup reproducible | Evidence owner TBD | Session observation versus persistent evidence |
| CLIP-PREQ-032 | Project／Restore／Build／Clipboard／Runtime／Evidence authorization | Authorization dependency | OPT-001..005 | WPF, WinUI 3 | PAIR-001..010 | SPIKE-001..012 | CLIP-001..022, GATE-001..010 | AGENTS.md、RESEARCH-TECH-CLIPBOARD-002 | Blocked | Separate authority records for Project、Restore、Build、Clipboard、Runtime、Evidence | No | No | No | No | No | No | No | No | No | Pending | All authorities are separately granted and scoped | Product owner TBD | Which authority is granted first? |

## 8. Blocker Register

建立 `CLIP-BLOCK-001` 起的連續 blocker register。`Open` 代表 readiness blocker 尚未解除，不代表技術不支援。

| Blocker ID | Source prerequisite | Description | Severity | Affected candidates | Affected hosts | Affected pairs | Affected spikes | Affected phase | Required resolution | Required evidence | Shared dependency | Authorization dependency | Owner | Status |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| CLIP-BLOCK-001 | CLIP-PREQ-001, 002 | UI Framework decision and experimental Host identity unresolved | Blocking | OPT-001..005 | WPF, WinUI 3 | PAIR-001..010 | SPIKE-001..012 | L1-L3 | Resolve or explicitly bound the UI Host research dependency | RESEARCH-TECH-UI-001 and future Host identity evidence | Shared UI host | Project、Build、Runtime | UI owner TBD | Open |
| CLIP-BLOCK-002 | CLIP-PREQ-003 | .NET／Windows SDK baseline not locally verified | Blocking | OPT-001..005 | WPF, WinUI 3 | PAIR-001..010 | SPIKE-001..012 | L1-L3 | Record compatible target baseline | Environment and project target record | Shared UI host | Project、Build | Build owner TBD | Open |
| CLIP-BLOCK-003 | CLIP-PREQ-004..008 | Candidate API／Interop route exists in docs but local experimental identity is not established | Blocking | OPT-001..005 | WPF, WinUI 3 | PAIR-001..010 | SPIKE-001..012 | L1-L3 | Bind one or more candidate routes to isolated experimental identities | API identity, adapter boundary and project evidence | Clipboard candidate | Project、Build、Runtime | Clipboard owner TBD | Open |
| CLIP-BLOCK-004 | CLIP-PREQ-009..011 | STA／COM／Dispatcher contract has no runtime evidence | Blocking | OPT-001..005 | WPF, WinUI 3 | PAIR-001..010 | SPIKE-001, 002, 006..012 | L1-L3 | Define approved observation route | Thread／Apartment／Dispatcher evidence | Threading／COM | Runtime、Clipboard write | Clipboard owner TBD | Open |
| CLIP-BLOCK-005 | CLIP-PREQ-012 | Packaged／unpackaged pair context not locally verified | Blocking | OPT-001..005 | WPF, WinUI 3 | PAIR-001..010 | SPIKE-001, 002, 005, 008, 010, 011 | L1-L3 | Define both package modes and controlled comparison | Package state and environment record | Platform state | Project、Build、Runtime | Build owner TBD | Open |
| CLIP-BLOCK-006 | CLIP-PREQ-013..015 | Clipboard isolation and operation permission separation not authorized | Blocking | OPT-001..005 | WPF, WinUI 3 | PAIR-001..010 | SPIKE-001..012 | L1-L3 | Approve isolated environment and separate Read／Write／Clear authority | Isolation policy and authority record | Clipboard isolation | Clipboard operation、Runtime | Privacy owner TBD | Open |
| CLIP-BLOCK-007 | CLIP-PREQ-016 | Synthetic Image contract is planned but no approved runtime input exists | Blocking | OPT-001..005 | WPF, WinUI 3 | PAIR-001..010 | SPIKE-001..012 | L1-L3 | Approve specification and later input creation authority | Synthetic specification and identity | Synthetic image | Evidence、Runtime | Evidence owner TBD | Open |
| CLIP-BLOCK-008 | CLIP-PREQ-017..022 | Format and multi-format publication evidence is absent | Blocking | OPT-001..005 | WPF, WinUI 3 | PAIR-001..010 | SPIKE-001..005 | L1 | Define scoped format／consumer observation | Format, consumer and ownership evidence | Format／consumer | Clipboard write、Runtime | Clipboard owner TBD | Open |
| CLIP-BLOCK-009 | CLIP-PREQ-023..025 | Alpha、pixel、color and consumer fidelity methods are not runtime-ready | Blocking | OPT-001..005 | WPF, WinUI 3 | PAIR-001..010 | SPIKE-001..005, 009 | L1-L2 | Approve measurement and privacy-safe consumer route | Pixel／Alpha／color comparison and consumer evidence | Format／consumer | Runtime、Evidence write | Rendering owner TBD | Open |
| CLIP-BLOCK-010 | CLIP-PREQ-026..029 | Contention、retry、lifetime and memory observation methods are not executable | Blocking | OPT-001..005 | WPF, WinUI 3 | PAIR-001..010 | SPIKE-006..009, 012 | L2-L3 | Define safe synthetic observation and bounded conditions | Timing、ownership、memory and cleanup evidence | Threading／COM | Runtime、Evidence write | Clipboard owner TBD | Open |
| CLIP-BLOCK-011 | CLIP-PREQ-030 | History／Cloud isolation is not available and settings must not be modified | Blocking | OPT-001..005 | WPF, WinUI 3 | PAIR-001..010 | SPIKE-010 | L3 | Establish isolated branch or defer without changing user settings | State observation and privacy review | Platform state | Runtime、Account／device authority | Privacy owner TBD | Open |
| CLIP-BLOCK-012 | CLIP-PREQ-031 | Evidence persistence and cleanup confirmation are not authorized | Blocking | OPT-001..005 | WPF, WinUI 3 | PAIR-001..010 | SPIKE-001..012 | L1-L3 | Define evidence root and cleanup authority | Evidence schema, privacy review, cleanup record | Evidence／privacy | Evidence write、Runtime | Evidence owner TBD | Open |
| CLIP-BLOCK-013 | CLIP-PREQ-032 | Project／Restore／Build／Runtime／Evidence authority remains ungranted | Blocking | OPT-001..005 | WPF, WinUI 3 | PAIR-001..010 | SPIKE-001..012 | L1-L3 | Obtain separately scoped authority; not granted by this document | Explicit authority record | Authorization | All listed operations | Product owner TBD | Open |

不得因 Runtime Plan 完成而關閉任何 blocker。Blocker resolution 必須由相應 evidence 或獨立授權審查完成。

## 9. Candidate Identity and Local Availability Baseline

| Candidate | API／Interop identity | Host | Experimental identity | Official evidence | Local availability | Build verified | Runtime verified | Status |
|---|---|---|---|---|---|---|---|---|
| CLIP-OPT-001 | WPF `System.Windows.Clipboard`／`IDataObject` | WPF | Not established | RESEARCH-TECH-CLIPBOARD-001 | Unknown | No | No | Blocked |
| CLIP-OPT-002 | WinRT `Windows.ApplicationModel.DataTransfer.Clipboard`／`DataPackage` | WinUI 3 | Not established | RESEARCH-TECH-CLIPBOARD-001 | Unknown | No | No | Blocked |
| CLIP-OPT-003 | Win32 OLE Clipboard／COM `IDataObject` | WPF、WinUI 3、Win32／OLE | Not established | RESEARCH-TECH-CLIPBOARD-001 | Unknown | No | No | Blocked |
| CLIP-OPT-004 | Win32 Raw Clipboard APIs | WPF、WinUI 3、Win32／OLE | Not established | RESEARCH-TECH-CLIPBOARD-001 | Unknown | No | No | Blocked |
| CLIP-OPT-005 | Host-neutral abstraction with WPF、WinUI 3 and native adapters | WPF、WinUI 3 | Adapter composition not established | ARCH-0003、ARCH-0004、RESEARCH-TECH-CLIPBOARD-001 | Unknown | No | No | Blocked |

本表不形成 Candidate ranking。`Local availability=Unknown`、`Build verified=No`、`Runtime verified=No` 均不是 technology rejection；它們是 current readiness state。

## 10. Candidate–Host Pair Register

保留正好十個 `CLIP-PAIR-001..010`。Pair readiness 不由 API existence 單獨推導；所有 execution authorization 都是 `Not granted`。

| Pair ID | Candidate | Host | Eligibility from RESEARCH-TECH-CLIPBOARD-002 | Required API／Interop | COM／STA requirement | Dispatcher requirement | Packaging requirement | Project | Restore | Build | Clipboard operation | Runtime | Current readiness | Blocking IDs | Exclusion evidence | Execution authorization | Notes |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| CLIP-PAIR-001 | OPT-001 WPF Clipboard | WPF | Conditionally eligible | WPF Clipboard／`IDataObject` | Host-specific STA／COM evidence | WPF UI Dispatcher | Packaged／unpackaged separate | Required | Required | Required | Write only under separate authority | Required | Blocked | BLOCK-001..013 | None | Not granted | Basic WPF route |
| CLIP-PAIR-002 | OPT-002 WinRT Clipboard | WinUI 3 | Conditionally eligible | WinRT Clipboard／`DataPackage` | Host-specific apartment evidence | WinUI 3 Dispatcher | Packaged／unpackaged separate | Required | Required | Required | Write only under separate authority | Required | Blocked | BLOCK-001..013 | None | Not granted | Basic WinUI 3 route |
| CLIP-PAIR-003 | OPT-003 OLE Clipboard | WPF | Conditionally eligible | OLE／COM `IDataObject` | COM／STA required evidence | WPF Dispatcher if host-bound | Packaged／unpackaged separate | Required | Required | Required | Write only under separate authority | Required | Blocked | BLOCK-001..013 | None | Not granted | WPF OLE adapter route |
| CLIP-PAIR-004 | OPT-004 Raw Win32 Clipboard | WPF | Conditionally eligible | Raw Win32 Clipboard APIs | Native thread／COM evidence | WPF Dispatcher if host-bound | Packaged／unpackaged separate | Required | Required | Required | Write only under separate authority | Required | Blocked | BLOCK-001..013 | None | Not granted | WPF native interop route |
| CLIP-PAIR-005 | OPT-001 WPF Clipboard | WinUI 3 | Unknown | WPF Clipboard／adapter interop | Adapter-specific evidence | WinUI 3 Dispatcher | Packaged／unpackaged separate | Required | Required | Required | Write only under separate authority | Required | Not evaluated | BLOCK-001, 003, 004, 006..013 | None | Not granted | Cross-host suitability unknown |
| CLIP-PAIR-006 | OPT-002 WinRT Clipboard | WPF | Unknown | WinRT projection／adapter interop | Adapter-specific evidence | WPF Dispatcher | Packaged／unpackaged separate | Required | Required | Required | Write only under separate authority | Required | Not evaluated | BLOCK-001, 003, 004, 006..013 | None | Not granted | Cross-host suitability unknown |
| CLIP-PAIR-007 | OPT-003 OLE Clipboard | WinUI 3 | Conditionally eligible | OLE／COM `IDataObject` | COM／STA required evidence | WinUI 3 Dispatcher | Packaged／unpackaged separate | Required | Required | Required | Write only under separate authority | Required | Blocked | BLOCK-001..013 | None | Not granted | WinUI 3 OLE adapter route |
| CLIP-PAIR-008 | OPT-004 Raw Win32 Clipboard | WinUI 3 | Conditionally eligible | Raw Win32 Clipboard／native adapter | Native thread／COM evidence | WinUI 3 Dispatcher | Packaged／unpackaged separate | Required | Required | Required | Write only under separate authority | Required | Blocked | BLOCK-001..013 | None | Not granted | WinUI 3 native interop route |
| CLIP-PAIR-009 | OPT-005 Host-neutral abstraction | WPF | Conditionally eligible | WPF adapter plus scoped native route | Adapter contract and host STA | WPF Dispatcher | Packaged／unpackaged separate | Required | Required | Required | Write only under separate authority | Required | Blocked | BLOCK-001..013 | None | Not granted | Abstraction is not an API |
| CLIP-PAIR-010 | OPT-005 Host-neutral abstraction | WinUI 3 | Conditionally eligible | WinUI 3 adapter plus scoped native route | Adapter contract and host apartment | WinUI 3 Dispatcher | Packaged／unpackaged separate | Required | Required | Required | Write only under separate authority | Required | Blocked | BLOCK-001..013 | None | Not granted | Abstraction is not an API |

## 11. Clipboard Isolation Readiness

| Isolation capability | Specification status | Required environment | Clipboard operation dependency | Privacy dependency | Readiness | Blocking ID |
|---|---|---|---|---|---|---|
| Dedicated test user／VM／isolated session | Partially resolved | Dedicated account、disposable VM 或 isolated session | Write only after authority | No private Clipboard data | Blocked | BLOCK-006 |
| Existing Clipboard content policy | Partially resolved | Precondition must not inspect user data | Read No、Clear No | Must prove no private content | Blocked | BLOCK-006 |
| Clipboard Read prohibition | Resolved as a boundary | No read operation | Read No | No data inspection | Conditionally ready | BLOCK-006 |
| Clipboard Clear prohibition | Resolved as a boundary | No clear operation | Clear No | No user data destruction | Conditionally ready | BLOCK-006 |
| Clipboard overwrite consent | Blocked | Explicit isolated authority | Write separate | No user data overwrite | Blocked | BLOCK-006 |
| History disabled branch | Partially resolved | Isolated environment with observed state | Write separate | No settings mutation | Blocked | BLOCK-011 |
| History enabled branch | Blocked | Isolated environment and privacy review | Write separate | Synthetic only | Blocked | BLOCK-011 |
| Cloud disabled branch | Partially resolved | Isolated state observation | Write separate | No account data | Blocked | BLOCK-011 |
| Cloud enabled isolated branch | Blocked | Dedicated account／device or approved VM | Write separate | No unauthorized sync | Blocked | BLOCK-011 |
| Test account／device boundary | Blocked | No extra account unless separately authorized | No account operation now | Account identity not persisted | Blocked | BLOCK-011 |
| Test consumer boundary | Blocked | Isolated consumer only | Write／read by future consumer authority | Synthetic only | Blocked | BLOCK-008, 009 |
| Process termination cleanup | Blocked | Normal／abnormal termination scenarios | Write separate | Residual synthetic data control | Blocked | BLOCK-010, 012 |
| Residual payload cleanup | Blocked | Approved isolated cleanup | Clear remains No for user Clipboard | Synthetic only | Blocked | BLOCK-006, 012 |
| Failure recovery | Blocked | Bounded failure and cancellation path | Write／retry separate | No workflow mutation | Blocked | BLOCK-010, 012 |
| Private-data detection | Blocked | Precondition check without Clipboard read | Read No | Stop on uncertainty | Blocked | BLOCK-006 |
| Stop condition enforcement | Partially resolved | Manual and future runbook boundary | No operation until conditions pass | Privacy stop mandatory | Blocked | BLOCK-006, 012 |

本文件不得建立或啟動隔離環境。

## 12. Synthetic Image Readiness

| Synthetic capability | Specification status | Asset required | Runtime dependency | Evidence dependency | Readiness | Blocking ID |
|---|---|---|---|---|---|---|
| Fixed dimensions | Partially resolved | Specification only; no asset | Runtime later | Environment record | Blocked | BLOCK-007 |
| Small／normal／large classes | Partially resolved | Specification only; no asset | Runtime later | Memory record | Blocked | BLOCK-007 |
| Opaque region | Partially resolved | Specification only | Runtime later | Pixel comparison | Blocked | BLOCK-007 |
| Transparent region | Partially resolved | Specification only | Runtime later | Alpha comparison | Blocked | BLOCK-007 |
| Alpha gradient | Partially resolved | Specification only | Runtime later | Alpha comparison | Blocked | BLOCK-007 |
| Premultiplied Alpha reference | Partially resolved | Specification only | Runtime later | Alpha mode record | Blocked | BLOCK-007, 009 |
| Straight Alpha reference | Partially resolved | Specification only | Runtime later | Alpha mode record | Blocked | BLOCK-007, 009 |
| One-pixel border | Partially resolved | Specification only | Runtime later | Pixel comparison | Blocked | BLOCK-007 |
| Corner markers | Partially resolved | Specification only | Runtime later | Known coordinates | Blocked | BLOCK-007 |
| Center marker | Partially resolved | Specification only | Runtime later | Known coordinates | Blocked | BLOCK-007 |
| RGB primary blocks | Partially resolved | Specification only | Runtime later | Color／pixel comparison | Blocked | BLOCK-007, 009 |
| Grayscale gradient | Partially resolved | Specification only | Runtime later | Pixel／color comparison | Blocked | BLOCK-007, 009 |
| Mixed-language text | Partially resolved | Specification only | Runtime later | Consumer observation | Blocked | BLOCK-007, 009 |
| Fine-line pattern | Partially resolved | Specification only | Runtime later | Pixel comparison | Blocked | BLOCK-007, 009 |
| Known pixel coordinates | Partially resolved | Specification only | Runtime later | Expected coordinate record | Blocked | BLOCK-007, 009 |
| SDR color block | Partially resolved | Specification only | Runtime later | Color comparison | Blocked | BLOCK-007, 009 |
| Wide-color substitute metadata | Partially resolved | Specification only | Runtime later | Metadata responsibility | Blocked | BLOCK-009 |
| Synthetic run identifier | Partially resolved | Specification only | Runtime later | Evidence traceability | Blocked | BLOCK-007, 012 |

本文件不得建立 Image、Bitmap、PNG 或 payload。

## 13. Clipboard Format Readiness

| Format | Producer specification | Consumer specification | Alpha method | Color method | Lifetime method | Current readiness | Blocking ID |
|---|---|---|---|---|---|---|---|
| Framework Bitmap | Candidate／Host-specific object | WPF／WinUI 3 test consumer | Known marker comparison | Metadata and pixel record | Object／process lifetime | Blocked | BLOCK-008, 009 |
| `CF_BITMAP` | Native bitmap representation where applicable | Win32／framework consumer | Do not assume preservation | Conversion responsibility record | Handle／owner record | Blocked | BLOCK-008, 009 |
| `CF_DIB` | DIB header／stride／buffer | DIB-capable consumer | Separate Alpha observation | Header／channel interpretation | Buffer ownership | Blocked | BLOCK-008, 009 |
| `CF_DIBV5` | DIBV5 header／masks／buffer | DIBV5-capable consumer | Mask／Alpha observation | Color metadata responsibility | Buffer ownership | Blocked | BLOCK-008, 009 |
| PNG registered format | PNG byte stream | PNG-capable consumer | Decoded Alpha comparison | Decoder／metadata record | Stream lifetime | Blocked | BLOCK-008, 009 |
| WinRT Bitmap representation | WinRT `DataPackage` representation | WinRT／framework consumer | Consumer-specific comparison | Conversion responsibility | Reference／process lifetime | Blocked | BLOCK-008, 009 |
| OLE `IDataObject` | One or more OLE representations | OLE consumer | Per-format observation | Per-format observation | OLE ownership | Blocked | BLOCK-003, 004, 008 |
| WinRT `DataPackage` | One or more DataPackage representations | WinRT consumer | Per-format observation | Per-format observation | Package lifetime | Blocked | BLOCK-003, 008 |
| Multi-format publication | Multiple independent representations | Consumer-selected representation | Per-format comparison | Selection and conversion record | Aggregate lifetime | Blocked | BLOCK-008, 010 |

明確維持：

- PNG stream 不等於 decoded Bitmap。
- Format publication 成功不等於 Consumer 互通成功。
- Consumer 可貼上不等於 Alpha fidelity 通過。
- 本文件不得選擇產品正式格式。

## 14. Consumer Readiness

| Consumer class | Verification purpose | Required format | Launch required | Third-party dependency | Privacy boundary | Readiness |
|---|---|---|---|---|---|---|
| WPF test consumer | Framework bitmap、DIB／DIBV5、PNG observation | Scoped representation | Future isolated launch | No | Synthetic only | Blocked |
| WinUI 3 test consumer | WinRT bitmap、PNG、multi-format observation | Scoped representation | Future isolated launch | No | Synthetic only | Blocked |
| Win32／OLE test consumer | OLE object and standard format observation | OLE／DIB formats | Future isolated launch | No | Synthetic only | Blocked |
| Basic image-editor class | Interoperability observation | Documented supported format | Future isolated launch | Informative only | No real image | Blocked |
| Office-style consumer class | Interoperability observation | Documented supported format | Future isolated launch | Informative only | No account／document data | Blocked |
| Browser／web-content consumer class | Optional interoperability observation | Separately authorized format | Future isolated launch | Informative only | No network upload or private page | Deferred |
| Clipboard History surface | Materialization and format observation | Platform-observable format | Future isolated observation | Platform surface | No setting mutation | Blocked |
| Cloud Clipboard surface | Cross-device boundary observation | Platform-observable format | Future isolated observation | Account／device dependency | No extra account or unauthorized sync | Blocked |

不得開啟或操作任何 Consumer。

## 15. Threading／COM Readiness

| Scenario | Apartment requirement | Thread／Dispatcher requirement | Project evidence | Runtime evidence | Readiness | Blocking ID |
|---|---|---|---|---|---|---|
| WPF UI STA | STA | Active WPF Dispatcher | Planned only | None | Blocked | BLOCK-001, 004 |
| WPF background STA | STA | Explicit marshal／Dispatcher boundary | Planned only | None | Blocked | BLOCK-004 |
| WPF background MTA | MTA | Direct write must not be assumed | Planned only | None | Blocked | BLOCK-004 |
| WinUI 3 UI thread | Host-specific | Active WinUI 3 Dispatcher | Planned only | None | Blocked | BLOCK-001, 004 |
| WinUI 3 background thread | Host-specific | Explicit marshal／Dispatcher boundary | Planned only | None | Blocked | BLOCK-004 |
| OLE with COM initialized | STA／COM initialized | Candidate-specific | Planned only | None | Blocked | BLOCK-004 |
| OLE without required COM initialization | Unknown／not initialized | Candidate-specific | Planned only | None | Blocked | BLOCK-004 |
| Dispatcher shutdown | Host-specific | Shutting down | Planned only | None | Blocked | BLOCK-004, 010 |
| Application shutdown during publication | Host-specific | Shutting down | Planned only | None | Blocked | BLOCK-004, 010 |
| Cancellation during retry | Host-specific | Active or shutting down | Planned only | None | Blocked | BLOCK-004, 010 |

不得將官方文件描述視為 Runtime 通過。

## 16. Evidence and Privacy Readiness

| Evidence capability | Planned method | Clipboard operation required | Persistence required | Privacy risk | Current readiness | Blocking effect |
|---|---|---|---|---|---|---|
| Environment record | Record actual OS／SDK／Host／package fields | No | Yes | Low if synthetic | Blocked | Evidence Write not granted |
| Synthetic image specification | Store specification only | No | Yes | Low | Blocked | Evidence Write not granted |
| Producer payload metadata | Record type／size／format without bytes | Write | Yes | Medium | Blocked | Clipboard Write and Evidence Write |
| Clipboard format enumeration | Record format identity only | Write／read by consumer | Yes | Medium | Blocked | Clipboard and consumer authority |
| Consumer observation | Isolated test consumer | Write／consumer read | Yes | Medium | Blocked | Consumer launch and Evidence Write |
| Pixel comparison | Compare known synthetic markers | Write／consumer read | Yes | Low | Blocked | Runtime and Evidence Write |
| Alpha comparison | Compare known alpha markers | Write／consumer read | Yes | Low | Blocked | Runtime and Evidence Write |
| Color metadata | Record metadata／decoder responsibility | Write／consumer read | Yes | Low | Blocked | Runtime and Evidence Write |
| Thread／Apartment record | Record actual thread／COM／Dispatcher | No or write route | Yes | Low | Blocked | Runtime and Evidence Write |
| Dispatcher observation | Observe marshal／shutdown behavior | Write route | Yes | Low | Blocked | Runtime and Evidence Write |
| Contention failure | Synthetic bounded contention | Write | Yes | Medium | Blocked | Isolation and Runtime |
| Retry timing | Attempt／interval／timeout fields | Write | Yes | Medium | Blocked | Retry authority and Runtime |
| Ownership／lifetime observation | Process／owner／consumer timeline | Write | Yes | Medium | Blocked | Runtime and Evidence Write |
| Process termination observation | Normal／abnormal process scenario | Write | Yes | Medium | Blocked | Runtime and cleanup authority |
| Memory observation | Size／allocation／peak fields | Write | Yes | Low | Blocked | Isolated resource boundary |
| History／Cloud observation | Observe isolated state only | Write | Yes | High | Blocked | Account／device and privacy authority |
| Parallel File Output result | Independent output observation | No Clipboard read | Yes | Medium | Blocked | Output observation authority |
| Diagnostic log | Metadata only; no payload or private path | Operation-specific | Yes | High | Blocked | Privacy review and Evidence Write |
| Privacy review | Review synthetic and isolation controls | No | Yes | High | Blocked | Reviewer and cleanup evidence |
| Cleanup confirmation | Confirm isolated residue cleanup | No user Clipboard clear | Yes | Medium | Blocked | Cleanup authority |

明確規定：

- 沒有 Evidence Write authorization 不得建立 Artifact。
- Session observation 不等於 Persistent Evidence。
- 不得記錄私人 Clipboard payload。
- 不得將 Clipboard 內容寫入 Log。
- Clipboard 成功不能單獨關閉 Format、Alpha、Privacy 或 Lifetime blocker。

## 17. Environment Readiness

| Environment requirement | Existing evidence | Status | Required phase | Deferred allowed | Affected spikes |
|---|---|---|---|---|---|
| Windows 11 x64 | Official／research target only | Partially resolved | L1 | No | 001..012 |
| WPF Host | UI research only | Blocked | L1 | No | 001, 003..012 |
| WinUI 3 Host | UI research only | Blocked | L1 | No | 002..012 |
| Packaged mode | Plan only | Blocked | L1／L3 | Yes until 011 | 001, 002, 005, 008, 010, 011 |
| Unpackaged mode | Plan only | Blocked | L1 | No | 001..012 |
| Debug | No build evidence | Blocked | L1 | No | 001..012 |
| Release | No build evidence | Blocked | L1／L3 | Yes for planning | 001..012 |
| STA | Official baseline only | Partially resolved | L1／L2 | No | 001, 003, 006..012 |
| MTA | Official baseline only | Partially resolved | L2 | Yes until 006 | 006 |
| UI thread | Host not selected | Blocked | L1 | No | 001, 002, 006, 011, 012 |
| Background thread | No runtime evidence | Blocked | L2 | Yes until 006 | 006, 007, 008, 012 |
| History disabled | No isolated observation | Blocked | L3 | Yes until 010 | 010 |
| History enabled | No isolated observation | Blocked | L3 | Yes until 010 | 010 |
| Cloud disabled | No isolated observation | Blocked | L3 | Yes until 010 | 010 |
| Cloud enabled isolated branch | No account／device authority | Blocked | L3 | Yes; not L1 blocker | 010 |
| Cold process | Plan only | Blocked | L2 | Yes until 008 | 008 |
| Warm process | Plan only | Blocked | L2 | Yes until 008 | 008 |
| Normal termination | Plan only | Blocked | L2 | Yes until 008 | 008 |
| Abnormal termination | Plan only | Blocked | L2 | Yes until 008 | 008 |
| Stable memory observation environment | No resource boundary | Blocked | L2 | Yes until 009 | 009 |

不得要求 Phase L1 等待完整 Cloud Clipboard 驗證；但不得以此免除 Phase L3 的獨立 privacy readiness。

## 18. Per-Spike Readiness Matrix

建立正好十二列，覆蓋 `CLIP-SPIKE-001..012`。Readiness 由 Pair、Prerequisite、Isolation、Format、Threading、Evidence 及 Authorization 推導；所有 Execution authorized 都是 `No`。

| Spike | Required pairs | Required prerequisites | Isolation requirement | Required formats／consumers | Required evidence | Privacy condition | Readiness | Blocking IDs | Execution authorized |
|---|---|---|---|---|---|---|---|---|---|
| CLIP-SPIKE-001 | PAIR-001, 009 | PREQ-001, 003, 004, 009, 013, 016, 017, 025, 031, 032 | Isolated WPF session; no existing private Clipboard access | Framework Bitmap／WPF consumer | Environment、format、pixel／alpha、thread、cleanup | Synthetic only; Read／Clear No | Blocked | BLOCK-001..009, 012, 013 | No |
| CLIP-SPIKE-002 | PAIR-002, 010 | PREQ-002, 003, 005, 010, 012, 013, 016, 017, 025, 031, 032 | Isolated WinUI 3 session; package state recorded | WinRT Bitmap／WinUI 3 consumer | Environment、package、format、pixel／alpha、thread、cleanup | Synthetic only; no settings mutation | Blocked | BLOCK-001..009, 012, 013 | No |
| CLIP-SPIKE-003 | PAIR-003, 004, 007, 008 | PREQ-006, 007, 011, 013, 016, 019, 020, 023, 024, 025, 031, 032 | Isolated native／host session | DIB／DIBV5／Win32、WPF、WinUI 3 consumers | Header、mask、stride、pixel／alpha、ownership、cleanup | No real image bytes | Blocked | BLOCK-003..009, 012, 013 | No |
| CLIP-SPIKE-004 | PAIR-003, 004, 007, 008, 009, 010 | PREQ-006, 007, 013, 016, 021, 023, 024, 025, 031, 032 | Isolated decoder／consumer session | PNG stream／isolated consumer | Stream、decoder、decoded pixel／alpha、color、lifetime | No network or private content | Blocked | BLOCK-003, 006..009, 012, 013 | No |
| CLIP-SPIKE-005 | PAIR-001..010 | PREQ-004..008, 013, 016..025, 031, 032 | Isolated multi-format session | Multiple representations／scoped consumers | Enumeration、atomicity、selection、per-format fidelity、ownership | Synthetic only; no user settings | Blocked | BLOCK-001..013 | No |
| CLIP-SPIKE-006 | PAIR-001..010 | PREQ-009..011, 013, 016, 028, 031, 032 | Isolated thread／COM session | Fixed representation／minimal consumer | Apartment、COM、Dispatcher、shutdown、cancellation | No existing Clipboard inspection | Blocked | BLOCK-001, 004, 006, 007, 010, 012, 013 | No |
| CLIP-SPIKE-007 | PAIR-001..010 | PREQ-013..015, 026, 027, 031, 032 | Isolated synthetic contention source | Fixed representation／isolated consumer | Initial failure、attempts、timing、cancel、cleanup | Owner identity anonymized; no user data | Blocked | BLOCK-006, 010, 012, 013 | No |
| CLIP-SPIKE-008 | PAIR-001..010 | PREQ-013..015, 022, 028, 031, 032 | Isolated process lifecycle session | Fixed representation／consumer before and after process events | Ownership、immediate／delayed、process、consumer、cleanup | Synthetic only; no backup／restore | Blocked | BLOCK-006, 010, 012, 013 | No |
| CLIP-SPIKE-009 | PAIR-001..010 | PREQ-013, 016, 017..024, 029, 031, 032 | Isolated resource boundary | Size classes／minimal consumer | Size、allocation、memory、format、failure、cleanup | Synthetic only; bounded resource use | Blocked | BLOCK-006..009, 012, 013 | No |
| CLIP-SPIKE-010 | PAIR-001..010 | PREQ-013..015, 022, 030, 031, 032 | Isolated History／Cloud branch | Approved representation／History or Cloud surface | State、materialization、format、privacy、cleanup | No settings mutation, account or device transfer | Blocked | BLOCK-006, 011, 012, 013 | No |
| CLIP-SPIKE-011 | PAIR-001..010 | PREQ-001..003, 012, 013, 031, 032 | Same isolated environment across package modes | Same representation／same consumer | Package state、API、format、thread、lifetime、cleanup | Synthetic only; no deployment mutation | Blocked | BLOCK-001..006, 012, 013 | No |
| CLIP-SPIKE-012 | PAIR-001..010 | PREQ-013..016, 026, 027, 031, 032 | Isolated synthetic output and failure source | Fixed Clipboard representation; independent File Output observer | Clipboard failure、File Output result、no rerun、state preservation、cleanup | No real paths, title, image or Clipboard bytes | Blocked | BLOCK-006, 007, 010, 012, 013 | No |

## 19. Phase Readiness

### Phase L1 — Basic Publication and Format Interoperability

| Requirement | Current status |
|---|---|
| At least one WPF Pair Project／Build path clearly specified | Blocked |
| At least one WinUI 3 Pair Project／Build path clearly specified | Blocked |
| At least one Clipboard Candidate API／Interop identity fixed for an experimental route | Partially resolved, not executable |
| Isolation policy specified | Partially resolved, not executable |
| Synthetic basic image specified | Partially resolved, asset not created |
| Bitmap、DIB／DIBV5、PNG、multi-format Evidence method specified | Partially resolved, no runtime evidence |
| Clipboard Read／Write／Clear permissions separated | Partially resolved as boundary; operation authority not granted |
| Runtime and Evidence Write independently authorized | Blocked |
| Phase L1 readiness | Not ready |

### Phase L2 — Threading、Contention and Lifetime

另外需要：

- STA／MTA 及 Dispatcher 情境。
- Contention creation 及 observation。
- Retry observation。
- Process lifetime。
- Large-image memory observation。

目前 `Phase L2 readiness = Not ready`，因 `CLIP-BLOCK-004`、`CLIP-BLOCK-010`、`CLIP-BLOCK-012` 及 `CLIP-BLOCK-013` 尚未解除。

### Phase L3 — Platform Integration and Output Independence

另外需要：

- History／Cloud 隔離環境。
- Packaged／unpackaged 矩陣。
- Clipboard failure 與 File Output 獨立驗證。

目前 `Phase L3 readiness = Not ready`，因 `CLIP-BLOCK-005`、`CLIP-BLOCK-006`、`CLIP-BLOCK-011`、`CLIP-BLOCK-012` 及 `CLIP-BLOCK-013` 尚未解除。

每個 Phase 只能使用：

- `Ready`
- `Conditionally ready`
- `Not ready`

目前缺少 Project、Build、Clipboard 操作、Runtime 及 Evidence Write authority，三個 Phase 均不得標示 `Ready`。

## 20. Minimum Phase L1 Blocking Action Set

只列真正阻止最早 Phase L1 的事項，不把 L2／L3 全部項目升格為 L1 blocker。

| Action ID | Blocking condition | Source prerequisites | Source blockers | Affected pairs | Affected spikes | Required evidence | Documentary or execution requirement | Mutation required | Clipboard operation required | Authorization dependency | Completion condition |
|---|---|---|---|---|---|---|---|---|---|---|---|
| CLIP-BA-001 | WPF／WinUI 3 Host dependency remains unresolved | PREQ-001, 002, 003 | BLOCK-001, 002 | PAIR-001, 002, 009, 010 | SPIKE-001, 002, 005 | UI research traceability and experimental Host identity | Documentary review first; no Project creation in this action | No | No | UI／Project／Build authority later | Host boundary and permitted experimental identities are documented |
| CLIP-BA-002 | Candidate API／Interop route not bound to an experimental identity | PREQ-004..008 | BLOCK-003 | PAIR-001..010 | SPIKE-001..005 | Candidate identity and adapter boundary | Documentary binding; no API call | No | No | Project／Build／Runtime later | At least one scoped route is reviewable without selecting product technology |
| CLIP-BA-003 | Clipboard isolation policy is not executable | PREQ-013..015 | BLOCK-006 | PAIR-001..010 | SPIKE-001..005 | Isolation policy, no-read/no-clear boundary, privacy stop rules | Documentary policy review | No | No | Clipboard／Runtime later | Isolated environment and separate operation permissions are defined |
| CLIP-BA-004 | Synthetic Image input is not approved | PREQ-016 | BLOCK-007 | PAIR-001..010 | SPIKE-001..005 | Synthetic specification and run identity | Documentary specification only; no image creation | No | No | Evidence／Runtime later | Future input can be created only after separate authority |
| CLIP-BA-005 | Format／Consumer verification path is incomplete | PREQ-017..025 | BLOCK-008, 009 | PAIR-001..010 | SPIKE-001..005 | Format、consumer、Alpha／pixel／color method | Documentary matrix review; no consumer launch | No | No | Clipboard Write／Runtime／Evidence later | L1 formats and consumer boundaries are independently reviewable |
| CLIP-BA-006 | Evidence and cleanup path is not authorized | PREQ-031, 032 | BLOCK-012, 013 | PAIR-001..010 | SPIKE-001..005 | Evidence schema、privacy review、cleanup method | Documentary authority request may be a later task; no artifact now | No | No | Evidence Write、Project／Restore／Build／Runtime later | Separate authority and cleanup evidence path exist |

Shared UI Host action 必須引用 `RESEARCH-TECH-UI-001`、`ADR-0002` 與相關 Architecture；只有 Clipboard-specific action 才建立 `CLIP-BA` ID。本表不執行任何 action。

## 21. Overall Readiness Decision

只能使用：

- `Ready for clipboard runtime spike execution authorization review`
- `Conditionally ready for clipboard runtime spike execution authorization review`
- `Not ready`

必須機械式推導：

```text
Shared UI blockers
  + Clipboard prerequisites
  + Candidate–Host Pair readiness
  + Isolation readiness
  + Synthetic Image readiness
  + Format／Consumer readiness
  + Threading／COM readiness
  + Evidence／Privacy readiness
  + Clipboard Read／Write／Clear authority
  + Project／Restore／Build／Runtime／Evidence authority
→ Overall Readiness Decision
```

| Input | Current state | Decision effect |
|---|---|---|
| Shared UI blockers | BLOCK-001、002 Open | Blocking |
| Clipboard prerequisites | PREQ-001..032 contain Blocked／Partially resolved states | Blocking |
| Candidate–Host Pair readiness | Blocked or Not evaluated | Blocking |
| Isolation readiness | Blocked | Blocking |
| Synthetic Image readiness | Blocked; no asset created | Blocking |
| Format／Consumer readiness | Blocked; no consumer launched | Blocking |
| Threading／COM readiness | Blocked; no runtime evidence | Blocking |
| Evidence／Privacy readiness | Blocked; no artifact authority | Blocking |
| Clipboard Read／Write／Clear authority | No／No／No | Blocking |
| Project／Restore／Build authority | Not granted | Blocking |
| Runtime／Evidence authority | Not granted | Blocking |
| Overall Readiness Decision | `Not ready` | Do not execute |

依目前狀態固定為：

- Build Verification: `Not performed`
- Runtime Verification: `Not performed`
- Clipboard Runtime Spike Authorized: `No`
- Clipboard Read Authorized: `No`
- Clipboard Write Authorized: `No`
- Clipboard Clear Authorized: `No`
- Evidence Write Authorized: `No`
- Clipboard Decision: `Not made`
- Capture Decision: `Not made`
- Rendering Decision: `Not made`

## 22. Traceability

```text
CLIP prerequisite／blocker
  → CLIP Candidate–Host Pair
  → CLIP Spike
  → Phase readiness
  → Minimum Blocking Action
  → Future execution authorization review
  → Future Clipboard decision
```

| Source | Use in this readiness assessment |
|---|---|
| `docs/Research/Technology/29-clipboard-integration-feasibility.md` / `RESEARCH-TECH-CLIPBOARD-001` | Five Candidate、criteria、gates、gaps、official baseline、privacy and ownership boundary |
| `docs/Research/Technology/30-clipboard-integration-runtime-spike-plan.md` / `RESEARCH-TECH-CLIPBOARD-002` | Twelve Spike、format、isolation、evidence、phase and stop contracts |
| `Architecture/TECHNOLOGY-DECISION-ROADMAP.md` / `TD-004` | Clipboard Integration remains Candidate; no selection or ADR |
| `Architecture/adr/ADR-0002-ui-framework-selection.md` / `ADR-0002` | UI Framework remains Draft and unresolved |
| `docs/Research/Technology/01-ui-framework-feasibility.md` / `RESEARCH-TECH-UI-001` | WPF／WinUI 3 feasibility and shared UI dependency |
| `docs/Research/Technology/20-capture-backend-feasibility.md` / `RESEARCH-TECH-CAPTURE-001` | Capture boundary and no-capture-rerun dependency |
| `docs/Research/Technology/10-rendering-technology-feasibility.md` / `RESEARCH-TECH-RENDER-001` | Rendering boundary and no-rendering decision |
| `Architecture/ARCH-0001-architecture-principles.md` / `ARCH-0001` | Long-term boundary and maintainability principles |
| `Architecture/ARCH-0002-layer-model.md` / `ARCH-0002` | Layer and platform integration context |
| `Architecture/ARCH-0003-module-catalog.md` / `ARCH-0003` | Platform adapter and module boundary |
| `Architecture/ARCH-0004-component-boundaries.md` / `ARCH-0004` | Component ownership, `COMP-009` and `COMP-015` boundary |
| `Architecture/ARCH-0005-component-interactions.md` / `ARCH-0005` | Clipboard handoff and parallel output interactions |
| `PRD/PRD-0005-functional-requirements.md` / `FR-007` | Deliver result to clipboard requirement |
| `Specs/SPEC-0007-clipboard-handoff.md` / `FEAT-003` | Capture Result → Clipboard Ready → abstract Clipboard Consumer |
| `Specs/SPEC-0010-feature-integration.md` | Clipboard／File Output parallel independence |
| `AGENTS.md` | No build／run／test by default and documentation-only safety boundary |

實際文件名稱及 ID 必須從 Repository 原樣引用，不得猜測。Traceability 只說明依賴，不表示任何 source 已提供 runtime evidence。

## Completion Conditions

- 只建立 `31-clipboard-integration-runtime-spike-execution-readiness.md`。
- Document ID 固定為 `RESEARCH-TECH-CLIPBOARD-003`。
- Parent Runtime Plan 固定為 `RESEARCH-TECH-CLIPBOARD-002`。
- Parent Feasibility 固定為 `RESEARCH-TECH-CLIPBOARD-001`。
- 建立 `CLIP-PREQ-001..032`，每項包含完整 prerequisite fields。
- 建立連續的 `CLIP-BLOCK` Register。
- 覆蓋十個 `CLIP-PAIR-001..010`。
- 建立 Clipboard Isolation、Synthetic Image、Format、Consumer、Threading／COM、Evidence／Privacy 及 Environment readiness。
- 建立正好十二列 Per-Spike Readiness，覆蓋 `CLIP-SPIKE-001..012`。
- 建立 Phase L1–L3 Readiness。
- 建立最小 `CLIP-BA` 集合，只列 L1 真正 blocker。
- Overall Readiness 由矩陣機械式推導為 `Not ready`。
- 所有 Spike Execution authorization 為 `No`。
- Clipboard Read／Write／Clear 全部維持未授權。
- 不讀取、寫入、清除或備份 Clipboard。
- 不建立 Project、Prototype、Payload、Result、Source Code 或 Evidence。
- 不執行下載、安裝、Restore、Build、Run、Publish、Test 或 Runtime Spike。
- 不修改 UI／Capture／Rendering Research Line。
- 不選擇 Clipboard Technology。
- 不建立 Clipboard ADR。
- 不開始 Clipboard 或截圖功能。
- 唯讀檢查應確認 `git diff --check` 通過，且只涉及本文件。

## Current Execution Record

| Field | Value |
|---|---|
| Document created | This readiness assessment only |
| Local Clipboard inspection | Not performed |
| Clipboard read | Not performed |
| Clipboard write | Not performed |
| Clipboard clear | Not performed |
| Project／Solution／Prototype | Not created |
| Bitmap／DIB／DIBV5／PNG／payload | Not created |
| Result／Evidence artifact | Not created |
| Restore／Build／Run／Publish／Test | Not performed |
| Runtime Spike | Not performed |
| Technology selection | Not made |
| Clipboard ADR | Not created |
| Screenshot feature | Not started |
| Overall readiness | Not ready |
