# SnipPlus Repository Rules

## Repository Status

SnipPlus has an accepted revised v1 product baseline dated `2026-07-27`.

Current state:

- Canonical PRD、Specs、Architecture、ADRs and Implementation Contracts are aligned with the Repository owner’s explicit first-release decisions.
- Existing source is an earlier single-display technical prototype and is not conformant with the revised v1 workflow.
- `PRD/PRD-TRACEABILITY-MATRIX.md` contains the completed static requirements-to-code conformance audit and ordered correction plan.
- MainWindow exit、Virtual Desktop gap output and PNG-retention decisions are now resolved.
- Coding may proceed only through an explicit user task and must begin with the first unresolved prerequisite in the matrix.
- No additional readiness、closure、authorization-request or Research-document chain is required.

Do not describe the current application as v1-complete based on build、test counts or prior synthetic runtime evidence.

## Effective Source of Truth

Use sources in this order:

1. Accepted `PRD-0002` through `PRD-0006` and current PRD Freeze Review.
2. Accepted `SPEC-0001` through `SPEC-0010` and current Specification Baseline Review.
3. Accepted `ARCH-0001` through `ARCH-0005` and Accepted ADRs.
4. `Architecture/IMPLEMENTATION-CONTRACTS.md` version 2.1.
5. `Architecture/PROJECT-STRUCTURE.md`.
6. `PRD/PRD-TRACEABILITY-MATRIX.md` for current code／test conformance status and correction order.
7. Current code and tests as implementation evidence only.
8. Research、historical reviews and prior document chains as non-normative history.

When a lower-priority source conflicts with a higher-priority Accepted source, the higher-priority source wins. Do not silently rewrite accepted product behavior to match existing code.

## Accepted v1 Product Baseline

### Entry、residency and exit

- The user manually starts SnipPlus.
- SnipPlus remains resident while the application is running.
- A user setting enables or disables PrintScreen takeover.
- Enabled PrintScreen is the primary v1 capture entry.
- Disabled takeover does not intercept PrintScreen.
- An in-app capture command is secondary or diagnostic only.
- MainWindow `X` directly exits SnipPlus; it does not hide to the System Tray.
- Application exit releases PrintScreen takeover and leaves no hidden resident process.
- If a System Tray surface exists, its explicit Exit action uses the same shutdown path.

### Capture and Selection

- Freeze all connected displays before Selection becomes interactive.
- Present one logical Frozen Virtual Desktop canvas.
- Support negative coordinates、mixed DPI and one rectangular Selection crossing display boundaries.
- Show a semi-transparent mask outside the Selection and clear frozen content inside.
- Mouse release locks Selection and never writes Clipboard or a file.
- Locked Selection supports move、four-edge／four-corner resize and reselection.
- Physical non-display gaps inside the selected rectangle produce transparent final-image pixels.

### Editing and Annotation

- The Editing／confirmation stage always appears after a valid lock.
- Annotation actions are optional; explicit Complete、Save or Cancel is mandatory.
- Required v1 tools: Rectangle、Arrow／Line、Highlighter、Text、Mosaic／Blur、Numbered Marker、color、thickness、Undo and Redo.
- Applicable objects support selection、move、resize、restyle and delete.
- Annotation objects use Frozen Virtual Desktop coordinates and are clipped, not deleted, outside the current Selection.

### Output

- Complete writes Clipboard only.
- Save opens Save As、supports PNG only、initially opens Downloads、proposes `SnipPlus_yyyy-MM-dd_HHmmss.png` and also writes Clipboard.
- The user may change the Save As destination and filename.
- Save As cancellation returns to Editing.
- PNG failure returns to Editing and does not update Clipboard.
- Clipboard failure after PNG success retains the PNG、returns to Editing and reports that Clipboard delivery failed.
- Recoverable render、save or Clipboard failure retains Selection and Annotation state.
- Success is silent.

### Cancel and Focus

- Esc cancels before Selection、during drag and during Editing.
- Cancel writes neither Clipboard nor a file.
- Complete、successful Save、Cancel and terminal failure close capture UI and restore the application active before PrintScreen.
- SnipPlus does not automatically show its main window after the session.
- Normal SnipPlus windows do not appear in frozen source content.

## Explicitly Deferred

Do not add without a later explicit product decision:

- opaque freehand pen;
- ellipse;
- pin image to desktop;
- OCR;
- capture history;
- delayed capture;
- additional save formats;
- font-family selection、italic、underline or text background;
- HDR preservation、ARM64、cloud、sharing、plugins、telemetry、updates or release publication.

## Conformance Correction Order

Follow this order. Do not begin a later item while an earlier prerequisite remains `Missing` or `Incorrect`:

1. Resident lifecycle、direct application exit and user-controlled PrintScreen takeover setting.
2. PrintScreen entry integrated with `COMP-001`.
3. Frozen Virtual Desktop session context and per-display frame ownership.
4. All-display presentation、crosshair and cross-monitor initial Selection.
5. Locked Selection、move、edge／corner resize and reselection.
6. Accepted workflow state graph including `SelectionLocked` and `Editing`.
7. Function bar、Complete／Save／Cancel commitments and focus restoration.
8. Annotation document、required tools and object editing.
9. Annotation-only Undo／Redo、Virtual Desktop anchoring and Selection clipping.
10. Complete final render、transparent gaps and Clipboard.
11. Save As、Downloads default、PNG file output、same-result Clipboard and retained-file partial outcome.
12. Recoverable failure preservation、stale-revision protection and accessibility.
13. Explicitly authorized multi-display runtime verification.

For every focused correction:

- read the owning requirement、Spec and contract;
- inspect only relevant code and tests;
- classify existing behavior as reusable、partial、incorrect or obsolete;
- implement the smallest complete slice;
- add or update relevant tests;
- run only commands explicitly authorized by the current task;
- update `CHANGELOG.md` and corresponding conformance rows after evidence exists;
- stop before selecting the next slice.

## Remaining Open Product Decisions

Do not guess:

- quantitative performance targets;
- exact supported display-count and maximum Virtual Desktop dimensions;
- final keyboard-only Annotation acceptance scope.

The following are **not** open decisions anymore:

- MainWindow `X` exits and releases takeover;
- no close-to-tray behavior;
- final non-display gaps are transparent;
- Save As initially opens Downloads;
- PNG is retained after later Clipboard failure.

Stop only when the current implementation step reaches a still-unresolved decision.

## Architecture Discipline

- `COMP-001` remains the sole shared Workflow State Authority.
- Platform adapters return typed outcomes and never mutate shared workflow state.
- Keep concrete platform types out of Core and platform-neutral Contracts.
- One Session context owns frozen frames、Selection、Annotation、render and output revisions.
- Stale asynchronous outcomes never advance a newer or cancelled Session.
- Cleanup is idempotent.
- Mouse release cannot invoke Clipboard or file output.
- Clipboard and PNG Output remain separate capabilities; Save coordination requires both to succeed before overall completion.
- A successfully created PNG is user output and is not deleted by later Clipboard failure cleanup.

## Tool and Execution Rules

- Do not automatically restore、build、test、run or publish unless the current user task explicitly authorizes it.
- Do not launch Paint、Notepad or another external GUI fixture during ordinary development、static review、build or non-interactive tests.
- Interactive verification requires explicit authorization in the current task and prior disclosure of what will open.
- Prefer deterministic in-process synthetic frames.
- Never commit real desktop screenshots or Clipboard payloads.
- Redact private paths、window titles、account names and machine identifiers from evidence.

## Documentation Discipline

- Prefer updating Accepted canonical documents over creating another planning document.
- Do not create prerequisite、readiness reassessment、authorization request、artifact-control or closure-review files to simulate progress.
- Historical Clipboard D1 documents remain history and do not drive current implementation.
- Do not modify documentation merely to match existing code.
- Do not modify Accepted product scope without explicit Repository owner direction.
- After each focused task, stop and report before choosing another slice.