# SPEC-0005 Capture Workflow

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `SPEC-0005` |
| Feature ID | `FEAT-001` |
| Version | `1.0` |
| Status | `Accepted` |
| Last reviewed | `2026-07-27` |
| Normative sources | `PRD-0004`、`PRD-0005`、`PRD-0006`、`SPEC-0003` |

## 2. Scope

This Spec defines the first-release capture path from PrintScreen through a locked, editable, cross-monitor selection. It does not define annotation rendering details or output adapter internals.

```text
ResidentReady
→ PrintScreen
→ Freeze all displays
→ Present masked Virtual Desktop
→ Drag selection
→ Lock selection
→ Adjust or reselect
→ Enter Editing
```

## 3. Preconditions

- SnipPlus was manually started and remains resident.
- PrintScreen takeover is enabled.
- No other SnipPlus capture session is active.
- The application active before capture can be identified for later focus restoration.

When takeover is disabled, SnipPlus must not intercept PrintScreen.

## 4. Capture Start

1. Accept the PrintScreen action as one explicit capture request.
2. Record the pre-capture foreground application／window context.
3. Ensure normal SnipPlus windows will not appear in the frozen source.
4. Establish one Session ID.
5. Snapshot the complete display topology、physical bounds、DPI context and Virtual Desktop origin.
6. Acquire one frozen frame for every connected display before selection becomes interactive.
7. If any required frame or coordinate context cannot be established consistently, do not enter Selection.

An in-app capture button may invoke the same workflow as a secondary testable entry, but it must not replace PrintScreen as the product’s primary entry.

## 5. Virtual Desktop Presentation

- All connected displays participate in one logical Frozen Canvas.
- The logical canvas supports negative coordinates and arbitrary monitor arrangement.
- Each display shows its own frozen content aligned to the shared coordinate snapshot.
- A semi-transparent mask covers all selectable display content.
- The pointer becomes a crosshair across the selectable area.
- SnipPlus normal windows are not visible in the frozen source.
- The implementation is not required to allocate one physically contiguous giant bitmap; separate frames are allowed if user-visible behavior remains one canvas.

## 6. Initial Selection

- Pointer press establishes the selection origin.
- Pointer movement defines one rectangular selection in Virtual Desktop coordinates.
- The rectangle may cross one or more display boundaries.
- The selected interior shows original frozen content without the mask.
- The outside area remains dimmed.
- Pointer release locks the selection.
- Pointer release does not render final output、write Clipboard、save a file or end the session.
- A zero-size or invalid rectangle cannot enter `SelectionLocked`.

## 7. Locked Selection Editing

Before Complete or Save, the user can:

- drag inside the selection to move the whole region;
- drag four edges or four corners to resize;
- drag outside the current selection to create a replacement selection;
- use annotation tools;
- choose Complete、Save or Cancel.

Selection geometry remains in Frozen Virtual Desktop coordinates. Adjusting selection does not scale or move annotation objects.

## 8. Function Bar

- A valid locked selection always causes the editing／confirmation function bar to appear.
- The function bar prefers the area below the selection.
- When insufficient space exists, it moves above the selection.
- It must remain visible within an available display work area.
- Annotation actions can be skipped by choosing Complete immediately.

## 9. Cancellation

- Esc before any selection cancels the full session.
- Esc during selection drag cancels the drag and the full session.
- Esc after selection lock or during editing cancels the full session.
- Cancel creates no file and writes no Clipboard.
- Cancel closes all overlays and function bars、releases frozen frames and returns focus to the pre-capture application.
- Cancel never opens the SnipPlus main window.

## 10. Display Changes and Invalid Context

If display topology、DPI mapping or frame dimensions become inconsistent during a session:

- do not silently acquire a replacement source after the user has begun selection;
- do not reuse stale crop bounds;
- classify the session as failed or restart only after an explicit new capture request;
- close capture UI and restore the previous work context on terminal failure.

## 11. Required Acceptance Criteria

| ID | Criterion |
| --- | --- |
| `SPEC-0005-AC-001` | Enabled PrintScreen starts one capture session while disabled takeover does not intercept it. |
| `SPEC-0005-AC-002` | All connected displays are frozen before selection becomes interactive. |
| `SPEC-0005-AC-003` | One rectangular selection can cross display boundaries in Virtual Desktop coordinates. |
| `SPEC-0005-AC-004` | The selection interior shows unmasked frozen content while the outside remains dimmed. |
| `SPEC-0005-AC-005` | Mouse release locks selection and causes no Clipboard or file output. |
| `SPEC-0005-AC-006` | Locked selection supports move、edge／corner resize and reselection. |
| `SPEC-0005-AC-007` | Function bar appears after a valid lock and remains visible above or below the selection. |
| `SPEC-0005-AC-008` | Esc at each capture stage cancels without output and restores the previous application. |
| `SPEC-0005-AC-009` | SnipPlus normal windows are excluded from frozen source content. |
| `SPEC-0005-AC-010` | Display-context mismatch never produces a silently incorrect crop. |

## 12. Open Decisions

Implementation must not guess:

- final representation of non-display gaps in irregular monitor arrangements;
- exact system-tray and main-window close behavior;
- quantitative latency targets.

The prior single-monitor and mouse-release-to-complete workflow is superseded.
