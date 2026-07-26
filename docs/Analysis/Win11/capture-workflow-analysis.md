# Win11 Snipping Tool — Capture Workflow Analysis

狀態：`Draft`

這份文件只分析既有 Win11 Research 的流程結構，不代表 SnipPlus 的功能、UI、產品方向或技術實作。

## Analysis metadata

| Field | Value |
| --- | --- |
| Subject | Windows 11 Snipping Tool static image capture workflow |
| Analysis date | 2026-07-26 |
| Research source | [Capture workflow](../../Research/Win11/01-capture-workflow.md)；[Workflow state machine](../../Research/Win11/02-workflow-state-machine.md) |
| Research verification | Official Microsoft Support documentation review |
| Runtime verification | Not performed |
| Product decision | Not made |

## Research to Analysis boundary

Research 已確認的內容在本文件中被整理成流程與狀態；本文件不添加新的平台事實，也不把觀察結果轉成 SnipPlus 需求。

## Workflow

| Step | State | Entry | User action | System response | Exit |
| --- | --- | --- | --- | --- | --- |
| 1 | `Ready` | 從 Start、`Windows logo key + Shift + S` 或文件所列快捷鍵進入 | 啟動截圖流程 | 提供截圖流程；確切轉換時間為 `UNKNOWN` | 進入模式選擇，或走直接 `PrtSc` 路徑 |
| 2 | `ModeSelection` | 截圖工具或 snipping overlay 可用 | 選擇 Rectangle、Window、Full screen 或 Freeform | 依模式進入選取，或產生截圖 | 進入 `AreaSelection` 或 `Captured` |
| 3 | `AreaSelection` | 選擇 Rectangle 或 Freeform | Click-and-drag 定義範圍 | 選取完成後產生 snip | 進入 `Captured`；取消行為為 `UNKNOWN` |
| 4 | `Captured` | Window、Full screen 的立即擷取，或範圍選取完成 | 通常不需要額外動作 | 自動複製到 clipboard，並顯示完成通知 | 進入 `ClipboardAvailable` 或 `NotificationAvailable` |
| 5 | `NotificationAvailable` | 完成通知出現 | 選取通知 | 開啟 Snipping Tool editor | 進入 `Editor` |
| 6 | `Editor` | 通知或工具流程開啟結果 | 可選擇 annotate、crop、Text actions、copy、save 或 share | 執行文件所列的編輯或交付動作；各動作的失敗反應為 `UNKNOWN` | 交付到 clipboard、saved、shared，或離開；close 行為為 `UNKNOWN` |

## State transition analysis

| State | Enter | Exit | Trigger | Dependency | Failure |
| --- | --- | --- | --- | --- | --- |
| `Ready` | 工具可由 Start 或入口快捷鍵啟動 | `ModeSelection` 或直接 clipboard 路徑 | 使用者啟動入口 | Start、keyboard entry、Snipping Tool availability | `UNKNOWN` |
| `ModeSelection` | 截圖流程已進入可選模式狀態 | `AreaSelection` 或 `Captured` | 選擇 capture mode | Mode selection interaction | `UNKNOWN` |
| `AreaSelection` | Rectangle 或 Freeform 被選取 | `Captured` 或未知取消路徑 | 使用者 click-and-drag 完成範圍 | Pointer/input selection | `UNKNOWN` |
| `Captured` | 立即擷取完成或範圍選取完成 | `ClipboardAvailable`、`NotificationAvailable` | Capture completion | Capture result、clipboard、notification | `UNKNOWN` |
| `ClipboardAvailable` | 系統完成自動 clipboard copy | 外部 paste/handoff 或 editor flow | Clipboard copy 完成 | Clipboard | `UNKNOWN` |
| `NotificationAvailable` | 完成通知出現 | `Editor` 或未知通知結束路徑 | 使用者選取通知 | Notification activation | `UNKNOWN` |
| `Editor` | 使用者開啟截圖結果 | `Saved`、`Shared`、`ClipboardAvailable` 或未知 close 路徑 | Editor handoff | Snipping Tool editor、copy/save/share actions | `UNKNOWN` |

## User intent

以下只把 Research 中已觀察到的使用者動作整理成意圖，不代表 SnipPlus 的目標使用者或產品價值：

- 以已知入口開始一個 static image capture flow。
- 指定要擷取的範圍或影像模式。
- 在結果產生後，將影像交付到 clipboard，或選擇繼續在 editor 中處理。
- 在 editor 中選擇文件列出的 annotate、copy、save 或 share 動作。

## System intent

以下只整理 Research 中已觀察到的系統反應：

- 將使用者選擇轉換成對應的 capture path。
- 在 capture 完成後提供 snip result。
- 自動將結果複製到 clipboard，並提供完成通知。
- 透過通知或工具流程把結果交給 editor。
- 對 editor 中的 copy、save、share 與 annotation actions 提供交付路徑；各 action 的詳細成功與失敗反應為 `UNKNOWN`。

## Dependencies observed in the workflow

這裡的 dependency 是研究流程中可觀察到的外部依賴，不是 SnipPlus implementation dependency：

- Keyboard 或 Start 入口。
- 使用者輸入與範圍選取。
- Capture mode selection。
- Clipboard。
- Completion notification。
- Snipping Tool editor 與其 documented actions。

## Workflow timeline

```text
User ↓ System ↓ Overlay ↓ Selection ↓ Toolbar ↓ Clipboard
```

| Step | User | System | Overlay | Selection | Toolbar | Clipboard |
| --- | --- | --- | --- | --- | --- | --- |
| 1. Entry | 啟動工具或快捷鍵 | 提供 capture flow | overlay 的確切外觀為 `UNKNOWN` | 尚未選取 | `UNKNOWN` | 尚未產生結果 |
| 2. Mode | 選擇 capture mode | 分流到對應模式 | overlay 可用；視覺細節為 `UNKNOWN` | Rectangle / Freeform 需要後續選取 | `UNKNOWN` | 尚未產生結果 |
| 3. Selection | Click-and-drag 定義範圍 | 完成選取後產生 snip | 變灰等描述已由 Research 記錄；其他細節為 `UNKNOWN` | Rectangle / Freeform | `UNKNOWN` | 尚未完成 copy |
| 4. Completion | 不需額外動作即可取得結果 | 產生結果並顯示 notification | 通知存在；時序與外觀為 `UNKNOWN` | 已完成或不適用 | `UNKNOWN` | 自動 copy 已由 Research 確認 |
| 5. Handoff | 選取通知或在工具中繼續 | 開啟 editor | editor 的視覺外觀為 `UNKNOWN` | 不適用 | 詳細 toolbar 行為為 `UNKNOWN` | 可 copy 或交付到外部 |

## Known unknowns

- `UNKNOWN`：取消、close、failure 與 recovery 的完整狀態轉換。
- `UNKNOWN`：PrtSc 行為衝突在目前 Windows 環境的實際結果。
- `UNKNOWN`：多螢幕、DPI scaling、HDR、focus 與 selection 邊界行為。
- `UNKNOWN`：notification 的確切 timing、lifetime 與 activation failure。
- `UNKNOWN`：clipboard、save、share 或 editor activation 失敗時的使用者可見反應。

## Prohibited conclusions

- 不從本文件新增 SnipPlus capture、editor、OCR、share 或 clipboard 功能。
- 不從本文件決定 SnipPlus UI、技術架構、產品範圍或優先順序。
- 不比較 LINE、ShareX、Snagit 或其他產品。
- 不把本文件直接當成 PRD、Spec、UX wireframe 或 coding task。
