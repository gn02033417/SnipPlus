# PRD-0004 Core Workflow

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `PRD-0004` |
| Version | `1.4` |
| Status | `Accepted` |
| Product authority | Repository owner through explicit product decisions |
| Last reviewed | `2026-07-27` |
| Scope | SnipPlus v1 first-release core workflow |

## 2. Product Workflow

```text
User manually starts SnipPlus
→ SnipPlus remains resident while running
→ User enables PrintScreen takeover
→ User presses PrintScreen
→ Preserve the current foreground-work context
→ Validate the supported four-4K display envelope
→ Exclude SnipPlus windows from the capture source
→ Freeze all connected supported displays for one Session
→ Present one continuous Virtual Desktop Selection canvas
→ User creates a cross-monitor rectangular Selection
→ Mouse release validates and locks the Selection
→ Show the Editing／confirmation function bar
→ User may edit annotations with pointer input or perform no Annotation action
→ User chooses Complete、Save or Cancel
```

### Complete path

```text
Validate final Selection dimensions
→ Render current Selection and annotations
→ Represent non-display gaps as transparent pixels
→ If still running after 300 ms, show non-blocking progress
→ Write final image to Clipboard
→ Close overlays and function bar
→ Restore the pre-capture foreground application and focus
→ End silently
```

### Save path

```text
Validate final Selection dimensions
→ Render current Selection and annotations
→ Represent non-display gaps as transparent pixels
→ Open Windows Save As with Downloads as the initial folder
→ Save and retain PNG
→ If post-dialog work remains after 300 ms, show non-blocking progress
→ Write the same final image to Clipboard
→ Close capture UI only after Clipboard succeeds
→ Restore the pre-capture foreground application and focus
→ End silently
```

If Save As is cancelled, return to Editing. If PNG creation fails, remain in Editing and do not update Clipboard. If PNG succeeds but Clipboard fails, retain the PNG、remain in Editing and show an actionable Clipboard error.

### Cancel path

```text
Discard current capture Session
→ Do not write Clipboard
→ Do not create a file
→ Close overlays and function bar
→ Restore the pre-capture foreground application and focus
→ Do not show MainWindow
```

## 3. Entry、Residency and Exit

- The user manually starts SnipPlus.
- SnipPlus remains resident while running so it can receive PrintScreen when MainWindow is not active.
- A setting enables or disables PrintScreen takeover.
- Disabled takeover does not intercept PrintScreen.
- PrintScreen is the primary v1 capture entry.
- An in-app Start Capture command may remain secondary or diagnostic.
- MainWindow `X` directly exits SnipPlus; it does not hide to the System Tray.
- Exit releases PrintScreen takeover and leaves no hidden resident process.
- Any explicit tray Exit action uses the same shutdown path.

## 4. Supported Capacity and Capture Experience

The supported v1 source envelope is:

- `1`–`4` active logical desktop display surfaces;
- each display no larger than `3840 × 2160` physical pixels;
- total active source pixels no greater than `33,177,600`;
- Virtual Desktop width and height each no greater than `16,384` physical pixels;
- final Selection width and height each no greater than `16,384` pixels;
- final Selection area no greater than `67,108,864` pixels;
- transparent non-display gaps count toward final Selection area;
- an 8K display is outside v1.

The larger final-area ceiling permits transparent gaps in irregular layouts without reducing the four-4K source guarantee.

When PrintScreen is accepted:

1. Do not show MainWindow.
2. Validate complete display topology before interactive Selection.
3. Exclude SnipPlus windows from frozen content.
4. Freeze every connected supported display for the same Session.
5. Cover every display with a semi-transparent mask.
6. Change the pointer to a crosshair.
7. Allow one rectangular Selection spanning displays.
8. During drag, show clear frozen content inside and dimmed content outside.
9. Mouse release validates and locks Selection; it does not create output.
10. Before Complete or Save, allow pointer move、edge／corner resize and reselection.

When topology or Selection exceeds a limit:

- do not omit、downscale or partially capture displays;
- fail before interactive Selection or lock, as applicable;
- release partial resources;
- restore the pre-capture work context;
- show an actionable supported-limit message;
- return to resident readiness.

Physical gaps between displays contain no source pixels. Corresponding final-image pixels are transparent (`alpha = 0`).

## 5. Editing and Confirmation

The Editing／confirmation stage always appears after a valid Selection is locked.

Annotation actions are optional. The user may immediately press Complete to produce an unannotated result.

Required controls:

- Complete、Save and Cancel;
- Undo and Redo;
- Selection move、edge／corner resize and reselection;
- Rectangle;
- Arrow with no-arrow line mode;
- Highlighter;
- Text;
- Mosaic／Blur mode switch;
- Numbered Marker;
- Color;
- contextual thickness or size.

The function bar prefers the area below Selection and moves above it when necessary.

## 6. Annotation Product Rules

- Annotation creation and object editing are pointer-driven in v1.
- Annotation objects use Frozen Virtual Desktop physical-pixel coordinates.
- Output clips annotations to current Selection without deleting clipped object data.
- Moving or resizing Selection does not scale or relocate Annotation objects.
- Applicable objects support pointer selection、move、resize、restyle and delete.
- Function-bar Undo／Redo covers Annotation mutations only.
- Selection geometry changes are excluded from Annotation Undo／Redo history.

### Required tools

- **Rectangle:** editable outline with color and thickness.
- **Arrow／Line:** editable line with arrow-end or no-arrow mode.
- **Highlighter:** semi-transparent freehand stroke with rounded ends.
- **Text:** Microsoft JhengHei by default; content、color、font size and bold are editable; Chinese IME is supported.
- **Mosaic／Blur:** one rectangular privacy tool with per-object mode.
- **Numbered Marker:** starts at `1`、increments sequentially、preserves gaps after deletion、allows next-number change、color and size.

## 7. Keyboard Boundary

Required in v1:

- PrintScreen as the global capture key;
- Esc to cancel before Selection、during drag and during stable Editing;
- ordinary text editing and Chinese IME;
- accessible names and non-color-only selected／error indicators.

Deferred from v1:

- complete keyboard-only Annotation workflow;
- F6／Tab zone and object traversal;
- single-letter tool shortcuts;
- Ctrl-based Undo／Redo、Save or Complete shortcuts;
- Delete and Arrow-key object manipulation;
- keyboard-created Annotation objects;
- a pointer-unused acceptance scenario after `SelectionLocked`.

Esc is a core capture-cancellation key, not part of the deferred Annotation shortcut feature.

## 8. Quantitative Quality Targets

### Performance

- PrintScreen → interactive Selection: p95 `≤ 500 ms` Owner Reference／Standard、`≤ 1,000 ms` Maximum.
- Pointer-driven Selection／Annotation frame time: p95 `≤ 33 ms`.
- Pointer／UI action → visible response: p95 `≤ 100 ms`.
- Complete p95 tiers: `≤ 1.5 s`、`4 s`、`8 s`.
- Save p95 tiers after Save As confirmation: `≤ 2 s`、`6 s`、`12 s`.
- A still-running commit displays progress after `300 ms`.

### Memory and measurement

- Idle private working set `≤ 250 MB`.
- Maximum-envelope peak `≤ 2.0 GB`.
- Within `10 seconds` after cleanup, return to idle baseline plus `150 MB` or less.
- After `20` Standard Sessions, retained growth `≤ 50 MB`.
- Verification uses Release x64、no debugger、3 warm-ups and at least 30 measured runs; report p50、p95 and maximum.

### Owner Reference runtime profile

- primary `2560 × 1440`;
- lower `1920 × 1080` at Windows scaling `150%`;
- left `2560 × 1440`.

Detailed profiles and output-size classes are normative in PRD-0006.

## 9. Output Rules

### Complete

- Writes final image to Clipboard.
- Does not create a file.
- Ends only after Clipboard succeeds.
- Preserves transparent non-display gaps.

### Save

- PNG only.
- Save As opens each time in Downloads by default.
- Default filename is `SnipPlus_yyyy-MM-dd_HHmmss.png`.
- The user may change destination and filename.
- Successful Save also writes the same final image to Clipboard.
- Save As cancellation returns to Editing.
- PNG failure leaves Editing open and does not update Clipboard.
- PNG success followed by Clipboard failure retains the file and Editing state.

## 10. Cancellation and Focus Restoration

- Esc before Selection cancels the entire Session.
- Esc during drag cancels the drag and Session.
- Esc from stable Editing cancels the Session.
- Cancel never writes Clipboard or creates a file.
- Complete、Save success and Cancel close capture UI.
- Focus returns to the application active before PrintScreen.
- SnipPlus does not automatically show MainWindow after the Session.

## 11. Explicit Product Boundaries

The following previous assumptions are superseded:

- In-app Start Capture as primary entry.
- Single-monitor Selection.
- Mouse release completing capture.
- Immediate Clipboard after Selection.
- MainWindow close-to-tray.
- Opaque or fabricated display-gap pixels.
- PNG rollback after later Clipboard failure.
- Unlimited or partial display support.
- 8K displays inside v1.
- Complete keyboard-only Annotation and non-PrintScreen shortcuts as v1 requirements.

## 12. Remaining Design Freedom

Exact visual theme、icons、spacing、animation and progress-indicator styling remain implementation design choices. Product behavior、performance、four-4K capacity and deferred keyboard scope are finalized and must not be silently changed.