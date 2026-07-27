# SnipPlus Implementation Readiness Review

## Document Control

| Field | Value |
| --- | --- |
| Document ID | `IMPLEMENTATION-READINESS-REVIEW-001` |
| Status | `Accepted — ready for ordered v1 conformance correction` |
| Original review date | `2026-07-26` |
| Product baseline revision | `2026-07-27` |
| Conformance audit completed | `2026-07-27` |
| All v1 product decisions completed | `2026-07-27` |
| Authority | Repository owner through explicit product decisions |
| Review scope | SnipPlus v1 first release |

## 1. Executive Decision

The previous single-display technical slice proved reusable capture、image、crop、PNG and Clipboard foundations, but it does not represent the accepted first-release product workflow.

The accepted v1 workflow now includes fully defined:

- resident lifecycle、PrintScreen takeover and direct MainWindow exit;
- all-display Frozen Virtual Desktop and supported capacity envelope;
- cross-monitor Selection and transparent topology gaps;
- Selection lock、move、edge／corner resize and reselection;
- mandatory Editing／confirmation and required Annotation tools;
- complete keyboard-only Editing／Annotation acceptance from `SelectionLocked`;
- Complete、Save、retained PNG and recoverable output behavior;
- quantitative capture、interaction、output、memory and cleanup targets;
- cancellation、progress、focus restoration and accessibility behavior.

`PRD/PRD-TRACEABILITY-MATRIX.md` records the static comparison between accepted requirements and current code／tests／runtime evidence.

Therefore:

- no product decision or documentation gate remains before ordered implementation;
- coding must follow the matrix order and begin with the first unresolved prerequisite;
- existing code is reused only where the matrix classifies it as conforming or partial foundation;
- passing historical tests does not preserve obsolete product behavior;
- restore、build、test and runtime execution still require explicit authorization in the current task.

## 2. Effective Canonical Sources

| Priority | Source | Responsibility |
| --- | --- | --- |
| 1 | Accepted `PRD-0002`–`PRD-0006` | Product intent、quality and v1 scope |
| 2 | Accepted `SPEC-0001`–`SPEC-0010` | Observable behavior and acceptance criteria |
| 3 | Accepted `ARCH-0001`–`ARCH-0005` and ADRs | Ownership、dependencies and technology decisions |
| 4 | `IMPLEMENTATION-CONTRACTS-001` v2.2 | Shared identity、capacity、keyboard、performance、lifecycle and failure contracts |
| 5 | `PROJECT-STRUCTURE-001` | Current projects、toolchain and dependency direction |
| 6 | `PRD-TRACEABILITY-MATRIX-001` v2.2 | Current implementation status and ordered corrections |
| 7 | Code、tests and runtime evidence | Implementation evidence only |
| 8 | Historical Research and reviews | Non-normative background |

## 3. Superseded Implementation Assumptions

The following are invalid as current product behavior:

- in-app Start Capture as the primary entry;
- single-monitor-only Selection;
- mouse release directly triggering crop／Clipboard;
- Annotation as optional post-capture work;
- PNG file output as deferred;
- returning to or foregrounding MainWindow after every session;
- hiding MainWindow to tray when `X` is pressed;
- opaque or unspecified topology gaps;
- PNG rollback after later Clipboard failure;
- unlimited or silently partial display support;
- undefined performance、memory or keyboard-accessibility acceptance;
- treating pointer-driven Annotation tests as sufficient keyboard evidence.

Existing code implementing these assumptions is `Incorrect` or `Obsolete` under the conformance matrix.

## 4. Reusable Technical Assets

Subject to current contracts, these foundations may be reused:

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

Reuse does not authorize current workflow placement. Multi-display、capacity、revision、Editing、keyboard、transparency、performance and commitment contracts still apply.

## 5. Completed Conformance Audit Result

Current source remains a tested single-display technical prototype. Missing or incorrect areas include:

- resident lifecycle、PrintScreen takeover and exact exit release;
- capacity policy and over-limit failure;
- Frozen Virtual Desktop and per-display ownership;
- cross-monitor Selection and transparent gap output;
- locked-selection adjustment and accepted state graph;
- Editing、function bar、Annotation tools and keyboard focus model;
- Complete／Save commitment and progress behavior;
- Downloads Save As and retained-file outcome;
- performance／memory measurement evidence;
- recoverable Editing preservation、accessibility and focus restoration.

The matrix is the authority for row-level status.

## 6. Mandatory Correction Order

1. Resident lifecycle、direct application exit and user-controlled PrintScreen takeover setting.
2. PrintScreen entry integrated with `COMP-001`.
3. Capacity policy、Frozen Virtual Desktop session and per-display frame ownership.
4. All-display presentation、crosshair and cross-monitor initial Selection.
5. Locked Selection、move、edge／corner resize and reselection.
6. Accepted workflow state graph including `SelectionLocked` and `Editing`.
7. Function bar、Complete／Save／Cancel、progress and focus restoration.
8. Annotation document、required tools、keyboard focus model and object editing.
9. Annotation Undo／Redo、Virtual Desktop anchoring、Selection clipping and keyboard-only acceptance.
10. Complete final render、capacity revalidation、transparent gaps and Clipboard.
11. Windows Save As、Downloads default、PNG、same-result Clipboard and retained-file partial outcome.
12. Recoverable failure preservation、stale-revision protection、performance／memory evidence and accessibility.
13. Explicitly authorized Standard and Maximum multi-display runtime verification.

A later step must not begin while an earlier prerequisite remains unresolved.

## 7. First Focused Task Boundary

The first focused coding task must implement only:

- manually started resident application lifecycle;
- user-controlled PrintScreen takeover setting;
- registration and release boundary;
- MainWindow `X` as direct application exit;
- no close-to-tray behavior;
- exact release of takeover during exit;
- deterministic tests for enabled、disabled and exit release behavior.

It must stop before PrintScreen starts the full capture workflow unless the next slice is explicitly authorized.

## 8. Finalized Quality Baseline

### Performance

- Capture start p95 `≤ 500 ms` Standard、`≤ 1,000 ms` Maximum.
- Interaction p95 frame time `≤ 33 ms`; input response p95 `≤ 100 ms`.
- Complete tiers p95 `≤ 1.5 s`、`4 s`、`8 s`.
- Save tiers p95 `≤ 2 s`、`6 s`、`12 s` after Save As confirmation.
- Progress after `300 ms`.
- Idle `≤ 250 MB`、peak `≤ 2.0 GB`、cleanup and repeated-session limits per `PRD-0006`.

### Capacity

- `1`–`4` displays.
- Each `≤ 7,680 × 4,320`.
- Total source pixels `≤ 66,355,200`.
- Virtual Desktop width／height each `≤ 16,384`.
- Selection area `≤ 67,108,864` pixels with dimension caps.
- Unsupported capacity fails before Selection without partial capture.

### Keyboard-only Editing

- Scope begins at `SelectionLocked`.
- F6 zone navigation、Tab object／handle traversal、tool shortcuts、`1`／`10` pixel move／resize、keyboard object creation、IME、High Contrast、200% scaling、Narrator state and no keyboard trap are mandatory.
- Initial crosshair Selection remains pointer-driven in v1.

## 9. Authorization State

| Activity | State |
| --- | --- |
| Canonical document alignment | Completed |
| Requirements-to-code conformance review | Completed |
| All v1 product decisions | Completed |
| Focused v1 conformance coding | Allowed only through an explicit user task and in mandatory order |
| Unrelated feature expansion | Paused |
| Restore／build／test／runtime | Only when explicitly included in the current task |
| Interactive verification | Requires explicit current-task authorization and advance disclosure |
| New readiness／closure document chain | Prohibited |

## 10. Final Decision

`Repository documentation、the conformance audit and all visible v1 product decisions are complete. The next explicit implementation task begins with resident lifecycle、direct application exit and user-controlled PrintScreen takeover.`

No claim is made that current implementation satisfies the accepted first-release scope.