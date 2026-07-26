# ADR-0001 Documentation-first baseline

- Status: `Accepted`
- Date: `2026-07-26`
- Deciders: Repository maintainers

## Context

SnipPlus Repository 只有空白的文件骨架，沒有應用程式碼、技術棧、build 流程、測試或已核准的產品需求。同時，本階段明確要求先整理 Repository 與文件，不開始撰寫截圖功能。

如果直接開始實作，產品範圍、責任分層、資料隱私與驗收方式都會靠推測，後續很容易產生不可追溯的程式碼與文件。

## Decision

先建立 documentation-first baseline：

1. 以 `PRD/` 定義產品意圖與範圍。
2. 以 `Specs/` 定義可觀察、可驗收行為。
3. 以 `Architecture/` 定義邊界與責任。
4. 以 `Architecture/adr/` 保存有長期取捨的技術決策。
5. 以 `docs/` 管理導覽、工作流程、命名與品質標準。
6. 在 PRD 與核心 Specs 核准前，不新增應用程式碼或功能實作。

所有未確認內容必須標示 `TBD`、`Proposal` 或 `Assumption`。

## Alternatives considered

### 先建立最小程式原型

拒絕。原型可能快速展示畫面，但會把未確認的產品假設固化成結構，且目前沒有可驗收的目標。

### 只建立一份 README

拒絕。單一文件無法清楚分離產品需求、行為契約、架構理由與協作規則，長期會變成無法維護的混合文件。

### 先建立完整技術架構與技術棧

拒絕。產品平台、使用者與資料生命週期尚未確認，現在固定具體技術會製造不必要的返工。

## Consequences

### Positive

- 需求、規格、架構與決策有明確責任與追溯路徑。
- 未知內容可被看見，不會被誤當成已完成行為。
- 未來開始實作時，有一致的文件入口與品質門檻。

### Negative

- 短期內不會產生可執行的產品功能。
- 需要先完成 product discovery，才能把候選範圍轉為正式 Specs。
- 技術棧與部署細節暫時維持 `TBD`。

## Related documents

- [Product foundation](../../PRD/PRD-0001-product-foundation.md)
- [System overview](../system-overview.md)
- [Development Guide](../../docs/guides/development-guide.md)
- [Documentation baseline Spec](../../Specs/SPEC-0001-documentation-baseline.md)
