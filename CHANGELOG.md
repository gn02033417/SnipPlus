# Changelog

本檔案依 Keep a Changelog 精神記錄對使用者、維護者與文件治理有意義的變更。尚未有產品版本發布。

## [Unreleased]

### Added

- 建立 `README.md` 與 `docs/index.md` 文件入口。
- 建立 PRD、Specs、Architecture、Mermaid 架構圖與 ADR 結構。
- 建立 Development Guide、Coding Standard、Markdown naming rules 與 UI Wireframe 草稿。
- 建立 `CONTRIBUTING.md`、`ROADMAP.md`、`TODO.md` 與 Repository-specific `AGENTS.md`。
- 建立 `docs/Research/` framework、methodology、source policy、template、comparison matrix、glossary 與各研究對象入口。
- 新增 Windows 11 Snipping Tool capture workflow 與 workflow state machine 的 Research draft；內容只引用官方 Microsoft 文件，未開始 SnipPlus 設計。
- 建立 `docs/Analysis/` framework、analysis template 與 Win11 capture workflow analysis；內容只整理既有 Research，不新增產品決策。
- 建立 `docs/Decision/` framework、decision template 與 Win11 capture workflow decision，記錄採用判斷及其證據、風險與開放問題。
- PRD v1.0 完成 Baseline Review、Traceability Matrix 與 Freeze Review；Freeze 結論為 `Ready for Specs` / `Freeze Approved`。
- 建立 `SPEC-0002` Specification Guidelines、`SPEC-0003` System Requirements 與 `SPEC-0004` Feature Catalog。
- 新增 `SPEC-0005` `FEAT-001 Capture Workflow` Draft Spec；未建立其他 Feature Spec、Architecture 或程式碼。
- 新增 `SPEC-0006` `FEAT-005 Workflow Boundaries and Feedback` Draft Spec；定義完成、取消、失敗、交接異常、狀態轉換與回饋責任邊界。
- 新增 `SPEC-0007` `FEAT-003 Clipboard Handoff` Draft Spec；定義 Capture Result、Clipboard Ready 與 Clipboard Consumer 的交付責任邊界。
- 新增 `SPEC-0008` `FEAT-004 Capture Output` Draft Spec；定義 Result Created、Output Ready、Output Delivered 與 Output Completed 的抽象 lifecycle。
- 新增 `SPEC-0009` `FEAT-002 Annotation Capability` Draft Spec；定義建立、修改、移除 Annotation 的 optional capability 與 local lifecycle。
- 新增 `SPEC-0010` Feature Integration Draft；整合五個核心 Feature 的責任矩陣、Shared State、跨 Feature 依賴與整合缺口，不新增 Feature 或 Architecture。
- 新增 `ARCH-0001` Architecture Principles、`ARCH-0002` Layer Model 與 `ARCH-0003` Module Catalog Draft；仍未進入 Component、Technology、Project 或 Coding 設計。
- 新增 `ARCH-0004` Component Boundaries Draft；建立 COMP-001 至 COMP-018、Module/Feature mapping、Shared State access boundary、抽象資訊邊界與 Component dependency diagram，仍未進入 Interface、API、Technology、Project 或 Coding 設計。
- 新增 `ARCH-0005` Component Interactions Draft；建立 INT-001 至 INT-022、主要/取消/失敗互動序列、責任矩陣、禁止互動矩陣與 Information Exchange Mapping，仍未進入 Contract、API、Technology、Project 或 Coding 設計。
- 新增 Architecture Baseline Review Draft；Completeness、Traceability、Consistency、Dependency、Responsibility Coverage 與 Principle Compliance 均完成審查，Readiness 判定為 `Ready for ADR and Technology Selection`，Freeze Decision 為 `Freeze Approved`，仍未建立 ADR、技術選型或程式碼。
- 新增 `RESEARCH-TECH-UI-001` UI Framework Feasibility Research Draft；比較 WinUI 3、WPF、Avalonia 與 Windows Forms 的 Overlay、Windowing、DPI、Input、Drawing、Deployment、Accessibility 與維護性證據，建立 `UIF-001` 至 `UIF-018`、`UI-GATE-001` 至 `UI-GATE-007` 與未來 Runtime Spike 定義；未修改 `ADR-0002`，未執行 Prototype 或 Coding。
- 新增 `RESEARCH-TECH-UI-002` UI Framework Runtime Spike Plan Draft；將 11 個上游 Spike 轉成一致的環境紀錄、比較規則、證據類型、固定欄位、Coverage Matrix、Execution Order、Stop Rules、Result Artifact Plan 與 Decision Evidence Roll-up；`Execution Status: Not started`，未執行 Spike、Prototype、Build 或 Runtime Test。
- 新增 `RESEARCH-TECH-UI-003` UI Framework Runtime Spike Execution Readiness Draft；建立實驗版本基線、環境可用性登錄、等價行為基線、`UI-PREQ-xxx`、`UI-BLOCK-xxx`、11 個 Spike Readiness Matrix、Phase Readiness、Evidence Capture Readiness 與安全清理邊界；`Execution Status: Not started`、`Overall Readiness Decision: Not ready`，未執行任何 Spike、Prototype、測試或截圖功能。
- 新增 `RESEARCH-TECH-UI-004` UI Framework Runtime Environment Baseline Draft；以 2026-07-26 唯讀盤點記錄 Windows 11 build 26200、x64 硬體、3 個 active display、.NET、Windows SDK、WPT 與證據工具狀態，分開記錄官方與本機版本，並建立 `UI-ENV-GAP-001` 至 `UI-ENV-GAP-011` 與 Prerequisite／Blocker impact mapping；`Inspection Status: Partially completed`、`Runtime Verification: Not performed`，未安裝工具、未建置、未執行 Spike 或開始 Coding。
- 新增 `RESEARCH-TECH-UI-005` UI Framework Runtime Prerequisite Closure Plan Draft；建立 `UI-CLOSE-001` 至 `UI-CLOSE-015`，完整映射 11 個 Environment Gap、14 個 Prerequisite、9 個 Blocker 與 11 個 Spike，區分 Phase 1、2、3 與 Cross-phase；`Execution Status: Not started`、`Runtime Verification: Not performed`，未執行 Closure Action、未安裝工具、未建立 Prototype 或開始 Coding。
- 新增 `RESEARCH-TECH-UI-006` UI Framework Phase 1 Prerequisite Closure Record Draft；以 `UI-CLOSE-EVID-001` 至 `UI-CLOSE-EVID-015` 記錄官方版本、本機 Windows／display／DPI／HDR／toolchain／Windows SDK／AppX Runtime 與 evidence policy 的唯讀查核，建立 15 個 Closure Action execution records、Deferred Condition Register、14 個 Prerequisite、9 個 Blocker recommendation matrix 與 Findings Register；`Execution Status: Partially completed`、`Runtime Verification: Not performed`，未安裝、下載、Restore、Build、建立 Project、執行 Spike 或開始 Coding。
- 新增 `RESEARCH-TECH-UI-007` UI Framework Phase 1 Readiness Reassessment Draft；重新評估 10 個 Phase 1 Gate、`UI-PREQ-001` 至 `UI-PREQ-014`、`UI-BLOCK-001` 至 `UI-BLOCK-009`、11 個 Environment Gap、15 個 Closure Action 與 7 個 Findings；判定 `Readiness: Not ready`、`Runtime Spike Execution Authorized: No`，未修改上游文件、未建立 Project、未執行 Build 或開始 Coding。
- 新增 `RESEARCH-TECH-UI-008` UI Framework Phase 1 Execution Enablement Specification Draft；將 `BA-001` 至 `BA-008` 綁定為 `UI-ENABLE-001` 至 `UI-ENABLE-008`，規格化實驗工具鏈、candidate parity、display/DPI、synthetic input、evidence、safety、rollback 與 authorization boundary；`Final Enablement Status: Conditionally ready to request Phase 1 execution authorization`、`Current authorization: Not granted`、`Execution permitted: No`，未安裝、下載、Restore、Build、建立 Project、建立 Result 或開始 Coding。
- 新增 `RESEARCH-TECH-UI-009` UI Framework Phase 1 Enablement Execution Authorization Request Draft；建立 `UI-AUTH-001` 至 `UI-AUTH-008` 的一對一、逐項、可撤銷授權請求，定義 scope、風險、constraints、expiry、rollback、cleanup 與 evidence；`Authorization Decision: Pending`、`Enablement Execution Authorized: No`、`Execution permitted: No`，未執行任何操作。

### Changed

- 移除空白 `.gitkeep`，讓實際文件目錄取代 placeholder。

### Not released

- 尚無應用程式碼、截圖功能、build、test、package 或 deploy 產物。
