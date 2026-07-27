# PRD Freeze Review

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `PRD-FREEZE-REVIEW-001` |
| Version | `1.4` |
| Review date | `2026-07-27` |
| Product authority | Repository owner through explicit product decisions |
| Decision | `Freeze Approved` |
| Scope | SnipPlus v1 first release |

This review supersedes earlier wording that treated performance、capacity or keyboard-only Editing as unresolved or accepted a broader 8K／keyboard-only scope.

## 2. Normative Inputs

- [PRD-0002 User Experience Principles](PRD-0002-user-experience-principles.md)
- [PRD-0003 Product Vision](PRD-0003-product-vision.md)
- [PRD-0004 Core Workflow](PRD-0004-core-workflow.md)
- [PRD-0005 Functional Requirements](PRD-0005-functional-requirements.md) — Accepted v1.3
- [PRD-0006 Non-functional Requirements](PRD-0006-non-functional-requirements.md) — Accepted v1.4

The [Requirements-to-Code Conformance Matrix](PRD-TRACEABILITY-MATRIX.md) tracks implementation evidence and does not redefine product scope.

## 3. Freeze Checklist

| Review area | Result | Basis |
| --- | --- | --- |
| Primary entry、residency and exit | `PASS` | Manual startup、PrintScreen setting、direct MainWindow exit and takeover release are defined. |
| Capture source and display scope | `PASS` | All-display Frozen Virtual Desktop and cross-monitor Selection are required. |
| Capacity envelope | `PASS` | Up to four 4K displays、source-pixel、Virtual Desktop and output-allocation limits are fixed. |
| Owner Reference environment | `PASS` | 2K primary、FHD lower at 150% scaling and left 2K verification profile is fixed. |
| Performance and memory | `PASS` | p95 capture、interaction、output、progress、working-set and cleanup targets are fixed. |
| Irregular display gaps | `PASS` | Final output uses transparent pixels for physical non-display gaps. |
| Selection lifecycle | `PASS` | Initial drag、lock、pointer move、edge／corner resize、reselection and Esc cancellation are defined. |
| Editing／confirmation | `PASS` | Function bar is mandatory after a valid lock; Annotation actions are optional. |
| Annotation tools | `PASS` | Required pointer-driven tools、shared controls、object editing and Undo／Redo are defined. |
| Keyboard boundary | `PASS` | PrintScreen and Esc are required; keyboard-only Annotation and non-PrintScreen shortcuts are deferred. |
| Coordinate and clipping behavior | `PASS` | Annotation geometry uses Frozen Virtual Desktop coordinates and output clipping is defined. |
| Complete and Clipboard | `PASS` | Complete writes Clipboard only after explicit commitment. |
| Save and file output | `PASS` | Save As、Downloads、PNG、timestamp filename、Clipboard coupling and retained PNG behavior are defined. |
| Cancel、failure、progress and focus | `PASS` | Cancellation、progress、failure preservation、cleanup and focus restoration are defined. |
| Deferred capability boundary | `PASS` | Non-v1 capabilities are explicitly listed. |

## 4. Accepted v1 Product Baseline

```text
Manual startup and residency
→ user-controlled PrintScreen takeover
→ validate four-4K support envelope
→ PrintScreen freezes all connected supported displays
→ cross-monitor rectangular Selection on one Frozen Virtual Desktop
→ physical display gaps map to transparent output pixels
→ mouse release locks Selection
→ Editing／confirmation function bar
→ optional pointer-driven Annotation actions
→ Complete OR Save OR Cancel
```

### Performance

- Capture start p95 `≤ 500 ms` Owner Reference／Standard、`≤ 1,000 ms` Maximum.
- Pointer interaction p95 frame time `≤ 33 ms`; visible response p95 `≤ 100 ms`.
- Complete and Save meet the accepted output-size tiers.
- Progress appears after `300 ms` for a still-running commit.
- Memory meets idle、peak、cleanup and repeated-session targets in PRD-0006.
- Measurement uses `3` warm-up runs and at least `30` measured runs with p50、p95 and maximum reporting.

### Capacity

- `1`–`4` active logical display surfaces.
- Each display `≤ 3840 × 2160`.
- Total source pixels `≤ 33,177,600`.
- Virtual Desktop width／height each `≤ 16,384`.
- Selection width／height each `≤ 16,384`; area `≤ 67,108,864` pixels.
- Transparent gaps count toward Selection area.
- An 8K display is outside v1.
- Unsupported capacity fails before Selection without partial capture.

### Owner Reference Configuration

- primary `2560 × 1440`;
- lower `1920 × 1080` at Windows scaling `150%`;
- left `2560 × 1440`.

This configuration is mandatory for mixed-DPI runtime acceptance.

### Complete

- Render current Selection and Annotation revision.
- Use transparent pixels for physical display gaps.
- Write Clipboard.
- End only after Clipboard succeeds.
- Restore the previous application and show no success notification.

### Save

- Open Windows Save As in Downloads by default.
- Propose `SnipPlus_yyyy-MM-dd_HHmmss.png` and allow another destination／name.
- Save PNG and write the same result to Clipboard.
- End only after both succeed.
- If Clipboard fails after PNG success, retain the PNG and return to Editing.

### Cancel and Keys

- PrintScreen is the required global capture key.
- Esc cancels before Selection、during drag and during stable Editing.
- Cancel creates no file、writes no Clipboard、closes capture UI and restores the previous context.
- No other keyboard shortcut or keyboard-only Annotation workflow is required in v1.

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
- Keyboard-only Annotation workflow and non-PrintScreen tool／action shortcuts.
- HDR preservation、ARM64、cloud、sharing、plugins、telemetry、updates and release publication.

## 6. Product Decision Status

No visible v1 product or quality decision remains open. Implementation difficulty or benchmark failure does not authorize silent relaxation of accepted targets、four-4K limits or deferred keyboard scope.

## 7. Freeze Decision

`Freeze Approved for the complete SnipPlus v1 first-release product and quality baseline.`

Specs、Architecture、code and tests must conform to this accepted baseline. Existing implementation and historical review documents do not override it.