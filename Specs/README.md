# Specs

Specifications 定義可觀察、可驗收的行為。它們必須能被產品、設計、開發與測試共同理解，不把尚未核准的推測寫成實作契約。

## Specification v1.0 baseline

Specification v1.0 已完成 Baseline Review，正式 baseline 為：

- [SPEC-0002 Specification Guidelines](SPEC-0002-specification-guidelines.md)
- [SPEC-0003 System Requirements](SPEC-0003-system-requirements.md)
- [SPEC-0004 Feature Catalog](SPEC-0004-feature-catalog.md)
- [SPEC-0005 Capture Workflow](SPEC-0005-capture-workflow.md)
- [SPEC-0006 Workflow Boundaries and Feedback](SPEC-0006-workflow-boundaries-and-feedback.md)
- [SPEC-0007 Clipboard Handoff](SPEC-0007-clipboard-handoff.md)
- [SPEC-0008 Capture Output](SPEC-0008-capture-output.md)
- [SPEC-0009 Annotation Capability](SPEC-0009-annotation-capability.md)
- [SPEC-0010 Feature Integration](SPEC-0010-feature-integration.md)
- [Specification Baseline Review](SPEC-BASELINE-REVIEW.md)

Freeze Decision：`Freeze Approved`。Readiness：`Ready for Architecture`。

`SPEC-0001 Documentation Baseline` 是已 `Accepted` 的文件治理 Spec，不屬於 v1.0 Feature Specification Freeze 範圍。

## Current boundary

- 五個核心 Feature、shared state、workflow boundary、parallel downstream 與 acceptance criteria 已建立 baseline。
- Runtime behavior、Windows platform details、DPI／HDR、failure retry、Clipboard consumer behavior、Output delivery details 與 Annotation tool model 仍保留 `UNKNOWN/TBD`。
- Freeze Approved 不表示各文件已成為 implementation approval，也不表示技術棧、API、project structure、build 或 test 已完成。

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

## 狀態與變更

- `Draft` 不可單獨作為開發承諾。
- `Accepted` 表示單一 Spec 已核准。
- `Freeze Approved` 表示 baseline 集合可由 Architecture 引用。
- `Superseded` 保留歷史並連到替代 Spec。

Specification Freeze 後，Feature、state、boundary 或 acceptance criteria 的變更必須走 Spec Change Request，不得由 Architecture、ADR 或 Implementation 偷偷補入。
