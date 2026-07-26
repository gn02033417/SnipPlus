
# SnipPlus Repository Rules

## Repository status

SnipPlus is currently in the documentation-foundation phase. The repository
does not contain application source code, build configuration, tests, or an
approved product specification. Do not describe proposed behavior as an
implemented capability.

The current task is documentation-only. Do not add screenshot functionality,
application code, build scripts, dependencies, or runtime configuration unless
a later task explicitly authorizes implementation.

## Source of truth

Use the following order when documents disagree:

1. `docs/Research/` — externally observed facts and source-backed research.
2. `docs/Analysis/` — structured analysis of research, without product decisions.
3. `docs/Decision/` — explicit adoption decisions with evidence, risks, and open questions.
4. `PRD/` — product intent, users, goals, and scope.
5. `Specs/` — observable behavior and acceptance criteria.
6. `Architecture/` — system boundaries, responsibilities, and technical decisions.
7. `Architecture/adr/` — decisions that explain why a durable choice was made.
8. `docs/` — navigation, working agreements, standards, and design references.

Research facts must include their source and verification method. Analysis must
refer back to Research and must not silently become a product requirement or
design decision. Decision records must separate the decision value, reason,
evidence, risk, and open question. If a fact, requirement, or design is not
confirmed in these documents, label it `UNKNOWN`, `TBD`, `Proposal`, or
`Assumption`; do not silently fill the gap.

## Documentation rules

- Start at `README.md` or `docs/index.md`.
- Keep product requirements separate from implementation details.
- Update the relevant PRD or Spec before changing Architecture for a product
  behavior change.
- Add an ADR for a durable decision with meaningful trade-offs.
- Use the naming rules in `docs/standards/markdown-naming.md`.
- Keep Markdown links relative so the repository remains portable.
- Preserve Traditional Chinese text and UTF-8 encoding.
- Every new document must state its status when the content is not final.

## Change boundaries

- Prefer small, focused document changes.
- Do not edit unrelated files.
- Do not run build, restore, test, publish, deploy, or application runtime
  commands as part of documentation work.
- Static checks may inspect Markdown links, headings, file names, and Git diff.
- Runtime behavior is unverified until an implementation exists and a later
  task explicitly requests verification.
