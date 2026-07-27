# SPEC-0009 Annotation Capability

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `SPEC-0009` |
| Feature ID | `FEAT-002` |
| Version | `1.2` |
| Status | `Accepted` |
| Last reviewed | `2026-07-27` |
| Normative sources | `PRD-0004`、`PRD-0005`、`PRD-0006`、`SPEC-0003`、`SPEC-0005` |

## 2. Workflow Boundary

- Annotation actions occur after a valid Selection is locked.
- The Editing／confirmation stage always appears.
- Annotation actions are optional: the user may press Complete without creating an annotation.
- The stage remains active until Complete、successful Save or Cancel.
- Mouse release from initial Selection is not an Annotation completion event.
- V1 Annotation creation and object editing are pointer-driven.
- Keyboard-only Annotation and non-PrintScreen tool／action shortcuts are deferred.

## 3. Coordinate and Object Model

- Every annotation belongs to one capture Session ID.
- Geometry is stored in Frozen Virtual Desktop physical-pixel coordinates.
- Selection move、resize or reselection does not scale or move existing annotation objects.
- Final render clips annotations to the current Selection bounds.
- Object portions outside the Selection are not rendered but are not automatically deleted.
- Expanding the Selection can reveal previously clipped portions.
- Where applicable, an object can be selected、moved、resized、restyled and deleted with pointer interaction.

## 4. Undo and Redo

Function-bar Undo／Redo controls cover:

- object creation;
- deletion;
- movement;
- resize;
- text content edits;
- numbered-marker setting changes;
- color、line thickness、font size、bold and other supported style changes;
- Mosaic／Blur mode or region edits.

Undo／Redo does not include Selection move、Selection resize or reselection.

Keyboard shortcuts for Undo／Redo are deferred.

## 5. Required v1 Tools

### 5.1 Rectangle

- Pointer drag creates a rectangular outline.
- Color and line thickness are editable.
- The object supports pointer selection、movement、resize and deletion.

### 5.2 Arrow／Line

- Pointer drag creates a line object.
- End style supports an arrow at the end or no arrow at either end.
- Color and line thickness are editable.
- The object supports pointer selection、movement、endpoint resize and deletion.

### 5.3 Highlighter

- Pointer input creates a semi-transparent freehand stroke.
- Stroke ends are rounded.
- Color and thickness are editable.
- The object supports applicable pointer movement、resize and deletion.
- The general opaque freehand pen is not included in v1.

### 5.4 Text

- Pointer click within the Selection starts text entry.
- Existing text can be selected、moved and edited.
- Supported style controls are color、font size and bold.
- Default font is Microsoft JhengHei (`微軟正黑體`).
- Normal text entry and Chinese IME input must work while the text editor owns focus.
- Font-family selection、italic、underline and text background are deferred.

### 5.5 Mosaic／Blur

- One privacy tool switches between Mosaic and Blur.
- Pointer input applies the effect by dragging a rectangular region.
- The selected mode is stored per object; switching the active tool mode does not alter existing objects.
- The object supports pointer selection、movement、resize、mode change and deletion.

### 5.6 Numbered Marker

- The first marker defaults to `1`.
- Each later marker increments by one.
- Pointer input places the marker at the chosen point.
- Deleting a marker does not renumber remaining markers.
- Undo restores the original number; Redo removes it again without recalculating other markers.
- The user can change the next starting number through the function bar.
- Color and size are editable.
- Default appearance is a solid circle with a centered number.

## 6. Shared Tool Controls

- Color selection applies to compatible tools.
- Line thickness applies to Rectangle、Arrow／Line and Highlighter.
- Text uses font size rather than line thickness.
- Numbered Marker uses marker size.
- Unsupported controls must not silently affect incompatible objects.
- Required controls expose understandable accessible names and state.
- Selected tools and errors use indicators in addition to color alone.

## 7. Function Bar

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

## 8. Keyboard Boundary

The following are not v1 acceptance requirements:

- F6 zone navigation;
- Tab／Shift+Tab traversal as a complete canvas／object workflow;
- `V`、`R`、`A`、`H`、`T`、`M` or `N` tool shortcuts;
- Ctrl+Z、Ctrl+Y、Ctrl+S or Ctrl+Enter shortcuts;
- Delete or Arrow-key Annotation manipulation;
- deterministic keyboard-created Annotation objects;
- keyboard-only object creation、editing、styling or output completion;
- a pointer-unused test beginning at `SelectionLocked`.

PrintScreen remains the global capture key. Esc remains the capture-cancellation key under `SPEC-0005` and `SPEC-0006`; Esc is not an Annotation editing shortcut.

## 9. Deferred Tools and Capabilities

- Opaque freehand pen.
- Ellipse.
- Pin image to desktop.
- OCR.
- Capture history.
- Delayed capture.
- Keyboard-only Annotation workflow and non-PrintScreen tool／action shortcuts.

## 10. Acceptance Criteria

| ID | Criterion |
| --- | --- |
| `SPEC-0009-AC-001` | The user can skip all Annotation actions and choose Complete. |
| `SPEC-0009-AC-002` | All required v1 tools can create the defined object or effect through pointer input. |
| `SPEC-0009-AC-003` | Editable objects support applicable pointer select、move、resize、style and delete operations. |
| `SPEC-0009-AC-004` | Function-bar Undo／Redo covers Annotation changes but not Selection geometry changes. |
| `SPEC-0009-AC-005` | Annotation geometry remains anchored to Frozen Virtual Desktop coordinates during Selection adjustment. |
| `SPEC-0009-AC-006` | Output clips annotations to the Selection without deleting clipped object data. |
| `SPEC-0009-AC-007` | Text defaults to Microsoft JhengHei and supports color、size、bold and Chinese IME input. |
| `SPEC-0009-AC-008` | Number deletion preserves numbering gaps and Undo restores original numbers. |
| `SPEC-0009-AC-009` | Mosaic and Blur are selectable modes of one rectangular privacy tool. |
| `SPEC-0009-AC-010` | No keyboard-only Annotation or non-PrintScreen shortcut claim is required for v1 acceptance. |
| `SPEC-0009-AC-011` | Required function-bar controls expose accessible names and selected／error state is not communicated by color alone. |

The previously accepted complete keyboard-only Annotation workflow is superseded and deferred.