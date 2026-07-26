# System Overview

狀態：`Draft`

## 1. Architecture intent

在產品需求尚未核准前，先固定責任邊界與追溯關係，避免未來把 UI、產品規則、平台能力與檔案 I/O 混成不可維護的單一模組。

## 2. Planned logical boundaries

| Boundary | Responsibility | Current status |
| --- | --- | --- |
| Presentation | 顯示狀態、接收使用者輸入、呈現錯誤與結果 | Wireframe only |
| Application | 協調一個完整的使用者操作流程 | TBD |
| Domain | 定義產品規則、狀態轉移與驗證 | TBD |
| Platform adapters | 封裝 OS、螢幕、快捷鍵、剪貼簿等能力 | TBD |
| Storage | 保存或刪除使用者明確要求的資料 | TBD |
| External services | 外部整合；若需要才建立 | TBD |
| Observability | 安全、可定位的診斷資訊 | TBD |

目前只有第一層有 Wireframe 草稿，其餘邊界是為未來實作預留的責任位置，不表示已有對應程式碼。

## 3. Dependency direction

預期依賴方向如下：

```text
Presentation -> Application -> Domain
Platform adapters --------^ 
Storage ------------------^
External services --------^
Observability ------------^
```

Presentation 不應直接依賴具體平台或儲存；Application 負責協調，Domain 保持可測試且不依賴 UI；外部副作用由 adapter 邊界隔離。這些規則在技術棧確定後需要用實際模組驗證。

## 4. Data and privacy baseline

- 螢幕內容、剪貼簿內容與產出結果一律視為可能敏感資料。
- 未經產品決策，不保存、不同步、不寫入診斷 log。
- 保存、刪除、暫存與失敗清理必須有明確生命週期。
- 錯誤訊息可以提供診斷上下文，但不得洩漏使用者內容或秘密。

## 5. Failure and cancellation baseline

未來每個長時間或可取消操作都必須定義：開始、進行中、成功、取消、失敗、可重試與不可重試狀態。具體狀態名稱與轉移等 PRD/Spec 核准後再固定。

## 6. Deployment and operations

目前沒有部署目標、版本策略、更新機制、telemetry 或 rollback 設計。這些項目在平台與產品範圍確定後補入 Architecture 與相應 ADR，不預先創造假設。

## 7. Architecture quality attributes

後續至少需要針對以下品質屬性建立可驗證目標：

- 可維護性：模組責任與變更影響清楚。
- 可測試性：核心規則不依賴 UI 或真實平台才能驗證。
- 隱私：使用者內容的保存與 log 可追蹤、可控制。
- 可用性：取消、失敗與恢復行為一致。
- 相容性：平台、顯示器與縮放行為有明確支援範圍。
- 效能：主要任務的延遲與資源使用有測量方式。
