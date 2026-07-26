# SPEC Baseline Review

狀態：`Draft`

本文件是 Specification layer 的 Baseline Review，類似 [PRD Freeze Review](../PRD/PRD-FREEZE-REVIEW.md)。它審查既有 Spec 是否能形成可供 Architecture 引用的 v1.0 文件基線，不新增需求、不修改任何既有 Spec，也不開始 Architecture 或 Coding。

## 1. Review Scope

本次 Review 的正式範圍如下：

| Document | Role | Review status |
| --- | --- | --- |
| [SPEC-0002 Specification Guidelines](SPEC-0002-specification-guidelines.md) | Spec 結構、狀態、追溯與 Review 規則 | `Draft` |
| [SPEC-0003 System Requirements](SPEC-0003-system-requirements.md) | 共用 Session、State、Handoff 與 lifecycle 能力 | `Draft` |
| [SPEC-0004 Feature Catalog](SPEC-0004-feature-catalog.md) | 五個核心 Feature 與 FR/SR 對應 | `Draft` |
| [SPEC-0005 Capture Workflow](SPEC-0005-capture-workflow.md) | `FEAT-001` Capture Workflow | `Draft` |
| [SPEC-0006 Workflow Boundaries and Feedback](SPEC-0006-workflow-boundaries-and-feedback.md) | `FEAT-005` 共同完成、取消、失敗與回饋邊界 | `Draft` |
| [SPEC-0007 Clipboard Handoff](SPEC-0007-clipboard-handoff.md) | `FEAT-003` Capture Result 交付邊界 | `Draft` |
| [SPEC-0008 Capture Output](SPEC-0008-capture-output.md) | `FEAT-004` Output lifecycle 與交付邊界 | `Draft` |
| [SPEC-0009 Annotation Capability](SPEC-0009-annotation-capability.md) | `FEAT-002` optional Annotation capability | `Draft` |
| [SPEC-0010 Feature Integration](SPEC-0010-feature-integration.md) | 五個核心 Feature 的跨 Feature 整合 | `Draft` |

[SPEC-0001 Documentation Baseline](SPEC-0001-documentation-baseline.md) 是既有且已 `Accepted` 的文件治理基線，作為本 Review 的背景依據，不列入本次 v1.0 Feature Specification Freeze 範圍。

### Review basis

- Frozen PRD：`PRD-0002` 至 `PRD-0006`、PRD Baseline Review、PRD Traceability Matrix 與 PRD Freeze Review。
- 上述九份 Specification 文件的內容、相對連結、狀態、追溯與 Mermaid 結構。
- 靜態文件檢查結果；本 Review 不代表 runtime、build 或 test verification。

## 2. Completeness Review

Review 結果只能使用 `PASS`、`PARTIAL` 或 `FAIL`。

| Document | Expected responsibility | Required content observed | Result | Note |
| --- | --- | --- | --- | --- |
| SPEC-0002 | Specification Standard | Structure、traceability、status、naming、Mermaid、review process、acceptance rules | `PASS` | 可作為所有後續 Spec 的格式基線。 |
| SPEC-0003 | System Requirements | SR-001 至 SR-005、shared state table、state vocabulary、sequence、edge cases、AC、open questions | `PASS` | 規定 shared state，不決定 implementation。 |
| SPEC-0004 | Feature Catalog | FEAT-001 至 FEAT-005、FR/SR/NFR mapping、priority、status、future placeholder、AC | `PASS` | 五個核心 Feature 均有唯一 Feature ID。 |
| SPEC-0005 | Capture Workflow | FEAT-001 scope、preconditions、entry、state、state diagram、sequence、edge cases、AC、questions | `PASS` | 只涵蓋 Capture Workflow 與交接邊界。 |
| SPEC-0006 | Workflow Boundaries and Feedback | taxonomy、completion、cancel、failure、feedback、state transition、cross-feature responsibility、AC | `PASS` | 共同 boundary 不吞併其他 Feature 的內部行為。 |
| SPEC-0007 | Clipboard Handoff | handoff boundary、ownership、failure、Clipboard local state、sequence、privacy、AC | `PASS` | 不決定 Clipboard API、資料格式或平台實作。 |
| SPEC-0008 | Capture Output | Non-goals、output boundary、ownership、local lifecycle、failure、sequence、privacy、AC | `PASS` | 不決定檔案格式、儲存、分享或 Cloud。 |
| SPEC-0009 | Annotation Capability | Non-goals、optional boundary、local lifecycle、ownership、state、state diagram、sequence、failure、AC | `PASS` | 不決定 Annotation Tool 或子工具。 |
| SPEC-0010 | Feature Integration | responsibility matrix、integration sequence、state mapping、dependencies、gaps、AC | `PASS` | 不是新 Feature；明確保留 Clipboard/Output 平行路徑。 |

### Completeness conclusion

`PASS`。Review 範圍內的每一份文件都有明確責任、狀態、追溯入口、Acceptance Criteria 與 Open Questions，沒有發現缺少必要 Specification layer 文件的情況。

## 3. Traceability Review

### 3.1 Product requirement chain

| Product capability | FR | SR | Feature | Specification | Result |
| --- | --- | --- | --- | --- | --- |
| Capture start、selection、completion | `FR-001`、`FR-002`、`FR-003` | `SR-001`、`SR-002`、`SR-005` | `FEAT-001` | SPEC-0004 → SPEC-0005 | `PASS` |
| Optional Annotation create、modify、remove | `FR-004`、`FR-005`、`FR-006` | `SR-003`、`SR-005` | `FEAT-002` | SPEC-0004 → SPEC-0009 | `PASS` |
| Clipboard Handoff | `FR-007` | `SR-001`、`SR-004` | `FEAT-003` | SPEC-0004 → SPEC-0007 | `PASS` |
| Capture Output | `FR-008` | `SR-001`、`SR-004` | `FEAT-004` | SPEC-0004 → SPEC-0008 | `PASS` |
| Completion、Cancel、Exit | `FR-009`、`FR-010` | `SR-001`、`SR-002`、`SR-005` | `FEAT-001`、`FEAT-005` | SPEC-0005 → SPEC-0006 → SPEC-0010 | `PASS` |
| Error、failure feedback | `FR-011` | `SR-001`、`SR-005` | `FEAT-005`，各 Feature 提供 detail | SPEC-0006 → SPEC-0010 | `PASS` |

### 3.2 Non-functional requirement chain

| NFR area | IDs | Specification coverage | Result |
| --- | --- | --- | --- |
| Performance | `NFR-001` | Capture、Clipboard、Output、Annotation 與 Integration 的 Requirements Mapping/AC | `PASS` |
| Reliability | `NFR-002`、`NFR-003` | Shared boundary、各 Feature failure boundary、Integration state/dependency mapping | `PASS` |
| Usability | `NFR-004`、`NFR-005` | Windows interaction、optional Annotation、Integration optional path | `PASS` |
| Accessibility | `NFR-006` | 各 Feature 的 lifecycle/AC 與 Integration mapping | `PASS` |
| Compatibility | `NFR-007` | Capture Workflow 與 Integration 的上游 trace | `PASS` |
| Maintainability | `NFR-008`、`NFR-009` | Governance、Normative References、traceability、Review process | `PASS` |
| Extensibility | `NFR-010` | Annotation optional boundary 與 future capability boundaries | `PASS` |
| Privacy | `NFR-011` | Clipboard、Output 與 Feature Integration 的 privacy boundary | `PASS` |
| Security | `NFR-012` | Capture explicit action 與 shared workflow entry boundary | `PASS` |
| Documentation lifecycle | `NFR-013` | SPEC-0002、SPEC-0010 與本 Review 的 change/freeze policy | `PASS` |

### 3.3 Traceability gaps

下列項目是已知的 Review gap；本文件只記錄，不修正：

- 部分 NFR 是透過 upstream Spec 或 SPEC-0010 inherited trace，不一定在每一個 Feature Spec 的每一個 AC 中重複列出。
- Runtime verification 尚未證明 Windows platform、entry point、focus、display、DPI、HDR、handoff、output 或 annotation 的實際行為。
- `UNKNOWN/TBD` 尚未轉換成產品決策或正式 runtime evidence。
- Spec 文件目前仍是 `Draft`，本 Review 的 Freeze Decision 不等同於每份 Spec 已完成個別 implementation approval。

Traceability overall result：`PARTIAL`。文件鏈已建立；上述 gap 是有意保留的產品、平台與 lifecycle 未決項目，不是由本 Review 自行補完。

## 4. Consistency Review

| Consistency area | Review observation | Result |
| --- | --- | --- |
| Shared State | SPEC-0005、SPEC-0006、SPEC-0007、SPEC-0008、SPEC-0009、SPEC-0010 均引用 SPEC-0003 shared vocabulary；local lifecycle 另行標示。 | `PASS` |
| Feature Boundary | SPEC-0006 定義共同 boundary；SPEC-0007、SPEC-0008、SPEC-0009 各自保留內部責任；SPEC-0010 建立矩陣。 | `PASS` |
| Primary Owner | SPEC-0010 每個 scenario 都列出唯一 Primary Owner。 | `PASS` |
| Optional Annotation | SPEC-0003、SPEC-0009 與 SPEC-0010 都保留可略過 Annotation 的基本路徑。 | `PASS` |
| Clipboard/Output relationship | SPEC-0007、SPEC-0008、SPEC-0010 將兩者視為同一 Result 的平行 downstream paths，不建立彼此必要依賴。 | `PASS` |
| Acceptance Criteria | 各 Feature 使用文件專屬 AC namespace；未發現同一 namespace 內的重複或相互矛盾要求。 | `PASS` |
| Scope discipline | 沒有在本 Review scope 中建立 Tool、Overlay、Toolbar、Architecture 或 Coding requirement。 | `PASS` |

No inconsistency identified.

## 5. Outstanding Gaps

本節整理所有仍保留的 `UNKNOWN`、`TBD` 與 Runtime Verification，並不提供解法：

| Area | Outstanding gap | Current status | Impact |
| --- | --- | --- | --- |
| Capture entry | `PrtSc`、Windows shortcut、Start/app entry 的 SnipPlus runtime 行為 | `UNKNOWN/TBD` | Architecture 只能保留 entry boundary，不能推導平台實作。 |
| Selection | Invalid/zero selection、Cancel、focus、multi-monitor、DPI、HDR | `UNKNOWN/TBD` | Selection implementation contract 尚未形成。 |
| Annotation | v1.0 是否啟用、進入/略過/完成/取消、preservation、history、undo、redo | `UNKNOWN/TBD` | 不得拆 Annotation 子工具或預設 editor model。 |
| Clipboard | Consumer acceptance、handoff failure、retry、result preservation、existing content | `UNKNOWN/TBD` | 不得推導 Clipboard API 或資料格式。 |
| Output | Output Ready/Delivered/Completed 的 runtime 判定、consumer、preservation、retry | `UNKNOWN/TBD` | 不得推導 File IO、storage 或 output format。 |
| Cross-feature flow | Clipboard/Output 是否可同時執行、failure 後另一條路徑是否繼續 | `UNKNOWN/TBD` | Integration 只保留平行路徑，不新增順序依賴。 |
| Shared boundary | Recoverable/Terminal classification、Cancel trigger、Error feedback channel、Exit side effects | `UNKNOWN/TBD` | Architecture 不得把未決語意寫成確定狀態機。 |
| Privacy | Result preservation、storage、sync、share、external processing 的正式產品界線 | `TBD` | 不得假設 cloud 或外部傳輸。 |
| Verification | Runtime、build、test 與 platform evidence 尚未建立 | `UNKNOWN` | 本 Review 只確認文件基線，不宣稱可執行性。 |

## 6. Readiness Assessment

可用狀態固定為：`Ready for Architecture`、`Conditionally Ready`、`Not Ready`。

### Assessment

**`Ready for Architecture`**

理由：

- SPEC-0002 至 SPEC-0010 的責任範圍與必要章節已完成。
- PRD v1.0 已 Freeze；Spec 沒有新增產品需求。
- 五個核心 Feature 都有唯一 Feature ID、FR/SR/NFR trace 與 Feature Spec。
- SPEC-0010 已將 Primary Owner、Shared State、平行 downstream paths 與跨 Feature gaps 集中記錄。
- 所有未決項目已明確標示 `UNKNOWN/TBD`，不需要在本 Review 內偷偷補完。
- 靜態文件連結與 whitespace checks 已通過；這不是 runtime/build/test verification。

## 7. Freeze Decision

可用決策固定為：`Freeze Approved`、`Freeze Deferred`。

### Decision

**`Freeze Approved`**

Freeze 範圍只包括：

- 本 Review Scope 列出的 SPEC-0002 至 SPEC-0010 文件基線。
- 五個核心 Feature 的責任、邊界、Shared State vocabulary、Integration relationship 與既有 traceability。
- 已記錄的 `UNKNOWN/TBD` 與 Cross-feature Gaps。

Freeze 不包括：

- Architecture technology choice。
- API、Class、Framework、Storage、UI、Tool、Output format 或 implementation。
- Runtime、build、test、package、deploy 或 release readiness。
- 將各份 Spec 的 `Draft` 狀態直接改成 `Approved`。

## 8. Architecture Entry Criteria

Architecture 工作開始前必須滿足：

- 本文件的 `Freeze Approved` 結論被接受為 Specification baseline。
- 不再新增 Feature，也不改變五個核心 Feature 的 boundary 或 Primary Owner。
- Architecture 文件只能引用 Frozen Spec 與其明確的 `UNKNOWN/TBD`，不得自行把未決項目寫成已決定行為。
- Architecture 中的每一個 capability、component、boundary 與 major decision 都能回溯至本 Review Scope 的 Spec。
- 若 Architecture 發現產品需求或 Feature boundary 缺口，必須先走 Change Policy，不得在 Architecture 內偷偷新增需求。
- Architecture 與 ADR 必須與本 Specification baseline 分開管理；本 Review 不預先決定技術方案。

## 9. Change Policy

Specification Freeze 後，任何涉及產品需求、Feature boundary、Shared State、Traceability 或 Acceptance Criteria 的修改，都必須遵循：

```text
Research
  ↓
Analysis
  ↓
Decision
  ↓
PRD Change（若涉及產品需求）
  ↓
Spec Change Request
  ↓
Review
  ↓
Approve
```

禁止直接修改 Frozen PRD 或已凍結 Spec，再用 Architecture、Coding、Test 或 Changelog 掩蓋變更來源。

## 10. Review Conclusion

Specification v1.0 基線已具備進入 Architecture 的文件條件；已知產品、平台與 runtime 缺口保留為明確的 `UNKNOWN/TBD`，不阻擋 Architecture 開始，但也不構成任何 implementation authorization。

本 Review 完成後停止。不得在本文件中建立 Architecture、ADR、程式碼、測試、Arrow、Rectangle、Text、Toolbar、Overlay 或其他子 Feature Spec。
