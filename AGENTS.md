# SnipPlus Repository Rules

## Repository status

SnipPlus has Frozen PRD、Specification and Architecture baselines, Accepted ADR-0002 through ADR-0007, Accepted Implementation Contracts, an Accepted Project Structure and an Approved Implementation Readiness Review.

The repository does not yet contain application source code、build configuration、tests or runtime evidence.

Current state：

- Implementation preparation：Complete.
- First vertical slice：Approved; not started.
- Additional pre-coding paperwork：Not required.
- Source-code creation：Allowed only when the current user task explicitly requests implementation.
- Restore/build/test/runtime：Allowed only when explicitly included in that implementation/verification task.

Do not describe planned or accepted behavior as implemented or verified until code and evidence exist.

## Source of truth

Use the source that owns the disputed concern：

1. Frozen `PRD/` — product intent、goals and scope.
2. Frozen `Specs/` — observable behavior and acceptance criteria.
3. Frozen `Architecture/ARCH-*` — Layer、Module、Component and Interaction ownership.
4. Accepted `Architecture/adr/` — durable technology decisions.
5. `Architecture/IMPLEMENTATION-CONTRACTS.md` — cross-project semantic、lifecycle and failure contracts.
6. `Architecture/PROJECT-STRUCTURE.md` — toolchain、project mapping and build/test baseline.
7. `docs/IMPLEMENTATION-READINESS-REVIEW.md` — approved first-slice scope and non-goals.
8. `docs/Research/` — external facts and historical evidence; Research does not override Accepted decisions.
9. `docs/Analysis/` and `docs/Decision/` — analysis/adoption history.
10. Other `docs/` — guides、navigation、standards and evidence records.

When sources at the same ownership level conflict, stop and report rather than silently choosing.

## First vertical slice boundary

Implementation tasks may create only the approved solution/projects and functionality required for：

- Explicit in-app capture command.
- Single-monitor region selection.
- Windows.Graphics.Capture one-shot acquisition and crop.
- Canonical BGRA8 premultiplied SoftwareBitmap result.
- Composition／Win2D result display.
- DataPackage Clipboard delivery.
- Cancellation、typed failure、bounded retry and cleanup.
- Required Unit、Contract、Rendering and authorized Platform tests.

Do not add global hotkeys、multi-monitor stitching、window-capture product mode、annotation tools、file-output UI、HDR preservation、DXGI/GDI fallback、telemetry、cloud、OCR、plugins、updates or release publication unless a later task explicitly changes scope.

## Implementation workflow

1. Read Implementation Readiness、Contracts and Project Structure.
2. Create the approved skeleton.
3. Restore/build the empty baseline before feature work when authorized.
4. Implement Core contracts/state/failure tests before Windows side effects.
5. Keep platform types out of Core.
6. Run only the checks authorized by the current task.
7. Record actual findings; do not rewrite failed evidence as success.
8. Stop on a scope、dependency、privacy or architecture conflict.

## Documentation anti-proliferation

Do not create prerequisite、readiness reassessment、authorization request、artifact-control or closure-review documents simply because implementation is incomplete or a test fails.

A new planning/decision document requires：

- changed product scope;
- a verified compatibility/runtime conflict;
- changed Architecture ownership;
- a materially new release/platform boundary;
- a superseding durable decision.

The Clipboard D1 chain ending at RESEARCH-TECH-CLIPBOARD-052 is closed.

## Safety and evidence

- Never capture automatically; explicit user action is required.
- Use synthetic/public fixtures for tests.
- Do not commit real desktop screenshots or Clipboard payloads.
- Redact account names、paths、window titles and machine identifiers from evidence.
- Build/test artifacts belong under ignored artifact roots.
- Clipboard and Output remain independent.
- COMP-001 remains the sole Workflow State Authority.

## Change discipline

- Prefer focused changes.
- Update tests with behavior changes.
- Update CHANGELOG and actual evidence/status after implementation.
- Modify Frozen sources or Accepted ADRs only through the corresponding change/supersession process.
- Do not modify documentation merely to create the appearance of progress.
