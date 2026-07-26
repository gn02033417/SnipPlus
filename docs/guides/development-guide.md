# Development Guide

狀態：`Draft`

本指南定義 SnipPlus 從需求到實作的長期工作流程。目前 Repository 尚未進入應用程式開發階段，因此環境指令、技術棧與發布流程仍是 `TBD`。

## 1. 變更前先判斷文件層級

| 問題 | 應更新的文件 |
| --- | --- |
| 外部世界實際如何運作？ | `docs/Research/` |
| 研究結果的流程與狀態如何整理？ | `docs/Analysis/` |
| 是否採用某個已分析的 workflow？ | `docs/Decision/` |
| 為誰解決什麼問題？ | `PRD/` |
| 使用者可觀察到什麼行為？ | `Specs/` |
| 系統如何分層、整合與部署？ | `Architecture/` |
| 為什麼選這個長期方案？ | `Architecture/adr/` |
| 團隊如何執行與協作？ | `docs/`、`CONTRIBUTING.md` |

不要用 Architecture 取代 PRD，也不要把未核准的 UI 草稿當成 Spec。Decision 只記錄採用判斷，不取代 PRD。

## 2. 建議工作流程

1. 先記錄外部事實於 Research。
2. 將已驗證的流程與狀態整理於 Analysis。
3. 在 Decision 記錄採用、拒絕或部分採用的判斷，保留理由、證據、風險與開放問題。
4. 在 PRD 建立範圍與成功條件；未確認內容標示 `TBD`。
5. 把已核准的使用者行為拆成可驗收的 Spec。
6. 更新架構邊界、資料流、失敗處理與非功能需求。
7. 遇到難以回復或有明顯取捨的技術選擇，新增 ADR。
8. PRD、Spec 與 Architecture 對齊後才開始實作。
9. 實作完成後補測試、文件與 Changelog，並做與變更風險相稱的驗證。

## 3. 目前已知限制

- 尚未選定語言、UI framework、作業系統支援範圍或儲存方式。
- 尚未建立 build、test、lint、package 或 deploy 指令。
- 不得從空白 Repository 推測 runtime 行為。
- 截圖功能目前只允許在產品文件中討論，不在本文件整理階段實作。

## 4. 文件變更檢查

文件變更至少確認：

- 內容有正確的狀態標記。
- H1、章節階層與檔名符合規則。
- 新增與改名文件已加入 index。
- 相對連結指向存在的文件。
- `PRD/`、`Specs/`、`Architecture/` 之間沒有互相矛盾。
- Changelog 與 TODO 反映此次變更。

## 5. 未來實作的最低品質門檻

在技術棧確定後，至少建立：

- 可重現的本機 setup 指令。
- 可單獨執行的 format、lint、test 與 build 指令。
- CI 對應的檢查項目。
- 失敗時可定位的 logging 與錯誤處理。
- release、rollback 與資料相容性說明。

實際指令應在技術棧確定後補入本文件，不先寫猜測指令。
