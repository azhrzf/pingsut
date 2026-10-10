---
name: commit-message
description: Generate git commit messages in a consistent category(scope) plus description format, with an optional bulleted body. Use this skill whenever the user asks for a commit message, pastes a git diff, git status, staged changes, or a description of code changes and wants them committed, or asks to "write/fix/reword/review a commit message", even if they don't say which format to use. Also use it to validate or correct an existing commit message.
---

# Commit message generator

Turns a diff or a description of changes into a commit message with a predictable `category(scope): description` shape.
The consistent shape makes history scannable, and the category tells a reader at a glance whether behavior changed.

## Format

One line:

```
<category>: <description>
```

One line with scope:

```
<category>(<scope>): <description>
```

Multi-line (subject, blank line, body):

```
<category>(<scope>): <brief description>

- <detail>
- <detail>
```

Rules:

- Write the colon directly after the category (or after the closing parenthesis of the scope). No space before the
  colon.
- `<category>` is lowercase and must be one of the categories below.
- `<scope>` is optional. It names the part of the code the commit touches (for example `runner`, `driver`, `webapi`,
  `auth`). Use lowercase. Omit it when the change is cross-cutting or no clear module applies, rather than inventing
  one.
- The subject is a short, lowercase-start, imperative phrase without a trailing period ("add execution endpoint",
  "improve the runner events"). Keep it around 72 characters or fewer so it displays fully in git log and most UIs.
- Add a body only when the subject cannot carry the "what changed" on its own, such as several distinct changes in one
  commit. Use a blank line between subject and body, and `- ` bullets in the body.

## Categories

| Category   | Use when                                                                         |
|------------|----------------------------------------------------------------------------------|
| `feat`     | The change affects the application's behavior (new capability, changed behavior) |
| `fix`      | Correcting undesired behavior (a bug or issue)                                   |
| `refactor` | Restructuring code without changing application behavior                         |
| `test`     | Adding or changing tests only                                                    |
| `doc`      | Documentation changes only (`doc`, not `docs`)                                   |
| `chore`    | Housekeeping: tooling, config, dependency bumps, editor/analyzer settings        |
| `release`  | Merging a release branch back to develop and main                                |

### Choosing between close categories

The deciding question is "would a user or a calling system observe different behavior?"

- Yes, and it corrects something broken: `fix`.
- Yes, and it is new or intentionally different: `feat`.
- No (pure restructure, rename, extraction): `refactor`.
- A bug fix that also adds a test: use `fix`, and mention the test in the body if useful. Use `test` only when the
  commit is solely tests.

## Special cases

- **Merge commits** keep the message git generates; do not rewrite them into this format.
- **Issue or ticket references.** Do not add an issue number to the subject by default. If the user wants one, put it at
  the end of the body (for example `Refs #123`) or follow whatever style they ask for.

## Workflow

1. Get the changes. If you have shell access in a repo, run `git diff --staged` (fall back to `git diff` and
   `git status` if nothing is staged). Otherwise use what the user pasted or described.
2. Work out what changed and why, from the diff itself. Do not guess intent the diff does not show; ask one short
   question if the purpose is unclear.
3. If the diff mixes unrelated concerns (for example a bug fix plus an unrelated refactor), say so and suggest splitting
   into separate commits, then give a message for each. One commit should have one purpose, and the category can only be
   one value.
4. Pick the category, then the scope, then write the subject. Add a body only if warranted.
5. Output the message in a code block, ready to paste. Add one line on why you chose that category if it was not
   obvious. If you made an assumption, say so.

## Examples

Change: new HTTP endpoint to start a protocol execution.

```
feat(webapi): add execution endpoint
```

Change: runner raises a new event when a protocol is assigned, listeners consolidated, endpoint updated.

```
feat(runner): improve the runner events

- introduce new event that is raised when a protocol is assigned
- consolidate event listener
- update the web api assignment endpoint
```

Change: analyzer settings updated so these findings become errors.

```
chore: mark async analyzer findings as errors

- async methods returning void
- async methods not awaited
```

Change: renamed a class and moved files, no behavior change.

```
refactor(driver): extract plate movement into its own class
```

Change: null reference when a device does not respond.

```
fix(driver): handle null response when device is not responding
```

Change: only a new unit test for the not-responding case.

```
test(driver): add case for device not responding
```

## Validating an existing message

When the user pastes a commit message to check, compare it against the rules above and report only real deviations
(wrong or uppercase category, space before colon, missing blank line before the body, scope that is not a code area,
trailing period). Then give the corrected version.