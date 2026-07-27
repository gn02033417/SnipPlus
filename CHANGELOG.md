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
- 為 packaged `win-x64` restore graph 加入共用 `RuntimeIdentifiers`，並以 locked restore 驗證 framework-dependent MSIX publish；未更換核准的 SDK 或 package 版本。
- Package manifest 改用 AppX 接受的 PNG logo assets，補上 `BackgroundColor` 與 `runFullTrust`，以符合 packaged WinUI 3 build validation。
- 修正 `ResultReady → Cancelled` 狀態 transition，並補上 capture service、ResultReady presentation callback 與 Clipboard cancellation cleanup tests。
- Windows platform test 改用 Windows App SDK 2.3 bootstrap 與 `DisplayArea.GetFromPoint` 取得實際 display id；未更換核准 package 版本。
- Clipboard adapter 保留 production WinRT `SetContentWithOptions`／`Flush` default，新增 publisher／flush injection seam 以 deterministic 驗證 bounded retry 與 cancellation。

### Verified

- `dotnet restore SnipPlus.sln --locked-mode`：成功。
- `dotnet build SnipPlus.sln -c Release -p:Platform=x64 --no-restore`：成功，0 warnings、0 errors。
- `dotnet test SnipPlus.sln -c Release -p:Platform=x64 --no-build -- --filter "TestCategory!=Interactive&TestCategory!=Manual"`：3 個 baseline test assemblies 成功，3 passed、0 failed、0 skipped。
- Core／Contracts contract slice：同一非互動指令成功，16 passed、0 failed、0 skipped。
- Rendering／image slice：6 passed、0 failed、0 skipped；包含 deterministic pixel conversion、crop boundary、PNG encoding、lease cleanup、Win2D rendering 與 cancellation。
- Capture adapter compile verification：`WindowsGraphicsCaptureAdapter` 已通過 Release x64 build；實際 platform capture 仍由 Interactive category 個別驗證。
- Windows capture platform test 已建立為 `Platform`／`Capture`／`Interactive` category，未混入非互動測試。
- Clipboard retry policy tests 已加入一般非互動測試集合；完整非互動測試目前 24 passed、0 failed、0 skipped。
- Cancellation hardening tests 已驗證 capture `OperationCanceledException`、ResultReady callback cancellation、Clipboard cancellation 與 `ResultReady → Cancelled` legal transition。
- Clipboard adapter filter 已驗證 busy retry success、retry budget boundary 與 retry cancellation：3 passed、0 failed、0 skipped；成功路徑只呼叫一次 `Flush()`，History／roaming 仍為 false。
- Packaged runtime cancellation verification 已完成；`Start Capture → Cancel` 回到主畫面並回報 `Capture cancelled.`。
- 完整非互動測試更新為 31 passed、0 failed、0 skipped。
- Windows platform test 已可用 `Platform`／`Capture`／`Interactive` filter 單獨執行；support check 與 in-memory monitor frame check 均 passed，0 failed、0 skipped。
- Packaged runtime verification 已使用 public synthetic blank Paint fixture 完成 selection、monitor frame、crop、render、PNG encode、Clipboard publication 與成功狀態回報；未保存 desktop screenshot、Clipboard payload 或任何私人資料。
- Application shell 已通過 packaged WinUI 3 Release x64 build；publish 僅有 `mspdbcmf.exe` 缺少造成的 symbol generation warning，未影響 package build 或 runtime verification。

### Not released

- 未進行 release publication、store deployment 或 release artifact 提交；本次 package 只用於本機 runtime verification。
- Implementation Readiness approval 是開始 bounded first vertical slice 的文件門檻；本次實作與 runtime 結果另以本檔案 Verified 記錄。
