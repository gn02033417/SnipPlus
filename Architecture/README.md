# Architecture

狀態：`Accepted v1 baseline`

SnipPlus architecture reflects the accepted resident PrintScreen、direct application exit、all-display Frozen Virtual Desktop、cross-monitor Selection、transparent gap output、mandatory Editing／confirmation、Annotation、Clipboard and PNG Save workflow.

## Effective Sources

Read in this order:

1. [ARCH-0001 Architecture Principles](ARCH-0001-architecture-principles.md)
2. [ARCH-0002 Layer Model](ARCH-0002-layer-model.md)
3. [ARCH-0003 Module Catalog](ARCH-0003-module-catalog.md)
4. [ARCH-0004 Component Boundaries](ARCH-0004-component-boundaries.md)
5. [ARCH-0005 Component Interactions](ARCH-0005-component-interactions.md)
6. [Architecture Baseline Review](ARCH-BASELINE-REVIEW.md)
7. [Implementation Contracts v2.1](IMPLEMENTATION-CONTRACTS.md)
8. [Project Structure and Toolchain](PROJECT-STRUCTURE.md)
9. [ADR index](adr/README.md)
10. [Requirements-to-Code Conformance Matrix](../PRD/PRD-TRACEABILITY-MATRIX.md)

## Accepted Technology Baseline

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

## Fixed Ownership Boundaries

- `COMP-001` is the sole Workflow State Authority.
- One capture Session owns one Frozen Virtual Desktop topology、all per-display frames、Selection revision、Annotation revision and output revision identities.
- `FEAT-001` owns resident entry、direct application exit、PrintScreen takeover boundary、display freeze and Selection lifecycle.
- MainWindow `X` exits SnipPlus and releases takeover; it does not hide to tray.
- `FEAT-002` is a required Editing／Annotation Feature; Annotation actions are optional for the user.
- `FEAT-003` owns Clipboard publication.
- `FEAT-004` owns Windows Save As、Downloads initial folder、PNG file delivery and retained File Reference.
- `FEAT-005` owns cancellation、failure preservation、cleanup、feedback and focus restoration.
- Mouse release locks Selection and never commits output.
- Complete and Save are explicit commitments.
- Physical non-display gaps become transparent pixels in the canonical final image.
- Clipboard and PNG Output remain separate capabilities; Save coordination requires both to succeed before overall completion.
- A PNG that has been created successfully remains user output after later Clipboard failure.
- Platform adapters return typed outcomes and do not mutate shared state or declare product completion.

## Current Implementation State

| Area | State |
| --- | --- |
| Architecture documents | Accepted and aligned with v1.2 |
| Solution／projects | Present |
| Reusable technical foundation | One-display WGC、same-frame crop、image、PNG encoder and Clipboard retry present |
| Resident PrintScreen／direct exit cleanup | Missing／partial close foundation |
| Frozen Virtual Desktop／cross-monitor Selection | Missing |
| Transparent gap composition | Missing |
| SelectionLocked／Editing state model | Missing／current model obsolete |
| Function bar and Annotation | Missing |
| Save As、Downloads default and retained-file workflow | Missing |
| Focus restoration | Missing |
| Product conformance | Correction required through the existing matrix |

## Current Next Action

Follow [PRD-TRACEABILITY-MATRIX-001](../PRD/PRD-TRACEABILITY-MATRIX.md) in order, beginning with resident lifecycle、MainWindow direct exit and user-controlled PrintScreen takeover. Do not begin with Annotation、Clipboard hardening、Packaging or unrelated scope expansion.

## Architecture Change Rule

Update these existing documents only when accepted product behavior、ownership or a durable technology decision changes. Normal implementation work updates source、tests、CHANGELOG and the conformance matrix. Do not create another readiness or closure-document family.