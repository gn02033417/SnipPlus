# SnipPlus Repository Rules

## Repository Status

SnipPlus has an accepted revised v1 product baseline dated `2026-07-27`.

Current state:

- Canonical PRD、Specs and Implementation Contracts have been aligned to the repository owner’s explicit first-release decisions.
- Existing code represents an earlier technical vertical slice and has not yet been proven conformant with the revised v1 scope.
- New feature coding is paused.
- The next activity is one requirements-to-code conformance matrix.
- No additional readiness、closure、authorization-request or research-document chain is required.

Do not describe the current application as v1-complete based on build、test count or prior synthetic runtime evidence.

## Effective Source of Truth

Use sources in this order:

1. Accepted `PRD-0004`、`PRD-0005` and `PRD-0006`, together with existing product principles and vision.
2. Accepted `SPEC-0003` through `SPEC-0010`.
3. Frozen Architecture and Accepted ADRs.
4. `Architecture/IMPLEMENTATION-CONTRACTS.md` version 2.0.
5. Current code and tests as implementation evidence only.
6. Research、historical readiness reviews and prior document chains as non-normative history.

When a lower-priority source conflicts with a higher-priority source, the higher-priority accepted source wins. Do not silently rewrite accepted product behavior to match existing code.

## Accepted v1 Product Baseline

### Entry and residency

- The user manually starts SnipPlus.
- SnipPlus remains resident in the background.
- A user setting enables or disables PrintScreen takeover.
- Enabled PrintScreen is the primary v1 capture entry.
- Disabled takeover must not intercept PrintScreen.
- An in-app capture command is secondary or diagnostic only.

### Capture and selection

- Freeze all connected displays before selection becomes interactive.
- Present one logical Frozen Virtual Desktop canvas.
- Support negative coordinates、mixed DPI and one rectangular selection crossing display boundaries.
- Show a semi-transparent mask outside the selection and clear frozen content inside.
- Mouse release locks selection and never writes Clipboard or a file.
- Locked selection supports move、four-edge／four-corner resize and reselection.

### Editing and annotation

- The editing／confirmation stage always appears after a valid lock.
- Annotation actions are optional; explicit Complete、Save or Cancel is mandatory.
- Required v1 tools: Rectangle、Arrow／Line、Highlighter、Text、Mosaic／Blur、Numbered Marker、color、thickness、Undo and Redo.
- Applicable objects support selection、move、resize、restyle and delete.
- Annotation objects use Frozen Virtual Desktop coordinates and are clipped, not deleted, outside current selection.

### Output

- Complete writes Clipboard only.
- Save opens Save As、supports PNG only、proposes `SnipPlus_yyyy-MM-dd_HHmmss.png` and also writes Clipboard.
- Save As cancellation returns to Editing.
- Recoverable render、save or Clipboard failure retains selection and annotations.
- Success is silent.

### Cancel and focus

- Esc cancels before selection、during drag and during Editing.
- Cancel writes neither Clipboard nor file.
- Complete、successful Save、Cancel and terminal failure close capture UI and restore the application active before PrintScreen.
- SnipPlus does not automatically show its main window after the session.
- Normal SnipPlus windows must not appear in frozen source content.

## Explicitly Deferred

Do not add without a later explicit product decision:

- opaque freehand pen;
- ellipse;
- pin image to desktop;
- OCR;
- capture history;
- delayed capture;
- additional save formats;
- cloud、sharing、plugins、telemetry、updates or release publication.

## Requirements-to-code Conformance Review

Before selecting new coding work, update one existing traceability／conformance matrix.

For every accepted requirement or acceptance criterion, record:

- expected product behavior;
- owning Spec and contract;
- current code reference or `Not implemented`;
- current test reference or `No coverage`;
- runtime evidence state;
- status: `Conforms`、`Partial`、`Missing`、`Incorrect`、`Obsolete` or `Blocked by product decision`;
- focused required action.

Review order:

1. resident PrintScreen entry;
2. multi-display freeze and Virtual Desktop coordinates;
3. cross-monitor selection and mask;
4. selection adjustment;
5. function bar;
6. annotation tools and object editing;
7. annotation clipping and Undo／Redo;
8. Complete Clipboard flow;
9. Save PNG plus Clipboard flow;
10. Cancel、errors、cleanup and focus restoration;
11. privacy and test-fixture boundaries;
12. deferred-feature exclusion.

Do not begin implementation while producing the matrix. Stop after reporting findings and proposing one focused next coding slice.

## Open Product Decisions

Do not guess these behaviors:

- exact system-tray menu and main-window close behavior;
- output representation for gaps between irregularly arranged displays;
- rollback／retention after PNG succeeds but Clipboard fails;
- quantitative performance targets;
- final keyboard-only annotation acceptance scope.

Mark affected rows `Blocked by product decision`.

## Architecture Discipline

- `COMP-001` remains the sole shared Workflow State Authority.
- Platform adapters return outcomes and never mutate shared workflow state.
- Keep platform types out of Core except explicitly accepted canonical image boundaries.
- One session context owns frozen frames、selection、annotations、render and output revisions.
- Stale async outcomes never advance a newer or cancelled session.
- Cleanup is idempotent.

## Tool and Execution Rules

- Do not automatically restore、build、test、run or publish unless the current user task explicitly authorizes it.
- Do not launch Paint、Notepad or another external GUI fixture during ordinary development、static review、build or non-interactive tests.
- Interactive verification requires explicit authorization in the current task and prior disclosure of what will open.
- Prefer deterministic in-process synthetic frames.
- Never commit real desktop screenshots or Clipboard payloads.
- Redact private paths、window titles、account names and machine identifiers from evidence.

## Documentation Discipline

- Prefer updating accepted canonical documents over creating another planning document.
- Do not create prerequisite、readiness reassessment、authorization request、artifact-control or closure-review files to simulate progress.
- Historical Clipboard D1 documents remain history and do not drive current implementation.
- Do not modify documentation merely to match existing code.
- Do not modify accepted product scope without explicit repository-owner direction.
- After each focused task, stop and report before choosing another slice.
