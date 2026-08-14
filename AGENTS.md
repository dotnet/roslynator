# Agent instructions

## Git workflow

- Never push or commit directly to `main` (or `master`).
- Create a feature branch, commit there, and open a PR.
- Do not push to remote unless the user asks.
- Do not force-push to `main`, rewrite published history, or use `--force` / `--force-with-lease` on shared branches unless the user explicitly requests it.
- Do not create commits unless the user asks.
- Do not skip git hooks (`--no-verify`) unless the user asks.
- Do not merge a PR unless the user explicitly asks.
- When a PR resolves a GitHub issue, include `Fixes #NNNN` (or `Closes #NNNN`) on its own line in the PR body. A markdown link alone does not create a GitHub issue reference or auto-close the issue.

## This repository

- Follow [CONTRIBUTING.md](CONTRIBUTING.md).
- New `RCS` / `RR` / `RCF` work requires an approved GitHub issue (see CONTRIBUTING.md).
