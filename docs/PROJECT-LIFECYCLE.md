# SnipPlus Repository Project Lifecycle

## Document Control

| Field | Value |
| --- | --- |
| Document ID | `PROJECT-LIFECYCLE` |
| Status | `Accepted` |
| Version | `1.0` |
| Last reviewed | `2026-07-27` |
| Normative references | `AGENTS.md`、current PRD／Specs／Architecture baselines、existing conformance matrix |

## 1. Purpose

This document explains which existing artifact owns each type of change. It does **not** require every implementation task to repeat Research、Analysis、Decision、Freeze、Readiness and Closure phases.

## 2. Current repository position

```text
Accepted Product baseline
→ Accepted Specification baseline
→ Accepted Architecture／ADR／Contracts
→ Existing implementation prototype
→ Requirements-to-code conformance correction
→ Verification
→ Release later
```

The repository is currently in **implementation conformance correction**. Additional prerequisite documentation is not required before each coding slice.

## 3. Change ownership

| Change type | Update this source first | Typical evidence |
| --- | --- | --- |
| User-visible product behavior or v1 scope | Existing PRD | Explicit Repository owner decision |
| Observable acceptance behavior | Owning Spec | Acceptance criteria and traceability |
| Layer、module、component or dependency ownership | Existing Architecture document | Responsibility and impact review |
| Durable technology choice | Existing or superseding ADR | Verified trade-off or incompatibility |
| Cross-project data／lifecycle contract | `IMPLEMENTATION-CONTRACTS.md` | Contract tests and integration findings |
| Source behavior inside accepted boundaries | Source and tests | Build、tests and applicable runtime evidence |
| Current implementation conformance | `PRD-TRACEABILITY-MATRIX.md` | Code、test and runtime references |
| User／maintainer-visible change history | `CHANGELOG.md` | Actual completed change |
| Release scope and publication | Release process documents when authorized | Signed／published artifacts and release verification |

## 4. Normal implementation loop

```text
Select first unresolved conformance row
→ read owning PRD／Spec／Architecture
→ inspect minimal current code and tests
→ implement focused slice
→ add or update tests
→ run only authorized verification
→ update CHANGELOG and the existing conformance row
→ stop before the next slice
```

Normal implementation work does not create a new readiness、authorization、reassessment or closure document.

## 5. When Research and Analysis are required

Use Research／Analysis only when:

- a current public platform fact must be verified;
- official Windows／SDK behavior is unclear;
- a verified runtime conflict has more than one viable design response;
- the Repository owner requests competitive or product research;
- a new product-visible capability is being considered.

Research is evidence. It does not override Accepted PRD、Specs、Architecture or ADRs by itself.

## 6. When to update PRD or Specs

Update existing PRD／Specs only when:

- the Repository owner changes product behavior or scope;
- an existing requirement is ambiguous enough to produce different user-visible outcomes;
- an acceptance criterion is missing or contradictory;
- a verified implementation finding proves the accepted behavior is impossible or materially unsafe.

Do not modify PRD／Specs merely to make current code appear conforming.

## 7. When to update Architecture or ADRs

Update Architecture when responsibility、dependency or lifecycle ownership changes.

Create or supersede an ADR only for a durable decision such as:

- replacing an Accepted framework or platform API;
- introducing a new storage、distribution or update model;
- adding a cross-cutting runtime dependency;
- accepting a hard-to-reverse compatibility or performance trade-off.

A normal bug fix or missing v1 implementation does not require a new ADR.

## 8. Verification

Verification must match the behavior being claimed:

- Unit／Contract tests for platform-neutral rules.
- Rendering and adapter tests for deterministic platform boundaries.
- Interactive Windows verification only when explicitly authorized.
- Multi-display、PrintScreen and focus behavior require applicable runtime evidence before rows become `Conforms`.

Build success or test counts alone do not prove user-visible conformance.

## 9. Privacy and evidence

- Do not persist real desktop screenshots or Clipboard payloads as repository evidence.
- Redact private window titles、paths and machine identifiers.
- Normal development and non-interactive tests do not launch external GUI fixtures.
- Historical evidence remains historical and is not rewritten as proof of the current product workflow.

## 10. Release boundary

The project is not released. Release publication、Store deployment、signing、distribution and update strategy remain outside the current conformance-correction phase and require explicit authorization.
