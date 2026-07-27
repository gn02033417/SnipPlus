# ADR Baseline

## Document Control

| Field | Value |
| --- | --- |
| Document ID | `ADR-BASELINE` |
| Document type | ADR governance framework |
| Status | `Accepted` |
| Version | `1.0` |
| Last reviewed | `2026-07-27` |
| Normative references | Accepted Architecture baseline and Repository rules |

This file defines how Architecture Decision Records are governed. It is not itself a numbered technology decision.

## 1. When an ADR is required

Create or supersede an ADR for a durable、cross-boundary or hard-to-reverse decision such as:

- replacing the UI framework、rendering stack、capture backend、canonical image model、Clipboard API or test platform;
- introducing a new storage、distribution、update or external-service model;
- accepting a material compatibility、privacy、performance or deployment trade-off;
- changing a responsibility boundary that affects multiple modules or projects.

Do **not** create an ADR for:

- ordinary missing implementation;
- a focused bug fix within Accepted contracts;
- test additions;
- documentation status updates;
- temporary diagnostics;
- a failed build that can be corrected without changing a durable decision.

## 2. ADR lifecycle

```text
Draft
→ Review
→ Accepted
→ Superseded or Deprecated
```

| Status | Meaning |
| --- | --- |
| `Draft` | Decision is being written and is not effective. |
| `Review` | Options and trade-offs are ready for authority review. |
| `Accepted` | Effective architecture or technology decision. |
| `Superseded` | Replaced by a later Accepted ADR; retain history and link replacement. |
| `Deprecated` | No longer applicable without a direct replacement. |

Only Accepted ADRs are normative.

## 3. ID and file rules

- Use `ADR-NNNN-short-decision-name.md`.
- IDs are never reused.
- One ADR addresses one primary decision.
- Superseded and Deprecated files remain in the repository.
- Numbered ADRs live under `Architecture/adr/`.
- This governance framework does not consume a numbered ADR ID.

## 4. Required ADR sections

Every ADR must include:

1. Document control and status.
2. Context and decision drivers.
3. Options considered.
4. Decision.
5. Consequences and trade-offs.
6. Traceability to accepted PRD／Specs／Architecture or verified findings.
7. Verification expectations.
8. Supersession or reconsideration conditions.

## 5. Decision authority and traceability

- Product-visible behavior remains owned by accepted PRD／Specs and cannot be changed by an ADR alone.
- Architecture responsibility changes must be reflected in the owning Architecture documents.
- Implementation follows only Accepted ADRs.
- Existing code and runtime findings may trigger an ADR review but do not silently change an Accepted decision.
- The current accepted ADR index is `Architecture/adr/README.md`.

## 6. Current boundary

ADR-0002 through ADR-0007 remain effective for the accepted SnipPlus v1 conformance correction. No additional ADR is currently required before the first implementation step.

A verified future conflict should create one targeted ADR. It must not restart a repetitive Research／readiness／authorization／closure document chain.
