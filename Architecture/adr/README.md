# Architecture Decision Records

ADR 用來記錄影響多個模組、難以回復，或需要長期保存取捨理由的架構與技術選擇。

## ADR 清單

| ADR | Topic | Status | Effective decision |
| --- | --- | --- | --- |
| [ADR-0001 Documentation-first baseline](ADR-0001-documentation-first.md) | Repository documentation-first governance | Accepted | Yes |
| [ADR-0002 UI Framework Selection](ADR-0002-ui-framework-selection.md) | Desktop UI Framework | Accepted | WinUI 3 |

只有 `Accepted` ADR 可以作為下游 Contract、Project Structure、Implementation planning 與 Verification 的有效技術來源。

## Decision backlog

後續重大技術主題依 [Technology Decision Roadmap](../TECHNOLOGY-DECISION-ROADMAP.md) 管理。

目前核心順序：

1. ~~UI Framework~~ — Accepted through ADR-0002。
2. Rendering Technology。
3. Capture Backend。
4. Clipboard Integration。
5. Image Representation。
6. Testing Strategy。

每個主題應直接收斂為一份主要 ADR。既有 Research 作為 evidence，不先建立新的 prerequisite、authorization-request 或 closure-review 文件。

## 建立規則

每份 ADR 至少包含：

- Status
- Context
- Decision Drivers
- Options Considered
- Decision
- Trade-offs
- Consequences
- Traceability
- Review Record
- Change and Supersession

規則：

- 一份 ADR 只處理一個重大決策。
- ADR 必須引用 Architecture requirement、finding 或 Decision Roadmap item。
- Draft ADR 必須完成 Review，並由明確 authority 接受後才能成為 `Accepted`。
- Accepted ADR 的核心 decision 不直接覆寫；需要改變時建立新 ADR 並標示 Supersedes。
- ADR 不得反向修改 Frozen PRD、Frozen Specs 或 Frozen Architecture ownership。
- Implementation detail、暫時除錯筆記與一般 TODO 不使用 ADR。
- 編號只遞增，不重用。
