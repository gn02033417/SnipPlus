# UI Framework Runtime Environment Baseline

狀態：`Draft`

本文件是 `RESEARCH-TECH-UI-004` 的 Environment and Version Evidence Baseline。它記錄 2026-07-26 以唯讀方式取得的 Windows、硬體、SDK、Runtime、Build Tool 與證據工具資料，並把資料映射回 `RESEARCH-TECH-UI-003` 的前置條件與阻塞項目。本文件不安裝工具、不建立 Project、不執行 Build、不執行 Runtime Spike，也不建立 Prototype。

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | RESEARCH-TECH-UI-004 |
| Title | UI Framework Runtime Environment Baseline |
| Status | Draft |
| Research Type | Environment and Version Evidence Inspection |
| Inspection Status | Partially completed |
| Runtime Verification | Not performed |
| Parent Readiness Record | RESEARCH-TECH-UI-003 |
| Owner | TBD |
| Last reviewed | Not reviewed |
| Version | 0.1 |
| Inspection date | 2026-07-26 |
| Evidence capture date | 2026-07-26 |
| Normative References | `docs/Research/Technology/01-ui-framework-feasibility.md`, `docs/Research/Technology/02-ui-framework-runtime-spike-plan.md`, `docs/Research/Technology/03-ui-framework-runtime-spike-execution-readiness.md`, `Architecture/adr/ADR-0002-ui-framework-selection.md` |
| Informative References | Official release and platform sources listed in [Traceability](#16-traceability) |
| Supersedes | None |
| Superseded by | None |

## 2. Purpose

本文件用於：

- 取得可驗證的本機環境與設備資料。
- 查核 Runtime Spike 可使用的精確實驗版本。
- 確認哪些測試環境真實存在，哪些仍為 `Unknown` 或 `Unavailable`。
- 將證據映射回 `UI-PREQ-xxx` 與 `UI-BLOCK-xxx`。
- 為重新評估 Phase 1 environment sufficiency 提供依據。

本文件的版本資料分成兩類：官方目前可用版本，以及本機目前已安裝或可使用版本。兩者不得混寫，也不得把本機盤點結果當成產品正式版本決策。

## 3. Scope

本次盤點只涵蓋：

- Windows 版本與 Build。
- CPU architecture、CPU、RAM、GPU 與 Driver。
- 螢幕數量、可觀察的顯示資訊、DPI 與 HDR 證據狀態。
- 已安裝的 .NET SDK／Runtime。
- Windows SDK。
- Visual Studio／Build Tools 的唯讀可見性。
- WinUI 3／Windows App SDK 的官方候選版本與本機可用性。
- WPF 對應的 Windows Desktop Runtime。
- Accessibility 與 Diagnostic tools。
- x64、ARM64、Packaged、Unpackaged 測試能力的證據狀態。

## 4. Non-goals

本文件不得：

- 安裝、移除或升級任何工具。
- 建立 Project 或 Solution。
- 執行 Build、Restore 或 Publish。
- 執行 Runtime Spike、Performance Test、Accessibility Test 或 Deployment Test。
- 建立 Prototype 或產品 Source Code。
- 修改 `ADR-0002`、`RESEARCH-TECH-UI-003`、PRD、Specs 或 Architecture。
- 決定正式產品版本。
- 選擇 Capture、Rendering 或 Clipboard Backend。
- 建立實際 Result Artifact。

## 5. Evidence Rules

- Framework、SDK 與 Tool 版本的官方資料必須來自官方 release、download 或 platform 文件。
- 本機環境必須來自唯讀系統查詢、Registry 查詢、已安裝工具的版本輸出或系統介面。
- 每筆本機證據必須記錄查核日期。
- 官方可用版本與本機已安裝版本必須分開記錄。
- 找不到資料時使用 `Unknown`，不得猜測。
- 沒有實體或可重現測試設備時使用 `Unavailable`，不得虛構。
- 本文件提出的候選版本只供未來 Spike 比較，不代表產品正式選型。
- 不得將網路文章、社群說法或搜尋摘要作為唯一版本依據。
- 本文件的「可用」只代表可被盤點或作為未來前置條件的候選，不代表 Spike 已獲授權執行。

## 6. Evidence Status Vocabulary

本文件的 Evidence Status 只能使用：

- `Verified`
- `Partially verified`
- `Unavailable`
- `Unknown`
- `Conflicting evidence`

版本用途只能使用：

- `Candidate for spike`
- `Installed and available`
- `Not installed`
- `Deferred`
- `Blocked`

如果資料只透過 PATH、單一 Registry location 或單一 WMI class 取得，不能擴大宣稱為完整安裝狀態；此時使用 `Partially verified` 或 `Unknown`。

## 7. Local System Inventory

下表來自 2026-07-26 的唯讀 Windows system query。未取得的欄位保留為 `Unknown`，不補填推測值。

| Field | Observed value | Evidence method | Evidence date | Status |
| --- | --- | --- | --- | --- |
| Windows edition | Microsoft Windows 11 專業版 | `Win32_OperatingSystem.Caption` | 2026-07-26 | Verified |
| Windows version | 10.0.26200 | `Win32_OperatingSystem.Version` | 2026-07-26 | Verified |
| Windows build | 26200 | `Win32_OperatingSystem.BuildNumber` | 2026-07-26 | Verified |
| System architecture | x64-based PC; OS 64-bit | `Win32_ComputerSystem.SystemType` and `Win32_OperatingSystem.OSArchitecture` | 2026-07-26 | Verified |
| CPU | AMD Ryzen 7 5800X 8-Core Processor | `Win32_Processor.Name` | 2026-07-26 | Verified |
| CPU topology | 8 physical cores; 16 logical processors | `Win32_Processor.NumberOfCores` and `NumberOfLogicalProcessors` | 2026-07-26 | Verified |
| Installed RAM | 68,628,316,160 bytes; approximately 63.9 GiB | `Win32_ComputerSystem.TotalPhysicalMemory` | 2026-07-26 | Verified |
| GPU | NVIDIA GeForce RTX 2070 SUPER | `Win32_VideoController.Name` | 2026-07-26 | Verified |
| GPU driver | 32.0.15.9186 | `Win32_VideoController.DriverVersion` | 2026-07-26 | Verified |
| Reported primary video mode | 2560 × 1440 | `Win32_VideoController.VideoModeDescription` | 2026-07-26 | Partially verified |
| Active display count | 3 active monitor records | `WmiMonitorBasicDisplayParams` | 2026-07-26 | Verified |
| Per-monitor resolution | Not separately mapped by the read-only query | WMI monitor records did not expose per-display mode | 2026-07-26 | Unknown |
| Per-monitor DPI scaling | Not obtained; `LogPixels` was empty and `Win8DpiScaling` was 0 in the queried user registry value | `HKCU:\Control Panel\Desktop` and WMI inspection | 2026-07-26 | Unknown |
| HDR availability/state | Not obtained | No HDR-specific evidence was captured | 2026-07-26 | Unknown |
| D: available space | 427,518,914,560 bytes; approximately 398.1 GiB | `Win32_LogicalDisk` for D: | 2026-07-26 | Verified |
| D: total size | 1,000,202,039,296 bytes; approximately 931.5 GiB | `Win32_LogicalDisk` for D: | 2026-07-26 | Verified |
| Power mode | 平衡 | `powercfg /getactivescheme` | 2026-07-26 | Verified |
| User privilege context | Not Administrator; standard-user context observed | `WindowsPrincipal.IsInRole(Administrator)` | 2026-07-26 | Verified |

### Current display records

The three active records below are real WMI records. Their exact desktop resolutions, per-monitor scaling and HDR state remain unverified.

| Display ID | WMI identity | Physical size reported by EDID | Resolution | Scaling | HDR | Primary | Evidence status |
| --- | --- | --- | --- | --- | --- | --- | --- |
| DISPLAY-001 | `DISPLAY\AUS2704\...\UID24832_0` | 60 × 34 cm | Not separately verified; primary GPU mode reports 2560 × 1440 | Unknown | Unknown | Unknown | Partially verified |
| DISPLAY-002 | `DISPLAY\APT0464\...\UID24836_0` | 31 × 23 cm | Not separately verified | Unknown | Unknown | Unknown | Partially verified |
| DISPLAY-003 | `DISPLAY\AUS2704\...\UID24837_0` | 60 × 34 cm | Not separately verified | Unknown | Unknown | Unknown | Partially verified |

## 8. Installed Development Environment

本節只記錄已觀察到的安裝或命令可見性，不執行任何 Build 或 Restore。

| Tool | Installed version | Architecture | Location or evidence | Status |
| --- | --- | --- | --- | --- |
| Visual Studio | Not found in PATH; no instance found in queried Visual Studio Setup Registry locations | Unknown | `Get-Command devenv`; Visual Studio Setup Registry query | Not found |
| Visual Studio Build Tools | Not found in PATH; no instance found in queried Visual Studio Setup Registry locations | Unknown | `Get-Command msbuild`; Visual Studio Setup Registry query | Not found |
| .NET SDK | 10.0.302; MSBuild 18.6.11 | x64 host, RID `win-x64` | `dotnet --list-sdks`, `dotnet --info`; `C:\Program Files\dotnet\sdk\10.0.302` | Installed and available |
| .NET Runtime | 6.0.36, 8.0.29, 9.0.18, 10.0.10 | x64 host | `dotnet --list-runtimes`; `C:\Program Files\dotnet\shared\Microsoft.NETCore.App` | Installed and available |
| Windows Desktop Runtime | 6.0.36, 8.0.29, 9.0.18, 10.0.10 | x64 host | `dotnet --list-runtimes`; `Microsoft.WindowsDesktop.App` entries | Installed and available |
| Windows SDK | 10.0.22621.0 and 10.0.26100.0 include trees | x64 tools available; ARM64/x86 bin folders also present | `C:\Program Files (x86)\Windows Kits\10\Include` and `bin` directory listing | Installed and available |
| Windows App SDK／WinUI workload | No .NET workload listed; direct Windows App SDK package availability not fully inspected | Unknown | `dotnet workload list`; no project or package restore performed | Unknown |
| Git | 2.55.0.3 | Unknown | `C:\Program Files\Git\cmd\git.exe` via `Get-Command git` | Installed and available |
| PowerShell 7 | 7.6.3 | x64 host | `C:\Program Files\PowerShell\7\pwsh.exe` via `Get-Command pwsh` | Installed and available |
| Windows PowerShell | 10.0.26100.8875 command version | System-provided | `C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe` via `Get-Command powershell` | Installed and available |

### Diagnostic and evidence tools observed

| Tool | Version | Location | Status | Limitation |
| --- | --- | --- | --- | --- |
| Windows Performance Recorder (`wpr.exe`) | 10.0.26100.4188 | Windows Performance Toolkit | Installed and available | No measurement session was run. |
| Windows Performance Analyzer (`wpa.exe`) | 11.7.383.0 | Windows Performance Toolkit | Installed and available | No trace was opened. |
| `xperf.exe` | 10.0.26100.4188 | Windows Performance Toolkit | Installed and available | No trace was collected. |
| `inspect.exe` | Not found through PATH query | Not observed | Not found | Accessibility inspection remains blocked. |
| Accessibility Insights | Not found through command query | Not observed | Not found | No Accessibility inspection tool was selected or installed. |
| `dotnet-trace` | Not found through PATH query | Not observed | Not found | WPT remains the only observed diagnostic candidate. |
| `dotnet-counters` | Not found through PATH query | Not observed | Not found | No runtime measurement was executed. |

## 9. Experimental Version Candidate Baseline

官方版本與本機版本分開記錄。以下官方版本是 2026-07-26 查核時可由官方來源取得的候選；候選用途為未來 Spike，不是產品選型。

| Baseline ID | Technology | Official stable version | Local availability | Proposed spike version | Evidence source/date | Status |
| --- | --- | --- | --- | --- | --- | --- |
| ENV-BASE-001 | Windows App SDK／WinUI 3 | Windows App SDK 2.3.1, released 2026-07-16 | Unknown; no project/package inspection performed | 2.3.1, experimental use only | [Windows App SDK downloads](https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/downloads), 2026-07-26 | Partially verified |
| ENV-BASE-002 | .NET SDK／Runtime | .NET 10.0.10; SDK 10.0.302, released 2026-07-14 | Installed: SDK 10.0.302 and Runtime 10.0.10 | 10.0.302 / 10.0.10, experimental use only | [.NET 10 downloads](https://dotnet.microsoft.com/en-us/download/dotnet/10.0), 2026-07-26; local `dotnet` output | Verified |
| ENV-BASE-003 | WPF | Windows Desktop Runtime 10.0.10 candidate | Installed: `Microsoft.WindowsDesktop.App 10.0.10`; WPF behavior not runtime tested | .NET 10.0.10, experimental use only | [.NET 10 downloads](https://dotnet.microsoft.com/en-us/download/dotnet/10.0), 2026-07-26; local `dotnet` output | Partially verified |
| ENV-BASE-004 | Windows SDK | 10.0.28000.2270, June 2026 release | Installed: 10.0.22621.0 and 10.0.26100.0 | 10.0.26100.0, experimental use only | [Windows SDK release notes](https://learn.microsoft.com/en-us/windows/apps/windows-sdk/release-notes), 2026-07-26; local Include directory | Partially verified |
| ENV-BASE-005 | Visual Studio／Build Tools | Visual Studio 2026 stable channel; release history identifies 18.8.0 | No instance found in queried PATH and Setup Registry locations | None; blocked pending local installation evidence | [Visual Studio release history](https://learn.microsoft.com/en-us/visualstudio/releases/2026/release-history), 2026-07-26 | Unavailable |
| ENV-BASE-006 | Accessibility inspection | Official Windows accessibility testing guidance; exact tool not selected | `inspect.exe` and Accessibility Insights command queries found no tool | None; blocked pending tool evidence | [Accessibility testing](https://learn.microsoft.com/en-us/windows/apps/design/accessibility/accessibility-testing), 2026-07-26 | Unknown |
| ENV-BASE-007 | Diagnostic／measurement | Windows Performance Toolkit candidate | WPR 10.0.26100.4188, WPA 11.7.383.0 and xperf 10.0.26100.4188 observed | WPR/WPA/xperf observed versions, experimental use only | [Windows Performance Toolkit](https://learn.microsoft.com/en-us/windows-hardware/test/wpt/), 2026-07-26; local command query | Verified |

`ENV-BASE-001`、`ENV-BASE-005` 與 `ENV-BASE-006` 仍不能解除 `RESEARCH-TECH-UI-003` 的版本或工具 Blocker。官方穩定版本存在不代表本機已安裝，也不代表可以開始 Spike。

## 10. Display Environment Inventory

目前已確認有 3 個 active monitor records，但沒有取得每一個顯示器的完整桌面解析度、Per-Monitor DPI 或 HDR state。此限制不得被解讀為環境不存在。

| Display ID | Resolution | Scaling | HDR | Position | Primary | Evidence status |
| --- | --- | --- | --- | --- | --- | --- |
| DISPLAY-001 | Unknown; GPU reports 2560 × 1440 as a video mode | Unknown | Unknown | Unknown | Unknown | Partially verified |
| DISPLAY-002 | Unknown | Unknown | Unknown | Unknown | Unknown | Partially verified |
| DISPLAY-003 | Unknown | Unknown | Unknown | Unknown | Unknown | Partially verified |

環境判定：

- 單螢幕基線：`Partially verified`；目前有 3 個 active displays，但沒有以單螢幕模式建立環境證據。
- 多螢幕相同 DPI：`Unknown`；有 3 個 active displays，但 DPI 未取得。
- 多螢幕異質 DPI：`Unknown`；沒有 per-monitor DPI 證據。
- HDR：`Unknown`；沒有 HDR capability/state 證據。
- 不同解析度組合：`Unknown`；目前查詢只確認一個 GPU video mode，沒有完成逐顯示器映射。

## 11. Architecture and Packaging Availability

下表是能力的環境盤點，不是部署或 Runtime Test 結果。

| Capability | Environment available | Evidence | Status | Affected Spikes |
| --- | --- | --- | --- | --- |
| x64 execution | Yes; current host is x64 and `.NET` RID is `win-x64` | Windows system inventory and `dotnet --info` | Verified | UI-SPIKE-001–011 |
| ARM64 execution | No ARM64 device evidence in this inspection; current host is x64 | `Win32_ComputerSystem.SystemType` | Unavailable | UI-SPIKE-010, UI-SPIKE-011 |
| Packaged application testing | Not verified; no Project or deployment artifact exists | No packaging test performed | Unknown | UI-SPIKE-011 |
| Unpackaged application testing | Not verified; no Project or startup artifact exists | No unpackaged test performed | Unknown | UI-SPIKE-011 |
| Clean-machine deployment testing | Not available from current single-host inspection | No clean-machine evidence | Unavailable | UI-SPIKE-011 |
| Administrator testing | Current user is not Administrator | `WindowsPrincipal.IsInRole(Administrator)` returned false | Verified for standard-user context | UI-SPIKE-001–011 |
| Standard-user testing | Current user is in standard-user context | Same read-only identity query | Verified for current context only | UI-SPIKE-001–011 |

模擬器、虛擬機或目前這台 x64 主機不得自動視為 ARM64、clean-machine 或 packaged deployment 的等價環境。

## 12. Evidence Tool Inventory

本節只記錄證據工具準備度，不建立或保存實際 Spike 證據。

| Evidence type | Tool | Installed/version | Usable now | Limitation | Status |
| --- | --- | --- | --- | --- | --- |
| Screenshot | TBD; future evidence only | Not selected | Unknown | No product screenshot functionality is being implemented. | Unknown |
| Screen recording | TBD; future evidence only | Not selected | Unknown | No recording was made. | Unknown |
| Diagnostic logging | WPR／ETW candidate | WPR 10.0.26100.4188 | Yes for future read-only plan, not executed | Collection profile and metadata rules are not fixed. | Partially verified |
| Timing measurement | WPA／xperf candidate | WPA 11.7.383.0; xperf 10.0.26100.4188 | Yes for future plan, not executed | No timing workload or threshold is defined. | Partially verified |
| Environment recording | PowerShell 7.6.3, CIM and Registry read-only queries | Installed and available | Yes for inventory | Per-monitor DPI and HDR were not obtained by the selected queries. | Partially verified |
| Accessibility inspection | `inspect.exe` or Accessibility Insights candidate | Not found in command query | No evidence of usable tool | Tool and inspection procedure are not selected. | Unavailable |
| Deployment inspection | Visual Studio／Build Tools candidate | Not found in queried PATH and Setup Registry | No | No Project, packaging artifact or deployment path exists. | Unavailable |
| Failure reproduction | Controlled future result record | No prototype | No | No failure workload exists in this documentation-only task. | Blocked |

## 13. Environment Gap Register

每筆 Gap 只能在補上對應證據後標示 `Resolved`。目前所有 Gap 均為 `Open`。

| Gap ID | Description | Evidence | Affected prerequisite | Affected blocker | Affected spikes | Severity | Resolution option | Current status | Notes |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| UI-ENV-GAP-001 | Per-monitor DPI scaling not verified | LogPixels empty; no per-monitor DPI record | UI-PREQ-004, UI-PREQ-005 | UI-BLOCK-003 | UI-SPIKE-003, 004, 007–009 | Blocking | Obtain a repeatable per-monitor DPI record | Open | Do not infer DPI from physical size. |
| UI-ENV-GAP-002 | Per-display desktop resolution mapping incomplete | Only one GPU video mode observed | UI-PREQ-004, UI-PREQ-005 | UI-BLOCK-003 | UI-SPIKE-003, 004, 008, 009 | Blocking | Obtain per-display mode evidence | Open | Current active-display count is verified. |
| UI-ENV-GAP-003 | HDR state and capability unknown | No HDR evidence captured | UI-PREQ-007 | UI-BLOCK-005 | UI-SPIKE-003, 004, 011 | Non-blocking for non-HDR cases | Record On／Off／Not available per display | Open | HDR-specific coverage remains blocked. |
| UI-ENV-GAP-004 | ARM64 device unavailable in current inspection | Current host is x64; no ARM64 device record | UI-PREQ-006 | UI-BLOCK-004 | UI-SPIKE-010, 011 | Blocking | Provide a real ARM64 device or explicitly defer | Open | x64 cannot substitute for ARM64. |
| UI-ENV-GAP-005 | Packaged and unpackaged paths not verified | No Project or deployment artifact | UI-PREQ-008 | UI-BLOCK-006 | UI-SPIKE-011 | Blocking | Record controlled packaged and unpackaged capability | Open | No installation or project creation allowed here. |
| UI-ENV-GAP-006 | WinUI 3／Windows App SDK local availability unknown | No .NET workload; no package inspection | UI-PREQ-001 | UI-BLOCK-001 | UI-SPIKE-001–011 | Blocking | Verify local candidate package/workload without installing | Open | Official 2.3.1 is not local availability evidence. |
| UI-ENV-GAP-007 | Visual Studio／Build Tools not observed | PATH and queried Setup Registry had no instance | UI-PREQ-001, UI-PREQ-011 | UI-BLOCK-001, UI-BLOCK-007 | UI-SPIKE-001–011 | Blocking | Provide existing installation evidence or defer tool-dependent work | Open | Do not install during this task. |
| UI-ENV-GAP-008 | Accessibility inspection tool not observed | `inspect.exe` and Accessibility Insights command queries not found | UI-PREQ-010 | UI-BLOCK-007 | UI-SPIKE-010, 011 | Blocking | Provide an existing inspection tool and version | Open | No accessibility test was run. |
| UI-ENV-GAP-009 | Evidence storage and naming not operationally verified | Rules exist in RESEARCH-TECH-UI-003; no result directory exists | UI-PREQ-012 | UI-BLOCK-007 | UI-SPIKE-001–011 | Blocking | Review storage boundary and metadata rule | Open | No result artifact is created here. |
| UI-ENV-GAP-010 | Safety and cleanup not runtime verified | Documentation exists; no Overlay or Focus test | UI-PREQ-013 | UI-BLOCK-008 | UI-SPIKE-001–011 | Blocking | Review safety checklist before any execution | Open | Documentation cannot substitute runtime evidence. |
| UI-ENV-GAP-011 | Independent execution authorization absent | Parent record remains Draft and Not ready | UI-PREQ-014 | UI-BLOCK-009 | UI-SPIKE-001–011 | Blocking | Complete independent Review and authorization | Open | This document cannot self-authorize. |

## 14. Prerequisite and Blocker Impact Mapping

以下只提出對 `RESEARCH-TECH-UI-003` 的 status recommendation，不直接修改該文件。

| UI-PREQ／UI-BLOCK ID | Evidence found | Resulting status recommendation | Reason |
| --- | --- | --- | --- |
| UI-PREQ-001 | .NET and Windows SDK versions verified; WinUI／Windows App SDK and Build Tool local availability incomplete | Keep `Blocked` | Candidate version and local availability are not both complete. |
| UI-PREQ-002 | x64 Windows 11 build, CPU, GPU and RAM verified; display detail incomplete | Keep `Blocked` | The complete comparable baseline is not reproducible yet. |
| UI-PREQ-003 | Equivalent behavior is documented in RESEARCH-TECH-UI-003 | Keep `Blocked` | No equivalent Prototype behavior has been runtime verified. |
| UI-PREQ-004 | Three active displays observed; per-monitor DPI and resolution mapping unknown | Keep `Blocked` | DPI matrix is incomplete. |
| UI-PREQ-005 | Three active display records observed; heterogeneous DPI not proven | Keep `Blocked` | Monitor count is not evidence of heterogeneous DPI. |
| UI-PREQ-006 | Current host x64; no ARM64 device evidence | Recommend `Deferred` | ARM64 is not available in this inspection and requires separate scope. |
| UI-PREQ-007 | HDR state not obtained | Keep `Blocked` | HDR-specific tests cannot be authorized without display evidence. |
| UI-PREQ-008 | No packaged or unpackaged artifact path verified | Keep `Blocked` | No deployment capability evidence exists. |
| UI-PREQ-009 | Synthetic input is specified but not instantiated | Keep `Blocked` | No Prototype or input artifact exists. |
| UI-PREQ-010 | Accessibility tool not found in command query | Keep `Blocked` | Tool and version are not available for inspection. |
| UI-PREQ-011 | WPR/WPA/xperf observed; measurement procedure not fixed | Keep `Blocked` | Tool presence does not establish a comparable measurement method. |
| UI-PREQ-012 | Naming and storage rules documented; no result directory created | Keep `Blocked` | Operational evidence boundary is not reviewed. |
| UI-PREQ-013 | Safety and cleanup rules documented; no runtime verification | Keep `Blocked` | Documentation does not prove cleanup behavior. |
| UI-PREQ-014 | Parent readiness record is Draft and Not ready | Keep `Blocked` | No independent execution authorization exists. |
| UI-BLOCK-001 | Official candidates and local .NET/SDK data are partially available | Keep `Open` | WinUI／Build Tool local evidence is incomplete. |
| UI-BLOCK-002 | x64 host and Windows build verified | Keep `Open` | Complete display and reproducibility record is incomplete. |
| UI-BLOCK-003 | Three active displays verified; DPI and per-display modes unknown | Keep `Open` | DPI comparison cannot be performed. |
| UI-BLOCK-004 | No ARM64 device evidence | Keep `Open` or `Deferred` | A separate ARM64 environment is required. |
| UI-BLOCK-005 | WPT tools available; HDR unknown | Keep `Open` | Measurement method and HDR state remain incomplete. |
| UI-BLOCK-006 | Packaging not verified | Keep `Open` | No packaged/unpackaged test path exists. |
| UI-BLOCK-007 | WPT available; Accessibility and evidence storage incomplete | Keep `Open` | Required evidence chain is incomplete. |
| UI-BLOCK-008 | Safety rules documented only | Keep `Open` | No runtime cleanup evidence exists. |
| UI-BLOCK-009 | Independent Review and authorization absent | Keep `Open` | Environment inspection cannot authorize Spike execution. |

## 15. Phase 1 Environment Sufficiency

判定：`Partially sufficient`

已具備的環境證據：

- Windows 11 Pro build 26200。
- x64 host、AMD Ryzen 7 5800X、NVIDIA RTX 2070 SUPER。
- 3 個 active display records。
- .NET SDK 10.0.302。
- Windows Desktop Runtime 10.0.10。
- Windows SDK 10.0.22621.0 與 10.0.26100.0。
- WPR、WPA 與 xperf 可見。

仍不足以重新評估為 `Ready` 的項目：

- WinUI 3／Windows App SDK 本機可用性。
- Visual Studio／Build Tools 可用性。
- Per-monitor DPI 與逐顯示器解析度。
- HDR 狀態。
- 等價 Prototype 行為與合成輸入。
- Safety cleanup 的 runtime evidence。
- `RESEARCH-TECH-UI-003` Review 與獨立 execution authorization。

`Partially sufficient` 只代表環境證據已取得一部分，不代表 Phase 1 Ready、不代表 Spike 授權，也不代表 WinUI 3 或 WPF 通過。

## 16. Traceability

### Repository references

- [UI Framework Feasibility](01-ui-framework-feasibility.md)
- [UI Framework Runtime Spike Plan](02-ui-framework-runtime-spike-plan.md)
- [UI Framework Runtime Spike Execution Readiness](03-ui-framework-runtime-spike-execution-readiness.md)
- [ADR-0002: UI Framework Selection](../../../Architecture/adr/ADR-0002-ui-framework-selection.md)
- [Technology Decision Roadmap](../../../Architecture/TECHNOLOGY-DECISION-ROADMAP.md)

### Traceability chain

`Observed environment → Evidence → UI-PREQ / UI-BLOCK → UI-SPIKE → Phase readiness reassessment`

本文件的盤點結果只能提供後續 Review 使用，不自動修改 `RESEARCH-TECH-UI-003`、`ADR-0002`、PRD、Specs 或 Architecture。

### Official sources

- [Latest Windows App SDK downloads](https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/downloads)
- [.NET 10 downloads](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- [Windows SDK release notes](https://learn.microsoft.com/en-us/windows/apps/windows-sdk/release-notes)
- [Visual Studio release history](https://learn.microsoft.com/en-us/visualstudio/releases/2026/release-history)
- [Windows Performance Toolkit](https://learn.microsoft.com/en-us/windows-hardware/test/wpt/)
- [Windows accessibility testing](https://learn.microsoft.com/en-us/windows/apps/design/accessibility/accessibility-testing)

## 17. Completion Boundary

完成本文件不代表：

- 前置條件已全部解除。
- Phase 1 已 Ready。
- 可以執行 Spike。
- Prototype 已建立。
- Framework 已選擇。
- `ADR-0002` 可以 Accepted。
- 產品 Runtime 或 SDK 已決定。

### 允許的最小同步更新

主要交付物：

- `docs/Research/Technology/04-ui-framework-runtime-environment-baseline.md`

允許的索引更新：

- `docs/Research/Technology/README.md`
- `docs/Research/README.md`
- `docs/index.md`
- `CHANGELOG.md`
- `TODO.md`

同步更新只能新增文件連結、Draft 狀態與待 Review 項目，不得新增產品決策或直接解除父文件的 Blocker。

### Prohibited actions for this task

- 不得安裝、移除或升級工具。
- 不得執行 Build、Restore、Publish、Runtime Spike、Performance Test、Accessibility Test 或 Deployment Test。
- 不得建立 Project、Prototype、Result directory、Result Artifact 或 Source Code。
- 不得修改 `RESEARCH-TECH-UI-003`、`ADR-0002`、PRD、Specs 或 Architecture。
- 不得開始 Rendering／Capture ADR。
- 不得開始正式 Coding。

