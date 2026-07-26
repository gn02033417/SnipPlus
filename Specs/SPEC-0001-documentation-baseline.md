# SPEC-0001 Documentation Baseline

狀態：`Accepted`

## 1. 目的

定義 SnipPlus 文件 Repository 在產品實作開始前必須具備的最小可維護基線。

## 2. Scope

本 Spec 涵蓋文件入口、PRD、Specs、Architecture、ADR、指南、協作文件、roadmap、changelog 與 todo 的責任分界。

本 Spec 不定義任何截圖或其他應用程式功能。

## 3. Requirements

### DOC-001 文件入口

使用者可以從根目錄 `README.md` 或 `docs/index.md` 找到所有主要文件區域。

### DOC-002 需求與規格分離

產品目標與範圍放在 `PRD/`；可驗收行為放在 `Specs/`；兩者不得只靠口頭同步。

### DOC-003 架構與決策可追溯

系統邊界與技術責任放在 `Architecture/`；有長期取捨的決策必須在 `Architecture/adr/` 有可連結的 ADR。

### DOC-004 未知內容可辨識

任何尚未確認的內容都使用 `TBD`、`Proposal` 或 `Assumption` 標示，不能用肯定語氣偽裝成已核准需求。

### DOC-005 命名一致

所有新 Markdown 文件遵守 [Markdown naming rules](../docs/standards/markdown-naming.md)，並更新對應 index。

### DOC-006 變更可追蹤

對使用者或維護流程有意義的文件變更更新根目錄 `CHANGELOG.md`；未完成事項更新 `TODO.md` 或來源文件的 open decisions。

## 4. Acceptance checklist

- `README.md` 與 `docs/index.md` 都能導向主要文件。
- PRD、Specs、Architecture 與 ADR 各有入口。
- 文件檔名可由規則推導，且沒有 `Final`、`Latest` 或日期版號。
- 目前沒有應用程式碼或截圖功能實作。
