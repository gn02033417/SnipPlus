# Development Guide

狀態：`Draft`

本指南定義 SnipPlus 從 Frozen requirements 到 implementation 的工作流程。Repository 已完成 PRD、Specification 與 abstract Architecture baseline，但尚未通過 implementation readiness，也沒有 application source code、build configuration 或 runtime evidence。

## 1. Current lifecycle position

| Area | Current state |
| --- | --- |
| PRD v1.0 | Freeze Approved |
| Specification v1.0 | Freeze Approved |
| Architecture baseline | Freeze Approved |
| ADR-0002 UI Framework | Draft; WinUI 3 proposed, not accepted |
| Other core technology decisions | Candidate |
| Contracts and Project Structure | Incomplete |
| Implementation | Not started |
| Verification | Not started |

Start with [Repository Current State and Implementation Readiness Audit](../REPOSITORY-CURRENT-STATE-AND-IMPLEMENTATION-READINESS-AUDIT.md).

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
2. 將核心 Technology Decision Candidate 收斂為 ADR。
3. Review ADR，只有 `Accepted` ADR 可成為 implementation source。
4. 建立必要的 consolidated contracts，不為每個欄位建立獨立 closure chain。
5. 建立 Component-to-project mapping 與 Solution／Project Structure。
6. 定義 setup、format、lint、test、build 與 CI plan。
7. 定義第一個 vertical slice、non-goals、acceptance criteria、cleanup 與 rollback。
8. 完成一份 Implementation Readiness Review。
9. 只有明確授權的 implementation task 才能建立 source code。
10. 實作後建立 tests、runtime evidence、CHANGELOG 與 verification result。

## 4. ADR workflow

### Candidate → Draft ADR

建立 ADR 前必須具備：

- 明確的 Architecture requirement、finding 或 Decision Roadmap item。
- Decision context、drivers、options、risks 與 non-goals。
- 可引用的 Research evidence。
- 不會改寫 Frozen PRD、Specs 或 Architecture ownership。

### Draft ADR → Accepted

必須：

- 完成 Review Record。
- 有明確 reviewer 與 acceptance authority。
- 解決 blocking comments。
- 記錄 trade-offs、consequences 與 supersession。

Draft proposal 不得被當作有效決策。ADR-0002 目前仍在此階段。

## 5. Documentation anti-proliferation rule

不要因為文件結論是 `Not ready` 就自動建立下一份 prerequisite、readiness reassessment、authorization request 或 closure review。

只有下列情況可新增治理文件：

- 新 external evidence；
- 新 human／authority decision；
- Accepted upstream change；
- 新 runtime／implementation evidence；
- materially different decision boundary。

Clipboard D1 039→052 documentary chain 已完成並停止。後續 Clipboard 工作應收斂為 ADR、contract 或明確 runtime task。

## 6. Implementation readiness minimum

開始 Coding 前至少必須確認：

- 必要核心 ADR 已 Accepted 或明確 Deferred。
- Shared Result／Image Result contract 已定義。
- Capture、Clipboard、Output 與 Failure boundary 已定義。
- Component-to-project mapping 已建立。
- Solution／Project Structure 已建立。
- Runtime／language／SDK versions 已決定並可追溯。
- setup、format、lint、test、build 與 CI plan 可執行。
- 第一個 vertical slice 有明確 scope、non-goals 與 acceptance criteria。
- Verification evidence 與 cleanup expectation 已定義。
- Implementation Readiness Review 明確允許開始。

## 7. 文件變更檢查

文件變更至少確認：

- 狀態標記正確。
- H1、章節階層與檔名符合規則。
- 新增與改名文件已加入 index。
- README、ROADMAP、TODO 與 CHANGELOG 反映狀態變更。
- 相對連結指向存在文件。
- PRD、Specs、Architecture 與 ADR 沒有語意衝突。
- Draft／Candidate 沒有被誤寫成 Accepted。
- 沒有以文件數量取代 decision 或 evidence。

## 8. 未來實作的最低品質門檻

技術棧 Accepted 後，至少建立：

- 可重現的本機 setup 指令。
- 可單獨執行的 format、lint、test 與 build 指令。
- CI 對應檢查。
- 可定位 failure 的 logging 與 error handling。
- Unit、integration 與 platform verification boundary。
- Release、rollback 與資料相容性說明。

實際指令應在技術棧、SDK 與 Project Structure 確定後補入，不先寫猜測指令。
