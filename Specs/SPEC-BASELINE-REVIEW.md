# Specification Baseline Review

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `SPEC-BASELINE-REVIEW-001` |
| Version | `1.4` |
| Review date | `2026-07-27` |
| Authority | Repository owner through accepted current PRD baseline |
| Decision | `Baseline Accepted` |
| Scope | `SPEC-0002` through `SPEC-0010` |

This review supersedes the previously accepted 8K-capable envelope and complete keyboard-only Editing／Annotation scope.

## 2. Reviewed Specifications

| Document | Current responsibility | Result |
| --- | --- | --- |
| `SPEC-0002` | Specification structure、traceability and acceptance rules | `PASS` |
| `SPEC-0003` | Shared state、exit、performance、four-4K capacity、keyboard boundary and system invariants | `PASS` |
| `SPEC-0004` | Accepted v1 Feature catalog and deferred boundaries | `PASS` |
| `SPEC-0005` | Resident PrintScreen、capacity validation、all-display freeze and cross-monitor Selection | `PASS` |
| `SPEC-0006` | Success、progress、Esc cancellation、failure、cleanup、exit and focus | `PASS` |
| `SPEC-0007` | Clipboard commitment and retained-file relationship | `PASS` |
| `SPEC-0008` | Downloads-default PNG Save As、transparent output and retained PNG | `PASS` |
| `SPEC-0009` | Mandatory Editing、required pointer-driven Annotation tools and deferred keyboard-only scope | `PASS` |
| `SPEC-0010` | Integrated workflow、capacity、performance and ownership | `PASS` |

## 3. Baseline Consistency

The accepted Specification set consistently requires:

- manual startup、resident lifecycle and user-controlled PrintScreen takeover;
- MainWindow `X` directly exits、releases takeover and does not hide to tray;
- capacity validation before interactive Selection;
- one Session-owned Frozen Virtual Desktop covering all supported displays;
- one rectangular Selection spanning displays;
- transparent final-image pixels for physical non-display gaps;
- mouse release to `SelectionLocked`, never directly to output;
- pointer move、edge／corner resize and reselection before commitment;
- mandatory Editing／confirmation with optional Annotation actions;
- Rectangle、Arrow／Line、Highlighter、Text、Mosaic／Blur and Numbered Marker;
- pointer-driven Annotation creation and object editing;
- function-bar Undo／Redo、Virtual Desktop anchoring and output clipping;
- Complete to Clipboard only;
- Save As initially opening Downloads、PNG and the same Clipboard result;
- PNG retention when later Clipboard delivery fails;
- Esc cancellation at accepted capture stages;
- non-blocking progress after `300 ms`;
- quantitative performance、memory and cleanup targets;
- recoverable output failure returning to Editing with state preserved;
- stale Session／revision outcomes unable to advance workflow state;
- keyboard-only Annotation and non-PrintScreen shortcuts deferred.

No current Spec contains a valid path from mouse release directly to Clipboard or a valid partial-display capture path.

## 4. Accepted Quality Baseline

### Performance

- Capture start p95 `≤ 500 ms` Owner Reference／Standard and `≤ 1,000 ms` Maximum.
- Pointer interaction frame p95 `≤ 33 ms`; visible response p95 `≤ 100 ms`.
- Complete p95 tiers `≤ 1.5 s`、`4 s`、`8 s`.
- Save p95 tiers `≤ 2 s`、`6 s`、`12 s` after Save As confirmation.
- Progress after `300 ms`.
- Idle、peak、cleanup and repeated-session memory targets from PRD-0006.
- Measurement uses 3 warm-ups and at least 30 measured runs with p50、p95 and maximum reporting.

### Capacity

- `1`–`4` logical displays.
- Each `≤ 3840 × 2160`.
- Total source pixels `≤ 33,177,600`.
- Virtual Desktop width／height each `≤ 16,384`.
- Selection width／height each `≤ 16,384`; area `≤ 67,108,864` pixels.
- Unsupported capacity fails before Selection without partial capture.
- 8K displays are outside v1.

### Owner Reference Runtime Profile

- primary `2560 × 1440`;
- lower `1920 × 1080` at Windows scaling `150%`;
- left `2560 × 1440`.

### Keyboard Boundary

Required:

- PrintScreen entry;
- Esc cancellation;
- ordinary text editing and Chinese IME;
- accessible control names and non-color-only state indicators.

Deferred:

- complete keyboard-only Annotation;
- F6／Tab zone and object workflow;
- tool、Ctrl、Delete and Arrow-key shortcuts;
- keyboard-created Annotation objects;
- pointer-unused acceptance after `SelectionLocked`.

## 5. Acceptance Readiness

| Area | Result |
| --- | --- |
| User-visible workflow | `PASS` |
| State、exit and commitment boundaries | `PASS` |
| Multi-display、four-4K capacity、coordinates and transparent gaps | `PASS` |
| Pointer-driven Annotation tool and object requirements | `PASS` |
| Keyboard boundary and deferred shortcut scope | `PASS` |
| Clipboard、Downloads Save As and retained PNG behavior | `PASS` |
| Cancellation、progress and recoverable failure behavior | `PASS` |
| Quantitative performance and memory acceptance | `PASS` |
| Privacy and external-GUI evidence boundaries | `PASS` |
| Current code conformance | `FAIL — tracked separately in PRD-TRACEABILITY-MATRIX-001` |

Specification acceptance does not imply implementation completion.

## 6. Product Decision Status

No visible v1 product or quality decision remains open. Any change to targets、four-4K capacity or deferred keyboard scope requires explicit Repository owner approval.

## 7. Baseline Decision

`SPEC-0002` through `SPEC-0010` form the accepted complete SnipPlus v1 Specification baseline.`

Architecture、code、tests and runtime verification must conform to this baseline. Historical Research and prior reviews are non-normative where they conflict.