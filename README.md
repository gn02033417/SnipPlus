# SnipPlus

SnipPlus is a Windows desktop capture product repository. Product、Specification、Architecture、technology decisions、implementation contracts and Project Structure are complete for the first vertical slice. Application implementation has not started.

## Start here

- [Implementation Readiness Review](docs/IMPLEMENTATION-READINESS-REVIEW.md) — `Approved for first vertical slice implementation`
- [Implementation Contracts](Architecture/IMPLEMENTATION-CONTRACTS.md)
- [Project Structure and Toolchain Baseline](Architecture/PROJECT-STRUCTURE.md)
- [ADR index](Architecture/adr/README.md)
- [Frozen PRD baseline](PRD/PRD-FREEZE-REVIEW.md)
- [Frozen Specification baseline](Specs/SPEC-BASELINE-REVIEW.md)
- [Frozen Architecture baseline](Architecture/ARCH-BASELINE-REVIEW.md)
- [Development Guide](docs/guides/development-guide.md)
- [Documentation index](docs/index.md)

## Current status

| Area | Status |
| --- | --- |
| Product requirements | PRD v1.0 `Freeze Approved` |
| Behavioral specifications | Specification v1.0 `Freeze Approved` |
| Architecture | Abstract baseline `Freeze Approved` |
| UI framework | ADR-0002 `Accepted`; WinUI 3 |
| Rendering | ADR-0003 `Accepted`; WinUI XAML／Composition + Win2D |
| Capture | ADR-0004 `Accepted`; Windows.Graphics.Capture |
| Image representation | ADR-0005 `Accepted`; BGRA8 premultiplied SoftwareBitmap |
| Clipboard | ADR-0006 `Accepted`; WinRT DataPackage |
| Testing | ADR-0007 `Accepted`; MSTest.Sdk + MTP |
| Contracts | `Accepted` |
| Toolchain / Project Structure | `Accepted` |
| Implementation readiness | **Approved for first vertical slice** |
| Application code | Not started |
| Build/runtime evidence | Not performed; required implementation output |

## Accepted first-slice baseline

- C# 14 / .NET SDK 10.0.302.
- Windows 11 24H2 x64.
- Windows App SDK 2.3.1.
- Win2D 1.4.0.
- MSTest.Sdk 4.1.0 and Microsoft.Testing.Platform.
- Packaged framework-dependent WinUI 3 development model.
- Contracts、Core、Windows and App source projects plus three test projects.

## Next action

**Issue an explicit first vertical slice implementation task.**

No additional pre-coding paperwork is required. The implementation task should create the approved solution/projects, restore/build the empty baseline, implement the bounded capture-to-Clipboard path, add tests and record actual evidence.

The approved scope excludes global hotkeys、multi-monitor stitching、window-capture product mode、annotation tools、file-output UI、HDR preservation、telemetry、cloud、OCR、plugins and release publication.

## Documentation boundary

Research files remain historical evidence. Do not create more prerequisite、authorization-request or closure-review chains. Documentation changes during implementation require a concrete scope change、official compatibility issue or verified build/runtime finding.
