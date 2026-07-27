# PRD-0004 Core Workflow

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `PRD-0004` |
| Version | `1.2` |
| Status | `Accepted` |
| Product authority | Repository owner through explicit product decisions |
| Last reviewed | `2026-07-27` |
| Scope | SnipPlus v1 first-release core workflow |

## 2. Product Workflow

SnipPlus 的第一版核心流程固定為：

```text
User manually starts SnipPlus
→ SnipPlus remains resident while the application is running
→ User enables PrintScreen takeover
→ User presses PrintScreen
→ Preserve the current foreground-work context
→ Exclude SnipPlus windows from the capture source
→ Freeze all connected displays for one capture session
→ Present one continuous Virtual Desktop selection canvas
→ User creates a cross-monitor rectangular selection
→ Mouse release locks the selection
→ Show the editing／confirmation function bar
→ User may edit annotations or perform no annotation action
→ User chooses Complete、Save or Cancel
```

### Complete path

```text
Render the current selection and annotations
→ Represent non-display gaps as transparent pixels
→ Write the final image to Clipboard
→ Close all overlays and function bars
→ Restore the pre-capture foreground application and focus
→ End the session without a success notification
```

### Save path

```text
Render the current selection and annotations
→ Represent non-display gaps as transparent pixels
→ Open Windows Save As with Downloads as the initial folder
→ Save PNG
→ Write that same final image to Clipboard
→ Close all overlays and function bars
→ Restore the pre-capture foreground application and focus
→ End the session without a success notification
```

If Save As is cancelled, return to the editing stage. If PNG creation fails, remain in Editing and do not update Clipboard. If PNG creation succeeds but Clipboard delivery fails, retain the PNG at the user-selected destination, remain in Editing and show an actionable Clipboard error. The default Save As destination is the user’s Downloads folder, but the user may choose another folder.

### Cancel path

```text
Discard the current capture session
→ Do not write Clipboard
→ Do not create a file
→ Close all overlays and function bars
→ Restore the pre-capture foreground application and focus
→ Do not show the SnipPlus main window
```

## 3. Entry、Residency and Exit

- The user manually starts SnipPlus.
- After startup, SnipPlus remains resident so it can receive PrintScreen while the main window is not active.
- SnipPlus provides a setting that enables or disables PrintScreen takeover.
- When takeover is disabled, SnipPlus must not intercept PrintScreen.
- PrintScreen is the primary v1 capture entry when takeover is enabled.
- An in-app Start Capture command may remain as a diagnostic or secondary entry, but it is not the primary product workflow.
- Closing the MainWindow with `X` directly exits SnipPlus; it does not hide the application to the System Tray.
- Application exit releases PrintScreen takeover immediately and leaves no hidden resident process.
- If a System Tray surface is present, its explicit Exit action uses the same shutdown path. A separate close-to-tray behavior is not part of v1.

## 4. Capture and Selection Experience

When PrintScreen is accepted:

1. SnipPlus must not present its normal main window.
2. SnipPlus windows must not appear in the frozen capture source.
3. All connected displays are frozen for the same capture session.
4. Every display is covered by a semi-transparent mask.
5. The pointer changes to a crosshair across the selectable Virtual Desktop.
6. The user may create one rectangular selection spanning multiple displays.
7. During drag, the region outside the selection remains dimmed and the region inside the selection shows the original frozen content without the mask.
8. Mouse release locks the selection; it does not complete the capture and does not write Clipboard.
9. Before Complete or Save, the user can move the selection、resize it from edges or corners、or drag elsewhere to create a new selection.

The user-visible Virtual Desktop is one continuous coordinate space. Implementation may retain separate per-display frozen frames, but selection、annotations and final composition must behave as one session canvas.

When a rectangular selection crosses a physical gap between irregularly arranged displays, the gap contains no captured desktop pixels. The corresponding final-image pixels are transparent (`alpha = 0`).

## 5. Editing and Confirmation Stage

The editing／confirmation stage always appears after a valid selection is locked.

Annotation actions are optional; the stage itself is not optional. The user can immediately press Complete to produce an unannotated result.

### Required v1 controls

- Complete.
- Save.
- Cancel.
- Undo and Redo.
- Selection move、edge／corner resize and reselection.
- Rectangle.
- Arrow with a no-arrow line mode.
- Highlighter.
- Text.
- Mosaic／Blur mode switch.
- Numbered marker.
- Color selection.
- Line thickness.

### Deferred from v1

- Opaque freehand pen.
- Ellipse.
- Pin image to desktop.
- OCR.
- Capture history.
- Delayed capture.

The function bar should appear below the selection when space permits and move above it when necessary to remain visible within an available display work area.

## 6. Annotation Product Rules

- All annotation objects are stored in Frozen Virtual Desktop coordinates.
- Annotation is only rendered into the selected output bounds.
- Content outside the current selection is clipped from output but the object is not immediately deleted.
- Resizing or moving the selection does not scale or relocate existing annotation objects.
- Restoring a larger selection can make previously clipped object portions visible again.
- Created objects can be selected、moved、resized、restyled and deleted where applicable.
- Undo／Redo covers annotation creation、deletion、movement、resize、content edits and style changes.
- Selection movement and resizing are not part of the annotation Undo／Redo history.

## 7. Required Annotation Behavior

### Rectangle

Creates an editable rectangular outline using the selected color and line thickness.

### Arrow／Line

Creates an editable line. The end style supports at least:

- arrow at the end;
- no arrow at either end.

### Highlighter

Creates a semi-transparent freehand stroke with rounded ends. The general opaque freehand pen remains deferred.

### Text

- Click inside the selection to enter text.
- Existing text can be moved and edited.
- Color、font size and bold are editable.
- Default font is Microsoft JhengHei (`微軟正黑體`).
- Font selection、italic、underline and text background are deferred.

### Mosaic／Blur

One privacy tool switches between Mosaic and Blur. The user applies the effect with a rectangular region, not a freehand brush.

### Numbered Marker

- First marker defaults to `1`; later markers increment sequentially.
- Deleting a marker does not renumber remaining markers.
- The next starting number can be changed.
- Color and size are editable.
- Default appearance is a solid circular background with a centered number.

## 8. Output Rules

### Complete

- Writes the final image to Clipboard.
- Does not create a file.
- Ends the workflow only after Clipboard delivery succeeds.
- Non-display gaps inside the selected rectangle are transparent.

### Save

- First-release format is PNG only.
- Save As opens each time.
- Save As initially opens the Downloads folder.
- Default filename format is `SnipPlus_yyyy-MM-dd_HHmmss.png`.
- The user may change the destination and filename.
- A successful Save also writes the same final image to Clipboard.
- Cancelling Save As returns to editing.
- A PNG save failure does not close the editor and does not update Clipboard.
- If PNG succeeds but Clipboard fails, the file is retained, the editor remains open and the Clipboard failure is reported.

## 9. Cancellation and Focus Restoration

- `Esc` before selection cancels the entire session.
- `Esc` during drag cancels the current drag and ends the entire session.
- `Esc` after the function bar appears cancels the entire session.
- Cancel never writes Clipboard or creates a file.
- Complete、Save success and Cancel close every capture overlay and function bar.
- Focus returns to the application that was active before PrintScreen.
- SnipPlus does not automatically return to or show its main window.

## 10. Explicit Product Boundaries

The following previous assumptions are superseded:

- In-app Start Capture is not the primary v1 entry.
- Single-monitor selection is not the v1 product scope.
- Cross-monitor selection is not a deferred non-goal.
- Mouse release does not complete capture.
- Annotation is not merely an optional handoff after capture completion.
- Clipboard is not written immediately after region selection.
- Closing MainWindow does not hide SnipPlus to the System Tray.
- Non-display gaps are not filled with arbitrary captured or opaque pixels.
- An already-created PNG is not rolled back after a later Clipboard failure.

## 11. Remaining Product Questions

The following details remain intentionally unresolved and must not be guessed:

- Visual styling beyond the fixed interaction behavior.
- Quantitative latency targets.
- Final keyboard-only annotation acceptance scope.

These questions require explicit product decisions before their affected implementation slice begins.