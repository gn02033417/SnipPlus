# PRD Freeze Review

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `PRD-FREEZE-REVIEW-001` |
| Version | `1.1` |
| Review date | `2026-07-27` |
| Product authority | Repository owner through explicit product decisions |
| Decision | `Freeze Approved` |
| Scope | SnipPlus v1 first release |

This review supersedes the earlier v1.0 review that covered only `FR-001`–`FR-011` and treated PrintScreen、multi-display、Annotation and file output as unresolved or deferred.

## 2. Normative Inputs

- [PRD-0002 User Experience Principles](PRD-0002-user-experience-principles.md)
- [PRD-0003 Product Vision](PRD-0003-product-vision.md)
- [PRD-0004 Core Workflow](PRD-0004-core-workflow.md) — Accepted v1.1
- [PRD-0005 Functional Requirements](PRD-0005-functional-requirements.md) — Accepted v1.1
- [PRD-0006 Non-functional Requirements](PRD-0006-non-functional-requirements.md) — Accepted v1.1

The [Requirements-to-Code Conformance Matrix](PRD-TRACEABILITY-MATRIX.md) is implementation evidence and gap tracking; it does not redefine product scope.

## 3. Freeze Checklist

| Review area | Result | Basis |
| --- | --- | --- |
| Primary entry and residency | `PASS` | Manual startup、background residency and user-controlled PrintScreen takeover are defined. |
| Capture source and display scope | `PASS` | All-display frozen Virtual Desktop and cross-monitor rectangular selection are required. |
| Selection lifecycle | `PASS` | Initial drag、lock、move、edge／corner resize、reselection and Esc cancellation are defined. |
| Editing／confirmation | `PASS` | Function bar is mandatory after a valid lock; annotation actions are optional. |
| Annotation tools | `PASS` | Required v1 tools、shared controls、object editing and Undo／Redo are defined. |
| Coordinate and clipping behavior | `PASS` | Annotation geometry uses Frozen Virtual Desktop coordinates and output clipping is defined. |
| Complete and Clipboard | `PASS` | Complete writes Clipboard only after explicit commitment. |
| Save and file output | `PASS` | Save As、PNG、timestamp filename and Clipboard coupling are defined. |
| Cancel、failure and focus | `PASS` | No-output cancellation、recoverable failure preservation、cleanup and focus restoration are defined. |
| Deferred capability boundary | `PASS` | Non-v1 capabilities are explicitly listed. |
| Traceability IDs | `PASS` | `FR-001`–`FR-045`、`FR-D01`–`FR-D08` and `NFR-001`–`NFR-039` provide stable identifiers. |

## 4. Accepted v1 Product Baseline

```text
Manual startup and residency
→ optional PrintScreen takeover enabled by the user
→ PrintScreen freezes all connected displays
→ cross-monitor rectangular selection on one Frozen Virtual Desktop
→ mouse release locks selection
→ editing／confirmation function bar
→ optional annotation actions
→ Complete OR Save OR Cancel
```

### Complete

- Render current selection and annotation revision.
- Write Clipboard.
- End only after Clipboard succeeds.
- Close capture UI、restore the previous application and show no success notification.

### Save

- Open Windows Save As.
- Save PNG using the proposed `SnipPlus_yyyy-MM-dd_HHmmss.png` filename.
- Write the same rendered result to Clipboard.
- End only after PNG and Clipboard succeed.

### Cancel

- Create no file and write no Clipboard.
- Close capture UI and restore the previous work context.
- Do not automatically show the SnipPlus main window.

## 5. Explicitly Deferred

- Opaque freehand pen.
- Ellipse annotation.
- Pin image to desktop.
- OCR.
- Capture history.
- Delayed capture.
- Additional save formats.
- Font-family selection、italic、underline and text background.
- HDR preservation、ARM64、cloud、sharing、plugins、telemetry、updates and release publication.

## 6. Remaining Open Product Decisions

These do not invalidate the accepted v1 workflow, but implementation must stop before selecting visible behavior:

- Representation of non-display gaps in irregular monitor layouts.
- Exact System Tray menu and MainWindow close-button behavior.
- Retention／rollback when PNG creation succeeds but Clipboard publication fails.
- Final keyboard-only annotation acceptance standard.
- Quantitative performance targets after measurement.

## 7. Freeze Decision

`Freeze Approved for SnipPlus v1 first-release product behavior.`

Specs、Architecture、code and tests must conform to this accepted baseline. Existing implementation and historical review documents do not override it. Product-visible changes require explicit Repository owner direction and updates to the existing canonical documents; they do not require a new prerequisite／closure document family.
