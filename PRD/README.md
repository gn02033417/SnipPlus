# PRD

產品需求文件定義 SnipPlus 為誰解決什麼問題、第一版必須做到什麼，以及哪些能力明確延後。它不描述 class、API 或平台實作。

## Effective Product Baseline

目前有效的產品基線為 `2026-07-27` 經 Repository owner 明確確認的完整 v1 基線：

- [PRD-0002 User Experience Principles](PRD-0002-user-experience-principles.md) — `Accepted`
- [PRD-0003 Product Vision](PRD-0003-product-vision.md) — `Accepted`
- [PRD-0004 Core Workflow](PRD-0004-core-workflow.md) — `Accepted`
- [PRD-0005 Functional Requirements](PRD-0005-functional-requirements.md) — `Accepted`
- [PRD-0006 Non-functional Requirements](PRD-0006-non-functional-requirements.md) — `Accepted v1.3`
- [PRD Freeze Review](PRD-FREEZE-REVIEW.md) — `Accepted v1.3`
- [Requirements-to-Code Conformance Matrix](PRD-TRACEABILITY-MATRIX.md) — `Reviewed v2.2`

Earlier discovery and review records remain historical evidence only. Where they conflict with the accepted current documents, the current documents are normative.

## Product Baseline Summary

- Manual startup and residency while the application is running.
- MainWindow `X` directly exits and releases PrintScreen takeover; no close-to-tray behavior.
- User-controlled PrintScreen takeover.
- Capacity validation before Selection.
- All-display Frozen Virtual Desktop capture.
- Cross-monitor rectangular Selection.
- Transparent final-image pixels for physical non-display gaps.
- Selection move、resize and reselection.
- Mandatory Editing／confirmation stage with optional Annotation actions.
- Required v1 Annotation tools and complete keyboard-only operation from `SelectionLocked`.
- Complete to Clipboard.
- Save As initially opens Downloads、writes PNG and Clipboard.
- PNG is retained when later Clipboard publication fails.
- Cancel、progress、failure preservation、cleanup and focus restoration.

## Accepted Quality Baseline

### Performance

- Capture start p95 `≤ 500 ms` Standard、`≤ 1,000 ms` Maximum.
- Interaction frame p95 `≤ 33 ms`; visible response p95 `≤ 100 ms`.
- Complete and Save use size-tiered latency targets.
- Commit progress appears after `300 ms`.
- Idle、peak、cleanup and repeated-session memory limits are fixed in PRD-0006.

### Capacity

- `1`–`4` logical displays.
- Each display `≤ 7,680 × 4,320`.
- Total source pixels `≤ 66,355,200`.
- Virtual Desktop width／height each `≤ 16,384`.
- Final Selection area `≤ 67,108,864` pixels with dimensional limits.

### Keyboard-only Editing

- Scope begins at `SelectionLocked`.
- Required tools、object operations、styles、Undo／Redo、Save、Complete and Cancel are available without pointer input.
- Initial crosshair Selection remains pointer-driven in v1.

## Change Rules

- New product-visible behavior requires explicit Repository owner direction.
- Specs may clarify acceptance behavior but may not silently expand or relax PRD scope.
- Existing code does not override accepted PRD.
- No current v1 product-quality decision remains open.
- Update existing canonical documents rather than creating another readiness or closure family.

## Historical Documents

- `PRD-0001 Product Foundation` is an early discovery artifact.
- `PRD-BASELINE-REVIEW.md` retains historical review context and is non-normative when conflicting with the current baseline.