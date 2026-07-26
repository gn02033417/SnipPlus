# SnipPlus 文件入口

## 開始實作前必讀

1. [Implementation Readiness Review](IMPLEMENTATION-READINESS-REVIEW.md) — 已核准第一個 vertical slice。
2. [Implementation Contracts](../Architecture/IMPLEMENTATION-CONTRACTS.md)
3. [Project Structure and Toolchain Baseline](../Architecture/PROJECT-STRUCTURE.md)
4. [ADR index](../Architecture/adr/README.md)
5. [Frozen PRD baseline](../PRD/PRD-FREEZE-REVIEW.md)
6. [Frozen Specification baseline](../Specs/SPEC-BASELINE-REVIEW.md)
7. [Frozen Architecture baseline](../Architecture/ARCH-BASELINE-REVIEW.md)
8. [Development Guide](guides/development-guide.md)
9. [Repository rules](../AGENTS.md)

## Current lifecycle state

| Area | Current state |
| --- | --- |
| PRD／Specs／Architecture | Freeze Approved |
| ADR-0002 through ADR-0007 | Accepted |
| Contracts | Accepted |
| Project Structure / Toolchain | Accepted |
| Implementation readiness | **Approved for first vertical slice** |
| Application code | Not started |
| Build/runtime evidence | Not performed; implementation output |

## Effective decisions

- [ADR-0002 UI Framework](../Architecture/adr/ADR-0002-ui-framework-selection.md) — WinUI 3。
- [ADR-0003 Rendering](../Architecture/adr/ADR-0003-rendering-technology.md) — XAML／Composition + Win2D。
- [ADR-0004 Capture](../Architecture/adr/ADR-0004-capture-backend.md) — Windows.Graphics.Capture。
- [ADR-0005 Image Representation](../Architecture/adr/ADR-0005-image-representation.md) — BGRA8 premultiplied SoftwareBitmap。
- [ADR-0006 Clipboard](../Architecture/adr/ADR-0006-clipboard-integration.md) — WinRT DataPackage。
- [ADR-0007 Testing](../Architecture/adr/ADR-0007-testing-strategy.md) — MSTest.Sdk + MTP。

## Documentation areas

### Product and behavior

- [PRD index](../PRD/README.md)
- [Specs index](../Specs/README.md)

### Architecture and implementation

- [Architecture index](../Architecture/README.md)
- [Technology Decision Roadmap](../Architecture/TECHNOLOGY-DECISION-ROADMAP.md)
- [Implementation Contracts](../Architecture/IMPLEMENTATION-CONTRACTS.md)
- [Project Structure](../Architecture/PROJECT-STRUCTURE.md)
- [ADR index](../Architecture/adr/README.md)

### Research history

- [Research framework](Research/README.md)
- [Technology Research index](Research/Technology/README.md)
- [Analysis framework](Analysis/README.md)
- [Decision framework](Decision/README.md)

Technology Research remains evidence/history. It does not override Accepted ADRs and must not be extended through repetitive readiness/closure documents.

### Guides and management

- [Development Guide](guides/development-guide.md)
- [Coding Standard](guides/coding-standard.md)
- [ROADMAP](../ROADMAP.md)
- [TODO](../TODO.md)
- [CHANGELOG](../CHANGELOG.md)
- [Repository current-state audit](REPOSITORY-CURRENT-STATE-AND-IMPLEMENTATION-READINESS-AUDIT.md)

## Current next action

The next change should be an explicit implementation task creating solution/project files、source code、tests and evidence. No additional pre-coding documentation is required.
