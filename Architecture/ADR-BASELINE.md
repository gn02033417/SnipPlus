# ADR Baseline

## Document Control

| Field | Value |
| --- | --- |
| Document ID | ADR-BASELINE |
| Title | Architecture Decision Record Framework |
| Document Type | ADR Governance Framework |
| Status | Draft |
| Architecture Stability | Draft |
| Version | 0.1 |
| Owner | TBD |
| Last reviewed | Not reviewed |
| Normative References | ARCH-0001、ARCH-0002、ARCH-0003、ARCH-0004、ARCH-0005、ARCH-BASELINE-REVIEW |
| Informative References | PRD-FREEZE-REVIEW、SPEC-BASELINE-REVIEW、Architecture/adr/ADR-0001-documentation-first.md |

本文件是 ADR Framework，不是真正的 Architecture Decision Record。它不消耗任何數字 ADR ID，也不替既有 ADR-0001 重新分類或改寫。

## 1. Purpose

本文件定義 SnipPlus 未來所有 Architecture Decision Record 的共同格式、生命週期、識別碼、追溯規則、Review 規則與變更邊界。

目標是讓每一份長期架構決策都能回答：

- 為什麼需要這個決策。
- 哪些選項曾被考慮。
- 取捨與後果是什麼。
- 決策如何追溯至 Frozen PRD、Frozen Specs 或 Architecture。
- 什麼情況需要取代、廢止或重新審查。

## 2. Scope

本 Framework 涵蓋：

- ADR Lifecycle。
- ADR ID Policy。
- Decision Categories。
- Required Sections。
- Decision evidence 與 review evidence。
- Architecture → ADR → Implementation traceability。
- ADR Change Policy。
- ADR Acceptance Criteria。
- ADR 與 Frozen PRD、Frozen Specs、Architecture 的邊界。

## 3. Non-goals

本文件不得：

- 建立任何真正的 ADR。
- 做 Framework、Rendering、Clipboard、Storage 或 Testing 的技術決策。
- 選擇 WinUI、WPF、Avalonia 或其他 UI framework。
- 選擇 C#、.NET 或其他 Language/Runtime 版本。
- 選擇 Graphics API。
- 選擇 Clipboard API。
- 建立 Project Structure。
- 建立 Interface、Class、Service、API 或 Source code。
- 修改 Frozen PRD、Frozen Specs 或已核准的 Architecture baseline。
- 將方便實作直接當作 Architecture Decision。

## 4. ADR Lifecycle

每一份真正 ADR 必須依下列生命週期管理：

~~~text
Draft
  ↓
Review
  ↓
Accepted
  ↓
Superseded
  ↓
Deprecated
~~~

生命週期狀態定義：

| Status | Meaning | Allowed action |
| --- | --- | --- |
| Draft | 決策文件正在建立，尚未完成 Review。 | 可以補充 Context、Options、Trade-offs 與 Evidence。 |
| Review | 文件已提交審查，內容等待責任人確認。 | 可以提出 review comments；不得當作已核准決策使用。 |
| Accepted | 決策已通過 Review，成為目前有效的架構決策。 | Implementation 與後續 Architecture 文件可以引用。 |
| Superseded | 已由新的 Accepted ADR 取代。 | 保留歷史內容；必須連結取代它的 ADR。 |
| Deprecated | 決策不再適用，且沒有新的有效取代關係。 | 保留歷史內容；不得作為新的實作依據。 |

生命週期規則：

- 只有 Accepted ADR 可以作為目前有效的長期技術決策依據。
- Draft 或 Review ADR 不得被描述為已決定的技術基線。
- Superseded ADR 不得刪除，必須保留 Superseded by 關係。
- Deprecated ADR 不得刪除，必須保留失效原因與日期。
- 重新討論既有 Accepted Decision 時，建立新的 ADR 或 Change Request，不直接覆寫原文件。
- ADR-BASELINE 本身是 Framework，不套用上述技術決策狀態轉換。

## 5. ADR ID Policy

真正的 ADR 使用：

~~~text
ADR-0002-short-decision-name.md
ADR-0003-another-decision-name.md
~~~

識別碼規則：

- 使用 ADR-NNNN 格式，NNNN 為四位數字。
- 每個數字 ID 永不重用。
- Superseded 或 Deprecated 的 ID 永久保留。
- 檔名使用小寫 kebab-case decision name。
- 一份 ADR 只處理一個重大架構決策。
- ADR ID 不得與 ARCH、MOD、COMP、FEAT、SPEC、FR、SR 或 AC ID 混用。
- 真正 ADR 儲存於 Architecture/adr/。
- ADR-BASELINE 位於 Architecture/ 根目錄，代表治理 Framework，不是數字 ADR。
- 既有 ADR-0001 保持原位置與原 ID；若未來需要重新審查，依本 Framework 建立新的 ADR 或 Review Change Request。

## 6. Decision Categories

Decision Category 用來分類決策，不代表任何類別已經做出選擇。

| Category | Scope |
| --- | --- |
| Framework | 長期應用 framework、runtime 或基礎平台的選擇。 |
| Rendering | 顯示、繪製、視覺輸出與 rendering boundary 的長期取捨。 |
| Clipboard | Clipboard handoff、consumer boundary 與平台交付方式的長期取捨。 |
| Storage | Output storage、檔案保存、狀態保存或外部儲存邊界的長期取捨。 |
| Testing | 測試策略、驗證層級、測試工具或測試環境的長期取捨。 |
| Platform Integration | OS、輸入、焦點、顯示器與平台互動邊界的長期取捨。 |
| Interaction | Component Interaction、Contract、狀態轉移或跨邊界協作的長期取捨。 |
| Performance | Responsiveness、資源使用或長期效能策略的長期取捨。 |
| Reliability | 失敗分類、復原、資料保護或中斷處理的長期取捨。 |
| Security and Privacy | 權限、資料暴露、隱私邊界與安全模型的長期取捨。 |
| Accessibility | 可存取性、回饋、輸入與使用者完成流程的長期取捨。 |
| Observability | Diagnostics、檢查證據、診斷輸出或維護可見性的長期取捨。 |
| Deployment and Operations | Build、release、distribution、support 或運維邊界的長期取捨。 |

若一份 ADR 同時涉及多個 Category，必須明確說明主要 Category；不得用多個 Category 掩蓋多個彼此獨立的決策。

## 7. Required Sections

每一份真正 ADR 必須包含下列章節：

### 7.1 Document Control

至少包含：

- Document ID。
- Title。
- Status。
- Decision Category。
- Version。
- Owner。
- Date proposed。
- Date reviewed。
- Date accepted。
- Supersedes。
- Superseded by。
- Normative References。
- Informative References。

### 7.2 Context

說明：

- 需要做決策的問題。
- 觸發決策的 Architecture requirement、Open Question 或 Finding。
- 目前的邊界與已知限制。
- 不做決策的風險。

Context 不得把未核准的猜測寫成現況事實；未知內容必須標示 UNKNOWN 或 TBD。

### 7.3 Decision Drivers

列出影響決策的因素，例如：

- Frozen PRD 或 Frozen Spec 的必要約束。
- Architecture Layer、Module、Component 或 Interaction boundary。
- Maintainability。
- Reliability。
- Security and Privacy。
- Accessibility。
- Performance。
- Operational cost。
- Reversibility。

Decision Drivers 必須能追溯至來源文件或明確的架構需求。

### 7.4 Options Considered

至少列出：

- Option name。
- Option description。
- Advantages。
- Disadvantages。
- Constraint conflicts。
- Evidence status。

未選用的 Option 仍要保留，避免未來重複分析相同取捨。

### 7.5 Decision

只記錄一個主要決策：

- Selected option。
- Decision statement。
- Scope of applicability。
- Explicit exclusions。

若尚未完成 Review，Decision 必須維持 Draft，不得使用 Accepted 的語氣。

### 7.6 Trade-offs

明確記錄：

- 得到什麼。
- 放棄什麼。
- 新增什麼風險。
- 降低什麼風險。
- 哪些後果是可接受的。
- 哪些後果需要後續 ADR、Spec 或驗證。

### 7.7 Consequences

至少分成：

- Positive consequences。
- Negative consequences。
- Neutral consequences。
- Follow-up work。
- Revisit conditions。

Consequence 不得把尚未授權的 Coding task 當作既定實作計畫。

### 7.8 Traceability

至少包含：

- Architecture source。
- PRD/Spec source，若該決策涉及產品需求或功能行為。
- Decision evidence。
- Implementation reference，若已存在。
- Verification evidence，若已存在。
- Related ADR。

### 7.9 Review Record

至少包含：

- Reviewer。
- Review date。
- Review result。
- Open comments。
- Resolution of comments。
- Acceptance authority。

### 7.10 Change and Supersession

至少包含：

- What would invalidate this decision。
- What would trigger a new ADR。
- Supersedes。
- Superseded by。
- Deprecated reason，若適用。

## 8. ADR Template

未來建立真正 ADR 時，使用以下最小結構：

~~~markdown
# ADR-NNNN Decision Title

## Document Control

| Field | Value |
| --- | --- |
| Document ID | ADR-NNNN |
| Title | Decision Title |
| Status | Draft |
| Decision Category | TBD |
| Version | 0.1 |
| Owner | TBD |
| Date proposed | YYYY-MM-DD |
| Date reviewed | Not reviewed |
| Date accepted | Not accepted |
| Supersedes | None |
| Superseded by | None |
| Normative References | TBD |
| Informative References | TBD |

## Context

## Decision Drivers

## Options Considered

## Decision

## Trade-offs

## Consequences

## Traceability

## Review Record

## Change and Supersession
~~~

Template 中的 TBD、None 與 Not reviewed 必須依實際狀態更新；不得將 placeholder 留在 Accepted ADR 的必要決策欄位。

## 9. Traceability Policy

ADR 的正式來源鏈：

~~~text
Architecture requirement or unresolved architecture question
  ↓
ADR
  ↓
Implementation
  ↓
Verification evidence
~~~

與產品治理鏈的關係：

~~~text
Frozen PRD
  ↓
Frozen Specs
  ↓
Architecture
  ↓
ADR
  ↓
Implementation
~~~

規則：

- ADR 必須引用至少一個 Architecture requirement、Architecture Open Question 或 Architecture Finding。
- 若決策影響產品功能或使用者可見行為，必須引用對應 Frozen PRD 與 Frozen Spec。
- ADR 不得只引用 Research 作為唯一決策依據。
- Implementation reference 在尚未實作時標示 Not implemented 或 TBD，不得虛構路徑。
- Verification evidence 在尚未驗證時標示 Not verified 或 TBD。
- ADR 不得反向修改 Frozen PRD、Frozen Specs 或 Architecture；若發現衝突，先建立 Change Request。
- 每個 Accepted ADR 必須能被未來的 Implementation 與 Review 文件反向引用。

## 10. Review Rules

Review ADR 時至少確認：

- 是否只處理一個重大決策。
- Context 是否描述真實問題而非實作偏好。
- Decision Drivers 是否有來源。
- Options 是否包含合理替代方案。
- Trade-offs 是否具體且雙向。
- Consequences 是否包含負面後果。
- 是否沒有偷偷新增 Feature、Module、Component 或產品需求。
- 是否沒有繞過 Frozen PRD、Frozen Specs 或 Architecture Change Policy。
- 是否保留 UNKNOWN、TBD 與驗證限制。
- 是否能追溯到後續 Implementation 與 Verification evidence。
- 是否說明何時需要重新審查或建立新 ADR。

Review 結果只能是：

~~~text
Approved
Changes requested
Rejected
~~~

Review 結果 Approved 只是表示文件可以進入 Accepted 流程；實際 Status 仍必須依 Document Control 與 Lifecycle 記錄。

## 11. Change Policy

Architecture 或產品來源變更時，必須依下列順序判斷：

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
ADR（若涉及長期架構取捨）
  ↓
Review
  ↓
Approve
~~~

ADR 變更規則：

- 不直接覆寫已 Accepted 的 ADR。
- 只要核心 Decision 改變，就建立新的 ADR。
- 新 ADR 必須標示 Supersedes 舊 ADR。
- 舊 ADR 改為 Superseded，保留歷史內容。
- 只修正拼字、連結或非語意格式時，可以依文件維護規則修改，但不得改變 Decision 或 Consequences。
- 若變更會影響 Frozen PRD、Frozen Specs 或 Architecture ownership，必須先回到相應 Change Request。

## 12. Acceptance Criteria

真正 ADR 必須符合：

- 使用未重複的 ADR-NNNN ID。
- 位於 Architecture/adr/。
- Status 與 Lifecycle 狀態一致。
- 包含所有 Required Sections。
- 只包含一個主要決策。
- Decision Category 已標示。
- Context、Decision Drivers、Options、Trade-offs 與 Consequences 彼此一致。
- 所有來源、相關 ADR、Implementation 與 Verification reference 都有明確狀態。
- 沒有將 UNKNOWN、TBD 或未驗證內容寫成已確認事實。
- 沒有新增未授權的產品需求。
- 沒有直接修改 Frozen PRD、Frozen Specs 或 Architecture baseline。
- 有 Reviewer、Review date、Review result 與 acceptance authority。
- Accepted ADR 有清楚的 Supersession 與 Revisit conditions。
- Markdown relative links 與 git diff --check 通過。

## 13. Governance Boundaries

ADR Framework 與其他文件的責任邊界：

| Artifact | Owns | Does not own |
| --- | --- | --- |
| Frozen PRD | Product goals、scope、user-visible requirements | Technology trade-offs |
| Frozen Specs | Feature/system behavior、state、acceptance criteria | Long-term technology selection |
| Architecture | Layer、Module、Component、Interaction boundaries | Specific technical implementation |
| ADR | A single long-term architecture or technology decision | Product requirement rewrite |
| Implementation | Concrete realization of Accepted decisions | Rewriting the decision record |
| Verification evidence | Evidence that behavior or decision was checked | Declaring an unreviewed decision Accepted |

若 Artifact 之間發生衝突，依 Change Policy 處理，不由 Implementation 直接改寫治理文件。

## 14. Completion Boundary

完成 ADR-BASELINE 只代表：

- ADR Lifecycle 已定義。
- ADR ID Policy 已定義。
- Decision Categories 已定義。
- Required Sections 與 Template 已定義。
- Traceability Policy 已定義。
- Review Rules、Change Policy 與 Acceptance Criteria 已定義。
- Governance boundaries 已定義。

完成 ADR-BASELINE 不代表：

- 已建立任何真正 ADR。
- 已選擇 Framework、Rendering、Clipboard、Storage 或 Testing 技術。
- 已選擇 WinUI、WPF、Avalonia、C#、.NET 或 Graphics API。
- 已建立 Project Structure。
- 已建立 Interface、Class、Service、API 或 Source code。
- 可以開始 Coding。

## 15. Prohibited Decisions

本 Framework 不得建立或決定：

- 任何具體 ADR。
- 任何 Framework、Rendering、Clipboard、Storage、Testing 或 Platform implementation。
- 任何 Language、Runtime、Graphics API 或 Clipboard API。
- Project、Assembly、Namespace 或 deployment structure。
- Interface、Class、Service、API、Event、Command 或 Message schema。
- Overlay、Toolbar、Annotation tools 或 Screenshot coding。

