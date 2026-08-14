# Release and CI Reference

## GitVersion (`GitVersion.yml`)

- Version not hardcoded in `Directory.Build.props` — CI sets from SemVer
- `main` → `beta` prerelease
- PRs → `alpha` prerelease

## Tags

| Stream | Pattern | Example |
|--------|---------|---------|
| Analyzers, extensions | `v*` | `v4.17.0` |
| CLI | `cli-v*` | `cli-v0.14.0` |

```bash
git tag v4.17.0 && git push origin v4.17.0
```

Do not amend published tags.

## `generate_all.ps1` (from `tools/`)

1. `generate_code.ps1`
2. `generate_configuration_file.ps1`
3. `generate_metadata.ps1` → `tools/build/`
4. `generate_cli_docs.ps1`
5. `generate_ref_docs.ps1`

Most doc output lands in docs site repo, not roslynator.

## CI verify (`.github/workflows/build.yml`)

```bash
cd src && dotnet restore Roslynator.sln
cd src && dotnet build Roslynator.sln --no-restore
cd src && dotnet format Roslynator.sln --no-restore --verify-no-changes --severity info
cd src && dotnet test Roslynator.sln --no-build
```

## Docs site pins (`josefpihrt.github.io/.github/workflows/build.yml`)

```yaml
RoslynatorRef: v4.17.0
RoslynatorCliRef: cli-v0.14.0
```

Controls CI checkout for MetadataGenerator, CLI docs, API ref, `configuration.md`.

## Pack (reference)

Dual Roslyn (`roslyn3.8`, `roslyn4.7`) NuGet packs for analyzer/refactoring/codefix projects. VSIX via msbuild on Windows in `src/VisualStudio`.

Docs site deploys on `v*` tag push to docs repo.
