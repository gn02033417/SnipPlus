# Capture Backend Official Prerequisite Evidence Baseline

| Field | Value |
|---|---|
| Document ID | RESEARCH-TECH-CAPTURE-006 |
| Title | Capture Backend Official Prerequisite Evidence Baseline |
| Status | Draft |
| Research Type | Official Source Prerequisite Evidence Baseline |
| Parent Enablement Specification | RESEARCH-TECH-CAPTURE-005 |
| Parent Closure Plan | RESEARCH-TECH-CAPTURE-004 |
| Parent Readiness Record | RESEARCH-TECH-CAPTURE-003 |
| Parent Runtime Plan | RESEARCH-TECH-CAPTURE-002 |
| Parent Feasibility | RESEARCH-TECH-CAPTURE-001 |
| Official-source Research | Performed in this document only |
| Local Environment Inspection | Not performed |
| Package Cache Inspection | Not performed |
| Build Verification | Not performed |
| Runtime Verification | Not performed |
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

> 根據 Microsoft 第一方資料，Phase C1 所需的 Capture Candidate API／SDK 身分、Host activation／interop 路徑、平台與封裝需求、Graphics-device dependency、Frame／Pixel／Coordinate 行為及 Failure boundary，可以確定到什麼程度？

本文件用於補足 RESEARCH-TECH-CAPTURE-005 中可透過官方資料關閉的 specification gaps。

它不是：

- Capture Backend 重新評比文件。
- Local Inspection Record。
- Build Verification。
- Runtime Evidence。
- Authorization Request。
- Capture Backend Decision。
- Capture ADR。

## 2. Scope

研究範圍只包含：

- CAP-OPT-001 Windows Graphics Capture。
- CAP-OPT-002 DXGI Desktop Duplication。
- CAP-OPT-003 GDI-based capture。
- CAP-OPT-004 Window-oriented mechanisms。
- CAP-OPT-005 Hybrid primary／fallback strategy。
- WinUI 3 與 WPF Host。
- CAP-PAIR-001..010。
- CAP-ENABLE-001..007。
- CAP-CGATE-001..010。
- CAP-PREQ-001..030。
- 與 Phase C1 直接相關的 CAP-BLOCK。
- API、SDK、header、namespace、interop、graphics device、packaging、platform、frame 及 failure evidence。

Phase C2／C3 的 HDR、完整 Overlay exclusion、Cursor control、Device-loss、完整效能量測等項目保留 Evidence Gap，不因此擴大本文件為 Runtime validation。

## 3. Non-goals

本文件不得：

- 執行本機環境盤點。
- 查詢 Package Cache。
- 執行 dotnet --info、workload、AppX、Registry 或 SDK inventory。
- 下載或安裝 SDK、Runtime、Package、Tool 或 workload。
- Restore Package。
- 建立 Project、Solution、Prototype 或 Source Code。
- 執行 Build、Run、Publish 或 Capture API。
- 擷取桌面、視窗、螢幕或 Frame。
- 建立 Screenshot、PNG、Log、Result 或 Measurement Artifact。
- 修改 RESEARCH-TECH-CAPTURE-001..005。
- 修改 UI／Rendering Research Line。
- 修改 ADR-0002。
- 建立 Capture ADR。
- 選擇 Capture Backend。
- 將舊 PRD 的 WPF／WGC 偏好視為已核准決策。

## 4. Source Acceptance Policy

主要證據只能使用：

- Microsoft Learn。
- Windows SDK 官方 API Reference。
- Windows App SDK 官方文件。
- Microsoft 官方 GitHub Repository／Sample。
- Microsoft 官方平台需求、版本、封裝及安全限制文件。
- Microsoft 維護的 Package Registry metadata，如確實涉及 Package。

第三方資料只能記錄為 Informative，不得用來關閉 prerequisite。

不得把以下內容作為主要證據：

- 搜尋結果摘要。
- 個人部落格。
- Stack Overflow。
- 論壇留言。
- 未確認維護者的 wrapper 文件。
- AI 摘要。
- 僅能證明 Sample 存在、不能證明平台支援範圍的文章。

### 4.1 Official source list

| Source | URL | Use |
|---|---|---|
| Windows App SDK overview | https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/ | Host/framework/platform scope |
| Windows App SDK UI Interop | https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/win32/_uiinterop/ | HWND、HMONITOR and interop headers |
| Manage app windows | https://learn.microsoft.com/en-us/windows/apps/develop/ui/manage-app-windows | WinUI／WPF／Win32 window interop |
| Screen capture | https://learn.microsoft.com/en-us/windows/apps/develop/media-authoring-processing/screen-capture | WGC frame flow and pixel format |
| GraphicsCaptureItem API | https://learn.microsoft.com/en-us/uwp/api/windows.graphics.capture.graphicscaptureitem?view=winrt-28000 | WGC item identity and source methods |
| Desktop Duplication API | https://learn.microsoft.com/en-us/windows/win32/direct3ddxgi/desktop-dup-api | DXGI duplication and frame surface |
| BitBlt API | https://learn.microsoft.com/en-us/windows/win32/api/wingdi/nf-wingdi-bitblt | GDI transfer and CAPTUREBLT |
| PrintWindow API | https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-printwindow | Window-oriented HDC rendering |
| GetDIBits API | https://learn.microsoft.com/en-us/windows/win32/api/wingdi/nf-wingdi-getdibits | Bitmap／DIB readback |
| Windows app development | https://learn.microsoft.com/en-us/windows/apps/ | WinUI 3／WPF platform context |
| UI migration guidance | https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/migrate-to-windows-app-sdk/guides/winui3 | WPF／WinUI framework boundary |
| Desktop app guidance | https://learn.microsoft.com/en-us/windows/apps/desktop/ | API／package／deployment separation |

Access date for all sources in this document: 2026-07-26.

## 5. Controlled Vocabulary

### 5.1 Claim Status

只能使用：

- Confirmed by official source
- Partially confirmed
- Conflicting official evidence
- Unknown
- Not applicable

### 5.2 Host Support Status

只能使用：

- Officially documented
- API available, Host integration unverified
- Requires documented native interop
- Requires runtime prototype
- Not aligned by official evidence
- Unknown

### 5.3 Experimental Identity Status

只能使用：

- Candidate identity identified
- Partially identified
- Blocked by unresolved official evidence
- TBD
- Not applicable

### 5.4 Evidence Sufficiency

只能使用：

- Sufficient for static specification
- Partially sufficient
- Insufficient
- Conflicting
- Not applicable

不得使用：

- Best
- Winner
- Recommended for product
- Definitely compatible
- Should work
- Fast enough
- Production ready

## 6. Official Evidence Register

建立連續 Evidence ID；本文件只建立 CAP-OFF-EVID-001..012，不代表後續沒有新的官方證據。

| Evidence ID | Candidate | Official source title | Claim status | Related CAP-ENABLE | Related CAP-CGATE |
|---|---|---|---|---|---|
| CAP-OFF-EVID-001 | All | Windows App SDK overview | undefined | CAP-ENABLE-001, CAP-ENABLE-002 | CAP-CGATE-001, CAP-CGATE-002 |
| CAP-OFF-EVID-002 | WinUI 3／WPF | UI Interop | undefined | CAP-ENABLE-001, CAP-ENABLE-002 | CAP-CGATE-001, CAP-CGATE-003 |
| CAP-OFF-EVID-003 | WinUI 3／WPF | Manage app windows | undefined | CAP-ENABLE-001 | CAP-CGATE-001 |
| CAP-OFF-EVID-004 | Windows Graphics Capture | Screen capture | undefined | CAP-ENABLE-002, CAP-ENABLE-005 | CAP-CGATE-002, CAP-CGATE-003, CAP-CGATE-006 |
| CAP-OFF-EVID-005 | Windows Graphics Capture | GraphicsCaptureItem Class | undefined | CAP-ENABLE-002, CAP-ENABLE-005, CAP-ENABLE-007 | CAP-CGATE-002, CAP-CGATE-003, CAP-CGATE-010 |
| CAP-OFF-EVID-006 | DXGI Desktop Duplication | Desktop Duplication API | undefined | CAP-ENABLE-002, CAP-ENABLE-005 | CAP-CGATE-002, CAP-CGATE-003, CAP-CGATE-006 |
| CAP-OFF-EVID-007 | GDI-based capture | BitBlt function | undefined | CAP-ENABLE-002, CAP-ENABLE-005 | CAP-CGATE-002, CAP-CGATE-005 |
| CAP-OFF-EVID-008 | Window-oriented mechanisms | PrintWindow function | undefined | CAP-ENABLE-002, CAP-ENABLE-004, CAP-ENABLE-007 | CAP-CGATE-003, CAP-CGATE-010 |
| CAP-OFF-EVID-009 | GDI-based capture | GetDIBits function | undefined | CAP-ENABLE-005 | CAP-CGATE-006, CAP-CGATE-007 |
| CAP-OFF-EVID-010 | WinUI 3／WPF | Windows app development and UI migration | undefined | CAP-ENABLE-001, CAP-ENABLE-003 | CAP-CGATE-001, CAP-CGATE-003 |
| CAP-OFF-EVID-011 | Windows Graphics Capture | Screen capture pixel format note | undefined | CAP-ENABLE-004, CAP-ENABLE-005 | CAP-CGATE-004, CAP-CGATE-007 |
| CAP-OFF-EVID-012 | WinUI 3／WPF | Build desktop apps for Windows | undefined | CAP-ENABLE-001, CAP-ENABLE-003 | CAP-CGATE-001, CAP-CGATE-008 |

每個實質技術 claim 必須至少引用一個 CAP-OFF-EVID。每筆 Evidence 的完整固定欄位如下：

### CAP-OFF-EVID-001

| Field | Value |
| --- | --- |
| Evidence ID | CAP-OFF-EVID-001 |
| Claim | Windows App SDK 可加入 WPF、WinForms、Win32 等既有 desktop app；Windows 10 1809+ platform scope |
| Candidate | All |
| Host | WinUI 3／WPF |
| Related CAP-PAIR | CAP-PAIR-001..010 |
| Related CAP-PREQ | CAP-PREQ-001, CAP-PREQ-003 |
| Related CAP-BLOCK | CAP-BLOCK-001, CAP-BLOCK-002 |
| Related CAP-ENABLE | CAP-ENABLE-001, CAP-ENABLE-002 |
| Related CAP-CGATE | CAP-CGATE-001, CAP-CGATE-002 |
| Official source title | Windows App SDK overview |
| Official publisher／maintainer | Microsoft |
| Source URL | https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/ |
| Publication／update date | Not stated in captured source |
| Access date | 2026-07-26 |
| API／SDK／Package identity | Windows App SDK identity; WinUI 3 and existing desktop frameworks |
| Supported Windows version | Windows 10 1809+ per source overview |
| Supported architecture | x86/x64/ARM claims require version-specific confirmation |
| Required header／namespace／assembly | Managed／native boundary remains host-specific |
| Packaging context | Officially documented; release channel matters |
| Managed／native boundary | Confirmed by official source |
| Relevant limitation | Static host/platform scope only; not local availability or runtime proof |
| Claim status |  |
| Decision implication |  |
| Runtime validation still required | Yes；本文件只建立 static official evidence，未執行 runtime。 |

### CAP-OFF-EVID-002

| Field | Value |
| --- | --- |
| Evidence ID | CAP-OFF-EVID-002 |
| Claim | Windows App SDK UI Interop exposes HWND／HMONITOR conversion and named interop headers |
| Candidate | WinUI 3／WPF |
| Host | WinUI 3 and WPF interop |
| Related CAP-PAIR | CAP-PAIR-001..010 |
| Related CAP-PREQ | CAP-PREQ-003, CAP-PREQ-004 |
| Related CAP-BLOCK | CAP-BLOCK-001, CAP-BLOCK-002 |
| Related CAP-ENABLE | CAP-ENABLE-001, CAP-ENABLE-002 |
| Related CAP-CGATE | CAP-CGATE-001, CAP-CGATE-003 |
| Official source title | UI Interop |
| Official publisher／maintainer | Microsoft |
| Source URL | https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/win32/_uiinterop/ |
| Publication／update date | 2025-03-19 page update |
| Access date | 2026-07-26 |
| API／SDK／Package identity | winrt/microsoft.ui.interop.h and related headers |
| Supported Windows version | Windows App SDK interop surface |
| Supported architecture | Architecture-specific native headers remain to be selected |
| Required header／namespace／assembly | WinRT／COM／native interop |
| Packaging context | Officially documented |
| Managed／native boundary | Confirmed by official source |
| Relevant limitation | Interop API existence only; capture integration remains unverified |
| Claim status |  |
| Decision implication |  |
| Runtime validation still required | Yes；本文件只建立 static official evidence，未執行 runtime。 |

### CAP-OFF-EVID-003

| Field | Value |
| --- | --- |
| Evidence ID | CAP-OFF-EVID-003 |
| Claim | AppWindow works alongside WinUI, WPF, WinForms and Win32 framework window APIs |
| Candidate | WinUI 3／WPF |
| Host | WinUI 3 and WPF |
| Related CAP-PAIR | CAP-PAIR-001..010 |
| Related CAP-PREQ | CAP-PREQ-001, CAP-PREQ-003 |
| Related CAP-BLOCK | CAP-BLOCK-001 |
| Related CAP-ENABLE | CAP-ENABLE-001 |
| Related CAP-CGATE | CAP-CGATE-001 |
| Official source title | Manage app windows |
| Official publisher／maintainer | Microsoft |
| Source URL | https://learn.microsoft.com/en-us/windows/apps/develop/ui/manage-app-windows |
| Publication／update date | Not stated in captured source |
| Access date | 2026-07-26 |
| API／SDK／Package identity | AppWindow and Microsoft.UI.Win32Interop |
| Supported Windows version | Windows App SDK supported desktop frameworks |
| Supported architecture | Exact target framework and package identity TBD |
| Required header／namespace／assembly | HWND／AppWindow interop |
| Packaging context | Officially documented |
| Managed／native boundary | Confirmed by official source |
| Relevant limitation | Window management support is not capture support |
| Claim status |  |
| Decision implication |  |
| Runtime validation still required | Yes；本文件只建立 static official evidence，未執行 runtime。 |

### CAP-OFF-EVID-004

| Field | Value |
| --- | --- |
| Evidence ID | CAP-OFF-EVID-004 |
| Claim | Windows.Graphics.Capture acquires frames from a display or application window; frame pool uses a D3D device and supported pixel format |
| Candidate | Windows Graphics Capture |
| Host | WinUI 3／WPF host to be verified |
| Related CAP-PAIR | CAP-PAIR-001, CAP-PAIR-002 |
| Related CAP-PREQ | CAP-PREQ-003..011, CAP-PREQ-022 |
| Related CAP-BLOCK | CAP-BLOCK-002 |
| Related CAP-ENABLE | CAP-ENABLE-002, CAP-ENABLE-005 |
| Related CAP-CGATE | CAP-CGATE-002, CAP-CGATE-003, CAP-CGATE-006 |
| Official source title | Screen capture |
| Official publisher／maintainer | Microsoft |
| Source URL | https://learn.microsoft.com/en-us/windows/apps/develop/media-authoring-processing/screen-capture |
| Publication／update date | 2025 page update shown by search |
| Access date | 2026-07-26 |
| API／SDK／Package identity | Windows.Graphics.Capture; Direct3D11CaptureFramePool; DXGI_FORMAT_B8G8R8A8_UNORM |
| Supported Windows version | Windows 10 1803+ API family; exact app SDK version TBD |
| Supported architecture | D3D device and frame-pool integration |
| Required header／namespace／assembly | WinRT／Direct3D11 boundary |
| Packaging context | Officially documented |
| Managed／native boundary | Confirmed by official source |
| Relevant limitation | HD color note prevents generalizing one pixel format to every display mode |
| Claim status |  |
| Decision implication |  |
| Runtime validation still required | Yes；本文件只建立 static official evidence，未執行 runtime。 |

### CAP-OFF-EVID-005

| Field | Value |
| --- | --- |
| Evidence ID | CAP-OFF-EVID-005 |
| Claim | GraphicsCaptureItem identifies capture target and exposes display/window creation methods plus Size and Closed |
| Candidate | Windows Graphics Capture |
| Host | WinUI 3／WPF host to be verified |
| Related CAP-PAIR | CAP-PAIR-001, CAP-PAIR-002 |
| Related CAP-PREQ | CAP-PREQ-003..006, CAP-PREQ-022 |
| Related CAP-BLOCK | CAP-BLOCK-002 |
| Related CAP-ENABLE | CAP-ENABLE-002, CAP-ENABLE-005, CAP-ENABLE-007 |
| Related CAP-CGATE | CAP-CGATE-002, CAP-CGATE-003, CAP-CGATE-010 |
| Official source title | GraphicsCaptureItem Class |
| Official publisher／maintainer | Microsoft |
| Source URL | https://learn.microsoft.com/en-us/uwp/api/windows.graphics.capture.graphicscaptureitem?view=winrt-28000 |
| Publication／update date | Version history includes Windows 10 1803 and 1809 method additions |
| Access date | 2026-07-26 |
| API／SDK／Package identity | Windows.Graphics.Capture.GraphicsCaptureItem |
| Supported Windows version | Windows 10 version 1803 API contract v6.0 |
| Supported architecture | WindowId／DisplayId to source mapping must be verified per host |
| Required header／namespace／assembly | WinRT object and host interop |
| Packaging context | Officially documented |
| Managed／native boundary | Confirmed by official source |
| Relevant limitation | Creation method existence does not prove permission, picker or product UX |
| Claim status |  |
| Decision implication |  |
| Runtime validation still required | Yes；本文件只建立 static official evidence，未執行 runtime。 |

### CAP-OFF-EVID-006

| Field | Value |
| --- | --- |
| Evidence ID | CAP-OFF-EVID-006 |
| Claim | Desktop Duplication exposes IDXGIOutputDuplication::AcquireNextFrame and B8G8R8A8_UNORM desktop surface |
| Candidate | DXGI Desktop Duplication |
| Host | WinUI 3／WPF through native interop |
| Related CAP-PAIR | CAP-PAIR-003, CAP-PAIR-004 |
| Related CAP-PREQ | CAP-PREQ-005..011, CAP-PREQ-022 |
| Related CAP-BLOCK | CAP-BLOCK-002 |
| Related CAP-ENABLE | CAP-ENABLE-002, CAP-ENABLE-005 |
| Related CAP-CGATE | CAP-CGATE-002, CAP-CGATE-003, CAP-CGATE-006 |
| Official source title | Desktop Duplication API |
| Official publisher／maintainer | Microsoft |
| Source URL | https://learn.microsoft.com/en-us/windows/win32/direct3ddxgi/desktop-dup-api |
| Publication／update date | Not stated in captured source |
| Access date | 2026-07-26 |
| API／SDK／Package identity | IDXGIOutputDuplication; D3D11 texture; DXGI |
| Supported Windows version | Windows 8+ API family per official article |
| Supported architecture | One output duplication object and multi-output assembly remain runtime questions |
| Required header／namespace／assembly | Native DXGI／D3D11 boundary |
| Packaging context | Officially documented |
| Managed／native boundary | Confirmed by official source |
| Relevant limitation | Official article describes desktop surface, not SnipPlus crop contract |
| Claim status |  |
| Decision implication |  |
| Runtime validation still required | Yes；本文件只建立 static official evidence，未執行 runtime。 |

### CAP-OFF-EVID-007

| Field | Value |
| --- | --- |
| Evidence ID | CAP-OFF-EVID-007 |
| Claim | BitBlt copies pixels between device contexts, uses logical units, supports CAPTUREBLT semantics and requires Gdi32／wingdi.h |
| Candidate | GDI-based capture |
| Host | WinUI 3／WPF through Win32 interop |
| Related CAP-PAIR | CAP-PAIR-005, CAP-PAIR-006 |
| Related CAP-PREQ | CAP-PREQ-005, CAP-PREQ-014..016 |
| Related CAP-BLOCK | CAP-BLOCK-002, CAP-BLOCK-004 |
| Related CAP-ENABLE | CAP-ENABLE-002, CAP-ENABLE-005 |
| Related CAP-CGATE | CAP-CGATE-002, CAP-CGATE-005 |
| Official source title | BitBlt function |
| Official publisher／maintainer | Microsoft |
| Source URL | https://learn.microsoft.com/en-us/windows/win32/api/wingdi/nf-wingdi-bitblt |
| Publication／update date | 2021-10-13 page update |
| Access date | 2026-07-26 |
| API／SDK／Package identity | wingdi.h; Gdi32.lib; Gdi32.dll; HDC |
| Supported Windows version | Desktop Windows API; minimum Windows 2000 client |
| Supported architecture | DPI awareness, physical pixels, device compatibility and readback remain separate |
| Required header／namespace／assembly | Native GDI／managed host boundary |
| Packaging context | Officially documented |
| Managed／native boundary | Confirmed by official source |
| Relevant limitation | API existence does not establish product screen semantics |
| Claim status |  |
| Decision implication |  |
| Runtime validation still required | Yes；本文件只建立 static official evidence，未執行 runtime。 |

### CAP-OFF-EVID-008

| Field | Value |
| --- | --- |
| Evidence ID | CAP-OFF-EVID-008 |
| Claim | PrintWindow copies a visual window to an HDC; it is synchronous and owner application handles WM_PRINT/WM_PRINTCLIENT |
| Candidate | Window-oriented mechanisms |
| Host | WinUI 3／WPF through HWND |
| Related CAP-PAIR | CAP-PAIR-007, CAP-PAIR-008 |
| Related CAP-PREQ | CAP-PREQ-017..019, CAP-PREQ-028 |
| Related CAP-BLOCK | CAP-BLOCK-005, CAP-BLOCK-006, CAP-BLOCK-011 |
| Related CAP-ENABLE | CAP-ENABLE-002, CAP-ENABLE-004, CAP-ENABLE-007 |
| Related CAP-CGATE | CAP-CGATE-003, CAP-CGATE-010 |
| Official source title | PrintWindow function |
| Official publisher／maintainer | Microsoft |
| Source URL | https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-printwindow |
| Publication／update date | 2024-02-22 page update |
| Access date | 2026-07-26 |
| API／SDK／Package identity | winuser.h; User32.lib; HWND; HDC |
| Supported Windows version | Desktop Windows API; minimum Windows XP client |
| Supported architecture | Occlusion, minimized state, owner behavior and composition limitations remain |
| Required header／namespace／assembly | Native User32／managed host boundary |
| Packaging context | Officially documented |
| Managed／native boundary | Confirmed by official source |
| Relevant limitation | Synchronous owner rendering is not equivalent to desktop capture |
| Claim status |  |
| Decision implication |  |
| Runtime validation still required | Yes；本文件只建立 static official evidence，未執行 runtime。 |

### CAP-OFF-EVID-009

| Field | Value |
| --- | --- |
| Evidence ID | CAP-OFF-EVID-009 |
| Claim | GetDIBits retrieves bitmap bits into a DIB and documents bottom-up／top-down origin behavior |
| Candidate | GDI-based capture |
| Host | WinUI 3／WPF through Win32 interop |
| Related CAP-PAIR | CAP-PAIR-005, CAP-PAIR-006 |
| Related CAP-PREQ | CAP-PREQ-022, CAP-PREQ-023 |
| Related CAP-BLOCK | CAP-BLOCK-004, CAP-BLOCK-007 |
| Related CAP-ENABLE | CAP-ENABLE-005 |
| Related CAP-CGATE | CAP-CGATE-006, CAP-CGATE-007 |
| Official source title | GetDIBits function |
| Official publisher／maintainer | Microsoft |
| Source URL | https://learn.microsoft.com/en-us/windows/win32/api/wingdi/nf-wingdi-getdibits |
| Publication／update date | 2022-12-06 page update |
| Access date | 2026-07-26 |
| API／SDK／Package identity | wingdi.h; Gdi32.lib; HBITMAP; DIB |
| Supported Windows version | Desktop Windows API; minimum Windows 2000 client |
| Supported architecture | Crop edge, channel order and pixel comparison policy remain product-specific |
| Required header／namespace／assembly | Native bitmap／managed buffer boundary |
| Packaging context | Officially documented |
| Managed／native boundary | Confirmed by official source |
| Relevant limitation | DIB transfer is not PNG persistence or fidelity proof |
| Claim status |  |
| Decision implication |  |
| Runtime validation still required | Yes；本文件只建立 static official evidence，未執行 runtime。 |

### CAP-OFF-EVID-010

| Field | Value |
| --- | --- |
| Evidence ID | CAP-OFF-EVID-010 |
| Claim | Windows App SDK supports WinUI 3 and modernizing existing WPF apps, but framework API surfaces differ |
| Candidate | WinUI 3／WPF |
| Host | WinUI 3 and WPF |
| Related CAP-PAIR | CAP-PAIR-001, CAP-PAIR-002 |
| Related CAP-PREQ | CAP-PREQ-001, CAP-PREQ-003, CAP-PREQ-027 |
| Related CAP-BLOCK | CAP-BLOCK-001, CAP-BLOCK-010 |
| Related CAP-ENABLE | CAP-ENABLE-001, CAP-ENABLE-003 |
| Related CAP-CGATE | CAP-CGATE-001, CAP-CGATE-003 |
| Official source title | Windows app development and UI migration |
| Official publisher／maintainer | Microsoft |
| Source URL | https://learn.microsoft.com/en-us/windows/apps/; https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/migrate-to-windows-app-sdk/guides/winui3 |
| Publication／update date | 2026 page updates shown by search |
| Access date | 2026-07-26 |
| API／SDK／Package identity | WinUI 3; Windows App SDK; WPF interop |
| Supported Windows version | Windows 10 1809+ for Windows App SDK overview |
| Supported architecture | Project target framework and package mode remain unverified |
| Required header／namespace／assembly | Framework／Win32／WinRT boundary |
| Packaging context | Officially documented |
| Managed／native boundary | Confirmed by official source |
| Relevant limitation | Migration guidance is not proof of capture integration |
| Claim status |  |
| Decision implication |  |
| Runtime validation still required | Yes；本文件只建立 static official evidence，未執行 runtime。 |

### CAP-OFF-EVID-011

| Field | Value |
| --- | --- |
| Evidence ID | CAP-OFF-EVID-011 |
| Claim | Screen capture guidance notes HD color can change content pixel format from the common BGRA format |
| Candidate | Windows Graphics Capture |
| Host | WinUI 3／WPF host to be verified |
| Related CAP-PAIR | CAP-PAIR-001, CAP-PAIR-002 |
| Related CAP-PREQ | CAP-PREQ-013, CAP-PREQ-016, CAP-PREQ-022 |
| Related CAP-BLOCK | CAP-BLOCK-003, CAP-BLOCK-007 |
| Related CAP-ENABLE | CAP-ENABLE-004, CAP-ENABLE-005 |
| Related CAP-CGATE | CAP-CGATE-004, CAP-CGATE-007 |
| Official source title | Screen capture pixel format note |
| Official publisher／maintainer | Microsoft |
| Source URL | https://learn.microsoft.com/en-us/windows/apps/develop/media-authoring-processing/screen-capture |
| Publication／update date | 2025 page update shown by search |
| Access date | 2026-07-26 |
| API／SDK／Package identity | CaptureDirectXPixelFormat; DXGI format note |
| Supported Windows version | Windows HD color context |
| Supported architecture | Exact format negotiation and color metadata require later evidence |
| Required header／namespace／assembly | GPU texture／pixel metadata |
| Packaging context | Officially documented |
| Managed／native boundary | Confirmed by official source |
| Relevant limitation | Does not close HDR or color-fidelity runtime gaps |
| Claim status |  |
| Decision implication |  |
| Runtime validation still required | Yes；本文件只建立 static official evidence，未執行 runtime。 |

### CAP-OFF-EVID-012

| Field | Value |
| --- | --- |
| Evidence ID | CAP-OFF-EVID-012 |
| Claim | Official desktop app guidance separates API reference, packaging and deployment from runtime support decisions |
| Candidate | WinUI 3／WPF |
| Host | All hosts |
| Related CAP-PAIR | CAP-PAIR-001..010 |
| Related CAP-PREQ | CAP-PREQ-001, CAP-PREQ-027 |
| Related CAP-BLOCK | CAP-BLOCK-001, CAP-BLOCK-010 |
| Related CAP-ENABLE | CAP-ENABLE-001, CAP-ENABLE-003 |
| Related CAP-CGATE | CAP-CGATE-001, CAP-CGATE-008 |
| Official source title | Build desktop apps for Windows |
| Official publisher／maintainer | Microsoft |
| Source URL | https://learn.microsoft.com/en-us/windows/apps/desktop/ |
| Publication／update date | Not stated in captured source |
| Access date | 2026-07-26 |
| API／SDK／Package identity | Windows App SDK and Windows SDK documentation families |
| Supported Windows version | Windows 10／11 desktop scope |
| Supported architecture | Package／deployment option and local tool availability remain unknown |
| Required header／namespace／assembly | SDK／package／host boundary |
| Packaging context | Confirmed by official source |
| Managed／native boundary | Documentation topology is not build evidence |
| Relevant limitation |  |
| Claim status |  |
| Decision implication |  |
| Runtime validation still required | Yes；本文件只建立 static official evidence，未執行 runtime。 |


## 7. Candidate API Identity Baseline

建立五個主要 Candidate records：

| Candidate ID | Candidate | Exact official API family | Technology owner | SDK／header／namespace | Managed／native boundary | Minimum platform | Evidence | Experimental identity status |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| CAP-OPT-001 | Windows Graphics Capture | Windows.Graphics.Capture; GraphicsCaptureItem; Direct3D11CaptureFramePool; GraphicsCaptureSession | Microsoft | Windows.Graphics.Capture; Windows.Graphics.DirectX.Direct3D11; WinRT projection | WinRT item/session to D3D11 frame pool | Windows 10 1803 API family; exact project target TBD | CAP-OFF-EVID-004, CAP-OFF-EVID-005, CAP-OFF-EVID-011 | Candidate identity identified |
| CAP-OPT-002 | DXGI Desktop Duplication | IDXGIOutputDuplication; AcquireNextFrame; D3D11 texture | Microsoft | DXGI 1.2; D3D11; native COM | DXGI/D3D11 surface to managed host | Windows 8+ API family; exact project target TBD | CAP-OFF-EVID-006 | Candidate identity identified |
| CAP-OPT-003 | GDI-based capture | BitBlt; GetDIBits; HDC; HBITMAP | Microsoft | wingdi.h; Gdi32.lib; User32 interop where needed | Native HDC/DIB to managed host | Desktop Windows API; exact project target TBD | CAP-OFF-EVID-007, CAP-OFF-EVID-009 | Candidate identity identified |
| CAP-OPT-004 | Window-oriented mechanisms | PrintWindow; HWND/HDC; related window APIs kept separate | Microsoft | winuser.h; User32.lib | Native HWND/HDC to managed host | Desktop Windows API; exact host behavior TBD | CAP-OFF-EVID-008 | Partially identified |
| CAP-OPT-005 | Hybrid primary/fallback strategy | Constituent WGC/DXGI/GDI/window APIs; no single API identity | Microsoft APIs as applicable | Each constituent identity listed separately | Multiple native/WinRT paths with shared product contract TBD | Candidate-dependent | CAP-OFF-EVID-004, CAP-OFF-EVID-006, CAP-OFF-EVID-007, CAP-OFF-EVID-008 | Blocked by unresolved official evidence |


### 7.1 Windows Graphics Capture

至少查證並由 CAP-OFF-EVID-004、005、011 支援：

- Capture API family 的正式名稱。
- Capture item 建立途徑。
- Picker 與 HWND／monitor interop 是否為不同路徑。
- Frame pool、session 與 graphics-device dependency。
- Supported pixel format evidence。
- Cursor、border 或 related session properties，如官方有明確資料。
- Packaged／unpackaged 限制。
- WinUI 3／WPF activation boundary。

結論只到 static identity：Windows.Graphics.Capture API family 已由官方文件確認；WPF integration、local availability、package version、runtime frame fidelity 仍未確認。

### 7.2 DXGI Desktop Duplication

至少查證並由 CAP-OFF-EVID-006 支援：

- Desktop Duplication API 的正式 interface identity。
- D3D11 device requirement。
- Adapter／output dependency。
- Desktop frame、dirty／move rect及 pointer metadata。
- Rotation、multi-output與 access-lost behavior。
- Windows platform requirement。
- 是否需要逐 Output 處理而非單一 Virtual Desktop frame。

結論只到 static identity：IDXGIOutputDuplication 與 AcquireNextFrame 已由官方文件確認；SnipPlus virtual desktop composition、crop contract、host integration 仍未確認。

### 7.3 GDI-based Capture

至少查證並由 CAP-OFF-EVID-007、009 支援：

- Device context、bitmap與 copy operation identity。
- Bit-block transfer 相關 API。
- Layered window／CAPTUREBLT 等官方語意，如適用。
- Physical pixel與 DPI awareness 的責任邊界。
- GPU／HDR／protected-content 限制是否有官方證據。
- GDI API 存在與產品適用性必須分開。

結論只到 static identity：GDI API 與 header／library 已由官方文件確認；產品 capture semantics、DPI、readback、fidelity 仍未確認。

### 7.4 Window-oriented Mechanisms

至少分離並由 CAP-OFF-EVID-005、008 支援：

- Window handle-oriented capture。
- Windows Graphics Capture 的 window item。
- GDI／PrintWindow 類型機制。
- Occluded、minimized及非 client／composition content 限制。
- 不得把多個不同 API 合併成單一官方機制。

結論只到 API identities separately identified；沒有統一的 Window-oriented 官方 Candidate。

### 7.5 Hybrid Strategy

必須記錄：

- 它是策略，不是單一官方 API。
- Primary candidate。
- Fallback candidate。
- Trigger condition。
- Shared frame／coordinate contract requirement。
- 不同 API 產出語意不一致的風險。
- 所有組成 identity 未固定時保持 TBD。

本文件不把 Hybrid 排名，也不把它標記為 product recommendation。

## 8. Candidate–Host Official Compatibility Matrix

覆蓋 CAP-PAIR-001..010：

| Pair | Candidate | Host | Official invocation evidence | Activation／interop route | Graphics-device dependency | Packaging dependency | Host support status | Evidence IDs |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| CAP-PAIR-001 | CAP-OPT-001 | WinUI 3 | CAP-OFF-EVID-004, CAP-OFF-EVID-005 | See activation matrix; host route is not runtime verified | See device matrix; candidate-specific | TBD; no local package inspection | API available, Host integration unverified | CAP-OFF-EVID-004, CAP-OFF-EVID-005 |
| CAP-PAIR-002 | CAP-OPT-001 | WPF | CAP-OFF-EVID-004, CAP-OFF-EVID-005 | See activation matrix; host route is not runtime verified | See device matrix; candidate-specific | TBD; no local package inspection | API available, Host integration unverified | CAP-OFF-EVID-004, CAP-OFF-EVID-005 |
| CAP-PAIR-003 | CAP-OPT-002 | WinUI 3 | CAP-OFF-EVID-006 | See activation matrix; host route is not runtime verified | See device matrix; candidate-specific | TBD; no local package inspection | Requires documented native interop | CAP-OFF-EVID-006 |
| CAP-PAIR-004 | CAP-OPT-002 | WPF | CAP-OFF-EVID-006 | See activation matrix; host route is not runtime verified | See device matrix; candidate-specific | TBD; no local package inspection | Requires documented native interop | CAP-OFF-EVID-006 |
| CAP-PAIR-005 | CAP-OPT-003 | WinUI 3 | CAP-OFF-EVID-007, CAP-OFF-EVID-009 | See activation matrix; host route is not runtime verified | See device matrix; candidate-specific | TBD; no local package inspection | Requires documented native interop | CAP-OFF-EVID-007, CAP-OFF-EVID-009 |
| CAP-PAIR-006 | CAP-OPT-003 | WPF | CAP-OFF-EVID-007, CAP-OFF-EVID-009 | See activation matrix; host route is not runtime verified | See device matrix; candidate-specific | TBD; no local package inspection | Requires documented native interop | CAP-OFF-EVID-007, CAP-OFF-EVID-009 |
| CAP-PAIR-007 | CAP-OPT-004 | WinUI 3 | CAP-OFF-EVID-008 | See activation matrix; host route is not runtime verified | See device matrix; candidate-specific | TBD; no local package inspection | Requires documented native interop | CAP-OFF-EVID-008 |
| CAP-PAIR-008 | CAP-OPT-004 | WPF | CAP-OFF-EVID-008 | See activation matrix; host route is not runtime verified | See device matrix; candidate-specific | TBD; no local package inspection | Requires documented native interop | CAP-OFF-EVID-008 |
| CAP-PAIR-009 | CAP-OPT-005 | WinUI 3 | CAP-OFF-EVID-004, CAP-OFF-EVID-006, CAP-OFF-EVID-007, CAP-OFF-EVID-008 | See activation matrix; host route is not runtime verified | See device matrix; candidate-specific | TBD; no local package inspection | Unknown | CAP-OFF-EVID-004, CAP-OFF-EVID-006, CAP-OFF-EVID-007, CAP-OFF-EVID-008 |
| CAP-PAIR-010 | CAP-OPT-005 | WPF | CAP-OFF-EVID-004, CAP-OFF-EVID-006, CAP-OFF-EVID-007, CAP-OFF-EVID-008 | See activation matrix; host route is not runtime verified | See device matrix; candidate-specific | TBD; no local package inspection | Unknown | CAP-OFF-EVID-004, CAP-OFF-EVID-006, CAP-OFF-EVID-007, CAP-OFF-EVID-008 |


要求：

- 十個 Pair 各一列。
- WinUI 3 與 WPF 獨立。
- API 可從 .NET 呼叫不等於 Host integration 已驗證。
- Sample 存在不等於所有封裝模式均支援。
- Unknown 不得轉為 Not aligned。
- Not aligned by official evidence 必須有明確官方限制。
- 不因舊 PRD 指定 WPF／WGC 而提高 Evidence Status。

## 9. Activation and Interop Matrix

| Candidate／Host | Capture source creation | Required handle／object | Interop interface／factory | Thread／dispatcher constraint | Consent／picker implication | Evidence |
| --- | --- | --- | --- | --- | --- | --- |
| WGC × WinUI 3 | GraphicsCaptureItem from picker／DisplayId／WindowId | HWND／HMONITOR or WindowId／DisplayId | Windows App SDK interop and WinRT projection | Dispatcher／thread specifics TBD | Picker／consent behavior documented where applicable | CAP-OFF-EVID-002, CAP-OFF-EVID-004, CAP-OFF-EVID-005 |
| WGC × WPF | GraphicsCaptureItem via documented or future interop path | HWND／HMONITOR or WindowId／DisplayId | WPF HWND plus WinRT／COM interop TBD | Host integration unverified | Picker／consent behavior requires host test | CAP-OFF-EVID-002, CAP-OFF-EVID-003, CAP-OFF-EVID-005 |
| DXGI × WinUI 3 | IDXGIOutputDuplication per output | IDXGIOutput／D3D11 device | Native COM／D3D11 interop | Threading and device lifetime TBD | No picker claim from Duplication API | CAP-OFF-EVID-006, CAP-OFF-EVID-010 |
| DXGI × WPF | IDXGIOutputDuplication per output | IDXGIOutput／D3D11 device | Native COM／D3D11 interop | Host integration unverified | No picker claim from Duplication API | CAP-OFF-EVID-006 |
| GDI × WinUI 3 | HDC／HBITMAP via Win32 | HWND／HDC／HBITMAP | wingdi.h／User32 interop | UI thread impact TBD | No consent claim from BitBlt | CAP-OFF-EVID-002, CAP-OFF-EVID-007, CAP-OFF-EVID-009 |
| GDI × WPF | HDC／HBITMAP via Win32 | HWND／HDC／HBITMAP | wingdi.h／User32 interop | UI thread impact TBD | No consent claim from BitBlt | CAP-OFF-EVID-007, CAP-OFF-EVID-009 |
| Window mechanisms × WinUI 3 | PrintWindow or separately evidenced window API | HWND／HDC | winuser.h／User32 interop | Synchronous owner-render behavior | No full desktop or overlay exclusion claim | CAP-OFF-EVID-008 |
| Window mechanisms × WPF | PrintWindow or separately evidenced window API | HWND／HDC | winuser.h／User32 interop | Synchronous owner-render behavior | Minimized／occluded semantics open | CAP-OFF-EVID-008 |
| Hybrid × WinUI 3 | Constituent API selected later | Candidate-dependent | Multiple interop boundaries | Shared contract TBD | Fallback trigger TBD | CAP-OFF-EVID-004, CAP-OFF-EVID-006, CAP-OFF-EVID-007, CAP-OFF-EVID-008 |
| Hybrid × WPF | Constituent API selected later | Candidate-dependent | Multiple interop boundaries | Shared contract TBD | Fallback trigger TBD | CAP-OFF-EVID-004, CAP-OFF-EVID-006, CAP-OFF-EVID-007, CAP-OFF-EVID-008 |


沒有官方明確證據時使用 Unknown。此矩陣描述官方 API boundary，不申請任何 Capture authority。

## 10. SDK、Header、Namespace and Package Matrix

| Candidate integration | SDK／Package identity | Header／namespace／assembly | Target framework implication | Native library | Package acquisition required | Evidence |
| --- | --- | --- | --- | --- | --- | --- |
| Windows Graphics Capture | Windows SDK／Windows App SDK identity TBD | Windows.Graphics.Capture; Windows.Graphics.DirectX.Direct3D11; WinRT projection | Windows 10 1803 API family; target framework TBD | Direct3D11／DXGI | No package acquisition performed | CAP-OFF-EVID-004, CAP-OFF-EVID-005 |
| DXGI Desktop Duplication | Windows SDK identity TBD | dxgi1_2.h; d3d11.h; COM interfaces | Native target framework TBD | D3D11／DXGI | No package acquisition performed | CAP-OFF-EVID-006 |
| GDI-based capture | Windows SDK identity TBD | wingdi.h; winuser.h; HDC/HBITMAP | Desktop target framework TBD | GDI／DIB | No package acquisition performed | CAP-OFF-EVID-007, CAP-OFF-EVID-009 |
| Window-oriented mechanisms | Windows SDK identity TBD | winuser.h; HWND/HDC | Desktop target framework TBD | User32／GDI | No package acquisition performed | CAP-OFF-EVID-008 |
| Windows App SDK host interop | Windows App SDK package identity TBD | winrt/microsoft.ui.interop.h; Microsoft.UI.Win32Interop | WinUI 3／WPF integration target TBD | WinRT／COM／HWND | No package acquisition performed | CAP-OFF-EVID-001, CAP-OFF-EVID-002, CAP-OFF-EVID-003 |
| Hybrid strategy | Constituent identities must be listed | Each constituent header／namespace | Candidate-dependent | Multiple boundaries | No package acquisition performed | CAP-OFF-EVID-004, CAP-OFF-EVID-006, CAP-OFF-EVID-007, CAP-OFF-EVID-008 |


要求：

- Windows SDK API 不得虛構成 NuGet Package。
- WinRT projection、Windows App SDK及 Windows SDK 必須分開。
- 官方 Sample 中的 helper 不得誤寫成平台 API。
- Managed wrapper 與原生平台 API 必須分開。
- Package acquisition required 只能描述未來實驗需求，不代表已授權。
- 本機 availability 一律 Unknown。

## 11. Graphics Device and Frame Dependency Matrix

| Candidate | Graphics device requirement | Adapter／output relationship | Frame acquisition object | Pixel format evidence | Alpha behavior | CPU readback implication | Evidence |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Windows Graphics Capture | D3D device required for Direct3D11CaptureFramePool | Source item size; display／window item | Direct3D11CaptureFrame | BGRA documented; HD color caveat | GPU texture to CPU readback TBD | CAP-OFF-EVID-004, CAP-OFF-EVID-011 |
| DXGI Desktop Duplication | D3D11 device／context | One IDXGIOutputDuplication per output | DXGI surface from AcquireNextFrame | B8G8R8A8_UNORM documented | CPU processing not product-verified | CAP-OFF-EVID-006 |
| GDI-based capture | Device contexts and compatible bitmap | Source／destination DC compatibility | HBITMAP／DIB | Format conversion documented; policy TBD | GetDIBits readback documented | CAP-OFF-EVID-007, CAP-OFF-EVID-009 |
| Window-oriented mechanisms | HDC target and owner window | HWND source | HDC visual result | Pixel semantics depend on owner render | Readback method separate | CAP-OFF-EVID-008, CAP-OFF-EVID-009 |
| Hybrid strategy | Each constituent device path | Candidate-dependent | Mixed frame objects | Shared pixel contract TBD | Conversion and comparison TBD | CAP-OFF-EVID-004, CAP-OFF-EVID-006, CAP-OFF-EVID-007, CAP-OFF-EVID-009 |


要求：

- D3D11、DXGI與 WinRT graphics-device role 必須分開。
- 不得憑推測填寫 alpha channel 語意。
- Frame 可取得不代表可直接輸出 PNG。
- GPU texture 可用不代表 CPU crop／pixel comparison 已完成。
- CPU readback 沒有官方完整證據時標示 Unknown 或 Requires runtime prototype。

## 12. Coordinate and Multi-monitor Official Evidence

| Capability | Candidate | Official claim | Coordinate domain | Multi-monitor implication | Remaining runtime need | Evidence |
| --- | --- | --- | --- | --- | --- | --- |
| Virtual desktop | All | Official bounds are API／environment inputs, not product contract | Physical／logical domain TBD | Multi-monitor assembly TBD | CAP-OFF-GAP-009 | CAP-OFF-EVID-006, CAP-OFF-EVID-007 |
| Per-output capture | DXGI Desktop Duplication | Duplication is output-oriented | Output-local physical pixels | Virtual composition not automatic | CAP-OFF-GAP-005 | CAP-OFF-EVID-006 |
| DisplayId source | Windows Graphics Capture | TryCreateFromDisplayId exists | Item size／frame domain | Display mapping requires host evidence | CAP-OFF-GAP-004 | CAP-OFF-EVID-005 |
| WindowId／HWND source | Windows Graphics Capture／PrintWindow | Window source identity documented separately | Window-local or DC-local domain | Occlusion and composition open | CAP-OFF-GAP-007 | CAP-OFF-EVID-005, CAP-OFF-EVID-008 |
| Negative coordinates | All | Not closed by cited API evidence | Signed virtual coordinates TBD | Topology fixture required | CAP-OFF-GAP-009 | CAP-OFF-EVID-006, CAP-OFF-EVID-007 |
| DPI awareness | GDI／host framework | BitBlt documents logical units | Physical conversion TBD | Per-monitor mapping TBD | CAP-OFF-GAP-006 | CAP-OFF-EVID-007 |
| Frame size | WGC／DXGI | Item／surface dimensions documented | Frame-local pixels | Crop relation TBD | CAP-OFF-GAP-013 | CAP-OFF-EVID-004, CAP-OFF-EVID-006 |
| Crop rectangle | All | No SnipPlus crop contract in API docs | Inclusive／exclusive TBD | Off-by-one runtime need | CAP-OFF-GAP-009, CAP-OFF-GAP-013 | CAP-OFF-EVID-007, CAP-OFF-EVID-009 |
| Rotation | DXGI Desktop Duplication | Official article describes rotation handling | Output orientation TBD | Multiple outputs need fixture | CAP-OFF-GAP-005 | CAP-OFF-EVID-006 |
| Rounding policy | All | No product decision | TBD | Mixed-DPI runtime need | CAP-OFF-GAP-009 | CAP-OFF-EVID-007 |


要求：

- Virtual desktop、per-output capture、negative virtual coordinates、physical pixels、DPI awareness、rotation、frame size、source bounds、crop rectangle、topology changes 均保留。
- 官方 API 回傳尺寸不得自動解讀為 SnipPlus coordinate contract。
- Rounding policy 維持 TBD。
- 文件無法證明的 off-by-one、mixed-DPI 與 topology timing 問題建立 CAP-OFF-GAP。

## 13. Window、Overlay and Cursor Evidence

| Capability | Candidate | Officially controllable | Known limitation | Phase | Runtime verification required | Evidence gap | Evidence |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Self-overlay／overlay | All | No direct evidence that every API excludes product overlay | Capture semantics TBD | C2/C3 | Yes later | CAP-OFF-GAP-010 | CAP-OFF-EVID-004, CAP-OFF-EVID-008 |
| Window exclusion | All | No universal exclusion claim accepted | API-specific | C2/C3 | Yes later | CAP-OFF-GAP-010 | CAP-OFF-EVID-004, CAP-OFF-EVID-008 |
| Cursor | WGC／DXGI | Candidate-specific property/metadata requires source evidence | Frame metadata TBD | C2 | Yes later | CAP-OFF-GAP-007 | CAP-OFF-EVID-004, CAP-OFF-EVID-006 |
| Capture border／indication | WGC | Platform behavior must be directly cited | System indication TBD | C2 | Yes later | CAP-OFF-GAP-010 | CAP-OFF-EVID-004 |
| Occluded window | PrintWindow／WGC | Official limitation scope not fully closed | Window content semantics TBD | C2 | Yes later | CAP-OFF-GAP-007 | CAP-OFF-EVID-005, CAP-OFF-EVID-008 |
| Minimized window | PrintWindow／WGC | Official behavior not accepted as universal | Window content semantics TBD | C2 | Yes later | CAP-OFF-GAP-007 | CAP-OFF-EVID-005, CAP-OFF-EVID-008 |
| Layered／transparent window | GDI／window APIs | CAPTUREBLT documented for layered windows in BitBlt | Composition fidelity TBD | C2 | Yes later | CAP-OFF-GAP-006, CAP-OFF-GAP-007 | CAP-OFF-EVID-007 |
| Resize／close | WGC／DXGI | GraphicsCaptureItem Closed is documented; full recovery not | Resource recreation TBD | C3 | Yes later | CAP-OFF-GAP-012 | CAP-OFF-EVID-005, CAP-OFF-EVID-006 |


至少涵蓋 self-overlay、window exclusion、cursor、capture border／system indication、occluded、minimized、layered／transparent window、resize／close。

不得聲稱官方 API 可以完整解決 Overlay self-capture，除非有直接證據。

## 14. Security、Privacy and Protected-content Evidence

| Boundary | Candidate | Official behavior | Prohibited interpretation | Runtime test allowed | Evidence gap | Evidence |
| --- | --- | --- | --- | --- | --- | --- |
| Protected content | All | No bypass allowed; exact behavior needs platform evidence | Treating blank frame as supported policy | No in this document | CAP-OFF-GAP-011 | CAP-OFF-EVID-004, CAP-OFF-EVID-006 |
| Secure Desktop／UAC | All | No evidence accepted here for capture behavior | Assuming ordinary desktop API applies | No in this document | CAP-OFF-GAP-011 | CAP-OFF-EVID-004, CAP-OFF-EVID-006 |
| User consent／picker | WGC | Picker path is distinct from programmatic source path | Assuming picker consent is universal | No in this document | CAP-OFF-GAP-003, CAP-OFF-GAP-010 | CAP-OFF-EVID-004, CAP-OFF-EVID-005 |
| Access denial | All | Failure must remain observable and lawful | Treating failure as permission to bypass | No in this document | CAP-OFF-GAP-011, CAP-OFF-GAP-012 | CAP-OFF-EVID-004, CAP-OFF-EVID-006 |
| Session isolation | All | No platform-wide claim accepted | Assuming all sessions are capturable | No in this document | CAP-OFF-GAP-011 | CAP-OFF-EVID-001, CAP-OFF-EVID-004 |
| Black／blank frame | All | Requires candidate-specific runtime evidence | Treating blank output as successful capture | No in this document | CAP-OFF-GAP-011, CAP-OFF-GAP-013 | CAP-OFF-EVID-004, CAP-OFF-EVID-006 |
| Private desktop content | All | No private frame may be saved in this research | Using real desktop as test fixture | No in this document | CAP-OFF-GAP-015 | CAP-OFF-EVID-012 |


規則：

- 不得規避平台限制。
- Runtime Evidence 只能使用 synthetic scene。
- 不得保存私人桌面內容。
- Protected content、Secure Desktop、UAC、user consent、access denial 與 blank frame behavior 不由本文件作產品決策。

## 15. Failure and Recovery Official Evidence

| Candidate | Failure condition | Official error／status | Required resource recreation | Session impact | Runtime evidence required | Evidence |
| --- | --- | --- | --- | --- | --- | --- |
| Windows Graphics Capture | Item Closed／session or frame-pool failure | Official Closed event; other sequence TBD | Recreate only after separate authorization | Unknown | Future synthetic runtime | CAP-OFF-EVID-005, CAP-OFF-GAP-012 |
| DXGI Desktop Duplication | Access lost／output change | Official article and API error handling require further record | Recreate duplication／device only in future experiment | Unknown | Future synthetic runtime | CAP-OFF-EVID-006, CAP-OFF-GAP-012 |
| GDI-based capture | BitBlt／GetDIBits failure | Boolean／return code documented | Release DC／bitmap according to future implementation | Unknown | Future synthetic runtime | CAP-OFF-EVID-007, CAP-OFF-EVID-009 |
| Window-oriented mechanisms | PrintWindow failure or owner non-response | Zero return and synchronous behavior documented | Stop and release HDC; no retry policy here | Unknown | Future synthetic runtime | CAP-OFF-EVID-008, CAP-OFF-GAP-007 |
| All | Display topology change | Product sequence not documented | Separate recovery design required | Unknown | Future synthetic runtime | CAP-OFF-GAP-012 |
| All | Source close／resize | WGC Closed is one documented signal; others TBD | Recreate scope TBD | Unknown | Future synthetic runtime | CAP-OFF-EVID-005, CAP-OFF-GAP-012 |
| All | Null／blank／stale frame | No success inference allowed | Stop and preserve diagnostic context later | Unknown | Future synthetic runtime | CAP-OFF-GAP-011, CAP-OFF-GAP-013 |
| All | Cleanup／Dispose | Resource lifecycle must be specified per candidate | Manifest and rollback owner TBD | Unknown | Future synthetic runtime | CAP-OFF-GAP-014, CAP-OFF-GAP-015 |


官方文件未說明完整 Recovery sequence 時，不得自行宣告已解決。Failure evidence 只定義後續要查證的邊界，不執行 recovery。

## 16. Official Evidence Gap Register

建立連續 Gap ID；本文件建立 CAP-OFF-GAP-001..015。

| Gap ID | Candidate | Host | Related Pair | Related CAP-ENABLE | Status |
|---|---|---|---|---|---|
| CAP-OFF-GAP-001 | All | All | CAP-PAIR-001..010 | CAP-ENABLE-001, CAP-ENABLE-003 | Open |
| CAP-OFF-GAP-002 | All | WinUI 3／WPF | CAP-PAIR-001..010 | CAP-ENABLE-002, CAP-ENABLE-003 | Open |
| CAP-OFF-GAP-003 | Windows Graphics Capture | WPF | CAP-PAIR-002 | CAP-ENABLE-001, CAP-ENABLE-002, CAP-ENABLE-003 | Open |
| CAP-OFF-GAP-004 | Windows Graphics Capture | WinUI 3／WPF | CAP-PAIR-001, CAP-PAIR-002 | CAP-ENABLE-004, CAP-ENABLE-005 | Open |
| CAP-OFF-GAP-005 | DXGI Desktop Duplication | WinUI 3／WPF | CAP-PAIR-003, CAP-PAIR-004 | CAP-ENABLE-002, CAP-ENABLE-005 | Open |
| CAP-OFF-GAP-006 | GDI-based capture | WinUI 3／WPF | CAP-PAIR-005, CAP-PAIR-006 | CAP-ENABLE-005 | Open |
| CAP-OFF-GAP-007 | Window-oriented mechanisms | WinUI 3／WPF | CAP-PAIR-007, CAP-PAIR-008 | CAP-ENABLE-004, CAP-ENABLE-007 | Open |
| CAP-OFF-GAP-008 | Hybrid strategy | WinUI 3／WPF | CAP-PAIR-009, CAP-PAIR-010 | CAP-ENABLE-002, CAP-ENABLE-005 | Open |
| CAP-OFF-GAP-009 | All | WinUI 3／WPF | CAP-PAIR-001..010 | CAP-ENABLE-005 | Open |
| CAP-OFF-GAP-010 | All | WinUI 3／WPF | CAP-PAIR-001..010 | CAP-ENABLE-004, CAP-ENABLE-007 | Open |
| CAP-OFF-GAP-011 | All | WinUI 3／WPF | CAP-PAIR-001..010 | CAP-ENABLE-004, CAP-ENABLE-006 | Open |
| CAP-OFF-GAP-012 | DXGI Desktop Duplication; Windows Graphics Capture | WinUI 3／WPF | CAP-PAIR-001..004 | CAP-ENABLE-006, CAP-ENABLE-007 | Open |
| CAP-OFF-GAP-013 | Windows Graphics Capture; DXGI Desktop Duplication | WinUI 3／WPF | CAP-PAIR-001..004 | CAP-ENABLE-005 | Open |
| CAP-OFF-GAP-014 | All | WinUI 3／WPF | CAP-PAIR-001..010 | CAP-ENABLE-003, CAP-ENABLE-006 | Open |
| CAP-OFF-GAP-015 | All | WinUI 3／WPF | CAP-PAIR-001..010 | CAP-ENABLE-006 | Open |

每個 Gap 的完整固定欄位如下：

### CAP-OFF-GAP-001

| Field | Value |
| --- | --- |
| Gap ID | CAP-OFF-GAP-001 |
| Missing claim | Local Windows／SDK／runtime availability is unknown for every Candidate–Host Pair |
| Candidate | All |
| Host | All |
| Related Pair | CAP-PAIR-001..010 |
| Related prerequisite | CAP-PREQ-001, CAP-PREQ-027 |
| Related blocker | CAP-BLOCK-001, CAP-BLOCK-010 |
| Related enablement item | CAP-ENABLE-001, CAP-ENABLE-003 |
| Related closure gate | CAP-CGATE-001, CAP-CGATE-008 |
| Official sources checked | No local inspection allowed |
| Why evidence is insufficient | 官方資料只支援靜態 claim，無法單獨證明本機 availability、Host integration、Build、Runtime 或產品 contract。 |
| Required next evidence | 版本化 source citation、isolated project record、read-only environment record 或依法授權的 synthetic runtime evidence。 |
| Local inspection required | Yes；本文件未執行。 |
| Package acquisition required | As applicable；本文件未執行。 |
| Build required | Yes for future host／candidate prototype；本文件未執行。 |
| Runtime required | Yes when the gap concerns frame、coordinate、failure or fidelity；本文件未執行。 |
| Blocks Phase C1 | Yes or remains a Phase C1 prerequisite gap |
| Status | Open |

### CAP-OFF-GAP-002

| Field | Value |
| --- | --- |
| Gap ID | CAP-OFF-GAP-002 |
| Missing claim | Exact experimental SDK／Package／target framework identity is not fixed |
| Candidate | All |
| Host | WinUI 3／WPF |
| Related Pair | CAP-PAIR-001..010 |
| Related prerequisite | CAP-PREQ-003..012, CAP-PREQ-027 |
| Related blocker | CAP-BLOCK-002, CAP-BLOCK-010 |
| Related enablement item | CAP-ENABLE-002, CAP-ENABLE-003 |
| Related closure gate | CAP-CGATE-002, CAP-CGATE-003 |
| Official sources checked | Official docs distinguish API families but do not choose repository package version |
| Why evidence is insufficient | 官方資料只支援靜態 claim，無法單獨證明本機 availability、Host integration、Build、Runtime 或產品 contract。 |
| Required next evidence | 版本化 source citation、isolated project record、read-only environment record 或依法授權的 synthetic runtime evidence。 |
| Local inspection required | Yes；本文件未執行。 |
| Package acquisition required | As applicable；本文件未執行。 |
| Build required | Yes for future host／candidate prototype；本文件未執行。 |
| Runtime required | Yes when the gap concerns frame、coordinate、failure or fidelity；本文件未執行。 |
| Blocks Phase C1 | Yes or remains a Phase C1 prerequisite gap |
| Status | Open |

### CAP-OFF-GAP-003

| Field | Value |
| --- | --- |
| Gap ID | CAP-OFF-GAP-003 |
| Missing claim | WPF-specific WGC activation and frame-pool integration remains unverified |
| Candidate | Windows Graphics Capture |
| Host | WPF |
| Related Pair | CAP-PAIR-002 |
| Related prerequisite | CAP-PREQ-003, CAP-PREQ-006, CAP-PREQ-027 |
| Related blocker | CAP-BLOCK-001, CAP-BLOCK-002, CAP-BLOCK-010 |
| Related enablement item | CAP-ENABLE-001, CAP-ENABLE-002, CAP-ENABLE-003 |
| Related closure gate | CAP-CGATE-001..003 |
| Official sources checked | Requires isolated host prototype later |
| Why evidence is insufficient | 官方資料只支援靜態 claim，無法單獨證明本機 availability、Host integration、Build、Runtime 或產品 contract。 |
| Required next evidence | 版本化 source citation、isolated project record、read-only environment record 或依法授權的 synthetic runtime evidence。 |
| Local inspection required | Yes；本文件未執行。 |
| Package acquisition required | As applicable；本文件未執行。 |
| Build required | Yes for future host／candidate prototype；本文件未執行。 |
| Runtime required | Yes when the gap concerns frame、coordinate、failure or fidelity；本文件未執行。 |
| Blocks Phase C1 | Yes or remains a Phase C1 prerequisite gap |
| Status | Open |

### CAP-OFF-GAP-004

| Field | Value |
| --- | --- |
| Gap ID | CAP-OFF-GAP-004 |
| Missing claim | WGC HD color／alpha negotiation and exact pixel metadata are not closed |
| Candidate | Windows Graphics Capture |
| Host | WinUI 3／WPF |
| Related Pair | CAP-PAIR-001, CAP-PAIR-002 |
| Related prerequisite | CAP-PREQ-013, CAP-PREQ-016, CAP-PREQ-022 |
| Related blocker | CAP-BLOCK-003, CAP-BLOCK-007 |
| Related enablement item | CAP-ENABLE-004, CAP-ENABLE-005 |
| Related closure gate | CAP-CGATE-004, CAP-CGATE-007 |
| Official sources checked | Official note confirms variability, not product policy |
| Why evidence is insufficient | 官方資料只支援靜態 claim，無法單獨證明本機 availability、Host integration、Build、Runtime 或產品 contract。 |
| Required next evidence | 版本化 source citation、isolated project record、read-only environment record 或依法授權的 synthetic runtime evidence。 |
| Local inspection required | Yes；本文件未執行。 |
| Package acquisition required | As applicable；本文件未執行。 |
| Build required | Yes for future host／candidate prototype；本文件未執行。 |
| Runtime required | Yes when the gap concerns frame、coordinate、failure or fidelity；本文件未執行。 |
| Blocks Phase C1 | Yes or remains a Phase C1 prerequisite gap |
| Status | Open |

### CAP-OFF-GAP-005

| Field | Value |
| --- | --- |
| Gap ID | CAP-OFF-GAP-005 |
| Missing claim | DXGI output-per-monitor assembly and virtual desktop composition are not a product contract |
| Candidate | DXGI Desktop Duplication |
| Host | WinUI 3／WPF |
| Related Pair | CAP-PAIR-003, CAP-PAIR-004 |
| Related prerequisite | CAP-PREQ-014, CAP-PREQ-015, CAP-PREQ-022 |
| Related blocker | CAP-BLOCK-004, CAP-BLOCK-007 |
| Related enablement item | CAP-ENABLE-002, CAP-ENABLE-005 |
| Related closure gate | CAP-CGATE-005..007 |
| Official sources checked | Requires multi-output runtime evidence later |
| Why evidence is insufficient | 官方資料只支援靜態 claim，無法單獨證明本機 availability、Host integration、Build、Runtime 或產品 contract。 |
| Required next evidence | 版本化 source citation、isolated project record、read-only environment record 或依法授權的 synthetic runtime evidence。 |
| Local inspection required | Yes；本文件未執行。 |
| Package acquisition required | As applicable；本文件未執行。 |
| Build required | Yes for future host／candidate prototype；本文件未執行。 |
| Runtime required | Yes when the gap concerns frame、coordinate、failure or fidelity；本文件未執行。 |
| Blocks Phase C1 | Yes or remains a Phase C1 prerequisite gap |
| Status | Open |

### CAP-OFF-GAP-006

| Field | Value |
| --- | --- |
| Gap ID | CAP-OFF-GAP-006 |
| Missing claim | GDI logical units, DPI awareness and physical-pixel conversion are not resolved for SnipPlus |
| Candidate | GDI-based capture |
| Host | WinUI 3／WPF |
| Related Pair | CAP-PAIR-005, CAP-PAIR-006 |
| Related prerequisite | CAP-PREQ-014..016, CAP-PREQ-023 |
| Related blocker | CAP-BLOCK-004, CAP-BLOCK-007 |
| Related enablement item | CAP-ENABLE-005 |
| Related closure gate | CAP-CGATE-005, CAP-CGATE-006 |
| Official sources checked | Rounding remains TBD |
| Why evidence is insufficient | 官方資料只支援靜態 claim，無法單獨證明本機 availability、Host integration、Build、Runtime 或產品 contract。 |
| Required next evidence | 版本化 source citation、isolated project record、read-only environment record 或依法授權的 synthetic runtime evidence。 |
| Local inspection required | Yes；本文件未執行。 |
| Package acquisition required | As applicable；本文件未執行。 |
| Build required | Yes for future host／candidate prototype；本文件未執行。 |
| Runtime required | Yes when the gap concerns frame、coordinate、failure or fidelity；本文件未執行。 |
| Blocks Phase C1 | Yes or remains a Phase C1 prerequisite gap |
| Status | Open |

### CAP-OFF-GAP-007

| Field | Value |
| --- | --- |
| Gap ID | CAP-OFF-GAP-007 |
| Missing claim | Window-oriented capture behavior for occluded, minimized, layered or composition content is not closed |
| Candidate | Window-oriented mechanisms |
| Host | WinUI 3／WPF |
| Related Pair | CAP-PAIR-007, CAP-PAIR-008 |
| Related prerequisite | CAP-PREQ-017..019, CAP-PREQ-028 |
| Related blocker | CAP-BLOCK-005, CAP-BLOCK-006, CAP-BLOCK-011 |
| Related enablement item | CAP-ENABLE-004, CAP-ENABLE-007 |
| Related closure gate | CAP-CGATE-004, CAP-CGATE-010 |
| Official sources checked | Requires lawful synthetic runtime scenarios later |
| Why evidence is insufficient | 官方資料只支援靜態 claim，無法單獨證明本機 availability、Host integration、Build、Runtime 或產品 contract。 |
| Required next evidence | 版本化 source citation、isolated project record、read-only environment record 或依法授權的 synthetic runtime evidence。 |
| Local inspection required | Yes；本文件未執行。 |
| Package acquisition required | As applicable；本文件未執行。 |
| Build required | Yes for future host／candidate prototype；本文件未執行。 |
| Runtime required | Yes when the gap concerns frame、coordinate、failure or fidelity；本文件未執行。 |
| Blocks Phase C1 | Yes or remains a Phase C1 prerequisite gap |
| Status | Open |

### CAP-OFF-GAP-008

| Field | Value |
| --- | --- |
| Gap ID | CAP-OFF-GAP-008 |
| Missing claim | Hybrid primary／fallback shared frame and coordinate semantics are not an official API claim |
| Candidate | Hybrid strategy |
| Host | WinUI 3／WPF |
| Related Pair | CAP-PAIR-009, CAP-PAIR-010 |
| Related prerequisite | CAP-PREQ-003, CAP-PREQ-022, CAP-PREQ-023 |
| Related blocker | CAP-BLOCK-002, CAP-BLOCK-007 |
| Related enablement item | CAP-ENABLE-002, CAP-ENABLE-005 |
| Related closure gate | CAP-CGATE-002, CAP-CGATE-006 |
| Official sources checked | Strategy remains TBD until constituent identities are fixed |
| Why evidence is insufficient | 官方資料只支援靜態 claim，無法單獨證明本機 availability、Host integration、Build、Runtime 或產品 contract。 |
| Required next evidence | 版本化 source citation、isolated project record、read-only environment record 或依法授權的 synthetic runtime evidence。 |
| Local inspection required | Yes；本文件未執行。 |
| Package acquisition required | As applicable；本文件未執行。 |
| Build required | Yes for future host／candidate prototype；本文件未執行。 |
| Runtime required | Yes when the gap concerns frame、coordinate、failure or fidelity；本文件未執行。 |
| Blocks Phase C1 | Yes or remains a Phase C1 prerequisite gap |
| Status | Open |

### CAP-OFF-GAP-009

| Field | Value |
| --- | --- |
| Gap ID | CAP-OFF-GAP-009 |
| Missing claim | Negative coordinates, inclusive／exclusive edges, rounding and off-by-one behavior are not closed |
| Candidate | All |
| Host | WinUI 3／WPF |
| Related Pair | CAP-PAIR-001..010 |
| Related prerequisite | CAP-PREQ-014..016, CAP-PREQ-023 |
| Related blocker | CAP-BLOCK-004, CAP-BLOCK-007 |
| Related enablement item | CAP-ENABLE-005 |
| Related closure gate | CAP-CGATE-005, CAP-CGATE-006 |
| Official sources checked | Official API bounds do not form product coordinate contract |
| Why evidence is insufficient | 官方資料只支援靜態 claim，無法單獨證明本機 availability、Host integration、Build、Runtime 或產品 contract。 |
| Required next evidence | 版本化 source citation、isolated project record、read-only environment record 或依法授權的 synthetic runtime evidence。 |
| Local inspection required | Yes；本文件未執行。 |
| Package acquisition required | As applicable；本文件未執行。 |
| Build required | Yes for future host／candidate prototype；本文件未執行。 |
| Runtime required | Yes when the gap concerns frame、coordinate、failure or fidelity；本文件未執行。 |
| Blocks Phase C1 | Yes or remains a Phase C1 prerequisite gap |
| Status | Open |

### CAP-OFF-GAP-010

| Field | Value |
| --- | --- |
| Gap ID | CAP-OFF-GAP-010 |
| Missing claim | Overlay self-capture, exclusion and system indication behavior is not fully evidenced |
| Candidate | All |
| Host | WinUI 3／WPF |
| Related Pair | CAP-PAIR-001..010 |
| Related prerequisite | CAP-PREQ-018, CAP-PREQ-019, CAP-PREQ-028 |
| Related blocker | CAP-BLOCK-005, CAP-BLOCK-006, CAP-BLOCK-011 |
| Related enablement item | CAP-ENABLE-004, CAP-ENABLE-007 |
| Related closure gate | CAP-CGATE-004, CAP-CGATE-010 |
| Official sources checked | Do not infer unsupported exclusion capability |
| Why evidence is insufficient | 官方資料只支援靜態 claim，無法單獨證明本機 availability、Host integration、Build、Runtime 或產品 contract。 |
| Required next evidence | 版本化 source citation、isolated project record、read-only environment record 或依法授權的 synthetic runtime evidence。 |
| Local inspection required | Yes；本文件未執行。 |
| Package acquisition required | As applicable；本文件未執行。 |
| Build required | Yes for future host／candidate prototype；本文件未執行。 |
| Runtime required | Yes when the gap concerns frame、coordinate、failure or fidelity；本文件未執行。 |
| Blocks Phase C1 | Yes or remains a Phase C1 prerequisite gap |
| Status | Open |

### CAP-OFF-GAP-011

| Field | Value |
| --- | --- |
| Gap ID | CAP-OFF-GAP-011 |
| Missing claim | Protected content, Secure Desktop, UAC and denied-access behavior require explicit platform evidence and lawful runtime tests |
| Candidate | All |
| Host | WinUI 3／WPF |
| Related Pair | CAP-PAIR-001..010 |
| Related prerequisite | CAP-PREQ-020, CAP-PREQ-025 |
| Related blocker | CAP-BLOCK-008, CAP-BLOCK-009 |
| Related enablement item | CAP-ENABLE-004, CAP-ENABLE-006 |
| Related closure gate | CAP-CGATE-007, CAP-CGATE-009 |
| Official sources checked | No bypass or real protected content |
| Why evidence is insufficient | 官方資料只支援靜態 claim，無法單獨證明本機 availability、Host integration、Build、Runtime 或產品 contract。 |
| Required next evidence | 版本化 source citation、isolated project record、read-only environment record 或依法授權的 synthetic runtime evidence。 |
| Local inspection required | Yes；本文件未執行。 |
| Package acquisition required | As applicable；本文件未執行。 |
| Build required | Yes for future host／candidate prototype；本文件未執行。 |
| Runtime required | Yes when the gap concerns frame、coordinate、failure or fidelity；本文件未執行。 |
| Blocks Phase C1 | Yes or remains a Phase C1 prerequisite gap |
| Status | Open |

### CAP-OFF-GAP-012

| Field | Value |
| --- | --- |
| Gap ID | CAP-OFF-GAP-012 |
| Missing claim | Device-loss, topology-change, access-lost and resource recreation sequence is not fully documented for one product flow |
| Candidate | DXGI Desktop Duplication; Windows Graphics Capture |
| Host | WinUI 3／WPF |
| Related Pair | CAP-PAIR-001..004 |
| Related prerequisite | CAP-PREQ-021, CAP-PREQ-028 |
| Related blocker | CAP-BLOCK-011, CAP-BLOCK-012 |
| Related enablement item | CAP-ENABLE-006, CAP-ENABLE-007 |
| Related closure gate | CAP-CGATE-008, CAP-CGATE-010 |
| Official sources checked | Requires later runtime recovery evidence |
| Why evidence is insufficient | 官方資料只支援靜態 claim，無法單獨證明本機 availability、Host integration、Build、Runtime 或產品 contract。 |
| Required next evidence | 版本化 source citation、isolated project record、read-only environment record 或依法授權的 synthetic runtime evidence。 |
| Local inspection required | Yes；本文件未執行。 |
| Package acquisition required | As applicable；本文件未執行。 |
| Build required | Yes for future host／candidate prototype；本文件未執行。 |
| Runtime required | Yes when the gap concerns frame、coordinate、failure or fidelity；本文件未執行。 |
| Blocks Phase C1 | Yes or remains a Phase C1 prerequisite gap |
| Status | Open |

### CAP-OFF-GAP-013

| Field | Value |
| --- | --- |
| Gap ID | CAP-OFF-GAP-013 |
| Missing claim | GPU texture to CPU readback, crop and pixel-difference method remains unverified |
| Candidate | Windows Graphics Capture; DXGI Desktop Duplication |
| Host | WinUI 3／WPF |
| Related Pair | CAP-PAIR-001..004 |
| Related prerequisite | CAP-PREQ-022, CAP-PREQ-023 |
| Related blocker | CAP-BLOCK-007 |
| Related enablement item | CAP-ENABLE-005 |
| Related closure gate | CAP-CGATE-006, CAP-CGATE-007 |
| Official sources checked | No threshold or fidelity assertion |
| Why evidence is insufficient | 官方資料只支援靜態 claim，無法單獨證明本機 availability、Host integration、Build、Runtime 或產品 contract。 |
| Required next evidence | 版本化 source citation、isolated project record、read-only environment record 或依法授權的 synthetic runtime evidence。 |
| Local inspection required | Yes；本文件未執行。 |
| Package acquisition required | As applicable；本文件未執行。 |
| Build required | Yes for future host／candidate prototype；本文件未執行。 |
| Runtime required | Yes when the gap concerns frame、coordinate、failure or fidelity；本文件未執行。 |
| Blocks Phase C1 | Yes or remains a Phase C1 prerequisite gap |
| Status | Open |

### CAP-OFF-GAP-014

| Field | Value |
| --- | --- |
| Gap ID | CAP-OFF-GAP-014 |
| Missing claim | Official API documentation does not prove Package Restore, Build, output or cleanup behavior in this repository |
| Candidate | All |
| Host | WinUI 3／WPF |
| Related Pair | CAP-PAIR-001..010 |
| Related prerequisite | CAP-PREQ-027, CAP-PREQ-030 |
| Related blocker | CAP-BLOCK-010 |
| Related enablement item | CAP-ENABLE-003, CAP-ENABLE-006 |
| Related closure gate | CAP-CGATE-008, CAP-CGATE-009 |
| Official sources checked | Local execution remains prohibited |
| Why evidence is insufficient | 官方資料只支援靜態 claim，無法單獨證明本機 availability、Host integration、Build、Runtime 或產品 contract。 |
| Required next evidence | 版本化 source citation、isolated project record、read-only environment record 或依法授權的 synthetic runtime evidence。 |
| Local inspection required | Yes；本文件未執行。 |
| Package acquisition required | As applicable；本文件未執行。 |
| Build required | Yes for future host／candidate prototype；本文件未執行。 |
| Runtime required | Yes when the gap concerns frame、coordinate、failure or fidelity；本文件未執行。 |
| Blocks Phase C1 | Yes or remains a Phase C1 prerequisite gap |
| Status | Open |

### CAP-OFF-GAP-015

| Field | Value |
| --- | --- |
| Gap ID | CAP-OFF-GAP-015 |
| Missing claim | Privacy, retention, evidence root and cleanup policy need human ownership beyond API documentation |
| Candidate | All |
| Host | WinUI 3／WPF |
| Related Pair | CAP-PAIR-001..010 |
| Related prerequisite | CAP-PREQ-024..026, CAP-PREQ-029..030 |
| Related blocker | CAP-BLOCK-008, CAP-BLOCK-009 |
| Related enablement item | CAP-ENABLE-006 |
| Related closure gate | CAP-CGATE-007, CAP-CGATE-009 |
| Official sources checked | No private desktop content may be persisted |
| Why evidence is insufficient | 官方資料只支援靜態 claim，無法單獨證明本機 availability、Host integration、Build、Runtime 或產品 contract。 |
| Required next evidence | 版本化 source citation、isolated project record、read-only environment record 或依法授權的 synthetic runtime evidence。 |
| Local inspection required | Yes；本文件未執行。 |
| Package acquisition required | As applicable；本文件未執行。 |
| Build required | Yes for future host／candidate prototype；本文件未執行。 |
| Runtime required | Yes when the gap concerns frame、coordinate、failure or fidelity；本文件未執行。 |
| Blocks Phase C1 | Yes or remains a Phase C1 prerequisite gap |
| Status | Open |


Gap Status 只使用 Open 或 Accepted documentation limitation。不得將搜尋不到官方文件解讀為不支援。

## 17. Enablement Evidence Mapping

建立正好七列：

| Enablement Item | Required official claims | Evidence IDs | Gap IDs | Specification improvement | Remaining gap | Status recommendation |
| --- | --- | --- | --- | --- | --- | --- |
| CAP-ENABLE-001 | Host／framework／SDK baseline | CAP-OFF-EVID-001..003, CAP-OFF-EVID-010 | CAP-OFF-GAP-001..003 | Official evidence separates framework support from local availability | Host identity and shared UI authority remain unresolved | Partially specified |
| CAP-ENABLE-002 | Candidate API／SDK／Interop identity | CAP-OFF-EVID-004..010 | CAP-OFF-GAP-002, CAP-OFF-GAP-003, CAP-OFF-GAP-008 | Names API families and native boundaries | Exact version and host path require later closure | Partially specified |
| CAP-ENABLE-003 | Isolated Project／Package／Restore／Build boundary | CAP-OFF-EVID-001, CAP-OFF-EVID-012 | CAP-OFF-GAP-001, CAP-OFF-GAP-002, CAP-OFF-GAP-014 | Clarifies documentation cannot prove local build | No project or package evidence allowed in this task | Blocked |
| CAP-ENABLE-004 | Synthetic Scene and source behavior | CAP-OFF-EVID-004, CAP-OFF-EVID-005, CAP-OFF-EVID-008, CAP-OFF-EVID-011 | CAP-OFF-GAP-004, CAP-OFF-GAP-007, CAP-OFF-GAP-010, CAP-OFF-GAP-011 | Separates documented API claims from synthetic runtime scenarios | Scene and overlay behavior remain runtime gaps | Partially specified |
| CAP-ENABLE-005 | Coordinate／Crop／Frame method | CAP-OFF-EVID-004, CAP-OFF-EVID-006, CAP-OFF-EVID-007, CAP-OFF-EVID-009, CAP-OFF-EVID-011 | CAP-OFF-GAP-004..006, CAP-OFF-GAP-009, CAP-OFF-GAP-013 | Records official bounds and pixel caveats | Rounding, crop and fidelity are not closed | Partially specified |
| CAP-ENABLE-006 | Privacy／Evidence／Cleanup boundary | CAP-OFF-EVID-004, CAP-OFF-EVID-006, CAP-OFF-EVID-008, CAP-OFF-EVID-012 | CAP-OFF-GAP-011, CAP-OFF-GAP-012, CAP-OFF-GAP-014, CAP-OFF-GAP-015 | Defines evidence governance questions | No evidence write or privacy acceptance performed | Partially specified |
| CAP-ENABLE-007 | Runtime authority and stop boundary | CAP-OFF-EVID-005, CAP-OFF-EVID-006, CAP-OFF-EVID-008 | CAP-OFF-GAP-007, CAP-OFF-GAP-010, CAP-OFF-GAP-012 | Keeps runtime outside official static baseline | Independent runtime authorization remains required | Blocked |


Status recommendation 只能使用 Specified、Partially specified、Blocked、Deferred、Not applicable。這只是新建議，不修改 RESEARCH-TECH-CAPTURE-005。

## 18. Candidate–Host Pair Evidence Mapping

建立正好十列：

| Pair | Accepted official evidence | Unresolved official gap | Local evidence required | Build required | Runtime required | Pair recommendation |
| --- | --- | --- | --- | --- | --- | --- |
| CAP-PAIR-001 | CAP-OFF-EVID-004, CAP-OFF-EVID-005 | CAP-OFF-GAP-001, CAP-OFF-GAP-002, CAP-OFF-GAP-004 | Yes; not performed | Yes; not performed | Yes when frame／coordinate／failure semantics are in scope | CAP-ENABLE-001, CAP-ENABLE-002 |
| CAP-PAIR-002 | CAP-OFF-EVID-004, CAP-OFF-EVID-005 | CAP-OFF-GAP-001, CAP-OFF-GAP-003, CAP-OFF-GAP-004 | Yes; not performed | Yes; not performed | Yes when frame／coordinate／failure semantics are in scope | CAP-ENABLE-001, CAP-ENABLE-002 |
| CAP-PAIR-003 | CAP-OFF-EVID-006 | CAP-OFF-GAP-001, CAP-OFF-GAP-005, CAP-OFF-GAP-012 | Yes; not performed | Yes; not performed | Yes when frame／coordinate／failure semantics are in scope | CAP-ENABLE-002, CAP-ENABLE-005 |
| CAP-PAIR-004 | CAP-OFF-EVID-006 | CAP-OFF-GAP-001, CAP-OFF-GAP-005, CAP-OFF-GAP-012 | Yes; not performed | Yes; not performed | Yes when frame／coordinate／failure semantics are in scope | CAP-ENABLE-002, CAP-ENABLE-005 |
| CAP-PAIR-005 | CAP-OFF-EVID-007, CAP-OFF-EVID-009 | CAP-OFF-GAP-001, CAP-OFF-GAP-006, CAP-OFF-GAP-013 | Yes; not performed | Yes; not performed | Yes when frame／coordinate／failure semantics are in scope | CAP-ENABLE-002, CAP-ENABLE-005 |
| CAP-PAIR-006 | CAP-OFF-EVID-007, CAP-OFF-EVID-009 | CAP-OFF-GAP-001, CAP-OFF-GAP-006, CAP-OFF-GAP-013 | Yes; not performed | Yes; not performed | Yes when frame／coordinate／failure semantics are in scope | CAP-ENABLE-002, CAP-ENABLE-005 |
| CAP-PAIR-007 | CAP-OFF-EVID-008 | CAP-OFF-GAP-001, CAP-OFF-GAP-007, CAP-OFF-GAP-010 | Yes; not performed | Yes; not performed | Yes when frame／coordinate／failure semantics are in scope | CAP-ENABLE-002, CAP-ENABLE-004, CAP-ENABLE-007 |
| CAP-PAIR-008 | CAP-OFF-EVID-008 | CAP-OFF-GAP-001, CAP-OFF-GAP-007, CAP-OFF-GAP-010 | Yes; not performed | Yes; not performed | Yes when frame／coordinate／failure semantics are in scope | CAP-ENABLE-002, CAP-ENABLE-004, CAP-ENABLE-007 |
| CAP-PAIR-009 | CAP-OFF-EVID-004, CAP-OFF-EVID-006, CAP-OFF-EVID-007, CAP-OFF-EVID-008 | CAP-OFF-GAP-001, CAP-OFF-GAP-008, CAP-OFF-GAP-009 | Yes; not performed | Yes; not performed | Yes when frame／coordinate／failure semantics are in scope | CAP-ENABLE-002, CAP-ENABLE-005 |
| CAP-PAIR-010 | CAP-OFF-EVID-004, CAP-OFF-EVID-006, CAP-OFF-EVID-007, CAP-OFF-EVID-008 | CAP-OFF-GAP-001, CAP-OFF-GAP-003, CAP-OFF-GAP-008 | Yes; not performed | Yes; not performed | Yes when frame／coordinate／failure semantics are in scope | CAP-ENABLE-002, CAP-ENABLE-005 |


要求：

- 十個 Pair 全部覆蓋。
- 官方 evidence 完整不代表 Build／Runtime 通過。
- 未來 Runtime required 不得因官方 Sample 存在而改為 No。
- Pair recommendation 不得形成 Candidate ranking。

## 19. Phase C1 Closure Gate Evidence Mapping

覆蓋 CAP-CGATE-001..010：

| Closure Gate | Official evidence contribution | Remaining documentary requirement | Remaining non-documentary requirement | Evidence sufficiency |
| --- | --- | --- | --- | --- |
| CAP-CGATE-001 | Shared WinUI 3／WPF Host dependencies | CAP-OFF-EVID-001..003, CAP-OFF-EVID-010 | Exact local framework／SDK identity and shared authority | Project and local inspection | Partially sufficient |
| CAP-CGATE-002 | One-shot Candidate exact API／SDK identity | CAP-OFF-EVID-004..009 | Exact version/package and selected candidate not fixed | Package／project decision | Sufficient for static specification |
| CAP-CGATE-003 | Candidate–Host Project／Interop boundary | CAP-OFF-EVID-002, CAP-OFF-EVID-003, CAP-OFF-EVID-005, CAP-OFF-EVID-008 | Host-specific activation and native interop remain open | Isolated host prototype | Partially sufficient |
| CAP-CGATE-004 | Basic synthetic scene contract | CAP-OFF-EVID-004, CAP-OFF-EVID-005, CAP-OFF-EVID-008, CAP-OFF-EVID-011 | Scene acceptance and color／overlay behavior not verified | Synthetic runtime scene | Partially sufficient |
| CAP-CGATE-005 | Virtual desktop／monitor／negative-coordinate model | CAP-OFF-EVID-006, CAP-OFF-EVID-007, CAP-OFF-EVID-009 | Product coordinate contract not in API docs | Multi-monitor inspection | Partially sufficient |
| CAP-CGATE-006 | Crop and off-by-one method | CAP-OFF-EVID-007, CAP-OFF-EVID-009, CAP-OFF-EVID-011 | Rounding and fidelity thresholds not decided | Runtime crop evidence | Partially sufficient |
| CAP-CGATE-007 | Frame／metadata／privacy evidence obligation | CAP-OFF-EVID-004, CAP-OFF-EVID-006, CAP-OFF-EVID-011, CAP-OFF-EVID-012 | Evidence schema and privacy owner absent | Evidence write review | Partially sufficient |
| CAP-CGATE-008 | Project／Restore／Build／Runtime／Evidence authority separation | CAP-OFF-EVID-001, CAP-OFF-EVID-012 | Official docs cannot prove repository operations | Independent authorizations | Insufficient |
| CAP-CGATE-009 | Result storage and cleanup boundary | CAP-OFF-EVID-012 | No product retention or cleanup decision | Governance review | Insufficient |
| CAP-CGATE-010 | Runtime execution remains independent authorization | CAP-OFF-EVID-004..009 | Runtime behavior unverified | Runtime authorization and spike | Sufficient for static specification |


Evidence sufficiency 只能使用 Sufficient for static specification、Partially sufficient、Insufficient 或 Not applicable。

明確規定：

- 官方文件不能取代 Local availability。
- API Reference 不能取代 Project creation。
- Sample 不能取代 Restore／Build evidence。
- Host interoperability 文件不能取代 Runtime frame acquisition。
- Pixel-format 文件不能取代 Crop fidelity。
- Security 文件不能取代實際 privacy review。
- Failure code 文件不能取代 Recovery observation。

## 20. Official Evidence Baseline Status

本文件目前狀態：

- Official prerequisite evidence baseline status: Official prerequisite evidence baseline partially complete
- Reassessment sufficiency: Partially sufficient for reassessment
- Local Environment Inspection: Not performed
- Package Cache Inspection: Not performed
- Build Verification: Not performed
- Runtime Verification: Not performed
- Closure Execution Authorized: No
- Capture Runtime Spike Authorized: No
- Evidence Write Authorized: No
- Capture Decision: Not made
- Rendering Decision: Not made

不能因文件完成而自動標記為 complete 或 sufficient。官方資料已關閉部分 static identity claims，但不會關閉 local、build、runtime、privacy、crop fidelity 或 product decision gaps。

### 20.1 Explicit prerequisite coverage

逐一展開 CAP-PREQ-001..030，避免用範圍文字取代可查核的 coverage：

| Prerequisite | Related Enablement | Related Closure Gate | Official evidence state | Remaining gap |
|---|---|---|---|---|
| CAP-PREQ-001 | CAP-ENABLE-001 | CAP-CGATE-001 | Partially confirmed | CAP-OFF-GAP-001, CAP-OFF-GAP-002 |
| CAP-PREQ-002 | CAP-ENABLE-001 | CAP-CGATE-001 | Partially confirmed | CAP-OFF-GAP-001, CAP-OFF-GAP-002 |
| CAP-PREQ-003 | CAP-ENABLE-002 | CAP-CGATE-002, CAP-CGATE-003 | Partially confirmed | CAP-OFF-GAP-002, CAP-OFF-GAP-003 |
| CAP-PREQ-004 | CAP-ENABLE-002 | CAP-CGATE-003 | Partially confirmed | CAP-OFF-GAP-003 |
| CAP-PREQ-005 | CAP-ENABLE-002 | CAP-CGATE-002, CAP-CGATE-003 | Partially confirmed | CAP-OFF-GAP-002, CAP-OFF-GAP-005 |
| CAP-PREQ-006 | CAP-ENABLE-002 | CAP-CGATE-002, CAP-CGATE-003 | Partially confirmed | CAP-OFF-GAP-003, CAP-OFF-GAP-007 |
| CAP-PREQ-007 | CAP-ENABLE-002 | CAP-CGATE-002 | Partially confirmed | CAP-OFF-GAP-004, CAP-OFF-GAP-013 |
| CAP-PREQ-008 | CAP-ENABLE-002 | CAP-CGATE-002 | Partially confirmed | CAP-OFF-GAP-002 |
| CAP-PREQ-009 | CAP-ENABLE-002 | CAP-CGATE-002 | Partially confirmed | CAP-OFF-GAP-002 |
| CAP-PREQ-010 | CAP-ENABLE-002 | CAP-CGATE-002 | Partially confirmed | CAP-OFF-GAP-002 |
| CAP-PREQ-011 | CAP-ENABLE-002 | CAP-CGATE-002 | Partially confirmed | CAP-OFF-GAP-004, CAP-OFF-GAP-005 |
| CAP-PREQ-012 | CAP-ENABLE-002 | CAP-CGATE-002 | Unknown | CAP-OFF-GAP-002, CAP-OFF-GAP-008 |
| CAP-PREQ-013 | CAP-ENABLE-004 | CAP-CGATE-004, CAP-CGATE-007 | Partially confirmed | CAP-OFF-GAP-004, CAP-OFF-GAP-011 |
| CAP-PREQ-014 | CAP-ENABLE-005 | CAP-CGATE-005, CAP-CGATE-006 | Partially confirmed | CAP-OFF-GAP-005, CAP-OFF-GAP-009 |
| CAP-PREQ-015 | CAP-ENABLE-005 | CAP-CGATE-005 | Partially confirmed | CAP-OFF-GAP-005, CAP-OFF-GAP-009 |
| CAP-PREQ-016 | CAP-ENABLE-005 | CAP-CGATE-005, CAP-CGATE-006 | Partially confirmed | CAP-OFF-GAP-004, CAP-OFF-GAP-006, CAP-OFF-GAP-009 |
| CAP-PREQ-017 | CAP-ENABLE-004 | CAP-CGATE-004 | Partially confirmed | CAP-OFF-GAP-007 |
| CAP-PREQ-018 | CAP-ENABLE-004 | CAP-CGATE-004 | Partially confirmed | CAP-OFF-GAP-007, CAP-OFF-GAP-010 |
| CAP-PREQ-019 | CAP-ENABLE-004 | CAP-CGATE-004 | Partially confirmed | CAP-OFF-GAP-007, CAP-OFF-GAP-010 |
| CAP-PREQ-020 | CAP-ENABLE-004 | CAP-CGATE-004, CAP-CGATE-007 | Unknown | CAP-OFF-GAP-011 |
| CAP-PREQ-021 | CAP-ENABLE-004 | CAP-CGATE-004, CAP-CGATE-010 | Unknown | CAP-OFF-GAP-012 |
| CAP-PREQ-022 | CAP-ENABLE-005 | CAP-CGATE-006, CAP-CGATE-007 | Partially confirmed | CAP-OFF-GAP-004, CAP-OFF-GAP-013 |
| CAP-PREQ-023 | CAP-ENABLE-005 | CAP-CGATE-006 | Partially confirmed | CAP-OFF-GAP-009, CAP-OFF-GAP-013 |
| CAP-PREQ-024 | CAP-ENABLE-006 | CAP-CGATE-007 | Unknown | CAP-OFF-GAP-015 |
| CAP-PREQ-025 | CAP-ENABLE-006 | CAP-CGATE-007 | Unknown | CAP-OFF-GAP-011, CAP-OFF-GAP-015 |
| CAP-PREQ-026 | CAP-ENABLE-006 | CAP-CGATE-007, CAP-CGATE-009 | Unknown | CAP-OFF-GAP-011, CAP-OFF-GAP-015 |
| CAP-PREQ-027 | CAP-ENABLE-003 | CAP-CGATE-008 | Unknown | CAP-OFF-GAP-001, CAP-OFF-GAP-002, CAP-OFF-GAP-014 |
| CAP-PREQ-028 | CAP-ENABLE-007 | CAP-CGATE-010 | Unknown | CAP-OFF-GAP-007, CAP-OFF-GAP-010, CAP-OFF-GAP-012 |
| CAP-PREQ-029 | CAP-ENABLE-006 | CAP-CGATE-009 | Unknown | CAP-OFF-GAP-014, CAP-OFF-GAP-015 |
| CAP-PREQ-030 | CAP-ENABLE-006 | CAP-CGATE-009 | Unknown | CAP-OFF-GAP-014, CAP-OFF-GAP-015 |

## 21. Traceability

| Trace source | Mapping | Future use | Current state |
| --- | --- | --- | --- |
| Official source | CAP-OFF-EVID / CAP-OFF-GAP | Candidate API identity and gap reassessment | Static research only |
| CAP-OFF-EVID | Candidate API／SDK identity | CAP-PAIR and CAP-ENABLE review | Referenced |
| CAP-OFF-GAP | Unresolved official claim | CAP-PREQ / CAP-BLOCK / CAP-CGATE reassessment | Open |
| CAP-PAIR-001..010 | Candidate–Host evidence mapping | Future host prototype and runtime spike | In scope; no ranking |
| CAP-PREQ-001..030 | Prerequisite implication | Future closure plan reassessment | Not closed |
| CAP-BLOCK-001..012 | Blocker implication | Future enablement reassessment | Not closed |
| CAP-ENABLE-001..007 | Evidence improvement | Future enablement status update | Recommendation only |
| CAP-CGATE-001..010 | Gate evidence sufficiency | Future closure authorization review | Not passed |
| RESEARCH-TECH-UI-007..009 | Shared UI authority context | Future shared UI review | Inherited only |
| RESEARCH-TECH-RENDER-003 | Rendering dependency context | Future synthetic scene review | Referenced only |
| Architecture/adr/ADR-0002-ui-framework-selection.md | UI decision context | Future UI authority review | Draft; unresolved |
| Architecture/TECHNOLOGY-DECISION-ROADMAP.md | Technology decision context | Future candidate decision | No decision made |


## 22. Completion Conditions

本文件完成條件：

- 只建立 docs/Research/Technology/25-capture-backend-official-prerequisite-evidence-baseline.md。
- 只使用 Microsoft 第一方資料作為主要證據。
- 建立五個 Candidate API identity records。
- 覆蓋十個 CAP-PAIR。
- 建立 Activation／Interop、SDK／Header／Namespace、Graphics Device／Frame、Coordinate、Overlay／Cursor、Security及 Failure／Recovery matrices。
- 每個實質技術 claim 均有 CAP-OFF-EVID。
- 無法證明的項目建立 CAP-OFF-GAP。
- 覆蓋七個 CAP-ENABLE。
- 覆蓋十個 CAP-CGATE。
- 不執行本機盤點或 Package Cache 查詢。
- 不下載、安裝、Restore、Build、Run 或呼叫 Capture API。
- 不建立 Project、Prototype、Result、Capture Frame、Screenshot 或 Evidence Artifact。
- 不修改 RESEARCH-TECH-CAPTURE-001..005。
- 不修改 UI／Rendering Research Line。
- 不建立 Capture ADR。
- 不選擇 Capture Backend。
- git diff --check 應通過。
