
# `fix` Command

Fixes all diagnostics in the specified solution.

## Synopsis

```
roslynator fix <SOLUTION>
[-a|--analyzer-assemblies]
[--batch-size]
[--ignore-analyzer-references]
[--ignore-compiler-errors]
[--ignored-compiler-diagnostics]
[--ignored-diagnostics]
[--ignored-projects]
[--msbuild-path]
[-p|--properties]
```

## Arguments

**`SOLUTION`**

The solution file to fix.

### Optional Options

**`a-|--analyzer-assemblies`**

Defines one or more paths to:

* analyzer assembly
* directory that should be searched recursively for analyzer assemblies

**`--batch-size`**

Defines maximum number of diagnostics that can be fixed in one batch.

**`--ignore-analyzer-references`**

Indicates whether Roslynator should ignore analyzers that are referenced in projects.

**`--ignore-compiler-errors`**

Indicates whether fixing should continue even if compilation has errors.

**`--ignored-compiler-diagnostics`**

Defines compiler diagnostic identifiers that should be ignored even if `--ignore-compiler-errors` is not set.

**`--ignored-diagnostics`**

Defines diagnostic identifiers that should not be fixed.

**`--ignored-projects`**

Defines project names that should not be fixed.

**`--msbuild-path`**

Defines a path to MSBuild.

*Note: If the path to MSBuild is not specified and there are installed multiple instances of MSBuild the instance with the highest version will be used.*

**`-p|--properties`** `<NAME=VALUE>`

Defines one or more MSBuild properties.

## See Also

* [Roslynator Command-Line Interface](README.md)
