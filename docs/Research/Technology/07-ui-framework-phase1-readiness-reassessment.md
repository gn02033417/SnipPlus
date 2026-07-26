# UI Framework Phase 1 Readiness Reassessment

本文件依據 `RESEARCH-TECH-UI-003` 至 `RESEARCH-TECH-UI-006` 的計畫、環境基線與唯讀 closure evidence，重新評估 Phase 1 Windowing Feasibility 是否具備進入「獨立執行授權審查」的條件。本文件是 Readiness Reassessment，不是執行授權，也不是 Runtime Spike 結果。

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `RESEARCH-TECH-UI-007` |
| Title | UI Framework Phase 1 Readiness Reassessment |
| Status | `Draft` |
| Research Type | Readiness Reassessment |
| Runtime Verification | `Not performed` |
| Runtime Spike Execution | `Not authorized` |
| Evidence Baseline | `RESEARCH-TECH-UI-006` |
| Owner | TBD |
| Last reviewed | Not reviewed |
| Version | 0.1 |
| Reassessment date | 2026-07-26 |
| Normative References | `RESEARCH-TECH-UI-003`, `RESEARCH-TECH-UI-004`, `RESEARCH-TECH-UI-005`, `RESEARCH-TECH-UI-006`, `Architecture/adr/ADR-0002-ui-framework-selection.md` |
| Informative References | `RESEARCH-TECH-UI-001`, `RESEARCH-TECH-UI-002`, `Architecture/TECHNOLOGY-DECISION-ROADMAP.md` |
| Supersedes | None |
| Superseded by | None |

## 2. Reassessment Scope

本次只重新評估 Phase 1 的進入條件：

- Windowing。
- Overlay composition 的可驗證前置條件。
- Focus lifecycle。
- Pointer input 的 synthetic isolation 前置條件。
- 基本顯示拓樸與 DPI。
- WinUI 3 與 WPF 的等價 experimental build path。
- Evidence storage、naming 與 metadata boundary。
- Safety、interruption recovery 與 cleanup readiness。
- 進入獨立 Phase 1 execution authorization review 的條件。

Phase 2 與 Phase 3 只在本文件中記錄依賴及 Deferred 狀態，不重新展開其執行準備度。ARM64、packaging、clean-machine deployment、完整 Accessibility 與非必要 HDR branch 不得被重新包裝成 Phase 1 的必要條件。

## 3. Non-goals and Authorization Boundary

本文件不得：

- 安裝或更新工具、SDK、Runtime、Workload 或 Package。
- 建立 Result directory、Project、Solution、Prototype、Overlay 或產品 source code。
- 執行 `restore`、`build`、`run`、`publish`、WPR trace、效能量測、Accessibility test 或 Deployment test。
- 執行任何 `UI-SPIKE-001` 至 `UI-SPIKE-011`。
- 建立 Screenshot、Screen recording、Capture hook 或真實螢幕資料管線。
- 修改 `RESEARCH-TECH-UI-003` 至 `RESEARCH-TECH-UI-006`、`ADR-0002`、PRD、Specs 或 Architecture。
- 選擇 WinUI 3 或 WPF。
- 由本文件自行授權後續執行。

固定結論：`Runtime Spike Execution Authorized: No`。

## 4. Evidence Acceptance Rules

### 4.1 Evidence Acceptance Status

Evidence acceptance status 只能使用：

- `Accepted`：evidence 的來源、範圍與限制足以支持該項 evidence claim。
- `Accepted with limitation`：evidence 可使用，但有明確限制；限制必須寫在 limitation 或 remaining condition。
- `Insufficient`：目前 evidence 不足以支持該項 claim。
- `Conflicting`：來源之間存在未解決衝突，不能直接作為 positive readiness evidence。
- `Deferred`：該 evidence claim 被明確移至 Phase 2/3 或非必要 branch。

### 4.2 Phase 1 Gate Result

Phase 1 Gate 的 `Result` 欄只能使用：

- `Satisfied`：現有 evidence 已滿足該 Gate 的 reassessment acceptance condition。
- `Partially satisfied`：已有可用 evidence，但仍有明確 limitation；不得當作 execution-ready。
- `Unsatisfied`：必要 evidence 尚不存在或現有證據不能支持該 Gate。
- `Deferred`：該條件被明確移至 Phase 2/3 或非必要 branch，且不阻塞 Phase 1 scope。

這兩套 vocabulary 不得交叉使用。`Accepted`、`Accepted with limitation`、`Insufficient` 與 `Conflicting` 只可描述 evidence acceptance；`Satisfied`、`Partially satisfied`、`Unsatisfied` 與 `Deferred` 才可放在 Phase 1 Gate 的 `Result` 欄。

### 4.3 Evidence precedence

1. 有查核日期的官方版本與支援資料，可關閉「版本身分」的識別問題，但不能證明本機已安裝或可建置。
2. 本機唯讀 inventory 可證明觀察到的主機、Runtime、檔案或 PATH 狀態，但不能把 package presence 推論成 SDK、template 或 build capability。
3. Policy 文件可以證明規則已定義，不能取代 Runtime evidence、成功操作 evidence 或 cleanup evidence。
4. 未執行 Build、Prototype、Runtime Spike 或測試時，不得宣告 framework feasible。
5. ARM64、Packaging、Clean-machine 與非必要 HDR 缺口可以 Deferred，但必須保留 reactivation condition。
6. 對互相衝突的來源，保留原 Finding，明確記錄判定權重與 limitation，不刪除較弱來源。

### 4.4 Evidence baseline

本文件不新增本機查詢；使用 `RESEARCH-TECH-UI-006` 的 `UI-CLOSE-EVID-001` 至 `UI-CLOSE-EVID-015`。因此：

- `Runtime Verification` 維持 `Not performed`。
- `Result root` 不存在，且本文件不建立它。
- 本機 observed value、官方 candidate value 與 future runtime result 維持分離。

## 5. Phase 1 Gate Reassessment

| Gate | Required evidence | Current evidence | Result | Remaining condition |
| --- | --- | --- | --- | --- |
| `P1-GATE-001` Windows x64 baseline | 可重現的 Windows edition/build、architecture 與 host identity | Windows 11 Pro、Build `26200`、DisplayVersion `25H2`、x64；官方 GA release-health evidence 支持 `26200.8875` 作為目前 Phase 1 host baseline | `Satisfied` | 保留 `ProductName = Windows 10 Pro` 與 BuildLabEx 差異為 compatibility metadata limitation；不可用於推論更廣的 servicing policy |
| `P1-GATE-002` WinUI 3 experimental version | 官方 candidate、local Runtime、SDK/package 與可辨識的 build path | 官方 Windows App SDK `2.3.1` 已查核；本機觀察到 `Microsoft.WindowsAppRuntime.2` `2.3.1.0`；SDK/template/build path 未證明 | `Partially satisfied` | 分別取得既有 SDK／template／build-path provenance，並由後續授權的驗證工作確認；不得以 Runtime package 關閉 blocker |
| `P1-GATE-003` WPF experimental version | 官方或明確 candidate、.NET Desktop Runtime、SDK 與可辨識的 build path | .NET SDK `10.0.302`、MSBuild `18.6.11` 與 Windows Desktop Runtime `10.0.10` 已觀察；WPF project build 未執行 | `Partially satisfied` | 取得可重現且與 WinUI 可比較的 build-path provenance；不得因 WPF 準備度較高直接選擇 WPF |
| `P1-GATE-004` Equivalent build path | 兩個 candidate 都有相同 acceptance boundary、版本記錄與可驗證 build path | 行為基線與版本規則已文件化；兩者均沒有 build verification | `Unsatisfied` | 未來須在獨立授權下建立等價驗證條件；本文件不建立 project 或執行 build |
| `P1-GATE-005` Display topology baseline | 每個 display 的 identity、resolution、position、primary 與 extend/duplicate 狀態 | PnP／EDID 觀察到 3 個 active records；desktop mapping、position、primary 與模式未完整取得 | `Partially satisfied` | 另行授權唯讀取得可重現的單螢幕 baseline 及多螢幕 branch |
| `P1-GATE-006` DPI evidence path | Per-monitor effective DPI 的可重現查核方法與 evidence 欄位 | `LogPixels`、`PerMonitorSettings` 未取得；`Win8DpiScaling=0` 不足以代表 effective DPI | `Unsatisfied` | 固定不修改顯示設定的 DPI evidence path，並取得至少一個可重現 baseline |
| `P1-GATE-007` Synthetic content / input isolation | 不依賴真實畫面資料的 synthetic content、pointer 與 keyboard input boundary | 行為 checklist 已定義，但沒有 Prototype、input artifact 或 runtime-independent closure evidence | `Unsatisfied` | 在獨立授權下確認 synthetic input contract；不得接觸真實螢幕內容 |
| `P1-GATE-008` Evidence storage and naming | approved path、命名、metadata、owner、retention 與 sensitive-data boundary | naming／storage policy 已確認；result root 不存在，owner／retention 尚未獨立 Review | `Partially satisfied` | 由 reviewer 核准 storage owner、retention 與 metadata；本文件不建立目錄 |
| `P1-GATE-009` Safety and cleanup | termination、focus、topmost、shortcut、process、interruption recovery checklist 及可測試條件 | safety／cleanup policy 已文件化；沒有 Overlay、Focus、Topmost 或 interruption runtime evidence | `Partially satisfied` | 完成獨立 safety review；執行前必須有可中斷 cleanup acceptance condition |
| `P1-GATE-010` Independent execution authorization | 針對 Phase 1 的獨立 Review 與明確 authorization record | 目前只有 read-only closure authorization；沒有 Runtime Spike authorization | `Unsatisfied` | 取得獨立 Review 與明確 Phase-specific authorization；本文件不得自我授權 |

## 6. Source Status Reassessment

以下只是本次建議，不會修改來源文件的實際狀態。

### 6.1 Prerequisites

| Source ID | New status recommendation | Evidence basis | Scope interpretation |
| --- | --- | --- | --- |
| `UI-PREQ-001` | `Partially satisfied` | `UI-CLOSE-EVID-001`–`004`, `009`, `010` | 官方 candidate、.NET 與 Runtime 有部分證據；WinUI SDK／template／build path 與完整 candidate parity 未證明 |
| `UI-PREQ-002` | `Satisfied` | `UI-CLOSE-EVID-006` | Windows 11 25H2 x64 baseline 可作為目前 reassessment host；metadata conflict 仍須標註 limitation |
| `UI-PREQ-003` | `Unsatisfied` | `UI-CLOSE-EVID-015` | Equivalent Overlay behavior 只有文件 baseline，沒有 runtime-independent synthetic artifact |
| `UI-PREQ-004` | `Unsatisfied` | `UI-CLOSE-EVID-007`, `008` | Display mapping 與 effective DPI matrix 不完整 |
| `UI-PREQ-005` | `Unsatisfied` | `UI-CLOSE-EVID-007` | Active record 數量不等於 heterogeneous DPI topology |
| `UI-PREQ-006` | `Deferred` | `UI-CLOSE-EVID-015` | ARM64 屬 Phase 3，x64 不替代 ARM64 |
| `UI-PREQ-007` | `Deferred` | `UI-CLOSE-EVID-007`, `008` | 非 HDR Phase 1 branch 可先不納入；HDR branch 需重新啟用條件 |
| `UI-PREQ-008` | `Deferred` | `UI-CLOSE-EVID-015` | Packaged／Unpackaged capability 屬 Phase 3，不阻塞 Windowing feasibility |
| `UI-PREQ-009` | `Unsatisfied` | `UI-CLOSE-EVID-015` | Synthetic Capture Input 尚未實際定義成可驗證 artifact |
| `UI-PREQ-010` | `Deferred` | `UI-CLOSE-EVID-012`, `015` | Accessibility completeness 屬 Phase 3；Phase 1 仍須保留日後 inspection boundary |
| `UI-PREQ-011` | `Partially satisfied` | `UI-CLOSE-EVID-005`, `011`, `012` | WPR／WPA／xperf 存在，但 comparable measurement procedure 尚未核准；Phase 1 基本 windowing 不先宣告 performance ready |
| `UI-PREQ-012` | `Partially satisfied` | `UI-CLOSE-EVID-013` | 命名與保存規則存在，但 owner、retention 與 operational review 未完成 |
| `UI-PREQ-013` | `Partially satisfied` | `UI-CLOSE-EVID-014` | Safety／cleanup policy 已存在，但沒有 runtime proof |
| `UI-PREQ-014` | `Unsatisfied` | `UI-CLOSE-EVID-015` | 只有 read-only closure authorization，沒有 Phase 1 execution authorization |

### 6.2 Blockers

| Source ID | New status recommendation | Reassessment | Phase 1 impact |
| --- | --- | --- | --- |
| `UI-BLOCK-001` | `Open` | WinUI SDK/build path 與 candidate parity 未證明 | Blocks P1-GATE-002、004 |
| `UI-BLOCK-002` | `Accepted limitation` | Windows 11 25H2 x64 baseline 可接受；registry metadata conflict 保留 | 不再以 host identity 本身阻塞 Phase 1，但限制結論範圍 |
| `UI-BLOCK-003` | `Open` | Display position、primary、resolution mapping、DPI 未完成 | Blocks P1-GATE-005、006 |
| `UI-BLOCK-004` | `Deferred` | ARM64 明確移至 Phase 3 | Does not block Phase 1 |
| `UI-BLOCK-005` | `Deferred` | 非 HDR branch 不需要 HDR evidence；HDR branch 仍可 reactivation | Does not block non-HDR Phase 1 branch |
| `UI-BLOCK-006` | `Deferred` | Packaging path 移至 Phase 3 | Does not block Windowing feasibility |
| `UI-BLOCK-007` | `Open` | Evidence operation、Accessibility boundary 與 measurement procedure 不完整 | Blocks P1-GATE-008、009 and future execution boundary |
| `UI-BLOCK-008` | `Open` | Safety／cleanup 只有 policy，沒有可測試 runtime acceptance | Blocks P1-GATE-009 |
| `UI-BLOCK-009` | `Open` | 沒有獨立 Review 與 Phase-specific authorization | Blocks P1-GATE-010 and all Runtime Spike execution |

### 6.3 Environment gaps

| Source ID | New status recommendation | Reassessment |
| --- | --- | --- |
| `UI-ENV-GAP-001` | `Open` | Effective per-monitor DPI 未取得；保留為 Phase 1 blocker |
| `UI-ENV-GAP-002` | `Open` | Per-display resolution／desktop mapping 未取得；保留為 Phase 1 blocker |
| `UI-ENV-GAP-003` | `Deferred` | HDR branch 非必要時不阻塞；重新啟用條件必須記錄 |
| `UI-ENV-GAP-004` | `Deferred` | ARM64 僅影響 Phase 3 distribution coverage |
| `UI-ENV-GAP-005` | `Deferred` | Packaged／Unpackaged 與 clean-machine 屬 Phase 3 |
| `UI-ENV-GAP-006` | `Open` | Runtime package 不足以關閉 WinUI SDK/build-path gap |
| `UI-ENV-GAP-007` | `Open` | Visual Studio／Build Tools provenance 與等價 build path 未完成 |
| `UI-ENV-GAP-008` | `Deferred` | Accessibility inspection completeness 屬 Phase 3；不得宣告已完成 |
| `UI-ENV-GAP-009` | `Open` | Storage policy 尚未經 owner／retention review，且 result root 不存在 |
| `UI-ENV-GAP-010` | `Open` | Safety／cleanup 僅為 policy evidence |
| `UI-ENV-GAP-011` | `Open` | Read-only authorization 不等於 execution authorization |

### 6.4 Closure actions and findings

| Source ID | New status recommendation | Reassessment |
| --- | --- | --- |
| `UI-CLOSE-001` | `Partially resolved` | Official candidate 與部分 local Runtime evidence 可用；WinUI SDK／build path 未證明 |
| `UI-CLOSE-002` | `Accepted limitation` | Windows 11 25H2 x64 可作為 baseline；registry metadata conflict 保留 |
| `UI-CLOSE-003` | `Keep blocked` | Display topology mapping 尚不足以進入 comparison |
| `UI-CLOSE-004` | `Keep blocked` | Effective per-monitor DPI 尚未取得 |
| `UI-CLOSE-005` | `Deferred` | HDR 非必要 Phase 1 branch 延後 |
| `UI-CLOSE-006` | `Partially resolved` | Windows App Runtime package 存在，但 SDK／template／build path 未證明 |
| `UI-CLOSE-007` | `Keep blocked` | .NET SDK MSBuild 存在，但等價 WPF build path 未驗證 |
| `UI-CLOSE-008` | `Partially resolved` | Windows SDK tool files 存在，但 PATH 與 successful operation 未驗證 |
| `UI-CLOSE-009` | `Deferred` | Accessibility tool completeness 延至 Phase 3 |
| `UI-CLOSE-010` | `Keep blocked` | WPT presence 不等於 comparable measurement procedure |
| `UI-CLOSE-011` | `Pending review` | Storage／naming policy 存在，但 owner、retention 與 operational review 未完成 |
| `UI-CLOSE-012` | `Keep open` | Safety／cleanup policy 存在，但沒有 runtime proof |
| `UI-CLOSE-013` | `Deferred` | ARM64 延至 Phase 3 |
| `UI-CLOSE-014` | `Deferred` | Packaging、unpackaged 與 clean-machine path 延至 Phase 3 |
| `UI-CLOSE-015` | `Keep blocked` | 仍只有 read-only authorization |
| `UI-CLOSURE-FIND-001` | `Accepted limitation` | 官方 GA evidence 提升 Windows 11 25H2 baseline 權重；registry conflict 不刪除，改列 non-blocking compatibility metadata finding |
| `UI-CLOSURE-FIND-002` | `Open` | Display topology incomplete，仍是 Phase 1 blocker |
| `UI-CLOSURE-FIND-003` | `Open` | .NET SDK MSBuild 不是完整 IDE/toolchain proof |
| `UI-CLOSURE-FIND-004` | `Open` | Runtime package 不代表 SDK／template／build path |
| `UI-CLOSURE-FIND-005` | `Open` | SDK tool file presence、PATH availability、successful operation 必須分開 |
| `UI-CLOSURE-FIND-006` | `Open` | Policy 不取代 evidence artifact 或 cleanup runtime proof |
| `UI-CLOSURE-FIND-007` | `Open` | 沒有獨立 Phase 1 execution authorization |

## 7. Windows Identity Finding Reassessment

`UI-CLOSURE-FIND-001` 不刪除，建議從原本的 blocking interpretation 調整為 `Accepted limitation`：

- `UI-CLOSE-EVID-006` 的 CIM、registry 與 local build metadata 仍然保留。
- 官方 Windows 11 release-health evidence 將 host 的 `26200.8875` 視為 Windows 11 25H2 General Availability Channel 的有效 baseline evidence。
- 因此 Windows 11 25H2 x64 可作為 Phase 1 reassessment baseline；不是只靠單一 Build number 猜測。
- `ProductName = Windows 10 Pro` 與 `BuildLabEx` 的差異仍須保留為 compatibility metadata limitation。
- 官方 GA release-health evidence 的判定權重高於單一 registry `ProductName` 欄位，但不抹除 registry discrepancy，也不擴張成完整 servicing/support policy 結論。

這項調整只影響 host identity 的 readiness interpretation，不會解除 display、toolchain、evidence、safety 或 authorization blockers。

## 8. Toolchain Readiness Reassessment

| Candidate | Runtime available | SDK available | Build path identified | Build verified | Readiness |
| --- | --- | --- | --- | --- | --- |
| WinUI 3 | `Partially satisfied` — `Microsoft.WindowsAppRuntime.2` `2.3.1.0` observed | `Unsatisfied` — local SDK/package/template proof absent | `Unsatisfied` | `Unsatisfied` | `Not ready` |
| WPF | `Satisfied` — Windows Desktop Runtime `10.0.10` observed | `Partially satisfied` — .NET SDK `10.0.302` and Windows SDK evidence | `Partially satisfied` — .NET SDK MSBuild exists, IDE/Build Tools provenance incomplete | `Unsatisfied` | `Not ready` |

這張表只描述 evidence readiness，不是 framework selection。WPF 的 local evidence 較完整，不構成直接採用 WPF 的決策；WinUI 的 Runtime package 也不構成 WinUI build path 已就緒的證明。

## 9. Display Readiness Reassessment

目前只可建立以下邊界：

- 單螢幕 baseline 是 Phase 1 的最低可行起點，但目前尚未取得完整 primary、resolution、position 與 effective DPI evidence。
- 多螢幕與 heterogeneous DPI 是 Phase 1 首批必要的 comparison branch，至少要保留一個可重現的多螢幕／DPI 分支；不能用「有三個 active records」代替 topology proof。
- Per-monitor DPI 必須先確認 evidence path 與 effective value 的取得方式；不可由 `Win8DpiScaling=0`、EDID physical size 或 monitor model 推算。
- HDR capability/state 可在非 HDR Phase 1 branch `Deferred`；若 scope 重新納入 HDR，必須 reactivation 並另行取得 per-display evidence。
- Display gaps 若未補足，`P1-GATE-005` 與 `P1-GATE-006` 維持 `Partially satisfied`／`Unsatisfied`，不得宣告 Overlay framework feasible。

## 10. Deferred Scope Register

| Condition | Target phase | Reason | Reactivation condition | Blocks Phase 1? |
| --- | --- | --- | --- | --- |
| ARM64 | Phase 3 | Current host is x64；沒有 ARM64 device evidence | 真實 ARM64 environment 或明確 support-scope decision | No |
| Packaged delivery | Phase 3 | 沒有 Project、package 或 deployment artifact | 受控 packaged test path 與 artifact record | No |
| Unpackaged delivery comparison | Phase 3 | 沒有 startup artifact 或 comparison run | 受控 unpackaged path 與 comparison rule | No |
| Clean-machine deployment | Phase 3 | Current host 不是 clean-machine evidence | 核准 clean-machine boundary 與 deployment evidence | No |
| Non-essential HDR branch | Phase 1 optional branch / Phase 3 | HDR capability/state 未取得，非 HDR branch 不依賴它 | Product scope 納入 HDR 且完成 per-display HDR evidence | No for non-HDR branch |
| Phase 3 Accessibility completeness | Phase 3 | `inspect`、`AccScope`、Accessibility Insights 未觀察到，且本文件不執行 test | 工具、版本、inspection method 與 accessibility evidence boundary 核准 | No for Phase 1 windowing gate |

Deferred 不等於 completed；每個項目仍須在重新啟用時重新評估。

## 11. Remaining Blocking Actions

以下只保留真正阻止 Phase 1 進入 execution authorization review 的事項；Phase 2／3 scope 不重新加入：

| Blocking action | Required evidence | Mutation required | Separate authorization required |
| --- | --- | --- | --- |
| WinUI 3 SDK／template／build-path provenance | 既有 local SDK/package/template evidence，與 official candidate version 分離 | No for this reassessment; future verification may require project/toolchain action | Yes |
| WPF 與 WinUI 等價 build-path definition | 兩個 candidate 的同一 acceptance boundary、版本與 provenance | No for this reassessment | Yes |
| Display topology baseline | 每個 display 的 resolution、position、primary、extend/duplicate 與 identity mapping | No for this reassessment；不得修改 display setting | Yes for further inspection |
| Per-monitor DPI evidence path | 可重現的 effective DPI method 與 metadata | No for this reassessment；不得修改 display setting | Yes |
| Synthetic content/input isolation | 不接觸真實螢幕資料的 synthetic content、pointer、keyboard acceptance definition | Future prototype may be required | Yes |
| Evidence storage governance | owner、retention、metadata、sensitive-data review 與 approved boundary | No result directory in this task | Yes for approval |
| Safety／cleanup acceptance | 可中斷的 termination、focus、topmost、shortcut、process 與 cleanup review | Future runtime execution only after approval | Yes |
| Independent Phase 1 authorization | Review record、scope、stop rules、evidence boundary 與明確 authorization | No | Yes |

### 11.1 Decision Derivation Table

下表正好對應上述 8 個 Phase 1 Blocking Actions。`Evidence status` 使用 Evidence Acceptance Status vocabulary；`Blocks authorization review` 使用本表固定的 `Yes`／`No` 判定欄，不是 Gate Result。

| Blocking Action | Related Gate | Source IDs | Evidence status | Remaining condition | Blocks authorization review |
| --- | --- | --- | --- | --- | --- |
| `BA-001` WinUI 3 SDK／template／build-path provenance | `P1-GATE-002`, `P1-GATE-004` | `UI-PREQ-001`, `UI-BLOCK-001`, `UI-CLOSURE-FIND-004` | `Insufficient` | SDK、template、candidate parity 與 build path 尚未以 local evidence 證明 | Yes |
| `BA-002` WPF 與 WinUI 等價 build-path definition | `P1-GATE-003`, `P1-GATE-004` | `UI-PREQ-001`, `UI-PREQ-003`, `UI-BLOCK-001`, `UI-CLOSURE-FIND-003` | `Insufficient` | 兩個 candidate 尚未有相同 acceptance boundary 與可比較 build-path provenance | Yes |
| `BA-003` Display topology baseline | `P1-GATE-005` | `UI-PREQ-004`, `UI-PREQ-005`, `UI-BLOCK-003`, `UI-ENV-GAP-002`, `UI-CLOSURE-FIND-002` | `Insufficient` | 每個 display 的 resolution、position、primary、extend/duplicate 與 identity mapping 尚未完整 | Yes |
| `BA-004` Per-monitor DPI evidence path | `P1-GATE-006` | `UI-PREQ-004`, `UI-PREQ-005`, `UI-BLOCK-003`, `UI-ENV-GAP-001`, `UI-CLOSURE-FIND-002` | `Insufficient` | Effective per-monitor DPI 的可重現取得方法與 baseline 尚未完成 | Yes |
| `BA-005` Synthetic content/input isolation | `P1-GATE-007` | `UI-PREQ-003`, `UI-PREQ-009`, `UI-CLOSURE-FIND-006` | `Insufficient` | Synthetic content、pointer、keyboard boundary 尚未形成可驗證 artifact | Yes |
| `BA-006` Evidence storage governance | `P1-GATE-008` | `UI-PREQ-012`, `UI-BLOCK-007`, `UI-ENV-GAP-009`, `UI-CLOSURE-FIND-006` | `Accepted with limitation` | Policy 已定義，但 owner、retention、operational review 與 result evidence 尚未完成 | Yes |
| `BA-007` Safety／cleanup acceptance | `P1-GATE-009` | `UI-PREQ-013`, `UI-BLOCK-008`, `UI-ENV-GAP-010`, `UI-CLOSURE-FIND-006` | `Accepted with limitation` | Policy 已定義，但 termination、focus、topmost、interruption 與 cleanup runtime acceptance 尚未完成 | Yes |
| `BA-008` Independent Phase 1 authorization | `P1-GATE-010` | `UI-PREQ-014`, `UI-BLOCK-009`, `UI-ENV-GAP-011`, `UI-CLOSURE-FIND-007` | `Insufficient` | 沒有獨立 Review 與 Phase-specific authorization record | Yes |

機械式推導規則：任一 `BA-001` 至 `BA-008` 尚未完成，即不得開始 Phase 1 execution authorization review；因此本文件的唯一結論固定為：

`Open Phase 1 Blocking Actions: 8`  →  `Readiness Decision: Not ready`  →  `Runtime Spike Execution Authorized: No`

## 12. Readiness Decision

### 12.1 Decision

本次結論：`Not ready`。

理由是 P1-GATE-001 的 host baseline 已足以作為 reassessment input，但 P1-GATE-002 至 P1-GATE-010 仍有 build path、display/DPI、synthetic input、evidence operation、safety review 或 authorization 缺口。這些缺口足以阻止 Phase 1 進入 execution authorization review 的完成狀態。

本結論不表示 WinUI 3 或 WPF 不可行；它只表示目前證據尚不足以讓任何一方進入受控 Runtime Spike。

### 12.2 Fixed authorization outcome

| Decision field | Value |
| --- | --- |
| Phase 1 readiness | `Not ready` |
| Ready for Phase 1 execution authorization review | `No` |
| Runtime Spike Execution Authorized | `No` |
| Framework selected | None |
| Runtime verification | `Not performed` |
| Source documents modified by this reassessment | None |

即使未來另一份 Review 將 readiness 改為 `Ready for Phase 1 execution authorization review`，仍必須由獨立授權記錄明確核准；本文件不具備該權限。

## 13. Traceability

### 13.1 Reassessment chain

```text
UI-ENV-GAP
  -> UI-CLOSE
    -> UI-CLOSE-EVID
      -> UI-PREQ / UI-BLOCK reassessment
        -> P1-GATE
          -> Readiness decision
            -> Future independent execution authorization review
```

### 13.2 Repository references

- [UI Framework Feasibility Research](01-ui-framework-feasibility.md)
- [UI Framework Runtime Spike Plan](02-ui-framework-runtime-spike-plan.md)
- [UI Framework Runtime Spike Execution Readiness](03-ui-framework-runtime-spike-execution-readiness.md)
- [UI Framework Runtime Environment Baseline](04-ui-framework-runtime-environment-baseline.md)
- [UI Framework Runtime Prerequisite Closure Plan](05-ui-framework-runtime-prerequisite-closure-plan.md)
- [UI Framework Phase 1 Prerequisite Closure Record](06-ui-framework-phase1-prerequisite-closure-record.md)
- [ADR-0002: UI Framework Selection](../../../Architecture/adr/ADR-0002-ui-framework-selection.md)
- [Technology Decision Roadmap](../../../Architecture/TECHNOLOGY-DECISION-ROADMAP.md)

### 13.3 Evidence mapping

| Reassessment layer | Source |
| --- | --- |
| Official version and support evidence | `UI-CLOSE-EVID-001`–`004`, `006`, `010`, plus official references in `RESEARCH-TECH-UI-006` |
| Host and display evidence | `UI-CLOSE-EVID-006`–`008` |
| Toolchain and SDK file evidence | `UI-CLOSE-EVID-009`–`011` |
| Measurement/accessibility evidence | `UI-CLOSE-EVID-005`, `012` |
| Storage and safety policy evidence | `UI-CLOSE-EVID-013`, `014` |
| Authorization boundary | `UI-CLOSE-EVID-015` |

### 13.4 Official references carried from the evidence baseline

- [Windows App SDK downloads](https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/downloads) — stable `2.3.1`，checked 2026-07-26。
- [.NET 10 downloads](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) — latest `10.0.10`、SDK `10.0.302`，checked 2026-07-26。
- [Windows SDK release notes](https://learn.microsoft.com/en-us/windows/apps/windows-sdk/release-notes) — current page lists `10.0.28000.2526`，checked 2026-07-26。
- [Visual Studio 2026 release history](https://learn.microsoft.com/en-us/visualstudio/releases/2026/release-history) — Stable `18.8.1`，checked 2026-07-26。
- [Windows Performance Toolkit](https://learn.microsoft.com/en-us/windows-hardware/test/wpt/) — WPR、WPA 與 xperf reference。
- [Accessibility testing](https://learn.microsoft.com/en-us/windows/apps/design/accessibility/accessibility-testing) — Accessibility Insights 與 Windows SDK legacy tool guidance。
- [Windows 11 release health](https://learn.microsoft.com/en-us/windows/release-health/windows11-release-information) — `26200.8875` 的 GA channel identity evidence，checked 2026-07-26。

## 14. Completion Boundary

完成本文件只代表完成一次基於既有 evidence 的 readiness reassessment，不代表：

- Phase 1 已 Ready。
- 任一 Runtime Spike 已獲授權、建立或執行。
- WinUI 3、Windows App SDK 或 WPF 已選定。
- Build path、DPI path、synthetic input、evidence storage 或 cleanup 已 runtime verified。
- ARM64、Packaging、Clean-machine 或 Accessibility 已完成。
- `ADR-0002` 可以由 Draft 轉為 Accepted。
- 任何 Screenshot、Screen recording、Capture、Clipboard 或 Annotation 程式碼已建立。

### 允許的最小同步更新

主要交付物：

- `docs/Research/Technology/07-ui-framework-phase1-readiness-reassessment.md`

允許最小更新：

- `docs/Research/Technology/README.md`
- `docs/Research/README.md`
- `docs/index.md`
- `CHANGELOG.md`
- `TODO.md`

同步更新只能新增文件連結、Draft 狀態與待 Review 項目，不得修改上游 Research、ADR、PRD、Specs 或 Architecture。

### Prohibited actions for this task

- 不安裝、下載、更新或移除工具、SDK、Runtime、Workload 或 Package。
- 不建立 Project、Solution、Prototype、Result directory 或 Source Code。
- 不執行 Restore、Build、Run、Publish、WPR trace、效能量測、Accessibility test、Deployment test 或 Runtime Spike。
- 不建立 Screenshot、Screen recording、Capture hook、Overlay 或真實螢幕資料管線。
- 不修改 `RESEARCH-TECH-UI-003` 至 `RESEARCH-TECH-UI-006`、ADR、PRD、Specs 或 Architecture。
