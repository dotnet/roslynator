---
name: add-compiler-diagnostic-fix
description: Use when adding a Roslynator code fix for C# compiler error CS#### or RCF####, editing Diagnostics.xml or CodeFixes.xml, or when compiler-diagnostic-fixes-testing.md shows XunitCompilerDiagnosticFixVerifier or bare DiagnosticId like "CS0106" — in-repo uses AbstractCSharpCompilerDiagnosticFixVerifier and CompilerDiagnosticIdentifiers. Contributor how-to exists but testing page targets NuGet consumers.
---

# Add Compiler Diagnostic Fix

## Overview

Compiler fixes use `Diagnostics.xml` + `CodeFixes.xml` → codegen → `CompilerDiagnosticCodeFixProvider` → test. Contributor how-to: [how-to-add-compiler-diagnostic-fix](https://josefpihrt.github.io/docs/roslynator/how-to-add-compiler-diagnostic-fix). Do not follow [compiler-diagnostic-fixes-testing](https://josefpihrt.github.io/docs/roslynator/compiler-diagnostic-fixes-testing) alone — it targets NuGet consumers.

## When to Use

- New fix for `CS####` compiler diagnostic
- New `RCF####` entry linking one or more `CS` ids
- Extending fix coverage for an existing compiler error

**Not for:** Roslynator analyzers (`add-analyzer`), refactorings (`add-refactoring`).

**Gate:** approved GitHub issue (CONTRIBUTING.md).

Read [references/implementation.md](references/implementation.md) before writing tests.

## Quick Reference

| Step | Location |
|------|----------|
| Diagnostic catalog | `src/Diagnostics.xml` |
| Fix metadata | `src/CodeFixes.xml` |
| Codegen | `cd tools && pwsh ./generate_code.ps1` |
| Provider | `src/CodeFixes/CSharp/CodeFixes/` |
| Tests | `src/Tests/CodeFixes.Tests/CS####IdentifierTests.cs` |
| Changelog | `ChangeLog.md` |

## Implementation

Full XML, provider, and test patterns: [references/implementation.md](references/implementation.md).

Changelog:

```markdown
- Add code fix "TITLE" for CS#### ([#PR](https://github.com/dotnet/roslynator/pull/PR))
```

Verify:

```bash
cd tools && pwsh ./generate_code.ps1
cd src && dotnet build Roslynator.sln
cd src && dotnet test Tests/CodeFixes.Tests --no-build --filter "FullyQualifiedName~CS####"
cd src && dotnet format Roslynator.sln --no-restore --verify-no-changes --severity warn
```

## Common Mistakes

| Mistake | Fix |
|---------|-----|
| Follow `compiler-diagnostic-fixes-testing.md` | In-repo: `AbstractCSharpCompilerDiagnosticFixVerifier` |
| `DiagnosticId = "CS0106"` literal | Use `CompilerDiagnosticIdentifiers.CS####_Identifier` constant |
| `[|...|]` in compiler fix tests | Compiler diagnostic location is implicit |
| Skip `IsEnabled` in provider | Required for EditorConfig / global compiler-fix toggles |
| Only read testing doc | Also need `Diagnostics.xml`, `CodeFixes.xml`, codegen, provider base |
| Codegen from repo root | `cd tools && pwsh ./generate_code.ps1` |
