# ROADMAP

狀態：`Draft`

Roadmap 使用階段與出口條件，不先承諾日期。日期要等產品範圍、技術棧與人力確認後再加入。

## Current phase — Documentation foundation

狀態：`In progress`

已建立：文件入口、命名規則、PRD/Specs/Architecture 分層、ADR、Development Guide、Coding Standard、Wireframe、協作與追蹤文件。

出口條件：主要文件能互相連結，未知內容可辨識，且沒有把尚未實作的功能描述成已完成。

## Phase 1 — Product discovery

目標：確認目標使用者、核心問題、主要情境、平台與成功指標。

出口條件：`PRD-0001` 從 `Draft` 轉為 `Accepted`，並核准 non-goals 與資料隱私原則。

## Phase 2 — Behavioral specification

目標：把核准的核心流程拆成可驗收的 `SPEC-NNNN` 文件，包含正常、取消、失敗與恢復行為。

出口條件：核心流程有完整 acceptance criteria，UI Wireframe 與 Specs 對齊。

## Phase 3 — Technical foundation

目標：選定平台、技術棧、模組邊界、儲存與發布策略，並為真正的長期取捨建立 ADR。

出口條件：Architecture 能對應到實際 solution 結構，setup、build、test、lint 與 release 方式可重現。

## Phase 4 — First usable implementation

目標：依 Accepted Specs 實作最小可用流程，建立測試與可定位的錯誤處理。

出口條件：核心流程可驗收、取消與失敗路徑有測試、隱私與資料生命週期符合決策。

## Phase 5 — Hardening and release

目標：處理相容性、效能、可觀測性、文件、版本與 rollback。

出口條件：有發布檢查表、已知限制、CHANGELOG、支援範圍與回復策略。

## Not scheduled

截圖模式、編輯、OCR、雲端同步、分享、外部整合與其他候選能力都尚未排程；要先通過產品 discovery 與 scope decision。
