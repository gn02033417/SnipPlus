# UI Framework Feasibility for SnipPlus

狀態：`Draft`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | RESEARCH-TECH-UI-001 |
| Title | UI Framework Feasibility for SnipPlus |
| Status | Draft |
| Research Type | Technology Feasibility |
| Runtime Verification | Not performed |
| Owner | TBD |
| Research date | 2026-07-26 |
| Windows target | Windows 11 desktop; multi-monitor; mixed-DPI conditions; mouse, keyboard and pen-capable input are in scope |
| Source review boundary | Official Microsoft and framework documentation review |
| Last reviewed | Not reviewed |
| Version | 0.1 |
| Normative References | `Architecture/adr/ADR-0002-ui-framework-selection.md`, `Architecture/ADR-BASELINE.md`, `docs/Research/template.md` |
| Informative References | Official framework and platform documentation listed in [Sources](#6-sources) |

## 2. Purpose

This document supplies SnipPlus-specific feasibility evidence for `ADR-0002`.

It evaluates whether WinUI 3 and WPF have documented capabilities relevant to a screenshot product's borderless overlay, window management, DPI and coordinate handling, pointer and keyboard input, drawing and hit testing, and deployment constraints. It records evidence and gaps; it does not turn those observations into a final framework decision.

## 3. Scope

### Primary comparison

- WinUI 3 with the Windows App SDK.
- WPF on .NET.

### Secondary reference

- Avalonia, only for its platform and rendering model.
- Windows Forms, only for its documented Windows desktop and Windows App SDK interop positioning.

### SnipPlus-specific conditions

- A capture surface may need to cover one or more displays.
- The surface may need to be borderless, topmost, partially transparent and visually aligned with the captured desktop.
- A user may drag a selection with a mouse, touch-capable pointer or pen.
- Pointer capture, cancellation, focus acquisition and focus return are relevant to the workflow.
- Selection and optional annotation require accurate coordinates, drawing and hit testing.
- The basic capture path must remain lightweight and maintainable over several years.
- Distribution must be evaluated as part of the framework trade-off, but no packaging mode is selected here.

## 4. Non-goals

This research does not:

- Make the final Desktop UI Framework decision.
- Modify `ADR-0002` or change its status.
- Select a Windows App SDK version, language, runtime or target framework.
- Select a rendering backend, capture backend or Clipboard API.
- Define project structure, classes, interfaces, services, events or source code.
- Build a prototype or execute runtime, performance, accessibility or deployment tests.
- Modify the frozen PRD, Specs or Architecture documents.
- Start screenshot-function coding.

## 5. Method and Evidence Vocabulary

The research method is an official-documentation review performed on 2026-07-26. Each observation is separated into three levels:

| Label | Meaning |
| --- | --- |
| Documented capability | The cited source explicitly describes the capability or platform behavior. |
| Product inference | A bounded interpretation of documented capability in relation to a SnipPlus condition. It is not a requirement or decision. |
| Unknown | The sources do not establish the behavior, or it requires runtime verification. |

No claim in this document is based on a running SnipPlus application. No build, prototype or runtime test was performed.

### Required Evidence Status Vocabulary

Comparison-matrix and gate-status cells use only these values:

- `Confirmed by official documentation`
- `Partially supported`
- `Requires runtime prototype`
- `Unknown`
- `Not aligned`

## 6. Sources

| Source | Type | Published / updated | Accessed | Version | URL |
| --- | --- | --- | --- | --- | --- |
| WinUI 3 | Official Microsoft Learn | 2026-07-17 | 2026-07-26 | Windows App SDK version not selected | <https://learn.microsoft.com/en-us/windows/apps/winui/winui3/> |
| Windows app development documentation | Official Microsoft Learn | Not recorded on source page | 2026-07-26 | Current documentation set | <https://learn.microsoft.com/en-us/windows/apps/> |
| Manage app windows | Official Microsoft Learn | Not recorded on source page | 2026-07-26 | Windows App SDK version not selected | <https://learn.microsoft.com/en-us/windows/apps/develop/ui/manage-app-windows> |
| Handle pointer input | Official Microsoft Learn | Not recorded on source page | 2026-07-26 | Windows App SDK version not selected | <https://learn.microsoft.com/en-us/windows/apps/develop/input/handle-pointer-input> |
| User interface migration, including WinUI | Official Microsoft Learn | Not recorded on source page | 2026-07-26 | Windows App SDK version not selected | <https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/migrate-to-windows-app-sdk/guides/winui3> |
| Windows App SDK deployment overview | Official Microsoft Learn | Not recorded on source page | 2026-07-26 | Windows App SDK version not selected | <https://learn.microsoft.com/en-us/windows/apps/package-and-deploy/deploy-overview> |
| Windows App SDK deployment for unpackaged apps | Official Microsoft Learn | Not recorded on source page | 2026-07-26 | Windows App SDK version not selected | <https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/deploy-unpackaged-apps> |
| Windows Presentation Foundation overview | Official Microsoft Learn | Not recorded on source page | 2026-07-26 | .NET version not selected | <https://learn.microsoft.com/en-us/dotnet/desktop/wpf/overview/> |
| Windows in WPF overview | Official Microsoft Learn | Not recorded on source page | 2026-07-26 | .NET version not selected | <https://learn.microsoft.com/en-us/dotnet/desktop/wpf/windows/> |
| WPF input overview | Official Microsoft Learn | 2025-05-07 | 2026-07-26 | .NET version not selected | <https://learn.microsoft.com/en-us/dotnet/desktop/wpf/advanced/input-overview> |
| WPF graphics rendering overview | Official Microsoft Learn | Not recorded on source page | 2026-07-26 | .NET version not selected | <https://learn.microsoft.com/en-us/dotnet/desktop/wpf/graphics-multimedia/wpf-graphics-rendering-overview> |
| WPF hit testing in the visual layer | Official Microsoft Learn | Not recorded on source page | 2026-07-26 | .NET version not selected | <https://learn.microsoft.com/en-us/dotnet/desktop/wpf/graphics-multimedia/hit-testing-in-the-visual-layer> |
| Deploy a WPF application | Official Microsoft Learn | 2025-05-07 | 2026-07-26 | .NET version not selected | <https://learn.microsoft.com/en-us/dotnet/desktop/wpf/app-development/deploying-a-wpf-application-wpf> |
| Avalonia getting started | Official Avalonia documentation | Not recorded on source page | 2026-07-26 | Avalonia version not selected | <https://docs.avaloniaui.net/docs/get-started/> |
| Avalonia supported platforms | Official Avalonia documentation | Not recorded on source page | 2026-07-26 | Avalonia version not selected | <https://docs.avaloniaui.net/docs/supported-platforms> |
| Avalonia cross-platform architecture | Official Avalonia documentation | Not recorded on source page | 2026-07-26 | Avalonia version not selected | <https://docs.avaloniaui.net/docs/fundamentals/cross-platform-architecture> |
| Windows Forms overview | Official Microsoft Learn | Not recorded on source page | 2026-07-26 | .NET version not selected | <https://learn.microsoft.com/en-us/dotnet/desktop/winforms/overview/> |

## 7. Evaluation Criteria

| Criterion ID | Criterion | Evidence needed for a future decision |
| --- | --- | --- |
| UIF-001 | Borderless topmost capture overlay | Window style, topmost behavior and overlay ownership must be feasible together. |
| UIF-002 | Transparent or semi-transparent window composition | The framework must document a viable composition boundary for the desktop surface. |
| UIF-003 | Per-pixel alpha requirement | The required alpha behavior must be documented or independently verified. |
| UIF-004 | Multi-monitor placement | Window placement and coverage must remain correct across displays. |
| UIF-005 | Per-monitor DPI awareness | Scale changes must preserve selection and visual-coordinate fidelity. |
| UIF-006 | Focus acquisition and restoration | Capture must acquire and return focus without an undefined workflow boundary. |
| UIF-007 | Global capture-entry interoperability | The framework must coexist with the documented Windows capture-entry boundary. |
| UIF-008 | Pointer and keyboard input handling | Press, move, release, capture, cancellation, keyboard commands and pen-relevant input must be representable. |
| UIF-009 | Low-latency overlay activation | Capture entry must have a measurable path to an interactive overlay. |
| UIF-010 | Custom drawing and annotation suitability | Selection geometry and optional annotation need a maintainable drawing model. |
| UIF-011 | Hit testing and editable visual objects | Selection and annotation objects need predictable hit testing and editing boundaries. |
| UIF-012 | Accessibility | The framework must provide a credible accessibility baseline for controls and input. |
| UIF-013 | Fluent visual alignment | The framework must align with the frozen Windows Fluent-first principle at acceptable cost. |
| UIF-014 | HWND and Win32 interoperability | Platform integration must remain possible without bypassing ownership boundaries. |
| UIF-015 | Deployment complexity | Dependencies, footprint, installation, update and servicing implications must be understood. |
| UIF-016 | x64 and ARM64 distribution | Required processor architectures must have a documented distribution path. |
| UIF-017 | Testability | The framework must permit isolated verification of workflow, rendering and platform boundaries. |
| UIF-018 | Long-term maintenance | The ecosystem, documentation and ownership model must support multi-year maintenance. |

## 8. WinUI 3 Evidence

### 8.1 Overlay and windowing

Microsoft documents `AppWindow` as a high-level abstraction of a top-level `HWND`, with a one-to-one mapping. The Windows App SDK windowing APIs can be used with WinUI, WPF, Windows Forms and Win32, so the existence of `AppWindow` is not by itself a WinUI-exclusive advantage.

The documented `OverlappedPresenter` can set `IsAlwaysOnTop`, and the documented `FullScreenPresenter` provides a borderless, title-bar-free full-screen presentation. These facts establish useful window primitives for a capture workflow.

The sources do not establish that the following combination is already suitable for SnipPlus without further verification:

- Borderless full-screen coverage across all target displays.
- A translucent or partially transparent desktop-composition surface.
- Exact click-through or input-routing behavior outside the selected region.
- Focus acquisition and focus return after capture cancellation or completion.

**Finding:** `UIF-001` and `UIF-002` are `Partially supported` from documentation evidence. Windowing primitives are documented; the complete capture overlay is not proven.

### 8.2 DPI and coordinates

The Windows App SDK migration guidance states that WinUI 3 desktop applications should use `XamlRoot.RasterizationScale` and respond to `XamlRoot.Changed` for DPI changes. It separately identifies Win32 APIs for raw screen-resolution work.

This is sufficient evidence that a WinUI 3 application has a documented DPI observation path. It does not prove that a future selection rectangle, captured image, pointer coordinate and drawn annotation will remain aligned across mixed-DPI monitors without product-specific coordinate rules and runtime testing.

**Finding:** `UIF-004` is `Partially supported`. The scale signal is documented; SnipPlus coordinate fidelity is not verified.

### 8.3 Input, capture and cancellation

Microsoft documents pointer events including pressed, moved, released, canceled and capture-lost behavior for WinUI 3 desktop applications. Pointer capture can constrain input to a UI element while the pointer moves outside its bounds. The same documentation records that pointer cancellation can occur when the display configuration changes, the desktop is locked or the user logs off.

This maps well to a drag-selection state machine, but it also identifies an explicit edge case that the product workflow must own. The documentation does not define SnipPlus-specific keyboard cancellation, focus return or global shortcut behavior.

**Finding:** `UIF-005` is `Partially supported` for framework input primitives and for the complete SnipPlus workflow.

### 8.4 Drawing and hit testing

WinUI 3 is documented as a XAML-based framework with layout and high-DPI visual support. The Windows UI documentation lists `Canvas` among the available layout panels. These facts establish a presentation and layout surface.

The sources reviewed here do not prove the exact retained drawing, geometry editing, annotation hit-testing, or full-screen overlay composition model required by SnipPlus. This research therefore does not select a rendering technology and does not infer one from the UI framework.

**Finding:** `UIF-006` is `Partially supported`; custom drawing and annotation feasibility remain an evidence gap.

### 8.5 Deployment and servicing

The Windows App SDK documentation explicitly distinguishes framework-dependent and self-contained deployment. It also distinguishes packaged, packaged-with-external-location and unpackaged applications. Framework-dependent deployment can reduce the distributed application footprint and receive shared servicing, but it adds runtime dependencies. Self-contained deployment increases the application footprint and places servicing responsibility on the application release.

The unpackaged deployment guidance adds runtime requirements for loading the Windows App SDK framework package and using the Bootstrapper API. These are real deployment concerns for a lightweight desktop utility, but the product has not yet defined its distribution, update or offline goals.

**Finding:** `UIF-007` is `Partially supported`. WinUI 3 deployment choices are documented, but SnipPlus has not selected a deployment target or evaluated its acceptable footprint.

### 8.6 Performance boundary

WinUI 3 documentation uses terms such as high-performance rendering and smooth animations, but those statements are framework positioning, not SnipPlus measurements. No latency, startup, memory, pointer sampling or capture timing evidence was collected.

**Finding:** `UIF-008` is `Requires runtime prototype` until a later, authorized verification stage.

### 8.7 Criteria Coverage

| Criterion | Status | Evidence boundary |
| --- | --- | --- |
| UIF-001 | Partially supported | `AppWindow`, full-screen and topmost primitives are documented; the complete capture overlay is not. |
| UIF-002 | Unknown | Current sources do not establish a transparent full-window desktop composition surface. |
| UIF-003 | Unknown | Per-pixel alpha behavior for the required overlay is not established. |
| UIF-004 | Partially supported | Window size and position are documented; multi-display coverage is not verified. |
| UIF-005 | Partially supported | `XamlRoot.RasterizationScale` and change notification are documented; coordinate fidelity is not verified. |
| UIF-006 | Partially supported | Window and pointer primitives exist; focus return is not a SnipPlus-proven behavior. |
| UIF-007 | Unknown | The reviewed sources do not define global capture-entry interoperability. |
| UIF-008 | Confirmed by official documentation | Pointer and keyboard input are documented; complete workflow ownership remains open. |
| UIF-009 | Requires runtime prototype | No framework documentation supplies SnipPlus-specific latency evidence. |
| UIF-010 | Partially supported | XAML layout and visual surfaces exist; custom annotation suitability is not proven. |
| UIF-011 | Unknown | The reviewed WinUI sources do not establish editable visual-object hit testing for this workflow. |
| UIF-012 | Partially supported | Platform controls and input guidance provide a baseline; overlay accessibility is not verified. |
| UIF-013 | Confirmed by official documentation | Microsoft positions WinUI 3 with Fluent Design and modern Windows UI. |
| UIF-014 | Confirmed by official documentation | Windows App SDK windowing is based on the HWND model. |
| UIF-015 | Partially supported | Packaged, unpackaged, framework-dependent and self-contained options are documented. |
| UIF-016 | Partially supported | Windows App SDK deployment documentation describes x64 and ARM64 distribution concerns. |
| UIF-017 | Unknown | No SnipPlus test harness or isolation evidence exists. |
| UIF-018 | Partially supported | Official documentation and active platform guidance exist; product maintenance cost is not measured. |

## 9. WPF Evidence

### 9.1 Overlay and windowing

WPF documents window location, size and z-order. A window can be placed in the topmost z-order through its `Topmost` property.

WPF also documents a non-rectangular transparent-window pattern using `WindowStyle=None`, `AllowsTransparency=True` and `Background=Transparent`. The same documentation warns that the normal non-client buttons are not available in this state and must be supplied by the application if needed.

This is more direct documentation for a transparent, borderless window than the WinUI-specific sources reviewed here. It still does not prove a SnipPlus-ready full-screen, multi-monitor capture overlay, nor does it settle focus and input behavior across the desktop.

**Finding:** `UIF-001` is `Partially supported`, with stronger direct transparency evidence than the current WinUI evidence. `UIF-002` is also `Partially supported` because multi-monitor coverage remains unverified.

### 9.2 DPI and coordinates

WPF is documented as resolution-independent and based on a vector rendering engine. The framework therefore provides a strong conceptual basis for device-independent layout and graphics.

That property does not automatically prove correct per-monitor behavior for a capture overlay. Per-monitor changes, monitor transitions, physical-to-logical conversion and selection-image alignment still require explicit product rules and verification.

**Finding:** `UIF-004` is `Partially supported`. WPF has strong resolution-independent rendering evidence, but SnipPlus mixed-DPI behavior is not verified.

### 9.3 Input, focus and hit testing

WPF documents input support for mouse, keyboard, touch and stylus. Its input model includes routed events, focus management and mouse capture. WPF also documents that keyboard focus is unique across the desktop and that pointer coordinates are interpreted relative to a chosen element.

The WPF visual layer documents hit testing against visual objects, including transparent objects, and supports returning results in z-order. These are directly relevant to selection boundaries and future annotation interactions.

The documentation still does not define the exact SnipPlus focus handoff, global hotkey behavior or cancellation policy.

**Finding:** `UIF-005` is `Partially supported` for framework input primitives. `UIF-006` has stronger direct evidence in WPF than in the current WinUI 3 source set, but remains `Partially supported` for the full product workflow.

### 9.4 Drawing and visual composition

WPF documents a retained visual layer, a visual tree that determines rendering order, vector graphics instructions and `DrawingVisual` as a lightweight drawing class. It also documents geometry and hit testing in the visual layer.

These sources establish a coherent framework-level model for drawing and hit testing. They do not prove the frame rate, memory behavior, capture-image composition or annotation editing experience that SnipPlus would require.

**Finding:** `UIF-006` is `Partially supported`, with a stronger documented drawing and hit-testing foundation than the current WinUI 3 evidence.

### 9.5 Deployment and servicing

Microsoft documents WPF deployment options including XCopy, Windows Installer and ClickOnce. The documentation records different trade-offs: XCopy is simple but does not provide versioning, uninstallation or rollback; Windows Installer integrates with the desktop but does not itself provide application updating; ClickOnce provides versioning, rollback, uninstallation and automatic updates within its supported model.

These are documented options, not a SnipPlus deployment recommendation. The required .NET/runtime and packaging assumptions for the future product remain unselected.

**Finding:** `UIF-007` is `Partially supported`. WPF presents familiar deployment paths, but the product has not defined the footprint, update and offline constraints needed for a comparison.

### 9.6 Performance boundary

WPF documentation describes hardware acceleration and a retained rendering model, but no SnipPlus-specific latency or resource evidence was collected.

**Finding:** `UIF-008` is `Requires runtime prototype` until a later, authorized verification stage.

### 9.7 Criteria Coverage

| Criterion | Status | Evidence boundary |
| --- | --- | --- |
| UIF-001 | Partially supported | Topmost and transparent borderless window patterns are documented; the complete capture overlay is not. |
| UIF-002 | Partially supported | `AllowsTransparency` and transparent background are documented for a window; desktop composition is not verified. |
| UIF-003 | Partially supported | Alpha-composited transparent-window behavior is directly documented; required per-pixel behavior is not verified. |
| UIF-004 | Partially supported | Window location and size are documented; multi-display coverage is not verified. |
| UIF-005 | Partially supported | Resolution-independent/vector rendering is documented; mixed-DPI selection alignment is not verified. |
| UIF-006 | Partially supported | Focus, keyboard focus and mouse capture are documented; SnipPlus focus return is not proven. |
| UIF-007 | Unknown | The reviewed WPF sources do not define global capture-entry interoperability. |
| UIF-008 | Confirmed by official documentation | Mouse, keyboard, touch and stylus input are documented; complete workflow ownership remains open. |
| UIF-009 | Requires runtime prototype | No framework documentation supplies SnipPlus-specific latency evidence. |
| UIF-010 | Partially supported | Retained visuals, vector drawing and `DrawingVisual` are documented; annotation suitability is not proven. |
| UIF-011 | Confirmed by official documentation | Visual-layer hit testing, z-order results and transparent-object hit testing are documented. |
| UIF-012 | Partially supported | Input and focus services are documented; overlay accessibility is not verified. |
| UIF-013 | Partially supported | WPF supports styling and graphics; Fluent-first alignment would require product-specific evaluation. |
| UIF-014 | Confirmed by official documentation | Windows App SDK windowing documentation includes WPF interoperability. |
| UIF-015 | Partially supported | XCopy, Windows Installer and ClickOnce options are documented. |
| UIF-016 | Unknown | The reviewed WPF sources do not establish the required x64/ARM64 distribution path. |
| UIF-017 | Unknown | No SnipPlus test harness or isolation evidence exists. |
| UIF-018 | Partially supported | WPF has mature documentation and active .NET support; product maintenance cost is not measured. |

## 10. Secondary Reference Findings

### Avalonia

Avalonia documents a cross-platform .NET/XAML approach with its own rendering engine and platform-specific handling. Its supported-platform documentation describes platform tiers across Windows, macOS, Linux, mobile and WebAssembly.

This makes Avalonia relevant if cross-platform delivery becomes a product requirement. It also means that the framework introduces a broader platform surface than the current Windows-first PRD requires. This research did not perform the SnipPlus-specific overlay, DPI, input, drawing or deployment comparison at the same depth as WinUI 3 and WPF.

**Finding:** Avalonia remains a qualified alternative for a future cross-platform decision, not a ruled-out option and not a current recommendation.

### Windows Forms

Microsoft documents Windows Forms as a Windows desktop UI framework with controls, graphics, data binding and user input. Microsoft also documents that Windows App SDK `AppWindow` APIs can be used alongside Windows Forms windows.

This establishes Windows Forms as a technically qualified Windows desktop option. The reviewed sources do not provide enough SnipPlus-specific evidence for a deep comparison of transparent overlays, custom drawing and mixed-DPI selection behavior.

**Finding:** Windows Forms remains a qualified reference alternative, but its special-overlay feasibility is `Unknown` in this research depth.

## 11. Comparison Matrix

The matrix describes evidence maturity, not a final framework score. Every cell uses only the required Evidence Status Vocabulary.

| Criterion | WinUI 3 | WPF | Avalonia | Windows Forms | Evidence status |
| --- | --- | --- | --- | --- | --- |
| `UIF-001` Borderless topmost capture overlay | Partially supported | Partially supported | Requires runtime prototype | Unknown | Partially supported |
| `UIF-002` Transparent or semi-transparent composition | Unknown | Partially supported | Unknown | Unknown | Partially supported |
| `UIF-003` Per-pixel alpha requirement | Unknown | Partially supported | Unknown | Unknown | Unknown |
| `UIF-004` Multi-monitor placement | Partially supported | Partially supported | Requires runtime prototype | Requires runtime prototype | Partially supported |
| `UIF-005` Per-monitor DPI awareness | Partially supported | Partially supported | Requires runtime prototype | Requires runtime prototype | Partially supported |
| `UIF-006` Focus acquisition and restoration | Partially supported | Partially supported | Unknown | Unknown | Partially supported |
| `UIF-007` Global capture-entry interoperability | Unknown | Unknown | Unknown | Unknown | Unknown |
| `UIF-008` Pointer and keyboard input handling | Confirmed by official documentation | Confirmed by official documentation | Partially supported | Partially supported | Partially supported |
| `UIF-009` Low-latency overlay activation | Requires runtime prototype | Requires runtime prototype | Requires runtime prototype | Requires runtime prototype | Requires runtime prototype |
| `UIF-010` Custom drawing and annotation suitability | Partially supported | Partially supported | Partially supported | Partially supported | Partially supported |
| `UIF-011` Hit testing and editable visual objects | Unknown | Confirmed by official documentation | Unknown | Unknown | Partially supported |
| `UIF-012` Accessibility | Partially supported | Partially supported | Unknown | Partially supported | Partially supported |
| `UIF-013` Fluent visual alignment | Confirmed by official documentation | Partially supported | Unknown | Partially supported | Partially supported |
| `UIF-014` HWND and Win32 interoperability | Confirmed by official documentation | Confirmed by official documentation | Partially supported | Confirmed by official documentation | Confirmed by official documentation |
| `UIF-015` Deployment complexity | Partially supported | Partially supported | Unknown | Partially supported | Partially supported |
| `UIF-016` x64 and ARM64 distribution | Partially supported | Unknown | Unknown | Unknown | Partially supported |
| `UIF-017` Testability | Unknown | Unknown | Unknown | Unknown | Unknown |
| `UIF-018` Long-term maintenance | Partially supported | Partially supported | Unknown | Partially supported | Partially supported |

## 12. Critical Feasibility Gates

These gates identify evidence that must exist before `ADR-0002` can move toward acceptance. Current status is not a framework decision.

| Gate ID | Gate | Required evidence | Current status |
| --- | --- | --- | --- |
| UI-GATE-001 | Transparent topmost overlay | Official evidence or a future runtime prototype for the combined behavior | Requires runtime prototype |
| UI-GATE-002 | Multi-monitor and DPI | Official evidence or a future runtime prototype across mixed-DPI displays | Requires runtime prototype |
| UI-GATE-003 | Capture-entry latency | A future runtime prototype with a defined measurement boundary | Requires runtime prototype |
| UI-GATE-004 | Focus and input behavior | A future runtime prototype covering acquire, cancel and restore | Requires runtime prototype |
| UI-GATE-005 | Annotation rendering suitability | Official evidence or a future prototype for drawing and hit testing | Requires runtime prototype |
| UI-GATE-006 | Deployment suitability | Official deployment evidence mapped to SnipPlus footprint and servicing goals | Partially supported |
| UI-GATE-007 | Accessibility baseline | Official framework evidence plus later overlay-specific verification | Partially supported |

## 13. Required Future Runtime Spikes

These are future evidence spikes only. They were not executed and must not be interpreted as implementation instructions.

| Spike | Purpose | Frameworks compared | Evidence required | Pass condition | Failure implication |
| --- | --- | --- | --- | --- | --- |
| SPIKE-001 Virtual desktop overlay | Compare one overlay spanning the virtual desktop with one overlay per display | WinUI 3, WPF | Measured bounds, z-order and visibility across display arrangements | Required coverage is stable in both arrangements | Framework remains `Requires runtime prototype` for `UIF-001` and `UIF-004` |
| SPIKE-002 Borderless transparent composition | Verify borderless, topmost and transparent composition together | WinUI 3, WPF | Visual composition result and input routing record | Overlay is visible as intended and receives only intended input | `UI-GATE-001` remains open |
| SPIKE-003 DPI scale matrix | Verify 100%, 125%, 150% and 200% scale conditions | WinUI 3, WPF | Pointer, window, image and annotation coordinate measurements | Coordinate error stays within an agreed product tolerance | `UI-GATE-002` remains open |
| SPIKE-004 Heterogeneous DPI multi-monitor | Move or activate capture across displays with different scale factors | WinUI 3, WPF | Display transition and active-selection behavior | Selection and visual bounds remain aligned | Per-monitor workflow is not accepted |
| SPIKE-005 Capture-entry latency | Measure Print Screen to interactive overlay readiness | WinUI 3, WPF | Timestamped input, window-ready and first-interaction events | Result meets a product-approved latency target | Basic workflow performance risk remains unresolved |
| SPIKE-006 Focus lifecycle | Verify focus acquisition, cancellation and restoration | WinUI 3, WPF | Focus owner before, during and after the workflow | Original application focus is restored on every tested exit path | `UI-GATE-004` remains open |
| SPIKE-007 High-frequency pointer movement | Verify pointer capture and selection updates during fast movement | WinUI 3, WPF | Pointer event continuity, cancellation and rendered updates | No unacceptable loss, lag or boundary drift | Input and selection risk remains unresolved |
| SPIKE-008 Selection rectangle rendering | Verify selection geometry against the displayed desktop image | WinUI 3, WPF | Geometry, scale and repaint observations | Rectangle follows pointer and maps to output bounds | `UIF-010` remains `Requires runtime prototype` |
| SPIKE-009 Annotation object hit testing | Verify representative annotation objects can be selected and edited | WinUI 3, WPF | Hit-test, z-order and edit-state observations | Object targeting is predictable at required scales | `UI-GATE-005` remains open |
| SPIKE-010 Architecture distribution | Compare x64 and ARM64 deployment outputs | WinUI 3, WPF | Package contents, startup and architecture compatibility evidence | Required architectures launch through the selected distribution path | `UIF-016` remains unresolved |
| SPIKE-011 Packaged and unpackaged startup | Compare packaged and unpackaged startup dependencies | WinUI 3, WPF | Installation, initialization and offline-start observations | Approved deployment path starts without unplanned dependencies | `UI-GATE-006` remains open |

## 14. Traceability

| Source | Relevance to this research |
| --- | --- |
| `PRD/PRD-0002-user-experience-principles.md` | Windows-first, Fluent-first and muscle-memory constraints. |
| `PRD/PRD-0003-product-vision.md` | Windows desktop product direction and long-term product intent. |
| `PRD/PRD-0004-core-workflow.md` | Entry, active selection, completion and cancellation workflow boundaries. |
| `PRD/PRD-0006-non-functional-requirements.md` | Accessibility, compatibility, performance and maintainability expectations. |
| `Specs/SPEC-0005-capture-workflow.md` | Capture workflow states and selection behavior. |
| `Specs/SPEC-0009-annotation-capability.md` | Optional drawing and annotation boundary. |
| `Specs/SPEC-0010-feature-integration.md` | Cross-feature integration and responsibility boundaries. |
| `Architecture/ARCH-0002-layer-model.md` | UI and platform-integration separation. |
| `Architecture/ARCH-0004-component-boundaries.md` | Display, input and shared-state ownership boundaries. |
| `Architecture/ARCH-0005-component-interactions.md` | Interaction and failure-propagation constraints. |
| `Architecture/ARCH-BASELINE-REVIEW.md` | Architecture freeze and technology-decision entry criteria. |
| `Architecture/adr/ADR-0002-ui-framework-selection.md` | Decision under evidence review; remains Draft. |
| `Architecture/TECHNOLOGY-DECISION-ROADMAP.md` | TD-001 UI Framework and downstream technology-decision dependencies. |

## 15. Research Findings

The following findings are limited to evidence-supported observations:

- WinUI 3 currently has stronger documented Fluent alignment and documented Windows App SDK window, input, DPI and deployment primitives.
- WPF currently has stronger documented evidence for resolution-independent graphics, alpha-composited desktop windows, retained drawing and visual-layer hit testing.
- Windows App SDK windowing support across multiple desktop UI frameworks means HWND or `AppWindow` interoperability is not a WinUI 3-only advantage.
- SnipPlus-specific overlay feasibility, mixed-DPI alignment, focus restoration, low-latency activation and annotation performance remain unverified.
- Avalonia and Windows Forms remain qualified reference alternatives, but this research did not evaluate them at the same depth as the primary comparison.

This section does not select WinUI 3, reject WPF or change `ADR-0002`.

## 16. Evidence Readiness

**Selected status:** `Partially sufficient`

Allowed readiness values are:

- `Sufficient for ADR acceptance`
- `Partially sufficient`
- `Insufficient for ADR acceptance`

The selected status means the official evidence is sufficient for continued ADR review, but not sufficient to move `ADR-0002` to Accepted because the critical overlay, DPI, focus, latency, drawing and deployment gates remain open.

## 17. Open Questions

- Can WinUI 3 complete the SnipPlus overlay with acceptably low risk?
- Can WPF achieve Fluent-first alignment at an acceptable maintenance cost?
- Is native HWND or layered-window interop required for the final overlay behavior?
- Should rendering remain decoupled from the Desktop UI Framework decision?
- Will the packaging mode affect the framework decision?
- Is ARM64 a first-version requirement?
- Should the UI Framework own annotation rendering, or only host it?
- When may the future runtime spikes be authorized?

## 18. Known Limitations

- No SnipPlus project, executable or runtime implementation exists for direct verification.
- No framework version, .NET version, Windows App SDK version or packaging mode has been selected.
- Official documentation describes framework capabilities, not the complete SnipPlus workflow.
- The reviewed WinUI 3 sources do not directly establish a transparent, full-screen, multi-monitor capture overlay with the required focus and input behavior.
- The reviewed WPF sources establish transparent-window and drawing primitives, but not the complete capture workflow or its performance.
- Avalonia and Windows Forms were intentionally reviewed at lower depth.
- No build, prototype, runtime, performance, accessibility or deployment test was executed.

## 19. UNKNOWN / TBD

- Exact WinUI 3 overlay transparency and desktop-composition behavior.
- Exact multi-monitor coverage and coordinate-conversion rules for both primary frameworks.
- Per-monitor DPI transition behavior while a selection is active.
- Focus acquisition and restoration without disturbing the original application state.
- Global shortcut registration and conflict behavior.
- Pointer cancellation policy for display changes, lock, logoff and deactivation.
- Drawing and hit-testing performance for the intended annotation scope.
- Startup latency, capture latency, frame rate, memory footprint and power impact.
- Packaging mode, update strategy, offline installation and acceptable distribution size.
- Overlay-specific accessibility evidence.

## 20. Conclusion

This research keeps `ADR-0002` in `Draft` with acceptance deferred.

WinUI 3 and WPF are both technically qualified for continued investigation. The current evidence shows different strengths, but neither framework has been proven against the complete SnipPlus overlay, mixed-DPI, focus, input, drawing and deployment conditions.

No final framework choice is made. No ADR, PRD, Specs, Architecture or source-code file is changed by this research.

## 21. References

- [WinUI 3](https://learn.microsoft.com/en-us/windows/apps/winui/winui3/)
- [Windows app development documentation](https://learn.microsoft.com/en-us/windows/apps/)
- [Manage app windows](https://learn.microsoft.com/en-us/windows/apps/develop/ui/manage-app-windows)
- [Handle pointer input](https://learn.microsoft.com/en-us/windows/apps/develop/input/handle-pointer-input)
- [User interface migration, including WinUI](https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/migrate-to-windows-app-sdk/guides/winui3)
- [Windows App SDK deployment overview](https://learn.microsoft.com/en-us/windows/apps/package-and-deploy/deploy-overview)
- [Windows App SDK deployment for unpackaged apps](https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/deploy-unpackaged-apps)
- [Windows Presentation Foundation overview](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/overview/)
- [Windows in WPF overview](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/windows/)
- [WPF input overview](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/advanced/input-overview)
- [WPF graphics rendering overview](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/graphics-multimedia/wpf-graphics-rendering-overview)
- [WPF hit testing in the visual layer](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/graphics-multimedia/hit-testing-in-the-visual-layer)
- [Deploy a WPF application](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/app-development/deploying-a-wpf-application-wpf)
- [Avalonia getting started](https://docs.avaloniaui.net/docs/get-started/)
- [Avalonia supported platforms](https://docs.avaloniaui.net/docs/supported-platforms)
- [Avalonia cross-platform architecture](https://docs.avaloniaui.net/docs/fundamentals/cross-platform-architecture)
- [Windows Forms overview](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/overview/)

## 22. Review Boundary

This is a Decision Evidence Research record. Its observations must not be copied into `ADR-0002`, PRD or Specs as accepted product or architecture decisions without a separate review and explicit traceability update.
