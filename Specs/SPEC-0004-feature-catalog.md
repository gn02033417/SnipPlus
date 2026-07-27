# SPEC-0004 Feature Catalog

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `SPEC-0004` |
| Version | `1.1` |
| Status | `Accepted` |
| Last reviewed | `2026-07-27` |
| Sources | `PRD-0004`、`PRD-0005`、`PRD-0006`、`SPEC-0003` |

## 2. Accepted v1 Features

| Feature ID | Name | Priority | Status | Scope |
| --- | --- | --- | --- | --- |
| `FEAT-001` | Resident Capture and Multi-display Selection | `Must` | `Accepted` | Manual startup、resident mode、PrintScreen takeover、all-display freeze、cross-monitor rectangular selection、selection move／resize／reselect. |
| `FEAT-002` | Editing and Annotation | `Must` | `Accepted` | Mandatory confirmation stage with optional annotation actions; rectangle、arrow／line、highlighter、text、mosaic／blur、numbered marker、color、thickness、Undo／Redo and object editing. |
| `FEAT-003` | Clipboard Handoff | `Must` | `Accepted` | Complete and successful Save publish the rendered final image to Clipboard; failure retains editing state. |
| `FEAT-004` | PNG File Output | `Must` | `Accepted` | Save As、PNG-only first release、timestamp filename proposal、save plus Clipboard completion. |
| `FEAT-005` | Workflow Boundaries and Recovery | `Must` | `Accepted` | Esc cancellation、failure preservation、cleanup、focus restoration and silent success. |

## 3. Required Cross-feature Rules

- PrintScreen is the primary v1 entry when takeover is enabled.
- Single-monitor selection is not the first-release product boundary.
- Mouse release locks a selection; it does not complete capture.
- Editing／confirmation is mandatory; annotation actions are optional.
- Clipboard delivery happens only after Complete or successful Save commitment.
- File Output and Clipboard are separate operations within the Save path, but both must succeed before Save is reported complete.
- All features share one capture Session ID and one Frozen Virtual Desktop coordinate snapshot.

## 4. Deferred Capabilities

The following are not independent v1 Features and must not be pulled into implementation without a later product decision:

- Opaque freehand pen.
- Ellipse annotation.
- Pin image to desktop.
- OCR.
- Capture history.
- Delayed capture.
- Additional image formats.
- Cloud、sharing、plugins、telemetry or update system.

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
| `SPEC-0004-AC-001` | Every accepted v1 capability maps to one existing Feature and one normative Spec. |
| `SPEC-0004-AC-002` | Annotation is a Must feature while individual annotation actions remain optional. |
| `SPEC-0004-AC-003` | Multi-display selection and PrintScreen takeover are included rather than deferred. |
| `SPEC-0004-AC-004` | Deferred capabilities are explicitly excluded from first-release implementation. |

The previous Candidate catalog、placeholder filenames and statement that Toolbar／annotation tools lacked product support are superseded.
