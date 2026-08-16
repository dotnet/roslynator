---
name: add-analyzer
description: Use when adding a new RCS#### diagnostic in roslynator (RCS0 formatting, RCS1 general, RCS9 code-analysis), wiring roslynator_* EditorConfig options, or when docs say CHANGELOG.md, XunitDiagnosticVerifier, or analyzers-testing.md — those are wrong for in-repo contribution.
---

# Add Analyzer

## Overview

Roslynator analyzers are metadata-driven: edit `Analyzers.xml`, codegen, implement analyzer + code fix, test, changelog. New analyzers never set `<Status>` — only `IsEnabledByDefault`.

## When to Use

- New `RCS0` / `RCS1` / `RCS9` rule requested in an approved issue
- Analyzer needs a new or existing `roslynator_*` config option
- Picking package: `RCS0` → Formatting, `RCS1` → Analyzers, `RCS9` → CodeAnalysis.Analyzers

**Not for:** deprecating rules (`deprecate-analyzer-or-refactoring`), bug fixes (`fix-analyzer-bug`), refactorings (`add-refactoring`).

**Gate:** CONTRIBUTING.md requires an approved GitHub issue before implementation.

**Read this skill and `references/implementation.md` before writing tests.** Published [analyzers-testing.md](https://josefpihrt.github.io/docs/roslynator/analyzers-testing) targets NuGet consumers — copying it produces code that does not compile in this repo.

## Confirm metadata parameters (hard gate)

**STOP. Do NOT edit `Analyzers.xml`, run codegen, or implement until the user has confirmed every required parameter below.** Do not invent defaults when the user (or issue) did not state them.

Use `AskQuestion` when available; otherwise ask conversationally. Batch related choices.

| Parameter | Required? | Allowed / notes |
|-----------|-----------|-----------------|
| `Id` | propose | Compute next free `RCS0` / `RCS1` / `RCS9` id from `Analyzers.xml`; **do not ask** unless the issue conflicts or multiple ids are plausible |
| `Identifier` | yes | PascalCase; drives generated names |
| `Title` | yes | Short description |
| `DefaultSeverity` | yes | `Hidden`, `Info`, `Warning`, or `Error` — **always ask** |
| `IsEnabledByDefault` | yes | `true` / `false` — **always ask** (RCS0 often `false`) |
| `MessageFormat` | if message has `{n}` placeholders | Otherwise omit (same as Title) |
| Code fix? | yes | Strongly recommended; confirm yes/no |
| `SupportsFadeOut` / fade-out analyzer | no | Ask only if unused-code style |
| `MinLanguageVersion` | no | Ask only if C# version-gated |
| Config option | no | See [config-options.md](references/config-options.md) |

When proposing `Id`, state the chosen value in your plan/summary (e.g. “using next free **RCS9012**”). Skip asking other parameters only when the approved issue or the user's message already states the value explicitly.

## Quick Reference

| Step | Location / command |
|------|-------------------|
| Metadata | `src/Analyzers.xml` — see [analyzer-schema.md](references/analyzer-schema.md) |
| Config option | `src/ConfigOptions.xml` — see [config-options.md](references/config-options.md) |
| Codegen | `cd tools && pwsh ./generate_code.ps1` |
| Analyzer | RCS1 → `Analyzers/CSharp/Analysis/`; RCS0 → `Formatting.Analyzers/CSharp/`; RCS9 → `CodeAnalysis.Analyzers/CSharp/` |
| Code fix | matching `*.CodeFixes/CSharp/CodeFixes/` |
| Tests | `src/Tests/<Package>.Tests/RCS####IdentifierTests.cs` |
| Changelog | `CHANGELOG.md` under `## [Unreleased]` |

## Implementation

1. Confirm metadata parameters (hard gate above).
2. Add `<Analyzer>` entry; schema in [references/analyzer-schema.md](references/analyzer-schema.md). Use docs-site [analyzer-metadata](https://josefpihrt.github.io/docs/roslynator/analyzer-metadata), not `Template.Analyzers.xml`.
3. Optional config option before codegen — [references/config-options.md](references/config-options.md).
4. Codegen from `tools/` (required cwd — see Common Mistakes).
5. Implement analyzer, code fix (if confirmed), tests — [references/implementation.md](references/implementation.md).

Changelog line:

```markdown
- Add analyzer "TITLE" ([RCS1234](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1234)) ([#PR](https://github.com/dotnet/roslynator/pull/PR))
```

Verify:

```bash
cd tools && pwsh ./generate_code.ps1
cd src && dotnet build Roslynator.sln
cd src && dotnet test Roslynator.sln --no-build --filter "FullyQualifiedName~RCS####"
cd src && dotnet format Roslynator.sln --no-restore --verify-no-changes --severity info
```

## Common Mistakes

| Mistake | Fix |
|---------|-----|
| Guess `DefaultSeverity` / `IsEnabledByDefault` | Ask — hard gate above |
| Follow `analyzers-testing.md` verbatim | In-repo: `AbstractCSharpDiagnosticVerifier` + `Descriptor = DiagnosticRules.X` |
| `pwsh tools/generate_code.ps1` from repo root | Run `cd tools && pwsh ./generate_code.ps1` — generator uses `../src` relative to cwd |
| `<Status>` on new analyzer | Use only `IsEnabledByDefault`; lifecycle is `deprecate-analyzer-or-refactoring` |
| `context.ReportDiagnostic` | Use `DiagnosticHelpers.ReportDiagnostic` |
| Lazy `SupportedDiagnostics` field initializer | Use `Immutable.InterlockedInitialize` pattern |
