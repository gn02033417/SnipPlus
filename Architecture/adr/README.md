# Architecture Decision Records

ADR 記錄影響多個模組、難以回復，或需要長期保存取捨理由的架構與技術選擇。

## Accepted ADRs

| ADR | Topic | Effective decision |
| --- | --- | --- |
| [ADR-0001 Documentation-first baseline](ADR-0001-documentation-first.md) | Repository governance | Documentation-first governance |
| [ADR-0002 UI Framework Selection](ADR-0002-ui-framework-selection.md) | Desktop UI Framework | WinUI 3 |
| [ADR-0003 Rendering Technology](ADR-0003-rendering-technology.md) | Rendering | WinUI XAML／Composition + Win2D adapter |
| [ADR-0004 Capture Backend](ADR-0004-capture-backend.md) | Platform capture | Windows.Graphics.Capture |
| [ADR-0005 Image Representation](ADR-0005-image-representation.md) | Canonical image result | BGRA8 premultiplied SoftwareBitmap |
| [ADR-0006 Clipboard Integration](ADR-0006-clipboard-integration.md) | Windows Clipboard | WinRT DataPackage + privacy options + Flush |
| [ADR-0007 Testing Strategy](ADR-0007-testing-strategy.md) | Test framework/platform | MSTest.Sdk + Microsoft.Testing.Platform |

All P0 ADRs required by the approved first vertical slice are Accepted.

Accepted does not mean runtime verified. Actual restore、build、test and Windows behavior must be demonstrated by implementation evidence.

## Implementation Sources

- [Technology Decision Roadmap](../TECHNOLOGY-DECISION-ROADMAP.md)
- [Implementation Contracts](../IMPLEMENTATION-CONTRACTS.md)
- [Project Structure and Toolchain Baseline](../PROJECT-STRUCTURE.md)
- [Implementation Readiness Review](../../docs/IMPLEMENTATION-READINESS-REVIEW.md)

## Current Boundary

- No additional pre-coding ADR is required for the first vertical slice.
- Deferred P1/P2 decisions must not be pulled into the slice without scope change.
- A verified incompatibility may trigger a targeted corrective or superseding ADR.
- A failed implementation must not restart prerequisite／authorization／closure document chains.

## ADR Rules

Each ADR must contain：

- Status and document control.
- Context and decision drivers.
- Options considered.
- One primary decision.
- Trade-offs and consequences.
- Traceability and review record.
- Change/supersession conditions.

Only `Accepted` ADRs are effective. Core accepted decisions are not overwritten; changes use a new ADR and `Supersedes` relationship.
