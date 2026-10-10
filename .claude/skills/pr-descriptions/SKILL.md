---
name: pr-description
description: Writes pull request descriptions. Use when create a PR, or when the user asks to summarize changes for a pull request.
---

When writing a PR description:

1. Run `git diff develop...HEAD` to see all changes on this branch
2. Write a description following this format:

## What
One sentence explaining what this PR does.

## WHy
Brief context on why this change is needed

## Changes
- Bullet points of spesific changes made
- Group telated changes together
- Mention any files deleted or renamed

## Testing
How to verify this works. Include spesific commands if relevant.

Keep descriptions concise. Focus on what a reviewer need to know.