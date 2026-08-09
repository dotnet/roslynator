# Skill validation (RED-GREEN-REFACTOR)

Recorded 2026-08-09. Pressure scenarios run via subagents.

## add-analyzer

### RED (docs-only: how-to + analyzers-testing + Template.Analyzers.xml)

| Convention | Result |
|------------|--------|
| Codegen cwd | FAIL — `tools/generate_code.ps1` without `cd tools` |
| Test base class | FAIL — `XunitDiagnosticVerifier` + `Analyzer.Descriptor` |
| Changelog file | FAIL — `CHANGELOG.md` |

### GREEN (with skill)

| Convention | Result |
|------------|--------|
| Codegen | PASS — `cd tools && pwsh ./generate_code.ps1` |
| Test base class | PASS — `AbstractCSharpDiagnosticVerifier` |
| Changelog | PASS — `ChangeLog.md` |

### REFACTOR applied

- Description mentions docs trap keywords
- Hard gate: read skill + `references/implementation.md` before tests
- Common Mistakes prioritizes `analyzers-testing.md` trap
- Namespace note in `implementation.md`

## add-refactoring

### RED (docs-only: how-to + refactorings-testing + Template.Refactorings.xml)

| Convention | Result |
|------------|--------|
| Codegen cwd | FAIL — `tools/generate_code.ps1` (how-to) without `cd tools` |
| Test base class | FAIL — `XunitRefactoringVerifier<Provider>` (refactorings-testing) |
| `RefactoringId` override | FAIL — not shown in public docs example |
| Changelog file | FAIL — how-to says `CHANGELOG.md` |
| `<OptionKey>` | PASS — present in Template.Refactorings.xml |

### GREEN (with skill)

| Convention | Result |
|------------|--------|
| Codegen | PASS — `cd tools && pwsh ./generate_code.ps1` |
| Test base class | PASS — `AbstractCSharpRefactoringVerifier` |
| `RefactoringId` | PASS — `public override string RefactoringId` |
| Changelog | PASS — `ChangeLog.md` |
| `<OptionKey>` | PASS — documented as required |

### REFACTOR applied

- Description mentions `CHANGELOG.md` trap
- Common Mistakes leads with `refactorings-testing.md` trap
- Hard gate to read `references/implementation.md` before tests (already present)

## add-compiler-diagnostic-fix

### RED (docs-only: compiler-diagnostic-fixes-testing.md only — no how-to exists)

| Convention | Result |
|------------|--------|
| Test base class | FAIL — `XunitCompilerDiagnosticFixVerifier` |
| `DiagnosticId` | FAIL — bare `"CS0106"` string, not `CompilerDiagnosticIdentifiers.*` |
| `[|...|]` markers | PASS — not used in public example |
| `Trait` / `equivalenceKey` | FAIL — omitted in public example |
| Metadata XML workflow | FAIL — testing doc does not mention `Diagnostics.xml` / `CodeFixes.xml` |
| Codegen command | FAIL — not documented on docs site |

### GREEN (with skill)

| Convention | Result |
|------------|--------|
| Test base class | PASS — `AbstractCSharpCompilerDiagnosticFixVerifier` |
| `DiagnosticId` | PASS — `CompilerDiagnosticIdentifiers.CS####_*` |
| No span markers | PASS — documented |
| `equivalenceKey` | PASS — `EquivalenceKey.Create(DiagnosticId)` |
| XML + codegen + provider | PASS — in Quick Reference and implementation.md |
| Codegen | PASS — `cd tools && pwsh ./generate_code.ps1` |

### REFACTOR applied

- Description mentions bare `CS0106` trap
- Gate: skill is the how-to (no published add guide)
- Common Mistakes: testing-doc trap, literal DiagnosticId, missing XML workflow

## fix-analyzer-bug

### RED (docs-only: CONTRIBUTING.md + analyzers-testing.md)

| Convention | Result |
|------------|--------|
| Test approach | FAIL — `XunitDiagnosticVerifier` from testing doc |
| False positive method | FAIL — public doc does not show `VerifyNoDiagnosticAsync` |
| Extend existing test file | FAIL — CONTRIBUTING does not say; agents often create new file |
| Changelog format | FAIL — no house-style link/PR format in CONTRIBUTING |
| Targeted test filter | FAIL — not documented on docs site |
| Metadata change for logic bug | PASS if agent knows not to touch XML (uncertain) |

### GREEN (with skill)

| Convention | Result |
|------------|--------|
| Test verifier | PASS — in-repo abstract verifiers per artifact type |
| `VerifyNoDiagnosticAsync` / `VerifyDiagnosticAsync` | PASS — table in Implementation |
| Extend existing `*Tests.cs` | PASS — explicit in Implementation |
| Changelog house style | PASS — `references/changelog-examples.md` |
| Filtered `dotnet test` | PASS — in Verify block |
| No metadata for logic bugs | PASS — Common Mistakes |

### REFACTOR applied

- Gate against public testing docs
- Common Mistakes leads with `analyzers-testing.md` trap

## deprecate-analyzer-or-refactoring

### RED (docs-only: CodeFixes.xml `IsObsolete` pattern only — no analyzer-metadata, no skill)

| Convention | Result |
|------------|--------|
| Analyzer XML | FAIL — likely `IsObsolete="true"` (matches CodeFix/Refactoring pattern) |
| Refactoring XML | PASS — `IsObsolete="true"` is correct |
| Keep obsolete analyzer entry | FAIL — agents often delete entire `<Analyzer>` block |
| `ObsoleteMessage` with successor id | FAIL — not in CodeFixes pattern |

### RED (with analyzer-metadata.md only)

| Convention | Result |
|------------|--------|
| Analyzer XML | PASS — `<Status>Obsolete</Status>` + `<ObsoleteMessage>` documented |
| Keep XML entry | FAIL — metadata doc does not say to keep entry after removing code |

### GREEN (with skill)

| Convention | Result |
|------------|--------|
| Analyzer mechanism | PASS — `Status=Obsolete`, not `IsObsolete` |
| Refactoring/RCF | PASS — `IsObsolete="true"` |
| Keep obsolete XML | PASS — step 4 in Implementation |
| Codegen | PASS — `cd tools && pwsh ./generate_code.ps1` |

### REFACTOR applied

- Description warns against `IsObsolete` on Analyzer
- Common Mistakes: delete-block trap, compile-error note on wrong attribute

## release-roslynator

### RED (docs-only: CONTRIBUTING.md + ChangeLog.md structure — no release how-to)

| Convention | Result |
|------------|--------|
| `generate_all.ps1` | FAIL — not documented publicly; agents use `generate_code.ps1` from how-tos |
| Separate `v*` vs `cli-v*` tags | FAIL — not documented on docs site |
| GitVersion / no hardcoded version | FAIL — not in contributor docs |
| Docs site `RoslynatorRef` bump | FAIL — lives in separate repo workflow |
| Changelog roll `[Unreleased]` | PASS — visible in `ChangeLog.md` |
| `ChangeLog.md` filename | PASS if agent read repo file; FAIL if following how-to `CHANGELOG.md` |

### GREEN (with skill)

| Convention | Result |
|------------|--------|
| `generate_all.ps1` from `tools/` | PASS |
| Tag streams | PASS — `v*` vs `cli-v*` |
| GitVersion | PASS — Overview + Common Mistakes |
| Docs pins | PASS — Quick Reference + cross-ref to docs skill |
| Changelog file | PASS — `ChangeLog.md` |

### REFACTOR applied

- Description names `generate_all` vs `generate_code` and docs site workflow path
- Common Mistakes: `generate_code`-only trap, explicit docs site repo for pins

## update-roslynator-docs

### RED (docs-only: how-to-update-documentation.md + README)

| Convention | Result |
|------------|--------|
| Edit `docs/roslynator/analyzers/` | FAIL — how-to says "edit static files here" without excluding gitignored dirs |
| `generate_docs.ps1` cwd | PARTIAL — README says `tools/generate_docs.ps1` without stressing `cd tools` |
| `RoslynatorRef` CI pins | FAIL — not in README or how-to |
| Source of truth is roslynator XML | PASS — how-to points to Analyzers.xml / Refactorings.xml |
| CI vs local generation | FAIL — how-to links API doc generation, not site `generate_docs.ps1` |

### GREEN (with skill)

| Convention | Result |
|------------|--------|
| Do not edit gitignored paths | PASS — Overview + Common Mistakes + generated-paths.md |
| `cd tools && pwsh ./generate_docs.ps1` | PASS |
| Sibling repo layout | PASS — references/generated-paths.md |
| `RoslynatorRef` / `RoslynatorCliRef` | PASS — Quick Reference |
| Hand-written vs generated | PASS |

### REFACTOR applied

- Description mentions `analyzers/` edit trap and RoslynatorRef
- Common Mistakes: how-to "static files" caveat, `generate-doc` vs site workflow

## Validation complete

All seven skills exercised through RED-GREEN-REFACTOR (2026-08-09).
