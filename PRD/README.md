# PRD

產品需求文件定義「為誰解決什麼問題、為什麼做、做到哪裡」。它不描述類別、函式、API 或資料表等實作細節。

## Formal PRD v1.0 baseline

PRD v1.0 已完成 Freeze Review，正式 baseline 為：

- [PRD-0002 User Experience Principles](PRD-0002-user-experience-principles.md)
- [PRD-0003 Product Vision](PRD-0003-product-vision.md)
- [PRD-0004 Core Workflow](PRD-0004-core-workflow.md)
- [PRD-0005 Functional Requirements](PRD-0005-functional-requirements.md)
- [PRD-0006 Non-functional Requirements](PRD-0006-non-functional-requirements.md)
- [PRD Baseline Review](PRD-BASELINE-REVIEW.md)
- [PRD Traceability Matrix](PRD-TRACEABILITY-MATRIX.md)
- [PRD Freeze Review](PRD-FREEZE-REVIEW.md)

Freeze Decision：`Freeze Approved`。

Freeze Approved 表示上述集合可作為 Specification 的固定產品來源；它不表示 implementation、runtime verification 或 release 已完成。

## Initial discovery artifact

- [PRD-0001 Product Foundation](PRD-0001-product-foundation.md) — 初始 Repository discovery 文件，狀態為 `Draft`。

PRD-0001 保留早期脈絡，但不是正式 PRD v1.0 baseline，也不得再被稱為「目前唯一的產品基線」。其內容若與 Frozen PRD v1.0 不一致，以 Frozen baseline 為準。

## PRD 狀態

- `Draft`：可討論，不能直接作為新開發承諾。
- `Proposal`：候選範圍或方案，等待決策。
- `Accepted`：單一文件已核准。
- `Freeze Approved`：一組 PRD 文件形成固定版本 baseline。
- `Superseded`：被後續 PRD 或 change decision 取代。

## Change rule

PRD v1.0 Freeze 後，新增需求、scope、priority 或產品行為必須走：

```text
Research → Analysis → Decision → PRD Change Request → Review → Approve
```

不得由 Specs、Architecture、ADR 或 Implementation 反向偷偷加入產品需求。
