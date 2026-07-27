# SPEC-0002 Specification Guidelines

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `SPEC-0002` |
| Version | `1.0` |
| Status | `Accepted` |
| Last reviewed | `2026-07-27` |
| Scope | All normative files under `Specs/` |

## 2. Purpose

Specifications translate accepted PRD into observable、testable system behavior. They are the contract shared by product review、implementation and verification.

A Spec does not replace PRD、Architecture or ADRs and does not treat current code as the source of truth.

## 3. Required content

A normative Feature or integration Spec must include, as applicable:

- Document Control.
- Purpose and Scope.
- Normative PRD／FR／NFR sources.
- Preconditions and entry boundary.
- Normal workflow or behavior rules.
- State and lifecycle behavior.
- Cancellation、failure and recovery behavior.
- Edge cases and invalid input behavior.
- Cross-feature ownership or handoff rules.
- Acceptance Criteria with stable IDs.
- Explicit Open Decisions that implementation must not guess.

Not every Spec needs identical headings, but it must contain enough information to implement and verify its owning behavior without relying on historical Research or implicit assumptions.

## 4. Traceability rules

- Every product capability traces to accepted `FR-` or `NFR-` IDs.
- Shared system behavior traces to `SPEC-0003`.
- Feature ownership traces to `SPEC-0004` and the owning Feature Spec.
- Cross-feature sequencing traces to `SPEC-0010`.
- Architecture and tests must reference the owning Spec or Acceptance Criterion.
- A Spec may clarify acceptance behavior but may not silently add a new product capability.

## 5. Status model

| Status | Meaning |
| --- | --- |
| `Draft` | Incomplete and not implementation authority. |
| `Review` | Ready for product／engineering／test review. |
| `Accepted` | Normative current behavior contract. |
| `Superseded` | Replaced by a later Accepted document or revision. |
| `Deprecated` | Retained for history but no longer applicable. |

Only Accepted Specs are normative.

## 6. Acceptance Criteria rules

Acceptance Criteria must:

- use a stable document namespace such as `SPEC-0005-AC-001`;
- describe observable behavior or externally verifiable invariants;
- separate success、cancel、recoverable failure and terminal failure where relevant;
- state when no output or side effect is allowed;
- include identity、revision、cleanup and privacy boundaries when relevant;
- avoid requiring a specific class、method or private implementation unless the requirement is an accepted architecture contract;
- be specific enough that a passing build alone cannot be mistaken for product conformance.

## 7. Unknown and open behavior

Use an explicit Open Decision when multiple user-visible outcomes remain possible.

Implementation must stop before choosing an unresolved behavior. It must not convert `TBD` into code merely because one option is convenient.

Current explicit product decisions are listed in accepted PRD／Specs and the conformance matrix.

## 8. Architecture boundary

Specs define behavior, not technology. They may require:

- one stable session;
- immutable frame ownership;
- state authority;
- output identity;
- cleanup or failure semantics.

They do not choose WinUI、WGC、Win2D、SoftwareBitmap、DataPackage、MSTest or project layout; those choices belong to Accepted Architecture／ADRs／Project Structure.

## 9. Change rules

- Product-visible scope change → update PRD first.
- Acceptance clarification inside accepted scope → update the owning Spec.
- Responsibility or technology change → update Architecture／ADR after Spec remains consistent.
- Implementation evidence → update code、tests、CHANGELOG and the existing conformance matrix.
- Do not create duplicate readiness、authorization or closure Specs because implementation is incomplete.

## 10. Current baseline

The current Accepted Specification set is `SPEC-0003` through `SPEC-0010`, as recorded by `SPEC-BASELINE-REVIEW.md` and indexed by `Specs/README.md`.
