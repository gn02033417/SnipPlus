# SnipPlus

SnipPlus is a Windows desktop screenshot product. The accepted v1 baseline requires resident PrintScreen capture、capacity-aware all-display Frozen Virtual Desktop Selection、mandatory Editing／confirmation、pointer-driven Annotation、Clipboard completion and PNG Save.

The repository contains a working single-display capture／crop／Clipboard technical prototype. That prototype is **not** the accepted v1 product workflow and is being corrected through the requirements-to-code conformance plan.

## Start Here

Read these sources in order:

1. [Repository rules](AGENTS.md)
2. [Accepted core workflow](PRD/PRD-0004-core-workflow.md)
3. [Functional requirements](PRD/PRD-0005-functional-requirements.md)
4. [Non-functional and quality requirements](PRD/PRD-0006-non-functional-requirements.md)
5. [Capture Workflow Spec](Specs/SPEC-0005-capture-workflow.md)
6. [Annotation Capability Spec](Specs/SPEC-0009-annotation-capability.md)
7. [Feature Integration Spec](Specs/SPEC-0010-feature-integration.md)
8. [Implementation Contracts](Architecture/IMPLEMENTATION-CONTRACTS.md)
9. [Requirements-to-Code Conformance Matrix](PRD/PRD-TRACEABILITY-MATRIX.md)
10. [Implementation Readiness Review](docs/IMPLEMENTATION-READINESS-REVIEW.md)

Historical Research、Analysis、Decision and earlier reviews do not override these accepted sources.

## Accepted v1 Workflow

```text
User manually starts SnipPlus
→ SnipPlus remains resident while running
→ User enables PrintScreen takeover
→ validate the four-4K support envelope
→ PrintScreen freezes all connected supported displays
→ present one masked Frozen Virtual Desktop
→ user creates a cross-monitor rectangular Selection
→ mouse release locks the Selection
→ show Editing／confirmation function bar
→ user may adjust the Selection and optionally annotate with pointer input
→ Complete OR Save OR Cancel
```

- **Complete** renders the current Selection and annotations、uses transparent pixels for physical display gaps、writes Clipboard and ends only after Clipboard succeeds.
- **Save** opens Windows Save As in Downloads by default、proposes `SnipPlus_yyyy-MM-dd_HHmmss.png`、writes PNG、publishes the same result to Clipboard and ends only after both succeed.
- If Clipboard fails after PNG creation, the PNG remains at the selected destination and the workflow returns to Editing.
- **Cancel** creates no output、closes capture UI and restores the pre-capture work context.
- MainWindow `X` directly exits SnipPlus、releases PrintScreen takeover and does not hide to the System Tray.

## Accepted Quality Baseline

### Performance

- PrintScreen accepted → interactive all-display Selection: p95 `≤ 500 ms` Owner Reference／Standard、`≤ 1,000 ms` Maximum.
- Pointer-driven Selection／Annotation frame time p95 `≤ 33 ms`; visible response p95 `≤ 100 ms`.
- Complete p95 tiers: `≤ 1.5 s`、`4 s`、`8 s`.
- Save p95 tiers after Save As confirmation: `≤ 2 s`、`6 s`、`12 s`.
- A commit still running after `300 ms` shows non-blocking progress.
- Idle private working set `≤ 250 MB`; maximum-envelope peak `≤ 2.0 GB`; cleanup and repeated-session limits are defined in PRD-0006.
- Measurement uses `3` warm-up runs and at least `30` measured runs with p50、p95 and maximum reporting.

### Supported Capacity

- `1`–`4` active logical desktop display surfaces.
- Each display `≤ 3840 × 2160` physical pixels.
- Total source pixels `≤ 33,177,600`.
- Virtual Desktop width and height each `≤ 16,384`.
- Final Selection width and height each `≤ 16,384`; area `≤ 67,108,864` pixels.
- Unsupported configurations fail before Selection without partial capture.
- An 8K display is outside v1.

### Owner Reference Configuration

Mandatory real-world mixed-DPI verification includes:

- primary `2560 × 1440`;
- lower `1920 × 1080` at Windows scaling `150%`;
- left `2560 × 1440`.

### Keyboard Boundary

Required:

- PrintScreen capture entry;
- Esc cancellation;
- ordinary text editing and Chinese IME;
- accessible names and non-color-only selected／error state.

Deferred:

- complete keyboard-only Annotation;
- F6／Tab zone and object workflow;
- tool、Ctrl、Delete and Arrow-key shortcuts;
- keyboard-created Annotation objects;
- pointer-unused acceptance after `SelectionLocked`.

## Current Status

| Area | Status |
| --- | --- |
| Product and quality requirements | Accepted complete v1 baseline |
| Behavioral specifications | Accepted complete v1 baseline |
| Architecture and ADRs | Accepted current baseline |
| Implementation contracts | Accepted v2.3 |
| Conformance matrix | Reviewed — implementation correction required |
| Solution and projects | Present |
| Technical capture prototype | Implemented and historically verified |
| Accepted v1 workflow conformance | Correction required |
| Product decisions blocking coding | None |
| First focused coding slice | Requires explicit user authorization |
| Release status | Not released |

## Current Implementation Order

1. Resident lifecycle、MainWindow direct exit and takeover setting.
2. PrintScreen entry through `COMP-001`.
3. Four-4K capacity policy、Frozen Virtual Desktop Session and per-display frame ownership.
4. Cross-monitor presentation and initial Selection.
5. Locked-Selection pointer move、resize and reselection.
6. Accepted workflow state graph.
7. Function bar、Complete／Save／Cancel、progress and focus restoration.
8. Annotation document、required pointer-driven tools and object editing.
9. Annotation Undo／Redo、Virtual Desktop anchoring and clipping.
10. Complete final render、capacity validation、transparent gaps and Clipboard.
11. Save As、Downloads default、PNG、same-result Clipboard and retained-file outcome.
12. Failure preservation、performance／memory evidence and required accessibility.
13. Explicitly authorized Owner Reference、Standard and Maximum runtime verification.

## Accepted Technology Baseline

- C# 14 / .NET SDK 10.0.302.
- Windows 11 24H2 x64 implementation baseline.
- Windows App SDK 2.3.1 and WinUI 3.
- Win2D 1.4.0.
- Windows.Graphics.Capture.
- BGRA8 premultiplied SoftwareBitmap.
- WinRT DataPackage Clipboard.
- MSTest.Sdk 4.1.0 with Microsoft.Testing.Platform.

## Documentation Boundary

Do not create more prerequisite、authorization、readiness or closure chains. Product-visible changes update existing PRD／Specs first. Implementation work updates code、tests、CHANGELOG and the existing conformance matrix.