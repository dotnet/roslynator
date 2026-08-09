---
name: fix-analyzer-bug
description: Use when fixing a Roslynator analyzer false positive or false negative, RCS####, RR####, or CS####/RCF#### regression, incorrect diagnostic reporting, or bug report citing a specific rule id — not when adding new rules.
---

# Fix Analyzer Bug

## Overview

Reproduce in an existing test file, minimal code fix, changelog entry. Highest-frequency contributor task.

## When to Use

- Analyzer reports when it should not (false positive)
- Analyzer misses case it should report (false negative)
- Code fix produces wrong transformation
- Issue title includes `RCS####`, `RR####`, `CS####`, or `RCF####`

**Not for:** new rules (`add-analyzer`, `add-refactoring`, `add-compiler-diagnostic-fix`), deprecation (`deprecate-analyzer-or-refactoring`).

For test patterns use in-repo verifiers — not public NuGet testing docs.

## Quick Reference

| Prefix | Code | Tests | Verifier |
|--------|------|-------|----------|
| RCS1 | `Analyzers/CSharp/Analysis/` | `Tests/Analyzers.Tests/` | `AbstractCSharpDiagnosticVerifier` |
| RCS0 | `Formatting.Analyzers/CSharp/` | `Tests/Formatting.Analyzers.Tests/` | `AbstractCSharpDiagnosticVerifier` |
| RCS9 | `CodeAnalysis.Analyzers/CSharp/` | `Tests/CodeAnalysis.Analyzers.Tests/` | `AbstractCSharpDiagnosticVerifier` |
| RR | `Refactorings/CSharp/Refactorings/` | `Tests/Refactorings.Tests/` | `AbstractCSharpRefactoringVerifier` |
| CS / RCF | `CodeFixes/CSharp/CodeFixes/` | `Tests/CodeFixes.Tests/` | `AbstractCSharpCompilerDiagnosticFixVerifier` |

Search `DiagnosticIdentifiers.X` / `DiagnosticRules.X` / `CompilerDiagnosticIdentifiers.*` in generated files. Roslynator analyzer code fixes live in matching `.CodeFixes` projects.

## Implementation

Add `[Fact]` to existing `*Tests.cs` — new file only if none exists.

| Case | Method |
|------|--------|
| False positive | `VerifyNoDiagnosticAsync` |
| False negative | `VerifyDiagnosticAsync` with `[|...|]` |
| Analyzer/refactoring fix regression | `VerifyDiagnosticAndFixAsync` |
| Compiler fix regression | `VerifyFixAsync` (no `[|...|]` markers) |

Config-dependent: `options: Options.AddConfigOption(...)`.

Verify (adjust project and filter):

```bash
cd src && dotnet build Roslynator.sln
cd src && dotnet test Tests/Analyzers.Tests --no-build --filter "FullyQualifiedName~RCS####"
cd src && dotnet test Tests/CodeFixes.Tests --no-build --filter "FullyQualifiedName~CS####"
cd src && dotnet format Roslynator.sln --no-restore --verify-no-changes --severity info
```

Changelog examples: [references/changelog-examples.md](references/changelog-examples.md).

## Common Mistakes

| Mistake | Fix |
|---------|-----|
| Copy test pattern from `analyzers-testing.md` | In-repo `AbstractCSharpDiagnosticVerifier` + `DiagnosticRules.X` |
| Copy from `compiler-diagnostic-fixes-testing.md` | In-repo `AbstractCSharpCompilerDiagnosticFixVerifier` + `CompilerDiagnosticIdentifiers.*` |
| New test file per case | Extend existing `RCS####...Tests.cs` or `CS####...Tests.cs` |
| Change `Analyzers.xml` for logic bugs | Metadata only for docs/severity defaults |
| `[|...|]` in compiler fix tests | Compiler diagnostic location is implicit |
| Full test suite | Filter by rule id is enough |
| Skip test | CONTRIBUTING.md requires tests for bug fixes |
| `CHANGELOG.md` | `ChangeLog.md` at repo root |
