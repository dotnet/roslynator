
# `analyze` Command

Analyzes specified project or solution and reports diagnostics.

## Synopsis

```
roslynator analyze <PROJECT|SOLUTION>
[-a|--analyzer-assemblies]
[--culture]
[--execution-time]
[--ignore-analyzer-references]
[--ignore-compiler-diagnostics]
[--ignored-diagnostics]
[--ignored-projects]
[--language]
[--file-log]
[--file-log-verbosity]
[--msbuild-path]
[--output]
[--projects]
[-p|--properties]
[--report-not-configurable]
[--report-suppressed-diagnostics]
[--severity-level]
[--supported-diagnostics]
[-v|--verbosity]
```

## Arguments

**`PROJECT|SOLUTION`**

The project or solution to analyze.

### Optional Options

**`a-|--analyzer-assemblies`** <PATH>

Defines one or more paths to:

* analyzer assembly
* directory that should be searched recursively for analyzer assemblies

**`--culture`** <CULTURE_ID>

Defines culture that should be used to display diagnostic message.

**`--execution-time`**

Indicates whether to measure execution time of each analyzer.

**`--ignore-analyzer-references`**

Indicates whether Roslynator should ignore analyzers that are referenced in projects.

**`--ignore-compiler-diagnostics`**

Indicates whether to display compiler diagnostics.

**`--ignored-diagnostics`** <DIAGNOSTIC_ID>

Defines diagnostics that should not be reported.

**`--ignored-projects`** <PROJECT_NAME>

Defines projects that should not be analyzed.

**`--language`** `{cs[harp]|v[isual-]b[asic])}`

Defines project language.

**`--msbuild-path`** <MSBUILD_PATH>

Defines a path to MSBuild. This option must be specified if there are multiple locations of MSBuild (usually multiple installations of Visual Studio).

**`--output`** <OUTPUT_FILE>

Defines path to file that will store reported diagnostics in XML format.

**`--projects`** <PROJECT_NAME>

Defines projects that should be analyzed.

**`-p|--properties`** `<NAME=VALUE>`

Defines one or more MSBuild properties.

**`--report-not-configurable`**

Indicates whether diagnostics that have tag '[NotConfigurable](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.wellknowndiagnostictags.notconfigurable?view=roslyn-dotnet)' should be reported.

**`--report-suppressed-diagnostics`**

Indicates whether suppressed diagnostics should be reported.

**`--severity-level`** `{hidden|info|warning|error}`

Defines minimally required severity for a diagnostic. Default value is `info`.

**`--supported-diagnostics`** <DIAGNOSTIC_ID>

Defines diagnostics that should be reported.

**`-v|--verbosity`** `{q[uiet]|m[inimal]|n[ormal]|d[etailed]|diag[nostic]}`

Defines the amount of information to display in the log.

## See Also

* [Roslynator Command-Line Interface](README.md)
