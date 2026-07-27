# SPEC-0005 Capture Workflow

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `SPEC-0005` |
| Feature ID | `FEAT-001` |
| Version | `1.2` |
| Status | `Accepted` |
| Last reviewed | `2026-07-27` |
| Normative sources | `PRD-0004`、`PRD-0005`、`PRD-0006`、`SPEC-0003` |

## 2. Scope

This Spec defines the first-release capture path from PrintScreen through a locked、editable、cross-monitor Selection. It fixes application exit、supported display capacity、capture-start performance and non-display-gap behavior. It does not define annotation-rendering details or output-adapter internals.

```text
ResidentReady
→ PrintScreen
→ Validate supported display envelope
→ Freeze all displays
→ Present masked Virtual Desktop
→ Drag Selection
→ Lock Selection
→ Adjust or reselect
→ Enter Editing
```

## 3. Preconditions and Exit Boundary

- SnipPlus was manually started and remains resident.
- PrintScreen takeover is enabled.
- No other SnipPlus capture session is active.
- The application active before capture can be identified for later focus restoration.
- Closing MainWindow with `X` exits SnipPlus、releases takeover and does not hide the process to the System Tray.
- If a System Tray surface is present, its explicit Exit action uses the same shutdown path.

When takeover is disabled or the process exits, SnipPlus must not intercept PrintScreen.

## 4. Supported Display Envelope

One v1 capture session supports:

- `1` through `4` active logical desktop display surfaces;
- each display no larger than `7,680 × 4,320` physical pixels;
- total active source pixels no greater than `66,355,200`;
- Virtual Desktop bounding width and height each no greater than `16,384` physical pixels;
- final Selection width and height each no greater than `16,384` pixels;
- final Selection area no greater than `67,108,864` pixels.

Rules:

- Mirrored outputs resolving to one logical desktop surface count once.
- Transparent topology gaps count toward final Selection area.
- Display count、per-display size、total source pixels and Virtual Desktop bounds are validated before interactive Selection.
- Selection size and area are validated before `SelectionLocked` and again before final render.
- Exceeding any limit is a classified unsupported-capacity failure, not a partial-success condition.

## 5. Capture Start

1. Accept the PrintScreen action as one explicit capture request.
2. Record the pre-capture foreground application／window context.
3. Ensure normal SnipPlus windows will not appear in the frozen source.
4. Establish one Session ID.
5. Snapshot the complete display topology、physical bounds、DPI context and Virtual Desktop origin.
6. Validate the complete topology against section 4.
7. Acquire one frozen frame for every required display before Selection becomes interactive.
8. Verify frame dimensions、identity and coordinate version against the accepted snapshot.
9. If any required frame、coordinate context or capacity condition is invalid, do not enter Selection.

An in-app capture button may invoke the same workflow as a secondary testable entry, but it must not replace PrintScreen as the product’s primary entry.

Performance acceptance:

- Standard profile: PrintScreen accepted through interactive all-display Selection p95 `≤ 500 ms`.
- Maximum profile: p95 `≤ 1,000 ms`.
- Measurement follows `PRD-0006 §3.4`; these are release gates rather than per-session timeout values.

## 6. Virtual Desktop Presentation

- All connected displays participate in one logical Frozen Canvas.
- The logical canvas supports negative coordinates and arbitrary monitor arrangement inside the accepted envelope.
- Each display shows its own frozen content aligned to the shared coordinate snapshot.
- A semi-transparent mask covers all selectable display content.
- The pointer becomes a crosshair across the selectable area.
- SnipPlus normal windows are not visible in the frozen source.
- The implementation is not required to allocate one physically contiguous giant bitmap; separate frames are allowed if user-visible behavior remains one canvas.
- Physical gaps between irregularly arranged displays contain no captured source pixels.
- When a Selection includes such a gap, corresponding final-image pixels are transparent (`alpha = 0`).

## 7. Initial Selection

- Pointer press establishes the Selection origin.
- Pointer movement defines one rectangular Selection in Virtual Desktop coordinates.
- The rectangle may cross one or more display boundaries.
- The selected interior shows original frozen content without the mask.
- The outside area remains dimmed.
- Pointer release validates the final dimensions and locks the Selection.
- Pointer release does not render final output、write Clipboard、save a file or end the session.
- A zero-size、invalid or capacity-exceeding rectangle cannot enter `SelectionLocked`.

## 8. Locked Selection Editing

Before Complete or Save, the user can:

- drag inside the Selection to move the whole region;
- drag four edges or four corners to resize;
- drag outside the current Selection to create a replacement Selection;
- use the complete keyboard move／resize model defined by `SPEC-0009`;
- use Annotation tools;
- choose Complete、Save or Cancel.

Selection geometry remains in Frozen Virtual Desktop coordinates. Adjusting Selection does not scale or move Annotation objects.

Selection and keyboard movement must provide visible response within p95 `100 ms` and maintain p95 interaction frame time `≤ 33 ms` under the supported envelope.

## 9. Function Bar

- A valid locked Selection always causes the Editing／confirmation function bar to appear.
- The function bar prefers the area below the Selection.
- When insufficient space exists, it moves above the Selection.
- It must remain visible within an available display work area.
- Annotation actions can be skipped by choosing Complete immediately.
- F6、Tab and Shift+Tab navigation are defined by `SPEC-0009`.

## 10. Cancellation and Unsupported Capacity

- Esc before any Selection cancels the full session.
- Esc during Selection drag cancels the drag and the full session.
- Esc from stable Editing cancels the full session; transient Editing state consumes the first Esc according to `SPEC-0009`.
- Cancel creates no file and writes no Clipboard.
- Cancel closes all overlays and function bars、releases frozen frames and returns focus to the pre-capture application.
- Cancel never opens the SnipPlus main window.

When the display configuration exceeds the v1 envelope:

- do not omit or downscale displays;
- do not enter interactive Selection;
- show an actionable supported-limit failure;
- release any frames already acquired;
- restore the previous work context;
- return to `ResidentReady` so a later request can succeed after configuration changes.

## 11. Display Changes and Invalid Context

If display topology、DPI mapping or frame dimensions become inconsistent during a session:

- do not silently acquire a replacement source after the user has begun Selection;
- do not reuse stale crop bounds;
- do not reinterpret a capacity-exceeding topology as partial support;
- classify the session as failed or restart only after an explicit new capture request;
- close capture UI and restore the previous work context on terminal failure.

## 12. Required Acceptance Criteria

| ID | Criterion |
| --- | --- |
| `SPEC-0005-AC-001` | Enabled PrintScreen starts one capture session while disabled takeover and process exit do not intercept it. |
| `SPEC-0005-AC-002` | All required displays are validated and frozen before Selection becomes interactive. |
| `SPEC-0005-AC-003` | One rectangular Selection can cross display boundaries in Virtual Desktop coordinates. |
| `SPEC-0005-AC-004` | The Selection interior shows unmasked frozen content while the outside remains dimmed. |
| `SPEC-0005-AC-005` | Mouse release locks Selection and causes no Clipboard or file output. |
| `SPEC-0005-AC-006` | Locked Selection supports move、edge／corner resize and reselection. |
| `SPEC-0005-AC-007` | Function bar appears after a valid lock and remains visible above or below the Selection. |
| `SPEC-0005-AC-008` | Esc at each stable capture stage cancels without output and restores the previous application. |
| `SPEC-0005-AC-009` | SnipPlus normal windows are excluded from frozen source content. |
| `SPEC-0005-AC-010` | Display-context mismatch never produces a silently incorrect crop. |
| `SPEC-0005-AC-011` | Closing MainWindow exits SnipPlus rather than hiding it to tray and releases PrintScreen takeover. |
| `SPEC-0005-AC-012` | Non-display gaps included by a cross-monitor Selection render as transparent pixels. |
| `SPEC-0005-AC-013` | Every topology inside the accepted display envelope can enter Selection when capture resources are valid. |
| `SPEC-0005-AC-014` | Any topology exceeding one or more limits fails before Selection without partial capture. |
| `SPEC-0005-AC-015` | Standard and Maximum capture-start scenarios meet p95 `500 ms` and `1,000 ms` targets. |
| `SPEC-0005-AC-016` | Selection interaction meets p95 `33 ms` frame time and p95 `100 ms` visible-response targets. |

## 13. Finalized Decisions

Quantitative capture-start targets、display-count、Virtual Desktop size、Selection output limits and keyboard Editing scope are accepted. No remaining product decision blocks this workflow slice.

The prior single-monitor、mouse-release-to-complete、close-to-tray、undefined-capacity and unquantified capture workflow is superseded.