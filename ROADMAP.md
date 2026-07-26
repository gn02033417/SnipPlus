# ROADMAP

狀態：`Accepted`

Roadmap 使用階段與出口條件，不先承諾日期。具體時程要等必要 ADR、工程契約與人力確認後再加入。

## Completed baselines

### Product baseline

狀態：`Complete`

- PRD v1.0 Freeze Approved。
- 產品願景、UX 原則、核心流程、FR、NFR 與追溯基線已建立。

### Behavioral specification baseline

狀態：`Complete`

- Specification v1.0 Freeze Approved。
- 五個核心 Feature、shared state、workflow boundary 與 acceptance criteria 已建立。

### Abstract architecture baseline

狀態：`Complete`

- Architecture baseline Freeze Approved。
- Layer、Module、Component、Interaction、ownership 與 dependency boundary 已建立。

### Repository governance foundation

狀態：`Complete`

- 文件入口、生命週期、命名規則、ADR governance、Development Guide、Coding Standard、Wireframe、協作與追蹤文件已建立。

## Current phase — Technology decisions and implementation preparation

狀態：`In progress`

目標：將 Candidate 技術主題收斂為 Accepted ADR，建立必要工程契約、Project Structure 與 Verification Strategy。

目前重點：

1. Review 並接受、否決或退回 ADR-0002 UI Framework Selection。
2. 完成 Rendering Technology、Capture Backend、Clipboard Integration、Image Representation 與 Testing Strategy 的核心決策。
3. 建立 Shared Result、Capture、Clipboard、Output 與 Failure contracts。
4. 建立 Component-to-project mapping 與 Solution／Project Structure。
5. 建立單一 Implementation Readiness Review。

出口條件：

- 核心 P0 ADR 已 Accepted 或明確 Deferred。
- 技術選擇不改寫 Frozen PRD、Specs 或 Architecture ownership。
- 必要 Interface／Data contracts 已可供實作與測試引用。
- Solution／Project Structure、setup、build、test 與 CI plan 可重現。
- 第一個 vertical slice 的 scope、non-goals、acceptance criteria 與 verification plan 明確。
- Implementation Readiness Review 明確授權或拒絕開始 coding。

## Next phase — First vertical slice

狀態：`Not started`

目標：依 Accepted baselines 與 ADR 實作最小、可驗證的端到端流程。

建議邊界：

- 單一 Windows desktop host。
- 單一最小 capture path。
- 明確的 Capture Result contract。
- 單一 downstream delivery path。
- 基本取消、失敗與 cleanup。
- 自動化測試與可重現 runtime evidence。

出口條件：

- 核心流程符合 Frozen Specs acceptance criteria。
- 取消、失敗與 cleanup 有測試。
- Platform-specific behavior 有 evidence。
- 未在 Implementation 內新增產品需求或改寫 Architecture。

## Later phase — Product hardening and release

狀態：`Not started`

目標：處理相容性、效能、可觀測性、packaging、update、rollback 與支援範圍。

出口條件：

- 發布檢查表、版本策略、CHANGELOG 與 rollback policy 完整。
- 已知限制、支援範圍與 verification result 可追溯。
- Release artifact 不繞過 Verification。

## Stopped documentation pattern

`docs/Research/Technology/29–80` 保留為歷史研究與治理證據。Clipboard D1 的 039→052 documentary chain 已停止。

除非出現新的外部證據、實際 Authority 決策、Accepted change 或 runtime evidence，不再建立同類型 prerequisite、readiness reassessment、authorization request 或 closure-review 文件。

## Deferred product capabilities

以下能力只有在 Frozen PRD／Specs 的 change control 通過後才進入正式排程：

- OCR
- 雲端同步
- 分享與外部整合
- Plugin architecture
- 複雜 annotation toolset
- 跨平台支援
