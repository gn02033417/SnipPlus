# Technology Decision Roadmap

## Document Control

| Field | Value |
| --- | --- |
| Document ID | TECHNOLOGY-DECISION-ROADMAP |
| Status | Accepted |
| Version | 2.0 |
| Owner | Repository owner |
| Last reviewed | 2026-07-26 |
| Implementation-critical P0 decisions | Complete |
| Implementation readiness | Approved through IMPLEMENTATION-READINESS-REVIEW-001 |

## 1. Outcome

All technology decisions required for the approved first vertical slice are Accepted.

| Decision ID | Topic | Priority | Status | Effective artifact |
| --- | --- | --- | --- | --- |
| TD-001 | UI Framework | P0 | Accepted | ADR-0002 — WinUI 3 |
| TD-002 | Rendering Technology | P0 | Accepted | ADR-0003 — WinUI XAML／Composition + Win2D |
| TD-003 | Capture Backend | P0 | Accepted | ADR-0004 — Windows.Graphics.Capture |
| TD-004 | Clipboard Integration | P0 | Accepted | ADR-0006 — WinRT DataPackage Clipboard |
| TD-005 | Image Representation | P0 | Accepted | ADR-0005 — BGRA8 premultiplied SoftwareBitmap |
| TD-006 | Plugin Architecture | P2 | Deferred | Not required for first vertical slice |
| TD-007 | Configuration | P1 | Deferred | .NET primitives sufficient initially |
| TD-008 | Logging | P1 | Deferred | Minimum diagnostic contract defined; framework selection later |
| TD-009 | Telemetry | P2 | Deferred | Not in current product scope |
| TD-010 | Packaging | P1 | Partially bounded | Packaged framework-dependent MSIX for development; release strategy deferred |
| TD-011 | Testing Strategy | P0 | Accepted | ADR-0007 — MSTest.Sdk + Microsoft.Testing.Platform |
| TD-012 | Update Strategy | P2 | Deferred | Not required before implementation |

## 2. Accepted First-Slice Stack

- C# 14 / .NET 10.
- WinUI 3.
- Windows App SDK 2.3.1.
- WinUI XAML／Microsoft.UI.Composition.
- Win2D 1.4.0.
- Windows.Graphics.Capture.
- Canonical BGRA8 premultiplied SoftwareBitmap.
- WinRT DataPackage Clipboard publication.
- MSTest.Sdk 4.1.0 with Microsoft.Testing.Platform.
- Windows 11 24H2 x64 first-slice baseline.

Detailed version、project and packaging boundaries are owned by [PROJECT-STRUCTURE-001](PROJECT-STRUCTURE.md).

## 3. Effective Engineering Sources

- [ADR index](adr/README.md)
- [Implementation Contracts](IMPLEMENTATION-CONTRACTS.md)
- [Project Structure and Toolchain Baseline](PROJECT-STRUCTURE.md)
- [Implementation Readiness Review](../docs/IMPLEMENTATION-READINESS-REVIEW.md)

## 4. Deferred Decisions

The following do not block the first vertical slice：

- Final public packaging/signing/distribution.
- Configuration framework.
- Logging framework.
- Telemetry.
- Update strategy.
- Plugin architecture.
- ARM64.
- Support below the first-slice Windows 11 24H2 baseline.

They become active only when the product/release scope requires them.

## 5. Runtime Verification Boundary

Accepted ADRs select bounded technologies; they do not claim that repository source has already restored、built or run.

Actual implementation must produce evidence for：

- Package compatibility and restore.
- WinUI host startup.
- WGC source/frame behavior.
- Coordinate/crop fidelity.
- Win2D and SoftwareBitmap conversion.
- Clipboard publication and consumer compatibility.
- Cancellation、failure、retry and cleanup.

A concrete failure may trigger a targeted ADR or contract correction. It must not restart prerequisite/closure chains.

## 6. Anti-proliferation Rule

No additional pre-coding technology roadmap、readiness、authorization or closure document is required.

New decision documents require：

- A verified implementation/runtime conflict.
- A changed product requirement.
- A changed Architecture ownership boundary.
- A materially new platform/release scope.

## 7. Next Action

`Begin the explicitly authorized first vertical slice implementation.`

The decision phase is closed for that scope. Normal next changes should be solution/project files、source code、tests、CHANGELOG and implementation evidence—not more prerequisite documentation.
