# ADR-0007 Testing Strategy

## Document Control

| Field | Value |
| --- | --- |
| Document ID | `ADR-0007` |
| Title | Testing Strategy |
| Status | `Accepted` |
| Decision category | Testing |
| Version | `1.2` |
| Owner | Repository owner |
| Date accepted | `2026-07-26` |
| Last reviewed | `2026-07-27` |
| Supersedes | None |
| Superseded by | None |
| Normative references | Accepted PRD／Specs、Architecture baseline、ADR-0002 through ADR-0006 |

## Context

SnipPlus requires verification across:

- platform-neutral workflow and Session rules;
- four-4K capacity and over-limit failure;
- Owner Reference mixed-DPI configuration;
- Virtual Desktop and cross-display coordinates;
- pointer Selection lock、move、resize and reselection;
- pointer-driven Annotation document、object editing and Undo／Redo;
- deterministic raster、transparent gaps、effects、crop and PNG output;
- Windows capture、PrintScreen、display、focus and Clipboard integration;
- Complete and Save sequencing、progress and retained PNG behavior;
- cancellation、recoverable failure preservation、cleanup and stale-outcome rejection;
- quantitative performance and memory evidence.

Keyboard-only Annotation and non-PrintScreen tool／action shortcuts are deferred and are not v1 coverage obligations.

## Decision

Use **MSTest.Sdk 4.1.0 with Microsoft.Testing.Platform** for all SnipPlus test projects.

1. Keep one testing framework and platform across the solution.
2. Separate tests by responsibility rather than arbitrary end-to-end coupling.
3. Default non-interactive runs exclude `Interactive` and `Manual` categories.
4. Windows platform behavior is categorized explicitly and is not inferred from pure unit tests.
5. Interactive runtime verification requires explicit authorization in the current task.
6. Tests use synthetic or public fixtures and do not persist private desktop screenshots or Clipboard payloads.
7. Build or test success does not automatically mark a user-visible requirement `Conforms`; applicable evidence is required.
8. Performance acceptance uses 3 warm-ups and at least 30 measured runs per scenario, reporting p50、p95 and maximum.
9. Tests must not make a keyboard-only Annotation conformance claim for v1.

## Test Project Responsibilities

### `SnipPlus.Contracts.Tests`

Verify:

- required IDs、defaults and validation;
- immutable Session／display／Selection／Annotation／delivery contracts;
- capacity-policy constants and boundary outcomes;
- image metadata and ownership;
- failure、retained-file and outcome invariants;
- disposal and lease behavior.

### `SnipPlus.Core.Tests`

Verify:

- accepted shared-state graph;
- resident-entry and takeover policy through platform-neutral abstractions;
- one active capture-Session rule;
- Session／Selection／Annotation／Result revision identity;
- four-4K capacity and unsupported-topology routing;
- Frozen Virtual Desktop topology and cross-display intersection rules;
- pointer Selection、lock、move、resize and reselection;
- pointer Annotation operations、numbering and Undo／Redo;
- Selection adjustment excluded from Annotation history;
- Complete and Save sequencing;
- retained PNG after later Clipboard failure;
- progress-state scheduling after `300 ms`;
- recoverable failure returning to Editing;
- Esc cancellation、stale outcome rejection and idempotent cleanup.

### `SnipPlus.Windows.Tests`

Verify:

- BGRA8 conversion、crop、composition、transparent gaps and output pixels;
- Win2D geometry、text、Mosaic／Blur and Highlighter rendering;
- PNG encoding and file-delivery adapter behavior;
- Clipboard publication、Flush、privacy defaults and bounded retry;
- WGC support、per-display frame acquisition and cleanup;
- platform pointer input、display／DPI and foreground-context outcomes.

## Categories

Use categories consistently where applicable:

- `Unit`
- `Contract`
- `Rendering`
- `Capture`
- `Clipboard`
- `Output`
- `Annotation`
- `Capacity`
- `Performance`
- `Cancellation`
- `Platform`
- `Interactive`
- `Manual`

`Platform` does not automatically mean interactive. A test is `Interactive` when it needs a real desktop Session、visible windows、global input、actual Clipboard or focus restoration.

## Required v1 Verification Coverage

Before v1 conformance can be claimed, evidence covers:

1. Manual startup and resident lifecycle.
2. PrintScreen takeover enabled、disabled and released on exit.
3. Four-4K capacity boundaries and typed over-limit failure.
4. All supported displays frozen before Selection.
5. Owner Reference `2560×1440` primary、`1920×1080` lower at `150%` scaling and left `2560×1440`.
6. Negative-origin、mixed-DPI and cross-monitor Selection.
7. Clear interior、dimmed exterior and crosshair presentation.
8. Mouse release producing `SelectionLocked` without output.
9. Pointer Selection movement、edge／corner resize and reselection.
10. Mandatory function bar and zero-Annotation Complete path.
11. Required pointer-driven Annotation tools、object editing and history.
12. Annotation anchoring and Selection clipping.
13. Complete final render、transparent gaps and Clipboard only.
14. Save As、PNG and same-result Clipboard.
15. Save-dialog cancellation、retained PNG and recoverable failure preservation.
16. Esc Cancel、terminal cleanup and foreground restoration.
17. Stale Session／revision outcomes rejected.
18. Accessible names、state and non-color-only indicators.
19. Capture、pointer interaction、Complete／Save、progress and memory targets from PRD-0006.
20. No v1 claim for keyboard-only Annotation or non-PrintScreen shortcuts.

## Performance and Runtime Profiles

- **Owner Reference:** primary `2560 × 1440`、lower `1920 × 1080` at `150%` scaling、left `2560 × 1440`.
- **Standard:** up to two displays、total source pixels `≤ 16,588,800`.
- **Maximum:** up to four displays、each `≤ 3840 × 2160`.

Measurement:

- Release x64 without debugger;
- Windows 11 24H2 x64、16 GB RAM or more、D3D11-class GPU、SSD;
- 3 warm-ups and at least 30 measured runs;
- report p50、p95 and maximum;
- exclude user decision time inside Save As.

Exact latency and memory thresholds remain normative in PRD-0006.

## Execution Model

Authorized default commands are defined by `PROJECT-STRUCTURE-001`. Ordinary non-interactive tests exclude `Interactive` and `Manual`.

Interactive verification must:

- state which windows or applications will appear and why;
- use the smallest necessary scope;
- clean up temporary windows、processes、Clipboard content and artifacts;
- avoid committing real user content;
- record limitations honestly.

Normal product startup、build and non-interactive tests must not launch Paint、Notepad or another external GUI fixture.

## Current Implementation State

- Contracts、Core and Windows test projects exist.
- Low-level state、coordinate、image、crop、PNG、Clipboard retry and one-display WGC tests exist.
- Several workflow tests encode the obsolete mouse-release-to-Clipboard state graph and must be superseded.
- Resident、PrintScreen、capacity、multi-display、Editing、Annotation、Save、performance and focus-restoration coverage is missing.
- Keyboard-only Annotation coverage is intentionally deferred.

The testing technology decision remains conforming; coverage is incomplete.

## Reconsideration Conditions

Revisit only if MSTest.Sdk／Microsoft.Testing.Platform cannot support required filtering、Windows App SDK initialization、CI execution or deterministic isolation without a materially different test platform.