# Specification Baseline Review

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `SPEC-BASELINE-REVIEW-001` |
| Version | `1.1` |
| Review date | `2026-07-27` |
| Authority | Repository owner through accepted PRD v1.1 |
| Decision | `Baseline Accepted` |
| Scope | `SPEC-0002` through `SPEC-0010` |

This review supersedes the earlier draft review that described Annotation as an optional post-capture capability、left tool behavior undefined、treated file format as unresolved and retained single-monitor／immediate-Clipboard assumptions.

## 2. Reviewed Specifications

| Document | Current responsibility | Result |
| --- | --- | --- |
| `SPEC-0002` | Specification structure、traceability and acceptance rules | `PASS` |
| `SPEC-0003` | Shared v1 state、session and system invariants | `PASS` |
| `SPEC-0004` | Accepted v1 Feature catalog and deferred boundaries | `PASS` |
| `SPEC-0005` | Resident PrintScreen、all-display freeze and cross-monitor selection | `PASS` |
| `SPEC-0006` | Success、cancel、recoverable／terminal failure、cleanup and focus | `PASS` |
| `SPEC-0007` | Clipboard commitment after Complete or successful Save | `PASS` |
| `SPEC-0008` | PNG Save As and dual PNG／Clipboard completion | `PASS` |
| `SPEC-0009` | Mandatory editing stage、required annotation tools and history | `PASS` |
| `SPEC-0010` | Integrated cross-feature workflow and ownership | `PASS` |

## 3. Baseline Consistency

The accepted specification set consistently requires:

- manual startup and resident lifecycle;
- user-controlled PrintScreen takeover;
- one session-owned Frozen Virtual Desktop covering all connected displays;
- one rectangular selection that can span displays;
- clear selection interior and dimmed exterior;
- mouse release to `SelectionLocked`, never directly to output;
- move、edge／corner resize and reselection before commitment;
- mandatory editing／confirmation with optional annotation actions;
- Rectangle、Arrow／Line、Highlighter、Text、Mosaic／Blur and Numbered Marker;
- annotation object editing、Undo／Redo、Virtual Desktop anchoring and output clipping;
- Complete to Clipboard only;
- Save to PNG and the same Clipboard result;
- recoverable output failure returning to Editing with state preserved;
- Cancel with no output、capture-UI cleanup and focus restoration;
- stale session／revision outcomes unable to advance workflow state.

No current Spec contains a valid path from mouse release directly to Clipboard.

## 4. Feature and Ownership Review

| Feature | Normative owner | Required v1 boundary |
| --- | --- | --- |
| `FEAT-001` | `SPEC-0005` | Resident entry、display freeze、Virtual Desktop and selection lifecycle |
| `FEAT-002` | `SPEC-0009` | Editing function bar、annotation document、tools and Undo／Redo |
| `FEAT-003` | `SPEC-0007` | Clipboard publication after explicit commitment |
| `FEAT-004` | `SPEC-0008` | Save As、PNG creation and file-delivery outcome |
| `FEAT-005` | `SPEC-0006` | Cancel、failure preservation、cleanup、feedback and focus restoration |
| Shared integration | `SPEC-0010` | Single session／revision context and legal feature order |

Clipboard and PNG Output remain separate capability boundaries. They are coordinated by the Save workflow, which requires both to succeed before Save is complete.

## 5. Acceptance Readiness

| Area | Result |
| --- | --- |
| User-visible workflow | `PASS` |
| State and commitment boundaries | `PASS` |
| Multi-display and coordinate requirements | `PASS` |
| Annotation tool and object requirements | `PASS` |
| Clipboard and PNG output behavior | `PASS` |
| Cancellation and recoverable failure behavior | `PASS` |
| Privacy and external-GUI evidence boundaries | `PASS` |
| Stable acceptance-criterion namespaces | `PASS` |
| Current code conformance | `FAIL — tracked separately in PRD-TRACEABILITY-MATRIX-001` |

Specification acceptance does not imply implementation completion.

## 6. Remaining Explicit Decisions

Implementation must not guess:

- representation of non-display gaps in irregular monitor layouts;
- exact System Tray menu and MainWindow close-button behavior;
- retention／rollback when PNG succeeds but Clipboard fails;
- final keyboard-only annotation acceptance standard;
- quantitative performance targets.

## 7. Baseline Decision

`SPEC-0002` through `SPEC-0010` form the accepted SnipPlus v1 Specification baseline.

Architecture、code、tests and runtime verification must conform to this baseline. Historical Research and prior draft reviews are non-normative where they conflict. Implementation gaps are recorded in the existing requirements-to-code conformance matrix; they do not justify creating another readiness or closure-document chain.
