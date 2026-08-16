---
name: release-roslynator
description: Use when shipping a roslynator release, rolling CHANGELOG.md [Unreleased], updating the VS Code extension changelog, creating a GitHub v* release, or optionally tagging cli-v*.
---

# Release Roslynator

## Overview

Four hard-gated steps: changelog PR → merge → GitHub release (`v*`) → optional CLI tag (`cli-v*`). Versions come from the user. GitVersion derives package versions from tags — do not hardcode in props.

**Violating the letter of the gates is violating the spirit of the gates.** Do not batch steps or confirm once for the whole release.

## Prerequisites

- **Analyzer/extension version** (e.g. `4.17.0`) — required. If missing: **STOP. Do NOT proceed.** Ask the user.
- **CLI version** (e.g. `0.15.0` / `cli-v0.15.0`) — only needed if Step 4 is confirmed.
- **Today’s date** for changelog headers (`YYYY-MM-DD`).

**Not for:** day-to-day contributor work.

## Hard gates

Before each step below: present what you will do, then use interactive confirmation (`AskQuestion` if available; otherwise ask and wait). Do **not** use markdown checkboxes.

**STOP. Do NOT proceed** to the next step until the user explicitly confirms the current step. Providing a CLI version does **not** skip Step 4 confirmation.

## Quick Reference

| Step | Gate | Action |
|------|------|--------|
| 1 | Confirm before push/PR | Changelog PR (root `CHANGELOG.md` + VS Code `package/CHANGELOG.md`) |
| 2 | Confirm before merge | Squash-merge PR; pull `main` |
| 3 | Confirm before create | GitHub release → creates `vX.Y.Z`; title same as tag (`vX.Y.Z`) |
| 4 | Opt-in only | Optional `cli-v*` tag on a real commit; push |

CI / GitVersion details: [references/release-and-ci.md](references/release-and-ci.md).

## Step 1 — Changelog PR

**STOP until user confirms Step 1.**

1. Branch from latest `main` (e.g. `release/X.Y.Z`).
2. Root [`CHANGELOG.md`](../../../CHANGELOG.md): under `## [Unreleased]`, insert `## [X.Y.Z] - YYYY-MM-DD` so prior Unreleased bullets become that version’s section (same pattern as bump PR #1785). Leave `[Unreleased]` empty above it.
3. [`src/VisualStudioCode/package/CHANGELOG.md`](../../../src/VisualStudioCode/package/CHANGELOG.md): copy that new version section (header + body) under `[Unreleased]`.
4. Commit, push, open PR — title `Bump version to X.Y.Z`.

Do **not** run `generate_all.ps1` as part of this PR unless the user asks.

**STOP.** Wait for Step 2 confirmation.

## Step 2 — Merge

**STOP until user confirms Step 2.**

Squash-merge the bump PR (after CI is acceptable to the user):

```bash
gh pr merge <PR_NUMBER> --squash
```

Pull latest `main`. Note the squash commit SHA on `main`.

**STOP.** Wait for Step 3 confirmation.

## Step 3 — GitHub release

**STOP until user confirms Step 3.**

Create the analyzer/extension release (this creates/pushes tag `vX.Y.Z` and triggers publish CI):

- Tag: `vX.Y.Z`
- Title: `vX.Y.Z` (same as tag, **with** `v` prefix)
- Notes: body of `## [X.Y.Z]` from root `CHANGELOG.md` (sections under that header, not the heading line alone)
- Target: squash commit on `main` (or `main` after pull)

Example:

```bash
gh release create "vX.Y.Z" --title "vX.Y.Z" --notes "..." --target "<sha-or-main>"
```

**STOP.** Wait for Step 4 confirmation (or user declines CLI tagging).

## Step 4 — Optional CLI tag

**STOP until user opts in.** If they decline, the release workflow ends here.

If yes (CLI version required — ask if missing):

1. `git checkout main && git pull origin main`
2. Tag `cli-vA.B.C` on a real commit (same commit as `vX.Y.Z` unless the user specifies otherwise)
3. `git push origin cli-vA.B.C`
4. Verify both `v*` and `cli-v*` resolve to commits (`git rev-parse`, `git ls-remote`)

Do not amend published tags.

## Common Mistakes

| Mistake | Fix |
|---------|------|
| Title `4.17.0` without `v` | Title matches tag: `v4.17.0` |
| Tag CLI because version was in the prompt | Step 4 is opt-in; still ask |
| Merge + release + CLI in one go | Four separate STOPs |
| Soft “ask later” while editing/pushing | Confirm **before** push/PR, merge, `gh release create`, CLI tag push |
| `generate_all.ps1` in bump PR | Out of scope unless the user asks |
| Same tag for CLI and analyzers | `v*` vs `cli-v*` |
| Amend published tag | New tag instead |
| Hardcode version in props | GitVersion + tags |
