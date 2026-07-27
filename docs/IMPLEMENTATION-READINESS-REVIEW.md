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

The previous single-display technical slice proves reusable capture、image、crop、PNG and Clipboard foundations, but it does not represent the accepted first-release product workflow.

The accepted v1 baseline now fully defines:

- resident lifecycle、PrintScreen takeover and direct MainWindow exit;
- all-display Frozen Virtual Desktop and four-4K capacity envelope;
- the Repository owner’s three-display mixed-DPI runtime profile;
- cross-monitor Selection and transparent topology gaps;
- Selection lock、pointer move、edge／corner resize and reselection;
- mandatory Editing／confirmation and required pointer-driven Annotation tools;
- Complete、Save、retained PNG and recoverable output behavior;
- quantitative capture、interaction、output、memory and cleanup targets;
- cancellation、progress、focus restoration and baseline accessibility;
- keyboard-only Annotation and non-PrintScreen shortcuts explicitly deferred.

Therefore:

- no product decision or documentation gate remains before ordered implementation;
- coding must follow the conformance matrix order;
- existing code is reused only where classified as conforming or partial foundation;
- historical build／test evidence does not preserve obsolete behavior;
- restore、build、test and runtime execution still require explicit authorization in the current task.

## 2. Effective Canonical Sources

| Priority | Source | Responsibility |
| --- | --- | --- |
| 1 | Accepted `PRD-0002`–`PRD-0006` | Product intent、quality and v1 scope |
| 2 | Accepted `SPEC-0001`–`SPEC-0010` | Observable behavior and acceptance criteria |
| 3 | Accepted `ARCH-0001`–`ARCH-0005` and ADRs | Ownership、dependencies and technology decisions |
| 4 | `IMPLEMENTATION-CONTRACTS-001` v2.3 | Shared identity、capacity、performance、keyboard boundary、lifecycle and failure contracts |
| 5 | `PROJECT-STRUCTURE-001` | Projects、toolchain and dependency direction |
| 6 | `PRD-TRACEABILITY-MATRIX-001` | Current implementation status and correction order |
| 7 | Code、tests and runtime evidence | Implementation evidence only |
| 8 | Historical Research and reviews | Non-normative background |

## 3. Superseded Assumptions

The following are invalid as current v1 behavior:

- in-app Start Capture as the primary entry;
- single-monitor-only Selection;
- mouse release directly triggering crop／Clipboard;
- Annotation as optional post-capture work;
- PNG file output as deferred;
- MainWindow returning after every Session;
- close-to-tray behavior;
- opaque or unspecified topology gaps;
- PNG rollback after Clipboard failure;
- unlimited or silently partial display support;
- 8K displays inside the v1 support envelope;
- complete keyboard-only Annotation and non-PrintScreen tool／action shortcuts as v1 requirements.

## 4. Reusable Technical Assets

Subject to current contracts, these foundations may be reused:

- one-display Windows.Graphics.Capture acquisition;
- immutable frozen-frame ownership and same-frame crop;
- clear-inside／dim-outside mask behavior;
- single-display coordinate and crop utilities;
- BGRA8 premultiplied SoftwareBitmap representation;
- Win2D／WinUI image presentation;
- PNG encoding;
- WinRT Clipboard delivery with bounded cancellable retry;
- shared state authority and typed failure infrastructure;
- deterministic Contracts、Core and Windows tests.

Reuse does not authorize current workflow placement. Multi-display、capacity、revision、Editing、transparency、performance and commitment contracts still apply.

## 5. Current Implementation Gaps

Current source remains a tested single-display technical prototype. Missing or incorrect areas include:

- resident lifecycle、PrintScreen takeover and exact exit release;
- four-4K capacity policy and over-limit failure;
- Frozen Virtual Desktop and per-display ownership;
- cross-monitor Selection and transparent gap output;
- locked-selection adjustment and accepted state graph;
- Editing、function bar and pointer-driven Annotation tools;
- Complete／Save commitment and progress behavior;
- Downloads Save As and retained-file outcome;
- performance／memory measurement evidence;
- recoverable Editing preservation、required accessibility and focus restoration.

Keyboard-only Annotation is deferred and must not be added to the missing-v1 list.

## 6. Mandatory Correction Order

1. Resident lifecycle、direct application exit and user-controlled PrintScreen takeover setting.
2. PrintScreen entry integrated with `COMP-001`.
3. Four-4K capacity policy、Frozen Virtual Desktop Session and per-display frame ownership.
4. All-display presentation、crosshair and cross-monitor initial Selection.
5. Locked Selection、pointer move、edge／corner resize and reselection.
6. Accepted workflow state graph including `SelectionLocked` and `Editing`.
7. Function bar、Complete／Save／Cancel、progress and focus restoration.
8. Annotation document、required pointer-driven tools and object editing.
9. Annotation Undo／Redo、Virtual Desktop anchoring and Selection clipping.
10. Complete final render、capacity revalidation、transparent gaps and Clipboard.
11. Windows Save As、Downloads default、PNG、same-result Clipboard and retained-file outcome.
12. Recoverable failure preservation、stale-revision protection、performance／memory evidence and required accessibility.
13. Explicitly authorized Owner Reference、Standard and Maximum runtime verification.

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

- Capture start p95 `≤ 500 ms` Owner Reference／Standard、`≤ 1,000 ms` Maximum.
- Pointer interaction p95 frame time `≤ 33 ms`; visible response p95 `≤ 100 ms`.
- Complete p95 tiers `≤ 1.5 s`、`4 s`、`8 s`.
- Save p95 tiers `≤ 2 s`、`6 s`、`12 s` after Save As confirmation.
- Progress after `300 ms`.
- Idle `≤ 250 MB`、peak `≤ 2.0 GB`、cleanup and repeated-session limits per `PRD-0006`.
- Measurement uses 3 warm-ups and at least 30 measured runs.

### Capacity

- `1`–`4` displays.
- Each `≤ 3840 × 2160`.
- Total source pixels `≤ 33,177,600`.
- Virtual Desktop width／height each `≤ 16,384`.
- Selection width／height each `≤ 16,384`; area `≤ 67,108,864` pixels.
- Unsupported capacity fails before Selection without partial capture.
- 8K displays are outside v1.

### Owner Reference Configuration

- primary `2560 × 1440`;
- lower `1920 × 1080` at Windows scaling `150%`;
- left `2560 × 1440`.

### Keyboard Boundary

Required:

- PrintScreen entry;
- Esc cancellation;
- ordinary text entry and Chinese IME;
- accessible names and non-color-only state indicators.

Deferred:

- keyboard-only Annotation;
- F6／Tab workflow;
- tool、Ctrl、Delete and Arrow-key shortcuts;
- keyboard-created objects;
- pointer-unused Editing acceptance.

## 9. Authorization State

| Activity | State |
| --- | --- |
| Canonical document alignment | Completed |
| Requirements-to-code conformance review | Completed |
| All v1 product decisions | Completed |
| Focused v1 conformance coding | Allowed only through an explicit user task and mandatory order |
| Unrelated feature expansion | Paused |
| Restore／build／test／runtime | Only when explicitly included in the current task |
| Interactive verification | Requires explicit current-task authorization and advance disclosure |
| New readiness／closure document chain | Prohibited |

## 10. Final Decision

`Repository documentation、the conformance audit and all visible v1 product decisions are complete. The next explicit implementation task begins with resident lifecycle、direct application exit and user-controlled PrintScreen takeover.`

No claim is made that current implementation satisfies the accepted first-release scope.