# ADR-0003 Rendering Technology

## Document Control

| Field | Value |
| --- | --- |
| Document ID | `ADR-0003` |
| Title | Rendering Technology |
| Status | `Accepted` |
| Decision category | Rendering |
| Version | `1.1` |
| Owner | Repository owner |
| Date accepted | `2026-07-26` |
| Last reviewed | `2026-07-27` |
| Supersedes | None |
| Superseded by | None |
| Normative references | Accepted PRD／Specs、Architecture baseline、ADR-0002、ADR-0005 |

## Context

SnipPlus v1 requires rendering for:

- all-display frozen selection presentation;
- dimmed outside region and clear selected interior;
- selection border、edge／corner handles and hit testing;
- editing／confirmation function bar integration;
- Rectangle、Arrow／Line、Highlighter and Numbered Marker;
- Text drawing and measurement using Microsoft JhengHei by default;
- Mosaic／Blur effects;
- annotation previews and object-selection visuals;
- clipping Annotation objects to the current selection;
- deterministic final raster production for Complete and Save.

Rendering must remain separate from Capture、workflow state、Clipboard and PNG file delivery.

## Options considered

### WinUI XAML／Composition only

Appropriate for retained UI composition, but insufficient as the sole bounded path for bitmap effects、pixel processing and deterministic offscreen raster output.

### Direct2D／DirectWrite directly

Provides maximum control but adds native interop、resource-lifetime and deployment complexity without a verified need.

### Win2D only

Strong for immediate-mode drawing and effects, but should not replace normal WinUI control layout、focus、accessibility and retained UI composition.

### SkiaSharp

Capable but adds a second native rendering engine and cross-platform complexity that is not a v1 driver.

### Bounded WinUI Composition plus Win2D

Combines native retained UI composition with a testable immediate-mode rendering and effects boundary.

## Decision

Adopt a bounded hybrid architecture:

1. **WinUI 3 XAML and Microsoft.UI.Composition** own retained application UI、overlay composition、function-bar controls、selection handles、focus and accessibility presentation.
2. **Win2D** owns immediate-mode geometry、text、bitmap effects、clipping、render targets and deterministic offscreen raster production.
3. Core and Contracts exchange platform-neutral render intent、scene、style、selection and revision information. They do not reference Win2D、Composition or concrete XAML drawing types.
4. The rendering boundary receives one Frozen Virtual Desktop session、current Selection Revision and current Annotation Revision.
5. Final rendering creates one immutable raster result representing only the current selection and visible Annotation content.
6. Final output excludes masks、selection borders、handles、function bar and normal SnipPlus windows.
7. Complete and Save use equivalent final-render semantics and the same immutable result identity for the committed revision.
8. Clipboard and PNG file delivery remain separate downstream capabilities. The Save workflow coordinates them; the renderer does not publish either destination.
9. Direct Direct2D／DirectWrite or another rendering engine requires a later ADR supported by verified limitations.

## Coordinate and rendering contract

The rendering contract distinguishes:

- Frozen Virtual Desktop physical coordinates;
- per-display source-pixel coordinates;
- host DIPs;
- Annotation object geometry;
- output pixels;
- selection、annotation and output revisions;
- pixel format、alpha mode and color-space metadata;
- text measurement and font fallback;
- object z-order and hit-test identity;
- preview rendering versus final committed raster.

Selection adjustment does not scale or move Annotation geometry. Final rendering intersects all objects with the current selection bounds.

## Ownership boundary

| Concern | Owner | Rendering relationship |
| --- | --- | --- |
| Shared workflow state | `COMP-001` | Renderer receives intent and returns outcomes only. |
| Frozen display acquisition | Capture capability／adapter | Supplies immutable source frames. |
| Selection semantics | `COMP-005` | Supplies geometry and Selection Revision. |
| Editing and Annotation document | `COMP-007`／`COMP-008` | Supplies objects、styles and Annotation Revision. |
| Final raster | `COMP-006` plus rendering adapter | Produces immutable selected-and-annotated result. |
| Clipboard publication | `COMP-009`／`COMP-015` | Consumes final result after Complete or Save sequencing. |
| PNG delivery | `COMP-010`／`COMP-016` | Consumes final result during Save. |

The rendering adapter does not mutate shared state、capture the desktop、open Save As or publish Clipboard.

## Consequences

### Benefits

- Native fit with WinUI 3 and the Windows-first product boundary.
- Appropriate separation between controls／composition and explicit raster／effect work.
- Supports required v1 Annotation tools and deterministic final output.
- Preserves a replaceable platform-neutral render-intent boundary.

### Costs and risks

- Preview and final raster paths must share coordinate、style and clipping rules.
- Multi-display mixed-DPI presentation requires explicit verification.
- Win2D device and resource loss require typed failure and cleanup behavior.
- Mosaic、Blur and text fidelity require deterministic tests.

## Current implementation state

- WinUI image presentation、Win2D rendering foundation、BGRA8 image handling and deterministic crop tests exist.
- Current code does not implement the accepted multi-display scene、selection handles、function bar、Annotation document、required tools or complete final-render composition.
- The technology decision conforms; product rendering implementation remains partial.

## Reconsideration conditions

Revisit only if verified evidence shows the accepted WinUI／Win2D path cannot satisfy required mixed-DPI presentation、Annotation effects、text fidelity、performance or final-raster correctness without a materially different rendering technology.
