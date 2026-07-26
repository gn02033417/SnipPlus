# PRD

產品需求文件定義「為誰解決什麼問題、為什麼做、做到哪裡」。它不描述類別、函式或資料表等實作細節。

## 文件清單

- [PRD-0001 Product foundation](PRD-0001-product-foundation.md) — 目前唯一的產品基線，狀態為 `Draft`。

## PRD 狀態

- `Draft`：可討論，不能直接作為開發承諾。
- `Proposal`：候選範圍或方案，等待決策。
- `Accepted`：已核准，可拆成 Specs。
- `Superseded`：被後續 PRD 取代。

## 建立規則

- 每份 PRD 使用 `PRD-NNNN-kebab-case.md` 命名。
- 先寫問題與成功條件，再寫解法方向。
- 未確認內容標示 `TBD`，候選內容標示 `Proposal`。
- PRD 變更若影響架構，必須同步檢查 Architecture 與 ADR。
