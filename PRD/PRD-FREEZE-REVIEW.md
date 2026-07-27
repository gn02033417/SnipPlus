# PRD Freeze Review

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `PRD-FREEZE-REVIEW-001` |
| Version | `1.3` |
| Review date | `2026-07-27` |
| Product authority | Repository owner through explicit product decisions |
| Decision | `Freeze Approved` |
| Scope | SnipPlus v1 first release |

This review supersedes earlier wording that treated PrintScreen、multi-display、Annotation、file output、MainWindow close behavior、display gaps、PNG rollback、performance、capacity or keyboard-only Editing as unresolved.

## 2. Normative Inputs

- [PRD-0002 User Experience Principles](PRD-0002-user-experience-principles.md)
- [PRD-0003 Product Vision](PRD-0003-product-vision.md)
- [PRD-0004 Core Workflow](PRD-0004-core-workflow.md)
- [PRD-0005 Functional Requirements](PRD-0005-functional-requirements.md)
- [PRD-0006 Non-functional Requirements](PRD-0006-non-functional-requirements.md) — Accepted v1.3

The [Requirements-to-Code Conformance Matrix](PRD-TRACEABILITY-MATRIX.md) is implementation evidence and gap tracking; it does not redefine product scope.

## 3. Freeze Checklist

| Review area | Result | Basis |
| --- | --- | --- |
| Primary entry、residency and exit | `PASS` | Manual startup、PrintScreen setting、direct MainWindow exit and takeover release are defined. |
| Capture source and display scope | `PASS` | All-display Frozen Virtual Desktop and cross-monitor Selection are required. |
| Capacity envelope | `PASS` | Display count、per-display size、total source pixels、Virtual Desktop and Selection limits are fixed. |
| Performance and memory | `PASS` | p95 capture、interaction、output、progress、working-set and cleanup targets are fixed. |
| Irregular display gaps | `PASS` | Final output uses transparent pixels for physical non-display gaps. |
| Selection lifecycle | `PASS` | Initial drag、lock、move、edge／corner resize、reselection and Esc behavior are defined. |
| Editing／confirmation | `PASS` | Function bar is mandatory after a valid lock; Annotation actions are optional. |
| Annotation tools | `PASS` | Required v1 tools、shared controls、object editing and Undo／Redo are defined. |
| Keyboard-only Editing | `PASS` | Scope、focus model、commands、object creation、IME and acceptance procedure are defined. |
| Coordinate and clipping behavior | `PASS` | Annotation geometry uses Frozen Virtual Desktop coordinates and output clipping is defined. |
| Complete and Clipboard | `PASS` | Complete writes Clipboard only after explicit commitment. |
| Save and file output | `PASS` | Save As、Downloads initial folder、PNG、timestamp filename、Clipboard coupling and retained PNG behavior are defined. |
| Cancel、failure、progress and focus | `PASS` | Transient Esc、stable cancel、progress、failure preservation、cleanup and focus restoration are defined. |
| Deferred capability boundary | `PASS` | Non-v1 capabilities are explicitly listed. |

## 4. Accepted v1 Product Baseline

```text
Manual startup and residency
→ optional PrintScreen takeover enabled by the user
→ validate supported display envelope
→ PrintScreen freezes all connected displays
→ cross-monitor rectangular Selection on one Frozen Virtual Desktop
→ physical display gaps map to transparent output pixels
→ mouse release locks Selection
→ Editing／confirmation function bar
→ optional Annotation actions by pointer or keyboard
→ Complete OR Save OR Cancel
```

### Performance

- Capture start p95 `≤ 500 ms` Standard、`≤ 1,000 ms` Maximum.
- Interaction p95 frame time `≤ 33 ms`; discrete response p95 `≤ 100 ms`.
- Complete and Save meet the accepted output-size tiers.
- Progress appears after `300 ms` for a still-running commit.
- Memory meets idle、peak、cleanup and repeated-session targets in PRD-0006.

### Capacity

- `1`–`4` active logical display surfaces.
- Each `≤ 7,680 × 4,320`.
- Total source pixels `≤ 66,355,200`.
- Virtual Desktop width／height each `≤ 16,384`.
- Selection width／height each `≤ 16,384`; area `≤ 67,108,864` pixels.
- Unsupported capacity fails before Selection without partial capture.

### Complete

- Render current Selection and Annotation revision.
- Use transparent pixels for physical non-display gaps.
- Write Clipboard.
- End only after Clipboard succeeds.
- Close capture UI、restore the previous application and show no success notification.

### Save

- Open Windows Save As in Downloads by default.
- Propose `SnipPlus_yyyy-MM-dd_HHmmss.png` and allow another destination／name.
- Save PNG.
- Write the same rendered result to Clipboard.
- End only after PNG and Clipboard succeed.
- If Clipboard fails after PNG success, retain the PNG and return to Editing with an actionable Clipboard error.

### Cancel and Esc

- Esc before Selection and during drag cancels the session.
- First Esc closes transient Editing state; Esc from stable Editing cancels the session.
- Cancel creates no file、writes no Clipboard、closes capture UI and restores the previous work context.

### Keyboard-only Editing

- Acceptance begins at `SelectionLocked`; initial crosshair Selection remains pointer-driven.
- Every required tool、object operation、style、Undo／Redo、Save、Complete and Cancel operation works without pointer input.
- Visible focus、High Contrast、200% scaling、Narrator state and Chinese IME are included.

### Application Exit

- MainWindow `X` directly exits SnipPlus.
- Exit releases PrintScreen takeover.
- No close-to-tray or hidden resident process remains.

## 5. Explicitly Deferred

- Opaque freehand pen.
- Ellipse Annotation.
- Pin image to desktop.
- OCR.
- Capture history.
- Delayed capture.
- Additional save formats.
- Font-family selection、italic、underline and text background.
- HDR preservation、ARM64、cloud、sharing、plugins、telemetry、updates and release publication.

## 6. Product Decision Status

No visible v1 product or quality decision remains open. Implementation difficulty or initial benchmark failure does not authorize silent relaxation of accepted targets or limits.

## 7. Freeze Decision

`Freeze Approved for the complete SnipPlus v1 first-release product and quality baseline.`

Specs、Architecture、code and tests must conform to this accepted baseline. Existing implementation and historical review documents do not override it. Product-visible changes require explicit Repository owner direction and updates to existing canonical documents; they do not require a new prerequisite／closure document family.