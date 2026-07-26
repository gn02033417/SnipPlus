# SPEC-0004 Feature Catalog

狀態：`Draft`

## Purpose

本文件建立 SnipPlus 的 Feature Catalog，回答「目前 PRD v1.0 定義了哪些產品 Feature，以及每個 Feature 如何追溯到 FR 與 SR」。

本文件只建立目錄，不描述 Feature 如何實作，也不建立任何 Feature Spec。

## Scope

Catalog 只收錄已存在於 PRD-0005 Functional Requirements 與 SPEC-0003 System Requirements 的能力群組。沒有 FR、SR 或 PRD 來源的項目不加入 Catalog。

以下識別碼保持獨立：

| Identifier type | Format | Meaning |
| --- | --- | --- |
| Document ID | `SPEC-NNNN` | 一份 Specification 文件。 |
| Feature ID | `FEAT-NNN` | 一個產品 Feature。 |
| Requirement ID | `FR-NNN`、`NFR-NNN`、`SR-NNN`、`AC-NNN` | 需求、系統能力或驗收條件。 |

Feature status `Candidate` 表示已由 PRD 能力整理出來，但尚未建立並 Approved 對應的 Feature Spec；它不是 implementation commitment。

## Feature Catalog

| Feature ID | Name | FR | SR | Priority | Status | Purpose | Dependencies | Future Spec |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `FEAT-001` | Capture Workflow | `FR-001`、`FR-002`、`FR-003`、`FR-009`、`FR-010` | `SR-001`、`SR-002`、`SR-005` | `Must` | `Candidate` | 讓使用者啟動、選取、完成或取消一次 capture workflow。 | [PRD-0004 Core Workflow](../PRD/PRD-0004-core-workflow.md)；[SPEC-0003 System Requirements](SPEC-0003-system-requirements.md) | `SPEC-0101-capture-workflow.md` placeholder |
| `FEAT-002` | Annotation | `FR-004`、`FR-005`、`FR-006` | `SR-003` | `Should` | `Candidate` | 提供 capture result 的 optional 後續標註能力。 | [PRD-0002 UX Principles](../PRD/PRD-0002-user-experience-principles.md)；[PRD-0005 Annotation](../PRD/PRD-0005-functional-requirements.md#3-annotation)；[SPEC-0003](SPEC-0003-system-requirements.md#sr-003--optional-annotation-state) | `SPEC-0102-annotation.md` placeholder |
| `FEAT-003` | Clipboard Handoff | `FR-007` | `SR-004` | `Must` | `Candidate` | 將完成的 capture result 交付至 clipboard 的產品能力。 | [PRD-0005 Clipboard](../PRD/PRD-0005-functional-requirements.md#5-clipboard)；[SPEC-0003 Clipboard Handoff](SPEC-0003-system-requirements.md#sr-004--clipboard-handoff) | `SPEC-0103-clipboard-handoff.md` placeholder |
| `FEAT-004` | Capture Output | `FR-008` | `SR-001`、`SR-004` | `Must` | `Candidate` | 產出可供使用者繼續處理或交付的 screenshot result。 | [PRD-0003 Product Scope](../PRD/PRD-0003-product-vision.md#3-product-scope)；[PRD-0005 Output](../PRD/PRD-0005-functional-requirements.md#6-output) | `SPEC-0104-capture-output.md` placeholder |
| `FEAT-005` | Workflow Boundaries and Feedback | `FR-009`、`FR-010`、`FR-011` | `SR-001`、`SR-002`、`SR-005` | `Must` | `Candidate` | 維持完成、取消、錯誤與離開等 workflow boundary，並提供適當狀態回饋。 | [PRD-0004 Exit Points](../PRD/PRD-0004-core-workflow.md#4-exit-points)；[PRD-0005 Error Handling](../PRD/PRD-0005-functional-requirements.md#8-error-handling) | `SPEC-0105-workflow-boundaries.md` placeholder |

## Feature classification

### Included

以上五個 Feature 都能回溯到已 Freeze 的 PRD capability 與 SPEC-0003 的 SR。它們是目錄分類，不是新的需求。

### Not cataloged as independent Features

以下項目目前不獨立列為 Feature，因為目前文件沒有足夠的 FR/SR 來源支持它們成為獨立產品能力：

- Overlay。
- Toolbar。
- Arrow、Rectangle 或其他 annotation tool。
- OCR、AI、Plugin。
- History。
- Cloud sync、team collaboration、cloud storage。
- Video capture。

這些名稱可能在未來成為 UI element、module、optional capability 或新的 Feature，但本文件不替它們做決策。

## Feature traceability

```text
PRD-0005 FR
     ↓
SPEC-0003 SR
     ↓
FEAT-NNN Catalog entry
     ↓
Future Feature Spec placeholder
```

規則：

- 每個 Catalog entry 必須至少引用一個 `FR-` 與一個 `SR-`；若某項只適用於產品能力，必須在 Gap 或 Open Questions 說明。
- Feature ID 不與 Document ID 或 Requirement ID 共用編號序列。
- Feature 合併、拆分或改名時，既有 Feature ID 不重用；需要由後續 Review 決定新舊 ID 關係。
- `Future Spec` 只是一個名稱 placeholder，不代表文件已建立或已授權實作。
- 每份 Feature Spec 必須引用本 Catalog 的 `FEAT-NNN`、相關 `FR-`、`SR-` 與 `NFR-`。

## Future Spec placeholders

以下只預留追溯名稱，不建立檔案：

| Feature ID | Future Spec placeholder | File status |
| --- | --- | --- |
| `FEAT-001` | Capture Workflow | Placeholder only |
| `FEAT-002` | Annotation | Placeholder only |
| `FEAT-003` | Clipboard Handoff | Placeholder only |
| `FEAT-004` | Capture Output | Placeholder only |
| `FEAT-005` | Workflow Boundaries and Feedback | Placeholder only |

## Open Questions

- `TBD`：Capture Workflow 是否應在未來拆成多個 Feature。
- `TBD`：Annotation 是否在 v1.0 進入 Approved Feature Spec。
- `TBD`：Capture Output 與 Clipboard Handoff 是否維持兩個獨立 Feature。
- `UNKNOWN`：未來 runtime verification 是否會導致 Feature scope 或 grouping 改變。
- `TBD`：未來是否需要額外的 Feature ID status，例如 Deprecated 或 Superseded；目前只使用 `Candidate`。

本文件不回答以上問題，也不建立任何功能 Spec。

## Acceptance Criteria

| ID | Acceptance criterion | Traces to |
| --- | --- | --- |
| `AC-001` | 每個 Catalog entry 都有唯一 `FEAT-NNN`、Name、FR、SR、Priority、Status、Purpose、Dependencies 與 Future Spec placeholder。 | `SPEC-0002`、`NFR-008` |
| `AC-002` | Catalog 中的 Feature 能回溯到 PRD-0005 的 capability 與 SPEC-0003 的 SR。 | `FEAT-001` – `FEAT-005`、`SR-001` – `SR-005` |
| `AC-003` | Overlay、Toolbar、工具、AI、Plugin、History、Cloud 與 Video 等未被核准的項目沒有被建立成 Feature Spec。 | `PRD-0003`、`PRD-0005`、`NFR-013` |
| `AC-004` | Future Spec 只以 placeholder 出現，沒有在 `Specs/` 建立功能文件。 | `SPEC-0002`、`NFR-008` |

## Review status

- Status：`Draft`
- Product review：`TBD`
- Engineering review：`TBD`
- Test review：`TBD`
- Approval date：`TBD`
