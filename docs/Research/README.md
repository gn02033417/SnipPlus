# Research Framework

狀態：`Accepted`

`Research/` 是 SnipPlus 的外部事實來源，記錄目前產品、平台與工具實際可觀察到的行為。它不是 PRD、UX 設計稿，也不是功能 Spec。

## 文件責任

```text
 Research  ->  Analysis  ->  Decision  ->  PRD  ->  Specs  ->  Architecture  ->  Coding
 事實          分析          採用判斷      產品決策    行為契約     技術邊界          實作
```

- `Research/`：描述「外部世界目前如何運作」，必須有來源與驗證方式。
- `Analysis/`：整理研究中的流程、狀態、意圖、依賴與失敗邊界，不直接做產品決策。
- `Decision/`：記錄是否採用研究與分析中的流程元素，以及理由、證據、風險與開放問題。
- `PRD/`：描述「SnipPlus 決定解決什麼問題與採用什麼範圍」。
- `Specs/`：描述「SnipPlus 必須提供哪些可驗收行為」。
- `Architecture/`：描述「系統如何承擔已核准的行為」。

Research 不得直接新增 SnipPlus 需求。研究結果可先進入 Analysis 做中立整理，再透過 Decision 記錄採用判斷；PRD 仍負責正式定義產品範圍。

## 研究入口

- [Win11](Win11/README.md)
- [LINE](LINE/README.md)
- [Snagit](Snagit/README.md)
- [ShareX](ShareX/README.md)
- [Greenshot](Greenshot/README.md)
- [Flameshot](Flameshot/README.md)
- [CleanShot](CleanShot/README.md)
- [Codex](Codex/README.md)

### Technology feasibility

- [UI framework feasibility for SnipPlus](Technology/01-ui-framework-feasibility.md) — WinUI 3、WPF 與次要候選的 Overlay、DPI、Input、Drawing、Deployment 證據研究；目前為 `Draft`，不作 Framework 最終決策。
- [UI framework runtime spike plan](Technology/02-ui-framework-runtime-spike-plan.md) — `RESEARCH-TECH-UI-002`，只規劃未來一致化驗證；`Execution Status: Not started`，不執行 Spike 或 Prototype。
- [UI framework runtime spike execution readiness](Technology/03-ui-framework-runtime-spike-execution-readiness.md) — `RESEARCH-TECH-UI-003`，只記錄版本、環境、前置條件與阻塞狀態；`Execution Status: Not started`、`Overall Readiness Decision: Not ready`。
- [UI framework runtime environment baseline](Technology/04-ui-framework-runtime-environment-baseline.md) — `RESEARCH-TECH-UI-004`，記錄唯讀環境、工具與官方版本證據；`Inspection Status: Partially completed`、`Runtime Verification: Not performed`，不解除父文件 Blocker。
- [UI framework runtime prerequisite closure plan](Technology/05-ui-framework-runtime-prerequisite-closure-plan.md) — `RESEARCH-TECH-UI-005`，規劃 Gap、Prerequisite、Blocker 與 Spike 的 Closure Action 及 Phase 分類；`Execution Status: Not started`、`Runtime Verification: Not performed`，不授權任何執行。
- [UI framework Phase 1 prerequisite closure record](Technology/06-ui-framework-phase1-prerequisite-closure-record.md) — `RESEARCH-TECH-UI-006`，記錄官方與本機唯讀 closure evidence、Deferred conditions、status recommendations 與 findings；`Execution Status: Partially completed`、`Runtime Verification: Not performed`，不授權 Runtime Spike。
- [UI framework Phase 1 readiness reassessment](Technology/07-ui-framework-phase1-readiness-reassessment.md) — `RESEARCH-TECH-UI-007`，重新評估 10 個 Phase 1 Gate 與真正阻塞事項；`Status: Draft`、`Readiness: Not ready`、`Runtime Verification: Not performed`，不授權 Runtime Spike。
- [UI framework Phase 1 execution enablement specification](Technology/08-ui-framework-phase1-execution-enablement-specification.md) — `RESEARCH-TECH-UI-008`，將 `BA-001` 至 `BA-008` 轉為一對一 Enablement Item；`Status: Draft`、`Current authorization: Not granted`、`Execution permitted: No`，不執行任何操作。
- [UI framework Phase 1 enablement execution authorization request](Technology/09-ui-framework-phase1-enablement-execution-authorization-request.md) — `RESEARCH-TECH-UI-009`，將 8 個 Enablement Item 轉為逐項 `UI-AUTH-001` 至 `UI-AUTH-008`；`Authorization Decision: Pending`、`Execution permitted: No`，不授權 Runtime Spike。

以上入口目前只定義未來研究主題，沒有填入任何產品事實。

## Required documents

- [Methodology](methodology.md)
- [Source policy](source-policy.md)
- [Research template](template.md)
- [Comparison matrix](comparison-matrix.md)
- [Glossary](glossary.md)

## 禁止事項

- 不得以記憶補完未知資訊。
- 不得把未驗證內容寫成事實。
- 不得在 Research 直接設計 SnipPlus。
- 不得在此區建立產品 PRD、功能 Spec 或程式碼。
- 過期內容必須標示版本、日期與過期風險。
