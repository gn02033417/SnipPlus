# PRD Baseline Review

狀態：`Draft`

本文件是 SnipPlus PRD v1.0 的正式審核報告。它只記錄審查結果，不新增需求、不修改既有 PRD，也不重新設計產品。

## 1. Review Scope

本次審查範圍：

- [PRD-0002 User Experience Principles](PRD-0002-user-experience-principles.md)
- [PRD-0003 Product Vision](PRD-0003-product-vision.md)
- [PRD-0004 Core Workflow](PRD-0004-core-workflow.md)
- [PRD-0005 Functional Requirements](PRD-0005-functional-requirements.md)
- [PRD-0006 Non-functional Requirements](PRD-0006-non-functional-requirements.md)

審查方式：只讀檢查文件內容、章節、ID、Source、Dependencies、跨文件引用與責任邊界；沒有執行 runtime、build、test 或 application code。

## 2. Completeness Review

狀態只能使用 `PASS`、`PARTIAL` 或 `FAIL`。

| Document area | Status | Review result |
| --- | --- | --- |
| UX Principles | `PASS` | PRD-0002 已建立十項 UX 原則，並明確排除功能、UI、技術與效能數值設計。 |
| Product Vision | `PASS` | PRD-0003 已包含 Product Statement、Goals、Scope、Target Users、Principles、Success Criteria 與 Assumptions。 |
| Core Workflow | `PASS` | PRD-0004 已包含 Workflow Scope、Primary Workflow Mermaid、Entry Points、Exit Points、States、User Journey、Constraints 與 Open Questions。 |
| Functional Requirements | `PASS` | PRD-0005 的 FR-001 至 FR-011 具備唯一 ID、Priority、Dependencies 與 Source，且停留在 capability 層級。 |
| Non-functional Requirements | `PASS` | PRD-0006 的 NFR-001 至 NFR-013 具備唯一 ID、Priority、Dependencies 與 Source，且沒有技術方案或數值效能目標。 |

## 3. Traceability Review

### Traceability chain

```text
Research → Analysis → Decision → FR → NFR
                       ↓          ↓
                      PRD-0002 → PRD-0003 → PRD-0004
```

### Review result

整體狀態：`PARTIAL`

已確認：

- Win11 的 Research、Analysis 與 Decision 文件彼此有明確連結。
- FR-001 至 FR-008 可透過 Decision、PRD-0003 或 PRD-0004 找到產品依據。
- FR-004 至 FR-006 明確引用 Annotation 的 Decision 與 PRD-0002 optional 原則。
- NFR-001 至 NFR-013 都有 `NFR-` ID、Dependencies 與 Source。
- NFR-004、NFR-005、NFR-006、NFR-007、NFR-010 明確引用或依賴 PRD-0002 UX Principles。
- PRD-0005 已建立「未來 Spec 與 test case 必須回溯 FR」的要求。

待補追溯項目：

- `FR-009`、`FR-010`、`FR-011` 的直接 Source 目前主要是 PRD-0004 或 Analysis，沒有在 requirement table 直接列出對應的 Decision 文件；可透過 PRD-0004 的 Source boundary 間接追溯，但直接鏈結仍不完整。
- `NFR-002`、`NFR-003`、`NFR-008`、`NFR-009`、`NFR-011`、`NFR-012`、`NFR-013` 沒有全部直接引用 PRD-0002；它們主要依賴 PRD-0003、PRD-0004、PRD-0005 或 Repository governance。這不一定是產品矛盾，但不符合「所有 NFR 都能直接追溯到 UX Principles」的完整條件。
- NFR-009 與 NFR-013 的 Dependencies 使用了 `AGENTS.md` 或 Development Guide 類的治理文件；若後續維持「Dependencies 只能引用 Research、Analysis、Decision、PRD」的限制，需在變更流程中重新檢視這兩項欄位。

本次只列出缺漏，不修正任何文件。

## 4. Consistency Review

### Cross-document checks

| Check | Status | Result |
| --- | --- | --- |
| UX Principles ↔ NFR | `PASS` | NFR-004、NFR-005、NFR-010 與 UX 原則保持一致；沒有要求進階能力阻擋基本流程。 |
| Workflow ↔ FR | `PASS` | FR-001 至 FR-003、FR-007、FR-009、FR-010 對應 PRD-0004 的主要狀態與出口。 |
| Scope ↔ Vision | `PASS` | PRD-0004 的 static image core workflow 位於 PRD-0003 Windows Desktop 與 screenshot workflow 範圍內。 |
| Annotation optionality | `PASS` | PRD-0002、PRD-0003、PRD-0004、PRD-0005 與 NFR-005 都保留 Annotation 的 optional 性質。 |
| Windows-first direction | `PASS` | PRD-0002 Principle 8、PRD-0003 Scope 與 NFR-007 沒有互相矛盾。 |
| Privacy / cloud boundary | `PASS` | PRD-0003 將 cloud 類能力列為目前範圍外，NFR-011 沒有自行加入 cloud implementation。 |

No inconsistency identified.

上述 `PARTIAL` 只代表 traceability 尚有缺口，不代表已發現 PRD 內容互相矛盾。

## 5. Open Items

本節只整理目前的 `UNKNOWN`、`TBD` 與 `Assumption`，不解決它們。

### UNKNOWN

- Runtime 未驗證。
- `PrtSc / PrintScreen` 在目標 Windows 環境的實際流程。
- Region Selection 的取消、最小範圍、focus、DPI、多螢幕與 HDR 行為。
- Capture、Clipboard、Notification、Editor、Save、Share 與 Close 的 failure、recovery 與 side effects。
- Screenshot result 的正式輸出格式、保存方式與生命週期。
- Accessibility 的完整支援矩陣與驗證方法。
- 產品的最低 Windows version、硬體條件與支援政策。

### TBD

- 正式支援的 entry points 與 capture scopes。
- Annotation 是否在 v1.0 實作，以及其正式 capability 範圍。
- 可量化的 performance、reliability、accessibility 與 success metrics。
- Privacy、retention、local handling、外部傳輸與分享政策。
- Technical stack、deployment、permissions、storage 與 release policy。
- FR-009 至 FR-011 與 Decision 文件的直接追溯補強方式。

### Assumptions

- 主要使用者已具有部分 Windows 截圖操作經驗。
- Windows 行為可能隨版本更新而改變。
- 後續 runtime research 可能修正部分 Research、Analysis 或 Decision。
- Product Vision 不足以直接建立可執行的 Spec。

## 6. Risks

本節只列風險，不提出解法：

- Runtime 尚未驗證，文件中的平台行為可能與目標環境不同。
- Windows 行為可能隨版本更新，造成 Research、Decision 與 PRD 的來源漂移。
- 官方文件對取消、錯誤、recovery、DPI、多螢幕與 PrtSc 行為的資訊不足或存在差異。
- Traceability 尚有間接鏈結，開始 Specs 後可能造成需求來源判斷不一致。
- PRD 全部仍為 `Draft`，若未完成 Freeze 就開始 Specs，可能產生返工。
- Success Criteria 尚未量化，後續可能難以判斷產品是否達成願景。
- Privacy、Accessibility、Compatibility 與 Reliability 的細節尚未定義，可能影響後續 scope。

## 7. Readiness Assessment

`Conditionally Ready`

理由：五份 PRD 的內容、章節與責任邊界完整，跨文件沒有發現矛盾；但 Traceability Review 仍為 `PARTIAL`，且存在未解的 `UNKNOWN`、`TBD` 與治理欄位邊界。只有在這些缺口被接受或依變更流程處理後，才適合將 PRD 作為 Specs 的唯一產品來源。

## 8. Freeze Decision

`Freeze Deferred`

PRD v1.0 目前不凍結。此判斷不修改任何 PRD，也不新增產品需求；它只表示目前 Baseline Review 尚未達到無條件 Freeze 的門檻。

## 9. Change Policy

PRD Freeze 後，任何修改必須依照以下鏈結進行：

```text
Research → Analysis → Decision → PRD Change → Review → Approve
```

- 不得直接修改已 Freeze 的 PRD。
- 新增或變更需求必須先有對應的 Research、Analysis 或 Decision 證據。
- 變更後必須重新檢查 FR、NFR、Traceability 與跨文件一致性。
- Specs 只能引用已 Review 通過的 PRD 與 requirement IDs。

## Review outcome

本次 Baseline Review 的結論是：內容完整、跨文件無矛盾，但追溯性仍為 `PARTIAL`，因此 `Conditionally Ready`，`Freeze Deferred`。本報告完成後不開始 Specs、Architecture、ADR 或 Coding。
