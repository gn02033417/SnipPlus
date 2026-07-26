# Technology Decision Roadmap

## Document Control

| Field | Value |
| --- | --- |
| Document ID | TECHNOLOGY-DECISION-ROADMAP |
| Title | Technology Decision Roadmap |
| Document Type | Decision Backlog and Ordering Roadmap |
| Status | Accepted |
| Architecture Stability | Draft |
| Version | 1.1 |
| Owner | Repository owner |
| Last reviewed | 2026-07-26 |
| Normative References | ARCH-0001、ARCH-0002、ARCH-0003、ARCH-0004、ARCH-0005、ARCH-BASELINE-REVIEW、ADR-BASELINE |
| Informative References | PRD-FREEZE-REVIEW、SPEC-BASELINE-REVIEW、ROADMAP.md、TODO.md、REPOSITORY-READINESS-AUDIT-001 |

本文件管理重大技術決策的 backlog、依賴與順序。真正有效的選擇只存在於 `Accepted` ADR。

## 1. Purpose

本 Roadmap 回答：

> SnipPlus 哪些重大技術決策已完成、哪些仍阻擋 implementation readiness，以及下一個最有價值的決策是什麼？

原則：

- 依 Frozen Architecture 排序決策。
- 讓 Accepted upstream decision 解鎖下游主題。
- 保留 Candidate、UNKNOWN、TBD 與 runtime verification gap。
- 不以 prerequisite／closure 文件取代真正 ADR。
- 未完成必要決策、contracts 與 Project Structure 前，不開始 Coding。

## 2. Decision Lifecycle

```text
Candidate
  -> Ready
  -> Draft ADR
  -> Review
  -> Accepted
```

| Status | Meaning | Allowed action |
| --- | --- | --- |
| Candidate | 已知需要決策，但 context、evidence 或 upstream dependency 尚未完成。 | 補齊 context、drivers、options、risks 與 dependencies。 |
| Ready | 建立 ADR 的必要來源已足夠。 | 建立一份只處理單一主題的 ADR。 |
| Draft ADR | 真正 ADR 已存在但尚未通過 Review。 | 補齊內容並提交 Review。 |
| Review | Reviewer 正在確認 decision、trade-offs、evidence 與 boundaries。 | 解決 comments；不得當成有效基線。 |
| Accepted | ADR 已獲 acceptance authority 接受。 | 下游 ADR、contracts、Project Structure 與 implementation planning 可以引用。 |
| Deferred | 題目仍有效，但不屬於目前 vertical slice 的必要前置。 | 保留原因與重新啟動條件。 |
| Rejected | 候選方向被拒絕。 | 保留理由；必要時建立新候選或 ADR。 |

## 3. Decision Backlog

| Decision ID | Topic | Priority | Depends On | Status | Effective artifact |
| --- | --- | --- | --- | --- | --- |
| TD-001 | UI Framework | P0 | Architecture Freeze、ADR Framework | Accepted | ADR-0002 — WinUI 3 |
| TD-002 | Rendering Technology | P0 | TD-001、rendering boundaries | Accepted | ADR-0003 — WinUI Composition + Win2D |
| TD-003 | Capture Backend | P0 | TD-001、TD-002、MOD-008 boundary | Candidate | None |
| TD-004 | Clipboard Integration | P0 | COMP-009、COMP-015、TD-001 | Candidate | None |
| TD-005 | Image Representation | P0 | TD-003、TD-004、COMP-006 boundary | Candidate | None |
| TD-006 | Plugin Architecture | P2 | TD-001、Feature／Module stability | Candidate | None |
| TD-007 | Configuration | P1 | TD-001、ADR-BASELINE、runtime boundary | Candidate | None |
| TD-008 | Logging | P1 | TD-001、TD-007、failure ownership | Candidate | None |
| TD-009 | Telemetry | P2 | TD-008、Security and Privacy review | Candidate | None |
| TD-010 | Packaging | P1 | TD-001、TD-007、TD-011 | Candidate | None |
| TD-011 | Testing Strategy | P0 | TD-001、TD-002、Architecture boundaries | Candidate | None |
| TD-012 | Update Strategy | P2 | TD-010、TD-011、Deployment and Operations scope | Candidate | None |

Priority：

| Priority | Meaning |
| --- | --- |
| P0 | 影響核心 workflow 或多個下游決策；implementation readiness 前必須 Accepted 或明確 Deferred。 |
| P1 | 影響 maintainability、verification、delivery 或 operations。 |
| P2 | 可在核心 vertical slice 穩定後處理，不得提前成為必要依賴。 |

Decision ID 永不重用。

## 4. Completed Decisions

### TD-001 UI Framework

- Status：`Accepted`
- Effective ADR：[ADR-0002 UI Framework Selection](adr/ADR-0002-ui-framework-selection.md)
- Accepted option：WinUI 3
- Acceptance date：2026-07-26
- Coding authorized：No

### TD-002 Rendering Technology

- Status：`Accepted`
- Effective ADR：[ADR-0003 Rendering Technology](adr/ADR-0003-rendering-technology.md)
- Accepted architecture：WinUI 3 XAML／`Microsoft.UI.Composition` retained visual layer plus Win2D immediate-mode drawing、bitmap、effect and offscreen-raster path behind an abstract rendering adapter
- Acceptance date：2026-07-26
- Runtime verification：Not performed
- Coding authorized：No

ADR-0003 does not select Language、Runtime、Windows App SDK version、Win2D package version、Capture Backend、Clipboard API、Image Representation、Packaging、Testing framework or Project Structure.

## 5. Next Decision

**TD-003 Capture Backend** is the next primary P0 decision.

Reasons：

- TD-001 and TD-002 are Accepted.
- Capture defines how a platform result reaches the Shared Result／rendering boundary.
- The first vertical slice cannot acquire a real capture result until the Capture Backend boundary is selected.
- Existing `docs/Research/Technology/20–28` must be consolidated directly into one Capture Backend ADR; no new readiness or closure chain is required.

### Parallel preparation

- TD-005 Image Representation should be developed in close coordination with TD-003 and the Shared Result contract.
- TD-004 Clipboard Integration may define ownership、lifetime、failure and retry boundaries without assuming a final payload type.
- TD-011 Testing Strategy may define coverage categories now, but concrete runtime matrices depend on TD-003 and TD-005.

## 6. Recommended Decision Order

### Phase 1 — Core platform and workflow foundation

1. ~~TD-001 UI Framework~~ — **Accepted through ADR-0002**.
2. ~~TD-002 Rendering Technology~~ — **Accepted through ADR-0003**.
3. TD-003 Capture Backend.
4. TD-005 Image Representation.
5. TD-004 Clipboard Integration.
6. TD-011 Testing Strategy.

TD-003、TD-004 and TD-005 may be analysed together, but each ADR／contract must keep capture acquisition、in-memory representation and clipboard delivery as separate responsibilities.

### Phase 2 — Maintainability and delivery foundation

1. TD-007 Configuration.
2. TD-008 Logging.
3. TD-010 Packaging.
4. TD-012 Update Strategy.

### Phase 3 — Deferred extensibility and operations

1. TD-006 Plugin Architecture.
2. TD-009 Telemetry.

## 7. Candidate → Ready Criteria

A Candidate can become Ready only when：

- Decision Context is explicit.
- An Architecture requirement、finding or unresolved question is cited.
- Frozen PRD／Spec sources are cited where behavior is affected.
- Upstream dependencies are Accepted or the dependency risk is explicitly retained.
- Decision Drivers and at least two reasonable directions are available.
- Non-goals、risk、reversibility、migration and rollback impact are recorded.
- Runtime evidence is marked `Verified`、`Not verified` or `Not required`.
- The decision does not silently add product requirements or change Architecture ownership.

## 8. ADR Acceptance Requirements

An ADR can become Accepted only when：

- ADR-BASELINE required sections are present.
- It handles one major decision.
- Reviewer、review date、result and acceptance authority are recorded.
- Options、trade-offs and positive／negative／neutral consequences are complete.
- Critical UNKNOWN／TBD values are explained or moved to explicit follow-up decisions.
- Supersession is clear.
- Frozen PRD、Specs and Architecture ownership remain unchanged.
- The corresponding Roadmap row is updated.

Runtime verification may remain pending when the ADR selects a bounded architecture rather than claiming product runtime success. Pending verification must become an explicit implementation-readiness requirement.

## 9. Freeze Boundary

Technology decisions must not directly rewrite：

- Frozen PRD goals、scope、FR、NFR or UX Principles.
- Frozen Specs features、states、acceptance criteria or behavior boundaries.
- ARCH-0002 dependency direction.
- ARCH-0003 Module ownership.
- ARCH-0004 Component ownership and Shared State access policy.
- ARCH-0005 interaction ownership and Clipboard／Output parallel boundary.
- COMP-001 as the sole Shared State Authority.
- Annotation as optional.

## 10. Anti-proliferation Rule

A `Not ready` conclusion does not automatically create another prerequisite、reassessment、authorization-request or closure-review document.

New documents require materially new evidence、a human decision、an Accepted upstream change、runtime evidence or a different decision／contract boundary.

Existing Research must be consolidated into ADR Context、Options、Evidence、Trade-offs and Consequences.

## 11. Open Questions

- Which Capture Backend and capture API boundary should TD-003 select?
- What in-memory image representation、pixel format、alpha、color-space and lifetime should TD-005 select?
- What clipboard payload、consumer compatibility、retry and ownership boundary should TD-004 select?
- What unit、integration、platform、visual and runtime verification depth should TD-011 require?
- Which C#／.NET／Windows App SDK／Win2D versions should Project Structure fix?
- Which packaged／unpackaged and framework-dependent／self-contained strategy should TD-010 select?

## 12. Current Outcome

`TD-001 and TD-002 Accepted; capture、image、clipboard、testing、contracts and project structure remain implementation blockers.`

Shortest remaining path：

1. Accept TD-003 Capture Backend.
2. Accept TD-005 Image Representation and TD-004 Clipboard Integration.
3. Accept TD-011 Testing Strategy.
4. Define consolidated Shared Result、rendering、capture、clipboard、output and failure contracts.
5. Define Language／Runtime／SDK versions and Solution／Project Structure.
6. Perform one Implementation Readiness Review.

This Roadmap does not authorize Coding、Restore、Build、Test、Package or Runtime execution.
