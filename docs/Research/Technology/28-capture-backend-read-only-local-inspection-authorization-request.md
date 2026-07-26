# Capture Backend Read-only Local Inspection Authorization Request

| Field | Value |
|---|---|
| Document ID | RESEARCH-TECH-CAPTURE-009 |
| Title | Capture Backend Read-only Local Inspection Authorization Request |
| Status | Draft |
| Research Type | Read-only Local Inspection Authorization Request |
| Parent Inspection Plan | RESEARCH-TECH-CAPTURE-008 |
| Parent Reassessment | RESEARCH-TECH-CAPTURE-007 |
| Official Evidence Baseline | RESEARCH-TECH-CAPTURE-006 |
| Authorization Decision | Pending |
| Current Authorization | Not granted |
| Inspection Execution Authorized | No |
| Evidence File Creation Authorized | No |
| Closure Execution Authorized | No |
| Build Verification | Not performed |
| Runtime Verification | Not performed |
| Capture Runtime Spike Authorized | No |
| Evidence Write Authorized | No |
| UI Framework Decision | Unresolved — ADR-0002 remains Draft |
| Rendering Decision | Not made |
| Capture Decision | Not made |
| Requested by | TBD |
| Decision authority | TBD |
| Decision date | TBD |
| Owner | TBD |

## 1. Purpose

本文件只回答：是否應將 RESEARCH-TECH-CAPTURE-008 已定義的 20 個 `CAP-INSPECT` 項目，提交給真人進行唯讀本機檢查授權審查。

本文件是 Authorization Request，不是：

- Human Authorization Decision。
- Inspection Record。
- Inspection Execution。
- Closure、Build 或 Runtime 授權。
- Capture Backend Decision。
- Rendering Decision。
- ADR-0002 的決策修訂。
- Capture ADR。
- 截圖、錄影或 Frame 產生工作。

本文件不得自行批准、執行或寫入任何檢查結果。20 個 Request 必須直接引用 008 的既有 Inspection Item、planned method 與 planned command/API；若 008 不足，只能新增本文件的缺口標記，不得改寫上游計畫。

## 2. Scope

申請範圍僅限 Standard-user、No-network、No-mutation、No-file-output 的本機唯讀觀察：

- 已安裝 Windows、SDK、Runtime、Visual Studio、Build Tools、`vswhere` 與 MSBuild 的版本及路徑 metadata。
- 已存在的 Windows SDK、Windows App SDK、WinUI 3、WPF targeting pack、WinRT metadata、Header、Assembly、native asset 與 Package metadata。
- 指定 Registry value 的唯讀狀態，以及已存在的 AppX package identity。
- NuGet config 的受限來源 metadata、global-packages path、已存在 package identity/version/dependency/native asset。
- GPU、driver、D3D capability、display topology、monitor bounds、negative-coordinate、DPI、scaling、HDR 與 color-state 的非敏感 technical metadata。
- Repository isolation boundary、既有 planned experiment/result root 的存在狀態，以及 Shared UI Research 的可繼承 evidence reference。

所有 20 個 Request 都只申請「觀察並回傳至目前互動 Session」，不申請建立檔案、Result directory、Persistent log 或 Evidence。

## 3. Non-goals and Authority Boundary

本次申請不包含：

- 網路查詢、下載、安裝、Restore、Package acquisition 或 Cache mutation。
- Visual Studio Installer、NuGet source/config、Registry、PATH、Display、DPI、HDR、color profile 或 Power Plan mutation。
- 建立 Project、Solution、Prototype、Source Code、Result directory、Evidence file 或 command output file。
- Build、Run、Publish、Test、Capture API invocation、Capture Session、Screen Recording、Screenshot、Frame 或 Pixel Difference。
- 管理員權限、credential value、token、password、private key、完整環境變數、完整 config、完整 Registry export、Window title、Desktop image 或私人內容。
- 修改 `RESEARCH-TECH-CAPTURE-003..008`、`RESEARCH-TECH-UI-*`、`RESEARCH-TECH-RENDER-*` 或 `ADR-0002-ui-framework-selection.md`。
- 關閉 `CAP-OFF-GAP`、`CAP-PREQ`、`CAP-BLOCK`、`CAP-ENABLE`、`CAP-CGATE`，或選定 Capture Backend。

任何 Request 的授權都不會自動延伸到其他 Request、Shared UI mutation、Project、Restore、Build、Runtime、Capture API 或 Evidence persistence。

## 4. Controlled Vocabulary and Binding Rules

### 4.1 Authorization vocabulary

`Authorization Decision` 只能使用：`Pending`、`Approved`、`Approved with constraints`、`Rejected`、`Deferred`。本文件建立時 20 筆均為 `Pending`，不可代填真人決定。

`Execution permitted` 只能使用 `No` 或經真人決定後的明確值；本文件建立時 20 筆均為 `No`。

`Request Packaging Status` 只能使用：

- `Ready for human authorization review`。
- `Conditionally ready for human authorization review`。
- `Not ready for human authorization review`。

`Risk Classification` 固定為 `R0 — Read-only local inspection`。

### 4.2 One-to-one binding

下列 20 個 Request 必須與 008 的 20 個 Inspection Item 一對一，不得合併、拆分、增加第 21 筆或修改上游問題：

| Authorization Request | Parent Inspection Item | Subject |
|---|---|---|
| CAP-INSPECT-AUTH-001 | CAP-INSPECT-001 | Windows edition/build/architecture baseline |
| CAP-INSPECT-AUTH-002 | CAP-INSPECT-002 | .NET SDK inventory |
| CAP-INSPECT-AUTH-003 | CAP-INSPECT-003 | .NET Runtime/Windows Desktop Runtime inventory |
| CAP-INSPECT-AUTH-004 | CAP-INSPECT-004 | Visual Studio/Build Tools/vswhere availability |
| CAP-INSPECT-AUTH-005 | CAP-INSPECT-005 | MSBuild path and provenance |
| CAP-INSPECT-AUTH-006 | CAP-INSPECT-006 | Windows SDK version roots |
| CAP-INSPECT-AUTH-007 | CAP-INSPECT-007 | Windows App SDK Runtime inventory |
| CAP-INSPECT-AUTH-008 | CAP-INSPECT-008 | Windows App SDK SDK/Package Cache assets |
| CAP-INSPECT-AUTH-009 | CAP-INSPECT-009 | WinUI 3 templates/targets/experimental host assets |
| CAP-INSPECT-AUTH-010 | CAP-INSPECT-010 | WPF targeting pack/reference assemblies |
| CAP-INSPECT-AUTH-011 | CAP-INSPECT-011 | Windows Graphics Capture WinRT metadata/headers/interop |
| CAP-INSPECT-AUTH-012 | CAP-INSPECT-012 | Direct3D 11/DXGI/Desktop Duplication assets |
| CAP-INSPECT-AUTH-013 | CAP-INSPECT-013 | GDI/User32/Window-oriented API assets |
| CAP-INSPECT-AUTH-014 | CAP-INSPECT-014 | NuGet sources/config/global-packages path |
| CAP-INSPECT-AUTH-015 | CAP-INSPECT-015 | Capture-related cached Package identity/versions/native assets |
| CAP-INSPECT-AUTH-016 | CAP-INSPECT-016 | Candidate dependency/transitive metadata |
| CAP-INSPECT-AUTH-017 | CAP-INSPECT-017 | Repository isolation and planned experiment/result roots |
| CAP-INSPECT-AUTH-018 | CAP-INSPECT-018 | GPU/driver/D3D11 capability boundary |
| CAP-INSPECT-AUTH-019 | CAP-INSPECT-019 | Display topology/monitor bounds/negative coordinates/DPI |
| CAP-INSPECT-AUTH-020 | CAP-INSPECT-020 | HDR/color state and Shared UI evidence inheritance |

每個 Request 的 parent method 與 command/API 必須直接引用 008；本文件不自行設計替代命令，不申請整個 PowerShell、`dotnet`、MSBuild、NuGet 或 Registry 工具的不受限制使用權。

## 5. Authorization Request Records

以下每筆都是獨立的 R0 Request。`Official evidence` 只引用 006 的既有 mapping，不在本文件新增官方 claim；`Future local evidence` 是未來規劃 ID，不是本輪建立的檔案。

### 5.1 CAP-INSPECT-AUTH-001

| Field | Value |
|---|---|
| Authorization Request ID | CAP-INSPECT-AUTH-001 |
| Source Inspection Item | CAP-INSPECT-001 |
| Subject | Windows edition/build/architecture baseline |
| Question | OS edition、build、CPU architecture 是否可由既有本機 metadata 唯讀觀察？ |
| Related Official Gaps | CAP-OFF-GAP-001 |
| Prerequisites | CAP-PREQ-001 |
| Blockers | CAP-BLOCK-001 |
| Candidate–Host Pairs | CAP-PAIR-001..010 |
| Enablements | CAP-ENABLE-001 |
| Gates | CAP-CGATE-001 |
| Shared UI IDs | RESEARCH-TECH-UI-006..009，where applicable；Pending |
| Official evidence | RESEARCH-TECH-CAPTURE-006 existing mapping；no new research |
| Parent planned method | Process inventory read，依 008 §7.1；not executed |
| Parent planned command/API | Process inventory read，依 008 §7.1；not executed |
| Safety class | R0 — Read-only local inspection |
| Exact requested scope | 只讀本機 Windows edition/build/architecture metadata |
| Explicit exclusions | Network、admin、mutation、file output、Build、Run、Capture API |
| Execution environment | Local workstation；Standard user |
| Required privilege | Standard user |
| Network | No |
| Mutation | No |
| File creation | No |
| Output redirection | No |
| Registry write | No |
| Package/cache mutation | No |
| Display/system mutation | No |
| Capture API invocation | No |
| Sensitive-data risk/redaction | 不輸出完整環境變數、SID、私人路徑；只回傳必要版本 metadata |
| Expected observation fields | edition、build、architecture、environment alias |
| Future local evidence ID/destination | CAP-LOCAL-EVID-001；future result root only，not created |
| Success condition | 在核准範圍內取得必要 OS metadata並回傳 Session |
| Not-observed interpretation | Not observed 不等於 Unsupported |
| Conflict handling | 保留 conflicting observation，停止推論並回報 |
| Tool-missing handling | 回報 unavailable；不得安裝或替換工具 |
| Stop conditions | 需要權限、網路、檔案、mutation、Build/Run 或超出 008 |
| Cleanup requirement | 不得建立暫存物；若工具產生暫存則停止並回報 |
| Phase C1 impact | Host baseline；不關閉 Build/Runtime gate |
| Requested authorization | Required before execution |
| Proposed constraints | Standard user、No network、No mutation、No file output |
| Authorization Decision | Pending |
| Decision authority/date | TBD / TBD |
| Execution permitted | No |
| Owner | TBD |
| Open questions | Human authority、environment alias、allowed metadata fields TBD |

### 5.2 CAP-INSPECT-AUTH-002

| Field | Value |
|---|---|
| Authorization Request ID | CAP-INSPECT-AUTH-002 |
| Source Inspection Item | CAP-INSPECT-002 |
| Subject | .NET SDK inventory |
| Question | 已存在的 .NET SDK identity、version、architecture、root 是否可唯讀觀察？ |
| Related Official Gaps | CAP-OFF-GAP-001、002、014 |
| Prerequisites | CAP-PREQ-001、002、027 |
| Blockers | CAP-BLOCK-001、010 |
| Candidate–Host Pairs | CAP-PAIR-001..010 |
| Enablements | CAP-ENABLE-001、003 |
| Gates | CAP-CGATE-001、008 |
| Shared UI IDs | RESEARCH-TECH-UI-006..009，where applicable；Pending |
| Official evidence | RESEARCH-TECH-CAPTURE-006 existing mapping；no new research |
| Parent planned method | Process inventory read，依 008 §7.1；not executed |
| Parent planned command/API | Process inventory read，依 008 §7.1；not executed |
| Safety class | R0 — Read-only local inspection |
| Exact requested scope | 只讀已存在 SDK version、architecture、installation root |
| Explicit exclusions | Restore、download、install、source update、Build、Run、完整 path dump |
| Execution environment | Local workstation；Standard user |
| Required privilege | Standard user |
| Network | No |
| Mutation | No |
| File creation | No |
| Output redirection | No |
| Registry write | No |
| Package/cache mutation | No |
| Display/system mutation | No |
| Capture API invocation | No |
| Sensitive-data risk/redaction | 清理 user name、credential、token 與不相關私人路徑 |
| Expected observation fields | SDK id、version、architecture、sanitized root、source |
| Future local evidence ID/destination | CAP-LOCAL-EVID-002；future result root only，not created |
| Success condition | 取得已存在 SDK metadata且不改變 machine state |
| Not-observed interpretation | Not observed 不等於 SDK 不支援 |
| Conflict handling | 分開記錄不同 SDK instance，不自行選定 authority |
| Tool-missing handling | 回報 unavailable；不得 restore/install |
| Stop conditions | 需要下載、Restore、寫入、Build、admin 或超出 008 |
| Cleanup requirement | 不建立暫存或 cache；若產生則停止並回報 |
| Phase C1 impact | Host/build prerequisite contribution；仍需 Project/Build |
| Requested authorization | Required before execution |
| Proposed constraints | Standard user、No network、No mutation、No file output |
| Authorization Decision | Pending |
| Decision authority/date | TBD / TBD |
| Execution permitted | No |
| Owner | TBD |
| Open questions | SDK instances 的 sanitized path 與 authority TBD |

### 5.3 CAP-INSPECT-AUTH-003

| Field | Value |
|---|---|
| Authorization Request ID | CAP-INSPECT-AUTH-003 |
| Source Inspection Item | CAP-INSPECT-003 |
| Subject | .NET Runtime/Windows Desktop Runtime inventory |
| Question | Runtime identity、version、architecture、source 是否可唯讀觀察，且不被誤寫成 Build/Run 證據？ |
| Related Official Gaps | CAP-OFF-GAP-001、002、014 |
| Prerequisites | CAP-PREQ-001、002、027 |
| Blockers | CAP-BLOCK-001、010 |
| Candidate–Host Pairs | CAP-PAIR-001..010 |
| Enablements | CAP-ENABLE-001、003 |
| Gates | CAP-CGATE-001、008 |
| Shared UI IDs | RESEARCH-TECH-UI-006..009，where applicable；Pending |
| Official evidence | RESEARCH-TECH-CAPTURE-006 existing mapping；no new research |
| Parent planned method | Process inventory read，依 008 §7.1；not executed |
| Parent planned command/API | Process inventory read，依 008 §7.1；not executed |
| Safety class | R0 — Read-only local inspection |
| Exact requested scope | 只讀 .NET/Windows Desktop Runtime version、architecture、source |
| Explicit exclusions | 不執行 application、Build、Restore、Runtime spike 或 package install |
| Execution environment | Local workstation；Standard user |
| Required privilege | Standard user |
| Network | No |
| Mutation | No |
| File creation | No |
| Output redirection | No |
| Registry write | No |
| Package/cache mutation | No |
| Display/system mutation | No |
| Capture API invocation | No |
| Sensitive-data risk/redaction | 僅回傳必要 version/architecture；移除私人路徑與 credential |
| Expected observation fields | runtime id、version、architecture、source、environment alias |
| Future local evidence ID/destination | CAP-LOCAL-EVID-003；future result root only，not created |
| Success condition | 取得既有 Runtime metadata並維持只讀 |
| Not-observed interpretation | Not observed 不等於 Runtime 不存在或不支援 |
| Conflict handling | 分別記錄 runtime family；不宣稱 host compatibility |
| Tool-missing handling | 回報 unavailable；不得安裝或 Restore |
| Stop conditions | 需要啟動 app、Build、Run、network、mutation 或 admin |
| Cleanup requirement | 不建立 runtime/cache artifacts |
| Phase C1 impact | Shared host baseline；不關閉 runtime gate |
| Requested authorization | Required before execution |
| Proposed constraints | Standard user、No network、No mutation、No file output |
| Authorization Decision | Pending |
| Decision authority/date | TBD / TBD |
| Execution permitted | No |
| Owner | TBD |
| Open questions | Runtime identity 與 host authority mapping TBD |

### 5.4 CAP-INSPECT-AUTH-004

| Field | Value |
|---|---|
| Authorization Request ID | CAP-INSPECT-AUTH-004 |
| Source Inspection Item | CAP-INSPECT-004 |
| Subject | Visual Studio/Build Tools/vswhere availability |
| Question | 已存在的 IDE、Build Tools、`vswhere` instance state 是否可由檔案 metadata 唯讀觀察？ |
| Related Official Gaps | CAP-OFF-GAP-001、002、014 |
| Prerequisites | CAP-PREQ-001、002、027 |
| Blockers | CAP-BLOCK-001、010 |
| Candidate–Host Pairs | CAP-PAIR-001..010 |
| Enablements | CAP-ENABLE-001、003 |
| Gates | CAP-CGATE-001、008 |
| Shared UI IDs | RESEARCH-TECH-UI-006..009，where applicable；Pending |
| Official evidence | RESEARCH-TECH-CAPTURE-006 existing mapping；no new research |
| Parent planned method | File-system metadata read，依 008 §7.1；not executed |
| Parent planned command/API | File-system metadata read，依 008 §7.1；not executed |
| Safety class | R0 — Read-only local inspection |
| Exact requested scope | 只讀既有 IDE/Build Tools/`vswhere` version、path、instance state |
| Explicit exclusions | Visual Studio Installer modification、workload install、Build、Restore、network |
| Execution environment | Local workstation；Standard user |
| Required privilege | Standard user |
| Network | No |
| Mutation | No |
| File creation | No |
| Output redirection | No |
| Registry write | No |
| Package/cache mutation | No |
| Display/system mutation | No |
| Capture API invocation | No |
| Sensitive-data risk/redaction | 清理 user name、完整私人 path、license/token 等非必要資料 |
| Expected observation fields | tool id、version、architecture、path provenance、instance state |
| Future local evidence ID/destination | CAP-LOCAL-EVID-004；future result root only，not created |
| Success condition | 僅取得既有 tool metadata且無 Installer/config mutation |
| Not-observed interpretation | Not observed 不等於 IDE/Build Tools 不可用 |
| Conflict handling | 分別保留 instance；不將 availability 改寫成 Build pass |
| Tool-missing handling | 回報 unavailable；不得下載或安裝 |
| Stop conditions | 需要 Installer、admin、network、寫入或 Build |
| Cleanup requirement | 不建立 discovery output file |
| Phase C1 impact | Build prerequisite contribution；仍需另行授權 Build |
| Requested authorization | Required before execution |
| Proposed constraints | Standard user、No network、No mutation、No file output |
| Authorization Decision | Pending |
| Decision authority/date | TBD / TBD |
| Execution permitted | No |
| Owner | TBD |
| Open questions | Instance authority、sanitized path format TBD |

### 5.5 CAP-INSPECT-AUTH-005

| Field | Value |
|---|---|
| Authorization Request ID | CAP-INSPECT-AUTH-005 |
| Source Inspection Item | CAP-INSPECT-005 |
| Subject | MSBuild path and provenance |
| Question | MSBuild path、file version、architecture、provenance 是否可唯讀確認而不執行 Build？ |
| Related Official Gaps | CAP-OFF-GAP-001、002、014 |
| Prerequisites | CAP-PREQ-002、027 |
| Blockers | CAP-BLOCK-001、010 |
| Candidate–Host Pairs | CAP-PAIR-001..010 |
| Enablements | CAP-ENABLE-001、003 |
| Gates | CAP-CGATE-001、008 |
| Shared UI IDs | RESEARCH-TECH-UI-006..009，where applicable；Pending |
| Official evidence | RESEARCH-TECH-CAPTURE-006 existing mapping；no new research |
| Parent planned method | File-system metadata read，依 008 §7.1；not executed |
| Parent planned command/API | File-system metadata read，依 008 §7.1；not executed |
| Safety class | R0 — Read-only local inspection |
| Exact requested scope | 只讀 MSBuild path、file version、architecture、provenance |
| Explicit exclusions | `msbuild` execution、`devenv /Build`、Project creation、Restore、network |
| Execution environment | Local workstation；Standard user |
| Required privilege | Standard user |
| Network | No |
| Mutation | No |
| File creation | No |
| Output redirection | No |
| Registry write | No |
| Package/cache mutation | No |
| Display/system mutation | No |
| Capture API invocation | No |
| Sensitive-data risk/redaction | 清理私人 path 與 user identity；只回傳 sanitized provenance |
| Expected observation fields | path alias、file version、architecture、provenance |
| Future local evidence ID/destination | CAP-LOCAL-EVID-005；future result root only，not created |
| Success condition | 取得 MSBuild identity，不啟動 MSBuild |
| Not-observed interpretation | Not observed 不等於缺少 Build capability |
| Conflict handling | 分開記錄候選 path；不自行選 authority |
| Tool-missing handling | 回報 unavailable；不得安裝或替換 |
| Stop conditions | 任何 Build、Restore、Installer、network、mutation 或 admin |
| Cleanup requirement | 不建立 output 或 log |
| Phase C1 impact | Build prerequisite contribution；Build 仍未授權 |
| Requested authorization | Required before execution |
| Proposed constraints | Standard user、No network、No mutation、No file output |
| Authorization Decision | Pending |
| Decision authority/date | TBD / TBD |
| Execution permitted | No |
| Owner | TBD |
| Open questions | MSBuild provenance authority TBD |

### 5.6 CAP-INSPECT-AUTH-006

| Field | Value |
|---|---|
| Authorization Request ID | CAP-INSPECT-AUTH-006 |
| Source Inspection Item | CAP-INSPECT-006 |
| Subject | Windows SDK version roots |
| Question | Windows SDK roots、include/lib/winmd/header asset 是否已存在且可唯讀確認？ |
| Related Official Gaps | CAP-OFF-GAP-001、002、003 |
| Prerequisites | CAP-PREQ-003、004、005 |
| Blockers | CAP-BLOCK-002 |
| Candidate–Host Pairs | CAP-PAIR-001..010 |
| Enablements | CAP-ENABLE-002 |
| Gates | CAP-CGATE-002、003 |
| Shared UI IDs | RESEARCH-TECH-UI-006..009，where applicable；Pending |
| Official evidence | RESEARCH-TECH-CAPTURE-006 existing mapping；no new research |
| Parent planned method | File-system metadata read，依 008 §7.1；not executed |
| Parent planned command/API | File-system metadata read，依 008 §7.1；not executed |
| Safety class | R0 — Read-only local inspection |
| Exact requested scope | 只讀 SDK version roots、headers、libs、WinRT metadata presence |
| Explicit exclusions | SDK install/update、Project、Restore、Build、network、file output |
| Execution environment | Local workstation；Standard user |
| Required privilege | Standard user |
| Network | No |
| Mutation | No |
| File creation | No |
| Output redirection | No |
| Registry write | No |
| Package/cache mutation | No |
| Display/system mutation | No |
| Capture API invocation | No |
| Sensitive-data risk/redaction | 只回傳 SDK version與必要 sanitized path；移除私人路徑 |
| Expected observation fields | SDK version、include/lib/winmd presence、architecture、source |
| Future local evidence ID/destination | CAP-LOCAL-EVID-006；future result root only，not created |
| Success condition | 確認既有 assets 的 presence metadata，不形成 Build claim |
| Not-observed interpretation | Not observed 不等於 API/asset 不支援 |
| Conflict handling | 多版本並存時保留 conflict，不自動選 version |
| Tool-missing handling | 回報 unavailable；不得下載 SDK |
| Stop conditions | 需要安裝、Restore、Build、admin、network 或 mutation |
| Cleanup requirement | 不建立 inventory file |
| Phase C1 impact | Candidate identity contribution；Project/Build/Runtime 仍分離 |
| Requested authorization | Required before execution |
| Proposed constraints | Standard user、No network、No mutation、No file output |
| Authorization Decision | Pending |
| Decision authority/date | TBD / TBD |
| Execution permitted | No |
| Owner | TBD |
| Open questions | SDK version authority、asset classification TBD |

### 5.7 CAP-INSPECT-AUTH-007

| Field | Value |
|---|---|
| Authorization Request ID | CAP-INSPECT-AUTH-007 |
| Source Inspection Item | CAP-INSPECT-007 |
| Subject | Windows App SDK Runtime inventory |
| Question | 已存在 Windows App SDK Runtime package identity/version/architecture 是否可唯讀觀察？ |
| Related Official Gaps | CAP-OFF-GAP-001、002、003 |
| Prerequisites | CAP-PREQ-001、003、004 |
| Blockers | CAP-BLOCK-001、002 |
| Candidate–Host Pairs | CAP-PAIR-001、002、009、010 |
| Enablements | CAP-ENABLE-001、002 |
| Gates | CAP-CGATE-001、002、003 |
| Shared UI IDs | RESEARCH-TECH-UI-006..009，where applicable；Pending |
| Official evidence | RESEARCH-TECH-CAPTURE-006 existing mapping；no new research |
| Parent planned method | AppX inventory read，依 008 §7.1；not executed |
| Parent planned command/API | AppX inventory read，依 008 §7.1；not executed |
| Safety class | R0 — Read-only local inspection |
| Exact requested scope | 只讀既有 package identity、version、architecture、runtime state |
| Explicit exclusions | `Add-AppxPackage`、`Register-AppxPackage`、install、network、Project、Build、Capture API |
| Execution environment | Local workstation；Standard user |
| Required privilege | Standard user |
| Network | No |
| Mutation | No |
| File creation | No |
| Output redirection | No |
| Registry write | No |
| Package/cache mutation | No |
| Display/system mutation | No |
| Capture API invocation | No |
| Sensitive-data risk/redaction | 不輸出 user package content、完整 private path 或 account identity |
| Expected observation fields | package identity、version、architecture、state、source |
| Future local evidence ID/destination | CAP-LOCAL-EVID-007；future result root only，not created |
| Success condition | 只讀取得既有 AppX identity，不註冊或安裝 |
| Not-observed interpretation | Not observed 不等於 Windows App SDK runtime 不可用 |
| Conflict handling | 保留不同 package family/version，禁止自行宣告 host compatibility |
| Tool-missing handling | 回報 unavailable；不得安裝/註冊 package |
| Stop conditions | 需要 admin、AppX mutation、network、file output、Build 或 Capture API |
| Cleanup requirement | 不建立 AppX/cache artifacts |
| Phase C1 impact | WinUI host boundary contribution；不批准 UI framework |
| Requested authorization | Required before execution |
| Proposed constraints | Standard user、No network、No mutation、No file output |
| Authorization Decision | Pending |
| Decision authority/date | TBD / TBD |
| Execution permitted | No |
| Owner | TBD |
| Open questions | Runtime package authority與 unpackaged boundary TBD |

### 5.8 CAP-INSPECT-AUTH-008

| Field | Value |
|---|---|
| Authorization Request ID | CAP-INSPECT-AUTH-008 |
| Source Inspection Item | CAP-INSPECT-008 |
| Subject | Windows App SDK SDK/Package Cache assets |
| Question | 已存在的 Windows App SDK SDK/package/WinRT/interop assets 是否可在不 Restore 下唯讀觀察？ |
| Related Official Gaps | CAP-OFF-GAP-002、003、014 |
| Prerequisites | CAP-PREQ-003、004、010、027 |
| Blockers | CAP-BLOCK-002、010 |
| Candidate–Host Pairs | CAP-PAIR-001、002、009、010 |
| Enablements | CAP-ENABLE-002、003 |
| Gates | CAP-CGATE-002、003、008 |
| Shared UI IDs | RESEARCH-TECH-UI-006..009，where applicable；Pending |
| Official evidence | RESEARCH-TECH-CAPTURE-006 existing mapping；no new research |
| Parent planned method | Package metadata read，依 008 §7.1；not executed |
| Parent planned command/API | Package metadata read，依 008 §7.1；not executed |
| Safety class | R0 — Read-only local inspection |
| Exact requested scope | 只讀已存在 Package ID、version、TFM、asset names與 metadata |
| Explicit exclusions | Restore、download、cache clean/mutation、credential value、Build、Project |
| Execution environment | Local workstation；Standard user |
| Required privilege | Standard user |
| Network | No |
| Mutation | No |
| File creation | No |
| Output redirection | No |
| Registry write | No |
| Package/cache mutation | No |
| Display/system mutation | No |
| Capture API invocation | No |
| Sensitive-data risk/redaction | 不讀 credential value；只回傳 package metadata與公開 source hostname |
| Expected observation fields | package ID、version、TFM、RID、asset names、source category |
| Future local evidence ID/destination | CAP-LOCAL-EVID-008；future result root only，not created |
| Success condition | 取得 cache 中已存在 asset metadata，且 cache 不變更 |
| Not-observed interpretation | Not observed 不等於 package 不存在於其他授權來源 |
| Conflict handling | 分別記錄 package/version/TFM conflict，不形成 candidate ranking |
| Tool-missing handling | 回報 unavailable；不得 Restore或下載 |
| Stop conditions | 需要 network、Restore、cache mutation、credential、file output或 admin |
| Cleanup requirement | 不清除或建立 cache內容 |
| Phase C1 impact | Package evidence contribution；仍需 Project/Build |
| Requested authorization | Required before execution |
| Proposed constraints | Standard user、No network、No mutation、No file output |
| Authorization Decision | Pending |
| Decision authority/date | TBD / TBD |
| Execution permitted | No |
| Owner | TBD |
| Open questions | global-packages path sanitized representation TBD |

### 5.9 CAP-INSPECT-AUTH-009

| Field | Value |
|---|---|
| Authorization Request ID | CAP-INSPECT-AUTH-009 |
| Source Inspection Item | CAP-INSPECT-009 |
| Subject | WinUI 3 templates/targets/experimental host assets |
| Question | 既有 WinUI 3 template、targets、props、host assets 是否可唯讀確認而不建立 Project？ |
| Related Official Gaps | CAP-OFF-GAP-001、002、003、014 |
| Prerequisites | CAP-PREQ-001、002、027 |
| Blockers | CAP-BLOCK-001、002、010 |
| Candidate–Host Pairs | CAP-PAIR-001、003、005、007、009 |
| Enablements | CAP-ENABLE-001、002、003 |
| Gates | CAP-CGATE-001、003、008 |
| Shared UI IDs | RESEARCH-TECH-UI-006..009，where applicable；Pending |
| Official evidence | RESEARCH-TECH-CAPTURE-006 existing mapping；no new research |
| Parent planned method | File-system metadata read，依 008 §7.1；not executed |
| Parent planned command/API | File-system metadata read，依 008 §7.1；not executed |
| Safety class | R0 — Read-only local inspection |
| Exact requested scope | 只讀 template、target、framework、asset path provenance |
| Explicit exclusions | `dotnet new`、Project/Solution/Prototype creation、Restore、Build、Run、install |
| Execution environment | Local workstation；Standard user |
| Required privilege | Standard user |
| Network | No |
| Mutation | No |
| File creation | No |
| Output redirection | No |
| Registry write | No |
| Package/cache mutation | No |
| Display/system mutation | No |
| Capture API invocation | No |
| Sensitive-data risk/redaction | 不輸出私人 repository 外 path、user identity 或 token |
| Expected observation fields | template/target ID、version、framework、path provenance、asset presence |
| Future local evidence ID/destination | CAP-LOCAL-EVID-009；future result root only，not created |
| Success condition | 只讀取得既有 host assets，不建立 experimental host |
| Not-observed interpretation | Not observed 不等於 WinUI 3 不可建立或不支援 |
| Conflict handling | 多版本或缺檔分開記錄；不宣告 pair 通過 |
| Tool-missing handling | 回報 unavailable；不得執行 `dotnet new` 或安裝 |
| Stop conditions | 需要建立檔案、Project、Restore、Build、admin、network |
| Cleanup requirement | 不產生 template/project artifacts |
| Phase C1 impact | WinUI host path contribution；UI decision仍 unresolved |
| Requested authorization | Required before execution |
| Proposed constraints | Standard user、No network、No mutation、No file output |
| Authorization Decision | Pending |
| Decision authority/date | TBD / TBD |
| Execution permitted | No |
| Owner | TBD |
| Open questions | WinUI host identity、unpackaged/packaged scope TBD |

### 5.10 CAP-INSPECT-AUTH-010

| Field | Value |
|---|---|
| Authorization Request ID | CAP-INSPECT-AUTH-010 |
| Source Inspection Item | CAP-INSPECT-010 |
| Subject | WPF targeting pack/reference assemblies |
| Question | WPF targeting pack、reference assemblies、TFM、version 是否可唯讀確認而不 Build？ |
| Related Official Gaps | CAP-OFF-GAP-001、002、003、014 |
| Prerequisites | CAP-PREQ-001、002、027 |
| Blockers | CAP-BLOCK-001、002、010 |
| Candidate–Host Pairs | CAP-PAIR-002、004、006、008、010 |
| Enablements | CAP-ENABLE-001、003 |
| Gates | CAP-CGATE-001、003、008 |
| Shared UI IDs | RESEARCH-TECH-UI-006..009，where applicable；Pending |
| Official evidence | RESEARCH-TECH-CAPTURE-006 existing mapping；no new research |
| Parent planned method | File-system metadata read，依 008 §7.1；not executed |
| Parent planned command/API | File-system metadata read，依 008 §7.1；not executed |
| Safety class | R0 — Read-only local inspection |
| Exact requested scope | 只讀 WPF reference assembly path、TFM、version、targeting pack presence |
| Explicit exclusions | Build、Project creation、Restore、install、network、runtime execution |
| Execution environment | Local workstation；Standard user |
| Required privilege | Standard user |
| Network | No |
| Mutation | No |
| File creation | No |
| Output redirection | No |
| Registry write | No |
| Package/cache mutation | No |
| Display/system mutation | No |
| Capture API invocation | No |
| Sensitive-data risk/redaction | 清理私人 path、user name、license與非必要 assembly內容 |
| Expected observation fields | targeting pack、reference path、TFM、version、architecture |
| Future local evidence ID/destination | CAP-LOCAL-EVID-010；future result root only，not created |
| Success condition | 取得既有 WPF host asset metadata，不宣稱 Build compatibility |
| Not-observed interpretation | Not observed 不等於 WPF host 不可用 |
| Conflict handling | 保留多個 TFM/version；不自動選 host |
| Tool-missing handling | 回報 unavailable；不得安裝 targeting pack |
| Stop conditions | 需要 Build、Restore、install、network、admin或檔案寫入 |
| Cleanup requirement | 不建立 project/build artifact |
| Phase C1 impact | WPF host path contribution；仍需 Project/Build/Runtime |
| Requested authorization | Required before execution |
| Proposed constraints | Standard user、No network、No mutation、No file output |
| Authorization Decision | Pending |
| Decision authority/date | TBD / TBD |
| Execution permitted | No |
| Owner | TBD |
| Open questions | WPF targeting pack authority與 framework scope TBD |

### 5.11 CAP-INSPECT-AUTH-011

| Field | Value |
|---|---|
| Authorization Request ID | CAP-INSPECT-AUTH-011 |
| Source Inspection Item | CAP-INSPECT-011 |
| Subject | Windows Graphics Capture WinRT metadata/headers/interop |
| Question | WGC namespace、WinRT metadata、headers、assembly、interop definition 是否已存在？ |
| Related Official Gaps | CAP-OFF-GAP-002、003、004 |
| Prerequisites | CAP-PREQ-003、004、005、006、007、008 |
| Blockers | CAP-BLOCK-002 |
| Candidate–Host Pairs | CAP-PAIR-001、002、009、010 |
| Enablements | CAP-ENABLE-002 |
| Gates | CAP-CGATE-002、003 |
| Shared UI IDs | RESEARCH-TECH-UI-006..009，where applicable；Pending |
| Official evidence | RESEARCH-TECH-CAPTURE-006 existing mapping；no new research |
| Parent planned method | File-system metadata read，依 008 §7.1；not executed |
| Parent planned command/API | File-system metadata read，依 008 §7.1；not executed |
| Safety class | R0 — Read-only local inspection |
| Exact requested scope | 只讀 namespace、header、WinMD、assembly、version、interop asset presence |
| Explicit exclusions | 不呼叫 WGC、建立 session/frame pool/device、Build、Run、Capture API |
| Execution environment | Local workstation；Standard user |
| Required privilege | Standard user |
| Network | No |
| Mutation | No |
| File creation | No |
| Output redirection | No |
| Registry write | No |
| Package/cache mutation | No |
| Display/system mutation | No |
| Capture API invocation | No |
| Sensitive-data risk/redaction | 不輸出 private package content、credential、desktop/window data |
| Expected observation fields | namespace、header、WinMD、assembly、version、source |
| Future local evidence ID/destination | CAP-LOCAL-EVID-011；future result root only，not created |
| Success condition | 確認 development identity，不宣稱 invocation/runtime 成功 |
| Not-observed interpretation | Not observed 不等於 WGC API 不支援 |
| Conflict handling | SDK/WinAppSDK/projection 分層記錄，不能混成單一 identity |
| Tool-missing handling | 回報 unavailable；不得 Restore/install |
| Stop conditions | 需要 Capture API、device、frame、Project、Build、network或admin |
| Cleanup requirement | 不建立 WinRT/interop output |
| Phase C1 impact | WGC identity contribution；runtime evidence仍未取得 |
| Requested authorization | Required before execution |
| Proposed constraints | Standard user、No network、No mutation、No file output |
| Authorization Decision | Pending |
| Decision authority/date | TBD / TBD |
| Execution permitted | No |
| Owner | TBD |
| Open questions | Projection/assembly authority mapping TBD |

### 5.12 CAP-INSPECT-AUTH-012

| Field | Value |
|---|---|
| Authorization Request ID | CAP-INSPECT-AUTH-012 |
| Source Inspection Item | CAP-INSPECT-012 |
| Subject | Direct3D 11/DXGI/Desktop Duplication assets |
| Question | D3D11、DXGI、COM headers/libs/native metadata 是否已存在，且不建立 device？ |
| Related Official Gaps | CAP-OFF-GAP-002、003、005 |
| Prerequisites | CAP-PREQ-003、004、005、006、011 |
| Blockers | CAP-BLOCK-002 |
| Candidate–Host Pairs | CAP-PAIR-003、004、009、010 |
| Enablements | CAP-ENABLE-002 |
| Gates | CAP-CGATE-002、003 |
| Shared UI IDs | RESEARCH-TECH-UI-006..009，where applicable；Pending |
| Official evidence | RESEARCH-TECH-CAPTURE-006 existing mapping；no new research |
| Parent planned method | File-system metadata read，依 008 §7.1；not executed |
| Parent planned command/API | File-system metadata read，依 008 §7.1；not executed |
| Safety class | R0 — Read-only local inspection |
| Exact requested scope | 只讀 header、lib、COM metadata、native asset、version presence |
| Explicit exclusions | D3D device、DXGI output duplication、frame acquisition、Build、Run、Capture API |
| Execution environment | Local workstation；Standard user |
| Required privilege | Standard user |
| Network | No |
| Mutation | No |
| File creation | No |
| Output redirection | No |
| Registry write | No |
| Package/cache mutation | No |
| Display/system mutation | No |
| Capture API invocation | No |
| Sensitive-data risk/redaction | 僅回傳 technical asset metadata，不讀桌面、monitor serial或私人內容 |
| Expected observation fields | header/lib、COM identity、version、architecture、source |
| Future local evidence ID/destination | CAP-LOCAL-EVID-012；future result root only，not created |
| Success condition | 取得既有 development asset presence，不宣稱 device/runtime pass |
| Not-observed interpretation | Not observed 不等於 D3D11/DXGI 不可用 |
| Conflict handling | 分開記錄 SDK/native asset與 GPU capability，不互相替代 |
| Tool-missing handling | 回報 unavailable；不得安裝 SDK或 driver |
| Stop conditions | 需要建立 device/session、Capture API、Build、network、admin或mutation |
| Cleanup requirement | 不建立 graphics device或 output |
| Phase C1 impact | Device prerequisite contribution；recovery/frame仍 deferred |
| Requested authorization | Required before execution |
| Proposed constraints | Standard user、No network、No mutation、No file output |
| Authorization Decision | Pending |
| Decision authority/date | TBD / TBD |
| Execution permitted | No |
| Owner | TBD |
| Open questions | Native asset provenance與 adapter correlation TBD |

### 5.13 CAP-INSPECT-AUTH-013

| Field | Value |
|---|---|
| Authorization Request ID | CAP-INSPECT-AUTH-013 |
| Source Inspection Item | CAP-INSPECT-013 |
| Subject | GDI/User32/Window-oriented API assets |
| Question | GDI、User32、PrintWindow、BitBlt、GetDIBits 的既有 development asset 是否可唯讀確認而不擷取？ |
| Related Official Gaps | CAP-OFF-GAP-002、003、006、007 |
| Prerequisites | CAP-PREQ-003、004、006、017、018、019 |
| Blockers | CAP-BLOCK-002、005、006 |
| Candidate–Host Pairs | CAP-PAIR-005..008 |
| Enablements | CAP-ENABLE-002、004 |
| Gates | CAP-CGATE-002、003、004 |
| Shared UI IDs | RESEARCH-TECH-UI-006..009，where applicable；Pending |
| Official evidence | RESEARCH-TECH-CAPTURE-006 existing mapping；no new research |
| Parent planned method | File-system metadata read，依 008 §7.1；not executed |
| Parent planned command/API | File-system metadata read，依 008 §7.1；not executed |
| Safety class | R0 — Read-only local inspection |
| Exact requested scope | 只讀 header、lib、interop identity、asset version |
| Explicit exclusions | `PrintWindow`、`BitBlt`、`GetDIBits` invocation、window/desktop capture、Build、Run |
| Execution environment | Local workstation；Standard user |
| Required privilege | Standard user |
| Network | No |
| Mutation | No |
| File creation | No |
| Output redirection | No |
| Registry write | No |
| Package/cache mutation | No |
| Display/system mutation | No |
| Capture API invocation | No |
| Sensitive-data risk/redaction | 不讀 Window title、Desktop image、private UI；只回傳 API asset metadata |
| Expected observation fields | header/lib、interop identity、version、source |
| Future local evidence ID/destination | CAP-LOCAL-EVID-013；future result root only，not created |
| Success condition | 確認 API development identity，不驗證 capture behavior |
| Not-observed interpretation | Not observed 不等於 GDI/window mechanism 不支援 |
| Conflict handling | GDI asset與 window behavior evidence 分開；保留 unknown |
| Tool-missing handling | 回報 unavailable；不得建立 capture prototype |
| Stop conditions | 需要 HWND/window/desktop data、Capture API、file、Build、admin或network |
| Cleanup requirement | 不建立 HDC、bitmap、DIB 或 output |
| Phase C1 impact | API boundary contribution；behavior deferred to later authorized phase |
| Requested authorization | Required before execution |
| Proposed constraints | Standard user、No network、No mutation、No file output |
| Authorization Decision | Pending |
| Decision authority/date | TBD / TBD |
| Execution permitted | No |
| Owner | TBD |
| Open questions | Asset-only evidence如何支援 legacy host TBD |

### 5.14 CAP-INSPECT-AUTH-014

| Field | Value |
|---|---|
| Authorization Request ID | CAP-INSPECT-AUTH-014 |
| Source Inspection Item | CAP-INSPECT-014 |
| Subject | NuGet sources/config/global-packages path |
| Question | NuGet provenance、global-packages path、公開 source hostname、credential presence category 是否可受限唯讀觀察？ |
| Related Official Gaps | CAP-OFF-GAP-001、002、014、015 |
| Prerequisites | CAP-PREQ-027、029、030 |
| Blockers | CAP-BLOCK-008、009、010 |
| Candidate–Host Pairs | CAP-PAIR-001..010 |
| Enablements | CAP-ENABLE-003、006 |
| Gates | CAP-CGATE-008、009 |
| Shared UI IDs | RESEARCH-TECH-UI-006..009，where applicable；Pending |
| Official evidence | RESEARCH-TECH-CAPTURE-006 existing mapping；no new research |
| Parent planned method | NuGet configuration read，依 008 §7.1；not executed |
| Parent planned command/API | NuGet configuration read，依 008 §7.1；not executed |
| Safety class | R0 — Read-only local inspection |
| Exact requested scope | 只讀公開 source hostname、path alias、global-packages path、credential presence category |
| Explicit exclusions | credential value、完整 config、source mutation、Restore、download、network query |
| Execution environment | Local workstation；Standard user |
| Required privilege | Standard user |
| Network | No |
| Mutation | No |
| File creation | No |
| Output redirection | No |
| Registry write | No |
| Package/cache mutation | No |
| Display/system mutation | No |
| Capture API invocation | No |
| Sensitive-data risk/redaction | 絕不輸出 credential、token、password、完整 config或私人 path |
| Expected observation fields | source hostname、path alias、credential category、cache root alias |
| Future local evidence ID/destination | CAP-LOCAL-EVID-014；future result root only，not created |
| Success condition | 取得受限 provenance metadata且不查網路、不讀秘密值 |
| Not-observed interpretation | Not observed 不等於 source/cache 不存在 |
| Conflict handling | 保留多個 config scope；不自動決定 authority |
| Tool-missing handling | 回報 unavailable；不得修改 NuGet config/source |
| Stop conditions | 需要 credential、network、Restore、config write、完整輸出或admin |
| Cleanup requirement | 不建立 NuGet output；不得 clean cache |
| Phase C1 impact | Package authority boundary；Evidence persistence仍未授權 |
| Requested authorization | Required before execution |
| Proposed constraints | Standard user、No network、No mutation、No file output、secret redaction |
| Authorization Decision | Pending |
| Decision authority/date | TBD / TBD |
| Execution permitted | No |
| Owner | TBD |
| Open questions | allowed source hostname redaction與 path alias TBD |

### 5.15 CAP-INSPECT-AUTH-015

| Field | Value |
|---|---|
| Authorization Request ID | CAP-INSPECT-AUTH-015 |
| Source Inspection Item | CAP-INSPECT-015 |
| Subject | Capture-related cached Package identity/versions/native assets |
| Question | 已存在的 capture-related package ID、version、TFM、RID、native assets 是否可唯讀確認？ |
| Related Official Gaps | CAP-OFF-GAP-002、014 |
| Prerequisites | CAP-PREQ-003、010、027 |
| Blockers | CAP-BLOCK-002、010 |
| Candidate–Host Pairs | CAP-PAIR-001..010 |
| Enablements | CAP-ENABLE-002、003 |
| Gates | CAP-CGATE-002、008 |
| Shared UI IDs | RESEARCH-TECH-UI-006..009，where applicable；Pending |
| Official evidence | RESEARCH-TECH-CAPTURE-006 existing mapping；no new research |
| Parent planned method | Package metadata read，依 008 §7.1；not executed |
| Parent planned command/API | Package metadata read，依 008 §7.1；not executed |
| Safety class | R0 — Read-only local inspection |
| Exact requested scope | 只讀 cache 中已存在 package identity/version/TFM/RID/native asset metadata |
| Explicit exclusions | Download、Restore、cache clean/mutation、credential、Package ranking、Build、Run |
| Execution environment | Local workstation；Standard user |
| Required privilege | Standard user |
| Network | No |
| Mutation | No |
| File creation | No |
| Output redirection | No |
| Registry write | No |
| Package/cache mutation | No |
| Display/system mutation | No |
| Capture API invocation | No |
| Sensitive-data risk/redaction | 只回傳公開 package metadata；移除 credential與不相關私人 path |
| Expected observation fields | package ID、version、TFM、RID、native asset names、source category |
| Future local evidence ID/destination | CAP-LOCAL-EVID-015；future result root only，not created |
| Success condition | 取得既有 package identity，不把 presence 當成 Build/Runtime pass |
| Not-observed interpretation | Not observed 不等於 candidate 不支援 |
| Conflict handling | 多版本/TFM/RID 分開記錄；不排名 |
| Tool-missing handling | 回報 unavailable；不得 Restore/download |
| Stop conditions | 需要 network、cache mutation、secret value、Build、admin或file output |
| Cleanup requirement | 不清理或改寫 cache |
| Phase C1 impact | Package evidence contribution；Project/Build仍為 separate gates |
| Requested authorization | Required before execution |
| Proposed constraints | Standard user、No network、No mutation、No file output |
| Authorization Decision | Pending |
| Decision authority/date | TBD / TBD |
| Execution permitted | No |
| Owner | TBD |
| Open questions | Capture-related package identity scope TBD |

### 5.16 CAP-INSPECT-AUTH-016

| Field | Value |
|---|---|
| Authorization Request ID | CAP-INSPECT-AUTH-016 |
| Source Inspection Item | CAP-INSPECT-016 |
| Subject | Candidate dependency/transitive metadata |
| Question | 五個 Candidate 的既有 dependency、TFM、RID、native asset metadata 是否可唯讀整理而不形成 ranking？ |
| Related Official Gaps | CAP-OFF-GAP-002、008、014 |
| Prerequisites | CAP-PREQ-003、004、010、012、027 |
| Blockers | CAP-BLOCK-002、010 |
| Candidate–Host Pairs | CAP-PAIR-001..010 |
| Enablements | CAP-ENABLE-002、003 |
| Gates | CAP-CGATE-002、003、008 |
| Shared UI IDs | RESEARCH-TECH-UI-006..009，where applicable；Pending |
| Official evidence | RESEARCH-TECH-CAPTURE-006 existing mapping；no new research |
| Parent planned method | Package metadata read，依 008 §7.1；not executed |
| Parent planned command/API | Package metadata read，依 008 §7.1；not executed |
| Safety class | R0 — Read-only local inspection |
| Exact requested scope | 只讀 dependency ID/version、target framework、RID、native asset、source metadata |
| Explicit exclusions | Restore、network query、download、candidate ranking、Build、Project、credential |
| Execution environment | Local workstation；Standard user |
| Required privilege | Standard user |
| Network | No |
| Mutation | No |
| File creation | No |
| Output redirection | No |
| Registry write | No |
| Package/cache mutation | No |
| Display/system mutation | No |
| Capture API invocation | No |
| Sensitive-data risk/redaction | 移除 credential、private path、未授權 package content |
| Expected observation fields | candidate alias、dependency、version、TFM/RID、native asset、source |
| Future local evidence ID/destination | CAP-LOCAL-EVID-016；future result root only，not created |
| Success condition | 取得既有 metadata，維持 unknown/Not observed，不形成 ranking |
| Not-observed interpretation | Not observed 不等於 candidate 不支援或被排除 |
| Conflict handling | dependency conflict 原樣記錄；交由後續 authorized review |
| Tool-missing handling | 回報 unavailable；不得 Restore/install |
| Stop conditions | 需要 network、Restore、ranking、Build、file、admin或mutation |
| Cleanup requirement | 不修改 cache或 package metadata |
| Phase C1 impact | Candidate identity contribution；不決定 backend |
| Requested authorization | Required before execution |
| Proposed constraints | Standard user、No network、No mutation、No file output |
| Authorization Decision | Pending |
| Decision authority/date | TBD / TBD |
| Execution permitted | No |
| Owner | TBD |
| Open questions | Candidate alias與 metadata normalization TBD |

### 5.17 CAP-INSPECT-AUTH-017

| Field | Value |
|---|---|
| Authorization Request ID | CAP-INSPECT-AUTH-017 |
| Source Inspection Item | CAP-INSPECT-017 |
| Subject | Repository isolation and planned experiment/result roots |
| Question | workspace boundary、tracked/untracked metadata、planned result root existence 是否可唯讀確認而不建立目錄？ |
| Related Official Gaps | CAP-OFF-GAP-014、015 |
| Prerequisites | CAP-PREQ-027、029、030 |
| Blockers | CAP-BLOCK-008、009、010 |
| Candidate–Host Pairs | CAP-PAIR-001..010 |
| Enablements | CAP-ENABLE-003、006 |
| Gates | CAP-CGATE-008、009 |
| Shared UI IDs | RESEARCH-TECH-UI-006..009，where applicable；Pending |
| Official evidence | RESEARCH-TECH-CAPTURE-006 existing mapping；no new research |
| Parent planned method | File-system metadata read，依 008 §7.1；not executed |
| Parent planned command/API | File-system metadata read，依 008 §7.1；not executed |
| Safety class | R0 — Read-only local inspection |
| Exact requested scope | 只讀 workspace alias、root existence、tracked/untracked metadata、planned roots |
| Explicit exclusions | `New-Item`、file creation、Result/Evidence creation、repo outside boundary、Build、network |
| Execution environment | Current Repository workspace；Standard user |
| Required privilege | Standard user |
| Network | No |
| Mutation | No |
| File creation | No |
| Output redirection | No |
| Registry write | No |
| Package/cache mutation | No |
| Display/system mutation | No |
| Capture API invocation | No |
| Sensitive-data risk/redaction | 不輸出 workspace 外私人路徑、檔案內容或秘密資料 |
| Expected observation fields | workspace alias、root existence、tracked/untracked summary、boundary notes |
| Future local evidence ID/destination | CAP-LOCAL-EVID-017；future result root only，not created |
| Success condition | 確認 isolation boundary與既有 root 狀態，不修改 repository |
| Not-observed interpretation | Not observed 不等於 root 不可建立；只表示本輪未觀察 |
| Conflict handling | 保留 filesystem/git metadata 差異；停止擴大範圍 |
| Tool-missing handling | 回報 unavailable；不得建立替代 root |
| Stop conditions | 需要 workspace 外讀取、寫檔、建立目錄、admin、network或Build |
| Cleanup requirement | 不建立檔案、目錄、log或暫存物 |
| Phase C1 impact | Isolation/evidence authority contribution；Evidence write仍未授權 |
| Requested authorization | Required before execution |
| Proposed constraints | Standard user、No network、No mutation、No file output、workspace boundary only |
| Authorization Decision | Pending |
| Decision authority/date | TBD / TBD |
| Execution permitted | No |
| Owner | TBD |
| Open questions | Result root authority、tracked state snapshot format TBD |

### 5.18 CAP-INSPECT-AUTH-018

| Field | Value |
|---|---|
| Authorization Request ID | CAP-INSPECT-AUTH-018 |
| Source Inspection Item | CAP-INSPECT-018 |
| Subject | GPU/driver/D3D11 capability boundary |
| Question | GPU name、driver version、D3D feature metadata 是否可唯讀取得而不建立 device/session？ |
| Related Official Gaps | CAP-OFF-GAP-005、012、013 |
| Prerequisites | CAP-PREQ-005、006、011、021、022 |
| Blockers | CAP-BLOCK-007、011、012 |
| Candidate–Host Pairs | CAP-PAIR-003、004、009、010 |
| Enablements | CAP-ENABLE-002、005、007 |
| Gates | CAP-CGATE-002、006、010 |
| Shared UI IDs | RESEARCH-TECH-UI-006..009，where applicable；Pending |
| Official evidence | RESEARCH-TECH-CAPTURE-006 existing mapping；no new research |
| Parent planned method | Graphics capability read，依 008 §7.1；not executed |
| Parent planned command/API | Graphics capability read，依 008 §7.1；not executed |
| Safety class | R0 — Read-only local inspection |
| Exact requested scope | 只讀 adapter alias、GPU name、driver version、feature metadata |
| Explicit exclusions | 建立 D3D device、Desktop Duplication、Capture Session、device loss simulation、driver update |
| Execution environment | Local workstation；Standard user |
| Required privilege | Standard user |
| Network | No |
| Mutation | No |
| File creation | No |
| Output redirection | No |
| Registry write | No |
| Package/cache mutation | No |
| Display/system mutation | No |
| Capture API invocation | No |
| Sensitive-data risk/redaction | 不輸出 monitor serial、Machine SID、desktop image或私人資料 |
| Expected observation fields | adapter alias、GPU、driver、D3D feature metadata、source |
| Future local evidence ID/destination | CAP-LOCAL-EVID-018；future result root only，not created |
| Success condition | 取得既有 capability metadata，不宣稱 frame/recovery pass |
| Not-observed interpretation | Not observed 不等於 GPU/API capability 不存在 |
| Conflict handling | 多 adapter/driver 分開記錄；不得選定 runtime authority |
| Tool-missing handling | 回報 unavailable；不得安裝 driver或 workload |
| Stop conditions | 需要 device/session、admin、network、display mutation、file output或Capture API |
| Cleanup requirement | 不建立 graphics object或 result |
| Phase C1 impact | Device prerequisite contribution；recovery維持 C3 deferred |
| Requested authorization | Required before execution |
| Proposed constraints | Standard user、No network、No mutation、No file output |
| Authorization Decision | Pending |
| Decision authority/date | TBD / TBD |
| Execution permitted | No |
| Owner | TBD |
| Open questions | GPU capability fields與 privacy redaction TBD |

### 5.19 CAP-INSPECT-AUTH-019

| Field | Value |
|---|---|
| Authorization Request ID | CAP-INSPECT-AUTH-019 |
| Source Inspection Item | CAP-INSPECT-019 |
| Subject | Display topology/monitor bounds/negative coordinates/DPI |
| Question | monitor count、bounds、primary、negative-coordinate topology、per-monitor DPI/scaling 是否可唯讀觀察？ |
| Related Official Gaps | CAP-OFF-GAP-004、005、006、009、013 |
| Prerequisites | CAP-PREQ-014、015、016、022、023 |
| Blockers | CAP-BLOCK-004、007 |
| Candidate–Host Pairs | CAP-PAIR-001..010 |
| Enablements | CAP-ENABLE-005 |
| Gates | CAP-CGATE-005、006、007 |
| Shared UI IDs | RESEARCH-TECH-UI-006..009，where applicable；Pending |
| Official evidence | RESEARCH-TECH-CAPTURE-006 existing mapping；no new research |
| Parent planned method | Display configuration read，依 008 §7.1；not executed |
| Parent planned command/API | Display configuration read，依 008 §7.1；not executed |
| Safety class | R0 — Read-only local inspection |
| Exact requested scope | 只讀 monitor count/bounds/primary、negative coordinates、DPI/scaling |
| Explicit exclusions | 不改變 display arrangement、primary、DPI、HDR、color profile、Power Plan或 Capture API |
| Execution environment | Local workstation；Standard user |
| Required privilege | Standard user |
| Network | No |
| Mutation | No |
| File creation | No |
| Output redirection | No |
| Registry write | No |
| Package/cache mutation | No |
| Display/system mutation | No |
| Capture API invocation | No |
| Sensitive-data risk/redaction | 不輸出 monitor serial、window title、desktop image、私人 display content |
| Expected observation fields | monitor alias、bounds、primary、negative coordinate、DPI/scaling |
| Future local evidence ID/destination | CAP-LOCAL-EVID-019；future result root only，not created |
| Success condition | 取得 technical topology metadata且不改變顯示設定 |
| Not-observed interpretation | Not observed 不等於 topology/DPI 不支援 |
| Conflict handling | 記錄 timing/rounding/coordinate conflict；不代入 product contract |
| Tool-missing handling | 回報 unavailable；不得安裝或修改 display tooling |
| Stop conditions | 需要 display mutation、admin、desktop/window content、file、Capture API或network |
| Cleanup requirement | 不建立 display snapshot file |
| Phase C1 impact | Coordinate input contribution；runtime/crop fidelity仍未驗證 |
| Requested authorization | Required before execution |
| Proposed constraints | Standard user、No network、No mutation、No file output、technical metadata only |
| Authorization Decision | Pending |
| Decision authority/date | TBD / TBD |
| Execution permitted | No |
| Owner | TBD |
| Open questions | Coordinate/DPI observation redaction與 alias policy TBD |

### 5.20 CAP-INSPECT-AUTH-020

| Field | Value |
|---|---|
| Authorization Request ID | CAP-INSPECT-AUTH-020 |
| Source Inspection Item | CAP-INSPECT-020 |
| Subject | HDR/color state and Shared UI evidence inheritance |
| Question | HDR/color technical state及既有 Shared UI evidence 是否可在不改變系統、不讀私人畫面下唯讀參照？ |
| Related Official Gaps | CAP-OFF-GAP-010、011、012、015 |
| Prerequisites | CAP-PREQ-020、024、025、026、029、030 |
| Blockers | CAP-BLOCK-005、006、008、009、012 |
| Candidate–Host Pairs | CAP-PAIR-001..010 |
| Enablements | CAP-ENABLE-004、006、007 |
| Gates | CAP-CGATE-004、007、009、010 |
| Shared UI IDs | UI-AUTH-001..008；全部 Pending；RESEARCH-TECH-UI-006..009 inherited only |
| Official evidence | RESEARCH-TECH-CAPTURE-006 existing mapping；no new research |
| Parent planned method | Display configuration read；Environment inheritance，依 008 §7.1；not executed |
| Parent planned command/API | Display configuration read；Environment inheritance，依 008 §7.1；not executed |
| Safety class | R0 — Read-only local inspection |
| Exact requested scope | 只讀 HDR enabled state、color-space/profile technical metadata、Shared UI evidence reference |
| Explicit exclusions | Desktop image、Window title、overlay/cursor observation、DPI/HDR/color mutation、Evidence write、Capture API |
| Execution environment | Local workstation；Standard user；repository boundary only |
| Required privilege | Standard user |
| Network | No |
| Mutation | No |
| File creation | No |
| Output redirection | No |
| Registry write | No |
| Package/cache mutation | No |
| Display/system mutation | No |
| Capture API invocation | No |
| Sensitive-data risk/redaction | 不讀畫面、私人內容、monitor serial、credential；只回傳 technical metadata/reference |
| Expected observation fields | HDR state、color metadata、Shared UI source、authority state、redaction notes |
| Future local evidence ID/destination | CAP-LOCAL-EVID-020；future result root only，not created |
| Success condition | 取得可安全重用的 technical/UI authority reference，不改寫 UI decision |
| Not-observed interpretation | Not observed 不等於 HDR/color/Shared UI capability 不存在 |
| Conflict handling | UI authority、environment state、capture claim 分開記錄；維持 Pending |
| Tool-missing handling | 回報 unavailable；不得修改 display/UI authority或建立 evidence |
| Stop conditions | 需要畫面/Window data、mutation、file、admin、network、Build、Runtime或Capture API |
| Cleanup requirement | 不建立 screenshot、recording、evidence或 display snapshot |
| Phase C1 impact | Shared UI/evidence boundary contribution；不批准 Evidence write或 runtime |
| Requested authorization | Required before execution |
| Proposed constraints | Standard user、No network、No mutation、No file output、UI-AUTH remains Pending |
| Authorization Decision | Pending |
| Decision authority/date | TBD / TBD |
| Execution permitted | No |
| Owner | TBD |
| Open questions | UI authority、future evidence persistence與 HDR field scope TBD |

## 6. Command Boundary Register

本表正好 20 列。每一列只引用 RESEARCH-TECH-CAPTURE-008 的 planned method/command/API；本文件不新增替代命令。所有 `Decision` 初始為 `Pending`，所有 command 均為 `not executed`。

| Request | Parent method/API reference | Safety | Standard user | Network | Mutation | File output | Decision |
|---|---|---|---|---|---|---|---|
| CAP-INSPECT-AUTH-001 | 008 §7.1 Process inventory read；not executed | R0 | Yes | No | No | No | Pending |
| CAP-INSPECT-AUTH-002 | 008 §7.1 Process inventory read；not executed | R0 | Yes | No | No | No | Pending |
| CAP-INSPECT-AUTH-003 | 008 §7.1 Process inventory read；not executed | R0 | Yes | No | No | No | Pending |
| CAP-INSPECT-AUTH-004 | 008 §7.1 File-system metadata read；not executed | R0 | Yes | No | No | No | Pending |
| CAP-INSPECT-AUTH-005 | 008 §7.1 File-system metadata read；not executed | R0 | Yes | No | No | No | Pending |
| CAP-INSPECT-AUTH-006 | 008 §7.1 File-system metadata read；not executed | R0 | Yes | No | No | No | Pending |
| CAP-INSPECT-AUTH-007 | 008 §7.1 AppX inventory read；not executed | R0 | Yes | No | No | No | Pending |
| CAP-INSPECT-AUTH-008 | 008 §7.1 Package metadata read；not executed | R0 | Yes | No | No | No | Pending |
| CAP-INSPECT-AUTH-009 | 008 §7.1 File-system metadata read；not executed | R0 | Yes | No | No | No | Pending |
| CAP-INSPECT-AUTH-010 | 008 §7.1 File-system metadata read；not executed | R0 | Yes | No | No | No | Pending |
| CAP-INSPECT-AUTH-011 | 008 §7.1 File-system metadata read；not executed | R0 | Yes | No | No | No | Pending |
| CAP-INSPECT-AUTH-012 | 008 §7.1 File-system metadata read；not executed | R0 | Yes | No | No | No | Pending |
| CAP-INSPECT-AUTH-013 | 008 §7.1 File-system metadata read；not executed | R0 | Yes | No | No | No | Pending |
| CAP-INSPECT-AUTH-014 | 008 §7.1 NuGet configuration read；not executed | R0 | Yes | No | No | No | Pending |
| CAP-INSPECT-AUTH-015 | 008 §7.1 Package metadata read；not executed | R0 | Yes | No | No | No | Pending |
| CAP-INSPECT-AUTH-016 | 008 §7.1 Package metadata read；not executed | R0 | Yes | No | No | No | Pending |
| CAP-INSPECT-AUTH-017 | 008 §7.1 File-system metadata read；not executed | R0 | Yes | No | No | No | Pending |
| CAP-INSPECT-AUTH-018 | 008 §7.1 Graphics capability read；not executed | R0 | Yes | No | No | No | Pending |
| CAP-INSPECT-AUTH-019 | 008 §7.1 Display configuration read；not executed | R0 | Yes | No | No | No | Pending |
| CAP-INSPECT-AUTH-020 | 008 §7.1 Display configuration read；Environment inheritance；not executed | R0 | Yes | No | No | No | Pending |

命令邊界規則：

- 不得把 008 的 planned command/API 改寫成「執行整個 PowerShell、`dotnet`、MSBuild、NuGet、Registry」的 unrestricted permission。
- 不得使用 command output redirection、`Export-Csv`、`Set-Content`、`Out-File` 或其他持久化方式。
- 不得因工具同時具有寫入能力，就推定該寫入能力屬於本申請範圍。
- 若唯讀性、參數或資料範圍無法確認，必須停止並回報，不得自行擴大命令。

## 7. Explicitly Prohibited Operations

即使工具另有唯讀功能，本次 Request 也不得包含：

- `dotnet new`、`dotnet restore`、`dotnet build`、`dotnet run`。
- `dotnet workload install`、`dotnet workload update`。
- `msbuild`、`devenv /Build`。
- `nuget install`、`winget install`、`choco install`。
- Visual Studio Installer modification。
- `Add-AppxPackage`、`Register-AppxPackage`。
- NuGet source add/remove/enable/disable。
- `New-Item`、`Set-Content`、`Out-File`、`Export-Csv`。
- Registry write、PATH write、Display/DPI/HDR/Power Plan mutation。
- command output redirection、persistent log、Result/Evidence file creation。
- Capture API invocation、Screenshot、Screen Recording、Frame acquisition、Pixel Difference。
- 任何需要管理員權限、網路、下載、安裝、Restore 或改變 Package Cache 的操作。

## 8. Allowed Observation Boundary

只有在真人針對明確 Request 核准後，才可觀察：

- 已安裝 Windows、SDK、Runtime、IDE、Build Tool、MSBuild 的版本與架構。
- 已存在的 Header、WinMD、Assembly、native asset、Package identity/version/metadata。
- 已存在的 Registry value 與 AppX package identity；僅限指定唯讀欄位。
- 已存在的 Graphics/Display technical metadata：GPU、driver、D3D feature、monitor count/bounds、primary、negative coordinates、DPI/scaling、HDR/color state。
- Repository boundary 與既有 root 的 metadata；不得建立 missing directory。
- Shared UI evidence 的既有 reference；不得把 inherited evidence 改成 capture/runtime proof。

不得：

- 建立缺少的目錄、檔案、Project、Solution、Prototype、Log 或 Evidence。
- Restore、下載或安裝缺少的 Package/SDK/Runtime/workload。
- 改變系統狀態以製造測試條件。
- 啟動 Capture Session、讀取 Desktop image、Window title 或私人內容。
- 將 `Not observed` 改寫成 `Unsupported`，或將 asset existence 改寫成 Build/Runtime 通過。

## 9. Shared UI Authority Boundary

本次 Request 只可引用 Shared UI 的既有 authority context，不得批准任何 Shared UI mutation。下表 18 列明確保留 UI 與 Capture 權責邊界。

| Shared capability | UI authority source | Current UI decision | Capture request effect | Execution effect |
|---|---|---|---|---|
| Windows baseline | UI-AUTH-001 / RESEARCH-TECH-UI-006..009 | Pending | 可引用 host baseline | 只讀；不改 UI |
| .NET SDK/Runtime | UI-AUTH-001 | Pending | 可引用 shared host inventory | 不批准 restore/build |
| Visual Studio/Build Tools | UI-AUTH-001 | Pending | 可引用既有 tool identity | 不批准 Installer/build |
| Windows SDK | UI-AUTH-001、UI-AUTH-005 | Pending | 可引用 SDK asset boundary | 不批准 SDK install |
| Windows App SDK | UI-AUTH-001、UI-AUTH-005 | Pending | 可引用 runtime/SDK identity | 不批准 AppX/package mutation |
| WinUI 3 host assets | UI-AUTH-001、UI-AUTH-002 | Pending | 可引用 host evidence | 不批准 Project/host creation |
| WPF host assets | UI-AUTH-001、UI-AUTH-002 | Pending | 可引用 host evidence | 不批准 Project/Build |
| Handle ownership | UI-AUTH-002 | Pending | 保持 host/capture handle 分離 | 不執行 HWND/HDC capture |
| Overlay lifecycle | UI-AUTH-003 | Pending | 不觀察 overlay/cursor | 不建立 overlay |
| Coordinate authority | UI-AUTH-004 | Pending | 只觀察 technical topology | 不改 product coordinate contract |
| Rendering/device evidence | UI-AUTH-005 | Pending | 只讀既有 metadata | 不建立 device/frame |
| Project/build authority | UI-AUTH-006 | Pending | 只記錄 prerequisites | 不批准 project/restore/build |
| Evidence persistence | UI-AUTH-007 | Pending | future IDs remain planned | 不建立 evidence/result |
| Runtime/display mutation | UI-AUTH-008 | Pending | 只讀 display state | 不改 DPI/HDR/display |
| Package/cache authority | UI-AUTH-006 | Pending | 只讀 existing cache | 不 Restore/clean/mutate |
| Capture backend authority | UI-AUTH-005、UI-AUTH-006 | Pending | 不形成 candidate decision | 不選 backend |
| Privacy/protected content | UI-AUTH-008 | Pending | 排除私人畫面與 protected content | 不呼叫 Capture API |
| Recovery/performance authority | UI-AUTH-005、UI-AUTH-008 | Pending | 保留為 future runtime evidence | 不執行 recovery/performance |

固定：`UI-AUTH-001..008` 全部為 `Pending`。本次獲准最多只代表指定 Request 的唯讀 observation，不代表 UI Enablement、Project、Restore、Build、Runtime 或 Evidence write 獲准。

## 10. Package Cache Boundary

人工審查範圍最多包含：

- 讀取 global-packages path 的 sanitized alias。
- 列出已存在 Package ID/version 目錄。
- 讀取已存在 `.nuspec`、dependency、target-framework、runtime-specific/native asset metadata。
- 讀取公開 Package source hostname，以及 credential presence category：`Present`、`Not present`、`Not inspected`。

不得：

- 網路查詢最新版本、Download、Restore、Cache clean 或 mutation。
- 讀取 credential value、token、password 或 private key。
- 輸出完整 NuGet config、完整 path、完整 environment variables 或不相關私人資料。
- 依 Cache 內容形成 Candidate ranking，或將 Package presence 改寫成 Build compatibility。

## 11. Graphics and Display Boundary

只能申請唯讀取得：

- GPU name、driver version、D3D feature metadata。
- Monitor count、bounds、primary marker、negative-coordinate topology。
- Per-monitor DPI/scaling。
- HDR enabled state、color-space/profile 的非敏感 technical metadata。

不得：

- 建立 D3D capture device workload、Desktop Duplication 或 Graphics Capture Session。
- 改變 display arrangement、primary monitor、DPI、HDR、color profile 或 Power Plan。
- 模擬 device loss、讀取 Desktop image、Window title、monitor serial 或私人畫面。
- 把 bounds、DPI 或 HDR observation 直接轉成 SnipPlus product Coordinate Contract、Crop fidelity、Privacy 或 Recovery 結論。

## 12. Sensitive-data Boundary

禁止讀取或輸出：

- NuGet credential value、API key、token、password、private key。
- 完整環境變數、完整 NuGet config、完整 Registry export。
- 不相關私人路徑、使用者檔名、Window title、Desktop image。
- Monitor serial number、Machine SID 或其他非必要識別資料。

允許回傳：

- 公開 Package source hostname。
- Credential presence category，不包含秘密值。
- 清理後的 SDK、Tool、Package path alias、版本、架構與 API metadata。
- 不含私人桌面內容的 Display technical metadata。

如預期輸出可能包含敏感資料，必須先停止；不得靠事後刪除或覆寫來補救。

## 13. Evidence Persistence Boundary

固定狀態：

| Persistence capability | Current authorization |
|---|---|
| Evidence File Creation Authorized | No |
| Result Directory Creation Authorized | No |
| Command Output Redirection Authorized | No |
| Persistent Log Creation Authorized | No |
| Screenshot Creation Authorized | No |
| Capture Frame Creation Authorized | No |

即使未來真人核准某些 Inspection，最高效果也只是將受限唯讀 observation 回傳到目前互動 Session。`CAP-LOCAL-EVID-001..020` 是規劃中的 future IDs；本文件不得建立目錄、檔案或核准持久化。

## 14. Inspection Batch Design

本節只規劃批次，不執行。

### Batch C-I1 — Shared Host and SDK Inventory

- Included Request IDs: `CAP-INSPECT-AUTH-001..010`。
- Entry criteria: 20 筆 Request 維持 Pending，且 human authorization 尚未批准。
- Required decision state: 每一筆必須有明確 Request ID、R0、Standard user、No network、No mutation、No file output。
- Exact allowed scope: OS、.NET、Visual Studio/Build Tools、Windows SDK、Windows App SDK、WinUI 3、WPF identity/asset metadata。
- Sensitive-data controls: sanitized path、無 credential value、無完整 config、無私人內容。
- Stop conditions: 安裝、Restore、Project、Build、Run、檔案寫入、admin、network。
- Exit criteria: observation 回傳 Session；不建立結果檔案；Not observed 保持語意。
- Execution permission: No。
- Dependency on previous Batch: none。

### Batch C-I2 — Capture Development Assets and Package Metadata

- Included Request IDs: `CAP-INSPECT-AUTH-011..016`。
- Entry criteria: C-I1 不會自動批准 C-I2；需另有明確 human authorization。
- Required decision state: 相關 Request 的 method/API 與 command boundary 已被明確引用 008。
- Exact allowed scope: WGC、D3D11/DXGI、GDI/User32、NuGet、既有 Package/native asset metadata。
- Sensitive-data controls: 不讀 credential、token、private key、完整 config；不形成 candidate ranking。
- Stop conditions: Capture API、device/session、Restore、download、cache mutation、Build、file output。
- Exit criteria: 回傳 package/asset observation；仍需 Project/Build/Runtime evidence。
- Execution permission: No。
- Dependency on previous Batch: host identity 可作 context，但不授予 additional authority。

### Batch C-I3 — Repository、Graphics and Display Observation

- Included Request IDs: `CAP-INSPECT-AUTH-017..020`。
- Entry criteria: C-I1/C-I2 不會自動批准 C-I3；需另有明確 human authorization。
- Required decision state: workspace boundary、technical metadata fields 與 redaction rules 明確。
- Exact allowed scope: repository isolation、既有 roots、GPU/driver/D3D metadata、display/DPI/HDR/color metadata、Shared UI references。
- Sensitive-data controls: workspace boundary only；不讀 desktop/window/private data；不建立 evidence。
- Stop conditions: workspace 外讀取、Display mutation、Capture API、image/frame、Build/Run、file output。
- Exit criteria: 回傳 technical observation；UI-AUTH、capture decision、evidence write 保持 Pending/No。
- Execution permission: No。
- Dependency on previous Batch: only shared definitions；不繼承 execution authorization。

所有 Batch 初始均為 `Execution permission: No`。某一 Batch 未來獲准，不代表其他 Batch 自動獲准。

## 15. Stop Conditions

未來執行任何已批准 Request 時，遇到下列任一情況必須立即停止並回報：

- 需要管理員權限、網路、下載、安裝、Restore 或 Package Cache mutation。
- 需要建立、修改或刪除檔案、目錄、Result、Evidence、Log 或 Project。
- 需要 Registry write、PATH write、Display/DPI/HDR/color profile/Power Plan mutation。
- 需要 Build、Run、Publish、Test、Capture API、Capture Session、Device、Frame、Screenshot 或 Recording。
- 需要取得 credential、token、password、private key、完整 config、Window title、Desktop image、私人內容或非必要識別資料。
- 實際 command/API、參數、範圍與 008 不一致。
- 唯讀性、mutation risk、資料 redaction 或安全分類無法確認。
- 操作超出已核准的 Request ID 或 Batch。
- 需要用 observation 填補 Project、Build、Runtime、Privacy、Recovery、Performance 或 Capture decision。
- 工具產生未授權 output，或無法保證不留下 machine/repository state。

## 16. Human Decision Record

本表正好 20 列，供真人後續填寫；本輪不得填入 `Approved`、`Rejected`、`Deferred` 或任何 Authority/Date。

| Request | Inspection Item | Risk | Requested authorization | Decision | Constraints | Authority | Date | Execution permitted |
|---|---|---|---|---|---|---|---|---|
| CAP-INSPECT-AUTH-001 | CAP-INSPECT-001 | R0 — Read-only local inspection | Required before execution | Pending | See request record | TBD | TBD | No |
| CAP-INSPECT-AUTH-002 | CAP-INSPECT-002 | R0 — Read-only local inspection | Required before execution | Pending | See request record | TBD | TBD | No |
| CAP-INSPECT-AUTH-003 | CAP-INSPECT-003 | R0 — Read-only local inspection | Required before execution | Pending | See request record | TBD | TBD | No |
| CAP-INSPECT-AUTH-004 | CAP-INSPECT-004 | R0 — Read-only local inspection | Required before execution | Pending | See request record | TBD | TBD | No |
| CAP-INSPECT-AUTH-005 | CAP-INSPECT-005 | R0 — Read-only local inspection | Required before execution | Pending | See request record | TBD | TBD | No |
| CAP-INSPECT-AUTH-006 | CAP-INSPECT-006 | R0 — Read-only local inspection | Required before execution | Pending | See request record | TBD | TBD | No |
| CAP-INSPECT-AUTH-007 | CAP-INSPECT-007 | R0 — Read-only local inspection | Required before execution | Pending | See request record | TBD | TBD | No |
| CAP-INSPECT-AUTH-008 | CAP-INSPECT-008 | R0 — Read-only local inspection | Required before execution | Pending | See request record | TBD | TBD | No |
| CAP-INSPECT-AUTH-009 | CAP-INSPECT-009 | R0 — Read-only local inspection | Required before execution | Pending | See request record | TBD | TBD | No |
| CAP-INSPECT-AUTH-010 | CAP-INSPECT-010 | R0 — Read-only local inspection | Required before execution | Pending | See request record | TBD | TBD | No |
| CAP-INSPECT-AUTH-011 | CAP-INSPECT-011 | R0 — Read-only local inspection | Required before execution | Pending | See request record | TBD | TBD | No |
| CAP-INSPECT-AUTH-012 | CAP-INSPECT-012 | R0 — Read-only local inspection | Required before execution | Pending | See request record | TBD | TBD | No |
| CAP-INSPECT-AUTH-013 | CAP-INSPECT-013 | R0 — Read-only local inspection | Required before execution | Pending | See request record | TBD | TBD | No |
| CAP-INSPECT-AUTH-014 | CAP-INSPECT-014 | R0 — Read-only local inspection | Required before execution | Pending | See request record | TBD | TBD | No |
| CAP-INSPECT-AUTH-015 | CAP-INSPECT-015 | R0 — Read-only local inspection | Required before execution | Pending | See request record | TBD | TBD | No |
| CAP-INSPECT-AUTH-016 | CAP-INSPECT-016 | R0 — Read-only local inspection | Required before execution | Pending | See request record | TBD | TBD | No |
| CAP-INSPECT-AUTH-017 | CAP-INSPECT-017 | R0 — Read-only local inspection | Required before execution | Pending | See request record | TBD | TBD | No |
| CAP-INSPECT-AUTH-018 | CAP-INSPECT-018 | R0 — Read-only local inspection | Required before execution | Pending | See request record | TBD | TBD | No |
| CAP-INSPECT-AUTH-019 | CAP-INSPECT-019 | R0 — Read-only local inspection | Required before execution | Pending | See request record | TBD | TBD | No |
| CAP-INSPECT-AUTH-020 | CAP-INSPECT-020 | R0 — Read-only local inspection | Required before execution | Pending | See request record | TBD | TBD | No |

## 17. Request Completeness Matrix

本表正好 20 列。`Yes` 代表 Request record 足以進入人工審查，不代表授權；所有 record 均以 `Yes` 完成包裝，但 execution 仍為 `No`。

| Request | Parent method bound | Command classified | Standard-user only | No network | No mutation | No file output | Sensitive-data controls | Stop conditions | Complete |
|---|---|---|---|---|---|---|---|---|---|
| CAP-INSPECT-AUTH-001 | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes |
| CAP-INSPECT-AUTH-002 | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes |
| CAP-INSPECT-AUTH-003 | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes |
| CAP-INSPECT-AUTH-004 | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes |
| CAP-INSPECT-AUTH-005 | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes |
| CAP-INSPECT-AUTH-006 | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes |
| CAP-INSPECT-AUTH-007 | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes |
| CAP-INSPECT-AUTH-008 | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes |
| CAP-INSPECT-AUTH-009 | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes |
| CAP-INSPECT-AUTH-010 | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes |
| CAP-INSPECT-AUTH-011 | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes |
| CAP-INSPECT-AUTH-012 | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes |
| CAP-INSPECT-AUTH-013 | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes |
| CAP-INSPECT-AUTH-014 | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes |
| CAP-INSPECT-AUTH-015 | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes |
| CAP-INSPECT-AUTH-016 | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes |
| CAP-INSPECT-AUTH-017 | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes |
| CAP-INSPECT-AUTH-018 | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes |
| CAP-INSPECT-AUTH-019 | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes |
| CAP-INSPECT-AUTH-020 | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes | Yes |

## 18. Overall Request Status

### 18.1 Mechanical derivation

20 個 Request records 均具備：

- parent method/command binding to 008。
- `R0 — Read-only local inspection`。
- Standard-user-only。
- No network。
- No mutation。
- No file output。
- Sensitive-data controls。
- Stop conditions。
- Human Decision Record 與 future evidence obligation。

因此：

> Overall Request Status: `Conditionally ready for human authorization review`

此狀態不是 `Approved`，也不是 `Inspection Execution Authorized`。之所以不是 Ready，是因為 Owner、Human Authority、decision date、Shared UI authority 與 future evidence persistence 仍為 TBD/Pending，且 008 上游的 local observation 尚未執行。

### 18.2 Fixed current state

| State | Current value |
|---|---|
| Authorization Decision | Pending |
| Current Authorization | Not granted |
| Inspection Execution Authorized | No |
| Evidence File Creation Authorized | No |
| Closure Execution Authorized | No |
| Build Verification | Not performed |
| Runtime Verification | Not performed |
| Capture Runtime Spike Authorized | No |
| Evidence Write Authorized | No |
| Capture Decision | Not made |
| Rendering Decision | Not made |
| UI Framework Decision | Unresolved — ADR-0002 remains Draft |

## 19. Approval Effect Boundary

即使未來真人批准某個或某批 Request，最高效果只能是：

> 在核准的 Request ID 與限制內，執行 Standard-user、No-network、No-mutation、No-file-output 的本機唯讀觀察，並將受限 observation 回傳目前互動 Session。

批准不代表：

- 可以建立 Evidence file、Result directory、Log 或 output redirection。
- 可以 Restore、下載、安裝 SDK、Runtime、Tool、Package 或 workload。
- 可以建立 Project、Solution、Prototype、Source Code。
- 可以 Build、Run、Publish、Test 或執行 Capture Runtime Spike。
- 可以呼叫 Capture API、建立 Session、Device、Frame、Screenshot 或 Recording。
- `CAP-OFF-GAP` 已關閉、`CAP-PREQ` 已解決、`CAP-BLOCK` 已關閉、`CAP-ENABLE` 已完成。
- Candidate–Host Pair 已通過、Capture Backend 已選定、可以建立 Capture ADR。
- 可以修改 `ADR-0002-ui-framework-selection.md` 或開始正式截圖功能。

## 20. Traceability

| Trace source | Mapping | Future use | Current state |
|---|---|---|---|
| CAP-OFF-GAP-001..015 | Official gap → CAP-INSPECT-001..020 → CAP-INSPECT-AUTH-001..020 | Future human authorization and local observation | Open/partial |
| CAP-INSPECT-001..020 | Inspection item → matching authorization request | Future read-only inspection | Not executed |
| CAP-INSPECT-AUTH-001..020 | Request → future human decision | Scope-limited authorization | Pending |
| CAP-LOCAL-EVID-001..020 | Future observation → future evidence decision | Future evidence persistence review | Not created |
| CAP-PREQ-001..030 | Prerequisite → request scope | Future closure reassessment | Not closed |
| CAP-BLOCK-001..012 | Blocker → request contribution | Future readiness review | Open or deferred |
| CAP-PAIR-001..010 | Candidate–Host Pair → host/capture asset coverage | Future Project/Build/Runtime work | No ranking |
| CAP-ENABLE-001..007 | Observation contribution → enablement | Future authorization packaging | Recommendation only |
| CAP-CGATE-001..010 | Local contribution → closure gate | Future closure review | Not passed |
| RESEARCH-TECH-CAPTURE-003..008 | Existing capture research line | Parent evidence and method binding | Referenced |
| RESEARCH-TECH-UI-006..009 | Shared UI evidence inheritance | Avoid duplicate host checks | Inherited only |
| RESEARCH-TECH-RENDER-003 | Rendering prerequisite context | Future synthetic scene review | Referenced only |
| ADR-0002-ui-framework-selection.md | UI decision context | Future authority review | Draft; unresolved |
| Architecture/TECHNOLOGY-DECISION-ROADMAP.md | Technology decision context | Future candidate decision | No decision made |

實際文件名稱與 ID 必須以 Repository 原樣為準；本文件沒有新增官方 evidence claim，也沒有建立 Human Authorization Decision。

## Completion Conditions

- 只建立 `docs/Research/Technology/28-capture-backend-read-only-local-inspection-authorization-request.md`。
- 建立正好 20 個 `CAP-INSPECT-AUTH-001..020`，並與 20 個 `CAP-INSPECT-001..020` 一對一。
- 不新增或修改任何 Inspection method/command/API；所有 boundary 直接引用 008。
- 建立正好 20 列 Command Boundary Register、20 列 Human Decision Record、20 列 Request Completeness Matrix。
- 每個 Request 都是 R0、Standard user、No network、No mutation、No file output。
- 所有 Decision 都是 `Pending`，所有 Authority/Date 都是 `TBD`，所有 `Execution permitted` 都是 `No`。
- Evidence File、Result directory、Persistent log、output redirection、Screenshot、Capture Frame 全部未授權。
- 不執行任何 Inspection command，不建立 Result、Evidence、Project、Prototype 或 Source Code。
- 不執行下載、安裝、Restore、Build、Run、Publish、Capture API 或 Runtime Spike。
- 不建立 Human Authorization Decision，不修改 UI/Rendering Research Line，不修改 ADR-0002，不建立 Capture ADR。
- `Overall Request Status` 只能推導為 `Conditionally ready for human authorization review`。
- `git diff --check` 應通過。

## Current Execution Record

| Item | Status |
|---|---|
| Authorization request created | Yes — this document only |
| Human authorization decision | Not created |
| Inspection command execution | Not performed |
| Local environment inspection | Not performed |
| Package cache inspection | Not performed |
| Build verification | Not performed |
| Runtime verification | Not performed |
| Capture runtime spike | Not performed |
| Evidence file creation | Not performed |
| Screenshot/recording/frame creation | Not performed |
| Capture backend decision | Not made |
| Rendering decision | Not made |
