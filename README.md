# SnipPlus

SnipPlus is a documentation-first Windows desktop product repository. Product, behavioral-specification and abstract-architecture baselines are established; the UI framework decision is accepted; implementation has not started.

## Start here

- [Repository current-state and implementation-readiness audit](docs/REPOSITORY-CURRENT-STATE-AND-IMPLEMENTATION-READINESS-AUDIT.md)
- [Documentation index](docs/index.md)
- [Project lifecycle](docs/PROJECT-LIFECYCLE.md)
- [Frozen PRD baseline](PRD/PRD-FREEZE-REVIEW.md)
- [Frozen Specification baseline](Specs/SPEC-BASELINE-REVIEW.md)
- [Frozen Architecture baseline](Architecture/ARCH-BASELINE-REVIEW.md)
- [Accepted UI Framework ADR](Architecture/adr/ADR-0002-ui-framework-selection.md)
- [Technology decision roadmap](Architecture/TECHNOLOGY-DECISION-ROADMAP.md)
- [Development Guide](docs/guides/development-guide.md)
- [Contributing](CONTRIBUTING.md)

## Repository map

```text
SnipPlus/
├─ Architecture/       Architecture baseline, ADR governance and accepted/candidate decisions
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
| UI framework | ADR-0002 `Accepted`; WinUI 3 |
| Rendering Technology | Candidate; next primary decision |
| Capture Backend、Clipboard、Image Representation、Testing | Candidate / not accepted |
| Interface contracts | Not completed |
| Project structure | Not completed |
| Application code | Not started |
| Runtime verification | Not started |
| Build, test, CI and release | Not established |

## Current working direction

The repository no longer needs additional prerequisite or closure-review chains.

The next useful work is:

1. Produce and review the Rendering Technology ADR using `docs/Research/Technology/10–18` as evidence.
2. Continue with Capture Backend、Clipboard Integration、Image Representation and Testing Strategy decisions.
3. Define consolidated contracts and Project Structure.
4. Perform one Implementation Readiness Review.

`docs/Research/Technology/29–80` remains historical research and governance evidence. The 039–052 documentary chain is closed and must not be extended automatically without new evidence or an explicit human decision.

## Working principle

Use the frozen PRD、Specification、Architecture and Accepted ADR baselines as upstream sources. Keep `Draft`、`Candidate`、`UNKNOWN` and `TBD` visible. ADR acceptance does not authorize coding; source-code creation requires an explicit implementation task after Implementation Readiness approval.
