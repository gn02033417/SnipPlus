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

- Owner Reference／Standard Selection becomes interactive within p95 `500 ms`; Maximum within p95 `1,000 ms`.
- Configurations exceeding the accepted four-4K envelope show an actionable error before this surface appears.
- Partial display capture is never shown.

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
- Selection adjustment、tool use and object editing are pointer-driven in v1.
- Pointer interaction maintains p95 frame time `≤ 33 ms` and visible response p95 `≤ 100 ms`.

## 5. Annotation Controls

| Tool | Pointer-driven v1 behavior |
| --- | --- |
| Selection | Move／resize capture region and select applicable Annotation objects. |
| Rectangle | Drag an editable outline. |
| Arrow／Line | Drag an editable arrow or no-arrow line. |
| Highlighter | Draw a semi-transparent freehand stroke. |
| Text | Click to enter text; support Microsoft JhengHei、color、size、bold and Chinese IME. |
| Mosaic／Blur | Drag a rectangular privacy region and switch the object mode. |
| Numbered Marker | Click to place the next numbered marker. |
| Undo／Redo | Use visible function-bar controls for Annotation history. |
| Color／Size | Modify compatible active or selected objects. |

Keyboard-only Annotation、keyboard-created objects and non-PrintScreen tool／action shortcuts are deferred.

## 6. Complete Path

```text
[Complete]
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
[Save]
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

- Save As cancellation returns to Editing.
- PNG failure leaves Editing open and does not update Clipboard.
- Clipboard failure after PNG creation leaves Editing open、retains PNG and reports the partial outcome.
- Save latency after path confirmation uses p95 `2 s`、`6 s` or `12 s` tiers.

## 8. Cancel and Exit

### Capture Cancel

```text
Esc or [Cancel]
→ create no file
→ write no Clipboard
→ close capture UI
→ dispose Session resources
→ restore pre-capture application
```

Esc cancels before Selection、during drag and after Selection lock／during Editing. V1 does not require a transient first-Esc keyboard-editing hierarchy.

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
- each `≤ 3840 × 2160`;
- total source pixels `≤ 33,177,600`;
- Virtual Desktop width／height each `≤ 16,384`;
- final Selection width／height each `≤ 16,384`;
- final Selection area `≤ 67,108,864` pixels;
- 8K displays are outside v1.

The over-limit message states the supported boundary、does not expose private display identifiers and returns SnipPlus to resident readiness after cleanup.

## 10. Owner Reference Runtime Layout

The release-verification layout includes:

```text
left 2560×1440       primary 2560×1440
                      └─ lower 1920×1080 at 150% scaling
```

The exact physical alignment follows the Repository owner’s Windows display configuration during authorized verification.

## 11. Deferred UI Capabilities

- opaque freehand pen;
- ellipse Annotation;
- pin image to desktop;
- OCR;
- capture history;
- delayed capture;
- additional save formats;
- font-family selection、italic、underline or text background;
- keyboard-only Annotation workflow;
- F6／Tab zone and object traversal;
- tool、Ctrl、Delete and Arrow-key shortcuts;
- keyboard-created Annotation objects.

## 12. Remaining Visual Design Freedom

Exact visual theme、icons、spacing、animation and progress-indicator styling remain implementation design choices. Product behavior、four-4K capacity、performance thresholds and deferred keyboard scope are fixed.