# SPEC-0008 Capture Output

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `SPEC-0008` |
| Feature ID | `FEAT-004` |
| Version | `1.0` |
| Status | `Accepted` |
| Last reviewed | `2026-07-27` |
| Normative sources | `PRD-0004`、`PRD-0005`、`PRD-0006`、`SPEC-0007`、`SPEC-0009` |

## 2. First-release Output Modes

The first release has two explicit output actions:

| Action | File | Clipboard | End session |
| --- | --- | --- | --- |
| Complete | No | Yes | Only after Clipboard success |
| Save | PNG | Yes | Only after PNG and Clipboard success |

Cancel creates neither output.

## 3. Final Render Input

Both Complete and Save use one final render built from:

- the current locked selection;
- source content from the current Frozen Virtual Desktop session;
- all annotation objects visible inside the current selection;
- clipping at the exact selection bounds;
- the current annotation styles and effects.

The final render must not contain:

- masked overlay UI;
- selection border or resize handles;
- function bar;
- SnipPlus normal windows;
- annotation portions outside the selection;
- content captured after the session’s frozen frames.

## 4. Save As Behavior

- Save opens the Windows Save As experience every time.
- First-release output format is PNG only.
- Proposed default filename is `SnipPlus_yyyy-MM-dd_HHmmss.png`.
- The user may change the destination and filename through Save As.
- Cancelling Save As returns to Editing with the selection and annotations unchanged.
- Save As cancellation is not capture cancellation.

## 5. Save Commit Sequence

```text
User chooses Save
→ Freeze the current editing revision for output
→ Render final image
→ Show Save As
→ If cancelled, return to Editing
→ Write PNG
→ Publish the same rendered image to Clipboard
→ If both succeed, clean up and restore focus
```

A Save workflow is not complete unless both PNG creation and Clipboard publication succeed.

## 6. Failure Behavior

### Render failure

- No file is created.
- Clipboard is not updated.
- Editing remains open with current state preserved.

### Save failure

- Clipboard is not updated.
- Editing remains open.
- The user receives an actionable error and may retry or cancel.

### Clipboard failure after PNG creation

- The workflow remains in Editing and reports Clipboard failure.
- It must not silently claim complete success.
- The created-file rollback／retention policy remains unresolved and requires a separate product decision before implementation chooses behavior.

## 7. Output Identity and Idempotency

- Each render request carries Session ID、Result ID and editing revision identity.
- Retrying must not accidentally save or publish an older editing revision.
- Platform output adapters report outcomes and never mutate shared workflow state.
- Repeated callbacks from a stale request cannot complete a newer session.
- Cancel must prevent later completion of an in-flight output request where cancellation is supported.

## 8. Privacy and Evidence

- PNG Save is local user-directed output.
- No cloud upload、sync or external transfer is part of v1.
- Real screenshot files and Clipboard payloads must not be committed as test evidence.
- Deterministic synthetic images are preferred for automated output tests.

## 9. Acceptance Criteria

| ID | Criterion |
| --- | --- |
| `SPEC-0008-AC-001` | Complete creates no file and writes Clipboard only after explicit commitment. |
| `SPEC-0008-AC-002` | Save proposes PNG with the required timestamp filename pattern. |
| `SPEC-0008-AC-003` | Save As cancellation returns to Editing without output or state loss. |
| `SPEC-0008-AC-004` | PNG and Clipboard receive the same final rendered image. |
| `SPEC-0008-AC-005` | Save failure prevents Clipboard update and preserves Editing. |
| `SPEC-0008-AC-006` | Clipboard failure prevents session completion and preserves Editing. |
| `SPEC-0008-AC-007` | Overlay、handles and function-bar visuals never appear in final output. |
| `SPEC-0008-AC-008` | Stale output callbacks cannot complete a different session or revision. |

Additional formats、automatic destinations and background saving are deferred.
