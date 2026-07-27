# Coding Standard

狀態：`Accepted`

## 1. Scope

This standard applies to Repository-owned C# 14／.NET 10 source and tests. It supplements `.editorconfig`、`Directory.Build.props`、Accepted Architecture、Implementation Contracts and Project Structure.

Generated WinUI code and third-party source are governed by their toolchain, but Repository-owned integration code must still preserve the accepted boundaries.

## 2. Core principles

- Prefer correctness、readability、testability and maintainability over cleverness.
- Keep product behavior in Core rather than WinUI code-behind or platform adapters.
- Fix the root workflow／state／ownership defect rather than layering a visual workaround over it.
- Reuse current technical foundations only after conformance review.
- Do not add abstractions、projects or dependencies without a demonstrated responsibility need.
- Do not implement deferred product scope without explicit direction.

## 3. Language and type rules

- Use nullable reference types.
- Prefer immutable records or readonly value types for intents、snapshots、revisions and outcomes.
- Use required members when absence would violate a contract.
- Validate public contract inputs at the owning boundary.
- Use `CancellationToken` for cancellable asynchronous work.
- Implement `IDisposable` or explicit lease ownership for image、frame、stream and platform resources.
- Cleanup must be idempotent where the workflow can invoke it through more than one path.
- Avoid `async void` except UI event handlers; event handlers should delegate to testable asynchronous methods.
- Do not block asynchronous platform calls with `.Result`、`.Wait()` or synchronous polling.

## 4. Naming

- Use accepted domain terms: `CaptureSession`、`FrozenVirtualDesktop`、`DisplaySnapshot`、`SelectionRevision`、`AnnotationRevision`、`FinalRender`、`ClipboardDelivery` and `OutputDelivery`.
- Boolean names read as conditions, such as `IsTakeoverEnabled` or `CleanupCompleted`.
- Method names describe action and result boundary.
- Avoid vague containers such as `Manager`、`Helper` or `Util` unless the responsibility is genuinely narrow and explicit.
- Use the same concept name across PRD、Specs、contracts、source and tests.

## 5. Project and dependency rules

- `SnipPlus.Contracts` depends on no source project.
- `SnipPlus.Core` depends only on Contracts.
- `SnipPlus.Windows` depends on Contracts and accepted platform packages, not Core.
- `SnipPlus.App` is the composition root and may reference Contracts、Core and Windows.
- No circular references.
- Concrete WinUI、WGC、Win2D、DataPackage、picker、window-handle or filesystem implementation types do not leak into Core contracts.
- `COMP-001` remains the sole shared Workflow State Authority.
- Platform adapters return typed outcomes and never mutate shared state.

## 6. Workflow rules

Code must preserve these invariants:

- one Session ID owns all display frames、selection、annotations and output requests;
- mouse release locks Selection and never publishes Clipboard or a file;
- Editing／confirmation is mandatory while Annotation actions are optional;
- Selection changes do not scale or move Annotation geometry;
- Complete creates no file and ends only after Clipboard success;
- Save coordinates PNG and Clipboard and ends only after both succeed;
- recoverable output failure preserves Editing state;
- stale session／revision outcomes cannot advance state;
- Cancel creates no output and invalidates pending completion;
- successful、cancelled and terminal sessions perform capture-UI cleanup and focus restoration.

## 7. Error handling

- Do not swallow exceptions or leave empty `catch` blocks.
- Expected platform and workflow failures use typed outcomes and stable failure codes.
- Error messages identify the failed operation without exposing screen pixels、Clipboard content or private window titles.
- Retry must be bounded、cancellable and owned by the correct capability.
- A partial success must not be reported as full success.
- Terminal resource loss and recoverable output contention must remain distinct.

## 8. Resource ownership

- Every acquired display frame has one session owner and is disposed exactly once.
- Final image results remain alive for the complete delivery lifetime.
- Leases must not outlive the owning result contract.
- Cancellation and exception paths release capture sessions、frame pools、streams、bitmaps、pointer capture and transient windows.
- Cleanup methods tolerate repeated calls without double-disposal failure.

## 9. Testing

- Test names describe scenario and expected outcome.
- Unit tests cover platform-neutral state、geometry、history、commitment and failure rules.
- Contract tests cover identity、defaults、immutability、disposal and invalid input.
- Windows tests cover image、PNG、Clipboard and platform adapters.
- Interactive PrintScreen、multi-display、window and focus tests are categorized and excluded from default test runs.
- Use synthetic or public images; do not commit real desktop screenshots or Clipboard payloads.
- Bug fixes should add a deterministic failing test when the defect is testable below the interactive UI layer.
- Passing build or test counts do not by themselves prove product conformance.

## 10. UI and accessibility

- WinUI code-behind translates UI events into intents; it does not own workflow semantics.
- Required controls expose understandable accessible names and current state.
- Color is not the only indicator of tool selection、selection boundary or failure.
- Function-bar placement and visibility follow accepted Specs.
- Exact styling may evolve without changing product behavior.

## 11. Privacy and external processes

- Screen、Clipboard and saved image data are sensitive.
- Do not log pixel data、full window titles or unredacted local paths.
- Normal product operation、restore、build、unit tests and static checks do not launch Paint、Notepad or other external GUI fixtures.
- Interactive external windows require explicit authorization in the current task.

## 12. Code review checklist

Review at least:

- requirement and acceptance source;
- correct project／component ownership;
- legal state and revision behavior;
- cancellation、failure and cleanup paths;
- tests for changed behavior;
- privacy and interactive-verification boundary;
- absence of deferred scope or unnecessary dependency;
- `CHANGELOG.md` and conformance-matrix updates when evidence exists.
