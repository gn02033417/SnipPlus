# SnipPlus Implementation Readiness Review

## Document Control

| Field | Value |
| --- | --- |
| Document ID | `IMPLEMENTATION-READINESS-REVIEW-001` |
| Status | `Accepted — ready for ordered v1 conformance correction` |
| Original review date | `2026-07-26` |
| Product baseline revision | `2026-07-27` |
| Conformance audit completed | `2026-07-27` |
| Product-decision blockers updated | `2026-07-27` |
| Authority | Repository owner through explicit product decisions |
| Review scope | SnipPlus v1 first release |

## 1. Executive Decision

The previous single-display technical slice proved reusable capture、image、crop、PNG and Clipboard foundations, but it does not represent the accepted first-release product workflow.

The accepted v1 workflow requires:

- manually started resident application;
- user-controlled PrintScreen takeover;
- MainWindow `X` directly exiting the application and releasing takeover;
- all-display Frozen Virtual Desktop session;
- one cross-monitor rectangular Selection;
- transparent final-image pixels for physical non-display gaps;
- Selection lock、move、edge／corner resize and reselection;
- mandatory Editing／confirmation with optional Annotation actions;
- required v1 Annotation tool set;
- Complete to Clipboard;
- Save As initially opening Downloads、PNG creation and Clipboard;
- PNG retention when later Clipboard delivery fails;
- explicit cancellation、failure preservation、cleanup and focus restoration.

`PRD/PRD-TRACEABILITY-MATRIX.md` records the completed static comparison between accepted requirements and current code／tests／runtime evidence.

Therefore:

- no additional documentation gate is required before a focused implementation correction;
- coding must follow the matrix order and begin with the first unresolved prerequisite;
- existing code is reused only where the matrix classifies it as conforming or partial foundation;
- passing historical tests does not preserve obsolete product behavior;
- restore、build、test and runtime execution still require explicit authorization in the current task.

## 2. Effective Canonical Sources

| Priority | Source | Responsibility |
| --- | --- | --- |
| 1 | Accepted `PRD-0002`–`PRD-0006` | Product intent and v1 scope |
| 2 | Accepted `SPEC-0001`–`SPEC-0010` | Observable behavior and acceptance criteria |
| 3 | Accepted `ARCH-0001`–`ARCH-0005` and ADRs | Ownership、dependencies and technology decisions |
| 4 | `IMPLEMENTATION-CONTRACTS-001` v2.1 | Shared information、identity、lifecycle and failure contracts |
| 5 | `PROJECT-STRUCTURE-001` | Current projects、toolchain and dependency direction |
| 6 | `PRD-TRACEABILITY-MATRIX-001` v2.1 | Current implementation status and ordered corrections |
| 7 | Code、tests and runtime evidence | Implementation evidence only |
| 8 | Historical Research and reviews | Non-normative background |

## 3. Superseded Implementation Assumptions

The following are invalid as current product behavior:

- in-app Start Capture as the primary entry;
- single-monitor-only Selection;
- cross-monitor Selection as a non-goal;
- mouse release directly triggering crop／Clipboard;
- Annotation as an optional post-capture branch;
- Clipboard publication immediately after region Selection;
- PNG file output as deferred;
- Annotation tools as unspecified future work;
- returning to or foregrounding the SnipPlus main window after every session;
- hiding MainWindow to the System Tray when `X` is pressed;
- treating irregular-layout gaps as opaque or unspecified output;
- rolling back or deleting a PNG after later Clipboard failure.

Existing code implementing these assumptions is `Incorrect` or `Obsolete` under the conformance matrix.

## 4. Reusable Technical Assets

Subject to the current contracts, these foundations may be reused:

- Windows.Graphics.Capture one-display acquisition;
- immutable frozen-frame ownership and same-frame crop;
- clear-inside／dim-outside mask behavior;
- single-display coordinate and crop utilities;
- BGRA8 premultiplied SoftwareBitmap representation;
- Win2D／WinUI image presentation;
- PNG encoding;
- WinRT Clipboard delivery with bounded cancellable retry;
- shared state authority and typed failure infrastructure;
- deterministic Contracts、Core and Windows tests.

Reuse does not authorize their current workflow placement. Multi-display、revision、Editing、transparency and commitment contracts still apply.

## 5. Completed Conformance Audit Result

The static audit concluded:

- current source is a tested single-display technical prototype;
- resident lifecycle and PrintScreen takeover are missing;
- exact direct-exit／takeover-release behavior is not implemented;
- Frozen Virtual Desktop and per-display ownership are missing;
- cross-monitor Selection and transparent gap output are missing;
- mouse release still commits output and is incorrect;
- locked-selection adjustment is missing;
- the current shared-state graph is obsolete;
- Editing、function bar and required Annotation tools are missing;
- Save As、Downloads default and PNG delivery coordination are missing;
- retained-file partial outcome is missing;
- recoverable Editing preservation、stale-revision protection and focus restoration are missing.

The matrix is the authority for row-level status.

## 6. Mandatory Correction Order

1. Resident lifecycle、direct application exit and user-controlled PrintScreen takeover setting.
2. PrintScreen entry integrated with `COMP-001`.
3. Frozen Virtual Desktop session and per-display frame ownership.
4. All-display presentation、crosshair and cross-monitor initial Selection.
5. Locked Selection、move、edge／corner resize and reselection.
6. Accepted workflow state graph including `SelectionLocked` and `Editing`.
7. Function bar、Complete／Save／Cancel and focus restoration.
8. Annotation document、required tools and object editing.
9. Annotation Undo／Redo、Virtual Desktop anchoring and Selection clipping.
10. Complete final render、transparent gap pixels and Clipboard.
11. Windows Save As、Downloads default、PNG、same-result Clipboard and retained-file partial outcome.
12. Recoverable failure preservation、stale-revision protection and accessibility.
13. Explicitly authorized multi-display runtime verification.

A later step must not begin while an earlier prerequisite remains unresolved.

## 7. First Focused Task Boundary

The first focused coding task is now product-decision-ready.

It must implement only:

- manually started resident application lifecycle;
- user-controlled PrintScreen takeover setting;
- registration and release boundary;
- MainWindow `X` as direct application exit;
- no close-to-tray behavior;
- exact release of takeover during exit;
- deterministic tests for enabled、disabled and exit release behavior.

It must stop before PrintScreen starts the full capture workflow unless that next slice is explicitly authorized.

## 8. Remaining Open Product Decisions

Coding must not guess:

- quantitative performance targets;
- exact supported display-count and maximum Virtual Desktop dimensions;
- final keyboard-only Annotation acceptance scope.

The following no longer block implementation:

- System Tray／MainWindow close behavior;
- non-display gap representation;
- Save As initial Downloads folder;
- PNG retention after Clipboard failure.

## 9. Authorization State

| Activity | State |
| --- | --- |
| Canonical document alignment | Completed |
| Requirements-to-code conformance review | Completed |
| Product decisions for first coding slice | Completed |
| Focused v1 conformance coding | Allowed only through an explicit user task and in the mandatory order |
| Unrelated feature expansion | Paused |
| Restore／build／test／runtime | Only when explicitly included in the current task |
| Interactive verification | Requires explicit current-task authorization and advance disclosure |
| New readiness／closure document chain | Prohibited |

## 10. Final Decision

`Repository documentation、the static conformance audit and the product decisions required by the first coding slice are complete. The next explicit implementation task begins with resident lifecycle、direct application exit and user-controlled PrintScreen takeover.`

No claim is made that the current implementation satisfies the accepted first-release scope.