# TODO

狀態：`Accepted`

這份清單只保留尚未完成、且有明確下一步的事項。產品功能在 PRD 核准前不直接拆成 implementation tasks。

## P0 — 必須先決定

- [ ] 確認目標使用者與第一個核心使用情境。
- [ ] 確認 SnipPlus 的產品問題與不可取代價值。
- [ ] 決定支援平台、最低版本與使用者權限模型。
- [ ] 決定資料保存、暫存、刪除與 log 的隱私策略。

## P1 — PRD 核准後

- [ ] 將 `PRD-0001` 補齊成功指標、scope 與 non-goals，並轉為 `Accepted`。
- [ ] 將核准的主要流程拆成 `SPEC-NNNN`。
- [ ] 依正式流程更新 UI Wireframe 與 acceptance criteria。
- [ ] 選定技術棧、solution 結構與本機開發方式。
- [ ] 為真正有長期取捨的技術選擇新增 ADR。

## Research framework completed

- [x] 建立 Research 與 PRD、Specs 的責任分界。
- [x] 建立研究方法、來源政策、統一模板、比較矩陣與研究術語表。
- [x] 建立 Win11、LINE、Snagit、ShareX、Greenshot、Flameshot、CleanShot、Codex 研究入口。
- [ ] Review `docs/Research/Win11/01-capture-workflow.md` 與 `02-workflow-state-machine.md`。
- [ ] Review `docs/Research/Technology/01-ui-framework-feasibility.md`，確認 `UIF-001` 至 `UIF-018`、`UI-GATE-001` 至 `UI-GATE-007` 與 Runtime Spike 邊界後，再決定是否補足 `ADR-0002` 證據。
- [ ] Review `docs/Research/Technology/02-ui-framework-runtime-spike-plan.md`，確認執行權限、環境、版本、測試硬體與門檻都明確後，才可另行授權 Runtime Spike。
- [ ] Review `docs/Research/Technology/03-ui-framework-runtime-spike-execution-readiness.md`，確認實驗版本、真實環境紀錄、`UI-PREQ-xxx`、`UI-BLOCK-xxx`、證據工具與安全清理條件後，才可評估是否授權 Phase 1 Runtime Spike。
- [ ] Review `docs/Research/Technology/04-ui-framework-runtime-environment-baseline.md`，確認官方版本、本機版本、DPI／HDR／ARM64／Packaging 證據與 `UI-ENV-GAP-xxx` 後，再決定是否能重新評估 `RESEARCH-TECH-UI-003` 的 Blocker。
- [ ] Review `docs/Research/Technology/05-ui-framework-runtime-prerequisite-closure-plan.md`，確認 `UI-CLOSE-xxx` 的 Phase 分類、Closure Action、Evidence、Authorization Boundary 與完整 Impact Matrix 後，再決定是否另行授權前置條件關閉工作。
- [ ] Review `docs/Research/Technology/06-ui-framework-phase1-prerequisite-closure-record.md`，確認 `UI-CLOSE-EVID-xxx`、15 個 Closure Action execution records、Deferred Condition、Prerequisite／Blocker recommendations 與 Findings 後，再決定是否建立 Readiness Reassessment。
- [ ] Review `docs/Research/Technology/07-ui-framework-phase1-readiness-reassessment.md`，確認 10 個 Phase 1 Gate、Windows identity limitation、toolchain／display／DPI blockers、Deferred scope 與 `Runtime Spike Execution Authorized: No` 後，再決定是否進入獨立授權審查。
- [ ] Review `docs/Research/Technology/08-ui-framework-phase1-execution-enablement-specification.md`，確認 `BA-001` 至 `BA-008` 與 `UI-ENABLE-001` 至 `UI-ENABLE-008` 一對一、所有 authorization 欄位為 `Not granted`／`No`，以及未來 evidence、safety、rollback 邊界後，再決定是否進入人工授權審查。
- [ ] Review `docs/Research/Technology/09-ui-framework-phase1-enablement-execution-authorization-request.md`，確認 `UI-AUTH-001` 至 `UI-AUTH-008` 的逐項 scope、risk、constraints、expiry、rollback、cleanup 與 `Authorization Decision: Pending` 後，再決定是否提出任何實際 Enablement execution authorization。

## Analysis layer

- [x] 建立 Analysis framework 與統一分析模板。
- [x] 建立 Win11 capture workflow 的流程、狀態、意圖、依賴與失敗邊界分析。
- [ ] Review `docs/Analysis/Win11/capture-workflow-analysis.md`，確認它沒有被誤讀為 PRD、Spec 或產品方案。

## Decision layer

- [x] 建立 Decision framework 與統一決策模板。
- [x] 建立 Win11 capture workflow 的採用判斷、理由、證據、風險與開放問題。
- [ ] Review `docs/Decision/Win11/capture-workflow-decision.md`，確認採用判斷後再進入 PRD。

## Specs governance

- [x] 建立 Specification Guidelines、System Requirements 與 Feature Catalog。
- [x] 建立 `FEAT-001 Capture Workflow` 的第一份 Draft Feature Spec。
- [x] 建立 `FEAT-005 Workflow Boundaries and Feedback` 的 Draft Feature Spec。
- [x] 建立 `FEAT-003 Clipboard Handoff` 的 Draft Feature Spec。
- [x] 建立 `FEAT-004 Capture Output` 的 Draft Feature Spec。
- [x] 建立 `FEAT-002 Annotation Capability` 的 Draft Feature Spec。
- [x] 建立五個核心 Feature 的 `SPEC-0010` Feature Integration Draft。
- [ ] Review `Specs/SPEC-0005-capture-workflow.md`，確認只涵蓋 FEAT-001 與交接邊界。
- [ ] Review `Specs/SPEC-0006-workflow-boundaries-and-feedback.md`，確認共同取消、失敗、交接異常與回饋邊界沒有偷加入 UI 或技術決策。
- [ ] Review `Specs/SPEC-0007-clipboard-handoff.md`、`SPEC-0008-capture-output.md` 與 `SPEC-0009-annotation-capability.md`，確認三個 Feature 的責任彼此獨立且沒有進入實作層。
- [ ] Review `Specs/SPEC-0010-feature-integration.md`，確認五個核心 Feature 的 Primary Owner、Shared State 與 downstream paths 沒有衝突或未授權依賴。

## Architecture baseline

- [x] 建立 `ARCH-0001` Architecture Principles。
- [x] 建立 `ARCH-0002` Layer Model。
- [x] 建立 `ARCH-0003` Module Catalog。
- [x] 建立 `ARCH-0004` Component Boundaries。
- [x] 建立 `ARCH-0005` Component Interactions。
- [x] 建立 Architecture Baseline Review。
- [ ] Review `Architecture/ARCH-0001-architecture-principles.md`、`ARCH-0002-layer-model.md` 與 `ARCH-0003-module-catalog.md`，確認 Layer、Module、Feature boundary 與依賴方向一致。
- [ ] Review `Architecture/ARCH-0004-component-boundaries.md`，確認 Component ownership、Shared State access、Clipboard/Output 平行關係與 Annotation optional boundary 一致。
- [ ] Review `Architecture/ARCH-0005-component-interactions.md`，確認 INT catalog、Shared State transition authority、平行 downstream 與禁止互動一致。
- [x] 完成 Architecture Baseline Review；目前沒有 Blocking Finding，Freeze Decision 為 `Freeze Approved`，下一階段可進入 ADR 與 Technology Selection。

## P2 — 技術基線建立後

- [ ] 建立可重現的 setup、format、lint、test 與 build 指令。
- [ ] 建立 CI 檢查與文件連結檢查。
- [ ] 定義版本、發布、rollback 與支援政策。
- [ ] 建立與產品需求相符的測試策略。

## Deferred

- [ ] 截圖或擷取功能的正式 scope。
- [ ] 編輯、OCR、分享、雲端同步與外部整合。
- [ ] 多螢幕、DPI scaling、快捷鍵與系統匣行為。

以上項目沒有在 PRD 核准前排入開發承諾。
