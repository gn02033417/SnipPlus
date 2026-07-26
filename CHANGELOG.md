# Changelog

本檔案依 Keep a Changelog 精神記錄對使用者、維護者與文件治理有意義的變更。尚未有產品版本發布。

## [Unreleased]

### Added

- 建立 Repository 文件入口、Research／Analysis／Decision framework、PRD、Specs、Architecture、ADR governance、Development Guide、Coding Standard、Markdown naming rules、UI Wireframe、CONTRIBUTING、ROADMAP 與 TODO。
- PRD v1.0 完成 Baseline Review、Traceability Matrix 與 Freeze Review；結論為 `Ready for Specs`／`Freeze Approved`。
- Specification v1.0 建立 SPEC-0002 至 SPEC-0010，並完成 Baseline Review；結論為 `Ready for Architecture`／`Freeze Approved`。
- Architecture 建立 ARCH-0001 至 ARCH-0005、Architecture Baseline Review、ADR Baseline 與 Technology Decision Roadmap；結論為 `Ready for ADR and Technology Selection`／`Freeze Approved`。
- 建立 ADR-0002 UI Framework Selection Draft，提出 WinUI 3；目前尚未 Review 或 Accepted。
- 建立 UI Framework Technology Research 01–09，包含 feasibility、runtime spike plan、environment baseline、prerequisite closure 與 authorization-request records；未執行 runtime spike。
- 建立 Rendering Technology Research 10–18；保留 feasibility、evidence、readiness 與 inspection boundaries，未形成 Accepted ADR。
- 建立 Capture Backend Research 20–28；保留 feasibility、evidence、readiness 與 inspection boundaries，未形成 Accepted ADR。
- 建立 Clipboard Integration Research 29–80，包含 feasibility、runtime planning、evidence packages、D1 governance／privacy／authorization controls與 closure reviews；未執行 Clipboard inspection、runtime test 或 coding。
- 完成 Clipboard D1 039→052 documentary chain。最終狀態仍為 Artifact Creation Permission `No`、Drafting Start Permission `No`、Execution Permission `Not provided`。
- 建立 `docs/PROJECT-LIFECYCLE.md`，集中描述 Research 至 Release 的治理生命週期。
- 建立 `docs/REPOSITORY-CURRENT-STATE-AND-IMPLEMENTATION-READINESS-AUDIT.md`，確認文件基線已完成、implementation readiness 尚未完成，並停止自動延伸同類 prerequisite／closure 文件。

### Changed

- 將 Repository 入口、AGENTS、ROADMAP 與 TODO 對齊實際 Frozen PRD／Specs／Architecture 狀態。
- Roadmap 的 Current Phase 由 Documentation Foundation 改為 Technology Decisions and Implementation Preparation。
- TODO 改為只保留 Accepted ADR、工程契約、Project Structure、Verification Strategy 與 Implementation Readiness 等真正阻擋項。
- 加入文件防增生規則：沒有新 evidence、human decision、accepted change 或 runtime evidence 時，不再建立新的 prerequisite、readiness reassessment、authorization request 或 closure-review 文件。
- 移除空白 `.gitkeep`，讓實際文件目錄取代 placeholder。

### Not released

- 尚無應用程式碼、build configuration、runtime verification、automated tests、package、deploy 或 release artifact。
- ADR-0002 尚未 Accepted；Rendering、Capture Backend、Clipboard Integration、Image Representation 與 Testing Strategy 尚未成為有效技術決策。
