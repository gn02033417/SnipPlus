# Architecture

狀態：`Implementation baseline accepted`

SnipPlus 的抽象 Architecture、implementation-critical ADR、cross-project contracts 與 Project Structure 已完成第一個 vertical slice 所需的收斂。

## Effective sources

- [Architecture Baseline Review](ARCH-BASELINE-REVIEW.md) — Layer、Module、Component、Interaction ownership。
- [ADR index](adr/README.md) — Accepted technology decisions。
- [Implementation Contracts](IMPLEMENTATION-CONTRACTS.md) — Cross-project information/lifecycle boundaries。
- [Project Structure](PROJECT-STRUCTURE.md) — Toolchain、project mapping、build/test baseline。
- [Implementation Readiness Review](../docs/IMPLEMENTATION-READINESS-REVIEW.md) — Approved implementation scope。

## Accepted technology baseline

| Area | Decision |
| --- | --- |
| UI | WinUI 3 |
| Rendering | WinUI XAML／Microsoft.UI.Composition + Win2D adapter |
| Capture | Windows.Graphics.Capture |
| Image | BGRA8 premultiplied SoftwareBitmap |
| Clipboard | WinRT DataPackage |
| Testing | MSTest.Sdk + Microsoft.Testing.Platform |
| Language/runtime | C# 14 / .NET 10 |
| Initial platform | Windows 11 24H2 x64 |

## Fixed ownership boundaries

- Product Workflow → Feature Coordination → Domain Capability → Platform Integration。
- COMP-001 是唯一 Workflow State Authority。
- COMP-001 through COMP-013 map to `SnipPlus.Core`。
- COMP-014 through COMP-018 map to `SnipPlus.Windows`。
- Cross-project semantic types map to `SnipPlus.Contracts`。
- WinUI host/composition root maps to `SnipPlus.App`。
- Clipboard and Output remain parallel downstream paths。
- Annotation remains optional。
- Platform adapters do not own product semantics。

## Current implementation state

| Area | State |
| --- | --- |
| Documentation readiness | Approved |
| Solution/projects | Not created |
| Source code | Not started |
| Restore/build/test | Not performed |
| Runtime verification | Not performed |

The absence of runtime evidence is now an implementation task, not a documentation blocker.

## Next action

Create the approved solution/projects and implement the bounded first vertical slice. Additional Architecture planning is prohibited unless an actual implementation finding requires a targeted correction or superseding decision.
