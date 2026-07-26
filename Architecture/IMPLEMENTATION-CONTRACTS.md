# SnipPlus Implementation Contracts

## Document Control

| Field | Value |
| --- | --- |
| Document ID | IMPLEMENTATION-CONTRACTS-001 |
| Status | Accepted |
| Version | 1.0 |
| Owner | Repository owner |
| Date accepted | 2026-07-26 |
| Scope | First vertical slice and stable cross-project information boundaries |
| Normative References | Frozen PRD／Specs、ARCH-0002 through ARCH-0005、ADR-0002 through ADR-0007 |
| Implementation authorized | No; authorization is owned by the Implementation Readiness Review |

## 1. Purpose

This document resolves the information-format and lifecycle gaps identified by ARCH-0004 without changing Module or Component ownership.

It defines the minimum semantic contracts required to implement:

- Capture request and selection.
- Windows Graphics Capture acquisition.
- Canonical image result.
- Rendering intent and raster output.
- Clipboard delivery.
- Output delivery.
- Workflow transitions.
- Cancellation、failure、retry and cleanup.

A code implementation may use records、interfaces or classes, but it must preserve these semantics and dependency directions.

## 2. Contract Principles

1. `COMP-001` remains the only Workflow State Authority.
2. Other components return outcomes or submit transition requests; they never mutate shared state directly.
3. Domain contracts do not expose WGC、Win2D、Composition、DataPackage or file-system implementation types, except the Windows-specific canonical bitmap explicitly accepted by ADR-0005.
4. Clipboard and Output remain parallel downstream operations.
5. Annotation remains optional.
6. Cancellation is cooperative and idempotent.
7. Every owned image、stream、capture session and rendering resource has one disposal owner.
8. Empty or blank data is never converted into a successful result when an error is known.
9. Platform errors are translated into stable failure codes before crossing into Feature Coordination.
10. Logs and diagnostics contain metadata only by default, not captured pixels or Clipboard payloads.

## 3. Workflow State Contract

### 3.1 Shared states

```text
Idle
  -> Starting
  -> Selecting
  -> Capturing
  -> ResultReady
  -> Delivering
  -> Completed
```

Terminal alternatives from any permitted state:

- `Cancelled`
- `Failed`

`Completed`、`Cancelled` and `Failed` transition back to `Idle` only through explicit session cleanup.

### 3.2 Legal transition requests

| From | To | Request owner |
| --- | --- | --- |
| Idle | Starting | Session Lifecycle |
| Starting | Selecting | Capture Request |
| Selecting | Capturing | Selection Boundary |
| Selecting | Cancelled | Selection／Session Lifecycle |
| Capturing | ResultReady | Capture Result Boundary |
| Capturing | Cancelled | Session Lifecycle |
| Capturing | Failed | Failure Classification |
| ResultReady | Delivering | Feature Flow Coordinator |
| ResultReady | Completed | Feature Flow Coordinator when no delivery is requested |
| Delivering | Completed | Completion Boundary |
| Delivering | Failed | Failure Classification only when the selected required delivery fails |
| Delivering | ResultReady | Feature Flow Coordinator when a retryable downstream delivery fails and the image remains valid |
| Completed／Cancelled／Failed | Idle | Session cleanup |

Clipboard failure does not invalidate a valid `ImageResult`; it normally returns the workflow to `ResultReady` for retry or another downstream action.

## 4. CaptureIntent

Producer：`COMP-003`／`COMP-004`  
Consumer：`COMP-014`

Required fields：

| Field | Meaning |
| --- | --- |
| RequestId | Unique request correlation ID |
| SessionId | Owning workflow session |
| SourceKind | `Monitor` for the first vertical slice; `Window` reserved |
| SourceId | Stable opaque source identifier for the request lifetime |
| SourcePhysicalBounds | Physical-pixel rectangle of the selected monitor/source |
| SelectionDipBounds | User selection in host DIPs |
| SelectionPhysicalBounds | Converted virtual-screen physical-pixel rectangle |
| CropBoundsInSource | Physical-pixel rectangle relative to the WGC source frame |
| DpiScaleX／DpiScaleY | Conversion scale used |
| CoordinateVersion | Display-context snapshot/version |
| IncludeCursor | `false` for the first slice |
| RequestedAt | UTC timestamp |
| Cancellation | Cooperative cancellation token/context |

Invariants：

- All rectangles use left/top inclusive and right/bottom exclusive edges.
- Width and height must be positive.
- `CropBoundsInSource` must be wholly inside the current source frame.
- A changed display topology or DPI context invalidates the intent.
- Conversion uses physical pixels for capture and crop; DIPs are never passed directly to WGC.

## 5. CaptureOutcome

Producer：`COMP-014`  
Consumer：`COMP-004`／`COMP-006`

Exactly one variant：

### Success

- RequestId／SessionId.
- Source frame width、height and format.
- Source physical bounds.
- Crop bounds used.
- Capture timestamp.
- Canonical `ImageResult`.
- Warnings that do not invalidate the image.

### Cancelled

- RequestId／SessionId.
- Cancellation origin.
- Whether source/session creation had started.
- Cleanup completed flag.

### Failure

- RequestId／SessionId.
- Stable `Failure` contract.
- Cleanup completed flag.
- Whether retry requires a new selection/source.

WGC frame、frame-pool、capture-session and temporary GPU resources do not cross this boundary.

## 6. ImageResult

Producer：`COMP-006` or rendering completion  
Consumers：`COMP-007`、`COMP-009`、`COMP-010` and presentation adapter

Normative representation follows ADR-0005：

- Immutable owned `SoftwareBitmap`.
- `Bgra8`.
- Premultiplied alpha.
- sRGB SDR.

Required metadata：

| Field | Meaning |
| --- | --- |
| ResultId | Unique result identity |
| SessionId | Owning session |
| PixelWidth／PixelHeight | Physical-pixel dimensions |
| PixelFormat | `Bgra8` |
| AlphaMode | `Premultiplied` |
| ColorSpace | `sRGB SDR` |
| DpiX／DpiY | Display/encoding metadata, not coordinate conversion authority |
| RowStride | Actual row stride |
| SourceKind | Monitor/window category |
| SourcePhysicalBounds | Original source bounds |
| CropPhysicalBounds | Region represented by the result |
| CapturedAt | UTC timestamp |
| CursorIncluded | `false` initially |
| ContentVersion | Increments when a new flattened annotated result replaces the previous version |
| IsDisposed | Lifetime guard |

Ownership rules：

- Shared Result owns the canonical bitmap.
- Consumers receive a read-only lease valid for the operation duration.
- Clipboard／Output create and own temporary streams.
- Disposal is idempotent.
- Access after disposal returns `InvalidResultLifetime` rather than undefined behavior.
- A replacement result disposes the old result only after active leases complete or are cancelled.

## 7. RenderIntent and RenderOutcome

Producer：Selection／Annotation／Presentation coordination  
Consumer：ADR-0003 rendering adapter

### RenderIntent

- SceneId and ContentVersion.
- Canvas logical size.
- Logical-to-output transform.
- Ordered render nodes.
- Background policy: transparent or explicit synthetic/result background.
- Clip bounds.
- Target: `Display` or `CanonicalRaster`.
- Cancellation.

Minimum render node semantics：

- Rectangle／line／path geometry.
- Text run with font family、size、weight、alignment and fallback policy.
- Image node referencing an ImageResult lease.
- Selection rectangle and handles.
- Mosaic region.
- Opacity、transform、clip and z-order.

Hit-test intent is a separate semantic query over the same logical geometry; drawing success does not imply hit-test success.

### RenderOutcome

- Success with rendered scene metadata and optional new ImageResult for `CanonicalRaster`.
- Cancelled.
- Failure.

Display-only Composition resources never become a canonical ImageResult without an explicit raster operation.

## 8. ClipboardDeliveryRequest and Result

Producer：`COMP-009`  
Consumer：`COMP-015`

### Request

- DeliveryId／SessionId／ResultId.
- Valid ImageResult lease.
- History allowed: `false` initially.
- Roaming allowed: `false` initially.
- Maximum attempts: `5`.
- Total retry budget: `1 second`.
- Cancellation.

### Result

Exactly one：

- `Delivered`: SetContentWithOptions and Flush completed.
- `RetryableFailure`: image remains valid; includes Failure and attempts used.
- `TerminalFailure`: includes Failure.
- `Cancelled`.

The Clipboard adapter owns PNG stream encoding and disposal. It does not clear Clipboard contents before successful replacement.

## 9. OutputDeliveryRequest and Result

Producer：`COMP-010`  
Consumer：`COMP-016`

The first vertical slice may defer user file output, but the contract is fixed to preserve parallel downstream ownership.

### Request

- DeliveryId／SessionId／ResultId.
- ImageResult lease.
- Destination supplied by an explicit user action.
- Format: `Png` initially.
- Overwrite policy.
- Cancellation.

### Result

- `Delivered` with non-sensitive destination identifier.
- `RetryableFailure`.
- `TerminalFailure`.
- `Cancelled`.

Output failure does not alter Clipboard result, and Clipboard failure does not alter Output result.

## 10. Failure Contract

Required fields：

| Field | Meaning |
| --- | --- |
| Code | Stable code from the catalog |
| Category | `Validation`、`Unsupported`、`Permission`、`Contention`、`Resource`、`Device`、`Session`、`IO`、`Cancelled`、`Unexpected` |
| Severity | `Info`、`Warning`、`Error` |
| Recoverability | `RetrySameIntent`、`RetryNewIntent`、`UserActionRequired`、`TerminalForSession` |
| UserMessageKey | Localizable message key, not raw exception text |
| DiagnosticMessage | Redacted technical summary |
| NativeCode | Optional HRESULT/Win32 code |
| Operation | Owning operation |
| CorrelationId | Request／delivery identifier |
| OccurredAt | UTC timestamp |
| InnerFailure | Optional nested stable failure |

Minimum stable codes：

- `InvalidStateTransition`
- `InvalidCaptureIntent`
- `InvalidCoordinateMapping`
- `UnsupportedCapture`
- `CapturePermissionDenied`
- `CaptureSourceUnavailable`
- `CaptureSourceClosed`
- `CaptureFrameTimeout`
- `CaptureFrameSizeChanged`
- `CaptureDeviceLost`
- `ProtectedContentUnavailable`
- `InvalidResultLifetime`
- `RenderingResourceLost`
- `RenderingFailed`
- `EncodingFailed`
- `ClipboardBusy`
- `ClipboardPublicationRejected`
- `OutputAccessDenied`
- `OutputWriteFailed`
- `Cancelled`
- `UnexpectedFailure`

Raw platform exceptions do not cross the contract boundary.

## 11. Retry Contract

- Retry is owned by the capability that understands the side effect.
- Feature Coordination may request retry but does not implement platform delay loops.
- Retry attempts are bounded by count and time.
- Cancellation interrupts waiting immediately.
- A retry never reuses stale display coordinates、closed capture items or disposed images.
- Unexpected exceptions are not retried automatically.
- Clipboard contention can retry the same ImageResult.
- Capture coordinate/device/session failures generally require a new intent or selection.

## 12. Async and Thread Boundary

- Public capability operations are asynchronous and cancellation-aware.
- Shared state transitions are serialized by COMP-001.
- WinUI XAML／Composition mutations and Clipboard publication execute on the UI dispatcher/apartment.
- WGC frame callbacks and GPU/CPU conversion may execute off the UI thread.
- Domain validation and coordinate calculations have no UI-thread dependency.
- File/PNG encoding may execute off the UI thread while holding a valid result lease.
- No blocking `.Result`／`.Wait()` is permitted in UI or coordination paths.

## 13. Cleanup Contract

A session is clean only when：

- Capture session/frame pool/frame are closed.
- Temporary Direct3D／Win2D resources are released.
- Temporary streams are disposed.
- Pending retries are cancelled.
- ImageResult is retained only if the workflow remains `ResultReady`; otherwise disposed.
- Overlay/input hooks owned by the session are removed.
- No temporary captured image remains on disk.
- Clipboard is not cleared as part of cleanup.
- Shared State has reached the declared terminal/idle transition.

Cleanup failure is recorded separately and must not overwrite the primary failure.

## 14. First Vertical Slice Contract Scope

Required：

- Explicit app command starts one session.
- One monitor and one region selection.
- WGC one-shot capture.
- Canonical SoftwareBitmap result.
- Display through ADR-0003 adapter.
- Clipboard publication through ADR-0006.
- Cancel、failure、retry and cleanup.
- Automated Unit、Contract and deterministic Rendering tests.
- Authorized Windows platform verification.

Deferred：

- Global Print Screen interception.
- Multi-monitor stitched capture.
- Window capture product mode.
- Annotation mutation tools.
- File Output UI.
- History/roaming opt-in.
- HDR preservation.
- Packaging/distribution hardening.

## 15. Acceptance Verification

| Check | Result |
| --- | --- |
| ARCH-0004 information boundaries resolved | PASS |
| COMP-001 remains sole state authority | PASS |
| Clipboard and Output remain parallel | PASS |
| Annotation remains optional | PASS |
| Coordinate spaces explicit | PASS |
| Image ownership and disposal explicit | PASS |
| Typed failure/retry contract explicit | PASS |
| Async/thread boundary explicit | PASS |
| First-slice scope explicit | PASS |
| Coding authorized by this document | No |
