# ARCH-0004 Component Boundaries

## Document Control

| Field | Value |
| --- | --- |
| Document ID | ARCH-0004 |
| Title | Component Boundaries |
| Review Status | Draft |
| Architecture Stability | Draft |
| Version | 0.1 |
| Owner | TBD |
| Last reviewed | Not reviewed |
| Normative References | ARCH-0001、ARCH-0002、ARCH-0003、SPEC-0003、SPEC-0010、SPEC-BASELINE-REVIEW |
| Informative References | PRD-0002 至 PRD-0006、PRD-FREEZE-REVIEW、docs/Analysis/、docs/Decision/ |

## 1. Purpose

本文件將 ARCH-0003 的抽象 Module 責任細分為可管理的 Component Boundary，固定 Component 的 ownership、責任方向、共享狀態存取邊界與抽象資訊交換範圍。

本文件也為後續的 Component Interaction、Contract 與 ADR 文件提供架構基線，但不定義實際 Class、Interface、Method 或 Framework。

## 2. Scope

本文件只涵蓋：

- Component identity。
- Owning Module 與 Primary Layer。
- Primary responsibility。
- Inputs 與 outputs 的抽象語意。
- Allowed dependencies 與 Prohibited dependencies。
- Shared-state access boundary。
- Feature、Spec 與 Module traceability。

## 3. Non-goals

本文件不定義或決定：

- Class 名稱。
- Interface 名稱。
- Method signature。
- API endpoint。
- Event Bus。
- Message Broker。
- DI Container。
- Namespace。
- Project 或 Assembly。
- Threading。
- Serialization。
- Persistence。
- UI implementation。
- Framework 或 Language 選擇。
- Windows API 選擇。

## 4. Component ID Policy

Component 使用獨立且不可重用的識別碼：

~~~text
COMP-001
COMP-002
COMP-003
...
~~~

規則如下：

- Component ID 永不重用。
- Component 改名不改 ID。
- Deprecated ID 永久保留，不重新分配。
- Component ID 不得與 MOD、FEAT、SPEC、FR、SR、AC 混用。

Component Status 只能使用：

~~~text
Required
Candidate
Deferred
Deprecated
~~~

若必要性仍無法由 Frozen Spec 推導，維持 Candidate，不自行提升為 Required。

## 5. Component Boundary Catalog

### 5.1 COMP-001 — Workflow State Authority

| Field | Value |
| --- | --- |
| Component ID | COMP-001 |
| Name | Workflow State Authority |
| Status | Required |
| Owning Module | MOD-001 Workflow Orchestration |
| Primary Layer | Product Workflow Layer |
| Purpose | 持有共享 workflow state 的唯一權威邊界。 |
| Responsibilities | 維持 SPEC-0003 定義的共享狀態語意；接受合法狀態轉移請求；拒絕未授權轉移。 |
| Inputs | 抽象 workflow transition request、session lifecycle outcome、Feature status。 |
| Outputs | 共享 workflow state、transition outcome、state read result。 |
| Owns | Shared workflow state authority 與合法轉移語意。 |
| Does not own | Feature 內部行為、平台互動、UI 呈現、Clipboard 或 Output delivery。 |
| Allowed dependencies | COMP-002、COMP-003、COMP-011、COMP-012。 |
| Prohibited dependencies | 直接依賴 Platform Component；直接擁有 Capture、Annotation、Clipboard 或 Output 行為。 |
| Shared-state access | Authority。 |
| Feature traceability | FEAT-001、FEAT-005。 |
| Spec traceability | SPEC-0003、SPEC-0005、SPEC-0006、SPEC-0010。 |
| Architectural risks | 可能成為所有責任的集中點；需維持只擁有共享狀態權威。 |
| Open questions | Shared state 的完整邊界與 runtime verification：TBD。 |

### 5.2 COMP-002 — Session Lifecycle Boundary

| Field | Value |
| --- | --- |
| Component ID | COMP-002 |
| Name | Session Lifecycle Boundary |
| Status | Required |
| Owning Module | MOD-001 Workflow Orchestration |
| Primary Layer | Product Workflow Layer |
| Purpose | 管理單一 capture session 的建立、終止與 lifecycle 語意。 |
| Responsibilities | 建立 session intent；維持 session 的開始、完成、取消與失敗邊界；向 COMP-001 提交狀態轉移請求。 |
| Inputs | Capture session intent、user cancellation intent、Feature completion/failure status。 |
| Outputs | session lifecycle outcome、transition request、session boundary status。 |
| Owns | Session lifecycle 語意與 session boundary。 |
| Does not own | Shared state authority、Capture selection 內部行為、平台實作、UI。 |
| Allowed dependencies | COMP-001、COMP-003、COMP-011、COMP-012。 |
| Prohibited dependencies | 直接依賴 COMP-014 至 COMP-018；直接擁有平台互動。 |
| Shared-state access | Request transition。 |
| Feature traceability | FEAT-001、FEAT-005。 |
| Spec traceability | SPEC-0003、SPEC-0005、SPEC-0006、SPEC-0010。 |
| Architectural risks | Session lifecycle 與 Workflow State Authority 責任重疊。 |
| Open questions | Session 與 Capture Result 的生命週期關聯：TBD。 |

### 5.3 COMP-003 — Feature Flow Coordinator

| Field | Value |
| --- | --- |
| Component ID | COMP-003 |
| Name | Feature Flow Coordinator |
| Status | Required |
| Owning Module | MOD-002 Feature Coordination |
| Primary Layer | Feature Coordination Layer |
| Purpose | 協調五個 Feature 的合法流程方向，不擁有 Feature 內部能力。 |
| Responsibilities | 依 Frozen Specs 分派 Feature intent；維持 Annotation optional；啟動 Clipboard 與 Output 的平行 downstream path。 |
| Inputs | workflow intent、Feature status、Capture Result available status。 |
| Outputs | Feature coordination request、transition request、downstream path status。 |
| Owns | Cross-feature coordination intent 與合法順序。 |
| Does not own | Shared state authority、Capture、Annotation、Clipboard、Output 或平台操作。 |
| Allowed dependencies | COMP-001、COMP-002、COMP-004、COMP-007、COMP-009、COMP-010、COMP-011。 |
| Prohibited dependencies | 直接依賴 COMP-014 至 COMP-018；吞併 Domain Component 內部責任。 |
| Shared-state access | Request transition。 |
| Feature traceability | FEAT-001、FEAT-002、FEAT-003、FEAT-004、FEAT-005。 |
| Spec traceability | SPEC-0010、SPEC-0005、SPEC-0006、SPEC-0007、SPEC-0008、SPEC-0009。 |
| Architectural risks | 可能成為跨 Feature 的 God Component。 |
| Open questions | Feature coordination 與 session lifecycle 的分界：TBD。 |

### 5.4 COMP-004 — Capture Request Boundary

| Field | Value |
| --- | --- |
| Component ID | COMP-004 |
| Name | Capture Request Boundary |
| Status | Required |
| Owning Module | MOD-003 Capture Capability |
| Primary Layer | Domain Capability Layer |
| Purpose | 接受並驗證抽象 Capture Request。 |
| Responsibilities | 維持 Capture Request 的產品語意；判斷 request 是否進入 Selection；回報 request outcome。 |
| Inputs | Capture intent、selection scope intent、platform capture context。 |
| Outputs | accepted/rejected capture request、selection request、capture failure detail。 |
| Owns | Capture Request 的抽象語意。 |
| Does not own | Shared workflow state、平台 Capture 行為、Selection UI、Output 或 Clipboard。 |
| Allowed dependencies | COMP-003、COMP-005、COMP-014、COMP-017、COMP-018。 |
| Prohibited dependencies | 直接依賴具體平台技術；直接修改 COMP-001。 |
| Shared-state access | Request transition。 |
| Feature traceability | FEAT-001。 |
| Spec traceability | SPEC-0003、SPEC-0005、SPEC-0010。 |
| Architectural risks | Capture Request、Selection 與 Result 過度切割。 |
| Open questions | Capture Request 與 Selection 是否合併部分邊界：TBD。 |

### 5.5 COMP-005 — Selection Boundary

| Field | Value |
| --- | --- |
| Component ID | COMP-005 |
| Name | Selection Boundary |
| Status | Required |
| Owning Module | MOD-003 Capture Capability |
| Primary Layer | Domain Capability Layer |
| Purpose | 維持 Selection 的抽象狀態與完成邊界。 |
| Responsibilities | 接受 selection intent；維持 selecting、completed、cancelled 與 failed 的抽象語意；交付 selection outcome。 |
| Inputs | Selection request、input context、selection completion/cancel intent。 |
| Outputs | selection outcome、capture-ready outcome、selection failure detail。 |
| Owns | Selection state 的抽象語意與 selection completion boundary。 |
| Does not own | Input platform behavior、UI rendering、Shared State authority、Output 或 Clipboard。 |
| Allowed dependencies | COMP-004、COMP-006、COMP-014、COMP-017、COMP-018。 |
| Prohibited dependencies | 直接依賴 Windows API 或具體輸入技術；直接擁有 user-facing feedback。 |
| Shared-state access | Request transition。 |
| Feature traceability | FEAT-001。 |
| Spec traceability | SPEC-0003、SPEC-0005、SPEC-0010。 |
| Architectural risks | Selection 與 platform interaction boundary 混淆。 |
| Open questions | Selection 的完整 scope 語意與 runtime 邊界：UNKNOWN/TBD。 |

### 5.6 COMP-006 — Capture Result Boundary

| Field | Value |
| --- | --- |
| Component ID | COMP-006 |
| Name | Capture Result Boundary |
| Status | Required |
| Owning Module | MOD-003 Capture Capability |
| Primary Layer | Domain Capability Layer |
| Purpose | 產生並交付抽象 Capture Result。 |
| Responsibilities | 接收 Selection outcome；建立 Capture Result available 語意；將結果交給 Annotation、Clipboard 與 Output downstream。 |
| Inputs | completed selection outcome、capture outcome。 |
| Outputs | abstract Capture Result、result-ready status、result failure detail。 |
| Owns | Capture Result 的抽象可用性邊界。 |
| Does not own | Result 的資料格式、Storage、Clipboard handoff、Output delivery 或 Annotation mutation。 |
| Allowed dependencies | COMP-005、COMP-007、COMP-009、COMP-010。 |
| Prohibited dependencies | 直接依賴 COMP-015 或 COMP-016；定義資料格式或持久化。 |
| Shared-state access | Request transition。 |
| Feature traceability | FEAT-001、FEAT-003、FEAT-004。 |
| Spec traceability | SPEC-0003、SPEC-0005、SPEC-0007、SPEC-0008、SPEC-0010。 |
| Architectural risks | Shared Result contract ownership 不清。 |
| Open questions | Capture Result 的抽象語意與 consumer handoff：TBD。 |

### 5.7 COMP-007 — Annotation Session Boundary

| Field | Value |
| --- | --- |
| Component ID | COMP-007 |
| Name | Annotation Session Boundary |
| Status | Required |
| Owning Module | MOD-004 Annotation Capability |
| Primary Layer | Domain Capability Layer |
| Purpose | 管理 optional annotation session 的進入、跳過與完成語意。 |
| Responsibilities | 接受 Result Available；允許使用者選擇 annotation path 或直接跳過；維持 annotation session boundary。 |
| Inputs | Capture Result available status、annotation intent、skip intent。 |
| Outputs | annotation session status、annotated result available status、skip outcome。 |
| Owns | Optional annotation session 的抽象生命週期。 |
| Does not own | Annotation tool selection、UI、Annotation object format、Output 或 Clipboard internals。 |
| Allowed dependencies | COMP-003、COMP-006、COMP-008、COMP-011。 |
| Prohibited dependencies | 將 Annotation 變成 Capture、Clipboard 或 Output 的必要前置條件。 |
| Shared-state access | Request transition。 |
| Feature traceability | FEAT-002。 |
| Spec traceability | SPEC-0003、SPEC-0009、SPEC-0010。 |
| Architectural risks | Optional path 被錯誤提升為必要流程。 |
| Open questions | Annotation session 的完整 runtime boundary：TBD。 |

### 5.8 COMP-008 — Annotation Mutation Boundary

| Field | Value |
| --- | --- |
| Component ID | COMP-008 |
| Name | Annotation Mutation Boundary |
| Status | Candidate |
| Owning Module | MOD-004 Annotation Capability |
| Primary Layer | Domain Capability Layer |
| Purpose | 統一建立、修改、移除 Annotation 的抽象語意邊界。 |
| Responsibilities | 接受 annotation change intent；回報 changed、removed 或 failed outcome；不決定具體工具。 |
| Inputs | annotation session status、annotation change intent。 |
| Outputs | annotation change outcome、annotated result status、failure detail。 |
| Owns | Annotation mutation 的抽象責任邊界。 |
| Does not own | Arrow、Rectangle、Text、Blur、Pen 或其他具體工具；資料格式、Storage、Undo/Redo 或 UI。 |
| Allowed dependencies | COMP-007、COMP-006、COMP-011。 |
| Prohibited dependencies | 依賴未批准的 Tool Spec；直接依賴 Output 或 Clipboard。 |
| Shared-state access | Request transition。 |
| Feature traceability | FEAT-002。 |
| Spec traceability | SPEC-0003、SPEC-0009、SPEC-0010。 |
| Architectural risks | Mutation boundary 可能過早固定。 |
| Open questions | 是否等 Tool Specs 後再拆分 mutation responsibility：TBD。 |

### 5.9 COMP-009 — Clipboard Handoff Boundary

| Field | Value |
| --- | --- |
| Component ID | COMP-009 |
| Name | Clipboard Handoff Boundary |
| Status | Required |
| Owning Module | MOD-005 Clipboard Handoff Capability |
| Primary Layer | Domain Capability Layer |
| Purpose | 管理 Capture Result 或 Annotated Result 到 Clipboard Consumer 的 downstream handoff。 |
| Responsibilities | 維持 handoff pending、consumer accepted 與 handoff error 的抽象語意；回報 handoff outcome。 |
| Inputs | Capture Result、Annotated Result、Clipboard handoff intent。 |
| Outputs | Clipboard-ready status、consumer acceptance outcome、handoff failure detail。 |
| Owns | Clipboard handoff product semantics。 |
| Does not own | Clipboard API、資料格式、Consumer 內部行為、Output delivery 或 Shared State authority。 |
| Allowed dependencies | COMP-003、COMP-006、COMP-007、COMP-015、COMP-017。 |
| Prohibited dependencies | 依賴 COMP-010 完成；直接依賴平台 API 或具體資料格式。 |
| Shared-state access | Request transition。 |
| Feature traceability | FEAT-003。 |
| Spec traceability | SPEC-0003、SPEC-0007、SPEC-0010。 |
| Architectural risks | Handoff semantics 洩漏平台行為。 |
| Open questions | Consumer acceptance 與既有 Clipboard context：UNKNOWN/TBD。 |

### 5.10 COMP-010 — Output Delivery Boundary

| Field | Value |
| --- | --- |
| Component ID | COMP-010 |
| Name | Output Delivery Boundary |
| Status | Required |
| Owning Module | MOD-006 Output Capability |
| Primary Layer | Domain Capability Layer |
| Purpose | 管理 Capture Result 或 Annotated Result 的 downstream Output delivery。 |
| Responsibilities | 維持 output-ready、delivery pending、delivered 與 error 的抽象語意；回報 delivery outcome。 |
| Inputs | Capture Result、Annotated Result、Output delivery intent。 |
| Outputs | Output-ready status、delivery outcome、output failure detail。 |
| Owns | Output delivery product semantics。 |
| Does not own | File format、Storage、File IO、Save location、Clipboard handoff 或 UI。 |
| Allowed dependencies | COMP-003、COMP-006、COMP-007、COMP-016、COMP-018。 |
| Prohibited dependencies | 依賴 COMP-009 完成；直接依賴平台 Output API 或持久化。 |
| Shared-state access | Request transition。 |
| Feature traceability | FEAT-004。 |
| Spec traceability | SPEC-0003、SPEC-0008、SPEC-0010。 |
| Architectural risks | Output delivery 過早落入檔案與 Storage 設計。 |
| Open questions | Output Adapter 是否為 MVP 必要：TBD。 |

### 5.11 COMP-011 — Completion and Cancellation Boundary

| Field | Value |
| --- | --- |
| Component ID | COMP-011 |
| Name | Completion and Cancellation Boundary |
| Status | Required |
| Owning Module | MOD-007 Workflow Boundary and Feedback |
| Primary Layer | Feature Coordination Layer |
| Purpose | 統一跨 Feature 的完成與取消語意。 |
| Responsibilities | 分類 completion、cancel 與 downstream handoff boundary；向 COMP-001 提交合法的流程轉移請求。 |
| Inputs | Feature completion status、user cancellation intent、downstream outcome。 |
| Outputs | shared completion/cancellation outcome、transition request、boundary status。 |
| Owns | Completion 與 cancellation 的跨 Feature boundary semantics。 |
| Does not own | Capture、Annotation、Clipboard、Output 的內部成功或失敗判斷。 |
| Allowed dependencies | COMP-001、COMP-002、COMP-003、COMP-006、COMP-009、COMP-010。 |
| Prohibited dependencies | 直接依賴 Platform Component；吞併 Feature 內部 ownership。 |
| Shared-state access | Request transition。 |
| Feature traceability | FEAT-005。 |
| Spec traceability | SPEC-0003、SPEC-0005、SPEC-0006、SPEC-0010。 |
| Architectural risks | Boundary component 與 Feature Flow Coordinator 責任重疊。 |
| Open questions | Completion 與 downstream delivery 的正式關係：TBD。 |

### 5.12 COMP-012 — Failure Classification Boundary

| Field | Value |
| --- | --- |
| Component ID | COMP-012 |
| Name | Failure Classification Boundary |
| Status | Required |
| Owning Module | MOD-007 Workflow Boundary and Feedback |
| Primary Layer | Feature Coordination Layer |
| Purpose | 將 Feature failure 分類為共享流程語意。 |
| Responsibilities | 接收各 owning Component 的 failure detail；分類 recoverable、terminal、handoff 或 interruption boundary；不改寫原始 ownership。 |
| Inputs | Capture、Annotation、Clipboard、Output、platform failure detail。 |
| Outputs | failure classification、shared boundary status、transition request。 |
| Owns | Cross-feature failure classification semantics。 |
| Does not own | 各 Feature 的內部 failure 判斷、平台 failure cause、retry policy 或 logging。 |
| Allowed dependencies | COMP-001、COMP-003、COMP-004、COMP-007、COMP-009、COMP-010、COMP-014、COMP-015、COMP-016。 |
| Prohibited dependencies | 直接執行平台操作；吞併各 Feature 的 failure ownership。 |
| Shared-state access | Request transition。 |
| Feature traceability | FEAT-005。 |
| Spec traceability | SPEC-0003、SPEC-0006、SPEC-0010。 |
| Architectural risks | Failure classification 可能變成所有錯誤的集中責任。 |
| Open questions | Runtime failure taxonomy：UNKNOWN/TBD。 |

### 5.13 COMP-013 — Feedback Boundary

| Field | Value |
| --- | --- |
| Component ID | COMP-013 |
| Name | Feedback Boundary |
| Status | Candidate |
| Owning Module | MOD-007 Workflow Boundary and Feedback |
| Primary Layer | Feature Coordination Layer |
| Purpose | 定義回饋需求與可存取性邊界，不決定 UI 呈現。 |
| Responsibilities | 接收 completion、cancel、failure 與 interruption classification；描述需要回饋的語意與 urgency。 |
| Inputs | COMP-011 outcome、COMP-012 classification、accessibility requirement。 |
| Outputs | feedback requirement、feedback status、boundary evidence。 |
| Owns | User-facing feedback 的抽象需求 boundary。 |
| Does not own | Notification、Editor、Overlay、Toolbar、UI rendering 或平台 display implementation。 |
| Allowed dependencies | COMP-011、COMP-012、COMP-018。 |
| Prohibited dependencies | 直接依賴 UI、Notification API 或具體呈現技術。 |
| Shared-state access | Read。 |
| Feature traceability | FEAT-005。 |
| Spec traceability | SPEC-0003、SPEC-0006、SPEC-0010。 |
| Architectural risks | Feedback boundary 與 UI implementation 混淆。 |
| Open questions | Feedback 是否需要獨立 Component：TBD。 |

### 5.14 COMP-014 — Platform Capture Adapter Boundary

| Field | Value |
| --- | --- |
| Component ID | COMP-014 |
| Name | Platform Capture Adapter Boundary |
| Status | Required |
| Owning Module | MOD-008 Platform Capture Integration |
| Primary Layer | Platform Integration Layer |
| Purpose | 隔離平台 Capture 行為，提供抽象 capture outcome。 |
| Responsibilities | 接受抽象 Capture Request；對接平台 capture context；回報 capture outcome，不將平台型別洩漏到上層。 |
| Inputs | COMP-004 的抽象 request、platform context。 |
| Outputs | abstract capture outcome、platform failure detail。 |
| Owns | Platform capture interaction boundary。 |
| Does not own | Capture product semantics、Selection state、Shared State、UI 或技術選擇。 |
| Allowed dependencies | COMP-017、COMP-018 的抽象 context：TBD。 |
| Prohibited dependencies | 依賴 Product Workflow、Feature Coordination 或 Domain Component 的內部實作。 |
| Shared-state access | No access。 |
| Feature traceability | FEAT-001。 |
| Spec traceability | SPEC-0003、SPEC-0005、SPEC-0010。 |
| Architectural risks | Platform adapter 洩漏具體 platform type。 |
| Open questions | Runtime verification 是否要求新的 platform boundary：TBD。 |

### 5.15 COMP-015 — Platform Clipboard Adapter Boundary

| Field | Value |
| --- | --- |
| Component ID | COMP-015 |
| Name | Platform Clipboard Adapter Boundary |
| Status | Required |
| Owning Module | MOD-009 Platform Clipboard Integration |
| Primary Layer | Platform Integration Layer |
| Purpose | 隔離平台 Clipboard 行為，提供抽象 handoff outcome。 |
| Responsibilities | 接受抽象 Clipboard handoff intent；對接 platform Clipboard context；回報 consumer acceptance 或 error。 |
| Inputs | COMP-009 的抽象 handoff intent、platform context。 |
| Outputs | Clipboard handoff outcome、platform failure detail。 |
| Owns | Platform Clipboard interaction boundary。 |
| Does not own | Clipboard product semantics、資料格式、Output、Storage、Shared State 或 UI。 |
| Allowed dependencies | COMP-017、COMP-018 的抽象 context：TBD。 |
| Prohibited dependencies | 依賴 Product Workflow、Feature Coordination 或直接依賴 COMP-010。 |
| Shared-state access | No access。 |
| Feature traceability | FEAT-003。 |
| Spec traceability | SPEC-0003、SPEC-0007、SPEC-0010。 |
| Architectural risks | Platform Clipboard 行為與 product handoff semantics 混淆。 |
| Open questions | Existing Clipboard content、permission 與 consumer context：UNKNOWN/TBD。 |

### 5.16 COMP-016 — Platform Output Adapter Boundary

| Field | Value |
| --- | --- |
| Component ID | COMP-016 |
| Name | Platform Output Adapter Boundary |
| Status | Candidate |
| Owning Module | MOD-010 Platform Output Integration |
| Primary Layer | Platform Integration Layer |
| Purpose | 隔離平台 Output 行為，提供抽象 delivery outcome。 |
| Responsibilities | 接受抽象 Output delivery intent；對接 platform output context；回報 delivery outcome。 |
| Inputs | COMP-010 的抽象 delivery intent、platform context。 |
| Outputs | Output delivery outcome、platform failure detail。 |
| Owns | Platform Output interaction boundary。 |
| Does not own | Output product semantics、File format、Storage、File IO、Clipboard 或 UI。 |
| Allowed dependencies | COMP-017、COMP-018 的抽象 context：TBD。 |
| Prohibited dependencies | 依賴 Product Workflow、Feature Coordination 或直接依賴 COMP-009。 |
| Shared-state access | No access。 |
| Feature traceability | FEAT-004。 |
| Spec traceability | SPEC-0003、SPEC-0008、SPEC-0010。 |
| Architectural risks | Output adapter 過早被視為 MVP 必要。 |
| Open questions | Output delivery 的平台 scope 與必要性：TBD。 |

### 5.17 COMP-017 — Platform Input Boundary

| Field | Value |
| --- | --- |
| Component ID | COMP-017 |
| Name | Platform Input Boundary |
| Status | Candidate |
| Owning Module | MOD-011 Platform Interaction Integration |
| Primary Layer | Platform Integration Layer |
| Purpose | 隔離鍵盤、指標與其他輸入平台行為。 |
| Responsibilities | 提供抽象 input context；回報 input context change；不決定 workflow 或 Selection 語意。 |
| Inputs | platform input context、user input outcome。 |
| Outputs | abstract input context、input availability status。 |
| Owns | Platform input interaction boundary。 |
| Does not own | Capture Request、Selection semantics、Shortcut policy、UI 或 Shared State。 |
| Allowed dependencies | Platform support relationship：TBD。 |
| Prohibited dependencies | 依賴 Product Workflow、Feature Coordination 或 Domain Component 的內部實作。 |
| Shared-state access | No access。 |
| Feature traceability | FEAT-001、FEAT-005。 |
| Spec traceability | SPEC-0003、SPEC-0005、SPEC-0006、SPEC-0010。 |
| Architectural risks | Input boundary 可能與 Display Context 混合。 |
| Open questions | Platform Input 是否需再拆分：TBD。 |

### 5.18 COMP-018 — Platform Display Context Boundary

| Field | Value |
| --- | --- |
| Component ID | COMP-018 |
| Name | Platform Display Context Boundary |
| Status | Candidate |
| Owning Module | MOD-011 Platform Interaction Integration |
| Primary Layer | Platform Integration Layer |
| Purpose | 隔離焦點、螢幕、DPI、HDR 等 platform context。 |
| Responsibilities | 提供抽象 display/focus context；回報 context change；支援 feedback、Capture、Clipboard 與 Output 的平台邊界。 |
| Inputs | platform focus、display、DPI、HDR context。 |
| Outputs | abstract display context、focus/context status。 |
| Owns | Platform display and focus context boundary。 |
| Does not own | UI rendering、Capture、Output、Clipboard、Shared State 或產品流程。 |
| Allowed dependencies | Platform support relationship：TBD。 |
| Prohibited dependencies | 依賴 Product Workflow、Feature Coordination 或 Domain Component 的內部實作。 |
| Shared-state access | No access。 |
| Feature traceability | FEAT-001、FEAT-003、FEAT-004、FEAT-005。 |
| Spec traceability | SPEC-0003、SPEC-0006、SPEC-0007、SPEC-0008、SPEC-0010。 |
| Architectural risks | Platform context 過度泛化，或洩漏成 UI implementation。 |
| Open questions | Focus、Display、DPI、HDR 是否需拆分：TBD。 |

## 6. Module-to-Component Mapping

| Module | Primary Components | Supporting Components | Coverage status |
| --- | --- | --- | --- |
| MOD-001 Workflow Orchestration | COMP-001、COMP-002 | — | Covered |
| MOD-002 Feature Coordination | COMP-003 | COMP-011、COMP-012、COMP-013 | Covered |
| MOD-003 Capture Capability | COMP-004、COMP-005、COMP-006 | — | Covered |
| MOD-004 Annotation Capability | COMP-007 | COMP-008 | Covered; optional mutation |
| MOD-005 Clipboard Handoff Capability | COMP-009 | — | Covered |
| MOD-006 Output Capability | COMP-010 | — | Covered |
| MOD-007 Workflow Boundary and Feedback | COMP-011、COMP-012 | COMP-013 | Covered |
| MOD-008 Platform Capture Integration | COMP-014 | — | Covered |
| MOD-009 Platform Clipboard Integration | COMP-015 | — | Covered |
| MOD-010 Platform Output Integration | COMP-016 | — | Covered; Candidate |
| MOD-011 Platform Interaction Integration | COMP-017、COMP-018 | — | Covered; Candidate |

每個 Component 只有一個 Owning Module，Component 不跨 Module 擁有責任。

## 7. Feature-to-Component Mapping

| Feature | Primary Components | Supporting Components | Platform Components |
| --- | --- | --- | --- |
| FEAT-001 Capture Workflow | COMP-002 Session Lifecycle Boundary | COMP-001、COMP-003、COMP-004、COMP-005、COMP-006、COMP-011、COMP-012 | COMP-014、COMP-017、COMP-018 |
| FEAT-002 Annotation | COMP-007 Annotation Session Boundary | COMP-008、COMP-011、COMP-012 | COMP-018：TBD |
| FEAT-003 Clipboard Handoff | COMP-009 Clipboard Handoff Boundary | COMP-001、COMP-003、COMP-006、COMP-011、COMP-012 | COMP-015、COMP-018：TBD |
| FEAT-004 Capture Output | COMP-010 Output Delivery Boundary | COMP-001、COMP-003、COMP-006、COMP-011、COMP-012 | COMP-016、COMP-018：TBD |
| FEAT-005 Workflow Boundaries and Feedback | COMP-011 Completion and Cancellation Boundary | COMP-001、COMP-002、COMP-003、COMP-012、COMP-013 | COMP-017、COMP-018：TBD |

FEAT-001 至 FEAT-005 的 Primary Component 所屬 Module 分別與 ARCH-0003 的 Primary Module 一致。

## 8. Component Dependency Rules

- COMP-001 是 Shared State 唯一的 Authority。
- 其他 Component 不得直接修改 Shared State，只能使用 Read、Request transition 或 No access。
- COMP-003 可協調 Domain Components，但不得執行平台操作。
- Domain Components 只能透過 Platform Adapter Boundary 接觸平台行為。
- Platform Components 不得依賴 Product Workflow 或 Feature Coordination Components。
- Clipboard 與 Output Component 保持平行，不得互相成為必要依賴。
- Annotation Components 必須維持 Optional。
- COMP-012 可分類 failure，但不得吞併各 Feature 的內部 failure ownership。
- Component 之間不得形成循環依賴。
- Component 間只能交換 Frozen Specs 已支持的抽象資訊。

## 9. Component Dependency Diagram

以下 Mermaid 只表示抽象 Component 的合法依賴方向；虛線表示尚未固定的 platform supporting context。

~~~mermaid
flowchart TB
    C1["COMP-001 Workflow State Authority"]
    C2["COMP-002 Session Lifecycle Boundary"]
    C3["COMP-003 Feature Flow Coordinator"]

    C4["COMP-004 Capture Request Boundary"]
    C5["COMP-005 Selection Boundary"]
    C6["COMP-006 Capture Result Boundary"]

    C7["COMP-007 Annotation Session Boundary - Optional"]
    C8["COMP-008 Annotation Mutation Boundary - Candidate"]
    C9["COMP-009 Clipboard Handoff Boundary"]
    C10["COMP-010 Output Delivery Boundary"]
    C11["COMP-011 Completion and Cancellation Boundary"]
    C12["COMP-012 Failure Classification Boundary"]
    C13["COMP-013 Feedback Boundary - Candidate"]

    C14["COMP-014 Platform Capture Adapter Boundary"]
    C15["COMP-015 Platform Clipboard Adapter Boundary"]
    C16["COMP-016 Platform Output Adapter Boundary - Candidate"]
    C17["COMP-017 Platform Input Boundary - Candidate"]
    C18["COMP-018 Platform Display Context Boundary - Candidate"]

    C2 -->|Request transition| C1
    C3 -->|Request transition| C1
    C11 -->|Request transition| C1
    C12 -->|Request transition| C1

    C3 --> C4
    C4 --> C5
    C5 --> C6
    C3 -.->|Optional path| C7
    C7 -.->|Candidate mutation| C8

    C3 --> C9
    C3 --> C10
    C3 --> C11
    C9 --> C12
    C10 --> C12
    C7 --> C12
    C11 --> C13
    C12 --> C13

    C4 --> C14
    C9 --> C15
    C10 --> C16
    C14 -.->|Platform context TBD| C17
    C14 -.->|Display context TBD| C18
    C15 -.->|Platform context TBD| C18
    C16 -.->|Platform context TBD| C18
~~~

圖中的 C9 Clipboard 與 C10 Output 是平行 downstream 分支；C7 Annotation 明確標示為 Optional。C1 是唯一 Shared State Authority，其他 Component 只能提出 transition request 或讀取狀態。

## 10. Responsibility Matrix

| Responsibility | Primary Component | Supporting Component | Explicitly not owned by |
| --- | --- | --- | --- |
| Shared workflow state | COMP-001 | COMP-002、COMP-003 | COMP-004 至 COMP-018 |
| Session lifecycle | COMP-002 | COMP-001、COMP-003 | Platform Components |
| Capture request | COMP-004 | COMP-003、COMP-014 | COMP-001、COMP-009、COMP-010 |
| Selection | COMP-005 | COMP-004、COMP-014、COMP-017 | COMP-001、COMP-010 |
| Capture result | COMP-006 | COMP-005、COMP-007 | COMP-009、COMP-010 的 delivery details |
| Annotation session | COMP-007 | COMP-006、COMP-003 | COMP-009、COMP-010 |
| Annotation mutation | COMP-008 | COMP-007 | COMP-009、COMP-010、Platform Components |
| Clipboard handoff | COMP-009 | COMP-006、COMP-015 | COMP-010 |
| Output delivery | COMP-010 | COMP-006、COMP-016 | COMP-009 |
| Completion | COMP-011 | COMP-001、COMP-003 | Feature capability internals |
| Cancellation | COMP-011 | COMP-001、COMP-002 | Platform Components |
| Failure classification | COMP-012 | owning Feature Components | Feature internal failure ownership |
| Feedback requirements | COMP-013 | COMP-011、COMP-012 | UI、Overlay、Toolbar |
| Platform capture interaction | COMP-014 | COMP-017、COMP-018 | Domain Components |
| Platform clipboard interaction | COMP-015 | COMP-018 | COMP-009 的 product semantics |
| Platform output interaction | COMP-016 | COMP-018 | COMP-010 的 product semantics |
| Input interaction | COMP-017 | COMP-014、COMP-018 | Product Workflow Components |
| Focus/display/DPI/HDR context | COMP-018 | COMP-014、COMP-015、COMP-016 | Product Workflow Components |

每項責任只有一個 Primary Component。

## 11. Information Boundary Catalog

本節只描述抽象語意，不定義 Schema、Record、DTO、Message、Serialization 或其他資料格式。

| Information boundary | Producer | Consumer | Meaning | Format status |
| --- | --- | --- | --- | --- |
| Capture Request | COMP-003、COMP-004 | COMP-004、COMP-014 | 請求啟動一次 Capture workflow。 | TBD |
| Selection State | COMP-005 | COMP-006、COMP-001 | Selection 的抽象進行、完成、取消或失敗語意。 | TBD |
| Capture Result | COMP-006 | COMP-007、COMP-009、COMP-010 | 一次 Capture 已產生且可交付的抽象結果。 | TBD |
| Annotation Change | COMP-008 | COMP-007、COMP-006 | Annotation 建立、修改或移除的抽象變更語意。 | TBD |
| Clipboard Handoff Request | COMP-009 | COMP-015 | 請求將結果交付給 Clipboard Consumer。 | TBD |
| Output Delivery Request | COMP-010 | COMP-016 | 請求將結果交付至 Output downstream。 | TBD |
| Workflow Transition Request | COMP-002、COMP-003、COMP-011、COMP-012 | COMP-001 | 請求共享 workflow state 進行合法轉移。 | TBD |
| Failure Classification | COMP-012 | COMP-001、COMP-011、COMP-013 | 將 Feature 或 platform failure 轉為共享流程邊界語意。 | TBD |
| Feedback Requirement | COMP-013 | 未定義的 feedback consumer | 描述使用者需要知道的 completion、cancel、failure 或 interruption 結果。 | TBD |
| Platform Context Change | COMP-017、COMP-018 | COMP-004、COMP-005、COMP-009、COMP-010、COMP-013 | 平台輸入、焦點、顯示與脈絡的抽象變化。 | TBD |

Information boundary 的 Producer、Consumer 與 Format 仍可由後續 Frozen Spec 或 ADR 變更流程調整；本文件不提前決定實作型態。

## 12. Traceability

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
Component Boundary
~~~

每個 Component 必須追溯至：

- 一個 Owning Module。
- 一個或多個 Feature 或 Spec。
- ARCH-0001 Architecture Principles。
- ARCH-0002 Layer Model。
- ARCH-0003 Module Catalog。

本文件不得直接以 Research 內容建立 Component；Research 必須先經 Analysis、Decision、PRD 與 Spec 的既定來源鏈。

## 13. Architectural Risks

以下只記錄風險，不在本文件解決：

- COMP-001 成為 God Component。
- Workflow State Authority 與 Session Lifecycle 重疊。
- Feature Flow Coordinator 吞併 Domain responsibility。
- Capture Request、Selection、Result 過度切割。
- Annotation Mutation Boundary 過早固定。
- Failure Classification 吞併 Feature failure ownership。
- Feedback Boundary 與 UI 混淆。
- Platform Adapter Boundary 洩漏平台型別。
- Information Boundary 被誤解成具體資料結構。
- Candidate Component 過早提升為 Required。

## 14. Open Questions

以下問題保留為 TBD，不在本文件回答：

- Capture Request、Selection、Result 是否應合併部分 Component。
- Annotation Mutation 是否應等 Tool Specs 後再固定。
- Feedback 是否需要獨立 Component。
- Platform Input 與 Display Context 是否需要拆分。
- Output Adapter 是否為 MVP 必要。
- Shared Result contract 由哪個 Component 擁有。
- Runtime verification 是否改變 Platform Component 邊界。
- Component 如何映射到未來 Project 或 Assembly。
- Component interaction 是同步還是非同步。

## 15. Completion Boundary

完成 ARCH-0004 不代表：

- Interface Contract 完成。
- Component Interaction Spec 完成。
- ADR 完成。
- 技術選型完成。
- Project structure 完成。
- Ready for Coding。

本文件完成的判定條件：

- ARCH-0004 檔案存在，Review Status 與 Architecture Stability 都是 Draft。
- COMP-001 至 COMP-018 唯一且不可重用。
- 每個 Component 只有一個 Owning Module 與一個 Primary Layer。
- 只有 COMP-001 具有 Shared State Authority。
- 其他 Component 只使用 Read、Request transition 或 No access。
- MOD-001 至 MOD-011 全部有 Component coverage。
- 五個 Feature 全部可追溯到 Component。
- Module-to-Component Mapping、Feature-to-Component Mapping、Dependency Diagram、Responsibility Matrix 與 Information Boundary Catalog 均存在。
- Clipboard 與 Output 維持平行。
- Annotation 維持 Optional。
- 沒有 Interface、API、Class、Framework 或技術選型。
- 沒有修改 Frozen PRD、Frozen Specs 或既有 Architecture。
- Markdown relative links 與 git diff --check 通過。

## 16. Prohibited Decisions

本文件不得建立或決定：

- Interface Catalog。
- Method Signature。
- API。
- Event、Command 或 Message design。
- DTO 或 Message Schema。
- Class。
- Service。
- Project、Assembly 或 Namespace。
- DI。
- Threading 或 Async model。
- Persistence、Logging 或 Telemetry。
- Framework、Language 或 Windows API。
- UI、Overlay、Toolbar 或 Annotation tools。
- Source code。
- 新 Feature、Module 或 ADR。

