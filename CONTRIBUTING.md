# Contributing to SnipPlus

狀態：`Accepted`

## 先理解文件來源

開始變更前，先讀：

1. [README](README.md)
2. [文件入口](docs/index.md)
3. [AGENTS.md](AGENTS.md)
4. 與變更直接相關的 PRD、Spec 或 Architecture 文件

## 變更流程

1. 說明變更目的與來源。
2. 判斷變更屬於 PRD、Spec、Architecture、ADR、Guide 或 project tracking。
3. 做最小範圍的修改，保留未確認內容的狀態標記。
4. 同步更新 index、`CHANGELOG.md`、`TODO.md` 或相關交叉連結。
5. 做 Markdown、連結、命名、狀態與 diff 的靜態檢查。
6. 在 PR 描述中清楚區分文件變更與 runtime 驗證；沒有執行就不要宣稱通過。

## 文件要求

- 使用台灣繁體中文描述產品與流程；保留 API、path、code 與正式技術名稱。
- 每份文件有唯一 H1，並在未定案時標示 `Draft` 或 `Proposal`。
- 使用 [Markdown naming rules](docs/standards/markdown-naming.md)。
- 需求寫行為與結果，不先寫不必要的 implementation detail。
- 新增 ADR 前確認這是需要長期保存的決策，而不是一般偏好。

## 目前階段的禁止事項

- 未經明確授權，不新增截圖功能或其他應用程式碼。
- 不新增未經需求支持的 dependency、平台支援或外部服務。
- 不把候選 Wireframe、PRD 或 Architecture 草稿當成已發布功能。

## Commit 與 Pull Request

目前未強制特定 commit tool。Commit message 應簡短、使用動詞並描述單一目的，例如：

```text
docs: establish product and architecture baseline
```

Pull Request 至少說明：變更目的、影響文件、狀態變更、靜態檢查結果、尚未驗證項目，以及是否修改應用程式碼。
