# Technology Research

狀態：`Accepted`

This directory contains Technology Feasibility Research, evidence plans, readiness records and governance history. It does not contain Accepted technical decisions, production prototypes or source code.

## Research lines

### UI Framework — 01 through 09

| Range | Coverage | Current boundary |
| --- | --- | --- |
| 01 | Framework feasibility | WinUI 3、WPF、Avalonia、Windows Forms evidence comparison |
| 02–05 | Runtime spike planning and prerequisite closure | Plans only; no authorized runtime spike |
| 06–09 | Evidence, readiness, enablement and authorization request | Authorization not granted; execution not permitted |

Primary decision destination: [ADR-0002 UI Framework Selection](../../../Architecture/adr/ADR-0002-ui-framework-selection.md), currently `Draft` and not accepted.

### Rendering Technology — 10 through 18

| Range | Coverage | Current boundary |
| --- | --- | --- |
| 10 | Rendering feasibility | Research evidence only |
| 11–14 | Runtime spike and prerequisite planning | No implementation authorization |
| 15–18 | Evidence baseline, reassessment and inspection request | No runtime verification or Accepted ADR |

Primary decision destination: future Rendering Technology ADR.

### Capture Backend — 20 through 28

| Range | Coverage | Current boundary |
| --- | --- | --- |
| 20 | Capture backend feasibility | Research evidence only |
| 21–24 | Runtime spike and prerequisite planning | No implementation authorization |
| 25–28 | Evidence baseline, reassessment and inspection request | No runtime verification or Accepted ADR |

Primary decision destination: future Capture Backend ADR.

### Clipboard Integration — 29 through 80

| Range | Coverage | Current boundary |
| --- | --- | --- |
| 29–35 | Feasibility, runtime planning, prerequisite evidence and reassessment | No clipboard implementation or inspection |
| 36–47 | Authorization-readiness, inspection planning and evidence consolidation | Documentary evidence only |
| 48–55 | D1–D6 packages and evidence-specific readiness | No operational authorization |
| 56–62 | D1 inspection requests, submission readiness and authority/channel governance | Requests not submitted; authority not identified |
| 63–70 | Human-governance worksheet and input-collection request chain | Human input not collected |
| 71–80 | Role/privacy/drafting authorization and artifact-creation controls | Documentary closure reached; artifact creation and execution not authorized |

The final chain `039 → 040 → … → 052` ends in document 80. It preserves:

- Artifact Creation Permission: `No`
- Drafting Authorization Artifact: `Not created`
- Drafting Start Permission: `No`
- Collection Authorization: `Not provided`
- Execution Permission: `Not provided`

This chain is closed. Do not create document 81 or another same-pattern prerequisite／readiness／authorization／closure document unless new evidence, an explicit human decision, an accepted upstream change or runtime evidence materially changes the state.

Primary decision destination: future Clipboard Integration ADR and consolidated Clipboard contract.

## Indexing rule

The numbered files are retained as historical evidence. They are grouped by research line here instead of repeating every long title. File numbering gaps are intentional and must not be reused.

Use repository search or the directory listing for an exact numbered file. Each file remains the authoritative source for its own Document ID, status and boundary.

## Boundary

- Research does not become an Accepted technical decision by accumulation.
- Runtime execution requires an explicit task and an approved verification boundary.
- Runtime results must be stored separately from plans.
- Accepted choices belong in `Architecture/adr/`.
- Closely related engineering questions should be consolidated into ADR or contract packages rather than extended through repeated closure-review layers.
- This directory must not become a project, prototype or source-code directory.
