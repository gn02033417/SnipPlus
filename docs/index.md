# SnipPlus 文件入口

這是 SnipPlus 的文件總入口。新成員先讀目前狀態稽核，再依工作類型進入對應資料夾。

## 先讀這些

1. [Repository 入口](../README.md)
2. [Repository current-state and implementation-readiness audit](REPOSITORY-CURRENT-STATE-AND-IMPLEMENTATION-READINESS-AUDIT.md)
3. [Project lifecycle](PROJECT-LIFECYCLE.md)
4. [PRD Freeze Review](../PRD/PRD-FREEZE-REVIEW.md)
5. [Specification Baseline Review](../Specs/SPEC-BASELINE-REVIEW.md)
6. [Architecture Baseline Review](../Architecture/ARCH-BASELINE-REVIEW.md)
7. [ADR-0002 UI Framework Selection](../Architecture/adr/ADR-0002-ui-framework-selection.md)
8. [Technology Decision Roadmap](../Architecture/TECHNOLOGY-DECISION-ROADMAP.md)
9. [Development Guide](guides/development-guide.md)
10. [Contributing](../CONTRIBUTING.md)

## Current lifecycle state

| Area | Current state |
| --- | --- |
| PRD v1.0 | Freeze Approved |
| Specification v1.0 | Freeze Approved |
| Architecture baseline | Freeze Approved |
| UI Framework decision | ADR-0002 Accepted; WinUI 3 |
| Rendering Technology | Candidate; next primary decision |
| Other core technology decisions | Candidate |
| Contracts and project structure | Incomplete |
| Implementation and verification | Not started |

## 文件分區

### Product

- [PRD index](../PRD/README.md)
- [PRD Freeze Review](../PRD/PRD-FREEZE-REVIEW.md)
- [PRD Traceability Matrix](../PRD/PRD-TRACEABILITY-MATRIX.md)

### Research

- [Research framework](Research/README.md)
- [Research methodology](Research/methodology.md)
- [Research source policy](Research/source-policy.md)
- [Research template](Research/template.md)
- [Comparison matrix](Research/comparison-matrix.md)
- [Research glossary](Research/glossary.md)
- [Win11 research](Research/Win11/README.md)
- [Technology Research index](Research/Technology/README.md) — UI Framework 01–09、Rendering 10–18、Capture Backend 20–28、Clipboard Integration 29–80。

Technology Research 是 evidence 與治理歷史，不是 Accepted technical decision。Clipboard D1 039→052 documentary chain 已停止，不得自動建立第 81 項同類 closure 文件。

### Analysis

- [Analysis framework](Analysis/README.md)
- [Analysis template](Analysis/analysis-template.md)
- [Win11 analysis](Analysis/Win11/README.md)
- [Win11 capture workflow analysis](Analysis/Win11/capture-workflow-analysis.md)

### Decision

- [Decision framework](Decision/README.md)
- [Decision template](Decision/decision-template.md)
- [Win11 decision](Decision/Win11/README.md)
- [Win11 capture workflow decision](Decision/Win11/capture-workflow-decision.md)

### Specification

- [Specs index](../Specs/README.md)
- [Specification Guidelines](../Specs/SPEC-0002-specification-guidelines.md)
- [System Requirements](../Specs/SPEC-0003-system-requirements.md)
- [Feature Catalog](../Specs/SPEC-0004-feature-catalog.md)
- [Capture Workflow](../Specs/SPEC-0005-capture-workflow.md)
- [Workflow Boundaries and Feedback](../Specs/SPEC-0006-workflow-boundaries-and-feedback.md)
- [Clipboard Handoff](../Specs/SPEC-0007-clipboard-handoff.md)
- [Capture Output](../Specs/SPEC-0008-capture-output.md)
- [Annotation Capability](../Specs/SPEC-0009-annotation-capability.md)
- [Feature Integration](../Specs/SPEC-0010-feature-integration.md)
- [Specification Baseline Review](../Specs/SPEC-BASELINE-REVIEW.md)

### Architecture and ADR

- [Architecture index](../Architecture/README.md)
- [ARCH-0001 Architecture Principles](../Architecture/ARCH-0001-architecture-principles.md)
- [ARCH-0002 Layer Model](../Architecture/ARCH-0002-layer-model.md)
- [ARCH-0003 Module Catalog](../Architecture/ARCH-0003-module-catalog.md)
- [ARCH-0004 Component Boundaries](../Architecture/ARCH-0004-component-boundaries.md)
- [ARCH-0005 Component Interactions](../Architecture/ARCH-0005-component-interactions.md)
- [Architecture Baseline Review](../Architecture/ARCH-BASELINE-REVIEW.md)
- [Technology Decision Roadmap](../Architecture/TECHNOLOGY-DECISION-ROADMAP.md)
- [ADR baseline](../Architecture/ADR-BASELINE.md)
- [ADR index](../Architecture/adr/README.md)
- [ADR-0001 Documentation-first](../Architecture/adr/ADR-0001-documentation-first.md)
- [ADR-0002 UI Framework Selection](../Architecture/adr/ADR-0002-ui-framework-selection.md) — Accepted; WinUI 3。

### Guides, standards and design

- [Development Guide](guides/development-guide.md)
- [Coding Standard](guides/coding-standard.md)
- [Markdown naming rules](standards/markdown-naming.md)
- [UI Wireframe](design/ui-wireframe.md)

### Project management

- [ROADMAP](../ROADMAP.md)
- [TODO](../TODO.md)
- [CHANGELOG](../CHANGELOG.md)
- [Repository readiness audit](REPOSITORY-CURRENT-STATE-AND-IMPLEMENTATION-READINESS-AUDIT.md)

## 文件狀態

- `Draft`：內容可供討論，尚未成為有效決策或 implementation contract。
- `Proposal`／`Candidate`：候選方向，等待決策前置條件或 Review。
- `Accepted`：已核准，可由下游引用。
- `Freeze Approved`：對應文件集合已形成固定基線，但不等於 coding authorization。
- `Superseded`：已被新文件或新決策取代。
- `Archived`：保留歷史脈絡，不再更新。

## 維護規則

- 新文件必須更新對應 index。
- 狀態改變時同步更新 README、ROADMAP、TODO 與 CHANGELOG。
- 行為需求寫在 PRD 或 Specs，不要只寫在 Research。
- 長期技術決策寫在 ADR；Draft ADR 不得當成 Accepted decision。
- 沒有新 evidence 或 decision 時，不建立新的 prerequisite／closure 文件。
- 所有 Markdown 命名遵守 [Markdown naming rules](standards/markdown-naming.md)。
