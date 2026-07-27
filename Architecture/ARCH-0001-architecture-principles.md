# ARCH-0001 Architecture Principles

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `ARCH-0001` |
| Version | `1.1` |
| Architecture stability | `Accepted` |
| Last reviewed | `2026-07-27` |
| Normative sources | Accepted current PRD、`SPEC-0003`–`SPEC-0010` |

## 2. Purpose

This document fixes the Architecture rules that all SnipPlus v1 modules、components、contracts and implementation must follow. It does not replace product requirements or define visual styling.

## 3. Principles

### Principle 1 — Accepted PRD and Specs are authoritative

Architecture organizes responsibility and technology but never weakens、expands or reinterprets accepted behavior. Existing code and historical Research are evidence, not product authority.

### Principle 2 — One shared Workflow State Authority

`COMP-001` alone advances shared workflow state. UI、domain capabilities and platform adapters request transitions or return typed outcomes.

### Principle 3 — One stable Capture Session context

One Session owns:

- Session ID;
- pre-capture foreground context;
- capacity-validation result;
- Frozen Virtual Desktop topology and coordinate version;
- one immutable frame per participating display;
- Selection、Annotation and output revisions;
- cancellation and cleanup ownership.

All preview、render、Clipboard and PNG operations refer to that same context.

### Principle 4 — Supported capacity is explicit

- V1 supports `1`–`4` active logical displays.
- Each display is no larger than `3840 × 2160`.
- Total source pixels do not exceed `33,177,600`.
- Virtual Desktop and Selection allocation limits are defined by PRD-0006.
- 8K displays are outside v1.
- Unsupported capacity never produces omitted、downscaled or partial display capture.

### Principle 5 — Selection and commitment are separate

Mouse release locks Selection. It never commits Clipboard or file output. Mandatory Editing／confirmation occurs before Complete、Save or Cancel.

### Principle 6 — Editing is required; Annotation actions are optional

`FEAT-002` is required because function bar、Selection adjustment、Annotation document and explicit commitment are part of v1. The user may create no annotations and press Complete.

### Principle 7 — V1 editing is pointer-driven

Selection adjustment、Annotation creation and object manipulation are pointer-driven in v1. PrintScreen and Esc remain required keys. Keyboard-only Annotation and non-PrintScreen tool／action shortcuts are deferred.

### Principle 8 — Complete and Save are explicit commitments

- Complete renders current revisions and publishes Clipboard only.
- Save creates PNG and publishes the same image to Clipboard.
- Save completes only after both obligations succeed.
- A successfully created PNG is retained if later Clipboard publication fails.

### Principle 9 — Platform adapters do not own product semantics

Windows capture、input、display、focus、Clipboard and file adapters return typed outcomes. They never decide workflow completion or mutate shared state.

### Principle 10 — Recoverable failure preserves user work

Recoverable render、save or Clipboard failure returns to Editing with current Selection、Annotation document and Session resources preserved. Terminal failure performs idempotent cleanup and restores the previous work context where possible.

### Principle 11 — Coordinate and alpha correctness are Session invariants

Virtual Desktop coordinates support negative origins、irregular monitor arrangement and mixed DPI. Selection and Annotation geometry use the frozen Session snapshot. Physical non-display gaps render as transparent pixels. Context mismatch fails explicitly.

### Principle 12 — Performance is measured, not asserted

- Release evidence uses 3 warm-ups and at least 30 measured runs.
- Results report p50、p95 and maximum.
- Capture、pointer interaction、output、progress and memory targets are normative in PRD-0006.
- Performance targets are release gates, not arbitrary runtime cancellation timeouts.

### Principle 13 — Privacy and evidence are local by default

Capture occurs only after explicit user action. Frozen pixels、Annotation state and Clipboard payload remain local. Real desktop screenshots and Clipboard content are not committed as Repository evidence. Normal product startup and non-interactive tests do not launch external GUI fixtures.

### Principle 14 — Reuse requires conformance

Existing one-display WGC、crop、image、PNG and Clipboard code may be reused only when it conforms to the accepted capacity、multi-display、Editing、revision and output contracts.

### Principle 15 — Documentation stays small and canonical

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

## 6. Product Decision Status

No visible v1 product or quality decision remains open. Transparent gaps、direct exit、retained PNG、performance targets、four-4K capacity and deferred keyboard-only scope are fixed by accepted PRD／Specs.