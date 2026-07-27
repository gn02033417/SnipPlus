# Technology Decision Roadmap

## Document Control

| Field | Value |
| --- | --- |
| Document ID | `TECHNOLOGY-DECISION-ROADMAP` |
| Status | `Accepted` |
| Version | `2.1` |
| Owner | Repository owner |
| Last reviewed | `2026-07-27` |
| Implementation-critical decisions | Complete for the current v1 correction |

## 1. Current Outcome

The Accepted technology decisions remain valid for the corrected SnipPlus v1 workflow. The product-scope expansion from the old single-display technical slice to resident PrintScreen、Frozen Virtual Desktop、Editing／Annotation and PNG Save does not currently require replacing the chosen UI、capture、rendering、image、Clipboard or test technologies.

| Decision ID | Topic | Priority | Status | Effective artifact |
| --- | --- | --- | --- | --- |
| TD-001 | UI Framework | P0 | Accepted | ADR-0002 — WinUI 3 |
| TD-002 | Rendering Technology | P0 | Accepted | ADR-0003 — WinUI XAML／Composition + Win2D |
| TD-003 | Capture Backend | P0 | Accepted | ADR-0004 — Windows.Graphics.Capture |
| TD-004 | Clipboard Integration | P0 | Accepted | ADR-0006 — WinRT DataPackage Clipboard |
| TD-005 | Image Representation | P0 | Accepted | ADR-0005 — BGRA8 premultiplied SoftwareBitmap |
| TD-006 | Plugin Architecture | P2 | Deferred | Not in v1 scope |
| TD-007 | Configuration | P1 | Deferred | Add only from demonstrated needs |
| TD-008 | Logging | P1 | Deferred | Select from actual operational failures |
| TD-009 | Telemetry | P2 | Deferred | Not in v1 scope |
| TD-010 | Packaging | P1 | Partially bounded | Development MSIX accepted; release strategy deferred |
| TD-011 | Testing Strategy | P0 | Accepted | ADR-0007 — MSTest.Sdk + Microsoft.Testing.Platform |
| TD-012 | Update Strategy | P2 | Deferred | Not in v1 scope |

## 2. Accepted Stack

- C# 14 / .NET 10.
- WinUI 3 and Windows App SDK 2.3.1.
- WinUI XAML／Microsoft.UI.Composition.
- Win2D 1.4.0.
- Windows.Graphics.Capture.
- Canonical BGRA8 premultiplied SoftwareBitmap.
- WinRT DataPackage Clipboard.
- MSTest.Sdk 4.1.0 with Microsoft.Testing.Platform.
- Windows 11 24H2 x64 current implementation baseline.

Detailed project and build boundaries are owned by `PROJECT-STRUCTURE-001`.

## 3. Technology Work Required by v1 Correction

These are implementation tasks inside Accepted technologies, not new technology decisions:

- resident process and PrintScreen interception boundary;
- display enumeration、Virtual Desktop topology and mixed-DPI context;
- per-display WGC acquisition and session ownership;
- multi-display overlay／selection composition;
- annotation object rendering and effects through accepted rendering technology;
- Windows Save As and PNG file delivery;
- foreground-context recording and restoration;
- stale-session／revision protection and additional tests.

A targeted ADR is required only if a verified implementation finding shows that an Accepted technology cannot satisfy an accepted boundary.

## 4. Existing Runtime Evidence

Existing evidence confirms only technical foundations:

- package restore and Release x64 build;
- one-display WGC frame acquisition;
- one frozen frame and same-frame crop;
- BGRA8 image、crop、PNG encoding and Win2D presentation;
- Clipboard publication、retry and privacy defaults;
- synthetic single-display packaged workflow.

It does not verify resident PrintScreen、all-display freeze、cross-monitor selection、Editing／Annotation、Save As or focus restoration.

## 5. Deferred Decisions

Do not activate until the product or release scope requires them:

- final packaging、signing、distribution and update strategy;
- configuration framework;
- logging framework;
- telemetry;
- plugins;
- ARM64;
- broader Windows support;
- cloud、sharing and external services;
- HDR preservation and additional output formats.

## 6. Runtime Verification Boundary

The v1 correction must eventually produce explicitly authorized evidence for:

- PrintScreen takeover enable／disable and process-exit release;
- display enumeration and all-display freeze;
- negative-origin and mixed-DPI Virtual Desktop behavior;
- cross-monitor selection、lock、move、resize and reselection;
- Editing function bar and required Annotation tools;
- Complete and Save commitment sequencing;
- focus restoration and main-window exclusion;
- recoverable failure preservation and stale-outcome rejection.

Real desktop pixels and Clipboard payloads are not persisted as repository evidence.

## 7. Anti-proliferation Rule

Do not create additional technology readiness、authorization or closure documents for ordinary implementation gaps. Use:

- existing ADRs for accepted decisions;
- one targeted ADR for a verified durable conflict;
- source and tests for implementation;
- `CHANGELOG.md` and the existing conformance matrix for evidence.

## 8. Next Action

Begin only the explicitly authorized first step in `PRD-TRACEABILITY-MATRIX-001`: resident lifecycle and user-controlled PrintScreen takeover. Technology selection is not the current blocker.
