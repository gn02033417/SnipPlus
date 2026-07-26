# PRD-0001 Product Foundation

狀態：`Draft`

## 1. 文件目的

建立 SnipPlus 在正式進入產品開發前的共同基線。由於目前 Repository 沒有既有產品需求或應用程式碼，本文件只確認已知事實、限制與需要決策的問題，不虛構功能承諾。

## 2. 已知事實

- Repository 名稱為 SnipPlus。
- 目前只有文件骨架，沒有 runtime、build、test 或 deploy 流程。
- 本階段要求先完成 Repository 與文件治理，不開始撰寫截圖功能。
- UI Wireframe 可用來討論方向，但尚未成為實作契約。

## 3. Product hypothesis

`Proposal`：SnipPlus 可能是協助使用者擷取或處理螢幕內容的工具。這只是由專案名稱與目前任務推導出的候選方向，尚未核准，不得直接轉成產品承諾。

## 4. Problem statement

目前尚未確認：

- 目標使用者是誰。
- 使用者在什麼情境下遇到什麼問題。
- 現有工具的不足為何。
- SnipPlus 的不可取代價值是什麼。

在上述問題釐清前，任何具體功能、平台、格式、保存或分享策略都維持 `TBD`。

## 5. Goals

### 本階段目標

- 建立可長期維護的文件入口與命名規則。
- 讓產品需求、行為規格、架構與技術決策各有明確責任。
- 讓未來的實作者能從文件判斷什麼已核准、什麼仍待決定。

### 未來產品目標

`TBD`。需在產品 discovery 後補上可測量且能驗證的目標。

## 6. Non-goals

- 本階段不撰寫截圖、擷取、編輯、儲存或分享功能。
- 本階段不選定技術棧、平台、資料格式或部署方式。
- 不將 Wireframe 視為視覺設計稿或完成的互動規格。
- 不用文件數量取代使用者研究與產品決策。

## 7. Candidate scope

以下只是討論用候選範圍，不是已核准的 backlog：

| Candidate | 說明 | 狀態 |
| --- | --- | --- |
| Start a primary capture-like task | 從單一入口開始主要任務 | Proposal |
| Select an area or source | 指定主要操作範圍或來源 | Proposal |
| Review a result | 查看主要任務產生的結果 | Proposal |
| Deliver or save a result | 複製、保存或交付結果 | TBD |
| Preferences | 調整使用者偏好 | TBD |

## 8. Success metrics

目前全部為 `TBD`。至少需要決定：主要任務完成率、完成時間、取消率、失敗率、結果交付成功率，以及隱私或資料保存相關的品質指標。

## 9. Constraints and risks

- 螢幕內容可能包含敏感資料，任何保存、log、分享與同步都必須有明確授權與生命週期。
- 多螢幕、DPI scaling、權限、焦點與快捷鍵可能影響體驗，需在平台確定後驗證。
- 沒有技術棧與產品範圍前，不應承諾效能、相容性或發布日期。

## 10. Open decisions

1. 目標平台與最低支援版本。
2. 核心使用者與最重要使用情境。
3. 主要任務的正式命名與邊界。
4. 支援的來源、結果格式與編輯能力。
5. 本機資料保存、刪除與隱私模型。
6. 是否需要快捷鍵、系統匣、分享或外部整合。
7. 成功指標與發布門檻。

## 11. Exit criteria for acceptance

本 PRD 可以從 `Draft` 轉為 `Accepted` 前，必須補齊目標使用者、問題陳述、核心流程、非目標、成功指標與產品範圍，並由產品負責人確認。核准後，才建立對應的功能 Specs。
