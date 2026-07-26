# Architecture

狀態：`Accepted baseline / Draft maturity`

本區描述 SnipPlus 的抽象系統邊界、責任分層、ownership、dependency 與長期技術決策。Architecture baseline 已 Freeze Approved；UI Framework 已 Accepted；其餘核心技術選擇、contracts、project structure 與 runtime evidence 尚未完成。

## Architecture v1.0 baseline

- [ARCH-0001 Architecture Principles](ARCH-0001-architecture-principles.md)
- [ARCH-0002 Layer Model](ARCH-0002-layer-model.md)
- [ARCH-0003 Module Catalog](ARCH-0003-module-catalog.md)
- [ARCH-0004 Component Boundaries](ARCH-0004-component-boundaries.md)
- [ARCH-0005 Component Interactions](ARCH-0005-component-interactions.md)
- [Architecture Baseline Review](ARCH-BASELINE-REVIEW.md)

Freeze Decision：`Freeze Approved`。
Readiness：`Ready for ADR and Technology Selection`。

Architecture Stability 仍為 `Draft`，表示成熟度尚未宣告 Stable；不否定 v1.0 baseline 的 Freeze Decision。

## Decision and navigation documents

- [System overview](system-overview.md)
- [Mermaid architecture diagram](architecture-diagram.md)
- [ADR Baseline](ADR-BASELINE.md)
- [Technology Decision Roadmap](TECHNOLOGY-DECISION-ROADMAP.md)
- [ADR index](adr/README.md)
- [Accepted UI Framework ADR](adr/ADR-0002-ui-framework-selection.md)
- [Repository readiness audit](../docs/REPOSITORY-CURRENT-STATE-AND-IMPLEMENTATION-READINESS-AUDIT.md)

## 建議閱讀順序

1. [PRD Freeze Review](../PRD/PRD-FREEZE-REVIEW.md)
2. [Specification Baseline Review](../Specs/SPEC-BASELINE-REVIEW.md)
3. [Architecture Baseline Review](ARCH-BASELINE-REVIEW.md)
4. [ADR-0002 UI Framework Selection](adr/ADR-0002-ui-framework-selection.md)
5. [Technology Decision Roadmap](TECHNOLOGY-DECISION-ROADMAP.md)
6. [ADR index](adr/README.md)

## Fixed architecture boundaries

- Product Workflow → Feature Coordination → Domain Capability → Platform Integration。
- COMP-001 是唯一 Shared State Authority。
- Annotation 維持 Optional。
- Clipboard 與 Output 維持平行 downstream。
- Platform-specific behavior 必須隔離在 Platform Integration boundary。
- Implementation 不得直接改寫 Feature、Module、Component 或 Interaction ownership。

## Accepted technology boundary

| Area | Accepted decision | Scope boundary |
| --- | --- | --- |
| Desktop UI Framework | WinUI 3 through ADR-0002 | UI host only; does not select Language／Runtime、Windows App SDK version、Rendering、Capture、Clipboard、Packaging、Testing or Project Structure |

## Remaining engineering gaps

| Area | Current state | Required next stage |
| --- | --- | --- |
| Rendering Technology | Candidate | TD-002 Rendering ADR; next primary decision |
| Capture Backend | Candidate | TD-003 Capture Backend ADR |
| Clipboard Integration | Candidate | TD-004 Clipboard Integration ADR |
| Image Representation | Candidate | TD-005 Image Representation ADR／contract |
| Testing Strategy | Candidate | TD-011 Testing Strategy ADR |
| Shared Result contract | TBD | Contract package |
| Failure and retry semantics | TBD | Contract review |
| Clipboard／Output completion semantics | TBD | ADR or contract review |
| Component interaction sync／async | TBD | ADR |
| Component-to-project mapping | TBD | Project Structure |
| Runtime verification | Not performed | Verification stage |

## Current architecture status

| Layer | Current state |
| --- | --- |
| Product intent | Frozen PRD baseline |
| User-visible behavior | Frozen Specification baseline |
| Abstract architecture | Freeze Approved |
| UI framework | WinUI 3 Accepted |
| Remaining technology selection | In progress |
| Interface and data contracts | Incomplete |
| Project / assembly mapping | Incomplete |
| Application implementation | Not started |
| Build / test / runtime verification | Not established |

任何新增實作都必須先說明它位於哪一層、對應哪個 Module／Component、依賴方向為何，以及如何由 Frozen Spec 與 Accepted ADR 驗收。
