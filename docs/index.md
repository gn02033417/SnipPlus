# SnipPlus 文件入口

## 現行規範來源

依序閱讀：

1. [Repository rules](../AGENTS.md)
2. [PRD index](../PRD/README.md)
3. [PRD-0004 Core Workflow](../PRD/PRD-0004-core-workflow.md)
4. [PRD-0005 Functional Requirements](../PRD/PRD-0005-functional-requirements.md)
5. [PRD-0006 Non-functional and Quality Requirements](../PRD/PRD-0006-non-functional-requirements.md)
6. [Specs index](../Specs/README.md)
7. [Implementation Contracts](../Architecture/IMPLEMENTATION-CONTRACTS.md)
8. [Project Structure and Toolchain](../Architecture/PROJECT-STRUCTURE.md)
9. [Requirements-to-Code Conformance Matrix](../PRD/PRD-TRACEABILITY-MATRIX.md)
10. [Implementation Readiness Review](IMPLEMENTATION-READINESS-REVIEW.md)

Research、Analysis、Decision、早期 Freeze Review 與先前 vertical-slice evidence 只作為歷史背景；與現行 Accepted PRD／Specs 衝突時不得採用。

## Current Lifecycle State

| Area | Current state |
| --- | --- |
| Product baseline | Accepted complete v1 quality baseline |
| Specification baseline | Accepted current revision |
| Architecture baseline | Accepted current revision |
| ADR-0002 through ADR-0007 | Accepted |
| Implementation Contracts | Accepted v2.2 |
| Conformance Matrix | Reviewed v2.2 |
| Solution／projects | Present |
| Technical prototype | Single-display capture／crop／Clipboard foundation present |
| Product conformance | Correction required |
| Visible product decisions | All resolved |
| Current activity | Begin only through an explicit task following the correction order |
| Release | Not released |

## Finalized Quality Baseline

### Performance

- Capture start p95 `≤ 500 ms` Standard、`≤ 1,000 ms` Maximum.
- Interaction p95 `≤ 33 ms` frame time and `≤ 100 ms` visible response.
- Complete／Save size-tiered targets、`300 ms` progress threshold and memory limits are defined in PRD-0006.

### Capacity

- `1`–`4` logical displays.
- Each `≤ 7,680 × 4,320`.
- Total source pixels `≤ 66,355,200`.
- Virtual Desktop width／height each `≤ 16,384`.
- Selection area `≤ 67,108,864` pixels with dimensional limits.

### Keyboard-only Editing

- Scope begins at `SelectionLocked`.
- Complete function-bar、tool、object、style、Undo／Redo、Save、Complete and Cancel workflow works without pointer input.
- Initial crosshair Selection remains pointer-driven in v1.

## Product and Behavior

- [PRD index](../PRD/README.md)
- [Core Workflow](../PRD/PRD-0004-core-workflow.md)
- [Functional Requirements](../PRD/PRD-0005-functional-requirements.md)
- [Non-functional Requirements](../PRD/PRD-0006-non-functional-requirements.md)
- [Requirements-to-Code Conformance Matrix](../PRD/PRD-TRACEABILITY-MATRIX.md)
- [Specs index](../Specs/README.md)

## Architecture and Implementation

- [Architecture index](../Architecture/README.md)
- [Architecture overview](../Architecture/system-overview.md)
- [Architecture diagram](../Architecture/architecture-diagram.md)
- [Implementation Contracts](../Architecture/IMPLEMENTATION-CONTRACTS.md)
- [Project Structure](../Architecture/PROJECT-STRUCTURE.md)
- [ADR index](../Architecture/adr/README.md)
- [Technology Decision Roadmap](../Architecture/TECHNOLOGY-DECISION-ROADMAP.md)
- [Implementation Readiness Review](IMPLEMENTATION-READINESS-REVIEW.md)

## Design and Development

- [Current v1 UI wireframe](design/ui-wireframe.md)
- [Development Guide](guides/development-guide.md)
- [Coding Standard](guides/coding-standard.md)
- [ROADMAP](../ROADMAP.md)
- [TODO](../TODO.md)
- [CHANGELOG](../CHANGELOG.md)

## Historical Evidence

- [Research framework](Research/README.md)
- [Technology Research](Research/Technology/README.md)
- [Analysis framework](Analysis/README.md)
- [Decision framework](Decision/README.md)
- [Repository audit](REPOSITORY-CURRENT-STATE-AND-IMPLEMENTATION-READINESS-AUDIT.md)

Do not extend historical Research through repetitive prerequisite、authorization or closure documents.

## Current Next Action

The next explicit implementation task begins with:

```text
resident lifecycle
→ MainWindow X directly exits
→ no close-to-tray behavior
→ user-controlled PrintScreen takeover
→ exact takeover release on disable and exit
```

It must stop before the next correction slice unless separately authorized. It must not begin with Annotation、Clipboard hardening、Packaging or unrelated feature expansion.