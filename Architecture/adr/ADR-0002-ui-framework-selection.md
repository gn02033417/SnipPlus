# ADR-0002 UI Framework Selection

## Document Control

| Field | Value |
| --- | --- |
| Document ID | ADR-0002 |
| Title | UI Framework Selection |
| Status | Accepted |
| Decision Category | Framework |
| Version | 1.0 |
| Owner | Repository owner |
| Date proposed | 2026-07-26 |
| Date reviewed | 2026-07-26 |
| Date accepted | 2026-07-26 |
| Supersedes | None |
| Superseded by | None |
| Normative References | PRD-0002、PRD-0003、PRD-0006、SPEC-0003、SPEC-0010、ARCH-0002、ARCH-0003、ARCH-0004、ARCH-0005、ARCH-BASELINE-REVIEW、ADR-BASELINE |
| Informative References | Official Microsoft WPF／WinUI／Windows App SDK documentation、Official Avalonia documentation、RESEARCH-TECH-UI-001 through RESEARCH-TECH-UI-009 |

## Context

SnipPlus 是 Windows-first 的桌面產品。Frozen UX Principles 要求：

- 保留 Windows muscle memory。
- Windows Fluent first。
- Windows first over cross-platform。
- Basic workflow 不應被 advanced capability 或 framework complexity 阻擋。
- 未來功能必須能在既有 workflow boundary 內演進。

目前 Architecture baseline 已建立：

- Product Workflow Layer → Feature Coordination Layer → Domain Capability Layer → Platform Integration Layer。
- Capture、Annotation、Clipboard、Output 與 Workflow Boundary 的抽象 ownership。
- COMP-001 是唯一 Shared State Authority。
- Platform Integration 必須隔離平台互動，不能由上層 Component 直接執行平台操作。

需要先決定 Desktop UI Framework，讓後續 Rendering、Capture Backend、Clipboard Integration、Packaging 與 Testing ADR 能使用一致的 UI host boundary。

本 ADR 只決定 UI Framework。以下內容不在本 ADR 決定：

- Language 或 Runtime 版本。
- Windows App SDK 版本。
- Rendering Technology。
- Capture Backend。
- Clipboard API 或 Clipboard implementation。
- Packaging、installer、update strategy 或 deployment mode。
- Project Structure、Interface、Class、Service 或 Source code。

## Decision Drivers

| Driver | Weight | Source |
| --- | --- | --- |
| Windows-first product alignment | High | PRD-0002、PRD-0003、NFR-007 |
| Windows Fluent visual and interaction alignment | High | PRD-0002、NFR-004 |
| Native Windows desktop capability | High | PRD-0003、ARCH-0002 Platform Integration Layer |
| Maintainability over multiple years | High | PRD-0003、NFR-008、ARCH-0001 |
| Support for future platform interaction boundaries | High | ARCH-0003、ARCH-0004、ARCH-0005 |
| Basic workflow remains simple | High | PRD-0002、PRD-0004、SPEC-0005 |
| Cross-platform capability | Low | PRD-0002 explicitly prioritizes Windows over cross-platform |
| Deployment flexibility | Medium | NFR-007、TD-010 Packaging remains a separate Candidate |
| Accessibility and input support | Medium | NFR-006、ARCH-0005 Feedback/Input boundaries |

## Options Considered

### Option A — WinUI 3

| Area | Assessment |
| --- | --- |
| Alignment | Strong alignment with Windows-first and Fluent-first principles. |
| Advantages | Microsoft positions WinUI 3 as the recommended native UI framework for new Windows desktop applications; it provides Fluent controls, XAML, modern input support and Windows App SDK integration. |
| Disadvantages | Windows App SDK deployment and servicing introduce separate framework-dependent／self-contained and packaged／unpackaged decisions. |
| Constraint conflicts | None with the Frozen Windows-first product scope; runtime verification is still required. |
| Evidence status | Official Microsoft documentation reviewed on 2026-07-26. |
| Evidence | [WinUI 3 overview](https://learn.microsoft.com/en-us/windows/apps/winui/winui3/)、[Windows app development documentation](https://learn.microsoft.com/en-us/windows/apps/)、[Windows App SDK deployment overview](https://learn.microsoft.com/en-us/windows/apps/package-and-deploy/deploy-overview) |

### Option B — WPF

| Area | Assessment |
| --- | --- |
| Alignment | Strong Windows desktop and long-term maturity alignment; weaker default alignment with the explicit modern Fluent-first direction. |
| Advantages | Mature Windows-only .NET framework with XAML, controls, data binding, layout, vector rendering, graphics, animation, styles and templates. |
| Disadvantages | A Fluent-first product would require more explicit styling and visual-system ownership. |
| Constraint conflicts | No direct conflict, but greater custom UI ownership would work against the current maintainability and Fluent-first drivers. |
| Evidence status | Official Microsoft documentation reviewed on 2026-07-26. |
| Evidence | [WPF overview](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/overview/)、[WPF application development](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/app-development/) |

### Option C — Avalonia

| Area | Assessment |
| --- | --- |
| Alignment | Strong when cross-platform delivery is a primary goal; weaker for the current Windows-first boundary. |
| Advantages | Cross-platform .NET/XAML approach covering Windows, macOS, Linux, mobile and WebAssembly. |
| Disadvantages | Adds a cross-platform abstraction and independently rendered control surface that the current Frozen PRD does not require. |
| Constraint conflicts | Optimizes for a product requirement that is currently absent; Windows-specific shell, capture and clipboard boundaries still require explicit platform integration. |
| Evidence status | Official Avalonia documentation reviewed on 2026-07-26. |
| Evidence | [Avalonia getting started](https://docs.avaloniaui.net/docs/get-started/)、[Avalonia supported platforms](https://docs.avaloniaui.net/docs/supported-platforms)、[Avalonia cross-platform architecture](https://docs.avaloniaui.net/docs/fundamentals/cross-platform-architecture) |

### Option D — Windows Forms

| Area | Assessment |
| --- | --- |
| Alignment | Strong for traditional Windows business applications; weaker for a modern Fluent-first product with overlay-like interaction and future platform boundaries. |
| Advantages | Mature, straightforward Windows desktop application model. |
| Disadvantages | Requires more custom ownership for modern visual language, adaptive interaction and overlay-oriented behavior. |
| Constraint conflicts | Technically viable, but weaker against the current Fluent-first and future platform-integration drivers. |
| Evidence status | Qualified Windows desktop alternative retained for comparison. |
| Evidence | [Microsoft Windows app development documentation](https://learn.microsoft.com/en-us/windows/apps/) |

## Decision

### Accepted Decision

Select **WinUI 3** as the SnipPlus Desktop UI Framework.

WinUI 3 is accepted because:

1. It directly aligns with the Frozen Windows-first and Fluent-first product principles.
2. Microsoft currently positions WinUI 3 as the recommended native UI framework for new Windows desktop applications.
3. It provides a native Windows desktop UI host without adding a cross-platform product requirement.
4. It fits the Frozen Architecture separation: presentation remains above Feature Coordination and Domain Capability, while Windows-specific behavior remains behind Platform Integration boundaries.
5. It leaves Rendering, Capture, Clipboard, Packaging, Testing, Language and Runtime as separate decisions instead of silently coupling them to the framework choice.

### Scope of Applicability

This decision applies to the primary SnipPlus Windows desktop presentation host and future UI composition that must conform to the Frozen Feature、Module、Component and Interaction boundaries.

### Explicit Exclusions

This ADR does not select:

- A specific Windows App SDK version.
- A specific Language or Runtime version.
- A Rendering Technology.
- A Capture Backend.
- A Clipboard API or implementation.
- A packaging or deployment mode.
- A Project Structure.
- A component, interface, service or source-code design.

## Trade-offs

### Benefits accepted

- Direct alignment with the product’s Windows Fluent-first direction.
- A native Windows desktop presentation boundary.
- A clear host for future Windows-specific platform interaction decisions.
- Lower conceptual mismatch between the target environment and primary UI framework.
- Existing Feature、Module and Component ownership remains unchanged.

### Costs accepted

- The primary target is intentionally narrowed to Windows.
- Windows App SDK packaging, runtime deployment and servicing require later decisions.
- The product does not receive cross-platform UI portability from the selected framework.
- Future rendering, capture and clipboard decisions must operate within the WinUI 3 host boundary.
- Runtime verification remains required; this decision is based on Frozen product／architecture constraints and official evidence, not a running SnipPlus implementation.

### Rejected alternatives

- WPF remains a valid Windows desktop framework, but its default product alignment is weaker for the explicit Fluent-first direction.
- Avalonia remains valid if cross-platform becomes a real product requirement, but that requirement is not present in the Frozen PRD.
- Windows Forms remains a mature option, but it is not the best fit for the planned modern Windows presentation and future platform-interaction direction.

## Consequences

### Positive consequences

- Future UI-related decisions can use WinUI 3 as the accepted host-framework assumption.
- TD-002 Rendering Technology、TD-003 Capture Backend、TD-004 Clipboard Integration、TD-005 Image Representation、TD-010 Packaging and TD-011 Testing Strategy can be evaluated against a declared UI framework.
- The product remains aligned with Windows-first and Fluent-first principles.
- No cross-platform abstraction is required before the product actually needs one.

### Negative consequences

- A future mandatory cross-platform requirement requires a new ADR and may supersede this decision.
- Windows App SDK deployment and servicing choices remain unresolved.
- Accessibility、input、focus、display scaling、capture coordination and packaging behavior still require later verification.
- A native Windows choice may make future platform portability more expensive.

### Neutral consequences

- C#／C++、.NET／Runtime version、Windows App SDK version and packaging mode remain separate decisions.
- Rendering、Capture Backend、Clipboard Integration、Image Representation and Testing Strategy remain unresolved.
- This ADR does not create a Solution、Project、Interface、Class、Service or source file.
- Implementation and runtime evidence remain absent until separately authorized work occurs.

### Follow-up work

The following are decision and verification follow-ups, not coding instructions:

- Evaluate TD-002 Rendering Technology.
- Evaluate TD-003 Capture Backend.
- Evaluate TD-004 Clipboard Integration.
- Evaluate TD-005 Image Representation.
- Evaluate TD-011 Testing Strategy.
- Define runtime verification evidence for the accepted framework.
- Decide Windows App SDK version、Language／Runtime and Packaging through their own approved decision flow.

## Traceability

### Product and Specification Sources

| Source | Relevance |
| --- | --- |
| PRD-0002 User Experience Principles | Windows muscle memory、Windows Fluent first、Windows over cross-platform。 |
| PRD-0003 Product Vision | Windows desktop product direction and long-term product goals. |
| PRD-0006 Non-functional Requirements | NFR-004 familiar Windows、NFR-006 accessibility、NFR-007 Windows Desktop、NFR-008 maintainability、NFR-010 extensibility。 |
| SPEC-0003 System Requirements | Shared workflow states and platform-neutral behavior boundaries. |
| SPEC-0010 Feature Integration | Feature responsibility and downstream boundary. |

### Architecture Sources

| Source | Relevance |
| --- | --- |
| ARCH-0002 Layer Model | Presentation/UI remains above Platform Integration. |
| ARCH-0003 Module Catalog | MOD-011 Platform Interaction Integration and Feature-to-Module ownership. |
| ARCH-0004 Component Boundaries | COMP-017 Platform Input、COMP-018 Platform Display Context and Shared State access policy. |
| ARCH-0005 Component Interactions | UI interaction cannot bypass Component ownership; Clipboard and Output remain parallel. |
| ARCH-BASELINE-REVIEW | Architecture v1.0 Freeze Approved; technology decisions may proceed without rewriting ownership. |
| ADR-BASELINE | Required sections、review、acceptance、supersession and traceability rules. |

### Decision Roadmap Source

| Source | Relevance |
| --- | --- |
| TD-001 UI Framework | Decision completed by this ADR. |
| TD-002 Rendering Technology | Uses ADR-0002 as an accepted upstream dependency. |
| TD-003 Capture Backend | Uses ADR-0002 and Platform Capture boundaries as upstream context. |
| TD-004 Clipboard Integration | Uses ADR-0002 and Clipboard boundaries as upstream context. |
| TD-005 Image Representation | Must remain independent of the UI framework where Architecture requires. |
| TD-010 Packaging | Uses the accepted host framework but remains a separate decision. |
| TD-011 Testing Strategy | Must verify the accepted framework and platform boundaries. |

### External Evidence

| Source | Evidence used |
| --- | --- |
| [Microsoft WinUI 3 overview](https://learn.microsoft.com/en-us/windows/apps/winui/winui3/) | Recommended native UI framework for new Windows desktop applications; Fluent、XAML、high-DPI and Windows App SDK positioning. |
| [Microsoft Windows app development documentation](https://learn.microsoft.com/en-us/windows/apps/) | WinUI 3 with Windows App SDK is the recommended platform for new native Windows apps. |
| [Microsoft WPF overview](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/overview/) | WPF remains a Windows-only .NET desktop framework with extensive mature capabilities. |
| [Microsoft Windows App SDK deployment overview](https://learn.microsoft.com/en-us/windows/apps/package-and-deploy/deploy-overview) | Deployment choices remain a separate decision. |
| [Avalonia supported platforms](https://docs.avaloniaui.net/docs/supported-platforms) | Avalonia targets multiple platforms with tiered support. |

### Implementation and Verification

| Artifact | Status |
| --- | --- |
| Implementation reference | Not implemented |
| Runtime verification evidence | Not verified |
| Packaging evidence | Not verified |
| Accessibility evidence | Not verified |

## Review Record

| Field | Value |
| --- | --- |
| Reviewer | ChatGPT repository review |
| Review date | 2026-07-26 |
| Review result | Approved |
| Review basis | Frozen PRD／Specs／Architecture、ADR-BASELINE、RESEARCH-TECH-UI-001 through 009、official Microsoft and Avalonia documentation rechecked on 2026-07-26 |
| Open comments | None |
| Resolution of comments | Added explicit neutral consequences, clarified accepted scope and retained all implementation／runtime exclusions |
| Acceptance authority | Repository owner through explicit instruction to proceed with the next repository step |

Review findings:

- The ADR handles one major decision only.
- Context and Decision Drivers are traceable to Frozen sources.
- WinUI 3、WPF、Avalonia and Windows Forms are retained as qualified alternatives.
- Trade-offs include benefits、costs、rejected alternatives and negative consequences.
- The accepted decision does not require changes to Frozen PRD、Specs or Architecture ownership.
- Runtime verification limitations remain explicit.
- No product feature、Project Structure、API or source code is introduced.

## Change and Supersession

This ADR must be superseded or revisited if any of the following occurs:

- Frozen PRD changes from Windows-first to a mandatory cross-platform target.
- Frozen UX Principles remove or materially change Windows Fluent first.
- Architecture Layer Model changes the UI or Platform Integration boundary.
- WinUI 3 no longer satisfies a required product、accessibility、reliability or maintainability constraint.
- Runtime verification finds a blocking incompatibility with the Frozen workflow or platform boundaries.
- A later ADR selects a different UI Framework.

If the core UI Framework decision changes:

- Create a new ADR-NNNN.
- Set the new ADR to `Supersedes ADR-0002`.
- Change this ADR to `Superseded` and link the new ADR.
- Preserve this file as historical evidence.
- Do not overwrite the original Decision or Consequences.
- Re-run Architecture and Technology Decision traceability review if Layer、Module、Component or Interaction ownership may be affected.

## Acceptance Verification

| Acceptance check | Result |
| --- | --- |
| Unique ADR ID and correct location | PASS |
| Required sections present | PASS |
| Single major decision | PASS |
| Frozen-source traceability | PASS |
| Official evidence linked and rechecked | PASS |
| Reasonable alternatives retained | PASS |
| Trade-offs and negative consequences recorded | PASS |
| Neutral consequences recorded | PASS |
| Runtime limitations retained | PASS |
| Review Record completed | PASS |
| Acceptance authority recorded | PASS |
| Frozen PRD／Specs／Architecture changes required | No |
| TD-001 roadmap update required | Yes; update in the same accepted documentation change flow |
| Coding authorized | No |

## Non-goals

This ADR does not:

- Start Coding.
- Create a project or solution.
- Create a class、interface、service or API.
- Select C# or .NET.
- Select a Windows App SDK version.
- Select a Graphics API.
- Select a Capture API.
- Select a Clipboard API.
- Select a packaging mode.
- Select a testing framework.
- Modify Frozen PRD、Specs or Architecture.
