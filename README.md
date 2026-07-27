# SnipPlus

SnipPlus is a Windows desktop screenshot product. The accepted v1 product baseline requires resident PrintScreen capture、all-display frozen Virtual Desktop selection、a mandatory editing／confirmation stage、first-release annotation tools、Clipboard completion and PNG Save.

The repository already contains a working single-display capture／crop／Clipboard technical prototype. That prototype is **not** the accepted v1 product workflow and is being corrected through the requirements-to-code conformance plan.

## Start here

Read these sources in order:

1. [Repository rules](AGENTS.md)
2. [Accepted core workflow](PRD/PRD-0004-core-workflow.md)
3. [Functional requirements](PRD/PRD-0005-functional-requirements.md)
4. [Non-functional requirements](PRD/PRD-0006-non-functional-requirements.md)
5. [Capture Workflow Spec](Specs/SPEC-0005-capture-workflow.md)
6. [Annotation Capability Spec](Specs/SPEC-0009-annotation-capability.md)
7. [Feature Integration Spec](Specs/SPEC-0010-feature-integration.md)
8. [Implementation Contracts](Architecture/IMPLEMENTATION-CONTRACTS.md)
9. [Requirements-to-Code Conformance Matrix](PRD/PRD-TRACEABILITY-MATRIX.md)
10. [Implementation Readiness Review](docs/IMPLEMENTATION-READINESS-REVIEW.md)

Historical Research、Analysis、Decision and earlier baseline reviews do not override these accepted sources.

## Accepted v1 workflow

```text
User manually starts SnipPlus
→ SnipPlus remains resident
→ User enables PrintScreen takeover
→ PrintScreen freezes all connected displays
→ Present one masked Frozen Virtual Desktop
→ User creates a cross-monitor rectangular selection
→ Mouse release locks the selection
→ Show editing／confirmation function bar
→ User may adjust the selection and optionally annotate
→ Complete OR Save OR Cancel
```

- **Complete** renders the current selection and annotations、writes Clipboard and ends only after Clipboard succeeds.
- **Save** opens Windows Save As、writes PNG、publishes the same result to Clipboard and ends only after both succeed.
- **Cancel** creates no output、closes capture UI and restores the pre-capture work context.

## Current status

| Area | Status |
| --- | --- |
| Product requirements | Accepted v1.1 |
| Behavioral specifications | Accepted current baseline |
| Architecture principles／layers／modules／components | Accepted current baseline |
| Technology ADRs | ADR-0002 through ADR-0007 Accepted |
| Implementation contracts | Accepted v2.0 |
| Solution and projects | Present |
| Technical capture prototype | Implemented and previously verified |
| Accepted v1 workflow conformance | Correction required |
| Current coding authorization | Only through an explicit task following the conformance correction order |
| Release status | Not released |

The current code provides reusable one-display WGC、same-frame crop、image、PNG encoding and Clipboard foundations. It does not yet provide resident PrintScreen takeover、Frozen Virtual Desktop、cross-monitor selection、locked-selection editing、required annotation tools、Save As workflow or focus restoration.

## Current implementation order

1. Resident lifecycle and takeover setting.
2. PrintScreen entry through `COMP-001`.
3. Frozen Virtual Desktop session and per-display frame ownership.
4. Cross-monitor presentation and initial selection.
5. Locked-selection move、resize and reselection.
6. Accepted workflow state graph including `SelectionLocked` and `Editing`.
7. Function bar、Complete／Save／Cancel and focus restoration.
8. Annotation document and required tools.
9. Annotation Undo／Redo、Virtual Desktop anchoring and clipping.
10. Complete final render plus Clipboard.
11. Save As、PNG plus the same Clipboard result.
12. Failure preservation、stale-revision protection and accessibility.
13. Explicitly authorized multi-display runtime verification.

Each completed implementation slice must update [PRD-TRACEABILITY-MATRIX-001](PRD/PRD-TRACEABILITY-MATRIX.md) using actual code、tests and applicable runtime evidence.

## Accepted technology baseline

- C# 14 / .NET SDK 10.0.302.
- Windows 11 24H2 x64 implementation baseline.
- Windows App SDK 2.3.1 and WinUI 3.
- Win2D 1.4.0.
- Windows.Graphics.Capture.
- BGRA8 premultiplied SoftwareBitmap.
- WinRT DataPackage Clipboard.
- MSTest.Sdk 4.1.0 with Microsoft.Testing.Platform.

## Documentation boundary

Do not create more prerequisite、authorization、readiness or closure chains. Product-visible changes update existing PRD／Specs first. Implementation work updates code、tests、CHANGELOG and the existing conformance matrix. A new ADR is required only for a durable technology or architecture decision that cannot be handled by the accepted baseline.
