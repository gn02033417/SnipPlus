# ADR-0007 Testing Strategy

## Document Control

| Field | Value |
| --- | --- |
| Document ID | ADR-0007 |
| Title | Testing Strategy |
| Status | Accepted |
| Decision Category | Testing |
| Version | 1.0 |
| Owner | Repository owner |
| Date proposed | 2026-07-26 |
| Date reviewed | 2026-07-26 |
| Date accepted | 2026-07-26 |
| Supersedes | None |
| Superseded by | None |
| Normative References | ADR-0002、ADR-0003、ADR-0004、ADR-0005、ADR-0006、PRD-0006、SPEC-0003、SPEC-0005、SPEC-0006、SPEC-0007、SPEC-0008、SPEC-0009、SPEC-0010、ARCH-0001、ARCH-0002、ARCH-0003、ARCH-0004、ARCH-0005、ADR-BASELINE |
| Informative References | Official MSTest.Sdk、Microsoft.Testing.Platform and .NET testing documentation |

## Context

SnipPlus requires tests across pure workflow logic、data contracts、deterministic rendering and Windows platform integrations. A single undifferentiated test suite would either force Windows APIs into unit tests or leave platform behavior unverified.

The repository also needs one test platform and one invocation model. Microsoft documentation explicitly warns against mixing VSTest-based and Microsoft.Testing.Platform-based projects in one solution execution configuration.

## Options Considered

### MSTest.Sdk with Microsoft.Testing.Platform

Microsoft-supported test framework and project SDK, aligned with .NET and Visual Studio. MTP is enabled by default and supports CLI/CI execution.

### xUnit.net

Mature and widely used, but provides no product-specific advantage over the Microsoft-native stack for this Windows-first repository.

### NUnit

Mature and capable, but similarly adds a separate framework choice without a requirement that MSTest cannot satisfy.

### Mixed frameworks or mixed VSTest/MTP

Rejected because it increases configuration、filtering and CI inconsistency before the project has a verified need.

## Accepted Decision

1. Use **MSTest.Sdk 4.1.0** for all .NET test projects.
2. Use **Microsoft.Testing.Platform (MTP)** as the only .NET test platform.
3. Do not set `UseVSTest=true` and do not mix VSTest-based test projects into the solution execution path.
4. Pin `MSTest.Sdk` in solution-level `global.json` under `msbuild-sdks`.
5. Use the `Default` MSTest SDK extension profile initially for TRX and code-coverage support.
6. Tests use deterministic synthetic fixtures. No test may capture or persist private desktop、Clipboard or user-file content.

## Test Layers

### Domain unit tests

Cover：

- Workflow state transitions.
- Selection geometry and coordinate conversion rules.
- Failure classification and retry decisions.
- Result lifetime and ownership rules.
- Annotation model and render-intent generation.

Requirements：fast、parallel-safe、no UI thread、no Windows capture or Clipboard API.

### Contract tests

Cover：

- CaptureIntent and ImageResult invariants.
- DIP／physical-pixel conversions and rounding.
- BGRA8 premultiplied-alpha semantics.
- Disposal and invalid-use behavior.
- Clipboard and Output delivery result independence.
- Recoverable／terminal failure mapping.

### Rendering tests

Cover synthetic scenes through the rendering adapter：

- Geometry、text、selection handles and clipping.
- Premultiplied-alpha edges.
- Mosaic/pixelation regions.
- Display render versus canonical raster comparison.
- DPI-specific output dimensions.
- Resource recreation behavior.

Pixel tests use repository-owned synthetic inputs and expected outputs. A changed golden file requires explicit review and may not be updated automatically merely to make a test pass.

### Windows platform integration tests

Run only on an interactive Windows x64 environment with explicit authorization：

- WinUI host startup.
- WGC support/source-item creation.
- One-shot synthetic/public scene capture.
- Overlay exclusion and cursor policy.
- Clipboard publication、flush、contention and representative consumer checks.
- Cleanup、source closure and device/session failure paths.

Tests must be category-filterable and skipped with an explicit reason when the environment lacks required interactive capabilities. A skip is not a pass.

### Manual smoke tests

Allowed only for behaviors not reliably automatable, such as user-visible focus transitions or selected external consumer compatibility. Each manual test must have fixed steps、expected result、environment and evidence record.

## CI Policy

Pull request / normal CI：

- Restore with locked dependencies.
- Build in Release x64.
- Run Domain and Contract tests.
- Run deterministic Rendering tests that do not require an interactive desktop.
- Emit TRX and coverage artifacts.

Windows interactive verification pipeline or explicitly authorized local run：

- Run Windows platform integration category.
- Use synthetic/public fixtures only.
- Store metadata、logs and approved synthetic image diffs only.
- Clean temporary image and Clipboard resources.

No CI job may access the real user desktop or Clipboard by default.

## Quality Gates

- No failed required test.
- No unexpected skipped Domain、Contract or deterministic Rendering test.
- New behavior requires tests at the owning layer.
- State transition、failure and cleanup branches are mandatory even without a repository-wide percentage target.
- Code coverage is diagnostic initially; no arbitrary percentage gate is introduced before a stable implementation baseline.
- Platform verification failures block declaring the related capability verified but do not silently rewrite an ADR.

## Test Naming and Categories

Categories：

- `Unit`
- `Contract`
- `Rendering`
- `Platform`
- `Clipboard`
- `Capture`
- `Interactive`
- `Manual`

Test names describe condition and expected outcome. Time、randomness and filesystem roots must be injectable or controlled.

## Privacy and Evidence Boundary

- Synthetic images only unless the user explicitly authorizes a different fixture.
- No real desktop screenshots in the repository.
- No raw Clipboard payload retention.
- Remove account names、paths、window titles and machine identifiers from logs.
- Test artifacts have a documented root and cleanup rule.
- A failure artifact may contain only approved synthetic content and required metadata.

## Trade-offs

### Benefits

- One Microsoft-supported framework/platform.
- Simplified project and CI configuration.
- Clear separation between pure logic and interactive Windows verification.
- Test evidence directly maps to Accepted ADR risks.

### Costs

- Interactive capture and Clipboard tests require a Windows session and cannot run in every CI environment.
- Pixel tests require careful golden-file governance.
- MTP tooling differs from legacy VSTest options and requires consistent CLI configuration.

### Neutral consequences

- The decision does not require a coverage percentage.
- The decision does not select a CI provider.
- Manual tests remain possible but cannot replace automatable contract tests.

## External Evidence

| Source | Evidence used |
| --- | --- |
| [Get started with MSTest](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-mstest-getting-started) | Microsoft recommends MSTest.Sdk and documents `MSTest.Sdk/4.1.0` with `net10.0`. |
| [MSTest SDK configuration](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-mstest-sdk) | MSTest.Sdk enables the MSTest runner/MTP by default and documents extension profiles. |
| [Run tests with MSTest](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-mstest-running-tests) | MTP runner is embedded in the test project and remains callable through `dotnet test`. |
| [Testing platform overview](https://learn.microsoft.com/en-us/dotnet/core/testing/microsoft-testing-platform-vs-vstest) | A solution should consistently use one platform and not mix VSTest and MTP execution. |

## Review Record

| Field | Value |
| --- | --- |
| Reviewer | ChatGPT repository review |
| Review date | 2026-07-26 |
| Review result | Approved |
| Open comments | Interactive pipeline provider remains a Project/CI implementation detail |
| Resolution | Selected one platform and defined required test layers/privacy boundaries |
| Acceptance authority | Repository owner through explicit instruction to continue toward coding readiness |

## Implementation State

| Artifact | Status |
| --- | --- |
| Test projects | Not created |
| Test execution | Not performed |
| Coding authorized | No |

## Non-goals

This ADR does not create tests、select GitHub Actions versus another CI provider、run a desktop session or authorize application code.
