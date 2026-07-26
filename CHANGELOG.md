# Changelog

本檔案依 Keep a Changelog 精神記錄對使用者、維護者與文件治理有意義的變更。尚未有產品版本發布。

## [Unreleased]

### Added

- 建立 Repository 文件治理、Research／Analysis／Decision、PRD、Specs、Architecture、ADR governance、Development Guide、Coding Standard、ROADMAP 與 TODO。
- PRD v1.0、Specification v1.0 與 Architecture baseline 完成 Freeze Review。
- 建立 UI Framework Research 01–09、Rendering Research 10–18、Capture Backend Research 20–28 與 Clipboard Research 29–80；未執行其中規劃的 runtime spike。
- 完成並停止 Clipboard D1 039→052 documentary chain。
- 建立 Repository Current State and Implementation Readiness Audit，移除過度細分的文件延伸模式。
- 新增 `ADR-0002`：WinUI 3 UI Framework。
- 新增 `ADR-0003`：WinUI XAML／Microsoft.UI.Composition + Win2D rendering adapter。
- 新增 `ADR-0004`：Windows.Graphics.Capture Capture Backend。
- 新增 `ADR-0005`：BGRA8 premultiplied SoftwareBitmap canonical image representation。
- 新增 `ADR-0006`：WinRT DataPackage Clipboard integration with history/roaming disabled by default。
- 新增 `ADR-0007`：MSTest.Sdk + Microsoft.Testing.Platform testing strategy。
- 新增 `Architecture/IMPLEMENTATION-CONTRACTS.md`，定義 workflow、capture、image、render、clipboard、output、failure、retry、thread 與 cleanup contracts。
- 新增 `Architecture/PROJECT-STRUCTURE.md`，固定 C# 14、.NET 10.0.302、Windows App SDK 2.3.1、Win2D 1.4.0、MSTest.Sdk 4.1.0、x64 與 project mapping。
- 新增 `docs/IMPLEMENTATION-READINESS-REVIEW.md`，結論為 `Approved for first vertical slice implementation`。
- 建立第一階段 solution／project skeleton：`SnipPlus.sln`、4 個 source projects、3 個 test projects、中央套件管理與 committed lock files。
- 新增 `SnipPlus.Contracts` 的 workflow、capture、coordinate、image、failure、Clipboard 與 Output semantic contracts。
- 新增 `SnipPlus.Core` 的 `COMP-001` workflow state authority、DIP-to-physical coordinate mapping 與 capture/Clipboard outcome coordination。
- 新增 Core／Contracts tests，覆蓋 legal／illegal transition、cancellation、failure classification、coordinate bounds／rounding、canonical image metadata 與 Clipboard defaults。
- 新增 canonical BGRA8 premultiplied `SoftwareBitmap` image pipeline、PNG encoding、crop 與 Win2D rendering adapter。
- 新增以 `Windows.Graphics.Capture` 為後端的 monitor capture adapter，含 bounded frame wait、content-size validation、cleanup 與 platform category test。
- 新增 WinRT `DataPackage` PNG Clipboard delivery，預設關閉 history／roaming、成功後 `Flush()`，並加入 bounded cancellable contention retry。
- 新增 packaged WinUI 3 application shell、明確 `Start Capture` command、單螢幕 selection surface、DIP／physical-pixel context 建立與結果 presentation。

### Changed

- README、ROADMAP、TODO、Architecture／ADR index、Technology Decision Roadmap、Development Guide、Research index 與 Repository audit 對齊實際狀態。
- Technology Decision Roadmap 的 implementation-critical P0 decisions 全部改為 Accepted。
- ROADMAP Current Phase 改為 `First vertical slice implementation — Ready; not started`。
- TODO 移除前置文件 backlog，改為 solution、source、test 與 evidence 工作。
- 明確凍結第一個 vertical slice 的前置文件；沒有實作發現或 scope change 時不再新增 pre-coding paperwork。
- `global.json` 加入 .NET 10 的 `Microsoft.Testing.Platform` test runner opt-in，讓 `dotnet test` 使用 MTP 而非已不支援的 VSTest target。

### Verified

- `dotnet restore SnipPlus.sln --locked-mode`：成功。
- `dotnet build SnipPlus.sln -c Release -p:Platform=x64 --no-restore`：成功，0 warnings、0 errors。
- `dotnet test SnipPlus.sln -c Release -p:Platform=x64 --no-build -- --filter "TestCategory!=Interactive&TestCategory!=Manual"`：3 個 baseline test assemblies 成功，3 passed、0 failed、0 skipped。
- Core／Contracts contract slice：同一非互動指令成功，16 passed、0 failed、0 skipped。
- Rendering／image slice：6 passed、0 failed、0 skipped；包含 deterministic pixel conversion、crop boundary、PNG encoding、lease cleanup、Win2D rendering 與 cancellation。
- Capture adapter compile verification：`WindowsGraphicsCaptureAdapter` 已通過 Release x64 build；實際 platform capture 仍由 Interactive category 個別驗證。
- Windows capture platform test 已建立為 `Platform`／`Capture`／`Interactive` category，未混入非互動測試。
- Clipboard retry policy tests 已加入一般非互動測試集合；完整非互動測試目前 24 passed、0 failed、0 skipped。
- Application shell 已通過 packaged WinUI 3 Release x64 build；尚未以真實 desktop／Clipboard 內容建立 evidence artifact。

### Not released

- 尚無完整 application shell、Clipboard publication runtime verification、package artifact、deploy 或 release artifact。
- Implementation Readiness approval 只表示文件足以開始 bounded first vertical slice，不表示技術已在 Repository 中實際 restore、build 或 run 成功。
