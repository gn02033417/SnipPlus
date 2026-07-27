# SnipPlus v1 Requirements-to-Code Conformance Matrix

狀態：`Reviewed — implementation correction required`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `PRD-TRACEABILITY-MATRIX-001` |
| Version | `2.2` |
| Review date | `2026-07-27` |
| Product baseline | Accepted `PRD-0004`–`PRD-0006` current revisions and `SPEC-0003`–`SPEC-0010` |
| Reviewed implementation | Branch `remove-snipplus-md` technical prototype |
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

## 3. Evidence Inventory

### Current source foundations

- `src/SnipPlus.App/App.xaml.cs`
- `src/SnipPlus.App/MainWindow.xaml`
- `src/SnipPlus.App/MainWindow.xaml.cs`
- `src/SnipPlus.Core/CaptureWorkflowCoordinator.cs`
- `src/SnipPlus.Core/WorkflowStateAuthority.cs`
- `src/SnipPlus.Core/CoordinateMapper.cs`
- `src/SnipPlus.Contracts/CaptureContracts.cs`
- `src/SnipPlus.Contracts/WorkflowContracts.cs`
- `src/SnipPlus.Contracts/DeliveryContracts.cs`
- `src/SnipPlus.Windows/WindowsGraphicsCaptureAdapter.cs`
- `src/SnipPlus.Windows/WinRtClipboardDeliveryAdapter.cs`
- `src/SnipPlus.Windows/PngEncoder.cs`

### Current tests

- Contracts、Core and Windows test projects exist.
- Deterministic tests cover one-display capture／crop、SoftwareBitmap、PNG encoding、Clipboard retry and portions of cleanup.
- No tests currently prove resident PrintScreen、all-display freeze、capacity rejection、cross-monitor Selection、Editing、Annotation、keyboard-only operation、Save As、transparent gap output、quantitative performance or focus restoration.

### Historical runtime boundary

The packaged synthetic verification demonstrated only:

```text
Start Capture button
→ one display frozen frame
→ drag Selection
→ mouse release
→ immediate crop／Clipboard
→ main-window status
```

This remains evidence for the one-display technical foundation only. It is invalid as v1 workflow-conformance evidence.

## 4. Executive Result

Current source is a reusable **single-display capture／crop／PNG／Clipboard technical prototype**, not a conforming SnipPlus v1 implementation.

Reusable foundations:

- one-display WGC acquisition;
- same-frame crop;
- clear-inside／dim-outside mask on one surface;
- single-display coordinate conversion;
- BGRA8 premultiplied SoftwareBitmap;
- PNG encoder;
- Clipboard delivery with bounded cancellable retry;
- shared state authority;
- deterministic low-level tests.

Blocking implementation gaps:

- resident lifecycle and PrintScreen takeover;
- direct-exit behavior with takeover release;
- capacity policy and unsupported-configuration failure;
- Frozen Virtual Desktop and per-display frame ownership;
- cross-monitor Selection and transparent gap output;
- SelectionLocked adjustment;
- accepted state graph;
- function bar and Annotation;
- keyboard focus、commands and keyboard object creation;
- Complete／Save commitment boundaries and progress state;
- Downloads-default Save As and retained-file partial outcome;
- quantitative timing、memory and repeated-session evidence;
- focus restoration and retained Editing after recoverable failure.

## 5. Functional Requirements Conformance

| Requirement | Accepted behavior | Current evidence | Status | Required action |
| --- | --- | --- | --- | --- |
| `FR-001` | Manual startup keeps SnipPlus resident while running. | App creates one MainWindow; no explicit resident service. | `Missing` | Implement application-owned resident lifecycle. |
| `FR-002`–`FR-004` | User controls PrintScreen takeover; disabled or exited app does not intercept. | No takeover service or setting. | `Missing` | Add persisted setting、registration and exact release. |
| `FR-005` | In-app capture is secondary. | It is currently the only entry. | `Incorrect` | Retain only after PrintScreen exists. |
| `FR-046`–`FR-047` | MainWindow `X` and explicit Exit terminate SnipPlus、release takeover and do not hide to tray. | Current close ends the window workflow but no takeover exists; no accepted exit service or tests. | `Partial` | Use one application-exit boundary; do not implement close-to-tray. |
| `FR-006`–`FR-010` | Freeze and present all displays、crosshair、cross-monitor Selection and mask behavior. | One display only; mask foundation exists. | `Missing／Partial` | Add Virtual Desktop snapshot、capacity validation、per-display frames and surfaces. |
| `FR-011` | Mouse release locks Selection and creates no output. | Release invokes crop／Clipboard. | `Incorrect` | Replace with `SelectionLocked → Editing`. |
| `FR-012` | Locked Selection supports move、edge／corner resize and reselection. | Not present. | `Missing` | Implement Selection revisions and pointer／keyboard handles. |
| `FR-048` | Non-display gap pixels in final output are transparent. | No multi-display composition or transparent gap tests. | `Missing` | Compose gaps as BGRA alpha `0`. |
| `FR-013`–`FR-016` | Mandatory Editing／confirmation function bar and Complete／Save／Cancel. | No Editing state or function bar. | `Missing／Incorrect` | Replace state graph and add function bar. |
| `FR-017`–`FR-023` | Required Annotation tools and styling. | No Annotation model or tools. | `Missing` | Implement pointer and keyboard object creation for the accepted tool set. |
| `FR-024`–`FR-025` | Annotation-only Undo／Redo. | Not present. | `Missing` | Add Annotation history independent of Selection revisions. |
| `FR-026`–`FR-029` | Frozen Virtual Desktop Annotation coordinates and clipping. | No Annotation model; one-display mapper only. | `Missing` | Implement after Virtual Desktop model stabilizes. |
| `FR-030` | Complete renders current revision and writes Clipboard only. | Crop／Clipboard exists at wrong mouse-release boundary. | `Incorrect` | Reuse delivery behind explicit Complete with progress. |
| `FR-031`、`FR-049` | Save As is PNG-only、initially opens Downloads、uses timestamp name and allows destination/name changes. | PNG encoder exists; Save As and destination policy do not. | `Partial` | Add Windows Save As with accepted defaults. |
| `FR-032` | Save sends the same final result to PNG and Clipboard. | No Save orchestration. | `Missing` | Share one Result ID across both operations. |
| `FR-033` | Save As cancel returns to Editing. | No Save dialog or Editing state. | `Missing` | Add `SaveDialogCancelled` outcome and focus restoration. |
| `FR-034`、`FR-045` | Recoverable output failure preserves Editing and actionable feedback. | Current UI disposes session after delivery attempt. | `Incorrect` | Retain frames、Selection、Annotation revision and keyboard focus context. |
| `FR-050` | Clipboard failure after PNG success retains PNG and returns to Editing. | No file output or retained-file outcome. | `Missing` | Add `RetainedFileReference`; never roll back the PNG. |
| `FR-035` | Success is silent. | MainWindow shows success status. | `Incorrect` | Restore prior application without success notification. |
| `FR-036`–`FR-038` | Esc cancels at capture and stable Editing stages. | Selection-stage Esc foundation only; transient hierarchy absent. | `Partial／Missing` | Implement transient Esc dismissal and stable-Editing cancellation. |
| `FR-039` | Cancel produces no output. | Existing Selection Cancel skips output. | `Conforms` | Preserve. |
| `FR-040`–`FR-043` | Cleanup、focus restoration、no MainWindow reopening、window exclusion. | Partial one-window hide／cleanup; no focus restoration. | `Partial／Incorrect` | Add session UI ownership and foreground-context service. |
| `FR-044` | Failures are never reported as success. | Typed failures exist, but retained Editing feedback does not. | `Partial` | Route typed outcomes to Editing feedback. |
| `FR-D01`–`FR-D08` | Deferred tools and formats remain absent. | None found. | `Conforms` | Preserve exclusion. |

## 6. Non-functional Conformance

| Requirement area | Accepted obligation | Current result | Status | Required action |
| --- | --- | --- | --- | --- |
| `NFR-001`–`NFR-003` | p95 capture、interaction、output、progress and memory targets. | Async foundations exist; no accepted measurement harness or evidence. | `Missing／Partial` | Add deterministic timers、30-run reporting、memory scenarios and release evidence. |
| `NFR-004`–`NFR-008` | Stable session and output integrity. | One frame and disposal foundation exist; accepted session/output obligations do not. | `Partial／Incorrect` | Generalize ownership and completion contracts. |
| `NFR-009`–`NFR-013` | Cross-display correctness、capacity limits and typed over-limit failure. | One-display and limited negative-origin tests only; no capacity policy. | `Missing／Partial` | Add envelope model、boundary tests、mixed-DPI、gap-alpha and cross-display tests. |
| `NFR-014`–`NFR-019` | Familiar interaction、silent success、progress and retained-error feedback. | One-display mask exists; immediate output and visible success conflict. | `Partial／Incorrect` | Implement explicit Editing、300 ms progress and feedback. |
| `NFR-020`–`NFR-023` | Focus／exit. | No foreground restoration or takeover service. `X` currently closes rather than hiding, but exact release is unimplemented. | `Partial／Missing` | Implement direct exit with exact takeover release. |
| `NFR-024`–`NFR-028` | Privacy／verification. | Local-only and synthetic-evidence boundaries are present. | `Conforms` | Preserve. |
| `NFR-029`–`NFR-031` | Complete keyboard-only Editing after Selection lock、visible focus and accessibility. | Accepted controls and Annotation model do not exist. | `Missing` | Implement F6／Tab model、shortcuts、default object creation、IME、High Contrast、200% and Narrator tests. |
| `NFR-032`–`NFR-036` | Maintainability. | Canonical documents and state authority are established. | `Conforms／Partial` | Trace each corrected slice to requirements and tests. |
| `NFR-037`–`NFR-039` | Windows v1 envelope and deferred compatibility. | Windows x64 foundation exists; envelope unimplemented and unverified. | `Missing／Partial` | Implement and verify all capacity boundaries. |

## 7. Acceptance-Criteria Conformance

| Spec area | Current result | Status |
| --- | --- | --- |
| `SPEC-0003` state／exit／gap／retention／performance／capacity／keyboard | Old state graph、no exit service、no capacity model、no measurement or keyboard workflow. | `Incorrect／Missing` |
| `SPEC-0004` feature catalog／deferred exclusion | Canonical features are documented; deferred capabilities absent. | `Conforms` |
| `SPEC-0005` entry／capacity／multi-display／Selection | One-display mask foundation; all-display entry、capacity and locked Selection missing. | `Partial／Missing／Incorrect` |
| `SPEC-0006` cancel／progress／failure／exit feedback | Cleanup foundations exist; transient Esc、progress、retained Editing and partial-save feedback missing. | `Partial／Missing` |
| `SPEC-0007` Clipboard | Adapter conforms technically; commitment placement and retained-file relationship missing. | `Partial／Incorrect` |
| `SPEC-0008` Output | Encoder exists; Save As、Downloads default、transparent gaps、progress and retained PNG missing. | `Partial／Missing` |
| `SPEC-0009` Annotation and keyboard | No Annotation model、focus model、commands or keyboard-only acceptance evidence. | `Missing` |
| `SPEC-0010` integration | Current path is mouse-release-to-Clipboard and reopens MainWindow; no capacity/performance integration. | `Incorrect` |

## 8. Code Classification

| Current asset | Classification | Disposition |
| --- | --- | --- |
| One-display `WindowsGraphicsCaptureAdapter` | `Partial` reusable foundation | Generalize under a capacity-aware multi-display session orchestrator. |
| `FrozenCaptureFrame` | `Partial` reusable foundation | Evolve into session-owned per-display frames. |
| `CoordinateMapper` | `Partial` reusable foundation | Add Virtual Desktop intersections、keyboard increments and transparent gaps. |
| One-display mask | `Partial` reusable behavior | Preserve clear-inside／dim-outside semantics. |
| `SelectionCanvas_PointerReleased → CompleteSelectionAsync` | `Obsolete` | Remove direct output. |
| Current `WorkflowState` graph | `Obsolete` | Replace with accepted states including `SelectionLocked` and `Editing`. |
| Crop／Clipboard orchestration | `Partial` technical foundation、`Incorrect` placement | Move behind Complete／Save and progress boundary. |
| Clipboard retry adapter | `Conforms` technical foundation | Preserve bounded retry and privacy defaults. |
| `PngEncoder` | `Partial` reusable foundation | Use in Save As and verify alpha／size tiers. |
| Current workflow tests | `Partial` historical foundation | Supersede tests asserting release-to-Clipboard and add quality acceptance tests. |

## 9. Required Correction Order

1. Resident lifecycle、direct MainWindow exit and takeover setting.
2. PrintScreen entry integrated with `COMP-001`.
3. Capacity policy、Frozen Virtual Desktop context and per-display frame ownership.
4. All-display presentation、crosshair and cross-monitor initial Selection.
5. Locked Selection、move、edge／corner resize and reselection.
6. Accepted workflow state graph.
7. Function bar、Complete／Save／Cancel、progress and focus restoration.
8. Annotation document、required tools、keyboard focus model and object editing.
9. Annotation Undo／Redo、Virtual Desktop anchoring、Selection clipping and keyboard-only acceptance.
10. Complete final render、capacity revalidation、transparent gaps and Clipboard.
11. Save As with Downloads default、PNG、same-result Clipboard and retained-file partial outcome.
12. Recoverable failure preservation、stale-revision protection、performance／memory evidence and accessibility.
13. Explicitly authorized multi-display runtime verification at Standard and Maximum profiles.

Each correction task updates matrix status only after code、tests and applicable evidence exist.

## 10. Product Decisions

### Resolved on `2026-07-27`

- MainWindow `X` directly exits SnipPlus and releases PrintScreen takeover; no close-to-tray behavior.
- An explicit tray Exit action, if present, uses the same exit path.
- Physical non-display gaps in final output are transparent.
- Save As initially opens Downloads and allows another destination.
- PNG is retained if later Clipboard publication fails.
- Quantitative capture、interaction、output、memory and cleanup targets are fixed in `PRD-0006 §3`.
- The supported envelope is `1`–`4` displays、`7,680 × 4,320` per display、`66,355,200` total source pixels、`16,384 × 16,384` Virtual Desktop dimensions and `67,108,864` maximum Selection area.
- Complete keyboard-only Editing／Annotation acceptance begins at `SelectionLocked` and follows `SPEC-0009`.

No current v1 behavior is `Blocked by product decision`.

## 11. Final Conclusion

Current code remains a tested single-display technical foundation. The first coding task remains resident lifecycle、direct application exit and user-controlled PrintScreen takeover. Quantitative quality、capacity and keyboard standards are now available for later slices and may not be relaxed silently to accommodate implementation difficulty.