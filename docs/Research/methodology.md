# Research Methodology

狀態：`Accepted`

所有 Research 文件遵循同一個流程，讓不同產品、平台與研究者的內容可以比較、追溯與更新。

## Workflow

```text
Question
   ↓
Source discovery
   ↓
Verification
   ↓
Observation record
   ↓
Review
   ↓
引用至 PRD（只有在產品決策後）
```

## 1. Define the question

先寫清楚要確認的外部事實，不從 SnipPlus 的預設解法出發。問題應能用觀察或來源回答，例如「工具在取消選取後如何回到待機狀態」。

## 2. Collect sources

依 [source policy](source-policy.md) 記錄來源類型、標題、URL 或識別資訊、發布日期、存取日期與版本。找不到可靠來源時標示 `UNKNOWN`，不要用常識補齊。

## 3. Verify

每一項重要觀察至少標示一種確認方式：官方文件、官方影片、官方 release note、實際操作、可重現測試，或明確標為社群觀察。不同來源衝突時保留衝突，不自行選邊。

## 4. Record observations

使用 [template](template.md)。將「看到的行為」與「研究者推論」分開；推論必須標示 `Inference`，不能混入事實段落。

## 5. Review

Review 檢查來源、版本、日期、驗證步驟、未知項目與是否誤寫 SnipPlus 設計。Review 前的內容狀態為 `Draft`，通過後才可標為 `Accepted`。

## 6. Reference from PRD

PRD 引用 Research 時，連到具體文件與章節，並說明產品決策是否與外部行為相同、取捨或刻意不同。Research 更新後重新檢查所有引用的 PRD。

## Research status

- `Draft`：正在收集或尚未完成驗證。
- `Accepted`：來源、驗證與限制已經過 review。
- `Stale`：外部版本或日期已變，需重新確認。
- `Superseded`：由更新研究取代，但保留歷史紀錄。
