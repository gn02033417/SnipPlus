# ADR-0006 Clipboard Integration

## Document Control

| Field | Value |
| --- | --- |
| Document ID | ADR-0006 |
| Title | Clipboard Integration |
| Status | Accepted |
| Decision Category | Clipboard / Platform Integration |
| Version | 1.0 |
| Owner | Repository owner |
| Date proposed | 2026-07-26 |
| Date reviewed | 2026-07-26 |
| Date accepted | 2026-07-26 |
| Supersedes | None |
| Superseded by | None |
| Normative References | ADR-0002、ADR-0005、PRD-0005、PRD-0006、SPEC-0006、SPEC-0007、SPEC-0010、ARCH-0002、ARCH-0003、ARCH-0004、ARCH-0005、ADR-BASELINE |
| Informative References | RESEARCH-TECH-CLIPBOARD-001 through RESEARCH-TECH-CLIPBOARD-052、official Windows DataTransfer Clipboard documentation |

## Context

The Clipboard path must deliver a completed immutable image without owning Capture、Rendering、Annotation、Output or Shared Workflow State. Research 29–80 accumulated official evidence and governance history but did not publish Clipboard content or create runtime evidence.

ADR-0005 now defines a canonical BGRA8 premultiplied `SoftwareBitmap`. The initial Clipboard path must be straightforward for WinUI 3、privacy-aware and independently testable.

## Options Considered

### WPF Clipboard wrapper

Rejected because the accepted host is WinUI 3 and adding WPF solely for Clipboard would create an unnecessary framework dependency.

### WinRT DataPackage Clipboard

Native Windows Runtime path aligned with WinUI 3. Supports bitmap streams、content options、history/roaming controls and flush semantics.

### Win32 OLE IDataObject

Powerful multi-format and delayed-rendering model, but adds COM ownership and more complex format/lifetime handling than the first slice requires.

### Raw Win32 Clipboard APIs

Maximum control over CF_DIBV5 and registered PNG formats, but requires explicit open/close、global-memory ownership and contention handling. Deferred until compatibility evidence requires it.

## Accepted Decision

Use `Windows.ApplicationModel.DataTransfer.DataPackage` and `Clipboard.SetContentWithOptions` as the initial Clipboard adapter.

1. Encode the immutable canonical result as an in-memory PNG stream.
2. Publish that stream through `DataPackage.SetBitmap(RandomAccessStreamReference)`.
3. Set `RequestedOperation = Copy`.
4. Use `ClipboardContentOptions` with:
   - `IsAllowedInHistory = false` by default.
   - `IsRoamable = false` by default.
5. Call `Clipboard.SetContentWithOptions` from the WinUI dispatcher/UI apartment.
6. If publication succeeds, call `Clipboard.Flush()` so content remains after application shutdown and the DataPackage is released from the source app.
7. Treat a `false` publication result or Clipboard-in-use exception as contention, not success.
8. Use a bounded retry policy owned by the Clipboard adapter: at most five attempts within one second, cancellation-aware, with increasing delays and no busy loop.
9. Return a typed delivery result to Feature Coordination; the adapter must not mutate Shared State directly.
10. Do not clear existing Clipboard content before a successful replacement.

## Privacy Defaults

- Clipboard History：disabled for SnipPlus-published image by default.
- Cross-device roaming：disabled by default.
- No application-specific private metadata, paths, window titles, account identifiers or source handles are placed in the DataPackage.
- The user may later opt into history/roaming only through an Accepted product/settings change.
- Clipboard is system-wide and cross-process; publication must be treated as intentional external disclosure.

## Data and Lifetime Boundary

- Input：immutable `ImageResult` from ADR-0005.
- Publication payload：owned in-memory PNG stream reference.
- The stream must remain valid through `SetContentWithOptions` and `Flush`.
- After flush and completion, temporary stream resources are disposed.
- The canonical SoftwareBitmap remains owned by Shared Result until workflow release.
- Clipboard completion is independent of File Output completion.

## Failure Categories

- `ClipboardBusy`
- `PublicationRejected`
- `EncodingFailed`
- `InvalidResult`
- `Cancelled`
- `Unsupported`
- `UnexpectedFailure`

A failure must preserve the completed image result so the user can retry or use Output. It must not report overall capture failure if capture and result creation already succeeded.

## Deferred Compatibility Path

If consumer verification shows that `DataPackage.SetBitmap` is insufficient for required applications, a later ADR may add a Win32 OLE adapter that publishes multiple equivalent formats, such as DIBV5 and a registered PNG format.

That path is not implemented speculatively.

## Verification Requirements

- Publish and paste into at least two representative Windows consumers.
- Alpha behavior through PNG/bitmap publication.
- Application shutdown after flush.
- History and roaming exclusion behavior where OS settings are enabled.
- Clipboard contention and bounded retry.
- Cancellation during retry.
- Large image behavior and memory cleanup.
- Clipboard success independent from Output failure and vice versa.

## External Evidence

| Source | Evidence used |
| --- | --- |
| [DataPackage.SetBitmap](https://learn.microsoft.com/en-us/uwp/api/windows.applicationmodel.datatransfer.datapackage.setbitmap) | Publishes a bitmap using a RandomAccessStreamReference. |
| [Clipboard.SetContentWithOptions](https://learn.microsoft.com/en-us/uwp/api/windows.applicationmodel.datatransfer.clipboard.setcontentwithoptions) | Publishes DataPackage content with history/roaming options and reports contention as failure. |
| [ClipboardContentOptions.IsAllowedInHistory](https://learn.microsoft.com/en-us/uwp/api/windows.applicationmodel.datatransfer.clipboardcontentoptions.isallowedinhistory) | Controls Clipboard History eligibility. |
| [ClipboardContentOptions.IsRoamable](https://learn.microsoft.com/en-us/uwp/api/windows.applicationmodel.datatransfer.clipboardcontentoptions.isroamable) | Controls cross-device roaming eligibility. |
| [Clipboard.Flush](https://learn.microsoft.com/en-us/uwp/api/windows.applicationmodel.datatransfer.clipboard.flush) | Releases DataPackage from the app and preserves content after shutdown. |
| [Clipboard formats](https://learn.microsoft.com/en-us/windows/win32/dataxchg/clipboard-formats) | Multiple formats may be added later if interoperability evidence requires them. |

## Trade-offs

### Benefits

- Native WinUI-compatible API.
- Explicit privacy controls.
- Simple lifecycle after `Flush`.
- No WPF、OLE or raw-memory dependency in the initial slice.
- Clear bounded retry and independent downstream failure.

### Costs

- Consumer compatibility depends on the WinRT bitmap publication path.
- PNG encoding adds memory and CPU cost.
- Advanced multi-format interoperability is Deferred.
- Runtime apartment and contention behavior must be verified.

## Review Record

| Field | Value |
| --- | --- |
| Reviewer | ChatGPT repository review |
| Review date | 2026-07-26 |
| Review result | Approved |
| Open comments | Multi-format OLE publication deferred pending evidence |
| Resolution | Selected the smallest native privacy-aware path and converted compatibility risk into verification |
| Acceptance authority | Repository owner through explicit instruction to continue toward coding readiness |

## Implementation State

| Artifact | Status |
| --- | --- |
| Adapter implementation | Not implemented |
| Runtime verification | Not verified |
| Coding authorized | No |

## Non-goals

This ADR does not read or clear Clipboard content、implement Clipboard History UI、publish private metadata、create code、or authorize Build／Run.
