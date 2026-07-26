# Clipboard Integration Official Prerequisite Evidence Baseline

## Document Control

| Field | Value |
|---|---|
| Document ID | RESEARCH-TECH-CLIPBOARD-006 |
| Title | Clipboard Integration Official Prerequisite Evidence Baseline |
| Status | Draft |
| Research Type | Official-source Prerequisite Evidence Baseline |
| Technology Decision | TD-004 Clipboard Integration |
| Parent Enablement Specification | RESEARCH-TECH-CLIPBOARD-005 |
| Parent Closure Plan | RESEARCH-TECH-CLIPBOARD-004 |
| Parent Execution Readiness | RESEARCH-TECH-CLIPBOARD-003 |
| Parent Runtime Plan | RESEARCH-TECH-CLIPBOARD-002 |
| Parent Feasibility | RESEARCH-TECH-CLIPBOARD-001 |
| Official-source Research | Performed in this document only |
| Local Environment Inspection | Not performed |
| Package Cache Inspection | Not performed |
| Build Verification | Not performed |
| Runtime Verification | Not performed |
| Closure Execution Authorized | No |
| Clipboard Runtime Spike Authorized | No |
| Clipboard Read Authorized | No |
| Clipboard Write Authorized | No |
| Clipboard Clear Authorized | No |
| Evidence Write Authorized | No |
| Shared UI Authorization Artifact | Not found / TBD |
| UI Framework Decision | Unresolved; ADR-0002 remains Draft |
| Clipboard Decision | Not made |
| Capture Decision | Not made |
| Rendering Decision | Not made |
| Owner | TBD |
| Last reviewed | 2026-07-26; official-source review only |

## 1. Purpose

本文件只建立 Clipboard Integration Phase L1 的官方前置證據基線。

研究問題是：

1. Microsoft 第一方資料能確認哪些 Clipboard API、Format、Interop identity？
2. 官方資料對 STA、COM、Dispatcher、foreground、message loop、ownership、lifetime、packaging、History、Cloud、privacy 與 failure 的界線說到什麼程度？
3. 哪些結論只能支援 static specification，仍不能取代 Local、Project、Package、Build、Consumer 或 Runtime evidence？
4. 哪些缺口必須保留為明確的 CLIP-OFF-GAP，而不能被推論成支援或不支援？

本文件不做 Clipboard 技術選擇，不修改上游研究，不批准任何操作。

## 2. Scope

本文件覆蓋以下既有識別：

| Scope family | Required coverage |
|---|---|
| Candidates | CLIP-OPT-001..005 |
| Candidate–Host pairs | CLIP-PAIR-001..010 |
| Prerequisites | CLIP-PREQ-001..032 |
| Blockers | CLIP-BLOCK-001..013 |
| Blocking Actions | CLIP-BA-001..006 |
| Closure Actions | CLIP-CLOSE-001..006 |
| Enablement Items | CLIP-ENABLE-001..006 |
| Upstream Gates | CLIP-GATE-001..010 |
| Closure Gates | CLIP-CGATE-001..011 |
| Existing evidence gaps | CLIP-GAP-001..018 |
| Shared UI authority gap | CLIP-ENABLE-GAP-001 |
| New official evidence | CLIP-OFF-EVID-001 onward |
| New official gaps | CLIP-OFF-GAP-001 onward |

官方研究只處理 Phase L1 的 Clipboard prerequisite。Phase L2、Phase L3 與所有 runtime spike 仍維持未授權。

## 3. Non-goals

本文件不得被解讀為下列任何一項的開始：

- 本機環境盤點。
- Package Cache 查詢。
- Project 建立。
- Package acquisition、Restore 或 Build。
- Run、Test、Publish、Deploy。
- Clipboard Read、Write、Clear 或 Backup。
- Clipboard History 開啟。
- Cloud Clipboard 設定變更。
- Consumer 啟動或互操作測試。
- Payload、Bitmap、PNG、DIB、DIBV5 或 Source Code 建立。
- Result directory 或 runtime Evidence Artifact 建立。
- Clipboard Runtime Spike。
- Screenshot 或任何截圖功能。
- Capture、Rendering 或 UI Research Line 修改。
- Clipboard Technology ranking、selection 或 recommendation。
- Clipboard ADR。
- UI-AUTH-* authority artifact 的建立或虛構。

## 4. Source Acceptance Policy

### 4.1 Primary source boundary

主要證據只能來自下列 Microsoft 第一方來源：

- Microsoft Learn .NET API Reference。
- Microsoft Learn Windows SDK API Reference。
- Microsoft Learn WinRT / Windows App SDK API Reference。
- Microsoft Learn Win32 Clipboard conceptual documentation。
- Microsoft Support 的 Windows Clipboard 官方使用說明。
- Microsoft 官方 Windows App SDK packaging 與 desktop documentation。

本文件以 2026-07-26 為 Access date。每一筆 substantive claim 都必須回到 CLIP-OFF-EVID record，再回到官方 URL。

### 4.2 Source quality rules

以下內容不得作為主要證據：

- 搜尋結果摘要。
- Stack Overflow。
- 個人部落格。
- 未確認維護者的 wrapper 文件。
- AI summary。
- 只有 sample 存在但未說明 platform semantics 的文章。
- Local machine 的偶然行為。
- 未經批准的實驗結果。

第三方資料如未來被找到，只能標示 Informative，不能關閉任何 prerequisite、blocker、gate 或 authorization。

### 4.3 Claim interpretation rules

- API existence 不等於 Local availability。
- .NET projection 不等於 Host integration。
- API Reference 不等於 Project、Package、Restore、Build 或 Runtime evidence。
- Sample 不等於目前 Repository 可建立同樣的 Project。
- STA、COM 或 Dispatcher 文件不等於目前 Host 已通過 threading runtime observation。
- Format definition 不等於 Consumer interoperability。
- Registered format identity 不等於 PNG consumer support。
- History / Cloud documentation 不等於本機或帳號設定已開啟。
- Official failure code 不等於產品 retry policy。
- Official ownership semantics 不等於 SnipPlus process-lifetime behavior。
- 找不到官方文件不等於不支援。

## 5. Controlled Vocabulary

### 5.1 Claim Status

只使用：

- Confirmed by official source
- Partially confirmed
- Conflicting official evidence
- Unknown
- Not applicable

### 5.2 Evidence Sufficiency

只使用：

- Sufficient for static specification
- Partially sufficient
- Insufficient
- Conflicting
- Not applicable

### 5.3 Host Support Status

只使用：

- Officially documented
- API available, Host integration unverified
- Requires documented native interop
- Requires runtime prototype
- Not aligned by official evidence
- Unknown

### 5.4 Prohibited conclusion words

本文件不得使用下列詞語作為技術結論：

- Best
- Winner
- Recommended
- Production ready
- Definitely compatible
- Should work

## 6. Existing Evidence Reuse Register

本節已完整檢視 RESEARCH-TECH-CLIPBOARD-001 中的 CLIP-EVID-001..018 與 CLIP-GAP-001..018。以下只判定在本官方基線中的 claim scope，不修改上游文件。

| Existing ID | Original claim scope | Reusable in this baseline | Limitation retained | New baseline action |
|---|---|---|---|---|
| CLIP-EVID-001 | WPF Clipboard exposes SetImage, SetData、SetDataObject and shares system Clipboard | Yes, narrower claim | WPF API identity does not close host, format or runtime behavior | Accepted with narrower claim |
| CLIP-EVID-002 | WPF SetDataObject(Object, Boolean) has process-exit retention semantics | Yes | Product shutdown behavior remains untested | Accepted |
| CLIP-EVID-003 | WPF DataObject / IDataObject participates in data transfer | Yes | Does not prove SnipPlus format fidelity | Accepted with narrower claim |
| CLIP-EVID-004 | WinRT DataPackage supports Bitmap and other data forms | Yes | Does not close desktop packaging or consumer route | Accepted with narrower claim |
| CLIP-EVID-005 | WinRT Clipboard.SetContent sets current content | Yes | Foreground and option requirements remain part of the claim | Accepted with narrower claim |
| CLIP-EVID-006 | DataPackage.SetBitmap accepts RandomAccessStreamReference | Yes | Stream lifetime and consumer acceptance remain open | Accepted with narrower claim |
| CLIP-EVID-007 | DataPackage.SetData supports custom format identity | Yes | Source and target knowledge are required | Accepted with narrower claim |
| CLIP-EVID-008 | System Clipboard is cross-process; standard and registered formats differ | Yes | Privacy and user-driven boundary remain mandatory | Accepted |
| CLIP-EVID-009 | One window opens Clipboard at a time; ownership and delayed rendering are distinct | Yes | Contention and product retry remain unverified | Accepted |
| CLIP-EVID-010 | Multiple formats, conversions, DIB/DIBV5 and History/Cloud controls exist | Yes, split into separate claims | Fidelity, consumer support and privacy policy are not closed | Accepted with narrower claim |
| CLIP-EVID-011 | CF_BITMAP, CF_DIB and CF_DIBV5 have different identities | Yes | Alpha, stride and pixel round-trip remain open | Accepted |
| CLIP-EVID-012 | OleSetClipboard retains IDataObject and supports delayed rendering | Yes | COM apartment and shutdown path remain host questions | Accepted with narrower claim |
| CLIP-EVID-013 | COM distinguishes STA and MTA | Yes | Does not define a SnipPlus adapter | Accepted |
| CLIP-EVID-014 | STA objects need owning-thread message dispatch | Yes | Host Dispatcher behavior still needs validation | Accepted with narrower claim |
| CLIP-EVID-015 | WinForms has multi-format SetData and contention exception behavior | Informative only for candidate adapter | WinForms is not a selected SnipPlus Host decision | Accepted with narrower claim |
| CLIP-EVID-016 | Official Support states 4 MB History item limit and Bitmap support | Yes, support-level claim | Not an API or total-memory contract | Accepted with narrower claim |
| CLIP-EVID-017 | Official Support states 25 History entries and optional sync | Yes, support-level claim | User/account/settings dependent | Accepted with narrower claim |
| CLIP-EVID-018 | Registered Clipboard format identity is created by registration | Yes | Registration does not establish PNG consumer support | Accepted with narrower claim |

| Existing gap range | Reused gap meaning | Baseline treatment |
|---|---|---|
| CLIP-GAP-001..003 | Local, packaged and unpackaged availability/integration are not proven by API docs | Retained as open; official evidence narrows the claim only |
| CLIP-GAP-004..005 | PNG and multiple-format Consumer interoperability remain unknown | Retained as open; no Consumer evidence created |
| CLIP-GAP-006..008 | Alpha, pixel, color and HDR conversion fidelity remain unknown | Retained as open; no image payload created |
| CLIP-GAP-009..011 | Dispatcher, contention, ownership and shutdown behavior remain host/runtime questions | Retained as open |
| CLIP-GAP-012..014 | Large image, History/Cloud and privacy control behavior require more than static docs | Retained as open |
| CLIP-GAP-015..017 | File Output separation, cleanup and privacy-safe evidence need non-documentary evidence | Retained as open |
| CLIP-GAP-018 | Documentation cannot rank candidates | Retained as open; no ranking introduced |

## 7. Official Source Inventory

本節列出本文件實際使用的 Microsoft 第一方來源。所有來源的 Access date 為 2026-07-26。

| Source ID | Official source | Publisher / maintainer | URL | Use |
|---|---|---|---|---|
| CLIP-SRC-001 | Clipboard Class, System.Windows | Microsoft Learn / .NET | https://learn.microsoft.com/en-us/dotnet/api/system.windows.clipboard?view=windowsdesktop-10.0 | WPF API identity |
| CLIP-SRC-002 | Clipboard.SetDataObject Method, System.Windows | Microsoft Learn / .NET | https://learn.microsoft.com/en-us/dotnet/api/system.windows.clipboard.setdataobject?view=windowsdesktop-10.0 | WPF data object and exit retention |
| CLIP-SRC-003 | Clipboard.GetDataObject Method, System.Windows | Microsoft Learn / .NET | https://learn.microsoft.com/en-us/dotnet/api/system.windows.clipboard.getdataobject?view=windowsdesktop-10.0 | WPF IDataObject and cross-process change |
| CLIP-SRC-004 | Clipboard.SetDataObject Method, System.Windows.Forms | Microsoft Learn / .NET | https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.clipboard.setdataobject?view=netframework-4.8.1 | STA, exception and retry overload evidence |
| CLIP-SRC-005 | Clipboard Class, Windows.ApplicationModel.DataTransfer | Microsoft Learn / WinRT | https://learn.microsoft.com/en-us/uwp/api/windows.applicationmodel.datatransfer.clipboard?view=winrt-28000 | WinRT identity, foreground, History and Roaming APIs |
| CLIP-SRC-006 | Clipboard.SetContent(DataPackage) | Microsoft Learn / WinRT | https://learn.microsoft.com/en-us/uwp/api/windows.applicationmodel.datatransfer.clipboard.setcontent?view=winrt-28000 | Foreground, exception, History and Cloud eligibility |
| CLIP-SRC-007 | Clipboard.Flush | Microsoft Learn / WinRT | https://learn.microsoft.com/en-us/uwp/api/windows.applicationmodel.datatransfer.clipboard.flush?view=winrt-26100 | DataPackage release and post-shutdown availability |
| CLIP-SRC-008 | DataPackage Class | Microsoft Learn / WinRT | https://learn.microsoft.com/en-us/uwp/api/windows.applicationmodel.datatransfer.datapackage?view=winrt-28000 | Bitmap and custom data identity |
| CLIP-SRC-009 | StandardDataFormats Class | Microsoft Learn / WinRT | https://learn.microsoft.com/en-us/uwp/api/windows.applicationmodel.datatransfer.standarddataformats?view=winrt-22621 | Standard and legacy format mapping |
| CLIP-SRC-010 | ClipboardContentOptions.IsAllowedInHistory | Microsoft Learn / WinRT | https://learn.microsoft.com/en-us/uwp/api/windows.applicationmodel.datatransfer.clipboardcontentoptions.isallowedinhistory?view=winrt-28000 | History exclusion option boundary |
| CLIP-SRC-011 | Clipboard Operations | Microsoft Learn / Win32 | https://learn.microsoft.com/en-us/windows/win32/dataxchg/clipboard-operations | Open, owner, delayed rendering and messages |
| CLIP-SRC-012 | OpenClipboard | Microsoft Learn / Windows SDK | https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-openclipboard | Exclusivity, window owner and close responsibility |
| CLIP-SRC-013 | EmptyClipboard | Microsoft Learn / Windows SDK | https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-emptyclipboard | Ownership assignment and handle release |
| CLIP-SRC-014 | SetClipboardData | Microsoft Learn / Windows SDK | https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setclipboarddata | Native handle transfer and delayed rendering |
| CLIP-SRC-015 | CloseClipboard | Microsoft Learn / Windows SDK | https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-closeclipboard | End of exclusive access |
| CLIP-SRC-016 | Standard Clipboard Formats | Microsoft Learn / Win32 | https://learn.microsoft.com/en-us/windows/win32/dataxchg/standard-clipboard-formats | CF_BITMAP, CF_DIB and CF_DIBV5 identity |
| CLIP-SRC-017 | Clipboard Formats | Microsoft Learn / Win32 | https://learn.microsoft.com/en-us/windows/win32/dataxchg/clipboard-formats | Multiple formats, conversion and Cloud control formats |
| CLIP-SRC-018 | RegisterClipboardFormatW | Microsoft Learn / Windows SDK | https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-registerclipboardformatw | Registered format identity and HGLOBAL |
| CLIP-SRC-019 | OleInitialize | Microsoft Learn / OLE | https://learn.microsoft.com/en-us/windows/win32/api/ole2/nf-ole2-oleinitialize | OLE, COM and STA initialization |
| CLIP-SRC-020 | OleSetClipboard | Microsoft Learn / OLE | https://learn.microsoft.com/en-us/windows/win32/api/ole2/nf-ole2-olesetclipboard | IDataObject retention, delayed rendering and failure codes |
| CLIP-SRC-021 | OleFlushClipboard | Microsoft Learn / OLE | https://learn.microsoft.com/en-us/windows/win32/api/ole2/nf-ole2-oleflushclipboard | Rendering and release at shutdown |
| CLIP-SRC-022 | OleGetClipboard | Microsoft Learn / OLE | https://learn.microsoft.com/en-us/windows/win32/api/ole2/nf-ole2-olegetclipboard | Data object retrieval, RPC possibility and untrusted input |
| CLIP-SRC-023 | Dispatcher Class | Microsoft Learn / WPF | https://learn.microsoft.com/en-us/dotnet/api/system.windows.threading.dispatcher?view=windowsdesktop-10.0 | WPF thread affinity and shutdown |
| CLIP-SRC-024 | Keep the UI thread responsive | Microsoft Learn / Windows Apps | https://learn.microsoft.com/en-us/windows/apps/develop/performance/keep-ui-thread-responsive | WinUI 3 DispatcherQueue boundary |
| CLIP-SRC-025 | Using Windows Runtime objects in a multithreaded environment | Microsoft Learn / Windows Apps | https://learn.microsoft.com/en-us/windows/apps/develop/threading/winrt-objects-multithreaded | UI-thread access and object lifetime |
| CLIP-SRC-026 | Windows App SDK | Microsoft Learn / Windows Apps | https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/ | Existing WPF, Windows Forms and Win32 host boundary |
| CLIP-SRC-027 | Package and deploy Windows apps overview | Microsoft Learn / Windows Apps | https://learn.microsoft.com/en-us/windows/apps/package-and-deploy/ | Packaged and unpackaged deployment boundary |
| CLIP-SRC-028 | Using the clipboard | Microsoft Support | https://support.microsoft.com/en-us/windows/apps/using-the-clipboard | User settings, 4 MB item, 25 entries and sync |

## 8. Official Evidence Register

每筆 CLIP-OFF-EVID 都使用以下固定欄位：Evidence ID、Claim、Candidate、Host、Related Pair、Related Prerequisites、Related Blockers、Related Blocking Actions、Related Closure Actions、Related Enablement Items、Related Upstream Gates、Related Closure Gates、Official source title、Publisher/maintainer、URL、Publication/update date、Access date、API/Format/Interop identity、Platform requirement、Namespace/header/assembly、Managed/native boundary、COM/Apartment requirement、Dispatcher requirement、Packaging context、Data ownership implication、Process lifetime implication、History/Cloud implication、Privacy implication、Known limitation、Claim status、Static specification effect、Runtime validation still required。

### CLIP-OFF-EVID-001 — WPF Clipboard identity

- Claim：System.Windows.Clipboard 是 WPF PresentationCore 的 Clipboard API，提供 Get、Set、SetImage、SetData 與 SetDataObject 類型的資料轉移入口。
- Candidate：CLIP-OPT-001。
- Host：WPF。
- Related Pair：CLIP-PAIR-001；CLIP-PAIR-002。
- Related Prerequisites：CLIP-PREQ-001、CLIP-PREQ-002、CLIP-PREQ-003。
- Related Blockers：CLIP-BLOCK-001、CLIP-BLOCK-002。
- Related Blocking Actions：CLIP-BA-001。
- Related Closure Actions：CLIP-CLOSE-001。
- Related Enablement Items：CLIP-ENABLE-001。
- Related Upstream Gates：CLIP-GATE-001、CLIP-GATE-002。
- Related Closure Gates：CLIP-CGATE-001、CLIP-CGATE-002。
- Official source title：Clipboard Class, System.Windows。
- Publisher/maintainer：Microsoft Learn / .NET。
- URL：https://learn.microsoft.com/en-us/dotnet/api/system.windows.clipboard?view=windowsdesktop-10.0
- Publication/update date：頁面版本可能含 prerelease API；未以更新日期作為相容性結論。
- Access date：2026-07-26。
- API/Format/Interop identity：System.Windows.Clipboard；SetImage；SetData；SetDataObject。
- Platform requirement：Windows desktop / WPF。
- Namespace/header/assembly：System.Windows；PresentationCore.dll。
- Managed/native boundary：Managed WPF wrapper over system Clipboard。
- COM/Apartment requirement：本頁未直接關閉 WPF-specific apartment claim；保留為 runtime question。
- Dispatcher requirement：Clipboard API 頁面未直接證明 SnipPlus Dispatcher route。
- Packaging context：Packaged / unpackaged 未由本來源關閉。
- Data ownership implication：SetDataObject data object identity must be treated separately from format fidelity。
- Process lifetime implication：Default retention requires separate SetDataObject claim。
- History/Cloud implication：本來源未說明 History / Cloud policy。
- Privacy implication：System Clipboard is not a private application channel。
- Known limitation：API surface does not prove consumer interoperability。
- Claim status：Confirmed by official source。
- Static specification effect：可建立 WPF candidate identity，不可建立 technology selection。
- Runtime validation still required：Host invocation、STA/Dispatcher、format fidelity、consumer and shutdown behavior。

### CLIP-OFF-EVID-002 — WPF SetDataObject retention

- Claim：WPF SetDataObject(Object, Boolean) 的 Boolean 參數區分應用程式結束後是否保留 Clipboard data；預設 overload 為 non-persistent。
- Candidate：CLIP-OPT-001。
- Host：WPF。
- Related Pair：CLIP-PAIR-001。
- Related Prerequisites：CLIP-PREQ-004、CLIP-PREQ-005。
- Related Blockers：CLIP-BLOCK-003。
- Related Blocking Actions：CLIP-BA-001、CLIP-BA-003。
- Related Closure Actions：CLIP-CLOSE-001、CLIP-CLOSE-003。
- Related Enablement Items：CLIP-ENABLE-001、CLIP-ENABLE-003。
- Related Upstream Gates：CLIP-GATE-005、CLIP-GATE-008。
- Related Closure Gates：CLIP-CGATE-005、CLIP-CGATE-008。
- Official source title：Clipboard.SetDataObject Method, System.Windows。
- Publisher/maintainer：Microsoft Learn / .NET。
- URL：https://learn.microsoft.com/en-us/dotnet/api/system.windows.clipboard.setdataobject?view=windowsdesktop-10.0
- Publication/update date：未以日期作為產品相容性結論。
- Access date：2026-07-26。
- API/Format/Interop identity：SetDataObject(Object)；SetDataObject(Object, Boolean)。
- Platform requirement：Windows desktop / WPF。
- Namespace/header/assembly：System.Windows；PresentationCore.dll。
- Managed/native boundary：Managed data object passed to system Clipboard。
- COM/Apartment requirement：本來源未關閉。
- Dispatcher requirement：本來源未關閉。
- Packaging context：本來源未關閉。
- Data ownership implication：Boolean controls persistence semantics, not all native ownership rules。
- Process lifetime implication：Officially documented distinction between clearing and leaving data after application exit。
- History/Cloud implication：Persistence does not equal History or Cloud inclusion policy。
- Privacy implication：Leaving content after exit increases retention surface and requires product policy。
- Known limitation：No SnipPlus shutdown evidence。
- Claim status：Confirmed by official source。
- Static specification effect：可將 retention、History 與 Cloud 分成不同 requirement。
- Runtime validation still required：Shutdown、owner change、History and Cloud observations。

### CLIP-OFF-EVID-003 — WPF IDataObject access

- Claim：WPF GetDataObject returns an IDataObject representing the entire system Clipboard；Microsoft also states that the shared Clipboard may change by other applications。
- Candidate：CLIP-OPT-001。
- Host：WPF。
- Related Pair：CLIP-PAIR-001。
- Related Prerequisites：CLIP-PREQ-006、CLIP-PREQ-007。
- Related Blockers：CLIP-BLOCK-001、CLIP-BLOCK-004。
- Related Blocking Actions：CLIP-BA-001、CLIP-BA-004。
- Related Closure Actions：CLIP-CLOSE-001、CLIP-CLOSE-004。
- Related Enablement Items：CLIP-ENABLE-001、CLIP-ENABLE-004。
- Related Upstream Gates：CLIP-GATE-001、CLIP-GATE-009。
- Related Closure Gates：CLIP-CGATE-001、CLIP-CGATE-009。
- Official source title：Clipboard.GetDataObject Method, System.Windows。
- Publisher/maintainer：Microsoft Learn / .NET。
- URL：https://learn.microsoft.com/en-us/dotnet/api/system.windows.clipboard.getdataobject?view=windowsdesktop-10.0
- Publication/update date：未以日期作為產品相容性結論。
- Access date：2026-07-26。
- API/Format/Interop identity：System.Windows.IDataObject；GetFormats；GetData。
- Platform requirement：Windows desktop / WPF。
- Namespace/header/assembly：System.Windows；PresentationCore.dll。
- Managed/native boundary：Managed IDataObject over shared system resource。
- COM/Apartment requirement：本來源未關閉。
- Dispatcher requirement：本來源未關閉。
- Packaging context：本來源未關閉。
- Data ownership implication：Returned object represents current contents, not a stable private snapshot by default。
- Process lifetime implication：Returned data object lifetime is not a product lifetime contract。
- History/Cloud implication：No History or Cloud policy in this source。
- Privacy implication：Clipboard data is shared and must be treated as untrusted external input。
- Known limitation：No format round-trip or consumer result。
- Claim status：Confirmed by official source。
- Static specification effect：可建立 cross-process data and ownership boundary。
- Runtime validation still required：Read behavior、format enumeration、failure and shutdown。

### CLIP-OFF-EVID-004 — .NET STA and retry surface

- Claim：System.Windows.Forms.Clipboard official reference documents STA requirement, ExternalException when another process uses Clipboard, and a SetDataObject overload with retryTimes and retryDelay。
- Candidate：CLIP-OPT-005 as adapter reference; not a Host selection。
- Host：Desktop host reference only。
- Related Pair：CLIP-PAIR-009、CLIP-PAIR-010。
- Related Prerequisites：CLIP-PREQ-008、CLIP-PREQ-009、CLIP-PREQ-010。
- Related Blockers：CLIP-BLOCK-005、CLIP-BLOCK-006。
- Related Blocking Actions：CLIP-BA-002、CLIP-BA-004。
- Related Closure Actions：CLIP-CLOSE-002、CLIP-CLOSE-004。
- Related Enablement Items：CLIP-ENABLE-002、CLIP-ENABLE-004。
- Related Upstream Gates：CLIP-GATE-003、CLIP-GATE-004。
- Related Closure Gates：CLIP-CGATE-003、CLIP-CGATE-004。
- Official source title：Clipboard.SetDataObject Method, System.Windows.Forms。
- Publisher/maintainer：Microsoft Learn / .NET。
- URL：https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.clipboard.setdataobject?view=netframework-4.8.1
- Publication/update date：.NET Framework API reference；not a SnipPlus host decision。
- Access date：2026-07-26。
- API/Format/Interop identity：System.Windows.Forms.Clipboard；SetDataObject(Object, Boolean, Int32, Int32)。
- Platform requirement：Windows desktop / Windows Forms reference surface。
- Namespace/header/assembly：System.Windows.Forms；System.Windows.Forms.dll。
- Managed/native boundary：Managed wrapper over native Clipboard operation。
- COM/Apartment requirement：Current thread must be STA。
- Dispatcher requirement：This source does not define WPF Dispatcher or WinUI DispatcherQueue。
- Packaging context：Not closed。
- Data ownership implication：Serializable data requirement and format-specific behavior are separate concerns。
- Process lifetime implication：copy Boolean and retry overload are separate fields。
- History/Cloud implication：Not defined。
- Privacy implication：Not defined by API reference。
- Known limitation：WinForms behavior cannot be copied into WPF or WinUI without host evidence。
- Claim status：Confirmed by official source。
- Static specification effect：Can define retry as a candidate capability, not a product policy。
- Runtime validation still required：Host-specific STA, contention, cancellation, UI responsiveness and cleanup。

### CLIP-OFF-EVID-005 — WinRT Clipboard identity and foreground requirement

- Claim：Windows.ApplicationModel.DataTransfer.Clipboard is a WinRT static class with SetContent, GetContent, Flush, History and Roaming APIs；the official page states that Clipboard access requires the calling application to be in focus on the UI thread。
- Candidate：CLIP-OPT-002。
- Host：WinUI 3 and WinRT-projection desktop host。
- Related Pair：CLIP-PAIR-003、CLIP-PAIR-004。
- Related Prerequisites：CLIP-PREQ-011、CLIP-PREQ-012、CLIP-PREQ-013。
- Related Blockers：CLIP-BLOCK-007、CLIP-BLOCK-008。
- Related Blocking Actions：CLIP-BA-002、CLIP-BA-005。
- Related Closure Actions：CLIP-CLOSE-002、CLIP-CLOSE-005。
- Related Enablement Items：CLIP-ENABLE-002、CLIP-ENABLE-005。
- Related Upstream Gates：CLIP-GATE-001、CLIP-GATE-003、CLIP-GATE-008。
- Related Closure Gates：CLIP-CGATE-001、CLIP-CGATE-003、CLIP-CGATE-008。
- Official source title：Clipboard Class, Windows.ApplicationModel.DataTransfer。
- Publisher/maintainer：Microsoft Learn / WinRT。
- URL：https://learn.microsoft.com/en-us/uwp/api/windows.applicationmodel.datatransfer.clipboard?view=winrt-28000
- Publication/update date：Version history documents Windows 10 version 1809 additions for History and Roaming APIs。
- Access date：2026-07-26。
- API/Format/Interop identity：Clipboard；SetContent；SetContentWithOptions；Flush；IsHistoryEnabled；IsRoamingEnabled。
- Platform requirement：Windows 10 UniversalApiContract v1.0 or later for the documented class。
- Namespace/header/assembly：Windows.ApplicationModel.DataTransfer。
- Managed/native boundary：WinRT projection; class metadata says standard marshaling and, in current view, ThreadingModel.Both。
- COM/Apartment requirement：Class metadata does not remove host UI and foreground requirement。
- Dispatcher requirement：Official page requires focus and UI thread for access。
- Packaging context：Desktop packaged/unpackaged host route remains unverified。
- Data ownership implication：DataPackage may be released by Flush; content operation has distinct lifetime semantics。
- Process lifetime implication：Flush allows content to remain after source app shutdown。
- History/Cloud implication：History and Roaming are exposed as OS/user state and APIs。
- Privacy implication：SetContent content may be eligible for History and synchronization。
- Known limitation：API availability does not prove current Host projection or package integration。
- Claim status：Confirmed by official source。
- Static specification effect：Must separate foreground/UI invocation, History, Roaming and lifetime requirements。
- Runtime validation still required：WPF/WinUI host route, foreground state, shutdown, History and Cloud behavior。

### CLIP-OFF-EVID-006 — WinRT SetContent and History/Cloud eligibility

- Claim：SetContent requires foreground use or debugger attachment; failure may throw; content is eligible for Clipboard History and device synchronization unless options are used。
- Candidate：CLIP-OPT-002。
- Host：WinUI 3 and WinRT-projection desktop host。
- Related Pair：CLIP-PAIR-003、CLIP-PAIR-004。
- Related Prerequisites：CLIP-PREQ-014、CLIP-PREQ-015、CLIP-PREQ-016。
- Related Blockers：CLIP-BLOCK-008、CLIP-BLOCK-009。
- Related Blocking Actions：CLIP-BA-005、CLIP-BA-006。
- Related Closure Actions：CLIP-CLOSE-005、CLIP-CLOSE-006。
- Related Enablement Items：CLIP-ENABLE-005、CLIP-ENABLE-006。
- Related Upstream Gates：CLIP-GATE-004、CLIP-GATE-008、CLIP-GATE-010。
- Related Closure Gates：CLIP-CGATE-004、CLIP-CGATE-008、CLIP-CGATE-010。
- Official source title：Clipboard.SetContent(DataPackage) Method。
- Publisher/maintainer：Microsoft Learn / WinRT。
- URL：https://learn.microsoft.com/en-us/uwp/api/windows.applicationmodel.datatransfer.clipboard.setcontent?view=winrt-28000
- Publication/update date：未以日期作為 product behavior 結論。
- Access date：2026-07-26。
- API/Format/Interop identity：Clipboard.SetContent(DataPackage)；Clipboard.SetContentWithOptions。
- Platform requirement：Windows Runtime Clipboard。
- Namespace/header/assembly：Windows.ApplicationModel.DataTransfer。
- Managed/native boundary：WinRT DataPackage projection。
- COM/Apartment requirement：Not separately defined by this method page。
- Dispatcher requirement：Foreground and application focus rule applies。
- Packaging context：Packaged/unpackaged availability remains a local/project question。
- Data ownership implication：DataPackage options influence system retention policy, not application ownership of every native representation。
- Process lifetime implication：Use Flush as a distinct process-lifetime path; do not infer it from SetContent alone。
- History/Cloud implication：Eligible by default; options can exclude。
- Privacy implication：A successful call can create local History or cross-device exposure。
- Known limitation：Documentation does not validate image format, size, or consumer support。
- Claim status：Confirmed by official source。
- Static specification effect：Clipboard Write and privacy controls must be separate authorization items。
- Runtime validation still required：Foreground failure, options, History, Cloud, format and shutdown。

### CLIP-OFF-EVID-007 — WinRT DataPackage and Bitmap representation

- Claim：DataPackage supports SetBitmap(RandomAccessStreamReference), SetData, SetHtmlFormat, SetRtf and StorageItems；StandardDataFormats.Bitmap supplies a format ID。
- Candidate：CLIP-OPT-002。
- Host：WinUI 3 and WinRT-projection desktop host。
- Related Pair：CLIP-PAIR-003、CLIP-PAIR-004。
- Related Prerequisites：CLIP-PREQ-017、CLIP-PREQ-018。
- Related Blockers：CLIP-BLOCK-007、CLIP-BLOCK-010。
- Related Blocking Actions：CLIP-BA-002、CLIP-BA-005。
- Related Closure Actions：CLIP-CLOSE-002、CLIP-CLOSE-005。
- Related Enablement Items：CLIP-ENABLE-002、CLIP-ENABLE-005。
- Related Upstream Gates：CLIP-GATE-002、CLIP-GATE-009。
- Related Closure Gates：CLIP-CGATE-002、CLIP-CGATE-009。
- Official source title：DataPackage Class and StandardDataFormats.Bitmap Property。
- Publisher/maintainer：Microsoft Learn / WinRT。
- URL：https://learn.microsoft.com/en-us/uwp/api/windows.applicationmodel.datatransfer.datapackage?view=winrt-28000 ; https://learn.microsoft.com/en-us/uwp/api/windows.applicationmodel.datatransfer.standarddataformats.bitmap?view=winrt-26100
- Publication/update date：未以日期作為 product fidelity 結論。
- Access date：2026-07-26。
- API/Format/Interop identity：DataPackage；SetBitmap(RandomAccessStreamReference)；StandardDataFormats.Bitmap。
- Platform requirement：Windows Runtime UniversalApiContract。
- Namespace/header/assembly：Windows.ApplicationModel.DataTransfer。
- Managed/native boundary：WinRT DataPackage and RandomAccessStreamReference boundary。
- COM/Apartment requirement：Not closed for each Host pair。
- Dispatcher requirement：DataPackage class identity does not prove the required Clipboard invocation thread。
- Packaging context：UWP/WinRT format identity does not close desktop package mode。
- Data ownership implication：Stream reference lifetime and materialization remain separate from DataPackage object identity。
- Process lifetime implication：Flush has separate documented post-shutdown semantics。
- History/Cloud implication：SetContent policy applies; DataPackage format alone does not control it。
- Privacy implication：Bitmap content can be eligible for system retention depending on publication path。
- Known limitation：Bitmap representation is not a claim about PNG byte publication or alpha fidelity。
- Claim status：Confirmed by official source。
- Static specification effect：Bitmap, PNG, DIB and DIBV5 must remain separate format rows。
- Runtime validation still required：Stream, format, consumer, History and shutdown behavior。

### CLIP-OFF-EVID-008 — Win32 Clipboard exclusivity and ownership

- Claim：OpenClipboard provides exclusive access; only one window can have Clipboard open; EmptyClipboard assigns ownership to the window currently holding it; CloseClipboard releases access。
- Candidate：CLIP-OPT-004。
- Host：Win32 desktop and native adapter boundary。
- Related Pair：CLIP-PAIR-007、CLIP-PAIR-008。
- Related Prerequisites：CLIP-PREQ-019、CLIP-PREQ-020、CLIP-PREQ-021。
- Related Blockers：CLIP-BLOCK-004、CLIP-BLOCK-005、CLIP-BLOCK-011。
- Related Blocking Actions：CLIP-BA-004。
- Related Closure Actions：CLIP-CLOSE-004、CLIP-CLOSE-005。
- Related Enablement Items：CLIP-ENABLE-004、CLIP-ENABLE-005。
- Related Upstream Gates：CLIP-GATE-003、CLIP-GATE-004、CLIP-GATE-005。
- Related Closure Gates：CLIP-CGATE-003、CLIP-CGATE-004、CLIP-CGATE-005。
- Official source title：OpenClipboard, EmptyClipboard, CloseClipboard and Clipboard Operations。
- Publisher/maintainer：Microsoft Learn / Windows SDK。
- URL：https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-openclipboard ; https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-emptyclipboard ; https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-closeclipboard ; https://learn.microsoft.com/en-us/windows/win32/dataxchg/clipboard-operations
- Publication/update date：OpenClipboard、EmptyClipboard、CloseClipboard pages last updated 2024-02-22。
- Access date：2026-07-26。
- API/Format/Interop identity：OpenClipboard；EmptyClipboard；CloseClipboard；GetClipboardOwner。
- Platform requirement：Windows desktop；User32.dll。
- Namespace/header/assembly：winuser.h；User32.lib / User32.dll。
- Managed/native boundary：Native window handle and User32 transaction。
- COM/Apartment requirement：Raw User32 operation does not itself establish COM initialization。
- Dispatcher requirement：Window/message ownership and host message processing still matter。
- Packaging context：Desktop package mode not closed by these APIs。
- Data ownership implication：Owner assignment occurs at EmptyClipboard; access and ownership are distinct events。
- Process lifetime implication：Owner window closure can affect delayed rendering and remaining formats。
- History/Cloud implication：Not defined by open/empty/close alone。
- Privacy implication：All applications can access system Clipboard。
- Known limitation：No product retry, timeout or cleanup policy。
- Claim status：Confirmed by official source。
- Static specification effect：Must model open contention, owner, close and publication as separate states。
- Runtime validation still required：Contention, failure, owner change, shutdown and cleanup。

### CLIP-OFF-EVID-009 — Native handle ownership transfer

- Claim：When SetClipboardData succeeds, the system owns hMem; an application must not free or write the data after transfer, and movable global memory is required for HGLOBAL data。
- Candidate：CLIP-OPT-004。
- Host：Win32 desktop and native adapter boundary。
- Related Pair：CLIP-PAIR-007、CLIP-PAIR-008。
- Related Prerequisites：CLIP-PREQ-022、CLIP-PREQ-023。
- Related Blockers：CLIP-BLOCK-011、CLIP-BLOCK-012。
- Related Blocking Actions：CLIP-BA-004、CLIP-BA-005。
- Related Closure Actions：CLIP-CLOSE-004、CLIP-CLOSE-005。
- Related Enablement Items：CLIP-ENABLE-004、CLIP-ENABLE-005。
- Related Upstream Gates：CLIP-GATE-002、CLIP-GATE-005。
- Related Closure Gates：CLIP-CGATE-002、CLIP-CGATE-005。
- Official source title：SetClipboardData function。
- Publisher/maintainer：Microsoft Learn / Windows SDK。
- URL：https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setclipboarddata
- Publication/update date：Last updated 2021-10-13。
- Access date：2026-07-26。
- API/Format/Interop identity：SetClipboardData(UINT, HANDLE)；HGLOBAL；GMEM_MOVEABLE。
- Platform requirement：Windows desktop；User32.dll。
- Namespace/header/assembly：winuser.h；User32.lib / User32.dll。
- Managed/native boundary：Native memory handle ownership transfer。
- COM/Apartment requirement：Not inherently a COM API。
- Dispatcher requirement：Delayed rendering depends on owner window message processing。
- Packaging context：Not closed。
- Data ownership implication：Ownership moves to the system after successful publication。
- Process lifetime implication：Delayed rendering can retain owner responsibility until rendered or released。
- History/Cloud implication：Separate registered control formats may affect History / Cloud inclusion。
- Privacy implication：System-owned data remains available to other processes according to format and OS policy。
- Known limitation：No alpha, stride or consumer fidelity guarantee。
- Claim status：Confirmed by official source。
- Static specification effect：Native memory release and ownership cannot be hidden inside generic adapter assumptions。
- Runtime validation still required：Allocation, publication, consumer, owner change, shutdown and cleanup。

### CLIP-OFF-EVID-010 — Delayed rendering and message boundary

- Claim：Windows supports delayed rendering by passing NULL to SetClipboardData; owner must respond to WM_RENDERFORMAT and WM_RENDERALLFORMATS with different open/close rules。
- Candidate：CLIP-OPT-003、CLIP-OPT-004。
- Host：WPF/WinUI 3/native desktop host through interop。
- Related Pair：CLIP-PAIR-005、CLIP-PAIR-006、CLIP-PAIR-007、CLIP-PAIR-008。
- Related Prerequisites：CLIP-PREQ-024、CLIP-PREQ-025。
- Related Blockers：CLIP-BLOCK-006、CLIP-BLOCK-011。
- Related Blocking Actions：CLIP-BA-003、CLIP-BA-004。
- Related Closure Actions：CLIP-CLOSE-003、CLIP-CLOSE-004。
- Related Enablement Items：CLIP-ENABLE-003、CLIP-ENABLE-004。
- Related Upstream Gates：CLIP-GATE-005、CLIP-GATE-009。
- Related Closure Gates：CLIP-CGATE-005、CLIP-CGATE-009。
- Official source title：Clipboard Operations。
- Publisher/maintainer：Microsoft Learn / Win32。
- URL：https://learn.microsoft.com/en-us/windows/win32/dataxchg/clipboard-operations
- Publication/update date：Published 2022-05-26。
- Access date：2026-07-26。
- API/Format/Interop identity：SetClipboardData(NULL)；WM_RENDERFORMAT；WM_RENDERALLFORMATS。
- Platform requirement：Windows desktop window procedure。
- Namespace/header/assembly：winuser.h / User32。
- Managed/native boundary：Native message and format rendering boundary。
- COM/Apartment requirement：OLE adapter may add COM requirements; raw delayed rendering does not remove message requirements。
- Dispatcher requirement：Owner must process messages while it owns delayed formats。
- Packaging context：Not closed。
- Data ownership implication：Owner keeps responsibility for unrendered formats。
- Process lifetime implication：Owner destruction requires render-all handling if formats are to survive。
- History/Cloud implication：Unrendered or unmaterialized formats may not be available for later consumers。
- Privacy implication：Data may be rendered on demand by another process。
- Known limitation：Does not establish a safe product implementation or retry schedule。
- Claim status：Confirmed by official source。
- Static specification effect：Immediate and delayed rendering are separate capability paths。
- Runtime validation still required：Message pump, process close, consumer requests and failure cleanup。

### CLIP-OFF-EVID-011 — Standard image format identity

- Claim：CF_BITMAP is an HBITMAP; CF_DIB is a BITMAPINFO followed by bits; CF_DIBV5 is a BITMAPV5HEADER followed by color-space information and bits。
- Candidate：CLIP-OPT-004。
- Host：Win32 desktop and native adapter boundary。
- Related Pair：CLIP-PAIR-007、CLIP-PAIR-008。
- Related Prerequisites：CLIP-PREQ-026、CLIP-PREQ-027。
- Related Blockers：CLIP-BLOCK-012、CLIP-BLOCK-013。
- Related Blocking Actions：CLIP-BA-004、CLIP-BA-005。
- Related Closure Actions：CLIP-CLOSE-004、CLIP-CLOSE-005。
- Related Enablement Items：CLIP-ENABLE-004、CLIP-ENABLE-005。
- Related Upstream Gates：CLIP-GATE-002、CLIP-GATE-009。
- Related Closure Gates：CLIP-CGATE-002、CLIP-CGATE-009。
- Official source title：Standard Clipboard Formats。
- Publisher/maintainer：Microsoft Learn / Windows SDK。
- URL：https://learn.microsoft.com/en-us/windows/win32/dataxchg/standard-clipboard-formats
- Publication/update date：Last updated 2020-12-11。
- Access date：2026-07-26。
- API/Format/Interop identity：CF_BITMAP=2；CF_DIB=8；CF_DIBV5=17。
- Platform requirement：Windows desktop；Winuser.h。
- Namespace/header/assembly：winuser.h；User32/GDI boundary。
- Managed/native boundary：HBITMAP or movable memory object。
- COM/Apartment requirement：Not inherent to format definition。
- Dispatcher requirement：Consumer and owner message processing remain separate。
- Packaging context：Not closed。
- Data ownership implication：Handle type and memory representation differ by format。
- Process lifetime implication：Publication and system ownership must be handled per native API。
- History/Cloud implication：Format identity alone does not specify History behavior。
- Privacy implication：Format does not make the content private。
- Known limitation：Alpha, premultiplication, stride and consumer round-trip are not closed by identity alone。
- Claim status：Confirmed by official source。
- Static specification effect：CF_BITMAP, CF_DIB and CF_DIBV5 must never be collapsed into one payload row。
- Runtime validation still required：Synthetic image fidelity, consumer acceptance and conversion behavior。

### CLIP-OFF-EVID-012 — DIB conversion and color boundary

- Claim：Microsoft documents that CF_DIB and CF_DIBV5 are preferred over device-dependent CF_BITMAP for bitmap copying and describes palette/color-space conversion behavior。
- Candidate：CLIP-OPT-004。
- Host：Win32 desktop and native adapter boundary。
- Related Pair：CLIP-PAIR-007、CLIP-PAIR-008。
- Related Prerequisites：CLIP-PREQ-026、CLIP-PREQ-027、CLIP-PREQ-028。
- Related Blockers：CLIP-BLOCK-012、CLIP-BLOCK-013。
- Related Blocking Actions：CLIP-BA-004、CLIP-BA-005。
- Related Closure Actions：CLIP-CLOSE-004、CLIP-CLOSE-005。
- Related Enablement Items：CLIP-ENABLE-004、CLIP-ENABLE-005。
- Related Upstream Gates：CLIP-GATE-002。
- Related Closure Gates：CLIP-CGATE-002。
- Official source title：Clipboard Formats。
- Publisher/maintainer：Microsoft Learn / Win32。
- URL：https://learn.microsoft.com/en-us/windows/win32/dataxchg/clipboard-formats
- Publication/update date：未以日期作為 product fidelity 結論。
- Access date：2026-07-26。
- API/Format/Interop identity：CF_BITMAP、CF_DIB、CF_DIBV5；palette and color-space conversion。
- Platform requirement：Windows desktop Clipboard format system。
- Namespace/header/assembly：Winuser.h; BITMAPINFO; BITMAPV5HEADER。
- Managed/native boundary：Native bitmap and color conversion boundary。
- COM/Apartment requirement：Not inherent to format definition。
- Dispatcher requirement：Not defined by format documentation。
- Packaging context：Not closed。
- Data ownership implication：Conversion may materialize another format at request time。
- Process lifetime implication：Clipboard close can trigger documented conversion behavior。
- History/Cloud implication：History/Cloud formats are documented separately from pixel fidelity。
- Privacy implication：Color metadata does not change cross-process visibility。
- Known limitation：No complete alpha/premultiplied/pixel guarantee for every consumer。
- Claim status：Partially confirmed。
- Static specification effect：Color and format fidelity remain independent gates。
- Runtime validation still required：Round-trip pixel comparison and target consumer observations。

### CLIP-OFF-EVID-013 — Registered format identity

- Claim：RegisterClipboardFormatW creates or reuses a named registered format; registered formats are identified by a value in 0xC000 through 0xFFFF and are represented as HGLOBAL when placed on the Clipboard。
- Candidate：CLIP-OPT-003、CLIP-OPT-004。
- Host：Native desktop adapter。
- Related Pair：CLIP-PAIR-005、CLIP-PAIR-006、CLIP-PAIR-007、CLIP-PAIR-008。
- Related Prerequisites：CLIP-PREQ-029。
- Related Blockers：CLIP-BLOCK-012۔
- Related Blocking Actions：CLIP-BA-004۔
- Related Closure Actions：CLIP-CLOSE-004۔
- Related Enablement Items：CLIP-ENABLE-004۔
- Related Upstream Gates：CLIP-GATE-002、CLIP-GATE-009۔
- Related Closure Gates：CLIP-CGATE-002、CLIP-CGATE-009۔
- Official source title：RegisterClipboardFormatW function。
- Publisher/maintainer：Microsoft Learn / Windows SDK。
- URL：https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-registerclipboardformatw
- Publication/update date：Last updated 2024-02-22。
- Access date：2026-07-26。
- API/Format/Interop identity：RegisterClipboardFormatW；GetClipboardFormatName；HGLOBAL。
- Platform requirement：Windows desktop；User32.dll。
- Namespace/header/assembly：winuser.h；User32.lib / User32.dll。
- Managed/native boundary：Native format ID and HGLOBAL payload boundary。
- COM/Apartment requirement：Not inherent to registration。
- Dispatcher requirement：Not defined by registration。
- Packaging context：Not closed。
- Data ownership implication：Registered identity does not define bytes, encoding or consumer contract。
- Process lifetime implication：HGLOBAL ownership follows publication API, not name registration。
- History/Cloud implication：Special registered names may control History / Cloud monitoring, but this does not apply to every custom name。
- Privacy implication：A registered format can still be read by a compatible process。
- Known limitation：Official docs do not establish a universal PNG registered-format consumer path。
- Claim status：Confirmed by official source。
- Static specification effect：PNG registered format remains a candidate row with open Consumer gap。
- Runtime validation still required：Registration, publication, consumer recognition and History/Cloud control behavior。

### CLIP-OFF-EVID-014 — OLE COM initialization

- Claim：OleInitialize must be called before OLE Clipboard functions; it initializes COM on the current apartment and specifies single-threaded apartment behavior。
- Candidate：CLIP-OPT-003。
- Host：WPF/WinUI 3/native desktop host through OLE interop。
- Related Pair：CLIP-PAIR-005、CLIP-PAIR-006。
- Related Prerequisites：CLIP-PREQ-008、CLIP-PREQ-030。
- Related Blockers：CLIP-BLOCK-005、CLIP-BLOCK-006۔
- Related Blocking Actions：CLIP-BA-003۔
- Related Closure Actions：CLIP-CLOSE-003۔
- Related Enablement Items：CLIP-ENABLE-003۔
- Related Upstream Gates：CLIP-GATE-003、CLIP-GATE-005۔
- Related Closure Gates：CLIP-CGATE-003、CLIP-CGATE-005۔
- Official source title：OleInitialize function。
- Publisher/maintainer：Microsoft Learn / OLE。
- URL：https://learn.microsoft.com/en-us/windows/win32/api/ole2/nf-ole2-oleinitialize
- Publication/update date：Last updated 2021-10-13。
- Access date：2026-07-26。
- API/Format/Interop identity：OleInitialize；CoInitializeEx；OleUninitialize；COINIT_APARTMENTTHREADED。
- Platform requirement：Windows desktop OLE。
- Namespace/header/assembly：ole2.h；Ole32.lib / Ole32.dll。
- Managed/native boundary：Native COM apartment boundary。
- COM/Apartment requirement：STA is required for the OLE initialization path; changing an existing MTA apartment fails with RPC_E_CHANGED_MODE。
- Dispatcher requirement：OLE operations and message loop behavior remain host responsibilities。
- Packaging context：Desktop packaging route not closed。
- Data ownership implication：COM initialization enables IDataObject and OLE Clipboard semantics。
- Process lifetime implication：Balanced OleUninitialize is required; shutdown sequencing remains open。
- History/Cloud implication：Not defined by OleInitialize。
- Privacy implication：Not defined by initialization; Clipboard remains cross-process。
- Known limitation：Initialization evidence is not a runtime proof for a selected Host。
- Claim status：Confirmed by official source。
- Static specification effect：OLE adapter must declare apartment and lifecycle prerequisites。
- Runtime validation still required：STA thread, message pump, reentrancy, shutdown and failure handling。

### CLIP-OFF-EVID-015 — OLE delayed rendering and ownership

- Claim：OleSetClipboard accepts IDataObject, increases its reference count for delayed rendering, assigns ownership to an internal OLE window and reports clipboard-open/empty/close/set failure HRESULTs。
- Candidate：CLIP-OPT-003。
- Host：WPF/WinUI 3/native desktop host through OLE interop。
- Related Pair：CLIP-PAIR-005、CLIP-PAIR-006。
- Related Prerequisites：CLIP-PREQ-022、CLIP-PREQ-024、CLIP-PREQ-030。
- Related Blockers：CLIP-BLOCK-005、CLIP-BLOCK-006、CLIP-BLOCK-011。
- Related Blocking Actions：CLIP-BA-003、CLIP-BA-004。
- Related Closure Actions：CLIP-CLOSE-003、CLIP-CLOSE-004。
- Related Enablement Items：CLIP-ENABLE-003、CLIP-ENABLE-004。
- Related Upstream Gates：CLIP-GATE-004、CLIP-GATE-005。
- Related Closure Gates：CLIP-CGATE-004、CLIP-CGATE-005。
- Official source title：OleSetClipboard function。
- Publisher/maintainer：Microsoft Learn / OLE。
- URL：https://learn.microsoft.com/en-us/windows/win32/api/ole2/nf-ole2-olesetclipboard
- Publication/update date：Last updated 2021-10-13。
- Access date：2026-07-26。
- API/Format/Interop identity：OleSetClipboard；IDataObject；CLIPBRD_E_CANT_OPEN；CLIPBRD_E_CANT_EMPTY；CLIPBRD_E_CANT_CLOSE；CLIPBRD_E_CANT_SET。
- Platform requirement：Windows desktop OLE。
- Namespace/header/assembly：ole2.h；Ole32.lib / Ole32.dll。
- Managed/native boundary：COM IDataObject and internal OLE window。
- COM/Apartment requirement：Requires compatible OLE/COM apartment。
- Dispatcher requirement：Delayed rendering is serviced through internal OLE window messages。
- Packaging context：Not closed。
- Data ownership implication：Reference count remains until OleFlushClipboard or OleSetClipboard(NULL)。
- Process lifetime implication：Source process lifetime affects delayed rendering until data is flushed。
- History/Cloud implication：Not defined by this API page。
- Privacy implication：IDataObject may expose data to another process through OLE transfer。
- Known limitation：HRESULT mapping does not define retry count, timeout or cancellation policy。
- Claim status：Confirmed by official source。
- Static specification effect：Ownership, delay, failure and clear are separate authorization boundaries。
- Runtime validation still required：Contention, render request, owner release and shutdown。

### CLIP-OFF-EVID-016 — OLE flush and post-shutdown availability

- Claim：OleFlushClipboard renders the IDataObject onto the Clipboard and releases the IDataObject pointer so data can remain available after source application shutdown。
- Candidate：CLIP-OPT-003。
- Host：WPF/WinUI 3/native desktop host through OLE interop。
- Related Pair：CLIP-PAIR-005、CLIP-PAIR-006。
- Related Prerequisites：CLIP-PREQ-024、CLIP-PREQ-031。
- Related Blockers：CLIP-BLOCK-006、CLIP-BLOCK-011。
- Related Blocking Actions：CLIP-BA-003、CLIP-BA-006。
- Related Closure Actions：CLIP-CLOSE-003、CLIP-CLOSE-006。
- Related Enablement Items：CLIP-ENABLE-003、CLIP-ENABLE-006。
- Related Upstream Gates：CLIP-GATE-005、CLIP-GATE-008。
- Related Closure Gates：CLIP-CGATE-005、CLIP-CGATE-008。
- Official source title：OleFlushClipboard function。
- Publisher/maintainer：Microsoft Learn / OLE。
- URL：https://learn.microsoft.com/en-us/windows/win32/api/ole2/nf-ole2-oleflushclipboard
- Publication/update date：Last updated 2021-06-29。
- Access date：2026-07-26。
- API/Format/Interop identity：OleFlushClipboard；IDataObject release；rendered HGLOBAL media。
- Platform requirement：Windows desktop OLE。
- Namespace/header/assembly：ole2.h；Ole32.lib / Ole32.dll。
- Managed/native boundary：OLE materialization and native Clipboard storage。
- COM/Apartment requirement：OLE/COM lifecycle remains required。
- Dispatcher requirement：Shutdown sequencing must allow OLE operation to complete。
- Packaging context：Not closed。
- Data ownership implication：Flush releases the IDataObject pointer held by Clipboard。
- Process lifetime implication：Post-shutdown availability is documented for flushed formats。
- History/Cloud implication：Materialization does not itself decide History / Cloud policy。
- Privacy implication：Data can outlive the source process and remain cross-process visible。
- Known limitation：Does not prove every advertised format renders successfully。
- Claim status：Confirmed by official source。
- Static specification effect：Flush is a distinct lifetime operation; it is not equivalent to Write or Clear。
- Runtime validation still required：Shutdown race, rendered formats, failure and consumer access。

### CLIP-OFF-EVID-017 — OLE read and untrusted data boundary

- Claim：OleGetClipboard returns an IDataObject; while the source application is running it may forward calls to the original object and potentially make RPC calls; Microsoft warns Clipboard data is not trusted。
- Candidate：CLIP-OPT-003。
- Host：WPF/WinUI 3/native desktop consumer boundary。
- Related Pair：CLIP-PAIR-005、CLIP-PAIR-006。
- Related Prerequisites：CLIP-PREQ-006、CLIP-PREQ-032。
- Related Blockers：CLIP-BLOCK-004、CLIP-BLOCK-010。
- Related Blocking Actions：CLIP-BA-004、CLIP-BA-006。
- Related Closure Actions：CLIP-CLOSE-004、CLIP-CLOSE-006。
- Related Enablement Items：CLIP-ENABLE-004、CLIP-ENABLE-006。
- Related Upstream Gates：CLIP-GATE-001、CLIP-GATE-009。
- Related Closure Gates：CLIP-CGATE-001、CLIP-CGATE-009。
- Official source title：OleGetClipboard function。
- Publisher/maintainer：Microsoft Learn / OLE。
- URL：https://learn.microsoft.com/en-us/windows/win32/api/ole2/nf-ole2-olegetclipboard
- Publication/update date：未以日期作為 product security 結論。
- Access date：2026-07-26。
- API/Format/Interop identity：OleGetClipboard；IDataObject；FORMATETC；TYMED。
- Platform requirement：Windows desktop OLE。
- Namespace/header/assembly：ole2.h；Ole32.lib / Ole32.dll。
- Managed/native boundary：COM IDataObject and possible cross-process RPC。
- COM/Apartment requirement：OLE/COM consumer context required。
- Dispatcher requirement：Message and process lifetime can affect forwarded calls。
- Packaging context：Not closed。
- Data ownership implication：Returned object can consume resources in the offering application。
- Process lifetime implication：Source process state changes the returned data-object path。
- History/Cloud implication：No History policy in this source。
- Privacy implication：Clipboard input is untrusted and must be parsed carefully。
- Known limitation：No product consumer contract or security review completed。
- Claim status：Confirmed by official source。
- Static specification effect：Clipboard read and untrusted-input handling require separate boundary。
- Runtime validation still required：Read, cancellation, source shutdown, malicious-format handling and resource cleanup。

### CLIP-OFF-EVID-018 — Windows App SDK host and packaging boundary

- Claim：Windows App SDK can be added to existing WPF, Windows Forms and Win32 apps; official deployment documentation distinguishes packaged, packaged-with-external-location and unpackaged models, including framework-dependent and self-contained runtime modes。
- Candidate：CLIP-OPT-001..005。
- Host：WPF and WinUI 3 comparison boundary。
- Related Pair：CLIP-PAIR-001..010。
- Related Prerequisites：CLIP-PREQ-001、CLIP-PREQ-011、CLIP-PREQ-013。
- Related Blockers：CLIP-BLOCK-001、CLIP-BLOCK-002、CLIP-BLOCK-007。
- Related Blocking Actions：CLIP-BA-001、CLIP-BA-002、CLIP-BA-005。
- Related Closure Actions：CLIP-CLOSE-001、CLIP-CLOSE-002、CLIP-CLOSE-005。
- Related Enablement Items：CLIP-ENABLE-001、CLIP-ENABLE-002、CLIP-ENABLE-005。
- Related Upstream Gates：CLIP-GATE-006、CLIP-GATE-007。
- Related Closure Gates：CLIP-CGATE-006、CLIP-CGATE-007。
- Official source title：Windows App SDK and Package and deploy Windows apps overview。
- Publisher/maintainer：Microsoft Learn / Windows Apps。
- URL：https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/ ; https://learn.microsoft.com/en-us/windows/apps/package-and-deploy/
- Publication/update date：Package overview last updated 2026-05-29。
- Access date：2026-07-26。
- API/Format/Interop identity：Windows App SDK; WinUI 3; packaged/unpackaged deployment modes。
- Platform requirement：Windows App SDK documentation states Windows 10 version 1809 or later for its APIs。
- Namespace/header/assembly：Windows App SDK and host project metadata; exact local package identity unknown。
- Managed/native boundary：NuGet-delivered SDK, WinRT and native desktop host boundaries。
- COM/Apartment requirement：Not closed by deployment docs。
- Dispatcher requirement：Host-specific Dispatcher or DispatcherQueue remains separate。
- Packaging context：Officially distinguishes package and runtime deployment modes; Clipboard integration is not automatically proven。
- Data ownership implication：Packaging mode does not define Clipboard format ownership。
- Process lifetime implication：Deployment mode does not define source process shutdown behavior。
- History/Cloud implication：OS/user state remains independent of package identity in this baseline。
- Privacy implication：Packaging does not make Clipboard data private。
- Known limitation：No current SnipPlus project or package asset inspection。
- Claim status：Confirmed by official source。
- Static specification effect：WPF and WinUI 3 packaged/unpackaged rows must remain distinct。
- Runtime validation still required：Local package assets, Restore, Build, host invocation and runtime behavior。

### CLIP-OFF-EVID-019 — WPF Dispatcher boundary

- Claim：WPF DispatcherObject has thread affinity; a background thread must delegate access to the Dispatcher associated with the UI thread; a shut-down Dispatcher cannot be restarted and queued work can be aborted。
- Candidate：CLIP-OPT-001、CLIP-OPT-005。
- Host：WPF。
- Related Pair：CLIP-PAIR-001、CLIP-PAIR-009。
- Related Prerequisites：CLIP-PREQ-009、CLIP-PREQ-010、CLIP-PREQ-014。
- Related Blockers：CLIP-BLOCK-005、CLIP-BLOCK-006。
- Related Blocking Actions：CLIP-BA-001、CLIP-BA-002。
- Related Closure Actions：CLIP-CLOSE-001、CLIP-CLOSE-002。
- Related Enablement Items：CLIP-ENABLE-001、CLIP-ENABLE-002。
- Related Upstream Gates：CLIP-GATE-003、CLIP-GATE-009。
- Related Closure Gates：CLIP-CGATE-003、CLIP-CGATE-009。
- Official source title：Dispatcher Class。
- Publisher/maintainer：Microsoft Learn / WPF。
- URL：https://learn.microsoft.com/en-us/dotnet/api/system.windows.threading.dispatcher?view=windowsdesktop-10.0
- Publication/update date：未以日期作為 Clipboard product conclusion。
- Access date：2026-07-26。
- API/Format/Interop identity：Dispatcher；Invoke；BeginInvoke；InvokeAsync；HasShutdownStarted。
- Platform requirement：WPF desktop。
- Namespace/header/assembly：System.Windows.Threading；WindowsBase.dll。
- Managed/native boundary：Managed dispatcher over UI thread message processing。
- COM/Apartment requirement：Dispatcher evidence does not replace COM apartment evidence。
- Dispatcher requirement：Thread affinity, queue and shutdown are documented。
- Packaging context：Not closed。
- Data ownership implication：Dispatcher controls access scheduling, not Clipboard native ownership。
- Process lifetime implication：Queued operation may be aborted during Dispatcher shutdown。
- History/Cloud implication：Not defined。
- Privacy implication：Not defined۔
- Known limitation：This does not prove the WPF Clipboard static API has the same invocation requirements in every path۔
- Claim status：Confirmed by official source。
- Static specification effect：WPF dispatcher handoff and shutdown are explicit open requirements۔
- Runtime validation still required：Clipboard call on UI/background paths and shutdown race۔

### CLIP-OFF-EVID-020 — WinUI 3 DispatcherQueue boundary

- Claim：Microsoft documents that WinUI 3 desktop apps use DispatcherQueue for UI-thread work; UI elements cannot be updated from background threads and DispatcherQueue.TryEnqueue is the documented handoff。
- Candidate：CLIP-OPT-002、CLIP-OPT-005。
- Host：WinUI 3。
- Related Pair：CLIP-PAIR-004、CLIP-PAIR-010。
- Related Prerequisites：CLIP-PREQ-012、CLIP-PREQ-014。
- Related Blockers：CLIP-BLOCK-007、CLIP-BLOCK-008。
- Related Blocking Actions：CLIP-BA-002、CLIP-BA-005。
- Related Closure Actions：CLIP-CLOSE-002、CLIP-CLOSE-005。
- Related Enablement Items：CLIP-ENABLE-002、CLIP-ENABLE-005。
- Related Upstream Gates：CLIP-GATE-003、CLIP-GATE-007。
- Related Closure Gates：CLIP-CGATE-003、CLIP-CGATE-007。
- Official source title：Keep the UI thread responsive; Using Windows Runtime objects in a multithreaded environment。
- Publisher/maintainer：Microsoft Learn / Windows Apps。
- URL：https://learn.microsoft.com/en-us/windows/apps/develop/performance/keep-ui-thread-responsive ; https://learn.microsoft.com/en-us/windows/apps/develop/threading/winrt-objects-multithreaded
- Publication/update date：Multithreaded WinRT page last updated 2026-07-15。
- Access date：2026-07-26。
- API/Format/Interop identity：Microsoft.UI.Dispatching.DispatcherQueue；TryEnqueue。
- Platform requirement：WinUI 3 / Windows App SDK desktop host。
- Namespace/header/assembly：Microsoft.UI.Dispatching and host window DispatcherQueue。
- Managed/native boundary：WinUI 3 XAML and Windows Runtime thread boundary。
- COM/Apartment requirement：DispatcherQueue evidence does not replace OLE/COM initialization evidence。
- Dispatcher requirement：UI updates and non-agile UI objects require UI-thread handoff。
- Packaging context：WinUI 3 deployment can be packaged or unpackaged; not closed for Clipboard route。
- Data ownership implication：Dispatcher does not define DataPackage or native handle lifetime。
- Process lifetime implication：UI object lifetime is bounded by its UI thread/window lifetime。
- History/Cloud implication：Not defined by Dispatcher documentation。
- Privacy implication：Not defined。
- Known limitation：Clipboard foreground rule and DispatcherQueue rule must be validated together in the selected host。
- Claim status：Confirmed by official source。
- Static specification effect：WinUI 3 UI-thread and shutdown boundaries are explicit; no direct clipboard authorization follows。
- Runtime validation still required：Clipboard call, foreground state, queue shutdown and object lifetime。

## 9. Candidate API / Interop Identity Baseline

### CLIP-OPT-001 — WPF Clipboard / IDataObject

| Field | Official baseline | Remaining boundary |
|---|---|---|
| Exact API identity | System.Windows.Clipboard; SetImage; SetData; SetDataObject; System.Windows.IDataObject | Current project reference and host integration unknown |
| Assembly / namespace | PresentationCore.dll; System.Windows | Local package and target framework unknown |
| Managed/native boundary | WPF managed wrapper over system Clipboard | Native ownership and message behavior not closed |
| STA / COM | WPF Clipboard page does not independently close the apartment contract | Do not infer; keep CLIP-OFF-GAP-009 open |
| Dispatcher | WPF DispatcherObject has thread affinity; Clipboard call path still needs host observation | UI/background/shutdown behavior unknown |
| Persistence | SetDataObject(Boolean) documents process-exit retention choice | Product privacy policy not made |
| Format identity | Framework Bitmap and IDataObject are available | CF_BITMAP, CF_DIB, CF_DIBV5 and PNG consumer fidelity unknown |
| Packaging | WPF is a possible existing desktop host | Packaged/unpackaged route unknown |
| Status | API available, Host integration unverified | No selection |

### CLIP-OPT-002 — WinRT Clipboard / DataPackage

| Field | Official baseline | Remaining boundary |
|---|---|---|
| Exact API identity | Windows.ApplicationModel.DataTransfer.Clipboard; DataPackage; StandardDataFormats.Bitmap | Current projection and host package unknown |
| Assembly / namespace | Windows.ApplicationModel.DataTransfer | Local reference and version unknown |
| Managed/native boundary | WinRT projection over Windows Clipboard | Desktop host integration unknown |
| Threading metadata | Clipboard page exposes WinRT metadata including standard marshaling / ThreadingModel.Both in current view | Foreground UI-thread rule still applies |
| Dispatcher | Official Clipboard page requires application focus and UI thread; WinUI uses DispatcherQueue | Combined runtime path unknown |
| Persistence | Clipboard.Flush releases DataPackage source object and supports post-shutdown availability | SetContent versus Flush product policy not made |
| Format identity | SetBitmap(RandomAccessStreamReference); SetData custom/legacy forms | Image bytes, alpha and consumer route unknown |
| History / Cloud | SetContent content eligible by default; options and OS APIs exist | User settings and privacy decision unknown |
| Packaging | Windows App SDK can serve existing desktop hosts | Packaged/unpackaged Clipboard route unknown |
| Status | API available, Host integration unverified | No selection |

### CLIP-OPT-003 — OLE Clipboard / COM IDataObject

| Field | Official baseline | Remaining boundary |
|---|---|---|
| Exact API identity | OleInitialize; OleSetClipboard; OleGetClipboard; OleFlushClipboard; IDataObject | Native adapter not created |
| Header / library | ole2.h; Ole32.lib; Ole32.dll | Local interop assets unknown |
| Managed/native boundary | COM IDataObject and OLE internal window | P/Invoke or projection route unknown |
| COM / apartment | OleInitialize initializes COM as STA and must precede OLE Clipboard functions | Host thread and message pump unknown |
| Dispatcher | Delayed rendering is delegated through OLE window messages | Host shutdown and reentrancy unknown |
| Ownership | OleSetClipboard AddRefs object; Flush or Set(NULL) releases it | Product lifetime policy not made |
| Format identity | OLE formats and IDataObject media | Bitmap, PNG, DIB/DIBV5 and consumer fidelity unknown |
| History / Cloud | Not defined by core OLE pages; Windows registered control formats are separate | Privacy policy unknown |
| Status | Requires documented native interop | No selection |

### CLIP-OPT-004 — Raw Win32 Clipboard

| Field | Official baseline | Remaining boundary |
|---|---|---|
| Exact API identity | OpenClipboard; EmptyClipboard; SetClipboardData; GetClipboardData; CloseClipboard; RegisterClipboardFormatW | Native adapter not created |
| Header / library | winuser.h; User32.lib; User32.dll | Local interop assets unknown |
| Managed/native boundary | HWND, HGLOBAL, HBITMAP and User32/GDI | Memory and handle ownership must be explicit |
| COM / apartment | Not inherent in raw User32 functions | Host message loop still required for delayed rendering |
| Dispatcher | Owner window and message procedure are relevant | Host integration unknown |
| Ownership | EmptyClipboard assigns owner; SetClipboardData transfers system ownership on success | Shutdown and cleanup unknown |
| Format identity | CF_BITMAP, CF_DIB, CF_DIBV5, registered formats | Consumer and pixel fidelity unknown |
| History / Cloud | Registered control formats can influence History / Cloud monitoring | Do not activate without privacy decision |
| Status | Requires documented native interop | No selection |

### CLIP-OPT-005 — Host-neutral Adapter

| Field | Official baseline | Remaining boundary |
|---|---|---|
| Nature | Architecture strategy, not a Windows API | No adapter contract created |
| Shared input contract | Must carry a future format-neutral image publication request | Not defined because capture/rendering decision is unresolved |
| Candidate-specific conversion | WPF, WinRT, OLE or Win32 representation conversion | No implementation or payload allowed |
| Failure normalization | Must preserve native failure, contention and privacy distinctions | No retry policy selected |
| Threading | Adapter must expose host thread/dispatcher prerequisites | Host-specific records not closed |
| Lifetime | Adapter must expose retention/flush/clear semantics separately | No lifecycle decision |
| Packaging | Must preserve WPF/WinUI packaged/unpackaged distinction | No package evidence |
| Status | Requires runtime prototype | No selection |

## 10. Candidate–Host Official Compatibility Matrix

| Pair | Candidate | Host | Official invocation evidence | Interop route | Threading / COM | Packaging | Host support status | Evidence IDs |
|---|---|---|---|---|---|---|---|---|
| CLIP-PAIR-001 | CLIP-OPT-001 WPF Clipboard | WPF | WPF Clipboard and IDataObject API pages | System.Windows managed wrapper | WPF Dispatcher documented separately; STA not closed by WPF page | Unknown | API available, Host integration unverified | CLIP-OFF-EVID-001..003, CLIP-OFF-EVID-019 |
| CLIP-PAIR-002 | CLIP-OPT-001 WPF Clipboard | WinUI 3 | WPF assembly/API identity exists | Cross-framework managed/native adapter would be required | WPF Dispatcher and WinUI DispatcherQueue are different host boundaries | Unknown | Requires documented native interop | CLIP-OFF-EVID-001, CLIP-OFF-EVID-018..020 |
| CLIP-PAIR-003 | CLIP-OPT-002 WinRT Clipboard | WPF | WinRT Clipboard/DataPackage API exists | WinRT projection from WPF desktop host | Foreground/UI rule; WPF Dispatcher route not closed | Unknown | API available, Host integration unverified | CLIP-OFF-EVID-005..007, CLIP-OFF-EVID-019 |
| CLIP-PAIR-004 | CLIP-OPT-002 WinRT Clipboard | WinUI 3 | Clipboard.SetContent and DataPackage are documented | WinRT projection / Windows App SDK host | Foreground/UI rule plus DispatcherQueue | Unknown | API available, Host integration unverified | CLIP-OFF-EVID-005..007, CLIP-OFF-EVID-020 |
| CLIP-PAIR-005 | CLIP-OPT-003 OLE Clipboard | WPF | OLE API and IDataObject are documented | Native OLE/COM adapter | OleInitialize STA; message pump and WPF Dispatcher remain | Unknown | Requires documented native interop | CLIP-OFF-EVID-014..017, CLIP-OFF-EVID-019 |
| CLIP-PAIR-006 | CLIP-OPT-003 OLE Clipboard | WinUI 3 | OLE API and IDataObject are documented | Native OLE/COM adapter | OleInitialize STA; WinUI DispatcherQueue remains | Unknown | Requires documented native interop | CLIP-OFF-EVID-014..017, CLIP-OFF-EVID-020 |
| CLIP-PAIR-007 | CLIP-OPT-004 Raw Win32 Clipboard | WPF | User32 Clipboard API is documented | P/Invoke/native adapter | Window message and WPF Dispatcher boundary | Unknown | Requires documented native interop | CLIP-OFF-EVID-008..013, CLIP-OFF-EVID-019 |
| CLIP-PAIR-008 | CLIP-OPT-004 Raw Win32 Clipboard | WinUI 3 | User32 Clipboard API is documented | P/Invoke/native adapter | Window message and WinUI DispatcherQueue boundary | Unknown | Requires documented native interop | CLIP-OFF-EVID-008..013, CLIP-OFF-EVID-020 |
| CLIP-PAIR-009 | CLIP-OPT-005 Host-neutral Adapter | WPF | Official sources document WPF and native capabilities separately | Future WPF adapter behind abstract boundary | WPF Dispatcher and candidate-specific requirements remain | Must be checked per selected package mode | Requires runtime prototype | CLIP-OFF-EVID-001..004, CLIP-OFF-EVID-018..019 |
| CLIP-PAIR-010 | CLIP-OPT-005 Host-neutral Adapter | WinUI 3 | Official sources document WinUI/WinRT and native capabilities separately | Future WinUI adapter behind abstract boundary | WinUI DispatcherQueue and candidate-specific requirements remain | Must be checked per selected package mode | Requires runtime prototype | CLIP-OFF-EVID-005..020 |

規則：

- WPF 與 WinUI 3 保持十列中的獨立 Host boundary。
- API 可由 .NET 或 WinRT 呼叫，不等於 Host integration 已驗證。
- 不形成 Candidate ranking。
- Unknown 不直接轉成排除。
- Requires documented native interop 不等於 runtime failure。

## 11. Clipboard Format Official Baseline

| Format row | Official identity | Producer representation | Ownership model | Alpha evidence | Color evidence | Consumer implication | Status | Evidence |
|---|---|---|---|---|---|---|---|---|
| Framework Bitmap | WPF BitmapSource or WinRT Bitmap | Framework-managed object or stream reference | Candidate-specific | Unknown | Unknown | Framework consumer route must be observed | Partially confirmed | CLIP-OFF-EVID-001、007 |
| CF_BITMAP | Standard format 2; HBITMAP | Device-dependent bitmap handle | Native handle/system ownership | Unknown | Device-dependent palette behavior documented | Legacy and current consumer behavior not universal | Confirmed by official source | CLIP-OFF-EVID-011、012 |
| CF_DIB | Standard format 8; BITMAPINFO plus bits | Movable global memory object | System-owned after SetClipboardData | Unknown | Palette and conversion rules documented | Consumer must recognize DIB | Confirmed by official source | CLIP-OFF-EVID-011、012 |
| CF_DIBV5 | Standard format 17; BITMAPV5HEADER plus color-space information and bits | Movable global memory object | System-owned after SetClipboardData | Unknown | Color-space conversion behavior documented | Consumer support and round-trip remain open | Confirmed by official source | CLIP-OFF-EVID-011、012 |
| PNG registered format | Application-defined registered format identity | Future byte stream only; no payload created here | Must follow registered HGLOBAL publication rules | Unknown | PNG metadata behavior not established here | Registration does not prove consumer recognition | Partially confirmed | CLIP-OFF-EVID-013 |
| WinRT Bitmap | StandardDataFormats.Bitmap | RandomAccessStreamReference through DataPackage | DataPackage and Flush semantics | Unknown | Unknown | WinRT-aware consumer route required | API available, Host integration unverified | CLIP-OFF-EVID-007 |
| OLE IDataObject | IDataObject with FORMATETC / STGMEDIUM | OLE data object | Reference count and delayed rendering | Unknown | Format-specific | OLE consumer may forward calls or use materialized data | Confirmed by official source | CLIP-OFF-EVID-015..017 |
| WinRT DataPackage | DataPackage with standard or custom formats | DataPackage methods and format ID | DataPackage lifetime / Flush | Unknown | Format-specific | Source and target need compatible format contract | Confirmed by official source | CLIP-OFF-EVID-006、007 |
| Multi-format publication | Multiple formats for same information | Several objects in one Clipboard operation | Candidate-specific | Format-specific | Format order and consumer choice matter | Consumer chooses recognized format; no global fidelity claim | Partially confirmed | CLIP-OFF-EVID-010..013 |

固定規則：

- CF_BITMAP、CF_DIB、CF_DIBV5 與 PNG registered format 不得視為相同 payload。
- Official format capability 與 actual Consumer compatibility 分開。
- Alpha、premultiplication、stride、pixel fidelity、HDR-to-SDR 與 embedded metadata 沒有在本文件被關閉。
- PNG registered format 只確認 registration mechanism，沒有確認 universal PNG clipboard consumer。
- 任何未來 fidelity 結論必須來自 synthetic runtime evidence，而非本文件。

## 12. Threading / COM / Dispatcher Baseline

| Scenario | Official requirement | Apartment | Dispatcher / thread | Failure boundary | Runtime still required | Evidence |
|---|---|---|---|---|---|---|
| WPF UI STA | WPF Dispatcher objects have thread affinity; WPF Clipboard API identity is documented | WPF-specific Clipboard apartment not closed | Use owning WPF Dispatcher for UI-bound objects | Dispatcher shutdown or invalid thread access | Yes | CLIP-OFF-EVID-001、019 |
| WPF background STA | Separate dispatcher and message processing may exist | STA claim requires explicit evidence for chosen API path | Must not assume UI Dispatcher can be bypassed | Cross-thread ownership and shutdown | Yes | CLIP-OFF-EVID-004、019 |
| WPF background MTA | OLE STA requirement conflicts with direct OLE use from MTA | MTA cannot be silently changed to STA by adapter | Dispatch to compatible STA if chosen | RPC_E_CHANGED_MODE or interop failure | Yes | CLIP-OFF-EVID-014、019 |
| WinUI 3 UI thread | Clipboard official page requires focus and UI thread | WinRT metadata may be Both; foreground rule remains | Use DispatcherQueue for UI work | Not foreground or queue shutdown | Yes | CLIP-OFF-EVID-005、020 |
| WinUI 3 background thread | UI elements and UI-bound objects cannot be used directly | Candidate-specific API remains unresolved | TryEnqueue to UI DispatcherQueue when required | Invalid thread or object lifetime failure | Yes | CLIP-OFF-EVID-020 |
| OLE with COM initialized | OleInitialize initializes COM as STA before OLE Clipboard use | STA | Message pump is relevant to OLE behavior | Initialization failure or incompatible apartment | Yes | CLIP-OFF-EVID-014、015 |
| OLE without initialization | Official OLE boundary requires initialization before COM/OLE calls | Unknown / invalid | No approved fallback is inferred | COM/OLE failure | Yes | CLIP-OFF-EVID-014 |
| Dispatcher shutdown | WPF Dispatcher cannot restart; queued work may abort | Host-specific | Queue state is part of operation state | Operation cancellation or cleanup race | Yes | CLIP-OFF-EVID-019、020 |
| App shutdown during publication | WPF, WinRT Flush and OLE Flush have separate lifetime semantics | Candidate-specific | Shutdown sequencing is not a generic retry | Partial publication or retained data | Yes | CLIP-OFF-EVID-002、006、016 |
| Retry cancellation | WinForms documents retry overload but no product policy | Candidate-specific | UI must remain responsive | Cancel, timeout and contention policy unknown | Yes | CLIP-OFF-EVID-004、008 |

不得把官方 STA、COM 或 Dispatcher 文件寫成「SnipPlus 已通過」。本表只建立 future enablement 的 static contract。

## 13. Data Ownership and Lifetime Baseline

| Capability | Candidate | Official behavior | Ownership responsibility | Remaining runtime question | Evidence |
|---|---|---|---|---|---|
| Immediate WPF data object publication | CLIP-OPT-001 | SetDataObject places IDataObject; Boolean controls exit retention | Separate persistence choice from format choice | Does current data remain after host exit? | CLIP-OFF-EVID-001、002 |
| WinRT DataPackage publication | CLIP-OPT-002 | SetContent sets current content; Flush releases DataPackage source object | Decide whether Flush is authorized separately | Does post-shutdown content remain for target consumer? | CLIP-OFF-EVID-005..007 |
| OLE IDataObject | CLIP-OPT-003 | OleSetClipboard increases reference count for delayed rendering | Keep object alive until Flush or clear path | When and how is release observed? | CLIP-OFF-EVID-015、016 |
| Raw HGLOBAL | CLIP-OPT-004 | System owns hMem after successful SetClipboardData | Do not write/free after transfer | What happens on failure before transfer? | CLIP-OFF-EVID-009 |
| HBITMAP | CLIP-OPT-004 | CF_BITMAP uses HBITMAP; format has device-dependent behavior | Native GDI ownership must be explicit | Consumer and owner cleanup | CLIP-OFF-EVID-011、012 |
| Delayed rendering | CLIP-OPT-003、004 | Owner must render while it owns Clipboard | Maintain message-capable owner | What happens when source closes? | CLIP-OFF-EVID-008、010、015 |
| Multiple formats | CLIP-OPT-001..004 | Windows supports multiple representations of same information | Every representation needs a lifecycle contract | Partial format publication and consumer choice | CLIP-OFF-EVID-010、017 |
| Clipboard owner change | CLIP-OPT-003、004 | EmptyClipboard or another app changes owner; release messages may occur | Stop assuming ownership after change | Does adapter observe and clean up correctly? | CLIP-OFF-EVID-008、010 |
| Consumer before producer termination | CLIP-OPT-003 | OleGetClipboard may forward calls to source object | Treat cross-process call as untrusted and time-sensitive | RPC and source shutdown behavior | CLIP-OFF-EVID-017 |
| Consumer after producer termination | CLIP-OPT-002、003 | Flush can materialize content for post-shutdown availability | Distinguish materialization from privacy | Which formats survive? | CLIP-OFF-EVID-006、016 |
| History / Cloud materialization | All | OS and options can retain or synchronize content | User policy and app opt-out must be explicit | Actual settings and account effect | CLIP-OFF-EVID-006、013、028 |

## 14. Contention, Failure and Retry Baseline

| Failure condition | Candidate | Official error / behavior | Retry support documented | Cleanup responsibility | Runtime observation required | Evidence |
|---|---|---|---|---|---|---|
| Clipboard unavailable | WPF / WinForms | ExternalException documented for Clipboard access failures | WinForms retry overload exists | Product policy not defined | Yes | CLIP-OFF-EVID-004 |
| OpenClipboard failure | CLIP-OPT-004 | OpenClipboard returns zero and GetLastError is available | No product retry policy | Ensure no half-open state | Yes | CLIP-OFF-EVID-008 |
| Clipboard owned by another process | CLIP-OPT-003、004 | Open or OLE operation may fail while another process holds Clipboard | No universal bounded retry | Preserve file/output boundary | Yes | CLIP-OFF-EVID-008、015 |
| STA / COM violation | CLIP-OPT-003 | OleInitialize may fail with RPC_E_CHANGED_MODE; WinForms reports ThreadStateException | No fallback inferred | Do not change apartment implicitly | Yes | CLIP-OFF-EVID-004、014 |
| Foreground failure | CLIP-OPT-002 | SetContent may throw when app is not foreground | SetContentWithOptions changes exception behavior, not product policy | Record no Clipboard result | Yes | CLIP-OFF-EVID-005、006 |
| Format conversion failure | All | Official docs describe format conversions but not every consumer path | No universal retry | Preserve original result and state | Yes | CLIP-OFF-EVID-010..012 |
| Partial multi-format publication | CLIP-OPT-003、004 | Delayed rendering and multiple formats can materialize separately | No atomic product guarantee inferred | Keep published-format evidence separate | Yes | CLIP-OFF-EVID-010 |
| Memory allocation failure | CLIP-OPT-004 | Native handle and HGLOBAL requirements are documented | No product retry policy | Native allocation cleanup required | Yes | CLIP-OFF-EVID-009、013 |
| Ownership loss | CLIP-OPT-003、004 | Owner changes when another app empties Clipboard | No retry policy | Stop delayed-rendering responsibility | Yes | CLIP-OFF-EVID-008、010、015 |
| Dispatcher shutdown | CLIP-OPT-001、005 | WPF queued work can abort; WinUI objects are bounded by UI thread/window lifetime | No Clipboard retry inferred | Cancel operation and preserve independent result | Yes | CLIP-OFF-EVID-019、020 |
| Packaging / interop failure | All | Official packaging docs distinguish deployment modes but do not prove Clipboard integration | No retry policy | Record environment prerequisite failure separately | Yes | CLIP-OFF-EVID-018 |

本文件不制定 retry 次數、retry 間隔、timeout、可重試錯誤清單或 cancellation policy。

## 15. History / Cloud Clipboard Baseline

| Capability | Official behavior | App control available | Format / size implication | Privacy implication | Runtime observation needed | Evidence |
|---|---|---|---|---|---|---|
| History enabled | WinRT exposes IsHistoryEnabled; Support explains Windows + V activation | OS/user setting, and API options where available | Platform-managed retention | Content can persist locally | Yes | CLIP-OFF-EVID-005、028 |
| History item exclusion | ClipboardContentOptions.IsAllowedInHistory exists; default is true when History is enabled | WinRT SetContentWithOptions path | Exclusion is policy, not format conversion | Can reduce local history exposure | Yes | CLIP-OFF-EVID-006、010 |
| Cloud / Roaming enabled | WinRT exposes IsRoamingEnabled; Support ties sync to account and setting | User setting and candidate-specific control format | Cross-device state | Account and cloud exposure | Yes | CLIP-OFF-EVID-005、006、028 |
| Cloud exclusion | Windows registered control format CanUploadToCloudClipboard is documented | Native registered-format path | Does not affect local History according to official docs | Requires explicit privacy review | Yes | CLIP-OFF-EVID-013、017 |
| Monitor exclusion | ExcludeClipboardContentFromMonitorProcessing is documented | Native registered-format path | All formats can be excluded from History/Cloud monitoring | Must not be activated without policy | Yes | CLIP-OFF-EVID-013 |
| Support size guidance | Microsoft Support states 4 MB per History item; Text, HTML and Bitmap are supported | No product setting inferred | Not total Clipboard memory or all-format limit | Large images may enter different paths | Yes | CLIP-OFF-EVID-028 |
| History count | Support states 25 copied entries, excluding pinned retention behavior | OS-managed | Retention is not controlled by publication API alone | Historical content may remain | Yes | CLIP-OFF-EVID-028 |
| Restart behavior | Support states History is cleared on restart except pinned items | OS/user setting | Not a Clipboard owner lifetime guarantee | Restart does not prove secure erasure | Yes | CLIP-OFF-EVID-028 |
| Multiple-format History | Format docs describe control formats, not all consumer materialization outcomes | Candidate-specific | Multi-format item treatment remains open | Privacy impact must be tested in isolation | Yes | CLIP-OFF-EVID-013、017 |
| Sensitive image persistence | Official docs establish retention and controls but not SnipPlus image policy | Requires future product decision | Alpha/PNG/DIB behavior remains open | Must use synthetic content for experiments | Yes | CLIP-OFF-EVID-006、012、028 |

不得修改任何 History、Cloud、account 或 Windows 設定。所有 control-format 行為維持未驗證。

## 16. Packaging and Desktop App Boundary

| Candidate / API | Packaged desktop | Unpackaged desktop | Windows App SDK dependency | Native interop dependency | Static evidence status | Runtime need |
|---|---|---|---|---|---|---|
| WPF Clipboard | WPF API identity exists; package mode unknown | WPF API identity exists; package mode unknown | Not required by WPF identity | System Clipboard boundary remains | API available, Host integration unverified | Local project and runtime |
| WinRT Clipboard | WinRT API identity exists; host package unknown | WinRT API identity exists; host package unknown | May be used through Windows App SDK host | Projection and foreground route | API available, Host integration unverified | Local project, package and runtime |
| OLE Clipboard | Desktop OLE APIs documented | Desktop OLE APIs documented | Not required by OLE identity | Required native COM interop | Requires documented native interop | Project, interop and runtime |
| Raw Win32 Clipboard | User32 APIs documented | User32 APIs documented | Not required by raw identity | Required P/Invoke/native interop | Requires documented native interop | Project, interop and runtime |
| Host-neutral adapter | Must preserve selected package mode | Must preserve selected package mode | Depends on selected host | Depends on selected candidate | Requires runtime prototype | Future after decisions |

固定結論：

- Windows SDK、WinRT projection、Windows App SDK 與 WPF API 是不同 dependency identity。
- API 存在不代表目前 Local asset 存在。
- Packaged support 不代表 unpackaged support。
- WPF 與 WinUI 3 不合併。
- 目前本機 availability 全部維持 Unknown。

## 17. Security and Privacy Baseline

| Boundary | Official evidence | Static interpretation | Remaining evidence |
|---|---|---|---|
| Cross-process visibility | Microsoft states all applications can access system Clipboard | Clipboard is shared, not private | Product read/write policy |
| User-driven operation | Win32 About the Clipboard says transfer should occur in response to user command | Clipboard write must remain user-initiated in future product requirements | UI authority artifact and product decision |
| Untrusted input | OleGetClipboard warns Clipboard data is not trusted | Read path requires defensive parsing | Consumer and security review |
| History persistence | Support documents History and restart/pinned behavior | Success does not mean local-only retention | Isolated runtime observation |
| Cloud sync | Support ties sync to account/work account and setting; WinRT exposes Roaming | App cannot assume no cross-device sync | User setting and privacy decision |
| Opt-out controls | WinRT options and Win32 registered control formats exist | Control path is candidate-specific and must be explicit | Runtime and policy |
| Process-exit retention | WPF, WinRT Flush and OLE Flush have different semantics | Retention and write must be separate permissions | Shutdown observation |
| Restore existing Clipboard | No official source makes restore zero-risk | Backup/restore is a separate operation and remains unauthorized | Future policy and runtime |
| Diagnostic logs | Official docs do not authorize logging image bytes | Future diagnostics must exclude payload and private names | Evidence policy |
| Synthetic-only evidence | Product policy from upstream research requires synthetic image for runtime | No private image may enter Clipboard experiments | Future authorization |

本基線不記錄任何私人 Window title、檔名、Clipboard payload 或 image bytes。

## 18. Official Evidence Gap Register

Gap status 只使用 Open 或 Accepted documentation limitation。官方搜尋不到的 claim 不得轉成不支援。

| Gap ID | Missing claim | Candidate | Host | Related Pair | Related prerequisites | Why official evidence is insufficient | Required next evidence | Local inspection | Project | Restore | Build | Clipboard operation | Runtime | Blocks L1 authorization | Status |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| CLIP-OFF-GAP-001 | Current Windows and host availability | CLIP-OPT-001..005 | WPF/WinUI 3 | CLIP-PAIR-001..010 | CLIP-PREQ-001 | API pages do not inspect this machine | Local baseline | Yes | No | No | No | No | Yes | Yes | Open |
| CLIP-OFF-GAP-002 | Current target framework and assembly availability | CLIP-OPT-001、002 | WPF/WinUI 3 | CLIP-PAIR-001..004 | CLIP-PREQ-002、011 | API identity does not prove project reference | Project/package inspection | No | Yes | Yes | Yes | No | Yes | Yes | Open |
| CLIP-OFF-GAP-003 | WPF Clipboard STA behavior in this Host | CLIP-OPT-001 | WPF | CLIP-PAIR-001 | CLIP-PREQ-003、009 | WPF Clipboard page does not close product host thread | Synthetic host spike | No | Yes | No | Yes | Yes | Yes | Yes | Open |
| CLIP-OFF-GAP-004 | WinRT Clipboard foreground behavior in WinUI 3 desktop | CLIP-OPT-002 | WinUI 3 | CLIP-PAIR-004 | CLIP-PREQ-012、014 | Official rule does not prove current host activation state | Packaged/unpackaged runtime spike | No | Yes | Yes | Yes | Yes | Yes | Yes | Open |
| CLIP-OFF-GAP-005 | WPF to WinUI 3 WPF-wrapper route | CLIP-OPT-001 | WinUI 3 | CLIP-PAIR-002 | CLIP-PREQ-005 | API existence does not document cross-framework route | Native interop specification and spike | No | Yes | Yes | Yes | Yes | Yes | Yes | Open |
| CLIP-OFF-GAP-006 | PNG registered format consumer recognition | CLIP-OPT-002..004 | WPF/WinUI 3 | CLIP-PAIR-003..008 | CLIP-PREQ-029 | Registration is not consumer support | Isolated Consumer test | No | Yes | No | Yes | Yes | Yes | Yes | Open |
| CLIP-OFF-GAP-007 | Multi-format acceptance order across Consumers | CLIP-OPT-001..005 | WPF/WinUI 3 | CLIP-PAIR-001..010 | CLIP-PREQ-018、026 | Official model does not cover selected Consumers | Multi-format runtime matrix | No | Yes | No | Yes | Yes | Yes | Yes | Open |
| CLIP-OFF-GAP-008 | Alpha-channel fidelity | CLIP-OPT-001..005 | WPF/WinUI 3 | CLIP-PAIR-001..010 | CLIP-PREQ-026、027 | Format definitions do not prove pixel round-trip | Synthetic pixel comparison | No | Yes | No | Yes | Yes | Yes | Yes | Open |
| CLIP-OFF-GAP-009 | Premultiplication, stride and pixel fidelity | CLIP-OPT-003、004 | Native consumer | CLIP-PAIR-005..008 | CLIP-PREQ-027 | Header and conversion docs do not prove round-trip | Synthetic format spike | No | Yes | No | Yes | Yes | Yes | Yes | Open |
| CLIP-OFF-GAP-010 | Color profile and HDR-to-SDR responsibility | CLIP-OPT-001..005 | WPF/WinUI 3 | CLIP-PAIR-001..010 | CLIP-PREQ-028 | Official Clipboard docs do not define product color contract | Future color evidence | No | Yes | No | Yes | Yes | Yes | Yes | Open |
| CLIP-OFF-GAP-011 | OLE IDataObject lifetime after producer shutdown | CLIP-OPT-003 | WPF/WinUI 3 | CLIP-PAIR-005、006 | CLIP-PREQ-030、031 | Static AddRef/Flush docs do not prove host shutdown | OLE lifetime spike | No | Yes | No | Yes | Yes | Yes | Yes | Open |
| CLIP-OFF-GAP-012 | Raw HGLOBAL and HBITMAP cleanup on failure | CLIP-OPT-004 | WPF/WinUI 3 | CLIP-PAIR-007、008 | CLIP-PREQ-022、023 | Ownership rule does not cover every failure branch | Native failure observation | No | Yes | No | Yes | Yes | Yes | Yes | Open |
| CLIP-OFF-GAP-013 | Contention and bounded retry | CLIP-OPT-001..005 | WPF/WinUI 3 | CLIP-PAIR-001..010 | CLIP-PREQ-020 | Official failure docs do not define product retry policy | Isolated contention spike | No | Yes | No | Yes | Yes | Yes | Yes | Open |
| CLIP-OFF-GAP-014 | Dispatcher shutdown during Clipboard publication | CLIP-OPT-001、002、005 | WPF/WinUI 3 | CLIP-PAIR-001、004、009、010 | CLIP-PREQ-010、014 | Dispatcher docs do not prove Clipboard transaction outcome | Shutdown runtime observation | No | Yes | No | Yes | Yes | Yes | Yes | Open |
| CLIP-OFF-GAP-015 | History treatment of multi-format bitmap publication | CLIP-OPT-001..005 | Windows 11 | CLIP-PAIR-001..010 | CLIP-PREQ-016、017 | Support and format docs do not cover every format combination | Isolated History spike | No | Yes | No | Yes | Yes | Yes | Open |
| CLIP-OFF-GAP-016 | Cloud sync treatment of image or registered formats | CLIP-OPT-002..004 | Windows 11 | CLIP-PAIR-003..008 | CLIP-PREQ-016、029 | Account setting and control formats need observation | Isolated Cloud boundary spike | No | Yes | No | Yes | Yes | Yes | Yes | Open |
| CLIP-OFF-GAP-017 | Actual package identity and Windows App SDK runtime mode | CLIP-OPT-002、005 | WinUI 3 | CLIP-PAIR-004、010 | CLIP-PREQ-011、013 | Deployment docs do not inspect current repository | Local/package/build evidence | Yes | Yes | Yes | Yes | No | Yes | Yes | Open |
| CLIP-OFF-GAP-018 | Privacy-safe evidence persistence and file boundary | CLIP-OPT-005 | WPF/WinUI 3 | CLIP-PAIR-009、010 | CLIP-PREQ-032 | Documentation cannot authorize evidence storage or private-data handling | Future approved evidence plan | No | Yes | No | Yes | No | Yes | Yes | Open |
| CLIP-OFF-GAP-019 | Shared UI authority artifact | CLIP-OPT-005 | WPF/WinUI 3 | CLIP-PAIR-009、010 | CLIP-PREQ-001 | No authority artifact exists in current research line | Human authority artifact | No | No | No | No | No | No | Yes | Open |
| CLIP-OFF-GAP-020 | Candidate suitability without ranking | CLIP-OPT-001..005 | WPF/WinUI 3 | CLIP-PAIR-001..010 | CLIP-PREQ-001..032 | Official documents cannot make product decision | Future decision record and authorized spike results | No | Yes | No | Yes | Yes | Yes | Yes | Open |

## 19. Enablement Evidence Mapping

| Enablement item | Required official claims | Evidence IDs | Official gap IDs | Specification improvement | Remaining gap | Status recommendation |
|---|---|---|---|---|---|---|
| CLIP-ENABLE-001 | WPF identity, data object, retention, Dispatcher boundary | CLIP-OFF-EVID-001..003、019 | CLIP-OFF-GAP-001..003、007..010、014..015 | Separate API identity, retention and host thread | Local, host, format and runtime evidence absent | Partially specified |
| CLIP-ENABLE-002 | WinRT Clipboard/DataPackage, foreground and DispatcherQueue | CLIP-OFF-EVID-005..007、020 | CLIP-OFF-GAP-001、002、004、007..010、014..017 | Separate foreground, format and package boundary | Project, package, History and runtime evidence absent | Partially specified |
| CLIP-ENABLE-003 | OLE initialization, IDataObject, delayed rendering and Flush | CLIP-OFF-EVID-014..017 | CLIP-OFF-GAP-005、007、009、011..013 | Separate COM, ownership, delay and shutdown | Native interop and runtime evidence absent | Partially specified |
| CLIP-ENABLE-004 | Raw User32, HGLOBAL, formats, owner and contention | CLIP-OFF-EVID-008..013 | CLIP-OFF-GAP-006..013、015..016 | Separate open, ownership, format and privacy controls | Native adapter and consumer evidence absent | Partially specified |
| CLIP-ENABLE-005 | WPF/WinUI host boundary, packaged/unpackaged and Dispatcher | CLIP-OFF-EVID-018..020 | CLIP-OFF-GAP-001..005、014、017、019 | Keep host and deployment combinations separate | Shared authority and local package evidence absent | Partially specified |
| CLIP-ENABLE-006 | History/Cloud, privacy, evidence and independent file boundary | CLIP-OFF-EVID-005、006、013、017、028 | CLIP-OFF-GAP-015..019 | Separate Write, History, Cloud, Clear and evidence permissions | No privacy decision or runtime evidence | Blocked |

### CLIP-ENABLE-001 — WPF API and Dispatcher boundary

Official evidence is sufficient for static API identity only. Local host, STA, format and runtime evidence remain open.

### CLIP-ENABLE-002 — WinRT Clipboard and foreground boundary

Official evidence is sufficient for static WinRT identity and foreground rules only. Host, package and runtime evidence remain open.

### CLIP-ENABLE-003 — OLE COM and lifetime boundary

Official evidence is sufficient for static OLE, COM, delayed-rendering and Flush semantics only. Native interop and shutdown evidence remain open.

### CLIP-ENABLE-004 — Raw Win32 ownership and format boundary

Official evidence is sufficient for static User32, handle ownership and format identity only. Native cleanup and Consumer evidence remain open.

### CLIP-ENABLE-005 — Host and deployment boundary

Official evidence is sufficient for separating WPF, WinUI 3, packaged and unpackaged identities only. Current project and package evidence remain open.

### CLIP-ENABLE-006 — History, Cloud, privacy and evidence boundary

Official evidence is sufficient for naming the privacy and retention boundaries only. No write, clear, evidence persistence or privacy decision is authorized.

## 20. Candidate–Host Pair Evidence Mapping

| Pair | Accepted official evidence | Unresolved official gap | Local evidence required | Build required | Runtime required | Pair recommendation |
|---|---|---|---|---|---|---|
| CLIP-PAIR-001 | WPF Clipboard, IDataObject, WPF Dispatcher | CLIP-OFF-GAP-001、003、007..010、014 | Yes | Yes | Yes | No ranking; keep as candidate |
| CLIP-PAIR-002 | WPF API and Windows App SDK host boundary | CLIP-OFF-GAP-002、005、017 | Yes | Yes | Yes | Requires documented native interop |
| CLIP-PAIR-003 | WinRT Clipboard/DataPackage from WPF context | CLIP-OFF-GAP-002、004、006、015..017 | Yes | Yes | Yes | API available, Host integration unverified |
| CLIP-PAIR-004 | WinRT Clipboard/DataPackage and WinUI DispatcherQueue | CLIP-OFF-GAP-001、002、004、015..017 | Yes | Yes | Yes | API available, Host integration unverified |
| CLIP-PAIR-005 | OLE COM, IDataObject, delayed rendering and Flush | CLIP-OFF-GAP-005、007、011..013 | Yes | Yes | Yes | Requires documented native interop |
| CLIP-PAIR-006 | OLE COM and WinUI DispatcherQueue boundary | CLIP-OFF-GAP-005、011..014、017 | Yes | Yes | Yes | Requires documented native interop |
| CLIP-PAIR-007 | User32 open/empty/set/close and standard formats | CLIP-OFF-GAP-006..013 | Yes | Yes | Yes | Requires documented native interop |
| CLIP-PAIR-008 | User32 and WinUI host boundary | CLIP-OFF-GAP-006..010、012..017 | Yes | Yes | Yes | Requires documented native interop |
| CLIP-PAIR-009 | Host-neutral strategy plus WPF evidence | CLIP-OFF-GAP-001..003、007..010、014、018..020 | Yes | Yes | Yes | Requires runtime prototype |
| CLIP-PAIR-010 | Host-neutral strategy plus WinUI evidence | CLIP-OFF-GAP-001、002、004、007..010、014、017..020 | Yes | Yes | Yes | Requires runtime prototype |

## 21. Closure Gate Evidence Mapping

| Closure gate | Official evidence contribution | Remaining documentary requirement | Remaining non-documentary requirement | Evidence sufficiency |
|---|---|---|---|---|
| CLIP-CGATE-001 API identity | Candidate API names, namespaces, headers and assemblies are documented | Current project reference and local availability | Local inspection and project evidence | Sufficient for static specification |
| CLIP-CGATE-002 format identity | CF_BITMAP, CF_DIB, CF_DIBV5, Bitmap and registered format mechanisms are documented | Product format contract | Synthetic image and Consumer evidence | Partially sufficient |
| CLIP-CGATE-003 threading | STA, OLE initialization, WPF Dispatcher and WinUI DispatcherQueue boundaries are documented | Candidate/Host invocation contract | Threading runtime observation | Partially sufficient |
| CLIP-CGATE-004 contention | OpenClipboard exclusivity, OLE failure HRESULTs and WinForms retry surface are documented | Product retry and cancellation policy | Contention spike | Partially sufficient |
| CLIP-CGATE-005 ownership/lifetime | EmptyClipboard owner, HGLOBAL transfer, IDataObject AddRef and Flush are documented | Adapter lifecycle contract | Owner and shutdown observation | Partially sufficient |
| CLIP-CGATE-006 packaged route | Windows App SDK and package deployment models are documented | Current package mode and dependency graph | Local/package/build evidence | Insufficient |
| CLIP-CGATE-007 unpackaged route | Unpackaged deployment is documented as a distinct mode | Current unpackaged project state | Local/package/build evidence | Insufficient |
| CLIP-CGATE-008 History/Cloud | WinRT History/Roaming APIs, control formats and Support guidance are documented | Privacy decision and allowed scope | Isolated setting/runtime observation | Partially sufficient |
| CLIP-CGATE-009 consumer interoperability | Official format and DataPackage/OLE models identify possible routes | Consumer contract | Consumer runtime evidence | Insufficient |
| CLIP-CGATE-010 privacy/evidence | Cross-process, untrusted input, History/Cloud and size guidance are documented | Evidence and privacy policy | Human review and synthetic-only runtime evidence | Partially sufficient |
| CLIP-CGATE-011 authority boundary | Official documents do not create Shared UI authority | Authority artifact and human decision | Explicit authorization | Insufficient |

明確規定：

- API Reference 不能取代 Local availability。
- Sample 不能取代 Project / Restore / Build evidence。
- STA 文件不能取代 Threading Runtime observation。
- Format 文件不能取代 Consumer interoperability。
- DIB/DIBV5 文件不能取代 Alpha 或 pixel comparison。
- History 文件不能取代隔離環境觀察。
- Failure 文件不能取代 Contention / Retry observation。
- Privacy 文件不能取代實際 Privacy review。
- Shared UI research 不能取代缺少的 authority artifact。

## 22. Shared UI Authority Artifact Gap

本節專門處理 CLIP-ENABLE-GAP-001。

| Requirement | Existing research evidence | Authority artifact found | Official research contribution | Remaining authority need | Effect |
|---|---|---|---|---|---|
| Shared UI source-of-truth | Existing UI / Capture / Rendering research line and ADR-0002 are referenced upstream | No | Official Clipboard docs do not identify SnipPlus UI authority | A real authority artifact must be supplied by the project owner | Blocks authorization |
| Framework authority | WPF and WinUI 3 are kept as separate Host rows | No | Microsoft docs describe both platform boundaries but do not choose SnipPlus framework | Human framework decision | Keeps UI Framework Decision unresolved |
| Capture handoff authority | Capture and Rendering decisions remain not made | No | Clipboard docs cannot establish capture output contract | Existing capture/rendering authority artifact | Clipboard evidence cannot authorize capture |
| Clipboard write authority | Write is separated from read and clear | No | Official API pages describe capability only | Explicit human authorization | No Clipboard Write permitted |
| Evidence persistence authority | Runtime evidence remains future scope | No | Official docs do not authorize storing artifacts | Approved evidence policy | No Evidence write permitted |

固定狀態：

- Authority artifact found: No。
- Authority reference: TBD。
- Authorization status: Not granted。
- 本文件不得建立 UI-AUTH-*。
- 本文件不得代替真人做 Framework、Capture、Rendering 或 Clipboard decision。

## 23. Coverage and Traceability Ledger

| Family | Exact coverage statement |
|---|---|
| CLIP-OPT | CLIP-OPT-001、002、003、004、005 all have identity records |
| CLIP-PAIR | CLIP-PAIR-001..010 all have official compatibility rows |
| CLIP-PREQ | CLIP-PREQ-001..032 are mapped through Candidate, evidence, gap and gate tables |
| CLIP-BLOCK | CLIP-BLOCK-001..013 are mapped through evidence and gap records |
| CLIP-BA | CLIP-BA-001..006 are mapped in each official evidence and enablement table |
| CLIP-CLOSE | CLIP-CLOSE-001..006 are mapped in each official evidence and enablement table |
| CLIP-ENABLE | CLIP-ENABLE-001..006 have exactly six enablement rows |
| CLIP-GATE | CLIP-GATE-001..010 are mapped in the official evidence records and closure gates |
| CLIP-CGATE | CLIP-CGATE-001..011 have exactly eleven closure gate rows |
| CLIP-GAP | CLIP-GAP-001、CLIP-GAP-002、CLIP-GAP-003、CLIP-GAP-004、CLIP-GAP-005、CLIP-GAP-006、CLIP-GAP-007、CLIP-GAP-008、CLIP-GAP-009、CLIP-GAP-010、CLIP-GAP-011、CLIP-GAP-012、CLIP-GAP-013、CLIP-GAP-014、CLIP-GAP-015、CLIP-GAP-016、CLIP-GAP-017、CLIP-GAP-018 were fully reviewed and retained as upstream open gaps |
| CLIP-ENABLE-GAP | CLIP-ENABLE-GAP-001 is explicitly open; no UI-AUTH-* is invented |
| CLIP-OFF-EVID | CLIP-OFF-EVID-001..020 are new sequential official evidence records |
| CLIP-OFF-GAP | CLIP-OFF-GAP-001..020 are new sequential official documentation gaps |
| Upstream documents | RESEARCH-TECH-CLIPBOARD-001..005 remain read-only and unmodified |
| Architecture boundary | TD-004, ADR-0002 and existing Architecture responsibility boundaries remain unresolved where stated upstream |

Traceability chain：

Microsoft first-party source
→ CLIP-OFF-EVID / CLIP-OFF-GAP
→ Candidate API / Format / Threading / Ownership / Lifetime identity
→ CLIP-PREQ / CLIP-BLOCK
→ CLIP-PAIR
→ CLIP-BA / CLIP-CLOSE / CLIP-ENABLE
→ CLIP-CGATE
→ Future Enablement Reassessment
→ Future Closure Authorization Request

## 24. Official Evidence Baseline Status

| Status field | Current value | Reason |
|---|---|---|
| Official prerequisite evidence baseline | Official prerequisite evidence baseline complete | Microsoft first-party source inventory and claim records are present |
| Reassessment sufficiency | Partially sufficient for reassessment | Official claims can sharpen the static boundary, but local/project/build/runtime evidence is absent |
| Candidate selection | Not made | No ranking or recommendation is allowed |
| UI Framework decision | Unresolved | Shared UI authority artifact is absent and ADR-0002 remains Draft |
| Capture decision | Not made | This research does not touch capture |
| Rendering decision | Not made | Format evidence does not choose rendering technology |
| Closure execution | Not started | No authorization |
| Clipboard Runtime Spike | Not started | No authorization |
| Clipboard Read / Write / Clear | Not performed | No authorization |
| Evidence persistence | Not performed | No authorization |
| Local Environment Inspection | Not performed | Explicit non-goal |
| Package Cache Inspection | Not performed | Explicit non-goal |
| Build Verification | Not performed | Explicit non-goal |
| Runtime Verification | Not performed | Explicit non-goal |

正式結論：

Official prerequisite evidence baseline complete; Partially sufficient for reassessment。

這個結論只表示官方文件基線已整理完成，不表示：

- Clipboard prerequisite closure 已完成。
- Closure Execution Authorized。
- Clipboard Runtime Spike Authorized。
- Clipboard Read、Write、Clear 或 Evidence Write Authorized。
- 任一 Candidate–Host pair 已通過 Build 或 Runtime。
- 任一 Clipboard Technology 已被選擇。

## 25. Completion Conditions

- 只建立 docs/Research/Technology/34-clipboard-integration-official-prerequisite-evidence-baseline.md。
- Document ID 固定為 RESEARCH-TECH-CLIPBOARD-006。
- RESEARCH-TECH-CLIPBOARD-001..005 未修改。
- CLIP-EVID-001..018 與 CLIP-GAP-001..018 已完整檢視並在本文件標示重用限制。
- 建立連續 CLIP-OFF-EVID-001..020 Register。
- 建立連續 CLIP-OFF-GAP-001..020 Register。
- 建立五個 Candidate official identity records。
- 覆蓋十個 CLIP-PAIR。
- 建立 Format、Threading / COM、Ownership / Lifetime、Contention / Failure、History / Cloud、Packaging、Privacy baselines。
- 覆蓋六個 CLIP-ENABLE。
- 覆蓋十一個 CLIP-CGATE。
- 明確處理 CLIP-ENABLE-GAP-001。
- 沒有建立或虛構 UI-AUTH-*。
- 只使用 Microsoft 第一方來源作為主要證據。
- 沒有執行本機盤點、Package Cache 查詢、下載、安裝、Restore、Build、Run、Test、Clipboard 操作或 Runtime Spike。
- 沒有建立 Project、Consumer、Payload、Result、Source Code 或 runtime Evidence Artifact。
- 沒有修改 UI / Capture / Rendering Research Line。
- 沒有選擇 Clipboard Technology。
- 沒有建立 Clipboard ADR。
- 沒有開始 Clipboard 或截圖功能。
- 待完成的 read-only check：目標檔案狀態、ID coverage、leading plus、trailing whitespace、git diff --check。
