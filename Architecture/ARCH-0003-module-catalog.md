# ARCH-0003 Module Catalog

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `ARCH-0003` |
| Version | `1.0` |
| Architecture stability | `Accepted` |
| Last reviewed | `2026-07-27` |
| Depends on | `ARCH-0001`、`ARCH-0002`、`SPEC-0010` |

## 2. Purpose

This catalog assigns one primary responsibility to each module required by the accepted SnipPlus v1 workflow. It does not define classes、methods or visual styling.

## 3. Module Catalog

| Module ID | Name | Status | Primary layer | Responsibility |
| --- | --- | --- | --- | --- |
| `MOD-001` | Workflow Orchestration | `Required` | Product Workflow | Own capture-session lifecycle and requests to the shared Workflow State Authority. |
| `MOD-002` | Feature Coordination | `Required` | Feature Coordination | Coordinate Capture／Selection、Editing／Annotation、Clipboard、PNG Output and Recovery in the accepted order. |
| `MOD-003` | Capture and Selection Capability | `Required` | Domain Capability | Model Frozen Virtual Desktop、per-display frames、selection geometry、lock、move、resize and reselection. |
| `MOD-004` | Editing and Annotation Capability | `Required` | Domain Capability | Own the mandatory editing stage、annotation document、required v1 tools、object editing and Undo／Redo. |
| `MOD-005` | Clipboard Handoff Capability | `Required` | Domain Capability | Define final-image Clipboard intent、result、retry and commitment semantics. |
| `MOD-006` | PNG Output Capability | `Required` | Domain Capability | Define Save As、PNG delivery intent、file result and Save commitment semantics. |
| `MOD-007` | Workflow Boundary and Recovery | `Required` | Feature Coordination | Classify success、Save-dialog cancel、recoverable failure、terminal failure、cleanup and focus restoration. |
| `MOD-008` | Platform Capture Integration | `Required` | Platform Integration | Enumerate and acquire immutable frames for all participating displays through platform capture APIs. |
| `MOD-009` | Platform Clipboard Integration | `Required` | Platform Integration | Publish the final rendered image to WinRT Clipboard with accepted privacy and retry rules. |
| `MOD-010` | Platform Output Integration | `Required` | Platform Integration | Provide Windows Save As、PNG file creation and typed delivery outcomes. |
| `MOD-011` | Platform Interaction Integration | `Required` | Platform Integration | Provide PrintScreen interception、display／DPI topology、pointer／keyboard input、window exclusion and focus restoration. |

## 4. Module Boundaries

### MOD-001 — Workflow Orchestration

Owns one capture session from accepted entry to `ResidentReady`. It requests legal state transitions from `COMP-001`. It does not capture pixels、render annotations or call platform APIs directly.

### MOD-002 — Feature Coordination

Coordinates:

```text
Resident entry
→ Freezing
→ Selecting
→ SelectionLocked
→ Editing
→ Complete OR Save OR Cancel
```

It does not own Feature internals. It must not route mouse release directly to Clipboard.

### MOD-003 — Capture and Selection Capability

Owns the platform-independent session model for:

- Virtual Desktop origin and physical bounds;
- per-display snapshots and frame identities;
- selection revisions in Virtual Desktop coordinates;
- initial drag、lock、move、edge／corner resize and reselection;
- cross-display intersections and crop planning.

It does not own UI controls or WGC objects.

### MOD-004 — Editing and Annotation Capability

Owns the required editing／confirmation capability. Annotation actions may be skipped, but this module remains required because the function bar、annotation document and explicit commitment boundary are part of v1.

Required tool families:

- Rectangle;
- Arrow／Line;
- Highlighter;
- Text;
- Mosaic／Blur;
- Numbered Marker;
- shared color、thickness／size controls;
- object selection、movement、resize、restyle、delete、Undo and Redo.

### MOD-005 — Clipboard Handoff Capability

Owns Clipboard commitment semantics after Complete or successful PNG creation in Save. It does not own Save As or file creation.

### MOD-006 — PNG Output Capability

Owns the Save As and PNG delivery capability. It does not own Clipboard publication. The Save workflow is complete only after both MOD-006 and MOD-005 report success for the same rendered result.

### MOD-007 — Workflow Boundary and Recovery

Owns cross-feature outcome classification and routing:

- successful Complete;
- successful Save;
- whole-session Cancel;
- Save-dialog cancellation returning to Editing;
- recoverable output failure preserving Editing;
- terminal failure with cleanup and focus restoration;
- stale outcome rejection.

### MOD-008 — Platform Capture Integration

Owns platform capture resources and typed outcomes. All required display frames are acquired before Selection becomes interactive. It does not own workflow state or product completion.

### MOD-009 — Platform Clipboard Integration

Owns DataPackage publication、bounded cancellable retry、payload lifetime and history／roaming defaults. It does not decide that the workflow is complete.

### MOD-010 — Platform Output Integration

Owns Windows Save As、PNG write and typed file-delivery outcomes. It does not choose rollback behavior after a later Clipboard failure until the product decision is made.

### MOD-011 — Platform Interaction Integration

Owns platform boundaries for:

- PrintScreen interception and release;
- connected-display enumeration;
- Virtual Desktop topology、negative origins and DPI context;
- pointer、crosshair and Esc input;
- excluding SnipPlus normal windows from capture;
- recording and restoring foreground context.

## 5. Dependency Rules

```mermaid
flowchart TB
    M1[MOD-001 Workflow Orchestration] --> M2[MOD-002 Feature Coordination]
    M2 --> M3[MOD-003 Capture and Selection]
    M2 --> M4[MOD-004 Editing and Annotation]
    M2 --> M5[MOD-005 Clipboard Handoff]
    M2 --> M6[MOD-006 PNG Output]
    M2 --> M7[MOD-007 Boundary and Recovery]
    M3 --> M8[MOD-008 Platform Capture]
    M3 --> M11[MOD-011 Platform Interaction]
    M5 --> M9[MOD-009 Platform Clipboard]
    M6 --> M10[MOD-010 Platform Output]
    M7 --> M11
```

- Domain modules depend on platform abstractions, not concrete platform types.
- Platform modules do not depend on Product Workflow implementation.
- MOD-005 and MOD-006 remain separate; MOD-002 coordinates their required Save sequence.
- MOD-004 is required, while creating annotations is optional for the user.
- No module may mutate shared workflow state except through `COMP-001`.
- No circular dependencies.

## 6. Deferred Capability Boundary

The v1 module catalog does not add modules for OCR、history、pinning、cloud、plugins、telemetry、updates、opaque freehand pen、ellipse or additional image formats.
