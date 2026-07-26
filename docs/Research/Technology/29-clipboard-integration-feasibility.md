# Clipboard Integration Feasibility

| Field | Value |
|---|---|
| Document ID | RESEARCH-TECH-CLIPBOARD-001 |
| Title | Clipboard Integration Feasibility |
| Status | Draft |
| Research Type | Technology Feasibility / Official Evidence Baseline |
| Technology Decision | `TD-004` — Clipboard Integration；依 `Architecture/TECHNOLOGY-DECISION-ROADMAP.md` 原樣引用，目前為 `Candidate` |
| Technology Decision Dependencies | `COMP-009`、`COMP-015`、`TD-001` |
| UI Framework Decision | Unresolved — ADR-0002 remains Draft |
| Rendering Decision | Not made |
| Capture Decision | Not made |
| Clipboard Decision | Not made |
| Build Verification | Not performed |
| Runtime Verification | Not performed |
| Clipboard Execution Authorized | No |
| Evidence Artifact Creation | Not performed |
| Owner | TBD |
| Last reviewed | Not reviewed |
| Official source access date | 2026-07-26 |

## 1. Purpose

本文件只回答：

> SnipPlus 在完成截圖後，應如何將最終影像可靠地寫入 Windows Clipboard；各候選技術在格式相容性、Alpha／Color fidelity、執行緒模型、Clipboard contention、資料生命週期、History／Cloud Clipboard、封裝模式與 Host interoperability 上有哪些官方支援及未知風險？

本文件是 feasibility 與 official evidence baseline，不是 Clipboard technology selection。Clipboard 與 File Output 必須維持平行且互相獨立：

- Clipboard 成功不得依賴 Save PNG 成功。
- Save PNG 成功不得依賴 Clipboard 成功。
- 任一輸出失敗不得修改已完成的 Annotation／Selection state。
- Clipboard component 不得接管 Shared Workflow State。

## 2. Scope

只研究：

- Windows 11 Clipboard 與 Windows desktop application context。
- 完成後影像的 Clipboard 寫入與 abstract handoff boundary。
- Bitmap、DIB、DIBV5、PNG registered format 與自訂 Clipboard format。
- 多格式同時提供、Alpha channel、Pixel format 與 color metadata。
- STA／COM／Dispatcher requirement、ownership、lifetime、delayed/immediate rendering。
- Clipboard contention、retry、timeout、failure、cleanup 與 application shutdown。
- Clipboard History、Cloud Clipboard、privacy、cross-process exposure 與 size limitation。
- Packaged／unpackaged desktop app 的待驗證影響。
- WinUI 3／WPF host interoperability 的候選路徑，不選定任何 Host。
- Clipboard 與 workflow、capture、rendering、file-output 的責任分界。

## 3. Non-goals

不得：

- 寫入、讀取或清除目前 Windows Clipboard。
- 呼叫任何 Clipboard API、建立 Bitmap、PNG、DIB 或 Clipboard payload。
- 建立 Project、Solution、Prototype、Source Code、Result 或 Evidence artifact。
- 執行 Restore、Build、Run、Publish、Test 或 Runtime Spike。
- 修改 Capture／Rendering／UI Research Line 或 ADR-0002。
- 建立 Clipboard ADR、選擇 Clipboard Technology 或形成 Candidate ranking。
- 研究 Save PNG、檔案路徑、檔名策略或 File Output implementation。
- 開始正式 Clipboard 功能或截圖功能。

## 4. Candidate Strategies

| ID | Candidate |
|---|---|
| CLIP-OPT-001 | WPF `System.Windows.Clipboard`／`IDataObject` |
| CLIP-OPT-002 | WinRT `Windows.ApplicationModel.DataTransfer.Clipboard`／`DataPackage` |
| CLIP-OPT-003 | Win32 OLE Clipboard／COM `IDataObject` |
| CLIP-OPT-004 | Win32 Raw Clipboard APIs |
| CLIP-OPT-005 | Host-neutral Clipboard abstraction with framework-specific adapters |

候選規則：

- `CLIP-OPT-005` 是架構策略，不是單一 Windows API。
- 不得因 WPF 或 WinUI API 較容易呼叫便直接選定。
- Framework wrapper 與底層 Windows Clipboard 行為必須分開。
- Raw Win32、OLE Clipboard 及 WinRT `DataPackage` 不得混成同一 Candidate。
- 舊 PRD、Spec 或 Research 中的技術偏好不得視為正式決策。
- Candidate identity、official support、product suitability 與 runtime verification 必須分開記錄。

## 5. Repository and Decision Context

| Source | Exact repository meaning | Effect on this research |
|---|---|---|
| `Architecture/TECHNOLOGY-DECISION-ROADMAP.md` | `TD-004 Clipboard Integration`；Priority `P0`；Depends On `COMP-009`、`COMP-015`、`TD-001`；Status `Candidate` | 本文件不能把 `TD-004` 升級為 `Ready`、`ADR` 或 `Accepted` |
| `PRD/PRD-0005-functional-requirements.md` | `FR-007`：使用者能將完成的 capture result 交付至 clipboard | 保留產品能力，不決定 API 或格式 |
| `Specs/SPEC-0007-clipboard-handoff.md` | `FEAT-003` 只定義 Capture Result → Clipboard Ready → abstract Clipboard Consumer | 本文件只補 technology research，不改 handoff contract |
| `Specs/SPEC-0010-feature-integration.md` | Clipboard 與 Output 是平行 downstream path | Clipboard 不得成為 File Output 的必要前置條件 |
| `Architecture/ARCH-0001-architecture-principles.md` | Shared State single source、Feature boundary 與 technology deferral | 不建立第二套 workflow state 或提前選 API |
| `Architecture/ARCH-0002-layer-model.md` | Platform Integration Layer 隔離 OS/Clipboard/Output side effect | Clipboard API 只能位於 platform boundary |
| `Architecture/ARCH-0003-module-catalog.md` | `MOD-005` owns handoff semantics；`MOD-009` owns platform Clipboard boundary | Product semantics 與 API/format 分離 |
| `Architecture/ARCH-0004-component-boundaries.md` | `COMP-009` handoff boundary；`COMP-015` platform adapter；Shared State only `COMP-001` | Clipboard 不直接修改 Shared State |
| `Architecture/ARCH-0005-component-interactions.md` | `INT-012` submits handoff；`INT-013` requests platform Clipboard delivery | 研究不擴張 interaction ownership |
| `Architecture/adr/ADR-0002-ui-framework-selection.md` | UI framework decision remains Draft/unresolved | WPF/WinUI pair 只能並列研究 |

## 6. Source Acceptance Policy

主要證據只能使用：

- Microsoft Learn。
- .NET 官方 API Reference。
- Windows SDK 官方 API Reference。
- Windows App SDK／WinRT 官方文件。
- Microsoft 官方 Sample 或 Repository。
- Microsoft 官方 Clipboard format、History 與 platform behavior 文件。

第三方來源只能標示為 `Informative`，不得單獨關閉 prerequisite、Gate 或 Gap。每筆 official evidence 至少記錄：

`Evidence ID`、`Claim`、`Candidate`、`Host`、`Official source title`、`Publisher`、`URL`、`Publication/update date`、`Access date`、`API/format identity`、`Supported platform`、`Threading/COM requirement`、`Packaging context`、`Limitation`、`Decision implication`、`Runtime verification still required`。

## 7. Controlled Vocabulary

### 7.1 Claim Status

只能使用：

- `Confirmed by official source`
- `Partially confirmed`
- `Conflicting official evidence`
- `Unknown`
- `Not applicable`

### 7.2 Candidate Support Status

只能使用：

- `Officially documented`
- `API available, product suitability unverified`
- `Requires documented native interop`
- `Requires runtime prototype`
- `Not aligned by official evidence`
- `Unknown`

### 7.3 Gate Status

只能使用：

- `Satisfied by documentation`
- `Partially satisfied`
- `Requires runtime prototype`
- `Unsatisfied`
- `Not evaluated`

不得把下列字詞當成研究結論：`Best`、`Winner`、`Recommended`、`Production ready`、`Reliable enough`、`Should work`。

## 8. Official Evidence Baseline

所有 URL 均為官方來源；本節只記錄文件 claim，不代表 SnipPlus 的 local availability、Build 或 Runtime 已驗證。

| Evidence ID | Claim | Candidate | Host | Official source title | Publisher | URL | Publication/update date | Access date | API/format identity | Supported platform | Threading/COM requirement | Packaging context | Limitation | Decision implication | Runtime verification still required |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| CLIP-EVID-001 | WPF `Clipboard` exposes `SetImage`, `SetData`, `SetDataObject` and shares the system Clipboard | CLIP-OPT-001 | WPF | Clipboard Class | Microsoft Learn | [System.Windows.Clipboard](https://learn.microsoft.com/en-us/dotnet/api/system.windows.clipboard?view=windowsdesktop-10.0) | Not stated | 2026-07-26 | `System.Windows.Clipboard`; `PresentationCore.dll` | Windows desktop/WPF | UI/STA behavior for this product remains to be verified | Desktop WPF | API surface does not establish format fidelity or consumer acceptance | WPF wrapper remains a documented candidate identity | Yes |
| CLIP-EVID-002 | WPF `SetDataObject(Object, Boolean)` distinguishes whether data remains after application exit | CLIP-OPT-001 | WPF | Clipboard.SetDataObject Method | Microsoft Learn | [Clipboard.SetDataObject](https://learn.microsoft.com/en-us/dotnet/api/system.windows.clipboard.setdataobject?view=windowsdesktop-10.0) | Not stated | 2026-07-26 | `SetDataObject(Object, Boolean)` | Windows desktop/WPF | Lifetime semantics are documented; product shutdown path is not tested | Desktop WPF | Persistence flag is not a privacy or History policy | Lifetime must be evaluated separately from format publication | Yes |
| CLIP-EVID-003 | WPF `DataObject`/`IDataObject` participates in Clipboard and drag-and-drop data transfer | CLIP-OPT-001 | WPF | DataObject Class | Microsoft Learn | [System.Windows.DataObject](https://learn.microsoft.com/en-us/dotnet/api/system.windows.dataobject?view=windowsdesktop-10.0) | Not stated | 2026-07-26 | `System.Windows.DataObject`; `IDataObject` | Windows desktop/WPF | COM/native interop boundary remains separate | Desktop WPF | Does not prove multi-format image fidelity in SnipPlus | WPF multi-format path remains unclosed | Yes |
| CLIP-EVID-004 | WinRT `DataPackage` supports Bitmap plus text/RTF/HTML/StorageItems and custom formats | CLIP-OPT-002 | WinRT/WinUI host | DataPackage Class | Microsoft Learn | [DataPackage](https://learn.microsoft.com/en-us/uwp/api/windows.applicationmodel.datatransfer.datapackage?view=winrt-28000) | Not stated | 2026-07-26 | `DataPackage`; `Windows.ApplicationModel.DataTransfer` | Windows Runtime; desktop host use remains contextual | DataPackage is documented as agile; host dispatcher route remains to be verified | Packaged/unpackaged effect not closed | Supported default/custom formats do not define PNG/Alpha fidelity | WinRT candidate identity is documented; product suitability remains unverified | Yes |
| CLIP-EVID-005 | `Clipboard.SetContent(DataPackage)` sets the current Clipboard content | CLIP-OPT-002 | WinRT/WinUI host | Clipboard.SetContent(DataPackage) Method | Microsoft Learn | [Clipboard.SetContent](https://learn.microsoft.com/en-us/uwp/api/windows.applicationmodel.datatransfer.clipboard.setcontent?view=winrt-28000) | Not stated | 2026-07-26 | `Clipboard.SetContent(DataPackage)` | Windows Runtime | Invocation/threading and desktop host projection require validation | Packaged/unpackaged effect not closed | Official API availability is not a runtime integration result | WinRT publication route remains candidate evidence | Yes |
| CLIP-EVID-006 | `DataPackage.SetBitmap` accepts `RandomAccessStreamReference` for the bitmap representation | CLIP-OPT-002 | WinRT/WinUI host | DataPackage.SetBitmap Method | Microsoft Learn | [DataPackage.SetBitmap](https://learn.microsoft.com/en-us/uwp/api/windows.applicationmodel.datatransfer.datapackage.setbitmap?view=winrt-28000) | Not stated | 2026-07-26 | `SetBitmap(RandomAccessStreamReference)` | Windows Runtime | Stream lifetime and consumer behavior require runtime verification | Packaged/unpackaged effect not closed | Bitmap contract is not a claim about PNG byte publication | Bitmap route needs format and interop spikes | Yes |
| CLIP-EVID-007 | `DataPackage.SetData` supports custom format identity but source/target must know the format | CLIP-OPT-002 | WinRT/WinUI host | DataPackage.SetData(String, Object) Method | Microsoft Learn | [DataPackage.SetData](https://learn.microsoft.com/en-us/uwp/api/windows.applicationmodel.datatransfer.datapackage.setdata?view=winrt-28000) | Not stated | 2026-07-26 | `SetData(formatId, value)` | Windows Runtime | Host projection and custom format lifetime require validation | Packaged/unpackaged effect not closed | Custom format interoperability remains a Gap until consumer evidence exists | Yes |
| CLIP-EVID-008 | All applications can access the system Clipboard; standard and registered formats are distinct | CLIP-OPT-003/004 | Win32 desktop | About the Clipboard | Microsoft Learn | [About the Clipboard](https://learn.microsoft.com/en-us/windows/win32/dataxchg/about-the-clipboard) | 2021-05-26 | 2026-07-26 | `Winuser.h`; standard/registered formats | Windows desktop | Native window/Clipboard transaction boundary | Desktop | User-driven and cross-process nature creates privacy boundary | No API can be called “private” merely because the source is SnipPlus | Yes |
| CLIP-EVID-009 | Only one window can have the Clipboard open; publication, ownership, delayed rendering and memory transfer are distinct | CLIP-OPT-004 | Win32 desktop | Clipboard Operations | Microsoft Learn | [Clipboard Operations](https://learn.microsoft.com/en-us/windows/win32/dataxchg/clipboard-operations) | 2022-05-26 | 2026-07-26 | `OpenClipboard`, `EmptyClipboard`, `SetClipboardData`, `CloseClipboard` | Windows desktop | Window message and owner lifetime affect delayed rendering | Desktop | Contention and cleanup are not solved by format identity | Contention/retry and ownership need runtime evidence | Yes |
| CLIP-EVID-010 | Windows supports multiple formats, synthesized conversions, DIB/DIBV5 behavior and History/Cloud control formats | CLIP-OPT-004 | Win32 desktop | Clipboard Formats | Microsoft Learn | [Clipboard Formats](https://learn.microsoft.com/en-us/windows/win32/dataxchg/clipboard-formats) | Not stated | 2026-07-26 | standard, registered, private, synthesized formats | Windows desktop | Native publication/lifetime rules apply | Desktop | Conversion does not prove Alpha/color fidelity for every consumer | Format matrix must keep conversion and fidelity separate | Yes |
| CLIP-EVID-011 | `CF_BITMAP`, `CF_DIB` and `CF_DIBV5` have different payload identities | CLIP-OPT-004 | Win32 desktop | Standard Clipboard Formats | Microsoft Learn | [Standard Clipboard Formats](https://learn.microsoft.com/en-us/windows/win32/dataxchg/standard-clipboard-formats) | 2020-12-11 | 2026-07-26 | `CF_BITMAP`=2, `CF_DIB`=8, `CF_DIBV5`=17 | Windows desktop | GDI handle versus movable memory object boundary | Desktop | Standard format identity does not prove Alpha preservation | DIB/DIBV5 behavior needs synthetic runtime validation | Yes |
| CLIP-EVID-012 | `OleSetClipboard` stores an `IDataObject`, uses delayed rendering, retains a reference and exposes failure HRESULTs | CLIP-OPT-003 | Win32 OLE/COM | OleSetClipboard function | Microsoft Learn | [OleSetClipboard](https://learn.microsoft.com/en-us/windows/win32/api/ole2/nf-ole2-olesetclipboard) | 2021-10-13 | 2026-07-26 | `OleSetClipboard`, `OleFlushClipboard`, `IDataObject` | Windows desktop | OLE/COM apartment and message behavior | Desktop | Delayed rendering and process lifetime are coupled | Ownership/lifetime spike is required | Yes |
| CLIP-EVID-013 | COM distinguishes STA and MTA; UI/message-loop threads are the documented STA use case | CLIP-OPT-003 | WPF/WinUI/native host | Processes, Threads, and Apartments | Microsoft Learn | [Processes, Threads, and Apartments](https://learn.microsoft.com/en-us/windows/win32/com/processes--threads--and-apartments) | 2026-07-17 | 2026-07-26 | `COINIT_APARTMENTTHREADED`, `COINIT_MULTITHREADED` | Windows desktop | STA message pump and cross-apartment marshaling matter | Host-specific dispatcher route remains open | COM model is not a product retry policy | Yes |
| CLIP-EVID-014 | STA objects must be called on their owning thread and the thread must retrieve/dispatch messages | CLIP-OPT-003 | WPF/WinUI/native host | Single-Threaded Apartments | Microsoft Learn | [Single-Threaded Apartments](https://learn.microsoft.com/en-us/windows/win32/com/single-threaded-apartments) | Not stated | 2026-07-26 | STA apartment/message loop | Windows desktop | Cross-thread direct calls are not valid for apartment objects | Host dispatcher and shutdown behavior remain unknown | Threading contract must be verified in each Host pair | Yes |
| CLIP-EVID-015 | .NET WinForms exposes multi-format `SetData` and reports `ExternalException` when another process uses Clipboard | CLIP-OPT-005 | Desktop host reference | Clipboard.SetData Method | Microsoft Learn | [System.Windows.Forms.Clipboard.SetData](https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.clipboard.setdata?view=windowsdesktop-10.0) | Not stated | 2026-07-26 | `Clipboard.SetData(String, Object)` | Windows desktop | WinForms wrapper is evidence for a possible adapter pattern, not a WPF/WinUI decision | Desktop | WinForms is not one of the selected Host decisions | Error and multi-format behavior require host-specific validation | Yes |
| CLIP-EVID-016 | Microsoft documents 4 MB per Clipboard History item and Bitmap as a supported history format | CLIP-OPT-001..005 | Windows 11 | Using the clipboard | Microsoft Support | [Using the clipboard](https://support.microsoft.com/en-us/windows/apps/using-the-clipboard) | Not stated | 2026-07-26 | Clipboard History; Bitmap; 4 MB/item | Windows 11 | Not an API threading contract | User setting/account dependent | Support guidance does not prove all image/registered-format cases | Size/privacy constraints need separate product and runtime review | Yes |
| CLIP-EVID-017 | Clipboard History is limited to 25 copied entries and can sync across devices when enabled | CLIP-OPT-001..005 | Windows 11 | Using the clipboard | Microsoft Support | [Using the clipboard](https://support.microsoft.com/en-us/windows/apps/using-the-clipboard) | Not stated | 2026-07-26 | History count; cloud sync | Windows 11 | Not an API threading contract | Microsoft/work account and settings dependent | App cannot assume local-only retention | Privacy boundary must remain explicit | Yes |
| CLIP-EVID-018 | System format registration is required for application-defined registered Clipboard formats | CLIP-OPT-004 | Win32 desktop | Clipboard Formats | Microsoft Learn | [Clipboard Formats](https://learn.microsoft.com/en-us/windows/win32/dataxchg/clipboard-formats) | Not stated | 2026-07-26 | `RegisterClipboardFormat`, `GetClipboardFormatName` | Windows desktop | Native transaction boundary | Desktop | Registered identity does not guarantee consumer support | PNG/custom format path remains unselected | Yes |

## 9. Evaluation Criteria

建立正好 `CLIP-001..022`；產品門檻未知時維持 `TBD`，不得自行制定 Retry 次數、Retry 間隔、Timeout、最大影像尺寸、最大記憶體或 Alpha／color 差異門檻。

| ID | Criterion | Official evidence basis | Current claim status | Runtime verification still required |
|---|---|---|---|---|
| CLIP-001 | Windows 11 availability | CLIP-EVID-008、016 | Confirmed by official source | Yes；local OS and runtime context |
| CLIP-002 | WPF host compatibility | CLIP-EVID-001、002、003 | Partially confirmed | Yes |
| CLIP-003 | WinUI 3 host compatibility | CLIP-EVID-004、005、006、007 | Partially confirmed | Yes |
| CLIP-004 | Packaged desktop compatibility | CLIP-EVID-004、005 | Unknown | Yes |
| CLIP-005 | Unpackaged desktop compatibility | CLIP-EVID-004、005、012 | Unknown | Yes |
| CLIP-006 | Bitmap Clipboard interoperability | CLIP-EVID-001、005、006、011 | Partially confirmed | Yes |
| CLIP-007 | DIB／DIBV5 support | CLIP-EVID-009、010、011 | Confirmed by official source | Yes；consumer and fidelity |
| CLIP-008 | PNG format support | CLIP-EVID-007、010、018 | Partially confirmed | Yes；registered-format consumer route |
| CLIP-009 | Multiple-format atomic publication | CLIP-EVID-009、010、015 | Partially confirmed | Yes |
| CLIP-010 | Alpha-channel fidelity | CLIP-EVID-010、011 | Unknown | Yes |
| CLIP-011 | Pixel-format fidelity | CLIP-EVID-010、011 | Partially confirmed | Yes |
| CLIP-012 | Color profile／HDR-to-SDR responsibility | CLIP-EVID-010、011 | Unknown | Yes |
| CLIP-013 | STA／COM threading requirements | CLIP-EVID-012、013、014 | Partially confirmed | Yes |
| CLIP-014 | UI thread／Dispatcher dependency | CLIP-EVID-001、013、014 | Unknown | Yes |
| CLIP-015 | Clipboard contention handling | CLIP-EVID-009、012、015 | Partially confirmed | Yes |
| CLIP-016 | Retry／timeout behavior | CLIP-EVID-009、012、015 | Unknown | Yes |
| CLIP-017 | Data ownership and lifetime | CLIP-EVID-002、009、012 | Partially confirmed | Yes |
| CLIP-018 | Immediate／delayed rendering | CLIP-EVID-009、012 | Confirmed by official source | Yes；host behavior |
| CLIP-019 | Clipboard History／Cloud Clipboard behavior | CLIP-EVID-010、016、017 | Partially confirmed | Yes |
| CLIP-020 | Large-image memory implications | CLIP-EVID-009、016 | Partially confirmed | Yes |
| CLIP-021 | Failure recovery and cleanup | CLIP-EVID-009、012、015 | Partially confirmed | Yes |
| CLIP-022 | Testability and privacy-safe evidence | CLIP-EVID-008、016、017 | Partially confirmed | Yes；synthetic runtime only |

## 10. Candidate Identity Matrix

| Candidate | Exact API identity | Namespace/header/assembly | Managed/native boundary | COM/STA dependency | Packaging context | Evidence | Local availability | Build/Runtime verified |
|---|---|---|---|---|---|---|---|---|
| CLIP-OPT-001 | `System.Windows.Clipboard`; `SetDataObject`; `SetImage` | `System.Windows`; `System.Windows.DataObject`; `PresentationCore.dll` | Managed WPF wrapper over system Clipboard | UI/STA behavior remains runtime question | WPF desktop; packaged/unpackaged impact unknown | CLIP-EVID-001..003 | Unknown | No |
| CLIP-OPT-002 | `Windows.ApplicationModel.DataTransfer.Clipboard`; `DataPackage`; `SetContent` | WinRT namespace; projection/metadata; `DataPackage` | Managed projection or native WinRT boundary | DataPackage agile claim does not close host Dispatcher path | Packaged/unpackaged impact unknown | CLIP-EVID-004..007 | Unknown | No |
| CLIP-OPT-003 | `OleSetClipboard`; `OleFlushClipboard`; `IDataObject` | `ole2.h`; Ole32.lib/DLL; COM `IDataObject` | Native OLE/COM and adapter boundary | STA/message/lifetime relevant | Desktop; packaging impact unknown | CLIP-EVID-012..014 | Unknown | No |
| CLIP-OPT-004 | `OpenClipboard`; `EmptyClipboard`; `SetClipboardData`; `CloseClipboard`; `RegisterClipboardFormat` | `Winuser.h`; GDI/HGLOBAL/native memory | Raw Win32/GDI/native memory | Window/message ownership; COM not inherently required | Desktop; packaging impact unknown | CLIP-EVID-008..011、018 | Unknown | No |
| CLIP-OPT-005 | Host-neutral abstraction; no single API identity | Framework-specific adapters; exact contract not created | Abstract product boundary over selected adapters | Depends on selected adapter; not assumed | Must support both Host contexts only after evidence | CLIP-EVID-001..018 | Unknown | No |

## 11. Clipboard Format Matrix

| Format | Official identity | Payload representation | Alpha behavior | Color metadata | Consumer compatibility | Candidate support | Evidence |
|---|---|---|---|---|---|---|---|
| Bitmap | Framework/WinRT Bitmap representation | `BitmapSource`, `Image` or `RandomAccessStreamReference` depending on candidate | Unknown across conversion/consumer path | Unknown | Consumer-specific | API available, product suitability unverified | CLIP-EVID-001、005、006 |
| `CF_BITMAP` | Standard format value `2`; `HBITMAP` | GDI bitmap handle | Not assumed complete | Device-dependent behavior must be considered | Broad legacy support is not a product claim | Officially documented | CLIP-EVID-009、011 |
| `CF_DIB` | Standard format value `8`; `BITMAPINFO` + bits | Movable global memory object | Requires synthetic fidelity test | DIB conversion rules documented; product color contract open | Consumer support varies | Officially documented | CLIP-EVID-009..011 |
| `CF_DIBV5` | Standard format value `17`; `BITMAPV5HEADER` + color space/bits | Movable global memory object | Must test source/consumer behavior | Color space fields documented; conversion responsibility open | Consumer support varies | Officially documented | CLIP-EVID-010、011 |
| PNG registered format | Application-registered format identity | PNG byte stream with registered format ID | Encoded PNG semantics require consumer evidence | Embedded PNG metadata and Clipboard consumer behavior separate | Not guaranteed by registration alone | Requires documented native interop | CLIP-EVID-007、010、018 |
| WinRT Bitmap | `StandardDataFormats.Bitmap` via `DataPackage.SetBitmap` | `RandomAccessStreamReference` | Not closed for SnipPlus final image | Not closed | WinRT-aware consumer route | API available, product suitability unverified | CLIP-EVID-004、006 |
| Framework-specific image object | WPF/host object representation | Framework-managed object/data object | Depends on adapter conversion | Depends on conversion | Not cross-framework by identity | Requires runtime prototype | CLIP-EVID-001..003 |
| Custom registered format | `RegisterClipboardFormat` name/value | Candidate-defined payload | Unknown | Unknown | Source and target must understand same format | Requires documented native interop | CLIP-EVID-007、018 |
| Multiple `IDataObject`/`DataPackage` formats | Multiple formats representing same result | Several payloads published under one Clipboard operation | Format-specific; no global guarantee | Format-specific | Consumer chooses recognized format | API available, product suitability unverified | CLIP-EVID-009、010、015 |

目前不得宣告任何一列是正式產品格式；`PNG byte stream` 與 decoded bitmap 不是同一 payload；`CF_BITMAP` 不得被假設為完整 Alpha 保存格式。

## 12. Candidate–Host Compatibility Matrix

建立正好十列，覆蓋五個 Candidate × 兩個 Host。`Unknown` 不直接變成排除；Framework wrapper 存在不等於 Clipboard contention 已解決；Sample 存在不等於目前 Repository 可 Build。

| Pair ID | Candidate | Host | Invocation route | Threading requirement | Packaging dependency | Native interop | Support status | Evidence |
|---|---|---|---|---|---|---|---|---|
| CLIP-PAIR-001 | CLIP-OPT-001 WPF Clipboard | WPF | `System.Windows.Clipboard`／`DataObject` | WPF UI/STA behavior runtime TBD | Packaged/unpackaged TBD | Wrapper over system Clipboard | Officially documented | CLIP-EVID-001..003 |
| CLIP-PAIR-002 | CLIP-OPT-001 WPF Clipboard | WinUI 3 | WPF assembly/adapter route | Cross-framework dispatcher route TBD | Packaged/unpackaged TBD | Required between WinUI host and WPF types | Requires documented native interop | CLIP-EVID-001..003 |
| CLIP-PAIR-003 | CLIP-OPT-002 WinRT Clipboard | WPF | WinRT projection from WPF desktop | Dispatcher/COM route runtime TBD | Packaged/unpackaged TBD | Required projection/interoperability | Requires documented native interop | CLIP-EVID-004..007 |
| CLIP-PAIR-004 | CLIP-OPT-002 WinRT Clipboard | WinUI 3 | WinRT `Clipboard.SetContent(DataPackage)` | DataPackage API documented; host route runtime TBD | Packaged/unpackaged TBD | Projection/native boundary remains | API available, product suitability unverified | CLIP-EVID-004..007 |
| CLIP-PAIR-005 | CLIP-OPT-003 Win32 OLE Clipboard | WPF | Native OLE adapter | STA/message loop and lifetime | Packaged/unpackaged TBD | `IDataObject`/COM interop required | Requires documented native interop | CLIP-EVID-012..014 |
| CLIP-PAIR-006 | CLIP-OPT-003 Win32 OLE Clipboard | WinUI 3 | Native OLE adapter | STA/message loop and lifetime | Packaged/unpackaged TBD | `IDataObject`/COM interop required | Requires documented native interop | CLIP-EVID-012..014 |
| CLIP-PAIR-007 | CLIP-OPT-004 Win32 Raw Clipboard | WPF | P/Invoke/native adapter | Window/message ownership; runtime TBD | Packaged/unpackaged TBD | `HGLOBAL`/GDI/format interop | Requires documented native interop | CLIP-EVID-008..011、018 |
| CLIP-PAIR-008 | CLIP-OPT-004 Win32 Raw Clipboard | WinUI 3 | P/Invoke/native adapter | Window/message ownership; runtime TBD | Packaged/unpackaged TBD | `HGLOBAL`/GDI/format interop | Requires documented native interop | CLIP-EVID-008..011、018 |
| CLIP-PAIR-009 | CLIP-OPT-005 Host-neutral abstraction | WPF | WPF adapter behind abstract boundary | Adapter-specific; runtime TBD | Must support chosen package mode | Adapter boundary required | Requires runtime prototype | CLIP-EVID-001..018 |
| CLIP-PAIR-010 | CLIP-OPT-005 Host-neutral abstraction | WinUI 3 | WinRT/native adapter behind abstract boundary | Adapter-specific; runtime TBD | Must support chosen package mode | Adapter boundary required | Requires runtime prototype | CLIP-EVID-001..018 |

本表不是 ranking，也不產生 `Winner`、`Best` 或 `Recommended` 結論。

## 13. Threading and COM Contract

官方文件確認 COM apartment、STA message loop、OLE object lifetime 與 cross-apartment boundary；但未由文件直接確認 SnipPlus 的 Host route、Dispatcher shutdown、retry 或 workflow completion 行為。

| Concern | Current evidence | Current status | Required future verification |
|---|---|---|---|
| STA requirement | COM documentation identifies UI/message-loop STA use | Partially satisfied | WPF and WinUI host spike |
| COM initialization | `CoInitializeEx` apartment choice is platform concern | Partially satisfied | Native/OLE adapter spike |
| UI thread/Dispatcher | Host-specific route not defined | Unknown | WPF/WinUI dispatcher spike |
| Background thread publication | Cross-thread object ownership is not assumed safe | Unknown | Background-thread spike |
| Dispatcher shutdown | No product-specific evidence | Unknown | Shutdown/cancellation spike |
| Clipboard operation vs workflow completion | SPEC-0007 distinguishes handoff boundary from consumer acceptance | Partially satisfied | Integration spike |
| Retry on original Dispatcher | No official product rule | Unknown | Contention/retry spike |
| Cancellation/application shutdown | OLE lifetime and app lifetime interact | Partially satisfied | Ownership/lifetime spike |
| OLE ownership | `OleSetClipboard` AddRef/delayed rendering documented | Confirmed by official source | Ownership/lifetime spike |
| Delayed rendering | Win32/OLE behavior documented | Confirmed by official source | Performance and shutdown spike |

固定架構邊界：

- `COMP-009` 或 Repository 中實際的 Workflow owner 只發出完成/交付事件。
- Clipboard adapter 不得修改 Shared Workflow State；只有 `COMP-001` 具 Shared State Authority。
- Clipboard contention 不得使 workflow 回退到 Editing 狀態。
- Clipboard retry 不得重新執行 Capture 或 Rendering。
- Clipboard 的 UI feedback、failure classification 與 state transition 依 `COMP-012`、`COMP-013`、`SPEC-0006` 的既有邊界，不能在本文件新增第二套。

## 14. Publication Contract

只規格研究問題，不做產品決策；不得建立正式 Interface、Class 或 DTO。

| Contract concern | Research boundary | Current status |
|---|---|---|
| Immutable input | Completed or annotated result supplied by upstream handoff | Partially satisfied |
| Image dimensions | Must be carried as observation, not inferred from Clipboard format | Unknown |
| Pixel format | Candidate/format-specific metadata | Partially satisfied |
| Row stride | Native DIB-specific concern | Unknown |
| Alpha premultiplication | Must be measured with synthetic input | Unknown |
| Color-space metadata | DIBV5/PNG/consumer responsibilities remain separate | Unknown |
| Candidate-specific conversion | Adapter responsibility; no global conversion chosen | Not evaluated |
| Multiple-format publication | Official Win32 multi-format model exists | Partially satisfied |
| Clipboard ownership transfer | Raw Win32/OLE differ in ownership/lifetime details | Partially satisfied |
| Success definition | Handoff outcome vs Consumer acceptance must remain separate | Partially satisfied |
| Partial format failure | Must not be silently treated as full success | Unknown |
| Retry payload | Whether retry reuses or rebuilds payload is unselected | Unknown |
| Cleanup | Ownership and failure cleanup are candidate-specific | Partially satisfied |

## 15. Clipboard Contention and Retry Boundary

至少研究：

- Clipboard 被其他 Process 占用、`OpenClipboard` 失敗、`OleSetClipboard` failure HRESULT。
- Framework exception 類型與 platform failure detail 的轉換邊界。
- Retry 是否由平台提供、是否需由應用程式實作，以及 retry 是否阻塞 Dispatcher。
- UI freeze、application shutdown、Clipboard owner 在 retry 間改變的情況。
- Timeout 後的 workflow 結果、Clipboard failure 是否仍允許平行 File Output 完成。

| Question | Official basis | Status | Product decision |
|---|---|---|---|
| Can another process hold the Clipboard open? | CLIP-EVID-009、012 | Confirmed by official source | Retry/timeout TBD |
| Does `OleSetClipboard` expose failure codes? | CLIP-EVID-012 | Confirmed by official source | Failure mapping TBD |
| Does platform provide SnipPlus retry policy? | No official product source | Unknown | TBD |
| Is retry safe on UI Dispatcher? | CLIP-EVID-013、014 plus no product evidence | Unknown | TBD |
| Must retry be bounded? | Product threshold not defined | Unknown | TBD |
| Does Clipboard failure cancel File Output? | SPEC-0010 says parallel; runtime outcome open | Partially satisfied | Must remain independent |
| Does failure preserve result? | SPEC-0007 marks preservation UNKNOWN/TBD | Partially satisfied | Runtime evidence required |
| Does retry repeat Capture/Rendering? | Architecture boundary prohibits it | Confirmed by repository boundary | Must not repeat |

不得自行決定 Retry policy、固定 Timeout、靜默重試、清除使用者既有 Clipboard 或重新產生 Capture Result。

## 16. Clipboard History and Cloud Clipboard

官方 Support 文件指出 Clipboard History 可保存多個項目、可釘選、可同步至 Cloud，並指出每個項目 4 MB 限制、Bitmap 支援與 25 個項目限制。官方 Win32 文件另記錄可用 registered format 控制 History/Cloud inclusion。這些都是平台能力與風險，不是 SnipPlus 已選擇的政策。

| Concern | Evidence | Current status | SnipPlus implication |
|---|---|---|---|
| History enabled state | CLIP-EVID-016、017 | Confirmed by official source | Must be treated as user/platform state |
| Cloud sync setting | CLIP-EVID-017 | Confirmed by official source | Clipboard success is not proof of local-only retention |
| 4 MB History item limit | CLIP-EVID-016 | Confirmed by official source | Large image gate remains open |
| Bitmap History support | CLIP-EVID-016 | Confirmed by official source | Does not close Alpha/color fidelity |
| 25-item History limit | CLIP-EVID-017 | Confirmed by official source | Retention is platform-managed |
| Registered exclusion/inclusion formats | CLIP-EVID-010、018 | Partially confirmed | Must not be enabled without privacy decision |
| Multiple formats in History | Official behavior not sufficient for all formats | Unknown | Runtime synthetic evidence required |
| Cloud treatment of image/registered formats | Official general guidance insufficient for product claim | Unknown | Gap remains |
| Application control over user settings | No authorization in this research | Not applicable | Do not modify History/Cloud settings |

## 17. Security and Privacy Boundary

明確記錄：

- Clipboard 是跨 Process 共享資源，其他 Process 可能讀取內容。
- Clipboard History 可能持久保存內容；Cloud Clipboard 可能涉及跨裝置同步。
- Clipboard 成功不得被解讀為資料已安全刪除。
- 不記錄實際 Clipboard image、image bytes、私人 Window title、私人路徑或使用者內容。
- Runtime Evidence 只能使用 synthetic image；diagnostic log 不得包含 image bytes 或私人內容。
- Clipboard failure message 不得包含私人 Window title 或完整 path。
- 不清除使用者既有 Clipboard 作為測試前置步驟，除非未來另有明確授權與隔離環境。

## 18. Failure Classification

| Failure class | Example boundary | Evidence status | Future handling question |
|---|---|---|---|
| Clipboard unavailable | Clipboard 目前無法開啟 | Partially confirmed | Retry/feedback TBD |
| Threading violation | STA／Dispatcher 條件不符合 | Unknown | Host-specific verification |
| Format conversion failure | Bitmap／DIB／PNG 轉換失敗 | Unknown | Preserve result and classify |
| Partial publication | 部分格式未成功 | Unknown | Define acceptance semantics |
| Ownership loss | Process 或 OLE object lifetime 問題 | Partially confirmed | Ownership spike |
| Memory pressure | 大型 payload 配置失敗 | Partially confirmed | Synthetic size observation |
| Packaging/interop failure | Host 或封裝模式不相容 | Unknown | Packaged/unpackaged spike |
| Privacy stop | Evidence 可能包含私人資料 | Confirmed by repository boundary | Stop; no silent success |
| Unknown platform failure | 官方文件無足夠分類 | Confirmed by research method | Preserve Unknown; do not guess |

研究但不決定：可重試/不可重試證據、cleanup 責任、Clipboard 原內容是否保持不變、平行 File Output 是否繼續、是否需要使用者可見錯誤、是否需要重新執行 publication。不得設計正式 UI 錯誤訊息。

## 19. Critical Gates

| Gate | Requirement | Current status | Blocking evidence |
|---|---|---|---|
| CLIP-GATE-001 | 至少一種候選可發布可互通的影像格式 | Partially satisfied | Consumer/runtime evidence |
| CLIP-GATE-002 | Alpha 及 pixel-format 責任可明確界定 | Requires runtime prototype | CLIP-010、011 |
| CLIP-GATE-003 | STA／COM／Dispatcher 需求已定義 | Partially satisfied | Host-specific behavior |
| CLIP-GATE-004 | Clipboard contention 及 retry 證據路徑已定義 | Partially satisfied | CLIP-015、016 |
| CLIP-GATE-005 | Data ownership 與 Process lifetime 已定義 | Partially satisfied | OLE/raw differences |
| CLIP-GATE-006 | WPF／WinUI 3 Host 路徑可分別驗證 | Requires runtime prototype | CLIP-002、003 |
| CLIP-GATE-007 | Packaged／unpackaged 影響可評估 | Not evaluated | CLIP-004、005 |
| CLIP-GATE-008 | Clipboard History／Cloud privacy 風險已界定 | Partially satisfied | Image/registered-format behavior |
| CLIP-GATE-009 | Clipboard 與 File Output 失敗保持獨立 | Partially satisfied | Integration runtime evidence |
| CLIP-GATE-010 | Evidence 可使用 synthetic data 重現 | Partially satisfied | Future spike evidence policy |

## 20. Ownership Boundary

### 20.1 Clipboard integration owns

- Candidate-specific Clipboard publication research。
- Format packaging research。
- Clipboard operation result classification。
- Clipboard-specific failure classification input。
- Clipboard ownership、lifetime、cleanup research。
- Candidate-specific retry observation。

### 20.2 Clipboard integration does not own

- Print Screen、Capture、Selection、Annotation 或 Rendering。
- Shared Workflow State、Shared State vocabulary 或 workflow transition。
- Save PNG、File naming、File-system storage、Output lifecycle。
- Application-wide error state、Clipboard History setting、Cloud Clipboard setting。
- Consumer 內部處理或資料消費。

## 21. Traceability to Frozen Repository Boundaries

| Clipboard research boundary | Product/architecture source | Exact implication |
|---|---|---|
| Deliver result | `PRD/PRD-0005-functional-requirements.md` `FR-007` | 完成 result 可交付至 clipboard；API/format remain open |
| Handoff start | `Specs/SPEC-0007-clipboard-handoff.md` `Clipboard Ready` | Technology adapter 不得改寫 upstream completion |
| Consumer acceptance | `Specs/SPEC-0007-clipboard-handoff.md` `Clipboard Consumer` | Consumer behavior remains outside this candidate study |
| Clipboard/Output parallelism | `Specs/SPEC-0010-feature-integration.md` | Clipboard failure must not become Output prerequisite |
| Product semantics | `Architecture/ARCH-0003-module-catalog.md` `MOD-005` | Handoff semantics remain separate from API/data format |
| Platform adapter | `Architecture/ARCH-0003-module-catalog.md` `MOD-009` | Platform-specific Clipboard interaction only |
| Component boundary | `Architecture/ARCH-0004-component-boundaries.md` `COMP-009`, `COMP-015` | No Shared State access; no Output/Storage ownership |
| Interaction path | `Architecture/ARCH-0005-component-interactions.md` `INT-012`, `INT-013` | Handoff request then platform delivery request |
| Decision ordering | `Architecture/TECHNOLOGY-DECISION-ROADMAP.md` `TD-004` | Candidate only; no selection/ADR |
| UI host uncertainty | `Architecture/adr/ADR-0002-ui-framework-selection.md` | WPF/WinUI pair research remains parallel |

## 22. Future Runtime Spikes

只定義，不執行；本節不得被解讀為 Clipboard、Build、Runtime 或 Evidence 授權。所有輸入必須是 synthetic image，所有結果應先回傳目前 Session，未來是否建立持久 Evidence 另需授權。

### 22.1 CLIP-SPIKE-001 — WPF basic bitmap publication

| Field | Value |
|---|---|
| Purpose | 驗證 WPF candidate 的基本 bitmap publication 與 consumer acceptance boundary |
| Candidates | CLIP-OPT-001、CLIP-OPT-005 |
| Hosts | WPF |
| Synthetic input | 固定尺寸、無私人內容、可重建 bitmap |
| Preconditions | UI host/Clipboard execution authorization、synthetic data policy |
| Execution outline | 建立 isolated prototype，publication 後由受控 consumer 觀察 formats；not executed |
| Required evidence | format list、publication outcome、consumer acceptance、thread、lifetime、cleanup |
| Functional pass condition | 在不修改 workflow state 下，至少清楚區分 publication 與 consumer acceptance |
| Measurement fields | dimensions、pixel format、alpha mode、format IDs、elapsed time、failure class |
| Privacy controls | synthetic only；不讀既有 Clipboard、window title 或 desktop image |
| Failure implication | 保留 result，將 Clipboard failure 與 workflow/file-output 分開分類 |
| Cleanup | isolated process/objects/Clipboard payload cleanup；future authorization required |
| Dependency | CLIP-GATE-001、003、009；UI decision still unresolved |
| Prohibited scope | product source、real screenshot、Capture、File Output、Clipboard History mutation |

### 22.2 CLIP-SPIKE-002 — WinUI 3 basic bitmap publication

| Field | Value |
|---|---|
| Purpose | 驗證 WinUI 3 host 使用 WinRT DataPackage/Clipboard 的基本 publication route |
| Candidates | CLIP-OPT-002、CLIP-OPT-005 |
| Hosts | WinUI 3 |
| Synthetic input | 固定尺寸 synthetic bitmap/stream |
| Preconditions | WinUI host decision、isolated project/build/runtime authorization |
| Execution outline | 建立 isolated prototype，使用 documented WinRT route；not executed |
| Required evidence | DataPackage route、host projection、formats、consumer acceptance、thread/lifetime |
| Functional pass condition | API route 可被 observation 清楚重現，且不形成 product selection |
| Measurement fields | format identity、stream metadata、dimensions、thread、elapsed time、outcome |
| Privacy controls | synthetic only；不讀或寫使用者目前 Clipboard |
| Failure implication | Host/projection failure 與 Clipboard consumer failure 分開 |
| Cleanup | isolated process and payload cleanup；future authorization required |
| Dependency | CLIP-GATE-001、003、006、009 |
| Prohibited scope | real capture、source code merge、File Output、History/Cloud setting mutation |

### 22.3 CLIP-SPIKE-003 — DIB/DIBV5 Alpha fidelity

| Field | Value |
|---|---|
| Purpose | 比較 `CF_DIB`／`CF_DIBV5` 的 Alpha、stride、color-space 與 consumer readback |
| Candidates | CLIP-OPT-003、CLIP-OPT-004、CLIP-OPT-005 |
| Hosts | WPF、WinUI 3、controlled native consumer |
| Synthetic input | 透明、半透明、opaque 色塊與明確 pixel pattern |
| Preconditions | Native clipboard/format execution authorization；no current user data |
| Execution outline | Prepare synthetic DIB/DIBV5 in isolated prototype and inspect controlled readback；not executed |
| Required evidence | headers、alpha values、stride、color fields、consumer readback、conversion notes |
| Functional pass condition | 觀察結果足以分開 format identity 與 fidelity，否則 Gate remains open |
| Measurement fields | width、height、stride、bit depth、alpha mode、color space、diff summary |
| Privacy controls | synthetic pixels only；不保留 real image |
| Failure implication | 建立 format/fidelity Gap，不選定產品格式 |
| Cleanup | release native memory/handles and isolated clipboard state |
| Dependency | CLIP-GATE-002、005 |
| Prohibited scope | real screenshot、HDR display mutation、product conversion code |

### 22.4 CLIP-SPIKE-004 — PNG registered-format interoperability

| Field | Value |
|---|---|
| Purpose | 驗證 PNG registered format 的 registration、publication 與 controlled consumer recognition |
| Candidates | CLIP-OPT-002、CLIP-OPT-003、CLIP-OPT-004 |
| Hosts | WPF、WinUI 3、controlled consumer |
| Synthetic input | Synthetic PNG bytes with known dimensions/alpha metadata |
| Preconditions | Registered-format and isolated runtime authorization |
| Execution outline | Register format in isolated process, publish and inspect controlled consumer; not executed |
| Required evidence | registered ID/name、payload length、consumer recognition、alpha/color readback |
| Functional pass condition | 明確知道 consumer 是否認得格式，不把 registration 當成 universal support |
| Measurement fields | format ID、bytes、dimensions、alpha、color metadata、outcome |
| Privacy controls | synthetic PNG only；no user Clipboard access |
| Failure implication | Registered format remains candidate-specific; no silent fallback claim |
| Cleanup | unregister/process cleanup where applicable; no user settings mutation |
| Dependency | CLIP-GATE-001、002、006 |
| Prohibited scope | selecting PNG as formal product format、real images、File Output |

### 22.5 CLIP-SPIKE-005 — Multi-format publication

| Field | Value |
|---|---|
| Purpose | 觀察同一 synthetic result 提供 bitmap/DIB/DIBV5/PNG/custom formats 的 publication boundary |
| Candidates | CLIP-OPT-001..005 |
| Hosts | WPF、WinUI 3、controlled consumers |
| Synthetic input | One immutable synthetic image with deterministic metadata |
| Preconditions | Multi-format publication authorization and controlled consumers |
| Execution outline | Publish multiple representations once; inspect enumeration and consumer choice；not executed |
| Required evidence | format order、consumer-selected format、partial failure、ownership、cleanup |
| Functional pass condition | 能區分 one publication、multiple representations 與 consumer acceptance |
| Measurement fields | format IDs、order、payload sizes、elapsed time、outcome per format |
| Privacy controls | synthetic only；no History/Cloud setting changes |
| Failure implication | Partial publication must be explicit; no full-success inference |
| Cleanup | release all payload variants and isolated Clipboard owner |
| Dependency | CLIP-GATE-001、009 |
| Prohibited scope | Candidate ranking、product API、real capture or File Output |

### 22.6 CLIP-SPIKE-006 — STA/background-thread behavior

| Field | Value |
|---|---|
| Purpose | 觀察 WPF/WinUI/native adapter 在 UI STA 與 background thread 的 publication 行為 |
| Candidates | CLIP-OPT-001..005 |
| Hosts | WPF、WinUI 3、native controlled host |
| Synthetic input | Reusable synthetic image and immutable payload |
| Preconditions | Threading/runtime authorization、message-pump-safe harness |
| Execution outline | Compare UI STA, dedicated STA and background paths without blocking message loop；not executed |
| Required evidence | apartment、thread ID alias、dispatcher state、deadlock/exception、outcome |
| Functional pass condition | Threading route and failure classification are observable without hanging host |
| Measurement fields | apartment model、dispatcher status、elapsed time、exception class、cleanup |
| Privacy controls | synthetic only；no real Clipboard content logged |
| Failure implication | Threading violation remains separate from format failure |
| Cleanup | stop isolated workers and release COM/Clipboard objects |
| Dependency | CLIP-GATE-003、004、005 |
| Prohibited scope | changing product threading model, permanent background service, source merge |

### 22.7 CLIP-SPIKE-007 — Clipboard contention and bounded retry observation

| Field | Value |
|---|---|
| Purpose | 觀察 controlled contention、failure signal與 bounded retry evidence |
| Candidates | CLIP-OPT-001..005 |
| Hosts | WPF、WinUI 3、native controlled consumer |
| Synthetic input | Synthetic image; no existing user Clipboard read |
| Preconditions | Isolated contention harness、explicit retry experiment authorization |
| Execution outline | Controlled second process holds Clipboard, publication observes failure and predefined experiment bounds；not executed |
| Required evidence | contention signal、retry attempts (if authorized)、elapsed time、owner transition、final outcome |
| Functional pass condition | Failure is observable and bounded; no policy is promoted automatically |
| Measurement fields | attempt count、interval observation、timeout observation、HRESULT/exception、result preservation |
| Privacy controls | synthetic only；do not clear user Clipboard outside isolated environment |
| Failure implication | Record retry feasibility Gap; do not define product retry policy |
| Cleanup | release isolated owner and verify no process remains |
| Dependency | CLIP-GATE-004、009 |
| Prohibited scope | arbitrary retry loop、user Clipboard clearing、real workflow mutation |

### 22.8 CLIP-SPIKE-008 — Ownership/process-lifetime behavior

| Field | Value |
|---|---|
| Purpose | 觀察 immediate/delayed rendering、OLE ownership、process shutdown與 payload lifetime |
| Candidates | CLIP-OPT-003、CLIP-OPT-004、CLIP-OPT-005 |
| Hosts | WPF/WinUI adapter and native OLE harness |
| Synthetic input | Synthetic payload with deterministic lifetime markers |
| Preconditions | Isolated process-lifetime authorization |
| Execution outline | Publish, request controlled consumer read, close/flush process in defined phases；not executed |
| Required evidence | AddRef/flush behavior where observable、render timing、post-exit availability、cleanup |
| Functional pass condition | Ownership and post-exit behavior are separately documented |
| Measurement fields | process state、owner state、format render state、elapsed time、consumer result |
| Privacy controls | synthetic only；no persistent real Clipboard content |
| Failure implication | Ownership loss is classified independently from format conversion |
| Cleanup | `OleFlushClipboard`/isolated cleanup only when authorized; no user data change |
| Dependency | CLIP-GATE-005、008 |
| Prohibited scope | deciding persistence policy、History/Cloud settings、production code |

### 22.9 CLIP-SPIKE-009 — Large synthetic image memory observation

| Field | Value |
|---|---|
| Purpose | 觀察 large synthetic payload allocation、publication、memory pressure與 failure boundary |
| Candidates | CLIP-OPT-001..005 |
| Hosts | WPF、WinUI 3、native controlled host |
| Synthetic input | Size ladder specified by future experiment authority；not product threshold |
| Preconditions | Explicit memory experiment authorization and isolated machine context |
| Execution outline | Publish authorized synthetic sizes, record process/memory/outcome；not executed |
| Required evidence | payload size、allocation outcome、peak memory、format outcome、cleanup |
| Functional pass condition | Produce evidence for memory behavior without setting a product maximum |
| Measurement fields | width、height、bytes、peak/private memory、elapsed time、failure class |
| Privacy controls | synthetic only；no screen or user image |
| Failure implication | Memory pressure remains a candidate failure; no automatic retry policy |
| Cleanup | release payloads and terminate isolated process |
| Dependency | CLIP-GATE-001、002、010 |
| Prohibited scope | setting maximum image size, real screenshot, performance promise |

### 22.10 CLIP-SPIKE-010 — Clipboard History/Cloud behavior boundary

| Field | Value |
|---|---|
| Purpose | 在受控設定與 synthetic payload 下觀察 History/Cloud visibility boundary |
| Candidates | CLIP-OPT-001..005 |
| Hosts | Windows 11 controlled user profile |
| Synthetic input | Clearly synthetic, non-sensitive image and registered control formats |
| Preconditions | Separate test account/profile, explicit privacy authorization, no production account |
| Execution outline | Observe platform behavior only; do not change user settings; not executed |
| Required evidence | History visibility、format visibility、size outcome、sync observation if authorized |
| Functional pass condition | Evidence differentiates platform setting from application guarantee |
| Measurement fields | settings state、format ID、size、History outcome、Cloud observation category |
| Privacy controls | isolated profile; no real user Clipboard; no cloud account data in logs |
| Failure implication | Privacy Gap remains if behavior cannot be safely proven |
| Cleanup | clear only isolated test profile data when explicitly authorized |
| Dependency | CLIP-GATE-008、010 |
| Prohibited scope | modifying production History/Cloud settings, uploading real images |

### 22.11 CLIP-SPIKE-011 — Packaged/unpackaged comparison

| Field | Value |
|---|---|
| Purpose | Compare Clipboard publication route in packaged and unpackaged desktop contexts |
| Candidates | CLIP-OPT-001..005 |
| Hosts | WPF and WinUI 3 isolated hosts |
| Synthetic input | Same deterministic synthetic payload in both contexts |
| Preconditions | Packaging/project/build/runtime authorization; currently absent |
| Execution outline | Build and run isolated package variants, then compare publication evidence；not executed |
| Required evidence | package identity、host route、format outcome、threading、lifetime、privacy |
| Functional pass condition | Packaging effects are explicit or remain Unknown |
| Measurement fields | package mode、version、format IDs、outcome、failure class、cleanup |
| Privacy controls | synthetic only; no production package or account |
| Failure implication | Packaging/interop failure remains separate from Clipboard availability |
| Cleanup | uninstall/cleanup only within authorized isolated scope |
| Dependency | CLIP-GATE-006、007 |
| Prohibited scope | installing project dependencies into product environment, production packaging |

### 22.12 CLIP-SPIKE-012 — Clipboard failure independent from File Output

| Field | Value |
|---|---|
| Purpose | Verify parallel downstream result semantics when Clipboard publication fails |
| Candidates | CLIP-OPT-001..005 as adapter variants |
| Hosts | Selected host pair after separate authorization |
| Synthetic input | One immutable synthetic Capture Result surrogate |
| Preconditions | Integration harness authorization; no real Capture or File Output path |
| Execution outline | Simulate Clipboard failure while separate output surrogate reports its own result；not executed |
| Required evidence | Clipboard outcome、Output outcome、shared state requests、result preservation、feedback boundary |
| Functional pass condition | Clipboard failure does not require Output failure or workflow-state rollback |
| Measurement fields | outcome per downstream path、state requests、failure owner、elapsed time |
| Privacy controls | synthetic data; no actual Clipboard or file persistence unless separately authorized |
| Failure implication | Maintain parallel path or create explicit integration Gap |
| Cleanup | isolate all harness state; no product source changes |
| Dependency | CLIP-GATE-009、010; SPEC-0010 |
| Prohibited scope | Capture execution, screenshot generation, formal workflow implementation |

## 23. Evidence Gap Register

`CLIP-GAP-001..018` 均是本文件目前仍未關閉的 evidence gaps；「官方文件未說明」不等於不支援。

| Gap ID | Missing claim | Candidate | Host | Related criterion | Related gate | Official sources checked | Why evidence is insufficient | Required future evidence | Local inspection required | Project required | Build required | Runtime required | Blocks Clipboard feasibility conclusion | Status |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| CLIP-GAP-001 | Local Windows/host availability | CLIP-OPT-001..005 | WPF/WinUI 3 | CLIP-001 | CLIP-GATE-001 | CLIP-EVID-001、004、008、016 | Official docs do not inspect this machine | Local baseline + spike | Yes | No | No | Yes | Yes | Open |
| CLIP-GAP-002 | Packaged WinUI Clipboard route | CLIP-OPT-002 | WinUI 3 | CLIP-004 | CLIP-GATE-007 | CLIP-EVID-004..007 | API docs do not prove package mode | CLIP-SPIKE-011 | No | Yes | Yes | Yes | Yes | Open |
| CLIP-GAP-003 | Unpackaged WPF/WinUI route | CLIP-OPT-001/002 | WPF/WinUI 3 | CLIP-005 | CLIP-GATE-006/007 | CLIP-EVID-001..007 | Host packaging context remains unknown | CLIP-SPIKE-011 | No | Yes | Yes | Yes | Yes | Open |
| CLIP-GAP-004 | Consumer interoperability for PNG registered format | CLIP-OPT-002..004 | WPF/WinUI 3 | CLIP-008 | CLIP-GATE-001 | CLIP-EVID-007、010、018 | Registration does not prove consumer recognition | CLIP-SPIKE-004 | No | Yes | Yes | Yes | Yes | Open |
| CLIP-GAP-005 | Multiple-format acceptance across consumers | CLIP-OPT-001..005 | WPF/WinUI 3 | CLIP-009 | CLIP-GATE-001/009 | CLIP-EVID-009、010、015 | Official model does not cover selected consumers | CLIP-SPIKE-005 | No | Yes | Yes | Yes | Yes | Open |
| CLIP-GAP-006 | Alpha fidelity | CLIP-OPT-001..005 | WPF/WinUI 3 | CLIP-010 | CLIP-GATE-002 | CLIP-EVID-010、011 | Format identity does not close conversion fidelity | CLIP-SPIKE-003 | No | Yes | Yes | Yes | Yes | Open |
| CLIP-GAP-007 | Pixel/stride fidelity | CLIP-OPT-003/004 | Native consumer | CLIP-011 | CLIP-GATE-002 | CLIP-EVID-010、011 | Header docs do not prove round-trip pixels | CLIP-SPIKE-003 | No | Yes | Yes | Yes | Yes | Open |
| CLIP-GAP-008 | Color/HDR-to-SDR responsibility | CLIP-OPT-001..005 | WPF/WinUI 3 | CLIP-012 | CLIP-GATE-002 | CLIP-EVID-010、011 | Product color contract and HDR route absent | Future color spike | Yes | Yes | Yes | Yes | Yes | Open |
| CLIP-GAP-009 | Dispatcher and background-thread behavior | CLIP-OPT-001..005 | WPF/WinUI 3 | CLIP-013/014 | CLIP-GATE-003 | CLIP-EVID-013、014 | COM docs do not define host adapter behavior | CLIP-SPIKE-006 | No | Yes | Yes | Yes | Yes | Open |
| CLIP-GAP-010 | Contention and bounded retry evidence | CLIP-OPT-001..005 | WPF/WinUI 3 | CLIP-015/016 | CLIP-GATE-004 | CLIP-EVID-009、012、015 | Failure APIs do not define product retry policy | CLIP-SPIKE-007 | No | Yes | Yes | Yes | Yes | Open |
| CLIP-GAP-011 | OLE/raw ownership after shutdown | CLIP-OPT-003/004 | WPF/WinUI 3 | CLIP-017/018 | CLIP-GATE-005 | CLIP-EVID-009、012 | Ownership rules need host/process evidence | CLIP-SPIKE-008 | No | Yes | Yes | Yes | Yes | Open |
| CLIP-GAP-012 | Large image memory behavior | CLIP-OPT-001..005 | WPF/WinUI 3 | CLIP-020 | CLIP-GATE-001 | CLIP-EVID-009、016 | 4 MB History guidance is not total memory guidance | CLIP-SPIKE-009 | No | Yes | Yes | Yes | Yes | Open |
| CLIP-GAP-013 | History/Cloud image treatment | CLIP-OPT-001..005 | Windows 11 | CLIP-019 | CLIP-GATE-008 | CLIP-EVID-010、016、017 | General settings docs do not close all format cases | CLIP-SPIKE-010 | No | Yes | Yes | Yes | Yes | Open |
| CLIP-GAP-014 | Privacy exclusion/control format behavior | CLIP-OPT-003/004 | Windows 11 | CLIP-019/022 | CLIP-GATE-008/010 | CLIP-EVID-010、018 | Control formats need isolated observation and policy | CLIP-SPIKE-010 | No | Yes | Yes | Yes | Yes | Open |
| CLIP-GAP-015 | Clipboard failure independent from File Output | CLIP-OPT-005 | WPF/WinUI 3 | CLIP-021/022 | CLIP-GATE-009 | SPEC-0010 + CLIP-EVID-009 | Architecture gives boundary, not runtime outcome | CLIP-SPIKE-012 | No | Yes | Yes | Yes | Yes | Open |
| CLIP-GAP-016 | Failure cleanup and result preservation | CLIP-OPT-001..005 | WPF/WinUI 3 | CLIP-021 | CLIP-GATE-005/009 | SPEC-0007 + CLIP-EVID-009、012 | Product recovery remains UNKNOWN/TBD | CLIP-SPIKE-007/008/012 | No | Yes | Yes | Yes | Yes | Open |
| CLIP-GAP-017 | Testability of privacy-safe evidence | CLIP-OPT-005 | WPF/WinUI 3 | CLIP-022 | CLIP-GATE-010 | Repository boundary + CLIP-EVID-016、017 | No approved harness/evidence persistence | Synthetic spike authorization | No | Yes | Yes | Yes | Yes | Open |
| CLIP-GAP-018 | Candidate suitability without ranking | CLIP-OPT-001..005 | WPF/WinUI 3 | CLIP-001..022 | CLIP-GATE-001..010 | All official evidence | Documentation and API identity cannot select product candidate | Future decision context | No | Yes | Yes | Yes | Yes | Open |

## 24. Evidence Readiness

只能使用：`Sufficient for Clipboard ADR`、`Partially sufficient`、`Insufficient for Clipboard ADR`。

### 24.1 Current assessment

> Evidence Readiness: `Partially sufficient`

理由：

- 官方文件已足以確認五個 Candidate 的 API identity、Windows Clipboard 的 ownership/format/multiple-format/delayed-rendering 概念、WPF/WinRT/Win32 的主要平台邊界，以及 Clipboard History/Cloud 的基本風險。
- 官方文件尚不足以確認 SnipPlus 在 WPF／WinUI 3、packaged／unpackaged、STA／Dispatcher、Alpha／color、contention/retry、process lifetime、large image、History/Cloud 與 parallel File Output 的產品適用性。
- Repository 的 PRD、Specs、Architecture 只定義 handoff、ownership、parallel downstream 與 failure boundary；不替 technology research 填入 API/format 結論。
- 因此不得建立 Clipboard ADR、不得標記 `Sufficient for Clipboard ADR`、不得選擇 Clipboard Technology。

### 24.2 Fixed current state

| State | Value |
|---|---|
| Technology Decision | TD-004 Candidate |
| UI Framework Decision | Unresolved — ADR-0002 remains Draft |
| Rendering Decision | Not made |
| Capture Decision | Not made |
| Clipboard Decision | Not made |
| Build Verification | Not performed |
| Runtime Verification | Not performed |
| Clipboard Execution Authorized | No |
| Evidence Artifact Creation | Not performed |
| Clipboard API invocation | Not performed |
| Clipboard read/write/clear | Not performed |
| Runtime Spike | Not performed |
| Candidate ranking | Not created |
| Clipboard ADR | Not created |

## 25. Traceability

```text
Frozen requirement / architecture boundary
  → CLIP criterion
  → official evidence
  → Candidate / Host Pair
  → CLIP Gate
  → Evidence Gap
  → future CLIP Spike
  → future Clipboard Decision
  → future Clipboard ADR only after readiness and review
```

| Trace source | Mapping | Future use | Current state |
|---|---|---|---|
| `PRD/PRD-0005-functional-requirements.md` `FR-007` | deliver completed result → Clipboard | Product requirement context | Frozen requirement; no API decision |
| `Specs/SPEC-0007-clipboard-handoff.md` | Capture Result → Clipboard Ready → abstract Consumer | Handoff semantics | Runtime behavior UNKNOWN/TBD |
| `Specs/SPEC-0010-feature-integration.md` | Clipboard and Output as parallel downstream paths | Integration boundary | No necessary dependency |
| `Architecture/ARCH-0001-architecture-principles.md` | single shared state authority and deferred technology | Governance | No implementation selection |
| `Architecture/ARCH-0002-layer-model.md` | Platform Integration isolates Clipboard side effect | Layer boundary | Candidate adapters only |
| `Architecture/ARCH-0003-module-catalog.md` | `MOD-005` vs `MOD-009` ownership | Module traceability | Candidate |
| `Architecture/ARCH-0004-component-boundaries.md` | `COMP-009` vs `COMP-015`; no Shared State access | Component traceability | Required boundary |
| `Architecture/ARCH-0005-component-interactions.md` | `INT-012` and `INT-013` request flow | Interaction traceability | Required abstract interaction |
| `Architecture/TECHNOLOGY-DECISION-ROADMAP.md` `TD-004` | Clipboard Integration decision candidate | Future ADR | Candidate; not selected |
| `Architecture/adr/ADR-0002-ui-framework-selection.md` | unresolved Host context | Future pair decision | Draft |
| `CLIP-OPT-001..005` | candidate identity → official evidence | Future comparison | No ranking |
| `CLIP-PAIR-001..010` | candidate × host → future runtime route | Future host verification | Not executed |
| `CLIP-001..022` | criterion → evidence/gap | Gate/readiness review | Partially confirmed |
| `CLIP-GATE-001..010` | evidence → future closure | Clipboard ADR readiness | Not closed |
| `CLIP-GAP-001..018` | missing claim → future evidence | Spike planning | Open |
| `CLIP-SPIKE-001..012` | gap/gate → isolated runtime evidence | Future authorized validation | Not executed |
| `RESEARCH-TECH-UI-001` | existing UI research context | Host boundary reference | Referenced only |
| `RESEARCH-TECH-CAPTURE-001` | existing capture research context | Upstream result boundary | Referenced only |
| `RESEARCH-TECH-RENDER-001` | existing rendering research context | Rendering/Clipboard separation | Referenced only |

## Completion Conditions

- 只建立 `docs/Research/Technology/29-clipboard-integration-feasibility.md`。
- 不修改任何其他 Repository 文件。
- 建立五個 Clipboard Candidate：`CLIP-OPT-001..005`。
- 建立正好 `CLIP-001..022`。
- 建立正好 `CLIP-PAIR-001..010`。
- 建立 Clipboard Format Matrix、Candidate Identity Matrix、Threading/COM、Publication、Contention/Retry、History/Cloud、Security/Privacy、Failure 與 Ownership Boundary。
- 建立正好 `CLIP-GATE-001..010`。
- 建立正好 `CLIP-SPIKE-001..012`，只定義不執行。
- 建立 `CLIP-GAP-001..018` Evidence Gap Register。
- Evidence Readiness 固定為 `Partially sufficient`。
- 不選擇 Clipboard Technology、不建立 Clipboard ADR、不建立 Candidate ranking。
- 不讀取、寫入或清除 Windows Clipboard。
- 不建立 Bitmap、PNG、DIB、Clipboard payload、Project、Prototype、Result、Source Code 或 Evidence Artifact。
- 不執行下載、安裝、Restore、Build、Run、Publish 或 Runtime Spike。
- 不修改 Capture／Rendering／UI Research Line，不修改 ADR-0002。
- 不開始正式 Clipboard 或截圖功能。
- `git diff --check` 通過。

## Current Execution Record

| Item | Status |
|---|---|
| Official web research | Performed using Microsoft official sources only for this document |
| Repository source inspection | Read-only; roadmap, PRD, Specs and Architecture context inspected |
| Clipboard read/write/clear | Not performed |
| Clipboard API invocation | Not performed |
| Bitmap/PNG/DIB payload creation | Not performed |
| Project/Prototype/Source Code | Not created |
| Build/Run/Restore/Publish/Test | Not performed |
| Runtime Spike | Not performed |
| Evidence artifact persistence | Not performed |
| Clipboard Technology decision | Not made |
| Clipboard ADR | Not created |
| Candidate ranking | Not created |
