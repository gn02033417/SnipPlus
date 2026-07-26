# Technology Decision Roadmap

## Document Control

| Field | Value |
| --- | --- |
| Document ID | TECHNOLOGY-DECISION-ROADMAP |
| Title | Technology Decision Roadmap |
| Document Type | Decision Backlog and Ordering Roadmap |
| Status | Accepted |
| Architecture Stability | Draft |
| Version | 1.0 |
| Owner | Repository owner |
| Last reviewed | 2026-07-26 |
| Normative References | ARCH-0001、ARCH-0002、ARCH-0003、ARCH-0004、ARCH-0005、ARCH-BASELINE-REVIEW、ADR-BASELINE |
| Informative References | PRD-FREEZE-REVIEW、SPEC-BASELINE-REVIEW、ROADMAP.md、TODO.md、REPOSITORY-READINESS-AUDIT-001 |

本文件管理重大技術決策的 backlog、依賴與順序。它不是技術選擇本身；真正有效的決策必須記錄於 `Accepted` ADR。

## 1. Purpose

本 Roadmap 回答：

> SnipPlus 還有哪些重大技術決策未完成，哪些已 Accepted，以及下一個最有價值的決策是什麼？

目標：

- 依 Frozen Architecture 排序決策。
- 讓 Accepted upstream decision 解鎖下游主題。
- 保留 Candidate、UNKNOWN、TBD 與 runtime verification gap。
- 避免以 prerequisite／closure 文件取代真正 ADR。
- 防止未經決策就建立 Project 或開始 Coding。

## 2. Decision Lifecycle

~~~text
Candidate
  ↓
Ready
  ↓
Draft ADR
  ↓
Review
  ↓
Accepted
~~~

| Status | Meaning | Allowed action |
| --- | --- | --- |
| Candidate | 已知需要決策，但 context、evidence 或 upstream dependency 尚未完成。 | 補齊 context、drivers、options、risks 與 dependencies。 |
| Ready | 建立 ADR 的必要來源已足夠。 | 建立一份只處理單一主題的 Draft ADR。 |
| Draft ADR | 真正 ADR 已存在但尚未通過 Review。 | 補齊內容並提交 Review。 |
| Review | Reviewer 正在確認 decision、trade-offs、evidence 與 boundaries。 | 解決 comments；不得當成有效基線。 |
| Accepted | ADR 已獲 acceptance authority 接受。 | 下游 ADR、Contract、Project Structure 與 Implementation planning 可以引用。 |
| Deferred | 題目仍有效，但不屬於目前 vertical slice 的必要前置。 | 保留原因與重新啟動條件。 |
| Rejected | 候選方向被拒絕。 | 保留理由；若問題仍存在，建立新候選或 ADR。 |

## 3. Decision Backlog

| Decision ID | Topic | Priority | Depends On | Status | Effective artifact |
| --- | --- | --- | --- | --- | --- |
| TD-001 | UI Framework | P0 | Architecture Freeze、ADR Framework | Accepted | ADR-0002 — WinUI 3 |
| TD-002 | Rendering Technology | P0 | TD-001、COMP-014、COMP-018 boundary | Candidate | None |
| TD-003 | Capture Backend | P0 | TD-001、TD-002、MOD-008 boundary | Candidate | None |
| TD-004 | Clipboard Integration | P0 | COMP-009、COMP-015、TD-001 | Candidate | None |
| TD-005 | Image Representation | P0 | TD-003、TD-004、COMP-006 boundary | Candidate | None |
| TD-006 | Plugin Architecture | P2 | TD-001、Feature／Module stability | Candidate | None |
| TD-007 | Configuration | P1 | TD-001、ADR-BASELINE、runtime boundary | Candidate | None |
| TD-008 | Logging | P1 | TD-001、TD-007、failure ownership | Candidate | None |
| TD-009 | Telemetry | P2 | TD-008、Security and Privacy review | Candidate | None |
| TD-010 | Packaging | P1 | TD-001、TD-007、TD-011 | Candidate | None |
| TD-011 | Testing Strategy | P0 | TD-001、TD-002、Architecture boundaries | Candidate | None |
| TD-012 | Update Strategy | P2 | TD-010、TD-011、Deployment and Operations scope | Candidate | None |

Priority：

| Priority | Meaning |
| --- | --- |
| P0 | 影響核心 workflow 或多個下游決策；implementation readiness 前必須 Accepted 或明確 Deferred。 |
| P1 | 影響 maintainability、verification、delivery 或 operations。 |
| P2 | 可在核心 vertical slice 穩定後處理，不得提前成為必要依賴。 |

Decision ID 永不重用。拆分或取代主題時必須保留原始 ID 的歷史狀態。

## 4. Current Decision State

### Completed

- **TD-001 UI Framework：Accepted**
- Effective ADR：[ADR-0002 UI Framework Selection](adr/ADR-0002-ui-framework-selection.md)
- Accepted option：WinUI 3
- Acceptance date：2026-07-26
- Implementation authorized：No

ADR-0002 只固定 UI host framework，不決定 Language、Runtime、Windows App SDK version、Rendering、Capture、Clipboard、Packaging、Testing 或 Project Structure。

### Next decision

**TD-002 Rendering Technology** 是目前下一個主要 P0 decision。

原因：

- TD-001 已 Accepted。
- Rendering 會影響 Capture Result、annotation、display／DPI／HDR verification 與 Image Representation。
- TD-003 Capture Backend 與 TD-011 Testing Strategy 需要可描述的 rendering boundary。
- 既有 `docs/Research/Technology/10–18` 可直接作為 decision evidence，不需要新增 readiness 或 closure chain。

### Parallel preparation allowed

TD-004 Clipboard Integration 可以整理 Decision Context 與 options，但在 Image Representation 與 Shared Result contract 尚未釐清前，不應假設具體 clipboard payload。

TD-011 Testing Strategy 可以先建立 coverage goals，但測試工具與 runtime matrix 應等待 Rendering／Capture decisions。

## 5. Recommended Decision Order

### Phase 1 — Core platform and workflow foundation

1. ~~TD-001 UI Framework~~ — **Accepted through ADR-0002**。
2. TD-002 Rendering Technology。
3. TD-003 Capture Backend。
4. TD-004 Clipboard Integration。
5. TD-005 Image Representation。
6. TD-011 Testing Strategy。

TD-004 與 TD-005 可以交錯分析，但必須以明確 contracts 分離 UI framework、image representation 與 clipboard delivery concerns。

### Phase 2 — Maintainability and delivery foundation

1. TD-007 Configuration。
2. TD-008 Logging。
3. TD-010 Packaging。
4. TD-012 Update Strategy。

### Phase 3 — Deferred extensibility and operations

1. TD-006 Plugin Architecture。
2. TD-009 Telemetry。

Phase 3 不得反向改變 Frozen Feature、Module 或 Component ownership。

## 6. Candidate → Ready Criteria

Decision Candidate 只有在以下條件成立時才能進入 Ready：

- Decision Context 明確。
- 可引用 Architecture requirement、finding 或 unresolved question。
- 若影響產品行為，具有 Frozen PRD／Spec reference。
- Upstream dependencies 已 Accepted，或依賴風險已明確接受。
- Decision Drivers 可驗證或引用。
- 至少兩個合理方向可比較；若只有一個方向，必須說明其他方向為何不合格。
- Non-goals 明確。
- 風險、reversibility、migration 和 rollback impact 已記錄。
- Runtime evidence 狀態標示為 Verified、Not verified 或 Not required。
- 不會偷偷新增產品需求或改變 Architecture ownership。

## 7. ADR Acceptance Requirements

ADR 進入 Accepted 前必須：

- 符合 ADR-BASELINE Required Sections。
- 一份 ADR 只處理一個重大決策。
- 具有 Review Record、reviewer、review date、review result 與 acceptance authority。
- Options、trade-offs、positive／negative／neutral consequences 完整。
- 所有關鍵 UNKNOWN／TBD 都被解釋或轉成 follow-up decision。
- Supersedes／Superseded by 關係明確。
- 不要求修改 Frozen PRD、Specs 或 Architecture；若需要，先走 Change Flow。
- Roadmap 對應 Decision ID 同步更新。

## 8. Freeze Boundary

Technology Decision 不得直接改寫：

- Frozen PRD 的 goals、scope、FR、NFR 或 UX Principles。
- Frozen Specs 的 Feature、state、acceptance criteria 或 behavior boundary。
- ARCH-0002 的 Layer dependency direction。
- ARCH-0003 的 Module ownership。
- ARCH-0004 的 Component ownership 與 Shared State access policy。
- ARCH-0005 的 Interaction ownership、prohibited interactions 與 Clipboard／Output parallel boundary。
- COMP-001 作為唯一 Shared State Authority。
- Annotation optional boundary。

若技術選擇迫使上述內容改變，必須先啟動：

~~~text
Research／Evidence
  ↓
Analysis
  ↓
Decision
  ↓
PRD Change（如需要）
  ↓
Spec Change（如需要）
  ↓
Architecture Change Request
  ↓
ADR
  ↓
Review and Accept
~~~

## 9. Anti-proliferation Rule

Decision 不因 `Not ready` 而自動衍生新的 prerequisite、readiness reassessment、authorization request 或 closure review。

新增文件必須帶來至少一項 materially new input：

- 新 official／external evidence；
- 新 human decision；
- Accepted upstream change；
- 新 runtime／prototype evidence；
- 不同的 decision boundary。

既有 Research 應直接整併至 ADR 的 Context、Options、Evidence、Trade-offs 和 Consequences。

## 10. Open Questions

- TD-002 是否選擇 Win2D、SkiaSharp、Composition／Direct2D 組合，或不同分層方案。
- TD-003 是否需要將 capture API 與 capture orchestration 拆成不同 ADR／contract。
- TD-004 的 clipboard payload、consumer compatibility、retry 和 ownership boundary。
- TD-005 的 in-memory image representation、pixel format、alpha、color space 和 lifetime。
- TD-011 的 unit、integration、platform、visual 和 runtime verification depth。
- TD-007 是否只涵蓋 app settings，或也包含 deployment／diagnostic configuration。
- TD-008 與 TD-009 的 privacy、retention 和 opt-in boundary。
- TD-010 的 packaged／unpackaged、framework-dependent／self-contained 和 distribution strategy。

## 11. Change Policy

- Decision ID 不重用。
- Accepted decision 的核心內容不直接覆寫；改變核心選擇時建立新 ADR，並標示 Supersedes。
- Roadmap status 必須與 ADR lifecycle 一致。
- Roadmap 可以調整順序與依賴，但不得自己做技術選擇。
- 影響產品需求、行為或 Architecture ownership 時，必須回到相應 change process。

## 12. Current Outcome

`TD-001 Accepted; remaining implementation-critical decisions incomplete.`

目前最短路徑：

1. 建立並 Review TD-002 Rendering Technology ADR。
2. 依 Rendering 決策推進 TD-003、TD-005 與 TD-011。
3. 以 Shared Result／Image Result contract 約束 TD-003、TD-004、TD-005。
4. 完成必要 P0 ADR、contracts 與 Project Structure 後，執行一次 Implementation Readiness Review。

本 Roadmap 不授權 Coding、Restore、Build、Test、Package 或 Runtime execution。
