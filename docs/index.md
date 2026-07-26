# SnipPlus 文件入口

這是 SnipPlus 的文件總入口。新成員先讀本頁，再依工作類型進入對應資料夾。

## 先讀這些

1. [Repository 入口](../README.md)
2. [產品基線](../PRD/PRD-0001-product-foundation.md)
3. [架構總覽](../Architecture/README.md)
4. [開發指南](guides/development-guide.md)
5. [協作規範](../CONTRIBUTING.md)

## 文件分區

### Product

- [PRD index](../PRD/README.md)
- [Product foundation](../PRD/PRD-0001-product-foundation.md)

### Research

- [Research framework](Research/README.md) — 研究定位、來源政策與文件模板。
- [Research methodology](Research/methodology.md)
- [Research source policy](Research/source-policy.md)
- [Research template](Research/template.md)
- [Comparison matrix](Research/comparison-matrix.md)
- [Research glossary](Research/glossary.md)
- [Product research entry points](Research/Win11/README.md)
- [UI framework feasibility research](Research/Technology/01-ui-framework-feasibility.md)
- [UI framework runtime spike plan](Research/Technology/02-ui-framework-runtime-spike-plan.md)
- [UI framework runtime spike execution readiness](Research/Technology/03-ui-framework-runtime-spike-execution-readiness.md)
- [UI framework runtime environment baseline](Research/Technology/04-ui-framework-runtime-environment-baseline.md)
- [UI framework runtime prerequisite closure plan](Research/Technology/05-ui-framework-runtime-prerequisite-closure-plan.md)
- [UI framework Phase 1 prerequisite closure record](Research/Technology/06-ui-framework-phase1-prerequisite-closure-record.md)
- [UI framework Phase 1 readiness reassessment](Research/Technology/07-ui-framework-phase1-readiness-reassessment.md)
- [UI framework Phase 1 execution enablement specification](Research/Technology/08-ui-framework-phase1-execution-enablement-specification.md)
- [UI framework Phase 1 enablement execution authorization request](Research/Technology/09-ui-framework-phase1-enablement-execution-authorization-request.md)

### Analysis

- [Analysis framework](Analysis/README.md) — 研究與產品決策之間的中立分析層。
- [Analysis template](Analysis/analysis-template.md)
- [Win11 analysis entry point](Analysis/Win11/README.md)
- [Win11 capture workflow analysis](Analysis/Win11/capture-workflow-analysis.md)

### Decision

- [Decision framework](Decision/README.md) — 將分析轉為可追蹤的採用判斷。
- [Decision template](Decision/decision-template.md)
- [Win11 decision entry point](Decision/Win11/README.md)
- [Win11 capture workflow decision](Decision/Win11/capture-workflow-decision.md)

### Specification

- [Specs index](../Specs/README.md)
- [Documentation baseline Spec](../Specs/SPEC-0001-documentation-baseline.md)
- [Specification Guidelines](../Specs/SPEC-0002-specification-guidelines.md)
- [System Requirements](../Specs/SPEC-0003-system-requirements.md)
- [Feature Catalog](../Specs/SPEC-0004-feature-catalog.md)
- [Capture Workflow Spec](../Specs/SPEC-0005-capture-workflow.md)
- [Workflow Boundaries and Feedback Spec](../Specs/SPEC-0006-workflow-boundaries-and-feedback.md)
- [Clipboard Handoff Spec](../Specs/SPEC-0007-clipboard-handoff.md)
- [Capture Output Spec](../Specs/SPEC-0008-capture-output.md)
- [Annotation Capability Spec](../Specs/SPEC-0009-annotation-capability.md)
- [Feature Integration Spec](../Specs/SPEC-0010-feature-integration.md)

### Architecture

- [Architecture index](../Architecture/README.md)
- [ARCH-0001 Architecture Principles](../Architecture/ARCH-0001-architecture-principles.md)
- [ARCH-0002 Layer Model](../Architecture/ARCH-0002-layer-model.md)
- [ARCH-0003 Module Catalog](../Architecture/ARCH-0003-module-catalog.md)
- [ARCH-0004 Component Boundaries](../Architecture/ARCH-0004-component-boundaries.md)
- [ARCH-0005 Component Interactions](../Architecture/ARCH-0005-component-interactions.md)
- [Architecture Baseline Review](../Architecture/ARCH-BASELINE-REVIEW.md)
- [System overview](../Architecture/system-overview.md)
- [Mermaid architecture diagram](../Architecture/architecture-diagram.md)
- [ADR index](../Architecture/adr/README.md)
- [ADR-0001: Documentation-first baseline](../Architecture/adr/ADR-0001-documentation-first.md)

### Guides and standards

- [Development Guide](guides/development-guide.md)
- [Coding Standard](guides/coding-standard.md)
- [Markdown naming rules](standards/markdown-naming.md)
- [UI Wireframe](design/ui-wireframe.md)

### Project management

- [ROADMAP](../ROADMAP.md)
- [TODO](../TODO.md)
- [CHANGELOG](../CHANGELOG.md)

## 文件狀態

文件狀態使用以下值：

- `Draft`：內容可供討論，尚未作為實作依據。
- `Proposal`：提出候選方案，等待產品或技術決策。
- `Accepted`：已核准，可作為對應範圍的來源文件。
- `Superseded`：已被新文件或新決策取代。
- `Archived`：保留歷史脈絡，不再更新。

目前 Repository 的產品與技術內容以 `Draft`、`Proposal` 為主；不要把它們當成已完成的 runtime 行為。

## 維護規則

- 新文件先更新對應的 index。
- 行為需求寫在 `PRD/` 或 `Specs/`，不要只寫在 `docs/`。
- 長期技術決策寫在 `Architecture/adr/`，並從架構入口連結。
- 所有 Markdown 命名遵守 [Markdown naming rules](standards/markdown-naming.md)。
