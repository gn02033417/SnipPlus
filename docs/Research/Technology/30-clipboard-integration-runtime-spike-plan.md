# Clipboard Integration Runtime Spike Plan

| Field | Value |
|---|---|
| Document ID | RESEARCH-TECH-CLIPBOARD-002 |
| Title | Clipboard Integration Runtime Spike Plan |
| Status | Draft |
| Research Type | Runtime Spike Planning / Evidence Collection Plan |
| Upstream feasibility baseline | `RESEARCH-TECH-CLIPBOARD-001` |
| Technology Decision | `TD-004` — Clipboard Integration；依 `Architecture/TECHNOLOGY-DECISION-ROADMAP.md` 原樣引用，目前為 `Candidate` |
| UI Framework Decision | Unresolved — `ADR-0002` remains Draft |
| Rendering Decision | Not made |
| Capture Decision | Not made |
| Clipboard Decision | Not made |
| Execution Status | Not started |
| Build Verification | Not performed |
| Runtime Verification | Not performed |
| Clipboard Runtime Spike Authorized | No |
| Clipboard Read Authorized | No |
| Clipboard Write Authorized | No |
| Clipboard Clear Authorized | No |
| Evidence Write Authorized | No |
| Owner | TBD |
| Last reviewed | Not reviewed |
| Planning date | 2026-07-26 |

本文件只定義未來 Clipboard runtime spike 的隔離條件、比較方法、觀察欄位、停止規則與證據路徑。本文件不授權任何 Clipboard 操作，不建立 Project、Prototype、payload、Result 或 Evidence Artifact，也不把任何 Spike 的結果預填為產品決策。

## 1. Purpose

本文件回答：

> 在不污染使用者 Clipboard、不混淆 Capture／Rendering／File Output 責任，且不把單一 basic publication 結果誤當成完整技術決策的前提下，未來應如何規劃並隔離驗證五個 Clipboard Candidate、WPF／WinUI 3 Host、影像格式、Alpha／pixel fidelity、STA／COM／Dispatcher、Contention、Retry、Ownership、Process lifetime、History／Cloud 與 Clipboard／File Output 獨立性？

本文件的所有 Spike 都是 Planned。它們的 execution outline 只描述未來操作；本輪不執行任何一項。

## 2. Scope

本計畫涵蓋：

- `CLIP-SPIKE-001` 至 `CLIP-SPIKE-012` 的固定 binding。
- `CLIP-OPT-001` 至 `CLIP-OPT-005` 的候選資格與 Host 分離。
- Windows 11 desktop context 下的 WPF、WinUI 3、Win32／OLE 與 framework-neutral adapter 邊界。
- Framework bitmap object、`CF_BITMAP`、`CF_DIB`、`CF_DIBV5`、PNG registered format、WinRT bitmap representation、OLE `IDataObject`、WinRT `DataPackage` 與 multi-format publication 的觀察規劃。
- Synthetic image contract、Clipboard isolation environment、consumer interoperability、pixel／alpha／color metadata、threading／COM、contention／retry、ownership／lifetime、memory、History／Cloud 與 privacy controls。
- `CLIP-001..022`、`CLIP-GATE-001..010` 及 `CLIP-GAP-001..018` 的 coverage path。
- Future result artifact 的命名與位置規劃，但不建立目錄或檔案。

Clipboard publication 與 File Output 保持平行：

- Clipboard publication 成功不得推導 File Output 成功。
- File Output 成功不得推導 Clipboard publication 成功。
- Clipboard 失敗不得重新執行 Capture 或 Rendering。
- File Output 失敗不得清除或改寫已完成的 Clipboard 結果。
- 任一輸出失敗不得修改已完成的 Annotation／Selection state。

## 3. Non-goals

本輪不得：

- 讀取目前使用者 Clipboard。
- 寫入目前使用者 Clipboard。
- 清除、備份、還原或比較目前使用者 Clipboard。
- 執行任何 Runtime Spike。
- 建立 Project、Solution、Prototype、Source Code 或正式產品 Architecture。
- 建立 Bitmap、DIB、DIBV5、PNG 或 Clipboard payload。
- 建立 Result directory 或任何 Evidence Artifact。
- 執行 Restore、Build、Run、Publish、Test 或 Runtime verification。
- 修改 Clipboard History 或 Cloud Clipboard 設定。
- 修改 `RESEARCH-TECH-CLIPBOARD-001`、UI／Capture／Rendering Research Line 或 `ADR-0002`。
- 選擇 Clipboard Technology、排名 Candidate 或建立 Clipboard ADR。
- 建立 Save PNG、檔案命名、檔案儲存或 File Output implementation。
- 啟動任何應用程式、consumer、Browser、image editor 或 Office-style consumer。
- 開始正式 Clipboard 或截圖功能。

## 4. Controlled Vocabulary

### 4.1 Spike Status

只使用：

- `Planned`
- `Ready`
- `Blocked`
- `Deferred`
- `Running`
- `Completed`
- `Invalidated`

本文件的十二個 Spike 全部固定為 `Planned`。本文件不得使用 `Running` 或 `Completed` 描述本輪狀態。

### 4.2 Candidate Eligibility

只使用：

- `Eligible`
- `Conditionally eligible`
- `Not aligned`
- `Unknown`

API 存在不得單獨使 Candidate 成為 `Eligible`。`Unknown` 不得直接轉為 `Not aligned`。

### 4.3 Runtime Inclusion

只使用：

- `Planned`
- `Blocked`
- `Excluded with evidence`
- `Not evaluated`

本文件的 Runtime inclusion 只表示未來規劃狀態，不表示實際執行結果。

### 4.4 Functional Result

只使用：

- `Pass`
- `Fail`
- `Blocked`
- `Not executed`

本文件不預填任何 Spike 的 Functional Result。

### 4.5 Readiness to Execute

只使用：

- `Ready for clipboard runtime spike execution`
- `Conditionally ready`
- `Not ready`

依本文件末尾的機械式推導，目前狀態為 `Not ready`。

## 5. Spike Binding Policy

下列十二個 ID、名稱與目的必須完整保留，不得重新編號、刪除、合併或改變原始目的：

| Spike ID | Title | Status | Runtime inclusion |
|---|---|---|---|
| CLIP-SPIKE-001 | WPF basic bitmap publication | Planned | Planned |
| CLIP-SPIKE-002 | WinUI 3 basic bitmap publication | Planned | Planned |
| CLIP-SPIKE-003 | DIB／DIBV5 Alpha fidelity | Planned | Planned |
| CLIP-SPIKE-004 | PNG registered-format interoperability | Planned | Planned |
| CLIP-SPIKE-005 | Multi-format publication | Planned | Planned |
| CLIP-SPIKE-006 | STA／background-thread behavior | Planned | Planned |
| CLIP-SPIKE-007 | Clipboard contention and bounded retry observation | Planned | Planned |
| CLIP-SPIKE-008 | Ownership／process-lifetime behavior | Planned | Planned |
| CLIP-SPIKE-009 | Large synthetic image memory observation | Planned | Planned |
| CLIP-SPIKE-010 | Clipboard History／Cloud behavior boundary | Planned | Planned |
| CLIP-SPIKE-011 | Packaged／unpackaged comparison | Planned | Planned |
| CLIP-SPIKE-012 | Clipboard failure independent from File Output | Planned | Planned |

規則：

- 未來上游規格不足時，先建立 `CLIP-PLAN-GAP-xxx`，不得默默擴張 Spike 的範圍。
- 每個 Spike 維持獨立的 pass／fail 語意。
- Clipboard publication 成功不得推導 File Output 成功；File Output 成功不得推導 Clipboard publication 成功。
- Clipboard failure 不得重新執行 Capture 或 Rendering。
- 不得修改 `RESEARCH-TECH-CLIPBOARD-001` 以配合本計畫。

## 6. Candidate Eligibility Matrix

| Candidate | WPF eligibility | WinUI 3 eligibility | Required API／interop | COM／STA dependency | Packaging implication | Runtime inclusion | Evidence basis |
|---|---|---|---|---|---|---|---|
| CLIP-OPT-001 WPF `System.Windows.Clipboard`／`IDataObject` | Conditionally eligible | Unknown | WPF managed Clipboard／`IDataObject` route；WinUI 3 interoperability requires separate observation | To be verified from host and API path | Packaged／unpackaged behavior must be compared | Planned | `RESEARCH-TECH-CLIPBOARD-001` official baseline; runtime interoperability not established |
| CLIP-OPT-002 WinRT `Windows.ApplicationModel.DataTransfer.Clipboard`／`DataPackage` | Unknown | Conditionally eligible | WinRT Clipboard／`DataPackage` route；WPF interoperability requires separate consumer observation | To be verified from host and projection path | Packaged／unpackaged behavior must be compared | Planned | `RESEARCH-TECH-CLIPBOARD-001` official baseline; product suitability unverified |
| CLIP-OPT-003 Win32 OLE Clipboard／COM `IDataObject` | Conditionally eligible | Conditionally eligible | Native OLE Clipboard／COM `IDataObject` route with host adapter | COM／STA behavior must be observed per host | Packaging and process lifetime must be observed | Planned | `RESEARCH-TECH-CLIPBOARD-001` official baseline; host route not selected |
| CLIP-OPT-004 Win32 Raw Clipboard APIs | Conditionally eligible | Conditionally eligible | Raw Win32 Clipboard API／format path with host adapter | COM／thread conditions must be verified for the selected operations | Packaging and native interop boundary must be observed | Planned | `RESEARCH-TECH-CLIPBOARD-001` official baseline; format responsibility unverified |
| CLIP-OPT-005 Host-neutral Clipboard abstraction with framework-specific adapters | Conditionally eligible | Conditionally eligible | WPF adapter, WinUI 3 adapter and any required native adapter remain separate | Adapter-specific threading contract required | Each adapter must be compared in both packaging modes | Planned | Architecture boundary plus `RESEARCH-TECH-CLIPBOARD-001`; abstraction is not an API |

本表不是 Candidate ranking。Eligibility 只表示是否值得規劃驗證，不表示產品採用或可投入 production。

## 7. Controlled Comparison Rules

未來若得到 runtime authority，所有可比較的 Spike 必須遵守同一組條件；若條件不同，結果不得放在同一結論中：

| Dimension | Required rule |
|---|---|
| OS | 使用相同 Windows edition／build；實際值必須記錄 |
| CPU | 使用相同 CPU architecture；實際值必須記錄 |
| Synthetic source | 使用相同 synthetic image source identity |
| Dimensions | 使用相同 physical pixel dimensions；不同尺寸另列 |
| Pixel format | 使用相同 pixel format；不同格式另列 |
| Alpha pattern | 使用相同 alpha pattern；premultiplied／straight 分開 |
| Color metadata | 使用相同 color metadata；未指定時記錄為 Unknown |
| Host | WPF 與 WinUI 3 分開；不得混合 host 結果 |
| Build configuration | Debug 與 Release 分開 |
| Packaging | Packaged 與 unpackaged 分開 |
| Thread | UI thread、background STA、background MTA 分開 |
| Rendering | Immediate 與 delayed rendering 分開 |
| Publication | 單格式與多格式 publication 分開 |
| Process | Cold process 與 warm process 分開 |
| Clipboard History | Enabled／disabled／not inspected 分開 |
| Cloud Clipboard | Enabled／disabled／not inspected 分開 |
| Conversion | Candidate-specific conversion 必須明確列出 |
| Consumer | Producer publication success 與 consumer interoperability 分開 |
| Retry | Initial contention failure 與 retry outcome 分開 |
| Workflow | Clipboard failure 不得重新執行 Capture 或 Rendering |

不得作以下推論：

- Consumer 可貼上不代表 Alpha fidelity 通過。
- PNG stream 可被讀取不代表 decoded bitmap 的 pixel fidelity 通過。
- Process 結束後仍可貼上不代表 delayed rendering 安全完成。
- Retry 成功不得掩蓋初始 contention failure。
- 一個 Host 的結果不得直接宣告另一個 Host 的結果。

## 8. Synthetic Image Contract

只規劃，不建立任何 image。未來使用的 synthetic image 至少要有：

- 固定 physical pixel dimensions，並分為小型、一般及大型尺寸類別。
- 完全不透明區域、完全透明區域與半透明 gradient。
- Premultiplied Alpha 測試區與 straight Alpha 參考區。
- 1-pixel border、四角識別標記、中心標記與 known pixel coordinates。
- RGB primary 色塊與 grayscale gradient。
- 中英文文字、fine-line pattern 與可辨識的邊界。
- SDR color block 與 wide-color substitute metadata；不可將 substitute 當成 HDR 結論。
- Unique synthetic run identifier；不得包含使用者資料。

每次未來執行必須記錄：

| Field | Requirement |
|---|---|
| Synthetic image identity | 實際 identity，不使用私人檔案名稱 |
| Dimensions | Width／height，physical pixels |
| Pixel format | 實際 producer format |
| Alpha mode | Premultiplied／straight／opaque／Unknown |
| Color metadata | 實際 metadata 或 Unknown |
| Expected markers | Known pixel coordinates 與預期值 |
| Image size class | Small／Normal／Large |
| Data origin | Synthetic only |

禁止使用：

- 真實 Screenshot、使用者桌面或私人圖片。
- Window title、Email、Browser、Chat、Credential 或個人資料。
- 正式 Capture output 或正式 Annotation output。

## 9. Clipboard Isolation Environment

未來 Runtime Spike 必須在不含私人 Clipboard 資料的隔離環境執行，至少規劃一種：

- Dedicated test user account。
- Disposable VM。
- 明確同意的 isolated desktop session。
- 其他可證明不含私人 Clipboard 資料的隔離 session。

每次執行前必須明確記錄：

| Boundary | Required planning decision |
|---|---|
| Existing Clipboard | 是否可能含私人內容；若可能，停止 |
| Read authority | 是否授權讀取；本文件固定為 No |
| Write authority | 是否授權寫入；本文件固定為 No |
| Clear authority | 是否授權清除；本文件固定為 No |
| User data protection | 如何避免覆蓋需要保存的資料 |
| Residual payload | 測試後如何避免殘留；未來才可依隔離環境處理 |
| Process cleanup | 正常與異常結束的清理方式 |
| History／Cloud | 是否隔離；不得修改使用者設定 |
| Consumer scope | 允許觀察的 test consumer |
| Failure stop | 任一 privacy／cleanup 不確定即停止 |

保存並還原使用者 Clipboard 不得被視為零風險操作。若無法證明隔離，Readiness 必須維持 `Not ready`。

## 10. Clipboard Format Verification Contract

未來只規劃觀察，不建立任何 payload：

| Format | Producer representation | Expected consumer representation | Alpha check | Color check | Lifetime check | Required evidence |
|---|---|---|---|---|---|---|
| Framework bitmap object | Candidate／Host specific object | Host-specific image consumer | Decode and compare known alpha markers | Record metadata and pixel result | Object and process lifetime | Producer identity, consumer observation, comparison |
| `CF_BITMAP` | Native bitmap representation where applicable | Win32／framework bitmap consumer | Do not assume Alpha preservation | Record conversion responsibility | Native handle ownership | Format enumeration and lifetime observation |
| `CF_DIB` | DIB representation | DIB-capable consumer | Verify Alpha treatment separately | Record header, stride and channel interpretation | Buffer ownership | Header／stride／consumer evidence |
| `CF_DIBV5` | DIBV5 representation | DIBV5-capable consumer | Verify masks and Alpha behavior | Record color metadata responsibility | Buffer ownership | Header／mask／pixel evidence |
| PNG registered format | PNG byte stream | PNG-capable consumer | Compare decoded pixels and alpha | Record color metadata and decoder behavior | Stream lifetime | Byte-stream metadata and decoded comparison |
| WinRT bitmap representation | WinRT `DataPackage` bitmap representation | WinRT／framework consumer | Consumer interpretation separately | Record platform conversion | Reference and process lifetime | DataPackage format and consumer observation |
| OLE `IDataObject` | OLE data object with one or more formats | OLE consumer | Per-format observation | Per-format observation | OLE ownership and lifetime | Format enumeration and owner observation |
| WinRT `DataPackage` | DataPackage with one or more data representations | WinRT consumer | Per-format observation | Per-format observation | DataPackage lifetime | Package format and consumer observation |
| Multi-format publication | Multiple independent representations | Consumer-selected representation | Compare each selected representation | Record conversion and selection | All representations and owner lifetime | Atomic publication observation |

規則：

- PNG stream 與 decoded bitmap 必須分開記錄。
- Alpha premultiplication、row stride、DIB header、channel mask 與 color metadata 責任必須實際記錄。
- Consumer interpretation 與 producer payload representation 必須分開。
- 不得自行宣告正式產品格式。
- 不得假設 `CF_BITMAP` 保留 Alpha。
- Format interoperability 未被證明時，對應 Gap 維持 open。

## 11. Consumer Interoperability Matrix

只規劃未來 consumer 類別，不啟動或操作任何 consumer：

| Consumer class | Required formats | Paste method | Verification method | Privacy boundary |
|---|---|---|---|---|
| WPF test consumer | Framework bitmap, DIB／DIBV5, PNG as applicable | Isolated test action | Format enumeration plus pixel／alpha comparison | Synthetic data only |
| WinUI 3 test consumer | WinRT bitmap, PNG, multiple formats as applicable | Isolated test action | Format enumeration plus pixel／alpha comparison | Synthetic data only |
| Win32／OLE test consumer | OLE `IDataObject`, `CF_BITMAP`, DIB／DIBV5 | Isolated test action | Native format and ownership observation | Synthetic data only |
| Basic image editor class | Documented supported image formats | Isolated paste observation | Interoperability observation only | No real image or private content |
| Office-style document consumer class | Documented supported image formats | Isolated paste observation | Interoperability observation only | No account or document data |
| Browser／web-content consumer class | Only if separately authorized | Isolated paste observation | Interoperability observation only | No network upload or private page |
| Clipboard History surface | Platform-observable formats | Isolated observation only | History materialization and format observation | No user settings modification |
| Cross-device Cloud Clipboard surface | Only if isolated conditions permit | Isolated observation only | Cloud boundary observation | No extra account, device or network authorization |

Third-party consumer 行為只能作 interoperability observation，不得寫成 Windows platform guarantee。

## 12. Threading／COM Verification Contract

未來需在隔離環境中分別規劃下列 scenario：

| Scenario | Apartment state | Thread | Dispatcher | Expected platform condition | Required observation |
|---|---|---|---|---|---|
| WPF UI STA | STA | UI thread | Active WPF Dispatcher | Host UI condition must be satisfied | Publication result, dispatcher response, exception or HRESULT |
| WPF background STA | STA | Background | Explicit dispatcher boundary | Background STA route must be separately observed | Publication result and marshal behavior |
| WPF background MTA | MTA | Background | Explicit dispatcher boundary | Do not assume direct Clipboard write is valid | Failure classification and recovery path |
| WinUI 3 UI thread | Host-defined | UI thread | Active WinUI dispatcher | Host route must be separately observed | Publication result and UI responsiveness |
| WinUI 3 background thread | Host-defined | Background | Explicit dispatcher boundary | Background route must be separately observed | Marshal, cancellation and failure |
| OLE Clipboard with COM initialized | STA／COM initialized | Explicit test thread | As applicable | OLE requirement must be recorded | COM condition, publication and ownership |
| OLE Clipboard without required COM initialization | Unknown／not initialized | Explicit test thread | As applicable | Only execute with separate authority | Failure classification; no product conclusion from absence alone |
| Dispatcher shutdown | Host-specific | UI or background | Shutting down | Must stop without re-running Capture／Rendering | Cancellation, cleanup and failure classification |
| Application shutdown during publication | Host-specific | UI or background | Shutting down | Process lifetime must be observed | Ownership, cleanup and post-shutdown consumer behavior |
| Cancellation during retry | Host-specific | Retry thread | Active or shutting down | Retry cancellation must be bounded | Attempt count, elapsed time and final state |

不得假設 background thread 可以直接寫入 Clipboard。Threading 結果必須與 Candidate、Host、packaging、format 與 process lifetime 一起記錄。

## 13. Contention and Retry Observation Contract

本計畫只定義量測欄位，不制定正式 product policy：

| Measurement field | Required observation |
|---|---|
| Initial publication result | 成功、失敗或未執行；不得覆蓋 initial failure |
| Failure／exception／HRESULT | 原始分類可安全記錄時才記錄；不得含私人內容 |
| Clipboard owner identity | 只能以安全匿名形式記錄；不保存私人識別資料 |
| Attempt count | Initial attempt 與 retry 分開 |
| Attempt timestamps | 使用可比較的時間欄位 |
| Retry interval | 實際 observed interval；正式值 TBD |
| Total elapsed time | 從 initial attempt 到 final observation |
| Dispatcher blocked duration | 若可安全量測則記錄 |
| UI responsiveness | 只作 observation，不建立 UI policy |
| Payload rebuild count | 記錄是否重新建立 representation |
| Clipboard owner changes | 只記錄匿名化狀態 |
| Cancellation result | 是否在 retry／timeout 前停止 |
| Shutdown result | 是否因 dispatcher／process shutdown 停止 |
| Final publication result | 與 initial result 分開 |
| File Output parallel result | 只觀察平行流程，不控制其結果 |

固定未決值：

- Retry threshold: `TBD`
- Retry interval: `TBD`
- Timeout: `TBD`
- Maximum image size: `TBD`
- Maximum memory budget: `TBD`
- Decision use: `Informative only`

不得在本文件制定正式 retry policy。任何 contention failure 必須允許 File Output 依既有架構獨立處理；不得把 workflow 返回 Editing，也不得重跑 Capture／Rendering。

## 14. Ownership and Lifetime Contract

未來研究必須分別觀察：

- Immediate data copy。
- OLE object ownership。
- Delayed rendering。
- Producer process 保持運行。
- Producer process 正常結束。
- Producer process 異常終止。
- Clipboard owner 變更。
- Clipboard flush 行為（僅在適用官方 API 時觀察）。
- Consumer 在 producer 結束前與結束後讀取。
- Multiple-format payload lifetime。
- Native handle cleanup。
- Stream lifetime。
- Managed object disposal。
- History／Cloud Clipboard materialization。

本文件不預先決定產品採用 immediate 或 delayed rendering。所有 ownership 結果都必須保留 Candidate、Host、format、thread 與 process context。

## 15. History／Cloud Clipboard Observation Boundary

只規劃下列隔離觀察情境：

| Scenario | Observation boundary |
|---|---|
| History disabled | 只記錄隔離環境的觀察條件 |
| History enabled, Cloud disabled | 觀察 image 是否 materialize 及格式呈現 |
| History and Cloud enabled | 只有在隔離條件與額外授權都滿足時規劃 |
| Format selection | 記錄 History 採用的 representation，如可安全觀察 |
| Multi-format publication | 記錄呈現方式，不推導 platform guarantee |
| Process end | 觀察 process 結束後是否仍可用；不預填結果 |
| Large image | 記錄 size、memory、materialization observation |
| Sensitive data | 只使用 synthetic image，禁止私人資料 |
| User settings unavailable | 以 Not inspected 記錄，不修改設定 |

不得：

- 修改目前使用者的 History 或 Cloud Clipboard 設定。
- 登入額外 Microsoft Account。
- 傳送測試資料至未授權裝置或服務。
- 將 Cloud sync 成功設為 Phase 1 必要條件。
- 保存帳號、裝置、Window title 或私人識別資訊。

## 16. Environment Record

未來每次 Spike 必填下列欄位；本文件不預填不存在的結果：

| Field | Requirement |
|---|---|
| Windows edition／build | 執行時記錄 |
| CPU architecture | 執行時記錄 |
| .NET version | 執行時記錄 |
| Host Framework／version | 執行時記錄 |
| Candidate API identity | 執行時記錄 |
| Apartment state | `STA`／`MTA`／`Unknown` |
| Dispatcher state | 執行時記錄 |
| Packaged state | `Packaged`／`Unpackaged` |
| Build configuration | `Debug`／`Release` |
| Process architecture | 實際值，例如 x64；不得猜測 |
| Clipboard History state | `Enabled`／`Disabled`／`Not inspected` |
| Cloud Clipboard state | `Enabled`／`Disabled`／`Not inspected` |
| Synthetic image identity | 執行時記錄 |
| Image dimensions | 執行時記錄 |
| Pixel format | 執行時記錄 |
| Alpha mode | 執行時記錄 |
| Test consumer | 執行時記錄 |
| Test timestamp | 執行時記錄 |
| Isolation environment | 執行時記錄 |
| User privilege | 執行時記錄 |

## 17. Evidence Types

未來可規劃下列 evidence types；本輪不建立任何 artifact：

- Environment record。
- Synthetic image specification。
- Producer payload metadata。
- Clipboard format enumeration。
- Consumer format observation。
- Pixel comparison。
- Alpha comparison。
- Color metadata observation。
- Thread／Apartment record。
- Dispatcher behavior。
- Contention failure record。
- Retry timing record。
- Ownership／lifetime record。
- Process termination observation。
- Memory observation。
- History／Cloud observation。
- Parallel File Output result。
- Privacy review。
- Cleanup confirmation。
- Diagnostic log。

Diagnostic log 不得包含 image bytes、Credential、私人路徑、Window title 或可識別使用者的 Clipboard owner information。

## 18. Per-Spike Execution Specifications

以下各節只定義未來執行的固定欄位。所有 `Status` 固定為 `Planned`；未填寫實際 Functional Result、測量值或 runtime conclusion。

### 18.1 CLIP-SPIKE-001 — WPF basic bitmap publication

| Field | Plan |
|---|---|
| Spike ID | CLIP-SPIKE-001 |
| Title | WPF basic bitmap publication |
| Status | Planned |
| Related criteria | CLIP-001, CLIP-002, CLIP-006, CLIP-009, CLIP-010, CLIP-011, CLIP-013, CLIP-017, CLIP-022 |
| Related gates | CLIP-GATE-001, CLIP-GATE-002, CLIP-GATE-003, CLIP-GATE-006, CLIP-GATE-010 |
| Related gaps | CLIP-GAP-001, CLIP-GAP-002, CLIP-GAP-003, CLIP-GAP-006, CLIP-GAP-010 |
| Candidate technologies | CLIP-OPT-001；必要時以 CLIP-OPT-005 的 WPF adapter 作邊界觀察 |
| Eligible Hosts | WPF |
| Purpose | 規劃 WPF basic bitmap publication 與基本 consumer interoperability 的最小觀察路徑 |
| Dependencies | Isolated Clipboard environment、synthetic image contract、WPF host authority、CLIP-OPT-001 official baseline |
| Preconditions | 不含私人 Clipboard 資料；synthetic image identity 已定義；Clipboard read／write／clear 與 runtime authority 另行核准 |
| Isolation environment | Dedicated test user、disposable VM 或明確同意的 isolated desktop session |
| Synthetic input | 小型 synthetic image，含 opaque、transparent、半透明 marker；不建立於本輪 |
| Clipboard formats | Framework bitmap object；其他格式不得在本 Spike 推論 |
| Producer configuration | WPF UI host、固定 pixel dimensions、固定 alpha pattern、固定 color metadata |
| Consumer configuration | WPF test consumer；只限隔離 synthetic data |
| Thread／Apartment configuration | WPF UI STA；dispatcher 狀態必須記錄 |
| Packaging configuration | 先明確記錄 packaged／unpackaged；不得混合結果 |
| Execution outline | 未來在隔離環境發布 synthetic bitmap，記錄 format、consumer、pixel／alpha、thread、ownership 與 cleanup observation |
| Required evidence | Environment record、format enumeration、consumer observation、pixel／alpha comparison、thread record、cleanup confirmation |
| Functional pass condition | Consumer 可取得預期 representation，且 known pixel／alpha markers 與 producer contract 的比較結果可重現；不預填結果 |
| Format interoperability verification | 記錄 producer representation 與 consumer interpretation；不得將貼上成功當作完整相容性 |
| Alpha／pixel verification | 比較 opaque、transparent、半透明與 known coordinates；premultiplication 另記錄 |
| Threading verification | 記錄 WPF UI STA、dispatcher 與 publication 時序 |
| Contention／retry observation | 本 Spike 只觀察初始 publication；若發生 contention，停止或依另行授權的 bounded retry Spike 處理 |
| Ownership／lifetime observation | 記錄 object、native handle、producer process 與 consumer 讀取時序 |
| Memory fields | Dimensions、pixel format、representation size、observed allocation；實際值執行時填寫 |
| History／Cloud boundary | 不修改設定；隔離條件不成立時不觀察 |
| Parallel File Output observation | 不控制、不推導 File Output 結果；若需同步觀察，使用獨立授權 |
| Privacy checks | Synthetic only；不讀取、保存或記錄現有 Clipboard |
| Failure condition | Host、thread、format、consumer、isolation 或 cleanup 條件不滿足 |
| Failure implication | 只標記此 WPF basic route 的 evidence gap，不做產品選擇結論 |
| Stop conditions | 需要讀寫既有 Clipboard、需要修改設定、出現私人資料、需要正式 workflow 或 source code |
| Cleanup | 依隔離環境核准的 cleanup procedure；本文件不執行 |
| Future result destination | `docs/Research/Technology/results/clipboard/CLIP-SPIKE-001-result.md`；只規劃 |
| Open questions | WPF bitmap representation 的跨 consumer Alpha／color fidelity、packaging 與 process lifetime |

### 18.2 CLIP-SPIKE-002 — WinUI 3 basic bitmap publication

| Field | Plan |
|---|---|
| Spike ID | CLIP-SPIKE-002 |
| Title | WinUI 3 basic bitmap publication |
| Status | Planned |
| Related criteria | CLIP-001, CLIP-003, CLIP-004, CLIP-006, CLIP-009, CLIP-010, CLIP-011, CLIP-013, CLIP-017, CLIP-022 |
| Related gates | CLIP-GATE-001, CLIP-GATE-002, CLIP-GATE-003, CLIP-GATE-006, CLIP-GATE-007, CLIP-GATE-010 |
| Related gaps | CLIP-GAP-001, CLIP-GAP-004, CLIP-GAP-005, CLIP-GAP-006, CLIP-GAP-011 |
| Candidate technologies | CLIP-OPT-002；必要時以 CLIP-OPT-005 的 WinUI 3 adapter 作邊界觀察 |
| Eligible Hosts | WinUI 3 |
| Purpose | 規劃 WinUI 3 basic bitmap publication 與基本 consumer interoperability 的最小觀察路徑 |
| Dependencies | Isolated Clipboard environment、synthetic image contract、WinUI 3 host authority、CLIP-OPT-002 official baseline |
| Preconditions | Host、Windows App SDK／WinRT identity、packaging state 與 runtime authority 已核准；本輪不執行 |
| Isolation environment | Dedicated test user、disposable VM 或明確同意的 isolated desktop session |
| Synthetic input | 與 WPF Spike 相同 identity、dimensions、alpha pattern、color metadata 的 synthetic image |
| Clipboard formats | WinRT bitmap representation；其他格式另由專項 Spike 規劃 |
| Producer configuration | WinUI 3 UI host、固定 image contract、固定 build configuration |
| Consumer configuration | WinUI 3 test consumer；不啟動正式 application |
| Thread／Apartment configuration | WinUI 3 UI thread；dispatcher／host thread state 必須記錄 |
| Packaging configuration | Packaged／unpackaged 分開；actual package context 必須記錄 |
| Execution outline | 未來於隔離環境發布 synthetic bitmap，觀察 WinUI 3 host、consumer、format、thread、lifetime 與 cleanup |
| Required evidence | Environment、API identity、format／consumer、pixel／alpha、dispatcher、packaging、cleanup evidence |
| Functional pass condition | Consumer 可取得預期 representation 且 known markers comparison 可重現；不預填結果 |
| Format interoperability verification | 分離 WinRT representation 與 consumer interpretation；不由 API existence 推導 product suitability |
| Alpha／pixel verification | 比較 known pixel、alpha mode、row／stride 或等價 metadata；不預填 fidelity |
| Threading verification | 記錄 WinUI 3 UI thread、dispatcher、marshal 與 cancellation boundary |
| Contention／retry observation | 只記錄初始 failure；bounded retry 由 CLIP-SPIKE-007 另行規劃 |
| Ownership／lifetime observation | 記錄 DataPackage／representation、producer process 與 consumer 讀取時序 |
| Memory fields | Dimensions、representation size、observed memory；實際值執行時填寫 |
| History／Cloud boundary | 不修改 History／Cloud；隔離條件不足即停止 |
| Parallel File Output observation | 與 File Output 保持獨立；不把任一結果作為另一結果的 pass condition |
| Privacy checks | Synthetic only；不讀取、保存或記錄現有 Clipboard |
| Failure condition | WinUI 3 host／package／dispatcher／format／consumer 或 cleanup 不可證明 |
| Failure implication | 只形成 WinUI 3 route evidence gap，不做 technology ranking |
| Stop conditions | 需要真實使用者資料、額外帳號／裝置、未授權網路或正式 source code |
| Cleanup | 依隔離環境核准的 cleanup procedure；本文件不執行 |
| Future result destination | `docs/Research/Technology/results/clipboard/CLIP-SPIKE-002-result.md`；只規劃 |
| Open questions | WinUI 3 packaged／unpackaged、WinRT bitmap consumer 與 WPF／Win32 interoperability |

### 18.3 CLIP-SPIKE-003 — DIB／DIBV5 Alpha fidelity

| Field | Plan |
|---|---|
| Spike ID | CLIP-SPIKE-003 |
| Title | DIB／DIBV5 Alpha fidelity |
| Status | Planned |
| Related criteria | CLIP-006, CLIP-007, CLIP-010, CLIP-011, CLIP-012, CLIP-017, CLIP-022 |
| Related gates | CLIP-GATE-001, CLIP-GATE-002, CLIP-GATE-005, CLIP-GATE-006, CLIP-GATE-010 |
| Related gaps | CLIP-GAP-006, CLIP-GAP-007, CLIP-GAP-008, CLIP-GAP-009, CLIP-GAP-012 |
| Candidate technologies | CLIP-OPT-003、CLIP-OPT-004；各 Host adapter 另列 |
| Eligible Hosts | WPF、WinUI 3、Win32／OLE test consumer |
| Purpose | 規劃比較 DIB 與 DIBV5 header、stride、channel mask、Alpha 與 color interpretation |
| Dependencies | Synthetic image contract、format verification contract、隔離 consumer、native format authority |
| Preconditions | 不建立 payload；未來須有 approved synthetic input 與 format observation authority |
| Isolation environment | Dedicated test user、disposable VM 或 isolated session |
| Synthetic input | 含透明、半透明、premultiplied／straight reference、primary color、grayscale、known coordinates 的 synthetic image |
| Clipboard formats | `CF_DIB`、`CF_DIBV5`；必要時與 framework bitmap representation 對照 |
| Producer configuration | 明確 header、stride、channel mask、alpha mode 與 color metadata |
| Consumer configuration | Win32／OLE、WPF、WinUI 3 test consumer；結果分 consumer 記錄 |
| Thread／Apartment configuration | Candidate-specific native／host thread；STA／MTA 與 dispatcher 分開 |
| Packaging configuration | Packaged／unpackaged 依 consumer route 分開記錄 |
| Execution outline | 未來發布 synthetic DIB／DIBV5，枚舉 representation，讓隔離 consumer 觀察並比較 pixels／alpha／metadata |
| Required evidence | Header／mask／stride record、format enumeration、consumer observation、pixel／alpha comparison、lifetime evidence |
| Functional pass condition | 每一 representation 的 consumer interpretation 與 known markers 可被分別記錄；不預填格式 pass |
| Format interoperability verification | `CF_DIB` 與 `CF_DIBV5` 分開；不得由互相可轉換推導等價互通 |
| Alpha／pixel verification | 比較 transparent、semi-transparent、premultiplication、channel mask 與 known coordinates |
| Threading verification | 記錄 native／host thread、COM／STA、dispatcher 與 publication 時序 |
| Contention／retry observation | 不在本 Spike 形成 retry policy；contention 交由 CLIP-SPIKE-007 |
| Ownership／lifetime observation | 記錄 native buffer、handle、data object、consumer timing 與 cleanup |
| Memory fields | Header size、stride、payload size、observed allocation；實際值執行時填寫 |
| History／Cloud boundary | 只作隔離規劃；不得修改或依賴 History／Cloud |
| Parallel File Output observation | 不與 File Output 綁定；另行觀察時不共享結論 |
| Privacy checks | Synthetic only；不使用真實 image bytes |
| Failure condition | Header／mask／stride／Alpha responsibility 無法確認或 consumer interpretation 不可重現 |
| Failure implication | 對應 format／fidelity gaps 保持 open；不得選定 DIB 或 DIBV5 |
| Stop conditions | 需要讀取既有 Clipboard、需要真實 image、無法 cleanup 或需修改正式 workflow |
| Cleanup | 依隔離環境核准 procedure；本文件不執行 |
| Future result destination | `docs/Research/Technology/results/clipboard/CLIP-SPIKE-003-result.md`；只規劃 |
| Open questions | Alpha preservation、color metadata responsibility、consumer conversion 與 lifetime |

### 18.4 CLIP-SPIKE-004 — PNG registered-format interoperability

| Field | Plan |
|---|---|
| Spike ID | CLIP-SPIKE-004 |
| Title | PNG registered-format interoperability |
| Status | Planned |
| Related criteria | CLIP-006, CLIP-008, CLIP-009, CLIP-010, CLIP-011, CLIP-012, CLIP-017, CLIP-022 |
| Related gates | CLIP-GATE-001, CLIP-GATE-002, CLIP-GATE-005, CLIP-GATE-006, CLIP-GATE-010 |
| Related gaps | CLIP-GAP-006, CLIP-GAP-008, CLIP-GAP-009, CLIP-GAP-013 |
| Candidate technologies | CLIP-OPT-003、CLIP-OPT-004、CLIP-OPT-005 adapter route |
| Eligible Hosts | WPF、WinUI 3、Win32／OLE test consumer |
| Purpose | 規劃 PNG byte stream registered format 與 decoded bitmap interoperability 的分離觀察 |
| Dependencies | Synthetic image contract、PNG format authority、isolated consumers、format comparison rules |
| Preconditions | 未來須明確 byte stream representation、decoder identity、color metadata 與 cleanup authority |
| Isolation environment | Disposable VM 或 isolated test user/session |
| Synthetic input | 含 Alpha、SDR color、grayscale、fine-line、known marker 的 synthetic image |
| Clipboard formats | PNG registered format；必要時與 DIB／framework bitmap 對照，但不合併結論 |
| Producer configuration | Stream identity、encoding metadata、alpha mode、color metadata、lifetime boundary |
| Consumer configuration | 至少一個 Win32／OLE、WPF 或 WinUI 3 consumer；第三方只作 observation |
| Thread／Apartment configuration | Producer host thread 與 decoder／consumer thread 分開記錄 |
| Packaging configuration | Packaged／unpackaged 分開；不推導跨模式結果 |
| Execution outline | 未來發布 synthetic PNG representation，記錄 byte stream metadata，再由隔離 consumer decode 並比較 pixels／alpha |
| Required evidence | Stream metadata、decoder／consumer identity、decoded pixel／alpha comparison、color record、lifetime evidence |
| Functional pass condition | PNG byte stream 與 decoded bitmap 的結果可分別重現；不預填 consumer pass |
| Format interoperability verification | 只判斷各 consumer 是否取得可比較 representation；不得將 byte acceptance 當作 fidelity |
| Alpha／pixel verification | 比較 decoded pixels、transparent／semi-transparent markers、row／stride、color metadata |
| Threading verification | 記錄 publication 與 consumer／decoder 的 thread boundary；不假設 UI dependency |
| Contention／retry observation | 只觀察 initial failure；retry 由 CLIP-SPIKE-007 規劃 |
| Ownership／lifetime observation | 記錄 stream、data object、process end 與 consumer timing |
| Memory fields | Stream size、decoded size、observed allocation、large-image fields separately |
| History／Cloud boundary | 不修改設定；只在隔離條件滿足後另行規劃 |
| Parallel File Output observation | 不把 PNG Clipboard stream 與 Save PNG output 視為同一責任 |
| Privacy checks | Synthetic stream only；不記錄 image bytes |
| Failure condition | Decoder、format identity、Alpha／color interpretation 或 stream lifetime 不可證明 |
| Failure implication | 只更新 PNG interoperability evidence，不建立正式 format decision |
| Stop conditions | 需要網路、私人資料、使用者設定修改、非隔離 consumer 或未授權 evidence write |
| Cleanup | 依隔離環境 procedure；本文件不執行 |
| Future result destination | `docs/Research/Technology/results/clipboard/CLIP-SPIKE-004-result.md`；只規劃 |
| Open questions | Registered format availability、consumer decoder behavior、Alpha／color profile responsibility |

### 18.5 CLIP-SPIKE-005 — Multi-format publication

| Field | Plan |
|---|---|
| Spike ID | CLIP-SPIKE-005 |
| Title | Multi-format publication |
| Status | Planned |
| Related criteria | CLIP-006, CLIP-007, CLIP-008, CLIP-009, CLIP-010, CLIP-011, CLIP-012, CLIP-017, CLIP-018 |
| Related gates | CLIP-GATE-001, CLIP-GATE-002, CLIP-GATE-005, CLIP-GATE-006, CLIP-GATE-009, CLIP-GATE-010 |
| Related gaps | CLIP-GAP-006, CLIP-GAP-007, CLIP-GAP-008, CLIP-GAP-009, CLIP-GAP-014 |
| Candidate technologies | CLIP-OPT-001、CLIP-OPT-002、CLIP-OPT-003、CLIP-OPT-004、CLIP-OPT-005；不得形成 ranking |
| Eligible Hosts | WPF、WinUI 3、Win32／OLE test consumer |
| Purpose | 規劃同一 publication 同時提供多個 representations，並觀察 consumer 選擇與 atomicity |
| Dependencies | Basic publication routes、format contract、consumer matrix、ownership contract |
| Preconditions | 每個 representation identity 已獨立定義；無私人 Clipboard 資料；未來需另外核准 multi-format write |
| Isolation environment | Disposable VM 或 dedicated isolated session |
| Synthetic input | 同一 synthetic image identity，固定 dimensions、pixel、alpha、color metadata |
| Clipboard formats | Framework bitmap、DIB／DIBV5、PNG registered format 或其他候選 representations；實際組合由核准 Spike 決定 |
| Producer configuration | 同一 publication attempt、各 representation 的 conversion responsibility、format metadata |
| Consumer configuration | WPF、WinUI 3、Win32／OLE 與其他隔離 consumer 分別觀察 |
| Thread／Apartment configuration | Host UI／background、STA／MTA、dispatcher 與 OLE route 分開 |
| Packaging configuration | Packaged／unpackaged 分開；不得以一種模式推導另一種 |
| Execution outline | 未來以同一 synthetic input 發布多格式，逐一觀察 enumeration、consumer selection、pixel／alpha、ownership 與 cleanup |
| Required evidence | Multi-format enumeration、atomic publication observation、consumer selection、per-format comparisons、lifetime evidence |
| Functional pass condition | 可證明 publication attempt 中各 representation 的邊界與 consumer selection；不預填 atomicity 結果 |
| Format interoperability verification | 每一 format separately verified；consumer fallback 不代表所有 formats 相容 |
| Alpha／pixel verification | Per-format comparison；不可用其中一格式結果覆蓋另一格式 |
| Threading verification | 記錄一次 publication 的 thread／dispatcher／COM boundary |
| Contention／retry observation | Initial publication failure 與 retry separately recorded；policy 未定義 |
| Ownership／lifetime observation | Multiple representation ownership、stream／handle lifetime、process end separately observed |
| Memory fields | Per-format size、aggregate size、observed allocation、large-image impact |
| History／Cloud boundary | 不修改 History／Cloud；format selection observation 需隔離授權 |
| Parallel File Output observation | Clipboard aggregate success 不代表 File Output；兩者需獨立 result |
| Privacy checks | Synthetic only；不得保存 Clipboard formats 的私人內容 |
| Failure condition | Partial publication、format mismatch、consumer selection 不可分類或 cleanup 不完整 |
| Failure implication | 只標記 multi-format atomicity／ownership gap；不形成正式 publication contract |
| Stop conditions | Partial publication 造成未可控殘留、需要清除使用者 Clipboard 或需改正式 workflow |
| Cleanup | 依隔離環境 procedure，必須確認所有 representations 的 cleanup |
| Future result destination | `docs/Research/Technology/results/clipboard/CLIP-SPIKE-005-result.md`；只規劃 |
| Open questions | Atomicity semantics、format priority／selection、History materialization、ownership after process exit |

### 18.6 CLIP-SPIKE-006 — STA／background-thread behavior

| Field | Plan |
|---|---|
| Spike ID | CLIP-SPIKE-006 |
| Title | STA／background-thread behavior |
| Status | Planned |
| Related criteria | CLIP-003, CLIP-004, CLIP-013, CLIP-014, CLIP-017, CLIP-018, CLIP-021 |
| Related gates | CLIP-GATE-003, CLIP-GATE-004, CLIP-GATE-005, CLIP-GATE-006, CLIP-GATE-010 |
| Related gaps | CLIP-GAP-003, CLIP-GAP-004, CLIP-GAP-005, CLIP-GAP-015 |
| Candidate technologies | CLIP-OPT-001、CLIP-OPT-002、CLIP-OPT-003、CLIP-OPT-004、CLIP-OPT-005 adapters |
| Eligible Hosts | WPF、WinUI 3、Win32／OLE test host |
| Purpose | 規劃 UI STA、background STA、background MTA、COM initialized、dispatcher shutdown 與 cancellation 的差異觀察 |
| Dependencies | Candidate identity matrix、Threading／COM contract、isolated environment、basic publication plan |
| Preconditions | 明確 thread／COM authority；不以 background thread 可寫入 Clipboard 作假設 |
| Isolation environment | Disposable VM 或 isolated desktop session |
| Synthetic input | 固定 synthetic image，所有 thread scenarios 使用相同 identity |
| Clipboard formats | 由各 candidate route 指定；每個 thread condition 分開記錄 |
| Producer configuration | 相同 host／format，僅改變 thread、apartment、dispatcher 或 shutdown condition |
| Consumer configuration | Minimal isolated test consumer；不啟動正式 app |
| Thread／Apartment configuration | WPF UI STA、WPF background STA、WPF background MTA、WinUI UI／background、OLE COM initialized／not initialized |
| Packaging configuration | 只在 thread contract 已可觀察後分別規劃 packaged／unpackaged |
| Execution outline | 未來以 controlled comparison 逐一執行 thread scenario，記錄 marshal、exception／HRESULT、dispatcher、cancellation、cleanup |
| Required evidence | Apartment／COM record、dispatcher behavior、publication result、failure classification、shutdown／cleanup evidence |
| Functional pass condition | 每個 scenario 的 platform condition、failure／success boundary 與 cleanup 可被重現；不預填 result |
| Format interoperability verification | 使用相同 representation 比較，不把 thread success 當作 format success |
| Alpha／pixel verification | 只確認 thread 變化未混入不同 image contract；完整 fidelity 由 format Spike 覆蓋 |
| Threading verification | 以每一 scenario 的 actual apartment、dispatcher、marshal、cancellation record 為必要證據 |
| Contention／retry observation | 只記錄 thread-related contention；bounded retry 由 CLIP-SPIKE-007 另行規劃 |
| Ownership／lifetime observation | 對照 UI／background、normal／abnormal shutdown 的 object lifetime |
| Memory fields | Thread-specific allocation、blocked duration、payload rebuild count；實際值執行時填寫 |
| History／Cloud boundary | 不涉及使用者 History／Cloud 設定；需要時另行隔離規劃 |
| Parallel File Output observation | 不與 Clipboard thread route 共用結論；File Output 不因 Clipboard thread failure 重跑 |
| Privacy checks | Synthetic only；不得為 thread setup 讀取或保存既有 Clipboard |
| Failure condition | COM／STA／dispatcher condition、shutdown 或 cancellation 邊界無法明確分類 |
| Failure implication | 保持 threading／COM gap open；不得宣告 universal thread contract |
| Stop conditions | 需要跨 process 私人資料、需要變更正式 dispatcher／workflow 或需要未授權 native operation |
| Cleanup | 各 thread scenario 需有正常／例外／取消 cleanup path；本文件不執行 |
| Future result destination | `docs/Research/Technology/results/clipboard/CLIP-SPIKE-006-result.md`；只規劃 |
| Open questions | Host-specific dispatcher requirement、COM initialization、shutdown timing、background marshal behavior |

### 18.7 CLIP-SPIKE-007 — Clipboard contention and bounded retry observation

| Field | Plan |
|---|---|
| Spike ID | CLIP-SPIKE-007 |
| Title | Clipboard contention and bounded retry observation |
| Status | Planned |
| Related criteria | CLIP-015, CLIP-016, CLIP-017, CLIP-021, CLIP-022 |
| Related gates | CLIP-GATE-004, CLIP-GATE-005, CLIP-GATE-009, CLIP-GATE-010 |
| Related gaps | CLIP-GAP-003, CLIP-GAP-015, CLIP-GAP-016 |
| Candidate technologies | Candidate-specific route selected only for approved observation; no ranking |
| Eligible Hosts | WPF、WinUI 3、Win32／OLE；Host separately recorded |
| Purpose | 規劃 Clipboard contention、initial failure、bounded retry observation、cancellation 與 timeout measurement |
| Dependencies | Threading contract、ownership contract、isolated contention source、retry authority |
| Preconditions | Retry threshold、interval、timeout、cleanup、privacy 與 synthetic data authority separately approved；目前均 TBD／No |
| Isolation environment | Disposable VM 或 dedicated isolated user/session；contention source 不含私人資料 |
| Synthetic input | Fixed synthetic image and fixed representation；不得重建 Capture／Rendering output |
| Clipboard formats | Candidate-specific format；每次 attempt 必須記錄 representation identity |
| Producer configuration | Initial attempt、bounded retry observer、cancellation／shutdown hooks；不建立正式 product policy |
| Consumer configuration | Isolated test consumer only；contention source 必須可安全控制 |
| Thread／Apartment configuration | Host／thread／dispatcher/COM conditions inherited from approved route |
| Packaging configuration | Packaged／unpackaged separate; do not mix failure rates |
| Execution outline | 未來建立隔離 contention condition，記錄 initial result、attempts、interval、elapsed time、owner changes、cancellation、final result |
| Required evidence | Failure／HRESULT classification、attempt timing、owner observation、retry cancellation、cleanup、parallel File Output observation |
| Functional pass condition | Initial contention 与 bounded observation 欄位可重現，且停止／取消不會重跑 Capture／Rendering；不預填 retry result |
| Format interoperability verification | Contentions use a fixed representation；format fidelity not inferred from retry success |
| Alpha／pixel verification | 只確認 retry 不改變 synthetic input identity；fidelity 由 format Spike 覆蓋 |
| Threading verification | 記錄 retry dispatcher、thread、COM／STA 與 shutdown condition |
| Contention／retry observation | Initial failure、attempt count、interval、timeout、cancellation、final result、owner change separately recorded |
| Ownership／lifetime observation | 觀察 retry 中 payload、data object、owner 與 process lifetime；不得預先選 immediate／delayed |
| Memory fields | Payload rebuild count、attempt allocation、peak observed memory、elapsed time |
| History／Cloud boundary | 不修改 History／Cloud；contention test 不需依賴 cloud sync |
| Parallel File Output observation | Clipboard failure 不得阻擋獨立 File Output；不得讓 File Output 反向觸發 retry |
| Privacy checks | Synthetic only；owner identity anonymized；不得記錄既有 Clipboard content |
| Failure condition | Contention source、timeout、cancellation、cleanup 或 owner observation 不安全／不可分類 |
| Failure implication | 只保留 retry／contention evidence gap；不制定正式 retry policy |
| Stop conditions | Retry 造成持續污染、需要清除使用者 Clipboard、需要未授權 cross-process control 或無法確認 cleanup |
| Cleanup | 解除隔離 contention source；確認 test payload 不殘留；不處理使用者 Clipboard |
| Future result destination | `docs/Research/Technology/results/clipboard/CLIP-SPIKE-007-result.md`；只規劃 |
| Open questions | Retry bound、timeout、owner observation、UI responsiveness、parallel output semantics |

### 18.8 CLIP-SPIKE-008 — Ownership／process-lifetime behavior

| Field | Plan |
|---|---|
| Spike ID | CLIP-SPIKE-008 |
| Title | Ownership／process-lifetime behavior |
| Status | Planned |
| Related criteria | CLIP-017, CLIP-018, CLIP-019, CLIP-021 |
| Related gates | CLIP-GATE-005, CLIP-GATE-008, CLIP-GATE-010 |
| Related gaps | CLIP-GAP-014, CLIP-GAP-017 |
| Candidate technologies | CLIP-OPT-001、CLIP-OPT-002、CLIP-OPT-003、CLIP-OPT-004、CLIP-OPT-005 adapters |
| Eligible Hosts | WPF、WinUI 3、Win32／OLE |
| Purpose | 規劃 immediate／delayed rendering、OLE ownership、native handle、stream、object disposal 與 producer process 結束的觀察 |
| Dependencies | Format contract、multi-format plan、threading contract、isolated consumers |
| Preconditions | 未來需有 process lifecycle authority；不得預先選 immediate 或 delayed rendering |
| Isolation environment | Disposable VM 或 isolated test user/session |
| Synthetic input | Fixed synthetic image，representation identity fixed per scenario |
| Clipboard formats | Framework bitmap、DIB／DIBV5、PNG stream、OLE／WinRT object as applicable |
| Producer configuration | Immediate and delayed scenarios separately planned；normal／abnormal process termination separately planned |
| Consumer configuration | Isolated test consumer before and after producer termination |
| Thread／Apartment configuration | Candidate／Host-specific thread and COM state recorded |
| Packaging configuration | Packaged／unpackaged separately; process lifetime recorded |
| Execution outline | 未來逐一觀察 producer running、normal exit、abnormal exit、owner change、consumer timing、cleanup |
| Required evidence | Ownership／lifetime timeline、format enumeration、consumer observation、process termination、native cleanup |
| Functional pass condition | Ownership、consumer timing、cleanup 與 process end 的邊界可被分別重現；不預填結果 |
| Format interoperability verification | Consumer access after process event is separate from format fidelity |
| Alpha／pixel verification | Use same synthetic identity; do not use post-exit access to infer pixel fidelity |
| Threading verification | Record thread／COM／dispatcher across publication and shutdown |
| Contention／retry observation | Retry not primary; any contention logged separately and not converted into policy |
| Ownership／lifetime observation | Immediate／delayed、handles、streams、managed object、OLE owner、normal／abnormal exit |
| Memory fields | Representation memory、native handle count where safely observable、stream lifetime |
| History／Cloud boundary | Materialization observation only if isolated and separately authorized |
| Parallel File Output observation | Process lifetime of Clipboard route must not own File Output route |
| Privacy checks | Synthetic only; no real Clipboard backup／restore |
| Failure condition | Ownership unclear, delayed data unavailable, cleanup unconfirmed or process event cannot be isolated |
| Failure implication | Keep ownership／lifetime gap open; do not declare immediate or delayed rendering policy |
| Stop conditions | Requires preservation of user Clipboard, unbounded residual payload, private data or formal application shutdown changes |
| Cleanup | Normal／abnormal process cleanup documented for future execution; no action now |
| Future result destination | `docs/Research/Technology/results/clipboard/CLIP-SPIKE-008-result.md`；只規劃 |
| Open questions | OLE ownership、flush applicability、delayed rendering、process exit、History materialization |

### 18.9 CLIP-SPIKE-009 — Large synthetic image memory observation

| Field | Plan |
|---|---|
| Spike ID | CLIP-SPIKE-009 |
| Title | Large synthetic image memory observation |
| Status | Planned |
| Related criteria | CLIP-010, CLIP-011, CLIP-012, CLIP-017, CLIP-020, CLIP-021, CLIP-022 |
| Related gates | CLIP-GATE-001, CLIP-GATE-002, CLIP-GATE-005, CLIP-GATE-008, CLIP-GATE-010 |
| Related gaps | CLIP-GAP-008, CLIP-GAP-009, CLIP-GAP-018 |
| Candidate technologies | All five candidates, one route at a time |
| Eligible Hosts | WPF、WinUI 3、Win32／OLE |
| Purpose | 規劃 small／normal／large synthetic image 的 representation size、allocation、memory pressure 與 cleanup observation |
| Dependencies | Synthetic image contract、format contract、memory measurement authority、isolated environment |
| Preconditions | Maximum image size／memory budget 固定為 TBD；不得用實際使用者 screenshot |
| Isolation environment | Disposable VM 或 isolated test user/session with known resource boundary |
| Synthetic input | Three size classes, fixed pixel／alpha／color patterns, no private data |
| Clipboard formats | Candidate-specific framework bitmap、DIB／DIBV5、PNG、multi-format as separately approved |
| Producer configuration | Same image contract across candidates; conversion and representation memory separately recorded |
| Consumer configuration | Minimal isolated consumer; large History／Cloud observation only under separate authorization |
| Thread／Apartment configuration | Same host/thread condition within each controlled comparison |
| Packaging configuration | Packaged／unpackaged separately; Debug／Release separately |
| Execution outline | 未來按 size class 發布 synthetic input，觀察 allocation、peak memory、format size、consumer access、cleanup |
| Required evidence | Environment、dimensions、representation size、memory observation、failure classification、cleanup |
| Functional pass condition | Memory fields and cleanup can be reproduced for each size／format／host condition；不預填 capacity result |
| Format interoperability verification | Memory observation does not imply consumer interoperability |
| Alpha／pixel verification | Same markers used to ensure size variation did not change input contract |
| Threading verification | Record UI／background、dispatcher、COM and blocked duration where applicable |
| Contention／retry observation | Record if memory pressure causes publication failure; retry policy remains out of scope |
| Ownership／lifetime observation | Track large stream／buffer／object lifetime and process cleanup |
| Memory fields | Dimensions、pixel format、source size、representation size、peak observed、cleanup result、failure boundary |
| History／Cloud boundary | Large image History／Cloud behavior only as separately authorized isolated observation |
| Parallel File Output observation | Memory pressure in Clipboard route must not cause File Output rerun or shared state mutation |
| Privacy checks | Synthetic only; no actual screenshot bytes |
| Failure condition | Resource boundary unknown, allocation cannot be safely measured, or cleanup is not verifiable |
| Failure implication | Keep large-image and memory gap open; do not create product size limit |
| Stop conditions | Requires user data, system-wide settings changes, unbounded resource consumption or admin privilege |
| Cleanup | Release isolated representations and confirm no residual test data; no action now |
| Future result destination | `docs/Research/Technology/results/clipboard/CLIP-SPIKE-009-result.md`；只規劃 |
| Open questions | Product memory threshold、large History behavior、conversion amplification、packaging difference |

### 18.10 CLIP-SPIKE-010 — Clipboard History／Cloud behavior boundary

| Field | Plan |
|---|---|
| Spike ID | CLIP-SPIKE-010 |
| Title | Clipboard History／Cloud behavior boundary |
| Status | Planned |
| Related criteria | CLIP-019, CLIP-020, CLIP-021, CLIP-022 |
| Related gates | CLIP-GATE-008, CLIP-GATE-009, CLIP-GATE-010 |
| Related gaps | CLIP-GAP-014, CLIP-GAP-017, CLIP-GAP-018 |
| Candidate technologies | Candidate-specific publication route；不預選、不排名 |
| Eligible Hosts | WPF、WinUI 3、Win32／OLE；Cloud surface 只有在隔離條件允許時 |
| Purpose | 規劃 History enabled／disabled、Cloud disabled／enabled、format materialization、process end 與 privacy boundary 的觀察 |
| Dependencies | Isolation environment、privacy authority、History／Cloud documentation、basic and multi-format plans |
| Preconditions | 不修改使用者設定；不登入額外帳號；需有明確 isolated session 與 cloud authority |
| Isolation environment | Disposable VM、dedicated test user 或明確核准的 isolated desktop session |
| Synthetic input | Synthetic image，含 unique run identifier，不含私人資料 |
| Clipboard formats | One or more previously approved representations; format selection separately logged |
| Producer configuration | History／Cloud state recorded as Enabled／Disabled／Not inspected; no setting mutation |
| Consumer configuration | History surface、isolated test consumer、Cloud surface only if separately authorized |
| Thread／Apartment configuration | Inherit approved host route; record thread／dispatcher／COM |
| Packaging configuration | Packaged／unpackaged separately; no product conclusion from one mode |
| Execution outline | 未來在隔離環境觀察 materialization、format selection、process end、large image、privacy exposure；不操作現有 user data |
| Required evidence | Environment record、settings state as observed、materialization observation、format identity、privacy review、cleanup |
| Functional pass condition | 只要求 observation boundary 與 privacy controls 可重現；不預填 History／Cloud behavior |
| Format interoperability verification | History／Cloud presentation is separate from consumer paste fidelity |
| Alpha／pixel verification | Only if isolated consumer permits safe synthetic comparison; no real data |
| Threading verification | Record publication thread and any surface／consumer boundary |
| Contention／retry observation | Not primary; record interruptions without changing system policy |
| Ownership／lifetime observation | Observe process end／materialization only where safe and authorized |
| Memory fields | Synthetic size, representation size, observed materialization limits |
| History／Cloud boundary | Central subject; never modify settings, account, device or sync destination |
| Parallel File Output observation | History／Cloud outcome cannot gate File Output; output paths remain independent |
| Privacy checks | Synthetic only; no private Clipboard read, backup, restore or cloud transfer |
| Failure condition | Isolation, authorization, account boundary, cleanup or privacy cannot be proven |
| Failure implication | History／Cloud evidence remains Unknown; no product privacy conclusion |
| Stop conditions | Requires account login, settings change, unauthorized network, private content or cross-device transfer |
| Cleanup | Remove only isolated synthetic residue under approved procedure; no user Clipboard cleanup |
| Future result destination | `docs/Research/Technology/results/clipboard/CLIP-SPIKE-010-result.md`；只規劃 |
| Open questions | History format choice、Cloud boundary、process end、large image behavior、privacy retention |

### 18.11 CLIP-SPIKE-011 — Packaged／unpackaged comparison

| Field | Plan |
|---|---|
| Spike ID | CLIP-SPIKE-011 |
| Title | Packaged／unpackaged comparison |
| Status | Planned |
| Related criteria | CLIP-004, CLIP-005, CLIP-013, CLIP-014, CLIP-017, CLIP-018, CLIP-021, CLIP-022 |
| Related gates | CLIP-GATE-003, CLIP-GATE-005, CLIP-GATE-006, CLIP-GATE-007, CLIP-GATE-010 |
| Related gaps | CLIP-GAP-004, CLIP-GAP-005, CLIP-GAP-015, CLIP-GAP-016 |
| Candidate technologies | All five candidates；same Candidate／Host pair must be compared across package modes |
| Eligible Hosts | WPF、WinUI 3、Win32／OLE |
| Purpose | 規劃 packaged 與 unpackaged desktop context 的 Clipboard、thread、format、lifetime、privacy 與 cleanup 差異 |
| Dependencies | Candidate／Host matrix、packaging authority、basic publication、threading、ownership plans |
| Preconditions | Actual package mode、build configuration、identity、privilege 與 runtime authority separately approved |
| Isolation environment | Same disposable VM／test user where possible；package mode is the controlled variable |
| Synthetic input | Same synthetic image identity、dimensions、pixel、alpha、color metadata |
| Clipboard formats | Same candidate-specific representation for both package modes |
| Producer configuration | Same host／format／thread conditions; only packaging state changes |
| Consumer configuration | Same isolated consumer set; results not mixed |
| Thread／Apartment configuration | Same UI／background／STA／MTA condition within pair |
| Packaging configuration | One Packaged run and one Unpackaged run；actual manifest／identity recorded only at execution |
| Execution outline | 未來以 controlled pair 比較 publication、consumer、thread、ownership、memory、History／Cloud boundary 與 cleanup |
| Required evidence | Environment, package state, API identity, format, thread, consumer, ownership, privacy, cleanup evidence |
| Functional pass condition | Pair differences are attributable to packaging state with all other controlled fields equal；不預填相容性結果 |
| Format interoperability verification | Compare same representation separately；no cross-mode inference without evidence |
| Alpha／pixel verification | Same synthetic markers and comparison method across pair |
| Threading verification | Same apartment／dispatcher contract and separate result records |
| Contention／retry observation | Record package-specific failure if observed; no retry policy |
| Ownership／lifetime observation | Compare process／package identity effects and cleanup |
| Memory fields | Same dimensions、representation size、peak observation、package overhead if measurable |
| History／Cloud boundary | Do not change settings; package mode cannot be used to alter privacy surface |
| Parallel File Output observation | Package difference in Clipboard route does not govern File Output |
| Privacy checks | Synthetic only; no package account／identity retention beyond necessary environment record |
| Failure condition | Package state cannot be proven, controlled variable contaminated, or cleanup differs unsafely |
| Failure implication | Packaging impact remains Unknown；do not select packaged／unpackaged product mode |
| Stop conditions | Requires installer／deployment change, admin action, account login or production project mutation |
| Cleanup | Remove only isolated test artifacts under approved procedure; no production package changes |
| Future result destination | `docs/Research/Technology/results/clipboard/CLIP-SPIKE-011-result.md`；只規劃 |
| Open questions | Windows App SDK／WinRT package effect、WPF package effect、native interop、History boundary |

### 18.12 CLIP-SPIKE-012 — Clipboard failure independent from File Output

| Field | Plan |
|---|---|
| Spike ID | CLIP-SPIKE-012 |
| Title | Clipboard failure independent from File Output |
| Status | Planned |
| Related criteria | CLIP-009, CLIP-015, CLIP-016, CLIP-021, CLIP-022 |
| Related gates | CLIP-GATE-004, CLIP-GATE-009, CLIP-GATE-010 |
| Related gaps | CLIP-GAP-003, CLIP-GAP-016, CLIP-GAP-018 |
| Candidate technologies | Candidate-specific Clipboard route plus independently defined File Output observation boundary；不選定 implementation |
| Eligible Hosts | WPF、WinUI 3、Win32／OLE；Host separately recorded |
| Purpose | 規劃證明 Clipboard failure 不會讓 File Output 失敗、重跑 Capture／Rendering 或改寫 shared workflow state |
| Dependencies | `SPEC-0010` parallel output boundary、`COMP-009`／`COMP-015` ownership、contention／retry plan、synthetic input |
| Preconditions | 只可在未來有明確 output observation authority 時執行；本文件不授權正式 workflow mutation |
| Isolation environment | Isolated synthetic test environment；Clipboard 與 File Output destination 均需不含私人資料 |
| Synthetic input | Fixed synthetic image identity；不使用正式 Capture／Annotation output |
| Clipboard formats | One approved candidate representation per scenario；format result separate |
| Producer configuration | Clipboard route intentionally produces classified failure or blocked condition；File Output route remains independent |
| Consumer configuration | No external consumer required for failure independence; optional isolated synthetic observer only |
| Thread／Apartment configuration | Record Clipboard route thread／dispatcher／COM state and independent output observation context |
| Packaging configuration | Packaged／unpackaged separate if output boundary depends on host mode |
| Execution outline | 未來在隔離 environment 觀察 Clipboard failure、File Output independent result、shared state preservation、no Capture／Rendering rerun、cleanup |
| Required evidence | Initial／final Clipboard result、File Output independent result、workflow state observation、retry record、no-rerun evidence、privacy review |
| Functional pass condition | Clipboard failure remains localized, File Output decision is independent, and no Capture／Rendering rerun or shared state mutation occurs；不預填結果 |
| Format interoperability verification | Not primary; fixed representation only to create the approved Clipboard failure boundary |
| Alpha／pixel verification | Not primary; confirm synthetic identity remains unchanged；完整 fidelity由其他 Spike覆蓋 |
| Threading verification | Record failure thread／dispatcher／COM condition and independent File Output context |
| Contention／retry observation | Include initial contention or bounded retry observation only as authorized; retry must not touch File Output state |
| Ownership／lifetime observation | Clipboard failure cleanup must not own or delete File Output data |
| Memory fields | Clipboard attempt memory、File Output independent memory if authorized、no shared payload inference |
| History／Cloud boundary | Failure test must not use History／Cloud settings or real user content |
| Parallel File Output observation | Central subject; success／failure is independently classified and not converted into Clipboard result |
| Privacy checks | Synthetic only; no file path, window title, Clipboard bytes or user data in diagnostics |
| Failure condition | Clipboard failure propagates to File Output, changes shared state, reruns Capture／Rendering, or cleanup is ambiguous |
| Failure implication | Architecture boundary is not evidence-complete; do not revise product workflow in this plan |
| Stop conditions | Requires formal application source change, production state mutation, real screenshot, private data or unapproved output write |
| Cleanup | Remove only isolated synthetic output under approved procedure; never clear user Clipboard |
| Future result destination | `docs/Research/Technology/results/clipboard/CLIP-SPIKE-012-result.md`；只規劃 |
| Open questions | Failure classification boundary、shared state preservation evidence、retry ownership、parallel output observation |

## 19. Gate Coverage Matrix

| Spike | CLIP criteria | CLIP Gate | Candidates | Hosts | Format／thread scenario | Evidence required |
|---|---|---|---|---|---|---|
| CLIP-SPIKE-001 | CLIP-001, 002, 006, 009, 010, 011, 013, 017, 022 | 001, 002, 003, 006, 010 | OPT-001, OPT-005 WPF adapter | WPF | Basic bitmap／UI STA | Format, consumer, pixel／alpha, thread, cleanup |
| CLIP-SPIKE-002 | CLIP-001, 003, 004, 006, 009, 010, 011, 013, 017, 022 | 001, 002, 003, 006, 007, 010 | OPT-002, OPT-005 WinUI adapter | WinUI 3 | WinRT bitmap／UI thread | API, package, format, consumer, thread, cleanup |
| CLIP-SPIKE-003 | CLIP-006, 007, 010, 011, 012, 017, 022 | 001, 002, 005, 006, 010 | OPT-003, OPT-004 | WPF, WinUI 3, Win32／OLE | DIB／DIBV5／native format | Header, mask, stride, Alpha, pixels, lifetime |
| CLIP-SPIKE-004 | CLIP-006, 008, 009, 010, 011, 012, 017, 022 | 001, 002, 005, 006, 010 | OPT-003, OPT-004, OPT-005 | WPF, WinUI 3, Win32／OLE | PNG stream／decoder | Stream, decoded pixels, Alpha, color, lifetime |
| CLIP-SPIKE-005 | CLIP-006, 007, 008, 009, 010, 011, 012, 017, 018 | 001, 002, 005, 006, 009, 010 | OPT-001..005 | WPF, WinUI 3, Win32／OLE | Multiple formats／consumer selection | Enumeration, atomicity observation, per-format fidelity, ownership |
| CLIP-SPIKE-006 | CLIP-003, 004, 013, 014, 017, 018, 021 | 003, 004, 005, 006, 010 | OPT-001..005 | WPF, WinUI 3, Win32／OLE | STA／MTA／COM／Dispatcher | Apartment, COM, dispatcher, cancellation, cleanup |
| CLIP-SPIKE-007 | CLIP-015, 016, 017, 021, 022 | 004, 005, 009, 010 | Candidate-specific | WPF, WinUI 3, Win32／OLE | Contention／bounded retry | Initial failure, timing, attempts, cancellation, parallel output |
| CLIP-SPIKE-008 | CLIP-017, 018, 019, 021 | 005, 008, 010 | OPT-001..005 | WPF, WinUI 3, Win32／OLE | Ownership／immediate／delayed／process end | Lifetime timeline, owner, consumer access, cleanup |
| CLIP-SPIKE-009 | CLIP-010, 011, 012, 017, 020, 021, 022 | 001, 002, 005, 008, 010 | OPT-001..005 | WPF, WinUI 3, Win32／OLE | Large synthetic image／memory | Environment, size, allocation, memory, failure, cleanup |
| CLIP-SPIKE-010 | CLIP-019, 020, 021, 022 | 008, 009, 010 | Candidate-specific | WPF, WinUI 3, Win32／OLE | History／Cloud boundary | Observed state, materialization, privacy, cleanup |
| CLIP-SPIKE-011 | CLIP-004, 005, 013, 014, 017, 018, 021, 022 | 003, 005, 006, 007, 010 | OPT-001..005 | WPF, WinUI 3, Win32／OLE | Packaged／unpackaged paired comparison | Package state, API, format, thread, lifecycle, cleanup |
| CLIP-SPIKE-012 | CLIP-009, 015, 016, 021, 022 | 004, 009, 010 | Candidate-specific | WPF, WinUI 3, Win32／OLE | Clipboard failure／independent File Output | Failure isolation, no rerun, state preservation, parallel result |

Coverage rules：

- `CLIP-001..022` 必須至少有 official evidence 或明確 Runtime Spike route；若仍無路徑，先建立 `CLIP-PLAN-GAP-xxx`。
- `CLIP-GATE-001..010` 均有至少一條 future execution path，但本文件不將 path 視為 gate satisfied。
- `CLIP-GAP-001..018` 均有後續 evidence route；Gap close 必須由未來 evidence review 決定。
- 不得以單一 basic publication Spike 關閉 Alpha、Contention、Lifetime、History 或 Cloud gate。
- 本計畫目前沒有新發現的未覆蓋 planning item；若後續矩陣出現未覆蓋項目，必須建立 `CLIP-PLAN-GAP-xxx` 後才可進入 execution readiness review。

## 20. Execution Phases

### Phase L1 — Basic Publication and Format Interoperability

| Field | Plan |
|---|---|
| Included | CLIP-SPIKE-001、002、003、004、005 |
| Entry criteria | Candidate／Host identity、synthetic image、isolated environment、format／consumer authority、Clipboard runtime authority |
| Exit criteria | Basic publication、DIB／DIBV5、PNG、multi-format 的 evidence path 可獨立 review；不代表 technology accepted |
| Required Candidates | 依各 Spike binding；不得排名 |
| Required Hosts | WPF、WinUI 3、Win32／OLE as applicable |
| Required formats | Framework bitmap、DIB／DIBV5、PNG、multi-format as separately authorized |
| Required evidence | Format enumeration、consumer observation、pixel／alpha、color、ownership、cleanup |
| Blocking conditions | No isolation、no Clipboard authority、private data risk、unknown cleanup、missing consumer boundary |
| Privacy boundary | Synthetic data only；不修改 History／Cloud |
| Cleanup requirement | Every representation and process lifetime must have approved cleanup path |
| Authorization dependency | Clipboard read／write／clear、runtime、evidence write、project/build authority all required separately |

Phase L1 不需要等待完整 Cloud Clipboard 驗證；但不得把 L1 結果外推為 History／Cloud、Contention、Lifetime 或產品決策結論。

### Phase L2 — Threading、Contention and Lifetime

| Field | Plan |
|---|---|
| Included | CLIP-SPIKE-006、007、008、009 |
| Entry criteria | L1 的相關 representation boundary 已可 review；thread／COM、retry、ownership、memory authority 已核准 |
| Exit criteria | Thread、contention、bounded observation、ownership／process lifetime、large-image memory evidence 可獨立 review |
| Required Candidates | Candidate-specific route；不要求所有 Candidate 同時執行 |
| Required Hosts | WPF、WinUI 3、Win32／OLE as applicable |
| Required formats | Fixed representation per scenario；format change must be separately recorded |
| Required evidence | Apartment／COM、dispatcher、attempt timing、ownership、memory、cleanup、parallel boundary |
| Blocking conditions | No isolated contention source、no bounded authority、unbounded memory risk、unknown lifetime |
| Privacy boundary | Synthetic only；owner identity anonymous；不得保存既有 Clipboard |
| Cleanup requirement | Normal、cancelled、timeout、abnormal shutdown 都須有 cleanup path |
| Authorization dependency | Runtime、Clipboard write、cross-process observation、evidence write authority required |

### Phase L3 — Platform Integration and Output Independence

| Field | Plan |
|---|---|
| Included | CLIP-SPIKE-010、011、012 |
| Entry criteria | History／Cloud privacy boundary、packaging authority、parallel output observation authority 已明確 |
| Exit criteria | History／Cloud、packaged／unpackaged、Clipboard failure／File Output independence evidence 可分別 review |
| Required Candidates | Candidate-specific route；不得以單一 route 宣告全平台結果 |
| Required Hosts | WPF、WinUI 3、Win32／OLE as applicable |
| Required formats | Prior approved representation；format result not inferred from platform phase |
| Required evidence | Settings observation、privacy、package state、failure isolation、no rerun、cleanup |
| Blocking conditions | Account／device authorization missing、setting mutation required、private data risk、formal workflow change required |
| Privacy boundary | No user settings change、no extra account、no unauthorized cloud/network transfer |
| Cleanup requirement | No residual synthetic materialization; no user Clipboard clear |
| Authorization dependency | Isolated History／Cloud、runtime、evidence write、output observation and workflow boundary authority |

## 21. Stop Rules

未來執行任一 Spike 時，以下任一情況必須立即停止並將 status 保留為 `Blocked` 或另行 review：

- 不在隔離環境。
- Clipboard 中可能存在需要保存的私人內容。
- 需要讀取使用者既有 Clipboard。
- 需要清除或備份使用者既有 Clipboard。
- 需要修改 History 或 Cloud Clipboard 設定。
- 需要登入或同步未授權帳號／裝置。
- 需要管理員權限但未獲得明確授權。
- 需要未授權網路傳輸。
- Evidence 會包含私人 image bytes、Credential、Window title 或私人路徑。
- Consumer 操作超出核准範圍。
- 實際 API、format、Host 或 package mode 與核准 Spike 不一致。
- Publication 造成持續性 Clipboard 污染且 cleanup 無法確認。
- 需要修改正式 Workflow、Shared State 或 application-wide error state。
- 需要重新執行 Capture 或 Rendering。
- 需要建立產品 Source Code、Project、Restore、Build、Runtime 或 Evidence write 操作但未獲核准。
- File Output 結果被 Clipboard failure／retry 直接控制。
- Clipboard failure 需要改寫已完成的 Annotation／Selection state。

## 22. Result Artifact Plan

只規劃，不建立：

```text
docs/Research/Technology/results/clipboard/
```

未來預計可有：

- `CLIP-SPIKE-001-result.md` 至 `CLIP-SPIKE-012-result.md`。
- Environment records。
- Synthetic image specification。
- Producer payload metadata。
- Consumer observations。
- Pixel／alpha comparison。
- Threading records。
- Contention／retry records。
- Ownership／lifetime records。
- Memory observations。
- History／Cloud observations。
- Parallel File Output observations。
- Privacy review。
- Cleanup confirmation。

本輪不得建立上述目錄、檔案、payload、Result 或 Evidence Artifact。

## 23. Decision Evidence Roll-up

未來 review 使用下列表格；本文件不預填實際 Result 或 Candidate ranking：

| CLIP Gate | Candidate | Host | Format／thread scenario | Result | Evidence completeness | Clipboard decision impact |
|---|---|---|---|---|---|---|
| CLIP-GATE-001..010 | Candidate-specific | Host-specific | Scenario-specific | 留空至未來執行 | 留空至 evidence review | `Supports candidate`／`Challenges candidate`／`Neutral`／`Insufficient evidence` |

`Clipboard decision impact` 只能使用：

- `Supports candidate`
- `Challenges candidate`
- `Neutral`
- `Insufficient evidence`

任一 impact 不得直接改寫 `TD-004` status；需另行建立與 review Clipboard ADR。

## 24. Readiness to Execute

Readiness 必須由下列矩陣機械式推導：

```text
Candidate／Host identity
  + isolated Clipboard environment
  + Synthetic image specification
  + Format／Consumer contract
  + Threading／COM contract
  + Evidence and privacy controls
  + Project／Restore／Build authority
  + Clipboard read／write／clear authority
  + Runtime authority
  + Evidence write authority
  → Readiness to Execute
```

目前推導：

| Input | Current state | Effect |
|---|---|---|
| Candidate／Host identity | Partially defined; runtime identity not verified | Not sufficient |
| Isolated Clipboard environment | Planned only | Blocking |
| Synthetic image specification | Planned only; no image created | Blocking |
| Format／Consumer contract | Planned; no interoperability evidence | Blocking |
| Threading／COM contract | Planned; no runtime observation | Blocking |
| Evidence and privacy controls | Planned; no artifact or review | Blocking |
| Project／Restore／Build authority | Not granted | Blocking |
| Clipboard read／write／clear authority | No | Blocking |
| Runtime authority | Not granted | Blocking |
| Evidence write authority | No | Blocking |
| Readiness to Execute | `Not ready` | Do not execute |

固定目前狀態：

- Execution Status: `Not started`
- Build Verification: `Not performed`
- Runtime Verification: `Not performed`
- Clipboard Runtime Spike Authorized: `No`
- Clipboard Read Authorized: `No`
- Clipboard Write Authorized: `No`
- Clipboard Clear Authorized: `No`
- Evidence Write Authorized: `No`
- Clipboard Decision: `Not made`
- Capture Decision: `Not made`
- Rendering Decision: `Not made`

## 25. Traceability

```text
Frozen requirement
  → CLIP criterion
  → CLIP Gate
  → CLIP Spike
  → Future Runtime Evidence
  → Future Clipboard Decision
```

| Source | Use in this plan |
|---|---|
| `docs/Research/Technology/29-clipboard-integration-feasibility.md` / `RESEARCH-TECH-CLIPBOARD-001` | Upstream candidate、criterion、gate、gap、format、privacy、ownership 與 spike binding |
| `Architecture/TECHNOLOGY-DECISION-ROADMAP.md` / `TD-004` | Clipboard Integration 仍為 Candidate；不升級為 Ready 或 Accepted |
| `Architecture/adr/ADR-0002-ui-framework-selection.md` / `ADR-0002` | UI Framework 仍 Draft；本計畫不決定 WPF／WinUI 3 |
| `PRD/PRD-0005-functional-requirements.md` / `FR-007` | Deliver result to clipboard 的 frozen functional requirement |
| `Specs/SPEC-0007-clipboard-handoff.md` / `FEAT-003` | Capture Result → Clipboard Ready → abstract Clipboard Consumer handoff boundary |
| `Specs/SPEC-0010-feature-integration.md` | Clipboard 與 File Output parallel、failure independence 與 workflow boundary |
| `Architecture/ARCH-0001-product-boundaries.md` / `ARCH-0001` | Product boundary、shared state 與長期維護責任 |
| `Architecture/ARCH-0002-system-context.md` / `ARCH-0002` | System context 與外部 platform boundary |
| `Architecture/ARCH-0003-layered-architecture.md` / `ARCH-0003` | Platform Integration Layer 與上層責任隔離 |
| `Architecture/ARCH-0004-component-responsibilities.md` / `ARCH-0004` | Component ownership，包含 `COMP-009`／`COMP-015` |
| `Architecture/ARCH-0005-integration-contracts.md` / `ARCH-0005` | Integration contract、Clipboard handoff 與 independent output boundary |
| `RESEARCH-TECH-UI-001` | UI framework research line；本計畫不改寫其結論 |
| `RESEARCH-TECH-CAPTURE-001` | Capture research line；Clipboard failure 不得重跑 Capture |
| `RESEARCH-TECH-RENDER-001` | Rendering research line；本計畫不決定 Rendering |

上游文件的實際名稱與 ID 必須從 Repository 原樣引用，不得因 runtime plan 自行猜測或新增產品決策。

## Completion Conditions

- 只建立 `30-clipboard-integration-runtime-spike-plan.md`。
- 保留 `CLIP-SPIKE-001..012` 原始 ID、名稱與目的。
- 十二個 Spike 的 Status 全部為 `Planned`。
- 建立 Candidate Eligibility Matrix，不形成 Candidate ranking。
- 建立 Controlled Comparison Rules、Synthetic Image Contract 與 Clipboard Isolation Environment。
- 建立 Format、Consumer、Threading／COM、Contention／Retry、Ownership／Lifetime contracts。
- 建立 History／Cloud Clipboard observation boundary、Environment Record 與 Evidence Types。
- 每個 Spike 具備固定 execution fields、pass condition、failure implication、stop conditions 與 cleanup boundary。
- 覆蓋 `CLIP-001..022`、`CLIP-GATE-001..010` 與 `CLIP-GAP-001..018`。
- 建立 Phase L1–L3、Stop Rules、Result Artifact Plan、Decision Evidence Roll-up 與 Readiness to Execute。
- Readiness 由矩陣推導為 `Not ready`。
- 不讀取、寫入或清除 Windows Clipboard。
- 不建立 Result directory、Clipboard payload、Project、Prototype、Source Code 或 Evidence Artifact。
- 不執行下載、安裝、Restore、Build、Run、Publish、Test 或 Runtime Spike。
- 不修改 UI／Capture／Rendering Research Line、上游 feasibility 文件或 `ADR-0002`。
- 不選擇 Clipboard Technology，不建立 Clipboard ADR，不開始正式 Clipboard 或截圖功能。
- 唯讀檢查應確認 `git diff --check` 通過。

## Current Execution Record

| Field | Value |
|---|---|
| Document created | This planning document only |
| Clipboard read | Not performed |
| Clipboard write | Not performed |
| Clipboard clear | Not performed |
| Payload created | No |
| Project／Prototype created | No |
| Result／Evidence artifact created | No |
| Restore／Build／Run／Publish／Test | Not performed |
| Runtime spike | Not performed |
| Technology selection | Not made |
| Clipboard ADR | Not created |
| Screenshot feature | Not started |
