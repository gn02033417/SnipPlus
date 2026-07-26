# PRD-0002 User Experience Principles

狀態：`Draft`

## 1. 文件目的

本文件只定義 SnipPlus 應遵守的 UX 原則，作為後續 Product Vision、Core Workflow、Functional Requirements 與 Non-functional Requirements 的共同產品基線。

本文件不是功能清單、UI 設計、Toolbar Spec、Overlay Spec、技術方案或程式碼授權。

## 2. 來源與邊界

本 PRD 只引用已完成的 Research、Analysis 與 Decision 層：

- [Win11 capture workflow Research](../docs/Research/Win11/01-capture-workflow.md)
- [Win11 capture workflow Analysis](../docs/Analysis/Win11/capture-workflow-analysis.md)
- [Win11 capture workflow Decision](../docs/Decision/Win11/capture-workflow-decision.md)

以上來源提供流程脈絡，不代表 SnipPlus 已完成所有產品範圍決策。未在本文件明確定義的功能、平台支援、效能指標、資料保存與整合方式仍為 `UNKNOWN` 或 `TBD`。

## 3. UX Principles

### Principle 1 — 不改變 Windows 使用者的肌肉記憶

SnipPlus 的主要操作應延續 Windows 使用者已熟悉的截圖入口與基本思考方式。使用者不應為了完成最基本的擷取而重新學習一套不同的操作語言。

本原則只定義熟悉感與操作連續性，不指定快捷鍵、視覺樣式或實作 API。

### Principle 2 — 新增能力不能增加基本操作的負擔

任何新增能力都必須保護基本擷取流程的直接性。進階能力不應讓使用者在完成日常擷取前，先處理額外設定、額外頁面或不必要的選擇。

具體步驟數、延遲與量化門檻尚未定義，保留為 `TBD`。

### Principle 3 — 主要情境保持為「PrintScreen → 框選 → 完成 → Ctrl+V」

對大多數日常使用情境，核心結果應能沿著「進入擷取、選取範圍、完成、貼上」的短流程取得。這是 UX 原則，不是已核准的功能 Spec，也不預先決定所有 capture mode。

目前「90%」只表示主要情境的產品優先順序方向，不是已建立的成功指標；正式數值仍為 `TBD`。

### Principle 4 — 進階能力全部是 Optional

Annotation、editor、OCR、分享、整合或其他進階能力都不應成為完成基本擷取的必要前置條件。使用者可以忽略進階能力，仍能完成主要工作。

本原則不代表任何進階能力已被 PRD 核准；各能力仍須在後續產品文件中個別決定。

### Principle 5 — 使用者不應需要閱讀教學才能完成基本工作

基本擷取流程應能透過介面與熟悉的操作直接理解。說明文字可以協助，但不應成為完成主要流程的必要依賴。

教學、提示與無障礙說明的具體形式仍為 `TBD`。

### Principle 6 — 常用工具在一層內完成

常用操作應在當前工作脈絡中可發現並完成，避免以三層以上的 menu nesting 隱藏基本工具。這是可發現性原則，不指定 toolbar 的位置、按鈕或視覺配置。

哪些操作屬於「常用」仍需由後續 PRD 與研究確認。

### Principle 7 — 優先遵循 Windows Fluent 語言

Windows 平台上的互動與視覺語言應優先與 Fluent 設計方向相容，而不是為了建立品牌感而引入不必要的自訂 UI 語言。

這項原則不等於已決定 UI framework、component library、色彩、icon 或 layout。

### Principle 8 — Windows first，暫不以跨平台為優先

SnipPlus 的 UX 優先針對 Windows 使用者與 Windows 工作習慣建立一致性。跨平台一致性目前不是第一優先事項。

最低 Windows 版本、未來跨平台範圍與支援政策仍為 `TBD`，本原則不直接否決未來評估。

### Principle 9 — 先快，再增加功能

基本擷取流程的反應速度與連續性優先於功能數量。產品應先確保使用者可以快速完成主要工作，再評估是否加入額外能力。

本階段不設定具體毫秒數、percentile 或其他效能目標；效能指標將在後續 PRD 階段定義。

### Principle 10 — 所有新增能力都必須可以不用

使用者應能選擇不使用新增能力，而不被迫改變基本工作流程。任何新增能力都必須能說明它如何保持 optional，以及不使用時的基本路徑為何。

若一項能力會變成完成基本擷取的必要條件，必須重新檢視它是否違反本原則。

## 4. Product decisions intentionally deferred

本文件刻意不決定以下內容：

- Toolbar、Overlay、Arrow、Annotation、OCR、AI、Plugin 或 Pin 的具體設計。
- 支援哪些 capture mode 的最終功能範圍。
- 詳細快捷鍵、視覺配置、interaction states 與 keyboard focus。
- 效能、可靠性、儲存、隱私、分享與外部整合指標。
- 技術棧、UI framework、架構、API 與部署方式。

以上內容必須在後續 PRD、Specs、Architecture 或 ADR 中依責任範圍另行處理。

## 5. Review status

- Product owner review：`TBD`
- Review date：`TBD`
- Next PRD sequence：Product Vision、Core Workflow、Functional Requirements、Non-functional Requirements
