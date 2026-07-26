# Development Guide

狀態：`Accepted for first vertical slice`

## 1. Current lifecycle position

| Area | Current state |
| --- | --- |
| PRD／Specs／Architecture | Freeze Approved |
| ADR-0002 through ADR-0007 | Accepted |
| Implementation Contracts | Accepted |
| Project Structure / Toolchain | Accepted |
| Implementation Readiness | Approved |
| Source code | Not started |
| Restore/build/test/runtime | Not performed |

Start with [Implementation Readiness Review](../IMPLEMENTATION-READINESS-REVIEW.md), [Implementation Contracts](../../Architecture/IMPLEMENTATION-CONTRACTS.md) and [Project Structure](../../Architecture/PROJECT-STRUCTURE.md).

## 2. Implementation task workflow

1. Confirm the requested work is inside the approved first-slice scope.
2. Create only the approved solution/configuration/projects.
3. Restore and build the empty x64 baseline.
4. Report and correct actual compatibility findings before adding features.
5. Implement Contracts and Core behavior with tests first.
6. Implement Windows adapters behind the accepted boundaries.
7. Compose the WinUI host without moving domain/platform ownership into App.
8. Run Unit、Contract、Rendering and authorized Platform verification.
9. Update CHANGELOG and evidence/status records with actual results.
10. Stop before any explicit non-goal or Frozen-source change.

## 3. Accepted toolchain

- C# 14.
- .NET SDK 10.0.302.
- `net10.0-windows10.0.26100.0`.
- Windows 11 24H2 x64 first-slice baseline.
- Windows App SDK 2.3.1.
- Win2D 1.4.0.
- MSTest.Sdk 4.1.0 with Microsoft.Testing.Platform.
- Packaged framework-dependent WinUI 3 development model.

Exact files、project mapping and commands are defined by PROJECT-STRUCTURE-001.

## 4. Dependency rules

- Contracts depends on no source project.
- Core depends only on Contracts.
- Windows depends only on Contracts plus platform packages.
- App composes Contracts、Core and Windows.
- No circular references.
- COMP-001 remains the sole Workflow State Authority.
- Platform adapters never mutate Shared State directly.
- Clipboard and Output remain independent downstream paths.
- Concrete WGC、Win2D、Composition and DataPackage types do not leak into Core.

## 5. Test-first boundaries

Before platform integration, implement and test：

- Legal workflow transitions.
- CaptureIntent validation.
- Coordinate conversion and crop rules.
- ImageResult ownership/disposal.
- Failure/retry classification.
- Clipboard/Output independence.

Platform work uses synthetic/public fixtures. No real desktop screenshot or Clipboard payload is committed as evidence.

## 6. Build and test commands

After an explicit implementation task authorizes execution：

```powershell
dotnet restore SnipPlus.sln --locked-mode
dotnet build SnipPlus.sln -c Release -p:Platform=x64 --no-restore
dotnet test SnipPlus.sln -c Release -p:Platform=x64 --no-build -- --filter "TestCategory!=Interactive&TestCategory!=Manual"
dotnet format SnipPlus.sln --verify-no-changes --no-restore
```

Interactive Capture/Clipboard verification requires an explicit Windows desktop execution scope.

## 7. Stop and report

Stop before continuing when：

- Pinned dependencies do not restore/build together.
- WGC、Win2D or DataPackage behavior contradicts an Accepted boundary.
- Coordinate/crop or alpha/pixel output is not deterministic.
- A dependency cycle appears necessary.
- Private content would be persisted.
- Frozen behavior/ownership must change.
- The task enters a first-slice non-goal.

Use a targeted corrective ADR/contract update only when supported by a concrete finding. Do not create another prerequisite/readiness/closure chain.

## 8. Documentation during implementation

No additional pre-coding planning is required.

Normal implementation changes should update：

- Source and tests.
- CHANGELOG.
- Actual implementation/build/runtime evidence.
- Known limitations discovered by verification.

README、Roadmap、ADR or contracts change only when the implementation reveals a real incompatibility or the user changes scope.
