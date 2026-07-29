﻿﻿# SnipPlus v1 Requirements-to-Code Conformance Matrix

狀態：`Implementation and automated verification passed; Repository owner five-item Stage 5 re-acceptance pending — resident lifecycle, PrintScreen request boundary, Windows multi-display freezing integration and fifth-slice Frozen Display Presentation／initial Selection foundations are implemented; Maximum four-4K envelope and later Selection／Editing／output slices remain partial or missing`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `PRD-TRACEABILITY-MATRIX-001` |
| Version | `2.9` |
| Review date | `2026-07-28` |
| Product baseline | Accepted current `PRD-0004`–`PRD-0006` and `SPEC-0003`–`SPEC-0010` |
| Reviewed implementation | Branch
emove-snipplus-md` technical prototype |
| Review type | Static requirements-to-code／test／runtime conformance review |
| Code changes authorized by this document | No |

This matrix compares accepted v1 behavior with current code、tests and historical runtime evidence. Passing builds or test counts do not prove user-visible conformance.

## 2. Status Definitions

| Status | Meaning |
| --- | --- |
| `Conforms` | Current implementation directly satisfies the requirement with relevant evidence. |
| `Partial` | A reusable portion exists, but accepted behavior or coverage is incomplete. |
| `Missing` | No implementation of the accepted behavior was found. |
| `Incorrect` | Current behavior conflicts with the accepted baseline. |
| `Obsolete` | Behavior belongs only to the superseded workflow. |
| `Blocked by product decision` | An unresolved visible product decision prevents implementation. No current v1 row has this status. |

## 3. Executive Result

Current source is a reusable **single-display capture／crop／PNG／Clipboard technical prototype with statically verified four-display topology／freezing integration and fifth-slice all-display presentation／initial Selection foundations**, not yet a conforming SnipPlus v1 implementation.

Reusable foundations:

- one-display WGC acquisition;
- same-frame crop;
- clear-inside／dim-outside mask on one surface;
- single-display coordinate conversion;
- BGRA8 premultiplied SoftwareBitmap;
- PNG encoder;
- Clipboard delivery with bounded cancellable retry;
- shared state authority;
- platform-neutral four-4K capacity policy、typed capacity outcomes、Frozen Virtual Desktop／Display snapshots and transparent gap policy;
- platform-neutral Capture Session context、per-display frozen frame set ownership、single `CaptureRequested → Freezing` transition and deterministic cleanup boundary;
- resident lifecycle coordinator、persisted takeover setting and platform-neutral PrintScreen boundary;
- Windows PrintScreen registration／release adapter and direct application-exit cleanup path;
- platform-neutral PrintScreen／secondary capture-request boundary、formal `ResidentReady` initial state and `ResidentReady → CaptureRequested` transition;
- Windows display topology mapping、physical Virtual Desktop bounds、DPI／orientation metadata、mirror dedupe and deterministic coordinate version;
- per-display Windows.Graphics.Capture adapter lifecycle、all-display prepare／start／parallel first-frame coordination、typed integration outcomes and complete frame-set cleanup;
- deterministic low-level tests.

Major implementation gaps:

- topology-change runtime verification and a stable runtime cancellation interruption;
- real all-display Overlay／Frozen Canvas／Crosshair／initial Selection runtime verification and transparent gap output;
- SelectionLocked adjustment;
- accepted state graph;
- function bar and pointer-driven Annotation;
- Complete／Save commitment boundaries and progress state;
- Downloads-default Save As and retained-file outcome;
- quantitative timing、memory and repeated-session evidence;
- the later Capture workflow runtime evidence;
- focus restoration and retained Editing after recoverable failure.

Keyboard-only Annotation and non-PrintScreen shortcuts are deferred and must not be classified as missing v1 work.

## 4. Functional Requirements Conformance

| Requirement | Accepted behavior | Current result | Status | Required action |
| --- | --- | --- | --- | --- |
| `FR-001` | Manual startup keeps SnipPlus resident while running. | `ResidentLifecycleCoordinator` is wired from MainWindow construction and owns resident takeover state; locked restore、Release x64 build and non-interactive tests passed. Repository owner acceptance found that Esc cleanup could terminate the process; the correction is committed but not yet re-accepted. | `Partial` | Complete the five-item Stage 5 re-acceptance and preserve the resident entry and exit boundary. |
| `FR-002` | User controls PrintScreen takeover. | `IPrintScreenTakeover`、persisted settings store and Windows `RegisterHotKey`／`UnregisterHotKey` implementation added; deterministic tests cover settings、idempotent registration／release and failure state. Current-HEAD packaged runtime verified enable、disable and restart persistence. | `Conforms` | Preserve the takeover setting boundary. |
| `FR-003` | When takeover is enabled and SnipPlus is resident, PrintScreen starts one capture session. | PrintScreen maps to a platform-neutral `CaptureRequest` and enters `COMP-001` exactly once through `ResidentReady → CaptureRequested`; the accepted request now has a static formal path through `CaptureRequested → Freezing` to all-display presentation and `Selecting`, with deterministic tests. Current packaged runtime evidence intentionally stops before the fifth-slice Overlay／Selection behavior. | `Partial` | Execute the separately authorized packaged Overlay／Selection runtime verification. |
| `FR-004` | Disabled takeover or application exit does not intercept PrintScreen. | Existing resident lifecycle／Windows registration-release implementation and packaged runtime evidence remain valid; late events after exit are ignored by deterministic tests. | `Conforms` | Preserve release and exit ownership. |
| `FR-005` | In-app capture is secondary. | Start Capture creates `SecondaryInAppCommand` and uses the same Core request boundary as PrintScreen; the accepted request is statically composed through the fifth-slice freezing／presentation coordinator and does not call the legacy `BeginCaptureAsync` path. Current-HEAD packaged runtime evidence covers only the earlier request boundary. | `Partial` | Preserve the secondary command while completing runtime verification and later output stages. |
| `FR-046`–`FR-047` | MainWindow `X` and explicit Exit terminate、release takeover and do not hide to tray. | MainWindow `Closed` calls the resident exit boundary before `Environment.Exit(0)`; no tray surface exists; deterministic exit／Dispose tests and current-HEAD packaged runtime process／cleanup checks passed. | `Conforms` | Preserve direct process exit and release ordering. |
| `FR-006`–`FR-010` | Freeze and present all supported displays、crosshair、cross-monitor Selection and mask. | Current Owner Reference topology、physical bounds／DPI、capacity and real per-display WGC complete frame-set acquisition are verified. The fifth-slice implementation now removes the per-overlay drawn crosshair lines and assigns one system `InputSystemCursorShape.Cross` to each input surface; deterministic tests and a clean packaged MSIX build passed. Repository owner manual acceptance previously found three simultaneous drawn crosses and non-following behavior; the corrected package is pending re-test. Maximum four-4K runtime remains unverified. | `Partial` | Complete Repository owner manual acceptance for the corrected Crosshair behavior and separately verify the broader four-4K envelope. |
| `FR-011` | Mouse release locks Selection and creates no output. | Initial valid release is routed through `COMP-001` from `Selecting → SelectionLocked`; current packaged runtime verified single／cross-monitor mouse release locks the selection without crop、Clipboard、PNG or output. Pointer adjustment and later Editing remain outside this slice. | `Conforms` | Preserve the no-output SelectionLocked boundary; add later adjustment only in its own slice. |
| `FR-012` | Locked Selection supports pointer move、edge／corner resize and reselection. | Not present. | `Missing` | Implement Selection revisions and pointer handles. |
| `FR-048` | Non-display gap pixels are transparent. | No multi-display composition or gap tests. | `Missing` | Compose gaps as BGRA alpha `0`. |
| `FR-013`–`FR-016` | Mandatory Editing function bar and Complete／Save／Cancel. | No Editing state or function bar. | `Missing／Incorrect` | Replace state graph and add function bar. |
| `FR-017`–`FR-023` | Required pointer-driven Annotation tools and styling. | No Annotation model or tools. | `Missing` | Implement accepted pointer-driven tool set only. |
| `FR-024`–`FR-025` | Function-bar Annotation-only Undo／Redo. | Not present. | `Missing` | Add Annotation history independent of Selection revisions. |
| `FR-026`–`FR-029` | Frozen Virtual Desktop Annotation coordinates and clipping. | No Annotation model. | `Missing` | Implement after Virtual Desktop model stabilizes. |
| `FR-030` | Complete renders current revision and writes Clipboard only. | Crop／Clipboard exists at wrong boundary. | `Incorrect` | Reuse delivery behind explicit Complete. |
| `FR-031`、`FR-049` | Save As is PNG-only、starts in Downloads、uses timestamp name and permits changes. | PNG encoder exists; Save As does not. | `Partial` | Add Windows Save As with accepted defaults. |
| `FR-032` | Save sends the same final result to PNG and Clipboard. | No Save orchestration. | `Missing` | Share one Result ID across both operations. |
| `FR-033` | Save As cancel returns to Editing. | No Save dialog or Editing state. | `Missing` | Add `SaveDialogCancelled` outcome. |
| `FR-034`、`FR-045` | Recoverable output failure preserves Editing and actionable feedback. | Current UI disposes Session after delivery attempt. | `Incorrect` | Retain frames、Selection and Annotation revision. |
| `FR-050` | Clipboard failure after PNG success retains PNG and returns to Editing. | No file output or retained-file outcome. | `Missing` | Add `RetainedFileReference`; never roll back PNG. |
| `FR-035` | Success is silent. | MainWindow shows success status. | `Incorrect` | Restore prior application without success notification. |
| `FR-036`–`FR-038` | Esc cancels at accepted capture stages. | Platform-neutral Esc routing is statically implemented for `Freezing`、`Selecting` and `SelectionLocked`; deterministic cleanup tests pass. Repository owner acceptance found that the pre-fix packaged Esc path terminated the process; the Windows key handler now defers cancellation until the key event returns, and owner re-acceptance is pending. Later accepted stages remain unimplemented. | `Partial` | Complete the five-item Stage 5 re-acceptance and verify later accepted stages separately. |
| `FR-039` | Cancel produces no output. | Fifth-slice cancellation disposes the session／frame set without crop、Clipboard、PNG or output in deterministic tests; current packaged runtime kept Clipboard sequence unchanged and produced no output during cancellation. | `Partial` | Preserve no-output cancellation through later stages. |
| `FR-040`–`FR-043` | Cleanup、focus restoration、no MainWindow reopening、window exclusion. | Source exclusion／hide is statically performed before frame acquisition; deterministic overlay/session cleanup and no-output tests pass. Repository owner acceptance found the pre-fix Esc path terminated the process; the corrected deferred cleanup path is awaiting re-acceptance. Focus restoration remains unverified. | `Partial` | Complete the five-item Stage 5 re-acceptance and add foreground-context behavior later. |
| `FR-044` | Failures are never reported as success. | Typed failures exist; retained Editing feedback does not. | `Partial` | Route typed outcomes to Editing feedback. |
| `FR-D01`–`FR-D09` | Deferred tools、formats and keyboard-only shortcuts remain absent. | No prohibited deferred implementation found. | `Conforms` | Preserve exclusion. |

## 5. Non-functional Conformance

| Requirement area | Accepted obligation | Current result | Status | Required action |
| --- | --- | --- | --- | --- |
| `NFR-001`–`NFR-003` | p95 capture、pointer interaction、output、progress and memory targets. | Async foundations exist; no accepted harness or evidence. | `Missing／Partial` | Add timers、30-run reporting and memory scenarios. |
| `NFR-004`–`NFR-008` | Stable Session and output integrity. | Immutable platform-neutral Session context、per-display frozen frame-set ownership、all-display overlay plan、same-frame presentation／selection reference、partial-failure cleanup and idempotent disposal have deterministic synthetic evidence; later output obligations remain. | `Partial` | Preserve ownership through runtime Selection、render and output slices. |
| `NFR-009`–`NFR-013` | Cross-display correctness、four-4K capacity and typed over-limit failure. | Four-4K policy、negative／mixed-DPI／irregular-gap snapshots、Windows topology mapping、current Owner Reference per-display WGC lifecycle and fifth-slice physical overlay／selection geometry are implemented with static／deterministic evidence; earlier Owner Reference overlay interaction passed, but the pre-fix Esc path terminated the process. The corrected cleanup path awaits owner re-acceptance; the broader four-4K envelope、topology-change runtime and stable cancellation interruption remain unverified. | `Partial` | Preserve the verified Owner Reference result and keep broader envelope verification separate. |
| `NFR-014`–`NFR-019` | Familiar interaction、silent success、progress and retained-error feedback. | All-display dim-outside／clear-inside mask、crosshair and initial drag geometry are statically implemented; Editing、progress、output and visible-success behavior remain incomplete. | `Partial／Incorrect` | Implement Editing、300 ms progress and feedback. |
| `NFR-020`–`NFR-023` | Focus and exit behavior. | Direct MainWindow X exit and takeover release path are statically implemented; deterministic lifecycle tests pass. Repository owner acceptance found the pre-fix Esc path terminated the process, while MainWindow X remains the explicit application-exit path. The correction awaits five-item owner re-acceptance; focus restoration belongs to a later slice. | `Partial` | Preserve exit／release; complete Stage 5 re-acceptance and add focus restoration later. |
| `NFR-024`–`NFR-028` | Privacy and verification boundaries. | Local-only and synthetic-evidence boundaries exist. | `Conforms` | Preserve. |
| `NFR-029`–`NFR-031` | Accessible names、Esc cancellation and non-color-only state. | Accepted controls do not exist. | `Missing／Partial` | Implement only accepted baseline accessibility; do not add deferred shortcut workflow. |
| `NFR-032`–`NFR-036` | Maintainability and traceability. | Canonical documents、single `WorkflowStateAuthority`、platform-neutral request／freezing／presentation／selection boundaries and deterministic capacity／session／ownership／geometry tests exist; later adjustment、Editing and output state graph remains unimplemented. | `Conforms／Partial` | Trace every corrected slice and preserve COMP-001 ownership. |
| `NFR-037`–`NFR-039` | Windows v1 four-4K envelope and deferred compatibility. | Platform-neutral four-4K policy and Windows topology／multi-display WGC integration are implemented; current Owner Reference three-display topology and real WGC freezing passed, while four-display runtime and topology-change runtime remain unverified. | `Partial` | Preserve the verified Owner Reference result and keep broader envelope verification separate. |

## 6. Code Classification

| Current asset | Classification | Disposition |
| --- | --- | --- |
| One-display `WindowsGraphicsCaptureAdapter` | `Partial` reusable foundation | Retained for legacy `ICaptureService`; now also supports one-display prepare／start／first-frame ownership under the four-4K multi-display orchestrator. |
| `WindowsDisplayTopologyProvider`／`WindowsDisplayTopologyMapper` | `Conforms` for current Owner Reference topology integration | Maps Windows physical display data to platform-neutral snapshots with DPI、orientation、negative coordinates、mirror dedupe and deterministic coordinate versions; current Owner Reference three-display runtime verified. |
| `WindowsFrozenDisplayFrameSetProvider`／`WindowsDisplayCaptureAdapterFactory` | `Conforms` for current Owner Reference freezing integration | Creates one adapter per display after Core capacity validation, prepares all, starts all, collects first frames in parallel and cleans every partial resource; current Owner Reference real WGC frame-set acquisition and three-session re-entry verified. |
| `FrozenCaptureFrame` | `Partial` reusable foundation | Evolve into Session-owned per-display frames. |
| `CoordinateMapper` | `Partial` reusable foundation | Add Virtual Desktop intersections、mixed-DPI mapping and transparent gaps. |
| One-display mask | `Partial` reusable behavior | Preserve clear-inside／dim-outside semantics. |
| `SelectionCanvas_PointerReleased → CompleteSelectionAsync` | `Obsolete` | Removed from the MainWindow product entry; no fifth-slice release-to-output path remains. |
| Current `WorkflowState` graph | `Partial` | Formal `ResidentReady → CaptureRequested → Freezing → Selecting → SelectionLocked／Cancelled／Failed` transitions and deterministic authority tests now exist; later `Editing` and output states remain. |
| Crop／Clipboard orchestration | `Partial` foundation、`Incorrect` placement | Move behind Complete／Save and progress boundary. |
| Clipboard retry adapter | `Conforms` technical foundation | Preserve bounded retry and privacy defaults. |
| `PngEncoder` | `Partial` reusable foundation | Use in Save As and verify alpha／size tiers. |
| Current workflow tests | `Partial` historical foundation | Supersede release-to-Clipboard assertions and add v1 quality tests. |
| `ResidentLifecycleCoordinator` and PrintScreen contracts | `Conforms` for the first resident lifecycle／takeover slice | Added setting、registration、event-boundary and exit cleanup ownership; deterministic non-interactive evidence and current-HEAD packaged runtime evidence passed. The later request boundary is tracked separately. |
| `WindowsPrintScreenTakeover` | `Conforms` for the first resident lifecycle／takeover slice | Added Win32 registration／release and HWND message boundary; deterministic adapter tests and current-HEAD packaged runtime registration／release evidence passed. |
| `CaptureRequest`／`CaptureRequestCoordinator`／application boundary | `Partial` for the second request-boundary slice | PrintScreen and `SecondaryInAppCommand` share one platform-neutral Core boundary; COMP-001 starts at `ResidentReady` and accepts only `ResidentReady → CaptureRequested`. Current-HEAD packaged runtime confirmed accepted first request、Busy rejection、restart reset and no Freezing／Capture side effect. Continue with the later capture slice. |
| `SupportedCapacityPolicy`、`VirtualDesktopSnapshot`、`DisplaySnapshot` | `Conforms` for the third-slice platform-neutral foundation | Four-4K source、Virtual Desktop、Selection allocation limits、negative coordinates、mixed DPI、irregular gaps and typed over-limit outcomes are implemented with synthetic contract evidence. | Preserve limits and validate again before later Selection／render allocation. |
| `CaptureSessionContext`、`FrozenDisplayFrame`、`FrozenDisplayFrameSet` | `Conforms` for the third-slice ownership foundation | One Session identity、RequestedAt、coordinate version、capacity result、cancellation and per-display immutable frame ownership are enforced; duplicate／missing／unknown／mismatched frames and partial cleanup are deterministic-tested, with current Owner Reference real frame-set metadata evidence. | Carry the same Session and frame set into later Selection and render. |
| `CaptureFreezingCoordinator` | `Partial` for the third／fourth freezing slices | Only the active accepted request can create one Session and request `CaptureRequested → Freezing`; capacity is checked before the all-display provider, complete frame-set attachment and cleanup outcomes are tested, with current Owner Reference provider/WGC runtime evidence. Fifth-slice presentation consumes the complete frame set without changing its ownership. | Preserve the freezing boundary and keep topology-change runtime separate. |
| `FrozenDisplayOverlayPlanBuilder`／`InitialSelectionCoordinator`／`CapturePresentationWorkflowCoordinator` | `Partial` for fifth-slice automated implementation; Repository owner five-item re-acceptance pending | Builds one physical overlay descriptor per display, presents only complete frame sets, maps pointer input to normalized physical Virtual Desktop bounds, and routes valid release／Esc through COMP-001 with deterministic cleanup tests. Idle pointer movement is ignored by the Core selection authority without changing `SelectionStatus.None` or revision. The Esc cleanup correction is committed and automated verification passed; later adjustment remains pending. | Preserve single-session and single-frame-set ownership; complete the five-item corrected fifth-slice acceptance before later adjustment. |
| `WindowsFrozenDisplayOverlayCoordinator`／`WindowsMainWindowCaptureSourceExclusion` | `Partial` for fifth-slice automated implementation; Repository owner five-item re-acceptance pending | Provides one per-display physical overlay, PMA V2 placement, own frozen frame presentation, dim-outside／clear-inside mask, shared pointer capture and a system `InputSystemCursorShape.Cross` on each input surface. The former per-overlay XAML crosshair lines were removed after owner acceptance found simultaneous static crosses. Esc now queues the workflow boundary after the `KeyDown` callback returns, avoiding synchronous close of the focused overlay; a clean-commit Development MSIX was rebuilt, signed, installed with `Status=Ok` and DLL hash parity. MainWindow is hidden before acquisition and cleanup remains idempotent. | Complete the five-item owner re-acceptance for resident Esc cleanup and background-focus PrintScreen; keep Maximum four-4K and topology-change runtime separate. |

## 7. Required Correction Order

1. Resident lifecycle、direct MainWindow exit and takeover setting — deterministic evidence passed; the Esc resident-process correction is committed and the five-item packaged re-acceptance is pending.
2. PrintScreen entry integrated with `COMP-001` through `ResidentReady → CaptureRequested` — static implementation、deterministic tests and current-HEAD packaged runtime verification passed; preserve this boundary while continuing through the third-slice Freezing foundation.
3. Four-4K capacity policy、Frozen Virtual Desktop context、per-display frame ownership、Windows topology／per-display WGC freezing integration and `CaptureRequested → Freezing` foundation — static implementation、deterministic non-interactive evidence and current Owner Reference three-display runtime passed; cancellation interruption remains inconclusive and topology-change runtime remains pending.
4. All-display presentation、crosshair and cross-monitor initial Selection — fifth slice implementation and deterministic verification passed; the owner found an Esc resident-process defect after the drawn-crosshair correction, the deferred-key cleanup correction is committed, and five-item owner re-acceptance is pending. Broader Maximum four-4K capacity remains pending.
5. Locked Selection、pointer move、edge／corner resize and reselection.
6. Accepted workflow state graph.
7. Function bar、Complete／Save／Cancel、progress and focus restoration.
8. Annotation document、required pointer-driven tools and object editing.
9. Annotation Undo／Redo、Virtual Desktop anchoring and Selection clipping.
10. Complete final render、capacity revalidation、transparent gaps and Clipboard.
11. Save As with Downloads default、PNG、same-result Clipboard and retained-file outcome.
12. Recoverable failure preservation、stale-revision protection、performance／memory evidence and required accessibility.
13. Explicitly authorized Owner Reference、Standard and Maximum runtime verification.

Each correction task updates matrix status only after code、tests and applicable evidence exist.

## 8. Finalized Product Decisions

- MainWindow `X` exits and releases takeover; no close-to-tray behavior.
- Physical non-display gaps are transparent.
- Save As starts in Downloads and allows another destination.
- PNG is retained if later Clipboard publication fails.
- Quantitative performance and memory targets are fixed in `PRD-0006 §3`.
- Maximum source capacity is four displays、each no larger than `3840 × 2160`、total source pixels `≤ 33,177,600`.
- Virtual Desktop and output-allocation caps remain fixed in `PRD-0006 §11`.
- Owner Reference verification uses 2K primary、FHD lower at 150% scaling and left 2K.
- Keyboard-only Annotation and non-PrintScreen tool／action shortcuts are deferred.
- PrintScreen entry and Esc cancellation remain required.

No current v1 behavior is `Blocked by product decision`.

## 9. Final Conclusion

The first resident lifecycle／PrintScreen takeover coding slice remains conforming with static implementation、contract tests、locked restore、Release x64 build、passing non-interactive verification and prior packaged Windows Runtime evidence. The second request-boundary slice has static implementation and current-HEAD packaged runtime evidence: PrintScreen and the secondary in-app command enter the sole `COMP-001` authority at `ResidentReady → CaptureRequested`. The third and fourth slices provide the platform-neutral four-4K capacity、Frozen Virtual Desktop、Capture Session、per-display frame ownership、Windows topology mapping and per-display WGC freezing integration; current Owner Reference three-display physical topology and real per-display WGC freezing passed. The fifth slice now has `97/97` non-interactive tests passing and the Esc key-handler correction committed, but Repository owner five-item re-acceptance remains pending after the prior process-termination defect. Maximum four-4K runtime、topology-change runtime、Selection adjustment、Editing、output and Annotation work are not claimed. No sixth slice is started.
