# PRD-0004 Core Workflow

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `PRD-0004` |
| Version | `1.3` |
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
→ Validate the supported display envelope
→ Exclude SnipPlus windows from the capture source
→ Freeze all connected supported displays for one capture session
→ Present one continuous Virtual Desktop Selection canvas
→ User creates a cross-monitor rectangular Selection
→ Mouse release validates and locks the Selection
→ Show the Editing／confirmation function bar
→ User may edit annotations by pointer or keyboard, or perform no Annotation action
→ User chooses Complete、Save or Cancel
```

### Complete path

```text
Validate final Selection dimensions
→ Render the current Selection and annotations
→ Represent non-display gaps as transparent pixels
→ If still running after 300 ms, show non-blocking progress
→ Write the final image to Clipboard
→ Close all overlays and function bars
→ Restore the pre-capture foreground application and focus
→ End the session without a success notification
```

### Save path

```text
Validate final Selection dimensions
→ Render the current Selection and annotations
→ Represent non-display gaps as transparent pixels
→ Open Windows Save As with Downloads as the initial folder
→ Save and retain PNG
→ If post-dialog work remains after 300 ms, show non-blocking progress
→ Write that same final image to Clipboard
→ Close all overlays and function bars only after Clipboard success
→ Restore the pre-capture foreground application and focus
→ End the session without a success notification
```

If Save As is cancelled, return to Editing. If PNG creation fails, remain in Editing and do not update Clipboard. If PNG creation succeeds but Clipboard delivery fails, retain the PNG at the user-selected destination、remain in Editing、restore the relevant keyboard focus context and show an actionable Clipboard error.

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
- Closing MainWindow with `X` directly exits SnipPlus; it does not hide the application to the System Tray.
- Application exit releases PrintScreen takeover immediately and leaves no hidden resident process.
- If a System Tray surface is present, its explicit Exit action uses the same shutdown path.

## 4. Supported Capacity and Capture Experience

The supported v1 envelope is:

- `1`–`4` active logical desktop display surfaces;
- each display no larger than `7,680 × 4,320` physical pixels;
- total active source pixels no greater than `66,355,200`;
- Virtual Desktop width and height each no greater than `16,384` physical pixels;
- final Selection width and height each no greater than `16,384` pixels;
- final Selection area no greater than `67,108,864` pixels.

Transparent non-display gaps count toward final Selection area. Mirrored outputs resolving to one logical surface count once.

When PrintScreen is accepted:

1. SnipPlus must not present its normal main window.
2. The complete display topology is validated before interactive Selection.
3. SnipPlus windows must not appear in the frozen capture source.
4. All connected supported displays are frozen for the same capture session.
5. Every display is covered by a semi-transparent mask.
6. The pointer changes to a crosshair across the selectable Virtual Desktop.
7. The user may create one rectangular Selection spanning multiple displays.
8. During drag, the region outside the Selection remains dimmed and the region inside shows original frozen content without the mask.
9. Mouse release validates and locks the Selection; it does not complete capture and does not write Clipboard.
10. Before Complete or Save, the user can move the Selection、resize it from edges or corners、or replace it.

When the topology or Selection exceeds any accepted limit:

- do not omit、downscale or partially capture displays;
- fail before interactive Selection or lock, as applicable;
- release partial resources;
- restore the pre-capture work context;
- provide an actionable supported-limit message;
- return to resident readiness.

A physical gap between irregularly arranged displays contains no captured pixels. Corresponding final-image pixels are transparent (`alpha = 0`).

## 5. Editing and Confirmation Stage

The Editing／confirmation stage always appears after a valid Selection is locked.

Annotation actions are optional; the stage itself is not optional. The user can immediately press Complete to produce an unannotated result.

Required v1 controls:

- Complete、Save and Cancel;
- Undo and Redo;
- Selection move、edge／corner resize and reselection;
- Rectangle;
- Arrow with no-arrow line mode;
- Highlighter;
- Text;
- Mosaic／Blur mode switch;
- Numbered Marker;
- Color selection;
- contextual thickness or size.

The function bar appears below the Selection when space permits and moves above it when necessary to remain visible.

## 6. Keyboard-only Editing

The complete keyboard-only v1 scope begins after `SelectionLocked`. Initial crosshair Selection remains pointer-driven.

From `SelectionLocked` onward, the user can without a pointer:

- navigate Function Bar and Canvas zones with `F6`;
- navigate controls、Selection、objects and handles with `Tab`／`Shift+Tab`;
- select tools with `V/R/A/H/T/M/N` when text entry is not active;
- create every required Annotation object using deterministic default placement;
- move Selection or objects by `1` pixel with Arrow keys and `10` pixels with Shift+Arrow;
- resize through focused handles using the same increments;
- edit text with normal Windows behavior and Chinese IME;
- change supported color、thickness、size、mode、bold and number values;
- Delete、Undo、Redo、Save、Complete and Cancel;
- return predictably from Save As、pickers、popovers and failures without a keyboard trap.

The first Esc closes or abandons transient picker、popover、text editor or uncommitted creation state. Esc from stable Editing cancels the complete capture session.

Visible focus、High Contrast、200% scaling and Narrator-readable names／states are required.

## 7. Annotation Product Rules

- All Annotation objects are stored in Frozen Virtual Desktop physical-pixel coordinates.
- Annotation is rendered only into selected output bounds.
- Content outside the current Selection is clipped but not deleted.
- Resizing or moving Selection does not scale or relocate existing Annotation objects.
- Restoring a larger Selection can reveal previously clipped portions.
- Applicable objects can be selected、moved、resized、restyled and deleted by pointer and keyboard.
- Undo／Redo covers Annotation creation、deletion、movement、resize、content edits and style changes.
- Selection geometry changes are not part of Annotation Undo／Redo history.

## 8. Required Annotation Behavior

### Rectangle

Creates an editable rectangular outline. Keyboard activation creates a deterministic centered rectangle.

### Arrow／Line

Creates an editable line with end-arrow or no-arrow mode. Keyboard activation creates a centered horizontal segment.

### Highlighter

Creates a semi-transparent freehand stroke with rounded ends. Keyboard activation creates a short centered horizontal highlighter stroke.

### Text

- Pointer click or keyboard activation begins text entry.
- Existing text can be moved and edited.
- Color、font size and bold are editable.
- Default font is Microsoft JhengHei (`微軟正黑體`).
- Chinese IME input is supported.
- Font selection、italic、underline and text background are deferred.

### Mosaic／Blur

One privacy tool switches between Mosaic and Blur and creates a rectangular region. Keyboard activation creates a deterministic centered region.

### Numbered Marker

- First marker defaults to `1`; later markers increment sequentially.
- Deleting a marker does not renumber remaining markers.
- The next starting number can be changed.
- Color and size are editable.
- Keyboard activation places the marker at Selection center.

## 9. Quantitative Quality Targets

### Performance

- PrintScreen accepted → interactive all-display Selection: p95 `≤ 500 ms` Standard、`≤ 1,000 ms` Maximum.
- Selection／Annotation frame time: p95 `≤ 33 ms`.
- Discrete input → visible response: p95 `≤ 100 ms`.
- Complete p95 tiers: `≤ 1.5 s`、`4 s`、`8 s`.
- Save after Save As confirmation p95 tiers: `≤ 2 s`、`6 s`、`12 s`.
- A still-running commit displays progress after `300 ms`.

### Memory and measurement

- Idle private working set `≤ 250 MB`.
- Maximum-envelope peak `≤ 2.0 GB`.
- Within `10 seconds` after cleanup, return to idle baseline plus `150 MB` or less.
- After `20` Standard sessions, retained growth `≤ 50 MB`.
- Verification uses Release x64、no debugger、3 warm-ups and at least 30 measured runs; report p50、p95 and maximum.

Detailed profiles and output-size classes are normative in PRD-0006.

## 10. Output Rules

### Complete

- Writes the final image to Clipboard.
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

## 11. Cancellation and Focus Restoration

- Esc before Selection cancels the entire session.
- Esc during drag cancels the drag and session.
- Esc first closes transient Editing state; Esc from stable Editing cancels the session.
- Cancel never writes Clipboard or creates a file.
- Complete、Save success and Cancel close every capture overlay and function bar.
- Focus returns to the application active before PrintScreen.
- SnipPlus does not automatically show MainWindow after the session.

## 12. Explicit Product Boundaries

The following previous assumptions are superseded:

- In-app Start Capture as primary entry.
- Single-monitor Selection.
- Mouse release completing capture.
- Annotation as optional post-capture handoff.
- Immediate Clipboard after region Selection.
- MainWindow close-to-tray.
- Opaque or fabricated display-gap pixels.
- PNG rollback after later Clipboard failure.
- Unlimited or partial display support.
- Undefined quantitative performance and memory acceptance.
- Pointer-only Annotation acceptance.

## 13. Remaining Design Freedom

Exact visual theme、icons、spacing、animation and progress-indicator styling remain implementation design choices. Product behavior、capacity、performance and keyboard acceptance are finalized and must not be guessed or silently relaxed.