# UI Wireframe

狀態：`Accepted behavioral wireframe`

This low-fidelity wireframe records accepted v1 screen responsibilities and interaction order. Exact visual styling、spacing、icons and animation remain implementation design choices unless constrained by PRD／Specs.

## 1. Resident Main-window Responsibility

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

- MainWindow `X` directly exits SnipPlus.
- `X` does not hide the application to the System Tray.
- Exit releases PrintScreen takeover and leaves no hidden process.
- A tray Exit command, if present, uses the same path.

## 2. Initial Capture Presentation

After enabled PrintScreen and successful capacity validation:

```text
All connected supported displays
┌──────────────────────────────┐ ┌──────────────────────────────┐
│ Frozen display content       │ │ Frozen display content       │
│ covered by translucent mask  │ │ covered by translucent mask  │
│                              │ │                              │
│              +               │ │                              │
│        crosshair pointer     │ │                              │
└──────────────────────────────┘ └──────────────────────────────┘
```

- All-display Selection becomes interactive within p95 `500 ms` Standard or `1,000 ms` Maximum.
- Configurations exceeding the accepted capacity envelope show an actionable error before this surface appears.
- Partial display capture is not shown.

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

- One rectangular Selection may cross display boundaries.
- A physical display gap has no source image and becomes transparent in final output.
- Mouse release validates size、locks Selection and creates no output.

## 4. Locked Selection and Function Bar

```text
┌───────────────────────────────────────────────┐
│                                               │
│     ┌─────────────────────────────────┐       │
│     │         locked Selection        │       │
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

- Prefer the function bar below Selection; move above when required.
- Keep it inside an available display work area.
- Pointer and keyboard interaction maintain p95 frame time `≤ 33 ms` and visible response p95 `≤ 100 ms`.

## 5. Keyboard-only Editing Model

Scope begins after a valid `SelectionLocked`; initial crosshair Selection remains pointer-driven.

```text
F6               switch Function Bar / Canvas zones
Tab / Shift+Tab  navigate controls, Selection, objects and handles
V R A H T M N    Selection / Rectangle / Arrow / Highlighter / Text / Mosaic / Number
Ctrl+Z / Ctrl+Y  Undo / Redo
Ctrl+S           Save
Ctrl+Enter       Complete
Delete           delete selected annotation
Arrow            move or resize by 1 pixel
Shift+Arrow      move or resize by 10 pixels
```

Keyboard tool activation creates and focuses a deterministic default object:

- Rectangle and Mosaic／Blur: centered default rectangle.
- Arrow／Line: centered horizontal segment.
- Highlighter: short centered horizontal stroke.
- Text: focused text box.
- Numbered Marker: marker at Selection center.

The first Esc closes transient picker、popover、text editor or uncommitted creation. Esc from stable Editing cancels the capture session.

Visible focus、High Contrast、200% scaling、Narrator names／states、Chinese IME and no keyboard trap are required.

## 6. Complete Path

```text
[Complete] or Ctrl+Enter
→ validate output dimensions
→ render source + annotations + transparent gap pixels
→ if still running after 300 ms, show non-blocking progress
→ write Clipboard
→ close capture UI
→ restore the pre-capture application
→ no success notification
```

Latency targets depend on output pixels: p95 `1.5 s`、`4 s` or `8 s`.

## 7. Save Path

```text
[Save] or Ctrl+S
→ validate output dimensions
→ render final image
→ Windows Save As opens in Downloads by default
→ propose SnipPlus_yyyy-MM-dd_HHmmss.png
→ user may change destination and filename
→ write and retain PNG
→ if post-dialog work remains after 300 ms, show non-blocking progress
→ write the same final image to Clipboard
→ on both successes close capture UI and restore focus
```

- Save As cancellation returns to Editing and restores focus.
- PNG failure leaves Editing open and does not update Clipboard.
- Clipboard failure after PNG creation leaves Editing open、retains PNG and reports the partial outcome.
- Save latency after path confirmation uses p95 `2 s`、`6 s` or `12 s` tiers.

## 8. Cancel and Exit

### Capture Cancel

```text
Esc from stable stage or [Cancel]
→ create no file
→ write no Clipboard
→ close capture UI
→ dispose session resources
→ restore pre-capture application
```

### Application Exit

```text
MainWindow [X] or explicit Exit
→ release PrintScreen takeover
→ cancel or invalidate owned work
→ dispose application resources
→ terminate SnipPlus
```

## 9. Capacity Feedback

Accepted limits:

- `1`–`4` logical displays;
- each `≤ 7,680 × 4,320`;
- total source pixels `≤ 66,355,200`;
- Virtual Desktop width／height each `≤ 16,384`;
- final Selection area `≤ 67,108,864` pixels with dimension caps.

An over-limit message states the supported boundary、does not expose private display identifiers and returns SnipPlus to resident readiness after cleanup.

## 10. Deferred UI Capabilities

- opaque freehand pen;
- ellipse Annotation;
- pin image to desktop;
- OCR;
- capture history;
- delayed capture;
- additional save formats;
- font-family selection、italic、underline or text background.

## 11. Remaining Visual Design Freedom

Exact visual theme、icons、spacing、animation and progress-indicator styling remain implementation design choices. Product behavior、capacity、performance thresholds and keyboard operation are fixed and are not open design decisions.