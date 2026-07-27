# PRD

產品需求文件定義 SnipPlus 為誰解決什麼問題、第一版必須做到什麼，以及哪些能力明確延後。它不描述 class、API 或平台實作。

## Effective Product Baseline

目前有效的產品基線為 `2026-07-27` 經 Repository owner 明確確認的修正版：

- [PRD-0002 User Experience Principles](PRD-0002-user-experience-principles.md)
- [PRD-0003 Product Vision](PRD-0003-product-vision.md)
- [PRD-0004 Core Workflow](PRD-0004-core-workflow.md) — `Accepted v1.1`
- [PRD-0005 Functional Requirements](PRD-0005-functional-requirements.md) — `Accepted v1.1`
- [PRD-0006 Non-functional Requirements](PRD-0006-non-functional-requirements.md) — `Accepted v1.1`
- [PRD Freeze Review](PRD-FREEZE-REVIEW.md) — current v1.1 acceptance record
- [Requirements-to-Code Conformance Matrix](PRD-TRACEABILITY-MATRIX.md) — current code／test gap authority

Earlier v1.0 discovery、baseline and freeze content remains historical evidence only. Where it conflicts with the accepted v1.1 documents above, v1.1 is normative.

## Product Baseline Summary

- Manual startup and background residency.
- User-controlled PrintScreen takeover.
- All-display frozen Virtual Desktop capture.
- Cross-monitor rectangular selection.
- Selection move、resize and reselection.
- Mandatory editing／confirmation stage with optional annotation actions.
- Required v1 annotation tools defined by PRD-0005.
- Complete to Clipboard.
- Save to PNG and Clipboard.
- Cancel、failure preservation、cleanup and focus restoration.

## Change Rules

- New product-visible behavior requires explicit Repository owner direction.
- Specs may clarify acceptance behavior but may not silently expand PRD scope.
- Existing code does not override accepted PRD.
- Unknown product decisions remain explicit and must not be guessed.
- Update existing canonical documents rather than creating another readiness or closure family.

## Historical Documents

- `PRD-0001 Product Foundation` is an early discovery artifact.
- `PRD-BASELINE-REVIEW.md` records the earlier v1.0 document review and is non-normative when conflicting with the current baseline.
