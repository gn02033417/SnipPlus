# SnipPlus Repository Rules

## Repository status

SnipPlus has frozen product-requirement, behavioral-specification and abstract-architecture baselines. The repository does not yet contain application source code, build configuration, tests or runtime evidence.

Current lifecycle state:

- PRD v1.0: `Freeze Approved`.
- Specification v1.0: `Freeze Approved`.
- Architecture baseline: `Freeze Approved`.
- ADR-0002 UI Framework Selection: `Draft`, not accepted.
- Remaining core technology decisions: Candidate.
- Interface contracts, project structure and implementation readiness: incomplete.

Do not describe proposed behavior as an implemented capability. Do not start source-code, build, restore, test, publish, deploy or runtime work unless an explicit later task authorizes it.

## Source of truth

Use the following order when documents disagree:

1. `docs/Research/` — externally observed facts and source-backed research.
2. `docs/Analysis/` — structured analysis of research, without product decisions.
3. `docs/Decision/` — explicit adoption decisions with evidence, risks and open questions.
4. Frozen `PRD/` baseline — product intent, users, goals and scope.
5. Frozen `Specs/` baseline — observable behavior and acceptance criteria.
6. Frozen `Architecture/` baseline — system boundaries, ownership and dependencies.
7. Accepted `Architecture/adr/` decisions — durable technical choices.
8. `docs/REPOSITORY-CURRENT-STATE-AND-IMPLEMENTATION-READINESS-AUDIT.md` — current cross-repository status and shortest remaining path.
9. Other `docs/` files — navigation, working agreements, standards and design references.

Draft ADRs are proposals, not effective decisions. Freeze decisions establish a baseline but do not authorize implementation.

Research facts must include source and verification method. Analysis must refer back to Research and must not silently become a product requirement or design decision. Decision records must separate decision value, reason, evidence, risk and open question. Label unconfirmed content `UNKNOWN`, `TBD`, `Candidate`, `Proposal` or `Assumption`.

## Documentation rules

- Start at `README.md`, `docs/index.md` or the repository readiness audit.
- Keep product requirements separate from implementation details.
- Update PRD or Specs before changing Architecture for a product-behavior change.
- Add or update an ADR for a durable technical decision with meaningful trade-offs.
- Use `docs/standards/markdown-naming.md`.
- Keep Markdown links relative.
- Preserve Traditional Chinese text and UTF-8 encoding.
- Every non-final document must state its status.
- Update the relevant index, ROADMAP, TODO and CHANGELOG when repository state changes.

## Anti-proliferation rule

Do not create another prerequisite, readiness reassessment, authorization request, artifact-creation control or closure-review document merely because the previous document concluded `Not ready`.

A new governance document is justified only when at least one of these exists:

- new external evidence;
- a new human or authority decision;
- an accepted upstream change;
- runtime or implementation evidence;
- a materially different decision boundary.

The Clipboard D1 documentary chain ending at `RESEARCH-TECH-CLIPBOARD-052` is closed. Do not extend it automatically.

Prefer consolidated outputs:

- one ADR per major decision;
- one contract package for closely related interfaces;
- one project-structure definition;
- one implementation-readiness review.

## Change boundaries

- Prefer small, focused changes, but allow one coordinated consistency update when several navigation/status files must change together.
- Do not edit unrelated product semantics while fixing indexes or status drift.
- Do not run build, restore, test, publish, deploy or application runtime commands during documentation work.
- Static checks may inspect Markdown links, headings, file names and Git diff.
- Runtime behavior remains unverified until implementation exists and a later task explicitly requests verification.
- Never treat Draft ADR, Candidate technology or documentary closure as implementation permission.
