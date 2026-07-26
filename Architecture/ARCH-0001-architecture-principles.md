# ARCH-0001 Architecture Principles

狀態：`Draft`

本文件是 Architecture layer 的治理基線，只固定 Architecture 應遵守的原則；它不是 Module Design、Component Design、Technology Decision 或 implementation plan。

## 1. Purpose

本文件的目的，是在 Architecture 進入具體邊界、模組與技術選擇前，先固定：

- Architecture 的責任範圍。
- Architecture 與 Frozen PRD、Frozen Specification 的關係。
- Product requirement、Feature boundary、Shared State、implementation 與 ADR 的責任分界。
- 未來 Architecture 變更的追溯與審查入口。

## 2. Scope

本文件只描述 Architecture Principles、Architectural Constraints、Traceability、Decision Policy、Change Policy 與 Open Questions。

本文件不描述：

- Module、Layer、Service、Class、Interface、API 或 Event。
- UI、Overlay、Toolbar、Annotation Tool、Clipboard implementation 或 Output format。
- Framework、作業系統 API、Database、File、Thread、Storage 或 deployment topology。
- 任何 implementation、build、test、package、deploy 或 release 操作。

## 3. Principles

### Principle 1 — Architecture shall not redefine product requirements

Architecture 不得自行新增、刪除或改寫 Frozen PRD 的產品需求、使用者價值、Feature scope、優先級或 acceptance intent。若 Architecture 發現產品需求缺口，必須回到既定 Change Policy。

### Principle 2 — Architecture shall derive only from Frozen Specifications

Architecture 的 capability、boundary、責任與約束只能由 Frozen PRD、Specification baseline 與已核准的 Architecture Decision 推導。未經 Freeze 的推測、聊天內容或個人偏好不得直接成為 Architecture contract。

### Principle 3 — Shared states have a single source of truth

Shared workflow state vocabulary 以 [SPEC-0003 System Requirements](../Specs/SPEC-0003-system-requirements.md) 為唯一來源。Feature local lifecycle 可以描述自身責任，但不得建立互相衝突的第二套 shared state model。

### Principle 4 — One responsibility per module

未來具體 Architecture element 必須有清楚且可驗收的主要責任。不得讓同一個 element 同時吞併產品規則、UI、平台副作用、資料保存與外部整合，除非後續 ADR 明確記錄其必要性與取捨。

### Principle 5 — Feature boundaries shall be preserved

Architecture 必須維持 [SPEC-0010 Feature Integration](../Specs/SPEC-0010-feature-integration.md) 定義的五個核心 Feature boundary 與 Primary Owner。`FEAT-003 Clipboard Handoff` 與 `FEAT-004 Capture Output` 的平行 downstream relationship 不得被 Architecture 偷改成彼此的必要依賴。

### Principle 6 — Implementation shall conform to Architecture

Implementation 應遵守已核准的 Architecture；當 implementation 遇到衝突、缺口或不可行條件時，必須提出 Architecture Change 或 ADR，而不是在程式碼、測試或部署設定中靜默修改 Architecture。

### Principle 7 — Runtime verification shall not silently redefine Architecture

Runtime evidence 可以揭露 `UNKNOWN/TBD`、平台差異、失敗邊界或實作風險，但不得直接把 Architecture 改成新的產品規則。需要改變產品或 Feature boundary 時，必須回到 Research、Analysis、Decision 與相應的 PRD/Spec Change 流程。

### Principle 8 — Technology decisions shall be traceable

任何具有長期影響、不可逆成本、跨邊界取捨或會限制未來演進的 technology decision，都必須有可追溯的 ADR。Architecture artifact 必須引用相關 ADR；本文件不預先建立或批准任何技術方案。

## 4. Architectural Constraints

Architecture layer 必須遵守：

- 不新增 Feature、FR、SR、NFR 或產品需求。
- 不修改 Frozen PRD 或 Frozen Specification；需求變更必須走 Change Policy。
- 不把 `UNKNOWN/TBD` 寫成已確認的產品、平台或 runtime 行為。
- 不以 Architecture 名義提前決定 Module、Layer、Service、Class、Interface、API、Framework、Database、File、Thread 或 Clipboard API。
- 不讓某一個 Feature 的內部實作吞併另一個 Feature 的 Primary Owner 責任。
- 不用 Research 內容直接取代 Frozen PRD/Spec；Research 必須先經過既定的 Analysis、Decision 與產品/規格流程。
- 不把 `Ready for Architecture` 誤讀成 `Ready for Coding`。

## 5. Traceability

Architecture artifact 的最小來源鏈為：

```text
Frozen PRD
  ↓
Specification v1.0 Freeze Approved
  ↓
Architecture artifact
  ↓
ADR（若有長期技術取捨）
```

Architecture 必須：

- 引用 [PRD Freeze Review](../PRD/PRD-FREEZE-REVIEW.md) 與 [SPEC Baseline Review](../Specs/SPEC-BASELINE-REVIEW.md) 作為進入基線。
- 直接引用與 artifact 責任相關的 Frozen PRD、SPEC-0003、SPEC-0010 與個別 Feature Spec。
- 讓每一個 capability、boundary、責任分配與 major constraint 可以回溯至來源文件。
- 在資料不足時引用 `UNKNOWN/TBD`，不要使用沒有來源的 implementation assumption。
- 不直接把 Research 當成 Architecture contract；Research 只能透過已核准的上游文件影響 Architecture。

## 6. Decision Policy

Architecture Decision 必須：

- 說明問題、背景、候選方案、取捨、影響與回滾或替代路徑。
- 判斷是否屬於 hard-to-reverse、跨邊界、長期維護或會限制演進的決策。
- 對符合條件的決策建立獨立 ADR，並從 Architecture 入口連結。
- 不在一般 Architecture prose 中隱藏重大取捨。
- 不以 ADR 取代 Frozen PRD、Spec 或產品批准。

本文件只定義 Decision Policy，不建立任何 ADR，也不選擇任何 technology。

## 7. Change Policy

Architecture 修改必須遵循：

```text
Research
  ↓
Analysis
  ↓
Decision
  ↓
PRD
  ↓
Spec
  ↓
Architecture Change
  ↓
Review
```

適用規則：

- 若變更涉及產品 intent、使用者可觀察行為或 Feature scope，先更新 PRD Change Request。
- 若變更涉及可驗收行為、FR/SR/NFR、Shared State 或 Feature boundary，先更新 Spec Change Request。
- 若變更只是 Architecture implementation constraint 或 technology trade-off，建立 Architecture Change 或 ADR，並維持既有 PRD/Spec 不變。
- 任何變更都必須保留來源、影響範圍、Review 結果與批准狀態。

## 8. Open Questions

本 Architecture Principles 不解決下列問題，只保留為後續 Architecture work 的輸入：

- Module、Layer、Service、Class、Interface 與 API 的正式邊界：`UNKNOWN/TBD`。
- Presentation、Application、Domain、Platform、Storage、External Services 與 Observability 的具體分配：`UNKNOWN/TBD`。
- 技術棧、目標 Windows 版本、部署方式與更新策略：`UNKNOWN/TBD`。
- Result、Annotation、Clipboard、Output 的資料生命週期與保存政策：`UNKNOWN/TBD`。
- Runtime、build、test、package、deploy、telemetry 與 rollback 的正式架構：`UNKNOWN/TBD`。
- 目前 PRD/Spec gaps 是否會要求 Architecture Change：`UNKNOWN`。

## 9. Completion Boundary

`ARCH-0001` 完成後，只代表 Architecture governance principles 已建立。它不代表 Module Design、Technology Selection、ADR、Implementation、Testing 或 Release 已開始或已批准。

完成本 Architecture Principles 文件後立即停止；下一份 Architecture 文件必須等待本文件 Review 與明確的下一個任務。
