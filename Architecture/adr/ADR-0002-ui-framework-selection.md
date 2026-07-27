# ADR-0002 UI Framework Selection

## Document Control

| Field | Value |
| --- | --- |
| Document ID | `ADR-0002` |
| Title | UI Framework Selection |
| Status | `Accepted` |
| Decision category | Framework |
| Version | `1.1` |
| Owner | Repository owner |
| Date accepted | `2026-07-26` |
| Last reviewed | `2026-07-27` |
| Supersedes | None |
| Superseded by | None |
| Normative references | Accepted PRD／Specs、Architecture baseline、ADR Baseline |

## Context

SnipPlus is a Windows-first desktop screenshot product requiring:

- a manually started resident application;
- a settings surface for PrintScreen takeover;
- full-screen／per-display capture overlays;
- cross-monitor Selection interaction;
- a mandatory Editing／confirmation function bar;
- Annotation controls and accessible interaction state;
- Windows Save As integration;
- composition with Windows capture、rendering、Clipboard and focus adapters.

The UI framework must support these responsibilities without owning product workflow semantics.

## Options considered

### WinUI 3

Windows App SDK desktop UI framework with Fluent-aligned controls、modern Windows lifecycle support、XAML composition and integration with Windows Runtime APIs.

### WPF

Mature Windows desktop framework with broad ecosystem support, but not selected because the repository chose the current Windows App SDK／WinUI direction and associated rendering／platform integration stack.

### Avalonia or other cross-platform frameworks

Not selected because cross-platform portability is not a v1 product driver and would add platform abstraction and deployment work without an accepted requirement.

## Decision

Use **WinUI 3 through Windows App SDK** as the SnipPlus application UI framework.

WinUI 3 owns:

- main-window and resident-application presentation;
- settings UI;
- capture overlay windows and composition surfaces;
- function-bar controls;
- Annotation interaction controls and accessible UI state;
- translation of pointer／keyboard events into platform-neutral intents;
- composition-root wiring of Core and Windows adapters.

WinUI 3 does not own:

- shared workflow state;
- Session、Selection or Annotation product rules;
- capture backend semantics;
- final completion decisions;
- Clipboard or PNG delivery semantics;
- platform-neutral failure classification.

`COMP-001` remains the sole shared Workflow State Authority. WinUI event handlers request actions through accepted Core boundaries.

## Application boundary

- `SnipPlus.App` is the composition root.
- UI code-behind remains thin and delegates to testable Core behavior.
- Concrete WinUI types do not leak into `SnipPlus.Core` or `SnipPlus.Contracts`.
- Capture overlays may use one window per display or another WinUI composition strategy as long as user-visible Virtual Desktop behavior conforms to Specs.
- Exact System Tray commands and MainWindow close-button behavior remain an open product decision; this ADR does not choose them.

## Consequences

### Benefits

- Native fit with Windows 11 and Fluent interaction language.
- Direct integration with Windows App SDK、WinRT and accepted rendering technology.
- Appropriate controls、focus、accessibility and window composition for the v1 workflow.
- Avoids adding a second desktop framework.

### Costs and risks

- Multi-window／multi-display overlay behavior requires explicit runtime verification.
- Resident lifecycle、PrintScreen interception and focus restoration still require platform integration beyond XAML controls.
- WinUI dispatcher／apartment requirements must be respected by Clipboard and UI-bound adapters.

## Current implementation state

- Packaged WinUI 3 application and main window exist.
- Current UI implements only the historical one-display Start Capture prototype.
- Resident lifecycle、PrintScreen setting、multi-display overlays、SelectionLocked editing、function bar、Annotation and Save As UI remain to be implemented.

The framework decision is conforming; the current product UI is not yet v1-conforming.

## Reconsideration conditions

Revisit only if verified implementation evidence shows WinUI 3 cannot satisfy an accepted v1 window、input、accessibility or deployment boundary without a materially different framework.
