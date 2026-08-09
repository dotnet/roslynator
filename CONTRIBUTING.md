# Contributing to Roslynator

Guidelines for contributing to the Roslynator repo.

## Agent Skills

Contributor workflows for AI agents (Cursor, Claude Code) live in [.claude/skills/](.claude/skills/) — adding analyzers, refactorings, compiler fixes, bug fixes, deprecation, and releases.

## Submitting Pull Requests

* **DO** submit issues for bug fixes or features.
* **DO** add unit tests for bug fixes or features.
* **DO** ensure submissions pass build and are merge conflict free.
* **DO** update changelog.
* **DO NOT** submit new analyzer/refactoring/fix without discussing it first.
* **DO NOT** submit large formatting/documentation changes without discussing it first.

## Creating Issues

* **DO** create a new issue rather than commenting on a closed issue.
* **DO** include the analyzer, refactoring, or error ID in the title (for example, `RCSxxxx`, `RRxxxx`, or `CSxxxx`).
* **DO** use a descriptive title that identifies the issue or requested feature.
* **DO** specify a detailed description of the issue or requested feature.
* **DO** provide the following for bug reports:
  * Describe the expected behavior and the actual behavior.
  * Provide example code that reproduces the issue.
  * Provide any relevant exception messages and stack traces.

## Coding Style

* **DO** follow [.NET Runtime Coding Style](https://github.com/dotnet/runtime/blob/main/docs/coding-guidelines/coding-style.md) (except using `s_` and `t_` prefix for field names).
* **DO** install the Roslynator extension for Visual Studio or VS Code and follow its suggestions.
