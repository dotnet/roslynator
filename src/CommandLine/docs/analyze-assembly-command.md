
# `analyze-assembly` Command

Searches file or directory for analyzer assemblies.

## Synopsis

```
roslynator analyze-assembly <PATH>
[--additional-paths]
[--language]
[--file-log]
[--file-log-verbosity]
[--no-analyzers]
[--no-fixers]
[-v|--verbosity]
```

## Arguments

**`PATH`**

The path to file or directory to analyze.

### Optional Options

**`--additional-paths`** <ASSEMBLY_PATH> <DIRECTORY_PATH>

Defines additional paths to search.

**`--language`** `{cs[harp]|v[isual-]b[asic])}`

Defines project language.

**`--no-analyzers`**

Indicates whether to search for DiagnosticAnalyzers.

**`--no-fixers`**

Indicates whether to search for CodeFixProviders.

**`-v|--verbosity`** `{q[uiet]|m[inimal]|n[ormal]|d[etailed]|diag[nostic]}`

Defines the amount of information to display in the log.

## See Also

* [Roslynator Command-Line Interface](README.md)
