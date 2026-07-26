# UI Framework Phase 1 Execution Enablement Specification

本文件將 `RESEARCH-TECH-UI-007` 的 8 個 Phase 1 Blocking Actions 轉為一對一的未來 Enablement Specification。它只規格化「若取得人工授權，未來需要如何準備與驗證」，不執行任何操作、不建立 Project、不建立 Result、不建立 Screenshot 或 Screen recording，也不改變 `RESEARCH-TECH-UI-007` 的 `Readiness Decision: Not ready`。

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `RESEARCH-TECH-UI-008` |
| Title | UI Framework Phase 1 Execution Enablement Specification |
| Status | `Draft` |
| Research Type | Execution Enablement Specification |
| Evidence Baseline | `RESEARCH-TECH-UI-007` |
| Runtime Verification | `Not performed` |
| Build Verification | `Not performed` |
| Current authorization | `Not granted` |
| Execution permitted | `No` |
| Owner | TBD |
| Last reviewed | Not reviewed |
| Version | 0.1 |
| Specification date | 2026-07-26 |
| Normative References | `RESEARCH-TECH-UI-003` 至 `RESEARCH-TECH-UI-007`, `Architecture/adr/ADR-0002-ui-framework-selection.md` |
| Informative References | `RESEARCH-TECH-UI-001`, `RESEARCH-TECH-UI-002`, `Architecture/TECHNOLOGY-DECISION-ROADMAP.md` |
| Supersedes | None |
| Superseded by | None |

## 2. Purpose and Boundary

本文件的目的：

- 將 `BA-001` 至 `BA-008` 綁定為 `UI-ENABLE-001` 至 `UI-ENABLE-008`。
- 明確描述每個 Enablement Item 的 evidence、操作類型、所需授權、成功條件與 rollback/cleanup。
- 分離 WinUI 3 與 WPF 的實驗工具鏈 baseline，不做產品技術選擇。
- 定義 Display、DPI、Synthetic Input、Evidence 與 Safety 的未來實驗邊界。
- 讓後續人工 reviewer 能看出哪些操作會改變本機、哪些操作需要下載或執行，以及哪些結果仍不可宣告。

本文件不授權任何操作。所有 Enablement Item 的 `Current authorization` 都是 `Not granted`，所有 `Execution permitted` 都是 `No`。

## 3. Non-goals and Prohibited Actions

本文件不得：

- 安裝或更新 IDE、Build Tools、SDK、Runtime、Workload、Package 或其他工具。
- 執行 Network download、Package restore、Project creation、Build、Run、Publish 或 Deployment。
- 建立 `docs/Research/Technology/results/ui-framework/` 或其他 Result directory。
- 建立 Project、Solution、Prototype、Overlay、Capture hook、Screen capture pipeline 或產品 source code。
- 建立 Screenshot、Screen recording、Diagnostic log、Measurement data 或其他 future result artifact。
- 修改 Windows Display Settings、DPI、HDR、Registry、Global shortcut 或系統權限。
- 執行 `UI-SPIKE-001` 至 `UI-SPIKE-011`。
- 決定正式產品 Runtime、SDK、Build Tool、WinUI 3 或 WPF。
- 修改 `RESEARCH-TECH-UI-003` 至 `RESEARCH-TECH-UI-007`、`ADR-0002`、PRD、Specs 或 Architecture。

## 4. Status and Authorization Vocabulary

### 4.1 Enablement Item Status

每個 `UI-ENABLE` 的 `Status` 只能使用：

- `Specified`：本文件已描述必要欄位、操作邊界、證據、授權與 rollback 條件。
- `Partially specified`：已有規格，但仍有明確的 specification gap。
- `Blocked`：沒有取得必要前置條件，不能進入下一步。
- `Deferred`：移至 Phase 2/3 或非必要 branch。
- `Not applicable`：經 scope review 確認不適用於該 Enablement Item。

不得使用 `Completed` 或 `Resolved` 作為 Enablement Item Status。`Specified` 不表示已安裝、已建置、已執行或已驗證。

### 4.2 Authorization Vocabulary

| Field | Allowed values | Meaning |
| --- | --- | --- |
| Current authorization | `Not granted`, `Granted` | 目前是否有明確人工授權；本文件所有項目固定為 `Not granted` |
| Execution permitted | `Yes`, `No` | 目前是否可執行；本文件所有項目固定為 `No` |
| Requested authorization | `Not yet requested`, `Required before execution`, `Not required` | 未來人工審查的要求，不代表目前已提出或核准 |

### 4.3 Operation vocabulary

所有未來操作只能使用下列 classification；每個 Enablement Item 可以有一種或多種：

| Classification | 說明 | 本文件是否執行 |
| --- | --- | --- |
| `Read-only inspection` | 查詢版本、環境、工具或 display evidence，不改變本機狀態 | No |
| `Repository documentation mutation` | 只修改本 Repository 的 Markdown | 本文件只進行此類文件修改 |
| `Development environment installation` | 安裝 IDE、Build Tools、SDK 或工具 | No |
| `Package acquisition` | Restore 或下載 NuGet package | No |
| `Experimental project creation` | 建立隔離式 Spike Project | No |
| `Build execution` | 編譯實驗 Project | No |
| `Runtime execution` | 啟動實驗程式或執行 Spike | No |
| `System configuration mutation` | 修改 DPI、HDR、快捷鍵、Registry 或其他系統設定 | No |
| `Evidence capture` | 建立 Log、Screenshot、Recording 或量測資料 | No |

## 5. Source Binding

本文件直接綁定 `RESEARCH-TECH-UI-007` 的原始 Blocking Action；不得重新編號、合併或拆分：

| Enablement ID | Source Blocking Action | Related P1 Gate | Source evidence |
| --- | --- | --- | --- |
| `UI-ENABLE-001` | `BA-001` WinUI 3 SDK／template／build-path provenance | `P1-GATE-002`, `P1-GATE-004` | `UI-PREQ-001`, `UI-BLOCK-001`, `UI-CLOSURE-FIND-004` |
| `UI-ENABLE-002` | `BA-002` WPF 與 WinUI 等價 build-path definition | `P1-GATE-003`, `P1-GATE-004` | `UI-PREQ-001`, `UI-PREQ-003`, `UI-BLOCK-001`, `UI-CLOSURE-FIND-003` |
| `UI-ENABLE-003` | `BA-003` Display topology baseline | `P1-GATE-005` | `UI-PREQ-004`, `UI-PREQ-005`, `UI-BLOCK-003`, `UI-ENV-GAP-002`, `UI-CLOSURE-FIND-002` |
| `UI-ENABLE-004` | `BA-004` Per-monitor DPI evidence path | `P1-GATE-006` | `UI-PREQ-004`, `UI-PREQ-005`, `UI-BLOCK-003`, `UI-ENV-GAP-001`, `UI-CLOSURE-FIND-002` |
| `UI-ENABLE-005` | `BA-005` Synthetic content/input isolation | `P1-GATE-007` | `UI-PREQ-003`, `UI-PREQ-009`, `UI-CLOSURE-FIND-006` |
| `UI-ENABLE-006` | `BA-006` Evidence storage governance | `P1-GATE-008` | `UI-PREQ-012`, `UI-BLOCK-007`, `UI-ENV-GAP-009`, `UI-CLOSURE-FIND-006` |
| `UI-ENABLE-007` | `BA-007` Safety／cleanup acceptance | `P1-GATE-009` | `UI-PREQ-013`, `UI-BLOCK-008`, `UI-ENV-GAP-010`, `UI-CLOSURE-FIND-006` |
| `UI-ENABLE-008` | `BA-008` Independent Phase 1 authorization | `P1-GATE-010` | `UI-PREQ-014`, `UI-BLOCK-009`, `UI-ENV-GAP-011`, `UI-CLOSURE-FIND-007` |

若發現上游描述不足，只能在本文件記錄 `ENABLEMENT-GAP`，不得修改上游文件。

## 6. Enablement Item Specifications

下列 8 個 Item 各自包含相同的必要欄位。所有操作都是未來候選操作；目前不執行。

### 6.1 UI-ENABLE-001 — WinUI 3 SDK／template／build-path provenance

| Field | Value |
| --- | --- |
| Enablement ID | `UI-ENABLE-001` |
| Source Blocking Action | `BA-001` |
| Related P1 Gate | `P1-GATE-002`, `P1-GATE-004` |
| Source evidence | `UI-PREQ-001`, `UI-BLOCK-001`, `UI-CLOSURE-FIND-004` |
| Current unresolved condition | Runtime package `2.3.1.0` observed；SDK package、template、candidate parity 與 build path 未證明 |
| Required final evidence | Official candidate version、local SDK/package/template provenance、resolved build path、environment metadata；`Build verified` 仍須等未來授權後驗證 |
| Proposed enablement operation | 先做 read-only provenance inspection；若必要且另獲授權，再建立隔離 Spike Project，取得 package/template/build evidence |
| Operation type | `Read-only inspection`; `Package acquisition`; `Experimental project creation`; `Build execution`; `Runtime execution` |
| Local system mutation required | Current: No；future project/package/build: Yes |
| Network download required | Potentially Yes；本文件不下載 |
| Package restore required | Potentially Yes；本文件不 restore |
| Project creation required | Potentially Yes；本文件不建立 |
| Build required | Yes for future verification；本文件不 build |
| Runtime execution required | Yes for future runtime comparison；本文件不 run |
| Administrator privilege required | Unknown；以實際 operation 及既有權限為準，不預先宣告需要或不需要 |
| Human authorization required | Yes，且需先通過 Phase-specific review |
| Expected system effect | 隔離的 SDK/template/build provenance 與 future experimental artifact；不得影響產品 source tree |
| Success condition | Runtime、SDK、template、MSBuild、candidate version 與 build path 可分離追溯；得到 reviewer 可接受的 evidence |
| Failure condition | 只能證明 Runtime package，或 package/template/build path 仍無法追溯 |
| Rollback／cleanup requirement | 移除 future test project、package cache delta、temporary process 與 result artifacts；保存 rollback record |
| Result artifact | Future `Environment record`、`Build record`、`Failure reproduction`；目前不建立 |
| Resulting readiness recommendation | `P1-GATE-002`／`004` 仍由 evidence review 判定，不自動選定 framework |
| Owner | TBD |
| Status | `Specified` |
| Open questions | Existing SDK/package/template 是否可用？primary 與 fallback build path 是否能維持 candidate parity？ |

### 6.2 UI-ENABLE-002 — WPF 與 WinUI 等價 build-path definition

| Field | Value |
| --- | --- |
| Enablement ID | `UI-ENABLE-002` |
| Source Blocking Action | `BA-002` |
| Related P1 Gate | `P1-GATE-003`, `P1-GATE-004` |
| Source evidence | `UI-PREQ-001`, `UI-PREQ-003`, `UI-BLOCK-001`, `UI-CLOSURE-FIND-003` |
| Current unresolved condition | WPF 的 .NET SDK／Desktop Runtime 較完整，但兩個 candidate 沒有同一 acceptance boundary 與 build provenance |
| Required final evidence | Candidate version matrix、同一 CPU/Windows/build configuration、同一 synthetic content、同一 build acceptance record |
| Proposed enablement operation | 建立 candidate parity checklist，指定 primary/fallback build path，之後才可在獨立授權下驗證 |
| Operation type | `Repository documentation mutation`; `Read-only inspection`; `Experimental project creation`; `Build execution` |
| Local system mutation required | Current: No；future project/build: Yes |
| Network download required | Potentially Yes；本文件不下載 |
| Package restore required | Potentially Yes；本文件不 restore |
| Project creation required | Potentially Yes；本文件不建立 |
| Build required | Yes for future parity verification；本文件不 build |
| Runtime execution required | Not required for path definition；future Windowing Spike requires separate authorization |
| Administrator privilege required | Unknown；未取得證據前保持 Unknown |
| Human authorization required | Yes，任何 project/build operation 前都需要 |
| Expected system effect | 形成可比較的 candidate build-path record，不形成產品工具鏈決策 |
| Success condition | WPF 與 WinUI 可用同一 acceptance boundary 進行 future comparison，且 build path 差異被記錄 |
| Failure condition | 任一 candidate 只能透過不同內容、不同 configuration 或不可追溯的工具鏈建置 |
| Rollback／cleanup requirement | Candidate 工作目錄、temporary package delta、process 與 evidence 必須可隔離清理 |
| Result artifact | `Candidate parity record`、`Build record`；目前不建立 |
| Resulting readiness recommendation | 只提供 `P1-GATE-004` 的 future input，不改變 framework selection |
| Owner | TBD |
| Status | `Specified` |
| Open questions | 是否存在不需安裝的 SDK-only fallback？兩個 candidate 能否使用相同 build configuration？ |

### 6.3 UI-ENABLE-003 — Display topology baseline

| Field | Value |
| --- | --- |
| Enablement ID | `UI-ENABLE-003` |
| Source Blocking Action | `BA-003` |
| Related P1 Gate | `P1-GATE-005` |
| Source evidence | `UI-PREQ-004`, `UI-PREQ-005`, `UI-BLOCK-003`, `UI-ENV-GAP-002`, `UI-CLOSURE-FIND-002` |
| Current unresolved condition | 3 個 active PnP/EDID records 已觀察，但每個 display 的 resolution、position、primary、extend/duplicate 與 physical identity mapping 不完整 |
| Required final evidence | `ENV-UI-001` 單螢幕 baseline、`ENV-UI-002` 多螢幕 baseline、每個 display path 的 resolution/position/primary/mode record |
| Proposed enablement operation | 只做 read-only display topology inspection；不改 Display Settings，不以 record count 推定 physical topology |
| Operation type | `Read-only inspection` |
| Local system mutation required | No |
| Network download required | No |
| Package restore required | No |
| Project creation required | No for topology inspection |
| Build required | No for topology inspection |
| Runtime execution required | No for topology inspection；future UI Spike separate |
| Administrator privilege required | Unknown；以既有 read-only inspection 能力為準 |
| Human authorization required | Yes，因為它是 Phase 1 execution enablement gate |
| Expected system effect | No system configuration change；只取得可重現 display evidence |
| Success condition | 可重現描述單螢幕、三螢幕、primary、position、resolution 與 extend/duplicate state |
| Failure condition | 只有 active record、GPU aggregate mode 或 monitor model，沒有 desktop mapping |
| Rollback／cleanup requirement | 不修改設定；只清理 temporary inspection output，不建立 result root |
| Result artifact | Future `Environment record`；目前不建立 |
| Resulting readiness recommendation | `P1-GATE-005` 可由 reviewer 重新判定；不自動解除 `BA-004` |
| Owner | TBD |
| Status | `Specified` |
| Open questions | 可取得哪些不改設定的 desktop position/primary evidence？是否需另行指定單螢幕測試 session？ |

### 6.4 UI-ENABLE-004 — Per-monitor DPI evidence path

| Field | Value |
| --- | --- |
| Enablement ID | `UI-ENABLE-004` |
| Source Blocking Action | `BA-004` |
| Related P1 Gate | `P1-GATE-006` |
| Source evidence | `UI-PREQ-004`, `UI-PREQ-005`, `UI-BLOCK-003`, `UI-ENV-GAP-001`, `UI-CLOSURE-FIND-002` |
| Current unresolved condition | `LogPixels`、`PerMonitorSettings` 與 effective per-monitor DPI 未取得；`Win8DpiScaling=0` 不足以代表 effective DPI |
| Required final evidence | 每個 display path 的 effective DPI method、raw/effective value、scaling、timestamp 與 limitation |
| Proposed enablement operation | 建立不修改顯示設定的 DPI inspection procedure，先以既有 display state 建立 baseline |
| Operation type | `Read-only inspection`; future `Evidence capture` |
| Local system mutation required | No；不得修改 DPI 或 Display Settings |
| Network download required | No for inspection |
| Package restore required | No for inspection |
| Project creation required | No for inspection |
| Build required | No for inspection；future framework Spike separately |
| Runtime execution required | No for inspection；future UI Spike separately |
| Administrator privilege required | Unknown；不能由 registry visibility 推論 privilege |
| Human authorization required | Yes |
| Expected system effect | No display configuration change；取得可重現 DPI evidence |
| Success condition | DPI matrix 能區分 same-DPI 與 heterogeneous-DPI branch，且不依賴 physical size 推算 |
| Failure condition | 只有 global DPI、registry flag 或 monitor model，沒有 per-monitor effective value |
| Rollback／cleanup requirement | 不修改系統；清除 temporary inspection output，不建立 persistent setting |
| Result artifact | Future `Environment record`、`Failure reproduction`；目前不建立 |
| Resulting readiness recommendation | `P1-GATE-006` 由 reviewer 判定；HDR 不因本項自動納入 |
| Owner | TBD |
| Status | `Specified` |
| Open questions | 可用的 read-only effective DPI source 是哪一個？如何記錄 display path 與 scaling mapping？ |

### 6.5 UI-ENABLE-005 — Synthetic content/input isolation

| Field | Value |
| --- | --- |
| Enablement ID | `UI-ENABLE-005` |
| Source Blocking Action | `BA-005` |
| Related P1 Gate | `P1-GATE-007` |
| Source evidence | `UI-PREQ-003`, `UI-PREQ-009`, `UI-CLOSURE-FIND-006` |
| Current unresolved condition | 行為 checklist 已有文件，但沒有 synthetic content、pointer、keyboard artifact 或 runtime-independent closure evidence |
| Required final evidence | Fixed synthetic canvas、固定 pointer/keyboard sequence、focus restore/cancel record，且不讀取真實桌面 |
| Proposed enablement operation | 先審查 synthetic contract；未來若獲授權，再建立隔離 experimental project，不接 Capture API、Print Screen 或 Clipboard |
| Operation type | `Repository documentation mutation`; `Experimental project creation`; `Runtime execution`; future `Evidence capture` |
| Local system mutation required | Current: No；future isolated project/process: Yes |
| Network download required | Potentially Yes depending on selected framework；本文件不下載 |
| Package restore required | Potentially Yes；本文件不 restore |
| Project creation required | Yes for future runtime validation；本文件不建立 |
| Build required | Yes for future validation；本文件不 build |
| Runtime execution required | Yes for future validation；本文件不 run |
| Administrator privilege required | Unknown；不預先宣告 |
| Human authorization required | Yes，並需明確禁止真實畫面資料 |
| Expected system effect | Future process 只接收 synthetic input/content，不改產品資料、不寫入 Clipboard |
| Success condition | Pointer、focus、cancel 與 hit-testing 可在不讀取真實螢幕的條件下重現 |
| Failure condition | 需要 Capture API、Print Screen、真實桌面或 Clipboard 才能完成測試 |
| Rollback／cleanup requirement | 終止 process、清理 temporary project/output、確認沒有 shortcut/hook/Clipboard 殘留 |
| Result artifact | Future `Runtime result`、`Failure reproduction`、`Cleanup confirmation`；目前不建立 |
| Resulting readiness recommendation | 只提供 `P1-GATE-007` future evidence，不宣告產品 Capture feasible |
| Owner | TBD |
| Status | `Specified` |
| Open questions | Synthetic input 是否能涵蓋所有 Phase 1 pointer/focus 行為？如何證明沒有讀取真實畫面？ |

### 6.6 UI-ENABLE-006 — Evidence storage governance

| Field | Value |
| --- | --- |
| Enablement ID | `UI-ENABLE-006` |
| Source Blocking Action | `BA-006` |
| Related P1 Gate | `P1-GATE-008` |
| Source evidence | `UI-PREQ-012`, `UI-BLOCK-007`, `UI-ENV-GAP-009`, `UI-CLOSURE-FIND-006` |
| Current unresolved condition | Naming/storage policy 已定義；result root 不存在，owner、retention、operational review 與 future artifact boundary 未獲獨立核准 |
| Required final evidence | Approved root、owner、retention、metadata schema、sensitive-data review、artifact inventory 與 review record |
| Proposed enablement operation | 只審查本規格與 future storage boundary；未獲授權前不建立 `results/ui-framework/` |
| Operation type | `Repository documentation mutation`; future `Evidence capture` |
| Local system mutation required | Current: No；future result creation: Yes |
| Network download required | No for storage review |
| Package restore required | No |
| Project creation required | No for storage review |
| Build required | No for storage review |
| Runtime execution required | No for storage review |
| Administrator privilege required | Unknown；依 future storage boundary決定 |
| Human authorization required | Yes，需核准 owner、retention 與 sensitive-data policy |
| Expected system effect | Future evidence 與 product source tree 分離，並可追溯 cleanup/retention |
| Success condition | 每份 artifact 都有 Spike ID、Framework、version、Windows build、architecture、configuration、timestamp、type 與 attempt number |
| Failure condition | Evidence 進入 product source tree、缺 metadata、含敏感使用者資料或無法判定 owner/retention |
| Rollback／cleanup requirement | 依 approved retention 與 cleanup policy 移除 temporary artifacts，保留 deletion/cleanup record |
| Result artifact | Future `Evidence inventory`、`Environment metadata`、`Cleanup confirmation`；目前不建立 |
| Resulting readiness recommendation | `P1-GATE-008` 需 reviewer 核准後才可進入 future execution review |
| Owner | TBD |
| Status | `Specified` |
| Open questions | Storage owner 是誰？retention 多久？哪些 diagnostic data 必須先做 sensitive-data review？ |

### 6.7 UI-ENABLE-007 — Safety／cleanup acceptance

| Field | Value |
| --- | --- |
| Enablement ID | `UI-ENABLE-007` |
| Source Blocking Action | `BA-007` |
| Related P1 Gate | `P1-GATE-009` |
| Source evidence | `UI-PREQ-013`, `UI-BLOCK-008`, `UI-ENV-GAP-010`, `UI-CLOSURE-FIND-006` |
| Current unresolved condition | Safety／cleanup policy 存在，但尚無 Overlay、Focus、Topmost、interrupt、process 或 cleanup runtime acceptance |
| Required final evidence | Preflight checklist、forced termination record、focus/topmost restore、shortcut cleanup、process check、interruption recovery 與 cleanup confirmation |
| Proposed enablement operation | 先審查 safety/rollback procedure；未獲授權前不建立 Overlay、不執行 process、不觸碰 Focus/Topmost |
| Operation type | `Repository documentation mutation`; future `Runtime execution`; future `Evidence capture` |
| Local system mutation required | Current: No；future test process/focus/topmost: Yes |
| Network download required | No for safety review |
| Package restore required | No for safety review |
| Project creation required | No for policy review；future Spike may require |
| Build required | No for policy review；future Spike may require |
| Runtime execution required | Yes for future cleanup acceptance；本文件不 run |
| Administrator privilege required | Unknown；不得預先宣告 |
| Human authorization required | Yes，且需明確 stop/rollback boundary |
| Expected system effect | Future failure or interruption leaves no test process, focus/topmost, shortcut or temporary artifact residue |
| Success condition | 每個 stop path 都能恢復 focus、移除 topmost、終止 process、清理 workspace 並留下 evidence |
| Failure condition | Process 殘留、shortcut 持續存在、focus/topmost 未恢復、或 cleanup 前切換另一 Framework |
| Rollback／cleanup requirement | Cleanup must run before next candidate；中斷時優先 cleanup，再保存 failure record |
| Result artifact | Future `Safety checklist`、`Failure reproduction`、`Cleanup confirmation`；目前不建立 |
| Resulting readiness recommendation | `P1-GATE-009` 仍需 future runtime acceptance，不由 policy 自動關閉 |
| Owner | TBD |
| Status | `Specified` |
| Open questions | 強制終止的 owner 與 escalation path 是什麼？如何在中斷時保證 cleanup record 可保存？ |

### 6.8 UI-ENABLE-008 — Independent Phase 1 authorization

| Field | Value |
| --- | --- |
| Enablement ID | `UI-ENABLE-008` |
| Source Blocking Action | `BA-008` |
| Related P1 Gate | `P1-GATE-010` |
| Source evidence | `UI-PREQ-014`, `UI-BLOCK-009`, `UI-ENV-GAP-011`, `UI-CLOSURE-FIND-007` |
| Current unresolved condition | 目前只有 read-only closure authorization，沒有獨立 Phase 1 execution authorization |
| Required final evidence | Independent review record、scope、stop rules、evidence boundary、requested operations、human approver、authorization date/expiry 與 explicit decision |
| Proposed enablement operation | 將前 7 個 Enablement Item 的 operation/impact/rollback 彙整給人工 reviewer；本文件不提出或批准 authorization |
| Operation type | `Repository documentation mutation`; future `Read-only inspection`; future authorization review |
| Local system mutation required | Current: No；future operations依 reviewer scope |
| Network download required | Depends on approved enablement item；Current: No |
| Package restore required | Depends on approved enablement item；Current: No |
| Project creation required | Depends on approved enablement item；Current: No |
| Build required | Depends on approved enablement item；Current: No |
| Runtime execution required | Depends on approved enablement item；Current: No |
| Administrator privilege required | Unknown until an operation is specifically approved |
| Human authorization required | Yes，必須是獨立且可追溯的人工授權 |
| Expected system effect | Current: none；future effect 只能限於 approved scope |
| Success condition | Reviewer 明確核准或拒絕每一類操作，並固定 `Current authorization`、scope、expiry、rollback 與 stop rules |
| Failure condition | 以文件、ChatGPT 回報或前一份 Review 推論出 authorization，或把 `Not ready` 改寫成 execution permission |
| Rollback／cleanup requirement | Authorization record 必須包含 revoke、expiry、rollback owner 與 cleanup evidence requirement |
| Result artifact | Future `Authorization record`；目前不建立 |
| Resulting readiness recommendation | 只決定是否可提交人工審查，不選擇 framework、不授權 Runtime Spike |
| Owner | TBD |
| Status | `Specified` |
| Open questions | 誰是獨立 reviewer？authorization scope 是否逐項或分批？有效期限與 revoke procedure 為何？ |

## 7. Operation Classification Summary

| Enablement Item | Read-only inspection | Documentation mutation | Installation | Package acquisition | Project creation | Build | Runtime | System mutation | Evidence capture |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `UI-ENABLE-001` | Yes | No | Potential | Potential | Potential | Potential | Potential | No current | Future only |
| `UI-ENABLE-002` | Yes | Yes | Potential | Potential | Potential | Potential | Future only | No current | Future only |
| `UI-ENABLE-003` | Yes | No | No | No | No | No | No current | No | Future record only |
| `UI-ENABLE-004` | Yes | No | No | No | No | No | No current | No | Future record only |
| `UI-ENABLE-005` | No current | Yes | Potential | Potential | Future | Future | Future | Future process only | Future only |
| `UI-ENABLE-006` | No current | Yes | No | No | No | No | No | Future storage only | Future only |
| `UI-ENABLE-007` | No current | Yes | No | No | Future only | Future only | Future | Future process/focus only | Future only |
| `UI-ENABLE-008` | No current | Yes | No current | No current | No current | No current | No current | No current | No current |

`Potential` 只表示 future specification 依賴，不表示本文件已請求、已批准或可執行。

## 8. Experimental Toolchain Baseline

### 8.1 Candidate matrix

| Candidate | Experimental framework | Runtime／SDK | Build path | Local status | Missing capability | Mutation required |
| --- | --- | --- | --- | --- | --- | --- |
| WinUI 3 | Windows App SDK stable candidate `2.3.1` | Local `Microsoft.WindowsAppRuntime.2` `2.3.1.0` observed；SDK package/template unknown | Primary: existing `.NET CLI`／SDK-only path if candidate project supports it；Fallback: existing Visual Studio stable or Build Tools/MSBuild if later observed | Runtime partial；SDK/build path blocked | SDK package、template、candidate build provenance、Build verified | Future package/project/build may mutate local state；none now |
| WPF | .NET Desktop Runtime `10.0.10`；.NET SDK `10.0.302` | Windows Desktop Runtime observed；Windows SDK files observed | Primary: existing `.NET CLI`／SDK-only path；Fallback: existing Visual Studio stable or Build Tools/MSBuild if later observed | Runtime and SDK partial；Build verified `No` | Equivalent build path、project provenance、Build verification | Future project/build may mutate local state；none now |

### 8.2 Build path policy

- `Primary experimental build path` 與 `Fallback experimental build path` 是 future execution proposal，不是產品工具鏈 ADR。
- Primary path 優先使用現有 local evidence；不得因路徑不存在而自動安裝工具。
- Fallback path 只有在 existing installation evidence 被 read-only inspection 證明後才能提出。
- WinUI Runtime package、SDK package、template、MSBuild 與 IDE 必須分開記錄。
- `Build verified` 在本文件固定為 `No`。
- WPF local evidence 較完整，不構成選用 WPF；WinUI Runtime package 存在，也不構成 WinUI build path ready。

## 9. Candidate Parity Rules

未來 WinUI 3 與 WPF 必須：

- 使用相同 CPU architecture。
- 使用相同 Windows baseline。
- 使用相同 Build configuration。
- 使用相同 Synthetic content。
- 使用相同 Pointer／Focus 行為範圍。
- 使用相同 Evidence naming 與 metadata。
- 使用相同 cold-start／warm-start 分類。
- 不加入 Capture Backend、Clipboard 或正式 Annotation Tool。
- 不把其中一方的 Prototype 程式碼直接作為另一方的比較基準。
- 不把 future experimental result 寫成產品 runtime fact。

## 10. Display and DPI Enablement Baseline

| Environment ID | Display path | Physical device evidence | Resolution | Position | Primary | Scaling／DPI | HDR | Availability |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `ENV-UI-001` | Single-display x64 baseline | Host x64；active display record observed | Per-display resolution Unknown | Unknown | Unknown | Unknown | Unknown | `Partially available` |
| `ENV-UI-002` | Multi-display baseline | 3 active PnP/EDID records observed | Per-display mapping Unknown | Unknown | Unknown | Unknown | Unknown | `Partially available` |
| `ENV-UI-003` | Same-DPI branch | No effective DPI evidence | Unknown | Unknown | Unknown | Unknown | Deferred | `Unknown` |
| `ENV-UI-004` | Heterogeneous-DPI branch | No proof that active records have different effective DPI | Unknown | Unknown | Unknown | Unknown | Deferred | `Unknown` |
| `ENV-UI-005` | HDR optional branch | HDR capability/state not obtained | Unknown | Unknown | Unknown | Unknown | `Deferred` | `Deferred` |

Enablement rules：

- `Unknown` 是未取得資料的狀態，不得以 monitor model、EDID physical size 或 record count 推算。
- `ENV-UI-001` 是 future Phase 1 首批最低 baseline 候選；它仍需完成 read-only mapping。
- `ENV-UI-002` 至 `ENV-UI-004` 可分批，但多螢幕與 heterogeneous DPI 不得永久從 UI feasibility 移除。
- HDR 在 non-HDR branch 可 `Deferred`；若重新納入，必須另行授權並取得 per-display evidence。
- 本文件不修改 Windows Display Settings。

## 11. Synthetic Input and Content Contract

本節只定義未來實驗的 synthetic input/content，不建立實作、不讀取真實桌面、不保存產品圖片。

| Contract element | Future fixed definition |
| --- | --- |
| Test canvas | 1024 × 768 logical units；只存在於 isolated experimental process |
| Background | 固定純色背景，不使用 desktop pixels |
| Contrast content | 四個固定高對比色塊，位置與尺寸在 run metadata 固定 |
| Selection rectangle | 初始位置 `(128, 96)`、尺寸 `160 × 120` logical units |
| Pointer sequence | Move to start → press → drag to end → release → cancel branch |
| Focus sequence | Enter experiment → focus target → cancel → restore focus → exit |
| Hit-testing object | 固定矩形 object；不連結產品 Annotation model |
| Keyboard sequence | 固定 cancel key、confirm key 與 focus-restore branch；不註冊永久 global shortcut |
| Prohibited input | Real desktop pixels、Print Screen hook、Capture API、Clipboard、產品圖片 |
| Acceptance | 相同 input sequence 在兩個 candidate 產生可比較的 state/event evidence |

## 12. Evidence Artifact Specification

Future result root 只做規劃，不能在本文件建立：

`docs/Research/Technology/results/ui-framework/`

### 12.1 Artifact types

| Artifact type | Required future content | Created now? |
| --- | --- | --- |
| Environment record | Host、Windows build、architecture、GPU、display、DPI、HDR、toolchain observation | No |
| Build record | Candidate、framework version、build path、configuration、command provenance、outcome | No |
| Runtime result | Spike ID、candidate、input sequence、observed window/focus/pointer outcome | No |
| Diagnostic log | Tool/version、timestamp、scope、failure or observation | No |
| Measurement data | Method、units、run count、environment、timestamp、limitations | No |
| Screenshot | Future synthetic-only visual evidence if separately authorized；不得包含 real desktop content | No |
| Screen recording | Future synthetic-only recording if separately authorized；不得包含 user content | No |
| Failure reproduction | Preconditions、steps、observed failure、repeatability、environment | No |
| Cleanup confirmation | Process、focus、topmost、shortcut、temporary directory、artifact cleanup result | No |

### 12.2 Naming and metadata

Future artifact naming must include：

`UI-SPIKE-NNN-framework-baseline-environment-run-evidence-type.ext`

Each artifact must carry：

- Spike ID。
- Framework and framework version。
- Windows build and CPU architecture。
- Build configuration。
- Environment ID。
- Timestamp and timezone。
- Evidence type。
- Execution attempt number。
- Outcome and limitation。
- Sensitive-data review result。

Evidence must remain outside product source tree and must not contain real desktop pixels, user content, secrets or credentials.

## 13. Safety and Rollback Specification

### 13.1 Preflight

- Confirm separate candidate work directory。
- Confirm current authorization scope、expiry、owner 與 stop rule。
- Confirm no previous candidate process、Focus、Topmost 或 global shortcut remains。
- Confirm no result directory mutation is outside approved storage boundary。
- Confirm synthetic-only input/content contract。

### 13.2 During execution

- Any unexpected focus、Topmost、process、input、display or evidence behavior triggers stop。
- 不得在 cleanup 完成前切換另一個 Framework。
- 不得把 failure recovery 省略成 generic success。
- 不得讀取或保存 real desktop pixels。

### 13.3 Rollback and cleanup

- Forced termination path must be defined before runtime execution。
- Clear Focus/Topmost state and unregister temporary shortcuts。
- Confirm no test process remains。
- Remove temporary project/output/package delta according to approved scope。
- Preserve failure reproduction and cleanup confirmation before deleting evidence。
- Re-check candidate isolation before the next run。

本節只定義 procedure，不執行任何 process、Focus、Topmost、shortcut、cleanup 或 rollback。

## 14. Authorization Request Matrix

| Enablement Item | Operation classifications | Mutation level | Requested authorization | Current authorization | Execution permitted |
| --- | --- | --- | --- | --- | --- |
| `UI-ENABLE-001` | Read-only; package; project; build; runtime | High if future install/restore/project/build is approved | `Required before execution` | `Not granted` | `No` |
| `UI-ENABLE-002` | Documentation; read-only; project; build | Medium to high for future parity project/build | `Required before execution` | `Not granted` | `No` |
| `UI-ENABLE-003` | Read-only inspection | None; no display mutation | `Required before execution` | `Not granted` | `No` |
| `UI-ENABLE-004` | Read-only inspection; future evidence capture | None; no DPI mutation | `Required before execution` | `Not granted` | `No` |
| `UI-ENABLE-005` | Documentation; project; build; runtime; evidence | High for future isolated process/project/evidence | `Required before execution` | `Not granted` | `No` |
| `UI-ENABLE-006` | Documentation; future evidence capture | Medium for future result/artifact creation | `Required before execution` | `Not granted` | `No` |
| `UI-ENABLE-007` | Documentation; future runtime; evidence | High for future process/focus/topmost/cleanup test | `Required before execution` | `Not granted` | `No` |
| `UI-ENABLE-008` | Documentation; authorization review | None in current document; governs other operations | `Required before execution` | `Not granted` | `No` |

本表不是 authorization request，也不是 approval。`Requested authorization` 只表示執行前必須取得的人工核准類型。

## 15. Enablement Completeness Matrix

| Blocking Action | Enablement Item | Specification complete | Required authorization identified | Evidence destination identified | Remaining specification gap |
| --- | --- | --- | --- | --- | --- |
| `BA-001` | `UI-ENABLE-001` | Yes | Yes | Yes, future approved root | Local SDK/template/build provenance and no-install fallback |
| `BA-002` | `UI-ENABLE-002` | Yes | Yes | Yes, future approved root | Candidate parity and equivalent build-path evidence |
| `BA-003` | `UI-ENABLE-003` | Yes | Yes | Yes, future Environment record | Per-display resolution/position/primary mapping |
| `BA-004` | `UI-ENABLE-004` | Yes | Yes | Yes, future Environment record | Effective per-monitor DPI method and matrix |
| `BA-005` | `UI-ENABLE-005` | Yes | Yes | Yes, future Runtime/Failure record | Synthetic-only artifact and input isolation proof |
| `BA-006` | `UI-ENABLE-006` | Yes | Yes | Yes, future approved root | Owner, retention, sensitive-data and operational review |
| `BA-007` | `UI-ENABLE-007` | Yes | Yes | Yes, future Safety/Cleanup record | Runtime interruption and cleanup acceptance |
| `BA-008` | `UI-ENABLE-008` | Yes | Yes | Yes, future Authorization record | Independent reviewer, expiry and revoke procedure |

`Specification complete = Yes` 只代表本文件已描述 enablement operation；不代表 source evidence 已關閉、不代表 build/runtime 已驗證、不代表 authorization 已取得。

## 16. Final Enablement Status

本文件的 final enablement status：`Conditionally ready to request Phase 1 execution authorization`。

這個結論只表示 8 個操作的規格、授權類型、證據目的地與 rollback boundary 已整理到可以提交人工審查；它不改變 `RESEARCH-TECH-UI-007` 的 `Readiness Decision: Not ready`，也不代表任何操作可執行。

| Decision field | Value |
| --- | --- |
| Final Enablement Status | `Conditionally ready to request Phase 1 execution authorization` |
| Current authorization | `Not granted` |
| Execution permitted | `No` |
| Runtime Spike Execution Authorized | `No` |
| Build verified | `No` |
| Framework selected | None |
| Product source code created | No |
| Result directory created | No |
| Screenshot or recording created | No |

若任何人工 reviewer 認定 8 個 Item 中有一項 specification 不足，Final Enablement Status 必須降為 `Not ready to request Phase 1 execution authorization`；不得自行改成 Ready 或 Conditionally ready。

## 17. Traceability

### 17.1 Enablement chain

```text
BA-001..BA-008
  -> UI-ENABLE-001..UI-ENABLE-008
    -> Required authorization
      -> Future enablement execution evidence
        -> Phase 1 execution authorization review
```

### 17.2 Repository references

- [UI Framework Runtime Spike Execution Readiness](03-ui-framework-runtime-spike-execution-readiness.md)
- [UI Framework Runtime Environment Baseline](04-ui-framework-runtime-environment-baseline.md)
- [UI Framework Runtime Prerequisite Closure Plan](05-ui-framework-runtime-prerequisite-closure-plan.md)
- [UI Framework Phase 1 Prerequisite Closure Record](06-ui-framework-phase1-prerequisite-closure-record.md)
- [UI Framework Phase 1 Readiness Reassessment](07-ui-framework-phase1-readiness-reassessment.md)
- [ADR-0002: UI Framework Selection](../../../Architecture/adr/ADR-0002-ui-framework-selection.md)
- [Technology Decision Roadmap](../../../Architecture/TECHNOLOGY-DECISION-ROADMAP.md)

### 17.3 Completion boundary

本文件完成後仍然：

- 不得建立 `RESEARCH-TECH-UI-009` 或其他新文件，除非下一份任務明確要求。
- 不得把 `Conditionally ready to request` 解讀為 `Execution permitted`。
- 不得修改 `RESEARCH-TECH-UI-007` 的 Readiness Decision。
- 不得把 future artifact specification 當作 actual runtime evidence。
- 不得把 planned Screenshot/Recording artifact 當作已完成的截圖功能。

### 允許的最小同步更新

主要交付物：

- `docs/Research/Technology/08-ui-framework-phase1-execution-enablement-specification.md`

允許最小更新：

- `docs/Research/Technology/README.md`
- `docs/Research/README.md`
- `docs/index.md`
- `CHANGELOG.md`
- `TODO.md`

同步更新只能新增文件連結、Draft 狀態與待 Review 項目，不得修改 `RESEARCH-TECH-UI-003` 至 `RESEARCH-TECH-UI-007`、ADR、PRD、Specs 或 Architecture。

## 18. Prohibited Actions for This Task

- 不安裝、下載、更新或移除工具、SDK、Runtime、Workload 或 Package。
- 不建立 Project、Solution、Prototype、Result directory 或 Source Code。
- 不執行 Restore、Build、Run、Publish、Deployment、Runtime Spike、Performance Test 或 Accessibility Test。
- 不建立 Screenshot、Screen recording、Capture hook、Overlay 或真實螢幕資料管線。
- 不修改 Registry、Display setting、DPI、HDR、shortcut 或系統狀態。
- 不修改 `RESEARCH-TECH-UI-003` 至 `RESEARCH-TECH-UI-007`、ADR、PRD、Specs 或 Architecture。
