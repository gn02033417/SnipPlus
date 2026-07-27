# ADR-0005 Image Representation

## Document Control

| Field | Value |
| --- | --- |
| Document ID | `ADR-0005` |
| Title | Canonical Image Representation |
| Status | `Accepted` |
| Decision category | Interaction／Data Contract |
| Version | `1.1` |
| Owner | Repository owner |
| Date accepted | `2026-07-26` |
| Last reviewed | `2026-07-27` |
| Supersedes | None |
| Superseded by | None |
| Normative references | Accepted PRD／Specs、Architecture baseline、ADR-0003、ADR-0004 |

## Context

Capture、selection presentation、Annotation rendering、Clipboard and PNG output require stable image ownership independent of WGC frame pools、Win2D devices and WinUI controls.

SnipPlus v1 also requires:

- one immutable frame per participating display;
- one logical Frozen Virtual Desktop session without requiring one giant bitmap;
- deterministic crop／composition across displays;
- one immutable final rendered result for Complete or Save;
- explicit Session、Result and revision identity;
- predictable pixel format、alpha、DPI、color and disposal behavior.

## Options considered

### Direct3D surface as the canonical result

Efficient for GPU processing but couples ownership to device／surface lifetime and is less convenient for deterministic CPU tests、PNG encoding and Clipboard streams.

### Win2D CanvasBitmap as the canonical result

Convenient for rendering but leaks rendering technology into shared contracts and ties lifetime to a CanvasDevice.

### Encoded PNG bytes as the canonical result

Portable for delivery, but unsuitable as the primary mutable-free source for crop、composition、preview and raster effects without repeated decode.

### BGRA8 premultiplied SoftwareBitmap

Windows-native、CPU-readable、explicitly owned and compatible with WinUI display、BitmapEncoder、Win2D conversion and Clipboard PNG publication.

## Decision

Use an immutable **BGRA8 premultiplied SoftwareBitmap-backed image result** as the canonical CPU-readable raster representation.

1. Pixel format is BGRA8.
2. Alpha mode is premultiplied.
3. Image metadata explicitly records width、height、row stride、DPI、color space、source bounds、crop bounds and capture time.
4. Each image result carries one Result ID and Session ID.
5. Frozen per-display frames also carry Display／Frame identity and the shared Virtual Desktop coordinate version through their session contracts.
6. Final rendered results carry the committed Selection Revision、Annotation Revision or equivalent output-revision identity through the request／result contract.
7. Image results are immutable after publication to another capability.
8. Ownership and leases are explicit; disposal is deterministic and safe when a temporary consumer lease is active.
9. A Frozen Virtual Desktop may contain multiple canonical per-display image results. This ADR does not require one contiguous full-desktop SoftwareBitmap.
10. Cross-monitor final rendering creates one canonical output image sized to the selected physical-pixel bounds.
11. PNG encoding and Clipboard publication consume the canonical final result but do not become its canonical in-memory representation.
12. GPU-native optimization may be added behind compatible contracts only after evidence demonstrates a need.

## Metadata requirements

A canonical image result includes, as applicable:

- `ResultId`;
- `SessionId`;
- pixel width and height;
- row stride;
- `Bgra8` pixel format;
- premultiplied alpha mode;
- color-space classification;
- DPI X and Y;
- source kind and source physical bounds;
- crop physical bounds;
- capture／render timestamp;
- cursor inclusion flag;
- content or revision identity needed to reject stale results.

Platform-neutral contracts must not expose raw WinUI、WGC or Win2D objects.

## Ownership and lifetime

- The capture session owns all Frozen display images until successful commitment、Cancel or terminal cleanup.
- Selection and preview consumers borrow or reference frames without taking workflow ownership.
- Final rendering produces a distinct immutable result.
- Clipboard and PNG delivery may share the same final result identity and use controlled leases／streams.
- Retryable delivery failure retains the final result and Editing session when required.
- Terminal failure or completed cleanup disposes images exactly once.
- A stale result is disposed without advancing workflow state.

## Coordinate and composition rules

- Per-display image pixel coordinates map to physical Virtual Desktop bounds through the session snapshot.
- Cross-display output uses intersection rectangles; it does not assume all displays share one DPI scale.
- Output bounds use a fixed exclusive right／bottom convention.
- Selection and Annotation geometry are converted to final output pixels using the same coordinate version.
- Non-display gaps are not silently fabricated as captured source pixels; their presentation remains an open product decision.

## Consequences

### Benefits

- Deterministic crop、composition、pixel comparison and PNG encoding.
- Clear ownership independent of capture and rendering devices.
- Direct compatibility with current Windows image and Clipboard paths.
- Supports multiple per-display frames and one final cross-monitor result.

### Costs and risks

- CPU-readable copies can use significant memory for large Virtual Desktop selections.
- Premultiplied alpha and row-stride handling must remain consistent across adapters.
- Large multi-display images require measured performance and bounded resource use.
- Color／HDR preservation remains deferred.

## Current implementation state

- BGRA8 premultiplied SoftwareBitmap result、metadata、leases、crop and PNG tests exist.
- Current contracts do not yet represent a Frozen Virtual Desktop frame collection or complete Selection／Annotation revision identity.
- The canonical image decision is conforming; multi-display and revision contracts remain partial.

## Reconsideration conditions

Revisit only if measured memory、performance、HDR or rendering requirements prove the SoftwareBitmap-centered canonical representation inadequate. A GPU-native optimization must preserve the accepted immutable result and delivery semantics or use a superseding ADR.
