# Windows 11 Snipping Tool — Capture Workflow

狀態：`Draft`

## Research metadata

| Field | Value |
| --- | --- |
| Subject | Windows 11 Snipping Tool static image capture |
| Platform | Windows 11 |
| Snipping Tool version | `UNKNOWN` — Microsoft Support page does not identify the app build |
| Research date | 2026-07-26 |
| Verification | Official Microsoft Support documentation review |
| Runtime verification | Not performed in this task |
| Scope | Entry, mode selection, area selection, capture completion, notification, clipboard and editor handoff |
| Out of scope | UI appearance, competitor comparison, SnipPlus design, implementation |

## Sources

| Source | Type | Published | Accessed | Version | URL |
| --- | --- | --- | --- | --- | --- |
| Use Snipping Tool to capture screenshots | Official Microsoft Support | `UNKNOWN` | 2026-07-26 | Applies to Windows 11 and Windows 10; app build `UNKNOWN` | <https://support.microsoft.com/en-US/Windows/Apps/use-snipping-tool-to-capture-screenshots> |
| Copy the window or screen contents | Official Microsoft Support | `UNKNOWN` | 2026-07-26 | `UNKNOWN` | <https://support.microsoft.com/en-US/Office/copy-the-window-or-screen-contents> |
| Keyboard shortcuts in Windows | Official Microsoft Support | `UNKNOWN` | 2026-07-26 | Applies to Windows 11 and Windows 10 | <https://support.microsoft.com/en-us/windows/keyboard-shortcuts-in-windows-dcc61a57-8ff0-cffe-9796-cb9706c75eec> |

## Source-backed workflow

The following records only behaviors described by the sources above. It is not a design for SnipPlus.

### 1. Enter the capture flow

| Field | Observation |
| --- | --- |
| Purpose | Start a static screenshot capture. |
| Trigger | Microsoft documents opening Snipping Tool from Start, pressing `Windows logo key + Shift + S`, or pressing `PrtSc`. |
| UI State | For `Windows logo key + Shift + S`, Microsoft describes a snipping overlay; another Microsoft page describes the desktop darkening so a region can be selected. Exact visual appearance is not recorded here. |
| User Action | Use one of the documented entry points. |
| System Response | The capture flow becomes available. The exact transition timing is `UNKNOWN` without runtime verification. |
| Exit Condition | A capture mode can be selected, or the direct `PrtSc` behavior applies. |

### 2. Select a capture mode

| Field | Observation |
| --- | --- |
| Purpose | Choose what portion of the screen will be captured. |
| Trigger | Snipping Tool is open or the snipping overlay is invoked. |
| UI State | Microsoft documents four image modes: Rectangle, Window, Full screen and Freeform. |
| User Action | Select a mode; the Support instructions also describe selecting `New` in the app flow. |
| System Response | Rectangle and Freeform require an area selection. Microsoft states that Full screen captures immediately; a separate Microsoft page states Window and Full screen capture immediately. |
| Exit Condition | The selected mode either starts area selection or produces a capture. |

### 3. Select an area when required

| Field | Observation |
| --- | --- |
| Purpose | Define the image bounds for a Rectangle or Freeform capture. |
| Trigger | Rectangle or Freeform mode is selected. |
| UI State | Microsoft describes the screen changing slightly to gray and describes click-and-drag for rectangular or free-form selection. Exact cursor, border, coordinate and focus behavior are `UNKNOWN`. |
| User Action | Click and drag around the target area. |
| System Response | A screenshot is produced when the selection completes, according to the Microsoft workflow. |
| Exit Condition | The selection is completed, or cancellation behavior occurs; exact cancellation behavior is `UNKNOWN`. |

### 4. Capture completion

| Field | Observation |
| --- | --- |
| Purpose | Produce the screenshot result. |
| Trigger | Full-screen or Window capture starts immediately, or Rectangle / Freeform drag completes. |
| UI State | The resulting snip is available to Snipping Tool. The exact editor activation timing is not independently runtime-verified. |
| User Action | None required for the documented automatic copy step. |
| System Response | Microsoft states that the screenshot is automatically copied to the clipboard and that a notification appears. Selecting the notification opens the image in the Snipping Tool editor. |
| Exit Condition | The user can paste from the clipboard, open the editor, save, copy, share or annotate. |

### 5. Optional editor handoff

| Field | Observation |
| --- | --- |
| Purpose | Continue working with the captured image. |
| Trigger | Select the completion notification, or use the Snipping Tool editor flow. |
| UI State | The image is open in the editor. This document does not describe the editor's visual appearance. |
| User Action | Microsoft documents optional annotation, crop, Text actions, undo/redo, copy, save and share actions. |
| System Response | The selected action changes or delivers the snip. Exact success and failure feedback are `UNKNOWN`. |
| Exit Condition | Copy, save, share or close; exact close behavior is `UNKNOWN`. |

## Direct keyboard paths

| Entry | Source-backed result | Confidence |
| --- | --- | --- |
| `Windows logo key + Shift + S` | Select a region to capture a screenshot to the clipboard; the Snipping Tool can then be opened for sharing or markup. | High, documentation only |
| `Windows logo key + Shift + R` | Opens the Snipping Tool overlay to capture a video clip. This is outside the static-image workflow in this document. | High, documentation only |
| `PrtSc` | One Microsoft page describes a static full-screen snapshot placed in the clipboard. Another keyboard-shortcut page describes `PrtScn` as selecting a region and notes configurable behavior. This conflict requires runtime verification. | `UNKNOWN` |
| `Windows logo key + PrtScn` | Microsoft documents a full-screen screenshot saved under the Pictures\Screenshots folder. | High, documentation only |

## State and boundary observations

Confirmed from the cited documentation:

- Static image modes include Rectangle, Window, Full screen and Freeform.
- Rectangle and Freeform use click-and-drag selection.
- The screenshot is automatically copied to the clipboard after capture.
- A notification can open the result in the Snipping Tool editor.
- The result can be saved, copied, shared or annotated in documented flows.

Not confirmed in this research:

- Default mode and whether the last mode persists across sessions.
- Exact `Esc` cancellation behavior inside the current Snipping Tool overlay.
- Selection minimum size, resize behavior, pointer appearance and keyboard focus.
- Multi-monitor, DPI scaling, HDR and mixed-scale behavior.
- Exact notification timing, lifetime and activation behavior.
- Failure behavior when clipboard, save, notification or editor activation fails.
- Exact close behavior and whether closing changes clipboard or saved data.

## Review boundary

This is a research record, not a product decision. Do not copy its observations into SnipPlus PRD or Specs without a separate product review and explicit source link.
