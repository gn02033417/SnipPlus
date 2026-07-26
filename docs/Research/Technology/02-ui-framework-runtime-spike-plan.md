# UI Framework Runtime Spike Plan

狀態：`Draft`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | RESEARCH-TECH-UI-002 |
| Title | UI Framework Runtime Spike Plan |
| Status | Draft |
| Research Type | Runtime Evidence Plan |
| Execution Status | Not started |
| Runtime Verification | Not performed |
| Owner | TBD |
| Last reviewed | Not reviewed |
| Version | 0.1 |
| Research plan date | 2026-07-26 |
| Normative References | `docs/Research/Technology/01-ui-framework-feasibility.md`, `Architecture/ADR-BASELINE.md`, `Architecture/adr/ADR-0002-ui-framework-selection.md` |
| Informative References | `PRD/`, `Specs/`, `Architecture/` documents listed in [Traceability](#17-traceability) |
| Supersedes | None |
| Superseded by | None |

## 2. Purpose

This document converts the 11 future spikes in `RESEARCH-TECH-UI-001` into a consistent, repeatable and comparable execution plan.

It is intended to:

- Use equivalent test conditions for WinUI 3 and WPF.
- Fix evidence collection rules before any future prototype work begins.
- Prevent a future prototype from changing the evaluation criteria opportunistically.
- Produce runtime evidence that can be used in a later `ADR-0002` Review.

This document defines a plan only. It does not execute a spike or report a result.

## 3. Scope

### Primary execution candidates

- WinUI 3.
- WPF.

### Secondary candidates

Avalonia and Windows Forms are retained as alternatives but are not included in the first complete Runtime Spike round. If both primary candidates fail a blocking gate, a separate change or extension plan must be reviewed. This plan does not exclude either secondary candidate.

### Source evidence boundary

The plan is derived from the 11 spikes in `RESEARCH-TECH-UI-001` and preserves their names and scope. It does not add product features or change the evaluation criteria in the upstream research.

## 4. Non-goals

This document does not:

- Execute any spike.
- Write a prototype.
- Create a solution or project.
- Decide the UI Framework.
- Modify `ADR-0002`.
- Select a Runtime or Language version.
- Select a Capture, Rendering or Clipboard backend.
- Create production source code.
- Define a formal Project Structure.
- Set a product performance KPI that has not been approved through PRD or NFR.

## 5. Spike ID Policy

The 11 upstream spikes retain their original order and receive execution-plan IDs:

| Upstream research spike | Execution-plan ID |
| --- | --- |
| SPIKE-001 | UI-SPIKE-001 |
| SPIKE-002 | UI-SPIKE-002 |
| SPIKE-003 | UI-SPIKE-003 |
| SPIKE-004 | UI-SPIKE-004 |
| SPIKE-005 | UI-SPIKE-005 |
| SPIKE-006 | UI-SPIKE-006 |
| SPIKE-007 | UI-SPIKE-007 |
| SPIKE-008 | UI-SPIKE-008 |
| SPIKE-009 | UI-SPIKE-009 |
| SPIKE-010 | UI-SPIKE-010 |
| SPIKE-011 | UI-SPIKE-011 |

Rules:

- IDs are never reused.
- Existing spikes must not be deleted or merged in this plan.
- If the upstream spike count or scope is inconsistent, record `PLAN-GAP`; do not edit the upstream research automatically.
- Spike Status may use only: `Planned`, `Ready`, `Running`, `Blocked`, `Completed`, `Invalidated`.
- Every spike in this plan remains `Planned`.

## 6. Test Environment Record

The following fields are required when a future execution is authorized. No execution result is prefilled here.

| Field | Required value at execution |
| --- | --- |
| Windows edition | `TBD at execution` |
| Windows build | `TBD at execution` |
| Framework and exact version | `TBD at execution` |
| Runtime version | `TBD at execution` |
| CPU architecture | `x64` or `ARM64`, recorded at execution |
| GPU and driver | `TBD at execution` |
| Monitor count | `TBD at execution` |
| Monitor resolutions | `TBD at execution` |
| DPI scaling per monitor | `TBD at execution` |
| HDR state | `On`, `Off` or `Not available` |
| Packaging mode | `Packaged` or `Unpackaged` |
| Power mode | `TBD at execution` |
| Debug or Release configuration | `TBD at execution` |
| Test timestamp | `TBD at execution` |
| Environment record owner | `TBD at execution` |

The environment record must be attached to every future result document. It must not be inferred from another run.

## 7. Controlled Comparison Rules

- WinUI 3 and WPF must use the same hardware and Windows build for a direct comparison.
- The comparison prototype must implement equivalent minimal behavior for both frameworks.
- An extra optimization may not be added to only one framework and then treated as a framework difference.
- Debug and Release observations must not be mixed.
- Each measurement must record its execution count; the count remains `TBD` until authorized execution.
- First launch and repeated launch must be recorded separately.
- Subjective observations and measured values must be recorded in separate fields.
- Functional success does not imply performance success.
- Official documentation evidence does not replace an explicitly required Runtime Gate.
- A spike prototype must not silently become production code.
- Any change to the test condition requires a new plan revision or a recorded plan exception.
- A result must include enough information for another person to reproduce the same observation.

## 8. Evidence Types

The plan uses the following fixed evidence types:

- Functional observation
- Measured value
- Screenshot
- Screen recording
- Diagnostic log
- Environment record
- Accessibility inspection
- Deployment artifact evidence
- Failure reproduction

Each spike must identify:

- Required evidence.
- Optional evidence.
- Evidence that is explicitly insufficient by itself.

A prose statement such as “works as expected” is not sufficient evidence for a gate.

## 9. Measurement Classification

### Functional Gate

Functional gate outcomes may be:

- `Pass`
- `Fail`
- `Blocked`
- `Not executed`

### Comparative Metric

For latency, memory, input-update behavior or another comparative metric:

- Record the actual measured values.
- Use equivalent measurement boundaries for WinUI 3 and WPF.
- Do not declare a product-level pass without an approved threshold.

### Threshold-dependent Metric

If PRD or NFR has not approved a threshold, record:

- `Threshold: TBD`
- `Decision use: Informative only`

This plan does not invent values such as `100 ms` or `150 MB`.

## 10. Spike Catalog

Each spike has the same fixed fields:

- Spike ID
- Title
- Status
- Related UIF criteria
- Related UI Gate
- Decision driver
- Frameworks compared
- Environment variations
- Purpose
- Preconditions
- Minimal behavior under test
- Execution steps
- Evidence required
- Measurements
- Functional pass condition
- Comparative observations
- Failure condition
- Failure implication
- Known limitations
- Safety and cleanup
- Result document destination
- Open questions

The execution steps describe future test actions only. They contain no source code and do not define a production feature.

### UI-SPIKE-001 — Virtual desktop overlay

| Field | Plan |
| --- | --- |
| Spike ID | UI-SPIKE-001 |
| Title | Virtual desktop overlay |
| Status | Planned |
| Related UIF criteria | UIF-001, UIF-004, UIF-005, UIF-014 |
| Related UI Gate | UI-GATE-001, UI-GATE-002 |
| Decision driver | Native Windows desktop capability; basic workflow; platform integration |
| Frameworks compared | WinUI 3 and WPF |
| Environment variations | One display; multiple displays; different display arrangements; x64 and ARM64 when available |
| Purpose | Compare one overlay spanning the virtual desktop with one overlay per display. |
| Preconditions | Test environment record is complete; equivalent minimal overlay behavior is authorized; no production project exists. |
| Minimal behavior under test | Show a bounded test surface over the selected display arrangement and observe its placement and visibility. |
| Execution steps | Record display geometry; run the same coverage case for each framework; repeat for one-display and multi-display arrangements; record exit and cleanup behavior. |
| Evidence required | Environment record; functional observation; screenshot; diagnostic log if placement fails; failure reproduction where applicable. |
| Measurements | Window bounds; display bounds; z-order observation; repeated-run consistency. |
| Functional pass condition | The planned coverage arrangement remains stable and the test surface appears at the intended display bounds. |
| Comparative observations | `TBD after execution`; record WinUI 3 and WPF separately without naming a winner. |
| Failure condition | The surface misses a display, drifts from recorded bounds or cannot be consistently shown. |
| Failure implication | UI-GATE-001 or UI-GATE-002 remains open for the affected framework. |
| Known limitations | This does not prove capture-image composition, click-through behavior or product focus policy. |
| Safety and cleanup | Use a disposable test surface; close it after each run; do not alter system display settings without authorization. |
| Result document destination | `docs/Research/Technology/results/ui-framework/UI-SPIKE-001-result.md` |
| Open questions | Is the product boundary one virtual-desktop surface or one surface per display? |

### UI-SPIKE-002 — Borderless transparent composition

| Field | Plan |
| --- | --- |
| Spike ID | UI-SPIKE-002 |
| Title | Borderless transparent composition |
| Status | Planned |
| Related UIF criteria | UIF-001, UIF-002, UIF-003, UIF-014 |
| Related UI Gate | UI-GATE-001 |
| Decision driver | Native Windows desktop capability; Fluent-first presentation; platform integration |
| Frameworks compared | WinUI 3 and WPF |
| Environment variations | One display; multiple displays; normal desktop; active foreground application; HDR state recorded but not changed by this plan |
| Purpose | Verify borderless, topmost and transparent composition as a combined condition. |
| Preconditions | UI-SPIKE-001 display bounds are known; environment record is complete; no final rendering backend is selected. |
| Minimal behavior under test | Show a borderless test surface with the planned transparency condition and observe composition and input routing. |
| Execution steps | Start the disposable surface; record border and title-bar state; place it over representative desktop content; observe topmost and input behavior; close and repeat for the comparison framework. |
| Evidence required | Functional observation; screenshot; screen recording for composition changes; diagnostic log for failure; environment record. |
| Measurements | Surface bounds; visibility state; activation time boundary; repeatability count recorded at execution. |
| Functional pass condition | The test surface exhibits the required borderless/topmost/transparent combination without an unplanned interaction side effect. |
| Comparative observations | `TBD after execution`; record documented differences without selecting a framework. |
| Failure condition | Transparency is unavailable, composition is incorrect, topmost behavior is unstable or input routing is undefined. |
| Failure implication | UI-GATE-001 remains open and the affected framework may be stopped under the plan Stop Rules. |
| Known limitations | This does not prove per-pixel alpha, capture quality or production accessibility. |
| Safety and cleanup | Do not leave a topmost surface running; restore focus and close the disposable surface after each run. |
| Result document destination | `docs/Research/Technology/results/ui-framework/UI-SPIKE-002-result.md` |
| Open questions | Is partial transparency sufficient, or is per-pixel alpha required? |

### UI-SPIKE-003 — DPI scale matrix

| Field | Plan |
| --- | --- |
| Spike ID | UI-SPIKE-003 |
| Title | DPI scale matrix |
| Status | Planned |
| Related UIF criteria | UIF-004, UIF-005, UIF-010, UIF-011 |
| Related UI Gate | UI-GATE-002, UI-GATE-005 |
| Decision driver | Basic workflow; accessibility; maintainable coordinate behavior |
| Frameworks compared | WinUI 3 and WPF |
| Environment variations | 100%, 125%, 150% and 200% scaling; one display; scale change before and during a test case when available |
| Purpose | Verify coordinate fidelity across the required scale-factor matrix. |
| Preconditions | The selected test environment can record the active scale; measurement boundary is approved; no product threshold is invented. |
| Minimal behavior under test | Display a known geometry and observe pointer, surface, image and annotation-coordinate relationships. |
| Execution steps | Record initial scale; run the same geometry case at each scale; record pointer and visual coordinates; repeat after a scale change if the environment supports it. |
| Evidence required | Environment record; measured values; screenshot; functional observation; failure reproduction for drift. |
| Measurements | Scale factor; expected and observed coordinates; measured drift; number of repetitions. |
| Functional pass condition | The agreed coordinate relationship remains stable; the actual tolerance remains `TBD` until approved. |
| Comparative observations | `TBD after execution`; compare coordinate behavior only under identical conditions. |
| Failure condition | Scale changes cause unrecorded geometry drift, selection mismatch or hit-test mismatch. |
| Failure implication | UI-GATE-002 or UI-GATE-005 remains open; no framework decision is made here. |
| Known limitations | No product-level pixel tolerance has been approved. |
| Safety and cleanup | Do not alter permanent display settings without authorization; restore the test display configuration after execution. |
| Result document destination | `docs/Research/Technology/results/ui-framework/UI-SPIKE-003-result.md` |
| Open questions | What coordinate error tolerance should be approved by PRD or NFR? |

### UI-SPIKE-004 — Heterogeneous DPI multi-monitor

| Field | Plan |
| --- | --- |
| Spike ID | UI-SPIKE-004 |
| Title | Heterogeneous DPI multi-monitor |
| Status | Planned |
| Related UIF criteria | UIF-004, UIF-005, UIF-006, UIF-007 |
| Related UI Gate | UI-GATE-002, UI-GATE-004 |
| Decision driver | Basic workflow; native Windows capability; focus and input boundary |
| Frameworks compared | WinUI 3 and WPF |
| Environment variations | At least two displays with different scale factors; display order and primary-display variations where available |
| Purpose | Observe capture activation or selection transition across displays with different DPI scales. |
| Preconditions | UI-SPIKE-003 scale observations exist; two-display environment is available; active-selection behavior is authorized for testing. |
| Minimal behavior under test | Move or activate the disposable selection surface across displays and record its geometry and focus state. |
| Execution steps | Record monitor layout and scale; start the same test case on each display; move the pointer or active surface across the boundary; record cancellation, focus and coordinate behavior. |
| Evidence required | Environment record; screenshot; screen recording; measured values; failure reproduction. |
| Measurements | Display bounds; scale factors; pointer and surface coordinates before and after transition; focus owner. |
| Functional pass condition | Selection and visual bounds remain aligned across the tested display transition. |
| Comparative observations | `TBD after execution`; keep monitor layout and scale identical between framework runs. |
| Failure condition | The active surface becomes misaligned, loses required input without a documented transition or fails to restore focus. |
| Failure implication | UI-GATE-002 or UI-GATE-004 remains open. |
| Known limitations | This does not select a multi-monitor product policy or capture backend. |
| Safety and cleanup | Stop the test if focus is lost unexpectedly; close the disposable surface and restore the original foreground application. |
| Result document destination | `docs/Research/Technology/results/ui-framework/UI-SPIKE-004-result.md` |
| Open questions | Is display transition during an active selection required in the first product version? |

### UI-SPIKE-005 — Capture-entry latency

| Field | Plan |
| --- | --- |
| Spike ID | UI-SPIKE-005 |
| Title | Capture-entry latency |
| Status | Planned |
| Related UIF criteria | UIF-007, UIF-009, UIF-017 |
| Related UI Gate | UI-GATE-003 |
| Decision driver | Basic workflow; fast before features; maintainability |
| Frameworks compared | WinUI 3 and WPF |
| Environment variations | Cold start; warm start; Debug and Release recorded separately; x64 and ARM64 when available |
| Purpose | Measure the path from a capture-entry signal to an interactive overlay-ready state. |
| Preconditions | An authorized entry signal and equivalent minimal test surface exist; timestamp boundaries are agreed; no product KPI is assumed. |
| Minimal behavior under test | Receive the test entry signal and show an interactive disposable surface. |
| Execution steps | Define and record input, process-start, surface-ready and first-interaction timestamps; run cold and warm cases; repeat for both frameworks under the same configuration. |
| Evidence required | Measured value; diagnostic log; environment record; functional observation; failure reproduction. |
| Measurements | Input-to-ready duration; ready-to-first-interaction duration; run count; cold/warm classification. |
| Functional pass condition | The surface reaches the defined ready state in every executed run; numeric threshold remains `TBD`. |
| Comparative observations | `TBD after execution`; record distributions and outliers without declaring a product pass. |
| Failure condition | Timestamp boundaries cannot be reproduced, the surface is not interactive or a run is missing required evidence. |
| Failure implication | UI-GATE-003 remains open; results are informative only until a threshold is approved. |
| Known limitations | No final global shortcut or capture backend is selected. |
| Safety and cleanup | Do not register a persistent shortcut; stop and remove all temporary test registrations after the run. |
| Result document destination | `docs/Research/Technology/results/ui-framework/UI-SPIKE-005-result.md` |
| Open questions | Which product or NFR document will define an acceptable latency threshold? |

### UI-SPIKE-006 — Focus lifecycle

| Field | Plan |
| --- | --- |
| Spike ID | UI-SPIKE-006 |
| Title | Focus lifecycle |
| Status | Planned |
| Related UIF criteria | UIF-006, UIF-008, UIF-014 |
| Related UI Gate | UI-GATE-004 |
| Decision driver | Windows muscle memory; basic workflow; accessibility |
| Frameworks compared | WinUI 3 and WPF |
| Environment variations | Capture start; normal completion; cancel; pointer cancellation; foreground application with keyboard focus |
| Purpose | Verify focus acquisition, cancellation and restoration through every planned exit path. |
| Preconditions | A disposable test surface and a known foreground application are available; focus observation method is agreed. |
| Minimal behavior under test | Acquire focus for the test surface, accept a test interaction, exit through each path and observe the focus owner. |
| Execution steps | Record focus owner before entry; enter the test surface; exercise completion and cancellation separately; record focus owner after exit; repeat for both frameworks. |
| Evidence required | Functional observation; screen recording; diagnostic log or focus inspection; environment record; failure reproduction. |
| Measurements | Focus owner before, during and after each path; time to restored focus if measurable; path classification. |
| Functional pass condition | The original focus owner is restored for every tested exit path, or an approved product rule explains the difference. |
| Comparative observations | `TBD after execution`; record behavior and failure paths separately. |
| Failure condition | Focus is lost, remains on the disposable surface or cannot be reproduced for an exit path. |
| Failure implication | UI-GATE-004 remains open; stop follow-on interaction spikes for the affected framework if required by Stop Rules. |
| Known limitations | This does not define final global hotkey ownership or accessibility acceptance. |
| Safety and cleanup | Never leave the test surface focused or topmost after a run; restore the original application manually if necessary. |
| Result document destination | `docs/Research/Technology/results/ui-framework/UI-SPIKE-006-result.md` |
| Open questions | Which cancellation paths are mandatory for the first product version? |

### UI-SPIKE-007 — High-frequency pointer movement

| Field | Plan |
| --- | --- |
| Spike ID | UI-SPIKE-007 |
| Title | High-frequency pointer movement |
| Status | Planned |
| Related UIF criteria | UIF-008, UIF-009, UIF-010, UIF-011 |
| Related UI Gate | UI-GATE-003, UI-GATE-004, UI-GATE-005 |
| Decision driver | Basic workflow; pointer input; responsive interaction |
| Frameworks compared | WinUI 3 and WPF |
| Environment variations | Slow movement; fast movement; diagonal movement; pointer capture; display change and cancellation case where available |
| Purpose | Verify pointer capture, selection updates and cancellation handling during fast movement. |
| Preconditions | Equivalent selection geometry is defined; input path is authorized; no final rendering backend is selected. |
| Minimal behavior under test | Receive pointer movement while a disposable selection is active and update the visible selection boundary. |
| Execution steps | Start a selection; perform controlled slow and fast movement; record event continuity and visual updates; repeat with cancellation or capture loss when available. |
| Evidence required | Screen recording; diagnostic log; measured value; functional observation; failure reproduction. |
| Measurements | Pointer event count where available; rendered update observations; lag or boundary drift measurement; cancellation classification. |
| Functional pass condition | No unacceptable event loss, lag or boundary drift under the approved test condition; threshold remains `TBD`. |
| Comparative observations | `TBD after execution`; compare the same movement path and display configuration. |
| Failure condition | Pointer capture is lost without a defined path, selection updates stop unexpectedly or boundary drift is not reproducible. |
| Failure implication | UI-GATE-003, UI-GATE-004 or UI-GATE-005 remains open as applicable. |
| Known limitations | No formal pointer rate or frame-rate target is approved. |
| Safety and cleanup | Stop movement if the surface becomes unresponsive; close the disposable surface and restore focus. |
| Result document destination | `docs/Research/Technology/results/ui-framework/UI-SPIKE-007-result.md` |
| Open questions | Which pointer devices must be included in the first execution round? |

### UI-SPIKE-008 — Selection rectangle rendering

| Field | Plan |
| --- | --- |
| Spike ID | UI-SPIKE-008 |
| Title | Selection rectangle rendering |
| Status | Planned |
| Related UIF criteria | UIF-005, UIF-008, UIF-010, UIF-011 |
| Related UI Gate | UI-GATE-002, UI-GATE-005 |
| Decision driver | Basic capture workflow; coordinate fidelity; drawing suitability |
| Frameworks compared | WinUI 3 and WPF |
| Environment variations | 100%, 125%, 150% and 200% DPI; one display and multi-display; small and large selections |
| Purpose | Verify selection geometry against the displayed desktop image and intended output bounds. |
| Preconditions | A known test image or desktop reference is available; coordinate recording is defined; the output is observational only. |
| Minimal behavior under test | Show a disposable selection rectangle that follows pointer input and records its bounds. |
| Execution steps | Start at known coordinates; drag through representative sizes; record displayed and reported bounds; repeat across scale factors and both frameworks. |
| Evidence required | Screenshot; screen recording; measured values; environment record; functional observation. |
| Measurements | Start/end pointer coordinates; rectangle bounds; scale factor; observed drift; repetition count. |
| Functional pass condition | The rectangle follows the pointer and maps to the intended output bounds under the approved tolerance. |
| Comparative observations | `TBD after execution`; compare geometry and repaint behavior without selecting a rendering backend. |
| Failure condition | The rectangle does not follow input, reports incorrect bounds or diverges between display and output coordinates. |
| Failure implication | UI-GATE-002 or UI-GATE-005 remains open; UI-SPIKE-009 must not be treated as complete. |
| Known limitations | This is not a final annotation design and does not select a capture implementation. |
| Safety and cleanup | Use a disposable reference surface; close it after each run; do not save or distribute captured content. |
| Result document destination | `docs/Research/Technology/results/ui-framework/UI-SPIKE-008-result.md` |
| Open questions | What output-boundary definition will be used for product acceptance? |

### UI-SPIKE-009 — Annotation object hit testing

| Field | Plan |
| --- | --- |
| Spike ID | UI-SPIKE-009 |
| Title | Annotation object hit testing |
| Status | Planned |
| Related UIF criteria | UIF-010, UIF-011, UIF-012 |
| Related UI Gate | UI-GATE-005, UI-GATE-007 |
| Decision driver | Optional annotation; accessibility; maintainability |
| Frameworks compared | WinUI 3 and WPF |
| Environment variations | One representative object; overlapping objects; different DPI scales; pointer and keyboard targeting where available |
| Purpose | Verify that representative disposable annotation objects can be targeted, selected and edited predictably. |
| Preconditions | The test object set and interaction sequence are defined without becoming a product feature design; scale conditions are recorded. |
| Minimal behavior under test | Display a small set of disposable visual objects and observe hit testing and selection state. |
| Execution steps | Place non-persistent test objects; target each object and overlap case; record hit result and edit-state transition; repeat at required scales. |
| Evidence required | Functional observation; screenshot; screen recording; accessibility inspection where applicable; failure reproduction. |
| Measurements | Hit target coordinates; selected object; z-order result; scale factor; repeatability. |
| Functional pass condition | Object targeting is predictable under the defined test cases and does not rely on undocumented behavior. |
| Comparative observations | `TBD after execution`; record framework-specific behavior and known gaps only. |
| Failure condition | Incorrect object is selected, transparent/overlapping objects produce inconsistent results or the state cannot be inspected. |
| Failure implication | UI-GATE-005 or UI-GATE-007 remains open; annotation feasibility remains unproven. |
| Known limitations | This does not define annotation tools, serialization, storage or final UI. |
| Safety and cleanup | Use non-persistent objects; close the test surface; do not write user data or application files. |
| Result document destination | `docs/Research/Technology/results/ui-framework/UI-SPIKE-009-result.md` |
| Open questions | Which annotation object types are necessary to establish feasibility? |

### UI-SPIKE-010 — Architecture distribution

| Field | Plan |
| --- | --- |
| Spike ID | UI-SPIKE-010 |
| Title | Architecture distribution |
| Status | Planned |
| Related UIF criteria | UIF-015, UIF-016, UIF-018 |
| Related UI Gate | UI-GATE-006 |
| Decision driver | Deployment flexibility; maintainability; Windows platform coverage |
| Frameworks compared | WinUI 3 and WPF |
| Environment variations | x64; ARM64 when a test device is available; framework-dependent or self-contained mode recorded but not selected here |
| Purpose | Compare architecture-specific deployment outputs and startup compatibility. |
| Preconditions | A future approved packaging experiment defines its artifact boundary; x64 and ARM64 environments are recorded separately. |
| Minimal behavior under test | Launch the disposable feasibility artifact on each available target architecture and record dependencies. |
| Execution steps | Record architecture; inspect the planned deployment artifact; launch under the same configuration; record startup and missing-dependency behavior; do not publish a product artifact. |
| Evidence required | Environment record; deployment artifact evidence; functional observation; diagnostic log; failure reproduction. |
| Measurements | Artifact size; dependency inventory; startup result; architecture; launch time if authorized. |
| Functional pass condition | The authorized artifact launches through the documented path on every required architecture. |
| Comparative observations | `TBD after execution`; do not treat artifact size as a product threshold. |
| Failure condition | Required architecture cannot launch, dependencies are undocumented or the artifact cannot be reproduced. |
| Failure implication | UI-GATE-006 remains open and UI-SPIKE-011 may require a revised dependency plan. |
| Known limitations | Packaging mode, update strategy and product architecture support are not selected by this plan. |
| Safety and cleanup | Use disposable artifacts; do not install system-wide dependencies without explicit authorization; remove temporary artifacts after the run. |
| Result document destination | `docs/Research/Technology/results/ui-framework/UI-SPIKE-010-result.md` |
| Open questions | Is ARM64 a first-version requirement or a later distribution target? |

### UI-SPIKE-011 — Packaged and unpackaged startup

| Field | Plan |
| --- | --- |
| Spike ID | UI-SPIKE-011 |
| Title | Packaged and unpackaged startup |
| Status | Planned |
| Related UIF criteria | UIF-015, UIF-016, UIF-018 |
| Related UI Gate | UI-GATE-006 |
| Decision driver | Deployment complexity; lightweight desktop utility; servicing |
| Frameworks compared | WinUI 3 and WPF |
| Environment variations | Packaged; unpackaged; offline start; clean environment; x64 and ARM64 when available |
| Purpose | Compare startup dependencies and servicing assumptions for packaged and unpackaged feasibility artifacts. |
| Preconditions | A future deployment experiment has an approved artifact boundary and does not change the framework decision; environment record is complete. |
| Minimal behavior under test | Install or place a disposable artifact through the recorded path and start it without a product installer. |
| Execution steps | Record packaging mode; record required dependencies; start online and offline where authorized; record initialization and failure behavior; repeat for both frameworks. |
| Evidence required | Environment record; deployment artifact evidence; functional observation; diagnostic log; failure reproduction. |
| Measurements | Startup result; dependency count; artifact size; initialization time if authorized; offline result. |
| Functional pass condition | The approved path starts without an unplanned dependency and its servicing implications are recorded. |
| Comparative observations | `TBD after execution`; record deployment trade-offs without selecting a mode. |
| Failure condition | Startup depends on an undocumented runtime, requires unauthorized machine changes or cannot be repeated offline as planned. |
| Failure implication | UI-GATE-006 remains open; deployment evidence is insufficient for `ADR-0002` acceptance. |
| Known limitations | This does not choose MSIX, installer, update strategy or Windows App SDK version. |
| Safety and cleanup | Do not perform system-wide installation without authorization; remove disposable packages and restore the test environment. |
| Result document destination | `docs/Research/Technology/results/ui-framework/UI-SPIKE-011-result.md` |
| Open questions | Which offline and servicing requirements will be approved for the product? |

## 11. Required Coverage Matrix

The matrix identifies planned coverage only. `Planned` is not an execution result.

| Spike | UIF criteria | UI Gate | WinUI 3 | WPF | Evidence required |
| --- | --- | --- | --- | --- | --- |
| UI-SPIKE-001 | UIF-001, UIF-004, UIF-005, UIF-014 | UI-GATE-001, UI-GATE-002 | Planned | Planned | Environment record; functional observation; bounds evidence |
| UI-SPIKE-002 | UIF-001, UIF-002, UIF-003, UIF-014 | UI-GATE-001 | Planned | Planned | Functional observation; screenshot; screen recording; failure reproduction |
| UI-SPIKE-003 | UIF-004, UIF-005, UIF-010, UIF-011 | UI-GATE-002, UI-GATE-005 | Planned | Planned | Environment record; measured values; screenshots; failure reproduction |
| UI-SPIKE-004 | UIF-004, UIF-005, UIF-006, UIF-007 | UI-GATE-002, UI-GATE-004 | Planned | Planned | Environment record; screen recording; focus and coordinate evidence |
| UI-SPIKE-005 | UIF-007, UIF-009, UIF-017 | UI-GATE-003 | Planned | Planned | Timestamps; diagnostic log; environment record; failure reproduction |
| UI-SPIKE-006 | UIF-006, UIF-008, UIF-014 | UI-GATE-004 | Planned | Planned | Focus observations; screen recording; failure reproduction |
| UI-SPIKE-007 | UIF-008, UIF-009, UIF-010, UIF-011 | UI-GATE-003, UI-GATE-004, UI-GATE-005 | Planned | Planned | Screen recording; diagnostics; measured values; functional observation |
| UI-SPIKE-008 | UIF-005, UIF-008, UIF-010, UIF-011 | UI-GATE-002, UI-GATE-005 | Planned | Planned | Geometry measurements; screenshots; screen recording |
| UI-SPIKE-009 | UIF-010, UIF-011, UIF-012 | UI-GATE-005, UI-GATE-007 | Planned | Planned | Hit-test observation; screenshots; accessibility inspection |
| UI-SPIKE-010 | UIF-015, UIF-016, UIF-018 | UI-GATE-006 | Planned | Planned | Deployment artifact evidence; environment record; diagnostic log |
| UI-SPIKE-011 | UIF-015, UIF-016, UIF-018 | UI-GATE-006 | Planned | Planned | Deployment artifact evidence; offline observation; failure reproduction |

Coverage rule:

- `UIF-001` through `UIF-018` must be covered by at least one spike or explicit upstream official evidence.
- `UI-GATE-001` through `UI-GATE-007` must have a corresponding execution path or be marked `PLAN-GAP`.
- No uncovered criterion is assumed to be covered.

## 12. Execution Order

The following order is a risk-oriented proposal only.

### Phase 1 — Blocking Windowing Feasibility

| Item | Entry criteria | Exit criteria | Blocking condition |
| --- | --- | --- | --- |
| Windowing and composition | Environment record complete; both candidate conditions defined | UI-SPIKE-001 and UI-SPIKE-002 have reproducible evidence or an explicit failure record | Transparent/topmost or display coverage cannot be established |
| Multi-monitor and DPI | Windowing observations recorded; scale matrix available | UI-SPIKE-003 and UI-SPIKE-004 have coordinate and transition evidence | Mixed-DPI behavior is not reproducible or remains misaligned |
| Focus and input | Windowing path is stable enough to receive input | UI-SPIKE-006 has an exit-path record | Focus cannot be acquired/restored or input ownership is undefined |

### Phase 2 — Interaction and Rendering Feasibility

| Item | Entry criteria | Exit criteria | Blocking condition |
| --- | --- | --- | --- |
| Activation latency | Entry path is reproducible | UI-SPIKE-005 has timestamp evidence | Measurement boundary cannot be reproduced |
| Pointer movement | Input and focus path is stable | UI-SPIKE-007 has event and visual evidence | Selection becomes unresponsive or drifts without explanation |
| Selection rendering | DPI and pointer evidence are available | UI-SPIKE-008 has geometry evidence | Selection bounds cannot map to intended output |
| Annotation hit testing | Selection surface is sufficiently stable | UI-SPIKE-009 has object-targeting evidence | Object targeting is not predictable or inspectable |

### Phase 3 — Delivery Feasibility

| Item | Entry criteria | Exit criteria | Blocking condition |
| --- | --- | --- | --- |
| Architecture distribution | A disposable artifact boundary is approved | UI-SPIKE-010 has architecture evidence | Required architecture cannot launch or be reproduced |
| Packaged/unpackaged startup | Architecture artifact is reproducible | UI-SPIKE-011 has dependency and startup evidence | Unplanned runtime or machine dependency is required |
| Accessibility inspection | Interaction surface is stable enough to inspect | Accessibility evidence is attached to the relevant result | Required baseline cannot be evaluated |

## 13. Stop Rules

During a future authorized execution:

- If a framework cannot pass the transparent-overlay Blocking Gate, stop its later non-essential spikes and record the reason.
- If execution reveals a required change to frozen PRD, Specs or Architecture, stop and record the dependency; do not silently continue.
- If a Capture or Rendering backend must be selected to continue, record the dependency; do not make that decision inside a spike.
- If a result cannot be reproduced, do not mark the spike `Completed`.
- If test environments differ, do not make a direct framework comparison.
- If a test changes system-wide settings or installs a dependency unexpectedly, stop and restore the environment before continuing.
- If a safety or privacy boundary is unclear, stop and request a separate decision.

## 14. Result Artifact Plan

This task creates no result document and no result directory.

Future results are planned for:

`docs/Research/Technology/results/ui-framework/`

Each spike may later produce:

- `UI-SPIKE-001-result.md`
- `UI-SPIKE-002-result.md`
- `UI-SPIKE-003-result.md`
- `UI-SPIKE-004-result.md`
- `UI-SPIKE-005-result.md`
- `UI-SPIKE-006-result.md`
- `UI-SPIKE-007-result.md`
- `UI-SPIKE-008-result.md`
- `UI-SPIKE-009-result.md`
- `UI-SPIKE-010-result.md`
- `UI-SPIKE-011-result.md`

No result file or execution evidence is created by this plan.

## 15. Decision Evidence Roll-up

The following table defines the future roll-up shape. Empty result cells are intentional and must remain empty until execution.

| UI Gate | WinUI 3 result | WPF result | Evidence completeness | ADR impact |
| --- | --- | --- | --- | --- |
| UI-GATE-001 |  |  |  |  |
| UI-GATE-002 |  |  |  |  |
| UI-GATE-003 |  |  |  |  |
| UI-GATE-004 |  |  |  |  |
| UI-GATE-005 |  |  |  |  |
| UI-GATE-006 |  |  |  |  |
| UI-GATE-007 |  |  |  |  |

When populated by a future result review, `ADR impact` may use only:

- `Supports proposed decision`
- `Challenges proposed decision`
- `Neutral`
- `Insufficient evidence`

The roll-up must not change `ADR-0002` automatically.

## 16. Readiness to Execute

**Selected status:** `Not ready`

Allowed values are:

- `Ready for runtime spike execution`
- `Conditionally ready`
- `Not ready`

The selected status is `Not ready` because the exact framework and runtime versions, test hardware, packaging experiment boundary and execution authority have not been established. This plan is complete as a document, but it does not authorize execution.

## 17. Traceability

| Source | Relevance |
| --- | --- |
| `docs/Research/Technology/01-ui-framework-feasibility.md` | Upstream UIF criteria, UI Gates and 11 spike definitions. |
| `Architecture/adr/ADR-0002-ui-framework-selection.md` | ADR whose evidence gap is being planned for later review. |
| `Architecture/ADR-BASELINE.md` | ADR lifecycle, required evidence and review boundaries. |
| `Architecture/TECHNOLOGY-DECISION-ROADMAP.md` | TD-001 UI Framework and related technology decision dependencies. |
| `PRD/PRD-0002-user-experience-principles.md` | Windows-first, Fluent-first and muscle-memory principles. |
| `PRD/PRD-0003-product-vision.md` | Product direction and long-term maintenance intent. |
| `PRD/PRD-0004-core-workflow.md` | Capture entry, selection, completion and cancellation workflow. |
| `PRD/PRD-0006-non-functional-requirements.md` | Performance, accessibility, compatibility and maintainability constraints. |
| `Specs/SPEC-0005-capture-workflow.md` | Capture workflow states and selection behavior. |
| `Specs/SPEC-0009-annotation-capability.md` | Optional annotation and drawing boundary. |
| `Specs/SPEC-0010-feature-integration.md` | Cross-feature responsibility and integration boundaries. |
| `Architecture/ARCH-0002-layer-model.md` | Layer and platform-integration separation. |
| `Architecture/ARCH-0004-component-boundaries.md` | Display, input and shared-state ownership. |
| `Architecture/ARCH-0005-component-interactions.md` | Interaction and failure-propagation constraints. |
| `Architecture/ARCH-BASELINE-REVIEW.md` | Architecture freeze and technology-decision entry criteria. |

## 18. Open Questions

- What is the minimum Windows 11 build for authorized spike execution?
- Which exact WinUI 3 and WPF versions should be compared?
- Are both x64 and ARM64 test devices available?
- Is a physical HDR display required for the first execution round?
- What is the order of packaged and unpackaged testing?
- Does Capture Entry require a separate ADR before execution?
- How can Rendering Backend concerns be kept from contaminating the UI Framework comparison?
- Which PRD or NFR source will define acceptable latency or resource thresholds?
- Which accessibility inspection tools are authorized?
- Must result artifacts preserve screen recordings and diagnostic logs, and for how long?
- Who authorizes creation and cleanup of disposable spike artifacts?

## 19. Completion Boundary

This plan is complete when the 11 spikes, environment record, controlled comparison rules, evidence types, fixed spike fields, coverage matrix, execution order, stop rules, result artifact plan, roll-up, readiness status, traceability and open questions are documented.

Completion of this plan does not mean any spike is `Ready`, `Running` or `Completed`. It does not make `ADR-0002` Accepted.

## 20. Prohibited Actions

Until a separate task explicitly authorizes execution, do not:

- Run a Runtime Spike.
- Build or run a prototype.
- Create a result directory or result document.
- Modify `ADR-0002`.
- Create a Rendering or Capture ADR.
- Create a Project Structure.
- Choose a Framework, Runtime, Capture or Rendering technology.
- Convert a disposable spike artifact into product source code.

## 21. Review Boundary

This is a Runtime Spike Execution Plan, not a Runtime Spike result report. Its planned conditions and fields must be reviewed before any execution. Future results must be written separately and then used in a new evidence review of `ADR-0002`.
