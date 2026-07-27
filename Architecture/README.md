# Architecture

狀態：`Accepted v1 baseline`

SnipPlus architecture now reflects the accepted resident PrintScreen、all-display Frozen Virtual Desktop、cross-monitor Selection、mandatory Editing／confirmation、Annotation、Clipboard and PNG Save workflow.

## Effective sources

Read in this order:

1. [ARCH-0001 Architecture Principles](ARCH-0001-architecture-principles.md)
2. [ARCH-0002 Layer Model](ARCH-0002-layer-model.md)
3. [ARCH-0003 Module Catalog](ARCH-0003-module-catalog.md)
4. [ARCH-0004 Component Boundaries](ARCH-0004-component-boundaries.md)
5. [ARCH-0005 Component Interactions](ARCH-0005-component-interactions.md)
6. [Architecture Baseline Review](ARCH-BASELINE-REVIEW.md)
7. [Implementation Contracts](IMPLEMENTATION-CONTRACTS.md)
8. [Project Structure and Toolchain](PROJECT-STRUCTURE.md)
9. [ADR index](adr/README.md)
10. [Requirements-to-Code Conformance Matrix](../PRD/PRD-TRACEABILITY-MATRIX.md)

## Accepted technology baseline

| Area | Decision |
| --- | --- |
| UI | WinUI 3 |
| Rendering | WinUI XAML／Microsoft.UI.Composition + Win2D |
| Capture | Windows.Graphics.Capture |
| Image | BGRA8 premultiplied SoftwareBitmap |
| Clipboard | WinRT DataPackage |
| Testing | MSTest.Sdk + Microsoft.Testing.Platform |
| Language/runtime | C# 14 / .NET 10 |
| Initial implementation baseline | Windows 11 24H2 x64 |

## Fixed ownership boundaries

- `COMP-001` is the sole Workflow State Authority.
- One capture session owns one Frozen Virtual Desktop topology、all per-display frames、selection revision、annotation revision and output revision identities.
- `FEAT-001` owns resident entry、display freeze and selection lifecycle.
- `FEAT-002` is a required Editing／Annotation Feature; annotation actions are optional for the user.
- `FEAT-003` owns Clipboard publication.
- `FEAT-004` owns Windows Save As and PNG file delivery.
- `FEAT-005` owns cancellation、failure preservation、cleanup、feedback and focus restoration.
- Mouse release locks Selection and never commits output.
- Complete and Save are explicit commitments.
- Clipboard and PNG Output remain separate capabilities; Save coordination requires both to succeed.
- Platform adapters return typed outcomes and do not mutate shared state or declare product completion.

## Current implementation state

| Area | State |
| --- | --- |
| Architecture documents | Accepted and aligned with v1 |
| Solution／projects | Present |
| Reusable technical foundation | One-display WGC、same-frame crop、image、PNG encoder and Clipboard retry present |
| Resident PrintScreen | Missing |
| Frozen Virtual Desktop／cross-monitor Selection | Missing |
| SelectionLocked／Editing state model | Missing／current model obsolete |
| Function bar and Annotation | Missing |
| Save As and PNG file workflow | Missing |
| Focus restoration | Missing |
| Product conformance | Correction required through the existing matrix |

## Current next action

Follow [PRD-TRACEABILITY-MATRIX-001](../PRD/PRD-TRACEABILITY-MATRIX.md) in order, beginning with resident lifecycle and user-controlled PrintScreen takeover. Do not begin with Annotation、Clipboard hardening、Packaging or unrelated scope expansion.

## Architecture change rule

Update these existing documents only when accepted product behavior、ownership or a durable technology decision changes. Normal implementation work updates source、tests、CHANGELOG and the conformance matrix. Do not create another readiness or closure-document family.
