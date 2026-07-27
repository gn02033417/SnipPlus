# ADR-0006 Clipboard Integration

## Document Control

| Field | Value |
| --- | --- |
| Document ID | `ADR-0006` |
| Title | Clipboard Integration |
| Status | `Accepted` |
| Decision category | Clipboard／Platform Integration |
| Version | `1.1` |
| Owner | Repository owner |
| Date accepted | `2026-07-26` |
| Last reviewed | `2026-07-27` |
| Supersedes | None |
| Superseded by | None |
| Normative references | Accepted PRD／Specs、Architecture baseline、ADR-0002、ADR-0005 |

## Context

SnipPlus v1 publishes Clipboard content only after an explicit user commitment:

- Complete; or
- Save after PNG creation succeeds.

Clipboard publication is not triggered by PrintScreen、frame freeze、mouse release、selection adjustment、Annotation edits、Save As cancellation or Cancel.

The Clipboard adapter must accept one immutable final rendered image without owning Capture、Selection、Annotation、PNG Output or shared workflow state.

## Options considered

### WPF Clipboard wrapper

Rejected because the accepted host is WinUI 3 and adding WPF only for Clipboard creates unnecessary framework coupling.

### WinRT DataPackage Clipboard

Native Windows Runtime path aligned with WinUI 3. Supports bitmap streams、history／roaming controls and Flush semantics.

### Win32 OLE IDataObject

Powerful multi-format model, but adds COM ownership and interoperability complexity before required consumer evidence exists.

### Raw Win32 Clipboard APIs

Maximum format control, but requires manual global-memory and contention handling and is not justified by current requirements.

## Decision

Use `Windows.ApplicationModel.DataTransfer.DataPackage` and `Clipboard.SetContentWithOptions` as the v1 Clipboard adapter.

1. Encode the immutable final result as an in-memory PNG stream.
2. Publish through `DataPackage.SetBitmap(RandomAccessStreamReference)`.
3. Set `RequestedOperation = Copy`.
4. Set `IsAllowedInHistory = false` by default.
5. Set `IsRoamable = false` by default.
6. Execute the required publication operation on the appropriate WinUI／Windows apartment.
7. Call `Clipboard.Flush()` after successful publication so payload lifetime is independent of transient capture UI resources.
8. Treat rejected publication or Clipboard contention as failure, never success.
9. Apply a bounded、cancellable retry policy: at most five attempts within one second unless a later accepted contract supersedes that limit.
10. Do not clear existing Clipboard content before a successful replacement.
11. Return a typed delivery result containing Delivery ID、Session ID and Result ID.
12. The adapter never mutates shared workflow state or declares the SnipPlus session complete.

## Commitment boundary

### Complete

```text
User chooses Complete
→ freeze current Selection／Annotation revisions
→ render final image
→ publish Clipboard
→ on success cleanup and restore focus
→ on recoverable failure return to Editing
```

Complete creates no file.

### Save

```text
User chooses Save
→ render final image
→ Save As and PNG creation succeed
→ publish the same Result ID to Clipboard
→ only after Clipboard success may Save complete
```

Save As cancellation and PNG failure do not publish Clipboard.

When Clipboard fails after PNG creation:

- the workflow remains in Editing;
- the failure is disclosed;
- the current Editing state is preserved;
- the file retention／rollback behavior remains an explicit unresolved product decision and must not be guessed by the adapter.

## Privacy defaults

- Clipboard History eligibility is disabled.
- Cross-device roaming is disabled.
- No private path、window title、account identifier、display handle or Annotation metadata is published.
- Real Clipboard payloads are not committed as test evidence.
- History or roaming opt-in requires a later explicit product/settings decision.

## Ownership and lifetime

- Input is one immutable canonical final image result.
- The delivery request carries current Session ID and Result ID.
- The PNG stream remains valid through publication and Flush.
- Temporary stream resources are disposed after delivery outcome is known.
- Recoverable failure may retain the final image through the Editing session for retry.
- Completion or terminal cleanup disposes retained results according to the session owner.
- A stale delivery outcome cannot advance a newer or cancelled session.

## Failure categories

At minimum:

- Clipboard busy／contention;
- publication rejected;
- PNG stream encoding failed;
- invalid or stale result identity;
- cancellation;
- unsupported platform behavior;
- unexpected failure.

Failure handling does not overwrite the user’s previous Clipboard content unless the replacement has actually succeeded.

## Verification requirements

- Successful publication and paste into representative Windows consumers.
- Alpha behavior and PNG payload fidelity.
- Flush lifetime after capture UI cleanup.
- History and roaming defaults.
- Bounded retry success、budget exhaustion and cancellation.
- Large-image resource cleanup.
- No publication before Complete or successful PNG creation in Save.
- Recoverable failure returns to Editing with the same revisions.
- Stale result／session callbacks cannot complete another session.

## Current implementation state

- WinRT DataPackage publication、PNG stream encoding、Flush、privacy defaults and bounded retry are implemented and covered by deterministic tests.
- Current historical workflow invokes Clipboard immediately after mouse release, which is an incorrect product placement.
- The adapter is a conforming reusable technical foundation; workflow integration must move it behind explicit Complete and Save commitments.

## Deferred compatibility path

A later verified consumer-compatibility problem may justify a superseding ADR for Win32 OLE or multi-format publication. That path is not implemented speculatively.
