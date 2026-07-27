# ADR-0004 Capture Backend

## Document Control

| Field | Value |
| --- | --- |
| Document ID | `ADR-0004` |
| Title | Capture Backend |
| Status | `Accepted` |
| Decision category | Platform Integration |
| Version | `1.1` |
| Owner | Repository owner |
| Date accepted | `2026-07-26` |
| Last reviewed | `2026-07-27` |
| Supersedes | None |
| Superseded by | None |
| Normative references | Accepted PRD／Specs、Architecture baseline、ADR-0002、ADR-0003、ADR-0005 |

## Context

SnipPlus v1 requires one explicit PrintScreen request to establish a stable Frozen Virtual Desktop session containing every connected display before Selection becomes interactive.

The capture backend must support:

- per-display source identity and physical bounds;
- negative Virtual Desktop origins and arbitrary display arrangement;
- immutable one-shot frames for all participating displays;
- exclusion of SnipPlus normal windows;
- short-lived capture resources with deterministic cleanup;
- same-session preview and final output without post-selection recapture;
- classified permission、source、device、size and timing failures.

Cross-monitor Selection is a product requirement. The backend does not need to allocate one giant Virtual Desktop bitmap; it must provide frames and metadata that allow the product to present and compose one logical canvas.

## Options considered

### Windows.Graphics.Capture

Official Windows desktop capture API with monitor／window capture items、Direct3D surfaces and documented Windows App SDK integration.

### DXGI Desktop Duplication

Provides detailed per-output desktop surfaces but adds substantially more native interop、adapter matching and access-loss recovery complexity.

### GDI BitBlt

Simple examples exist, but it is a weak fit for modern composition、DPI、color fidelity and future rendering requirements.

### Immediate hybrid fallback

Adding WGC plus DXGI or GDI fallback before a verified blocker would multiply capture、coordinate、failure and verification paths.

## Decision

Use **Windows.Graphics.Capture (WGC)** as the sole v1 capture backend.

1. Platform display context enumerates every connected display participating in the current Virtual Desktop snapshot.
2. One WGC capture item／adapter boundary is created for each required display source.
3. Every required display produces one usable immutable frame before `Selecting` begins.
4. Partial all-display freeze is not accepted as a successful session. Missing、inconsistent or invalid frames produce a classified failure.
5. Each frame records Session ID、Display ID、physical bounds in Virtual Desktop coordinates、pixel size、DPI context、capture timestamp and frame identity.
6. Selection preview uses the frozen per-display frames; it does not recapture desktop content while the user drags、moves、resizes、reselects or annotates.
7. Final rendering intersects the current Virtual Desktop selection with each display’s frozen bounds and composes the corresponding source regions into one output image.
8. The selected output uses the same frozen session that the user saw during Selection.
9. Capture resources are one-shot and short-lived unless a later accepted requirement needs continuous acquisition.
10. Cursor capture is disabled by default for screenshot output.
11. SnipPlus normal windows are excluded through platform capture exclusion and coordinated window visibility. Exclusion mechanisms are defense in depth, not permission to capture before UI cleanup is ready.
12. Protected content、secure desktop、permission denial、closed source、frame timeout、device loss、display topology change and frame-size mismatch return typed failures; they never become blank successful output.
13. DXGI Desktop Duplication and GDI are not speculative fallbacks. A verified WGC blocker requires a targeted superseding or extension ADR.

## Multi-display composition boundary

WGC owns per-display acquisition, not the product’s unified canvas semantics.

The accepted model permits:

- separate immutable frame objects per display;
- one shared Virtual Desktop origin and coordinate version;
- cross-display intersection and composition by accepted capture／render boundaries;
- display-specific DPI and orientation metadata;
- non-contiguous display layouts.

The visual representation of physical gaps between irregularly arranged displays remains an explicit product decision. The backend must preserve enough topology information for that decision; it must not invent gap pixels as captured desktop content.

## Ownership boundary

| Concern | Owner |
| --- | --- |
| PrintScreen acceptance and session state | Product Workflow／Feature Coordination |
| Display enumeration、DPI and foreground context | Platform Display Context boundary |
| Per-display WGC item and frame acquisition | Platform Capture Adapter |
| Frozen frame collection and session ownership | Capture Session／Capture capability |
| Virtual Desktop Selection geometry | Selection capability |
| Cross-display source intersection | Capture／render contract |
| Final selected-and-annotated raster | Final Render boundary |
| Clipboard and PNG delivery | Separate delivery capabilities |

The WGC adapter does not mutate shared state、own Selection、render Annotation UI、publish Clipboard or write files.

## Deferred capture capabilities

- Window capture as a user-selectable product mode.
- Scrolling capture.
- Continuous recording or video.
- Audio capture.
- HDR-preserving output.
- Secure desktop or protected-content access.
- DXGI or GDI fallback.

**Multi-display capture and cross-monitor Selection are not deferred.**

## Verification requirements

- Enumerate all displays and verify stable physical bounds／origin.
- Acquire exactly one usable frame per required display before Selection.
- Verify same-session preview and output without recapture.
- Verify negative-origin and mixed-DPI mappings.
- Verify a Selection crossing two or more displays.
- Verify display topology／DPI change produces classified failure rather than stale output.
- Verify window exclusion、cancellation and exact-once resource cleanup.
- Verify failures do not expose private screen content in evidence.

Interactive display and focus verification requires explicit authorization in the current task.

## Current implementation state

- One-display WGC acquisition、bounded frame wait、frame validation、same-frame crop and categorized platform tests exist.
- All-display enumeration、per-display session ownership、cross-monitor composition and topology-change handling are missing.
- The WGC decision remains accepted; the current adapter is a partial reusable foundation.

## Reconsideration conditions

Revisit only after verified evidence shows WGC cannot satisfy an accepted display、permission、fidelity、performance or lifecycle requirement on the supported Windows baseline.
