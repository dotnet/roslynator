# Agent instructions

## Git workflow

- Never push or commit directly to `main` (or `master`).
- Create a feature branch, commit there, and open a PR.
- Do not push to remote unless the user asks.
- Do not force-push to `main` or rewrite published history unless the user explicitly requests it.
- Avoid `--force` / `--force-with-lease` on shared branches unless the user explicitly requests it.
- Do not create commits unless the user asks.
- Do not skip git hooks (`--no-verify`) unless the user asks.
- Do not merge your own PR unless the user explicitly asks.
- When a PR resolves a GitHub issue, include `Fixes #NNNN` (or `Closes #NNNN`) on its own line in the PR body. A markdown link alone does not create a GitHub issue reference or auto-close the issue.

## This repository

- Follow [CONTRIBUTING.md](CONTRIBUTING.md).
- Use [.claude/skills/](.claude/skills/) for analyzer, refactoring, compiler-fix, bug-fix, deprecation, and release workflows.
- Do not implement a new analyzer, refactoring, or compiler fix without an approved GitHub issue.
