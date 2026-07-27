# Architecture Baseline Review

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `ARCH-BASELINE-REVIEW-001` |
| Version | `1.0` |
| Review date | `2026-07-27` |
| Architecture stability | `Accepted` |
| Normative sources | Accepted PRD v1.1、`SPEC-0003`–`SPEC-0010`、`ARCH-0001`–`ARCH-0005`、Accepted ADRs、`IMPLEMENTATION-CONTRACTS-001` |
| Decision | `Architecture baseline accepted; implementation conformance correction required` |

This review supersedes the earlier draft architecture review that treated Annotation as optional infrastructure、kept Clipboard and Output as unconditional parallel paths、reported no source code or runtime evidence and did not include resident PrintScreen or Frozen Virtual Desktop responsibilities.

## 2. Reviewed Architecture

| Artifact | Responsibility | Result |
| --- | --- | --- |
| `ARCH-0001` | Architecture principles and invariants | `PASS` |
| `ARCH-0002` | Layer responsibilities and dependency direction | `PASS` |
| `ARCH-0003` | Required v1 module catalog | `PASS` |
| `ARCH-0004` | `COMP-001`–`COMP-018` ownership and boundaries | `PASS` |
| `ARCH-0005` | Legal component sequences and prohibited interactions | `PASS` |
| `IMPLEMENTATION-CONTRACTS-001` v2.0 | Shared session、revision、output and cleanup contracts | `PASS` |
| ADR-0002 through ADR-0007 | UI、rendering、capture、image、Clipboard and test technologies | `PASS` |
| PROJECT-STRUCTURE-001 | Current solution and toolchain mapping | `PASS` |

## 3. Architecture Coverage

The accepted baseline covers:

- manually started resident lifecycle;
- user-controlled PrintScreen takeover and release;
- pre-capture foreground-context recording;
- all-display topology and Frozen Virtual Desktop snapshot;
- immutable per-display frame ownership;
- cross-monitor selection、lock、move、resize and reselection;
- mandatory Editing／confirmation and function-bar command semantics;
- required Annotation document、objects、styles and Undo／Redo;
- final render for one Session／Selection／Annotation revision;
- Complete-to-Clipboard commitment;
- Save As、PNG and same-result Clipboard commitment;
- recoverable failure preservation;
- terminal cleanup、stale-outcome rejection and focus restoration;
- privacy、external-GUI and evidence boundaries.

## 4. Consistency Review

| Rule | Result |
| --- | --- |
| `COMP-001` is the sole shared-state authority. | `PASS` |
| Mouse release cannot invoke Clipboard or file output. | `PASS` |
| Editing is required while annotation actions are optional. | `PASS` |
| Annotation geometry stays in Frozen Virtual Desktop coordinates. | `PASS` |
| Clipboard and PNG Output are separate capabilities coordinated by Save. | `PASS` |
| Save succeeds only after PNG and Clipboard succeed. | `PASS` |
| Recoverable output failure returns to Editing with state preserved. | `PASS` |
| Platform adapters return outcomes and do not own product completion. | `PASS` |
| Stale session／revision results cannot advance state. | `PASS` |
| Successful、cancelled and terminal sessions clean up and restore work context. | `PASS` |

No accepted architecture document retains the old single-monitor、optional-editor or immediate-Clipboard workflow.

## 5. Technology Review

The existing Accepted technologies remain compatible with the corrected product scope:

- WinUI 3 for the application host and capture UI.
- WinUI XAML／Composition plus Win2D for presentation and rendering.
- Windows.Graphics.Capture for per-display frame acquisition.
- BGRA8 premultiplied SoftwareBitmap for canonical image results.
- WinRT DataPackage for Clipboard publication.
- MSTest.Sdk with Microsoft.Testing.Platform.

The architecture correction does not require replacing these ADRs. Multi-display orchestration、resident entry、editing and output sequencing require new implementation behind the existing technology boundaries.

## 6. Implementation Conformance Result

`FAIL — current code does not yet conform to the accepted architecture.`

Reusable foundations:

- one-display WGC acquisition;
- one-frame ownership and same-frame crop;
- one-display mask presentation;
- one-display coordinate conversion;
- canonical image、crop、PNG encoder and Clipboard retry;
- shared state authority and deterministic low-level tests.

Blocking architecture gaps:

- resident lifecycle and PrintScreen boundary absent;
- no Frozen Virtual Desktop or per-display session owner;
- no cross-monitor selection model;
- obsolete mouse-release-to-output interaction;
- obsolete state graph without `SelectionLocked` or `Editing`;
- no Editing or Annotation components;
- no Save As／PNG delivery coordination;
- no selection／annotation／output revision model;
- no focus restoration.

Detailed row-level status is owned by [PRD-TRACEABILITY-MATRIX-001](../PRD/PRD-TRACEABILITY-MATRIX.md).

## 7. Required Implementation Order

1. Resident lifecycle and takeover setting.
2. PrintScreen entry through `COMP-001`.
3. Frozen Virtual Desktop and per-display frames.
4. Cross-monitor presentation and initial selection.
5. Locked-selection adjustment.
6. Accepted state graph.
7. Function bar、commitments and focus restoration.
8. Annotation document and required tools.
9. History、anchoring and clipping.
10. Complete final render and Clipboard.
11. Save As、PNG and same-result Clipboard.
12. Failure preservation、stale revisions and accessibility.
13. Authorized multi-display verification.

## 8. Open Decisions

Implementation must stop before choosing:

- non-display-gap representation;
- exact System Tray and MainWindow close behavior;
- PNG retention after later Clipboard failure;
- final keyboard-only annotation standard;
- quantitative performance targets.

## 9. Final Decision

`Architecture baseline accepted for SnipPlus v1.`

Coding may proceed only through an explicit task that follows the conformance correction order. No additional Architecture readiness or closure document is required. Each implementation slice updates code、tests、CHANGELOG and the existing conformance matrix.
