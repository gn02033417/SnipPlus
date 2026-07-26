# PRD-0003 Product Vision

狀態：`Draft`

## 1. Product Statement

SnipPlus 是一套以 Windows 原生操作習慣為核心，協助使用者快速完成螢幕影像擷取與後續交付的 desktop product。

SnipPlus 的定位是延續使用者熟悉的工作方式，降低從「想要擷取」到「可以繼續使用結果」之間的摩擦，而不是重新發明一套截圖語言。

## 2. Product Goals

- 降低日常螢幕影像擷取與後續交付的操作成本。
- 保持 Windows 使用者對基本截圖操作的熟悉感與連續性。
- 讓基本工作流程直接可理解，不依賴教學才能完成。
- 讓進階能力維持 optional，不阻擋主要擷取工作。
- 讓使用者能在短而清楚的流程中取得可繼續使用的結果。
- 以 Windows desktop experience 作為第一優先，維持平台內的一致性。

## 3. Product Scope

### In Scope

目前產品願景涵蓋以下高階範圍：

- Windows Desktop 使用情境。
- Static image screenshot workflow。
- 從 capture entry 到 capture result 的核心工作流程。
- Capture result 的基本交付脈絡，例如 clipboard handoff。
- 可選的 post-capture workflow；具體編輯能力尚未定義。
- 以既有 Windows 操作習慣為基礎的產品體驗。

以上是產品方向與範圍邊界，不是功能需求清單。每一項具體行為仍須在後續 PRD 與 Specs 中另行定義。

### Out of Scope for the current product definition

以下目前不納入產品定義：

- Cloud sync。
- Team collaboration。
- Cloud storage。
- AI 功能。
- Cross-platform product strategy。
- Video capture。
- 任何尚未經 Research、Analysis、Decision 與 PRD review 的新增能力。

`Out of Scope` 表示目前產品範圍外，不代表永久否決未來重新研究的可能性。

## 4. Target Users

本文件只描述使用者類型，不建立 Persona：

- 一般 Windows 使用者：需要快速完成日常螢幕影像擷取。
- 軟體工程師：需要擷取畫面以進行開發溝通、問題回報或文件整理。
- 技術文件撰寫者：需要取得畫面結果並交付到文件或其他工作脈絡。
- 客服與測試人員：需要以短流程取得可供說明、驗證或回報的畫面結果。

各類使用者的優先順序、頻率、環境與詳細需求仍為 `TBD`。

## 5. Product Principles

UX 原則統一引用 [PRD-0002 User Experience Principles](PRD-0002-user-experience-principles.md)，本文件不重寫其內容。

後續所有 Product Vision、Core Workflow、Functional Requirements 與 Non-functional Requirements 都必須檢查是否違反 PRD-0002 的原則。

## 6. Success Criteria

本階段只定義成功方向，不設定 KPI 或數值：

- 使用者不需要重新學習基本 Windows 截圖操作。
- 大部分日常擷取工作可以沿著清楚而短的流程完成。
- 基本擷取不會被進階能力、設定或額外內容阻擋。
- Capture result 能順利進入使用者下一個工作脈絡。
- 使用者可以不依賴教學理解主要工作流程。
- 產品在 Windows desktop 上呈現一致、熟悉且不造成額外負擔的體驗。

具體的可量化成功指標、可觀察事件與驗證方法將在後續 PRD 階段定義。

## 7. Assumptions

- SnipPlus 的第一個產品目標是 Windows desktop，而不是跨平台產品。
- 使用者已具有部分 Windows 截圖操作經驗，產品應尊重而非破壞這些習慣。
- 後續 runtime research 可能修正部分 Research、Analysis 或 Decision 的內容。
- Windows 行為可能隨作業系統或工具版本更新而改變，來源與版本需要持續追蹤。
- Product Vision 不足以直接建立可執行的功能 Spec。
- 技術棧、部署方式、資料保存、隱私策略與支援版本目前仍為 `TBD`。

## 8. Deferred decisions

本文件刻意不決定：

- Core Workflow 的完整步驟與所有例外狀態。
- Screenshot、annotation、editor、OCR、share 或其他能力的詳細需求。
- UI、Overlay、Toolbar、Arrow、interaction state 與 visual design。
- 效能、可靠性、相容性與可觀測性的數值門檻。
- 技術架構、API、儲存、同步、權限與發布方式。

以上內容必須依照固定順序，在後續 PRD 文件中逐步定義；未經核准前不得直接進入 Specs 或 Coding。

## 9. Review status

- Product owner review：`TBD`
- Review date：`TBD`
- Next PRD document：`PRD-0004-core-workflow.md`
