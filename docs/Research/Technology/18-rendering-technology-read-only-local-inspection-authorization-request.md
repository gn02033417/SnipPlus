# Rendering Technology Read-only Local Inspection Authorization Request

## Document Control

| Field | Value |
|---|---|
| Document ID | `RESEARCH-TECH-RENDER-009` |
| Title | Rendering Technology Read-only Local Inspection Authorization Request |
| Status | Draft |
| Research Type | Read-only Inspection Authorization Request |
| Parent Inspection Plan | `RESEARCH-TECH-RENDER-008` |
| Parent Reassessment | `RESEARCH-TECH-RENDER-007` |
| Authorization Decision | `Pending` |
| Current Authorization | `Not granted` |
| Inspection Execution Authorized | `No` |
| Closure Execution Authorized | `No` |
| Build Verification | `Not performed` |
| Runtime Verification | `Not performed` |
| Runtime Spike Execution Authorized | `No` |
| Rendering Decision | `Not made` |
| Requested by | `TBD` |
| Decision authority | `TBD` |
| Decision date | `TBD` |
| Last reviewed | `Not reviewed` |

## 1. 任務目的

本文件只回答：

> 是否應依 `RESEARCH-TECH-RENDER-008` 所定義的安全邊界，授權執行 `RND-INSPECT-001` 至 `RND-INSPECT-018` 的本機唯讀查核？

這是 Authorization Request，不是：

- Inspection Record
- Inspection Execution
- Closure Authorization
- Build Authorization
- Runtime Spike Authorization
- Rendering Decision
- `TD-002` ADR

本文件本身不執行任何查核命令，不建立查核結果，不建立 Evidence 檔案，也不改變 Shared UI authority。

## 2. Scope

本文件只能申請：

- Standard-user 本機唯讀查核。
- 既有檔案與目錄 metadata 讀取。
- 既有 Registry value 讀取。
- 既有 AppX／Package inventory 讀取。
- 既有 NuGet configuration 與 cache metadata 讀取。
- 既有 SDK、Runtime、Build Tool、Targeting Pack 與 Native asset 盤點。
- 既有 UI Research evidence 的引用。
- 將未來輸出整理為明確的 Evidence Record，但本輪不建立該 Record。

不得申請：

- 網路存取。
- Package download。
- Package Restore。
- Tool、SDK、Runtime 或 workload 安裝。
- Project／Solution 建立。
- Build、Run、Publish。
- Registry、PATH、NuGet source 或 Cache 修改。
- Result directory 建立。
- Screenshot、Screen recording 或 Rendering output。
- 系統 Display、DPI、HDR 或 Power Plan 變更。
- 管理員權限。
- Runtime Spike。

## 3. Controlled Vocabulary

### 3.1 Authorization Decision

只能使用：

- `Pending`
- `Approved`
- `Approved with constraints`
- `Rejected`
- `Deferred`

本文件建立時所有 request 的值均為 `Pending`。

### 3.2 Execution Permission

只能使用 `No`。不得預填 `Yes`。

### 3.3 Request Status

只能使用：

- `Ready for human authorization review`
- `Conditionally ready for human authorization review`
- `Not ready for human authorization review`

### 3.4 Risk Classification

本文件只能使用 `R0 — Read-only local inspection`。不得把 R1 至 R4 當作本文件的已申請範圍。

### 3.5 Fixed authorization values

每一個 `RND-INSPECT-AUTH` 均固定使用下列值：

| Field | Fixed value |
|---|---|
| Required privilege | `Standard user` |
| Network access | `No` |
| Mutation expected | `No` |
| File creation expected | `No` |
| Registry write expected | `No` |
| Package/Cache mutation expected | `No` |
| Risk classification | `R0 — Read-only local inspection` |
| Requested authorization | `Required before execution` |
| Authorization Decision | `Pending` |
| Decision authority | `TBD` |
| Decision date | `TBD` |
| Execution permitted | `No` |
| Owner | `TBD` |

## 4. Request ID Binding

建立正好 18 組一對一 Binding：

| Authorization Request ID | Source Inspection Item |
|---|---|
| `RND-INSPECT-AUTH-001` | `RND-INSPECT-001` |
| `RND-INSPECT-AUTH-002` | `RND-INSPECT-002` |
| `RND-INSPECT-AUTH-003` | `RND-INSPECT-003` |
| `RND-INSPECT-AUTH-004` | `RND-INSPECT-004` |
| `RND-INSPECT-AUTH-005` | `RND-INSPECT-005` |
| `RND-INSPECT-AUTH-006` | `RND-INSPECT-006` |
| `RND-INSPECT-AUTH-007` | `RND-INSPECT-007` |
| `RND-INSPECT-AUTH-008` | `RND-INSPECT-008` |
| `RND-INSPECT-AUTH-009` | `RND-INSPECT-009` |
| `RND-INSPECT-AUTH-010` | `RND-INSPECT-010` |
| `RND-INSPECT-AUTH-011` | `RND-INSPECT-011` |
| `RND-INSPECT-AUTH-012` | `RND-INSPECT-012` |
| `RND-INSPECT-AUTH-013` | `RND-INSPECT-013` |
| `RND-INSPECT-AUTH-014` | `RND-INSPECT-014` |
| `RND-INSPECT-AUTH-015` | `RND-INSPECT-015` |
| `RND-INSPECT-AUTH-016` | `RND-INSPECT-016` |
| `RND-INSPECT-AUTH-017` | `RND-INSPECT-017` |
| `RND-INSPECT-AUTH-018` | `RND-INSPECT-018` |

Binding rules:

- 不得合併 Inspection Item。
- 不得拆分後改變原始查核語意。
- 不得增加第 19 個 Inspection Item。
- 不得在本文件新增查核命令。
- 所有 method／command 必須引用 `RESEARCH-TECH-RENDER-008`。
- 上游規格不足時記錄 `RND-INSPECT-AUTH-GAP-xxx`，不得修改上游。

## 5. Authorization Request Record Template

每一個 request record 必須包含下列欄位；第 6 節的 18 個 record 逐一套用本模板：

1. Authorization Request ID
2. Source Inspection Item
3. Inspection subject
4. Inspection question
5. Source Gap IDs
6. Related Enablement Items
7. Related Closure Gates
8. Related Candidate–Host Pairs
9. Planned method reference
10. Planned command／API reference
11. Safety classification
12. Exact authorized scope requested
13. Explicit exclusions
14. Execution environment
15. Required privilege
16. Network access
17. Mutation expected
18. File creation expected
19. Registry write expected
20. Package／Cache mutation expected
21. Sensitive-data exposure risk
22. Sensitive-data redaction requirement
23. Expected output fields
24. Future Evidence ID
25. Future Evidence destination
26. Success condition
27. Stop conditions
28. Failure／tool-missing handling
29. Conflict handling
30. Cleanup requirement
31. Risk classification
32. Requested authorization
33. Proposed constraints
34. Authorization Decision
35. Decision authority
36. Decision date
37. Execution permitted
38. Owner
39. Open questions

任何欄位未能以安全方式指定時，必須停留在 `TBD` 或建立 `RND-INSPECT-AUTH-GAP-xxx`，不得自行放寬權限。

## 6. Authorization Request Records

### 6.1 `RND-INSPECT-AUTH-001`

| Field | Request value |
|---|---|
| Authorization Request ID | `RND-INSPECT-AUTH-001` |
| Source Inspection Item | `RND-INSPECT-001` — Windows／architecture baseline inheritance |
| Inspection subject | Current Windows build、OS architecture、process architecture 與既有 UI baseline |
| Inspection question | Current Windows build、OS architecture、process architecture 與既有 UI baseline 是否可供 Rendering prerequisite 使用？ |
| Source Gap IDs | `RND-OFF-GAP-013`, `RND-OFF-GAP-015` |
| Related Enablement Items | `RND-ENABLE-001`, `RND-ENABLE-002`, `RND-ENABLE-005` |
| Related Closure Gates | `RND-CGATE-002`, `RND-CGATE-005` |
| Related Candidate–Host Pairs | `RND-PAIR-001..010` |
| Planned method reference | `RESEARCH-TECH-RENDER-008` / `RND-INSPECT-001`; 先引用 `RESEARCH-TECH-UI-006..009`，再補足缺欄位 |
| Planned command／API reference | `Get-CimInstance Win32_OperatingSystem`; `Get-CimInstance Win32_ComputerSystem`; `[Environment]::Is64BitOperatingSystem` |
| Safety classification | Environment inheritance；R0 read-only |
| Exact authorized scope requested | 只讀最小 Windows build、OS architecture、system type、current user context；不記錄完整使用者名稱 |
| Explicit exclusions | 不讀取無關使用者資料、序號、完整 machine identity、Registry export、Display mutation、Power Plan 或 Runtime output |
| Execution environment | PowerShell；目標工作區之外的系統唯讀查詢；僅在未來核准後執行 |
| Required privilege | `Standard user` |
| Network access | `No` |
| Mutation expected | `No` |
| File creation expected | `No` |
| Registry write expected | `No` |
| Package／Cache mutation expected | `No` |
| Sensitive-data exposure risk | user name、domain、serial、machine name、profile path |
| Sensitive-data redaction requirement | 遮罩 user name、domain、serial、machine name、完整 profile path |
| Expected output fields | Windows build、OS architecture、system type、current user context（最小化） |
| Future Evidence ID | `RND-LOCAL-EVID-001` |
| Future Evidence destination | `docs/Research/Technology/results/rendering/local-prerequisite-inspection/`；本輪不建立 |
| Success condition | 不變更系統即可取得最小 OS／architecture 欄位；不足時記錄 unavailable |
| Stop conditions | 需要管理員、網路、檔案輸出、Registry write、完整 identity 或與 008 不同的命令時立即停止 |
| Failure／tool-missing handling | 只記錄 command unavailable／permission denied；不安裝工具、不改用未核准命令 |
| Conflict handling | 與既有 UI evidence 不一致時標記 `Conflicting`，保留兩方來源，不自行選擇 |
| Cleanup requirement | 不建立檔案；Session output 只保留清理後欄位 |
| Risk classification | `R0 — Read-only local inspection` |
| Requested authorization | `Required before execution` |
| Proposed constraints | Standard user、No network、No mutation、No file output、引用既有 UI evidence 優先 |
| Authorization Decision | `Pending` |
| Decision authority | `TBD` |
| Decision date | `TBD` |
| Execution permitted | `No` |
| Owner | `TBD` |
| Open questions | 是否需要新增 OS evidence 欄位由授權者決定；不在本 request 自行擴大 |

### 6.2 `RND-INSPECT-AUTH-002`

| Field | Request value |
|---|---|
| Authorization Request ID | `RND-INSPECT-AUTH-002` |
| Source Inspection Item | `RND-INSPECT-002` — .NET SDK inventory |
| Inspection subject | 已存在的 .NET SDK versions、SDK root 與 command path |
| Inspection question | 已存在的 .NET SDK versions 是否足以描述未來 project target 的可選範圍？ |
| Source Gap IDs | `RND-OFF-GAP-001`, `RND-OFF-GAP-006`, `RND-OFF-GAP-007`, `RND-OFF-GAP-011` |
| Related Enablement Items | `RND-ENABLE-001`, `RND-ENABLE-005` |
| Related Closure Gates | `RND-CGATE-003`, `RND-CGATE-004` |
| Related Candidate–Host Pairs | `RND-PAIR-001..010` |
| Planned method reference | `RESEARCH-TECH-RENDER-008` / `RND-INSPECT-002`；只查既有 executable 與 SDK list |
| Planned command／API reference | `Get-Command dotnet`; `dotnet --list-sdks` |
| Safety classification | Process inventory read；R0 read-only |
| Exact authorized scope requested | 讀取既有 SDK version、SDK root 與 command path |
| Explicit exclusions | 不 restore、下載、安裝、修改 SDK、修改 PATH、建立 project、Build、Run 或輸出完整 user path |
| Execution environment | PowerShell standard user；僅在未來授權後執行 |
| Required privilege | `Standard user` |
| Network access | `No` |
| Mutation expected | `No` |
| File creation expected | `No` |
| Registry write expected | `No` |
| Package／Cache mutation expected | `No` |
| Sensitive-data exposure risk | SDK root、command path 可能包含 user profile 或機器資訊 |
| Sensitive-data redaction requirement | 遮罩 user profile path 與 machine-specific path 片段 |
| Expected output fields | SDK version、SDK root（清理後）、command path（清理後） |
| Future Evidence ID | `RND-LOCAL-EVID-002` |
| Future Evidence destination | Future local inspection evidence root；本輪不建立 |
| Success condition | 取得既有 SDK version list，或明確記錄 command unavailable |
| Stop conditions | `dotnet` 嘗試 network、restore、寫入 cache、需要檔案輸出或回傳非唯讀行為時停止 |
| Failure／tool-missing handling | 記錄 command unavailable；不安裝 .NET SDK、不改用 package manager |
| Conflict handling | 與官方版本資料或既有 UI evidence 不一致時標記版本 evidence conflict，不判定 unsupported |
| Cleanup requirement | 不建立檔案；清理 command path 後才可保留 Session output |
| Risk classification | `R0 — Read-only local inspection` |
| Requested authorization | `Required before execution` |
| Proposed constraints | 只執行 008 指定命令；不使用 `dotnet restore`、`dotnet build`、`dotnet run`、`dotnet new` |
| Authorization Decision | `Pending` |
| Decision authority | `TBD` |
| Decision date | `TBD` |
| Execution permitted | `No` |
| Owner | `TBD` |
| Open questions | Target framework 是否需另行定義，留待後續 research，不由本 request 推定 |

### 6.3 `RND-INSPECT-AUTH-003`

| Field | Request value |
|---|---|
| Authorization Request ID | `RND-INSPECT-AUTH-003` |
| Source Inspection Item | `RND-INSPECT-003` — .NET Runtime／Windows Desktop Runtime inventory |
| Inspection subject | 已存在的 .NET Runtime 與 Windows Desktop Runtime |
| Inspection question | 已存在的 .NET Runtime 與 Windows Desktop Runtime 是否可供未來 Host 啟動前置檢查？ |
| Source Gap IDs | `RND-OFF-GAP-001`, `RND-OFF-GAP-002`, `RND-OFF-GAP-006`, `RND-OFF-GAP-007` |
| Related Enablement Items | `RND-ENABLE-001`, `RND-ENABLE-002`, `RND-ENABLE-005` |
| Related Closure Gates | `RND-CGATE-002`, `RND-CGATE-003`, `RND-CGATE-005` |
| Related Candidate–Host Pairs | `RND-PAIR-001`, `RND-PAIR-002`, `RND-PAIR-007`, `RND-PAIR-008` |
| Planned method reference | `RESEARCH-TECH-RENDER-008` / `RND-INSPECT-003`；讀取 installed runtime list，不啟動 project |
| Planned command／API reference | `dotnet --list-runtimes`; targeted `Get-ChildItem` runtime metadata only |
| Safety classification | Process inventory read；R0 read-only |
| Exact authorized scope requested | 讀取 Runtime name、version、architecture（若 exposed）與 command path |
| Explicit exclusions | 不啟動應用程式碼、不建立 project、不 restore、不安裝 Runtime、不修改 runtime files |
| Execution environment | PowerShell standard user；僅在未來授權後執行 |
| Required privilege | `Standard user` |
| Network access | `No` |
| Mutation expected | `No` |
| File creation expected | `No` |
| Registry write expected | `No` |
| Package／Cache mutation expected | `No` |
| Sensitive-data exposure risk | user path、machine-specific installation path |
| Sensitive-data redaction requirement | 遮罩 user path 與 machine-specific installation path |
| Expected output fields | Runtime name、version、architecture（若有）、command path |
| Future Evidence ID | `RND-LOCAL-EVID-003` |
| Future Evidence destination | Future local inspection evidence root；本輪不建立 |
| Success condition | 不啟動 application code 即可取得 runtime inventory，或標記 unavailable |
| Stop conditions | 需要啟動 project、要求網路／安裝／寫入或需未核准查核時停止 |
| Failure／tool-missing handling | 記錄 runtime command unavailable；不安裝 SDK／Runtime |
| Conflict handling | Runtime version 與 target framework 不一致時只記錄 conflict，不宣稱 build failure |
| Cleanup requirement | 不建立檔案；不清理既有 cache |
| Risk classification | `R0 — Read-only local inspection` |
| Requested authorization | `Required before execution` |
| Proposed constraints | 僅 runtime inventory；不啟動任何 project 或 runtime spike |
| Authorization Decision | `Pending` |
| Decision authority | `TBD` |
| Decision date | `TBD` |
| Execution permitted | `No` |
| Owner | `TBD` |
| Open questions | Windows Desktop Runtime 的 host binding 是否另需 build evidence，留待後續 gate |

### 6.4 `RND-INSPECT-AUTH-004`

| Field | Request value |
|---|---|
| Authorization Request ID | `RND-INSPECT-AUTH-004` |
| Source Inspection Item | `RND-INSPECT-004` — Installed workload inventory |
| Inspection subject | 已存在的 .NET workload、manifest version 與 advertising state |
| Inspection question | 已存在的 .NET workload 是否可被識別，且是否影響 WinUI 3／WPF future project setup？ |
| Source Gap IDs | `RND-OFF-GAP-001`, `RND-OFF-GAP-006`, `RND-OFF-GAP-011` |
| Related Enablement Items | `RND-ENABLE-001`, `RND-ENABLE-005` |
| Related Closure Gates | `RND-CGATE-003`, `RND-CGATE-004` |
| Related Candidate–Host Pairs | `RND-PAIR-001`, `RND-PAIR-005`, `RND-PAIR-007` |
| Planned method reference | `RESEARCH-TECH-RENDER-008` / `RND-INSPECT-004`；讀取 workload list |
| Planned command／API reference | `dotnet workload list` |
| Safety classification | Process inventory read；R0 read-only |
| Exact authorized scope requested | 讀取 installed workload IDs、manifest version、advertising state（若顯示） |
| Explicit exclusions | 禁止 workload install/update、restore、下載、建立 project、修改 manifest 或 cache |
| Execution environment | PowerShell standard user；僅在未來授權後執行 |
| Required privilege | `Standard user` |
| Network access | `No` |
| Mutation expected | `No` |
| File creation expected | `No` |
| Registry write expected | `No` |
| Package／Cache mutation expected | `No` |
| Sensitive-data exposure risk | source credential、machine identifier、profile path |
| Sensitive-data redaction requirement | 不輸出 source credential、machine identifier 或完整 profile path |
| Expected output fields | Installed workload IDs、manifest version、advertising state（若有） |
| Future Evidence ID | `RND-LOCAL-EVID-004` |
| Future Evidence destination | Future local inspection evidence root；本輪不建立 |
| Success condition | Workload list 以唯讀文字取得，或明確標記 unavailable |
| Stop conditions | workload command 嘗試網路、update、install、寫入或要求管理員時停止 |
| Failure／tool-missing handling | 記錄 command unavailable；不執行任何 workload 修復 |
| Conflict handling | workload list 與官方 enablement evidence 不一致時保留 conflict，不自行關閉 gap |
| Cleanup requirement | 不建立輸出檔案；不修改 workload state |
| Risk classification | `R0 — Read-only local inspection` |
| Requested authorization | `Required before execution` |
| Proposed constraints | 僅 `dotnet workload list`；明確禁止 install/update |
| Authorization Decision | `Pending` |
| Decision authority | `TBD` |
| Decision date | `TBD` |
| Execution permitted | `No` |
| Owner | `TBD` |
| Open questions | workload advertising state 是否可靠，只能作為 observation，不作為支援性結論 |

### 6.5 `RND-INSPECT-AUTH-005`

| Field | Request value |
|---|---|
| Authorization Request ID | `RND-INSPECT-AUTH-005` |
| Source Inspection Item | `RND-INSPECT-005` — Visual Studio／Build Tools／vswhere availability |
| Inspection subject | Existing Visual Studio、Build Tools、`vswhere` 與 installed instance metadata |
| Inspection question | Existing Visual Studio、Build Tools 與 `vswhere` 是否能被定位，且 provenance 是否可記錄？ |
| Source Gap IDs | `RND-OFF-GAP-001`, `RND-OFF-GAP-003`, `RND-OFF-GAP-004`, `RND-OFF-GAP-015` |
| Related Enablement Items | `RND-ENABLE-001`, `RND-ENABLE-005` |
| Related Closure Gates | `RND-CGATE-002`, `RND-CGATE-004`, `RND-CGATE-005` |
| Related Candidate–Host Pairs | `RND-PAIR-001..010` |
| Planned method reference | `RESEARCH-TECH-RENDER-008` / `RND-INSPECT-005`；定位既有 executable 並查詢 installed instance metadata |
| Planned command／API reference | `Get-Command vswhere.exe`; `vswhere.exe -products * -format json` |
| Safety classification | Process inventory read；R0 read-only |
| Exact authorized scope requested | 讀取 installation path、product ID、display version、catalog version、MSBuild component presence |
| Explicit exclusions | 不啟動 IDE、Installer、Build、Restore、安裝 `vswhere`、修改 VS 或 license state |
| Execution environment | PowerShell standard user；僅在未來授權後執行 |
| Required privilege | `Standard user` |
| Network access | `No` |
| Mutation expected | `No` |
| File creation expected | `No` |
| Registry write expected | `No` |
| Package／Cache mutation expected | `No` |
| Sensitive-data exposure risk | user path、subscription/license fields、machine name |
| Sensitive-data redaction requirement | 遮罩完整 user path、subscription／license fields、machine name |
| Expected output fields | Installation path（清理後）、product ID、display version、catalog version、MSBuild component presence |
| Future Evidence ID | `RND-LOCAL-EVID-005` |
| Future Evidence destination | Future local inspection evidence root；本輪不建立 |
| Success condition | 不啟動 IDE 即可描述 existing tool availability 與 provenance |
| Stop conditions | `vswhere` 不存在而需要安裝、查詢要求管理員／網路／寫入或輸出 credential 時停止 |
| Failure／tool-missing handling | 記錄 tool unavailable；不自動安裝 `vswhere` |
| Conflict handling | VS metadata 與 MSBuild metadata 不一致時標記 conflict，分開保留 provenance |
| Cleanup requirement | 不建立檔案；不修改 installation 或 Registry |
| Risk classification | `R0 — Read-only local inspection` |
| Requested authorization | `Required before execution` |
| Proposed constraints | 只讀既有 instance；不得啟動 Visual Studio Installer |
| Authorization Decision | `Pending` |
| Decision authority | `TBD` |
| Decision date | `TBD` |
| Execution permitted | `No` |
| Owner | `TBD` |
| Open questions | MSBuild component provenance 是否完整，留待 read-only output 與後續 gate 判斷 |

### 6.6 `RND-INSPECT-AUTH-006`

| Field | Request value |
|---|---|
| Authorization Request ID | `RND-INSPECT-AUTH-006` |
| Source Inspection Item | `RND-INSPECT-006` — MSBuild path and provenance |
| Inspection subject | 可用 MSBuild path、file version 與 parent installation provenance |
| Inspection question | 可用 MSBuild path、file version 與 parent installation provenance 是否可被唯讀記錄？ |
| Source Gap IDs | `RND-OFF-GAP-003`, `RND-OFF-GAP-004`, `RND-OFF-GAP-006`, `RND-OFF-GAP-007`, `RND-OFF-GAP-015` |
| Related Enablement Items | `RND-ENABLE-001`, `RND-ENABLE-005` |
| Related Closure Gates | `RND-CGATE-004`, `RND-CGATE-005` |
| Related Candidate–Host Pairs | `RND-PAIR-003`, `RND-PAIR-004`, `RND-PAIR-007`, `RND-PAIR-008` |
| Planned method reference | `RESEARCH-TECH-RENDER-008` / `RND-INSPECT-006`；定位 executable 並讀取 file metadata/hash only if already known |
| Planned command／API reference | `Get-Command msbuild.exe`; `Get-Item`; `[System.Diagnostics.FileVersionInfo]::GetVersionInfo(...)` |
| Safety classification | Process and file metadata read；R0 read-only |
| Exact authorized scope requested | 讀取 resolved path、file version、product version、source installation |
| Explicit exclusions | 不執行 MSBuild、不 Build、不修改檔案、不 hash 未核准的大型範圍、不列出無關 installation |
| Execution environment | PowerShell standard user；僅在未來授權後執行 |
| Required privilege | `Standard user` |
| Network access | `No` |
| Mutation expected | `No` |
| File creation expected | `No` |
| Registry write expected | `No` |
| Package／Cache mutation expected | `No` |
| Sensitive-data exposure risk | user path、machine name、unrelated installation paths |
| Sensitive-data redaction requirement | 遮罩 user path、machine name、unrelated installation paths |
| Expected output fields | Resolved path、file version、product version、source installation |
| Future Evidence ID | `RND-LOCAL-EVID-006` |
| Future Evidence destination | Future local inspection evidence root；本輪不建立 |
| Success condition | 不執行 build 即可捕捉 MSBuild provenance |
| Stop conditions | path 未知而需廣泛掃描、命令需要 build／network／write、或無法確認唯讀性時停止 |
| Failure／tool-missing handling | 若 `msbuild.exe` 不可用，reuse `RND-INSPECT-005` output；不安裝或修復工具 |
| Conflict handling | multiple MSBuild instances 分別記錄，不自行選定 active path |
| Cleanup requirement | 不建立檔案；不改變 executable metadata |
| Risk classification | `R0 — Read-only local inspection` |
| Requested authorization | `Required before execution` |
| Proposed constraints | 只讀既有 path/version/provenance；禁止 `/Build` |
| Authorization Decision | `Pending` |
| Decision authority | `TBD` |
| Decision date | `TBD` |
| Execution permitted | `No` |
| Owner | `TBD` |
| Open questions | 若無直接 MSBuild path，是否足以由 VS instance evidence 推導，留待 decision authority |

### 6.7 `RND-INSPECT-AUTH-007`

| Field | Request value |
|---|---|
| Authorization Request ID | `RND-INSPECT-AUTH-007` |
| Source Inspection Item | `RND-INSPECT-007` — Windows SDK version roots |
| Inspection subject | Existing Windows SDK version roots、Include／Lib／bin locations |
| Inspection question | Existing Windows SDK version roots and Include／Lib／bin locations 是否存在？ |
| Source Gap IDs | `RND-OFF-GAP-003`, `RND-OFF-GAP-004`, `RND-OFF-GAP-015` |
| Related Enablement Items | `RND-ENABLE-001`, `RND-ENABLE-005` |
| Related Closure Gates | `RND-CGATE-003`, `RND-CGATE-005` |
| Related Candidate–Host Pairs | `RND-PAIR-003`, `RND-PAIR-004` |
| Planned method reference | `RESEARCH-TECH-RENDER-008` / `RND-INSPECT-007`；讀取 known Registry location 與 targeted directories |
| Planned command／API reference | `Get-ItemProperty HKLM:\SOFTWARE\Microsoft\Windows Kits\Installed Roots`; targeted `Test-Path` |
| Safety classification | Registry read；R0 read-only |
| Exact authorized scope requested | 讀取 SDK version root、Include／Lib／bin existence 與 architecture subfolders |
| Explicit exclusions | 不 Registry write、不列出無關 Registry、不廣泛掃描 SDK、不建立 SDK path |
| Execution environment | PowerShell standard user；僅在未來授權後執行 |
| Required privilege | `Standard user` |
| Network access | `No` |
| Mutation expected | `No` |
| File creation expected | `No` |
| Registry write expected | `No` |
| Package／Cache mutation expected | `No` |
| Sensitive-data exposure risk | machine identifiers、unrelated Registry values |
| Sensitive-data redaction requirement | 不輸出 unrelated Registry values 或 machine identifiers |
| Expected output fields | SDK version root、Include／Lib／bin existence、architecture subfolders |
| Future Evidence ID | `RND-LOCAL-EVID-007` |
| Future Evidence destination | Future local inspection evidence root；本輪不建立 |
| Success condition | 不 Registry write 即可捕捉 existing Windows SDK root metadata |
| Stop conditions | Registry path 不明而需廣泛 export、需要 admin、嘗試寫入或資料超出 known location 時停止 |
| Failure／tool-missing handling | 記錄 Registry key unavailable；不建立或修改 SDK root |
| Conflict handling | Registry roots 與 filesystem existence 不一致時分別記錄，不自行修正 |
| Cleanup requirement | 不建立檔案；不修改 Registry |
| Risk classification | `R0 — Read-only local inspection` |
| Requested authorization | `Required before execution` |
| Proposed constraints | 僅 read-only known Registry key 與 targeted `Test-Path` |
| Authorization Decision | `Pending` |
| Decision authority | `TBD` |
| Decision date | `TBD` |
| Execution permitted | `No` |
| Owner | `TBD` |
| Open questions | SDK root architecture folder是否完整，須以既有 path observation 為準 |

### 6.8 `RND-INSPECT-AUTH-008`

| Field | Request value |
|---|---|
| Authorization Request ID | `RND-INSPECT-AUTH-008` |
| Source Inspection Item | `RND-INSPECT-008` — Direct2D／DirectWrite development assets |
| Inspection subject | Existing `d2d1.h`、`dwrite.h`、libraries 與相關 metadata |
| Inspection question | Existing headers、metadata、libraries 與 related Direct2D／DirectWrite assets 是否可被定位？ |
| Source Gap IDs | `RND-OFF-GAP-003`, `RND-OFF-GAP-004`, `RND-OFF-GAP-015` |
| Related Enablement Items | `RND-ENABLE-005` |
| Related Closure Gates | `RND-CGATE-004`, `RND-CGATE-005` |
| Related Candidate–Host Pairs | `RND-PAIR-003`, `RND-PAIR-004` |
| Planned method reference | `RESEARCH-TECH-RENDER-008` / `RND-INSPECT-008`；使用 007 SDK roots，檢查 targeted assets |
| Planned command／API reference | Targeted `Test-Path`; `Get-Item` on known `d2d1.h`, `dwrite.h`, library metadata paths |
| Safety classification | File-system metadata read；R0 read-only |
| Exact authorized scope requested | 只讀 headers、libraries、version metadata（若有）與 architecture folders |
| Explicit exclusions | 不 recurse unrelated SDK、compile、load native code、建立 asset、下載 SDK、修改檔案 |
| Execution environment | PowerShell standard user；僅在未來授權後執行 |
| Required privilege | `Standard user` |
| Network access | `No` |
| Mutation expected | `No` |
| File creation expected | `No` |
| Registry write expected | `No` |
| Package／Cache mutation expected | `No` |
| Sensitive-data exposure risk | machine-specific path、unrelated SDK directory names |
| Sensitive-data redaction requirement | 不 recurse unrelated SDK，並遮罩 machine-specific path |
| Expected output fields | Header／library path（清理後）、version metadata、architecture folders |
| Future Evidence ID | `RND-LOCAL-EVID-008` |
| Future Evidence destination | Future local inspection evidence root；本輪不建立 |
| Success condition | 不 compile 或 load native code 即可記錄 asset existence |
| Stop conditions | SDK root 未知而需廣泛掃描、需要 build／network／write、或命令無法證明唯讀時停止 |
| Failure／tool-missing handling | 記錄 asset unavailable；不安裝 Windows SDK、不建立替代檔案 |
| Conflict handling | Header/library version 不一致時標記 conflict，不推導 runtime compatibility |
| Cleanup requirement | 不建立檔案；不修改 SDK assets |
| Risk classification | `R0 — Read-only local inspection` |
| Requested authorization | `Required before execution` |
| Proposed constraints | 只使用 007 既有 roots；只讀 known asset paths |
| Authorization Decision | `Pending` |
| Decision authority | `TBD` |
| Decision date | `TBD` |
| Execution permitted | `No` |
| Owner | `TBD` |
| Open questions | Native asset presence 仍不等於 build 或 runtime evidence |

### 6.9 `RND-INSPECT-AUTH-009`

| Field | Request value |
|---|---|
| Authorization Request ID | `RND-INSPECT-AUTH-009` |
| Source Inspection Item | `RND-INSPECT-009` — Windows App SDK Runtime package inventory |
| Inspection subject | Existing Windows App SDK Runtime packages、architecture 與 registration state |
| Inspection question | Existing Windows App SDK Runtime packages、architecture 與 registration state 是否可被唯讀記錄？ |
| Source Gap IDs | `RND-OFF-GAP-001`, `RND-OFF-GAP-006`, `RND-OFF-GAP-015` |
| Related Enablement Items | `RND-ENABLE-001`, `RND-ENABLE-002`, `RND-ENABLE-005` |
| Related Closure Gates | `RND-CGATE-002`, `RND-CGATE-005` |
| Related Candidate–Host Pairs | `RND-PAIR-001`, `RND-PAIR-005`, `RND-PAIR-007` |
| Planned method reference | `RESEARCH-TECH-RENDER-008` / `RND-INSPECT-009`；只查既有 AppX package metadata |
| Planned command／API reference | `Get-AppxPackage -Name Microsoft.WindowsAppRuntime*` |
| Safety classification | AppX inventory read；R0 read-only |
| Exact authorized scope requested | 讀取 package name、version、architecture、status、publisher |
| Explicit exclusions | 不 Register、Install、Remove、下載 package、不讀取不相關 package content |
| Execution environment | PowerShell standard user；僅在未來授權後執行 |
| Required privilege | `Standard user` |
| Network access | `No` |
| Mutation expected | `No` |
| File creation expected | `No` |
| Registry write expected | `No` |
| Package／Cache mutation expected | `No` |
| Sensitive-data exposure risk | user SID、install location details |
| Sensitive-data redaction requirement | 遮罩 user SID 與不需要的 install location details |
| Expected output fields | Package name、version、architecture、status、publisher |
| Future Evidence ID | `RND-LOCAL-EVID-009` |
| Future Evidence destination | Future local inspection evidence root；本輪不建立 |
| Success condition | 不 Register／Install action 即可列出 existing package metadata |
| Stop conditions | 命令要求 admin、Package mutation、network、或輸出 user SID 時停止 |
| Failure／tool-missing handling | 記錄 AppX inventory unavailable；不啟動 App Installer 或其他修復流程 |
| Conflict handling | package registration 與 SDK/cache evidence 不一致時分開標記，不判定 support |
| Cleanup requirement | 不建立檔案；不改變 AppX state |
| Risk classification | `R0 — Read-only local inspection` |
| Requested authorization | `Required before execution` |
| Proposed constraints | 僅指定 package name filter；不得使用 Register／Add／Remove 操作 |
| Authorization Decision | `Pending` |
| Decision authority | `TBD` |
| Decision date | `TBD` |
| Execution permitted | `No` |
| Owner | `TBD` |
| Open questions | Runtime package presence 是否足以支持 Host，必須保留為後續 build/runtime question |

### 6.10 `RND-INSPECT-AUTH-010`

| Field | Request value |
|---|---|
| Authorization Request ID | `RND-INSPECT-AUTH-010` |
| Source Inspection Item | `RND-INSPECT-010` — Windows App SDK SDK／NuGet cache assets |
| Inspection subject | Existing Windows App SDK SDK、global-packages path 與 named package roots |
| Inspection question | Existing Windows App SDK SDK and NuGet cache assets 是否存在且版本可被辨識？ |
| Source Gap IDs | `RND-OFF-GAP-001`, `RND-OFF-GAP-006`, `RND-OFF-GAP-011` |
| Related Enablement Items | `RND-ENABLE-001`, `RND-ENABLE-005` |
| Related Closure Gates | `RND-CGATE-003`, `RND-CGATE-004` |
| Related Candidate–Host Pairs | `RND-PAIR-001`, `RND-PAIR-005`, `RND-PAIR-007` |
| Planned method reference | `RESEARCH-TECH-RENDER-008` / `RND-INSPECT-010`；先解析既有 global-packages path，再讀 named roots |
| Planned command／API reference | `dotnet nuget locals global-packages --list`; targeted `Test-Path` and `Get-Item` |
| Safety classification | File-system metadata read；R0 read-only |
| Exact authorized scope requested | 讀取 cache path、package ID、version folder、TFM／RID asset folder presence |
| Explicit exclusions | 不 restore、download、清 cache、寫入 cache、讀取無關 package、建立 result directory |
| Execution environment | PowerShell standard user；僅在未來授權後執行 |
| Required privilege | `Standard user` |
| Network access | `No` |
| Mutation expected | `No` |
| File creation expected | `No` |
| Registry write expected | `No` |
| Package／Cache mutation expected | `No` |
| Sensitive-data exposure risk | username、source credentials、unrelated package names |
| Sensitive-data redaction requirement | 遮罩 username、source credentials 與 unrelated package names |
| Expected output fields | Cache path（清理後）、package ID、version folder、TFM／RID asset folder presence |
| Future Evidence ID | `RND-LOCAL-EVID-010` |
| Future Evidence destination | Future local inspection evidence root；本輪不建立 |
| Success condition | 不 restore 或 cache mutation 即可讀取 named cache metadata |
| Stop conditions | global-packages command 觸發 network／restore／write、path 不可限制或需建立路徑時停止 |
| Failure／tool-missing handling | 記錄 cache path unavailable；不自行推測 package absence |
| Conflict handling | package cache identity 與 official package evidence 不一致時標記 version/identity conflict |
| Cleanup requirement | 不清理 package cache；不建立任何檔案 |
| Risk classification | `R0 — Read-only local inspection` |
| Requested authorization | `Required before execution` |
| Proposed constraints | 只讀既有 path 與 named package roots；禁止 restore/cache clean |
| Authorization Decision | `Pending` |
| Decision authority | `TBD` |
| Decision date | `TBD` |
| Execution permitted | `No` |
| Owner | `TBD` |
| Open questions | SDK cache 是否存在與 package build compatibility 必須分離判斷 |

### 6.11 `RND-INSPECT-AUTH-011`

| Field | Request value |
|---|---|
| Authorization Request ID | `RND-INSPECT-AUTH-011` |
| Source Inspection Item | `RND-INSPECT-011` — WinUI 3 template and MSBuild target assets |
| Inspection subject | Existing WinUI 3 templates、targets、props 與 SDK assets |
| Inspection question | Existing WinUI 3 templates、targets、props 與 SDK asset 是否可被唯讀辨識？ |
| Source Gap IDs | `RND-OFF-GAP-001`, `RND-OFF-GAP-006`, `RND-OFF-GAP-011` |
| Related Enablement Items | `RND-ENABLE-001`, `RND-ENABLE-005` |
| Related Closure Gates | `RND-CGATE-002`, `RND-CGATE-004` |
| Related Candidate–Host Pairs | `RND-PAIR-001`, `RND-PAIR-005`, `RND-PAIR-007` |
| Planned method reference | `RESEARCH-TECH-RENDER-008` / `RND-INSPECT-011`；列出 existing templates，讀 named SDK target metadata |
| Planned command／API reference | `dotnet new list`; targeted `Get-Item`／`Get-Content` on already identified SDK target files |
| Safety classification | File-system metadata read；R0 read-only |
| Exact authorized scope requested | 讀取 template identity、installed version、target path、required package identity |
| Explicit exclusions | 不使用 `dotnet new` 建立 project、不 restore、不 build、不讀 credential-bearing config、不列出無關 templates |
| Execution environment | PowerShell standard user；僅在未來授權後執行 |
| Required privilege | `Standard user` |
| Network access | `No` |
| Mutation expected | `No` |
| File creation expected | `No` |
| Registry write expected | `No` |
| Package／Cache mutation expected | `No` |
| Sensitive-data exposure risk | user path、credential-bearing config、unrelated template metadata |
| Sensitive-data redaction requirement | 不 dump credential-bearing config；遮罩 user path；只保留 named templates |
| Expected output fields | Template identity、installed version、target path、required package identity |
| Future Evidence ID | `RND-LOCAL-EVID-011` |
| Future Evidence destination | Future local inspection evidence root；本輪不建立 |
| Success condition | 不建立 project 即可取得 existing template/target metadata |
| Stop conditions | `dotnet new` 需要 create、network、restore、write，或 target file 未被 008 指定時停止 |
| Failure／tool-missing handling | 記錄 template/target unavailable；不建立替代 project |
| Conflict handling | template、target、package identity 不一致時保留各自 evidence，不推導 build support |
| Cleanup requirement | 不建立 project 或輸出檔案；不清除既有 metadata |
| Risk classification | `R0 — Read-only local inspection` |
| Requested authorization | `Required before execution` |
| Proposed constraints | `dotnet new list` 只能作 inventory；禁止 `dotnet new` 建立行為 |
| Authorization Decision | `Pending` |
| Decision authority | `TBD` |
| Decision date | `TBD` |
| Execution permitted | `No` |
| Owner | `TBD` |
| Open questions | Template list 是否需依 host 分組，留待後續 enablement reassessment |

### 6.12 `RND-INSPECT-AUTH-012`

| Field | Request value |
|---|---|
| Authorization Request ID | `RND-INSPECT-AUTH-012` |
| Source Inspection Item | `RND-INSPECT-012` — WPF targeting pack and reference assemblies |
| Inspection subject | WPF targeting pack、reference assemblies 與 known build paths |
| Inspection question | WPF targeting pack、reference assemblies 與 build path 是否存在？ |
| Source Gap IDs | `RND-OFF-GAP-002`, `RND-OFF-GAP-007`, `RND-OFF-GAP-015` |
| Related Enablement Items | `RND-ENABLE-001`, `RND-ENABLE-005` |
| Related Closure Gates | `RND-CGATE-002`, `RND-CGATE-004`, `RND-CGATE-005` |
| Related Candidate–Host Pairs | `RND-PAIR-002`, `RND-PAIR-004`, `RND-PAIR-006`, `RND-PAIR-008`, `RND-PAIR-010` |
| Planned method reference | `RESEARCH-TECH-RENDER-008` / `RND-INSPECT-012`；只讀 targeted `Reference Assemblies` 與 known WPF target paths |
| Planned command／API reference | Targeted `Test-Path`; `Get-Item` on known WPF reference assembly paths |
| Safety classification | File-system metadata read；R0 read-only |
| Exact authorized scope requested | 讀取 TFM folder、WPF assembly file version、reference path |
| Explicit exclusions | 不 recurse unrelated framework folders、不建立 project、不 build、不 restore、不安裝 targeting pack |
| Execution environment | PowerShell standard user；僅在未來授權後執行 |
| Required privilege | `Standard user` |
| Network access | `No` |
| Mutation expected | `No` |
| File creation expected | `No` |
| Registry write expected | `No` |
| Package／Cache mutation expected | `No` |
| Sensitive-data exposure risk | machine-specific path、unrelated framework folder names |
| Sensitive-data redaction requirement | 不 recurse unrelated folders，並遮罩 machine-specific path |
| Expected output fields | TFM folder、WPF assembly file version、reference path（清理後） |
| Future Evidence ID | `RND-LOCAL-EVID-012` |
| Future Evidence destination | Future local inspection evidence root；本輪不建立 |
| Success condition | 不建立 project 即可記錄 existing WPF targeting assets |
| Stop conditions | 需要廣泛 framework scan、Build、Restore、install、network 或 write 時停止 |
| Failure／tool-missing handling | 記錄 WPF targeting asset unavailable；不安裝或修復 targeting pack |
| Conflict handling | WPF asset 與 official TFM evidence 不一致時標記 conflict，不判定 unsupported |
| Cleanup requirement | 不建立檔案；不修改 reference assemblies |
| Risk classification | `R0 — Read-only local inspection` |
| Requested authorization | `Required before execution` |
| Proposed constraints | 只讀 known WPF reference paths；禁止 broad framework enumeration |
| Authorization Decision | `Pending` |
| Decision authority | `TBD` |
| Decision date | `TBD` |
| Execution permitted | `No` |
| Owner | `TBD` |
| Open questions | WPF build path 是否需獨立候選 pair evidence，留待後續 build gate |

### 6.13 `RND-INSPECT-AUTH-013`

| Field | Request value |
|---|---|
| Authorization Request ID | `RND-INSPECT-AUTH-013` |
| Source Inspection Item | `RND-INSPECT-013` — Win2D cached package identity and versions |
| Inspection subject | `Microsoft.Graphics.Win2D` cached package ID、version、TFM、native asset folders |
| Inspection question | `Microsoft.Graphics.Win2D` cached package ID、version、TFM 與 native asset folders 是否存在？ |
| Source Gap IDs | `RND-OFF-GAP-005`, `RND-OFF-GAP-011` |
| Related Enablement Items | `RND-ENABLE-005` |
| Related Closure Gates | `RND-CGATE-003`, `RND-CGATE-004`, `RND-CGATE-005` |
| Related Candidate–Host Pairs | `RND-PAIR-005`, `RND-PAIR-006` |
| Planned method reference | `RESEARCH-TECH-RENDER-008` / `RND-INSPECT-013`；只讀 existing package root、nuspec、named asset folders |
| Planned command／API reference | Targeted `Get-Item`; `Get-Content` for existing `.nuspec`; no restore |
| Safety classification | Package metadata read；R0 read-only |
| Exact authorized scope requested | 讀取 Package ID、version、TFM、RID、native asset folder、dependency metadata |
| Explicit exclusions | 不 package acquisition、不 restore、不改 cache、不下載、不讀無關 package、不修改 `.nuspec` |
| Execution environment | PowerShell standard user；僅在未來授權後執行 |
| Required privilege | `Standard user` |
| Network access | `No` |
| Mutation expected | `No` |
| File creation expected | `No` |
| Registry write expected | `No` |
| Package／Cache mutation expected | `No` |
| Sensitive-data exposure risk | cache username、unrelated packages |
| Sensitive-data redaction requirement | 遮罩 cache username，忽略 unrelated packages |
| Expected output fields | Package ID、version、TFM、RID、native asset folder、dependency metadata |
| Future Evidence ID | `RND-LOCAL-EVID-013` |
| Future Evidence destination | Future local inspection evidence root；本輪不建立 |
| Success condition | 不 package acquisition 即可取得 existing Win2D metadata |
| Stop conditions | package root 不明而需 broad scan、任何 restore/network/write 或需建立 cache 時停止 |
| Failure／tool-missing handling | 記錄 package metadata unavailable；不補裝 Win2D |
| Conflict handling | Cached version 與 official package version 不一致時標記 version conflict，不自行選定版本 |
| Cleanup requirement | 不清理 cache、不建立檔案、不修改 package metadata |
| Risk classification | `R0 — Read-only local inspection` |
| Requested authorization | `Required before execution` |
| Proposed constraints | 僅 existing named package root 與 `.nuspec`；禁止 restore |
| Authorization Decision | `Pending` |
| Decision authority | `TBD` |
| Decision date | `TBD` |
| Execution permitted | `No` |
| Owner | `TBD` |
| Open questions | Win2D cached presence 不等於 host/build/runtime compatibility |

### 6.14 `RND-INSPECT-AUTH-014`

| Field | Request value |
|---|---|
| Authorization Request ID | `RND-INSPECT-AUTH-014` |
| Source Inspection Item | `RND-INSPECT-014` — SkiaSharp cached packages and native assets |
| Inspection subject | SkiaSharp core、WPF／WinUI view、Win32／WinUI native asset package |
| Inspection question | SkiaSharp core、WPF／WinUI view、Win32／WinUI native asset package 的 cached identity and versions 是否存在？ |
| Source Gap IDs | `RND-OFF-GAP-006`, `RND-OFF-GAP-007`, `RND-OFF-GAP-010`, `RND-OFF-GAP-015` |
| Related Enablement Items | `RND-ENABLE-005` |
| Related Closure Gates | `RND-CGATE-003`, `RND-CGATE-004`, `RND-CGATE-005` |
| Related Candidate–Host Pairs | `RND-PAIR-007`, `RND-PAIR-008` |
| Planned method reference | `RESEARCH-TECH-RENDER-008` / `RND-INSPECT-014`；只讀 named package roots、nuspec dependency metadata、native asset folders |
| Planned command／API reference | Targeted `Get-Item`; `Get-Content` existing `.nuspec`; no package acquisition |
| Safety classification | Package metadata read；R0 read-only |
| Exact authorized scope requested | 讀取 core/view/native package IDs、versions、TFM、RID、native asset folder、dependency relation |
| Explicit exclusions | 不 package acquisition、不 restore、不解決 official version conflict、不修改 cache、不執行 native code |
| Execution environment | PowerShell standard user；僅在未來授權後執行 |
| Required privilege | `Standard user` |
| Network access | `No` |
| Mutation expected | `No` |
| File creation expected | `No` |
| Registry write expected | `No` |
| Package／Cache mutation expected | `No` |
| Sensitive-data exposure risk | user cache path、unrelated package data |
| Sensitive-data redaction requirement | 遮罩 user cache path 與 unrelated package data |
| Expected output fields | Core/view/native IDs、versions、TFM、RID、native asset folder、dependency relation |
| Future Evidence ID | `RND-LOCAL-EVID-014` |
| Future Evidence destination | Future local inspection evidence root；本輪不建立 |
| Success condition | 不 package acquisition 即可取得 existing package metadata，且不以假設解決 version conflict |
| Stop conditions | package root 不明需 broad scan、命令要求 network/write/restore、或 native code 要被載入時停止 |
| Failure／tool-missing handling | 記錄 package/native asset unavailable；不自行安裝或選版 |
| Conflict handling | 保留 Repository release context 與 cached/NuGet version evidence 的 conflict，不判定 support |
| Cleanup requirement | 不清理 cache、不建立檔案、不修改 package metadata |
| Risk classification | `R0 — Read-only local inspection` |
| Requested authorization | `Required before execution` |
| Proposed constraints | 僅 named package roots、`.nuspec` 與 native asset metadata；禁止 package acquisition |
| Authorization Decision | `Pending` |
| Decision authority | `TBD` |
| Decision date | `TBD` |
| Execution permitted | `No` |
| Owner | `TBD` |
| Open questions | SkiaSharp official version conflict 仍需後續 acquisition/build evidence，不由本 request 解決 |

### 6.15 `RND-INSPECT-AUTH-015`

| Field | Request value |
|---|---|
| Authorization Request ID | `RND-INSPECT-AUTH-015` |
| Source Inspection Item | `RND-INSPECT-015` — NuGet sources, config provenance and global-packages path |
| Inspection subject | Existing NuGet sources、config precedence 與 global-packages path |
| Inspection question | Existing NuGet sources、config precedence 與 global-packages path 是否可被唯讀記錄而不暴露 credentials？ |
| Source Gap IDs | `RND-OFF-GAP-006`, `RND-OFF-GAP-007`, `RND-OFF-GAP-010`, `RND-OFF-GAP-011` |
| Related Enablement Items | `RND-ENABLE-005`, `RND-ENABLE-006` |
| Related Closure Gates | `RND-CGATE-003`, `RND-CGATE-004`, `RND-CGATE-008` |
| Related Candidate–Host Pairs | `RND-PAIR-005`, `RND-PAIR-007`, `RND-PAIR-008` |
| Planned method reference | `RESEARCH-TECH-RENDER-008` / `RND-INSPECT-015`；列 source names/path，credentials suppressed |
| Planned command／API reference | `dotnet nuget list source`; `dotnet nuget locals global-packages --list`; targeted config metadata read |
| Safety classification | NuGet configuration read；R0 read-only |
| Exact authorized scope requested | 讀取 source name、enabled state、config provenance、global-packages path |
| Explicit exclusions | 不打印 password/API key/token/private feed credentials、不修改 source、不 restore、不下載、不輸出完整 config |
| Execution environment | PowerShell standard user；僅在未來授權後執行 |
| Required privilege | `Standard user` |
| Network access | `No` |
| Mutation expected | `No` |
| File creation expected | `No` |
| Registry write expected | `No` |
| Package／Cache mutation expected | `No` |
| Sensitive-data exposure risk | password、API key、access token、private feed URL、credential provider output |
| Sensitive-data redaction requirement | 永不輸出 credentials；只記錄 hostname/public URL、enabled state 與 credential presence |
| Expected output fields | Source name、enabled state、config provenance、global-packages path（清理後） |
| Future Evidence ID | `RND-LOCAL-EVID-015` |
| Future Evidence destination | Future local inspection evidence root；本輪不建立 |
| Success condition | 在 secrets omitted 的前提下捕捉 config provenance |
| Stop conditions | 命令要 network、輸出 secrets、修改 source/config/cache 或需 redaction 無法保證時停止 |
| Failure／tool-missing handling | 記錄 config unavailable；不以另一個未核准 tool 讀取 credential store |
| Conflict handling | config precedence、source list、cache path 不一致時保留各來源並標記 conflict |
| Cleanup requirement | 不建立檔案；不修改 NuGet config/source/cache |
| Risk classification | `R0 — Read-only local inspection` |
| Requested authorization | `Required before execution` |
| Proposed constraints | credentials suppressed；禁止 `dotnet restore`、source writes、cache operations |
| Authorization Decision | `Pending` |
| Decision authority | `TBD` |
| Decision date | `TBD` |
| Execution permitted | `No` |
| Owner | `TBD` |
| Open questions | 任何 private feed 的 existence 只能記錄 credential presence，不得驗證可用性 |

### 6.16 `RND-INSPECT-AUTH-016`

| Field | Request value |
|---|---|
| Authorization Request ID | `RND-INSPECT-AUTH-016` |
| Source Inspection Item | `RND-INSPECT-016` — Cached dependency metadata and transitive dependency evidence |
| Inspection subject | Existing `.nuspec`、assets metadata 與 transitive dependency records |
| Inspection question | Existing `.nuspec`、assets metadata 與 transitive dependency records 是否能補足 package identity evidence？ |
| Source Gap IDs | `RND-OFF-GAP-006`, `RND-OFF-GAP-007`, `RND-OFF-GAP-010`, `RND-OFF-GAP-011`, `RND-OFF-GAP-015` |
| Related Enablement Items | `RND-ENABLE-005` |
| Related Closure Gates | `RND-CGATE-004`, `RND-CGATE-005` |
| Related Candidate–Host Pairs | `RND-PAIR-005..008` |
| Planned method reference | `RESEARCH-TECH-RENDER-008` / `RND-INSPECT-016`；讀既有 package metadata，僅在明確 scope 內讀既有 project assets |
| Planned command／API reference | Targeted `Get-Content` for `.nuspec`, `project.assets.json` 或 equivalent existing metadata；no Restore |
| Safety classification | Package metadata read；R0 read-only |
| Exact authorized scope requested | 讀取 direct dependency、transitive dependency、TFM、RID、native asset relationship |
| Explicit exclusions | 不 Restore、不生成 assets、不讀取無關 project、不輸出 embedded credentials、不修改 metadata |
| Execution environment | PowerShell standard user；僅在未來授權後執行 |
| Required privilege | `Standard user` |
| Network access | `No` |
| Mutation expected | `No` |
| File creation expected | `No` |
| Registry write expected | `No` |
| Package／Cache mutation expected | `No` |
| Sensitive-data exposure risk | source URLs with credentials、unrelated project paths |
| Sensitive-data redaction requirement | 遮罩 embedded credentials、source secrets 與 unrelated project paths |
| Expected output fields | Direct dependency、transitive dependency、TFM、RID、native asset relationship |
| Future Evidence ID | `RND-LOCAL-EVID-016` |
| Future Evidence destination | Future local inspection evidence root；本輪不建立 |
| Success condition | 不生成 assets 即可讀取既有 dependency metadata |
| Stop conditions | 需要 Restore、network、file output、unbounded project scan 或 secrets 無法清理時停止 |
| Failure／tool-missing handling | 記錄 metadata unavailable；不自行產生或修復 assets |
| Conflict handling | nuspec、assets 與 official dependency evidence 不一致時保留 conflict，不關閉 gap |
| Cleanup requirement | 不建立／更新 assets；不修改 package metadata |
| Risk classification | `R0 — Read-only local inspection` |
| Requested authorization | `Required before execution` |
| Proposed constraints | 只讀已存在 metadata；禁止 Restore 與 output redirection |
| Authorization Decision | `Pending` |
| Decision authority | `TBD` |
| Decision date | `TBD` |
| Execution permitted | `No` |
| Owner | `TBD` |
| Open questions | transitive dependency completeness 需以既有 metadata 可見範圍為限，不推論未出現內容 |

### 6.17 `RND-INSPECT-AUTH-017`

| Field | Request value |
|---|---|
| Authorization Request ID | `RND-INSPECT-AUTH-017` |
| Source Inspection Item | `RND-INSPECT-017` — Repository isolation and evidence-root existence |
| Inspection subject | Planned isolation path 與 future evidence root 是否存在 |
| Inspection question | Planned isolation path 與 future evidence root 是否存在；若不存在是否能維持不建立原則？ |
| Source Gap IDs | `RND-OFF-GAP-014`, `RND-OFF-GAP-016` |
| Related Enablement Items | `RND-ENABLE-004`, `RND-ENABLE-006` |
| Related Closure Gates | `RND-CGATE-007`, `RND-CGATE-008` |
| Related Candidate–Host Pairs | `RND-PAIR-001..010` |
| Planned method reference | `RESEARCH-TECH-RENDER-008` / `RND-INSPECT-017`；只檢查 exact path existence |
| Planned command／API reference | `Test-Path` for `experiments/rendering/<host>/<candidate>/` and `docs/Research/Technology/results/rendering/` |
| Safety classification | File-system metadata read；R0 read-only |
| Exact authorized scope requested | 讀取 exact path、exists flag、file/directory type |
| Explicit exclusions | 不建立 path、不 broad enumerate、不列出 unrelated files、不寫 output、不接觸 external workspace |
| Execution environment | PowerShell standard user；僅在未來授權後執行 |
| Required privilege | `Standard user` |
| Network access | `No` |
| Mutation expected | `No` |
| File creation expected | `No` |
| Registry write expected | `No` |
| Package／Cache mutation expected | `No` |
| Sensitive-data exposure risk | external workspace paths、unrelated file names |
| Sensitive-data redaction requirement | 不列出 unrelated files，不揭露 workspace 外路徑 |
| Expected output fields | Exact path（清理後）、exists flag、file/directory type |
| Future Evidence ID | `RND-LOCAL-EVID-017` |
| Future Evidence destination | No destination may be created by this item |
| Success condition | path existence 可知且不建立 directory、不 broad enumerate |
| Stop conditions | path scope 不明、需要建立 directory、需讀取 workspace 外內容或需 write 時停止 |
| Failure／tool-missing handling | 記錄 path check unavailable；不建立替代 evidence root |
| Conflict handling | Parent plans 與實際 exists flag 不一致時記錄 conflict，保留 parent statement |
| Cleanup requirement | 不建立任何 path 或檔案；不清理既有資料 |
| Risk classification | `R0 — Read-only local inspection` |
| Requested authorization | `Required before execution` |
| Proposed constraints | 僅 exact `Test-Path`；不得使用 `New-Item`、`Set-Content` 或 broad enumeration |
| Authorization Decision | `Pending` |
| Decision authority | `TBD` |
| Decision date | `TBD` |
| Execution permitted | `No` |
| Owner | `TBD` |
| Open questions | Evidence write authorization 必須獨立決策，不因本 request 而自動產生 |

### 6.18 `RND-INSPECT-AUTH-018`

| Field | Request value |
|---|---|
| Authorization Request ID | `RND-INSPECT-AUTH-018` |
| Source Inspection Item | `RND-INSPECT-018` — GPU／Display／DPI／HDR evidence inheritance and remaining gaps |
| Inspection subject | Existing GPU、Display topology、DPI、HDR evidence 與 remaining Runtime-only gaps |
| Inspection question | Existing UI evidence 是否已足夠描述 GPU、Display topology、DPI 與 HDR；哪些仍必須保留為 Runtime evidence？ |
| Source Gap IDs | `RND-OFF-GAP-012`, `RND-OFF-GAP-013`, `RND-OFF-GAP-014` |
| Related Enablement Items | `RND-ENABLE-002`, `RND-ENABLE-003`, `RND-ENABLE-004` |
| Related Closure Gates | `RND-CGATE-002`, `RND-CGATE-006`, `RND-CGATE-007` |
| Related Candidate–Host Pairs | `RND-PAIR-001..010` |
| Planned method reference | `RESEARCH-TECH-RENDER-008` / `RND-INSPECT-018`；先比較 `RESEARCH-TECH-UI-006..009`，只規劃缺少欄位 |
| Planned command／API reference | Future targeted `Get-CimInstance Win32_VideoController`; existing UI evidence lookup; no display mutation |
| Safety classification | Environment inheritance；R0 read-only |
| Exact authorized scope requested | 只讀 GPU vendor/model、driver version、display count、DPI evidence provenance、HDR evidence state；優先 reuse existing UI evidence |
| Explicit exclusions | Screenshot、screen recording、rendering output、Display/DPI/HDR/Power Plan mutation、monitor serial、device ID、Runtime Spike |
| Execution environment | PowerShell standard user；僅在未來授權後執行；既有 UI evidence lookup 優先 |
| Required privilege | `Standard user` |
| Network access | `No` |
| Mutation expected | `No` |
| File creation expected | `No` |
| Registry write expected | `No` |
| Package／Cache mutation expected | `No` |
| Sensitive-data exposure risk | monitor serial、device ID、user profile、machine identifiers |
| Sensitive-data redaction requirement | 遮罩 monitor serial、device ID、user profile 與 machine identifiers |
| Expected output fields | GPU vendor/model、driver version、display count、DPI provenance、HDR state、remaining Runtime-only gaps |
| Future Evidence ID | `RND-LOCAL-EVID-018` |
| Future Evidence destination | Future local inspection evidence root；本輪不建立 |
| Success condition | 明確做出 reuse decision，並保留剩餘 Runtime-only gaps |
| Stop conditions | 需要 display mutation、screenshot、screen recording、network、admin、file output 或 Runtime Spike 時停止 |
| Failure／tool-missing handling | 只記錄 evidence unavailable；不以缺少 GPU query 推導不支援 |
| Conflict handling | Existing UI evidence、future hardware query、DPI/HDR provenance 不一致時標記 conflict，保留 Runtime-only status |
| Cleanup requirement | 不建立圖片、錄影、Evidence 檔或 rendering output；不改變顯示設定 |
| Risk classification | `R0 — Read-only local inspection` |
| Requested authorization | `Required before execution` |
| Proposed constraints | UI evidence inheritance 優先；任何新 query 仍限於 008 所列唯讀範圍 |
| Authorization Decision | `Pending` |
| Decision authority | `TBD` |
| Decision date | `TBD` |
| Execution permitted | `No` |
| Owner | `TBD` |
| Open questions | GPU/DPI/HDR 的完整性仍須由後續 Runtime evidence 決定；本 request 不批准 Runtime |

## 7. Command Boundary Register

下表只引用 `RESEARCH-TECH-RENDER-008` 已列出的 planned command／API，不新增查核命令。

| Request | Planned command／API source | Read-only classification | Network capable | Mutation capable | Output redirection permitted | Decision |
|---|---|---|---|---|---|---|
| `RND-INSPECT-AUTH-001` | `Get-CimInstance Win32_OperatingSystem`; `Get-CimInstance Win32_ComputerSystem`; `[Environment]::Is64BitOperatingSystem` | Environment inheritance | `No` | `No` | `No` | `Pending` |
| `RND-INSPECT-AUTH-002` | `Get-Command dotnet`; `dotnet --list-sdks` | Process inventory read | `No` | `No` | `No` | `Pending` |
| `RND-INSPECT-AUTH-003` | `dotnet --list-runtimes`; targeted `Get-ChildItem` | Process inventory read | `No` | `No` | `No` | `Pending` |
| `RND-INSPECT-AUTH-004` | `dotnet workload list` | Process inventory read | `No` | `No` | `No` | `Pending` |
| `RND-INSPECT-AUTH-005` | `Get-Command vswhere.exe`; `vswhere.exe -products * -format json` | Process inventory read | `No` | `No` | `No` | `Pending` |
| `RND-INSPECT-AUTH-006` | `Get-Command msbuild.exe`; `Get-Item`; `FileVersionInfo` | Process and file metadata read | `No` | `No` | `No` | `Pending` |
| `RND-INSPECT-AUTH-007` | Targeted `Get-ItemProperty`; targeted `Test-Path` | Registry read | `No` | `No` | `No` | `Pending` |
| `RND-INSPECT-AUTH-008` | Targeted `Test-Path`; targeted `Get-Item` | File-system metadata read | `No` | `No` | `No` | `Pending` |
| `RND-INSPECT-AUTH-009` | `Get-AppxPackage -Name Microsoft.WindowsAppRuntime*` | AppX inventory read | `No` | `No` | `No` | `Pending` |
| `RND-INSPECT-AUTH-010` | `dotnet nuget locals global-packages --list`; targeted `Test-Path` and `Get-Item` | File-system metadata read | `No` | `No` | `No` | `Pending` |
| `RND-INSPECT-AUTH-011` | `dotnet new list`; targeted `Get-Item`／`Get-Content` | File-system metadata read | `No` | `No` | `No` | `Pending` |
| `RND-INSPECT-AUTH-012` | Targeted `Test-Path`; targeted `Get-Item` | File-system metadata read | `No` | `No` | `No` | `Pending` |
| `RND-INSPECT-AUTH-013` | Targeted `Get-Item`; `Get-Content` existing `.nuspec` | Package metadata read | `No` | `No` | `No` | `Pending` |
| `RND-INSPECT-AUTH-014` | Targeted `Get-Item`; `Get-Content` existing `.nuspec` | Package metadata read | `No` | `No` | `No` | `Pending` |
| `RND-INSPECT-AUTH-015` | `dotnet nuget list source`; `dotnet nuget locals global-packages --list`; targeted config metadata read | NuGet configuration read | `No` | `No` | `No` | `Pending` |
| `RND-INSPECT-AUTH-016` | Targeted `Get-Content` for existing metadata | Package metadata read | `No` | `No` | `No` | `Pending` |
| `RND-INSPECT-AUTH-017` | Exact `Test-Path` for planned paths | File-system metadata read | `No` | `No` | `No` | `Pending` |
| `RND-INSPECT-AUTH-018` | Future targeted `Get-CimInstance Win32_VideoController`; existing UI evidence lookup | Environment inheritance | `No` | `No` | `No` | `Pending` |

明確禁止下列命令或同等行為：

```text
dotnet restore
dotnet build
dotnet run
dotnet new
dotnet workload install
dotnet workload update
nuget install
winget install
choco install
msbuild
devenv /Build
Add-AppxPackage
Register-AppxPackage
New-Item
Set-Content
Out-File
Export-Csv
reg add
reg delete
setx
```

不得因工具可附帶唯讀參數，就申請整個工具的不受限制使用權。所有 output redirection 均為 `No`。

## 8. Allowed Observation Boundary

未來經批准後，只允許觀察：

- 已安裝版本。
- 已存在路徑。
- 已存在 Package ID／version directory。
- 已存在 `.nuspec` 與 dependency metadata。
- 已存在 Native／runtime asset directory。
- 已存在 Target Framework directory。
- 已存在 Registry value。
- 已存在 Build Tool／SDK provenance。
- 已存在 Display／GPU／DPI evidence。
- Tool 缺少或資料不存在的明確狀態。

不得：

- 建立缺少的路徑。
- Restore 缺少的 Package。
- 修復缺少的 workload。
- 自動安裝 `vswhere`。
- 啟動 Visual Studio Installer。
- 修改 NuGet source。
- 清理 Package Cache。
- 下載 metadata。
- 將 `Not observed` 改寫成 `Unsupported`。

## 9. Sensitive Data Boundary

不得收集或輸出：

- NuGet authenticated source credentials。
- API Key、Token 或 Password。
- Private key、certificate private material。
- 使用者完整環境變數。
- 不相關的使用者檔案路徑。
- Repository 外的私人文件名稱。
- Machine identity 中與研究無關的個人資訊。
- 完整 Registry export。
- 完整 NuGet config 原文，如包含 credential。

允許保存：

- Package source hostname 或公開 URL。
- Credential presence：`Present`／`Not present`／`Not inspected`。
- 已清理的工具與 Package 路徑。
- 版本、架構及公開 metadata。
- 已移除敏感內容的 standard output。

若任何 command 不能保證上述清理，該 command 的 execution permission 維持 `No`，並記錄為 `RND-INSPECT-AUTH-GAP-xxx`，不得現場放寬範圍。

## 10. Shared UI Authority Boundary

| Shared capability | UI authority source | Current UI decision | Rendering request effect | Execution effect |
|---|---|---|---|---|
| Windows baseline | `RESEARCH-TECH-UI-006` / `UI-AUTH-001` | `Pending` | 只可引用或補足唯讀 baseline | 不批准 UI mutation、Build 或 Runtime |
| .NET SDK／Runtime inventory | `RESEARCH-TECH-UI-006` / `UI-AUTH-002` | `Pending` | 只可申請既有 SDK／Runtime inventory | 不批准 Restore、Install、Build 或 Run |
| Visual Studio／Build Tools | `RESEARCH-TECH-UI-007` / `UI-AUTH-003` | `Pending` | 只可申請既有 tool provenance | 不批准 Installer、MSBuild Build 或 tool mutation |
| Windows SDK | `RESEARCH-TECH-UI-007` / `UI-AUTH-004` | `Pending` | 只可申請 known Registry/path metadata | 不批准 SDK install、Registry write 或 compile |
| WinUI 3／Windows App SDK | `RESEARCH-TECH-UI-008` / `UI-AUTH-005` | `Pending` | 只可申請 existing Runtime/template/cache metadata | 不批准 project creation、Restore 或 Runtime |
| WPF build path | `RESEARCH-TECH-UI-008` / `UI-AUTH-006` | `Pending` | 只可申請 existing targeting assets | 不批准 WPF project、Build 或 Run |
| Display／DPI | `RESEARCH-TECH-UI-009` / `UI-AUTH-007` | `Pending` | 只可 reuse existing evidence 或規劃唯讀缺欄位查詢 | 不批准 screenshot、Display/DPI mutation |
| GPU／driver | `RESEARCH-TECH-UI-009` / `UI-AUTH-008` | `Pending` | 只可申請清理後 GPU evidence | 不批准 native execution、Runtime Spike 或 rendering output |

本文件不得批准 Shared UI mutation。Rendering inspection request 只能申請唯讀觀察。即使本文件未來被批准，也不代表 UI Enablement、Build 或 Runtime 已獲准。

## 11. Evidence Creation Boundary

本文件只申請「觀察」的授權，不申請建立 Evidence 檔案。未來若要保存結果，必須另行決定：

- 是否允許建立 Result directory。
- 是否允許建立 Markdown／JSON／TXT Evidence。
- 是否允許 command output redirection。
- 敏感資料如何清理。
- Evidence retention 與 cleanup。

因此本文件所有 request 均固定為：

- `File creation expected: No`
- `Output redirection permitted: No`
- `Execution permitted: No`

未來執行階段的查核結果只能回傳至當時授權的互動 Session，不得落地為檔案，除非另有獨立 Evidence Write Authorization。

## 12. Inspection Batch Design

本節只規劃 batch，不執行。

### Batch I1 — Shared Host Inventory

| Field | Value |
|---|---|
| Included Request IDs | `RND-INSPECT-AUTH-001..012` |
| Entry criteria | 人工核准對應 request；仍為 Standard user、No network、No mutation、No file output |
| Approved scope required | Windows、.NET、workload、VS／Build Tools、MSBuild、Windows SDK、Direct2D／DirectWrite、AppX、WinUI、WPF 的既有 metadata |
| Stop conditions | 任一 008 stop condition；任何 command 需要 install、restore、build、run、network 或 write |
| Expected observations | Existing／Not observed／Unavailable／Conflicting 的最小欄位 |
| Sensitive-data boundary | 遮罩 user、machine、credential、unrelated path；不輸出完整環境或 config |
| Exit criteria | 001..012 各自有清理後 Session observation，或明確記錄 unavailable |
| Execution permission | `No` |
| Dependency on previous Batch | None；但不代表其他 batch 自動獲准 |

### Batch I2 — Candidate Package Inventory

| Field | Value |
|---|---|
| Included Request IDs | `RND-INSPECT-AUTH-013..016` |
| Entry criteria | Batch I1 的相關路徑若已被授權且仍為 Standard user、No network、No mutation、No file output |
| Approved scope required | Win2D、SkiaSharp、NuGet source/config/cache 與既有 dependency metadata |
| Stop conditions | package acquisition、Restore、cache mutation、credential exposure、output redirection 或 scope drift |
| Expected observations | Existing package identity、version、TFM、RID、native asset、dependency relation，或 unavailable/conflicting |
| Sensitive-data boundary | 不輸出 credentials、private feed secret、unrelated packages 或完整 user cache path |
| Exit criteria | 013..016 各自有清理後 Session observation，或明確記錄 unavailable/conflicting |
| Execution permission | `No` |
| Dependency on previous Batch | 不因 Batch I1 核准而自動獲准；需獨立 decision |

### Batch I3 — Repository and Environment Evidence Inheritance

| Field | Value |
|---|---|
| Included Request IDs | `RND-INSPECT-AUTH-017..018` |
| Entry criteria | 人工核准 exact path check 或 existing UI evidence comparison；仍為 No network、No mutation、No file output |
| Approved scope required | Exact repository isolation/evidence-root existence 與 UI evidence inheritance、GPU／Display／DPI／HDR gap comparison |
| Stop conditions | 建立 directory、broad enumeration、Screenshot、screen recording、Display mutation、Runtime Spike 或 scope drift |
| Expected observations | Path exists flag、UI evidence reuse decision、remaining Runtime-only gaps |
| Sensitive-data boundary | 不揭露 workspace 外路徑、monitor serial、device ID、user profile 或 machine identity |
| Exit criteria | 017..018 各自有清理後 Session observation，或明確記錄 unavailable/conflicting |
| Execution permission | `No` |
| Dependency on previous Batch | 不因 Batch I1／I2 核准而自動獲准；需獨立 decision |

前一個 Batch 未來獲准，不代表後一個 Batch 自動獲准。每個 Batch 都必須重新確認 request scope、stop conditions、sensitive-data boundary 與 execution permission。

## 13. Stop Conditions

未來執行時發生下列任一情況，必須立即停止：

- 命令需要管理員權限。
- 命令要求網路連線。
- 命令嘗試下載、Restore 或更新。
- 命令建立或修改檔案。
- 命令修改 Registry、PATH、NuGet config 或 Cache。
- 查核需要啟動 Visual Studio Installer。
- 查核需要建立 Project。
- 查核需要 Build 或 Runtime execution。
- 輸出包含未清理的 credential、token、private key 或 password。
- 實際命令與 `RESEARCH-TECH-RENDER-008` 記錄不同。
- 工具版本或參數使唯讀性無法確認。
- 操作超出已核准 Request ID。
- 無法保證 mutation risk 為零。
- 需要 Screenshot、Screen recording 或 Rendering output。
- 需要變更 Display、DPI、HDR 或 Power Plan。

停止後只能回報清理後的失敗／停止狀態；不得為了完成 request 而改用未核准工具或擴張範圍。

## 14. Human Decision Record

下表正好 18 列；初始值全部為 Pending，不能視為已授權：

| Request | Inspection Item | Risk | Requested authorization | Decision | Constraints | Authority | Date | Execution permitted |
|---|---|---|---|---|---|---|---|---|
| `RND-INSPECT-AUTH-001` | Windows／architecture baseline | `R0` | Required before execution | `Pending` | See request record | `TBD` | `TBD` | `No` |
| `RND-INSPECT-AUTH-002` | .NET SDK inventory | `R0` | Required before execution | `Pending` | See request record | `TBD` | `TBD` | `No` |
| `RND-INSPECT-AUTH-003` | .NET Runtime／Windows Desktop Runtime | `R0` | Required before execution | `Pending` | See request record | `TBD` | `TBD` | `No` |
| `RND-INSPECT-AUTH-004` | Installed workload inventory | `R0` | Required before execution | `Pending` | See request record | `TBD` | `TBD` | `No` |
| `RND-INSPECT-AUTH-005` | Visual Studio／Build Tools／vswhere | `R0` | Required before execution | `Pending` | See request record | `TBD` | `TBD` | `No` |
| `RND-INSPECT-AUTH-006` | MSBuild path and provenance | `R0` | Required before execution | `Pending` | See request record | `TBD` | `TBD` | `No` |
| `RND-INSPECT-AUTH-007` | Windows SDK version roots | `R0` | Required before execution | `Pending` | See request record | `TBD` | `TBD` | `No` |
| `RND-INSPECT-AUTH-008` | Direct2D／DirectWrite assets | `R0` | Required before execution | `Pending` | See request record | `TBD` | `TBD` | `No` |
| `RND-INSPECT-AUTH-009` | Windows App SDK Runtime packages | `R0` | Required before execution | `Pending` | See request record | `TBD` | `TBD` | `No` |
| `RND-INSPECT-AUTH-010` | Windows App SDK SDK／NuGet cache | `R0` | Required before execution | `Pending` | See request record | `TBD` | `TBD` | `No` |
| `RND-INSPECT-AUTH-011` | WinUI 3 templates／targets | `R0` | Required before execution | `Pending` | See request record | `TBD` | `TBD` | `No` |
| `RND-INSPECT-AUTH-012` | WPF targeting pack | `R0` | Required before execution | `Pending` | See request record | `TBD` | `TBD` | `No` |
| `RND-INSPECT-AUTH-013` | Win2D cached package | `R0` | Required before execution | `Pending` | See request record | `TBD` | `TBD` | `No` |
| `RND-INSPECT-AUTH-014` | SkiaSharp packages／native assets | `R0` | Required before execution | `Pending` | See request record | `TBD` | `TBD` | `No` |
| `RND-INSPECT-AUTH-015` | NuGet source/config/cache provenance | `R0` | Required before execution | `Pending` | See request record | `TBD` | `TBD` | `No` |
| `RND-INSPECT-AUTH-016` | Cached dependency metadata | `R0` | Required before execution | `Pending` | See request record | `TBD` | `TBD` | `No` |
| `RND-INSPECT-AUTH-017` | Repository isolation/evidence root | `R0` | Required before execution | `Pending` | See request record | `TBD` | `TBD` | `No` |
| `RND-INSPECT-AUTH-018` | GPU／Display／DPI／HDR evidence | `R0` | Required before execution | `Pending` | See request record | `TBD` | `TBD` | `No` |

## 15. Overall Authorization Request Status

### 15.1 Derivation

`18 request records complete + all commands bound to parent plan + standard-user-only + no network + no mutation + no file creation + sensitive-data controls complete + stop conditions complete` → Overall Authorization Request Status.

### 15.2 Current status

`Conditionally ready for human authorization review`

此狀態只表示 request records 與安全邊界已整理到可提交人工審查，不表示任何 command 可以執行。

固定狀態：

- `Authorization Decision: Pending`
- `Current Authorization: Not granted`
- `Inspection Execution Authorized: No`
- `Closure Execution Authorized: No`
- `Build Verification: Not performed`
- `Runtime Verification: Not performed`
- `Runtime Spike Execution Authorized: No`
- `Rendering Decision: Not made`

## 16. Approval Effect

即使未來本文件被批准，其效果最多只是：

> 允許依指定 Request ID，在 Standard-user、No-network、No-mutation、No-file-output 條件下執行本機唯讀觀察。

不代表：

- 可以建立 Evidence 檔案。
- 可以建立 Result directory。
- 可以 Restore 或下載 Package。
- 可以安裝 SDK、Runtime、Tool 或 workload。
- 可以建立 Project。
- 可以 Build 或 Run。
- 可以執行 Runtime Spike。
- `RND-OFF-GAP` 已關閉。
- `RND-ENABLE` 已完成。
- Candidate–Host Pair 已通過。
- Rendering Technology 已選定。
- `TD-002` ADR 可以建立。

## 17. Traceability

```text
RND-OFF-GAP
  -> RND-INSPECT
  -> RND-INSPECT-AUTH
  -> Human decision
  -> Future read-only observation
  -> Future evidence-write decision
  -> Enablement reassessment
  -> Future closure authorization request
```

至少引用下列來源：

- `RESEARCH-TECH-RENDER-005` — Rendering Technology Execution Enablement Specification
- `RESEARCH-TECH-RENDER-006` — Rendering Technology Official Candidate Evidence Baseline
- `RESEARCH-TECH-RENDER-007` — Rendering Technology Execution Enablement Reassessment
- `RESEARCH-TECH-RENDER-008` — Rendering Technology Read-only Local Prerequisite Inspection Plan
- `RESEARCH-TECH-UI-006`
- `RESEARCH-TECH-UI-007`
- `RESEARCH-TECH-UI-008`
- `RESEARCH-TECH-UI-009`
- `ADR-0002`
- `TECHNOLOGY-DECISION-ROADMAP`

## 18. Completion Boundary

本文件完成條件：

- 只建立 `18-rendering-technology-read-only-local-inspection-authorization-request.md`。
- 不修改任何其他文件。
- 建立正好 18 個 `RND-INSPECT-AUTH`。
- 18 個 request 與 18 個 Inspection Item 一對一。
- 不新增或修改查核命令。
- 所有 request 均為 R0、Standard user、No network、No mutation。
- 所有 Decision 均為 `Pending`。
- 所有 Authority／Date 均為 `TBD`。
- 所有 `Execution permitted = No`。
- 所有 `File creation expected = No`。
- 所有 `Output redirection permitted = No`。
- 不執行任何查核命令。
- 不建立 Result directory、Evidence、Project、Prototype 或 Source Code。
- 不執行下載、安裝、Restore、Build、Run 或 Runtime Spike。
- 不建立 Closure Authorization 或 `TD-002` ADR。
- 不修改 `ADR-0002`。
- 只做允許的靜態檢查與 `git diff --check`。

本文件完成後，必須等候人工作出 authorization decision；不得因 `Conditionally ready for human authorization review` 自動執行任何 inspection。
