# SPEC-0009 Annotation Capability

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `SPEC-0009` |
| Feature ID | `FEAT-002` |
| Version | `1.0` |
| Status | `Accepted` |
| Last reviewed | `2026-07-27` |
| Normative sources | `PRD-0004`、`PRD-0005`、`PRD-0006`、`SPEC-0003`、`SPEC-0005` |

## 2. Workflow Boundary

- Annotation actions occur after a valid selection is locked.
- The editing／confirmation stage always appears.
- Annotation actions are optional: the user may press Complete without creating an annotation.
- The stage remains active until Complete、successful Save or Cancel.
- Mouse release from initial selection is not an Annotation completion event.

## 3. Coordinate and Object Model

- Every annotation belongs to one capture Session ID.
- Geometry is stored in Frozen Virtual Desktop coordinates.
- Selection move、resize or reselection does not scale or move existing annotation objects.
- Final render clips annotations to the current selection bounds.
- Object portions outside the selection are not rendered but are not automatically deleted.
- Expanding the selection can reveal previously clipped portions.
- Where applicable, an object can be selected、moved、resized、restyled and deleted.

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

Undo／Redo does not include selection move、selection resize or reselection.

## 5. Required v1 Tools

### 5.1 Rectangle

- Drag creates a rectangular outline.
- Color and line thickness are editable.
- The object supports selection、movement、resize and deletion.

### 5.2 Arrow／Line

- Drag creates a line object.
- At minimum, the end style supports an arrow at the end or no arrow at either end.
- Color and line thickness are editable.
- The object supports selection、movement、resize and deletion.

### 5.3 Highlighter

- Creates a semi-transparent freehand stroke.
- Stroke ends are rounded.
- Color and thickness are editable.
- The general opaque freehand pen is not included in v1.

### 5.4 Text

- Clicking within the selection starts text entry.
- Existing text can be selected、moved and edited.
- Supported style controls are color、font size and bold.
- Default font is Microsoft JhengHei (`微軟正黑體`).
- Font-family selection、italic、underline and text background are deferred.

### 5.5 Mosaic／Blur

- One privacy tool switches between Mosaic and Blur.
- The effect is applied by dragging a rectangular region.
- The selected mode is stored per object; switching the active tool mode does not alter existing objects.
- The object supports selection、movement、resize、mode change and deletion.

### 5.6 Numbered Marker

- The first marker defaults to `1`.
- Each later marker increments by one.
- Deleting a marker does not renumber remaining markers.
- Undo restores the original number; Redo removes it again without recalculating other markers.
- The user can change the next starting number.
- Color and size are editable.
- Default appearance is a solid circle with a centered number.

## 6. Shared Tool Controls

- Color selection applies to compatible tools.
- Line thickness applies to Rectangle、Arrow／Line and Highlighter.
- Text uses font size rather than line thickness.
- Numbered Marker uses marker size.
- Unsupported controls must not silently affect incompatible objects.

## 7. Function Bar Requirements

Required controls:

- selection mode;
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

## 8. Deferred Tools

- Opaque freehand pen.
- Ellipse.
- Pin image to desktop.
- OCR.
- Capture history.
- Delayed capture.

## 9. Acceptance Criteria

| ID | Criterion |
| --- | --- |
| `SPEC-0009-AC-001` | The user can skip all annotation actions and choose Complete. |
| `SPEC-0009-AC-002` | All required v1 tools can create the defined object or effect. |
| `SPEC-0009-AC-003` | Editable objects support the applicable select、move、resize、style and delete operations. |
| `SPEC-0009-AC-004` | Undo／Redo covers annotation changes but not selection geometry changes. |
| `SPEC-0009-AC-005` | Annotation geometry remains anchored to Frozen Virtual Desktop coordinates during selection adjustment. |
| `SPEC-0009-AC-006` | Output clips annotations to the selection without deleting clipped object data. |
| `SPEC-0009-AC-007` | Text defaults to Microsoft JhengHei and supports color、size and bold. |
| `SPEC-0009-AC-008` | Number deletion preserves numbering gaps and Undo restores original numbers. |
| `SPEC-0009-AC-009` | Mosaic and Blur are selectable modes of one rectangular privacy tool. |

The previous tool-agnostic optional Annotation scope is superseded.
