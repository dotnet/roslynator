# Changelog Examples

File: `ChangeLog.md` at repo root (not `CHANGELOG.md`).

## Analyzer fix

```markdown
### Fixed

- Fix analyzer [RCS1046](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1046) to report `async void` methods without `Async` suffix ([PR](https://github.com/dotnet/roslynator/pull/1790))
- Fix analyzer [RCS1265](https://josefpihrt.github.io/docs/roslynator/analyzers/RCS1265) to not report catch clauses with a `when` filter ([PR](https://github.com/dotnet/roslynator/pull/1789))
```

## Refactoring fix

Link `refactorings/RR####` instead of `analyzers/RCS####`.

## Compiler diagnostic fix

```markdown
### Fixed

- Fix code fix for CS0165 to initialize unassigned locals correctly ([PR](https://github.com/dotnet/roslynator/pull/1234))
```

## CLI fix

Prefix with `[CLI]`.

## Patterns

- Link id to docs URL
- Plain-language behavior change
- Backticks for code identifiers
- PR link when known
