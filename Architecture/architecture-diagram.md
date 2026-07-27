# Mermaid Architecture Diagram

狀態：`Accepted`

The diagram represents the accepted SnipPlus v1 responsibility and dependency baseline. It does not claim that every component is implemented.

## 1. Layer and Capability Diagram

```mermaid
flowchart TB
    User[User]
    App[SnipPlus.App
WinUI composition and capture UI]

    subgraph Workflow[Product Workflow and Feature Coordination]
        State[COMP-001
Workflow State Authority]
        Session[COMP-002
Session Lifecycle]
        Flow[COMP-003
Feature Flow Coordinator]
        Recovery[COMP-011/012/013
Completion, Recovery, Progress, Feedback]
    end

    subgraph Domain[Domain Capabilities]
        Freeze[COMP-004
Capacity, Capture Request and Freeze]
        Selection[COMP-005
Pointer Selection]
        Render[COMP-006
Capacity-aware Final Render]
        Editing[COMP-007
Editing Session]
        Annotation[COMP-008
Pointer Annotation Document and History]
        Clipboard[COMP-009
Clipboard Handoff]
        Output[COMP-010
PNG Output and Retained File]
    end

    subgraph Platform[Windows Platform Integration]
        CaptureAdapter[COMP-014
WGC Capture Adapter]
        ClipboardAdapter[COMP-015
WinRT Clipboard Adapter]
        OutputAdapter[COMP-016
Save As / PNG Adapter]
        Input[COMP-017
PrintScreen, Pointer and Esc]
        Display[COMP-018
Display, DPI and Foreground Context]
    end

    Contracts[(SnipPlus.Contracts
Session, Capacity, Selection, Annotation,
Image, Delivery and Failure contracts)]

    User --> App
    App --> Flow
    Input --> Flow
    Flow --> State
    Flow --> Session
    Flow --> Freeze
    Flow --> Selection
    Flow --> Editing
    Flow --> Clipboard
    Flow --> Output
    Flow --> Recovery

    Freeze --> CaptureAdapter
    Freeze --> Display
    Selection --> Input
    Selection --> Display
    Editing --> Annotation
    Render --> Annotation
    Render --> CaptureAdapter
    Clipboard --> ClipboardAdapter
    Output --> OutputAdapter
    Recovery --> Display

    App -. uses .-> Contracts
    Workflow -. uses .-> Contracts
    Domain -. uses .-> Contracts
    Platform -. implements .-> Contracts
```

## 2. Accepted Workflow Diagram

```mermaid
stateDiagram-v2
    [*] --> ResidentReady
    ResidentReady --> CaptureRequested: enabled PrintScreen
    CaptureRequested --> Freezing: four-4K topology supported
    CaptureRequested --> Failed: unsupported capacity
    Freezing --> Selecting: all frames ready
    Freezing --> Failed: capture/context failure
    Selecting --> SelectionLocked: pointer released with valid supported rectangle
    Selecting --> Cancelled: Esc / Cancel
    SelectionLocked --> Selecting: pointer reselection
    SelectionLocked --> Editing
    SelectionLocked --> Cancelled: Esc / Cancel
    Editing --> Editing: pointer adjust / annotate / function-bar undo-redo
    Editing --> CommittingClipboard: Complete
    Editing --> Saving: Save
    Editing --> Cancelled: Esc / Cancel
    CommittingClipboard --> Completed: Clipboard success
    CommittingClipboard --> Editing: recoverable failure
    Saving --> Editing: Save As cancelled / recoverable failure / retained PNG + Clipboard failure
    Saving --> Completed: PNG and Clipboard success
    Completed --> ResidentReady: cleanup and focus restore
    Cancelled --> ResidentReady: cleanup and focus restore
    Failed --> ResidentReady: terminal cleanup and recovery
```

There is no legal transition from mouse release directly to Clipboard or file output. Unsupported capacity never becomes partial Selection.

## 3. Quality and Scope Notes

- Supported source topology: up to four logical displays、each no larger than `3840 × 2160`.
- Owner Reference runtime profile: primary `2560 × 1440`、lower `1920 × 1080` at `150%` scaling、left `2560 × 1440`.
- Non-display gaps render transparently.
- Commit work still running after `300 ms` exposes non-blocking progress.
- V1 Selection adjustment and Annotation are pointer-driven.
- PrintScreen and Esc are required; keyboard-only Annotation and non-PrintScreen shortcuts are deferred.

## 4. Project Dependency Diagram

```mermaid
flowchart LR
    Contracts[SnipPlus.Contracts]
    Core[SnipPlus.Core]
    Windows[SnipPlus.Windows]
    App[SnipPlus.App]

    Core --> Contracts
    Windows --> Contracts
    App --> Contracts
    App --> Core
    App --> Windows
```

Rules:

- `SnipPlus.Contracts` depends on no source project.
- `SnipPlus.Core` contains product and domain rules without concrete Windows types.
- `SnipPlus.Windows` provides platform adapters and does not mutate shared workflow state.
- `SnipPlus.App` composes UI and adapters but does not own product semantics.
- No circular project references.

## 5. Current Conformance Note

Current source implements only one-display capture、same-frame crop、basic mask presentation、image／PNG foundations、Clipboard retry and an earlier state graph. Missing or obsolete areas are tracked in `PRD/PRD-TRACEABILITY-MATRIX.md`. Keyboard-only Annotation is deferred and is not a missing v1 item.