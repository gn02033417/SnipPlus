# PRD-0006 Non-functional Requirements

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `PRD-0006` |
| Version | `1.2` |
| Status | `Accepted` |
| Product authority | Repository owner through explicit product decisions |
| Last reviewed | `2026-07-27` |

## 2. Requirement Rules

- `Must` is required for the first release.
- Quantitative targets that have not been measured remain future verification work; they must not be invented.
- These requirements constrain Specs、Architecture、code and tests.

## 3. Performance and Responsiveness

| ID | Requirement | Priority |
| --- | --- | --- |
| `NFR-001` | PrintScreen takeover must start the capture experience without avoidable delay. | `Must` |
| `NFR-002` | Multi-display freeze、mask presentation、selection movement and annotation interaction must remain visibly responsive. | `Must` |
| `NFR-003` | Expensive render、save and Clipboard work must not freeze interaction without explicit progress or failure state. | `Must` |

No unsupported millisecond target is asserted by this PRD.

## 4. Reliability and State Integrity

| ID | Requirement | Priority |
| --- | --- | --- |
| `NFR-004` | One capture session owns one stable set of frozen display frames until Complete、successful Save、Cancel or terminal failure. | `Must` |
| `NFR-005` | Selection、annotations、rendered output and Clipboard／file delivery must refer to the same capture session and coordinate snapshot. | `Must` |
| `NFR-006` | Cancel and failure cleanup must be idempotent and must close every capture overlay and transient function bar. | `Must` |
| `NFR-007` | Output failure must not destroy the user’s current selection or annotations when retry is possible. | `Must` |
| `NFR-008` | A session must never be reported as completed unless its required Clipboard and Save obligations have succeeded; an already-created PNG may still remain after a later Clipboard failure. | `Must` |

## 5. Multi-display and Coordinate Correctness

| ID | Requirement | Priority |
| --- | --- | --- |
| `NFR-009` | The first release must support one rectangular selection spanning multiple displays. | `Must` |
| `NFR-010` | Virtual Desktop coordinates must support negative origins and arbitrary display arrangement; physical non-display gaps in final output must be transparent. | `Must` |
| `NFR-011` | Mixed-DPI pointer input must map deterministically to frozen physical-pixel content. | `Must` |
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

## 9. Accessibility

| ID | Requirement | Priority |
| --- | --- | --- |
| `NFR-029` | Complete、Save、Cancel and annotation controls must expose understandable accessible names and state. | `Must` |
| `NFR-030` | Keyboard cancellation with Esc must work before selection、during drag and during editing. | `Must` |
| `NFR-031` | Color must not be the only indicator of selected tool、selection boundary or error state. | `Must` |

Detailed keyboard-only annotation operation remains a separate acceptance review and must not be assumed complete without evidence.

## 10. Maintainability and Traceability

| ID | Requirement | Priority |
| --- | --- | --- |
| `NFR-032` | Product decisions must be represented in the smallest set of canonical PRD、Spec and contract documents. | `Must` |
| `NFR-033` | Historical Research and prior readiness chains must not override the accepted product baseline. | `Must` |
| `NFR-034` | Every implemented v1 capability and test must trace to an accepted FR、NFR and Spec acceptance criterion. | `Must` |
| `NFR-035` | Unknown product behavior must be reported instead of silently invented in code. | `Must` |
| `NFR-036` | COMP-001 remains the sole shared Workflow State Authority. | `Must` |

## 11. First-release Compatibility Boundary

| ID | Requirement | Priority |
| --- | --- | --- |
| `NFR-037` | Windows Desktop is the first-release platform. | `Must` |
| `NFR-038` | The implementation must support the approved Windows 11 baseline and multi-display desktop configuration used for verification. | `Must` |
| `NFR-039` | HDR preservation、ARM64、cross-platform and additional output formats remain deferred unless separately approved. | `Must` |

## 12. Open Quality Questions

The following remain unresolved:

- Quantitative latency targets.
- Exact supported display-count and maximum Virtual Desktop dimensions.
- Final keyboard-only annotation acceptance standard.

The following are now resolved and must not be treated as open questions:

- MainWindow `X` exits SnipPlus and releases PrintScreen takeover; it does not hide to tray.
- Non-display gaps in final output are transparent.
- A PNG that was successfully created is retained if later Clipboard publication fails.
- Save As initially proposes the Downloads folder.

Affected implementation must stop only for the remaining unresolved quality questions.