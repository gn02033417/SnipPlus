# SPEC-0004 Feature Catalog

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `SPEC-0004` |
| Version | `1.2` |
| Status | `Accepted` |
| Last reviewed | `2026-07-27` |
| Sources | `PRD-0004`、`PRD-0005`、`PRD-0006`、`SPEC-0003` |

## 2. Accepted v1 Features

| Feature ID | Name | Priority | Status | Scope |
| --- | --- | --- | --- | --- |
| `FEAT-001` | Resident Capture and Multi-display Selection | `Must` | `Accepted` | Manual startup、PrintScreen takeover、four-4K capacity validation、all-display freeze、cross-monitor rectangular Selection、pointer move／resize／reselect. |
| `FEAT-002` | Editing and Annotation | `Must` | `Accepted` | Mandatory confirmation stage with optional pointer-driven Annotation actions; Rectangle、Arrow／Line、Highlighter、Text、Mosaic／Blur、Numbered Marker、color、thickness、function-bar Undo／Redo and object editing. |
| `FEAT-003` | Clipboard Handoff | `Must` | `Accepted` | Complete and successful Save publish the rendered final image to Clipboard; failure retains Editing state. |
| `FEAT-004` | PNG File Output | `Must` | `Accepted` | Save As、PNG-only first release、timestamp filename proposal、save plus Clipboard completion and retained PNG partial outcome. |
| `FEAT-005` | Workflow Boundaries and Recovery | `Must` | `Accepted` | Esc cancellation、capacity／output failure preservation、progress、cleanup、focus restoration and silent success. |

## 3. Required Cross-feature Rules

- PrintScreen is the primary v1 entry when takeover is enabled.
- The supported source envelope is up to four displays、each no larger than `3840 × 2160`.
- Unsupported topology never produces partial capture.
- Single-monitor Selection is not the first-release product boundary.
- Mouse release locks Selection; it does not complete capture.
- Editing／confirmation is mandatory; Annotation actions are optional.
- Selection adjustment and Annotation are pointer-driven in v1.
- Clipboard delivery happens only after Complete or successful Save commitment.
- File Output and Clipboard are separate operations within Save, but both must succeed before Save is reported complete.
- All features share one capture Session ID and one Frozen Virtual Desktop coordinate snapshot.
- PrintScreen entry and Esc cancellation are required; other keyboard shortcuts are deferred.

## 4. Deferred Capabilities

The following must not be pulled into v1 implementation without a later product decision:

- Opaque freehand pen.
- Ellipse Annotation.
- Pin image to desktop.
- OCR.
- Capture history.
- Delayed capture.
- Additional image formats.
- Font-family selection、italic、underline and text background.
- Keyboard-only Annotation workflow.
- F6／Tab zone and object traversal as a complete workflow.
- Single-letter tool shortcuts.
- Ctrl-based Undo／Redo、Save or Complete shortcuts.
- Delete and Arrow-key object manipulation.
- Keyboard-created Annotation objects.
- HDR preservation、8K-display support、ARM64、cloud、sharing、plugins、telemetry or update system.

## 5. Feature Specifications

| Feature | Normative Spec |
| --- | --- |
| `FEAT-001` | `SPEC-0005 Capture Workflow` |
| `FEAT-002` | `SPEC-0009 Annotation Capability` |
| `FEAT-003` | `SPEC-0007 Clipboard Handoff` |
| `FEAT-004` | `SPEC-0008 Capture Output` |
| `FEAT-005` | `SPEC-0006 Workflow Boundaries and Feedback` |
| Cross-feature behavior | `SPEC-0010 Feature Integration` |

## 6. Acceptance Criteria

| ID | Criterion |
| --- | --- |
| `SPEC-0004-AC-001` | Every accepted v1 capability maps to one Feature and one normative Spec. |
| `SPEC-0004-AC-002` | Annotation is a Must feature while individual Annotation actions remain optional. |
| `SPEC-0004-AC-003` | Multi-display Selection and PrintScreen takeover are included rather than deferred. |
| `SPEC-0004-AC-004` | The maximum source topology is four 4K displays; 8K displays are deferred. |
| `SPEC-0004-AC-005` | Keyboard-only Annotation and non-PrintScreen tool／action shortcuts are deferred. |
| `SPEC-0004-AC-006` | Deferred capabilities are excluded from first-release implementation. |

The previous feature catalog without explicit capacity and keyboard boundaries is superseded.