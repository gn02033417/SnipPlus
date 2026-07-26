# Decision Framework

狀態：`Accepted`

`Decision/` 是 `Analysis/` 與 `PRD/` 之間的決策紀錄層。它回答「某個已研究、已分析的 workflow 或行為是否採用」，並保留理由、證據、風險與開放問題。它不取代 PRD，也不描述 UI 或工程實作。

## 文件生命週期

```text
Research  ->  Analysis  ->  Decision  ->  PRD  ->  Specs  ->  Architecture  ->  Coding
事實          分析          採用判斷      產品決策    行為契約     技術邊界          實作
```

- `Research/`：記錄有來源的外部事實。
- `Analysis/`：整理研究中的流程、狀態、意圖、依賴與未知邊界。
- `Decision/`：記錄是否採用，以及判斷的理由與限制。
- `PRD/`：定義 SnipPlus 的產品問題、範圍、目標與 non-goals。
- `Specs/`：定義可驗收的使用者可觀察行為。

## Decision values

每一個判斷只能使用以下值：

- `YES`：採用該 workflow 或行為層級概念。
- `NO`：不採用該 workflow 或行為層級概念。
- `PARTIAL`：只採用明確說出的部分，未決部分保持開放。
- `UNKNOWN`：證據不足，不能做可靠判斷。

這些值不代表 UI、技術棧、API 或 implementation 已決定。

## Required fields

每個 Decision 必須包含：

- `Decision`
- `Reason`
- `Evidence`
- `Risk`
- `Open Question`

證據必須連回 Analysis 或 Research；未知內容保留為 `UNKNOWN`，不能自行補完。

## Current decisions

- [Win11 decision entry point](Win11/README.md)
