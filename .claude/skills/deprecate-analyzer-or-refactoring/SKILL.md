---
name: deprecate-analyzer-or-refactoring
description: Use when obsoleting or retiring RCS####, RR####, or RCF#### in roslynator, replacing an analyzer with another id, or when tempted to use IsObsolete on an Analyzer entry — analyzers use Status Obsolete, not IsObsolete.
---

# Deprecate Analyzer or Refactoring

## Overview

Deprecation mechanism differs by artifact type. Wrong choice causes compile errors.

## When to Use

- Superseding an analyzer with another (`ObsoleteMessage` naming successor)
- Retiring a rule with no replacement (`Status=Disabled` for analyzers)
- Marking refactoring or compiler fix obsolete (`IsObsolete="true"`)

**Not for:** opt-in analyzers (`IsEnabledByDefault=false` is not deprecation).

## Quick Reference

| Artifact | Mechanism |
|----------|-----------|
| Analyzer (default) | `<Status>Obsolete</Status>` + `<ObsoleteMessage>` |
| Analyzer (retire) | `<Status>Disabled</Status>` — burns id |
| Refactoring / RCF | `IsObsolete="true"` on XML element |

Full detail: [references/mechanisms.md](references/mechanisms.md).

## Implementation

1. Update XML (mechanism per artifact — [references/mechanisms.md](references/mechanisms.md))
2. `cd tools && pwsh ./generate_code.ps1`
3. Remove implementation, registration, and `*Tests.cs`
4. **Keep** the obsolete XML entry — analyzers (`Status=Obsolete`), refactorings/RCF (`IsObsolete="true"`). Delete the whole `<Analyzer>` block only when retiring with `Status=Disabled`
5. `dotnet build` + format verify
6. `ChangeLog.md` under `### Changed`

## Common Mistakes

| Mistake | Fix |
|---------|-----|
| `IsObsolete="true"` on `<Analyzer>` | Use `<Status>Obsolete</Status>` + `<ObsoleteMessage>` — compile error otherwise |
| Delete entire `<Analyzer>` block when superseding | Keep obsolete entry; remove implementation/tests only |
| `Status=Disabled` with a successor | Use `Obsolete` + `ObsoleteMessage` |
| Confuse with `IsEnabledByDefault=false` | Opt-in is not deprecation |
| Skip codegen after XML change | Descriptors/options pages stay stale |
