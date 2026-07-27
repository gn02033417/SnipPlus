# SPEC-0009 Annotation Capability

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `SPEC-0009` |
| Feature ID | `FEAT-002` |
| Version | `1.1` |
| Status | `Accepted` |
| Last reviewed | `2026-07-27` |
| Normative sources | `PRD-0004`、`PRD-0005`、`PRD-0006`、`SPEC-0003`、`SPEC-0005` |

## 2. Workflow Boundary

- Annotation actions occur after a valid Selection is locked.
- The Editing／confirmation stage always appears.
- Annotation actions are optional: the user may press Complete without creating an annotation.
- The stage remains active until Complete、successful Save or Cancel.
- Mouse release from initial Selection is not an Annotation completion event.
- The complete keyboard-only Annotation scope begins at `SelectionLocked`. Initial crosshair region creation remains pointer-driven in v1.

## 3. Coordinate and Object Model

- Every annotation belongs to one capture Session ID.
- Geometry is stored in Frozen Virtual Desktop physical-pixel coordinates.
- Selection move、resize or reselection does not scale or move existing annotation objects.
- Final render clips annotations to the current Selection bounds.
- Object portions outside the Selection are not rendered but are not automatically deleted.
- Expanding the Selection can reveal previously clipped portions.
- Where applicable, an object can be selected、moved、resized、restyled and deleted.
- Keyboard movement and resize use `1` physical output pixel per Arrow key and `10` pixels per Shift+Arrow.

## 4. Undo and Redo

Undo／Redo must cover:

- object creation;
- deletion;
- movement;
- resize;
- text content edits;
- numbered-marker setting changes;
- color、line thickness、font size、bold and other supported style changes;
- Mosaic／Blur mode or region edits.

Undo／Redo does not include Selection move、Selection resize or reselection.

## 5. Required v1 Tools

### 5.1 Rectangle

- Pointer drag creates a rectangular outline.
- Keyboard activation creates a deterministic default rectangle centered inside the current Selection and focuses it.
- Color and line thickness are editable.
- The object supports selection、movement、resize and deletion.

### 5.2 Arrow／Line

- Pointer drag creates a line object.
- Keyboard activation creates a deterministic horizontal segment centered inside the current Selection and focuses it.
- At minimum, the end style supports an arrow at the end or no arrow at either end.
- Color and line thickness are editable.
- The object supports selection、movement、endpoint resize and deletion.

### 5.3 Highlighter

- Pointer input creates a semi-transparent freehand stroke.
- Keyboard activation creates a deterministic short horizontal highlighter stroke centered inside the current Selection and focuses it.
- Stroke ends are rounded.
- Color and thickness are editable.
- The object supports keyboard movement、applicable handle resize and deletion.
- The general opaque freehand pen is not included in v1.

### 5.4 Text

- Pointer click within the Selection starts text entry.
- Keyboard activation creates a text box inside the current Selection、focuses it and enters text editing.
- Existing text can be selected、moved and edited.
- Supported style controls are color、font size and bold.
- Default font is Microsoft JhengHei (`微軟正黑體`).
- Normal Windows text navigation、selection、Clipboard shortcuts and Chinese IME input must work while the text editor owns focus.
- Font-family selection、italic、underline and text background are deferred.

### 5.5 Mosaic／Blur

- One privacy tool switches between Mosaic and Blur.
- Pointer input applies the effect by dragging a rectangular region.
- Keyboard activation creates a deterministic default rectangular region centered inside the Selection and focuses it.
- The selected mode is stored per object; switching the active tool mode does not alter existing objects.
- The object supports selection、movement、resize、mode change and deletion.

### 5.6 Numbered Marker

- The first marker defaults to `1`.
- Each later marker increments by one.
- Pointer input places the marker at the chosen point.
- Keyboard activation places the marker at the current Selection center and focuses it.
- Deleting a marker does not renumber remaining markers.
- Undo restores the original number; Redo removes it again without recalculating other markers.
- The user can change the next starting number through a keyboard-accessible value control.
- Color and size are editable.
- Default appearance is a solid circle with a centered number.

## 6. Shared Tool Controls

- Color selection applies to compatible tools.
- Line thickness applies to Rectangle、Arrow／Line and Highlighter.
- Text uses font size rather than line thickness.
- Numbered Marker uses marker size.
- Unsupported controls must not silently affect incompatible objects.
- Every value control exposes its current value and permitted change through keyboard and accessibility APIs.

## 7. Function Bar and Focus Model

Required controls:

- Selection mode;
- Rectangle;
- Arrow／Line;
- Highlighter;
- Text;
- Mosaic／Blur;
- Numbered Marker;
- Undo;
- Redo;
- color;
- thickness or applicable size control;
- Save;
- Cancel;
- Complete.

The exact visual styling is not fixed, but required controls must be discoverable without multi-level menu nesting for ordinary use.

Keyboard focus rules:

- `F6` cycles major zones: function bar and canvas／object zone.
- `Tab`／`Shift+Tab` navigate controls within the active zone.
- In the canvas zone, traversal order is locked Selection、annotation objects in deterministic z-order and applicable resize handles.
- Focus is always visibly indicated by more than color alone.
- Returning from Save As、pickers、popovers or text editing restores focus to the invoking control or object.
- No operation may create a keyboard trap.

## 8. Required Keyboard Commands

When text entry is not active:

| Command | Action |
| --- | --- |
| `V` | Selection mode |
| `R` | Rectangle |
| `A` | Arrow／Line |
| `H` | Highlighter |
| `T` | Text |
| `M` | Mosaic／Blur |
| `N` | Numbered Marker |
| `Ctrl+Z` | Undo |
| `Ctrl+Y` | Redo |
| `Ctrl+S` | Save |
| `Ctrl+Enter` | Complete |
| `Delete` | Delete selected annotation object |
| Arrow keys | Move selected object or Selection by `1` pixel; move focused resize handle by `1` pixel |
| Shift+Arrow | Same operation by `10` pixels |
| `Enter`／`Space` | Activate focused control、tool、mode or value action |

Esc hierarchy:

1. Close an open picker、popover or dialog-owned transient state where applicable.
2. End or abandon the current uncommitted object-creation or text-editing operation and return to stable Editing.
3. Esc from stable Editing cancels the complete capture session according to `SPEC-0006`.

## 9. Keyboard-only Acceptance Procedure

Acceptance begins with a valid locked Selection and the pointer unused for the remainder of the scenario.

The test must demonstrate:

1. Function bar and canvas-zone navigation in both directions.
2. Creation of every required v1 object using only keyboard commands.
3. Deterministic object selection in z-order.
4. Movement and applicable resize at `1`-pixel and `10`-pixel increments.
5. Style、mode、text、number and size editing.
6. Delete、Undo and Redo.
7. Save As cancellation and return to the invoking context.
8. Complete、Save and Cancel invocation.
9. No keyboard trap after any picker、popover、text editor or Save As transition.
10. Visible focus at normal and 200% UI scaling.
11. Usable operation in Windows High Contrast.
12. Narrator-readable names、roles、states and values for all required controls and selected objects.
13. Chinese IME text entry without triggering single-letter tool shortcuts.

A passing mouse-driven Annotation test does not satisfy keyboard-only acceptance.

## 10. Deferred Tools and Accessibility Scope

Deferred tools:

- Opaque freehand pen.
- Ellipse.
- Pin image to desktop.
- OCR.
- Capture history.
- Delayed capture.

The v1 keyboard-only standard does not require initial crosshair region creation without a pointer. It covers the complete Editing／Annotation stage after `SelectionLocked`.

## 11. Acceptance Criteria

| ID | Criterion |
| --- | --- |
| `SPEC-0009-AC-001` | The user can skip all Annotation actions and choose Complete. |
| `SPEC-0009-AC-002` | All required v1 tools can create the defined object or effect through pointer input. |
| `SPEC-0009-AC-003` | Editable objects support the applicable select、move、resize、style and delete operations. |
| `SPEC-0009-AC-004` | Undo／Redo covers Annotation changes but not Selection geometry changes. |
| `SPEC-0009-AC-005` | Annotation geometry remains anchored to Frozen Virtual Desktop coordinates during Selection adjustment. |
| `SPEC-0009-AC-006` | Output clips annotations to the Selection without deleting clipped object data. |
| `SPEC-0009-AC-007` | Text defaults to Microsoft JhengHei and supports color、size、bold and Chinese IME input. |
| `SPEC-0009-AC-008` | Number deletion preserves numbering gaps and Undo restores original numbers. |
| `SPEC-0009-AC-009` | Mosaic and Blur are selectable modes of one rectangular privacy tool. |
| `SPEC-0009-AC-010` | From `SelectionLocked`, every required v1 object can be created without pointer input. |
| `SPEC-0009-AC-011` | F6、Tab and Shift+Tab provide deterministic zone、object and handle navigation without a keyboard trap. |
| `SPEC-0009-AC-012` | Arrow and Shift+Arrow perform deterministic `1`-pixel and `10`-pixel move／resize operations. |
| `SPEC-0009-AC-013` | Tool shortcuts、Undo／Redo、Save、Complete and Delete work whenever text entry does not own the keystroke. |
| `SPEC-0009-AC-014` | Esc closes transient editing state before stable-Editing session cancellation. |
| `SPEC-0009-AC-015` | Visible focus、High Contrast、200% scaling and Narrator state are verified for the complete function bar and object workflow. |
| `SPEC-0009-AC-016` | The keyboard-only acceptance procedure completes with the pointer unused after Selection lock. |

The previous tool-agnostic optional Annotation scope and undefined keyboard-only standard are superseded.