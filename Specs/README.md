# Specs

Specifications 定義可觀察、可驗收的行為。它們必須能被產品、設計、開發與測試共同理解，不把尚未核准的推測寫成實作契約。

## 文件清單

- [SPEC-0001 Documentation baseline](SPEC-0001-documentation-baseline.md) — 文件治理 Spec，狀態為 `Accepted`。
- [SPEC-0002 Specification Guidelines](SPEC-0002-specification-guidelines.md) — Spec 格式、追溯、狀態與 Review 規則，狀態為 `Draft`。
- [SPEC-0003 System Requirements](SPEC-0003-system-requirements.md) — 共用系統層能力，狀態為 `Draft`。
- [SPEC-0004 Feature Catalog](SPEC-0004-feature-catalog.md) — Feature 分類與 FEAT/FR/SR 追溯，狀態為 `Draft`。
- [SPEC-0005 Capture Workflow](SPEC-0005-capture-workflow.md) — `FEAT-001` 的第一份 Feature Spec，狀態為 `Draft`。
- [SPEC-0006 Workflow Boundaries and Feedback](SPEC-0006-workflow-boundaries-and-feedback.md) — `FEAT-005` 的共同完成、取消、失敗與回饋邊界 Spec，狀態為 `Draft`。
- [SPEC-0007 Clipboard Handoff](SPEC-0007-clipboard-handoff.md) — `FEAT-003` 的 Capture Result 交付邊界 Spec，狀態為 `Draft`。
- [SPEC-0008 Capture Output](SPEC-0008-capture-output.md) — `FEAT-004` 的正式 Output lifecycle Spec，狀態為 `Draft`。
- [SPEC-0009 Annotation Capability](SPEC-0009-annotation-capability.md) — `FEAT-002` 的 optional Annotation capability Spec，狀態為 `Draft`。
- [SPEC-0010 Feature Integration](SPEC-0010-feature-integration.md) — 五個核心 Feature 的跨 Feature 整合 Spec，不是新的 Feature，狀態為 `Draft`。

PRD v1.0 已完成 Freeze Review；目前已建立規範、系統需求、Feature Catalog、五個核心 Feature Draft Spec 與跨 Feature Integration Draft，尚未建立 Annotation 子工具或其他實作 Spec。

## Spec 結構

功能 Spec 至少包含：

1. 目的與來源 PRD。
2. Scope 與 non-goals。
3. 使用者或外部觸發條件。
4. 正常流程。
5. 取消、失敗與恢復流程。
6. 可驗收條件。
7. 非功能需求與限制。
8. 尚未決定的問題。

## 狀態與編號

- 使用 `SPEC-NNNN-kebab-case.md`。
- `Draft` 不可作為開發承諾。
- `Proposal` 等待產品或技術決策。
- `Accepted` 可進入 implementation planning。
- `Superseded` 保留歷史並連到替代 Spec。

Spec 的驗收條件必須描述行為，不直接指定不必要的 class、function 或檔案名稱。
