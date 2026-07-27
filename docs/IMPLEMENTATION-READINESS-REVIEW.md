# SnipPlus Implementation Readiness Review

## Document Control

| Field | Value |
| --- | --- |
| Document ID | IMPLEMENTATION-READINESS-REVIEW-001 |
| Status | Accepted — corrective amendment recorded |
| Original review date | 2026-07-26 |
| Corrective amendment date | 2026-07-27 |
| Reviewer | ChatGPT repository review |
| Acceptance authority | Repository owner through explicit implementation and corrective instructions |
| Review scope | First vertical slice only |
| Original readiness decision | Approved for first vertical slice implementation |
| Current corrective decision | The first vertical slice must correct Region Selection visibility and same-frame consistency before any scope expansion. |
| Source-code creation | Already started and present in the repository |
| Build／test／runtime evidence | Present for the initial vertical slice; evidence does not override the failed Region Selection product behavior. |

## 1. Executive Decision

The original documentation was sufficient to begin a bounded technical vertical slice, but it omitted one product-critical behavior contract:

> The user must select a region while viewing the exact frozen source frame that will later be cropped.

The initial implementation therefore satisfied the weak wording “show a single-monitor region-selection surface” while presenting an opaque gray surface and acquiring the actual desktop frame only after selection. That behavior is not acceptable for a screenshot product and does not satisfy the intent of PRD-0002 or PRD-0004.

The corrected decision is:

- The existing capture、crop、render、Clipboard、cancellation and test foundations may be retained.
- The current Region Selection sequence is a blocking first-slice defect.
- The next implementation work must correct the sequence to `Capture one frame → present that frame → select → crop that same frame`.
- No Clipboard hardening、Packaging、Output、Annotation or other feature expansion should continue before this correction is complete.
- No new prerequisite、readiness、authorization or closure-document chain is required.

## 2. Reviewed Baselines

| Area | Effective source | Result |
| --- | --- | --- |
| Product intent | PRD v1.0 Freeze Review | PASS |
| Observable behavior | Specification v1.0 Baseline Review plus corrected SPEC-0005 v0.2 | PASS after corrective clarification |
| Abstract architecture | Architecture Baseline Review | PASS |
| UI Framework | ADR-0002 — WinUI 3 | PASS |
| Rendering | ADR-0003 — WinUI XAML／Composition + Win2D | PASS |
| Capture Backend | ADR-0004 — Windows.Graphics.Capture | PASS |
| Image Representation | ADR-0005 — BGRA8 premultiplied SoftwareBitmap | PASS |
| Clipboard | ADR-0006 — WinRT DataPackage Clipboard | PASS |
| Testing | ADR-0007 — MSTest.Sdk + Microsoft.Testing.Platform | PASS |
| Information contracts | IMPLEMENTATION-CONTRACTS-001 | PASS; frozen-frame ownership must be reflected by the correction |
| Project/toolchain baseline | PROJECT-STRUCTURE-001 | PASS |
| Repository governance | Repository Current State Audit、AGENTS、Development Guide | PASS after current-state correction |

## 3. Readiness Criteria

### 3.1 Product and behavior

The first vertical slice must preserve all of the following:

- Primary workflow and success／cancel／failure boundaries are frozen.
- Clipboard and Output remain parallel downstream paths.
- Annotation remains optional.
- Privacy requires explicit user action before capture.
- Region Selection must show the source content being selected.
- A single immutable full-monitor source frame must be acquired before Region Selection begins.
- Selection presentation and final Crop must use that exact same frame.
- Selection completion must not trigger a second desktop capture for the result.
- Selection outside area may be dimmed; the selected source content must remain clearly visible.
- Normal product operation must not launch Paint or another external GUI fixture.
- First-slice non-goals remain explicit.

Result：`PASS after corrective clarification; implementation correction required`

### 3.2 Architecture and ownership

- Layer、Module and Component ownership remains frozen.
- COMP-001 remains the sole Workflow State Authority.
- Domain and platform dependencies remain separated.
- Component-to-project mapping remains acyclic.
- Rendering does not own Capture、Clipboard、Output or workflow state.
- Platform adapters do not own product semantics.
- Frozen source-frame ownership must have one explicit lifetime across acquisition、selection、crop、cancel、failure and cleanup.

Result：`PASS`

### 3.3 Technology decisions

All implementation-critical P0 decisions for the first vertical slice remain Accepted：

- UI Framework.
- Rendering Technology.
- Capture Backend.
- Image Representation.
- Clipboard Integration.
- Testing Strategy.

The Region Selection defect does not require replacing WinUI 3、Windows.Graphics.Capture、SoftwareBitmap、Win2D or WinRT Clipboard. It requires correcting workflow order and presentation ownership.

Configuration、Logging、Packaging hardening、Update、Telemetry and Plugin decisions remain Deferred／P1／P2.

Result：`PASS`

### 3.4 Contracts

The following boundaries remain valid：

- Workflow state and legal transitions.
- CaptureIntent and CaptureOutcome.
- Coordinate spaces and crop rules.
- Canonical immutable ImageResult.
- RenderIntent and RenderOutcome.
- Clipboard and Output request/result independence.
- Stable Failure and retry semantics.
- Async/thread boundaries.
- Ownership、disposal and cleanup.

The correction must additionally guarantee：

- One source-frame acquisition per completed selection session.
- Region Selection receives a presentable view of that frame.
- Crop consumes that same frame rather than requesting a later frame.
- Cancellation and failure release the frame exactly once.

Result：`PASS after corrective clarification`

### 3.5 Toolchain and project structure

Fixed baseline：

- C# 14.
- .NET SDK 10.0.302.
- `net10.0-windows10.0.26100.0`.
- Windows App SDK 2.3.1 stable.
- Win2D 1.4.0.
- MSTest.Sdk 4.1.0 with MTP.
- x64.
- Packaged framework-dependent single-project MSIX development model.
- Four source projects and three test projects.
- Locked restore and central package management.

Result：`PASS`

### 3.6 Verification plan

Required verification for the corrective slice：

- A deterministic fake or stub source proves that one workflow performs exactly one full-frame acquisition.
- Region Selection is initialized only after a source frame exists.
- Crop is performed against that same frame.
- Zero-size、out-of-bounds and display-context mismatch selections cannot succeed.
- Cancel and failure release the frozen frame and selection resources.
- Clipboard receives only the cropped result.
- Non-interactive tests do not start external GUI applications.
- Interactive packaged verification occurs only with explicit user authorization.
- Runtime verification must not persist private desktop screenshots or Clipboard payloads.

Result：`PASS as corrected plan; corrected implementation evidence pending`

## 4. Non-blocking Unknowns and Deferred Decisions

These remain outside the corrective slice：

- Public release minimum OS matrix beyond the first-slice Windows 11 24H2 baseline.
- ARM64.
- Global Print Screen or system-wide hotkey interception.
- Multi-monitor stitched capture.
- Window capture as a product mode.
- Advanced annotation tools and undo／redo.
- File Output UI and destination policy.
- HDR／wide-color preservation.
- Logging framework and telemetry.
- Final packaging、signing、installer、Store and update strategy.
- Plugin architecture.
- Cloud、OCR、sharing and cross-platform capabilities.
- Exact overlay brand styling、opacity、border and resize affordance.

They must not be pulled into the correction merely because they remain open.

## 5. Corrected First Vertical Slice

### 5.1 Required scope

1. Retain the existing solution、configuration and seven approved projects.
2. Retain the packaged x64 WinUI 3 application shell.
3. Provide an explicit in-app command to start one capture session.
4. Resolve one single-monitor display-context snapshot.
5. Acquire one immutable full-monitor source frame before Region Selection begins.
6. Present that same frame as the Region Selection background.
7. Permit the area outside the selection to be dimmed while keeping the source content inside the selection clearly visible.
8. Accept pointer input in DIPs and convert it to physical-pixel bounds using the same display-context snapshot.
9. Crop the final result from that exact frozen source frame; do not capture a second desktop frame after selection.
10. Publish an immutable BGRA8 premultiplied SoftwareBitmap ImageResult.
11. Display the result through the ADR-0003 rendering adapter.
12. Copy the result through the ADR-0006 Clipboard adapter.
13. Support cancellation、classified failure、bounded Clipboard retry and complete frozen-frame cleanup.
14. Add or update Unit、Contract and deterministic tests for acquisition count、same-frame crop and cleanup.
15. Run category-filtered Windows platform verification only when explicitly authorized.
16. Record actual restore、build、test and runtime findings after the correction.

### 5.2 Explicit non-goals

- No global hotkey or Print Screen interception.
- No automatic／background capture.
- No multi-monitor stitched result.
- No window-capture product mode.
- No annotation mutation tools.
- No toolbar redesign beyond what is necessary to present and select the frozen frame.
- No save dialog or file Output UI.
- No Clipboard History／roaming opt-in.
- No HDR preservation.
- No DXGI or GDI fallback.
- No telemetry、cloud、OCR、plugin or update system.
- No release publication.

## 6. Corrective Implementation Sequence

1. Read corrected SPEC-0005、Implementation Contracts and the relevant App／Core／Windows code.
2. Stop unrelated Clipboard hardening、Packaging and feature expansion.
3. Introduce or expose one-shot full-frame acquisition before Region Selection.
4. Establish explicit frozen-frame ownership and cleanup.
5. Present the frozen frame in the WinUI Region Selection surface.
6. Keep the source content visible while rendering the selection mask and rectangle.
7. Convert Selection DIPs against the same display-context snapshot.
8. Crop the same frame without a second capture call.
9. Add deterministic tests for one acquisition、same-frame crop、invalid selection and cancellation cleanup.
10. Run only authorized restore、build、non-interactive tests and interactive verification.
11. Update CHANGELOG and actual evidence only after results exist.
12. Stop and report before beginning another feature.

## 7. Stop Conditions During Correction

Stop and report before expanding scope if：

- The corrected design requires replacing an Accepted P0 technology decision.
- WGC cannot provide a usable pre-selection monitor frame on the supported baseline.
- The Selection UI cannot present the frozen frame without transferring workflow ownership into the platform layer.
- Coordinate conversion produces unbounded、stale or mismatched crop results.
- The implementation performs a second desktop capture after Selection for the final result.
- The selected region cannot remain visibly tied to the frozen source content.
- Canonical BGRA8／premultiplied conversion is not deterministic.
- Clipboard publication requires a materially different format／API strategy.
- A dependency cycle would be required.
- Private desktop or Clipboard content would be persisted as evidence.
- Interactive verification would launch Paint、Notepad or another external GUI without explicit user authorization.
- The requested change enters an explicit non-goal.

A stop condition may require a targeted corrective ADR or contract change, but it must not restart the old prerequisite／closure-document pattern.

## 8. Documentation Freeze and Corrective Exception

The implementation-preparation documentation set remains frozen. This amendment is permitted because actual runtime behavior revealed that the original “region-selection surface” wording did not protect a product-critical requirement.

No additional planning documents should be created unless：

- A concrete restore／build／runtime failure reveals another incorrect assumption.
- The user changes product scope.
- An official dependency／version change creates an actual compatibility issue.
- A Frozen source or Accepted ADR must be superseded.

Normal correction work should update existing Specs、code、tests、CHANGELOG and evidence／status records—not create more readiness documents.

## 9. Final Review Matrix

| Review area | Result |
| --- | --- |
| Product baseline | PASS |
| Specification baseline | PASS after SPEC-0005 corrective clarification |
| Architecture baseline | PASS |
| Required P0 ADRs | PASS |
| Contracts | PASS; frozen-frame lifetime clarification required in implementation |
| Project Structure | PASS |
| Toolchain versions | PASS |
| Test strategy | PASS after corrective test additions |
| First-slice technical foundation | PASS |
| First-slice Region Selection UX | FAIL in current implementation; correction required |
| Same-frame Selection／Crop contract | Defined by corrective amendment; implementation evidence pending |
| Non-goals | PASS |
| Privacy／evidence boundary | PASS |
| Additional prerequisite documentation required | No |
| Ready to continue coding | **Yes, only for the bounded Region Selection correction** |
| Ready for feature expansion | **No** |

## 10. Final Decision

`Approved only for bounded correction of the first vertical slice Region Selection workflow.`

The immediate implementation target is:

```text
Capture one immutable source frame
→ present that same frame for Region Selection
→ crop that same frame
→ deliver the result
```

Feature expansion remains blocked until the corrected runtime behavior is verified.
