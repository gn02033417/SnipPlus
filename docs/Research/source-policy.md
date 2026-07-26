# Research Source Policy

狀態：`Accepted`

## Preferred sources

優先使用能直接描述版本與行為的來源：

1. 官方產品文件與說明頁。
2. 官方 release notes、change log 或 support article。
3. 官方影片、示範或產品公告。
4. 可記錄環境與步驟的實際驗證。

## Secondary sources

以下來源只能作為線索或補充，不能默認為事實：

- 官方 GitHub repository、issue 或 discussion。
- 經驗豐富的技術社群文章。
- Reddit 或其他論壇；必須明確標示為社群觀察。

## Disallowed evidence

- AI 自行推測或沒有來源的回答。
- 未標版本與日期的截圖或影片。
- 無法重現、無法指出頁面或上下文的轉述。
- 把舊版本行為當成最新版行為。

## Required source record

每項可影響 PRD 的研究至少記錄：

| Field | Required content |
| --- | --- |
| Source | 標題、URL 或可定位識別資訊 |
| Source type | Official docs、release note、video、verification、community 等 |
| Product version | 產品版本、OS 版本或 `UNKNOWN` |
| Published date | 來源發布日期或 `UNKNOWN` |
| Accessed date | 實際查閱日期 |
| Verification | 如何確認觀察 |
| Confidence | `High`、`Medium`、`Low` 或 `UNKNOWN` |
| Limitations | 來源與驗證的限制 |

## Version policy

- 研究標題或開頭必須寫明研究對象與版本範圍。
- 最新版本若無法確認，標示 `UNKNOWN`，不寫「最新版」當作已知事實。
- 外部版本更新時，將舊文件標為 `Stale` 或建立新研究文件；不覆蓋歷史觀察。
