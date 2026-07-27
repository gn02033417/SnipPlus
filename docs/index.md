# SnipPlus 文件入口

## 現行規範來源

依序閱讀：

1. [Repository rules](../AGENTS.md)
2. [PRD index](../PRD/README.md)
3. [PRD-0004 Core Workflow](../PRD/PRD-0004-core-workflow.md)
4. [PRD-0005 Functional Requirements](../PRD/PRD-0005-functional-requirements.md)
5. [PRD-0006 Non-functional Requirements](../PRD/PRD-0006-non-functional-requirements.md)
6. [Specs index](../Specs/README.md)
7. [Implementation Contracts](../Architecture/IMPLEMENTATION-CONTRACTS.md)
8. [Project Structure and Toolchain](../Architecture/PROJECT-STRUCTURE.md)
9. [Requirements-to-Code Conformance Matrix](../PRD/PRD-TRACEABILITY-MATRIX.md)
10. [Implementation Readiness Review](IMPLEMENTATION-READINESS-REVIEW.md)

Research、Analysis、Decision、早期 Freeze Review 與先前 vertical-slice evidence 只作為歷史背景；與現行 Accepted PRD／Specs 衝突時不得採用。

## Current lifecycle state

| Area | Current state |
| --- | --- |
| Product baseline | Accepted v1.1 |
| Specification baseline | Accepted current revision |
| Architecture baseline | Accepted current revision |
| ADR-0002 through ADR-0007 | Accepted |
| Implementation Contracts | Accepted v2.0 |
| Solution／projects | Present |
| Technical prototype | Single-display capture／crop／Clipboard foundation present |
| Product conformance | Correction required |
| Current activity | Follow the conformance matrix correction order |
| Release | Not released |

## Product and behavior

- [PRD index](../PRD/README.md)
- [Core Workflow](../PRD/PRD-0004-core-workflow.md)
- [Functional Requirements](../PRD/PRD-0005-functional-requirements.md)
- [Non-functional Requirements](../PRD/PRD-0006-non-functional-requirements.md)
- [Requirements-to-Code Conformance Matrix](../PRD/PRD-TRACEABILITY-MATRIX.md)
- [Specs index](../Specs/README.md)

## Architecture and implementation

- [Architecture index](../Architecture/README.md)
- [Architecture overview](../Architecture/system-overview.md)
- [Architecture diagram](../Architecture/architecture-diagram.md)
- [Implementation Contracts](../Architecture/IMPLEMENTATION-CONTRACTS.md)
- [Project Structure](../Architecture/PROJECT-STRUCTURE.md)
- [ADR index](../Architecture/adr/README.md)
- [Technology Decision Roadmap](../Architecture/TECHNOLOGY-DECISION-ROADMAP.md)
- [Implementation Readiness Review](IMPLEMENTATION-READINESS-REVIEW.md)

## Design and development

- [Current v1 UI wireframe](design/ui-wireframe.md)
- [Development Guide](guides/development-guide.md)
- [Coding Standard](guides/coding-standard.md)
- [ROADMAP](../ROADMAP.md)
- [TODO](../TODO.md)
- [CHANGELOG](../CHANGELOG.md)

## Historical evidence

- [Research framework](Research/README.md)
- [Technology Research](Research/Technology/README.md)
- [Analysis framework](Analysis/README.md)
- [Decision framework](Decision/README.md)
- [Repository audit](REPOSITORY-CURRENT-STATE-AND-IMPLEMENTATION-READINESS-AUDIT.md)

Do not extend historical Research through repetitive prerequisite、authorization or closure documents.

## Current next action

The next implementation task begins with the first unresolved prerequisite in `PRD-TRACEABILITY-MATRIX-001`: resident lifecycle and user-controlled PrintScreen takeover. It must not begin with Annotation、Clipboard hardening、Packaging or unrelated feature expansion.
