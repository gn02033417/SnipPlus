# PRD

產品需求文件定義 SnipPlus 為誰解決什麼問題、第一版必須做到什麼，以及哪些能力明確延後。它不描述 class、API 或平台實作。

## Effective Product Baseline

目前有效的產品基線為 `2026-07-27` 經 Repository owner 明確確認的修正版：

- [PRD-0002 User Experience Principles](PRD-0002-user-experience-principles.md)
- [PRD-0003 Product Vision](PRD-0003-product-vision.md)
- [PRD-0004 Core Workflow](PRD-0004-core-workflow.md) — `Accepted v1.1`
- [PRD-0005 Functional Requirements](PRD-0005-functional-requirements.md) — `Accepted v1.1`
- [PRD-0006 Non-functional Requirements](PRD-0006-non-functional-requirements.md) — `Accepted v1.1`

The earlier v1.0 Freeze Review、Traceability Matrix and Freeze artifacts remain historical review records. Where they conflict with accepted v1.1 documents, v1.1 is normative.

## Product Baseline Summary

- Manual startup and background residency.
- User-controlled PrintScreen takeover.
- All-display frozen Virtual Desktop capture.
- Cross-monitor rectangular selection.
- Selection move、resize and reselection.
- Mandatory editing／confirmation stage with optional annotation actions.
- Required annotation tools defined by PRD-0005.
- Complete to Clipboard.
- Save to PNG and Clipboard.
- Cancel、failure preservation、cleanup and focus restoration.

## Change Rules

- New product-visible behavior requires explicit Repository owner direction.
- Specs may clarify acceptance behavior but may not silently expand PRD scope.
- Existing code does not override accepted PRD.
- Unknown product decisions must remain explicit rather than being guessed.

## Historical Documents

- `PRD-0001 Product Foundation` remains an early discovery artifact.
- `PRD-BASELINE-REVIEW.md`、`PRD-TRACEABILITY-MATRIX.md` and `PRD-FREEZE-REVIEW.md` are historical until the traceability matrix is updated against v1.1.
