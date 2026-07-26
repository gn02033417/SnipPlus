# PRD-0005 Functional Requirements

狀態：`Draft`

## 1. 文件定位

本文件回答唯一問題：SnipPlus 在 v1.0 必須具備哪些功能能力（Capability）？

本文件只描述使用者可理解的能力，不描述如何實作、不列出 Toolbar 按鈕、不定義 Arrow、Rectangle、Circle、Pen、Highlighter、OCR、AI、Plugin 或任何其他工具細節。

每個 requirement 都有唯一 `FR-` ID，並可追溯到既有的 Research、Analysis、Decision 或 PRD 文件。未來每份 Spec 必須引用至少一個本文件的 FR ID。

## 2. Requirement format

| Field | Meaning |
| --- | --- |
| ID | 穩定且唯一的 Functional Requirement identifier。 |
| Title | 能力的簡短名稱。 |
| Description | 使用者可理解的能力描述，不包含 implementation。 |
| Priority | 只使用 `Must`、`Should` 或 `Could`。 |
| Dependencies | 直接影響本能力的既有文件或流程。 |
| Source | 支持此能力的 Research、Analysis、Decision 或 PRD 來源。 |

## 3. Capture

### FR-001 — Start capture workflow

| Field | Value |
| --- | --- |
| ID | `FR-001` |
| Title | Start capture workflow |
| Description | 使用者能從一個合法入口啟動一次 static image capture workflow。 |
| Priority | `Must` |
| Dependencies | `PRD-0003`、`PRD-0004`、Windows entry decision |
| Source | [Capture entry decision](../docs/Decision/Win11/capture-workflow-decision.md#capture-entry-workflow)；[Core Workflow entry points](PRD-0004-core-workflow.md#3-entry-points) |

### FR-002 — Select capture region or scope

| Field | Value |
| --- | --- |
| ID | `FR-002` |
| Title | Select capture region or scope |
| Description | 使用者能指定這次 capture 要取得的影像區域或已核准的 capture scope。 |
| Priority | `Must` |
| Dependencies | `FR-001`、`PRD-0004` |
| Source | [Region selection decision](../docs/Decision/Win11/capture-workflow-decision.md#rectangle--freeform-selection)；[Core Workflow states](PRD-0004-core-workflow.md#5-workflow-states) |

### FR-003 — Complete capture

| Field | Value |
| --- | --- |
| ID | `FR-003` |
| Title | Complete capture |
| Description | 使用者能完成一次 capture，並取得可繼續處理或交付的 capture result。 |
| Priority | `Must` |
| Dependencies | `FR-002`、`PRD-0004` |
| Source | [Capture workflow decision](../docs/Decision/Win11/capture-workflow-decision.md#capture-entry-workflow)；[Core Workflow complete state](PRD-0004-core-workflow.md#5-workflow-states) |

## 4. Annotation

Annotation 是 optional capability。以下 requirements 只描述「能對 capture result 進行後續標註處理」的能力，不決定任何工具類型、工具數量或 UI 內容。

### FR-004 — Create annotations on a capture result

| Field | Value |
| --- | --- |
| ID | `FR-004` |
| Title | Create annotations on a capture result |
| Description | 使用者能在 capture result 上建立標註內容。 |
| Priority | `Should` |
| Dependencies | `FR-003`、`PRD-0002` Principle 4、`PRD-0004` Annotation state |
| Source | [Post-capture workflow decision](../docs/Decision/Win11/capture-workflow-decision.md#post-capture-toolbar--annotation-stage)；[Core Workflow scope](PRD-0004-core-workflow.md#1-workflow-scope) |

### FR-005 — Modify annotations

| Field | Value |
| --- | --- |
| ID | `FR-005` |
| Title | Modify annotations |
| Description | 使用者能修改已建立的標註內容，而不需要重新建立整個 capture result。 |
| Priority | `Should` |
| Dependencies | `FR-004`、`PRD-0002` Principle 4 |
| Source | [Post-capture workflow decision](../docs/Decision/Win11/capture-workflow-decision.md#post-capture-toolbar--annotation-stage)；[Core Workflow user journey](PRD-0004-core-workflow.md#6-user-journey) |

### FR-006 — Remove annotations

| Field | Value |
| --- | --- |
| ID | `FR-006` |
| Title | Remove annotations |
| Description | 使用者能移除已建立的標註內容，並保留 capture result 的後續處理能力。 |
| Priority | `Should` |
| Dependencies | `FR-004`、`FR-005` |
| Source | [Post-capture workflow decision](../docs/Decision/Win11/capture-workflow-decision.md#post-capture-toolbar--annotation-stage)；[Core Workflow annotation state](PRD-0004-core-workflow.md#5-workflow-states) |

## 5. Clipboard

### FR-007 — Deliver the result to the clipboard

| Field | Value |
| --- | --- |
| ID | `FR-007` |
| Title | Deliver the result to the clipboard |
| Description | 使用者能將完成的 capture result 交付至 clipboard，以便進入下一個工作脈絡。 |
| Priority | `Must` |
| Dependencies | `FR-003`、`PRD-0004` Clipboard Ready state |
| Source | [Clipboard handoff decision](../docs/Decision/Win11/capture-workflow-decision.md#automatic-clipboard-handoff)；[Core Workflow clipboard state](PRD-0004-core-workflow.md#5-workflow-states) |

## 6. Output

### FR-008 — Produce a screenshot result

| Field | Value |
| --- | --- |
| ID | `FR-008` |
| Title | Produce a screenshot result |
| Description | 系統能在 capture workflow 完成後產出可供使用者繼續處理或交付的 screenshot result。 |
| Priority | `Must` |
| Dependencies | `FR-001`、`FR-002`、`FR-003` |
| Source | [Product Vision scope](PRD-0003-product-vision.md#3-product-scope)；[Core Workflow complete state](PRD-0004-core-workflow.md#5-workflow-states) |

本 requirement 不定義輸出格式、檔案副檔名、壓縮方式、儲存位置或任何 API。

## 7. Workflow Control

### FR-009 — Complete and exit the workflow

| Field | Value |
| --- | --- |
| ID | `FR-009` |
| Title | Complete and exit the workflow |
| Description | 使用者能在 capture result 完成並交付後結束本次 workflow，回到下一個工作脈絡。 |
| Priority | `Must` |
| Dependencies | `FR-003`、`FR-007`、`PRD-0004` |
| Source | [Core Workflow exit points](PRD-0004-core-workflow.md#4-exit-points)；[Product Vision goals](PRD-0003-product-vision.md#2-product-goals) |

### FR-010 — Cancel before completion

| Field | Value |
| --- | --- |
| ID | `FR-010` |
| Title | Cancel before completion |
| Description | 使用者能在 capture workflow 完成前放棄目前的操作，且不被迫完成一次不需要的 capture。 |
| Priority | `Should` |
| Dependencies | `FR-001`、`PRD-0004` Cancel boundary |
| Source | [Core Workflow exit points](PRD-0004-core-workflow.md#4-exit-points)；[Workflow analysis unknowns](../docs/Analysis/Win11/capture-workflow-analysis.md#known-unknowns) |

取消的實際觸發方式、資料 side effects 與 recovery 行為留給後續 Specs 定義。

## 8. Error Handling

### FR-011 — Provide appropriate feedback when the workflow cannot complete

| Field | Value |
| --- | --- |
| ID | `FR-011` |
| Title | Provide appropriate feedback when the workflow cannot complete |
| Description | 當 capture、result delivery 或 workflow control 無法正常完成時，使用者能收到足以理解目前狀態的適當回饋。 |
| Priority | `Must` |
| Dependencies | `FR-003`、`FR-007`、`FR-009`、`PRD-0004` Error boundary |
| Source | [Core Workflow error state](PRD-0004-core-workflow.md#5-workflow-states)；[Win11 analysis failure boundary](../docs/Analysis/Win11/capture-workflow-analysis.md#known-unknowns) |

本 requirement 不設計錯誤畫面、錯誤文字、retry policy、logging、API 或 recovery implementation。

## 9. Traceability summary

| Capability area | Requirement IDs | Product source |
| --- | --- | --- |
| Capture | `FR-001` – `FR-003` | `PRD-0003`、`PRD-0004`、Decision |
| Annotation | `FR-004` – `FR-006` | `PRD-0002`、`PRD-0004`、Decision |
| Clipboard | `FR-007` | `PRD-0004`、Decision |
| Output | `FR-008` | `PRD-0003`、`PRD-0004` |
| Workflow Control | `FR-009` – `FR-010` | `PRD-0002`、`PRD-0004` |
| Error Handling | `FR-011` | `PRD-0004`、Analysis |

## 10. Requirement boundary

- 本文件的 requirement 是 capability，不是 UI 或 implementation instruction。
- `Must`、`Should`、`Could` 是本 PRD 的唯一 priority values。
- 每一份 Spec 必須引用至少一個 `FR-` ID。
- 每一個後續 test case 應能回溯到對應的 `FR-` ID。
- 未經 PRD-0005 review 的內容不得自行進入 Specs 或 Coding。
