---
name: add-refactoring
description: Use when adding a new RR#### refactoring in roslynator, editing Refactorings.xml, registering in RefactoringContext, or when refactorings-testing.md shows XunitRefactoringVerifier or how-to says CHANGELOG.md — in-repo uses AbstractCSharpRefactoringVerifier and ChangeLog.md.
---

# Add Refactoring

## Overview

Refactorings are metadata-driven: `Refactorings.xml` → codegen → register in `RefactoringContext` → implement → test → changelog.

## When to Use

- New `RR####` refactoring in an approved issue
- Registering a refactoring action on a syntax kind
- `<OptionKey>` required on every refactoring entry

**Not for:** analyzers (`add-analyzer`), compiler fixes (`add-compiler-diagnostic-fix`), deprecation (`deprecate-analyzer-or-refactoring`).

**Gate:** approved GitHub issue (CONTRIBUTING.md).

Read [references/implementation.md](references/implementation.md) before writing tests — public [refactorings-testing.md](https://josefpihrt.github.io/docs/roslynator/refactorings-testing) uses `XunitRefactoringVerifier`, which does not match in-repo tests.

## Quick Reference

| Step | Location / command |
|------|-------------------|
| Metadata | `src/Refactorings.xml` |
| Codegen | `cd tools && pwsh ./generate_code.ps1` |
| Register | `RefactoringContext.cs` or helper under `Refactorings/CSharp/Refactorings/` |
| Implement | same folder |
| Tests | `src/Tests/Refactorings.Tests/RR####IdentifierTests.cs` |
| Changelog | `ChangeLog.md` under `## [Unreleased]` |

## Implementation

Details and examples: [references/implementation.md](references/implementation.md).

Changelog:

```markdown
- Add refactoring "TITLE" ([RR####](https://josefpihrt.github.io/docs/roslynator/refactorings/RR####)) ([#PR](https://github.com/dotnet/roslynator/pull/PR))
```

Verify:

```bash
cd tools && pwsh ./generate_code.ps1
cd src && dotnet build Roslynator.sln
cd src && dotnet test Tests/Refactorings.Tests --no-build --filter "FullyQualifiedName~RR####"
cd src && dotnet format Roslynator.sln --no-restore --verify-no-changes --severity warn
```

## Common Mistakes

| Mistake | Fix |
|---------|-----|
| Follow `refactorings-testing.md` verbatim | In-repo: `AbstractCSharpRefactoringVerifier` + `RefactoringId` override |
| Missing `<OptionKey>` | Required — EditorConfig id for enable/disable |
| `CHANGELOG.md` in how-to | File is `ChangeLog.md` at repo root |
| Codegen from repo root | `cd tools && pwsh ./generate_code.ps1` |
| `[|...|]` for diagnostics | Selection span for refactorings |
| Only `<Syntaxes>` / `<Span>` | Documentation-only; registration is in code |
