# Markdown 命名規則

狀態：`Accepted`

本規則統一 SnipPlus 的 Markdown 檔名、目錄名稱與文件內標題，避免文件數量增加後難以搜尋、排序與維護。

## 目錄命名

- 一般目錄使用小寫 `kebab-case`，例如 `docs/guides/`、`docs/standards/`。
- 領域根目錄保留既有大寫名稱：`Architecture/`、`PRD/`、`Specs/`。
- 不使用空白、底線、日期作為一般目錄名稱。
- 目錄名稱應描述內容，不使用 `misc/`、`new/`、`temp/`。

## 檔案命名

### 根目錄文件

固定入口與協作文件使用全大寫慣例：

- `README.md`
- `AGENTS.md`
- `CONTRIBUTING.md`
- `ROADMAP.md`
- `CHANGELOG.md`
- `TODO.md`

### PRD

格式：`PRD-NNNN-kebab-case.md`

例：`PRD-0001-product-foundation.md`

### Specs

格式：`SPEC-NNNN-kebab-case.md`

例：`SPEC-0001-documentation-baseline.md`

### ADR

格式：`ADR-NNNN-kebab-case.md`

例：`ADR-0001-documentation-first.md`

ADR 編號一旦發布不得重用。被取代的 ADR 保留原檔，更新狀態與替代文件連結。

### `docs/` 一般文件

- 使用小寫 `kebab-case`，例如 `development-guide.md`。
- 不在檔名重複父目錄已表達的分類，例如 `docs/guides/guide-development.md` 不採用。
- `index.md` 只用於目錄入口，不用於一般主題文件。

## 文件標題

- 每個 Markdown 檔案只能有一個 H1，且放在第一個內容標題位置。
- H1 應與文件用途一致，不把狀態或日期塞進檔名。
- H2 用於主要章節，H3 用於章節內分組；避免跳級。
- 文件開頭若內容尚未核准，加入 `狀態：`。

## 連結與資產

- 優先使用相對連結，例如 `[架構總覽](../../Architecture/README.md)`。
- 連結文字描述目標，不使用「點這裡」。
- 圖片與其他資產使用小寫 `kebab-case`，放在所屬文件的 `assets/` 目錄；目前不建立空的資產目錄。
- Mermaid 圖直接放在 `.md`，除非圖需要在多個文件重用。

## 變更規則

- 新增 PRD、Spec 或 ADR 前，先更新對應的 `README.md` index。
- 只改文件內容、不改語意時，不重新編號。
- 文件改名應在同一個變更中更新所有已知連結。
- 不用檔名表達 `Draft`、`Final`、`Latest`；狀態放在文件內容與 index。
