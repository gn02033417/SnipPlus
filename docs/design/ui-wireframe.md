# UI Wireframe

狀態：`Accepted behavioral wireframe`

This low-fidelity wireframe records the accepted v1 screen responsibilities and interaction order. Exact visual styling、spacing、icons and animation remain implementation design choices unless constrained by PRD／Specs.

## 1. Resident Main-window Responsibility

The user manually starts SnipPlus. The main window provides at least:

```text
+--------------------------------------------------+
| SnipPlus                                      [X]|
|--------------------------------------------------|
| PrintScreen takeover                             |
| [ Enabled / Disabled ]                           |
|                                                  |
| Current status: Resident / Capturing / Error     |
|                                                  |
| [Start Capture - secondary/diagnostic entry]     |
+--------------------------------------------------+
```

Behavior:

- The in-app Start Capture command is secondary or diagnostic, not the primary v1 entry.
- MainWindow `X` directly exits SnipPlus.
- `X` does not hide the application to the System Tray.
- Application exit releases PrintScreen takeover and leaves no hidden resident process.
- If a System Tray surface exists, its explicit Exit command uses the same shutdown path.

## 2. Initial Capture Presentation

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

## 3. Dragging a Cross-monitor Selection

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
- A physical gap between displays has no source image. In final output, that part of the rectangle is transparent.

## 4. Locked Selection and Function Bar

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

## 5. Annotation Controls

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

## 6. Complete Path

```text
[Complete]
→ lock current Selection and Annotation revisions
→ render selected source plus visible annotations
→ use transparent pixels for physical display gaps
→ write Clipboard
→ on success close overlays and function bar
→ restore the pre-capture application
→ no success notification
```

On recoverable Clipboard or render failure, remain in Editing、preserve user work and show an actionable retry／cancel path.

## 7. Save Path

```text
[Save]
→ lock current revisions
→ render final image with transparent display-gap pixels
→ Windows Save As opens in Downloads by default
→ propose SnipPlus_yyyy-MM-dd_HHmmss.png
→ user may change destination and filename
→ write PNG
→ retain the PNG
→ write the same final image to Clipboard
→ on both successes close capture UI and restore focus
```

- Save As cancellation returns to Editing without an error.
- PNG failure leaves Editing open and does not update Clipboard.
- Clipboard failure after PNG creation leaves Editing open and retains the PNG at the selected destination.
- Feedback states that file saving succeeded but Clipboard delivery failed.

## 8. Cancel Path

```text
Esc or [Cancel]
→ create no file
→ write no Clipboard
→ close all overlays and function bars
→ dispose capture-session resources
→ restore the pre-capture application
→ do not automatically show the SnipPlus main window
```

## 9. Application Exit Path

```text
MainWindow [X] or explicit Exit
→ release PrintScreen takeover
→ cancel or invalidate owned work
→ dispose application resources
→ terminate SnipPlus
```

This is not capture Cancel and is not hide-to-tray behavior.

## 10. Deferred UI Capabilities

Do not include in v1:

- opaque freehand pen;
- ellipse;
- pin image to desktop;
- OCR;
- capture history;
- delayed capture;
- additional save formats;
- font-family selection、italic、underline or text background.

## 11. Remaining Open Design Decisions

- Exact visual styling beyond accepted behavior.
- Final keyboard-only Annotation interaction standard.
- Quantitative performance and maximum-display limits after measurement.

System Tray close behavior、transparent gap output、Downloads default and PNG retention are resolved and must not be treated as open.