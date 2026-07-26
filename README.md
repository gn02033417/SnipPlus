# SnipPlus

SnipPlus is an intentionally documentation-first product repository. It is
currently establishing the product language, requirements, architecture, and
development rules before application implementation begins.

目前 Repository 沒有應用程式程式碼、build 設定、測試或已核准的產品功能。專案名稱與使用者後續提到的截圖方向只能視為背景，不代表任何功能已經定義或完成。

## Start here

- [Documentation index](docs/index.md) — 文件入口與導覽。
- [Research framework](docs/Research/README.md) — 外部事實研究的格式與來源政策。
- [Analysis framework](docs/Analysis/README.md) — 將研究整理成可審查的流程與狀態分析。
- [Decision framework](docs/Decision/README.md) — 記錄採用判斷、證據、風險與開放問題。
- [Product foundation](PRD/PRD-0001-product-foundation.md) — 目前的產品基線與待確認事項。
- [Specifications](Specs/README.md) — Spec 狀態與建立規則。
- [Architecture overview](Architecture/README.md) — 系統邊界與目前架構狀態。
- [Development Guide](docs/guides/development-guide.md) — 從需求到實作的工作流程。
- [Contributing](CONTRIBUTING.md) — 文件與未來程式碼的協作規則。

## Repository map

```text
SnipPlus/
├─ Architecture/       系統架構與 ADR
├─ PRD/                產品需求與產品決策
├─ Specs/              可驗收的行為規格
├─ docs/               導覽、研究、分析、決策、指南、規範與設計草稿
├─ AGENTS.md           Repository 工作規則
├─ CONTRIBUTING.md     協作與變更流程
├─ ROADMAP.md          階段與出口條件
├─ CHANGELOG.md        可追蹤的變更紀錄
├─ TODO.md             未完成事項與開放決策
└─ README.md           Repository 入口
```

## Current status

| Area | Status |
| --- | --- |
| Product definition | Draft / discovery required |
| Research, Analysis and Decision | Research baseline established; Analysis and Decision in progress |
| Behavioral Specs | Documentation baseline only |
| Architecture | Baseline established; implementation boundaries are TBD |
| Application code | Not started |
| Screenshot functionality | Not started and intentionally out of scope for this task |

## Working principle

先確認問題、使用者與可驗收行為，再決定架構與實作。任何尚未確認的內容都必須明確標示，不以文件完整度假裝產品已經定案。
