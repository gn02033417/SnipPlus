# ARCH-0005 Component Interactions

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | ARCH-0005 |
| Title | Component Interactions |
| Review Status | Draft |
| Architecture Stability | Draft |
| Version | 0.1 |
| Owner | TBD |
| Last reviewed | Not reviewed |
| Normative References | ARCH-0001、ARCH-0002、ARCH-0003、ARCH-0004、SPEC-0003、SPEC-0005、SPEC-0006、SPEC-0007、SPEC-0008、SPEC-0009、SPEC-0010 |
| Informative References | PRD-0002 至 PRD-0006、PRD-FREEZE-REVIEW、ARCH-0004 Information Boundary Catalog |

## 2. Purpose

本文件固定 Component 間的合法互動方向，描述主要工作流程、取消流程與失敗流程的 Component 協作，並保護 COMP-001 Workflow State Authority。

本文件也為後續 Contract、ADR 與 Project Structure 文件提供依據，但不定義實際 Interface、Method、Event 或資料格式。

## 3. Scope

本文件只涵蓋：

- Interaction initiator。
- Interaction recipient。
- Interaction purpose。
- Preconditions。
- Expected outcome。
- Shared-state transition request。
- Failure propagation。
- Prohibited interactions。
- Traceability。

## 4. Non-goals

本文件不定義或決定：

- Interface 名稱。
- Method 名稱或參數。
- Event、Command 或 Message Schema。
- 同步或非同步實作。
- Threading。
- Retry implementation。
- Timeout。
- Exception type。
- DI、Service Locator 或 Event Bus。
- Framework、Language 或 Windows API。
- UI、Overlay、Toolbar 或 Annotation Tool。

## 5. Interaction Vocabulary

以下是互動語意術語，不是 API 或訊息型別。

| Term | Meaning |
| --- | --- |
| Request | Component 對另一 Component 提出抽象能力要求。 |
| Response | 接收者回傳的抽象結果。 |
| Transition Request | 向 COMP-001 請求 Shared State 變更。 |
| Notification | 已發生事實的抽象告知，不代表 Event Bus。 |
| Rejection | 因前置條件或狀態不合法而拒絕互動。 |
| Failure Result | 無法完成要求的抽象失敗結果。 |
| Handoff | 將責任或結果移交給下一 Component。 |
| Boundary Feedback | 對使用者可理解回饋需求的抽象描述。 |

## 6. Interaction Rules

- 只有 COMP-001 可改變 Shared State。
- 其他 Component 只能提交 Transition Request、讀取狀態，或維持 No access。
- COMP-003 Feature Flow Coordinator 可協調 Domain Components，但不得直接執行平台操作。
- Domain Component 必須透過 Platform Adapter Component 使用平台能力。
- Platform Component 不得主動驅動 Product Workflow。
- Capture Result 只能由 COMP-006 建立完成語意。
- Annotation 必須維持 Optional，不得阻止未標註結果進入 downstream。
- Clipboard 與 Output 必須接收同一個抽象 Capture Result 邊界，並保持平行。
- COMP-012 只能分類 Failure，不得奪取原始 Failure Owner。
- COMP-013 只描述 Feedback Requirement，不得設計 UI。
- Component 不得跳過 Owning Module 的責任邊界。
- Component Interaction 不得形成循環依賴。
- 尚未由 Frozen Spec 支持的互動必須標示 TBD，不得自行建立。

## 7. Interaction Catalog

Interaction 使用獨立且不可重用的識別碼。Interaction Status 只能使用 Required、Candidate、Deferred、Deprecated。

| Interaction ID | Initiator | Recipient | Status | Purpose |
| --- | --- | --- | --- | --- |
| INT-001 | COMP-003 | COMP-002 | Required | 請求建立 Capture Session。 |
| INT-002 | COMP-002 | COMP-001 | Required | 請求進入合法 Session 起始狀態。 |
| INT-003 | COMP-003 | COMP-004 | Required | 提交 Capture Request。 |
| INT-004 | COMP-004 | COMP-014 | Required | 請求平台 Capture 能力。 |
| INT-005 | COMP-003 | COMP-005 | Required | 啟動 Selection。 |
| INT-006 | COMP-005 | COMP-018 | Candidate | 取得 Display Context。 |
| INT-007 | COMP-005 | COMP-006 | Required | 提交有效 Selection 以產生 Capture Result。 |
| INT-008 | COMP-006 | COMP-003 | Required | 回傳 Capture Result。 |
| INT-009 | COMP-003 | COMP-007 | Candidate | 啟動 Optional Annotation Session。 |
| INT-010 | COMP-007 | COMP-008 | Candidate | 提交 Annotation Mutation。 |
| INT-011 | COMP-007 | COMP-003 | Required | 回傳 Annotated 或未修改 Result。 |
| INT-012 | COMP-003 | COMP-009 | Required | 提交 Clipboard Handoff。 |
| INT-013 | COMP-009 | COMP-015 | Required | 請求平台 Clipboard 交付。 |
| INT-014 | COMP-003 | COMP-010 | Required | 提交 Output Delivery。 |
| INT-015 | COMP-010 | COMP-016 | Candidate | 請求平台 Output 交付。 |
| INT-016 | Feature Component（抽象占位） | COMP-012 | Required | 提交 Failure Classification。 |
| INT-017 | COMP-012 | COMP-011 | Required | 提交 Shared Failure 或 Termination Boundary。 |
| INT-018 | COMP-011 | COMP-001 | Required | 請求 Complete、Cancel 或 Error 狀態轉換。 |
| INT-019 | COMP-011 | COMP-013 | Candidate | 建立 Feedback Requirement。 |
| INT-020 | COMP-003 | COMP-001 | Required | 請求正常流程狀態轉換。 |
| INT-021 | COMP-017 | COMP-004 | Candidate | 提供 Capture Entry Input。 |
| INT-022 | COMP-018 | COMP-003 | Candidate | 通知 Platform Context Change。 |

INT-016 的 Feature Component 是對應 Failure Owner 的抽象占位，不是實際 Component ID。它允許 Capture、Annotation、Clipboard 或 Output 的 owning Component 提交原始 failure detail，而不新增 Component。

## 8. Interaction Definitions

每個 Interaction 使用以下固定欄位：Interaction ID、Name、Status、Initiator、Recipient、Purpose、Preconditions、Input meaning、Expected outcome、Shared-state effect、Failure owner、Failure propagation、Allowed follow-up、Prohibited follow-up、Feature traceability、Spec traceability、Open questions。

Shared-state effect 只能使用 None、Read、Transition request，不得使用 Direct transition。

### 8.1 INT-001 — Create Capture Session

| Field | Value |
| --- | --- |
| Interaction ID | INT-001 |
| Name | Create Capture Session |
| Status | Required |
| Initiator | COMP-003 Feature Flow Coordinator |
| Recipient | COMP-002 Session Lifecycle Boundary |
| Purpose | 請求建立一次 Capture Session。 |
| Preconditions | Workflow 可接受新的 Capture intent；目前沒有互斥的 active session。 |
| Input meaning | 使用者或 workflow 要開始一次 Capture session。 |
| Expected outcome | COMP-002 回傳 session lifecycle outcome。 |
| Shared-state effect | None。 |
| Failure owner | COMP-002。 |
| Failure propagation | 回傳 Rejection 或 Failure Result 給 COMP-003，再由 COMP-003 交給 COMP-011。 |
| Allowed follow-up | INT-002、INT-003。 |
| Prohibited follow-up | 直接呼叫 Platform Component；直接改變 COMP-001 的 Shared State。 |
| Feature traceability | FEAT-001、FEAT-005。 |
| Spec traceability | SPEC-0005、SPEC-0006、SPEC-0010。 |
| Open questions | Session 建立失敗的分類：TBD。 |

### 8.2 INT-002 — Start Session State

| Field | Value |
| --- | --- |
| Interaction ID | INT-002 |
| Name | Start Session State |
| Status | Required |
| Initiator | COMP-002 Session Lifecycle Boundary |
| Recipient | COMP-001 Workflow State Authority |
| Purpose | 請求進入合法的 Session 起始狀態。 |
| Preconditions | COMP-002 已接受 session intent。 |
| Input meaning | 一次 Capture session 已準備進入共享 workflow。 |
| Expected outcome | COMP-001 接受或拒絕狀態轉移。 |
| Shared-state effect | Transition request。 |
| Failure owner | COMP-001。 |
| Failure propagation | COMP-001 回傳 Rejection 給 COMP-002；COMP-002 再回報 COMP-003。 |
| Allowed follow-up | INT-003 或 INT-017。 |
| Prohibited follow-up | COMP-002 直接寫入 Shared State；Platform Component 主動改變 state。 |
| Feature traceability | FEAT-001、FEAT-005。 |
| Spec traceability | SPEC-0003、SPEC-0005、SPEC-0006。 |
| Open questions | 起始狀態的正式 runtime 名稱：TBD。 |

### 8.3 INT-003 — Submit Capture Request

| Field | Value |
| --- | --- |
| Interaction ID | INT-003 |
| Name | Submit Capture Request |
| Status | Required |
| Initiator | COMP-003 Feature Flow Coordinator |
| Recipient | COMP-004 Capture Request Boundary |
| Purpose | 提交抽象 Capture Request。 |
| Preconditions | Session 已進入可接受 Capture Request 的狀態。 |
| Input meaning | 使用者選擇開始 Capture 的 intent。 |
| Expected outcome | COMP-004 接受 request 並準備 Selection 或回傳 Rejection。 |
| Shared-state effect | None。 |
| Failure owner | COMP-004。 |
| Failure propagation | COMP-004 回傳 failure detail 給 COMP-003，再交給 COMP-012。 |
| Allowed follow-up | INT-004、INT-005、INT-016。 |
| Prohibited follow-up | 直接依賴 COMP-014 的具體平台行為；直接改變 COMP-001。 |
| Feature traceability | FEAT-001。 |
| Spec traceability | SPEC-0005、SPEC-0010。 |
| Open questions | Capture entry 的來源與條件：UNKNOWN/TBD。 |

### 8.4 INT-004 — Request Platform Capture

| Field | Value |
| --- | --- |
| Interaction ID | INT-004 |
| Name | Request Platform Capture |
| Status | Required |
| Initiator | COMP-004 Capture Request Boundary |
| Recipient | COMP-014 Platform Capture Adapter Boundary |
| Purpose | 請求平台執行抽象 Capture 行為。 |
| Preconditions | 抽象 Capture Request 已被 COMP-004 接受。 |
| Input meaning | 平台擷取能力的抽象要求，不包含技術型別。 |
| Expected outcome | COMP-014 回傳 capture outcome 或 platform failure detail。 |
| Shared-state effect | None。 |
| Failure owner | COMP-014。 |
| Failure propagation | COMP-014 回傳 platform failure detail 給 COMP-004，再交給 COMP-012。 |
| Allowed follow-up | INT-005、INT-016。 |
| Prohibited follow-up | COMP-014 主動驅動 Product Workflow；COMP-014 直接改變 COMP-001。 |
| Feature traceability | FEAT-001。 |
| Spec traceability | SPEC-0005、SPEC-0010。 |
| Open questions | Platform Capture 的完整 boundary：TBD。 |

### 8.5 INT-005 — Start Selection

| Field | Value |
| --- | --- |
| Interaction ID | INT-005 |
| Name | Start Selection |
| Status | Required |
| Initiator | COMP-003 Feature Flow Coordinator |
| Recipient | COMP-005 Selection Boundary |
| Purpose | 啟動 Selection 的抽象流程。 |
| Preconditions | Session 已進入可選取的 workflow state；Capture Request 已有效。 |
| Input meaning | Selection scope 與開始 Selection 的抽象 intent。 |
| Expected outcome | COMP-005 維持 Selection boundary 並回傳 outcome。 |
| Shared-state effect | None。 |
| Failure owner | COMP-005。 |
| Failure propagation | COMP-005 回傳 failure detail 給 COMP-003，再交給 COMP-012。 |
| Allowed follow-up | INT-006、INT-007、INT-016。 |
| Prohibited follow-up | COMP-005 直接改變 Shared State；直接依賴具體 UI。 |
| Feature traceability | FEAT-001。 |
| Spec traceability | SPEC-0005、SPEC-0010。 |
| Open questions | Selection scope 與平台 context 的完整關聯：UNKNOWN/TBD。 |

### 8.6 INT-006 — Read Display Context

| Field | Value |
| --- | --- |
| Interaction ID | INT-006 |
| Name | Read Display Context |
| Status | Candidate |
| Initiator | COMP-005 Selection Boundary |
| Recipient | COMP-018 Platform Display Context Boundary |
| Purpose | 取得 Selection 所需的抽象 Display Context。 |
| Preconditions | Selection 已開始，且平台脈絡可能影響 Selection。 |
| Input meaning | 對焦點、螢幕、DPI 或 HDR 等脈絡的抽象讀取要求。 |
| Expected outcome | COMP-018 回傳 abstract display context 或 context failure。 |
| Shared-state effect | Read。 |
| Failure owner | COMP-018。 |
| Failure propagation | COMP-018 回傳 context failure 給 COMP-005，再由 COMP-005 交給 COMP-012。 |
| Allowed follow-up | INT-007、INT-016。 |
| Prohibited follow-up | COMP-018 主動改變 workflow；COMP-005 直接依賴具體 display type。 |
| Feature traceability | FEAT-001。 |
| Spec traceability | SPEC-0005、SPEC-0010。 |
| Open questions | Display Context 是否為 Selection 的必要前置：TBD。 |

### 8.7 INT-007 — Submit Selection Result

| Field | Value |
| --- | --- |
| Interaction ID | INT-007 |
| Name | Submit Selection Result |
| Status | Required |
| Initiator | COMP-005 Selection Boundary |
| Recipient | COMP-006 Capture Result Boundary |
| Purpose | 提交有效 Selection 以產生 Capture Result。 |
| Preconditions | Selection 已完成且未被取消；Selection outcome 有效。 |
| Input meaning | 一次 Selection 已達成完成邊界。 |
| Expected outcome | COMP-006 建立 Capture Result available 語意。 |
| Shared-state effect | None。 |
| Failure owner | COMP-006。 |
| Failure propagation | COMP-006 回傳 result failure detail 給 COMP-005，再交給 COMP-012。 |
| Allowed follow-up | INT-008、INT-009、INT-012、INT-014、INT-016。 |
| Prohibited follow-up | COMP-005 直接建立 Capture Result；直接啟動 Clipboard 或 Output。 |
| Feature traceability | FEAT-001、FEAT-003、FEAT-004。 |
| Spec traceability | SPEC-0005、SPEC-0007、SPEC-0008、SPEC-0010。 |
| Open questions | Capture Result 的完整抽象 boundary：TBD。 |

### 8.8 INT-008 — Return Capture Result

| Field | Value |
| --- | --- |
| Interaction ID | INT-008 |
| Name | Return Capture Result |
| Status | Required |
| Initiator | COMP-006 Capture Result Boundary |
| Recipient | COMP-003 Feature Flow Coordinator |
| Purpose | 回傳 Capture Result available status。 |
| Preconditions | COMP-006 已建立 Capture Result 語意。 |
| Input meaning | 一次 Capture 已完成，結果可交給 optional Annotation 或 downstream paths。 |
| Expected outcome | COMP-003 決定進入 Annotation optional path 或直接進入平行 downstream。 |
| Shared-state effect | Read。 |
| Failure owner | COMP-006。 |
| Failure propagation | COMP-003 將 result failure detail 交給 COMP-012。 |
| Allowed follow-up | INT-009、INT-012、INT-014、INT-016、INT-020。 |
| Prohibited follow-up | COMP-006 直接改變 Shared State；COMP-003 將 Clipboard 與 Output 串成必要順序。 |
| Feature traceability | FEAT-001、FEAT-003、FEAT-004。 |
| Spec traceability | SPEC-0005、SPEC-0007、SPEC-0008、SPEC-0010。 |
| Open questions | Result 是否可同時供多個 downstream consumer 使用：TBD。 |

### 8.9 INT-009 — Start Optional Annotation

| Field | Value |
| --- | --- |
| Interaction ID | INT-009 |
| Name | Start Optional Annotation |
| Status | Candidate |
| Initiator | COMP-003 Feature Flow Coordinator |
| Recipient | COMP-007 Annotation Session Boundary |
| Purpose | 啟動 Optional Annotation Session。 |
| Preconditions | Capture Result available；使用者選擇進入 annotation path。 |
| Input meaning | 使用者希望對結果進行 annotation 的抽象 intent。 |
| Expected outcome | COMP-007 接受 session、允許 skip，或回傳 Rejection。 |
| Shared-state effect | None。 |
| Failure owner | COMP-007。 |
| Failure propagation | COMP-007 回傳 failure detail 給 COMP-003，再交給 COMP-012。 |
| Allowed follow-up | INT-010、INT-011、INT-016。 |
| Prohibited follow-up | 將 Annotation 變成必要流程；直接依賴具體 Annotation Tool。 |
| Feature traceability | FEAT-002。 |
| Spec traceability | SPEC-0009、SPEC-0010。 |
| Open questions | Annotation session 的 entry 與 skip 互動：TBD。 |

### 8.10 INT-010 — Submit Annotation Mutation

| Field | Value |
| --- | --- |
| Interaction ID | INT-010 |
| Name | Submit Annotation Mutation |
| Status | Candidate |
| Initiator | COMP-007 Annotation Session Boundary |
| Recipient | COMP-008 Annotation Mutation Boundary |
| Purpose | 提交建立、修改或移除 Annotation 的抽象 change intent。 |
| Preconditions | Annotation session 已接受 mutation path；具體 Tool Scope 已由後續 Spec 支持。 |
| Input meaning | Annotation 的抽象變更要求。 |
| Expected outcome | COMP-008 回傳 annotation change outcome。 |
| Shared-state effect | None。 |
| Failure owner | COMP-008。 |
| Failure propagation | COMP-008 回傳 failure detail 給 COMP-007，再交給 COMP-012。 |
| Allowed follow-up | INT-011、INT-016。 |
| Prohibited follow-up | 定義具體工具、資料格式、Undo/Redo 或 UI；直接啟動 Clipboard 或 Output。 |
| Feature traceability | FEAT-002。 |
| Spec traceability | SPEC-0009、SPEC-0010。 |
| Open questions | Annotation mutation 是否等 Tool Specs 後固定：TBD。 |

### 8.11 INT-011 — Return Annotated or Unmodified Result

| Field | Value |
| --- | --- |
| Interaction ID | INT-011 |
| Name | Return Annotated or Unmodified Result |
| Status | Required |
| Initiator | COMP-007 Annotation Session Boundary |
| Recipient | COMP-003 Feature Flow Coordinator |
| Purpose | 回傳 Annotated Result 或未修改 Result。 |
| Preconditions | Annotation session 已完成、被 skip，或 mutation 已回報 outcome。 |
| Input meaning | Result 可繼續進入 Clipboard 與 Output 的共同 downstream boundary。 |
| Expected outcome | COMP-003 將結果交給平行 downstream paths。 |
| Shared-state effect | Read。 |
| Failure owner | COMP-007。 |
| Failure propagation | COMP-003 將 annotation failure 交給 COMP-012；未修改結果不得被視為 failure。 |
| Allowed follow-up | INT-012、INT-014、INT-016、INT-020。 |
| Prohibited follow-up | 阻止 Annotation skip；將 Clipboard 與 Output 變成 Annotation 的必要後續。 |
| Feature traceability | FEAT-002、FEAT-003、FEAT-004。 |
| Spec traceability | SPEC-0009、SPEC-0007、SPEC-0008、SPEC-0010。 |
| Open questions | Annotated Result 與 Capture Result 的共同抽象語意：TBD。 |

### 8.12 INT-012 — Submit Clipboard Handoff

| Field | Value |
| --- | --- |
| Interaction ID | INT-012 |
| Name | Submit Clipboard Handoff |
| Status | Required |
| Initiator | COMP-003 Feature Flow Coordinator |
| Recipient | COMP-009 Clipboard Handoff Boundary |
| Purpose | 將同一個抽象 Result 提交給 Clipboard downstream。 |
| Preconditions | Capture Result 或 Annotated Result available。 |
| Input meaning | Clipboard Consumer handoff 的抽象要求。 |
| Expected outcome | COMP-009 建立 handoff pending 或回傳 Rejection。 |
| Shared-state effect | None。 |
| Failure owner | COMP-009。 |
| Failure propagation | COMP-009 將 handoff failure 交給 COMP-012。 |
| Allowed follow-up | INT-013、INT-016、INT-018。 |
| Prohibited follow-up | 依賴 COMP-010 先完成；直接依賴 Platform Clipboard API。 |
| Feature traceability | FEAT-003。 |
| Spec traceability | SPEC-0007、SPEC-0010。 |
| Open questions | Clipboard handoff 與 workflow Complete 的關聯：TBD。 |

### 8.13 INT-013 — Request Platform Clipboard Delivery

| Field | Value |
| --- | --- |
| Interaction ID | INT-013 |
| Name | Request Platform Clipboard Delivery |
| Status | Required |
| Initiator | COMP-009 Clipboard Handoff Boundary |
| Recipient | COMP-015 Platform Clipboard Adapter Boundary |
| Purpose | 請求平台完成 Clipboard 交付。 |
| Preconditions | Clipboard handoff intent 已被 COMP-009 接受。 |
| Input meaning | 平台 Clipboard 交付的抽象要求。 |
| Expected outcome | COMP-015 回傳 consumer acceptance 或 platform error。 |
| Shared-state effect | None。 |
| Failure owner | COMP-015。 |
| Failure propagation | COMP-015 將 platform failure detail 回傳 COMP-009，再交給 COMP-012。 |
| Allowed follow-up | INT-016、INT-018。 |
| Prohibited follow-up | COMP-015 主動驅動 workflow；依賴 COMP-016。 |
| Feature traceability | FEAT-003。 |
| Spec traceability | SPEC-0007、SPEC-0010。 |
| Open questions | Clipboard consumer、permission 與既有 context：UNKNOWN/TBD。 |

### 8.14 INT-014 — Submit Output Delivery

| Field | Value |
| --- | --- |
| Interaction ID | INT-014 |
| Name | Submit Output Delivery |
| Status | Required |
| Initiator | COMP-003 Feature Flow Coordinator |
| Recipient | COMP-010 Output Delivery Boundary |
| Purpose | 將同一個抽象 Result 提交給 Output downstream。 |
| Preconditions | Capture Result 或 Annotated Result available。 |
| Input meaning | Output delivery 的抽象要求。 |
| Expected outcome | COMP-010 建立 delivery pending 或回傳 Rejection。 |
| Shared-state effect | None。 |
| Failure owner | COMP-010。 |
| Failure propagation | COMP-010 將 delivery failure 交給 COMP-012。 |
| Allowed follow-up | INT-015、INT-016、INT-018。 |
| Prohibited follow-up | 依賴 COMP-009 先完成；直接依賴 File IO 或平台 Output API。 |
| Feature traceability | FEAT-004。 |
| Spec traceability | SPEC-0008、SPEC-0010。 |
| Open questions | Output delivery 與 workflow Complete 的關聯：TBD。 |

### 8.15 INT-015 — Request Platform Output Delivery

| Field | Value |
| --- | --- |
| Interaction ID | INT-015 |
| Name | Request Platform Output Delivery |
| Status | Candidate |
| Initiator | COMP-010 Output Delivery Boundary |
| Recipient | COMP-016 Platform Output Adapter Boundary |
| Purpose | 請求平台完成 Output 交付。 |
| Preconditions | Output delivery intent 已被 COMP-010 接受。 |
| Input meaning | 平台 Output 交付的抽象要求。 |
| Expected outcome | COMP-016 回傳 delivery outcome 或 platform error。 |
| Shared-state effect | None。 |
| Failure owner | COMP-016。 |
| Failure propagation | COMP-016 將 platform failure detail 回傳 COMP-010，再交給 COMP-012。 |
| Allowed follow-up | INT-016、INT-018。 |
| Prohibited follow-up | COMP-016 主動驅動 workflow；依賴 COMP-009。 |
| Feature traceability | FEAT-004。 |
| Spec traceability | SPEC-0008、SPEC-0010。 |
| Open questions | Output Adapter 是否為 MVP 必要：TBD。 |

### 8.16 INT-016 — Submit Failure Classification

| Field | Value |
| --- | --- |
| Interaction ID | INT-016 |
| Name | Submit Failure Classification |
| Status | Required |
| Initiator | Feature Component（抽象 Failure Owner 占位） |
| Recipient | COMP-012 Failure Classification Boundary |
| Purpose | 將 Feature 或 Platform failure detail 提交給共同 failure classification。 |
| Preconditions | owning Feature Component 已產生 failure detail。 |
| Input meaning | Capture、Annotation、Clipboard、Output 或 Platform failure 的抽象描述。 |
| Expected outcome | COMP-012 回傳 shared failure classification。 |
| Shared-state effect | None。 |
| Failure owner | 原始 Feature Component；COMP-012 不奪取 ownership。 |
| Failure propagation | COMP-012 將分類結果交給 COMP-011。 |
| Allowed follow-up | INT-017、INT-019。 |
| Prohibited follow-up | Feature Component 直接改 COMP-001；COMP-012 直接執行 recovery。 |
| Feature traceability | FEAT-001、FEAT-002、FEAT-003、FEAT-004、FEAT-005。 |
| Spec traceability | SPEC-0006、SPEC-0010。 |
| Open questions | Feature Component 的具體 failure taxonomy：TBD。 |

### 8.17 INT-017 — Submit Shared Failure Boundary

| Field | Value |
| --- | --- |
| Interaction ID | INT-017 |
| Name | Submit Shared Failure Boundary |
| Status | Required |
| Initiator | COMP-012 Failure Classification Boundary |
| Recipient | COMP-011 Completion and Cancellation Boundary |
| Purpose | 將 Feature failure 分類為 shared termination 或 recoverable boundary。 |
| Preconditions | COMP-012 已取得 failure detail；分類結果仍可為 TBD。 |
| Input meaning | Shared failure classification 與原始 ownership reference。 |
| Expected outcome | COMP-011 決定流程是否進入 failure termination boundary。 |
| Shared-state effect | None。 |
| Failure owner | 原始 Feature Component；分類責任由 COMP-012 擁有。 |
| Failure propagation | COMP-011 向 COMP-001 提交合法 transition request。 |
| Allowed follow-up | INT-018、INT-019。 |
| Prohibited follow-up | COMP-012 直接改變 Shared State；吞併原始 Feature failure。 |
| Feature traceability | FEAT-005。 |
| Spec traceability | SPEC-0006、SPEC-0010。 |
| Open questions | Recoverable 或 Terminal 的正式分類：TBD。 |

### 8.18 INT-018 — Request Complete, Cancel, or Error State

| Field | Value |
| --- | --- |
| Interaction ID | INT-018 |
| Name | Request Complete, Cancel, or Error State |
| Status | Required |
| Initiator | COMP-011 Completion and Cancellation Boundary |
| Recipient | COMP-001 Workflow State Authority |
| Purpose | 請求共享 workflow 進入 Complete、Cancel 或 Error state。 |
| Preconditions | Completion、cancellation 或 failure boundary 已被 COMP-011 分類。 |
| Input meaning | 跨 Feature 的終止或完成語意。 |
| Expected outcome | COMP-001 接受或拒絕 transition request。 |
| Shared-state effect | Transition request。 |
| Failure owner | COMP-001；原始 Feature failure ownership 仍保留。 |
| Failure propagation | Rejection 回到 COMP-011，再交給 COMP-012 或 COMP-013。 |
| Allowed follow-up | INT-019、INT-020。 |
| Prohibited follow-up | COMP-011 直接改變 Shared State；Platform Component 直接觸發此互動。 |
| Feature traceability | FEAT-005。 |
| Spec traceability | SPEC-0003、SPEC-0006、SPEC-0010。 |
| Open questions | Complete 的必要 downstream 條件：TBD。 |

### 8.19 INT-019 — Create Feedback Requirement

| Field | Value |
| --- | --- |
| Interaction ID | INT-019 |
| Name | Create Feedback Requirement |
| Status | Candidate |
| Initiator | COMP-011 Completion and Cancellation Boundary |
| Recipient | COMP-013 Feedback Boundary |
| Purpose | 建立使用者可理解的 Feedback Requirement。 |
| Preconditions | Completion、cancel 或 failure boundary 已被分類。 |
| Input meaning | 需要回饋的結果、urgency 與 accessibility context。 |
| Expected outcome | COMP-013 回傳抽象 Feedback Requirement。 |
| Shared-state effect | Read。 |
| Failure owner | COMP-013。 |
| Failure propagation | Feedback boundary failure 只回報為 feedback outcome，不改寫 workflow result。 |
| Allowed follow-up | 未定義的 feedback consumer；保持 TBD。 |
| Prohibited follow-up | COMP-013 直接呈現 UI；直接改變 COMP-001。 |
| Feature traceability | FEAT-005。 |
| Spec traceability | SPEC-0006、SPEC-0010。 |
| Open questions | Feedback Requirement 的最終 consumer：TBD。 |

### 8.20 INT-020 — Request Normal Workflow Transition

| Field | Value |
| --- | --- |
| Interaction ID | INT-020 |
| Name | Request Normal Workflow Transition |
| Status | Required |
| Initiator | COMP-003 Feature Flow Coordinator |
| Recipient | COMP-001 Workflow State Authority |
| Purpose | 請求 Capture、Annotation skip 或 downstream handoff 的正常流程狀態轉換。 |
| Preconditions | 相關 Feature outcome 已符合 Frozen Spec 的流程前置條件。 |
| Input meaning | 正常流程由一個共享 state 進入下一合法 state 的要求。 |
| Expected outcome | COMP-001 接受或拒絕 transition request。 |
| Shared-state effect | Transition request。 |
| Failure owner | COMP-001；原始 Feature Component 仍負責其內部 failure。 |
| Failure propagation | Rejection 回到 COMP-003，再交給 COMP-012 或 COMP-013。 |
| Allowed follow-up | INT-009、INT-012、INT-014、INT-018。 |
| Prohibited follow-up | COMP-003 直接改變 Shared State；以 UI 操作取代 shared workflow semantics。 |
| Feature traceability | FEAT-001、FEAT-002、FEAT-003、FEAT-004、FEAT-005。 |
| Spec traceability | SPEC-0003、SPEC-0005、SPEC-0006、SPEC-0007、SPEC-0008、SPEC-0009、SPEC-0010。 |
| Open questions | 正常流程的完整 state transition set：TBD。 |

### 8.21 INT-021 — Provide Capture Entry Input

| Field | Value |
| --- | --- |
| Interaction ID | INT-021 |
| Name | Provide Capture Entry Input |
| Status | Candidate |
| Initiator | COMP-017 Platform Input Boundary |
| Recipient | COMP-004 Capture Request Boundary |
| Purpose | 提供 Capture entry input 的抽象結果。 |
| Preconditions | Platform Input boundary 已取得合法 input context。 |
| Input meaning | 使用者啟動 Capture 的抽象 input fact，不包含具體按鍵。 |
| Expected outcome | COMP-004 接受或拒絕 Capture entry input。 |
| Shared-state effect | None。 |
| Failure owner | COMP-017。 |
| Failure propagation | input context failure 回報 COMP-004，再交給 COMP-012。 |
| Allowed follow-up | INT-003、INT-016。 |
| Prohibited follow-up | 具體按鍵或 UI 操作在本文件固定；COMP-017 主動改變 workflow。 |
| Feature traceability | FEAT-001。 |
| Spec traceability | SPEC-0005、SPEC-0010。 |
| Open questions | Capture entry 的具體輸入來源：UNKNOWN/TBD。 |

### 8.22 INT-022 — Notify Platform Context Change

| Field | Value |
| --- | --- |
| Interaction ID | INT-022 |
| Name | Notify Platform Context Change |
| Status | Candidate |
| Initiator | COMP-018 Platform Display Context Boundary |
| Recipient | COMP-003 Feature Flow Coordinator |
| Purpose | 通知 Platform Context Change 的抽象事實。 |
| Preconditions | Display、focus、DPI 或 HDR context 發生可觀察變化。 |
| Input meaning | 平台 context change 的抽象 Notification，不代表主動 workflow transition。 |
| Expected outcome | COMP-003 評估是否需要回報 active Feature；具體處理保持 TBD。 |
| Shared-state effect | Read。 |
| Failure owner | COMP-018。 |
| Failure propagation | Context failure 回報 COMP-003，再交給 COMP-012；不得自行終止 Session。 |
| Allowed follow-up | INT-016；是否需要 INT-018 保持 TBD。 |
| Prohibited follow-up | COMP-018 直接改變 COMP-001；Platform Context Change 直接驅動 Product Workflow。 |
| Feature traceability | FEAT-001、FEAT-003、FEAT-004、FEAT-005。 |
| Spec traceability | SPEC-0006、SPEC-0007、SPEC-0008、SPEC-0010。 |
| Open questions | Context Change 是否中斷 Session：TBD。 |

## 9. Primary Workflow Interaction Sequence

以下 Sequence Diagram 只描述抽象互動方向。所有 Shared State 變更都由 COMP-001 完成；Annotation 使用 opt；Clipboard 與 Output 是平行 downstream interaction。

~~~mermaid
sequenceDiagram
    actor User
    participant C3 as COMP-003 Feature Flow Coordinator
    participant C2 as COMP-002 Session Lifecycle Boundary
    participant C1 as COMP-001 Workflow State Authority
    participant C4 as COMP-004 Capture Request Boundary
    participant C5 as COMP-005 Selection Boundary
    participant C6 as COMP-006 Capture Result Boundary
    participant C7 as COMP-007 Annotation Session Boundary
    participant C9 as COMP-009 Clipboard Handoff Boundary
    participant C10 as COMP-010 Output Delivery Boundary
    participant C11 as COMP-011 Completion and Cancellation Boundary

    User->>C3: Capture intent
    C3->>C2: INT-001 Create Session
    C2->>C1: INT-002 Transition Request
    C1-->>C2: Session state outcome
    C3->>C4: INT-003 Capture Request
    C3->>C5: INT-005 Start Selection
    C5-->>C3: Selection outcome
    C5->>C6: INT-007 Completed Selection
    C6-->>C3: INT-008 Capture Result

    opt Optional Annotation
        C3->>C7: INT-009 Annotation intent
        C7-->>C3: INT-011 Annotated or unmodified Result
    end

    par Clipboard downstream
        C3->>C9: INT-012 Clipboard Handoff
        C9-->>C3: Clipboard outcome
    and Output downstream
        C3->>C10: INT-014 Output Delivery
        C10-->>C3: Output outcome
    end

    C3->>C11: Completion boundary
    C11->>C1: INT-018 Complete transition request
    C1-->>C11: Completion state outcome
~~~

## 10. Cancellation Interaction Sequence

具體按鍵或 UI 操作維持 UNKNOWN/TBD；本圖只描述取消 intent 與共享終止邊界。

~~~mermaid
sequenceDiagram
    actor User
    participant Active as Active Feature Component
    participant C11 as COMP-011 Completion and Cancellation Boundary
    participant C1 as COMP-001 Workflow State Authority
    participant C13 as COMP-013 Feedback Boundary
    participant C2 as COMP-002 Session Lifecycle Boundary

    User->>Active: Cancellation intent
    Active->>C11: Cancellation boundary
    C11->>C1: Transition Request: Cancel
    C1-->>C11: Cancel state outcome
    opt Optional Feedback Boundary
        C11->>C13: Cancellation Feedback Requirement
        C13-->>C11: Feedback outcome
    end
    C11->>C2: Session termination outcome
~~~

## 11. Failure Interaction Sequence

Recoverable、Terminal、Retry 與 Feedback channel 均保持 TBD。

~~~mermaid
sequenceDiagram
    participant Owner as Failure Owner
    participant C12 as COMP-012 Failure Classification Boundary
    participant C11 as COMP-011 Completion and Cancellation Boundary
    participant C1 as COMP-001 Workflow State Authority
    participant C13 as COMP-013 Feedback Boundary

    Owner->>C12: Failure Result
    C12-->>Owner: Classification outcome
    Note over C12: Recoverable or Terminal: TBD
    C12->>C11: Shared Failure or Termination Boundary
    C11->>C1: Transition Request: Error or termination
    C1-->>C11: State outcome
    opt Feedback Boundary Candidate
        C11->>C13: Feedback Requirement
        C13-->>C11: Feedback channel: TBD
    end
    Note over C12,C11: Retry policy: TBD
~~~

## 12. Interaction Responsibility Matrix

| Scenario | Initiator | Primary recipient | State authority | Failure owner | Feedback boundary |
| --- | --- | --- | --- | --- | --- |
| Start session | COMP-003 | COMP-002 | COMP-001 | COMP-002 | COMP-013：TBD |
| Start capture | COMP-003 | COMP-004 | COMP-001 | COMP-004 | COMP-013：TBD |
| Start selection | COMP-003 | COMP-005 | COMP-001 | COMP-005 | COMP-013：TBD |
| Produce result | COMP-005 | COMP-006 | COMP-001 | COMP-006 | COMP-013：TBD |
| Enter optional annotation | COMP-003 | COMP-007 | COMP-001 | COMP-007 | COMP-013：TBD |
| Skip annotation | COMP-007 | COMP-003 | COMP-001 | COMP-007 | COMP-013：TBD |
| Clipboard handoff | COMP-003 | COMP-009 | COMP-001 | COMP-009 | COMP-013：TBD |
| Output delivery | COMP-003 | COMP-010 | COMP-001 | COMP-010 | COMP-013：TBD |
| Complete workflow | COMP-003 | COMP-011 | COMP-001 | COMP-011 | COMP-013：TBD |
| Cancel workflow | User | COMP-011 | COMP-001 | COMP-011 | COMP-013：Candidate |
| Capture failure | COMP-004 | COMP-012 | COMP-001 | COMP-004 | COMP-013：TBD |
| Annotation failure | COMP-007 | COMP-012 | COMP-001 | COMP-007 | COMP-013：TBD |
| Clipboard failure | COMP-009 | COMP-012 | COMP-001 | COMP-009 | COMP-013：TBD |
| Output failure | COMP-010 | COMP-012 | COMP-001 | COMP-010 | COMP-013：TBD |
| Platform context interruption | COMP-018 | COMP-003 | COMP-001 | COMP-018 | COMP-013：TBD |

每個 Scenario 只有一個 Primary recipient 與一個 Failure owner；若 Feedback boundary 尚未成立，保留 Candidate 或 TBD。

## 13. Prohibited Interaction Matrix

| Initiator | Prohibited recipient | Reason |
| --- | --- | --- |
| COMP-003 | Platform Components | Feature Flow Coordinator 不得直接操作平台。 |
| Platform Components | COMP-001 | Platform 不得主動改變 Shared State。 |
| COMP-009 | COMP-010 | Clipboard 與 Output 不得形成必要依賴。 |
| COMP-010 | COMP-009 | Output 與 Clipboard 不得形成必要依賴。 |
| COMP-007、COMP-008 | Downstream blockage | Annotation 不得阻止 Annotation skip。 |
| COMP-012 | Feature recovery implementation | Failure Classification 不得直接實作 Feature recovery。 |
| COMP-013 | UI implementation | Feedback Boundary 不得直接呈現 UI。 |
| 任一非 COMP-001 Component | Shared State authority | 非 COMP-001 Component 不得直接改 Shared State。 |
| Domain Components | Concrete platform types | Domain 不得直接依賴具體平台型別。 |
| Platform Components | Product Workflow Layer | Platform 不得依賴 Product Workflow。 |

## 14. Information Exchange Mapping

本節引用 ARCH-0004 的 Information Boundary Catalog；所有 Format status 維持 TBD，不新增 DTO、Schema、Record 或 Message。

| Information meaning | Producer | Consumer | Interaction IDs | Format status |
| --- | --- | --- | --- | --- |
| Capture Request | COMP-003、COMP-004 | COMP-004、COMP-014 | INT-003、INT-004 | TBD |
| Selection State | COMP-005 | COMP-006、COMP-003 | INT-005、INT-006、INT-007 | TBD |
| Capture Result | COMP-006 | COMP-003、COMP-007、COMP-009、COMP-010 | INT-008、INT-009、INT-011、INT-012、INT-014 | TBD |
| Annotation Change | COMP-008 | COMP-007、COMP-003 | INT-010、INT-011 | TBD |
| Clipboard Handoff Request | COMP-009 | COMP-015 | INT-012、INT-013 | TBD |
| Output Delivery Request | COMP-010 | COMP-016 | INT-014、INT-015 | TBD |
| Workflow Transition Request | COMP-002、COMP-003、COMP-011 | COMP-001 | INT-002、INT-018、INT-020 | TBD |
| Failure Classification | Feature Component、COMP-012 | COMP-011、COMP-001、COMP-013 | INT-016、INT-017、INT-018、INT-019 | TBD |
| Feedback Requirement | COMP-011、COMP-012 | COMP-013 | INT-019 | TBD |
| Platform Context Change | COMP-017、COMP-018 | COMP-004、COMP-005、COMP-003 | INT-006、INT-021、INT-022 | TBD |

## 15. Traceability

~~~text
Frozen PRD
  ↓
Frozen Specs
  ↓
Feature
  ↓
Layer
  ↓
Module
  ↓
Component
  ↓
Interaction
~~~

每個 Interaction 必須至少追溯到：

- Initiator Component 或明確的抽象 Failure Owner。
- Recipient Component。
- 一個 Feature 或 Shared Boundary。
- 一份 Frozen Feature Spec。
- ARCH-0003 Module Catalog。
- ARCH-0004 Component Boundaries。

本文件不得直接以 Research 內容建立 Interaction；任何新增互動必須沿既定來源鏈進入 Review。

## 16. Architectural Risks

以下只記錄風險，不在本文件解決：

- COMP-003 成為 God Coordinator。
- 過多 Interaction 導致流程碎片化。
- Interaction Catalog 被誤解成 API。
- Shared-state transition request 過度集中。
- Annotation optional path 被實作成同步阻塞。
- Clipboard／Output 平行流程造成完成語意不清。
- Failure Classification 成為中央 Exception Handler。
- Platform context notification 主動驅動流程。
- Candidate Interaction 過早固定。
- Information meaning 被提前具體化為資料型別。

## 17. Open Questions

以下問題保留為 TBD，不在本文件回答：

- Clipboard 與 Output 是否同時執行、依序執行或由使用者選擇。
- Workflow 何時可宣告 Complete。
- Annotation skip 的確切互動。
- Capture Result 是否可被多個 downstream consumer 共用。
- Failure 的 Recoverable 或 Terminal 分類。
- Retry 是否存在。
- Platform Context Change 是否主動中斷 Session。
- Component Interaction 是同步或非同步。
- Interaction 是否需要未來 Contract 文件。
- Feedback Requirement 的最終消費者。

## 18. Completion Boundary

完成 ARCH-0005 不代表：

- Interface Contract 完成。
- API 完成。
- Event 或 Command Model 完成。
- Thread Model 完成。
- Technology Selection 完成。
- Project Structure 完成。
- Architecture Freeze 完成。
- Ready for Coding。

本文件完成的判定條件：

- ARCH-0005 檔案存在，Review Status 與 Architecture Stability 都是 Draft。
- INT-001 至 INT-022 唯一且不可重用。
- 每個 Interaction 都有唯一 Initiator 與 Recipient；INT-016 的抽象占位已明確說明。
- Shared-state effect 僅使用 None、Read、Transition request。
- 沒有非 COMP-001 Component 直接改變 Shared State。
- Primary、Cancellation、Failure 三張 Sequence Diagram 均存在。
- Interaction Responsibility Matrix、Prohibited Interaction Matrix 與 Information Exchange Mapping 均存在。
- Clipboard 與 Output 維持平行。
- Annotation 維持 Optional。
- 沒有 Interface、API、Event Schema、Class 或技術選型。
- 沒有修改 Frozen PRD、Frozen Specs 或既有 Architecture。
- Markdown relative links 與 git diff --check 通過。

## 19. Prohibited Decisions

本文件不得建立或決定：

- Interface。
- Method、Parameter 或 Return type。
- API。
- Event type 或 Command type。
- Message schema。
- DTO 或 Record。
- Class 或 Service。
- Thread、Task 或 Async/await model。
- Event Bus 或 Message Broker。
- DI。
- Project、Assembly 或 Namespace。
- Framework、Language 或 Windows API。
- UI、Overlay、Toolbar 或 Annotation tools。
- Source code。
- 新 Feature、Module、Component 或 ADR。

