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

### UI framework decision

狀態：`Complete`

- ADR-0002 Accepted。
- Desktop UI Framework：WinUI 3。

### Rendering technology decision

狀態：`Complete`

- ADR-0003 Accepted。
- Retained host visuals：WinUI XAML／Microsoft.UI.Composition。
- Immediate 2D、bitmap、effect and offscreen raster path：Win2D behind a rendering adapter。
- Runtime verification remains pending and is not implied by ADR acceptance。

## Current phase — Remaining core technology decisions

狀態：`In progress`

目標：將剩餘 P0 技術主題收斂為 Accepted ADR，不再建立額外 prerequisite／closure 文件鏈。

目前順序：

1. **Capture Backend ADR** — 下一個主要任務。
2. Image Representation ADR。
3. Clipboard Integration ADR。
4. Testing Strategy ADR。

既有 Research 使用方式：

- Capture Backend：`docs/Research/Technology/20–28`
- Clipboard Integration：`docs/Research/Technology/29–80`
- Rendering：`docs/Research/Technology/10–18` 已由 ADR-0003 收斂；未執行項目只作 future verification plan

上述 Research 保留為 evidence 和歷史記錄；不得因 `Not ready` 自動新增下一層 authorization-request 或 closure-review 文件。

本階段出口條件：

- 核心 P0 ADR 已 Accepted 或明確 Deferred。
- 技術選擇不改寫 Frozen PRD、Specs 或 Architecture ownership。
- Capture、Clipboard、Image Representation 和 Testing dependency 可追溯。
- 關鍵 UNKNOWN／TBD 已轉成 contract、verification item 或明確 deferred decision。

## Next phase — Contracts and Project Structure

狀態：`Not started`

目標：建立實作和測試真正需要的工程邊界。

必要成果：

- Shared Result／Image Result contract。
- Rendering contract：render intent、coordinate spaces、alpha／color、resource recovery、display／raster equivalence。
- Capture Backend boundary contract。
- Clipboard Handoff contract。
- Output Delivery contract。
- Error、failure、retry、preservation 與 cleanup contract。
- Component interaction sync／async boundary。
- C#／.NET／Windows App SDK／Win2D version baseline。
- Component-to-project／assembly mapping。
- Solution／Project Structure。
- setup、format、lint、test、build 與 CI plan。

本階段出口條件：

- Contracts 能回溯至 Frozen Specs、Architecture 與 Accepted ADR。
- Project Structure 不改變既有 Module／Component ownership。
- Language、Runtime、SDK 和 dependency versions 具有正式來源。
- 最小 vertical slice 可由明確 Project 和 contract 實現。

## Following phase — Implementation readiness

狀態：`Not started`

建立一份 repository-wide Implementation Readiness Review，確認：

- Accepted source baselines。
- Accepted／Deferred P0 ADR。
- Contracts 和 ownership。
- Project Structure。
- setup、build、test 和 CI plan。
- 第一個 vertical slice 的 scope、non-goals 與 acceptance criteria。
- Verification evidence、cleanup 和 rollback expectations。

只有 Review 明確允許，且使用者另行下達 implementation task，才可以建立 application source code。

## First vertical slice

狀態：`Not started`

建議邊界：

- 單一 WinUI 3 Windows desktop host。
- 單一最小 capture path。
- 明確 Shared Result／Image Result contract。
- 以 ADR-0003 rendering adapter 顯示一個合成或 capture result。
- 單一 downstream delivery path。
- 基本取消、失敗和 cleanup。
- 自動化測試與可重現 runtime evidence。

出口條件：

- 核心流程符合 Frozen Specs acceptance criteria。
- 取消、失敗、resource recovery 與 cleanup 有測試。
- Platform-specific behavior 和 rendering output 有 evidence。
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

除非出現新的 external evidence、實際 Authority 決策、Accepted change 或 runtime evidence，不再建立同類型 prerequisite、readiness reassessment、authorization request 或 closure-review 文件。

## Deferred product capabilities

以下能力只有在 Frozen PRD／Specs change control 通過後才進入正式排程：

- OCR
- 雲端同步
- 分享與外部整合
- Plugin architecture
- 複雜 annotation toolset
- 跨平台支援
