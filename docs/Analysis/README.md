# Analysis Framework

狀態：`Accepted`

`Analysis/` 是 `Research/` 與 `Decision/` 之間的中立整理層。它把已存在的研究資料整理成流程、狀態、使用者意圖、系統意圖、依賴與失敗邊界，方便後續審查；它不替 SnipPlus 做產品選擇。

## 文件責任

```text
Research  ->  Analysis  ->  Decision  ->  PRD  ->  Specs  ->  Architecture  ->  Coding
事實          分析          採用判斷      產品決策    行為契約     技術邊界          實作
```

- `Research/`：記錄有來源的外部事實與目前可觀察行為。
- `Analysis/`：只解釋研究內容的結構與邊界，不添加新功能或方案。
- `Decision/`：在 PRD 前記錄採用、拒絕或部分採用的判斷與其依據。
- `PRD/`：在審查後決定產品問題、目標、範圍與 non-goals。
- `Specs/`：把核准的產品行為寫成可驗收的契約。

## Analysis 可以回答

- 一個已研究流程有哪些步驟與狀態轉換？
- 每個狀態的 `Entry`、`Exit`、`Trigger`、`Dependency`、`Failure` 是什麼？
- 使用者意圖與系統意圖如何從研究資料中分開描述？
- 哪些環節有證據，哪些仍是 `UNKNOWN`？

## Analysis 不可以回答

- SnipPlus 應該新增什麼功能。
- SnipPlus 應該採用什麼 UI、技術或產品策略。
- 某個 workflow 是否已被 SnipPlus 正式採用；這要記錄在 `Decision/`。
- 哪個競品較好，或應該複製哪個競品。
- PRD、Spec、UX wireframe 或 implementation plan 的內容。

## Required format

- [Analysis template](analysis-template.md)
- 每份分析文件必須連回來源 Research 文件。
- 每個未被來源確認的欄位標為 `UNKNOWN`、`TBD` 或 `Assumption`。
- 分析文件狀態使用 `Draft`，直到完成產品審查；`Accepted` 只表示框架或格式本身已核准，不表示產品決策已核准。

## Current analyses

- [Win11 analysis entry point](Win11/README.md)
