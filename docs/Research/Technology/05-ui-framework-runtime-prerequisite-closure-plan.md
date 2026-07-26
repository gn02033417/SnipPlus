# UI Framework Runtime Prerequisite Closure Plan

本文件規劃如何以最小、可追溯且需要明確授權的方式，關閉 UI Framework Runtime Spike 的前置條件、阻塞項目與環境缺口。本文件不執行任何 Closure Action，也不授權 Runtime Spike。

## Document Control

| Field | Value |
| --- | --- |
| Document ID | `RESEARCH-TECH-UI-005` |
| Title | UI Framework Runtime Prerequisite Closure Plan |
| Status | `Draft` |
| Research Type | Prerequisite Closure Plan |
| Execution Status | `Not started` |
| Runtime Verification | `Not performed` |
| Parent Readiness Record | `RESEARCH-TECH-UI-003` |
| Environment Baseline | `RESEARCH-TECH-UI-004` |
| Owner | TBD |
| Last reviewed | Not reviewed |
| Version | 0.1 |
| Plan date | 2026-07-26 |
| Normative references | `RESEARCH-TECH-UI-003`, `RESEARCH-TECH-UI-004`, `Architecture/adr/ADR-0002-ui-framework-selection.md` |
| Informative references | `RESEARCH-TECH-UI-001`, `RESEARCH-TECH-UI-002`, `Architecture/TECHNOLOGY-DECISION-ROADMAP.md` |
| Supersedes | None |
| Superseded by | None |

## 1. Purpose, Scope and Non-goals

### 1.1 Purpose

本文件的目的：

- 將 `RESEARCH-TECH-UI-003` 與 `RESEARCH-TECH-UI-004` 的每個 Gap、Prerequisite、Blocker 與 Spike 建立可追溯的 Closure Action。
- 區分 Phase 1、Phase 2、Phase 3 必須先關閉的條件，避免把後續階段的條件錯誤地當成 Phase 1 的前置條件。
- 明確記錄所需證據、授權邊界、失敗 fallback 與預期狀態變化。
- 為未來的 Readiness Reassessment 提供可檢查的入口條件。

### 1.2 Scope

本文件只涵蓋：

- Windows Build 26200 的版本來源與 servicing channel 證據。
- 實體螢幕拓樸、逐螢幕解析度、排列方式與 Per-monitor DPI 證據。
- HDR capability/state 的分支條件。
- WinUI 3、Windows App SDK、WPF、.NET、MSBuild 與 Windows SDK 的本機可用性辨識。
- Phase 1 的 Visual Studio、Build Tools 或 CLI 建置路徑選項，僅列方案，不執行。
- Accessibility、WPT、timing、evidence storage、naming 與 cleanup 的準備條件。
- ARM64、Packaging、Clean-machine 的 Phase 3 範圍與 Deferred 規則。
- 對 `UI-ENV-GAP-001` 至 `UI-ENV-GAP-011`、`UI-PREQ-001` 至 `UI-PREQ-014`、`UI-BLOCK-001` 至 `UI-BLOCK-009` 與 `UI-SPIKE-001` 至 `UI-SPIKE-011` 的完整 impact mapping。

### 1.3 Non-goals

本文件不得：

- 安裝、移除或更新任何工具、SDK、Runtime、Workload 或 Framework。
- 建立 Project、Solution、Prototype、Result Artifact 或 Source Code。
- 執行 Build、Restore、Publish、Runtime Spike、Performance Test、Accessibility Test 或 Deployment Test。
- 修改 `ADR-0002`、`RESEARCH-TECH-UI-003`、`RESEARCH-TECH-UI-004`、Frozen PRD、Specs 或 Architecture。
- 宣告 Phase 1 Ready、選定 WinUI 3 或 WPF、決定正式產品版本。
- 選擇 Capture、Rendering、Clipboard 或 Annotation Backend。
- 建立任何截圖功能、截圖 hook、Capture API 或真實螢幕資料管線。
- 以本文件自我授權任何 Closure Action 或 Runtime Spike。

## 2. Plan and Status Vocabulary

### 2.1 Phase classification

每個來源項目只能使用下列分類：

- `Phase 1 Blocking`：會阻止 Windowing Feasibility，或直接影響 Phase 1 所需的可重現環境、等價行為、證據與安全條件。
- `Phase 2 Blocking`：只在 Interaction and Rendering Feasibility 開始前必須關閉。
- `Phase 3 Blocking`：只在 Delivery Feasibility、Packaging、Architecture distribution 或 Accessibility delivery 開始前必須關閉。
- `Cross-phase`：所有相關 Spike 都需要，或是執行授權、證據保存與安全清理等共用條件。
- `Deferred`：對目前 Phase 沒有必要，且透過上游 traceability 明確延後到指定 Phase。
- `Not applicable`：經來源文件與 Spike mapping 證明與該項目無關。

Phase classification 是規劃分類，不會自動改變上游狀態。

### 2.2 Action status

Closure Action 只能使用：

- `Planned`：已定義方法，但尚未開始，也沒有執行證據。
- `Blocked`：需要工具、環境、變更授權或獨立審查，目前不能開始。
- `Deferred`：依 traceability 延至後續 Phase，不影響目前 Phase 的最小門檻。
- `Not applicable`：由上游關係確認不適用。
- `Completed`：只有對應證據與 Review 已存在時才能使用；本文件沒有任何 Action 使用此狀態。

### 2.3 Target status vocabulary

本文件的 target status 只提出建議，不直接修改上游文件。建議值為：

- `Resolved`：對應證據已被獨立審查並足以關閉該項目。
- `Keep Open`：證據仍不足，來源項目保持開啟。
- `Keep Blocked`：前置條件尚未具備，不能授權下游工作。
- `Deferred`：已明確移至後續 Phase，且不阻止目前 Phase。
- `Pending Review`：資料已備妥但尚未經獨立 Review。

## 3. Phase Classification Rules

### 3.1 No unrelated gate expansion

Phase 1 不得等待與 Windowing Feasibility 無直接關係的 ARM64、Packaging、Clean-machine 或完整 HDR coverage。這些條件只有在以下情況才會影響 Phase 1：

- `UI-SPIKE-003` 或 `UI-SPIKE-004` 明確選擇 HDR 分支作為 Phase 1 的必要案例。
- 上游 Review 明確把某一個顯示條件列為 Phase 1 的比較基線。
- 沒有替代的非 HDR、x64、single-host 基線可以回答所定義的 Phase 1 問題。

ARM64、Packaged、Unpackaged 與 Clean-machine 預設屬於 Phase 3；不得為了提前關閉它們而擴大 Phase 1 scope。

### 3.2 Phase 1 blocking rule

Phase 1 的 Closure Action 必須只處理下列最小集合：

- Framework、Windows App SDK、WPF、.NET、MSBuild 與 Windows SDK 的精確實驗版本。
- x64 Windows 基線與可重現的 Windows Build、架構、GPU、顯示拓樸紀錄。
- 逐螢幕解析度、排列方式與必要的 Per-monitor DPI 證據。
- 等價 Prototype Behavior Baseline 的審查狀態；不建立 Prototype。
- 合成輸入與不接觸真實螢幕內容的邊界。
- 最低 Focus、Pointer、DPI 證據方式。
- Evidence storage、naming、metadata、safety 與 cleanup 規則。
- 明確的獨立 Review 與 execution authorization。

### 3.3 Phase 2 and Phase 3 rule

- Phase 2 才可處理高頻 Pointer、Selection rendering、Hit Testing 與 timing evidence 的具體 workload；不得把它們提前變成產品實作。
- Phase 3 才可處理 ARM64、Packaging、Unpackaged、Clean-machine、Deployment Artifact 與 Accessibility delivery evidence。
- HDR 只在有明確 HDR 測試分支時成為對應 Spike 的 Blocking 條件；非 HDR 的 Phase 1 案例不可被無限期阻塞。

## 4. Minimum Phase 1 Gate

開始 Windowing Feasibility 前，至少必須具備以下證據或明確審查結果：

1. WinUI 3／Windows App SDK 與 WPF／.NET 的精確實驗版本已記錄，且每個版本有官方來源、查核日期與本機可用性欄位。
2. 至少有一條可辨識、可重現且已被明確選擇的建置路徑；該路徑不得以 `MSBuild` 單一命令可見性推定完整 Visual Studio 或 Build Tools 安裝。
3. x64 Windows 基線包含 Edition、Build、架構、CPU、RAM、GPU、Driver 與證據日期。
4. 實體顯示器拓樸至少能區分顯示器識別、解析度、排列、Primary 狀態與資料來源；不能只用 WMI record count 推定實體螢幕數。
5. Phase 1 所需的 Per-monitor DPI 案例已可重現，或已明確將非必要 DPI 分支延後並記錄理由。
6. 等價 Prototype Behavior Baseline、合成輸入規則與禁止真實 Capture／Clipboard／正式 Output 的邊界已通過 Review；不代表 Prototype 已建立。
7. Evidence storage、naming、metadata、敏感資料排除與 artifact retention 規則已通過 Review；不建立結果目錄。
8. Overlay safety、Focus restore、Topmost cleanup、termination 與 interruption handling checklist 已通過 Review；不代表 runtime cleanup 已驗證。
9. 必要的 diagnostic／timing evidence 方法已指定到工具與版本層級；工具存在不等於 measurement 已執行。
10. `RESEARCH-TECH-UI-003` 與本文件已完成獨立 Review，並取得針對 Phase 1 的明確 execution authorization。

以上條件未全部滿足時，Phase 1 建議狀態維持 `Not ready`，任何 Spike 的 `Execution authorized` 維持 `No`。

## 5. Closure Action Register

下列 Action 只定義未來如何關閉條件，不代表已執行。所有 Action 的 Owner 目前均為 `TBD`，沒有任何 Action 可使用 `Completed`。

### UI-CLOSE-001 — 固定實驗版本與官方來源

- Source: `UI-ENV-GAP-006`, `UI-PREQ-001`, `UI-BLOCK-001`
- Affected Spikes: `UI-SPIKE-001`–`UI-SPIKE-011`
- Phase classification: `Phase 1 Blocking`
- Required evidence: Framework、Windows App SDK、WPF、.NET、Windows SDK、Build Tool 的精確版本、官方來源、查核日期與本機 availability。
- Proposed action: 建立一份版本 baseline record，將 official stable version、installed version、candidate for spike 與 product decision 分開。
- Read-only inspection possible: Yes, only after a separate explicit authorization for the inspection task。
- Installation or system mutation required: No for inspection; Yes if缺少候選工具而要補裝。
- Human authorization required: Yes。
- Success condition: 每個 Phase 1 framework candidate 都有可核對的精確版本與本機狀態。
- Failure／fallback: 缺資料就維持 `Blocked`；不得用官方版本代替本機可用性。必要時保留 WPF／WinUI candidate 為未授權狀態。
- Resulting status recommendation: `Pending Review`，證據不足時 `Keep Blocked`。
- Owner: TBD
- Status: `Blocked`
- Open questions: WinUI 3 與 Windows App SDK 的本機 package availability 是否可在不 restore、不安裝的條件下取得？

### UI-CLOSE-002 — 確認 Windows Build 26200 來源與 servicing channel

- Source: `UI-ENV-GAP-011`, `UI-PREQ-002`, `UI-BLOCK-002`
- Affected Spikes: `UI-SPIKE-001`–`UI-SPIKE-011`
- Phase classification: `Phase 1 Blocking`
- Required evidence: Windows Edition、Version、Build、servicing channel、更新來源與查核日期。
- Proposed action: 以唯讀系統資訊建立 host baseline，並在 future result metadata 中固定記錄 Build 來源。
- Read-only inspection possible: Yes, after explicit authorization。
- Installation or system mutation required: No for inspection; any servicing change is outside this plan。
- Human authorization required: Yes。
- Success condition: 可重現地識別測試 host，且不把特殊 Build 結果泛化為所有 Windows 11。
- Failure／fallback: 保留 `Open`；若 servicing channel 無法確認，限制結論範圍並不得授權跨環境推論。
- Resulting status recommendation: `Pending Review` 或 `Keep Open`。
- Owner: TBD
- Status: `Blocked`
- Open questions: Build 26200 的正式 servicing 定位與本次 Spike 可泛化範圍為何？

### UI-CLOSE-003 — 確認實體螢幕拓樸與解析度

- Source: `UI-ENV-GAP-002`, `UI-PREQ-002`, `UI-PREQ-004`, `UI-PREQ-005`, `UI-BLOCK-002`, `UI-BLOCK-003`
- Affected Spikes: `UI-SPIKE-003`, `UI-SPIKE-004`, `UI-SPIKE-007`–`UI-SPIKE-009`
- Phase classification: `Phase 1 Blocking`
- Required evidence: 每個實體顯示器的識別、桌面解析度、位置、Primary 狀態、連接關係與查核日期。
- Proposed action: 建立可重現的單螢幕與多螢幕 topology record；將 WMI active record 與實體設備證據分開。
- Read-only inspection possible: Yes, after explicit authorization。
- Installation or system mutation required: No, unless另有設備配置需求；設備配置不在本文件。
- Human authorization required: Yes。
- Success condition: 不再用 record count 推定實體螢幕，且 Phase 1 所需解析度案例可重現。
- Failure／fallback: `UI-ENV-GAP-002` 維持 `Open`，相應 Spike 維持 `Blocked`。
- Resulting status recommendation: `Pending Review` 或 `Keep Open`。
- Owner: TBD
- Status: `Blocked`
- Open questions: 目前 3 個 WMI active records 是否對應 3 個實體螢幕，以及各自的 desktop mode 與位置為何？

### UI-CLOSE-004 — 關閉 Per-monitor DPI 證據缺口

- Source: `UI-ENV-GAP-001`, `UI-PREQ-004`, `UI-PREQ-005`, `UI-BLOCK-003`
- Affected Spikes: `UI-SPIKE-003`, `UI-SPIKE-004`, `UI-SPIKE-007`–`UI-SPIKE-009`
- Phase classification: `Phase 1 Blocking`
- Required evidence: 每個顯示器的 Per-monitor DPI/scaling、DPI context、可重現的 monitor combination 與 evidence date。
- Proposed action: 只為 Phase 1 需要的 DPI cases 建立 evidence record；非必要的異質 DPI coverage 明確移至 Phase 2 或 Phase 3。
- Read-only inspection possible: Yes, after explicit authorization。
- Installation or system mutation required: No for inspection; changing user display settings is prohibited by this plan。
- Human authorization required: Yes。
- Success condition: Phase 1 DPI matrix 可被重現並能區分 same-DPI 與 heterogeneous-DPI。
- Failure／fallback: 未取得 per-monitor evidence 時維持 `Open`；不得從實體尺寸或單一 registry value 猜測。
- Resulting status recommendation: `Pending Review` 或 `Keep Blocked`。
- Owner: TBD
- Status: `Blocked`
- Open questions: Phase 1 minimum DPI matrix 是否只需要 current same-DPI baseline，還是必須包含異質 DPI branch？

### UI-CLOSE-005 — 定義 HDR 分支的必要性

- Source: `UI-ENV-GAP-003`, `UI-PREQ-007`, `UI-BLOCK-005`
- Affected Spikes: `UI-SPIKE-003`, `UI-SPIKE-004`, `UI-SPIKE-011`
- Phase classification: `Phase 1 Blocking` for an explicitly selected HDR branch; otherwise `Deferred`
- Required evidence: 每個相關顯示器的 HDR capability、current state、Windows setting evidence 與 test branch decision。
- Proposed action: 由 Review 明確決定 HDR 是否為 Phase 1 的必要比較案例；若不是，將 HDR coverage 保留為後續 scope。
- Read-only inspection possible: Yes, after explicit authorization。
- Installation or system mutation required: No for inspection; changing HDR settings is outside this plan。
- Human authorization required: Yes。
- Success condition: HDR branch 有明確必要性與可觀察 evidence；非必要分支不阻塞 non-HDR Phase 1。
- Failure／fallback: HDR 維持 `Open` 或 `Deferred`；不得把 Unknown 寫成 Off 或 Available。
- Resulting status recommendation: `Deferred` for non-HDR Phase 1，否則 `Keep Blocked`。
- Owner: TBD
- Status: `Blocked`
- Open questions: HDR 是否屬於目前 Framework windowing feasibility 的必要驗證範圍？

### UI-CLOSE-006 — 確認 WinUI 3／Windows App SDK 本機可用性

- Source: `UI-ENV-GAP-006`, `UI-PREQ-001`, `UI-BLOCK-001`
- Affected Spikes: `UI-SPIKE-001`–`UI-SPIKE-011`
- Phase classification: `Phase 1 Blocking`
- Required evidence: 不安裝、不 restore 前提下可取得的 package/workload/toolchain availability；若不可取得，必須記錄為 Unknown。
- Proposed action: 針對現有環境做最小、唯讀的可用性確認；缺少 evidence 就不建立 project，也不補裝依賴。
- Read-only inspection possible: Yes, after explicit authorization。
- Installation or system mutation required: No for availability inspection; Yes for missing package installation, which is outside this plan。
- Human authorization required: Yes。
- Success condition: WinUI 3／Windows App SDK candidate 可與 exact version record 對應，且不把官方下載頁當作本機證據。
- Failure／fallback: 維持 `Blocked`；若只有官方版本，僅可記為 Candidate for spike。
- Resulting status recommendation: `Pending Review` 或 `Keep Blocked`。
- Owner: TBD
- Status: `Blocked`
- Open questions: 本機是否已有可用 package cache、SDK reference 或其他受控來源？

### UI-CLOSE-007 — 確認 WPF、.NET SDK 與 MSBuild 的來源區分

- Source: `UI-PREQ-001`, `UI-PREQ-011`, `UI-BLOCK-001`, `UI-BLOCK-007`
- Affected Spikes: `UI-SPIKE-001`–`UI-SPIKE-011`
- Phase classification: `Phase 1 Blocking`
- Required evidence: .NET SDK、Windows Desktop Runtime、MSBuild、Visual Studio／Build Tools 的版本、來源、RID、architecture 與可用性。
- Proposed action: 建立 WPF candidate 的 toolchain evidence，並將 `.NET SDK MSBuild` 與 Visual Studio／Build Tools 分開記錄；僅提出 CLI、VS 或 Build Tools 的待選路徑。
- Read-only inspection possible: Yes, after explicit authorization。
- Installation or system mutation required: No for inspection; installing a toolchain is outside this plan。
- Human authorization required: Yes。
- Success condition: 建置路徑的每一個必要元件都能被證據支持，沒有從單一 `MSBuild` version 推定完整 IDE。
- Failure／fallback: WPF candidate 可保持 `Blocked`；不得宣告 WPF Prototype 可建置或執行。
- Resulting status recommendation: `Pending Review` 或 `Keep Blocked`。
- Owner: TBD
- Status: `Blocked`
- Open questions: 目前 `MSBuild 18.6.11` 的實際來源與 WPF 建置所需元件是否完整？

### UI-CLOSE-008 — 確認 Windows SDK 完整工具鏈能力

- Source: `UI-ENV-GAP-007`, `UI-PREQ-001`, `UI-PREQ-011`, `UI-BLOCK-001`, `UI-BLOCK-007`
- Affected Spikes: `UI-SPIKE-001`–`UI-SPIKE-011`
- Phase classification: `Phase 1 Blocking`
- Required evidence: headers、libraries、manifest、signing、packaging、build tools、architecture support 與 exact SDK version。
- Proposed action: 以既有 SDK evidence 區分 Include tree 與完整 toolchain capability；不安裝或修改 SDK。
- Read-only inspection possible: Yes, after explicit authorization。
- Installation or system mutation required: No for inspection; any SDK installation is outside this plan。
- Human authorization required: Yes。
- Success condition: Phase 1 所需的 SDK capability 與版本可核對；不把 Include directory 當作完整 SDK。
- Failure／fallback: `UI-BLOCK-001` 與 `UI-BLOCK-007` 維持 `Open`。
- Resulting status recommendation: `Pending Review` 或 `Keep Open`。
- Owner: TBD
- Status: `Blocked`
- Open questions: Phase 1 windowing candidate 實際需要哪些 Windows SDK components？

### UI-CLOSE-009 — 固定 Accessibility inspection 路徑

- Source: `UI-ENV-GAP-008`, `UI-PREQ-010`, `UI-BLOCK-007`
- Affected Spikes: `UI-SPIKE-010`, `UI-SPIKE-011`
- Phase classification: `Phase 3 Blocking`
- Required evidence: 工具名稱、版本、安裝狀態、inspection method、target scope 與 evidence naming。
- Proposed action: 將 Accessibility tool 保持為 Phase 3 prerequisite；在取得獨立授權前不安裝、不執行 inspection。
- Read-only inspection possible: Yes, after explicit authorization。
- Installation or system mutation required: Maybe; installation is a separate authorized task。
- Human authorization required: Yes。
- Success condition: 有可重複的 inspection tool 與方法，且不將 Accessibility guidance 誤寫為已完成檢測。
- Failure／fallback: Phase 3 維持 `Not ready`；Phase 1 不因未選定 Accessibility tool 而擴大 scope。
- Resulting status recommendation: `Deferred` for Phase 1，`Keep Blocked` for Phase 3。
- Owner: TBD
- Status: `Deferred`
- Open questions: Phase 3 的 Accessibility acceptance evidence 需要哪一種工具與最低檢查範圍？

### UI-CLOSE-010 — 固定 WPT／timing evidence 方法

- Source: `UI-PREQ-011`, `UI-BLOCK-005`, `UI-BLOCK-007`
- Affected Spikes: `UI-SPIKE-005`, `UI-SPIKE-007`–`UI-SPIKE-011`
- Phase classification: `Phase 2 Blocking`
- Required evidence: tool/version、measurement procedure、units、run metadata、failure handling 與 identical method for both candidates。
- Proposed action: 先建立 measurement method record；不執行 trace、timing collection 或 KPI threshold 定義。
- Read-only inspection possible: Yes, after explicit authorization。
- Installation or system mutation required: No for documenting available WPT; tracing may have separate operational effects and requires approval。
- Human authorization required: Yes。
- Success condition: WinUI 3 與 WPF candidate 使用同一可重複的 measurement procedure。
- Failure／fallback: 保持 `Blocked`；工具存在但方法未審查時不得宣告 timing ready。
- Resulting status recommendation: `Pending Review` 或 `Keep Blocked`。
- Owner: TBD
- Status: `Blocked`
- Open questions: 哪些 Phase 2 measurement 是決策必要證據，哪些只屬於非阻塞診斷資料？

### UI-CLOSE-011 — 審查 Evidence storage 與 naming

- Source: `UI-ENV-GAP-009`, `UI-PREQ-012`, `UI-BLOCK-007`
- Affected Spikes: `UI-SPIKE-001`–`UI-SPIKE-011`
- Phase classification: `Cross-phase`
- Required evidence: 儲存邊界、命名規則、metadata schema、retention、敏感資料排除、外部結果目錄與 cleanup owner。
- Proposed action: 只審查規則與路徑，不建立 result directory、不產生 screenshot、recording 或 diagnostic artifact。
- Read-only inspection possible: Yes, after explicit authorization。
- Installation or system mutation required: No for documentation review。
- Human authorization required: Yes。
- Success condition: 每一個 future result 都能由 Spike、framework、baseline、environment、run 與 outcome 追溯。
- Failure／fallback: `UI-ENV-GAP-009` 維持 `Open`；未完成 review 前不得產生實際結果。
- Resulting status recommendation: `Pending Review` 或 `Keep Blocked`。
- Owner: TBD
- Status: `Blocked`
- Open questions: evidence 目錄的正式 owner、retention 期限與外部 storage boundary 為何？

### UI-CLOSE-012 — 審查 Overlay safety 與 cleanup procedure

- Source: `UI-ENV-GAP-010`, `UI-PREQ-013`, `UI-BLOCK-008`
- Affected Spikes: `UI-SPIKE-001`–`UI-SPIKE-011`
- Phase classification: `Cross-phase`
- Required evidence: termination、Focus restore、Topmost restore、interruption、process cleanup、temporary artifact cleanup 與 rollback checklist。
- Proposed action: 先完成文件審查與中斷情境清單；不建立 Overlay、不執行 runtime cleanup test。
- Read-only inspection possible: Yes, after explicit authorization。
- Installation or system mutation required: No for review; runtime testing is a separate authorized action。
- Human authorization required: Yes。
- Success condition: 安全清單可被 future runner 使用，且每個中斷情境都有可觀察的 cleanup outcome。
- Failure／fallback: 維持 `Open`；沒有 runtime evidence 就不能標示 `Resolved`。
- Resulting status recommendation: `Pending Review` 或 `Keep Open`。
- Owner: TBD
- Status: `Blocked`
- Open questions: 需要哪些既有系統狀態快照，才能證明 Focus、Topmost 與 temporary state 已恢復？

### UI-CLOSE-013 — 明確處理 ARM64 範圍

- Source: `UI-ENV-GAP-004`, `UI-PREQ-006`, `UI-BLOCK-004`
- Affected Spikes: `UI-SPIKE-010`, `UI-SPIKE-011`
- Phase classification: `Phase 3 Blocking`
- Required evidence: 真實 ARM64 device／VM scope、Windows version、toolchain、architecture、display與 deployment record；若不提供，需有明確 Deferred decision。
- Proposed action: 在 Phase 1 不等待 ARM64；於 Phase 3 readiness review 決定提供設備或正式 Deferred。
- Read-only inspection possible: Yes, only if an ARM64 environment already exists and receives explicit authorization。
- Installation or system mutation required: Maybe; creating or provisioning ARM64 environment is outside this plan。
- Human authorization required: Yes。
- Success condition: ARM64 coverage either has a reproducible environment record or is explicitly Deferred with affected Spike scope。
- Failure／fallback: `UI-BLOCK-004` remains `Open`; x64 evidence cannot substitute for ARM64。
- Resulting status recommendation: `Deferred` for Phase 1，`Keep Blocked` for Phase 3 until decided。
- Owner: TBD
- Status: `Deferred`
- Open questions: ARM64 是否是目前產品支援範圍，或只屬於 Phase 3 的可選分支？

### UI-CLOSE-014 — 明確處理 Packaged、Unpackaged 與 Clean-machine

- Source: `UI-ENV-GAP-005`, `UI-PREQ-008`, `UI-BLOCK-006`
- Affected Spikes: `UI-SPIKE-011`
- Phase classification: `Phase 3 Blocking`
- Required evidence: Packaged／Unpackaged mode、deployment artifact、startup result、clean-machine boundary、standard-user/admin context 與 cleanup record。
- Proposed action: 將 delivery test path 保留至 Phase 3；不建立 Project、package、deployment artifact 或 clean-machine environment。
- Read-only inspection possible: No current artifact exists; future inspection requires explicit authorization。
- Installation or system mutation required: Yes for actual deployment testing; this plan does not authorize it。
- Human authorization required: Yes。
- Success condition: 兩種模式與 clean-machine boundary 都有受控、可回溯的測試路徑，或被明確 Deferred。
- Failure／fallback: `UI-BLOCK-006` 維持 `Open`；不得用 current host 的單一命令可見性代替 deployment evidence。
- Resulting status recommendation: `Deferred` for Phase 1，`Keep Blocked` for Phase 3。
- Owner: TBD
- Status: `Deferred`
- Open questions: Phase 3 是否必須同時涵蓋 Packaged、Unpackaged 與 Clean-machine，或可分批授權？

### UI-CLOSE-015 — 取得獨立 Review 與 execution authorization

- Source: `UI-ENV-GAP-011`, `UI-PREQ-014`, `UI-BLOCK-009`
- Affected Spikes: `UI-SPIKE-001`–`UI-SPIKE-011`
- Phase classification: `Cross-phase`
- Required evidence: `RESEARCH-TECH-UI-003` review、本文件 review、已關閉的 Phase-specific Blocking items 與明確書面 authorization。
- Proposed action: 待所有 Phase 1 Blocking Action 有證據後，建立獨立 Readiness Reassessment；本文件不得自我核准。
- Read-only inspection possible: No authorization record exists yet。
- Installation or system mutation required: No; authorization is a governance action。
- Human authorization required: Yes, explicitly。
- Success condition: 審查者、範圍、允許的 Phase、允許的 Spike 與禁止的行為均被記錄。
- Failure／fallback: 所有 `Execution authorized` 維持 `No`；`UI-BLOCK-009` 維持 `Open`。
- Resulting status recommendation: `Pending Review` 或 `Keep Open`。
- Owner: TBD
- Status: `Blocked`
- Open questions: 哪位 owner 或 reviewer 可以針對 Phase 1 做獨立授權？

## 6. Full Impact Matrix

本矩陣覆蓋 `RESEARCH-TECH-UI-004` 的 11 個 Environment Gap、`RESEARCH-TECH-UI-003` 的 14 個 Prerequisite、9 個 Blocker 與 11 個 Spike。所有 target status 都是建議，不直接修改來源文件。

### 6.1 Environment Gaps

| Source item | Phase | Closure Action | Required evidence | Current status | Target status |
| --- | --- | --- | --- | --- | --- |
| `UI-ENV-GAP-001` | Phase 1 Blocking | `UI-CLOSE-004` Per-monitor DPI | Per-monitor DPI matrix and monitor combinations | Open | Keep Blocked |
| `UI-ENV-GAP-002` | Phase 1 Blocking | `UI-CLOSE-003` Display topology | Per-display resolution, position and primary record | Open | Pending Review |
| `UI-ENV-GAP-003` | Phase 1 Blocking for HDR branch; otherwise Deferred | `UI-CLOSE-005` HDR branch decision | HDR capability/state and scope decision | Open | Deferred or Keep Blocked |
| `UI-ENV-GAP-004` | Phase 3 Blocking | `UI-CLOSE-013` ARM64 scope | ARM64 environment or explicit Deferred record | Open | Deferred or Keep Blocked |
| `UI-ENV-GAP-005` | Phase 3 Blocking | `UI-CLOSE-014` Packaging path | Packaged/Unpackaged/Clean-machine evidence | Open | Deferred or Keep Blocked |
| `UI-ENV-GAP-006` | Phase 1 Blocking | `UI-CLOSE-001`, `UI-CLOSE-006` Version and local availability | Exact versions and local package/tool availability | Open | Keep Blocked |
| `UI-ENV-GAP-007` | Phase 1 Blocking | `UI-CLOSE-007`, `UI-CLOSE-008` Toolchain and SDK | Source-separated toolchain and SDK capability record | Open | Keep Blocked |
| `UI-ENV-GAP-008` | Phase 3 Blocking | `UI-CLOSE-009` Accessibility path | Tool/version and repeatable inspection method | Open | Deferred or Keep Blocked |
| `UI-ENV-GAP-009` | Cross-phase | `UI-CLOSE-011` Evidence storage | Approved storage, naming, metadata and retention | Open | Keep Open until Review |
| `UI-ENV-GAP-010` | Cross-phase | `UI-CLOSE-012` Safety and cleanup | Reviewed interruption and cleanup checklist | Open | Keep Open until runtime evidence |
| `UI-ENV-GAP-011` | Cross-phase | `UI-CLOSE-015` Authorization | Independent Review and explicit authorization | Open | Keep Open |

### 6.2 Prerequisites

| Source item | Phase | Closure Action | Required evidence | Current status | Target status |
| --- | --- | --- | --- | --- | --- |
| `UI-PREQ-001` | Phase 1 Blocking | `UI-CLOSE-001`, `UI-CLOSE-006`, `UI-CLOSE-007`, `UI-CLOSE-008` | Exact framework, SDK, runtime and toolchain baseline | Blocked | Keep Blocked until complete |
| `UI-PREQ-002` | Phase 1 Blocking | `UI-CLOSE-002`, `UI-CLOSE-003` | Reproducible x64 Windows and display record | Blocked | Pending Review |
| `UI-PREQ-003` | Phase 1 Blocking | Existing behavior baseline review; no new Prototype | Approved equivalent behavior checklist | Blocked | Keep Blocked until Review |
| `UI-PREQ-004` | Phase 1 Blocking | `UI-CLOSE-003`, `UI-CLOSE-004` | Resolution and DPI matrix | Blocked | Keep Blocked |
| `UI-PREQ-005` | Phase 1 Blocking for heterogeneous-DPI branch | `UI-CLOSE-003`, `UI-CLOSE-004` | Reproducible multi-monitor heterogeneous-DPI record | Blocked | Keep Blocked or Deferred by branch |
| `UI-PREQ-006` | Phase 3 Blocking | `UI-CLOSE-013` | ARM64 device and architecture record | Blocked | Deferred or Keep Blocked |
| `UI-PREQ-007` | Phase 1 Blocking for HDR branch; otherwise Deferred | `UI-CLOSE-005` | HDR capability/state and branch decision | Blocked | Deferred or Keep Blocked |
| `UI-PREQ-008` | Phase 3 Blocking | `UI-CLOSE-014` | Controlled packaged/unpackaged test path | Blocked | Deferred or Keep Blocked |
| `UI-PREQ-009` | Phase 1 Blocking | Existing synthetic input baseline review | Approved synthetic input definition; no real Capture | Blocked | Keep Blocked until Review |
| `UI-PREQ-010` | Phase 3 Blocking | `UI-CLOSE-009` | Accessibility tool and repeatable inspection method | Blocked | Deferred or Keep Blocked |
| `UI-PREQ-011` | Phase 2 Blocking | `UI-CLOSE-007`, `UI-CLOSE-010` | Same measurement method for both candidates | Blocked | Keep Blocked |
| `UI-PREQ-012` | Cross-phase | `UI-CLOSE-011` | Storage boundary, naming and metadata | Blocked | Keep Open until Review |
| `UI-PREQ-013` | Cross-phase | `UI-CLOSE-012` | Safety, termination and cleanup evidence | Blocked | Keep Open until runtime evidence |
| `UI-PREQ-014` | Cross-phase | `UI-CLOSE-015` | Independent Review and authorization | Blocked | Keep Blocked |

### 6.3 Blockers

| Source item | Phase | Closure Action | Required evidence | Current status | Target status |
| --- | --- | --- | --- | --- | --- |
| `UI-BLOCK-001` | Phase 1 Blocking | `UI-CLOSE-001`, `UI-CLOSE-006`, `UI-CLOSE-007`, `UI-CLOSE-008` | Exact version and local toolchain evidence | Open | Keep Open |
| `UI-BLOCK-002` | Phase 1 Blocking | `UI-CLOSE-002`, `UI-CLOSE-003` | Windows servicing and reproducible display baseline | Open | Keep Open |
| `UI-BLOCK-003` | Phase 1 Blocking | `UI-CLOSE-003`, `UI-CLOSE-004` | Resolution, topology and DPI evidence | Open | Keep Open |
| `UI-BLOCK-004` | Phase 3 Blocking | `UI-CLOSE-013` | ARM64 record or explicit Deferred decision | Open | Deferred or Keep Open |
| `UI-BLOCK-005` | Phase 1 HDR branch or Phase 2 measurement | `UI-CLOSE-005`, `UI-CLOSE-010` | HDR branch and timing method evidence | Open | Deferred or Keep Open |
| `UI-BLOCK-006` | Phase 3 Blocking | `UI-CLOSE-014` | Packaging and deployment capability | Open | Deferred or Keep Open |
| `UI-BLOCK-007` | Cross-phase | `UI-CLOSE-009`, `UI-CLOSE-010`, `UI-CLOSE-011` | Accessibility, diagnostics and evidence chain | Open | Keep Open |
| `UI-BLOCK-008` | Cross-phase | `UI-CLOSE-012` | Reviewed safety and cleanup procedure | Open | Keep Open |
| `UI-BLOCK-009` | Cross-phase | `UI-CLOSE-015` | Independent Review and authorization | Open | Keep Open |

### 6.4 Spikes

| Source item | Phase | Closure Action | Required evidence | Current status | Target status |
| --- | --- | --- | --- | --- | --- |
| `UI-SPIKE-001` | Phase 1 | `UI-CLOSE-001`, `002`, `006`, `007`, `011`, `012`, `015` | Version, x64 host, behavior, evidence and safety records | Blocked | Not authorized |
| `UI-SPIKE-002` | Phase 1 | `UI-CLOSE-001`, `002`, `006`, `007`, `011`, `012`, `015` | Same baseline as `UI-SPIKE-001` with composition scope | Blocked | Not authorized |
| `UI-SPIKE-003` | Phase 1 | `UI-CLOSE-001`–`005`, `007`, `010`–`012`, `015` | Version, display, DPI, HDR branch, timing and safety records | Blocked | Not authorized |
| `UI-SPIKE-004` | Phase 1 | `UI-CLOSE-001`–`005`, `007`, `010`–`012`, `015` | Heterogeneous-DPI and optional HDR evidence | Blocked | Not authorized |
| `UI-SPIKE-005` | Phase 2 | `UI-CLOSE-001`, `002`, `007`, `010`–`012`, `015` | Identical timing method and evidence storage | Blocked | Not authorized |
| `UI-SPIKE-006` | Phase 1 | `UI-CLOSE-001`, `002`, `003`, `007`, `011`, `012`, `015` | Focus lifecycle behavior baseline and cleanup review | Blocked | Not authorized |
| `UI-SPIKE-007` | Phase 2 | `UI-CLOSE-001`, `002`, `003`, `004`, `007`, `010`–`012`, `015` | Pointer, DPI and measurement evidence | Blocked | Not authorized |
| `UI-SPIKE-008` | Phase 2 | `UI-CLOSE-001`, `002`, `003`, `004`, `007`, `010`–`012`, `015` | Selection rendering workload and same evidence method | Blocked | Not authorized |
| `UI-SPIKE-009` | Phase 2 | `UI-CLOSE-001`, `002`, `003`, `004`, `007`, `010`–`012`, `015` | Hit testing workload and same evidence method | Blocked | Not authorized |
| `UI-SPIKE-010` | Phase 3 | `UI-CLOSE-001`, `002`, `007`–`013`, `015` | Architecture, Accessibility, deployment and safety records | Blocked | Not authorized |
| `UI-SPIKE-011` | Phase 3 | `UI-CLOSE-001`, `002`, `005`, `007`–`015` | Packaging, ARM64, HDR, Accessibility and deployment evidence | Blocked | Not authorized |

## 7. Authorization Boundary

| Action type | 是否可直接執行 |
| --- | --- |
| 唯讀系統查詢 | 待後續明確核准 |
| 官方版本查核 | 待後續明確核准 |
| 審查既有文件與索引 | 只限本文件與必要索引的靜態文件工作 |
| 建立或修改 Project／Solution | 禁止 |
| 安裝／更新 Framework、SDK、Runtime、Workload 或 Tool | 禁止，需個別明確授權 |
| 修改系統設定、顯示設定或 servicing state | 禁止 |
| 建立 Prototype | 禁止 |
| Build／Restore／Publish | 禁止 |
| Runtime Spike | 禁止 |
| Screenshot、Screen recording 或真實 Capture | 禁止 |
| Accessibility inspection | 禁止 |
| Deployment、Packaging 或 Clean-machine test | 禁止 |
| 修改 `ADR-0002` 或上游 readiness record | 禁止，本文件只能提出建議 |
| Phase 1 execution authorization | 必須由獨立 Review 明確授予 |

## 8. Recommended Closure Order

建議未來依下列順序另行授權，每一階段都必須保留未完成項目，不得自動跳到下一步：

1. 先審查版本、Windows Build、toolchain 與 Windows SDK 的來源區分：`UI-CLOSE-001`、`002`、`006`、`007`、`008`。
2. 再建立必要的 x64 host 與實體顯示拓樸、解析度、DPI evidence：`UI-CLOSE-003`、`004`。
3. 由 Review 決定 HDR 是否屬於 Phase 1 必要分支：`UI-CLOSE-005`。
4. 審查等價 behavior、synthetic input、Evidence storage、naming、metadata、safety 與 cleanup：對應 `UI-PREQ-003`、`UI-PREQ-009`、`UI-CLOSE-011`、`012`。
5. 固定 Phase 2 的 measurement method，並將其留在 Phase 2：`UI-CLOSE-010`。
6. 將 ARM64、Accessibility、Packaging、Unpackaged 與 Clean-machine 明確保留在 Phase 3 或建立 Deferred decision：`UI-CLOSE-009`、`013`、`014`。
7. 由獨立 reviewer 建立 Readiness Reassessment，完成 `UI-CLOSE-015`；沒有此步驟不得開始任何 Spike。

## 9. Readiness Reassessment Entry Criteria

只有下列條件全部成立，才可建立新的 Readiness Reassessment：

- 所有 Phase 1 Blocking Action 都有對應證據或明確、經 Review 的 branch-specific Deferred 理由。
- `UI-ENV-GAP-001` 至 `UI-ENV-GAP-011` 每一項都有 Closure Action、Phase classification 與 current status；沒有未分類的 Gap。
- `UI-PREQ-001` 至 `UI-PREQ-014` 與 `UI-BLOCK-001` 至 `UI-BLOCK-009` 全部被映射，且沒有被本文件直接改寫。
- Framework、SDK、Runtime 與 toolchain 的 exact version evidence 已分開記錄 official availability 與 local availability。
- x64 host、Windows Build、實體顯示拓樸、解析度與 Phase 1 所需 DPI evidence 已可重現。
- Build path 已由證據支持，但尚未因本文件而建立 Product Project 或開始產品建置。
- Equivalent behavior、synthetic input、evidence storage、safety 與 cleanup 規則已通過文件 Review；runtime behavior 仍不得宣稱已驗證。
- ARM64、HDR、Packaging、Accessibility、Clean-machine 等非必要 Phase 1 條件已被正確分類為後續 Phase 或 branch-specific requirement。
- `RESEARCH-TECH-UI-003` 與本文件都有獨立 Review，並有明確列出的 allowed scope、blocked actions 與 execution authorization。
- 所有 `UI-SPIKE-001` 至 `UI-SPIKE-011` 在新的 authorization record 建立前，`Execution authorized` 仍為 `No`。

## 10. Traceability

### 10.1 Repository references

- [UI Framework Feasibility](01-ui-framework-feasibility.md)
- [UI Framework Runtime Spike Plan](02-ui-framework-runtime-spike-plan.md)
- [UI Framework Runtime Spike Execution Readiness](03-ui-framework-runtime-spike-execution-readiness.md)
- [UI Framework Runtime Environment Baseline](04-ui-framework-runtime-environment-baseline.md)
- [ADR-0002: UI Framework Selection](../../../Architecture/adr/ADR-0002-ui-framework-selection.md)
- [Technology Decision Roadmap](../../../Architecture/TECHNOLOGY-DECISION-ROADMAP.md)

### 10.2 Traceability chain

`UI-ENV-GAP → UI-CLOSE → UI-PREQ / UI-BLOCK → UI-SPIKE → Phase readiness reassessment`

每個 Closure Action 必須能回溯到來源證據，並能向下說明受影響的 Spike；不得因完成一個 Action 就自動宣告上游項目 Resolved。新的 status 只能透過獨立 Review 寫回對應的上游文件。

### 10.3 Evidence separation

- Official source evidence：用於確認候選版本或官方定義，不代表本機安裝。
- Local environment evidence：用於確認本機目前可觀察狀態，不代表產品可建置或可部署。
- Runtime evidence：只有在另行授權並實際執行後才能建立；本文件沒有 Runtime evidence。
- Product decision evidence：由 ADR、PRD、Specs 與 Architecture governance 管理，本文件不取代它們。

## 11. Overall Plan Status

| Decision | Value | Meaning |
| --- | --- | --- |
| Closure plan status | `Closure plan complete` | 11 個 Gap、14 個 Prerequisite、9 個 Blocker 與 11 個 Spike 已有分類與 mapping。 |
| Prerequisite closure execution | `Not ready` | 沒有明確授權，也沒有開始執行任何 Closure Action。 |
| Phase 1 readiness | `Not ready` | 上游 `RESEARCH-TECH-UI-003` 仍為 `Not ready`。 |
| Runtime Spike authorization | `No` | 本文件不能自我授權。 |
| Framework decision | Not decided | `ADR-0002` 維持 `Draft`。 |

`Closure plan complete` 只表示規劃文件完成，不代表任何前置條件已關閉、工具已安裝、環境已補齊或 Spike 可以開始。

## 12. Completion Boundary

完成本文件不代表：

- `UI-ENV-GAP`、`UI-PREQ` 或 `UI-BLOCK` 已被上游文件標示為 Resolved。
- Phase 1、Phase 2 或 Phase 3 已 Ready。
- 任一 `UI-SPIKE` 已獲授權或已執行。
- WPF、WinUI 3 或 Windows App SDK 已被選定。
- Prototype、Project、Result、Capture、Screenshot、Clipboard 或 Annotation 程式碼已建立。
- ADR-0002 可以從 Draft 轉為 Accepted。
- 任何系統設定、工具、SDK、Runtime、Workload、Packaging 或部署狀態已被修改。

本文件的結果僅限於 Closure Plan；後續任何環境查詢、工具安裝、建置、執行、量測、Accessibility、Packaging、Deployment 或 Runtime Spike，都必須另開明確授權的工作。

### 允許的最小同步更新

主要交付物：

- `docs/Research/Technology/05-ui-framework-runtime-prerequisite-closure-plan.md`

必要索引更新只允許：

- `docs/Research/Technology/README.md`
- `docs/Research/README.md`
- `docs/index.md`
- `CHANGELOG.md`
- `TODO.md`

索引更新只能新增連結、`Draft` 狀態與待 Review 項目，不得修改上游 readiness、ADR、PRD、Specs 或 Architecture 的判定。

### Prohibited actions for this task

- 不安裝、移除或升級工具。
- 不建立 Project、Solution、Prototype、Result directory 或 Source Code。
- 不執行 Build、Restore、Publish、Runtime Spike、Performance、Accessibility 或 Deployment Test。
- 不建立 Screenshot、Screen recording、Capture hook 或真實螢幕資料。
- 不修改 `ADR-0002`、`RESEARCH-TECH-UI-003`、`RESEARCH-TECH-UI-004`、PRD、Specs 或 Architecture。

