# Deprecation Mechanisms

## Analyzers — use `<Status>`, not `IsObsolete`

```xml
<Status>Obsolete</Status>
<ObsoleteMessage>Use RCS0061 instead</ObsoleteMessage>
```

Default when a successor exists. `Status=Disabled` only when retiring with no replacement (burns id, strips from docs/EditorConfig, internal descriptor with error `[Obsolete]`).

Do **not** use `IsObsolete="true"` on analyzers — `AnalyzerMetadata.IsObsolete` is error-level obsolete.

## Refactorings and code fixes — use `IsObsolete="true"`

```xml
<Refactoring Id="RR0006" ... IsObsolete="true">
<CodeFix Id="RCF0001" ... IsObsolete="true">
```

Codegen excludes obsolete entries from descriptors and options; identifiers keep `[Obsolete]`.

## Not deprecation

`IsEnabledByDefault=false` — active, documented, opt-in only.

## After metadata change

```bash
cd tools && pwsh ./generate_code.ps1
```

Remove implementation, registration, and `*Tests.cs`.

**Keep the XML entry** for obsolete rules:

- Analyzer: `Status=Obsolete` — descriptor and docs must remain
- Refactoring / RCF: `IsObsolete="true"` — same; codegen keeps identifiers with `[Obsolete]`

Only remove the entire `<Analyzer>` block when using `Status=Disabled` to burn an id. Refactorings and compiler fixes have no `Status=Disabled`; obsolete entries stay in XML with `IsObsolete="true"`.

Changelog:

```markdown
### Changed

- Mark analyzer [RCS0014](...) as obsolete; use [RCS0061](...) instead ([PR](...))
```
