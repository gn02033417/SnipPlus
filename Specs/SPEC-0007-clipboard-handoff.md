# SPEC-0007 Clipboard Handoff

狀態：`Accepted`

## 1. Document Control

| Field | Value |
| --- | --- |
| Document ID | `SPEC-0007` |
| Feature ID | `FEAT-003` |
| Version | `1.0` |
| Status | `Accepted` |
| Last reviewed | `2026-07-27` |
| Normative sources | `PRD-0004`、`PRD-0005`、`PRD-0006`、`SPEC-0003`、`SPEC-0009` |

## 2. Clipboard Commitment Boundary

Clipboard is written only after an explicit user commitment:

- Complete; or
- successful Save path after PNG creation has completed.

The following never write Clipboard:

- PrintScreen capture start;
- source-frame freeze;
- initial drag;
- mouse release;
- selection move、resize or reselection;
- annotation edits;
- Save As cancellation;
- Cancel.

## 3. Final Image Contract

Clipboard receives one final rendered image representing:

- the current selection bounds;
- source pixels from the current frozen capture session;
- all visible annotation content clipped to the current selection;
- the same rendered result used by PNG Save when Save is chosen.

Clipboard must not receive:

- the full Virtual Desktop when only a region was selected;
- uncommitted preview state;
- annotation content outside selection bounds;
- a frame from a later capture time;
- a stale result from a previous session.

## 4. Complete Path

```text
User chooses Complete
→ Lock current editing state for commit
→ Render final image
→ Publish final image to Clipboard
→ If successful, clean up capture UI and restore focus
→ End session silently
```

Complete does not create a file.

If rendering or Clipboard publication fails:

- do not report completion;
- retain the current selection and annotations;
- return or remain in Editing;
- show an actionable error;
- allow retry or Cancel.

## 5. Save Path Relationship

Save owns file creation; Clipboard Handoff owns Clipboard publication.

```text
User chooses Save
→ Save As succeeds
→ PNG creation succeeds
→ Publish the exact same rendered image to Clipboard
→ Only after both succeed may the session complete
```

Cancelling Save As does not invoke Clipboard Handoff.

If PNG creation fails, Clipboard is not updated.

If Clipboard publication fails after file creation:

- do not report the complete Save workflow as successful;
- retain the editing state;
- disclose the Clipboard failure;
- the exact rollback policy for the already-created PNG remains an explicit unresolved product decision and must not be guessed.

## 6. Retry and Privacy

- Clipboard busy handling may use bounded retry according to accepted Architecture contracts.
- Retry must be cancellable.
- Clipboard History and roaming remain disabled by default.
- Successful Clipboard publication must preserve payload lifetime as required by the accepted Clipboard ADR.
- Real Clipboard payloads must not be persisted as repository evidence.

## 7. Session and State Rules

- Clipboard requests include the current Session ID and final Result ID.
- Only `COMP-001` may transition shared workflow state.
- Platform Clipboard adapters return an outcome; they do not declare the product session complete.
- A success outcome applies only to the exact request and result supplied.
- Cancellation or a new capture session invalidates stale Clipboard completion callbacks.

## 8. Acceptance Criteria

| ID | Criterion |
| --- | --- |
| `SPEC-0007-AC-001` | Mouse release and annotation edits never write Clipboard. |
| `SPEC-0007-AC-002` | Complete writes one final selected-and-annotated image and creates no file. |
| `SPEC-0007-AC-003` | Save publishes the same rendered image to PNG and Clipboard. |
| `SPEC-0007-AC-004` | Save As cancellation returns to Editing without Clipboard update. |
| `SPEC-0007-AC-005` | Clipboard failure retains selection and annotations and does not close the workflow. |
| `SPEC-0007-AC-006` | Stale or mismatched session callbacks cannot complete a newer session. |
| `SPEC-0007-AC-007` | History／roaming remain disabled and retry remains bounded and cancellable. |

The previous automatic Clipboard handoff immediately after capture completion is superseded.
