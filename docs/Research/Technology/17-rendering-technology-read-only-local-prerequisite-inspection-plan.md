# Rendering Technology Read-only Local Prerequisite Inspection Plan

## Document Control

| Field | Value |
|---|---|
| Document ID | `RESEARCH-TECH-RENDER-008` |
| Title | Rendering Technology Read-only Local Prerequisite Inspection Plan |
| Status | Draft |
| Research type | Read-only local inspection plan |
| Execution status | Not started |
| Local environment inspection | Not performed |
| Package cache inspection | Not performed |
| Build verification | Not performed |
| Runtime verification | Not performed |
| Parent reassessment | `RESEARCH-TECH-RENDER-007` |
| Official evidence baseline | `RESEARCH-TECH-RENDER-006` |
| Parent enablement specification | `RESEARCH-TECH-RENDER-005` |
| Host framework decision | Unresolved |
| Rendering decision | Not made |
| Inspection execution authorized | No |
| Closure execution authorized | No |
| Runtime spike execution authorized | No |
| Owner | TBD |
| Last reviewed | Not reviewed |
| Version | 0.1 |
| Date | 2026-07-26 |
| References | `RESEARCH-TECH-RENDER-003` through `007`, `RESEARCH-TECH-UI-006` through `009`, `ADR-0002`, `TECHNOLOGY-DECISION-ROADMAP` |

## 1. 任務目的

本文件只回答：

> 為了補足 `RESEARCH-TECH-RENDER-007` 中可由本機唯讀查核取得的證據，應檢查哪些 SDK、Runtime、Build Tool、Package Cache、Host integration asset 與 Rendering dependency；每項檢查的安全邊界、輸出欄位及證據義務為何？

這是 Inspection Plan，不是 Inspection Record。所有命令與 API 只作為未來計畫文字記錄，本文件建立時不得執行任何命令。

## 2. Scope

只規劃下列唯讀查核：

- Windows、CPU architecture 與既有 UI baseline 的繼承。
- .NET SDK、.NET Runtime、Windows Desktop Runtime 與 workload inventory。
- Visual Studio、Build Tools、`vswhere`、MSBuild 與 Windows SDK 的既有可用性。
- Direct2D／DirectWrite development asset 的唯讀存在性。
- Windows App SDK Runtime package 的既有 inventory。
- WinUI 3 template、MSBuild target 與 Windows App SDK SDK asset 的既有存在性。
- WPF targeting pack、reference assembly 與 build path 的既有存在性。
- Win2D、SkiaSharp、integration package 與 native asset 的既有 Package Cache metadata。
- NuGet source、config provenance、global-packages path 與已存在 dependency metadata。
- Repository isolation boundary 與未來 evidence root 是否已存在；不得建立它們。
- GPU、Display、DPI、HDR 證據的既有 UI Research 繼承與剩餘缺口。

## 3. Non-goals

本文件不得：

- 執行任何規劃中的命令或 API。
- 進行新的官方網路研究。
- 下載或安裝 SDK、Runtime、Tool、Package 或 workload。
- 修改 NuGet source 或設定。
- 執行 Package acquisition 或 Restore。
- 建立 Project、Solution、Prototype 或 Source Code。
- 執行 Build、Run、Publish、測量或 Runtime Spike。
- 建立 Result directory、實際 Log、Environment Record、Package Inventory 或 Evidence。
- 修改 Registry、PATH、Display、DPI、HDR 或 Power Plan。
- 使用系統管理員權限。
- 建立 `RND-AUTH`。
- 修改 UI Research Line 或 `RESEARCH-TECH-RENDER-001..007`。
- 選擇 Rendering Technology。

## 4. Controlled Vocabulary

### 4.1 Inspection Item Status

只能使用：

- `Planned`
- `Blocked`
- `Deferred`
- `Not applicable`

### 4.2 Future Observation Result

只能使用：

- `Observed`
- `Not observed`
- `Conflicting`
- `Unavailable`
- `Not executed`

本文件所有 Observation Result 必須為 `Not executed`。

### 4.3 Inspection Authorization

只能使用：

- `Not granted`

### 4.4 Execution Permission

只能使用：

- `No`

不得使用 `Completed`、`Resolved`、`Approved` 或 `Yes`。本文件描述的是未授權計畫，不是結果。

## 5. Source Binding

所有 Inspection Item 必須追溯到至少一項既有來源：

| Source family | Binding |
|---|---|
| Official evidence gaps | `RND-OFF-GAP-001..016` |
| Enablement items | `RND-ENABLE-001..006` |
| Closure gates | `RND-CGATE-001..008` |
| Prerequisites | `RND-PREQ-001..021` |
| Blocking actions | `RND-BLOCK-001..009` |
| Candidate–Host pairs | `RND-PAIR-001..010` |
| Shared UI research | `RESEARCH-TECH-UI-006..009` |

規則：

- 只規劃能透過本機唯讀查核取得的證據。
- 需要 Package acquisition、Build 或 Runtime 的 Gap 不得偽裝成唯讀查核。
- Shared UI Host evidence 必須引用既有 UI Research ID。
- 不重複定義 `UI-AUTH-001..008`。
- 上游描述不足時建立 `RND-INSPECT-GAP-xxx` 候選註記，不修改上游。

## 6. Inspection Item Register

建立正好 18 個 Inspection Item：

| ID | Inspection subject | Primary evidence class | Planned status |
|---|---|---|---|
| `RND-INSPECT-001` | Windows／architecture baseline inheritance | Environment inheritance | Planned |
| `RND-INSPECT-002` | .NET SDK inventory | Process inventory read | Planned |
| `RND-INSPECT-003` | .NET Runtime／Windows Desktop Runtime inventory | Process inventory read | Planned |
| `RND-INSPECT-004` | Installed workload inventory | Process inventory read | Planned |
| `RND-INSPECT-005` | Visual Studio／Build Tools／`vswhere` availability | Process inventory read | Planned |
| `RND-INSPECT-006` | MSBuild path and provenance | Process and file metadata read | Planned |
| `RND-INSPECT-007` | Windows SDK version roots | Registry read | Planned |
| `RND-INSPECT-008` | Direct2D／DirectWrite development assets | File-system metadata read | Planned |
| `RND-INSPECT-009` | Windows App SDK Runtime package inventory | AppX inventory read | Planned |
| `RND-INSPECT-010` | Windows App SDK SDK／NuGet cache assets | File-system metadata read | Planned |
| `RND-INSPECT-011` | WinUI 3 template and MSBuild target assets | File-system metadata read | Planned |
| `RND-INSPECT-012` | WPF targeting pack and reference assemblies | File-system metadata read | Planned |
| `RND-INSPECT-013` | Win2D cached package identity and versions | Package metadata read | Planned |
| `RND-INSPECT-014` | SkiaSharp cached packages and native assets | Package metadata read | Planned |
| `RND-INSPECT-015` | NuGet sources, config provenance and global-packages path | NuGet configuration read | Planned |
| `RND-INSPECT-016` | Cached dependency metadata and transitive dependency evidence | Package metadata read | Planned |
| `RND-INSPECT-017` | Repository isolation and evidence-root existence | File-system metadata read | Planned |
| `RND-INSPECT-018` | GPU／Display／DPI／HDR evidence inheritance and remaining gaps | Environment inheritance | Planned |

## 7. Detailed Inspection Item Specifications

每個 Item 都使用相同的安全與證據欄位。下列 planned command 或 API 僅供未來授權審查，不得在本輪執行。

### RND-INSPECT-001 — Windows／architecture baseline inheritance

| Field | Planned value |
|---|---|
| Inspection Item ID | `RND-INSPECT-001` |
| Inspection question | Current Windows build、OS architecture、process architecture 與既有 UI baseline 是否可供 Rendering prerequisite 使用？ |
| Source Gap IDs | `RND-OFF-GAP-013`, `015` |
| Related Enablement Items | `RND-ENABLE-001`, `002`, `005` |
| Related Closure Gates | `RND-CGATE-002`, `005` |
| Related Candidate–Host Pairs | `RND-PAIR-001..010` |
| Dependency ownership | Environment |
| Existing evidence | `RESEARCH-TECH-UI-006..009` 的既有 Windows／Host／Display baseline；不重複查詢已足夠的資料 |
| Planned read-only method | 先引用既有 UI evidence，再以標準使用者唯讀查詢補足缺欄位 |
| Planned command or API | `Get-CimInstance Win32_OperatingSystem`; `Get-CimInstance Win32_ComputerSystem`; `[Environment]::Is64BitOperatingSystem` |
| Command execution environment | PowerShell，目標工作區之外的系統唯讀查詢；未來才可執行 |
| Expected privilege | Standard user |
| Network access required | No |
| Mutation risk | None |
| Expected output fields | Windows build、OS architecture、system type、current user context；不記錄完整使用者名稱 |
| Sensitive-data considerations | 遮罩 user name、domain、serial、machine name、完整 profile path |
| Proposed future evidence ID | `RND-LOCAL-EVID-001` |
| Proposed future evidence destination | `docs/Research/Technology/results/rendering/local-prerequisite-inspection/`；本輪不建立 |
| Success condition | 可在不變更系統的前提下取得最小 OS／architecture 欄位 |
| Not-observed interpretation | `Not executed` 只表示尚未查核，不表示不相容 |
| Conflict handling | 與 UI evidence 不一致時保留兩者、建立 conflict note，不修改上游 |
| Failure／tool-missing handling | 記錄 command unavailable 或 access denied，不升級權限 |
| Fallback method | 只使用既有 UI evidence；不得用猜測補值 |
| Phase R1 impact | 影響 architecture baseline；未來可能阻止 R1 package／Build scope |
| Inspection authorization | Not granted |
| Execution permitted | No |
| Observation result | Not executed |
| Owner | TBD |
| Status | Planned |
| Open questions | Exact target architecture、WinUI/WPF process architecture 是否相同 |

### RND-INSPECT-002 — .NET SDK inventory

| Field | Planned value |
|---|---|
| Inspection Item ID | `RND-INSPECT-002` |
| Inspection question | 已存在的 .NET SDK versions 是否足以描述未來 project target 的可選範圍？ |
| Source Gap IDs | `RND-OFF-GAP-001`, `006`, `007`, `011` |
| Related Enablement Items | `RND-ENABLE-001`, `005` |
| Related Closure Gates | `RND-CGATE-003`, `004` |
| Related Candidate–Host Pairs | `RND-PAIR-001..010` |
| Dependency ownership | Environment |
| Existing evidence | `RND-OFF-EVID-013`, `017` 的官方 version／TFM separation；無本機版本證據 |
| Planned read-only method | 查詢既有 `dotnet` executable 與 SDK list，不 restore、不修改 SDK |
| Planned command or API | `Get-Command dotnet`; `dotnet --list-sdks` |
| Command execution environment | PowerShell standard user；未來授權後才可執行 |
| Expected privilege | Standard user |
| Network access required | No |
| Mutation risk | None |
| Expected output fields | SDK version、SDK root、command path；不記錄 credential 或完整 user path |
| Sensitive-data considerations | 遮罩 user profile path、machine-specific path 片段 |
| Proposed future evidence ID | `RND-LOCAL-EVID-002` |
| Proposed future evidence destination | Future local inspection evidence root；本輪不建立 |
| Success condition | 取得既有 SDK version list 或明確記錄 command unavailable |
| Not-observed interpretation | 缺少 SDK 只表示本機未知，不排除官方 Candidate |
| Conflict handling | 多個 SDK root 或版本衝突時逐項保留 provenance |
| Failure／tool-missing handling | 記錄 `dotnet` missing；不得下載或安裝 |
| Fallback method | 只記錄既有 UI／parent evidence，不能推定版本 |
| Phase R1 impact | 影響 project target feasibility，可能阻止 future Build scope |
| Inspection authorization | Not granted |
| Execution permitted | No |
| Observation result | Not executed |
| Owner | TBD |
| Status | Planned |
| Open questions | Future project target framework 尚未決定 |

### RND-INSPECT-003 — .NET Runtime／Windows Desktop Runtime inventory

| Field | Planned value |
|---|---|
| Inspection Item ID | `RND-INSPECT-003` |
| Inspection question | 已存在的 .NET Runtime 與 Windows Desktop Runtime 是否可供未來 Host 啟動前置檢查？ |
| Source Gap IDs | `RND-OFF-GAP-001`, `002`, `006`, `007` |
| Related Enablement Items | `RND-ENABLE-001`, `002`, `005` |
| Related Closure Gates | `RND-CGATE-002`, `003`, `005` |
| Related Candidate–Host Pairs | `RND-PAIR-001`, `002`, `007`, `008` |
| Dependency ownership | Environment |
| Existing evidence | Parent documents only state Runtime not performed |
| Planned read-only method | 讀取 installed runtime list，不啟動任何 project |
| Planned command or API | `dotnet --list-runtimes`; `Get-ChildItem` targeted runtime metadata only |
| Command execution environment | PowerShell standard user；未來授權後才可執行 |
| Expected privilege | Standard user |
| Network access required | No |
| Mutation risk | None |
| Expected output fields | Runtime name、version、architecture if exposed、command path |
| Sensitive-data considerations | 遮罩 user path與machine-specific installation path |
| Proposed future evidence ID | `RND-LOCAL-EVID-003` |
| Proposed future evidence destination | Future local inspection evidence root；本輪不建立 |
| Success condition | Runtime inventory can be captured without launching application code |
| Not-observed interpretation | Runtime absent or unknown does not prove Candidate incompatibility |
| Conflict handling | 同名不同 version 逐項保留，不自行選 stable |
| Failure／tool-missing handling | Tool missing is an observation gap；不得安裝 |
| Fallback method | Parent evidence only |
| Phase R1 impact | Supports host prerequisite description; does not close Runtime evidence |
| Inspection authorization | Not granted |
| Execution permitted | No |
| Observation result | Not executed |
| Owner | TBD |
| Status | Planned |
| Open questions | WPF target and WinUI target may require different Runtime rows |

### RND-INSPECT-004 — Installed workload inventory

| Field | Planned value |
|---|---|
| Inspection Item ID | `RND-INSPECT-004` |
| Inspection question | 已存在的 .NET workload 是否可被識別，且是否影響 WinUI 3／WPF 的 future project setup？ |
| Source Gap IDs | `RND-OFF-GAP-001`, `006`, `011` |
| Related Enablement Items | `RND-ENABLE-001`, `005` |
| Related Closure Gates | `RND-CGATE-003`, `004` |
| Related Candidate–Host Pairs | `RND-PAIR-001`, `005`, `007` |
| Dependency ownership | Environment |
| Existing evidence | No workload inventory; Build not performed |
| Planned read-only method | 讀取 workload list；禁止 workload install/update |
| Planned command or API | `dotnet workload list` |
| Command execution environment | PowerShell standard user；未來授權後才可執行 |
| Expected privilege | Standard user |
| Network access required | No |
| Mutation risk | None |
| Expected output fields | Installed workload IDs、manifest version、advertising state if shown |
| Sensitive-data considerations | 不輸出 source credential、machine identifier 或完整 profile path |
| Proposed future evidence ID | `RND-LOCAL-EVID-004` |
| Proposed future evidence destination | Future local inspection evidence root；本輪不建立 |
| Success condition | Workload list is captured as read-only text or marked unavailable |
| Not-observed interpretation | No workload result does not prove package incompatibility |
| Conflict handling | Manifest version mismatch must be recorded as conflict |
| Failure／tool-missing handling | Record tool missing or workload command failure; no repair |
| Fallback method | No fallback installation; retain `Not executed` |
| Phase R1 impact | May block future WinUI project setup description |
| Inspection authorization | Not granted |
| Execution permitted | No |
| Observation result | Not executed |
| Owner | TBD |
| Status | Planned |
| Open questions | Whether future project requires an explicit workload |

### RND-INSPECT-005 — Visual Studio／Build Tools／vswhere availability

| Field | Planned value |
|---|---|
| Inspection Item ID | `RND-INSPECT-005` |
| Inspection question | Existing Visual Studio、Build Tools 與 `vswhere` 是否能被定位，且 provenance 是否可記錄？ |
| Source Gap IDs | `RND-OFF-GAP-001`, `003`, `004`, `015` |
| Related Enablement Items | `RND-ENABLE-001`, `005` |
| Related Closure Gates | `RND-CGATE-002`, `004`, `005` |
| Related Candidate–Host Pairs | `RND-PAIR-001..010` |
| Dependency ownership | Environment |
| Existing evidence | Build tools not inspected; Build not performed |
| Planned read-only method | Locate existing executable and query installed instance metadata |
| Planned command or API | `Get-Command vswhere.exe`; `vswhere.exe -products * -format json` |
| Command execution environment | PowerShell standard user；未來授權後才可執行 |
| Expected privilege | Standard user |
| Network access required | No |
| Mutation risk | None |
| Expected output fields | Installation path、product ID、display version、catalog version、MSBuild component presence |
| Sensitive-data considerations | 遮罩完整 user path、subscription／license fields、machine name |
| Proposed future evidence ID | `RND-LOCAL-EVID-005` |
| Proposed future evidence destination | Future local inspection evidence root；本輪不建立 |
| Success condition | Existing tool availability and provenance can be described without launching IDE |
| Not-observed interpretation | `vswhere` unavailable does not prove Build Tools absent |
| Conflict handling | Multiple installations must remain separate records |
| Failure／tool-missing handling | Record missing tool; do not install or open installer |
| Fallback method | Targeted file metadata read of known installation path if already available |
| Phase R1 impact | Affects future Build operation planning |
| Inspection authorization | Not granted |
| Execution permitted | No |
| Observation result | Not executed |
| Owner | TBD |
| Status | Planned |
| Open questions | Which IDE or Build Tools instance will be authorized for future spike |

### RND-INSPECT-006 — MSBuild path and provenance

| Field | Planned value |
|---|---|
| Inspection Item ID | `RND-INSPECT-006` |
| Inspection question | 可用 MSBuild path、file version 與 parent installation provenance 是否可被唯讀記錄？ |
| Source Gap IDs | `RND-OFF-GAP-003`, `004`, `006`, `007`, `015` |
| Related Enablement Items | `RND-ENABLE-001`, `005` |
| Related Closure Gates | `RND-CGATE-004`, `005` |
| Related Candidate–Host Pairs | `RND-PAIR-003`, `004`, `007`, `008` |
| Dependency ownership | Environment |
| Existing evidence | No MSBuild path record; Build not performed |
| Planned read-only method | Locate executable and read file metadata/hash only if path is already known |
| Planned command or API | `Get-Command msbuild.exe`; `Get-Item`; `[System.Diagnostics.FileVersionInfo]::GetVersionInfo(...)` |
| Command execution environment | PowerShell standard user；未來授權後才可執行 |
| Expected privilege | Standard user |
| Network access required | No |
| Mutation risk | None |
| Expected output fields | Resolved path、file version、product version、source installation |
| Sensitive-data considerations | 遮罩 user path、machine name、unrelated installation paths |
| Proposed future evidence ID | `RND-LOCAL-EVID-006` |
| Proposed future evidence destination | Future local inspection evidence root；本輪不建立 |
| Success condition | MSBuild provenance is captured without running a build |
| Not-observed interpretation | Missing MSBuild is an environment gap only |
| Conflict handling | Different MSBuild versions remain separate with path provenance |
| Failure／tool-missing handling | Record unavailable; no PATH edit or installer action |
| Fallback method | Reuse `RND-INSPECT-005` output if it includes MSBuild component evidence |
| Phase R1 impact | Blocks exact future Build command packaging |
| Inspection authorization | Not granted |
| Execution permitted | No |
| Observation result | Not executed |
| Owner | TBD |
| Status | Planned |
| Open questions | MSBuild version must align with selected future project target |

### RND-INSPECT-007 — Windows SDK version roots

| Field | Planned value |
|---|---|
| Inspection Item ID | `RND-INSPECT-007` |
| Inspection question | Existing Windows SDK version roots and Include／Lib／bin locations 是否存在？ |
| Source Gap IDs | `RND-OFF-GAP-003`, `004`, `015` |
| Related Enablement Items | `RND-ENABLE-001`, `005` |
| Related Closure Gates | `RND-CGATE-003`, `005` |
| Related Candidate–Host Pairs | `RND-PAIR-003`, `004` |
| Dependency ownership | Environment |
| Existing evidence | Official native API identity only; local SDK inventory absent |
| Planned read-only method | Read installed roots from known Registry location and targeted directories |
| Planned command or API | `Get-ItemProperty HKLM:\SOFTWARE\Microsoft\Windows Kits\Installed Roots`; targeted `Test-Path` |
| Command execution environment | PowerShell standard user；未來授權後才可執行 |
| Expected privilege | Standard user |
| Network access required | No |
| Mutation risk | None |
| Expected output fields | SDK version root、Include／Lib／bin existence、architecture subfolders |
| Sensitive-data considerations | Do not dump unrelated Registry values or machine identifiers |
| Proposed future evidence ID | `RND-LOCAL-EVID-007` |
| Proposed future evidence destination | Future local inspection evidence root；本輪不建立 |
| Success condition | Existing Windows SDK root metadata is captured without Registry write |
| Not-observed interpretation | Missing root does not invalidate Direct2D official API identity |
| Conflict handling | Registry root and directory mismatch is a conflict note |
| Failure／tool-missing handling | Record access denied or missing root; no admin elevation |
| Fallback method | Parent official evidence only |
| Phase R1 impact | Affects Direct2D／DirectWrite future Build scope |
| Inspection authorization | Not granted |
| Execution permitted | No |
| Observation result | Not executed |
| Owner | TBD |
| Status | Planned |
| Open questions | Exact Windows SDK baseline remains TBD |

### RND-INSPECT-008 — Direct2D／DirectWrite development assets

| Field | Planned value |
|---|---|
| Inspection Item ID | `RND-INSPECT-008` |
| Inspection question | Existing headers、metadata、libraries 與 related Direct2D／DirectWrite assets 是否可被定位？ |
| Source Gap IDs | `RND-OFF-GAP-003`, `004`, `015` |
| Related Enablement Items | `RND-ENABLE-005` |
| Related Closure Gates | `RND-CGATE-004`, `005` |
| Related Candidate–Host Pairs | `RND-PAIR-003`, `004` |
| Dependency ownership | Rendering-specific |
| Existing evidence | `RND-OFF-EVID-006..008` confirms API identity, not local assets |
| Planned read-only method | Use SDK roots from `RND-INSPECT-007`; inspect targeted asset paths and metadata |
| Planned command or API | Targeted `Test-Path`; `Get-Item` on known `d2d1.h`, `dwrite.h`, library metadata paths |
| Command execution environment | PowerShell standard user；未來授權後才可執行 |
| Expected privilege | Standard user |
| Network access required | No |
| Mutation risk | None |
| Expected output fields | Header／library path、version metadata if present、architecture folders |
| Sensitive-data considerations | Do not recurse unrelated SDK directories; redact machine-specific paths |
| Proposed future evidence ID | `RND-LOCAL-EVID-008` |
| Proposed future evidence destination | Future local inspection evidence root；本輪不建立 |
| Success condition | Asset existence can be recorded without compiling or loading native code |
| Not-observed interpretation | Asset absence is a local prerequisite gap, not official API rejection |
| Conflict handling | Header／library version mismatch remains conflicting evidence |
| Failure／tool-missing handling | Record missing path; no SDK installation |
| Fallback method | Windows SDK root metadata only |
| Phase R1 impact | Blocks precise Direct2D／DirectWrite Build package boundary |
| Inspection authorization | Not granted |
| Execution permitted | No |
| Observation result | Not executed |
| Owner | TBD |
| Status | Planned |
| Open questions | Managed interop wrapper remains undecided |

### RND-INSPECT-009 — Windows App SDK Runtime package inventory

| Field | Planned value |
|---|---|
| Inspection Item ID | `RND-INSPECT-009` |
| Inspection question | Existing Windows App SDK Runtime packages、architecture與registration state 是否可被唯讀記錄？ |
| Source Gap IDs | `RND-OFF-GAP-001`, `006`, `015` |
| Related Enablement Items | `RND-ENABLE-001`, `002`, `005` |
| Related Closure Gates | `RND-CGATE-002`, `005` |
| Related Candidate–Host Pairs | `RND-PAIR-001`, `005`, `007` |
| Dependency ownership | Environment |
| Existing evidence | Official deployment boundary only; local Runtime inventory absent |
| Planned read-only method | Query existing AppX package metadata only |
| Planned command or API | `Get-AppxPackage -Name Microsoft.WindowsAppRuntime*` |
| Command execution environment | PowerShell standard user；未來授權後才可執行 |
| Expected privilege | Standard user |
| Network access required | No |
| Mutation risk | None |
| Expected output fields | Package name、version、architecture、status、publisher；no package content extraction |
| Sensitive-data considerations | Redact user SID、install location details not needed for evidence |
| Proposed future evidence ID | `RND-LOCAL-EVID-009` |
| Proposed future evidence destination | Future local inspection evidence root；本輪不建立 |
| Success condition | Existing package metadata is listed without register/install action |
| Not-observed interpretation | No package result does not prove WinUI 3 unsupported |
| Conflict handling | Multiple architecture/version rows retain exact provenance |
| Failure／tool-missing handling | Record cmdlet unavailable or access denied; no admin elevation |
| Fallback method | Reuse existing UI evidence or official deployment evidence |
| Phase R1 impact | Supports future WinUI Runtime prerequisite description only |
| Inspection authorization | Not granted |
| Execution permitted | No |
| Observation result | Not executed |
| Owner | TBD |
| Status | Planned |
| Open questions | Packaged versus unpackaged future Host remains unresolved |

### RND-INSPECT-010 — Windows App SDK SDK／NuGet cache assets

| Field | Planned value |
|---|---|
| Inspection Item ID | `RND-INSPECT-010` |
| Inspection question | Existing Windows App SDK SDK and NuGet cache assets 是否存在且版本可被辨識？ |
| Source Gap IDs | `RND-OFF-GAP-001`, `006`, `011` |
| Related Enablement Items | `RND-ENABLE-001`, `005` |
| Related Closure Gates | `RND-CGATE-003`, `004` |
| Related Candidate–Host Pairs | `RND-PAIR-001`, `005`, `007` |
| Dependency ownership | Environment |
| Existing evidence | Official package/version metadata; no local cache inspection |
| Planned read-only method | Resolve existing global-packages path, then inspect only named package roots |
| Planned command or API | `dotnet nuget locals global-packages --list`; targeted `Test-Path` and `Get-Item` |
| Command execution environment | PowerShell standard user；未來授權後才可執行 |
| Expected privilege | Standard user |
| Network access required | No |
| Mutation risk | None |
| Expected output fields | Cache path、package ID、version folder、TFM／RID asset folder presence |
| Sensitive-data considerations | Redact username, source credentials and unrelated package names |
| Proposed future evidence ID | `RND-LOCAL-EVID-010` |
| Proposed future evidence destination | Future local inspection evidence root；本輪不建立 |
| Success condition | Named cache metadata is read without restore or cache mutation |
| Not-observed interpretation | Cache absence does not exclude the Candidate |
| Conflict handling | Package version folder and nuspec mismatch remains conflict |
| Failure／tool-missing handling | Record path unavailable; no cache creation or restore |
| Fallback method | No local fallback; retain official evidence only |
| Phase R1 impact | May inform future package acquisition scope; does not close Build |
| Inspection authorization | Not granted |
| Execution permitted | No |
| Observation result | Not executed |
| Owner | TBD |
| Status | Planned |
| Open questions | Exact package lock remains undecided |

### RND-INSPECT-011 — WinUI 3 template and MSBuild target assets

| Field | Planned value |
|---|---|
| Inspection Item ID | `RND-INSPECT-011` |
| Inspection question | Existing WinUI 3 templates、targets、props 與 SDK asset 是否可被唯讀辨識？ |
| Source Gap IDs | `RND-OFF-GAP-001`, `006`, `011` |
| Related Enablement Items | `RND-ENABLE-001`, `005` |
| Related Closure Gates | `RND-CGATE-002`, `004` |
| Related Candidate–Host Pairs | `RND-PAIR-001`, `005`, `007` |
| Dependency ownership | Shared UI research／Rendering-specific |
| Existing evidence | `RESEARCH-TECH-UI-006..009`; no local template inventory |
| Planned read-only method | List existing templates and inspect named SDK target metadata without creating a project |
| Planned command or API | `dotnet new list`; targeted `Get-Item`／`Get-Content` on already identified SDK target files |
| Command execution environment | PowerShell standard user；未來授權後才可執行 |
| Expected privilege | Standard user |
| Network access required | No |
| Mutation risk | None |
| Expected output fields | Template identity、installed version、target path、required package identity |
| Sensitive-data considerations | Do not dump unrelated templates or credential-bearing config |
| Proposed future evidence ID | `RND-LOCAL-EVID-011` |
| Proposed future evidence destination | Future local inspection evidence root；本輪不建立 |
| Success condition | Existing template/target metadata is captured without `dotnet new` creation |
| Not-observed interpretation | Template absent does not prove WinUI 3 unsupported |
| Conflict handling | Template version and SDK target version mismatch is recorded |
| Failure／tool-missing handling | Record missing template/tool; no template install |
| Fallback method | Existing UI evidence and Windows App SDK official identity |
| Phase R1 impact | May block future project setup specification |
| Inspection authorization | Not granted |
| Execution permitted | No |
| Observation result | Not executed |
| Owner | TBD |
| Status | Planned |
| Open questions | Future WinUI 3 project template is not selected |

### RND-INSPECT-012 — WPF targeting pack and reference assemblies

| Field | Planned value |
|---|---|
| Inspection Item ID | `RND-INSPECT-012` |
| Inspection question | WPF targeting pack、reference assemblies 與 build path 是否存在？ |
| Source Gap IDs | `RND-OFF-GAP-002`, `007`, `015` |
| Related Enablement Items | `RND-ENABLE-001`, `005` |
| Related Closure Gates | `RND-CGATE-002`, `004`, `005` |
| Related Candidate–Host Pairs | `RND-PAIR-002`, `004`, `006`, `008`, `010` |
| Dependency ownership | Environment |
| Existing evidence | `RND-OFF-EVID-005`, `024`; local WPF pack not inspected |
| Planned read-only method | Inspect only targeted `Reference Assemblies` and known WPF target paths |
| Planned command or API | Targeted `Test-Path`; `Get-Item` on known WPF reference assembly paths |
| Command execution environment | PowerShell standard user；未來授權後才可執行 |
| Expected privilege | Standard user |
| Network access required | No |
| Mutation risk | None |
| Expected output fields | TFM folder、WPF assembly file version、reference path |
| Sensitive-data considerations | Do not recurse unrelated framework folders; redact machine-specific path |
| Proposed future evidence ID | `RND-LOCAL-EVID-012` |
| Proposed future evidence destination | Future local inspection evidence root；本輪不建立 |
| Success condition | Existing WPF targeting assets are recorded without project creation |
| Not-observed interpretation | Missing targeting pack does not invalidate WPF official identity |
| Conflict handling | Multiple target packs remain separate; no automatic latest selection |
| Failure／tool-missing handling | Record path unavailable; no targeting pack installation |
| Fallback method | Parent WPF official evidence |
| Phase R1 impact | May block future WPF project target description |
| Inspection authorization | Not granted |
| Execution permitted | No |
| Observation result | Not executed |
| Owner | TBD |
| Status | Planned |
| Open questions | Future WPF TFM and deployment model remain TBD |

### RND-INSPECT-013 — Win2D cached package identity and versions

| Field | Planned value |
|---|---|
| Inspection Item ID | `RND-INSPECT-013` |
| Inspection question | `Microsoft.Graphics.Win2D` cached package ID、version、TFM與native asset folders 是否存在？ |
| Source Gap IDs | `RND-OFF-GAP-005`, `011` |
| Related Enablement Items | `RND-ENABLE-005` |
| Related Closure Gates | `RND-CGATE-003`, `004`, `005` |
| Related Candidate–Host Pairs | `RND-PAIR-005`, `006` |
| Dependency ownership | Rendering-specific |
| Existing evidence | `RND-OFF-EVID-009..011`; no local cache evidence |
| Planned read-only method | Inspect only existing package root, nuspec and named asset folders |
| Planned command or API | Targeted `Get-Item`; `Get-Content` for existing `.nuspec`; no restore |
| Command execution environment | PowerShell standard user；未來授權後才可執行 |
| Expected privilege | Standard user |
| Network access required | No |
| Mutation risk | None |
| Expected output fields | Package ID、version、TFM、RID、native asset folder、dependency metadata |
| Sensitive-data considerations | Redact cache username and unrelated packages |
| Proposed future evidence ID | `RND-LOCAL-EVID-013` |
| Proposed future evidence destination | Future local inspection evidence root；本輪不建立 |
| Success condition | Existing Win2D metadata is captured without package acquisition |
| Not-observed interpretation | Cache absence does not exclude Win2D |
| Conflict handling | Registry version and cache version are separate observations |
| Failure／tool-missing handling | Record missing cache or malformed nuspec; no repair |
| Fallback method | Official evidence baseline only |
| Phase R1 impact | Informs package scope; WPF Host gap remains open |
| Inspection authorization | Not granted |
| Execution permitted | No |
| Observation result | Not executed |
| Owner | TBD |
| Status | Planned |
| Open questions | Exact Win2D version and WPF treatment remain undecided |

### RND-INSPECT-014 — SkiaSharp cached packages and native assets

| Field | Planned value |
|---|---|
| Inspection Item ID | `RND-INSPECT-014` |
| Inspection question | SkiaSharp core、WPF／WinUI view、Win32／WinUI native asset package 的 cached identity and versions 是否存在？ |
| Source Gap IDs | `RND-OFF-GAP-006`, `007`, `010`, `015` |
| Related Enablement Items | `RND-ENABLE-005` |
| Related Closure Gates | `RND-CGATE-003`, `004`, `005` |
| Related Candidate–Host Pairs | `RND-PAIR-007`, `008` |
| Dependency ownership | Rendering-specific |
| Existing evidence | `RND-OFF-EVID-016..021`; official version context remains conflicting |
| Planned read-only method | Inspect named package roots, nuspec dependency metadata and native asset folders only |
| Planned command or API | Targeted `Get-Item`; `Get-Content` existing `.nuspec`; no package acquisition |
| Command execution environment | PowerShell standard user；未來授權後才可執行 |
| Expected privilege | Standard user |
| Network access required | No |
| Mutation risk | None |
| Expected output fields | Core/view/native package IDs、versions、TFM、RID、native asset folder、dependency relation |
| Sensitive-data considerations | Redact user cache path and unrelated package data |
| Proposed future evidence ID | `RND-LOCAL-EVID-014` |
| Proposed future evidence destination | Future local inspection evidence root；本輪不建立 |
| Success condition | Existing package metadata is captured without resolving the official version conflict by assumption |
| Not-observed interpretation | Missing native asset does not prove SkiaSharp Host incompatibility |
| Conflict handling | Preserve repository, registry and cache version provenance separately |
| Failure／tool-missing handling | Record cache unavailable or malformed metadata; no restore or repair |
| Fallback method | Official evidence baseline with `Conflicting` status |
| Phase R1 impact | Blocks exact package/native asset authorization scope |
| Inspection authorization | Not granted |
| Execution permitted | No |
| Observation result | Not executed |
| Owner | TBD |
| Status | Planned |
| Open questions | Which version line, if any, can be proposed after conflict resolution |

### RND-INSPECT-015 — NuGet sources, config provenance and global-packages path

| Field | Planned value |
|---|---|
| Inspection Item ID | `RND-INSPECT-015` |
| Inspection question | Existing NuGet sources、config precedence與global-packages path 是否可被唯讀記錄而不暴露 credentials？ |
| Source Gap IDs | `RND-OFF-GAP-006`, `007`, `010`, `011` |
| Related Enablement Items | `RND-ENABLE-005`, `006` |
| Related Closure Gates | `RND-CGATE-003`, `004`, `008` |
| Related Candidate–Host Pairs | `RND-PAIR-005`, `007`, `008` |
| Dependency ownership | Environment／Evidence |
| Existing evidence | No local NuGet config provenance; package acquisition prohibited |
| Planned read-only method | List source names and paths with credentials suppressed; list cache path only |
| Planned command or API | `dotnet nuget list source`; `dotnet nuget locals global-packages --list`; targeted config metadata read |
| Command execution environment | PowerShell standard user；未來授權後才可執行 |
| Expected privilege | Standard user |
| Network access required | No |
| Mutation risk | None |
| Expected output fields | Source name、enabled state、config provenance、global-packages path |
| Sensitive-data considerations | Never print passwords, API keys, access tokens, private feed URLs or credential provider output |
| Proposed future evidence ID | `RND-LOCAL-EVID-015` |
| Proposed future evidence destination | Future local inspection evidence root；本輪不建立 |
| Success condition | Config provenance is captured with secrets omitted |
| Not-observed interpretation | Source/cache absence does not exclude Candidate |
| Conflict handling | Multiple config layers remain separate; no source enable/disable mutation |
| Failure／tool-missing handling | Record command unavailable; no package manager repair |
| Fallback method | Read only non-secret path metadata from already authorized scope |
| Phase R1 impact | Affects future package acquisition specification, not official support |
| Inspection authorization | Not granted |
| Execution permitted | No |
| Observation result | Not executed |
| Owner | TBD |
| Status | Planned |
| Open questions | Private feeds are out of scope unless separately authorized |

### RND-INSPECT-016 — Cached dependency metadata and transitive dependency evidence

| Field | Planned value |
|---|---|
| Inspection Item ID | `RND-INSPECT-016` |
| Inspection question | Existing `.nuspec`、assets metadata與transitive dependency records 是否能補足 package identity evidence？ |
| Source Gap IDs | `RND-OFF-GAP-006`, `007`, `010`, `011`, `015` |
| Related Enablement Items | `RND-ENABLE-005` |
| Related Closure Gates | `RND-CGATE-004`, `005` |
| Related Candidate–Host Pairs | `RND-PAIR-005..008` |
| Dependency ownership | Evidence |
| Existing evidence | Official package metadata; no local dependency metadata |
| Planned read-only method | Read existing package metadata and existing project assets only if explicitly in scope |
| Planned command or API | Targeted `Get-Content` for `.nuspec`, `project.assets.json` or equivalent existing metadata; no Restore |
| Command execution environment | PowerShell standard user；未來授權後才可執行 |
| Expected privilege | Standard user |
| Network access required | No |
| Mutation risk | None |
| Expected output fields | Direct dependency、transitive dependency、TFM、RID、native asset relationship |
| Sensitive-data considerations | Redact source URLs with embedded credentials and unrelated project paths |
| Proposed future evidence ID | `RND-LOCAL-EVID-016` |
| Proposed future evidence destination | Future local inspection evidence root；本輪不建立 |
| Success condition | Existing dependency metadata can be read without generating assets |
| Not-observed interpretation | Missing metadata does not prove dependency absence |
| Conflict handling | Direct package metadata and cache metadata are kept as separate evidence sources |
| Failure／tool-missing handling | Record malformed or unavailable metadata; no regeneration |
| Fallback method | Official evidence baseline with Unknown/Conflicting status |
| Phase R1 impact | May improve package scope but cannot close Build or Runtime |
| Inspection authorization | Not granted |
| Execution permitted | No |
| Observation result | Not executed |
| Owner | TBD |
| Status | Planned |
| Open questions | No project assets may exist because this task forbids Project creation |

### RND-INSPECT-017 — Repository isolation and evidence-root existence

| Field | Planned value |
|---|---|
| Inspection Item ID | `RND-INSPECT-017` |
| Inspection question | Planned isolation path與future evidence root 是否存在；若不存在是否能維持不建立原則？ |
| Source Gap IDs | `RND-OFF-GAP-014`, `016` |
| Related Enablement Items | `RND-ENABLE-004`, `006` |
| Related Closure Gates | `RND-CGATE-007`, `008` |
| Related Candidate–Host Pairs | `RND-PAIR-001..010` |
| Dependency ownership | Evidence |
| Existing evidence | Parent plans explicitly state result directory not created |
| Planned read-only method | Check exact path existence only; never create, enumerate broad roots or write output |
| Planned command or API | `Test-Path` for `experiments/rendering/<host>/<candidate>/` and `docs/Research/Technology/results/rendering/` |
| Command execution environment | PowerShell standard user；未來授權後才可執行 |
| Expected privilege | Standard user |
| Network access required | No |
| Mutation risk | None |
| Expected output fields | Exact path, exists flag, file/directory type |
| Sensitive-data considerations | Do not list unrelated files or reveal external workspace paths |
| Proposed future evidence ID | `RND-LOCAL-EVID-017` |
| Proposed future evidence destination | No destination may be created by this item |
| Success condition | Path existence is known without directory creation or broad enumeration |
| Not-observed interpretation | Missing root is expected and does not indicate failure |
| Conflict handling | Existing unexpected content is recorded as scope conflict, not deleted |
| Failure／tool-missing handling | Record access failure; no retry with elevated privilege |
| Fallback method | Parent plan state remains authoritative |
| Phase R1 impact | Defines isolation prerequisite and evidence boundary |
| Inspection authorization | Not granted |
| Execution permitted | No |
| Observation result | Not executed |
| Owner | TBD |
| Status | Planned |
| Open questions | Exact host/candidate directory names remain future plan data |

### RND-INSPECT-018 — GPU／Display／DPI／HDR evidence inheritance and remaining gaps

| Field | Planned value |
|---|---|
| Inspection Item ID | `RND-INSPECT-018` |
| Inspection question | Existing UI evidence 是否已足夠描述 GPU、Display topology、DPI與HDR；哪些仍必須保留為 Runtime evidence？ |
| Source Gap IDs | `RND-OFF-GAP-012`, `013`, `014` |
| Related Enablement Items | `RND-ENABLE-002`, `003`, `004` |
| Related Closure Gates | `RND-CGATE-002`, `006`, `007` |
| Related Candidate–Host Pairs | `RND-PAIR-001..010` |
| Dependency ownership | Shared UI research／Environment／Evidence |
| Existing evidence | Reuse `RESEARCH-TECH-UI-006..009`; no new display query in this plan |
| Planned read-only method | Compare existing UI evidence fields to Rendering prerequisite fields; only plan missing reads |
| Planned command or API | Future targeted `Get-CimInstance Win32_VideoController`; existing UI evidence lookup; no display mutation |
| Command execution environment | PowerShell standard user；未來授權後才可執行 |
| Expected privilege | Standard user |
| Network access required | No |
| Mutation risk | None |
| Expected output fields | GPU vendor/model, driver version, display count, DPI evidence provenance, HDR evidence state |
| Sensitive-data considerations | Redact monitor serials, device IDs, user profile and machine identifiers |
| Proposed future evidence ID | `RND-LOCAL-EVID-018` |
| Proposed future evidence destination | Future local inspection evidence root；本輪不建立 |
| Success condition | Reuse decision and remaining Runtime-only gaps are explicit |
| Not-observed interpretation | Missing local display data does not imply renderer incompatibility |
| Conflict handling | Existing UI and future local data retain separate timestamps and provenance |
| Failure／tool-missing handling | Record unavailable; no driver update or display setting change |
| Fallback method | Existing UI evidence only |
| Phase R1 impact | DPI/Display gaps can remain Deferred when they do not block request specification |
| Inspection authorization | Not granted |
| Execution permitted | No |
| Observation result | Not executed |
| Owner | TBD |
| Status | Planned |
| Open questions | HDR and fractional DPI remain Runtime workload questions |

## 8. Planned Command Safety Classification

每個規劃中的 command 必須屬於下列唯讀分類：

| Classification | 說明 | Allowed in this plan | Executed in this task |
|---|---|---|---|
| Process inventory read | 查詢既有 executable、tool version 或 package list | Planned only | No |
| File-system metadata read | 查詢指定檔案、目錄、版本與大小 | Planned only | No |
| Registry read | 只讀取 Registry | Planned only | No |
| AppX inventory read | 只列出已安裝 Package metadata | Planned only | No |
| NuGet configuration read | 只讀取 source、config 與 cache path | Planned only | No |
| Package metadata read | 只讀取既有 `.nuspec`、assets 或 package metadata | Planned only | No |
| Environment inheritance | 引用既有研究證據，不重新查詢 | Planned only | No |

明確禁止規劃成可執行操作：

- `dotnet restore`
- `dotnet build`
- `dotnet run`
- `dotnet new`
- `nuget install`
- `winget install`
- `choco install`
- Visual Studio Installer 變更。
- workload install／update。
- AppX install／register。
- NuGet source add／remove／enable／disable。
- Registry write。
- 目錄建立或檔案輸出。

命令文字可以記錄於文件，但不得在本輪執行。

## 9. Shared UI Evidence Inheritance Matrix

| Rendering requirement | UI source evidence | Reusable evidence | Re-query required | Remaining Rendering gap |
|---|---|---|---|---|
| Windows 11 x64 baseline | `RESEARCH-TECH-UI-006` | Yes if timestamp and scope remain valid | Only if stale or incomplete | `RND-OFF-GAP-001`, `015` |
| .NET SDK／Runtime baseline | `RESEARCH-TECH-UI-006` | Partial | Planned `RND-INSPECT-002`, `003` if fields absent | `RND-OFF-GAP-001`, `006`, `007` |
| Windows SDK baseline | `RESEARCH-TECH-UI-007` | Partial | Planned `RND-INSPECT-007`, `008` | `RND-OFF-GAP-003`, `004` |
| Visual Studio／Build Tools | `RESEARCH-TECH-UI-007` | Partial | Planned `RND-INSPECT-005`, `006` if provenance absent | `RND-OFF-GAP-003`, `004`, `015` |
| WinUI 3／Windows App SDK availability | `RESEARCH-TECH-UI-008` | Partial | Planned `RND-INSPECT-009..011` | `RND-OFF-GAP-001`, `006`, `011` |
| WPF build path | `RESEARCH-TECH-UI-008` | Partial | Planned `RND-INSPECT-012` | `RND-OFF-GAP-002`, `007` |
| Display topology | `RESEARCH-TECH-UI-006` | Partial | Planned `RND-INSPECT-018` only if stale | `RND-OFF-GAP-013` |
| Per-monitor DPI | `RESEARCH-TECH-UI-009` | Partial | Runtime evidence remains separate | `RND-OFF-GAP-013` |
| GPU／driver | `RESEARCH-TECH-UI-009` | Partial | Planned `RND-INSPECT-018` if missing | `RND-OFF-GAP-012`, `013` |
| Evidence storage policy | `RESEARCH-TECH-UI-009` | Yes for policy | No duplicate query | `RND-OFF-GAP-014` |
| Safety／cleanup | `RESEARCH-TECH-UI-009` | Yes for boundary | No duplicate query | `RND-OFF-GAP-014` |
| Shared execution authorization | `RESEARCH-TECH-UI-009` | Yes for authority boundary | Reuse `UI-AUTH-001..008` | Shared authority remains Pending |

可繼承證據不得無理由重複查詢；不完整或過期證據才可規劃重新查詢。Shared UI authorization 仍由 `UI-AUTH-001..008` 管理。

## 10. Package Cache Inspection Boundary

### 10.1 Allowed future read scope

只允許規劃：

- 查詢 global-packages path。
- 列出已存在的 Package ID 與 version directories。
- 讀取已存在的 `.nuspec`。
- 讀取既有 dependency metadata。
- 辨識 native asset folder、target framework folder 與 runtime-specific asset folder。
- 記錄 package path、version、TFM、RID 與 provenance。

### 10.2 Forbidden future operations

不得：

- Restore 缺少的 Package。
- 連線 NuGet 查詢最新版本。
- 修改、刪除或清理 Package Cache。
- 將 Package Cache 存在視為 Build compatibility。
- 將 Package Cache 不存在視為 Candidate 不支援。
- 讀取 credential store、`.env`、private key、token、password 或 private feed credential。

## 11. Gap-to-Inspection Matrix

| Gap | Current disposition | Can local read-only inspection contribute | Inspection IDs | Remaining evidence class | Blocks authorization request |
|---|---|---|---|---|---|
| `RND-OFF-GAP-001` | Open | Yes, Host asset inventory only | `001`, `002`, `009`, `011` | Build evidence | Yes |
| `RND-OFF-GAP-002` | Requires runtime evidence | Partially, WPF asset inventory only | `003`, `012` | Runtime evidence | Yes |
| `RND-OFF-GAP-003` | Requires build evidence | Yes, SDK and MSBuild assets | `005`–`008` | Build evidence | Yes |
| `RND-OFF-GAP-004` | Requires build evidence | Yes, SDK and MSBuild assets | `005`–`008` | Build evidence | Yes |
| `RND-OFF-GAP-005` | Accepted documentation limitation | No direct closure; may identify local package only | `013` | Runtime evidence | Yes |
| `RND-OFF-GAP-006` | Requires build evidence | Yes, package/cache metadata | `002`, `009`–`016` | Build evidence | Yes |
| `RND-OFF-GAP-007` | Requires build evidence | Yes, WPF/package metadata | `003`, `012`, `014`–`016` | Build evidence | Yes |
| `RND-OFF-GAP-008` | Open | No, components are not named | `017` | Build evidence | Yes |
| `RND-OFF-GAP-009` | Open | No, components are not named | `017` | Build evidence | Yes |
| `RND-OFF-GAP-010` | Requires package acquisition evidence | Cache can show local provenance only | `014`–`016` | Package acquisition evidence | Yes |
| `RND-OFF-GAP-011` | Requires package acquisition evidence | Cache can show local package only | `010`, `013`, `015`, `016` | Package acquisition evidence | Yes |
| `RND-OFF-GAP-012` | Deferred | Existing GPU inheritance may inform scope | `018` | Deferred Phase R2 | No |
| `RND-OFF-GAP-013` | Deferred | Existing display/DPI evidence may be reused | `001`, `018` | Deferred Phase R2 | No |
| `RND-OFF-GAP-014` | Open | Path existence only; method still documentary | `017`, `018` | Shared UI authority | Yes |
| `RND-OFF-GAP-015` | Requires build evidence | Architecture and asset inventory | `001`, `006`–`010`, `013`, `014` | Build evidence | Yes |
| `RND-OFF-GAP-016` | Deferred | No product decision from inventory | `017` | Deferred Phase R3 | No |

唯讀盤點不能關閉 Build 或 Runtime Gap，也不能形成 Candidate selection。

## 12. Candidate–Host Inspection Coverage

| Pair | Required local evidence | Inspection IDs | Package Cache contribution | Build still required | Runtime still required |
|---|---|---|---|---|---|
| `RND-PAIR-001` Framework-native／WinUI 3 | SDK、Runtime、template、architecture | `001`, `002`, `003`, `009`, `011` | Host asset presence only | Yes | Yes |
| `RND-PAIR-002` Framework-native／WPF | Runtime、MSBuild、WPF targeting pack | `001`, `003`, `006`, `012` | WPF asset presence only | Yes | Yes |
| `RND-PAIR-003` Direct2D／DirectWrite／WinUI 3 | SDK roots、native headers／libraries、MSBuild | `005`–`008` | Not applicable to native API identity | Yes | Yes |
| `RND-PAIR-004` Direct2D／DirectWrite／WPF | SDK roots、native headers／libraries、WPF path | `005`–`008`, `012` | Not applicable to native API identity | Yes | Yes |
| `RND-PAIR-005` Win2D／WinUI 3 | SDK、Runtime、template、Win2D cache | `009`–`013`, `015`, `016` | Package identity only | Yes | Yes |
| `RND-PAIR-006` Win2D／WPF | Win2D cache and WPF path | `012`, `013`, `015`, `016` | Package presence only | Yes | Yes |
| `RND-PAIR-007` SkiaSharp／WinUI 3 | SDK、Runtime、view/native package cache | `009`–`011`, `014`–`016` | Package/native asset metadata only | Yes | Yes |
| `RND-PAIR-008` SkiaSharp／WPF | WPF path and SkiaSharp Win32/native cache | `012`, `014`–`016` | Package/native asset metadata only | Yes | Yes |
| `RND-PAIR-009` Hybrid／WinUI 3 | Component ownership and shared Host inheritance | `001`, `009`–`011`, `017` | Cannot close unnamed components | Yes | Yes |
| `RND-PAIR-010` Hybrid／WPF | Component ownership and WPF inheritance | `001`, `012`, `017` | Cannot close unnamed components | Yes | Yes |

## 13. Future Evidence Plan

只規劃，不建立：

```text
docs/Research/Technology/results/rendering/local-prerequisite-inspection/
```

### 13.1 Future evidence IDs

| Evidence range | Meaning | Current state |
|---|---|---|
| `RND-LOCAL-EVID-001`–`018` | One future evidence record per Inspection Item | Not created |
| `RND-INSPECT-GAP-xxx` | Future planning gap if an item cannot be safely specified | Not created |

### 13.2 Required future evidence fields

每筆未來 Evidence 至少記錄：

- Inspection Item ID。
- Timestamp。
- Windows build。
- User privilege level。
- Exact command or API。
- Exit code。
- Standard output。
- Standard error。
- Observed paths。
- Observed versions。
- Evidence source。
- Conflict notes。
- Sensitive values removed。
- Interpretation。
- Related Gap。
- Related Enablement Item。

本輪不得建立目錄、Evidence、Log 或 Environment Record。

## 14. Authorization Packaging Matrix

正好 18 列；每列都固定為 standard-user、no-network、no-mutation、Not granted、No。

| Inspection Item | Safety classification | Standard-user only | Network required | Mutation expected | Current authorization | Execution permitted |
|---|---|---|---|---|---|---|
| `RND-INSPECT-001` | Environment inheritance | Yes | No | No | Not granted | No |
| `RND-INSPECT-002` | Process inventory read | Yes | No | No | Not granted | No |
| `RND-INSPECT-003` | Process inventory read | Yes | No | No | Not granted | No |
| `RND-INSPECT-004` | Process inventory read | Yes | No | No | Not granted | No |
| `RND-INSPECT-005` | Process inventory read | Yes | No | No | Not granted | No |
| `RND-INSPECT-006` | Process and file metadata read | Yes | No | No | Not granted | No |
| `RND-INSPECT-007` | Registry read | Yes | No | No | Not granted | No |
| `RND-INSPECT-008` | File-system metadata read | Yes | No | No | Not granted | No |
| `RND-INSPECT-009` | AppX inventory read | Yes | No | No | Not granted | No |
| `RND-INSPECT-010` | File-system metadata read | Yes | No | No | Not granted | No |
| `RND-INSPECT-011` | File-system metadata read | Yes | No | No | Not granted | No |
| `RND-INSPECT-012` | File-system metadata read | Yes | No | No | Not granted | No |
| `RND-INSPECT-013` | Package metadata read | Yes | No | No | Not granted | No |
| `RND-INSPECT-014` | Package metadata read | Yes | No | No | Not granted | No |
| `RND-INSPECT-015` | NuGet configuration read | Yes | No | No | Not granted | No |
| `RND-INSPECT-016` | Package metadata read | Yes | No | No | Not granted | No |
| `RND-INSPECT-017` | File-system metadata read | Yes | No | No | Not granted | No |
| `RND-INSPECT-018` | Environment inheritance | Yes | No | No | Not granted | No |

## 15. Readiness to Request Inspection Authorization

### 15.1 Decision vocabulary

只能使用：

- `Ready to request read-only local inspection authorization`
- `Conditionally ready to request read-only local inspection authorization`
- `Not ready to request read-only local inspection authorization`

### 15.2 Mechanical derivation

```text
Inspection methods fully specified
+ all commands classified as read-only
+ no network dependency
+ no administrator dependency
+ evidence obligations defined
+ rollback not required because no mutation
------------------------------------------------
= Inspection Authorization Readiness
```

### 15.3 Current readiness

**Inspection Authorization Readiness: `Conditionally ready to request read-only local inspection authorization`**

理由：18 個 Item、命令分類、安全欄位與證據義務已定義；但本文件本身沒有授權權限，且上游 Shared UI authority、Rendering closure execution 與 Runtime Spike 仍未授權。這個判定只表示未來可以另行提出唯讀查核授權審查，不表示任何查核已被授權或執行。

即使判定為 Conditionally ready，仍固定：

- `Inspection Execution Authorized: No`
- `Closure Execution Authorized: No`
- `Build Verification: Not performed`
- `Runtime Verification: Not performed`
- `Runtime Spike Execution Authorized: No`
- `Rendering Decision: Not made`

## 16. Traceability

```text
RND-OFF-GAP
  -> RND-INSPECT-001..018
  -> Future RND-LOCAL-EVID
  -> RND-ENABLE reassessment
  -> Future closure authorization request
```

至少引用：

- `RESEARCH-TECH-RENDER-003`
- `RESEARCH-TECH-RENDER-004`
- `RESEARCH-TECH-RENDER-005`
- `RESEARCH-TECH-RENDER-006`
- `RESEARCH-TECH-RENDER-007`
- `RESEARCH-TECH-UI-006`
- `RESEARCH-TECH-UI-007`
- `RESEARCH-TECH-UI-008`
- `RESEARCH-TECH-UI-009`
- `ADR-0002`
- `TECHNOLOGY-DECISION-ROADMAP`

## 17. Completion Boundary

本任務完成條件：

- 只建立 `17-rendering-technology-read-only-local-prerequisite-inspection-plan.md`。
- 建立正好 18 個 unique `RND-INSPECT`。
- 覆蓋 16 個 `RND-OFF-GAP`。
- 覆蓋十個 Candidate–Host Pair。
- 所有規劃命令均明確分類為唯讀。
- 所有項目均為 standard-user、no-network、no-mutation。
- 所有 `Current authorization = Not granted`。
- 所有 `Execution permitted = No`。
- 所有 `Observation result = Not executed`。
- 不執行任何計畫命令。
- 不建立 Result directory、Project、Prototype、Source Code、Evidence、Log 或 Environment Record。
- 不執行下載、安裝、Restore、Build、Run、Publish 或 Runtime Spike。
- 不建立 `RND-AUTH`。
- 不修改 ADR-0002 或建立 TD-002 ADR。
- 只做唯讀文件檢查，確認 `git diff --check` 不產生 whitespace error。

