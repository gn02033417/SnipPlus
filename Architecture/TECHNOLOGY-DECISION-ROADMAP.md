# Technology Decision Roadmap

## Document Control

| Field | Value |
| --- | --- |
| Document ID | TECHNOLOGY-DECISION-ROADMAP |
| Title | Technology Decision Roadmap |
| Document Type | Decision Backlog and Ordering Roadmap |
| Status | Draft |
| Architecture Stability | Draft |
| Version | 0.1 |
| Owner | TBD |
| Last reviewed | Not reviewed |
| Normative References | ARCH-0001、ARCH-0002、ARCH-0003、ARCH-0004、ARCH-0005、ARCH-BASELINE-REVIEW、ADR-BASELINE |
| Informative References | PRD-FREEZE-REVIEW、SPEC-BASELINE-REVIEW、ROADMAP.md、TODO.md |

本文件是 Decision Roadmap，不是 ADR，也不是 Technology Selection。它只列出未來可能需要 ADR 的重大技術主題、前置條件、依賴關係與建議先後順序。

## 1. Purpose

本 Roadmap 用於回答：

> SnipPlus 後續有哪些重大技術決策需要 ADR，以及它們的建議先後順序是什麼？

它讓後續技術決策可以：

- 依 Frozen Architecture 的責任邊界排序。
- 先處理會影響多個下游決策的基礎主題。
- 明確保留尚未成熟的 Candidate、UNKNOWN 與 TBD。
- 避免未經排序就直接建立 ADR 或開始 Coding。

## 2. Scope

本文件只列出：

- 需要進入 ADR 的重大技術主題。
- 每個主題的 Decision ID。
- Priority。
- Depends On。
- 目前 Status。
- 建議的決策順序。
- Prerequisites 與 Freeze Boundary。
- 尚未能回答的 Open Questions。

本文件不包含任何具體技術選擇、比較結論、實作設計或程式碼。

## 3. Decision Pipeline

所有 Technology Decision Candidate 依下列流程前進：

~~~text
Candidate
  ↓
Ready
  ↓
ADR
  ↓
Review
  ↓
Accepted
~~~

Pipeline 狀態定義：

| Status | Meaning | Allowed action |
| --- | --- | --- |
| Candidate | 已知可能需要決策，但前置條件或問題定義尚未完成。 | 補齊 prerequisites、sources、dependencies 與 open questions。 |
| Ready | Context、Decision Drivers 與必要的 Frozen source 已足夠，可以建立真正 ADR。 | 建立一份只處理單一主題的 ADR。 |
| ADR | 已建立真正 ADR，等待 Review。 | 依 ADR-BASELINE 完成 Review；不得當作 Accepted 決策。 |
| Accepted | 真正 ADR 已通過 Review，成為有效決策。 | 下游文件與 Implementation 可以引用。 |

目前 Roadmap 中的項目全部維持 Candidate；本文件不建立任何真正 ADR，也不把任何項目升級為 Ready 或 Accepted。

## 4. Decision Backlog

| Decision ID | Topic | Priority | Depends On | Status |
| --- | --- | --- | --- | --- |
| TD-001 | UI Framework | P0 | Architecture Freeze、ADR Framework | Candidate |
| TD-002 | Rendering Technology | P0 | TD-001、COMP-014、COMP-018 boundary | Candidate |
| TD-003 | Capture Backend | P0 | TD-001、TD-002、MOD-008 boundary | Candidate |
| TD-004 | Clipboard Integration | P0 | COMP-009、COMP-015、TD-001 | Candidate |
| TD-005 | Image Representation | P0 | TD-003、TD-004、COMP-006 boundary | Candidate |
| TD-006 | Plugin Architecture | P2 | TD-001、Feature/Module stability | Candidate |
| TD-007 | Configuration | P1 | TD-001、ADR-BASELINE、runtime boundary | Candidate |
| TD-008 | Logging | P1 | TD-001、TD-007、failure ownership | Candidate |
| TD-009 | Telemetry | P2 | TD-008、Security and Privacy review | Candidate |
| TD-010 | Packaging | P1 | TD-001、TD-007、TD-011 | Candidate |
| TD-011 | Testing Strategy | P0 | TD-001、TD-002、Architecture boundaries | Candidate |
| TD-012 | Update Strategy | P2 | TD-010、TD-011、Deployment and Operations scope | Candidate |

Priority 定義：

| Priority | Meaning |
| --- | --- |
| P0 | 會影響核心流程或多個下游技術決策，應優先準備。 |
| P1 | 會影響 maintainability、verification 或 packaging，但可在核心基礎後處理。 |
| P2 | 可在核心產品路徑穩定後處理，不能提前變成必要依賴。 |

Decision ID 永不重用。若未來拆分或合併主題，必須建立新的 Decision ID 並保留原始 ID 的歷史狀態。

## 5. Recommended Decision Order

以下是建議排序，不是技術決策：

### Phase 0 — Governance readiness

前置條件：

- Architecture v1.0 Baseline 已獲 Freeze Approved。
- ADR-BASELINE 已完成 Review。
- Frozen PRD 與 Frozen Specs 不因技術排序而改寫。
- 所有 Candidate、UNKNOWN、TBD 與 Open Findings 保持可見。

### Phase 1 — Core platform and workflow foundations

建議順序：

1. TD-001 UI Framework。
2. TD-002 Rendering Technology。
3. TD-003 Capture Backend。
4. TD-004 Clipboard Integration。
5. TD-005 Image Representation。

這一階段只決定需要先處理的主題順序；不預設任何選項，也不宣告任何主題 Ready。

### Phase 2 — Verification and maintainability foundations

建議順序：

1. TD-011 Testing Strategy。
2. TD-007 Configuration。
3. TD-008 Logging。

這些主題的實際順序仍需依 Phase 1 的 ADR 結果與新的 Open Questions 重新檢查。

### Phase 3 — Delivery and extensibility

建議順序：

1. TD-010 Packaging。
2. TD-006 Plugin Architecture。
3. TD-012 Update Strategy。
4. TD-009 Telemetry。

Phase 3 不得反向迫使核心 Feature、Module 或 Component ownership 改變。

## 6. Prerequisites

任何 Decision Candidate 要變成 Ready，至少必須具備：

- 明確的 Decision Context。
- 對應的 Architecture requirement、Architecture Finding 或 unresolved architecture question。
- 對應的 Frozen PRD/Frozen Spec reference，若主題影響產品功能或行為。
- 已知的上游 Decision ID 與依賴關係。
- 可列出的 Decision Drivers。
- 至少一個合理的替代方向；不要求本 Roadmap 先列出選項。
- 明確的 Non-goals。
- 已知的風險、不可逆程度與回退影響。
- 目前 runtime evidence 的狀態；尚未驗證時標示 UNKNOWN/TBD。
- 確認不會直接修改 Feature、Module、Component 或 Interaction ownership。

建立真正 ADR 前，還必須確認：

- ADR-BASELINE 已通過 Review。
- 題目只包含一個重大決策。
- 沒有正在處理的上游 Change Request。
- 依賴的上游 ADR 已 Accepted，或明確記錄為尚未完成的前置條件。

## 7. Freeze Boundary

下列內容在 Architecture Freeze 後不可由 Technology Decision Roadmap 直接改寫：

- Frozen PRD 的產品目標與產品範圍。
- Frozen Specs 的功能行為、狀態與驗收邊界。
- ARCH-0002 的四層責任與依賴方向。
- ARCH-0003 的 Module ownership 與 Feature-to-Module mapping。
- ARCH-0004 的 Component ownership 與 Shared State access policy。
- ARCH-0005 的 Interaction ownership、禁止互動與平行 downstream boundary。
- COMP-001 是唯一 Shared State Authority。
- Annotation 維持 Optional。
- Clipboard 與 Output 維持平行 downstream。

若某個技術選擇會迫使上述內容改變，必須先啟動：

~~~text
Research
  ↓
Analysis
  ↓
Decision
  ↓
PRD Change（若涉及產品需求）
  ↓
Spec Change（若涉及功能或行為）
  ↓
Architecture Change Request
  ↓
ADR
  ↓
Review
  ↓
Approve
~~~

Roadmap 不得跳過 Change Policy，也不得以「技術方便」作為跳過 Frozen source 的理由。

## 8. Decision Readiness Rules

### 8.1 Candidate → Ready

只有在下列條件都成立時，項目才能標示 Ready：

- Prerequisites 已完成或有明確 evidence。
- Depends On 的上游項目已 Accepted，或依賴風險已被明確批准保留。
- Decision Context 已穩定。
- Decision Drivers 可被驗證或引用。
- 不會偷偷新增產品需求。
- 不會直接改變 Architecture ownership。

### 8.2 Ready → ADR

標示 Ready 後，才可以建立對應的 ADR-NNNN。

建立 ADR 時：

- 一個 Decision ID 對應一份主要 ADR。
- ADR 必須引用本 Roadmap 的 Decision ID。
- ADR 必須依 ADR-BASELINE 的 Required Sections。
- Roadmap 的 Status 保持 ADR，直到真正 ADR 進入 Accepted 或被退回。

### 8.3 ADR → Accepted

只有真正 ADR 完成 Review 並由責任人接受後，才能標示 Accepted。

Accepted ADR 必須：

- 具有 Review Record。
- 具有完整 Traceability。
- 記錄 Trade-offs 與 Consequences。
- 記錄 Supersedes、Superseded by 或 None。
- 不得留下未解釋的關鍵 TBD。

## 9. Dependency Notes

依賴關係只描述先後條件，不代表被依賴項目已經決策：

- TD-001 是多數 platform-facing 主題的共同前置候選。
- TD-002 的 Context 應引用 rendering boundary，但本 Roadmap 不選 rendering technology。
- TD-003 的 Context 應引用 MOD-008、COMP-014 與 Capture Result boundary。
- TD-004 的 Context 應引用 MOD-009、COMP-015 與 Clipboard Handoff boundary。
- TD-005 的 Context 應引用 COMP-006 與 downstream consumers，但不定義任何格式。
- TD-011 應覆蓋 Architecture boundary 與後續 runtime verification，不預設測試工具。
- TD-010 與 TD-012 應在 delivery/operations scope 明確後才進入 Ready。
- TD-006、TD-009 不能在核心 workflow 尚未穩定前成為必要依賴。

若依賴關係在後續 Analysis、Decision 或 ADR Review 中改變，必須保留變更原因與來源。

## 10. Open Questions

以下問題維持 UNKNOWN/TBD，不在 Roadmap 內回答：

- 哪些 Candidate 會實際進入 ADR。
- P0 項目的精確先後是否需要依 runtime evidence 調整。
- TD-001 與 TD-002 是否需要分成更多獨立決策。
- TD-003 Capture Backend 是否需要多份 ADR。
- TD-004 Clipboard Integration 是否與 TD-005 Image Representation 共享同一個 Decision Context。
- TD-006 Plugin Architecture 是否屬於產品未來範圍。
- TD-007 Configuration 的 scope 是否包含 user settings、runtime state 或 deployment settings。
- TD-008 Logging 與 TD-009 Telemetry 的 privacy boundary。
- TD-010 Packaging 與 TD-012 Update Strategy 的發佈責任。
- TD-011 Testing Strategy 的 runtime verification depth。
- 是否需要新增 Security and Privacy、Accessibility 或 Performance 的獨立 Decision ID。
- Decision ID 是否需要與外部 issue、task 或 release 進行對應。

## 11. Roadmap Change Policy

本 Roadmap 可以新增、拆分、合併或調整 Decision Candidate，但每次變更必須：

- 保留既有 Decision ID 的歷史識別。
- 不重用已 Deprecated、Superseded 或移除的 ID。
- 記錄變更原因與來源。
- 不把 Roadmap change 當成真正的技術決策。
- 不直接修改 Frozen PRD、Frozen Specs 或 Architecture baseline。
- 若變更涉及產品需求，先回到 PRD Change Request。
- 若變更涉及功能或行為，先回到 Spec Change Request。
- 若變更涉及長期架構取捨，建立真正 ADR。

## 12. Acceptance Criteria

本 Roadmap 的完成條件：

- TECHNOLOGY-DECISION-ROADMAP.md 存在。
- Document Control、Purpose、Scope、Decision Pipeline、Decision Backlog、Prerequisites、Freeze Boundary 與 Open Questions 均存在。
- Decision Pipeline 明確為 Candidate → Ready → ADR → Review → Accepted。
- Decision Backlog 至少包含 TD-001 至 TD-012。
- 每個 Decision 都有 Topic、Priority、Depends On 與 Status。
- Roadmap 只列決策主題與順序，不包含任何技術選擇。
- 所有項目目前維持 Candidate，沒有建立真正 ADR。
- 沒有選擇 WinUI、WPF、Avalonia、Skia、Windows Graphics Capture、GDI、C#、.NET 或其他技術。
- 沒有修改 Frozen PRD、Frozen Specs 或 Architecture baseline。
- 沒有建立 Project Structure、Interface、Class、API 或程式碼。
- Markdown relative links 與 git diff --check 通過。

## 13. Completion Boundary

完成本文件不代表：

- 任何 Technology Decision 已完成。
- 任何真正 ADR 已建立。
- 任何 Candidate 已升級為 Ready 或 Accepted。
- Framework、Rendering、Clipboard、Storage 或 Testing 技術已選擇。
- Project Structure 已建立。
- Interface Contract 已建立。
- 可以開始 Coding。

本文件完成後，下一步必須由 Review 指定要檢查或推進哪一個 Decision Candidate；不得自行建立 ADR-0002。

## 14. Prohibited Decisions

本 Roadmap 不得選擇或決定：

- WinUI。
- WPF。
- Avalonia。
- Skia。
- Windows Graphics Capture。
- GDI。
- C#。
- .NET。
- 任何 Graphics API。
- 任何 Clipboard API。
- 任何 Storage implementation。
- 任何 Testing framework。
- 任何 Project Structure。
- 任何 Source code。

