---
name: release-roslynator
description: Use when preparing a roslynator release, creating v* or cli-v* version tags, rolling ChangeLog.md [Unreleased], running generate_all.ps1 (not just generate_code), or bumping RoslynatorRef in josefpihrt.github.io CI.
---

# Release Roslynator

## Overview

GitVersion derives versions from tags. Separate streams: `v*` (analyzers/extensions) and `cli-v*` (CLI). After tag, bump docs site CI pins.

## When to Use

- Shipping a new analyzer/extension version
- CLI-only release (`cli-v*`)
- Rolling `## [Unreleased]` into a versioned changelog section
- Post-release docs alignment (`RoslynatorRef` / `RoslynatorCliRef`)

**Not for:** day-to-day contributor workflows; docs publication details → `update-roslynator-docs` in docs repo.

## Quick Reference

| Step | Action |
|------|--------|
| Changelog | Roll `ChangeLog.md` `[Unreleased]` → `[x.y.z] - date` |
| Generate | `cd tools && pwsh ./generate_all.ps1` |
| Verify | See [references/release-and-ci.md](references/release-and-ci.md) CI verify block |
| Tag | `v4.x.y` or `cli-v0.x.y` |
| Docs pins | `RoslynatorRef` / `RoslynatorCliRef` in docs site workflow |

Details: [references/release-and-ci.md](references/release-and-ci.md).

## Common Mistakes

| Mistake | Fix |
|---------|-----|
| Only `generate_code.ps1` at release | Use `generate_all.ps1` — includes metadata, CLI docs, API ref |
| Hardcode version in props | GitVersion + tag drives CI version |
| Same tag for CLI and analyzers | Separate `v*` and `cli-v*` streams |
| Skip docs pin bump in docs site repo | Update `RoslynatorRef` / `RoslynatorCliRef` in workflow |
| `CHANGELOG.md` | `ChangeLog.md` at repo root |
| Amend published tag | Create new tag instead |
