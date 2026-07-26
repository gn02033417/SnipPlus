# SnipPlus

SnipPlus is a documentation-first Windows desktop product repository. Product、behavioral-specification and abstract-architecture baselines are established; UI Framework and Rendering Technology decisions are accepted; implementation has not started.

## Start here

- [Repository current-state and implementation-readiness audit](docs/REPOSITORY-CURRENT-STATE-AND-IMPLEMENTATION-READINESS-AUDIT.md)
- [Documentation index](docs/index.md)
- [Project lifecycle](docs/PROJECT-LIFECYCLE.md)
- [Frozen PRD baseline](PRD/PRD-FREEZE-REVIEW.md)
- [Frozen Specification baseline](Specs/SPEC-BASELINE-REVIEW.md)
- [Frozen Architecture baseline](Architecture/ARCH-BASELINE-REVIEW.md)
- [Accepted UI Framework ADR](Architecture/adr/ADR-0002-ui-framework-selection.md)
- [Accepted Rendering Technology ADR](Architecture/adr/ADR-0003-rendering-technology.md)
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
| Rendering Technology | ADR-0003 `Accepted`; WinUI XAML／Composition + Win2D; runtime not verified |
| Capture Backend | Candidate; next primary decision |
| Clipboard、Image Representation、Testing | Candidate / not accepted |
| Interface contracts | Not completed |
| Language／Runtime／SDK versions | Not selected |
| Project structure | Not completed |
| Application code | Not started |
| Runtime verification | Not started |
| Build、test、CI and release | Not established |

## Current working direction

The repository no longer needs additional prerequisite or closure-review chains.

The next useful work is:

1. Produce and review the Capture Backend ADR using `docs/Research/Technology/20–28` as evidence.
2. Decide Image Representation、Clipboard Integration and Testing Strategy.
3. Define consolidated Shared Result、rendering、capture、clipboard、output and failure contracts.
4. Fix Language／Runtime／SDK versions and Solution／Project Structure.
5. Perform one Implementation Readiness Review.

`docs/Research/Technology/10–18` remains historical Rendering research and verification planning. ADR-0003 now owns the effective Rendering decision. `docs/Research/Technology/29–80` remains historical Clipboard research and governance evidence; the 039–052 documentary chain is closed.

## Working principle

Use Frozen PRD、Specification、Architecture and Accepted ADR baselines as upstream sources. Keep `Draft`、`Candidate`、`UNKNOWN` and `TBD` visible. ADR acceptance does not authorize coding; source-code creation requires an explicit implementation task after Implementation Readiness approval.
