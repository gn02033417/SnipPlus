# ADR-0002 UI Framework Selection

## Document Control

| Field | Value |
| --- | --- |
| Document ID | ADR-0002 |
| Title | UI Framework Selection |
| Status | Draft |
| Decision Category | Framework |
| Version | 0.1 |
| Owner | TBD |
| Date proposed | 2026-07-26 |
| Date reviewed | Not reviewed |
| Date accepted | Not accepted |
| Supersedes | None |
| Superseded by | None |
| Normative References | PRD-0002、PRD-0003、PRD-0006、SPEC-0003、SPEC-0010、ARCH-0002、ARCH-0003、ARCH-0004、ARCH-0005、ARCH-BASELINE-REVIEW、ADR-BASELINE |
| Informative References | Official Microsoft WPF/WinUI/Windows App SDK documentation、Official Avalonia documentation |

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

因此，需要先決定 Desktop UI Framework，讓後續 ADR 能在相同的 UI host boundary 上討論 Rendering、Capture Backend、Clipboard Integration、Packaging 與 Testing。

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
| Strengths | Microsoft describes WinUI 3 as a modern native UI framework for Windows desktop applications, with Fluent Design, XAML, high-DPI visuals and Windows App SDK integration. |
| Risks | Windows App SDK deployment has packaged, unpackaged, framework-dependent and self-contained choices; these introduce a separate deployment decision. |
| Platform fit | Strong for a Windows desktop product that expects future Windows platform integration. |
| Cross-platform fit | Not applicable, which is acceptable because cross-platform is not a current product goal. |
| Evidence | [WinUI 3 overview](https://learn.microsoft.com/en-us/windows/apps/winui/winui3/)、[Windows app development documentation](https://learn.microsoft.com/en-us/windows/apps/)、[Windows App SDK deployment overview](https://learn.microsoft.com/en-us/windows/apps/package-and-deploy/deploy-overview) |

### Option B — WPF

| Area | Assessment |
| --- | --- |
| Alignment | Strong Windows desktop and long-term maturity alignment; weaker default alignment with modern Windows Fluent visual language. |
| Strengths | Microsoft documents WPF as a Windows-only .NET desktop framework with XAML, data binding, controls, layout, vector rendering, graphics and hardware acceleration. |
| Risks | Achieving a modern Windows Fluent-first experience would require more explicit styling and visual-system ownership; that could enlarge UI maintenance responsibility. |
| Platform fit | Strong for Windows desktop applications and mature desktop workflows. |
| Cross-platform fit | Not applicable; this is not a disadvantage for the current Windows-only scope. |
| Evidence | [WPF overview](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/overview/)、[WPF application development](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/app-development/) |

### Option C — Avalonia

| Area | Assessment |
| --- | --- |
| Alignment | Strong if cross-platform delivery is a primary goal; weaker for the current Windows-first product boundary. |
| Strengths | Avalonia documents a cross-platform .NET UI framework with its own rendering engine and shared UI approach across Windows, macOS, Linux, mobile and WebAssembly. |
| Risks | The cross-platform abstraction and independently rendered controls add a platform surface that the current PRD does not require; Windows-specific shell and capture/clipboard integration would still require explicit platform boundaries. |
| Platform fit | Viable, but optimized for a broader platform target than the current scope. Avalonia’s supported platform tiers also vary by operating-system version. |
| Cross-platform fit | Strong, but this is not a current product success criterion. |
| Evidence | [Avalonia getting started](https://docs.avaloniaui.net/docs/get-started/)、[Avalonia supported platforms](https://docs.avaloniaui.net/docs/supported-platforms)、[Avalonia cross-platform architecture](https://docs.avaloniaui.net/docs/fundamentals/cross-platform-architecture) |

### Option D — Windows Forms

| Area | Assessment |
| --- | --- |
| Alignment | Strong for rapid traditional Windows business applications; weaker for a modern Fluent-first product with custom overlay and future interaction boundaries. |
| Strengths | Mature Windows desktop option with a straightforward control-based application model. |
| Risks | The current product direction emphasizes modern Windows visual language, flexible overlay-like interaction and future platform integration; Windows Forms would require more custom ownership around those boundaries. |
| Platform fit | Windows-only and technically viable. |
| Cross-platform fit | Not applicable. |
| Evidence | [Microsoft Windows app development documentation](https://learn.microsoft.com/en-us/windows/apps/) |

## Decision

### Proposed Decision

Select **WinUI 3** as the SnipPlus Desktop UI Framework.

WinUI 3 is the best fit for the current Frozen product and Architecture baseline because:

1. It directly aligns with Windows-first and Fluent-first product principles.
2. Microsoft positions WinUI 3 as the modern native UI framework for new Windows desktop applications.
3. It provides a native Windows desktop UI boundary without adding cross-platform requirements that the product does not currently have.
4. It fits the existing Architecture separation: UI presentation remains above Feature Coordination and Domain Capability, while Windows-specific behavior remains behind Platform Integration boundaries.
5. It leaves Rendering, Capture, Clipboard, Packaging, Testing and Language/Runtime as separate decisions instead of silently coupling them to the UI framework choice.

This decision is **Draft** until ADR Review and acceptance are completed. It is a proposed decision, not an Accepted technical baseline.

### Explicit Exclusions

This ADR does not select:

- A specific Windows App SDK version.
- A specific Language or Runtime version.
- A Rendering Technology.
- A Capture Backend.
- A Clipboard API or implementation.
- A packaging or deployment mode.
- A Project Structure.
- A component or service design.

## Trade-offs

### Benefits accepted

- Direct alignment with the product’s Windows Fluent-first direction.
- A native Windows desktop presentation boundary.
- A clear host for future Windows-specific platform interaction decisions.
- Lower conceptual mismatch between the product’s target environment and its primary UI framework.
- A decision that keeps the current Feature, Module and Component boundaries intact.

### Costs accepted

- The framework choice intentionally narrows the primary target to Windows.
- Windows App SDK packaging and runtime deployment require a later decision.
- The product will not receive cross-platform UI portability from the selected framework.
- Future rendering, capture and clipboard decisions must respect the WinUI 3 host boundary.
- Runtime verification is still required; this ADR is based on product/architecture constraints and official documentation, not a running SnipPlus implementation.

### Rejected alternatives

- WPF remains a valid Windows desktop framework, but its default product alignment is weaker for the explicit Fluent-first direction.
- Avalonia remains a valid candidate when cross-platform becomes a real product requirement, but that requirement is not present in the Frozen PRD.
- Windows Forms remains a valid mature Windows option, but it does not best fit the planned modern Windows presentation and future platform interaction direction.

## Consequences

### Positive consequences

- Future UI-related decisions can use WinUI 3 as the host-framework assumption.
- TD-002 Rendering Technology, TD-003 Capture Backend, TD-004 Clipboard Integration, TD-005 Image Representation, TD-010 Packaging and TD-011 Testing Strategy can be evaluated against a declared UI framework.
- The product remains aligned with Windows-first and Fluent-first principles.
- No cross-platform abstraction is required before the product needs one.

### Negative consequences

- A future cross-platform requirement would require a new ADR and could invalidate this decision.
- Windows App SDK deployment and servicing choices remain unresolved.
- The team must verify accessibility, input, focus, display scaling, capture coordination and packaging behavior during later verification.
- A native Windows choice may make future platform portability more expensive.

### Follow-up work

The following are follow-up candidates, not implementation instructions:

- Review and accept this ADR.
- Evaluate TD-002 Rendering Technology.
- Evaluate TD-003 Capture Backend.
- Evaluate TD-004 Clipboard Integration.
- Evaluate TD-010 Packaging.
- Define runtime verification evidence for the selected framework.

## Traceability

### Product and Specification Sources

| Source | Relevance |
| --- | --- |
| PRD-0002 User Experience Principles | Windows muscle memory、Windows Fluent first、Windows over cross-platform。 |
| PRD-0003 Product Vision | Windows desktop product direction與 long-term product goals。 |
| PRD-0006 Non-functional Requirements | NFR-004 familiar Windows、NFR-006 accessibility、NFR-007 Windows Desktop、NFR-008 maintainability、NFR-010 extensibility。 |
| SPEC-0003 System Requirements | Shared workflow states and platform-neutral behavior boundaries。 |
| SPEC-0010 Feature Integration | Feature responsibility 與 downstream boundary。 |

### Architecture Sources

| Source | Relevance |
| --- | --- |
| ARCH-0002 Layer Model | Presentation/UI 必須位於既定 Layer boundary 上方，Platform Integration 保持隔離。 |
| ARCH-0003 Module Catalog | MOD-011 Platform Interaction Integration 與 Feature-to-Module ownership。 |
| ARCH-0004 Component Boundaries | COMP-017 Platform Input、COMP-018 Platform Display Context、Shared State access policy。 |
| ARCH-0005 Component Interactions | UI-related interaction 不得繞過 Component ownership；Clipboard/Output 保持平行。 |
| ARCH-BASELINE-REVIEW | Architecture v1.0 Freeze Approved；Technology Decision 可以開始，但不應改寫 Frozen Architecture。 |
| ADR-BASELINE | Required Sections、Review、Acceptance、Supersession 與 Traceability rules。 |

### Decision Roadmap Source

| Source | Relevance |
| --- | --- |
| TD-001 UI Framework | 本 ADR 對應的 Technology Decision Roadmap item。 |
| TD-002 Rendering Technology | 依賴本 ADR 的後續 Candidate。 |
| TD-003 Capture Backend | 依賴本 ADR 與 Platform Capture boundary 的後續 Candidate。 |
| TD-004 Clipboard Integration | 依賴本 ADR 與 Clipboard boundary 的後續 Candidate。 |
| TD-010 Packaging | 依賴本 ADR 的後續 Candidate。 |

### External Evidence

| Source | Evidence used |
| --- | --- |
| [Microsoft WinUI 3 overview](https://learn.microsoft.com/en-us/windows/apps/winui/winui3/) | WinUI 3 的 Windows desktop、Fluent、XAML、high-DPI 與 Windows App SDK positioning。 |
| [Microsoft Windows app development documentation](https://learn.microsoft.com/en-us/windows/apps/) | Microsoft currently positions WinUI 3 as the recommended platform for new native Windows desktop apps；同時列出 WPF、Windows Forms 與 .NET MAUI 的不同定位。 |
| [Microsoft WPF overview](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/overview/) | WPF 的 Windows-only、XAML、graphics、data binding 與成熟 desktop capability。 |
| [Microsoft Windows App SDK deployment overview](https://learn.microsoft.com/en-us/windows/apps/package-and-deploy/deploy-overview) | WinUI 3 hosting context 的 framework-dependent/self-contained deployment trade-off；本 ADR 不選部署模式。 |
| [Avalonia getting started](https://docs.avaloniaui.net/docs/get-started/) | Avalonia 的 cross-platform .NET/XAML positioning。 |
| [Avalonia supported platforms](https://docs.avaloniaui.net/docs/supported-platforms) | Avalonia 跨 Windows、macOS、Linux、mobile、WebAssembly 與 platform tier 差異。 |
| [Avalonia cross-platform architecture](https://docs.avaloniaui.net/docs/fundamentals/cross-platform-architecture) | Avalonia own rendering engine、shared UI 與 platform-specific boundary model。 |

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
| Reviewer | TBD |
| Review date | Not reviewed |
| Review result | Not reviewed |
| Open comments | None recorded |
| Resolution of comments | Not applicable |
| Acceptance authority | TBD |

## Change and Supersession

This ADR must be superseded or revisited if any of the following occurs:

- Frozen PRD changes from Windows-first to a mandatory cross-platform target.
- Frozen UX Principles remove or materially change Windows Fluent first.
- Architecture Layer Model changes the UI or Platform Integration boundary.
- WinUI 3 no longer satisfies a required product, accessibility, reliability or maintainability constraint.
- Runtime verification finds a blocking incompatibility with the frozen workflow or platform boundaries.
- A later ADR selects a different UI Framework.

If the core UI Framework decision changes:

- Create a new ADR-NNNN.
- Set the new ADR to Supersedes ADR-0002.
- Preserve this file as historical evidence.
- Do not overwrite the Decision or Consequences of this ADR.
- Re-run Architecture and Technology Decision traceability review if the change affects Layer, Module, Component or Interaction ownership.

## Acceptance Criteria

This ADR can move from Draft to Review only when:

- The comparison includes WPF, WinUI 3 and Avalonia.
- At least one additional qualified Windows desktop option is recorded or explicitly ruled out.
- Decision Drivers are traced to Frozen PRD, Specs or Architecture.
- Official evidence is linked for the key framework claims.
- The Decision is limited to UI Framework selection.
- Language/Runtime, Rendering, Capture, Clipboard, Packaging and Project Structure remain explicit exclusions.
- Trade-offs and negative consequences are recorded.
- Runtime verification limitations are not hidden.
- Review Record is completed by the designated reviewer.

This ADR can move from Review to Accepted only when:

- Review comments are resolved or explicitly accepted.
- The proposed WinUI 3 decision is approved by the acceptance authority.
- No Frozen PRD, Frozen Specs or Architecture changes are required.
- The Decision Roadmap reference TD-001 is updated through the approved documentation change flow.

## Non-goals

This ADR does not:

- Start Coding.
- Create a project.
- Create a solution.
- Create a class, interface, service or API.
- Select C# or .NET.
- Select a Windows App SDK version.
- Select a Graphics API.
- Select a Capture API.
- Select a Clipboard API.
- Select a packaging mode.
- Select a testing framework.
- Modify PRD, Specs or Architecture.

