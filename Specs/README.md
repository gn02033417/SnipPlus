# Specs

Specifications define observable、testable behavior. They implement accepted PRD without inventing product decisions or treating existing code as the source of truth.

## Effective Specification Baseline

The effective first-release baseline is the accepted `2026-07-27` revision:

- [SPEC-0002 Specification Guidelines](SPEC-0002-specification-guidelines.md)
- [SPEC-0003 System Requirements](SPEC-0003-system-requirements.md) — `Accepted v1.2`
- [SPEC-0004 Feature Catalog](SPEC-0004-feature-catalog.md) — `Accepted v1.1`
- [SPEC-0005 Capture Workflow](SPEC-0005-capture-workflow.md) — `Accepted v1.1`
- [SPEC-0006 Workflow Boundaries and Feedback](SPEC-0006-workflow-boundaries-and-feedback.md) — `Accepted v1.1`
- [SPEC-0007 Clipboard Handoff](SPEC-0007-clipboard-handoff.md) — `Accepted v1.1`
- [SPEC-0008 Capture Output](SPEC-0008-capture-output.md) — `Accepted v1.1`
- [SPEC-0009 Annotation Capability](SPEC-0009-annotation-capability.md) — `Accepted v1.0`
- [SPEC-0010 Feature Integration](SPEC-0010-feature-integration.md) — `Accepted v1.1`
- [Specification Baseline Review](SPEC-BASELINE-REVIEW.md) — current acceptance record

Earlier draft wording is historical only. Where an old review or Research file conflicts with the accepted specifications above, the accepted specifications are normative.

## Key Corrected Behavior

- PrintScreen takeover is the primary v1 entry when enabled.
- MainWindow `X` exits SnipPlus、releases takeover and does not hide to tray.
- All connected displays are frozen into one logical Virtual Desktop session.
- One rectangular selection may span multiple displays.
- Non-display gap pixels in final output are transparent.
- Mouse release locks selection and never commits output.
- Editing／confirmation always appears; Annotation actions can be skipped.
- Complete writes Clipboard only.
- Save As initially opens Downloads、creates PNG and writes Clipboard.
- PNG is retained if later Clipboard delivery fails.
- Recoverable output failure retains Editing state.
- Cancel closes capture UI、produces no output and restores the previous application.

## Specification Rules

- A Spec must trace to accepted FR／NFR IDs.
- Acceptance criteria describe user-visible or externally observable behavior.
- Architecture and code may not redefine accepted Spec behavior.
- Unknown product choices must be marked and escalated.
- Do not create additional readiness or closure Specs because implementation is incomplete.

## Current Implementation Relationship

`PRD/PRD-TRACEABILITY-MATRIX.md` is the current authority for mapping these Specs to code、tests and runtime evidence. The next implementation work follows that matrix; no additional Specification planning document is required.