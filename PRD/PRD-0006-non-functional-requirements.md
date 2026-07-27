# PRD-0006 Non-functional Requirements

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `PRD-0006` |
| Version | `1.3` |
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
| `NFR-002` | Selection、object movement、resize、tool switching and Annotation interaction must meet the frame-time and input-response targets below. | `Must` |
| `NFR-003` | Render、PNG and Clipboard work must meet the output-size targets below and present a busy／progress state when work is not immediate. | `Must` |

### 3.1 Verification Profiles

| Profile | Display configuration | Final-output class |
| --- | --- | --- |
| `Standard` | Up to 2 displays、total active source pixels no more than `16,588,800` | Up to `8,294,400` pixels (`3840 × 2160`) |
| `Maximum` | Within the complete v1 support envelope in section 11 | Up to `67,108,864` pixels and the dimensional limits in section 11 |

Reference machine for release verification:

- Windows 11 24H2 x64;
- 16 GB RAM or more;
- hardware-accelerated Direct3D 11-class GPU;
- SSD storage;
- Release x64 build without debugger attachment.

### 3.2 Quantitative Targets

| Operation | Standard target | Maximum target |
| --- | --- | --- |
| PrintScreen accepted → all-display Selection interactive | p95 `≤ 500 ms` | p95 `≤ 1,000 ms` |
| Selection／Annotation interaction frame time | p95 `≤ 33 ms` | p95 `≤ 33 ms` |
| Discrete keyboard／pointer action → visible response | p95 `≤ 100 ms` | p95 `≤ 100 ms` |
| Complete: final render + Clipboard, output `≤ 8,294,400` pixels | p95 `≤ 1.5 s` | Same |
| Complete: output `≤ 33,177,600` pixels | p95 `≤ 4 s` | Same |
| Complete: output `≤ 67,108,864` pixels | — | p95 `≤ 8 s` |
| Save after the user confirms Save As, output `≤ 8,294,400` pixels | p95 `≤ 2 s` | Same |
| Save after confirmation, output `≤ 33,177,600` pixels | p95 `≤ 6 s` | Same |
| Save after confirmation, output `≤ 67,108,864` pixels | — | p95 `≤ 12 s` |

Additional rules:

- No Selection or Annotation input operation may block visible input processing for more than `100 ms` under the supported envelope.
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
- Use deterministic synthetic content for automated timing where possible; authorized real multi-display runtime verification remains required for final acceptance.
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
| `NFR-011` | Mixed-DPI pointer and keyboard input must map deterministically to frozen physical-pixel content. | `Must` |
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

## 9. Accessibility and Keyboard-only Editing

| ID | Requirement | Priority |
| --- | --- | --- |
| `NFR-029` | Complete、Save、Cancel、tool、style and annotation-object controls must expose understandable accessible names、roles、values and state. | `Must` |
| `NFR-030` | Keyboard cancellation with Esc must work before selection、during drag and at stable Editing; an open transient editor or picker consumes the first Esc before stable Editing cancellation applies. | `Must` |
| `NFR-031` | Color must not be the only indicator of selected tool、selection boundary、focus or error state. | `Must` |

The complete v1 keyboard-only Annotation acceptance scope starts after a valid Selection is locked and continues through Complete、Save or Cancel. Initial crosshair region creation remains pointer-driven in v1.

From `SelectionLocked` onward, the user must be able to complete the following without a pointer:

- enter and leave the function bar and canvas zones;
- select every required tool;
- create at least one object for Rectangle、Arrow／Line、Highlighter、Text、Mosaic／Blur and Numbered Marker;
- select objects in deterministic z-order;
- move objects and the locked Selection by `1` physical output pixel with Arrow keys and `10` pixels with Shift+Arrow;
- focus applicable resize handles and resize by the same `1`／`10` pixel increments;
- edit text through normal Windows text editing and Chinese IME input;
- change every applicable style、mode、number and size value;
- delete objects;
- Undo and Redo;
- invoke Save、Complete and Cancel;
- recover predictably from dialogs、pickers and transient editors without a keyboard trap.

Required keyboard model:

- `F6` cycles major zones; `Tab`／`Shift+Tab` navigate within the active zone.
- `V` Selection、`R` Rectangle、`A` Arrow／Line、`H` Highlighter、`T` Text、`M` Mosaic／Blur、`N` Numbered Marker when text entry is not active.
- `Ctrl+Z` Undo、`Ctrl+Y` Redo、`Ctrl+S` Save、`Ctrl+Enter` Complete、`Delete` remove selected object.
- Activating a creation tool from the keyboard creates a deterministic default object inside the current Selection and focuses it for movement、resize and styling. Highlighter creates a short horizontal stroke; Text creates and focuses a text box; Numbered Marker is placed at the Selection center.
- In the canvas zone, `Tab`／`Shift+Tab` traverse Selection、annotation objects in z-order and applicable resize handles.
- The first Esc closes an open picker、popover、text editor or uncommitted creation operation. Esc from stable Editing cancels the capture session.

Acceptance requires a keyboard-only test from `SelectionLocked` through object creation、editing、Undo／Redo and each output action with the pointer unused. Visible focus、high-contrast operation、200% UI scaling and Narrator-readable control names／states are included in acceptance.

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
- each display no larger than `7,680 × 4,320` physical pixels;
- total active display-source pixels no greater than `66,355,200`;
- Virtual Desktop bounding-box width no greater than `16,384` physical pixels;
- Virtual Desktop bounding-box height no greater than `16,384` physical pixels;
- final Selection width and height each no greater than `16,384` physical pixels;
- final Selection area no greater than `67,108,864` pixels;
- transparent non-display gaps count toward final Selection area because they still require output allocation;
- mirrored outputs resolving to one logical desktop surface count once.

When any limit is exceeded:

- do not omit displays or enter a partial capture session;
- fail before interactive Selection with an actionable supported-limit message;
- release all acquired frames and restore the pre-capture work context;
- keep SnipPlus resident and able to accept a later capture after the display configuration changes.

## 12. Finalized Quality Decisions

The following are accepted and no longer open:

- quantitative performance、responsiveness and memory targets in section 3;
- the supported display-count、resolution、Virtual Desktop and output-size envelope in section 11;
- the complete keyboard-only Editing／Annotation acceptance standard in section 9;
- MainWindow `X` exits SnipPlus and releases PrintScreen takeover;
- non-display gaps in final output are transparent;
- a successfully created PNG is retained if later Clipboard publication fails;
- Save As initially proposes the Downloads folder.

No remaining product-quality decision blocks the ordered v1 implementation. Future changes to these limits require explicit Repository owner approval and updates to the existing canonical documents.