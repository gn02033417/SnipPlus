# ADR-0007 Testing Strategy

## Document Control

| Field | Value |
| --- | --- |
| Document ID | `ADR-0007` |
| Title | Testing Strategy |
| Status | `Accepted` |
| Decision category | Testing |
| Version | `1.1` |
| Owner | Repository owner |
| Date accepted | `2026-07-26` |
| Last reviewed | `2026-07-27` |
| Supersedes | None |
| Superseded by | None |
| Normative references | Accepted PRD／Specs、Architecture baseline、ADR-0002 through ADR-0006 |

## Context

SnipPlus requires verification across:

- platform-neutral workflow and session rules;
- Virtual Desktop and cross-display coordinate behavior;
- selection lock、move、resize and reselection;
- Annotation document、object editing and Undo／Redo;
- deterministic raster、effects、crop and PNG output;
- Windows capture、PrintScreen、display、focus and Clipboard integration;
- Complete and Save commitment sequencing;
- cancellation、recoverable failure preservation、cleanup and stale-outcome rejection.

A single undifferentiated suite would either force Windows APIs into unit tests or leave platform behavior unverified.

## Options considered

### MSTest.Sdk with Microsoft.Testing.Platform

Microsoft-supported framework and project SDK aligned with .NET and Visual Studio. Supports one consistent runner and category filtering.

### xUnit.net

Mature and capable, but offers no product-specific advantage requiring a second framework choice.

### NUnit

Mature and capable, but similarly adds an unnecessary framework variation.

### Mixed frameworks or VSTest／MTP configurations

Rejected because mixed execution models complicate filtering、CI and reproducibility.

## Decision

Use **MSTest.Sdk 4.1.0 with Microsoft.Testing.Platform** for all SnipPlus test projects.

1. Keep one testing framework and one platform across the solution.
2. Separate tests by responsibility rather than by arbitrary end-to-end coupling.
3. Default non-interactive runs exclude `Interactive` and `Manual` categories.
4. Windows platform behavior is categorized explicitly and is not inferred from pure unit tests.
5. Interactive runtime verification requires explicit authorization in the current task.
6. Tests use synthetic or public fixtures and do not persist private desktop screenshots or Clipboard payloads.
7. Build or test success does not automatically mark a user-visible requirement `Conforms`; applicable runtime evidence is required.

## Test project responsibilities

### `SnipPlus.Contracts.Tests`

Verify:

- required IDs、defaults and validation;
- immutable session／display／selection／annotation／delivery contracts;
- image metadata and ownership;
- failure and outcome invariants;
- disposal and lease behavior.

### `SnipPlus.Core.Tests`

Verify:

- accepted shared-state graph;
- resident-entry and takeover policy through platform-neutral abstractions;
- one active capture-session rule;
- Session／Selection／Annotation／Result revision identity;
- Frozen Virtual Desktop topology and cross-display intersection rules;
- initial selection、lock、move、resize and reselection;
- Annotation object operations、numbering and Undo／Redo;
- selection adjustment excluded from Annotation history;
- Complete and Save commitment sequencing;
- recoverable failure returning to Editing;
- cancellation、stale outcome rejection and idempotent cleanup.

### `SnipPlus.Windows.Tests`

Verify:

- BGRA8 conversion、crop、composition and output pixels;
- Win2D geometry、text、Mosaic／Blur and Highlighter rendering;
- PNG encoding and file-delivery adapter behavior;
- Clipboard publication、Flush、privacy defaults and bounded retry;
- WGC support、per-display frame acquisition and cleanup;
- platform input、display／DPI and foreground-context outcomes.

## Categories

Use categories consistently where applicable:

- `Unit`
- `Contract`
- `Rendering`
- `Capture`
- `Clipboard`
- `Output`
- `Annotation`
- `Cancellation`
- `Platform`
- `Interactive`
- `Manual`

`Platform` does not automatically mean interactive. A test is marked `Interactive` when it needs a real desktop session、visible windows、global input、Clipboard interaction with the actual OS or focus restoration.

## Required v1 verification coverage

Before accepted v1 product conformance can be claimed, evidence must cover:

1. Manual startup and resident lifecycle.
2. PrintScreen takeover enabled、disabled and released on exit.
3. All connected displays frozen before Selection.
4. Negative-origin、mixed-DPI and cross-monitor Selection.
5. Clear interior、dimmed exterior and crosshair presentation.
6. Mouse release producing `SelectionLocked` without output.
7. Selection movement、edge／corner resize and reselection.
8. Mandatory function bar and zero-Annotation Complete path.
9. Required Annotation tools、object editing and history.
10. Annotation anchoring and selection clipping.
11. Complete final render and Clipboard only.
12. Save As、PNG and same-result Clipboard.
13. Save-dialog cancellation and recoverable failure preservation.
14. Cancel、terminal cleanup and foreground-context restoration.
15. Stale session／revision outcomes rejected.
16. Accessible names、state and non-color-only indicators.

## Execution model

Authorized default commands are defined by `PROJECT-STRUCTURE-001`. The ordinary non-interactive test command excludes `Interactive` and `Manual`.

Interactive verification must:

- state which windows or applications will appear and why;
- use the smallest necessary scope;
- clean up temporary windows、processes、Clipboard content and artifacts;
- avoid committing real user content;
- record limitations honestly.

Normal product startup、build and non-interactive tests must not launch Paint、Notepad or another external GUI fixture.

## Current implementation state

- Contracts、Core and Windows test projects exist.
- Low-level state、coordinate、image、crop、PNG、Clipboard retry and one-display WGC tests exist.
- Several workflow tests encode the obsolete mouse-release-to-Clipboard state graph and must be superseded rather than treated as v1 evidence.
- Resident、PrintScreen、multi-display、Editing、Annotation、Save and focus-restoration coverage is missing.

The testing technology decision remains conforming; coverage is incomplete.

## Reconsideration conditions

Revisit only if MSTest.Sdk／Microsoft.Testing.Platform cannot support required filtering、Windows App SDK initialization、CI execution or deterministic test isolation without a materially different test platform.
