# UI Wireframe

狀態：`Accepted behavioral wireframe`

This low-fidelity wireframe records the accepted v1 screen responsibilities and interaction order. Exact visual styling、spacing、icons and animation remain implementation design choices unless constrained by PRD／Specs.

## 1. Resident main-window responsibility

The user manually starts SnipPlus. The main window provides at least:

```text
+--------------------------------------------------+
| SnipPlus                                         |
|--------------------------------------------------|
| PrintScreen takeover                             |
| [ Enabled / Disabled ]                           |
|                                                  |
| Current status: Resident / Capturing / Error     |
|                                                  |
| [Start Capture - secondary/diagnostic entry]     |
|                                                  |
| Settings not fixed by v1:                        |
| - exact System Tray menu                         |
| - exact close-button behavior                    |
+--------------------------------------------------+
```

The in-app Start Capture command may invoke the same workflow for diagnostics but is not the primary v1 entry.

## 2. Initial capture presentation

After enabled PrintScreen:

```text
All connected displays
┌──────────────────────────────┐ ┌──────────────────────────────┐
│ Frozen display content       │ │ Frozen display content       │
│ covered by translucent mask  │ │ covered by translucent mask  │
│                              │ │                              │
│              +               │ │                              │
│        crosshair pointer     │ │                              │
└──────────────────────────────┘ └──────────────────────────────┘
```

Requirements:

- one frozen frame exists for every connected display before interaction;
- all displays participate in one logical Virtual Desktop;
- the pointer is a crosshair throughout the selectable area;
- SnipPlus normal windows are excluded from frozen content;
- Esc cancels the complete capture session.

## 3. Dragging a cross-monitor selection

```text
Display A                         Display B
┌──────────────────────────────┐ ┌──────────────────────────────┐
│ dimmed                       │ │ dimmed                       │
│          ┌───────────────────┼─┼──────────────┐               │
│ dimmed   │ clear frozen      │ │ clear frozen │      dimmed   │
│          │ selected content  │ │ content      │               │
│          └───────────────────┼─┼──────────────┘               │
│ dimmed                       │ │ dimmed                       │
└──────────────────────────────┘ └──────────────────────────────┘
```

- One rectangular selection may cross display boundaries.
- The selected interior shows original frozen content without the mask.
- The exterior remains dimmed.
- Mouse release locks the selection and does not create output.

## 4. Locked selection and function bar

After a valid mouse release:

```text
┌───────────────────────────────────────────────┐
│                                               │
│     ┌─────────────────────────────────┐       │
│     │         locked selection        │       │
│     │   annotations are drawn here    │       │
│     └─────────────────────────────────┘       │
│       ◉ handles on corners and edges          │
│                                               │
│     [Selection] [Rectangle] [Arrow/Line]      │
│     [Highlighter] [Text] [Mosaic/Blur]        │
│     [Number] [Undo] [Redo] [Color] [Size]     │
│     [Save] [Cancel] [Complete]                │
└───────────────────────────────────────────────┘
```

Function-bar placement:

- Prefer below the selection.
- Move above when there is insufficient space.
- Remain inside an available display work area.

Before commitment the user can:

- drag inside the selection to move it;
- drag four edges or four corners to resize;
- drag elsewhere to replace it;
- create or edit annotations;
- perform no annotation action and choose Complete;
- choose Save or Cancel.

## 5. Annotation controls

Required v1 tools:

| Tool | Required behavior |
| --- | --- |
| Selection | Select、move、resize or delete applicable annotation objects; adjust current capture region. |
| Rectangle | Outline with color and line thickness. |
| Arrow／Line | End arrow or no-arrow straight-line mode. |
| Highlighter | Semi-transparent freehand stroke with rounded ends. |
| Text | Microsoft JhengHei by default; content edit、move、color、font size and bold. |
| Mosaic／Blur | One rectangular privacy tool with mode switch stored per object. |
| Numbered Marker | Solid circular marker、automatic numbering、configurable next number、color and size. |
| Undo／Redo | Annotation changes only; selection geometry changes are excluded. |
| Color | Applies to compatible objects. |
| Thickness／Size | Contextual control for line、text or marker size. |

Annotation objects remain anchored to Frozen Virtual Desktop coordinates. Changing the capture selection does not scale or move existing annotation objects. Output clips objects to the current selection without deleting clipped data.

## 6. Complete path

```text
[Complete]
→ lock current Selection and Annotation revisions
→ render selected source plus visible annotations
→ write Clipboard
→ on success close overlays and function bar
→ restore the pre-capture application
→ no success notification
```

On recoverable Clipboard or render failure, remain in Editing、preserve user work and show an actionable retry／cancel path.

## 7. Save path

```text
[Save]
→ lock current revisions
→ render final image
→ Windows Save As
→ propose SnipPlus_yyyy-MM-dd_HHmmss.png
→ write PNG
→ write the same final image to Clipboard
→ on both successes close capture UI and restore focus
```

- Save As cancellation returns to Editing without an error.
- PNG failure leaves Editing open and does not update Clipboard.
- Clipboard failure after PNG creation leaves Editing open; PNG retention／rollback remains an explicit open product decision.

## 8. Cancel path

```text
Esc or [Cancel]
→ create no file
→ write no Clipboard
→ close all overlays and function bars
→ dispose capture-session resources
→ restore the pre-capture application
→ do not automatically show the SnipPlus main window
```

## 9. Deferred UI capabilities

Do not include in v1:

- opaque freehand pen;
- ellipse;
- pin image to desktop;
- OCR;
- capture history;
- delayed capture;
- additional save formats;
- font-family selection、italic、underline or text background.

## 10. Open design decisions

- Exact System Tray commands and MainWindow close-button behavior.
- Visual treatment of non-display gaps in irregular monitor arrangements.
- Final keyboard-only Annotation interaction standard.

These remain explicit decisions and must not be silently invented during implementation.
