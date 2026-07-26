# PRD-0006 Non-functional Requirements

狀態：`Draft`

## 1. 文件定位

本文件回答唯一問題：SnipPlus 應具備哪些品質屬性（Quality Attributes）？

本文件不描述如何寫、不決定 API、UI framework、renderer、plugin SDK、技術架構或任何工程實作，也不設定數值效能目標。

每個 requirement 都有唯一 `NFR-` ID，並可追溯到既有的 Research、Analysis、Decision 或 PRD。未來 Specs 與 Architecture 必須尊重本文件核准後的品質邊界。

## 2. Requirement format

| Field | Meaning |
| --- | --- |
| ID | 穩定且唯一的 Non-functional Requirement identifier。 |
| Title | 品質屬性的簡短名稱。 |
| Description | 使用者或維護者可理解的品質要求，不包含 implementation。 |
| Priority | 只使用 `Must`、`Should` 或 `Could`。 |
| Dependencies | 直接影響本品質要求的既有文件或產品邊界。 |
| Source | 支持此品質要求的 Research、Analysis、Decision 或 PRD 來源。 |

## 3. Performance

### NFR-001 — Keep the primary workflow responsive

| Field | Value |
| --- | --- |
| ID | `NFR-001` |
| Title | Keep the primary workflow responsive |
| Description | Capture、Region Selection、Complete 與 Clipboard Ready 的主要流程應保持流暢，不應讓使用者感受到不必要的等待。 |
| Priority | `Must` |
| Dependencies | `FR-001`、`FR-002`、`FR-003`、`FR-007`、`PRD-0002` Principle 9 |
| Source | [PRD-0002 Principle 9](PRD-0002-user-experience-principles.md#principle-9--先快再增加功能)；[PRD-0004 primary workflow](PRD-0004-core-workflow.md#2-primary-workflow) |

本 requirement 不設定 milliseconds、percentile、throughput 或任何其他數值門檻。

## 4. Reliability

### NFR-002 — End interrupted workflows safely

| Field | Value |
| --- | --- |
| ID | `NFR-002` |
| Title | End interrupted workflows safely |
| Description | 當 workflow 被取消、中斷或無法完成時，流程應能安全結束，不使使用者被迫停留在不明確的狀態。 |
| Priority | `Must` |
| Dependencies | `FR-010`、`FR-011`、`PRD-0004` Cancel and Error states |
| Source | [PRD-0004 exit points](PRD-0004-core-workflow.md#4-exit-points)；[PRD-0004 workflow states](PRD-0004-core-workflow.md#5-workflow-states) |

### NFR-003 — Protect the user’s current work context

| Field | Value |
| --- | --- |
| ID | `NFR-003` |
| Title | Protect the user’s current work context |
| Description | Capture workflow 的中斷、失敗或結束不應不必要地破壞使用者當前的工作脈絡。 |
| Priority | `Must` |
| Dependencies | `FR-009`、`FR-010`、`FR-011`、`PRD-0003` Product Goals |
| Source | [PRD-0003 Product Goals](PRD-0003-product-vision.md#2-product-goals)；[PRD-0004 open questions](PRD-0004-core-workflow.md#8-open-questions) |

## 5. Usability

### NFR-004 — Preserve familiar Windows interaction patterns

| Field | Value |
| --- | --- |
| ID | `NFR-004` |
| Title | Preserve familiar Windows interaction patterns |
| Description | 主要 capture workflow 應符合 Windows 使用者已熟悉的操作習慣，並保持基本流程容易理解。 |
| Priority | `Must` |
| Dependencies | `FR-001`、`FR-002`、`FR-003`、`PRD-0002` Principles 1、3、5、8 |
| Source | [PRD-0002 UX Principles](PRD-0002-user-experience-principles.md)；[PRD-0003 Product Statement](PRD-0003-product-vision.md#1-product-statement) |

### NFR-005 — Keep advanced capabilities optional

| Field | Value |
| --- | --- |
| ID | `NFR-005` |
| Title | Keep advanced capabilities optional |
| Description | 進階能力不應成為使用者完成基本 capture、Complete 或 Clipboard Ready 流程的必要前置條件。 |
| Priority | `Must` |
| Dependencies | `FR-004`、`FR-005`、`FR-006`、`PRD-0002` Principles 2、4、10 |
| Source | [PRD-0002 Principle 4](PRD-0002-user-experience-principles.md#principle-4--進階能力全部是-optional)；[PRD-0004 workflow scope](PRD-0004-core-workflow.md#1-workflow-scope) |

## 6. Accessibility

### NFR-006 — Support accessible completion of the primary workflow

| Field | Value |
| --- | --- |
| ID | `NFR-006` |
| Title | Support accessible completion of the primary workflow |
| Description | 主要 capture workflow 應能以符合 Windows Accessibility expectations 的方式被理解與完成，且不把單一輸入方式視為唯一可能路徑。 |
| Priority | `Should` |
| Dependencies | `FR-001`、`FR-002`、`FR-009`、`PRD-0002` Principles 1、5、8 |
| Source | [PRD-0002 UX Principles](PRD-0002-user-experience-principles.md)；[PRD-0004 User Journey](PRD-0004-core-workflow.md#6-user-journey) |

具體 accessibility standard、支援矩陣、輸入方式與驗證方法仍為 `TBD`，本 requirement 不設計 UI。

## 7. Compatibility

### NFR-007 — Prioritize Windows Desktop compatibility

| Field | Value |
| --- | --- |
| ID | `NFR-007` |
| Title | Prioritize Windows Desktop compatibility |
| Description | 產品的主要相容性目標應是 Windows Desktop 使用情境，並維持與 Windows 原生操作習慣相容的產品方向。 |
| Priority | `Must` |
| Dependencies | `PRD-0003` Product Scope、`PRD-0004` Entry Points、`PRD-0005` Functional Requirements |
| Source | [PRD-0003 Product Scope](PRD-0003-product-vision.md#3-product-scope)；[PRD-0002 Principle 8](PRD-0002-user-experience-principles.md#principle-8--windows-first暫不以跨平台為優先) |

最低 Windows version、支援版本策略、硬體條件與跨平台未來範圍仍為 `TBD`。

## 8. Maintainability

### NFR-008 — Preserve documentation-first traceability

| Field | Value |
| --- | --- |
| ID | `NFR-008` |
| Title | Preserve documentation-first traceability |
| Description | 產品、需求、規格、架構與測試之間應維持可追溯鏈結；每個未來 Spec 與 test case 都應能回溯到對應的 FR 或 NFR。 |
| Priority | `Must` |
| Dependencies | `PRD-0005` Requirements Traceability、Repository documentation rules |
| Source | [PRD-0005 Traceability summary](PRD-0005-functional-requirements.md#9-traceability-summary)；[Development Guide](../docs/guides/development-guide.md) |

### NFR-009 — Keep product documentation maintainable

| Field | Value |
| --- | --- |
| ID | `NFR-009` |
| Title | Keep product documentation maintainable |
| Description | 長期維護時，產品文件應保持明確責任分界、狀態標記、來源連結與開放問題，不以文件完整度假裝未確認內容已定案。 |
| Priority | `Must` |
| Dependencies | `AGENTS.md`、Research、Analysis、Decision、PRD layers |
| Source | [Repository rules](../AGENTS.md)；[Research framework](../docs/Research/README.md)；[Decision framework](../docs/Decision/README.md) |

## 9. Extensibility

### NFR-010 — Allow future capabilities without breaking the primary workflow

| Field | Value |
| --- | --- |
| ID | `NFR-010` |
| Title | Allow future capabilities without breaking the primary workflow |
| Description | 未來新增能力應能與主要 capture workflow 保持清楚邊界，不迫使使用者改變基本工作習慣或使用進階能力。 |
| Priority | `Should` |
| Dependencies | `FR-004`、`FR-005`、`FR-006`、`PRD-0002` Principles 2、4、10、`PRD-0004` |
| Source | [PRD-0002 UX Principles](PRD-0002-user-experience-principles.md)；[PRD-0003 Product Goals](PRD-0003-product-vision.md#2-product-goals) |

本 requirement 不設計 plugin、extension point、SDK 或 module architecture。

## 10. Privacy

### NFR-011 — Keep capture handling within an explicit privacy boundary

| Field | Value |
| --- | --- |
| ID | `NFR-011` |
| Title | Keep capture handling within an explicit privacy boundary |
| Description | Capture result 的保存、交付、分享或外部傳輸都必須有明確的產品邊界與使用者理解，不應在未經明確決策的情況下假設 cloud storage、cloud sync 或外部處理。 |
| Priority | `Must` |
| Dependencies | `FR-003`、`FR-007`、`PRD-0003` Out of Scope、`PRD-0005` Output |
| Source | [PRD-0003 Out of Scope](PRD-0003-product-vision.md#out-of-scope-for-the-current-product-definition)；[PRD-0005 Output](PRD-0005-functional-requirements.md#6-output) |

本 requirement 不決定 local storage、cloud policy、retention、encryption 或任何技術方案；具體隱私政策仍為 `TBD`。

## 11. Security

### NFR-012 — Require an explicit user action before capture

| Field | Value |
| --- | --- |
| ID | `NFR-012` |
| Title | Require an explicit user action before capture |
| Description | 系統不應在沒有使用者明確啟動 capture workflow 的情況下擷取畫面。 |
| Priority | `Must` |
| Dependencies | `FR-001`、`PRD-0004` Entry Points、`NFR-011` |
| Source | [PRD-0004 Entry Points](PRD-0004-core-workflow.md#3-entry-points)；[PRD-0005 Start capture workflow](PRD-0005-functional-requirements.md#fr-001--start-capture-workflow) |

本 requirement 不指定 Windows API、permission model、process boundary 或 security implementation。

## 12. Constraints

### NFR-013 — Respect the product documentation lifecycle

| Field | Value |
| --- | --- |
| ID | `NFR-013` |
| Title | Respect the product documentation lifecycle |
| Description | 後續產品與工程工作應依照 `Research → Analysis → Decision → PRD → Specs → Architecture → Coding` 的責任順序，不能跳過已定義的產品依據。 |
| Priority | `Must` |
| Dependencies | `PRD-0002`、`PRD-0003`、`PRD-0004`、`PRD-0005`、`AGENTS.md` |
| Source | [Repository rules](../AGENTS.md)；[Development Guide](../docs/guides/development-guide.md)；[Decision framework](../docs/Decision/README.md) |

本 requirement 是產品與文件治理約束，不是工程架構或 implementation instruction。

## 13. Traceability summary

| Quality area | Requirement IDs | Product source |
| --- | --- | --- |
| Performance | `NFR-001` | `PRD-0002`、`PRD-0004` |
| Reliability | `NFR-002` – `NFR-003` | `PRD-0003`、`PRD-0004` |
| Usability | `NFR-004` – `NFR-005` | `PRD-0002`、`PRD-0003`、`PRD-0004` |
| Accessibility | `NFR-006` | `PRD-0002`、`PRD-0004` |
| Compatibility | `NFR-007` | `PRD-0002`、`PRD-0003` |
| Maintainability | `NFR-008` – `NFR-009` | `PRD-0005`、Repository rules |
| Extensibility | `NFR-010` | `PRD-0002`、`PRD-0003` |
| Privacy | `NFR-011` | `PRD-0003`、`PRD-0005` |
| Security | `NFR-012` | `PRD-0004`、`PRD-0005` |
| Constraints | `NFR-013` | Repository rules、Development Guide |

## 14. Requirement boundary

- 本文件描述 quality attributes，不描述 technical solution。
- `Must`、`Should`、`Could` 是唯一 priority values。
- 本文件不設定數值效能目標。
- 本文件不決定 WPF、WinUI、C#、API、Clipboard API、Renderer、Plugin SDK 或其他技術。
- 後續 Specs 必須能追溯到 `FR-` 或 `NFR-` ID。
- 未經 PRD Baseline Review 通過，不開始 Specs 或 Coding。
