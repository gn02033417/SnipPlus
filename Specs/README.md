# Specs

Specifications define observable、testable behavior. They implement accepted PRD without inventing product decisions or treating existing code as the source of truth.

## Effective Specification Baseline

The effective first-release baseline is the accepted `2026-07-27` complete v1 revision:

- [SPEC-0002 Specification Guidelines](SPEC-0002-specification-guidelines.md)
- [SPEC-0003 System Requirements](SPEC-0003-system-requirements.md) — `Accepted v1.4`
- [SPEC-0004 Feature Catalog](SPEC-0004-feature-catalog.md) — `Accepted v1.1`
- [SPEC-0005 Capture Workflow](SPEC-0005-capture-workflow.md) — current Accepted revision
- [SPEC-0006 Workflow Boundaries and Feedback](SPEC-0006-workflow-boundaries-and-feedback.md) — current Accepted revision
- [SPEC-0007 Clipboard Handoff](SPEC-0007-clipboard-handoff.md) — current Accepted revision
- [SPEC-0008 Capture Output](SPEC-0008-capture-output.md) — current Accepted revision
- [SPEC-0009 Annotation Capability](SPEC-0009-annotation-capability.md) — `Accepted v1.2`
- [SPEC-0010 Feature Integration](SPEC-0010-feature-integration.md) — current Accepted revision
- [Specification Baseline Review](SPEC-BASELINE-REVIEW.md) — `Accepted v1.4`

Earlier draft wording is historical only. Where an old review or Research file conflicts with the accepted specifications above, the accepted specifications are normative.

## Key Corrected Behavior

- PrintScreen takeover is the primary v1 entry when enabled.
- MainWindow `X` exits SnipPlus、releases takeover and does not hide to tray.
- Display capacity is validated before Selection; unsupported configurations never produce partial capture.
- All connected supported displays are frozen into one logical Virtual Desktop Session.
- One rectangular Selection may span multiple displays.
- Non-display gap pixels in final output are transparent.
- Mouse release locks Selection and never commits output.
- Editing／confirmation always appears; Annotation actions can be skipped.
- Annotation creation and object editing are pointer-driven in v1.
- Complete writes Clipboard only.
- Save As initially opens Downloads、creates PNG and writes Clipboard.
- PNG is retained if later Clipboard delivery fails.
- Recoverable output failure retains Editing state.
- Commit progress appears after `300 ms` without changing silent-success behavior.

## Accepted Quality Standards

- Capture start p95 `≤ 500 ms` Owner Reference／Standard、`≤ 1,000 ms` Maximum.
- Pointer interaction p95 frame time `≤ 33 ms`; visible response p95 `≤ 100 ms`.
- Complete／Save output-size tiers and memory limits are defined in PRD-0006.
- Capacity: `1`–`4` displays、`3840 × 2160` per display、`33,177,600` total source pixels、`16,384` maximum Virtual Desktop dimension and `67,108,864` maximum Selection area.
- Owner Reference verification uses primary `2560 × 1440`、lower `1920 × 1080` at `150%` scaling and left `2560 × 1440`.
- PrintScreen and Esc are required; keyboard-only Annotation and non-PrintScreen tool／action shortcuts are deferred.

## Specification Rules

- A Spec must trace to accepted FR／NFR IDs.
- Acceptance criteria describe user-visible or externally observable behavior.
- Architecture and code may not redefine or silently relax accepted Spec behavior.
- No current visible v1 product decision remains open.
- Do not create additional readiness or closure Specs because implementation is incomplete.

## Current Implementation Relationship

`PRD/PRD-TRACEABILITY-MATRIX.md` is the current authority for mapping these Specs to code、tests and runtime evidence. The next implementation work follows that matrix; no additional Specification planning document is required.