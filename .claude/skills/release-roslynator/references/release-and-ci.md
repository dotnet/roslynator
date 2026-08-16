# Release and CI Reference

Supporting facts for `release-roslynator`. The gated release ritual lives in SKILL.md — this file is CI/GitVersion context only.

## GitVersion (`GitVersion.yml`)

- Version not hardcoded in `Directory.Build.props` — CI sets from SemVer
- `main` → `beta` prerelease
- PRs → `alpha` prerelease

## Tags

| Stream | Pattern | Example | How it is created |
|--------|---------|---------|-------------------|
| Analyzers, extensions | `v*` | `v4.17.0` | GitHub release (Step 3) |
| CLI | `cli-v*` | `cli-v0.14.0` | Optional git tag push (Step 4) |

```bash
# CLI only — analyzer tag normally comes from gh release create
git tag cli-v0.14.0 <sha> && git push origin cli-v0.14.0
```

Both tags must point at real commits (often the same bump commit). Do not amend published tags.

## Tag-triggered publish (`.github/workflows/build.yml`)

| Tag | Publishes |
|-----|-----------|
| `v*` | Analyzer/refactoring/code-fix NuGets, VS Code / Open VSX / VS VSIX |
| `cli-v*` | CLI NuGet packages |

## CI verify (optional local check — not a release gate)

```bash
cd src && dotnet restore Roslynator.sln
cd src && dotnet build Roslynator.sln --no-restore
cd src && dotnet format Roslynator.sln --no-restore --verify-no-changes --severity warn
cd src && dotnet test Roslynator.sln --no-build
```

## `generate_all.ps1` (from `tools/`) — not part of the bump PR

Use only if the user asks or codegen is clearly drifted. Default release bump touches changelogs only.

1. `generate_code.ps1`
2. `generate_configuration_file.ps1`
3. `generate_metadata.ps1` → `tools/build/`
4. `generate_cli_docs.ps1`
5. `generate_ref_docs.ps1`

## Pack (reference)

Dual Roslyn (`roslyn3.8`, `roslyn4.7`) NuGet packs for analyzer/refactoring/codefix projects. VSIX via msbuild on Windows in `src/VisualStudio`.
