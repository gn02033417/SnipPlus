# PRD-0006 Non-functional Requirements

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `PRD-0006` |
| Version | `1.4` |
| Status | `Accepted` |
| Product authority | Repository owner through explicit product decisions |
| Last reviewed | `2026-07-27` |

## 2. Requirement Rules

- `Must` is required for the first release.
- Quantitative targets are release-acceptance criteria measured by the protocol in this document; they are not runtime timeout values unless explicitly stated.
- These requirements constrain Specs、Architecture、code and tests.

## 3. Performance and Responsiveness

| ID | Requirement | Priority |
| --- | --- | --- |
| `NFR-001` | PrintScreen acceptance through an interactive all-display masked Selection must meet the profile-specific p95 latency targets below. | `Must` |
| `NFR-002` | Pointer-driven Selection、object movement、resize、tool switching and Annotation interaction must meet the frame-time and input-response targets below. | `Must` |
| `NFR-003` | Render、PNG and Clipboard work must meet the output-size targets below and present a busy／progress state when work is not immediate. | `Must` |

### 3.1 Verification Profiles

| Profile | Display configuration | Purpose |
| --- | --- | --- |
| `Owner Reference` | 3 displays: primary `2560 × 1440`、lower `1920 × 1080` at Windows scaling `150%`、left `2560 × 1440` | Mandatory mixed-DPI／layout verification matching the Repository owner’s current environment |
| `Standard` | Up to 2 displays、total active source pixels no more than `16,588,800` | Common release-performance verification |
| `Maximum` | Up to 4 active displays、each no larger than `3840 × 2160`、within the complete capacity envelope in section 11 | Maximum v1 support verification |

Reference machine for release verification:

- Windows 11 24H2 x64;
- 16 GB RAM or more;
- hardware-accelerated Direct3D 11-class GPU;
- SSD storage;
- Release x64 build without debugger attachment.

### 3.2 Quantitative Targets

| Operation | Owner Reference／Standard target | Maximum target |
| --- | --- | --- |
| PrintScreen accepted → all-display Selection interactive | p95 `≤ 500 ms` | p95 `≤ 1,000 ms` |
| Selection／Annotation interaction frame time | p95 `≤ 33 ms` | p95 `≤ 33 ms` |
| Discrete pointer／UI action → visible response | p95 `≤ 100 ms` | p95 `≤ 100 ms` |
| Complete: final render + Clipboard, output `≤ 8,294,400` pixels | p95 `≤ 1.5 s` | Same |
| Complete: output `≤ 33,177,600` pixels | p95 `≤ 4 s` | Same |
| Complete: output `≤ 67,108,864` pixels including transparent gaps | — | p95 `≤ 8 s` |
| Save after the user confirms Save As, output `≤ 8,294,400` pixels | p95 `≤ 2 s` | Same |
| Save after confirmation, output `≤ 33,177,600` pixels | p95 `≤ 6 s` | Same |
| Save after confirmation, output `≤ 67,108,864` pixels including transparent gaps | — | p95 `≤ 12 s` |

Additional rules:

- No pointer-driven Selection or Annotation operation may block visible input processing for more than `100 ms` under the supported envelope.
- When a commit operation has not completed within `300 ms`, a non-blocking busy／progress state must become visible.
- Successful completion remains silent after the operation finishes.

### 3.3 Memory Targets

- Idle private working set after startup stabilization: `≤ 250 MB` on the reference machine.
- Peak private working set during a maximum-envelope capture session: `≤ 2.0 GB`.
- Within `10 seconds` after session cleanup, private working set must return to no more than the stabilized idle baseline plus `150 MB`.
- After `20` consecutive Standard-profile capture sessions, retained steady-state growth must be `≤ 50 MB` compared with the post-warm-up baseline.

### 3.4 Measurement Protocol

- Use `3` warm-up runs followed by at least `30` measured runs per scenario.
- Report p50、p95 and maximum; release acceptance is based on p95 plus absence of hangs、crashes and resource leaks.
- Use deterministic synthetic content for automated timing where possible.
- Final acceptance must include explicitly authorized runtime verification on the Owner Reference three-display configuration and a Maximum-profile configuration.
- User decision time inside Save As is excluded from Save timing.

## 4. Reliability and State Integrity

| ID | Requirement | Priority |
| --- | --- | --- |
| `NFR-004` | One capture session owns one stable set of frozen display frames until Complete、successful Save、Cancel or terminal failure. | `Must` |
| `NFR-005` | Selection、annotations、rendered output and Clipboard／file delivery must refer to the same capture session and coordinate snapshot. | `Must` |
| `NFR-006` | Cancel and failure cleanup must be idempotent and must close every capture overlay and transient function bar. | `Must` |
| `NFR-007` | Output failure must not destroy the user’s current selection or annotations when retry is possible. | `Must` |
| `NFR-008` | A session must never be reported as completed unless its required Clipboard and Save obligations have succeeded; an already-created PNG may remain after a later Clipboard failure. | `Must` |

## 5. Multi-display and Coordinate Correctness

| ID | Requirement | Priority |
| --- | --- | --- |
| `NFR-009` | The first release must support one rectangular selection spanning multiple displays within the v1 support envelope. | `Must` |
| `NFR-010` | Virtual Desktop coordinates must support negative origins and arbitrary display arrangement; physical non-display gaps in final output must be transparent. | `Must` |
| `NFR-011` | Mixed-DPI pointer input must map deterministically to frozen physical-pixel content, including the Owner Reference `150%` lower display. | `Must` |
| `NFR-012` | Moving or resizing a selection must not scale、shift or corrupt annotation geometry anchored to the frozen canvas. | `Must` |
| `NFR-013` | Display topology or DPI change that invalidates a session must produce a classified failure rather than silently using stale bounds. | `Must` |

## 6. Usability

| ID | Requirement | Priority |
| --- | --- | --- |
| `NFR-014` | The workflow must preserve familiar Windows screenshot behavior: PrintScreen、crosshair selection、dimmed outside region and clear inside region. | `Must` |
| `NFR-015` | Mouse release must not be mistaken for final confirmation; the function bar and explicit Complete／Save actions define commitment. | `Must` |
| `NFR-016` | Annotation operations are optional, but the editing／confirmation stage is always available after a valid selection. | `Must` |
| `NFR-017` | The function bar must remain visible by repositioning around the selection when necessary. | `Must` |
| `NFR-018` | Successful completion must be silent and return the user to the previous work context. | `Must` |
| `NFR-019` | Error messages must state the failed operation and preserve an actionable retry or cancel path when possible; after PNG success and Clipboard failure, feedback must also state that the PNG was retained. | `Must` |

## 7. Focus、Exit and Work-context Protection

| ID | Requirement | Priority |
| --- | --- | --- |
| `NFR-020` | The application active before PrintScreen must be recorded for later focus restoration. | `Must` |
| `NFR-021` | SnipPlus normal windows must be excluded from the frozen capture source. | `Must` |
| `NFR-022` | Complete、successful Save and Cancel must not open or foreground the SnipPlus main window. | `Must` |
| `NFR-023` | Disabling takeover、closing MainWindow with `X` or otherwise exiting SnipPlus must release PrintScreen interception and terminate without leaving a hidden resident process. | `Must` |

## 8. Privacy and Security

| ID | Requirement | Priority |
| --- | --- | --- |
| `NFR-024` | Screen capture occurs only after an explicit user PrintScreen or authorized secondary capture action. | `Must` |
| `NFR-025` | Frozen screen content、annotation state and Clipboard payload remain local unless a future explicit product decision adds external transfer. | `Must` |
| `NFR-026` | Real desktop screenshots and Clipboard payloads must not be committed as repository evidence. | `Must` |
| `NFR-027` | Normal development、build、unit test and product startup must not launch Paint or another external GUI fixture. | `Must` |
| `NFR-028` | Interactive verification that opens external windows requires explicit authorization in the current task. | `Must` |

## 9. Accessibility and Keyboard Boundary

| ID | Requirement | Priority |
| --- | --- | --- |
| `NFR-029` | Complete、Save、Cancel and required function-bar controls must expose understandable accessible names and state. | `Must` |
| `NFR-030` | Esc must cancel capture before selection、during drag and during stable Editing as defined by the accepted capture workflow. | `Must` |
| `NFR-031` | Color must not be the only indicator of selected tool、selection boundary or error state. | `Must` |

The following are explicitly deferred from v1:

- keyboard-only creation、selection、movement、resize and styling of Annotation objects;
- F6／Tab zone and object traversal as a complete product workflow;
- single-letter tool shortcuts;
- Ctrl-based Undo／Redo、Save or Complete shortcuts;
- Delete and Arrow-key object manipulation;
- a release gate requiring the pointer to remain unused after `SelectionLocked`.

PrintScreen remains the required global capture key. Esc remains the required capture-cancellation key. No other keyboard shortcut is part of the v1 acceptance baseline.

## 10. Maintainability and Traceability

| ID | Requirement | Priority |
| --- | --- | --- |
| `NFR-032` | Product decisions must be represented in the smallest set of canonical PRD、Spec and contract documents. | `Must` |
| `NFR-033` | Historical Research and prior readiness chains must not override the accepted product baseline. | `Must` |
| `NFR-034` | Every implemented v1 capability and test must trace to an accepted FR、NFR and Spec acceptance criterion. | `Must` |
| `NFR-035` | Unknown product behavior must be reported instead of silently invented in code. | `Must` |
| `NFR-036` | COMP-001 remains the sole shared Workflow State Authority. | `Must` |

## 11. First-release Compatibility and Capacity Boundary

| ID | Requirement | Priority |
| --- | --- | --- |
| `NFR-037` | Windows Desktop is the first-release platform. | `Must` |
| `NFR-038` | The implementation must support the approved Windows 11 baseline and the complete capacity envelope below. | `Must` |
| `NFR-039` | HDR preservation、ARM64、cross-platform and additional output formats remain deferred unless separately approved. | `Must` |

The supported v1 display envelope is:

- `1` through `4` active logical desktop display surfaces;
- each display no larger than `3,840 × 2,160` physical pixels;
- total active display-source pixels no greater than `33,177,600` (`4 × 3840 × 2160`);
- Virtual Desktop bounding-box width no greater than `16,384` physical pixels;
- Virtual Desktop bounding-box height no greater than `16,384` physical pixels;
- final Selection width and height each no greater than `16,384` physical pixels;
- final Selection area no greater than `67,108,864` pixels;
- transparent non-display gaps count toward final Selection area because they still require output allocation;
- mirrored outputs resolving to one logical desktop surface count once;
- an 8K display is outside the v1 support envelope even when total pixel count would otherwise fit.

The `67,108,864` final-area limit intentionally exceeds four active 4K source surfaces so ordinary irregular arrangements can include transparent gaps without reducing the four-4K source guarantee.

When any limit is exceeded:

- do not omit displays or enter a partial capture session;
- fail before interactive Selection with an actionable supported-limit message;
- release all acquired frames and restore the pre-capture work context;
- keep SnipPlus resident and able to accept a later capture after the display configuration changes.

## 12. Finalized Quality Decisions

The following are accepted and no longer open:

- quantitative performance、responsiveness and memory targets in section 3;
- the Repository owner’s three-display mixed-DPI verification profile;
- a maximum source configuration of four 4K displays;
- the display-count、resolution、Virtual Desktop and output-allocation envelope in section 11;
- keyboard-only Annotation and non-PrintScreen shortcut support are deferred;
- PrintScreen entry and Esc cancellation remain required;
- MainWindow `X` exits SnipPlus and releases PrintScreen takeover;
- non-display gaps in final output are transparent;
- a successfully created PNG is retained if later Clipboard publication fails;
- Save As initially proposes Downloads.

No remaining product-quality decision blocks the ordered v1 implementation. Future changes require explicit Repository owner approval and updates to the existing canonical documents.