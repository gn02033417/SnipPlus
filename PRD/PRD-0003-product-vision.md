# PRD-0003 Product Vision

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `PRD-0003` |
| Version | `1.1` |
| Status | `Accepted` |
| Product authority | Repository owner through explicit product decisions |
| Last reviewed | `2026-07-27` |
| Scope | SnipPlus v1 product vision |

## 2. Product Statement

SnipPlus is a Windows desktop screenshot product that preserves familiar PrintScreen behavior while adding a controlled post-selection editing and delivery workflow.

The product reduces friction between “I need this part of the screen” and “I can paste or save the result,” without forcing the user to learn a different screenshot language or enter a heavyweight editor.

## 3. Product Goals

- Let a user manually start SnipPlus and keep it available in the background.
- Let the user explicitly choose whether SnipPlus takes over PrintScreen.
- Freeze and present all connected displays as one coherent Virtual Desktop capture session.
- Allow one rectangular selection to span multiple displays.
- Preserve a clear、familiar drag-selection experience.
- Keep the selected region adjustable until explicit commitment.
- Always offer a lightweight editing／confirmation stage while allowing zero Annotation actions.
- Provide a practical first-release Annotation tool set for communication and privacy.
- Deliver the final image to Clipboard through Complete.
- Deliver the same final image to PNG and Clipboard through Save.
- Protect the user’s previous foreground work context and keep success silent.
- Keep screen content local and under explicit user control.

## 4. Target Users

- General Windows users who regularly copy screenshots into chat、email or documents.
- Software engineers who capture UI、errors、logs and implementation behavior for communication.
- Technical writers who need annotated screenshots for documentation.
- Customer support and QA users who need clear issue evidence.
- Business users who need to hide sensitive details、mark steps or save a local PNG.

The first release does not require separate persona-specific workflows.

## 5. First-release Product Scope

### In scope

- Windows desktop application.
- Manual startup and background residency.
- User-controlled PrintScreen takeover.
- Static image screenshot workflow.
- All-display frozen capture session.
- One cross-monitor rectangular selection.
- Semi-transparent exterior mask and clear selected interior.
- Selection lock、move、edge／corner resize and reselection.
- Mandatory editing／confirmation function bar.
- Rectangle、Arrow／Line、Highlighter、Text、Mosaic／Blur and Numbered Marker.
- Annotation object selection、movement、resize、restyle、delete and Undo／Redo.
- Complete to Clipboard.
- Windows Save As to PNG and Clipboard.
- Cancel、recoverable failure preservation、cleanup and foreground-context restoration.

### Explicitly deferred

- Opaque freehand pen.
- Ellipse Annotation.
- Pin image to desktop.
- OCR.
- Capture history.
- Delayed capture.
- Additional image formats.
- Font-family selection、italic、underline and text background.
- Video capture.
- Cloud sync、cloud storage、sharing、team collaboration and AI features.
- Cross-platform product strategy.
- Telemetry、plugins、updates and release distribution infrastructure.

Deferred does not mean permanently rejected. It means the capability must not be included in v1 without a later explicit product decision.

## 6. Product Experience

The expected common path is:

```text
User starts SnipPlus once
→ enables PrintScreen takeover
→ presses PrintScreen when needed
→ selects a region across the desktop
→ optionally adjusts or annotates
→ presses Complete
→ pastes with Ctrl+V
```

The Save path differs only at commitment:

```text
Select and optionally annotate
→ Save
→ choose PNG destination
→ PNG and Clipboard receive the same final image
```

The product should remain lightweight in perception even though the Editing stage is always available.

## 7. Success Direction

A successful v1 product demonstrates that:

- enabled PrintScreen reliably starts SnipPlus capture without requiring the main window to be foreground;
- all displays appear frozen and selection remains spatially correct across monitors and DPI contexts;
- mouse release is clearly a Selection lock rather than accidental final output;
- the function bar is discoverable and does not obstruct the selection unnecessarily;
- the user can complete without creating an Annotation;
- required Annotation tools produce predictable editable results;
- Complete places the expected image on Clipboard;
- Save produces the same image in PNG and Clipboard;
- recoverable failures do not destroy the user’s current work;
- successful and cancelled sessions return to the previous application without unnecessary notification.

Quantitative KPIs are not invented before measurement.

## 8. Product Assumptions

- Windows 11 24H2 x64 is the current implementation and verification baseline, not necessarily the final public minimum.
- WinUI、WGC、Win2D、SoftwareBitmap and WinRT Clipboard decisions are owned by Accepted ADRs and may be superseded only through verified architecture decisions.
- One interactive capture session owns capture overlays at a time.
- Screen content remains local by default.
- Existing source is a technical prototype and does not define product scope.

## 9. Open Product Decisions

The accepted vision does not yet decide:

- exact System Tray commands and MainWindow close-button behavior;
- representation of non-display gaps in irregular monitor layouts;
- PNG retention／rollback after later Clipboard failure;
- final keyboard-only Annotation acceptance standard;
- quantitative performance targets.

Implementation must not silently invent these behaviors when it reaches them.
