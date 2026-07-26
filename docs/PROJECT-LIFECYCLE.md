# SnipPlus Repository Project Lifecycle

## Document Control

| Field | Value |
| --- | --- |
| Document ID | PROJECT-LIFECYCLE |
| Title | SnipPlus Repository Project Lifecycle |
| Document Type | Repository Governance Overview |
| Status | Draft |
| Version | 0.1 |
| Owner | TBD |
| Last reviewed | Not reviewed |
| Normative References | AGENTS.md、PRD-FREEZE-REVIEW、SPEC-BASELINE-REVIEW、ARCH-BASELINE-REVIEW、ADR-BASELINE |
| Informative References | README.md、docs/index.md、ROADMAP.md、TODO.md、CHANGELOG.md |

## 1. Purpose

本文件描述 SnipPlus Repository 的完整文件生命週期，讓維護者知道：

- 每個階段的目的。
- 每個階段接受什麼輸入。
- 每個階段產出什麼文件或證據。
- 進入與離開階段需要哪些條件。
- 哪些 Freeze gate 已經固定邊界。
- 變更應該回到哪一個來源層級。

本文件是 Repository 的最高層治理總覽，不取代 PRD、Specs、Architecture、ADR、Implementation 或 Verification 文件。

## 2. Scope

本文件只描述文件生命週期與治理邊界：

- Research。
- Analysis。
- Decision。
- PRD。
- PRD Freeze。
- Specification。
- Specification Freeze。
- Architecture。
- Architecture Freeze。
- ADR。
- Implementation。
- Verification。
- Release。

本文件不是：

- 開發流程。
- Coding tutorial。
- Technology Selection。
- Project Structure。
- Feature Specification。
- Architecture design。
- Build、Test 或 Deployment instruction。

## 3. Repository Lifecycle

~~~mermaid
flowchart LR
    R["Research"]
    A["Analysis"]
    D["Decision"]
    P["PRD"]
    PF["PRD Freeze"]
    S["Specification"]
    SF["Specification Freeze"]
    AR["Architecture"]
    AF["Architecture Freeze"]
    ADR["ADR"]
    I["Implementation"]
    V["Verification"]
    RL["Release"]

    R --> A --> D --> P --> PF --> S --> SF --> AR --> AF --> ADR --> I --> V --> RL
~~~

生命週期是一條治理來源鏈，不表示每個需求都必須在每個階段產生相同數量的文件。任何階段的變更都必須保留其來源、影響範圍與 Review evidence。

## 4. Phase Definitions

### 4.1 Research

| Field | Definition |
| --- | --- |
| Purpose | 收集外部行為、使用者情境、平台事實與可靠來源。 |
| Inputs | 使用者問題、官方文件、可驗證的外部觀察。 |
| Outputs | Research notes、source policy、evidence、UNKNOWN/TBD boundary。 |
| Entry Criteria | 問題範圍與研究目的已描述。 |
| Exit Criteria | 來源已記錄；事實、推論與未知內容已分離；沒有偷偷形成產品決策。 |

Research 只能提供證據，不直接建立 PRD requirement、Spec、Architecture 或技術決策。

### 4.2 Analysis

| Field | Definition |
| --- | --- |
| Purpose | 將 Research evidence 整理為 workflow、狀態、角色、依賴與限制。 |
| Inputs | 已審閱的 Research artifacts。 |
| Outputs | Analysis model、state interpretation、workflow analysis、boundary notes。 |
| Entry Criteria | Research source 與 UNKNOWN/TBD boundary 可追溯。 |
| Exit Criteria | 分析結果與來源一致；未把分析推論寫成已核准產品需求。 |

Analysis 可以整理語意，但不自行建立 Feature、技術選擇或實作設計。

### 4.3 Decision

| Field | Definition |
| --- | --- |
| Purpose | 將需要產品層判斷的分析結果轉成明確的採用、拒絕、部分採用或未知狀態。 |
| Inputs | Research、Analysis 與可審查的 evidence。 |
| Outputs | Decision records、rationale、risk、open questions、decision status。 |
| Entry Criteria | 分析範圍、證據與候選選項已可辨識。 |
| Exit Criteria | Decision status、evidence、risk 與 open questions 已記錄；未越過產品需求邊界。 |

Decision 是產品與行為的判斷層；長期技術取捨應在 Architecture Freeze 後進入 ADR。

### 4.4 PRD

| Field | Definition |
| --- | --- |
| Purpose | 定義產品願景、目標使用者、範圍、原則、功能需求與非功能需求。 |
| Inputs | 已審閱的 Research、Analysis、Decision 與產品需求 Change Request。 |
| Outputs | PRD、FR、NFR、product scope、success direction、open questions。 |
| Entry Criteria | 產品問題與目標已明確；Decision evidence 可追溯。 |
| Exit Criteria | PRD requirements、scope、priorities 與 open questions 可被 Specs 引用。 |

PRD 描述要解決什麼問題與應有什麼產品行為，不描述具體技術實作。

### 4.5 PRD Freeze

| Field | Definition |
| --- | --- |
| Purpose | 固定進入 Specification 的產品需求基線。 |
| Inputs | PRD-0002 至 PRD-0006、PRD Baseline Review、Traceability Matrix。 |
| Outputs | PRD Freeze Review、Freeze Decision、change control boundary。 |
| Entry Criteria | Functional、non-functional、traceability、consistency 與 open gaps 已 Review。 |
| Exit Criteria | Freeze Approved；新增產品需求必須走 PRD Change Request；Specs 不得自行補出新產品能力。 |

### 4.6 Specification

| Field | Definition |
| --- | --- |
| Purpose | 將 Frozen PRD 轉成 system requirements、Feature catalog、workflow、state、edge cases 與 acceptance criteria。 |
| Inputs | PRD v1.0 Frozen、Specification Guidelines、approved change requests。 |
| Outputs | SPEC、SR、Feature mapping、state/sequence semantics、acceptance criteria。 |
| Entry Criteria | PRD Freeze Approved；每份 Spec 能引用 FR/NFR/PRD。 |
| Exit Criteria | Feature ownership、system states、cross-feature boundaries、acceptance criteria 與 open gaps 已審閱。 |

Specification 描述系統應如何表現，但不指定 framework、language、API、class 或 project structure。

### 4.7 Specification Freeze

| Field | Definition |
| --- | --- |
| Purpose | 固定進入 Architecture 的行為與系統需求基線。 |
| Inputs | SPEC-0002 至 SPEC-0010、Specification Baseline Review、traceability evidence。 |
| Outputs | Specification Freeze Review、Freeze Decision、Spec change control boundary。 |
| Entry Criteria | Completeness、traceability、consistency、Feature ownership 與 acceptance coverage 已 Review。 |
| Exit Criteria | Specification v1.0 Freeze Approved；Architecture 不得自行新增 Feature 或改寫行為。 |

### 4.8 Architecture

| Field | Definition |
| --- | --- |
| Purpose | 將 Frozen Specifications 分解為 Layer、Module、Component、Interaction 與依賴邊界。 |
| Inputs | Frozen PRD、Frozen Specifications、Architecture Principles、approved Architecture Change Request。 |
| Outputs | ARCH-0001 至 ARCH-0005、Module/Component/Interaction mapping、risks、TBD、open findings。 |
| Entry Criteria | Specification Freeze Approved；Architecture source chain 已明確。 |
| Exit Criteria | Layer、Module、Component、Interaction 的 ownership、依賴方向、Shared State authority 與禁止邊界已建立。 |

Architecture 定義抽象責任邊界，不直接選擇 framework、language、API 或實作細節。

### 4.9 Architecture Freeze

| Field | Definition |
| --- | --- |
| Purpose | 固定進入 ADR 與 Technology Decision Roadmap 的抽象架構基線。 |
| Inputs | ARCH-0001 至 ARCH-0005、ARCH-BASELINE-REVIEW、ADR-BASELINE、TECHNOLOGY-DECISION-ROADMAP。 |
| Outputs | Architecture Baseline Review、Freeze Decision、Architecture change control boundary。 |
| Entry Criteria | Architecture completeness、traceability、consistency、dependency、responsibility coverage 與 principle compliance 已 Review。 |
| Exit Criteria | Architecture v1.0 Freeze Approved；Layer、Module、Component、Interaction 不得被實作直接改寫。 |

Architecture Freeze 不代表技術棧、Interface Contract、Project Structure 或 Runtime verification 已完成。

### 4.10 ADR

| Field | Definition |
| --- | --- |
| Purpose | 記錄一個重大且長期有效的 Architecture 或 Technology Decision。 |
| Inputs | Accepted Architecture requirement、unresolved Architecture question、Decision Roadmap Candidate。 |
| Outputs | ADR-NNNN、Options、Decision、Trade-offs、Consequences、Review Record、Supersession links。 |
| Entry Criteria | Decision Candidate 已達 Ready；ADR-BASELINE 已可引用；一份 ADR 只處理一個重大決策。 |
| Exit Criteria | ADR Review 通過並進入 Accepted，或以 Draft/Review 狀態保留待處理。 |

ADR 不得反向改寫 Frozen PRD、Frozen Specs 或 Architecture；發現衝突時必須回到 Change Flow。

### 4.11 Implementation

| Field | Definition |
| --- | --- |
| Purpose | 依 Accepted PRD、Specs、Architecture 與 ADR 實現產品。 |
| Inputs | Frozen PRD、Frozen Specs、Frozen Architecture baseline、Accepted ADR、Implementation Change Request。 |
| Outputs | Source code、project artifacts、configuration、implementation evidence。 |
| Entry Criteria | 上游治理文件已達相應 Freeze/Accepted 狀態；實作範圍與 verification plan 可追溯。 |
| Exit Criteria | Implementation artifact 可被 Review，且每個重要行為與決策都有來源與驗證入口。 |

Implementation 不得自行新增產品需求、Feature、Module、Component 或長期技術決策。

### 4.12 Verification

| Field | Definition |
| --- | --- |
| Purpose | 驗證實作是否符合 Frozen PRD、Frozen Specs、Architecture 與 Accepted ADR。 |
| Inputs | Implementation artifact、acceptance criteria、test/verification plan、runtime evidence。 |
| Outputs | Verification results、defects、evidence、release recommendation、change findings。 |
| Entry Criteria | Implementation artifact 已可被檢查；驗證範圍、成功條件與限制已記錄。 |
| Exit Criteria | 結果、限制、未驗證項目與後續 Change Request 已明確記錄。 |

Verification evidence 不能把未驗證的行為或技術假設寫成 Architecture fact。

### 4.13 Release

| Field | Definition |
| --- | --- |
| Purpose | 將已驗證、可支援的產品版本交付給目標使用者。 |
| Inputs | Verification results、release decision、packaging/update decisions、support information。 |
| Outputs | Release artifact、version record、CHANGELOG、support/rollback evidence。 |
| Entry Criteria | Release scope、verification result、known limitations 與必要的 operations decision 已審閱。 |
| Exit Criteria | Release record 已建立；版本、變更、限制與後續維護入口可追溯。 |

Release 不得繞過 Verification，也不能以發布壓力取代未完成的 PRD、Spec、Architecture 或 ADR Review。

## 5. Freeze Gates

### 5.1 PRD Freeze

代表：

- 產品目標、範圍、FR、NFR 與主要 UX 原則已建立基線。
- Specs 可以引用固定的產品需求。
- 新產品需求必須走 PRD Change Request。

不代表：

- Feature 已實作。
- Spec 已完成。
- Architecture、技術選擇或 ADR 已完成。
- 可以開始 Coding。

### 5.2 Specification Freeze

代表：

- Feature、system requirements、states、boundaries 與 acceptance criteria 已建立基線。
- Architecture 可以依據固定行為定義抽象責任。
- 新功能或行為變更必須走 Spec Change Request。

不代表：

- Architecture 已完成。
- 技術棧、API、Interface 或 Project Structure 已選擇。
- Implementation 或 Runtime verification 已完成。

### 5.3 Architecture Freeze

代表：

- Layer、Module、Component、Interaction 與 dependency boundary 已固定。
- ADR 與 Technology Decision Roadmap 可以依據抽象架構進行。
- 實作不得直接改寫 Architecture ownership 或 Shared State authority。

不代表：

- Candidate 已全部變成 Accepted。
- Framework、Language、Graphics、Clipboard 或 Storage technology 已選擇。
- Interface Contract、Project Structure、Implementation、Verification 或 Release 已完成。

## 6. Artifact Mapping

| Phase | Primary Artifacts |
| --- | --- |
| Research | docs/Research/ |
| Analysis | docs/Analysis/ |
| Decision | docs/Decision/ |
| PRD | PRD/ |
| PRD Freeze | PRD/PRD-BASELINE-REVIEW.md、PRD/PRD-TRACEABILITY-MATRIX.md、PRD/PRD-FREEZE-REVIEW.md |
| Specification | Specs/ |
| Specification Freeze | Specs/SPEC-BASELINE-REVIEW.md |
| Architecture | Architecture/ARCH-0001 至 ARCH-0005 |
| Architecture Freeze | Architecture/ARCH-BASELINE-REVIEW.md |
| ADR Governance | Architecture/ADR-BASELINE.md |
| Technology Decision Roadmap | Architecture/TECHNOLOGY-DECISION-ROADMAP.md |
| ADR | Architecture/adr/ |
| Implementation | TBD；目前尚無應用程式碼 |
| Verification | TBD；目前尚無 runtime evidence |
| Release | CHANGELOG.md、ROADMAP.md、release artifacts：TBD |

Artifact Mapping 是導航，不取代各目錄內文件的詳細規則。

## 7. Traceability

治理追溯鏈：

~~~text
Research
  ↓
Analysis
  ↓
Decision
  ↓
PRD
  ↓
Specification
  ↓
Architecture
  ↓
ADR
  ↓
Implementation
  ↓
Verification
  ↓
Release
~~~

規則：

- 每個下游 artifact 必須能指出其上游來源。
- Research 不得直接跳過 Analysis、Decision 與 PRD/Spec 來源鏈建立 Architecture。
- PRD Freeze 固定產品需求；Specification Freeze 固定行為與驗收；Architecture Freeze 固定抽象責任與依賴。
- ADR 必須引用 Architecture requirement、Architecture question 或 Architecture finding。
- Implementation 必須能引用 Frozen PRD、Frozen Specs、Architecture 與 Accepted ADR。
- Verification 必須引用 acceptance criteria、Architecture boundary 或 ADR consequence。
- Release 必須能回溯到 Verification result 與已知限制。
- UNKNOWN、TBD、Candidate 與未驗證 evidence 不得在下游被默認轉成已確認事實。

## 8. Change Flow

依變更影響範圍選擇回溯路徑：

~~~text
Research Change
  ↓
Analysis Review
  ↓
Decision Review
  ↓
PRD Change Request（若涉及產品需求）
  ↓
Spec Change Request（若涉及功能、狀態或驗收）
  ↓
Architecture Change Request（若涉及 Layer、Module、Component、Interaction 或依賴）
  ↓
ADR Change or New ADR（若涉及長期架構取捨）
  ↓
Implementation Change
  ↓
Verification
  ↓
Release Review
~~~

變更原則：

- 不直接修改 Frozen PRD、Frozen Specs 或 Frozen Architecture。
- 不由 Implementation 反向定義產品需求。
- 不由 Verification 結果偷偷改寫 Architecture。
- 影響多層時，從最上游受影響的層開始建立 Change Request。
- 小型文件格式修正不得改變語意、ownership、status 或 decision。
- 任何真正的技術決策都必須進入 ADR Lifecycle。

## 9. Governance Principles

本 Repository 目前已建立的治理原則：

- Research、Analysis、Decision、PRD、Specs、Architecture、ADR、Implementation、Verification 與 Release 各有不同責任。
- Frozen PRD 與 Frozen Specs 是產品需求與系統行為的來源基線。
- Architecture 不得重新定義產品需求。
- Layer、Module、Component 與 Interaction 必須保留單一責任與可追溯 ownership。
- COMP-001 是 Shared State 的唯一 Authority。
- Clipboard 與 Output 保持平行 downstream。
- Annotation 保持 Optional。
- Candidate、UNKNOWN、TBD 與 runtime verification gap 必須保持可見。
- ADR 只處理單一重大長期決策。
- Implementation 不得直接修改治理來源。
- Freeze 不等於技術選擇、Coding readiness 或 Release readiness。

相關來源：

- AGENTS.md 的 Repository governance 與 change boundaries。
- Architecture/ARCH-0001-architecture-principles.md。
- Architecture/ARCH-BASELINE-REVIEW.md。
- Architecture/ADR-BASELINE.md。

## 10. Open Questions

以下問題維持 UNKNOWN/TBD，不在本文件回答：

- Implementation 的正式 project structure 與 technology mapping。
- ADR Accepted 後與 Implementation artifact 的具體連結格式。
- Verification 的 runtime evidence 儲存與版本策略。
- Release artifact、rollback 與 support policy 的正式 owner。
- Technology Decision Roadmap 中 Candidate 進入 Ready 的實際審核責任人。
- Research、Analysis、Decision 文件的正式 review authority 是否需要獨立角色。
- 多個並行 Feature stream 如何在不破壞 Freeze boundary 的情況下合併。
- Release 後發現需求或架構問題時的回溯深度。

## 11. Completion Boundary

完成 PROJECT-LIFECYCLE 只代表：

- Repository 的文件生命週期已被單一總覽描述。
- 13 個主要階段都有 Purpose、Inputs、Outputs、Entry Criteria 與 Exit Criteria。
- PRD Freeze、Specification Freeze、Architecture Freeze 的意義與非意義已分開。
- Artifact Mapping、Traceability、Change Flow 與 Governance Principles 已建立。
- 目前治理文件的 Freeze 狀態與未決問題可由單一入口理解。

完成本文件不代表：

- 新增任何產品需求。
- 修改 PRD、Specs 或 Architecture。
- 建立 ADR。
- 完成 Technology Selection。
- 建立 Project Structure。
- 開始 Coding。
- 完成 Build、Test、Verification 或 Release。

## 12. Prohibited Decisions

本文件不得：

- 建立新需求。
- 修改 PRD。
- 修改 Spec。
- 修改 Architecture。
- 建立 ADR。
- 開始 Technology Selection。
- 開始 Coding。
- 選擇 Framework、Language、Graphics API、Clipboard API、Storage implementation 或 Testing framework。
- 定義 Interface、Class、Service、API 或 Project Structure。
- 建立 Overlay、Toolbar 或 Screenshot functionality。

