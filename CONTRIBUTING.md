# Contributing to SnipPlus

狀態：`Accepted`

## 先理解現行來源

開始變更前，依序讀取：

1. [README](README.md)
2. [AGENTS.md](AGENTS.md)
3. [Requirements-to-Code Conformance Matrix](PRD/PRD-TRACEABILITY-MATRIX.md)
4. 與變更直接相關的 PRD、Spec、Architecture／Contract
5. 最小必要的 current code and tests

Historical Research、Analysis、Decision and prior baseline reviews are background only and do not override accepted v1 sources.

## 變更流程

1. 說明變更目的、對應 requirement／acceptance criterion and current conformance status.
2. 判斷現有程式是 reusable、partial、incorrect or obsolete.
3. 做最小完整 behavioral slice，不跳過 earlier missing prerequisites.
4. 同步新增或修改 relevant Unit／Contract／platform tests.
5. Only run restore、build、test or runtime commands explicitly authorized by the current task.
6. Update `CHANGELOG.md` with actual results.
7. Update the existing conformance-matrix rows only after code、tests and applicable evidence exist.
8. Stop before the next correction step、deferred capability or unresolved product decision.

## 文件要求

- 使用台灣繁體中文描述產品與流程；保留 API、path、code 與正式技術名稱。
- 每份文件使用唯一 H1 and an explicit status.
- 使用 [Markdown naming rules](docs/standards/markdown-naming.md).
- Product behavior belongs in existing PRD／Specs; do not hide product decisions in code or Architecture prose.
- New ADRs require a durable technology or responsibility decision, not ordinary missing implementation.
- Do not create repeated readiness、authorization、reassessment or closure documents.

## Implementation rules

- `COMP-001` remains the sole shared Workflow State Authority.
- Platform adapters return typed outcomes and do not declare product completion.
- Mouse release locks Selection and never commits output.
- Editing／confirmation is mandatory; annotation actions may be skipped.
- Complete and Save are explicit commitments.
- Save coordinates separate PNG and Clipboard capabilities and succeeds only after both obligations succeed.
- Real desktop screenshots and Clipboard payloads are not committed as evidence.
- Normal development and non-interactive tests do not launch external GUI fixtures.

## Current implementation boundary

The current source is a reusable single-display capture／crop／Clipboard technical prototype. The next work follows `PRD-TRACEABILITY-MATRIX-001`, starting with resident lifecycle and user-controlled PrintScreen takeover.

Do not begin with Annotation、Clipboard hardening、Packaging or unrelated scope expansion.

## Commit and Pull Request

Commit messages should be short、imperative and limited to one purpose, for example:

```text
core: add resident capture lifecycle
```

A Pull Request should state:

- requirement／Spec source;
- current and target conformance status;
- source and tests changed;
- commands actually run;
- runtime behavior verified or not verified;
- privacy／interactive-verification considerations;
- remaining gaps and stop conditions.
