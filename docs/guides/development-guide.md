# Development Guide

狀態：`Draft`

本指南定義 SnipPlus 從 Frozen requirements 到 implementation 的工作流程。Repository 已完成 PRD、Specification 與 abstract Architecture baseline，且 UI Framework ADR 已 Accepted；但尚未通過 implementation readiness，也沒有 application source code、build configuration 或 runtime evidence。

## 1. Current lifecycle position

| Area | Current state |
| --- | --- |
| PRD v1.0 | Freeze Approved |
| Specification v1.0 | Freeze Approved |
| Architecture baseline | Freeze Approved |
| ADR-0002 UI Framework | Accepted; WinUI 3 |
| Rendering Technology | Candidate; next primary decision |
| Capture Backend、Clipboard、Image Representation、Testing | Candidate |
| Contracts and Project Structure | Incomplete |
| Implementation | Not started |
| Verification | Not started |

Start with [Repository Current State and Implementation Readiness Audit](../REPOSITORY-CURRENT-STATE-AND-IMPLEMENTATION-READINESS-AUDIT.md) and [Technology Decision Roadmap](../../Architecture/TECHNOLOGY-DECISION-ROADMAP.md).

## 2. 變更前先判斷文件層級

| 問題 | 應更新的文件 |
| --- | --- |
| 外部世界實際如何運作？ | `docs/Research/` |
| 研究結果的流程與狀態如何整理？ | `docs/Analysis/` |
| 是否採用某個產品 workflow？ | `docs/Decision/` |
| 為誰解決什麼問題？ | `PRD/` |
| 使用者可觀察到什麼行為？ | `Specs/` |
| 系統如何分層與分配責任？ | `Architecture/ARCH-*` |
| 為什麼選擇某個長期技術方案？ | `Architecture/adr/` |
| Interface、data、failure 或 lifecycle contract 是什麼？ | Architecture contract package or explicitly authorized detailed-design area |
| Solution、Project 與 assembly 如何映射？ | Project Structure artifact |
| 是否可以開始實作？ | Implementation Readiness Review |

不要用 Architecture 取代 PRD，也不要把 Research、Draft ADR 或 UI Wireframe 當成 implementation contract。

## 3. Current recommended workflow

1. 使用 Frozen PRD、Specs 與 Architecture 確認決策邊界。
2. 只把 `Accepted` ADR 當成有效 technology source。
3. 依 Decision Roadmap 推進剩餘核心決策，目前先處理 Rendering Technology。
4. 建立必要的 consolidated contracts，不為每個欄位建立獨立 closure chain。
5. 建立 Component-to-project mapping 與 Solution／Project Structure。
6. 定義 setup、format、lint、test、build 與 CI plan。
7. 定義第一個 vertical slice、non-goals、acceptance criteria、cleanup 與 rollback。
8. 完成一份 Implementation Readiness Review。
9. 只有明確授權的 implementation task 才能建立 source code。
10. 實作後建立 tests、runtime evidence、CHANGELOG 與 verification result。

## 4. Accepted UI framework boundary

[ADR-0002](../../Architecture/adr/ADR-0002-ui-framework-selection.md) 已接受 WinUI 3 作為 Desktop UI Framework。

這代表後續決策可以假設：

- 主要 desktop presentation host 使用 WinUI 3。
- UI presentation 不得繞過 Frozen Platform Integration boundaries。
- Windows Fluent-first 和 Windows-first 保持有效。

這不代表已決定：

- C# 或 C++。
- .NET／Runtime version。
- Windows App SDK version。
- Rendering Technology。
- Capture Backend。
- Clipboard API。
- Image Representation。
- Packaging mode。
- Testing framework。
- Solution／Project Structure。

ADR acceptance 不授權建立 Project、Restore、Build、Test 或 source code。

## 5. Next ADR — Rendering Technology

建立 Rendering ADR 時應直接使用 `docs/Research/Technology/10–18`，並回答：

- Rendering responsibility 位於哪些 Frozen Module／Component boundaries。
- Overlay／preview／annotation／final image 的 rendering concerns 如何分離。
- Win2D、SkiaSharp、Windows Composition／Direct2D 或其他候選的適用範圍與代價。
- DPI、HDR、alpha、pixel format、color space 和 lifetime 哪些屬於 Rendering，哪些屬於 Image Result contract。
- UI framework 與 rendering engine 是否需要隔離 adapter boundary。
- 是否需要 runtime spike；若需要，只建立一次明確授權的 execution task 與 result artifact。

Rendering ADR 不得同時選 Capture API、Clipboard API、Image file format 或 Project Structure。

## 6. ADR workflow

### Candidate → Draft ADR

建立 ADR 前必須具備：

- 明確的 Architecture requirement、finding 或 Decision Roadmap item。
- Decision context、drivers、options、risks 與 non-goals。
- 可引用的 Research evidence。
- 已 Accepted 的 upstream dependency，或明確記錄的 dependency risk。
- 不會改寫 Frozen PRD、Specs 或 Architecture ownership。

### Draft ADR → Accepted

必須：

- 完成 Review Record。
- 有明確 reviewer 與 acceptance authority。
- 解決 blocking comments。
- 記錄 options、trade-offs、positive／negative／neutral consequences 與 supersession。
- 同步更新 Decision Roadmap、README、TODO、CHANGELOG 和相關 index。

Draft proposal 不得被當作有效決策。

## 7. Documentation anti-proliferation rule

不要因為文件結論是 `Not ready` 就自動建立下一份 prerequisite、readiness reassessment、authorization request 或 closure review。

只有下列情況可新增治理文件：

- 新 external evidence；
- 新 human／authority decision；
- Accepted upstream change；
- 新 runtime／implementation evidence；
- materially different decision boundary。

Clipboard D1 039→052 documentary chain 已完成並停止。後續 Clipboard 工作應收斂為 ADR、contract 或明確 runtime task。

## 8. Implementation readiness minimum

開始 Coding 前至少必須確認：

- 必要核心 ADR 已 Accepted 或明確 Deferred。
- Shared Result／Image Result contract 已定義。
- Capture、Clipboard、Output 與 Failure boundary 已定義。
- Component-to-project mapping 已建立。
- Solution／Project Structure 已建立。
- Language、Runtime、SDK versions 已決定並可追溯。
- setup、format、lint、test、build 與 CI plan 可執行。
- 第一個 vertical slice 有明確 scope、non-goals 與 acceptance criteria。
- Verification evidence 與 cleanup expectation 已定義。
- Implementation Readiness Review 明確允許開始。

## 9. 文件變更檢查

文件變更至少確認：

- 狀態標記正確。
- H1、章節階層與檔名符合規則。
- 新增與改名文件已加入 index。
- README、ROADMAP、TODO 與 CHANGELOG 反映狀態變更。
- 相對連結指向存在文件。
- PRD、Specs、Architecture 與 ADR 沒有語意衝突。
- Draft／Candidate 沒有被誤寫成 Accepted。
- Accepted ADR 的下游狀態已同步。
- 沒有以文件數量取代 decision 或 evidence。

## 10. 未來實作的最低品質門檻

技術棧 Accepted 後，至少建立：

- 可重現的本機 setup 指令。
- 可單獨執行的 format、lint、test 與 build 指令。
- CI 對應檢查。
- 可定位 failure 的 logging 與 error handling。
- Unit、integration、platform 與 visual verification boundary。
- Release、rollback 與資料相容性說明。

實際指令應在 Language、Runtime、SDK 與 Project Structure 確定後補入，不先寫猜測指令。
