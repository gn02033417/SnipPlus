# ADR-0003 Rendering Technology

## Document Control

| Field | Value |
| --- | --- |
| Document ID | ADR-0003 |
| Title | Rendering Technology |
| Status | Accepted |
| Decision Category | Rendering |
| Version | 1.0 |
| Owner | Repository owner |
| Date proposed | 2026-07-26 |
| Date reviewed | 2026-07-26 |
| Date accepted | 2026-07-26 |
| Supersedes | None |
| Superseded by | None |
| Normative References | ADR-0002、PRD-0002、PRD-0003、PRD-0006、SPEC-0005、SPEC-0008、SPEC-0009、SPEC-0010、ARCH-0002、ARCH-0003、ARCH-0004、ARCH-0005、ARCH-BASELINE-REVIEW、ADR-BASELINE |
| Informative References | RESEARCH-TECH-RENDER-001 through RESEARCH-TECH-RENDER-009、official Microsoft WinUI 3、Microsoft.UI.Composition and Win2D documentation |

## Context

ADR-0002 accepts WinUI 3 as the SnipPlus desktop UI framework. SnipPlus still requires an explicit rendering decision for:

- Selection and overlay feedback.
- Vector annotation geometry.
- Text drawing and measurement.
- Resize and rotation handles.
- Hit-test visualization.
- Mosaic／pixelation and other bounded raster effects.
- Alpha composition and clipping.
- Final raster production for a separate Output boundary.
- DPI-aware mapping between host DIPs、renderer logical units and output pixels.

The Frozen Architecture requires:

- Domain and Feature boundaries own annotation semantics、selection state and workflow intent.
- Platform Integration owns display、DPI and platform-specific behavior.
- Rendering must not own Capture、Clipboard、Output delivery or Shared State Authority.
- Clipboard and Output remain parallel downstream consumers.
- Concrete rendering APIs must remain behind an adapter／contract boundary rather than leaking into Domain Capability.

Rendering Research 10–18 compared framework-native retained rendering、Direct2D／DirectWrite、Win2D、SkiaSharp and a hybrid strategy. The research accumulated extensive official-source and planning evidence but no product runtime evidence. This ADR therefore selects a bounded architecture and primary technology path while preserving mandatory implementation verification.

## Decision Drivers

| Driver | Priority | Source |
| --- | --- | --- |
| Native fit with the Accepted WinUI 3 host | High | ADR-0002 |
| Retained-mode interaction visuals and composition | High | PRD-0002、SPEC-0005、Microsoft.UI.Composition evidence |
| Immediate-mode 2D drawing、bitmap and effect support | High | SPEC-0009、Rendering Research 10–18、Win2D evidence |
| Clear separation of domain render intent from platform APIs | High | ARCH-0002、ARCH-0003、ARCH-0004 |
| DPI-aware drawing and output mapping | High | PRD-0006、Rendering Research |
| Maintainable replacement boundary | High | ARCH-0001、PRD-0003、PRD-0006 |
| Avoid unnecessary third-party/native interop surface | Medium | ARCH-0001、Rendering Research |
| Deterministic output and testability | High | SPEC-0008、TD-011 dependency |
| Cross-platform rendering portability | Low | Frozen PRD is Windows-first |

## Options Considered

### Option A — WinUI 3 XAML／Composition only

Use XAML elements and `Microsoft.UI.Composition` for all interaction visuals and rendering.

**Advantages**

- Native to the Accepted WinUI 3 host.
- Strong fit for retained-mode visuals、clipping、opacity、animation and interaction feedback.
- Minimal additional package surface for basic UI rendering.

**Disadvantages**

- Does not by itself provide the clearest bounded path for bitmap effects、pixel operations and deterministic offscreen raster output.
- Product-level mosaic、export fidelity and resource behavior would still require custom implementation paths.

### Option B — Direct2D／DirectWrite directly

Use native Direct2D／DirectWrite APIs through an explicit managed／native integration layer.

**Advantages**

- Maximum low-level rendering control.
- Strong native graphics and text capability.

**Disadvantages**

- Adds direct native interop、resource lifetime and architecture-specific deployment complexity.
- Requires a custom WinUI host adapter and more explicit error／device recovery work.
- Exposes a broader low-level surface than the first product baseline requires.

### Option C — Win2D only

Use Win2D as the primary drawing surface for all visual and raster work.

**Advantages**

- Official WinUI／Windows App SDK integration.
- Immediate-mode GPU-accelerated 2D drawing over Direct2D.
- Bitmap、text、geometry、effect and render-target capabilities.

**Disadvantages**

- Does not replace WinUI XAML／Composition ownership of the general UI visual tree.
- Interaction controls、focus、layout and normal UI composition should not be forced into one custom canvas.
- Resource lifecycle and output fidelity still require verification.

### Option D — SkiaSharp

Use SkiaSharp and WinUI views as the primary rendering engine.

**Advantages**

- Broad vector／raster API and possible future portability.
- WinUI and WPF integration packages exist.

**Disadvantages**

- Introduces an additional third-party rendering engine and native asset lifecycle.
- Cross-platform portability is not a current product driver.
- Adds dependency、deployment、text and host-integration verification without a demonstrated product need.

### Option E — Bounded hybrid WinUI Composition + Win2D

Use WinUI 3 XAML／`Microsoft.UI.Composition` for retained-mode host visuals and interaction composition, with Win2D behind a rendering adapter for immediate-mode 2D drawing、bitmap work、effects and offscreen raster production.

**Advantages**

- Uses native technologies aligned with the Accepted WinUI 3 host.
- Separates UI composition from explicit raster/effect work.
- Avoids direct Direct2D interop in product-facing layers while retaining Direct2D capability through Win2D.
- Avoids an unnecessary third-party rendering engine.
- Supports a replaceable renderer contract and testable render intent.

**Disadvantages**

- Requires strict ownership boundaries between XAML／Composition and Win2D surfaces.
- Two rendering paths can drift unless coordinate、color、alpha and output contracts are explicit.
- Runtime verification remains mandatory.

## Accepted Decision

Adopt a **bounded hybrid rendering architecture**:

1. **WinUI 3 XAML and `Microsoft.UI.Composition`** are the primary retained-mode visual and interaction-composition technologies.
2. **Win2D** is the primary immediate-mode 2D rendering technology for operations that require explicit geometry drawing、text drawing、bitmap processing、effects、render targets or offscreen raster production.
3. Product and Domain layers communicate through an abstract render-intent／scene contract. They must not reference Win2D、Composition or concrete XAML drawing types.
4. A WinUI rendering adapter translates render intent、host DIPs and platform display context into Composition／Win2D operations.
5. Final raster production is owned by the rendering boundary, but delivery、encoding policy and persistence remain under the separate Output contract.
6. Direct2D／DirectWrite may be used only as implementation details underneath Win2D or through a future separately accepted ADR when Win2D cannot satisfy a verified requirement.
7. SkiaSharp is not selected for the initial product baseline. It remains a revisit option only if a future accepted requirement demands portability or a capability unavailable through the selected Windows-native path.

## Scope of Applicability

This decision applies to:

- Selection and overlay visuals hosted inside the SnipPlus WinUI 3 application boundary.
- Annotation display and interaction feedback.
- Text、geometry、clipping、alpha and bounded raster effects.
- Synthetic and product rendering paths that produce a raster candidate for Output.
- Rendering resource lifecycle and device-loss handling inside the rendering adapter.

## Explicit Exclusions

This ADR does not select or define:

- Capture Backend or desktop-pixel acquisition.
- Global overlay window topology、focus policy or keyboard hook behavior.
- Clipboard API or Clipboard payload.
- Final image representation contract or encoder policy.
- File storage、Output destination or persistence.
- Language／Runtime version.
- Windows App SDK version.
- Win2D package version.
- Packaging or deployment mode.
- Project／assembly structure.
- Concrete classes、interfaces、method signatures or source files.
- Product performance thresholds.
- HDR or color-management policy.

## Ownership Boundary

| Concern | Owner | Rendering relationship |
| --- | --- | --- |
| Workflow state | COMP-001／Feature Coordination | Renderer receives intent; never mutates state directly |
| Capture entry and platform capture | Capture boundary | Supplies a result or failure; renderer never invokes capture APIs |
| Selection semantics | Domain／Selection boundary | Supplies selection geometry and style intent |
| Annotation model and commands | Annotation capability | Supplies objects、layering、transform and style intent |
| Display／DPI context | Platform Integration | Supplies monitor and coordinate context |
| Retained host visuals | WinUI XAML／Composition adapter | Owns visual-tree and interaction composition |
| Immediate 2D／effect rendering | Win2D adapter | Owns drawing sessions、resources、effects and render targets |
| Output delivery | Output capability | Consumes an approved raster／image result |
| Clipboard handoff | Clipboard capability | Parallel downstream consumer; not owned by renderer |

## Required Rendering Contract Properties

The later consolidated contract must distinguish:

- Host DIPs.
- Renderer logical coordinates.
- Display physical pixels.
- Source image pixels.
- Output pixels.
- Alpha mode and pixel format.
- Color-space／HDR treatment.
- Text measurement and font fallback.
- Object z-order and hit-test intent.
- Render-resource creation、invalidation and recovery.
- Display rendering versus final raster equivalence.

No implementation may collapse these spaces or responsibilities without an accepted contract change.

## Trade-offs

### Benefits accepted

- Strong alignment with WinUI 3 and the Windows-native product boundary.
- Retained-mode host composition remains separate from explicit drawing and raster effects.
- Win2D supplies a managed WinRT path over Direct2D without requiring direct native API ownership in product layers.
- Rendering APIs remain replaceable behind an abstract intent and adapter boundary.
- The initial baseline avoids an additional third-party graphics engine.

### Costs accepted

- Coordinate、alpha、color and output equivalence must be governed across Composition and Win2D.
- Win2D device and resource lifecycle must be handled explicitly.
- Some UI visuals may remain XAML／Composition while final raster generation uses Win2D, requiring equivalence tests.
- Direct low-level optimization may require a future ADR if verified requirements exceed Win2D.
- Windows-native rendering narrows future portability.

### Neutral consequences

- Annotation tools and image representation remain separate decisions／contracts.
- Capture and Clipboard decisions remain unresolved.
- Language、Runtime、SDK and package versions remain unresolved.
- Runtime evidence remains absent until an explicitly authorized implementation or verification task runs.
- WPF rendering parity is no longer a product requirement because ADR-0002 accepted WinUI 3 as the host framework.

## Consequences

### Positive

- TD-003 Capture Backend can target a defined downstream rendering boundary.
- TD-005 Image Representation can define a payload independent of concrete rendering APIs.
- TD-011 Testing Strategy can define deterministic render-intent and raster verification.
- Project Structure can isolate domain render intent from the WinUI rendering adapter.

### Negative

- Incorrect boundary enforcement could leak Win2D or XAML types into Domain layers.
- Resource-loss、DPI and output-fidelity defects may not appear until runtime verification.
- Composition and Win2D behavior can diverge if rendering intent is implemented twice rather than shared.

### Follow-up work

The following are required decision／contract work, not coding authorization:

1. Decide TD-003 Capture Backend.
2. Decide TD-004 Clipboard Integration.
3. Decide TD-005 Image Representation.
4. Decide TD-011 Testing Strategy.
5. Define consolidated Shared Result／Image Result and rendering contracts.
6. Define Project／assembly mapping and versions.
7. Verify a minimal synthetic scene through WinUI 3／Composition／Win2D during the first authorized vertical slice.

## Verification Requirements

Acceptance of this ADR does not claim runtime success. The implementation-readiness package must require verification of:

- WinUI 3 host startup and rendering-surface creation.
- Win2D package restore and supported process architecture.
- Continuous selection redraw without stale pixels.
- Vector geometry、text and handle rendering.
- DPI and logical-to-output coordinate mapping.
- Alpha and transparent-background behavior.
- Bounded mosaic／pixelation effect.
- Display render versus final raster comparison.
- Device／resource recreation and failure handling.
- Cleanup with no retained temporary output outside the approved evidence boundary.

A failed verification does not silently change this ADR. A blocking incompatibility triggers review or a superseding ADR.

## Traceability

| Source | Relevance |
| --- | --- |
| ADR-0002 | Accepted WinUI 3 host framework |
| PRD-0002 | Windows muscle memory and Fluent-first UX |
| PRD-0003 | Windows desktop product direction and maintainability |
| PRD-0006 | DPI、accessibility、performance and maintainability constraints |
| SPEC-0005 | Selection and capture workflow visuals |
| SPEC-0008 | Separate Output lifecycle and delivery boundary |
| SPEC-0009 | Optional annotation capability and visual objects |
| SPEC-0010 | Cross-feature ownership and downstream separation |
| ARCH-0002 | Presentation、Domain and Platform dependency direction |
| ARCH-0003 | Rendering-related module ownership |
| ARCH-0004 | Component and Shared State boundaries |
| ARCH-0005 | Interaction and prohibited dependency boundaries |
| RESEARCH-TECH-RENDER-001 through 009 | Candidate comparison、official evidence、gaps and runtime-verification plan |
| TD-002 | Technology Decision Roadmap item completed by this ADR |

## External Evidence

| Source | Evidence used |
| --- | --- |
| [Microsoft WinUI 3](https://learn.microsoft.com/en-us/windows/apps/winui/winui3/) | WinUI 3 is the Windows App SDK native desktop UI framework and XAML host. |
| [Microsoft visual layer overview](https://learn.microsoft.com/en-us/windows/apps/develop/composition/visual-layer) | `Microsoft.UI.Composition` is the WinUI 3／Windows App SDK retained visual layer for graphics、effects and animation. |
| [Microsoft XAML and Composition interoperability](https://learn.microsoft.com/en-us/windows/apps/develop/composition/xaml-comp-interop) | WinUI 3 XAML elements are backed by Composition and can interoperate with Composition operations. |
| [Microsoft Win2D overview](https://learn.microsoft.com/en-us/windows/apps/develop/win2d/in-a-core-app) | Win2D is an immediate-mode GPU-accelerated 2D WinRT API that integrates with WinUI XAML and exposes drawing、bitmap、text and effect capabilities. |
| [Microsoft Win2D quick start](https://learn.microsoft.com/en-us/windows/apps/develop/win2d/quick-start) | Win2D provides a `CanvasControl` drawing path inside WinUI XAML. |

## Implementation and Verification State

| Artifact | Status |
| --- | --- |
| Implementation reference | Not implemented |
| Runtime verification | Not verified |
| Package restore evidence | Not verified |
| DPI／output fidelity evidence | Not verified |
| Device-resource recovery evidence | Not verified |
| Coding authorized | No |

## Review Record

| Field | Value |
| --- | --- |
| Reviewer | ChatGPT repository review |
| Review date | 2026-07-26 |
| Review result | Approved |
| Review basis | Frozen PRD／Specs／Architecture、Accepted ADR-0002、RESEARCH-TECH-RENDER-001 through 009 and current official Microsoft documentation |
| Open comments | None blocking acceptance of the bounded architecture decision |
| Resolution of comments | Removed WPF parity as a decision requirement after ADR-0002; selected a bounded native hybrid; retained mandatory runtime verification and version decisions |
| Acceptance authority | Repository owner through repeated explicit instruction to proceed with the next documentation convergence step |

Review findings:

- The ADR handles one major decision: the rendering architecture and primary technologies.
- The decision does not alter Frozen Feature、Module、Component or Interaction ownership.
- The selected technologies have official WinUI 3 documentation and a clear host relationship.
- Direct2D、SkiaSharp、Capture、Clipboard、Output and version decisions remain explicitly separated.
- Absence of runtime evidence is visible and converted into required verification, not false success.
- No Project、package、source code or runtime operation is created by this ADR.

## Change and Supersession

Revisit or supersede this ADR if:

- ADR-0002 is superseded by a non-WinUI host.
- Runtime verification shows a blocking Win2D／Composition incompatibility.
- Accepted requirements demand cross-platform rendering.
- Verified fidelity、performance、HDR or color requirements cannot be met through the selected path.
- Direct2D or another rendering engine becomes necessary as a primary product dependency.
- Architecture changes rendering ownership or dependency direction.

If superseded:

- Create a new ADR with a new number.
- Mark the new ADR as superseding ADR-0003.
- Change this ADR to `Superseded` and preserve its decision history.

## Acceptance Verification

| Check | Result |
| --- | --- |
| Unique ADR ID and correct location | PASS |
| Single major decision | PASS |
| Accepted upstream UI framework | PASS |
| Frozen-source traceability | PASS |
| Official first-party evidence | PASS |
| Alternatives retained | PASS |
| Benefits、costs and negative consequences recorded | PASS |
| Runtime limitations explicit | PASS |
| Ownership boundaries preserved | PASS |
| Review Record completed | PASS |
| Coding authorized | No |

## Non-goals

This ADR does not:

- Create a Solution or Project.
- Add Win2D or any package.
- Select Language、Runtime、SDK or package versions.
- Restore、Build、Run or Test.
- Implement rendering、capture、annotation、clipboard or output.
- Create screenshot or runtime evidence.
- Modify Frozen PRD、Specs or Architecture ownership.
