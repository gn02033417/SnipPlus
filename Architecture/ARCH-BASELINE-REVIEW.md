# Architecture Baseline Review

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `ARCH-BASELINE-REVIEW-001` |
| Version | `1.1` |
| Review date | `2026-07-27` |
| Architecture stability | `Accepted` |
| Normative sources | Accepted current PRD、`SPEC-0003`–`SPEC-0010`、`ARCH-0001`–`ARCH-0005`、Accepted ADRs、`IMPLEMENTATION-CONTRACTS-001` v2.3 |
| Decision | `Architecture baseline accepted; implementation conformance correction required` |

This review supersedes earlier architecture wording that left close behavior、gap output、PNG retention、performance、capacity or keyboard scope unresolved.

## 2. Reviewed Architecture

| Artifact | Responsibility | Result |
| --- | --- | --- |
| `ARCH-0001` | Architecture principles and invariants | `PASS` |
| `ARCH-0002` | Layer responsibilities and dependency direction | `PASS` |
| `ARCH-0003` | Required v1 module catalog | `PASS` |
| `ARCH-0004` | Component ownership and boundaries | `PASS` |
| `ARCH-0005` | Legal component sequences and prohibited interactions | `PASS` |
| `IMPLEMENTATION-CONTRACTS-001` v2.3 | Session、capacity、output、performance、keyboard boundary and cleanup contracts | `PASS` |
| ADR-0002 through ADR-0007 | UI、rendering、capture、image、Clipboard and test technologies | `PASS` |
| `PROJECT-STRUCTURE-001` | Current solution and toolchain mapping | `PASS` |

## 3. Architecture Coverage

The accepted baseline covers:

- manually started resident lifecycle;
- user-controlled PrintScreen takeover and exact release;
- MainWindow direct exit with no close-to-tray behavior;
- pre-capture foreground-context recording;
- four-4K capacity validation;
- Owner Reference three-display mixed-DPI profile;
- all-display topology and Frozen Virtual Desktop snapshot;
- immutable per-display frame ownership;
- cross-monitor Selection、lock、pointer move、resize and reselection;
- mandatory Editing／confirmation and function-bar commands;
- pointer-driven Annotation document、objects、styles and Undo／Redo;
- final render for one Session／Selection／Annotation revision;
- transparent non-display gaps;
- Complete-to-Clipboard commitment;
- Save As、PNG and same-result Clipboard commitment;
- retained PNG after later Clipboard failure;
- recoverable failure preservation;
- terminal cleanup、stale-outcome rejection and focus restoration;
- quantitative performance、memory and progress gates;
- PrintScreen／Esc keyboard boundary and deferred keyboard-only Annotation;
- privacy、external-GUI and evidence boundaries.

## 4. Consistency Review

| Rule | Result |
| --- | --- |
| `COMP-001` is the sole shared-state authority. | `PASS` |
| Capacity validation prohibits partial display capture. | `PASS` |
| Mouse release cannot invoke Clipboard or file output. | `PASS` |
| Editing is required while Annotation actions are optional. | `PASS` |
| V1 Selection and Annotation manipulation are pointer-driven. | `PASS` |
| Annotation geometry stays in Frozen Virtual Desktop coordinates. | `PASS` |
| Clipboard and PNG Output are separate capabilities coordinated by Save. | `PASS` |
| Save succeeds only after PNG and Clipboard succeed. | `PASS` |
| PNG remains retained after later Clipboard failure. | `PASS` |
| Recoverable output failure returns to Editing with state preserved. | `PASS` |
| Platform adapters return outcomes and do not own product completion. | `PASS` |
| Stale Session／revision results cannot advance state. | `PASS` |
| Successful、cancelled and terminal Sessions clean up and restore work context. | `PASS` |
| Keyboard-only Annotation and non-PrintScreen tool／action shortcuts are deferred. | `PASS` |

No accepted Architecture document may retain the old single-monitor、immediate-Clipboard、8K-capable or complete keyboard-only v1 workflow.

## 5. Technology Review

The accepted technologies remain compatible with the current product scope:

- WinUI 3 for application host and capture UI;
- WinUI XAML／Composition plus Win2D for presentation and rendering;
- Windows.Graphics.Capture for per-display frame acquisition;
- BGRA8 premultiplied SoftwareBitmap for canonical image results;
- WinRT DataPackage for Clipboard publication;
- MSTest.Sdk with Microsoft.Testing.Platform.

The current product correction does not require replacing these ADRs. It requires implementation behind the accepted boundaries.

## 6. Quality and Capacity Contracts

### Capacity

- `1`–`4` active logical display surfaces;
- each display `≤ 3840 × 2160`;
- total source pixels `≤ 33,177,600`;
- Virtual Desktop width／height each `≤ 16,384`;
- final Selection width／height each `≤ 16,384`;
- final Selection area `≤ 67,108,864` pixels;
- 8K displays outside v1.

### Performance

- 3 warm-ups and at least 30 measured runs;
- p50、p95 and maximum reporting;
- capture start p95 `≤ 500 ms` Owner Reference／Standard、`≤ 1,000 ms` Maximum;
- pointer interaction p95 frame time `≤ 33 ms` and response `≤ 100 ms`;
- accepted Complete／Save tiers、progress and memory limits from PRD-0006.

### Owner Reference Runtime Profile

- primary `2560 × 1440`;
- lower `1920 × 1080` at Windows scaling `150%`;
- left `2560 × 1440`.

## 7. Implementation Conformance Result

`FAIL — current code does not yet conform to the accepted Architecture.`

Reusable foundations:

- one-display WGC acquisition;
- one-frame ownership and same-frame crop;
- one-display mask presentation;
- one-display coordinate conversion;
- canonical image、crop、PNG encoder and Clipboard retry;
- shared state authority and deterministic low-level tests.

Blocking Architecture gaps:

- resident lifecycle and PrintScreen boundary;
- four-4K capacity policy;
- Frozen Virtual Desktop and per-display Session owner;
- cross-monitor Selection model;
- obsolete mouse-release-to-output interaction;
- obsolete state graph without `SelectionLocked` or `Editing`;
- pointer-driven Editing and Annotation components;
- Save As／PNG delivery coordination;
- Selection／Annotation／output revision model;
- performance／memory evidence;
- focus restoration.

Keyboard-only Annotation is deferred and is not a v1 Architecture gap.

Detailed row-level status is owned by [PRD-TRACEABILITY-MATRIX-001](../PRD/PRD-TRACEABILITY-MATRIX.md).

## 8. Required Implementation Order

1. Resident lifecycle、direct exit and takeover setting.
2. PrintScreen entry through `COMP-001`.
3. Four-4K capacity、Frozen Virtual Desktop and per-display frames.
4. Cross-monitor presentation and initial Selection.
5. Pointer locked-Selection adjustment.
6. Accepted state graph.
7. Function bar、commitments、progress and focus restoration.
8. Pointer-driven Annotation document and required tools.
9. History、anchoring and clipping.
10. Complete final render、transparent gaps and Clipboard.
11. Save As、PNG and same-result Clipboard.
12. Failure preservation、stale revisions、performance／memory and baseline accessibility.
13. Authorized Owner Reference、Standard and Maximum verification.

## 9. Product Decision Status

No visible v1 product or quality decision remains open. The four-4K envelope、performance protocol and deferred keyboard scope may not be silently changed based on implementation difficulty.

## 10. Final Decision

`Architecture baseline accepted for SnipPlus v1.`

Coding may proceed only through an explicit task following the correction order. No additional Architecture readiness or closure document is required.