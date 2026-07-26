# PRD Traceability Matrix

狀態：`Draft`

本文件只整理目前文件之間的追溯關係，不新增需求、不修改 Research、Analysis、Decision 或 PRD，也不建立任何 Specs。

## Matrix 1 — Research → Analysis → Decision → FR

| Research source | Analysis source | Decision source | Related FR | Coverage |
| --- | --- | --- | --- | --- |
| [Win11 capture workflow](../docs/Research/Win11/01-capture-workflow.md) — entry and capture modes | [Capture workflow analysis](../docs/Analysis/Win11/capture-workflow-analysis.md#workflow) | [Capture workflow decision](../docs/Decision/Win11/capture-workflow-decision.md#capture-entry-workflow) | `FR-001`, `FR-002`, `FR-003` | Direct workflow chain |
| [Win11 workflow state machine](../docs/Research/Win11/02-workflow-state-machine.md) — states and transitions | [State transition analysis](../docs/Analysis/Win11/capture-workflow-analysis.md#state-transition-analysis) | [Capture and region decisions](../docs/Decision/Win11/capture-workflow-decision.md#rectangle--freeform-selection) | `FR-002`, `FR-003`, `FR-009`, `FR-010` | Decision coverage; cancellation remains `UNKNOWN` |
| [Win11 capture completion](../docs/Research/Win11/01-capture-workflow.md#4-capture-completion) — clipboard and notification | [System intent and timeline](../docs/Analysis/Win11/capture-workflow-analysis.md#system-intent) | [Clipboard handoff](../docs/Decision/Win11/capture-workflow-decision.md#automatic-clipboard-handoff) | `FR-007`, `FR-008`, `FR-009` | Direct delivery chain |
| [Optional editor handoff](../docs/Research/Win11/01-capture-workflow.md#5-optional-editor-handoff) | [User and system intent](../docs/Analysis/Win11/capture-workflow-analysis.md#user-intent) | [Editor handoff](../docs/Decision/Win11/capture-workflow-decision.md#completion-notification-to-editor) | `FR-004`, `FR-005`, `FR-006` | Partial; editor details remain open |
| [Research unknowns](../docs/Research/Win11/01-capture-workflow.md#state-and-boundary-observations) | [Known unknowns](../docs/Analysis/Win11/capture-workflow-analysis.md#known-unknowns) | Decision records retain `UNKNOWN` | `FR-010`, `FR-011` | Gap remains; no invented behavior |
| [Video path](../docs/Research/Win11/01-capture-workflow.md#direct-keyboard-paths) | [Analysis scope boundary](../docs/Analysis/Win11/capture-workflow-analysis.md#prohibited-conclusions) | [Video capture path](../docs/Decision/Win11/capture-workflow-decision.md#video-capture-path) | None | Explicitly outside static image v1 scope |

## Matrix 2 — FR → NFR

| Functional Requirement | Related Non-functional Requirements | Relationship |
| --- | --- | --- |
| `FR-001` Start capture workflow | `NFR-001`, `NFR-004`, `NFR-007`, `NFR-012`, `NFR-013` | Responsiveness, familiar entry, Windows scope, explicit user action and lifecycle |
| `FR-002` Select capture region or scope | `NFR-001`, `NFR-004`, `NFR-006`, `NFR-007`, `NFR-013` | Responsive, usable, accessible and Windows-first selection boundary |
| `FR-003` Complete capture | `NFR-001`, `NFR-002`, `NFR-003`, `NFR-004`, `NFR-011`, `NFR-013` | Completion quality, safe interruption, work-context and privacy boundary |
| `FR-004` Create annotations | `NFR-005`, `NFR-006`, `NFR-010` | Optional advanced capability, accessibility direction and extensibility |
| `FR-005` Modify annotations | `NFR-005`, `NFR-006`, `NFR-010` | Optional advanced capability without breaking the primary workflow |
| `FR-006` Remove annotations | `NFR-005`, `NFR-006`, `NFR-010` | Optional advanced capability and reversible post-capture handling |
| `FR-007` Deliver result to clipboard | `NFR-001`, `NFR-003`, `NFR-011`, `NFR-013` | Responsive delivery, context protection, privacy and lifecycle |
| `FR-008` Produce screenshot result | `NFR-001`, `NFR-003`, `NFR-011` | Result quality, user context and explicit privacy boundary |
| `FR-009` Complete and exit workflow | `NFR-002`, `NFR-003`, `NFR-011`, `NFR-013` | Safe ending, context protection, privacy and lifecycle |
| `FR-010` Cancel before completion | `NFR-002`, `NFR-003`, `NFR-013` | Safe interruption and governed workflow control |
| `FR-011` Provide feedback when workflow cannot complete | `NFR-002`, `NFR-003`, `NFR-011`, `NFR-013` | Safe failure, context protection, privacy boundary and lifecycle |

## Matrix 3 — FR → Future Spec placeholders

目前只建立追溯 placeholder，不建立 `Specs/` 文件，也不預先決定 Spec 的實作內容。

| Functional Requirement | Future Spec placeholder | Placeholder status |
| --- | --- | --- |
| `FR-001`, `FR-002`, `FR-003` | Capture | Placeholder only; Spec not created |
| `FR-004`, `FR-005`, `FR-006` | Annotation | Placeholder only; tool details not defined |
| `FR-007` | Clipboard | Placeholder only; API and format not defined |
| `FR-008` | Output | Placeholder only; output format not defined |
| `FR-009`, `FR-010` | Workflow | Placeholder only; state behavior not specified |
| `FR-011` | Error Handling | Placeholder only; feedback and recovery not specified |

Future Spec placeholders are labels for traceability only. They are not approved filenames, implementation tasks or feature commitments.

## Matrix 4 — Decision Coverage

| Decision | Covered by FR | Covered by NFR | Coverage |
| --- | --- | --- | --- |
| Capture entry workflow — `YES` | `FR-001` | `NFR-004`, `NFR-007`, `NFR-012` | Covered |
| Rectangle / Freeform selection — `YES` | `FR-002` | `NFR-004`, `NFR-006`, `NFR-007` | Covered at capability level |
| Window / Full screen capture — `YES` | `FR-002`, `FR-003` | `NFR-001`, `NFR-007` | Covered at scope level; exact modes remain open |
| Automatic clipboard handoff — `YES` | `FR-007` | `NFR-001`, `NFR-011` | Covered |
| Completion notification to editor — `PARTIAL` | `FR-003`, `FR-009` | `NFR-005`, `NFR-010` | Partial; notification and editor details remain open |
| Post-capture toolbar / annotation stage — `PARTIAL` | `FR-004`, `FR-005`, `FR-006` | `NFR-005`, `NFR-006`, `NFR-010` | Partial; tool contents intentionally absent |
| Video capture path — `NO` | None | `NFR-013` scope governance only | Explicitly not covered by static image v1 |

## Matrix 5 — Gap Analysis

| Gap | Current evidence | Impact | Status |
| --- | --- | --- | --- |
| `FR-009` to `FR-011` lack direct Decision links in their requirement Source fields | They currently trace through PRD-0004 or Analysis | Decision coverage may require manual interpretation | Open |
| Several NFRs do not directly cite PRD-0002 UX Principles | `NFR-002`, `NFR-003`, `NFR-008`, `NFR-009`, `NFR-011`, `NFR-012`, `NFR-013` rely on other PRD or governance sources | NFR-to-UX trace is not complete as a direct mapping | Open |
| Governance documents appear in some NFR Dependencies | `NFR-009` and `NFR-013` reference `AGENTS.md` or Development Guide | Dependency field boundary may need a later review | Open |
| Runtime behavior is not verified | Research, Analysis and Decision retain `UNKNOWN` for PrtSc, cancellation, failure, recovery and platform edge cases | Product confidence may differ across Windows versions | Open |
| Future Specs do not exist | Matrix 3 contains placeholders only | No implementation contract is authorized yet | Expected at this stage |
| Some NFRs are not directly related to a functional capability | Maintainability and lifecycle constraints apply to the repository rather than a single FR | Future Specs may need separate governance mapping | Open |

## Current traceability conclusion

目前追溯矩陣已能呈現 Research、Analysis、Decision、FR、NFR 與未來 Spec placeholder 之間的關係，但 Gap Analysis 仍有開放項目。這份文件不替任何缺口做決策，也不把 `PARTIAL` 改寫成 `PASS`。

本矩陣完成後仍不得開始 Specs。是否進入 PRD Freeze，必須由後續 PRD Freeze Review 另行判定。
