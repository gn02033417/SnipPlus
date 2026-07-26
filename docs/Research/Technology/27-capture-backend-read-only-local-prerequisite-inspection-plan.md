# Capture Backend Read-only Local Prerequisite Inspection Plan

| Field | Value |
|---|---|
| Document ID | RESEARCH-TECH-CAPTURE-008 |
| Title | Capture Backend Read-only Local Prerequisite Inspection Plan |
| Status | Draft |
| Research Type | Read-only Local Inspection Plan |
| Parent Reassessment | RESEARCH-TECH-CAPTURE-007 |
| Official Evidence Baseline | RESEARCH-TECH-CAPTURE-006 |
| Parent Enablement Specification | RESEARCH-TECH-CAPTURE-005 |
| Parent Closure Plan | RESEARCH-TECH-CAPTURE-004 |
| Execution Status | Not started |
| Official-source Research | Not performed in this document |
| Local Environment Inspection | Not performed |
| Package Cache Inspection | Not performed |
| Build Verification | Not performed |
| Runtime Verification | Not performed |
| Inspection Execution Authorized | No |
| Closure Execution Authorized | No |
| Capture Runtime Spike Authorized | No |
| Evidence Write Authorized | No |
| UI Framework Decision | Unresolved — ADR-0002 remains Draft |
| Rendering Decision | Not made |
| Capture Decision | Not made |
| Owner | TBD |
| Last reviewed | Not reviewed |

## 1. Purpose

本文件只回答：

> 為了補足 RESEARCH-TECH-CAPTURE-007 中可由本機唯讀觀察取得的證據，應檢查哪些 Host SDK、Windows SDK、Capture API development assets、Graphics／Interop dependencies、Package Cache、Build Tool、Display environment 與 Repository boundary；每項查核的安全範圍、輸出欄位及證據義務為何？

這是 Inspection Plan，不是：

- Inspection Record。
- Closure Execution。
- Authorization Request。
- Build Verification。
- Runtime Spike。
- Capture Backend Decision。
- Capture ADR。

本文件建立時不得執行任何計畫命令。

## 2. Scope

只規劃下列唯讀查核：

- Windows edition、build 及 CPU architecture。
- .NET SDK、Runtime 及 Windows Desktop Runtime。
- Visual Studio、Build Tools、vswhere 與 MSBuild provenance。
- Windows SDK version roots。
- Windows App SDK Runtime、SDK 及既有 Package identity。
- WinUI 3 experimental host assets。
- WPF targeting pack 及 reference assemblies。
- Windows Graphics Capture 相關 WinRT metadata、Header 及 Interop definition。
- Direct3D 11、DXGI 及 Desktop Duplication development assets。
- GDI、User32 及 Window-oriented API development assets。
- NuGet configuration 與 global-packages path。
- 已存在的 Capture-related Package Cache。
- Candidate dependency metadata 及 native assets。
- Repository isolation boundary。
- Planned experiment 及 result roots 是否已存在。
- GPU、driver 及 D3D11 capability 的既有可觀察資訊。
- Display topology、monitor bounds 及 negative-coordinate capability。
- DPI awareness、per-monitor DPI 及 scaling。
- HDR 及 color-state 的唯讀狀態。
- Shared UI Research evidence 的可重用範圍。

## 3. Non-goals

不得：

- 執行任何規劃中的命令。
- 進行新的官方網路研究。
- 下載或安裝 SDK、Runtime、Package、Tool 或 workload。
- 修改 Visual Studio Installer。
- 修改 NuGet source 或 config。
- Restore Package。
- 建立 Project、Solution、Prototype 或 Source Code。
- 執行 Build、Run、Publish 或 Capture API。
- 擷取桌面、視窗、螢幕或 Frame。
- 建立 Screenshot、Recording、PNG 或 Pixel Difference。
- 建立 Result directory。
- 建立實際 Log、Inventory 或 Evidence。
- 修改 Registry、PATH、Display、DPI、HDR 或 Power Plan。
- 使用管理員權限。
- 建立 CAP-AUTH。
- 修改 RESEARCH-TECH-CAPTURE-001..007。
- 修改 UI／Rendering Research Line。
- 修改 ADR-0002 或建立 Capture ADR。
- 選擇 Capture Backend。
- 開始正式截圖功能。

## 4. Controlled Vocabulary

### 4.1 Inspection Item Status

只能使用：

- Planned。
- Blocked。
- Deferred。
- Not applicable。

### 4.2 Future Observation Result

只能使用：

- Observed。
- Not observed。
- Conflicting。
- Unavailable。
- Not executed。

本文件所有 Observation Result 固定為 Not executed。

### 4.3 Inspection Authorization

只能使用：

- Not granted。

### 4.4 Execution Permission

只能使用：

- No。

不得使用：

- Completed。
- Resolved。
- Approved。
- Authorized。
- Executed。

## 5. Source Binding

每個 Inspection Item 必須追溯到至少一項：

- CAP-OFF-GAP-001..015。
- CAP-PREQ-001..030。
- CAP-BLOCK-001..012。
- CAP-PAIR-001..010。
- CAP-ENABLE-001..007。
- CAP-CGATE-001..010。

規則：

- 只規劃能由本機唯讀觀察取得的證據。
- 需要 Project creation、Restore、Build 或 Runtime 的項目不得偽裝為唯讀查核。
- 共用 Host evidence 必須引用現有 UI Research ID。
- 不得重新定義 UI-AUTH-001..008。
- 官方文件已確認的 API claim 不得無理由重新研究。
- 上游規格不足時建立 CAP-INSPECT-GAP-xxx，不得修改上游。

## 6. Inspection Item Register

建立正好 20 個 Inspection Item：

| ID | Inspection subject |
|---|---|
| CAP-INSPECT-001 | Windows edition/build/architecture baseline |
| CAP-INSPECT-002 | .NET SDK inventory |
| CAP-INSPECT-003 | .NET Runtime/Windows Desktop Runtime inventory |
| CAP-INSPECT-004 | Visual Studio/Build Tools/vswhere availability |
| CAP-INSPECT-005 | MSBuild path and provenance |
| CAP-INSPECT-006 | Windows SDK version roots |
| CAP-INSPECT-007 | Windows App SDK Runtime inventory |
| CAP-INSPECT-008 | Windows App SDK SDK/Package Cache assets |
| CAP-INSPECT-009 | WinUI 3 templates/targets/experimental host assets |
| CAP-INSPECT-010 | WPF targeting pack/reference assemblies |
| CAP-INSPECT-011 | Windows Graphics Capture WinRT metadata/headers/interop |
| CAP-INSPECT-012 | Direct3D 11/DXGI/Desktop Duplication assets |
| CAP-INSPECT-013 | GDI/User32/Window-oriented API assets |
| CAP-INSPECT-014 | NuGet sources/config/global-packages path |
| CAP-INSPECT-015 | Capture-related cached Package identity/versions/native assets |
| CAP-INSPECT-016 | Candidate dependency/transitive metadata |
| CAP-INSPECT-017 | Repository isolation and planned experiment/result roots |
| CAP-INSPECT-018 | GPU/driver/D3D11 capability boundary |
| CAP-INSPECT-019 | Display topology/monitor bounds/negative coordinates/DPI |
| CAP-INSPECT-020 | HDR/color state and Shared UI evidence inheritance |

## 7. 每個 Inspection Item 固定欄位

每個 CAP-INSPECT 必須包含：

- Inspection Item ID、Inspection subject、Inspection question。
- Related Official Gap IDs、Related Prerequisites、Related Blockers。
- Related Candidate–Host Pairs、Related Enablement Items、Related Closure Gates。
- Dependency ownership、Shared UI source IDs、Existing official evidence。
- Planned read-only method、Planned command／API、Command execution environment。
- Safety classification、Expected privilege、Network access required、Mutation risk。
- File output expected、Registry write expected、Package／Cache mutation expected、Display／system mutation expected。
- Expected output fields、Sensitive-data considerations。
- Proposed future Local Evidence ID、Proposed future evidence destination。
- Success condition、Not-observed interpretation、Conflict handling、Tool-missing handling、Fallback method。
- Phase C1 impact、Inspection authorization、Execution permitted、Observation result、Owner、Status、Open questions。

固定：

- Expected privilege: Standard user。
- Network access required: No。
- Mutation risk: None。
- File output expected: No。
- Registry write expected: No。
- Package/Cache mutation expected: No。
- Display/system mutation expected: No。
- Inspection authorization: Not granted。
- Execution permitted: No。
- Observation result: Not executed。
- Owner: TBD。

## 7.1 Per-item Planned Field Matrix

每列均為規劃值；不代表已執行或已取得 Local Evidence：

| ID | Subject | Question | Gap | Prerequisite | Blocker | Pairs | Enablement | Gates | Ownership | UI source | Official evidence | Read-only method | Planned command/API | Environment | Safety | Expected output | Future Evidence | C1 impact | Status |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| CAP-INSPECT-001 | Windows edition/build/architecture baseline | OS edition、build、CPU architecture | CAP-OFF-GAP-001 | CAP-PREQ-001 | CAP-BLOCK-001 | CAP-PAIR-001..010 | CAP-ENABLE-001 | CAP-CGATE-001 | Shared UI/capture-specific; owner TBD | RESEARCH-TECH-UI-006..009 where applicable; Pending | CAP-OFF-EVID referenced in parent baseline | Process inventory read | Process inventory read; not executed | Standard user; local workstation only | Read-only; no mutation | OS version and architecture metadata | CAP-LOCAL-EVID-001 | C1 local baseline | Planned |
| CAP-INSPECT-002 | .NET SDK inventory | .NET SDK identity、version、architecture及 root | CAP-OFF-GAP-001、002、014 | CAP-PREQ-001、002、027 | CAP-BLOCK-001、010 | CAP-PAIR-001..010 | CAP-ENABLE-001、003 | CAP-CGATE-001、008 | Shared UI/capture-specific; owner TBD | RESEARCH-TECH-UI-006..009 where applicable; Pending | CAP-OFF-EVID referenced in parent baseline | Process inventory read | Process inventory read; not executed | Standard user; local workstation only | Read-only; no mutation | SDK version、architecture、installation root | CAP-LOCAL-EVID-002 | C1 host/build baseline | Planned |
| CAP-INSPECT-003 | .NET Runtime/Windows Desktop Runtime inventory | Runtime identity；不把 Runtime 視為 Build 或 Run | CAP-OFF-GAP-001、002、014 | CAP-PREQ-001、002、027 | CAP-BLOCK-001、010 | CAP-PAIR-001..010 | CAP-ENABLE-001、003 | CAP-CGATE-001、008 | Shared UI/capture-specific; owner TBD | RESEARCH-TECH-UI-006..009 where applicable; Pending | CAP-OFF-EVID referenced in parent baseline | Process inventory read | Process inventory read; not executed | Standard user; local workstation only | Read-only; no mutation | Runtime version、architecture、source | CAP-LOCAL-EVID-003 | C1 host/build baseline | Planned |
| CAP-INSPECT-004 | Visual Studio/Build Tools/vswhere availability | IDE、Build Tools、vswhere既有安裝狀態 | CAP-OFF-GAP-001、002、014 | CAP-PREQ-001、002、027 | CAP-BLOCK-001、010 | CAP-PAIR-001..010 | CAP-ENABLE-001、003 | CAP-CGATE-001、008 | Shared UI/capture-specific; owner TBD | RESEARCH-TECH-UI-006..009 where applicable; Pending | CAP-OFF-EVID referenced in parent baseline | File-system metadata read | File-system metadata read; not executed | Standard user; local workstation only | Read-only; no mutation | tool version、installation path、instance state | CAP-LOCAL-EVID-004 | C1 build prerequisite | Planned |
| CAP-INSPECT-005 | MSBuild path and provenance | MSBuild path、version、architecture及來源；不執行 | CAP-OFF-GAP-001、002、014 | CAP-PREQ-002、027 | CAP-BLOCK-001、010 | CAP-PAIR-001..010 | CAP-ENABLE-001、003 | CAP-CGATE-001、008 | Shared UI/capture-specific; owner TBD | RESEARCH-TECH-UI-006..009 where applicable; Pending | CAP-OFF-EVID referenced in parent baseline | File-system metadata read | File-system metadata read; not executed | Standard user; local workstation only | Read-only; no mutation | path、file version、architecture、provenance | CAP-LOCAL-EVID-005 | C1 build prerequisite | Planned |
| CAP-INSPECT-006 | Windows SDK version roots | SDK roots、headers、libs、WinRT metadata檔案邊界 | CAP-OFF-GAP-001、002、003 | CAP-PREQ-003、004、005 | CAP-BLOCK-002 | CAP-PAIR-001..010 | CAP-ENABLE-002 | CAP-CGATE-002、003 | Shared UI/capture-specific; owner TBD | RESEARCH-TECH-UI-006..009 where applicable; Pending | CAP-OFF-EVID referenced in parent baseline | File-system metadata read | File-system metadata read; not executed | Standard user; local workstation only | Read-only; no mutation | SDK version、include/lib/winmd presence | CAP-LOCAL-EVID-006 | C1 candidate identity | Planned |
| CAP-INSPECT-007 | Windows App SDK Runtime inventory | Runtime identity與 version family；不安裝或註冊 | CAP-OFF-GAP-001、002、003 | CAP-PREQ-001、003、004 | CAP-BLOCK-001、002 | CAP-PAIR-001、002、009、010 | CAP-ENABLE-001、002 | CAP-CGATE-001、002、003 | Shared UI/capture-specific; owner TBD | RESEARCH-TECH-UI-006..009 where applicable; Pending | CAP-OFF-EVID referenced in parent baseline | AppX inventory read | AppX inventory read; not executed | Standard user; local workstation only | Read-only; no mutation | package family、version、architecture | CAP-LOCAL-EVID-007 | C1 host boundary | Planned |
| CAP-INSPECT-008 | Windows App SDK SDK/Package Cache assets | 既有 SDK、package、WinRT/interop asset；不 Restore | CAP-OFF-GAP-002、003、014 | CAP-PREQ-003、004、010、027 | CAP-BLOCK-002、010 | CAP-PAIR-001、002、009、010 | CAP-ENABLE-002、003 | CAP-CGATE-002、003、008 | Shared UI/capture-specific; owner TBD | RESEARCH-TECH-UI-006..009 where applicable; Pending | CAP-OFF-EVID referenced in parent baseline | Package metadata read | Package metadata read; not executed | Standard user; local workstation only | Read-only; no mutation | package ID、version、TFM、asset names | CAP-LOCAL-EVID-008 | C1 package boundary | Planned |
| CAP-INSPECT-009 | WinUI 3 templates/targets/experimental host assets | 既有 template、targets、props；不建立 Project | CAP-OFF-GAP-001、002、003、014 | CAP-PREQ-001、002、027 | CAP-BLOCK-001、002、010 | CAP-PAIR-001、003、005、007、009 | CAP-ENABLE-001、002、003 | CAP-CGATE-001、003、008 | Shared UI/capture-specific; owner TBD | RESEARCH-TECH-UI-006..009 where applicable; Pending | CAP-OFF-EVID referenced in parent baseline | File-system metadata read | File-system metadata read; not executed | Standard user; local workstation only | Read-only; no mutation | template、target、framework、path provenance | CAP-LOCAL-EVID-009 | C1 WinUI host path | Planned |
| CAP-INSPECT-010 | WPF targeting pack/reference assemblies | WPF targeting pack、reference assemblies；不 Build | CAP-OFF-GAP-001、002、003、014 | CAP-PREQ-001、002、027 | CAP-BLOCK-001、002、010 | CAP-PAIR-002、004、006、008、010 | CAP-ENABLE-001、003 | CAP-CGATE-001、003、008 | Shared UI/capture-specific; owner TBD | RESEARCH-TECH-UI-006..009 where applicable; Pending | CAP-OFF-EVID referenced in parent baseline | File-system metadata read | File-system metadata read; not executed | Standard user; local workstation only | Read-only; no mutation | reference assembly path、TFM、version | CAP-LOCAL-EVID-010 | C1 WPF host path | Planned |
| CAP-INSPECT-011 | Windows Graphics Capture WinRT metadata/headers/interop | WGC metadata、headers、namespace、interop；不呼叫 API | CAP-OFF-GAP-002、003、004 | CAP-PREQ-003..008 | CAP-BLOCK-002 | CAP-PAIR-001、002、009、010 | CAP-ENABLE-002 | CAP-CGATE-002、003 | Shared UI/capture-specific; owner TBD | RESEARCH-TECH-UI-006..009 where applicable; Pending | CAP-OFF-EVID referenced in parent baseline | File-system metadata read | File-system metadata read; not executed | Standard user; local workstation only | Read-only; no mutation | namespace、header、winmd、assembly、version | CAP-LOCAL-EVID-011 | C1 WGC identity | Planned |
| CAP-INSPECT-012 | Direct3D 11/DXGI/Desktop Duplication assets | D3D11、DXGI、COM headers/libs；不建立 device | CAP-OFF-GAP-002、003、005 | CAP-PREQ-003、004、005、006、011 | CAP-BLOCK-002 | CAP-PAIR-003、004、009、010 | CAP-ENABLE-002 | CAP-CGATE-002、003 | Shared UI/capture-specific; owner TBD | RESEARCH-TECH-UI-006..009 where applicable; Pending | CAP-OFF-EVID referenced in parent baseline | File-system metadata read | File-system metadata read; not executed | Standard user; local workstation only | Read-only; no mutation | header、lib、COM metadata、version | CAP-LOCAL-EVID-012 | C1 device boundary | Planned |
| CAP-INSPECT-013 | GDI/User32/Window-oriented API assets | GDI、User32、PrintWindow、BitBlt、GetDIBits asset；不擷取 | CAP-OFF-GAP-002、003、006、007 | CAP-PREQ-003、004、006、017、018、019 | CAP-BLOCK-002、005、006 | CAP-PAIR-005..008 | CAP-ENABLE-002、004 | CAP-CGATE-002、003、004 | Shared UI/capture-specific; owner TBD | RESEARCH-TECH-UI-006..009 where applicable; Pending | CAP-OFF-EVID referenced in parent baseline | File-system metadata read | File-system metadata read; not executed | Standard user; local workstation only | Read-only; no mutation | header、lib、interop identity | CAP-LOCAL-EVID-013 | C1 API boundary；C2 behavior deferred | Planned |
| CAP-INSPECT-014 | NuGet sources/config/global-packages path | NuGet provenance與 cache path；不讀 credential、不修改 | CAP-OFF-GAP-001、002、014、015 | CAP-PREQ-027、029、030 | CAP-BLOCK-008、009、010 | CAP-PAIR-001..010 | CAP-ENABLE-003、006 | CAP-CGATE-008、009 | Shared UI/capture-specific; owner TBD | RESEARCH-TECH-UI-006..009 where applicable; Pending | CAP-OFF-EVID referenced in parent baseline | NuGet configuration read | NuGet configuration read; not executed | Standard user; local workstation only | Read-only; no mutation | source hostname、path alias、credential presence category | CAP-LOCAL-EVID-014 | C1 authority boundary | Planned |
| CAP-INSPECT-015 | Capture-related cached Package identity/versions/native assets | 既有 capture package metadata；不下載、不刪除、不修改 | CAP-OFF-GAP-002、014 | CAP-PREQ-003、010、027 | CAP-BLOCK-002、010 | CAP-PAIR-001..010 | CAP-ENABLE-002、003 | CAP-CGATE-002、008 | Shared UI/capture-specific; owner TBD | RESEARCH-TECH-UI-006..009 where applicable; Pending | CAP-OFF-EVID referenced in parent baseline | Package metadata read | Package metadata read; not executed | Standard user; local workstation only | Read-only; no mutation | ID、version、TFM、RID、native asset names | CAP-LOCAL-EVID-015 | C1 package evidence | Planned |
| CAP-INSPECT-016 | Candidate dependency/transitive metadata | 五個 Candidate 的既有 dependency metadata；不 ranking | CAP-OFF-GAP-002、008、014 | CAP-PREQ-003、004、010、012、027 | CAP-BLOCK-002、010 | CAP-PAIR-001..010 | CAP-ENABLE-002、003 | CAP-CGATE-002、003、008 | Shared UI/capture-specific; owner TBD | RESEARCH-TECH-UI-006..009 where applicable; Pending | CAP-OFF-EVID referenced in parent baseline | Package metadata read | Package metadata read; not executed | Standard user; local workstation only | Read-only; no mutation | dependency ID、version、target、native asset、source | CAP-LOCAL-EVID-016 | C1 identity；no ranking | Planned |
| CAP-INSPECT-017 | Repository isolation and planned experiment/result roots | workspace boundary與既有 result root；不建立目錄 | CAP-OFF-GAP-014、015 | CAP-PREQ-027、029、030 | CAP-BLOCK-008、009、010 | CAP-PAIR-001..010 | CAP-ENABLE-003、006 | CAP-CGATE-008、009 | Shared UI/capture-specific; owner TBD | RESEARCH-TECH-UI-006..009 where applicable; Pending | CAP-OFF-EVID referenced in parent baseline | File-system metadata read | File-system metadata read; not executed | Standard user; local workstation only | Read-only; no mutation | workspace alias、tracked/untracked metadata、root existence | CAP-LOCAL-EVID-017 | C1 isolation；no file creation | Planned |
| CAP-INSPECT-018 | GPU/driver/D3D11 capability boundary | GPU、driver、D3D metadata；不建立 device/session | CAP-OFF-GAP-005、012、013 | CAP-PREQ-005、006、011、021、022 | CAP-BLOCK-007、011、012 | CAP-PAIR-003、004、009、010 | CAP-ENABLE-002、005、007 | CAP-CGATE-002、006、010 | Shared UI/capture-specific; owner TBD | RESEARCH-TECH-UI-006..009 where applicable; Pending | CAP-OFF-EVID referenced in parent baseline | Graphics capability read | Graphics capability read; not executed | Standard user; local workstation only | Read-only; no mutation | adapter alias、driver version、feature metadata | CAP-LOCAL-EVID-018 | C1 device prerequisite；C3 recovery deferred | Planned |
| CAP-INSPECT-019 | Display topology/monitor bounds/negative coordinates/DPI | monitor、bounds、primary、negative coordinates、per-monitor DPI | CAP-OFF-GAP-004、005、006、009 | CAP-PREQ-014、015、016、022、023 | CAP-BLOCK-004、007 | CAP-PAIR-001..010 | CAP-ENABLE-005 | CAP-CGATE-005、006、007 | Shared UI/capture-specific; owner TBD | RESEARCH-TECH-UI-006..009 where applicable; Pending | CAP-OFF-EVID referenced in parent baseline | Display configuration read | Display configuration read; not executed | Standard user; local workstation only | Read-only; no mutation | count、bounds、origin、DPI category、primary flag；no desktop image | CAP-LOCAL-EVID-019 | C1 coordinate boundary | Planned |
| CAP-INSPECT-020 | HDR/color state and Shared UI evidence inheritance | HDR/color metadata及可重用 UI evidence；不切換 HDR | CAP-OFF-GAP-010、011、013、015 | CAP-PREQ-020、024、025、026、029 | CAP-BLOCK-005、008、009 | CAP-PAIR-001..010 | CAP-ENABLE-004、006 | CAP-CGATE-004、007、009 | Shared UI/capture-specific; owner TBD | RESEARCH-TECH-UI-006..009 where applicable; Pending | CAP-OFF-EVID referenced in parent baseline | Display configuration read；Environment inheritance | Display configuration read；Environment inheritance; not executed | Standard user; local workstation only | Read-only; no mutation | HDR state、color-state category、inherited evidence IDs；no desktop image | CAP-LOCAL-EVID-020 | Deferred；C2/C3 details remain deferred | Deferred |

補充欄位固定套用至上表每一列：

| Field | Fixed planned value |
|---|---|
| Expected privilege | Standard user |
| Network access required | No |
| Mutation risk | None |
| File output expected | No |
| Registry write expected | No |
| Package/Cache mutation expected | No |
| Display/system mutation expected | No |
| Sensitive-data considerations | 不讀 credentials、tokens、private files、window titles、desktop image 或非必要 machine identifiers |
| Proposed future evidence destination | Planned only; docs/Research/Technology/results/capture/local-prerequisite-inspection/; not created |
| Success condition | Read-only metadata question is answerable without changing system or repository state |
| Not-observed interpretation | Not observed does not mean Unsupported |
| Conflict handling | 保留 conflicting paths/versions，升級後續 review；不自動選 winner |
| Tool-missing handling | Record Unavailable；不安裝或下載 replacement |
| Fallback method | Narrower read-only metadata inspection already present on system；not executed |
| Inspection authorization | Not granted |
| Execution permitted | No |
| Observation result | Not executed |
| Owner | TBD |
| Open questions | Exact local path、version、owner及 evidence root remains TBD |

## 8. Planned Command Safety Classification

| Classification | 說明 |
|---|---|
| Process inventory read | 查詢既有工具、SDK或 Runtime版本 |
| File-system metadata read | 讀取既有檔案、目錄、版本與 metadata |
| Registry read | 只讀取指定 Registry value |
| AppX inventory read | 只列出已安裝 Package |
| NuGet configuration read | 只讀取 Source、Config與 Cache path |
| Package metadata read | 只讀取已存在的 nuspec、runtime assets及 dependency metadata |
| Graphics capability read | 只讀取 GPU、driver或 feature metadata，不建立 graphics device |
| Display configuration read | 只讀取 monitor、bounds、DPI、HDR及 color state |
| Environment inheritance | 引用既有 UI／Rendering research evidence，不重新查詢 |

本文件可以記錄未來 command文字，但不得執行。禁止規劃成已允許操作：

- dotnet new、dotnet restore、dotnet build、dotnet run。
- dotnet workload install、dotnet workload update。
- msbuild、devenv /Build。
- nuget install、winget install、choco install。
- Visual Studio Installer修改。
- AppX install／register。
- NuGet source add／remove／enable／disable。
- Registry write。
- Display／DPI／HDR mutation。
- 目錄或檔案建立。
- Command output redirection。
- Capture API invocation。

## 9. Shared UI Evidence Inheritance Matrix

| Capture requirement | UI source evidence | Current UI authority | Reusable evidence | Re-query required | Remaining Capture gap |
|---|---|---|---|---|---|
| Windows 11 x64 baseline | RESEARCH-TECH-UI-007..009 | UI-AUTH-001 Pending | Existing OS baseline if current | Only if stale or Capture metadata missing | CAP-OFF-GAP-001 |
| .NET SDK／Runtime | RESEARCH-TECH-UI-007..009 | UI-AUTH-001 Pending | Existing version identity | Only if stale or target differs | CAP-OFF-GAP-001、002 |
| Visual Studio／Build Tools | RESEARCH-TECH-UI-007..009 | UI-AUTH-006 Pending | Existing tool provenance | Capture-specific asset delta only | CAP-OFF-GAP-014 |
| Windows SDK | RESEARCH-TECH-UI-007..009 | UI-AUTH-001 Pending | Existing SDK baseline | Capture header/WinRT delta only | CAP-OFF-GAP-002 |
| Windows App SDK | RESEARCH-TECH-UI-007..009 | UI-AUTH-001 Pending | Existing host package identity | Capture-specific package delta only | CAP-OFF-GAP-002 |
| WinUI 3 build path | RESEARCH-TECH-UI-007..009 | UI-AUTH-001、006 Pending | Existing host boundary | Only missing capture asset | CAP-OFF-GAP-003 |
| WPF build path | RESEARCH-TECH-UI-007..009 | UI-AUTH-001、006 Pending | Existing host boundary | Only missing capture asset | CAP-OFF-GAP-003 |
| Experimental repository isolation | AGENTS.md、RESEARCH-TECH-UI-007..009 | UI-AUTH-006 Pending | Existing workspace boundary | Check planned roots only | CAP-OFF-GAP-014、015 |
| Display topology | RESEARCH-TECH-UI-007..009 | UI-AUTH-004 Pending | Existing technical metadata | Capture coordinate delta | CAP-OFF-GAP-009 |
| Per-monitor DPI | RESEARCH-TECH-UI-007..009 | UI-AUTH-004 Pending | Existing DPI evidence | Mapping delta only | CAP-OFF-GAP-006、009 |
| GPU／driver | RESEARCH-TECH-UI-007..009 | UI-AUTH-005 Pending | Existing adapter metadata | D3D/Capture delta | CAP-OFF-GAP-005、013 |
| HDR observation | RESEARCH-TECH-UI-007..009 | UI-AUTH-005 Pending | Existing color-state evidence | Optional C2/C3 detail | CAP-OFF-GAP-010、011 |
| Evidence storage policy | Future governance | UI-AUTH-007 Pending | No accepted root | Owner and retention only | CAP-OFF-GAP-015 |
| Safety／cleanup | AGENTS.md and future governance | UI-AUTH-007、008 Pending | Boundary rules only | Future authorization review | CAP-OFF-GAP-015 |
| Project／Restore／Build authority | AGENTS.md | UI-AUTH-006 Pending | Prohibition boundary only | Separate authorization | CAP-OFF-GAP-014 |
| Runtime authority | RESEARCH-TECH-CAPTURE-007 | UI-AUTH-008 Pending | No execution authority | Separate runtime request | CAP-OFF-GAP-012 |

要求：UI-AUTH-001..008 全部保持 Pending；既有 evidence不得無理由重查；Shared UI authority pending不得改寫為 Capture authority。

## 10. Official Evidence to Local Inspection Boundary

| Official claim | Evidence IDs | What official evidence proves | Local question remaining | Inspection IDs |
|---|---|---|---|---|
| WGC API identity | CAP-OFF-EVID-004、005、011 | API、item、frame-pool及 pixel caveat identity | WinRT metadata、header、package、target是否存在 | CAP-INSPECT-006、008、011 |
| WGC activation／interop routes | CAP-OFF-EVID-002、003、005 | HWND、HMONITOR、WindowId、DisplayId及 interop context | Host-specific source creation asset是否存在 | CAP-INSPECT-007..011 |
| Desktop Duplication identity | CAP-OFF-EVID-006 | IDXGIOutputDuplication、AcquireNextFrame及 output boundary | DXGI／D3D11 asset是否存在 | CAP-INSPECT-006、012 |
| D3D11／DXGI dependency | CAP-OFF-EVID-006、011 | Device、surface、frame及 pixel-format context | Header、lib、native metadata、driver capability | CAP-INSPECT-012、018 |
| GDI API identity | CAP-OFF-EVID-007、009 | BitBlt、HDC、HBITMAP、GetDIBits identity | Windows SDK header/lib及 interop asset | CAP-INSPECT-006、013 |
| Window-oriented mechanisms | CAP-OFF-EVID-008、009 | PrintWindow、HWND、HDC及 readback boundary | GDI/User32 development asset；不查 runtime semantics | CAP-INSPECT-006、013 |
| Windows App SDK identity | CAP-OFF-EVID-001、002、003、012 | Host、interop、package/deployment conceptual boundary | Installed Runtime、SDK、cache identity | CAP-INSPECT-007、008 |
| Required headers／namespaces | CAP-OFF-EVID-002、004、006、007、008、009 | Official names | Local file and target asset existence | CAP-INSPECT-006、008、011..013 |
| Packaging context | CAP-OFF-EVID-001、012 | API/package/deployment concerns are distinct | NuGet config、cache、package asset identity | CAP-INSPECT-008、014..016 |
| Graphics-device dependency | CAP-OFF-EVID-004、006、011 | Device/frame object boundary | Existing driver/capability metadata only | CAP-INSPECT-012、018 |
| Coordinate／output model | CAP-OFF-EVID-004、006、007、009 | Output、item、DIB及 frame dimensions as inputs | Current topology、bounds、DPI metadata | CAP-INSPECT-019 |
| Failure／recovery boundary | CAP-OFF-EVID-006、008、011 | Official failure/lifecycle boundary | Local capability metadata only; no recovery run | CAP-INSPECT-018、020 |

官方 API存在不代表 local asset存在；local asset存在不代表 Project、Restore、Build或 Runtime通過；Runtime成功不代表 Crop fidelity、Privacy或 Recovery通過。

## 11. Package Cache Inspection Boundary

只允許規劃：

- 查詢 global-packages path。
- 列出已存在的 Package ID及版本目錄。
- 讀取已存在的 nuspec、target-framework folders及 runtime-specific assets。
- 辨識 native DLL、WinMD、interop asset是否存在。
- 讀取 dependency metadata。
- 讀取公開 Package source hostname。

不得 Restore、下載、修改、清理 Cache、讀取 credential、把 Package存在視為 Build compatibility、把 Package不存在視為 Candidate不支援，或依 Cache形成 Candidate ranking。

## 12. Capture Development Asset Matrix

建立正好五列；Local presence result固定為 Not executed：

| Candidate | Required development asset | Expected source | Inspection IDs | Local presence result | Build still required | Runtime still required |
|---|---|---|---|---|---|---|
| CAP-OPT-001 Windows Graphics Capture | WinRT、Direct3D11、Windows App SDK interop及 frame-pool assets | Windows SDK／Windows App SDK／existing cache | CAP-INSPECT-006、008、011 | Not executed | Yes | Yes |
| CAP-OPT-002 DXGI Desktop Duplication | DXGI、D3D11、COM及 output duplication assets | Windows SDK／existing native metadata | CAP-INSPECT-006、012、018 | Not executed | Yes | Yes |
| CAP-OPT-003 GDI-based capture | wingdi、User32、HDC、HBITMAP、DIB assets | Windows SDK／desktop API | CAP-INSPECT-006、013 | Not executed | Yes | Yes |
| CAP-OPT-004 Window-oriented mechanisms | winuser、User32、PrintWindow及 readback assets | Windows SDK／desktop API | CAP-INSPECT-006、013 | Not executed | Yes | Yes |
| CAP-OPT-005 Hybrid strategy | Each constituent asset separately；no single asset identity | Candidate-dependent existing metadata | CAP-INSPECT-006、008、011..016 | Not executed | Yes | Yes |

## 13. Candidate–Host Inspection Coverage

建立正好十列：

| Pair | Required Host evidence | Required Capture evidence | Inspection IDs | Local availability contribution | Project still required | Build still required | Runtime still required |
|---|---|---|---|---|---|---|---|
| CAP-PAIR-001 WGC × WinUI 3 | WinUI 3 host/runtime/interop | WGC WinRT/frame assets | CAP-INSPECT-007..011 | Not executed | Yes | Yes | Yes |
| CAP-PAIR-002 WGC × WPF | WPF host/interop | WGC WinRT/frame assets | CAP-INSPECT-006、010、011 | Not executed | Yes | Yes | Yes |
| CAP-PAIR-003 DXGI × WinUI 3 | WinUI 3 host/native interop | DXGI/D3D11 assets | CAP-INSPECT-007、009、012、018 | Not executed | Yes | Yes | Yes |
| CAP-PAIR-004 DXGI × WPF | WPF host/native interop | DXGI/D3D11 assets | CAP-INSPECT-010、012、018 | Not executed | Yes | Yes | Yes |
| CAP-PAIR-005 GDI × WinUI 3 | WinUI 3 HWND/HDC interop | GDI/User32/DIB assets | CAP-INSPECT-009、013 | Not executed | Yes | Yes | Yes |
| CAP-PAIR-006 GDI × WPF | WPF HWND/HDC interop | GDI/User32/DIB assets | CAP-INSPECT-010、013 | Not executed | Yes | Yes | Yes |
| CAP-PAIR-007 Window-oriented × WinUI 3 | WinUI 3 HWND/interop | PrintWindow/User32/readback | CAP-INSPECT-009、013 | Not executed | Yes | Yes | Yes |
| CAP-PAIR-008 Window-oriented × WPF | WPF HWND/interop | PrintWindow/User32/readback | CAP-INSPECT-010、013 | Not executed | Yes | Yes | Yes |
| CAP-PAIR-009 Hybrid × WinUI 3 | WinUI 3 host plus each constituent | Constituent assets/shared contract | CAP-INSPECT-007..016 | Not executed | Yes | Yes | Yes |
| CAP-PAIR-010 Hybrid × WPF | WPF host plus each constituent | Constituent assets/shared contract | CAP-INSPECT-006、008、010..016 | Not executed | Yes | Yes | Yes |

Unknown不得因檔案缺少直接改為 Excluded with evidence；local availability只能改善 readiness evidence，不形成 Pair通過。

## 14. Gap-to-Inspection Matrix

完整覆蓋 CAP-OFF-GAP-001..015：

| Gap | Current disposition | Can read-only inspection contribute | Inspection IDs | Remaining evidence class | Blocks Phase C1 authorization request |
|---|---|---|---|---|---|
| CAP-OFF-GAP-001 | Requires local inspection | Yes; OS、SDK、Runtime、host identity | CAP-INSPECT-001..010 | Project/build still separate | Yes |
| CAP-OFF-GAP-002 | Requires package acquisition evidence | Partial; existing package/cache only | CAP-INSPECT-006、008、011..016 | Package、Project、Build | Yes |
| CAP-OFF-GAP-003 | Requires project evidence | Partial; host asset availability only | CAP-INSPECT-007..013 | Experimental project | Yes |
| CAP-OFF-GAP-004 | Requires runtime evidence | Partial; topology/frame prerequisites | CAP-INSPECT-018、019 | Runtime | Yes |
| CAP-OFF-GAP-005 | Requires runtime evidence | Partial; driver/output prerequisites | CAP-INSPECT-012、018、019 | Runtime | Yes |
| CAP-OFF-GAP-006 | Requires runtime evidence | Partial; DPI metadata only | CAP-INSPECT-019 | Runtime | Yes |
| CAP-OFF-GAP-007 | Deferred Phase C2 | No closure by asset inspection | CAP-INSPECT-013 | Deferred Phase C2 | No; deferred |
| CAP-OFF-GAP-008 | Accepted documentation limitation | Partial; constituent metadata only | CAP-INSPECT-006、008、011..016 | Experimental project | Yes |
| CAP-OFF-GAP-009 | Requires runtime evidence | Partial; topology/DPI inputs only | CAP-INSPECT-019 | Runtime | Yes |
| CAP-OFF-GAP-010 | Deferred Phase C2 | No closure by static inspection | CAP-INSPECT-020 | Deferred Phase C2 | No; deferred |
| CAP-OFF-GAP-011 | Deferred Phase C2 | No closure of security behavior | CAP-INSPECT-020 | Deferred Phase C2 | No; deferred |
| CAP-OFF-GAP-012 | Deferred Phase C3 | Driver metadata only; no recovery observation | CAP-INSPECT-018、020 | Deferred Phase C3 | No; deferred |
| CAP-OFF-GAP-013 | Requires runtime evidence | Device/readback prerequisites only | CAP-INSPECT-012、018、019 | Runtime | Yes |
| CAP-OFF-GAP-014 | Requires build evidence | Tool、SDK、package metadata only | CAP-INSPECT-002..016 | Project、Restore、Build | Yes |
| CAP-OFF-GAP-015 | Requires evidence-write authority | Existing root/metadata boundary only | CAP-INSPECT-014、017、020 | Evidence write、Shared UI authority | Yes |

唯讀盤點不得宣稱能關閉 Build或 Runtime Gap；HDR、完整 Overlay、Cursor、Recovery performance不得無理由升格為 Phase C1 blocker；Not observed不等於 Unsupported。

## 15. Environment Observation Boundary

只規劃唯讀觀察：

- Windows build、CPU architecture、GPU name、driver version、D3D feature metadata。
- Monitor count、physical bounds、primary monitor、negative-coordinate topology。
- DPI scaling per monitor、HDR enabled state、color-space/profile metadata，如可安全取得。
- Packaged/unpackaged capability evidence與 Debug/Release build capability evidence來源，但不執行 Build。

不得改變 monitor arrangement、primary monitor、DPI、HDR、color profile、Power Plan；不得建立 D3D capture session、模擬 device loss或執行 Capture API。

## 16. Sensitive-data Boundary

不得規劃讀取或輸出 NuGet credential、API key、token、password、private key、完整環境變數、完整 NuGet config、完整 Registry export、不相關使用者檔名、私人 Repository外內容、Window title、Desktop image、Monitor serial number或 Machine SID。

允許規劃保存公開 Package source hostname、Credential presence category、清理後的 SDK/Package/tool path、公開版本/架構/API metadata及不含私人桌面內容的 Display technical metadata。

## 17. Future Evidence Plan

只規劃，不建立：

docs/Research/Technology/results/capture/local-prerequisite-inspection/

CAP-LOCAL-EVID-001 至 CAP-LOCAL-EVID-020 一對一對應 CAP-INSPECT-001..020。每筆未來 Evidence至少包含 Local Evidence ID、Inspection Item ID、Timestamp、environment alias、privilege、exact command/API、safety classification、exit code、stdout/stderr摘要、observed paths/versions/architecture、source、sensitive values removed、conflict notes、interpretation、related Gap/Prerequisite/Enablement、cleanup confirmation。

本輪不得建立目錄或 Evidence。

## 18. Authorization Packaging Matrix

建立正好 20 列：

| Inspection Item | Safety classification | Standard-user only | Network required | Mutation expected | File output expected | Current authorization | Execution permitted |
|---|---|---|---|---|---|---|---|
| CAP-INSPECT-001 | Process inventory read | Yes | No | No | No | Not granted | No |
| CAP-INSPECT-002 | Process inventory read | Yes | No | No | No | Not granted | No |
| CAP-INSPECT-003 | Process inventory read | Yes | No | No | No | Not granted | No |
| CAP-INSPECT-004 | File-system metadata read | Yes | No | No | No | Not granted | No |
| CAP-INSPECT-005 | File-system metadata read | Yes | No | No | No | Not granted | No |
| CAP-INSPECT-006 | File-system metadata read | Yes | No | No | No | Not granted | No |
| CAP-INSPECT-007 | AppX inventory read | Yes | No | No | No | Not granted | No |
| CAP-INSPECT-008 | Package metadata read | Yes | No | No | No | Not granted | No |
| CAP-INSPECT-009 | File-system metadata read | Yes | No | No | No | Not granted | No |
| CAP-INSPECT-010 | File-system metadata read | Yes | No | No | No | Not granted | No |
| CAP-INSPECT-011 | File-system metadata read | Yes | No | No | No | Not granted | No |
| CAP-INSPECT-012 | File-system metadata read | Yes | No | No | No | Not granted | No |
| CAP-INSPECT-013 | File-system metadata read | Yes | No | No | No | Not granted | No |
| CAP-INSPECT-014 | NuGet configuration read | Yes | No | No | No | Not granted | No |
| CAP-INSPECT-015 | Package metadata read | Yes | No | No | No | Not granted | No |
| CAP-INSPECT-016 | Package metadata read | Yes | No | No | No | Not granted | No |
| CAP-INSPECT-017 | File-system metadata read | Yes | No | No | No | Not granted | No |
| CAP-INSPECT-018 | Graphics capability read | Yes | No | No | No | Not granted | No |
| CAP-INSPECT-019 | Display configuration read | Yes | No | No | No | Not granted | No |
| CAP-INSPECT-020 | Display configuration read；Environment inheritance | Yes | No | No | No | Not granted | No |

固定：Standard-user only = Yes、Network required = No、Mutation expected = No、File output expected = No、Current authorization = Not granted、Execution permitted = No。

## 19. Readiness to Request Inspection Authorization

只能使用：

- Ready to request capture read-only local inspection authorization。
- Conditionally ready to request capture read-only local inspection authorization。
- Not ready to request capture read-only local inspection authorization。

推導條件：

- 20 Inspection Items fully specified。
- All planned commands classified as read-only。
- Standard-user-only。
- No network dependency。
- No mutation。
- No file output。
- Sensitive-data controls complete。
- Future evidence obligations defined。

Inspection Authorization Readiness: Conditionally ready to request capture read-only local inspection authorization。

理由：

- 20個查核項目的安全分類、輸出限制、敏感資料規則與未來 evidence欄位已定義。
- Owner、Human Authorization、future evidence root與 Shared UI authority仍為 TBD／Pending。
- 因此可作為後續 request的規格草稿，不代表 request已建立或查核已獲准。

即使結果為 Conditionally ready，仍固定：

- Inspection Execution Authorized: No。
- Closure Execution Authorized: No。
- Build Verification: Not performed。
- Runtime Verification: Not performed。
- Capture Runtime Spike Authorized: No。
- Evidence Write Authorized: No。
- Capture Decision: Not made。
- Rendering Decision: Not made。

## 19.1 Explicit Coverage Index

### Capture prerequisite coverage

| Prerequisite | Inspection contribution | Current status |
|---|---|---|
| CAP-PREQ-001 | CAP-INSPECT-001、002、003、007 | Planned |
| CAP-PREQ-002 | CAP-INSPECT-002、003、004、005、009、010 | Planned |
| CAP-PREQ-003 | CAP-INSPECT-006、008、011、012、013、015、016 | Planned |
| CAP-PREQ-004 | CAP-INSPECT-006、008、011、012、013、016 | Planned |
| CAP-PREQ-005 | CAP-INSPECT-006、012、018 | Planned |
| CAP-PREQ-006 | CAP-INSPECT-011、012、013、018 | Planned |
| CAP-PREQ-007 | CAP-INSPECT-011 | Planned |
| CAP-PREQ-008 | CAP-INSPECT-011 | Planned |
| CAP-PREQ-009 | CAP-INSPECT-011、012、013 | Planned |
| CAP-PREQ-010 | CAP-INSPECT-008、015、016 | Planned |
| CAP-PREQ-011 | CAP-INSPECT-012、018 | Planned |
| CAP-PREQ-012 | CAP-INSPECT-016 | Planned |
| CAP-PREQ-013 | CAP-INSPECT-017 | Planned |
| CAP-PREQ-014 | CAP-INSPECT-019 | Planned |
| CAP-PREQ-015 | CAP-INSPECT-019 | Planned |
| CAP-PREQ-016 | CAP-INSPECT-019 | Planned |
| CAP-PREQ-017 | CAP-INSPECT-013 | Deferred |
| CAP-PREQ-018 | CAP-INSPECT-013 | Deferred |
| CAP-PREQ-019 | CAP-INSPECT-013 | Deferred |
| CAP-PREQ-020 | CAP-INSPECT-020 | Deferred |
| CAP-PREQ-021 | CAP-INSPECT-018 | Deferred |
| CAP-PREQ-022 | CAP-INSPECT-018、019 | Planned |
| CAP-PREQ-023 | CAP-INSPECT-019 | Planned |
| CAP-PREQ-024 | CAP-INSPECT-020 | Deferred |
| CAP-PREQ-025 | CAP-INSPECT-020 | Deferred |
| CAP-PREQ-026 | CAP-INSPECT-020 | Deferred |
| CAP-PREQ-027 | CAP-INSPECT-002、003、004、005、008、009、010、014、015、016、017 | Planned |
| CAP-PREQ-028 | No local inspection closure; runtime remains separately authorized | Deferred |
| CAP-PREQ-029 | CAP-INSPECT-014、017、020 | Planned |
| CAP-PREQ-030 | CAP-INSPECT-014、017 | Planned |

### Capture blocker coverage

| Blocker | Inspection contribution | Current status |
|---|---|---|
| CAP-BLOCK-001 | CAP-INSPECT-001、002、003、004、005、007、009、010 | Open |
| CAP-BLOCK-002 | CAP-INSPECT-006、007、008、009、010、011、012、013、015、016 | Open |
| CAP-BLOCK-003 | No local-only closure; synthetic project remains separate | Open |
| CAP-BLOCK-004 | CAP-INSPECT-019 | Open |
| CAP-BLOCK-005 | CAP-INSPECT-013、020 | Deferred |
| CAP-BLOCK-006 | CAP-INSPECT-013 | Deferred |
| CAP-BLOCK-007 | CAP-INSPECT-018、019 | Open |
| CAP-BLOCK-008 | CAP-INSPECT-014、017、020 | Open |
| CAP-BLOCK-009 | CAP-INSPECT-014、017、020 | Open |
| CAP-BLOCK-010 | CAP-INSPECT-002、003、004、005、008、009、010、014、015、016、017 | Open |
| CAP-BLOCK-011 | No local-only closure; runtime remains separately authorized | Open |
| CAP-BLOCK-012 | CAP-INSPECT-018；recovery remains C3 | Deferred |

### Enablement coverage

| Enablement | Inspection contribution | Current status |
|---|---|---|
| CAP-ENABLE-001 | CAP-INSPECT-001、002、003、004、005、007、009、010 | Partially specified |
| CAP-ENABLE-002 | CAP-INSPECT-006、008、011、012、013、015、016 | Partially specified |
| CAP-ENABLE-003 | CAP-INSPECT-002、003、004、005、008、009、010、014、015、016、017 | Blocked |
| CAP-ENABLE-004 | CAP-INSPECT-013、020 | Partially specified |
| CAP-ENABLE-005 | CAP-INSPECT-018、019 | Partially specified |
| CAP-ENABLE-006 | CAP-INSPECT-014、017、020 | Partially specified |
| CAP-ENABLE-007 | No local-only closure; runtime remains separate | Blocked |

### Closure gate coverage

| Closure gate | Inspection contribution | Current status |
|---|---|---|
| CAP-CGATE-001 | CAP-INSPECT-001、002、003、004、005、007、009、010 | Blocked |
| CAP-CGATE-002 | CAP-INSPECT-006、008、011、012、013、015、016 | Partially specified |
| CAP-CGATE-003 | CAP-INSPECT-006、007、008、009、010、011、012、013 | Blocked |
| CAP-CGATE-004 | CAP-INSPECT-013、020 | Partially specified |
| CAP-CGATE-005 | CAP-INSPECT-019 | Blocked |
| CAP-CGATE-006 | CAP-INSPECT-018、019 | Blocked |
| CAP-CGATE-007 | CAP-INSPECT-018、019、020 | Blocked |
| CAP-CGATE-008 | CAP-INSPECT-002、003、004、005、008、009、010、014、015、016、017 | Blocked |
| CAP-CGATE-009 | CAP-INSPECT-014、017、020 | Blocked |
| CAP-CGATE-010 | No local-only closure; future runtime boundary | Deferred |

### Shared UI authority coverage

| Authority | Current decision | Effect |
|---|---|---|
| UI-AUTH-001 | Pending | Host/framework identity may be inherited but not executed |
| UI-AUTH-002 | Pending | Handle ownership remains separate |
| UI-AUTH-003 | Pending | Overlay lifecycle remains separate |
| UI-AUTH-004 | Pending | Coordinate authority remains separate |
| UI-AUTH-005 | Pending | Rendering/device evidence remains separate |
| UI-AUTH-006 | Pending | Project/build authority remains separately authorized |
| UI-AUTH-007 | Pending | Evidence persistence remains unauthorized |
| UI-AUTH-008 | Pending | Runtime/display mutation remains unauthorized |

## 20. Traceability

| Trace source | Mapping | Future use | Current state |
|---|---|---|---|
| CAP-OFF-GAP-001..015 | Gap → CAP-INSPECT item | Future local evidence planning | Plan only |
| CAP-INSPECT-001..020 | Inspection item → CAP-LOCAL-EVID-001..020 | Future authorized observation | Not executed |
| CAP-PREQ-001..030 | Prerequisite → inspection scope | Future closure reassessment | Not closed |
| CAP-BLOCK-001..012 | Blocker → local contribution | Future readiness review | Open or deferred |
| CAP-PAIR-001..010 | Pair → Host/Capture asset coverage | Future project/build/runtime | No ranking |
| CAP-ENABLE-001..007 | Inspection contribution → enablement | Future authorization packaging | Recommendation only |
| CAP-CGATE-001..010 | Local contribution → closure gate | Future closure review | Not passed |
| RESEARCH-TECH-CAPTURE-003..007 | Existing capture research line | Future inspection authorization | Referenced |
| RESEARCH-TECH-UI-006..009 | Shared UI evidence inheritance | Avoid duplicate Host checks | Inherited only |
| RESEARCH-TECH-RENDER-003 | Rendering dependency context | Future synthetic scene review | Referenced only |
| ADR-0002-ui-framework-selection.md | UI decision context | Future authority review | Draft; unresolved |
| Architecture/TECHNOLOGY-DECISION-ROADMAP.md | Technology decision context | Future candidate decision | No decision made |

實際文件名稱與 ID必須從 Repository原樣引用。

## 完成條件

- 只建立 docs/Research/Technology/27-capture-backend-read-only-local-prerequisite-inspection-plan.md。
- 不修改任何其他文件。
- 建立正好 20 個 CAP-INSPECT-001..020。
- 覆蓋 15 個 CAP-OFF-GAP。
- 覆蓋 30 個 CAP-PREQ 及 12 個 CAP-BLOCK。
- 覆蓋十個 CAP-PAIR。
- 覆蓋七個 CAP-ENABLE與十個 CAP-CGATE。
- 所有規劃命令明確分類為唯讀。
- 所有 Inspection Item均為 Standard-user、No-network、No-mutation、No-file-output。
- 所有 Inspection authorization = Not granted。
- 所有 Execution permitted = No。
- 所有 Observation result = Not executed。
- 不執行任何計畫命令。
- 不建立 Result directory、Evidence、Project、Prototype或 Source Code。
- 不執行下載、安裝、Restore、Build、Run、Capture API或 Runtime Spike。
- 不建立 CAP-AUTH。
- 不修改 UI／Rendering Research Line。
- 不修改 ADR-0002或建立 Capture ADR。
- git diff --check應通過。

完成後停止本文件任務，等待下一個單一文件指令。
