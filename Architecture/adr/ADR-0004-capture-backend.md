# ADR-0004 Capture Backend

## Document Control

| Field | Value |
| --- | --- |
| Document ID | ADR-0004 |
| Title | Capture Backend |
| Status | Accepted |
| Decision Category | Platform Integration |
| Version | 1.0 |
| Owner | Repository owner |
| Date proposed | 2026-07-26 |
| Date reviewed | 2026-07-26 |
| Date accepted | 2026-07-26 |
| Supersedes | None |
| Superseded by | None |
| Normative References | ADR-0002、ADR-0003、PRD-0002、PRD-0003、PRD-0004、PRD-0006、SPEC-0005、SPEC-0006、SPEC-0010、ARCH-0002、ARCH-0003、ARCH-0004、ARCH-0005、ARCH-BASELINE-REVIEW、ADR-BASELINE |
| Informative References | RESEARCH-TECH-CAPTURE-001 through RESEARCH-TECH-CAPTURE-009、official Microsoft Windows.Graphics.Capture and DXGI Desktop Duplication documentation |

## Context

SnipPlus requires a Windows capture source that can provide a frame for a selected region while preserving the Frozen ownership boundaries:

- Workflow and Selection own capture intent and selection state.
- Platform Integration owns source acquisition and platform failures.
- Rendering owns visual presentation and raster effects, not desktop acquisition.
- Shared Result／Image Representation owns the result passed downstream.
- Clipboard and Output are parallel consumers after capture completion.

Capture Research 20–28 compared Windows Graphics Capture、DXGI Desktop Duplication、GDI-based capture、window-oriented mechanisms and hybrid strategies. The research did not run product capture code, but it established official API identities、coordinate risks、security boundaries and required verification.

ADR-0002 and ADR-0003 now provide the WinUI 3 host and rendering boundary needed to make a bounded capture decision.

## Decision Drivers

| Driver | Priority | Source |
| --- | --- | --- |
| Official WinUI 3／Windows desktop support | High | ADR-0002、Microsoft screen-capture documentation |
| Display or window frame acquisition | High | SPEC-0005、PRD-0004 |
| Secure OS-defined capture boundary | High | PRD-0006、Capture Research |
| Snapshot and short-lived frame acquisition | High | SPEC-0005、SPEC-0006 |
| Explicit monitor/window source identity | High | ARCH-0004、Capture Research |
| GPU-surface interoperability with rendering | High | ADR-0003 |
| Failure、resize and device lifecycle | High | SPEC-0006、ARCH-0005 |
| Avoid low-level native duplication complexity in the initial slice | Medium | ARCH-0001 |
| Cross-platform capture | Low | Frozen PRD is Windows-first |

## Options Considered

### Option A — Windows.Graphics.Capture

Use `Windows.Graphics.Capture` with a `GraphicsCaptureItem` representing a monitor or window and a `Direct3D11CaptureFramePool` producing frames.

**Advantages**

- Official Windows desktop capture API for display/window frames and snapshots.
- Documented WinUI 3 considerations.
- Integrates with Direct3D surfaces and the Accepted rendering boundary.
- Supports monitor/window item creation through desktop interop.
- Defines frame-pool recreation and session lifecycle behavior.

**Disadvantages**

- Region capture is not a first-class arbitrary rectangle source; SnipPlus must crop a monitor frame using an explicit coordinate contract.
- Overlay exclusion、DPI mapping、HDR behavior and first-frame timing require runtime verification.
- Protected and secure content may be unavailable or blank by platform design.

### Option B — DXGI Desktop Duplication

Use `IDXGIOutputDuplication` for each output and acquire desktop frames directly.

**Advantages**

- Detailed per-output frame、dirty/move rectangle and cursor metadata.
- Direct GPU desktop surface access.
- Explicit output and rotation model.

**Disadvantages**

- Greater Direct3D／DXGI native interop and device-lifecycle complexity.
- Per-output composition and multi-monitor stitching remain application responsibilities.
- Access loss、mode change and adapter/output matching require substantial recovery logic.
- Better aligned with continuous collaboration/streaming than the initial one-shot product path.

### Option C — GDI BitBlt

Use desktop/window device contexts and `BitBlt`.

**Advantages**

- Mature API and simple basic examples.
- CPU-readable bitmap path.

**Disadvantages**

- Weak fit for modern GPU surfaces、HDR/color handling and future fidelity requirements.
- Official documentation states `BitBlt` does not perform color management.
- Layered windows、DPI and modern composition behavior add risk.
- Not selected as a primary modern Windows capture path.

### Option D — Hybrid primary/fallback

Select WGC as primary with an immediately implemented DXGI or GDI fallback.

**Advantages**

- Potentially broadens environment coverage.

**Disadvantages**

- Multiplies capture、coordinate、failure、privacy and verification paths before a verified need exists.
- Makes the first vertical slice significantly harder to reason about and test.

## Accepted Decision

Use **Windows.Graphics.Capture (WGC)** as the sole initial Capture Backend.

1. The backend creates a `GraphicsCaptureItem` for an approved monitor or window source.
2. The initial region-capture path uses a monitor item, acquires one usable frame, and crops the selected region using a separately defined physical-pixel coordinate contract.
3. `GraphicsCapturePicker` may be used for user-selected window/monitor flows. Direct monitor/window creation through `IGraphicsCaptureItemInterop` may be used when the product workflow already owns an explicit approved source handle.
4. The frame pool and capture session are short-lived for a one-shot capture unless a later accepted requirement needs continuous acquisition.
5. Capture frame processing may occur away from the UI thread. Composition visual updates remain on the WinUI UI thread.
6. Cursor capture is disabled by default for the initial screenshot result. A later explicit product setting may change this.
7. Product overlay windows must be excluded from capture when supported and must also follow an explicit hide／settle／capture timing contract. `WDA_EXCLUDEFROMCAPTURE` is a defense-in-depth mechanism, not a security guarantee.
8. Protected content、secure desktop、unsupported hardware、permission denial、closed source、frame-pool resize and device/session failures return classified failures; they never become blank successful results.
9. DXGI Desktop Duplication is not implemented as an initial fallback. A verified WGC blocker may trigger a new ADR or an explicit extension of this decision.
10. GDI capture is rejected as the primary and fallback path for the initial product baseline.

## Initial Scope

Included：

- One monitor source.
- One-shot frame acquisition.
- Region crop from the monitor frame.
- Explicit physical-pixel source bounds and crop bounds.
- Cursor excluded.
- Overlay exclusion／hide coordination.
- Cancellation and classified failure.
- Cleanup and frame/session disposal.

Deferred：

- Multi-monitor stitched capture.
- Window capture as a product mode.
- Scrolling capture.
- Continuous recording.
- Audio capture.
- HDR-preserving output.
- Secure desktop or protected-content access.
- DXGI fallback.

## Ownership Boundary

| Concern | Owner |
| --- | --- |
| Capture command and workflow state | Feature Coordination |
| Selection geometry in host DIPs | Selection capability |
| Monitor/window source identity | Platform Integration／Capture adapter |
| Source frame acquisition and lifecycle | WGC Capture Backend |
| DIP-to-physical-pixel conversion | Shared coordinate contract using Platform Display Context |
| Region crop | Capture／Image boundary according to the contract |
| Display and annotation rendering | ADR-0003 rendering adapter |
| Shared Result／Image Result | Image Representation contract |
| Clipboard and Output delivery | Separate downstream capabilities |

The backend must not mutate Shared State directly、own selection state、render UI、publish Clipboard content or write output files.

## Coordinate Contract Requirements

The implementation contract must record：

- Virtual desktop origin.
- Selected `HMONITOR` identity and physical-pixel bounds.
- Host DIP selection rectangle.
- DPI scale used for conversion.
- Physical-pixel crop rectangle relative to the captured monitor frame.
- Inclusive／exclusive edge convention.
- Rounding policy.
- Source frame size and timestamp.
- Topology/DPI version used by the conversion.

A stale or invalid mapping must return a failure and request a new selection/capture cycle.

## Failure Categories

At minimum：

- `Unsupported`
- `PermissionDenied`
- `SourceUnavailable`
- `SourceClosed`
- `InvalidCoordinateMapping`
- `FrameTimeout`
- `FrameSizeChanged`
- `DeviceLost`
- `SessionChanged`
- `ProtectedContentUnavailable`
- `Cancelled`
- `UnexpectedFailure`

Recoverable versus terminal behavior belongs in the consolidated failure contract.

## Trade-offs

### Benefits accepted

- Native Windows capture path aligned with WinUI 3.
- Lower initial complexity than direct DXGI duplication.
- Direct3D-surface boundary can integrate with the selected renderer and image contract.
- Secure platform behavior and user-visible capture boundary remain intact.
- One backend keeps the first vertical slice testable.

### Costs accepted

- Region capture requires monitor-frame crop and rigorous coordinate conversion.
- Overlay timing and exclusion require verification.
- WGC frame/device lifecycle must be handled explicitly.
- Some sources and protected content may be unavailable.
- A future verified limitation may require a DXGI fallback ADR.

### Neutral consequences

- Image Representation remains a separate decision.
- The accepted backend does not select a D3D device wrapper、language or package version.
- Window capture capability exists at the API level but is Deferred as a product mode.
- Runtime success is not claimed by this document.

## Verification Requirements

The first authorized vertical slice must verify：

- `GraphicsCaptureSession.IsSupported()` handling.
- WinUI 3 host and source-item creation.
- One-shot frame acquisition from a synthetic/public monitor scene.
- Cursor exclusion.
- Overlay exclusion and hide/capture timing.
- DIP-to-physical-pixel conversion.
- Negative virtual-screen coordinates where available.
- Exact crop edges against a known synthetic pattern.
- Frame-size change and source-closed handling.
- Device/session cleanup.
- No retained screenshot evidence outside the approved test boundary.

## Traceability

| Source | Relevance |
| --- | --- |
| ADR-0002 | Accepted WinUI 3 host |
| ADR-0003 | Accepted rendering and Direct3D/Win2D boundary |
| PRD-0004 | Core capture workflow |
| PRD-0006 | Platform、privacy、DPI and reliability requirements |
| SPEC-0005 | Capture trigger、selection、completion and cancellation |
| SPEC-0006 | Failure、cancel and cleanup behavior |
| ARCH-0003／0004／0005 | Capture module、component and interaction ownership |
| RESEARCH-TECH-CAPTURE-001 through 009 | Candidate evidence、gaps and verification plan |
| TD-003 | Roadmap item completed by this ADR |

## External Evidence

| Source | Evidence used |
| --- | --- |
| [Microsoft screen capture](https://learn.microsoft.com/en-us/windows/apps/develop/media-authoring-processing/screen-capture) | WGC acquires frames from a display or window for streams or snapshots; WinUI 3 integration and frame-pool recreation are documented. |
| [IGraphicsCaptureItemInterop](https://learn.microsoft.com/en-us/windows/win32/api/windows.graphics.capture.interop/nn-windows-graphics-capture-interop-igraphicscaptureiteminterop) | Desktop interop creates capture items for monitor or window handles. |
| [GraphicsCaptureSession.IsCursorCaptureEnabled](https://learn.microsoft.com/en-us/uwp/api/windows.graphics.capture.graphicscapturesession.iscursorcaptureenabled) | Capture session can include or exclude the cursor. |
| [SetWindowDisplayAffinity](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowdisplayaffinity) | `WDA_EXCLUDEFROMCAPTURE` is supported on modern Windows but is not DRM or an absolute security guarantee. |
| [Microsoft Desktop Duplication API](https://learn.microsoft.com/en-us/windows/win32/direct3ddxgi/desktop-dup-api) | DXGI duplication remains a lower-level alternative with per-output GPU surfaces and metadata. |

## Implementation and Verification State

| Artifact | Status |
| --- | --- |
| Implementation reference | Not implemented |
| Runtime verification | Not verified |
| Coordinate fidelity evidence | Not verified |
| Overlay exclusion evidence | Not verified |
| Coding authorized | No |

## Review Record

| Field | Value |
| --- | --- |
| Reviewer | ChatGPT repository review |
| Review date | 2026-07-26 |
| Review result | Approved |
| Review basis | Frozen PRD／Specs／Architecture、ADR-0002、ADR-0003、RESEARCH-TECH-CAPTURE-001 through 009 and current official Microsoft documentation |
| Open comments | None blocking the bounded initial backend decision |
| Resolution of comments | Selected one primary backend、deferred fallback and product modes、converted runtime unknowns into verification requirements |
| Acceptance authority | Repository owner through explicit instruction to continue documentation convergence toward coding readiness |

## Change and Supersession

Revisit or supersede if：

- WGC fails a required verified scenario.
- A Frozen requirement adds continuous capture、recording or unsupported source types.
- A DXGI fallback becomes necessary.
- The minimum supported Windows version no longer supports the selected interop path.
- Architecture changes capture ownership.

## Acceptance Verification

| Check | Result |
| --- | --- |
| Single major decision | PASS |
| Accepted upstream ADRs | PASS |
| Official API evidence | PASS |
| Initial and deferred scope explicit | PASS |
| Coordinate and failure boundaries explicit | PASS |
| Security bypass prohibited | PASS |
| Runtime limitations explicit | PASS |
| Frozen ownership preserved | PASS |
| Coding authorized | No |

## Non-goals

This ADR does not：

- Create source code or a project.
- Invoke capture APIs.
- Capture or retain desktop pixels.
- Select Image Representation、Clipboard、Output、Language、Runtime、SDK or package versions.
- Restore、Build、Run or Test.
- Modify Frozen PRD、Specs or Architecture ownership.
