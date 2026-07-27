# Development Guide

狀態：`Accepted for v1 conformance correction`

## 1. Current lifecycle position

| Area | Current state |
| --- | --- |
| PRD | Accepted v1.1 |
| Specs | Accepted current baseline |
| Architecture | Accepted current baseline |
| ADR-0002 through ADR-0007 | Accepted |
| Implementation Contracts | Accepted v2.0 |
| Solution／projects | Present |
| Technical prototype | Present and partially reusable |
| Accepted v1 conformance | Correction required |
| Current implementation authority | Only an explicit task following the conformance matrix |

Start with:

1. `AGENTS.md`
2. `PRD/PRD-TRACEABILITY-MATRIX.md`
3. the owning PRD／Spec rows;
4. `Architecture/IMPLEMENTATION-CONTRACTS.md`;
5. the smallest relevant source and test files.

Do not re-read all historical Research for ordinary implementation work.

## 2. Implementation task workflow

1. Identify the first unresolved prerequisite in the conformance correction order.
2. Read only its owning PRD、Spec、Architecture／Contract and current code／tests.
3. Classify existing code as reusable、partial、incorrect or obsolete before editing.
4. Implement the smallest complete behavioral slice.
5. Add or update Unit／Contract tests for state、identity、validation、failure and cleanup.
6. Add platform or UI verification only when the current task explicitly authorizes it.
7. Run only the restore／build／test／runtime commands authorized by the current task.
8. Update `CHANGELOG.md` with actual results.
9. Update the corresponding conformance-matrix rows only after code、tests and applicable evidence exist.
10. Stop before the next correction step or any explicit open product decision.

## 3. Required correction order

1. Resident lifecycle and takeover setting.
2. PrintScreen entry integrated with `COMP-001`.
3. Frozen Virtual Desktop and per-display frame ownership.
4. All-display presentation and cross-monitor initial selection.
5. Locked-selection move、resize and reselection.
6. Accepted workflow state graph.
7. Function bar、Complete／Save／Cancel and focus restoration.
8. Annotation document and required tools.
9. Annotation Undo／Redo、anchoring and clipping.
10. Complete final render and Clipboard.
11. Save As、PNG and the same Clipboard result.
12. Failure preservation、stale-revision protection and accessibility.
13. Explicitly authorized multi-display runtime verification.

Do not skip ahead because a later adapter or helper already exists.

## 4. Accepted toolchain

- C# 14.
- .NET SDK 10.0.302.
- `net10.0-windows10.0.26100.0`.
- Windows App SDK 2.3.1.
- Win2D 1.4.0.
- MSTest.Sdk 4.1.0 with Microsoft.Testing.Platform.
- Packaged framework-dependent WinUI 3 development model.
- Windows 11 24H2 x64 current implementation baseline.

Exact package、project and build configuration is owned by `Architecture/PROJECT-STRUCTURE.md`.

## 5. Dependency and ownership rules

- `SnipPlus.Contracts` depends on no source project.
- `SnipPlus.Core` depends only on Contracts.
- `SnipPlus.Windows` depends on Contracts and platform packages, not Core product implementation.
- `SnipPlus.App` composes Contracts、Core and Windows.
- `COMP-001` is the sole Workflow State Authority.
- Platform adapters return typed outcomes and never mutate shared state.
- Mouse release never invokes Clipboard or file output.
- Editing／confirmation is mandatory; annotation actions are optional.
- Complete and Save are explicit commitments.
- Clipboard and PNG Output remain separate capabilities coordinated by the Save workflow.
- Concrete WinUI、WGC、Win2D、DataPackage、picker and file types do not leak into Core contracts.
- No circular project or component dependencies.

## 6. Test boundaries

Prefer deterministic tests for:

- legal and illegal workflow transitions;
- Session／Selection／Annotation／Result revision identity;
- cancellation and stale-outcome rejection;
- Virtual Desktop coordinate and cross-display intersection rules;
- selection lock、move、resize and reselection;
- annotation object operations and Undo／Redo;
- clipping and final-render composition;
- Complete and Save commitment sequencing;
- recoverable failure preservation;
- idempotent cleanup;
- Clipboard retry and PNG encoding.

Use synthetic/public images. Do not persist real desktop screenshots or Clipboard payloads as evidence.

## 7. Build and test commands

Run only when explicitly authorized in the current task:

```powershell
dotnet restore SnipPlus.sln --locked-mode
dotnet build SnipPlus.sln -c Release -p:Platform=x64 --no-restore
dotnet test SnipPlus.sln -c Release -p:Platform=x64 --no-build -- --filter "TestCategory!=Interactive&TestCategory!=Manual"
dotnet format SnipPlus.sln --verify-no-changes --no-restore
```

Interactive Capture、PrintScreen、focus or multi-display verification requires explicit authorization for the current task. Before launching, state which windows or processes will appear and why.

Normal build、tests and product startup must not launch Paint、Notepad or another external GUI fixture.

## 8. Stop and report

Stop before continuing when:

- implementation reaches an unresolved product decision;
- a required technology replacement or Accepted ADR supersession appears necessary;
- display topology or mixed-DPI mapping cannot be made deterministic;
- selection、annotation or output identities can become stale or mismatched;
- recoverable output failure cannot preserve Editing state;
- a dependency cycle appears necessary;
- private screen or Clipboard content would be persisted;
- the requested work enters a deferred capability;
- interactive verification was not explicitly authorized.

Open decisions currently include non-display-gap presentation、System Tray／MainWindow close behavior、PNG retention after Clipboard failure、keyboard-only Annotation acceptance and quantitative performance targets.

## 9. Documentation during implementation

Normal changes update:

- source and tests;
- `CHANGELOG.md`;
- the existing conformance-matrix rows;
- actual limitations or runtime evidence.

Update PRD／Specs only for explicit product-visible changes. Update Architecture／ADR only for a real ownership or durable technology decision. Do not create another prerequisite、readiness、authorization or closure chain.
