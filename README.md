# SnipPlus

SnipPlus is a documentation-first Windows desktop product repository. Product, behavioral-specification and abstract-architecture baselines are established; implementation has not started.

## Start here

- [Repository current-state and implementation-readiness audit](docs/REPOSITORY-CURRENT-STATE-AND-IMPLEMENTATION-READINESS-AUDIT.md)
- [Documentation index](docs/index.md)
- [Project lifecycle](docs/PROJECT-LIFECYCLE.md)
- [Frozen PRD baseline](PRD/PRD-FREEZE-REVIEW.md)
- [Frozen Specification baseline](Specs/SPEC-BASELINE-REVIEW.md)
- [Frozen Architecture baseline](Architecture/ARCH-BASELINE-REVIEW.md)
- [Technology decision roadmap](Architecture/TECHNOLOGY-DECISION-ROADMAP.md)
- [Development Guide](docs/guides/development-guide.md)
- [Contributing](CONTRIBUTING.md)

## Repository map

```text
SnipPlus/
├─ Architecture/       Architecture baseline, ADR governance and decisions
├─ PRD/                Frozen product-requirement baseline
├─ Specs/              Frozen observable-behavior baseline
├─ docs/               Research, analysis, decisions, audits, guides and design references
├─ AGENTS.md           Repository work rules
├─ CONTRIBUTING.md     Collaboration and change workflow
├─ ROADMAP.md          Current phase and exit criteria
├─ CHANGELOG.md        Meaningful repository changes
├─ TODO.md             Active decision and engineering backlog
└─ README.md           Repository entry point
```

## Current status

| Area | Status |
| --- | --- |
| Product requirements | PRD v1.0 `Freeze Approved` |
| Behavioral specifications | Specification v1.0 `Freeze Approved` |
| Architecture | Abstract baseline `Freeze Approved` |
| UI framework | ADR-0002 Draft; WinUI 3 proposed, not accepted |
| Other core technology decisions | Candidate / not accepted |
| Interface contracts | Not completed |
| Project structure | Not completed |
| Application code | Not started |
| Runtime verification | Not started |
| Build, test, CI and release | Not established |

## Current working direction

The repository no longer needs additional prerequisite or closure-review chains. The next useful work is to accept or reject core ADRs, define consolidated contracts and project structure, then perform one implementation-readiness review.

`docs/Research/Technology/29–80` remains historical research and governance evidence. The 039–052 documentary chain is closed and must not be extended automatically without new evidence or an explicit human decision.

## Working principle

Use the frozen PRD, Specification and Architecture baselines as upstream sources. Keep `Draft`, `Candidate`, `UNKNOWN` and `TBD` visible. Documentation completeness does not authorize implementation; coding requires an explicit implementation task.
