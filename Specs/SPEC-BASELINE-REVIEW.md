# Specification Baseline Review

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `SPEC-BASELINE-REVIEW-001` |
| Version | `1.3` |
| Review date | `2026-07-27` |
| Authority | Repository owner through accepted current PRD baseline |
| Decision | `Baseline Accepted` |
| Scope | `SPEC-0002` through `SPEC-0010` |

This review supersedes earlier wording that left tool behavior、capacity、performance、keyboard-only Editing or output details unresolved.

## 2. Reviewed Specifications

| Document | Current responsibility | Result |
| --- | --- | --- |
| `SPEC-0002` | Specification structure、traceability and acceptance rules | `PASS` |
| `SPEC-0003` | Shared state、exit、performance、capacity、keyboard and system invariants | `PASS` |
| `SPEC-0004` | Accepted v1 Feature catalog and deferred boundaries | `PASS` |
| `SPEC-0005` | Resident PrintScreen、capacity validation、all-display freeze and cross-monitor Selection | `PASS` |
| `SPEC-0006` | Success、progress、Esc、cancel、failure、cleanup、exit and focus | `PASS` |
| `SPEC-0007` | Clipboard commitment and retained-file relationship | `PASS` |
| `SPEC-0008` | Downloads-default PNG Save As、transparent output and retained PNG | `PASS` |
| `SPEC-0009` | Mandatory Editing、required Annotation tools and complete keyboard-only acceptance | `PASS` |
| `SPEC-0010` | Integrated workflow、capacity、performance and keyboard ownership | `PASS` |

## 3. Baseline Consistency

The accepted specification set consistently requires:

- manual startup、resident lifecycle and user-controlled PrintScreen takeover;
- MainWindow `X` directly exits、releases takeover and does not hide to tray;
- capacity validation before interactive Selection;
- one session-owned Frozen Virtual Desktop covering all connected supported displays;
- one rectangular Selection that can span displays;
- transparent final-image pixels for physical non-display gaps;
- mouse release to `SelectionLocked`, never directly to output;
- move、edge／corner resize and reselection before commitment;
- mandatory Editing／confirmation with optional Annotation actions;
- Rectangle、Arrow／Line、Highlighter、Text、Mosaic／Blur and Numbered Marker;
- pointer and keyboard Annotation object creation／editing;
- Annotation Undo／Redo、Virtual Desktop anchoring and output clipping;
- Complete to Clipboard only;
- Save As initially opening Downloads、PNG and the same Clipboard result;
- PNG retention when later Clipboard delivery fails;
- transient Esc dismissal before stable-Editing cancellation;
- responsive progress after `300 ms`;
- quantitative capture、interaction、output、memory and cleanup targets;
- complete keyboard-only Editing acceptance from `SelectionLocked`;
- recoverable output failure returning to Editing with state and focus context preserved;
- stale session／revision outcomes unable to advance workflow state.

No current Spec contains a valid path from mouse release directly to Clipboard or a valid partial-display capture path.

## 4. Feature and Ownership Review

| Feature | Normative owner | Required v1 boundary |
| --- | --- | --- |
| `FEAT-001` | `SPEC-0005` | Resident entry、direct exit、capacity validation、display freeze、Virtual Desktop and Selection lifecycle |
| `FEAT-002` | `SPEC-0009` | Editing function bar、keyboard focus、Annotation document、tools and Undo／Redo |
| `FEAT-003` | `SPEC-0007` | Clipboard publication after explicit commitment |
| `FEAT-004` | `SPEC-0008` | Downloads-default Save As、PNG creation and retained-file outcome |
| `FEAT-005` | `SPEC-0006` | Cancel、progress、failure preservation、cleanup、feedback and focus restoration |
| Shared integration | `SPEC-0010` | Single session／revision context、capacity、quality gates and legal feature order |

Clipboard and PNG Output remain separate capability boundaries. Save requires both to succeed before overall completion, but a successfully created PNG remains retained if later Clipboard delivery fails.

## 5. Accepted Quality Baseline

### Performance

- Capture start p95 `≤ 500 ms` Standard and `≤ 1,000 ms` Maximum.
- Interaction frame p95 `≤ 33 ms`; discrete response p95 `≤ 100 ms`.
- Complete p95 tiers `≤ 1.5 s`、`4 s`、`8 s`.
- Save p95 tiers `≤ 2 s`、`6 s`、`12 s` after Save As confirmation.
- Progress after `300 ms`.
- Idle、peak、cleanup and repeated-session memory targets from PRD-0006.

### Capacity

- `1`–`4` logical displays.
- Each `≤ 7,680 × 4,320`.
- Total source pixels `≤ 66,355,200`.
- Virtual Desktop width／height each `≤ 16,384`.
- Selection area `≤ 67,108,864` pixels with dimensional limits.
- Unsupported capacity fails before Selection without partial capture.

### Keyboard-only Editing

- Scope starts at `SelectionLocked`.
- F6／Tab focus model、tool shortcuts、default keyboard object creation、`1`／`10` pixel move／resize、IME、High Contrast、200% scaling、Narrator state and no keyboard trap are required.
- Initial crosshair Selection remains pointer-driven in v1.

## 6. Acceptance Readiness

| Area | Result |
| --- | --- |
| User-visible workflow | `PASS` |
| State、exit and commitment boundaries | `PASS` |
| Multi-display、capacity、coordinates and transparent gaps | `PASS` |
| Annotation tool and object requirements | `PASS` |
| Complete keyboard-only Editing standard | `PASS` |
| Clipboard、Downloads Save As and retained PNG behavior | `PASS` |
| Cancellation、progress and recoverable failure behavior | `PASS` |
| Quantitative performance and memory acceptance | `PASS` |
| Privacy and external-GUI evidence boundaries | `PASS` |
| Stable acceptance-criterion namespaces | `PASS` |
| Current code conformance | `FAIL — tracked separately in PRD-TRACEABILITY-MATRIX-001` |

Specification acceptance does not imply implementation completion.

## 7. Product Decision Status

No visible v1 product or quality decision remains open. Any change to targets、capacity or keyboard scope requires explicit Repository owner approval.

## 8. Baseline Decision

`SPEC-0002` through `SPEC-0010` form the accepted complete SnipPlus v1 Specification baseline.

Architecture、code、tests and runtime verification must conform to this baseline. Historical Research and prior reviews are non-normative where they conflict. Implementation gaps stay in the existing conformance matrix; they do not justify another readiness or closure-document chain.