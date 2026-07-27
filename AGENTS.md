# SnipPlus Repository Rules

## Repository status

SnipPlus has Frozen PRD、Specification and Architecture baselines, Accepted ADR-0002 through ADR-0007, Accepted Implementation Contracts, an Accepted Project Structure and an Approved Implementation Readiness Review with a corrective amendment for Region Selection behavior.

The repository now contains：

- `SnipPlus.sln` and approved build configuration.
- Four source projects and three test projects.
- A packaged WinUI 3 application shell.
- Capture、coordinate mapping、crop、render、Clipboard、cancellation and retry implementations.
- Unit、Contract、Rendering and authorized Platform tests.
- Recorded restore、build、test and runtime evidence.

Current state：

- Implementation preparation：Complete.
- First vertical slice：Implemented; the Region Selection UX correction is complete and verified.
- Corrected workflow：One frozen source frame is acquired before selection、shown during selection、and used for the final crop without a second desktop capture.
- Implementation evidence：Locked restore、Release x64 build、33 non-interactive tests、2 Windows platform tests and packaged synthetic runtime verification are recorded in `CHANGELOG.md`.
- Feature expansion：Paused after the corrected Capture／Selection workflow; do not start the next feature in this task.
- Additional prerequisite paperwork：Not required.
- Source-code changes：Allowed only when the current user task explicitly requests implementation or correction.
- Restore／build／test／runtime：Allowed only when explicitly included in the current task.

Do not describe build or test success as proof of acceptable product behavior. Runtime evidence must satisfy the current Spec and user-visible workflow, not merely demonstrate that the technical pipeline runs.

## Source of truth

Use the source that owns the disputed concern：

1. Frozen `PRD/` — product intent、goals and scope.
2. Frozen `Specs/` — observable behavior and acceptance criteria. Corrected `SPEC-0005` owns the visible frozen-frame Selection and same-frame Crop requirement.
3. Frozen `Architecture/ARCH-*` — Layer、Module、Component and Interaction ownership.
4. Accepted `Architecture/adr/` — durable technology decisions.
5. `Architecture/IMPLEMENTATION-CONTRACTS.md` — cross-project semantic、lifecycle and failure contracts.
6. `Architecture/PROJECT-STRUCTURE.md` — toolchain、project mapping and build／test baseline.
7. `docs/IMPLEMENTATION-READINESS-REVIEW.md` — approved first-slice scope、non-goals and corrective implementation boundary.
8. `docs/Research/` — external facts and historical evidence; Research does not override Accepted decisions.
9. `docs/Analysis/` and `docs/Decision/` — analysis／adoption history.
10. Other `docs/` — guides、navigation、standards and evidence records.

When sources at the same ownership level conflict, stop and report rather than silently choosing.

## Corrected first vertical slice boundary

Implementation or correction tasks may create or modify only the functionality required for：

- Explicit in-app capture command.
- Resolve one single-monitor display-context snapshot.
- Acquire one immutable full-monitor source frame before Region Selection.
- Present that same frame as the Region Selection background.
- Keep source content clearly visible inside the selected region; the outside region may be dimmed.
- Convert Selection DIPs to physical-pixel bounds using the same display-context snapshot.
- Crop the final result from that exact frozen frame without a second desktop capture after selection.
- Canonical BGRA8 premultiplied SoftwareBitmap result.
- Composition／Win2D result display.
- DataPackage Clipboard delivery.
- Cancellation、typed failure、bounded retry and complete frozen-frame cleanup.
- Required Unit、Contract、Rendering and explicitly authorized Platform／runtime tests.

Do not add global hotkeys、multi-monitor stitching、window-capture product mode、annotation tools、toolbar redesign、file-output UI、HDR preservation、DXGI／GDI fallback、telemetry、cloud、OCR、plugins、updates or release publication unless a later task explicitly changes scope.

## Implementation workflow

1. Read corrected `SPEC-0005`、Implementation Readiness、Contracts and Project Structure.
2. Inspect only the files needed for the reported behavior or requested slice.
3. Stop unrelated Clipboard hardening、Packaging or feature expansion while the Region Selection defect remains open.
4. Keep COMP-001 as the sole Workflow State Authority.
5. Keep platform types out of Core and platform adapters out of product-semantic ownership.
6. Establish explicit frozen-frame ownership across acquisition、presentation、selection、crop、cancel、failure and cleanup.
7. Add or update tests with every behavior correction.
8. Run only the checks explicitly authorized by the current task.
9. Record actual findings; do not rewrite failed evidence as success.
10. Stop on a scope、dependency、privacy、architecture or product-behavior conflict.
11. After the requested correction, stop and report before starting another feature.

## Interactive verification and external GUI rules

- Interactive Platform or packaged runtime verification requires explicit authorization in the current user task.
- Before launching an interactive verification, state which application or window will be opened and why.
- Do not automatically start Paint、Notepad or any other external GUI fixture during normal development、restore、build、unit tests、static checks or product startup.
- Normal SnipPlus Capture Workflow must never launch an external GUI application.
- Prefer deterministic in-process synthetic frames、checkerboards、color blocks、gradients or a SnipPlus-owned test surface.
- An external GUI fixture may be used only when the user explicitly authorizes that exact interactive verification.
- Interactive test processes and temporary windows must be cleaned up after the authorized verification.
- Do not infer authorization from a previous task、a prior successful run or the existence of an Interactive test category.

## Documentation anti-proliferation

Do not create prerequisite、readiness reassessment、authorization request、artifact-control or closure-review documents simply because implementation is incomplete or a test fails.

A new planning or decision document requires：

- changed product scope;
- a verified compatibility or runtime conflict;
- changed Architecture ownership;
- a materially new release or platform boundary;
- a superseding durable decision.

The Clipboard D1 chain ending at `RESEARCH-TECH-CLIPBOARD-052` is closed.

The current Region Selection correction must modify existing Specifications、code、tests、CHANGELOG and evidence records only as necessary. Do not restart the prior document chain.

## Safety and evidence

- Never capture automatically; explicit user action is required.
- Use synthetic or public fixtures for tests.
- Do not commit real desktop screenshots or Clipboard payloads.
- Do not persist private desktop content as runtime evidence.
- Redact account names、paths、window titles and machine identifiers from evidence.
- Build／test artifacts belong under ignored artifact roots.
- Clipboard and Output remain independent.
- COMP-001 remains the sole Workflow State Authority.
- Selection presentation and final Crop must use the same frozen source frame.
- Build success、test success and Clipboard success do not compensate for an unusable Selection experience.

## Change discipline

- Prefer focused changes.
- Update tests with behavior changes.
- Update CHANGELOG and actual evidence／status only after implementation results exist.
- Modify Frozen sources or Accepted ADRs only through an explicit user-directed correction or the corresponding change／supersession process.
- Do not modify documentation merely to create the appearance of progress.
- Do not continue with the next backlog item while a reported blocking product defect remains unresolved.
