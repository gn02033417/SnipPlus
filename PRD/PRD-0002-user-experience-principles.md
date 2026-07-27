# PRD-0002 User Experience Principles

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `PRD-0002` |
| Version | `1.1` |
| Status | `Accepted` |
| Product authority | Repository owner through explicit product decisions |
| Last reviewed | `2026-07-27` |
| Scope | SnipPlus v1 user-experience principles |

## 2. Purpose

This document defines the product-level UX principles that constrain Core Workflow、Functional Requirements、Specifications、Architecture and implementation. It does not choose implementation APIs or exact visual styling.

## 3. UX Principles

### Principle 1 — Preserve Windows screenshot muscle memory

The primary v1 entry is PrintScreen when the user has enabled SnipPlus takeover. Initial selection uses a crosshair、a dimmed exterior and a clear selected interior. The user should recognize the workflow without learning a new screenshot language.

### Principle 2 — Show the exact source being selected

Selection occurs on a frozen representation of all connected displays. The user must see the source content being selected, and the final result must come from the same frozen capture session rather than a later desktop frame.

### Principle 3 — Keep basic capture direct, but require explicit commitment

The common path is:

```text
PrintScreen
→ select
→ function bar
→ Complete
→ Ctrl+V
```

Mouse release locks the selection but is not final confirmation. Explicit Complete or Save prevents accidental output while adding only one clear commitment step.

### Principle 4 — Editing is always available; annotation actions are optional

The editing／confirmation stage is a required part of the v1 workflow. The user may ignore every Annotation tool and choose Complete immediately. “Optional Annotation” means zero annotation actions are allowed; it does not mean the confirmation stage is bypassed.

### Principle 5 — Multi-display behavior must feel like one desktop

All connected displays participate in one logical Frozen Virtual Desktop. A single rectangular selection can cross display boundaries. Negative origins、mixed DPI and monitor arrangement must not create visibly incorrect selection or output.

### Principle 6 — Selection remains controllable until commitment

After mouse release, the user can move the selection、resize from edges or corners、or drag elsewhere to replace it. Selection adjustment must not unexpectedly scale or move existing Annotation objects.

### Principle 7 — Common tools stay discoverable in one working context

The v1 function bar exposes ordinary tools without deep menu nesting:

- Selection;
- Rectangle;
- Arrow／Line;
- Highlighter;
- Text;
- Mosaic／Blur;
- Numbered Marker;
- Undo／Redo;
- color and applicable size／thickness;
- Save、Cancel and Complete.

Exact icons、layout and visual styling remain design choices as long as required controls are readily discoverable.

### Principle 8 — Protect the user’s existing work context

SnipPlus normal windows must not appear in the frozen source. Complete、successful Save and Cancel close capture UI and return the user to the application active before PrintScreen. The main SnipPlus window does not automatically foreground after the session.

### Principle 9 — Success is quiet; failure is actionable

Successful completion produces no success Toast or Dialog. Recoverable render、save or Clipboard failure keeps the editing session open、preserves the user’s current work and clearly offers retry or Cancel.

### Principle 10 — Keep screen content local and user-directed

Capture occurs only after explicit user action. Screen pixels、Annotation state、saved PNG and Clipboard payload stay local unless a future explicit product decision adds external transfer. Normal operation and evidence collection must not expose private content unnecessarily.

### Principle 11 — Responsive behavior is more important than feature count

Capture start、all-display presentation、selection adjustment and Annotation interaction must remain visibly responsive. Unsupported latency numbers must not be invented; quantitative targets follow measurement.

### Principle 12 — Deferred capabilities must not burden v1

OCR、history、pinning、delayed capture、cloud、sharing、plugins、additional image formats and other deferred features must not appear as prerequisites or unexplained inactive UI in the first-release workflow.

## 4. Relationship to optional capabilities

A capability is “optional” when the user can complete the common screenshot task without using it. This principle applies to individual Annotation actions and future advanced capabilities.

The following are still mandatory product boundaries:

- PrintScreen capture when takeover is enabled;
- all-display Frozen Virtual Desktop;
- explicit Selection lock;
- editing／confirmation function bar;
- Complete、Save and Cancel actions;
- Clipboard commitment and failure preservation.

## 5. Deferred design details

This document does not fix:

- exact visual colors、icons、animation or spacing;
- exact System Tray menu and MainWindow close-button behavior;
- representation of non-display gaps in irregular monitor layouts;
- final keyboard-only Annotation interaction standard;
- quantitative performance targets.

Those details must remain explicit until decided; implementation may not silently choose user-visible behavior that conflicts with the principles above.
