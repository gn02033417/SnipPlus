# ARCH-0001 Architecture Principles

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `ARCH-0001` |
| Version | `1.0` |
| Architecture stability | `Accepted` |
| Last reviewed | `2026-07-27` |
| Normative sources | Accepted PRD v1.1、`SPEC-0003`–`SPEC-0010` |

## 2. Purpose

This document fixes the architecture rules that all SnipPlus v1 modules、components、contracts and implementation must follow. It does not replace product requirements or define visual styling.

## 3. Principles

### Principle 1 — Accepted PRD and Specs are authoritative

Architecture may organize responsibility and technology, but it must not weaken、expand or reinterpret accepted user-visible behavior. Existing code and historical Research are evidence, not product authority.

### Principle 2 — One shared Workflow State Authority

`COMP-001` is the only authority that advances shared workflow state. UI、domain capabilities and platform adapters request transitions or return outcomes; they do not independently declare the session complete.

### Principle 3 — One stable capture session context

One capture session owns:

- one Session ID;
- pre-capture foreground context;
- one Frozen Virtual Desktop topology and coordinate version;
- one immutable frame for every participating display;
- selection、annotation and output revision identities;
- cancellation and cleanup ownership.

All selection preview、annotation preview、final rendering、Clipboard and PNG output must refer to that same context.

### Principle 4 — Selection and commitment are separate

Mouse release locks the selection. It never commits Clipboard or file output. The mandatory editing／confirmation stage is entered before Complete、Save or Cancel.

### Principle 5 — Editing is required; annotation actions are optional

`FEAT-002` is a required v1 Feature because the function bar、selection adjustment、annotation document and confirmation boundary are part of the product workflow. The user may perform zero annotation actions and choose Complete immediately.

### Principle 6 — Complete and Save are explicit commitments

- Complete renders the current revision and publishes Clipboard only.
- Save renders the current revision、creates PNG and publishes the same image to Clipboard.
- Save is successful only when its required PNG and Clipboard obligations succeed.

Clipboard and file output remain separate capability boundaries; workflow coordination may require both for one Save commitment.

### Principle 7 — Platform adapters do not own product semantics

Windows capture、input、display、focus、Clipboard and file adapters return typed outcomes. They do not decide whether a user workflow is complete、recoverable or cancelled and do not mutate shared state.

### Principle 8 — Recoverable failure preserves user work

Recoverable render、save or Clipboard failure returns to Editing with the current selection、annotation document and session resources preserved. Terminal failure performs idempotent cleanup and restores the previous work context where possible.

### Principle 9 — Coordinate correctness is a session invariant

Virtual Desktop coordinates support negative origins、arbitrary monitor arrangement and mixed DPI. Selection and annotation geometry are anchored to the frozen session coordinate snapshot. Display-context mismatch must fail explicitly rather than produce a silently incorrect result.

### Principle 10 — Privacy and evidence are local by default

Capture occurs only after explicit user action. Frozen screen pixels、annotation state and Clipboard payload remain local. Real desktop screenshots and Clipboard content must not be committed as repository evidence. Normal development and product startup must not launch external GUI fixtures.

### Principle 11 — Reuse requires conformance

Existing one-display WGC、crop、image、PNG and Clipboard code may be reused only when it conforms to the accepted multi-display、editing、revision and output contracts. Passing tests for a superseded workflow do not authorize that workflow.

### Principle 12 — Documentation stays small and canonical

Product-visible changes update existing PRD and Specs. Architecture changes update the smallest owning Architecture or ADR document. Implementation progress updates code、tests、CHANGELOG and the existing conformance matrix. Do not create repeated readiness、authorization or closure chains.

## 4. Dependency Rules

```text
Product Workflow
→ Feature Coordination
→ Domain Capabilities
→ Platform Integration
```

- Upper layers depend on contracts, not concrete Windows types.
- Core product rules remain testable without real desktop capture or Clipboard access.
- Platform Integration has no authority over PRD、Feature scope or shared state.
- Components must not form circular dependencies.
- A durable cross-boundary technology trade-off requires an ADR.

## 5. Change Policy

- Product intent or user-visible behavior change → update accepted PRD first.
- Observable acceptance behavior change → update the owning Spec.
- Responsibility or dependency change → update Architecture and the conformance matrix.
- Durable technology change → create or supersede an ADR.
- Implementation-only correction inside accepted boundaries → change code、tests、CHANGELOG and conformance evidence.

## 6. Explicit Open Decisions

Architecture must not choose these without product direction:

- representation of non-display gaps in irregular monitor layouts;
- exact System Tray and MainWindow close-button behavior;
- retention／rollback after PNG success followed by Clipboard failure;
- final keyboard-only annotation acceptance standard;
- quantitative performance targets.
