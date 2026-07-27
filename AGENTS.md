# SnipPlus Repository Rules

## Repository Status

SnipPlus has an accepted v1 product and quality baseline dated `2026-07-27`.

Current state:

- Canonical PRD、Specs、Architecture and Implementation Contracts are aligned with the Repository owner’s explicit decisions.
- Existing source is an earlier single-display technical prototype and is not conformant with the accepted v1 workflow.
- `PRD/PRD-TRACEABILITY-MATRIX.md` owns current code／test conformance status and correction order.
- No product-quality decision currently blocks ordered v1 implementation.
- Coding may proceed only through an explicit user task and in the required order.
- Do not create more readiness、authorization、closure or Research-document chains.

Do not describe the current application as v1-complete based on build、test counts or historical synthetic runtime evidence.

## Effective Source of Truth

Use sources in this order:

1. Accepted `PRD-0002` through `PRD-0006` and current PRD Freeze Review.
2. Accepted `SPEC-0001` through `SPEC-0010` and current Specification Baseline Review.
3. Accepted `ARCH-0001` through `ARCH-0005` and Accepted ADRs.
4. `Architecture/IMPLEMENTATION-CONTRACTS.md` version 2.3.
5. `Architecture/PROJECT-STRUCTURE.md`.
6. `PRD/PRD-TRACEABILITY-MATRIX.md`.
7. Current code and tests as implementation evidence only.
8. Research and historical reviews as non-normative background.

Higher-priority Accepted sources override lower-priority sources and existing code.

## Accepted v1 Product Baseline

### Entry、residency and exit

- The user manually starts SnipPlus.
- SnipPlus remains resident while running.
- A user setting enables or disables PrintScreen takeover.
- Enabled PrintScreen is the primary v1 capture entry.
- Disabled takeover does not intercept PrintScreen.
- MainWindow `X` directly exits SnipPlus、releases takeover and leaves no hidden resident process.
- An in-app capture command is secondary or diagnostic only.

### Capture and Selection

- Freeze all connected supported displays before Selection becomes interactive.
- Present one logical Frozen Virtual Desktop canvas.
- Support negative coordinates、mixed DPI and one rectangular Selection crossing display boundaries.
- Show a semi-transparent mask outside Selection and clear frozen content inside.
- Mouse release locks Selection and never writes Clipboard or a file.
- Locked Selection supports pointer move、four-edge／four-corner resize and reselection.
- Physical non-display gaps produce transparent final-image pixels.

### Editing and Annotation

- Editing／confirmation always appears after a valid lock.
- Annotation actions are optional; Complete、Save or Cancel is mandatory.
- Required tools: Rectangle、Arrow／Line、Highlighter、Text、Mosaic／Blur、Numbered Marker、color、thickness、Undo and Redo.
- Applicable objects support pointer selection、move、resize、restyle and delete.
- Annotation objects use Frozen Virtual Desktop coordinates and are clipped, not deleted, outside current Selection.
- V1 Annotation acceptance is pointer-driven.

### Output

- Complete writes Clipboard only.
- Save opens Save As、supports PNG only、initially opens Downloads、proposes `SnipPlus_yyyy-MM-dd_HHmmss.png` and also writes Clipboard.
- Save As cancellation returns to Editing.
- PNG failure returns to Editing and does not update Clipboard.
- Clipboard failure after PNG success retains the PNG、returns to Editing and reports the partial outcome.
- Recoverable output failure retains Selection and Annotation state.
- Success is silent.
- A commit still running after `300 ms` shows non-blocking progress.

### Cancel and focus

- PrintScreen is the required global capture key.
- Esc cancels before Selection、during drag and during stable Editing.
- Cancel writes neither Clipboard nor a file.
- Complete、successful Save、Cancel and terminal failure close capture UI and restore the application active before PrintScreen.
- SnipPlus does not automatically show MainWindow after the Session.
- Normal SnipPlus windows do not appear in frozen source content.

## Quantitative Performance Baseline

Measurement protocol:

- Release x64 without debugger;
- Windows 11 24H2 x64;
- 16 GB RAM or more;
- Direct3D 11-class hardware acceleration;
- SSD;
- `3` warm-up runs and at least `30` measured runs;
- report p50、p95 and maximum.

Required targets:

- PrintScreen → interactive Selection: p95 `≤ 500 ms` Owner Reference／Standard、`≤ 1,000 ms` Maximum.
- Pointer-driven Selection／Annotation frame time: p95 `≤ 33 ms`.
- Pointer／UI action → visible response: p95 `≤ 100 ms`.
- Complete p95 tiers: `≤ 1.5 s`、`4 s`、`8 s`.
- Save p95 tiers after Save As confirmation: `≤ 2 s`、`6 s`、`12 s`.
- Idle private working set `≤ 250 MB`; maximum peak `≤ 2.0 GB`.
- Cleanup and repeated-session memory limits are normative in `PRD-0006`.

## Supported v1 Capacity Envelope

- `1` through `4` active logical desktop display surfaces.
- Each display `≤ 3840 × 2160` physical pixels.
- Total active display-source pixels `≤ 33,177,600`.
- Virtual Desktop width and height each `≤ 16,384` physical pixels.
- Final Selection width and height each `≤ 16,384` pixels.
- Final Selection area `≤ 67,108,864` pixels.
- Transparent topology gaps count toward final Selection area.
- An 8K display is outside v1.
- Unsupported capacity fails before interactive Selection without partial capture.

Mandatory real-world verification includes the Repository owner’s current configuration:

- primary `2560 × 1440`;
- lower `1920 × 1080` at Windows scaling `150%`;
- left `2560 × 1440`.

## Keyboard Boundary

Required in v1:

- PrintScreen capture entry;
- Esc capture cancellation;
- ordinary text entry including Chinese IME;
- accessible names and non-color-only state indicators for required controls.

Deferred from v1:

- complete keyboard-only Annotation workflow;
- F6／Tab zone and object traversal as a product workflow;
- single-letter tool shortcuts;
- Ctrl-based Undo／Redo、Save or Complete shortcuts;
- Delete and Arrow-key object manipulation;
- keyboard-created Annotation objects;
- pointer-unused acceptance after `SelectionLocked`.

Do not implement or claim these deferred shortcuts without a later explicit product decision.

## Explicitly Deferred Product Capabilities

- opaque freehand pen;
- ellipse;
- pin image to desktop;
- OCR;
- capture history;
- delayed capture;
- additional save formats;
- font-family selection、italic、underline or text background;
- keyboard-only Annotation and non-PrintScreen tool／action shortcuts;
- HDR preservation、ARM64、cloud、sharing、plugins、telemetry、updates or release publication.

## Conformance Correction Order

1. Resident lifecycle、direct application exit and user-controlled PrintScreen takeover setting.
2. PrintScreen entry integrated with `COMP-001`.
3. Four-4K capacity policy、Frozen Virtual Desktop context and per-display frame ownership.
4. All-display presentation、crosshair and cross-monitor initial Selection.
5. Locked Selection、pointer move、edge／corner resize and reselection.
6. Accepted workflow state graph.
7. Function bar、Complete／Save／Cancel、progress and focus restoration.
8. Annotation document、required pointer-driven tools and object editing.
9. Annotation Undo／Redo、Virtual Desktop anchoring and Selection clipping.
10. Complete final render、capacity revalidation、transparent gaps and Clipboard.
11. Save As、Downloads default、PNG、same-result Clipboard and retained-file outcome.
12. Recoverable failure preservation、stale-revision protection、performance／memory evidence and required accessibility.
13. Explicitly authorized Owner Reference、Standard and Maximum runtime verification.

For each focused task:

- read the owning requirements、Specs and contracts;
- inspect only relevant code and tests;
- implement the smallest complete slice;
- run only explicitly authorized commands;
- update `CHANGELOG.md` and the conformance matrix after evidence exists;
- stop before selecting the next slice.

## Tool and Execution Rules

- Do not automatically restore、build、test、run or publish unless explicitly authorized in the current task.
- Do not launch Paint、Notepad or another external GUI fixture during ordinary development or non-interactive tests.
- Interactive verification requires explicit authorization and advance disclosure.
- Prefer deterministic in-process synthetic frames.
- Never commit real desktop screenshots or Clipboard payloads.
- Redact private paths、window titles、account names and machine identifiers from evidence.

## Documentation Discipline

- Update existing canonical documents rather than creating another planning family.
- Do not create prerequisite、readiness reassessment、authorization request、artifact-control or closure-review files.
- Do not modify Accepted product scope merely to match existing code.
- Product changes require explicit Repository owner direction.
- After each focused task, stop and report before choosing another slice.