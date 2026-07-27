# SnipPlus Implementation Readiness Review

## Document Control

| Field | Value |
| --- | --- |
| Document ID | `IMPLEMENTATION-READINESS-REVIEW-001` |
| Status | `Accepted — implementation paused for conformance review` |
| Original review date | `2026-07-26` |
| Product baseline revision | `2026-07-27` |
| Authority | Repository owner through explicit product decisions |
| Review scope | SnipPlus v1 first release |

## 1. Executive Decision

The previous first vertical slice proved parts of the technical pipeline, but it no longer represents the accepted first-release product scope.

The accepted v1 workflow now requires:

- manually started resident application;
- user-controlled PrintScreen takeover;
- all-display frozen capture session;
- one cross-monitor Virtual Desktop selection;
- locked selection with move、edge／corner resize and reselection;
- mandatory editing／confirmation stage with optional annotation actions;
- required v1 annotation tool set;
- Complete to Clipboard;
- Save to PNG and Clipboard;
- explicit cancellation、failure preservation、cleanup and focus restoration.

Therefore:

- Existing code is not authorized to be treated as product-complete merely because build and tests pass.
- Feature coding is paused until a requirements-to-code conformance matrix identifies what is present、missing、incorrect or obsolete.
- No new readiness、closure or authorization-document chain is required.
- The next documentation activity is the existing traceability／conformance matrix, not another planning document family.

## 2. Effective Canonical Sources

| Priority | Source | Responsibility |
| --- | --- | --- |
| 1 | `PRD-0002`、`PRD-0003`、accepted `PRD-0004`–`PRD-0006` | Product intent and first-release scope |
| 2 | accepted `SPEC-0003`–`SPEC-0010` | Observable behavior and acceptance criteria |
| 3 | Frozen Architecture and Accepted ADRs | Ownership and technology decisions |
| 4 | `IMPLEMENTATION-CONTRACTS-001` v2.0 | Shared information、lifecycle and failure contracts |
| 5 | Code and tests | Current implementation evidence only |
| 6 | Research and historical readiness documents | Historical evidence; non-normative when conflicting |

## 3. Superseded First-slice Assumptions

The following previous implementation boundaries are no longer valid product scope:

- in-app Start Capture as the primary entry;
- single-monitor-only selection;
- cross-monitor selection as a non-goal;
- mouse release leading directly to crop／Clipboard;
- Annotation as an optional post-capture branch;
- Clipboard publication immediately after region selection;
- file output UI as deferred;
- annotation tools as unspecified future work.

Existing code implementing these assumptions must be classified as `Incorrect` or `Obsolete` in the conformance review rather than silently preserved.

## 4. Existing Technical Assets That May Be Reused

Subject to conformance review, the following foundations may remain valuable:

- Windows.Graphics.Capture acquisition;
- frozen-frame ownership and cleanup concepts;
- coordinate mapping and crop utilities;
- canonical BGRA8 premultiplied image representation;
- Win2D rendering infrastructure;
- WinRT Clipboard delivery and bounded retry;
- shared state authority;
- cancellation and typed failure infrastructure;
- deterministic testing infrastructure.

Reuse is not automatic approval. Each asset must be checked against multi-display、revision、editing and output contracts.

## 5. Required Conformance Matrix

Before further feature coding, create or update one existing matrix that maps every accepted first-release requirement to:

| Column | Required meaning |
| --- | --- |
| Requirement ID | Accepted `FR`、`NFR` or Spec AC |
| Product behavior | Plain-language expected user-visible result |
| Owning Spec／Contract | Normative source |
| Current code | File／type／method or `Not implemented` |
| Current tests | Test reference or `No coverage` |
| Runtime evidence | Verified、Not verified or Invalid evidence |
| Status | `Conforms`、`Partial`、`Missing`、`Incorrect`、`Obsolete`、`Blocked by product decision` |
| Required action | Focused correction or no action |

The matrix must begin with the user-visible workflow and then descend into architecture and code. It must not infer conformity from filenames or test counts.

## 6. Mandatory Review Order

1. PrintScreen takeover and resident lifecycle.
2. Multi-display freeze and Virtual Desktop coordinate model.
3. Cross-monitor selection、mask presentation and cursor behavior.
4. Locked-selection move、resize and reselection.
5. Editing／confirmation function bar.
6. Required annotation tools and object editing.
7. Annotation coordinates、clipping and Undo／Redo.
8. Complete Clipboard-only flow.
9. Save PNG plus Clipboard flow.
10. Cancel、failure preservation、cleanup and focus restoration.
11. Privacy、external-GUI and evidence boundaries.
12. Deferred-capability exclusion.

## 7. Open Product Decisions That Block Affected Code

Coding must not guess:

- exact system-tray menu and main-window close behavior;
- output representation for non-display gaps in irregular monitor layouts;
- file rollback／retention when PNG creation succeeds but Clipboard delivery fails;
- quantitative performance targets;
- final keyboard-only annotation acceptance scope.

These do not block creation of the conformance matrix. They block only their affected implementation decisions.

## 8. Authorization State

| Activity | State |
| --- | --- |
| Canonical document correction | Completed |
| Requirements-to-code conformance review | Authorized as next activity |
| New feature coding | Paused |
| Focused code correction | Not yet selected; depends on matrix findings |
| Restore／build／test／runtime | Only when explicitly included in the later focused task |
| Interactive external-GUI verification | Requires explicit current-task authorization |
| New readiness-document chain | Prohibited |

## 9. Final Decision

`Canonical product documents are aligned. Continue with a requirements-to-code conformance matrix before selecting the next coding task.`

No claim is made that the current implementation satisfies the revised first-release scope.
