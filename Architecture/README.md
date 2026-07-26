# Architecture

狀態：`Draft`

本區描述 SnipPlus 的系統邊界、責任分層、資料流與長期技術決策。因為目前沒有應用程式碼、技術棧或部署環境，本架構是「可演進的基線」，不是已完成的實作圖。

## 文件清單

- [ARCH-0001 Architecture Principles](ARCH-0001-architecture-principles.md) — Architecture 治理原則，Stability `Draft`。
- [ARCH-0002 Layer Model](ARCH-0002-layer-model.md) — 抽象 Layer、依賴方向與 Layer responsibility，Stability `Draft`。
- [ARCH-0003 Module Catalog](ARCH-0003-module-catalog.md) — 抽象 Module、Feature-to-Module mapping 與依賴邊界，Stability `Draft`。
- [ARCH-0004 Component Boundaries](ARCH-0004-component-boundaries.md) — 抽象 Component Boundary、Module/Feature mapping 與 Shared State access boundary，Stability `Draft`。
- [ARCH-0005 Component Interactions](ARCH-0005-component-interactions.md) — 抽象 Component 互動方向、狀態轉移請求、Failure propagation 與禁止互動，Stability `Draft`。
- [Architecture Baseline Review](ARCH-BASELINE-REVIEW.md) — Architecture v1.0 完整性、一致性、追溯性、Readiness 與 Freeze Decision，Review Status `Draft`。
- [System overview](system-overview.md) — 現況、邊界、責任與約束。
- [Mermaid architecture diagram](architecture-diagram.md) — 目前基線與待定邊界的視覺化。
- [ADR index](adr/README.md) — Architecture Decision Records。

## 建議閱讀順序

1. 先讀 [PRD-0001](../PRD/PRD-0001-product-foundation.md) 了解產品狀態。
2. 再讀 [system overview](system-overview.md) 了解架構責任。
3. 需要理解決策原因時，查看 [ADR](adr/README.md)。

## Current architecture status

Architecture 目前已建立治理原則、抽象 Layer Model、Module Catalog 與 Component Boundary Catalog；Technology、Project 與 Implementation 仍未建立。

| Layer | Current state | Owner document |
| --- | --- | --- |
| Product intent | Draft | `PRD/` |
| User-visible behavior | Baseline only | `Specs/` |
| Presentation / UI | Wireframe proposal only | `docs/design/` |
| Application / domain / infrastructure | Not implemented | TBD |
| Persistence / external integrations | Not selected | TBD |
| Build / release / operations | Not established | `docs/guides/` + TBD |

任何新增實作都必須先說明它位於哪一層、依賴方向為何，以及如何由 Spec 驗收。
