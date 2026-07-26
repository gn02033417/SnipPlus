# Rendering Technology Official Candidate Evidence Baseline

## Document Control

| Field | Value |
|---|---|
| Document ID | `RESEARCH-TECH-RENDER-006` |
| Title | Rendering Technology Official Candidate Evidence Baseline |
| Status | Draft |
| Research type | Official source evidence baseline |
| Execution | Official-source research only |
| Local environment inspection | Not performed |
| Package cache inspection | Not performed |
| Build verification | Not performed |
| Runtime verification | Not performed |
| Parent enablement specification | `RESEARCH-TECH-RENDER-005` |
| Parent closure plan | `RESEARCH-TECH-RENDER-004` |
| Technology decision | `TD-002 Rendering Technology` |
| Rendering technology host framework decision | Unresolved |
| Rendering decision | Not made |
| Closure execution authorized | No |
| Runtime spike execution authorized | No |
| Owner | TBD |
| Last reviewed | Not reviewed |
| Version | 0.1 |
| Date | 2026-07-26 |
| Supersedes | None |
| References | `RESEARCH-TECH-RENDER-001` through `005`, `ADR-0002`, `TECHNOLOGY-DECISION-ROADMAP` |

## 1. Purpose

本文件建立 Rendering Candidate 的官方第一方資料基線，用來回答：

> 五個 Rendering Candidate 的精確技術身分、套件或 API、官方版本、Host 範圍、Managed／Native dependency 與未來實驗版本，可以由官方資料確定到什麼程度？

本文件的用途是：

- 關閉 `RESEARCH-TECH-RENDER-005` 中可以由官方資料完成的 specification gaps。
- 固定五個 Candidate 的正式名稱與技術邊界。
- 分離官方可用版本、本機可用性、Build 驗證與 Runtime 驗證。
- 為後續 Rendering prerequisite authorization request 提供可追溯的候選身分證據。
- 防止「Package 頁面存在」被誤解為「特定 Host 已相容」或「本機可以建置」。

本文件不做 Rendering Technology 選擇，也不替 `TD-002` 建立 ADR。

## 2. Scope

本文件只研究以下 Candidate、Host 與官方證據面：

| Scope item | Included |
|---|---|
| Candidate | `RND-OPT-001` Framework-native retained-mode rendering |
| Candidate | `RND-OPT-002` Direct2D／DirectWrite |
| Candidate | `RND-OPT-003` Win2D |
| Candidate | `RND-OPT-004` SkiaSharp |
| Candidate | `RND-OPT-005` Hybrid interaction and rendering surface |
| Host | WinUI 3 with Windows App SDK |
| Host | WPF |
| Pair coverage | `RND-PAIR-001` through `RND-PAIR-010` |
| Enablement coverage | `RND-ENABLE-001` through `RND-ENABLE-006` |
| Gate coverage | `RND-CGATE-001` through `RND-CGATE-008` |
| Evidence | Official product documentation, official API reference, official maintainer repository, official release notes, official package registry metadata |

## 3. Non-goals and Prohibited Operations

本輪不得執行以下行為：

- 執行本機系統查詢、SDK inventory、AppX inventory 或 workload inventory。
- 查詢 NuGet global package cache 或任何本機 Package cache。
- 執行 `dotnet --info`、workload query、Restore、Build、Run 或 Publish。
- 下載或安裝 SDK、Runtime、Package、native asset 或工具。
- 建立 Project、Prototype、Result directory、Source Code、Reference Image、PNG 或量測資料。
- 進行 Runtime interoperability、DPI、Overlay、hit testing 或 export fidelity 驗證。
- 選擇 Rendering Technology、選擇正式產品版本或關閉 `TD-002`。
- 建立 TD-002 ADR 或修改 `ADR-0002`。
- 修改 `RESEARCH-TECH-RENDER-001` 至 `005`。
- 修改 UI Research Line。

本文件的任何 `Current official version` 都不表示本機已安裝，也不表示可 Restore、Build 或 Runtime execution。

## 4. Source Acceptance Policy

### 4.1 Evidence priority

來源優先順序固定如下：

1. 官方產品文件。
2. 官方 API reference。
3. 官方維護者 Repository 與 Release Notes。
4. 官方 Package Registry metadata。
5. 官方相容性、支援生命週期或平台需求文件。

### 4.2 Source treatment

| Source type | Treatment |
|---|---|
| Microsoft Learn product or API documentation | Primary evidence |
| Microsoft-maintained GitHub Repository | Primary evidence for repository-maintained identity and release context |
| NuGet Gallery package metadata | Primary registry evidence for package identity, versions, frameworks and declared dependencies |
| Official release notes | Primary release evidence |
| Third-party article or sample | Informative only; cannot close a prerequisite |
| Search result summary | Discovery only; never evidence |
| AI-generated summary | Never evidence |
| Forum, Stack Overflow or personal blog | Informative only; never sole compatibility evidence |

### 4.3 Claim discipline

每一個實質相容性結論至少引用一個 `RND-OFF-EVID`。若來源只證明 API 或 Package 存在，結論只能寫成 API／Package identity；不能延伸為特定 Host 已支援。

「官方沒有找到明確聲明」只能形成 `Unknown` 或 `Documentation insufficient`，不得直接形成 `Not aligned`。`Not aligned` 必須有官方限制或明確缺少支援的證據。

## 5. Evidence Vocabulary

### 5.1 Claim Status

只能使用以下值：

- `Confirmed by official source`
- `Partially confirmed`
- `Conflicting official evidence`
- `Unknown`
- `Not applicable`

### 5.2 Host Support Status

只能使用以下值：

- `Officially documented`
- `Official package or API exists, host suitability unverified`
- `Requires managed/native interop`
- `Requires runtime prototype`
- `Not aligned by official evidence`
- `Unknown`

### 5.3 Experimental Version Status

只能使用以下值：

- `Proposed for future spike`
- `Candidate version identified`
- `Blocked by unresolved compatibility`
- `TBD`

### 5.4 Forbidden conclusion words

以下字詞在本文件中不代表正式結論，不得作為 Candidate status：

- `Best`
- `Winner`
- `Recommended for product`
- `Definitely compatible`
- `Should work`
- `Probably supported`

## 6. Official Evidence Register

下表是本文件的唯一官方來源登錄表。`Access date` 是本輪研究日期；若官方頁面沒有明確出版日或 Release date，記錄為 `Not stated on accessed page`，不自行推算。

| Evidence ID | Official source title | Publisher／maintainer | Source URL | Access date | Main claim supported |
|---|---|---|---|---|---|
| `RND-OFF-EVID-001` | WinUI in the Windows App SDK (WinUI 3) | Microsoft | [Microsoft Learn: WinUI 3](https://learn.microsoft.com/en-us/windows/apps/winui/winui3/) | 2026-07-26 | WinUI 3 is the native UI platform component delivered by Windows App SDK; C#／C++ and XAML boundary |
| `RND-OFF-EVID-002` | Windows App SDK platform overview | Microsoft | [Microsoft Learn: platform overview](https://learn.microsoft.com/en-us/windows/apps/develop/platform/) | 2026-07-26 | WinUI 3 XAML namespace and supported language projections |
| `RND-OFF-EVID-003` | Windows app development documentation | Microsoft | [Microsoft Learn: Windows app development](https://learn.microsoft.com/en-us/windows/apps/) | 2026-07-26 | WinUI 3 is recommended for new native Windows desktop apps; WPF is a mature desktop framework |
| `RND-OFF-EVID-004` | Canvas Class (`Microsoft.UI.Xaml.Controls`) | Microsoft | [Microsoft Learn: Canvas class](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.controls.canvas) | 2026-07-26 | WinUI Canvas identity as a XAML `Panel`; not evidence of a cross-host drawing package |
| `RND-OFF-EVID-005` | WPF Graphics Rendering Overview | Microsoft | [Microsoft Learn: WPF graphics rendering overview](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/graphics-multimedia/wpf-graphics-rendering-overview) | 2026-07-26 | WPF `Visual`, persisted render data and `DrawingContext` rendering model |
| `RND-OFF-EVID-006` | Direct2D portal | Microsoft | [Microsoft Learn: Direct2D](https://learn.microsoft.com/en-us/windows/win32/direct2d/direct2d-portal) | 2026-07-26 | Direct2D is a hardware-accelerated, immediate-mode, 2-D graphics API |
| `RND-OFF-EVID-007` | Text Rendering with Direct2D and DirectWrite | Microsoft | [Microsoft Learn: Direct2D and DirectWrite](https://learn.microsoft.com/en-us/windows/win32/direct2d/direct2d-and-directwrite) | 2026-07-26 | Direct2D／DirectWrite relationship, text layout, glyph and rendering API boundaries |
| `RND-OFF-EVID-008` | Overview of the Windows Graphics Architecture | Microsoft | [Microsoft Learn: graphics architecture](https://learn.microsoft.com/en-us/windows/win32/learnwin32/overview-of-the-windows-graphics-architecture) | 2026-07-26 | Direct2D and DirectWrite are Windows graphics APIs; official guidance distinguishes them from framework rendering |
| `RND-OFF-EVID-009` | Overview of Win2D | Microsoft | [Microsoft Learn: Win2D overview](https://learn.microsoft.com/en-us/windows/apps/develop/win2d/in-a-core-app) | 2026-07-26 | Win2D is a WinRT immediate-mode 2-D API with GPU acceleration for WinUI Windows App SDK apps |
| `RND-OFF-EVID-010` | Tutorial: Build a simple Win2D app | Microsoft | [Microsoft Learn: Win2D quick start](https://learn.microsoft.com/en-us/windows/apps/develop/win2d/quick-start) | 2026-07-26 | `Microsoft.Graphics.Win2D`, `CanvasControl`, C#／XAML usage and x86／x64 architecture requirement in tutorial |
| `RND-OFF-EVID-011` | `Microsoft.Graphics.Win2D` package metadata | Microsoft／win2d | [NuGet Gallery: Microsoft.Graphics.Win2D](https://www.nuget.org/packages/Microsoft.Graphics.Win2D/) | 2026-07-26 | Package identity, registry version `1.4.0`, compatible framework metadata and native package nature |
| `RND-OFF-EVID-012` | Windows App SDK overview | Microsoft | [Microsoft Learn: Windows App SDK](https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/) | 2026-07-26 | Windows App SDK can be used with WinUI 3 and existing WPF／WinForms／Win32 applications; this does not make WinUI rendering host-neutral |
| `RND-OFF-EVID-013` | Windows versions and SDK overview | Microsoft | [Microsoft Learn: versioning overview](https://learn.microsoft.com/en-us/windows/apps/get-started/versioning-overview) | 2026-07-26 | Windows App SDK versioning is separate from Windows OS and Windows SDK versioning; current line must be recorded separately |
| `RND-OFF-EVID-014` | Windows App SDK deployment overview | Microsoft | [Microsoft Learn: deployment overview](https://learn.microsoft.com/en-us/windows/apps/package-and-deploy/deploy-overview) | 2026-07-26 | Framework-dependent／self-contained modes and x64／ARM64 architecture implications |
| `RND-OFF-EVID-015` | Windows App SDK deployment architecture | Microsoft | [Microsoft Learn: deployment architecture](https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/deployment-architecture) | 2026-07-26 | Framework package, runtime package and packaged／unpackaged dependency boundary |
| `RND-OFF-EVID-016` | SkiaSharp Repository README | Microsoft／Xamarin maintainers | [GitHub: mono/SkiaSharp](https://github.com/mono/SkiaSharp) | 2026-07-26 | SkiaSharp identity, cross-platform .NET API, WinUI 3 and Windows Classic Desktop／WPF support claims |
| `RND-OFF-EVID-017` | `SkiaSharp` package metadata | Microsoft／Xamarin maintainers | [NuGet Gallery: SkiaSharp](https://www.nuget.org/packages/SkiaSharp/) | 2026-07-26 | Core package identity, stable registry version `4.150.1`, target framework metadata and Windows native asset dependency |
| `RND-OFF-EVID-018` | `SkiaSharp.NativeAssets.Win32` package metadata | Microsoft／Xamarin maintainers | [NuGet Gallery: SkiaSharp.NativeAssets.Win32](https://www.nuget.org/packages/SkiaSharp.NativeAssets.Win32) | 2026-07-26 | Win32 native asset package identity and version `4.150.1`; package presence is not deployment verification |
| `RND-OFF-EVID-019` | `SkiaSharp.Views.WPF` package metadata | Microsoft／Xamarin maintainers | [NuGet Gallery: SkiaSharp.Views.WPF](https://www.nuget.org/packages/SkiaSharp.Views.WPF) | 2026-07-26 | WPF view integration package identity and registry version `4.150.1` |
| `RND-OFF-EVID-020` | `SkiaSharp.Views.WinUI` package metadata | Microsoft／Xamarin maintainers | [NuGet Gallery: SkiaSharp.Views.WinUI](https://www.nuget.org/packages/SkiaSharp.Views.WinUI/) | 2026-07-26 | WinUI view integration package identity, `Microsoft.WindowsAppSDK` dependency and WinUI native asset dependency |
| `RND-OFF-EVID-021` | SkiaSharp release context | Microsoft／Xamarin maintainers | [GitHub: SkiaSharp releases](https://github.com/mono/SkiaSharp/releases) | 2026-07-26 | Official repository release context; discrepancy with registry version is retained as an evidence gap |
| `RND-OFF-EVID-022` | Latest Windows App SDK downloads | Microsoft | [Microsoft Learn: Windows App SDK downloads](https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/downloads) | 2026-07-26 | Runtime redistribution includes architecture-specific packages; no local availability conclusion |
| `RND-OFF-EVID-023` | WinUI 3 official scope | Microsoft | [Microsoft Learn: WinUI 3 overview](https://learn.microsoft.com/en-ca/windows/apps/winui/winui3/) | 2026-07-26 | WinUI 3 is delivered as part of Windows App SDK and is a Windows desktop UI framework |
| `RND-OFF-EVID-024` | WPF visual rendering model | Microsoft | [Microsoft Learn: WPF rendering overview](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/graphics-multimedia/wpf-graphics-rendering-overview) | 2026-07-26 | WPF `Visual` and drawing content are retained by the graphics system rather than an immediate-mode screen API |

## 7. Candidate Identity Baseline

### 7.1 Five-candidate baseline

| Candidate | Exact official identity | Technology owner | Package／API identity | Current official version | Experimental version status | Host scope | Evidence |
|---|---|---|---|---|---|---|---|
| `RND-OPT-001` Framework-native retained-mode rendering | WinUI 3 XAML／WPF Visual rendering surfaces; not one cross-host product | Microsoft | WinUI 3 is delivered by Windows App SDK; WPF uses framework `Visual`／`DrawingContext` | WinUI line is governed by Windows App SDK version; WPF framework version TBD | Candidate version identified per Host | WinUI 3 and WPF must be recorded separately | `RND-OFF-EVID-001`, `004`, `005`, `012`, `013` |
| `RND-OPT-002` Direct2D／DirectWrite | Windows native 2-D and text APIs | Microsoft | `ID2D1*` Direct2D API and `IDWrite*` DirectWrite API; no cross-host managed package asserted | Windows SDK version TBD; OS API identity is separate from Windows App SDK | Candidate version identified | Win32 API identity confirmed; Host integration remains separate | `RND-OFF-EVID-006`, `007`, `008`, `013` |
| `RND-OPT-003` Win2D | Win2D Windows Runtime immediate-mode 2-D API | Microsoft／win2d | `Microsoft.Graphics.Win2D`; `CanvasControl`; `CanvasDrawingSession` | `1.4.0` registry metadata | Candidate version identified | Officially documented for WinUI Windows App SDK; WPF suitability unverified | `RND-OFF-EVID-009`, `010`, `011`, `012` |
| `RND-OPT-004` SkiaSharp | SkiaSharp .NET bindings for Google Skia | Microsoft／Xamarin maintainers | `SkiaSharp`; `SkiaSharp.Views.WPF`; `SkiaSharp.Views.WinUI`; Windows native asset packages | `4.150.1` stable registry metadata; repository release context conflicts | Candidate version identified | Official repository claims WinUI 3 and WPF support; project-specific interoperability remains unverified | `RND-OFF-EVID-016` through `021` |
| `RND-OPT-005` Hybrid interaction and rendering surface | Strategy combination, not a product or package | Depends on selected components | Interaction owner, rendering surface and interop boundary must be named per implementation | Not applicable; components retain their own versions | Blocked by unresolved compatibility | Separate WinUI 3 and WPF combinations only | `RND-OFF-EVID-001`, `005`, `006`, `009`, `012`, `016` |

### 7.2 Framework-native Host separation

`Framework-native` is a strategy label, not a package identity. The following identities must remain separate:

| Host | Official identity | Rendering model recorded here | What the evidence does not prove | Evidence |
|---|---|---|---|---|
| WinUI 3 | WinUI 3 delivered by Windows App SDK, using `Microsoft.UI.Xaml` | XAML／Visual framework surface; `Canvas` is a XAML `Panel` | A dedicated custom immediate-mode rendering API, WPF compatibility or product-level capture fidelity | `RND-OFF-EVID-001`, `002`, `004`, `012` |
| WPF | WPF `Visual` rendering layer and `DrawingContext` render data | Retained visual render data with vector, image and glyph content | WinUI 3 API equivalence, GPU path equivalence or cross-host package identity | `RND-OFF-EVID-005`, `024` |

### 7.3 Direct2D／DirectWrite separation

Direct2D and DirectWrite are recorded as Windows native API families. Their official existence does not establish a managed wrapper, XAML host adapter, or project-specific interop contract.

| Layer | Identity | Evidence boundary |
|---|---|---|
| Geometry／bitmap | Direct2D `ID2D1*` API family | Official API and rendering model only |
| Text layout／glyph | DirectWrite `IDWrite*` API family | Official text layout and rendering relationship only |
| Managed interop | Not fixed by this document | Requires a future candidate-specific interop choice and prototype |
| WinUI 3 host | Not fixed by this document | Host integration evidence remains open |
| WPF host | Not fixed by this document | Host integration evidence remains open |

## 8. Official Version Separation

每個 Candidate 必須分開記錄以下版本概念：

| Version concept | Meaning | This document's value |
|---|---|---|
| Current official release | Officially published current line at research time | Recorded where the official source exposes it |
| Latest stable release | Officially marked stable registry or release value | Recorded separately from prerelease |
| Experimental spike candidate | Version proposed for a future isolated spike | Candidate identity only; not installed or restored |
| Locally available version | Version found on the current machine | `Unknown`; local inspection prohibited |
| Build-verified version | Version that built in this repository | `No`; Build prohibited |
| Runtime-verified version | Version that rendered and passed runtime evidence | `No`; Runtime prohibited |
| Product version | Version selected for production | `Not decided` |

The following current official values are source facts only:

| Candidate or host | Current official source value | Interpretation | Evidence |
|---|---|---|---|
| WinUI 3／Windows App SDK | `2.2.x` line is listed in official versioning material; patch is not fixed here | Official SDK line only; no local or build claim | `RND-OFF-EVID-013` |
| WPF | No single rendering package version is defined by this baseline | Host framework version must follow selected .NET/WPF project baseline | `RND-OFF-EVID-003`, `005` |
| Direct2D／DirectWrite | Windows SDK and OS API versioning are separate from Windows App SDK | Exact SDK version must be fixed by future project setup | `RND-OFF-EVID-006`, `007`, `013` |
| Win2D | `Microsoft.Graphics.Win2D` `1.4.0` registry metadata | Package identity only; no local availability or Host build claim | `RND-OFF-EVID-010`, `011` |
| SkiaSharp | `SkiaSharp` `4.150.1` and Windows integration package metadata | Stable registry value at access date; repository release context is retained as conflict | `RND-OFF-EVID-016` through `021` |

## 9. Candidate–Host Official Compatibility Matrix

本矩陣覆蓋 `RND-PAIR-001` 至 `RND-PAIR-010`。`Status` 是官方文件證據狀態，不是本機 Build 或 Runtime 狀態。

| Pair | Candidate | Host | Official support claim | Package／API evidence | Interop required | Unsupported boundary | Status | Evidence IDs |
|---|---|---|---|---|---|---|---|---|
| `RND-PAIR-001` | `RND-OPT-001` Framework-native | WinUI 3 | WinUI 3 is the native UI framework in Windows App SDK | WinUI 3 XAML／Visual APIs; no separate cross-host renderer | Host-native framework surface | Custom rendering and export fidelity not proven | Officially documented | `001`, `002`, `004`, `012` |
| `RND-PAIR-002` | `RND-OPT-001` Framework-native | WPF | WPF Visual rendering and DrawingContext are official framework capabilities | WPF `Visual`／`DrawingContext` | Host-native framework surface | WinUI 3 equivalence and product capture fidelity not proven | Officially documented | `005`, `024` |
| `RND-PAIR-003` | `RND-OPT-002` Direct2D／DirectWrite | WinUI 3 | Direct2D／DirectWrite APIs exist; official Host adapter is not fixed here | Windows SDK native API families | Managed／native and XAML interop required | WinUI 3 integration path and Runtime rendering not documented by this baseline | Requires managed/native interop | `006`, `007`, `008`, `012` |
| `RND-PAIR-004` | `RND-OPT-002` Direct2D／DirectWrite | WPF | Direct2D／DirectWrite APIs exist; official WPF adapter is not fixed here | Windows SDK native API families | Managed／native and WPF interop required | WPF surface integration and retained/immediate composition not proven | Requires managed/native interop | `005`, `006`, `007`, `008` |
| `RND-PAIR-005` | `RND-OPT-003` Win2D | WinUI 3 | Win2D docs explicitly target WinUI Windows App SDK apps | `Microsoft.Graphics.Win2D`, `CanvasControl` | WinRT／XAML package integration | Package metadata does not replace project Build or Runtime verification | Officially documented | `009`, `010`, `011` |
| `RND-PAIR-006` | `RND-OPT-003` Win2D | WPF | Official Win2D docs accessed here describe WinUI Windows App SDK usage, not direct WPF support | Win2D package exists | Host interop would be required if pursued | Direct official WPF Host support is not established | Unknown | `009`, `010`, `011`, `012` |
| `RND-PAIR-007` | `RND-OPT-004` SkiaSharp | WinUI 3 | Official repository lists WinUI 3; official package metadata lists WinUI view integration | `SkiaSharp`, `SkiaSharp.Views.WinUI`, native asset package | Package and native asset integration | Project-specific SDK, architecture and Runtime fidelity not verified | Official package or API exists, host suitability unverified | `016`, `017`, `018`, `020`, `021` |
| `RND-PAIR-008` | `RND-OPT-004` SkiaSharp | WPF | Official repository lists Windows Classic Desktop／WPF; official WPF view package exists | `SkiaSharp`, `SkiaSharp.Views.WPF`, Win32 native asset package | Package and native asset integration | Project-specific target framework, deployment and Runtime fidelity not verified | Official package or API exists, host suitability unverified | `016` through `019`, `021` |
| `RND-PAIR-009` | `RND-OPT-005` Hybrid | WinUI 3 | Components may have separate official identities; no single Hybrid package exists | Depends on named interaction and rendering components | Component-to-host interop required | Combination has no official support claim as one product | Requires runtime prototype | `001`, `006`, `009`, `012`, `016` |
| `RND-PAIR-010` | `RND-OPT-005` Hybrid | WPF | Components may have separate official identities; no single Hybrid package exists | Depends on named interaction and rendering components | Component-to-host interop required | Combination has no official support claim as one product | Requires runtime prototype | `005`, `006`, `012`, `016` |

## 10. Target Framework and Platform Matrix

官方資料能確認平台邊界，但不能替代本專案未來的 project-level target selection。

| Candidate／integration | Target framework | Minimum Windows version | Windows App SDK dependency | WPF dependency | Architectures | Packaging implication | Evidence |
|---|---|---|---|---|---|---|---|
| Framework-native／WinUI 3 | Windows App SDK project target; exact .NET TFM TBD | Windows 10 1809+ line in WinUI documentation | Required for WinUI 3 | Not applicable | x86／x64／ARM family is part of WinUI scope | Packaged or unpackaged deployment must follow Windows App SDK model | `001`, `003`, `013`, `014` |
| Framework-native／WPF | Selected .NET/WPF TFM TBD | OS support follows selected WPF/.NET baseline | Not required for WPF rendering surface | WPF framework | Architecture follows selected project and native dependency choices | Existing WPF deployment model; Windows App SDK is optional and separate | `003`, `005`, `012` |
| Direct2D／DirectWrite／WinUI 3 | Windows SDK and selected managed interop TFM TBD | Windows API availability must be fixed by future SDK baseline | Host integration not fixed | Not applicable | Native interop architecture must match host process | Native binaries and registration/deployment implications require future evidence | `006`, `007`, `013`, `014` |
| Direct2D／DirectWrite／WPF | Selected .NET/WPF TFM plus interop baseline TBD | Windows API availability must be fixed by future SDK baseline | Optional and not implied | WPF host plus interop boundary | Native interop architecture must match host process | Native binaries and deployment implications require future evidence | `005`, `006`, `007`, `014` |
| Win2D／WinUI 3 | Package metadata lists `net6.0-windows10.0.19041` compatibility; project TFM TBD | Win2D tutorial targets Windows App SDK/WinUI and has platform requirements | Required by WinUI host | Not applicable | Tutorial requires x86 or x64 rather than Any CPU; ARM64 needs separate confirmation | Native C++ implementation and architecture-specific build implication | `009`, `010`, `011`, `014` |
| Win2D／WPF | Package registry lists compatible framework metadata, but direct WPF Host support is not established | Unknown for direct WPF integration | Not established | WPF host would need an integration boundary | Unknown | Native package and host integration implications unknown | `010`, `011`, `012` |
| SkiaSharp core／WinUI view | Registry lists compatible .NET Windows TFMs; exact project TFM TBD | Windows view package metadata includes Windows target framework | `SkiaSharp.Views.WinUI` metadata declares `Microsoft.WindowsAppSDK` dependency | Not applicable | Native assets and WinUI package architecture must be matched | WinUI native assets and Windows App SDK deployment must be planned separately | `016`, `017`, `020`, `014` |
| SkiaSharp core／WPF view | Registry lists .NET Framework 4.6.2+ and .NET Standard compatibility; project TFM TBD | Windows support depends on selected .NET/WPF baseline | Not required by WPF view package identity | `SkiaSharp.Views.WPF` | Win32 native assets must match process architecture | `SkiaSharp.NativeAssets.Win32` is a dependency implication, not deployment proof | `016` through `019` |
| Hybrid／WinUI 3 | Component-specific; TBD | Component-specific | Depends on selected WinUI component | Not applicable | Union of all component architecture constraints | Packaging must include every selected component's requirements | `001`, `012`, `014`, `016` |
| Hybrid／WPF | Component-specific; TBD | Component-specific | Optional per component | WPF plus selected rendering integration | Union of all component architecture constraints | Packaging must include every selected component's requirements | `005`, `012`, `014`, `016` |

### 10.1 Architecture interpretation

- A registry-compatible TFM is package metadata, not repository compatibility.
- An x86／x64 or ARM64 statement is an architecture requirement or support claim, not a local inventory result.
- A Windows App SDK dependency is a package or deployment relationship, not evidence that the current machine has the runtime.
- `Any CPU` is not accepted for the Win2D tutorial path because the official tutorial explicitly requests a concrete architecture; this is a future project constraint only.

## 11. Managed and Native Dependency Matrix

| Candidate | Managed package | Native API／binary | Transitive dependency evidence | Runtime asset implication | Deployment implication | Evidence |
|---|---|---|---|---|---|---|
| Framework-native／WinUI 3 | Windows App SDK／WinUI project references; exact package set TBD | Framework and platform binaries managed by selected Windows App SDK deployment model | Official deployment docs define framework, main, singleton and related package boundary | Runtime package model must be selected later | Framework-dependent and self-contained modes are separate choices | `RND-OFF-EVID-001`, `012`, `014`, `015` |
| Framework-native／WPF | WPF framework references | Framework rendering implementation | No separate third-party renderer asserted | Native renderer is part of WPF/.NET runtime boundary | Follows WPF deployment baseline | `RND-OFF-EVID-005`, `024` |
| Direct2D／DirectWrite | No managed package fixed by this document | Windows SDK Direct2D／DirectWrite COM／Win32 API families | Official APIs are native; managed projection is not selected | Future interop layer must define native loading and lifetime | Deployment must account for process architecture and interop binaries if any | `RND-OFF-EVID-006`, `007`, `008` |
| Win2D | `Microsoft.Graphics.Win2D` | Win2D uses Direct2D and is implemented in C++ according to the official tutorial boundary | Package metadata and quick start identify package and architecture considerations | Native package assets and architecture need future Restore／Build evidence | WinUI host and concrete architecture required by future spike | `RND-OFF-EVID-009`, `010`, `011` |
| SkiaSharp／WinUI | `SkiaSharp`, `SkiaSharp.Views.WinUI` | `SkiaSharp.NativeAssets.WinUI` plus core native Skia assets | Official package metadata declares package dependencies; native asset contents not inspected locally | Native WinUI asset package must match selected version and architecture | Windows App SDK and Skia native asset deployment both require future evidence | `RND-OFF-EVID-016`, `017`, `020` |
| SkiaSharp／WPF | `SkiaSharp`, `SkiaSharp.Views.WPF` | `SkiaSharp.NativeAssets.Win32` | Core package metadata declares Windows native asset dependency; no local cache inspection | Win32 native asset package must match selected version and architecture | Deployment must carry compatible native assets | `RND-OFF-EVID-016` through `019` |
| Hybrid | Component-specific managed package set | Component-specific native API／binary set | Cannot be inferred before components are named | Union of selected component asset requirements | Packaging and deployment are an explicit future closure item | `RND-OFF-EVID-001`, `005`, `006`, `009`, `016` |

## 12. Experimental Version Proposal

下表是未來 Spike 的候選版本提案，不是產品決策，也不是本機可用性聲明。

| Candidate | Proposed experimental identity／version | Host | Evidence basis | Compatibility confidence | Remaining validation | Proposal status |
|---|---|---|---|---|---|---|
| `RND-OPT-001` Framework-native | WinUI 3 on Windows App SDK `2.2.x` line; WPF selected framework baseline TBD | WinUI 3 and WPF as separate rows in future spike | Official Host framework identity and version separation | Partially documented | Host-specific render surface, DPI, overlay, text and export workload | Candidate version identified |
| `RND-OPT-002` Direct2D／DirectWrite | Windows SDK version TBD; `ID2D1*`／`IDWrite*` API baseline | WinUI 3 | Official native API identity; no selected managed interop | Documentation insufficient | Managed/native lifetime, XAML interop, surface composition and target architecture | Proposed for future spike |
| `RND-OPT-002` Direct2D／DirectWrite | Windows SDK version TBD; `ID2D1*`／`IDWrite*` API baseline | WPF | Official native API identity; no selected WPF adapter | Documentation insufficient | Managed/native lifetime, WPF surface interop, composition and target architecture | Proposed for future spike |
| `RND-OPT-003` Win2D | `Microsoft.Graphics.Win2D` `1.4.0` | WinUI 3 | Official Win2D overview, quick start and registry metadata | Officially documented | Restore, concrete project target, architecture build, Runtime workload and export evidence | Candidate version identified |
| `RND-OPT-003` Win2D | `Microsoft.Graphics.Win2D` `1.4.0` | WPF | Package identity exists but direct official WPF support is not established | Documentation insufficient | Host adapter, project build and Runtime interoperability | Blocked by unresolved compatibility |
| `RND-OPT-004` SkiaSharp | `SkiaSharp`／`SkiaSharp.Views.WinUI` `4.150.1` plus matching native assets | WinUI 3 | Official repository support list and registry dependency metadata | Partially documented | Resolve repository/registry release context, Restore, architecture build and Runtime fidelity | Candidate version identified |
| `RND-OPT-004` SkiaSharp | `SkiaSharp`／`SkiaSharp.Views.WPF` `4.150.1` plus matching Win32 assets | WPF | Official repository support list and WPF package metadata | Partially documented | Resolve release context, Restore, architecture build, deployment and Runtime fidelity | Candidate version identified |
| `RND-OPT-005` Hybrid | Named component versions TBD | WinUI 3 | Strategy has no single official package or version | Documentation insufficient | Define interaction owner, rendering owner and every interop boundary | Blocked by unresolved compatibility |
| `RND-OPT-005` Hybrid | Named component versions TBD | WPF | Strategy has no single official package or version | Documentation insufficient | Define interaction owner, rendering owner and every interop boundary | Blocked by unresolved compatibility |

### 12.1 Version proposal rules

- `Current official release` cannot automatically become `Experimental spike candidate`.
- A WinUI 3 integration package and WPF integration package must be recorded separately when they differ.
- Native asset packages must be version-aligned with their managed package before a future Restore is authorized.
- A prerelease registry entry is not selected merely because it is newer.
- The repository／registry discrepancy for SkiaSharp is an open evidence issue, not a product recommendation.

## 13. Official Evidence Gap Register

| Gap ID | Candidate | Host | Missing claim | Sources checked | Why evidence is insufficient | Related Pair | Related prerequisite | Related enablement item | Required future evidence | Runtime prototype required | Blocks Phase R1 | Status |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| `RND-OFF-GAP-001` | Framework-native | WinUI 3 | Exact custom rendering surface suitable for the workload | WinUI overview, Canvas API | Canvas is a XAML panel; the baseline does not establish a custom renderer/export path | `001` | `RND-PREQ-001` | `RND-ENABLE-001` | Official API selection plus isolated project evidence | Yes | Yes | Open |
| `RND-OFF-GAP-002` | Framework-native | WPF | Exact custom rendering surface and capture fidelity boundary | WPF rendering overview | WPF Visual/DrawingContext is documented, but product workload fidelity is not | `002` | `RND-PREQ-001` | `RND-ENABLE-001` | Isolated workload result and coordinate/fidelity evidence | Yes | Yes | Open |
| `RND-OFF-GAP-003` | Direct2D／DirectWrite | WinUI 3 | Managed/native interop and surface composition contract | Direct2D, DirectWrite, Windows App SDK docs | Native API existence does not define the Host integration layer | `003` | `RND-PREQ-002` | `RND-ENABLE-002` | Official interop API references plus Runtime prototype | Yes | Yes | Open |
| `RND-OFF-GAP-004` | Direct2D／DirectWrite | WPF | Managed/native interop and WPF composition contract | Direct2D, DirectWrite, WPF docs | Native API existence does not define the WPF integration layer | `004` | `RND-PREQ-002` | `RND-ENABLE-002` | WPF interop prototype and evidence | Yes | Yes | Open |
| `RND-OFF-GAP-005` | Win2D | WPF | Official direct WPF Host support | Win2D overview and quick start | Official pages accessed describe WinUI Windows App SDK usage; absence is not proof of incompatibility | `006` | `RND-PREQ-001` | `RND-ENABLE-001` | Official support statement or accepted Runtime prototype | Yes | Yes | Open |
| `RND-OFF-GAP-006` | SkiaSharp | WinUI 3 | Exact SDK／architecture／deployment combination for this project | SkiaSharp README and package metadata | Generic support list does not verify this project’s target or deployment model | `007` | `RND-PREQ-005` | `RND-ENABLE-005` | Future Restore／Build／Runtime evidence | Yes | Yes | Open |
| `RND-OFF-GAP-007` | SkiaSharp | WPF | Exact target framework and deployment combination for this project | SkiaSharp README and package metadata | Package compatibility is not project Build or native asset deployment evidence | `008` | `RND-PREQ-005` | `RND-ENABLE-005` | Future Restore／Build／Runtime evidence | Yes | Yes | Open |
| `RND-OFF-GAP-008` | Hybrid | WinUI 3 | Component ownership and interop topology | Framework, Direct2D, Win2D and SkiaSharp sources | Hybrid is a strategy, not an official package or support claim | `009` | `RND-PREQ-006` | `RND-ENABLE-006` | Named component matrix and Runtime prototype | Yes | Yes | Open |
| `RND-OFF-GAP-009` | Hybrid | WPF | Component ownership and interop topology | Framework, Direct2D, Win2D and SkiaSharp sources | Hybrid is a strategy, not an official package or support claim | `010` | `RND-PREQ-006` | `RND-ENABLE-006` | Named component matrix and Runtime prototype | Yes | Yes | Open |
| `RND-OFF-GAP-010` | SkiaSharp | WinUI 3/WPF | Repository release context versus NuGet `4.150.1` registry context | Official repository and NuGet metadata | Accessed official surfaces expose different release context | `007`, `008` | `RND-PREQ-005` | `RND-ENABLE-005` | Maintainer release mapping or package release notes | No | No | Open |
| `RND-OFF-GAP-011` | Win2D | WinUI 3 | Exact package version compatibility with selected Windows App SDK line | Win2D package and Windows App SDK version docs | Package registry and SDK line are separately versioned; no project lock is selected | `005` | `RND-PREQ-005` | `RND-ENABLE-005` | Future package compatibility and Restore evidence | Yes | Yes | Open |
| `RND-OFF-GAP-012` | All | WinUI 3/WPF | Mixed-language text and font fallback fidelity for target workload | Framework and renderer source pages | General text API capability does not prove target fonts, fallback or pixel output | `001` through `010` | `RND-PREQ-003` | `RND-ENABLE-003` | Synthetic workload Runtime evidence | Yes | Yes | Open |
| `RND-OFF-GAP-013` | All | WinUI 3/WPF | DPI and fractional-scale behavior | Framework and deployment sources | Platform support claims do not replace target monitor configuration evidence | `001` through `010` | `RND-PREQ-002` | `RND-ENABLE-002` | DPI workload and coordinate comparison | Yes | Yes | Open |
| `RND-OFF-GAP-014` | All | WinUI 3/WPF | Reference image and pixel-difference method | No official candidate source can close product fidelity method | Method is repository-specific evidence, not package metadata | `001` through `010` | `RND-PREQ-004` | `RND-ENABLE-004` | Future evidence method approval and result artifacts | Yes | Yes | Open |
| `RND-OFF-GAP-015` | All | WinUI 3/WPF | Architecture-specific native asset loading | Package/deployment sources | Official package metadata does not prove this repository’s native load path | `001` through `010` | `RND-PREQ-005` | `RND-ENABLE-005` | Future architecture Build and Runtime evidence | Yes | Yes | Open |
| `RND-OFF-GAP-016` | All | WinUI 3/WPF | Product-level rendering decision | All official sources | Technology selection is a product and architecture decision outside this research file | `001` through `010` | `RND-PREQ-006` | `RND-ENABLE-006` | Future decision record after evidence review | Yes | Yes | Open |

## 14. Enablement Evidence Mapping

下表只記錄本文件對 `RESEARCH-TECH-RENDER-005` 的改善；不修改該文件。

| Enablement item | Required official evidence | Evidence IDs | Specification improved | Remaining gap | Status recommendation |
|---|---|---|---|---|---|
| `RND-ENABLE-001` Shared host build path | Exact WinUI 3／WPF host identities and framework boundaries | `001` through `005`, `012`, `013` | Host identities are separated; no cross-host renderer is inferred | Project target and Build evidence remain absent | Partially specified |
| `RND-ENABLE-002` Display／DPI | Official framework and platform scope only | `001`, `003`, `005`, `014` | Platform boundary is recorded | DPI and fractional-scale behavior remain Runtime gaps | Partially specified |
| `RND-ENABLE-003` Synthetic workload | Official text, geometry and immediate/retained rendering capabilities | `005`, `007`, `009`, `016` | Candidate capability claims are separated from workload proof | No workload has run | Partially specified |
| `RND-ENABLE-004` Evidence storage／method | Official sources cannot define repository-specific result method | None sufficient | Evidence limitation is explicit | Result storage and measurement method remain repository authorization work | Blocked |
| `RND-ENABLE-005` Candidate package／native dependency | Package identities, declared versions and native asset implications | `010`, `011`, `014` through `020` | Package/native identity baseline is recorded | Restore, architecture and deployment are unverified | Partially specified |
| `RND-ENABLE-006` Closure execution authorization | Official source boundary and non-substitutability rules | `012` through `015` | Official evidence is separated from execution authorization | Human authorization and closure evidence remain absent | Blocked |

## 15. Phase R1 Official Evidence Sufficiency

| Closure gate | Official evidence contribution | Remaining non-documentary requirement | Evidence sufficiency |
|---|---|---|---|
| `RND-CGATE-001` Source and identity baseline | Candidate names, package/API identities and source acceptance policy | None for identity scope; project identity still TBD | Sufficient from official sources |
| `RND-CGATE-002` Host framework boundary | WinUI 3 and WPF are separated; WinUI 3 is Windows App SDK delivered | Host-specific project configuration and Runtime composition | Partially sufficient |
| `RND-CGATE-003` Version baseline | Official registry／SDK version concepts are separated | Lock exact experimental version after project target is known | Partially sufficient |
| `RND-CGATE-004` Package and native dependency | Declared packages and native asset implications are listed | Restore and native load evidence | Partially sufficient |
| `RND-CGATE-005` Architecture baseline | Official x86／x64／ARM64 and Win2D architecture constraints are recorded | Project Build on approved architecture | Partially sufficient |
| `RND-CGATE-006` Synthetic workload readiness | Official API capability claims inform workload coverage | Actual workload execution and evidence | Insufficient |
| `RND-CGATE-007` Evidence method readiness | Official sources identify what they cannot prove | Reference image, capture, diff and cleanup artifacts | Insufficient |
| `RND-CGATE-008` Authorization readiness | Official evidence and execution boundaries are separated | Human authorization plus future closure record | Insufficient |

官方文件不能取代以下證據：

- 本機 Build verification。
- Package Restore evidence。
- Runtime interoperability evidence。
- Overlay、DPI、hit testing 或 export fidelity evidence。
- Native asset loading與部署結果。
- Reference image與pixel-difference calculation。

## 16. Overall Evidence Status

### 16.1 Status

| Measure | Current value |
|---|---|
| Official candidate evidence baseline | Official candidate evidence baseline partially complete |
| Sufficient to reassess execution enablement specification | Partially sufficient for reassessment |
| Local environment inspection | Not performed |
| Package cache inspection | Not performed |
| Build verification | Not performed |
| Runtime verification | Not performed |
| Closure execution authorized | No |
| Runtime spike execution authorized | No |
| Rendering decision | Not made |
| Product version | Not decided |

### 16.2 Derivation

官方來源已足以固定候選名稱、官方 API／Package identity、來源邊界、版本概念與部分 Host 宣稱；仍不足以固定本專案的 project target、native asset deployment、Runtime interoperability、DPI／overlay／text fidelity 或正式產品版本。因此不能把此文件標示為 `Official candidate evidence baseline complete`，也不能據此請求執行授權。

## 17. Traceability

```text
Official source
  -> RND-OFF-EVID
  -> Candidate identity baseline
  -> RND-PAIR-001..010
  -> RND-PREQ-001..006
  -> RND-ENABLE-001..006
  -> RND-CGATE-001..008
  -> Future enablement reassessment
  -> Future authorization request
  -> Future closure execution evidence
  -> Rendering readiness reassessment
  -> Future TD-002 decision record
```

### 17.1 Repository references

| Reference | Role |
|---|---|
| `docs/Research/Technology/10-rendering-technology-feasibility.md` | Candidate feasibility baseline |
| `docs/Research/Technology/11-rendering-technology-runtime-spike-plan.md` | Future spike design |
| `docs/Research/Technology/12-rendering-technology-runtime-spike-execution-readiness.md` | Runtime execution readiness boundary |
| `docs/Research/Technology/13-rendering-technology-prerequisite-closure-plan.md` | Prerequisite closure actions |
| `docs/Research/Technology/14-rendering-technology-prerequisite-execution-enablement-specification.md` | Enablement specification |
| `Architecture/adr/ADR-0002-ui-framework-selection.md` | Existing UI framework decision boundary; not modified |
| `Architecture/TECHNOLOGY-DECISION-ROADMAP.md` | Technology decision roadmap; not modified |

## 18. Completion Boundary

本輪完成條件如下：

- 只建立 `15-rendering-technology-official-candidate-evidence-baseline.md`。
- 不修改 README、index、CHANGELOG、TODO 或其他文件。
- 建立五個 Candidate identity records。
- 保留十個 `RND-PAIR` rows。
- 建立 Target Framework、Managed／Native Dependency 與 Experimental Version matrices。
- 每個實質官方 claim 都可追溯到 `RND-OFF-EVID`。
- 無法由官方資料證明的項目都建立 `RND-OFF-GAP`。
- 覆蓋六個 `RND-ENABLE` 與八個 `RND-CGATE`。
- 只使用官方第一方資料作為主要證據。
- 沒有執行本機盤點、Package cache 查詢、下載、安裝、Restore、Build、Run 或 Runtime Spike。
- 沒有建立 Project、Prototype、Result、Source Code 或實際 Rendering Evidence。
- 沒有修改 `ADR-0002` 或建立 TD-002 ADR。
- 沒有撰寫任何截圖功能。
- 完成後由唯讀檢查確認此文件是本輪唯一新增檔案，並確認 `git diff --check` 不產生 whitespace error。

