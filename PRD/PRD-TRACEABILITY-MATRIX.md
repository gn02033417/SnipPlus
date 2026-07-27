# SnipPlus v1 Requirements-to-Code Conformance Matrix

狀態：`Reviewed — implementation correction required`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `PRD-TRACEABILITY-MATRIX-001` |
| Version | `2.3` |
| Review date | `2026-07-27` |
| Product baseline | Accepted current `PRD-0004`–`PRD-0006` and `SPEC-0003`–`SPEC-0010` |
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

## 3. Executive Result

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

Major implementation gaps:

- resident lifecycle and PrintScreen takeover;
- direct-exit behavior with exact takeover release;
- four-4K capacity policy and typed over-limit failure;
- Frozen Virtual Desktop and per-display frame ownership;
- cross-monitor Selection and transparent gap output;
- SelectionLocked adjustment;
- accepted state graph;
- function bar and pointer-driven Annotation;
- Complete／Save commitment boundaries and progress state;
- Downloads-default Save As and retained-file outcome;
- quantitative timing、memory and repeated-session evidence;
- focus restoration and retained Editing after recoverable failure.

Keyboard-only Annotation and non-PrintScreen shortcuts are deferred and must not be classified as missing v1 work.

## 4. Functional Requirements Conformance

| Requirement | Accepted behavior | Current result | Status | Required action |
| --- | --- | --- | --- | --- |
| `FR-001` | Manual startup keeps SnipPlus resident while running. | MainWindow exists; no explicit resident service. | `Missing` | Implement application-owned resident lifecycle. |
| `FR-002`–`FR-004` | User controls PrintScreen takeover; disabled or exited app does not intercept. | No takeover service or setting. | `Missing` | Add persisted setting、registration and exact release. |
| `FR-005` | In-app capture is secondary. | It is currently the only entry. | `Incorrect` | Retain only as secondary after PrintScreen exists. |
| `FR-046`–`FR-047` | MainWindow `X` and explicit Exit terminate、release takeover and do not hide to tray. | Close foundation exists; takeover release service and tests do not. | `Partial` | Use one application-exit boundary. |
| `FR-006`–`FR-010` | Freeze and present all supported displays、crosshair、cross-monitor Selection and mask. | One display only; mask foundation exists. | `Missing／Partial` | Add capacity-aware Virtual Desktop and per-display frames. |
| `FR-011` | Mouse release locks Selection and creates no output. | Release invokes crop／Clipboard. | `Incorrect` | Replace with `SelectionLocked → Editing`. |
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
| `FR-036`–`FR-038` | Esc cancels at accepted capture stages. | Selection-stage Esc foundation only. | `Partial／Missing` | Add Esc through accepted state graph. |
| `FR-039` | Cancel produces no output. | Existing Selection cancel skips output. | `Conforms` | Preserve. |
| `FR-040`–`FR-043` | Cleanup、focus restoration、no MainWindow reopening、window exclusion. | Partial cleanup; no focus restoration. | `Partial／Incorrect` | Add foreground-context service. |
| `FR-044` | Failures are never reported as success. | Typed failures exist; retained Editing feedback does not. | `Partial` | Route typed outcomes to Editing feedback. |
| `FR-D01`–`FR-D09` | Deferred tools、formats and keyboard-only shortcuts remain absent. | No prohibited deferred implementation found. | `Conforms` | Preserve exclusion. |

## 5. Non-functional Conformance

| Requirement area | Accepted obligation | Current result | Status | Required action |
| --- | --- | --- | --- | --- |
| `NFR-001`–`NFR-003` | p95 capture、pointer interaction、output、progress and memory targets. | Async foundations exist; no accepted harness or evidence. | `Missing／Partial` | Add timers、30-run reporting and memory scenarios. |
| `NFR-004`–`NFR-008` | Stable Session and output integrity. | One-frame ownership foundation exists; accepted Session obligations do not. | `Partial／Incorrect` | Generalize ownership and completion contracts. |
| `NFR-009`–`NFR-013` | Cross-display correctness、four-4K capacity and typed over-limit failure. | One-display tests only; no capacity policy. | `Missing／Partial` | Add envelope model、mixed-DPI、gap-alpha and boundary tests. |
| `NFR-014`–`NFR-019` | Familiar interaction、silent success、progress and retained-error feedback. | One-display mask exists; immediate output and visible success conflict. | `Partial／Incorrect` | Implement Editing、300 ms progress and feedback. |
| `NFR-020`–`NFR-023` | Focus and exit behavior. | No foreground restoration or takeover service. | `Partial／Missing` | Implement direct exit with exact release. |
| `NFR-024`–`NFR-028` | Privacy and verification boundaries. | Local-only and synthetic-evidence boundaries exist. | `Conforms` | Preserve. |
| `NFR-029`–`NFR-031` | Accessible names、Esc cancellation and non-color-only state. | Accepted controls do not exist. | `Missing／Partial` | Implement only accepted baseline accessibility; do not add deferred shortcut workflow. |
| `NFR-032`–`NFR-036` | Maintainability and traceability. | Canonical documents and state authority exist. | `Conforms／Partial` | Trace every corrected slice. |
| `NFR-037`–`NFR-039` | Windows v1 four-4K envelope and deferred compatibility. | Windows x64 foundation exists; envelope unimplemented. | `Missing／Partial` | Implement and verify all capacity boundaries. |

## 6. Code Classification

| Current asset | Classification | Disposition |
| --- | --- | --- |
| One-display `WindowsGraphicsCaptureAdapter` | `Partial` reusable foundation | Generalize under a four-4K capacity-aware multi-display orchestrator. |
| `FrozenCaptureFrame` | `Partial` reusable foundation | Evolve into Session-owned per-display frames. |
| `CoordinateMapper` | `Partial` reusable foundation | Add Virtual Desktop intersections、mixed-DPI mapping and transparent gaps. |
| One-display mask | `Partial` reusable behavior | Preserve clear-inside／dim-outside semantics. |
| `SelectionCanvas_PointerReleased → CompleteSelectionAsync` | `Obsolete` | Remove direct output. |
| Current `WorkflowState` graph | `Obsolete` | Replace with accepted states including `SelectionLocked` and `Editing`. |
| Crop／Clipboard orchestration | `Partial` foundation、`Incorrect` placement | Move behind Complete／Save and progress boundary. |
| Clipboard retry adapter | `Conforms` technical foundation | Preserve bounded retry and privacy defaults. |
| `PngEncoder` | `Partial` reusable foundation | Use in Save As and verify alpha／size tiers. |
| Current workflow tests | `Partial` historical foundation | Supersede release-to-Clipboard assertions and add v1 quality tests. |

## 7. Required Correction Order

1. Resident lifecycle、direct MainWindow exit and takeover setting.
2. PrintScreen entry integrated with `COMP-001`.
3. Four-4K capacity policy、Frozen Virtual Desktop context and per-display frame ownership.
4. All-display presentation、crosshair and cross-monitor initial Selection.
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

Current code remains a tested single-display technical foundation. The first coding task remains resident lifecycle、direct application exit and user-controlled PrintScreen takeover. Quality、four-4K capacity and deferred keyboard boundaries may not be silently changed to accommodate implementation difficulty.