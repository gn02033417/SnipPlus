# Specification Baseline Review

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `SPEC-BASELINE-REVIEW-001` |
| Version | `1.2` |
| Review date | `2026-07-27` |
| Authority | Repository owner through accepted PRD v1.2 |
| Decision | `Baseline Accepted` |
| Scope | `SPEC-0002` through `SPEC-0010` |

This review supersedes earlier wording that described Annotation as optional post-capture work、left tools or output undefined、retained single-monitor／immediate-Clipboard assumptions or treated exit、gap and PNG retention behavior as unresolved.

## 2. Reviewed Specifications

| Document | Current responsibility | Result |
| --- | --- | --- |
| `SPEC-0002` | Specification structure、traceability and acceptance rules | `PASS` |
| `SPEC-0003` | Shared v1 state、session、exit and system invariants | `PASS` |
| `SPEC-0004` | Accepted v1 Feature catalog and deferred boundaries | `PASS` |
| `SPEC-0005` | Resident PrintScreen、direct exit、all-display freeze、cross-monitor Selection and gap behavior | `PASS` |
| `SPEC-0006` | Success、cancel、partial Save failure、cleanup、exit and focus | `PASS` |
| `SPEC-0007` | Clipboard commitment and retained-file relationship | `PASS` |
| `SPEC-0008` | Downloads-default PNG Save As、transparent output and retained PNG | `PASS` |
| `SPEC-0009` | Mandatory Editing、required Annotation tools and history | `PASS` |
| `SPEC-0010` | Integrated cross-feature workflow and ownership | `PASS` |

## 3. Baseline Consistency

The accepted specification set consistently requires:

- manual startup and residency while the application is running;
- user-controlled PrintScreen takeover;
- MainWindow `X` directly exits、releases takeover and does not hide to tray;
- one session-owned Frozen Virtual Desktop covering all connected displays;
- one rectangular Selection that can span displays;
- transparent final-image pixels for physical non-display gaps;
- clear Selection interior and dimmed exterior;
- mouse release to `SelectionLocked`, never directly to output;
- move、edge／corner resize and reselection before commitment;
- mandatory Editing／confirmation with optional Annotation actions;
- Rectangle、Arrow／Line、Highlighter、Text、Mosaic／Blur and Numbered Marker;
- Annotation object editing、Undo／Redo、Virtual Desktop anchoring and output clipping;
- Complete to Clipboard only;
- Save As initially opening Downloads、PNG and the same Clipboard result;
- PNG retention when later Clipboard delivery fails;
- recoverable output failure returning to Editing with state preserved;
- Cancel with no output、capture-UI cleanup and focus restoration;
- stale session／revision outcomes unable to advance workflow state.

No current Spec contains a valid path from mouse release directly to Clipboard.

## 4. Feature and Ownership Review

| Feature | Normative owner | Required v1 boundary |
| --- | --- | --- |
| `FEAT-001` | `SPEC-0005` | Resident entry、direct exit、display freeze、Virtual Desktop and Selection lifecycle |
| `FEAT-002` | `SPEC-0009` | Editing function bar、Annotation document、tools and Undo／Redo |
| `FEAT-003` | `SPEC-0007` | Clipboard publication after explicit commitment |
| `FEAT-004` | `SPEC-0008` | Downloads-default Save As、PNG creation and retained file outcome |
| `FEAT-005` | `SPEC-0006` | Cancel、failure preservation、cleanup、feedback and focus restoration |
| Shared integration | `SPEC-0010` | Single session／revision context and legal feature order |

Clipboard and PNG Output remain separate capability boundaries. Save requires both to succeed before overall completion, but a successfully created PNG remains retained if later Clipboard delivery fails.

## 5. Acceptance Readiness

| Area | Result |
| --- | --- |
| User-visible workflow | `PASS` |
| State、exit and commitment boundaries | `PASS` |
| Multi-display、coordinates and transparent gaps | `PASS` |
| Annotation tool and object requirements | `PASS` |
| Clipboard、Downloads Save As and retained PNG behavior | `PASS` |
| Cancellation and recoverable failure behavior | `PASS` |
| Privacy and external-GUI evidence boundaries | `PASS` |
| Stable acceptance-criterion namespaces | `PASS` |
| Current code conformance | `FAIL — tracked separately in PRD-TRACEABILITY-MATRIX-001` |

Specification acceptance does not imply implementation completion.

## 6. Remaining Explicit Decisions

Implementation must not guess:

- final keyboard-only Annotation acceptance standard;
- quantitative performance targets;
- exact supported display-count and maximum Virtual Desktop dimensions.

The following are resolved:

- MainWindow／System Tray exit behavior;
- transparent non-display gaps;
- Save As initial Downloads folder;
- PNG retention after later Clipboard failure.

## 7. Baseline Decision

`SPEC-0002` through `SPEC-0010` form the accepted SnipPlus v1.2 Specification baseline.

Architecture、code、tests and runtime verification must conform to this baseline. Historical Research and prior reviews are non-normative where they conflict. Implementation gaps stay in the existing conformance matrix; they do not justify another readiness or closure-document chain.