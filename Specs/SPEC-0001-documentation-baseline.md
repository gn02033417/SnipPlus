# SPEC-0001 Documentation Baseline

狀態：`Accepted`

## 1. Purpose

Define the minimum maintainable documentation baseline for the SnipPlus Repository throughout product development. This Spec governs document ownership and navigation; it does not define screenshot product behavior.

## 2. Scope

This Spec covers:

- Repository entry points;
- PRD、Specs、Architecture and ADR responsibility boundaries;
- implementation and conformance evidence navigation;
- guides、Roadmap、Changelog and TODO responsibilities;
- explicit treatment of historical and superseded documents;
- documentation anti-proliferation rules.

## 3. Requirements

### DOC-001 — Clear entry points

`README.md` and `docs/index.md` must lead to the current accepted product、behavior、Architecture、contracts、conformance and development sources.

### DOC-002 — Responsibility separation

- Product intent and scope belong in `PRD/`.
- Observable behavior and acceptance criteria belong in `Specs/`.
- Responsibility and dependency boundaries belong in `Architecture/`.
- Durable technology decisions belong in `Architecture/adr/`.
- Implementation status belongs in code、tests、`CHANGELOG.md` and the existing conformance matrix.

### DOC-003 — Current source hierarchy

Accepted PRD／Specs／Architecture and ADRs are normative. Existing code and historical Research are evidence only and do not override accepted behavior.

### DOC-004 — Unknown behavior remains explicit

Unresolved user-visible behavior must be identified as an Open Decision. Implementation must not silently turn an unknown into a product rule.

### DOC-005 — Historical documents are labeled

A historical or superseded document must state that status and link to its current replacement. It must not appear in the normal implementation reading order.

### DOC-006 — Naming and navigation remain consistent

Markdown files follow [Markdown naming rules](../docs/standards/markdown-naming.md), use one H1 and update the relevant index when their role changes.

### DOC-007 — Changes remain traceable

- User／maintainer-visible changes update `CHANGELOG.md`.
- Current implementation work updates `PRD/PRD-TRACEABILITY-MATRIX.md` when evidence exists.
- Remaining implementation tasks update `TODO.md` or the owning Open Decision.

### DOC-008 — Documentation does not become a substitute for implementation

Missing code、failed tests or incomplete verification do not justify creating repeated prerequisite、readiness、authorization、reassessment or closure documents.

### DOC-009 — Execution claims require evidence

Documentation must distinguish:

- not run;
- build／test verified;
- runtime verified;
- historical evidence;
- accepted product conformance.

A passing build or test count is not sufficient to claim user-visible conformance.

### DOC-010 — Privacy applies to evidence

Real desktop screenshots、Clipboard payloads、private window titles and machine identifiers are not committed as routine evidence.

## 4. Acceptance Checklist

- `README.md` and `docs/index.md` show the accepted v1 source order.
- PRD、Specs、Architecture、ADRs、contracts and conformance matrix have clear entry points.
- Historical reviews are labeled and do not override current sources.
- Roadmap and TODO reflect the current v1 conformance correction rather than the old single-display slice.
- Documentation states that source code and tests already exist while product conformance remains incomplete.
- No current canonical document routes mouse release directly to Clipboard or treats multi-display／Annotation／PNG Save as deferred v1 scope.
- No new repetitive readiness or closure chain is required before the next explicit implementation task.
