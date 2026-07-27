# SPEC-0005 Capture Workflow

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `SPEC-0005` |
| Feature ID | `FEAT-001` |
| Version | `1.3` |
| Status | `Accepted` |
| Last reviewed | `2026-07-27` |
| Normative sources | `PRD-0004`、`PRD-0005`、`PRD-0006`、`SPEC-0003` |

## 2. Scope

This Spec defines the first-release capture path from PrintScreen through a locked、editable、cross-monitor Selection. It fixes application exit、four-4K capacity、capture-start performance and non-display-gap behavior. Annotation details are owned by `SPEC-0009`.

```text
ResidentReady
→ PrintScreen
→ Validate four-4K support envelope
→ Freeze all supported displays
→ Present masked Virtual Desktop
→ Drag Selection
→ Lock Selection
→ Adjust or reselect by pointer
→ Enter Editing
```

## 3. Preconditions and Exit Boundary

- SnipPlus was manually started and remains resident.
- PrintScreen takeover is enabled.
- No other SnipPlus capture Session is active.
- The pre-capture foreground application can be recorded.
- MainWindow `X` exits SnipPlus、releases takeover and does not hide to tray.
- Any explicit tray Exit action uses the same shutdown path.

When takeover is disabled or the process exits, SnipPlus does not intercept PrintScreen.

## 4. Supported Display Envelope

One v1 capture Session supports:

- `1` through `4` active logical desktop display surfaces;
- each display no larger than `3840 × 2160` physical pixels;
- total active source pixels no greater than `33,177,600`;
- Virtual Desktop bounding width and height each no greater than `16,384` physical pixels;
- final Selection width and height each no greater than `16,384` pixels;
- final Selection area no greater than `67,108,864` pixels;
- transparent topology gaps count toward final Selection area;
- mirrored outputs resolving to one logical surface count once;
- an 8K display is outside v1.

The larger output-area ceiling allows irregular four-4K layouts to include transparent gaps.

Validation rules:

- display count、per-display size、total source pixels and Virtual Desktop bounds are validated before interactive Selection;
- Selection dimensions and area are validated before `SelectionLocked` and final render;
- exceeding any limit is a classified unsupported-capacity failure, not partial success.

## 5. Capture Start

1. Accept PrintScreen as one explicit capture request.
2. Record the pre-capture foreground application context.
3. Ensure SnipPlus windows are excluded from frozen content.
4. Establish one Session ID.
5. Snapshot display topology、physical bounds、DPI context and Virtual Desktop origin.
6. Validate the complete topology against section 4.
7. Acquire one frozen frame for every required display before Selection becomes interactive.
8. Verify frame dimensions、identity and coordinate version.
9. Do not enter Selection if any required frame、coordinate or capacity condition is invalid.

An in-app capture button may use the same workflow as a secondary test entry but cannot replace PrintScreen as the primary product entry.

Performance acceptance:

- Owner Reference／Standard profile: PrintScreen accepted through interactive all-display Selection p95 `≤ 500 ms`;
- Maximum profile: p95 `≤ 1,000 ms`;
- measurement follows `PRD-0006 §3.4` and is a release gate, not a runtime timeout.

Mandatory Owner Reference runtime configuration:

- primary `2560 × 1440`;
- lower `1920 × 1080` at Windows scaling `150%`;
- left `2560 × 1440`.

## 6. Virtual Desktop Presentation

- All supported displays participate in one logical Frozen Canvas.
- The canvas supports negative coordinates and arbitrary arrangements inside the accepted envelope.
- Each display shows its frozen content aligned to the shared coordinate snapshot.
- A semi-transparent mask covers all selectable display content.
- The pointer becomes a crosshair across the selectable area.
- SnipPlus windows are not visible in frozen content.
- One giant bitmap is not required; separate frames are allowed if behavior remains one canvas.
- Physical gaps contain no source pixels.
- Selected gap regions render as transparent pixels (`alpha = 0`).

## 7. Initial Selection

- Pointer press establishes the Selection origin.
- Pointer movement defines one rectangle in Virtual Desktop coordinates.
- The rectangle may cross display boundaries.
- The selected interior shows original frozen content without the mask.
- The outside area remains dimmed.
- Pointer release validates dimensions and locks Selection.
- Pointer release does not render output、write Clipboard、save a file or end the Session.
- A zero-size、invalid or capacity-exceeding rectangle cannot enter `SelectionLocked`.

## 8. Locked Selection Editing

Before Complete or Save, the user can with pointer input:

- drag inside Selection to move the whole region;
- drag four edges or four corners to resize;
- drag outside current Selection to create a replacement Selection;
- use Annotation tools;
- choose Complete、Save or Cancel.

Selection geometry remains in Frozen Virtual Desktop coordinates. Adjusting Selection does not scale or move Annotation objects.

Pointer movement and resize must provide visible response within p95 `100 ms` and maintain p95 interaction frame time `≤ 33 ms` under the supported envelope.

Keyboard-only Selection manipulation is deferred.

## 9. Function Bar

- A valid locked Selection always causes the Editing／confirmation function bar to appear.
- The function bar prefers the area below Selection.
- When space is insufficient, it moves above Selection.
- It remains visible within an available display work area.
- Annotation actions can be skipped by choosing Complete immediately.
- Keyboard-only function-bar navigation and shortcuts are deferred.

## 10. Cancellation and Unsupported Capacity

- Esc before any Selection cancels the full Session.
- Esc during Selection drag cancels the drag and full Session.
- Esc after Selection lock or while the function bar is visible cancels the full Session.
- Cancel creates no file and writes no Clipboard.
- Cancel closes overlays and function bars、releases frozen frames and restores the pre-capture application.
- Cancel never opens MainWindow.

When display configuration exceeds the v1 envelope:

- do not omit or downscale displays;
- do not enter interactive Selection;
- show an actionable supported-limit failure;
- release frames already acquired;
- restore the previous work context;
- return to `ResidentReady`.

## 11. Display Changes and Invalid Context

If topology、DPI mapping or frame dimensions become inconsistent during a Session:

- do not silently acquire replacement source content after Selection begins;
- do not reuse stale crop bounds;
- do not reinterpret unsupported topology as partial support;
- classify the Session as failed or restart only after a new explicit capture request;
- close capture UI and restore previous work context on terminal failure.

## 12. Required Acceptance Criteria

| ID | Criterion |
| --- | --- |
| `SPEC-0005-AC-001` | Enabled PrintScreen starts one capture Session while disabled takeover and process exit do not intercept it. |
| `SPEC-0005-AC-002` | All required displays are validated and frozen before Selection becomes interactive. |
| `SPEC-0005-AC-003` | One rectangular Selection can cross display boundaries in Virtual Desktop coordinates. |
| `SPEC-0005-AC-004` | Selection interior shows unmasked frozen content while outside remains dimmed. |
| `SPEC-0005-AC-005` | Mouse release locks Selection and causes no Clipboard or file output. |
| `SPEC-0005-AC-006` | Locked Selection supports pointer move、edge／corner resize and reselection. |
| `SPEC-0005-AC-007` | Function bar appears after a valid lock and remains visible above or below Selection. |
| `SPEC-0005-AC-008` | Esc at each capture stage cancels without output and restores the previous application. |
| `SPEC-0005-AC-009` | SnipPlus windows are excluded from frozen source content. |
| `SPEC-0005-AC-010` | Display-context mismatch never produces a silently incorrect crop. |
| `SPEC-0005-AC-011` | MainWindow `X` exits SnipPlus and releases PrintScreen takeover. |
| `SPEC-0005-AC-012` | Non-display gaps render as transparent pixels. |
| `SPEC-0005-AC-013` | Every topology inside the four-4K envelope can enter Selection when capture resources are valid. |
| `SPEC-0005-AC-014` | Any topology exceeding one or more limits fails before Selection without partial capture. |
| `SPEC-0005-AC-015` | Owner Reference／Standard and Maximum capture-start scenarios meet p95 `500 ms` and `1,000 ms` targets. |
| `SPEC-0005-AC-016` | Pointer Selection interaction meets p95 `33 ms` frame time and p95 `100 ms` visible-response targets. |
| `SPEC-0005-AC-017` | V1 acceptance does not require keyboard-only Selection or function-bar shortcuts. |

## 13. Finalized Decisions

Quantitative capture targets、four-4K capacity、Owner Reference mixed-DPI verification and pointer-driven Selection scope are accepted. Keyboard-only Selection／Annotation and non-PrintScreen shortcuts are deferred. No product decision blocks this workflow slice.