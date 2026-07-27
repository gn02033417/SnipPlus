# SnipPlus v1 Requirements-to-Code Conformance Matrix

狀態：`Reviewed — implementation correction required`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `PRD-TRACEABILITY-MATRIX-001` |
| Version | `2.0` |
| Review date | `2026-07-27` |
| Product baseline | Accepted `PRD-0004`–`PRD-0006` and `SPEC-0003`–`SPEC-0010` |
| Reviewed implementation | Branch `remove-snipplus-md` at parent commit `8e020c31929804da58c724f36ca7a1e4145a930e` |
| Review type | Static requirements-to-code／test／runtime conformance review |
| Code changes authorized by this document | No |

本文件取代舊版 Research／Decision placeholder matrix。它不新增產品需求；它只比較已接受的 v1 產品基線與目前程式碼、測試及 runtime evidence。

## 2. Status Definitions

| Status | Meaning |
| --- | --- |
| `Conforms` | Current implementation directly satisfies the requirement with relevant code and evidence. |
| `Partial` | A reusable portion exists, but accepted user-visible behavior or coverage is incomplete. |
| `Missing` | No current implementation of the accepted behavior was found. |
| `Incorrect` | Current implementation performs behavior that conflicts with the accepted baseline. |
| `Obsolete` | The implementation belongs only to a superseded workflow and should not remain as product behavior. |
| `Blocked by product decision` | Implementation cannot proceed because an accepted document explicitly leaves a visible behavior unresolved. |

A passing build、test count or platform test is not sufficient to mark a user-visible requirement `Conforms`.

## 3. Evidence Inventory

### Current application and workflow code

- [App.xaml.cs](../src/SnipPlus.App/App.xaml.cs)
- [MainWindow.xaml](../src/SnipPlus.App/MainWindow.xaml)
- [MainWindow.xaml.cs](../src/SnipPlus.App/MainWindow.xaml.cs)
- [CaptureWorkflowCoordinator.cs](../src/SnipPlus.Core/CaptureWorkflowCoordinator.cs)
- [WorkflowStateAuthority.cs](../src/SnipPlus.Core/WorkflowStateAuthority.cs)
- [CoordinateMapper.cs](../src/SnipPlus.Core/CoordinateMapper.cs)
- [CaptureContracts.cs](../src/SnipPlus.Contracts/CaptureContracts.cs)
- [WorkflowContracts.cs](../src/SnipPlus.Contracts/WorkflowContracts.cs)
- [DeliveryContracts.cs](../src/SnipPlus.Contracts/DeliveryContracts.cs)
- [WindowsGraphicsCaptureAdapter.cs](../src/SnipPlus.Windows/WindowsGraphicsCaptureAdapter.cs)
- [WinRtClipboardDeliveryAdapter.cs](../src/SnipPlus.Windows/WinRtClipboardDeliveryAdapter.cs)
- [PngEncoder.cs](../src/SnipPlus.Windows/PngEncoder.cs)

### Current tests

- [CaptureWorkflowCoordinatorTests.cs](../tests/SnipPlus.Core.Tests/CaptureWorkflowCoordinatorTests.cs)
- [WorkflowStateAuthorityTests.cs](../tests/SnipPlus.Core.Tests/WorkflowStateAuthorityTests.cs)
- [CoordinateMapperTests.cs](../tests/SnipPlus.Core.Tests/CoordinateMapperTests.cs)
- [SoftwareBitmapTests.cs](../tests/SnipPlus.Windows.Tests/SoftwareBitmapTests.cs)
- [WinRtClipboardDeliveryAdapterTests.cs](../tests/SnipPlus.Windows.Tests/WinRtClipboardDeliveryAdapterTests.cs)
- [WindowsGraphicsCapturePlatformTests.cs](../tests/SnipPlus.Windows.Tests/WindowsGraphicsCapturePlatformTests.cs)

### Runtime evidence boundary

[CHANGELOG.md](../CHANGELOG.md) records a packaged synthetic checkerboard verification for the superseded single-display flow:

```text
Start Capture button
→ one display frozen frame
→ drag selection
→ mouse release
→ immediate crop／Clipboard
→ main-window success status
```

That evidence remains valid for the one-frame WGC／crop／Clipboard technical foundation. It is **invalid as evidence of accepted v1 product conformance** because it does not verify resident PrintScreen takeover、all-display freeze、cross-monitor selection、locked-selection editing、function bar、annotations、Save or focus restoration.

## 4. Executive Conformance Result

The current implementation is a reusable technical prototype, not an implementation of the accepted v1 workflow.

### Reusable foundations

- One-display WGC frame acquisition.
- Same-frame crop without post-selection recapture.
- Single-display mask presentation with clear selection interior.
- DIP-to-physical mapping for one display context.
- Canonical BGRA8 premultiplied image result.
- Deterministic crop and in-memory PNG encoding.
- WinRT Clipboard publication with bounded cancellable retry.
- Idempotent frozen-frame disposal.
- One shared workflow state authority.

### Blocking product gaps

- No resident lifecycle or PrintScreen takeover.
- No all-display frozen session or Virtual Desktop model.
- No cross-monitor selection.
- Mouse release still commits output immediately.
- No locked-selection move、resize or reselection.
- No editing／confirmation function bar.
- No annotation object model or required v1 tools.
- No Save As／PNG file-delivery workflow.
- Recoverable output failure does not retain an editing session.
- No pre-capture foreground-context restoration.
- The SnipPlus main window is shown again after completion or cancellation.

## 5. Functional Requirements Conformance

| Requirement ID | Accepted product behavior | Owning Spec／Contract | Current code | Current tests | Runtime evidence | Status | Required action |
| --- | --- | --- | --- | --- | --- | --- | --- |
| `FR-001` | Manual startup leaves SnipPlus resident in background. | `SPEC-0003 SR-001`、`SPEC-0005` | `App.OnLaunched` creates and activates one `MainWindow`; closing the window disposes the workflow. No resident／tray lifecycle exists. | No coverage. | Not verified. | `Missing` | Add an explicit resident application lifecycle before capture features. |
| `FR-002` | User setting enables or disables PrintScreen takeover. | `SPEC-0003 SR-001`、`SPEC-0005-AC-001` | No setting、binding or takeover service exists. | No coverage. | Not verified. | `Missing` | Implement persisted/user-controlled takeover setting and state. |
| `FR-003` | Enabled PrintScreen starts one capture session. | `SPEC-0005-AC-001` | No PrintScreen／hotkey entry exists. | No coverage. | Not verified. | `Missing` | Add the accepted primary entry and route it through COMP-001. |
| `FR-004` | Disabled takeover does not intercept PrintScreen. | `SPEC-0005-AC-001` | No takeover implementation exists. | No coverage. | Not verified. | `Missing` | Verify disabled and process-exit release behavior. |
| `FR-005` | In-app capture may remain secondary, not primary. | `SPEC-0005 §4` | `StartCaptureButton` is currently the only and therefore primary entry. | No UI coverage. | Runtime uses this button. | `Incorrect` | Retain only as secondary／diagnostic after PrintScreen exists; remove “first vertical slice” product presentation. |
| `FR-006` | Freeze every connected display before selection. | `SPEC-0005-AC-002`、Contracts v2 §3–4 | `BeginCaptureAsync` resolves one `DisplayArea` from the current window and creates one display adapter／frame. | Coordinator proves one frame call; platform test captures one primary display. | One-display only. | `Incorrect` | Introduce one session-owned collection of per-display snapshots and frames. |
| `FR-007` | Present one continuous Frozen Virtual Desktop canvas. | `SPEC-0005 §5` | Contracts and UI contain one `DisplayContextSnapshot` and one `SelectionFrameImage`; no Virtual Desktop context exists in code. | No coverage. | Not verified. | `Missing` | Add Virtual Desktop snapshot、origin、display collection and multi-window／surface presentation. |
| `FR-008` | Every display is masked and pointer becomes crosshair. | `SPEC-0005-AC-004` | Four mask rectangles exist on one selection surface; no explicit crosshair cursor assignment and no all-display surfaces. | No UI coverage. | Single-display mask verified only. | `Partial` | Extend mask／cursor behavior across the entire Virtual Desktop. |
| `FR-009` | One rectangle can span multiple displays. | `SPEC-0005-AC-003` | Selection is constrained to one WinUI `Canvas` and one display context. | No cross-display tests. | Not verified. | `Missing` | Implement selection geometry in Virtual Desktop coordinates. |
| `FR-010` | During drag, inside is clear and outside remains dimmed. | `SPEC-0005-AC-004` | `UpdateSelectionMasks` implements this for one display. | No UI automation; packaged synthetic verification observed it on one display. | Verified only for one display. | `Partial` | Preserve behavior while extending it across display boundaries. |
| `FR-011` | Mouse release locks selection and performs no output. | `SPEC-0005-AC-005`、`SPEC-0007-AC-001` | `SelectionCanvas_PointerReleased` immediately calls `CompleteSelectionAsync`, which crops and writes Clipboard. | Coordinator tests encode immediate capture／Clipboard behavior. | Runtime verifies immediate Clipboard after release. | `Incorrect` | Replace release-to-output with `SelectionLocked → Editing`. |
| `FR-012` | Locked selection supports move、edge／corner resize and reselection. | `SPEC-0005-AC-006` | No locked-selection mode、handles、move or reselection behavior exists. | No coverage. | Not verified. | `Missing` | Implement selection revisions and interaction handles before annotation work. |
| `FR-013` | Function bar appears after valid selection lock. | `SPEC-0005-AC-007` | No editing function bar exists; current top-right stack only contains instruction text and Cancel during selection. | No coverage. | Not verified. | `Missing` | Add mandatory editing／confirmation bar owned by FEAT-002. |
| `FR-014` | Editing stage always appears; annotation actions may be skipped. | `SPEC-0003-AC-003`、`SPEC-0009-AC-001` | Workflow states contain no `SelectionLocked` or `Editing`; release bypasses confirmation. | Existing state tests validate the superseded state model. | Not verified. | `Incorrect` | Replace workflow state model before adding tools. |
| `FR-015` | Editing offers Complete、Save and Cancel. | `SPEC-0003-AC-004` | No Complete or Save controls; Cancel exists only during selection. | No coverage. | Not verified. | `Missing` | Add three explicit commitments with separate outcomes. |
| `FR-016` | Function bar remains visible below or above selection. | `SPEC-0005-AC-007` | No function bar or positioning policy exists. | No coverage. | Not verified. | `Missing` | Implement placement against available display work areas. |
| `FR-017`–`FR-023` | Rectangle、Arrow／Line、Highlighter、Text、Mosaic／Blur、Numbered Marker、color and thickness. | `SPEC-0009-AC-002`、`003`、`007`–`009` | Repository source inventory contains no annotation document、tool、object or editor implementation. | No annotation tests. | Not verified. | `Missing` | Create the accepted annotation object model and tools; do not add deferred tools. |
| `FR-024`–`FR-025` | Annotation Undo／Redo; selection changes excluded from that history. | `SPEC-0009-AC-004` | No annotation history or selection revision history exists. | No coverage. | Not verified. | `Missing` | Implement command／revision history scoped only to annotations. |
| `FR-026`–`FR-029` | Annotation geometry uses Frozen Virtual Desktop coordinates and is clipped without deletion or transformation. | `SPEC-0009-AC-005`–`006` | No annotation geometry exists; current `DisplayContextSnapshot` represents one monitor only. | Single-display coordinate tests only. | Not verified. | `Missing` | Implement Virtual Desktop annotation coordinates after the multi-display model is stable. |
| `FR-030` | Explicit Complete renders current revision, writes Clipboard, and ends only after success. | `SPEC-0007-AC-002`、`SPEC-0008-AC-001` | Crop／Clipboard foundation exists, but it is triggered by mouse release and has no annotation render revision. | Coordinator and Clipboard tests cover technical delivery. | Clipboard foundation verified; explicit Complete not verified. | `Incorrect` | Reuse adapters behind an explicit Editing commit boundary. |
| `FR-031` | Save As、PNG only、timestamp filename proposal. | `SPEC-0008-AC-002` | In-memory `PngEncoder` exists; no Save As UI、file adapter or filename policy exists. | PNG stream encoding test exists. | File save not verified. | `Partial` | Reuse encoder; add Windows Save As and local file delivery. |
| `FR-032` | Save writes the same final image to PNG and Clipboard, then ends. | `SPEC-0007-AC-003`、`SPEC-0008-AC-004` | No Save orchestration or output adapter implementation exists. | No integration coverage. | Not verified. | `Missing` | Implement one render identity shared by file and Clipboard operations. |
| `FR-033` | Save As cancellation returns to Editing. | `SPEC-0008-AC-003` | No Save dialog or Editing state exists. | No coverage. | Not verified. | `Missing` | Add distinct `SaveDialogCancelled` outcome without session cancellation. |
| `FR-034` | Save／Clipboard failure preserves Editing and provides actionable error. | `SPEC-0006-AC-003`、`SPEC-0007-AC-005` | Coordinator can retain a cropped result on retryable Clipboard failure, but `MainWindow` has already left selection and disposes the capture session; no annotations or Editing state remain. | Retryable Clipboard result is unit tested, not editing preservation. | Runtime did not verify failure recovery. | `Incorrect` | Retain full session／selection／annotation revision and return to Editing. |
| `FR-035` | Successful completion is silent. | `SPEC-0006-AC-006` | Main window is shown and `StatusText` becomes `Capture copied to Clipboard.`. | No product-level test. | Runtime explicitly verified success status. | `Incorrect` | Close capture UI silently and restore the previous app without foregrounding MainWindow. |
| `FR-036` | Esc before selection cancels the entire session. | `SPEC-0005-AC-008` | Esc works only after the one-display selection surface has been created and focused; no resident／freeze-stage cancellation path is modeled. | Cancellation coordinator tests cover resource cleanup. | Cancel from selection surface verified. | `Partial` | Add cancellation from capture request／freezing and all overlays. |
| `FR-037` | Esc during drag cancels drag and session. | `SPEC-0005-AC-008` | Selection surface KeyDown calls cancellation, but there is no cross-display pointer／overlay lifecycle. | No UI input test. | Not specifically verified during active drag. | `Partial` | Verify pointer capture release、all-overlay cleanup and focus restoration. |
| `FR-038` | Esc during Editing cancels entire session. | `SPEC-0006-AC-001` | No Editing state exists. | No coverage. | Not verified. | `Missing` | Add Editing cancellation path. |
| `FR-039` | Cancel writes neither Clipboard nor file. | `SPEC-0005 §9` | Current selection Cancel does not call output services. | Cancellation tests skip crop／Clipboard. | Packaged Cancel verified. | `Conforms` | Preserve this invariant in all new states. |
| `FR-040` | Complete、successful Save and Cancel close all capture UI. | `SPEC-0006-AC-005` | Current selection surface closes, but no function bar exists and MainWindow remains visible. | No UI lifecycle tests. | Partial only. | `Partial` | Introduce capture-UI ownership and cleanup across all display surfaces. |
| `FR-041` | Restore the pre-capture application focus. | `SPEC-0006-AC-005`、`SPEC-0010-AC-007` | No pre-capture foreground context is recorded or restored. | No coverage. | Not verified. | `Missing` | Add foreground-context snapshot and bounded restoration service. |
| `FR-042` | Do not show MainWindow after session end. | `SPEC-0010-AC-007` | `LeaveSelectionMode` reveals `CommandBar`; capture setup also calls `_appWindow.Show()`. | No UI lifecycle tests. | Runtime returns to MainWindow status. | `Incorrect` | Keep normal windows hidden and return to resident state. |
| `FR-043` | Exclude SnipPlus normal windows from frozen content. | `SPEC-0005-AC-009` | Current MainWindow uses display affinity and is hidden before one-display acquisition. No multi-window／multi-display policy exists. | Platform capture test does not assert SnipPlus exclusion. | Synthetic runtime cannot prove desktop exclusion. | `Partial` | Generalize exclusion to every normal SnipPlus window and verify with authorized runtime test. |
| `FR-044` | Failures are not silently reported as success. | `SPEC-0006` | Capture、mapping and Clipboard failures usually set a status message; operation-specific Editing recovery is absent. | Failure classifications and retry results are unit tested. | Failure UX not verified. | `Partial` | Route typed outcomes to stage-appropriate feedback without discarding state. |
| `FR-045` | Recoverable output failure retains selection and annotations. | `SPEC-0006-AC-003`、`SPEC-0010-AC-004` | No Editing or annotation state; `MainWindow` disposes session in `finally`. | Only cropped-result retention is tested. | Not verified. | `Incorrect` | Retain Frozen Virtual Desktop、selection revision and annotation document. |
| `FR-D01`–`FR-D08` | Deferred tools and formats remain absent. | `SPEC-0004-AC-004`、`SPEC-0009 §8` | No opaque pen、ellipse、pin、OCR、history、delay、extra formats or advanced text-style implementation found. | No exclusion tests. | Not applicable. | `Conforms` | Keep excluded during v1 correction. |

## 6. Non-functional Requirements Conformance

| Requirement ID | Quality obligation | Current evidence | Status | Required action |
| --- | --- | --- | --- | --- |
| `NFR-001` | PrintScreen starts without avoidable delay. | No PrintScreen entry exists. | `Missing` | Measure only after resident takeover exists; do not invent a latency target. |
| `NFR-002` | Multi-display freeze、selection and annotation remain responsive. | Only one-display selection exists; no annotation or multi-display runtime evidence. | `Missing` | Design asynchronous acquisition and render without blocking the interaction thread. |
| `NFR-003` | Render、save and Clipboard work do not freeze interaction. | Capture／Clipboard methods are async; no Editing、Save progress or recoverable UI exists. | `Partial` | Preserve UI responsiveness through explicit committing states. |
| `NFR-004` | One session owns stable frozen frames until termination. | One immutable `FrozenCaptureFrame` exists and is disposed idempotently; no all-display frame set. | `Partial` | Generalize ownership to `FrozenDisplayFrames[]`. |
| `NFR-005` | Selection、annotations and outputs share session／coordinate snapshot. | Session IDs exist for capture／Clipboard; no annotation revision、Virtual Desktop context or output revision validation. | `Partial` | Add session and revision identity checks at every async boundary. |
| `NFR-006` | Cleanup is idempotent and closes all capture UI. | Frozen-frame disposal is idempotent and tested; only one selection surface exists. | `Partial` | Add aggregate cleanup for overlays、bar、pointer capture and focus resources. |
| `NFR-007` | Recoverable failure preserves selection／annotations. | Current UI discards capture session after output attempt. | `Incorrect` | Return to Editing with state retained. |
| `NFR-008` | Completion requires all output obligations. | Production path waits for Clipboard, but coordinator can complete with a null Clipboard service and Save obligations do not exist. | `Incorrect` | Separate diagnostic paths from product completion and model Save obligations. |
| `NFR-009` | Cross-display rectangular selection. | Not implemented. | `Missing` | Implement Virtual Desktop selection. |
| `NFR-010` | Negative origins and arbitrary display arrangement. | `PhysicalRect` and one coordinate test accept a negative monitor origin; no display topology model exists. | `Partial` | Add display collection and gap-aware Virtual Desktop bounds. |
| `NFR-011` | Mixed-DPI mapping is deterministic. | One-display scalar conversion and rounding tests exist; no mixed-DPI cross-monitor mapping. | `Partial` | Add per-display transforms and boundary tests. |
| `NFR-012` | Selection changes do not corrupt annotation geometry. | No annotation or adjustable selection exists. | `Missing` | Anchor annotation geometry to Frozen Virtual Desktop coordinates. |
| `NFR-013` | Invalid topology／DPI produces classified failure. | Frame-size mismatch and out-of-bounds mapping are rejected; no topology-change detection. | `Partial` | Add topology／coordinate-version validation for the complete session. |
| `NFR-014` | Familiar PrintScreen、crosshair、mask behavior. | Mask behavior exists on one display; PrintScreen、explicit crosshair and cross-monitor behavior are absent. | `Partial` | Complete the accepted entry and presentation. |
| `NFR-015` | Mouse release is not final commitment. | Current release immediately crops and publishes Clipboard. | `Incorrect` | Add locked-selection and Editing states. |
| `NFR-016` | Editing stage always available. | Not implemented. | `Missing` | Implement before annotations or output expansion. |
| `NFR-017` | Function bar remains visible. | Not implemented. | `Missing` | Implement display-aware placement. |
| `NFR-018` | Silent completion restores work context. | MainWindow reappears and success status is shown; no focus restoration. | `Incorrect` | Implement silent cleanup and context restoration. |
| `NFR-019` | Actionable operation-specific errors. | Status messages exist, but no retained retry context. | `Partial` | Bind typed failures to retry／cancel actions in Editing. |
| `NFR-020` | Record pre-capture foreground context. | Not implemented. | `Missing` | Add to immutable session context. |
| `NFR-021` | Exclude SnipPlus normal windows. | MainWindow hide／display-affinity foundation exists for one window. | `Partial` | Cover all normal windows and displays. |
| `NFR-022` | Do not foreground MainWindow after session. | Current code shows MainWindow. | `Incorrect` | Return to resident state without foreground UI. |
| `NFR-023` | Disable／exit releases PrintScreen interception. | No interception exists. | `Missing` | Implement lifecycle-safe registration and release. |
| `NFR-024` | Capture requires explicit user action. | Current product path requires Start Capture button; no automatic background capture. | `Conforms` | Preserve for PrintScreen and authorized secondary entry. |
| `NFR-025` | Screen、annotation and Clipboard data remain local. | No cloud／external transfer code found. | `Conforms` | Preserve. |
| `NFR-026` | No private screenshot／Clipboard evidence committed. | Current recorded evidence uses synthetic data and states Clipboard was cleared. | `Conforms` | Preserve. |
| `NFR-027` | Normal work does not launch Paint／external GUI. | No external-process launch source exists; runtime correction used internal synthetic source. | `Conforms` | Preserve. |
| `NFR-028` | External-window verification requires current authorization. | AGENTS and test categorization establish this boundary. | `Conforms` | Preserve and record authorization per run. |
| `NFR-029` | Accessible names and state for controls. | Basic buttons have visible text, but accepted controls and tool-state accessibility do not exist. | `Missing` | Add AutomationProperties／state semantics with the function bar. |
| `NFR-030` | Esc works before selection、during drag and Editing. | Selection-surface Esc exists; freeze and Editing stages are absent. | `Partial` | Add stage-complete keyboard cancellation tests. |
| `NFR-031` | Color is not the only state indicator. | Tool selection and error visual model do not exist. | `Missing` | Require icon／shape／label or other non-color indicators. |
| `NFR-032` | Small canonical document set. | Accepted PRD／Specs／Contracts are consolidated; this file reuses the existing matrix. | `Conforms` | Do not create another audit document. |
| `NFR-033` | Historical documents do not override baseline. | Readiness and AGENTS explicitly establish current precedence. | `Conforms` | Preserve. |
| `NFR-034` | Every capability and test traces to accepted requirements. | Product documents now trace; existing code／test names do not yet trace to v1 IDs and mostly test superseded behavior. | `Partial` | Add requirement／AC references when correcting each slice. |
| `NFR-035` | Unknown behavior is not silently invented. | Current accepted docs retain display-gap、latency、keyboard and rollback questions; current implementation predates the revised baseline. | `Partial` | Stop at explicit unresolved visible behavior. |
| `NFR-036` | COMP-001 is sole state authority. | `WorkflowStateAuthority` is the shared authority and platform adapters return outcomes. | `Conforms` | Replace its obsolete state graph without changing ownership. |
| `NFR-037` | Windows Desktop first-release platform. | WinUI 3／Windows App SDK／Windows adapters only. | `Conforms` | Preserve. |
| `NFR-038` | Verify approved Windows 11 multi-display configuration. | Windows capture support and one primary-display frame were verified; accepted multi-display workflow was not. | `Partial` | Add authorized multi-display verification after implementation. |
| `NFR-039` | HDR、ARM64、cross-platform and extra formats remain deferred. | No implementation of those capabilities found. | `Conforms` | Preserve exclusion. |

## 7. Specification Acceptance-Criteria Conformance

| Acceptance criterion | Current conformance | Status |
| --- | --- | --- |
| `SPEC-0003-AC-001` | Current state model has no `SelectionLocked`／`Editing`; release proceeds to capture. | `Incorrect` |
| `SPEC-0003-AC-002` | No cross-display selection context. | `Missing` |
| `SPEC-0003-AC-003` | No mandatory confirmation stage. | `Missing` |
| `SPEC-0003-AC-004` | Complete／Save／Cancel are not implemented as distinct actions. | `Missing` |
| `SPEC-0003-AC-005` | Retryable failure does not restore Editing state. | `Incorrect` |
| `SPEC-0003-AC-006` | MainWindow exclusion is partial; focus restoration is absent. | `Partial` |
| `SPEC-0004-AC-001`–`004` | Canonical features map to Specs and deferred capabilities remain excluded. | `Conforms` |
| `SPEC-0005-AC-001` | PrintScreen takeover does not exist. | `Missing` |
| `SPEC-0005-AC-002`–`003` | All-display freeze and cross-display selection do not exist. | `Missing` |
| `SPEC-0005-AC-004` | Clear-inside／dim-outside works only on one display. | `Partial` |
| `SPEC-0005-AC-005` | Mouse release currently writes Clipboard. | `Incorrect` |
| `SPEC-0005-AC-006`–`007` | Locked adjustment and function bar do not exist. | `Missing` |
| `SPEC-0005-AC-008` | Esc works only in current selection surface and does not restore prior focus. | `Partial` |
| `SPEC-0005-AC-009` | MainWindow exclusion has a one-window foundation but lacks runtime proof. | `Partial` |
| `SPEC-0005-AC-010` | Frame-size／crop validation exists; full topology mismatch handling does not. | `Partial` |
| `SPEC-0006-AC-001` | Esc coverage is incomplete. | `Partial` |
| `SPEC-0006-AC-002` | Save As cancellation path does not exist. | `Missing` |
| `SPEC-0006-AC-003` | Recoverable output failure discards the editing session. | `Incorrect` |
| `SPEC-0006-AC-004` | Current Clipboard path waits for delivery, but Save and explicit Complete do not exist. | `Partial` |
| `SPEC-0006-AC-005` | Capture surface closes, but MainWindow reappears and focus restoration is absent. | `Incorrect` |
| `SPEC-0006-AC-006` | Current success status is visible. | `Incorrect` |
| `SPEC-0006-AC-007` | No session／revision stale-outcome validation exists. | `Missing` |
| `SPEC-0006-AC-008` | Frozen frame and current cleanup operations are idempotent and unit tested. | `Conforms` |
| `SPEC-0007-AC-001` | Mouse release writes Clipboard. | `Incorrect` |
| `SPEC-0007-AC-002` | Clipboard adapter works, but there is no explicit Complete or annotation render. | `Incorrect` |
| `SPEC-0007-AC-003`–`004` | Save integration and Save-dialog cancellation do not exist. | `Missing` |
| `SPEC-0007-AC-005` | Clipboard failure does not preserve Editing. | `Incorrect` |
| `SPEC-0007-AC-006` | Request IDs exist, but returned delivery IDs／revision are not validated before state advancement. | `Missing` |
| `SPEC-0007-AC-007` | History／roaming disabled; bounded cancellable retry is implemented and tested. | `Conforms` |
| `SPEC-0008-AC-001` | Clipboard publication is not behind explicit Complete. | `Incorrect` |
| `SPEC-0008-AC-002` | PNG encoder exists; Save As and timestamp filename do not. | `Partial` |
| `SPEC-0008-AC-003`–`006` | Save cancellation、dual output and failure preservation do not exist. | `Missing` |
| `SPEC-0008-AC-007` | Current crop excludes selection UI because it crops the frozen source; annotation／function-bar render integration is absent. | `Partial` |
| `SPEC-0008-AC-008` | No output revision or stale-callback protection exists. | `Missing` |
| `SPEC-0009-AC-001`–`009` | No editing stage、annotation object model、required tools、history or clipping behavior exists. | `Missing` |
| `SPEC-0010-AC-001` | Current integrated path goes directly from mouse release to Clipboard. | `Incorrect` |
| `SPEC-0010-AC-002` | One-frame session identity exists, but all-display、annotation and output revision context is absent. | `Partial` |
| `SPEC-0010-AC-003` | Complete and Save responsibilities are not implemented. | `Missing` |
| `SPEC-0010-AC-004` | Recoverable output failure does not retain Editing. | `Incorrect` |
| `SPEC-0010-AC-005` | COMP-001 remains the sole shared state authority. | `Conforms` |
| `SPEC-0010-AC-006` | Stale-session／revision outcomes are not rejected. | `Missing` |
| `SPEC-0010-AC-007` | Previous focus is not restored and MainWindow is reopened. | `Incorrect` |

## 8. Code Classification

| Current asset | Classification | Disposition |
| --- | --- | --- |
| `WindowsGraphicsCaptureAdapter` one-display acquisition | `Partial` reusable foundation | Generalize through a multi-display session orchestrator; do not make one adapter own product workflow. |
| `FrozenCaptureFrame` | `Partial` reusable foundation | Evolve into per-display frozen-frame ownership under one session. |
| `CoordinateMapper.CreateMonitorIntent` | `Partial` reusable foundation | Keep per-display mapping utility; add Virtual Desktop and cross-display intersection mapping. |
| One-display selection mask | `Partial` reusable UI behavior | Preserve clear-inside／dim-outside semantics while replacing single-window assumptions. |
| `SelectionCanvas_PointerReleased → CompleteSelectionAsync` | `Obsolete` product behavior | Remove direct output; transition to locked selection and Editing. |
| Current `WorkflowState` graph | `Obsolete` product behavior | Replace with accepted `ResidentReady → Freezing → Selecting → SelectionLocked → Editing → Committing／Saving` graph. |
| Current immediate crop／Clipboard orchestration | `Partial` technical foundation、`Incorrect` product placement | Reuse crop and Clipboard services only behind explicit Complete／Save commitments. |
| Clipboard retry adapter | `Conforms` technical foundation | Preserve bounded retry、cancellation、history and roaming defaults. |
| `PngEncoder` | `Partial` reusable foundation | Reuse inside a user-directed Save As file-delivery service. |
| Existing tests | `Partial` historical foundation | Keep low-level deterministic tests; supersede workflow tests that assert mouse-release-to-Clipboard behavior. |

## 9. Required Correction Order

No feature coding should start from the old workflow sequence. Correct implementation in this order:

1. Resident lifecycle and takeover setting.
2. PrintScreen entry integrated with COMP-001.
3. Frozen Virtual Desktop session context and per-display frame ownership.
4. Virtual Desktop presentation、crosshair and cross-monitor initial selection.
5. Locked selection、move、edge／corner resize and reselection.
6. Accepted workflow state graph including `SelectionLocked` and `Editing`.
7. Function bar、Complete／Save／Cancel commitments and focus restoration.
8. Annotation document、required tools and object editing.
9. Annotation-only Undo／Redo、Virtual Desktop anchoring and selection clipping.
10. Complete final render plus Clipboard.
11. Save As、PNG file output plus the same Clipboard result.
12. Recoverable failure preservation、stale-revision protection and accessibility verification.
13. Authorized multi-display runtime verification.

Each correction task must update this matrix row from `Missing`／`Incorrect`／`Partial` only after code、tests and applicable runtime evidence exist.

## 10. Blocking Open Product Decisions

The following remain `Blocked by product decision` only when implementation reaches them:

- Representation of non-display gaps in irregular monitor layouts.
- Exact system-tray menu and MainWindow close-button behavior.
- Quantitative latency targets.
- Final keyboard-only annotation acceptance standard.
- Rollback／retention behavior when PNG creation succeeds but Clipboard publication fails.

These decisions do not justify silently reverting to single-monitor、immediate Clipboard or no-editing behavior.

## 11. Final Conclusion

Current code should be treated as a tested single-display capture／crop／Clipboard foundation. It does not conform to the accepted SnipPlus v1 product workflow.

The next coding task must begin with the first `Missing` prerequisite in the correction order—not with Annotation、Clipboard hardening、Packaging or additional output features.
