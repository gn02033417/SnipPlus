# PRD-0005 Functional Requirements

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `PRD-0005` |
| Version | `1.2` |
| Status | `Accepted` |
| Product authority | Repository owner through explicit product decisions |
| Last reviewed | `2026-07-27` |
| Scope | SnipPlus v1 first release |

## 2. Requirement Rules

- `Must` means required for the first release.
- `Deferred` means explicitly excluded from the first release.
- Requirements describe observable product capability, not implementation APIs.
- Specs、Architecture、code and tests must trace to these IDs.

## 3. Capture Entry、Residency and Exit

| ID | Requirement | Priority |
| --- | --- | --- |
| `FR-001` | The user can manually start SnipPlus and keep it resident while the application is running. | `Must` |
| `FR-002` | SnipPlus provides a setting to enable or disable PrintScreen takeover. | `Must` |
| `FR-003` | When takeover is enabled and SnipPlus is resident, pressing PrintScreen starts one capture session. | `Must` |
| `FR-004` | When takeover is disabled or SnipPlus exits, SnipPlus does not intercept PrintScreen. | `Must` |
| `FR-005` | An in-app capture command may exist as a secondary or diagnostic entry but is not the primary v1 entry. | `Must` |
| `FR-046` | Closing MainWindow with `X` directly exits SnipPlus, releases PrintScreen takeover and does not hide the application to the System Tray. | `Must` |
| `FR-047` | If a System Tray surface exists, its explicit Exit action uses the same application-exit and PrintScreen-release path. | `Must` |

## 4. Multi-display Freeze and Selection

| ID | Requirement | Priority |
| --- | --- | --- |
| `FR-006` | A capture session freezes all connected displays before selection begins. | `Must` |
| `FR-007` | The user sees one continuous Virtual Desktop selection canvas covering all connected displays. | `Must` |
| `FR-008` | Every display shows a semi-transparent mask and the pointer becomes a crosshair during initial selection. | `Must` |
| `FR-009` | The user can create one rectangular selection that spans multiple displays. | `Must` |
| `FR-010` | During drag, the selected region shows the original frozen content without the mask while the outside region remains dimmed. | `Must` |
| `FR-011` | Releasing the mouse locks the selection but does not complete capture or write Clipboard. | `Must` |
| `FR-012` | Before output, the user can move the locked selection、resize it from edges or corners、or drag elsewhere to create a new selection. | `Must` |
| `FR-048` | Final-image pixels corresponding to physical non-display gaps inside a cross-monitor selection are transparent. | `Must` |

## 5. Editing and Confirmation

| ID | Requirement | Priority |
| --- | --- | --- |
| `FR-013` | A function bar appears after a valid selection is locked. | `Must` |
| `FR-014` | The editing／confirmation stage always appears, but the user can skip all annotation actions and immediately choose Complete. | `Must` |
| `FR-015` | The user can choose Complete、Save or Cancel from the editing stage. | `Must` |
| `FR-016` | The function bar remains visible by preferring the area below the selection and moving above it when needed. | `Must` |

## 6. Annotation Tools

| ID | Requirement | Priority |
| --- | --- | --- |
| `FR-017` | The user can create、select、move、resize、restyle and delete rectangle annotations. | `Must` |
| `FR-018` | The user can create an arrow and switch it to a no-arrow straight-line mode. | `Must` |
| `FR-019` | The user can create semi-transparent freehand highlighter strokes. | `Must` |
| `FR-020` | The user can create、move and edit text using Microsoft JhengHei by default, with color、font size and bold controls. | `Must` |
| `FR-021` | The user can apply rectangular Mosaic or Blur regions using one tool with a mode switch. | `Must` |
| `FR-022` | The user can place numbered markers that increment sequentially, preserve gaps after deletion, allow a new starting number and support color／size changes. | `Must` |
| `FR-023` | The user can choose annotation color and line thickness where applicable. | `Must` |
| `FR-024` | The user can Undo and Redo annotation creation、deletion、movement、resize、content and style changes. | `Must` |
| `FR-025` | Selection movement、selection resize and reselection are not part of annotation Undo／Redo history. | `Must` |

## 7. Annotation Coordinate and Clipping Rules

| ID | Requirement | Priority |
| --- | --- | --- |
| `FR-026` | Annotation objects are anchored to Frozen Virtual Desktop coordinates. | `Must` |
| `FR-027` | Annotation output is clipped to the current selection bounds. | `Must` |
| `FR-028` | Annotation portions outside a reduced selection are not output but the underlying objects are not deleted. | `Must` |
| `FR-029` | Moving or resizing the selection does not scale or relocate existing annotation objects. | `Must` |

## 8. Clipboard and File Output

| ID | Requirement | Priority |
| --- | --- | --- |
| `FR-030` | Complete renders the current selection and annotations、writes the final image to Clipboard and ends only after Clipboard delivery succeeds. | `Must` |
| `FR-031` | Save opens Windows Save As、supports PNG only in v1、initially proposes the Downloads folder and proposes `SnipPlus_yyyy-MM-dd_HHmmss.png`. | `Must` |
| `FR-032` | A successful Save writes the same final image to both the selected PNG file and Clipboard, then ends the session. | `Must` |
| `FR-033` | Cancelling Save As returns to the editing stage without cancelling the capture session. | `Must` |
| `FR-034` | Save or Clipboard failure leaves the editing stage open and provides an actionable error. | `Must` |
| `FR-035` | Successful completion does not show a success notification. | `Must` |
| `FR-049` | The user may change the Save As folder and filename from their proposed defaults. | `Must` |
| `FR-050` | If PNG creation succeeds but later Clipboard delivery fails, SnipPlus retains the created PNG at the selected destination、returns to Editing and reports the Clipboard failure. | `Must` |

## 9. Cancel and Focus Restoration

| ID | Requirement | Priority |
| --- | --- | --- |
| `FR-036` | Esc before selection cancels the entire capture session. | `Must` |
| `FR-037` | Esc during drag cancels the drag and the entire capture session. | `Must` |
| `FR-038` | Esc after the function bar appears cancels the entire capture session. | `Must` |
| `FR-039` | Cancel writes neither Clipboard nor a file. | `Must` |
| `FR-040` | Complete、successful Save and Cancel close all capture overlays and function bars. | `Must` |
| `FR-041` | After Complete、successful Save or Cancel, focus returns to the application active before PrintScreen. | `Must` |
| `FR-042` | SnipPlus does not automatically show its main window after the session ends. | `Must` |
| `FR-043` | SnipPlus normal windows are excluded from the frozen capture source. | `Must` |

## 10. Error Feedback

| ID | Requirement | Priority |
| --- | --- | --- |
| `FR-044` | Capture、freeze、selection、render、save or Clipboard failures are not silently reported as success. | `Must` |
| `FR-045` | Recoverable output failures retain the current selection and annotations so the user can retry or cancel. | `Must` |

## 11. Explicitly Deferred Capabilities

| ID | Capability | Priority |
| --- | --- | --- |
| `FR-D01` | Opaque freehand pen | `Deferred` |
| `FR-D02` | Ellipse annotation | `Deferred` |
| `FR-D03` | Pin image to desktop | `Deferred` |
| `FR-D04` | OCR | `Deferred` |
| `FR-D05` | Capture history | `Deferred` |
| `FR-D06` | Delayed capture | `Deferred` |
| `FR-D07` | Additional save formats beyond PNG | `Deferred` |
| `FR-D08` | Font family selection、italic、underline and text background | `Deferred` |

## 12. Traceability Summary

| Capability | Requirement IDs |
| --- | --- |
| Entry／residency／exit | `FR-001` – `FR-005`、`FR-046` – `FR-047` |
| Multi-display selection／gap output | `FR-006` – `FR-012`、`FR-048` |
| Editing confirmation | `FR-013` – `FR-016` |
| Annotation tools | `FR-017` – `FR-025` |
| Coordinates／clipping | `FR-026` – `FR-029` |
| Clipboard／file output | `FR-030` – `FR-035`、`FR-049` – `FR-050` |
| Cancel／focus | `FR-036` – `FR-043` |
| Error feedback | `FR-044` – `FR-045` |
| Deferred | `FR-D01` – `FR-D08` |

The previous `FR-001`–`FR-045` v1.1 wording remains valid except where superseded or extended by `FR-046`–`FR-050` in this accepted v1.2 requirement set.