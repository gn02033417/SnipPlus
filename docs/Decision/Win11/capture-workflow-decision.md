# Win11 Snipping Tool — Capture Workflow Decision

狀態：`Draft`

這份文件只記錄 SnipPlus 是否採用既有 Win11 capture workflow 的行為層級概念。它不設計 SnipPlus UI、不決定 toolbar 內容、不建立 PRD 或 Specs，也不代表已有程式碼。

## Decision metadata

| Field | Value |
| --- | --- |
| Subject | Windows 11 static image capture workflow |
| Decision date | 2026-07-26 |
| Analysis source | [Win11 capture workflow analysis](../../Analysis/Win11/capture-workflow-analysis.md) |
| Research source | [Win11 capture workflow Research](../../Research/Win11/01-capture-workflow.md)；[Workflow state machine](../../Research/Win11/02-workflow-state-machine.md) |
| Runtime verification | `UNKNOWN` — inherited from Research and Analysis |
| Decision review | Pending human review |

## Decision summary

| Workflow element | Decision | Short reason |
| --- | --- | --- |
| Capture entry workflow | `YES` | 保留一個由使用者入口啟動 static image capture 的基本流程概念。 |
| Rectangle / Freeform selection | `YES` | 保留明確範圍選取的 workflow 概念；不決定選取 UI。 |
| Window / Full screen capture | `YES` | 保留不同 capture scope 的 workflow 概念；不決定介面呈現。 |
| Automatic clipboard handoff | `YES` | 保留 capture 完成後可交付到 clipboard 的行為概念。 |
| Completion notification to editor | `PARTIAL` | 保留 capture result 可進入後續處理的 handoff 概念；通知與 editor 細節未決。 |
| Post-capture toolbar / annotation stage | `PARTIAL` | 保留 capture 後可繼續處理的流程位置；toolbar 與 annotation 內容未決。 |
| Video capture path | `NO` | 本 Decision 只涵蓋 static image workflow；Win+Shift+R 在 Research 中已標為範圍外。 |

## Decision records

### Capture entry workflow

| Field | Value |
| --- | --- |
| Decision | `YES` |
| Reason | 需要保留一個由使用者入口開始 capture 的流程概念，作為後續產品討論的行為邊界。 |
| Evidence | [Analysis workflow step 1](../../Analysis/Win11/capture-workflow-analysis.md#workflow)；[Research source-backed workflow](../../Research/Win11/01-capture-workflow.md#1-enter-the-capture-flow) |
| Risk | 目前尚未決定 SnipPlus 支援哪些實際入口、快捷鍵或權限行為。 |
| Open Question | SnipPlus 的正式入口與支援平台是否在 PRD 中採用相同範圍？ |

### Rectangle / Freeform selection

| Field | Value |
| --- | --- |
| Decision | `YES` |
| Reason | 保留使用者定義影像範圍的 workflow 概念；這不是 selection UI 或 interaction Spec。 |
| Evidence | [Analysis state transition](../../Analysis/Win11/capture-workflow-analysis.md#state-transition-analysis)；[Research area selection](../../Research/Win11/01-capture-workflow.md#3-select-an-area-when-required) |
| Risk | Selection 的取消、最小尺寸、focus、DPI 與多螢幕行為仍為 `UNKNOWN`。 |
| Open Question | 正式 PRD 是否要同時採用 Rectangle 與 Freeform，或只採用其中一種？ |

### Window / Full screen capture

| Field | Value |
| --- | --- |
| Decision | `YES` |
| Reason | 保留不同 capture scope 的 workflow 概念，不等於決定 UI mode selector。 |
| Evidence | [Analysis workflow](../../Analysis/Win11/capture-workflow-analysis.md#workflow)；[Research capture modes](../../Research/Win11/01-capture-workflow.md#2-select-a-capture-mode) |
| Risk | Window selection 的邊界、焦點與多螢幕行為未經 runtime 驗證。 |
| Open Question | SnipPlus 的正式 scope 是否包含 Window 與 Full screen 兩種模式？ |

### Automatic clipboard handoff

| Field | Value |
| --- | --- |
| Decision | `YES` |
| Reason | 保留 capture result 可交付到 clipboard 的基本 workflow 概念。 |
| Evidence | [Analysis system intent](../../Analysis/Win11/capture-workflow-analysis.md#system-intent)；[Research capture completion](../../Research/Win11/01-capture-workflow.md#4-capture-completion) |
| Risk | Clipboard failure、格式、生命週期與 privacy policy 尚未決定。 |
| Open Question | PRD 是否採用 clipboard 作為必要交付，還是僅作為可選 handoff？ |

### Completion notification to editor

| Field | Value |
| --- | --- |
| Decision | `PARTIAL` |
| Reason | 保留 capture result 可進入後續 editor 的流程位置；不採用任何特定 notification 或 editor UI 設計。 |
| Evidence | [Analysis workflow timeline](../../Analysis/Win11/capture-workflow-analysis.md#workflow-timeline)；[Research optional editor handoff](../../Research/Win11/01-capture-workflow.md#5-optional-editor-handoff) |
| Risk | Notification timing、lifetime、activation failure 與 close behavior 都是 `UNKNOWN`。 |
| Open Question | 是否需要 notification 作為必要入口，或允許其他 editor handoff？ |

### Post-capture toolbar / annotation stage

| Field | Value |
| --- | --- |
| Decision | `PARTIAL` |
| Reason | 只保留 capture 後存在後續處理階段的 workflow 位置；toolbar、annotation、OCR 與 action set 不在本文件決定。 |
| Evidence | [Analysis workflow timeline](../../Analysis/Win11/capture-workflow-analysis.md#workflow-timeline)；[Research optional editor handoff](../../Research/Win11/01-capture-workflow.md#5-optional-editor-handoff) |
| Risk | 若將流程位置誤讀為功能承諾，會在 PRD 前提前擴張 scope。 |
| Open Question | 後續 PRD 是否採用 editor，以及 editor 的正式範圍為何？ |

### Video capture path

| Field | Value |
| --- | --- |
| Decision | `NO` |
| Reason | 本文件範圍是 static image capture；Research 已將 `Windows logo key + Shift + R` 的 video path 標為範圍外。 |
| Evidence | [Research direct keyboard paths](../../Research/Win11/01-capture-workflow.md#direct-keyboard-paths) |
| Risk | 這不是永久否決所有未來 video 需求，只是本次 static image decision 的 scope boundary。 |
| Open Question | 未來是否需要另開 Research、Analysis、Decision 與 PRD 流程處理 video capture？ |

## UNKNOWN retained

- Runtime verification：`UNKNOWN`。
- PrtSc 在目前 Windows 環境的實際行為：`UNKNOWN`。
- Selection cancellation、close、failure、recovery、DPI、多螢幕與 HDR：`UNKNOWN`。
- Clipboard、notification、save、share 與 editor activation failure 的使用者可見反應：`UNKNOWN`。

## Decision boundary

以上 `YES`、`NO` 與 `PARTIAL` 只代表行為層級的暫存採用判斷，不能直接當成 SnipPlus PRD、Specs、UI design 或 implementation authorization。正式產品範圍仍須在 PRD 經 review 後確認。
