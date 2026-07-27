# PRD

產品需求文件定義 SnipPlus 為誰解決什麼問題、第一版必須做到什麼，以及哪些能力明確延後。它不描述 class、API 或平台實作。

## Effective Product Baseline

目前有效的產品基線為 `2026-07-27` 經 Repository owner 明確確認的修正版：

- [PRD-0002 User Experience Principles](PRD-0002-user-experience-principles.md) — `Accepted`
- [PRD-0003 Product Vision](PRD-0003-product-vision.md) — `Accepted`
- [PRD-0004 Core Workflow](PRD-0004-core-workflow.md) — `Accepted v1.2`
- [PRD-0005 Functional Requirements](PRD-0005-functional-requirements.md) — `Accepted v1.2`
- [PRD-0006 Non-functional Requirements](PRD-0006-non-functional-requirements.md) — `Accepted v1.2`
- [PRD Freeze Review](PRD-FREEZE-REVIEW.md) — current acceptance record
- [Requirements-to-Code Conformance Matrix](PRD-TRACEABILITY-MATRIX.md) — `Reviewed v2.1`

Earlier v1.0／v1.1 discovery and review records remain historical evidence only. Where they conflict with accepted v1.2 documents, v1.2 is normative.

## Product Baseline Summary

- Manual startup and residency while the application is running.
- MainWindow `X` directly exits and releases PrintScreen takeover; no close-to-tray behavior.
- User-controlled PrintScreen takeover.
- All-display Frozen Virtual Desktop capture.
- Cross-monitor rectangular selection.
- Transparent final-image pixels for physical non-display gaps.
- Selection move、resize and reselection.
- Mandatory Editing／confirmation stage with optional Annotation actions.
- Required v1 Annotation tools defined by PRD-0005.
- Complete to Clipboard.
- Save As initially opens Downloads、writes PNG and Clipboard.
- PNG is retained when later Clipboard publication fails.
- Cancel、failure preservation、cleanup and focus restoration.

## Change Rules

- New product-visible behavior requires explicit Repository owner direction.
- Specs may clarify acceptance behavior but may not silently expand PRD scope.
- Existing code does not override accepted PRD.
- Unknown product decisions remain explicit and must not be guessed.
- Update existing canonical documents rather than creating another readiness or closure family.

## Historical Documents

- `PRD-0001 Product Foundation` is an early discovery artifact.
- `PRD-BASELINE-REVIEW.md` retains historical review context and is non-normative when conflicting with the current baseline.