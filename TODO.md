# TODO

狀態：`Accepted`

本清單只保留目前仍未完成、且具有明確下一步的工作。文件數量不是進度指標；沒有新 evidence 或 decision 時，不建立新的 prerequisite／closure 文件。

## P0 — 核心技術決策

- [x] UI Framework ADR Accepted：`ADR-0002`，WinUI 3。
- [x] Rendering Technology ADR Accepted：`ADR-0003`，WinUI XAML／Composition + Win2D rendering adapter。
- [ ] 建立並審查 Capture Backend ADR。**目前下一個主要任務。**
- [ ] 建立並審查 Image Representation ADR。
- [ ] 建立並審查 Clipboard Integration ADR。
- [ ] 建立並審查 Testing Strategy ADR。

每份 ADR 只處理一個重大決策，直接使用既有 Research／Architecture evidence，不先建立新的 authorization-request 或 closure-review 鏈。

### Capture Backend ADR 必須回答

- 第一個 vertical slice 使用哪個 Windows capture backend。
- Platform capture API 與 Capture orchestration／workflow ownership 如何分離。
- Capture result 如何進入 Shared Result／Image Representation boundary。
- Window、display、region 或其他 capture modes 哪些屬於初始 scope，哪些 Deferred。
- Cancellation、permission、unsupported target、device-loss 與 failure 如何回報。
- 是否需要受控 runtime verification；若需要，只建立一個明確 execution task 與一份 result artifact。

## P0 — 工程契約與 Project Structure

- [ ] 定義 Shared Result／Image Result contract 與 ownership。
- [ ] 定義 Rendering contract，包括 render intent、coordinate spaces、alpha／color、resource recovery 與 final raster boundary。
- [ ] 定義 Capture Backend boundary contract。
- [ ] 定義 Clipboard Handoff contract，包括 success、failure、retry、preservation 與 cleanup。
- [ ] 定義 Output Delivery contract。
- [ ] 定義 recoverable／terminal failure 與 retry contract。
- [ ] 決定 component interaction 的 sync／async boundary。
- [ ] 決定 C#／.NET／Windows App SDK／Win2D version baseline。
- [ ] 建立 Component-to-project／assembly mapping。
- [ ] 建立 Solution／Project Structure。
- [ ] 更新 Development Guide，加入可重現的 setup、format、lint、test 與 build plan。

相關 Architecture findings：`ARCH-FIND-004` 至 `ARCH-FIND-009`。

## P0 — Implementation readiness

- [ ] 建立一份 repository-wide Implementation Readiness Review。
- [ ] 定義第一個 vertical slice 的 scope、non-goals 與 acceptance criteria。
- [ ] 定義 verification evidence、test boundary、cleanup 與 rollback expectation。
- [ ] 確認 coding task 具有明確授權；在此之前不得建立 application source code。

## P1 — Repository consistency

- [x] 建立 Repository Current State and Implementation Readiness Audit。
- [x] 將 README 與 AGENTS 對齊 Frozen PRD／Specs／Architecture 現況。
- [x] 將 ROADMAP 移至 Technology Decisions 階段。
- [x] 維護主要 README、index 與 ADR index。
- [x] 維護 `docs/Research/Technology/README.md`，按研究線分組列出 01–80。
- [x] 確認目前新增文件都有索引入口與正確相對連結。
- [ ] 將狀態漂移與 Markdown link 檢查納入未來 CI。

## P1 — Verification and delivery foundation

- [ ] 建立 CI 檢查，包括 Markdown link、format、build 與 test。
- [ ] 定義 logging 與可定位錯誤的最低要求。
- [ ] 決定 Configuration strategy。
- [ ] 決定 Packaging strategy。
- [ ] 定義版本、發布、rollback 與支援政策。

## Completed baselines

- [x] Research／Analysis／Decision framework。
- [x] PRD v1.0 Freeze Approved。
- [x] Specification v1.0 Freeze Approved。
- [x] Architecture baseline Freeze Approved。
- [x] ADR governance baseline。
- [x] Technology Decision Roadmap。
- [x] UI Framework ADR Accepted：WinUI 3。
- [x] Rendering Technology ADR Accepted：WinUI Composition + Win2D。
- [x] UI Framework、Rendering、Capture Backend 與 Clipboard research chains。
- [x] Clipboard D1 039→052 documentary closure chain completed and stopped。

## Deferred product capabilities

以下項目必須先走 PRD／Spec Change Control，不直接排入 implementation：

- OCR
- Cloud sync
- Sharing and external integrations
- Cross-platform support
- Plugin architecture as a required dependency
- Advanced annotation toolset
- Additional capture modes beyond the first approved vertical slice
