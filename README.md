# SnipPlus

SnipPlus is a Windows desktop screenshot product. The accepted v1 baseline requires resident PrintScreen capture、capacity-aware all-display Frozen Virtual Desktop Selection、mandatory Editing／confirmation、first-release Annotation tools、complete keyboard-only Editing from `SelectionLocked`、Clipboard completion and PNG Save.

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
→ validate supported display envelope
→ PrintScreen freezes all connected displays
→ Present one masked Frozen Virtual Desktop
→ User creates a cross-monitor rectangular Selection
→ Mouse release locks the Selection
→ Show Editing／confirmation function bar
→ User may adjust the Selection and optionally annotate by pointer or keyboard
→ Complete OR Save OR Cancel
```

- **Complete** renders the current Selection and annotations、uses transparent pixels for physical display gaps、writes Clipboard and ends only after Clipboard succeeds.
- **Save** opens Windows Save As in Downloads by default、proposes `SnipPlus_yyyy-MM-dd_HHmmss.png`、writes PNG、publishes the same result to Clipboard and ends only after both succeed.
- If Clipboard fails after PNG creation, the PNG remains at the selected destination and the workflow returns to Editing.
- **Cancel** creates no output、closes capture UI and restores the pre-capture work context.
- MainWindow `X` directly exits SnipPlus、releases PrintScreen takeover and does not hide to the System Tray.

## Accepted Quality Baseline

### Performance

- PrintScreen accepted → interactive all-display Selection: p95 `≤ 500 ms` Standard、`≤ 1,000 ms` Maximum.
- Selection／Annotation frame time p95 `≤ 33 ms`; discrete input response p95 `≤ 100 ms`.
- Complete p95 tiers: `≤ 1.5 s`、`4 s`、`8 s`.
- Save p95 tiers after Save As confirmation: `≤ 2 s`、`6 s`、`12 s`.
- A commit still running after `300 ms` shows non-blocking progress.
- Idle private working set `≤ 250 MB`; maximum-envelope peak `≤ 2.0 GB`; cleanup and repeated-session limits are defined in PRD-0006.

### Supported capacity

- `1`–`4` active logical desktop display surfaces.
- Each display `≤ 7,680 × 4,320` physical pixels.
- Total source pixels `≤ 66,355,200`.
- Virtual Desktop width and height each `≤ 16,384`.
- Final Selection width and height each `≤ 16,384`; area `≤ 67,108,864` pixels.
- Unsupported configurations fail before Selection without partial capture.

### Keyboard-only Editing

- Scope begins at `SelectionLocked`; initial crosshair Selection remains pointer-driven in v1.
- Every required tool and object operation、style、Undo／Redo、Save、Complete and Cancel works without pointer input.
- F6／Tab navigation、tool shortcuts、keyboard object creation、`1`／`10` pixel movement／resize、Chinese IME、High Contrast、200% scaling、Narrator state and no keyboard trap are required.

## Current Status

| Area | Status |
| --- | --- |
| Product and quality requirements | Accepted complete v1 baseline |
| Behavioral specifications | Accepted complete v1 baseline |
| Architecture and ADRs | Accepted current baseline |
| Implementation contracts | Accepted v2.2 |
| Conformance matrix | Reviewed v2.2 |
| Solution and projects | Present |
| Technical capture prototype | Implemented and historically verified |
| Accepted v1 workflow conformance | Correction required |
| Product decisions blocking coding | None |
| First focused coding slice | Requires explicit user authorization |
| Release status | Not released |

## Current Implementation Order

1. Resident lifecycle、MainWindow direct exit and takeover setting.
2. PrintScreen entry through `COMP-001`.
3. Capacity policy、Frozen Virtual Desktop session and per-display frame ownership.
4. Cross-monitor presentation and initial Selection.
5. Locked-Selection move、resize and reselection.
6. Accepted workflow state graph.
7. Function bar、Complete／Save／Cancel、progress and focus restoration.
8. Annotation document、required tools、keyboard focus model and object editing.
9. Annotation Undo／Redo、Virtual Desktop anchoring、clipping and keyboard acceptance.
10. Complete final render、capacity validation、transparent gaps and Clipboard.
11. Save As、Downloads default、PNG、same-result Clipboard and retained-file outcome.
12. Failure preservation、performance／memory evidence and accessibility.
13. Explicitly authorized Standard and Maximum multi-display runtime verification.

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

Do not create more prerequisite、authorization、readiness or closure chains. Product-visible changes update existing PRD／Specs first. Implementation work updates code、tests、CHANGELOG and the existing conformance matrix. A new ADR is required only for a durable technology or Architecture decision that cannot be handled by the accepted baseline.