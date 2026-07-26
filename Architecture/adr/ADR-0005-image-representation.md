# ADR-0005 Image Representation

## Document Control

| Field | Value |
| --- | --- |
| Document ID | ADR-0005 |
| Title | Canonical Image Representation |
| Status | Accepted |
| Decision Category | Interaction / Data Contract |
| Version | 1.0 |
| Owner | Repository owner |
| Date proposed | 2026-07-26 |
| Date reviewed | 2026-07-26 |
| Date accepted | 2026-07-26 |
| Supersedes | None |
| Superseded by | None |
| Normative References | ADR-0003、ADR-0004、PRD-0004、PRD-0005、PRD-0006、SPEC-0007、SPEC-0008、SPEC-0009、SPEC-0010、ARCH-0002、ARCH-0003、ARCH-0004、ARCH-0005、ADR-BASELINE |
| Informative References | Microsoft SoftwareBitmap、SoftwareBitmapSource、BitmapEncoder and Win2D pixel-format documentation |

## Context

Capture、Rendering、Annotation、Clipboard and Output require one stable result representation. Without a canonical representation, concrete Direct3D、Win2D、XAML or Clipboard types would leak across Architecture boundaries and each downstream path could interpret pixel format、alpha、DPI and lifetime differently.

The first product baseline needs deterministic behavior more than zero-copy optimization. A future optimized GPU representation may be added behind the same contract after evidence exists.

## Decision Drivers

- Compatible with WinUI 3 display.
- Compatible with Windows image encoding and Clipboard bitmap streams.
- CPU-readable for deterministic tests、crop and pixel comparison.
- Explicit pixel format、alpha、color and lifetime.
- Independent from WGC frame-pool and Win2D device lifetime.
- Immutable after publication to Shared Result.
- Simple ownership and disposal model.
- Sufficient for first vertical slice without precluding future GPU optimization.

## Options Considered

### Direct3D surface as canonical result

Efficient for Capture and rendering, but couples downstream consumers to device、thread and resource lifetime. Clipboard、encoding and deterministic tests require additional readback paths.

### Win2D CanvasBitmap as canonical result

Convenient for rendering, but ties the domain result to Win2D resource/device ownership and is not the natural contract for Clipboard or non-rendering consumers.

### Encoded PNG bytes as canonical result

Portable and easy to persist, but repeatedly encoding/decoding for annotation and rendering is inefficient and hides pixel/alpha behavior behind a compressed artifact.

### SoftwareBitmap as canonical result

Provides an uncompressed, CPU-readable Windows image object with explicit pixel format and alpha mode. It can be shown in WinUI through `SoftwareBitmapSource`, encoded through `BitmapEncoder`, and converted to or from rendering/capture surfaces.

## Accepted Decision

The canonical SnipPlus image result is an immutable, owned **`SoftwareBitmap` in `BitmapPixelFormat.Bgra8` with `BitmapAlphaMode.Premultiplied`**.

Initial color policy：

- Working color space：sRGB SDR.
- Channel order：BGRA.
- Channel depth：8 bits per channel.
- Alpha：premultiplied.
- Orientation：top-left logical origin; no hidden orientation transform.
- Row stride：must be recorded; consumers must not assume `width * 4` unless the contract verifies it.

The Shared Result contract wraps the bitmap and metadata. It does not expose mutable pixel access after publication.

## Required Metadata

- Result ID.
- Width and height in physical pixels.
- Pixel format and alpha mode.
- Row stride.
- Logical DPI X/Y used for display metadata.
- Source monitor/window identity category without private title data.
- Source physical-pixel bounds.
- Crop physical-pixel bounds.
- Capture timestamp.
- Color-space policy (`sRGB SDR` for v1).
- Cursor included (`false` for initial scope).
- Annotation/rendering state version.
- Ownership/disposal state.

## Ownership and Lifetime

1. Capture and Rendering may use temporary Direct3D or Win2D resources internally.
2. Before publishing a successful Shared Result, the pipeline converts/copies into the canonical SoftwareBitmap.
3. The result becomes immutable when published.
4. Shared Result owns the SoftwareBitmap and disposes it when the workflow releases the result.
5. Clipboard and Output must create their own stream/encoded data during the valid result lifetime or take an explicitly owned copy.
6. A disposed result cannot be reused or reported as success.
7. Failed or cancelled workflows must dispose temporary and canonical image resources.

## Boundary Rules

- Domain contracts may reference an abstract `ImageResult`, not concrete Win2D or Direct3D types.
- Platform adapters may convert WGC surfaces into the canonical representation.
- Rendering adapters may create temporary CanvasBitmap／render targets but publish only the canonical result.
- Clipboard and Output consume an immutable result and cannot mutate Shared State or pixel data.
- PNG is a delivery encoding, not the in-memory domain representation.

## HDR and Wide Color

HDR/wide-color preservation is Deferred. Initial behavior converts or rejects unsupported source formats into the declared sRGB SDR BGRA8 result according to a verified conversion path.

The implementation must not silently label an unverified HDR conversion as faithful. Source format and conversion outcome must be observable in diagnostics without storing private image content.

## Trade-offs

### Benefits

- Deterministic and testable pixel contract.
- Straightforward WinUI display and Windows encoder compatibility.
- Clear disposal and downstream lifetime.
- Decouples Capture／Rendering devices from Clipboard／Output.
- Simple initial implementation.

### Costs

- Requires GPU-to-CPU readback/copy for WGC/Win2D output.
- Uses uncompressed system memory.
- May be less efficient for large or continuous capture workloads.
- HDR/wide-color is not preserved in the initial baseline.

### Neutral consequences

- A future GPU-backed cache can be added as an internal optimization without changing canonical semantics.
- PNG encoding remains an Output/Clipboard operation.
- Annotation object representation remains separate from the flattened result.

## Verification Requirements

- Exact BGRA channel values for a synthetic color pattern.
- Premultiplied-alpha edge cases.
- Width、height and stride validation.
- WGC surface to SoftwareBitmap crop fidelity.
- Win2D render-target to SoftwareBitmap fidelity.
- SoftwareBitmapSource display.
- PNG encode/decode pixel comparison.
- Clipboard round-trip through at least two representative consumers.
- Disposal and use-after-dispose failure behavior.
- Large-image memory boundary and cleanup.

## External Evidence

| Source | Evidence used |
| --- | --- |
| [SoftwareBitmapSource](https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.media.imaging.softwarebitmapsource) | WinUI display requires BGRA with premultiplied alpha or no alpha. |
| [BitmapEncoder.SetSoftwareBitmap](https://learn.microsoft.com/en-us/uwp/api/windows.graphics.imaging.bitmapencoder.setsoftwarebitmap) | Windows encoder accepts BGRA8 SoftwareBitmap input. |
| [Win2D pixel formats](https://learn.microsoft.com/en-us/windows/apps/develop/win2d/pixel-formats) | BGRA8 with premultiplied alpha is the normal Win2D default. |
| [Create/edit/save bitmap images](https://learn.microsoft.com/en-us/windows/apps/develop/media-authoring-processing/imaging) | Microsoft documents BGRA8 premultiplied SoftwareBitmap creation and XAML usage. |

## Review Record

| Field | Value |
| --- | --- |
| Reviewer | ChatGPT repository review |
| Review date | 2026-07-26 |
| Review result | Approved |
| Open comments | HDR/wide-color deferred; performance must be verified |
| Resolution | Selected a deterministic first-slice representation and preserved future optimization boundary |
| Acceptance authority | Repository owner through explicit instruction to continue toward coding readiness |

## Change and Supersession

A new ADR is required if the canonical semantics change to a GPU-only、high-bit-depth、wide-color or encoded representation. Internal caches and adapters do not supersede this decision if the published contract remains equivalent.

## Implementation State

| Artifact | Status |
| --- | --- |
| Contract implementation | Not implemented |
| Runtime verification | Not verified |
| Coding authorized | No |

## Non-goals

This ADR does not create code、select encoder settings beyond initial PNG compatibility、define persistence、or authorize Build／Run／Capture.
