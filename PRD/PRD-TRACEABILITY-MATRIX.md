﻿﻿# SnipPlus v1 Requirements-to-Code Conformance Matrix

狀態：`Stage 5 Repository owner manual acceptance passed for the Owner Reference environment with the documented Windows PrintScreen compatibility prerequisite; Stage 6A SelectionLocked adjustment has static implementation and deterministic evidence, while packaged Stage 6A runtime verification remains pending; Stage 6B and later workflow stages remain unstarted or unverified`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `PRD-TRACEABILITY-MATRIX-001` |
| Version | `3.1` |
| Review date | `2026-07-29` |
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

Current source is a reusable **single-display capture／crop／PNG／Clipboard technical prototype with statically verified four-display topology／freezing integration、fifth-slice all-display presentation／initial Selection foundations and Stage 6A SelectionLocked adjustment**, not yet a conforming SnipPlus v1 implementation.

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
- SelectionLocked move、eight physical hit-test handles、edge／corner resize、outside-drag reselection、revision／rollback semantics and Windows handle／cursor mapping with deterministic evidence.

Major implementation gaps:

- topology-change runtime verification and a stable runtime cancellation interruption;
- transparent gap output;
- packaged Windows runtime verification of the Stage 6A SelectionLocked adjustment;
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
| `FR-001` | Manual startup keeps SnipPlus resident while running. | `ResidentLifecycleCoordinator`、single-instance activation and direct MainWindow exit are implemented; Repository owner manual acceptance confirmed Esc keeps the process resident, repeated sessions work, resident activation reuses the existing MainWindow and MainWindow `X` fully exits. | `Conforms` | Preserve the resident entry and exit boundary. |
| `FR-002` | User controls PrintScreen takeover. | `IPrintScreenTakeover`、persisted settings store、Windows `RegisterHotKey`／`UnregisterHotKey` and the application-owned message-only HWND are implemented. Repository owner confirmed background registration and repeated enable／disable behavior when the documented Windows native PrintScreen setting is disabled. Deterministic tests cover settings、idempotent registration／release and failure state. | `Conforms` | Preserve the registration boundary and document the Windows compatibility prerequisite. |
| `FR-003` | When takeover is enabled and SnipPlus is resident, PrintScreen starts one capture session. | PrintScreen maps to a platform-neutral `CaptureRequest` and enters `COMP-001` exactly once through `ResidentReady → CaptureRequested`; the accepted request has a static formal path through `CaptureRequested → Freezing` to all-display presentation and `Selecting`, with deterministic tests. Owner Reference packaged runtime accepted the fifth-slice Overlay／initial Selection; Stage 6A adjustment runtime remains pending. | `Partial` | Execute the separately authorized packaged Stage 6A adjustment runtime verification. |
| `FR-004` | Disabled takeover or application exit does not intercept PrintScreen. | Resident lifecycle release and late-event boundaries are deterministic-tested. Repository owner confirmed disabled takeover stops SnipPlus receipt, post-Esc background PrintScreen can start a new session, and MainWindow `X` leaves no SnipPlus process. The documented Windows native PrintScreen setting controls whether Windows itself shows its capture surface. | `Conforms` | Preserve the release and exit boundary. |
| `FR-005` | In-app capture is secondary. | Start Capture creates `SecondaryInAppCommand` and uses the same Core request boundary as PrintScreen; Repository owner repeated the secondary entry through the accepted Stage 5 overlay／initial Selection flow and cancelled without output. | `Conforms` | Preserve the shared request boundary; later output stages remain separate. |
| `FR-046`–`FR-047` | MainWindow `X` and explicit Exit terminate、release takeover and do not hide to tray. | MainWindow `Closed` still calls the application-exit boundary before `Environment.Exit(0)`; no tray surface exists; deterministic exit／Dispose tests pass. The current custom `AppInstance` and message-only owner package has not yet received the owner’s current packaged X／process cleanup re-acceptance. | `Partial` | Re-run current packaged MainWindow X and process cleanup verification. |
| `FR-006`–`FR-010` | Freeze and present all supported displays、crosshair、cross-monitor Selection and mask. | Owner Reference three-display topology、physical bounds／DPI、per-display WGC frame set、Frozen Overlay、mask、single system Crosshair and single／cross-display initial Selection passed Repository owner manual acceptance. Maximum four-4K runtime remains unverified. | `Partial` | Preserve the accepted Owner Reference result and separately verify the broader four-4K envelope. |
| `FR-011` | Mouse release locks Selection and creates no output. | Initial valid release is routed through `COMP-001` from `Selecting → SelectionLocked`; current packaged runtime verified single／cross-monitor mouse release locks the selection without crop、Clipboard、PNG or output. Pointer adjustment and later Editing remain outside this slice. | `Conforms` | Preserve the no-output SelectionLocked boundary; add later adjustment only in its own slice. |
| `FR-012` | Locked Selection supports pointer move、edge／corner resize and reselection. | `SelectionInteractionMode`、physical hit-testing for eight handles、clamped move、normalized／flipping resize、outside-drag reselection、invalid-release rollback and `SelectionRevision` are implemented in Core; Windows overlays expose physical handle placement and cursor mapping. `COMP-001` remains `SelectionLocked`, and deterministic tests cover the adjustment contract. Packaged Windows runtime verification is pending. | `Partial` | Execute Owner Reference packaged Stage 6A move／resize／reselection and cleanup verification. |
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
| `FR-036`–`FR-038` | Esc cancels at accepted capture stages. | Platform-neutral Esc routing is statically implemented for `Freezing`、`Selecting` and `SelectionLocked`; deterministic cleanup tests pass. Repository owner confirmed pre-drag、drag and SelectionLocked Esc close all overlays, keep the resident process alive, do not reopen MainWindow and permit a new background PrintScreen session. Later accepted stages remain unimplemented. | `Partial` | Preserve the accepted Stage 5 cancellation behavior; verify later accepted stages separately. |
| `FR-039` | Cancel produces no output. | Fifth-slice cancellation disposes the session／frame set without crop、Clipboard、PNG or output in deterministic tests; current packaged runtime kept Clipboard sequence unchanged and produced no output during cancellation. | `Partial` | Preserve no-output cancellation through later stages. |
| `FR-040`–`FR-043` | Cleanup、focus restoration、no MainWindow reopening、window exclusion. | Source exclusion／hide is statically performed before frame acquisition; deterministic overlay/session cleanup and no-output tests pass. Repository owner confirmed active capture is not disturbed by reactivation, Esc does not reopen MainWindow, and MainWindow `X` fully exits. Full focus restoration after later output stages remains unimplemented. | `Partial` | Preserve the accepted Stage 5 lifecycle and add later focus restoration separately. |
| `FR-044` | Failures are never reported as success. | Typed failures exist; retained Editing feedback does not. | `Partial` | Route typed outcomes to Editing feedback. |
| `FR-D01`–`FR-D09` | Deferred tools、formats and keyboard-only shortcuts remain absent. | No prohibited deferred implementation found. | `Conforms` | Preserve exclusion. |

## 5. Non-functional Conformance

| Requirement area | Accepted obligation | Current result | Status | Required action |
| --- | --- | --- | --- | --- |
| `NFR-001`–`NFR-003` | p95 capture、pointer interaction、output、progress and memory targets. | Async foundations exist; no accepted harness or evidence. | `Missing／Partial` | Add timers、30-run reporting and memory scenarios. |
| `NFR-004`–`NFR-008` | Stable Session and output integrity. | Immutable platform-neutral Session context、per-display frozen frame-set ownership、all-display overlay plan、same-frame presentation／selection reference、partial-failure cleanup and idempotent disposal have deterministic synthetic evidence; later output obligations remain. | `Partial` | Preserve ownership through runtime Selection、render and output slices. |
| `NFR-009`–`NFR-013` | Cross-display correctness、four-4K capacity and typed over-limit failure. | Four-4K policy、negative／mixed-DPI／irregular-gap snapshots、Windows topology mapping、current Owner Reference per-display WGC lifecycle、fifth-slice physical overlay／selection geometry and Stage 6A physical adjustment／boundary contracts are implemented with static／deterministic evidence. Repository owner accepted the three-display Owner Reference overlay／Crosshair／initial Selection runtime; Stage 6A adjustment runtime、broader four-4K envelope、topology-change runtime and stable cancellation interruption remain unverified. | `Partial` | Preserve the accepted Owner Reference result and execute Stage 6A adjustment runtime before broader envelope verification. |
| `NFR-014`–`NFR-019` | Familiar interaction、silent success、progress and retained-error feedback. | All-display dim-outside／clear-inside mask、crosshair and initial drag geometry are statically implemented; Editing、progress、output and visible-success behavior remain incomplete. | `Partial／Incorrect` | Implement Editing、300 ms progress and feedback. |
| `NFR-020`–`NFR-023` | Focus and exit behavior. | Direct MainWindow X exit、takeover release、custom AppInstance activation routing and resident activation boundary are statically implemented; deterministic lifecycle／activation tests pass. Repository owner confirmed background PrintScreen、pre-drag Esc、MainWindow reactivation、resident process retention and process cleanup for Stage 5. Full focus restoration after later output stages belongs to a later slice. | `Partial` | Preserve the accepted Stage 5 behavior and add later focus restoration separately. |
| `NFR-024`–`NFR-028` | Privacy and verification boundaries. | Local-only and synthetic-evidence boundaries exist. | `Conforms` | Preserve. |
| `NFR-029`–`NFR-031` | Accessible names、Esc cancellation and non-color-only state. | Accepted controls do not exist. | `Missing／Partial` | Implement only accepted baseline accessibility; do not add deferred shortcut workflow. |
| `NFR-032`–`NFR-036` | Maintainability and traceability. | Canonical documents、single `WorkflowStateAuthority`、platform-neutral request／freezing／presentation／selection boundaries and deterministic capacity／session／ownership／geometry tests exist. Stage 6A adds a typed adjustment contract without a second workflow authority; Editing and output state graph remain unimplemented. | `Conforms／Partial` | Trace every corrected slice and preserve COMP-001 ownership. |
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
| `ResidentLifecycleCoordinator` and PrintScreen contracts | `Conforms` for Stage 5 Owner Reference with the documented Windows prerequisite | Added setting、registration、event-boundary and exit cleanup ownership; deterministic non-interactive evidence and Repository owner background／Esc／exit acceptance pass. |
| `WindowsPrintScreenTakeover` | `Conforms` for Stage 5 Owner Reference with the documented Windows prerequisite | Added Win32 registration／release and an application-owned message-only HWND boundary; deterministic adapter tests and owner background `WM_HOTKEY`／release acceptance pass when the Windows native PrintScreen setting is disabled. |
| `ISettingsLauncher`／`WindowsSettingsLauncher` | `Conforms` for Stage 5 compatibility guidance | Platform-neutral async settings boundary、typed success／failure result、official `ms-settings:easeofaccess-keyboard` adapter and deterministic fake-launch tests exist; no Registry or takeover mutation is performed. |
| `Program`／`AppInstance` and `ResidentActivationBoundary` | `Partial` for Stage 5 single-instance activation | Custom `Program.Main` registers fixed key `SnipPlus.Main` before XAML initialization, redirects secondary activation and exits without creating a second MainWindow or resident lifecycle; the Core activation boundary deterministically ignores active capture／application exit and shows only ResidentReady. Current packaged Start-menu reactivation and process-count behavior remain unverified. |
| `CaptureRequest`／`CaptureRequestCoordinator`／application boundary | `Partial` for the second request-boundary slice | PrintScreen and `SecondaryInAppCommand` share one platform-neutral Core boundary; COMP-001 starts at `ResidentReady` and accepts only `ResidentReady → CaptureRequested`. Current-HEAD packaged runtime confirmed accepted first request、Busy rejection、restart reset and no Freezing／Capture side effect. Continue with the later capture slice. |
| `SupportedCapacityPolicy`、`VirtualDesktopSnapshot`、`DisplaySnapshot` | `Conforms` for the third-slice platform-neutral foundation | Four-4K source、Virtual Desktop、Selection allocation limits、negative coordinates、mixed DPI、irregular gaps and typed over-limit outcomes are implemented with synthetic contract evidence. | Preserve limits and validate again before later Selection／render allocation. |
| `CaptureSessionContext`、`FrozenDisplayFrame`、`FrozenDisplayFrameSet` | `Conforms` for the third-slice ownership foundation | One Session identity、RequestedAt、coordinate version、capacity result、cancellation and per-display immutable frame ownership are enforced; duplicate／missing／unknown／mismatched frames and partial cleanup are deterministic-tested, with current Owner Reference real frame-set metadata evidence. | Carry the same Session and frame set into later Selection and render. |
| `CaptureFreezingCoordinator` | `Partial` for the third／fourth freezing slices | Only the active accepted request can create one Session and request `CaptureRequested → Freezing`; capacity is checked before the all-display provider, complete frame-set attachment and cleanup outcomes are tested, with current Owner Reference provider/WGC runtime evidence. Fifth-slice presentation consumes the complete frame set without changing its ownership. | Preserve the freezing boundary and keep topology-change runtime separate. |
| `FrozenDisplayOverlayPlanBuilder`／`InitialSelectionCoordinator`／`CapturePresentationWorkflowCoordinator` | `Partial` for Stage 5 Owner Reference plus Stage 6A static adjustment | Builds one physical overlay descriptor per display, presents only complete frame sets, maps pointer input to normalized physical Virtual Desktop bounds, and routes valid release／Esc through COMP-001. Stage 6A adds locked move／resize／reselection with revision and rollback while authority remains `SelectionLocked`; deterministic tests pass. Owner Reference initial Selection and no-output cancellation are accepted, but Stage 6A packaged runtime remains pending. | Execute Stage 6A runtime without changing session or workflow ownership. |
| `WindowsFrozenDisplayOverlayCoordinator`／`WindowsMainWindowCaptureSourceExclusion` | `Partial` for Stage 6A static overlay adjustment | Provides one per-display physical overlay, PMA V2 placement, own frozen frame presentation, dim-outside／clear-inside mask, shared pointer capture, eight physical handle surfaces and hit-test cursor mapping. Stage 5 initial Selection and Esc cleanup are owner-accepted; Stage 6A handle／cursor／mixed-DPI runtime remains pending. | Execute Owner Reference Stage 6A runtime; keep Maximum four-4K and later output work separate. |

## 7. Required Correction Order

1. Resident lifecycle、direct MainWindow exit and takeover setting — deterministic evidence and Repository owner Stage 5 manual acceptance passed for the Owner Reference environment; preserve the documented Windows PrintScreen prerequisite.
2. PrintScreen entry integrated with `COMP-001` through `ResidentReady → CaptureRequested` — static implementation、deterministic tests and current-HEAD packaged runtime verification passed; preserve this boundary while continuing through the third-slice Freezing foundation.
3. Four-4K capacity policy、Frozen Virtual Desktop context、per-display frame ownership、Windows topology／per-display WGC freezing integration and `CaptureRequested → Freezing` foundation — static implementation、deterministic non-interactive evidence and current Owner Reference three-display runtime passed; cancellation interruption remains inconclusive and topology-change runtime remains pending.
4. All-display presentation、crosshair and cross-monitor initial Selection — fifth slice implementation、deterministic verification and Repository owner Owner Reference manual acceptance passed. Broader Maximum four-4K capacity remains pending.
5. Locked Selection、pointer move、edge／corner resize and reselection — Stage 6A static implementation and deterministic tests passed; packaged runtime verification pending.
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

The corrected Stage 5 resident lifecycle／PrintScreen／activation source and fifth-slice Owner Reference Overlay／Crosshair／initial Selection acceptance remain recorded with the documented Windows PrintScreen compatibility prerequisite. Current HEAD has locked restore、Release x64 build with `0 warnings / 0 errors`、limited Stage 6A format verification、`git diff --check` and `115/115` deterministic non-interactive tests passing. Stage 6A adds platform-neutral SelectionLocked move、eight physical edge／corner handles、clamped and flipping resize、outside-drag reselection、revision／rollback semantics and Windows overlay handle／cursor mapping while preserving `COMP-001` as the sole workflow authority. No packaged Stage 6A runtime verification has been run; therefore FR-012 remains `Partial`. Maximum four-4K runtime、topology-change runtime、transparent gap output、Editing、output and Annotation work are not claimed. Stage 6B and later slices are not started.
